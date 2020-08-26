using ControlzEx.Theming;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FalkirkBinAlert
{
    /// <summary>
    /// Interaction logic for MetroStylePicker.xaml
    /// </summary>
    public partial class MetroThemePicker : UserControl
    {
        private readonly List<string> themeTypes = new List<string> { "Light", "Dark" };

        private readonly List<string> themeColors = new List<string> {
            "Red", "Green", "Blue", "Purple", "Orange", "Lime", "Emerald",
            "Teal", "Cyan", "Cobalt", "Indigo", "Violet", "Pink", "Magenta",
            "Crimson", "Amber", "Yellow", "Brown", "Olive", "Steel", "Mauve",
            "Taupe", "Sienna"
        };

        public MetroThemePicker()
        {
            InitializeComponent();
            SetDataSources();
            SetInitialSelections();
            WireEvents();
        }

        private void WireEvents()
        {
            ThemeType.SelectionChanged += Theme_SelectionChanged;
            ThemeColor.SelectionChanged += Theme_SelectionChanged;
        }

        private void SetDataSources()
        {
            themeColors.Sort();
            ThemeType.ItemsSource = themeTypes;
            ThemeColor.ItemsSource = themeColors;
        }

        private void SetInitialSelections()
        {
            // Using app settings here is bad,
            // but I'm too lazy to define dependancy properties
            var currentTheme = Properties.Settings.Default.Theme.Split('.');
            ThemeType.SelectedIndex = themeTypes.IndexOf(currentTheme[0]);
            ThemeColor.SelectedIndex = themeColors.IndexOf(currentTheme[1]);
        }

        private void Theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ThemeManager.Current.ChangeTheme(Application.Current, ThemeName);
        }

        public string ThemeName
        {
            get
            {
                var type = (string)ThemeType.SelectedItem;
                var color = (string)ThemeColor.SelectedItem;
                return $"{type}.{color}";
            }
        }
    }
}
