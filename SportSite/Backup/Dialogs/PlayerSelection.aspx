<%@ Page language="c#" Codebehind="PlayerSelection.aspx.cs" AutoEventWireup="false" Inherits="SportSite.Dialogs.PlayerSelection" %>
<!DOCTYPE html>
<html>
	<head>
	    <meta http-equiv="Content-Type" content="text/html; charset=windows-1255" />
		<title id="PageTitle">����� ������ ������</title>
		<script type="text/javascript">
			var ERR_ID_TOO_SHORT = 1;
			function ShowError(err) {
				var msg = "";
				switch (err) {
					case ERR_ID_TOO_SHORT:
						msg = "Student ID number must be at least 6 digits"; // "���� ����� ���� ���� ����� ����� 6 �����";
						break;
				}
				if (msg.length > 0)
					alert(msg);
			}
		</script>
		<style type="text/css"> .MainDiv { direction: rtl } </style>
	</head>
	<body>
		<div id="debugArea" style="display: none;">
			<textarea id="debug_textarea" rows="20" cols="10"></textarea><br />
			<input type="button" value="Evaluate" onclick="eval(document.getElementById('debug_textarea').value);" /> 
			<input type="button" value="hide" onclick="document.getElementById('debugArea').style.display = 'none';" />
		</div>
		<div class="MainDiv">
			<form id="Form1" method="post" runat="server">
				�����:
				<asp:Label ID="lblTeamName" Runat="server"></asp:Label><br>
				�����:
				<asp:DropDownList ID="ddlGrades" Runat="server" AutoPostBack="True"></asp:DropDownList>
				<input type="button" value="[����� �����]" onclick="ClearSelection(this.form.elements['selected_players']);" /><br />
				<asp:label id="Label3" runat="server">���� ����� ����: </asp:label>&nbsp;
				<asp:textbox id="txtStudentNumber" runat="server" Width="105px" MaxLength="10"></asp:textbox>&nbsp;
				<input name="btnFindStudent" type="button" onmouseup="FindStudent(this);" onmousedown="cursor_wait(this);" value="���" /><br />
				<div id="PlayersPanel" style="overflow-x: auto; overflow: scroll; width: 400px; height: 350px">
					<asp:Table ID="TblStudents" Runat="server" BorderWidth="1">
						<asp:TableRow Runat="server">
							<asp:TableHeaderCell Runat="server">����</asp:TableHeaderCell>
							<asp:TableHeaderCell Runat="server">�� �����</asp:TableHeaderCell>
							<asp:TableHeaderCell Runat="server">�� ����</asp:TableHeaderCell>
							<asp:TableHeaderCell Runat="server">���� ����� ����</asp:TableHeaderCell>
							<asp:TableHeaderCell Runat="server">��� ������</asp:TableHeaderCell>
						</asp:TableRow>
					</asp:Table>
				</div>
				<br />
				<input type="button" name="BtnConfirm" disabled value="�����" onclick="ConfirmClicked(this);" />&nbsp;&nbsp;&nbsp;
				<input type="button" name="BtnCancel" value="�����" onclick="CancelClicked(this);" />
			</form>
			<iframe name="FindStudentFrame" id="FindStudentFrame" src="about:blank"></iframe>					
			<form action="<%=Request.ServerVariables["Script_Name"]%>" name="FindStudentForm" method="get" onsubmit="return false;" target="FindStudentFrame">
				<input type="hidden" name="action" value="findstudent" />
				<input type="hidden" name="num" value="" />
			</form>
		</div>
	</body>
</html>
