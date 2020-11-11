using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataProcessor.DataSource.File.Tests
{
    [TestClass]
    public class LineParserTest
    {
        [TestMethod]
        public void Parse_Given_a_csv_line_Fields_are_parsed()
        {
            var target = new LineParser();

            var actual = target.Parse("field-1,field-2,field-3");

            Assert.AreEqual(3, actual.LongLength);
            Assert.AreEqual("field-1", actual[0]);
            Assert.AreEqual("field-2", actual[1]);
            Assert.AreEqual("field-3", actual[2]);
        }

        [TestMethod]
        public void Parse_Given_a_line_with_pipe_delimiter_Fields_are_parsed()
        {
            var target = new LineParser
            {
                Delimiter = "|"
            };

            var actual = target.Parse("field-1|field-2|field-3");

            Assert.AreEqual(3, actual.LongLength);
            Assert.AreEqual("field-1", actual[0]);
            Assert.AreEqual("field-2", actual[1]);
            Assert.AreEqual("field-3", actual[2]);
        }

        [TestMethod]
        public void Parse_Given_a_csv_line_that_has_fields_enclosed_in_quotes_Fields_are_parsed()
        {
            var target = new LineParser
            {
                HasFieldsEnclosedInQuotes = true
            };

            var actual = target.Parse("\"field-1\",\"field-2\",\"field-3\"");

            Assert.AreEqual(3, actual.LongLength);
            Assert.AreEqual("field-1", actual[0]);
            Assert.AreEqual("field-2", actual[1]);
            Assert.AreEqual("field-3", actual[2]);
        }

        [TestMethod]
        public void Parse_Given_a_csv_line_that_has_fields_enclosed_in_quotes_and_one_field_with_comma_Fields_are_parsed()
        {
            var target = new LineParser
            {
                HasFieldsEnclosedInQuotes = true
            };

            var actual = target.Parse("\"field-1,1\",\"field-2\",\"field-3\"");

            Assert.AreEqual(3, actual.LongLength);
            Assert.AreEqual("field-1,1", actual[0]);
            Assert.AreEqual("field-2", actual[1]);
            Assert.AreEqual("field-3", actual[2]);
        }

        [TestMethod]
        public void Parse_Given_a_csv_line_that_has_fields_enclosed_in_quotes_and_one_field_with_quotes_Fields_are_parsed()
        {
            var target = new LineParser
            {
                HasFieldsEnclosedInQuotes = true
            };

            var actual = target.Parse("\"field-1\"1\",\"field-2\",\"field-3\"");

            Assert.AreEqual(3, actual.LongLength);
            Assert.AreEqual("field-1\"1", actual[0]);
            Assert.AreEqual("field-2", actual[1]);
            Assert.AreEqual("field-3", actual[2]);
        }
    }
}
