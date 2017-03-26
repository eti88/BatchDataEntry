
using System;
using System.Windows.Forms;
using NLog;


namespace BatchDataEntry.Views
{
    public partial class AdobeViewer : System.Windows.Forms.UserControl
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private string pdfFilePath;
        //public AxAcroPDF acrobatViewer;

        public AdobeViewer()
        {
            try
            {
                InitializeComponent();
                PdfViewer.setShowToolbar(true);
                PdfViewer.setPageMode("none");
                PdfViewer.setLayoutMode("SinglePage");
                PdfViewer.setView("Fit");
                PdfViewer.TabStop = false;
            }
            catch (Exception e)
            {
                logger.Error("[PDFCONTROL]" + e.ToString());
            }
            
        }

        public string PdfFilePath
        {
            get { return pdfFilePath; }

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
            try
            {
                PdfViewer.setShowToolbar(true);
                PdfViewer.src = PdfFilePath;              
                PdfViewer.LoadFile(PdfFilePath);

                PdfViewer.setPageMode("none");
                PdfViewer.setLayoutMode("SinglePage");
                PdfViewer.setView("Fit");
                PdfViewer.gotoFirstPage();
            }
            catch (Exception e)
            {
                logger.Error("[PDFCONTROL]" + e.ToString());
            }          
        }

        public void ScrollPdfDown()
        {
            PdfViewer.gotoNextPage();
        }

        public void ScrollPdfUp()
        {
            PdfViewer.gotoPreviousPage();
        }

        public void AdobeViewer_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
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
