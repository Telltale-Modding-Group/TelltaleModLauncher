using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TelltaleModLauncher.Files
{
    public class GameVersionSettings
    {
        //the version of said game
        public GameVersion Game_Version { get; set; }

        //the directory of the game
        public string Game_Location { get; set; } = "Undefined";

        //the location of the game executable
        public string Game_LocationExe { get; set; } = "Undefined";

        //the ttarch enum value for the game
        public string Game_LibTelltale_GameID
        {
            get
            {
                return GameVersion_Functions.Get_GameID_FromVersion(Game_Version);
            }
        }

        //the location of the game mods folder (where the archives of the game are, either in /Archives or /Packs)
        public string Game_Location_Mods 
        {
            get
            {
                if (Directory.Exists(Game_Location + "/Archives/"))
                    return Game_Location + "/Archives/";
                else if (Directory.Exists(Game_Location + "/Packs/"))
                    return Game_Location + "/Packs/";
                else if (Directory.Exists(Game_Location + "/Pack/"))
                    return Game_Location + "/Pack/";
                else
                    return "";
            }
        }
    }
}
