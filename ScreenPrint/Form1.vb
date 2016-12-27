Imports System.Windows.Forms
Imports System.ComponentModel
Imports System.Runtime.InteropServices
Imports System.Drawing.Imaging
Imports System.Net

Public Class Form1
    Private Const WM_HOTKEY As Integer = &H312
    Private Const MOD_ALT As Integer = &H1
    Private Const MOD_CONTROL As Integer = &H2
    Private Const MOD_SHIFT As Integer = &H4

    Private contextMenu1 As System.Windows.Forms.ContextMenu
    Friend WithEvents notifyIcon1 As System.Windows.Forms.NotifyIcon
    Friend WithEvents menuItem1, menuItem2, menuItem3 As System.Windows.Forms.MenuItem
    Private WithEvents httpclient As WebClient
    Dim keyCode, keyCode2
    Dim keyModifier, keyModifier2
    Dim asdf
    Dim firstRun = 0
    Dim tries As Integer = 0
    Public item1 As New ListViewItem("Images", 0)
    Public imageListLarge As New ImageList
    Public noOfSaves = 0
    Dim enterToEscape = False

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        My.Settings.Save()

        e.Cancel = True

        Me.WindowState = FormWindowState.Minimized
        Me.Visible = False
    End Sub

    Private Sub Form1_HandleCreated(sender As Object, e As EventArgs) Handles Me.HandleCreated

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.contextMenu1 = New System.Windows.Forms.ContextMenu
        Me.menuItem1 = New System.Windows.Forms.MenuItem
        Me.menuItem1.Index = 0
        Me.menuItem1.Text = "Restore"
        Me.menuItem2 = New System.Windows.Forms.MenuItem
        Me.menuItem2.Index = 2
        Me.menuItem2.Text = "Exit"
        Me.menuItem3 = New System.Windows.Forms.MenuItem
        Me.menuItem3.Index = 1
        Me.menuItem3.Text = "Clipboard"

        If My.Settings.Format = ".png" Then
            RadioButtonPNG.Select()
        ElseIf My.Settings.Format = ".jpg" Then
            RadioButtonJPG.Select()
        ElseIf My.Settings.Format = ".bmp" Then
            RadioButtonBMP.Select()
        ElseIf My.Settings.Format = ".gif" Then
            RadioButtonGIF.Select()
        End If

        Me.contextMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.menuItem1, menuItem3, Me.menuItem2})
        Me.notifyIcon1 = New System.Windows.Forms.NotifyIcon(Me.components)
        notifyIcon1.Icon = (Me.Icon)
        notifyIcon1.ContextMenu = contextMenu1

        If My.Settings.SaveDir = "nib" Then
            My.Settings.SaveDir = FileIO.SpecialDirectories.MyPictures & "\ScreenPrint\"
        End If


        TextBox1.Text = My.Settings.SaveDir
        ComboBox1.SelectedItem = My.Settings.CtrlAltFn
        ComboBox2.SelectedItem = My.Settings.Key.ToString
        ComboBox4.SelectedItem = My.Settings.SelectRegionCtrlAltFn
        ComboBox5.SelectedItem = My.Settings.SelectRegionKey


        httpclient = New WebClient
        Dim sourceurl = "https://sites.google.com/site/harrybrownsoftware/screenprintupdate.txt"
        Dim fildir = My.Application.Info.DirectoryPath.ToString & "\" & "screenprintupdate.txt"
        Try
            httpclient.DownloadFileAsync(New Uri(sourceurl), (fildir))
        Catch ex As Exception
            MsgBox("Failed to check for updates. " & ErrorToString(), MsgBoxStyle.Critical, "Error")
        End Try


        Dim xSize As System.Drawing.Size
        xSize.Height = 196
        xSize.Width = 196
        imageListLarge.ImageSize = xSize
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            notifyIcon1.Visible = True
            Me.Visible = False
            If firstRun = 0 Then
                notifyIcon1.BalloonTipText = "ScreenPrint is now running from the system tray."
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info
                notifyIcon1.ShowBalloonTip(500)
                firstRun += 1
            End If
        End If
    End Sub

    Private Sub notifyIcon1_DoubleClick(Sender As Object, e As EventArgs) Handles notifyIcon1.DoubleClick
        If Me.WindowState = FormWindowState.Minimized Then
            Me.Visible = True
            Me.WindowState = FormWindowState.Normal
        End If

        Me.BringToFront()
    End Sub

    Private Sub menuItem1_Click(Sender As Object, e As EventArgs) Handles menuItem1.Click
        If Me.WindowState = FormWindowState.Minimized Then
            Me.Visible = True
            Me.WindowState = FormWindowState.Normal
        End If

        Me.BringToFront()
    End Sub

    Private Sub menuItem2_Click(Sender As Object, e As EventArgs) Handles menuItem2.Click
        My.Settings.Save()
        Hotkey.unregisterHotkeys(Me, 1)
        Hotkey.unregisterHotkeys(Me, 2)
        Hotkey.unregisterHotkeys(Me, 3)

        End
    End Sub

    Private Sub menuItem3_Click(Sender As Object, e As EventArgs) Handles menuItem3.Click
        Clipboard.Show()
        Clipboard.BringToFront()
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If (FolderBrowserDialog1.ShowDialog() = DialogResult.OK) Then
            TextBox1.Text = FolderBrowserDialog1.SelectedPath
            If TextBox1.Text.Last <> "\" Then
                TextBox1.Text += "\"
            End If
            If System.IO.Directory.Exists(TextBox1.Text) Then
                My.Settings.SaveDir = TextBox1.Text
            Else
                MsgBox("Error selecting directory.", MsgBoxStyle.OkOnly, "Warning")
            End If
        End If
    End Sub

    Private Sub TextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyDown
        If e.KeyCode = Keys.Enter Then
            If TextBox1.Text.Last <> "\" Then
                TextBox1.Text += "\"
            End If
            If System.IO.Directory.Exists(TextBox1.Text) Then
                My.Settings.SaveDir = TextBox1.Text
                MsgBox(My.Settings.SaveDir)
            Else
                My.Settings.SaveDir = TextBox1.Text
                MsgBox("Directory does not currently exist. When the first screen capture is taken, ScreenPrint will try to create the directory instead.", MsgBoxStyle.OkOnly, "Warning")
            End If
        End If
        enterToEscape = True
    End Sub

    Private Sub TextBox1_LostFocus(sender As Object, e As EventArgs) Handles TextBox1.LostFocus
        If enterToEscape = False Then
            If TextBox1.Text.Last <> "\" Then
                TextBox1.Text += "\"
            End If
            If System.IO.Directory.Exists(TextBox1.Text) Then
                My.Settings.SaveDir = TextBox1.Text
            Else
                My.Settings.SaveDir = TextBox1.Text
                MsgBox("Directory does not currently exist. When the first screen capture is taken, ScreenPrint will try to create the directory instead.", MsgBoxStyle.OkOnly, "Warning")
            End If
        End If
        enterToEscape = True
    End Sub

    'System wide hotkey event handling
    Protected Overrides Sub WndProc(ByRef m As System.Windows.Forms.Message)
        If m.Msg = Hotkey.WM_HOTKEY Then
            Hotkey.handleHotKeyEvent(m.WParam)
        End If
        MyBase.WndProc(m)
    End Sub

#Region "ComboBox Changes"
    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Hotkey.unregisterHotkeys(Me, 1)
        Hotkey.unregisterHotkeys(Me, 2)

        My.Settings.CtrlAltFn = ComboBox1.SelectedItem
        Label7.Text = ComboBox1.SelectedItem

        If My.Settings.CtrlAltFn = "Ctrl" Then
            keyModifier = Hotkey.KeyModifier.Control
        ElseIf My.Settings.CtrlAltFn = "Alt" Then
            keyModifier = Hotkey.KeyModifier.Alt
        End If

        Hotkey.RegisterHotKey(Me, keyCode, keyModifier, 1)
        Hotkey.RegisterHotKey(Me, keyCode, (Hotkey.KeyModifier.Shift Xor keyModifier), 2)

        My.Settings.Save()
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        Hotkey.unregisterHotkeys(Me, 1)
        Hotkey.unregisterHotkeys(Me, 2)

        My.Settings.Key = ComboBox2.SelectedItem
        Label8.Text = ComboBox2.SelectedItem

        keyCode = KeySet1()

        Hotkey.RegisterHotKey(Me, keyCode, keyModifier, 1)
        Hotkey.RegisterHotKey(Me, keyCode, (Hotkey.KeyModifier.Shift Xor keyModifier), 2)

        My.Settings.Save()
    End Sub

    Private Sub ComboBox4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox4.SelectedIndexChanged
        Hotkey.unregisterHotkeys(Me, 3)

        My.Settings.SelectRegionCtrlAltFn = ComboBox4.SelectedItem

        If My.Settings.SelectRegionCtrlAltFn = "Ctrl" Then
            keyModifier2 = Hotkey.KeyModifier.Control
        ElseIf My.Settings.SelectRegionCtrlAltFn = "Alt" Then
            keyModifier2 = Hotkey.KeyModifier.Alt
        End If

        My.Settings.SelectRegionCtrlAltFn = ComboBox4.SelectedItem

        Hotkey.RegisterHotKey(Me, keyCode2, keyModifier2, 3)

        My.Settings.Save()
    End Sub

    Private Sub ComboBox5_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox5.SelectedIndexChanged
        Hotkey.unregisterHotkeys(Me, 3)

        My.Settings.SelectRegionKey = ComboBox5.SelectedItem

        keyCode2 = KeySet2()

        Hotkey.RegisterHotKey(Me, keyCode2, keyModifier2, 3)

        My.Settings.Save()
    End Sub

#End Region

#Region "Boring stuff"
    Private Function KeySet1()
        If My.Settings.Key = "F1" Then
            Return 112
        ElseIf My.Settings.Key = "F2" Then
            Return 113
        ElseIf My.Settings.Key = "F3" Then
            Return 114
        ElseIf My.Settings.Key = "F4" Then
            Return 115
        ElseIf My.Settings.Key = "F5" Then
            Return 116
        ElseIf My.Settings.Key = "F6" Then
            Return 117
        ElseIf My.Settings.Key = "F7" Then
            Return 118
        ElseIf My.Settings.Key = "F8" Then
            Return 119
        ElseIf My.Settings.Key = "F9" Then
            Return 120
        ElseIf My.Settings.Key = "F10" Then
            Return 121
        ElseIf My.Settings.Key = "F11" Then
            Return 122
        ElseIf My.Settings.Key = "F12" Then
            Return 123
        Else
            Return Asc(My.Settings.Key)
        End If
    End Function

    Private Function KeySet2()
        If My.Settings.SelectRegionKey = "[" Then
            Return Keys.OemOpenBrackets
        ElseIf My.Settings.SelectRegionKey = "]" Then
            Return 221
        ElseIf My.Settings.SelectRegionKey = ";" Then
            Return 186
        ElseIf My.Settings.SelectRegionKey = "'" Then
            Return 192
        ElseIf My.Settings.SelectRegionKey = "#" Then
            Return 222
        ElseIf My.Settings.SelectRegionKey = "," Then
            Return 188
        ElseIf My.Settings.SelectRegionKey = "." Then
            Return 190
        ElseIf My.Settings.SelectRegionKey = "/" Then
            Return 191
        Else
            MsgBox("ERRO")
        End If
    End Function
#End Region

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        My.Settings.Save()
        Hotkey.unregisterHotkeys(Me, 1)
        Hotkey.unregisterHotkeys(Me, 2)
        Hotkey.unregisterHotkeys(Me, 3)

        End
    End Sub

#Region "Radio button changes"
    Private Sub RadioButtonPNG_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButtonPNG.CheckedChanged
        If RadioButtonPNG.Checked = True Then
            My.Settings.Format = ".png"
        End If
    End Sub

    Private Sub RadioButtonJPG_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButtonJPG.CheckedChanged
        If RadioButtonJPG.Checked = True Then
            My.Settings.Format = ".jpg"
        End If
    End Sub

    Private Sub RadioButtonBMP_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButtonBMP.CheckedChanged
        If RadioButtonBMP.Checked = True Then
            My.Settings.Format = ".bmp"
        End If
    End Sub

    Private Sub RadioButtonGIF_CheckedChanged(sender As Object, e As EventArgs) Handles RadioButtonGIF.CheckedChanged
        If RadioButtonGIF.Checked = True Then
            My.Settings.Format = ".gif"
        End If
    End Sub

#End Region

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        About.Show()
        About.BringToFront()
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        Try
            If CheckBox1.Checked = True Then
                My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True).SetValue(Application.ProductName, Application.ExecutablePath)
            Else
                My.Computer.Registry.LocalMachine.OpenSubKey("SOFTWARE\Microsoft\Windows\CurrentVersion\Run", True).DeleteValue(Application.ProductName)

            End If
        Catch
            MsgBox("Could not complete action. Try running as administrator.", MsgBoxStyle.Exclamation, "Error")
        End Try

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Clipboard.Show()
        Clipboard.BringToFront()
    End Sub

    Private Sub httpclient_DownloadFileCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs) Handles httpclient.DownloadFileCompleted

        If tries = 0 Then

            Dim filetext As String = IO.File.ReadAllText(My.Application.Info.DirectoryPath.ToString & "\" & "screenprintupdate.txt")
            If filetext > My.Application.Info.Version.ToString Then
                If MsgBox("New updates are available! Visit website to download? ", MsgBoxStyle.YesNo, "Updates available") = MsgBoxResult.Yes Then
                    Process.Start("https://sites.google.com/site/harrybrownsoftware/downloads")
                End If
            Else

            End If
            tries = 1
        End If
    End Sub
End Class

Public Class Hotkey
    Public Shared Function captureActiveWindow() As Image
        Return CaptureWindow(User32.GetForegroundWindow, Nothing)
    End Function

    Public Shared Function CaptureWindow(ByVal handle As IntPtr, ByVal r As Rectangle) As Image
        ' get the size
        Dim windowRect As New User32.RECT

        Dim width As Integer
        Dim height As Integer

        If r = Nothing Then
            User32.GetWindowRect(handle, windowRect)
            windowRect.left -= 1
            windowRect.top -= 1
            width = windowRect.right - windowRect.left + 1
            height = windowRect.bottom - windowRect.top + 1
        Else
            windowRect.left = r.Left + 1
            windowRect.top = r.Top + 1
            width = r.Width - 1
            height = r.Height - 1
            windowRect.right = windowRect.left + width
            windowRect.bottom = windowRect.top + height
        End If

        width *= SaveForm.getScalingFactor()
        height *= SaveForm.getScalingFactor()

        Dim img As Bitmap = New Bitmap(width, height)
        Dim gr As Graphics = Graphics.FromImage(img)
        gr.CopyFromScreen(windowRect.left * SaveForm.getScalingFactor(), windowRect.top * SaveForm.getScalingFactor(), 0, 0, New Size(width, height))

        Return img

    End Function

#Region "Declarations - WinAPI, Hotkey constant and Modifier Enum"
    ''' <summary>
    ''' Declaration of winAPI function wrappers. The winAPI functions are used to register / unregister a hotkey
    ''' </summary>
    Public Declare Function RegisterHotKey Lib "user32" _
    (ByVal hwnd As IntPtr, ByVal id As Integer, ByVal fsModifiers As Integer, ByVal vk As Integer) As Integer

    Public Declare Function UnregisterHotKey Lib "user32" (ByVal hwnd As IntPtr, ByVal id As Integer) As Integer

    Public Const WM_HOTKEY As Integer = &H312

    Enum KeyModifier
        None = 0
        Alt = &H1
        Control = &H2
        Shift = &H4
        Winkey = &H8
    End Enum 'This enum is just to make it easier to call the registerHotKey function: The modifier integer codes are replaced by a friendly "Alt","Shift" etc.
#End Region


#Region "Hotkey registration, unregistration and handling"
    Public Shared Sub registerHotkey(ByRef sourceForm As Form, ByVal triggerKey As Integer, ByVal modifier As KeyModifier, ByVal hotkeyID As Integer)
        RegisterHotKey(sourceForm.Handle, hotkeyID, modifier, triggerKey)
    End Sub
    Public Shared Sub unregisterHotkeys(ByRef sourceForm As Form, ByVal hotkeyID As Integer)
        UnregisterHotKey(sourceForm.Handle, hotkeyID)  'Remember to call unregisterHotkeys() when closing your application.
    End Sub
    Public Shared Sub handleHotKeyEvent(ByVal hotkeyID As IntPtr)
        Dim bounds As Rectangle
        Dim screenshot As System.Drawing.Bitmap
        Dim graph As Graphics

        Dim month As String = Date.Today.Month.ToString
        If Len(month) = 1 Then
            month = "0" + month
        End If
        Dim day As String = Date.Today.Day.ToString
        If Len(day) = 1 Then
            day = "0" + day
        End If

        Dim format

        Select Case My.Settings.Format.ToString
            Case ".png"
                format = System.Drawing.Imaging.ImageFormat.Png
            Case ".jpg"
                format = System.Drawing.Imaging.ImageFormat.Jpeg
            Case ".bmp"
                format = System.Drawing.Imaging.ImageFormat.Bmp
            Case ".gif"
                format = System.Drawing.Imaging.ImageFormat.Gif
        End Select

        Dim FileName As String = ("Screenshot_" + Date.Today.Year.ToString + month + day + "_" + DateTime.Now.ToString("HHmmssff"))


        Select Case hotkeyID
            Case 1
                bounds = Screen.PrimaryScreen.Bounds
                screenshot = New System.Drawing.Bitmap(bounds.Width * SaveForm.getScalingFactor(), bounds.Height * SaveForm.getScalingFactor(), System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                graph = Graphics.FromImage(screenshot)

                Dim blockRegionSize = bounds.Size
                blockRegionSize.Height = blockRegionSize.Height * SaveForm.getScalingFactor()
                blockRegionSize.Width = blockRegionSize.Width * SaveForm.getScalingFactor()

                graph.CopyFromScreen(bounds.X * SaveForm.getScalingFactor(), bounds.Y * SaveForm.getScalingFactor(), 0, 0, blockRegionSize, CopyPixelOperation.SourceCopy)
                Form1.PictureBox1.Image = screenshot

                If System.IO.Directory.Exists(My.Settings.SaveDir) Then
                    Try
                        Form1.PictureBox1.Image.Save(My.Settings.SaveDir + FileName + My.Settings.Format, format)
                    Catch
                        MsgBox("Unable to save screenshot. Is the file path valid?", MsgBoxStyle.Critical, "Error")
                    End Try
                Else
                    Try
                        System.IO.Directory.CreateDirectory(My.Settings.SaveDir)
                        Form1.PictureBox1.Image.Save(My.Settings.SaveDir + FileName + My.Settings.Format, format)
                    Catch
                        MsgBox("Unable to save screenshot. Is the file path valid?", MsgBoxStyle.Critical, "Error")
                    End Try
                End If

            Case 2

                Form1.PictureBox1.Image = captureActiveWindow()
                If System.IO.Directory.Exists(My.Settings.SaveDir) Then
                    Try
                        Form1.PictureBox1.Image.Save(My.Settings.SaveDir + FileName + My.Settings.Format, format)
                    Catch
                        MsgBox("Unable to save screenshot. Is the file path valid?", MsgBoxStyle.Critical, "Error")
                    End Try
                Else
                    Try
                        System.IO.Directory.CreateDirectory(My.Settings.SaveDir)
                        Form1.PictureBox1.Image.Save(My.Settings.SaveDir + FileName + My.Settings.Format, format)
                    Catch
                        MsgBox("Unable to save screenshot. Is the file path valid?", MsgBoxStyle.Critical, "Error")
                    End Try
                End If

            Case 3
                SelectableRegion1.Show()
        End Select
    End Sub
#End Region

    Private Sub TakeScreenshot()

    End Sub
End Class

Public Class User32

    Public Structure RECT
        Public left As Integer
        Public top As Integer
        Public right As Integer
        Public bottom As Integer
    End Structure 'RECT

    Declare Function GetDesktopWindow Lib "user32.dll" () As IntPtr

    Declare Function GetWindowRect Lib "user32.dll" ( _
        ByVal hwnd As IntPtr, _
        ByRef lpRect As RECT) As Int32

    Declare Function GetForegroundWindow Lib "user32.dll" _
    Alias "GetForegroundWindow" () As IntPtr
End Class