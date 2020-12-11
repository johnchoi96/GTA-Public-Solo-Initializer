using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows.Forms;

namespace GTAPublicInitializer
{
    /// <summary>
    /// Main screen controller.
    /// </summary>
    public partial class MainScreen : Form
    {

        /// <summary>
        /// URL that returns the download link for PSTools zip file.
        /// </summary>
        private readonly string pstoolsUrl = "https://download.sysinternals.com/files/PSTools.zip";

        /// <summary>
        /// If this is null, download has not started.
        /// </summary>
        private WebClient webClient = null;

        /// <summary>
        /// Returns the temporary path for the zip file to be downloaded and saved.
        /// In this path, the zip file is not extracted.
        /// </summary>
        private string ZipPath
        {
            get
            {
                string path = @"%TEMP%\JohnChoi\";

                path = Environment.ExpandEnvironmentVariables(path);
                return path;
            }
        }

        /// <summary>
        /// Returns the final location for the PS Tools where it is extracted.
        /// </summary>
        private string ExtractPath
        {
            get
            {
                string path = @"%LOCALAPPDATA%\JohnChoi\";

                path = Environment.ExpandEnvironmentVariables(path);
                return path;
            }
        }

        /// <summary>
        /// Returns true if the zip path exists.
        /// </summary>
        private bool ZipPathExists
        {
            get
            {
                return Directory.Exists(ZipPath);
            }
        }

        /// <summary>
        /// Returns true if the extraction path exists.
        /// </summary>
        private bool ExtractionPathExists
        {
            get
            {
                return Directory.Exists(ExtractPath);
            }
        }

        /// <summary>
        /// Returns true if required ps tool exists. In this case, checks for <code>pssuspend.exe</code>.
        /// </summary>
        private bool PSToolExists
        {
            get
            {
                if (!ExtractionPathExists)
                {
                    return false;
                }
                return File.Exists(ExtractPath + "pssuspend.exe");
            }
        }

        /// <summary>
        /// Constructor.
        /// Sets up the button states.
        /// </summary>
        public MainScreen()
        {
            InitializeComponent();
            installButton.Enabled = !PSToolExists;
            runButton.Enabled = PSToolExists;
        }

        /// <summary>
        /// Defines behavior for install button.
        /// </summary>
        /// <param name="sender">sender of the event</param>
        /// <param name="e">event argument</param>
        private void InstallClicked(object sender, EventArgs e)
        {
            StatusLabel.Text = "Installing....Please Wait...";
            CheckInstallPaths();
            Console.WriteLine(ExtractPath);

            DownloadPSTool();

            // delete temp file
            DeleteDirectory(ZipPath);
        }

        /// <summary>
        /// Defines behavior for run button.
        /// </summary>
        /// <param name="sender">sender for this event</param>
        /// <param name="e">event argument</param>
        private void RunClicked(object sender, EventArgs e)
        {
            // check if confirmed
            if (!IsConfirmed())
            {
                return;
            }
            StatusLabel.Text = "Running...";
            // Use ProcessStartInfo class
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                FileName = ExtractPath + "pssuspend.exe",
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = "gta5"
            };

            try
            {
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
                // done running so wait for 10 seconds...
                System.Threading.Thread.Sleep(10 * 1000);

                // done waiting so resume gta 5
                startInfo.Arguments = "gta5 -r";

                using (Process exeProcess = Process.Start(startInfo))
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception exception)
            {
                // Log error.
                string msgtitle = "Error";
                string msgmessage = exception.Message;
                MessageBoxIcon msgicon = MessageBoxIcon.Error;
                MessageBoxButtons msgbuttons = MessageBoxButtons.OK;
                MessageBox.Show(msgmessage, msgtitle, msgbuttons, msgicon);
                return;
            }
            // Display message box
            StatusLabel.Text = "Done";
            string title = "Complete!";
            string message = "GTA Public Server Initiated, ni[B][B]a plz confirm?";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            MessageBoxIcon icon = MessageBoxIcon.Exclamation;
            DialogResult result = MessageBox.Show(message, title, buttons, icon);

            if (result == DialogResult.No)
            {
                title = "b r u h";
                message = "PlEaSe CoNFiRM?";
                result = MessageBox.Show(message, title, buttons, icon);
                if (result == DialogResult.No)
                {
                    title = "bravo romeo uniform hotel";
                    message = "Papa lima echo alpha sierra echo charlie oscar november foxtrot india romeo mike";
                    MessageBox.Show(message, title, buttons, icon);
                }
            }
            
            StatusLabel.Text = "Idle";
        }

        private bool IsConfirmed()
        {
            if (!ConfirmCheckbox.Checked)
            {
                string title = "Can I get some credit?";
                string message = "You have not confirmed, please confirm";
                MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ConfirmCheckbox.Checked;
        }

        public void UninstallClicked(object sender, EventArgs e)
        {
            StatusLabel.Text = "Bruh moment initiated";
            string title = "Are you sure?";
            string message = "Are you sure you want to uninstall this?";
            MessageBoxButtons buttons = MessageBoxButtons.OKCancel;
            MessageBoxIcon icon = MessageBoxIcon.Warning;
            DialogResult result = MessageBox.Show(message, title, buttons, icon);
            if (result == DialogResult.OK)
            {
                title = "Seriously?";
                message = "Do you know how much time I spent on this thing?";
                buttons = MessageBoxButtons.YesNoCancel;
                icon = MessageBoxIcon.Exclamation;
                result = MessageBox.Show(message, title, buttons, icon);
                if (result != DialogResult.Cancel)
                {
                    // Delete extract path
                    DeleteDirectory(ExtractPath);
                    // Delete temp path
                    DeleteDirectory(ZipPath);
                    // Set up buttons
                    installButton.Enabled = !PSToolExists;
                    runButton.Enabled = PSToolExists;
                    StatusLabel.Text = "Idle";
                }
            }
            StatusLabel.Text = "Idle";
        }

        private void DeleteDirectory(string path)
        {
            try
            {
                // first, delete all files in the given directory path
                IEnumerable<string> iterator = Directory.EnumerateFiles(path);
                foreach (string filepath in iterator)
                {
                    File.Delete(filepath);
                }
                // now delete the directory itself
                Directory.Delete(path);
            }
            catch
            {
                return;
            }
            
        }

        public void HelpClicked(object sender, EventArgs e)
        {
            string title = "Information";
            string message = "Before you run this, GTA V must be running and you must be in the public server.";
            message += "\n\nThis application basically kicks yourself out of the current public server and makes a new";
            message += "public server with only you in it.";
            message += "\nDeveloped by @johnchoi96 please confirm?";

            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void CloseClicked(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        /// <summary>
        /// Download <code>PSTools.zip</code> to the <code>zipPath</code>.
        /// Calls <code>Completed()</code> method asynchronously when the download finishes.
        /// </summary>
        private void DownloadPSTool()
        {
            // do nothing if webClient is not null, which means it is in the middle of download.
            if (webClient != null)
            {
                return;
            }
            // specify download target path and file name
            string downloadPath = ZipPath + "PSTools.zip";
            webClient = new WebClient();
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
            webClient.DownloadFileAsync(new Uri(pstoolsUrl), downloadPath);
            Console.WriteLine("downloading..");
        }

        /// <summary>
        /// Performs the pre-setup before the installation begins.
        /// If <code>ZipPath</code> does not exist, it creates one.
        /// If <code>ExtractPath</code> does not exist, it creates one.
        /// </summary>
        private void CheckInstallPaths()
        {
            // check download path
            if (!ZipPathExists)
            {
                Directory.CreateDirectory(ZipPath);
            }
            // check extraction path
            if (!ExtractionPathExists)
            {
                Directory.CreateDirectory(ExtractPath);
            }
        }

        /// <summary>
        /// Asynchronous method.
        /// Called when the zip file finished downloading.
        /// Sets the <code>webClient</code> to null to indicate the completion of download and
        /// performs zip extraction to the specified directory (<code>ExtractPath</code>).
        /// Displays a message box when extraction is complete and enables/disables buttons based
        /// on whether if the required file exists in <code>ExtractPath</code>.
        /// </summary>
        /// <param name="sender">sender for this event</param>
        /// <param name="e">event argument</param>
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            webClient = null;

            // unzip
            try
            {
                ZipFile.ExtractToDirectory(ZipPath + "PSTools.zip", ExtractPath);
            } 
            catch
            {
                StatusLabel.Text = "Error";
                string msgTitle = "Error";
                string msgMessage = "Check internet connection!";
                MessageBox.Show(msgMessage, msgTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                StatusLabel.Text = "Idle";
                return;
            }


            // show confirmation alert box
            string title = "Installation Done";
            string message = "Installation Complete!";
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBoxIcon icon = MessageBoxIcon.Information;
            MessageBox.Show(message, title, buttons, icon);

            // if no errors...
            installButton.Enabled = !PSToolExists;
            runButton.Enabled = PSToolExists;
            StatusLabel.Text = "Idle";
        }
    }
}
