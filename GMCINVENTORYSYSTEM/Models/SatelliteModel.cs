using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GMCINVENTORYSYSTEM.Models
{
	public class SatelliteModel
	{
		public string ReferenceNo { get; set; }
		public string Department { get; set; }
		public string Purpose { get; set; }
		public string Responsible { get; set; }
		public string StockNo { get; set; }
		public string ItemName { get; set; }
		public string Description { get; set; }
		public string Qty { get; set; }
		public string Type { get; set; }
		public string TypeName { get; set; }
		public string ApproverID { get; set; }
		public string CompanyID { get; set; }
		public string GroupName { get; set; }
		public string Designation { get; set; }
		public string RequestStatus { get; set; }
		public string DateRequested { get; set; }
		public string DateReleased { get; set; }
		public string Delivered { get; set; }
		public string ItemReleased { get; set; }
		public int ItemOutStock { get; set; }
		public int itemCount { get; set; }
		public string StockOnHand { get; set; }
		public string RejectRemarks { get; set; }
		public string TypeRequest { get; set; }
		public string Requester { get; set; }
		



		public List<SatelliteModel> dtUniqueRequest { get; set; }
		public List<SatelliteModel> dtRoutingUniqueApproval { get; set; }
		public List<SatelliteModel> dtUniqueReleasing { get; set; }
		public List<SelectListItem> ddItemList { get; set; }
		public List<SelectListItem> ddlApprover { get; set; }
		public List<SatelliteModel> dtRequestlist { get; set; }
		public List<SatelliteModel> dtMonitoringRequest { get; set; }
		public List<SatelliteModel> dtStockoutLogs { get; set; }
	}

}