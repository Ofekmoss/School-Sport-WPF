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
		catch(ex) {
			try {
				objXML = new ActiveXObject("Microsoft.XMLHTTP");
			}
			catch(ex) {
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

/*
	this function send request to the given URL via
	GET method.
*/
function SendAjaxRequest(strURL, strCallbackFunction) {
	//get xmlhttp component:
	var objXML = GetXmlHTTP();
	
	//got anything?
	if (!objXML)
		return false;
	
	//attach local callback function:
	objXML.onreadystatechange = function()
	{
		AjaxPageLoad(objXML, strCallbackFunction);
	}
	
	//send request:
	objXML.open("GET", strURL, true);
	objXML.send(null);
} //end function SendAjaxRequest

/*
	this function is called after we got response
	from the server.
*/
function AjaxPageLoad(objXML, strCallbackFunction) {
	//ready?
	if (objXML.readyState != 4)
		return false;

	//get status:
	var status=objXML.status;
	
	//maybe not successful?
	if (status != 200) {
		alert("AJAX: server status "+status);
		return false;
	}
	
	//get response text:
	var strResponse = objXML.responseText;
	
	//call function
	eval(strCallbackFunction+"(\""+strResponse.replace(/"/g, "\\\"")+"\");");
	return true;
} //end function AjaxPageLoad

//window.location.href.indexOf("http")