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

            Campo voce = item as Campo;
            if (voce.TipoCampo == EnumTypeOfCampo.AutocompletamentoCsv)
                return element.FindResource("AutocompleteCsvDataTemplate") as DataTemplate;
            else if (voce.TipoCampo == EnumTypeOfCampo.AutocompletamentoDbSqlite)
                return element.FindResource("AutocompleteDbDataTemplate") as DataTemplate;
            else if (voce.TipoCampo == EnumTypeOfCampo.AutocompletamentoDbSql)
                return element.FindResource("AutocompleteDbSqlDataTemplate") as DataTemplate;
            else if(voce.TipoCampo == EnumTypeOfCampo.Data)
                return element.FindResource("TextBoxDataFormatTemplate") as DataTemplate;
            else
                return element.FindResource("TextBoxDataTemplate") as DataTemplate;
        }
    }
}
