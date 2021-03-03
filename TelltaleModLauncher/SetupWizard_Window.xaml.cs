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
using TelltaleModLauncher.Utillities;

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

            var GameVersions_ToStringList = Enum.GetNames(typeof(GameVersion));

            ui_window_appversion_label.Content = appSettings.appVersionString;
            ui_gamesetup_gameversion_combobox.ItemsSource = GameVersions_ToStringList;
            ui_gamesetup_gamedirectoryexe_textbox.Text = appSettings.Get_Current_GameVersionSettings_GameExeLocation();
        }

        //---------------- XAML FUNCTIONS ----------------

        private void ui_gamesetup_gameversion_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GameVersion selectedGameVersion = (GameVersion)ui_gamesetup_gameversion_combobox.SelectedIndex;
            
            appSettings.ChangeGameVersion(selectedGameVersion);

            UpdateUI();
        }

        private void ui_gamesetup_ttarchexePathNumber_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUI();
        }

        private void ui_gamesetup_gamedirectoryexeBrowse_button_Click(object sender, RoutedEventArgs e)
        {
            appSettings.Set_Current_GameVersionSettings_GameExeLocation();

            UpdateUI();
        }

        private void ui_window_help_button_Click(object sender, RoutedEventArgs e)
        {
            appSettings.Open_LauncherHelpSetup();

            UpdateUI();
        }

        private void ui_gamesetup_done_Click(object sender, RoutedEventArgs e)
        {
            GameVersion selectedGameVersion = (GameVersion)ui_gamesetup_gameversion_combobox.SelectedItem;
            appSettings.ChangeGameVersion(selectedGameVersion);

            modManager.ChangedGameVersion();
            appSettings.Set_Current_GameVersionSettings_GameExeLocation();

            appSettings.UpdateChangesToFile();

            this.Hide();
            mainWindow.Show();
            mainWindow.Activate();
        }

        //---------------- XAML FUNCTIONS END ----------------
    }
}
