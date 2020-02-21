using GMCINVENTORYSYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace GMCINVENTORYSYSTEM.Controllers
{
    [SessionExpireFilter]
    public class MaintenanceController : Controller
    {

        public JsonRequestBehavior JsonRequestBehavior { get; private set; }
        public List<SelectListItem> Data { get; private set; }

        AllModels models = new AllModels();
        MaintenanceModel maintenance = new MaintenanceModel();
        DBCentral dBCentral = new DBCentral();
        DBMaintenance dBMaintenance = new DBMaintenance();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserManagement(string id, string UserID)
        {
            MaintenanceModel maintenance = new MaintenanceModel();
            DBMaintenance dBMaintenance = new DBMaintenance();
            if (id == "table")
            {
                ViewBag.MaintenanceContent = "table";
                maintenance.dtUserInformation = dBMaintenance.PopulateUserListDB();
                maintenance.StatusList = dBMaintenance.LocationStatusListDB();

            }
            else if (id == "create")
            {
                maintenance.ddlDeparment = dBMaintenance.DepartmentListDB();
                maintenance.ddlGroup = dBMaintenance.SelectAllUserGroupDB();
                maintenance.ddlUserRole = dBMaintenance.UserRoleListDB();
                ViewBag.MaintenanceContent = "create";

            }
            else if (id == "edit")
            {
                ViewBag.MaintenanceContent = "edit";
                maintenance = dBMaintenance.PopulateUserInformationDB(UserID);
                maintenance.ddlGroup = dBMaintenance.SelectAllUserGroupDB();
                maintenance.ddlUserRole = dBMaintenance.UserRoleListDB();
                maintenance.ddlDeparment = dBMaintenance.DepartmentListDB();
                maintenance.ddlDesignation = dBMaintenance.SelectUserDesignationDB(maintenance.GroupID);
            }
            models.Maintenance = maintenance;
            models.RoleAccess = GetRoleAccess();
            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "74")
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

        public ActionResult ItemLocation()
        {
            MaintenanceModel maintenance = new MaintenanceModel();
            DBMaintenance dBMaintenance = new DBMaintenance();

            maintenance.ddlMain = dBMaintenance.SelectAllMainItemlocationDB();
            models.Maintenance = maintenance;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "72")
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

        public ActionResult EditItemLocation()
        {
            MaintenanceModel maintenance = new MaintenanceModel();
            DBMaintenance dBMaintenance = new DBMaintenance();

            //maintenance.ddlMain = dBMaintenance.SelectAllMainItemlocationDB();
            maintenance.ddlMain = dBMaintenance.SelectAllMainItemlocationStatusDB();

            maintenance.dtRack = dBMaintenance.PopulateAllMainInformationDB();
            maintenance.StatusList = dBMaintenance.LocationStatusListDB();
            models.Maintenance = maintenance;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "72")
                {
                    access = true;
                }
            }
            if (access == true)
            {
                return View("~/Views/Maintenance/E_ItemLocation.cshtml", models);
            }
            else
            {
                return Redirect("/Home/Dashboard");
            }
        }

        //public List<TreeViewModel> GetItemTreeData(string id)
        //{
        //    List<TreeViewModel> Nodes = new List<TreeViewModel>();
        //    if (id == "create")
        //    {
        //        ItemLocationEntities entities = new ItemLocationEntities();

        //        //Loop and add the Parent Nodes.
        //        foreach (RACK rack in entities.RACKs)
        //        {
        //            Nodes.Add(new TreeViewModel { id = rack.RackNo.ToString(), parent = "#", text = rack.RackName });
        //        }
        //        //Loop and add the Child Nodes.
        //        foreach (DRAWER drawer in entities.DRAWERs)
        //        {
        //            if (drawer.MainID == "RACK 001")
        //            {
        //                //Nodes.Add(new TreeViewModel { id = drawer.MainID.ToString() + "-" + drawer.DrawerID.ToString(), parent = drawer.MainID.ToString(), text = drawer.DrawerName });
        //                Nodes.Add(new TreeViewModel { id = drawer.DrawerID.ToString(), parent = drawer.MainID.ToString(), text = drawer.DrawerName });
        //            }
        //        }
        //        //Loop and add the Child Nodes.
        //        foreach (BIN bin in entities.BINs)
        //        {
        //            Nodes.Add(new TreeViewModel { id = bin.BinID.ToString(), parent = bin.DrawerID.ToString(), text = bin.BinName });
        //        }
        //        //Loop and add the Child Nodes.
        //        foreach (ITEM_LOCATION itemlocation in entities.ITEM_LOCATION)
        //        {
        //            Nodes.Add(new TreeViewModel { id = itemlocation.BinNo.ToString() + "-" + itemlocation.ItemLocationID.ToString(), parent = itemlocation.BinNo.ToString(), text = itemlocation.ItemName });
        //        }

        //        //Serialize to JSON string.
        //        ViewBag.Json = (new JavaScriptSerializer()).Serialize(Nodes);
        //    }
        //    else
        //    {
        //        DBMaintenance dBMaintenance = new DBMaintenance();
        //        RoleEntities entities = new RoleEntities();
        //        MainAccess MainData = new MainAccess();
        //        SubAccess SubData = new SubAccess();
        //        MainData.MainList = dBMaintenance.PopulateMainDatabase(id);
        //        SubData.SubList = dBMaintenance.PopulateSubDatabase(id);
        //        int count = 0;

        //        foreach (USERMAINMENU type in entities.USERMAINMENUs)
        //        {
        //            count = 0;
        //            foreach (MainAccess mainItem in MainData.MainList)
        //            {
        //                if (mainItem.MainID == "2" || mainItem.MainID == "6" || mainItem.MainID == "7")
        //                {
        //                    if (mainItem.MainID == type.MainAccessKey.ToString())
        //                    {
        //                        Nodes.Add(new TreeViewModel { id = type.MainAccessKey.ToString(), parent = "#", text = type.MainName, state = new TreeViewAttribute { selected = true } });
        //                        count++;
        //                        break;
        //                    }
        //                }
        //            }
        //            if (count == 0)
        //            {
        //                Nodes.Add(new TreeViewModel { id = type.MainAccessKey.ToString(), parent = "#", text = type.MainName, state = new TreeViewAttribute { selected = false } });
        //            }
        //        }
        //        foreach (USERSUBMENU subType in entities.USERSUBMENUs)
        //        {
        //            count = 0;
        //            foreach (SubAccess subItem in SubData.SubList)
        //            {
        //                if (subItem.SubID == subType.SubAccessKey.ToString())
        //                {
        //                    Nodes.Add(new TreeViewModel { id = subType.SubMainAccessKey.ToString() + "-" + subType.SubAccessKey.ToString(), parent = subType.SubMainAccessKey.ToString(), text = subType.SubName, state = new TreeViewAttribute { selected = true } });
        //                    count++;
        //                    break;
        //                }
        //            }
        //            if (count == 0)
        //            {
        //                Nodes.Add(new TreeViewModel { id = subType.SubMainAccessKey.ToString() + "-" + subType.SubAccessKey.ToString(), parent = subType.SubMainAccessKey.ToString(), text = subType.SubName, state = new TreeViewAttribute { selected = false } });
        //            }
        //        }

        //        ViewBag.Json = (new JavaScriptSerializer()).Serialize(Nodes);

        //    }
        //    return Nodes;
        //}

        public ActionResult ManualAdjustment()
        {
            maintenance.ddlAllItemStockNo = dBCentral.PopulateAllitemStockNo();
            models.Maintenance = maintenance;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "73")
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

        public ActionResult ItemManagement(string id, string ItemCode)
        {
            DBMaintenance dBMaintenance = new DBMaintenance();
            MaintenanceModel maintenance = new MaintenanceModel();
            string GroupID = Session["GroupID"].ToString();
            string DesignationID = Session["DesignationID"].ToString();
            string UserID = Session["UserID"].ToString();

            if (id == "table")
            {
                ViewBag.ItemMaster = true;
                maintenance.dtIteminformation = dBMaintenance.PopulateItemInformationDB(GroupID, DesignationID);

            }
            else if (id == "create")
            {
                ViewBag.ItemMaster = false;
                maintenance.ddlType = dBMaintenance.PopulateTypeOfItem();
            }
            else
            {
                maintenance = dBMaintenance.Edit_PopulateItemInformationDB(ItemCode);

                maintenance.ddlType = dBMaintenance.PopulateTypeOfItem();
                maintenance.ddlGroup = dBMaintenance.SelectAllUserGroupDB();
                maintenance.ddlDesignation = dBMaintenance.SelectUserDesignationDB(maintenance.Group);
                maintenance.PopulateItemLocation = dBMaintenance.PopulateItemLocationDB(ItemCode);
                maintenance.ddlMain = dBMaintenance.SelectAllMainItemlocationDB();
                maintenance.StatusList = dBMaintenance.LocationStatusListDB();
            }
            models.Maintenance = maintenance;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "71")
                {
                    access = true;
                }
            }
            if (access == true)
            {
                if (id == "edit")
                {
                    return View("~/Views/Maintenance/E_ItemMasterList.cshtml", models);
                }
                else
                {
                    return View("~/Views/Maintenance/ItemMasterList.cshtml", models);
                }
            }
            else
            {
                return Redirect("/Home/Dashboard");
            }
        }

        //public ActionResult EditItemManagement(string id)
        //{
        //    DBMaintenance dBMaintenance = new DBMaintenance();

        //    MaintenanceModel maintenance = new MaintenanceModel();

        //    maintenance = dBMaintenance.Edit_PopulateItemInformationDB(id);

        //    maintenance.ddlType = dBMaintenance.PopulateTypeOfItem();
        //    maintenance.ddlGroup = dBMaintenance.SelectAllUserGroupDB();
        //    maintenance.ddlDesignation = dBMaintenance.SelectUserDesignationDB(maintenance.Group);
        //    maintenance.PopulateItemLocation = dBMaintenance.PopulateItemLocationDB(id);
        //    maintenance.ddlMain = dBMaintenance.SelectAllMainItemlocationDB();
        //    maintenance.StatusList = dBMaintenance.LocationStatusListDB();

        //    models.Maintenance = maintenance;
        //    models.RoleAccess = GetRoleAccess();

        //    var access = false;
        //    foreach (var item in models.RoleAccess.Sub)
        //    {
        //        if (item == "71")
        //        {
        //            access = true;
        //        }
        //    }
        //    if (access == true)
        //    {
        //        return View("~/Views/Maintenance/E_ItemMasterlist.cshtml", models);
        //    }
        //    else
        //    {
        //        return Redirect("/Home/Dashboard");
        //    }

        //}

        public ActionResult SMTP()
        {
            maintenance = dBMaintenance.PopulateSMTP();
            models.Maintenance = maintenance;
            models.RoleAccess = GetRoleAccess();

            var access = false;
            foreach (var item in models.RoleAccess.Sub)
            {
                if (item == "76")
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
        public JsonResult Update_SMTP(MaintenanceModel model)
        {

            var i = dBMaintenance.Update_SMTPDB(model);

            return Json(i);
        }
        [HttpPost]
        public JsonResult SMTP_TestEmail(string EmailAddress)
        {
            int i = 0;

            var To = EmailAddress;
            var Subject = "Test Email";
            var body = "This is a Test Email";

            maintenance = dBMaintenance.PopulateSMTP();
            try
            {
                MailMessage mail = new MailMessage();

                //email  output
                mail.From = new MailAddress(maintenance.smtpEmail, "procurementsystem@ati.com");
                mail.To.Add("" + To + "");
                mail.Subject = "" + Subject + "";
                mail.Body = "" + body + "";

                //email connection
                SmtpClient SmtpServer = new SmtpClient("" + maintenance.smtpServer + "");
                SmtpServer.Port = Convert.ToInt32(maintenance.smtpPort);
                SmtpServer.Credentials = new NetworkCredential("" + maintenance.smtpEmail + "", "" + maintenance.smtpPassword + "");

                //email sender
                SmtpServer.Send(mail);

                i++;
            }
            catch
            {
                i = 0;
            }

            return Json(i);
        }

        [HttpPost]
        public JsonResult InsertRackInformation(List<MaintenanceModel> ItemLocationList)
        {
            var result = "";
            DBMaintenance dBMaintenance = new DBMaintenance();
            MaintenanceModel maintenance = new MaintenanceModel();
            maintenance.ddlMain = dBMaintenance.SelectAllMainItemlocationStatusDB();
            foreach (var item in maintenance.ddlMain)
            {
                foreach (var item2 in ItemLocationList.OrderBy(o => o.Rows))
                {
                    if (item2.RackNo.Trim() == item.Value.Trim() || item.Value.Trim().Equals(item2.RackNo.Trim(), StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (result == "")
                        {
                            result = "Exist-" + item2.Rows;
                        }
                        else
                        {
                            result = result + ", " + item2.Rows;
                            char[] param = { ',' };
                            result = result.TrimStart(param).Trim();
                        }
                    }
                }
            }
            if (result != "")
            {
                return Json(result);
            }

            var i = dBMaintenance.InsertRackInformationDB(ItemLocationList);

            return Json(i);

        }

        [HttpPost]
        public JsonResult InsertDrawerInformation(List<MaintenanceModel> ItemLocationList)
        {
            var result = "";
            var valres = "";
            DBMaintenance dBMaintenance = new DBMaintenance();
            foreach (var rack in ItemLocationList.OrderBy(r => r.MainNo))
            {
                if (valres == "" || valres != rack.MainNo)
                {
                    valres = rack.MainNo;
                }
                else if (valres == rack.MainNo)
                {
                    continue;
                }
                maintenance.ddlDrawerNo = dBMaintenance.SelectDrawerItemlocationStatusDB(rack.MainNo);
                foreach (var item in maintenance.ddlDrawerNo)
                {
                    foreach (var item2 in ItemLocationList.Where(s => s.MainNo == rack.MainNo).OrderBy(i => i.DrawerNo))
                    {
                        if (item2.DrawerNo.Trim() == item.Value.Trim() || item.Value.Trim().Equals(item2.DrawerNo.Trim(), StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (result == "")
                            {
                                result = "Exist-" + item2.Rows;
                            }
                            else
                            {
                                result = result + ", " + item2.Rows;
                                char[] param = { ',' };
                                result = result.TrimStart(param).Trim();
                            }
                        }
                    }
                }
            }

            if (result != "")
            {
                return Json(result);
            }

            var z = dBMaintenance.InsertDrawerInformationDB(ItemLocationList);

            return Json(z);
        }

        [HttpPost]
        public JsonResult InsertBinInformation(List<MaintenanceModel> ItemLocationList)
        {
            var result = "";
            var valMain = "";
            var valRack = "";
            DBMaintenance dBMaintenance = new DBMaintenance();
            foreach (var rack in ItemLocationList.OrderBy(r => r.MainNo))
            {
                if (valMain == null || valMain != rack.MainNo)
                {
                    valMain = rack.MainNo;
                }
                else if (valMain == rack.MainNo)
                {
                    continue;
                }
                foreach (var drawer in ItemLocationList.Where(d => d.MainNo == rack.MainNo))
                {
                    if (valRack == null || valRack != drawer.DrawerNo)
                    {
                        valRack = drawer.DrawerNo;
                    }
                    else if (valRack == drawer.DrawerNo)
                    {
                        continue;
                    }
                    maintenance.ddlBin = dBMaintenance.SelectBinItemlocationStatusDB(drawer.DrawerNo, rack.MainNo);
                    foreach (var bin in maintenance.ddlBin)
                    {
                        foreach (var bin2 in ItemLocationList.Where(s => s.DrawerNo == drawer.DrawerNo).OrderBy(b => b.BinNo))
                        {
                            if (bin2.BinNo.Trim() == bin.Value.Trim() || bin.Value.Trim().Equals(bin2.BinNo.Trim(), StringComparison.CurrentCultureIgnoreCase))
                            {
                                if (result == "")
                                {
                                    result = "Exist-" + bin2.Rows;
                                }
                                else
                                {
                                    result = result + ", " + bin2.Rows;
                                    char[] param = { ',' };
                                    result = result.TrimStart(param).Trim();
                                }
                            }
                        }
                    }
                }
            }
            if (result != "")
            {
                return Json(result);
            }

            var i = dBMaintenance.InsertBinInformationDB(ItemLocationList);

            return Json(i);
        }

        [HttpPost]
        public JsonResult UpdateRackInformation(MaintenanceModel maintenance)
        {
            var result = "";
            var i = 0;
            DBMaintenance dBMaintenance = new DBMaintenance();
            List<MaintenanceModel> DBItemList = new List<MaintenanceModel>();
            DBItemList = dBMaintenance.PopulateAllItemLocationDB();

            foreach (var DBItem in DBItemList.Where(w => w.RackNo != maintenance.MainNo))
            {
                if (DBItem.RackNo.Trim() == maintenance.ChangeNo.Trim() || DBItem.RackNo.Trim().Equals(maintenance.ChangeNo.Trim(), StringComparison.CurrentCultureIgnoreCase))
                {
                    result = "Exist-" + maintenance.ChangeNo;
                }
            }

            if (result != "")
            {
                return Json(result);
            }

            i = dBMaintenance.UpdateLocation(maintenance);
            return Json(i, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateDrawerInformation(MaintenanceModel maintenance)
        {
            var result = "";
            var i = 0;
            DBMaintenance dBMaintenance = new DBMaintenance();
            List<MaintenanceModel> DBItemList = new List<MaintenanceModel>();
            DBItemList = dBMaintenance.PopulateAllItemLocationDB();

            foreach (var DBItem in DBItemList.Where(w => w.RackNo == maintenance.MainNo && w.DrawerNo.Trim().Equals(maintenance.ChangeNo.Trim(), StringComparison.CurrentCultureIgnoreCase)))
            {
                result = "Exist-" + maintenance.ChangeNo;
            }

            if (result != "")
            {
                return Json(result);
            }

            i = dBMaintenance.UpdateLocation(maintenance);
            return Json(i, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateBinInformation(MaintenanceModel maintenance)
        {
            var result = "";
            var i = 0;
            DBMaintenance dBMaintenance = new DBMaintenance();
            List<MaintenanceModel> DBItemList = new List<MaintenanceModel>();
            DBItemList = dBMaintenance.PopulateAllItemLocationDB();

            foreach (var dbItem in DBItemList.Where(w => w.RackNo == maintenance.MainNo && w.DrawerNo == maintenance.DrawerNo && w.BinNo.Trim().Equals(maintenance.ChangeNo.Trim(), StringComparison.CurrentCultureIgnoreCase)))
            {
                if (result == "")
                {
                    result = "Exist-" + maintenance.ChangeNo;
                }
            }

            if (result != "")
            {
                return Json(result);
            }

            i = dBMaintenance.UpdateLocation(maintenance);
            return Json(i, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateItemLocationStatus(MaintenanceModel maintenance)
        {
            var i = 0;
            DBMaintenance dBMaintenance = new DBMaintenance();
            if (maintenance.LocationStatus != "1")
            {
                var v = dBMaintenance.ItemLocationHasValueDB(maintenance);
                if (v > 0)
                {
                    return Json("Exist");
                }
            }
            i = dBMaintenance.UpdateLocationStatus(maintenance);
            return Json(i);
        }

        [HttpPost]
        public JsonResult RetrieveDrawerTableData(MaintenanceModel maintenance)
        {
            List<MaintenanceModel> DBItemList = new List<MaintenanceModel>();
            DBItemList = dBMaintenance.PopulateSelectedDrawerList(maintenance);

            var query = (from e in DBItemList
                         select new { e.LocationStatus, e.DrawerNo, e.DrawerStatus });

            return Json(DBItemList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RetrieveBinTableData(MaintenanceModel maintenance)
        {
            List<MaintenanceModel> DBItemList = new List<MaintenanceModel>();
            DBItemList = dBMaintenance.PopulateSelectedBinList(maintenance);

            var query = (from e in DBItemList
                         select new { e.LocationStatus, e.BinNo, e.BinStatus });

            return Json(DBItemList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult InsertItemMasterInformation(List<MaintenanceModel> ItemLocationList, MaintenanceModel maintenance)
        {
            var i = "";
            DBMaintenance dBMaintenance = new DBMaintenance();
            List<MaintenanceModel> ItemDescription = new List<MaintenanceModel>();
            ItemDescription = dBMaintenance.ItemDescriptionList();
            if (maintenance.ItemDescription == null)
            {
                maintenance.ItemDescription = "";
            }
            var x = dBMaintenance.CheckItemCodeIfExist(maintenance);
            if (x != 0)
            {
                return Json("StockInvalid");
            }
            foreach (var item in ItemDescription.Where(w => w.Specification.Trim() == maintenance.Specification.Trim() && w.ItemDescription.Trim() == maintenance.ItemDescription))
            {
                i = item.StockNo;
            }
            if (i != "")
            {
                return Json("Duplicated");
            }

            var information = dBMaintenance.InsertItemMasterInformationDB(maintenance, Session["UserID"].ToString());

            if (information != 0)
            {
                var Grid = dBMaintenance.InsertItemMasterLocationDB(ItemLocationList, maintenance);

                if (Grid == 0)
                {
                    if (maintenance.Group != "2")
                    {
                        return Json(Grid);
                    }
                }
            }

            return Json(information);
        }

        [HttpPost]
        public JsonResult UpdateItemMasterInformation(List<MaintenanceModel> ItemLocationList, MaintenanceModel maintenance)
        {
            var i = "";
            DBMaintenance dBMaintenance = new DBMaintenance();
            List<MaintenanceModel> ItemDescription = new List<MaintenanceModel>();
            ItemDescription = dBMaintenance.ItemDescriptionList();

            if (maintenance.OldStockNo != maintenance.StockNo)
            {
                var x = dBMaintenance.CheckItemCodeIfExist(maintenance);

                if (x != 0)
                {
                    return Json("StockInvalid");
                }
                else
                {
                    foreach (var item in ItemDescription.Where(w => w.Specification.Trim() == maintenance.Specification.Trim() && w.ItemDescription.Trim() == maintenance.ItemDescription && w.StockNo.Trim() != maintenance.OldStockNo.Trim()))
                    {
                        i = item.StockNo;
                    }
                }
            }
            else
            {
                foreach (var item in ItemDescription.Where(w => w.Specification.Trim() == maintenance.Specification.Trim() && w.ItemDescription.Trim() == maintenance.ItemDescription && w.StockNo.Trim() != maintenance.StockNo.Trim()))
                {
                    i = item.StockNo;
                }
            }

            if (i != "")
            {
                return Json("Duplicated");
            }

            if (maintenance.Status != "1")
            {
                var a = dBMaintenance.ItemHasStockDB(maintenance);
                if (a > 0)
                {
                    return Json("Exist");
                }
            }

            var information = dBMaintenance.UpdateItemMasterInformationDB(maintenance, Session["UserID"].ToString());

            if (information != 0)
            {
                dBMaintenance.DeleteItemlocation(maintenance.OldStockNo);

                var Grid = dBMaintenance.InsertItemMasterLocationDB(ItemLocationList, maintenance);

                if (Grid == 0)
                {
                    return Json(Grid);
                }
            }

            return Json(information);
        }

        [HttpPost]
        public JsonResult UpdateItemMasterStatus(MaintenanceModel maintenance)
        {
            DBMaintenance dBMaintenance = new DBMaintenance();
            if (maintenance.Status != "1")
            {
                var a = dBMaintenance.ItemHasStockDB(maintenance);
                if (a > 0)
                {
                    return Json("Exist");
                }
            }

            var data = dBMaintenance.UpdateItemMasterStatusDB(maintenance);

            return Json(data);
        }

        [HttpGet]
        public JsonResult RetreiveUserGroup()
        {

            DBMaintenance dBMaintenance = new DBMaintenance();

            var Category = dBMaintenance.SelectAllUserGroupDB();

            return Json(Data = Category, JsonRequestBehavior = JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult RetrieveLocationStatus(MaintenanceModel maintenance)
        {
            var data = dBMaintenance.RetrieveLocationStatusDB(maintenance);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult RetreiveMainItemLocation()
        {
            DBMaintenance dBMaintenance = new DBMaintenance();

            var LocationCategory = dBMaintenance.SelectAllMainItemlocationDB();


            return Json(Data = LocationCategory, JsonRequestBehavior = JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult RetreiveBinItemLocation(string DrawerNo, string MainNo, MaintenanceModel maintenance)
        {
            DBMaintenance dBMaintenance = new DBMaintenance();
            if (maintenance.LocationStatus != null)
            {
                var Status = dBMaintenance.RetrieveLocationStatusDB(maintenance);

                var LocationCategory = dBMaintenance.SelectBinItemlocationStatusDB(DrawerNo, MainNo);

                return Json(new { Data = LocationCategory, Status }, JsonRequestBehavior = JsonRequestBehavior.AllowGet);
            }
            else
            {
                var LocationCategory = dBMaintenance.SelectBinItemlocationDB(DrawerNo, MainNo);

                return Json(Data = LocationCategory, JsonRequestBehavior = JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult RetreiveDrawerItemLocation(string MainNo)
        {
            DBMaintenance dBMaintenance = new DBMaintenance();

            var LocationCategory = dBMaintenance.SelectDrawerItemlocationDB(MainNo);


            return Json(Data = LocationCategory, JsonRequestBehavior = JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult RetreiveDrawerItemLocationStatus(string MainNo)
        {
            DBMaintenance dBMaintenance = new DBMaintenance();

            var LocationCategory = dBMaintenance.SelectDrawerItemlocationStatusDB(MainNo);


            return Json(Data = LocationCategory, JsonRequestBehavior = JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult RetreiveNoDrawerItemLocation(string MainNo, MaintenanceModel maintenance)
        {
            DBMaintenance dBMaintenance = new DBMaintenance();
            if (maintenance.LocationStatus != null)
            {
                var Status = dBMaintenance.RetrieveLocationStatusDB(maintenance);

                var LocationCategory = dBMaintenance.SelectNoDrawerItemlocationDB(MainNo);

                return Json(new { Data = LocationCategory, Status }, JsonRequestBehavior = JsonRequestBehavior.AllowGet);
            }
            else
            {
                var LocationCategory = dBMaintenance.SelectNoDrawerItemlocationDB(MainNo);


                return Json(Data = LocationCategory, JsonRequestBehavior = JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult RetreiveDesignation(string Group)
        {
            DBMaintenance dBMaintenance = new DBMaintenance();

            var LocationCategory = dBMaintenance.SelectUserDesignationDB(Group);


            return Json(Data = LocationCategory, JsonRequestBehavior = JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RetreivePullOut(MaintenanceModel maintenance)
        {
            DBMaintenance dBMaintenance = new DBMaintenance();

            var PullOutList = dBMaintenance.SelectPullOutDB(maintenance);


            return Json(PullOutList, JsonRequestBehavior = JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UserManagement(MaintenanceModel maintenance)
        {
            DBMaintenance dBMaintenance = new DBMaintenance();

            if (maintenance.UserID != null)
            {
                //Update User info
                var data = dBMaintenance.InventoryUpdateUserDB(maintenance);

                return Json(data);
            }
            else
            {
                var data2 = dBMaintenance.CheckUserNameExistsDB(maintenance.Username.Trim());
                if (data2 != "")
                {
                    return Json(data2);
                }
                //Insert User info
                var data = dBMaintenance.InventoryInsertUserDB(maintenance);

                return Json(data);
            }
        }

        [HttpPost]
        public JsonResult ResetPassword(MaintenanceModel maintenance)
        {
            string UserID = Session["UserID"].ToString();
            DBMaintenance dBMaintenance = new DBMaintenance();

            string temp = "";
            string IDString = "";
            string RandomChars = "";
            RandomChars = "1,2,3,4,5,6,7,8,9,0";
            string VerificationCode = "";
            Char[] seprateChar = { ',' };
            string[] arrRandomChar = RandomChars.Split(seprateChar);
            Random rand = new Random();
            for (int i = 0; i < 4; i++)
            {
                temp = arrRandomChar[rand.Next(0, arrRandomChar.Length)];
                IDString += temp;
                VerificationCode = IDString;
            }
            maintenance.Code = VerificationCode;

            var result = dBMaintenance.InventoryUserResetPasswordDB(maintenance, UserID);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateUserStatus(MaintenanceModel maintenance)
        {
            DBMaintenance dBMaintenance = new DBMaintenance();
            var data = dBMaintenance.InventoryUpdateUserStatusDB(maintenance);

            return Json(data);
        }

        public RoleAccessModel GetRoleAccess()
        {
            DBMaintenance dBMaintenance = new DBMaintenance();
            RoleAccessModel RoleModel = new RoleAccessModel();
            RoleModel = dBMaintenance.RoleAccess(Session["UserRoleID"].ToString());

            return RoleModel;
        }

        public RoleManagementModel GetRoleData(string id)
        {
            DBMaintenance dBMaintenance = new DBMaintenance();
            RoleManagementModel RoleDataModel = new RoleManagementModel();
            if (id != "table" && id != "back" && id != "create" && id != null)
            {
                RoleDataModel = dBMaintenance.RoleDisplayInfoDatabase(id);
            }
            RoleDataModel.PopulateRoleList = dBMaintenance.PopulateRoleListDatabase();

            return RoleDataModel;
        }

        public ActionResult RoleManagement(string id, string RoleID)
        {
            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-5));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            if (Session["UserID"] != null)
            {
                if (id == "table")
                {
                    ViewBag.RoleAccessContent = "table";
                    models.RoleAccess = GetRoleAccess();
                    models.RoleManagement = GetRoleData(id);
                }
                else if (id == "create")
                {
                    ViewBag.RoleAccessContent = "create";
                    models.RoleAccess = GetRoleAccess();
                    models.RoleManagement = GetRoleData(id);
                    GetTreeData(id);
                }
                else
                {
                    ViewBag.RoleAccessContent = "edit";
                    models.RoleAccess = GetRoleAccess();
                    models.RoleManagement = GetRoleData(RoleID);
                    GetTreeData(RoleID);
                }

                var access = false;
                foreach (var item in models.RoleAccess.Sub)
                {
                    if (item == "75")
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
            else
            {
                return RedirectToAction("LoginAuthentication", "Login");
            }
        }

        //public ActionResult EditRoleManagement(string id, string RoleID)
        //{
        //    Session["RoleID"] = RoleID;
        //    ViewBag.RoleAccessContent = "edit";
        //    AllModels Model = new AllModels();
        //    Model.RoleAccess = GetRoleAccess();
        //    Model.RoleManagement = GetRoleData(RoleID);
        //    GetTreeData(RoleID);

        //    var access = false;
        //    foreach (var item in Model.RoleAccess.Sub)
        //    {
        //        if (item == "75")
        //        {
        //            access = true;
        //        }
        //    }
        //    if (access == true)
        //    {
        //        return View("~/Views/Maintenance/RoleManagement.cshtml", Model);
        //    }
        //    else
        //    {
        //        return Redirect("/Home/Dashboard");
        //    }
        //}

        public List<TreeViewModel> GetTreeData(string id)
        {
            List<TreeViewModel> Nodes = new List<TreeViewModel>();
            if (id == "create")
            {
                DBMaintenance dBMaintenance = new DBMaintenance();
                MainAccess AllMainData = new MainAccess();
                SubAccess AllSubData = new SubAccess();
                AllMainData.AllMainList = dBMaintenance.PopulateAllMainDatabase();
                AllSubData.AllSubList = dBMaintenance.PopulateAllSubDatabase();

                foreach (MainAccess allMain in AllMainData.AllMainList)
                {
                    Nodes.Add(new TreeViewModel { id = allMain.AllMainID.ToString(), parent = "#", text = allMain.AllMainName });
                } foreach (SubAccess allSub in AllSubData.AllSubList)
                {
                    Nodes.Add(new TreeViewModel { id = allSub.AllSubMainID.ToString() + "-" + allSub.AllSubID.ToString(), parent = allSub.AllSubMainID.ToString(), text = allSub.AllSubName });
                }

                //Serialize to JSON string.
                ViewBag.Json = (new JavaScriptSerializer()).Serialize(Nodes);
            }
            else
            {
                DBMaintenance dBMaintenance = new DBMaintenance();
                MainAccess MainData = new MainAccess();
                SubAccess SubData = new SubAccess();
                MainAccess AllMainData = new MainAccess();
                SubAccess AllSubData = new SubAccess();
                AllMainData.AllMainList = dBMaintenance.PopulateAllMainDatabase();
                AllSubData.AllSubList = dBMaintenance.PopulateAllSubDatabase();
                MainData.MainList = dBMaintenance.PopulateMainDatabase(id);
                SubData.SubList = dBMaintenance.PopulateSubDatabase(id);
                int count = 0;

                foreach (MainAccess allMain in AllMainData.AllMainList)
                {
                    count = 0;
                    foreach (MainAccess mainItem in MainData.MainList)
                    {
                        if (mainItem.MainID == "1" || mainItem.MainID == "4")
                        {
                            if (mainItem.MainID == allMain.AllMainID.ToString())
                            {
                                Nodes.Add(new TreeViewModel { id = allMain.AllMainID.ToString(), parent = "#", text = allMain.AllMainName, state = new TreeViewAttribute { selected = true } });
                                count++;
                                break;
                            }
                        }
                    }
                    if (count == 0)
                    {
                        Nodes.Add(new TreeViewModel { id = allMain.AllMainID.ToString(), parent = "#", text = allMain.AllMainName, state = new TreeViewAttribute { selected = false } });
                    }
                }
                foreach (SubAccess allSub in AllSubData.AllSubList)
                {
                    count = 0;
                    foreach (SubAccess subItem in SubData.SubList)
                    {
                        if (subItem.SubID == allSub.AllSubID.ToString())
                        {
                            Nodes.Add(new TreeViewModel { id = allSub.AllSubMainID.ToString() + "-" + allSub.AllSubID.ToString(), parent = allSub.AllSubMainID.ToString(), text = allSub.AllSubName, state = new TreeViewAttribute { selected = true } });
                            count++;
                            break;
                        }
                    }
                    if (count == 0)
                    {
                        Nodes.Add(new TreeViewModel { id = allSub.AllSubMainID.ToString() + "-" + allSub.AllSubID.ToString(), parent = allSub.AllSubMainID.ToString(), text = allSub.AllSubName, state = new TreeViewAttribute { selected = false } });
                    }
                }

                ViewBag.Json = (new JavaScriptSerializer()).Serialize(Nodes);

            }
            return Nodes;
        }

        [HttpPost]
        public JsonResult Insert(RoleManagementModel Role, string selectedItems)
        {
            int count = 0;
            var result = "";
            DBMaintenance dBMaintenance = new DBMaintenance();
            AllModels Models = new AllModels();
            var RoleList = dBMaintenance.PopulateRoleListDatabase();

            foreach (var item in RoleList)
            {
                if (item.RoleName.Equals(Role.RoleName, StringComparison.CurrentCultureIgnoreCase))
                {
                    result = "Exist-" + item.RoleName;
                    return Json(result);
                }
            }


            TreeViewModel TreeView = new TreeViewModel();
            List<TreeViewModel> Module = new List<TreeViewModel>();
            List<TreeViewModel> items = (new JavaScriptSerializer()).Deserialize<List<TreeViewModel>>(selectedItems);
            foreach (var selected in items)
            {
                if (selected.parent == "#")
                {
                    count = 0;
                    foreach (var item in items)
                    {
                        if (selected.id == item.parent)
                        {
                            count++;
                        }
                    }
                    if (count == 0)
                    {
                        TreeView = new TreeViewModel
                        {
                            selectedID = selected.id,
                        };
                        Module.Add(TreeView);
                    }
                }
                else
                {
                    TreeView = new TreeViewModel
                    {
                        selectedID = selected.id,
                    };
                    Module.Add(TreeView);
                }

                //if (selected.id != "3" && selected.id != "4" && selected.id != "5" &&
                //    selected.id != "9" && selected.id != "10" && selected.id != "11")
                //{
                //    TreeView = new TreeViewModel
                //    {
                //        selectedID = selected.id,
                //    };
                //    Module.Add(TreeView);
                //}
            }
            TreeView.Module = Module;
            Models.TreeView = TreeView;
            Models.RoleManagement = Role;
            //dBMaintenance.RoleManagementInsertRoleModule(Models, Session["UserID"].ToString());
            var i = dBMaintenance.RoleManagementInsertRoleModule(Models, Session["UserID"].ToString());
            return Json(i);
            //return RedirectToAction("RoleManagement");
        }

        [HttpPost]
        public JsonResult Update(RoleManagementModel Role, string selectedItems)
        {
            int count = 0;
            DBMaintenance dBMaintenance = new DBMaintenance();
            AllModels Models = new AllModels();
            var RoleList = dBMaintenance.PopulateRoleListDatabase();

            foreach (var item in RoleList.Where(w => w.AccessUserID != Role.RoleID))
            {
                if (item.RoleName.Equals(Role.RoleName,StringComparison.CurrentCultureIgnoreCase))
                {
                    var result = "Exist-" + item.RoleName;
                    return Json(result);
                }
            }

            TreeViewModel TreeView = new TreeViewModel();
            List<TreeViewModel> Module = new List<TreeViewModel>();
            List<TreeViewModel> items = (new JavaScriptSerializer()).Deserialize<List<TreeViewModel>>(selectedItems);
            foreach (var selected in items)
            {
                if (selected.parent == "#")
                {
                    count = 0;
                    foreach (var item in items)
                    {
                        if (selected.id == item.parent)
                        {
                            count++;
                        }
                    }
                    if (count == 0)
                    {
                        TreeView = new TreeViewModel
                        {
                            selectedID = selected.id,
                        };
                        Module.Add(TreeView);
                    }
                }
                else
                {
                    TreeView = new TreeViewModel
                    {
                        selectedID = selected.id,
                    };
                    Module.Add(TreeView);
                }


                //
                //if (selected.id != "3" && selected.id != "4" && selected.id != "5" &&
                //    selected.id != "9" && selected.id != "10" && selected.id != "11")
                //{
                //    TreeView = new TreeViewModel
                //    {
                //        selectedID = selected.id,
                //    };
                //    Module.Add(TreeView);
                //}
            }
            TreeView.Module = Module;
            Models.TreeView = TreeView;
            Models.RoleManagement = Role;
            dBMaintenance.RoleManagementDeleteRoleModule(Models);
            var i = dBMaintenance.RoleManagementUpdateRoleModule(Models, Session["UserID"].ToString());
            //return RedirectToAction("RoleManagement");
            return Json(i);
        }

        [HttpPost]
        public JsonResult RetreiveDetails(string StockNo)
        {
            var Details = dBMaintenance.SelectDetailsDB(StockNo);
            return Json(Details);
        }

        [HttpPost]
        public JsonResult UpdateStockOnHand(List<CentralModel> ManualAdjustList, List<MaintenanceModel> TransactionDetailList)
        {
            var DataGrid = dBMaintenance.InsertManualAdjustDB(ManualAdjustList, Session["UserID"].ToString());
            if (DataGrid != 0)
            {
                var Transactions = dBMaintenance.InsertTransactionDetails(TransactionDetailList, Session["UserID"].ToString());
            }
            return Json(DataGrid);
        }

        [HttpPost]
        public JsonResult InsertRequestPR(List<CentralModel> information, CentralModel central)
        {
            var Data = dBCentral.InsertRequestPRDB(central, Session["UserID"].ToString());

            if (Data != 0)
            {
                var DataGrid = dBCentral.InsertItemListRequestPRDB(information, central);
            }

            return Json(Data);
        }
    }
}