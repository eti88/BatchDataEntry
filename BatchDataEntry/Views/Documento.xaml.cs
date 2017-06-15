using NLog;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using TextBox = System.Windows.Controls.TextBox;
using System.Linq;
using System.Windows.Data;

namespace BatchDataEntry.Views
{
    /// <summary>
    /// Logica di interazione per Documento.xaml
    /// </summary>
    public partial class Documento : Window
    {
        private readonly int MILLISEC_UPDATE = 1000;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public Documento()
        {
            InitializeComponent();
            int st = (Properties.Settings.Default.StartFocusCol >= 0) ? Properties.Settings.Default.StartFocusCol : 0;
            SetFocusOnSelectedTextBox(st);
        }

        public void SetFocusOnSelectedTextBox(int pos = 0)
        {
            if (FieldItems.Items == null) return;

            for (int i = 0; i < FieldItems.Items.Count; i++)
            {
                if (i == pos)
                {
                    var cnt = FieldItems.ItemContainerGenerator.ContainerFromIndex(i);
                    TextBox t2 = GetChild<TextBox>(cnt);
                    t2.Focus();
                    Keyboard.Focus(t2);
                }
            }
        }

        private void ButtonStop_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public T GetChild<T>(DependencyObject obj) where T : DependencyObject
        {
            DependencyObject child = null;
            for (Int32 i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child.GetType() == typeof(T))
                {
                    break;
                }
                else if (child != null)
                {
                    child = GetChild<T>(child);
                    if (child != null && child.GetType() == typeof(T))
                    {
                        break;
                    }
                }
            }
            return child as T;
        }

        private void DocumentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetFocusOnSelectedTextBox(10);
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        { 
            Task.Factory.StartNew(() =>
            {
              Thread.Sleep(MILLISEC_UPDATE);
                try { this.Dispatcher.BeginInvoke((Action)(() => SetFocusOnSelectedTextBox(Properties.Settings.Default.StartFocusCol))); }catch(Exception exc) { logger.Error(exc); }
                
            });            
        }

        private void buttonPrevious_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(MILLISEC_UPDATE);
                try { this.Dispatcher.BeginInvoke((Action)(() => SetFocusOnSelectedTextBox(Properties.Settings.Default.StartFocusCol))); } catch (Exception exc) { logger.Error(exc); }
            });
        }

        private void buttonIndexes_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(MILLISEC_UPDATE);
                try { this.Dispatcher.BeginInvoke((Action)(() => SetFocusOnSelectedTextBox(Properties.Settings.Default.StartFocusCol))); } catch (Exception exc) { logger.Error(exc); }
            });
        }

        // Permette di trovare l'elemento ui dentro ai ContentControl
        public T FindElementByName<T>(FrameworkElement element, string sChildName) where T : FrameworkElement
        {
            T childElement = null;
            var nChildCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < nChildCount; i++)
            {
                FrameworkElement child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                if (child == null)
                    continue;

                if (child is T && child.Name.Equals(sChildName))
                {
                    childElement = (T)child;
                    break;
                }

                childElement = FindElementByName<T>(child, sChildName);

                if (childElement != null)
                    break;
            }
            return childElement;
        }
    }   
}
