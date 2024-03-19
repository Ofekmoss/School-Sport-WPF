<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="System.Collections.Generic" %>
<script runat="server" language="C#">
	public void Page_Load(object sender, EventArgs e)
	{
		Page.ClientScript.RegisterClientScriptInclude("swfobject", "JS/swfobject.js");		
		Page.ClientScript.RegisterClientScriptInclude("common", "JS/common.js");

		ddlFlashMovies.Attributes["onchange"] = String.Format("AssignDimensions('{0}', '{1}', '{2}');", ddlFlashMovies.ClientID, tbFlashWidth.ClientID, tbFlashHeight.ClientID);
		lbSaveXmlError.Visible = false;

		if (!Page.IsPostBack)
		{
			BindDropDowns();
			Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "flash_preview", String.Format("AddOnloadCommand(\"{0}\");", ddlFlashMovies.Attributes["onchange"]), true);
		}
	}

	public void BindDropDowns()
	{
		ddlFlashMovies.Items.Clear();
		ddlXmlFiles.Items.Clear();
		string strFolderPath = Server.MapPath(".");
		DirectoryInfo objFolder = new DirectoryInfo(strFolderPath);
		List<FileInfo> arrFiles = new List<FileInfo>(objFolder.GetFiles());
		arrFiles.ForEach(delegate(FileInfo objFile)
		{
			string strFileName = objFile.Name;
			if (String.Equals(Path.GetExtension(strFileName), ".swf", StringComparison.CurrentCultureIgnoreCase))
				ddlFlashMovies.Items.Add(new ListItem(strFileName, strFileName));
			else if (String.Equals(Path.GetExtension(strFileName), ".xml", StringComparison.CurrentCultureIgnoreCase))
				ddlXmlFiles.Items.Add(new ListItem(strFileName, strFileName));
		});
	}
	
	public void FlashPreview(object sender, EventArgs e)
	{
		string strFlashUrl = ddlFlashMovies.SelectedValue;
		if (!String.IsNullOrEmpty(strFlashUrl))
		{
			int width;
			int height;
			Int32.TryParse(tbFlashWidth.Text, out width);
			Int32.TryParse(tbFlashHeight.Text, out height);
			if (width < 10)
				width = 200;
			if (height < 10)
				height = 200;
			string strJS = String.Format("AddOnloadCommand(\"RegisterFlashMovie('{0}', {1}, {2}, '{3}', '', '#ffffff');\");", strFlashUrl, width, height, FlashPreviewContainer.ClientID);
			Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), "flash_preview", strJS, true);
		}
	}

	public void EditXmlFile(object sender, EventArgs e)
	{
		string strFileName = ddlXmlFiles.SelectedValue;
		if (!String.IsNullOrEmpty(strFileName))
		{
			string strFilePath = Server.MapPath(strFileName);
			if (File.Exists(strFilePath))
			{
				txtXmlFile.InnerHtml = File.ReadAllText(strFilePath, Encoding.GetEncoding(1255));
				hfXmlFile.Value = strFileName;
				ToggleXmlPanels(false);
			}
		}
	}

	public void SaveXmlFile(object sender, EventArgs e)
	{
		string strFileName = hfXmlFile.Value;
		if (String.IsNullOrEmpty(strFileName))
		{
			lbSaveXmlError.Text = "missing XML file name";
			return;
		}
		
		string strFileContents = txtXmlFile.InnerText;
		XmlDocument document = new XmlDocument();
		bool blnSuccess = true;
		try
		{
			//document.LoadXml(GetXmlContents(strFileContents));
			document.LoadXml(strFileContents);
		}
		catch (Exception ex)
		{
			blnSuccess = false;
			lbSaveXmlError.Text = "Invalid XML. Error while parsing: " + ex.Message;
			lbSaveXmlError.Visible = true;
		}

		if (blnSuccess)
		{
			string strFilePath = Server.MapPath(strFileName);
			File.WriteAllText(strFilePath, strFileContents, Encoding.GetEncoding(1255));
			ToggleXmlPanels(true);
		}
	}

	public void RestoreXmlFile(object sender, EventArgs e)
	{
		string strFileName = hfXmlFile.Value;
		if (!String.IsNullOrEmpty(strFileName))
		{
			string strFilePath = Server.MapPath(strFileName);
			txtXmlFile.InnerText = File.ReadAllText(strFilePath, Encoding.GetEncoding(1255));
		}
	}
	
	public void ToggleXmlPanels(bool blnEditMode)
	{
		pnlEditXmlFile.Visible = blnEditMode;
		pnlSaveXmlFile.Visible = !blnEditMode;
	}

	public string GetXmlContents(string strRawXml)
	{
		if (!String.IsNullOrEmpty(strRawXml))
		{
			List<string> arrLines = new List<string>(strRawXml.Split('\n'));
			if (arrLines.Count > 0 && arrLines[0].StartsWith("<?xml"))
			{
				arrLines.RemoveAt(0);
			}
			return String.Join("\n", arrLines.ToArray());
			
		}
		return String.Empty;
	}
</script>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<meta http-equiv="Content-Type" content="text/html; charset=windows-1255" />
    <title>Flash Test Center</title>
    <style type="text/css">
		body * {font-size: 12px;}
		body, div, span {line-height: 25px;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
		<asp:Panel ID="pnlLogin" runat="server" Visible="false">
			<asp:Label runat="server" Text="Password: " /><asp:TextBox ID="tbPassword" runat="server" TextMode="Password" /><br />
			<asp:Button runat="server" Text="Submit" />
		</asp:Panel>
		<asp:Panel ID="pnlContents" runat="server">
			<fieldset style="width: 800px;">
				<legend>Flash Preview</legend>
				<span>Please choose flash movie to preview:<br /></span>
				<asp:DropDownList ID="ddlFlashMovies" runat="server" /><br />
				<span>Width: <asp:TextBox ID="tbFlashWidth" runat="server" Text="200" /><br /></span>
				<span>Height: <asp:TextBox ID="tbFlashHeight" runat="server" Text="200" /><br /></span>
				<div id="FlashPreviewContainer" runat="server"></div>
				<asp:Button ID="btnFlashPreview" runat="server" OnClick="FlashPreview" Text="Preview" />
			</fieldset><br />
			<fieldset style="width: 800px;">
				<legend>Edit XML Files</legend>
				<asp:HiddenField ID="hfXmlFile" runat="server" />
				<asp:Panel ID="pnlEditXmlFile" runat="server">
					<span>Please choose file to edit:<br /></span>
					<asp:DropDownList ID="ddlXmlFiles" runat="server" /><br />
					<asp:Button ID="btnEditXmlFile" runat="server" OnClick="EditXmlFile" Text="Edit" />
				</asp:Panel>
				<asp:Panel ID="pnlSaveXmlFile" runat="server" Visible="false">
					<span>Edit file contents below:<br /></span>
					<textarea id="txtXmlFile" runat="server" cols="110" rows="30" /><br />
					<asp:Label ID="lbSaveXmlError" runat="server" ForeColor="Red" /><br />
					<asp:Button	 ID="btnSaveXmlFile" runat="server" OnClick="SaveXmlFile" Text="Save" /> 
					<asp:Button	 ID="btnRestoreXmlFile" runat="server" OnClick="RestoreXmlFile" Text="Restore" OnClientClick="return confirm('Changes you made will be ignored. Continue?');" />
				</asp:Panel>
			</fieldset><br />
		</asp:Panel>
    </div>
    </form>
</body>
</html>
