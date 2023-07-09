using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TelegarmBot_Jmenka.Data.User;
using Telegram.Bot.Types;

namespace TelegarmBot_Jmenka.Database
{
	public static class DatabaseConnector
	{
	 	private static readonly string configFilePath = "../../../dbsettings.xml";

		private static string? connectionString;
		public static string? ConnectionString { get 
			{
				if (IsConnection)
				{
					return connectionString;
				}
				else return null;
			}
			private set { connectionString = value; } }
		public static bool IsConnection { get; set; }
		public static void Connection()
		{
			string? connectToServerString = null;

			string connToServerName = "ConnectionToServer";
			string connToDatabaseName = "ConnectionToDatabase";

			using (XmlReader reader = XmlReader.Create(configFilePath))
			{
				while (reader.Read())
				{
					if (reader.NodeType == XmlNodeType.Element && reader.Name == "add")
					{
						string nameAttribute = reader.GetAttribute("name");
						if (nameAttribute == connToServerName)
						{
							connectToServerString = reader.GetAttribute("connectionString");
						}
						else if (nameAttribute == connToDatabaseName)
						{
							ConnectionString = reader.GetAttribute("connectionString");
						}
					}
				}
			}

			if (!string.IsNullOrEmpty(connectToServerString) && !string.IsNullOrEmpty(connectionString))
			{
                string databaseName = "DBcontroller";

				if (DatabaseExist(connectToServerString, databaseName))
				{

				}
				else
				{
					using (var context = new DBcontroller(connectionString))
					{
						System.Data.Entity.Database.SetInitializer(new CreateDatabaseIfNotExists<DBcontroller>());

						context.Database.Initialize(true);
					}
				}

				Console.WriteLine("connect");
				IsConnection = true;
			}
			else
			{
				IsConnection = false;
				throw new Exception("SQL server connection falied");
			}
		}

		private static bool DatabaseExist(string connectionString, string databaseName)
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				try
				{
					connection.Open();
				}
				catch { return false; }

				bool exists = false;

				string checkDatabaseQuery = $"SELECT COUNT(*) FROM sys.databases WHERE name = '{databaseName}'";
				using (SqlCommand command = new SqlCommand(checkDatabaseQuery, connection))
				{
					int count = Convert.ToInt32(command.ExecuteScalar());

					if (count > 0)
					{
						exists = true;
					}
				}

				connection.Close();

				return exists;
			}
		}
	}
}
