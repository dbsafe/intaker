using DataProcessor;
using DataProcessor.InputDefinitionFile;
using FileValidator.Domain;
using FileValidator.Domain.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FileValidator.Blazor.Pages
{
    public partial class LoadedFileGroupsJsonPage
    {
        private EditorManager _editorManager;
        private ParsedData20 _parsedData;
        private InputDefinitionFile20 _inputDefinitionFile;

        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        public LoadedFilePageState LoadedFilePageState { get; set; }

        [Inject]
        public LoadedFileGroupsJsonPageState LoadedFileGroupJsonPageState { get; set; }

        [Inject]
        public ApplicationsEvents ApplicationsEvents { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                Opening();
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ApplicationsEvents.MenuItemClicked += MenuItemClicked;

            if (LoadedFilePageState.ParsedDataAndSpec20 != null)
            {
                _parsedData = LoadedFilePageState.ParsedDataAndSpec20.ParsedData;
                _inputDefinitionFile = LoadedFilePageState.ParsedDataAndSpec20.InputDefinitionFile;
            }
        }

        private void Opening()
        {
            if (_parsedData == null)
            {
                return;
            }

            _editorManager = EditorManager.CreateJsonEditor(JS, "editor", true);

            var groups = GroupHelper.BuildRowGroups(_parsedData.DataRows, _inputDefinitionFile.Datas);
            _editorManager.SetValue(groups.ToJson());

            if (LoadedFileGroupJsonPageState.CursorPosition != null)
            {
                _editorManager.MoveCursorToPosition(LoadedFileGroupJsonPageState.CursorPosition);
            }

            _editorManager.Focus();
            StateHasChanged();
        }

        private void MenuItemClicked(object sender, MenuItemClickedEventArgs e)
        {
            Closing();
        }

        private void Closing()
        {
            if (_editorManager != null)
            {
                LoadedFileGroupJsonPageState.CursorPosition = _editorManager.GetCursorPosition();
            }

            ApplicationsEvents.MenuItemClicked -= MenuItemClicked;
        }
    }
}
