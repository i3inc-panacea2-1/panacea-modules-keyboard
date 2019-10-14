using Panacea.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
using System.Windows.Native.Extensions;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Panacea.Modules.Keyboard
{
    /// <summary>
    /// Interaction logic for KeyboardWindow.xaml
    /// </summary>
    public partial class KeyboardWindow : NonFocusableWindow
    {


        public FrameworkElement Keyboard
        {
            get { return (FrameworkElement)GetValue(KeyboardProperty); }
            set { SetValue(KeyboardProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Keyboard.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty KeyboardProperty =
            DependencyProperty.Register("Keyboard", typeof(FrameworkElement), typeof(KeyboardWindow), new PropertyMetadata(null));


        public KeyboardWindow()
        {
            InitializeComponent();
        }

        private void KeyboardWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            //await Task.Delay(10);
            //var sc = Screen.PrimaryScreen;
            //if (Screen.AllScreens.Count() > 1)
            //{
            //    sc = Screen.AllScreens.First(s => !s.Primary);
            //}
            //var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
            //var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

            //var dpiX = (int)dpiXProperty.GetValue(null, null);
            //var dpiY = (int)dpiYProperty.GetValue(null, null);
            //Width = sc.WorkingArea.Width * 96.0/ dpiX;
            //Height = sc.WorkingArea.Height *  96.0 /dpiY / 3.0;
            //SetValue(WidthProperty, Width);
            //Left = sc.WorkingArea.Left *  96.0/ dpiX;
            //Top = sc.WorkingArea.Bottom *  96.0 / dpiY - ActualHeight;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            //var h = new WindowInteropHelper(this);
            //h.EnsureHandle();
            //var source = HwndSource.FromHwnd(h.Handle);
            //source.AddHook(new HwndSourceHook(WndProc));
        }
        private void KeyboardWindow_Loaded(object sender, RoutedEventArgs e)
        {



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

        private void BtnHide_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
        public new void Show()
        {
            var sc = Screen.PrimaryScreen;
            if (Screen.AllScreens.Count() > 1)
            {
                sc = Screen.AllScreens.First(s => !s.Primary);
            }
            var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);
            var dpiY = (int)dpiYProperty.GetValue(null, null);
            Height = sc.WorkingArea.Height * 96.0 /dpiY / 4;
            this.ToAppBar(System.Windows.Native.ABEdge.Bottom);
            base.Show();
        }

        public new void Hide()
        {
            this.ToAppBar(System.Windows.Native.ABEdge.None);
            base.Hide();
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
