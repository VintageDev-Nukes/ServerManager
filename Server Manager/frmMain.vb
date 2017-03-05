Imports System.Text
Public Class frmMain

    Dim configFile = IO.Path.Combine(Application.StartupPath, "server_conf.ini"), SvIP, MyIP As String
    Dim SvState As ServerState
    Dim svProcess As Process

    Private Shadows Sub Load() Handles MyBase.Load
        INIFileManager.FilePath = configFile
        SvIP = INIFileManager.Key.Get("server-ip")
        SvState = [Enum].Parse(GetType(ServerState), INIFileManager.Key.Get("server-state"))
        MyIP = ServerManager.GetMyIP()
    End Sub

    Private Sub CallServer()

        svProcess = New Process()

        Try
            svProcess.StartInfo.FileName = "start.bat"
            svProcess.StartInfo.UseShellExecute = True
            'svProcess.StartInfo.RedirectStandardOutput = True
            'svProcess.StartInfo.RedirectStandardError = True
            svProcess.Start()
            svProcess.WaitForExit()
            SvState = ServerState.Closed
            INIFileManager.Key.Set("server-state", SvState.ToString())
            'IO.File.WriteAllText("finalLog.txt", svProcess.StandardOutput.ReadToEnd())
            svProcess.Close()
            If svProcess IsNot Nothing Then
                svProcess = Nothing
            End If
        Catch ex As Exception
            MsgBox((ex.Message))
        End Try

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        If SvIP = MyIP Then
            SvState = DirectCast([Enum].Parse(GetType(ServerState), INIFileManager.Key.Get("server-state")), ServerState)
            If SvState = ServerState.Opened Then
                If svProcess Is Nothing Then
                    CallServer()
                End If
            End If
        End If
    End Sub

    Private Sub btnServer_Click(sender As Object, e As EventArgs) Handles btnServer.Click
        SvState = ServerState.Opened
        INIFileManager.Key.Set("server-state", SvState.ToString())
    End Sub

End Class
