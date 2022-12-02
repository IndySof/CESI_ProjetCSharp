using EasySave.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EasySave.Views
{
    class View
    {
        public string language { get; set; }
        private ViewModel viewModel;
        public View(ViewModel _viewModel, string _language)
        {
            this.viewModel = _viewModel;
            this.language = _language;
        }
        public string MainMenu()
        {
            if (language == "EN")
            {
                Console.Clear();
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1 - Show Works in the wait-list");
                Console.WriteLine("2 - Add work");
                Console.WriteLine("3 - Delete Work");
                Console.WriteLine("4 - Launch the backup");
                Console.WriteLine("5 - Choose language");
                Console.WriteLine("6 - Exit");
                Console.Write("\r\nSelect an option: ");
            }
            else
            {
                Console.Clear();
                Console.WriteLine("Choisissez une option :");
                Console.WriteLine("1 - Montrer les œuvres dans la liste d'attente");
                Console.WriteLine("2 - Ajouter un travail");
                Console.WriteLine("3 - Supprimer un travail");
                Console.WriteLine("4 - Lancer la sauvegarde");
                Console.WriteLine("5 - Choisir la langue");
                Console.WriteLine("6 - Exit");
                Console.Write("\r\nSélectionner une option : ");
            }
            return Console.ReadLine();
        }

        public int LanguageChoice()
        {
            Console.Clear();
            DisplayMessage(309);
            DisplayMessage(2);
            Console.WriteLine();
            Console.WriteLine("1. English");
            Console.WriteLine("2. Français");
            return Int32.Parse(Console.ReadLine());
        }

        public int ChooseBackupType()
        {
            if (language == "EN")
            {
                Console.WriteLine(
                    "\nChoose a type of Backup: " +
                    "\n1 - Full " +
                    "\n2 - Differential");
            }
            else
            {
                Console.WriteLine(
                    "\nChoisissez un type de sauvegarde : " +
                    "\n1 - Complète " +
                    "\n2 - Différentielle");
            }
            
            return Int32.Parse(Console.ReadLine());
        }

        public void FormatWorks(int _shift)
        {
            var works = this.viewModel.model.works;
            for (int i = 0; i < works.Count; i++)
            {
                Console.WriteLine(
                    "\n" + (i + _shift) + " - Name: " + works[i].name
                  + "\n    Source: " + works[i].source
                  + "\n    Destination: " + works[i].destination
                  + "\n    Type: " + works[i].backupType);
            }
        }

        public int LaunchBackupChoice()
        {
            Console.Clear();
            if (language == "EN")
            {
                Console.WriteLine(
                    "Choose the work to save: " +
                    "\n\n1 - all");
            }
            else
            {
                Console.WriteLine(
                    "Choisissez le travail à sauvegarder :" +
                    "\n\n1 - Tous");
            }

            FormatWorks(2);
            DisplayMessage(2);
            return Int32.Parse(Console.ReadLine());
        }

        public void DisplayWorks()
        {
            Console.Clear();
            if (language == "EN")
            {
                Console.WriteLine("Work list:");
            }
            else
            {
                Console.WriteLine("Liste des travaux:");
            }
            

            FormatWorks(1);
            DisplayMessage(1);
        }
        public string AddWorkName()
        {
            Console.Clear();
            if (language == "EN")
            {
                Console.WriteLine("Parameter to add a work:");
                DisplayMessage(1);
                Console.WriteLine("\nEnter a name (1 to 20 characters):");
            }
            else
            {
                Console.WriteLine("Paramètre pour ajouter une œuvre :");
                DisplayMessage(1);
                Console.WriteLine("\nSaisissez un nom (1 à 20 caractères) :");
            }
            
            string name = Console.ReadLine();
            while (!CheckName(name))
            {
                name = Console.ReadLine();
            }
            return name;
        }

        private string RectifyPath(string _path)
        {
            if (_path != "0" && _path.Length >= 1)
            {
                _path += (_path.EndsWith("/") || _path.EndsWith("\\")) ? "" : "\\";
                _path = _path.Replace("/", "\\");
            }
            return _path.ToLower();
        }

        public string AddWorkSouce()
        {
            if (language == "EN")
            {
                Console.WriteLine("\nEnter directory source. ");
            }
            else
            {
                Console.WriteLine("\nEntrer un répertoire source");
            }
            
            string src = RectifyPath(Console.ReadLine());

            while (!Directory.Exists(src) && src != "0")
            {
                DisplayMessage(211);
                src = RectifyPath(Console.ReadLine());
            }
            return src;
        }

        public string AddWorkDestination(string _src)
        {
            if (language == "EN")
            {
                Console.WriteLine("\nEnter directory source. ");
            }
            else
            {
                Console.WriteLine("\nEntrer un répertoire de destination");
            }
            string dst = RectifyPath(Console.ReadLine());
            while (!CheckWorkDst(_src, dst))
            {
                dst = RectifyPath(Console.ReadLine());
            }
            return dst;
        }

        private bool CheckWorkDst(string _source, string _destination)
        {
            if (_destination == "0")
            {
                return true;

            }
            else if (Directory.Exists(_destination))
            {
                if (_source != _destination)
                {
                    if (_destination.Length > _source.Length)
                    {
                        if (_source != _destination.Substring(0, _source.Length))
                        {
                            return true;
                        }
                        else
                        {
                            DisplayMessage(217);
                            return false;
                        }
                    }
                    return true;
                }
                DisplayMessage(212);
                return false;
            }
            DisplayMessage(213);
            return false;
        }
        private bool CheckName(string _name)
        {
            int length = _name.Length;

            if (length >= 1 && length <= 20)
            {
                if (!this.viewModel.model.works.Exists(work => work.name == _name))
                {
                    return true;
                }
                DisplayMessage(214);
                return false;
            }
            DisplayMessage(215);
            return false;
        }
        public void DisplayFiledError(string _name)
        {
            Console.WriteLine("File named " + _name + " failed.");
        }

        public int RemoveWorkChoice()
        {
            Console.Clear();
            Console.WriteLine("Choose the work to remove :");

            //Display all works 
            FormatWorks(1);
            DisplayMessage(2);
            return Int32.Parse(Console.ReadLine());
        }
        public void DisplayBackupRecap(string _name, double _transferTime)
        {
            Console.WriteLine("\n\n" +
                "Backup : " + _name + " finished\n"
                + "\nTime taken : " + _transferTime + " ms\n");
            DisplayProgressBar(100);
        }

        private string ConvertSize(long _octet)
        {
            if (_octet > 1000000000000)
            {
                return Math.Round((decimal)_octet / 1000000000000, 2) + " To";
            }
            else if (_octet > 1000000000)
            {
                return Math.Round((decimal)_octet / 1000000000, 2) + " Go";
            }
            else if (_octet > 1000000)
            {
                return Math.Round((decimal)_octet / 1000000, 2) + " Mo";
            }
            else if (_octet > 1000)
            {
                return Math.Round((decimal)_octet / 1000, 2) + " ko";
            }
            else
            {
                return _octet + " o";
            }
        }
        public void DisplayCurrentState(string _name, int _fileLeft, long _leftSize, long _curSize, int _pourcent)
        {
            Console.Clear();
            Console.WriteLine(
                "Current backup : " + _name
                + "\nSize of the current file : " + ConvertSize(_curSize)
                + "\nNumber of files left : " + _fileLeft
                + "\nSize of the files left : " + ConvertSize(_leftSize) + "\n");
            DisplayProgressBar(_pourcent);
        }
        private void DisplayProgressBar(int _pourcent)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("Progress: [ " + _pourcent + "%]");
            Console.ResetColor();

            Console.Write(" [");
            for (int i = 0; i < 100; i += 5)
            {
                if (_pourcent > i)
                {
                    Console.Write("#");
                }
                else
                {
                    Console.Write(".");
                }
            }
            Console.Write("]\n\n");
        }

        public void DisplayMessage(int errorCode)
        {
            if (errorCode < 100 && language == "FR")
            {
                switch (errorCode)
                {
                    case 1:
                        Console.WriteLine("\nAppuyez sur la touche Enter pour afficher le menu . . .");
                        Console.ReadLine();
                        break;
                    case 2:
                        Console.WriteLine("\n(Entrez 0 pour revenir au menu)");
                        break;
                }
            }
            if (errorCode < 100)
            {
                switch (errorCode)
                {
                    case 1:
                        Console.WriteLine("\nPress Enter key to display menu . . .");
                        Console.ReadLine();
                        break;
                    case 2:
                        Console.WriteLine("\n(Enter 0 to return to the menu)");
                        break;
                }
            }
            else if (errorCode < 200 && language == "FR")
            {
                switch (errorCode)
                {
                    // Success message from 100 to 199
                    case 100:
                        Console.WriteLine("\n----- BIENVENUE SUR EASYSAVE -----");
                        DisplayMessage(1);
                        break;

                    case 101:
                        Console.WriteLine("\nLe travail a été ajouté avec succès !");
                        DisplayMessage(1);
                        break;

                    case 102:
                        Console.WriteLine("\nLe travail a été sauvegardé avec succès !");
                        break;

                    case 103:
                        Console.WriteLine("\nLe travail a été enlevé avec succès !");
                        DisplayMessage(1);
                        break;

                    case 104:
                        Console.WriteLine("\nSauvegarde réussie !");
                        break;

                    case 105:
                        Console.WriteLine("\nAucune modification depuis la dernière sauvegarde complète !\n");
                        break;
                }
            }
            else if (errorCode < 200)
            {
                switch (errorCode)
                {
                    // Success message from 100 to 199
                    case 100:
                        Console.WriteLine("\n----- WELCOME ON EASYSAVE -----");
                        DisplayMessage(1);
                        break;

                    case 101:
                        Console.WriteLine("\nThe work was added with success!");
                        DisplayMessage(1);
                        break;

                    case 102:
                        Console.WriteLine("\nThe work was saved with success!");
                        break;

                    case 103:
                        Console.WriteLine("\nThe work was removed with success!");
                        DisplayMessage(1);
                        break;

                    case 104:
                        Console.WriteLine("\nBackup success !");
                        break;

                    case 105:
                        Console.WriteLine("\nNo modification since the last full backup!\n");
                        break;
                }
            }
            else if(errorCode < 300 && language == "FR")
            {
                Console.ForegroundColor = ConsoleColor.Red;
                switch (errorCode)
                {
                    // Error message from 200 to 299
                    case 200:
                        Console.WriteLine("\nVeuillez restaurer votre fichier de sauvegarde JSON.");
                        DisplayMessage(1);
                        break;

                    case 201:
                        Console.WriteLine("\nÉchec de l'ajout de travail.");
                        DisplayMessage(1);
                        break;

                    case 202:
                        Console.WriteLine("\nÉchec de l'enregistrement du travail.");
                        DisplayMessage(1);
                        break;

                    case 203:
                        Console.WriteLine("\nImpossible de retirer le travail.");
                        DisplayMessage(1);
                        break;

                    case 204:
                        Console.WriteLine("\nLa liste de travail est vide.");
                        DisplayMessage(1);
                        break;

                    case 205:
                        Console.WriteLine("\nLa liste de travail est complète.");
                        DisplayMessage(1);
                        break;

                    case 206:
                        Console.WriteLine("\nVeuillez entrer une option valide");
                        break;

                    case 207:
                        Console.WriteLine("\nÉchec du déplacement d'un fichier, le fichier source ou de destination n'existe pas.");
                        break;

                    case 208:
                        Console.WriteLine("\nLe type de sauvegarde sélectionné n'existe pas.");
                        break;

                    case 209:
                        Console.WriteLine("\nÉchec de la copie du fichier.");
                        DisplayMessage(1);
                        break;
                    case 210:
                        Console.WriteLine("\nÉchec de la création du dossier de sauvegarde.");
                        DisplayMessage(1);
                        break;
                    case 211:
                        Console.WriteLine("\nLe répertoire n'existe pas. Veuillez entrer une source de répertoire valide. ");
                        break;

                    case 212:
                        Console.WriteLine("\nChoisissez un chemin différent de la source.");
                        break;

                    case 213:
                        Console.WriteLine("\nLe répertoire n'existe pas. Veuillez entrer une direction de répertoire valide.");
                        break;

                    case 214:
                        Console.WriteLine("\nWorkName déjà pris. Veuillez entrer un autre nom.");
                        break;

                    case 215:
                        Console.WriteLine("\nEntrez un nom VALIDE (1 à 20 caractères) :");
                        break;

                    case 216:
                        Console.WriteLine("\nLa sauvegarde s'est terminée avec une erreur.");
                        break;

                    case 217:
                        Console.WriteLine("\nLe répertoire de destination ne peut pas être à l'intérieur du répertoire source.");
                        break;

                    default:
                        Console.WriteLine("\nFailed : Erreur inconnue.");
                        DisplayMessage(1);
                        break;
                }
            }
            else if (errorCode < 300)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                switch (errorCode)
                {
                    // Error message from 200 to 299
                    case 200:
                        Console.WriteLine("\nPlease restore your JSON backup file.");
                        DisplayMessage(1);
                        break;

                    case 201:
                        Console.WriteLine("\nFailed to add work.");
                        DisplayMessage(1);
                        break;

                    case 202:
                        Console.WriteLine("\nFailed to saved work.");
                        DisplayMessage(1);
                        break;

                    case 203:
                        Console.WriteLine("\nFailed to removed work.");
                        DisplayMessage(1);
                        break;

                    case 204:
                        Console.WriteLine("\nWork List is empty.");
                        DisplayMessage(1);
                        break;

                    case 205:
                        Console.WriteLine("\nWork List is full.");
                        DisplayMessage(1);
                        break;

                    case 206:
                        Console.WriteLine("\nPlease enter a valid option");
                        break;

                    case 207:
                        Console.WriteLine("\nFailed to move a file, destination or source file do not exists.");
                        break;

                    case 208:
                        Console.WriteLine("\nSelected backup type doesn't exists.");
                        break;

                    case 209:
                        Console.WriteLine("\nFailed to copy file.");
                        DisplayMessage(1);
                        break;

                    case 210:
                        Console.WriteLine("\nFailed to create the backup folder.");
                        DisplayMessage(1);
                        break;
                    case 211:
                        Console.WriteLine("\nDirectory doesn't exist. Please enter a valid directory source. ");
                        break;

                    case 212:
                        Console.WriteLine("\nChoose a different path from the source. ");
                        break;

                    case 213:
                        Console.WriteLine("\nDirectory doesn't exist. Please enter a valid directory direction. ");
                        break;

                    case 214:
                        Console.WriteLine("\nWorkName already taken. Please enter an other name.");
                        break;

                    case 215:
                        Console.WriteLine("\nEnter a VALID name (1 to 20 characters):");
                        break;

                    case 216:
                        Console.WriteLine("\nBackup finished with error.");
                        break;

                    case 217:
                        Console.WriteLine("\nDestination directory cannot be inside the source directory.");
                        break;

                    default:
                        Console.WriteLine("\nFailed : Error Unknown.");
                        DisplayMessage(1);
                        break;
                }
            }
            Console.ResetColor();
        }
    }
}
