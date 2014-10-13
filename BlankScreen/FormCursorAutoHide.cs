using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BlankScreen
{
    public class FormCursorAutoHide
    {
        Timer _timer;
        System.Drawing.Point _ptHide;
        bool _visible = true;
        public FormCursorAutoHide(Form frm, int interval)
        {
            _timer = new Timer() { Interval = interval };
            _timer.Tick += _timer_Tick;
            frm.MouseEnter += frm_MouseEnter;
            frm.MouseLeave += frm_MouseLeave;

            frm.MouseMove += frm_MouseMove;
        }

        void frm_MouseLeave(object sender, EventArgs e)
        {
            _timer.Stop();
        }

        void frm_MouseMove(object sender, MouseEventArgs e)
        {
            Visible = true;
            _timer.Start();
        }

        void frm_MouseEnter(object sender, EventArgs e)
        {
            Visible = true;
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            Visible = false;
        }

        public bool Visible
        {
            get
            {
                return _visible;
            }
            private set
            {
                if (value != _visible)
                {
                    if (value)
                    {
                        if (_ptHide != null && Cursor.Position.Equals(_ptHide)) // still at the location it was hidden it, don't show till it actually moves
                        {
                            return;
                        }
                        Cursor.Show();
                    }
                    else
                    {
                        _ptHide = Cursor.Position;
                        Cursor.Hide();
                    }
                    _visible = value;
                }
            }
        }
    }
}
