﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelltaleModLauncher
{
    class AppSettings
    {
        public string Default_Game_Version { get; set; }
        public string Location_Ttarchext { get; set; }
        public string Location_LuaCompiler { get; set; }
        public string Location_LuaDecompiler { get; set; }
        public List<GameVersionSettings> GameVersionSettings { get; set; }

        public void WriteNewFile()
        {

        }

        public void UpdateChangesToFile()
        {

        }
    }
}
