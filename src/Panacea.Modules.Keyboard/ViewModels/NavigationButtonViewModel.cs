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
        public NavigationButtonViewModel(KeyboardPlugin plugin)
        {
            ClickCommand = new RelayCommand(args =>
            {
                plugin.ToggleKeyboard();
            });
        }

        public ICommand ClickCommand { get; }
    }
}
