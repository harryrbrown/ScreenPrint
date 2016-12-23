Public Class ImageViewer

    Private Sub CopyToClipboardToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyToClipboardToolStripMenuItem.Click
        My.Computer.Clipboard.SetImage(PictureBox1.Image)
    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        Try
            Dim bmp = PictureBox1.Image

            Dim month As String = Date.Today.Month.ToString
            If Len(month) = 1 Then
                month = "0" + month
            End If
            Dim day As String = Date.Today.Day.ToString
            If Len(day) = 1 Then
                day = "0" + day
            End If

            Dim Format
            Select Case My.Settings.Format
                Case ".png"
                    Format = System.Drawing.Imaging.ImageFormat.Png
                Case ".jpg"
                    Format = System.Drawing.Imaging.ImageFormat.Jpeg
                Case ".bmp"
                    Format = System.Drawing.Imaging.ImageFormat.Bmp
                Case ".gif"
                    Format = System.Drawing.Imaging.ImageFormat.Gif
            End Select

            Dim FileName As String = ("Screenshot_" + Date.Today.Year.ToString + month + day + "_" + DateTime.Now.ToString("HHmmssff"))

            If System.IO.Directory.Exists(My.Settings.SaveDir) Then
                Try
                    bmp.Save(My.Settings.SaveDir + FileName + My.Settings.Format, Format)
                Catch
                    MsgBox("Unable to save screenshot. Is the file path valid?", MsgBoxStyle.Critical, "Error")
                End Try
            Else
                Try
                    System.IO.Directory.CreateDirectory(My.Settings.SaveDir)
                    bmp.Save(My.Settings.SaveDir + FileName + My.Settings.Format, Format)
                Catch
                    MsgBox("Unable to save screenshot. Is the file path valid?", MsgBoxStyle.Critical, "Error")
                End Try
            End If
            Me.Close()

        Catch ex As Exception
            MsgBox("Unable to save captured region.", MsgBoxStyle.Critical, "Error")
        End Try

    End Sub

    Private Sub ImageViewer_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Z Then
            ImageEditorControls.DesktopLocation = MousePosition
            ImageEditorControls.Show()
            ImageEditorControls.BringToFront()
            Me.Focus()
        End If

    End Sub

    Private Sub ImageViewer_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = Keys.Z Then
            ImageEditorControls.Close()
        End If
    End Sub

    Private Sub ImageViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class