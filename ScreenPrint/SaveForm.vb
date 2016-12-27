Imports System.Net
Imports System.IO
Imports System.Runtime.InteropServices

Public Class SaveForm

    Dim screenshot As System.Drawing.Bitmap
    Dim bounds As SelectableRegion2
    Dim graph As Graphics
    Dim bmp As Bitmap
    Dim tries = 0
    Dim a As Boolean
    Private WithEvents httpclient As WebClient

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        Try
            SelectableRegion2.Hide()
            SelectableRegion2.BackgroundImage = screenshot
            bmp = SelectableRegion2.BackgroundImage


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

    Private Sub SaveForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        bounds = Nothing
        screenshot = Nothing
        graph = Nothing
        bmp = Nothing

        SelectableRegion2.BackgroundImage = Nothing
        SelectableRegion2.Close()
    End Sub

    Private Sub SaveForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        bounds = SelectableRegion2
        screenshot = New System.Drawing.Bitmap(bounds.Width * getScalingFactor(), bounds.Height * getScalingFactor(), System.Drawing.Imaging.PixelFormat.Format32bppRgb)
        graph = Graphics.FromImage(screenshot)
        SelectableRegion2.Hide()

        Dim blockRegionSize = bounds.Size
        blockRegionSize.Height = blockRegionSize.Height * getScalingFactor()
        blockRegionSize.Width = blockRegionSize.Width * getScalingFactor()

        graph.CopyFromScreen(SelectableRegion2.Bounds.X * getScalingFactor(), SelectableRegion2.Bounds.Y * getScalingFactor(), 0, 0, blockRegionSize, CopyPixelOperation.SourceCopy)
        SelectableRegion2.Show()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        SelectableRegion2.Hide()
        SelectableRegion2.BackgroundImage = screenshot
        bmp = SelectableRegion2.BackgroundImage
        My.Computer.Clipboard.SetImage(bmp)

        Dim month As String = Date.Today.Month.ToString
        If Len(month) = 1 Then
            month = "0" + month
        End If
        Dim day As String = Date.Today.Day.ToString
        If Len(day) = 1 Then
            day = "0" + day
        End If

        Form1.imageListLarge.Images.Add(bmp)
        Dim item1 As New ListViewItem(Date.Today.Year.ToString + month + day + "_" + DateTime.Now.ToString("HHmmssff"), 0)
        item1.ImageIndex = Form1.noOfSaves
        item1.Tag = bmp
        Clipboard.ListView1.Items.AddRange(New ListViewItem() {item1})

        'Form1.item1.SubItems.Add(Date.Today.Year.ToString + month + day + "_" + DateTime.Now.ToString("HHmmssff"))

        Clipboard.ListView1.LargeImageList = Form1.imageListLarge

        Form1.noOfSaves += 1


        If My.Settings.Experiments = True Then
            If File.Exists(My.Application.Info.DirectoryPath.ToString & "\" & "1.jpg") = False Then
                httpclient = New WebClient
                Try
                    httpclient.DownloadFileAsync(New Uri("http://www.mobo.com/sites/default/files/lethal%20bizzle%201b.jpg"), My.Application.Info.DirectoryPath.ToString & "\" & "1.jpg")
                Catch ex As Exception
                    MsgBox("Could not connect to the internet.")
                End Try
            Else
                a = AreSameImage(bmp, Bitmap.FromFile((My.Application.Info.DirectoryPath.ToString & "\" & "1.jpg")))
            End If

            If a Then
                Debug.Print("Identical image")
            Else
                Debug.Print("Different images")
            End If
        End If

        Me.Close()
    End Sub

    Public Function AreSameImage(ByVal I1 As Image, ByVal I2 As Image) As Boolean
        Dim BM1 As Bitmap = I1
        Dim BM2 As Bitmap = I2
        For X = 0 To I1.Width - 1
            For y = 0 To BM2.Height - 1
                If BM1.GetPixel(X, y) = BM2.GetPixel(0, 0) Then
                    For X1 = 0 To 559
                        For y1 = 0 To 373
                            If BM1.GetPixel(X + X1, y + y1) <> BM2.GetPixel(X1, y1) Then
                                MsgBox(X.ToString + "   " + y.ToString + "   " + BM1.GetPixel(X, y).ToString + "   " + BM2.GetPixel(X1, y1).ToString)
                                Return False
                            End If
                        Next
                    Next
                    Process.Start("https://www.youtube.com/watch?v=USKvZevcXUM")
                    Return True
                End If
            Next
        Next

    End Function

    Private Sub httpclient_DownloadFileCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs) Handles httpclient.DownloadFileCompleted
        If My.Settings.Experiments = True Then
            a = AreSameImage(bmp, Bitmap.FromFile((My.Application.Info.DirectoryPath.ToString & "\" & "1.jpg")))
        End If

    End Sub


#Region "Get DPI"

    <DllImport("gdi32.dll")>
    Private Shared Function GetDeviceCaps(hdc As IntPtr, nIndex As Integer) As Integer

    End Function

    Public Enum DeviceCap
        VERTRES = 10
        DESKTOPVERTRES = 117

        ' http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
    End Enum

    Public Function getScalingFactor() As Single
        Dim g As Graphics = Graphics.FromHwnd(IntPtr.Zero)
        Dim desktop As IntPtr = g.GetHdc()
        Dim LogicalScreenHeight As Integer = GetDeviceCaps(desktop, CInt(DeviceCap.VERTRES))
        Dim PhysicalScreenHeight As Integer = GetDeviceCaps(desktop, CInt(DeviceCap.DESKTOPVERTRES))

        Dim ScreenScalingFactor As Single = CSng(PhysicalScreenHeight) / CSng(LogicalScreenHeight)

        Return ScreenScalingFactor
        ' 1.25 = 125%
    End Function
#End Region

End Class