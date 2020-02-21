using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using GMCINVENTORYSYSTEM.Models;


namespace GMCINVENTORYSYSTEM
{
    public class MvcApplication : System.Web.HttpApplication
    {
        //string con;
        //string con2;
        //string con3;


        //string DBConnection()
        //{
        //	DatabaseConnectionClass databaseConnectionClass = new DatabaseConnectionClass();
        //	string Con = databaseConnectionClass.var1;
        //	string User = databaseConnectionClass.var2;
        //	string Pass = databaseConnectionClass.var3;
        //	string Connectionstring = File.ReadAllText(Con);
        //	string ConnectionUserName = File.ReadAllText(User);
        //	string ConnectionPasswrod = File.ReadAllText(Pass);

        //	string Con1 = Connectionstring.TrimEnd();
        //	string User1 = EncryptionAndDecryption.Decrypt(ConnectionUserName);
        //	string Pass1 = EncryptionAndDecryption.Decrypt(ConnectionPasswrod);

        //	con = "Data Source=" + Con1.ToString() + ";Initial Catalog=DB_CORE;Network Library=DBMSSOCN;User ID=" + User1.ToString() + "; Password=" + Pass1.ToString() + "";
        //          con2 = "Data Source=" + Con1.ToString() + ";Initial Catalog=DB_CENTRAL;Network Library=DBMSSOCN;User ID=" + User1.ToString() + "; Password=" + Pass1.ToString() + "";
        //          con3 = "Data Source=" + Con1.ToString() + ";Initial Catalog=DB_INVENTORY;Network Library=DBMSSOCN;User ID=" + User1.ToString() + "; Password=" + Pass1.ToString() + "";

        //	return con;
        //}

        protected void Application_Start()
        {

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            MvcHandler.DisableMvcResponseHeader = true;



        }
        protected void Application_End()
        {

        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
        }

        protected void Application_EndRequest()
        {
            if (Context.Items["AjaxPermissionDenied"] is bool)
            {
                Context.Response.StatusCode = 401;
                Context.Response.End();
            }
        }

        private void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex is HttpAntiForgeryException)
            {
                Response.Clear();
                Server.ClearError(); //make sure you log the exception first
                Response.Redirect("/Login/LoginAuthentication", true);
            }
            //if (ex is ActionResult)
            //{
            //    Response.Clear();
            //    Server.ClearError(); //make sure you log the exception first
            //    Response.Redirect("/Home/Index", true);
            //}
        }
    }
}
