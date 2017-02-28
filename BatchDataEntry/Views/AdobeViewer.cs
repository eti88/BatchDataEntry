
using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;


namespace BatchDataEntry.Views
{
    public partial class AdobeViewer : System.Windows.Forms.UserControl
    {
        private string pdfFilePath;
        //public AxAcroPDF acrobatViewer;

        public AdobeViewer()
        {
            InitializeComponent();
            PdfViewer.setShowToolbar(false);
            PdfViewer.setView("FitH");
        }

        public string PdfFilePath
        {
            get
            {
                return pdfFilePath;
            }

            set
            {
                if (pdfFilePath != value)
                {
                    pdfFilePath = value;
                    ChangeCurrentDisplayedPdf();
                }
            }
        }

        public void Print()
        {
            PdfViewer.printWithDialog();
        }

        private void ChangeCurrentDisplayedPdf()
        {
            PdfViewer.LoadFile(PdfFilePath);
            PdfViewer.src = PdfFilePath;
            PdfViewer.setViewScroll("FitH", 0);
        }

        public void ScrollPdfDown()
        {
            PdfViewer.gotoNextPage();
        }

        public void ScrollPdfUp()
        {
            PdfViewer.gotoPreviousPage();
        }

        private void AdobeViewer_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PageUp)
            {
                ScrollPdfUp();
            }
            else if (e.KeyCode == Keys.PageDown)
            {
                ScrollPdfDown();
            }
        }
    }
}
