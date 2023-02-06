namespace org.antlr.v4.test.runtime;

public static class FileUtils
{
    public static void WriteFile(string dir, string fileName, string content)
    {
        try
        {
            File.WriteAllText(Path.Combine(dir, fileName), content);
        }
        catch (IOException ioe)
        {
            Console.Error.WriteLine("can't write file:" + ioe.Message);
        }
    }

    public static string ReadFile(string dir, string fileName)
    {
        try
        {
            return File.ReadAllText(Path.Combine(dir, fileName));
        }
        catch (IOException ioe)
        {
            Console.Error.WriteLine("can't write file:" + ioe.Message);
        }
        return null;
    }

    public static void ReplaceInFile(string sourcePath, String target, String replacement)
    {
        ReplaceInFile(sourcePath, sourcePath, target, replacement);
    }

    public static void ReplaceInFile(string sourcePath, string destPath, String target, String replacement)
    {
        var content = File.ReadAllText(sourcePath);
        var newContent = content.Replace(target, replacement);
        File.WriteAllText(destPath, newContent);
    }

    public static void MakeDirectory(String dir)
    {
        Directory.CreateDirectory(dir);
    }

    public static void DeleteDirectory(string f)
    {
        if (Directory.Exists(f))
        {
            Directory.Delete(f, true);
        }
    }
}
