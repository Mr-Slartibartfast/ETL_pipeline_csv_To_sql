using System.Data;
using System.Linq;

namespace ETL_pipeline_csv_To_sql
{
    public static class DataQualityChecker
    {
        public static DataQualityReport Validate(
            DataTable table)
        {
            DataQualityReport report = new();

            report.TotalRows = table.Rows.Count;

            // Count null/invalid Dates
            report.NullCustomerIds =
                table.AsEnumerable()
                     .Count(r => r.IsNull("Date"));

            // Count null/invalid numeric fields (Open/High/Low/Close)
            report.NullNames =
                table.AsEnumerable()
                     .Count(r => r.IsNull("Open") || r.IsNull("High") || r.IsNull("Low") || r.IsNull("Close"));

            // Duplicates by Date
            report.DuplicateCustomerIds =
                table.AsEnumerable()
                     .GroupBy(r => r.Field<DateTime?>("Date"))
                     .Count(g => g.Key != null && g.Count() > 1);

            return report;
        }
    }
}
