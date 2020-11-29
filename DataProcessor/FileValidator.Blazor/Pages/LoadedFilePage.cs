using DataProcessor.Models;
using FileValidator.Domain.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;

namespace FileValidator.Blazor.Pages
{
    public partial class LoadedFilePage
    {
        private Row _headerRow;
        private Row _trailerRow;
        private IEnumerable<Row> _dataRows;
        private IEnumerable<string> _headerColumns;
        private IEnumerable<string> _trailerColumns;
        private IEnumerable<string> _dataColumns;

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

            if (LoadedFilePageState.ParsedDataAndSpec != null)
            {
                _headerRow = LoadedFilePageState.ParsedDataAndSpec.ParsedData.Header;
                _trailerRow = LoadedFilePageState.ParsedDataAndSpec.ParsedData.Trailer;
                _dataRows = LoadedFilePageState.ParsedDataAndSpec.ParsedData.DataRows;

                _headerColumns = LoadedFilePageState.ParsedDataAndSpec.InputDefinitionFile.Header.Fields.Select(a => a.Name);
                _trailerColumns = LoadedFilePageState.ParsedDataAndSpec.InputDefinitionFile.Trailer.Fields.Select(a => a.Name);
                _dataColumns = LoadedFilePageState.ParsedDataAndSpec.InputDefinitionFile.Data.Fields.Select(a => a.Name);
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
