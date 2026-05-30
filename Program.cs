using System.IO;
using System.Threading.Tasks;
using ETL_pipeline_csv_To_sql;

Console.WriteLine("Watching Landing Zone...");

var watcher = new FileSystemWatcher(
    @"C:\LandingZone",
    "*.csv");

watcher.Created += async (sender, e) =>
{
    Console.WriteLine($"File detected: {e.Name}");

    await Task.Delay(2000);

    await CsvProcessor.ProcessFile(e.FullPath);
};

watcher.EnableRaisingEvents = true;

Console.ReadLine();
