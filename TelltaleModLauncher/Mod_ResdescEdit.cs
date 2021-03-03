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

        //thread management
        private bool taskFinished;

        public Mod_ResdescEdit(AppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        void EncryptFile_OnProcessExit(object sender, EventArgs e)
        {
            taskFinished = true;
        }

        private Process encryptProcessObj;
        private ProcessStartInfo encryptProcessStartInfoObj;

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
            //set.priority = 
            //set.gameDataPriority = 

            List<string> file_lines = new List<string>(File.ReadAllLines(file));

            foreach(string line in file_lines)
            {

            }
        }
    }
}
