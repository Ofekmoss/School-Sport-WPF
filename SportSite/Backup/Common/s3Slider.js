/* ------------------------------------------------------------------------
	s3Slider
	
	Developped By: Boban Karišik -> http://www.serie3.info/
        CSS Help: Mészáros Róbert -> http://www.perspectived.com/
	Version: 1.0
	
	Copyright: Feel free to redistribute the script/modify it, as
			   long as you leave my infos at the top.
------------------------------------------------------------------------- */
(function($){  
    $.fn.s3Slider = function(vars) {       
	var element     = this;
	var timeOut     = (vars.timeOut != undefined) ? vars.timeOut : 4000;
	var current     = null;
	var timeOutFn   = null;
	var faderStat   = true;
	var items       = $("." + element[0].id + "Image");
	
	var fadeElement = function(isMouseOut) {
		var thisTimeOut = (isMouseOut) ? (timeOut/2) : timeOut;
		thisTimeOut = (faderStat) ? 10 : thisTimeOut;
		if(items.length > 0) {
			timeOutFn = setTimeout(makeSlider, thisTimeOut);
		} else {
			console.log("Poof..");
		}
	}
	
	var makeSlider = function() {
		current = (current != null) ? current : items[(items.length-1)];
		var currNo = jQuery.inArray(current, items) + 1;
		currNo = (currNo == items.length) ? 0 : (currNo - 1);
		var newMargin   = $(element).width() * currNo;
		if(faderStat == true) {
			$(items[currNo]).fadeIn((timeOut/6), function() {
				faderStat = false;
				current = items[currNo];
				fadeElement(false);
			});
		} else {
			$(items[currNo]).fadeOut((timeOut/6), function() {
				faderStat = true;
				current = items[(currNo+1)];
				fadeElement(false);
			});
		}
	};
        
        makeSlider();

    };  

})(jQuery);  