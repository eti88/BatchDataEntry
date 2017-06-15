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

        public Documento()
        {
            InitializeComponent();
            st = (Properties.Settings.Default.StartFocusCol >= 0) ? Properties.Settings.Default.StartFocusCol : 0;
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = false;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
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
                        Console.WriteLine("Textbox pos: " + i);
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
                bw.RunWorkerAsync();
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
                    bw.RunWorkerAsync();
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
                    bw.RunWorkerAsync();
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
                    bw.RunWorkerAsync();
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
            Thread.Sleep(MILLISEC_UPDATE);
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
                new Action(delegate ()
                {
                    SetFocusOnSelectedTextBox(st);
                }));
        }
    }   
}
