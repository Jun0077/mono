'Author:
'   Satya Sudha K (ksathyasudha@novell.com)
'
' (C) 2005 Novell, Inc.

Imports System

Module M
	Function F (a As Long, b As Integer, c As Long) As Integer
		return 1
	End Function
	Function F (a As String, b as Date, c As Integer) As Integer
		return 2
	End Function
	Sub Main ()
		Dim obj As Object = "123"
		if F (obj,1,2) <> 1 Then
			throw new Exception ("Overload Resolution failed in latebinding")
		End If
	End Sub
End Module
