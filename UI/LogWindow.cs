using System;
using System.Drawing;
using System.Windows.Forms;

namespace Garry.Control4.Jailbreak.UI
{
    public partial class LogWindow : Form
    {
        public LogWindow(Form mainWindow, string title = "LogWindow")
        {
            Owner = mainWindow;
            InitializeComponent(title);

            CenterToParent();
            Show();
        }

        // All log writes funnel through here. The jailbreak now runs on a background
        // thread, so writes can arrive off the UI thread — marshal them back before
        // touching the textBox (WinForms controls are single-threaded).
        private void WriteColored(Color color, string v)
        {
            if (textBox.IsDisposed) return;
            if (textBox.InvokeRequired)
            {
                // The window can be closed mid-run now that the jailbreak is off-thread.
                try { textBox.Invoke((Action)(() => WriteColored(color, v))); }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
                return;
            }

            _progressLineStart = -1;
            textBox.SelectionColor = color;
            textBox.AppendText(v);

            textBox.ScrollToCaret();
            textBox.Refresh();
        }

        internal void WriteNormal(string v)
        {
            WriteColored(Color.Black, v);
        }

        internal void WriteSuccess(string v)
        {
            WriteColored(Color.Green, v);
        }

        // ReSharper disable once UnusedMember.Global
        internal void WriteWarning(string v)
        {
            WriteColored(Color.Orange, v);
        }

        internal void WriteError(Exception v)
        {
            WriteError($"\n{v.Message}\n");
            WriteNormal($"{v.StackTrace}\n");
        }

        internal void WriteError(string v)
        {
            WriteColored(Color.Red, v);
        }

        internal void WriteTrace(string v)
        {
            WriteColored(Color.Gray, v);
        }

        internal void WriteHighlight(string v)
        {
            WriteColored(Color.Blue, v);
        }

        private int _progressLineStart = -1;

        internal void WriteProgress(string v)
        {
            if (textBox.IsDisposed) return;
            if (textBox.InvokeRequired)
            {
                try { textBox.Invoke((Action)(() => WriteProgress(v))); }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
                return;
            }

            if (_progressLineStart >= 0)
            {
                // Replace the previous progress text in-place
                textBox.Select(_progressLineStart, textBox.TextLength - _progressLineStart);
                textBox.SelectionColor = Color.Gray;
                textBox.SelectedText = v;
                _progressLineStart = textBox.TextLength - v.Length;
            }
            else
            {
                _progressLineStart = textBox.TextLength;
                textBox.SelectionColor = Color.Gray;
                textBox.AppendText(v);
            }

            textBox.ScrollToCaret();
            textBox.Refresh();
        }

        internal void WriteHeader(string title)
        {
            var line = new string('\u2500', 50 - title.Length);
            WriteColored(Color.Blue, $"\n\u2500\u2500 {title} {line}\n");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            Owner.Enabled = true;
        }
    }
}