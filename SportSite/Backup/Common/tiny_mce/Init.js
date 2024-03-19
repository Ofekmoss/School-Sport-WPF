//	theme_advanced_buttons1: "save,newdocument,|,bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,styleselect,formatselect,fontselect,fontsizeselect",
//	theme_advanced_buttons2: "cut,copy,paste,pastetext,pasteword,|,search,replace,|,bullist,numlist,|,outdent,indent,blockquote,|,undo,redo,|,link,unlink,anchor,image,cleanup,help,code,|,insertdate,inserttime,preview,|,forecolor,backcolor",
//	theme_advanced_buttons3: "tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,charmap,emotions,iespell,media,advhr,|,print,|,ltr,rtl,|,fullscreen",
//	theme_advanced_buttons4: "insertlayer,moveforward,movebackward,absolute,|,styleprops,spellchecker,|,cite,abbr,acronym,del,ins,attribs,|,visualchars,nonbreaking,template,blockquote,pagebreak,|,insertfile,insertimage",
tinyMCE.init({
	// General options
	mode: "textareas",
	theme: "advanced",
	plugins: "style,layer,table,advhr,advimage,advlink,inlinepopups,insertdatetime,preview,media,searchreplace,paste,directionality,nonbreaking,imagemanager",
	width: "480",

	// Theme options
	theme_advanced_buttons1: "forecolor,backcolor,|,bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,cut,copy,paste,pastetext,pasteword,|,search,replace,",
	theme_advanced_buttons2: "styleselect,formatselect,fontselect,fontsizeselect,|,bullist,numlist,|,outdent,indent,",
	theme_advanced_buttons3: "link,unlink,anchor,image,cleanup,code,|,insertdate,inserttime,preview,|,hr,removeformat,|,sub,sup,|,charmap,emotions,iespell,media,advhr,|,ltr,rtl,",
	theme_advanced_buttons4: "",
	theme_advanced_toolbar_location: "top",
	theme_advanced_toolbar_align: "right",
	theme_advanced_statusbar_location: "bottom",
	theme_advanced_resizing: true,

	// Example content CSS (should be your site CSS)
	content_css: "",

	// Drop lists for link/image/media/template dialogs
	template_external_list_url: "",
	external_link_list_url: "",
	external_image_list_url: "",
	media_external_list_url: "",

	// Replace values for the template plugin
	template_replace_values: {
		username: "Some User",
		staffid: "991234"
	}
});