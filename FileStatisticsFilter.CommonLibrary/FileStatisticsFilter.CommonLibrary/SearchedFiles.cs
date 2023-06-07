namespace FileStatisticsFilter.CommonLibrary
{
    public class SearchedFiles
    {
        public SearchedFile[] Files { get; set; } = null!;

        public SearchedFiles()
        {
            
        }

        public SearchedFiles(IEnumerable<FileInfo> files)
        {
            Files = files.Select(file => new SearchedFile(file)).ToArray();
        }

        public void LoadFromCsv(FileInfo file)
        {
            using var sr = new StreamReader(file.FullName);
            var list = new List<SearchedFile>();

            sr.ReadLine(); // Skip first line

            while (sr.ReadLine() is { } currentLine)
            {
                SearchedFile sf = new(currentLine, '\t');
                list.Add(sf);
            }

            Files = list.ToArray();
        }

        public void SaveToCsv(FileInfo file)
        {
            if (File.ReadAllText(file.FullName).Equals(string.Empty))
            {
                File.WriteAllText(file.FullName, SearchedFile.ToCsvHeaderLine());
            }

            foreach (var fi in Files)
            {
                File.AppendAllText(file.FullName, fi.ToCsvLine() + '\n');
            }
        }
    }
}
