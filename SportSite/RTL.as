/*******************
* creator@ Ben Fhala
* for@ Flashoo and Flashers out there ;)
* site@ http://anxpl.com/home/ltr/
* mail@ ben@anxpl.com 
* cell@ +1.347.409.4499 (USA)
* ver: 0.300.3
* creation date: 01/17/2006
* Mod Date: 02/12/2007
*********************/


class RTL{
	static private var _firstRTLLetter:String="א";
	static private var _lastRTLLetter:String="ת";
	static private var _ifFirst:String="([{<-\"''\"->}])";
	static private var _withBars:Boolean=false;
	static private var _addvanced:Boolean=false;
	
	static public function setLTR(firstRTLLetter:String,lastRTLLetter:String):Void{
		_firstRTLLetter=firstRTLLetter;
		_lastRTLLetter=lastRTLLetter;
	}
	static public function enableBars(bState:Boolean):Boolean{
		if(bState==null) bState=false;
		_withBars=bState;
		if(_addvanced) _addvanced=_withBars;
		return _withBars;
	}
	static public function enableAdvanced(bState:Boolean):Boolean{
		if(bState==null) bState=false;
		_addvanced=bState;
		_withBars=bState;
		return _addvanced;
	}
	static public function prepSingleLine(sLine:String):String{
		var aLine:Array=sLine.split(" ");
		for(var i:String in aLine) aLine[i]=invert(aLine[i]);
		trace(groupArrayMemebers(aLine,0));
		aLine.reverse();
		return aLine.join(" ");
	}
	
	static public function prepMultiLine(sLine:String,nLetters:Number):String{
		var aLines:Array=sLine.split(String.fromCharCode(Key.ENTER));
		for(var i:Number=0;i<aLines.length;i++)
			aLines[i]=prepLongLine(aLines[i],nLetters);
			
		return aLines.join("\n");
	}
	//private static methods that make things tick...
	static private function invert(word:String):String{
		if(isRTL(word)){
			var aWord:Array=word.split("");
			aWord.reverse()
			word=aWord.join("");
			if(_withBars) word=changeChar(word);
		}
		return word;
	}
	
	static private function changeChar(word:String):String{
		var aWord:Array;
		if(_addvanced){
			trace("");
			
			aWord=word.split("");
			//bars from all types and 2 langs in one word
			var startPoint:Number=-1;
			var endPoint:Number=-1;
			for(var i:Number=0;i<aWord.length;i++){
				
				var index:Number=_ifFirst.indexOf(aWord[i]);
				if(index!=-1){
					aWord[i]=_ifFirst.charAt(_ifFirst.length-index-1);
				}else if(!(isRTL(aWord[i]))){
					trace(":<<<<<:::"+startPoint);
					if(startPoint>-1){
						endPoint=i;
					}else{
						startPoint=i+1;
					}
				}
			}
			word=aWord.join("");
			// 2 langs in one word
			
			if(startPoint>-1){
				aWord=word.slice(startPoint-1,endPoint+1).split("");
				aWord.reverse();
				word= word.slice(0,startPoint-1)+aWord.join("")+word.slice(endPoint+1);
			}
		}else{
			aWord=word.split("");
			var indexStart:Number=_ifFirst.indexOf(aWord[0]);
			var indexEnd:Number=_ifFirst.indexOf(aWord[aWord.length-1]);
			if(indexStart!=-1)
				aWord[0]=_ifFirst.charAt(_ifFirst.length-indexStart-1);
			if(indexEnd!=-1)
				aWord[aWord.length-1]=_ifFirst.charAt(_ifFirst.length-indexEnd-1);
			word=aWord.join("");
		}
		
		
		return word;
	}
	static private function isRTL(word:String):Boolean{
		var sLetter:String=word.split("")[0];
		if(_ifFirst.indexOf(sLetter)!=-1) sLetter=word.split("")[1];

		if(sLetter>=_firstRTLLetter && sLetter<=_lastRTLLetter){
			return true;
		}
		
		return false;
	}
	static private function groupArrayMemebers(aGroup:Array,nStart:Number):Boolean{
		var aGroup:Array;
		if(!aGroup[nStart+1]){
			return true;
		}else{
			if(isRTL(aGroup[nStart]) && isRTL(aGroup[nStart+1])){
				var sWord:String=aGroup.splice(nStart,1)[0];
				aGroup[nStart]+=" "+sWord;
				//trace(aGroup[nStart]);
				groupArrayMemebers(aGroup,nStart);
				
			}else if(!(isRTL(aGroup[nStart])) && !(isRTL(aGroup[nStart+1]))){
				var sWord:String=aGroup.splice(nStart,1)[0];
				aGroup[nStart]=sWord+" "+aGroup[nStart];
				groupArrayMemebers(aGroup,nStart);
				
			}else{
				groupArrayMemebers(aGroup,++nStart);
			}
		}
	}

	
	static private function prepLongLine(sSubLine:String,nLetters:Number):String{
		if(sSubLine.length<=nLetters){
			sSubLine=prepSingleLine(sSubLine);
		}else{

			var nSubStart:Number=0;
			var nSubEnd:Number;
			var aMultiLine:Array=new Array();
			
			while(sSubLine.length>(nSubStart+nLetters)){
				nSubEnd=sSubLine.lastIndexOf(" ",nSubStart+nLetters);
				if(nSubEnd==-1) break;
				//aMultiLine.push(sSubLine.substr(nSubStart,nSubEnd));
				aMultiLine.push(sSubLine.substr(nSubStart, (nSubEnd-nSubStart))); //aniche from Flashoo
				nSubStart=nSubEnd;
			}
			
			aMultiLine.push(sSubLine.substr(nSubStart));
			for(var i:Number=0; i<aMultiLine.length;i++){
				aMultiLine[i]=prepSingleLine(aMultiLine[i]);
			}
			sSubLine=aMultiLine.join("\n");
		}
		
		return sSubLine;
	}
}