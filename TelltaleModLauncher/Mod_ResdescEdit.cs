using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Windows.Forms;

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
        /// Encrpyts a given file using ttarchexe
        /// </summary>
        /// <param name="game_archiveLocation"></param>
        /// <param name="extractionDumpLocation"></param>
        private void EncryptFile(string fileLocation)
        {
            //initalize our process and process information
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();

            //setup some values for the process
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.FileName = "cmd.exe";
            processStartInfo.WorkingDirectory = appSettings.Get_Current_GameVersionSettings_GameDirectory();

            //for reading the console output
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;

            //call our process exit so we can know later if it's done.
            process.EnableRaisingEvents = true;
            process.Exited += new EventHandler(EncryptFile_OnProcessExit);
            //process.OutputDataReceived += new DataReceivedEventHandler(Output_OnOutputDataReceived);

            //command line useage (IMPORTANT)
            string fileLocationDirectory = Path.GetDirectoryName(fileLocation);
            string cmd_arguments1 = String.Format("/c {0} {1} {2} {3}", appSettings.appSettingsFile.Location_Ttarchext, appSettings.Get_Current_GameVersionSettings_ttarchNumber().ToString(), fileLocation, fileLocationDirectory);

            //assign the arguments
            processStartInfo.Arguments = cmd_arguments1;

            //begin the process
            process.StartInfo = processStartInfo;
            process.Start();

            //start read console output
            process.BeginOutputReadLine();

            encryptProcessStartInfoObj = processStartInfo;
            encryptProcessObj = process;

            process.OutputDataReceived += Process_OutputDataReceived;
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string cmd_arguments2 = String.Format("y");

            encryptProcessStartInfoObj.Arguments = cmd_arguments2;

            //begin the process again
            encryptProcessObj.StartInfo = encryptProcessStartInfoObj;
            encryptProcessObj.Start();
        }

        void DecryptFile_OnProcessExit(object sender, EventArgs e)
        {
            taskFinished = true;
        }

        /// <summary>
        /// Extracts a given archive and dump's its contents into the given extraction dump location
        /// </summary>
        /// <param name="game_archiveLocation"></param>
        /// <param name="extractionDumpLocation"></param>
        private void DecryptFile(string fileLocation)
        {
            //initalize our process and process information
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();

            //setup some values for the process
            processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            processStartInfo.CreateNoWindow = true;
            processStartInfo.FileName = "cmd.exe";
            processStartInfo.WorkingDirectory = appSettings.Get_Current_GameVersionSettings_GameDirectory();

            //for reading the console output
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.UseShellExecute = false;

            //call our process exit so we can know later if it's done.
            process.EnableRaisingEvents = true;
            process.Exited += new EventHandler(DecryptFile_OnProcessExit);
            //process.OutputDataReceived += new DataReceivedEventHandler(Output_OnOutputDataReceived);

            //command line useage (IMPORTANT)
            string fileLocationDirectory = Path.GetDirectoryName(fileLocation);
            string cmd_arguments = String.Format("/c {0} -V 7 -e 0 {1} {2} {3}", appSettings.appSettingsFile.Location_Ttarchext, appSettings.Get_Current_GameVersionSettings_ttarchNumber().ToString(), fileLocation, fileLocationDirectory);

            //assign the arguments
            processStartInfo.Arguments = cmd_arguments;

            //begin the process
            process.StartInfo = processStartInfo;
            process.Start();

            //start read console output
            process.BeginOutputReadLine();
        }
    }
}
