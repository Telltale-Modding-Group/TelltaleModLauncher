using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace TelltaleModLauncher
{
    class AppSettings
    {
        public AppSettingsFile appSettingsFile;
        public List<GameVersionSettings> GameVersionSettings { get; set; }
        public GameVersionSettings current_GameVersionSettings;

        private IOManagement ioManagement;
        private static string systemDocumentsPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string configFile_filename = "TelltaleModLauncher_Config.json";
        private static string configFile_directory_location = systemDocumentsPath + "/TelltaleModLauncher/";
        private static string configFile_file_location = configFile_directory_location + configFile_filename;

        /// <summary>
        /// Application Settings Class
        /// </summary>
        public AppSettings(IOManagement ioManagement)
        {
            this.ioManagement = ioManagement;

            current_GameVersionSettings = new GameVersionSettings();

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

        private void New_GameVersionSettingsList()
        {
            GameVersionSettings = new List<GameVersionSettings>();

            List<string> enumList = Enum.GetNames(typeof(GameVersion)).ToList();

            for (int i = 0; i < enumList.Count; i++)
            {
                GameVersionSettings new_gameVersionSettings = new GameVersionSettings();
                new_gameVersionSettings.Game_Version = enumList[i];

                GameVersionSettings.Add(new_gameVersionSettings);
            }
        }

        public void ReadConfigFile()
        {
            appSettingsFile = new AppSettingsFile();
            GameVersionSettings = new List<GameVersionSettings>();

            //read the data from the config file
            string jsonText = File.ReadAllText(configFile_file_location);

            //parse the data into a json array
            JArray array = JArray.Parse(jsonText);

            JObject AppSettingsFile_fromJson = array[0] as JObject;
            JArray GameVersionSettings_fromJson = array[1] as JArray;

            //loop through each property to get the data
            foreach (JProperty property in AppSettingsFile_fromJson.Properties())
            {
                string name = property.Name;

                if (name.Equals(nameof(appSettingsFile.Default_Game_Version)))
                    appSettingsFile.Default_Game_Version = (string)property.Value;

                if (name.Equals(nameof(appSettingsFile.Location_LuaCompiler)))
                    appSettingsFile.Location_LuaCompiler = (string)property.Value;

                if (name.Equals(nameof(appSettingsFile.Location_LuaDecompiler)))
                    appSettingsFile.Location_LuaDecompiler = (string)property.Value;

                if (name.Equals(nameof(appSettingsFile.Location_Ttarchext)))
                    appSettingsFile.Location_Ttarchext = (string)property.Value;
            }

            //loop through each property to get the data
            foreach (JObject obj in GameVersionSettings_fromJson.Children<JObject>())
            {
                GameVersionSettings new_gameVersionSettings = new GameVersionSettings();

                //loop through each property to get the data
                foreach (JProperty property in obj.Properties())
                {
                    string name = property.Name;

                    if (name.Equals(nameof(appSettingsFile.Default_Game_Version)))
                        new_gameVersionSettings.Game_Location = (string)property.Value;

                    if (name.Equals(nameof(appSettingsFile.Location_LuaCompiler)))
                        new_gameVersionSettings.Game_LocationExe = (string)property.Value;

                    if (name.Equals(nameof(appSettingsFile.Location_LuaDecompiler)))
                        new_gameVersionSettings.Game_Location_Mods = (string)property.Value;

                    if (name.Equals(nameof(appSettingsFile.Location_Ttarchext)))
                        new_gameVersionSettings.Game_Ttarch_GameEnumNumber = (int)property.Value;

                    if (name.Equals(nameof(appSettingsFile.Location_Ttarchext)))
                        new_gameVersionSettings.Game_Version = (string)property.Value;
                }

                GameVersionSettings.Add(new_gameVersionSettings);
            }
        }

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

        public void UpdateChangesToFile()
        {
            ioManagement.DeleteFile(configFile_file_location);
            WriteToFile();
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

        public string Get_Current_GameVersionName()
        {
            return current_GameVersionSettings.Game_Version;
        }
        //---------------- GETTERS END ----------------
        //---------------- MODIFIERS ----------------
        public void Set_Current_AppSettings_DefaultGameVersion(string version)
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

            ioManagement.GetFolderPath(ref newFilePath, "Select the Game Executable File");

            if (File.Exists(newFilePath))
            {
                current_GameVersionSettings.Game_Location = Path.GetDirectoryName(newFilePath);
                current_GameVersionSettings.Game_LocationExe = newFilePath;
            }
        }
        //---------------- MODIFIERS END ----------------
    }
}
