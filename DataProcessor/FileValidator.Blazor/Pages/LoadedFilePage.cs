using DataProcessor.InputDefinitionFile;
using DataProcessor.InputDefinitionFile.Models;
using DataProcessor.Models;
using DataProcessor.Transformations;
using FileValidator.Domain.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;

namespace FileValidator.Blazor.Pages
{
    public partial class LoadedFilePage
    {
        private Row _headerRow;
        private Row _trailerRow;
        private IEnumerable<Row> _dataRows;
        private IEnumerable<DataRow20> _decodedDataRows20;
        private IEnumerable<DataRow20> _undecodedDataRows20;

        private RowDefinition _headerDefinition;
        private RowDefinition _trailerDefinition;
        private RowDefinition _dataDefinition10;
        private Datas _dataRowsDefinition20;

        private string _frameworkVersion;
        private ParsedDataAndSpec10 _parsedDataAndSpec10;
        private ParsedDataAndSpec20 _parsedDataAndSpec20;

        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        public LoadedFilePageState LoadedFilePageState { get; set; }

        [Inject]
        public ApplicationsEvents ApplicationsEvents { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ApplicationsEvents.MenuItemClicked += MenuItemClicked;

            _frameworkVersion = LoadedFilePageState.FrameworkVersion;
            if (_frameworkVersion != InputDefinitionFile10.VERSION && _frameworkVersion != InputDefinitionFile20.VERSION)
            {
                return;
            }

            if (_frameworkVersion == InputDefinitionFile10.VERSION)
            {
                _parsedDataAndSpec10 = LoadedFilePageState.ParsedDataAndSpec10;

                _headerRow = _parsedDataAndSpec10.ParsedData.Header;
                _trailerRow = _parsedDataAndSpec10.ParsedData.Trailer;
                _dataRows = _parsedDataAndSpec10.ParsedData.DataRows;

                _headerDefinition = _parsedDataAndSpec10.InputDefinitionFile.Header;
                _trailerDefinition = _parsedDataAndSpec10.InputDefinitionFile.Trailer;
                _dataDefinition10 = _parsedDataAndSpec10.InputDefinitionFile.Data;
            }

            if (_frameworkVersion == InputDefinitionFile20.VERSION)
            {
                _parsedDataAndSpec20 = LoadedFilePageState.ParsedDataAndSpec20;

                _headerRow = _parsedDataAndSpec20.ParsedData.Header;
                _trailerRow = _parsedDataAndSpec20.ParsedData.Trailer;
                _decodedDataRows20 = _parsedDataAndSpec20.ParsedData.DecodedDataRows;
                _undecodedDataRows20 = _parsedDataAndSpec20.ParsedData.UndecodedDataRows;

                _headerDefinition = _parsedDataAndSpec20.InputDefinitionFile.Header;
                _trailerDefinition = _parsedDataAndSpec20.InputDefinitionFile.Trailer;
                _dataRowsDefinition20 = _parsedDataAndSpec20.InputDefinitionFile.Datas;
            }
        }

        private void MenuItemClicked(object sender, MenuItemClickedEventArgs e)
        {
            Closing();
        }

        private void Closing()
        {
            ApplicationsEvents.MenuItemClicked -= MenuItemClicked;
        }
    }
}
