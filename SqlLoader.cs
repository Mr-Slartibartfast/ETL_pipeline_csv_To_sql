using Microsoft.Data.SqlClient;
using System.Data;

public static class SqlLoader
{
    public static async Task Load(
        DataTable table)
    {
        string connectionString =
            @"Server=localhost\SQLEXPRESS;
              Database=DataWarehouse;
              Trusted_Connection=True;
              TrustServerCertificate=True;";

        using SqlConnection conn =
            new(connectionString);

        await conn.OpenAsync();

        using SqlBulkCopy bulk =
            new(conn);

        bulk.DestinationTableName =
            "dbo.Customer";

        bulk.BatchSize = 5000;

        await bulk.WriteToServerAsync(table);

        Console.WriteLine(
            $"{table.Rows.Count} rows loaded.");
    }
}