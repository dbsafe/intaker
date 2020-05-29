using DataProcessor.Domain.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DataProcessor.DataSource.File.Tests
{
    [TestClass]
    public class FileDataSourceTest
    {
        private readonly string _testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Process_Given_a_file_that_does_not_exist_Should_throw_an_exception()
        {
            var target = CreateTarget("test-file.txt", true);
            var context = new ParserContext();

            try
            {
                target.Process(context);
                Assert.Fail("An exception was not thrown");
            }
            catch (FileNotFoundException ex)
            {
                Assert.AreEqual("File not found", ex.Message);
            }
        }

        [TestMethod]
        public void Process_Given_a_file_OnProcessField_event_should_be_raised_for_every_field()
        {
            var target = CreateTarget("test-file-01.csv", true);
            var context = new ParserContext();

            var expected = new List<Field>
            {
                new Field { RowIndex = 0, Index = 0, Raw = "field-1a" },
                new Field { RowIndex = 0, Index = 1, Raw = "field-1b" },

                new Field { RowIndex = 1, Index = 0, Raw = "field-2a" },
                new Field { RowIndex = 1, Index = 1, Raw = "field-2b" },
                new Field { RowIndex = 1, Index = 2, Raw = "field-2c" },

                new Field { RowIndex = 2, Index = 0, Raw = "field-3a" },
                new Field { RowIndex = 2, Index = 1, Raw = "field-3b" }
            };

            var actual = new List<Field>();
            target.ProcessField += (sender, e) => { actual.Add(e.Field); };

            target.Process(context);

            AssertFields(expected, actual);
        }

        [TestMethod]
        public void Process_Given_a_file_OnProcessRow_event_should_be_raised_for_every_field()
        {
            var target = CreateTarget("test-file-01.csv", true);
            var context = new ParserContext();

            var row0 = new Row
            {
                Index = 0,
                Raw = "\"field-1a\",\"field-1b\""
            };

            row0.Fields.Add(new Field { RowIndex = 0, Index = 0, Raw = "field-1a" });
            row0.Fields.Add(new Field { RowIndex = 0, Index = 1, Raw = "field-1b" });

            var row1 = new Row
            {
                Index = 1,
                Raw = "\"field-2a\",\"field-2b\",\"field-2c\""
            };

            row1.Fields.Add(new Field { RowIndex = 1, Index = 0, Raw = "field-2a" });
            row1.Fields.Add(new Field { RowIndex = 1, Index = 1, Raw = "field-2b" });
            row1.Fields.Add(new Field { RowIndex = 1, Index = 2, Raw = "field-2c" });

            var row2 = new Row
            {
                Index = 2,
                Raw = "\"field-3a\",\"field-3b\""
            };

            row2.Fields.Add(new Field { RowIndex = 2, Index = 0, Raw = "field-3a" });
            row2.Fields.Add(new Field { RowIndex = 2, Index = 1, Raw = "field-3b" });

            var expected = new List<Row> { row0, row1, row2 };
            var actual = new List<Row>();
            target.ProcessRow += (sender, e) => { actual.Add(e.Row); };

            target.Process(context);

            AssertRows(expected, actual);
        }

        [TestMethod]
        public void Process_Given_that_context_indicates_abort_from_field_event_The_process_should_stop()
        {
            var target = CreateTarget("test-file-01.csv", true);
            var context = new ParserContext();
            var rowCount = 0;
            var fieldCount = 0;

            target.ProcessField += (sender, e) =>
            {
                fieldCount++;
                if (fieldCount == 3)
                {
                    context.Abort = true;
                }
            };
            target.ProcessRow += (sender, e) => { rowCount++; };

            target.Process(context);

            Assert.AreEqual(1, rowCount);
            Assert.AreEqual(3, fieldCount);
        }

        [TestMethod]
        public void Process_Given_that_context_indicates_abort_from_row_event_The_process_should_stop()
        {
            var target = CreateTarget("test-file-01.csv", true);
            var context = new ParserContext();
            var rowCount = 0;
            var fieldCount = 0;

            target.ProcessField += (sender, e) => { fieldCount++; };
            target.ProcessRow += (sender, e) => 
            { 
                rowCount++;
                if (rowCount == 2)
                {
                    context.Abort = true;
                }
            };

            target.Process(context);

            Assert.AreEqual(2, rowCount);
            Assert.AreEqual(5, fieldCount);
        }

        private void AssertRows(IList<Row> expected, IList<Row> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                AssertRow(expected[i], actual[i]);
            }
        }

        private void AssertRow(Row expected, Row actual)
        {
            AssertFields(expected.Fields, actual.Fields);
            Assert.AreEqual(expected.Index, actual.Index);
            Assert.AreEqual(expected.Raw, actual.Raw);
            Assert.AreEqual(expected.ValidationResult, actual.ValidationResult);
        }

        private void AssertFields(IList<Field> expected, IList<Field> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                AssertField(expected[i], actual[i]);
            }
        }

        private void AssertField(Field expected, Field actual)
        {
            Assert.AreEqual(expected.Index, actual.Index);
            Assert.AreEqual(expected.Raw, actual.Raw);
            Assert.AreEqual(expected.RowIndex, actual.RowIndex);
            Assert.AreEqual(expected.ValidationResult, actual.ValidationResult);
            Assert.AreEqual(expected.Value, actual.Value);
        }

        private void Target_ProcessRow(object sender, Domain.Contracts.ProcessRowEventArgs e)
        {
            throw new NotImplementedException();
        }

        private FileDataSource CreateTarget(string filename, bool hasFieldsEnclosedInQuotes)
        {
            var config = new FileDataSourceConfig
            {
                Delimiter = ",",
                HasFieldsEnclosedInQuotes = hasFieldsEnclosedInQuotes,
                Path = Path.Combine(_testDirectory, "TestFiles", filename)
            };

            return new FileDataSource(config);
        }
    }
}
