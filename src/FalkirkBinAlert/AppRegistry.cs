using Microsoft.Win32;
using System.Reflection;

namespace FalkirkBinAlert
{
    internal static class AppRegistry
    {
        public static bool RunOnStartup
        {
            set
            {
                const string valueNamew = "FalkirkBins";

                using (RegistryKey runKey = Registry.CurrentUser.OpenSubKey
                    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    if (value)
                    {
                        var assembly = Assembly.GetEntryAssembly();
                        runKey.SetValue(valueNamew, assembly.Location);
                    }
                    else
                    {
                        runKey.DeleteValue(valueNamew, false);
                    }
                }
            }
        }
    }
}
