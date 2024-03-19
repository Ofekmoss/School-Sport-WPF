var _rootPath="";
var _flashMoviesCount=0;
var _flash_picURL="";
var arrErrors=new Array();
var _dialogElement=0;
var _showWaitElement = 0;
var _showWaitMessage = "";
var _showWaitSeconds = 0;
var _showWaitTimer = 0;
var _showWaitCurrentSeconds = 0;

function MakeBlackCells(strTableId) {
	var objTable = document.getElementById(strTableId);
	if (!objTable)
		return;
	for (var r = 0; r < objTable.rows.length; r++) {
		var objRow = objTable.rows[r];
		for (c = 0; c < objRow.cells.length; c++) {
			var objCell = objRow.cells[c];
			if (objCell.style.backgroundColor == "black") {
				var width = objCell.clientWidth;
				var height = objCell.clientHeight;
				objCell.innerHTML = "<img src=\"images/pix_black.gif\" width=\"" + width + "\" height=\"" + height + "\" />";
			}
		}
	}
}

function AddQuestion() {
	var objWindow = OpenActionWindow("FAQ", _add_question_title, "", "", _add_question_texts);
}

function ShowWait(element, strMsg, seconds) {
	if (typeof element == "undefined") {
		element = _showWaitElement;
		strMsg = _showWaitMessage;
		seconds = _showWaitSeconds;
	}
	if (!element)
		return;
	if (_showWaitCurrentSeconds >= seconds) {
		window.clearTimeout(_showWaitTimer);
		_showWaitElement = 0;
		_showWaitMessage = "";
		_showWaitSeconds = "";
		_showWaitCurrentSeconds = 0;
		return;
	}
	element.style.fontWeight = "bold";
	if (_showWaitCurrentSeconds > 0)
		element.disabled = true;
	var percentage = parseInt((_showWaitCurrentSeconds/seconds)*100);
	element.innerHTML = strMsg+"... "+percentage+"%";
	_showWaitElement = element;
	_showWaitMessage = strMsg;
	_showWaitSeconds = seconds;
	_showWaitCurrentSeconds++;
	_showWaitTimer = window.setTimeout("ShowWait();", 1000);
}

function MoveContentsDown() {
	var articleHeight = 121;
	var arrElements = new Array("MiddleBannerPanel", "SportFlowersPanel", "BottomBannerPanel", "MoreArticlesLinkPanel");
	for (var i=0; i<arrElements.length; i++) {
		var objDiv = document.getElementById(arrElements[i]);
		objDiv.style.top = (objDiv.offsetTop+articleHeight)+"px";
	}
}

function AddDebugText(strText) {
	var objDiv = document.createElement("div");
	objDiv.innerHTML = strText;
	document.body.appendChild(objDiv);
}

function CloseDialog() {
	if (_dialogElement) {
		document.body.removeChild(_dialogElement);
		_dialogElement = 0;
	}
}

function OpenDialog(strURL, width, height) {
	/*
	var result=0;
	if (window.showModalDialog) {
		result = window.showModalDialog(strURL, "", "dialogHeight:"+height+",dialogWidth:"+width+",resizable:yes");
	}
	else {
		result = window.open(strURL, "_blank", "modal=1,dialog=1,resizable=1,width="+width+",height="+height);
	}
	return result;
	*/
	
	if (!window.showModalDialog) {
		return window.open(strURL, "_blank", "modal=1,dialog=1,resizable=1,width="+width+",height="+height)
	}
	
	var objDiv = document.createElement("div");
	objDiv.style.position = "absolute";
	objDiv.style.left = "0px";
	objDiv.style.top = "0px";
	objDiv.style.width = document.body.scrollWidth+"px";
	objDiv.style.height = document.body.scrollHeight+"px";	
	objDiv.style.backgroundColor = "#C0C0C0";
	objDiv.zIndex = 0;
	
	var objFrame = document.createElement("iframe");
	objFrame.style.position = "absolute";
	objFrame.style.left = parseInt((document.body.clientWidth-width)/2)+"px";
	objFrame.style.top = parseInt((document.body.clientHeight-height)/2)+"px";	
	objFrame.width = width+"px";
	objFrame.height =  height+"px";
	objFrame.frameborder = "0";
	objFrame.scrolling = "auto";
	objFrame.src = strURL;
	objFrame.zIndex = 0;
	
	/*
	_elements = new Array();
	for (var i=0; i<document.body.childNodes.length; i++) {
		var element=document.body.childNodes[i];
		if (element.style) {
			_elements[element] = element.style.display;
			element.style.visibility = "hidden";
		}
	}
	*/
	
	_dialogElement = objDiv;
	objDiv.appendChild(objFrame);
	document.body.appendChild(objDiv);
}

function ApplyMaxRows() {
	var arrTables=document.getElementsByTagName("table");
	for (var i=0; i<arrTables.length; i++) {
		var elm=arrTables[i];
		if (elm.attributes["maxrows"]) {
			var maxRows=parseInt(elm.attributes["maxrows"].value);
			if (!isNaN(maxRows)) {
				var rowsCount=elm.rows.length;
				if (rowsCount > maxRows) {
					var totalHeight=0;
					for (var j=0; j<maxRows; j++)
						totalHeight += elm.rows[j].scrollHeight;
					var objDiv=document.createElement("div");
					objDiv.style.height = totalHeight+"px";
					objDiv.style.width = (elm.scrollWidth+50)+"px";
					objDiv.style.overflow = "scroll";
					objDiv.style.overflowX = "hidden";
					objDiv.style.overflowY = "scroll";
					elm.parentNode.insertBefore(objDiv, elm);
					elm.parentNode.removeChild(elm);
					objDiv.appendChild(elm);
				}
			}
		}
	}
}

function SetDefaultFocus() {
	for (var i=0; i<document.forms.length; i++) {
		for (var j=0; j<document.forms[i].elements.length; j++) {
			var elm=document.forms[i].elements[j];
			if (elm.type == "text") {
				if (elm.attributes["default"]) {
					elm.focus();
					break;
				}
			}
		}
	}
}

function AddError(strMessage) {
	arrErrors[arrErrors.length] = strMessage;
	var objPanel=document.getElementById("ErrorPanel");
	var objLabel=document.getElementById("lbShowError");
	if (!objPanel)
		return false;
	objPanel.innerHTML += strMessage;
	objPanel.innerHTML += "<br />";
	if (objLabel)
		objLabel.style.display = "block";
	return true;
}

function AddComboOption(objCombo, strValue, strText) {
	var newOption=new Option();
	newOption.value = strValue;
	newOption.text = strText;
	objCombo.options.add(newOption);
	return true;
}

function RegisterFlashMovie(url, width, height, containerID, flashVars, bgColor) {
	var so = new SWFObject(url, "movie_"+(_flashMoviesCount+1), width+"", height+"", "7", bgColor);
	if ((flashVars)&&(flashVars.length > 0)) {
		var arrVarPairs=flashVars.split("&");
		for (var i=0; i<arrVarPairs.length; i++) {
			arrVarPairs[i] = ReplaceGlobal(arrVarPairs[i], "{amp}", "&");
			var arrFlashVar=arrVarPairs[i].split("=");
			if (arrFlashVar.length >= 2) {
				var strVarName=arrFlashVar[0];
				var strVarValue=PartialJoin(arrFlashVar, 1, "=");
				so.addVariable(strVarName, strVarValue);
				if (strVarName == "picUrl")
					_flash_picURL = strVarValue;
			}
		}
	}
	so.write(containerID);
	_flashMoviesCount++;
}

function PartialJoin(arr, index, delimeter) {
	var result="";
	for (var i=index; i<arr.length; i++) {
		result += arr[i];
		if (i < (arr.length-1))
			result += delimeter;
	}
	return result;
}

function FillVerticalGap() {
	var objPageContents=GetElementByClassName("PageContentsPanel", new Array("div", "table"));
	var objFooter = document.getElementById("FooterPanel");
	if (objPageContents) {
		ResizeContents(objPageContents);
	}
	
	var objMainBody=document.getElementById("MainViewBody");
	var footerLeft=objFooter.offsetLeft;
	var footerTop=objFooter.offsetTop;
	var vertDiff=footerTop-522;
	if (vertDiff > 0) {
		var objDiv=document.createElement("div");
		objDiv.className = "VerticalGapPanel";
		objDiv.style.height = vertDiff+"px";
		objDiv.innerHTML = "&nbsp;";
		objMainBody.appendChild(objDiv);
	}
	
	if (!document.all) {
		var arrElements=document.getElementsByTagName("div");
		for (var i=0; i<arrElements.length; i++) {
			var element=arrElements[i];
			if (element.className == "OrdinaryArticlePanel")
				element.style.position = "relative";
		}
	}
}

var _extraContentsID="";
function ResizeContents(objPageContents) {
	if (typeof objPageContents == "undefined") {
		if (_pageContentsID.length == 0) {
			objPageContents = GetElementByClassName("PageContentsPanel", new Array("div", "table"));
			_pageContentsID = objPageContents.id;
		}
		objPageContents = document.getElementById(_pageContentsID);
	}
	
	var objExtraContents=0;
	if (_extraContentsID.length == 0) {
		objExtraContents = GetElementByClassName("ExtraContentsPanel", new Array("div", "table"));
		_extraContentsID = objExtraContents.id;
	}
	objExtraContents = document.getElementById(_extraContentsID);
	var objFooter = document.getElementById("FooterPanel");
	var objNavBar=document.getElementById("NavBarPanel");
	var objBottomBanner=document.getElementById("BottomBannerPanel");
	var navBarBottom=(objNavBar.offsetTop+objNavBar.offsetHeight);
	var bottomBannerBottom=(objBottomBanner.offsetTop+objBottomBanner.offsetHeight);
	var pageContentsBottom = (objPageContents.offsetTop + objPageContents.offsetHeight);
	objExtraContents.style.display = "";		
	var extraHeight=objExtraContents.offsetHeight;		
	var diff=(navBarBottom-pageContentsBottom-35);
	if (diff > 0) {
		var objCommentsTable = document.getElementById("ArticleCommentsPanel");
		if (!objCommentsTable)
			objPageContents.style.height = (objPageContents.offsetHeight+diff)+"px";
	} else {
		objNavBar.style.height = (objNavBar.offsetHeight+(-1*diff))+"px";
	}
	pageContentsBottom = (objPageContents.offsetTop+objPageContents.offsetHeight);		
	objExtraContents.style.top = (pageContentsBottom+25)+"px";
	var objBottomPanel=GetElementByClassName("BottomInnerPanel", new Array("div"));
	if (objBottomPanel) {
		if (document.all) {
			objBottomPanel.style.top = objPageContents.clientHeight; //objPageContents.style.height;
		}
		else {
			objBottomPanel.style.top = (parseInt(pageContentsBottom)-objBottomPanel.offsetHeight)+"px";
			objBottomPanel.style.left = "15px";
		}
	}
	
	var objMainBody=document.getElementById("MainViewBody");
	var objLeftCell=document.getElementById("LeftTableCell");
	var objCenterCell=document.getElementById("CenterTableCell");
	var objRightCell=document.getElementById("RightTableCell");
	var navBarBottom=(objNavBar.offsetTop+objNavBar.offsetHeight);
	var pageContentsBottom=(objPageContents.offsetTop+objPageContents.offsetHeight);
	/*
	var objEventsReportPanel=document.getElementById("EventReportsPanel");
	try {
		var vvv=objEventsReportPanel.offsetTop+objEventsReportPanel.offsetHeight;
		var ttt=document.createElement("div");
		ttt.style.position = "absolute";
		ttt.style.left = "500px";
		ttt.style.top = vvv+"px";
		ttt.style.backgroundColor = "yellow";
		ttt.innerHTML = "1234567890";
		objEventsReportPanel.parentNode.appendChild(ttt);
	} catch (ex) {
		AddDebugText(ex.description);
	}
	//AddDebugText(navBarBottom+"<br />"+pageContentsBottom+"<br />"+
	//	objEventsReportPanel.offsetTop+"<br />"+objEventsReportPanel.offsetHeight);
	*/
	var bottomBannerBottom=(objBottomBanner.offsetTop+objBottomBanner.offsetHeight);
	var maxBottom=parseInt(FindMaxNumber(new Array(navBarBottom, pageContentsBottom, bottomBannerBottom)));
	maxBottom += extraHeight;
	var height=maxBottom+objFooter.offsetHeight;
	objFooter.style.top = maxBottom+"px";
	objMainBody.style.height = height+"px";
	objLeftCell.style.height = height+"px";
	objCenterCell.style.height = height+"px";
	objRightCell.style.height = height+"px";
}

function AssignButtonPanels()
{
	var arrElements=document.getElementsByTagName("div");
	for (var i=0; i<arrElements.length; i++)
	{
		var element=arrElements[i];
		var strClass=element.className;
		if (strClass == "ButtonPanel")
		{
			element.onmouseover=new Function("this.className = \"ButtonPanelOver\";");
			element.onmouseout=new Function("this.className = \""+strClass+"\";");
		}
		if (strClass == "ButtonPanelSub")
		{
			element.onmouseover=new Function("this.className = \"ButtonPanelSubOver\";");
			element.onmouseout=new Function("this.className = \""+strClass+"\";");
		}		
	}
	
	arrElements = document.getElementsByTagName("img");
	for (var i=0; i<arrElements.length; i++)
	{
		var element=arrElements[i];
		if (element.className == "ButtonPanelMore")
		{
			var objSpan=0;
			var objParent=element.parentNode.parentNode;
			for (var j=0; j<objParent.childNodes.length; j++) {
				if (objParent.childNodes[j].className == "ButtonPanelGap") {
					objSpan = objParent.childNodes[j];
					break;
				}
			}
			if (objSpan) {
				objSpan.innerHTML = "";
				while (element.offsetLeft > 10)
					objSpan.innerHTML += "&nbsp;";
			}
		}
	}
}

function GetElementByClassName(strClassName, arrPossibleTags) {
	//var s=strClassName+"\n";
	for (var i=0; i<arrPossibleTags.length; i++) {
		var arrElements=document.getElementsByTagName(arrPossibleTags[i]);
		for (var j=0; j<arrElements.length; j++) {
			var element=arrElements[j];
			//s += element.className.toLowerCase()+", ";
			if (element.className.toLowerCase() == strClassName.toLowerCase())
				return element;
		}
	}
	//alert(s);
	return 0;
}

function FindMaxNumber(arrNumbers) {
	var max=0;
	if (arrNumbers.length > 0)
		max = parseFloat(arrNumbers[0]);
	for (var i=1; i<arrNumbers.length; i++) {
		var curNum=parseFloat(arrNumbers[i]);
		if ((!isNaN(curNum))&&(curNum > max))
			max = curNum;
	}
	return max;
}

function CalcElementHeight(element) {
	if (typeof element.childNodes == "undefined")
		return 0;
	if ((typeof element.style != "undefined")&&(element.style.display == "none"))
		return 0;
	if (element.childNodes.length == 0)
	{
		if ((typeof element.offsetTop == "undefined")||(isNaN(element.offsetTop)))
			return 0;
		return (element.offsetTop+element.offsetHeight);
	}
	var result=element.offsetHeight;
	var parentTop=element.offsetTop;
	for (var i=0; i<element.childNodes.length; i++)
	{
		var curHeight=CalcElementHeight(element.childNodes[i])+parentTop;
		if (curHeight > result)
			result = curHeight;
	}
	return result;
}

function AutoFitCombos()
{
	var arrElements=document.getElementsByTagName("select");
	for (var i=0; i<arrElements.length; i++)
	{
		var element=arrElements[i];
		if (element.parentNode.nodeName.toLowerCase() == "td") {
			var elWidth=parseInt(element.parentNode.style.width);
			if ((!isNaN(elWidth))&&(elWidth > 0))
				element.style.width = elWidth+"px";
		}
	}
}

function ApplyTableRowColors()
{
	var arrElements=document.getElementsByTagName("table");
	for (var i=0; i<arrElements.length; i++)
	{
		var element=arrElements[i];
		if (element.className.toLowerCase() == "champtable") {
			var arrColors=new Array("#F7F7F6", "#EDEDEB");
			var row=0;
			var count=0;
			while (row<element.rows.length) {
				if (element.rows[row].cells[0].nodeName.toLowerCase() == "td") {
					element.rows[row].style.backgroundColor = arrColors[count % arrColors.length];
					count++;
				}
				row++;
			}
		}
	}
}

function Print(_obj, type)
{
	var m="", item, flag;
	for (item in _obj)
	{
		flag = true;
		if (typeof type != "undefined")
		{
			if (type == "numbers")
			{
				var value=_obj[item]*-1*-1;
				if (isNaN(value))
					flag = false;
			}
		}
		if (flag)
			m += item+": "+_obj[item]+", ";
	}
	alert(m);
}

function ToIntDef(value, defaultValue) {
	if ((!value)||(value.lenth == 0))
		return defaultValue;
	var result=parseInt(value);
	if (isNaN(result))
		result = defaultValue;
	return result;
}

function MeasureControl(objControl)
{
	//return objControl.scrollWidth;
	var tempControl=document.createElement("span");
	tempControl.innerHTML = objControl.innerHTML;
	//tempControl.style.display = "none";
	document.body.appendChild(tempControl);
	var result=tempControl.offsetWidth;
	document.body.removeChild(tempControl);
	return result;
}

function ReplaceGlobal(str, toReplace, replaceWith)
{
	return eval("str.replace(/"+toReplace+"/g, replaceWith)");
}

function FindAncestor(objControl, ancestorTagName)
{
	var parent=objControl.parentNode;
	var vv=0;
	while (parent)
	{
		if (vv > 1000)
			break;
		if (parent.nodeName.toLowerCase() == ancestorTagName.toLowerCase())
			return parent;
		parent = parent.parentNode;
		vv += 1;
	}
	//alert(vv);
	return null;
}

function FindInnerText(objControl, innerText, nestingLevel)
{
	if ((typeof nestingLevel != "undefined")&&(nestingLevel > 100))
		return innerText;
	
	if (typeof innerText == "undefined")
		innerText = "";
	
	if (!objControl)
		return innerText;
	
	if (typeof nestingLevel == "undefined")
		nestingLevel = 0;
	
	var text=objControl.nodeValue;
	if (!text)
		text = "";
	if (objControl.nodeName.toLowerCase() == "br")
		return "\n";
	
	
	for (var i=0; i<objControl.childNodes.length; i++)
	{
		text += FindInnerText(objControl.childNodes[i], objControl.childNodes[i].nodeValue, nestingLevel+1);
	}
	
	return text;
}

function SetControlAttribute(objControl, attName, attValue)
{
	if (typeof objControl.attributes[attName] == "undefined")
	{
		var objAttribute=document.createAttribute(attName);
		objAttribute.value = attValue+"";
		objControl.attributes.setNamedItem(objAttribute);
	}
	objControl.attributes[attName].value = attValue;
}

function PrintArray(arr)
{
	var strResult="";
	for (var key in arr)
	{
		var value=arr[key];
		strResult += "key: "+key+", value: "+value+"\n";
	}
	alert(strResult);
}

function ArrayJoin(arr, delimeter, joinByKeys)
{
	var strResult="";
	for (var key in arr)
	{
		var value=arr[key];
		strResult += (joinByKeys)?key:value;
		strResult += delimeter;
	}
	if (strResult.length > 0)
		strResult = strResult.substr(0, strResult.length-1);
	return strResult;
}

function DigitOnly(event)
{
	//get IE event:
	if (typeof event == "undefined")
		event = window.event;
	
	//get key code, or char code for standard browser and convert into character::
	var keyCode=event.keyCode;
	if ((typeof keyCode == "undefined")||(keyCode == 0))
		keyCode = event.charCode;
	var myChar=String.fromCharCode(keyCode);
	
	//allow backspace:
	if (keyCode == 8)
		return true;
	
	//verify the character is digit:
	return (isNaN(parseInt(myChar)) == false);
}

function ConfirmNumeric(objControl, minValue, maxValue)
{
	var value=objControl.value;
	value=parseInt(value);
	if (isNaN(value))
		return ErrorAndFocus(objControl, String_Grid["ErrorNoValue"]);
	if ((value < minValue)||(value > maxValue))
		return ErrorAndFocus(objControl, String_Grid["ErrorOutOfRange"]);
	return true;
}

function ErrorAndFocus(objControl, msg)
{
	alert(msg);
	objControl.focus();
	objControl.select();
	return false;
}

function FindNodeAfter(node) {
	var parent=node.parentNode;
	for (var i=0; i<parent.childNodes.length-1; i++) {
		if (parent.childNodes[i] == node)
			return parent.childNodes[i+1];
	}
	return 0;
}

function PushNode(parent, node, after) {
	if (after) {
		parent.insertBefore(node, after);
	}
	else {
		parent.appendChild(node);
	}
}

function SwapNode(node1, node2)
{
	var parent1=node1.parentNode;
	var parent2=node2.parentNode;
	var after1=FindNodeAfter(node1);
	var after2=FindNodeAfter(node2);
	PushNode(parent1, node2, after1);
	PushNode(parent2, node1, after2);
}

function FindChildNode(parentNode, childName, blnPartial) {
	if (typeof blnPartial == "undefined")
		blnPartial = false;
	
	var parentName = parentNode.nodeName.toLowerCase();
	var childName = childName.toLowerCase();
	if (blnPartial) {
		if (parentName.indexOf(childName) >= 0)
			return parentNode;
	} else {
		if (parentName == childName)
			return parentNode;
	}
	
	for (var i=0; i<parentNode.childNodes.length; i++) {
		var result=FindChildNode(parentNode.childNodes[i], childName, blnPartial);
		if (result)
			return result;
	}
	
	return 0;
}

function FindChildNodeById(parentNode, childID, blnPartial) {
	if (typeof blnPartial == "undefined")
		blnPartial = false;
	
	var parentName = "";
	try {
		parentName = parentNode.id.toLowerCase();
	} catch (e) {
	}
	
	var childName = childID.toLowerCase();
	if (blnPartial) {
		if (parentName.indexOf(childName) >= 0)
			return parentNode;
	} else {
		if (parentName == childName)
			return parentNode;
	}
	
	try {
		for (var i=0; i<parentNode.childNodes.length; i++) {
			var result=FindChildNodeById(parentNode.childNodes[i], childID, blnPartial);
			if (result)
				return result;
		}
	} catch (e) {
	}
	
	return 0;
}

function FindChildNodesByType(parentNode, childNodeName, arrNodes) {
	if (typeof arrNodes == "undefined")
		arrNodes = new Array();
	
	if (parentNode.nodeName.toLowerCase() == childNodeName)
		arrNodes[arrNodes.length] = parentNode;
	
	for (var i=0; i<parentNode.childNodes.length; i++)
		FindChildNodesByType(parentNode.childNodes[i], childNodeName, arrNodes);
	
	return arrNodes;
}

function cursor_wait(o) {
	document.body.style.cursor = "wait";
	o.style.cursor = "wait";
}

function cursor_clear(o) {
	document.body.style.cursor = "default";
	o.style.cursor = "default";
}

function ApplyGlobalLinks(objTable){
	if ((typeof objTable != "undefined")&&(objTable.nodeName.toLowerCase() == "table"))
	{
		var strTableLink=objTable.getAttribute("global_link");
		if ((!strTableLink)||(strTableLink.length == 0))	
			return false;
		var strLinkClass=objTable.getAttribute("global_link_class");
		if (!strLinkClass)
			strLinkClass="";
		for (var row=0; row<objTable.rows.length; row++) {
			var objRow=objTable.rows[row];
			var rowLink=objRow.getAttribute("global_link");
			if (!rowLink)
				rowLink = "";
			var strLink=strTableLink;
			if (rowLink.length > 0) {
				var arrParts=rowLink.split(",");
				for (var i=1; i<=arrParts.length; i++)
					strLink = strLink.replace("%"+i, arrParts[i-1]);
			}
			for (var col=0; col<objRow.cells.length; col++) {
				var objCell=objRow.cells[col];
				if (objCell.nodeName.toLowerCase() == "th")
					break;
				var strHTML=objCell.innerHTML;
				if ((strHTML.length > 0)&&(strHTML != "&nbsp;")&&
					(strHTML.toLowerCase().indexOf(strLink.toLowerCase()) < 0))
				{
					var strLinkHtml="<a href=\""+strLink+"\"";
					if (strLinkClass.length > 0)
						strLinkHtml += " class=\""+strLinkClass+"\"";
					strLinkHtml += ">";
					strHTML = strLinkHtml+strHTML+"</a>";
					objCell.innerHTML = strHTML;
				}
			}
		}
		return true;
	}
	
	var arrTables=document.getElementsByTagName("table");
	for (var i=0; i<arrTables.length; i++) {
		var curTable=arrTables[i];
		var strTemp=curTable.getAttribute("global_link");
		if ((strTemp)&&(strTemp.length > 0))
			ApplyGlobalLinks(curTable);
	}
	return true;
}

function ToggleVisibility(elementID)
{
	var objControl=document.getElementById(elementID);
	if (objControl)
	{
		if (objControl.style.display == "none")
		{
			objControl.style.display = ""; //inline
		}
		else
		{
			objControl.style.display = "none";
		}
	}
}

function GetSelectedValue(objCombo)
{
	if (objCombo.selectedIndex < 0)
		return "";
	return objCombo.options[objCombo.selectedIndex].value;
}

function AutoCalcSum(objTable, performCalculation)
{
	if (objTable.nodeName.toLowerCase() != "table")
		objTable = FindAncestor(objTable, "table");
	
	if (typeof performCalculation == "undefined")
		performCalculation = false;
	
	for (var row=0; row<objTable.rows.length; row++) {
		var objRow=objTable.rows[row];
		var objTotElement=FindChildNodeById(objRow, "total_", true);
		var sum=0;
		if (objTotElement) {
			for (var col=0; col<objRow.cells.length; col++) {
				var objCell=objRow.cells[col];
				var arrInputs = FindChildNodesByType(objCell, "input");
				for (var i=0; i<arrInputs.length; i++) {
					var objInput=arrInputs[i];
					if ((objInput.type == "text")&&(objInput.id.indexOf("total_") < 0)) {
						if (performCalculation) {
							sum += ToIntDef(objInput.value, 0);
						} else {
							objInput.onkeyup = new Function("AutoCalcSum(this, true);");
						}
					}
				}
			}
			if (performCalculation)
				objTotElement.innerHTML = sum+"";
		}
	}
}

var _pageContentsID="";
function CommentClicked(objLink, strContainerID) {
	var objContainer=document.getElementById(strContainerID);
	if (!objContainer)
		return false;
	var blnHidden=(objContainer.style.display == "none");
	objContainer.style.display = "";
	var diff=objContainer.offsetHeight;
	var objRow=FindAncestor(objLink, "tr");
	if (blnHidden == false) {
		objContainer.style.display = "none";
		diff = (diff*(-1));
		objRow.style.backgroundColor = "";
	}
	else {
		objRow.style.backgroundColor = "#D0E2E6";
	}
	var objPageContents=0;
	if (_pageContentsID.length == 0) {
	   objPageContents = GetElementByClassName("PageContentsPanel", new Array("div", "table"));
	   _pageContentsID = objPageContents.id;
	}
	objPageContents = document.getElementById(_pageContentsID);
	ResizeContents(objPageContents);
	return true;
}

function GetOffsetWidth(element) {

	if ((element.style.length > 0)&&(element.style.width.indexOf("%") > 0)) {
		var strWidth=element.style.width.replace("%", "");
		var iPercent=parseInt(strWidth);
		return (iPercent/100)*(GetOffsetWidth(element.parentNode));
	}
	return element.offsetWidth;
}

function MakeRoundCorners(strElementID, strBgColor) {
	return false;
	var objDiv=document.getElementById(strElementID);
	if (objDiv.offsetHeight < 15)
		return;
	var elementWidth=objDiv.offsetWidth; //GetOffsetWidth(objDiv);
	for (var i=0; i<10; i++) {
		for (j=0; j<(10-i); j++) {
			var oppositePosX=(elementWidth-j);
			objDiv.appendChild(GeneratePixel(strBgColor, j, i));
			objDiv.appendChild(GeneratePixel(strBgColor, oppositePosX, i));
		}
		if (i == 0) {
			var oppositePosX=(elementWidth-10);
			objDiv.appendChild(GeneratePixel(strBgColor, 10, i));
			objDiv.appendChild(GeneratePixel(strBgColor, oppositePosX, i));
		}
	}
}

function GeneratePixel(strColor, posX, posY) {
	var objPixel=document.createElement("div");
	objPixel.style.backgroundColor = strColor;
	objPixel.style.position = "absolute";
	objPixel.style.top = posY+"px";
	objPixel.style.left = posX+"px";
	objPixel.style.fontSize = "1px";
	objPixel.innerHTML = "&nbsp;";
	return objPixel;
}

function CompetitionFilterChanged(objCombo) {
	var strName = objCombo.name;
	var blnGotValue = (objCombo.selectedIndex > 0);
	var btnCompetitors = document.getElementById("btnCompetitorsReport");
	var btnTeams = document.getElementById("btnTeamsReport");
	var btnFull = document.getElementById("btnFullReport");
	if (strName == "competition") {
		ChangeFilterButton(btnCompetitors, !blnGotValue);
	} else if (strName == "team") {
		ChangeFilterButton(btnFull, !blnGotValue);
	}
}

function ChangeFilterButton(objButton, blnDisabled) {
	if (!objButton)
		return;
	objButton.disabled = blnDisabled;
	var strElementID = objButton.getAttribute("disabledtextid");
	if (strElementID.length > 0) {
		var objSpan = document.getElementById(strElementID);
		if (objSpan)
			objSpan.style.display = (blnDisabled)?"":"none";
	}
}

function StartsWith(s1, s2)
{
	if (s1.length < s2.length)
		return false;
	return (s1.substr(0, s2.length) == s2);
}

function IsValidDate(strValue, minYear, maxYear) {
	var parts = strValue.split("/");
	if (parts.length != 3)
		return false;
	var day = parseInt(Number(parts[0]));
	var month = parseInt(Number(parts[1]));
	var year = parseInt(Number(parts[2]));
	if (isNaN(day) || isNaN(month) || isNaN(year))
		return false;
	if (day < 1 || day > 31 || month < 1 || month > 12 || year < minYear || year > maxYear)
		return false;
	var date = new Date();
	date.setYear(year);
	date.setDate(1);
	date.setMonth(month - 1);
	date.setDate(day);
	return date.getDate() === day && date.getMonth() === month - 1;
}