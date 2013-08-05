using System;
using System.Drawing;
using System.Windows.Forms;
using TopMost.Properties;

namespace TopMost
{
    public partial class Main : Form
    {
        private readonly ContextMenu _contextMenu;
        
        public Main()
        {
            InitializeComponent();

            Icon = Resources.TopMost;
            notifyIcon.Icon = Resources.TopMost;

            _contextMenu = new ContextMenu(new[] { new MenuItem("Exit", (sender, args) => Close()) });
            notifyIcon.ContextMenu = _contextMenu;

            Win32Api.RegisterHotKey(Handle, 0, (int) (KeyModifier.Control | KeyModifier.Alt), Keys.Space.GetHashCode());
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Win32Api.UnregisterHotKey(Handle, 0);
        }

        private void Main_Shown(object sender, EventArgs e)
        {
            Visible = false;
        }

        [Flags]
        enum KeyModifier
        {
            None = 0,
            Alt = 1,
            Control = 2,
            Shift = 4,
            WinKey = 8
        }

        private const int WM_HOTKEY = 0x0312;
        private IntPtr _handle = IntPtr.Zero;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_HOTKEY)
            {
                if (_handle == IntPtr.Zero)
                {
                    SetWindowOnTop();
                }
                else
                {
                    UnsetWindowOnTop();
                    _handle = IntPtr.Zero;
                }
            }
        }

        const Win32Api.SWPFlags Flags =
                Win32Api.SWPFlags.SWP_NOMOVE
                |
                Win32Api.SWPFlags.SWP_NOSIZE
                |
                Win32Api.SWPFlags.SWP_SHOWWINDOW
                ;

        private void SetWindowOnTop()
        {
            _handle = Win32Api.GetForegroundWindow();
            Win32Api.SetWindowPos(_handle, (IntPtr) Win32Api.SpecialWindowHandles.HWND_TOPMOST, 0, 0, 0, 0, Flags);
        }

        private void UnsetWindowOnTop()
        {
            Win32Api.SetWindowPos(_handle, (IntPtr)Win32Api.SpecialWindowHandles.HWND_NOTOPMOST, 0, 0, 0, 0, Flags);
        }
    }
}
