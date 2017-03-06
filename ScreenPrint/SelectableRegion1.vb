Imports System.Drawing
Imports System.Runtime.InteropServices

Public Class SelectableRegion1
    Dim topleftX, topleftY

    Private Sub SelectableRegion1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Dim keyCode
        keyCode = KeySet2()

        If e.KeyCode = keyCode Or e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub SelectableRegion1_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        SelectableRegion2.Show()
        SelectableRegion2.Location = Cursor.Position
        topleftX = SelectableRegion2.Location.X
        topleftY = SelectableRegion2.Location.Y
        SelectableRegion2.Location = SelectableRegion2.Location
        SelectableRegion2.Width = 1
        SelectableRegion2.Height = 1

        ColourPicker.Hide()
    End Sub

    Private Sub SelectableRegion1_MouseMove(sender As Object, e As MouseEventArgs) Handles Me.MouseMove
        If Cursor.Position.X > topleftX And Cursor.Position.Y > topleftY Then
            SelectableRegion2.Size = Cursor.Position - SelectableRegion2.Location
            topleftX = SelectableRegion2.Location.X
            topleftY = SelectableRegion2.Location.Y

        End If

        If Cursor.Position.Y < topleftY Then
            SelectableRegion2.Top = Cursor.Position.Y
            SelectableRegion2.Height = topleftY - Cursor.Position.Y

            If Cursor.Position.X < topleftX Then
                SelectableRegion2.Left = Cursor.Position.X
                SelectableRegion2.Width = topleftX - Cursor.Position.X
            Else
                SelectableRegion2.Width = Cursor.Position.X - topleftX
            End If
        End If

        If Cursor.Position.X < topleftX Then
            SelectableRegion2.Left = Cursor.Position.X
            SelectableRegion2.Width = topleftX - Cursor.Position.X

            If Cursor.Position.Y < topleftY Then
                SelectableRegion2.Top = Cursor.Position.Y
                SelectableRegion2.Height = topleftY - Cursor.Position.Y
            Else
                SelectableRegion2.Height = Cursor.Position.Y - topleftY
            End If
        End If

        If My.Settings.ColourPicker = True Then
            ColourPicker.Left = Cursor.Position.X + 10
            ColourPicker.Top = Cursor.Position.Y - 85

            Dim a As New Drawing.Bitmap(1, 1)
            Dim b As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(a)
            b.CopyFromScreen(New Drawing.Point(MousePosition.X, MousePosition.Y), New Drawing.Point(0, 0), a.Size)
            Dim c As Drawing.Color = a.GetPixel(0, 0)
            ColourPicker.Panel1.BackColor = c
            ColourPicker.Label2.Text = c.R
            ColourPicker.Label3.Text = c.G
            ColourPicker.Label4.Text = c.B
        End If

    End Sub

    Private Sub SelectableRegion1_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        Me.Hide()
        SaveForm.Show()
    End Sub

    Private Sub SelectableRegion1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim bounds = Screen.PrimaryScreen.Bounds

        For i As Integer = 0 To Screen.AllScreens.Count - 1
            bounds = Rectangle.Union(bounds, Screen.AllScreens(i).Bounds)
        Next

        Me.Bounds = bounds

        ColourPicker.Show()
    End Sub

#Region "Boring stuff"
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

    Private Sub SelectableRegion1_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        ColourPicker.Hide()
    End Sub
#End Region
End Class