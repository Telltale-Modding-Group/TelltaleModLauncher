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
using System.Drawing;
using ControlzEx.Theming;
using TelltaleModLauncher.Utillities;
using TelltaleModLauncher.Files;
using TsudaKageyu;

namespace TelltaleModLauncher
{
    /// <summary>
    /// Main UI Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        //main script objects
        private AppSettings appSettings;
        private ModManager modManager;

        //xaml windows
        private ModManager_ViewMod modManager_ViewMod;
        private SetupWizard_Window setupWizard_Window;
        private ModManager_ViewMod_ViewText modManager_ViewMod_ViewText;
        //recomendation: singleton pattern for managing window instances

        //IMPORTANT: xaml calls UpdateUI() too soon causing errors due to incomplete initalization.
        //This 'nullifies' UpdateUI() until InitalizeApplication() is done.
        private bool startingUp = true;

        //the bitmap image object for the big launch button
        private BitmapImage bitmapimage;

        /// <summary>
        /// XAML Main Window Initalization
        /// </summary>
        public MainWindow()
        {
            //xaml initalization
            InitializeComponent();

            //application initalization
            InitalizeApplication();

            //update the ui
            UpdateUI();

            //update our app settings UI with the values from the config file
            ui_settings_darkmode_toggle.IsOn = appSettings.appSettingsFile.UI_LightMode;
            ui_settings_defocus_toggle.IsOn = appSettings.appSettingsFile.UI_WindowDefocusing;
            ui_launcher_gameversion_combobox.SelectedIndex = (int)appSettings.appSettingsFile.Default_Game_Version;
        }

        /// <summary>
        /// Initalizes the main script objects and windows
        /// </summary>
        private void InitalizeApplication()
        {
            //initalize our objects
            appSettings = new AppSettings();
            modManager = new ModManager(appSettings, this);

            //get the mods from the current game version folder
            modManager.GetModsFromFolder();

            //create our window objects
            modManager_ViewMod_ViewText = new ModManager_ViewMod_ViewText();
            modManager_ViewMod = new ModManager_ViewMod(modManager_ViewMod_ViewText, appSettings);
            setupWizard_Window = new SetupWizard_Window(appSettings, this, modManager);

            //we finished starting up now, so lets set this to false
            startingUp = false;

            //check if the currently selected game that was selected is valid, if it isn't then pop open the setup wizzard
            if (appSettings.IsGameSetupAndValid() == false)
                InitalizeSetupWizard();
        }

        /// <summary>
        /// Initalizes the setup wizard and hides the main window
        /// </summary>
        private void InitalizeSetupWizard()
        {
            //hide our main launcher window
            this.Hide();

            //show our setup wizard menu
            setupWizard_Window.ShowDialog();
        }

        /// <summary>
        /// Gets the file icon of an .exe
        /// </summary>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        private ImageSource GetFileIcon(string fileLocation)
        {
            //extract icon
            IconExtractor iconExtractor = new IconExtractor(fileLocation);

            //get main icon
            Icon icon = iconExtractor.GetIcon(0);

            //split the icon (as there are multiple sizes embeded)
            Icon[] splitIcons = IconUtil.Split(icon);

            //dispose of the original icon object since we no longer need it
            icon.Dispose();

            //the highest quality icon, will be assigned later
            Icon iconHQ;

            //pixel width, will be changed through iterations and compared
            int width = 0;

            //the index of the highest quality one we find in the list
            int hqIndex = 0;

            //run a loop through the icon list to get highest quality icon
            for(int i = 0; i < splitIcons.Length; i++)
            {
                //if the pixel width is bigger than our width variable then assign it
                if (splitIcons[i].Width > width)
                {
                    //assign the pixel width (used for comparison later)
                    width = splitIcons[i].Width;

                    //get the index of the element
                    hqIndex = i;
                }
            }

            //if the loop finds the one with the highest quality, then use it using the hq index
            iconHQ = splitIcons[hqIndex];

            //convert the icon to a bitmap
            Bitmap bitmap = IconUtil.ToBitmap(iconHQ);

            //create a temporary path for it
            string tempPath = appSettings.Get_App_ConfigDirectory() + "tempIconPreview.bmp";

            //if there is already a file of the same name, get rid of it
            if (System.IO.File.Exists(tempPath))
                IOManagement.DeleteFile(tempPath);

            //save the bitmap to the temp path we defined
            bitmap.Save(tempPath);

            //clear the original object if it isn't already
            if (bitmapimage != null)
                bitmapimage = null;

            //initallize our bitmap image object with a new one (not the same as the bitmap we defined prior)
            bitmapimage = new BitmapImage();
            bitmapimage.BeginInit(); //initalize the image
            bitmapimage.UriSource = new Uri(tempPath, UriKind.Absolute); //set the path
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad; //cache the image since we only need it once
            bitmapimage.EndInit(); //end the initalization

            //return the final image
            return bitmapimage;
        }

        /// <summary>
        /// Updates the UI to reflect the new values from the scripts.
        /// </summary>
        public void UpdateUI()
        {
            //if the app is starting up, don't continue (otherwise we get issues that you don't want)
            if (startingUp)
                return;

            bool ifModSelected = ui_modmanager_modlist_listview.SelectedIndex != -1 && ui_modmanager_modlist_listview.SelectedItem != null;
            bool ifModsExist = ui_modmanager_modlist_listview.Items.Count != 0;
            bool ifCanLaunchGame = ui_launcher_gameversion_combobox.SelectedIndex != -1 && appSettings.IsGameSetupAndValid();

            //launcher stuff
            string gameExePath = appSettings.Get_Current_GameVersionSettings_GameExeLocation();
            ui_launcher_gameversion_combobox.ItemsSource = GameVersion_Functions.Get_Versions_StringList(true);
            ui_launcher_launchgame_tile_gameModsAmount_textBlock.Text = string.Format("Mods: {0}", modManager.mods.Count);
            ui_launcher_launchgame_tile.IsEnabled = ifCanLaunchGame;
            ui_window_appversion_label.Content = appSettings.appVersionString;

            if(ui_launcher_gameversion_combobox.SelectedItem != null)
                ui_launcher_launchgame_tile_gameTitle_textBlock.Text = ui_launcher_gameversion_combobox.SelectedItem.ToString();

            ui_launcher_launchgame_tile_exeIcon_image.Source = null;

            if(System.IO.File.Exists(gameExePath))
                ui_launcher_launchgame_tile_exeIcon_image.Source = GetFileIcon(gameExePath);

            //mod manager stuff
            ui_modmanager_addmod_button.IsEnabled = ifCanLaunchGame;
            ui_modmanager_removemod_button.IsEnabled = ifModSelected;
            ui_modmanager_viewmod_button.IsEnabled = ifModSelected;
            ui_modmanager_openmodfolder_button.IsEnabled = ifCanLaunchGame;
            ui_modmanager_purgemod_button.IsEnabled = ifModsExist;
            ui_modmanager_refreshmodfolder_button.IsEnabled = ifCanLaunchGame;
            ui_modmanager_modlist_listview.IsEnabled = ifCanLaunchGame;
            ui_modmanager_modlist_listview.ItemsSource = modManager.mods;
            ui_modmanager_modlist_listview.Items.Refresh();
            ui_modmanager_modlist_listview_contextmenu_add.IsEnabled = ui_modmanager_addmod_button.IsEnabled;
            ui_modmanager_modlist_listview_contextmenu_remove.IsEnabled = ui_modmanager_removemod_button.IsEnabled;
            ui_modmanager_modlist_listview_contextmenu_openmodfolder.IsEnabled = ui_modmanager_openmodfolder_button.IsEnabled;
            ui_modmanager_modlist_listview_contextmenu_refreshmods.IsEnabled = ui_modmanager_refreshmodfolder_button.IsEnabled;
            ui_modmanager_modlist_listview_contextmenu_view.IsEnabled = ui_modmanager_viewmod_button.IsEnabled;
            ui_modmanager_gameversion_label.Content = appSettings.Get_Current_GameVersionName();
            ui_modmanager_gameversion_label.Content = string.Format("Game: {0}", appSettings.Get_Current_GameVersionName().ToString().Replace("_", " "));

            //settings stuff
            ui_settings_gameversion_label.Content = string.Format("Current Game: {0}", appSettings.Get_Current_GameVersionName().ToString().Replace("_", " "));
            ui_settings_gameversion_combobox.ItemsSource = GameVersion_Functions.Get_Versions_StringList(true);
            ui_settings_gameversion_combobox.SelectedIndex = (int)appSettings.appSettingsFile.Default_Game_Version;
            ui_settings_gamedirectoryexe_textbox.Text = appSettings.Get_Current_GameVersionSettings_GameExeLocation();
        }

        /// <summary>
        /// Blurs and darkens the current window (if defocus is enabled)
        /// </summary>
        private void UI_Effect_DeFocusWindow()
        {
            //if the effect isn't on, don't continue
            if (appSettings.Get_AppSettings_DefocusEffect() == false)
                return;

            //create our blur effect
            System.Windows.Media.Effects.BlurEffect blurEffect = new System.Windows.Media.Effects.BlurEffect();
            blurEffect.Radius = 10;

            //set the blur effect and our opacity to darken the window
            this.Effect = blurEffect;
            this.Opacity = 0.75;
        }

        /// <summary>
        /// Clears the effects applied to the window (if defocus is enabled)
        /// </summary>
        private void UI_Effect_ClearEffects()
        {
            //if the effect isn't on, don't continue
            if (appSettings.Get_AppSettings_DefocusEffect() == false)
                return;

            //remove the effect and set the opacity back
            this.Effect = null;
            this.Opacity = 1;
        }

        //---------------- MAIN APPLICATION ACTIONS ----------------
        /// <summary>
        /// Adds a mod to the game
        /// </summary>
        private void Action_AddMod()
        {
            //(if enabled) defocus the main window
            UI_Effect_DeFocusWindow();

            //call the main function in the mod manager for adding a mod
            modManager.AddMod();

            //(if enabled) clear focus effects when we finish/cancled adding a mod
            UI_Effect_ClearEffects();

            //update the UI to reflect any new changes
            UpdateUI();
        }

        private void Action_RemoveMod()
        {
            //call the main removal function in the mod manager for removing a mod
            modManager.RemoveMod(ui_modmanager_modlist_listview.SelectedIndex);

            //update the UI to reflect any new changes
            UpdateUI();
        }

        private void Action_PurgeMod()
        {
            //(if enabled) defocus the main window
            UI_Effect_DeFocusWindow();

            //call the main function to remove all mods in a mod folder
            modManager.PurgeMods();

            //(if enabled) clear focus effects when we finish/canceled purging mods
            UI_Effect_ClearEffects();

            //update the UI to reflect any new changes
            UpdateUI();
        }

        private void Action_ViewMod()
        {
            //(if enabled) defocus the main window
            UI_Effect_DeFocusWindow();

            //get the currently selected mod in the list view
            Mod selectedMod = modManager.mods[ui_modmanager_modlist_listview.SelectedIndex];

            //set the mod in our viewmod window to what we selected and open it
            modManager_ViewMod.SetMod(selectedMod);
            modManager_ViewMod.ShowDialog();

            //(if enabled) clear focus effects when we finish/canceled viewing a mod
            UI_Effect_ClearEffects();

            //update the UI to reflect any new changes
            UpdateUI();
        }

        private void Action_SetCurrentGameExePath()
        {
            //(if enabled) defocus the main window
            UI_Effect_DeFocusWindow();

            //open a window and tell the user to set the game exe location, then update those changes to the file
            appSettings.Set_Current_GameVersionSettings_GameExeLocation();
            appSettings.UpdateChangesToFile();

            //(if enabled) clear focus effects when we finish/canceled setting a path for the game
            UI_Effect_ClearEffects();

            //update the UI to reflect any new changes
            UpdateUI();
        }

        private void Action_OpenCurrentGameModsFolder()
        {
            //call the main function in the mod manager for opening the mod folder in explorer
            modManager.OpenModFolder();

            //update the UI to reflect any new changes
            UpdateUI();
        }

        private void Action_GetModsFromCurrentGameModsFolder()
        {
            //call the main function in the mod manager for getting the mods in the folder of the currently selected game
            modManager.GetModsFromFolder();

            //update the UI to reflect any new changes
            UpdateUI();
        }

        //---------------- XAML FUNCTIONS ----------------
        private void ui_modmanager_addmod_button_Click(object sender, RoutedEventArgs e)
        {
            Action_AddMod();
        }

        private void ui_modmanager_removemod_button_Click(object sender, RoutedEventArgs e)
        {
            Action_RemoveMod();
        }

        private void ui_modmanager_purgemod_button_Click(object sender, RoutedEventArgs e)
        {
            Action_PurgeMod();
        }

        private void ui_modmanager_modlist_listview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //update the UI to reflect any new changes
            UpdateUI();
        }

        private void ui_modmanager_viewmod_button_Click(object sender, RoutedEventArgs e)
        {
            Action_ViewMod();
        }

        private void ui_modmanager_modlist_listview_contextmenu_view_click(object sender, RoutedEventArgs e)
        {
            Action_ViewMod();
        }

        private void ui_modmanager_modlist_listview_contextmenu_remove_click(object sender, RoutedEventArgs e)
        {
            Action_RemoveMod();
        }

        private void ui_modmanager_modlist_listview_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Action_ViewMod();
        }

        private void ui_settings_gamedirectoryexeBrowse_button_Click(object sender, RoutedEventArgs e)
        {
            Action_SetCurrentGameExePath();
        }

        private void ui_launcher_launchgame_tile_Click(object sender, RoutedEventArgs e)
        {
            appSettings.LaunchGame();
        }

        private void ui_modmanager_openmodfolder_button_Click(object sender, RoutedEventArgs e)
        {
            Action_OpenCurrentGameModsFolder();
        }

        private void ui_window_help_button_Click(object sender, RoutedEventArgs e)
        {
            appSettings.Open_LauncherHelp();
        }

        private void ui_modmanager_modlist_listview_contextmenu_add_click(object sender, RoutedEventArgs e)
        {
            Action_AddMod();
        }
        private void ui_modmanager_modlist_listview_contextmenu_openmodfolder_click(object sender, RoutedEventArgs e)
        {
            Action_OpenCurrentGameModsFolder();
        }

        private void ui_modmanager_refreshmodfolder_button_Click(object sender, RoutedEventArgs e)
        {
            Action_GetModsFromCurrentGameModsFolder();
        }

        private void ui_modmanager_modlist_listview_contextmenu_refreshmods_click(object sender, RoutedEventArgs e)
        {
            Action_GetModsFromCurrentGameModsFolder();
        }

        private void ui_settings_gamedirectoryexe_textbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Action_SetCurrentGameExePath();
        }

        private void ui_launcher_gameversion_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //get the currently selected game version enum value from the launcher tab
            GameVersion selectedGameVersion = (GameVersion)ui_launcher_gameversion_combobox.SelectedIndex;

            //tell the app to switch to this version
            appSettings.ChangeGameVersion(selectedGameVersion);

            //tell the manager we changed game versions
            modManager.ChangedGameVersion();

            //update the UI to reflect any new changes
            UpdateUI();
        }

        private void ui_settings_defocus_toggle_Toggled(object sender, RoutedEventArgs e)
        {
            //set the value of the defocus toggle to that of the UI element in the settings tab
            appSettings.Set_Current_AppSettings_UI_DefocusEffect(ui_settings_defocus_toggle.IsOn);

            //update changes to file automatically
            appSettings.UpdateChangesToFile();

            //update the UI to reflect any new changes
            UpdateUI();
        }

        private void ui_settings_gameversion_combobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //get the currently selected game version from the settings tab, to set our DEFAULT game version for when the launcher starts up again
            GameVersion selectedGameVersion = (GameVersion)ui_settings_gameversion_combobox.SelectedIndex;

            //tell the app we switched the default game version
            appSettings.Set_Current_AppSettings_DefaultGameVersion(selectedGameVersion);

            //update those changes to the file
            appSettings.UpdateChangesToFile();

            //update the UI to reflect any new changes
            UpdateUI();
        }

        private void ui_settings_darkmode_toggle_Toggled(object sender, RoutedEventArgs e)
        {
            //get the UI theme based on what is toggled
            string darkmodeTheme = ui_settings_darkmode_toggle.IsOn ? "Light.Blue" : "Dark.Blue";

            //set the theme of all of our windows to what we toggled
            ThemeManager.Current.ChangeTheme(this, darkmodeTheme);
            modManager_ViewMod.UI_ChangeTheme(darkmodeTheme);
            setupWizard_Window.UI_ChangeTheme(darkmodeTheme);

            //update the value in the app config file
            appSettings.Set_Current_AppSettings_UI_LightMode(ui_settings_darkmode_toggle.IsOn);

            //update the changes to file
            appSettings.UpdateChangesToFile();

            //update the UI to reflect any new changes
            UpdateUI();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //shut down the app (cause for whatever reason it still runs in the background)
            Application.Current.Shutdown();
        }
        //---------------- XAML FUNCTIONS END ----------------
    }
}
