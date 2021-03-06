﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace BatchDataEntry.Views
{
    /// <summary>
    /// Logica di interazione per NuovaColonna.xaml
    /// </summary>
    public partial class NuovaColonna : Window
    {
        public NuovaColonna()
        {
            InitializeComponent();
            CmbTable.IsEnabled = false;
        }

        private void ButtonSalvaModel_OnClicknClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void checkBoxDisableColumn_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxPartialSave.IsEnabled = true;
        }

        private void checkBoxIsPrimary_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxIsPrimary.IsEnabled = true;
            checkBoxIsSecondary.IsEnabled = false;
        }

        private void checkBoxIsPrimary_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxIsPrimary.IsEnabled = true;
            checkBoxIsSecondary.IsEnabled = true;
        }

        private void checkBoxIsSecondary_Unchecked(object sender, RoutedEventArgs e)
        {
            checkBoxIsPrimary.IsEnabled = true;
            checkBoxIsSecondary.IsEnabled = true;
        }

        private void checkBoxIsSecondary_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxIsPrimary.IsEnabled = false;
            checkBoxIsSecondary.IsEnabled = true;
        }

        private void CmbTipoCampo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(CmbTipoCampo.SelectedItem != null)
            {               
                switch (CmbTipoCampo.SelectedValue.ToString())
                {
                    case "AutocompletamentoDbSql":
                        CmbTable.IsEnabled = true;
                        cmbColumbTableSource.IsEnabled = true;
                        break;
                    default:
                        if(CmbTable != null)
                        {
                            CmbTable.IsEnabled = false;
                            CmbTable.SelectedIndex = -1;
                            CmbTable.SelectedItem = null;
                            cmbColumbTableSource.IsEnabled = false;
                            cmbColumbTableSource.SelectedIndex = -1;
                        }                      
                        break;
                }
            }
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (CmbTipoCampo == null) return;
            if(CmbTable != null && CmbTipoCampo.SelectedValue.ToString() == "Normale")
                CmbTable.IsEnabled = false;
            if(cmbColumbTableSource != null && CmbTipoCampo.SelectedValue.ToString() == "Normale")
                cmbColumbTableSource.IsEnabled = false;
        }
    }
}
