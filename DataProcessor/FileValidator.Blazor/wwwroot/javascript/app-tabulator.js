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
    displayChildrenGroup: (row, childrenRowGroup, columnInfos, errorsAndWarningsColumnInfo) => {
        if (!childrenRowGroup) {
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
            headerVisible: true,
            layout: "fitDataTable",
            data: childrenRowGroup,
            columns: tabulator.getColumnInfo(childrenRowGroup, columnInfos),
            rowFormatter: function (row) {
                tabulator.displayErrorsAndWarnings(row, errorsAndWarningsColumnInfo);
            }
        })
    },
    displayChildrenGroups: (row, columnInfos, errorsAndWarningsColumnInfo) => {
        var data = row.getData();
        if (!data.childrenRowGroups) {
            return;
        }

        for (var i = 0; i < data.childrenRowGroups.length; i++) {

            var childrenRowGroup = data.childrenRowGroups[i];
            tabulator.displayChildrenGroup(row, childrenRowGroup, columnInfos, errorsAndWarningsColumnInfo)
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
    getColumnInfo: (data, columnInfos) => {
        if (data.lenght == 0)
            return null;
        return columnInfos[data[0].columnInfoIndex];
    },
    init20: (id, tableModel, errorsAndWarningsColumnInfo) => {
        try {
            var masterTabulatorData = {
                data: tableModel.tableData,
                layout: "fitDataStretch",
                columns: tabulator.getColumnInfo(tableModel.tableData, tableModel.columnInfos),
                rowFormatter: function (row) {
                    tabulator.displayErrorsAndWarnings(row, errorsAndWarningsColumnInfo);
                    tabulator.displayChildrenGroups(row, tableModel.columnInfos, errorsAndWarningsColumnInfo);
                }
            };
            table = new Tabulator(id, masterTabulatorData);
        }
        catch (err) {
            console.log(err.message);
        }
    }
};
