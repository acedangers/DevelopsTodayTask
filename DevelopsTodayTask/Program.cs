using DevelopsTodayTask.Models;
using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: ETLApp <csv_file_path>");
            return;
        }

        string csvFilePath = args[0];
        List<CabTripRecord> trips = CsvProcessor.ReadCsv(csvFilePath);

        using (var db = new AppDbContext())
        {
            db.Trips.AddRange(trips);
            db.SaveChanges();
        }

        Console.WriteLine("ETL process completed successfully.");
    }
}
