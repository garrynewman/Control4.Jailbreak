using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace  Garry.Control4.Jailbreak
{
	public partial class Director : UserControl
	{
		MainWindow MainWindow;

		public Director( MainWindow mainWindow )
		{
			InitializeComponent();

			MainWindow = mainWindow;
		}

		public async Task RefreshList()
		{
			refreshLink.Enabled = false;

			using ( var sddp = new Utility.Sddp() )
			{
				sddp.OnResponse = r =>
				{
					if ( r.St != "c4:director" )
						return;

					Invoke( (Action)(() =>
					{
						controllerList.Items.Add( r );

						if ( controllerList.SelectedIndex < 0 )
						{
							controllerList.SelectedIndex = 0;
							Connect( r );
						}
					}));
				};

				sddp.Search( "c4:director" );

				await Task.Delay( 4000 );
			}

			refreshLink.Enabled = true;
		}

		private void SelectedIndexChanged( object sender, EventArgs e )
		{
			if ( controllerList.SelectedItem == null )
			{
				controllerAddress.Text = "";
				return;
			}

			controllerAddress.Text = (controllerList.SelectedItem as Utility.Sddp.DeviceResponse).EndPoint.Address.ToString();
		}

		private void ControllerAddressTextChanged( object sender, EventArgs e )
		{
			selectButton.Enabled = !string.IsNullOrWhiteSpace( controllerAddress.Text );
		}

		private async void ConnectButtonClicked( object sender, EventArgs e )
		{
			if ( controllerList.SelectedItem == null )
				return;

			Enabled = false;
			try
			{
				await Connect( controllerList.SelectedItem as Utility.Sddp.DeviceResponse );
			}
			catch ( System.Exception exception )
			{
				Trace.WriteLine( exception );
				// Back to normal
			}
			finally
			{
				Enabled = true;
			}
		}

		async Task<bool> Connect( Utility.Sddp.DeviceResponse connection )
		{
			try
			{
				MainWindow.SetStatusRight( $"Connecting to {connection.EndPoint.Address}.." );

				var director = new DirectorManager( connection.EndPoint.Address );

				var result = await director.TryInitialize();
				if ( !result )
				{
					MessageBox.Show( "Couldn't connect to director", "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Information );
					return false;
				}

				MainWindow.ConnectedDirector = director;
				MainWindow.SetStatusRight( $"Connected to {connection.EndPoint.Address}" );

				UpdateDirectorInfo();
				return true;
			}
			catch ( System.Exception )
			{
				return false;
			}
		}

		internal void DirectorDisconnected()
		{
			directorInfo.Text = "";
			MainWindow.SetStatusRight( $"Not Connected" );

			controllerList.Items.Clear();
		}

		private void UpdateDirectorInfo()
		{
			MainWindow.Backup.Enabled = true;
			MainWindow.DirectorPatch.Enabled = true;
			MainWindow.Restore.Enabled = true;

			directorInfo.Text = $"System Name: {MainWindow.ConnectedDirector.SystemName}\n";
			directorInfo.Text += $"Common Name: {MainWindow.ConnectedDirector.CommonName}\n";
			directorInfo.Text += $"Version: {MainWindow.ConnectedDirector.Version}";
		}

		private void RefreshLink( object sender, LinkLabelLinkClickedEventArgs e )
		{
			controllerList.Items.Clear();
			RefreshList();
		}
	}
}
