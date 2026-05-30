using System.IO;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using ETL_pipeline_csv_To_sql;

Console.WriteLine("Watching Landing Zone...");

var watcher = new FileSystemWatcher(
    @"C:\Users\Desktop\LandingZone", // Adjust path as needed - path must exist before running !!
    "*.csv");

// Track files currently being processed to avoid duplicate handling
var processing = new ConcurrentDictionary<string, byte>(StringComparer.OrdinalIgnoreCase);

watcher.Created += (sender, e) =>
{
    var fullPath = e.FullPath;

    // If already processing this file, skip
    if (!processing.TryAdd(fullPath, 0))
    {
        Console.WriteLine($"Already processing: {e.Name}");
        return;
    }

    // Run processing in background to avoid blocking the FileSystemWatcher thread
    _ = Task.Run(async () =>
    {
        try
        {
            Console.WriteLine($"File detected: {e.Name}");

            await Task.Delay(2000);

            await CsvProcessor.ProcessFile(fullPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing {e.Name}: {ex.Message}");
        }
        finally
        {
            processing.TryRemove(fullPath, out _);
        }
    });
};

watcher.EnableRaisingEvents = true;

Console.ReadLine();
