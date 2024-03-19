var MIN_RESOLUTION = [1024, 768];
var _scrollInternalData = new Array();
var _scrollingElementsCount = 0;
var SCROLL_PIXELS = 5;
var SCROLL_MAX_ITERATIONS = 10;
var PAUSE_WHEN_HOVER = true;
var SCROLLER_LEFT_DIFF = 35; //parseInt($("#EventContents").css("left"))
var SCROLLER_TOP_DIFF_EVENTS = 56; //parseInt($("#EventContents").css("top"))
var SCROLLER_TOP_DIFF_GAMES = 66; //parseInt($("#EventContents").css("top"))
var EVENT_CALENDAR_DESCRIPTION_WIDTH = 450;

//function WindowLoad(evt) {
$(document).ready(function(event) {

	//hide focus:
	$("a").each(function (index) { $(this).attr("hidefocus", "hidefocus"); });

	//shrink images:
	ApplyMaxWidth();

	//resolution
	CheckResolution();

	//Flash:
	RegisterFlashBanner("top_banner", "/ZooZoo/Flash/bees.swf");

	//fancy zoom:
	$('.makefancyzoom').fancyZoom({ directory: '/Images/fancyzoom' });

	//homepage only..
	if (_isHomePage) {
		//slider:
		if ($('#EventGallerySlider').length > 0)
			$('#EventGallerySlider').s3Slider({ timeOut: 5000 });

		//quick poll
		InitPollElements();

		//scroller:
		InitScrollingElement("Events");
		InitScrollingElement("StreetGames");
		if (_scrollingElementsCount > 0)
			window.setInterval("GlobalScroller();", 250);

		//flash:
		RegisterFlashBanner("hp_middle_banner", "/ZooZoo/Flash/poppies.swf")
	}
	else {
		RegisterFlashBanner("side_banner", "/ZooZoo/Flash/poppies.swf")

		//events
		HookEventsCode();

		//weekly game:
		ApplyWeeklyGame();
		ApplyWroteOnUs();

		//contact us:
		ApplyContactUs();
	}
});

function ApplyContactUs() {
	if ($(".contactus_input").length == 0)
		return;

	$("#contactus_input_name").focus();

	$(".contactus_input").keypress(function (e) {
		var keyCode = e.which || e.keyCode;
		if (keyCode == 13 && this.id != "contactus_input_message") {
			$("#contactus_submit_button").click();
			return false;
		}
		return true;
	});



	$("#contactus_submit_button").click(function () {
		var oNameInput = $("#contactus_input_name");
		var oEmailInput = $("#contactus_input_email");
		var oSubjectInput = $("#contactus_input_subject");
		var strName = jQuery.trim(oNameInput.text());
		var strEmail = jQuery.trim(oEmailInput.text());
		var strSubject = jQuery.trim(oSubjectInput.text());
		var strMessage = jQuery.trim($("#contactus_input_message").html());

		if (strName.length == 0)
			return MarkInvalidField(oNameInput);
		if (!IsValidEmail(strEmail))
			return MarkInvalidField(oEmailInput);
		if (strSubject.length == 0 && strMessage.length == 0)
			return MarkInvalidField(oSubjectInput);

		$.post('/ZooZoo/default.aspx', { "action": "contact", "name": strName, "email": strEmail, "subject": strSubject, "message": strMessage }, function (data) {
			if (data != "OK")
				alert(data);
		});

		var oPlaceHolder = $(".contactus_placeholder");
		oPlaceHolder.html("<img src=\"/ZooZoo/Images/contactus_done.gif\" border=\"0\" />");
		oPlaceHolder.css("background-image", "none");
		var oImage = oPlaceHolder.find("img");
		var totalWidth = oPlaceHolder.width();
		var totalHeight = oPlaceHolder.height();
		var imgWidth = oImage.width();
		var imgHeight = oImage.height();
		var imgLeft = parseInt((totalWidth - imgWidth) / 2);
		var imgTop = parseInt((totalHeight - imgHeight) / 2);
		oImage.css({ position: "absolute", left: imgLeft + "px", top: imgTop + "px" });
	});
}

function IsValidEmail(value) {
	// contributed by Scott Gonzalez: http://projects.scottsplayground.com/email_address_validation/
	return value.length == 0 || /^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$/i.test(value);
}

function MarkInvalidField(element) {
	element.focus();
	element.animate({ backgroundColor: "#BE1E2D" }, 1500, function () {
		$(this).animate({ backgroundColor: "#ffffff" }, 1000);
	});
	return false;
}

function ApplyWeeklyGame() {
	CenterInsideParent($(".WeeklyGameImage"));
}

function ApplyWroteOnUs() {
	CenterInsideParent($(".WroteOnUsImage"));
}

function CenterInsideParent(element) {
	if (element.length == 1) {
		element.load(function () {
			var oParent = $(this).parent();
			var totalWidth = parseInt(oParent.css("width"), 10);
			var totalHeight = parseInt(oParent.css("height"), 10);
			var picWidth = $(this).width();
			var picHeight = $(this).height();
			//alert([totalWidth, totalHeight, picWidth, picHeight].join("\n"));
			if (picWidth > totalWidth) {
				$(this).width(totalWidth);
			}
			else if (picHeight > totalHeight) {
				$(this).height(totalHeight);
			}
			picWidth = $(this).width();
			picHeight = $(this).height();
			if (picWidth < totalWidth || picHeight < totalHeight) {
				var left = parseInt((totalWidth - picWidth) / 2);
				var top = parseInt((totalHeight - picHeight) / 2);
				$(this).css({ position: "absolute", left: left + "px", top: top + "px" });
			}
		});
		element.attr("src", element.attr("src"));
	}
}

function HookEventsCode() {
	var oInnerTextBox = $("#ec_inner_text_box");
	$(".ec_inner_text").each(function (index) {
		$(this).css("cursor", "pointer");
		$(this).click(function () {
			var day = parseInt($(this).parent().find(".ec_date").text(), 10);
			if (!isNaN(day)) {
				var arrEvents = arrEventCalendarData[day + ""];
				if (typeof arrEvents != "undefined" && arrEvents && arrEvents.length > 0)
					oInnerTextBox.html("<div class=\"ec_inner_text_box\">" + arrEvents.join("<hr />") + "</div>");
				else
					oInnerTextBox.html("");
			}
		});
		$(this).fancyZoom({ directory: '/Images/fancyzoom', width: EVENT_CALENDAR_DESCRIPTION_WIDTH });
	});
}

function CheckResolution() {
	if ($("body").width() < $("#LogoPlaceHolder").width()) {
		var margin = $("#top_banner").height() - 30;
		$.gritter.add({ title: '&nbsp;', text: '<div align="right" style="direction: rtl; margin-top: ' + margin + 'px;"><div style="text-decoration: underline; font-weight: bold;">שימו לב!</div><div>אתר זה עובד בצורה מיטבית ברזולוציה ' + MIN_RESOLUTION[0] + 'x' + MIN_RESOLUTION[1] + '</div></div>' });
	}
	else {
		//RTL for chrome?
		if (_isChrome)
			document.getElementsByTagName("html")[0].dir = "rtl";
	}
}

function ApplyMaxWidth() {
	var arrImages = document.getElementsByTagName("img");
	for (var i = 0; i < arrImages.length; i++) {
		var oCurImage = arrImages[i];
		var sCurValue = oCurImage.getAttribute("max_width");
		if (typeof sCurValue != "undefined" && sCurValue != null && sCurValue.length > 0) {
			var nMaxWidth = ToIntDef(sCurValue, 0);
			if (nMaxWidth > 0 && oCurImage.width > nMaxWidth) {
				oCurImage.width = nMaxWidth;
			}
		}
	}
}

function InitScrollingElement(sCaption) {
    var oPlaceholder = $("." + sCaption + "Placeholder");
    if (oPlaceholder.length == 1) {
    	var oContents = $("#" + sCaption + "Contents");
    	var oWrapper = $("#" + sCaption + "Wrapper");
		var placeHolderPosition = oPlaceholder.position();
		oContents.css("visibility", "visible");
        //oWrapper.css("left", (placeHolderPosition.left + SCROLLER_LEFT_DIFF) + "px");
		//oWrapper.css("top", (placeHolderPosition.top + SCROLLER_TOP_DIFF_EVENTS) + "px");
		oWrapper.css("left", SCROLLER_LEFT_DIFF + "px");
		oWrapper.css("top", SCROLLER_TOP_DIFF_EVENTS + "px");

        var oLink = $(".hp_all_" + sCaption.toLowerCase());
        var linkPosition = oLink.position();
		var linkLeft = linkPosition.left;
		var linkTop = linkPosition.top;
        oLink.css("left", (placeHolderPosition.left + linkLeft) + "px");
        oLink.css("top", (placeHolderPosition.top + linkTop) + "px");
        oLink.css("height", "35px");

        if (oContents.height() > oWrapper.height())
            InitializeScroller(sCaption + "Contents");
    }
}

function RegisterFlashBanner(containerID, bannerURL) {
	var obj = $("#" + containerID);
	if (obj.length == 1 && typeof SWFObject != "undefined") {
		var bannerWidth = obj.width();
		var bannerHeight = obj.height();
		var oSWF = new SWFObject(bannerURL, "movie_" + containerID, bannerWidth + "", bannerHeight + "", "7", "#ffffff");
		oSWF.write(containerID);
	}
}

function InitializeScroller(sCurID) {
    var oDiv = document.getElementById(sCurID);
    if (oDiv) {
        var arrInnerDivs = new Array();
        for (var j = 0; j < oDiv.childNodes.length; j++) {
            var oChild = oDiv.childNodes[j];
            if (oChild.nodeName.toLowerCase() == "div")
                arrInnerDivs[arrInnerDivs.length] = oChild;
        }
        _scrollInternalData[sCurID] = new Array();
        _scrollInternalData[sCurID]["nodes"] = arrInnerDivs;
        _scrollInternalData[sCurID]["scroll_value"] = 0;
        _scrollInternalData[sCurID]["scroll_offset"] = 0;
        _scrollInternalData[sCurID]["scroll_index"] = 0;
        _scrollInternalData[sCurID]["iterations"] = 0;
        _scrollInternalData[sCurID]["mouse_over"] = false;

        oDiv.onmouseover = function () {
            _scrollInternalData[this.id]["mouse_over"] = true;
        }

        oDiv.onmouseout = function () {
            _scrollInternalData[this.id]["mouse_over"] = false;
        }

        _scrollingElementsCount++;
    }
}

function GlobalScroller() {
	for (var key in _scrollInternalData) {
		if (_scrollInternalData[key]["mouse_over"] == true && PAUSE_WHEN_HOVER)
			continue;

		var oRoot = document.getElementById(key);
		if (_scrollInternalData[key]["iterations"] >= SCROLL_MAX_ITERATIONS) {
			while (oRoot.childNodes.length > 0)
				oRoot.removeChild(oRoot.childNodes[0]);
			for (var i = 0; i < _scrollInternalData[key]["nodes"].length; i++)
				oRoot.appendChild(_scrollInternalData[key]["nodes"][i]);
			_scrollInternalData[key]["iterations"] = 0;
			_scrollInternalData[key]["scroll_value"] = 0;
			_scrollInternalData[key]["scroll_index"] = 0;
			_scrollInternalData[key]["scroll_offset"] = 0;
		}

		var nCurScrollValue = _scrollInternalData[key]["scroll_value"] + SCROLL_PIXELS;
		var nScrollOffset = _scrollInternalData[key]["scroll_offset"] + SCROLL_PIXELS; ;
		var nScrollIndex = _scrollInternalData[key]["scroll_index"];
		var oCurDiv = _scrollInternalData[key]["nodes"][nScrollIndex];
		if (nScrollOffset >= oCurDiv.offsetHeight) {
			oRoot.appendChild(oCurDiv.cloneNode(true));
			nScrollOffset = 0;
			nScrollIndex++;
			if (nScrollIndex >= _scrollInternalData[key]["nodes"].length) {
				_scrollInternalData[key]["iterations"]++;
				nScrollIndex = 0;
			}
			_scrollInternalData[key]["scroll_index"] = nScrollIndex;
		}

		oRoot.style.marginTop = "-" + nCurScrollValue + "px";
		_scrollInternalData[key]["scroll_value"] = nCurScrollValue;
		_scrollInternalData[key]["scroll_offset"] = nScrollOffset;
	}
}

function InitPollElements() {
	$(".PollAnswerContainer").each(function (index) {
		$(this).mouseover(function () {
			if ($(".PollSubmitButton").attr("clicked") == "1")
				return;
			var blnSelected = ($(this).attr("selected") == "1");
			if (!blnSelected)
				$(this).find("[class^=PollAnswerBullet_]").attr("class", "PollAnswerBullet_Hover");
		});

		$(this).mouseout(function () {
			if ($(".PollSubmitButton").attr("clicked") == "1")
				return;
			var blnSelected = ($(this).attr("selected") == "1");
			if (!blnSelected)
				$(this).find("[class^=PollAnswerBullet_]").attr("class", "PollAnswerBullet_Empty");
		});

		$(this).click(function () {
			if ($(".PollSubmitButton").attr("clicked") == "1")
				return;

			$(this).find("[class^=PollAnswerBullet_]").attr("class", "PollAnswerBullet_Filled");
			$(this).attr("selected", "1");
			GetPollHiddenInput().value = (index + "");
			var sender = $(this).get(0);
			$(".PollAnswerContainer").each(function (index) {
				if ($(this).get(0) != sender) {
					$(this).find("[class^=PollAnswerBullet_]").attr("class", "PollAnswerBullet_Empty");
					$(this).attr("selected", "0");
				}
			});
		});
	});

	$(".PollSubmitButton").click(function () {
		if ($(this).attr("clicked") == "1")
			return;

		var selectedAnswerIndex = $("#PollAnswer").val();
		if (typeof selectedAnswerIndex == "undefined" || selectedAnswerIndex == null || selectedAnswerIndex.length == 0) {
			$(".PollAnswerContainer").each(function (index) {
				$(this).find("[class^=PollAnswerBullet_]").attr("class", "PollAnswerBullet_Hover");
			});
			window.setTimeout("ResetPollAnswers();", 500);
			return false;
		}

		//document.forms[0].submit();
		$(this).attr("clicked", "1");
		$(this).css("cursor", "default");
		var answerId = $(".PollAnswerContainer").get(selectedAnswerIndex).getAttribute("answer_id");
		SendPollVote(answerId);
	});
}

function SendPollVote(answerId) {
	$.post('/ZooZoo/default.aspx', { "action": "vote", "answer": answerId }, function (data) {
		var oPlaceHolder = $(".QuickPollPlaceholder");
		oPlaceHolder.css("background-image", "url(/ZooZoo/Images/quickpoll_bg_voted.gif)");
		oPlaceHolder.css("width", "208px");
		oPlaceHolder.css("height", "371px");
		oPlaceHolder.css("margin-top", "0px");

		var oInternalContents = $(".PollInternalContents");
		oInternalContents.html(data);
		oInternalContents.css("height", "260px");
	});
}

function ResetPollAnswers() {
    $(".PollAnswerContainer").each(function (index) {
        $(this).find("[class^=PollAnswerBullet_]").attr("class", "PollAnswerBullet_Empty");
    });
}

function GetPollHiddenInput() {
    var oInput = document.getElementById("PollAnswer");
    if (oInput == null || !oInput) {
        oInput = document.createElement("input");
        oInput.type = "hidden";
        oInput.id = "PollAnswer";
        oInput.name = "PollAnswer";
        document.forms[0].appendChild(oInput);
    }
    return oInput;
}

function GalleryImageClick(oLink) {
	oLink.href = oLink.getElementsByTagName("img")[0].src;
}

function EventCalendarDateChosen() {
	var oMonthDropDown = $(".select_month_ddl");
	if (oMonthDropDown.length > 1)
		oMonthDropDown = $("#zoom_content .select_month_ddl");
	var oYearDropDown = $(".select_year_ddl");
	if (oYearDropDown.length > 1)
		oYearDropDown = $("#zoom_content .select_year_ddl");
	var month = oMonthDropDown.val();
	var year = oYearDropDown.val();
	if (month.length < 2)
		month = "0" + month;
	var oLink = $(".event_calendar_navigate");
	var sHref = oLink.attr("href");
	sHref = sHref.replace("$month", month);
	sHref = sHref.replace("$year", year);
	window.location.href = sHref;
}

function CheckElpasedTime(dtLastExecution, nThreshold) {
	var dtNow = new Date();
	var elpasedSeconds = (dtNow.getTime() - dtLastExecution.getTime()) / 1000;
	return (parseInt(elpasedSeconds) >= nThreshold);
}

function ToIntDef(data, defVal) {
	var num = parseInt(data);
	if (isNaN(num))
		num = defVal;
	return num;
}