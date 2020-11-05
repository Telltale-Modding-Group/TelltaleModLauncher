using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
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
                RemoveModFiles(selectedMod);
                ioManagement.DeleteFile(selectedMod.Get_ModInfoJson_FilePath());
                mods.Remove(selectedMod);
            }
        }

        public void RemoveModFiles(Mod mod)
        {
            foreach (string modfile in mod.ModFiles)
            {
                string modfile_pathOnDisk = Path.Combine(appSettings.Get_Current_GameVersionSettings_ModsLocation(), modfile);
                ioManagement.DeleteFile(modfile_pathOnDisk);
            }
        }

        public void CheckIfModFilesExist(Mod mod, bool showPrompt = true)
        {
            List<string> missingModFiles = new List<string>();

            foreach(string modfile in mod.ModFiles)
            {
                string modfile_pathOnDisk = Path.Combine(appSettings.Get_Current_GameVersionSettings_ModsLocation(), modfile);

                if (!File.Exists(modfile_pathOnDisk))
                {
                    missingModFiles.Add(modfile);
                }
            }

            string missingFileString = string.Join(", ", missingModFiles);
            string message = string.Format("The following files for the mod '{0}' are missing! {1}. Do you want to remove the mod and it's contents?", mod.ModDisplayName, missingFileString);

            if (ioManagement.MessageBox_Confirmation(message, "Missing Mod Files!", MessageBoxIcon.Error))
            {
                RemoveMod(mods.IndexOf(mod));
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
                string dir = appSettings.Get_Current_GameVersionSettings_ModsLocation();

                foreach (Mod mod in mods)
                {
                    foreach (string modfile in mod.ModFiles)
                    {
                        string modfile_pathOnDisk = Path.Combine(appSettings.Get_Current_GameVersionSettings_ModsLocation(), modfile);
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

        /// <summary>
        /// Extracts the mod file zip contents to the Game Mods directory.
        /// </summary>
        public void ExtractModZipFileContents_ToDirectory(Mod mod)
        {
            using (ZipArchive archive = ZipFile.OpenRead(modZipFilePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (Path.HasExtension(entry.FullName))
                    {
                        // Gets the full path to ensure that relative segments are removed.
                        string modFile_extractedFromArchive = Path.GetFullPath(Path.Combine(appSettings.Get_Current_GameVersionSettings_ModsLocation(), entry.Name));

                        if (entry.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                            mod.Set_ModInfoJson_FilePath(modFile_extractedFromArchive);

                        // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that are case-insensitive.
                        if (modFile_extractedFromArchive.StartsWith(appSettings.Get_Current_GameVersionSettings_ModsLocation(), StringComparison.Ordinal))
                            entry.ExtractToFile(modFile_extractedFromArchive);
                    }
                }
            }
        }

        /// <summary>
        /// Reads a mod zip file.
        /// <para>Extracts the modinfo.json file</para>
        /// </summary>
        /// <param name="newModZipFilePath"></param>
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
            if (Directory.Exists(appSettings.Get_Current_GameVersionSettings_ModsLocation()))
            {
                List<string> modJsonFilesPath = ioManagement.GetFilesPathsByExtension(appSettings.Get_Current_GameVersionSettings_ModsLocation(), ".json");

                foreach (string jsonFilePath in modJsonFilesPath)
                {
                    Json_ReadModFile(jsonFilePath, true);
                }

                foreach(Mod mod in mods)
                {
                    CheckIfModFilesExist(mod);
                }
            }    
        }

        /// <summary>
        /// Opens the Game Mods folder with Windows Explorer.
        /// </summary>
        public void OpenModFolder()
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = appSettings.Get_Current_GameVersionSettings_ModsLocation();
            processStartInfo.UseShellExecute = true;
            processStartInfo.Verb = "open";

            Process.Start(processStartInfo);
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
            RemoveModFiles(orignalMod);
            ioManagement.DeleteFile(orignalMod.Get_ModInfoJson_FilePath());
            mods.Remove(orignalMod);

            ExtractModZipFileContents_ToDirectory(newMod);
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
                ExtractModZipFileContents_ToDirectory(newMod);

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
