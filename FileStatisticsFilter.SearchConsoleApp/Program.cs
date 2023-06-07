using FileStatisticsFilter.CommonLibrary;

namespace FileStatisticsFilter.SearchConsoleApp
{
    internal class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">File directory</param>
        /// <param name="output">Csv file to output in</param>
        /// <param name="regex">Optional - if mathes regex</param>
        /// <param name="recursive">Optional - if true, searches also subdirectories</param>
        static void Main(DirectoryInfo input, FileInfo output, string? regex = null, bool recursive = false)
        {
            //Console.WriteLine(input.ToString());
            //Console.WriteLine(output.ToString());
            //Console.WriteLine(regex!);
            //Console.WriteLine(recursive.ToString());

            if (!input.Exists)
            {
                Console.Error.WriteLine("Input file does not exist!");
                return;
            }

            if (!output.Exists)
            {
                Console.Error.WriteLine("Output file does not exist!");
                return;
            }

            SaveFilesToCsv(input, output, regex, recursive);
        }

        private static void SaveFilesToCsv(DirectoryInfo input, FileInfo output, string? regex = null, bool recursive = false)
        {
            DirectorySearch(input.FullName, recursive, output);
            Console.WriteLine("Successfully saved to csv file.");
        }

        private static void DirectorySearch(string dir, bool recursive, FileInfo output)
        {
            List<FileInfo> fileInfos = new();

            try
            {
                foreach (var filePath in Directory.GetFiles(dir))
                {
                    FileInfo fi = new(filePath);
                    fileInfos.Add(fi);
                }

                SearchedFiles sf = new(fileInfos);
                sf.SaveToCsv(output);

                if (recursive)
                {
                    foreach (var directory in Directory.GetDirectories(dir))
                    {
                        DirectorySearch(directory, recursive, output);
                    }
                }
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Error when saving data to csv file!");
            }
        }
    }
}