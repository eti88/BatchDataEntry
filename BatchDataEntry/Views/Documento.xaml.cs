using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BatchDataEntry.Models;
using ComboBox = System.Windows.Controls.ComboBox;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using Label = System.Windows.Controls.Label;
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

        public void SetFocusOnPdfControl()
        {
            FocusManager.SetFocusedElement(DocumentWindow, PdfViewerUc);

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

        private void DocumentWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F2)
                SetFocusOnPdfControl();
            else if (e.Key == Key.F3)
            {
                int startCol = Properties.Settings.Default.StartFocusCol;
                SetFocusOnSelectedTextBox(startCol);
            }else if (e.Key == Key.F5)
                this.PdfViewerUc.IsEnabled = !PdfViewerUc.IsEnabled;
                
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

        private void FieldItems_Loaded(object sender, RoutedEventArgs e)
        {
            HandleAcrobatBitchFocus();
        }

        private void HandleAcrobatBitchFocus()
        {
            Console.WriteLine("StartHandleBitch");
            PdfViewerUc.IsEnabled = false;
            int startCol = Properties.Settings.Default.StartFocusCol;

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(700);
                Dispatcher.Invoke(() =>
                {
                    PdfViewerUc.IsEnabled = true;
                });
            });

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(500);
                Dispatcher.Invoke(() =>
                {
                    SetFocusOnSelectedTextBox(startCol);
                });
            });
        }

        private void buttonIndexes_Click(object sender, RoutedEventArgs e)
        {
            HandleAcrobatBitchFocus();
        }

        private void buttonPrevious_Click(object sender, RoutedEventArgs e)
        {
            HandleAcrobatBitchFocus();
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        {
            HandleAcrobatBitchFocus();
        }
    }   
}
