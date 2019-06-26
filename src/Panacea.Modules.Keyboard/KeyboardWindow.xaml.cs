using Panacea.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Panacea.Modules.Keyboard
{
    /// <summary>
    /// Interaction logic for KeyboardWindow.xaml
    /// </summary>
    public partial class KeyboardWindow : NonFocusableWindow
    {
        public KeyboardWindow()
        {
            InitializeComponent();
            var h = new WindowInteropHelper(this);
            h.EnsureHandle();
            var source = HwndSource.FromHwnd(h.Handle);
            source.AddHook(new HwndSourceHook(WndProc));
            var sc = Screen.PrimaryScreen;
            Height = sc.WorkingArea.Height / 3.2;
            Width = sc.WorkingArea.Width;
            Left = sc.WorkingArea.Left;
            Top = sc.WorkingArea.Bottom - Height;
        }
        private const int WM_MOUSEACTIVATE = 0x0021, MA_NOACTIVATE = 0x0003;
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_MOUSEACTIVATE)
            {
                handled = true;
                return new IntPtr(MA_NOACTIVATE);
            }
            return IntPtr.Zero;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
