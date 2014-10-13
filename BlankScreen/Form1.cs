using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlankScreen
{
    public partial class Form1 : Form
    {
        FormCursorAutoHide _formCursor;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs ev)
        {
            _formCursor = new FormCursorAutoHide(this, 3000);
            if (Screen.AllScreens.Count() > 1)
            {
                int index = 0;
                foreach( var screen in Screen.AllScreens )
                {
                    var name = string.Format("Screen &{0}", index+1);
                    int screenIndex = index;
                    var handler = new EventHandler((o, e) => screenHandler_Click(o, ev, screenIndex));
                    contextMenuStrip1.Items.Add(name, null, handler);
                    index++;
                }
            }
            
            contextMenuStrip1.Items.Add(new ToolStripSeparator());
            contextMenuStrip1.Items.Add(new ToolStripMenuItem("&Topmost", null, new EventHandler(topmostToolStripMenuItem_Click)) { CheckOnClick = true, Checked = Settings1.Default.Topmost });
            contextMenuStrip1.Items.Add(new ToolStripMenuItem("&Minimize", null, new EventHandler(minimiseToolStripMenuItem_Click)));
            contextMenuStrip1.Items.Add("E&xit", null, new EventHandler(exitToolStripMenuItem_Click));

            this.TopMost = Settings1.Default.Topmost;
            if( Settings1.Default.ScreenIndex >= 0 )
            {
                labelHelp.Hide();
                screenHandler_Click(this, null, Settings1.Default.ScreenIndex);
            }
        }

        private void topmostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings1.Default.Topmost = this.TopMost = ((ToolStripMenuItem)sender).Checked;
        }

        private void minimiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void screenHandler_Click(object sender, EventArgs e, int index)
        {
            if( index >= 0 && index < Screen.AllScreens.Count() )
            {
                var screen = Screen.AllScreens[index];
                this.Location = screen.Bounds.Location;
                this.Size = screen.Bounds.Size;
                Settings1.Default.ScreenIndex = index;
                labelHelp.Hide();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings1.Default.Save();
        }
    }
}
