using Panacea.Core;
using Panacea.Modularity;
using Panacea.Modularity.UiManager;
using Panacea.Modules.Keyboard.Models;
using Panacea.Modules.Keyboard.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Panacea.Modules.Keyboard
{
    public class KeyboardPlugin : IPlugin
    {
        KeyboardWindow _kbWindow;
        VirtualKeyboard _keyboard;
        DateKeyboard _dateKeyboard;
        NumberKeyboard _numberKeyboard;
        NavigationButtonViewModel _navButton;
        private readonly PanaceaServices _core;

        public KeyboardPlugin(PanaceaServices core)
        {
            _core = core;
        }

        public Task BeginInit()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }

        public async Task EndInit()
        {
            List<Language> languages = null;
            var res = await _core.HttpClient.GetObjectAsync<GetVersionsResponse>("get_versions/");
            if (res.Success)
            {
                languages = res.Result.InputLanguages;
            }
            else
            {
                throw new Exception(res.Error);
            }
            _kbWindow = new KeyboardWindow();
            _keyboard = new VirtualKeyboard(languages.Select(l=> new System.Globalization.CultureInfo(l.Code)).ToList());
            _dateKeyboard = new DateKeyboard();
            _numberKeyboard = new NumberKeyboard();
            if(_core.TryGetUiManager(out IUiManager ui))
            {
                _navButton = new NavigationButtonViewModel(this);
                ui.AddNavigationBarControl(_navButton);
            }
           

            //ShowKeyboard(_keyboard);
            //return Task.CompletedTask;
            EventManager.RegisterClassHandler(typeof(UIElement), System.Windows.Input.Keyboard.PreviewGotKeyboardFocusEvent,
                (KeyboardFocusChangedEventHandler)OnPreviewGotKeyboardFocus);

        }

        private void OnPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Debug.WriteLine("GotFocus");
            var txt = e.NewFocus as TextBoxBase;
            var pass = e.NewFocus as PasswordBox;
            if (txt != null)
            {
                HandleElement(txt);
            }
            else if(pass != null)
            {
                HandleElement(pass);
            }
            else
            {
                
                HideKeyboard();
            }
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
                    ShowKeyboard(_keyboard);
                }
                else if ((scope.Names as List<InputScopeName>).Any(s => s.NameValue == InputScopeNameValue.Date))
                {
                    _dateKeyboard.Init();
                    ShowKeyboard(_dateKeyboard);
                }
                else if ((scope.Names as List<InputScopeName>).Any(s => s.NameValue == InputScopeNameValue.Number))
                {

                    ShowKeyboard(_numberKeyboard);
                }
            }
            else
            {
                ShowKeyboard(_keyboard);
            }
        }

        private void Txt_LostFocus(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Lost focus");
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
                ShowKeyboard(_keyboard);
            }
        }

        internal bool IsKeyboardOpen => _kbWindow.IsVisible;

        internal void ShowKeyboard(FrameworkElement content)
        {
            Debug.WriteLine("Showing");
            Application.Current.Dispatcher.Invoke(() =>
            {
                _kbWindow.Keyboard = content;
                _kbWindow.Show();
            });
        }

        internal void HideKeyboard()
        {
            Debug.WriteLine("Hiding");
            Application.Current.Dispatcher.Invoke(() =>
            {
                _kbWindow.Hide();
            });
        }

        public Task Shutdown()
        {
            return Task.CompletedTask;
        }
    }
}
