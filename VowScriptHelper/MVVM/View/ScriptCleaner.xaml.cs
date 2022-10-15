using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace VowScriptHelper.MVVM.View
{
    /// <summary>
    /// Interaction logic for ScriptCleaner.xaml
    /// </summary>
    public partial class ScriptCleaner : UserControl
    {
        public ScriptCleaner()
        {
            InitializeComponent();
        }

        private void FileDropStackPanel_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;


            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);

                if (!fileName.EndsWith(".txt"))
                    continue;

                string path = Path.Combine(Path.GetDirectoryName(file), file);

                List<string> lines = new List<string>(System.IO.File.ReadAllLines(path));

                FormatLines(ref lines);


                var outPut = new System.Text.StringBuilder();

                for (int i= 0; i < lines.Count; i++)
                {
                    outPut.AppendLine(lines[i]);

                }

                OutputBox.Text = outPut.ToString();
            }

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

        private static string RemoveBetween(string sourceString, string startTag, string endTag)
        {
            Regex regex = new Regex(string.Format("{0}(.*?){1}", Regex.Escape(startTag), Regex.Escape(endTag)), RegexOptions.RightToLeft);
            string newString = regex.Replace(sourceString, startTag + endTag);

            newString = newString.Replace("{}", "");

            return newString;
        }

       
    }
}
