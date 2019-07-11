﻿using Panacea.Controls;
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
            var h = new WindowInteropHelper(this);
            h.EnsureHandle();
            var source = HwndSource.FromHwnd(h.Handle);
            source.AddHook(new HwndSourceHook(WndProc));
            Loaded += KeyboardWindow_Loaded;
            SizeChanged += KeyboardWindow_SizeChanged;
           
        }

        private void KeyboardWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            //await Task.Delay(10);
            var sc = Screen.PrimaryScreen;
            if (Screen.AllScreens.Count() > 1)
            {
                sc = Screen.AllScreens.First(s => !s.Primary);
            }
            Width = sc.WorkingArea.Width;
            Height = sc.WorkingArea.Height / 2.8;
            SetValue(WidthProperty, Width);
            Left = sc.WorkingArea.Left;
            Top = sc.WorkingArea.Bottom - ActualHeight;
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

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
