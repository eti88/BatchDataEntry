using System.Windows;
using System.Windows.Controls;
using BatchDataEntry.Models;

namespace BatchDataEntry.Helpers
{
    public class ViewerControlTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            Batch b = item as Batch;
            if(b == null) return element.FindResource("DefaultControlViewer") as DataTemplate;

            if (b.TipoFile == TipoFileProcessato.Pdf)
                return element.FindResource("PdfControlViewer") as DataTemplate;
            else if (b.TipoFile == TipoFileProcessato.Tiff)
                return element.FindResource("TiffControlViewer") as DataTemplate;
            else
                return element.FindResource("DefaultControlViewer") as DataTemplate;
        }
    }
}
