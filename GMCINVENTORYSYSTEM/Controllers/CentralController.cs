using GMCINVENTORYSYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GMCINVENTORYSYSTEM.Controllers
{

    public class CentralController : Controller
    {
		public JsonRequestBehavior JsonRequestBehavior { get; private set; }
		public List<SelectListItem> Data { get; private set; }
		public List<CentralModel> DataList { get; private set; }



		DBCentral dBCentral = new DBCentral();
		CentralModel centralModel = new CentralModel();

		// GET: Central
		public ActionResult Index()
        {
            return View();
        }

		public ActionResult RequestPR()
		{
			return View();
		}

		public ActionResult Releasing()
		{
			return View();
		}

		public ActionResult Receiving()
		{

			//centralModel.dtReceiving = dBCentral.PopulateRequestPRDB();
			return View(centralModel);
		}

		public ActionResult CRequestMonitoring()
		{
			return View();
		}

		[HttpPost]
		public JsonResult InsertRequestPR(List<CentralModel> ItemRequestList, CentralModel central)
		{


			var Data = dBCentral.InsertRequestPRDB(central,Session["UserID"].ToString());

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
		public JsonResult RetrieveItemRequestPR(string PRNo, string ReferenceNo)
		{
			var GroupID = Session["GroupID"].ToString();
			 var DesignationID = Session["DesignationID"].ToString();
			var Category = dBCentral.PopulateItemRequestForReceivingDB(PRNo, ReferenceNo , GroupID , DesignationID);

			return Json(DataList = Category, JsonRequestBehavior = JsonRequestBehavior.AllowGet);

		}

		[HttpPost]
		public JsonResult UpdateItemRequestForReceiving(List<CentralModel> ItemRequestList, CentralModel central)
		{
			int i = 0;
			int totalCollected = 0;
			foreach (CentralModel centrallist in ItemRequestList)
			{
				if (centrallist.ItemReceived != null)
				{
					totalCollected = int.Parse(centrallist.ItemReceived ?? "0") + int.Parse(centrallist.collected);

					if (totalCollected > int.Parse(centrallist.qty))
					{
						return Json("Qty_Limit");
					}
					else
					{
						if (centrallist.ItemReceived == "0")
						{
							return Json("NotAllowed");
						}
					}

					i++;
				}
			}

			if (i == 0)
			{
				return Json(0);
			}

				var Data2 = dBCentral.UpdateItemRequestReceivedDB(ItemRequestList, central, Session["UserID"].ToString());

				if (Data2 == 0)
				{
					return Json("system_error");

				}
				else
				{
					return Json(Data2);
				}
			


		}


	}

}