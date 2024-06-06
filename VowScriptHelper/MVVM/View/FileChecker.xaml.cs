using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using File = System.IO.File;
using Path = System.IO.Path;

namespace VowScriptHelper.MVVM.View
{
    /// <summary>
    /// Interaction logic for FileChecker.xaml
    /// </summary>
    public partial class FileChecker : UserControl
    {
        public FileChecker()
        {
            InitializeComponent();
        }
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void FileDropStackPanel_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            FillBoxes((string[])e.Data.GetData(DataFormats.FileDrop));
        }


        private void FillBoxes(string[] files)
        {
            var outPut = new StringBuilder();

            List<string> lines = new List<string>();

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);

                if (!fileName.EndsWith(".txt"))
                    continue;

                string path = Path.Combine(Path.GetDirectoryName(file), file);

                lines.AddRange(File.ReadAllLines(path));
            }

            FormatLines(ref lines);
            FillInputBox(lines);



        }



        private void FillInputBox(List<string> lines)
        {
            InputBox.Clear();
            for (int i = 0; i < lines.Count; i++)
            {

                string str = lines[i];
                InputBox.Text += str + "\n";
            }
        }

        private void FillCodeBox(List<string> lines, List<string> fileNames)
        {
            CodeOutputBox.Clear();

            string directory = DirectoryInputBox.Text;
            if (String.IsNullOrWhiteSpace(directory))
            {
                CodeOutputBox.Text = "Please enter a directory";
                return;
            }
            
            //Check if the diretory exists
            if (!Directory.Exists(directory))
            {
                CodeOutputBox.Text = "Directory does not exist";
                return;
            }
            
            //Get a list of all file names inside of the directory
            string[] files = Directory.GetFiles(directory);
            List<string> fileNamesInDirectory = new List<string>();
            
            //Find out if fileNames contains any files not in the fileNamesInDirectory and if fileNamesInDirectory contains any files not in fileNames
            foreach (var file in files)
            {
                fileNamesInDirectory.Add(Path.GetFileName(file).Replace(".wav", "").Replace(".ogg", ""));
            }
            
            // Find out if fileNames contains any files not in the fileNamesInDirectory
            var filesNotInDirectory = fileNames.Except(fileNamesInDirectory).ToList();

            // Find out if fileNamesInDirectory contains any files not in fileNames
            var filesNotInFileNames = fileNamesInDirectory.Except(fileNames).ToList();
            
            StringBuilder stringBuilder = new StringBuilder();

            //If bot hare empty
            if (filesNotInDirectory.Count == 0 && filesNotInFileNames.Count == 0)
            {
                CodeOutputBox.Text = "All files are in the directory and in the file";
                return;
            }
            
            
            
            //If there are files in fileNames that are not in the directory
            if (filesNotInDirectory.Count > 0)
            {
                stringBuilder.AppendLine("The following files are not in the directory:");
                foreach (var file in filesNotInDirectory)
                {
                    stringBuilder.AppendLine(file);
                }
            }
            
            //If there are files in the directory that are not in fileNames
            if (filesNotInFileNames.Count > 0)
            {
                stringBuilder.AppendLine("The following files are in the directory but not in the file:");
                foreach (var file in filesNotInFileNames)
                {
                    stringBuilder.AppendLine(file);
                }
            }
            
            CodeOutputBox.Text = stringBuilder.ToString();
        }


        private static string GetCodeLine(string line, string fileName)
        {
            string outPut = "s.addSound(\"";
            line = line.Replace("\"", "\\\"");

            outPut += RemoveCharacterChangeNote(line).Trim();
            outPut += "\", \"";


            outPut += fileName;
            outPut += "\", false);";
            return outPut;
        }


        private List<string> GenerateFileNames(List<string> lines)
        {
            List<string> fileNames = new List<string>();
            String questName = QuestNameInPutBox.Text;
            questName = questName.ToLower();
            //Remove empty chars
            questName = questName.Replace(" ", "");


            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                string name = GetName(line).ToLower();
                //Remove empty chars
                name = name.Replace(" ", "");
                name = Regex.Replace(name, @"[^a-zA-Z0-9]", "");

                name = questName + "-" + name;

                int dialogueNumber = 1;
                foreach (string fileName in fileNames)
                {
                    if (fileName.Contains(name + "-"))
                        dialogueNumber++;
                }

                fileNames.Add(name + "-" + dialogueNumber);
            }

            return fileNames;
        }


        private static string RemoveCharacterChangeNote(string line)
        {

            if (line.Contains("/$"))
            {
                return GetStringBefore(line, "/$");
            }
            return line;

        }

        private static string GetName(string line)
        {
            int dollarSlashIndex = line.IndexOf("/$");
            if (dollarSlashIndex != -1)
            {
                return line.Substring(dollarSlashIndex + 2).Trim();
            }


            if (!line.StartsWith("["))
            {
                return "unknown";
            }

            int index = line.IndexOf("] ");
            if (index == -1)
            {
                return "unknown";
            }

            string name = line.Substring(index + 2);
            name = GetStringBefore(name, ":");
            return name.Trim();
        }


        private static string GetStringBefore(string text, string stopAt = "-")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }

            return String.Empty;
        }

        private static void FormatLines(ref List<string> lines)
        {
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                string line = lines[i];


                if (line.StartsWith("//") || line.StartsWith("---") || line == "" || line.Contains("Emotions will"))
                {
                    lines.RemoveAt(i);
                    continue;
                }

                //Remove notes
                if (line.Contains("{") && line.Contains("}"))
                {
                    line = RemoveBetween(line, "{", "}");
                }
                if (line.Contains("//"))
                {
                    line = GetStringBefore(line, "//");
                }
                lines[i] = line;

            }

        }



        private static string RemoveBetween(string sourceString, string startTag, string endTag)
        {
            Regex regex = new Regex(string.Format("{0}(.*?){1}", Regex.Escape(startTag), Regex.Escape(endTag)), RegexOptions.RightToLeft);
            string newString = regex.Replace(sourceString, startTag + endTag);

            newString = newString.Replace("{}", "");

            return newString;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {

            List<string> lines = new List<string> { };

            for (int i = 0; i < InputBox.LineCount; i++)
            {
                String line = InputBox.GetLineText(i);
                if (line.Replace(" ", "") == "")
                    continue;
                lines.Add(line);
            }


            List<string> fileNames = GenerateFileNames(lines);

            FillCodeBox(lines, fileNames);
        }


        private void Border_MouseEnter(object sender, MouseEventArgs e)
        {
            GenerateButtonGradientStart.Color = Color.FromRgb(54, 127, 169);
            GenerateButtonGradientEnd.Color = Color.FromRgb(20, 70, 117);
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            GenerateButtonGradientStart.Color = Color.FromRgb(91, 195, 255);
            GenerateButtonGradientEnd.Color = Color.FromRgb(29, 115, 195);
        }


    }
}
