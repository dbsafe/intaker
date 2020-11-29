using DataProcessor.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace FileValidator.Blazor.Components
{
    public partial class FileRowComponent
    {
        private List<DisplayData> _displayData;
        private IEnumerable<Row> _rows;

        [Parameter]
        public IEnumerable<string> ColumnHeaders { get; set; }

        [Parameter]
        public IEnumerable<Row> Rows 
        { 
            get { return _rows; }
            set
            {
                _rows = value;
                SetDisplayData();
            }
        }

        private void SetDisplayData()
        {
            _displayData = new List<DisplayData>();
            foreach (var row in _rows)
            {
                var rowspan = row.ValidationResult == ValidationResultType.Warning ? 2 : 1;
                _displayData.Add(new DisplayData { Row = row, IsRowData = true, Rowspan = rowspan });

                if (row.ValidationResult == ValidationResultType.Warning)
                {
                    _displayData.Add(new DisplayData { Row = row, Rowspan = 1 }); ;
                }
            }
        }

        private class DisplayData
        {
            public bool IsRowData { get; set; }
            public Row Row { get; set; }
            public int Rowspan { get; set; }
        }
    }
}
