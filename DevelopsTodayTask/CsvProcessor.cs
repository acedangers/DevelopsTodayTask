using CsvHelper;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Linq;
using DevelopsTodayTask.Models;

public class CsvProcessor
{
    public static List<CabTripRecord> ReadCsv(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        return csv.GetRecords<CabTripRecord>().ToList();
    }
}
