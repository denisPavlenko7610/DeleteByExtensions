namespace DeleteByExtensions
{
    public class FileWorker
    {
        int deletedFoldersCount = 0;

        public string ReadPath()
        {
            string folderPath = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(folderPath))
                return "";

            return folderPath;
        }

        public void DeleteFiles(string folderPath)
        {
            int deletedFiles = 0;

            string[] extensions = { ".pdf", ".srt", ".jpg", ".mp3", ".html", ".txt", ".rar", ".docx", ".zip", ".pptx"};

            try
            {
                foreach (var extension in extensions)
                {
                    string[] filesToDelete = Directory.GetFiles(folderPath, $"*{extension}", SearchOption.AllDirectories);

                    Console.WriteLine($"Found {filesToDelete.Length} file(s) with the extension {extension}.");

                    foreach (string fileToDelete in filesToDelete)
                    {
                        File.Delete(fileToDelete);
                        deletedFiles++;
                    }
                }

                Console.WriteLine($"Deleted files: {deletedFiles}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting files: {ex.Message}");
            }
        }

        public void DeleteEmptyFolders(string targetDirectory)
        {
            try
            {
                foreach (var directory in Directory.GetDirectories(targetDirectory))
                {
                    DeleteEmptyFolders(directory);

                    if (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0)
                    {
                        Directory.Delete(directory, false);
                        deletedFoldersCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting folders: {ex.Message}");
            }
        }

        public void ShowDeletedEmptyFolders()
        {
            Console.WriteLine($"Deleted empty folders: {deletedFoldersCount}");
        }
    }
}
