using Panacea.Modules.Keyboard.Controls;
using Panacea.Modules.Keyboard.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsInput;

namespace Panacea.Modules.Keyboard
{
    /// <summary>
    /// Interaction logic for VirtualKeyboard.xaml
    /// </summary>
    public partial class VirtualKeyboard : System.Windows.Controls.UserControl
    {
        public VirtualKeyboard(List<CultureInfo> avlanguages)
        {
            InitializeComponent();
            SetLanguages(avlanguages);
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                languages.SelectionChanged += (oo, ee) =>
                {
                    if (KbButton.IsAltGrPressed)
                    {

                        InputSimulator.SimulateKeyUp(VirtualKeyCode.RMENU);
                        InputSimulator.SimulateKeyUp(VirtualKeyCode.LCONTROL);
                    }

                    if (Environment.OSVersion.Version.Major > 6)
                    {
                        var lang = InputLanguage.InstalledInputLanguages.Cast<InputLanguage>().First(l => l.Culture.Equals(((InputLanguage)((ComboBoxItem)languages.SelectedItem).Tag).Culture));
                        var layout = KeyboardLayout.Load((InputLanguage)((ComboBoxItem)languages.SelectedItem).Tag);
                        layout.Activate();

                        InputLanguage.CurrentInputLanguage = lang;
                    }
                    InputLanguageManager.Current.CurrentInputLanguage = ((InputLanguage)((ComboBoxItem)languages.SelectedItem).Tag).Culture;

                };

                LoadXml();
                InputLanguageManager.Current.InputLanguageChanged += (oo, ee) =>
                {
                    LoadXml();
                    foreach (ComboBoxItem it in languages.Items)
                    {

                        if (InputLanguageManager.Current.CurrentInputLanguage.Name == (((InputLanguage)it.Tag).Culture).Name)
                            languages.SelectedItem = it;
                    }
                };

            }
        }

        [DllImport("user32.dll")]
        private static extern long GetKeyboardLayoutName(
          System.Text.StringBuilder pwszKLID);
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hhwnd, uint msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32.dll")]
        private static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint ags);
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        private const uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
        private const uint KLF_ACTIVATE = 1;

        public void SetLanguages(IReadOnlyList<CultureInfo> info)
        {
            languages.Items.Clear();
            var added = new Dictionary<string, CultureInfo>();
            var notadded = new Dictionary<string, CultureInfo>();
            foreach (InputLanguage lang in System.Windows.Forms.InputLanguage.InstalledInputLanguages)
            {
                try
                {
                    if (info.Any(l => l.Name == lang.Culture.Name))
                    {
                        if (added.ContainsKey(lang.Culture.NativeName)) continue;
                        languages.Items.Add(new ComboBoxItem { Content = lang.Culture.NativeName, Tag = lang });
                        added.Add(lang.Culture.NativeName, lang.Culture);
                        if (lang.Culture.Name == InputLanguageManager.Current.CurrentInputLanguage.Name)
                            languages.SelectedIndex = languages.Items.Count - 1;
                    }
                    else notadded.Add(lang.Culture.NativeName, lang.Culture);
                }
                catch (Exception ex)
                {
                    //Host.Logger.Error(this, lang?.Culture?.Name);
                }

            }
        }

        string GetPath(params string[] path)
        {
            var arr = new string[] { System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) }.Concat(path).ToArray();
            return System.IO.Path.Combine(arr);
        }

        private void LoadXml()
        {
            //MessageBox.Show(InputLanguageManager.Current.CurrentInputLanguage.Name);
            var url = GetPath("Resources", "KeyboardMappings" , InputLanguageManager.Current.CurrentInputLanguage.Name + ".xml");
            var mapp =
                MappingsSerializer.Deserialize(
                    url);
            if (mapp == null) return;
            //if (mapp.SupportsAltGr) altgr.Visibility = System.Windows.Visibility.Visible;
            //else
            altgr.Visibility = System.Windows.Visibility.Hidden;

            var children = GetLogicalChildCollection<KbButton>(this);
            foreach (KbButton btn in children)
            {

                Map m = mapp.AllMappings.Where(a => a.Key == btn.VirtualKey.ToString()).FirstOrDefault();
                if (m != null)
                {
                    if (btn.Switched) btn.Switch();
                    if (btn.FakeToggle && btn.Toggled) btn.SimulateKeyDown();
                    btn.AltCaption = m.AltGrValue;
                    btn.FakeToggle = m.FakeToggle;
                    btn.CapsLikeShift = m.CapsLikeShift;
                    if (btn.Caption != "") btn.Caption = m.Value;
                    if (m.ShiftValue != null) btn.ShiftCaption = m.ShiftValue;

                    //btn.OnVirtualKeyDown();
                    //btn.OnVirtualKeyUp();

                }
                else
                {
                    //Debugger.Break();
                }

            }
        }

        public static List<T> GetLogicalChildCollection<T>(object parent) where T : DependencyObject
        {
            var logicalCollection = new List<T>();
            GetLogicalChildCollection(parent as DependencyObject, logicalCollection);
            return logicalCollection;
        }

        private static void GetLogicalChildCollection<T>(DependencyObject parent, List<T> logicalCollection)
            where T : DependencyObject
        {
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            foreach (object child in children)
            {
                if (child is DependencyObject)
                {
                    var depChild = child as DependencyObject;
                    if (child is T)
                    {
                        logicalCollection.Add(child as T);
                    }
                    GetLogicalChildCollection(depChild, logicalCollection);
                }
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }


    internal sealed class KeyboardLayout
    {
        [DllImport("user32.dll",
           CallingConvention = CallingConvention.StdCall,
           CharSet = CharSet.Unicode,
           EntryPoint = "LoadKeyboardLayout",
           SetLastError = true,
           ThrowOnUnmappableChar = false)]
        static extern uint LoadKeyboardLayout(
           StringBuilder pwszKLID,
           uint flags);

        [DllImport("user32.dll",
            CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Unicode,
            EntryPoint = "GetKeyboardLayout",
            SetLastError = true,
            ThrowOnUnmappableChar = false)]
        static extern uint GetKeyboardLayout(
            uint idThread);

        [DllImport("user32.dll",
            CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Unicode,
            EntryPoint = "ActivateKeyboardLayout",
            SetLastError = true,
            ThrowOnUnmappableChar = false)]
        static extern uint ActivateKeyboardLayout(
            uint hkl,
            uint Flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetForegroundWindow();

        static class KeyboardLayoutFlags
        {
            public const uint KLF_ACTIVATE = 0x00000001;
            public const uint KLF_SETFORPROCESS = 0x00000100;
        }

        private readonly uint hkl;

        private KeyboardLayout(InputLanguage cultureInfo)
        {

            this.hkl = (uint)cultureInfo.Handle;
        }

        private KeyboardLayout(uint hkl)
        {
            this.hkl = hkl;
        }

        public uint Handle
        {
            get
            {
                return this.hkl;
            }
        }

        public static KeyboardLayout GetCurrent()
        {
            uint hkl = GetKeyboardLayout((uint)System.Threading.Thread.CurrentThread.ManagedThreadId);
            return new KeyboardLayout(hkl);
        }

        public static KeyboardLayout Load(InputLanguage culture)
        {
            return new KeyboardLayout(culture);
        }

        public void Activate()
        {

            ActivateKeyboardLayout(this.hkl, KeyboardLayoutFlags.KLF_SETFORPROCESS);
            PostMessage(GetForegroundWindow(), 0x0050, 0, (int)hkl);
        }
    }
}
