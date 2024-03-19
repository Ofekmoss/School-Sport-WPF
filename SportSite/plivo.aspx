<%@ Page validateRequest="false"  %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>plivo tracking</title>
	<script language="C#" runat="server">
		void Page_Load(object sender, EventArgs e)
		{
			if (HandleRequest())
			{
				Response.Write("OK");
			}
			else
			{
				Response.Write("You should not get here");
			}
			Response.End();
		}

		bool HandleRequest()
		{
			if (!Request.HttpMethod.Equals("POST", StringComparison.CurrentCultureIgnoreCase))
				return false;
			string toNumber = Request.Form["To"] + "";
			string fromNumber = Request.Form["From"] + "";
			string deliveryStatus = Request.Form["Status"] + "";
			if (toNumber.Length == 0 || deliveryStatus.Length == 0)
			{
				WriteToLog("Got blank tracking data");
				return false;
			}
			WriteToLog("Got SMS tracking data. Destincation: " + toNumber + ", source: " + fromNumber + ", status: " + deliveryStatus);
			try
			{
				return WriteToDatabase();
			}
			catch (Exception ex)
			{
				WriteToLog("General error writing to database: " + ex);
			}
			return true;
		}

		bool WriteToDatabase()
		{
			string[] requestParameters = new string[] { "To", "From", "Status", "MessageUUID", "ParentMessageUUID", "PartInfo", "Units", "TotalRate", "TotalAmount", "MNC", "MCC" };
			System.Collections.Generic.Dictionary<string, string> parameterMapping = new System.Collections.Generic.Dictionary<string, string>();
			parameterMapping.Add("To", "To_Number");
			parameterMapping.Add("From", "From_Number");
			parameterMapping.Add("Status", "Delivery_Status");
			System.Collections.Generic.List<string> fieldNames = new System.Collections.Generic.List<string>();
			foreach (string parameterName in requestParameters)
			{
				string curFieldName = parameterMapping.ContainsKey(parameterName) ? parameterMapping[parameterName] : parameterName;
				fieldNames.Add(curFieldName);
			}
			string connectionString = "data source=localhost;user id=SportFlowerService;password=sportflower123456;database=SportFlowers";
			using (System.Data.SqlClient.SqlConnection connection = new System.Data.SqlClient.SqlConnection(connectionString))
			{
				try
				{
					connection.Open();
				}
				catch (Exception ex)
				{
					WriteToLog("error opening connection: " + ex.Message);
					connection.Dispose();
					return true;
				}
				string strSQL = "Insert Into SMS_Tracking (" + string.Join(", ", fieldNames) + ", Time_Sent) Values (";
				for (int i = 0; i < requestParameters.Length; i++)
				{
					if (i > 0)
						strSQL += ", ";
					strSQL += "@p" + (i + 1);
				}
				strSQL += ", @sent)";
				using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(strSQL, connection))
				{
					for (int i = 0; i < requestParameters.Length; i++)
					{
						string currentParameterName = requestParameters[i];
						string currentValue = Request.Form[currentParameterName] + "";
						command.Parameters.Add("@p" + (i + 1), currentValue);
					}
					command.Parameters.Add("@sent", DateTime.Now);
					try
					{
						command.ExecuteNonQuery();
						WriteToLog("Inserted new record to database");
					}
					catch (Exception ex)
					{
						WriteToLog("error inserting into database: " + ex.Message);
					}
				}
				connection.Close();
			}
			return true;
		}

		void WriteToLog(string msg)
		{
			string logLine = string.Format("[{0}] {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"), msg);
			System.IO.File.AppendAllLines(Server.MapPath("plivo.txt"), new string[] { logLine });
		}
	</script>
</head>
<body>
</body>
</html>
