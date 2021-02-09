using DataProcessor.InputDefinitionFile.Models;
using DataProcessor.Models;
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

        private RowDefinition _headerDefinition;
        private RowDefinition _trailerDefinition;
        private RowDefinition _dataDefinition;

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

            if (LoadedFilePageState.ParsedDataAndSpec10 != null)
            {
                _headerRow = LoadedFilePageState.ParsedDataAndSpec10.ParsedData.Header;
                _trailerRow = LoadedFilePageState.ParsedDataAndSpec10.ParsedData.Trailer;
                _dataRows = LoadedFilePageState.ParsedDataAndSpec10.ParsedData.DataRows;

                _headerDefinition = LoadedFilePageState.ParsedDataAndSpec10.InputDefinitionFile.Header;
                _trailerDefinition = LoadedFilePageState.ParsedDataAndSpec10.InputDefinitionFile.Trailer;
                _dataDefinition = LoadedFilePageState.ParsedDataAndSpec10.InputDefinitionFile.Data;
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
