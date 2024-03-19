<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestZone.aspx.cs" Inherits="SportSite.NewRegister.TestZone" %>
<%@ Register TagPrefix="ISF" TagName="Information" Src="InfoTooltip.ascx" %>
<%@ Register TagPrefix="ISF" TagName="PagedTable" Src="PagedTable.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Test Zone</title>
	<meta runat="server" id="metRedirect" http-equiv="refresh" content="5;URL=TestZone.aspx" visible="false" />

	    <!--[if lte IE 7]>
    <script src="js/json3.min.js"></script>
    <![endif]-->

    <script src="js/jquery.min.js"></script>

    <!-- jQuery UI-->
    <link rel="stylesheet" href="css/jquery-ui.min.css">
    <script src="js/jquery-ui.js"></script>

    <!-- Bootstrap -->
    <link href="css/bootstrap/bootstrap.css" rel="stylesheet">

    <!-- Charts morris -->
    <link rel="stylesheet" href="css/morris.css">
    <script src="js/raphael-min.js"></script>
    <script src="js/morris.min.js"></script>

    <!-- sub menu -->
    <link rel="stylesheet" href="css/navbar.css">
    <script src="js/navbar.js" defer=""></script>

    <!-- flaticon css -->
    <link rel="stylesheet" type="text/css" href="css/flaticon/flaticon.css">

	<!-- custom -->
	<script src="char-limit.js"></script>
	<script src="file-upload-progress.js"></script>
	<link href="css/bootstrap-switch.css" rel="stylesheet">
	<link rel="stylesheet" type="text/css" href="Register.css">
	<link href="css/style.css" rel="stylesheet">
	<link href="css/table-view.css" rel="stylesheet">
	<link href="css/frame.css" rel="stylesheet">
	<link href="css/icons.css" rel="stylesheet">

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

    <style type="text/css" runat="server" id="cssMain">
    	#ArticleSubmittedMessage
		{
			display: none;
		}
		
        #toTop{
            position: fixed;
            bottom: 55px;
            left: 40px;
            cursor: pointer;
            display: none;
        }
        #toTop .fa {margin-right: 5px;}
        .fixed-table-toolbar .dropdown-menu li { text-align: right; }
        
        .article-pictures-placeholder .article-picture, .invalid-picture
        {
        	width: 258px;
        	height: 172px;
        	display: none;
       	}
       	
       	.file-too-big, .invalid-attachment, #pnlConfirmDelete
       	{
       		display: none;
       	}
       	
       	.required-field
       	{
       		color: Red;
       		font-weight: bold;
       		display: none;
       	}
       	
       	.article-pictures-placeholder .article-picture
       	{
			border: 0;
       	}
       	
       	.invalid-picture
       	{
       		font-weight: bold;
       		text-align: center;
       	}
       	
       	.article-pictures-placeholder .remove-picture 
       	{
       		display: none;
       	}
       	
       	.link-url
       	{
       		direction: ltr;
       	}
       	
       	.article-link, .article-attachment
       	{
       		margin-top: 10px;
       	}
       	
       	.article-link:hover .table-actions, .article-attachment:hover .table-actions
       	{
			display: block;
		}
		
		.row-actions
		{
			visibility: hidden;
		}
		
		.article-link:hover .row-actions, .article-attachment:hover .row-actions, .article-link-template:hover .row-actions, .article-attachment-template:hover .row-actions
       	{
			visibility: visible;
		}
		
		.attachment-preview
		{
			display: none;
		}
		
		.cancel-upload
		{
			margin-right: 10px;
		}
		
		#FileUploadStatus
		{
			display: none;
		}
		
		#FileUploadStatus .row
		{
			margin-top: 10px;
		}
		
		#ArticlesPreviewFrame
		{
			position: absolute;
			display: none;
			left: 300px;
			top: 100px;
			border: 3px solid black;
			margin: 0;
			width: 550px;
			height: 435px;
			background-color: White;
		}
    </style>

	<script type="text/javascript">
		var __somethingHasChanged = false;
		var __maxFileSize = 12582912; //12MB
		var __articlePictureInputs = ["fupFirstPicture", "fupSecondPicture", "fupThirdPicture", "fupFourthPicture"];
		var __articlePicturesToRemove = [];
		var __articleAttachmentsToRemove = [];

		$(window).bind("beforeunload", function (event) {
			if (__somethingHasChanged) {
				return "שינויים שבוצעו יאבדו, האם להמשיך?";
			}
		});

		function MakeSequentialNames(placeholder) {
			placeholder.find(".row").each(function () {
				var row = $(this);
				var index = row.index();
				row.find("input[type='text'],input[type='hidden']").each(function () {
					var input = $(this);
					var currentName = input.attr("name");
					input.attr("name", currentName + "_" + index);
				});
			});
		}

		function FormSubmitted(event) {
			function ExtractPictureTokens() {
				return __articlePictureInputs.map(function (elementId) {
					var fileInput = $("#" + elementId);
					var fileToken = fileInput.data("file-token");
					return fileToken || "";
				});
			}

			function AssignAttachmentTokens() {
				$(".attachment-file").each(function () {
					var fileInput = $(this);
					var parentRow = fileInput.parents(".row").first();
					parentRow.find(".attachment-token").val(fileInput.data("file-token"));
				});
			}

			var isArticleDeleted = $('#hidDeleteArticle').val() == "yes";
			if (!isArticleDeleted) {
				var form = $(this);
				if (MissingRequiredField("txtCaption") || MissingRequiredField("txtArticleContents")) {
					event.preventDefault();
					return false;
				}
				MakeSequentialNames($("#ArticleLinksPlaceholder"));
				$("#ArticleImages").val(ExtractPictureTokens().join(","));
				$("#imagesToRemove").val(__articlePicturesToRemove.join("|"));
				$("#attachmentsToRemove").val(__articleAttachmentsToRemove.join("|"));
				AssignAttachmentTokens();
				MakeSequentialNames($("#ArticleAttachmentsPlaceholder"));
			}

			$(window).unbind("beforeunload");
			return true;
		}

		function SetRequiredFieldVisibility(input, blnShow) {
			var parentDiv = input.parents("div").first();
			var requiredFieldSpan = parentDiv.find(".required-field");
			if (blnShow)
				requiredFieldSpan.show();
			else
				requiredFieldSpan.hide();
		}

		function MissingRequiredField(elementId) {
			var input = $("#" + elementId);
			var value = $.trim(input.val());
			if (value.length == 0) {
				SetRequiredFieldVisibility(input, true);
				window.scrollTo(0, 0);
				input.focus();
				return true;
			}
			return false;
		}

		function SwitchCheckboxChanged(event) {
			var checkBox = $(event.target);
			var id = checkBox.data("toggle-element") || "";
			if (id.length > 0) {
				var selected = checkBox.is(":checked");
				var elementToToggle = $("#" + id);
				if (selected)
					elementToToggle.show("slow");
				else
					elementToToggle.hide("slow");
			}
			return true;
		}

		function ExtractFileName(rawPath) {
			var splitChar = rawPath.indexOf("\\") >= 0 ? "\\" : "/"
			var temp = rawPath.split(splitChar);
			return temp[temp.length - 1];
		}

		function HandleUpload(uploadItemId) {
			var uploadToken = $("#hidUploadToken").val();
			var metaData = FileUploadProgress.GetMetaData(uploadItemId);
			var formData = new FormData();

			// Add the uploaded image content to the form data collection
			formData.append("UploadedFile", metaData.RawFile);
			formData.append("Token", uploadToken);
			
			// Make Ajax request with the contentType = false, and procesDate = false
			var ajaxRequest = $.ajax({
				type: "POST",
				url: "UploadFile.aspx",
				contentType: false,
				processData: false,
				data: formData
			});

			ajaxRequest.done(function (fileToken, textStatus) {
				if (fileToken && fileToken.length > 0) {
					FileUploadProgress.UpdateMetaData(uploadItemId, "FileToken", fileToken);
					FileUploadProgress.FileUploaded(uploadItemId);
					metaData.FileInput.data("file-token", fileToken);
				}
			});
		}

		function UploadAborted(uploadItemId) {
			var removeButton = FileUploadProgress.GetMetaData(uploadItemId).RemoveButton;
			if (removeButton != null)
				removeButton.click();
			if (!FileUploadProgress.StillInProgress()) {
				$("#FileUploadStatus").hide();
				$("input[type='submit']").show();
			}
		}

		function UploadFinished(uploadItemId) {
			if (!FileUploadProgress.StillInProgress()) {
				$("#FileUploadStatus").hide();
				$("input[type='submit']").show();
			}
		}

		function HandleArticlePictures() {
			function RemoveButtonClicked() {
				var removeButton = $(this);
				var parentBox = removeButton.parents("div").first();
				var fileInput = parentBox.find("input[type='file']");
				var articlePicture = parentBox.find(".article-picture");
				var label = parentBox.find("label");
				fileInput.val("");
				fileInput.data("file-token", "");
				fileInput.show();
				if (removeButton.data("clicked") != "true")
					__articlePicturesToRemove.push(ExtractFileName(articlePicture.attr("src")));
				articlePicture.hide();
				removeButton.hide();
				label.attr("for", label.data("for"));
				removeButton.data("clicked", "true");
				__somethingHasChanged = true;
			}

			function FileChanged() {
				var fileInput = $(this);
				var parentBox = fileInput.parents("div").first();
				var removeButton = parentBox.find(".remove-picture");
				var articlePicture = parentBox.find(".article-picture");
				var invalidPictureDiv = parentBox.find(".invalid-picture");
				invalidPictureDiv.hide();
				if (fileInput.val().length == 0) {
					removeButton.hide();
				} else {
					var rawInput = this;
					if (rawInput.files && rawInput.files[0]) {
						var fileTimer = window.setTimeout(function () {
							$("#FileUploadStatus").show();
							$("input[type='submit']").hide();
							var fileCaption = "תמונה '" + ExtractFileName(fileInput.val()) + "'";
							FileUploadProgress.AddFile(fileCaption, HandleUpload, UploadAborted, UploadFinished, {
								'RemoveButton': removeButton, 'FileInput': fileInput, 'RawFile': rawInput.files[0]
							});
							__somethingHasChanged = true;
						}, 500);
						fileInput.data("file-timer", fileTimer);
						var reader = new FileReader();
						reader.onload = function (e) {
							articlePicture.attr('src', e.target.result);
							articlePicture.show();
						}
						reader.readAsDataURL(rawInput.files[0]);
					}
					removeButton.show();
				}
			}

			$(".article-pictures-placeholder").each(function () {
				var picturesPlaceHolder = $(this);
				picturesPlaceHolder.find(".remove-picture").bind("click", RemoveButtonClicked);
				picturesPlaceHolder.find("input[type='file']").bind("change", FileChanged);
				picturesPlaceHolder.find(".article-picture").each(function () {
					var articlePicture = $(this);
					var parentBox = articlePicture.parents("div").first();
					var removeButton = parentBox.find(".remove-picture");
					var fileInput = parentBox.find("input[type='file']");
					var invalidPictureDiv = parentBox.find(".invalid-picture");
					var label = parentBox.find("label");
					if (articlePicture.attr("src") != "//:0") {
						removeButton.show();
						articlePicture.show();
						fileInput.hide();
						label.data("for", label.attr("for"));
						label.attr("for", "");
					}
					articlePicture.bind("error", function () {
						var image = $(this);
						if (image.attr("src") != "//:0") {
							invalidPictureDiv.show("slow");
							var fileTimer = parseInt(fileInput.data("file-timer"));
							if (!isNaN(fileTimer))
								window.clearTimeout(fileTimer);
							window.setTimeout(function () {
								invalidPictureDiv.fadeOut("slow");
							}, 5000);
							removeButton.click();
						}
					});
				});
			});
		}

		function ApplyPreview(previewDiv, description, url) {
			if (url.length == 0)
				url = "#";
			previewDiv.find("a").attr("href", url).text(description);
		}

		function HandleArticleLinks() {
			function AddArticleLink() {
				var linksPlaceholder = $("#ArticleLinksPlaceholder");
				var templateRow = linksPlaceholder.find(".article-link-template");
				var newLinkRow = templateRow.clone(true, true).removeClass("article-link-template").addClass("article-link");
				newLinkRow.find("input").val("");
				linksPlaceholder.append(newLinkRow);
				ApplyLinkPreview(newLinkRow.find(".link-preview"));
				return newLinkRow;
			}

			function RemoveArticleLink() {
				var removeLinkButton = $(this);
				var parentRow = removeLinkButton.parents(".row").first();
				var isTemplate = parentRow.hasClass("article-link-template");
				if (isTemplate) {
					parentRow.find("input").val("");
					ApplyLinkPreview(parentRow.find(".link-preview"));
				} else {
					parentRow.remove();
				}
				__somethingHasChanged = true;
			}

			function ApplyLinkPreview(sender) {
				var parentRow = sender.parents(".row").first();
				var  previewDiv = parentRow.find(".link-preview");
				var descriptionInput = parentRow.find(".link-description");
				var urlInput = parentRow.find(".link-url");
				ApplyPreview(previewDiv, descriptionInput.val(), urlInput.val());
			}

			var linksPlaceholder = $("#ArticleLinksPlaceholder")
			linksPlaceholder.find(".add-new-link").bind("click", AddArticleLink);
			linksPlaceholder.find(".delete-link").bind("click", RemoveArticleLink);
			linksPlaceholder.find("input").bind("keyup mouseup paste", function () {
				ApplyLinkPreview($(this));
			});

			if (typeof _articleLinks != "undefined") {
				for (var i = 0; i < _articleLinks.length; i++) {
					var articleLink = _articleLinks[i];
					var linkDescription = articleLink.Description;
					var linkUrl = articleLink.Url;
					var linkRow = (i == 0) ? linksPlaceholder.find(".article-link-template") : AddArticleLink();
					linkRow.find(".link-description").val(linkDescription);
					linkRow.find(".link-url").val(linkUrl);
					ApplyLinkPreview(linkRow.find(".link-preview"));
				}
			}
		}

		function HandleArticleAttachments() {
			function AddArticleAttachment() {
				var attachmentsPlaceholder = $("#ArticleAttachmentsPlaceholder");
				var templateRow = attachmentsPlaceholder.find(".article-attachment-template");
				var newAttachmentRow = templateRow.clone(true, true).removeClass("article-attachment-template").addClass("article-attachment");
				newAttachmentRow.find("input").show().val("");
				newAttachmentRow.find(".attachment-preview").hide();
				attachmentsPlaceholder.append(newAttachmentRow);
				return newAttachmentRow;
			}

			function ExtractExtension(fileName) {
				var extension = fileName.split(".").slice(-1)[0];
				return (extension == fileName) ? "" : extension;
			}

			function ValidateAttachmentExtension(attachmentName, fileInput, parentBox) {
				var extension = ExtractExtension(attachmentName).toLowerCase();
				if (_validAttachmentExtensions.filter(function (x) { return $.inArray(extension, x.Extensions) >= 0; }).length == 0) {
					var invalidAttachment = parentBox.find(".invalid-attachment");
					invalidAttachment.show();
					fileInput.hide();
					fileInput.val("");
					window.setTimeout(function () {
						invalidAttachment.hide("slow", function () {
							fileInput.show("slow");
						});
					}, 5000);
					return false;
				}
				return true;
			}

			function FileChanged() {
				var fileInput = $(this);
				var rawInput = this;
				var parentBox = fileInput.parents("div").first();
				var parentRow = fileInput.parents(".row").first();
				var removeButton = parentRow.find(".delete-attachment");
				var fileName = fileInput.val();
				if (fileName.length >= 0 && ValidateAttachmentExtension(fileName, fileInput, parentBox) && rawInput.files && rawInput.files[0]) {
					if (rawInput.files[0].size > __maxFileSize) {
						var tooBigSpan = parentRow.find(".file-too-big");
						tooBigSpan.show();
						fileInput.hide();
						window.setTimeout(function () {
							tooBigSpan.hide("slow", function () {
								fileInput.show();
								fileInput.val("");
							});
						}, 5000);
					} else {
						$("#FileUploadStatus").show();
						$("input[type='submit']").hide();
						var fileCaption = "קובץ מצורף '" + ExtractFileName(fileInput.val()) + "'";
						FileUploadProgress.AddFile(fileCaption, HandleUpload, UploadAborted, UploadFinished, {
							'RemoveButton': removeButton, 'FileInput': fileInput, 'RawFile': rawInput.files[0]
						});
						__somethingHasChanged = true;
					}
				}
			}

			function RemoveArticleAttachment() {
				var removeAttachmentButton = $(this);
				var parentRow = removeAttachmentButton.parents(".row").first();
				var fileInput = parentRow.find("input");
				var attachmentId = parentRow.find(".attachment-id").val();
				fileInput.data("file-token", "");
				parentRow.find(".attachment-id").val("");
				parentRow.find(".attachment-token").val("");
				if (removeAttachmentButton.data("clicked") != "true")
					__articleAttachmentsToRemove.push(attachmentId);
				var isTemplate = parentRow.hasClass("article-attachment-template");
				if (isTemplate) {
					fileInput.show().val("");
					parentRow.find(".attachment-preview").hide();
				} else {
					parentRow.remove();
				}
				removeAttachmentButton.data("clicked", "true")
				__somethingHasChanged = true;
			}

			var attachmentsPlaceholder = $("#ArticleAttachmentsPlaceholder");
			attachmentsPlaceholder.find(".add-new-attachment").bind("click", AddArticleAttachment);
			attachmentsPlaceholder.find(".delete-attachment").bind("click", RemoveArticleAttachment);
			attachmentsPlaceholder.find("input[type='file']").bind("change", FileChanged);
			if (typeof _articleAttachments != "undefined") {
				for (var i = 0; i < _articleAttachments.length; i++) {
					var articleAttachment = _articleAttachments[i];
					var attachmentDescription = articleAttachment.Description;
					var attachmentUrl = articleAttachment.Url;
					var attachmentId = articleAttachment.Id;
					var attachmentRow = (i == 0) ? attachmentsPlaceholder.find(".article-attachment-template") : AddArticleAttachment();
					attachmentRow.find(".attachment-description").val(attachmentDescription);
					attachmentRow.find(".attachment-id").val(attachmentId);
					attachmentRow.find(".attachment-file").hide();
					var previewDiv = attachmentRow.find(".attachment-preview");
					ApplyPreview(previewDiv, attachmentDescription, attachmentUrl);
					previewDiv.show();
				}
			}
		}

		function HandleDeleteArticle() {
			var deleteArticlePanel = $("#pnlDeleteArticle");
			var deleteConfirmationPanel = $("#pnlConfirmDelete");
			$("#btnDeleteArticle").bind("click", function () {
				deleteArticlePanel.fadeOut("normal", function () {
					deleteConfirmationPanel.fadeIn("normal", function () {
						window.scrollTo(0, $(window).height());
					});
				});
				//alert(_articleId);
			});

			$("#pnlConfirmDelete .abort-delete").bind("click", function () {
				deleteConfirmationPanel.fadeOut("normal", function () {
					deleteArticlePanel.fadeIn("normal");
				});
			});
		}

		function GetPagedTableIdCells() {
			var cells = [];
			var visibleRows = $(".paged-table").find("tr:visible");
			if (visibleRows.length > 1) {
				for (var i = 1; i < visibleRows.length; i++) {
					var row = visibleRows.eq(i);
					cells.push(row.find("td").eq(0));
				}
			}
			return cells;
		}

		function CloseArticlesPreview() {
			$("#ArticlesPreviewFrame").hide("slow");
			var cells = GetPagedTableIdCells();
			for (var i = 0; i < cells.length; i++) {
				var currentCell = cells[i];
				var dataContainer = currentCell.find(".data-container");
				dataContainer.draggable('disable');
				var originalContentSpan = currentCell.find(".original-content");
				dataContainer.hide();
				originalContentSpan.prependTo(currentCell);
			}
		}

		function HandleArticlesPreview() {
			function GetOrCreateDataContainer(tableCell) {
				var dataContainer = tableCell.find(".data-container");
				if (dataContainer.length == 0) {
					var existingContents = tableCell.html();
					tableCell.html("");
					dataContainer = $("<div></div>").addClass("data-container").css("z-index", "999");
					dataContainer.appendTo(tableCell);
					var labelSpan = $("<span></span>").addClass("label label-info");
					labelSpan.appendTo(dataContainer);
					var originalContentSpan = $("<span></span>").addClass("original-content").html(existingContents);
					originalContentSpan.appendTo(labelSpan);
					$("<span></span>").html("&nbsp;&nbsp;").appendTo(labelSpan);
					$("<span></span>").addClass("glyphicon glyphicon-eject").appendTo(labelSpan);
					dataContainer.draggable({
						iframeFix: true,
						iframeOffset: $("#ArticlesPreviewFrame").offset(),
						revert: true
					});
				} else {
					var originalContentSpan = tableCell.find(".original-content");
					originalContentSpan.prependTo(tableCell.find(".label-info"));
					dataContainer.show();
				}
				return dataContainer;
			}
			
			function BindDragEvents() {
				var cells = GetPagedTableIdCells();
				for (var i = 0; i < cells.length; i++) {
					var currentCell = cells[i];
					var dataContainer = currentCell.find(".data-container");
					dataContainer.unbind("dragstart");
					dataContainer.unbind("dragstop");
					dataContainer.bind('dragstart', function () {
						$("#ArticlesPreviewFrame").contents().find(".article-panel").show("slow");
					}).bind('dragstop', function () {
						$("#ArticlesPreviewFrame").contents().find(".article-panel").hide("slow");
					});
				}
			}

			function SetHomepageArticleClicked() {
				var button = $(this);
				var articlesPreviewFrame = $("#ArticlesPreviewFrame");
				var frameTop = button.offset().top;
				var frameLeft = parseInt(($(window).width() - articlesPreviewFrame.width()) / 2);
				if (frameLeft < 0)
					frameLeft = 0;
				articlesPreviewFrame.css("top", frameTop + "px").css("left", frameLeft + "px").show("slow");
				var cells = GetPagedTableIdCells();
				for (var i = 0; i < cells.length; i++) {
					var currentCell = cells[i];
					var dataContainer = GetOrCreateDataContainer(currentCell);
					dataContainer.draggable('enable');
				}
				articlesPreviewFrame.attr('src', articlesPreviewFrame.data('source')); //function (i, val) { return val; }
				event.preventDefault();
				event.stopPropagation();
				return false;
			}

			function PreviewFrameLoaded() {
				var previewFrame = $(this);
				var frameContents = previewFrame.contents();
				frameContents.find("a").attr("target", "_blank");
				frameContents.find(".article-panel").droppable({
					greedy: true,
					// tolerance can be set to 'fit', 'intersect', 'pointer', or 'touch'
					tolerance: 'touch',
					over: function (event, ui) {
						//$(event.target).css('background-color', '#cacaca');
					},
					out: function (event, ui) {
						//$(event.target).css('background-color', '');
					},
					drop: function (event, ui) {
						var articleType = $(this).data("article-type");
						var articleId = $.trim($(ui.draggable).text());
						console.log("assigning article " + articleId + " as type " + articleType + "...");
						$.post("HomepageArticlesPreview.aspx", { "action": "set", "id": articleId, "type": articleType }, function () {
							previewFrame.attr('src', previewFrame.data('source'));
						});
					}
				});
				BindDragEvents();
			}

			$("body").bind("click", CloseArticlesPreview);
			$("#btnSetHomepageArticles").bind("click", SetHomepageArticleClicked);
			$("#ArticlesPreviewFrame").bind("load", PreviewFrameLoaded);
		}

		$(document).ready(function () {
			if ($("#pnlAddNewArticle").length == 1) {
				$(".allowed-attachments").html(_validAttachmentExtensions.map(function (x) {
					return x.Extensions.length > 1 ? x.Description + " (" + x.Extensions.join(", ") + ")" : x.Description;
				}).join("<br />"));

				$('[data-toggle-element]').each(function () {
					SwitchCheckboxChanged({ target: this });
				});
				$("input,textarea").bind("change", function () {
					__somethingHasChanged = true;
				});
				$("input,textarea").bind("keyup mouseup paste", function () {
					SetRequiredFieldVisibility($(this), false);
				});
				$("form").bind("submit", FormSubmitted);
				HandleArticlePictures();
				HandleArticleLinks();
				HandleArticleAttachments();
				HandleDeleteArticle();
				FileUploadProgress.Init($("#FileUploadStatus").find(".panel-body"));
			}

			HandleArticlesPreview();
		});
	</script>
</head>
<body>
	<div id="ArticleSubmittedMessage" class="panel panel-primary" style="direction: rtl;">
		<div class="panel-heading" style="padding: 15px 25px;">
			<div class="panel-title">
				<strong>
				<span class="glyphicon glyphicon-ok"></span>
				כתבה
				<span id="lbArticleDeletedSuccessfully">נמחקה</span>
				<span id="lbArticleAddedSuccessfully">נוספה</span>
				<span id="lbArticleEditedSuccessfully">נערכה</span>
				בהצלחה. טוען מחדש את רשימת הכתבות, נא להמתין...
				</strong>
				(<a href="TestZone.aspx">לחץ כאן במידה והעמוד לא מתרענן אחרי כמה שניות</a>)
			</div>
		</div>
	</div>
    <form id="form1" runat="server">
	<asp:HiddenField ID="hidUploadToken" runat="server" ClientIDMode="Static" />
	<asp:HiddenField ID="hidDeleteArticle" runat="server" ClientIDMode="Static" Value="no" />
	<asp:HiddenField ID="ArticleLinks" runat="server" ClientIDMode="Static" />
	<asp:HiddenField ID="ArticleImages" runat="server" ClientIDMode="Static" />
	<asp:HiddenField ID="imagesToRemove" runat="server" ClientIDMode="Static" />
	<asp:HiddenField ID="ArticleAttachments" runat="server" ClientIDMode="Static" />
	<asp:HiddenField ID="attachmentsToRemove" runat="server" ClientIDMode="Static" />
    <div dir="rtl">
        <div class="navbar navbar-default navbar-fixed-top" role="navigation">
            <nav class="navbar navbar-default navbar-static-top" role="navigation" style="margin-bottom: 0">
                <div class="container-fluid">
                    <!-- Brand and toggle get grouped for better mobile display -->
                    <div class="navbar-header">
                        <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1">
                            <span class="flaticon-menu48"></span>
                        </button>
                        <a class="navbar-brand" href="../Main.aspx"><img src="images/logo.png" style="height: 50px; margin-top: -15px;" alt="logo" /></a>
                    </div>

                    <!-- Collect the nav links, forms, and other content for toggling -->
                    <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
                        <ul class="nav navbar-nav navbar-left" style="margin-right: 20px">
                            <li class="dropdown">
                                <a class="dropdown-toggle selectable non-selectable" data-toggle="dropdown" role="button2" aria-expanded="false">
                                    <span class="flaticon-user168"></span> ישראל ישראלי</asp:Literal>
                                </a>
                                <ul class="dropdown-menu" role="menu" style="text-align: right; width: 200px;">
                                    <li><div style="padding-right: 12px;">חוג בית</div>
                                        <hr style="margin-top: 6px; margin-bottom: 10px;"/>
                                    </li>
                                    <li style="padding-bottom: 6px;">
                                        <a href="../Register.aspx?interface=old" class="menu-link" style="padding-right: 14px;" id="btnLogout">
											<img src="images/exitIsf.svg" class="menu-icon" alt="logout" /> יציאה
										</a>
                                    </li>
                                </ul>
                            </li>
						    <!-- li ui-sref-active="active">
								<a href="../Register.aspx?interface=old" class="logo" style="color: Red;">
									<span class="glyphicon glyphicon-remove-sign"></span>חזרה לממשק ישן
								</a>
							</li -->
                        </ul>

                        <ul class="nav navbar-nav navbar-right" style="margin-right: 20px">
                            <li class="dropdown">
                                <a class="dropdown-toggle" data-toggle="dropdown" role="button2" aria-expanded="false">
                                    <span class="flaticon-two293"></span>פעולות<span class="caret"></span></a>
                                <ul class="dropdown-menu" role="menu" style="text-align: right; width: 200px;">
                                    <li ui-sref-active="active">
										<a href="Articles.aspx" class="logo">
											<span class="flaticon-document240"></span> ניהול כתבות
										</a>
									</li>
                                </ul>
                            </li>

                            <li ui-sref-active="active">
								<a href="Default.aspx" class="logo">
									<span class="flaticon-transport10"></span>לוח בקרה
								</a>
							</li>
                        </ul>
                    </div>
                </div>
            </nav>
        </div>

        <div id="page-wrapper">
            <br/>
			<ISF:PagedTable id="tblArticles" runat="server" NewItemTitle="הוספת כתבה חדשה" NewItemUrl="?edit=new" SingularCaption="כתבה" PluralCaption="כתבות"></ISF:PagedTable>
			<button id="btnSetHomepageArticles" type="button" class="btn btn-success">
				<span class="glyphicon glyphicon-home"></span> קביעת כתבות בעמוד הבית
			</button>
			<div runat="server" visible="false">
				<div class="pull-left">
					
				</div>
				<div class="panel panel-primary" id="pnlAddNewArticle">
					<div class="panel-heading">
						<div class="panel-title">
							<strong>הוספת כתבה חדשה</strong>
						</div>
					</div>
					<div class="panel-body">
						<div id="foobarbaz" style="z-index: 900; width: 50px; height: 50px; background-color: Black; color: White;">
							drag me
						</div>
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
								<select id="ddlArticleRegion" runat="server" class="form-control" ClientIDMode="Static" style="display: none;"></select>
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
							<asp:Button ID="btnSendArticle" runat="server" CssClass="btn btn-primary btn-lg" Text="שלח כתבה" />
						</div>
						<div style="margin-top: 20px;" id="pnlDeleteArticle">
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
        </div>

        <div id="page-footer">
            <div class="row">
                <div class="col-md-2" dir="ltr">
                    <p>&copy; MIR 2016</p>
                </div>
                <div class="col-md-10">
                    &nbsp;
                </div>
            </div>
        </div>
	</div>

	<iframe id="ArticlesPreviewFrame" data-source="HomepageArticlesPreview.aspx"></iframe>

	<!-- Include all compiled plugins (below), or include individual files as needed -->
    <script src="js/bootstrap.min.js"></script>
	<script src="js/bootstrap-switch.js"></script>
    <script>
    	$('.btn').button();
    	$('[data-toggle="tooltip"]').tooltip();
    	$(".bootstrap-switch").bootstrapSwitch({ onSwitchChange: SwitchCheckboxChanged });
    </script>

    </form>
</body>
</html>
