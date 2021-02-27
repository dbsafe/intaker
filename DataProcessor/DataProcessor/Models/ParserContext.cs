using DataProcessor.Transformations;
using System.Collections.Generic;

namespace DataProcessor.Models
{
    public interface IParserContext
    {
        bool IsAborted { get; set; }
        string CurrentRowRaw { get; set; }
        bool IsCurrentRowTheLast { get; set; }
        int CurrentRowIndex { get; set; }
        string[] CurrentRowRawFields { get; set; }
        int InvalidDataRowCount { get; }
        IList<string> Errors { get; }
    }

    public class ParserContext<TRow> : IParserContext
    {
        public string CurrentRowRaw { get; set; }
        public string[] CurrentRowRawFields { get; set; }
        public int CurrentRowIndex { get; set; }
        public bool IsCurrentRowTheLast { get; set; }
        public bool IsAborted { get; set; }
        public ValidationResultType ValidationResult { get; set; }
        public IList<string> Errors { get; set; } = new List<string>();
        public IList<TRow> DataRows { get; set; } = new List<TRow>();
        public IList<TRow> InvalidDataRows { get; set; } = new List<TRow>();
        public Row Header { get; set; }
        public Row Trailer { get; set; }

        public int InvalidDataRowCount => InvalidDataRows.Count;
    }

    public class ParserContext10 : ParserContext<Row>
    {       
    }

    public class ParserContext20 : ParserContext<DataRow20>
    {
        public DataRow20 CurrentDataRow20 { get; set; }
        public int DataTypeFieldIndex { get; set; }
        public int DataKeyFieldIndex { get; set; }
        public IList<DataRow20> DataRowsWithUnknownTypes { get; set; } = new List<DataRow20>();
    }
}
