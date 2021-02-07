window.aceEditor = {
    textEditorInit: (id, readOnly) => {
        try {
            var editor = ace.edit(id);
            editor.setTheme("ace/theme/chrome");
            editor.session.setMode("ace/mode/text");
            editor.setReadOnly(readOnly);
            editor.setOption("maxLines", 100);
            editor.setOption("minLines", 50);
            editor.setValue("");
        }
        catch (err) {
            console.log(err.message);
        }
    },

    xmlEditorInit: (id, readOnly) => {
        try {
            var editor = ace.edit(id);
            editor.setTheme("ace/theme/chrome");
            editor.session.setMode("ace/mode/xml");
            editor.setReadOnly(readOnly);
            editor.setOption("maxLines", 100);
            editor.setOption("minLines", 50);
            editor.setValue("");
        }
        catch (err) {
            console.log(err.message);
        }
    },

    jsonEditorInit: (id, readOnly) => {
        try {
            var editor = ace.edit(id);
            editor.setTheme("ace/theme/chrome");
            editor.session.setMode("ace/mode/json");
            editor.setReadOnly(readOnly);
            editor.setOption("maxLines", 100);
            editor.setOption("minLines", 50);
            editor.setValue("");
        }
        catch (err) {
            console.log(err.message);
        }
    },

    editorSetValue: (id, val) => {
        try {
            var el = document.getElementById(id);
            el.env.editor.session.setValue(val);
        }
        catch (err) {
            console.log(err.message);
        }
    },

    editorGetValue: (id) => {
        try {
            var el = document.getElementById(id);
            return el.env.editor.session.getValue();
        }
        catch (err) {
            console.log(err.message);
        }
    },

    getCursorPosition: (id) => {
        try {
            var el = document.getElementById(id);
            return el.env.editor.getCursorPosition();
        }
        catch (err) {
            console.log(err.message);
        }
    },

    moveCursorToPosition: (id, pos) => {
        try {
            var el = document.getElementById(id);
            return el.env.editor.moveCursorToPosition(pos);
        }
        catch (err) {
            console.log(err.message);
        }
    },

    focus: (id) => {
        try {
            var el = document.getElementById(id);
            el.env.editor.focus();
        }
        catch (err) {
            console.log(err.message);
        }
    },

    setReadOnly: (id, readOnly) => {
        try {
            var el = document.getElementById(id);
            el.env.editor.setReadOnly(readOnly);
        }
        catch (err) {
            console.log(err.message);
        }
    },
};
