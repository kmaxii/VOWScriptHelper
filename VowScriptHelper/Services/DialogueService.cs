using System;
using VowScriptHelper.Interfaces;

namespace VowScriptHelper.Services
{
    public class DialogueService : IDialogueHandler
    {
        public void test()
        {
            Console.WriteLine("hi");
        }

        /// <summary>
        /// Extracts the speaker name from a dialogue line in "tasim: hi" format.
        /// </summary>
        public string GetName(string line)
        {
            // Character-change note: "some line /$ newcharacter"
            int dollarSlashIndex = line.IndexOf("/$");
            if (dollarSlashIndex != -1)
                return line.Substring(dollarSlashIndex + 2).Trim();

            int colon = line.IndexOf(':');
            if (colon > 0)
                return line.Substring(0, colon).Trim();

            return "unknown";
        }
    }
}
