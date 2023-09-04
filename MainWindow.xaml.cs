using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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

            Dictionary<string, int> deletedFilesCount = DeleteFiles(folderPath, selectedExtensions, out int deleteEmptyFoldersCount);

            ShowDeleteSummary(deletedFilesCount, deleteEmptyFoldersCount);
        }

        private string[] GetSelectedExtensions()
        {
            List<string> selectedExtensions = new List<string>();

            string[] checkboxNames = { "PdfCheckBox", "SrtCheckBox", "JpgCheckBox", "Mp3CheckBox", "HtmlCheckBox",
                               "TxtCheckBox", "RarCheckBox", "DocxCheckBox", "ZipCheckBox", "PptxCheckBox", "VttCheckBox" };

            foreach (string checkboxName in checkboxNames)
            {
                System.Windows.Controls.CheckBox? checkbox = FindName(checkboxName)
                    as System.Windows.Controls.CheckBox;
                if (checkbox != null && checkbox.IsChecked == true)
                {
                    selectedExtensions.Add("." + checkboxName.ToLower().Replace("checkbox", ""));
                }
            }

            return selectedExtensions.ToArray();
        }

        private Dictionary<string, int> DeleteFiles(string folderPath, string[] extensions, out int deleteEmptyFoldersCount)
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


                deleteEmptyFoldersCount = 0;
                
                if (DeleteEmptyFoldersCheckBox.IsChecked == true)
                    DeleteEmptyFolders(folderPath);

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
                deleteEmptyFoldersCount = 0;
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

        private void ShowDeleteSummary(Dictionary<string, int> deletedFilesCount, int emptyFoldersCount)
        {
            string summary = "Deleted files summary:\n";

            foreach (var kvp in deletedFilesCount)
            {
                summary += $"{kvp.Key} - {kvp.Value} files\n";
            }

            summary += "Deleted empty folders - " + emptyFoldersCount + " folders";

            System.Windows.MessageBox.Show(summary);
        }

        private void SetAllCheckboxes(bool isChecked)
        {
            foreach (UIElement element in ((Grid)Content).Children)
            {
                if (element is System.Windows.Controls.CheckBox checkbox)
                {
                    checkbox.IsChecked = isChecked;
                }
            }
        }

        private void CheckAllClick(object sender, RoutedEventArgs e)
        {
            SetAllCheckboxes(true);
        }

        private void UncheckAllClick(object sender, RoutedEventArgs e)
        {
            SetAllCheckboxes(false);
        }
    }
}
