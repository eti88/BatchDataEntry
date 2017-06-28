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
using System.ComponentModel;
using System.Windows.Threading;
using System.Collections.Generic;
using BatchDataEntry.Models;
using WpfControls.Editors;

namespace BatchDataEntry.Views
{
    /// <summary>
    /// Logica di interazione per Documento.xaml
    /// </summary>
    public partial class Documento : Window
    {
        private readonly int MILLISEC_UPDATE = 800;
        private static Logger logger = LogManager.GetCurrentClassLogger();
        BackgroundWorker bw;
        private int st;
        private List<Concatenation> _concatenations;

        public Documento()
        {
            InitializeComponent();
            st = (Properties.Settings.Default.StartFocusCol >= 0) ? Properties.Settings.Default.StartFocusCol : 0;
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = false;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }

        public Documento(List<Concatenation> concatenations)
        {
            InitializeComponent();
            st = (Properties.Settings.Default.StartFocusCol >= 0) ? Properties.Settings.Default.StartFocusCol : 0;
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = false;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

            if (concatenations == null)
                _concatenations = new List<Concatenation>();
            else
                _concatenations = concatenations;
        }

        public void SetFocusOnSelectedTextBox(int pos = 0)
        {
            if (FieldItems.Items == null) return;
            if (pos < 0) return;
            for (int i = 0; i < FieldItems.Items.Count; i++)
            {
                //  Il focus è abilitato solo per i bottoni (indicizzazione, navigazione e stop) e per i campi definiti dal modello
                if (i == pos)
                {
                    ContentPresenter cp = FieldItems.ItemContainerGenerator.ContainerFromIndex(pos) as ContentPresenter;
                    TextBox tb = FindVisualChild<TextBox>(cp);
                    if (tb != null)
                    {
                        tb.Focus();
                    }
                    break;
                }
            }
        }

        private void ButtonStop_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public static T FindVisualChild<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        return (T)child;
                    }

                    T childItem = FindVisualChild<T>(child);
                    if (childItem != null) return childItem;
                }
            }
            return null;
        }

        private void DocumentWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync(st);
            }
        }

        private void buttonNext_Click(object sender, RoutedEventArgs e)
        { 
            Task.Factory.StartNew(() =>
            {
              Thread.Sleep(MILLISEC_UPDATE);
                try {
                    this.Dispatcher.BeginInvoke((Action)(() => SetFocusOnSelectedTextBox(Properties.Settings.Default.StartFocusCol)));
                } catch(Exception exc) {
                    logger.Error(exc);
                }

                if (bw.IsBusy != true)
                {
                    bw.RunWorkerAsync(st);
                }
            });            
        }

        private void buttonPrevious_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(MILLISEC_UPDATE);
                try {
                    this.Dispatcher.BeginInvoke((Action)(() => SetFocusOnSelectedTextBox(Properties.Settings.Default.StartFocusCol)));
                } catch (Exception exc) {
                    Console.WriteLine(exc);
                }

                if (bw.IsBusy != true)
                {
                    bw.RunWorkerAsync(st);
                }
            });
        }

        private void buttonIndexes_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(MILLISEC_UPDATE);
                try {
                    this.Dispatcher.BeginInvoke((Action)(() => SetFocusOnSelectedTextBox(Properties.Settings.Default.StartFocusCol)));
                } catch (Exception exc) {
                    logger.Error(exc);
                }

                if (bw.IsBusy != true)
                {
                    bw.RunWorkerAsync(st);
                }
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

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            int val = (int) e.Argument;
            if(val == st)
                Thread.Sleep(MILLISEC_UPDATE);
            e.Result = val;
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            int steps = (int)e.Result;

            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                new Action(delegate ()
                {
                    SetFocusOnSelectedTextBox(steps);
                }));
        }

        private void AutoCompleteTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (bw == null) return;
            if (FieldItems.Items == null) return;

            if (bw.IsBusy != true)
            {
                if (_concatenations != null && _concatenations.Count > 0)
                {
                    // confrontare concatenazioni con box corrente per sapere la sua posizione (tag) e pescare la concatenazione correlata
                    AutoCompleteTextBox tbox = sender as AutoCompleteTextBox;
                    if (tbox == null) return;
                    foreach (Concatenation concatenation in _concatenations)
                    {
                        concatenation.InitPositions();
                        int position = Convert.ToInt32(tbox.Tag);
                        if (concatenation.Positions.Contains(position))
                        {
                            #if DEBUG
                            Console.WriteLine(string.Format("Set focus on {0} position", concatenation.END_POS));
                            #endif
                            bw.RunWorkerAsync(concatenation.END_POS + 1);
                            e.Handled = true;
                            break;
                        }
                    }
                }
            }
        }
    }   
}
