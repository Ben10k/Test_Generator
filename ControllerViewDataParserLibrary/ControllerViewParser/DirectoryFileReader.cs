using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ControllerViewDataParserLibrary.ControllerViewParser {
    public static class DirectoryFileReader {
        private static List<string> ReadDirectory(string directory, string fileExtension) {
            return Directory.GetFiles(directory, "*." + fileExtension).ToList();
        }

        public static List<string> ReadControllerClasses(string directory) {
            return ReadDirectory(directory + "Controllers", "cs");
        }

        public static List<string> ReadViewClasses(string directory) {
            var list = new List<string>();
            DirSearch(directory + "Views", list, "cshtml");
            return list;
        }

        private static void DirSearch(string rootDirectory, List<string> list, string fileExtension) {
            try {
                list.AddRange(ReadDirectory(rootDirectory, fileExtension));
                foreach (var directory in Directory.GetDirectories(rootDirectory))
                    DirSearch(directory, list, fileExtension);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}