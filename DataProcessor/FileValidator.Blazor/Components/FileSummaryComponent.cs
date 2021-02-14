using FileValidator.Domain.Services;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace FileValidator.Blazor.Components
{
    public partial class FileSummaryComponent
    {
        private Model _model = new Model();
        private ParsedDataAndSpec10 _parsedDataAndSpec;

        [Parameter]
        public ParsedDataAndSpec10 ParsedDataAndSpec
        {
            get
            {
                return _parsedDataAndSpec;
            }
            set
            {
                _parsedDataAndSpec = value;
                if (_parsedDataAndSpec != null)
                {
                    SetPrivateProperties();
                }
                else
                {
                    Clear();
                }
            }
        }

        private void SetPrivateProperties()
        {
            _model.FileHasErrors = _parsedDataAndSpec.ParsedData.Errors.Count > 0;
            _model.Errors = _parsedDataAndSpec.ParsedData.Errors;
            _model.DataRowCount = _parsedDataAndSpec.ParsedData.DataRows.Count;
            _model.InvalidRowCount = _parsedDataAndSpec.ParsedData.InvalidDataRows.Count;
            _model.ValidationResult = _parsedDataAndSpec.ParsedData.ValidationResult.ToString();
        }

        private void Clear()
        {
            _model = new Model();
        }

        private class Model
        {
            public bool FileHasErrors { get; set; }
            public int DataRowCount { get; set; }
            public int InvalidRowCount { get; set; }
            public string ValidationResult { get; set; }
            public IEnumerable<string> Errors { get; set; }
        }
    }
}
