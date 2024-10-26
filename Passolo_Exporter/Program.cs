using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    class CsvEntry
    {
        public string Comment { get; set; }
        public string Source { get; set; }
        public Dictionary<string, string> Targets { get; set; }

        public CsvEntry()
        {
            Targets = new Dictionary<string, string>();
        }
    }

    static void Main()
    {
        string folderPath = Directory.GetCurrentDirectory();
        var csvFiles = Directory.GetFiles(folderPath, "*.csv");

        if (csvFiles.Length == 0)
        {
            Console.WriteLine("No CSV files found in the directory.");
            return;
        }

        var languages = new HashSet<string>();
        var dataDict = new Dictionary<string, CsvEntry>();

        // Extract language codes and populate the dictionary
        foreach (var file in csvFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            string languageCode = fileName.Substring(fileName.Length - 5);
            languages.Add(languageCode);

            var lines = File.ReadAllLines(file).Skip(1); // Skip the header row

            foreach (var line in lines)
            {
                // Replace "; with a unique temporary delimiter
                string tempLine = line.Replace("\";", "\"⇴").Replace(";\"", "⇴\"");
                var values = tempLine.Split('⇴');

                if (values.Length < 3) continue;

                string source = values[0].Trim('"');
    string target = values[1].Trim('"');
    string id = values[2].Trim('"');
    string comment = values.Length > 3 ? values[3].Trim('"') : string.Empty;

                if (!dataDict.ContainsKey(id))
                {
                    dataDict[id] = new CsvEntry
                    {
                        Comment = comment,
                        Source = source
};
                }

                dataDict[id].Targets[languageCode] = $"{languageCode}_{target}";
            }
        }

        // Create Merged.csv
        using (var writer = new StreamWriter(Path.Combine(folderPath, "Merged.csv")))
        {
            // Write header
            writer.Write("en-US;");
            writer.Write(string.Join(";", languages.Select(lang => $"{lang}")));
            writer.WriteLine(";ID;Comment");

            // Write data rows
            foreach (var entry in dataDict)
            {
                var id = entry.Key;
var csvEntry = entry.Value;

writer.Write($"{csvEntry.Source};");

                foreach (var lang in languages)
                {
                    if (csvEntry.Targets.ContainsKey(lang))
                    {
                        string targetValue = csvEntry.Targets[lang];
                        writer.Write($"{targetValue.Substring(6)};");
                    }
                    else
                    {
                        writer.Write(";");
                    }
                }

                writer.WriteLine($"{id};{csvEntry.Comment}");
            }
        }

        Console.WriteLine("Merged.csv has been created successfully.");
        Console.ReadKey();
    }
}
