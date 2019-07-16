using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Panacea.Modules.Keyboard.Views
{
    /// <summary>
    /// Interaction logic for LanguageButton.xaml
    /// </summary>
    public partial class LanguageButton : UserControl
    {
        public LanguageButton()
        {
            InitializeComponent();
            popup.CustomPopupPlacementCallback = new CustomPopupPlacementCallback(PlacePopup);
        }
        public CustomPopupPlacement[] PlacePopup(Size popupSize,
                                        Size targetSize,
                                        Point offset)
        {
            var p = LanguageButtonBtn.PointToScreen(new Point(0, 0));
            var placement1 =
                new CustomPopupPlacement(new Point(p.X + LanguageButtonBtn.ActualWidth / 2 - popupSize.Width / 2, p.Y - popupSize.Height), PopupPrimaryAxis.Vertical);

            var placement2 =
                new CustomPopupPlacement(new Point(10, 20), PopupPrimaryAxis.Horizontal);

            var ttplaces =
                    new CustomPopupPlacement[] { placement1 };
            return ttplaces;
        }
    }
    public class HeightConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value * 1.8;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
