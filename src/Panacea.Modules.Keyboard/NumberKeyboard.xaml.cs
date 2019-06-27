using System;
using System.Collections.Generic;
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
using WindowsInput;

namespace Panacea.Modules.Keyboard
{
    /// <summary>
    /// Interaction logic for NumberKeyboard.xaml
    /// </summary>
    public partial class NumberKeyboard : UserControl
    {
        public NumberKeyboard()
        {
            InitializeComponent();
        }

       
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void MixButtons()
        {
            /*
            points = new List<Point>
            {
                new Point(0, 0),
                new Point(0, 1),
                new Point(0, 2),
                new Point(1, 0),
                new Point(1, 1),
                new Point(1, 2),
                new Point(2, 0),
                new Point(2, 1),
                new Point(2, 2),
                new Point(3, 1)
            };
            var r = new Random(DateTime.Now.Millisecond);
            int i = 0;
            while (points.Count > 0)
            {
                int ind = r.Next(0, points.Count - 1);
                Point p = points[ind];
                points.Remove(p);
                ((Button)FindName("b" + i)).SetValue(Grid.RowProperty, (int)p.X);
                ((Button)FindName("b" + i)).SetValue(Grid.ColumnProperty, (int)p.Y);
                i++;
            }*/
        }
  
      
        private void b1_Click(object sender, RoutedEventArgs e)
        {
            var b = (Button)sender;
            string s = b.Content.ToString();
            if (b == bBack) s = "Back";
            var k = new KeyConverter();
            var mykey = VirtualKeyCode.BACK;
            if (b != bBack)
                mykey = (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), "VK_" + s);
            InputSimulator.SimulateKeyPress(mykey);
        }
    }
}
