using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Panacea.Modules.Keyboard.Views
{
    /// <summary>
    /// Interaction logic for NavigationButton.xaml
    /// </summary>
    public partial class NavigationButton : UserControl
    {
        public NavigationButton()
        {
            InitializeComponent();
        }
    }

    class NotBooleanToVisibilityConverter : IValueConverter
    {
        BooleanToVisibilityConverter _converter = new BooleanToVisibilityConverter();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _converter.Convert(!(bool)value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
