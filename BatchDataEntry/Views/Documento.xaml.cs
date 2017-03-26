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

        public Documento()
        {
            InitializeComponent();      
        }

        public void SetFocusOnSelectedTextBox(int pos)
        {
            if(FieldItems.Items == null) return;
            
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
    }   
}
