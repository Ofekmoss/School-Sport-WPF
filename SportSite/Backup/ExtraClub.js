var ValueKeeperAction = {
	Read: 0, Write: 1
};

if (typeof $ !== "undefined") {
	$(document).ready(function () {
		if (window.localStorage) {
			ApplyTableValueKeepers();
			var counter = HandleValueKeeper(ValueKeeperAction.Read);
			if (counter > 0) {
				var oResetLink = $("<a></a>").attr("href", "#").html("נקה ערכים שמורים").bind("click", function () {
					var arrValueKeeperInputs = $("input").filter(function () {
						return ($(this).data("valuekeeper") || "").length > 0;
					});
					arrValueKeeperInputs.each(function () {
						$(this).val("");
					});
					HandleValueKeeper(ValueKeeperAction.Write);
					$(this).hide();
					return false;
				});
				$(".PageContentsPanel").append("<br>").append(oResetLink);
			}

			$(document).click(function (evt) {
				var sender = evt.srcElement || evt.target;
				if (($(sender).attr("src") || "").indexOf("btn_print.jpg") >= 0 || sender.type === 'image')
					HandleValueKeeper(ValueKeeperAction.Write);
			});
		}
	});
}

function ApplyTableValueKeepers() {
	$("table[data-valuekeeper]").each(function () {
		var oTable = $(this);
		var cellCounter = 0;
		var currentCaption = oTable.data("valuekeeper") || "";
		if (currentCaption.length > 0) {
			oTable.find("input").each(function () {
				var currentInput = $(this);
				var currentId = currentInput.data("valuekeeper") || "";
				if (currentId.length == 0) {
					currentId = currentCaption + "_cell_" + cellCounter;
					currentInput.data("valuekeeper", currentId);
					cellCounter++;
				}
			});
		}
	});
}

function HandleValueKeeper(action) {
	var currentPage = document.location.href;
	var index = currentPage.lastIndexOf("/") + 1;
	currentPage = currentPage.substr(index);
	index = currentPage.indexOf("?");
	if (index > 0)
		currentPage = currentPage.substr(0, index);
	var keyTemplate = "valuekeeper_" + currentPage + "_" + _loggedUserSchoolId + "_$id";
	var storedValuesCounter = 0;
	var arrValueKeeperInputs = $("input").filter(function () {
		return ($(this).data("valuekeeper") || "").length > 0;
	});
	arrValueKeeperInputs.each(function () {
		var currentInput = $(this);
		if (currentInput.val().length == 0 || action == ValueKeeperAction.Write) {
			var currentId = currentInput.data("valuekeeper") || "";
			var currentStoredValue = "";
			if (currentId.length > 0) {
				var currentKey = keyTemplate.replace("$id", currentId);
				switch (action) {
					case ValueKeeperAction.Read:
						currentStoredValue = localStorage[currentKey] || "";
						currentInput.val(currentStoredValue);
						if (currentStoredValue.length > 0)
							storedValuesCounter++;
						break;
					case ValueKeeperAction.Write:
						localStorage[currentKey] = currentInput.val();
						break;
				}
			}
		}
	});

	return storedValuesCounter;
}