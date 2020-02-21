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
	public class DBSatellite
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

		private void DBCommonReferenceNo()
		{
			DBConnection();
			using (SqlConnection connection = new SqlConnection(con))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand("Generate_CentralReferenceNo", connection))
				{
					SqlDataAdapter sda = new SqlDataAdapter(command);
					DataTable dt = new DataTable();
					sda.Fill(dt);

					SqlDataReader sdr = command.ExecuteReader();

					while (sdr.Read())
					{
						ReferenceNo = sdr["cs_ControlNumber"].ToString();
					}
					connection.Close();
				}
			}
		}

		private void DBUniqueReferenceNo(string Group,string Designation)
		{
			DBConnection();
			using (SqlConnection connection = new SqlConnection(con))
			{
				connection.Open();
				using (SqlCommand command = new SqlCommand("Generate_SatelliteReferenceNo", connection))
				{
					SqlDataAdapter sda = new SqlDataAdapter(command);
					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.AddWithValue("@GroupID", Group);
					command.Parameters.AddWithValue("@DesignationID", Designation);
					DataTable dt = new DataTable();
					sda.Fill(dt);

					SqlDataReader sdr = command.ExecuteReader();

					while (sdr.Read())
					{
						ReferenceNo = sdr["ss_ControlNumber"].ToString();
					}
					connection.Close();
				}
			}
		}

		public int InsertRequestDB(SatelliteModel satellite, string responsible,string Group,string Designation)
		{
			DBConnection();
			if (satellite.Type == "Central" || satellite.Type == "Common")
			{
				DBCommonReferenceNo();
                satellite.Type = "1";
			}
			else
			{
				DBUniqueReferenceNo(Group, Designation);
                satellite.Type = "2";
            }
			SqlConnection connection = null;
			SqlCommand command = null;
			int i = 0;

			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Insert_RequestInformation", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@Type", satellite.Type ?? "");
						command.Parameters.AddWithValue("@Designation", Designation ?? "");
						command.Parameters.AddWithValue("@Approver", satellite.ApproverID ?? "");
						command.Parameters.AddWithValue("@GroupID", Group ?? "");
						command.Parameters.AddWithValue("@ReferenceNo", ReferenceNo ?? "");
						command.Parameters.AddWithValue("@Purpose", satellite.Purpose);
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

		public int InsertItemRequestDB(List<SatelliteModel> listRequest, SatelliteModel satellite,string Group,string Designation)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			int i = 0;

			try
			{
				if (listRequest == null)
				{
					listRequest = new List<SatelliteModel>();
				}

				using (connection = new SqlConnection(con))
				{
					connection.Open();

					foreach (SatelliteModel item in listRequest)
					{
						using (command = new SqlCommand("Insert_RequestInformationGRID", connection))
						{
							command.CommandType = CommandType.StoredProcedure;

							command.Parameters.AddWithValue("@Type", satellite.Type ?? "");
							command.Parameters.AddWithValue("@Group", Group ?? "");
							command.Parameters.AddWithValue("@Designation", Designation ?? "");
							command.Parameters.AddWithValue("@ReferenceNo", ReferenceNo ?? "");
							command.Parameters.AddWithValue("@StockNo", item.StockNo);
							command.Parameters.AddWithValue("@Qty", item.Qty);
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

		public List<SatelliteModel> PopulateUniqueItemForReleasing(string Group, string Designation,string User)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
		    List<SatelliteModel> ListUniqueItem = new List<SatelliteModel>();

			try
			{
				using (connection = new SqlConnection(con))
				{
					using (command = new SqlCommand("Table_PopulateItemUniqueForReleasing", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@GroupID", Group);
						command.Parameters.AddWithValue("@UserID", User);
						command.Parameters.AddWithValue("@DesignationID", Designation);

						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);
								foreach (DataRow row in dt.Rows)
								{
									SatelliteModel list = new SatelliteModel()
									{
										ReferenceNo = row["ReferenceNo"].ToString(),
										CompanyID = row["CompanyID"].ToString(),
										Responsible = row["Responsible"].ToString(),
										DateRequested = row["DateRequested"].ToString(),
										RequestStatus = row["RequestStatus"].ToString(),
										Purpose = row["Purpose"].ToString(),
										GroupName = row["GroupID"].ToString(),
										Designation = row["DesignationID"].ToString(),
										Requester = row["RequestedBy"].ToString(),
									};
									ListUniqueItem.Add(list);
								}
								return ListUniqueItem;
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

		public List<SatelliteModel> PopulateItemUniqueForReleasingDB(string ReferenceNo)
		{
			DBConnection();
			List<SatelliteModel> listItemForReleasing = new List<SatelliteModel>();
			SqlConnection connection = null;
			SqlCommand command = null;

			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Table_PopulateAllUniqueItemRequest", connection))
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
									SatelliteModel central = new SatelliteModel()
									{
										StockNo = row["StockNo"].ToString(),
										ItemName = row["ItemName"].ToString(),
										Qty = row["Qty"].ToString(),
										StockOnHand = row["StockOnHand"].ToString(),
										Delivered = row["Delivered"].ToString(),
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

		public int UpdateItemUniqueReleasedDB(List<SatelliteModel> listRequest, SatelliteModel satellite, string responsible)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			int i = 0;

			try
			{
				if (listRequest == null)
				{
					listRequest = new List<SatelliteModel>();
				}


				using (connection = new SqlConnection(con))
				{
					connection.Open();
					foreach (SatelliteModel item in listRequest)
					{
						if (!string.IsNullOrWhiteSpace(item.ItemReleased))
						{
							using (command = new SqlCommand("Insert_ItemUniqueReleased", connection))
							{
								command.CommandType = CommandType.StoredProcedure;
								command.Parameters.AddWithValue("@ReferenceNo", satellite.ReferenceNo);
								command.Parameters.AddWithValue("@StockNo", item.StockNo);
								command.Parameters.AddWithValue("@ItemName", item.ItemName);
								command.Parameters.AddWithValue("@Qty", item.Qty);
								command.Parameters.AddWithValue("@Item_Released", item.ItemReleased);
								command.Parameters.AddWithValue("@Date_Released", satellite.DateReleased);
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

		public List<SatelliteModel> PopulateManageUniqueRequest(string GroupID, string DesignationID, string UserID, string Date, string Week)
		{
			SqlConnection connection = null;
			SqlCommand command = null;
			DBConnection();
			List<SatelliteModel> listUniqueRequest = new List<SatelliteModel>();
			try
			{
				using (connection = new SqlConnection(con))
				{
					using (command = new SqlCommand("Table_PopulateUniqueRequestStatus", connection))
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
								foreach (DataRow row in dt.Rows)
								{
									SatelliteModel satellite = new SatelliteModel()
									{
										ReferenceNo = row["ReferenceNo"].ToString(),
										DateRequested = row["DateRequested"].ToString(),
										RequestStatus = row["RequestStatus"].ToString(),
                                        Requester = row["RequestedBy"].ToString(),
                                        Responsible = row["FUllName"].ToString(),
									};
									listUniqueRequest.Add(satellite);
								}
								return listUniqueRequest;
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

		public List<SatelliteModel> PopulateAllRequestUnique(string ApproverID, string DesignationID)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			List<SatelliteModel> listRequestUnique = new List<SatelliteModel>();
			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Table_PopulateAllRequestUnique", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@Approver", ApproverID);
						command.Parameters.AddWithValue("@DesignationID", DesignationID);
						using (SqlDataAdapter sda = new SqlDataAdapter(command))
						{
							using (DataTable dt = new DataTable())
							{
								sda.Fill(dt);

								foreach (DataRow row in dt.Rows)
								{
									SatelliteModel satellite = new SatelliteModel()
									{
										ReferenceNo = row["ReferenceNo"].ToString(),
										CompanyID = row["CompanyID"].ToString(),
										Responsible = row["FullName"].ToString(),
										Designation = row["Designation"].ToString(),
										Department = row["Department"].ToString(),
										DateRequested = row["DateRequested"].ToString(),
										Purpose = row["Purpose"].ToString(),
										RequestStatus = row["RequestStatus"].ToString(),
										ApproverID = row["Approver"].ToString(),

									};

									listRequestUnique.Add(satellite);
								}
								return listRequestUnique;
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

		public List<SatelliteModel> PopulateAllItemUniqueRequest(string ReferenceNo)
		{
			DBConnection();
			SqlConnection connection = null;
			SqlCommand command = null;
			List<SatelliteModel> listitemUnqiue = new List<SatelliteModel>();
			try
			{
				using (connection = new SqlConnection(con))
				{
					connection.Open();
					using (command = new SqlCommand("Table_PopulateAllUniqueItemRequest", connection))
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
									SatelliteModel model = new SatelliteModel()
									{
										StockNo = row["StockNo"].ToString(),
										ItemName = row["ItemName"].ToString(),
										Qty = row["Qty"].ToString(),
										StockOnHand = row["StockOnHand"].ToString(),
									};

									listitemUnqiue.Add(model);
								}
								return listitemUnqiue;
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

		public int UpdateRejectRoutingApprovalUniqueDB(SatelliteModel satellite, string Responsible)
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
					using (command = new SqlCommand("Update_RejectRoutingApprovalUnique", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@ReferenceNo", satellite.ReferenceNo);
						command.Parameters.AddWithValue("@RejectRemarks", satellite.RejectRemarks);
						command.Parameters.AddWithValue("@TypeRequest", satellite.TypeRequest);
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

		public int UpdateApprovedRoutingApprovalUniqueDB(SatelliteModel satellite, string Responsible)
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
					using (command = new SqlCommand("Update_ApprovedRoutingApprovalUnique", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.AddWithValue("@ReferenceNo", satellite.ReferenceNo);
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



	}



}