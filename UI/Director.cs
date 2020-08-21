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
	public partial class Director
	{
		MainWindow MainWindow;

		public Director( MainWindow mainWindow )
		{
			MainWindow = mainWindow;
		}

		public async Task RefreshList()
		{			
			using ( var sddp = new Utility.Sddp() )
			{
				sddp.OnResponse = r =>
				{
					if ( r.St != "c4:director" )
						return;

					_ = Connect( r );
				};

				sddp.Search( "c4:director" );

				await Task.Delay( 4000 );
			}
		}

		async Task<bool> Connect( Utility.Sddp.DeviceResponse connection )
		{
			try
			{
				MainWindow.DirectorPatch.Address.Text = connection.EndPoint.Address.ToString();

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
				return true;
			}
			catch ( System.Exception )
			{
				return false;
			}
		}

		internal void DirectorDisconnected()
		{
			MainWindow.SetStatusRight( $"Not Connected" );
		}
	}
}
