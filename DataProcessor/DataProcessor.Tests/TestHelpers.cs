using DataProcessor.DataSource.File;
using DataProcessor.Models;
using DataProcessor.Transformations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DataProcessor.Tests
{
    public static class TestHelpers
    {
        public static FileDataSource<TParserContext> CreateFileDataSource<TParserContext>(
            string filename, 
            bool hasFieldsEnclosedInQuotes, 
            string delimiter,
            string commentedOutIndicator)
            where TParserContext : IParserContext
        {
            var testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var config = new FileDataSourceConfig
            {
                Delimiter = delimiter,
                HasFieldsEnclosedInQuotes = hasFieldsEnclosedInQuotes,
                Path = Path.Combine(testDirectory, "TestFiles", filename),
                CommentedOutIndicator = commentedOutIndicator
            };

            return new FileDataSource<TParserContext>(config);
        }

        public static FileDataSource<TParserContext> CreateFileDataSource<TParserContext>(string filename, bool hasFieldsEnclosedInQuotes)
            where TParserContext : IParserContext
        {
            return CreateFileDataSource<TParserContext>(filename, hasFieldsEnclosedInQuotes, ",", null);
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

        public static void PrintRowJsons(this TestContext testContext, IEnumerable<DataRow20> rows)
        {
            foreach (var row in rows)
            {
                var message = $"Row Index: {row.Row.Index}\nJSON:\n{row.Row.Json}";
                testContext.WriteLine(message);
            }
        }
    }
}
