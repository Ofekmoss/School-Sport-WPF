<%@ Page language="c#" Codebehind="Register.aspx.cs" AutoEventWireup="false" Inherits="SportSite.Register" validateRequest="false" %>
<%@ Register TagPrefix="ISF" TagName="MainView" Src="Controls/MainView.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head>
		<title>התאחדות הספורט לבתי הספר בישראל - ניהול אתר</title>
		<meta http-equiv="Content-Type" content="text/html; charset=windows-1255" />
		<link rel="icon" href="favicon.ico" type="image/x-icon" />
		<link rel="shortcut icon" href="favicon.ico" />
		<script src="jquery-1.11.1.min.js" type="text/javascript"></script>

		<!-- Bootstrap -->
		<link href="bootstrap/dist/css/bootstrap.css" rel="stylesheet" />
		<link href="bootstrap/dist/css/bootstrap-datetimepicker.min.css" rel="stylesheet" />
		<link rel="stylesheet" href="bootstrap-table/src/bootstrap-table.css" />

		<script type="text/javascript" src="bootstrap/dist/js/bootstrap.min.js"></script>
		<script type="text/javascript" src="bootstrap/dist/js/bootstrap-datetimepicker.min.js" charset="UTF-8"></script>
		<script type="text/javascript" src="bootstrap/dist/js/locales/bootstrap-datetimepicker.he.js" charset="UTF-8"></script>

		<style type="text/css" media="screen">
			.printable {display: none;}
			.InputBottomOnly {text-align: center;}
		</style>		
		<style type="text/css" media="print">
			.printable {display: block;}
			.InputBottomOnly {text-align: center; border-top-style: none; border-left-style: none; border-right-style: none;}
		</style>
		<style type="text/css">
			.PageContentsPanel, .PageContentsPanel div, .PageContentsPanel span, .PageContentsPanel a, 
			.PageContentsPanel th, .PageContentsPanel td, .PageContentsPanel li, .PageContentsPanel input {
				font-size: 15px;
			}
		</style>
	</head>
	<body>
		<form id="Form1" method="post" runat="server">
			<input type="hidden" id="userSchoolID" name="userSchoolID" runat="server" />
			<input type="hidden" name="action" />
			<input type="hidden" name="TeamID" id="TeamID" runat="server" />
			<input type="hidden" name="MessageID" id="MessageID" runat="server" />
			<input type="hidden" name="pending_players_team" id="pending_players_team" runat="server" />
			<input type="hidden" name="SelectedStudent" value="" />
			<input type="hidden" name="SelectedSport" value="" />
			<input type="hidden" name="PlayerID" value="" />			
			<ISF:MainView id="IsfMainView" Runat="server"></ISF:MainView>
		</form>
		<div id="pnlChooseChampionship" runat="server" Visible="false">
			<div class="ChampChoose">
				אליפות: <input type="text" name="ChampionshipName_1" size="30" readonly="readonly" value="$name" dir="rtl" />&nbsp;&nbsp;&nbsp;
				<input type="button" name="btnChooseChamp_1" value="..." onclick="ChooseChampCategory(event, this);" />&nbsp;&nbsp;&nbsp;
				<a href="#" onclick="ClearChamp(event, this, 1); return false">[מחק]</a><br />
				<input type="hidden" name="CategoryID_1" value="$id" />
			</div>
		</div>

		<!-- Include all compiled plugins (below), or include individual files as needed -->
		<script type="text/javascript" src="bootstrap/dist/js/bootstrap.min.js"></script>
		<script type="text/javascript">
    		$('.btn').button();
	    </script>
	</body>
</html>
