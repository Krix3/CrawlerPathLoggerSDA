using System;
using System.Collections.Generic;
using System.IO;

public class CrawlerPathLogger
{
    private List<string> DirsArr { get; set; } = new List<string>();
    private string FilePathLog { get; set; }
    private string LogicDiskLetter { get; set; }

    public CrawlerPathLogger(string filePathLog, string logicDiskLetter)
    {
        FilePathLog = filePathLog;
        LogicDiskLetter = logicDiskLetter;
    }

    public void SetFilePathLog(string filePathLog)
    {
        FilePathLog = filePathLog;
    }

    public string GetFilePathLog()
    {
        return FilePathLog;
    }

    public void SetLogicDiskLetter(string logicDiskLetter)
    {
        LogicDiskLetter = logicDiskLetter;
    }

    public string GetLogicDiskLetter()
    {
        return LogicDiskLetter;
    }

    public void Crawl()
    {
        DirsArr.Clear();
        CrawlDirectory(LogicDiskLetter);
    }

    private void CrawlDirectory(string currentDirectory)
    {
        try
        {
            string[] directories = Directory.GetDirectories(currentDirectory);
            foreach (string directory in directories)
            {
                DirsArr.Add(directory);
                CrawlDirectory(directory);
            }

            string[] files = Directory.GetFiles(currentDirectory);
            foreach (string file in files)
            {
                DirsArr.Add(file);
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Handle the case where the application does not have access to the directory
            Console.WriteLine($"Access denied to directory: {currentDirectory}");
        }
    }

    public void WriteLog()
    {
        using (StreamWriter writer = new StreamWriter(FilePathLog, false))
        {
            foreach (string path in DirsArr)
            {
                if (Directory.Exists(path))
                {
                    var dirInfo = new DirectoryInfo(path);
                    writer.WriteLine($"{dirInfo.CreationTime}, {dirInfo.Name}");
                    foreach (var file in dirInfo.GetFiles())
                    {
                        writer.WriteLine($"    -> {file.CreationTime}, {file.Name}, {file.Extension}");
                    }
                    foreach (var subDir in dirInfo.GetDirectories())
                    {
                        writer.WriteLine($"    -> {subDir.CreationTime}, {subDir.Name}");
                    }
                }
                else if (File.Exists(path))
                {
                    var fileInfo = new FileInfo(path);
                    writer.WriteLine($"    -> {fileInfo.CreationTime}, {fileInfo.Name}, {fileInfo.Extension}");
                }
            }
        }
    }

    public void ReadLog()
    {
        if (File.Exists(FilePathLog))
        {
            using (StreamReader reader = new StreamReader(FilePathLog))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }
        else
        {
            Console.WriteLine("Log file not found.");
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Введите путь к файлу для лога:");
        string filePathLog = Console.ReadLine();

        Console.WriteLine("Введите букву логического диска (например, C:\\):");
        string logicDiskLetter = Console.ReadLine();

        CrawlerPathLogger crawler = new CrawlerPathLogger(filePathLog, logicDiskLetter);

        Console.WriteLine("Начинается сканирование директорий...");
        crawler.Crawl();

        Console.WriteLine("Запись данных в лог-файл...");
        crawler.WriteLog();

        Console.WriteLine("Чтение данных из лог-файла...");
        crawler.ReadLog();

        Console.WriteLine("Работа завершена.");
    }
}
