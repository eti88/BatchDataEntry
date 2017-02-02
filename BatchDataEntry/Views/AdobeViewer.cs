using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Windows.Forms;


namespace BatchDataEntry.Views
{
    public partial class AdobeViewer : UserControl
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
    }
}
