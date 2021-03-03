using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelltaleModLauncher.Files
{
    public class Mod
    {
        /// <summary>
        /// Mod Json Format Version
        /// </summary>
        public string ModJsonFormatVersion { get; } = "v1.0";

        /// <summary>
        /// The display name of the mod.
        /// </summary>
        public string ModDisplayName { get; set; }

        /// <summary>
        /// The version number string of the mod.
        /// </summary>
        public string ModVersion { get; set; }

        /// <summary>
        /// The author of the mod.
        /// </summary>
        public string ModAuthor { get; set; }

        /// <summary>
        /// The game version the mod is compatible with.
        /// </summary>
        public string ModCompatibility { get; set; }

        /// <summary>
        /// The list of filename.extension's assoicated with the mod.
        /// </summary>
        public List<string> ModFiles { get; set; }

        /// <summary>
        /// The list of filenames.extension's that are associated with the mod to be placed in the game directory.
        /// </summary>
        public List<string> ModFilesGameDirectory { get; set; }

        /// <summary>
        /// The file path of the mod info json file.
        /// </summary>
        public string ModInfoJson_FilePath { get; set; }

        /// <summary>
        /// Sets the resource priority for the mod archives
        /// </summary>
        public int ModResourcePriority { get; set; }

        /// <summary>
        /// Creates a blank mod object, must be filled
        /// </summary>
        public Mod()
        {

        }

        /// <summary>
        /// Creates a mod object with the given data to reference the actual mod data on the disk.
        /// </summary>
        /// <param name="ModDisplayName"></param>
        /// <param name="ModVersion"></param>
        /// <param name="ModAuthor"></param>
        /// <param name="ModFiles"></param>
        public Mod (string ModDisplayName, string ModVersion, string ModAuthor, List<string> ModFiles, string ModCompatibility)
        {
            this.ModAuthor = ModAuthor;
            this.ModDisplayName = ModDisplayName;
            this.ModVersion = ModVersion;
            this.ModFiles = ModFiles;
            this.ModCompatibility = ModCompatibility;
        }
    }
}
