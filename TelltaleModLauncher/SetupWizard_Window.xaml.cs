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
    /// Interaction logic for SetupWizard.xaml
    /// </summary>
    public partial class SetupWizard_Window
    {
        //main objects passed down from MainWindow
        private AppSettings appSettings;
        private MainWindow mainWindow;
        private IOManagement ioManagement;
        private ModManager modManager;

        /// <summary>
        /// Initalizes the setup wizzard window.
        /// </summary>
        /// <param name="appSettings"></param>
        /// <param name="mainWindow"></param>
        /// <param name="ioManagement"></param>
        /// <param name="modManager"></param>
        public SetupWizard_Window(AppSettings appSettings, MainWindow mainWindow, IOManagement ioManagement, ModManager modManager)
        {
            InitializeComponent();

            this.appSettings = appSettings;
            this.mainWindow = mainWindow;
            this.ioManagement = ioManagement;
            this.modManager = modManager;

            UpdateUI();
        }

        /// <summary>
        /// Updates the UI elements to reflect the new data from the objects.
        /// </summary>
        public void UpdateUI()
        {
            string darkmodeTheme = appSettings.Get_AppSettings_LightMode() ? "Light.Blue" : "Dark.Blue";
            ThemeManager.Current.ChangeTheme(this, darkmodeTheme);

            bool ifOtherGame = appSettings.Get_Current_GameVersionName() == GameVersion.Other;

            var GameVersions_ToStringList = Enum.GetValues(typeof(GameVersion)).Cast<GameVersion>();

            ui_gamesetup_gameversion_combobox.ItemsSource = GameVersions_ToStringList;
            ui_gamesetup_gamedirectoryexe_textbox.Text = appSettings.Get_Current_GameVersionSettings_GameExeLocation();
            ui_gamesetup_gamemodsdirectory_textbox.Text = appSettings.Get_Current_GameVersionSettings_ModsLocation();

            ui_dependencies_luacompilerPath_textbox.Text = appSettings.appSettingsFile.Location_LuaCompiler;
            ui_dependencies_luadecompilerPath_textbox.Text = appSettings.appSettingsFile.Location_LuaDecompiler;
            ui_dependencies_ttarchexePath_textbox.Text = appSettings.appSettingsFile.Location_Ttarchext;

            ui_gamesetup_ttarchexePathNumber_combobox.SelectedIndex = appSettings.Get_Current_GameVersionSettings_ttarchNumber();
            ui_gamesetup_ttarchexePathNumber_combobox.IsEnabled = ifOtherGame;
            List<string> ttarchext_GameEnumNames = Enum.GetNames(typeof(ttarchext_GameEnums)).ToList();
            List<string> ttarchext_GameEnumDisplayNames = new List<string>();

            foreach (string ttarchextName in ttarchext_GameEnumNames)
            {
                int gameEnumValue = (int)Enum.Parse(typeof(ttarchext_GameEnums), ttarchextName);
                string displayName = string.Format("{0} | {1}", gameEnumValue, ttarchextName);

                ttarchext_GameEnumDisplayNames.Add(displayName);
            }

            ui_gamesetup_ttarchexePathNumber_combobox.ItemsSource = ttarchext_GameEnumDisplayNames;
        }

        //---------------- XAML FUNCTIONS ----------------

        private void ui_gamesetup_gameversion_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GameVersion selectedGameVersion = (GameVersion)ui_gamesetup_gameversion_combobox.SelectedItem;

            appSettings.ChangeGameVersion(selectedGameVersion);
            modManager.ChangedGameVersion();

            UpdateUI();
        }

        private void ui_gamesetup_ttarchexePathNumber_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            appSettings.Set_Current_GameVersionSettings_EnumNumber(ui_gamesetup_ttarchexePathNumber_combobox.SelectedIndex);
            appSettings.UpdateChangesToFile();

            UpdateUI();
        }

        private void ui_gamesetup_gamedirectoryexeBrowse_button_Click(object sender, RoutedEventArgs e)
        {
            appSettings.Set_Current_GameVersionSettings_GameExeLocation();
            appSettings.UpdateChangesToFile();

            UpdateUI();
        }

        private void ui_gamesetup_gamemodsdirectoryBrowse_button_Click(object sender, RoutedEventArgs e)
        {
            appSettings.Set_Current_GameVersionSettings_GameModsDirectory();
            appSettings.UpdateChangesToFile();

            UpdateUI();
        }

        private void ui_dependencies_ttarchexePathBrowse_button_Click(object sender, RoutedEventArgs e)
        {
            string path = "";

            ioManagement.GetFilePath(ref path, "Locate the ttarchexe Executable");

            if (string.IsNullOrEmpty(path))
                return;

            appSettings.Set_Current_AppSettings_ttarchextLocation(path);
            appSettings.UpdateChangesToFile();

            UpdateUI();
        }

        private void ui_dependencies_luacompilerPathBrowse_button_Click(object sender, RoutedEventArgs e)
        {
            string path = "";

            ioManagement.GetFilePath(ref path, "Locate the Lua Compiler");

            if (string.IsNullOrEmpty(path))
                return;

            appSettings.Set_Current_AppSettings_LuaCompilerLocation(path);
            appSettings.UpdateChangesToFile();

            UpdateUI();
        }

        private void ui_dependencies_luadecompilerPathBrowse_button_Click(object sender, RoutedEventArgs e)
        {
            string path = "";

            ioManagement.GetFilePath(ref path, "Locate the Lua Decompiler");

            if (string.IsNullOrEmpty(path))
                return;

            appSettings.Set_Current_AppSettings_LuaDecompilerLocation(path);
            appSettings.UpdateChangesToFile();

            UpdateUI();
        }

        private void ui_gamesetup_done_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            mainWindow.Show();
            mainWindow.Activate();
        }

        //---------------- XAML FUNCTIONS END ----------------
    }
}
