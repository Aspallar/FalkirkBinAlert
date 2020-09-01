using Microsoft.Win32;
using System.Linq;
using System.Reflection;

namespace FalkirkBinAlert
{
    internal static class AppRegistry
    {
        const string valueName = "FalkirkBins";
        const string keyName = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        public static bool RunOnStartup
        {
            get
            {
                using (RegistryKey runKey = Registry.CurrentUser.OpenSubKey(keyName, false))
                    return runKey.GetValueNames().Contains(valueName);
            }

            set
            {
                using (RegistryKey runKey = Registry.CurrentUser.OpenSubKey(keyName, true))
                {
                    if (value)
                    {
                        var assembly = Assembly.GetEntryAssembly();
                        runKey.SetValue(valueName, assembly.Location);
                    }
                    else
                    {
                        runKey.DeleteValue(valueName, false);
                    }
                }
            }
        }
    }
}
