<% Option Explicit %>
<% Response.Buffer = True %>
<%
Const FILE_PATH="C:\Inetpub\wwwroot\Web.config"
Dim objFSo, objFile, strData
Set objFSO = Server.CreateObject("Scripting.FileSystemObject")
Response.Write("File: " & FILE_PATH & ", Exist? " & objFSO.FileExists(FILE_PATH) & "<br />")
Response.Flush()
Set objFile = objFSO.OpenTextFile(FILE_PATH)
strData = objFile.ReadAll
objFile.Close
Response.Write("Got " & Len(strData) & " bytes of data<br />")
Response.Flush()
Set objFile = objFSO.CreateTextFile(FILE_PATH)
objFile.Write(strData)
objFile.Close
Response.Write("Success")
Response.Flush()
Set objFile = Nothing
Set objFSO = Nothing
'Server.CreateObject("WSCript.Shell").Run "cmd.exe /c iisreset /reboot"
%>