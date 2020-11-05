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
    /// Main UI Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        //main script objects
        private IOManagement ioManagement;
        private AppSettings appSettings;
        private ModManager modManager;

        //xaml windows
        private ModManager_ViewMod modManager_ViewMod = new ModManager_ViewMod();

        //IMPORTANT: xaml calls UpdateUI() too soon causing errors due to incomplete initalization.
        //This 'nullifies' UpdateUI() until InitalizeApplication() is done.
        private bool startingUp = true;

        /// <summary>
        /// XAML Main Window Initalization
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            InitalizeApplication();
            UpdateUI();

            ui_settings_darkmode_toggle.IsOn = appSettings.appSettingsFile.UI_LightMode;
            ui_launcher_gameversion_combobox.SelectedIndex = (int)appSettings.appSettingsFile.Default_Game_Version;
        }

        /// <summary>
        /// Initalizes the main script objects
        /// </summary>
        public void InitalizeApplication()
        {
            ioManagement = new IOManagement();
            appSettings = new AppSettings(ioManagement);
            modManager = new ModManager(appSettings, ioManagement);
            modManager.GetModsFromFolder();

            startingUp = false;
        }

        /// <summary>
        /// Updates the UI to reflect the new values from the scripts.
        /// </summary>
        public void UpdateUI()
        {
            if (startingUp)
                return;

            bool ifModSelected = ui_modmanager_modlist_listview.SelectedIndex != -1 && ui_modmanager_modlist_listview.SelectedItem != null;
            bool ifModsExist = ui_modmanager_modlist_listview.Items.Count != 0;
            bool ifCanLaunchGame = ui_launcher_gameversion_combobox.SelectedIndex != -1 && appSettings.IsGameSetupAndValid();
            bool ifOtherGame = appSettings.Get_Current_GameVersionName() == GameVersion.Other;

            var GameVersions_ToStringList = Enum.GetValues(typeof(GameVersion)).Cast<GameVersion>();

            //launcher stuff
            ui_launcher_gameversion_combobox.ItemsSource = GameVersions_ToStringList;
            ui_launcher_gamedirectory_label.Content = appSettings.Get_Current_GameVersionSettings_GameExeLocation();
            ui_launcher_gamemodsamount_label.Content = string.Format("Mods Installed: {0}", modManager.mods.Count);
            ui_launcher_launchgame_tile.IsEnabled = ifCanLaunchGame;

            //mod manager stuff
            ui_modmanager_addmod_button.IsEnabled = ifCanLaunchGame;
            ui_modmanager_removemod_button.IsEnabled = ifModSelected;
            ui_modmanager_viewmod_button.IsEnabled = ifModSelected;
            ui_modmanager_openmodfolder_button.IsEnabled = ifCanLaunchGame;
            ui_modmanager_purgemod_button.IsEnabled = ifModsExist;
            ui_modmanager_modlist_listview.IsEnabled = ifCanLaunchGame;
            ui_modmanager_modlist_listview.ItemsSource = modManager.mods;
            ui_modmanager_modlist_listview.Items.Refresh();
            ui_modmanager_gameversion_label.Content = appSettings.Get_Current_GameVersionName();
            ui_modmanager_gameversion_label.Content = string.Format("Game: {0}", appSettings.Get_Current_GameVersionName().ToString().Replace("_", " "));

            //settings stuff
            ui_settings_gameversion_label.Content = string.Format("Current Game: {0}", appSettings.Get_Current_GameVersionName().ToString().Replace("_", " "));
            ui_settings_gameversion_combobox.ItemsSource = GameVersions_ToStringList;
            ui_settings_gameversion_combobox.SelectedIndex = (int)appSettings.appSettingsFile.Default_Game_Version;
            ui_settings_gamedirectoryexe_textbox.Text = appSettings.Get_Current_GameVersionSettings_GameExeLocation();
            ui_settings_gamemodsdirectory_textbox.Text = appSettings.Get_Current_GameVersionSettings_ModsLocation();
            ui_settings_luacompilerPath_textbox.Text = appSettings.appSettingsFile.Location_LuaCompiler;
            ui_settings_luadecompilerPath_textbox.Text = appSettings.appSettingsFile.Location_LuaDecompiler;
            ui_settings_ttarchexePath_textbox.Text = appSettings.appSettingsFile.Location_Ttarchext;
            ui_settings_ttarchexePathNumber_textbox.IsEnabled = ifOtherGame;
            ui_settings_ttarchexePathNumber_textbox.Text = appSettings.Get_Current_GameVersionSettings_ttarchNumber().ToString();
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

        private void ui_modmanager_purgemod_button_Click(object sender, RoutedEventArgs e)
        {
            modManager.PurgeMods();

            UpdateUI();
        }

        private void ui_modmanager_modlist_listview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUI();
        }

        private void ui_settings_ttarchexePathBrowse_button_Click(object sender, RoutedEventArgs e)
        {
            string path = "";

            ioManagement.GetFilePath(ref path, "Locate the ttarchext executable");

            if (string.IsNullOrEmpty(path))
                return;

            appSettings.Set_Current_AppSettings_ttarchextLocation(path);
            appSettings.UpdateChangesToFile();

            UpdateUI();
        }

        private void ui_settings_luacompilerPathBrowse_button_Click(object sender, RoutedEventArgs e)
        {
            string path = "";

            ioManagement.GetFilePath(ref path, "Locate the Lua Compiler executable");

            if (string.IsNullOrEmpty(path))
                return;

            appSettings.Set_Current_AppSettings_LuaCompilerLocation(path);
            appSettings.UpdateChangesToFile();

            UpdateUI();
        }
        private void ui_settings_luadecompilerPathBrowse_button_Click(object sender, RoutedEventArgs e)
        {
            string path = "";

            ioManagement.GetFilePath(ref path, "Locate the Lua Decompiler executable");

            if (string.IsNullOrEmpty(path))
                return;

            appSettings.Set_Current_AppSettings_LuaDecompilerLocation(path);
            appSettings.UpdateChangesToFile();

            UpdateUI();
        }

        private void ui_settings_darkmode_toggle_Toggled(object sender, RoutedEventArgs e)
        {
            string darkmodeTheme = ui_settings_darkmode_toggle.IsOn ? "Light.Blue" : "Dark.Blue";

            ThemeManager.Current.ChangeTheme(this, darkmodeTheme);
            modManager_ViewMod.UI_ChangeTheme(darkmodeTheme);

            appSettings.Set_Current_AppSettings_UI_LightMode(ui_settings_darkmode_toggle.IsOn);
            appSettings.UpdateChangesToFile();

            UpdateUI();
        }

        private void ui_modmanager_viewmod_button_Click(object sender, RoutedEventArgs e)
        {
            Mod selectedMod = modManager.mods[ui_modmanager_modlist_listview.SelectedIndex];

            modManager_ViewMod.Show();
            modManager_ViewMod.SetMod(selectedMod);

            UpdateUI();
        }

        private void ui_modmanager_modlist_listview_contextmenu_view_click(object sender, RoutedEventArgs e)
        {
            Mod selectedMod = modManager.mods[ui_modmanager_modlist_listview.SelectedIndex];

            modManager_ViewMod.Show();
            modManager_ViewMod.SetMod(selectedMod);

            UpdateUI();
        }

        private void ui_modmanager_modlist_listview_contextmenu_remove_click(object sender, RoutedEventArgs e)
        {
            modManager.RemoveMod(ui_modmanager_modlist_listview.SelectedIndex);

            UpdateUI();
        }

        private void ui_modmanager_modlist_listview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Mod selectedMod = modManager.mods[ui_modmanager_modlist_listview.SelectedIndex];

            modManager_ViewMod.Show();
            modManager_ViewMod.SetMod(selectedMod);
        }

        private void ui_launcher_gameversion_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GameVersion selectedGameVersion = (GameVersion)ui_launcher_gameversion_combobox.SelectedItem;

            appSettings.ChangeGameVersion(selectedGameVersion);
            modManager.ChangedGameVersion();

            UpdateUI();
        }

        private void ui_settings_ttarchexePathNumber_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (startingUp)
                return;

            try
            {
                int userValue = int.Parse(ui_settings_ttarchexePathNumber_textbox.Text);
                appSettings.Set_Current_GameVersionSettings_EnumNumber(userValue);
                appSettings.UpdateChangesToFile();
            }
            catch(Exception exception)
            {
                MessageBox.Show(exception.ToString(), exception.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            UpdateUI();
        }

        private void ui_settings_gamedirectoryexeBrowse_button_Click(object sender, RoutedEventArgs e)
        {
            appSettings.Set_Current_GameVersionSettings_GameExeLocation();
            appSettings.UpdateChangesToFile();

            UpdateUI();
        }

        private void ui_settings_gamemodsdirectoryBrowse_button_Click(object sender, RoutedEventArgs e)
        {
            appSettings.Set_Current_GameVersionSettings_GameModsDirectory();
            appSettings.UpdateChangesToFile();

            UpdateUI();
        }

        private void ui_settings_gameversion_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GameVersion selectedGameVersion = (GameVersion)ui_settings_gameversion_combobox.SelectedItem;

            appSettings.Set_Current_AppSettings_DefaultGameVersion(selectedGameVersion);
            appSettings.UpdateChangesToFile();

            UpdateUI();
        }

        private void ui_launcher_launchgame_tile_Click(object sender, RoutedEventArgs e)
        {
            appSettings.LaunchGame();
        }

        private void ui_modmanager_openmodfolder_button_Click(object sender, RoutedEventArgs e)
        {
            modManager.OpenModFolder();

            UpdateUI();
        }
        //---------------- XAML FUNCTIONS END ----------------
    }
}
