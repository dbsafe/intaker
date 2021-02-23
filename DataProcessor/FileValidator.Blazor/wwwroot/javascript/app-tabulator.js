window.tabulator = {
    displayErrorsAndWarnings: (row, errorsAndWarningsColumnInfo) => {
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
    displayChildrenGroup: (row, errorsAndWarningsColumnInfo) => {
        var data = row.getData();
        if (!data.childrenRows) {
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

        var childrenRowsSubTable = new Tabulator(tableEl, {
            headerVisible: false,
            layout: "fitDataTable",
            data: data.childrenRows,
            columns: data.columnInfo
        })
    },
    displayChildrenGroups: (row, errorsAndWarningsColumnInfo) => {
        var data = row.getData();
        if (!data.childrenGroups) {
            return;
        }

        var childrenGroups = data.childrenGroups;

        for (var i = 0; i < childrenGroups.length; i++) {
            var childrenGroup = childrenGroups[i];
            console.log(childrenGroup);
        }
    },
    init10: (id, tableModel, errorsAndWarningsColumnInfo) => {
        try {
            var tabulatorData = {
                data: tableModel.tableData,
                layout: "fitDataStretch",
                columns: tableModel.columnInfo,
                rowFormatter: function (row) {
                    tabulator.displayErrorsAndWarnings(row, errorsAndWarningsColumnInfo);
                }
            };

            table = new Tabulator(id, tabulatorData);
        }
        catch (err) {
            console.log(err.message);
        }
    },
    init20: (id, tableModel, errorsAndWarningsColumnInfo) => {
        try {
            var masterTabulatorData = {
                data: tableModel.tableData,
                layout: "fitDataStretch",
                columns: tableModel.masterColumnInfo,
                rowFormatter: function (row) {
                    tabulator.displayErrorsAndWarnings(row, errorsAndWarningsColumnInfo);
                    tabulator.displayChildrenGroups(row, errorsAndWarningsColumnInfo);
                }
            };
            table = new Tabulator(id, masterTabulatorData);
        }
        catch (err) {
            console.log(err.message);
        }
    }
};
