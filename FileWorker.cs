namespace DeleteByExtensions;

public class FileWorker
{
    int deletedFoldersCount = 0;

    public string ReadPath()
    {
        string folderPath = Console.ReadLine().Trim();
        if (String.IsNullOrWhiteSpace(folderPath))
            return "";

        string outputString = new string(folderPath.Skip(1).Take(folderPath.Length - 2).ToArray());
        return outputString;
    }

    public void DeleteFiles(string folderPath)
    {
        int deletedFiles = 0;

        string[] extensions = { ".pdf", ".srt", ".jpg", ".mp3", ".html", ".txt", ".rar", ".docx", ".zip", ".pptx" };

        foreach (var extension in extensions)
        {
            string[] filesToDelete = Directory.GetFiles($"{folderPath}", $"*{extension}", SearchOption.AllDirectories);

            Console.WriteLine($"Found {filesToDelete.Length} file(s) with the extension {extension}.");

            foreach (string fileToDelete in filesToDelete)
            {
                File.Delete(fileToDelete);
                deletedFiles++;
            }
        }

        Console.WriteLine($"Deleted files: {deletedFiles}");
    }

    public void DeleteEmptyFolders(string targetDirectory)
    {
        foreach (var directory in Directory.GetDirectories(targetDirectory))
        {
            var s = directory;
            DeleteEmptyFolders(directory);

            if (Directory.GetFiles(directory).Length == 0 && Directory.GetDirectories(directory).Length == 0)
            {
                Directory.Delete(directory, false);
                deletedFoldersCount++;
            }
        }
    }

    public void ShowDeletedEmptyFolders()
    { 
        Console.WriteLine($"Deleted empty folders: {deletedFoldersCount}");
    }
}
