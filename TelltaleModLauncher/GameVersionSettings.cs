using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelltaleModLauncher
{
    class GameVersionSettings
    {
        private int userSet_ttarchGameEnumNumber;

        public string Game_Version { get; set; }
        public string Game_Location { get; set; } = "Game Location";
        public string Game_LocationExe { get; set; } = "Game Exe Location";
        public string Game_Location_Mods { get; set; } = "Game Mods Location";

        public int Game_Ttarch_GameEnumNumber
        {
            get
            {
                switch (Game_Version)
                {
                    //NOTE: These strings need to match the existing GameVersion enums
                    case "The_Walking_Dead_Definitive_Edition":
                        return 67;
                    default:
                        return userSet_ttarchGameEnumNumber;
                }
            }
            set
            {
                userSet_ttarchGameEnumNumber = value;
            }
        }
    }
}
