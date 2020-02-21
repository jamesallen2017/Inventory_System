using GMCINVENTORYSYSTEM.Models;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace GMCINVENTORYSYSTEM.Controllers
{
    public class ReportController : Controller
    {
        public string con;

        public string ReferenceNo { get; set; }

        private string CoreDB = ConfigurationManager.ConnectionStrings["CoreDB"].ConnectionString;

        DBSatellite dBSatellite = new DBSatellite();
        DBMaintenance dBMaintenance = new DBMaintenance();
        DBCentral dBCentral = new DBCentral();

        MaintenanceModel maintenanceModel = new MaintenanceModel();
        CentralModel centralModel = new CentralModel();
        SatelliteModel satellite = new SatelliteModel();



        // GET: Report
        //
        // GET: /Report/
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult InventoryReports()
		{
            try
            {
                if (Session["UserRoleID"].ToString() != "2" && Session["UserRoleID"].ToString() != "3" &&
                Session["UserRoleID"].ToString() != "4" && Session["UserRoleID"].ToString() != "5")
                {
                    return Redirect("/Home/Dashboard");
                }
            }
            catch (Exception)
            {
                return Redirect("/Home/Dashboard");
            }

			dBCentral.DBConnection();
			var cons = dBCentral.con;

			var DesignationID = Session["DesignationID"].ToString();
			var GroupID = Session["GroupID"].ToString();
            string Designation = "";
            if (Session["ddDesignation"] != null)
            {
                Designation = Session["ddDesignation"].ToString() ?? "";
            }

			using (SqlConnection con = new SqlConnection(cons))
			{
                //Warning[] warnings;
                //string[] streams;
                //string MIMETYPE = string.Empty;
                //string encoding = string.Empty;
                //string extension = string.Empty;

				//process report
				ReportViewer rptviewer = new ReportViewer();
				rptviewer.ProcessingMode = ProcessingMode.Local;

				using (SqlCommand cmd2 = new SqlCommand("Report_InventoryDetails", con))
				{
					con.Open();
					cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@ddlDesignationID", Designation);
					cmd2.Parameters.AddWithValue("@DesignationID", DesignationID);
					cmd2.Parameters.AddWithValue("@GroupID", GroupID);

					//fill dataset
					SqlDataAdapter adp = new SqlDataAdapter(cmd2);
					DataSet ds = new DataSet();
					//call stored procedure
					adp.Fill(ds, "Report_InventoryDetails");

                    rptviewer.LocalReport.DataSources.Clear();
					ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
					rptviewer.LocalReport.DataSources.Add(datasource);

					con.Close();
                }
                rptviewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath);
                //call report path
                rptviewer.LocalReport.ReportPath += @"Views/Report/Inventory.rdlc";
                rptviewer.SizeToReportContent = true;
                rptviewer.Height = Unit.Percentage(100);
                rptviewer.Width = Unit.Percentage(100);
                rptviewer.LocalReport.Refresh();

                ViewBag.ReportViewer = rptviewer;

                ////add datasoure to report
                ////convert report data to bytes
                //byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                ////process report to pdf format
                //Session["PreviewTest"] = "Download";
                //Response.ClearHeaders();
                //Response.ContentType = "application/pdf";
                //Response.BinaryWrite(bytes);
                //Response.Flush();
                //Response.Close();
                //con.Close();
			}
			return View();
		}

		//Item Location Reports
		[HttpGet]
        public ActionResult ItemLocationReports()
        {
            try
            {
                if (Session["UserRoleID"].ToString() != "2" && Session["UserRoleID"].ToString() != "3" &&
                Session["UserRoleID"].ToString() != "4" && Session["UserRoleID"].ToString() != "5")
                {
                    return Redirect("/Home/Dashboard");
                }
            }
            catch (Exception)
            {
                return Redirect("/Home/Dashboard");
            }
            if (Session["Model"].ToString() == "Central")
            {
                string id = Session["LocationItemNo"].ToString();
                dBCentral.DBConnection();
                var cons = dBCentral.con;

                using (SqlConnection con = new SqlConnection(cons))
                {
                    DataSet ds = new DataSet();
                    Warning[] warnings;
                    string[] streams;
                    string MIMETYPE = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;
                    //process report
                    ReportViewer rptviewer = new ReportViewer();
                    rptviewer.ProcessingMode = ProcessingMode.Local;
                    //call report path
                    rptviewer.LocalReport.ReportPath = "Views/Report/ItemLocation.rdlc";

                    ReportParameter Status = new ReportParameter("Status", Session["Model"].ToString());
                    rptviewer.LocalReport.SetParameters(new ReportParameter[] { Status });

                    using (SqlCommand cmd2 = new SqlCommand("Report_ItemLocation", con))
                    {
                        con.Open();
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@Items", id ?? "");

                        //fill dataset
                        SqlDataAdapter adp = new SqlDataAdapter(cmd2);
                        //call stored procedure
                        adp.Fill(ds, "Report_ItemLocation");
                        ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                        rptviewer.LocalReport.DataSources.Add(datasource);
                        ReportDataSource datasource1 = new ReportDataSource("DataSet2", ds.Tables[0]);
                        rptviewer.LocalReport.DataSources.Add(datasource1);

                        con.Close();
                    }

                    //using (SqlCommand command = new SqlCommand("Report_FilteredItemLocation", con))
                    //{
                    //    con.Open();
                    //    command.CommandType = CommandType.StoredProcedure;
                    //    command.Parameters.AddWithValue("@Status", id);

                    //    //fill dataset
                    //    SqlDataAdapter adp = new SqlDataAdapter(command);
                    //    //DataSet ds = new DataSet();
                    //    //call stored procedure
                    //    adp.Fill(ds, "Report_FilteredItemLocation");
                    //    rptviewer.LocalReport.DataSources.Add(datasource);

                    //    con.Close();
                    //}

                    //add datasoure to report
                    //convert report data to bytes
                    byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                    //process report to pdf format
                    Session["PreviewTest"] = "Download";
                    Response.ClearHeaders();
                    Response.ContentType = "application/pdf";
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    Response.Close();
                    con.Close();
                }
            }
            else if (Session["Model"].ToString() == "Maintenance")
            {
                string id = Session["ItemLocation"].ToString();
                dBCentral.DBConnection();
                var cons = dBCentral.con;

                using (SqlConnection con = new SqlConnection(cons))
                {
                    Warning[] warnings;
                    string[] streams;
                    string MIMETYPE = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;
                    //process report
                    ReportViewer rptviewer = new ReportViewer();
                    rptviewer.ProcessingMode = ProcessingMode.Local;

                    //call report path
                    rptviewer.LocalReport.ReportPath = "Views/Report/ItemLocation.rdlc";

                    ReportParameter Status = new ReportParameter("Status", Session["Model"].ToString());
                    rptviewer.LocalReport.SetParameters(new ReportParameter[] { Status });

                    using (SqlCommand cmd2 = new SqlCommand("Report_ItemLocation", con))
                    {
                        con.Open();
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@Items", id ?? "");

                        //fill dataset
                        SqlDataAdapter adp = new SqlDataAdapter(cmd2);
                        DataSet ds = new DataSet();
                        //call stored procedure
                        adp.Fill(ds, "Report_ItemLocation");
                        ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                        rptviewer.LocalReport.DataSources.Add(datasource);

                        con.Close();
                    }

                    using (SqlCommand cmd2 = new SqlCommand("Report_FilteredItemLocation", con))
                    {
                        con.Open();
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@Status", id);

                        //fill dataset
                        SqlDataAdapter adp = new SqlDataAdapter(cmd2);
                        DataSet ds = new DataSet();
                        //call stored procedure
                        adp.Fill(ds, "Report_FilteredItemLocation");
                        ReportDataSource datasource = new ReportDataSource("DataSet2", ds.Tables[0]);
                        rptviewer.LocalReport.DataSources.Add(datasource);

                        con.Close();
                    }

                    //add datasoure to report
                    //convert report data to bytes
                    byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                    //process report to pdf format
                    Session["PreviewTest"] = "Download";
                    Response.ClearHeaders();
                    Response.ContentType = "application/pdf";
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    Response.Close();
                    con.Close();
                }
            }
            return View();
        }

        public ActionResult ManualAdjustmentReport()
        {
            try
            {
                if (Session["UserRoleID"].ToString() != "2" && Session["UserRoleID"].ToString() != "3" &&
                Session["UserRoleID"].ToString() != "4" && Session["UserRoleID"].ToString() != "5")
                {
                    return Redirect("/Home/Dashboard");
                }
            }
            catch (Exception)
            {
                return Redirect("/Home/Dashboard");
            }
            var From = "";
            var To = "";
            if (Session["ManualFrom"].ToString() != null && Session["ManualTo"].ToString() != null)
	        {
		        From = Session["ManualFrom"].ToString();
                To = Session["ManualTo"].ToString();
	        }
            dBCentral.DBConnection();
            var cons = dBCentral.con;

            using (SqlConnection con = new SqlConnection(cons))
            {
                Warning[] warnings;
                string[] streams;
                string MIMETYPE = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                //process report
                ReportViewer rptviewer = new ReportViewer();
                rptviewer.ProcessingMode = ProcessingMode.Local;
                //call report path
                rptviewer.LocalReport.ReportPath = "Views/Report/ManualAdjustment.rdlc";

                using (SqlCommand cmd2 = new SqlCommand("Report_ManualAdjustment", con))
                {
                    con.Open();
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@From", From);
                    cmd2.Parameters.AddWithValue("@To", To);

                    //fill dataset
                    SqlDataAdapter adp = new SqlDataAdapter(cmd2);
                    DataSet ds = new DataSet();
                    //call stored procedure
                    adp.Fill(ds, "Report_ManualAdjustment");
                    ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                    rptviewer.LocalReport.DataSources.Add(datasource);

                    con.Close();
                }

                //add datasoure to report
                //convert report data to bytes
                byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                //process report to pdf format
                Session["PreviewTest"] = "Download";
                Response.ClearHeaders();
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.Close();
                con.Close();
            }
            return View();
        }

        public ActionResult ItemMonitoringReports()
        {
            try
            {
                if (Session["UserRoleID"].ToString() != "2" && Session["UserRoleID"].ToString() != "3" &&
                Session["UserRoleID"].ToString() != "4" && Session["UserRoleID"].ToString() != "5")
                {
                    return Redirect("/Home/Dashboard");
                }
            }
            catch (Exception)
            {
                return Redirect("/Home/Dashboard");
            }
            if (Session["Report"].ToString() == "ReceivedItem") //ReceivedItemMonitoring
            {
                var From = "";
                var To = "";
                string Designation = "";
                if (Session["ReceivedDesignation"] != null)
                {
                    Designation = Session["ReceivedDesignation"].ToString();
                }
                if (Session["ReceivedFrom"].ToString() != null && Session["ReceivedTo"].ToString() != null)
                {
                    From = Session["ReceivedFrom"].ToString();
                    To = Session["ReceivedTo"].ToString();
                }
                dBCentral.DBConnection();
                var cons = dBCentral.con;

                using (SqlConnection con = new SqlConnection(cons))
                {
                    Warning[] warnings;
                    string[] streams;
                    string MIMETYPE = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;
                    //process report
                    ReportViewer rptviewer = new ReportViewer();
                    rptviewer.ProcessingMode = ProcessingMode.Local;
                    //call report path
                    rptviewer.LocalReport.ReportPath = "Views/Report/ReceivedItem.rdlc";

                    using (SqlCommand cmd2 = new SqlCommand("Report_ReceivedItem", con))
                    {
                        con.Open();
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@Designation", Designation);
                        cmd2.Parameters.AddWithValue("@From", From);
                        cmd2.Parameters.AddWithValue("@To", To);

                        //fill dataset
                        SqlDataAdapter adp = new SqlDataAdapter(cmd2);
                        DataSet ds = new DataSet();
                        //call stored procedure
                        adp.Fill(ds, "Table_PopulateRecivedLogs");
                        ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                        rptviewer.LocalReport.DataSources.Add(datasource);

                        con.Close();
                    }

                    //add datasoure to report
                    //convert report data to bytes
                    byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                    //process report to pdf format
                    Session["PreviewTest"] = "Download";
                    Response.ClearHeaders();
                    Response.ContentType = "application/pdf";
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    Response.Close();
                    con.Close();
                }
            }
            else if (Session["Report"].ToString() == "ReleasedItem") //ReleasedItemMonitoring
            {

                var From = "";
                var To = "";
                string Designation = "";
                if (Session["ReleasedDesignation"] != null)
                {
                    Designation = Session["ReleasedDesignation"].ToString();
                }
                if (Session["ReleasedFrom"].ToString() != null && Session["ReleasedTo"].ToString() != null)
                {
                    From = Session["ReleasedFrom"].ToString();
                    To = Session["ReleasedTo"].ToString();
                }
                dBCentral.DBConnection();
                var cons = dBCentral.con;

                using (SqlConnection con = new SqlConnection(cons))
                {
                    Warning[] warnings;
                    string[] streams;
                    string MIMETYPE = string.Empty;
                    string encoding = string.Empty;
                    string extension = string.Empty;
                    //process report
                    ReportViewer rptviewer = new ReportViewer();
                    rptviewer.ProcessingMode = ProcessingMode.Local;
                    //call report path
                    rptviewer.LocalReport.ReportPath = "Views/Report/ReleasedItem.rdlc";

                    using (SqlCommand cmd2 = new SqlCommand("Report_ReleasedItem", con))
                    {
                        con.Open();
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@Designation", Designation);
                        cmd2.Parameters.AddWithValue("@From", From);
                        cmd2.Parameters.AddWithValue("@To", To);

                        //fill dataset
                        SqlDataAdapter adp = new SqlDataAdapter(cmd2);
                        DataSet ds = new DataSet();
                        //call stored procedure
                        adp.Fill(ds, "Table_PopulateReleaseLogs");
                        ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                        rptviewer.LocalReport.DataSources.Add(datasource);

                        con.Close();
                    }

                    //add datasoure to report
                    //convert report data to bytes
                    byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                    //process report to pdf format
                    Session["PreviewTest"] = "Download";
                    Response.ClearHeaders();
                    Response.ContentType = "application/pdf";
                    Response.BinaryWrite(bytes);
                    Response.Flush();
                    Response.Close();
                    con.Close();
                }
            }
            return View();
        }

        public ActionResult TransactionReports()
        {
            try
            {
                if (Session["UserRoleID"].ToString() != "2" && Session["UserRoleID"].ToString() != "3" &&
                Session["UserRoleID"].ToString() != "4" && Session["UserRoleID"].ToString() != "5")
                {
                    return Redirect("/Home/Dashboard");
                }
            }
            catch (Exception)
            {
                return Redirect("/Home/Dashboard");
            }
            var From = "";
            var To = "";
            var Transaction = "";
            if (Session["ItemFrom"].ToString() != null && Session["ItemTo"].ToString() != null)
            {
                From = Session["ItemFrom"].ToString();
                To = Session["ItemTo"].ToString();
            }
            if (Session["TransactionType"].ToString() != null)
            {
                Transaction = Session["TransactionType"].ToString();
            }
            dBCentral.DBConnection();
            var cons = dBCentral.con;

            using (SqlConnection con = new SqlConnection(cons))
            {
                Warning[] warnings;
                string[] streams;
                string MIMETYPE = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                //process report
                ReportViewer rptviewer = new ReportViewer();
                rptviewer.ProcessingMode = ProcessingMode.Local;
                //call report path
                rptviewer.LocalReport.ReportPath = "Views/Report/Transaction.rdlc";

                using (SqlCommand cmd2 = new SqlCommand("Report_Transactions", con))
                {
                    con.Open();
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@From",From);
                    cmd2.Parameters.AddWithValue("@To", To);
                    cmd2.Parameters.AddWithValue("@Type", Transaction);

                    //fill dataset
                    SqlDataAdapter adp = new SqlDataAdapter(cmd2);
                    DataSet ds = new DataSet();
                    //call stored procedure
                    adp.Fill(ds, "Report_Transactions");
                    ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                    rptviewer.LocalReport.DataSources.Add(datasource);

                    con.Close();
                }

                //add datasoure to report
                //convert report data to bytes
                byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                //process report to pdf format
                Session["PreviewTest"] = "Download";
                Response.ClearHeaders();
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.Close();
                con.Close();
            }
            return View();
        }

        public ActionResult ListOfPRReports()
        {
            try
            {
                if (Session["UserRoleID"].ToString() != "2" && Session["UserRoleID"].ToString() != "3" &&
                Session["UserRoleID"].ToString() != "4" && Session["UserRoleID"].ToString() != "5")
                {
                    return Redirect("/Home/Dashboard");
                }
            }
            catch (Exception)
            {
                return Redirect("/Home/Dashboard");
            }
            var From = "";
            var To = "";
            if (Session["PRFrom"].ToString() != null && Session["PRTo"].ToString() != null)
            {
                From = Session["PRFrom"].ToString();
                To = Session["PRTo"].ToString();
            }
            dBCentral.DBConnection();
            var cons = dBCentral.con;

            using (SqlConnection con = new SqlConnection(cons))
            {
                Warning[] warnings;
                string[] streams;
                string MIMETYPE = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                //process report
                ReportViewer rptviewer = new ReportViewer();
                rptviewer.ProcessingMode = ProcessingMode.Local;
                //call report path
                rptviewer.LocalReport.ReportPath = "Views/Report/ListOfPR.rdlc";

                using (SqlCommand cmd2 = new SqlCommand("Report_ListofPR", con))
                {
                    con.Open();
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@From", From);
                    cmd2.Parameters.AddWithValue("@To", To);

                    //fill dataset
                    SqlDataAdapter adp = new SqlDataAdapter(cmd2);
                    DataSet ds = new DataSet();
                    //call stored procedure
                    adp.Fill(ds, "Report_ListofPR");
                    ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                    rptviewer.LocalReport.DataSources.Add(datasource);

                    con.Close();
                }

                //add datasoure to report
                //convert report data to bytes
                byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                //process report to pdf format
                Session["PreviewTest"] = "Download";
                Response.ClearHeaders();
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.Close();
                con.Close();
            }
            return View();
        }

        [HttpGet]
        public ActionResult PRListReports()
        {
            try
            {
                if (Session["UserRoleID"].ToString() != "2" && Session["UserRoleID"].ToString() != "3" &&
                Session["UserRoleID"].ToString() != "4" && Session["UserRoleID"].ToString() != "5")
                {
                    return Redirect("/Home/Dashboard");
                }
            }
            catch (Exception)
            {
                return Redirect("/Home/Dashboard");
            }
            string DesignationID = "";
            if (Session["PRList"] != null)
            {
                DesignationID = Session["PRList"].ToString();
            }
            dBCentral.DBConnection();
            var cons = dBCentral.con;

            using (SqlConnection con = new SqlConnection(cons))
            {
                Warning[] warnings;
                string[] streams;
                string MIMETYPE = string.Empty;
                string encoding = string.Empty;
                string extension = string.Empty;
                //process report
                ReportViewer rptviewer = new ReportViewer();
                rptviewer.ProcessingMode = ProcessingMode.Local;
                //call report path
                rptviewer.LocalReport.ReportPath = "Views/Report/PRList.rdlc";

                using (SqlCommand cmd2 = new SqlCommand("Report_PRList", con))
                {
                    con.Open();
                    cmd2.CommandType = CommandType.StoredProcedure;
                    cmd2.Parameters.AddWithValue("@SelectedDesignation", DesignationID ?? "");

                    //fill dataset
                    SqlDataAdapter adp = new SqlDataAdapter(cmd2);
                    DataSet ds = new DataSet();
                    //call stored procedure
                    adp.Fill(ds, "Report_PRList");
                    ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);
                    rptviewer.LocalReport.DataSources.Add(datasource);

                    con.Close();
                }

                //add datasoure to report
                //convert report data to bytes
                byte[] bytes = rptviewer.LocalReport.Render("PDF", null, out MIMETYPE, out encoding, out extension, out streams, out warnings);
                //process report to pdf format
                Session["PreviewTest"] = "Download";
                Response.ClearHeaders();
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(bytes);
                Response.Flush();
                Response.Close();
                con.Close();
            }
            return View();
        }
    }
}