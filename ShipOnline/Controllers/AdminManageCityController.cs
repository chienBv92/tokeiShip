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

namespace ShipOnline.Controllers
{
    public class AdminManageCityController : BaseController
    {
        #region List
        public ActionResult CityList()
        {
            CmnEntityModel currentUser = Session["CmnEntityModel"] as CmnEntityModel;

            var authorityList = currentUser != null? currentUser.USER_AUTHORITY : 0;
            

            if (currentUser == null || authorityList != 2)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            CityModel model = new CityModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataTableModel dt, CityModel condition)
        {
            if (ModelState.IsValid)
            {
                using (ManageCityService service = new ManageCityService())
                {
                    int total_row = 0;
                    var dataList = service.SearchCityList(dt, condition, out total_row);

                    int order = 1;
                    int totalRowCount = dataList.Count();
                    int lastItem = dt.iDisplayLength + dt.iDisplayStart;

                    var result = Json(
                        new
                        {
                            sEcho = dt.sEcho,
                            iTotalRecords = total_row,
                            iTotalDisplayRecords = total_row,
                            aaData = (from i in dataList
                                      select new object[]
                                {
                                    order++,
                                    i.CITY_CD,
                                    i.CITY_ZIP_CD != null ? HttpUtility.HtmlEncode(i.CITY_ZIP_CD) : String.Empty,
                                    i.CITY_NAME != null ? HttpUtility.HtmlEncode(i.CITY_NAME) : String.Empty,
                                    i.STATUS =="1"? "Hiển thị" : "Ẩn",
                                    i.DEL_FLG
                                })

                        });

                    result.MaxJsonLength = Int32.MaxValue;
                    return result;
                }
            }
            return new EmptyResult();
        }

        #endregion

        #region REGISTER/ UPDATE
        public ActionResult CityEdit(int CityCd = 0)
        {
            CmnEntityModel currentUser = Session["CmnEntityModel"] as CmnEntityModel;
            var authorityList = currentUser != null ? currentUser.USER_AUTHORITY : 0;

            if (currentUser == null || authorityList != 2)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            CityModel model = new CityModel();
            ManageCityDa dataAccess = new ManageCityDa();
            if (CityCd > 0)
            {
                model = dataAccess.getInfoCity(CityCd);
            }
            
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(CityModel model)
        {
            try
            {
                using (ManageCityService service = new ManageCityService())
                {
                    if (ModelState.IsValid)
                    {
                        bool isNew = false;
                        MstCity entity = new MstCity();
                        //int ExistCity = da.MstCity.SingleOrDefault(i => i.CITY_CD == model.CITY_CD) == null? 0 : 1;
                        if (model.CITY_CD_HIDDEN == 0)
                        {
                                isNew = true;
                                entity.DEL_FLG = DeleteFlag.NON_DELETE;
                                entity.CITY_CD = model.CITY_CD;
                                entity.CITY_ZIP_CD = model.CITY_ZIP_CD;
                                entity.CITY_NAME = model.CITY_NAME;
                                entity.DSP_ORDER = model.DSP_ORDER;
                                entity.STATUS = model.STATUS;
                                
                                service.InsertCity(entity);
                                JsonResult result = Json(new { isNew = isNew }, JsonRequestBehavior.AllowGet);
                                return result;
                        }
                        else
                        {
                            isNew = false;
                            entity.CITY_CD = model.CITY_CD;
                            entity.DEL_FLG = DeleteFlag.NON_DELETE;
                            entity.CITY_ZIP_CD = model.CITY_ZIP_CD;
                            entity.CITY_NAME = model.CITY_NAME;
                            entity.DSP_ORDER = model.DSP_ORDER;
                            entity.STATUS = model.STATUS;

                            service.UpdateCity(entity);
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

        //check exist
        public ActionResult CheckExistCityCd(int cityCd, int cityCdOld)
        {
            if (Request.IsAjaxRequest())
            {
                // Declare new DataAccess object
                ManageCityDa dataAccess = new ManageCityDa();

                var exist = dataAccess.FindCity(cityCd, cityCdOld);
                JsonResult result = Json(new
                {
                    exist
                }, JsonRequestBehavior.AllowGet);

                return result;

            }
            return new EmptyResult();
        }
       
        #endregion

        #region DELETE
        public ActionResult Delete(long CITY_CD = 0)
        {
            if (Request.IsAjaxRequest())
            {
                if (ModelState.IsValid && CITY_CD > 0)
                {
                    using (var service = new ManageCityService())
                    {
                        var deleteResult = service.DeleteCity(CITY_CD);

                        JsonResult result = Json(new
                        {
                            statusCode = deleteResult ? Constant.SUCCESSFUL : Constant.INTERNAL_SERVER_ERROR
                        }, JsonRequestBehavior.AllowGet);

                        return result;
                    }
                }
                else
                {
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
                }
            }

            return new EmptyResult();
        }

        #endregion


	}
}