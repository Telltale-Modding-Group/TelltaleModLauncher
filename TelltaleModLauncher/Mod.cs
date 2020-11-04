using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelltaleModLauncher
{
    public class Mod
    {
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
