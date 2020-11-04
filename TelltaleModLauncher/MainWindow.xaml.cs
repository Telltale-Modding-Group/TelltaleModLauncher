﻿using System;
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
        //main mod manager class
        private ModManager modManager = new ModManager();
        private ModManager_ViewMod modManager_ViewMod = new ModManager_ViewMod();

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
            bool ifModSelected = ui_modmanager_modlist_listview.SelectedIndex != -1 && ui_modmanager_modlist_listview.SelectedItem != null;
            bool ifModsExist = ui_modmanager_modlist_listview.Items.Count != 0;
            bool ifCanLaunchGame = ui_launcher_gameversion_combobox.SelectedIndex != -1;

            ui_launcher_gameversion_combobox.ItemsSource = Enum.GetValues(typeof(GameVersion)).Cast<GameVersion>().ToList();
            ui_launcher_launchgame_tile.IsEnabled = ifCanLaunchGame;

            //ui_label.Content = app_Settings.Default_Game_Version.Replace("_", " ");

            ui_modmanager_removemod_button.IsEnabled = ifModSelected;
            ui_modmanager_viewmod_button.IsEnabled = ifModSelected;
            ui_modmanager_purgemod_button.IsEnabled = ifModsExist;
            ui_modmanager_modlist_listview.ItemsSource = modManager.mods;
            ui_modmanager_modlist_listview.Items.Refresh();
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
            modManager_ViewMod.UI_ChangeTheme(darkmodeTheme);
        }

        private void ui_modmanager_viewmod_button_Click(object sender, RoutedEventArgs e)
        {
            Mod selectedMod = modManager.mods[ui_modmanager_modlist_listview.SelectedIndex];

            modManager_ViewMod.Show();
            modManager_ViewMod.SetMod(selectedMod);
        }

        private void ui_modmanager_modlist_listview_contextmenu_view_click(object sender, RoutedEventArgs e)
        {
            Mod selectedMod = modManager.mods[ui_modmanager_modlist_listview.SelectedIndex];

            modManager_ViewMod.Show();
            modManager_ViewMod.SetMod(selectedMod);
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
            UpdateUI();
        }
        //---------------- XAML FUNCTIONS END ----------------
    }
}
