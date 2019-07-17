using Panacea.Controls;
using Panacea.Modularity.UiManager;
using Panacea.Modules.Keyboard.Models;
using Panacea.Modules.Keyboard.Views;
using Panacea.Multilinguality;
using Panacea.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.Keyboard.ViewModels
{
    [View(typeof(LanguageButton))]
    class LanguageButtonViewModel : SettingsControlViewModelBase
    {
        bool _popOpen;
        public bool PopupOpen
        {
            get => _popOpen;
            set
            {
                _popOpen = value;
                OnPropertyChanged();
            }
        }
        private Language _selectedLanguage;
        public Language SelectedLanguage {
            get => _selectedLanguage;
            set {
                if (value == null) return;
                _selectedLanguage = value;
                OnPropertyChanged();
                LanguageContext.Instance.Culture = new CultureInfo(value?.Code);
                PopupOpen = false;
            }
        }
        public List<Language> Languages { get; set; }
        public LanguageButtonViewModel(List<Language> _languages)
        {
            Languages = _languages;
            var initialLanguage = Languages.FirstOrDefault(l => l.Code == LanguageContext.Instance.Culture.Name);
            if (initialLanguage != null)
            {
                SelectedLanguage = initialLanguage;
            }
            ClickCommand = new RelayCommand(args =>
            {
                PopupOpen = !PopupOpen;
            },args => !PopupOpen);
        }
        public RelayCommand ClickCommand { get; }
    }
}
