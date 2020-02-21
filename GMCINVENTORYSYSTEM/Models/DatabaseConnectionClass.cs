using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using GMCINVENTORYSYSTEM.Models;

namespace GMCINVENTORYSYSTEM.Models
{
	public class DatabaseConnectionClass
	{




	

		public string var1 = HttpContext.Current.Server.MapPath("~/DatabaseConnection/ConnectionString.txt");
		public string var2 = HttpContext.Current.Server.MapPath("~/DatabaseConnection/Username.txt");
		public string var3 = HttpContext.Current.Server.MapPath("~/DatabaseConnection/Password.txt");



	}
}