using GMCINVENTORYSYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace GMCINVENTORYSYSTEM.Controllers
{
    public class LoginController : Controller
    {
        string con;
        string con2;
        string con3;

        public string DBConnection()
        {
            DatabaseConnectionClass databaseConnectionClass = new DatabaseConnectionClass();
            string Con = databaseConnectionClass.var1;
            string User = databaseConnectionClass.var2;
            string Pass = databaseConnectionClass.var3;
            string Connectionstring = System.IO.File.ReadAllText(Con);
            string ConnectionUserName = System.IO.File.ReadAllText(User);
            string ConnectionPasswrod = System.IO.File.ReadAllText(Pass);

            string Con1 = Connectionstring.TrimEnd();
            string User1 = EncryptionAndDecryption.Decrypt(ConnectionUserName);
            string Pass1 = EncryptionAndDecryption.Decrypt(ConnectionPasswrod);

            con = "Data Source=" + Con1.ToString() + ";Initial Catalog=DB_CORE;Network Library=DBMSSOCN;User ID=" + User1.ToString() + "; Password=" + Pass1.ToString() + "";
            con2 = "Data Source=" + Con1.ToString() + ";Initial Catalog=DB_CENTRAL;Network Library=DBMSSOCN;User ID=" + User1.ToString() + "; Password=" + Pass1.ToString() + "";
            con3 = "Data Source=" + Con1.ToString() + ";Initial Catalog=DB_INVENTORY;Network Library=DBMSSOCN;User ID=" + User1.ToString() + "; Password=" + Pass1.ToString() + "";

            return con;
        }

        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
                // GET: Login
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
                return RedirectToAction("Dashboard", "Home");
            return View();
        }

        private void EnsureLoggedOut()
        {
            if (Request.IsAuthenticated)  // If the request is (still) marked as authenticated we send the user to the logout action
                LogoutAuthentication();
        }

        public ActionResult DatabaseAuthentication()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DatabaseAuthentication(AllModels model)
        {
            string ConnectionString = Server.MapPath("~/DatabaseConnection/ConnectionString.txt");
            string Username = Server.MapPath("~/DatabaseConnection/Username.txt");
            string Password = Server.MapPath("~/DatabaseConnection/Password.txt");

            string AdminUser = Server.MapPath("~/DatabaseConnection/AdminUser.txt");
            string AdminPass = Server.MapPath("~/DatabaseConnection/AdminPassword.txt");

            string AdminUserName = System.IO.File.ReadAllText(AdminUser);
            string AdminPassword = System.IO.File.ReadAllText(AdminPass);

            string User2 = EncryptionAndDecryption.Decrypt(AdminUserName);
            string Pass2 = EncryptionAndDecryption.Decrypt(AdminPassword);

            if (model.Maintenance.AdminUser != User2.ToString().TrimEnd() && model.Maintenance.AdminPassword != Pass2.ToString().TrimEnd())
            {
                ViewBag.HeaderError = string.Format("Unable  to connect to server.");
                ViewBag.MessageError = string.Format("Check your Admin Username or Admin Password.");
                return View();
            }


            using (StreamWriter sw = System.IO.File.CreateText(ConnectionString))
            {
                sw.WriteLine(model.Maintenance.DatabaseConnection);
            }

            using (StreamWriter sw = System.IO.File.CreateText(Username))
            {
                sw.WriteLine(EncryptionAndDecryption.Encrypt(model.Maintenance.DatabaseUser));
            }

            using (StreamWriter sw = System.IO.File.CreateText(Password))
            {
                sw.WriteLine(EncryptionAndDecryption.Encrypt(model.Maintenance.DatabasePassword));
            }

            DBConnection();

            using (SqlConnection connection = new SqlConnection(con))
            {
                try
                {
                    connection.Open();
                }
                catch
                {
                    ViewBag.HeaderError = string.Format("Unable  to connect to server.");
                    ViewBag.MessageError = string.Format("Check your server connection.");
                    return View();
                }
            }

            ViewBag.HeaderSuccess = string.Format("Connection Success.");
            ViewBag.MessageSuccess = string.Format("Your Connection Server has connected.");
            return View();
        }

        public ActionResult LoginDatabase()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginDatabase(AllModels model)
        {
            string AdminUser = Server.MapPath("~/DatabaseConnection/AdminUser.txt");
            string AdminPass = Server.MapPath("~/DatabaseConnection/AdminPassword.txt");

            string AdminUserName = System.IO.File.ReadAllText(AdminUser);
            string AdminPassword = System.IO.File.ReadAllText(AdminPass);

            string User2 = EncryptionAndDecryption.Decrypt(AdminUserName);
            string Pass2 = EncryptionAndDecryption.Decrypt(AdminPassword);

            if (model.Maintenance.AdminUser != User2.ToString().TrimEnd() || model.Maintenance.AdminPassword != Pass2.ToString().TrimEnd())
            {
                ViewBag.HeaderError = string.Format("Unable  to connect to server.");
                ViewBag.MessageError = string.Format("Check your Admin Username or Admin Password.");
                return View();
            }
            else
            {
                Session["AdminUser"] = User2;
                return RedirectToAction("UpdateData", "Login");
            }
        }

        public ActionResult UpdateData(string type)
        {
            if (Session["AdminUser"] == null)
            {
                return RedirectToAction("LoginDatabase","Login");
            }
            return View();
        }

        [HttpPost]
        public JsonResult UpdateDatabaseData(MaintenanceModel maintenance)
        {
            var result = 0;
            DBMaintenance dbmaintenance = new DBMaintenance();
            if (maintenance.ProcessType == "1")
            {
                result = dbmaintenance.UpdateReleasingData(maintenance);
            }
            else if (maintenance.ProcessType == "2")
            {
                result = dbmaintenance.UpdateReceivingData(maintenance);
            }
            if (result != 0)
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(result);
        }

        public ActionResult LogoutDatabase()
        {
            try
            {
                // First we clean the authentication ticket like always
                FormsAuthentication.SignOut();

                // Second we clear the principal to ensure the user does not retain any authentication
                HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
                Session.Abandon();
                Session.Clear();
                System.Web.HttpContext.Current.Session.RemoveAll();

                DBConnection();
                SqlDependency.Stop(con);
                SqlDependency.Stop(con2);
                SqlDependency.Stop(con3);
                return RedirectToAction("LoginDatabase", "Login");

                //return View("~/Views/Login/LoginAuthentication.cshtml");
            }
            catch
            {
                FormsAuthentication.SignOut();
                // Second we clear the principal to ensure the user does not retain any authentication
                HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
                Session.Abandon();
                Session.Clear();
                System.Web.HttpContext.Current.Session.RemoveAll();

                DBConnection();
                SqlDependency.Stop(con);
                SqlDependency.Stop(con2);
                SqlDependency.Stop(con3);
                return RedirectToAction("LoginDatabase","Login");

                //return View("~/Views/Login/LoginAuthentication.cshtml");

            }
        }


        public ActionResult LogoutAuthentication()
        {
            try
            {
                // First we clean the authentication ticket like always
                FormsAuthentication.SignOut();

                // Second we clear the principal to ensure the user does not retain any authentication
                HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
                Session.Abandon();
                Session.Clear();
                System.Web.HttpContext.Current.Session.RemoveAll();

                DBConnection();
                SqlDependency.Stop(con);
                SqlDependency.Stop(con2);
                SqlDependency.Stop(con3);
                ViewBag.LoginAuthentication = "Login";
                return RedirectToAction("LoginAuthentication");

                //return View("~/Views/Login/LoginAuthentication.cshtml");
            }
            catch
            {
                FormsAuthentication.SignOut();
                // Second we clear the principal to ensure the user does not retain any authentication
                HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
                Session.Abandon();
                Session.Clear();
                System.Web.HttpContext.Current.Session.RemoveAll();

                DBConnection();
                SqlDependency.Stop(con);
                SqlDependency.Stop(con2);
                SqlDependency.Stop(con3);
                ViewBag.LoginAuthentication = "Login";
                return RedirectToAction("LoginAuthentication");

                //return View("~/Views/Login/LoginAuthentication.cshtml");

            }
        }

        [AllowAnonymous]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult LoginAuthentication()
        {
 
            if (Session["UserID"] != null)
                return RedirectToAction("Dashboard", "Home");

            ViewBag.LoginAuthentication = "Login";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LoginAuthentication(MaintenanceModel maintenance)
        {
            var username = maintenance.Username;
            var password = maintenance.Password;
            //if (User.Identity.IsAuthenticated)
            //{
            //    return RedirectToAction("Dashboard", "Home");
            //}
            //System.Web.Helpers.AntiForgery.Validate();
            try
            {
                DBConnection();

                DBMaintenance dBMaintenance = new DBMaintenance();
                maintenance = dBMaintenance.UserAuthentication(maintenance);

            }
            catch
            {
                ViewBag.LoginAuthentication = "Login";
                ViewBag.HeaderError = string.Format("Unable  to connect to server.");
                ViewBag.MessageError = string.Format("Check your server connection settings.");

                ViewBag.UserField = username;
                return View();
            }
            
            if (maintenance != null)
            {

                if (maintenance.Status == "2")
                {
                    ViewBag.LoginAuthentication = "Login";
                    ViewBag.HeaderError = string.Format("Account Issue.");
                    ViewBag.MessageError = string.Format("Your account has been inactive.");

                    ViewBag.UserField = username;
                    return View();
                }

                else if (maintenance.ResetPassword == 1)
                {
                    if (maintenance.Code.Trim() == password || maintenance.Password.Trim() == "P@ssw0rd")
                    {
                        ViewBag.LoginAuthentication = "ResetPassword";
                        AllModels models = new AllModels();
                        models.Maintenance = maintenance;
                        return View(models);
                    }
                    else
                    {
                        ViewBag.LoginAuthentication = "Login";
                        ViewBag.HeaderWarning = string.Format("Reset Password.");
                        ViewBag.MessageWarning = string.Format("Please Enter Code.");

                        ViewBag.UserField = username;
                        return View();
                    }
                }
                else
                {
                    if (maintenance.Code == password)
                    {
                        ViewBag.LoginAuthentication = "Login";
                        ViewBag.HeaderError = string.Format("Login Failed.");
                        ViewBag.MessageError = string.Format("Username or Password is incorrect.");

                        ViewBag.UserField = username;
                        return View();
                    }
                    else
                    {
                        Session["UserID"] = maintenance.UserID;
                        Session["Fullname"] = maintenance.Fullname;
                        Session["UserRoleID"] = maintenance.RoleID;
                        Session["UserRole"] = maintenance.RoleName;
                        Session["Department"] = maintenance.DepartmentID;
                        Session["GroupID"] = maintenance.GroupID;
                        Session["DesignationID"] = maintenance.DesignationID;
                        Session["GroupName"] = maintenance.Group;
                        Session["DesignationName"] = maintenance.Designation;

                        FormsAuthentication.SetAuthCookie(Session["UserID"].ToString(), false); // render Session into Authentication Cookie

                        SqlDependency.Start(con);
                        SqlDependency.Start(con2);
                        SqlDependency.Start(con3);
                        NotificationComponent NC = new NotificationComponent();
                        NC.group = maintenance.GroupID;
                        NC.Notificaton(maintenance.GroupID);
                        NC.UpdateItemRequestNotificaton();
                        NC.NotificatonItem();
                        NC.NotificatonUpdateItem();
                        return RedirectToAction("Dashboard", "Home");
                    }
                }
            }
            else
            {
                ViewBag.LoginAuthentication = "Login";
                ViewBag.HeaderError = string.Format("Login Failed.");
                ViewBag.MessageError = string.Format("Username or Password is incorrect.");

                ViewBag.UserField = username;
                return View();
            }
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult ResetPassword(MaintenanceModel maintenance)
        {
            DBConnection();
            DBMaintenance dBMaintenance = new DBMaintenance();

            var result = dBMaintenance.ResetPassword(maintenance);

            maintenance = dBMaintenance.PopulateUserInformationDB(maintenance.UserID);

            //ViewBag.LoginAuthentication = "Login";
            //ViewBag.HeaderSuccess = string.Format("New password set.");
            //ViewBag.MessageSuccess = string.Format("Your Password has been changed.");

            Session["UserID"] = maintenance.UserID;
            Session["Fullname"] = maintenance.Fullname;
            Session["UserRoleID"] = maintenance.RoleID;
            Session["UserRole"] = maintenance.RoleName;
            Session["Department"] = maintenance.DepartmentID;
            Session["GroupID"] = maintenance.GroupID;
            Session["DesignationID"] = maintenance.DesignationID;
            Session["GroupName"] = maintenance.Group;
            Session["DesignationName"] = maintenance.Designation;

            FormsAuthentication.SetAuthCookie(Session["UserID"].ToString(), false); // render Session into Authentication Cookie
            SqlDependency.Start(con);
            SqlDependency.Start(con2);
            SqlDependency.Start(con3);
            NotificationComponent NC = new NotificationComponent();
            NC.group = maintenance.DesignationID;
            NC.Notificaton(maintenance.DesignationID);
            NC.UpdateItemRequestNotificaton();
            NC.NotificatonItem();
            NC.NotificatonUpdateItem();
            return Json(result);
        }

        //public ActionResult ConfirmResetPassword()
        //{
        //    return RedirectToAction("Dashboard", "Home");
        //}
    }
}