using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");
var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);
var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

GenerateSalesSummary(salesFiles, salesTotalDir);

foreach (var file in salesFiles)
{
    Console.WriteLine(file);
}

IEnumerable<string> FindFiles(string foldername)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(foldername, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
     
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    // read files loop
    foreach (var file in salesFiles)
    {
        // read the contents of the file
        string salesJson = File.ReadAllText(file);

        // parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

        // add the amount found in the total field to the salestotal variable
        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}

void GenerateSalesSummary(IEnumerable<string> salesFiles, string salesTotalDir)
{
    double salesTotal = 0;
    StringBuilder details = new StringBuilder();

    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);
        string fileName = Path.GetFileName(file);

        double fileTotal = 0;

        if (fileName.Equals("sales.json", StringComparison.OrdinalIgnoreCase))
        {
            SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
            fileTotal = data?.Total ?? 0;
        }
        else if (fileName.Equals("salestotals.json", StringComparison.OrdinalIgnoreCase))
        {
            SalesTotalData? data = JsonConvert.DeserializeObject<SalesTotalData?>(salesJson);
            fileTotal = data?.OverallTotal ?? 0;
        }
        
        salesTotal += fileTotal;

        details.AppendLine($"{Path.GetFileName(Path.GetDirectoryName(file))}/{Path.GetFileName(file)}: {fileTotal:C}");
    }
    
    StringBuilder report = new StringBuilder();

    report.AppendLine("Sales Summary");
    report.AppendLine("========================");
    report.AppendLine($"Total Sales: {salesTotal:C}");
    report.AppendLine();
    report.AppendLine("Details:");
    report.Append(details);

    File.WriteAllText(Path.Combine(salesTotalDir, "salesSummary.txt"), report.ToString());
}

record SalesData (double Total);

record SalesTotalData(double OverallTotal);
