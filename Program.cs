namespace DeleteByExtensions;
class Program
{
    public static void Main(string[] args)
    {
        ShowInitMessage();

        FileWorker fileWorker = Init();

        string outputString = fileWorker.ReadPath();

        fileWorker.DeleteFiles(outputString);
        fileWorker.DeleteEmptyFolders(outputString);
        fileWorker.ShowDeletedEmptyFolders();

        Console.ReadKey();
    }

    private static void ShowInitMessage()
    {
        Console.WriteLine("Enter the path to the folder:");
    }

    private static FileWorker Init()
    {
        return new FileWorker();
    }
}