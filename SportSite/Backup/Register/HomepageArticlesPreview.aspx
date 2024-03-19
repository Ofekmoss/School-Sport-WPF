<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HomepageArticlesPreview.aspx.cs" Inherits="SportSite.NewRegister.HomepageArticlesPreview" %>
<%@ Register TagPrefix="ISF" TagName="Article" Src="~/Controls/Article.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
	<meta http-equiv="Content-Type" content="text/html; charset=windows-1255" />
	<script src="js/jquery.min.js"></script>

    <!-- jQuery UI-->
    <link rel="stylesheet" href="css/jquery-ui.min.css">

    <script src="js/jquery-ui.min.js"></script>

	<!-- Bootstrap -->
    <link href="css/bootstrap/bootstrap.css" rel="stylesheet">

	<link rel="stylesheet" type="text/css" href="../SportSite.css" />
	<script src="../Common/Common.js"></script>
	<asp:Literal ID="lbOnloadJS" Runat="server"></asp:Literal>
	<style type="text/css">
		.main-caption
		{
			position: absolute;
			top: 0px;
			width: 544px;
			height: 20px;
			line-height: 20px;
			text-align: center;
			vertical-align: middle;
			font-size: 18px;
		}
		
		.ArticlePicFlashSmall
		{
			top: 0px;
		}
		#IsfExtraArticle_ArticleMainTable .ArticlePicFlashSmall
		{
			top: 120px;
		}
		.article-panel
		{
			display: none;
			position: absolute;
			right: 20px;
			width: 100px;
			height: 70px;
			line-height: 70px;
			z-index: 999;
			text-align: center;
			vertical-align: middle;
			font-weight: bold;
			color: Black;
			background-color: White;
		}
		
		.button-container, .confirm-changes
		{
			position: absolute;
			top: 385px;
			text-align: center;
		}
		
		.button-container
		{
			width: 181px;
		}
		
		.confirm-changes
		{
			display: none;
			width: 540px;
			left: 0px;
			direction: rtl;
			padding: 0;
			margin-bottom: 0px;
		}
		
		.changes-applied
		{
			position: absolute;
			display: none;
			left: 0px;
			top: 0px;
			width: 540px;
			height: 360px;
			line-height: 360px;
			text-align: center;
			vertical-align: middle;
			direction: rtl;
		}
		
		.changes-applied, .changes-applied span
		{
			font-size: 30px;
			padding: 0;
		}
		
		.changes-applied .glyphicon
		{
			color: Green;
		}
		
		.apply-changes-button
		{
			left: 0px;
		}
		
		.reset-changes-button
		{
			left: 181px;
		}
		
		.close-frame-button
		{
			left: 362px;
		}
	</style>
</head>
<body>
    <form id="form1" runat="server">
		<div class="text-primary main-caption">
			נא לגרור כתבות רצויות למיקום המתאים למטה
		</div>
		<div id="MainArticlePanel" class="ArticlePanel" style="top: 20px; left: 0px;">
			<ISF:Article id="IsfMainArticle" runat="server" OverrideLink="HomepageArticlesPreview.aspx"></ISF:Article>
		</div>
		<div id="SubArticlePanel" class="ArticlePanel" style="top: 140px; left: 0px;">
			<ISF:Article id="IsfSubArticle" runat="server" OverrideLink="HomepageArticlesPreview.aspx"></ISF:Article>
			<ISF:Article id="IsfExtraArticle" runat="server" OverrideLink="HomepageArticlesPreview.aspx"></ISF:Article>
		</div>
		<div class="alert alert-success changes-applied" role="alert">
			<span class="changes-applied-pending">
				מערכת מבצעת שינויים, נא להמתין...
			</span>
			<span class="changes-applied-done" style="display: none;">
				<span class="glyphicon glyphicon-ok"></span> שינויים בוצעו בהצלחה
			</span>
			<span class="changes-failed" style="display: none;">
				<span class="glyphicon glyphicon-exclamation-sign"></span> שגיאה בעת ביצוע שינויים
			</span>
		</div>
		<div class="alert alert-warning confirm-changes" role="alert">
			פעולה זו תשנה את עמוד הבית, נא לאשר. 
			&nbsp;&nbsp;
			<button type="button" class="btn btn-primary confirm-change-button">
				הבנתי, החל שינויים
			</button>
			&nbsp;&nbsp;
			<button type="button" class="btn btn-primary cancel-change-button">
				ביטול
			</button>
		</div>
		<div class="button-container apply-changes-button">
			<button type="button" class="btn btn-primary" disabled="disabled">
				החל שינויים בעמוד בית
			</button>
		</div>
		<div class="button-container reset-changes-button">
			<button type="button" class="btn btn-primary" disabled="disabled">
				איפוס שינויים
			</button>
		</div>
		<div class="button-container close-frame-button">
			<button type="button" class="btn btn-primary">
				סגור חלון זה
			</button>
		</div>
		<div class="article-panel" style="top: 40px;" data-article-type="1">
			שחרר כתבה כאן
		</div>
		<div class="article-panel" style="top: 160px;" data-article-type="2">
			שחרר כתבה כאן
		</div>
		<div class="article-panel" style="top: 280px;" data-article-type="3">
			שחרר כתבה כאן
		</div>
    </form>
	<script type="text/javascript">
		if (typeof _hasChanges != "undefined" && _hasChanges == true) {
			$(".apply-changes-button").find("button").prop("disabled", "");
			$(".reset-changes-button").find("button").prop("disabled", "");
		}

		$(".close-frame-button button").bind("click", function () {
			if (window.parent && window.parent.CloseArticlesPreview) {
				window.parent.CloseArticlesPreview();
			}
		});

		$(".apply-changes-button").bind("click", function () {
			$(".button-container").hide();
			$(".confirm-changes").show();
		});

		$(".cancel-change-button").bind("click", function () {
			$(".confirm-changes").hide();
			$(".button-container").show();
		});

		$(".reset-changes-button").bind("click", function () {
			$.post("HomepageArticlesPreview.aspx", { "action": "reset" }, function () {
				window.location.href = window.location.href;
			});
		});

		$(".confirm-change-button").bind("click", function () {
			$(".cancel-change-button").trigger("click");
			$(".main-caption").hide();
			$(".ArticlePanel").hide();
			$(".changes-applied").show();
			$(".apply-changes-button").hide();
			$(".reset-changes-button").hide();
			$.post("HomepageArticlesPreview.aspx", { "action": "apply" }, function () {
				$(".changes-applied-pending").hide();
				$(".changes-applied-done").show();
				window.setTimeout(function () {
					$(".close-frame-button button").trigger("click");
				}, 5000);
			}).fail(function (response) {
				//alert('Error: ' + response.responseText);
				$(".changes-applied").removeClass("alert-success").addClass("alert-danger");
				$(".changes-applied-pending").hide();
				$(".changes-failed").show();
				$(".changes-failed").find(".glyphicon").css("color", "red");
				window.setTimeout(function () {
					$(".close-frame-button button").trigger("click");
				}, 5000);
			});
		});
	</script>

    <!-- Include all compiled plugins (below), or include individual files as needed -->
    <script src="js/bootstrap.min.js"></script>
    <script>
    	$('.btn').button();
    </script>
</body>
</html>
