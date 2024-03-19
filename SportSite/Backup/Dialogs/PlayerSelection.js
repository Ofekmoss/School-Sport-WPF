var selectPlayerColors=new Array();
var SelectedPlayers=new Array();
var selectedPlayersCount=0;

/*********************************
// WindowLoad                   //
*********************************/
window.onload=function WindowLoad(event) {
	if (document.forms[0].elements[txtStudentNumberID])
		document.forms[0].elements[txtStudentNumberID].focus();
	
	var arrTables=document.getElementsByTagName("table");
	for (var i=0; i<arrTables.length; i++)
		ApplyTableColors(arrTables[i]);
	
	var arrCheckboxes=document.forms[0].elements["selected_players"];
	if (arrCheckboxes) {
		for (var i=0; i<arrCheckboxes.length; i++)
			AttachStudentClick(arrCheckboxes[i]);
	}
	
	var totalWidth=120;
	totalWidth += GetClientWidth(document.getElementById(tblStudentsID));
	var totalHeight=130;
	totalHeight += GetClientHeight(document.getElementById(lblTeamNameID));
	totalHeight += GetClientHeight(document.forms[0].elements[ddlGradesID]);
	totalHeight += GetClientHeight(document.forms[0].elements[txtStudentNumberID]);
	totalHeight += GetClientHeight(document.getElementById("PlayersPanel"));
	
	window.resizeTo(totalWidth, totalHeight);
	var windowLeft=window.screenLeft;
	if (typeof windowLeft == "undefined")
		windowLeft = window.screenX;
	var windowTop=window.screenTop;
	if (typeof windowTop == "undefined")
		windowTop = window.screenY;
	
	if ((totalHeight+windowTop) >= (screen.height-100)) {
		var iTemp=screen.height-(totalHeight+windowTop);
		window.moveBy(0, (iTemp >= 0)?iTemp:0);
	}
} //end function WindowLoad

/*********************************
// ConfirmClicked               //
*********************************/
function ConfirmClicked(sender) {
	sender.disabled = true;
	sender.form.elements["BtnCancel"].disabled = true;
	//var checkboxGroup=sender.form.elements["selected_players"];
	SelectedPlayers=new Array();
	//for (var i=0; i<checkboxGroup.length; i++) {
	for (var i=0; i<sender.form.elements.length; i++) {
		var element=sender.form.elements[i];
		if (element.name == "selected_players") {
			var objCheckbox=element; //checkboxGroup[i];
			if (objCheckbox.checked) {
				var playerid=objCheckbox.value;		
				var objRow=FindAncestor(objCheckbox, "TR");
				var playername=FindInnerText(objRow.cells[firstNameIndex]);
				playername += " "+FindInnerText(objRow.cells[lastNameIndex]);
				SelectedPlayers[playerid] = playername;
			}
		}
	}
	ModalResult = modalResultOK;
} //end function ConfirmClicked

/*********************************
// CancelClicked                //
*********************************/
function CancelClicked(sender) {
	sender.disabled = true;
	sender.form.elements["BtnConfirm"].disabled = true;
	ModalResult = modalResultCancel;
} //end function CancelClicked

function GetClientHeight(element) {
	return (element)?element.clientHeight:0;
}

function GetClientWidth(element) {
	return (element)?element.clientWidth:0;
}

function GetSelectedText(objCombo) {
	return objCombo.options[objCombo.selectedIndex].text;
}

function ToggleGroupSelection(objGroup, state) {
	for (var i=0; i<objGroup.length; i++) {
		objGroup[i].checked = state;
			if (typeof objGroup[i].onclick != "undefined")
				objGroup[i].onclick();
	}
}

function ClearSelection(objGroup) {
	for (var i=0; i<objGroup.length; i++) {
		if (objGroup[i].checked) {
			objGroup[i].checked = false;
			if (typeof objGroup[i].onclick != "undefined")
				objGroup[i].onclick();
		}
	}
}

function ApplyTableColors(objTable) {
	var strColors=objTable.getAttribute("colors");
	var addLight=parseInt(objTable.getAttribute("addlight"));
	if ((strColors)&&(strColors.length > 0)&&(!isNaN(addLight))) {
		var arrColors=strColors.split(",");
		var fixedRows=parseInt(objTable.getAttribute("fixedrows"));
		if (isNaN(fixedRows))
			fixedRows=0;
		for (var j=fixedRows; j<objTable.rows.length; j++) {
			var strColor=arrColors[j % arrColors.length];
			if (objTable.rows[j].style.backgroundColor != "white") {
				objTable.rows[j].onmouseover = new Function("PutMoreLight(this, '"+strColor+"', "+addLight+");");
				objTable.rows[j].onmouseout = new Function("RestoreColor(this);");
				objTable.rows[j].style.backgroundColor = strColor;
			}
		}
	}
}

function FindStudent(objButton) {
	var objTextbox = objButton.form.elements[txtStudentNumberID];
	var studentNumber = objTextbox.value;
	
	if (studentNumber.length < 5) {
		cursor_clear(objButton);
		ShowError(ERR_ID_TOO_SHORT);
		return false;
	}
	
	var objTable=document.getElementById(tblStudentsID);
	for (var i=1; i<objTable.rows.length; i++) {
		var strHTML=objTable.rows[i].cells[idNumberIndex].innerHTML;
		if (strHTML.indexOf(studentNumber) >= 0) {
			var objCheckbox=FindChildNode(objTable.rows[i].cells[selectIndex], "input");
			if (objCheckbox.disabled) {
				alert(strAlreadyExistErr);
			}
			else {
				objCheckbox.focus();
				objCheckbox.click();
			}
			cursor_clear(objButton);
			return true;
		}
	}
	
	var objForm=document.forms["FindStudentForm"];
	objForm.elements["num"].value = studentNumber;
	objForm.submit();
}

function FindStudentResults(studentID, strFirstName, strLastName, strIdNumber, strGrade) {
	cursor_clear(document.forms[0].elements["btnFindStudent"]);
	var objTable=document.getElementById(tblStudentsID);
	
	if (parseInt(studentID) < 0) {
		alert(strDoesNotExistErr);
		return false;
	}
	
	//table row...
	var objRow=document.createElement("tr");
	
	//1st cell - Grade	
	var objCell=document.createElement("td");
	objCell.innerHTML = strGrade;
	objRow.appendChild(objCell);
	
	//2nd cell - Family name.
	objCell=document.createElement("td");
	objCell.innerHTML = strLastName;
	objRow.appendChild(objCell);
	
	//3rd cell - First name.
	objCell=document.createElement("td");
	objCell.innerHTML = strFirstName;
	objRow.appendChild(objCell);
	
	//4th cell - id number.
	objCell=document.createElement("td");
	objCell.innerHTML = strIdNumber;
	objRow.appendChild(objCell);
	
	//5th cell - checkbox.
	objCell=document.createElement("td");
	var objCheckbox=document.createElement("input");
	objCheckbox.type = "checkbox";
	objCheckbox.name = "selected_players";
	objCheckbox.value = studentID;
	AttachStudentClick(objCheckbox);
	objCell.appendChild(objCheckbox);
	objRow.appendChild(objCell);
	
	if (document.all)
		objTable.childNodes[0].appendChild(objRow);
	else
		objTable.appendChild(objRow);
	ApplyTableColors(objTable);	
	objCheckbox.click();
	
	return true;
}

function SelectPlayer(sender, event) {
	var objRow=FindAncestor(sender, "tr");
	var objTable=FindAncestor(objRow, "table");
	
	if (!objRow)
		return true;
	
	if (sender.checked == false) {
		objRow.style.backgroundColor = selectPlayerColors[objRow];
		selectedPlayersCount--;
		SetControlAttribute(objRow, "add_light", "1");
	}
	else {
		selectPlayerColors[objRow] = objRow.style.backgroundColor;
		objRow.style.backgroundColor = strHighlightColor;
		selectedPlayersCount++;
		SetControlAttribute(objRow, "add_light", "0");
	}
	
	var objButton=document.forms[0].elements["BtnConfirm"];
	objButton.disabled = (selectedPlayersCount == 0);
	
	if (!sender.checked)
		return true;
	
	var index=1;
	while ((index < objTable.rows.length)&&(FindChildNode(objTable.rows[index].cells[selectIndex], "input").checked)) {
		index++;
		if (index >= objTable.rows.length)
			break;		
	}
	if (index < objTable.rows.length)
	{
		var firstNotChecked=objTable.rows[index];
		SwapNode(objRow, firstNotChecked);
		sender.checked = true;
		sender.focus();
	}
}

function AttachStudentClick(objCheckbox) {
	objCheckbox.onclick = new Function("SelectPlayer(this);");
}

function StudentKeyPress(event, objTextbox) {
	var keyCode=event.keyCode;
	if (keyCode == 13) {
		FindStudent(objTextbox);
		return false;
	}
	return ((keyCode >= 48)&&(keyCode <= 57));
}