

using System.IO;

public static class DeleteSaveFile
{
    public static void Delete(string fileName, string profile, string fileDirectory)
    {
        string fullPath = Path.Combine(fileDirectory, profile, fileName);
        
        File.Delete(fullPath);
    }
}