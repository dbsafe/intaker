using FileValidator.Domain.Services;
using Microsoft.JSInterop;

namespace FileValidator.Blazor
{
    public class EditorManager
    {
        private readonly IJSInProcessRuntime _js;
        private readonly string _id;

        public static EditorManager CreateTextEditor(IJSRuntime js, string id, bool readOnly)
        {
            var editor = new EditorManager(js, id);
            editor.InitializeEditor("aceEditor.textEditorInit", readOnly);
            return editor;
        }

        public static EditorManager CreateXmlEditor(IJSRuntime js, string id, bool readOnly)
        {
            var editor = new EditorManager(js, id);
            editor.InitializeEditor("aceEditor.xmlEditorInit", readOnly);
            return editor;
        }

        public static EditorManager CreateJsonEditor(IJSRuntime js, string id, bool readOnly)
        {
            var editor = new EditorManager(js, id);
            editor.InitializeEditor("aceEditor.jsonEditorInit", readOnly);
            return editor;
        }

        private EditorManager(IJSRuntime js, string id)
        {
            _js = js as IJSInProcessRuntime;
            _id = id;
        }

        private void InitializeEditor(string method, bool readOnly)
        {
            _js.InvokeVoid(method, _id, readOnly);
        }

        public string GetValue()
        {
            return _js.Invoke<string>("aceEditor.editorGetValue", _id);
        }

        public void SetValue(string content)
        {
            _js.InvokeVoid("aceEditor.editorSetValue", _id, content);
        }

        public CursorPosition GetCursorPosition()
        {
            return _js.Invoke<CursorPosition>("aceEditor.getCursorPosition", _id);
        }

        public void MoveCursorToPosition(CursorPosition cursorPosition)
        {
            _js.Invoke<CursorPosition>("aceEditor.moveCursorToPosition", _id, cursorPosition);
        }

        public void Focus() => _js.InvokeVoid("aceEditor.focus", _id);

        public void SetReadOnly(bool readOnly) => _js.InvokeVoid("aceEditor.setReadOnly", _id, readOnly);
    }
}
