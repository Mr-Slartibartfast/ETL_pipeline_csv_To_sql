using System;

namespace ETL_pipeline_csv_To_sql
{
    public class DataQualityReport
    {
        public int TotalRows { get; set; }

        public int NullCustomerIds { get; set; }

        public int NullNames { get; set; }

        public int DuplicateCustomerIds { get; set; }

        public bool HasErrors =>
            NullCustomerIds > 0 ||
            DuplicateCustomerIds > 0;

        public void Print()
        {
            Console.WriteLine("========== DQ REPORT ==========");

            Console.WriteLine($"Rows: {TotalRows}");
            Console.WriteLine($"Null IDs: {NullCustomerIds}");
            Console.WriteLine($"Null Names: {NullNames}");
            Console.WriteLine($"Duplicates: {DuplicateCustomerIds}");

            Console.WriteLine("===============================");
        }
    }
}
