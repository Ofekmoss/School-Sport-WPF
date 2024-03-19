<%@ Page Title="" Language="C#" EnableViewState="false" MasterPageFile="~/Register/Register.Master" validateRequest="false" AutoEventWireup="true" CodeBehind="Articles.aspx.cs" Inherits="SportSite.NewRegister.Articles" %>
<%@ Register TagPrefix="ISF" TagName="PagedTable" Src="PagedTable.ascx" %>
<%@ Register TagPrefix="ISF" TagName="Information" Src="InfoTooltip.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
	<meta runat="server" id="metRedirect" http-equiv="refresh" content="5;URL=Articles.aspx" visible="false" />
	<script src="char-limit.js"></script>
	<script src="file-upload-progress.js"></script>
	<link href="css/bootstrap-switch.css" rel="stylesheet" />
	<style type="text/css" runat="server" id="cssArticleSubmitted" visible="false">
		form { display: none; }
	</style>
	<style type="text/css" runat="server" id="cssArticleDeleted" visible="false">
		#lbArticleAddedSuccessfully { display: none; }
		#lbArticleEditedSuccessfully { display: none; }
	</style>

	<style type="text/css" runat="server" id="cssArticleEdited" visible="false">
		#lbArticleAddedSuccessfully { display: none; }
		#lbArticleDeletedSuccessfully { display: none; }
	</style>

	<style type="text/css" runat="server" id="cssArticleAdded" visible="false">
		#lbArticleEditedSuccessfully { display: none; }
		#lbArticleDeletedSuccessfully { display: none; }
	</style>
	<link href="Articles.css" rel="stylesheet" runat="server" id="mainCssLink" visible="false" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="OutsideForm" runat="server">
	<div id="ArticleSubmittedMessage" class="panel panel-primary" style="direction: rtl;">
		<div class="panel-heading" style="padding: 15px 25px;">
			<div class="panel-title">
				<strong>
				<span class="glyphicon glyphicon-ok"></span>
				כתבה
				<span id="lbArticleAddedSuccessfully">נוספה</span>
				<span id="lbArticleDeletedSuccessfully">נמחקה</span>
				<span id="lbArticleEditedSuccessfully">נערכה</span>
				בהצלחה. טוען מחדש את רשימת הכתבות, נא להמתין...
				</strong>
				(<a href="Articles.aspx">לחץ כאן במידה והעמוד לא מתרענן אחרי כמה שניות</a>)
			</div>
		</div>
	</div>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
	<ISF:PagedTable id="tblArticles" runat="server" NewItemTitle="הוספת כתבה חדשה" NewItemUrl="?edit=new" SingularCaption="כתבה" PluralCaption="כתבות"></ISF:PagedTable>
	<button id="btnSetHomepageArticles" type="button" class="btn btn-success">
		<span class="glyphicon glyphicon-home"></span> קביעת כתבות בעמוד הבית
	</button>
	<div id="pnlAddOrEdit" runat="server" visible="false">
		<asp:HiddenField ID="hidUploadToken" runat="server" ClientIDMode="Static" />
		<asp:HiddenField ID="hidArticleId" runat="server" ClientIDMode="Static" />
		<asp:HiddenField ID="hidDeleteArticle" runat="server" ClientIDMode="Static" Value="no" />
		<asp:HiddenField ID="hidArticleImages" runat="server" ClientIDMode="Static" />
		<asp:HiddenField ID="imagesToRemove" runat="server" ClientIDMode="Static" />
		<asp:HiddenField ID="attachmentsToRemove" runat="server" ClientIDMode="Static" />
		<div class="panel panel-primary">
			<div class="panel-heading">
				<div class="panel-title">
					<strong runat="server" id="lbPanelTitle"></strong>
				</div>
			</div>
			<div class="panel-body">
				<div class="row">
					<div class="col-md-3"> <!-- col-md-offset-1 -->
						<div class="form-group">
							<div class="pull-left">
								<ISF:Information runat="server" Text="ניתן גם לכתוב בשם מישהו אחר. השם יופיע לצד תאריך פרסום הכתבה."></ISF:Information>
							</div>
							<label for="txtAuthor">מאת</label>
							<input type="text" tabindex="3" class="form-control show-remaining-chars" maxlength="50"  id="txtAuthor" runat="server" placeholder="שם" ClientIDMode="Static" value="" data-char-limit="50" />
						</div>
					</div>
					<div class="col-md-5">
						<div class="form-group">
							<label for="name">כותרת משנה</label>
							<textarea tabindex="2" class="form-control show-remaining-chars" maxlength="255" id="txtSubCaption" runat="server" cols="30" rows="3" ClientIDMode="Static" placeholder="כותרת משנה"></textarea>
						</div>
					</div>
					<div class="col-md-4">
						<div class="form-group">
							<label for="txtCaption">כותרת הכתבה</label>
							<span class="required-field">&nbsp;&nbsp;&nbsp;שדה נדרש</span>
							<input type="text" tabindex="1" class="form-control show-remaining-chars" maxlength="100" id="txtCaption" runat="server" ClientIDMode="Static" placeholder="כותרת" />
						</div>
					</div>
				</div>
				<div class="row">
					<div class="col-md-3">
						<label for="">&nbsp;</label>
						<br />
						<select id="ddlArticleRegion" name="ddlArticleRegion" class="form-control" style="display: none;"></select>
					</div>
					<div class="col-md-1">
						<label for="chkRegionalArticle">כתבה מחוזית?</label>
						&nbsp;&nbsp;
						<ISF:Information runat="server" Text="כתבה מחוזית תופיע בראש מסך האליפויות של המחוז הרצוי" DataPlacement="Top"></ISF:Information>
						<br />
						<input type="checkbox" tabindex="6" class="form-control bootstrap-switch" id="chkRegionalArticle"
							name="RegionalArticle" runat="server" ClientIDMode="Static" value="1"
							data-on-text="כן" data-off-text="לא" data-toggle-element="ddlArticleRegion" />
					</div>
					<div class="col-md-4">
						<label for="SubOrPrimary">כתבה משנית/ראשית?</label>
						&nbsp;&nbsp;
						<ISF:Information runat="server" Text="כתבה ראשית תופיע תמיד בעמוד הבית של האתר. יכולה להיות כתבה ראשית אחת בלבד.<br />עד שתי כתבות משניות יופיעו  בעמוד הבית, מתחת לכתבה הראשית." DataPlacement="Top"></ISF:Information>
						<br />
						<div class="btn-group" data-toggle="buttons">
							<label class="btn btn-default" id="lblArticle_Main" runat="server">
								<input tabindex="5" type="radio" name="SubOrPrimary" id="rbArticle_Main" runat="server" /> ראשית
							</label>
							<label class="btn btn-default" id="lblArticle_Sub" runat="server">
								<input tabindex="5" type="radio" name="SubOrPrimary" id="rbArticle_Sub" runat="server" value="" /> משנית
							</label>
						</div>
					</div>
					<div class="col-md-4">
						<label for="chkHotLink">קישור חם?</label>
						&nbsp;&nbsp;
						<ISF:Information runat="server" Text="קישור לכתבה המוגדרת קישור חם יופיע באופן  בולט בתפריט בכל העמודים באתר." DataPlacement="Top"></ISF:Information>
						<br />
						<input type="checkbox" tabindex="4" class="form-control bootstrap-switch" id="chkHotLink" 
						name="HotLink" runat="server" ClientIDMode="Static" value="1"
						data-on-text="כן" data-off-text="לא" />
					</div>
				</div>
				<div class="row">
					<div class="col-md-12">
						<label for="txtArticleContents">תוכן הכתבה</label>
						<span class="required-field">&nbsp;&nbsp;&nbsp;שדה נדרש</span>
						<textarea tabindex="7" class="form-control" id="txtArticleContents" runat="server" rows="15" ClientIDMode="Static" placeholder="תוכן"></textarea>
					</div>
				</div>
				<div class="row article-pictures-placeholder" style="margin-top: 10px;">
					<div class="col-md-3">
						<button type="button" class="pull-left btn btn-xs btn-danger remove-picture" tabindex="11">הסרה</button>
						<label for="fupFourthPicture">תמונה רביעית</label>
						<br />
						<input type="file" id="fupFourthPicture" class="form-control" tabindex="11" />
						<br />
						<div class="bg-danger invalid-picture">
							תמונה לא תקינה, נא להעלות קובץ תמונה באחד הפורמטים הבאים:<br />JPG, JPEG, GIF, PNG, BMP
						</div>
						<asp:Image ID="imgFourthPicture" ImageUrl="//:0" runat="server" CssClass="article-picture" />
					</div>
					<div class="col-md-3">
						<button type="button" class="pull-left btn btn-xs btn-danger remove-picture" tabindex="10">הסרה</button>
						<label for="fupThirdPicture">תמונה שלישית</label>
						<br />
						<input type="file" id="fupThirdPicture" class="form-control" tabindex="10" />
						<br />
						<div class="bg-danger invalid-picture">
							תמונה לא תקינה, נא להעלות קובץ תמונה באחד הפורמטים הבאים:<br />JPG, JPEG, GIF, PNG, BMP
						</div>
						<asp:Image ID="imgThirdPicture" ImageUrl="//:0" runat="server" CssClass="article-picture" />
					</div>
					<div class="col-md-3">
						<button type="button" class="pull-left btn btn-xs btn-danger remove-picture" tabindex="9">הסרה</button>
						<label for="fupSecondPicture">תמונה שנייה</label>
						<br />
						<input type="file" id="fupSecondPicture" class="form-control" tabindex="9" />
						<br />
						<div class="bg-danger invalid-picture">
							תמונה לא תקינה, נא להעלות קובץ תמונה באחד הפורמטים הבאים:<br />JPG, JPEG, GIF, PNG, BMP
						</div>
						<asp:Image ID="imgSecondPicture" ImageUrl="//:0" runat="server" CssClass="article-picture" />
					</div>
					<div class="col-md-3">
						<button type="button" class="pull-left btn btn-xs btn-danger remove-picture" tabindex="8">הסרה</button>
						<label for="fupFirstPicture">תמונה ראשונה</label>
						<br />
						<input type="file" id="fupFirstPicture" class="form-control" tabindex="8" />
						<br />
						<div class="bg-danger invalid-picture">
							תמונה לא תקינה, נא להעלות קובץ תמונה באחד הפורמטים הבאים:<br />JPG, JPEG, GIF, PNG, BMP
						</div>
						<asp:Image ID="imgFirstPicture" ImageUrl="//:0" runat="server" CssClass="article-picture" />
					</div>
				</div>
				<div class="row" style="margin-top: 10px;">
					<div id="ArticleAttachmentsPlaceholder" class="col-md-6">
						<div class="pull-left">
							<button type="button" class="btn btn-success btn-xs pull-left add-new-attachment">
								<span class="glyphicon glyphicon-plus"></span> הוספת קובץ מצורף חדש
		                    </button>
						</div>
						<div class="pull-right">
							<asp:Image ID="imgLoggedIn" runat="server" ImageUrl="~/Images/member_online.png" Visible="false" />
						</div>
						<label for="">קבצים מצורפים</label>
						<br /><br />
						<div class="row article-attachment-template" style="position: relative;">
							<input type="hidden" name="ArticleAttachmentId" class="attachment-id" />
							<input type="hidden" name="ArticleAttachmentToken" class="attachment-token" />
							<div class="col-md-1 row-actions">
								<button type="button" class="btn btn-danger btn-xs delete-attachment">
									הסרה
								</button>
							</div>
							<div class="col-md-5">
								<div class="attachment-preview">
									<a href="#" target="attachment"></a>
								</div>
								<input type="file" name="ArticleAttachmentFile" class="form-control attachment-file" style="display: inline;" />
								<div class="bg-danger file-too-big">
									ניתן להעלות קבצים עד 12 מגהבייט בלבד
								</div>
								<div class="bg-danger invalid-attachment">
									סוג קובץ שגוי, ניתן להעלות אחד מהסוגים הבאים:<br />
									<div class="allowed-attachments"></div>
								</div>
							</div>
							<div class="col-md-6">
								<input type="text" name="ArticleAttachmentDescription" class="form-control attachment-description" placeholder="תיאור הקובץ המצורף" />
							</div>
						</div>
					</div>
					<div id="ArticleLinksPlaceholder" class="col-md-6">
						<div class="pull-left">
							<button type="button" class="btn btn-success btn-xs pull-left add-new-link">
								<span class="glyphicon glyphicon-plus"></span> הוספת קישור חדש
		                    </button>
						</div>
						<label for="">קישורים</label>
						<br /><br />
						<div class="row article-link-template" style="position: relative;">
							<div class="col-md-1 row-actions">
								<button type="button" class="btn btn-danger btn-xs delete-link">
									הסרה
								</button>
							</div>
							<div class="col-md-3">
								<div class="link-preview">
									<a href="#" target="preview"></a>
								</div>
							</div>
							<div class="col-md-4">
								<input type="text" name="ArticleLinkDescription" class="form-control link-description" placeholder="תיאור הקישור" />
							</div>
							<div class="col-md-4">
								<input type="text" name="ArticleLinkUrl" class="form-control link-url" placeholder="כתובת הקישור" />
							</div>
						</div>
					</div>
				</div>
				<div id="FileUploadStatus" class="panel panel-default">
					<div class="panel-heading">
						<div class="panel-title">
						מצב התקדמות  העלאת  קבצים		
						(ניתן לשלוח כתבה אחרי שכל הקבצים עלו)
						</div>
					</div>
					<div class="panel-body">
								
					</div>
				</div>
				<div style="margin-top: 10px;">
					<asp:Button ID="btnSendArticle" runat="server" ClientIDMode="Static" CssClass="btn btn-primary btn-lg" Text="שלח כתבה" />
				</div>
				<div style="margin-top: 20px;" id="pnlDeleteArticle" runat="server" ClientIDMode="Static" visible="false">
					<button id="btnDeleteArticle" type="button" class="btn btn-danger">
						<span class="glyphicon glyphicon-trash"></span> מחק כתבה זו
					</button>
				</div>
				<div style="margin-top: 20px;" id="pnlConfirmDelete">
					<div class="panel panel-warning">
						<div class="panel-heading">
							<div class="panel-title">מחיקת כתבה</div>
						</div>
						<div class="panel-body">
							<h3>
							האם למחוק את הכתבה?<br />
								פעולה זו אינה הפיכה!
							</h3>
							<asp:Button runat="server" CssClass="btn btn-primary confirm-delete" Text="כן, בצע מחיקה" 
								OnClientClick="$('#hidDeleteArticle').val('yes');" />
							&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
							<button type="button" class="btn btn-primary abort-delete">לא, בטל מחיקה</button>
						</div>
					</div>
				</div>
			</div>
		</div>
	</div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="AfterContents" runat="server">
	<iframe id="ArticlesPreviewFrame" data-source="HomepageArticlesPreview.aspx"></iframe>
	<script src="js/bootstrap-switch.js"></script>
	<script>
		$('[data-toggle="tooltip"]').tooltip();
		$(".bootstrap-switch").bootstrapSwitch({ onSwitchChange: SwitchCheckboxChanged });
    </script>
</asp:Content>