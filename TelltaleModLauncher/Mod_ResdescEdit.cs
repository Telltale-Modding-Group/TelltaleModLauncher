using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Windows.Forms;
using LibTelltale;

namespace TelltaleModLauncher
{
    public class Mod_ResdescEdit
    {
        private AppSettings appSettings;

        public Mod_ResdescEdit(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        /// <summary>
        /// Encrypts a given file using libtelltale
        /// </summary>
        /// <param name="game_archiveLocation"></param>
        /// <param name="extractionDumpLocation"></param>
        private void EncryptFile(string fileLocation)
        {
            //read the file into a byte array
            byte[] file_bytes = File.ReadAllBytes(fileLocation);

            //check if the file has a lenc extension
            bool isLenc = Path.GetExtension(fileLocation).Equals(".lenc");

            //get the game ID to encrypt the file with the proper encryption key that matches the current selected game
            string gameID = appSettings.current_GameVersionSettings.Game_LibTelltale_GameID;

            //use libtelltale and encrypt the file
            MemoryHelper.Bytes bytes = Config.EncryptResourceDescription(file_bytes, gameID, false, isLenc);

            //overwrite the original resource description file with the encrypted bytes
            File.WriteAllBytes(fileLocation, bytes.bytes);

            //free the memory since we have no use for it
            MemoryHelper.FreeReadBytes(bytes);
        }

        /// <summary>
        /// Decrypts a given file using libtelltale
        /// </summary>
        /// <param name="game_archiveLocation"></param>
        /// <param name="extractionDumpLocation"></param>
        private void DecryptFile(string fileLocation)
        {
            //read the file into a byte array
            byte[] file_bytes = File.ReadAllBytes(fileLocation);

            //get the game ID to decrypt the file with the proper encryption key that matches the current selected game
            string gameID = appSettings.current_GameVersionSettings.Game_LibTelltale_GameID;

            //use libtelltale and decrypt the file
            MemoryHelper.Bytes bytes = Config.DecryptResourceDescription(file_bytes, gameID, false);

            //overwrite the original resource description file with the decrypted bytes
            File.WriteAllBytes(fileLocation, bytes.bytes);

            //free the memory since we have no use for it
            MemoryHelper.FreeReadBytes(bytes);
        }

        public void Parse_File(string file)
        {
            List<string> file_lines = new List<string>(File.ReadAllLines(file));

            string archivePriority_name = "set.priority";
            string archiveGamePriority_name = "set.gameDataPriority";

            int archivePriority_value = 0;
            int archiveGamePriority_value = 0;

            foreach(string line in file_lines)
            {
                string archivePriority_file = line.Remove(archivePriority_name.Length - 1, Math.Abs(line.Length - archivePriority_name.Length));
                string archiveGamePriority_file = line.Remove(archiveGamePriority_name.Length - 1, Math.Abs(line.Length - archiveGamePriority_name.Length));

                if (archivePriority_file.Equals(archivePriority_name))
                {
                    string a = line.Remove(0, archivePriority_file.Length);
                    string noSpaces = a.Replace(" ", "");
                    string noEquals = noSpaces.Replace("=", "");

                    archivePriority_value = int.Parse(noEquals);
                }

                if (archiveGamePriority_file.Equals(archivePriority_name))
                {
                    string a = line.Remove(0, archiveGamePriority_file.Length);
                    string noSpaces = a.Replace(" ", "");
                    string noEquals = noSpaces.Replace("=", "");

                    archiveGamePriority_value = int.Parse(noEquals);
                }
            }
        }

        public void Edit_File(string file, int archivePriority_value, int archiveGamePriority_value)
        {
            List<string> file_lines = new List<string>(File.ReadAllLines(file));

            string archivePriority_name = "set.priority";
            string archiveGamePriority_name = "set.gameDataPriority";

            int index = 0;

            foreach (string line in file_lines)
            {
                string archivePriority_file = line.Remove(archivePriority_name.Length - 1, Math.Abs(line.Length - archivePriority_name.Length));
                string archiveGamePriority_file = line.Remove(archiveGamePriority_name.Length - 1, Math.Abs(line.Length - archiveGamePriority_name.Length));

                if (archivePriority_file.Equals(archivePriority_name))
                {
                    string a = line.Remove(0, archivePriority_file.Length);
                    string noSpaces = a.Replace(" ", "");
                    string noEquals = noSpaces.Replace("=", "");

                    file_lines[index] = string.Format("{0} = {1}", archivePriority_name, archivePriority_value.ToString());
                }

                if (archiveGamePriority_file.Equals(archivePriority_name))
                {
                    string a = line.Remove(0, archiveGamePriority_file.Length);
                    string noSpaces = a.Replace(" ", "");
                    string noEquals = noSpaces.Replace("=", "");

                    file_lines[index] = string.Format("{0} = {1}", archiveGamePriority_name, archiveGamePriority_value.ToString());
                }

                index++;
            }

            File.WriteAllLines(file, file_lines);
        }
    }
}
