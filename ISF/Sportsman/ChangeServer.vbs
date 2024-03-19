'-------------------------------
'ChangeServer.vbs
'Changes the server definition in config file.
'Last Modified: 27/02/2005
'-------------------------------

'''D:\ISF\Bin\Debug

Option Explicit

Const CONFIG_FILE_DEFAULT="Sportsman.exe.config"

Dim strConfigPath, objFSO, objFile
Dim strAllFile, arrLines, strCurLine
Dim x, arrTmp, arrTmp2
Dim strTemp, strValue, strServerAddress
Dim strNewValue, iLinesChanged

' input cofiguration file name from user:
strConfigPath = InputBox("Configuration file path: ", "Sportsman - Change Server", _
					MapPath(CONFIG_FILE_DEFAULT))

' verify not empty:
If Len(strConfigPath)=0 Then
	'no file name given, abort:
	MsgBox "No file name given, aborting program.", vbOKOnly, "error" : WScript.Quit
End If

' initialize file system object:
Set objFSO=CreateObject("Scripting.FileSystemObject")

' verify file exists:
If Not(objFSO.FileExists(strConfigPath)) Then
	'file does not exist, abort:
	MsgBox "file "&strConfigPath&" does not exist, aborting program", vbOKOnly, "error"
	Set objFSO = Nothing : WScript.Quit
End If

' input server address:
strServerAddress = InputBox("New Server Address: ", "Sportsman - Change Server", "www.amitbb.co.il")

' verify file exists:
If Len(strServerAddress)=0 Then
	'no new address...
	MsgBox "no sever address, aborting program", vbOKOnly, "error"
	Set objFSO = Nothing : WScript.Quit
End If

' getting here means the file is valid, open it:
Set objFile=objFSO.OpenTextFile(strConfigPath, 1) 'ForReading

' Read file contents into local variable:
strAllFile = objFile.ReadAll
objFile.Close

' Split into lines:
arrLines = Split(strAllFile, VBCrLf)

' iterate over the lines, look for lines to replace:
iLinesChanged = 0
For x=0 To UBound(arrLines)
	' get current line:
	strCurLine = arrLines(x)
	
	' check for url value:
	If InStr(1, strCurLine, "http://", 1)>0 Then
		' found candiate, try to get value:
		arrTmp = Split(strCurLine, "value=")
		
		'verify we have single value:
		If UBound(arrTmp)=1 Then
			'single value, get it:
			strTemp=arrTmp(1)
			
			'remove quotes:
			strValue=Mid(strTemp, 2, InStr(2, strTemp, Chr(34))-2)
			
			' split into parts:
			arrTmp2 = Split(strValue, "/")
			
			' the server address would be the third item:
			If UBound(arrTmp2)>=2 Then
				'replace the server address with the given address:
				arrTmp2(2) = strServerAddress
				
				'define new value:
				strNewValue = Join(arrTmp2, "/")
				
				'update current line:
				strCurLine = Replace(strCurLine, strValue, strNewValue)
				
				'update lines array:
				arrLines(x) = strCurLine
				
				'update counter:
				iLinesChanged = iLinesChanged+1
			End If
			
			'free used memory:
			Erase arrTmp2
		End If
		
		' free used memory:
		Erase arrTmp		
	End If
Next

' check if any lines were updated:
If iLinesChanged>0 Then
	' Overwrite the file:
	Set objFile=objFSO.CreateTextFile(strConfigPath, True)
	
	' write new contents:
	objFile.Write(Join(arrLines, VBCrLf))
	
	' all done.
	objFile.Close
	MsgBox "Done, "&iLinesChanged&" lines were updated.", vbOKOnly, "message"
Else  
	MsgBox "no lines were updated.", vbOKOnly, "message"
End If

Erase arrLines
Set objFile=Nothing
Set objFSO=Nothing

Function MapPath(strVirtualPath)
	Dim result, strRootFolderPath
	strRootFolderPath = ExtractFolder(WScript.ScriptFullName)
	result = strRootFolderPath&"\"
	result = result&Replace(strVirtualPath, "/", "\")
	MapPath=result
End Function

Function ExtractFolder(strPath)
	ExtractFolder=Left(strPath, InStrRev(strPath, "\")-1)
End Function