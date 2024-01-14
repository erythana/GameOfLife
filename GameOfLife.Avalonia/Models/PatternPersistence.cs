using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Exception = System.Exception;

namespace GameOfLife.Avalonia.Models;

public class PatternPersistence
{
    public void ExportCellsToFile(SaveableCells cellsToExport)
    {
        var encodedString = GetEncodedPattern(cellsToExport);
        try
        {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppInfo.ApplicationName, "patterns.txt");
            var fileInfo = new FileInfo(path);
            if (!fileInfo.Directory!.Exists)
                fileInfo.Directory.Create();
            File.AppendAllText(path, encodedString + Environment.NewLine);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);//do something
        }
    }

    public IEnumerable<SaveableCells> LoadCellsFromFile()
    {
        var cells = new List<SaveableCells>();
        
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppInfo.ApplicationName, "patterns.txt");
        try
        {
            var fileContent = File.ReadAllText(path);
            foreach (var line in fileContent.Split(Environment.NewLine))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                
                var fileBytes = Convert.FromBase64String(line);
                var decodedFile = System.Text.Encoding.UTF8.GetString(fileBytes);
                cells.Add(System.Text.Json.JsonSerializer.Deserialize<SaveableCells>(decodedFile)!);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return cells;
    }

    public bool DeleteCellsFromFile(SaveableCells cellsToExport)
    {
        var encodedString = GetEncodedPattern(cellsToExport);
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppInfo.ApplicationName, "patterns.txt");
        try
        {
            var modifiedFileContent = File.ReadAllText(path).Split(Environment.NewLine).Where(line => !string.IsNullOrWhiteSpace(line) && line != encodedString);
            File.WriteAllLines(path, modifiedFileContent);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private static string GetEncodedPattern(SaveableCells cellsToExport)
    {
        var cells = System.Text.Json.JsonSerializer.Serialize(cellsToExport);
        var bytes = System.Text.Encoding.UTF8.GetBytes(cells);
        return Convert.ToBase64String(bytes);
    }
}