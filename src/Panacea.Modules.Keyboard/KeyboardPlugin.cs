using Panacea.Core;
using Panacea.Modularity;
using Panacea.Modularity.Keyboard;
using Panacea.Modularity.UiManager;
using Panacea.Modules.Keyboard.Models;
using Panacea.Modules.Keyboard.ViewModels;
using Panacea.Multilinguality;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Panacea.Modules.Keyboard
{
    public class KeyboardPlugin : IKeyboardPlugin
    {
        KeyboardWindow _kbWindow;
        VirtualKeyboard _keyboard;
        DateKeyboard _dateKeyboard;
        NumberKeyboard _numberKeyboard;
        LanguageButtonViewModel _languageButton { get; set; }
        NavigationButtonViewModel _navButton;
        List<Language> _languages;
        private readonly PanaceaServices _core;

        [PanaceaInject("IgnoreKeyboards", "Keyboard devices to ignore when desciding if Panacea should show the on screen keyboard", "IgnoreKeyboards=PID_0E41;PID_TEST")]
        protected string IgnoreKeyboards { get; set; } = "";

        public KeyboardPlugin(PanaceaServices core)
        {
            _core = core;
        }

        public async Task BeginInit()
        {
            await Task.Run(() =>
            {
                using (var searcher = new ManagementObjectSearcher("Select DeviceID from Win32_Keyboard"))
                {
                    foreach (ManagementObject keyboard in searcher.Get())
                    {
                        var id = keyboard.GetPropertyValue("DeviceID").ToString();
                        _core.Logger.Info(this, id);
                    }
                }
            });
        }

        public void Dispose()
        {

        }

        public async Task EndInit()
        {
            List<Language> inputLanguages = null;
            var res = await _core.HttpClient.GetObjectAsync<GetVersionsResponse>("get_versions/");
            if (res.Success)
            {
                var trans = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

                foreach (var k in res.Result.Translations)
                {
                    trans.Add(k.Key, new Dictionary<string, Dictionary<string, string>>()); // language
                    foreach (var kk in k.Value)
                    {
                        trans[k.Key].Add(kk.Key, new Dictionary<string, string>());
                        foreach (var transl in kk.Value)
                        {
                            trans[k.Key][kk.Key].Add(transl.Id, transl.Trans);
                        }
                    }
                }
                LanguageContext.Instance.Dictionary.translations = trans;
                inputLanguages = res.Result.InputLanguages;
                _languages = res.Result.Languages;
            }
            else
            {
                throw new Exception(res.Error);
            }
            _kbWindow = new KeyboardWindow();
            _kbWindow.IsVisibleChanged += _kbWindow_IsVisibleChanged;
            _keyboard = new VirtualKeyboard(inputLanguages.Select(l => new System.Globalization.CultureInfo(l.Code)).ToList());
            _dateKeyboard = new DateKeyboard();
            _numberKeyboard = new NumberKeyboard();
            if (_core.TryGetUiManager(out IUiManager ui))
            {
                _navButton = new NavigationButtonViewModel(this);
                ui.AddNavigationBarControl(_navButton);
                _languageButton = new LanguageButtonViewModel(_languages);
                ui.AddNavigationBarControl(_languageButton);
            }


            //ShowKeyboard(_keyboard);
            //return Task.CompletedTask;
            EventManager.RegisterClassHandler(typeof(UIElement), System.Windows.Input.Keyboard.PreviewGotKeyboardFocusEvent,
                (KeyboardFocusChangedEventHandler)OnPreviewGotKeyboardFocus);

            //Automation.AddAutomationFocusChangedEventHandler(new  AutomationFocusChangedEventHandler(OnUIAutomationEvent));

        }

        private void _kbWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            KeyboardOpenChanged?.Invoke(this, (bool)e.NewValue);
        }

        //private void OnUIAutomationEvent(object sender, AutomationEventArgs e)
        //{
        //    Debug.WriteLine("external");
        //    var el = AutomationElement.FocusedElement;
        //    var current = el.Current;
        //    Debug.WriteLine(current.ControlType.ProgrammaticName);
        //    if (current.HasKeyboardFocus)
        //    {
        //        if (IsKeyboardOpen) return;
        //        ShowKeyboard(_keyboard);
        //    }
        //    else HideKeyboard();
        //    Debug.WriteLine("end external");
        //}

        private void OnPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            //Debug.WriteLine("internal");
            //Debug.WriteLine("GotFocus");
            var txt = e.NewFocus as TextBoxBase;
            var pass = e.NewFocus as PasswordBox;
            if (txt != null)
            {
                HandleElement(txt);
            }
            else if (pass != null)
            {
                HandleElement(pass);
            }
            else
            {

                HideKeyboard();
            }
            //Debug.WriteLine("end internal");
        }

        void HandleElement(FrameworkElement el)
        {
            el.LostFocus -= Txt_LostFocus;
            //el.LostFocus += Txt_LostFocus;
            var scope = el.GetValue(FrameworkElement.InputScopeProperty) as InputScope;
            if (scope != null)
            {
                if ((scope.Names as List<InputScopeName>).Any(s => s.NameValue == InputScopeNameValue.EmailUserName))
                {
                    ShowKeyboard(KeyboardType.Normal);
                }
                else if ((scope.Names as List<InputScopeName>).Any(s => s.NameValue == InputScopeNameValue.Date))
                {
                    _dateKeyboard.Init();
                    ShowKeyboard(KeyboardType.Date);
                }
                else if ((scope.Names as List<InputScopeName>).Any(s => s.NameValue == InputScopeNameValue.Number))
                {

                    ShowKeyboard(KeyboardType.Number);
                }
            }
            else
            {
                ShowKeyboard(KeyboardType.Normal);
            }
        }

        private void Txt_LostFocus(object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine("Lost focus");
            var txt = sender as FrameworkElement;
            txt.LostFocus -= Txt_LostFocus;
            HideKeyboard();
        }

        internal void ToggleKeyboard()
        {
            if (IsKeyboardOpen)
            {
                HideKeyboard();
            }
            else
            {
                ShowKeyboard(KeyboardType.Normal);
            }
        }

        internal bool IsKeyboardOpen => _kbWindow.IsVisible;

        public Task Shutdown()
        {
            return Task.CompletedTask;
        }

        public async void ShowKeyboard(KeyboardType type)
        {
            void ShowKeyboard(FrameworkElement content)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _kbWindow.Keyboard = content;
                    _kbWindow.Show();
                });

            }
            switch (type)
            {
                case KeyboardType.Date:
                    ShowKeyboard(_dateKeyboard);
                    break;
                case KeyboardType.Number:
                    _visible = true;
                    if (await ShouldShowKeyboard() && _visible)
                    {
                        ShowKeyboard(_numberKeyboard);
                    }
                    break;
                default:
                    _visible = true;
                    if (await ShouldShowKeyboard() && _visible)
                    {
                        ShowKeyboard(_keyboard);
                    }
                    break;
            }
        }

        private Task<bool> ShouldShowKeyboard()
        {
            return Task.Run(() =>
            {
                using (var searcher = new ManagementObjectSearcher("Select DeviceID from Win32_Keyboard"))
                {
                    foreach (ManagementObject keyboard in searcher.Get())
                    {
                        var id = keyboard.GetPropertyValue("DeviceID").ToString();
                        if (!id.Equals("") && IgnoreKeyboards.Split(';').Any(s => id.Contains(s)))
                        {
                            return true;
                        }
                    }
                }
                return false;
            });
        }
        bool _visible = false;
        
        public event EventHandler<bool> KeyboardOpenChanged;
        public async void HideKeyboard()
        {
            _visible = false;
            await Task.Delay(400);
            if (_visible) return;
            //Debug.WriteLine("Hiding");
            Application.Current.Dispatcher.Invoke(() =>
            {
                _kbWindow.Hide();
            });
        }
    }
}
