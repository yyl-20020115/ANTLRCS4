namespace org.antlr.v4.test.runtime;

public static class FileUtils
{
    public static void writeFile(String dir, String fileName, String content)
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

    public static String readFile(String dir, String fileName)
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

    public static void replaceInFile(string sourcePath, String target, String replacement)
    {
        replaceInFile(sourcePath, sourcePath, target, replacement);
    }

    public static void replaceInFile(string sourcePath, string destPath, String target, String replacement)
    {
        String content = File.ReadAllText(sourcePath);
        String newContent = content.Replace(target, replacement);
        File.WriteAllText(destPath, newContent);  
    }

    public static void mkdir(String dir)
    {
        Directory.CreateDirectory(dir);
    }

    public static void deleteDirectory(string f)
    {
        if (Directory.Exists(f))
        {
            Directory.Delete(f, true);
        }
    }
}
