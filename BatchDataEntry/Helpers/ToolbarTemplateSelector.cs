using BatchDataEntry.Models;
using System.Windows;
using System.Windows.Controls;

namespace BatchDataEntry.Helpers
{
    public class ToolbarTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            Batch b = item as Batch;
            if (b == null) return element.FindResource("ToolbarDefaultCtrl") as DataTemplate;

            if (b.TipoFile == TipoFileProcessato.Pdf)
                return element.FindResource("ToolbarPdfCtrl") as DataTemplate;
            else if (b.TipoFile == TipoFileProcessato.Tiff)
                return element.FindResource("ToolbarTiffCtrl") as DataTemplate;
            else
                return element.FindResource("ToolbarDefaultCtrl") as DataTemplate;
        }
    }
}
