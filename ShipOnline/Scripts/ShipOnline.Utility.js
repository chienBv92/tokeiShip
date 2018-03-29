//define by ChienBV


var ShipOnline = ShipOnline || {};

ShipOnline.utility = (function () {

    function fixedEncodeURIComponent(str) {
        return encodeURIComponent(str).replace(/[!'()*]/g, function (c) {
            return '%' + c.charCodeAt(0).toString(16);
        });
    }

    //display multiline attribute
    function displayMultiline() {
        $('.display-multiline').each(function () {
            $(this).html($(this).text().replace(/\n/g, '<br />'));
        });
    }

    // delete "-" characters at the begining
    function deleteHyphenCharacters(groupName) {
        var groupNameLength = groupName.length;
        while (groupName.charAt(0) == '-') {
            groupName = groupName.substring(1, groupNameLength);
        }
        return groupName;
    }

    function displayGroupName() {
        setTimeout(function () {
            $('.ddl-group-id :selected').each(function () {
                var currentGroupName = $(this).text();
                var resultGroupName = deleteHyphenCharacters(currentGroupName);

                // Contract View screen
                if ($('#contractView').length != 0) {
                    $(this).parents('.content').find('.display-ddl-group-id').text(resultGroupName).addClass('txt-width-250');
                }

                // Estimate View screen
                if ($('#estimateView').length != 0) {
                    $('.ddl-group-id').parent().find('.display-ddl-group-id').text(resultGroupName);
                }
            });
        }, 500);
    }

    function formatJSONDate(jsonDate) {
        return $.datepicker.formatDate('yy年mm月dd日', new Date(parseInt(jsonDate.substr(6))));
    }

    function formatDateFromJson() {
        return $.datepicker.formatDate('yy/mm/dd', new Date(parseInt(jsonDate.substr(6))));
    }

    function formatDateYearMonth(takeDate) {
        
        return takeDate.split('/')[1] + '/' + takeDate.split('/')[0] + '/' + takeDate.split('/')[2];
    }

    // Format Time: decimal number to {hour:minute}
    function formatMinute(x) {
        if (x == "" || x == 0) {
            return "00";
        } else if (x < 10) {
            return '0' + x;
        } else {
            return '' + x;
        }
    }

    function formatHour(x) {
        if (x == "" || x == 0) {
            return "00";
        }
        else if (x >= 10 || x <= -10) {
            return x;
        } else if (x < 10 && x > 0) {
            return '0' + x;
        } else if (x < 0 && x > -10) {
            return '-0' + x * (-1);
        }
    }

    function formatDecimalToTime(inputTime)
    {
        inputTime = Math.round(inputTime * 100) / 100;
        var minActual = Math.round(Math.floor(inputTime) == 0 && inputTime < 0
                            ? ((Math.floor(inputTime) + 1) - inputTime) * 60
                            : (inputTime - Math.floor(inputTime)) * 60);
        minActual = minActual != 0 && inputTime < 0 ? 60 - minActual : minActual;

        var hourActual = '00';
        if (minActual != 0 && inputTime < 0) {
            hourActual = Math.floor(inputTime) + 1;
        } else {
            hourActual = Math.floor(inputTime);
        }

        var resultHour = (hourActual == 0 && inputTime < 0 ? '-' + formatHour(hourActual) : formatHour(hourActual));
        var resultMin = formatMinute(minActual);

        return resultHour + '<span class="space_total">:</span>' + resultMin;
    }

    function focusTextbox() {
        $(document).off('input:text, textarea, input:password');
        $(document).on('focus', 'input:text, textarea, input:password', function () {
            $(this).focus(function () { $(this).select(); })
            $(this).mouseup(function (e) {
                e.preventDefault();
                $(this).unbind(e.type);
            });
        });
    }

    function setFocusAmount(control)
    {
        if ($(control).is(":focus")) {
            $(control).focus();
            $(control).select();
        }
    }

    // Format money string to integer
    function convertMoneyToInt(input, canNegative) {
        if (input.length == 0)
            return 0;

        var strValue = input.replace(/,/g, '');
        var intValue = validPositiveNumeric(strValue) ? parseInt(strValue) : 0;

        if (typeof (canNegative) !== "undefined" && canNegative == true)
            intValue = validNegativeNumeric(strValue) ? parseInt(strValue) : 0;

        return intValue;
    }

    // Format integer to money string with symbol ','
    function convertIntToMoney(input) {
        var result = '0';

        if ($.isNumeric(input)) {
            result = parseInt(input).toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,");
        }

        return result;
    }

    // Replace all symbol ',' in money
    function replaceAllMoney($targetContent) {
        var $targetElement = $('.money, .negative-money, .negative-money-max');

        if (typeof ($targetContent) != 'undefined') {
            $targetElement = $targetContent.find('.money, .negative-money, .negative-money-max');
        }

        $targetElement.each(function () {
            var data = $(this).val();
            if (data.length > 0) {
                $(this).val(data.replace(/,/g, ''));
            }
        });
    }

    // Format money in textbox to string with symbol ','
    function formatMoney(obj) {
        obj = obj != null ? obj : '.money, .negative-money, .negative-money-max';

        $(obj).each(function () {
            var data = convertMoneyToInt($(this).val(), true);
            var money = convertIntToMoney(data);

            $(this).val(money);
        });
    }

    // Format money in label to string with symbol ','
    function formatMoneyLabel() {
        $('label.lbl-money, label.money').each(function () {
            var data = convertMoneyToInt($(this).text(), true);
            var money = convertIntToMoney(data);

            $(this).text(money);
        });
    }

    // Check isvalid date
    // Return true if valid, false if invalid
    function isValidDate(date) {
        var bits = date.split('/');
        var checkingDate = new Date(bits[0], bits[1] - 1, bits[2]);
        return checkingDate && (checkingDate.getMonth() + 1) == bits[1] && checkingDate.getDate() == Number(bits[2]);
    }

    // Check validation of date field
    // date is input data
    // format is datetime type (yyyy/mm/dd or yyyy/mm)
    // control is field name
    // return invalid message if invalid, null if valid
    function validDate(date, format, control) {
        var constantErr = Constant.ERR_FORMAT_DATE;
        if (date.length > 0) {
            if (format == 'yyyy/mm') {
                date += '/01';
                constantErr = Constant.ERR_FORMAT_YM;
            }

            if (!/^[0-9]{4}\/[0-9]{1,2}\/[0-9]{1,2}/.test(date) || !isValidDate(date)) {
                return control + constantErr;
            }

            var year = parseInt(date.split('/')[0]);

            if (Constant.MIN_YEAR > year || year > Constant.MAX_YEAR) {
                if (format == 'yyyy/mm') {
                    return control + Constant.ERR_INCORRECT_DATE;
                }
                return control + Constant.ERR_INCORRECT_DATE;
            }
        }

        return null;
    }

    // Compare date custom ChienBv
    function compareDateCustom(startDate, endDate, format) {
        var startYear = startDate.split('/')[2];
        var startMonth = startDate.split('/')[1];
        var startDay = startDate.split('/')[0];

        var endYear = endDate.split('/')[2];
        var endMonth = endDate.split('/')[1];
        var endDay = endDate.split('/')[0];

        var valid = true;
        if (startYear > endYear) {
            valid = false;
        } else if (startYear == endYear) {
            if (startMonth > endMonth) {
                valid = false;
            } else if (startMonth == endMonth) {
                if (startDay > endDay) {
                    valid = false;
                }
            }
        }

        return valid;
    }

    // Compare startDate with endDate
    // Return true if valid, false if invalid
    function compareDate(startDate, endDate, format) {
        var start = $.datepicker.formatDate('dd/mm/yy', new Date(startDate));
        var end = $.datepicker.formatDate('dd/mm/yy', new Date(endDate));

        if (format == 'mm/yyyy') {
            start = $.datepicker.formatDate('dd/mm/yy', new Date(startDate + '/01'));
            end = $.datepicker.formatDate('dd/mm/yy', new Date(endDate + '/01'));
        }

        var valid = true;
        if (start > end) {
            valid = false;
        }

        return valid;
    }

    // Compare startDate with endDate
    // Return true if valid, false if invalid
    function compareDateRange(startDate, endDate, rangeMonth) {
        var start = new Date(startDate + '/01');
        var end = new Date(endDate + '/01');

        var valid = true;

        if ((end.getMonth() + end.getFullYear() * 12 - start.getMonth() - start.getFullYear() * 12) > rangeMonth) {
            valid = false;
        }

        return valid;
    }

    // Check range of a duration time
    function validateRangeYear(startYear, endYear, rangeYear) {
        var arrStart = startYear.split('/');
        var arrEnd = endYear.split('/');
        var sYear = parseInt(arrStart[0]);
        var sMonth = parseInt(arrStart[1]);
        var eYear = parseInt(arrEnd[0]);
        var eMonth = parseInt(arrEnd[1]);

        if ((eYear - sYear) * 12 + (eMonth - sMonth) > rangeYear) {
            return false;
        }
        return true;
    }

    // Check end date on range start date to (startdate + 11month)
    function validateRangeMonth(startYear, endYear) {
        var valid = true;
        var arrStart = startYear.split('/');
        var arrEnd = endYear.split('/');
        var sYear = parseInt(arrStart[0]);
        var sMonth = parseInt(arrStart[1]);
        var eYear = parseInt(arrEnd[0]);
        var eMonth = parseInt(arrEnd[1]);

        if (((eYear - sYear) === 1 && eMonth > (sMonth - 1) || ((eYear - sYear) > 1))) {
            valid = false;
        }
        return valid;
    }

    function onChangeStartAndEndDateContract() {
        var startDate = $('#CONTRACT_PRJ_PERIOD_START').val();
        var endDate = $('#CONTRACT_PRJ_PERIOD_END').val();
        var errInvalid;
        if (startDate.length === 0) {
            return;
        }
        if (endDate.length === 0) {
            return;
        }
        errInvalid = ShipOnline.utility.validDate(startDate, Constant.DATE_FORMAT, Constant.WORK_PERIOD_START);
        if (errInvalid != null) {
            ShipOnline.utility.showMessageModal(errInvalid, true);
            return;
        }

        if (startDate.length > Constant.MAX_DATE) { // valid string length
            errInvalid = Constant.WORK_PERIOD_START + Constant.ERR_INCORRECT_DATE;
            ShipOnline.utility.showMessageModal(errInvalid, true);
            return;
        }

        if (endDate.length > Constant.MAX_DATE) { // valid string length
            errInvalid = Constant.WORK_PERIOD_END + Constant.ERR_INCORRECT_DATE;
            ShipOnline.utility.showMessageModal(errInvalid, true);
            return;
        }

        var errInvalidEndDate = ShipOnline.utility.validDate(endDate, Constant.DATE_FORMAT, Constant.WORK_PERIOD_END);
        if (errInvalidEndDate != null) {
            ShipOnline.utility.showMessageModal(errInvalidEndDate, true);
            return;
        }

        var errInvalidStartDate = ShipOnline.utility.validDate(endDate, Constant.DATE_FORMAT, Constant.WORK_PERIOD_START);
        if (errInvalidStartDate != null) {
            ShipOnline.utility.showMessageModal(errInvalidStartDate, true);
            return;
        }

        if (errInvalidEndDate == null) {
            if (endDate.length > 0 && !ShipOnline.utility.compareDate(startDate, endDate, Constant.DATE_FORMAT)) {
                ShipOnline.utility.showMessageModal(Constant.ERR_COMPARE_TOW_DATE.replace('{0}', Constant.WORK_PERIOD_END).replace('{1}', Constant.WORK_PERIOD_START), true);
                return;
            }
        }
        if (errInvalidStartDate == null) {
            if (endDate.length > 0 && !ShipOnline.utility.compareDate(startDate, endDate, Constant.DATE_FORMAT)) {
                ShipOnline.utility.showMessageModal(Constant.ERR_COMPARE_TOW_DATE.replace('{0}', Constant.WORK_PERIOD_END).replace('{1}', Constant.WORK_PERIOD_START), true);
                return;
            }
        }
    }

    function validateDeliveryDate() {
        var deliveryDate = $('#CONTRACT_DELIVERY_DATE').val();
        var errInvalid;

        errInvalid = ShipOnline.utility.validDate(deliveryDate, Constant.DATE_FORMAT, Constant.DELIVERY_DATE);

        if (errInvalid != null) {
            ShipOnline.utility.showMessageModal(errInvalid, true);
            return;
        }

        if (deliveryDate.length > Constant.MAX_DATE) { // valid string length
            errInvalid = Constant.DELIVERY_DATE + Constant.ERR_INCORRECT_DATE;
            ShipOnline.utility.showMessageModal(errInvalid, true);
            return;
        }
    }

    // Check alphanumeric only
    function validAlphanumeric(input) {
        return /^[a-zA-Z0-9]*$/.test(input);
    }

    // Check positive number only
    function validPositiveNumeric(value) {
        var data = value.trim().length == 0 ? '0' : value;

        if (data.indexOf('-') !== -1 || data.indexOf('.') !== -1 || !$.isNumeric(data)) {
            return false;
        }

        return true;
    }

    // Check negative number only
    function validNegativeNumeric(input) {
        if (input.indexOf('.') !== -1 || !$.isNumeric(input)) {
            return false;
        }
        return true;
    }

    // Check decimal number only
    function validDecimalNumeric(input) {
        if (input.indexOf('-') !== -1 || input.indexOf(',') !== -1 || !$.isNumeric(input)) {
            return false;
        }
        return true;
    }

    function validDecimalByType(value, type) {
        var data = value.trim().length == 0 ? '0' : value;

        if (validDecimalNumeric(data)) {
            var arr = data.toString().split('.');
            var positivePartMaxLength = type == Constant.DECIMAL_4_2 ? 2 : (type == Constant.DECIMAL_5_2 ? 3: 5);

            if (arr[0].length == 0
                || arr[0].length > positivePartMaxLength
                || (typeof (arr[1]) != 'undefined' && (arr[1].length == 0 || arr[1].length > 2))) {
                return false;
            }
        } else {
            return false;
        }

        return true;
    }

    // Show client error message on the top of page
    function showClientError(errMessage, position, isPopup) {
        position = typeof (position) === 'undefined' ? '#title' : position;

        $('.validation-summary-errors').remove();
        var html = '<div class="validation-summary-errors"><ul>';

        for (var i = 0; i < errMessage.length; i++) {
            html += '<li class="first last">' + errMessage[i] + '</li>';
        }
        html += '</ul></div>';
        $(position).after(html);
        if (isPopup) {
            $('#showPopup .main-popup').scrollTop(0);
        } else {
            $(document).scrollTop(0);
        }

        return;
    }

    // Check user acount and password
    function validAccount(input) {
        var re = Constant.REGEX_PASSWORD;
        return !re.test(input);
    }

    // Check numeric only
    function validNumeric(input) {
        return /^[0-9]+$/.test(input);
    }

    // Check phone number only
    function validPhone(input) {
        return /^[0-9/-]+$/.test(input);
    }

    // Check URL
    function validURL(input) {
        var re = /^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$/;
        return re.test(input);
    }
    
    // Check email
    function validEmail(input) {
        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        return re.test(input);
    }

    //control fullsize and half size
    function validFullHalfSize(control) {
        control.on("change keyup", function (e) {
            var text = $(this).val();
            var regX = /[^a-zA-Z0-9\!\""\#\$\%\&\'\(\)\=\~\|\-\^\@@\[\;\:\]\,\.\/\`\{\+\*\}\>\?\<\'\?\/\\\_\ ]/g;
            if (regX.test(text)) {
                text = text.replace(regX, '');
                $(this).val(text);
            }
        });
    }

    //Check fullsize and half size
    function CheckValidFullHalfSize(value) {
        var regX = /[^a-zA-Z0-9\!\""\#\$\%\&\'\(\)\=\~\|\-\^\@@\[\;\:\]\,\.\/\`\{\+\*\}\>\?\<\'\?\/\\\_\ ]/g;

        if (regX.test(value)) {
            return false;
        }

        return true;
    }

    function checkInputNumber(control) {
        control.on("change keyup", function (e) {
            var text = control.val();
            var regX = /[^0-9]/g;
            if (regX.test(text)) {
                control.val(text.replace(regX, ''));
            }
        });
    }

    function checkInputPhone(control) {
        control.on("change keyup", function (e) {
            var text = control.val();
            var regX = /[^0-9/-]/g;
            if (regX.test(text)) {
                control.val(text.replace(regX, ''));
            }
        });
    }

    function checkCompareBasetime(basetimelower, basetimeupper, paymentMethodType) {
        // Check payment method Type 
        if (paymentMethodType != '1') // not 時間幅精算
        {
            return null;
        }
        var _basetimelower = basetimelower.length > 0 ? parseFloat(basetimelower.trim()) : null;
        var _basetimeupper = basetimeupper.length > 0 ? parseFloat(basetimeupper.trim()) : null;

        if (_basetimelower <= _basetimeupper) {
            return null;
        }
        return Constant.ERR_COMPARE_BASETIME;
    }

    // Check TargetYM is not out of range of project duration
    function isValidTargetYM(startDate, endDate, targetYm, closingDay) {
        var targetYmDate;
        if (closingDay == ClosingDay.Day31) {
            targetYm += '/01';
            targetYmDate = new Date(targetYm);
            targetYmDate.setMonth(targetYmDate.getMonth() + 1);
            targetYmDate.setDate(targetYmDate.getDate() - 1);
        } else {
            targetYm += '/' + (closingDay.length == 1 ? "0" + closingDay : closingDay);
            targetYmDate = new Date(targetYm);
        }
        if ((new Date(targetYm).getYear() > new Date(endDate).getYear()) && new Date(targetYm).getMonth() == 00 && new Date(endDate).getMonth() == 11 && closingDay < new Date(endDate).getDate()) {
            return true;
        }
        else if (targetYmDate < new Date(startDate)
            || (new Date(targetYm).getYear() > new Date(endDate).getYear())
            || (new Date(targetYm).getYear() == new Date(endDate).getYear()
                && new Date(targetYm).getMonth() - new Date(endDate).getMonth() > 1)
            || (new Date(targetYm).getYear() == new Date(endDate).getYear()
                && new Date(targetYm).getMonth() - new Date(endDate).getMonth() == 1 && closingDay >= new Date(endDate).getDate())) {
            return  false;
        }

        return true;
    }

    // Encode html to string
    function htmlEncode(value) {
        return $('<div/>').text(value).html();
    }

    // Decode string to html
    function htmlDecode(value) {
        return $('<div/>').html(value).text();
    }

    // Rounding decimal data


    function isInteger(num) {
        return (num ^ 0) === num;
    }

    // Round money by unit
    function roundMoneyUnit(roundUnit, value) {
        if ($.isNumeric(roundUnit)) {
            roundUnit = parseFloat(Math.pow(10, parseInt(roundUnit)));

            var tempValue = value / roundUnit;

            value = Math.floor(tempValue) * roundUnit;
        }

        return value;
    }

    // Rounding time (hours) by unit (minute)
    function roundTimeByUnit(value, unit) {

        // minute overbalance
        var minute = (value % 1 * 60).toFixed(0);

        if (minute !== 0 && unit !== 0 && minute !== unit && minute % unit !== 0) {

            // return value by cut all decimal of working time
            if (minute < unit) {
                return parseInt(value);
            }

            // convert minute to hour
            var newMinute = parseInt(minute / unit) * unit; // minute rounded (round down)
            var hourOverbalanceArr = (newMinute / 60).toString().split('.');
            var hourOverbalance = hourOverbalanceArr.length > 1 ? hourOverbalanceArr[1].substring(0, 2) :'00';

            value = parseInt(value).toString() + '.' + hourOverbalance;
        }

        return parseFloat(value);
    }

    // Create html for dialog
    function htmlDialog(type, message) {
        var dialogId = type == 1 ? 'dialog-message' : 'dialog-confirm';
        var iconClass = type == 1 ? 'ui-icon-circle-check' : 'ui-icon-alert';

        var html = '<div id="' + dialogId + '">'
            + '<p>'
            + '<span class="ui-icon ' + iconClass + '"></span>'
            + message;

        if (type != 1)
            html += '<br><img src="/Images/ajax-loader.gif" />';

        html += '</p></div>';

        return html;
    }

    // Show message dialog
    function showMessageDialog(message, urlRedirect, alert, formId) {
        var type = (typeof (alert) !== 'undefined' && alert !== null) ? 2 : 1;
        var dialogId = (typeof (alert) !== 'undefined' && alert !== null) ? '#dialog-confirm' : '#dialog-message';
        if (typeof (body) !== 'undefined')
        {
            $(body).before(htmlDialog(type, message));
        }
        else
        {
            $(login).before(htmlDialog(type, message));
        }        

        $(dialogId).dialog({
            modal: true,
            closeOnEscape: false,
            buttons: {
                OK: function () {
                    $(this).dialog("close");

                    if (typeof(urlRedirect) !== 'undefined' && urlRedirect !== null)
                        window.location.href = urlRedirect;

                    if (typeof (formId) !== 'undefined' && formId !== null)
                        $(formId).submit();
                }
            }
        });
        $(".ui-dialog-titlebar").hide();
    }    

    // Show confirm dialog when submit form
    function showSubmitConfirmDialog(message, formId, urlRedirect) {
        $(body).before(htmlDialog(2, message));

        $("#dialog-confirm").dialog({
            modal: true,
            closeOnEscape: false,
            buttons: {
                OK: function () {
                    if (typeof (formId) !== 'undefined' && formId !== null) {
                        $('#dialog-confirm img').css('display', 'block');
                        $('div.ui-dialog button').attr('disabled', 'disabled').children('span').addClass('disabled');

                        ShipOnline.utility.replaceAllMoney();
                        $(formId).submit();
                    }

                    if (typeof (urlRedirect) !== 'undefined' && urlRedirect !== null)
                        window.location.href = urlRedirect;
                },
                'キャンセル': function () {
                    $(this).dialog("close");
                }
            }
        });
        $(".ui-dialog-titlebar").hide();
    }

    // Show message dialog
    function showMessageDialogCB(alert, message, callback) {
        var type = (typeof (alert) !== 'undefined' && alert !== null) ? 2 : 1;
        var dialogId = (typeof (alert) !== 'undefined' && alert !== null) ? '#dialog-confirm' : '#dialog-message';
        $(body).before(htmlDialog(type, message));

        $(dialogId).dialog({
            modal: true,
            closeOnEscape: false,
            buttons: {
                OK: function () {
                    $(this).dialog("close");
                    callback(null);
                }
            }
        });
        $(".ui-dialog-titlebar").hide();
    }

    // set status is session timeout
    function setStatusTimeOut() {
        $(window).off('beforeunload');
        localStorage.setItem('sessiontimeout', 'true');
        window.location.href = Constant.URL_REDIRECT_TIMEOUT;
    }

    function IsAuthenticateTimeout(message, form, surl) {
        var success = 1;
        
        $.ajax({
            type: "GET",
            url: surl,            
            dataType: 'json',
            contentType: "application/json; charset=utf-8",
            async: false,
            cache: false,
            success: function () {
                if (form != '')
                {
                    if (message != '')
                    {
                        $(form).submit();
                    }
                    else
                    {
                        showSubmitConfirmDialog(message, form);
                    }
                }                

                success = 1;
                return;
            },
            error: function (error) {
                if (error.status == HTTPCode.TIME_OUT) { //419: Authentication Timeout
                    setStatusTimeOut();
                }
                success = 0;
                return;
            }
        });
        return success;
    }

    // check session timeout
    function checkTimeout() {
        $.ajax({
            type: "GET",
            url: '/Common/Common/CheckTimeOut',
            dataType: 'json',
            cache: false,
            success: function () {
                return;
            },
            error: function (error) {
                if (error.status == HTTPCode.TIME_OUT) { //419: Authentication Timeout
                    setStatusTimeOut();
                }
                return;
            }
        });
    }

    function buildColumShortText(data, className) {
        data = data != null ? data : '';
        className = className != null ? className : '';

        var html = '<div class="short-text text-overflow ' + className+ '" title="' + data + '">' + data + '</div>';
        return html;
    }

    function buildColumShortTextArea(data, className, maxRow) {
        data = nvl(data, '');
        className = nvl(className, '');
        maxRow = typeof (maxRow) != 'undefined' ? maxRow : 0;

        var html = '';

        if (data.length == 0) {
            html = '&nbsp;';
        } else {
            var dataArea = data.trim().split('\n');
            var valueLength = 0;

            for (var i = 0; i < dataArea.length; i++) {
                valueLength += dataArea[i].length;

                if (i == dataArea.length - 1 || valueLength > 50 || (maxRow > 0 && i == maxRow - 1)) {
                    className += ' short-text text-overflow';
                }

                html += '<div class="wrap-text ' + className + '" title="' + data + '">' + dataArea[i] + '</div>';

                if (valueLength > 50) {
                    break;
                }
            }
        }

        return html;
    }


    function builDetailFormCode(url, id) {
        var html = '<form method="POST" action="' + url + '">'
            + '<input type="hidden" name="id" value="' + id + '"/>'
            + '<a href="#" class="detail-link" onclick="$(this).parent().submit();">詳細</a>'
            + '</form>';
        return html;
    }

    function builDetailFormCodeLink(id) {
        var s = '<form method="post" action="@Url.Action("Select", "Customer")">';
        s += '<input type="hidden" name="id" value="' + id + '"/>';
        s += '<a href="#"><button type="submit" class="formSubmitLink">編集</button></a>';
        s += '</form>';
            
        return s;
    }

    function imeControl(control, type) {
        if (type === 'active') {
            control.css('ime-mode', 'active');
        } else if (type === 'inactive') {
            control.css('ime-mode', 'inactive');
        }else {
            control.css('ime-mode', 'disabled');
        }
    }

    // get data from server by Ajax GET. Return result
    function getDataByAjax(url, param) {
        var result;

        $.ajax({
            type: 'GET',
            url: url,
            data: param,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                result = data;
            },
            error: function (err) {
                if (err.status == HTTPCode.TIME_OUT) { //419: Authentication Timeout
                    setStatusTimeOut();
                }
                return;
            }
        });
        return result;
    }

    // get data from server by Ajax GET. Callback result
    function getDataByAjaxCB(url, param, callback) {
        $.ajax({
            type: 'GET',
            url: url,
            data: param,
            dataType: 'json',
            async: false,
            cache: false,
            success: function (data) {
                callback(data);
            },
            error: function (err) {
                if (err.status == HTTPCode.TIME_OUT) { //419: Authentication Timeout
                    setStatusTimeOut();
                }
                return;
            }
        });
    }

    // get data from server by Ajax GET with param is serialize to check valid data. Return result
    function checkDataExistByAjax(url, param) {
        var result;

        $.ajax({
            type: 'GET',
            url: url,
            data: param,
            dataType: 'json',
            traditional: true,
            async: false,
            cache: false,
            success: function (data) {
                result = data;
            },
            error: function (err) {
                if (err.status == HTTPCode.TIME_OUT) { //419: Authentication Timeout
                    setStatusTimeOut();
                }
                return;
            }
        });
        return result;
    }

    // get image data by ajax
    function getImageByAjax(url, param, callback) {
        $.ajax({
            type: 'GET',
            url: url,
            data: param,
            contentType: 'application/json',
            dataType: 'json',
            cache: false,
            success: function (data) {
                callback(data);
            },
            error: function () {
                if (error.status == HTTPCode.TIME_OUT) { //419: Authentication Timeout
                    setStatusTimeOut();
                }
                callback(null);
            }
        });
    }

    // Get month colum array on data table list bt start date and end date of project
    function getMonthCols(startDate, endDate) {
        var sY = parseInt(startDate.split('/')[0]);
        var sM = parseInt(startDate.split('/')[1]);
        var eY = parseInt(endDate.split('/')[0]);
        var eM = parseInt(endDate.split('/')[1]);
        var columArr = [];

        for (var Y = eY, M = eM, i = 0; Y > sY || (Y == sY && M >= sM) ; i++, M--) {
            var YM;
            if (M == 0) {
                M = 12;
                Y--;
            }

            if (M < 10)
                YM = Y.toString() + '/0' + M.toString();
            else
                YM = Y.toString() + '/' + M.toString();
            columArr.unshift(YM);
        }
        return columArr;
    }

    // Set title colum with month for data table list
    function bindMonthCols(startDate, endDate) {
        var colums = getMonthCols(startDate, endDate);

        $('table.tb-detail tr th.th-month').remove();
        for (var i = 0; i < colums.length; i++) {
            $('table.tb-detail tr.month-colum').append('<th class="th-month">' + colums[i] + '</th>');
        }
    }

    function bindTotalCols(colums, startDate, endDate) {
        var htmlManday = '';
        var htmlMoney = '';

        if (colums == null) {
            colums = getMonthCols(startDate, endDate);
        }

        for (var i = 0; i < colums.length; i++) {
            htmlManday += '<td class="right">'
                + '<label class="font-normal" name="' + colums[i] + '">0.0</label>'
                + '<label>人日</label>'
                + '</td>';
            htmlMoney += '<td class="right">'
                + '<label class="font-normal money" name="' + colums[i] + '">0</label>'
                + '<label>円</label>'
                + '</td>';
        }

        $('table.tb-ma-center tr.tr-total, table.tb-ma-sales-center tr.tr-total, table.tb-sc-center tr.tr-total').empty();
        $('table.tb-ma-center tr.tr-total').append(htmlManday);
        $('table.tb-ma-sales-center tr.tr-total, table.tb-sc-center tr.tr-total').append(htmlMoney);
    }

    // Bind data for tag link list by customer
    function bindTagsByCustomer(customerId, control) {
        var param = { customerId: customerId };
        var data = getDataByAjax('/Common/GetTagListJson', param);

        var $ddlTagLink = $(control);
        $ddlTagLink.empty();

        if (data.length > 0) {
            var html = '<option value="">指定なし</option>';

            for (var i = 0; i < data.length; i++) {
                html += '<option value="' + data[i].tag_id + '">' + data[i].tag_name + '</option>';
            }

            $ddlTagLink.append(html);
        }
    }

    // Build edit form 
    function buildEditForm(url, idName, idValue, isNewOrEdit, actionLinkName, isExtend, extendUrl, extendLinkName) {
        var editForm = '';
        var IDCopy = '';
        if (isExtend == 1)
        {
            if (idValue != 0) {
                IDCopy = 'registSource';
                editForm = '<span style="float:left;">'
                editForm += '<form method="post" action="' + url + '">';
                editForm += '<input type="hidden" name="' + idName + '" value="' + idValue + '"/>';
                editForm += '<input type="hidden" name="' + IDCopy + '" value="' + 2 + '"/>';
                editForm += '<a href="#"><button type="submit" class="formSubmitLink">' + actionLinkName + '</button></a>';
                editForm += '</form>';
                //s += '／';
                editForm += '</span>';

                editForm += '<span >';
                editForm += '／';
                editForm += '</span>';

                editForm += '<span style="float:right;">';
                editForm += '<form method="post" action="' + extendUrl + '">';
                editForm += '<input type="hidden" name="' + idName + '" value="' + idValue + '"/>';
                editForm += '<input type="hidden" name="' + IDCopy + '" value="' + 3 + '"/>';
                editForm += '<a href="#"><button type="submit" class="formSubmitLink">' + extendLinkName + '</button></a>';
                editForm += '</form>';
                editForm += '</span>';
            }                       
        }
        else
        {
            if (idValue != 0) {
                if (isNewOrEdit == 0) {
                    IDCopy = 'isLinkToCopy';
                    editForm = '<form method="post" action="' + url + '">';
                    editForm += '<input type="hidden" name="' + idName + '" value="' + idValue + '"/>';
                    editForm += '<input type="hidden" name="' + IDCopy + '" value="' + 1 + '"/>';
                    editForm += '<a href="#"><button type="submit" class="formSubmitLink">' + actionLinkName + '</button></a>';
                    editForm += '</form>';
                }
                else if (isNewOrEdit == 1) {
                    editForm = '<form method="post" action="' + url + '">';
                    editForm += '<input type="hidden" name="' + idName + '" value="' + idValue + '"/>';
                    editForm += '<a href="#"><button type="submit" class="formSubmitLink">' + actionLinkName + '</button></a>';
                    editForm += '</form>';
                }
                else if (isNewOrEdit == 2)
                {
                    editForm = '<form method="post" action="' + url + '">';
                    editForm += '<input type="hidden" name="' + idName + '" value="' + idValue + '"/>';
                    editForm += '<input type="hidden" name="registSource" value="' + 4 + '"/>';
                    editForm += '<a href="#"><button type="submit" class="formSubmitLink">' + actionLinkName + '</button></a>';
                    editForm += '</form>';
                }
                else
                {
                    editForm = '<form method="post" action="' + url + '">';
                    editForm += '<input type="hidden" name="' + idName + '" value="' + idValue + '"/>';
                    editForm += '<input type="hidden" name="registSource" value="' + 1 + '"/>';
                    editForm += '<a href="#"><button type="submit" class="formSubmitLink">' + actionLinkName + '</button></a>';
                    editForm += '</form>';
                }
            }            
        }
                
        return editForm;
    }

    function buildConfirmForm(havelink, url, idName, idValue, actionName,idLink) {
        var confirmForm = '';
        if (havelink != 0)
        {
            confirmForm = '<form method="post" id= "frmEstimateSend" action="' + url + '">';
            confirmForm += '<input type="hidden" name="' + idName + '" value="' + idValue + '"/>';
            confirmForm += '<a href="#"><button type="button" id="' + idLink + '"  class="formSubmitLink">' + actionName + '</button></a>';
            confirmForm += '</form>';
        }       

        return confirmForm;
    }

    function GetDateTimeNow() {
        // create Date object for current location
        var d = new Date();

        // convert to msec
        // add local time zone offset 
        // get UTC time in msec
        var utc = d.getTime() + (d.getTimezoneOffset() * 60000);

        //var currentdate = new Date(utc);
        var currentdate = new Date(utc + (3600000 * 7)); // create new Date object Japanese
        var month = currentdate.getMonth() + 1;
        var day = currentdate.getDate();
        var hour = currentdate.getHours();
        var minutes = currentdate.getMinutes();

        var nowDate = 
            (day < 10 ? '0' : '') + day + '/' + (month < 10 ? '0' : '') + month + '/' + currentdate.getFullYear()  + '  ' + hour + ":" + (minutes < 10 ? '0' : '') + minutes + 'Time';

        return nowDate;
    }

    function GetDateNow() {
        // create Date object for current location
        var d = new Date();

        // convert to msec
        // add local time zone offset 
        // get UTC time in msec
        var utc = d.getTime() + (d.getTimezoneOffset() * 60000);

        // create new Date object Japanese
        var currentdate = new Date(utc + (3600000 * 9));
        var month = currentdate.getMonth() + 1;
        var day = currentdate.getDate();
        var nowDate = currentdate.getFullYear() + '/' +
            (month < 10 ? '0' : '') + month + '/' +
            (day < 10 ? '0' : '') + day;

        return nowDate;
    }

    function InitDatepickerMonths()
    {
        $(".date.datepicker-months").datepicker({
            format: "yyyy/mm",
            viewMode: "months",
            minViewMode: "months",
            autoclose: true,
            language: 'ja'
        });
    }

    function InitDatepickerDays()
    {
        $(".date.datepicker-days").datepicker({
            autoclose: true,
            language: 'ja'
        });
    }

    // Check user permission to show button or link
    function checkPermission(control,isAllow) {
        if ($(control).length > 0) {
            if (isAllow && isAllow == true) {
                return true;
            } else {
                return false;
            }            
        }
        return false;
    }

    function showConfirmModal(message, callback) {
        BootstrapDialog.closeAll();
        var clicked = false;
        BootstrapDialog.show({
            title: '<i class="ui-icon ui-icon-alert"></i>',
            id: 'dlgConfirmModal',
            message: message,
            closable: false,
            buttons: [{
                label: 'No',
                cssClass: 'btn btn-default btn-md-cancel',
                action: function (dialog) {
                    if (!clicked)
                    {
                        clicked = true;
                        dialog.close();

                        if (typeof (callback) !== 'undefined') {
                            callback(false);
                        }
                    }                    
                }
            }, {
                label: 'YES',
                cssClass: 'btn btn-orange btn-md-ok',
                id: 'btnConfirmModalOK',
                action: function (dialog) {
                    if (!clicked)
                    {
                        clicked = true;
                        dialog.close();

                        if (typeof (callback) == 'function') {
                            callback(true);
                        }
                    }                    
                }
            }]
        });
    }


    function showMessageModal(message, warning, callback) {
        BootstrapDialog.closeAll();
        var iconType = warning ? 'ui-icon-alert' : 'ui-icon-circle-check';

        if (typeof (message) != 'undefined' && message.length > 0) {
            message = message.replace(/\n/g, '</br>');
        }

        BootstrapDialog.show({
            title: '<i class="ui-icon ' + iconType + '"></i>',
            id: 'dlgMessageModal',
            message: message,
            closable: false,
            buttons: [{
                label: 'OK',
                cssClass: 'btn btn-orange btn-md-ok',
                id: 'btnMessageModalOK',
                action: function (dialog) {
                    $("#onloadDiv").hide();
                    dialog.close();

                    if (typeof (callback) == 'function') {
                        callback(null);
                    }
                }
            }]
        });
    }

    // Set data for product no
    function setProductNo(productSeqNo) {
        if ($("#PRODUCT_NO").length > 0) {
            if (productSeqNo == '') {
                $("#PRODUCT_NO").val('');
            } else {
                var productSeqNoArr = productSeqNo.toString().split(',');
                var valueArr = [];

                for (var i = 0; i < productSeqNoArr.length; i++) {
                    var param = { "productsegno": productSeqNoArr[i] };
                    var data = getDataByAjax('/Estimate/Estimate/GetExpandFlagByProductSegNo', param);

                    valueArr.push(data.productno);
                }

                $("#PRODUCT_NO").val(valueArr.toString());
            }
        }
    }

    // Bind data of PIC to select list ChienBV
    function bindDistrict($cityCd, $districtCd) {
        var cityCd = $cityCd.find('option:selected').val();

        var $districtCdSelect = $districtCd.find('option:not(:first)');
        $districtCdSelect.each(function () {
            if ($(this).parent().is('i')) $(this).unwrap();
        });
        $districtCdSelect.wrap('<i></i>');

        if (cityCd != null && cityCd.length > 0 && $.isNumeric(cityCd)) {
            var param = { "cityCd": parseInt(cityCd) };
            var dataPicList = getDataByAjax('/Common/GetDistrictByCityCd', param);

            $.each(dataPicList, function (index, data) {
                var keyItem = data.key;

                $districtCdSelect.each(function () {
                    var valueArr = this.value.split('_');
                    var cityKey = parseInt(valueArr[0]);
                    var districtKey = parseInt(valueArr[1]);

                    if (cityKey == cityCd && districtKey == keyItem) {
                        $(this).unwrap();
                        //$districtCd.val(this.value);
                        return false;
                    }
                });
            });
        }
    }

    function bindDistrictExtend($cityCd, $districtCd) {
        if (typeof ($cityCd) == 'undefined') {
            $cityCd = $('.renderCity');
            $districtCd = $('.renderDistrict');
        }
        bindDistrict($cityCd, $districtCd);
    }
    // MCNB
    // Render when select citycd
    function bindDistrictExtend2($cityCd, $districtCd, $townCd) {
        if (typeof ($cityCd) == 'undefined') {
            $cityCd = $('.renderCity');
            $districtCd = $('.renderDistrict');
            $townCd = $('.renderTown');
        }
        var cityCd = $cityCd.find('option:selected').val();

        var $districtCdSelect = $districtCd.find('option:not(:first)');
        var $townOption = $townCd.find('option:not(:first)');
        $townOption.each(function () {
            if ($(this).parent().is('i')) {
                $(this).unwrap();
            }
        });
        $districtCdSelect.each(function () {
            if ($(this).parent().is('i')) $(this).unwrap();
        });
        $districtCdSelect.wrap('<i></i>');
        $townOption.wrap('<i></i>');

        if (cityCd != null && cityCd.length > 0 && $.isNumeric(cityCd)) {
            var param = { "cityCd": parseInt(cityCd) };
            var dataPicList = getDataByAjax('/Common/GetDistrictByCityCd', param);

            $.each(dataPicList, function (index, data) {
                var keyItem = data.key;

                $districtCdSelect.each(function () {
                    var valueArr = this.value.split('_');
                    var cityKey = parseInt(valueArr[0]);
                    var districtKey = parseInt(valueArr[1]);

                    if (cityKey == cityCd && districtKey == keyItem) {
                        $(this).unwrap();
                        //$districtCd.val(this.value);
                        return false;
                    }
                });
            });
        }

        bindTown($cityCd, $districtCd, $townCd);
    }

    function bindTown($cityCd, $districtCd, $townCd) {
        var cityCd = $cityCd.find('option:selected').val();
        var districtSelect = $districtCd.find('option:selected').val();
        var valueArr = districtSelect.split('_');
        var cityKey = parseInt(valueArr[0]);
        var districtCd = valueArr[1];

        var $townOption = $townCd.find('option:not(:first)');
        $townOption.each(function () {
            if ($(this).parent().is('i')) $(this).unwrap();
        });
        $townOption.wrap('<i></i>');

        if (cityCd != null && cityCd.length > 0 && $.isNumeric(cityCd)
            && districtCd != null && districtCd.length > 0 && $.isNumeric(districtCd)) {
            var param = { "cityCd": parseInt(cityCd), "districtCd": districtCd };
            var dataTownList = getDataByAjax('/Common/GetTownByDistrictCd', param);

            $.each(dataTownList, function (index, data) {
                var keyItem = data.key;

                $townOption.each(function () {
                    var valueArr = this.value.split('_');
                    var cityKey = parseInt(valueArr[0]);
                    var districtKey = valueArr[1];
                    var TownKey = parseInt(valueArr[2]);

                    if (cityKey == cityCd && districtKey == districtCd && TownKey == keyItem) {
                        $(this).unwrap();
                        //$districtCd.val(this.value);
                        return false;
                    }
                });
            });
        }
    }

    // Bind data sub contract type to select list
    function bindSubContractType($contractType, $subContractType, $product, noDisplayBlank) {
        if (Constant.IS_DISPLAY_SUBCONTRACT_TYPE) {
            var isMultiple = false;

            if ($subContractType.attr('multiple') == 'multiple') {

                isMultiple = true;
            }

            var $selectedContractType = $contractType.find('option:selected');
            var $subContractTypeOption = noDisplayBlank || isMultiple ? $subContractType.find('option') : $subContractType.find('option:not(:first)');
            var $productOption = noDisplayBlank ? $product.find('option') : $product.find('option:not(:first)');

            $subContractTypeOption.each(function () {
                if ($(this).parent().is('i')) {
                    $(this).unwrap();
                }
            });

            $productOption.each(function () {
                if ($(this).parent().is('i')) {
                    $(this).unwrap();
                }
            });

            $subContractTypeOption.wrap('<i></i>');
            $productOption.wrap('<i></i>');

            if ($selectedContractType.length > 1) {
                $subContractType.val('').prop('disabled', true);
                $product.val('').prop('disabled', true);
            } else {
                $subContractType.prop('disabled', false);
                $product.prop('disabled', false);

                var contractType = $selectedContractType.val();

                if (contractType != null && contractType.length > 0 && $.isNumeric(contractType)) {
                    var param = { "contractType": parseInt(contractType) };
                    var dataSubContractType = getDataByAjax('/Common/Common/GetSubContractTypeSelectList', param);
                    var selectFirst = true;

                    $.each(dataSubContractType, function (index, data) {
                        var keyItem = data.key;

                        $subContractTypeOption.each(function () {
                            var valueArr = this.value.split('_');

                            if (valueArr[0] == contractType && valueArr[1] == keyItem) {
                                $(this).unwrap();

                                if (noDisplayBlank && selectFirst && !isMultiple) {

                                    selectFirst = false;
                                    $subContractType.val(this.value);
                                }

                                return false;
                            }
                        });
                    });
                } else {
                    $subContractType.find('option:selected').each(function () {
                        $(this).removeAttr('selected');
                    });
                }
            }
        }

        bindProduct($contractType, $subContractType, $product, noDisplayBlank);
        setProductNo('');
    }

    function bindSubContractTypeExtend($contractType, $subContractType, $product, noDisplayBlank) {
        if (typeof ($contractType) == 'undefined') {
            $contractType = $('.ddlContractTypeMaster');
            $subContractType = $('.ddlSubContractTypeMaster');
            $product = $('.ddlProductMaster');
        }
        bindSubContractType($contractType, $subContractType, $product, noDisplayBlank);
    }

    // Bind data product to select list
    function bindProduct($contractType, $subContractType, $product, noDisplayBlank) {
        if ($product.length > 0) {
            var isMultiple = false;

            if ($subContractType.attr('multiple') == 'multiple') {

                isMultiple = true;
            }

            var $selectedOption = $subContractType.find('option:selected');
            var $productOption = noDisplayBlank || isMultiple ? $product.find('option') : $product.find('option:not(:first)');

            $productOption.each(function () {
                if ($(this).parent().is('i')) {
                    $(this).unwrap();
                }
            });

            $productOption.wrap('<i></i>');

            if ($selectedOption.length > 1 || $contractType.find('option:selected').length > 1) {
                $product.val('').prop('disabled', true);
            } else {
                $product.prop('disabled', false);

                var selectedValue = $selectedOption.length == 0 ? '' : $selectedOption.val().split('_')[1];
                var contractType = $contractType.find('option:selected').val();
                var subContractType = Constant.IS_DISPLAY_SUBCONTRACT_TYPE ? selectedValue : Constant.DEFAULT_SUB_CONTRACT_TYPE_VALUE;

                if (contractType != null && subContractType != null
                    && contractType.length > 0 && subContractType.length > 0
                    && $.isNumeric(contractType) && $.isNumeric(subContractType)) {

                    var param = { "contractType": parseInt(contractType), "subContractType": parseInt(subContractType) };
                    var dataProduct = getDataByAjax('/Common/Common/GetProductSelectList', param);
                    var selectFirst = true;

                    $.each(dataProduct, function (index, data) {
                        var keyItem = data.key;

                        $productOption.each(function () {
                            if (this.value == keyItem) {
                                $(this).unwrap();

                                if (noDisplayBlank && selectFirst && !isMultiple) {
                                    selectFirst = false;
                                    $product.val(this.value);
                                }

                                return false;
                            }
                        });
                    });
                }
            }

            setProductNo('');
        }
    }

    function bindProductExtend($contractType, $subContractType, $product, noDisplayBlank) {
        if (typeof ($contractType) == 'undefined') {
            $contractType = $('.ddlContractTypeMaster');
            $subContractType = $('.ddlSubContractTypeMaster');
            $product = $('.ddlProductMaster');
        }

        bindProduct($contractType, $subContractType, $product, noDisplayBlank);
    }

    function getSameDayAddMonths(inputDate, addMonth) {
        var year = inputDate.getFullYear();
        var month = inputDate.getMonth();
        var day = inputDate.getDate();
        if (day === 0) {
            day = 1;
        }
        if (day >= new Date(year, month + 1, 0).getDate()) {
            return new Date(year, month + addMonth + 1, 0); // End of month
        }
        return new Date(year, month + addMonth, day);
    }


    function LastDayOfMonth(Year, Month) {
        return new Date((new Date(Year, Month, 1)) - 1).getDate();
    }

    function autoCompleteYearMonthDay(dateString) {
        var splittedString = dateString.split('/');
        if (splittedString.length == 3) {
            var year = splittedString[0], month = splittedString[1], day = splittedString[2];

            year = !parseInt(year) ? '2000' : (parseInt(year).toString().length == 4) ? parseInt(year) : parseInt(year) > 9999 ? '9999' : parseInt(year) < 999 ? (parseInt(year) + 2000).toString() : year;

            month = !parseInt(month) ? '01' : (parseInt(month).toString().length >= 2) ? (month > 12 ? '12' : parseInt(month)) : (parseInt(month).toString().length == 1) ? '0' + parseInt(month) : month.length == 0 ? '01' : month;

            var lastDate = LastDayOfMonth(year, month);

            day = !parseInt(day) ? '01' : (parseInt(day).toString().length >= 2) ? (day > lastDate ? lastDate : parseInt(day)) : (parseInt(day).toString().length == 1) ? '0' + parseInt(day) : day.length == 0 ? '01' : day;

            return year + '/' + month + '/' + day;
        }
        return dateString;
    }

    function autoCompleteYearMonth(ymString) {
        var splittedString = ymString.split('/');
        if (splittedString.length == 2) {
            var year = splittedString[0], month = splittedString[1];

            year = !parseInt(year) ? '2000' : (parseInt(year).toString().length == 4) ? parseInt(year) : parseInt(year) > 9999 ? '9999' : parseInt(year) < 999 ? (parseInt(year) + 2000).toString() : year;

            month = !parseInt(month) ? '01' : (parseInt(month).toString().length >= 2) ? (month > 12 ? '12' : parseInt(month)) : (parseInt(month).toString().length == 1) ? '0' + parseInt(month) : month.length == 0 ? '01' : month;

            return year + '/' + month;
        }
        return ymString;
    }

    //  Gets the browser name or returns an empty string if unknown
    var checkBrowser = function () {
        // Return cached result if avalable, else get result then cache it.
        if (checkBrowser.prototype._cachedResult)
            return checkBrowser.prototype._cachedResult;

        // Opera 8.0+ (UA detection to detect Blink/v8-powered Opera)
        var isOpera = !!window.opera || navigator.userAgent.indexOf(' OPR/') >= 0;
        // Firefox 1.0+
        var isFirefox = typeof InstallTrigger !== 'undefined';
        // At least Safari 3+: "[object HTMLElementConstructor]"
        var isSafari = Object.prototype.toString.call(window.HTMLElement).indexOf('Constructor') > 0;
        // Chrome 1+
        var isChrome = !!window.chrome && !isOpera;
        // At least IE6
        var isIE = false || !!document.documentMode;

        return (checkBrowser.prototype._cachedResult =
            isOpera ? 'Opera' :
            isFirefox ? 'Firefox' :
            isSafari ? 'Safari' :
            isChrome ? 'Chrome' :
            isIE ? 'IE' :
            '');
    };

    function ShowPopup(url, param) {
        if (!typeof(param) == 'object') {
            param = null;
        }

        ClosePopup(function () {
            $('#loadingPopup').show();

            $('#showPopup .modal-body').load(url, param, function () {
                setTimeout(function () {
                    $('#loadingPopup').hide();
                    $('#showPopup').modal('show');
                }, 500);
            });
        });
    }

    function ShowPopupInfor(url, param) {
        if (!typeof (param) == 'object') {
            param = null;
        }

        ClosePopup(function () {
            $('#loadingPopup').show();

            $('#showPopupInfor .modal-body').load(url, param, function () {
                setTimeout(function () {
                    $('#loadingPopup').hide();
                    $('#showPopupInfor').modal('show');
                }, 500);
            });
        });
    }

    function ClosePopup(callback) {
        $('#showPopup .modal-dialog').removeClass('register-screen');
        $('#showPopup .modal-body').removeClass('register-popup');

        $('#showPopup .modal-popup').removeClass('register-screen');
        $('#showPopup .modal-popup').removeClass('edit-engineer-popup');

        $('#showPopup .main-popup').removeClass('edit-order-popup');
        $('#showPopup .main-popup').removeClass('edit-engineer-popup');
        $('#showPopup .main-popup').removeClass('input-popup');

        $('#showPopup .main-popup').removeClass('custom-height');

        $('#showPopup .modal-body').empty();
        $('#showPopup .modal-backdrop').remove();
        $('#showPopup').modal('hide');

        if (callback)
            callback();
    }
    function ClosePopupInfor(callback) {
        $('#showPopupInfor .modal-body').empty();
        $('#showPopupInfor .modal-backdrop').remove();
        $('#showPopupInfor').modal('hide');

        if (callback)
            callback();
    }

    function preventXSS(mylink) {
        //var agree = confirm('Are you sure you want to remove ' + my_string + '?');        
        if (mylink.indexOf("javascript") != -1 || mylink.indexOf("alert") != -1 || mylink.indexOf("script") != -1) {
            return false;
        }
        else {
            return true;
        }
    }


    // Bind sub category by main category
    function BindSubEngineerCategory($mainControl, $subControl) {
        var mainSeqNoString = $mainControl.find('option:selected').val() + '_';

        $subControl.find('option').each(function () {
            // hide all
            if ($(this).index() != 0) $(this).wrap('<i></i>');

            // unhide sub category by main category
            if (mainSeqNoString != null && mainSeqNoString.length > 0 && this.value.indexOf(mainSeqNoString) == 0) $(this).unwrap();
        });
    }

    function Convert_JPCharacters(str, option) {
        option = option || "KV";
        if (option.match('K')) {
            if (option.match('V')) {
                // han-kaku katakana (with daku-ten) to zen-kaku kata-kana
                str = str.replace(/([\uff66\uff73\uff76-\uff84\uff8a-\uff8e\uff9c])\uff9e/g, function (m, c) {
                    var f = {
                        0xff76: 0x30ac, 0xff77: 0x30ae, 0xff78: 0x30b0, 0xff79: 0x30b2, 0xff7a: 0x30b4,
                        0xff7b: 0x30b6, 0xff7c: 0x30b8, 0xff7d: 0x30ba, 0xff7e: 0x30bc, 0xff7f: 0x30be,
                        0xff80: 0x30c0, 0xff81: 0x30c2, 0xff82: 0x30c5, 0xff83: 0x30c7, 0xff84: 0x30c9,
                        0xff8a: 0x30d0, 0xff8b: 0x30d3, 0xff8c: 0x30d6, 0xff8d: 0x30d9, 0xff8e: 0x30dc,
                        0xff73: 0x30f4, 0xff9c: 0x30f7, 0xff66: 0x30fa
                    };
                    return String.fromCharCode(f[c.charCodeAt(c)]);
                })
                .replace(/([\uff8a-\uff8e])\uff9f/g, function (m, c) {
                    var f = {
                        0xff8a: 0x30d1, 0xff8b: 0x30d4, 0xff8c: 0x30d7, 0xff8d: 0x30da, 0xff8e: 0x30dd
                    };
                    return String.fromCharCode(f[c.charCodeAt(c)]);
                });
            }
            // han-kaku kata-kana to zen-kaku kata-kana
            str = str.replace(/[\uff61-\uff9d]/g, function (c) {
                var m = {
                    0xff61: 0x3002, 0xff62: 0x300c, 0xff63: 0x300d, 0xff64: 0x3001, 0xff65: 0x30fb,
                    0xff66: 0x30f2, 0xff67: 0x30a1, 0xff68: 0x30a3, 0xff69: 0x30a5, 0xff6a: 0x30a7,
                    0xff6b: 0x30a9, 0xff6c: 0x30e3, 0xff6d: 0x30e5, 0xff6e: 0x30e7, 0xff6f: 0x30c3,
                    0xff70: 0x30fc, 0xff71: 0x30a2, 0xff72: 0x30a4, 0xff73: 0x30a6, 0xff74: 0x30a8,
                    0xff75: 0x30aa, 0xff76: 0x30ab, 0xff77: 0x30ad, 0xff78: 0x30af, 0xff79: 0x30b1,
                    0xff7a: 0x30b3, 0xff7b: 0x30b5, 0xff7c: 0x30b7, 0xff7d: 0x30b9, 0xff7e: 0x30bb,
                    0xff7f: 0x30bd, 0xff80: 0x30bf, 0xff81: 0x30c1, 0xff82: 0x30c4, 0xff83: 0x30c6,
                    0xff84: 0x30c8, 0xff85: 0x30ca, 0xff86: 0x30cb, 0xff87: 0x30cc, 0xff88: 0x30cd,
                    0xff89: 0x30ce, 0xff8a: 0x30cf, 0xff8b: 0x30d2, 0xff8c: 0x30d5, 0xff8d: 0x30d8,
                    0xff8e: 0x30db, 0xff8f: 0x30de, 0xff90: 0x30df, 0xff91: 0x30e0, 0xff92: 0x30e1,
                    0xff93: 0x30e2, 0xff94: 0x30e4, 0xff95: 0x30e6, 0xff96: 0x30e8, 0xff97: 0x30e9,
                    0xff98: 0x30ea, 0xff99: 0x30eb, 0xff9a: 0x30ec, 0xff9b: 0x30ed, 0xff9c: 0x30ef,
                    0xff9d: 0x30f3
                };
                return String.fromCharCode(m[c.charCodeAt(0)]);
            });
        } else if (option.match('H')) {
            if (option.match('V')) {
                // han-kaku kata-kana (with daku-ten) to zen-kaku hira-gana
                str = str.replace(/([\uff73\uff76-\uff84\uff8a-\uff8e])\uff9e/g, function (m, c) {
                    var f = {
                        0xff73: 0x3094,
                        0xff76: 0x304c, 0xff77: 0x304e, 0xff78: 0x3050, 0xff79: 0x3052, 0xff7a: 0x3054,
                        0xff7b: 0x3056, 0xff7c: 0x3058, 0xff7d: 0x305a, 0xff7e: 0x305c, 0xff7f: 0x305e,
                        0xff80: 0x3060, 0xff81: 0x3062, 0xff82: 0x3065, 0xff83: 0x3067, 0xff84: 0x3069,
                        0xff8a: 0x3070, 0xff8b: 0x3073, 0xff8c: 0x3076, 0xff8d: 0x3079, 0xff8e: 0x307c
                        //0xff9c:0x30f7, 0xff66:0x30fa // vwa vwo
                    };
                    return String.fromCharCode(f[c.charCodeAt(c)]);
                })
                .replace(/([\uff8a-\uff8e])\uff9f/g, function (m, c) {
                    var f = {
                        0xff8a: 0x3071, 0xff8b: 0x3074, 0xff8c: 0x3077, 0xff8d: 0x307a, 0xff8e: 0x307d
                    };
                    return String.fromCharCode(f[c.charCodeAt(c)]);
                });
            }
            // han-kaku kata-kana to zen-kaku hira-gana
            str = str.replace(/[\uff61-\uff9d]/g, function (c) {
                var f = {
                    0xff61: 0x3002, 0xff62: 0x300c, 0xff63: 0x300d, 0xff64: 0x3001, 0xff65: 0x30fb,
                    0xff66: 0x3092, 0xff67: 0x3041, 0xff68: 0x3043, 0xff69: 0x3045, 0xff6a: 0x3047,
                    0xff6b: 0x3049, 0xff6c: 0x3083, 0xff6d: 0x3085, 0xff6e: 0x3087, 0xff6f: 0x3063,
                    0xff70: 0x30fc, 0xff71: 0x3042, 0xff72: 0x3044, 0xff73: 0x3046, 0xff74: 0x3048,
                    0xff75: 0x304a, 0xff76: 0x304b, 0xff77: 0x304d, 0xff78: 0x304f, 0xff79: 0x3051,
                    0xff7a: 0x3053, 0xff7b: 0x3055, 0xff7c: 0x3057, 0xff7d: 0x3059, 0xff7e: 0x305b,
                    0xff7f: 0x305d, 0xff80: 0x305f, 0xff81: 0x3061, 0xff82: 0x3064, 0xff83: 0x3066,
                    0xff84: 0x3068, 0xff85: 0x306a, 0xff86: 0x306b, 0xff87: 0x306c, 0xff88: 0x306d,
                    0xff89: 0x306e, 0xff8a: 0x306f, 0xff8b: 0x3072, 0xff8c: 0x3075, 0xff8d: 0x3078,
                    0xff8e: 0x307b, 0xff8f: 0x307e, 0xff90: 0x307f, 0xff91: 0x3080, 0xff92: 0x3081,
                    0xff93: 0x3082, 0xff94: 0x3084, 0xff95: 0x3086, 0xff96: 0x3088, 0xff97: 0x3089,
                    0xff98: 0x308a, 0xff99: 0x308b, 0xff9a: 0x308c, 0xff9b: 0x308d, 0xff9c: 0x308f,
                    0xff9d: 0x3093
                };
                return String.fromCharCode(f[c.charCodeAt(0)]);
            });
        } else if (option.match('k')) {
            // zen-kaku kata-kana to han-kaku kata-kana
            str = str.replace(/[\u3001\u3002\u300c\u300d\u30a1-\u30ab\u30ad\u30af\u30b1\u30b3\u30b5\u30b7\u30b9\u30bb\u30bd\u30bf\u30c1\u30c3\u30c4\u30c6\u30c8\u30ca-\u30cf\u30d2\u30d5\u30d8\u30db\u30de\u30df-\u30ed\u30ef\u30f2\u30f3\u30fb]/g, function (c) {
                var f = {
                    0x3001: 0xff64, 0x3002: 0xff61, 0x300c: 0xff62, 0x300d: 0xff63, 0x30a1: 0xff67,
                    0x30a2: 0xff71, 0x30a3: 0xff68, 0x30a4: 0xff72, 0x30a5: 0xff69, 0x30a6: 0xff73,
                    0x30a7: 0xff6a, 0x30a8: 0xff74, 0x30a9: 0xff6b, 0x30aa: 0xff75, 0x30ab: 0xff76,
                    0x30ad: 0xff77, 0x30af: 0xff78, 0x30b1: 0xff79, 0x30b3: 0xff7a, 0x30b5: 0xff7b,
                    0x30b7: 0xff7c, 0x30b9: 0xff7d, 0x30bb: 0xff7e, 0x30bd: 0xff7f, 0x30bf: 0xff80,
                    0x30c1: 0xff81, 0x30c3: 0xff6f, 0x30c4: 0xff82, 0x30c6: 0xff83, 0x30c8: 0xff84,
                    0x30ca: 0xff85, 0x30cb: 0xff86, 0x30cc: 0xff87, 0x30cd: 0xff88, 0x30ce: 0xff89,
                    0x30cf: 0xff8a, 0x30d2: 0xff8b, 0x30d5: 0xff8c, 0x30d8: 0xff8d, 0x30db: 0xff8e,
                    0x30de: 0xff8f, 0x30df: 0xff90, 0x30e0: 0xff91, 0x30e1: 0xff92, 0x30e2: 0xff93,
                    0x30e3: 0xff6c, 0x30e4: 0xff94, 0x30e5: 0xff6d, 0x30e6: 0xff95, 0x30e7: 0xff6e,
                    0x30e8: 0xff96, 0x30e9: 0xff97, 0x30ea: 0xff98, 0x30eb: 0xff99, 0x30ec: 0xff9a,
                    0x30ed: 0xff9b, 0x30ef: 0xff9c, 0x30f2: 0xff66, 0x30f3: 0xff9d, 0x30fb: 0xff65
                };
                return String.fromCharCode(f[c.charCodeAt(0)]);
            }).replace(/[\u30ac\u30ae\u30b0\u30b2\u30b4\u30b6\u30b8\u30ba\u30bc\u30be\u30c0\u30c2\u30c5\u30c7\u30c9\u30d0\u30d3\u30d6\u30d9\u30dc\u30f4\u30f7\u30fa]/g, function (c) {
                // with daku-ten
                var f = {
                    0x30ac: 0xff76, 0x30ae: 0xff77, 0x30b0: 0xff78, 0x30b2: 0xff79, 0x30b4: 0xff7a,
                    0x30b6: 0xff7b, 0x30b8: 0xff7c, 0x30ba: 0xff7d, 0x30bc: 0xff7e, 0x30be: 0xff7f,
                    0x30c0: 0xff80, 0x30c2: 0xff81, 0x30c5: 0xff82, 0x30c7: 0xff83, 0x30c9: 0xff84,
                    0x30d0: 0xff8a, 0x30d3: 0xff8b, 0x30d6: 0xff8c, 0x30d9: 0xff8d, 0x30dc: 0xff8e,
                    0x30f4: 0xff73, 0x30f7: 0xff9c, 0x30fa: 0xff66
                }
                return String.fromCharCode(f[c.charCodeAt(0)]) + "\uff9e";
            }).replace(/[\u30d1\u30d4\u30d7\u30da\u30dd]/g, function (c) {
                // with han-daku-ten
                var f = {
                    0x30d1: 0xff8a, 0x30d4: 0xff8b, 0x30d7: 0xff8c, 0x30da: 0xff8d, 0x30dd: 0xff8e
                }
                return String.fromCharCode(f[c.charCodeAt(0)]) + "\uff9f";
            })
            ;
        } else if (option.match('h')) {
            // zen-kaku hira-gana to han-kaku kata-kana
            str = str.replace(/[\u3001\u3002\u300c\u300d\u3041-\u304b\u304d\u304f\u3051\u3053\u3055\u3057\u3059\u305b\u305d\u305f\u3061\u3063\u3064\u3066\u3068\u306a-\u306f\u3072\u3075\u3078\u307b\u307e\u307f-\u308d\u308f\u3092\u3093\u30fb]/g, function (c) {
                var f = {
                    0x3001: 0xff64, 0x3002: 0xff61, 0x300c: 0xff62, 0x300d: 0xff63, 0x3041: 0xff67,
                    0x3042: 0xff71, 0x3043: 0xff68, 0x3044: 0xff72, 0x3045: 0xff69, 0x3046: 0xff73,
                    0x3047: 0xff6a, 0x3048: 0xff74, 0x3049: 0xff6b, 0x304a: 0xff75, 0x304b: 0xff76,
                    0x304d: 0xff77, 0x304f: 0xff78, 0x3051: 0xff79, 0x3053: 0xff7a, 0x3055: 0xff7b,
                    0x3057: 0xff7c, 0x3059: 0xff7d, 0x305b: 0xff7e, 0x305d: 0xff7f, 0x305f: 0xff80,
                    0x3061: 0xff81, 0x3063: 0xff6f, 0x3064: 0xff82, 0x3066: 0xff83, 0x3068: 0xff84,
                    0x306a: 0xff85, 0x306b: 0xff86, 0x306c: 0xff87, 0x306d: 0xff88, 0x306e: 0xff89,
                    0x306f: 0xff8a, 0x3072: 0xff8b, 0x3075: 0xff8c, 0x3078: 0xff8d, 0x307b: 0xff8e,
                    0x307e: 0xff8f, 0x307f: 0xff90, 0x3080: 0xff91, 0x3081: 0xff92, 0x3082: 0xff93,
                    0x3083: 0xff6c, 0x3084: 0xff94, 0x3085: 0xff6d, 0x3086: 0xff95, 0x3087: 0xff6e,
                    0x3088: 0xff96, 0x3089: 0xff97, 0x308a: 0xff98, 0x308b: 0xff99, 0x308c: 0xff9a,
                    0x308d: 0xff9b, 0x308f: 0xff9c, 0x3092: 0xff66, 0x3093: 0xff9d, 0x30fb: 0xff65
                };
                return String.fromCharCode(f[c.charCodeAt(0)]);
            }).replace(/[\u304c\u304e\u3050\u3052\u3054\u3056\u3058\u305a\u305c\u305e\u3060\u3062\u3065\u3067\u3069\u3070\u3073\u3076\u3079\u307c\u3094]/g, function (c) {
                // with daku-ten
                var f = {
                    0x304c: 0xff76, 0x304e: 0xff77, 0x3050: 0xff78, 0x3052: 0xff79, 0x3054: 0xff7a,
                    0x3056: 0xff7b, 0x3058: 0xff7c, 0x305a: 0xff7d, 0x305c: 0xff7e, 0x305e: 0xff7f,
                    0x3060: 0xff80, 0x3062: 0xff81, 0x3065: 0xff82, 0x3067: 0xff83, 0x3069: 0xff84,
                    0x3070: 0xff8a, 0x3073: 0xff8b, 0x3076: 0xff8c, 0x3079: 0xff8d, 0x307c: 0xff8e,
                    0x3094: 0xff73
                };
                return String.fromCharCode(f[c.charCodeAt(0)]) + "\uff9e";
            }).replace(/[\u3071\u3074\u3077\u307a\u307d]/g, function (c) {
                // with han-daku-ten
                var f = {
                    0x3071: 0xff8a, 0x3074: 0xff8b, 0x3077: 0xff8c, 0x307a: 0xff8d, 0x307d: 0xff8e
                };
                return String.fromCharCode(f[c.charCodeAt(0)]) + "\uff9f";
            })
            ;
        }
        if (option.match('c')) {
            // zen-kaku kata-kana to zen-kaku hira-gana
            str = str.replace(/[\u30a1-\u30f6]/g, function (c) {
                return String.fromCharCode(c.charCodeAt(0) - 0x0060)
            });
        } else if (option.match('C')) {
            // zen-kaku hira-gana to zen-kaku kata-kana
            str = str.replace(/[\u3041-\u3086]/g, function (c) {
                return String.fromCharCode(c.charCodeAt(0) + 0x0060)
            });
        }
        if (option.match('r')) {
            // zen-kaku alphabets to han-kaku
            str = str.replace(/[\uff21-\uff3a\uff41-\uff5a]/g, function (c) {
                return String.fromCharCode(c.charCodeAt(0) - 0xFee0)
            });
        } else if (option.match('R')) {
            // han-kaku alphabets to zen-kaku
            str = str.replace(/[A-Za-z]/g, function (c) {
                return String.fromCharCode(c.charCodeAt(0) + 0xFee0)
            });
        }
        if (option.match('n')) {
            // zen-kaku numbers to han-kaku
            str = str.replace(/[\uff10-\uff19]/g, function (c) {
                return String.fromCharCode(c.charCodeAt(0) - 0xfee0);
            });
        } else if (option.match('N')) {
            // han-kaku numbers to zen-kaku
            str = str.replace(/[\u0030-\u0039]/g, function (c) {
                return String.fromCharCode(c.charCodeAt(0) + 0xfee0);
            });
        }
        // Characters included in "a", "A" options are
        // "!" - "~" excluding '"', "'", "\", "~"
        if (option.match('a')) {
            // zen-kaku alphabets and numbers to han-kaku
            str = str.replace(/[\uff01\uff03-\uff06\uff08-\uff3b\uff3d-\uff5d]/g, function (c) {
                return String.fromCharCode(c.charCodeAt(0) - 0xfee0);
            });
        } else if (option.match('A')) {
            // han-kaku alphabets and numbers to zen-kaku
            str = str.replace(/[\u0021\u0023-\u0026\u0028-\u005b\u005d-\u007d]/g, function (c) {
                return String.fromCharCode(c.charCodeAt(0) + 0xfee0);
            });
        }
        if (option.match('s')) {
            // zen-kaku space to han-kaku
            str = str.replace(/\u3000/g, "\u0020");
        } else if (option.match('S')) {
            // han-kaku space to zen-kaku
            str = str.replace(/\u0020/g, "\u3000");
        }
        return str;
    }

    return {
        Convert_JPCharacters:Convert_JPCharacters,
        bindDistrictExtend: bindDistrictExtend,
        bindDistrictExtend2:bindDistrictExtend2,
        bindTown:bindTown,
        preventXSS: preventXSS,
        formatJSONDate: formatJSONDate,
        formatDateFromJson: formatDateFromJson,
        formatDecimalToTime: formatDecimalToTime,
        formatMoney: formatMoney,
        formatMoneyLabel: formatMoneyLabel,
        convertMoneyToInt: convertMoneyToInt,
        convertIntToMoney: convertIntToMoney,
        replaceAllMoney: replaceAllMoney,
        validDate: validDate,
        validateRangeYear: validateRangeYear,
        validateRangeMonth: validateRangeMonth,
        validateDeliveryDate: validateDeliveryDate,
        compareDate: compareDate,
        compareDateRange: compareDateRange,
        validAlphanumeric: validAlphanumeric,
        validPositiveNumeric: validPositiveNumeric,
        validNegativeNumeric: validNegativeNumeric,
        showClientError: showClientError,
        validNumeric: validNumeric,
        validAccount: validAccount,
        validEmail: validEmail,
        validFullHalfSize: validFullHalfSize,
        htmlEncode: htmlEncode,
        htmlDecode: htmlDecode,
        roundTimeByUnit: roundTimeByUnit,
        showMessageDialog: showMessageDialog,
        showMessageDialogCB: showMessageDialogCB,
        htmlDialog: htmlDialog,
        showSubmitConfirmDialog: showSubmitConfirmDialog,
        checkInputNumber: checkInputNumber,
        checkCompareBasetime: checkCompareBasetime,
        buildColumShortText: buildColumShortText,
        buildColumShortTextArea: buildColumShortTextArea,
        builDetailFormCode: builDetailFormCode,
        IsAuthenticateTimeout: IsAuthenticateTimeout,
        setStatusTimeOut: setStatusTimeOut,
        checkTimeout: checkTimeout,
        validURL: validURL,
        focusTextbox: focusTextbox,
        setFocusAmount: setFocusAmount,
        validDecimalNumeric: validDecimalNumeric,
        validDecimalByType: validDecimalByType,
        checkInputPhone: checkInputPhone,
        imeControl: imeControl,
        getDataByAjax: getDataByAjax,
        getDataByAjaxCB: getDataByAjaxCB,
        checkDataExistByAjax: checkDataExistByAjax,
        getImageByAjax: getImageByAjax,
        getMonthCols: getMonthCols,
        bindMonthCols: bindMonthCols,
        bindTotalCols: bindTotalCols,
        bindTagsByCustomer: bindTagsByCustomer,
        buildEditForm: buildEditForm,
        buildConfirmForm: buildConfirmForm,
        validPhone: validPhone,
        GetDateTimeNow: GetDateTimeNow,
        GetDateNow: GetDateNow,
        InitDatepickerMonths: InitDatepickerMonths,
        InitDatepickerDays: InitDatepickerDays,
        checkPermission: checkPermission,
        showConfirmModal: showConfirmModal,
        showMessageModal: showMessageModal,
        displayMultiline: displayMultiline,
        bindDistrictExtend: bindDistrictExtend,

        bindSubContractTypeExtend: bindSubContractTypeExtend,
        bindProductExtend: bindProductExtend,
        setProductNo: setProductNo,
        displayGroupName: displayGroupName,
        checkBrowser: checkBrowser,
        GetSameDayAddMonths: getSameDayAddMonths,
        autoCompleteYearMonthDay: autoCompleteYearMonthDay,
        autoCompleteYearMonth: autoCompleteYearMonth,
        isValidTargetYM: isValidTargetYM,
        ShowPopup: ShowPopup,
        ShowPopupInfor: ShowPopupInfor,
        ClosePopup: ClosePopup,
        ClosePopupInfor: ClosePopupInfor,
        roundMoneyUnit: roundMoneyUnit,
        fixedEncodeURIComponent: fixedEncodeURIComponent,
        CheckValidFullHalfSize: CheckValidFullHalfSize,
        BindSubEngineerCategory: BindSubEngineerCategory,
        compareDateCustom: compareDateCustom
    };
}());