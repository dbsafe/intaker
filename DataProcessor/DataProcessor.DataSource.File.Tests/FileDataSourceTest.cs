using DataProcessor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DataProcessor.DataSource.File.Tests
{
    /// <summary>
    /// Test for FileDataSource and base class BaseDataSource
    /// </summary>
    [TestClass]
    public class FileDataSourceTest
    {
        private readonly string _testDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Process_Given_a_file_that_does_not_exist_Should_throw_an_exception()
        {
            var target = CreateTarget("test-file.txt", true);
            var context = new ParserContext<Row>();

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
        public void Process_Given_a_file_ProcessField_event_should_be_raised_for_every_field()
        {
            var target = CreateTarget("test-file-01.csv", true);
            var context = new ParserContext<Row>();

            var expected = new List<Field>
            {
                new Field { Index = 0, Raw = "field-1a", ValidationResult = ValidationResultType.Valid, Value = "1a" },
                new Field { Index = 1, Raw = "field-1b", ValidationResult = ValidationResultType.Valid, Value = "1b" },

                new Field { Index = 0, Raw = "field-2a", ValidationResult = ValidationResultType.Valid, Value = "2a" },
                new Field { Index = 1, Raw = "field-2b", ValidationResult = ValidationResultType.Valid, Value = "2b" },
                new Field { Index = 2, Raw = "field-2c", ValidationResult = ValidationResultType.Valid, Value = "2c" },

                new Field { Index = 0, Raw = "field-3a", ValidationResult = ValidationResultType.Valid, Value = "3a" },
                new Field { Index = 1, Raw = "field-3b", ValidationResult = ValidationResultType.Valid, Value = "3b" }
            };

            var actual = new List<Field>();
            target.ProcessField += (sender, e) =>
            {
                e.Field.Value = e.Field.Raw[6..];
                actual.Add(e.Field);
            };

            target.Process(context);

            AssertFields(expected, actual);
        }

        [TestMethod]
        public void Process_ProcessField_Fields_should_reference_the_parent_row()
        {
            var target = CreateTarget("test-file-01.csv", true);
            var context = new ParserContext<Row>();

            var row0 = new Row
            {
                Index = 0,
                Raw = "\"field-1a\",\"field-1b\"",
                ValidationResult = ValidationResultType.Valid
            };

            row0.Fields.Add(new Field { Row = row0, Index = 0, Raw = "field-1a", ValidationResult = ValidationResultType.Valid });
            row0.Fields.Add(new Field { Row = row0, Index = 1, Raw = "field-1b", ValidationResult = ValidationResultType.Valid });

            var row1 = new Row
            {
                Index = 1,
                Raw = "\"field-2a\",\"field-2b\",\"field-2c\"",
                ValidationResult = ValidationResultType.Valid
            };

            row1.Fields.Add(new Field { Row = row1, Index = 0, Raw = "field-2a", ValidationResult = ValidationResultType.Valid });
            row1.Fields.Add(new Field { Row = row1, Index = 1, Raw = "field-2b", ValidationResult = ValidationResultType.Valid });
            row1.Fields.Add(new Field { Row = row1, Index = 2, Raw = "field-2c", ValidationResult = ValidationResultType.Valid });

            var row2 = new Row
            {
                Index = 2,
                Raw = "\"field-3a\",\"field-3b\"",
                ValidationResult = ValidationResultType.Valid
            };

            row2.Fields.Add(new Field { Row = row2, Index = 0, Raw = "field-3a", ValidationResult = ValidationResultType.Valid });
            row2.Fields.Add(new Field { Row = row2, Index = 1, Raw = "field-3b", ValidationResult = ValidationResultType.Valid });

            var expected = new List<Row> { row0, row0, row1, row1, row1, row2, row2 };


            var actual = new List<Row>();
            target.ProcessField += (sender, e) =>
            {
                actual.Add(e.Field.Row);
            };

            target.Process(context);

            AssertRows(expected, actual);
        }

        [TestMethod]
        public void Process_Given_a_file_Context_in_processField_event_should_indicate_the_row_index()
        {
            var target = CreateTarget("test-file-01.csv", true);
            var context = new ParserContext<Row>();

            var expected = new List<int>
            {
                0,
                0,

                1,
                1,
                1,

                2,
                2
            };

            var actual = new List<int>();
            target.ProcessField += (sender, e) => { actual.Add(e.Context.CurrentRowIndex); };

            target.Process(context);

            AssertIndexes(expected, actual);
        }

        [TestMethod]
        public void Process_Given_a_file_Context_in_processField_event_should_indicate_the_raw_fields()
        {
            var target = CreateTarget("test-file-01.csv", true);
            var context = new ParserContext<Row>();

            var expected = new List<string[]>
            {
                new string[] { "field-1a", "field-1b" },
                new string[] { "field-1a", "field-1b" },
                new string[] { "field-2a","field-2b","field-2c" },
                new string[] { "field-2a","field-2b","field-2c" },
                new string[] { "field-2a","field-2b","field-2c" },
                new string[] { "field-3a","field-3b" },
                new string[] { "field-3a","field-3b" }
            };

            var actual = new List<string[]>();

            target.ProcessField += (sender, e) => { actual.Add(e.Context.CurrentRowRawFields); };

            target.Process(context);

            AssertRawFields(expected, actual);
        }

        [TestMethod]
        public void Process_ProcessField_Given_that_context_indicates_abort_The_process_should_stop()
        {
            var target = CreateTarget("test-file-01.csv", true);
            var context = new ParserContext<Row>();
            var rowCount = 0;
            var fieldCount = 0;

            target.ProcessField += (sender, e) =>
            {
                fieldCount++;
                if (fieldCount == 3)
                {
                    context.IsAborted = true;
                }
            };

            target.AfterProcessRow += (sender, e) => { rowCount++; };

            target.Process(context);

            Assert.AreEqual(1, rowCount);
            Assert.AreEqual(3, fieldCount);
        }

        [TestMethod]
        public void Process_Given_a_file_BeforeProcessRow_event_should_be_raised_for_every_row()
        {
            var target = CreateTarget("test-file-01.csv", true);
            var context = new ParserContext<Row>();

            var row0 = new Row
            {
                Index = 0,
                Raw = "\"field-1a\",\"field-1b\"",
                ValidationResult = ValidationResultType.Valid
            };

            var row1 = new Row
            {
                Index = 1,
                Raw = "\"field-2a\",\"field-2b\",\"field-2c\"",
                ValidationResult = ValidationResultType.Valid
            };

            var row2 = new Row
            {
                Index = 2,
                Raw = "\"field-3a\",\"field-3b\"",
                ValidationResult = ValidationResultType.Valid
            };

            var expected = new List<Row> { row0, row1, row2 };
            var actual = new List<Row>();
            target.BeforeProcessRow += (sender, e) =>
            {
                Assert.AreEqual(0, e.Row.Fields.Count, "Rows should not have fields at this time");
                var rowAtTheTimeOfTheEvent = new Row
                {
                    Index = e.Row.Index,
                    Raw = e.Row.Raw,
                    RawFields = e.Row.RawFields,
                    ValidationResult = ValidationResultType.Valid
                };

                actual.Add(rowAtTheTimeOfTheEvent);
            };

            target.Process(context);

            AssertRows(expected, actual);
        }

        [TestMethod]
        public void Process_Given_a_file_with_commented_line_BeforeProcessRow_event_should_be_raised_for_every_row_that_is_not_commented_out()
        {
            var target = CreateTarget("test-file-02.csv", true, "#");
            var context = new ParserContext<Row>();

            var row0 = new Row
            {
                Index = 0,
                Raw = "\"field-1a\",\"field-1b\"",
                ValidationResult = ValidationResultType.Valid
            };

            var row2 = new Row
            {
                Index = 2,
                Raw = "\"field-3a\",\"field-3b\"",
                ValidationResult = ValidationResultType.Valid
            };

            var expected = new List<Row> { row0, row2 };
            var actual = new List<Row>();
            target.BeforeProcessRow += (sender, e) =>
            {
                Assert.AreEqual(0, e.Row.Fields.Count, "Rows should not have fields at this time");
                var rowAtTheTimeOfTheEvent = new Row
                {
                    Index = e.Row.Index,
                    Raw = e.Row.Raw,
                    RawFields = e.Row.RawFields,
                    ValidationResult = ValidationResultType.Valid
                };

                actual.Add(rowAtTheTimeOfTheEvent);
            };

            target.Process(context);

            AssertRows(expected, actual);
        }

        [TestMethod]
        public void Process_BeforeProcessRow_Given_that_context_indicates_abort_The_process_should_stop()
        {
            var target = CreateTarget("test-file-01.csv", true);
            var context = new ParserContext<Row>();
            var rowCount = 0;
            var fieldCount = 0;

            target.ProcessField += (sender, e) => { fieldCount++; };

            target.BeforeProcessRow += (sender, e) => 
            { 
                rowCount++; 
                if (rowCount == 2)
                {
                    e.Context.IsAborted = true;
                }
            };

            target.Process(context);

            Assert.AreEqual(2, rowCount);
            Assert.AreEqual(2, fieldCount);
        }

        [TestMethod]
        public void Process_Given_a_file_AfterProcessRow_event_should_be_raised_for_every_row()
        {
            var target = CreateTarget("test-file-01.csv", true);
            var context = new ParserContext<Row>();

            var row0 = new Row
            {
                Index = 0,
                Raw = "\"field-1a\",\"field-1b\"",
                ValidationResult = ValidationResultType.Valid
            };

            row0.Fields.Add(new Field { Row = row0, Index = 0, Raw = "field-1a", ValidationResult = ValidationResultType.Valid });
            row0.Fields.Add(new Field { Row = row0, Index = 1, Raw = "field-1b", ValidationResult = ValidationResultType.Valid });

            var row1 = new Row
            {
                Index = 1,
                Raw = "\"field-2a\",\"field-2b\",\"field-2c\"",
                ValidationResult = ValidationResultType.Valid
            };

            row1.Fields.Add(new Field { Row = row1, Index = 0, Raw = "field-2a", ValidationResult = ValidationResultType.Valid });
            row1.Fields.Add(new Field { Row = row1, Index = 1, Raw = "field-2b", ValidationResult = ValidationResultType.Valid });
            row1.Fields.Add(new Field { Row = row1, Index = 2, Raw = "field-2c", ValidationResult = ValidationResultType.Valid });

            var row2 = new Row
            {
                Index = 2,
                Raw = "\"field-3a\",\"field-3b\"",
                ValidationResult = ValidationResultType.Valid
            };

            row2.Fields.Add(new Field { Row = row2, Index = 0, Raw = "field-3a", ValidationResult = ValidationResultType.Valid });
            row2.Fields.Add(new Field { Row = row2, Index = 1, Raw = "field-3b", ValidationResult = ValidationResultType.Valid });

            var expected = new List<Row> { row0, row1, row2 };
            var actual = new List<Row>();
            target.BeforeProcessRow += (sender, e) => { actual.Add(e.Row); };

            target.Process(context);

            AssertRows(expected, actual);
        }

        [TestMethod]
        public void Process_AfterProcessRow_Given_that_context_indicates_abort_The_process_should_stop()
        {
            var target = CreateTarget("test-file-01.csv", true);
            var context = new ParserContext<Row>();
            var rowCount = 0;
            var fieldCount = 0;

            target.ProcessField += (sender, e) => { fieldCount++; };

            target.AfterProcessRow += (sender, e) =>
            {
                rowCount++;
                if (rowCount == 2)
                {
                    e.Context.IsAborted = true;
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
            Assert.AreEqual(expected.ValidationResult, actual.ValidationResult);
            Assert.AreEqual(expected.Value, actual.Value);
        }

        private FileDataSource<ParserContext<Row>> CreateTarget(string filename, bool hasFieldsEnclosedInQuotes, string commentedOutIndicator = null)
        {
            var config = new FileDataSourceConfig
            {
                Delimiter = ",",
                HasFieldsEnclosedInQuotes = hasFieldsEnclosedInQuotes,
                Path = Path.Combine(_testDirectory, "TestFiles", filename),
                CommentedOutIndicator = commentedOutIndicator
            };

            return new FileDataSource<ParserContext<Row>>(config);
        }

        private void AssertIndexes(IList<int> expected, IList<int> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        private void AssertRawFields(IList<string[]> expected, IList<string[]> actual)
        {
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                AssertRawFields(expected[i], actual[i]);
            }
        }

        private void AssertRawFields(string[] expected, string[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}
