using DevelopsTodayTask.Models;
using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        // Get the directory of the currently executing program (the bin folder)
        string projectDir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        // Combine the project directory with the relative path to the CSV file
        string csvFilePath = Path.Combine(projectDir, "sample-cab-data.csv");

        var importer = new CsvProcessor();
        importer.ImportCsvToDatabase(csvFilePath);
    }
}
