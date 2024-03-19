var CharLimit = {
	MessageTemplate: '<span class="chars-used">$chars_used</span>/$max_chars תווים',
	Apply: function (inputElement) {
		var charLimit = parseInt(inputElement.data("char-limit"));
		if (isNaN(charLimit))
			charLimit = parseInt(inputElement.attr("maxlength"));
		if (!isNaN(charLimit) && charLimit > 0) {
			var remainingCharsDiv = $("<div></div>");
			var currentValue = $.trim(inputElement.val());
			remainingCharsDiv.html(CharLimit.MessageTemplate.replace("$max_chars", charLimit).replace("$chars_used", currentValue.length));
			inputElement.after(remainingCharsDiv);
			inputElement.data("previous-value", currentValue);
			inputElement.bind("keyup paste change", function () {
				currentValue = $.trim(inputElement.val());
				var previousValue = inputElement.data("previous-value");
				if (currentValue != previousValue) {
					var currentLength = currentValue.length;
					remainingCharsDiv.removeClass("text-danger");
					remainingCharsDiv.find(".chars-used").html(currentLength.toString());
					if (currentLength >= charLimit) {
						remainingCharsDiv.addClass("text-danger");
						if (currentLength > charLimit) {
							currentValue = currentValue.substr(0, charLimit);
							remainingCharsDiv.find(".chars-used").html(charLimit);
							inputElement.val(currentValue);
						}
					}
					inputElement.data("previous-value", currentValue);
				}
			});
		}
	}
};

$(document).ready(function () {
	$(".show-remaining-chars").each(function () {
		CharLimit.Apply($(this));
	});
});