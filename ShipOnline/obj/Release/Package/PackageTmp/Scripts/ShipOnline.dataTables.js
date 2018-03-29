
function CreateDataTable(id, oSorting, oPaginate, oServerSide, aoColumnDefs, oScorllX, isSearchDialog, scrollY) {
    var param = {};

    param.oLanguage = {
        "sUrl": GetPath("/Content/dataTables_lang.txt")
    };
    param.bLengthChange = true;
    param.bInfo = true;
    if (oScorllX != null && oScorllX.bScorllX) {
        param.scrollX = oScorllX.bScorllX;
    }
    param.scrollCollapse = true;
    if (scrollY == true)
    {
        param.scrollY = "400px";
    }    
    param.sDom = (typeof (isSearchDialog) != 'undefined' || isSearchDialog) ? 't<"dialog-bottom"ipl>' : '<"tableHead"ipl>t';
    //param.sDom = 'Rlfrtip';
    //param.dom = 'Rlfrtip';    
    param.aLengthMenu = [5, 10,20,50, 100];
    param.bStateSave = oPaginate.bStateSave;
    param.stateDuration = -1;

    //param.fnStateSaveCallback = function (oSettings, oData) {
    //    if (window.location.pathname.indexOf('PMS06001') == -1) {
    //        if (oData["aaSorting"] != null) {
    //            oData["aaSorting"][0] = 0;
    //            oData["aaSorting"][1] = "asc";
    //        }

    //        if (oData["iStart"] != null) {
    //            oData["iStart"] = 0;
    //        }
    //    }

    //    $.ajax({
    //        "url": "/Common/SaveDataTableState",
    //        "data": { path: window.location.pathname, data: JSON.stringify(oData) },
    //        "dataType": "json",
    //        "method": "POST",
    //        "success": function() {
    //        }
    //    });
    //};

    //param.fnStateLoadCallback = function (oSettings) {
    //    var o;

    //    // Send an Ajax request to the server to get the data. Note that
    //    // this is a synchronous request.
    //    $.ajax({
    //        "url": "/Common/GetDataTableState",
    //        "data": { path : window.location.pathname },
    //        "async": false,
    //        "dataType": "json",
    //        "success": function (json) {
    //            o = json;
    //        }
    //    });

    //    if( o != null)
    //        return JSON.parse(o);
    //    else 
    //        return null;
    //};

    param.bProcessing = false;
    param.bFilter = false;
    
    if (oSorting != null) {
        param.bSort = oSorting.bSort;
        if (oSorting.bSort && oSorting.aaSorting != null) {
            param.aaSorting = oSorting.aaSorting;
        }
    }

    if (oPaginate != null && oPaginate.bPaginate != null) {
        param.bPaginate = oPaginate.bPaginate;
        if (oPaginate.bPaginate) {
            param.sPaginationType = "full_numbers";
            if (oPaginate.iDisplayLength) {
                param.iDisplayLength = oPaginate.iDisplayLength;
            }
            if (oPaginate.iDisplayStart) {
                param.iDisplayStart = oPaginate.iDisplayStart;
            }
        }
    }

    
    if (oServerSide != null && oServerSide.bServerSide != null) {
        param.bServerSide = oServerSide.bServerSide;
        if (oServerSide.bServerSide) {
            param.sAjaxSource = oServerSide.sAjaxSource;
            
            if (oServerSide.fnRowCallback != null) {
                param.fnRowCallback = oServerSide.fnRowCallback;
            }
            if (oServerSide.fnServerParams != null) {
                param.fnServerParams = oServerSide.fnServerParams;
            }

            param.fnServerData = function (sSource, aoData, fnCallback) {
                if (typeof (sort_colum) != 'undefined' && typeof (sort_type) != 'undefined') {
                    for (var i = 0; i < aoData.length; i++) {
                        if (aoData[i].name == 'iSortCol_0') {
                            aoData[i].value = sort_colum;
                            aoData[i + 1].value = sort_type;
                            break;
                        }
                    }
                }

                var ajaxParam = {};
                ajaxParam.dataType = 'json';
                ajaxParam.type = "POST";
                ajaxParam.async = true;
                ajaxParam.url = sSource;
                ajaxParam.data = aoData;

                if (oServerSide.fnBeforeSend) {
                    ajaxParam.beforeSend = oServerSide.fnBeforeSend;
                }

                if (oServerSide.fnDrawCallback) {
                    ajaxParam.complete = oServerSide.fnDrawCallback;
                }

                ajaxParam.success = function (data, status, xhr) {
                    if (data.ErrorMessages == null) {
                        if (oServerSide.fnInitComplete) {
                            oServerSide.fnInitComplete(data);
                        }
                        fnCallback(data, status, xhr);
                    }
                    else {
                        var errMess = "";
                        for (var i = 0; i < data.ErrorMessages.length; i++) {
                            errMess += data.ErrorMessages[i] + "<br />";
                        }

                        ShipOnline.utility.showMessageModal(errMess, true);
                    }
                }
                ajaxParam.error = function (err) {
                    window.location.href = '/Common/Common/Error/'; // giangvt comment
                }
                $.ajax(ajaxParam);
            }
        }
    }

    if (aoColumnDefs != null) {
        param.aoColumnDefs = aoColumnDefs;
    }    
    
    var dataTable = $(id).dataTable(param);
    return dataTable;
}
