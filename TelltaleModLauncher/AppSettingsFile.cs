using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelltaleModLauncher
{
    public class AppSettingsFile
    {
        public GameVersion Default_Game_Version { get; set; }
        public string Location_Ttarchext { get; set; } = "Undefined";
        public string Location_LuaCompiler { get; set; } = "Undefined";
        public string Location_LuaDecompiler { get; set; } = "Undefined";
        public bool UI_LightMode { get; set; } = false;
    }
}
