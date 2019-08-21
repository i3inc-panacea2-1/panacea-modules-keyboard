using Panacea.Controls;
using Panacea.Modules.Keyboard.Views;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Panacea.Modules.Keyboard.ViewModels
{
    [View(typeof(NavigationButton))]
    class NavigationButtonViewModel : ViewModelBase
    {
        bool _keyboardVisible;
        public bool KeyboardVisible
        {
            get => _keyboardVisible;
            set
            {
                _keyboardVisible = value;
                OnPropertyChanged();
            }
        }

        public override void Activate()
        {
            base.Activate();
            _plugin.KeyboardOpenChanged += _plugin_KeyboardOpenChanged;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            _plugin.KeyboardOpenChanged -= _plugin_KeyboardOpenChanged;
        }

        private void _plugin_KeyboardOpenChanged(object sender, bool e)
        {
            KeyboardVisible = e;
        }

        public NavigationButtonViewModel(KeyboardPlugin plugin)
        {
            _plugin = plugin;
            ClickCommand = new RelayCommand(args =>
            {
                plugin.ToggleKeyboard();
                
            });
        }

        private readonly KeyboardPlugin _plugin;

        public ICommand ClickCommand { get; }
    }
}
