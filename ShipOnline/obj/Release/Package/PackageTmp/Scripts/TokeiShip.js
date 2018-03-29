
var TokeiShip = TokeiShip || {};
TokeiShip.Utility = (function () {

    function validateDataSelectTime(){
        var invalidMess = false;
        var product_type = $('#PRODUCT_TYPE').val();

        var valid1 = TokeiShip.Utility.validateProductName();
        var valid2 = TokeiShip.Utility.validateReceivedName();
        var valid3 = TokeiShip.Utility.validateReceivedPhone();
        var valid4 = TokeiShip.Utility.validateReceivedAddress();
        var valid5 = TokeiShip.Utility.validateWeight();
        if (valid1 & valid2 & valid3 & valid4 & valid5)
            invalidMess = true;
        
        if (product_type == 2) {
            var valid7 = TokeiShip.Utility.validateProductSize();
            if (invalidMess && valid7)
            {
                invalidMess = true;
            } else {
                invalidMess = false;
            }
        }

        return invalidMess;

    };

    function validateTimeFromTo() {
        var selectTimeFrom = $('.selectTimeFrom').text();
        var takeHourFrom = $('.takeHourFrom').val();
        var selectTimeTo = $('.selectTimeTo').text();
        var takeHourTo = $('.takeHourTo').val();
        var selectDate = $('.selectDate').text();
        var takeDate = $('.takeDate').val();
        var selectMoney = $('.selectMoney').text();
        var shipMoney = $('.shipMoney').val();

        if (selectTimeFrom == null || selectTimeFrom.trim() == "" || selectTimeTo == null || selectTimeTo.trim() == "" || selectDate == null || selectDate.trim() == "" || selectMoney == null || selectMoney.trim() == "") { // required content
            $('.selectDateTime-error').text('Vui lòng chọn thời gian nhận');
            return false;
        } else if (parseInt(takeHourFrom) > parseInt(takeHourTo)) {
            $('.selectDateTime-error').text('Khoảng thời gian không đúng!');
            return false;
        } else {
            $('.selectDateTime-error').text('');
            return true;
        }
    }

    function validateProductName() {
        var productName = $('#PRODUCT_NAME').val();
        if (productName == null || productName.trim() == "") { // required content
            $('.product_name-error').text('Vui lòng nhập tên hàng hóa!');
            return false;
        } else if (productName.trim().length > 50) {
            $('.product_name-error').text('Tên hàng hóa không được quá 50 kí tự!');
            return false;
        } else {
            $('.product_name-error').text('');
            return true;
        }
    }

    function validateWeight() {
        var weight = $('#PRODUCT_WEIGHT').val();
        if (weight == null || weight.trim() == "" || weight == 0) { // required content
            $('.weight-error').text('Vui lòng chọn khối lượng!');
            return false;
        } else {
            $('.weight-error').text('');
            return true;
        }
    }

    function validateProductPrice() {
        var price = $('#PRICE_PRODUCT').val();
        if (price != null && price.length > 0) { // required content
            var price = ShipOnline.utility.convertMoneyToInt(price, true);
        }
        if (price > Constant.Max_PRICE_PRODUCT) {
            $('.price-error').text('Tiền thu hộ tối đa là 30 triệu VND!');
            return false;
        } else {
            $('.price-error').text('');
            return true;
        }
    }

    function validateProductSize() {
        var length = $('#PRODUCT_LENGTH').val();
        var width = $('#PRODUCT_WIDTH').val();
        var height = $('#PRODUCT_HEIGHT').val();

        if (length == null || length.trim() == "" || length == 0 ||
            width == null || width.trim() == "" || width == 0 ||
            height == null || height.trim() == "" || height == 0) { // required content
            $('.product-size-error').text('Vui lòng nhập kích thước hàng hóa!');
            return false;
        } if (length > 60 || width > 60 || height > 60) { // Gia tri max
            $('.product-size-error').text('Kích thước tối đa là 60');
            return false;
        }

        else {
            $('.product-size-error').text('');
            return true;
        }
    }

    function validateReceivedName() {
        var receivedName = $('#RECEIVED_NAME').val();
        if (receivedName == null || receivedName.trim() == "") { // required content
            $('.received_name-error').text('Vui lòng nhập tên người nhận!');
            return false;
        } else if (receivedName.trim().length > 50) {
            $('.received_name-error').text('Tên người nhận không được quá 50 kí tự!');
            return false;
        } else {
            $('.received_name-error').text('');
            return true;
        }
    }

    function validateReceivedPhone() {
        var receivedPhone = $('#RECEIVED_PHONE').val();
        if (receivedPhone == null || receivedPhone.trim() == "") { // required content
            $('.received_phone-error').text('Vui lòng nhập số điện thoại người nhận!');
            return false;
        } else if (receivedPhone.trim().length > 11) {
            $('.received_phone-error').text('Số điện thoại không được quá 13 kí tự!');
            return false;
        } else if (!ShipOnline.utility.validPhone(receivedPhone)) {
            $('.received_phone-error').text("Số điện thoại không đúng định dạng");
        } else {
            $('.received_phone-error').text('');
            return true;
        }
    }

    function validateReceivedAddress() {
        var cityCD = $('#RECEIVED_CITY').val();

        var districtCodeKey = $('#DISTRICT_CD_KEY').val();
        var districtCode = districtCodeKey != null ? districtCodeKey.split('_')[1] : 0;
        var townCdKey = $('#TOWN_CD_KEY').val();
        var townCd = townCdKey != null ? townCdKey.split('_')[2] : 0;
        var address = $('#RECEIVED_ADDRESS').val();

        if ((cityCD == null || cityCD.trim() == "" || cityCD == 0) ||
            (districtCodeKey == null || districtCodeKey.trim() == "" || districtCode == 0) ||
            (townCdKey == null || townCdKey.trim() == "" || townCd == 0) ||
            (address == null || address.trim() == "")) {
            $('.address-error').text('Vui lòng nhập địa chỉ người nhận!');
            return false;
        }else {
            $('.address-error').text('');
            return true;
        }
    }

    function caculator() {
        var productType = $('#PRODUCT_TYPE').val();
        return productType;
    }


    return {
        validateProductName: validateProductName,
        validateWeight: validateWeight,
        validateProductPrice: validateProductPrice,
        validateProductSize:validateProductSize,
        validateReceivedPhone:validateReceivedPhone,
        validateReceivedName: validateReceivedName,
        validateReceivedAddress:validateReceivedAddress,
        validateDataSelectTime: validateDataSelectTime,
        validateTimeFromTo:validateTimeFromTo,
        caculator: caculator
    };

}());
