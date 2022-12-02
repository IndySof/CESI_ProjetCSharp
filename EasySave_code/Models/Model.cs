using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace EasySave.Models
{
    class Model
    {
        private string listOfWorksPath = "./listOfWorks.json";
        private JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true
        };
        public List<Work> works;
        public Model()
        {
            this.works = new List<Work>();
        }
        public int AddWork(string _name, string _source, string _destination, BackupTypes _backupType)
        {
            try
            {
                Console.WriteLine("backup type = " + _backupType);
                this.works.Add(new Work(_name, _source, _destination, _backupType));
                SaveListWork();
                return 101;
            }
            catch
            {
                return 201;
            }
        }
        public int RemoveWork(int _workIndex)
        {
            try
            {
                this.works.RemoveAt(_workIndex);
                SaveListWork();
                return 103;
            }
            catch
            {
                return 203;
            }
        }
        public void SaveListWork()
        {
            File.WriteAllText(this.listOfWorksPath, JsonSerializer.Serialize(this.works, this.jsonOptions));
        }
        // Load Works and States at the beginning of the program
        public int GetListWork()
        {
            if (File.Exists(listOfWorksPath))
            {
                try
                {
                    // Read Works from JSON File (from ./BackupWorkSave.json) (use Work() constructor)
                    this.works = JsonSerializer.Deserialize<List<Work>>(File.ReadAllText(this.listOfWorksPath));
                }
                catch
                {
                    // Return Error Code
                    return 200;
                }
            }
            // Return Success Code
            return 100;
        }
        public bool SaveFileTo(Work _work, FileInfo _currentFile, string _destination, long _curSize, long _leftSize, int _totalFile, int fileIndex, int _pourcent)
        {
            DateTime startTimeFile = DateTime.Now;
            string currentDir = _currentFile.DirectoryName;
            string destinationDir = _destination;
            string savedFileName = Path.Combine(destinationDir, _currentFile.Name);

            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }
            try
            {
                _currentFile.CopyTo(savedFileName, true);
            }
            catch (IOException iox)
            {
                Console.WriteLine(iox.Message);
            }
            // Get the current dstFile
            string dstFile = destinationDir + _currentFile.Name;

            try
            {
                // Update the current work status
                _work.state.UpdateState(_pourcent, (_totalFile - fileIndex), _leftSize, _currentFile.FullName, dstFile);
                SaveListWork();

                // Copy the current file
                _currentFile.CopyTo(dstFile, true);

                // Save Log
                _work.CreateLog(startTimeFile, _currentFile.FullName, dstFile, _curSize, false);
                return true;
            }
            catch
            {
                _work.CreateLog(startTimeFile, _currentFile.FullName, dstFile, _curSize, true);
                return false;
            }
        }
    }
}
