using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelltaleModLauncher.Files
{
    public class AppSettingsFile
    {
        public GameVersion Default_Game_Version { get; set; }
        public bool UI_LightMode { get; set; } = false;
    }
}
