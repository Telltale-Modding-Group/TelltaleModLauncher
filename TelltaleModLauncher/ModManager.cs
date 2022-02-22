using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using TelltaleModLauncher.Utillities;
using TelltaleModLauncher.Files;

namespace TelltaleModLauncher
{
    /// <summary>
    /// The main class for the mod manager functionality.
    /// </summary>
    public class ModManager
    {
        //public variables
        public List<Mod> mods;

        //private variables
        private AppSettings appSettings;
        private MainWindow mainWindow;
        private string modZipFilePath;

        private FileSystemWatcher fileSystemWatcher;

        /// <summary>
        /// Initalizes a new Mod Manager object. The following fields must be given.
        /// </summary>
        /// <param name="appSettings"></param>
        /// <param name="ioManagement"></param>
        public ModManager(AppSettings appSettings, MainWindow mainWindow)
        {
            this.appSettings = appSettings;
            this.mainWindow = mainWindow;

            mods = new List<Mod>();

            //Initalize_FileSystemWatcher();
        }

        //------------------------- MAIN ACTIONS -------------------------
        //------------------------- MAIN ACTIONS -------------------------
        //------------------------- MAIN ACTIONS -------------------------

        /// <summary>
        /// Adds a mod to the mod manager.
        /// <para>Will prompt the user with a file browser to select a mod file (.zip)</para>
        /// </summary>
        public void AddMod()
        {
            //create a temporary string to contain the path
            string filePath = "";

            //prompt the user with a file browser dialog
            IOManagement.GetFilePath(ref filePath, "ZIP files|*.zip", "Select a Mod File");

            //if the string is not empty (meaning the user has chosen their path) read the zip file for a modinfo.json
            if (String.IsNullOrEmpty(filePath) == false)
                ReadModZipFile(filePath);
        }

        /// <summary>
        /// Removes a mod from the mod manager.
        /// <para>Will prompt the user if they want to remove the file.</para>
        /// <para>If yes, it will remove the files associated with the mod on the disk, and remove it from the mod manager.</para>
        /// </summary>
        /// <param name="selectedIndex"></param>
        public void RemoveMod(int selectedIndex, bool ignorePrompt = false)
        {
            //obtain the given mod at the selected index
            Mod selectedMod = mods[selectedIndex];

            if (ignorePrompt) //ignores the confirmation prompt and removes the mod object, mod contents, and json file
            {
                RemoveModFiles(selectedMod); //remove the files associated with the mod
                IOManagement.DeleteFile(selectedMod.ModInfoJson_FilePath); //remove the modinfo.json file
                mods.Remove(selectedMod); //remove the mod from the list in memory
            }
            else
            {
                //description for the confirmation prompt
                string promptDescription = string.Format("Are you sure you want to remove '{0}'?", selectedMod.ModDisplayName);

                //prompt the user about removing the mod
                if (MessageBoxes.Info_Confirm(promptDescription, "Remove Mod"))
                {
                    RemoveModFiles(selectedMod); //remove the files associated with the mod
                    IOManagement.DeleteFile(selectedMod.ModInfoJson_FilePath); //remove the modinfo.json file
                    mods.Remove(selectedMod); //remove the mod from the list in memory
                }
            }
        }

        /// <summary>
        /// Removes all mods from the mod manager.
        /// <para>Will prompt the user if they wish to proceed.</para>
        /// <para>If yes, it will remove all mods from the manager and the files associated with each from the disk.</para>
        /// </summary>
        public void PurgeMods()
        {
            //prompt the user upfront if they wish to remove all of the mods in the directory
            if (MessageBoxes.Warning_Confirm("Are you sure you want to purge the mod directory? This will remove all the mod files currently installed. This action can't be reverted.", "Purge Mods"))
            {
                //get the game mods location of the current game version
                string dir = appSettings.Get_Current_GameVersionSettings_ModsLocation();

                //loop through the mod list first and remove the files in relation to each mod
                foreach (Mod mod in mods)
                {
                    foreach (string modfile in mod.ModFiles)
                    {
                        string modfile_pathOnDisk = Path.Combine(appSettings.Get_Current_GameVersionSettings_ModsLocation(), modfile);
                        IOManagement.DeleteFile(modfile_pathOnDisk);
                    }
                }

                //loop through the mods directory again to get the .json files, and remove them
                foreach (string modJsonFile in IOManagement.GetFilesPathsByExtension(dir, ".json"))
                {
                    IOManagement.DeleteFile(modJsonFile);
                }

                //clear the mods list
                mods.Clear();
            }
        }

        /// <summary>
        /// Opens the Game Mods folder with Windows Explorer.
        /// </summary>
        public void OpenModFolder()
        {
            //create a windows explorer processinfo to be exectued
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = appSettings.Get_Current_GameVersionSettings_ModsLocation();
            processStartInfo.UseShellExecute = true;
            processStartInfo.Verb = "open";

            //start the process
            Process.Start(processStartInfo);
        }

        //------------------------- MAIN ACTIONS END -------------------------
        //------------------------- MAIN ACTIONS END -------------------------
        //------------------------- MAIN ACTIONS END -------------------------

        /// <summary>
        /// Initalizes the file system watcher object to watch the current game mods folder.
        /// TODO: STILL NEEDS WORK
        /// </summary>
        public void Initalize_FileSystemWatcher()
        {
            //get our values ready, look for .json files only
            string fileSystemWatcher_filter = "*.json";

            //if our app settings object is null, don't continue
            if (appSettings == null)
                return;

            //get the mod location of the current selected game
            string fileSystemWatcher_location = appSettings.Get_Current_GameVersionSettings_ModsLocation();

            //if our given directory location does not exist, don't continue
            if(!Directory.Exists(fileSystemWatcher_location))
                return;

            //if we already have a watcher, dispose of it
            if(fileSystemWatcher != null)
                fileSystemWatcher.Dispose();

            //initalize our file system watcher object
            fileSystemWatcher = new FileSystemWatcher(fileSystemWatcher_location, fileSystemWatcher_filter);
            fileSystemWatcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.Security;

            //add our events
            fileSystemWatcher.Changed += FileSystemWatcher_Changed;
            fileSystemWatcher.Created += FileSystemWatcher_Created;
            fileSystemWatcher.Deleted += FileSystemWatcher_Deleted;
            fileSystemWatcher.Renamed += FileSystemWatcher_Renamed;

            //initalize the file system watcher object
            fileSystemWatcher.EnableRaisingEvents = true;
            fileSystemWatcher.IncludeSubdirectories = false;
        }

        // TODO: STILL NEEDS WORK
        private void FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            GetModsFromFolder();
        }

        // TODO: STILL NEEDS WORK
        private void FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            GetModsFromFolder();
        }

        // TODO: STILL NEEDS WORK
        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            GetModsFromFolder();
        }

        // TODO: STILL NEEDS WORK
        private void FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            GetModsFromFolder();
        }

        /// <summary>
        /// Remove the mod files assoicated with the given mod on the disk.
        /// </summary>
        /// <param name="mod"></param>
        public void RemoveModFiles(Mod mod)
        {
            //loop through each of the mod files detailed in the mods .json file
            foreach (string modfile in mod.ModFiles)
            {
                string modfile_pathOnDisk = Path.Combine(appSettings.Get_Current_GameVersionSettings_ModsLocation(), modfile);

                if(File.Exists(modfile_pathOnDisk))
                    IOManagement.DeleteFile(modfile_pathOnDisk);
            }
        }

        /// <summary>
        /// Call this function when the game version is changed on the launcher, clears the list and checks the mod folder of the game version for mods
        /// </summary>
        public void ChangedGameVersion()
        {
            //check if the current selected game version has a mods location
            if(Directory.Exists(appSettings.Get_Current_GameVersionSettings_ModsLocation()) == false)
            {
                //clear the list
                mods.Clear();
            }
            else
            {
                mods.Clear();
                Initalize_FileSystemWatcher();
                GetModsFromFolder();
            }
        }

        /// <summary>
        /// Reads the Game Mod Folder for any existing mods by looking for a modinfo.json file.
        /// <para>Will fill the mods list on the mod manager if there are any found.</para>
        /// </summary>
        public void GetModsFromFolder()
        {
            //if the game mods directory exists
            if (Directory.Exists(appSettings.Get_Current_GameVersionSettings_ModsLocation()))
            {
                //clear the list
                mods.Clear();

                //get all of the modinfo.json files found in the folder
                List<string> modJsonFilesPath = IOManagement.GetFilesPathsByExtension(appSettings.Get_Current_GameVersionSettings_ModsLocation(), ".json");

                //go through the json files list and read each one to get the data
                foreach (string jsonFilePath in modJsonFilesPath)
                {
                    Json_ReadModFile_New(jsonFilePath, true, false);
                }

                //temp list for any bad mods found
                List<Mod> badMods = new List<Mod>();

                //loop through the existing mod list and check if each mod has its assoicated files
                foreach (Mod mod in mods)
                {
                    //if the given mod does not pass the check, then its missing files! so add it to the list
                    if (CheckIfModFilesExist(mod) == false)
                    {
                        badMods.Add(mod);
                    }
                }

                //loop through the list of the bad mods that have been found and remove each one.
                //NOTE: FUTURE REFERENCE, Give the user a prompt that notifies them. For now we will handle removing the bad mod automatically.
                foreach (Mod badMod in badMods)
                {
                    RemoveMod(mods.IndexOf(badMod), true);
                }
            }

            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                //update the UI on the main window
                mainWindow.UpdateUI();
            }));
        }

        /// <summary>
        /// Extracts the mod file zip contents to the Game Mods directory.
        /// </summary>
        public void ExtractModZipFileContents_ToDirectory(Mod mod)
        {
            //extract entire zip file's contents to the mods folder
            using (ZipArchive archive = ZipFile.OpenRead(modZipFilePath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    //check if the given entry has an extension, i.e. check if it is a file and not a folder (we want to extract the contents, not the folder)
                    if (Path.HasExtension(entry.FullName))
                    {
                        // Gets the full path to ensure that relative segments are removed.
                        string modFile_extractedFromArchive = Path.GetFullPath(Path.Combine(appSettings.Get_Current_GameVersionSettings_ModsLocation(), entry.Name));

                        //checks the extension for a .json file, if one is found then it sets it for the mod
                        if (entry.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                            mod.ModInfoJson_FilePath = modFile_extractedFromArchive;

                        // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that are case-insensitive.
                        if (modFile_extractedFromArchive.StartsWith(appSettings.Get_Current_GameVersionSettings_ModsLocation(), StringComparison.Ordinal))
                        {
                            //if there is an existing modfile that shares the same path/extension and everything, remove it
                            //NOTE: in the future, want to prompt the user about replacing mod files
                            if (File.Exists(modFile_extractedFromArchive))
                                IOManagement.DeleteFile(modFile_extractedFromArchive);

                            foreach (string gameDirFile in mod.ModFilesGameDirectory)
                            {
                                if (gameDirFile.Equals(entry.Name))
                                {

                                }
                            }

                            //extract the given entry to the game mods directory
                            entry.ExtractToFile(modFile_extractedFromArchive);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check if the given mod files exist. If the files are missing for the mod, it prompts the user if they wish to remove the mod.
        /// <para>returns false is there are missing mod files.</para>
        /// <para>returns true is there are no missing mod files.</para>
        /// </summary>
        /// <param name="mod"></param>
        /// <param name="showPrompt"></param>
        public bool CheckIfModFilesExist(Mod mod)
        {
            //initalize a temporary list for the missing mod file paths
            List<string> missingModFiles = new List<string>();

            //loop through the modfiles list in the mod for any missing files
            foreach (string modfile in mod.ModFiles)
            {
                string modfile_pathOnDisk = Path.Combine(appSettings.Get_Current_GameVersionSettings_ModsLocation(), modfile);

                //if the modfile does not exist on the disk, add it to the missing files list.
                if (!File.Exists(modfile_pathOnDisk))
                {
                    missingModFiles.Add(modfile);
                }
            }

            if (missingModFiles.Count != 0)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Checks the given mod's contents with all the other mod's contents found in the main mods list.
        /// </summary>
        /// <param name="newMod"></param>
        public void CheckModContents_WithExistingMods(Mod newMod, bool addMod = false)
        {
            List<Mod> conflictingMods = new List<Mod>(); //temp list for the conflicting mods
            List<string> conflictingModNames = new List<string>(); //temp list for the conflicting mods names (for the ui prompt)
            List<string> conflictingModFiles = new List<string>(); //temp list for the conflicting mods files (for the ui prompt)

            //loop through the mod list
            foreach (Mod mod in mods)
            {
                //create a boolean that states whether or not the given mod has conflicts
                bool hasConflictingFiles = false;

                //loop through the mod files for the current mod in the loop
                foreach(string file in mod.ModFiles)
                {
                    //loop through each 'newModFile' and check each one with the given mod 'file'
                    foreach(string newModFile in newMod.ModFiles)
                    {
                        if(file.Equals(newModFile))
                        {
                            hasConflictingFiles = true;
                            conflictingModFiles.Add(file);
                        }
                    }
                }

                //if there are conflicting files, add the current mod and its name to the list
                if(hasConflictingFiles)
                {
                    conflictingMods.Add(mod);
                    conflictingModNames.Add(mod.ModDisplayName);
                }
            }

            //if there are conflicting mods
            if(conflictingMods.Count != 0)
            {
                //for prompt ui
                string conflictingModsMessage = string.Join(", ", conflictingModNames);
                string conflictingModFilesMessage = string.Join(", ", conflictingModFiles);
                string finalMessage = string.Format("{0} has conflicts with the following mods! {1}. They share the following files! {2}. Do you want to remove the conflicting mods?", newMod.ModDisplayName, conflictingModsMessage, conflictingModFilesMessage);

                //notify the user about the conflicts, if they take action (press yes) it will remove the conflicting mods from the list, and removes the respective files and .json files
                if (MessageBoxes.Error_Confirm(finalMessage, "Mod Conflicts!"))
                {
                    foreach(Mod mod in conflictingMods)
                    {
                        RemoveModFiles(mod);
                        IOManagement.DeleteFile(mod.ModInfoJson_FilePath);
                        mods.Remove(mod);
                    }
                }

                return;
            }

            //if everything passes, add the mod (if the boolean on the function call has been set)
            if(addMod)
            {
                ExtractModZipFileContents_ToDirectory(newMod);
                mods.Add(newMod);
            }
        }

        /// <summary>
        /// Adds a mod with a bunch of validation checks.
        /// <para>If a validation check fails, it will not add the mod.</para>
        /// </summary>
        /// <param name="newMod"></param>
        public void ValidateAndAddMod(Mod newMod)
        {
            //if there are no mods in the list, go ahead and install it
            if (mods.Count < 1)
            {
                mods.Add(newMod);
                ExtractModZipFileContents_ToDirectory(newMod);

                return; //dont continue
            }

            //if there are mods, then loop through each one to check their properties
            foreach (Mod mod in mods)
            {
                //if mod shares the name
                if (mod.ModDisplayName.Equals(newMod.ModDisplayName))
                {
                    if (mod.ModVersion.Equals(newMod.ModVersion)) //if the mod is the same version and has the same name, this mod is a duplicate! stop!
                    {
                        string message = string.Format("You already have '{0}' version '{1}' installed!. Do you wish to replace it anyway?", mod.ModDisplayName, mod.ModVersion, newMod.ModVersion);

                        if (MessageBoxes.Info_Confirm(message, "Replace Mod"))
                        {
                            ReplaceMod(mod, newMod);
                        }

                        return;
                    }
                    else //if the mod has the same name but is a different version, then prompt the user if they want to replace it with the given version.
                    {
                        string message = string.Format("You have '{0}' installed with version '{1}'. Do you wish to replace it with this version {2}?", mod.ModDisplayName, mod.ModVersion, newMod.ModVersion);

                        if (MessageBoxes.Info_Confirm(message, "Replace Mod"))
                        {
                            ReplaceMod(mod, newMod);
                        }

                        return;
                    }
                }

                //now check the mod's compatiblity
                GameVersion parsedModCompatiblity;

                try
                {
                    //attempt to parse the mods game version string as a game version enum
                    parsedModCompatiblity = GameVersion_Functions.Get_Versions_ParseStringValue(newMod.ModCompatibility);
                }
                catch(Exception e)
                {
                    //the parsing failed, show the exception
                    MessageBoxes.Error(e.Message, e.ToString());
                    return; //dont continue
                }

                //if the parse is sucessful, now check if it matches the current game version we have selected
                if(parsedModCompatiblity.Equals(appSettings.Get_Current_GameVersionName()) == false)
                {
                    //for ui
                    string current_gameVersionString = Enum.GetName(typeof(GameVersion), appSettings.Get_Current_GameVersionName());
                    string mod_gameVersionString = Enum.GetName(typeof(GameVersion), parsedModCompatiblity);
                    string message = string.Format("'{0}' is incompatible with your current game version '{1}'. The mod was built for the following game version '{2}'.", mod.ModDisplayName, current_gameVersionString, mod_gameVersionString);

                    //notify the user
                    //NOTE: Future addition - add an option for the user to convert the mod version, but warn them that this may not work.
                    MessageBoxes.Error(message, "Incompatible Game Version!");

                    return;//dont continue
                }
            }

            //if all checks pass, do a final check to check the contents of the mod with the existing ones
            CheckModContents_WithExistingMods(newMod, true);
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
                            IOManagement.DeleteFile(modJson_extractedFromArchive);

                        // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that are case-insensitive.
                        if (modJson_extractedFromArchive.StartsWith(modZipFile_ParentDirectory, StringComparison.Ordinal))
                            entry.ExtractToFile(modJson_extractedFromArchive);
                    }
                }
            }

            //after extraction, read the json file
            Json_ReadModFile(modJson_extractedFromArchive);
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
            IOManagement.DeleteFile(orignalMod.ModInfoJson_FilePath);
            mods.Remove(orignalMod);

            ExtractModZipFileContents_ToDirectory(newMod);
            mods.Add(newMod);
        }

        /// <summary>
        /// Reads the data from the modinfo.json file and returns a mod object.
        /// </summary>
        /// <param name="modJsonFile"></param>
        /// <returns></returns>
        public Mod GetJsonData(string modJsonFile)
        {
            //create a new mod object
            Mod newMod = new Mod();

            //check if the json does not exist
            if(!File.Exists(modJsonFile))
            {
                string message = "This mod zip file is not setup like a mod launcher file! Please refer to the help section to see how a mod zip file should be setup.";

                MessageBoxes.Error(message, "Can't Add Mod!");

                return null;
            }

            //read the data from the config file
            string jsonText = File.ReadAllText(modJsonFile);

            //parse the data into a json array
            JObject obj = JObject.Parse(jsonText);

            //loop through each property to get the data
            foreach (JProperty property in obj.Properties())
            {
                //get the name of the property from the json object
                string name = property.Name;

                if (name.Equals(nameof(Mod.ModAuthor)))
                    newMod.ModAuthor = (string)property.Value;

                if (name.Equals(nameof(Mod.ModCompatibility)))
                    newMod.ModCompatibility = (string)property.Value;

                if (name.Equals(nameof(Mod.ModDisplayName)))
                    newMod.ModDisplayName = (string)property.Value;

                if (name.Equals(nameof(Mod.ModVersion)))
                    newMod.ModVersion = (string)property.Value;

                //if the property is a mod files array, parse the given files to a list
                if (name.Equals(nameof(Mod.ModFiles)))
                {
                    JArray fileArray = (JArray)obj[nameof(Mod.ModFiles)];
                    List<string> parsedModFiles = new List<string>();

                    foreach (JValue file in fileArray)
                    {
                        parsedModFiles.Add((string)file.Value);
                    }

                    newMod.ModFiles = parsedModFiles;
                }

                //if the property is a mod files array, parse the given files to a list
                if (name.Equals(nameof(Mod.ModFilesGameDirectory)))
                {
                    JArray fileArray = (JArray)obj[nameof(Mod.ModFilesGameDirectory)];
                    List<string> parsedModFilesGameDirectory = new List<string>();

                    foreach (JValue file in fileArray)
                    {
                        parsedModFilesGameDirectory.Add((string)file.Value);
                    }

                    newMod.ModFiles = parsedModFilesGameDirectory;
                }
            }

            newMod.ModInfoJson_FilePath = modJsonFile;

            return newMod;
        }

        /// <summary>
        /// Reads a modinfo.json file and parses the data into a new mod object.
        /// <para>After sucessfully getting the data, it will execute ValidateAndAddMod()</para>
        /// </summary>
        /// <param name="modJsonFile"></param>
        public void Json_ReadModFile(string modJsonFile, bool bypassValidation = false, bool deleteJson = true)
        {
            //create a new mod object
            Mod newMod = GetJsonData(modJsonFile);

            //if the new mod is null, do not continue!
            if(newMod == null)
                return;

            if (deleteJson)
            {
                //delete the modJsonFile since we don't need it anymore
                IOManagement.DeleteFile(modJsonFile);
            }

            //bypass the validation stage and just add the mod anyway
            //(for when this function is called to gather mods from the mod folder on startup)
            if (bypassValidation)
            {
                mods.Add(newMod);

                if(CheckIfModFilesExist(newMod) == false)
                    ExtractModZipFileContents_ToDirectory(newMod);
            }
            else
            {
                //otherwise, validate the mod. if its sucessful it will be added
                ValidateAndAddMod(newMod);
            }
        }

        /// <summary>
        /// Reads a modinfo.json file and parses the data into a new mod object.
        /// <para>After sucessfully getting the data, it will execute ValidateAndAddMod()</para>
        /// </summary>
        /// <param name="modJsonFile"></param>
        public void Json_ReadModFile_New(string modJsonFile, bool bypassValidation = false, bool deleteJson = true)
        {
            //create a new mod object
            Mod newMod = GetJsonData(modJsonFile);

            //if the new mod is null, do not continue!
            if (newMod == null)
                return;

            newMod.ModInfoJson_FilePath = modJsonFile;

            if (deleteJson)
            {
                //delete the modJsonFile since we don't need it anymore
                IOManagement.DeleteFile(modJsonFile);
            }

            //bypass the validation stage and just add the mod anyway
            //(for when this function is called to gather mods from the mod folder on startup)
            if (bypassValidation)
            {
                mods.Add(newMod);

                if (CheckIfModFilesExist(newMod) == false)
                    ExtractModZipFileContents_ToDirectory(newMod);
            }
            else
            {
                //otherwise, validate the mod. if its sucessful it will be added
                ValidateAndAddMod(newMod);
            }
        }
    }
}
