using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GMCINVENTORYSYSTEM.Models
{
    public class AllModels
    {
        public MaintenanceModel Maintenance { get; set; }
        public CentralModel Central { get; set; }
        public SatelliteModel Satellite { get; set; }


        public RoleManagementModel RoleManagement { get; set; }
        public RoleAccessModel RoleAccess { get; set; }
        public TreeViewModel TreeView { get; set; }

        //public TableSearchBox Search { get; set; }
    }

    public class MaintenanceModel
    {

        public string DatabaseConnection { get; set; }
        public string DatabaseUser { get; set; }
        public string DatabasePassword { get; set; }
        public string AdminUser { get; set; }
        public string AdminPassword { get; set; }

        public string smtpID { get; set; }
        public string smtpStatus { get; set; }
        public string smtpPort { get; set; }
        public string smtpPassword { get; set; }
        public string smtpConfirmPassword { get; set; }
        public string smtpServer { get; set; }
        public string smtpEmail { get; set; }
        public string Rows { get; set; }

        public string UserID { get; set; }
        public string CompanyID { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
        public string RoleID { get; set; }
        public string RoleName { get; set; }

        public string GroupID { get; set; }
        public string DepartmentID { get; set; }
        public string Department { get; set; }
        public string DesignationID { get; set; }
        public string Status { get; set; }
        public int ResetPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
        public string OldPassword { get; set; }

        public string Group { get; set; }
        public string Designation { get; set; }



        public string StockNo { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string Specification { get; set; }
        public string Details { get; set; }
        public string Unit { get; set; }
        public string Price { get; set; }
        public string StockOnHand { get; set; }
        public string SafetyStock { get; set; }
        public string MainNo { get; set; }
        public string RackNo { get; set; }
        public string RackDescription { get; set; }
        public string BinNo { get; set; }
        public string BinDescription { get; set; }
        public string DrawerNo { get; set; }
        public string DrawerDescription { get; set; }
        public string CabinetNo { get; set; }
        public string CabinetDescription { get; set; }
        public string DrawerList { get; set; }
        public string CriticalOrder { get; set; }

        public string CheckIfAlreadyTransac { get; set; }

        public string ManualCount { get; set; }
        public string HiddenBin { get; set; }
        public string OldStockNo { get; set; }
        public string TypeID { get; set; }
        public string HiddenDrawer { get; set; }

        public bool check { get; set; }
        public string Reason { get; set; }

        public string ReferenceNo { get; set; }
        public string PRNo { get; set; }
        public string DateRequested { get; set; }
        public string RequestStatus { get; set; }
        public string DateApprovedPR { get; set; }

        public string PRStatus { get; set; }
        public string LocationStatus { get; set; }
        public string RackStatus { get; set; }
        public string DrawerStatus { get; set; }
        public string BinStatus { get; set; }
        public string ChangeNo { get; set; }

        public string Pullout { get; set; }
        public string HiddenPullout { get; set; }
        public string StatusName { get; set; }

        public string Code { get; set; }

        public List<SelectListItem> ddlUserRole { get; set; }
        public List<SelectListItem> ddlDeparment { get; set; }
        public List<SelectListItem> ddlGroup { get; set; }
        public List<SelectListItem> ddlDesignation { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public List<SelectListItem> LocationStatusList { get; set; }

        public List<SelectListItem> ddlMain { get; set; }
        public List<SelectListItem> ddlType { get; set; }
        public List<SelectListItem> ddlBin { get; set; }
        public List<SelectListItem> ddlDrawerNo { get; set; }
        public List<SelectListItem> ddlAllItemStockNo { get; set; }

        public List<MaintenanceModel> dtItemMonitoring { get; set; }
        public List<MaintenanceModel> dtUserInformation { get; set; }
        public List<MaintenanceModel> dtIteminformation { get; set; }
        public List<MaintenanceModel> PopulateItemLocation { get; set; }
        public List<MaintenanceModel> dtLocationMonitoring { get; set; }
        public List<MaintenanceModel> dtRack { get; set; }

        //Transaction Code
        public string TransactionType { get; set; }
        public string TransactionTypes { get; set; }
        public string IMReferenceNo { get; set; }
        public string Item { get; set; }
        public string Quantity { get; set; }
        public string TransStockOnHand { get; set; }
        public string AdjustStockOnHand { get; set; }
        public string RemainingStock { get; set; }
        public string TransactionDate { get; set; }
        public string Responsible { get; set; }

        public List<MaintenanceModel> PopulateTransactions { get; set; }
        public List<SelectListItem> TransactionList { get; set; }

        public string ProcessType { get; set; }
        public string ItemType { get; set; }
        public string UpdateReferenceNo { get; set; }
        public string UpdatePRStatus { get; set; }
        public string UpdateItemStatus { get; set; }
        public string UpdateRequestStatus { get; set; }
        public string UpdateItemCode { get; set; }
        public string UpdateQty { get; set; }
    }

    public class table
{
    public string Action { get; set; }
    public string Status { get; set; }
    public string Number { get; set; }

    public table() { }

    public table(string Action, string Number, string Status)
    {
        this.Action = Action;
        this.Number = Number;
        this.Status = Status;
    }
}

    public class RoleManagementModel
    {
        public int AccessUserID { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; }

        public List<RoleManagementModel> PopulateRoleList { get; set; }
    }

    public class RoleAccessModel
    {
        public string[] Main { get; set; }
        public string[] Sub { get; set; }
        public int Access { get; set; }

        public List<RoleAccessModel> PopulateRoleAccess { get; set; }
    }

    public class MainAccess
    {
        public string MainID { get; set; }
        public string MainName { get; set; }

        public string AllMainID { get; set; }
        public string AllMainName { get; set; }

        public List<MainAccess> MainList { get; set; }
        public List<MainAccess> AllMainList { get; set; }
    }

    public class SubAccess
    {
        public string SubID { get; set; }
        public string SubMainID { get; set; }
        public string SubName { get; set; }

        public string AllSubID { get; set; }
        public string AllSubMainID { get; set; }
        public string AllSubName { get; set; }

        public List<SubAccess> SubList { get; set; }
        public List<SubAccess> AllSubList { get; set; }
    }

    public class TreeViewModel
    {
        public string id { get; set; }
        public string parent { get; set; }
        public string text { get; set; }
        public string selectedID { get; set; }

        public List<TreeViewModel> Module { get; set; }
        public TreeViewAttribute state { get; set; }
    }

    public class TreeViewAttribute
    {
        public bool selected { get; set; }
    }

}