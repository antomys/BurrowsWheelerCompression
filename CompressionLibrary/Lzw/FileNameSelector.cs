using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CompressionLibrary.Lzw
{
    public static class FileNameSelector
    {
        public static string GetFileName(string pFileName)
        {
            var newFileName = pFileName;
            if (!File.Exists(pFileName)) return newFileName;
            Console.WriteLine(pFileName + " already exists. Overwrite it?");
            var response = PromptUserYnq().ToLower();
                
            switch (response)
            {
                case "n":
                    newFileName = PromptUserFileName();
                    break;
                case "q":
                    return null;
            }

            return newFileName;
        }

        private static string PromptUserYnq()
        {
            var goodResponse = new Regex("[ynqYNQ]");
            string lineRead;
            var firstRun = true;

            do
            {
                if (!firstRun)
                {
                    Console.WriteLine("Entered invalid option, try again...");
                }

                Console.WriteLine("[Y]es, [N]o, [Q]uit");
                lineRead = Console.ReadLine();
                firstRun = false;
            } while (!goodResponse.IsMatch(lineRead));

            return lineRead;
        }

        private static string PromptUserFileName()
        {
            Console.WriteLine("Enter filename to use: ");
            return Console.ReadLine();
        }
    }
}
