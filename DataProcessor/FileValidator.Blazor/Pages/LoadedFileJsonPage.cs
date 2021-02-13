using DataProcessor.InputDefinitionFile;
using FileValidator.Domain;
using FileValidator.Domain.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FileValidator.Blazor.Pages
{
    public partial class LoadedFileJsonPage
    {
        private EditorManager _editorManager;
        private string _frameworkVersion;
        private ParsedDataAndSpec10 _parsedDataAndSpec10;
        private ParsedDataAndSpec20 _parsedDataAndSpec20;

        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        public LoadedFilePageState LoadedFilePageState { get; set; }

        [Inject]
        public LoadedFileJsonPageState LoadedFileJsonPageState { get; set; }

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
            _frameworkVersion = LoadedFilePageState.FrameworkVersion;
        }

        private void Opening()
        {
            if (_frameworkVersion != InputDefinitionFile10.VERSION && _frameworkVersion != InputDefinitionFile20.VERSION)
            {
                return;
            }

            _editorManager = EditorManager.CreateJsonEditor(JS, "editor", true);
            if (_frameworkVersion == InputDefinitionFile10.VERSION)
            {
                _parsedDataAndSpec10 = LoadedFilePageState.ParsedDataAndSpec10;
                _editorManager.SetValue(_parsedDataAndSpec10.ParsedData.ToJson());
            }

            if (_frameworkVersion == InputDefinitionFile20.VERSION)
            {
                _parsedDataAndSpec20 = LoadedFilePageState.ParsedDataAndSpec20;
                _editorManager.SetValue(_parsedDataAndSpec20.ParsedData.ToJson());
            }

            if (LoadedFileJsonPageState.CursorPosition != null)
            {
                _editorManager.MoveCursorToPosition(LoadedFileJsonPageState.CursorPosition);
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
                LoadedFileJsonPageState.CursorPosition = _editorManager.GetCursorPosition();
            }

            ApplicationsEvents.MenuItemClicked -= MenuItemClicked;
        }
    }
}
