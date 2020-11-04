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
        public string Game_Location { get; set; }
        public string Game_LocationExe { get; set; }
        public string Game_Location_Mods { get; set; }

        public int Game_Ttarch_GameEnumNumber
        {
            get
            {
                switch (Game_Version)
                {
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
