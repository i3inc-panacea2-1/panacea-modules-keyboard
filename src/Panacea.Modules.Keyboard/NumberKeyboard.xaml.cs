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

        public event EventHandler<string> KeyPressed;
        public event EventHandler Close;

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
        public void BindTo(params object[] controls)
        {
            KeyPressed += (oo, ee) =>
            {
                RoutedEvent routedEvent = TextCompositionManager.TextInputEvent;
                var k = new KeyConverter();
                var mykey = Key.Back;
                if (oo != bBack)
                    mykey = (Key)k.ConvertFromString("D" + ee);
                foreach (object o in controls)
                {
                    if (mykey != Key.Back)
                    {
                        (o as IInputElement).RaiseEvent(new TextCompositionEventArgs(
                            InputManager.Current.PrimaryKeyboardDevice,
                            new TextComposition(InputManager.Current, o as IInputElement, ee))
                        {
                            RoutedEvent = routedEvent
                        }
                            );
                    }
                    else
                    {
                        RoutedEvent routedEvent2 = System.Windows.Input.Keyboard.KeyDownEvent; // Event to send
                        if ((o as Control).IsFocused)
                        {
                            (o as IInputElement).RaiseEvent(
                                new KeyEventArgs(
                                    System.Windows.Input.Keyboard.PrimaryDevice,
                                    PresentationSource.FromVisual((o as Visual)),
                                    0,
                                    mykey)
                                { RoutedEvent = routedEvent2 }
                                );
                        }
                    }
                }

            };
        }

        private void BtnHide_OnClick(object sender, RoutedEventArgs e)
        {
            Close?.Invoke(this, null);
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
