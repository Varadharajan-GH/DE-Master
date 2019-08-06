
Public Class frmLogin

    'Public Const SOURCEPATH As String = ".\"
    Public Const SOURCEPATH As String = "D:\data\C_Process\"

    Dim usersFile As String = SOURCEPATH & "Admin\users.txt"

    Private Sub cmdExit_Click(sender As Object, e As EventArgs)
        Application.Exit()
    End Sub

    Private Sub cmdLogin_Click(sender As Object, e As EventArgs) Handles cmdLogin.Click
        Dim bLoggedIn As Boolean = False
        Dim TextLine As String = ""

        If System.IO.File.Exists(usersFile) = True Then
            Dim objReader As New System.IO.StreamReader(usersFile)

            Do While (objReader.Peek() <> -1 And bLoggedIn = False)
                TextLine = objReader.ReadLine()
                If TextLine = txtUserName.Text & " " & txtPassword.Text Then
                    bLoggedIn = True
                    txtPassword.Clear()
                    txtPassword.Focus()
                    Me.Hide()
                    frmMain.Enabled = True
                    frmMain.Show()
                    frmMain.lblUName.Text = txtUserName.Text
                    If lblLField.Text = "" Then
                        lblLField.Text = "txtAuthor"
                    End If
                    frmMain.Controls.Find(lblLField.Text, True)(0).Focus()
                    CType(frmMain.Controls.Find(lblLField.Text, True)(0), TextBox).SelectionStart = CType(frmMain.Controls.Find(lblLField.Text, True)(0), TextBox).Text.Length
                    CType(frmMain.Controls.Find(lblLField.Text, True)(0), TextBox).SelectionLength = 0
                End If
            Loop
            If Not bLoggedIn Then MsgBox("Wrong credentials")
            txtPassword.Focus()
            txtPassword.SelectAll()
            objReader.Close()
        Else
            MessageBox.Show("Users File Does Not Exist")
        End If
    End Sub

    Private Sub frmLogin_Disposed(sender As Object, e As EventArgs) Handles Me.Disposed
        FormMode.Show()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
        txtUserName.Focus()
    End Sub

    Private Sub txtUserName_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtUserName.KeyPress
        If Asc(e.KeyChar) = 13 Then
            txtPassword.Focus()
            txtPassword.SelectAll()
            e.Handled = True
        End If
    End Sub

    Private Sub txtPassword_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtPassword.KeyPress
        If Asc(e.KeyChar) = 13 Then
            cmdLogin.Focus()
            e.Handled = True
            cmdLogin.PerformClick()
        End If
    End Sub

End Class