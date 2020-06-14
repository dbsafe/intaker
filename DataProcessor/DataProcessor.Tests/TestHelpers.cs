using DataProcessor.DataSource.File;
using DataProcessor.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DataProcessor.Tests
{
    public static class TestHelpers
    {
        public static FileDataSource CreateFileDataSource(string filename, bool hasFieldsEnclosedInQuotes)
        {
            var testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var config = new FileDataSourceConfig
            {
                Delimiter = ",",
                HasFieldsEnclosedInQuotes = hasFieldsEnclosedInQuotes,
                Path = Path.Combine(testDirectory, "TestFiles", filename)
            };

            return new FileDataSource(config);
        }

        public static void PrintJson(this TestContext testContext, object obj)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            testContext.Print(json);
        }

        public static void Print(this TestContext testContext, string message)
        {
            testContext.WriteLine(message);
        }

        public static void PrintRowJsons(this TestContext testContext, IEnumerable<Row> rows)
        {
            foreach(var row in rows)
            {
                var message = $"Row Index: {row.Index}\nJSON:\n{row.Json}";
                testContext.WriteLine(message);
            }
        }
    }
}
