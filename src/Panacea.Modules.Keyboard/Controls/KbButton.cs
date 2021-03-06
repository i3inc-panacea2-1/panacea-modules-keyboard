﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using WindowsInput;

namespace Panacea.Modules.Keyboard.Controls
{
    public class KbButton : RepeatButton
    {
        private static SoundPlayer player;

        public static bool IsAltGrPressed { get { return InputSimulator.IsKeyDownAsync(VirtualKeyCode.RMENU) && InputSimulator.IsKeyDownAsync(VirtualKeyCode.LCONTROL); } }

        //private readonly DispatcherTimer timer;
        private bool switched;

        public bool Toggled { get; private set; }
        readonly List<ushort> shifts = new List<ushort> { (ushort)VirtualKeyCode.LSHIFT, (ushort)VirtualKeyCode.RSHIFT };
        public bool Switched { get { return switched; } }
        static event EventHandler<VirtualKeyCode> VirtualKeyDown;
        static event EventHandler<VirtualKeyCode> VirtualKeyUp;


        #region dependcy proprties

        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(String), typeof(KbButton),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (dep, val) =>
                    {
                        var self = dep as KbButton;
                        if (self == null) return;
                        self.CaptionVisibility = !string.IsNullOrEmpty(val.NewValue?.ToString())
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                    }));



        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(KbButton), new PropertyMetadata(null));


        // .NET Property wrapper

        public static readonly DependencyProperty ButtonBackgroundProperty =
            DependencyProperty.Register("ButtonBackground", typeof(Brush), typeof(KbButton),
                new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromRgb(0, 0, 0))));

        // .NET Property wrapper


        public static DependencyProperty ShiftCaptionProperty =
            DependencyProperty.Register("ShiftCaption", typeof(String), typeof(KbButton),
                new FrameworkPropertyMetadata(""));

        // .NET Property wrapper


        public static DependencyProperty AltCaptionProperty =
            DependencyProperty.Register("AltCaption", typeof(String), typeof(KbButton),
                new FrameworkPropertyMetadata(""));

        // .NET Property wrapper


        public static DependencyProperty FakeToggleProperty =
            DependencyProperty.Register("FakeToggle", typeof(bool), typeof(KbButton),
                new FrameworkPropertyMetadata(false));

        // .NET Property wrapper


        public static DependencyProperty VirtualKeyProperty =
            DependencyProperty.Register("VirtualKey", typeof(Keys), typeof(KbButton),
                new FrameworkPropertyMetadata(Keys.F1, OnVirtualKeyChanged));

        // .NET Property wrapper


        public static DependencyProperty RepeatProperty =
            DependencyProperty.Register("Repeat", typeof(bool), typeof(KbButton),
                new FrameworkPropertyMetadata(true));

        // .NET Property wrapper


        public static DependencyProperty AltableProperty =
            DependencyProperty.Register("Altable", typeof(bool), typeof(KbButton),
                new FrameworkPropertyMetadata(true));

        // .NET Property wrapper


        public static DependencyProperty CapsLikeShiftProperty =
            DependencyProperty.Register("CapsLikeShift", typeof(bool), typeof(KbButton),
                new FrameworkPropertyMetadata(true));

        // .NET Property wrapper



        public static DependencyProperty CaptionMarginProperty =
            DependencyProperty.Register("CaptionMargin", typeof(Thickness), typeof(KbButton),
                new FrameworkPropertyMetadata(null));

        public static DependencyProperty CaptionAlignmentProperty =
            DependencyProperty.Register("CaptionAlignment", typeof(VerticalAlignment), typeof(KbButton),
                new FrameworkPropertyMetadata(null));

        public Thickness CaptionMargin
        {
            get { return (Thickness)GetValue(CaptionMarginProperty); }
            set { SetValue(CaptionMarginProperty, value); }
        }

        public VerticalAlignment CaptionAlignment
        {
            get { return (VerticalAlignment)GetValue(CaptionAlignmentProperty); }
            set { SetValue(CaptionAlignmentProperty, value); }
        }

        public Brush ButtonBackground
        {
            get { return (Brush)GetValue(ButtonBackgroundProperty); }
            set { SetValue(ButtonBackgroundProperty, value); }
        }

        public String AltCaption
        {
            get { return (String)GetValue(AltCaptionProperty); }
            set { SetValue(AltCaptionProperty, value); }
        }

        public bool FakeToggle
        {
            get { return (bool)GetValue(FakeToggleProperty); }
            set { SetValue(FakeToggleProperty, value); }
        }

        public Keys VirtualKey
        {
            get { return (Keys)GetValue(VirtualKeyProperty); }
            set { SetValue(VirtualKeyProperty, value); }
        }

        public bool Repeat
        {
            get { return (bool)GetValue(RepeatProperty); }
            set { SetValue(RepeatProperty, value); }
        }

        public bool Altable
        {
            get { return (bool)GetValue(AltableProperty); }
            set { SetValue(AltableProperty, value); }
        }

        public bool CapsLikeShift
        {
            get { return (bool)GetValue(CapsLikeShiftProperty); }
            set { SetValue(CapsLikeShiftProperty, value); }
        }

        // .NET Property wrapper

        public String Caption
        {
            get { return (String)GetValue(CaptionProperty); }
            set
            {
                SetValue(CaptionProperty, value);
            }
        }

        public string ShiftCaption
        {
            get { return (string)GetValue(ShiftCaptionProperty); }
            set { SetValue(ShiftCaptionProperty, value); }
        }


        public static DependencyProperty LightOpacityProperty =
           DependencyProperty.Register("LightOpacity", typeof(double), typeof(KbButton),
               new FrameworkPropertyMetadata(0.0));

        public static DependencyProperty CaptionVisibilityProperty =
           DependencyProperty.Register("CaptionVisibility", typeof(Visibility), typeof(KbButton),
               new FrameworkPropertyMetadata(Visibility.Visible));

        public static DependencyProperty ShiftCaptionVisibilityProperty =
          DependencyProperty.Register("ShiftCaptionVisibility", typeof(Visibility), typeof(KbButton),
              new FrameworkPropertyMetadata(Visibility.Visible));

        public static DependencyProperty AltCaptionVisibilityProperty =
          DependencyProperty.Register("AltCaptionVisibility", typeof(Visibility), typeof(KbButton),
              new FrameworkPropertyMetadata(Visibility.Collapsed));



        public double LightOpacity
        {
            get { return (double)GetValue(LightOpacityProperty); }
            set { SetValue(LightOpacityProperty, value); }
        }

        public Visibility CaptionVisibility
        {
            get { return (Visibility)GetValue(CaptionVisibilityProperty); }
            set { SetValue(CaptionVisibilityProperty, value); }
        }

        public Visibility ShiftCaptionVisibility
        {
            get { return (Visibility)GetValue(ShiftCaptionVisibilityProperty); }
            set { SetValue(ShiftCaptionVisibilityProperty, value); }
        }

        public Visibility AltCaptionVisibility
        {
            get { return (Visibility)GetValue(AltCaptionVisibilityProperty); }
            set { SetValue(AltCaptionVisibilityProperty, value); }
        }
        #endregion


        static KbButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(KbButton), new FrameworkPropertyMetadata(typeof(KbButton)));
            var si =
                System.Windows.Application.GetResourceStream(
                    new Uri("pack://application:,,,/Panacea.Modules.Keyboard;component/Resources/Audio/keyboard.wav"));
            player = new SoundPlayer(si.Stream);
            player.Load();
            //m_GlobalHook.KeyDown += (oo, ee) =>
            //{
            //    if (_downs.ContainsKey(ee.KeyCode))
            //    {
            //        _downs[ee.KeyCode].ForEach(a => a());
            //    }


            //    if (ee.KeyCode == Keys.Capital || ee.KeyCode == Keys.ShiftKey || ee.KeyCode == Keys.Menu || ee.KeyCode == Keys.LShiftKey || ee.KeyCode == Keys.RShiftKey)
            //    {
            //        _buttons.ForEach(b => {
            //            b.CheckCapsAndIns();
            //            b.CheckCapsAnShift();
            //        });
            //    }
            //    if (ee.KeyCode != Keys.Capital && ee.KeyCode != Keys.ShiftKey && ee.KeyCode != Keys.Menu && ee.KeyCode != Keys.LShiftKey && ee.KeyCode != Keys.RShiftKey)
            //    {
            //        _buttons.ForEach((b) => b.OnDifferentKeyDown());
            //    }
            //};
            //m_GlobalHook.KeyUp += (oo, ee) =>
            //{
            //    if (_ups.ContainsKey(ee.KeyCode))
            //    {
            //        _ups[ee.KeyCode].ForEach(a => a());
            //    }
            //    if (ee.KeyCode == Keys.Capital || ee.KeyCode == Keys.ShiftKey || ee.KeyCode == Keys.Menu || ee.KeyCode == Keys.LShiftKey || ee.KeyCode == Keys.RShiftKey)
            //    {
            //        _buttons.ForEach(b => {
            //            b.CheckCapsAndIns();
            //            b.CheckCapsAnShift();
            //        });
            //    }
            //};

            //player.SoundLocation=Utils.Path()+
        }

        public KbButton()
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                IsVisibleChanged += KbButton_IsVisibleChanged;
            }

            Repeat = false;
            timer.Interval = TimeSpan.FromMilliseconds(300);
            timer.Tick += Timer_Tick;
            VirtualKeyDown += KbButton_VirtualKeyDown;
            VirtualKeyUp += KbButton_VirtualKeyUp;
        }

        private void KbButton_VirtualKeyUp(object sender, VirtualKeyCode e)
        {
            UpdateLight();
            UpdateButton();
        }

        private void KbButton_VirtualKeyDown(object sender, VirtualKeyCode e)
        {
            if (e != VirtualKeyCode.LSHIFT && e != VirtualKeyCode.RSHIFT)
            {
                OnDifferentKeyDown();
            }
            UpdateLight();
            UpdateButton();
        }

        private async void KbButton_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible) return;
            if (VirtualKey != Keys.Capital && VirtualKey != Keys.ShiftKey && VirtualKey != Keys.RShiftKey && VirtualKey != Keys.LShiftKey) return;
            if (Toggled)
            {
                await Task.Delay(30);
                SimulateKeyDown();
                await Task.Delay(30);
                SimulateKeyUp();
                _pressed = false;
            }
        }


        private static void OnVirtualKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            KbButton b = (KbButton)d;

            b.UpdateLight();
            b.UpdateButton();
        }



        void Timer_Tick(object sender, EventArgs e)
        {
            timer.Interval = TimeSpan.FromMilliseconds(150);
            SimulateKeyUp();
            SimulateKeyDown();
        }

        protected void UpdateButton()
        {
            if (IsShiftToggled())
            {
                if (IsCapsLockToggled() && CapsLikeShift)
                {
                    if (switched)
                    {
                        Switch();
                    }
                }
                else
                {
                    if (!switched)
                    {
                        Switch();
                    }
                }
            }
            else
            {
                if (IsCapsLockToggled() && CapsLikeShift)
                {
                    if (!switched)
                    {
                        Switch();
                    }
                }
                else
                {
                    if (switched)
                    {
                        Switch();
                    }
                }
            }
            if (IsAltGrPressed)
            {
                if (Altable)
                {
                    CaptionVisibility = Visibility.Collapsed;
                    ShiftCaptionVisibility = Visibility.Collapsed;
                    AltCaptionVisibility = Visibility.Visible;
                }
                if (new List<Keys>(){
                            Keys.ShiftKey,
                            Keys.Tab,
                            Keys.Return,
                            Keys.Capital,
                Keys.Back}.Contains(
                    this.VirtualKey))
                {
                    Visibility = System.Windows.Visibility.Hidden;
                }

            }
            else
            {
                if (new List<Keys>(){
                            Keys.ShiftKey,
                            Keys.Tab,
                            Keys.Return,
                            Keys.Capital,
                Keys.Back}.Contains(
                    this.VirtualKey))
                {
                    Visibility = System.Windows.Visibility.Visible;
                }
                if (!string.IsNullOrEmpty(Caption))
                {
                    CaptionVisibility = Visibility.Visible;
                    ShiftCaptionVisibility = Visibility.Visible;
                }
                else
                {
                    CaptionVisibility = Visibility.Collapsed;
                }
                AltCaptionVisibility = Visibility.Collapsed;
            }
        }

        bool IsShiftToggled()
        {
            return InputSimulator.IsKeyDownAsync(VirtualKeyCode.SHIFT);
        }

        bool IsCapsLockToggled()
        {
            return InputSimulator.IsTogglingKeyInEffect(VirtualKeyCode.CAPITAL);
        }

        protected void UpdateLight()
        {
            if (VirtualKey != Keys.Capital && VirtualKey != Keys.ShiftKey && VirtualKey != Keys.RShiftKey && VirtualKey != Keys.LShiftKey) return;
            if (VirtualKey == Keys.Capital)
            {
                if (IsCapsLockToggled() || Toggled)
                {
                    LightOpacity = 1;
                }
                else
                {
                    LightOpacity = 0;
                }
            }

            if (VirtualKey == Keys.LShiftKey || VirtualKey == Keys.RShiftKey)
            {
                if (IsShiftToggled())
                {
                    LightOpacity = 1;
                }
                else
                {
                    LightOpacity = 0;
                }
            }

        }

        async void OnDifferentKeyDown()
        {
            if ((VirtualKey == Keys.RShiftKey || VirtualKey == Keys.LShiftKey) && Toggled)
            {
                await Task.Delay(30);
                SimulateKeyDown();
                await Task.Delay(30);
                SimulateKeyUp();
                _pressed = false;
            }
        }

        void OnKeyDown()
        {
            if (FakeToggle && !Toggled)
            {
                Toggled = true;
            }

            ButtonBackground = new SolidColorBrush(Color.FromArgb(120, 255, 255, 255));
        }
        void OnKeyUp()
        {
            if (FakeToggle && Toggled)
            {
                Toggled = false;
            }
            ButtonBackground = Brushes.Transparent;
        }


        public void Switch()
        {
            if (ShiftCaption != "")
            {
                string tmp = Caption;
                Caption = ShiftCaption;
                ShiftCaption = tmp;
                switched = !switched;
            }
        }


        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (ShiftCaption == "")
            {
                CaptionMargin = new Thickness(15);
                CaptionAlignment = VerticalAlignment.Center;
            }
            else
            {
                CaptionMargin = new Thickness(-2, 18, 0, 3);
                CaptionAlignment = VerticalAlignment.Bottom;
            }

        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            player?.Play();
            base.OnMouseDown(e);
            e.Handled = true;
            if (!_pressed)
            {
                SimulateKeyDown();
                if (!FakeToggle)
                {
                    timer.Interval = TimeSpan.FromMilliseconds(300);
                    timer.Start();
                }
            }
            else
            {
                SimulateKeyUp();
            }
        }

        bool _pressed = false;

        public void SimulateKeyDown()
        {
            if (VirtualKey == Keys.LShiftKey) Console.WriteLine("f1");
            if (!FakeToggle)
            {
                if (VirtualKey != Keys.NoName)
                {
                    InputSimulator.SimulateKeyDown((VirtualKeyCode)VirtualKey);
                    VirtualKeyDown?.Invoke(this, (VirtualKeyCode)VirtualKey);
                }
                else
                {
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.LCONTROL);
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.RMENU);
                }
                _pressed = true;
            }
            else
            {
                if (!Toggled)
                {
                    Toggled = true;
                    ButtonBackground = new SolidColorBrush(Color.FromArgb(120, 255, 255, 255));
                    if (VirtualKey != Keys.NoName)
                    {
                        InputSimulator.SimulateKeyDown((VirtualKeyCode)VirtualKey);
                        VirtualKeyDown?.Invoke(this, (VirtualKeyCode)VirtualKey);
                    }
                    else
                    {
                        InputSimulator.SimulateKeyDown(VirtualKeyCode.LCONTROL);
                        InputSimulator.SimulateKeyDown(VirtualKeyCode.RMENU);
                    }
                }
                else
                {
                    Toggled = false;
                    ButtonBackground = Brushes.Transparent;
                    if (VirtualKey != Keys.NoName)
                    {
                        InputSimulator.SimulateKeyUp((VirtualKeyCode)VirtualKey);
                        VirtualKeyUp?.Invoke(this, (VirtualKeyCode)VirtualKey);
                    }
                    else
                    {

                        InputSimulator.SimulateKeyUp(VirtualKeyCode.RMENU);
                        InputSimulator.SimulateKeyUp(VirtualKeyCode.LCONTROL);
                    }
                }
            }
        }

        DispatcherTimer timer = new DispatcherTimer();

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            e.Handled = true;
            timer.Stop();
            SimulateKeyUp();
        }

        public void SimulateKeyUp()
        {
            if (!_pressed) return;

            if (!FakeToggle)
            {
                _pressed = false;
                if (VirtualKey != Keys.NoName)
                {
                    InputSimulator.SimulateKeyUp((VirtualKeyCode)VirtualKey);
                    VirtualKeyUp?.Invoke(this, (VirtualKeyCode)VirtualKey);

                }
                else if (IsAltGrPressed)
                {

                    InputSimulator.SimulateKeyUp(VirtualKeyCode.RMENU);
                    InputSimulator.SimulateKeyUp(VirtualKeyCode.LCONTROL);
                }
                else
                {

                    InputSimulator.SimulateKeyDown(VirtualKeyCode.LCONTROL);
                    InputSimulator.SimulateKeyDown(VirtualKeyCode.RMENU);
                }
            }
        }
        private void KbButtonBase_Loaded(object sender, RoutedEventArgs e)
        {

        }
        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            timer.Stop();
            if (_pressed)
            {
                SimulateKeyUp();

            }
        }

    }
}
