using GMCINVENTORYSYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GMCINVENTORYSYSTEM
{
    public class NotificationComponent
    {
        public string con;
        public string con2;
        public string con3;
        public string user, userrole, group;

        private string CoreDB = ConfigurationManager.ConnectionStrings["CoreDB"].ConnectionString;

        DatabaseConnectionClass databaseConnectionClass = new DatabaseConnectionClass();

        public string DBConnection()
        {
            string Con = databaseConnectionClass.var1;
            string User = databaseConnectionClass.var2;
            string Pass = databaseConnectionClass.var3;
            string Connectionstring = File.ReadAllText(Con);
            string ConnectionUserName = File.ReadAllText(User);
            string ConnectionPasswrod = File.ReadAllText(Pass);

            string Con1 = Connectionstring.TrimEnd();
            string User1 = EncryptionAndDecryption.Decrypt(ConnectionUserName);
            string Pass1 = EncryptionAndDecryption.Decrypt(ConnectionPasswrod);



            con = "Data Source=" + Con1.ToString() + ";Initial Catalog=DB_CORE;Network Library=DBMSSOCN;User ID=" + User1.ToString() + "; Password=" + Pass1.ToString() + "";
            con2 = "Data Source=" + Con1.ToString() + ";Initial Catalog=DB_CENTRAL;Network Library=DBMSSOCN;User ID=" + User1.ToString() + "; Password=" + Pass1.ToString() + "";
            con3 = "Data Source=" + Con1.ToString() + ";Initial Catalog=DB_INVENTORY;Network Library=DBMSSOCN;User ID=" + User1.ToString() + "; Password=" + Pass1.ToString() + "";

            return con;
        }

        public string name { get; set; }

        public void Notificaton(string group)
        {
            DBConnection();

            string sqlCommand = @"SELECT [ReferenceNo] FROM [dbo].[REQUEST_COMMON] where [RequestStatus] = 'FOR APPROVAL' and [GroupID] ='" + group +"'";

            using (SqlConnection connection = new SqlConnection(con2))
            {

                SqlCommand cmd = new SqlCommand(sqlCommand, connection);

                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);
                sqlDep.OnChange += new OnChangeEventHandler(sqlDep_OnChange);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // nothing need to add here
                }
            }

        }

        public void UpdateItemRequestNotificaton()
        {
            DBConnection();
            string sqlCommand = @"SELECT [ReferenceNo] FROM [dbo].[REQUEST_COMMON] where (([ItemStatus] = 'FOR PICKUP' and [RequestStatus] in ('APPROVED','FOR PICKUP')) or [RequestStatus] = 'REJECTED' and [NotificationStatus] = 'VIEW') and [Responsible] = '" + name + "'";
            //string sqlCommand = @"select [ReferenceNo]  From DB_CORE.dbo.NotifyCommonRequestContent";

            //using (SqlConnection connection = new SqlConnection(con2))
            using (SqlConnection connection = new SqlConnection(con2))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(sqlCommand, connection);

                //cmd.CommandType = CommandType.StoredProcedure;
                ////cmd.Parameters.Add(new SqlParameter("@UserID", user ?? ""));
                ////cmd.Parameters.Add(new SqlParameter("@UserRoleID", userrole));
                //cmd.Parameters.AddWithValue("@UserID", user ?? "");
                //cmd.Parameters.AddWithValue("@UserRoleID", userrole);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);
                sqlDep.OnChange += new OnChangeEventHandler(sqlDep_OnChange);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // nothing need to add here
                }
            }

        }



        public void NotificatonItem()
        {
            DBConnection();

            string sqlCommand = @"SELECT [StockNo] FROM [DB_CORE].[dbo].[NotificationInventory]";

            using (SqlConnection connection = new SqlConnection(con3))
            {

                SqlCommand cmd = new SqlCommand(sqlCommand, connection);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);
                sqlDep.OnChange += new OnChangeEventHandler(sqlDep_OnChange);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // nothing need to add here
                }
            }

        }

        public void NotificatonUpdateItem()
        {
            DBConnection();

            string sqlCommand = @"SELECT [StockNo] FROM [dbo].[ITEM_MATERLIST]";

            using (SqlConnection connection = new SqlConnection(con3))
            {

                SqlCommand cmd = new SqlCommand(sqlCommand, connection);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }
                cmd.Notification = null;
                SqlDependency sqlDep = new SqlDependency(cmd);
                sqlDep.OnChange += new OnChangeEventHandler(sqlDep_OnChange);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // nothing need to add here
                }
            }

        }


        void sqlDep_OnChange(Object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                NotificationHub.SendMessages();

                Notificaton(group);
                NotificatonItem();
                NotificatonUpdateItem();
                UpdateItemRequestNotificaton();
                //SupplierNotificaton(name);
                //CoCNotificaton(name);

            }

        }
    }
}