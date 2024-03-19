function AddTeamClick()
{
	//get pointer to the controls:
	var objImage=document.images["AddTeam"];
	var objPanel=document.getElementById("PnlAddTeam");
	
	//hide image and show Add Team panel:
	objImage.style.display = "none";
	objPanel.style.display = "inline";
}

function CancelAddTeam()
{
	//get pointer to the controls:
	var objImage=document.images["AddTeam"];
	var objPanel=document.getElementById("PnlAddTeam");
	
	//hide Add Team panel and show image:
	objPanel.style.display = "none";	
	objImage.style.display = "inline";
}

function AutoWrap(strName)
{
	var objTable=document.getElementById(strName);
	var autoWrapOffset=25;
	//alert(ReplaceGlobal("hello&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", "&nbsp;", "a"));
	//Print(objTable);
	for (var row=0; row<objTable.rows.length; row++)
	{
		for (var col=0; col<objTable.rows[row].cells.length; col++)
		{
			var objCell=objTable.rows[row].cells[col];
			if (objCell.childNodes.length == 0)
				continue;
			var vv=0; //safe loop!	
			var bigger=false;		
			autoWrapOffset = objCell.childNodes[0].innerHTML.length;
			var realWidth=MeasureControl(objCell)+autoWrapOffset;
			var cellWidth=objCell.clientWidth;
			if (realWidth >= cellWidth)
			{
				//alert("scroll: "+objCell.scrollWidth+", client: "+objCell.clientWidth);
				//Print(objCell, "numbers");
				//alert(objCell.innerHTML+"\n"+MeasureControl(objCell));
				//alert(objCell.innerHTML);
				//alert(GetArrayKeys(SplitIntoTags(objCell.innerHTML)));
				objCell.title = objCell.childNodes[0].innerHTML;
				bigger = true;
			}
			
			while (realWidth >= cellWidth)
			{
				//assume all text is in the first element and without additional tags.
				//alert(realWidth);
				var cellText=objCell.childNodes[0].innerHTML;
				if ((cellText.length == 0)||(vv > 1000))
					break;
				var index=cellText.length-1;
				while ((index >= 0)&&(cellText.substr(index, 1) == " "))
					index--;
				//alert(cellText.length+" , "+index);
				objCell.childNodes[0].innerHTML = cellText.substr(0, index)+cellText.substr(index+1, cellText.length);
				vv++;
				realWidth = MeasureControl(objCell)+autoWrapOffset;
			}
			
			if (bigger)
				objCell.childNodes[0].innerHTML = objCell.childNodes[0].innerHTML.substr(0, objCell.childNodes[0].innerHTML.length-3)+"...";
		}
	}
}

function SplitIntoTags(strHtml)
{
	//return array of html tags. first element is text outside any tag
	var result=new Array();
	var strBuffer="";
	var i=0;
	while (i<strHtml.length)
	{
		if (strHtml.substr(i, 1) == "<")
		{
			var strTag="<";
			i += 1;
			while ((i < strHtml.length)&&(strHtml.substr(i, 1) != ">"))
			{
				strTag += strHtml.substr(i, 1);
				i += 1;
			}
			if (strHtml.substr(i, 1) == ">")
				i += 1;
			strTag += ">";
			result[strTag] = "hello";
		}
		else
		{
			i++;
		}
	}
	return result;
}

function GetArrayKeys(arr)
{
	var result=new Array();
	for (var item in arr)
		result[result.length] = item;
	return result;
}

function SortTable(objHeader)
{
	//check if need to add the sort_order attribute:
	if (typeof objHeader.attributes["sort_order"] == "undefined")
	{
		var objAttribute=document.createAttribute("sort_order");
		objAttribute.value = "descending";
		objHeader.attributes.setNamedItem(objAttribute);
	}
	//alert(objHeader.attributes["sort_order"]);
	
	//get sort order:
	var sortOrder=objHeader.attributes["sort_order"].value;
	
	//get cell index: (in its row)
	var cellIndex=objHeader.cellIndex;
	
	//find the parent table of the given row
	var objTable=FindAncestor(objHeader, "table");
	
	//abort if no parent table:
	if (!objTable)
	{
		alert("can't sort: general error #001: no ancestor table");
		return false;
	}
	
	//build array with column values:
	var arrColValues=new Array();
	for (var i=0; i<objTable.rows.length; i++)
	{
		var objRow=objTable.rows[i];
		if (objRow != objHeader.parentNode)
		{
			if ((objHeader.parentNode.nodeName.toLowerCase() == "tr")&&
				(objRow.cells.length == objHeader.parentNode.cells.length))
			{
				var cellText=FindInnerText(objRow.cells[cellIndex]);
				arrColValues[arrColValues.length] = cellText;
			}
		}
	}
	
	//decide if numeric:
	var isColNumeric=IsNumeric(arrColValues, true);
	
	//iterate through the rows, use MaxSort algorithm to sort:
	for (var i=0; i<objTable.rows.length; i++)
	{
		//get current row
		var objRow=objTable.rows[i];
		
		//continue the loop if this is headers row: 
		if (objRow == objHeader.parentNode)
			continue;
		
		//also can't sort if the amount of cells in current row is different:
		if ((objHeader.parentNode.nodeName.toLowerCase() == "tr")&&
			(objRow.cells.length != objHeader.parentNode.cells.length))
		{
			continue;
		}
		
		//store the value and index of current row
		var currentValue=FindInnerText(objTable.rows[i].cells[cellIndex]);
		var currentIndex=i;
		
		//make numeric if needed:
		if (isColNumeric)
			currentValue = (currentValue)*(-1)*(-1);
		
		//iterate through next row, search for bigger/smaller values:
		for (j=i+1; j<objTable.rows.length; j++)
		{
			//get row in current position:
			var objRow2=objTable.rows[j];
			
			//continue the loop if this is headers row: 
			if (objRow2 == objHeader.parentNode)
				continue;
			
			//get value of the row in current position:
			var currentValue2=FindInnerText(objTable.rows[j].cells[cellIndex]);
			
			//make numeric if needed:
			if (isColNumeric)
				currentValue2 = (currentValue2)*(-1)*(-1);
			
			//compare current value with mix/max value we found so far
			if (((sortOrder == "descending")&&(currentValue2 < currentValue))||
				((sortOrder == "ascending")&&(currentValue2 > currentValue)))
			{
				//found new mix/max value, store it:
				currentValue = currentValue2;
				currentIndex = j;
			}
			
		} //end loop over table rows
		
		//swap the rows if max/min row is not current row
		if (currentIndex != i)
			SwapTableRows(objTable, i, currentIndex);
	} //end loop over table rows
	
	//change the sort order:
	objHeader.attributes["sort_order"].value = (sortOrder == "descending")?"ascending":"descending";
}

function SwapTableRows(objTable, index1, index2)
{
	var row1=objTable.rows[index1];
	var row2=objTable.rows[index2];
	for (var i=0; i<row1.cells.length; i++)
	{
		var strTemp=row1.cells[i].innerHTML;
		row1.cells[i].innerHTML = row2.cells[i].innerHTML;
		row2.cells[i].innerHTML = strTemp;
	}
}

function IsNumeric(arr, emptyIsNumeric)
{
	for (var i=0; i<arr.length; i++)
	{
		if ((arr[i].length == 0)&&(emptyIsNumeric))
			continue;
		var num=(arr[i])*(-1)*(-1);
		if (isNaN(num))
			return false;
	}
	return true;
}

function ListItemClick(event)
{
	if ((typeof event == "undefined")||(!event))
		event = window.event;
	var objListItem=event.currentTarget||event.srcElement;
	for (var i=0; i<objListItem.childNodes.length; i++)
	{
		var node=objListItem.childNodes[i];
		if (node.nodeName.toUpperCase() == "UL")
		{
			if (node.style.display == "none")
			{
				node.style.display = "block";
			}
			else
			{
				node.style.display = "none";
			}
		}
	}
	if (window.getSelection)
	{
		document.createRange().setStart(node, 0);
	}
	else
	{
		//alert(document.selection);
	}
	if (event.stopPropagation)
		event.stopPropagation();
	event.cancelBubble = true;
}

function ClearCombo(objCombo) {
	while (objCombo.options.length > 0)
		objCombo.removeChild(objCombo.options[0]);
}

function AddListItem(objCombo, strText, strValue) {
	var option=new Option();
	option.value = strValue;
	option.text = strText;
	objCombo.options[objCombo.options.length] = option;
}

function ShowElementInPosition(event, strElementID, offsetX, offsetY, absoluteX, absoluteY) {
	if ((typeof event == "undefined")||(!event))
		event = window.event;
	var eventX=event.x||event.pageX;
	var eventY=event.y||event.pageY;
	var scrollX=document.body.scrollLeft;
	var scrollY=document.body.scrollTop;
	var xPos=(absoluteX >= 0)?(absoluteX):(eventX+scrollX+offsetX);
	var yPos=(absoluteY >= 0)?(absoluteY):(eventY+scrollY+offsetY);
	var element=document.getElementById(strElementID);
	if (element) {
		element.style.left = xPos+"px";
		element.style.top = yPos+"px";
		element.style.display = "block";
	}
}

function SetComboValue(objCombo, strValue) {
	for (var i=0; i<objCombo.options.length; i++) {
		if (objCombo.options[i].value == strValue) {
			objCombo.options[i].selected = true;
			objCombo.selectedIndex = i;
		}
	}
}

function HideBlankRadioButtons()
{
	var arrInputs = document.getElementsByTagName("input");
	for (var i = 0; i < arrInputs.length; i++)
	{
		var oCurInput = arrInputs[i];
		if (oCurInput.type == "radio" && oCurInput.value.length == 0)
				oCurInput.style.display = "none";
	}
}