window.tabulator = {
    init10: (id, tabledData, columnInfo, subtableColumnInfo) => {
        try {
            table = new Tabulator(id, {
                data: tabledData,
                layout: "fitDataStretch",
                columns: columnInfo,
                rowFormatter: function (row) {
                    var data = row.getData();
                    if (!data.subtabledata) {
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

                    var subTable = new Tabulator(tableEl, {
                        headerVisible: false,
                        layout: "fitDataTable",
                        data: data.subtabledata,
                        columns: subtableColumnInfo
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
