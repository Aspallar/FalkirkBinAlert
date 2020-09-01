using ControlzEx.Theming;
using System.Net;
using System.Windows;

namespace FalkirkBinAlert
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.Expect100Continue = true;
            base.OnStartup(e);
            var settings = FalkirkBinAlert.Properties.Settings.Default;
            ThemeManager.Current.ChangeTheme(this, settings.Theme);
        }
    }
}
