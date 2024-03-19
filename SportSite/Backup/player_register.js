var _findStudentByIdUrlTemplate = "Register.aspx?action=FindStudentById&id_number=$idnumber&nnn=$time";
var _isExternalStudentUrlTemplate = "Register.aspx?action=CheckExternalStudent&id_number=$idnumber&nnn=$time";
var _studentSearchInProcess = false;
var _studentAddInProcess = false;

$(document).ready(function () {
	HandlePlayerTables();
	HandlePlayerSearch();
	HandleNewStudentForm();
});

function HandlePlayerTables() {
	$(".PlayersTable").each(function () {
		var oTable = $(this);
		HandlePlayerTable(oTable);
	});
}

function HandlePlayerSearch() {
	var oButton = $("#btnSearchRegisteredPlayer");
	var oTextbox = $(".RegisterPlayerSearch");
	var btnSearchRegisteredPlayer_click = function () {
		if (_studentSearchInProcess)
			return false;
		var studentId = oTextbox.val();
		if (studentId.length > 0) {
			var existingRow = $(".PlayersTable").find("tr").filter(function () {
				return $(this).find(".student_id_cell").text() == studentId;
			});
			if (existingRow.length === 1) {
				if (IsPlayerSelectionRowDisabled(existingRow)) {
					var name = existingRow.find(".student_first_name").text() + " " + existingRow.find(".student_last_name").text()
					alert(ALREADY_IN_TEAM_ERROR.replace("$name", name));
				} else {
					SelectPlayerRow(existingRow);
				}
			} else {
				if (studentId.length > 5) {
					var oImage = CreateSpinnerImage(20);
					WaitAndDisable(oButton, oImage, [oTextbox]);
					_studentSearchInProcess = true;
					$.get(_findStudentByIdUrlTemplate.replace("$idnumber", studentId).replace("$time", (new Date()).getTime()), function (rawResponse) {
						_studentSearchInProcess = false;
						EndWaitingAndReEnable(oButton, oImage, [oTextbox], btnSearchRegisteredPlayer_click);
						try {
							HandleStudentSearchResults(studentId, rawResponse);
						}
						catch (err) {
							alert(STUDENT_SEARCH_GENERAL_ERROR + "\n\n" + err);
						}
					});
				}
			}
		}
		return false;
	};
	oButton.bind("click", btnSearchRegisteredPlayer_click);

	oTextbox.bind("keypress", function (evt) {
		var keyCode = evt.keyCode || evt.which;
		if (keyCode === 13) {
			$("#btnSearchRegisteredPlayer").click();
			return false;
		} else {
			return (keyCode >= 48) && (keyCode <= 57);
		}
	});
}

function HandleStudentSearchResults(idNumber, rawResponse) {
	var response = $.parseJSON(rawResponse);
	if (response["exists"] == "true") {
		var grade = response["grade"];
		var matchingItem = $(".new_student_grade option").filter(function () {
			return $(this).text() == grade;
		});
		if (matchingItem.length > 0) {
			AddExistingStudent(response, idNumber);
		} else {
			var name = response["first_name"] + " " + response["last_name"];
			alert(STUDENT_DIFFERENT_GRADE.replace("$name", name));
			$(".RegisterPlayerSearch").focus();
		}
	} else {
		if (response["valid"] == "true") {
			//allow creating new student
			ShowNewStudentForm(idNumber);
		} else {
			alert(INVALID_ID_NUMBER);
			$(".RegisterPlayerSearch").focus();
		}
	}
}

function HandleNewStudentForm() {
	var oNewStudentPanel = $("#pnlAddNewStudent");
	var oChoosePlayersPanel = $("#pnlChoosePlayers");
	var oNewStudentButton = $("#btnSendNewStudent");
	var lblIdNumber = oNewStudentPanel.find(".new_student_id_number");
	var txtFirstName = oNewStudentPanel.find(".new_student_first_name");
	var txtLastName = oNewStudentPanel.find(".new_student_last_name");
	var txtBirthday = oNewStudentPanel.find(".new_student_birthday");
	var ddlGrade = oNewStudentPanel.find(".new_student_grade");

	oNewStudentPanel.find("input").bind("blur", function () {
		$(this).val($.trim($(this).val()));
	});

	$("#btnCancelNewStudent").click(function () {
		oNewStudentPanel.hide("slow", function () {
			oChoosePlayersPanel.show("slow", function () {
				$(".RegisterPlayerSearch").focus();
				$("#FooterPanel").show();
			});
		});
	});

	var btnSendNewStudent_click = function () {
		if (_studentAddInProcess)
			return;
		if (ValidateNewStudentValue(txtFirstName, true, false) && ValidateNewStudentValue(txtLastName, true, false) && ValidateNewStudentValue(txtBirthday, true, true)) {
			var affectedInputs = [txtFirstName, txtLastName, txtBirthday, ddlGrade];
			var idNumber = lblIdNumber.text();
			var firstName = txtFirstName.val();
			var lastName = txtLastName.val();
			var birthDay = txtBirthday.val();
			var grade = ddlGrade.val();
			var oImage = CreateSpinnerImage(45);
			_studentAddInProcess = true;
			WaitAndDisable(oNewStudentButton, oImage, affectedInputs);
			$.post("Register.aspx", { "action": "AddExternalStudent", "id_number": idNumber, "first_name": firstName,
				"last_name": lastName, "birthdate": birthDay, "grade": grade
			}, function (rawResponse) {
				_studentAddInProcess = false;
				EndWaitingAndReEnable(oNewStudentButton, oImage, affectedInputs, btnSendNewStudent_click);
				try {
					var response = $.parseJSON(rawResponse);
					if (response["success"] == "true") {
						oNewStudentPanel.hide("slow", function () {
							oChoosePlayersPanel.show("slow", function () {
								var selectedGrade = $(".new_student_grade option:selected").text();
								AddPlayerRow(response["internal_id"], "", idNumber, firstName, lastName, selectedGrade, birthDay);
							});
						});
					} else {
						alert(response["error"]);
					}
				} catch (ex) {
					alert(STUDENT_ADD_GENERAL_ERROR);
				}
			});
		}
	};
	oNewStudentButton.bind("click", btnSendNewStudent_click);
}

function ShowNewStudentForm(idNumber, blnEditMode, presetFirstName, presetLastName, presetBirthday, presetGrade) {
	var oNewStudentPanel = $("#pnlAddNewStudent");
	var oChoosePlayersPanel = $("#pnlChoosePlayers");
	var lblIdNumber = oNewStudentPanel.find(".new_student_id_number");
	var txtFirstName = oNewStudentPanel.find(".new_student_first_name");
	var txtLastName = oNewStudentPanel.find(".new_student_last_name");
	var txtBirthday = oNewStudentPanel.find(".new_student_birthday");
	var ddlGrade = oNewStudentPanel.find(".new_student_grade");
	var oFormCaption = oNewStudentPanel.find("h2");
	var btnSubmitForm = $("#btnSendNewStudent");
	var btnCancelSubmission = $("#btnCancelNewStudent");
	var currentIdNumber = lblIdNumber.text();
	if (typeof blnEditMode == "undefined")
		blnEditMode = false;
	RestoreOriginalText([oFormCaption, btnSubmitForm, btnCancelSubmission]);
	var selectedGradeIndex = 0;
	if (ddlGrade[0].selectedIndex <= 0) {
		var arrAvailableGrades = ($("#CategoryAvailableGrades").html() || "").split(",");
		var firstAvailableGrade = (arrAvailableGrades.length > 0) ? arrAvailableGrades[0] : "";
		selectedGradeIndex = ddlGrade.find("option").filter(function () {
			return $(this).text() == firstAvailableGrade;
		}).index();
		ddlGrade[0].selectedIndex = selectedGradeIndex;
	}
	if (currentIdNumber.length > 0 && currentIdNumber != idNumber) {
		txtFirstName.val("");
		txtLastName.val("");
		txtBirthday.val("");
		ddlGrade[0].selectedIndex = selectedGradeIndex;
	}

	if (blnEditMode) {
		PreserverOriginalTextAndChange([{ "element": oFormCaption, "text": EDIT_STUDENT_FORM_CAPTION }, 
			{ "element": btnSubmitForm, "text": EDIT_STUDENT_SUBMIT_BUTTON_CAPTION }, 
			{ "element": btnCancelSubmission, "text": EDIT_STUDENT_CANCEL_BUTTON_CAPTION }]);
		if (typeof presetFirstName !== "undefined")
			txtFirstName.val(presetFirstName);
		if (typeof presetLastName !== "undefined")
			txtLastName.val(presetLastName);
		if (typeof presetBirthday !== "undefined")
			txtBirthday.val(presetBirthday);
		if (typeof presetGrade !== "undefined") {
			ddlGrade.find("option").each(function () {
				if ($(this).text() == presetGrade) {
					$(this).attr("selected", "selected");
				}
			});
		}
	}

	lblIdNumber.text(idNumber);
	oChoosePlayersPanel.hide("slow", function () {
		oNewStudentPanel.show("slow", function () {
			txtFirstName.focus();
			$("#FooterPanel").hide();
		});
	});
}

function RestoreOriginalText(arrElements) {
	for (var i = 0; i < arrElements.length; i++) {
		var curElement = arrElements[i];
		var strText = curElement.data("original_text") || "";
		if (strText.length > 0)
			curElement.html(strText);
	}
}

function PreserverOriginalTextAndChange(elementMapping) {
	for (var i = 0; i < elementMapping.length; i++) {
		var currentMapping = elementMapping[i];
		var currentElement = currentMapping["element"];
		if ((currentElement.data("original_text") || "").length === 0)
			currentElement.data("original_text", currentElement.html());
		currentElement.html(currentMapping["text"])
	}
}

function ValidateNewStudentValue(oInput, blnRequired, blnDate) {
	var strCaption = oInput.prevAll("label:first").text().replace(":", "");
	var strValue = oInput.val();
	if (strValue.length == 0 && blnRequired) {
		alert(MISSING_STUDENT_VALUE.replace("$caption", strCaption));
		oInput.focus();
		return false;
	}

	if (blnDate && strValue.length > 0) {
		var maxYear = (new Date()).getFullYear() - 6;
		if (!IsValidDate(strValue, 1900, maxYear)) {
			alert(INVALID_BIRTHDAY);
			oInput.focus();
			return false;
		}
	}

	return true;
}

function AddExistingStudent(response, idNumber) {
	var comments = (response["school_id"] != _loggedUserSchoolId) ? STUDENT_DIFFERENT_SCHOOL.replace("$school", response["school_name"]) : "";
	AddPlayerRow(response["internal_id"], comments, idNumber, response["first_name"], response["last_name"], response["grade"], response["birthday"]);
}

function AddPlayerRow(student_id, comments, idNumber, firstName, lastName, grade, birthday) {
	var oTable = $(".PlayersTable");
	var existingRow = oTable.find("tr").filter(function () {
		return $(this).find(".student_id_cell").text() == idNumber;
	});
	if (existingRow.length === 1) {
		existingRow.data("birthday", birthday);
		existingRow.find(".student_first_name").text(firstName);
		existingRow.find(".student_last_name").text(lastName);
		existingRow.find(".student_grade").text(grade);
		return;
	}
	var oRow = $(".student_template_row").clone().removeClass("student_template_row");
	var oCheckbox = oRow.find("input[type='checkbox']");
	if (comments.length === 0)
		comments = "&nbsp;";
	oRow.data("birthday", birthday);
	oRow.find(".student_id_cell").text(idNumber);
	oRow.find(".student_first_name").text(firstName);
	oRow.find(".student_last_name").text(lastName);
	oRow.find(".student_grade").text(grade);
	oRow.find(".student_comments").html(comments);
	oCheckbox.val(student_id + "");
	oRow.show();
	oTable.find("tr").first().after(oRow);
	SelectPlayerRow(oRow);
	$(".RegisterActionButton").show();
}

function SelectPlayerRow(oRow) {
	$(".RegisterPlayerSearch").val("");
	var oCheckbox = oRow.find("input[type='checkbox']");
	oCheckbox.click();
	oCheckbox.focus();
}

function HandlePlayerTable(oTable) {
	var strColors = oTable.data("colors") || "";
	if (strColors.length == 0)
		return false;

	var addLight = parseInt(oTable.data("addlight"), 10);
	if (isNaN(addLight))
		return false;

	var arrColors = strColors.split(",");
	var fixedRows = parseInt(oTable.data("fixedrows"), 10);
	if (isNaN(fixedRows)) {
		fixedRows = 0;
	}

	oTable.on("mouseover", "tr", function () {
		var rowIndex = $(this).index();
		if (rowIndex >= fixedRows && !IsPlayerSelectionRowDisabled($(this))) {
			var strColor = arrColors[rowIndex % arrColors.length];
			PutMoreLight(this, strColor, addLight);
		}
	});

	oTable.on("mouseout", "tr", function () {
		var rowIndex = $(this).index();
		if (rowIndex >= fixedRows && !IsPlayerSelectionRowDisabled($(this)))
			RestoreColor(this);
	});

	oTable.on("click", "input[type='checkbox']", function () {
		var parentRow = $(this).parents("tr").first();
		ApplyEditButton();
		if (this.checked) {
			parentRow.data("click_original_bg", parentRow.css("background-color"));
			parentRow.css("background-color", "yellow");
			parentRow.attr("add_light", "0");
		} else {
			parentRow.attr("add_light", "1");
			parentRow.css("background-color", parentRow.data("click_original_bg"));
		}
	});

	oTable.find("tr").each(function () {
		var rowIndex = $(this).index();
		if (rowIndex >= fixedRows && !IsPlayerSelectionRowDisabled($(this))) {
			var strColor = arrColors[rowIndex % arrColors.length];
			$(this).css("background-color", strColor);
		}
	});

	return true;
}

function ApplyEditButton() {
	var oTable = $(".PlayersTable");
	if (oTable.length === 0)
		return;
	var strChangePicURL = "Register.aspx?action=UploadStudentPicture&idnumber=$id";
	var strEditButtonId = "btnEditExistingStudent";
	var strChangePictureLinkId = "lnkChangeStudentPicture";
	var oButton = $("#" + strEditButtonId);
	var oLink = $("#" + strChangePictureLinkId);
	if (oButton.length === 0) {
		oButton = $("<button></button>").attr("id", strEditButtonId).attr("type", "button").html(EDIT_STUDENT_BUTTON_TEXT);
		oButton.css("font-weight", "bold");
		oButton.bind("click", EditExistingStudentButtonClicked);
		oButton.insertBefore(oTable);
		var oBR = $("<br></br>");
		oBR.insertBefore(oTable);
		oLink = $("<a></a>").attr("id", strChangePictureLinkId).attr("href", "#").html(CHANGE_PICTURE_LINK_TEXT);
		oLink.css("font-weight", "bold");
		oLink.insertBefore(oTable);
	}
	oButton.hide();
	oLink.hide();
	var arrChecked = oTable.find("input[type='checkbox']:checked");
	if (arrChecked.length === 1) {
		var oCheckedBox = arrChecked.first();
		var idNumber = oCheckedBox.parents("tr").first().find("td").eq(4).text();
		oLink.attr("href", strChangePicURL.replace("$id", idNumber));
		oButton.show();
		oLink.show();
	}
}

function EditExistingStudentButtonClicked() {
	var oTable = $(".PlayersTable");
	if (oTable.length === 0)
		return false;

	var oCheckbox = oTable.find("input[type='checkbox']:checked").first();
	if (oCheckbox.length === 0)
		return false;
	var oRow = oCheckbox.parents("tr").first();
	var strBirthday = oRow.data("birthday") || "";
	var arrCells = oRow.find("td");
	var studentId = arrCells.eq(4).text();
	var oButton = $(this);
	var oImage = CreateSpinnerImage(20);
	WaitAndDisable(oButton, oImage, []);
	$.get(_isExternalStudentUrlTemplate.replace("$idnumber", studentId).replace("$time", (new Date()).getTime()), function (rawResponse) {
		EndWaitingAndReEnable(oButton, oImage, [], EditExistingStudentButtonClicked);
		var response = $.parseJSON(rawResponse);
		if (response["external"] == "true") {
			ShowNewStudentForm(studentId, true, arrCells.eq(3).text(), arrCells.eq(2).text(), strBirthday, arrCells.eq(1).text());
		} else {
			alert(EDIT_STUDENT_NOT_EXTERNAL);
		}
	});
	return true;
}

function IsPlayerSelectionRowDisabled(oRow) {
	var oCheckbox = oRow.find("input");
	if (oCheckbox.length === 0)
		return true;
	return oCheckbox.is(":disabled");
}

function EndWaitingAndReEnable(initiatingButton, spinnerImage, affectedElements, buttonClickHandler) {
	initiatingButton.data("original_text", spinnerImage.data("original_text"));
	spinnerImage.replaceWith(initiatingButton);
	for (var i = 0; i < affectedElements.length; i++)
		affectedElements[i].prop("disabled", false);
	initiatingButton.bind("click", buttonClickHandler);
	$("#FooterPanel").show();
}

function WaitAndDisable(initiatingButton, spinnerImage, affectedElements) {
	spinnerImage.data("original_text", initiatingButton.data("original_text"));
	initiatingButton.replaceWith(spinnerImage);
	for (var i = 0; i < affectedElements.length; i++)
		affectedElements[i].prop("disabled", true);
}

function CreateSpinnerImage(width) {
	var imageUrl = MapRelativePath("Images/spinner.gif");
	var oImage = $("<img />").attr("src", imageUrl).attr("border", "0").css("width", width + "px");
	return oImage;
}