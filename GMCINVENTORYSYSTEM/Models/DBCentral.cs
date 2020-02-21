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
	public class DBCentral
	{
		public string con;

		public string ReferenceNo { get; set; }

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

		public void DBCentralReferenceNo()
		{
			DBConnection();
			using (SqlConnection connection = new SqlConnection(con))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand("Generate_PRReferenceNo", connection))
				{
					SqlDataAdapter sda = new SqlDataAdapter(command);
					DataTable dt = new DataTable();
					sda.Fill(dt);

					SqlDataReader sdr = command.ExecuteReader();

					while (sdr.Read())
					{
						ReferenceNo = sdr["pr_ControlNumber"].ToString();
					}
					connection.Close();
				}
			}
		}

        public string CheckRequestStatusDB(string ReferenceNo)
        {
            DBConnection();
            SqlConnection connection = null;
            SqlCommand command = null;
            string i = "";

            try
            {
                using (connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (command = new SqlCommand("Check_RequestStatus", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", ReferenceNo);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);

                            using (SqlDataReader sdr = command.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    i = sdr["RequestStatus"].ToString();
                                }
                                return i;
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

		public int PopulateCountNotificationDB(string UserRole, string User)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			int i = 0;
			try
			{
				using (connection = new SqlConnection(con))
				{
					using (command = new SqlCommand("Select_NotificationCount", connection))
					{
						connection.Open();
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@UserRoleID", UserRole);
						command.Parameters.AddWithValue("@UserID", User);
						SqlDataAdapter sda = new SqlDataAdapter(command);
						DataTable dt = new DataTable();
						sda.Fill(dt);

						SqlDataReader sdr = command.ExecuteReader();
						while (sdr.Read())
						{
							i = Convert.ToInt32(sdr["TOTALCOUNT"].ToString());
						}
						return i;
					}
				}
			}
			catch
			{
				return i = 0;
			}
			finally
			{
				connection.Close();
			}
		}

        public int PopulateRequestCountNotificationDB(string UserRole, string User, string DesignationID)
        {
            DBConnection();
            SqlConnection connection = null;
            SqlCommand command = null;
            int i = 0;
            try
            {
                using (connection = new SqlConnection(con))
                {
                    using (command = new SqlCommand("Select_NotificationRequestCount", connection))
                    {
                        connection.Open();
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserRoleID", UserRole);
                        command.Parameters.AddWithValue("@UserID", User);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        SqlDataReader sdr = command.ExecuteReader();
                        while (sdr.Read())
                        {
                            i = Convert.ToInt32(sdr["TOTALCOUNT"].ToString());
                        }
                        return i;
                    }
                }
            }
            catch
            {
                return i = 0;
            }
            finally
            {
                connection.Close();
            }
        }

		public int PopulateItemCountNotificationDB()
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			int i = 0;
			try
			{
				using (connection = new SqlConnection(con))
				{
					using (command = new SqlCommand("Select_NotificationItemCount", connection))
					{
						connection.Open();
						command.CommandType = CommandType.StoredProcedure;
						SqlDataAdapter sda = new SqlDataAdapter(command);
						DataTable dt = new DataTable();
						sda.Fill(dt);

						SqlDataReader sdr = command.ExecuteReader();
						while (sdr.Read())
						{
							i = Convert.ToInt32(sdr["TOTALCOUNT"].ToString());
						}
						return i;
					}
				}
			}
			catch
			{
				return i = 0;
			}
			finally
			{
				connection.Close();
			}
		}
		
		public List<CentralModel> PopulateNotificationApprovalDetailsDB(string UserRole, string User)
		{
			List<CentralModel> listNotification = new List<CentralModel>();
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			try
			{
				using (connection = new SqlConnection(con))
				{
					using (command = new SqlCommand("Select_NotificationContent", connection))
					{
						connection.Open();
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@UserRoleID", UserRole);
						command.Parameters.AddWithValue("@UserID", User);
						SqlDataAdapter sda = new SqlDataAdapter(command);
						DataTable dt = new DataTable();
						sda.Fill(dt);

						foreach (DataRow row in dt.Rows)
						{
							CentralModel model = new CentralModel()
							{
								NotificationReferenceNo = row["ReferenceNo"].ToString(),
								NotificationDesignation = row["ExtraDetails"].ToString(),
								NotificationEntryDate = row["DateCreated"].ToString(),
                                NotifItemStatus = row["ItemStatus"].ToString(),
							};
							listNotification.Add(model);
						}
						return listNotification;
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

        public List<CentralModel> PopulateNotificationRequestDetailsDB(string UserRole, string User ,string DesignationID)
        {
            List<CentralModel> listNotification = new List<CentralModel>();
            DBConnection();
            SqlConnection connection = null;
            SqlCommand command = null;
            try
            {
                using (connection = new SqlConnection(con))
                {
                    using (command = new SqlCommand("Select_NotificationRequestContent", connection))
                    {
                        connection.Open();
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserRoleID", UserRole);
                        command.Parameters.AddWithValue("@UserID", User);
                        SqlDataAdapter sda = new SqlDataAdapter(command);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            CentralModel model = new CentralModel()
                            {
                                NotificationReferenceNo = row["ReferenceNo"].ToString(),
                                NotificationDesignation = row["ExtraDetails"].ToString(),
                                NotificationEntryDate = row["DateCreated"].ToString(),
                                NotifItemStatus = row["ItemStatus"].ToString(),
                            };
                            listNotification.Add(model);
                        }
                        return listNotification;
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

		public List<CentralModel> PopulateNotificationItemDetailsDB()
		{

			List<CentralModel> listNotification = new List<CentralModel>();
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			try
			{
				using (connection = new SqlConnection(con))
				{
					using (command = new SqlCommand("Select_NotificationItemContent", connection))
					{
						connection.Open();
						command.CommandType = CommandType.StoredProcedure;
						SqlDataAdapter sda = new SqlDataAdapter(command);
						DataTable dt = new DataTable();
						sda.Fill(dt);

						foreach (DataRow row in dt.Rows)
						{
							CentralModel model = new CentralModel()
							{
								NotificationStockNo = row["StockNo"].ToString(),
								NotificationDate = row["UpdateDateItemStock"].ToString(),
								NotificationItemName = row["ItemName"].ToString(),
								NotificationTypeOfItem = row["TypeName"].ToString(),
							};
							listNotification.Add(model);
						}
						return listNotification;
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

		public List<SelectListItem> PopulateAllitemStockNo()
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

					using (command = new SqlCommand("Select_PopulatePRAllItemStockNo", connection))
					{
						command.CommandType = CommandType.StoredProcedure;


						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									ListStockNo.Add(new SelectListItem
									{
                                        Text = string.Format("{0}|{1}|{2}", row["StockNo"].ToString(), row["ItemName"].ToString(), row["Specification"].ToString()),
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

		public List<SelectListItem> PopulateAllDesignationDB()
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			List<SelectListItem> ListDesignation = new List<SelectListItem>();
			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();

					using (command = new SqlCommand("Select_PopulateAllUserDesignation", connection))
					{
						command.CommandType = CommandType.StoredProcedure;


						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									ListDesignation.Add(new SelectListItem
									{
										Text = row["Designation"].ToString(),
										Value = row["DesignationID"].ToString(),
									});
								}
								return ListDesignation;
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

		public string SelectItemNameDB(string StockNo)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			string i = "";

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
									i = sdr["ItemName"].ToString();
								}
								return i;
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

        public string SelectDescriptionDB(string StockNo)
        {
            DBConnection();
            SqlConnection connection = null;
            SqlCommand command = null;
            string i = "";

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
                                    i = sdr["DesignationID"].ToString();
                                }
                                return i;
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

		public int InsertRequestPRDB(CentralModel central, string responsible)
		{
			DBConnection();
			DBCentralReferenceNo();
			SqlConnection connection = null;
			SqlCommand command = null;
			int i = 0;

			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Insert_RequestPRInformation", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@ReferenceNo", ReferenceNo ?? "");
						command.Parameters.AddWithValue("@PRNo", central.PRNo);
						command.Parameters.AddWithValue("@Purpose", central.Purpose);
						command.Parameters.AddWithValue("@DateApprovedPR", central.DateApprovedPR);
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

		public int InsertItemRequestPRDB(List<CentralModel> listRequest, CentralModel central)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			int i = 0;

			try
			{
				if (listRequest == null)
				{
					listRequest = new List<CentralModel>();
				}

				using (connection = new SqlConnection(con))
				{
					connection.Open();

					foreach (CentralModel item in listRequest)
					{
						using (command = new SqlCommand("Insert_RequestPRInformationGRID", connection))
						{
							command.CommandType = CommandType.StoredProcedure;

							command.Parameters.AddWithValue("@ReferenceNo", ReferenceNo ?? "");
							command.Parameters.AddWithValue("@PRNo", central.PRNo);
							command.Parameters.AddWithValue("@StockNo", item.StockNo);
							command.Parameters.AddWithValue("@ItemName", item.ItemName);
                            command.Parameters.AddWithValue("@Qty", item.qty);
                            command.Parameters.AddWithValue("@DesignationID", item.Designation);
						}
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

        public int RequestedPRQuantityDB(CentralModel central)
        {
            DBConnection();
            SqlConnection connection = null;
            SqlCommand command = null;
            int i = 0;

            try
            {
                i++;
                command.ExecuteNonQuery();
                return i;
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

		public List<CentralModel> PopulateRequestPRDB( string GroupID, string DesignationID)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			List<CentralModel> listReceiving = new List<CentralModel>();
			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Table_PopulateReceivingRequestPR", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@GroupID", GroupID);
						command.Parameters.AddWithValue("@DesignationID", DesignationID);

						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									CentralModel central = new CentralModel()
									{
										ReferenceNo = row["ReferenceNo"].ToString(),
										PRNo = row["PRNo"].ToString(),
										Responsible = row["Responsible"].ToString(),
										DateRequested = row["DateRequested"].ToString(),
										RequestStatus = row["RequestStatus"].ToString(),
										DateApprovedPR = row["DateApprovedPR"].ToString(),
									};
									listReceiving.Add(central);

								}
								return listReceiving;
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
				
		public List<CentralModel> PopulateItemRequestForReceivingDB(string PRNo, string ReferenceNo, string GroupID, string DesignationID)
		{
			DBConnection();
			List<CentralModel> listItemForReceiving = new List<CentralModel>();
			SqlConnection connection = null;
			SqlCommand command = null;

			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Table_PopulateItemRequestForReceiving", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@PRNo", PRNo);
						command.Parameters.AddWithValue("@ReferenceNo", ReferenceNo);
						command.Parameters.AddWithValue("@GroupID", GroupID);
						command.Parameters.AddWithValue("@DesignationID", DesignationID);

						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									CentralModel central = new CentralModel()
									{
										Number = row["Number"].ToString(),
										StockNo = row["StockNo"].ToString(),
										ItemName = row["ItemName"].ToString(),
										qty = row["Qty"].ToString(),
                                        Excess = row["Excess"].ToString(),
										StockOnHand = row["StockOnHand"].ToString(),
										collected = row["ItemReceived"].ToString(),
									};
									listItemForReceiving.Add(central);
								}
								return listItemForReceiving;
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
		
		public int UpdateItemUniqueRequestReceivedDB(List<CentralModel> listRequest, CentralModel central, string responsible)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			int i = 0;

			try
			{
				if (listRequest == null)
				{
					listRequest = new List<CentralModel>();
				}


				using (connection = new SqlConnection(con))
				{
					connection.Open();
					foreach (CentralModel item in listRequest)
					{
						if (!string.IsNullOrWhiteSpace(item.ItemReceived))
						{
							using (command = new SqlCommand("Insert_ItemUniqueRequestReceived", connection))
							{
								command.CommandType = CommandType.StoredProcedure;
								command.Parameters.AddWithValue("@ReferenceNo", central.DisplayReferenceNo);
								command.Parameters.AddWithValue("@PRNo", central.DisplayPRNo);
								command.Parameters.AddWithValue("@StockNo", item.StockNo);
								command.Parameters.AddWithValue("@ItemName", item.ItemName);
								command.Parameters.AddWithValue("@Qty", item.qty);
								command.Parameters.AddWithValue("@Number", item.Number);
								command.Parameters.AddWithValue("@SupplierName", central.SupplierName == null ? "" : central.SupplierName);
								command.Parameters.AddWithValue("@DateReceived", central.DateReceived);
								command.Parameters.AddWithValue("@Responsible", responsible);
								command.Parameters.AddWithValue("@Received", item.ItemReceived);
								i++;
								command.ExecuteNonQuery();

							}
						}

					}

					return i;
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

		public int UpdateItemRequestReceivedDB(List<CentralModel> listRequest, CentralModel central, string responsible)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			int i = 0;

			try
			{
				if (listRequest == null)
				{
					listRequest = new List<CentralModel>();
				}


				using (connection = new SqlConnection(con))
				{
					connection.Open();
					foreach (CentralModel item in listRequest)
					{
						if (!string.IsNullOrWhiteSpace(item.ItemReceived))
						{
							using (command = new SqlCommand("Insert_ItemRequestReceived", connection))
							{
								command.CommandType = CommandType.StoredProcedure;
								command.Parameters.AddWithValue("@ReferenceNo", central.DisplayReferenceNo);
								command.Parameters.AddWithValue("@PRNo", central.DisplayPRNo);
								command.Parameters.AddWithValue("@StockNo", item.StockNo);
								command.Parameters.AddWithValue("@ItemName", item.ItemName);
                                command.Parameters.AddWithValue("@Qty", item.qty);
                                command.Parameters.AddWithValue("@Excess", item.Excess ?? "");
								command.Parameters.AddWithValue("@Number", item.Number);
								command.Parameters.AddWithValue("@SupplierName", central.SupplierName == null ? "" : central.SupplierName);
								command.Parameters.AddWithValue("@DateReceived", central.DateReceived);
								command.Parameters.AddWithValue("@Responsible", responsible);
								command.Parameters.AddWithValue("@Received", item.ItemReceived ?? "0");
								i++;
								command.ExecuteNonQuery();

							}
						}

					}

					return i;
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

		public int UpdateItemCommonReleasedDB(List<CentralModel> listRequest, CentralModel central, string responsible)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			int i = 0;

			try
			{
				if (listRequest == null)
				{
					listRequest = new List<CentralModel>();
				}


				using (connection = new SqlConnection(con))
				{
					connection.Open();
					foreach (CentralModel item in listRequest)
					{
						if (!string.IsNullOrWhiteSpace(item.ItemReleased))
						{
							using (command = new SqlCommand("Insert_ItemCommonReleased", connection))
							{
								command.CommandType = CommandType.StoredProcedure;
								command.Parameters.AddWithValue("@ReferenceNo", central.DisplayReferenceNo);
								command.Parameters.AddWithValue("@StockNo", item.StockNo);
								command.Parameters.AddWithValue("@ItemName", item.ItemName);
								command.Parameters.AddWithValue("@Qty", item.qty);
								command.Parameters.AddWithValue("@Item_Released", item.ItemReleased);
								command.Parameters.AddWithValue("@Date_Released", central.DateReleased);
								command.Parameters.AddWithValue("@Responsible", responsible);
								
								i++;
								command.ExecuteNonQuery();

							}
						}

					}

					return i;
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

		public int NotifyRequesterDB(string ReferenceNo)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			int i = 0;
			try {
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Update_NotifyRequester", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@ReferenceNo", ReferenceNo);
						command.ExecuteNonQuery();
						i++;
					}
					return i;
				}
			}
			catch(Exception)
			{
				throw;
			}
			finally
			{
				connection.Close();
			}
		}

        public int Update_NotificationViewDB(string ReferenceNo)
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
                    using (command = new SqlCommand("Update_NotificationView", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReferenceNo", ReferenceNo);
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

		public List<CentralModel> PopulateAllRequestCommon(string ApproverID,string GroupID)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			List<CentralModel> listRequestCommon = new List<CentralModel>();
			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Table_PopulateAllRequestCommon", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@Approver", ApproverID);
						command.Parameters.AddWithValue("@Group", GroupID);
						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									CentralModel centralModel = new CentralModel()
									{
										ReferenceNo = row["ReferenceNo"].ToString(),
										CompanyID = row["CompanyID"].ToString(),
										Responsible = row["FullName"].ToString(),
										Designation = row["Designation"].ToString(),
										Department = row["Department"].ToString(),
										DateRequested = row["DateRequested"].ToString(),
										Purpose = row["Purpose"].ToString(),
										RequestStatus = row["RequestStatus"].ToString(),
										ApproverID = row["Requestedby"].ToString(),
                                        Requester = row["Requestedby"].ToString(),
									};

									listRequestCommon.Add(centralModel);
								}
								return listRequestCommon;
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

		public List<CentralModel> PopulateAllItemCommonRequest(string ReferenceNo)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			List<CentralModel> listitemCommon = new List<CentralModel>();
			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Table_PopulateAllCommonItemRequest", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.AddWithValue("@ReferenceNo", ReferenceNo);

						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{

								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									CentralModel model = new CentralModel()
									{
										StockNo = row["StockNo"].ToString(),
										ItemName = row["ItemName"].ToString(),
										qty = row["Qty"].ToString(),
										StockOnHand = row["StockOnHand"].ToString(),
										Purpose = row["Purpose"].ToString(),
										ApproverID = row["RequestedBy"].ToString(),
                                        RequestStatus = row["RequestStatuss"].ToString(),
                                        Responsible = row["Responsible"].ToString(),

									};

									listitemCommon.Add(model);
								}
								return listitemCommon;
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

        public List<CentralModel> PopulateItemPRRequestDB(string id,string Date,string Week)
        {
            DBConnection();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<CentralModel> listReceiving = new List<CentralModel>();
            try
            {
                using (connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (command = new SqlCommand("Table_PopulateMonitoringRequestPR", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", id);
                        command.Parameters.AddWithValue("@From", Week);
                        command.Parameters.AddWithValue("@To", Date);
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    CentralModel central = new CentralModel()
                                    {
                                        row = row["Row#"].ToString(),
                                        ReferenceNo = row["ReferenceNo"].ToString(),
                                        PRNo = row["PRNo"].ToString(),
                                        Responsible = row["Responsible"].ToString(),
                                        DateRequested = row["DateRequested"].ToString(),
                                        RequestStatus = row["RequestStatus"].ToString(),
                                        DateApprovedPR = row["DateApprovedPR"].ToString(),
                                    };
                                    listReceiving.Add(central);

                                }
                                return listReceiving;
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

		public List<CentralModel> PopulateItemCommonForReleasingDB(string ReferenceNo)
		{
			DBConnection();
			List<CentralModel> listItemForReleasing = new List<CentralModel>();
			SqlConnection connection = null;
			SqlCommand command = null;

			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Table_PopulateAllCommonItemRequest", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@ReferenceNo", ReferenceNo);

						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									CentralModel central = new CentralModel()
									{
										StockNo = row["StockNo"].ToString(),
										ItemName = row["ItemName"].ToString(),
										qty = row["Qty"].ToString(),
										Delivered = row["Delivered"].ToString(),
										StockOnHand = row["StockOnHand"].ToString(),
										ApproverID = row["Approver"].ToString(),
										DateApproved = row["CentralApprovedDate"].ToString(),
										Rejectedby = row["Rejectedby"].ToString(),
										DateRejected = row["RejectDate"].ToString(),
										RejectRemarks = row["RejectRemarks"].ToString(),

									};
									listItemForReleasing.Add(central);
								}
								return listItemForReleasing;
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
		
		public List<CentralModel> PopulateAllItemCommonForReleasingDB()
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			List<CentralModel> listForReleasing = new List<CentralModel>();

			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Table_PopulateItemCommonForReleasing", connection))
					{
						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach(DataRow row in dt.Rows)
								{

									CentralModel central = new CentralModel()
									{
										ReferenceNo = row["ReferenceNo"].ToString(),
										CompanyID = row["CompanyID"].ToString(),
										ApproverID = row["Approver"].ToString(),
										DateRequested = row["DateRequested"].ToString(),
										RequestStatus = row["RequestStatuss"].ToString(),
										Responsible = row["Requester"].ToString(),
										Purpose = row["Purpose"].ToString(),
										GroupName = row["GroupName"].ToString(),
										Designation = row["Designation"].ToString(),
                                        NotifRequestStatus = row["RequestStatus"].ToString(),
                                        NotifItemStatus = row["ItemStatus"].ToString(),
                                        Requester = row["ResponsibleID"].ToString(),
                                        UserRole = row["RoleID"].ToString(),
									};
									listForReleasing.Add(central);
								}
								return listForReleasing;
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

        public List<CentralModel> PopulateReceiveLogsDB(string Designation, string Group, string Date, string Week)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			List<CentralModel> listLogs = new List<CentralModel>();
			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Table_PopulateRecivedLogs", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@GroupID", Group);
                        command.Parameters.AddWithValue("@SelectedDesignation", Designation);
                        command.Parameters.AddWithValue("@From", Week);
                        command.Parameters.AddWithValue("@To", Date);

						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									CentralModel central = new CentralModel()
									{

										ReferenceNo = row["ReferenceNo"].ToString(),
                                        PRNo = row["PRNo"].ToString(),
										DateReceived = row["Date_Received"].ToString(),
										Responsible = row["Responsible"].ToString(),
										Designation = row["Designation"].ToString(),
										Purpose = row["Purpose"].ToString(),
										qty = row["Qty"].ToString(),
										ItemReceived = row["Item_Received"].ToString(),
										DateRequested = row["DateRequested"].ToString(),
										ItemName = row["ItemName"].ToString(),
										StockNo = row["StockNo"].ToString(),
                                        TotalReceived = row["Total_Received"].ToString(),
                                        row = row["Row#"].ToString(),
									};
									listLogs.Add(central);
								}
								return listLogs;
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

        public List<CentralModel> PopulateReleaseLogsDB(string Designation, string Group, string Date, string Week)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			List<CentralModel> listLogs = new List<CentralModel>();
			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Table_PopulateReleaseLogs", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@GroupID", Group);
                        command.Parameters.AddWithValue("@SelectedDesignation", Designation);
                        command.Parameters.AddWithValue("@From", Week);
                        command.Parameters.AddWithValue("@To", Date);

                        //if (Group == "2")
                        //{
                        //    command.Parameters.AddWithValue("@SelectedDesignation", Designation ?? "");

                        //}
                        //else
                        //{
                        //    command.Parameters.AddWithValue("@SelectedDesignation", model.Designation ?? "");

                        //}
                        //command.Parameters.AddWithValue("@DateReleased", model.DateReleased ?? "");

						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach(DataRow row in dt.Rows)
								{
									CentralModel central = new CentralModel()
									{
										ReferenceNo = row["ReferenceNo"].ToString(),
                                        Requester = row["RequestedBy"].ToString(),
										DateReleased = row["Date_Released"].ToString(),
										Responsible = row["Responsible"].ToString(),
										Designation = row["Designation"].ToString(),
                                        Purpose = row["Purpose"].ToString(),
										qty = row["Qty"].ToString(),
										ItemReleased = row["Item_Released"].ToString(),
										DateRequested = row["DateRequested"].ToString(),
										StockNo = row["StockNo"].ToString(),
										ItemName = row["ItemName"].ToString(),
                                        TotalReleased = row["Total_Released"].ToString(),
                                        row = row["Row#"].ToString(),
									};
									listLogs.Add(central);
								}
								return listLogs;
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
        
		public List<CentralModel> PopulateManageRequestedItem(string GroupID, string DesignationID,string Date,string Week)
		{
			SqlConnection connection = null;
			SqlCommand command = null;
			DBConnection();
			List<CentralModel> listCommonRequest = new List<CentralModel>();
			try
			{
				using (connection = new SqlConnection(con))
				{
                    using (command = new SqlCommand("Table_PopulateRequestedItem", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@GroupID", GroupID);
						command.Parameters.AddWithValue("@DesignationID", DesignationID);
                        command.Parameters.AddWithValue("@From", Week);
                        command.Parameters.AddWithValue("@To", Date);
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);
								foreach(DataRow row in dt.Rows)
								{
									CentralModel central = new CentralModel()
									{
										ReferenceNo = row["ReferenceNo"].ToString(),
										Requester = row["RequestedBy"].ToString(),
										Responsible = row["Responsible"].ToString(),
										Purpose = row["Purpose"].ToString(),
										DateRequested = row["DateRequested"].ToString(),
										RequestStatus = row["RequestStatus"].ToString(),
                                        Designation = row["Designation"].ToString(),
										TypeRequest = row["TYPESS"].ToString(),
										
									};
									listCommonRequest.Add(central);
								}
								return listCommonRequest;
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
		
		public List<CentralModel> PopulateManageCommonRequest(string GroupID, string DesignationID, string UserID, string Date, string Week)
		{
			SqlConnection connection = null;
			SqlCommand command = null;
			DBConnection();
			List<CentralModel> listCommonRequest = new List<CentralModel>();
			try
			{
				using (connection = new SqlConnection(con))
				{
					using (command = new SqlCommand("Table_PopulateCommonRequestStatus", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@GroupID", GroupID);
						command.Parameters.AddWithValue("@DesignationID", DesignationID);
						command.Parameters.AddWithValue("@UserID", UserID);
                        command.Parameters.AddWithValue("@From", Week);
                        command.Parameters.AddWithValue("@To", Date);
                        
						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);
								foreach(DataRow row in dt.Rows)
								{
									CentralModel central = new CentralModel()
									{
										ReferenceNo = row["ReferenceNo"].ToString(),
										Requester = row["RequestedBy"].ToString(),
										Responsible = row["Responsible"].ToString(),
										DateRequested = row["DateRequested"].ToString(),
										RequestStatus = row["RequestStatus"].ToString(),
									};
									listCommonRequest.Add(central);
								}
								return listCommonRequest;
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

		public int UpdateRejectRoutingApprovalCommonDB(CentralModel central, string Responsible)
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
					using (command = new SqlCommand("Update_RejectRoutingApprovalCommon", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@ReferenceNo", central.ReferenceNo);
						command.Parameters.AddWithValue("@RejectRemarks", central.RejectRemarks);
						command.Parameters.AddWithValue("@TypeRequest", central.TypeRequest);
						command.Parameters.AddWithValue("@Responsible", Responsible);

						command.ExecuteNonQuery();
						i++;
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

		public int UpdateApproveRoutingApprovalCommonDB(CentralModel central, string Responsible)
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
					using (command = new SqlCommand("Update_ApproveRoutingApprovalCommon", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@ReferenceNo", central.ReferenceNo);
						command.Parameters.AddWithValue("@TypeRequest", central.TypeRequest);

						command.ExecuteNonQuery();
						i++;
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

		public int UpdateApprovedRoutingApprovalCommonDB(CentralModel central, string Responsible)
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
					using (command = new SqlCommand("Update_ApprovedRoutingApprovalCommon", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@ReferenceNo", central.ReferenceNo);
						command.Parameters.AddWithValue("@ApproverID", Responsible);
						command.Parameters.AddWithValue("@TypeRequest", "APPROVED");

						command.ExecuteNonQuery();
						i++;
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

        public int UpdateItemPRStatus(string StockNo)
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
                    using (command = new SqlCommand("Update_ItemPRStatus", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@StockNo", StockNo);

                        command.ExecuteNonQuery();
                        i++;
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

        //ManualAdjustmentMonitoring
        public List<CentralModel> PopulateManualAdjustDB(string Date, string Week)
        {
            DBConnection();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<CentralModel> manualAdjustList = new List<CentralModel>();
            try
            {
                using (connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (command = new SqlCommand("Table_PopulateManualAdjustment", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@From", Week);
                        command.Parameters.AddWithValue("@To", Date);
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    int qty = 0;
                                    if (Convert.ToInt32( row["AdjustStock"].ToString()) > Convert.ToInt32( row["RemainingStock"].ToString()))
                                    {
                                        qty = Convert.ToInt32(row["AdjustStock"].ToString()) - Convert.ToInt32(row["RemainingStock"].ToString());
                                    }
                                    else
	                                {
                                        qty = Convert.ToInt32(row["RemainingStock"].ToString()) - Convert.ToInt32(row["AdjustStock"].ToString());
	                                }
                                    CentralModel central = new CentralModel()
                                    {
                                        StockNo = row["StockNo"].ToString(),
                                        AdjustDate = row["AdjustDate"].ToString(),
                                        ItemName = row["ItemName"].ToString(),
                                        StockOnHand = row["StockOnHand"].ToString(),
                                        AdjustQuantity = row["AdjustStock"].ToString(),
                                        //OldStockOnHand = row["RemainingStock"].ToString(),
                                        qty = qty.ToString(),
                                        OldStockOnHand = row["RemainingStock"].ToString(),
                                        Reason = row["Reason"].ToString(),
                                        Responsible = row["Responsible"].ToString()
                                    };
                                    manualAdjustList.Add(central);

                                }
                                return manualAdjustList;
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

        //New
        public List<CentralModel> ApprovedItemDB(string Date, string Week)
        {
            DBConnection();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<CentralModel> ApprovedItemList = new List<CentralModel>();
            try
            {
                using (connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (command = new SqlCommand("Table_PopulateApprovedRequest", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@From", Week);
                        command.Parameters.AddWithValue("@To", Date);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    CentralModel central = new CentralModel()
                                    {
                                        ReferenceNo = row["ReferenceNo"].ToString(),
                                        Requester = row["RequestedBys"].ToString(),
                                        Responsible = row["Responsibles"].ToString(),
                                        Purpose = row["Purpose"].ToString(),
										DateApproved = row["CentralApprovedDate"].ToString(),
										DateRequested = row["DateCreated"].ToString(),
										RequestStatus = row["RequestStatus"].ToString(),
										ApproverID = row["Approver"].ToString(),
                                        Designation = row["Designation"].ToString(),
									};
                                    ApprovedItemList.Add(central);

                                }
                                return ApprovedItemList;
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

        public List<CentralModel> RejectedItemDB(string id,string Date, string Week)
        {
            DBConnection();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<CentralModel> RejectedItemList = new List<CentralModel>();
            try
            {
                using (connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (command = new SqlCommand("Table_PopulateRejectedRequest", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@UserID", id);
                        command.Parameters.AddWithValue("@From", Week);
                        command.Parameters.AddWithValue("@To", Date);
                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    CentralModel central = new CentralModel()
                                    {
                                        ReferenceNo = row["ReferenceNo"].ToString(),
                                        Rejectedby = row["Rejectedbys"].ToString(),
										DateRejected = row["RejectDate"].ToString(),
										Responsible = row["Responsibles"].ToString(),
										DateRequested = row["DateRequested"].ToString(),
										Requester = row["RequestedBys"].ToString(),
										RequestStatus = row["RequestStatus"].ToString(),
										RejectRemarks = row["RejectRemarks"].ToString(),
										Purpose = row["Purpose"].ToString(),
                                        Designation = row["Designation"].ToString(),
									};
                                    RejectedItemList.Add(central);

                                }
                                return RejectedItemList;
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

        public List<CentralModel> ReleasedItemDB(string groupID,string id,string Date, string Week)
        {
            DBConnection();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<CentralModel> ReleasedItemList = new List<CentralModel>();
            try
            {
                using (connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (command = new SqlCommand("Table_PopulateReleasedItem", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;  
                        command.Parameters.AddWithValue("@GroupID", groupID);
                        command.Parameters.AddWithValue("@UserID", id);
                        command.Parameters.AddWithValue("@From", Week);
                        command.Parameters.AddWithValue("@To", Date);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    CentralModel central = new CentralModel()
                                    {
                                        ReferenceNo = row["ReferenceNo"].ToString(),
                                        Requester = row["RequestedBy"].ToString(),
                                        DateReleased = row["Date_Released"].ToString(),
                                        Responsible = row["Responsible"].ToString(),
                                        ItemName = row["ItemName"].ToString(),
                                        StockNo = row["StockNo"].ToString(),
										Designation = row["Designation"].ToString(),
										Purpose = row["Purpose"].ToString(),
                                        qty = row["Qty"].ToString(),
                                        ItemReleased = row["Item_Released"].ToString(),
                                        TotalReleased = row["Total_Released"].ToString(),
                                        DateRequested = row["DateRequested"].ToString(),
                                        row = row["Row#"].ToString(),
                                    };
                                    ReleasedItemList.Add(central);
                                }
                                return ReleasedItemList;
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

        public List<CentralModel> PRReceivedDB(string groupID, string id,string Date, string Week)
        {
            DBConnection();
            SqlConnection connection = null;
            SqlCommand command = null;
            List<CentralModel> PRReceivedList = new List<CentralModel>();
            try
            {
                using (connection = new SqlConnection(con))
                {
                    connection.Open();
                    using (command = new SqlCommand("Table_PopulateReceivedPR", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@GroupID", groupID);
                        command.Parameters.AddWithValue("@UserID", id);
                        command.Parameters.AddWithValue("@From", Week);
                        command.Parameters.AddWithValue("@To", Date);

                        using (SqlDataAdapter sda = new SqlDataAdapter(command))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                foreach (DataRow row in dt.Rows)
                                {
                                    CentralModel central = new CentralModel()
                                    {
                                        row = row["row#"].ToString(),
                                        ReferenceNo = row["ReferenceNo"].ToString(),
                                        PRNo = row["PRNo"].ToString(),
                                        Requester = row["requester"].ToString(),
                                        DateReceived = row["Date_Received"].ToString(),
                                        Responsible = row["Responsible"].ToString(),
                                        Designation = row["Designation"].ToString(),
                                        Purpose = row["Purpose"].ToString(),
                                        qty = row["Qty"].ToString(),
                                        ItemReceived = row["Item_Received"].ToString(),
                                        DateRequested = row["DateRequested"].ToString(),
                                        ItemName = row["ItemName"].ToString(),
										StockNo = row["StockNo"].ToString(),
                                        TotalReceived = row["Total_Received"].ToString(),
                                    };
                                    PRReceivedList.Add(central);
                                }
                                return PRReceivedList;
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

        #region ItemListMonitoringRequestForPR
        public int InsertItemListRequestPRDB(List<CentralModel> information, CentralModel central)
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

                    foreach (CentralModel item in information)
                    {
                        using (command = new SqlCommand("Insert_RequestPRInformationGRID", connection))
                        {
                            command.CommandType = CommandType.StoredProcedure;

                            command.Parameters.AddWithValue("@ReferenceNo", ReferenceNo ?? "");
                            command.Parameters.AddWithValue("@PRNo", central.PRNo);
                            command.Parameters.AddWithValue("@StockNo", item.StockNo);
                            command.Parameters.AddWithValue("@ItemName", item.ItemName);
                            command.Parameters.AddWithValue("@Qty", item.qty);
                        }
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
        #endregion
    }
}