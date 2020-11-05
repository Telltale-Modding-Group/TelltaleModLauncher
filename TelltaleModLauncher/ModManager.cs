using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
        public List<Mod> mods;

        //for testing
        public string tempModInfoFile = "A:/Work/MODDING/Github/TESTING-DIR/modinfo.json";

        //private variables
        private IOManagement ioManagement;
        private AppSettings appSettings;
        private string modZipFilePath;

        public ModManager (AppSettings appSettings, IOManagement ioManagement)
        {
            this.appSettings = appSettings;
            this.ioManagement = ioManagement;

            mods = new List<Mod>();
        }

        /// <summary>
        /// Adds a mod to the mod manager.
        /// <para>Will prompt the user with a file browser to select a mod file (.zip)</para>
        /// </summary>
        public void AddMod()
        {
            string filePath = "";

            ioManagement.GetFilePath(ref filePath, "ZIP files|*.zip", "Select a Mod File");

            if (String.IsNullOrEmpty(filePath) == false)
                ReadModZipFile(filePath);
        }

        /// <summary>
        /// Removes a mod from the mod manager.
        /// <para>Will prompt the user if they want to remove the file.</para>
        /// <para>If yes, it will remove the files associated with the mod on the disk, and remove it from the mod manager.</para>
        /// </summary>
        /// <param name="selectedIndex"></param>
        public void RemoveMod(int selectedIndex)
        {
            Mod selectedMod = mods[selectedIndex];

            string promptDescription = string.Format("Are you sure you want to remove '{0}'?", selectedMod.ModDisplayName);

            if (ioManagement.MessageBox_Confirmation(promptDescription, "Remove Mod"))
            {
                mods.RemoveAt(selectedIndex);
            }
        }

        /// <summary>
        /// Removes all mods from the mod manager.
        /// <para>Will prompt the user if they wish to proceed.</para>
        /// <para>If yes, it will remove all mods from the manager and the files associated with each from the disk.</para>
        /// </summary>
        public void PurgeMods()
        {
            if (ioManagement.MessageBox_Confirmation("Are you sure you want to purge the mod directory? This will remove all the mod files currently installed. This action can't be reverted.", "Purge Mods"))
            {
                string dir = "";

                foreach (Mod mod in mods)
                {
                    foreach (string modfile in mod.ModFiles)
                    {
                        string modfile_pathOnDisk = modfile;
                        ioManagement.DeleteFile(modfile_pathOnDisk);
                    }
                }

                foreach (string modJsonFile in ioManagement.GetFilesPathsByExtension(dir, ".json"))
                {
                    ioManagement.DeleteFile(modJsonFile);
                }

                mods.Clear();
            }
        }

        public void ExtractModZipFileContents_ToDirectory()
        {
            ZipFile.ExtractToDirectory(modZipFilePath, appSettings.Get_Current_GameVersionSettings_ModsLocation());
        }

        public void ReadModZipFile(string newModZipFilePath)
        {
            modZipFilePath = newModZipFilePath;
            modZipFilePath = Path.GetFullPath(modZipFilePath);
            string modZipFile_ParentDirectory = Path.GetDirectoryName(modZipFilePath);
            string modJson_extractedFromArchive = "";

            // Ensures that the last character on the extraction path is the directory separator char.
            if (!modZipFile_ParentDirectory.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                modZipFile_ParentDirectory += Path.DirectorySeparatorChar;

            using (ZipArchive archive = ZipFile.OpenRead(modZipFilePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                    {
                        // Gets the full path to ensure that relative segments are removed.
                        modJson_extractedFromArchive = Path.GetFullPath(Path.Combine(modZipFile_ParentDirectory, entry.Name));

                        if(File.Exists(modJson_extractedFromArchive))
                            ioManagement.DeleteFile(modJson_extractedFromArchive);

                        // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that are case-insensitive.
                        if (modJson_extractedFromArchive.StartsWith(modZipFile_ParentDirectory, StringComparison.Ordinal))
                            entry.ExtractToFile(modJson_extractedFromArchive);
                    }
                }
            }

            Json_ReadModFile(modJson_extractedFromArchive);
        }

        /// <summary>
        /// Reads the Game Mod Folder for any existing mods by looking for a modinfo.json file.
        /// <para>Will fill the mods list on the mod manager if there are any found.</para>
        /// </summary>
        public void GetModsFromFolder()
        {
            List<string> modJsonFilesPath = ioManagement.GetFilesPathsByExtension(appSettings.Get_Current_GameVersionSettings_ModsLocation(), ".json");

            foreach(string jsonFilePath in modJsonFilesPath)
            {
                Json_ReadModFile(jsonFilePath, true);
            }
        }

        /// <summary>
        /// Writes a brand new modinfo.json file
        /// </summary>
        /// <param name="modFileLocation"></param>
        /// <param name="mod"></param>
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

        /// <summary>
        /// Replaces an existing mod with a new mod.
        /// <para>Also replaces the files associated with the existing mod with the new one.</para>
        /// </summary>
        /// <param name="orignalMod"></param>
        /// <param name="newMod"></param>
        private void ReplaceMod(Mod orignalMod, Mod newMod)
        {
            mods.Remove(orignalMod);
            mods.Add(newMod);
        }

        /// <summary>
        /// Adds a mod with a bunch of validation checks.
        /// <para>If a validation check fails, it will not add the mod.</para>
        /// </summary>
        /// <param name="newMod"></param>
        public void ValidateAndAddMod(Mod newMod)
        {
            if(mods.Count < 1)
            {
                mods.Add(newMod);
                ExtractModZipFileContents_ToDirectory();

                return;
            }

            foreach (Mod mod in mods)
            {
                if(mod.ModDisplayName.Equals(newMod.ModDisplayName))
                {
                    if (mod.ModVersion.Equals(newMod.ModVersion))
                    {
                        string message = string.Format("You already have '{0}' version '{1}' installed!", newMod.ModDisplayName, newMod.ModVersion);

                        MessageBox.Show(message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        return;
                    }
                    else
                    {
                        string message = string.Format("You have '{0}' installed with version '{1}'. Do you wish to replace it with this version {2}?", mod.ModDisplayName, mod.ModVersion, newMod.ModVersion);

                        if(ioManagement.MessageBox_Confirmation(message, "Replace Mod"))
                        {
                            ReplaceMod(mod, newMod);
                        }

                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Reads a modinfo.json file and parses the data into a new mod object.
        /// <para>After sucessfully getting the data, it will execute ValidateAndAddMod()</para>
        /// </summary>
        /// <param name="modJsonFile"></param>
        public void Json_ReadModFile(string modJsonFile, bool bypassValidation = false)
        {
            //create a new mod object
            Mod newMod = new Mod();

            //read the data from the config file
            string jsonText = File.ReadAllText(modJsonFile);

            //parse the data into a json array
            JObject obj = JObject.Parse(jsonText);

            //loop through each property to get the data
            foreach (JProperty property in obj.Properties())
            {
                string name = property.Name;

                if (name.Equals(nameof(Mod.ModAuthor)))
                    newMod.ModAuthor = (string)property.Value;

                if (name.Equals(nameof(Mod.ModCompatibility)))
                    newMod.ModCompatibility = (string)property.Value;

                if (name.Equals(nameof(Mod.ModDisplayName)))
                    newMod.ModDisplayName = (string)property.Value;

                if (name.Equals(nameof(Mod.ModFiles)))
                {
                    JArray fileArray = (JArray)obj[nameof(Mod.ModFiles)];
                    List<string> parsedModFiles = new List<string>();

                    foreach(JValue file in fileArray)
                    {
                        parsedModFiles.Add((string)file.Value);
                    }

                    newMod.ModFiles = parsedModFiles;
                }

                if (name.Equals(nameof(Mod.ModVersion)))
                    newMod.ModVersion = (string)property.Value;
            }

            if(bypassValidation)
            {
                mods.Add(newMod);
            }
            else
            {
                ValidateAndAddMod(newMod);
            }
        }

        public string GetGameVersion(int gameVersion)
        {
            return GameVersion.The_Walking_Dead_Definitive_Edition.ToString();
        }

        public GameVersion ParseGameVersion(string gameVersion)
        {
            return (GameVersion)Enum.Parse(typeof(GameVersion), gameVersion);
        }
    }
}
