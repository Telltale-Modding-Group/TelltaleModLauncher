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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlzEx.Theming;

namespace TelltaleModLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ModManager modManager = new ModManager();

        public MainWindow()
        {
            InitializeComponent();
            InitalizeApplication();
            UpdateUI();
        }

        public void InitalizeApplication()
        {
            ui_modmanager_modlist_listview.ItemsSource = modManager.mods;
        }

        public void UpdateUI()
        {
            ui_modmanager_modlist_listview.ItemsSource = modManager.mods;
            ui_modmanager_modlist_listview.Items.Refresh();

            //label_gameName.Content = app_Settings.Default_Game_Version.Replace("_", " ");

            bool ifModSelected = ui_modmanager_modlist_listview.SelectedIndex != -1 && ui_modmanager_modlist_listview.SelectedItem != null;
            bool ifModsExist = ui_modmanager_modlist_listview.Items.Count != 0;
            ui_modmanager_removemod_button.IsEnabled = ifModSelected;
            ui_modmanager_editmod_button.IsEnabled = ifModSelected;
            //button_addMod.IsEnabled = ifModsLocationExist;
        }

        //---------------- XAML FUNCTIONS ----------------
        private void ui_modmanager_addmod_button_Click(object sender, RoutedEventArgs e)
        {
            modManager.AddMod();

            UpdateUI();
        }

        private void ui_modmanager_removemod_button_Click(object sender, RoutedEventArgs e)
        {
            modManager.RemoveMod(ui_modmanager_modlist_listview.SelectedIndex);

            UpdateUI();
        }

        private void ui_modmanager_editmod_button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ui_modmanager_purgemod_button_Click(object sender, RoutedEventArgs e)
        {
            modManager.PurgeMods();

            UpdateUI();
        }

        private void ui_modmanager_modlist_listview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUI();
        }

        private void ui_settings_gamedirectorybrowse_button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ui_settings_ttarchexePathBrowse_button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ui_settings_ttarchexePath_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ui_settings_luacompilerPath_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ui_settings_luacompilerPathBrowse_button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ui_settings_luadecompilerPath_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ui_settings_luadecompilerPathBrowse_button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ui_settings_darkmode_toggle_Toggled(object sender, RoutedEventArgs e)
        {
            string darkmodeTheme = ui_settings_darkmode_toggle.IsOn ? "Light.Blue" : "Dark.Blue";

            ThemeManager.Current.ChangeTheme(this, darkmodeTheme);
        }
        //---------------- XAML FUNCTIONS END ----------------
    }
}
