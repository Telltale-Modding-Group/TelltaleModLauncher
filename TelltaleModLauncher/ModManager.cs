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

        public string GetGameVersion(int gameVersion)
        {
            return GameVersion.The_Walking_Dead_Definitive_Edition.ToString();
        }

        public GameVersion ParseGameVersion(string gameVersion)
        {
            return (GameVersion)Enum.Parse(typeof(GameVersion), gameVersion);
        }

        public ModManager ()
        {
            
            //all of the values here are temporary and are for testing purposes
            List<string> modFiles = new List<string>();
            modFiles.Add("_resdesc_50_Boot_LoadAnyLevel.lua");
            modFiles.Add("_resdesc_50_Menu_LoadAnyLevel.lua");
            modFiles.Add("WDC_pc_Boot_data_LoadAnyLevel.ttarch2");
            modFiles.Add("WDC_pc_Menu_data_LoadAnyLevel.ttarch2");
            Mod mod1 = new Mod("Load Any Level", "4.0.0", "droyti", modFiles, GetGameVersion(1));
            mods.Add(mod1);
            
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

            string promptDescription = string.Format("Are you sure you want to remove '{0}'?", selectedMod.ModDisplayName);

            if(ioManagement.MessageBox_Confirmation(promptDescription, "Remove Mod"))
            {
                mods.RemoveAt(selectedIndex);
            }
        }

        public void PurgeMods()
        {
            if (ioManagement.MessageBox_Confirmation("Are you sure you want to purge the mod directory? This will remove all the mod files currently installed. This action can't be reverted.", "Purge Mods"))
            {
                //ioManagement.DeleteFilesInDirectory("", false);

                mods.Clear();
            }
        }

        public void GetModsFromFolder()
        {

        }

        public void Json_WriteModFile(string modFileLocation, Mod mod)
        {
            //open a stream writer to create the text file and write to it
            using (StreamWriter file = File.CreateText(modFileLocation))
            {
                //get our json seralizer
                JsonSerializer serializer = new JsonSerializer();

                //seralize the data and write it to the configruation file
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, mod);
            }
        }

        public void Json_ReadModFile(string modJsonFile)
        {
            Mod newMod = new Mod();

            //read the data from the config file
            string jsonText = File.ReadAllText(modJsonFile);

            //parse the data into a json array
            JArray array = JArray.Parse(jsonText);

            //loop through each jobject in the array
            foreach (JObject obj in array.Children<JObject>())
            {
                //loop through each property to get the data
                foreach (JProperty property in obj.Properties())
                {
                    string name = property.Name;
                    string value = (string)property.Value;

                    if (name.Equals(nameof(Mod.ModAuthor)))
                        newMod.ModAuthor = value;

                    if (name.Equals(nameof(Mod.ModCompatibility)))
                        newMod.ModCompatibility = value;

                    if (name.Equals(nameof(Mod.ModDisplayName)))
                        newMod.ModDisplayName = value;

                    if (name.Equals(nameof(Mod.ModFiles)))
                    {
                        //newMod.ModFiles = (List<string>)property.Value;
                    }

                    if (name.Equals(nameof(Mod.ModVersion)))
                        newMod.ModVersion = value;
                }
            }
        }
    }
}
