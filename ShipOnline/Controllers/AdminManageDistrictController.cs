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
    public class AdminManageDistrictController : BaseController
    {
        #region LIST
        // GET: /AdminManageDistrict/
        public ActionResult DistrictList()
        {
            CmnEntityModel currentUser = Session["CmnEntityModel"] as CmnEntityModel;
            var authorityList = currentUser.USER_AUTHORITY;

            if (currentUser == null || authorityList != 2)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            DistrictModel model = new DistrictModel();
            CommonService comService = new CommonService();

            model.CITY_LIST = comService.GetCityList().ToList().Select(
            f => new SelectListItem
            {
                Value = f.CITY_CD.ToString(),
                Text = f.CITY_NAME
            }).ToList();
            model.CITY_LIST.Insert(0, new SelectListItem { Value = Constant.DEFAULT_VALUE, Text = "" });

            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataTableModel dt, DistrictModel condition)
        {
            if (ModelState.IsValid)
            {
                using (ManageDistrictService service = new ManageDistrictService())
                {
                    int total_row = 0;
                    var dataList = service.SearchDistrictList(dt, condition, out total_row);

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
                                    order++,
                                    i.CITY_NAME != null ? HttpUtility.HtmlEncode(i.CITY_NAME) : String.Empty,
                                    i.DISTRICT_CD,
                                    i.DISTRICT_NAME != null ? HttpUtility.HtmlEncode(i.DISTRICT_NAME) : String.Empty,
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
        public ActionResult DistrictEdit(int CityCd = 0, int DistrictCd = 0)
        {
            CmnEntityModel currentUser = Session["CmnEntityModel"] as CmnEntityModel;
            var authorityList = currentUser != null ? currentUser.USER_AUTHORITY : 0;

            if (currentUser == null || authorityList != 2)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            DistrictModel model = new DistrictModel();

            CommonService comService = new CommonService();
            ManageDistrictDa dataAccess = new ManageDistrictDa();
            if (CityCd > 0 && DistrictCd >0)
            {
                model = dataAccess.getInfoDistrict(CityCd, DistrictCd);
            }

            model.CITY_LIST = comService.GetCityList().ToList().Select(
            f => new SelectListItem
            {
                Value = f.CITY_CD.ToString(),
                Text = f.CITY_NAME
            }).ToList();
            model.CITY_LIST.Insert(0, new SelectListItem { Value = Constant.DEFAULT_VALUE, Text = "" });

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(DistrictModel model)
        {
            try
            {
                using (ManageDistrictService service = new ManageDistrictService())
                {
                    if (ModelState.IsValid)
                    {
                        bool isNew = false;
                        MstDistrict entity = new MstDistrict();

                        if (model.CITY_CD_HIDDEN == 0 && model.DISTRICT_CD_HIDDEN == 0)
                        {
                            isNew = true;

                            service.InsertDistrict(model);
                            JsonResult result = Json(new { isNew = isNew }, JsonRequestBehavior.AllowGet);
                            return result;
                        }
                        else
                        {
                            isNew = false;

                            service.UpdateDistrict(model);
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
        public ActionResult CheckExistDistrictCd(int cityCd, int districtCd, int districtCdOld)
        {
            if (Request.IsAjaxRequest())
            {
                // Declare new DataAccess object
                ManageDistrictDa dataAccess = new ManageDistrictDa();

                var exist = dataAccess.CheckExistDistrictCd(cityCd, districtCd, districtCdOld);
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
        public ActionResult DeleteDistrict(long CITY_CD = 0, long DISTRICT_CD = 0)
        {
            if (Request.IsAjaxRequest())
            {
                if (ModelState.IsValid && CITY_CD > 0 && DISTRICT_CD > 0)
                {
                    using (var service = new ManageDistrictService())
                    {
                        var deleteResult = service.DeleteDistrict(CITY_CD, DISTRICT_CD);

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

        /// <summary>
        /// DELETE Town in Area
        /// </summary>
        /// <param name="cityCd"></param>
        /// <param name="districtCd"></param>
        /// <param name="townCD"></param>
        /// <param name="forUser"></param>
        /// <returns></returns>
        public ActionResult DeleteTownInArea(long cityCd = 0, long districtCd = 0, long townCD = 0, int forUser = 0)
        {
            if (Request.IsAjaxRequest())
            {
                if (ModelState.IsValid && cityCd > 0 && districtCd > 0 && townCD > 0)
                {
                    using (var service = new ManageDistrictService())
                    {
                        var deleteResult = service.DeleteTownInArea(cityCd, districtCd, townCD, forUser);

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

        public ActionResult DeleteGroupArea(long groupCd = 0, int forUser = 0)
        {
            if (Request.IsAjaxRequest())
            {
                if (ModelState.IsValid && groupCd > 0)
                {
                    using (var service = new ManageDistrictService())
                    {
                        var deleteResult = service.DeleteGroupArea(groupCd, forUser);

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

        #region LIST GROUP AREA
        // GET: /AdminManageDistrict/
        [HttpGet]
        public ActionResult GroupList()
        {
            CmnEntityModel currentUser = Session["CmnEntityModel"] as CmnEntityModel;
            var authorityList = currentUser.USER_AUTHORITY;

            if (currentUser == null || authorityList != 2)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            GroupAreaModel model = new GroupAreaModel();
            CommonService comService = new CommonService();

            var tmpCondition = GetRestoreData() as GroupAreaModel;
            if (tmpCondition != null)
            {
                model = tmpCondition;
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

            model.TOWN_LIST = comService.GetTownList().ToList().Select(
           f => new SelectListItem
           {
               Value = f.CITY_CD.ToString() + "_" + f.DISTRICT_CD.ToString() + "_" + f.TOWN_CD.ToString(),
               Text = f.TOWN_NAME
           }).ToList();

            ViewBag.GetGroupForUser = new SelectList(UtilityServices.UtilityServices.GetGroupForUser(), "Value", "Text");

            return View(model);
        }

        [HttpPost]
        public ActionResult GroupList(DataTableModel dt, GroupAreaModel condition)
        {
            if (ModelState.IsValid)
            {
                using (ManageDistrictService service = new ManageDistrictService())
                {
                    int total_row = 0;
                    //condition = Session["SearchGroupAreaList"] as GroupAreaModel ?? new GroupAreaModel();
                    var dataList = service.SearchGroupAreatList(dt, condition, out total_row);

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
                                    i.GROUP_CD,
                                    order++,
                                    i.GROUP_NAME != null ? HttpUtility.HtmlEncode(i.GROUP_NAME) : String.Empty,
                                    i.FOR_USER_TEXT = GroupForUser.Items[i.FOR_USER].ToString(),
                                    i.INS_DATE != null ? i.INS_DATE.Value.ToString("dd/MM/yyyy") : String.Empty,
                                    i.UPD_DATE != null ? i.UPD_DATE.Value.ToString("dd/MM/yyyy") : String.Empty,
                                    i.DEL_FLG,
                                    i.FOR_USER
                                })

                        });
                    SaveRestoreData(condition);
                    result.MaxJsonLength = Int32.MaxValue;
                    return result;
                }
            }
            return new EmptyResult();
        }
        #endregion

        #region REGISTER/ UPDATE GROUP AREA
        public ActionResult GroupDistrictEdit(long GroupCd = 0)
        {
            CmnEntityModel currentUser = Session["CmnEntityModel"] as CmnEntityModel;
            var authorityList = currentUser != null ? currentUser.USER_AUTHORITY : 0;

            if (currentUser == null || authorityList != 2)
            {
                return RedirectToAction("Login", "UserAccount");
            }

            GroupAreaModel model = new GroupAreaModel();

            CommonService comService = new CommonService();
            ManageDistrictDa dataAccess = new ManageDistrictDa();
            model.LIST_TOWN = new List<GroupTown>();
            if (GroupCd > 0)
            {
                model = dataAccess.getInfoGroupArea(GroupCd);
                var listTown = dataAccess.GetListTownInArea(model);
                if (listTown.Count() > 0)
                {
                    model.LIST_TOWN = listTown.ToList();
                    
                        foreach (var Town in model.LIST_TOWN)
                        {
                            if (model.FOR_USER == GroupForUser.Receive)
                            {
                            Town.DSP_ORDER = Town.DSP_ORDER_RECEIVE;
                            Town.GROUP_CD = Town.GROUP_CD_RECEIVE;
                            }
                            else
                            {
                                Town.DSP_ORDER = Town.DSP_ORDER_SENDER;
                                Town.GROUP_CD = Town.GROUP_CD_SENDER;
                            }
                        }
                        model.LIST_TOWN = model.LIST_TOWN.OrderBy(i => i.DSP_ORDER).ToList();

                }
                model.FOR_USER_TEXT = GroupForUser.Items[model.FOR_USER].ToString();
            }
            model.CITY_LIST = comService.GetCityList().ToList().Select(
            f => new SelectListItem
            {
                Value = f.CITY_CD.ToString(),
                Text = f.CITY_NAME
            }).ToList();
            //model.CITY_LIST.Insert(0, new SelectListItem { Value = Constant.DEFAULT_VALUE, Text = "" });

            model.DISTRICT_LIST = comService.GetDistrictList().ToList().Select(
           f => new SelectListItem
           {
               Value = f.CITY_CD.ToString() + "_" + f.DISTRICT_CD.ToString(),
               Text = f.DISTRICT_NAME
           }).ToList();

            model.TOWN_LIST = comService.GetTownList().ToList().Select(
           f => new SelectListItem
           {
               Value = f.CITY_CD.ToString() + "_" + f.DISTRICT_CD.ToString() + "_" + f.TOWN_CD.ToString(),
               Text = f.TOWN_NAME
           }).ToList();

            ViewBag.GetGroupForUser = new SelectList(UtilityServices.UtilityServices.GetGroupForUser(), "Value", "Text");


            return View(model);
        }

        [HttpPost]
        public ActionResult EditGroup(GroupAreaModel model)
        {
            try
            {
                using (ManageDistrictService service = new ManageDistrictService())
                {
                    if (ModelState.IsValid)
                    {
                        bool isNew = false;
                        bool success = true;
                        MstDistrict entity = new MstDistrict();

                        if (model.GROUP_CD == 0)
                        {
                            isNew = true;

                            var groupCd = service.InsertGroupArea(model);
                            if (groupCd > 0)
                            {
                                model.GROUP_CD = groupCd;
                                foreach (var town in model.LIST_TOWN)
                                {
                                    town.GROUP_CD = groupCd;
                                    var updateTown = service.UpdateTownInArea(town, model.FOR_USER);
                                    success = updateTown > 0 ? true : false;
                                    if (!success)
                                        break;
                                }
                            }
                            else
                            {
                                success = false;
                            }
                            JsonResult result = Json(new { isNew = isNew, success = success }, JsonRequestBehavior.AllowGet);
                            return result;
                        }
                        else
                        {
                            isNew = false;

                            var update = service.UpdateGroupArea(model);
                            if (update > 0)
                            {
                                foreach (var town in model.LIST_TOWN)
                                {
                                    town.GROUP_CD = model.GROUP_CD;
                                    var updateTown = service.UpdateTownInArea(town, model.FOR_USER);
                                    success = updateTown > 0 ? true : false;
                                    if (!success)
                                        break;
                                }
                            }else
                            {
                                success = false;
                            }

                            JsonResult result = Json(new { isNew = isNew, success = success }, JsonRequestBehavior.AllowGet);
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


    }
}