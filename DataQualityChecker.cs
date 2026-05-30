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

            report.NullCustomerIds =
                table.AsEnumerable()
                     .Count(r =>
                         string.IsNullOrWhiteSpace(
                             r["CustomerId"].ToString()));

            report.NullNames =
                table.AsEnumerable()
                     .Count(r =>
                         string.IsNullOrWhiteSpace(
                             r["Name"].ToString()));

            report.DuplicateCustomerIds =
                table.AsEnumerable()
                     .GroupBy(r =>
                         r["CustomerId"])
                     .Count(g => g.Count() > 1);

            return report;
        }
    }
}
