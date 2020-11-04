using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace TelltaleModLauncher
{
    class AppSettings
    {
        private static string systemDocumentsPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private static string configFile_filename = "TelltaleModLauncher_Config.json";
        private static string configFile_directory_location = systemDocumentsPath + "/TelltaleModLauncher/";
        private static string configFile_file_location = configFile_directory_location + configFile_filename;

        public AppSettingsFile appSettingsFile = new AppSettingsFile();
        public List<GameVersionSettings> GameVersionSettings { get; set; }

        private IOManagement ioManagement = new IOManagement();

        public void WriteNewFile()
        {
            //open a stream writer to create the text file and write to it
            using (StreamWriter file = File.AppendText(configFile_file_location))
            {
                //get our json seralizer
                JsonSerializer serializer = new JsonSerializer();

                List<object> jsonObjects = new List<object>();
                jsonObjects.Add(appSettingsFile);
                jsonObjects.Add(GameVersionSettings);

                //seralize the data and write it to the configruation file
                serializer.Formatting = Formatting.Indented;
                serializer.Serialize(file, jsonObjects);
            }
        }

        public void UpdateChangesToFile()
        {
            ioManagement.DeleteFile(configFile_file_location);
            WriteNewFile();
        }
    }
}
