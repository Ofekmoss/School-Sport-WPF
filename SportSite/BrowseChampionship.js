var _rd = new Array();

var _lastIndex=-1;
var _showCategory = true;
var _parentPanelID = "";
var _lastCategoryID = null;
var _lastChampName = null;

function RegionChanged(regionList) {
	var selRegion = regionList.options[regionList.selectedIndex].value;
	var champList = document.getElementById("ChampChampionship");
	var categoryList = document.getElementById("ChampCategory");
	ClearCombo(champList);
	if (categoryList)
		ClearCombo(categoryList);
	document.getElementById("btnSetChamp").disabled = "disabled";
	for (var region in _rd) {
		if (region == selRegion) {
			var arrChamps = _rd[region]["champs"];
			for (var champ in arrChamps) {
				AddListItem(champList, arrChamps[champ]["name"], champ);
			}
			return true;
		}
	}
	return false;
}

function ChampionshipChanged(champList) {
	if (_showCategory == false) {
		var strDisabled = (champList.selectedIndex >= 0) ? "" : "disabled";
		document.getElementById("btnSetChamp").disabled = strDisabled;	
		return true;
	}
	var selChamp = champList.options[champList.selectedIndex].value;
	var categoryList = document.getElementById("ChampCategory");
	ClearCombo(categoryList);
	document.getElementById("btnSetChamp").disabled = "disabled";
	for (var region in _rd) {
		var arrChamps = _rd[region]["champs"];
		for (var champ in arrChamps) {
			if (champ == selChamp) {
				var arrCategories = arrChamps[champ]["categories"];
				for (var category in arrCategories) {
					AddListItem(categoryList, arrCategories[category], category);
				}
				return true;
			}
		}
	}
	return false;
}

function CategoryChanged(categoryList) {
	var strDisabled = (categoryList.selectedIndex >= 0) ? "" : "disabled";
	document.getElementById("btnSetChamp").disabled = strDisabled;
}

function SetChampionship() {
	var categoryList = document.getElementById("ChampCategory");
	var champList = document.getElementById("ChampChampionship");
	var categoryID = "";
	if (_showCategory)	
		categoryID = categoryList.options[categoryList.selectedIndex].value;
	var champName = champList.options[champList.selectedIndex].text;
	var categoryName = categoryList.options[categoryList.selectedIndex].text;
	var champID = champList.options[champList.selectedIndex].value;
	var objChampName = _lastChampName || document.forms[0].elements["ChampionshipName_" + _lastIndex];
	var objCategoryID = _lastCategoryID || document.forms[0].elements["CategoryID_"+_lastIndex];
	var objChampionshipID = document.forms[0].elements["Championship_ID_"+_lastIndex];
	objChampName.value = champName + " " + categoryName;
	if (_showCategory)
		objCategoryID.value = categoryID;
	else
		objChampionshipID.value = champID;
	document.getElementById("ChooseCategory").style.display = "none";
	document.getElementById(_parentPanelID).style.display = "block";
	//document.forms[0].elements["btnChooseChamp_"+_lastIndex].focus();
}

function CancelSetChampionship() {
	document.getElementById("ChooseCategory").style.display = "none";
	document.getElementById(_parentPanelID).style.display = "block";
	//document.forms[0].elements["btnChooseChamp_"+_lastIndex].focus();
}

function ChooseChampCategory(event, objButton) {
	var index = parseInt((objButton.name.split("_"))[1]);
	_lastIndex = index;
	document.getElementById(_parentPanelID).style.display = "none";
	document.getElementById("ChooseCategory").style.display = "block";
	var regionList = document.getElementById("ChampRegion");
	var champList = document.getElementById("ChampChampionship");
	var categoryList = document.getElementById("ChampCategory");
	var oParent = $(objButton).parent("div");
	var objCategoryID = oParent.find("input[name='CategoryID_" + index + "']").get(0);
	_lastCategoryID = objCategoryID;
	_lastChampName = oParent.find("input[name='ChampionshipName_" + index + "']").get(0);
	var objChampionshipID = document.forms[0].elements["Championship_ID_" + index];	
	var categoryID = "";
	var champID = "";
	if (_showCategory) {
		categoryID = objCategoryID.value;
		ClearCombo(categoryList);
	}
	else {
		champID = objChampionshipID.value;
	}
	ClearCombo(champList);
	regionList.selectedIndex = -1;
	if (categoryID.length > 0 || champID.length > 0) {
		var blnStop = false;
		for (var region in _rd) {
			if (blnStop)
				break;
			var arrChamps = _rd[region]["champs"];
			for (var champ in arrChamps) {
				if (blnStop)
					break;
				if (_showCategory == false && champ == champID) {
					SetComboValue(regionList, region);
					RegionChanged(regionList);
					SetComboValue(champList, champ);
					ChampionshipChanged(champList);
					blnStop = true;
					break;
				}					
				var arrCategories=arrChamps[champ]["categories"];
				for (var category in arrCategories) {
					if (category == categoryID) {
						SetComboValue(regionList, region);
						RegionChanged(regionList);
						SetComboValue(champList, champ);
						ChampionshipChanged(champList);
						SetComboValue(categoryList, category);
						CategoryChanged(categoryList);
						blnStop = true;
						break;
					}
				}
			}
		}
	}
	regionList.focus();
}

function ClearChamp(event, oLink, index) {
	var oParent = $(oLink).parent("div");
	var objCategoryID = oParent.find("input[name='CategoryID_" + index + "']").get(0);
	var objChampName = oParent.find("input[name='ChampionshipName_" + index + "']").get(0);
	objChampName.value = "";
	if (_showCategory)
		objCategoryID.value = "";
	else
		document.forms[0].elements["Championship_ID_" + index].value = "";
}
