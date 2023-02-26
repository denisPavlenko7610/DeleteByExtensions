Console.WriteLine("Enter the path to the folder:");

string folderPath = Console.ReadLine();
int deletedFiles = 0;

string[] extensions = { ".pdf", ".srt", ".jpg" };
string outputString = new string(folderPath.Skip(1).Take(folderPath.Length-2).ToArray());


foreach (var extension in extensions)
{
    string[] filesToDelete = Directory.GetFiles($"{outputString}", $"*{extension}", SearchOption.AllDirectories);

    Console.WriteLine($"Found {filesToDelete.Length} file(s) with the extension {extension}.");

    foreach (string fileToDelete in filesToDelete)
    {
        File.Delete(fileToDelete);
        deletedFiles++;
    }
}

Console.WriteLine($"Deleted files: {deletedFiles}");
