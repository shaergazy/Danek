namespace Danek.BLL.Extesions;

public static class StringLikeDirectoryExtension
{
    public static void CreateDirectoryIfNotExist(this string dir)
    {
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
    }

    public static void DeleteDirectoryIfExist(this string dir)
    {
        if (Directory.Exists(dir))
            Directory.Delete(dir, true);
    }

    public static void DeleteDirectoryFiles(this string dir)
    {
        dir.DeleteDirectoryIfExist();
        dir.CreateDirectoryIfNotExist();
    }

    public static void MoveFilesToDirectory(this string destDir, IEnumerable<string> paths)
    {
        foreach (var x in paths)
            File.Move(x, $"{destDir}\\{Path.GetFileName(x)}");
    }


    public static void CopyDirectoryFiles(this string sourceDir, string destDir)
    {
        foreach (var x in sourceDir.DirectoryFiles())
        {
            var dest = $"{destDir}\\{Path.GetFileName(x)}";
            File.Copy(x, dest);
            File.SetCreationTime(dest, x.CreationTime());
        }
    }

    public static IList<string> DirectoryFiles(this string dir)
    {
        return !Directory.Exists(dir) ? new string[] { } : Directory.GetFiles(dir);
    }

    public static bool DirectoryIsEmpty(this string dir)
    {
        return !Directory.EnumerateFileSystemEntries(dir).Any();
    }
}
