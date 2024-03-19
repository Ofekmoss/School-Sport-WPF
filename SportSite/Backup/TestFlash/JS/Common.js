var _flashMoviesCount=0;
var _flash_picURL="";
var arrOnloadCommands = new Array();

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
				//alert(strVarName+" = "+strVarValue);
				if (strVarName == "picUrl")
					_flash_picURL = strVarValue;
			}
		}
	}
	so.write(containerID);
	_flashMoviesCount++;
}

function AssignDimensions(strDropdownId, strWidthId, strHeightId)
{
	var objCombo = document.getElementById(strDropdownId);
	var strValue = objCombo.value;
	if (strValue && strValue.length > 0)
	{
		strValue = strValue.toLowerCase();
		var arrTemp = strValue.split("x");
		for (var i = 0; i < arrTemp.length - 1; i++)
		{
			var nWidth = parseInt(arrTemp[i]);
			var nHeight = parseInt(arrTemp[i + 1]);
			if (!isNaN(nWidth) && !isNaN(nHeight))
			{
				document.getElementById(strWidthId).value = nWidth + "";
				document.getElementById(strHeightId).value = nHeight + "";
				break;
			}
		}
	}
}

function AddOnloadCommand(strCommand)
{
	arrOnloadCommands[arrOnloadCommands.length] = strCommand;
}

window.onload = function WindowLoad(event)
{
	for (var i = 0; i < arrOnloadCommands.length; i++)
	{
		var strCommand = arrOnloadCommands[i];
		eval(strCommand);
	}
}