using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
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
using FileStatisticsFilter.CommonLibrary;
using Microsoft.Win32;

namespace FileStatisticsFilter.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SearchedFiles _Files = new();
        private List<SearchedFile> _filesInDataGrid = new();
        private string _directory;
        private bool _includeSubDirectories;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Open(object sender, RoutedEventArgs e)
        {
            var fileDialog= new OpenFileDialog();
            fileDialog.InitialDirectory = "D:\\Workspace\\FileStatistiscFilter\\FileStatisticsFilter.SearchConsoleApp";
            //fileDialog.InitialDirectory = "C:\\";
            fileDialog.Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*";
            fileDialog.ShowDialog();

            if (!fileDialog.FileName.Any()) return;
            var filePath = fileDialog.FileName;
            FileInfo fi = new(filePath);
            _Files.LoadFromCsv(fi);

            foreach (var file in _Files.Files)
            {
                _filesInDataGrid.Add(file);
            }

            AddDirectories();

            RefreshDataGrid();
            RefreshStats();
            RefreshStatsGrid();
        }

        private void RefreshStatsGrid()
        {
            DataGridStats.Items.Clear();

            List<string> extentions = new();

            foreach (var file in _filesInDataGrid)
            {
                if (!extentions.Contains(file.Extension))
                {
                    extentions.Add(file.Extension);
                }
            }

            List<int> counts = new();

            List<long> size = new();

            foreach (var extention in extentions)
            {
                var count = 0;
                long tSize = 0;
                foreach (var file in _filesInDataGrid)
                {
                    if (file.Extension.Equals(extention))
                    {
                        count++;
                        tSize += file.Size;
                    }
                }
                counts.Add(count);
                size.Add(tSize);
            }
            

            List<string> sizes = new();

            foreach (var l in size)
            {
                sizes.Add(SearchedFile.GetSizeReadable(l));
            }

            for (int i = 0; i < extentions.Count; i++)
            {
                var row = new { Name = extentions[i], Count = counts[i], TotalSize = sizes[i] };
                DataGridStats.Items.Add(row);
            }
        }

        private void RefreshStats()
        {
            FilesCount.Content = DataGrid.Items.Count + " / " + _Files.Files.Length;
            DirectoriesCount.Content = GetDirectoriesCount();
            TotalSizeCount.Content = GetTotalSize();
            CreatedTimeOldest.Content = GetCreatedTimeOldest();
            CreatedTimeNewest.Content = GetCreatedTimeNewest();
            ModifiedTimeOldest.Content = GetModifiedTimeOldest();
            ModifiedTimeNewest.Content = GetModifiedTimeNewest();
            ReadonlyFilesCount.Content = GetReadOnly();
        }

        private string GetDirectoriesCount()
        {
            List<string> dirs = new();

            foreach (var searchedFile in _filesInDataGrid)
            {
                dirs.Add(searchedFile.Directory);
            }

            var directories = dirs.Distinct().ToList();

            return directories.Count + " / " + Directories.Items.Count;
        }

        private string GetModifiedTimeNewest()
        {
            var newest = _filesInDataGrid[0];

            foreach (var file in _filesInDataGrid)
            {
                if (file.ModifiedTime > newest.ModifiedTime)
                {
                    newest = file;
                }
            }

            return newest.ModifiedTime.ToString(CultureInfo.CurrentCulture);
        }

        private string GetModifiedTimeOldest()
        {
            var oldest = _filesInDataGrid[0];

            foreach (var file in _filesInDataGrid)
            {
                if (file.ModifiedTime < oldest.ModifiedTime)
                {
                    oldest = file;
                }
            }

            return oldest.ModifiedTime.ToString(CultureInfo.CurrentCulture);
        }

        private string GetCreatedTimeNewest()
        {
            var newest = _filesInDataGrid[0];

            foreach (var file in _filesInDataGrid)
            {
                if (file.CreatedTime > newest.CreatedTime)
                {
                    newest = file;
                }
            }

            return newest.CreatedTime.ToString(CultureInfo.CurrentCulture);
        }

        private string GetCreatedTimeOldest()
        {
            var oldest = _filesInDataGrid[0];

            foreach (var file in _Files.Files)
            {
                if (file.CreatedTime < oldest.CreatedTime)
                {
                    oldest = file;
                }
            }

            return oldest.CreatedTime.ToString(CultureInfo.CurrentCulture);
        }

        private int GetReadOnly()
        {
            var count = 0;

            foreach (var file in _filesInDataGrid)
            {
                if (file.IsReadOnly)
                {
                    count++;
                }
            }

            return count;
        }

        private string GetTotalSize()
        {
            long totalSize = 0;
            foreach (var file in _filesInDataGrid)
            {
                totalSize += file.Size;
            }

            return SearchedFile.GetSizeReadable(totalSize)!;
        }

        private void RefreshDataGrid()
        {
            DataGrid.Items.Clear();

            GetFilesInDataGrid();

            foreach (var data in _filesInDataGrid)
            {
                DataGrid.Items.Add(data);
            }

            RefreshStats();
            RefreshStatsGrid();
        }

        private void GetFilesInDataGrid()
        {
            _filesInDataGrid.Clear();

            if (_includeSubDirectories)
            {
                foreach (var file in _Files.Files)
                {
                    if (file.Directory.Contains(_directory))
                    {
                        _filesInDataGrid.Add(file);
                    }
                }
            }
            else
            {
                foreach (var file in _Files.Files)
                {
                    if (file.Directory.Equals(_directory))
                    {
                        _filesInDataGrid.Add(file);
                    }
                }
            }
        }

        private void AddDirectories()
        {
            List<string> dirs = new();

            foreach (var searchedFile in _Files.Files)
            {
                dirs.Add(searchedFile.Directory);
            }

            var directories = dirs.Distinct().ToList();

            foreach (var directory in directories)
            {
                Directories.Items.Add(directory);
            }

            Directories.SelectedIndex = 0;
        }

        private void IncludeSubdirectoriesChanged(object sender, RoutedEventArgs e)
        {
            switch (IncludeSubdirectories.IsChecked)
            {
                case false:
                    _includeSubDirectories = false;
                    break;
                case true:
                    _includeSubDirectories = true;
                    break;
            }

            if (_Files.Files == null)
            {
                return;
            }
            RefreshDataGrid();
        }

        private void DirectoryChanged(object sender, SelectionChangedEventArgs e)
        {
            _directory = Directories.SelectedItem.ToString()!;
            RefreshDataGrid();
        }
    }
}
