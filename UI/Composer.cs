using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Mono.Cecil;
using System.Diagnostics;

namespace  Garry.Control4.Jailbreak
{
	public partial class Composer : UserControl
	{
		const string NewExtension = ".modded.exe";

		MainWindow MainWindow;

		public Composer( MainWindow MainWindow )
		{
			this.MainWindow = MainWindow;

			InitializeComponent();
		}

		void UpdateLinks()
		{
			var moddedExists = System.IO.File.Exists( "C:\\Program Files (x86)\\Control4\\Composer\\Pro\\ComposerPro.modded.exe" );
			linkOpenPatched.Enabled = moddedExists;
		}

		private void PatchComposer( object sender, EventArgs eventargs )
		{
			var log = new LogWindow( MainWindow );

			log.WriteTrace( "Asking for ComposerPro.exe location\n" );

			OpenFileDialog open = new OpenFileDialog();
			open.Filter = "Executable Files|*.exe";
			open.Title = "Find Original ComposerPro.exe";
			open.InitialDirectory = "C:\\Program Files (x86)\\Control4\\Composer\\Pro";
			open.FileName = "ComposerPro.exe";

			if ( open.ShowDialog() != DialogResult.OK )
			{
				log.WriteError( "Cancelled\n" );
				return;
			}

			if ( string.IsNullOrEmpty( open.FileName ) )
			{
				log.WriteError( "Filename was invalid\n" );
				return;
			}

			log.WriteNormal( "Opening " );
			log.WriteHighlight( $"{open.FileName}\n" );

			using ( var val = AssemblyDefinition.ReadAssembly( open.FileName ) )
			{
				log.WriteTrace( $"Finding \"Control4.ComposerPro.MainForm\"\n" );

				var mainForm = val.MainModule.Types.SingleOrDefault( x => x.FullName == "Control4.ComposerPro.MainForm" );
				if ( mainForm  == null )
				{
					log.WriteError( "Oops - couldn't find the class Control4.ComposerPro.MainForm\n" );
					return;
				}

				log.WriteNormal( $"Found " );
				log.WriteHighlight( $"{mainForm}\n" );

				log.WriteTrace( $"Finding \"ShowStartupOnFirstRun\"\n" );

				var showStartupOnFirstRun = mainForm.Methods.SingleOrDefault( x => x.Name == "ShowStartupOnFirstRun" );
				if ( showStartupOnFirstRun == null )
				{
					log.WriteError( "Oops - couldn't find the method ShowStartupOnFirstRun (Maybe already patched?)\n" );
					return;
				}

				log.WriteNormal( $"Found " );
				log.WriteHighlight( $"{showStartupOnFirstRun}\n" );

				log.WriteNormal( $"Removing.." );
				mainForm.Methods.Remove( showStartupOnFirstRun );
				log.WriteSuccess( $" ..Done!\n" );

				// TODO - Skip update check
				// TODO - Add info in window title
				// TODO - Fix dealeraccount.xml not found exception

				try
				{
					var outFile = System.IO.Path.ChangeExtension( open.FileName, NewExtension );

					//
					// We might be re-patching for some reason, so delete the old one
					//
					if ( System.IO.File.Exists( outFile ) )
					{
						System.IO.File.Delete( outFile );
					}

					log.WriteNormal( $"Saving to " );
					log.WriteHighlight( $"{outFile}\n\n" );

					// Save the file
					val.Write( outFile );

					// We need to copy the config and manifest too, or we'll be missing a bunch of settings
					log.WriteNormal( $"Copying " );
					log.WriteHighlight( $"ComposerPro.exe.manifest" );
					log.WriteNormal( $" to " );
					log.WriteHighlight( $"ComposerPro{NewExtension}.manifest\n" );

					System.IO.File.Copy( $"{open.FileName}.manifest", $"{outFile}.manifest", true );

					log.WriteNormal( $"Copying " );
					log.WriteHighlight( $"ComposerPro.exe.config" );
					log.WriteNormal( $" to " );
					log.WriteHighlight( $"ComposerPro{NewExtension}.config\n" );

					System.IO.File.Copy( $"{open.FileName}.config", $"{outFile}.config", true );

					// Create desktop shortcut
					{
						log.WriteNormal( $"\nCreating Desktop Shortcut.. " );

						var shortcutPath = $"{Environment.GetFolderPath( Environment.SpecialFolder.DesktopDirectory )}\\Composer Pro (Patched).url";

						if ( System.IO.File.Exists( shortcutPath ) )
							System.IO.File.Delete( shortcutPath );

						using ( StreamWriter writer = new StreamWriter( shortcutPath ) )
						{
							var app = outFile.Replace( '\\', '/' );

							writer.WriteLine( "[InternetShortcut]" );
							writer.WriteLine( $"URL=file:///{app.Replace( " ", "%20" )}" );
							writer.WriteLine( "IconIndex=0" );
							writer.WriteLine( "IconFile=" + app );
							writer.Flush();
						}

						log.WriteSuccess( $"done!\n" );
					}


					log.WriteSuccess( $"\nAll done - you can close this window!\n" );
				}
				catch ( UnauthorizedAccessException e )
				{
					log.WriteError( $"Exception - {e.Message}\n" );
				}
			}
		}

		private void SearchGoogleForComposer( object sender, EventArgs e )
		{
			System.Diagnostics.Process.Start( $"https://www.google.com/search?q=ComposerPro-3.1.0.566729-res.exe" );
		}

		private void OpenControl4Reddit( object sender, EventArgs e )
		{
			System.Diagnostics.Process.Start( $"https://www.reddit.com/r/C4diy/" );
		}

		private void OpenComposerFolder( object sender, LinkLabelLinkClickedEventArgs e )
		{
			System.Diagnostics.Process.Start( $"C:\\Program Files (x86)\\Control4\\Composer" );
		}

		private void StartModdedComposer( object sender, LinkLabelLinkClickedEventArgs e )
		{
			System.Diagnostics.Process.Start( $"C:\\Program Files (x86)\\Control4\\Composer\\Pro\\ComposerPro.modded.exe" );
		}
	}
}
