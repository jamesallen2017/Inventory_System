using GMCINVENTORYSYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GMCINVENTORYSYSTEM.Controllers
{
    public class SatelliteController : Controller
    {
		DBSatellite dBSatellite = new DBSatellite();
		DBCentral dBCentral = new DBCentral();
		SatelliteModel satellite = new SatelliteModel();

		// GET: Satellite
		public ActionResult Index()
        {
            return View();
        }

		public ActionResult SReleasing()
		{
			ViewBag.Message = "Your contact page.";
			return View();
		}
		public ActionResult SReleasingLogs()
		{
			ViewBag.Message = "Your contact page.";
			return View();
		}
	

		public ActionResult SRequestMonitoring()
		{
			ViewBag.Message = "Your contact page.";
			return View();
		}



	}
}