using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CheckBox = System.Windows.Controls.CheckBox;
using Cursors = System.Windows.Input.Cursors;
using MessageBox = System.Windows.MessageBox;

namespace DeleteByExtension
{
    public partial class MainWindow
    {
        private static readonly List<string> ExtensionCheckboxNames =
        [
            "Pdf", "Srt", "Jpg", "Mp3", "Html",
            "Txt", "Rar", "Docx", "Zip", "Pptx", "Vtt"
        ];

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await DeleteButton_ClickAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Operation failed: {ex.Message}");
            }
        }

        private async Task DeleteButton_ClickAsync()
        {
            string folderPath = FolderPathTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                MessageBox.Show("Please select a folder first.");
                return;
            }

            var selectedExtensions = GetSelectedExtensions();
            if (!TryAddCustomExtension(selectedExtensions)) return;
            if (selectedExtensions.Count == 0)
            {
                MessageBox.Show("Please select at least one extension.");
                return;
            }

            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                bool deleteEmptyFolders = DeleteEmptyFoldersCheckBox.IsChecked == true;
                var result = await Task.Run(() => 
                    DeleteFilesAsync(folderPath, selectedExtensions, deleteEmptyFolders));

                ShowDeleteSummary(result.deletedFiles, result.emptyFoldersCount);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private bool TryAddCustomExtension(ICollection<string> extensions)
        {
            var customExt = CustomExtensionTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(customExt)) return true;

            if (!customExt.StartsWith(".")) customExt = "." + customExt;
            if (customExt.Length < 2)
            {
                MessageBox.Show("Custom extension must be at least 1 character.");
                return false;
            }

            extensions.Add(customExt);
            return true;
        }

        private List<string> GetSelectedExtensions()
        {
            return ExtensionCheckboxNames
                .Where(name => 
                    FindName($"{name}CheckBox") is CheckBox { IsChecked: true })
                .Select(name => "." + name.ToLower())
                .ToList();
        }

        private (Dictionary<string, int> deletedFiles, int emptyFoldersCount) 
            DeleteFilesAsync(string folderPath, List<string> extensions, bool deleteEmptyFolders)
        {
            var deletedFiles = new Dictionary<string, int>();
            foreach (var ext in extensions) deletedFiles[ext] = 0;

            // File deletion
            foreach (var extension in extensions)
            {
                try
                {
                    var files = Directory.EnumerateFiles(folderPath, $"*{extension}", SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        try
                        {
                            File.Delete(file);
                            deletedFiles[extension]++;
                        }
                        catch { /* Skip locked files */ }
                    }
                }
                catch { /* Skip inaccessible directories */ }
            }

            // Empty folder deletion
            int emptyFoldersCount = 0;
            if (deleteEmptyFolders)
            {
                try
                {
                    emptyFoldersCount = DeleteEmptyFoldersRecursive(folderPath);
                }
                catch { /* Ignore folder deletion errors */ }
            }

            return (deletedFiles, emptyFoldersCount);
        }

        private int DeleteEmptyFoldersRecursive(string path)
        {
            int deletedCount = 0;
            foreach (var directory in Directory.GetDirectories(path))
            {
                try
                {
                    deletedCount += DeleteEmptyFoldersRecursive(directory);
                }
                catch { /* Continue processing other folders */ }
            }

            try
            {
                if (!Directory.EnumerateFileSystemEntries(path).Any())
                {
                    Directory.Delete(path);
                    deletedCount++;
                }
            }
            catch { /* Ignore deletion errors */ }

            return deletedCount;
        }

        private void ShowDeleteSummary(Dictionary<string, int> deletedFiles, int emptyFolders)
        {
            var summary = new System.Text.StringBuilder("Deleted files summary:\n");
            foreach (var (extension, count) in deletedFiles)
            {
                summary.AppendLine($"{extension}: {count} files");
            }
            summary.Append($"Deleted empty folders: {emptyFolders}");
            MessageBox.Show(summary.ToString());
        }

        private void SetAllExtensionCheckboxes(bool isChecked)
        {
            foreach (var name in ExtensionCheckboxNames)
            {
                if (FindName($"{name}CheckBox") is CheckBox checkbox)
                {
                    checkbox.IsChecked = isChecked;
                }
            }
        }

        private void CheckAllClick(object sender, RoutedEventArgs e) => SetAllExtensionCheckboxes(true);
        private void UncheckAllClick(object sender, RoutedEventArgs e) => SetAllExtensionCheckboxes(false);

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new FolderBrowserDialog { Description = "Select folder" };
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FolderPathTextBox.Text = dialog.SelectedPath;
            }
        }
    }
}