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

namespace TelltaleModLauncher
{
    /// <summary>
    /// Main class pertaining to the application settings.
    /// </summary>
    class AppSettings
    {
        //public
        public AppSettingsFile appSettingsFile;
        public List<GameVersionSettings> GameVersionSettings { get; set; }
        public GameVersionSettings current_GameVersionSettings;

        //private
        private IOManagement ioManagement;
        private static string systemDocumentsPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string configFile_filename = "TelltaleModLauncher_Config.json";
        private static string configFile_directory_location = systemDocumentsPath + "/TelltaleModLauncher/";
        private static string configFile_file_location = configFile_directory_location + configFile_filename;

        /// <summary>
        /// Application Settings Class, creates an AppSettings object. This is called on application startup.
        /// <para>If there is an existing config file, it will parse the data from it.</para>
        /// <para>If there is not an existing config file, or a TelltaleModLauncher directory, create a new one.</para>
        /// </summary>
        public AppSettings(IOManagement ioManagement)
        {
            this.ioManagement = ioManagement;

            if (File.Exists(configFile_file_location))
            {
                ReadConfigFile();
            }
            else
            {
                appSettingsFile = new AppSettingsFile();
                New_GameVersionSettingsList();

                current_GameVersionSettings = GameVersionSettings[1];

                if (!Directory.Exists(configFile_directory_location))
                    ioManagement.CreateDirectory(configFile_directory_location);

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

            var enumList = Enum.GetValues(typeof(GameVersion)).Cast<GameVersion>();
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

                if (name.Equals(nameof(appSettingsFile.Location_LuaCompiler)))
                    appSettingsFile.Location_LuaCompiler = (string)property.Value;

                if (name.Equals(nameof(appSettingsFile.Location_LuaDecompiler)))
                    appSettingsFile.Location_LuaDecompiler = (string)property.Value;

                if (name.Equals(nameof(appSettingsFile.Location_Ttarchext)))
                    appSettingsFile.Location_Ttarchext = (string)property.Value;

                if (name.Equals(nameof(appSettingsFile.UI_LightMode)))
                    appSettingsFile.UI_LightMode = (bool)property.Value;
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

                    if (name.Equals(nameof(current_GameVersionSettings.Game_Location_Mods)))
                        new_gameVersionSettings.Game_Location_Mods = (string)property.Value;

                    if (name.Equals(nameof(current_GameVersionSettings.Game_Ttarch_GameEnumNumber)))
                        new_gameVersionSettings.Game_Ttarch_GameEnumNumber = (int)property.Value;

                    if (name.Equals(nameof(current_GameVersionSettings.Game_Version)))
                    {
                        new_gameVersionSettings.Game_Version = (GameVersion)(int)property.Value;
                    }
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
                ioManagement.DeleteFile(configFile_file_location);

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
            ioManagement.DeleteFile(configFile_file_location);
            WriteToFile();
        }

        /// <summary>
        /// Checks the current selected game version values if the following values are assigned/exist.
        /// <para>Game_LocationExe, Game_Location, and Game_Location_Mods</para>
        /// <para>returns false if one or all of these values aren't assigned/exist</para>
        /// </summary>
        /// <returns></returns>
        public bool IsGameSetupAndValid()
        {
            if (File.Exists(current_GameVersionSettings.Game_LocationExe) == false)
                return false;

            if (Directory.Exists(current_GameVersionSettings.Game_Location) == false)
                return false;

            if (Directory.Exists(current_GameVersionSettings.Game_Location_Mods) == false)
                return false;

            return true;
        }

        //---------------- GETTERS ----------------
        public int Get_Current_GameVersionSettings_ttarchNumber()
        {
            return current_GameVersionSettings.Game_Ttarch_GameEnumNumber;
        }

        public string Get_Current_GameVersionSettings_GameDirectory()
        {
            return current_GameVersionSettings.Game_Location;
        }

        public string Get_Current_GameVersionSettings_GameExeLocation()
        {
            return current_GameVersionSettings.Game_LocationExe;
        }

        public string Get_Current_GameVersionSettings_ModsLocation()
        {
            return current_GameVersionSettings.Game_Location_Mods;
        }

        public GameVersion Get_Current_GameVersionName()
        {
            return current_GameVersionSettings.Game_Version;
        }
        //---------------- GETTERS END ----------------
        //---------------- MODIFIERS ----------------
        public void Set_Current_AppSettings_DefaultGameVersion(GameVersion version)
        {
            appSettingsFile.Default_Game_Version = version;
        }

        public void Set_Current_AppSettings_LuaCompilerLocation(string location)
        {
            appSettingsFile.Location_LuaCompiler = location;
        }

        public void Set_Current_AppSettings_LuaDecompilerLocation(string location)
        {
            appSettingsFile.Location_LuaDecompiler = location;
        }

        public void Set_Current_AppSettings_ttarchextLocation(string location)
        {
            appSettingsFile.Location_Ttarchext = location;
        }

        public void Set_Current_AppSettings_UI_LightMode(bool state)
        {
            appSettingsFile.UI_LightMode = state;
        }

        public void Set_Current_GameVersionSettings_EnumNumber(int value)
        {
            current_GameVersionSettings.Game_Ttarch_GameEnumNumber = value;
        }

        public void Set_Current_GameVersionSettings(int selectedIndex)
        {
            current_GameVersionSettings = GameVersionSettings[selectedIndex];
        }

        public void Set_Current_GameVersionSettings_GameModsDirectory(string newPath = "")
        {
            string newDirectory = "";

            ioManagement.GetFolderPath(ref newDirectory, "Select the Game Mods folder");

            current_GameVersionSettings.Game_Location_Mods = newDirectory;
        }

        public void Set_Current_GameVersionSettings_GameExeLocation(string newPath = "")
        {
            string newFilePath = "";

            ioManagement.GetFilePath(ref newFilePath, "Exe Files (.exe)|*.exe", "Select the Game Executable File");

            if (File.Exists(newFilePath))
            {
                current_GameVersionSettings.Game_Location = Path.GetDirectoryName(newFilePath);
                current_GameVersionSettings.Game_LocationExe = newFilePath;
            }
        }
        //---------------- MODIFIERS END ----------------
    }
}
