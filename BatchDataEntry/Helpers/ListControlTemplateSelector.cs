using System.Windows;
using System.Windows.Controls;
using BatchDataEntry.Models;

namespace BatchDataEntry.Helpers
{
    class ListControlTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            Voce voce = item as Voce;
            if (voce.IsAutocomplete && voce.AUTOCOMPLETETYPE.Equals("CSV"))
            {
                return element.FindResource("AutocompleteCsvDataTemplate") as DataTemplate;
            }else if (voce.IsAutocomplete && voce.AUTOCOMPLETETYPE.Equals("DB"))
            {
                return element.FindResource("AutocompleteDbDataTemplate") as DataTemplate;
            }
            else
            {
                return element.FindResource("TextBoxDataTemplate") as DataTemplate;
            }

        }
    }
}
