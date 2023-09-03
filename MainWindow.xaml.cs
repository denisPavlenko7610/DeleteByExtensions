using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace FileDeletionApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var folderDialog = new FolderBrowserDialog
            {
                Description = "Select a folder"
            };

            DialogResult result = folderDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                FolderPathTextBox.Text = folderDialog.SelectedPath;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = FolderPathTextBox.Text;

            if (string.IsNullOrWhiteSpace(folderPath))
            {
                System.Windows.MessageBox.Show("Please enter a folder path.");
                return;
            }

            string[] selectedExtensions = GetSelectedExtensions();

            if (selectedExtensions.Length == 0)
            {
                System.Windows.MessageBox.Show("Please select at least one extension.");
                return;
            }

            Dictionary<string, int> deletedFilesCount = DeleteFiles(folderPath, selectedExtensions);

            ShowDeleteSummary(deletedFilesCount);
        }

        private string[] GetSelectedExtensions()
        {
            List<string> selectedExtensions = new List<string>();

            if (PdfCheckBox.IsChecked == true)
                selectedExtensions.Add(".pdf");

            if (SrtCheckBox.IsChecked == true)
                selectedExtensions.Add(".srt");

            if (JpgCheckBox.IsChecked == true)
                selectedExtensions.Add(".jpg");

            if (Mp3CheckBox.IsChecked == true)
                selectedExtensions.Add(".mp3");

            if (HtmlCheckBox.IsChecked == true)
                selectedExtensions.Add(".html");

            if (TxtCheckBox.IsChecked == true)
                selectedExtensions.Add(".txt");

            if (RarCheckBox.IsChecked == true)
                selectedExtensions.Add(".rar");

            if (DocxCheckBox.IsChecked == true)
                selectedExtensions.Add(".docx");

            if (ZipCheckBox.IsChecked == true)
                selectedExtensions.Add(".zip");

            if (PptxCheckBox.IsChecked == true)
                selectedExtensions.Add(".pptx");

            // Add checkboxes for other extensions here

            return selectedExtensions.ToArray();
        }

        private Dictionary<string, int> DeleteFiles(string folderPath, string[] extensions)
        {
            Dictionary<string, int> deletedFilesCount = new Dictionary<string, int>();

            try
            {
                foreach (var extension in extensions)
                {
                    var filesToDelete = Directory.GetFiles(folderPath, $"*{extension}", SearchOption.AllDirectories);

                    if (filesToDelete.Length > 0)
                    {
                        deletedFilesCount[extension] = filesToDelete.Length;

                        foreach (var file in filesToDelete)
                        {
                            File.Delete(file);
                        }
                    }
                }

                int deletedEmptyFoldersCount = DeleteEmptyFolders(folderPath);
                foreach (var extension in extensions)
                {
                    if (!deletedFilesCount.ContainsKey(extension))
                    {
                        deletedFilesCount[extension] = 0;
                    }
                }

                return deletedFilesCount;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"An error occurred: {ex.Message}");
                return null;
            }
        }

        private int DeleteEmptyFolders(string folderPath)
        {
            int deletedEmptyFoldersCount = 0;
            string[] subdirectories = Directory.GetDirectories(folderPath);

            foreach (string subdirectory in subdirectories)
            {
                int deletedCount = DeleteEmptyFolders(subdirectory);
                deletedEmptyFoldersCount += deletedCount;

                if (!Directory.EnumerateFileSystemEntries(subdirectory).Any())
                {
                    Directory.Delete(subdirectory);
                }
            }

            return deletedEmptyFoldersCount;
        }

        private void ShowDeleteSummary(Dictionary<string, int> deletedFilesCount)
        {
            string summary = "Deleted files summary:\n";

            foreach (var kvp in deletedFilesCount)
            {
                summary += $"{kvp.Key} - {kvp.Value} files\n";
            }

            summary += "Deleted empty folders - " + deletedFilesCount.Values.Sum() + " folders";

            System.Windows.MessageBox.Show(summary);
        }
    }
}
