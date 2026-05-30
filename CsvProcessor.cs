using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ETL_pipeline_csv_To_sql
{
    public static class CsvProcessor
    {
        public static async Task ProcessFile(string filePath)
        {
            DataTable table = new();

            table.Columns.Add("CustomerId");
            table.Columns.Add("Name");
            table.Columns.Add("Email");

            foreach (var line in File.ReadLines(filePath).Skip(1))
            {
                var cols = line.Split(',');

                table.Rows.Add(
                    cols.Length > 0 ? cols[0] : string.Empty,
                    cols.Length > 1 ? cols[1] : string.Empty,
                    cols.Length > 2 ? cols[2] : string.Empty);
            }

            var report = DataQualityChecker.Validate(table);

            report.Print();

            if (report.HasErrors)
            {
                Console.WriteLine("Load cancelled.");
                return;
            }

            await SqlLoader.Load(table);

            ArchiveFile(filePath);
        }

        static void ArchiveFile(string filePath)
        {
            string archive =
                Path.Combine(
                    @"C:\Archive",
                    Path.GetFileName(filePath));

            File.Move(filePath, archive, true);
        }
    }
}
