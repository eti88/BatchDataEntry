using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using TextBox = System.Windows.Controls.TextBox;

namespace BatchDataEntry.Views
{
    /// <summary>
    /// Logica di interazione per Documento.xaml
    /// </summary>
    public partial class Documento : Window
    {
        private readonly int MILLISEC_UPDATE = 1000;

        public Documento()
        {
            InitializeComponent();
            SetFocusOnSelectedTextBox(10);
        }

        public void SetFocusOnSelectedTextBox(int pos)
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
                this.Dispatcher.BeginInvoke((Action) (() => SetFocusOnSelectedTextBox(Properties.Settings.Default.StartFocusCol)));
            });            
        }

        private void buttonPrevious_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(MILLISEC_UPDATE);
                this.Dispatcher.BeginInvoke((Action)(() => SetFocusOnSelectedTextBox(Properties.Settings.Default.StartFocusCol)));
            });
        }

        private void buttonIndexes_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(MILLISEC_UPDATE);
                this.Dispatcher.BeginInvoke((Action)(() => SetFocusOnSelectedTextBox(Properties.Settings.Default.StartFocusCol)));
            });
        }
    }   
}
