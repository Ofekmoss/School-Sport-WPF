var _getStudentTeamsUrlTemplate = "Register.aspx?action=GetStudentTeams&student_id_number=$idnumber";
var arrOriginalColors = new Array();
var _captionFlashCount = 0;
var _admin = 0;
var arrStudentTeams = 0;
var currentStudentTeamIndex = 0;
var studentTeamPreviewTemplate = 0;

$(document).ready(function () {
	if (CheckDoubleNames())
		return false;

	$("#lnkAddAttachmentChamp").click(function () {
		var container = $("#ChampContainer");
		var clone = container.find(".ChampChoose").eq(0).clone();
		container.append(clone); //, input[type='hidden']
		clone.find("input[type='text']").each(function () {
			this.value = "";
		});
		return false;
	});

	var txtStudentIdNumber = $("input[name='StudentIdNumber']");
	if (txtStudentIdNumber.length === 1) {
		txtStudentIdNumber.focus();
		txtStudentIdNumber.bind("keypress", function (evt) {
			var keyCode = evt.keyCode || evt.which;
			if (keyCode == 13) {
				var oButton = $(this).parents("div").eq(0).find("input[type='image']");
				if (oButton.length === 1) {
					oButton.click();
					return false;
				}
			}
		});
	}

	HandleAlternativeContents();
	HandleStudentPictures();
	HandleIdNumber();
	HandleRequiredFields();
	HandlePositions();

	$(".HideFooter").bind("mouseover", function () {
		$("#FooterPanel").hide();
	}).bind("mouseout", function () {
		$("#FooterPanel").show();
	});
});

function HandleAlternativeContents() {
	var pnlAlternativeContents = $("#AlternativeContentsPanel");
	var pnlPageContents = $("#PageContentsPanel");
	if (pnlAlternativeContents.length == 1 && pnlPageContents.length == 1) {
		var pnlFooter = $("#FooterPanel");
		var rawAlternativeContents = $.trim(pnlAlternativeContents.html());
		if (rawAlternativeContents.length > 0) {
			arrClonedStyles = ["position", "left", "top", "width"];
			for (var i = 0; i < arrClonedStyles.length; i++) {
				var currentCssProp = arrClonedStyles[i];
				pnlAlternativeContents.css(currentCssProp, pnlPageContents.css(currentCssProp));
			}
			pnlPageContents.hide();
			pnlAlternativeContents.show();
			$(".DashboardPanel").find("button").bind("click", function () {
				var href = $(this).data("href") || "";
				if (href.length > 0)
					window.location.href = href;
			});
			
			$("#ArticleSearchPanel").find(".dismiss").bind("click", function () {
				var searchPanel = $(this).parents("div").first();
				var oButton = searchPanel.parents("div").first().find("button");
				searchPanel.hide("slow");
				oButton.show("slow");
			});

			$('.form_date').datetimepicker({
				language: 'he',
				format: 'dd/mm/yyyy',
				isRTL: true,
				autoclose: 1,
				minView: 2,
				maxView: 3,
				todayHighlight: 1,
				forceParse: 0
			}).on('changeDate', function (ev) {
				var oPicker = $(this).find(".datetimepicker");
				var oTextbox = $(this).find("input[type='text']").first();
				var date = new Date(ev.date);
				var formatted = AddZero(date.getDate()) + "/" + AddZero(date.getMonth() + 1) + "/" + date.getFullYear();
				oTextbox.data("timestamp", ev.date);
				oTextbox.val(formatted);
				oPicker.hide();
			});

			$(document).bind("click", function (event) {
				if ($(event.target).parents(".form_date").length == 0) {
					$(".datetimepicker").hide();
				}
			});

			window.setTimeout(function () {
				var contentsBottom = pnlAlternativeContents[0].offsetTop + pnlAlternativeContents[0].offsetHeight;
				if (contentsBottom > pnlFooter.position().top) {
					var oNavPanel = $("#LeftTableCell");
					var oMainBody = $("#MainViewBody");
					var mainHeight = contentsBottom + pnlFooter.height();
					pnlFooter.css("top", contentsBottom + "px");
					oNavPanel.css("height", mainHeight + "px");
					oMainBody.css("height", mainHeight + "px");
				} else {
					var contentsHeight = parseInt(pnlFooter.css("top")) - parseInt(pnlAlternativeContents.css("top"));
					if (!isNaN(contentsHeight) && contentsHeight > 0)
						pnlAlternativeContents.css("height", contentsHeight + "px");
				}
			}, 500);

			window.setTimeout(function () {
				$(".datetimepicker").hide();
				$('.form_date').find("input[type='text']").bind("focus", function () {
					var oPicker = $(this).parents("div").first().find(".datetimepicker");
					oPicker.show();
				});
			}, 100);
		}
	}
}

function AddZero(num) {
	return (num < 10 && num >= 0) ? "0" + num : num + "";
}

function UpdateExistingArticle(sender) {
	var oButton = $(sender);
	var oSearchPanel = $("#ArticleSearchPanel");
	oButton.hide("slow");
	oSearchPanel.show("slow");
}

function HandlePositions() {
	var arrBigMenuContainers = $(".big_menu_link_container");
	var oResultsPanel = $("#ResultsConfirmationPanel");
	var oMessagesPanel = $("#UserMessagesPanel");
	if (arrBigMenuContainers.length > 0 && oResultsPanel.length == 1) {
		var lastContainer = arrBigMenuContainers.last();
		var bottom = parseInt(lastContainer.css("top")) + lastContainer.height();
		var resultsTop = bottom + 10;
		oResultsPanel.css("position", "absolute").css("top", resultsTop + "px");
		if (oMessagesPanel.length == 1) {
			var currentTop = parseInt(oMessagesPanel.css("top"));
			var resultsHeight = oResultsPanel.height();
			oMessagesPanel.css("top", (currentTop + resultsHeight + 5) + "px");
		}
	}
}

function HandleRequiredFields() {
	var oActionPanel = $(".VisitorActionsPanel");
	if (oActionPanel.length == 0)
		return;
	oActionPanel.hide();
	if (typeof SHOW_PRINT_BUTTON != 'undefined' && SHOW_PRINT_BUTTON == true) {
		oActionPanel.show();
		$("#pnlNoNeedScreenshot").hide();
	}
	var oSubmitButton = $(".DefaultSubmitButton");
	var oRequiredNoticePanel = $("<div></div>").attr("id", "VisitorActionsPanel_required_notice").css({
		"border": "2px solid black", "padding": "10px", "text-align": "center", "width": "350px" });
	var oSpan = $("<span></span>").html("יש למלא שדות דרושים לפני שניתן להמשיך ברישום:");
	oSpan.css({ "color": "red", "font-weight": "bold" });
	oRequiredNoticePanel.append(oSpan);
	var oRequiredFieldsPlaceholder = $("<span></span>").attr("class", "required_fields_placeholder");
	oRequiredFieldsPlaceholder.css({ "font-weight": "bold" });
	oRequiredNoticePanel.append(oRequiredFieldsPlaceholder);
	oActionPanel.parent().append(oRequiredNoticePanel);
	oRequiredNoticePanel.hide();

	var requiredFields = [];
	var numericGroups = [];
	$("div.items-container").each(function () {
		var container = $(this);
		if (container.attr("required-fields")) {
			container.find("input[type='text']").each(function () {
				requiredFields.push($(this));
			});
		}
	});
	$("table").each(function () {
		var container = $(this);
		if (container.attr("required-table")) {
			var fields = [];
			container.find("input[type='text']").each(function () {
				fields.push($(this));
			});
			if (fields.length > 0)
				numericGroups.push(fields);
		}
	});

	if (requiredFields.length > 0 || numericGroups.length > 0) {
		for (var i = 0; i < requiredFields.length; i++) {
			var curField = requiredFields[i];
			curField.css("background-color", "#FFE5E5");
			curField.attr("title", "שדה דרוש");
		}
		var verifyRequiredFields = function () {
			var blnShow = true;
			var emptyFieldNames = ["<br />"];
			for (var i = 0; i < requiredFields.length; i++) {
				var curField = requiredFields[i];
				if (curField.val().length == 0) {
					blnShow = false;
					var oLabel = $("label[for='" + curField.attr("id") + "']");
					if (oLabel.length == 1) {
						var labelText = oLabel.html();
						var firstLabelText = curField.parents(".items-container").first().find("label").first().html();
						if (firstLabelText != labelText)
							labelText += " " + firstLabelText.replace("שם ", "");
						emptyFieldNames.push(labelText);
					}
				}
			}

			for (var i = 0; i < numericGroups.length; i++) {
				var curGroup = numericGroups[i];
				var numberExists = false;
				for (var j = 0; j < curGroup.length; j++) {
					var curValue = parseInt(curGroup[j].val(), 10);
					if (!isNaN(curValue) && curValue > 0) {
						numberExists = true;
						break;
					}
				}

				if (!numberExists) {
					blnShow = false;
					emptyFieldNames.push("<span color=\"red\">רישום לפחות קבוצה אחת</span>");
					break;
				}
			}

			if (blnShow) {
				oRequiredNoticePanel.hide();
				oSubmitButton.show();
			} else {
				oSubmitButton.hide();
				oRequiredNoticePanel.show();
				oRequiredFieldsPlaceholder.html(emptyFieldNames.join("<br />"));
			}
		};

		window.setInterval(verifyRequiredFields, 2000);
		verifyRequiredFields();
	}
}

function CheckDoubleNames() {
	var mapping = {};
	var dupeFound = false;
	$("input[type='text']").each(function() {
		var curName = $(this).attr("name");
		if (curName && curName.length > 0)
			mapping[curName] = true;
	});
	$("input[type='hidden']").each(function() {
		var curName = $(this).attr("name");
		if (curName && curName.length > 0 && mapping[curName])
			dupeFound = true;
	});

	if (dupeFound) {
		document.location.href = document.location.href;
		return true;
	}

	return false;
}

function HandleIdNumber() {
	var oInput = $("input[name='StudentIdNumber']");
	if (oInput.length === 1 && oInput.attr("type") === "text") {
		var strLocation = document.location.href + "";
		if (strLocation.indexOf("&idnumber=") > 0) {
			var arrTemp = strLocation.split("&idnumber=");
			var idNumber = arrTemp[1] || "";
			if (idNumber.length > 0 && !isNaN(parseInt(idNumber, 10))) {
				oInput.val(idNumber);
				var oForm = oInput.parents("form").first();
				var strAction = oForm.attr("action");
				strAction = strAction.replace("&idnumber=" + idNumber, "");
				oForm.attr("action", strAction);
				oForm.submit();
			}
		}
	}
}

function HandleStudentPictures() {
	var arrStudentPictures = $(".StudentPicturePreview");
	if (arrStudentPictures.length === 0)
		return;

	window.setTimeout(function () {
		arrStudentPictures.each(function () {
			ApplyStudentPicture($(this));
		});
	}, 1000);

	arrStudentTeams = $(".StudentTeamsPreview");
	if (arrStudentTeams.length > 0)
		ApplyStudentTeam();

	$(".ShowStudentPicture").bind("click", function () {
		var actualSrc = $(this).data("actualsrc") || "";
		if (actualSrc.length > 0) {
			var oLink = $(".StudentPicturePreview").first().parents("a").first().clone();
			var oImage = oLink.find("img");
			oLink.attr("href", actualSrc);
			oImage.data("actualsrc", actualSrc);
			$(this).replaceWith(oLink);
			oImage.show();
			ApplyStudentPicture(oImage);
		} else {
			alert("ERROR: missing actual src");
		}
	});

	$(".ShowStudentTeams").bind("click", function () {
		var idNumber = ($(this).data("idnumber") || "") + "";
		if (idNumber.length > 0) {
			if (studentTeamPreviewTemplate) {
				var oImage = studentTeamPreviewTemplate.clone();
				oImage.data("idnumber", idNumber);
				$(this).replaceWith(oImage);
				oImage.show();
				arrStudentTeams = $(".StudentTeamsPreview");
				currentStudentTeamIndex = 0;
				ApplyStudentTeam();
			} else {
				alert("ERROR: missing template");
			}
		} else {
			alert("ERROR: missing id number");
		}
	});
}

function ApplyStudentPicture(oImage) {
	var actualSrc = oImage.data("actualsrc");
	if (actualSrc && actualSrc.length > 0) {
		window.setTimeout(function () {
			oImage.attr("src", actualSrc);
		}, 10);
	} else {
		oImage.hide();
	}
}

function ApplyStudentTeam() {
	if (!arrStudentTeams || currentStudentTeamIndex >= arrStudentTeams.length)
		return;

	var oImage = arrStudentTeams.eq(currentStudentTeamIndex);
	if (!studentTeamPreviewTemplate)
		studentTeamPreviewTemplate = oImage.clone();
	var idNumber = (oImage.data("idnumber") || "") + "";
	if (idNumber && idNumber.length > 0) {
		window.setTimeout(function () {
			$.get(_getStudentTeamsUrlTemplate.replace("$idnumber", idNumber), function (rawResponse) {
				var strContents = "";
				var blnSuccess = false;
				try {
					var response = $.parseJSON(rawResponse);
					if (response["success"] == "true") {
						blnSuccess = true;
						strContents = eval(response["teams"]).join("<br />");
					} else {
						strContents = "Error: " + response["error"];
					}
				}
				catch (err) {
					strContents = "General error loading teams";
				}
				var oSpan = $("<span></span>").html(strContents);
				if (blnSuccess == false)
					oSpan.css("color", "red").css("font-weight", "bold");
				oImage.replaceWith(oSpan);
				currentStudentTeamIndex++;
				ApplyStudentTeam();
			});
		}, 10);
	} else {
		oImage.hide();
		currentStudentTeamIndex++;
		ApplyStudentTeam();
	}
}

function AssignCompetitorsCheckboxes() {
	var objTable = document.getElementById("CompetitionCompetitors");
	for (var i = 0; i < objTable.rows.length; i++) {
		var row = objTable.rows[i];
		for (var j = 0; j < row.cells.length; j++) {
			var cell = row.cells[j];
			for (var k = 0; k < cell.childNodes.length; k++) {
				var node = cell.childNodes[k];
				if (node.nodeName.toLowerCase() == "input") {
					var type = node.type.toLowerCase();
					var strFuncName = "";
					if (type == "checkbox") {
						strFuncName = "CompetitorCheckboxClick";
						if ((_admin == 1) && (node.checked))
							cell.style.backgroundColor = "blue";
					}
					else if (type == "radio")
						strFuncName = "CompetitorRadioClick";
					if (strFuncName.length > 0)
						node.onclick = new Function(strFuncName + "(this);");
				}
			}
		}
	}
}

function CompetitorCheckboxClick(objCheckbox) {
	var objCell = FindAncestor(objCheckbox, "td");
	if (_admin == 1)
		objCell.style.backgroundColor = (objCheckbox.checked) ? "blue" : "white";
	if (objCheckbox.checked)
		return;
	for (var i = 0; i < objCell.childNodes.length; i++) {
		var node = objCell.childNodes[i];
		if (node.nodeName.toLowerCase() == "input") {
			if (node.type == "radio")
				node.checked = false;
		}
	}
}

function CompetitorRadioClick(objRadio) {
	var objCell = FindAncestor(objRadio, "td");
	for (var i = 0; i < objCell.childNodes.length; i++) {
		var node = objCell.childNodes[i];
		if (node.nodeName.toLowerCase() == "input") {
			if (node.type == "checkbox") {
				node.checked = true;
				break;
			}
		}
	}
}

function CheckDeleteImages(objButton) {
	var arrCheckBoxes = GetFormElements(objButton.form, "checkbox", "delete_");
	var count = 0;
	for (var i = 0; i < arrCheckBoxes.length; i++) {
		var oCheckBox = arrCheckBoxes[i];
		var arrTemp = oCheckBox.name.split("_");
		var nIndex = parseInt(arrTemp[1]);
		if (!isNaN(nIndex) && oCheckBox.checked)
			count++;
	}
	if (count > 0)
		return confirm(_confirmDeleteImage.replace("%num", count + ""));

	return true;
}

function DeleteImageClick(objCheckbox) {
	var node = objCheckbox.parentNode;
	var element = 0;
	while (node) {
		if (node.getAttribute("alt_color")) {
			element = node;
			break;
		}
		node = node.parentNode;
	}
	if (element) {
		var color = (objCheckbox.checked) ? element.getAttribute("alt_color") : "";
		element.style.backgroundColor = color;
	}
}

function ValidateAdvertisements(objButton) {
	var objForm = objButton.form;
	var count = 0;
	for (var i = 0; i < objForm.elements.length; i++) {
		var element = objForm.elements[i];
		if ((element.type == "checkbox") && (element.name == "RemoveFile"))
			count += (element.checked) ? 1 : 0;
	}
	if (count > 0)
		return confirm(_adConfirmRemoveMsg.replace("%c", count + ""));
	return true;
}

function GetSelectedText(strComboName) {
	var objCombo = document.forms[0].elements[strComboName];
	var selIndex = objCombo.selectedIndex;
	if (selIndex < 0)
		return "";
	return objCombo.options[selIndex].text;
}

function ApplyReportDetails() {
	var objSpan = document.getElementById("EventReportsDetails");
	var strHTML = "";
	strHTML += GetSelectedText("start_day") + "/";
	strHTML += GetSelectedText("start_month") + "/";
	strHTML += GetSelectedText("start_year") + " - ";
	strHTML += GetSelectedText("end_day") + "/";
	strHTML += GetSelectedText("end_month") + "/";
	strHTML += GetSelectedText("end_year") + ", ";
	strHTML += GetSelectedText("sport") + ", ";
	strHTML += GetSelectedText("championship") + ", ";
	strHTML += GetSelectedText("category");
	objSpan.innerHTML = strHTML;
}

function AjaxComboChanged(objCombo, strPage, strKey, strCallBack) {
	var selIndex = objCombo.selectedIndex;
	if (selIndex >= 0) {
		var value = objCombo.options[selIndex].value;
		SendAjaxRequest(_serverURL + "/" + strPage + "?ajax=1&" + strKey + "=" + value +
			"&region=" + _region, strCallBack);
	}
}

function SportChanged(objCombo) {
	AjaxComboChanged(objCombo, "Register.aspx", "sport", "AfterSportChanged");
}

function ChampChanged(objCombo) {
	AjaxComboChanged(objCombo, "Register.aspx", "champ", "AfterChampChanged");
}

function ApplyEntityResponse(strData, comboName) {
	var arrData = strData.split("*");
	var objCombo = document.forms[0].elements[comboName];
	while (objCombo.options.length > 1)
		objCombo.removeChild(objCombo.options[objCombo.options.length - 1]);
	for (var i = 0; i < arrData.length; i++) {
		var arrTemp = arrData[i].split("|");
		var strValue = arrTemp[0];
		var strText = "";
		if (arrTemp.length > 0)
			strText = arrTemp[1];
		var option = new Option();
		option.value = strValue;
		option.text = strText;
		objCombo.options.add(option);
	}
}

function AfterSportChanged(strResponse) {
	ApplyEntityResponse(strResponse, "championship");
}

function AfterChampChanged(strResponse) {
	ApplyEntityResponse(strResponse, "category");
}

function SelectSubGroup(strSubGroup) {
	var objGroupCombo = document.getElementById("GroupNameCombo");
	var objSubGroupCombo = document.getElementById("SubGroupCombo");
	GalleryGroupChanged(objGroupCombo);
	objSubGroupCombo.selectedIndex = 0;
	for (var i = 1; i < objSubGroupCombo.options.length; i++) {
		if (objSubGroupCombo.options[i].value == strSubGroup) {
			objSubGroupCombo.selectedIndex = i;
			break;
		}
	}
}

function GalleryGroupChanged(objGroupsCombo) {
	var selIndex = objGroupsCombo.selectedIndex;
	var objSubGroupsCombo = document.getElementById("SubGroupCombo");
	while (objSubGroupsCombo.options.length > 1)
		objSubGroupsCombo.removeChild(objSubGroupsCombo.options[1]);
	if (selIndex < 1)
		return false;
	for (var i = 0; i < _arrSubGroups[selIndex - 1].length; i++) {
		var curValue = _arrSubGroups[selIndex - 1][i];
		AddComboOption(objSubGroupsCombo, curValue, curValue);
	}
	return true;
}

function CaptionKeyUp(objTextbox) {
	var objPanel = document.getElementById("CaptionPreviewPanel");
	var strText = objTextbox.value;
	var strContainerID = "CaptionPreviewFlash_" + _captionFlashCount;
	var objContainer = document.getElementById(strContainerID);
	var objDiv = document.createElement("div");
	var strMovie = GetCaptionFlash();
	if (objContainer)
		objPanel.removeChild(objContainer);
	_captionFlashCount++;
	var strContainerID = "CaptionPreviewFlash_" + _captionFlashCount;
	objDiv.id = strContainerID;
	objPanel.appendChild(objDiv);
	strText = ReplaceGlobal(strText, "\"", "{amp}quot;");
	RegisterFlashMovie(strMovie, 380, 30, strContainerID, "txt=" + strText, "#ffffff");
}

function MoveSelectedItem(objButton, elementName, moveBy, strHiddenField, strDelimeter) {
	var objCombo = objButton.form.elements[elementName];
	var objHiddenText = objButton.form.elements[strHiddenField];
	var selIndex = objCombo.selectedIndex;
	if ((moveBy != 1) && (moveBy != -1))
		return false;
	if (selIndex < 0)
		return false;
	if ((selIndex == 0) && (moveBy == -1))
		return false;
	if ((selIndex == (objCombo.options.length - 1)) && (moveBy == 1))
		return false;
	ReplaceComboOptions(objCombo, selIndex, selIndex + moveBy);
	objCombo.selectedIndex = (selIndex + moveBy);
	var strNewOrder = "";
	for (var i = 0; i < objCombo.options.length; i++) {
		var curValue = objCombo.options[i].value;
		var curText = objCombo.options[i].text;
		var strToAdd = (curValue.length > 0) ? curValue : curText;
		strNewOrder += strToAdd;
		if (i < (objCombo.options.length - 1))
			strNewOrder += strDelimeter;
	}
	objHiddenText.value = strNewOrder;
	return true;
}

function ReplaceComboOptions(objCombo, index1, index2) {
	var strTemp = "";

	strTemp = objCombo.options[index2].value;
	objCombo.options[index2].value = objCombo.options[index1].value;
	objCombo.options[index1].value = strTemp;

	strTemp = objCombo.options[index2].text;
	objCombo.options[index2].text = objCombo.options[index1].text;
	objCombo.options[index1].text = strTemp;
}

function MoveSelection(objButton, elementFrom, elementTo) {
	var objComboFrom = objButton.form.elements[elementFrom];
	var objComboTo = objButton.form.elements[elementTo];
	var selIndex = objComboFrom.selectedIndex;
	var objHiddenText = objButton.form.elements["AttachmentsOrder"];
	if (selIndex < 0)
		return false;
	var selOption = objComboFrom.options[selIndex];
	if (objHiddenText.value.length > 0)
		objHiddenText.value += ",";
	objHiddenText.value += selOption.value;
	var newOption = new Option();
	newOption.value = selOption.value;
	newOption.text = selOption.text;
	objComboTo.options.add(newOption);
	//objComboTo.appendChild(newOption);
	return true;
}

function ClearSelection(objButton, elementName) {
	var objCombo = objButton.form.elements[elementName];
	var objHiddenText = objButton.form.elements["AttachmentsOrder"];
	objHiddenText.value = "";
	while (objCombo.options.length > 0)
		objCombo.removeChild(objCombo.options[0]);
}

function TogglePlayersCheckboxes(objClickedCheckbox) {
	var strCheckedID = objClickedCheckbox.id;
	var blnChecked = objClickedCheckbox.checked;
	var arrTemp = strCheckedID.split("_");
	var champID = "-1";
	if (arrTemp.length == 3)
		champID = arrTemp[2];
	var arrElements = document.getElementsByTagName("input");
	for (var i = 0; i < arrElements.length; i++) {
		var element = arrElements[i];
		if (element.type == "checkbox") {
			var objCheckbox = element;
			var strCurID = objCheckbox.id;
			if ((typeof strCurID != "undefined") && (strCurID.length > 0)) {
				arrTemp = strCurID.split("_");
				if ((arrTemp.length == 3) && ((arrTemp[0] == "player") || (arrTemp[0] == "team"))) {
					if ((champID < 0) || (champID == arrTemp[1]))
						objCheckbox.checked = blnChecked;
				} //end if valid ID for team checkbox
			} //end if checkbox has any ID
		} //end if element is checkbox
	} //end loop over elements
} //end function TogglePlayersCheckboxes

function ShowPlayersRegisterForm(teamID) {
	window.open("Register.aspx?action=ShowPlayersRegisterForm&team=" + teamID, "_blank");
}

function CellClick(event, objCheckBox) {
	var objParentRow = FindAncestor(objCheckBox, "tr");
	var objParentTable = FindAncestor(objCheckBox, "table");
	var objActionsRow = FindTableRow(objParentTable, "ActionsRow");
	if (objCheckBox.checked) {
		if (objParentRow) {
			arrOriginalColors[H(objParentRow)] = objParentRow.style.backgroundColor;
			objParentRow.style.backgroundColor = "yellow";
		}
	}
	else {
		if (objParentRow) {
			objParentRow.style.backgroundColor = arrOriginalColors[H(objParentRow)];
		}
	}

	var objGroup = GetGroup(objCheckBox.form, objCheckBox.name);
	var checkedCount = 0;
	for (var i = 0; i < objGroup.length; i++) {
		checkedCount += (objGroup[i].checked) ? 1 : 0;
	}
	var objButton = FindByRealName(objCheckBox.form, "btnCommitTeams");
	if (objButton)
		objButton.disabled = (checkedCount > 0);
	/*
	objButton=FindByRealName(objCheckBox.form, "btnCommitPlayers");
	if (objButton)
	objButton.disabled = (checkedCount > 0);		
	*/
	//alert(objGroup.checked);
	if (objActionsRow)
		objActionsRow.style.display = (checkedCount > 0) ? "" : "none";
}

function DeleteTeamsClick(objButton) {
	var objGroup = GetGroup(objButton.form, "selected_teams");
	var checkedCount = 0;
	for (var i = 0; i < objGroup.length; i++) {
		checkedCount += (objGroup[i].checked) ? 1 : 0;
	}

	if (checkedCount > 0) {
		return confirm(String_Grid["confirmDeletePendingTeams"]);
	}

	return true;
}

function DeletePlayersClick(objButton) {
	var objGroup = GetGroup(objButton.form, "selected_players");
	var checkedCount = 0;
	for (var i = 0; i < objGroup.length; i++) {
		checkedCount += (objGroup[i].checked) ? 1 : 0;
	}

	if (checkedCount > 0) {
		return confirm(String_Grid["confirmDeletePendingPlayers"]);
	}

	return true;
}

function BindCheckBoxes(objMasterBox, strGroupName, strUndoButtonId) {
	var objCheckboxGroup = GetGroup(objMasterBox.form, strGroupName);
	for (var i = 0; i < objCheckboxGroup.length; i++) {
		var blnCurrentState = objCheckboxGroup[i].checked;
		if (blnCurrentState != objMasterBox.checked) {
			objCheckboxGroup[i].setAttribute("original_checked", blnCurrentState + "");
			objCheckboxGroup[i].checked = objMasterBox.checked;
			if (typeof objCheckboxGroup[i].onclick != "undefined" && objCheckboxGroup[i].onclick)
				objCheckboxGroup[i].onclick();
		}
	}

	if (typeof strUndoButtonId != "undefined" && strUndoButtonId.length > 0)
		document.getElementById(strUndoButtonId).style.display = "";
}

function UndoCheckBoxesBind(objUndoButton, strGroupName, strMasteBoxId) {
	var objCheckboxGroup = GetGroup(objUndoButton.form, strGroupName);
	for (var i = 0; i < objCheckboxGroup.length; i++) {
		var strOriginalState = objCheckboxGroup[i].getAttribute("original_checked");
		if (strOriginalState) {
			var blnOriginalState = (strOriginalState == "true");
			if (objCheckboxGroup[i].checked != blnOriginalState) {
				objCheckboxGroup[i].checked = blnOriginalState;
				if (typeof objCheckboxGroup[i].onclick != "undefined" && objCheckboxGroup[i].onclick)
					objCheckboxGroup[i].onclick();
			}
		}
	}

	objUndoButton.style.display = "none";

	if (strMasteBoxId && strMasteBoxId.length > 0) {
		var objMasterBox = document.getElementById(strMasteBoxId);
		if (objMasterBox)
			objMasterBox.checked = !objMasterBox.checked;
	}
}

function FindByRealName(objForm, name) {
	for (var i = 0; i < objForm.elements.length; i++) {
		var element = objForm.elements[i];
		if ((element.attributes["RealName"]) && (element.attributes["RealName"].value == name))
			return element;
	}
	return false;
}

function FindTableRow(objTable, rowID) {
	for (var i = 0; i < objTable.rows.length; i++) {
		if ((objTable.rows[i].attributes["id"]) && (objTable.rows[i].attributes["id"].value == rowID))
			return objTable.rows[i];
	}
	return 0;
}

function GetGroup(objForm, groupName) {
	var arrGroup = new Array();
	var objTemp = objForm.elements[groupName];
	if (typeof objTemp.length == "undefined") {
		arrGroup[0] = objTemp;
	}
	else {
		for (var i = 0; i < objTemp.length; i++)
			arrGroup[arrGroup.length] = objTemp[i];
	}
	return arrGroup;
}

/*
Hashing function for controls.
returns unique value for each control, which is their absolute position on screen.
version: 1.0
*/
function H(objControl) {
	return (objControl.offsetLeft + "," + objControl.offsetTop);
}

function CancelChangeNumber(id) {
	//hide the input panel:
	var objPanel = document.getElementById(id + "_change");
	objPanel.style.display = "none";

	//show the number panel:
	objPanel = document.getElementById(id);
	objPanel.style.display = "";

	//enable final commit button:
	var objButton = FindByRealName(document.forms[0], "btnCommitTeams");
	objButton.disabled = false;
	/*
	objButton=FindByRealName(document.forms[0], "btnCommitPlayers");
	objButton.disabled = false;	
	*/
}

function ShowChangeNumber(id, textBoxName) {
	//hide the number panel:
	var objPanel = document.getElementById(id);
	objPanel.style.display = "none";

	//show the input panel:
	objPanel = document.getElementById(id + "_change");
	objPanel.style.display = "";

	//put focus:
	document.forms[0].elements[textBoxName].focus();

	//select all text:
	document.forms[0].elements[textBoxName].select();

	//disable final commit button:
	var objButton = FindByRealName(document.forms[0], "btnCommitTeams");
	objButton.disabled = true;
	/*
	objButton=FindByRealName(document.forms[0], "btnCommitPlayers");
	objButton.disabled = true;	
	*/
}

function IsPrimaryClicked(objButton) {
	if (objButton.checked) {
		//button was checked - disable sub button:
		objButton.form.elements["IsSub"].disabled = true;
	}
	else {
		//button was unchecked - enable sub button:
		objButton.form.elements["IsSub"].disabled = false;
	}
}

function IsSubClicked(objButton) {
	if (objButton.checked) {
		//button was checked - disable primary button:
		objButton.form.elements["IsPrimary"].disabled = true;
	}
	else {
		//button was unchecked - enable primary button:
		objButton.form.elements["IsPrimary"].disabled = false;
	}
}

function PartialJoin(arr, index, delimeter) {
	var result = "";
	for (var i = index; i < arr.length; i++) {
		result += arr[i];
		if (i < (arr.length - 1))
			result += delimeter;
	}
	return result;
}

function ApplyLinkPreview(objTextbox, strToAdd) {
	var objForm = objTextbox.form;
	var strName = objTextbox.name;
	var arrTemp = strName.split("_");
	var strIndex = PartialJoin(arrTemp, 1, "_");
	var objPreview = document.getElementById("LinkPreview_" + strIndex);
	if (objPreview) {
		var strText = objForm.elements["LinkText_" + strIndex].value;
		var strURL = objForm.elements["LinkUrl_" + strIndex].value;
		objPreview.href = strURL;
		objPreview.innerHTML = strText;
		if ((typeof strToAdd != "undefined") && (strText.length > 0))
			objPreview.innerHTML += strToAdd;
	}
}

function ApplyAttachmentPreview(objTextbox) {
	var objForm = objTextbox.form;
	var strName = objTextbox.name;
	var arrTemp = strName.split("_");
	var strIndex = arrTemp[arrTemp.length - 1];
	var objPreview = document.getElementById("AttachmentPreview_" + strIndex);
	if (objPreview) {
		var strText = objForm.elements["Attachment_Text_" + strIndex].value;
		objPreview.innerHTML = strText;
	}
}

var _arrMaxChars = new Array();
function IterateTextAreas() {
	var arrTextAreas = document.getElementsByTagName("textarea");
	for (var i = 0; i < arrTextAreas.length; i++) {
		var strMaxChars = arrTextAreas[i].getAttribute("maxchars");
		if ((strMaxChars) && (strMaxChars.length > 0) && (!isNaN(parseInt(strMaxChars)))) {
			AttachMaxChars(arrTextAreas[i], parseInt(strMaxChars));
			arrTextAreas[i].onkeypress();
		}
	}
}

function IterateTables() {
	var arrTables = document.getElementsByTagName("table");
	for (var i = 0; i < arrTables.length; i++) {
		var autoCalcSum = arrTables[i].getAttribute("auto_calc_sum");
		if ((autoCalcSum) && (autoCalcSum == "1"))
			AutoCalcSum(arrTables[i]);
	}
}

function AttachMaxChars(objTextArea, maxChars) {
	var strName = objTextArea.name;
	_arrMaxChars[strName] = new Array();
	_arrMaxChars[strName]["textarea"] = objTextArea;
	_arrMaxChars[strName]["maxchars"] = maxChars;
	var strText = objTextArea.getAttribute("maxcharstext");
	if ((!strText) || (strText.length == 0))
		strText = "characters left: %1";
	strText = strText.replace("%1", "<span id=\"charsLeft_" + strName + "\"></span>");
	var objDiv = document.createElement("div");
	objDiv.id = "charsLeftContainer_" + strName;
	objDiv.innerHTML = strText;
	//objTextArea.parentNode.appendChild(objDiv);
	InsertAfter(objTextArea, objDiv);
	_arrMaxChars[strName]["charsleftcontrol"] = document.getElementById("charsLeft_" + strName);
	_arrMaxChars[strName]["lastvalue"] = objTextArea.value;
	objTextArea.onkeyup = new Function("TriggerCheckMaxChars(10);");
	objTextArea.onkeypress = new Function("TriggerCheckMaxChars(10);");
	objTextArea.onbeforepaste = new Function("TriggerCheckMaxChars(100);");
	//window.clearInterval(_arrMaxChars["timer"]);
	//_arrMaxChars["timer"] = window.setInterval("CheckMaxChars();", 1000);
}

function TriggerCheckMaxChars(interval) {
	window.setTimeout("CheckMaxChars();", interval);
}

function InsertAfter(element, newElement) {
	var elementAfter = 0;
	for (var i = 0; i < element.parentNode.childNodes.length - 1; i++) {
		if (element.parentNode.childNodes[i] == element) {
			elementAfter = element.parentNode.childNodes[i + 1]
			break;
		}
	}
	if (elementAfter)
		element.parentNode.insertBefore(newElement, elementAfter);
	else
		element.parentNode.appendChild(newElement);
}

function CheckMaxChars() {
	//window.clearInterval(_arrMaxChars["timer"]);
	for (key in _arrMaxChars) {
		if (key != "timer") {
			var strValue = _arrMaxChars[key]["textarea"].value;
			var maxChars = _arrMaxChars[key]["maxchars"];
			//alert(GetSelectionStart(_arrMaxChars[key]["textarea"]));
			if (strValue.length > maxChars) {
				_arrMaxChars[key]["textarea"].value = _arrMaxChars[key]["lastvalue"];
				strValue = _arrMaxChars[key]["textarea"].value;
			}
			else {
				_arrMaxChars[key]["lastvalue"] = strValue.substr(0, maxChars);
			}
			_arrMaxChars[key]["charsleftcontrol"].innerHTML = (maxChars - strValue.length) + " ";
		}
	}
	//_arrMaxChars["timer"] = window.setInterval("CheckMaxChars();", 1000);
}

function OpenPreview(elementName) {
	var objInput = document.forms[0].elements[elementName];
	var strURL = objInput.value;
	if (strURL.length > 2) {
		var arrTmp = elementName.split("_");
		var width = arrTmp[1];
		var height = arrTmp[2];
		window.open(strURL, "_blank", "width=" + width + ", height=" + height + ", titlebar=no, scrollbars=no, toolbar=no");
	}
	//return false;
}

function DatePreview(objCombo, caption) {
	var objForm = objCombo.form;
	var objDayCombo = objForm.elements[caption + "_day"];
	var objMonthCombo = objForm.elements[caption + "_month"];
	var objYearCombo = objForm.elements[caption + "_year"];
	var day = parseInt(GetSelectedValue(objDayCombo));
	var month = parseInt(GetSelectedValue(objMonthCombo));
	var year = parseInt(GetSelectedValue(objYearCombo));
	var objFrame = document.frames["frm_date_preview_" + caption];
	objFrame.location = "?action=check_date_preview&day=" + day + "&month=" + month + "&year=" + year + "&caption=" + caption;
}

function MarkAllCheckboxes(oMasterCheckBox, sPrefix) {
	var arrCheckboxes = GetFormElements(oMasterCheckBox.form, "checkbox", sPrefix);
	for (var i = 0; i < arrCheckboxes.length; i++) {
		var curCheckbox = arrCheckboxes[i];
		if (curCheckbox != oMasterCheckBox) {
			curCheckbox.checked = oMasterCheckBox.checked;
			if (curCheckbox.onclick && oMasterCheckBox.name && oMasterCheckBox.name.length > 0 && curCheckbox.name != oMasterCheckBox.name)
				curCheckbox.onclick();
		}
	}
}

function GetFormElements(oForm, sType, sPrefix) {
	var arrElements = new Array();
	for (var i = 0; i < oForm.elements.length; i++) {
		var element = oForm.elements[i];
		if (element.type == sType && element.name && StartsWith(element.name, sPrefix))
			arrElements[arrElements.length] = element;
	}
	return arrElements;
}

function DeleteImageClicked(oCheckBox) {
	var blnDeleted = oCheckBox.checked;
	var oFieldSet = oCheckBox.parentNode;
	while (oFieldSet && oFieldSet.nodeName.toLowerCase() != "fieldset")
		oFieldSet = oFieldSet.parentNode;
	if (oFieldSet) {
		var sNewColor = "";
		if (blnDeleted) {
			oFieldSet.setAttribute("org_bg_color", oFieldSet.style.backgroundColor);
			sNewColor = "red";
		}
		else {
			sNewColor = oFieldSet.getAttribute("org_bg_color");
		}
		if (sNewColor.length > 0)
			oFieldSet.style.backgroundColor = sNewColor;
	}
}

function PreviewTeacherCourses(oTextBox, sTargetPanelId) {
	var sMaxCoursesValue = oTextBox.value;
	if (sMaxCoursesValue.length == 0)
		return;

	var nMaxCourses = parseInt(sMaxCoursesValue);
	if (isNaN(nMaxCourses) || nMaxCourses < 1)
		return;

	var oTargetPanel = document.getElementById(sTargetPanelId);
	if (!oTargetPanel)
		return;

	var sPreviousValue = oTargetPanel.getAttribute("max_courses");
	if (sPreviousValue == sMaxCoursesValue)
		return;

	var sURL = window.location.href + "&tcts=" + (nMaxCourses + "");
	var oXMLHTTP = GetXmlHTTP();
	if (oXMLHTTP) {
		oXMLHTTP.onreadystatechange = function () {
			if (oXMLHTTP.readyState == 4 && oXMLHTTP.status == 200) {
				//get response text:
				var strResponse = oXMLHTTP.responseText;
				oTargetPanel.innerHTML = strResponse;
			}
		}

		oTargetPanel.setAttribute("max_courses", sMaxCoursesValue);
		oTargetPanel.innerHTML = "<img src=\"/Images/spinner.gif\" border=\"0\" alt=\"Please wait...\" />";

		//send request:
		oXMLHTTP.open("GET", sURL, true);
		oXMLHTTP.send(null);
	}
}

/*
this function creates and returns XMLHTTP component
if such can be created by the browser.
*/
function GetXmlHTTP() {
	//first check for IE:
	if (window.ActiveXObject) {
		var objXML = 0;
		try {
			objXML = new ActiveXObject("Msxml2.XMLHTTP");
		}
		catch (ex) {
			try {
				objXML = new ActiveXObject("Microsoft.XMLHTTP");
			}
			catch (ex) {
				alert("AJAX: your browser does not support proper XMLHTTP");
			}
		}
		return objXML;
	}

	//maybe Mozilla?
	if (window.XMLHttpRequest)
		return new XMLHttpRequest();

	//unknown browser..
	alert("AJAX: unknown browser.");
	return 0;
} //end function GetXmlHTTP

function ShowHideOldZooZooEvents(oButton) {
	var oDiv = document.getElementById("ZooZooPastEvents");
	if (oDiv) {
		if (oDiv.style.display == "none") {
			oDiv.style.display = "";
			oButton.innerHTML = "äñúø àéøåòéí éùðéí";
		}
		else {
			oButton.innerHTML = "äöâ àéøåòéí éùðéí";
			oDiv.style.display = "none";
		}
	}
}

function MapRelativePath(sPath) {
	return _ROOT_PATH + sPath;
}