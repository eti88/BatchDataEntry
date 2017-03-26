using System.Windows;
using System.Windows.Forms.Integration;
using BatchDataEntry.Views;

namespace BatchDataEntry.Components
{
    public class PdfViewerHost : WindowsFormsHost
    {
        public static readonly DependencyProperty PdfPathProperty = DependencyProperty.Register(
  "PdfPath", typeof(string), typeof(PdfViewerHost), new PropertyMetadata(PdfPathPropertyChanged));

       // public static readonly DependencyProperty PdfScrollUpProperty = DependencyProperty.Register("PdfScrlUp", typeof());

        private readonly AdobeViewer wrappedControl;

        public PdfViewerHost()
        {
            wrappedControl = new AdobeViewer();
            Child = wrappedControl;
            wrappedControl.TabStop = false;
        }

        public string PdfPath
        {
            get
            {
                return (string)GetValue(PdfPathProperty);
            }

            set
            {
                SetValue(PdfPathProperty, value);
            }
        }

        public void Print()
        {
            wrappedControl.Print();
        }

        private static void PdfPathPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PdfViewerHost host = (PdfViewerHost)d;
            host.wrappedControl.PdfFilePath = (string)e.NewValue;
        }
    }
}
