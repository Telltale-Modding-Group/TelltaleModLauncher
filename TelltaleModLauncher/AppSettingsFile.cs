using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelltaleModLauncher
{
    public class AppSettingsFile
    {
        public string Default_Game_Version { get; set; } = "Game Version";
        public string Location_Ttarchext { get; set; } = "ttarchext Location";
        public string Location_LuaCompiler { get; set; } = "Lua Compiler Location";
        public string Location_LuaDecompiler { get; set; } = "Lua Decompiler Location";
        public bool UI_LightMode { get; set; } = false;
    }
}
