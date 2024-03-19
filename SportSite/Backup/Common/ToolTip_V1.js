	/*
		ToolTip Version 1.0
		-------------------
		Can be applied to any html control.
		To activate tool tip, have this attribute:
			tooltip_v1="tooltip text here"
		And have this code inside the control:
			onmouseover="ShowToolTip(event);"
			onmouseout="HideToolTip(event);"
		For example:
			<span tooltip_v1="hello i'm the tooltip.<br />I can be some lines." 
				onmouseover="ShowToolTip(event);" onmouseout="HideToolTip(event);">
			some text go here...</span>
		The above html would cause the tooltip to appear over the text when mouse over.
	*/

	//general properties:
	var _tooltipPauseBeforeShow=100; //miliseconds to pause before showing the tooltip.
	var _tooltipDelay=12; //seconds before auto hiding. set 0 to make it permanent.
	var _tooltipBgColor="yellow"; //background color of tooltip box
	var _tooltipTextColor="black"; //color of tooltip text.
	var _tooltipFontFamily="Arial"; //font family of tooltip text.
	var _tooltipFontSize=12; //font size of tooltip text.
	var _tooltipFontBold=true; //whether the tooltip text will be bold or not
	
	//private data:
	var _tooltipContainerName="TooltipContainer_V1";
	
	function ShowToolTip(event)
	{
		//get container:
		var objContainer=document.getElementById(_tooltipContainerName);
		
		//get source element:
		var objSource=CStrDef(event.srcElement, event.target);
		
		if ((typeof objSource == "undefined")||(!objSource))
			return false;
		
		//create if does not exist:
		if (!objContainer)
		{
			objContainer = document.createElement("div");
			objContainer.id = _tooltipContainerName;
			objContainer.style.backgroundColor = _tooltipBgColor;
			objContainer.style.color = _tooltipTextColor;
			objContainer.style.fontFamily = _tooltipFontFamily;
			objContainer.style.fontSize = _tooltipFontSize;
			if (_tooltipFontBold)
				objContainer.style.fontWeight = "bold";
			objContainer.style.position = "absolute";
			objContainer.style.display = "none";
			document.body.appendChild(objContainer);
		}
		
		//get mouse position:
		var mouseX=CStrDef(event.x, event.pageX);
		var mouseY=CStrDef(event.y, event.pageY);
		
		//get body scroll:
		var scrollX=document.body.scrollLeft;
		var scrollY=document.body.scrollTop;
		
		//set container position:
		objContainer.style.left = (mouseX+scrollX)+"px";
		objContainer.style.top = (mouseY+scrollY)+"px";
		
		//read tooltip text:
		var strText=(objSource.attributes["tooltip_v1"])?objSource.attributes["tooltip_v1"].value:"";
		
		//verify we have something to display:
		if (strText.length == 0)
			return false;
		
		//set container text:
		objContainer.innerHTML = strText;
		
		//show container:
		setTimeout("_ShowTooltipContainer();", _tooltipPauseBeforeShow);
		
		//set timer if needed:
		if (_tooltipDelay > 0)
			setTimeout("HideToolTip();", _tooltipDelay*1000);
		
		return true;
	}
	
	
	function HideToolTip(event)
	{
		//get container:
		var objContainer=document.getElementById(_tooltipContainerName);
		if (objContainer)
		{
			objContainer.style.display = "none";
		}
	}
	
	function _ShowTooltipContainer()
	{
		//get container:
		var objContainer=document.getElementById(_tooltipContainerName);
		
		if (objContainer)
		{
			objContainer.style.display = "block";
		}
	}
	
	function CStrDef(strValue, strDefault)
	{
		if (typeof strValue == "undefined")
			return strDefault;
		return strValue;
	}