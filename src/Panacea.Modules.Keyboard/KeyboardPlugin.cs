using Panacea.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;

namespace Panacea.Modules.Keyboard
{
    public class KeyboardPlugin : IPlugin
    {
        KeyboardWindow _kbWindow;
        public Task BeginInit()
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {

        }

        public Task EndInit()
        {
            _kbWindow = new KeyboardWindow();
            Automation.AddAutomationFocusChangedEventHandler(OnFocusChanged);
            return Task.CompletedTask;
        }

        private void OnFocusChanged(object sender, AutomationFocusChangedEventArgs e)
        {
            var el = AutomationElement.FocusedElement;
            if (el.Current.ControlType == ControlType.Edit)
            {
                ShowKeyboard();
            }
            else
            {
                HideKeyboard();
            }

        }

        void ShowKeyboard()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _kbWindow.Show();
            }));
        }

        void HideKeyboard()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                _kbWindow.Hide();
            }));
        }

        public Task Shutdown()
        {
            return Task.CompletedTask;
        }
    }
}
