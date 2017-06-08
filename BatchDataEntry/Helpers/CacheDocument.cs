using BatchDataEntry.Components;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;

namespace BatchDataEntry.Helpers
{
    public class CacheDocumentSender
    {
        public event DocChangeEventHandler DocumentChanged;

        public CacheDocumentSender() { }

        protected virtual void OnDocumentChanged(CacheEventArgs e)
        {
            if (DocumentChanged != null) DocumentChanged(this, e);
        }
    }

    public class CacheDocumentReceiver
    {
        CacheDocumentSender docsender;
        private string TMP_PATH;
        private CircularBuffer<string> cbuffer;

        public CacheDocumentReceiver()
        {
            TMP_PATH = Path.Combine(Path.GetTempPath(), "_batchtmp");
            if (!Directory.Exists(TMP_PATH)) 
                Directory.CreateDirectory(TMP_PATH);
            else
                CleanTmpDir();

            cbuffer = new CircularBuffer<string>(10);
            docsender = new CacheDocumentSender();

            // Directory watcher
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = TMP_PATH;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.*";
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;
        }

        public void Connect()
        {
            docsender.DocumentChanged += new DocChangeEventHandler(this.sender_DocChanged);
        }

        private void sender_DocChanged(object sender, CacheEventArgs e)
        {
            foreach (string x in e.Files)
                cbuffer.Enqueue(x);

            Thread instanceConverter = new Thread(() => ConvertFiles(e.Files));
            instanceConverter.Start();
            instanceConverter.Join();
        }

        /// <summary>
        ///  Ogni volta che viene aggiunto un file nella directory temporanea
        ///  controlla se bisogna eliminare qualche file pdf generato in precedenza
        ///  cioè i file che non sono più presenti nel buffer circolare.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            CleanTmpDir();
        }

        private void CleanTmpDir()
        {
            if (cbuffer == null) return;
            string[] fileArray = Directory.GetFiles(TMP_PATH, "*.pdf");
            if (fileArray.Count() == 0) return;
            foreach (string element in fileArray)
            {
                if (!cbuffer.Contains(element))
                    File.Delete(element);
            }
        }

        public void FireDocChanged(List<string> files) {
            if (files != null)
                this.sender_DocChanged(this, new CacheEventArgs(files));
        }

        public void ConvertTiff(string filepath)
        {
            string filetmp = string.Format("{0}.pdf", Path.Combine(TMP_PATH, Path.GetFileNameWithoutExtension(filepath)));
            try
            {
                Document document = new Document();
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filetmp, FileMode.Create));

                Bitmap bmp = new Bitmap(filepath);
                int total = bmp.GetFrameCount(System.Drawing.Imaging.FrameDimension.Page);

                document.Open();
                PdfContentByte cb = writer.DirectContent;
                for (int k = 0; k < total; ++k)
                {
                    bmp.SelectActiveFrame(System.Drawing.Imaging.FrameDimension.Page, k);
                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(bmp, System.Drawing.Imaging.ImageFormat.Bmp);
                    img.ScalePercent(72f / img.DpiX * 100);
                    img.SetAbsolutePosition(0, 0);
                    cb.AddImage(img);
                    document.NewPage();
                }

                document.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public void ConvertFiles(List<string> files)
        {
            foreach(string f in files)
            {
                ConvertTiff(f);
            }
        }

        public string PrintTmpDir()
        {
            return this.TMP_PATH;
        }

        public string TempFilePath(string file)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            return Path.Combine(TMP_PATH, string.Format("{0}.pdf", name));
        }
    }

    public delegate void DocChangeEventHandler(object sender, CacheEventArgs e);

    public class CacheEventArgs : EventArgs
    {
        private readonly List<string> files_path;
        public List<string> Files { get { return files_path; } }

        public CacheEventArgs(List<string> x)
        {
            this.files_path = x;
        }
    }
}
