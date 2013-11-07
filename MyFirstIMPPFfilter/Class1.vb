Imports Fiddler


'1# Finished yum kipor תשעד 13/09/2013 15:45


Public Class MyFirstFilter : Implements IAutoTamper, IHandleExecAction
    Dim _good As String = "/hello/"
    Dim _bad As String = "/bdbdbdsm/"

    '"http\u00253A\u00252F\u00252Fen.wikipedia.org\u00252Fwiki\u00252FCommon_collector&amp"

    ' TODO: Add Reload Keywords.
    ' TODO: Add/Edit  Keywords list.
    ' TODO: Add Regix blocking. + Custom HTML Response.



    Public Sub AutoTamperRequestAfter(ByVal oSession As Fiddler.Session) Implements Fiddler.IAutoTamper.AutoTamperRequestAfter
        ' After i sent the request. (The original one)
    End Sub



    Public Sub AutoTamperRequestBefore(ByVal oSession As Fiddler.Session) Implements Fiddler.IAutoTamper.AutoTamperRequestBefore

        If oSession.PathAndQuery.Contains("imppf?showlist") Then

            Dim _str_beg As String = "<html><head><title>List</title></head><body>"
            _str_beg &= "<table border=1 width=500><tr> <td width=5%>Id</td> <td width=5%>Filtered?</td>  <td width=90%>Host</td> </tr>"

            For Each item As Session In FiddlerApplication.UI.GetAllSessions
                _str_beg &= "<tr><td><a href=http://127.0.0.1/imppf?item?" & item.id & ">" & item.id & "</a></td> <td> " & item("ui-comments") & "</td>  <td>" & _
                  Mid(item.host & item.PathAndQuery, 1, 100) & "</td></tr>"
            Next

            _str_beg &= "</body></html>"

            oSession.oRequest.FailSession(200, "OK", _str_beg)

        ElseIf oSession.PathAndQuery.Contains("imppf?item?") Then
            Dim _id As Integer = CInt(oSession.PathAndQuery.Split("?").Last)
            Dim _session As Session = Nothing

            For Each item As Session In FiddlerApplication.UI.GetAllSessions
                If item.id = _id Then
                    _session = item
                End If
            Next

            Dim _str_beg As String = "<html><head><title>Item</title></head><body>" & Now.ToString & "<br>" & _
                "<a href=http://127.0.0.1/imppf?showlist><- Back</a><br>"

            If _session IsNot Nothing Then


                _str_beg &= "Showing Item: " & _id & "<br>"
                _str_beg &= "Full Url: " & _session.host & _session.PathAndQuery & "<br>"

                _str_beg &= "<u>Raw Request:</u> <br>"
                _str_beg &= _session.GetRequestBodyAsString
                _str_beg &= "<br><u>Raw Response:</u> <br>"
                _str_beg &= _session.GetResponseBodyAsString.Replace("<", "&lt;").Replace(">", "&gt;")



            Else
                _str_beg &= "Session is no longer available"
            End If

            _str_beg &= "</body></html>"
            oSession.oRequest.FailSession(200, "OK", _str_beg)

        ElseIf oSession.PathAndQuery.Contains(_bad) Then
            oSession.PathAndQuery = oSession.PathAndQuery.Replace(_bad, _good)
            oSession("ui-comments") = "Filtered"
        End If

    End Sub

    Public Sub AutoTamperResponseAfter(ByVal oSession As Fiddler.Session) Implements Fiddler.IAutoTamper.AutoTamperResponseAfter
        'After i gave back the response to the user
    End Sub

    Public Sub AutoTamperResponseBefore(ByVal oSession As Fiddler.Session) Implements Fiddler.IAutoTamper.AutoTamperResponseBefore
        oSession.utilDecodeResponse()

        'Lemida Download
        oSession.utilSetResponseBody(oSession.GetResponseBodyAsString.Replace("mod/resource/view.php?", "mod/resource/view.php?redirect=1&"))

       

        If oSession.utilFindInResponse(_bad, False) >= 0 Then

            oSession.utilSetResponseBody("<html><body> Fiddler Bef </body></html>")
            oSession("ui-comments") = "Filtered"
        End If
    End Sub

    Public Sub OnBeforeReturningError(ByVal oSession As Fiddler.Session) Implements Fiddler.IAutoTamper.OnBeforeReturningError

    End Sub

    Public Sub OnBeforeUnload() Implements Fiddler.IFiddlerExtension.OnBeforeUnload

    End Sub

    Dim _app_path As String = ""

    Dim _temp_url As String = ""
    Dim _perm_url As String = ""
    Dim _temp_content As String = ""
    Dim _perm_content As String = ""

    Public Sub OnLoad() Implements Fiddler.IFiddlerExtension.OnLoad
        _app_path = FileIO.SpecialDirectories.MyDocuments & "\Fiddler2\Scripts\"

        _temp_url = LoadStringFromFile(_app_path & "temp_url.txt")
        _perm_url = LoadStringFromFile(_app_path & "perm_url.txt")
        _temp_content = LoadStringFromFile(_app_path & "temp_content.txt")
        _perm_content = LoadStringFromFile(_app_path & "perm_content.txt")
    End Sub

    Function LoadStringFromFile(ByVal Path As String) As String
        If FileIO.FileSystem.FileExists(Path) Then
            Return New IO.StreamReader(Path).ReadToEnd
        Else
            IO.File.Create(Path).Dispose()
            Return ""
        End If
    End Function

    Public Function OnExecAction(ByVal sCommand As String) As Boolean Implements Fiddler.IHandleExecAction.OnExecAction

        Return False 'Didnt handle
    End Function
End Class
