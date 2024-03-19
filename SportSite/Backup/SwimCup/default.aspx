<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="false"  %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<script language="C#" runat="server">
</script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<meta http-equiv="Content-Type" content="text/html; charset=windows-1255" />
    <title>Swim Cup 2013</title>
	<script type="text/javascript" src="jquery-1.7.1.min.js"></script>
	<script type="text/javascript">
		var _resizeFromCode = false;
		$(document).ready(function () {
			var oLogo = $("#imgMainLogo");
			oLogo.load(function () {
				var imgWidth = $(this).width();
				var totalWidth = $("body").width();
				//alert([imgWidth, totalWidth].join("\n"));
				if (imgWidth > totalWidth) {
					_resizeFromCode = true;
					$(this).width(totalWidth - 10);
					_resizeFromCode = false;
				}
			});
			oLogo.attr("src", oLogo.attr("src"));
		});

		$(window).resize(function () {
			if (!_resizeFromCode) {
				document.location.reload();
			}
		});
	</script>
	<style type="text/css">
		#ContentsMain { text-align: center; }
		.UnderConstructionLabel { margin-top: 10px; font-size: 2em; color: Black; }
	</style>
</head>
<body>
    <form id="form1" runat="server">
    <div id="ContentsMain">
		<img id="imgMainLogo" src="SwimCupLogo.png" alt="logo" title="" />
		<div class="UnderConstructionLabel">
			The site is under Construction<br />
			For further details please call<br />
			972-3-6896136<br />
			972-3-5619080<br />
			<a href="ISF Swimming 2013 Bulletin 1 E.pdf">Download bulletin no. 1 (English version)</a><br />
			<a href="ISF Swimming 2013 Bulletin 2 E.pdf">Download bulletin no. 2 (English version)</a>
		</div>
    </div>
    </form>
</body>
</html>
