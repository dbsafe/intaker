using FileValidator.Domain.Services;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;

namespace FileValidator.Blazor.Pages
{
    public partial class FileSpecificationsPage
    {
        private IEnumerable<FileSpecificationOption> _fileSpecs;
        private EditorManager _editorManager;
        private bool _inEditMode;
        private FileSpecification _newFileSpecification;
        private bool _allowSelection = true;

        [Inject]
        public IFileSpecificationsStore FileSpecificationsStore { get; set; }
        
        [Inject]
        public IJSRuntime JS { get; set; }
        
        [Inject]
        public FileSpecificationsPageState FileSpecificationsPageState { get; set; }
        
        [Inject]
        public ApplicationsEvents ApplicationsEvents { get; set; }
        
        [Inject]
        public IMatToaster Toaster { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                _editorManager = EditorManager.CreateXmlEditor(JS, "editor", true);
                Opening();
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ApplicationsEvents.MenuItemClicked += MenuItemClicked;
            _fileSpecs = FileSpecificationsStore.GetAllFileSpecificationOptions().Data.OrderBy(a => a.Name);
        }

        public void SelectionChangedEvent(object row)
        {
            LoadFileSpecification(row as FileSpecificationOption);
        }

        private void LoadFileSpecification(FileSpecificationOption fileSpecificationOption)
        {
            if (fileSpecificationOption == null)
            {
                ClearSelectedFileSpecification();
                return;
            }

            if (FileSpecificationsPageState.SelectedFileSpecification != null && fileSpecificationOption.Id == FileSpecificationsPageState.SelectedFileSpecification.Id)
            {
                return;
            }

            LoadSelectedFileSpecification(fileSpecificationOption);
        }

        private void LoadSelectedFileSpecification(FileSpecificationOption fileSpecificationOption)
        {
            var getFileSpecificationByIdResult = FileSpecificationsStore.GetFileSpecificationById(fileSpecificationOption.Id);
            if (!getFileSpecificationByIdResult.Succeed)
            {
                ClearSelectedFileSpecification();
                Toaster.Add(getFileSpecificationByIdResult.Message, MatToastType.Danger);
                return;
            }

            FileSpecificationsPageState.SelectedFileSpecification = getFileSpecificationByIdResult.Data;
            _editorManager.SetValue(getFileSpecificationByIdResult.Data.Content);
            StateHasChanged();
        }

        private void ClearSelectedFileSpecification()
        {
            _editorManager.SetValue(string.Empty);
            FileSpecificationsPageState.SelectedFileSpecification = null;
            StateHasChanged();
        }

        private void MenuItemClicked(object sender, MenuItemClickedEventArgs e)
        {
            Closing();
        }

        private void Closing()
        {
            FileSpecificationsPageState.CursorPosition = _editorManager.GetCursorPosition();
            ApplicationsEvents.MenuItemClicked -= MenuItemClicked;
        }

        private void Opening()
        {
            if (FileSpecificationsPageState.SelectedFileSpecification != null)
            {
                _editorManager.SetValue(FileSpecificationsPageState.SelectedFileSpecification.Content);
                _editorManager.MoveCursorToPosition(FileSpecificationsPageState.CursorPosition);
                _editorManager.Focus();
                StateHasChanged();
            }
        }

        private void Edit()
        {
            _newFileSpecification = FileSpecificationsPageState.SelectedFileSpecification.Clone();
            _inEditMode = true;
            _allowSelection = false;
            _editorManager.SetReadOnly(!_inEditMode);
            _editorManager.Focus();
        }

        private void Save()
        {
            _newFileSpecification.Content = _editorManager.GetValue();
            
            var updateFileSpecificationResult = FileSpecificationsStore.UpdateFileSpecification(_newFileSpecification);
            if (updateFileSpecificationResult.Succeed)
            {
                FileSpecificationsPageState.SelectedFileSpecification = _newFileSpecification;

                Toaster.Add("File specification saved", MatToastType.Success);

                _inEditMode = false;
                _allowSelection = true;
                _editorManager.SetReadOnly(!_inEditMode);
                _editorManager.Focus();
            }
            else
            {
                Toaster.Add(updateFileSpecificationResult.Message, MatToastType.Danger);
            }
        }

        private void Cancel()
        {
            FileSpecificationsPageState.CursorPosition = _editorManager.GetCursorPosition();
            _editorManager.SetValue(FileSpecificationsPageState.SelectedFileSpecification.Content);
            _editorManager.MoveCursorToPosition(FileSpecificationsPageState.CursorPosition);
            _inEditMode = false;
            _allowSelection = true;
            _editorManager.SetReadOnly(!_inEditMode);
            _editorManager.Focus();
        }

        private string GetTitleForSelectedItem()
        {
            if (FileSpecificationsPageState.SelectedFileSpecification == null)
            {
                return string.Empty;
            }

            return $"{FileSpecificationsPageState.SelectedFileSpecification.Name} - {FileSpecificationsPageState.SelectedFileSpecification.Description}";
        }
    }
}
