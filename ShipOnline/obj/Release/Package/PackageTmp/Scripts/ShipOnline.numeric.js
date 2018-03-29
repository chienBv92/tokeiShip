
$(function () {
    $(document).off('.numeric');
    $(document).on('focus', '.numeric', function () {
        $(".numeric").css({ "ime-mode": "disabled" }).numeric({ decimal: false, negative: false });
        // Integer
        $(".integer").css({ "ime-mode": "disabled" }).numeric({ decimal: false });
        // A positive number
        $(".positive").css({ "ime-mode": "disabled" }).numeric({ negative: false });
        // Positive integer
        $(".positive-integer").css({ "ime-mode": "disabled" }).numeric({ decimal: false, negative: false });
    });

    $(document).off('input.hour');
    $(document).on('focus', 'input.hour', function () {
        $("input.hour").css({ "ime-mode": "disabled" }).numeric({ decimal: false, negative: false });
    });

    $(document).off('input.hour');
    $(document).on('focusout', 'input.hour', function () {
        var data = ShipOnline.utility.validPositiveNumeric($(this).val()) ? $(this).val() : '00';
        $(this).val(data);
    });

    $(document).off('input.minute');
    $(document).on('focus', 'input.minute', function () {
        $("input.minute").css({ "ime-mode": "disabled" }).numeric({ decimal: false, negative: false });
    });

    $(document).off('input.minute');
    $(document).on('focusout', 'input.minute', function () {
        var data = ShipOnline.utility.validPositiveNumeric($(this).val()) ? $(this).val() : '00';
        $(this).val(data);
    });

    $(document).off('.tell-no');
    $(document).on('focus', '.tell-no', function () {
        $(this).css({ "ime-mode": "disabled" }).numeric({ decimal: false, negative: false });
    });

    $(document).off('.tell-no');
    $(document).on('focusout', '.tell-no', function () {
        var data = ShipOnline.utility.validPositiveNumeric($(this).val()) ? $(this).val() : '';

        $(this).val(data);
    });

    // Event focus in money field
    $(document).off('.negative-money');
    $(document).on('focus', '.negative-money', function () {
        $(this).val($(this).val().replace(/,/g, ''));
        $(".negative-money").css({ "ime-mode": "disabled" }).numeric({ decimal: false, negative: true, maxLength: Constant.POSITIVE_MONEY_MAX_LENGTH });
    });

    // Event focus out money field
    $(document).off('.negative-money');
    $(document).on('focusout', '.negative-money', function () {
        if (this.value.length > 0) {
            var content;
            content = ShipOnline.utility.Convert_JPCharacters(this.value, 'an');

            var data = ShipOnline.utility.validNegativeNumeric(content.replace(/,/g, '')) ? content.replace(/,/g, '') : '0';

            if (typeof ($(this).attr('readonly')) == 'undefined' && data.indexOf('-') === -1 && data.length > Constant.POSITIVE_MONEY_MAX_LENGTH) {
                data = data.substr(0, Constant.POSITIVE_MONEY_MAX_LENGTH);
            }

            data = ShipOnline.utility.convertMoneyToInt(data, true).toString();

            $(this).val(data.replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,"));
        }
        else
        {
            $(this).val(0);
        }
    });

    // Event focus in money field
    $(document).off('.money');
    $(document).on('focus', '.money', function () {
        $(this).val($(this).val().replace(/,/g, ''));
        var maxLength = typeof ($(this).attr('maxlength')) == 'undefined' ? Constant.POSITIVE_MONEY_MAX_LENGTH : parseInt($(this).attr('maxlength'));

        $(".money").css({ "ime-mode": "disabled" }).numeric({ decimal: false, negative: false, maxLength: maxLength });
    });

     //Event focus out money field
    $(document).off('.money');
    $(document).on('focusout', '.money', function () {
        var content;
        content = ShipOnline.utility.Convert_JPCharacters($(this).val(), 'n');

        var value = ShipOnline.utility.convertMoneyToInt(content.split('.')[0].replace('-', ''));
        var data = ShipOnline.utility.validPositiveNumeric(value.toString()) ? parseInt(value) : 0;

        $(this).val(data.toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,"));
    });

     //Event focus out numeric field
    $(document).off('.numeric');
    $(document).on('focusout', '.numeric', function () {
        var content;
        content = ShipOnline.utility.Convert_JPCharacters(this.value, 'n');

        var data = ShipOnline.utility.validPositiveNumeric(content) ? content : '0';

        if (data.length > 0) {
            data = parseInt(data);
        }

        $(this).val(data);
    });

    // Event focus in money field
    $(document).off('.decimal, .decimal_7_2, .decimal_5_2, .decimal_4_2');
    $(document).on('focus', '.decimal, .decimal_7_2, .decimal_5_2, .decimal_4_2', function () {
        $(this).css({ "ime-mode": "disabled" }).numeric({ decimal: '.', negative: false, decimalPlaces: 2 });
    });

    // Event focus out decimal field
    $(document).off('.decimal, .decimal_7_2, .decimal_5_2, .decimal_4_2');
    $(document).on('blur', '.decimal, .decimal_7_2, .decimal_5_2, .decimal_4_2', function () {
        //var zenkaku = '１２３４５６７８９０ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ　';
        //var hankaku = '1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ ';
        var content;        
        content = ShipOnline.utility.Convert_JPCharacters(this.value, 'n');

        if (!ShipOnline.utility.validDecimalNumeric(content)) { // validate
            $(this).val('0');
        } else {
            if (content.indexOf('0') === 0) { // replace digit 0 at first
                $(this).val(parseFloat(content));
            } else {
                var arr = content.split('.');
                var maxLength = 2;

                if ($(this).hasClass('decimal_7_2')) {
                    maxLength = 5;
                }

                if ($(this).hasClass('decimal_5_2')) {
                    maxLength = 3;
                }

                var integerPart = arr[0];

                if (integerPart.length > maxLength) {
                    integerPart = integerPart.substr(0, maxLength);
                }

                if (integerPart.length == 0) {
                    integerPart = '0';
                }

                if (arr.length > 1) {
                    var decimalPart = arr[1];

                    // if not have value after point, add value 0
                    if (decimalPart.length == 0) {
                        decimalPart = '0';
                    }

                    // if decimal part length > 2 character, cut string
                    if (decimalPart.length > 2) {
                        decimalPart = decimalPart.substr(0, 2);
                    }
                } else {
                    // if no have decimal part after dot character
                    decimalPart = '0';
                }

                var returnValue = integerPart + (content.indexOf('.') == -1 ? '' : '.' + decimalPart);

                // set value
                $(this).val(returnValue);
            }
        }
    });

    // Event focus in money field
    $(document).off('.negative-money-max');
    $(document).on('focus', '.negative-money-max', function () {
        $(this).val($(this).val().replace(/,/g, ''));
        $(".negative-money-max").css({ "ime-mode": "disabled" }).numeric({ decimal: false, negative: true, maxLength: Constant.POSITIVE_AMOUNT_MAX_LENGTH });
    });

    // Event focus out money field
    $(document).off('.negative-money-max');
    $(document).on('focusout', '.negative-money-max', function () {
        this.value = this.value.replace(/,/g, '');
        var data = ShipOnline.utility.validNegativeNumeric(this.value) ? this.value : '0';

        if (typeof ($(this).attr('readonly')) == 'undefined' && data.indexOf('-') === -1 && data.length > Constant.POSITIVE_AMOUNT_MAX_LENGTH) {
            data = data.substr(0, Constant.POSITIVE_AMOUNT_MAX_LENGTH);
        }

        data = ShipOnline.utility.convertMoneyToInt(data, true).toString();

        $(this).val(data.replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1,"));
    });
});
