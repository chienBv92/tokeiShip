using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShipOnline.Resources;
using ShipOnline.Models.Define;
using ShipOnline.Models.Entity;
using ShipOnline.Services;
using ShipOnline.UtilityService;
using ShipOnline.DataAccess;
using ShipOnline.Models;
using ShipOnline.Service.Common;
using System.Data;
using iEnterAsia.iseiQ.ReportServices;
using RazorPDF;
using iTextSharp.text.html.simpleparser;
using iTextSharp;
using Rotativa;
using Rotativa.MVC;

namespace ShipOnline.Controllers
{
    public class OrderShipController : BaseController
    {
        //
        // GET: /OrderShip/
        public ActionResult Index()
        {
            return View();
        }
        #region REGISTER/ UPDATE
        [HttpPost]
        public ActionResult RegisterOrder(OrderShipModel model)
        {
            try
            {
                using (OrderShipService service = new OrderShipService())
                {
                    if (ModelState.IsValid)
                    {
                        bool isNew = false;
                        
                        if (model.ORDER_ID == 0)
                        {
                            isNew = true;
                            model.DEL_FLG = DeleteFlag.NON_DELETE;

                            var shipId = service.InsertOrder(model);
                            JsonResult result = Json(new { ShipID = shipId }, JsonRequestBehavior.AllowGet);
                            return result;
                        }
                        else
                        {
                            isNew = false;

                            //service.UpdateCity(entity);
                            JsonResult result = Json(new { isNew = isNew }, JsonRequestBehavior.AllowGet);
                            return result;
                        }
                    }
                    else
                    {
                        var ErrorMessages = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                    }

                    return new EmptyResult();
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                System.Web.HttpContext.Current.Session["ERROR"] = ex;
                return new EmptyResult();
            }
        }
        #endregion

        #region VIEW ORDER SHIP
        public ActionResult ViewOrderDetail(long OrderId = 0)
        {
            OrderShipModel model = new OrderShipModel();
            CmnEntityModel currentUser = Session["CmnEntityModel"] as CmnEntityModel;
            var authorityList = currentUser != null ? currentUser.USER_AUTHORITY : 0;

            if (currentUser == null || currentUser.USER_ID == 0)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            CommonService comService = new CommonService();
            CommonDa da = new CommonDa();
            OrderShipDa dataAccess = new OrderShipDa();
            if (OrderId > 0)
            {
                OrderShipModel infor = new OrderShipModel();
                infor = dataAccess.getInforOrder(OrderId);
                model = infor != null ? infor : model;
                var inforUserSender = da.getInforUser(model.USER_ID);
                if (inforUserSender != null)
                {
                    model.SENDER_CITY_NAME = inforUserSender.CITY_NAME;
                    model.SENDER_DISTRICT_NAME = inforUserSender.DISTRICT_NAME;
                    model.SENDER_TOWN_NAME = inforUserSender.TOWN_NAME;
                    model.SENDER_NAME = inforUserSender.USER_NAME;
                    model.SENDER_PHONE = inforUserSender.USER_PHONE;
                    model.SENDER_ADDRESS = inforUserSender.USER_ADDRESS;
                }
                model.RECEIVE_TOTAL_MONEY = model.PRICE_PRODUCT.HasValue ? model.PRICE_PRODUCT.Value : 0;
                if (model.USER_PAYMENT == User_Pay.Receiver)
                {
                    decimal shipMoney = model.PRICE_SHIP;
                    decimal priceProduct = model.PRICE_PRODUCT.HasValue ? model.PRICE_PRODUCT.Value : 0;
                    model.RECEIVE_TOTAL_MONEY = priceProduct + shipMoney;
                }

                model.ORDER_STATUS_TEXT = OrderStatus.Items[model.ORDER_STATUS].ToString();
                model.OTHER_REQUIREMENT_TEXT = Other_Requirement.Items[model.OTHER_REQUIREMENT].ToString();
                model.PRODUCT_TYPE_TEXT = Product_Type.Items[model.PRODUCT_TYPE].ToString();
                model.SHIP_TYPE_TEXT = Ship_Type.Items[model.SHIP_TYPE].ToString();
                model.PRODUCT_WEIGHT_TEXT = Product_Weight.Items[model.PRODUCT_WEIGHT].ToString();
                model.PRODUCT_SIZE_TEXT = Constant.PRODUCT_SIZE_TEXT_DEFAULT;
                if (string.IsNullOrEmpty(model.ACCEPTANCE_NAME))
                {
                    model.ACCEPTANCE_NAME = Constant.ACCEPTANCE_NAME_DEFAULT;
                }
               

                if (model.PRODUCT_LENGTH != null && model.PRODUCT_HEIGHT != null && model.PRODUCT_WIDTH != null && model.PRODUCT_LENGTH > 0 && model.PRODUCT_HEIGHT > 0 && model.PRODUCT_WIDTH > 0)
                {
                    model.PRODUCT_SIZE_TEXT = model.PRODUCT_LENGTH.ToString() + "x" + model.PRODUCT_WIDTH.ToString() + "x" + model.PRODUCT_HEIGHT.ToString();
                }

            }


            return this.PartialView("ViewOrderDetail", model);
        }
        #endregion


        #region LIST
        [HttpGet]
        public ActionResult OrderShipList()
        {
            CmnEntityModel currentUser = Session["CmnEntityModel"] as CmnEntityModel;
            var authorityList = currentUser != null ? currentUser.USER_AUTHORITY : 0;

            if (currentUser == null || currentUser.USER_ID == 0)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            OrderShipCondition model = new OrderShipCondition();

            CommonService comService = new CommonService();
           
            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataTableModel dt, OrderShipCondition condition)
        {
            if (ModelState.IsValid)
            {
                using (OrderShipService service = new OrderShipService())
                {
                    int total_row = 0;
                    var OrderSummaryInfo = new OrderShipCondition();
                    if (!string.IsNullOrEmpty(condition.ORDER_STATUS_LIST))
                    {
                        List<long> statusList = condition.ORDER_STATUS_LIST.Split(',').Select(long.Parse).ToList();
                        if (statusList.Count > 0)
                        {
                            condition.ORDER_STATUS_LIST = string.Join("','", statusList.ToArray());
                        }
                    }
                    var totaldataList = service.SearchOrderShipListTotal(dt, condition);
                    var dataList = service.SearchOrderShipList(dt, condition, out total_row);
                    var dataByStatusWaitCOD = service.SearchOrderShipByStatus(OrderStatus.WaitCOD);
                    var totalDataNoneStatus = service.SearchOrderListTotalNoneStatus(dt, condition);

                    OrderSummaryInfo.TotalStatus_0 = totalDataNoneStatus.Where(i => i.ORDER_STATUS == OrderStatus.Create).ToList().Count();
                    OrderSummaryInfo.TotalStatus_1 = totalDataNoneStatus.Where(i => i.ORDER_STATUS == OrderStatus.TakingOrder).ToList().Count();
                    OrderSummaryInfo.TotalStatus_2 = totalDataNoneStatus.Where(i => i.ORDER_STATUS == OrderStatus.TakedOrder).ToList().Count();
                    OrderSummaryInfo.TotalStatus_3 = totalDataNoneStatus.Where(i => i.ORDER_STATUS == OrderStatus.Shiping).ToList().Count();
                    OrderSummaryInfo.TotalStatus_4 = totalDataNoneStatus.Where(i => i.ORDER_STATUS == OrderStatus.Delivery).ToList().Count();
                    OrderSummaryInfo.TotalStatus_5 = totalDataNoneStatus.Where(i => i.ORDER_STATUS == OrderStatus.ReShip).ToList().Count();
                    OrderSummaryInfo.TotalStatus_6 = totalDataNoneStatus.Where(i => i.ORDER_STATUS == OrderStatus.ReturnOrder).ToList().Count();
                    OrderSummaryInfo.TotalStatus_7 = totalDataNoneStatus.Where(i => i.ORDER_STATUS == OrderStatus.ReturnOrderSuccess).ToList().Count();
                    OrderSummaryInfo.TotalStatus_8 = totalDataNoneStatus.Where(i => i.ORDER_STATUS == OrderStatus.Finished).ToList().Count();
                    OrderSummaryInfo.TotalStatus_9 = totalDataNoneStatus.Where(i => i.ORDER_STATUS == OrderStatus.WaitCOD).ToList().Count();

                    OrderSummaryInfo.TotalStatusAll = totalDataNoneStatus.Count();
                    //var dataListWaitCOD = totaldataList.Where(i => i.ORDER_STATUS == OrderStatus.WaitCOD).ToList();
                    //var OrderMonth = totaldataList.Where(i => i.CREATE_DATE.ToString("MM/yyyy").b);

                    foreach (var data in totaldataList)
                    {
                        OrderSummaryInfo.TotalPriceProduct += data.PRICE_PRODUCT.Value;
                        if (data.USER_PAYMENT.Equals(User_Pay.Sender))
                        {
                            OrderSummaryInfo.TotalPriceShip += data.PRICE_SHIP;
                        }
                    }
                    OrderSummaryInfo.TotalMoney = OrderSummaryInfo.TotalPriceProduct - OrderSummaryInfo.TotalPriceShip;
                    decimal totalPriceWaitCOD = 0;
                    decimal totalShipWaitCOD = 0;
                    foreach (var data in dataByStatusWaitCOD)
                    {
                        totalPriceWaitCOD += data.PRICE_PRODUCT.Value;
                        if (data.USER_PAYMENT.Equals(User_Pay.Sender))
                        {
                            totalShipWaitCOD += data.PRICE_SHIP;
                        }
                    }
                    OrderSummaryInfo.TotalWaitCOD = totalPriceWaitCOD - totalShipWaitCOD;
                    OrderSummaryInfo.thisMonth = Utility.GetCurrentDateOnly().Month;
                    var thismonth = Utility.GetCurrentDateOnly().ToString("MM/yyyy");
                    var dataThisMonth = service.SearchOrderByMonth(thismonth);
                    OrderSummaryInfo.TotalOrderthisMonth = dataThisMonth.Count();

                    int totalRowCount = dataList.Count();
                    int lastItem = dt.iDisplayLength + dt.iDisplayStart;

                            var tableData = (from i in dataList
                                      select new object[]
                                {
                                    i.ORDER_ID,
                                    i.CREATE_DATE.ToString("dd/MM/yyyy"),
                                    i.SHIP_CODE,
                                    i.RECEIVED_NAME,
                                    i.RECEIVED_PHONE,
                                    i.PRODUCT_NAME,
                                    i.PRICE_PRODUCT,
                                    i.USER_PAYMENT == User_Pay.Sender ?i.PRICE_SHIP : 0,
                                    i.RECEIVE_HOUR_FROM +"h - " + i.RECEIVE_HOUR_TO +"h Ngày:" +i.RECEIVE_TIME_DATE.ToString("dd/MM/yyyy"), 
                                    OrderStatus.Items[i.ORDER_STATUS].ToString()
                                });
                     
                    var result = Json(new
                    {
                        sEcho = dt.sEcho,
                        iTotalRecords = total_row,
                        iTotalDisplayRecords = total_row,
                        objOrderSummaryInfo = OrderSummaryInfo,
                        aaData = tableData
                    });
                    Session["SearchOrderShipList"] = condition;

                    result.MaxJsonLength = Int32.MaxValue;
                    return result;
                }
            }
            else
            {
                var ErrorMessages = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
            }
            return new EmptyResult();
        }
        #endregion

        #region EXPORT CSV
        [HttpPost]
        public ActionResult ExportCSVCustom()
        {
            CommonDa da = new CommonDa();
            OrderShipDa dataAccess = new OrderShipDa();
            CmnEntityModel currentUser = Session["CmnEntityModel"] as CmnEntityModel;
            var authorityList = currentUser != null ? currentUser.USER_AUTHORITY : 0;

            if (currentUser == null || currentUser.USER_ID == 0)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            var modelCondition = Session["SearchOrderShipList"] as OrderShipCondition ?? new OrderShipCondition();

            string[] columns = this.CreateColumnsExportInfo();
            string fileName = "Danh_Sách_Đơn" + Utility.GetCurrentDateTime().ToString("dd/MM/yyyy HH:mm:ss") + ".csv";
            var dataExport = dataAccess.getDataForCSV(modelCondition);

            DataTable dt = ConvertToDataTableForExport(this, dataExport, columns);
            ReportServices.ExportToCsvData(this, dt, fileName, columns);

            return new EmptyResult();
        }

        private string[] CreateColumnsExportInfo()
        {
            List<string> columns = new List<string>();
            columns.Add("STT");
            columns.Add("Ngày khởi tạo");
            columns.Add("Mã đơn hàng");
            columns.Add("Tên người nhận");
            columns.Add("Số điện thoại");
            columns.Add("Tên hàng");
            columns.Add("Tiền thu hộ");
            columns.Add("Cước phí");
            columns.Add("Thời gian nhận");
            columns.Add("Trạng thái");

            return columns.ToArray();
        }

        public static DataTable ConvertToDataTableForExport(BaseController controller, IList<OrderShipModel> dataList, string[] columns)
        {
            DataTable dataTable = new DataTable("Dynamic");
            //Get all the properties
            for (int i = 0; i < columns.Length; i++)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(i.ToString());
            }

            for (int i = 0; i < dataList.Count(); i++ )
            {
                var dateString = dataList[i].RECEIVE_HOUR_FROM + "h - " + dataList[i].RECEIVE_HOUR_TO + "h Ngày:" + dataList[i].RECEIVE_TIME_DATE.ToString("dd/MM/yyyy");

                var values = new object[columns.Length];
                var row = 0;
                values[row++] = i + 1;

                values[row++] = ReportServices.FormatStringCsv(dataList[i].CREATE_DATE.ToString("dd/MM/yyyy"));
                values[row++] = ReportServices.FormatStringCsv(dataList[i].SHIP_CODE);
                values[row++] = ReportServices.FormatStringCsv(dataList[i].RECEIVED_NAME);
                values[row++] = ReportServices.FormatStringCsv(dataList[i].RECEIVED_PHONE);
                values[row++] = ReportServices.FormatStringCsv(dataList[i].PRODUCT_NAME);
                values[row++] = ReportServices.FormatDynamicDecimalCsv(dataList[i].PRICE_PRODUCT.Value);
                values[row++] = ReportServices.FormatDynamicDecimalCsv(dataList[i].PRICE_SHIP);
                values[row++] = ReportServices.FormatStringCsv(dateString);
                values[row++] = ReportServices.FormatStringCsv(OrderStatus.Items[dataList[i].ORDER_STATUS].ToString());

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
        #endregion

        #region PRINT PDF
        [HttpPost]
        public ActionResult PrintOrderCustom(string ORDER_ID_STRING)
        {
            CommonDa da = new CommonDa();
            OrderShipDa dataAccess = new OrderShipDa();
            List<OrderShipModel> listOrderShip = new List<OrderShipModel>();
            OrderShipModel model = new OrderShipModel();

            if (!string.IsNullOrEmpty(ORDER_ID_STRING))
            {
                List<long> ORDER_ID_LIST = ORDER_ID_STRING.Split(',').Select(long.Parse).ToList();
                if (ORDER_ID_LIST.Count > 0)
                {
                    for (int i = 0; i < ORDER_ID_LIST.Count(); i++)
                    {
                        if (ORDER_ID_LIST[i] > 0)
                        {
                            var infor = dataAccess.getInforOrder(ORDER_ID_LIST[i]);
                            model = infor != null ? infor : model;
                            var inforUserSender = da.getInforUser(model.USER_ID);
                            if (inforUserSender != null)
                            {
                                model.SENDER_CITY_NAME = inforUserSender.CITY_NAME;
                                model.SENDER_DISTRICT_NAME = inforUserSender.DISTRICT_NAME;
                                model.SENDER_TOWN_NAME = inforUserSender.TOWN_NAME;
                                model.SENDER_NAME = inforUserSender.USER_NAME;
                                model.SENDER_PHONE = inforUserSender.USER_PHONE;
                                model.SENDER_ADDRESS = inforUserSender.USER_ADDRESS;
                            }
                            model.RECEIVE_TOTAL_MONEY = model.PRICE_PRODUCT.HasValue ? model.PRICE_PRODUCT.Value : 0;
                            if (model.USER_PAYMENT == User_Pay.Receiver)
                            {
                                decimal shipMoney = model.PRICE_SHIP;
                                decimal priceProduct = model.PRICE_PRODUCT.HasValue ? model.PRICE_PRODUCT.Value : 0;
                                model.RECEIVE_TOTAL_MONEY = priceProduct + shipMoney;
                            }

                            model.ORDER_STATUS_TEXT = OrderStatus.Items[model.ORDER_STATUS].ToString();
                            model.OTHER_REQUIREMENT_TEXT = Other_Requirement.Items[model.OTHER_REQUIREMENT].ToString();
                            model.PRODUCT_TYPE_TEXT = Product_Type.Items[model.PRODUCT_TYPE].ToString();
                            model.SHIP_TYPE_TEXT = Ship_Type.Items[model.SHIP_TYPE].ToString();
                            model.PRODUCT_WEIGHT_TEXT = Product_Weight.Items[model.PRODUCT_WEIGHT].ToString();
                            model.PRODUCT_SIZE_TEXT = Constant.PRODUCT_SIZE_TEXT_DEFAULT;
                            if (string.IsNullOrEmpty(model.ACCEPTANCE_NAME))
                            {
                                model.ACCEPTANCE_NAME = Constant.ACCEPTANCE_NAME_DEFAULT;
                            }

                            if (model.PRODUCT_LENGTH != null && model.PRODUCT_HEIGHT != null && model.PRODUCT_WIDTH != null && model.PRODUCT_LENGTH > 0 && model.PRODUCT_HEIGHT > 0 && model.PRODUCT_WIDTH > 0)
                            {
                                model.PRODUCT_SIZE_TEXT = model.PRODUCT_LENGTH.ToString() + "x" + model.PRODUCT_WIDTH.ToString() + "x" + model.PRODUCT_HEIGHT.ToString();
                            }
                            listOrderShip.Add(model);
                        }
                    }
                        
                }
                return new ViewAsPdf("ViewOrderDetailPDF", listOrderShip);
                //return View("ViewOrderDetailPDF", model);
            }

            return new EmptyResult();
        }


        #endregion

    }
}