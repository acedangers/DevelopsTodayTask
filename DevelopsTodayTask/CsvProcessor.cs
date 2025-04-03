using CsvHelper;
using System.Globalization;
using DevelopsTodayTask.Models;
using CsvHelper.Configuration;

public class CsvProcessor
{
    private readonly TimeZoneInfo estTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

    public void ImportCsvToDatabase(string filePath)
    {
        var records = new List<CabTripRecord>();
        var duplicates = new List<CabTripRecord>();

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
        {
            // Read all records from the CSV file
            var allRecords = csv.GetRecords<dynamic>()
                                .Select(r => new CabTripRecord
                                {
                                    TpepPickupDatetime = ConvertToUtc(TryParseDateTime(r.tpep_pickup_datetime)),
                                    TpepDropoffDatetime = ConvertToUtc(TryParseDateTime(r.tpep_dropoff_datetime)),
                                    PassengerCount = int.TryParse(r.passenger_count, out int passengerCount) ? passengerCount : 0,
                                    TripDistance = double.TryParse(r.trip_distance, NumberStyles.Any, CultureInfo.InvariantCulture, out double tripDistance) ? tripDistance : 0,
                                    StoreAndFwdFlag = (r.store_and_fwd_flag.Trim() == "Y") ? "Yes" : "No",
                                    PULocationID = int.TryParse(r.PULocationID, out int puLocationID) ? puLocationID : 0,
                                    DOLocationID = int.TryParse(r.DOLocationID, out int doLocationID) ? doLocationID : 0,
                                    FareAmount = decimal.TryParse(r.fare_amount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal fareAmount) ? fareAmount : 0,
                                    TipAmount = decimal.TryParse(r.tip_amount, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal tipAmount) ? tipAmount : 0
                                })
                                .ToList();

            // Find duplicates
            var groupedRecords = allRecords
                .GroupBy(r => new { r.TpepPickupDatetime, r.TpepDropoffDatetime, r.PassengerCount })
                .ToList();

            // Separate duplicates and unique records
            foreach (var group in groupedRecords)
            {
                if (group.Count() > 1)
                {
                    duplicates.AddRange(group.Skip(1));
                }
                else
                {
                    records.Add(group.First());
                }
            }
        }

        BulkInsertToDatabase(records);
        WriteDuplicatesToCsv(duplicates);
    }

    // Helper method for parsing DateTime
    private DateTime TryParseDateTime(string dateTimeString)
    {
        DateTime result;
        string[] formats = { "MM/dd/yyyy hh:mm:ss tt", "MM/dd/yyyy hh:mm:ss", "yyyy-MM-dd hh:mm:ss tt" }; // Add more formats as needed
        if (DateTime.TryParseExact(dateTimeString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
        {
            return result;
        }
        else
        {
            return DateTime.MinValue;
        }
    }

    // Convert from EST to UTC
    private DateTime ConvertToUtc(DateTime dateTime)
    {
        DateTime estDateTime = TimeZoneInfo.ConvertTime(dateTime, estTimeZone);
        DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(estDateTime, estTimeZone);
        return utcDateTime;
    }

    private void BulkInsertToDatabase(List<CabTripRecord> records)
    {
        using (var context = new AppDbContext())
        {
            int batchSize = 1000; // Set the batch size as needed
            for (int i = 0; i < records.Count; i += batchSize)
            {
                var batch = records.Skip(i).Take(batchSize).ToList();
                context.SampleCabData.AddRange(batch); // Add batch to DbContext
                context.SaveChanges(); // Save changes after each batch
            }
        }
    }

    private void WriteDuplicatesToCsv(List<CabTripRecord> duplicates)
    {
        if (duplicates.Any())
        {
            using (var writer = new StreamWriter("duplicates.csv"))
            using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                csv.WriteRecords(duplicates); // Write all duplicate records to the CSV file
            }
        }
        else
        {
            Console.WriteLine("No duplicates found.");
        }
    }
}
