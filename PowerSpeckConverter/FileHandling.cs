using System;
using System.IO;

namespace PowerSpeckConverter
{
    static internal class FileHandling
    {
        internal static void ConvertFile(string file)
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(file);

            switch (Path.GetExtension(file))
            {
                case ".pptx":
                case ".ppt":
                    PowerPointHandling.ConvertToPowerSpeckPlayer(file, FindAlternative(file,"cfg"));
                    break;

                case ".cfg":
                case ".ini":
                    PowerPointHandling.ConvertToPowerpoint(file, FindAlternative(file,"pptx"));
                    break;
            }
        }

        internal static string FindAlternative(string file, string ext, bool isDirectory =false)
        {
            var n = 0;
            String alt, path = isDirectory ? file:Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file));

            do
            {
                alt = path + (n++ > 0 ? " (" + (n-1) + ")" : "") + (isDirectory?"":".") + ext;
            } while (File.Exists(alt) || Directory.Exists(alt));

            return alt;
        }
    }
}