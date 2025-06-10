using System;
using System.Windows.Forms;
using System.IO.Ports;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;

namespace Garry.Control4.Jailbreak.UI
{
    public partial class UartRecovery : UserControl
    {
        private readonly MainWindow _mainWindow;
        private ManagementEventWatcher _deviceWatcher;

        public UartRecovery(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            InitializeComponent();

            InitializeComboBox();
            InitializeDeviceWatcher();
        }

        private void InitializeComboBox()
        {
            // Populate an initial list of available COM ports
            UpdateAvailablePorts();
        }

        private void UpdateAvailablePorts()
        {
            // Get available ports
            comboBoxComPort.DataSource = SerialPort.GetPortNames();
        }

        private void InitializeDeviceWatcher()
        {
            // Create a query to listen for device arrival/removal
            var query = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent " +
                                          "WHERE EventType = 2 OR EventType = 3");
            _deviceWatcher = new ManagementEventWatcher(query);

            _deviceWatcher.EventArrived += DeviceChanged;
            _deviceWatcher.Start();
        }

        private void DeviceChanged(object sender, EventArrivedEventArgs e)
        {
            // Update the ComboBox when a device is added or removed
            Invoke(new Action(UpdateAvailablePorts));
        }

        private void UartRecoveryButton_Click(object sender, EventArgs e)
        {
            var log = new LogWindow(_mainWindow, "UART Recovery");

            if (!Recover(log))
            {
                return;
            }

            log.WriteHighlight("\nPassword authentication should now be temporarily enabled. You can now perform the jailbreak process as normal.\n");
        }

        private bool Recover(LogWindow log)
        {
            var selectedPort = comboBoxComPort.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(selectedPort))
            {
                log.WriteError("No COM port selected.");
                return false;
            }

            try
            {
                using (var serialPort = new SerialPort(selectedPort))
                {
                    // Configure the serial port
                    serialPort.BaudRate = 115200;
                    serialPort.Parity = Parity.None;
                    serialPort.StopBits = StopBits.One;
                    serialPort.DataBits = 8;
                    serialPort.Handshake = Handshake.None;
                    serialPort.Encoding = Encoding.ASCII;

                    // Step 1: Connect to the COM port
                    log.WriteNormal($"Connecting to port {selectedPort}... ");
                    try
                    {
                        serialPort.Open();
                    } 
                    catch (Exception)
                    {
                        log.WriteError("failed\n");
                        return false;
                    }
                    log.WriteSuccess("connected\n");


                    var succeeded = false;
                    foreach (var passwordFunc in new Func<string, string>[]{DirectorPatch.GetDirectorRootPassword,  _ => "t0talc0ntr0l4!" }) {
                        // Send Ctrl+D + newline in case we are already logged in and to wake up the device
                        serialPort.Write(new byte[] { 0x04, (byte)'\n' }, 0, 2);

                        // Step 2: Wait for a prompt
                        log.WriteNormal("Waiting for login prompt... ");
                        var (match, prompt) = WaitForPrompt(
                            serialPort,
                            new Regex(
                                @"^.+-(?<macAddress>[0-9a-f]{12})\s*login:\s*$",
                                RegexOptions.IgnoreCase | RegexOptions.Multiline
                            )
                        );
                        if (match == null)
                        {
                            log.WriteError("failed\n\n");
                            if (string.IsNullOrWhiteSpace(prompt))
                            {
                                log.WriteError(
                                    "No response from the device. Ensure the device is powered on and connected.\n");
                            }
                            else
                            {
                                log.WriteTrace("Console Output:\n" + prompt.Trim() + "\n");
                            }

                            return false;
                        }

                        var macAddress = match.Groups["macAddress"].Value;
                        log.WriteSuccess($"done - detected MAC {macAddress}\n");

                        // Step 3: Send username and wait for the password prompt
                        log.WriteNormal("Entering username root... ");
                        serialPort.WriteLine("root");
                        (match, prompt) = WaitForPrompt(serialPort, new Regex(@"^\s*password:\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline));
                        if (match == null)
                        {
                            log.WriteError("failed\n\n");
                            log.WriteTrace("Console Output:\n" + prompt.Trim() + "\n");
                            continue;
                        }
                        log.WriteSuccess("done\n");
                        
                        var password = passwordFunc(macAddress);

                        // Step 4: Enter password and wait for the shell prompt
                        log.WriteNormal($"Entering password {password}... ");
                        serialPort.WriteLine(password);
                        (match, prompt) = WaitForPrompt(serialPort, new Regex(@".*root@.*:~#\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline));
                        if (match == null)
                        {
                            log.WriteError("failed\n\n");
                            log.WriteTrace("Console Output:\n" + prompt.Trim() + "\n");
                            continue;
                        }
                        log.WriteSuccess("done\n");
                        succeeded = true;
                        break;
                    }
                    if (!succeeded)
                    {
                        return false;
                    }
                 
                    // Step 5: Re-enable password authentication in sshd_config
                    log.WriteNormal("Re-enabling password authentication in sshd_config... ");
                    serialPort.WriteLine("sed -i 's/PasswordAuthentication no/PasswordAuthentication yes/' /etc/ssh/sshd_config; echo $?");
                    var (match2, prompt2) = WaitForPrompt(serialPort, new Regex(@"^\s*0\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline));
                    if (match2 == null)
                    {
                        log.WriteError("failed\n\n");
                        log.WriteTrace("Console Output:\n" + prompt2.Trim() + "\n");
                        return false;
                    }
                    log.WriteSuccess("done\n");

                    // Step 6: Restart the SSH service
                    log.WriteNormal("Restarting SSH service... ");
                    serialPort.WriteLine("service sshd restart; echo $?");
                    (match2, prompt2) = WaitForPrompt(serialPort, new Regex(@"^\s*0\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline));
                    if (match2 == null)
                    {
                        log.WriteError("failed\n\n");
                        log.WriteTrace("Console Output:\n" + prompt2.Trim() + "\n");
                        return false;
                    }
                    log.WriteSuccess("done\n");
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.WriteError($"Error: {ex.Message}");
                return false;
            }
        }

        private static (Match, string) WaitForPrompt(SerialPort serialPort, Regex prompt, int timeout = 5000)
        {
            var buffer = new StringBuilder();
            var startTime = Environment.TickCount;

            while (Environment.TickCount - startTime < timeout)
            {
                try
                {
                    if (serialPort.BytesToRead <= 0)
                    {
                        continue;
                    }
                    buffer.Append(serialPort.ReadExisting());

                    var match = prompt.Match(buffer.ToString());
                    if (match.Success)
                    {
                        return (match, buffer.ToString());
                    }
                }
                catch (TimeoutException)
                {
                    break;
                }
            }

            return (null, buffer.ToString());
        }
    }
}