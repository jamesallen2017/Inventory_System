using GMCINVENTORYSYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace GMCINVENTORYSYSTEM.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    [SessionExpireFilter]
    public class HomeController : Controller
    {
        public static int commonreceive = 0, uniquereceive = 0, commonrelease = 0, uniquerelease = 0;
        DBSatellite dBSatellite = new DBSatellite();
        DBMaintenance dBMaintenance = new DBMaintenance();
        DBCentral dBCentral = new DBCentral();

        AllModels models = new AllModels();
        MaintenanceModel maintenanceModel = new MaintenanceModel();
        CentralModel centralModel = new CentralModel();
        SatelliteModel satellite = new SatelliteModel();

        public JsonRequestBehavior JsonRequestBehavior { get; private set; }
        public List<SelectListItem> Data { get; private set; }
        public List<CentralModel> DataList { get; private set; }
        public List<SatelliteModel> SDataList { get; private set; }

        public RoleAccessModel GetRoleAccess()
        {
            DBMaintenance dBMaintenance = new DBMaintenance();
            RoleAccessModel RoleModel = new RoleAccessModel();
            RoleModel = dBMaintenance.RoleAccess(Session["UserRoleID"].ToString());

            return RoleModel;
        }

        [HttpGet]
        public JsonResult GetCountNotification()
        {
            var UserRole = Session["UserRoleID"].ToString();
            var UserID = Session["UserID"].ToString();
            var data = dBCentral.PopulateCountNotificationDB(UserRole, UserID);
            return Json(data, JsonRequestBehavior = JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetRequestCountNotification()
        {
            var UserRole = Session["UserRoleID"].ToString();
            var UserID = Session["UserID"].ToString();
            var DesignationID = Session["DesignationID"].ToString();
            var data = dBCentral.PopulateRequestCountNotificationDB(UserRole, UserID, DesignationID);
            return Json(data, JsonRequestBehavior = JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetNotificationApprovalDetails()
        {
            var UserRole = Session["UserRoleID"].ToString();
            var UserID = Session["UserID"].ToString();
            var data = dBCentral.PopulateNotificationApprovalDetailsDB(UserRole, UserID);
            return Json(DataList = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetNotificationRequestDetails()
        {
            var UserRole = Session["UserRoleID"].ToString();
            var UserID = Session["UserID"].ToString();
            var DesignationID = Session["DesignationID"].ToString();
            var data = dBCentral.PopulateNotificationRequestDetailsDB(UserRole, UserID, DesignationID);
            return Json(DataList = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetItemCountNotification()
        {
            //var GroupID = Session["GroupID"].ToString();
            //var DesignationID = Session["DesignationID"].ToString();
            var data = dBCentral.PopulateItemCountNotificationDB();
            return Json(data, JsonRequestBehavior = JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetNotificationItemDetails()
        {

            var data = dBCentral.PopulateNotificationItemDetailsDB();
            return Json(DataList = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet);
        }

        public ActionResult Dashboard(AllModels model)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("LoginAuthentication", "Login");
            }

            if (Session["UserRoleID"].ToString() == "2" || Session["UserRoleID"].ToString() == "3"
                || Session["UserRoleID"].ToString() == "4" || Session["UserRoleID"].ToString() == "5")
            {
                ViewBag.Content = true;
            }

            string GroupID = Session["GroupID"].ToString();
            string DesignationID = Session["DesignationID"].ToString();
            string UserID = Session["UserID"].ToString();
            maintenanceModel.dtIteminformation = dBMaintenance.PopulateDashboardItemInformationDB(model.Maintenance, GroupID, DesignationID);

            if (model.Maintenance != null)
            {
                Session["ddDesignationID"] = model.Maintenance.DesignationID ?? "";
            }

            maintenanceModel.ddlDesignation = dBCentral.PopulateAllDesignationDB();
            models.Maintenance = maintenanceModel;
            models.RoleAccess = GetRoleAccess();
            return View("~/Views/Home/Index.cshtml", models);
        }

        public ActionResult CreateItemRequest(string id)
        {
            string GroupID = Session["GroupID"].ToString();
            string DesignationID = Session["DesignationID"].ToString();
            //satellite.ddItemList = dBMaintenance.PopulateRequestAllitemStockNo(id, GroupID, DesignationID);
            satellite.Type = id;
            if (id == "Central" || id == "Common")
            {
                satellite.TypeName = "Common";
                id = "1";
            }
            else if (id == "Unique")
            {
                satellite.TypeName = "Unique";
                id = "2";
            }
            satellite.ddlApprover = dBMaintenance.PopulateAllApprover(GroupID, DesignationID);

            satellite.ddItemList = dBMaintenance.PopulateRequestItemDescription(id, GroupID, DesignationID);

            models.Satellite = satellite;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "21")
                {
                    access = true;
                }
            }
            if (access == true)
            {
                return View("~/Views/Home/RequestForm.cshtml", models);
            }
            else
            {
                return Redirect("/Home/Dashboard");
            }
        }

        public ActionResult ApproveRequest()
        {
            var GroupID = Session["GroupID"].ToString();
            var UserID = Session["UserID"].ToString();
            centralModel.dtRoutingApproval = dBCentral.PopulateAllRequestCommon(UserID, GroupID);
            models.Central = centralModel;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Main)
            {
                if (item == "4")
                {
                    access = true;
                }
            }
            if (access == true)
            {
                return View("~/Views/Home/Common_Request.cshtml", models);
            }
            else
            {
                return Redirect("/Home/Dashboard");
            }
        }

        public ActionResult Unique_Request()
        {
            var DesignationID = Session["DesignationID"].ToString();
            var UserID = Session["UserID"].ToString();
            satellite.dtRoutingUniqueApproval = dBSatellite.PopulateAllRequestUnique(UserID, DesignationID);
            models.Satellite = satellite;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "0")
                {
                    access = true;
                }
            }
            if (access == true)
            {
                return View(models);
            }
            else
            {
                return Redirect("/Home/Dashboard");
            }
        }

        [HttpPost]
        public ActionResult CreatePR(AllModels model, string id)
        {
            if (id == "table")
            {
                ViewBag.PRContent = true;

                string GroupID = Session["GroupID"].ToString();
                string DesignationID = Session["DesignationID"].ToString();
                string UserID = Session["UserID"].ToString();

                maintenanceModel.dtItemMonitoring = dBMaintenance.PopulateItemMonitoringDB(model.Maintenance, GroupID, DesignationID);
                maintenanceModel.ddlDesignation = dBCentral.PopulateAllDesignationDB();
                models.Maintenance = maintenanceModel;
            }
            else
            {
                centralModel.ddItemlist = dBCentral.PopulateAllitemStockNo();
                dBCentral.DBCentralReferenceNo();
                centralModel.ReferenceNo = dBCentral.ReferenceNo;
                models.Central = centralModel;
            }
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "31")
                {
                    access = true;
                }
            }
            if (access == true)
            {
                return View("~/Views/Home/RequestPR.cshtml", models);
            }
            else
            {
                return Redirect("/Home/Dashboard");
            }
        }

        public ActionResult CreatePR(string id)
        {
            if (id == "table")
            {
                ViewBag.PRContent = "table";

                string GroupID = Session["GroupID"].ToString();
                string DesignationID = Session["DesignationID"].ToString();
                string UserID = Session["UserID"].ToString();

                maintenanceModel.dtItemMonitoring = dBMaintenance.PopulateItemMonitoringDB(maintenanceModel, GroupID, DesignationID);
                maintenanceModel.ddlDesignation = dBCentral.PopulateAllDesignationDB();
                models.Maintenance = maintenanceModel;
            }
            else
            {
                centralModel.ddItemlist = dBCentral.PopulateAllitemStockNo();
                dBCentral.DBCentralReferenceNo();
                centralModel.ReferenceNo = dBCentral.ReferenceNo;
                models.Central = centralModel;
            }
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "31")
                {
                    access = true;
                }
            }
            if (access == true)
            {
                return View("~/Views/Home/RequestPR.cshtml", models);
            }
            else
            {
                return Redirect("/Home/Dashboard");
            }
        }

        [HttpPost]
        public ActionResult RetrievePRNo()
        {
            dBCentral.DBCentralReferenceNo();
            var PRNo = dBCentral.ReferenceNo;
            return Json(PRNo, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult ReleaseItem()
        //{
        //    var GroupID = Session["GroupID"].ToString();
        //    centralModel.dtReleasing = dBCentral.PopulateAllItemCommonForReleasingDB();
        //    models.Central = centralModel;
        //    models.RoleAccess = GetRoleAccess();

        //    var access = false;
        //    foreach (var item in models.RoleAccess.Sub)
        //    {
        //        if (item == "22" && GroupID == "1")
        //        {
        //            access = true;
        //        }
        //    }
        //    if (access == true)
        //    {
        //        return View("~/Views/Home/Common_Releasing.cshtml", models);
        //    }
        //    else
        //    {
        //        return Redirect("/Home/Dashboard");
        //    }
        //}

        public ActionResult PRReceiving(string id)
        {
            string GroupID = Session["GroupID"].ToString();
            string DesignationID = Session["DesignationID"].ToString();

            if (id == "Central")
            {
                centralModel.dtReceiving = dBCentral.PopulateRequestPRDB(GroupID, DesignationID);
                models.Central = centralModel;
                models.RoleAccess = GetRoleAccess();
                var access = false;
                foreach (var item in models.RoleAccess.Sub)
                {
                    if (item == "32" && GroupID == "1")
                    {
                        access = true;
                    }
                }
                if (access == true)
                {
                    return View("~/Views/Home/Common_Receiving.cshtml", models);
                }
                else
                {
                    return Redirect("/Home/Dashboard");
                }
            }
            else if (id == "Satellite")
            {
                centralModel.dtReceiving = dBCentral.PopulateRequestPRDB(GroupID, DesignationID);
                models.Central = centralModel;
                models.RoleAccess = GetRoleAccess();
                var access = false;
                foreach (var item in models.RoleAccess.Sub)
                {
                    if (item == "32" && GroupID == "2")
                    {
                        access = true;
                    }
                }
                if (access == true)
                {
                    return View("~/Views/Home/Unique_Receiving.cshtml", models);
                }
                else
                {
                    return Redirect("/Home/Dashboard");
                }
            }
            else
            {
                return View();
            }
        }

        //public ActionResult PRReceiving()
        //{
        //    string GroupID = Session["GroupID"].ToString();
        //    string DesignationID = Session["DesignationID"].ToString();
        //    centralModel.dtReceiving = dBCentral.PopulateRequestPRDB(GroupID, DesignationID);
        //    models.Central = centralModel;
        //    models.RoleAccess = GetRoleAccess();

        //    var access = false;
        //    foreach (var item in models.RoleAccess.Sub)
        //    {
        //        if (item == "32" && GroupID == "2")
        //        {
        //            access = true;
        //        }
        //    }
        //    if (access == true)
        //    {
        //        return View("~/Views/Home/Unique_Receiving.cshtml", models);
        //    }
        //    else
        //    {
        //        return Redirect("/Home/Dashboard");
        //    }
        //}

        public ActionResult ReleaseItem(string id)
        {
            string GroupID = Session["GroupID"].ToString();
            string DesignationID = Session["DesignationID"].ToString();
            string UserID = Session["UserID"].ToString();

            if (id == "Central")
            {
                centralModel.dtReleasing = dBCentral.PopulateAllItemCommonForReleasingDB();
                models.Central = centralModel;
                models.RoleAccess = GetRoleAccess();
                var access = false;
                foreach (var item in models.RoleAccess.Sub)
                {
                    if (item == "22" && GroupID == "1")
                    {
                        access = true;
                    }
                }
                if (access == true)
                {
                    return View("~/Views/Home/Common_Releasing.cshtml", models);
                }
                else
                {
                    return Redirect("/Home/Dashboard");
                }
            }
            else if (id == "Satellite")
            {
                satellite.dtUniqueReleasing = dBSatellite.PopulateUniqueItemForReleasing(GroupID, DesignationID, UserID);
                models.Satellite = satellite;
                models.RoleAccess = GetRoleAccess();
                var access = false;
                foreach (var item in models.RoleAccess.Sub)
                {
                    if (item == "22" && GroupID == "2")
                    {
                        access = true;
                    }
                }
                if (access == true)
                {
                    return View("~/Views/Home/Unique_Releasing.cshtml", models);
                }
                else
                {
                    return Redirect("/Home/Dashboard");
                }
            }
            else
            {
                return View();
            }
        }

        public ActionResult CRequestMonitoring()
        {
            return View();
        }

        public ActionResult MonitoringCreatedPR()
        {
            #region ListForPR
            //DBMaintenance dBMaintenance = new DBMaintenance();
            //MaintenanceModel maintenance = new MaintenanceModel();
            //string GroupID = Session["GroupID"].ToString();
            //string DesignationID = Session["DesignationID"].ToString();
            //string UserID = Session["UserID"].ToString();


            //maintenance.dtItemMonitoring = dBMaintenance.PopulateItemMonitoringDB(model, GroupID, DesignationID);
            //maintenance.ddlDesignation = dBCentral.PopulateAllDesignationDB();

            //models.Maintenance = maintenance;
            //models.RoleAccess = GetRoleAccess();
            //return View(models);
            #endregion
            ViewBag.Date = DateTime.Now.ToString("MM/dd/yyyy");
            ViewBag.Week = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
            string Date = ViewBag.Date;
            string Week = ViewBag.Week;
            maintenanceModel.dtItemMonitoring = dBMaintenance.PopulatePRItemMonitoringDB(Date, Week);

            models.Maintenance = maintenanceModel;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "61")
                {
                    access = true;
                }
            }
            if (access == true)
            {
                return View("~/Views/Home/ItemMonitoring.cshtml", models);
            }
            else
            {
                return Redirect("/Home/Dashboard");
            }
        }

        [HttpPost]
        public ActionResult MonitoringCreatedPR(string Date, string Week)
        {
            maintenanceModel.dtItemMonitoring = dBMaintenance.PopulatePRItemMonitoringDB(Date, Week);
            models.Maintenance = maintenanceModel;
            models.RoleAccess = GetRoleAccess();

            return Json(maintenanceModel.dtItemMonitoring);
        }

        public ActionResult InquiryCreatedPR()
        {
            ViewBag.Date = DateTime.Now.ToString("MM/dd/yyyy");
            ViewBag.Week = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
            string Date = ViewBag.Date;
            string Week = ViewBag.Week;
            centralModel.dtPRRequest = dBCentral.PopulateItemPRRequestDB(Session["UserID"].ToString(), Date, Week);
            models.Central = centralModel;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "55")
                {
                    access = true;
                }
            }
            if (access == true)
            {
                return View("~/Views/Home/ItemRequestPRMonitoring.cshtml", models);
            }
            else
            {
                return Redirect("/Home/Dashboard");
            }
        }

        [HttpPost]
        public JsonResult InquiryCreatedPR(string Date, string Week)
        {
            centralModel.dtPRRequest = dBCentral.PopulateItemPRRequestDB(Session["UserID"].ToString(), Date, Week);
            models.Central = centralModel;
            models.RoleAccess = GetRoleAccess();

            return Json(centralModel.dtPRRequest);
        }

        public ActionResult MonitoringManualAdjustment()
        {
            ViewBag.Date = DateTime.Now.ToString("MM/dd/yyyy");
            ViewBag.Week = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
            string Date = ViewBag.Date;
            string Week = ViewBag.Week;
            centralModel.dtManualAdjustment = dBCentral.PopulateManualAdjustDB(Date, Week);
            models.Central = centralModel;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "66")
                {
                    access = true;
                }
            }
            if (access == true)
            {
                return View("~/Views/Home/ManualAdjustMonitoring.cshtml", models);
            }
            else
            {
                return Redirect("/Home/Dashboard");
            }
        }

        [HttpPost]
        public JsonResult MonitoringManualAdjustment(string Date, string Week)
        {
            centralModel.dtManualAdjustment = dBCentral.PopulateManualAdjustDB(Date, Week);
            models.Central = centralModel;
            models.RoleAccess = GetRoleAccess();

            return Json(centralModel.dtManualAdjustment);
        }


        //[HttpPost]
        //public ActionResult MonitoringReleasedItem(AllModels model)
        //{
        //    string GroupID = Session["GroupID"].ToString();
        //    string DesignationID = Session["DesignationID"].ToString();
        //    centralModel.dtReleasedLogs = dBCentral.PopulateReleaseLogsDB(model.Central, DesignationID, GroupID);
        //    centralModel.ddlDesignation = dBCentral.PopulateAllDesignationDB();
        //    models.Central = centralModel;
        //    models.RoleAccess = GetRoleAccess();

        //    var access = false;
        //    foreach (var item in models.RoleAccess.Sub)
        //    {
        //        if (item == "63")
        //        {
        //            access = true;
        //        }
        //    }
        //    if (access == true)
        //    {
        //        return View("~/Views/Home/Releasedlogs.cshtml", models);
        //    }
        //    else
        //    {
        //        return Redirect("/Home/Dashboard");
        //    }
        //}

        public ActionResult MonitoringReleasedItem()
        {
            ViewBag.Date = DateTime.Now.ToString("MM/dd/yyyy");
            ViewBag.Week = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
            string Date = ViewBag.Date;
            string Week = ViewBag.Week;
            string GroupID = Session["GroupID"].ToString();
            string DesignationID = Session["DesignationID"].ToString();
            centralModel.dtReleasedLogs = dBCentral.PopulateReleaseLogsDB(DesignationID, GroupID, Date, Week);
            centralModel.ddlDesignation = dBCentral.PopulateAllDesignationDB();
            models.Central = centralModel;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "63")
                {
                    access = true;
                }
            }
            if (access == true)
            {
                return View("~/Views/Home/Releasedlogs.cshtml", models);
            }
            else
            {
                return Redirect("/Home/Dashboard");
            }
        }

        [HttpPost]
        public JsonResult MonitoringReleasedItem(string Date,string Week,string DesignationID)
        {
            string GroupID = Session["GroupID"].ToString();
            centralModel.dtReleasedLogs = dBCentral.PopulateReleaseLogsDB(DesignationID, GroupID, Date, Week);
            centralModel.ddlDesignation = dBCentral.PopulateAllDesignationDB();
            models.Central = centralModel;
            models.RoleAccess = GetRoleAccess();

            return Json(centralModel.dtReleasedLogs);
        }

        //[HttpPost]
        //public ActionResult ReleasedFilterData(string Date, string Week, string DesignationID)
        //{
        //    var data = dBCentral.PopulateReleaseLogsDB(Session["DesignationID"].ToString(), Session["GroupID"].ToString(), Date, Week);
        //    Session["DateReleased"] = central.DateReleased;
        //    Session["Designation"] = DesignationID;
        //    Session["Report"] = "ReleasedItem";
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public ActionResult ReceivedFilterData(CentralModel central)
        //{
        //    var data = dBCentral.PopulateReceiveLogsDB(central, Session["DesignationID"].ToString(), Session["GroupID"].ToString());
        //    Session["DateReceived"] = central.DateReceived;
        //    Session["Designation"] = central.Designation;
        //    Session["Report"] = "ReceivedItem";
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult MonitoringPRReceived()
        {
            ViewBag.Date = DateTime.Now.ToString("MM/dd/yyyy");
            ViewBag.Week = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
            string Date = ViewBag.Date;
            string Week = ViewBag.Week;
            string GroupID = Session["GroupID"].ToString();
            string DesignationID = Session["DesignationID"].ToString();
            centralModel.dtReceivedLogs = dBCentral.PopulateReceiveLogsDB(DesignationID, GroupID, Date, Week);
            centralModel.ddlDesignation = dBCentral.PopulateAllDesignationDB();
            models.Central = centralModel;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "64")
                {
                    access = true;
                }
            }
            if (access == true)
            {
                return View("~/Views/Home/Receivedlogs.cshtml", models);
            }
            else
            {
                return Redirect("/Home/Dashboard");
            }
        }

        [HttpPost]
        public JsonResult MonitoringPRReceived(string Date, string Week, string DesignationID)
        {
            string GroupID = Session["GroupID"].ToString();
            centralModel.dtReceivedLogs = dBCentral.PopulateReceiveLogsDB(DesignationID, GroupID, Date, Week);
            centralModel.ddlDesignation = dBCentral.PopulateAllDesignationDB();
            models.Central = centralModel;
            models.RoleAccess = GetRoleAccess();

            return Json(centralModel.dtReceivedLogs);
        }

        //[HttpPost]
        //public ActionResult MonitoringPRReceived(AllModels model)
        //{
        //    string GroupID = Session["GroupID"].ToString();
        //    string DesignationID = Session["DesignationID"].ToString();
        //    centralModel.dtReceivedLogs = dBCentral.PopulateReceiveLogsDB(model.Central, DesignationID, GroupID);
        //    centralModel.ddlDesignation = dBCentral.PopulateAllDesignationDB();
        //    models.Central = centralModel;
        //    models.RoleAccess = GetRoleAccess();

        //    var access = false;
        //    foreach (var item in models.RoleAccess.Sub)
        //    {
        //        if (item == "64")
        //        {
        //            access = true;
        //        }
        //    }
        //    if (access == true)
        //    {
        //        return View("~/Views/Home/Receivedlogs.cshtml", models);
        //    }
        //    else
        //    {
        //        return Redirect("/Home/Dashboard");
        //    }
        //}

        public ActionResult CreatedItemRequest(string id)
        {
            ViewBag.Date = DateTime.Now.ToString("MM/dd/yyyy");
            ViewBag.Week = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
            string Date = ViewBag.Date;
            string Week = ViewBag.Week;
            string UserID = Session["UserID"].ToString();
            string GroupID = Session["GroupID"].ToString();
            string DesignationID = Session["DesignationID"].ToString();

            if (id == "Central" || id == "Common")
            {
                centralModel.TypeName = "Common";
                centralModel.dtCommonRequest = dBCentral.PopulateManageCommonRequest(GroupID, DesignationID, UserID, Date, Week);
                models.Central = centralModel;
                models.RoleAccess = GetRoleAccess();
                var access = false;
                foreach (var item in models.RoleAccess.Sub)
                {
                    if (item == "51")
                    {
                        access = true;
                    }
                }
                if (access == true)
                {
                    return View("~/Views/Home/ManageCommonRequest.cshtml", models);
                }
                else
                {
                    return Redirect("/Home/Dashboard");
                }
            }
            else if (id == "Unique")
            {
                satellite.dtUniqueRequest = dBSatellite.PopulateManageUniqueRequest(GroupID, DesignationID, UserID, Date, Week);
                models.Satellite = satellite;
                models.RoleAccess = GetRoleAccess();
                var access = false;
                foreach (var item in models.RoleAccess.Sub)
                {
                    if (item == "51")
                    {
                        access = true;
                    }
                }
                if (access == true)
                {
                    return View("~/Views/Home/ManageUniqueRequest.cshtml", models);
                }
                else
                {
                    return Redirect("/Home/Dashboard");
                }
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public JsonResult CreatedItemRequest(string Date, string Week, string Request)
        {
            string UserID = Session["UserID"].ToString();
            string GroupID = Session["GroupID"].ToString();
            string DesignationID = Session["DesignationID"].ToString();

            if (Request == "Common")
            {
                centralModel.TypeName = "Common";
                centralModel.dtCommonRequest = dBCentral.PopulateManageCommonRequest(GroupID, DesignationID, UserID, Date, Week);
                models.Central = centralModel;
                models.RoleAccess = GetRoleAccess();

                return Json(centralModel.dtCommonRequest);
            }
            else if (Request == "Unique")
            {
                satellite.dtUniqueRequest = dBSatellite.PopulateManageUniqueRequest(GroupID, DesignationID, UserID, Date, Week);
                models.Satellite = satellite;
                models.RoleAccess = GetRoleAccess();

                return Json(satellite.dtUniqueRequest);
            }
            else
            {
                return Json(0);
            }
        }

        //public ActionResult ManageUniqueRequest()
        //{
        //    string GroupID = Session["GroupID"].ToString();
        //    string DesignationID = Session["DesignationID"].ToString();

        //    satellite.dtUniqueRequest = dBSatellite.PopulateManageUniqueRequest(GroupID, DesignationID);
        //    models.Satellite = satellite;
        //    models.RoleAccess = GetRoleAccess();

        //    var access = false;
        //    foreach (var item in models.RoleAccess.Sub)
        //    {
        //        if (item == "51")
        //        {
        //            access = true;
        //        }
        //    }
        //    if (access == true)
        //    {
        //        return View("~/Views/Home/ItemRequestPRMonitoring.cshtml", models);
        //    }
        //    else
        //    {
        //        return Redirect("/Home/Dashboard");
        //    }
        //}

        [HttpPost]
        public ActionResult PRFilterData(MaintenanceModel mainModel)
        {
            var data = dBMaintenance.PopulateItemMonitoringDB(mainModel, Session["GroupID"].ToString(), Session["DesignationID"].ToString()); ;
            Session["PRList"] = mainModel.DesignationID;
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ApprovedItemRequest()
        {
            ViewBag.Date = DateTime.Now.ToString("MM/dd/yyyy");
            ViewBag.Week = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
            string Date = ViewBag.Date;
            string Week = ViewBag.Week;
            centralModel.dtApprovedItem = dBCentral.ApprovedItemDB(Date, Week);
            models.Central = centralModel;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "52")
                {
                    access = true;
                }
            }
            if (access == true)
            {
                return View(models);
            }
            else
            {
                return Redirect("/Home/Dashboard");
            }
        }

        [HttpPost]
        public JsonResult ApprovedItemRequest(string Date, string Week)
        {
            centralModel.dtApprovedItem = dBCentral.ApprovedItemDB(Date, Week);
            models.Central = centralModel;
            models.RoleAccess = GetRoleAccess();

            return Json(centralModel.dtApprovedItem);
        }

        public ActionResult RejectedItemRequest()
        {
            ViewBag.Date = DateTime.Now.ToString("MM/dd/yyyy");
            ViewBag.Week = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
            string Date = ViewBag.Date;
            string Week = ViewBag.Week;
            centralModel.dtRejectedItem = dBCentral.RejectedItemDB(Session["UserID"].ToString(), Date, Week);
            models.Central = centralModel;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "53")
                {
                    access = true;
                }
            }
            if (access == true)
            {
                return View(models);
            }
            else
            {
                return Redirect("/Home/Dashboard");
            }
        }

        [HttpPost]
        public JsonResult RejectedItemRequest(string Date, string Week)
        {
            centralModel.dtRejectedItem = dBCentral.RejectedItemDB(Session["UserID"].ToString(), Date, Week);
            models.Central = centralModel;
            models.RoleAccess = GetRoleAccess();

            return Json(centralModel.dtRejectedItem);
        }

        public ActionResult InquiryReleasedItem()
        {
            ViewBag.Date = DateTime.Now.ToString("MM/dd/yyyy");
            ViewBag.Week = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
            string Date = ViewBag.Date;
            string Week = ViewBag.Week;
            string GroupID = Session["GroupID"].ToString();
            centralModel.dtReleasedItem = dBCentral.ReleasedItemDB(GroupID, Session["UserID"].ToString(), Date, Week);
            models.Central = centralModel;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "54")
                {
                    access = true;
                }
            }
            if (access == true)
            {
                return View("~/Views/Home/ReleasedItem.cshtml", models);
            }
            else
            {
                return Redirect("/Home/Dashboard");
            }
        }

        [HttpPost]
        public JsonResult InquiryReleasedItem(string Date, string Week)
        {
            string GroupID = Session["GroupID"].ToString();
            centralModel.dtReleasedItem = dBCentral.ReleasedItemDB(GroupID, Session["UserID"].ToString(), Date, Week);
            models.Central = centralModel;
            models.RoleAccess = GetRoleAccess();

            return Json(centralModel.dtReleasedItem);
        }

        public ActionResult InquiryPRReceived()
        {
            ViewBag.Date = DateTime.Now.ToString("MM/dd/yyyy");
            ViewBag.Week = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
            string Date = ViewBag.Date;
            string Week = ViewBag.Week;
            string GroupID = Session["GroupID"].ToString();
            centralModel.dtPRReceived = dBCentral.PRReceivedDB(GroupID, Session["UserID"].ToString(), Date, Week);
            models.Central = centralModel;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "56")
                {
                    access = true;
                }
            }
            if (access == true)
            {
                return View("~/Views/Home/PRReceived.cshtml", models);
            }
            else
            {
                return Redirect("/Home/Dashboard");
            }
        }

        [HttpPost]
        public JsonResult InquiryPRReceived(string Date, string Week)
        {
            string GroupID = Session["GroupID"].ToString();
            centralModel.dtPRReceived = dBCentral.PRReceivedDB(GroupID, Session["UserID"].ToString(), Date, Week);
            models.Central = centralModel;
            models.RoleAccess = GetRoleAccess();

            return Json(centralModel.dtPRReceived);
        }

        public ActionResult MonitoringRequestedItem()
        {
            ViewBag.Date = DateTime.Now.ToString("MM/dd/yyyy");
            ViewBag.Week = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
            string Date = ViewBag.Date;
            string Week = ViewBag.Week;
            string GroupID = Session["GroupID"].ToString();
            string DesignationID = Session["DesignationID"].ToString();

            centralModel.dtRequestedItem = dBCentral.PopulateManageRequestedItem(GroupID, DesignationID, Date, Week);
            models.Central = centralModel;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "62")
                {
                    access = true;
                }
            }
            if (access == true)
            {
                return View("~/Views/Home/RequestedItem.cshtml", models);
            }
            else
            {
                return Redirect("/Home/Dashboard");
            }
        }

        [HttpPost]
        public JsonResult MonitoringRequestedItem(string Date, string Week)
        {
            string GroupID = Session["GroupID"].ToString();
            string DesignationID = Session["DesignationID"].ToString();

            centralModel.dtRequestedItem = dBCentral.PopulateManageRequestedItem(GroupID, DesignationID, Date, Week);
            models.Central = centralModel;
            models.RoleAccess = GetRoleAccess();

            return Json(centralModel.dtRequestedItem);
        }

        [HttpPost]
        public JsonResult NotifyRequester(string ReferenceNo, CentralModel central)
        {
            var Data = dBCentral.NotifyRequesterDB(ReferenceNo);
            NotificationComponent NC = new NotificationComponent();
            NC.name = central.Requester;
            NC.UpdateItemRequestNotificaton();
            return Json(central);
        }

        [HttpPost]
        public JsonResult InsertRequestPR(List<CentralModel> ItemRequestList, CentralModel central)
        {
            var Data = dBCentral.InsertRequestPRDB(central, Session["UserID"].ToString());

            if (Data != 0)
            {
                var DataGrid = dBCentral.InsertItemRequestPRDB(ItemRequestList, central);
            }

            return Json(Data);
        }

        [HttpGet]
        public JsonResult RetrieveAllStockNo()
        {
            var Category = dBCentral.PopulateAllitemStockNo();
            return Json(Data = Category, JsonRequestBehavior = JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult RetreiveItemName(string StockNo)
        {
            var Itemaname = dBCentral.SelectItemNameDB(StockNo);
            return Json(Itemaname);
        }

        [HttpPost]
        public JsonResult RetreiveDesignationID(string StockNO)
        {
            var DesignationID = dBCentral.SelectDescriptionDB(StockNO);
            return Json(DesignationID);
        }

        [HttpPost]
        public JsonResult RetrieveItemRequestPR(string PRNo, string ReferenceNo)
        {
            commonreceive = 0;
            uniquereceive = 0;
            var GroupID = Session["GroupID"].ToString();
            var DesignationID = Session["DesignationID"].ToString();
            var Category = dBCentral.PopulateItemRequestForReceivingDB(PRNo, ReferenceNo, GroupID, DesignationID);

            return Json(DataList = Category, JsonRequestBehavior = JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult GetItemLocation(CentralModel central)
        {
            Session["LocationItemNo"] = central.StockNo;
            Session["Model"] = "Central";
            var ItemLocate = central.StockNo;
            return Json(ItemLocate);
        }

        [HttpPost]
        public JsonResult UpdateItemRequestForReceiving(List<CentralModel> ItemRequestList, List<MaintenanceModel> TransactionDetailList, CentralModel central)
        {
            if (commonreceive == 0)
            {
                commonreceive++;
                int i = 0;
                foreach (CentralModel centrallist in ItemRequestList)
                {
                    if (centrallist.ItemReceived != null)
                    {

                        i++;
                    }
                }

                if (i == 0)
                {
                    commonreceive = 0;
                    return Json(0);
                }

                var Data2 = dBCentral.UpdateItemRequestReceivedDB(ItemRequestList, central, Session["UserID"].ToString());
                if (Data2 == 0)
                {
                    commonreceive = 0;
                    return Json("system_error");
                }
                else
                {
                    var Transactions = dBMaintenance.InsertTransactionDetails(TransactionDetailList, Session["UserID"].ToString());

                    return Json(Data2);
                }
            }
            else
            {
                return Json("Cannot");
            }
        }

        [HttpPost]
        public JsonResult UpdateItemUniqueRequestForReceiving(List<CentralModel> ItemRequestList, List<MaintenanceModel> TransactionDetailList, CentralModel central)
        {
            if (uniquereceive == 0)
            {
                uniquereceive++;
                int i = 0;
                foreach (CentralModel centrallist in ItemRequestList)
                {
                    if (centrallist.ItemReceived != null)
                    {

                        i++;
                    }
                }

                if (i == 0)
                {
                    uniquereceive = 0;
                    return Json(0);
                }

                var Data2 = dBCentral.UpdateItemUniqueRequestReceivedDB(ItemRequestList, central, Session["UserID"].ToString());

                if (Data2 == 0)
                {
                    uniquereceive = 0;
                    return Json("system_error");

                }
                else
                {
                    var Transactions = dBMaintenance.InsertTransactionDetails(TransactionDetailList, Session["UserID"].ToString());
                    return Json(Data2);
                }

            }
            else
            {
                return Json("Cannot");
            }
        }

        [HttpPost]
        public JsonResult UpdateItemRequestForReleasing(List<CentralModel> ItemRequestList, List<MaintenanceModel> TransactionDetailList, CentralModel central)
        {
            if (commonrelease == 0)
            {
                commonrelease++;
                //int count = 0; 
                int i = 0;
                List<CentralModel> listStockNO = new List<CentralModel>();

                foreach (CentralModel centrallist in ItemRequestList)
                {
                    if (centrallist.ItemReleased != null)
                    {

                        i++;
                    }
                }

                if (i == 0)
                {
                    commonrelease = 0;
                    return Json(0);
                }

                foreach (CentralModel row in ItemRequestList)
                {
                    dBMaintenance.CheckStockInventoryDB(row.StockNo);

                    var StockonHand = Convert.ToUInt32(dBMaintenance.StockonHand);
                    var SafetyStock = Convert.ToUInt32(dBMaintenance.SafetyStock);

                    var itemreleased = Convert.ToInt32(row.ItemReleased);
                    var TotalStock = StockonHand - itemreleased;

                    if (TotalStock < 0)
                    {
                        commonrelease = 0;
                        return Json("outofstock");
                    }
                }

                var Data2 = dBCentral.UpdateItemCommonReleasedDB(ItemRequestList, central, Session["UserID"].ToString());

                if (Data2 == 0)
                {
                    commonrelease = 0;
                    return Json("system_error");
                }

                else
                {
                    var Transactions = dBMaintenance.InsertTransactionDetails(TransactionDetailList, Session["UserID"].ToString());

                    central.ItemOutStock = dBCentral.PopulateItemCountNotificationDB();
                    central.itemCount = Data2;

                    //if (count != 0)
                    //{

                    //	var To = EmailAddress;
                    //	var Subject = "Test Email";
                    //	var body = "This is a Test Email";

                    //	MaintenanceModel maintenance = new MaintenanceModel();

                    //	maintenance = dBMaintenance.PopulateSMTP();
                    //	try
                    //	{
                    //		MailMessage mail = new MailMessage();

                    //		//email  output
                    //		mail.From = new MailAddress(maintenance.smtpEmail, "procurementsystem@ati.com");
                    //		mail.To.Add("" + To + "");
                    //		mail.Subject = "" + Subject + "";
                    //		mail.Body = "" + body + "";

                    //		//email connection
                    //		SmtpClient SmtpServer = new SmtpClient("" + maintenance.smtpServer + "");
                    //		SmtpServer.Port = Convert.ToInt32(maintenance.smtpPort);
                    //		SmtpServer.Credentials = new NetworkCredential("" + maintenance.smtpEmail + "", "" + maintenance.smtpPassword + "");

                    //		//email sender
                    //		SmtpServer.Send(mail);

                    //		i++;
                    //	}
                    //	catch
                    //	{
                    //		i = 0;
                    //	}
                    //}

                }
                NotificationComponent NC = new NotificationComponent();
                NC.name = central.Requester;
                NC.UpdateItemRequestNotificaton();

                return Json(central);
            }
            else
            {
                return Json("Cannot");
            }
        }

        [HttpPost]
        public JsonResult UpdateRoutingApprovalCommon(CentralModel central)
        {
            var GroupID = Session["GroupID"].ToString();
            var UserID = Session["UserID"].ToString();

            int data = 0;
            var data2 = dBCentral.CheckRequestStatusDB(central.ReferenceNo);

            if (data2 == "APPROVED" || data2 == "REJECTED")
            {
                return Json(data2);
            }

            else
            {
                if (central.TypeRequest == "APPROVED")
                {

                    data = dBCentral.UpdateApprovedRoutingApprovalCommonDB(central, Session["UserID"].ToString());
                }
                else if (central.TypeRequest == "REJECTED")
                {
                    data = dBCentral.UpdateRejectRoutingApprovalCommonDB(central, Session["UserID"].ToString());

                }
                else
                {
                    data = dBCentral.UpdateApproveRoutingApprovalCommonDB(central, Session["UserID"].ToString());
                }
                return Json(data);
            }
        }


        [HttpPost]
        public JsonResult UpdateRoutingApprovalUnique(SatelliteModel satellite)
        {
            var GroupID = Session["GroupID"].ToString();
            var UserID = Session["UserID"].ToString();

            int data = 0;

            if (satellite.TypeRequest == "REJECTED")
            {
                data = dBSatellite.UpdateRejectRoutingApprovalUniqueDB(satellite, Session["UserID"].ToString());

            }
            else
            {

                data = dBSatellite.UpdateApprovedRoutingApprovalUniqueDB(satellite, Session["UserID"].ToString());

            }

            return Json(data);
        }

        [HttpPost]
        public JsonResult RetrieveAllItemRequestCommon(string ReferenceNo)
        {
            var Category = dBCentral.PopulateAllItemCommonRequest(ReferenceNo);
            return Json(DataList = Category, JsonRequestBehavior = JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RetrieveAllItemRequestUnique(string ReferenceNo)
        {
            var Category = dBSatellite.PopulateAllItemUniqueRequest(ReferenceNo);
            return Json(SDataList = Category, JsonRequestBehavior = JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult RetreiveItemCommonForReleasing(string DisplayReferenceNo)
        {
            commonrelease = 0;
            var Category = dBCentral.PopulateItemCommonForReleasingDB(DisplayReferenceNo);
            NotificationComponent NC = new NotificationComponent();
            dBCentral.Update_NotificationViewDB(DisplayReferenceNo);
            NC.name = Session["UserID"].ToString();
            NC.UpdateItemRequestNotificaton();

            return Json(DataList = Category, JsonRequestBehavior = JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult InsertRequest(List<SatelliteModel> ItemRequestList, SatelliteModel satellite)
        {
            string GroupID = Session["GroupID"].ToString();
            string DesignationID = Session["DesignationID"].ToString();

            var Data = dBSatellite.InsertRequestDB(satellite, Session["UserID"].ToString(), GroupID, DesignationID);

            if (Data != 0)
            {
                var DataGrid = dBSatellite.InsertItemRequestDB(ItemRequestList, satellite, GroupID, DesignationID);
            }

            return Json(Data);
        }

        [HttpPost]
        public JsonResult RetreiveItemUniqueForReleasing(string ReferenceNo)
        {
            uniquerelease = 0;
            var Category = dBSatellite.PopulateItemUniqueForReleasingDB(ReferenceNo);
            return Json(SDataList = Category, JsonRequestBehavior = JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateItemUniqueRequestForReleasing(List<SatelliteModel> ItemRequestList, List<MaintenanceModel> TransactionDetailList, SatelliteModel satellite)
        {
            if (uniquerelease == 0)
            {
                uniquerelease++;
                //int count = 0; 
                int i = 0;
                List<SatelliteModel> listStockNO = new List<SatelliteModel>();

                foreach (SatelliteModel centrallist in ItemRequestList)
                {
                    if (centrallist.ItemReleased != null)
                    {

                        i++;
                    }
                }

                if (i == 0)
                {
                    uniquerelease = 0;
                    return Json(0);
                }


                foreach (SatelliteModel row in ItemRequestList)
                {
                    dBMaintenance.CheckStockInventoryDB(row.StockNo);

                    var StockonHand = Convert.ToUInt32(dBMaintenance.StockonHand);
                    var SafetyStock = Convert.ToUInt32(dBMaintenance.SafetyStock);

                    var itemreleased = Convert.ToInt32(row.ItemReleased);
                    var TotalStock = StockonHand - itemreleased;

                    if (TotalStock < 0)
                    {
                        uniquerelease = 0;
                        return Json("outofstock");

                    }
                    //else if (TotalStock <= SafetyStock)
                    //{
                    //    dBCentral.UpdateItemPRStatus(row.StockNo);
                    //}
                }

                var Data2 = dBSatellite.UpdateItemUniqueReleasedDB(ItemRequestList, satellite, Session["UserID"].ToString());

                if (Data2 == 0)
                {
                    uniquerelease = 0;
                    return Json("system_error");

                }

                else
                {
                    var Transactions = dBMaintenance.InsertTransactionDetails(TransactionDetailList, Session["UserID"].ToString());
                    satellite.ItemOutStock = dBCentral.PopulateItemCountNotificationDB();
                    satellite.itemCount = Data2;

                    return Json(satellite);
                }
            }
            else
            {
                return Json("Cannot");
            }
        }

        public ActionResult MonitoringItemMovement()
        {
            ViewBag.Date = DateTime.Now.ToString("MM/dd/yyyy");
            ViewBag.Week = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
            string Date = ViewBag.Date;
            string Week = ViewBag.Week;
            string Movement = "";
            DBMaintenance dbMaintenance = new DBMaintenance();
            maintenanceModel.PopulateTransactions = dbMaintenance.PopulateTransactionsDB(Date, Week, Movement);
            maintenanceModel.TransactionList = dbMaintenance.TransactionListDB();
            models.Maintenance = maintenanceModel;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "67")
                {
                    access = true;
                }
            }
            if (access == true)
            {
                return View("~/Views/Home/TransactionCode.cshtml", models);
            }
            else
            {
                return Redirect("/Home/Dashboard");
            }
        }

        [HttpPost]
        public JsonResult MonitoringItemMovement(string Date, string Week, string Movement)
        {
            DBMaintenance dbMaintenance = new DBMaintenance();
            maintenanceModel.PopulateTransactions = dbMaintenance.PopulateTransactionsDB(Date, Week, Movement);
            maintenanceModel.TransactionList = dbMaintenance.TransactionListDB();
            models.Maintenance = maintenanceModel;
            models.RoleAccess = GetRoleAccess();

            return Json(maintenanceModel.PopulateTransactions);
        }


        #region ItemLocationMonitoring
        public ActionResult MonitoringItemLocation(AllModels models)
        {
            maintenanceModel.dtLocationMonitoring = dBMaintenance.FilterMainItemlocationListDB(models.Maintenance);
            maintenanceModel.ddlMain = dBMaintenance.SelectAllMainItemlocationDB();
            models.Maintenance = maintenanceModel;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "65")
                {
                    access = true;
                }
            }
            if (access == true)
            {
                return View("~/Views/Home/ItemLocationMonitoring.cshtml", models);
            }
            else
            {
                return Redirect("/Home/Dashboard");
            }
        }

        [HttpPost]
        public JsonResult GetLocations(MaintenanceModel maintenance)
        {
            if (maintenance.LocationStatus == null)
            {
                maintenance.LocationStatus = "";
            }
            Session["ItemLocation"] = maintenance.LocationStatus;
            Session["Model"] = "Maintenance";
            var i = maintenance.LocationStatus;
            return Json(i, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public JsonResult SetFilterData(string From, string To, string Designation, string TransactionType, string Type)
        {
            if (Type == "ManualAdjustment")
            {
                Session["ManualFrom"] = From;
                Session["ManualTo"] = To;
            }
            else if (Type == "ItemMovement")
            {
                Session["ItemFrom"] = From;
                Session["ItemTo"] = To;
                Session["TransactionType"] = TransactionType;
            }
            else if (Type == "PRMonitoring")
            {
                Session["PRFrom"] = From;
                Session["PRTo"] = To;
            }
            else if (Type == "ReleasedLogs")
            {
                Session["Report"] = "ReleasedItem";
                Session["ReleasedFrom"] = From;
                Session["ReleasedTo"] = To;
                Session["ReleasedDesignation"] = Designation;
            }
            else if (Type == "ReceivedLogs")
            {
                Session["Report"] = "ReceivedItem";
                Session["ReceivedFrom"] = From;
                Session["ReceivedTo"] = To;
                Session["ReceivedDesignation"] = Designation;
            }
            else if (Type == "Index")
            {
                Session["ddDesignation"] = Designation;
            }
            else if (Type == "PRList")
            {
                Session["PRList"] = Designation;
            }

            return Json(1);
        }
    }
}