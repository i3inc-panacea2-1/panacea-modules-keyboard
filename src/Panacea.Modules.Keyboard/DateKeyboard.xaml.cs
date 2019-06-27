using Panacea.Controls;
using Panacea.Multilinguality;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace Panacea.Modules.Keyboard
{
    /// <summary>
    /// Interaction logic for DateKeyboard.xaml
    /// </summary>
    public partial class DateKeyboard : UserControl
    {
        public DateKeyboard()
        {
            Items = new ObservableCollection<DayInfo>();
            DataContext = this;
            SelectYearCommand = new RelayCommand(args =>
            {
                if (current == 0)
                {
                    var year = (int)args;
                    selectedDate = selectedDate == null ? new DateTime(year, 1, 1) : new DateTime(year, selectedDate.Value.Month, selectedDate.Value.Day);
                    FillMonths();
                }
                else if (current == 1)
                {
                    var month = (int)args;
                    selectedDate = new DateTime(selectedDate.Value.Year, month, 1);
                    FillDays();
                }
                else if (current == 2)
                {
                    var day = (int)args;
                    selectedDate = new DateTime(selectedDate.Value.Year, selectedDate.Value.Month, day);
                    var target = System.Windows.Input.Keyboard.FocusedElement;
                    if (target is TextBox)
                    {
                        ((TextBox)target).Text = selectedDate.Value.ToString(LanguageContext.Instance.Culture.DateTimeFormat.LongDatePattern, LanguageContext.Instance.Culture);
                        ((TextBox)target).Tag = selectedDate.Value;
                    }
                    var tRequest = new TraversalRequest(FocusNavigationDirection.Next);
                    var keyboardFocus = System.Windows.Input.Keyboard.FocusedElement as UIElement;

                    if (keyboardFocus != null)
                    {
                        keyboardFocus.MoveFocus(tRequest);
                    }
                }
            });
            InitializeComponent();
        }
        int yearsPerPage = 30;
        DateTime? selectedDate;
        int current = 0;
        int page = 0;


        public int GetDecade(int year)
        {
            return year / 100 * 100;
        }
        public void Init()
        {
            page = 0;
            selectedDate = null;
            FillYears();
        }
        public void FillYears()
        {
            current = 0;
            Items.Clear();
            for (int i = GetDecade(DateTime.Now.Year) - (page * yearsPerPage) - yearsPerPage; i < GetDecade(DateTime.Now.Year) - (page * yearsPerPage); i++)
            {
                Items.Add(new DayInfo() { Label = i.ToString(), Number = i });

            }
        }

        public void FillMonths()
        {
            current = 1;
            Items.Clear();
            for (int i = 1; i < 13; i++)
            {
                Items.Add(new DayInfo() { Label = new DateTime(2010, i, 1).ToString("MMM", LanguageContext.Instance.Culture), Number = i });
            }
        }

        public ObservableCollection<DayInfo> Items { get; set; }
        public void FillDays()
        {
            current = 2;
            Items.Clear();
            for (int i = 1; i <= System.DateTime.DaysInMonth(selectedDate.Value.Year, selectedDate.Value.Month); i++)
            {
                Items.Add(new DayInfo() { Label = i.ToString(), Number = i });
            }
        }

        private void prev_Click(object sender, RoutedEventArgs e)
        {
            if (current == 0)
            {
                page += 1;
                FillYears();
            }
            else if (current == 1)
            {
                FillYears();
            }
            else
            {
                FillMonths();
            }
        }

        private void next_Click(object sender, RoutedEventArgs e)
        {
            if (current == 0)
            {
                page -= 1;
                FillYears();
            }
            else if (current == 1)
            {
                FillDays();
            }
            else
            {

            }
        }

        public RelayCommand SelectYearCommand { get; }

        public class DayInfo
        {
            public string Label { get; set; }

            public int Number { get; set; }
        }
    }
}
