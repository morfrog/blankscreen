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
        FormMenu _formMenu;
        Rectangle _lastWindowRect;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs ev)
        {
            _formCursor = new FormCursorAutoHide(this, 3000);
            _formMenu = new FormMenu(contextMenuStrip1);
            _formMenu.MenuItemClicked += _formMenu_MenuItemClicked;
            this.MinimumSize = this.Size;

            if( Settings1.Default.WindowSize.IsEmpty )
            {
                var screen = Screen.PrimaryScreen;
                var size = new Size((screen.Bounds.Size.Width - this.DefaultSize.Width) / 2, (screen.Bounds.Size.Height - this.DefaultSize.Height) / 2);
                Point offCentre = Point.Add(screen.Bounds.Location, size);
                _lastWindowRect = new Rectangle(offCentre, this.DefaultSize);
            }
            else
            {
                _lastWindowRect = new Rectangle(Settings1.Default.WindowLocation, Settings1.Default.WindowSize);
            }

            this.Location = _lastWindowRect.Location;
            this.Size = _lastWindowRect.Size;

            this.TopMost = Settings1.Default.Topmost;
            if( Settings1.Default.ScreenIndex >= 0 )
            {
                SetScreenMode(ScreenMode.Fullscreen, Settings1.Default.ScreenIndex);
            }
        }

        void _formMenu_MenuItemClicked(FormMenu.FormMenuEventArgs args)
        {
            switch( args.Action )
            {
                case FormMenu.FormMenuEventArgs.MenuAction.Topmost:
                    Settings1.Default.Topmost = this.TopMost = args.On;
                    break;
                case FormMenu.FormMenuEventArgs.MenuAction.Minimise:
                    this.WindowState = FormWindowState.Minimized;
                    break;
                case FormMenu.FormMenuEventArgs.MenuAction.Exit:
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    break;
                case FormMenu.FormMenuEventArgs.MenuAction.Screen:
                    ChangeScreen(ScreenChange.Normal, args.Screen, args.On );
                    break;
            }
            
        }

        private int CurrentScreen
        {
            get
            {
                var pt = new Point( this.Location.X, this.Location.Y);
                pt.Offset(this.Width / 2, this.Height / 2);
                int index = 0;
                foreach (var screen in Screen.AllScreens)
                {
                    if (screen.Bounds.Contains(pt))
                    {
                        return index;
                    }
                    index++;
                }
                return 0;
            }
        }

        private ScreenMode CurrentScreenMode
        {
            get
            {
                return this.FormBorderStyle == System.Windows.Forms.FormBorderStyle.None ? ScreenMode.Fullscreen : ScreenMode.Window;
            }
        }

        private enum ScreenMode { Window, Fullscreen };
        private enum ScreenChange { Normal, Toggle };

        private void ChangeScreen(ScreenChange change, int screenIndex, bool on = true)
        {
            var currScreenMode = this.FormBorderStyle == System.Windows.Forms.FormBorderStyle.None ? ScreenMode.Fullscreen : ScreenMode.Window;
            var currScreenIndex = CurrentScreen;
            ScreenMode newScreenMode;

            if (change == ScreenChange.Toggle || (on ? currScreenMode == ScreenMode.Window : change == ScreenChange.Normal))
            {
                newScreenMode = currScreenMode == ScreenMode.Fullscreen ? ScreenMode.Window : ScreenMode.Fullscreen;
            }
            else
            {
                newScreenMode = currScreenIndex == screenIndex ? currScreenMode : ScreenMode.Fullscreen; // always fullscreen when changing screens
            }
            SetScreenMode(newScreenMode, screenIndex);
        }
        
        private void SetScreenMode(ScreenMode mode, int screenIndex)
        {
            if (screenIndex >= 0 && screenIndex < Screen.AllScreens.Count())
            {
                var screen = Screen.AllScreens[screenIndex];
                if (CurrentScreenMode != mode || CurrentScreen != screenIndex)
                {
                    if (mode == ScreenMode.Fullscreen)
                    {
                        if (CurrentScreenMode == ScreenMode.Window)
                        {
                            _lastWindowRect = new Rectangle(this.Location,this.Size);
                        }
                        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                        this.Location = screen.Bounds.Location;
                        this.Size = screen.Bounds.Size;
                        labelHelp.Hide();
                        _formMenu.SetScreen(screenIndex);
                    }
                    else
                    {
                        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                        this.Location = _lastWindowRect.Location;
                        this.Size = _lastWindowRect.Size;
                        labelHelp.Show();
                        _formMenu.SetScreen();
                    }
                }

                Settings1.Default.ScreenIndex = screenIndex;
                this.TopMost = mode == ScreenMode.Fullscreen && _formMenu.TopMost;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (CurrentScreenMode == ScreenMode.Fullscreen)
            {
                Settings1.Default.WindowLocation = _lastWindowRect.Location;
                Settings1.Default.WindowSize = _lastWindowRect.Size;
            }
            else
            {
                Settings1.Default.WindowLocation = this.Location;
                Settings1.Default.WindowSize = this.Size;
            }
            Settings1.Default.Save();
        }

        private void Form1_DoubleClick(object sender, EventArgs e)
        {
            int screenIndex;
            var mouseArgs = e as MouseEventArgs;
            if (mouseArgs != null )
            {
                var ptScreen = this.PointToScreen(mouseArgs.Location);
                var screen = Screen.FromPoint(ptScreen);
                screenIndex = Screen.AllScreens.ToList().IndexOf(screen);
            }
            else
            {
                screenIndex = CurrentScreen;
            }

            ChangeScreen(ScreenChange.Toggle, screenIndex);
        }

        private void labelHelp_DoubleClick(object sender, EventArgs e)
        {
            Form1_DoubleClick(sender, e);
        }
    }
}
