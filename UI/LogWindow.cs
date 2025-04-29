using System;
using System.Drawing;
using System.Windows.Forms;

namespace Garry.Control4.Jailbreak.UI
{
    public partial class LogWindow : Form
    {
        public LogWindow(Form mainWindow)
        {
            Owner = mainWindow;
            InitializeComponent();

            CenterToParent();
            Show();
        }

        private void Write(string v)
        {
            textBox.AppendText(v);

            textBox.ScrollToCaret();
            textBox.Refresh();
        }

        internal void WriteNormal(string v)
        {
            textBox.SelectionColor = Color.Black;
            Write(v);
        }

        internal void WriteSuccess(string v)
        {
            textBox.SelectionColor = Color.Green;
            Write(v);
        }

        // ReSharper disable once UnusedMember.Global
        internal void WriteWarning(string v)
        {
            textBox.SelectionColor = Color.Orange;
            Write(v);
        }

        internal void WriteError(Exception v)
        {
            WriteError($"\n{v.Message}\n");
            WriteNormal($"{v.StackTrace}\n");
        }

        internal void WriteError(string v)
        {
            textBox.SelectionColor = Color.Red;
            Write(v);
        }

        internal void WriteTrace(string v)
        {
            textBox.SelectionColor = Color.Gray;
            Write(v);
        }

        internal void WriteHighlight(string v)
        {
            textBox.SelectionColor = Color.Blue;
            Write(v);
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