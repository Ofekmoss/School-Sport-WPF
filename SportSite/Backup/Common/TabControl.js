var _currentTabIndex = -1;
function ChangeTab(index)
{
	if (index == _currentTabIndex)
		return;
	
	var strURL = arrTabAjax[index];
	if (strURL.length == 0)
		return;
	
	if (typeof _elementsToPost == "string")
	{
		if (strURL.indexOf("?") < 0)
			strURL += "?";
		var arrElements = _elementsToPost.split(",");
		for (var i = 0; i < arrElements.length; i++)
		{
			var strName = arrElements[i];
			var element = document.forms[0].elements[strName];
			if (element)
			{
				if (strURL.charAt(strURL.length - 1) != "?")
					strURL += "&";
				strURL += strName + "=" + element.value;
			}
		}
	}
	
	_currentTabIndex = index;
	
	var objTable = document.getElementById(_mainTableId);
	var selectedCellIndex = -1;
	var cellCount = 0;
	for (var i = 0; i < objTable.rows[0].cells.length; i += 2)
	{
		objTable.rows[0].cells[i].className = "tab_header";
		if (cellCount == index)
		{
			selectedCellIndex = i;
		}
		cellCount++;
	}
	objTable.rows[0].cells[selectedCellIndex].className = "tab_selected_header";

	var objCell = document.getElementById(_contentCellId);	
	
	if (arrTabStatus[index] == false)
	{
		objCell.innerHTML = arrDisabledTabText[index];
		return;
	}
	
	if (typeof _loadingText != "undefined" && _loadingText.length > 0)
		objCell.innerHTML = _loadingText;
	
	var objFrame = document.getElementById("TabControlFrame");
	objFrame.src = strURL;
}

function RefreshCurrentTab()
{
	var index = _currentTabIndex;
	_currentTabIndex = -1;
	ChangeTab(index);
}

function SetTabStatus(index, status)
{
	arrTabStatus[index] = status;
}

function DisableTab(index)
{
	SetTabStatus(index, false);
}

function EnableTab(index)
{
	SetTabStatus(index, true);
}

function TabLoadCompleted(strData)
{
	var objCell = document.getElementById(_contentCellId);
	objCell.innerHTML = strData;
	//_currentTabIndex = _currentlyLoadingIndex;
	//_tabIsLoading = false;
	//_currentlyLoadingIndex = -1;
}