using Microsoft.Data.SqlClient;
using System.Data;

public static class SqlLoader
{
    public static async Task Load(
        DataTable table)
    {
        // Use one valid server identifier. Example: local named instance // Adjust database name as needed below
        string connectionString =
            @"Server=.\SQLEXPRESS;
              Database=Stocks; 
              Trusted_Connection=True;
              TrustServerCertificate=True;";

        using SqlConnection conn =
            new(connectionString);

        await conn.OpenAsync();

        using SqlBulkCopy bulk =
            new(conn);

        bulk.DestinationTableName =
            "dbo.VFIFX"; // Adjust table name as needed

        bulk.BatchSize = 5000;

        await bulk.WriteToServerAsync(table);

        Console.WriteLine(
            $"{table.Rows.Count} rows loaded.");
    }
}