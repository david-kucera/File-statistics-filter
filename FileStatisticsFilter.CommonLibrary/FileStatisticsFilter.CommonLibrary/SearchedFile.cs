using System.Globalization;

namespace FileStatisticsFilter.CommonLibrary
{
    public class SearchedFile
    {
        public DateTime CreatedTime { get; set; }
        public string Directory { get; set; }
        public string Extension { get; set; }
        public string FileName { get; } = null!;
        public string FileNameWithoutExtention { get; set; }
        public string FullName { get; } = null!;
        public bool IsReadOnly { get; set; }
        public DateTime ModifiedTime { get; set; }
        public long Size { get; set; }
        public string SizeReadable => GetSizeReadable(Size)!;


        public SearchedFile(FileInfo file)
        {
            CreatedTime = file.CreationTime;
            ModifiedTime = file.LastWriteTime;
            IsReadOnly = file.IsReadOnly;
            Directory = file.DirectoryName!;
            Extension = file.Extension;
            FileNameWithoutExtention = file.Name.Remove(file.Name.IndexOf('.'));

            FileName = FileNameWithoutExtention + Extension;

            FullName = Directory + FileNameWithoutExtention;
            Size = file.Length;
        }

        public static string? GetSizeReadable(long i)
        {
            // Get absolute value
            long absolute_i = (i < 0 ? -i : i);
            // Determine the suffix and readable value
            string suffix;
            double readable;
            if (absolute_i >= 0x1000000000000000) // Exabyte
            {
                suffix = "EB";
                readable = (i >> 50);
            }
            else if (absolute_i >= 0x4000000000000) // Petabyte
            {
                suffix = "PB";
                readable = (i >> 40);
            }
            else if (absolute_i >= 0x10000000000) // Terabyte
            {
                suffix = "TB";
                readable = (i >> 30);
            }
            else if (absolute_i >= 0x40000000) // Gigabyte
            {
                suffix = "GB";
                readable = (i >> 20);
            }
            else if (absolute_i >= 0x100000) // Megabyte
            {
                suffix = "MB";
                readable = (i >> 10);
            }
            else if (absolute_i >= 0x400) // Kilobyte
            {
                suffix = "KB";
                readable = i;
            }
            else
            {
                return i.ToString("0 B"); // Byte
            }
            // Divide by 1024 to get fractional value
            readable = (readable / 1024);
            // Return formatted number with suffix
            return readable.ToString("0.### ") + suffix;
        }

        public SearchedFile(string csvLine, char delimiter = '\t')
        {
            var data = csvLine.Split(delimiter);

            Directory = data[0];
            FileNameWithoutExtention = data[1];
            Extension = data[2];
            Size = long.Parse(data[3]);
            CreatedTime = DateTime.Parse(data[4]);
            ModifiedTime = DateTime.Parse(data[5]);
            IsReadOnly = bool.Parse(data[6]);
        }

        public static string ToCsvHeaderLine(char delimiter = '\t')
        {
            string[] columns =
            {
                "Directory", "FileNameWithoutExtention", "Extention", "Size", "CreatedTime", "ModifiedTime",
                "IsReadOnly"
            };

            return columns.Aggregate("", (current, column) => current + (column + delimiter));
        }

        public string ToCsvLine(char delimiter = '\t')
        {
            var data = new string[7];

            data[0] = Directory;
            data[1] = FileNameWithoutExtention;
            data[2] = Extension;
            data[3] = Size.ToString();
            data[4] = CreatedTime.ToString(CultureInfo.CurrentCulture);
            data[5] = ModifiedTime.ToString(CultureInfo.CurrentCulture);
            data[6] = IsReadOnly.ToString(CultureInfo.CurrentCulture);

            return data.Aggregate("", (current, t) => current + (t + delimiter));
        }

    }
}