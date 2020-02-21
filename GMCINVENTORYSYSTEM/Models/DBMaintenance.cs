using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GMCINVENTORYSYSTEM.Models
{

	public class DBMaintenance
	{
		public string con;

		public string StockonHand;
		public string SafetyStock;

        //private string CoreDB = ConfigurationManager.ConnectionStrings["CoreDB"].ConnectionString;

		DatabaseConnectionClass databaseConnectionClass = new DatabaseConnectionClass();
		public string DBConnection()
		{
			string Con = databaseConnectionClass.var1;
			string User = databaseConnectionClass.var2;
			string Pass = databaseConnectionClass.var3;
			string Connectionstring = File.ReadAllText(Con);
			string ConnectionUserName = File.ReadAllText(User);
			string ConnectionPasswrod = File.ReadAllText(Pass);

			string Con1 = Connectionstring;
			string User1 = EncryptionAndDecryption.Decrypt(ConnectionUserName);
			string Pass1 = EncryptionAndDecryption.Decrypt(ConnectionPasswrod);



            con = "Data Source=" + Con1.ToString() + ";Initial Catalog=DB_CORE;Network Library=DBMSSOCN;User ID=" + User1.ToString() + "; Password=" + Pass1.ToString() + "";
			return con;
		}

		public int UpdateItemMasterInformationDB(MaintenanceModel maintenanceModel, string responsible)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			int i = 0;
			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Update_ItemMasterInformation", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@OldStockNo", maintenanceModel.OldStockNo.Trim());
						command.Parameters.AddWithValue("@StockNo", maintenanceModel.StockNo.Trim());
						command.Parameters.AddWithValue("@ItemName", maintenanceModel.ItemName.Trim());
                        command.Parameters.AddWithValue("@ItemDescription", maintenanceModel.ItemDescription ?? "");
						command.Parameters.AddWithValue("@Specification", maintenanceModel.Specification.Trim());
                        command.Parameters.AddWithValue("@Details", maintenanceModel.Details ?? "");
						command.Parameters.AddWithValue("@StockOnHand", maintenanceModel.StockOnHand.Trim());
						command.Parameters.AddWithValue("@SafetyStock", maintenanceModel.SafetyStock.Trim());
						command.Parameters.AddWithValue("@UOM", maintenanceModel.Unit.Trim());
						command.Parameters.AddWithValue("@GroupID", maintenanceModel.Group);
						command.Parameters.AddWithValue("@DesignationID", maintenanceModel.Designation ?? "");
						command.Parameters.AddWithValue("@TypeOfItem", maintenanceModel.TypeID);
                        command.Parameters.AddWithValue("@CriticalOrder", maintenanceModel.CriticalOrder.Trim());
                        command.Parameters.AddWithValue("@Status", maintenanceModel.Status);
                        command.Parameters.AddWithValue("@Pullout", maintenanceModel.Pullout ?? "");


						command.Parameters.AddWithValue("@Responsible", responsible);
						i++;
						command.ExecuteNonQuery();
						return i;
					}
				}
			}
			catch
			{
				i = 0;
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

        public int UpdateItemMasterStatusDB(MaintenanceModel maintenance)
        {
            DBConnection();
            int result = 0;
            SqlConnection COREConnection = null;

            using (COREConnection = new SqlConnection(con))
            {
                using (SqlCommand CORECommand = new SqlCommand("Update_ItemMasterStatus", COREConnection))
                {
                    CORECommand.CommandType = CommandType.StoredProcedure;
                    CORECommand.Parameters.AddWithValue("@StockNo", maintenance.StockNo);
                    CORECommand.Parameters.AddWithValue("@Status", maintenance.Status);

                    COREConnection.Open();
                    CORECommand.ExecuteNonQuery();
                    result++;
                    COREConnection.Close();
                    return result;
                }
            }
        }

        public int ItemHasStockDB(MaintenanceModel maintenance)
        {
            DBConnection();
            int i = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(con))
                {
                    connection.Open();

                    using (command = new SqlCommand("Check_ItemHasRequest", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@StockNo", maintenance.StockNo);

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        i = dt.Rows.Count;

                        //command.ExecuteNonQuery();
                    }

                    return i;

                }
            }
            catch
            {
                i = 0;
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SelectListItem> SelectPullOutDB(MaintenanceModel maintenance)
        {
            DBConnection();
            List<SelectListItem> PullOutList = new List<SelectListItem>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(con))
                {
                    using (command = new SqlCommand("Select_PopulatePullOut", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    PullOutList.Add(new SelectListItem
                                    {
                                        Text = row["Name"].ToString(),
                                        Value = row["ID"].ToString()
                                    });
                                }
                            }
                        }
                        return PullOutList;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }

        }


		public void DeleteItemlocation(string StockNo)
		{
			DBConnection();

			SqlConnection connection = null;
			SqlCommand command = null;
			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Delete_ItemLocation", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@StockNo", StockNo);
						command.ExecuteNonQuery();
					}
				}

			}
			catch
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

		public int InsertRackInformationDB(List<MaintenanceModel> maintenanceModel)
		{
			DBConnection();

			int i = 0;
			SqlConnection connection = null;
			SqlCommand command = null;
			try
			{
				if (maintenanceModel == null)
				{
					maintenanceModel = new List<MaintenanceModel>();
				}

				using (connection = new SqlConnection(con))
				{
					connection.Open();
					foreach (var item in maintenanceModel)
					{
						using (command = new SqlCommand("Insert_RackInformation", connection))
						{
							command.CommandType = CommandType.StoredProcedure;
							command.Parameters.AddWithValue("@RackNo", item.RackNo.ToUpper().Trim());
							command.Parameters.AddWithValue("@RackDescription", item.RackDescription ?? "");
							command.ExecuteNonQuery();
						}

						i++;
					}
					return i;
				}
			}
			catch
			{
				i = 0;
				throw;
			}
			finally
			{
				connection.Close();
			}

		}

		public int InsertDrawerInformationDB(List<MaintenanceModel> maintenanceModel)
		{
			DBConnection();
			int i = 0;
			SqlConnection connection = null;
			SqlCommand command = null;
			try
			{
				if (maintenanceModel == null)
				{
					maintenanceModel = new List<MaintenanceModel>();
				}

				using (connection = new SqlConnection(con))
				{
					connection.Open();
					foreach (var item in maintenanceModel)
					{
						using (command = new SqlCommand("Insert_DrawerInformation", connection))
						{
							command.CommandType = CommandType.StoredProcedure;
                            command.Parameters.AddWithValue("@DrawerID", item.DrawerNo.ToUpper().Trim());
							command.Parameters.AddWithValue("@DrawerName", item.DrawerDescription ?? "");
							command.Parameters.AddWithValue("@MainID", item.MainNo);
							i++;
							command.ExecuteNonQuery();
						}
					}
					return i;

				}
			}
			catch
			{
				i = 0;
				throw;
			}
			finally
			{
				connection.Close();
			}

		}

		public int InsertBinInformationDB(List<MaintenanceModel> maintenanceModel)
		{
			DBConnection();
			int i = 0;
			SqlConnection connection = null;
			SqlCommand command = null;
			try
			{
				if (maintenanceModel == null)
				{
					maintenanceModel = new List<MaintenanceModel>();
				}

				using (connection = new SqlConnection(con))
				{
					connection.Open();
					foreach (var item in maintenanceModel)
					{
						using (command = new SqlCommand("Insert_BinInformation", connection))
						{
							command.CommandType = CommandType.StoredProcedure;
							command.Parameters.AddWithValue("@MainID", item.MainNo);
							command.Parameters.AddWithValue("@DrawerID", item.DrawerNo ?? "");
                            command.Parameters.AddWithValue("@BinID", item.BinNo.ToUpper().Trim());
							command.Parameters.AddWithValue("@BinDescription", item.BinDescription?? "");
							i++;
							command.ExecuteNonQuery();
						}
					}
					return i;

				}
			}
			catch
			{
				i = 0;
				throw;
			}
			finally
			{
				connection.Close();
			}

		}

		public int InsertItemMasterInformationDB(MaintenanceModel maintenanceModel, string Responsible)
		{
			int i = 0;
            DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Insert_ItemMasterInformation", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@StockNo", maintenanceModel.StockNo.Trim());
						command.Parameters.AddWithValue("@ItemName", maintenanceModel.ItemName.Trim());
                        command.Parameters.AddWithValue("@ItemDescription", maintenanceModel.ItemDescription ?? "");
						command.Parameters.AddWithValue("@Specification", maintenanceModel.Specification.Trim());
                        command.Parameters.AddWithValue("@Details", maintenanceModel.Details ?? "");
						command.Parameters.AddWithValue("@StockOnHand", maintenanceModel.StockOnHand.Trim());
						command.Parameters.AddWithValue("@SafetyStock", maintenanceModel.SafetyStock.Trim());
						command.Parameters.AddWithValue("@UOM", maintenanceModel.Unit.Trim());
						command.Parameters.AddWithValue("@GroupID", maintenanceModel.Group);
						command.Parameters.AddWithValue("@DesignationID", maintenanceModel.Designation);
						command.Parameters.AddWithValue("@CriticalOrder", maintenanceModel.CriticalOrder.Trim());
						command.Parameters.AddWithValue("@TypeofItem", maintenanceModel.TypeID.Trim());
						command.Parameters.AddWithValue("@Responsible", Responsible);

						i++;
						command.ExecuteNonQuery();
						return i;
					}
				}
			}
			catch
			{
				i = 0;
				throw;
			}
			finally
			{
				connection.Close();
			}

		}

		public int InsertItemMasterLocationDB(List<MaintenanceModel> maintenanceModels, MaintenanceModel maintenance)
		{
			DBConnection();
			int i = 0;
			SqlConnection connection = null;
			SqlCommand command = null;

			try
			{
				if (maintenanceModels == null)
				{
					maintenanceModels = new List<MaintenanceModel>();
				}

				using (connection = new SqlConnection(con))
				{
					connection.Open();

					foreach (MaintenanceModel item in maintenanceModels)
					{
						using (command = new SqlCommand("Insert_ItemMasterGridInformation", connection))
						{
							command.CommandType = CommandType.StoredProcedure;

							command.Parameters.AddWithValue("@StockNo", maintenance.StockNo);
							command.Parameters.AddWithValue("@ItemName", maintenance.ItemName);
                            command.Parameters.AddWithValue("@MainNo", item.MainNo ?? "");
                            command.Parameters.AddWithValue("@BinNo", item.BinNo ?? "");
                            command.Parameters.AddWithValue("@DrawerNo", item.DrawerNo ?? "");

							i++;
							command.ExecuteNonQuery();
						}
					}
					return i;

				}
			}
			catch
			{
				i = 0;
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

		public MaintenanceModel PopulateSMTP()
		{

			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			MaintenanceModel model = null;
			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Select_SMTP", connection))
					{
						SqlDataAdapter sda = new SqlDataAdapter(command);
						DataTable dt = new DataTable();
						sda.Fill(dt);

						foreach (DataRow row in dt.Rows)
						{
							model = new MaintenanceModel()
							{
								smtpID = row["SMTP_ID"].ToString(),
								smtpPassword = row["SMTP_Password"].ToString(),
								smtpPort = row["SMTP_Port"].ToString(),
								smtpServer = row["SMTP_Server"].ToString(),
								smtpEmail = row["SMTP_Email"].ToString(),
								smtpStatus = row["SMTP_Status"].ToString(),
								smtpConfirmPassword = row["SMTP_Password"].ToString(),

							};
						}
						return model;
					}
				}
			}
			catch (Exception)
			{
			return model;
			}
			finally
			{
				connection.Close();
			}
		}

		public int Update_SMTPDB(MaintenanceModel model)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			int i = 0;
			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Insert_SMTP", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@SMTP_ID", model.smtpID ?? "");
						command.Parameters.AddWithValue("@SMTP_Server", model.smtpServer);
						command.Parameters.AddWithValue("@SMTP_Port", model.smtpPort);
						command.Parameters.AddWithValue("@SMTP_Email", model.smtpEmail);
						command.Parameters.AddWithValue("@SMTP_Password", model.smtpPassword);
						command.Parameters.AddWithValue("@SMTP_Status", model.smtpStatus);
						command.ExecuteNonQuery();
						i++;
					}
					return i;
				}

			}
			catch (Exception)
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

        public List<SelectListItem> LocationStatusListDB()
        {
            DBConnection();
            List<SelectListItem> StatusList = new List<SelectListItem>();
            using (SqlConnection COREConnection = new SqlConnection(con))
            {
                COREConnection.Open();
                using (SqlCommand CORECommand = new SqlCommand("Select_PopulateLocationStatus", COREConnection))
                {
                    CORECommand.CommandType = CommandType.StoredProcedure;


                    SqlDataAdapter sda = new SqlDataAdapter(CORECommand);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow items in dt.Rows)
                        {
                            StatusList.Add(new SelectListItem
                            {
                                Value = items["StatusID"].ToString(),
                                Text = items["StatusName"].ToString(),
                            });
                        }
                    }
                }
                COREConnection.Close();
                return StatusList;
            }
        }

        public List<SelectListItem> RetrieveLocationStatusDB(MaintenanceModel maintenance)
        {
            DBConnection();
            List<SelectListItem> StatusList = new List<SelectListItem>();
            using (SqlConnection COREConnection = new SqlConnection(con))
            {
                COREConnection.Open();
                using (SqlCommand CORECommand = new SqlCommand("Select_PopulateRetrieveLocationStatus", COREConnection))
                {
                    CORECommand.CommandType = CommandType.StoredProcedure;
                    CORECommand.Parameters.AddWithValue("@MainNo", maintenance.MainNo ?? "");
                    CORECommand.Parameters.AddWithValue("@DrawerNo", maintenance.DrawerNo ?? "");
                    CORECommand.Parameters.AddWithValue("@BinNo", maintenance.BinNo ?? "");

                    SqlDataAdapter sda = new SqlDataAdapter(CORECommand);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow items in dt.Rows)
                        {
                            StatusList.Add(new SelectListItem
                            {
                                Value = items["StatusID"].ToString(),
                                Text = items["StatusName"].ToString(),
                            });
                        }
                    }
                }
                COREConnection.Close();
                return StatusList;
            }
        }

        public int UpdateLocation(MaintenanceModel maintenance)
        {
            DBConnection();
            int i = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(con))
                {
                    connection.Open();

                    using (command = new SqlCommand("Update_ItemLocation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@MainNo", maintenance.MainNo);
                        command.Parameters.AddWithValue("@DrawerNo", maintenance.DrawerNo ?? "");
                        command.Parameters.AddWithValue("@BinNo", maintenance.BinNo ?? "");
                        command.Parameters.AddWithValue("@ChangeNo", maintenance.ChangeNo.ToUpper().Trim() ?? "");
                        command.Parameters.AddWithValue("@MainName", maintenance.RackDescription ?? "");

                        i++;
                        command.ExecuteNonQuery();
                    }

                    return i;

                }
            }
            catch
            {
                i = 0;
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public int UpdateLocationStatus(MaintenanceModel maintenance)
        {
            DBConnection();
            int i = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(con))
                {
                    connection.Open();

                    using (command = new SqlCommand("Update_ItemLocationStatus", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@MainNo", maintenance.MainNo);
                        command.Parameters.AddWithValue("@DrawerNo", maintenance.DrawerNo ?? "");
                        command.Parameters.AddWithValue("@BinNo", maintenance.BinNo ?? "");
                        command.Parameters.AddWithValue("@Status", maintenance.LocationStatus ?? "");

                        i++;
                        command.ExecuteNonQuery();
                    }
                    return i;

                }
            }
            catch
            {
                i = 0;
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<MaintenanceModel> PopulateAllItemLocationDB()
        {
            List<MaintenanceModel> ItemLocation = new List<MaintenanceModel>();
            SqlConnection connection = null;
            SqlCommand command = null;
            DBConnection();
            try
            {
                using (connection = new SqlConnection(con))
                {
                    using (command = new SqlCommand("Check_PopulateAllItemLocation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                        {
                            using (DataTable table = new DataTable())
                            {
                                dataAdapter.Fill(table);
                                foreach (DataRow row in table.Rows)
                                {
                                    MaintenanceModel model = new MaintenanceModel
                                    {
                                        RackNo = row["RackNo"].ToString(),
                                        DrawerNo = row["DrawerID"].ToString(),
                                        BinNo = row["BinID"].ToString(),
                                        RackStatus = row["RackStatus"].ToString(),
                                        DrawerStatus = row["DrawerStatus"].ToString(),
                                        BinStatus = row["BinStatus"].ToString(),
                                    };
                                    ItemLocation.Add(model);
                                }
                                return ItemLocation;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<MaintenanceModel> PopulateSelectedDrawerList(MaintenanceModel maintenance)
        {
            List<MaintenanceModel> DrawerList = new List<MaintenanceModel>();
            SqlConnection connection = null;
            SqlCommand command = null;
            DBConnection();
            try
            {
                using (connection = new SqlConnection(con))
                {
                    using (command = new SqlCommand("Select_GetDrawerList", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@EditVal", maintenance.MainNo ?? "");
                        using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                        {
                            using (DataTable table = new DataTable())
                            {
                                dataAdapter.Fill(table);
                                foreach (DataRow row in table.Rows)
                                {
                                    MaintenanceModel model = new MaintenanceModel
                                    {
                                        LocationStatus = "",
                                        DrawerNo = row["DrawerID"].ToString(),
                                        DrawerStatus = row["DrawerStatus"].ToString(),
                                    };
                                    DrawerList.Add(model);
                                }
                                return DrawerList;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public List<MaintenanceModel> PopulateSelectedBinList(MaintenanceModel maintenance)
        {
            List<MaintenanceModel> BinList = new List<MaintenanceModel>();
            SqlConnection connection = null;
            SqlCommand command = null;
            DBConnection();
            try
            {
                using (connection = new SqlConnection(con))
                {
                    using (command = new SqlCommand("Select_GetBinList", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@EditVal", maintenance.MainNo ?? "");
                        command.Parameters.AddWithValue("@Edit", maintenance.DrawerNo ?? "");
                        using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
                        {
                            using (DataTable table = new DataTable())
                            {
                                dataAdapter.Fill(table);
                                foreach (DataRow row in table.Rows)
                                {
                                    MaintenanceModel model = new MaintenanceModel
                                    {
                                        LocationStatus = "",
                                        BinNo = row["BinID"].ToString(),
                                        BinStatus = row["BinStatus"].ToString(),
                                    };
                                    BinList.Add(model);
                                }
                                return BinList;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public int ItemLocationHasValueDB(MaintenanceModel maintenance)
        {
            DBConnection();
            int i = 0;
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(con))
                {
                    connection.Open();

                    using (command = new SqlCommand("Select_PopulateAllItemInformation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@RackNo", maintenance.MainNo);
                        command.Parameters.AddWithValue("@DrawerNo", maintenance.DrawerNo ?? "");
                        command.Parameters.AddWithValue("@BinNo", maintenance.BinNo ?? "");

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        i = dt.Rows.Count;

                        //command.ExecuteNonQuery();
                    }
                    return i;

                }
            }
            catch
            {
                i = 0;
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<MaintenanceModel> PopulateAllMainInformationDB()
        {
            DBConnection();
            List<MaintenanceModel> listMain = new List<MaintenanceModel>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (command = new SqlCommand("Table_PopulateAllRackInformation", connection))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    MaintenanceModel Main = new MaintenanceModel()
                                    {
                                        RackNo = row["RackNo"].ToString(),
                                        RackDescription = row["RackName"].ToString(),
                                        LocationStatus = row["RackStatus"].ToString(),
                                    };
                                    listMain.Add(Main);
                                }
                                return listMain;
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SelectListItem> SelectAllMainItemlocationStatusDB()
        {
            DBConnection();
            List<SelectListItem> listMain = new List<SelectListItem>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (command = new SqlCommand("Select_PopulateMainItemLocationStatus", connection))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    listMain.Add(new SelectListItem
                                    {
                                        Text = row["RackName"].ToString(),
                                        Value = row["RackNo"].ToString(),
                                    });
                                }
                                return listMain;
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public List<SelectListItem> SelectBinItemlocationStatusDB(string DrawerNo, string MainNo)
        {
            DBConnection();
            List<SelectListItem> BinNoList = new List<SelectListItem>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(con))
                {
                    using (command = new SqlCommand("Select_PopulateBinItemLocationStatus", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@DrawerNo", DrawerNo ?? "");
                        command.Parameters.AddWithValue("@MainNo", MainNo ?? "");
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    BinNoList.Add(new SelectListItem
                                    {
                                        Text = row["BinName"].ToString(),
                                        Value = row["BinID"].ToString()
                                    });
                                }
                            }
                        }
                        return BinNoList;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

		public List<SelectListItem> SelectBinItemlocationDB(string DrawerNo,string MainNo)
		{
			DBConnection();
			List<SelectListItem> BinNoList = new List<SelectListItem>();
			SqlConnection connection = null;
			SqlCommand command = null;

			try
			{
				using (connection = new SqlConnection(con))
				{
					using (command = new SqlCommand("Select_PopulateBinItemLocation", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@DrawerNo", DrawerNo ?? "");
                        command.Parameters.AddWithValue("@MainNo", MainNo ?? "");
						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									BinNoList.Add(new SelectListItem
									{
										Text = row["BinName"].ToString(),
										Value = row["BinID"].ToString()
									});
								}
							}
						}
						return BinNoList;
					}
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

		public List<SelectListItem> SelectDrawerItemlocationDB(string MainNo)
		{
			DBConnection();
			List<SelectListItem> BinNoList = new List<SelectListItem>();
			SqlConnection connection = null;
			SqlCommand command = null;

			try
			{
				using (connection = new SqlConnection(con))
				{
					using (command = new SqlCommand("Select_PopulateDrawerItemLocation", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@MainNo", MainNo ?? "");
						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									BinNoList.Add(new SelectListItem
									{
										Text = row["DrawerName"].ToString(),
										Value = row["DrawerID"].ToString()
									});
								}
							}
						}
						return BinNoList;
					}
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

        public List<SelectListItem> SelectDrawerItemlocationStatusDB(string MainNo)
        {
            DBConnection();
            List<SelectListItem> BinNoList = new List<SelectListItem>();
            SqlConnection connection = null;
            SqlCommand command = null;

            try
            {
                using (connection = new SqlConnection(con))
                {
                    using (command = new SqlCommand("Select_PopulateDrawerItemLocationStatus", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@MainNo", MainNo ?? "");
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    BinNoList.Add(new SelectListItem
                                    {
                                        Text = row["DrawerName"].ToString(),
                                        Value = row["DrawerID"].ToString()
                                    });
                                }
                            }
                        }
                        return BinNoList;
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public string CheckUserNameExistsDB(string UserNmae)
        {
            DBConnection();
            SqlConnection connection = null;
            SqlCommand command = null;
            string UserNames = "";
            try
            {
                using (connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (command = new SqlCommand("Check_UserifExists", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Username", UserNmae);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                using (SqlDataReader sdr = command.ExecuteReader())
                                {
                                    while (sdr.Read())
                                    {
                                        UserNames = sdr["Username"].ToString();
                                    }
                                    return UserNames;
                                }

                            }
                        }

                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

		public List<SelectListItem> SelectNoDrawerItemlocationDB(string MainNo)
		{
			DBConnection();
			List<SelectListItem> BinNoList = new List<SelectListItem>();
			SqlConnection connection = null;
			SqlCommand command = null;

			try
			{
				using (connection = new SqlConnection(con))
				{
					using (command = new SqlCommand("Select_PopulateNoDrawerItemLocation", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@MainID", MainNo ?? "");
						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									BinNoList.Add(new SelectListItem
									{
										Text = row["BinName"].ToString(),
										Value = row["BinID"].ToString()
									});
								}
							}
						}
						return BinNoList;
					}
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

		public List<SelectListItem> SelectAllUserGroupDB()
		{
			DBConnection();
			List<SelectListItem> ListUserGroup = new List<SelectListItem>();
			SqlConnection connection = null;
			SqlCommand command = null;

			try
			{
				using (connection = new SqlConnection(con))
				{
					using (command = new SqlCommand("Select_PopulateAllUserGroup", connection))
					{
						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									ListUserGroup.Add(new SelectListItem
									{
										Text = row["GroupName"].ToString(),
										Value = row["GroupID"].ToString()
									});
								}
								return ListUserGroup;
							}
						}
					}
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

		public List<SelectListItem> SelectUserDesignationDB(string Group)
		{
			DBConnection();
			List<SelectListItem> DesignationList = new List<SelectListItem>();
			SqlConnection connection = null;
			SqlCommand command = null;

			try
			{
				using (connection = new SqlConnection(con))
				{
					using (command = new SqlCommand("Select_PopulateUserDesignation", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@GroupID", Group);
						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									DesignationList.Add(new SelectListItem
									{
										Text = row["Designation"].ToString(),
										Value = row["DesignationID"].ToString()
									});
								}
							}
						}
						return DesignationList;
					}
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				connection.Close();
			}

		}

		public List<MaintenanceModel> PopulateDashboardItemInformationDB(MaintenanceModel maintenance, string Group, string Designation)
		{
			List<MaintenanceModel> listdt = new List<MaintenanceModel>();
			SqlConnection connection = null;
			SqlCommand command = null;
			DBConnection();
			try
			{
				using (connection = new SqlConnection(con))
				{
					using (command = new SqlCommand("Table_PopulateDashboardItemInformation", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						if (maintenance != null)
						{
							command.Parameters.AddWithValue("@SelectedDesignationID", maintenance.DesignationID ?? "");
						}
						else
						{
							command.Parameters.AddWithValue("@SelectedDesignationID", "");
						}
						command.Parameters.AddWithValue("@GroupID", Group);
						command.Parameters.AddWithValue("@DesignationID", Designation);
						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									MaintenanceModel main = new MaintenanceModel()
									{
										StockNo = row["StockNo"].ToString(),
										ItemName = row["ItemName"].ToString(),
										StockOnHand = row["StockOnHand"].ToString(),
										SafetyStock = row["SafetyStock"].ToString(),
										Specification = row["Specification"].ToString(),
										Group = row["GroupName"].ToString(),
										TypeID = row["TypeName"].ToString(),
										Designation = row["Designation"].ToString(),
										CriticalOrder = row["CriticalOrder"].ToString(),

									};
									listdt.Add(main);
								}
								return listdt;
							}
						}
					}
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

		public List<MaintenanceModel> PopulateItemInformationDB(string Group, string Designation)
		{
            var name = "";
			List<MaintenanceModel> listdt = new List<MaintenanceModel>();
			SqlConnection connection = null;
			SqlCommand command = null;
			DBConnection();
			try
			{
				using (connection = new SqlConnection(con))
				{
					using (command = new SqlCommand("Table_PopulateItemInformation", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@GroupID", Group);
						command.Parameters.AddWithValue("@DesignationID", Designation);
						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
                                    if (row["Status"].ToString() == "InActive")
                                    {
                                        name = "DEACTIVATED";
                                    }
                                    else
                                    {
                                        name = "ACTIVE";
                                    }
									MaintenanceModel main = new MaintenanceModel()
									{
										StockNo = row["StockNo"].ToString(),
										ItemName = row["ItemName"].ToString(),
										StockOnHand = row["StockOnHand"].ToString(),
										SafetyStock = row["SafetyStock"].ToString(),
										Specification = row["Specification"].ToString(),
										Group = row["GroupName"].ToString(),
										TypeID = row["TypeName"].ToString(),
										Designation = row["Designation"].ToString(),
										CriticalOrder = row["CriticalOrder"].ToString(),
                                        Status = row["ItemStatus"].ToString(),
                                        StatusName = name,
									};
									listdt.Add(main);
								}
								return listdt;
							}
						}
					}
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

		public List<MaintenanceModel> PopulateItemMonitoringDB(MaintenanceModel model, string Group, string Designation)
		{
			List<MaintenanceModel> listdt = new List<MaintenanceModel>();
			SqlConnection connection = null;
			SqlCommand command = null;
			DBConnection();
			try
			{
				using (connection = new SqlConnection(con))
				{
					using (command = new SqlCommand("Table_PopulateItemMonitoring", connection))
					{
						command.CommandType = CommandType.StoredProcedure;


						if (Group == "2")
						{
							command.Parameters.AddWithValue("@SelectedDesignation", Designation ?? "");

						}
						else
						{
							command.Parameters.AddWithValue("@SelectedDesignation", model.DesignationID ?? "");

						}
						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									MaintenanceModel main = new MaintenanceModel()
									{
										StockNo = row["StockNo"].ToString(),
										ItemName = row["ItemName"].ToString(),
										StockOnHand = row["StockOnHand"].ToString(),
										SafetyStock = row["SafetyStock"].ToString(),
										Group = row["GroupName"].ToString(),
										TypeID = row["TypeName"].ToString(),
										Specification = row["Specification"].ToString(),
										CriticalOrder = row["CriticalOrder"].ToString(),
										Designation = row["Designation"].ToString(),
                                        PRStatus = row["Status"].ToString(),
                                        DesignationID =row["DesignationID"].ToString(),
									};
									listdt.Add(main);
								}
								return listdt;
							}
						}
					}
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

		public List<MaintenanceModel> PopulatePRItemMonitoringDB(string Date, string Week)
		{
			List<MaintenanceModel> PRItemList = new List<MaintenanceModel>();
			SqlConnection connection = null;
			SqlCommand command = null;
			DBConnection();
			try
			{
				using (connection = new SqlConnection(con))
				{
					using (command = new SqlCommand("Table_PopulatePRItemMonitoring", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@From", Week);
                        command.Parameters.AddWithValue("@To", Date);
						using (SqlDataAdapter dataAdapter = new SqlDataAdapter(command))
						{
							using (DataTable table = new DataTable())
							{
								dataAdapter.Fill(table);
								foreach (DataRow row in table.Rows)
								{
									MaintenanceModel model = new MaintenanceModel
									{
										ReferenceNo = row["ReferenceNo"].ToString(),
										PRNo = row["PRNo"].ToString(),
										Responsible = row["Responsible"].ToString(),
										DateRequested = row["DateRequested"].ToString(),
										RequestStatus = row["RequestStatus"].ToString(),
										DateApprovedPR = row["DateApprovedPR"].ToString()
									};
									PRItemList.Add(model);
								}
								return PRItemList;
							}
						}
					}
				}
			}
			catch (Exception)
			{

				throw;
			}
		}

		public List<SelectListItem> SelectAllMainItemlocationDB()
		{
			DBConnection();
			List<SelectListItem> listMain = new List<SelectListItem>();
			SqlConnection connection = null;
			SqlCommand command = null;

			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Select_PopulateMainItemLocation", connection))
					{
						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									listMain.Add(new SelectListItem
									{
										Text = row["RackName"].ToString(),
										Value = row["RackNo"].ToString(),
									});
								}
								return listMain;
							}
						}
					}
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

		public MaintenanceModel Edit_PopulateItemInformationDB(string StockNo)
		{
			DBConnection();
			MaintenanceModel maintenance = new MaintenanceModel();
			SqlConnection connection = null;
			SqlCommand command = null;

			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Edit_PopulateItemInformation", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@StockNo", StockNo);
						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								SqlDataReader sdr = command.ExecuteReader();

								while (sdr.Read())
								{
									maintenance = new MaintenanceModel()
									{

										OldStockNo = sdr["StockNo"].ToString(),
										StockNo = sdr["StockNo"].ToString(),
										ItemDescription = sdr["ItemDescription"].ToString(),
										ItemName = sdr["ItemName"].ToString(),
										Specification = sdr["Specification"].ToString(),
										Unit = sdr["UOM"].ToString(),
										Details = sdr["Details"].ToString(),
										StockOnHand = sdr["StockOnHand"].ToString(),
										SafetyStock = sdr["SafetyStock"].ToString(),
										Group = sdr["GroupID"].ToString(),
										Designation = sdr["DesignationID"].ToString(),
										TypeID = sdr["TypeofItem"].ToString(),
										CriticalOrder = sdr["CriticalOrder"].ToString(),
                                        Status = sdr["ItemStatus"].ToString(),
                                        Pullout = sdr["PulloutStatus"].ToString(),
                                        HiddenPullout = sdr["PulloutStatus"].ToString(),
									};
								}
                                sdr.Close();
                                using (SqlCommand command1 = new SqlCommand("Form_CheckIfAlreadyTransac", connection))
                                {
                                    command1.CommandType = CommandType.StoredProcedure;
                                    command1.Parameters.AddWithValue("@StockNo", StockNo);
                                    using (SqlDataAdapter sda1 = new SqlDataAdapter(command1))
                                    {
                                        using (DataTable dt1 = new DataTable())
                                        {
                                            sda1.Fill(dt1);

                                            if (dt1.Rows.Count > 0)
                                            {
                                                maintenance.CheckIfAlreadyTransac = "NotExist";
                                            }
                                            else
                                            {
                                                maintenance.CheckIfAlreadyTransac = "Exist";
                                            }
                                        }
                                    }
                                }
                                return maintenance;
							}
                        } 
					}
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

		public List<MaintenanceModel> PopulateItemLocationDB(string StockNo)
		{
			DBConnection();
			List<MaintenanceModel> listItemLocation = new List<MaintenanceModel>();
			SqlConnection connection = null;
			SqlCommand command = null;

			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Edit_PopulateItemLocation", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@StockNo", StockNo);

						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									MaintenanceModel main = new MaintenanceModel()
									{
										MainNo = row["MainNo"].ToString(),
										BinNo = row["BinNo"].ToString(),
										HiddenBin = row["BinNo"].ToString(),
										DrawerNo = row["DrawerNo"].ToString(),
										HiddenDrawer = row["DrawerNo"].ToString(),
									};
									listItemLocation.Add(main);
								}
								return listItemLocation;
							}
						}
					}
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

		public List<MaintenanceModel> PopulateUserListDB()
		{
			DBConnection();
			List<MaintenanceModel> PopulateUserList = new List<MaintenanceModel>();

			using (SqlConnection COREConnection = new SqlConnection(con))
			{
				using (SqlCommand CORECommand = new SqlCommand("Table_PopulateUserList", COREConnection))
				{
					SqlDataAdapter sda = new SqlDataAdapter(CORECommand);
					DataTable dt = new DataTable();
					sda.Fill(dt);

					if (dt.Rows.Count > 0)
					{
						foreach (DataRow items in dt.Rows)
						{
							MaintenanceModel UserModel = new MaintenanceModel
							{
								UserID = items["UserID"].ToString(),
								CompanyID = items["CompanyID"].ToString(),
								Fullname = items["Fullname"].ToString(),
								Username = items["Username"].ToString(),
								Password = items["Password"].ToString(),
								EmailAddress = items["EmailAddress"].ToString(),
								Status = items["Status"].ToString(),
								GroupID = items["GroupName"].ToString(),
								RoleID = items["RoleName"].ToString(),
								DepartmentID = items["Department"].ToString(),
								DesignationID = items["Designation"].ToString(),
							};
							PopulateUserList.Add(UserModel);
						}
					}
				}
			}
			return PopulateUserList;
		}

		public List<SelectListItem> StatusListDB()
		{
			DBConnection();
			List<SelectListItem> StatusList = new List<SelectListItem>();
			using (SqlConnection COREConnection = new SqlConnection(con))
			{
				COREConnection.Open();
				using (SqlCommand CORECommand = new SqlCommand("Select_PopulateUserStatus", COREConnection))
				{
					CORECommand.CommandType = CommandType.StoredProcedure;
					SqlDataAdapter sda = new SqlDataAdapter(CORECommand);
					DataTable dt = new DataTable();
					sda.Fill(dt);

					if (dt.Rows.Count > 0)
					{
						foreach (DataRow items in dt.Rows)
						{
							StatusList.Add(new SelectListItem
							{
								Value = items["StatusID"].ToString(),
								Text = items["Status"].ToString(),
							});
						}
					}
				}
				COREConnection.Close();
				return StatusList;
			}
		}

		public List<SelectListItem> DepartmentListDB()
		{
			DBConnection();
			List<SelectListItem> DepartmentList = new List<SelectListItem>();
			using (SqlConnection COREConnection = new SqlConnection(con))
			{
				COREConnection.Open();
				using (SqlCommand CORECommand = new SqlCommand("Select_PopulateUserDepartment", COREConnection))
				{
					CORECommand.CommandType = CommandType.StoredProcedure;
					SqlDataAdapter sda = new SqlDataAdapter(CORECommand);
					DataTable dt = new DataTable();
					sda.Fill(dt);

					if (dt.Rows.Count > 0)
					{
						foreach (DataRow items in dt.Rows)
						{
							DepartmentList.Add(new SelectListItem
							{
								Value = items["DeptID"].ToString(),
								Text = items["Department"].ToString()
							});
						}
					}
				}
				COREConnection.Close();
				return DepartmentList;
			}
		}

		public List<SelectListItem> UserRoleListDB()
		{
			DBConnection();
			List<SelectListItem> UserRoleList = new List<SelectListItem>();
			using (SqlConnection COREConnection = new SqlConnection(con))
			{
				COREConnection.Open();
				using (SqlCommand CORECommand = new SqlCommand("Select_PopulateUserRole", COREConnection))
				{
					CORECommand.CommandType = CommandType.StoredProcedure;
					SqlDataAdapter sda = new SqlDataAdapter(CORECommand);
					DataTable dt = new DataTable();
					sda.Fill(dt);

					if (dt.Rows.Count > 0)
					{
						foreach (DataRow items in dt.Rows)
						{
							UserRoleList.Add(new SelectListItem
							{
								Value = items["RoleID"].ToString(),
								Text = items["RoleName"].ToString()
							});
						}
					}
				}
				COREConnection.Close();
				return UserRoleList;
			}
		}

		public MaintenanceModel PopulateUserInformationDB(string id)
		{
			DBConnection();
			MaintenanceModel UserModel = null;
			using (SqlConnection COREConnection = new SqlConnection(con))
			{
				using (SqlCommand CORECommand = new SqlCommand("Form_PopulateUserInformation", COREConnection))
				{
					CORECommand.CommandType = CommandType.StoredProcedure;
					CORECommand.Parameters.AddWithValue("@UserID", id);
					SqlDataAdapter sda = new SqlDataAdapter(CORECommand);
					DataTable dt = new DataTable();
					sda.Fill(dt);

					if (dt.Rows.Count > 0)
					{
						foreach (DataRow items in dt.Rows)
						{
							UserModel = new MaintenanceModel
							{
								UserID = items["UserID"].ToString(),
								CompanyID = items["CompanyID"].ToString(),
								Fullname = items["Fullname"].ToString(),
								Username = items["Username"].ToString(),
								Password = items["Password"].ToString(),
								EmailAddress = items["EmailAddress"].ToString(),
								GroupID = items["GroupID"].ToString(),
								RoleID = items["RoleID"].ToString(),
								DepartmentID = items["Department"].ToString(),
                                Department = items["DepartmentID"].ToString(),
								DesignationID = items["DesignationID"].ToString(),
                                Group = items["GroupName"].ToString(),
                                Designation = items["Designation"].ToString(),
                                RoleName = items["RoleName"].ToString(),
                                ResetPassword = Convert.ToInt32(items["ResetPassword"].ToString()),
                                Status = items["Status"].ToString(),
                                Code = items["ResetCode"].ToString(),
                                OldPassword = items["OldPassword"].ToString()
							};
						}
					}
				}
			}
			return UserModel;
		}

		public int InventoryInsertUserDB(MaintenanceModel maintenance)
		{
			DBConnection();
			int result = 0;
			try
			{
				using (SqlConnection COREConnection = new SqlConnection(con))
				{
					using (SqlCommand CORECommand = new SqlCommand("Insert_UserInformation", COREConnection))
					{
						COREConnection.Open();
						CORECommand.CommandType = CommandType.StoredProcedure;
						CORECommand.Parameters.AddWithValue("@EntryDate", DateTime.Now.ToShortDateString());
						CORECommand.Parameters.AddWithValue("@CompanyID", maintenance.CompanyID);
						CORECommand.Parameters.AddWithValue("@Fullname", maintenance.Fullname);
						CORECommand.Parameters.AddWithValue("@Username", maintenance.Username);
						CORECommand.Parameters.AddWithValue("@EmailAddress", maintenance.EmailAddress);
						CORECommand.Parameters.AddWithValue("@GroupID", maintenance.GroupID);
						CORECommand.Parameters.AddWithValue("@RoleID", maintenance.RoleID);
						CORECommand.Parameters.AddWithValue("@DepartmentID", maintenance.DepartmentID);
						CORECommand.Parameters.AddWithValue("@DesignationID", maintenance.DesignationID);
						CORECommand.ExecuteNonQuery();
						result++;
						COREConnection.Close();
						return result;
					}
				}
			}
			catch (Exception)
			{
				result = 0;
				return result;
			}
		}

		public int InventoryUpdateUserDB(MaintenanceModel maintenance)
		{
			DBConnection();
			int result = 0;
			SqlConnection COREConnection = null;

			using (COREConnection = new SqlConnection(con))
			{
				COREConnection.Open();
				using (SqlCommand CORECommand = new SqlCommand("Update_UserInformation", COREConnection))
				{
					CORECommand.CommandType = CommandType.StoredProcedure;
					CORECommand.Parameters.AddWithValue("@UserID", maintenance.UserID);
					CORECommand.Parameters.AddWithValue("@CompanyID", maintenance.CompanyID);
					CORECommand.Parameters.AddWithValue("@Fullname", maintenance.Fullname);
					CORECommand.Parameters.AddWithValue("@Username", maintenance.Username);
					CORECommand.Parameters.AddWithValue("@EmailAddress", maintenance.EmailAddress);
					CORECommand.Parameters.AddWithValue("@GroupID", maintenance.GroupID);
					CORECommand.Parameters.AddWithValue("@RoleID", maintenance.RoleID);
					CORECommand.Parameters.AddWithValue("@DepartmentID", maintenance.DepartmentID);
					CORECommand.Parameters.AddWithValue("@DesignationID", maintenance.DesignationID);

					CORECommand.ExecuteNonQuery();
					result++;
					COREConnection.Close();
					return result;
				}
			}
		}

		public string InventoryUserResetPasswordDB(MaintenanceModel maintenance, string UserID)
		{
			DBConnection();
            string result = "";
			SqlConnection COREConnection = null;

			using (COREConnection = new SqlConnection(con))
			{
				using (SqlCommand CORECommand = new SqlCommand("Update_UserResetPassword", COREConnection))
				{
					CORECommand.CommandType = CommandType.StoredProcedure;
					CORECommand.Parameters.AddWithValue("@UserID", maintenance.UserID);
                    CORECommand.Parameters.AddWithValue("@Code", maintenance.Code);
                    CORECommand.Parameters.AddWithValue("@Responsible", UserID);

					COREConnection.Open();
					CORECommand.ExecuteNonQuery();

                    result = maintenance.Code;
					COREConnection.Close();
					return result;
				}
			}
		}

		public int InventoryUpdateUserStatusDB(MaintenanceModel maintenance)
		{
			DBConnection();
			int result = 0;
			SqlConnection COREConnection = null;

			using (COREConnection = new SqlConnection(con))
			{
				using (SqlCommand CORECommand = new SqlCommand("Update_UserStatus", COREConnection))
				{
					CORECommand.CommandType = CommandType.StoredProcedure;
					CORECommand.Parameters.AddWithValue("@CompanyID", maintenance.CompanyID);
					CORECommand.Parameters.AddWithValue("@Status", maintenance.Status);

					COREConnection.Open();
					CORECommand.ExecuteNonQuery();
					result++;
					COREConnection.Close();
					return result;
				}
			}
		}

		public MaintenanceModel UserAuthentication(MaintenanceModel maintenance)
		{
			DBConnection();
			using (SqlConnection COREConnection = new SqlConnection(con))
			{
				MaintenanceModel userInfo = null;
				COREConnection.Open();
				using (SqlCommand CORECommand = new SqlCommand("Form_PopulateLoginInformation", COREConnection))
				{
					CORECommand.CommandType = CommandType.StoredProcedure;
					CORECommand.Parameters.AddWithValue("@Username", maintenance.Username);
					CORECommand.Parameters.AddWithValue("@Password", maintenance.Password);
					using (SqlDataAdapter COREDataAdapter = new SqlDataAdapter(CORECommand))
					{
						using (DataTable COREDataTable = new DataTable())
						{
							COREDataAdapter.Fill(COREDataTable);

							if (COREDataTable.Rows.Count > 0)
							{
								foreach (DataRow item in COREDataTable.Rows)
								{
									userInfo = new MaintenanceModel
									{
										UserID = item["UserID"].ToString(),
										Username = item["Username"].ToString(),
										Password = item["OldPassword"].ToString(),
										CompanyID = item["CompanyID"].ToString(),
										Fullname = item["Fullname"].ToString(),
										RoleID = item["RoleID"].ToString(),
										DepartmentID = item["Department"].ToString(),
										Group = item["GroupName"].ToString(),
										Designation = item["Designation"].ToString(),
										DesignationID = item["DesignationID"].ToString(),
										GroupID = item["GroupID"].ToString(),
										RoleName = item["RoleName"].ToString(),
										ResetPassword = Convert.ToInt32(item["ResetPassword"].ToString()),
										Status = item["Status"].ToString(),
                                        Code = item["LoginCode"].ToString(),
									};
								}

							}
						}
					}
					COREConnection.Close();
				}
				return userInfo;

			}
		}

		public List<SelectListItem> PopulateTypeOfItem()
		{
			DBConnection();
			List<SelectListItem> listType = new List<SelectListItem>();
			SqlConnection connection = null;
			SqlCommand command = null;

			try
			{

				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Select_PopulateTypeOfItem", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									listType.Add(new SelectListItem
									{

										Text = row["TypeName"].ToString(),
										Value = row["TypeID"].ToString()
									});

								}
								return listType;
							}
						}
					}
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

		public int ResetPassword(MaintenanceModel LoginModel)
		{
			DBConnection();
			int result = 0;
			SqlConnection COREConnection = null;

			using (COREConnection = new SqlConnection(con))
			{
				using (SqlCommand CORECommand = new SqlCommand("Update_LoginResetPassword", COREConnection))
				{
					CORECommand.CommandType = CommandType.StoredProcedure;
					CORECommand.Parameters.AddWithValue("@UserID", LoginModel.UserID);
					CORECommand.Parameters.AddWithValue("@NewPassword", LoginModel.NewPassword);

					COREConnection.Open();
					CORECommand.ExecuteNonQuery();
					result++;
					COREConnection.Close();
					return result;
				}
			}
		}

		public List<SelectListItem> PopulateRequestAllitemStockNo(string Type, string Group, string Designation)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			List<SelectListItem> ListStockNo = new List<SelectListItem>();
			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();

					using (command = new SqlCommand("Select_PopulateAllItemStockNo", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@Type", Type ?? "");
						command.Parameters.AddWithValue("@GroupID", Group ?? "");
						command.Parameters.AddWithValue("@DesignationID", Designation ?? "");

						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									ListStockNo.Add(new SelectListItem
									{
										Text = row["StockNo"].ToString(),
										Value = row["StockNo"].ToString(),
									});
								}
								return ListStockNo;
							}
						}
					}
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

		//search
		public List<SelectListItem> PopulateRequestItemDescription(string Type, string Group, string Designation)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			List<SelectListItem> ListStockNo = new List<SelectListItem>();
			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();

					using (command = new SqlCommand("Select_PopulateAllItemStockNo", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@Type", Type ?? "");
						command.Parameters.AddWithValue("@GroupID", Group ?? "");
						command.Parameters.AddWithValue("@DesignationID", Designation ?? "");

						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									ListStockNo.Add(new SelectListItem
									{
										Value = row["StockNo"].ToString(),
                                        Text = string.Format("{0}|{1}|{2}", row["StockNo"].ToString(), row["ItemName"].ToString(), row["Specification"].ToString())
									});
								}
								return ListStockNo;
							}
						}
					}
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

		public List<SelectListItem> PopulateAllApprover(string Group, string Designation)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			List<SelectListItem> ListStockNo = new List<SelectListItem>();
			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();

					using (command = new SqlCommand("Table_PopulateApproverList", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@GroupID", Group ?? "");
						command.Parameters.AddWithValue("@DesignationID", Designation ?? "");

						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								if (dt.Rows.Count > 0)
								{
									foreach (DataRow row in dt.Rows)
									{
										ListStockNo.Add(new SelectListItem
										{
											Text = row["FullName"].ToString(),
											Value = row["UserID"].ToString(),
										});
									}
								}

								return ListStockNo;
							}
						}
					}
				}
			}
			catch
			{
				return ListStockNo;
			}
			finally
			{
				connection.Close();
			}
		}

		public void CheckStockInventoryDB(string StockNo)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;

			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Check_StockItemInventory", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@StockNo", StockNo);

						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								using (SqlDataReader sdr = command.ExecuteReader())
								{
									while (sdr.Read())
									{
										StockonHand = sdr["StockOnHand"].ToString();
										SafetyStock = sdr["SafetyStock"].ToString();
									}
								}

							}
						}

					}
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

        public int CheckItemCodeIfExist(MaintenanceModel maintenance)
        {
            DBConnection();
            SqlConnection connection = null;
            SqlCommand command = null;

            int i = 0;
            try
            {

                using (connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (command = new SqlCommand("Check_ItemIfExits", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@StockNo", maintenance.StockNo);
                        command.Parameters.AddWithValue("@PartName", maintenance.ItemName);
                        command.Parameters.AddWithValue("@Specification", maintenance.Specification);
                        command.Parameters.AddWithValue("@Description", maintenance.ItemDescription ?? "");

                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        SqlDataReader sdr = command.ExecuteReader();
                        while (sdr.Read())
                        {
                            var x = sdr["StockNo"].ToString();
                            i++;
                        }
                        return i;
                    }
                }

            }
            catch
            {
                i = 0;
                throw;  
            }
            finally
            {
                connection.Close();

            }
        }

        public List<MaintenanceModel> ItemDescriptionList()
        {
            DBConnection();
            List<MaintenanceModel> List = new List<MaintenanceModel>();
            using (SqlConnection COREConnection = new SqlConnection(con))
            {
                using (SqlCommand CORECommand = new SqlCommand("Check_ItemDescriptionIfExists", COREConnection))
                {
                    SqlDataAdapter sda = new SqlDataAdapter(CORECommand);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow items in dt.Rows)
                        {
                            MaintenanceModel UserModel = new MaintenanceModel
                            {
                                Specification = items["Specification"].ToString(),
                                ItemDescription = items["ItemDescription"].ToString(),
                                StockNo = items["StockNo"].ToString(),
                            };
                            List.Add(UserModel);
                        }
                    }
                }
            }

            return List;
        }


		#region -------------------------------------------------------------------------RoleAccess-------------------------------------------------------------------------
		public RoleAccessModel RoleAccess(string RoleID)
		{
			int less = 0;
			DBConnection();
			RoleAccessModel RoleModel = new RoleAccessModel();

			using (SqlConnection COREConnection = new SqlConnection(con))
			{
				using (SqlCommand CORECommand = new SqlCommand("Form_PopulateRoleAccess", COREConnection))
				{
					CORECommand.CommandType = CommandType.StoredProcedure;
					CORECommand.Parameters.AddWithValue("@RoleID", RoleID);
					using (SqlDataAdapter COREDataAdapter = new SqlDataAdapter(CORECommand))
					{
						using (DataTable COREDataTable = new DataTable())
						{
							COREDataAdapter.Fill(COREDataTable);

							if (COREDataTable.Rows.Count > 0)
							{
								string[] MainArr = new string[COREDataTable.Rows.Count];
								string[] SubArr = new string[COREDataTable.Rows.Count];
								for (int i = 0; i < COREDataTable.Rows.Count; i++)
								{
									//Main Role Access
									if (COREDataTable.Rows[i]["Main"].ToString() == COREDataTable.Rows[i == 0 ? COREDataTable.Rows.Count - 1 : i - 1]["Main"].ToString())
									{
										if (i == 0)
										{
											MainArr[i] = COREDataTable.Rows[i]["Main"].ToString();
										}
										else
										{
											less++;
											MainArr[i - less] = COREDataTable.Rows[i]["Main"].ToString();
										}
									}
									else if (MainArr[i == 0 ? MainArr.Length - 1 : i - 1] == null && MainArr[0] != null)
									{
										MainArr[i - less] = COREDataTable.Rows[i]["Main"].ToString();
									}
									else
									{
										MainArr[i] = COREDataTable.Rows[i]["Main"].ToString();
									}
									//Sub Role Access
									if (COREDataTable.Rows[i]["Sub"] == System.DBNull.Value)
									{
										SubArr[i] = null;
									}
									else
									{
										SubArr[i] = COREDataTable.Rows[i]["Sub"].ToString();
									}
									////Main Role Access
									//if (COREDataTable.Rows[i]["MainName"].ToString() == COREDataTable.Rows[i == 0 ? COREDataTable.Rows.Count - 1 : i - 1]["MainName"].ToString())
									//{
									//    if (i == 0)
									//    {
									//        MainArr[i] = COREDataTable.Rows[i]["MainName"].ToString();
									//    }
									//    else
									//    {
									//        less++;
									//        MainArr[i - less] = COREDataTable.Rows[i]["MainName"].ToString();
									//    }
									//}
									//else if (MainArr[i == 0 ? MainArr.Length - 1 : i - 1] == null && MainArr[0] != null)
									//{
									//    MainArr[i - less] = COREDataTable.Rows[i]["MainName"].ToString();
									//}
									//else
									//{
									//    MainArr[i] = COREDataTable.Rows[i]["MainName"].ToString();
									//}
									////Sub Role Access
									//if (COREDataTable.Rows[i]["SubName"] == System.DBNull.Value)
									//{
									//    SubArr[i] = null;
									//    //SubArr[i] = COREDataTable.Rows[i == 0 ? COREDataTable.Rows.Count - 1 : i - 1]["SubName"].ToString();
									//}
									//else if (Convert.ToInt32(COREDataTable.Rows[i]["Sub"]) == 31 || Convert.ToInt32(COREDataTable.Rows[i]["Sub"]) == 41 || Convert.ToInt32(COREDataTable.Rows[i]["Sub"]) == 51)
									//{
									//    if (Convert.ToInt32(COREDataTable.Rows[i]["Sub"]) == 31)
									//    {
									//        SubArr[i] = "CommonItemRequest";
									//    }
									//    else if (Convert.ToInt32(COREDataTable.Rows[i]["Sub"]) == 41)
									//    {
									//        SubArr[i] = "CommonInquiry";
									//    }
									//    else if (Convert.ToInt32(COREDataTable.Rows[i]["Sub"]) == 51)
									//    {
									//        SubArr[i] = "CommonApproveRequest";
									//    }
									//}
									//else if (Convert.ToInt32(COREDataTable.Rows[i]["Sub"]) == 32 || Convert.ToInt32(COREDataTable.Rows[i]["Sub"]) == 42 || Convert.ToInt32(COREDataTable.Rows[i]["Sub"]) == 52)
									//{
									//    if (Convert.ToInt32(COREDataTable.Rows[i]["Sub"]) == 32)
									//    {
									//        SubArr[i] = "UniqueItemRequest";
									//    }
									//    else if (Convert.ToInt32(COREDataTable.Rows[i]["Sub"]) == 42)
									//    {
									//        SubArr[i] = "UniqueInquiry";
									//    }
									//    else if (Convert.ToInt32(COREDataTable.Rows[i]["Sub"]) == 52)
									//    {
									//        SubArr[i] = "UniqueApproveRequest";
									//    }
									//}
									//else
									//{
									//    SubArr[i] = COREDataTable.Rows[i]["SubName"].ToString();
									//}
								}
								RoleModel = new RoleAccessModel
								{
									Main = MainArr,
									Sub = SubArr,
								};
							}
						}
					}
				}
			}
			return RoleModel;
        }

        public List<MainAccess> PopulateAllMainDatabase()
        {
            DBConnection();
            List<MainAccess> AllMainData = new List<MainAccess>();
            using (SqlConnection COREConnection = new SqlConnection(con))
            {
                using (SqlCommand CORECommand = new SqlCommand("Table_PopulateAllMainRoleAccess", COREConnection))
                {
                    CORECommand.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter COREDataAdapter = new SqlDataAdapter(CORECommand))
                    {
                        using (DataTable COREDataTable = new DataTable())
                        {
                            COREDataAdapter.Fill(COREDataTable);
                            if (COREDataTable.Rows.Count > 0)
                            {
                                foreach (DataRow items in COREDataTable.Rows)
                                {
                                    MainAccess Main = new MainAccess
                                    {
                                        AllMainID = items["MainID"].ToString(),
                                        AllMainName = items["MainName"].ToString(),
                                    };
                                    AllMainData.Add(Main);
                                }
                            }
                        }
                    }

                }
            }
            return AllMainData;
        }

        public List<SubAccess> PopulateAllSubDatabase()
        {
            DBConnection();
            List<SubAccess> AllSubData = new List<SubAccess>();
            using (SqlConnection COREConnection = new SqlConnection(con))
            {
                using (SqlCommand CORECommand = new SqlCommand("Table_PopulateAllSubRoleAccess", COREConnection))
                {
                    CORECommand.CommandType = CommandType.StoredProcedure;
                    using (SqlDataAdapter COREDataAdapter = new SqlDataAdapter(CORECommand))
                    {
                        using (DataTable COREDataTable = new DataTable())
                        {
                            COREDataAdapter.Fill(COREDataTable);
                            if (COREDataTable.Rows.Count > 0)
                            {
                                foreach (DataRow items in COREDataTable.Rows)
                                {
                                    SubAccess Sub = new SubAccess
                                    {
                                        AllSubID = items["SubAccessKey"].ToString(),
                                        AllSubMainID = items["SubMainAccessKey"].ToString(),
                                        AllSubName = items["Subname"].ToString(),
                                    };
                                    AllSubData.Add(Sub);
                                }
                            }
                        }
                    }

                }
            }
            return AllSubData;
        }

		public List<MainAccess> PopulateMainDatabase(string id)
		{
			DBConnection();
			List<MainAccess> MainData = new List<MainAccess>();
			using (SqlConnection COREConnection = new SqlConnection(con))
			{
				using (SqlCommand CORECommand = new SqlCommand("Table_PopulateMainRoleAccess", COREConnection))
				{
					CORECommand.CommandType = CommandType.StoredProcedure;
					CORECommand.Parameters.AddWithValue("@UserID", id);
					using (SqlDataAdapter COREDataAdapter = new SqlDataAdapter(CORECommand))
					{
						using (DataTable COREDataTable = new DataTable())
						{
							COREDataAdapter.Fill(COREDataTable);
							if (COREDataTable.Rows.Count > 0)
							{
								foreach (DataRow items in COREDataTable.Rows)
								{
									MainAccess Main = new MainAccess
									{
										MainID = items["Main"].ToString(),
										MainName = items["RoleID"].ToString(),
									};
									MainData.Add(Main);
								}
							}
						}
					}

				}
			}
			return MainData;
		}

		public List<SubAccess> PopulateSubDatabase(string id)
		{
			DBConnection();
			List<SubAccess> SubData = new List<SubAccess>();
			using (SqlConnection COREConnection = new SqlConnection(con))
			{
				using (SqlCommand CORECommand = new SqlCommand("Table_PopulateSubRoleAccess", COREConnection))
				{
					CORECommand.CommandType = CommandType.StoredProcedure;
					CORECommand.Parameters.AddWithValue("@UserID", id);
					using (SqlDataAdapter COREDataAdapter = new SqlDataAdapter(CORECommand))
					{
						using (DataTable COREDataTable = new DataTable())
						{
							COREDataAdapter.Fill(COREDataTable);
							if (COREDataTable.Rows.Count > 0)
							{
								foreach (DataRow items in COREDataTable.Rows)
								{
									SubAccess Sub = new SubAccess
									{
										SubID = items["Sub"].ToString(),
										SubMainID = items["SubMain"].ToString(),
										SubName = items["RoleName"].ToString(),
									};
									SubData.Add(Sub);
								}
							}
						}
					}

				}
			}
			return SubData;
		}

		public int RoleManagementInsertRoleModule(AllModels Model, string id)
		{
			DBConnection();
			int result = 0;
			try
			{
				using (SqlConnection COREConnection = new SqlConnection(con))
				{
					for (int module = 0; module < Model.TreeView.Module.Count; module++)
					{
						using (SqlCommand CORECommand = new SqlCommand("Insert_RoleModuleInformation", COREConnection))
						{
							COREConnection.Open();
							CORECommand.CommandType = CommandType.StoredProcedure;
							CORECommand.Parameters.AddWithValue("@Responsible", id);
							CORECommand.Parameters.AddWithValue("@RoleName", Model.RoleManagement.RoleName);
							CORECommand.Parameters.AddWithValue("@ModuleID", Model.TreeView.Module[module].selectedID);

							CORECommand.ExecuteNonQuery();
							COREConnection.Close();
						}
					}
					result++;
					return result;
				}
			}
			catch (Exception)
			{
				result = 0;
				return result;
			}
		}

		public int RoleManagementDeleteRoleModule(AllModels Model)
		{
			DBConnection();
			int result = 0;
			try
			{
				using (SqlConnection COREConnection = new SqlConnection(con))
				{
					using (SqlCommand CORECommand = new SqlCommand("Delete_RoleModuleInformation", COREConnection))
					{
						COREConnection.Open();
						CORECommand.CommandType = CommandType.StoredProcedure;
						CORECommand.Parameters.AddWithValue("@RoleID", Model.RoleManagement.RoleID);

						CORECommand.ExecuteNonQuery();
						COREConnection.Close();
					}
					result++;
					return result;
				}
			}
			catch (Exception)
			{
				result = 0;
				return result;
			}
		}

		public int RoleManagementUpdateRoleModule(AllModels Model, string id)
		{
			DBConnection();
			int result = 0;
			try
			{
				using (SqlConnection COREConnection = new SqlConnection(con))
				{
					for (int module = 0; module < Model.TreeView.Module.Count; module++)
					{
						using (SqlCommand CORECommand = new SqlCommand("Update_RoleModuleInformation", COREConnection))
						{
							COREConnection.Open();
							CORECommand.CommandType = CommandType.StoredProcedure;
							CORECommand.Parameters.AddWithValue("@SelectedRoleID", Model.RoleManagement.RoleID);
							CORECommand.Parameters.AddWithValue("@Responsible", id);
							CORECommand.Parameters.AddWithValue("@RoleName", Model.RoleManagement.RoleName);
							CORECommand.Parameters.AddWithValue("@ModuleID", Model.TreeView.Module[module].selectedID);

							CORECommand.ExecuteNonQuery();
							COREConnection.Close();
						}
					}
					result++;
					return result;
				}
			}
			catch (Exception)
			{
				result = 0;
				return result;
			}
		}

		public RoleManagementModel RoleDisplayInfoDatabase(string roleID)
		{
			DBConnection();
			RoleManagementModel RoleData = null;
			using (SqlConnection COREConnection = new SqlConnection(con))
			{
				using (SqlCommand CORECommand = new SqlCommand("Select_GetUserRoleInformation", COREConnection))
				{
					CORECommand.CommandType = CommandType.StoredProcedure;
					CORECommand.Parameters.AddWithValue("@RoleID", roleID);
					using (SqlDataAdapter COREDataAdapter = new SqlDataAdapter(CORECommand))
					{
						using (DataTable COREDataTable = new DataTable())
						{
							COREDataAdapter.Fill(COREDataTable);

							if (COREDataTable.Rows.Count > 0)
							{
								foreach (DataRow item in COREDataTable.Rows)
								{
									RoleData = new RoleManagementModel
									{
										RoleID = Convert.ToInt32(item["RoleID"].ToString()),
										RoleName = item["RoleName"].ToString()
									};
								}
							}
						}
					}
				}
			}
			return RoleData;
		}

		public List<RoleManagementModel> PopulateRoleListDatabase()
		{
			DBConnection();
			List<RoleManagementModel> PopulateRoleList = new List<RoleManagementModel>();
			using (SqlConnection COREConnection = new SqlConnection(con))
			{
				using (SqlCommand CORECommand = new SqlCommand("Select_PopulateUserRole", COREConnection))
				{
					using (SqlDataAdapter COREDataAdapter = new SqlDataAdapter(CORECommand))
					{
						using (DataTable COREDataTable = new DataTable())
						{
							COREDataAdapter.Fill(COREDataTable);
							if (COREDataTable.Rows.Count > 0)
							{
								foreach (DataRow items in COREDataTable.Rows)
								{
									RoleManagementModel RoleAccess = new RoleManagementModel
									{
										AccessUserID = Convert.ToInt32(items["RoleID"].ToString()),
										RoleName = items["RoleName"].ToString(),
									};
									PopulateRoleList.Add(RoleAccess);
								}
							}
						}
					}

				}
			}
			return PopulateRoleList;
		}
		#endregion

		#region ManualAdjustment
		public string SelectDetailsDB(string StockNo)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			string d = "", s = "", de = "", det = "", st = "";

			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Select_GetItemDescription", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@StockNo", StockNo);

						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							DataTable dt = new DataTable();
							sda.Fill(dt);

							using (SqlDataReader sdr = command.ExecuteReader())
							{
								while (sdr.Read())
								{
									s = sdr["Specification"].ToString();
									de = sdr["ItemDescription"].ToString();
									det = sdr["Details"].ToString();
									st = sdr["StockOnHand"].ToString();
								}
								d = s + " - " + de + " - " + det + "|-|" + st;
								return d;
							}
						}
					}
				}
			}
			catch
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

		public int InsertManualAdjustDB(List<CentralModel> listManualAdjust, string id)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			int i = 0;

			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					foreach (CentralModel stock in listManualAdjust)
					{
						using (command = new SqlCommand("Update_ManualAdjustInformation", connection))
						{
							command.CommandType = CommandType.StoredProcedure;
							command.Parameters.AddWithValue("@StockNo", stock.StockNo);
							command.Parameters.AddWithValue("@ManualStock", stock.qty);
							command.Parameters.AddWithValue("@Reason", stock.Reason);
							command.Parameters.AddWithValue("@Responsible", id);
							i++;
							command.ExecuteNonQuery();
						}
					}
					return i;
				}
			}
			catch
			{
				i = 0;
				throw;
			}
			finally
			{
				connection.Close();
			}
		}
		#endregion

		#region -------------------------------------------------------------------------Transactions-------------------------------------------------------------------------
		public List<MaintenanceModel> PopulateTransactionsDB(string Date,string Week,string Movement)
		{
			DBConnection();
			List<MaintenanceModel> PopulateTransactions = new List<MaintenanceModel>();
			using (SqlConnection COREConnection = new SqlConnection(con))
			{
				using (SqlCommand CORECommand = new SqlCommand("Table_PopulateTransactions", COREConnection))
				{
					using (SqlDataAdapter COREDateAdapter = new SqlDataAdapter(CORECommand))
					{
                        CORECommand.CommandType = CommandType.StoredProcedure;
                        CORECommand.Parameters.AddWithValue("@Movement", Movement);
                        CORECommand.Parameters.AddWithValue("@From", Week);
                        CORECommand.Parameters.AddWithValue("@To", Date);
                        using (DataTable COREDataTable = new DataTable())
						{
							COREDateAdapter.Fill(COREDataTable);
							if (COREDataTable.Rows.Count > 0)
							{
								foreach (DataRow rows in COREDataTable.Rows)
                                {
                                    int old = 0;
                                    if (rows["TransactionType"].ToString() == "Adjust")
                                    {
                                        if (Convert.ToInt32(rows["TransStockOnHand"].ToString()) > Convert.ToInt32(rows["RemainingStock"].ToString()))
                                        {
                                            old = Convert.ToInt32(rows["TransStockOnHand"].ToString()) - Convert.ToInt32(rows["RemainingStock"].ToString());
                                        }
                                        else
                                        {
                                            old = Convert.ToInt32(rows["RemainingStock"].ToString()) - Convert.ToInt32(rows["TransStockOnHand"].ToString());
                                        }
                                    }
                                    else
                                    {
                                        old = Convert.ToInt32(rows["Quantity"].ToString());
                                    }
									MaintenanceModel Transaction = new MaintenanceModel
									{
										TransactionType = rows["TransactionType"].ToString(),
										Item = rows["Item"].ToString(),
										Quantity = old.ToString(),
										TransStockOnHand = rows["TransStockOnHand"].ToString(),
										AdjustStockOnHand = rows["StockOnHand"].ToString(),
										RemainingStock = rows["RemainingStock"].ToString(),
										TransactionDate = rows["TransactionDate"].ToString(),
										Responsible = rows["Responsible"].ToString()
									};
									PopulateTransactions.Add(Transaction);
								}
							}
						}
					}
				}
			}
			return PopulateTransactions;
		}

        public List<SelectListItem> TransactionListDB()
        {
            DBConnection();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<SelectListItem> List = new List<SelectListItem>();
            try
            {
                using (connection = new SqlConnection(con))
                {
                    connection.Open();

                    using (command = new SqlCommand("Select_PopulateTransaction", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                if (dt.Rows.Count > 0)
                                {
                                    foreach (DataRow row in dt.Rows)
                                    {
                                        List.Add(new SelectListItem
                                        {
                                            Text = row["TypeName"].ToString(),
                                            Value = row["TransTypeID"].ToString(),
                                        });
                                    }
                                }

                                return List;
                            }
                        }
                    }
                }
            }
            catch
            {
                return List;
            }
            finally
            {
                connection.Close();
            }
        }

		public int InsertTransactionDetails(List<MaintenanceModel> TransactionDetailList, string id)
		{
			DBConnection();
			int result = 0;
			try
			{
				using (SqlConnection COREConnection = new SqlConnection(con))
				{
					foreach (var item in TransactionDetailList)
					{
                        if (!string.IsNullOrWhiteSpace(item.Quantity))
                        {
                            using (SqlCommand CORECommand = new SqlCommand("Insert_Transactions", COREConnection))
                            {
                                COREConnection.Open();
                                CORECommand.CommandType = CommandType.StoredProcedure;
                                CORECommand.Parameters.AddWithValue("@TransactionType", item.TransactionType);
                                CORECommand.Parameters.AddWithValue("@Item", item.Item);
                                CORECommand.Parameters.AddWithValue("@Quantity", item.Quantity);
                                CORECommand.Parameters.AddWithValue("@TransStockOnHand", item.TransStockOnHand);
                                CORECommand.Parameters.AddWithValue("@RemainingStock", item.RemainingStock);
                                CORECommand.Parameters.AddWithValue("@Responsible", id);
                                CORECommand.ExecuteNonQuery();
                                result++;
                                COREConnection.Close();
                            }
                        }
					}
				}
				result++;
				return result;
			}
			catch (Exception)
			{
				result = 0;
				return result;
			}
		}
		#endregion

        #region -------------------------------------------------------------------------ItemLocationMonitoring-------------------------------------------------------------------------
        public List<MaintenanceModel> FilterMainItemlocationListDB(MaintenanceModel maintenance)
        {
            DBConnection();
            List<MaintenanceModel> listMain = new List<MaintenanceModel>();
            SqlConnection connection = null;
            SqlCommand command = null;
            if (maintenance == null)
            {
                maintenance = new MaintenanceModel();
            }
            try
            {
                using (connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (command = new SqlCommand("Table_PopulateMainItemLocationFiltered", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@FilteredValue", maintenance.MainNo ?? "");
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    MaintenanceModel main = new MaintenanceModel()
                                    {
                                        RackNo = row["RackNo"].ToString(),
                                        DrawerNo = row["DrawerID"].ToString(),
                                        BinNo = row["BinID"].ToString()
                                    };
                                    listMain.Add(main);
                                }
                            }
                        }
                    }
                }
                return listMain;
            }
            catch
            {
                throw;
            }
            finally
            {
                connection.Close();
            }
        }
        #endregion


        public int UpdateReleasingData(MaintenanceModel maintenanceModel)
        {
            DBConnection();
            SqlConnection connection = null;
            SqlCommand command = null;
            int i = 0;
            try
            {
                using (connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_ItemReleasing", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Type", maintenanceModel.ProcessType);
                        command.Parameters.AddWithValue("@ReferenceNo", maintenanceModel.UpdateReferenceNo);
                        command.Parameters.AddWithValue("@RequestStatus", maintenanceModel.UpdateItemStatus);
                        command.Parameters.AddWithValue("@ItemStatus", maintenanceModel.UpdateRequestStatus);
                        command.Parameters.AddWithValue("@StockNo", maintenanceModel.UpdateItemCode);
                        command.Parameters.AddWithValue("@Qty", maintenanceModel.UpdateQty);
                        i++;
                        command.ExecuteNonQuery();
                        return i;
                    }
                }
            }
            catch
            {
                i = 0;
                throw;
            }
            finally
            {
                connection.Close();
            }
        }

        public int UpdateReceivingData(MaintenanceModel maintenanceModel)
        {
            DBConnection();
            SqlConnection connection = null;
            SqlCommand command = null;
            int i = 0;
            try
            {
                using (connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (command = new SqlCommand("Update_PRReceiving", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", maintenanceModel.UpdateReferenceNo);
                        command.Parameters.AddWithValue("@RequestStatus", maintenanceModel.UpdateItemStatus);
                        command.Parameters.AddWithValue("@PRStatus", maintenanceModel.UpdatePRStatus);
                        command.Parameters.AddWithValue("@StockNo", maintenanceModel.UpdateItemCode);
                        command.Parameters.AddWithValue("@Qty", maintenanceModel.UpdateQty);
                        i++;
                        command.ExecuteNonQuery();
                        return i;
                    }
                }
            }
            catch
            {
                i = 0;
                throw;
            }
            finally
            {
                connection.Close();
            }
        }
	}



}