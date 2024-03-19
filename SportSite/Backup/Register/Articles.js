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
		$("#hidArticleImages").val(ExtractPictureTokens().join(","));
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
		var previewDiv = parentRow.find(".link-preview");
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
	});

	$("#pnlConfirmDelete .abort-delete").bind("click", function () {
		deleteConfirmationPanel.fadeOut("normal", function () {
			deleteArticlePanel.fadeIn("normal");
		});
	});
}

function HandleArticleRegions() {
	if (typeof _articleRegions != "undefined") {
		var regionsDDL = $("#ddlArticleRegion");
		for (var i = 0; i < _articleRegions.length; i++) {
			var curArticleRegion = _articleRegions[i];
			regionsDDL.append($('<option>', {
				value: curArticleRegion.Id,
				text: curArticleRegion.Name
			}));
		}
		var selectedRegion = parseInt($("#chkRegionalArticle").data("region"));
		if (!isNaN(selectedRegion))
			regionsDDL.val(selectedRegion.toString());
	}
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
	HandleArticleRegions();
	HandleArticlesPreview();
	FileUploadProgress.Init($("#FileUploadStatus").find(".panel-body"));
});