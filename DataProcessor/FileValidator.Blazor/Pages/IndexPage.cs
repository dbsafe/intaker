﻿using DataProcessor.Models;
using FileValidator.Domain.Services;
using MatBlazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileValidator.Blazor.Pages
{
    public partial class IndexPage
    {
        private readonly Model _model = new Model();
        private EditorManager _editorManager;

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        [Inject]
        public IFileSpecificationsStore FileSpecificationsStore { get; set; }

        [Inject]
        public IFileDecoder FileDecoder { get; set; }

        [Inject]
        public IJSRuntime JS { get; set; }

        [Inject]
        public IMatToaster Toaster { get; set; }

        [Inject]
        public HomePageState HomePageState { get; set; }

        [Inject]
        public LoadedFilePageState LoadedFilePageState { get; set; }

        [Inject]
        public ApplicationsEvents ApplicationsEvents { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                _editorManager = EditorManager.CreateTextEditor(JS, "editor", false);
                Opening();
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            ApplicationsEvents.MenuItemClicked += MenuItemClicked;
            _model.Options = FileSpecificationsStore.GetAllFileSpecificationOptions().Data.OrderBy(a => a.Name);

            if (HomePageState.SelectedFileSpecId > 0)
            {
                _model.SelectedFileSpecId = HomePageState.SelectedFileSpecId;
            }
            else
            {
                if (_model.Options.Any())
                {
                    _model.SelectedFileSpecId = _model.Options.First().Id;
                }
            }
        }

        private void MenuItemClicked(object sender, MenuItemClickedEventArgs e)
        {
            Closing();
        }

        private async Task FilesReadyAsync(IMatFileUploadEntry[] files)
        {
            var file = files.FirstOrDefault();
            if (file == null)
            {
                return;
            }

            string content = string.Empty;
            using (var stream = new MemoryStream())
            {
                await file.WriteToStreamAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(stream);
                content = await reader.ReadToEndAsync();
            }

            _editorManager.SetValue(content);
        }

        private void Decode(MouseEventArgs e)
        {
            var content = _editorManager.GetValue();

            if (string.IsNullOrEmpty(content))
            {
                Toaster.Add("Content is empty", MatToastType.Warning);
                return;
            }

            if (_model.SelectedFileSpecId == -1)
            {
                Toaster.Add("A file definition must be selected", MatToastType.Warning);
                return;
            }

            var getFileSpecificationResult = FileSpecificationsStore.GetFileSpecificationById(_model.SelectedFileSpecId);
            if (!getFileSpecificationResult.Succeed)
            {
                Toaster.Add(getFileSpecificationResult.Message, MatToastType.Warning);
                return;
            }

            DecodeContent(content, getFileSpecificationResult.Data.Content);
        }

        private void DecodeContent(string content, string fileSpecXml)
        {
            _model.ProgressDialogIsOpen = true;
            StateHasChanged();
            try
            {
                LoadedFilePageState.ParsedDataAndSpec = FileDecoder.Load(content, fileSpecXml);

                switch (LoadedFilePageState.ParsedDataAndSpec.ParsedData.ValidationResult)
                {
                    case ValidationResultType.Valid:
                        Toaster.Add("Validation succeed", MatToastType.Success);
                        break;
                    case ValidationResultType.Warning:
                        Toaster.Add("Validation succeed with warnings", MatToastType.Warning);
                        break;
                    default:
                        Toaster.Add("Validation failed", MatToastType.Danger);
                        return;
                }

                Closing();
                NavigationManager.NavigateTo("loaded-file");
            }
            catch (Exception ex)
            {
                Toaster.Add(ex.Message, MatToastType.Danger);
                Console.WriteLine($"Error: {ex}");
            }
            finally
            {
                _model.ProgressDialogIsOpen = false;
                StateHasChanged();
            }
        }

        private void Closing()
        {
            HomePageState.CursorPosition = _editorManager.GetCursorPosition();
            HomePageState.InputDataContent = _editorManager.GetValue();
            HomePageState.SelectedFileSpecId = _model.SelectedFileSpecId;

            ApplicationsEvents.MenuItemClicked -= MenuItemClicked;
        }

        private void Opening()
        {
            if (!string.IsNullOrEmpty(HomePageState.InputDataContent))
            {
                _editorManager.SetValue(HomePageState.InputDataContent);
                _editorManager.MoveCursorToPosition(HomePageState.CursorPosition);
                _editorManager.Focus();
            }
        }

        private class Model
        {
            public int SelectedFileSpecId = -1;
            public bool ProgressDialogIsOpen = false;
            public IEnumerable<FileSpecificationOption> Options;
        }
    }
}