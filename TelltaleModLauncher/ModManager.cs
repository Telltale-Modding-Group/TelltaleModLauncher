using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace TelltaleModLauncher
{
    /// <summary>
    /// The main class for the mod manager functionality.
    /// </summary>
    class ModManager
    {
        //public variables
        public List<Mod> mods = new List<Mod>();

        public string tempModInfoFile = "A:/Work/MODDING/Github/TESTING-DIR/modinfo.json";

        //private variables
        private IOManagement ioManagement = new IOManagement();


        public ModManager ()
        {
            //all of the values here are temporary and are for testing purposes
            List<string> modFiles = new List<string>();
            modFiles.Add("_resdesc_50_Boot_LoadAnyLevel.lua");
            modFiles.Add("_resdesc_50_Menu_LoadAnyLevel.lua");
            modFiles.Add("WDC_pc_Boot_data_LoadAnyLevel.ttarch2");
            modFiles.Add("WDC_pc_Menu_data_LoadAnyLevel.ttarch2");
            Mod mod1 = new Mod("Load Any Level", "4.0.0", "droyti", modFiles);




            Mod mod2 = new Mod("Season 4 Graphic Black Disabler", "1.0.0", "changemymindpls1", new List<string>());
            Mod mod3 = new Mod("Season 1 Weapon Sounds Overhaul", "1.5.0", "changemymindpls1", new List<string>());
            Mod mod4 = new Mod("Ambience", "1.0.0", "droyti", new List<string>());
            Mod mod5 = new Mod("Super Speed for S1", "1.0.0", "imdogshitatwritingcode", new List<string>());
            Mod mod6 = new Mod("Graphics Enhancement Definitve Series", "2.0.0", "jesuschrist", new List<string>());
            Mod mod7 = new Mod("Telltale Trainer", "2.0.0", "stolenFromGtaV", new List<string>());

            mods.Add(mod1);
            mods.Add(mod2);
            mods.Add(mod3);
            mods.Add(mod4);
            mods.Add(mod5);
            mods.Add(mod6);
            mods.Add(mod7);
        }

        public void ReadModZipFile()
        {

        }

        public void ExtractModZipFile()
        {

        }

        public void AddMod()
        {
            string filePath = "";

            ioManagement.GetFilePath(ref filePath, "ZIP files|*.zip", "Select a Mod File");
        }

        public void RemoveMod(int selectedIndex)
        {
            Mod selectedMod = mods[selectedIndex];

            string promptDescription = string.Format("Are you sure you want to remove {0}?", selectedMod.ModDisplayName);

            if(ioManagement.MessageBox_Confirmation(promptDescription, "Remove Mod"))
            {
                mods.RemoveAt(selectedIndex);
            }
        }

        public void PurgeMods()
        {
            //temporarily here to test functionaltiy
            Json_WriteModFile();

            //if (ioManagement.MessageBox_Confirmation("Are you sure you want to purge the mod directory? This will remove all the mod files currently installed. This action can't be reverted.", "Purge Mods"))
            //{
                //mods.Clear();
            //}
        }

        public void GetModsFromFolder()
        {

        }

        public void Json_WriteModFile()
        {
            //open a stream writer to create the text file and write to it
            using (StreamWriter file = File.CreateText(tempModInfoFile))
            {
                //get our json seralizer
                JsonSerializer serializer = new JsonSerializer();

                //seralize the data and write it to the configruation file
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, mods[0]);
            }
        }

        public void Json_ReadModFile()
        {
            //read the data from the config file
            string jsonText = File.ReadAllText(tempModInfoFile);

            //parse the data into a json array
            JArray array = JArray.Parse(jsonText);

            /*
            //loop through each jarray to look for the current game version
            for (int i = 0; i < fromFile_gameSettingsList.Children<JObject>().Count(); i++)
            {
                JObject obj = fromFile_gameSettingsList.Children<JObject>().ElementAt(i);
                GameVersionSettings gameVersionSettings = gameVersionSettingsList[i];

                bool breakEarly = false;

                //loop through each property to get the data regarding the current game version
                foreach (JProperty property in obj.Properties())
                {
                    string name = property.Name;
                    string value = (string)property.Value;

                    if (Current_Game_Version.Equals(value))
                        breakEarly = true;

                    if (name.Equals("Game_Location"))
                        Current_Game_Location = value;

                    if (name.Equals("Game_Location_Mods"))
                        Current_Game_Location_Mods = value;

                    if (name.Equals("Game_Ttarch_GameEnumNumber"))
                        Current_Game_TtarchGameEnumNumber = int.Parse(value);
                }

                //if we found the game version were on, break out of the loop early
                if (breakEarly)
                    break;
            }
            */

            /*
            //read the data from the config file
            string jsonText = File.ReadAllText(config_file_location);

            //parse the data into json object
            JArray array = JArray.Parse(jsonText);
            JObject fromFile_appConfig = (JObject)array[0];

            //loop through the properties in the json object to get our data
            foreach (JProperty property in fromFile_appConfig.Properties())
            {
                string name = property.Name;
                string value = (string)property.Value;

                if (name.Equals("Default_Game_Version"))
                    Default_Game_Version = value;

                if (name.Equals("Location_Ttarchext"))
                    Location_Ttarchext = value;
            }
            */
        }
    }
}
