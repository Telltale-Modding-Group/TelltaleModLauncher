using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelltaleModLauncher
{
    class Mod
    {
        public string ModDisplayName { get; set; }
        public string ModVersion { get; set; }
        public string ModAuthor { get; set; }
        public string ModCompatability { get; set; }
        public List<string> ModFiles { get; set; }

        public Mod (string ModDisplayName, string ModVersion, string ModAuthor)
        {
            this.ModAuthor = ModAuthor;
            this.ModDisplayName = ModDisplayName;
            this.ModVersion = ModVersion;
        }

        public void SetModFiles(List<string> ModFiles)
        {
            this.ModFiles = ModFiles;
        }
    }
}
