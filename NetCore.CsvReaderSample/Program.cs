using Microsoft.VisualBasic.FileIO;
using NetCore.CsvReaderSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace NetCore.CsvReaderSample
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataset = ReadCsvFile("Inputs/dataset_mock.csv", (dataRow) =>
            {
                dataRow["Order Price"] = dataRow["Order Price"].Replace(",", "");
                return MapDataRow<Order>(dataRow);
            });

            var filledOrders = dataset.Where(x => x.Status == Status.FILLED);

            var convertedAmount = filledOrders.Where(x => x.Pair.StartsWith("EUR")).Select(x => double.Parse(Regex.Replace(x.Executed, @"[^\d,\.]", ""))).Sum();
            var totalDeposit = filledOrders.Where(x => x.Pair.EndsWith("EUR") && x.Side == Side.BUY)
                .Aggregate(convertedAmount, (amount, x) => amount += double.Parse(Regex.Replace(x.TradingTotal, @"[^\d,\.]", "")));

            Console.WriteLine("Total deposit amount = {0}EUR", totalDeposit);
            Console.ReadKey(true);
        }

        public static List<TResult> ReadCsvFile<TResult>(string path, Func<Dictionary<string, string>, TResult> map)
        {
            var result = new List<TResult>();
            using (var reader = new TextFieldParser(path) { Delimiters = new[] { "," } })
            {
                var headerRow = reader.ReadFields().Select((x, i) => KeyValuePair.Create(i, x)).ToDictionary(x => x.Key, x => x.Value);

                while (!reader.EndOfData)
                {
                    var dataRow = reader.ReadFields().Select((x, i) => KeyValuePair.Create(i, x)).ToDictionary(x => headerRow[x.Key], x => x.Value);
                    result.Add(map(dataRow));
                }
            }

            return result;
        }

        public static T MapDataRow<T>(Dictionary<string, string> dataRow) where T : class, new()
        {
            var model = Activator.CreateInstance<T>();
            var props = typeof(T).GetProperties();
            foreach (var prop in props)
            {
                var columnName = prop.GetCustomAttribute<CsvPropertyAttribute>()?.Name ?? prop.Name;

                if (!prop.CanWrite || prop.GetIndexParameters().Any())
                {
                    continue;
                }

                if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(model, dataRow[columnName]);
                    continue;
                }

                if (prop.PropertyType.IsEnum)
                {
                    var value = Enum.Parse(prop.PropertyType, dataRow[columnName]);
                    prop.SetValue(model, value);
                }
                else
                {
                    var value = Convert.ChangeType(dataRow[columnName], prop.PropertyType);
                    prop.SetValue(model, value);
                }
            }
            return model;
        }
    }
}
