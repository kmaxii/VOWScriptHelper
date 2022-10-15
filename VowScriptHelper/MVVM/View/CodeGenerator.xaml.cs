using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using static System.Net.WebRequestMethods;
using File = System.IO.File;
using Path = System.IO.Path;

namespace VowScriptHelper.MVVM.View
{
    /// <summary>
    /// Interaction logic for CodeGenerator.xaml
    /// </summary>
    public partial class CodeGenerator : UserControl
    {
        public CodeGenerator()
        {
            InitializeComponent();
        }

        private void FileDropStackPanel_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            FillBoxes((string[])e.Data.GetData(DataFormats.FileDrop);
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

            List<string> fileNames = GenerateFileNames(lines);

            FillCodeBox(lines, fileNames);
            FillJSONBox(fileNames);
        }

        private void FillCodeBox(List<string> lines, List<string> fileNames)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for(int i = 0; i < lines.Count; i++)
            {
                stringBuilder.AppendLine(GetCodeLine(lines[i], fileNames[i]));
            }
            CodeOutputBox.Text = stringBuilder.ToString();
        }

        private void FillJSONBox(List<string> fileNames)
        {

            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < fileNames.Count; i++)
            {

                string str = fileNames[i];
                JsonOutputBox.Text +=
                    "  \"" + str + "\": {\n" +
                    "    \"category\": \"voice\",\n" +
                    "    \"sounds\": [{ \"name\": \"wynnvp:" + str + "\", \"stream\": true}]\n" +
                    "  },\n";

                //stringBuilder.AppendLine(GetCodeLine(lines[i], fileNames[i]));
            }
        //    JsonOutputBox.Text = stringBuilder.ToString();

           
        }

        private static string GetCodeLine(string line, string fileName)
        {
            string outPut = "s.addSound(\"";
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


            for(int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                string name = GetName(line).ToLower();
                //Remove empty chars
                name = name.Replace(" ", "");

                name = questName + "-" + name;

                int dialogueNumber = 1;
                foreach(string fileName in fileNames)
                {
                    if (fileName.Contains(name))
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


            if (!line.StartsWith("[")){
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

    }
}
