using CsvHelper.Configuration;
using CsvHelper;
using System.Globalization;

namespace BankApp.Services
{
    public class FileManager
    {
        public List<T> ReadData<T>(string path)
        {
            if (!File.Exists(path))
            {
                return new List<T>();
            }
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ","
            };
            using var reader = new StreamReader(path);
            using var csvReader = new CsvReader(reader, config);
            return csvReader.GetRecords<T>().ToList();
        }

        public void WriteData<T>(List<T> records, string path)
        {
            using var writer = new StreamWriter(path);
            using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csvWriter.WriteRecords(records);
        }
    }
}
