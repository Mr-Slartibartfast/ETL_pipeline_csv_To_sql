using System;

namespace ETL_pipeline_csv_To_sql
{
    public class DataQualityReport
    {
        public int TotalRows { get; set; }

        // Reused property name: counts rows with missing or invalid Date
        public int NullCustomerIds { get; set; }

        // Reused property name: counts rows with any missing numeric value
        public int NullNames { get; set; }

        // Reused property name: duplicate dates
        public int DuplicateCustomerIds { get; set; }

        public bool HasErrors =>
            NullCustomerIds > 0 ||
            DuplicateCustomerIds > 0;

        public void Print()
        {
            Console.WriteLine("========== DQ REPORT ==========");

            Console.WriteLine($"Rows: {TotalRows}");
            Console.WriteLine($"Invalid/Missing Dates: {NullCustomerIds}");
            Console.WriteLine($"Missing Numeric Values (Open/High/Low/Close): {NullNames}");
            Console.WriteLine($"Duplicate Dates: {DuplicateCustomerIds}");

            Console.WriteLine("===============================");
        }
    }
}
