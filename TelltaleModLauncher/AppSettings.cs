using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using TelltaleModLauncher.Utillities;
using TelltaleModLauncher.Files;

namespace TelltaleModLauncher
{
    /// <summary>
    /// Main class pertaining to the application settings.
    /// </summary>
    public class AppSettings
    {
        //public
        public string appVersionString = "v0.7.0";

        public AppSettingsFile appSettingsFile;
        public List<GameVersionSettings> GameVersionSettings { get; set; }
        public GameVersionSettings current_GameVersionSettings;

        //private
        private static string systemDocumentsPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string configFile_filename = "TelltaleModLauncher_Config.json";
        private static string configFile_directory_location = systemDocumentsPath + "/TelltaleModLauncher/";
        private static string configFile_file_location = configFile_directory_location + configFile_filename;
        private static string launcherHelpLink = "https://github.com/Telltale-Modding-Group/TelltaleModLauncher/wiki/%5BHelp%5D";
        private static string launcherHelpSetupLink = "https://github.com/Telltale-Modding-Group/TelltaleModLauncher/wiki/%5BHelp%5D-Application-Setup";

        /// <summary>
        /// Application Settings Class, creates an AppSettings object. This is called on application startup.
        /// <para>If there is an existing config file, it will parse the data from it.</para>
        /// <para>If there is not an existing config file, or a TelltaleModLauncher directory, create a new one.</para>
        /// </summary>
        public AppSettings()
        {
            if (File.Exists(configFile_file_location))
            {
                ReadConfigFile();
            }
            else
            {
                appSettingsFile = new AppSettingsFile();
                New_GameVersionSettingsList();

                if (!Directory.Exists(configFile_directory_location))
                    IOManagement.CreateDirectory(configFile_directory_location);

                WriteToFile();
            }    
        }

        /// <summary>
        /// Launches the exe that is found in the current selected game version exe location.
        /// </summary>
        public void LaunchGame()
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = current_GameVersionSettings.Game_LocationExe;
            processStartInfo.WorkingDirectory = current_GameVersionSettings.Game_Location;

            Process.Start(processStartInfo);
        }
         
        /// <summary>
        /// Changes the current game version to the desired game version.
        /// </summary>
        /// <param name="newVersion"></param>
        public void ChangeGameVersion(GameVersion newVersion)
        {
            foreach(GameVersionSettings gameVersionSetting in GameVersionSettings)
            {
                if(newVersion.Equals(gameVersionSetting.Game_Version))
                {
                    current_GameVersionSettings = gameVersionSetting;
                    return;
                }
            }
        }

        /// <summary>
        /// Creates a new game version settings list.
        /// </summary>
        private void New_GameVersionSettingsList()
        {
            GameVersionSettings = new List<GameVersionSettings>();

            var enumListString = Enum.GetNames(typeof(GameVersion));

            for (int i = 0; i < enumListString.Length; i++)
            {
                GameVersionSettings new_gameVersionSettings = new GameVersionSettings();
                new_gameVersionSettings.Game_Version = (GameVersion)Enum.Parse(typeof(GameVersion), enumListString[i]);

                GameVersionSettings.Add(new_gameVersionSettings);
            }
        }

        /// <summary>
        /// Reads and parses the data from the app config file.
        /// </summary>
        public void ReadConfigFile()
        {
            appSettingsFile = new AppSettingsFile();
            GameVersionSettings = new List<GameVersionSettings>();

            //read the data from the config file
            string jsonText = File.ReadAllText(configFile_file_location);

            //parse the data into a json array
            JArray array;

            try
            {
                array = JArray.Parse(jsonText);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString(), e.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            JObject AppSettingsFile_fromJson = array[0] as JObject;
            JArray GameVersionSettings_fromJson = array[1] as JArray;

            //loop through each property to get the data
            foreach (JProperty property in AppSettingsFile_fromJson.Properties())
            {
                string name = property.Name;

                if (name.Equals(nameof(appSettingsFile.Default_Game_Version)))
                    appSettingsFile.Default_Game_Version = (GameVersion)(int)property.Value;

                if (name.Equals(nameof(appSettingsFile.UI_LightMode)))
                    appSettingsFile.UI_LightMode = (bool)property.Value;

                if (name.Equals(nameof(appSettingsFile.UI_WindowDefocusing)))
                    appSettingsFile.UI_WindowDefocusing = (bool)property.Value;
            }

            //loop through each property to get the data
            foreach (JObject obj in GameVersionSettings_fromJson.Children<JObject>())
            {
                GameVersionSettings new_gameVersionSettings = new GameVersionSettings();

                //loop through each property to get the data
                foreach (JProperty property in obj.Properties())
                {
                    string name = property.Name;

                    if (name.Equals(nameof(current_GameVersionSettings.Game_Location)))
                        new_gameVersionSettings.Game_Location = (string)property.Value;

                    if (name.Equals(nameof(current_GameVersionSettings.Game_LocationExe)))
                        new_gameVersionSettings.Game_LocationExe = (string)property.Value;

                    if (name.Equals(nameof(current_GameVersionSettings.Game_Version)))
                        new_gameVersionSettings.Game_Version = (GameVersion)(int)property.Value;
                }

                GameVersionSettings.Add(new_gameVersionSettings);
            }

            foreach(GameVersionSettings version in GameVersionSettings)
            {
                if (appSettingsFile.Default_Game_Version.Equals(version.Game_Version))
                    current_GameVersionSettings = version;
            }
        }

        /// <summary>
        /// Writes existing values of the App Settings objects into the config file.
        /// </summary>
        public void WriteToFile()
        {
            if(File.Exists(configFile_file_location))
                IOManagement.DeleteFile(configFile_file_location);

            //open a stream writer to create the text file and write to it
            using (StreamWriter file = File.CreateText(configFile_file_location))
            {
                //get our json seralizer
                JsonSerializer serializer = new JsonSerializer();

                List<object> jsonObjects = new List<object>();
                jsonObjects.Add(appSettingsFile);
                jsonObjects.Add(GameVersionSettings);

                //seralize the data and write it to the configruation file
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, jsonObjects);
            }
        }

        /// <summary>
        /// Updates the changes to the app config file by replacing it and writing a new one. (there is a better way of doing it, but this works fine)
        /// </summary>
        public void UpdateChangesToFile()
        {
            IOManagement.DeleteFile(configFile_file_location);
            WriteToFile();
        }

        /// <summary>
        /// Checks the current selected game version values if the following values are assigned/exist.
        /// <para>Game_LocationExe, Game_Location, and Game_Location_Mods</para>
        /// <para>returns false if one or all of these values aren't assigned/exist</para>
        /// </summary>
        /// <returns></returns>
        public bool IsGameSetupAndValid(bool showMessageBoxes = false)
        {
            if (current_GameVersionSettings == null)
                return true;

            if (File.Exists(current_GameVersionSettings.Game_LocationExe) == false)
            {
                if (showMessageBoxes)
                    MessageBoxes.Error("The game executable location does not exist!", "Error");

                return false;
            }

            if (Directory.Exists(current_GameVersionSettings.Game_Location) == false)
            {
                if (showMessageBoxes)
                    MessageBoxes.Error("The directory of the game does not exist!", "Error");

                return false;
            }

            if (Directory.Exists(current_GameVersionSettings.Game_Location_Mods) == false)
            {
                if (showMessageBoxes)
                    MessageBoxes.Error("The directory of the game archives/pack folder does not exist!", "Error");

                return false;
            }

            return true;
        }

        /// <summary>
        /// Opens up the default web explorer and directs the user to the launcher help page
        /// </summary>
        public void Open_LauncherHelp()
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = launcherHelpLink,
                UseShellExecute = true
            };

            Process.Start(processStartInfo);
        }

        /// <summary>
        /// Opens up the default web explorer and directs the user to the launcher help page
        /// </summary>
        public void Open_LauncherHelpSetup()
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = launcherHelpSetupLink,
                UseShellExecute = true
            };

            Process.Start(processStartInfo);
        }

        //---------------- GETTERS ----------------
        public string Get_Current_GameVersionSettings_LibTelltaleGameID()
        {
            if (current_GameVersionSettings == null)
                return "";

            return current_GameVersionSettings.Game_LibTelltale_GameID;
        }

        public string Get_Current_GameVersionSettings_GameDirectory()
        {
            if (current_GameVersionSettings == null)
                return "";

            return current_GameVersionSettings.Game_Location;
        }

        public string Get_Current_GameVersionSettings_GameExeLocation()
        {
            if (current_GameVersionSettings == null)
                return "";

            return current_GameVersionSettings.Game_LocationExe;
        }

        public string Get_Current_GameVersionSettings_ModsLocation()
        {
            if (current_GameVersionSettings == null)
                return "";

            return current_GameVersionSettings.Game_Location_Mods;
        }

        public GameVersion Get_Current_GameVersionName()
        {
            if (current_GameVersionSettings == null)
                return GameVersion.Other;

            return current_GameVersionSettings.Game_Version;
        }

        public bool Get_AppSettings_LightMode()
        {
            return appSettingsFile.UI_LightMode;
        }

        public bool Get_AppSettings_DefocusEffect()
        {
            return appSettingsFile.UI_WindowDefocusing;
        }

        public string Get_App_ConfigDirectory()
        {
            return configFile_directory_location;
        }
        //---------------- GETTERS END ----------------
        //---------------- MODIFIERS ----------------
        public void Set_Current_AppSettings_DefaultGameVersion(GameVersion version)
        {
            appSettingsFile.Default_Game_Version = version;
        }

        public void Set_Current_AppSettings_UI_LightMode(bool state)
        {
            appSettingsFile.UI_LightMode = state;
        }

        public void Set_Current_AppSettings_UI_DefocusEffect(bool state)
        {
            appSettingsFile.UI_WindowDefocusing = state;
        }

        public void Set_Current_GameVersionSettings(int selectedIndex)
        {
            current_GameVersionSettings = GameVersionSettings[selectedIndex];
        }

        public void Set_Current_GameVersionSettings_GameExeLocation()
        {
            string newFilePath = "";

            IOManagement.GetFilePath(ref newFilePath, "Exe Files (.exe)|*.exe", "Select the Game Executable File");

            if (File.Exists(newFilePath))
            {
                current_GameVersionSettings.Game_Location = Path.GetDirectoryName(newFilePath);
                current_GameVersionSettings.Game_LocationExe = newFilePath;
            }
        }
        //---------------- MODIFIERS END ----------------
    }
}
