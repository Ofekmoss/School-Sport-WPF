<%@ Page validateRequest="false"  %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>smtp2go bounce</title>
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
			var emailRecipient = (Request.Form["rcpt"] + "").Trim().ToLower();
			var callbackEvent = Request.Form["event"] + "";
			var bounceHost = Request.Form["host"] + "";
			var bounceMessage = Request.Form["message"] + "";
			var bounceType = Request.Form["bounce"] + "";
			WriteToLog("Got a bounced email: " + emailRecipient + ", event: " + callbackEvent);
			try
			{
				return WriteToDatabase(callbackEvent, emailRecipient, bounceHost, bounceMessage, bounceType);
			}
			catch (Exception ex)
			{
				WriteToLog("General error writing to database: " + ex);
			}
			return true;
		}

		bool WriteToDatabase(string eventType, string emailRecipient, string bounceHost, string bounceMessage, string bounceType)
		{
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
				string strSQL = "Select * From BouncedEmails Where Email=@email And EventType=@event";
				using (System.Data.SqlClient.SqlCommand command = new System.Data.SqlClient.SqlCommand(strSQL, connection))
				{
					command.Parameters.Add("@email", emailRecipient);
					command.Parameters.Add("@event", eventType);
					bool exists = false;
					using (System.Data.SqlClient.SqlDataReader reader = command.ExecuteReader())
					{
						exists = reader.Read();
						reader.Close();
					}
					strSQL = (exists) ? 
							"Update BouncedEmails Set LastBounceDate=@date, BounceServer=@server, BounceMessage=@message, BounceType=@type Where Email=@email And EventType=@event" :
							"Insert Into BouncedEmails (Email, LastBounceDate, BounceServer, BounceMessage, BounceType, EventType) Values (@email, @date, @server, @message, @type, @event)";
					command.CommandText = strSQL;
					command.Parameters.Add("@date", DateTime.Now);
					command.Parameters.Add("@server", bounceHost);
					command.Parameters.Add("@message", bounceMessage);
					command.Parameters.Add("@type", bounceType);
					try
					{
						command.ExecuteNonQuery();
						string logMessage = (exists) ? "Updated existing record sucessfully" : "Inserted new record to database";
						WriteToLog(logMessage);
					}
					catch (Exception ex)
					{
						WriteToLog("error " + ((exists) ? "updating" : "inserting into") + " database: " + ex.Message);
					}
				}
				connection.Close();
			}
			return true;
		}

		void WriteToLog(string msg)
		{
			string logLine = string.Format("[{0}] {1}", DateTime.Now.ToString("dd/MM/yyyy HH:mm"), msg);
			System.IO.File.AppendAllLines(Server.MapPath("smtp2go.txt"), new string[] { logLine });
		}
	</script>
</head>
<body>
</body>
</html>
