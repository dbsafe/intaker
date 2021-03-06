using DataProcessor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace DataProcessor.Transformations.Tests
{
    [TestClass]
    public class RowGroupsCreatorTest
    {
        private readonly IEnumerable<string> _lines = new string[]
        {
            // Group: key-a, master with children
            "D1,key-a,d1-f2-00,d1-f3-00,master-key-a",
            "D2,key-a,d2-f2-01,d2-f3-01,child-01-key-a",
            "D2,key-a,d2-f2-02,d2-f3-02,child-02-key-a",

            // Group: key-b, master with child
            "D1,key-b,d1-f2-03,d1-f3-03,master-key-b",
            "D2,key-b,d2-f2-04,d2-f3-04,child-05-key-b",

            // Group: key-c, master without children
            "D1,key-c,d1-f2-05,d1-f3-05,master-key-c",

            // Group: key-d, child without a master
            "D2,key-d,d2-f2-06,d2-f3-06,child-06-key-d",

            // Group: key-e, master without children
            "D1,key-e,d1-f2-07,d1-f3-07,master-key-e",

            // Group: key-a, one more children down here
            "D3,key-a,d2-f2-08,d2-f3-08,child-08-key-a",
        };

        private DataRow20[] _rows;
        private RowGroupsCreator _target;

        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void Initialize()
        {
            var config = new RowGroupsCreatorConfig { MasterDataType = "D1" };
            _target = new RowGroupsCreator(config);

            _rows = CreateRows();
        }

        private DataRow20[] CreateRows()
        {
            var rows = new List<DataRow20>();
            int rowIndex = 0;

            foreach (var line in _lines)
            {
                var row = CreateRow(rowIndex++, line);
                rows.Add(row);
            }

            return rows.ToArray();
        }

        private DataRow20 CreateRow(int index, string raw)
        {
            var row = new Row { Index = index, Raw = raw };

            var rawFields = raw.Split(',');
            var fieldIndex = 0;
            foreach (var rawField in rawFields)
            {
                row.Fields.Add(CreateField(fieldIndex++, rawField));
            }

            return new DataRow20
            {
                Row = row,
                DataType = rawFields[0],
                DataKey = rawFields[1]
            };
        }

        private Field CreateField(int index, string raw)
        {
            return new Field { Index = index, Raw = raw, Value = raw };
        }

        [TestMethod]
        public void BuildRowGroups_Given_rows_with_the_same_key_Rows_should_be_grouped_with_a_master_row()
        {
            var actual = _target.BuildRowGroups(_rows).ToArray(); ;
            TestContext.PrintJson(actual);

            var actualGroup0 = actual[0];
            Assert.AreSame(_rows[0], actualGroup0.MasterRow);
            var rowsGroupedByType0 = actualGroup0.RowsGroupedByType.ToArray();
            Assert.AreEqual(2, rowsGroupedByType0.Length);
            var list = rowsGroupedByType0[0].Value;
            Assert.AreEqual(2, list.Count);
            Assert.AreSame(_rows[1], list[0]);
            Assert.AreSame(_rows[2], list[1]);

            list = rowsGroupedByType0[1].Value;
            Assert.AreEqual(1, list.Count);
            Assert.AreSame(_rows[8], list[0]);

            var actualGroup1 = actual[1];
            Assert.AreSame(_rows[3], actualGroup1.MasterRow);
            var rowsGroupedByType1 = actualGroup1.RowsGroupedByType.ToArray();
            Assert.AreEqual(1, rowsGroupedByType1.Length);
            list = rowsGroupedByType1[0].Value;
            Assert.AreEqual(1, list.Count);
            Assert.AreSame(_rows[4], list[0]);
        }

        [TestMethod]
        public void BuildRowGroups_Given_master_rows_without_children_rows_Rows_should_be_the_master_row_in_separate_groups()
        {
            var actual = _target.BuildRowGroups(_rows).ToArray(); ;
            TestContext.PrintJson(actual);

            var actualGroup2 = actual[2];
            Assert.AreSame(_rows[5], actualGroup2.MasterRow);
            var rowsGroupedByType2 = actualGroup2.RowsGroupedByType.ToArray();
            Assert.AreEqual(0, rowsGroupedByType2.Length);

            var actualGroup4 = actual[4];
            Assert.AreSame(_rows[7], actualGroup4.MasterRow);
            var rowsGroupedByType4 = actualGroup4.RowsGroupedByType.ToArray();
            Assert.AreEqual(0, rowsGroupedByType4.Length);
        }

        [TestMethod]
        public void BuildRowGroups_Given_a_row_without_a_master_row_Row_should_be_in_a_group_without_a_master_row()
        {
            var actual = _target.BuildRowGroups(_rows).ToArray(); ;
            TestContext.PrintJson(actual);

            var actualGroup3 = actual[3];
            Assert.IsNull(actualGroup3.MasterRow);
            var rowsGroupedByType3 = actualGroup3.RowsGroupedByType.ToArray();
            Assert.AreEqual(1, rowsGroupedByType3.Length);
            var list = rowsGroupedByType3[0].Value;
            Assert.AreEqual(1, list.Count);
            Assert.AreSame(_rows[6], list[0]);
        }

        [TestMethod]
        public void BuildRowGroups_Given_a_collection_of_rows_Correct_amount_of_groups_should_be_created()
        {
            var actual = _target.BuildRowGroups(_rows).ToArray(); ;
            TestContext.PrintJson(actual);

            Assert.AreEqual(5, actual.Length);
        }
    }

    public static class TestHelpers
    {
        public static void PrintJson(this TestContext testContext, object obj)
        {
            var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            testContext.Print(json);
        }

        public static void Print(this TestContext testContext, string message)
        {
            testContext.WriteLine(message);
        }
    }
}
