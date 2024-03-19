<script language="C#" runat="server">
void Page_Load(object sender, EventArgs e)
{
	string id = Request.QueryString["v"] + "";
	if (id.Length < 30)
	{
		Label1.Text = "Missing or invalid ID";
		return;
	}
	
	string error;
	Dictionary<string, string> mapping = Mir.Common.Utils.Instance.ReadMappingFile(Server.MapPath("Download.txt"), out error);
	if (error.Length > 0)
	{
		Label1.Text = "Failed reading mapping file: " + error;
		return;
	}

	if (mapping == null || mapping.Count == 0)
	{
		Label1.Text = "Invalid mapping file";
		return;
	}

	if (!mapping.ContainsKey(id))
	{
		Label1.Text = "No such key " + Server.HtmlEncode(id);
		return;
	}

	string fileName = mapping[id];
	string filePath = Server.MapPath(fileName);
	if (!System.IO.File.Exists(filePath))
	{
		Label1.Text = "Requested file '" + fileName + "' does not exist";
		return;
	}
	
	System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);
	Response.Clear();
	if (filePath.EndsWith(".exe"))
		Response.ContentType = "application/exe";
	else
		Response.ContentType = "application/octet-stream";
	Response.AddHeader("Content-Disposition", "attachment; filename=" + fileInfo.Name);
	Response.AddHeader("Content-Length", fileInfo.Length.ToString());
	Response.WriteFile(fileInfo.FullName);
	Response.Flush();
	Response.End();
}
</script>
<!DOCTYPE html>
<html>
<head><title>Downloading...</title></head><body><form id="form1" runat="server">	<asp:Label ID="Label1" runat="server"></asp:Label></form></body></html>