namespace BatchDataEntry.Views
{
    partial class AdobeViewer
    {
        /// <summary> 
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione componenti

        /// <summary> 
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare 
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdobeViewer));
            this.PdfViewer = new AxAcroPDFLib.AxAcroPDF();
            ((System.ComponentModel.ISupportInitialize)(this.PdfViewer)).BeginInit();
            this.SuspendLayout();
            // 
            // PdfViewer
            // 
            this.PdfViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PdfViewer.Enabled = true;
            this.PdfViewer.Location = new System.Drawing.Point(0, 0);
            this.PdfViewer.Name = "PdfViewer";
            this.PdfViewer.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("PdfViewer.OcxState")));
            this.PdfViewer.Size = new System.Drawing.Size(367, 386);
            this.PdfViewer.TabIndex = 0;
            this.PdfViewer.TabStop = false;
            // 
            // AdobeViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.PdfViewer);
            this.Name = "AdobeViewer";
            this.Size = new System.Drawing.Size(367, 386);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AdobeViewer_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.PdfViewer)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxAcroPDFLib.AxAcroPDF PdfViewer;
    }
}
