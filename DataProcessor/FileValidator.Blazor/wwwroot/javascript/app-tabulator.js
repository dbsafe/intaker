window.tabulator = {
    showErrorsAndWarnings: (row, errorsAndWarningsColumnInfo) => {
        var data = row.getData();
        if (!data.errorsAndWarnings) {
            return;
        }

        var holderEl = document.createElement("div");
        var tableEl = document.createElement("div");

        holderEl.style.boxSizing = "border-box";
        holderEl.style.padding = "10px 30px 10px 10px";
        holderEl.style.borderTop = "1px solid #333";
        holderEl.style.borderBotom = "1px solid #333";
        holderEl.style.background = "#ddd";

        tableEl.style.border = "1px solid #333";

        holderEl.appendChild(tableEl);

        row.getElement().appendChild(holderEl);

        var errorSubTable = new Tabulator(tableEl, {
            headerVisible: false,
            layout: "fitDataTable",
            data: data.errorsAndWarnings,
            columns: errorsAndWarningsColumnInfo
        })
    },
    init10: (id, tabledData, columnInfo, errorsAndWarningsColumnInfo) => {
        try {
            table = new Tabulator(id, {
                data: tabledData,
                layout: "fitDataStretch",
                columns: columnInfo,
                rowFormatter: function (row) {
                    var data = row.getData();
                    if (!data.errorsAndWarnings) {
                        return;
                    }

                    var holderEl = document.createElement("div");
                    var tableEl = document.createElement("div");

                    holderEl.style.boxSizing = "border-box";
                    holderEl.style.padding = "10px 30px 10px 10px";
                    holderEl.style.borderTop = "1px solid #333";
                    holderEl.style.borderBotom = "1px solid #333";
                    holderEl.style.background = "#ddd";

                    tableEl.style.border = "1px solid #333";

                    holderEl.appendChild(tableEl);

                    row.getElement().appendChild(holderEl);

                    var errorSubTable = new Tabulator(tableEl, {
                        headerVisible: false,
                        layout: "fitDataTable",
                        data: data.errorsAndWarnings,
                        columns: errorsAndWarningsColumnInfo
                    })
                }
            });
        }
        catch (err) {
            console.log(err.message);
        }
    },
    init20: (id, tabledData, columnInfo) => {
        try {
            table = new Tabulator(id, {
                data: tabledData,
                layout: "fitDataStretch",
                columns: columnInfo,
            });
        }
        catch (err) {
            console.log(err.message);
        }
    }
};
