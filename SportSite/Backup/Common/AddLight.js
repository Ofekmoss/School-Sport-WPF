var arrColoredControls=new Array();
function PutMoreLight(objControl, color, lightAmount) {
	if ((objControl.attributes["add_light"])&&(objControl.attributes["add_light"].value == "0"))
		return true;
	arrColoredControls[objControl] = color;
	var R=HexToInt(color.substring(1, 3));
	var G=HexToInt(color.substring(3, 5));
	var B=HexToInt(color.substring(5, 7));
	R = SafeAdd(R, lightAmount, 0, 255);
	G = SafeAdd(G, lightAmount, 0, 255);
	B = SafeAdd(B, lightAmount, 0, 255);
	var lightedColor=BuildColor(R, G, B);
	objControl.style.backgroundColor = lightedColor;
}

function RestoreColor(objControl) {
	if ((objControl.attributes["add_light"])&&(objControl.attributes["add_light"].value == "0"))
		return true;
	objControl.style.backgroundColor = arrColoredControls[objControl];
}

function UpdateControlColor(objControl, color) {
	arrColoredControls[objControl] = color;
}

function IntToHex(num) {
	if (num < 10)
		return (num+"");
	switch (num) {
		case 10: return "a";
		case 11: return "b";
		case 12: return "c";
		case 13: return "d";
		case 14: return "e";
		case 15: return "f";								
	}
	return IntToHex(parseInt(num/16))+IntToHex(parseInt(num%16));
}

function HexToInt(strHex) {
	return parseInt(strHex, 16);
}

function SafeAdd(num, addition, min, max) {
	num += addition;
	if (num > max)
		num = max;
	if (num < min)
		num = min;
	return num;
}

function BuildColor(r, g, b) {
	var R=IntToHex(r);
	var G=IntToHex(g);
	var B=IntToHex(b);
	R=(R.length == 1)?("0"+R):R;
	G=(G.length == 1)?("0"+G):G;
	B=(B.length == 1)?("0"+B):B;
	return "#"+R+G+B;
}