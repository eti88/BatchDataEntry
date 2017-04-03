using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NLog;

namespace BatchDataEntry
{
    /// <summary>
    /// Logica di interazione per App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // UI Exceptions
            this.DispatcherUnhandledException += Application_DispatcherUnhandledException;

            // Thread exceptions
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            var exception = e.Exception;
            HandleUnhandledException(exception);
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            HandleUnhandledException(unhandledExceptionEventArgs.ExceptionObject as Exception);
            if (unhandledExceptionEventArgs.IsTerminating)
            {
                _logger.Info("Application is terminating due to an unhandled exception in a secondary thread.");
            }
        }

        private void HandleUnhandledException(Exception exception)
        {
            string message = "Unhandled exception";
            try
            {
                AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
                message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
            }
            catch (Exception exc)
            {
                _logger.Error(exc, "Exception in unhandled exception handler");
            }
            finally
            {
                _logger.Error(exception.ToString());
            }
        }

    }
}
