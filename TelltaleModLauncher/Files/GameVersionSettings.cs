using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelltaleModLauncher.Files
{
    public class GameVersionSettings
    {
        private string Game_LibTelltale_GameID_value;

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
                switch (Game_Version)
                {
                    case GameVersion.The_Walking_Dead_Definitive_Edition:
                        return "wdc";
                    case GameVersion.The_Walking_Dead_Final_Season:
                        return "wd4";
                    default:
                        return "";
                }
            }
            set
            {
                Game_LibTelltale_GameID_value = value;
            }
        }

        //the location of the game mods folder (where the archives of the game are, either in /Archives or /Packs)
        public string Game_Location_Mods 
        {
            get
            {
                switch (Game_Version)
                {
                    case GameVersion.The_Walking_Dead_Definitive_Edition:
                        return Game_Location + "/Archives/";
                    case GameVersion.The_Walking_Dead_Final_Season:
                        return Game_Location + "/Archives/";
                    default:
                        return "Undefined";
                }
            }
        }
    }
}
