using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ControlzEx.Theming;

namespace TelltaleModLauncher
{
    /// <summary>
    /// Interaction logic for ModManager_ViewMod.xaml
    /// </summary>
    public partial class ModManager_ViewMod
    {
        private Mod mod;

        public ModManager_ViewMod()
        {
            InitializeComponent();

            mod = new Mod("Mod Name", "0", "Mod Author", new List<string>(), "None");
        }

        public void SetMod(Mod mod)
        {
            this.mod = mod;

            UpdateUI();
        }

        private void UpdateUI()
        {
            ui_displayauthor_label.Content = string.Format("Author: {0}", mod.ModAuthor);
            ui_displaycompatibility_label.Content = string.Format("Compatibility: {0}", mod.ModCompatibility);
            ui_displayname_label.Content = string.Format("Name: {0}", mod.ModDisplayName);
            ui_displayversion_label.Content = string.Format("Version: {0}", mod.ModVersion);

            ui_displayfiles_listview.ItemsSource = mod.ModFiles;
        }

        private void ViewMod_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        public void UI_ChangeTheme(string theme)
        {
            ThemeManager.Current.ChangeTheme(this, theme);
        }
    }
}
