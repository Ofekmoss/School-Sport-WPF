<%@ Register TagPrefix="ISF" TagName="MainView" Src="Controls/MainView.ascx" %>
<%@ Page language="c#" Codebehind="Main.aspx.cs" AutoEventWireup="false" Inherits="SportSite.Main" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
	<head runat="server">
		<title>התאחדות הספורט לבתי הספר בישראל</title>
		<meta http-equiv="Content-Type" content="text/html; charset=windows-1255" />
		<meta id="mtTitle" runat="server" name="title" content="" visible="false" />
		<meta id="mtDescription" runat="server" name="Description" content="עמוד הבית של אתר התאחדות הספורט לבתי ספר" />	
		<meta name="Keywords" content="התאחדות, ספורט, בתי ספר, לוחות משחקים, טבלאות דירוג" />		
		<link id="mtPageThumb" runat="server" visible="false" rel="image_src" href="" />
		<link rel="icon" href="favicon.ico" type="image/x-icon" />
		<link rel="shortcut icon" href="favicon.ico" />
		<style type="text/css" media="screen">
			.printable {display: none;}
		</style>		
		<style type="text/css" media="print">
			.printable {display: block;}
		</style>
		<script type="text/javascript" src="jquery-1.10.2.min.js"></script>
		<script type="text/javascript">
			$(document).ready(function () {
				YnetApplyLink();
			});

			var _ynetApplyLinkTries = 0;
			function YnetApplyLink() {
				if (_ynetApplyLinkTries >= 100)
					return false;

				var arrEmbeds = $("#MainAdvertisementFlash").find("embed");
				if (arrEmbeds.length > 0) {
					var oEmbed = arrEmbeds.first();
					var embedWidth = oEmbed.attr("width");
					var embedHeight = oEmbed.attr("height");
					var embedSrc = oEmbed.attr("src");
					if (embedSrc.indexOf("ynet_new_") >= 0) {
						var wrapper = $("<div></div>").css({ "position": "absolute", "left": "0px", "top": "0px", "z-index": "1" });
						wrapper.css("width", embedWidth + "px").css("height", embedHeight + "px").css("background-color", "transparent");
						wrapper.css("opacity", "0").css("filter", "alpha(opacity = 0)").css("cursor", "pointer");
						wrapper.click(function () {
							document.location = "http://www.yedioth.co.il/GeneralForms.aspx?gf=7&ref=sport";
						});
						wrapper.insertBefore(oEmbed);
					}
					return true;
				}

				_ynetApplyLinkTries++;
				window.setTimeout(YnetApplyLink, 100);
				return false;
			}
		</script>
	</head>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<ISF:MainView id="IsfMainView" Runat="server"></ISF:MainView>
		</form>
	</body>
</html>
