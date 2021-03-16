using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelltaleModLauncher
{
    /// <summary>
    /// The supported Game Versions.
    /// <para>Add supported game versions in here</para>
    /// <para>NOTE: this is only a name identifier, it needs to be supported by the code too!</para>
    /// </summary>
    public enum GameVersion
    {
        The_Walking_Dead_Definitive_Series = 0,
        The_Walking_Dead_Season_Four = 1,
        The_Walking_Dead_Season_Three = 2,
        The_Walking_Dead_Michonne = 3,
        The_Walking_Dead_Season_Two = 4,
        The_Walking_Dead_Season_One = 5,
        Game_Of_Thrones = 6,
        The_Wolf_Among_Us = 7,
        Marvels_Gaurdians_Of_The_Galaxy = 8,
        Batman_The_Telltale_Series = 9,
        Batman_The_Enemy_Within = 10,
        Tales_from_The_Borderlands = 11,
        Telltale_Texas_Holdem = 12,
        Bone_Out_from_Boneville = 13,
        Bone_The_Great_Cow_Race = 14,
        CSI_3_Dimensions_of_Murder = 15,
        CSI_Deadly_Intent = 16,
        CSI_Fatal_Conspiracy = 17,
        CSI_Hard_Evidence = 18,
        Jurassic_Park_The_Game = 19,
        Law_and_Order_Legacies = 20,
        Minecraft_Story_Mode_Season_One = 21,
        Minecraft_Story_Mode_Season_Two = 22,
        Poker_Night_at_The_Inventory = 23,
        Poker_Night_2 = 24,
        Sam_and_Max_Season_One_101 = 25,
        Sam_and_Max_Season_One_102 = 26,
        Sam_and_Max_Season_One_103 = 27,
        Sam_and_Max_Season_One_104 = 28,
        Sam_and_Max_Season_One_105 = 29,
        Sam_and_Max_Season_One_106 = 30,
        Sam_and_Max_Season_Two_201 = 31,
        Sam_and_Max_Season_Two_202 = 32,
        Sam_and_Max_Season_Two_203 = 33,
        Sam_and_Max_Season_Two_204 = 34,
        Sam_and_Max_Season_Two_205 = 35,
        Sam_and_Max_Season_Three_301 = 36,
        Sam_and_Max_Season_Three_302 = 37,
        Sam_and_Max_Season_Three_303 = 38,
        Sam_and_Max_Season_Three_304 = 39,
        Sam_and_Max_Season_Three_305 = 40,
        Sam_and_Max_Save_The_World_Remastered = 41,
        Hector_Badge_Of_Carnage_101 = 42,
        Hector_Badge_Of_Carnage_102 = 43,
        Hector_Badge_Of_Carnage_103 = 44,
        Puzzle_Agent_101 = 45,
        Puzzle_Agent_102 = 46,
        Back_To_The_Future_101 = 47,
        Back_To_The_Future_102 = 48,
        Back_To_The_Future_103 = 49,
        Back_To_The_Future_104 = 50,
        Back_To_The_Future_105 = 51,
        Strong_Bad_Cool_Game_for_Attractive_People_101 = 52,
        Strong_Bad_Cool_Game_for_Attractive_People_102 = 53,
        Strong_Bad_Cool_Game_for_Attractive_People_103 = 54,
        Strong_Bad_Cool_Game_for_Attractive_People_104 = 55,
        Strong_Bad_Cool_Game_for_Attractive_People_105 = 56,
        Tales_of_Monkey_Island_101 = 57,
        Tales_of_Monkey_Island_102 = 58,
        Tales_of_Monkey_Island_103 = 59,
        Tales_of_Monkey_Island_104 = 60,
        Tales_of_Monkey_Island_105 = 61,
        Wallace_And_Gromits_Grand_Adventures_101 = 62,
        Wallace_And_Gromits_Grand_Adventures_102 = 63,
        Wallace_And_Gromits_Grand_Adventures_103 = 64,
        Wallace_And_Gromits_Grand_Adventures_104 = 65,
        Other = 66
    };

    public class GameVersion_Functions
    {
        public static List<string> Get_Versions_StringList(bool replaceUnderscoreWithSpaces = false)
        {
            if (replaceUnderscoreWithSpaces)
            {
                List<string> enumNames = new List<string>(Enum.GetNames(typeof(GameVersion)));
                List<string> changed_enumNames = new List<string>();

                foreach (string enumName in enumNames)
                {
                    changed_enumNames.Add(enumName.Replace("_", " "));
                }

                return changed_enumNames;
            }
            else
                return new List<string>(Enum.GetNames(typeof(GameVersion)));
        }

        public static GameVersion Get_Versions_ParseStringValue(string value)
        {
            return (GameVersion)Enum.Parse(typeof(GameVersion), value);
        }

        public static string Get_Versions_StringValue(GameVersion value)
        {
            return Enum.GetName(typeof(GameVersion), value);
        }

        public static int Get_Versions_IntValue(GameVersion value)
        {
            return (int)value;
        }

        public static GameVersion Get_Versions_ParseIntValue(int value)
        {
            return (GameVersion)value;
        }

        public static string Get_GameID_FromVersion(GameVersion version)
        {
            switch (version)
            {
                case GameVersion.Back_To_The_Future_101:
                    return "bttf101";
                case GameVersion.Back_To_The_Future_102:
                    return "bttf102";
                case GameVersion.Back_To_The_Future_103:
                    return "bttf103";
                case GameVersion.Back_To_The_Future_104:
                    return "bttf104";
                case GameVersion.Back_To_The_Future_105:
                    return "bttf105";
                case GameVersion.Batman_The_Telltale_Series:
                    return "batman";
                case GameVersion.Batman_The_Enemy_Within:
                    return "batman2";
                case GameVersion.Bone_Out_from_Boneville:
                    return "boneville";
                case GameVersion.Bone_The_Great_Cow_Race:
                    return "cowrace";
                case GameVersion.CSI_3_Dimensions_of_Murder:
                    return "csi3dimensions";
                case GameVersion.CSI_Deadly_Intent:
                    return "csideadly";
                case GameVersion.CSI_Fatal_Conspiracy:
                    return "csifatal";
                case GameVersion.CSI_Hard_Evidence:
                    return "csihard";
                case GameVersion.Game_Of_Thrones:
                    return "thrones";
                case GameVersion.Hector_Badge_Of_Carnage_101:
                    return "hector101";
                case GameVersion.Hector_Badge_Of_Carnage_102:
                    return "hector102";
                case GameVersion.Hector_Badge_Of_Carnage_103:
                    return "hector103";
                case GameVersion.Jurassic_Park_The_Game:
                    return "jurrassicpark";
                case GameVersion.Law_and_Order_Legacies:
                    return "lawandorder";
                case GameVersion.Marvels_Gaurdians_Of_The_Galaxy:
                    return "marvel";
                case GameVersion.Minecraft_Story_Mode_Season_One:
                    return "mcsm";
                case GameVersion.Minecraft_Story_Mode_Season_Two:
                    return "mc2";
                case GameVersion.Poker_Night_at_The_Inventory:
                    return "celebritypoker";
                case GameVersion.Poker_Night_2:
                    return "celebritypoker2";
                case GameVersion.Puzzle_Agent_101:
                    return "grickle101";
                case GameVersion.Puzzle_Agent_102:
                    return "grickle102";
                case GameVersion.Sam_and_Max_Save_The_World_Remastered:
                    return "sammaxremaster";
                case GameVersion.Sam_and_Max_Season_One_101:
                    return "sammax101";
                case GameVersion.Sam_and_Max_Season_One_102:
                    return "sammax102";
                case GameVersion.Sam_and_Max_Season_One_103:
                    return "sammax103";
                case GameVersion.Sam_and_Max_Season_One_104:
                    return "sammax104";
                case GameVersion.Sam_and_Max_Season_One_105:
                    return "sammax105";
                case GameVersion.Sam_and_Max_Season_One_106:
                    return "sammax106";
                case GameVersion.Sam_and_Max_Season_Three_301:
                    return "sammax301";
                case GameVersion.Sam_and_Max_Season_Three_302:
                    return "sammax302";
                case GameVersion.Sam_and_Max_Season_Three_303:
                    return "sammax303";
                case GameVersion.Sam_and_Max_Season_Three_304:
                    return "sammax304";
                case GameVersion.Sam_and_Max_Season_Three_305:
                    return "sammax305";
                case GameVersion.Sam_and_Max_Season_Two_201:
                    return "sammax201";
                case GameVersion.Sam_and_Max_Season_Two_202:
                    return "sammax202";
                case GameVersion.Sam_and_Max_Season_Two_203:
                    return "sammax203";
                case GameVersion.Sam_and_Max_Season_Two_204:
                    return "sammax204";
                case GameVersion.Sam_and_Max_Season_Two_205:
                    return "sammax205";
                case GameVersion.Strong_Bad_Cool_Game_for_Attractive_People_101:
                    return "sbcg4ap101";
                case GameVersion.Strong_Bad_Cool_Game_for_Attractive_People_102:
                    return "sbcg4ap102";
                case GameVersion.Strong_Bad_Cool_Game_for_Attractive_People_103:
                    return "sbcg4ap103";
                case GameVersion.Strong_Bad_Cool_Game_for_Attractive_People_104:
                    return "sbcg4ap104";
                case GameVersion.Strong_Bad_Cool_Game_for_Attractive_People_105:
                    return "sbcg4ap105";
                case GameVersion.Tales_from_The_Borderlands:
                    return "borderlands";
                case GameVersion.Tales_of_Monkey_Island_101:
                    return "monkeyisland101";
                case GameVersion.Tales_of_Monkey_Island_102:
                    return "monkeyisland102";
                case GameVersion.Tales_of_Monkey_Island_103:
                    return "monkeyisland103";
                case GameVersion.Tales_of_Monkey_Island_104:
                    return "monkeyisland104";
                case GameVersion.Tales_of_Monkey_Island_105:
                    return "monkeyisland105";
                case GameVersion.Telltale_Texas_Holdem:
                    return "texasholdem";
                case GameVersion.The_Walking_Dead_Season_One:
                    return "twd1";
                case GameVersion.The_Walking_Dead_Definitive_Series:
                    return "wdc";
                case GameVersion.The_Walking_Dead_Michonne:
                    return "michonne";
                case GameVersion.The_Walking_Dead_Season_Four:
                    return "wd4";
                case GameVersion.The_Walking_Dead_Season_Three:
                    return "wd3";
                case GameVersion.The_Walking_Dead_Season_Two:
                    return "wd2";
                case GameVersion.The_Wolf_Among_Us:
                    return "fables";
                case GameVersion.Wallace_And_Gromits_Grand_Adventures_101:
                    return "wag101";
                case GameVersion.Wallace_And_Gromits_Grand_Adventures_102:
                    return "wag102";
                case GameVersion.Wallace_And_Gromits_Grand_Adventures_103:
                    return "wag103";
                case GameVersion.Wallace_And_Gromits_Grand_Adventures_104:
                    return "wag104";
                default:
                    return "";
            }
        }
    }
}
