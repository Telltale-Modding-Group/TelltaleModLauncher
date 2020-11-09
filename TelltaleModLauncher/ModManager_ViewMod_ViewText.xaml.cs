using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace TelltaleModLauncher
{
    /// <summary>
    /// Interaction logic for ModManager_ViewMod_ViewText.xaml
    /// </summary>
    public partial class ModManager_ViewMod_ViewText
    {
        public string modDisplayName;
        public string filePath;
        public string fileContents;

        public ModManager_ViewMod_ViewText()
        {
            InitializeComponent();

            fileContents = "Empty";
        }

        public void OpenWindow(string filePath, string modDisplayName)
        {
            this.filePath = filePath;
            this.modDisplayName = modDisplayName;
            this.Show();
            this.Activate();

            LoadInContents();

            UpdateUI();
        }

        public bool CheckPreviewValidity(string newFilePath)
        {
            if (File.Exists(newFilePath) == false)
            {
                string message = string.Format("'{0}' does not exist!", newFilePath);

                MessageBox.Show(message, "Can't Read File", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }

            string fileExt = System.IO.Path.GetExtension(newFilePath);

            if (fileExt.Equals(".txt") == false)
            {
                string message = string.Format("File Type '{0}' is not supported for previewing!", fileExt);

                MessageBox.Show(message, "Can't Read File", MessageBoxButton.OK, MessageBoxImage.Error);

                return false;
            }

            return true;
        }

        public void LoadInContents()
        {
            if(File.Exists(filePath))
            {
                try // read the contents in a try catch as double protection incase the .txt contents are actually unsupported.
                {
                    fileContents = File.ReadAllText(filePath);
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.ToString(), e.Message, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        public void UpdateUI()
        {
            string fileName = System.IO.Path.GetFileName(filePath);
            string final_fileName = string.Format("{0}/{1}", modDisplayName, fileName);

            ui_viewtext_filename_label.Content = final_fileName;
            ui_viewtext_filecontents_textbox.Text = fileContents;
        }

        private void ViewText_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //hide the window on closing instead of creating a new window object every time
            e.Cancel = true;
            this.Hide();
        }
    }
}
