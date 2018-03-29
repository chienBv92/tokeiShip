using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace ShipOnline.Resources
{
    public class Constant
    {
        public const string REGEX_PASSWORD = @"^[a-zA-Z0-9_\!\""\#\$\%\&\'\(\)\=\~\|\-\^\@\[\;\:\]\,\.\/\`\{\+\*\}\>\?]*$";
        /// <summary>Key name for scroll top in session object</summary>
        public const string SESSION_SCROLL_TOP = "SESSION_SCROLL_TOP";

        public const string DEFAULT_VALUE = "0";
        public const int TOKEN_ERROR_CODE = -2147467259;
        public const int dateNormalNumber = 7;

        public const int TIME_OUT = 419;
        public const int NOT_FOUND = 404;
        public const int CREATED = 201;

        public const int SUCCESSFUL = 200;
        public const int INTERNAL_SERVER_ERROR = 500;
        public const int EXPECTATION_FAILED = 417;
        public const int MinuteExtend = 10;
        public const decimal money_inside_fastest = 30000;
        public const decimal money_inside_fast = 25000;
        public const decimal money_inside_normal = 20000;

        public const decimal money_outside_fast = 30000;
        public const decimal money_outside_normal = 25000;

        public const int CITY_ZIP_CD_LENGTH = 6;
        public const int CITY_NAME_LENGTH = 50;
        public const int MIN_INPUT_PASS = 6;
        public const int MAX_INPUT_PASS = 50;
        public const int MIN_USER_ACCOUNT_LENGTH = 6;
        public const int MAX_USER_ACCOUNT_LENGTH = 100;
        public const int MAX_USER_NAME_LENGTH = 50;
        public const int MAX_PHONE_LENGTH = 13;
        public const int MAX_EMAIL_LENGTH = 100;

        public const string PRODUCT_SIZE_TEXT_DEFAULT = "Mặc định";
        public const string ACCEPTANCE_NAME_DEFAULT = "Mới khởi tạo";
    }

    public class WindowName
    {

        public const string COOKIE_NAME = "WindowName";

        public const string MAIN = "Main";

        public static readonly OrderedDictionary Items = new OrderedDictionary
        {
            { "Login", MAIN }
        }.AsReadOnly();
    }

    public class Product_Type
    {
        public const string Normal = "0";

        public const string Broken = "1";

        public const string Cumbrous = "2";

        public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { Normal, "Hàng thường" },
                { Broken, "Hàng dễ vỡ" },
                { Cumbrous, "Hàng cồng kềnh" }
            }.AsReadOnly();
    }

    public class Ship_Type
    {
        public const int None = 0;
        public const int Fastest = 1;

        public const int Fast = 2;

        public const int Normal = 3;

        public const string NormalText = "Chuyển thường";

        public const string FastestText = "Cấp tốc";

        public const string FastText = "Chuyển nhanh";

        public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { None, "" },
                { Fastest, "Cấp tốc" },
                { Fast, "Chuyển nhanh"},
                { Normal, "Chuyển thường"}
            }.AsReadOnly();
    }

    public class User_Pay
    {
        public const string Sender = "0";

        public const string Receiver = "1";

        public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { Sender, "Người gửi trả phí" },
                { Receiver, "Người nhận trả phí" }
            }.AsReadOnly();
    }

    public class Area
    {
        public const string None = "0";

        public const string Person = "1";

        public const string ShopOnline = "2";

        public const string ShopElectrict = "3";

        public const string Company = "4";

        public const string Store = "5";

        public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { None, "Chọn đối tượng" },
                { Person, "Cá nhân" },
                { ShopOnline, "Shop Online" },
                { ShopElectrict, "Thương mại điện tử" },
                { Company, "Công ty" },
                { Store, "Cửa hàng" }
            }.AsReadOnly();
    }

    public class OrderStatus
    {
        public const int Create = 0;

        public const int TakingOrder = 1;

        public const int TakedOrder = 2;

        public const int Shiping = 3;

        public const int Delivery = 4;

        public const int ReShip = 5;

        public const int ReturnOrder = 6;

        public const int ReturnOrderSuccess = 7;

        public const int Finished = 8;

        public const int WaitCOD = 9;

        public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { Create, "Mới khởi tạo" },
                { TakingOrder, "Đã duyệt" },
                { TakedOrder, "Đã lấy hàng" },
                { Shiping, "Đang giao hàng" },
                { Delivery, "Đã giao" },
                { ReShip, "Chờ giao lại" },
                { ReturnOrder, "Hoàn hàng" },
                { ReturnOrderSuccess, " Đã hoàn hàng" },
                { Finished, "Hoàn tất" },
                { WaitCOD, "Chờ COD" }
            }.AsReadOnly();
    }
    public class Other_Requirement
    {
        public const string ViewNotTry = "0";

        public const string CanTry = "1";

        public const string NotView = "2";

        public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { ViewNotTry, "Cho xem không thử" },
                { CanTry, "Cho thử hàng" },
                { NotView, "Không cho xem hàng" }
            }.AsReadOnly();
    }

    public class Product_Weight
    {
        public const int Zero = 0;

        public const int One = 3;
        public const int Four = 4;
        public const int Five = 5;
        public const int Six = 6;
        public const int Seven = 7;
        public const int Eight = 8;
        public const int Nine = 9;
        public const int Ten = 10;
        public const int Eleven = 11;
        public const int twelve = 12;
        public const int thirteen = 13;
        public const int fourteen = 14;
        public const int fifteen = 15;

        public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { Zero, "Chọn khối lượng" },
                { One, "< 3 kg" },
                { Four, "4 kg" },
                { Five, "5 kg" },
                { Six, "6 kg" },
                { Seven, "7 kg" },
                { Eight, "8 kg" },
                { Nine, "9 kg" },
                { Ten, "10 kg" },
                { Eleven, "11 kg" },
                { twelve, "12 kg" },
                { thirteen, "13 kg" },
                { fourteen, "14 kg" },
                { fifteen, "15 kg" }

            }.AsReadOnly();
    }

    public class DeleteFlag
    {
        public const string NON_DELETE = "0";

        public const string DELETE = "1";

        public static readonly OrderedDictionary Items = new OrderedDictionary
        {
            { NON_DELETE, "Chưa xóa" },
            { DELETE, "Đã xóa" }
        }.AsReadOnly();
    }

    public class GroupCdArea
    {
        public const int NON_SET = 0;

        public const int HAVE_SET = 1;

        public static readonly OrderedDictionary Items = new OrderedDictionary
        {
            { NON_SET, "Chưa tạo nhóm" },
            { HAVE_SET, "Đã tạo nhóm" }
        }.AsReadOnly();
    }

    public class OrderDsp
    {
        public const int NON_SET = 0;

        public const int HAVE_SET = 1;

        public static readonly OrderedDictionary Items = new OrderedDictionary
        {
            { NON_SET, "Chưa set" },
            { HAVE_SET, "Đã tạo nhóm" }
        }.AsReadOnly();
    }

    public class LockFlag
    {
        public const string NON_LOCK = "0";

        public const string LOCK = "1";

        public static readonly OrderedDictionary Items = new OrderedDictionary
        {
            { NON_LOCK, "Chưa khóa" },
            { LOCK, "Khóa" }
        }.AsReadOnly();
    }

    public class InsideDistrict
    {
        public const string INSIDE = "1";

        public const string OUTSIDE = "0";

        public static readonly OrderedDictionary Items = new OrderedDictionary
        {
            { INSIDE, "Nội thành" },
            { OUTSIDE, "Ngoại thành" }
        }.AsReadOnly();
    }

    public class EmailConfirmed
    {
        public const string None = "0";

        public const string Yes = "1";

        public const string RePassword = "2";

        public static readonly OrderedDictionary Items = new OrderedDictionary
        {
            { None, "Chưa confirm" },
            { Yes, "Đang sử dụng" },
            { RePassword, "Reset mật khẩu" }
        }.AsReadOnly();
    }

    public class StatusFlag
    {
        public const string NON_DISPLAY = "0";

        public const string DISPLAY = "1";

        public static readonly OrderedDictionary Items = new OrderedDictionary
        {
            { NON_DISPLAY, "Ẩn" },
            { DISPLAY, "Hiển thị" }
        }.AsReadOnly();
    }

    public class User_Authority
    {
        public const int Set_Person  = 0;
        public const string Person = "0";
        public const string PersonText = "Người dùng";

        public const string Group = "1";
        public const string GroupText = "Nhân viên";

        public const string Company = "2";
        public const string CompanyText = "Quản trị";

        public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { Person, "Người dùng" },
                { Group, "Nhân viên" },
                { Company, "Quản trị" }
            }.AsReadOnly();
    }

    public class GroupForUser
    {
        public const int Receive = 0;
        public const int Sender = 1;

        public const string ReceiveUser = "0";
        public const string SenderUser = "1";

        public static readonly OrderedDictionary Items = new OrderedDictionary
            {
                { ReceiveUser, "Khu vực người nhận" },
                { SenderUser, "Khu vực người gửi" }
            }.AsReadOnly();
    }
}