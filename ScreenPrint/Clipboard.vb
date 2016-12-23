Public Class Clipboard

    Private Sub Clipboard_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True

        Me.Hide()
    End Sub

    Private Sub Clipboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub ListView1_DoubleClick(sender As Object, e As EventArgs) Handles ListView1.DoubleClick
        ImageViewer.Show()
        ImageViewer.PictureBox1.Image = ListView1.SelectedItems(0).Tag

    End Sub

    Private Sub ListView1_MouseUp(sender As Object, e As MouseEventArgs) Handles ListView1.MouseUp
        If e.Button = MouseButtons.Right Then
            If ListView1.GetItemAt(e.X, e.Y + 59) IsNot Nothing Then
                ListView1.GetItemAt(e.X, e.Y + 59).Selected = True
                ContextMenuStrip1.Show(Me, New Point(e.X, e.Y + 59))
            End If
        End If
    End Sub

    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged

    End Sub

    Private Sub DeleteItemToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteItemToolStripMenuItem.Click
        For i As Integer = ListView1.Items.Count - 1 To 0 Step -1
            If ListView1.Items(i).Selected Then ListView1.Items.RemoveAt(i)
        Next
    End Sub

    Private Sub CopyToClipboardToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles CopyToClipboardToolStripMenuItem.Click
        My.Computer.Clipboard.SetImage(ListView1.SelectedItems(0).Tag)
    End Sub

    Private Sub SaveToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SaveToolStripMenuItem.Click
        Try
            Dim bmp = ListView1.SelectedItems(0).Tag

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
End Class