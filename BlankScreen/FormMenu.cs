using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BlankScreen
{
    class FormMenu
    {
        private List<ToolStripMenuItem> screenMenuItems = new List<ToolStripMenuItem>();
        private ToolStripMenuItem topmostMenuItem;
        ContextMenuStrip _menu;

        public FormMenu(ContextMenuStrip menu)
        {
            _menu = menu;
            if (Screen.AllScreens.Count() > 1)
            {
                int index = 0;
                foreach (var screen in Screen.AllScreens)
                {
                    int screenIndex = index;
                    var item = new ToolStripMenuItem(
                        string.Format("Screen &{0}", index + 1), null,
                        new EventHandler((o, e) => MenuHandler(o,e,FormMenuEventArgs.MenuAction.Screen,screenIndex))
                    )
                    {
                        CheckOnClick = true,
                        Checked = Settings1.Default.ScreenIndex == screenIndex
                    };
                    screenMenuItems.Add(item);
                    _menu.Items.Add(item);
                    index++;
                }
            }

            _menu.Items.Add(new ToolStripSeparator());
            topmostMenuItem = new ToolStripMenuItem(
                "&Topmost", null,
                new EventHandler((o, e) => MenuHandler(o,e, FormMenuEventArgs.MenuAction.Topmost))
                )
                {
                    CheckOnClick = true,
                    Checked = Settings1.Default.Topmost
                };
            _menu.Items.Add(topmostMenuItem);
            _menu.Items.Add(
                "&Minimize", null,
                new EventHandler((o, e) => MenuHandler(o,e,FormMenuEventArgs.MenuAction.Minimise))
            );
            _menu.Items.Add(new ToolStripSeparator());
            _menu.Items.Add(
                "E&xit", null,
                new EventHandler((o, e) => MenuHandler(o,e,FormMenuEventArgs.MenuAction.Exit))
            );
        }

        private void MenuHandler(object sender, EventArgs args, FormMenuEventArgs.MenuAction action, int index = -1)
        {
            if( MenuItemClicked == null )
            {
                return;
            }
            var frmArgs = new FormMenuEventArgs(action)
            {
                On = (sender as ToolStripMenuItem).Checked,
                Screen = index
            };
            MenuItemClicked(frmArgs);
        }


        public class FormMenuEventArgs : EventArgs
        {
            public enum MenuAction {Screen,Topmost,Minimise,Exit};
            public FormMenuEventArgs(MenuAction action, int screen = -1 )
            {
                Action = action;
                Screen = screen;
            }

            public bool On { get; set; }

            public MenuAction Action { get; set; }

            public int Screen { get; set; }
        }

        public delegate void MenuItemAction(FormMenuEventArgs args);
        public event MenuItemAction MenuItemClicked;

        public void SetScreen(int selectedIndex = -1)
        {
            for (int index = 0; index < screenMenuItems.Count; index++)
            {
                bool check = index == selectedIndex;
                if (screenMenuItems[index].Checked != check)
                {
                    screenMenuItems[index].Checked = check;
                }
            }
        }

        public bool TopMost
        {
            get
            {
                return topmostMenuItem.Checked;
            }
            set
            {
                topmostMenuItem.Checked = value;
            }
        }

    }
}
