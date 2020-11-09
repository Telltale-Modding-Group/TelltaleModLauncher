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
using System.IO;

namespace TelltaleModLauncher
{
    /// <summary>
    /// Interaction logic for ModManager_ViewMod.xaml
    /// </summary>
    public partial class ModManager_ViewMod
    {
        //mod object, this will need to be set with SetMod() to display the mod values.
        private Mod mod;

        private AppSettings appSettings;
        private ModManager_ViewMod_ViewText modManager_ViewMod_ViewText;

        /// <summary>
        /// Opens a 'ViewMod' window to view a mod.
        /// </summary>
        public ModManager_ViewMod(ModManager_ViewMod_ViewText modManager_ViewMod_ViewText, AppSettings appSettings)
        {
            InitializeComponent();

            this.modManager_ViewMod_ViewText = modManager_ViewMod_ViewText;
            this.appSettings = appSettings;

            //create a temporary mod object with some default values
            mod = new Mod("Mod Name", "0", "Mod Author", new List<string>(), "None");
        }

        /// <summary>
        /// Sets the mod object so the values can be obtained and shown to the user.
        /// </summary>
        /// <param name="mod"></param>
        public void SetMod(Mod mod)
        {
            this.mod = mod;
            UpdateUI();
        }

        /// <summary>
        /// Updates the UI Elements of the XAML window to reflect the mod values
        /// </summary>
        private void UpdateUI()
        {
            ui_displayauthor_label.Content = string.Format("Author: {0}", mod.ModAuthor);
            ui_displaycompatibility_label.Content = string.Format("Compatibility: {0}", mod.ModCompatibility.Replace("_", " "));
            ui_displayname_label.Content = string.Format("Name: {0}", mod.ModDisplayName);
            ui_displayversion_label.Content = string.Format("Version: {0}", mod.ModVersion);

            ui_displayfiles_listview.ItemsSource = mod.ModFiles;
        }

        private void ViewMod_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //hide the window on closing instead of creating a new window object every time
            e.Cancel = true;
            this.Hide();
        }

        /// <summary>
        /// Changes the theme of the window.
        /// </summary>
        /// <param name="theme"></param>
        public void UI_ChangeTheme(string theme)
        {
            ThemeManager.Current.ChangeTheme(this, theme);
        }

        private void ui_displayfiles_listview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ui_displayfiles_listview.SelectedItem == null)
                return;

            string modDirectory = appSettings.Get_Current_GameVersionSettings_ModsLocation();
            string selectedModFilePath = (string)ui_displayfiles_listview.SelectedItem;
            string finalPath = System.IO.Path.Combine(modDirectory, selectedModFilePath);

            if (modManager_ViewMod_ViewText.CheckPreviewValidity(finalPath))
                modManager_ViewMod_ViewText.OpenWindow(finalPath, mod.ModDisplayName);
        }
    }
}
