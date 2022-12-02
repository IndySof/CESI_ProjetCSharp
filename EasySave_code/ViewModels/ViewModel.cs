using EasySave.Models;
using EasySave.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace EasySave.ViewModels
{
    class ViewModel
    {
        public Model model;
        public View view;
        public Settings settings { get; set; }
        private string settingsFilePath = "./Settings.json";
        private JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };

        // --- Constructor ---
        public ViewModel()
        {
            this.settings = Settings.GetInstance();
            this.settings.Update(Languages.EN);
            LoadSettings();
            this.model = new Model();
            this.view = new View(this, this.settings.language.ToString());
            this.view.DisplayMessage(this.model.GetListWork());
        }
        public void Run()
        {
            bool showMenu = true;
            while (showMenu)
            {
                switch (view.MainMenu())
                {
                    case "1":
                        DisplayWorkInView();
                        showMenu = true;
                        break;
                    case "2":
                        AddWork();
                        showMenu = true;
                        break;
                    case "3":
                        RemoveWork();
                        showMenu = true;
                        break;
                    case "4":
                        LaunchBackup();
                        showMenu = true;
                        break;
                    case "5":
                        ChooseLanguage();
                        break;
                    case "6":
                        showMenu = false;
                        break;
                    default:
                        showMenu = true;
                        break;
                }
            }
        }

        private void ChooseLanguage()
        {
            Languages language;
            switch (view.LanguageChoice())
            {
                case 0:
                    return;
                case 1:
                    language = Languages.EN;
                    break;
                case 2:
                    language = Languages.FR;
                    break;
                default:
                    language = Languages.EN;
                    break;
            }
            this.settings.Update(language);
            this.SaveSettings();
            this.view.language = language.ToString();
        }

        public int LoadSettings()
        {
            if (File.Exists(settingsFilePath))
            {
                try
                {
                    this.settings = JsonSerializer.Deserialize<Settings>(File.ReadAllText(this.settingsFilePath));
                }
                catch
                {
                    return 200;
                }
            }
            else
            {
                SaveSettings();
            }
            return 100;
        }

        public void SaveSettings()
        {
            File.WriteAllText(this.settingsFilePath, JsonSerializer.Serialize(this.settings, this.jsonOptions));
        }

        // Add Work in the program
        private void AddWork()
        {
            if (this.model.works.Count < 5)
            {
                string addWorkName = view.AddWorkName();
                if (addWorkName == "0") return;

                string addWorkSrc = view.AddWorkSouce();
                if (addWorkSrc == "0") return;

                string addWorkDest = view.AddWorkDestination(addWorkSrc);
                if (addWorkDest == "0") return;

                BackupTypes addWorkBackupType;
                switch (view.ChooseBackupType())
                {
                    case 0:
                        return;

                    case 1:
                        addWorkBackupType = BackupTypes.FULL;
                        break;

                    case 2:
                        addWorkBackupType = BackupTypes.DIFFERENTIAL;
                        break;

                    default:
                        addWorkBackupType = BackupTypes.FULL;
                        break;
                }
                this.view.DisplayMessage(model.AddWork(addWorkName, addWorkSrc, addWorkDest, addWorkBackupType));
            }
            else
            {
                this.view.DisplayMessage(205);
            }
        }
        private void RemoveWork()
        {
            if (this.model.works.Count > 0)
            {
                int RemoveChoice = this.view.RemoveWorkChoice() - 1;
                if (RemoveChoice == -1) return;

                this.view.DisplayMessage(this.model.RemoveWork(RemoveChoice));
            }
            else
            {
                this.view.DisplayMessage(204);
            }
        }

        private void DisplayWorkInView()
        {
            if (this.model.works.Count > 0)
            {
                this.view.DisplayWorks();
            }
            else
            {
                this.view.DisplayMessage(204);
            }
        }

        private void LaunchBackup()
        {
            if (this.model.works.Count > 0)
            {
                int userChoice = view.LaunchBackupChoice();

                switch (userChoice)
                {
                    // Return to the menu
                    case 0:
                        return;

                    // Run every work one by one
                    case 1:
                        foreach (Work work in this.model.works)
                        {
                            this.view.DisplayMessage(LaunchBackupType(work));
                            //this.view.DisplayMessage(1);
                        }
                        break;

                    // Run one work from his ID in the list
                    default:
                        int indexWork = userChoice - 2;
                        this.view.DisplayMessage(LaunchBackupType(this.model.works[indexWork]));
                        break;
                }
                this.view.DisplayMessage(1);
            }
            else
            {
                this.view.DisplayMessage(204);
            }
        }

        public int LaunchBackupType(Work _work)
        {
            DirectoryInfo dir = new DirectoryInfo(_work.source);
            if (!dir.Exists && !Directory.Exists(_work.destination))
            {
                return 207;
            }

            switch (_work.backupType)
            {
                case BackupTypes.DIFFERENTIAL:
                    string fullBackupDir = null;
                    DirectoryInfo[] dirs = new DirectoryInfo(_work.destination).GetDirectories();

                    foreach (DirectoryInfo directory in dirs)
                    {
                        if (directory.Name.IndexOf("_") > 0 && _work.name == directory.Name.Substring(0, directory.Name.IndexOf("_")))
                        {
                            fullBackupDir = directory.FullName;
                        }
                    }
              
                    if (fullBackupDir != null)
                    {
                        return DifferentialBackupSetup(_work, dir, fullBackupDir);
                    }
                    return FullBackupSetup(_work, dir);

                case BackupTypes.FULL:
                    return FullBackupSetup(_work, dir);

                default:
                    return 208;
            }
        }

        private int FullBackupSetup(Work _work, DirectoryInfo _dir)
        {
            long totalSize = 0;
            // Get evvery files of the source directory
            FileInfo[] files = _dir.GetFiles("*.*", SearchOption.AllDirectories);
            // Calcul the size of every files
            foreach (FileInfo file in files)
            {
                totalSize += file.Length;
            }
            return DoBackup(_work, files, totalSize);
        }

        // Differential Backup
        private int DifferentialBackupSetup(Work _work, DirectoryInfo _dir, string _fullBackupDir)
        {
            long totalSize = 0;

            // Get every files of the source directory
            FileInfo[] srcFiles = _dir.GetFiles("*.*", SearchOption.AllDirectories);
            List<FileInfo> filesToCopy = new List<FileInfo>();

            // Check if there is a modification between the current file and the last full backup
            foreach (FileInfo file in srcFiles)
            {
                string currFullBackPath = _fullBackupDir + "\\" + Path.GetRelativePath(_work.source, file.FullName);

                if (!File.Exists(currFullBackPath) || !IsSameFile(currFullBackPath, file.FullName))
                {
                    // Calcul the size of every files
                    totalSize += file.Length;

                    // Add the file to the list
                    filesToCopy.Add(file);
                }
            }

            // Test if there is file to copy
            if (filesToCopy.Count == 0)
            {
                _work.lastBackupDate = DateTime.Now.ToString("yyyy/MM/dd_HH:mm:ss");
                this.model.SaveListWork();
                this.view.DisplayMessage(3);
                this.view.DisplayBackupRecap(_work.name, 0);
                return 105;
            }
            return DoBackup(_work, filesToCopy.ToArray(), totalSize);
        }

        private bool IsSameFile(string path1, string path2)
        {
            byte[] file1 = File.ReadAllBytes(path1);
            byte[] file2 = File.ReadAllBytes(path2);

            if (file1.Length == file2.Length)
            {
                for (int i = 0; i < file1.Length; i++)
                {
                    if (file1[i] != file2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private int DoBackup(Work _work, FileInfo[] _files, long _totalSize)
        {
            DateTime startTime = DateTime.Now;
            string dest = _work.destination + "\\" + _work.name + "_" + startTime.ToString("yyyy-MM-ddTHH-mm-ss") + "\\";

            // Update the current work status
            _work.state = new State(_files.Length, _totalSize, _work.source, dest);
            _work.lastBackupDate = startTime.ToString("yyyy/MM/dd_HH:mm:ss");

            // Create the dst folder
            try
            {
                Directory.CreateDirectory(dest);
            }
            catch
            {
                return 210;
            }
            List<string> failedFiles = CopyFiles(_work, _files, dest, _totalSize);

            // Calculate the time of the all process of copy
            DateTime endTime = DateTime.Now;
            TimeSpan workTime = endTime - startTime;
            double transferTime = workTime.TotalMilliseconds;

            // Update the current work status
            _work.state = null;
            this.model.SaveListWork();
            this.view.DisplayMessage(3);

            foreach (string failedFile in failedFiles)
            {
                this.view.DisplayFiledError(failedFile);
            }
            this.view.DisplayBackupRecap(_work.name, transferTime);

            if (failedFiles.Count == 0)
            {
                // Return Success Code
                return 104;
            }
            else
            {
                // Return Error Code
                return 216;
            }
        }

        public List<string> CopyFiles(Work _work, FileInfo[] _files, string _destination, long _totalSize)
        {
            long leftSize = _totalSize;
            // Number of Files
            int totalFile = _files.Length;
            List<string> failedFiles = new List<string>();

            for (int i = 0; i < _files.Length; i++)
            {
                int pourcent = (i * 100 / totalFile);
                long curSize = _files[i].Length;
                leftSize -= curSize;
                if (this.model.SaveFileTo(_work, _files[i], _destination, curSize, leftSize, totalFile, i, pourcent))
                {
                    this.view.DisplayCurrentState(_work.name, (totalFile - i), leftSize, curSize, pourcent);
                }
                else
                {
                    failedFiles.Add(_files[i].Name);
                }
            }
            return failedFiles;
        }
    }
}


