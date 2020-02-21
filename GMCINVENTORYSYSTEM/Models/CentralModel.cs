using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GMCINVENTORYSYSTEM.Models
{
	public class CentralModel
	{
		public string NotificationReferenceNo { get; set; }
		public string NotificationDesignation { get; set; }
		public string NotificationEntryDate { get; set; }
		public string NotificationDate { get; set; }
		public string NotificationStockNo { get; set; }
		public string NotificationItemName { get; set; }
		public string NotificationTypeOfItem { get; set; }
		public string ReferenceNo { get; set; }
		public string StockNo { get; set; }
		public string PRNo { get; set; }
		public string Purpose { get; set; }
		public string Department { get; set; }
		public string ItemName { get; set; }
		public string Description { get; set; }
        public string qty { get; set; }
        public string Excess { get; set; }
		public string DateRequested { get; set; }
		public string Responsible { get; set; }
		public string RequestStatus { get; set; }
		public string ItemReceived { get; set; }
        public string TotalReceived { get; set; }
        public string ItemReleased { get; set; }
        public string TotalReleased { get; set; }

		public string SupplierName { get; set; }
		public string  DateReceived {get; set;}
		public string DateReleased { get; set; }
		public string DisplayPRNo { get; set; }
		public string DisplayReferenceNo { get; set; }
		public string collected { get; set; }
		public string CompanyID { get; set; }
		public string GroupName { get; set; }
		public string Designation { get; set; }
		public string RejectRemarks { get; set; }
		public string TypeRequest { get; set; }
		public string ApproverID { get; set; }
		public string Delivered { get; set; }
		public string DateApprovedPR { get; set; }
		public string StockOnHand { get; set; }
		public int ItemOutStock { get; set; }
		public int itemCount { get; set; }
		public string Number { get; set; }
		public string Releasedby { get; set; }
		public string DateApproved { get; set; }
		public string DateRejected { get; set; }
		public string Rejectedby { get; set; }
		public string Requester { get; set; }
        public string UserRole { get; set; }
		
		public string AdjustDate { get; set; }
        public string OldStockOnHand { get; set; }
        public string AdjustQuantity { get; set; }

        public string Reason { get; set; }
        public string NotifRequestStatus { get; set; }
        public string NotifItemStatus { get; set; }

        public string From { get; set; }
        public string To { get; set; }

        public string TypeName { get; set; }
        public string row { get; set; }
		
		public List<SelectListItem> ddItemlist { get; set; }
		public List<CentralModel> dtRequestMonitoring { get; set; }
		public List<CentralModel> dtRoutingApproval { get; set; }
		public List<CentralModel> dtCommonRequest { get; set; }
		public List<CentralModel> dtReleasing { get; set; }
		public List<CentralModel> dtReceiving { get; set; }
		public List<CentralModel> dtReleasedLogs { get; set; }
		public List<CentralModel> dtReceivedLogs { get; set; }
		public List<SelectListItem> ddlDesignation { get; set; }
		public List<SelectListItem> ddlTypeList { get; set; }

        public List<CentralModel> dtPRRequest { get; set; }
        public List<CentralModel> dtManualAdjustment { get; set; }
        public List<CentralModel> dtItemListRequestForPR { get; set; }

        //New
        public List<CentralModel> dtApprovedItem { get; set; }
        public List<CentralModel> dtRejectedItem { get; set; }
        public List<CentralModel> dtReleasedItem { get; set; }
        public List<CentralModel> dtRequestedItem { get; set; }
        public List<CentralModel> dtPRReceived { get; set; }
	}
}