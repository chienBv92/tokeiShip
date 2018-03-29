
function OnBlurAnkenNoFrom(fromid, toid) {
    var objFrom = document.getElementById(fromid);
    var objTo = document.getElementById(toid);

    if (objFrom.value != "") {

        objFrom.value = ("0000000" + objFrom.value).slice(-7);

        if (objTo.value == "") {
            objTo.value = objFrom.value;
        }
    }
}


function OnBlurAnkenNoTo(fromid, toid) {
    var objFrom = document.getElementById(fromid);
    var objTo = document.getElementById(toid);

    if (objTo.value != "") {
        objTo.value = ("0000000" + objTo.value).slice(-7);
    }

}

function OnBlurKingakuFormat(id) {
    var objTarget = document.getElementById(id);
    if (objTarget != null) {
        objTarget.value = FormatNumber(cmnPReplace(objTarget.value,",",""));
    }
}
function OnBlurSuryouFormat(id) {
    var objTarget = document.getElementById(id);
    if (objTarget != null) {
        objTarget.value = FormatNumber(cmnPReplace(objTarget.value,",",""));
    }
}

function FormatNumber(x) {
    var s = "" + x; 
    var p = s.indexOf(".");
    if (p < 0) { 
        p = s.length; 
    }
    var r = s.substring(p, s.length);
    for (var i = 0; i < p; i++) { 
        var c = s.substring(p - 1 - i, p - 1 - i + 1); 
        if (c < "0" || c > "9") { 
            r = s.substring(0, p - i) + r; 
            break;
        }
        if (i > 0 && i % 3 == 0) { 
            r = "," + r; 
        }
        r = c + r; 
    }
    return r;
}


function cmnPReplace(ValueString, TargetString, ReplaceString) {
    var ReturnString = ValueString;
    while (ReturnString.indexOf(TargetString) != -1) {
        ReturnString = ReturnString.replace(TargetString, ReplaceString);
    }
    return ReturnString;
}

function BeforeSubmitValidationErrorsClear() {
    $("#validationerror").empty();
}

$(function () {
    // Active current node on left menu
    if (window.location.pathname !== '/') {
        var path = window.location.pathname;
        $(".sidebar-small-menu, .main-sidebar").find("li").children("a").each(function () {
            var url = path == Constant.CONTRACT_LIST_NORMAL ? $(this).attr("data-url") : $(this).attr("href");
            if (path == url) {
                $(this).parents('.parent-node').addClass("active");
            }
        });
        $(".sidebar-small-menu, .main-sidebar").find("li").children("div").children("a").each(function () {
            var url = path == Constant.CONTRACT_LIST_NORMAL ? $(this).attr("data-url") : $(this).attr("href");            
            if (path == url) {
                $(this).parents('.parent-node').addClass("active");
            }
        });
    }

    // Common set today into text box
    $(document).off('.btnToday');
    $(document).on('click', '.btnToday', function () {
        var newDate = $.datepicker.formatDate("yy/mm/dd", new Date());

        $(this).parent().find('input:text').attr('value', newDate).val(newDate);
        $(this).parent().datepicker('update', newDate);
    });

    // Common disable button backspace to history back on browser
    $('html').on('keydown', function (event) {
        // if not focusing on textbox & textarea element
        if (!$(event.target).is('input:text') && !$(event.target).is('input:password') && !$(event.target).is('textarea')) {

            // keycode 8 is backspace, disable press
            if (event.which === 8) {
                return false;
            }
        }

        // if current browser is Firefox
        //if (ShipOnline.utility.checkBrowser() == 'Firefox') {
        //    // keycode 116 is F5 button
        //    if (event.keyCode === 116) {
        //        event.preventDefault();
        //        document.location.reload(true);
        //    }
        //}
    });

    $(document).off('#linkLogout');
    $(document).on('click', '#linkLogout', function () {
        ShipOnline.utility.showConfirmModal(Constant.CONFIRM_LOGOUT, function (action) {
            if (action) {
                window.location.href = '/UserManagement/Login/Logout';
            }
        });
    });
});