## ETL Project: CSV Data Import into MS SQL Server
This project is designed to read data from a CSV file, process it, and insert it into a pre-existing MS SQL Server database table. 
It uses C# with Entity Framework to perform the data import and transformations, and SQL Server for data storage. 
The project processes and inserts a flat dataset from a CSV into table in a database.

## SQL Scripts Used for Creating the Database and Tables
### Create database
CREATE DATABASE SampleCabData; 

### Create empty table
CREATE TABLE Sample_Cab_Data ( 
	id INT IDENTITY(1,1) PRIMARY KEY,
    tpep_pickup_datetime DATETIME,
    tpep_dropoff_datetime DATETIME,
    passenger_count INT,
    trip_distance FLOAT,
    store_and_fwd_flag VARCHAR(3),
    PULocationID INT,
    DOLocationID INT,
    fare_amount DECIMAL(18, 2),
    tip_amount DECIMAL(18, 2)
);

### Optimization for user queries
CREATE INDEX idx_PULocationId_tipAmount ON Sample_Cab_Data (PULocationID, tip_amount);
CREATE INDEX idx_trip_distance ON Sample_Cab_Data (trip_distance DESC);
CREATE INDEX idx_PULocationId ON Sample_Cab_Data (PULocationID);

### User queries
#### Find out which `PULocationId` (Pick-up location ID) has the highest tip_amount on average.
SELECT TOP 1 PULocationId, AVG(tip_amount) AS avg_tip_amount
FROM Sample_Cab_Data
GROUP BY PULocationId
ORDER BY avg_tip_amount DESC;

#### Find the top 100 longest fares in terms of `trip_distance`.
SELECT TOP 100 * 
FROM Sample_Cab_Data
ORDER BY trip_distance DESC;

#### Find the top 100 longest fares in terms of time spent traveling.
SELECT TOP 100 *, DATEDIFF(SECOND, tpep_pickup_datetime, tpep_dropoff_datetime) AS trip_duration
FROM Sample_Cab_Data
ORDER BY trip_duration DESC;

### Number of rows in your table after running the program
total_rows
29779
(before: 30001)

## Assumptions and Notes
### CSV Format 
It is assumed that the CSV file follows the structure defined in the project, including the columns:
  tpep_pickup_datetime
  
  tpep_dropoff_datetime
  
  passenger_count
  
  trip_distance
  
  store_and_fwd_flag
  
  PULocationID
  
  DOLocationID
  
  fare_amount
  
  tip_amount

### Timezone Conversion
The tpep_pickup_datetime and tpep_dropoff_datetime values are assumed to be in Eastern Standard Time (EST). 
They are automatically converted to UTC before being inserted into the database.

### Duplicate Removal
The program identifies and removes duplicates based on the combination of tpep_pickup_datetime, tpep_dropoff_datetime, and passenger_count. 
Any removed duplicates are written to a separate duplicates.csv file.

### Whitespace Handling
All text-based fields (such as store_and_fwd_flag) are trimmed of leading and trailing whitespace before insertion into the database.

### Performance Considerations
The program performs batch inserts to optimize performance when dealing with large datasets.

If the program were to process a CSV file of 10GB or more, further optimizations such as using SqlBulkCopy or external libraries like EF Extensions might be necessary for even faster performance.
I would use parallel processing to read and insert data concurrently, taking advantage of multi-core systems to speed up the process.

On the SQL Server side, I would ensure that indexes are only created on the most frequently queried columns to avoid unnecessary overhead.
For such a large dataset, robust logging to capture any issues during the import process and ensuring the system can recover easily in case of failure is crucial.

### Error Handling:
The program handles exceptions during parsing and insertion but may require further logging or more sophisticated error-handling mechanisms for production use.

## How to Run the Program
Ensure SQL Server is Running and the database and table are set up.
Place the CSV File in a known directory.
Configure the Connection String in the App.config file of the C# project.
Build and Run the project in Visual Studio or another C# development environment.

After running the program, the data from the CSV will be imported into the table.
