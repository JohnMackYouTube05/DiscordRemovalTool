using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Principal;
using System.Runtime.InteropServices;
using static DiscordRemovalTool.UacHelper;
using Microsoft.Win32;
using System.Threading;

namespace DiscordRemovalTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public string DiscordDownloadPath = Environment.GetEnvironmentVariable("TEMP") + "\\DiscordSetup.exe";
        private void Form1_Load(object sender, EventArgs e)
        {
            bool isAdmin = UacHelper.IsAdministrator(); //Check if program is running as an Administrator.
            if (!isAdmin)
            {
                MessageBox.Show("This program must be run as an administrator to work properly.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            string DiscordPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Discord";
            //Check if Discord is installed.
            if (Directory.Exists(DiscordPath)) {
                lbl_IsDiscordInstalled.Text = "YES";
                lbl_IsDiscordInstalled.ForeColor = Color.Lime;
            } else
            {
                lbl_IsDiscordInstalled.Text = "NO";
                lbl_IsDiscordInstalled.ForeColor = Color.Red;
                btnUninstall.Enabled = false;
            }
        }

        private void ChangeStatus(string message)
        {
            lblStatus.Text = message;
        }

        private void btnUninstall_Click(object sender, EventArgs e)
        {
            ChangeStatus("Checking if Discord is running...");
            Process[] DiscordProcess = Process.GetProcessesByName("Discord");
            if (DiscordProcess.Length > 0)
            {
                progressBar1.Value = 5;
                ChangeStatus("Discord is running. Killing all Discord processes...");
                foreach (Process p in DiscordProcess)
                {
                    ChangeStatus($"Killing Process {p.ProcessName} with ID {p.Id}...");
                    p.Kill();
                    if (progressBar1.Value < 15) { progressBar1.Value++; }
                }
            }
            else
            {
                ChangeStatus("Discord is not running. Moving on...");
                progressBar1.Value = 15;
            }
            ChangeStatus("Deleting Discord Start Menu shortcut...");
            File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Windows\\Start Menu\\Programs\\Discord Inc\\Discord.lnk"); //Delete Discord shortcut from the Start Menu folder.
            progressBar1.Value++;
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Discord.lnk"))
            {
                ChangeStatus("Deleting Discord Desktop shortcut...");
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\Discord.lnk");
                progressBar1.Value = 20;
            }
            ChangeStatus("Deleting Discord AppData directory...");
            string[] subdirs = Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\discord");
            if (subdirs.Length > 0)
            {
                foreach (string dirPath in subdirs)
                {
                    
                    string[] files = Directory.GetFiles(dirPath);
                    string[] dirs = Directory.GetDirectories(dirPath);
                    if (dirs.Length > 0)
                    {
                        foreach (string dir in dirs)
                        {
                            string[] sFiles = Directory.GetFiles(dir);
                            foreach(string file in sFiles)
                            {
                                if (progressBar1.Value < 35) { progressBar1.Value++; }
                                ChangeStatus($"Deleting {file}...");
                                try
                                {
                                    File.Delete(file);
                                }
                                catch (DirectoryNotFoundException ex)
                                {
                                    ChangeStatus(ex.Message);
                                }
                                catch (FileNotFoundException ex)
                                {
                                    ChangeStatus(ex.Message);
                                }
                                catch (IOException ex)
                                {
                                    ChangeStatus(ex.Message);
                                }
                            }
                            ChangeStatus($"Deleting {dir}");
                        }
                    }
                    else
                    {
                        ChangeStatus($"Deleting {dirPath} [Empty Directory]");
                        try
                        {
                            Directory.Delete(dirPath, true);
                        }
                        catch (DirectoryNotFoundException ex)
                        {
                            ChangeStatus(ex.Message);
                        }
                        catch (FileNotFoundException ex)
                        {
                            ChangeStatus(ex.Message);
                        }
                        catch (IOException ex)
                        {
                            ChangeStatus(ex.Message);
                        }

                    }
                    if (files.Length > 0)
                    {
                        foreach (string file in files)
                        {
                            if (progressBar1.Value < 35) { progressBar1.Value++; }
                            ChangeStatus($"Deleting {file}...");
                            try
                            {
                                File.Delete(file);
                            }
                            catch (DirectoryNotFoundException ex)
                            {
                                ChangeStatus(ex.Message);
                            }
                            catch (FileNotFoundException ex)
                            {
                                ChangeStatus(ex.Message);
                            }
                            catch (IOException ex)
                            {
                                ChangeStatus(ex.Message);
                            }
                        }
                        ChangeStatus($"Deleting {dirPath}");
                    } else
                    {
                        ChangeStatus($"Deleting {dirPath} [Empty Directory]");
                        try
                        {
                            Directory.Delete(dirPath, true);
                        }
                        catch (DirectoryNotFoundException ex)
                        {
                            ChangeStatus(ex.Message);
                        }
                        catch (FileNotFoundException ex)
                        {
                            ChangeStatus(ex.Message);
                        }
                        catch (IOException ex)
                        {
                            ChangeStatus(ex.Message);
                        }
                        progressBar1.Value = 35;
                    }
                }
            }
            string[] dirfiles = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\discord");
            if  (dirfiles.Length > 0)
            {
                foreach (string file in dirfiles)
                {
                    ChangeStatus($"Deleting {file}...");
                    try
                    {
                        File.Delete(file);
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                        ChangeStatus(ex.Message);
                    }
                    catch (FileNotFoundException ex)
                    {
                        ChangeStatus(ex.Message);
                    }
                    catch (IOException ex)
                    {
                        ChangeStatus(ex.Message);
                    }
                    if (progressBar1.Value < 45) { progressBar1.Value++; }
                }
            }
            ChangeStatus($"Deleting {Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Discord"}");
            Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\discord");
            progressBar1.Value = 45;
            ChangeStatus("Moving to Discord LocalAppData Directory.");
            string[] LAPSubDirs = Directory.GetDirectories(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Discord");
            if (LAPSubDirs.Length > 0)
            {
                foreach (string dir in LAPSubDirs)
                {
                    string[] sdFiles = Directory.GetFiles(dir);

                    if (sdFiles.Length > 0)
                    {
                        foreach (string sfile in sdFiles)
                        {
                            ChangeStatus($"Deleting {sfile}...");
                            File.Delete(sfile);
                            if (progressBar1.Value < 65)
                            {
                                progressBar1.Value++;
                            }
                        }
                        ChangeStatus($"Deleting {dir}");
                        try
                        {
                            Directory.Delete(dir, true);
                        }
                        catch (DirectoryNotFoundException ex)
                        {
                            ChangeStatus(ex.Message);
                        }
                        catch (FileNotFoundException ex)
                        {
                            ChangeStatus(ex.Message);

                        }
                        catch (IOException ex)
                        {
                            ChangeStatus(ex.Message);
                        }
                    }
                    else
                    {
                        ChangeStatus($"Deleting {dir} [Empty Directory]");
                        try
                        {
                            Directory.Delete(dir, true);
                        }
                        catch (DirectoryNotFoundException ex)
                        {
                            ChangeStatus(ex.Message);
                        }
                        catch (FileNotFoundException ex)
                        {
                            ChangeStatus(ex.Message);
                        }
                        catch (IOException ex)
                        {
                            ChangeStatus(ex.Message);
                        }
                    }
                }
            }
            progressBar1.Value = 65;
            string[] DiscordLAPFiles = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Discord");
            if (DiscordLAPFiles.Length > 0)
            {
                foreach (string f in DiscordLAPFiles)
                {
                    ChangeStatus($"Deleting {f}...");
                    File.Delete(f);
                    if (progressBar1.Value < 75) { progressBar1.Value++; }
                }
                ChangeStatus($"Deleting {Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Discord"}");
                try
                {
                    Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Discord");
                }
                catch (DirectoryNotFoundException ex)
                {
                    ChangeStatus(ex.Message);
                }
                catch (FileNotFoundException ex)
                {
                    ChangeStatus(ex.Message);
                }
                catch (IOException ex)
                {
                    ChangeStatus(ex.Message);
                }
                progressBar1.Value = 75;
            } else
            {
                ChangeStatus($"Deleting {Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Discord"} [Empty Directory]");
                try
                {
                    Directory.Delete(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Discord");
                }
                catch (DirectoryNotFoundException ex)
                {
                    ChangeStatus(ex.Message);
                }
                catch (FileNotFoundException ex)
                {
                    ChangeStatus(ex.Message);
                }
                catch (IOException ex)
                {
                    ChangeStatus(ex.Message);
                }
                progressBar1.Value = 75;
            }

            ChangeStatus("Deleting Discord Registry Keys...");
            string discordKeyPath = @"Software\Discord";

            using (RegistryKey discordKey =
                Registry.CurrentUser.OpenSubKey(discordKeyPath, writable: true))
            {
                if (discordKey != null)
                {
                    ChangeStatus("Deleting \\HKEY_CURRENT_USER\\Software\\Discord");
                    discordKey.DeleteSubKeyTree("Modules");
                    progressBar1.Value = 100;
                }
            }


            
            string DiscordPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Discord";
            //Check if Discord uninstalled successfully.
            if (Directory.Exists(DiscordPath))
            {
                ChangeStatus("Uninstalled Discord, but some residual files/directories remain. You'll have to go through the filesystem and delete these manually.");
                lbl_IsDiscordInstalled.Text = "YES";
                lbl_IsDiscordInstalled.ForeColor = Color.Lime;
            }
            else
            {
                ChangeStatus("Successfully uninstalled Discord!");
                lbl_IsDiscordInstalled.Text = "NO";
                lbl_IsDiscordInstalled.ForeColor = Color.Red;
                btnUninstall.Enabled = false;
            }

        }
        private void startDownload(string downloadUrl) //Starts a new download thread.
        {
            Thread thread = new Thread(() => {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += client_DownloadProgressChanged;
                client.DownloadFileCompleted += client_DownloadFileCompleted;
                
                client.DownloadFileAsync(new Uri(downloadUrl), DiscordDownloadPath);
            });
            thread.Start();
            btnUninstall.Enabled = false;
            btnReinstall.Enabled = false;
        }
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) //Method responsible for changing the progress bar while the setup program is downloading.
        {
            this.BeginInvoke((MethodInvoker)delegate {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                lblStatus.Text = "Downloading DiscordSetup.exe... (" + e.BytesReceived + " of " + e.TotalBytesToReceive + " bytes)";
                progressBar1.Value = int.Parse(Math.Truncate(percentage).ToString());
            });
        }
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e) //Runs once DiscordSetup.exe has downloaded successfully.
        {
            this.BeginInvoke((MethodInvoker)delegate {
                lblStatus.Text = "Completed! Running setup...";
                using (Process DiscordSetup = new Process())
                {
                    DiscordSetup.StartInfo.FileName = DiscordDownloadPath;
                    DiscordSetup.StartInfo.Verb = "runas";
                    DiscordSetup.Start();
                    DiscordSetup.WaitForExit();
                    //Check if Discord reinstalled correctly.
                    if (DiscordSetup.ExitCode == 0)
                    {
                        lblStatus.Text = $"Successfully reinstalled Discord!";
                        lbl_IsDiscordInstalled.ForeColor = Color.Lime;
                        lbl_IsDiscordInstalled.Text = "YES";
                    }
                    else
                    {
                        lblStatus.Text = "Failed to reinstall Discord.";
                        lbl_IsDiscordInstalled.ForeColor = Color.Red;
                        lbl_IsDiscordInstalled.Text = "NO";
                    }
                    
                    
                }
            });

        }
        private void btnReinstall_Click(object sender, EventArgs e)
        {
            
            var client = new WebClient();
            startDownload("https://discordapp.com/api/download?platform=win");
        }
    }
}
