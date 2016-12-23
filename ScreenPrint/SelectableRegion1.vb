Imports System.Drawing

Public Class SelectableRegion1

    Dim topleftX, topleftY

    Private Sub SelectableRegion1_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Dim keyCode
        keyCode = KeySet2()

        If e.KeyCode = keyCode Then
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

    End Sub

    Private Sub SelectableRegion1_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        Me.Hide()

        SaveForm.Show()
        
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
#End Region
End Class