using ShipOnline.Models.Define;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShipOnline.DataAccess;
using ShipOnline.Services;
using ShipOnline.Service.Common;
using ShipOnline.Resources;
using ShipOnline.Models.Entity;
using ShipOnline.Models;

namespace ShipOnline.Controllers
{
    public class AdminManageTownController : BaseController
    {
        //
        // GET: /AdminManageTown/
        
        #region LIST
        // GET: /AdminManageDistrict/
        public ActionResult TownList()
        {
            CmnEntityModel currentUser = Session["CmnEntityModel"] as CmnEntityModel;
            var authorityList = currentUser != null ? currentUser.USER_AUTHORITY : 0;
            
            if (currentUser == null || authorityList != 2)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            TownModel model = new TownModel();
            CommonService comService = new CommonService();

            model.CITY_LIST = comService.GetCityList().ToList().Select(
             f => new SelectListItem
             {
                 Value = f.CITY_CD.ToString(),
                 Text = f.CITY_NAME
             }).ToList();
            model.CITY_LIST.Insert(0, new SelectListItem { Value = Constant.DEFAULT_VALUE, Text = "" });

            model.DISTRICT_LIST = comService.GetDistrictList().ToList().Select(
            f => new SelectListItem
            {
                Value = f.CITY_CD.ToString() + "_" + f.DISTRICT_CD.ToString(),
                Text = f.DISTRICT_NAME
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataTableModel dt, TownModel condition)
        {
            if (ModelState.IsValid)
            {
                using (ManageTownService service = new ManageTownService())
                {
                    int total_row = 0;
                    var dataList = service.SearchTownList(dt, condition, out total_row);

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
                                    i.CITY_CD,
                                    i.DISTRICT_CD,
                                    order++,
                                    i.CITY_NAME != null ? HttpUtility.HtmlEncode(i.CITY_NAME) : String.Empty,
                                    i.DISTRICT_NAME != null ? HttpUtility.HtmlEncode(i.DISTRICT_NAME) : String.Empty,
                                    i.TOWN_CD,
                                    i.TOWN_NAME != null ? HttpUtility.HtmlEncode(i.TOWN_NAME) : String.Empty,
                                    i.STATUS =="1"? "Hiển thị" : "Ẩn",
                                    i.DEL_FLG
                                })

                        });

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



        #region REGISTER/ UPDATE
        public ActionResult TownEdit(int CityCd = 0, int DistrictCd = 0, int TownCd = 0)
        {
            CmnEntityModel currentUser = Session["CmnEntityModel"] as CmnEntityModel;
            var authorityList = currentUser != null ? currentUser.USER_AUTHORITY : 0;
            
            if (currentUser == null || authorityList != 2)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            TownModel model = new TownModel();

            CommonService comService = new CommonService();
            ManageTownDa dataAccess = new ManageTownDa();
            if (CityCd > 0 && DistrictCd > 0 && TownCd > 0)
            {
                TownModel infor = new TownModel();
                infor = dataAccess.getInfoTown(CityCd, DistrictCd, TownCd);
                model = infor != null ? infor : model;
            }

            model.CITY_LIST = comService.GetCityList().ToList().Select(
            f => new SelectListItem
            {
                Value = f.CITY_CD.ToString(),
                Text = f.CITY_NAME
            }).ToList();
            model.CITY_LIST.Insert(0, new SelectListItem { Value = Constant.DEFAULT_VALUE, Text = "" });

            model.DISTRICT_LIST = comService.GetDistrictList().ToList().Select(
            f => new SelectListItem
            {
                Value = f.CITY_CD.ToString() + "_" + f.DISTRICT_CD.ToString(),
                Text = f.DISTRICT_NAME
            }).ToList();
            

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(TownModel model)
        {
            try
            {
                using (ManageTownService service = new ManageTownService())
                {
                    if (ModelState.IsValid)
                    {
                        bool isNew = false;

                        if (model.CITY_CD_HIDDEN == 0 && model.DISTRICT_CD_HIDDEN == 0 && model.TOWN_CD_HIDDEN == 0)
                        {
                            isNew = true;

                            service.InsertTown(model);
                            JsonResult result = Json(new { isNew = isNew }, JsonRequestBehavior.AllowGet);
                            return result;
                        }
                        else
                        {
                            isNew = false;

                            service.UpdateTown(model);
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
        public ActionResult CheckExistTownCd(int cityCd, int districtCd, int townCD, int townCdOld)
        {
            if (Request.IsAjaxRequest())
            {
                // Declare new DataAccess object
                ManageTownDa dataAccess = new ManageTownDa();

                var exist = dataAccess.CheckExistTownCd(cityCd, districtCd, townCD, townCdOld);
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
        public ActionResult DeleteTown(long CITY_CD = 0, long DISTRICT_CD = 0, long TOWN_CD = 0)
        {
            if (Request.IsAjaxRequest())
            {
                if (ModelState.IsValid && CITY_CD > 0 && DISTRICT_CD > 0 && TOWN_CD > 0)
                {
                    using (var service = new ManageTownService())
                    {
                        var deleteResult = service.DeleteTown(CITY_CD, DISTRICT_CD, TOWN_CD);

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

        #region CHECK EXIST TOWN exist group
        /// <summary>
        /// CheckExistGroupTown by ForUser
        /// </summary>
        /// <param name="cityCd"></param>
        /// <param name="districtCd"></param>
        /// <param name="townCD"></param>
        /// <param name="townCdOld"></param>
        /// <returns></returns>
        public ActionResult CheckExistGroupTown(int cityCd, int districtCd, int townCD, int forUser)
        {
            if (Request.IsAjaxRequest())
            {
                // Declare new DataAccess object
                ManageTownDa dataAccess = new ManageTownDa();

                var exist = dataAccess.CheckExistGroupTown(cityCd, districtCd, townCD, forUser);
                JsonResult result = Json(new
                {
                    exist
                }, JsonRequestBehavior.AllowGet);

                return result;

            }
            return new EmptyResult();
        }

        #endregion

	}
}