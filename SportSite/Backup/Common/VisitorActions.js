function PrintElement(strElementID, strPageTitle)
{
	var element = document.getElementById(strElementID);
	if (!element) {
		alert("print element: invalid ID (" + strElementID + ")");
		return false;
	}
	
	if (typeof strPageTitle == "undefined")
		strPageTitle = "";
	
	var winWidth=element.offsetWidth+15;
	var winHeight=element.offsetHeight+50;
	
	for (var i=0; i<element.childNodes.length; i++) {
		if (typeof element.childNodes[i].className != "undefined") {
			if (element.childNodes[i].className.toLowerCase() == "printable") {
				var strOriginalDisplay = element.childNodes[i].style.display;
				element.childNodes[i].style.display = "block";
				var extraHeight = element.childNodes[i].offsetHeight;
				winHeight += extraHeight;
				element.childNodes[i].style.display = strOriginalDisplay;
			}
		}
	}
	
	var screenWidth = screen.width;
	var screenHeight = screen.height;	
	var winLeft = parseInt((screenWidth/2)-(winWidth/2));

	var winTop = 100;
	if (winHeight > (screenHeight-135)) {
		winHeight = (screenHeight-135);
		winTop = 5;
	}
			
	var strHTML = element.innerHTML;
	var tempIndex = strHTML.indexOf("FlashVars");
	var s = new String();
	if (tempIndex >= 0) {
		strHTML = strHTML.substr(0, tempIndex+9+9)+"picUrl="+_flash_picURL+strHTML.substr(tempIndex+9+9, strHTML.length);
	}
	//picUrl=/ISF/SportSite/Thumbnails/258_172/_ISF_SportSite_Images_Articles_article_21_331018.jpg
	var objWindow = window.open("about:blank", "print_window", "left="+winLeft+",top="+winTop+",width="+winWidth+",height="+winHeight+",titlebar=no,scrollbars=yes,toolbar=no");
	objWindow.document.write("<html>");
	objWindow.document.write("<head>");
	if (strPageTitle.length > 0)
		objWindow.document.write("<title>"+ReplaceGlobal(strPageTitle, "~", "\"")+"</title>");
	objWindow.document.write("<link rel=\"stylesheet\" type=\"text/css\" href=\"SportSite.css\" />");
	objWindow.document.write("<style type=\"text/css\" media=\"print\">");
	objWindow.document.write(" #PrintPanel {display: none;}");
	objWindow.document.write(" .small_when_printing, .small_when_printing table, .small_when_printing div, .small_when_printing td, .small_when_printing th {font-size: 10px;}");
	objWindow.document.write("</style>");
	objWindow.document.write("</head>");
	objWindow.document.write("<body>");
	objWindow.document.write("<div align=\"right\" dir=\"rtl\">");
	objWindow.document.write(strHTML);
	objWindow.document.write("<div id=\"PrintPanel\">");
	objWindow.document.write("<hr />");
	objWindow.document.write("<button type=\"button\" onclick=\"window.print(); window.close();\">"+_caption_print+"</button>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
	objWindow.document.write("<button type=\"button\" onclick=\"window.close();\">"+_caption_cancel+"</button>");
	objWindow.document.write("</div>");
	objWindow.document.write("</div>");
	objWindow.document.write("</body>");
	objWindow.document.write("</html>");
	objWindow.document.close();
	var arrPrintedInputs = objWindow.document.getElementsByTagName("input");
	var arrOriginalInputs = element.getElementsByTagName("input");
	if (arrPrintedInputs.length === arrOriginalInputs.length) {
		for (var i = 0; i < arrPrintedInputs.length; i++) {
			var currentPrintedInput = arrPrintedInputs[i];
			var currentOriginalInput = arrOriginalInputs[i];
			var currentType = currentOriginalInput.type || "text";
			switch (currentType.toLowerCase()) {
				case "text":
					currentPrintedInput.value = currentOriginalInput.value;
					break;
				case "checkbox":
					currentPrintedInput.checked = currentOriginalInput.checked;
					break;
			}
		}
	}
}

function OpenActionWindow(strWindowName, strWindowTitle, strElementID, strArticleID, arrText)
{
	if ((strElementID.length == 0)&&(strArticleID.length == 0)&&(strWindowName != "FAQ"))
		return 0;
	
	var winWidth=350;
	var winHeight=420;
	var screenWidth=screen.width;
	var winLeft=parseInt((screenWidth/2)-(winWidth/2));
	var strPage=(strElementID.length > 0)?"SendToFriend.aspx":"AddComment.aspx";
	var objWindow = window.open("about:blank", strWindowName, "left="+winLeft+", top=200, width="+winWidth+", height="+winHeight+", titlebar=no, scrollbars=no, toolbar=no");
	objWindow.document.write("<html>");
	objWindow.document.write("<head>");
	objWindow.document.write("<title>"+strWindowTitle+"</title>");
	objWindow.document.write("<"+"style"+" type=\"text/css\""+">");
	objWindow.document.write(" body, div, td, input, textarea {font-family: Arial; font-size: 12px; color: black;}");
	objWindow.document.write("<"+"/style"+">");
	objWindow.document.write("<"+"script"+" type=\"text/javascript\""+">");
	if (strElementID.length > 0) {
		objWindow.document.write("window.onload = WindowLoad;");
		objWindow.document.write("function WindowLoad(event) {");
		objWindow.document.write("   var element=window.opener.document.getElementById(\""+strElementID+"\");");
		objWindow.document.write("   document.forms[0].elements[\"Contents\"].value = element.innerHTML;");
		objWindow.document.write("}");
	}
	objWindow.document.write("function Validate(objForm) {");
	if ((strElementID.length > 0)||(strArticleID.length > 0)) {
		objWindow.document.write("   if (objForm.elements[\"name\"].value.length == 0) {");
		objWindow.document.write("      alert(\"Please fill your name.\");");
		objWindow.document.write("      objForm.elements[\"name\"].focus();");
		objWindow.document.write("      return false;");
		objWindow.document.write("   }");
	}
	if (strElementID.length > 0) {
		objWindow.document.write("   if (objForm.elements[\"email\"].value.length == 0) {");
		objWindow.document.write("      alert(\"Please fill your email.\");");
		objWindow.document.write("      objForm.elements[\"email\"].focus();");
		objWindow.document.write("      return false;");
		objWindow.document.write("   }");
		objWindow.document.write("   if (objForm.elements[\"friend_email\"].value.length == 0) {");
		objWindow.document.write("      alert(\"Please fill your friend's email.\");");
		objWindow.document.write("      objForm.elements[\"friend_email\"].focus();");
		objWindow.document.write("      return false;");
		objWindow.document.write("   }");
	}
	if (strArticleID.length > 0) {
		objWindow.document.write("   if (objForm.elements[\"subject\"].value.length == 0) {");
		objWindow.document.write("      alert(\"Please fill subject.\");");
		objWindow.document.write("      objForm.elements[\"subject\"].focus();");
		objWindow.document.write("      return false;");
		objWindow.document.write("   }");
	}
	if (strWindowName == "FAQ") {
		objWindow.document.write("   if (objForm.elements[\"contents\"].value.length == 0) {");
		objWindow.document.write("      alert(\"Please fill question.\");");
		objWindow.document.write("      objForm.elements[\"contents\"].focus();");
		objWindow.document.write("      return false;");
		objWindow.document.write("   }");	
	}
	objWindow.document.write("   return true;");
	objWindow.document.write("}");				
	objWindow.document.write("<"+"/script"+">");
	objWindow.document.write("</head>");
	objWindow.document.write("<body>");
	objWindow.document.write("<form action=\""+_rootPath+"/"+strPage+"\" method=\"post\" onsubmit=\"return Validate(this);\">");
	if (strElementID.length > 0) {
		objWindow.document.write("<input type=\"hidden\" name=\"URL\" value=\""+document.location+"\" />");
		objWindow.document.write("<input type=\"hidden\" name=\"Contents\" value=\"\" />");
	}
	if (strArticleID.length > 0)
		objWindow.document.write("<input type=\"hidden\" name=\"article\" value=\""+strArticleID+"\" />");
	if (strWindowName == "FAQ")
		objWindow.document.write("<input type=\"hidden\" name=\"FAQ\" value=\"1\" />");
	objWindow.document.write("<div align=\"right\" dir=\"rtl\"><center>");
	objWindow.document.write("<img src=\""+_rootPath+"/Images/logo_plain.gif\" /><br />");
	objWindow.document.write("<table cellpadding=\"0\" cellspacing=\"0\" style=\"width: 318px; height: 338px; background-image: url("+_rootPath+"/Images/bg_send_to.JPG);\">");
	objWindow.document.write("<tr>");
	objWindow.document.write("<td style=\"width: 20px;\">&nbsp;</td>");
	objWindow.document.write("<td style=\"width: 150px;\">&nbsp;</td>");
	objWindow.document.write("<td style=\"width: 20px;\">&nbsp;</td>");
	objWindow.document.write("<td style=\"width: 108px;\">&nbsp;</td>");
	objWindow.document.write("<td style=\"width: 20px;\">&nbsp;</td>");
	objWindow.document.write("</tr>");
	objWindow.document.write("<tr><td colspan=\"5\" style=\"height: 30px;\">&nbsp;</td></tr>");
	objWindow.document.write("<tr>");
	objWindow.document.write("<td>&nbsp;</td>");
	objWindow.document.write("<td align=\"right\">"+arrText[0]+"</td>");
	objWindow.document.write("<td>&nbsp;</td>");
	objWindow.document.write("<td align=\"right\"><input type=\"text\" name=\"name\" style=\"width: 108px;\" /></td>");
	objWindow.document.write("<td>&nbsp;</td>");
	objWindow.document.write("</tr>");
	objWindow.document.write("<tr><td colspan=\"5\" style=\"height: 10px;\">&nbsp;</td></tr>");
	objWindow.document.write("<tr>");
	objWindow.document.write("<td>&nbsp;</td>");
	objWindow.document.write("<td align=\"right\">"+arrText[1]+"</td>");
	objWindow.document.write("<td>&nbsp;</td>");
	objWindow.document.write("<td align=\"right\"><input type=\"text\" name=\"email\" dir=\"ltr\" style=\"width: 108px;\" /></td>");
	objWindow.document.write("<td>&nbsp;</td>");
	objWindow.document.write("</tr>");
	objWindow.document.write("<tr><td colspan=\"5\" style=\"height: 10px;\">&nbsp;</td></tr>");
	objWindow.document.write("<tr>");
	objWindow.document.write("<td>&nbsp;</td>");
	objWindow.document.write("<td align=\"right\">"+arrText[2]+"</td>");
	objWindow.document.write("<td>&nbsp;</td>");
	if (typeof _strSubject == "undefined")
		_strSubject = "";	
	objWindow.document.write("<td align=\"right\"><input type=\"text\" name=\""+((strElementID.length > 0)?"friend_email":"subject")+"\" dir=\""+((strElementID.length > 0)?"ltr":"rtl")+"\" value=\""+_strSubject+"\" style=\"width: 108px;\" /></td>");
	objWindow.document.write("<td>&nbsp;</td>");
	objWindow.document.write("</tr>");
	objWindow.document.write("<tr><td colspan=\"5\" style=\"height: 10px;\">&nbsp;</td></tr>");
	objWindow.document.write("<tr>");
	objWindow.document.write("<td>&nbsp;</td>");
	objWindow.document.write("<td align=\"right\">"+arrText[3]+"</td>");
	objWindow.document.write("<td>&nbsp;</td>");
	objWindow.document.write("<td align=\"right\"><textarea name=\""+((strElementID.length > 0)?"extra_text":"contents")+"\" style=\"width: 108px;\" rows=\"5\"></textarea></td>");
	objWindow.document.write("<td>&nbsp;</td>");
	objWindow.document.write("</tr>");
	objWindow.document.write("<tr><td colspan=\"5\" style=\"height: 10px;\">&nbsp;</td></tr>");
	objWindow.document.write("<tr>");
	objWindow.document.write("<td>&nbsp;</td>");
	objWindow.document.write("<td align=\"right\">");
	if (strElementID.length > 0) {
		objWindow.document.write("<input type=\"radio\" name=\"SendWhat\" value=\"reference\" checked=\"checked\" />"+arrText[4]+"<br />");
		objWindow.document.write("<input type=\"radio\" name=\"SendWhat\" value=\"html\" />"+arrText[5]);
	} else {
		objWindow.document.write("&nbsp;");
	}
	objWindow.document.write("</td>");
	objWindow.document.write("<td>&nbsp;</td>");
	objWindow.document.write("<td align=\"center\"><input type=\"image\" "+
		"src=\""+_rootPath+"/Images/btn_send_blue.gif\" /></td>");
	objWindow.document.write("<td>&nbsp;</td>");
	objWindow.document.write("</tr>");
	objWindow.document.write("<tr><td colspan=\"5\" style=\"height: 30px;\">&nbsp;</td></tr>");
	objWindow.document.write("</table><br />");
	objWindow.document.write("</center></div>");
	objWindow.document.write("</form>");
	objWindow.document.write("</body>");
	objWindow.document.write("</html>");
	objWindow.document.close();	
	return objWindow;
}

function SendToFriend(strElementID)
{
	var element = document.getElementById(strElementID);
	if (!element) {
		alert("send to friend: invalid ID (" + strElementID + ")");
		return false;
	}
	var objWindow=OpenActionWindow("send_to_friend_window", _send_friend_window_title, 
		strElementID, "", _send_friend_form_texts);
}

function AddComment(articleID)
{
	articleID = parseInt(articleID);
	if ((isNaN(articleID))||(articleID < 0)) {
		alert("add comment: invalid article ID (" + articleID + ")");
		return false;
	}
	var objWindow=OpenActionWindow("add_comment_window", _add_comment_window_title, 
		"", (articleID+""), _add_comment_form_texts);
}