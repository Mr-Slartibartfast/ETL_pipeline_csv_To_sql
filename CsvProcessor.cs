using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;

namespace ETL_pipeline_csv_To_sql
{
    public static class CsvProcessor
    {
        // Expect CSV columns: Date,Open,High,Low,Close
        public static async Task ProcessFile(string filePath)
        {
            DataTable table = new();
            table.Columns.Add("Date", typeof(DateTime));
            table.Columns.Add("Open", typeof(decimal));
            table.Columns.Add("High", typeof(decimal));
            table.Columns.Add("Low", typeof(decimal));
            table.Columns.Add("Close", typeof(decimal));

            var cfg = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };

            // Scope the reader/csv so they're disposed before Load/Archive
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, cfg))
            {
                if (cfg.HasHeaderRecord)
                {
                    await csv.ReadAsync();
                    csv.ReadHeader();
                }

                while (await csv.ReadAsync())
                {
                    var dateField = csv.GetField(0);
                    var openField = csv.GetField(1);
                    var highField = csv.GetField(2);
                    var lowField = csv.GetField(3);
                    var closeField = csv.GetField(4);

                    DateTime dateVal; bool dateOk = DateTime.TryParse(dateField, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateVal);
                    decimal openVal; bool openOk = decimal.TryParse(openField, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out openVal);
                    decimal highVal; bool highOk = decimal.TryParse(highField, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out highVal);
                    decimal lowVal; bool lowOk = decimal.TryParse(lowField, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out lowVal);
                    decimal closeVal; bool closeOk = decimal.TryParse(closeField, NumberStyles.Number | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out closeVal);

                    table.Rows.Add(
                      dateOk ? (object)dateVal : DBNull.Value,
                      openOk ? (object)openVal : DBNull.Value,
                      highOk ? (object)highVal : DBNull.Value,
                      lowOk ? (object)lowVal : DBNull.Value,
                      closeOk ? (object)closeVal : DBNull.Value);
                }
            } // reader and csv disposed here

            var report = DataQualityChecker.Validate(table);

            report.Print();

            if (report.HasErrors)
            {
                Console.WriteLine("Load cancelled.");
                return;
            }

            await SqlLoader.Load(table);

            // Attempt to archive asynchronously with retries to avoid races
            await ArchiveFileAsync(filePath, TimeSpan.FromSeconds(30));
        }

        // Async archive with retry loop to avoid race between a separate check and File.Move
        static async Task ArchiveFileAsync(string filePath, TimeSpan timeout)
        {
            string archiveFolder = @"C:\Users\Desktop\Archive"; // Adjust path as needed - path must exist before running !!
            Directory.CreateDirectory(archiveFolder);
            string archivePath = Path.Combine(archiveFolder, Path.GetFileName(filePath));

            var sw = System.Diagnostics.Stopwatch.StartNew();
            TimeSpan delay = TimeSpan.FromMilliseconds(250);

            while (sw.Elapsed < timeout)
            {
                try
                {
                    // Try move directly; if another process holds the file this will throw
                    File.Move(filePath, archivePath, overwrite: true);
                    return; // success
                }
                catch (IOException)
                {
                    // transient lock - wait asynchronously and retry
                    await Task.Delay(delay);
                }
            }

            throw new IOException($"Timeout waiting to move file: {filePath}");
        }
    }
}
