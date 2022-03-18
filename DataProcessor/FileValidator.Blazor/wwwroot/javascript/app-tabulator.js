window.tabulator = {
    displayEmptyRow: (row) => {
        var element = row.getElement();
        while (element.firstChild) {
            element.removeChild(element.firstChild);
        }

        var divElement = document.createElement("div");
        divElement.style.boxSizing = "border-box";
        divElement.style.padding = "10px 30px 10px 10px";
        divElement.style.borderTop = "1px solid #333";
        divElement.style.borderBotom = "1px solid #333";
        divElement.textContent = "Rows without a master row";

        element.appendChild(divElement);
    },
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

        // errorSubTable
        new Tabulator(tableEl, {
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

        // childrenRowsSubTable
        new Tabulator(tableEl, {
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

            new Tabulator(id, tabulatorData);
        }
        catch (err) {
            console.log(err.message);
        }
    },
    getColumnInfo: (data, columnInfos) => {
        if (data.length == 0)
            return null;
        return columnInfos[data[0].columnInfoIndex];
    },
    initDecodedRows20: (id, tableModel, errorsAndWarningsColumnInfo) => {
        try {
            var masterTabulatorData = {
                data: tableModel.tableData,
                layout: "fitDataStretch",
                columns: tabulator.getColumnInfo(tableModel.tableData, tableModel.columnInfos),
                rowFormatter: function (row) {
                    var data = row.getData();
                    if (data.lineNumber === undefined) {
                        tabulator.displayEmptyRow(row);

                    } else {
                        tabulator.displayErrorsAndWarnings(row, errorsAndWarningsColumnInfo);
                    }

                    tabulator.displayChildrenGroups(row, tableModel.columnInfos, errorsAndWarningsColumnInfo);
                }
            };
            new Tabulator(id, masterTabulatorData);
        }
        catch (err) {
            console.log(err.message);
        }
    },
    initUndecodedRows20: (id, tableModel) => {
        try {
            var masterTabulatorData = {
                data: tableModel.tableData,
                layout: "fitDataStretch",
                columns: tableModel.columnInfos
            };
            new Tabulator(id, masterTabulatorData);
        }
        catch (err) {
            console.log(err.message);
        }
    }
};
