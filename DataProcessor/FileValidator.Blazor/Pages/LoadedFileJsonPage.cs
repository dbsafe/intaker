using FileValidator.Domain;
using FileValidator.Domain.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FileValidator.Blazor.Pages
{
    public partial class LoadedFileJsonPage
    {
        private EditorManager _editorManager;

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
        }

        private void Opening()
        {
            if (LoadedFilePageState.ParsedDataAndSpec10 != null)
            {
                _editorManager = EditorManager.CreateJsonEditor(JS, "editor", true);
                _editorManager.SetValue(LoadedFilePageState.ParsedDataAndSpec10.ParsedData.ToJson());
                if (LoadedFileJsonPageState.CursorPosition != null)
                {
                    _editorManager.MoveCursorToPosition(LoadedFileJsonPageState.CursorPosition);
                }

                _editorManager.Focus();
                StateHasChanged();
            }
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
