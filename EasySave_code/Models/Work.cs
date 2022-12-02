using System;
using System.IO;

namespace EasySave.Models
{
	class Work
    {
        // --- Attributes --- //
        public string name { get; set; }
        public string source { get; set; }
        public string destination { get; set; }
        public BackupTypes backupType { get; set; }
        public State state { get; set; }
        public string lastBackupDate { get; set; }


        // --- Constructor --- //
        public Work() { }
        public Work(string _name, string _source, string _destination, BackupTypes _backupType)
        {
            this.name = _name;
            this.source = _source;
            this.destination = _destination;
            this.backupType = _backupType;
            this.state = null;
        }
        // --- Methods --- //
        public void CreateLog(DateTime _startDate, string _source, string _destination, long _fileSize, bool status)
        {
                // Prepare times log
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string startTime = _startDate.ToString("yyyy-MM-dd_HH-mm-ss");
            string elapsedTime = (DateTime.Now - _startDate).ToString();

            if (status)
            {
                elapsedTime = "-1";
            }

            // Create File if it doesn't exists
            if (!Directory.Exists("./Logs"))
            {
                Directory.CreateDirectory("./Logs");
            }

            // Write log
            File.AppendAllText($"./Logs/{today}.txt", $"{startTime}: {this.name}" +
                $"\nSource: {_source}" +
                $"\nDestination: {_destination}" +
                $"\nSize (Bytes): {_fileSize}" +
                $"\nElapsed Time: {elapsedTime}" +
                "\n\r\n");
        }
    }
}
