using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelltaleModLauncher
{
    public class GameVersionSettings
    {
        private int userSet_ttarchGameEnumNumber;

        public GameVersion Game_Version { get; set; }
        public string Game_Location { get; set; } = "Undefined";
        public string Game_LocationExe { get; set; } = "Undefined";
        public string Game_Location_Mods { get; set; } = "Undefined";

        public int Game_Ttarch_GameEnumNumber
        {
            get
            {
                switch (Game_Version)
                {
                    //NOTE: These strings need to match the existing GameVersion enums
                    case GameVersion.The_Walking_Dead_Definitive_Edition:
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
