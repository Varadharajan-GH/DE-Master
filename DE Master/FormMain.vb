Option Explicit On

Imports System.Xml
Imports System.Text.RegularExpressions

Structure Log
    Dim System_Type As String
    Dim System As String
    Dim Session_Start_Date As String
    Dim Session_Start_Time As String
    Dim Session_End_Date As String
    Dim Session_End_Time As String
    Dim Session_Production As String
    Dim Session_Suspend As String
    Dim Item_Accession As String
    Dim Item_Itemno As String
    Dim Status As String
    Dim References_Open As String
    Dim References_Close As String
    Dim References_Inserted As String
    Dim References_Deleted As String
End Structure

Public Class frmMain
    Dim srcPath As String = frmLogin.SOURCEPATH
    Dim xmlDoc As New XmlDocument(), xnlCitations As XmlNodeList
    Dim iNo As Integer, strInFile As String, curRef As Integer
    Public strInPath As String
    Dim strOutPath As String
    Dim strTempPath As String
    Dim strHelpPath As String
    Dim strLogPath As String
    Dim strQueryPath As String
    Dim dtSTime As DateTime
    Dim bNTR() As Boolean
    Dim bPattent() As Boolean
    Dim swProdTime As Stopwatch = New Stopwatch
    Dim swSuspTime As Stopwatch = New Stopwatch
    Dim swCurProd As Stopwatch = New Stopwatch
    Dim swCurSusp As Stopwatch = New Stopwatch
    Dim strPriorPath As String
    Dim strCurPath As String
    Dim strInvalidPath As String
    Dim iCompItems As Integer, iCompRefs As Integer
    Dim policyFile As String = srcPath & "Admin\policy.txt"
    Dim RepFile As String = srcPath & "Admin\DCI Repository.XML"
    Dim mRect As Rectangle
    Dim bIsPrior As Boolean
    Dim bIsPatent As Boolean
    Dim LogEntry As New Log
    Dim LogLoadFile As String
    Dim LogDoneFile As String

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim i As Integer

        If FormMode.chosenTool = ToolMode.QC Then
            strInPath = srcPath & "Output\"
            strCurPath = srcPath & "QC\Current\"
            strPriorPath = srcPath & "QC\Priority\"
            strOutPath = srcPath & "QC\Output\"
            strTempPath = srcPath & "QC\Temp\"
            strInvalidPath = srcPath & "QC\Invalid\"
            strHelpPath = srcPath & "Help\"
            strQueryPath = srcPath & "QC\Query\"
            strLogPath = srcPath & "QC\C_Logs\" & Format(Today, "yyyyMMdd") & "\"
        Else
            strInPath = srcPath & "Input\"
            strCurPath = srcPath & "Current\"
            strPriorPath = srcPath & "Priority\"
            strOutPath = srcPath & "Output\"
            strTempPath = srcPath & "Temp\"
            strHelpPath = srcPath & "Help\"
            strInvalidPath = srcPath & "Invalid\"
            strQueryPath = srcPath & "Query\"
            strLogPath = srcPath & "C_Logs\" & Format(Today, "yyyyMMdd") & "\"
        End If

        If Not My.Settings.MainBackColor.IsEmpty Then
            Me.BackColor = My.Settings.MainBackColor
            Me.gbZoneTool.BackColor = My.Settings.MainBackColor
        End If
        txtOCR.ContextMenu = New ContextMenu()

        LogLoadFile = strLogPath & "LogLoad-" & frmLogin.txtUserName.Text & ".txt"
        LogDoneFile = strLogPath & "LogDone-" & frmLogin.txtUserName.Text & ".txt"

        'Create Directories if they doesn't exist
        MakeDirectory(strCurPath)
        MakeDirectory(strPriorPath)
        MakeDirectory(strOutPath)
        MakeDirectory(strTempPath)
        MakeDirectory(strQueryPath)
        MakeDirectory(strInvalidPath)
        MakeDirectory(strLogPath)

        iCompItems = 0
        iCompRefs = 0
        lblCompItems.Text = iCompItems
        lblCompRefs.Text = iCompRefs
        dtSTime = Now
        swProdTime = Stopwatch.StartNew
        For i = 2 To 4
            Me.Controls.Find("txtArtn" & i, True)(0).Visible = False
            Me.Controls.Find("cmbArtn" & i, True)(0).Visible = False
        Next

        strInFile = fnGetXMLFile(strPriorPath)
        If strInFile = String.Empty Then
            bIsPrior = False
            strInFile = fnGetXMLFile(strInPath)

            If strInFile = "" Then
                MsgBox("No files for input", MsgBoxStyle.Information)
            Else
                MakeLogFile(strLogPath & frmLogin.txtUserName.Text & ".XML")
                LogEntry.System_Type = ""
                LogEntry.System = ""
                If Net.Dns.GetHostEntry(Net.Dns.GetHostName()).AddressList.Count > 3 Then
                    LogEntry.System_Type = "IP_Address"
                    LogEntry.System = Net.Dns.GetHostEntry(Net.Dns.GetHostName()).AddressList(3).ToString()
                Else
                    LogEntry.System_Type = "Name"
                    LogEntry.System = Net.Dns.GetHostName()
                End If
                fnLoadFile(strInFile)
            End If
        Else

            MakeLogFile(strLogPath & frmLogin.txtUserName.Text & ".XML")
            LogEntry.System_Type = ""
            LogEntry.System = ""
            If Net.Dns.GetHostEntry(Net.Dns.GetHostName()).AddressList.Count > 3 Then
                LogEntry.System_Type = "IP_Address"
                LogEntry.System = Net.Dns.GetHostEntry(Net.Dns.GetHostName()).AddressList(3).ToString()
            Else
                LogEntry.System_Type = "Name"
                LogEntry.System = Net.Dns.GetHostName()
            End If
            bIsPrior = True
            fnLoadFile(strInFile)
        End If
    End Sub

    Private Sub frmMain_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        focusToBox()
    End Sub

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub frmMain_EnabledChanged(sender As Object, e As EventArgs) Handles Me.EnabledChanged
        If Me.Enabled = True Then
            swSuspTime.Stop()
            swProdTime.Start()

            swCurSusp.Stop()
            swCurProd.Start()
        Else
            swProdTime.Stop()
            swSuspTime.Start()

            swCurProd.Stop()
            swCurSusp.Start()
        End If
    End Sub

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason = CloseReason.UserClosing Then
            cmdClose.PerformClick()
            e.Cancel = True
        End If
    End Sub

    Private Sub frmMain_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        Select Case e.KeyCode
            Case Keys.F1
                Help.ShowHelp(Me, strHelpPath & "ISIHELP.hlp.12-1-11.HLP")
                e.Handled = True
            Case Keys.F4
                cmdPrev.PerformClick()
                e.Handled = True
            Case Keys.F3
                cmdNext.PerformClick()
                e.Handled = True
            Case Keys.F10
                cmdDone.PerformClick()
                e.Handled = True
            Case Keys.F11
                cmdSuspend.PerformClick()
                e.Handled = True
            Case Keys.F7
                If (e.Modifiers = Keys.Control) Then
                    cmdAddArtn.PerformClick()
                    e.Handled = True
                ElseIf (e.Modifiers = Keys.Alt) Then
                    cmdDelArtn.PerformClick()
                    e.Handled = True
                ElseIf (e.Modifiers = Keys.None) Then
                    If iNo = 0 Then Exit Sub
                    Dim nod As XmlNode
                    nod = xnlCitations(iNo - 1).SelectSingleNode("CI_INFO/CI_JOURNAL")
                    If nod Is Nothing Then nod = xnlCitations(iNo - 1).SelectSingleNode("CI_INFO/CI_PATENT")
                    If nod Is Nothing Then Exit Sub
                    On Error Resume Next
                    Dim tstr As String
                    If rbAuthor.Checked Then
                        tstr = StrConv((nod.SelectSingleNode("CI_AUTHOR").InnerText), VbStrConv.Uppercase)
                        If tstr.Contains(".") Then
                            Dim atstr() As String = Split(tstr, ".")
                            tstr = Regex.Replace(atstr(0), "[^A-Z0-9* ]+", String.Empty) & "." & Regex.Replace(atstr(1), "[^A-Z0-9* ]+", String.Empty)
                        Else
                            tstr = Regex.Replace(tstr, "[^A-Z0-9* ]+", String.Empty)
                        End If
                        txtAuthor.Text = tstr

                        'txtAuthor.Text = Regex.Replace(StrConv((nod.SelectSingleNode("CI_AUTHOR").InnerText), VbStrConv.Uppercase), "[^A-Z0-9 ]+", String.Empty)
                        txtAuthor.Focus()
                        txtAuthor.SelectionStart = txtAuthor.Text.Length
                        txtAuthor.SelectionLength = 0
                    ElseIf rbVolume.Checked Then
                        tstr = Regex.Replace(StrConv(nod.SelectSingleNode("CI_VOLUME").InnerText, VbStrConv.Uppercase), "[^a-zA-Z0-9 ]+", String.Empty).Trim
                        If tstr.Length > 4 Then tstr = tstr.Substring(0, 4)
                        txtVolume.Text = tstr
                        If IsNumeric(txtVolume.Text) Then
                            txtVolume.Text = CInt(txtVolume.Text)
                        End If
                        txtVolume.Focus()
                        txtVolume.SelectionStart = txtVolume.Text.Length
                        txtVolume.SelectionLength = 0
                    ElseIf rbPage.Checked Then
                        tstr = Regex.Replace(StrConv(nod.SelectSingleNode("CI_PAGE").InnerText, VbStrConv.Uppercase), "[^A-Z0-9 ]+", String.Empty)
                        tstr = Split(tstr, "-")(0)
                        tstr = Regex.Replace(tstr, "[^a-zA-Z0-9 ]", String.Empty).Trim
                        If tstr.Length > 5 Then tstr = tstr.Substring(0, 5)
                        If IsNumeric(txtPage.Text) Then
                            txtPage.Text = CInt(txtPage.Text)
                        End If
                        txtPage.Text = tstr
                        txtPage.Focus()
                        txtPage.SelectionStart = txtPage.Text.Length
                        txtPage.SelectionLength = 0
                    ElseIf rbYear.Checked Then
                        tstr = StrConv(nod.SelectSingleNode("CI_YEAR").InnerText, VbStrConv.Uppercase)

                        tstr = Regex.Replace(tstr, "[^0-9]", String.Empty).Trim
                        If tstr.Length > 4 Then tstr = tstr.Substring(0, 4)
                        'If Regex.Match(tStr, "\d").Success Then
                        txtYear.Text = CInt(tstr)
                        txtYear.Focus()
                        txtYear.SelectionStart = txtYear.Text.Length
                        txtYear.SelectionLength = 0
                    ElseIf rbTitle.Checked Then
                        tstr = StrConv(xnlCitations(iNo - 1).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_TITLE").InnerText, VbStrConv.Uppercase)
                        If tstr.Length > 256 Then tstr = tstr.Substring(0, 256)
                        txtTitle.Text = tstr
                        txtTitle.Focus()
                        txtTitle.SelectionStart = txtTitle.Text.Length
                        txtTitle.SelectionLength = 0
                    End If
                    e.Handled = True
                    On Error GoTo 0
                End If
            Case Keys.L
                If e.Modifiers = (Keys.Control Or Keys.Alt) Then
                    lvlist.Visible = Not lvlist.Visible
                    pnlSource.Visible = Not lvlist.Visible
                    gbZoneTool.Visible = Not lvlist.Visible
                    e.Handled = True
                End If
        End Select
    End Sub


    Private Sub cmdPrev_Click(sender As Object, e As EventArgs) Handles cmdPrev.Click
        Log_Done("CheckDOI started")
        If bCheckDOI() = False Then Exit Sub
        Log_Done("CheckDOI completed")
        Log_Done("CheckAuthor started")
        If bCheckAuthor() = False Then Exit Sub
        Log_Done("CheckAuthor completed")

        If Not AllValid() Then
            Exit Sub
        End If

        If fnAllVisited() Then
            subViewRef(iNo - 1, "Prev")
        ElseIf MsgBox("All fields are not visited.Do you want to leave anyway?", MsgBoxStyle.YesNo, "Confirm goto previous") = MsgBoxResult.Yes Then
            subViewRef(iNo - 1, "Prev")
        End If
    End Sub

    Private Sub cmdNext_Click(sender As Object, e As EventArgs) Handles cmdNext.Click
        Log_Done("CheckDOI started")
        If bCheckDOI() = False Then Exit Sub
        Log_Done("CheckDOI completed")
        Log_Done("CheckAuthor started")
        If bCheckAuthor() = False Then Exit Sub
        Log_Done("CheckAuthor completed")

        If Not AllValid() Then
            Exit Sub
        End If

        If fnAllVisited() Then
            subViewRef(iNo + 1)
        ElseIf MsgBox("All fields are not visited.Do you want to leave anyway?", MsgBoxStyle.YesNo, "Confirm goto next") = MsgBoxResult.Yes Then
            subViewRef(iNo + 1)
        End If
    End Sub

    Private Sub cmdClose_Click(sender As Object, e As EventArgs) Handles cmdClose.Click
        If MsgBox("Do you want to exit?", MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
            Try
                'swLogFile.Close()
            Catch ex As Exception
            End Try

            If strInFile = "" Then
                Application.Exit()
            End If

            If MsgBox("Do you want to exit without complete current item?", MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
                If My.Computer.FileSystem.FileExists(strCurPath & System.IO.Path.GetFileName(strInFile)) Then
                    If bIsPrior Then
                        My.Computer.FileSystem.MoveFile(strCurPath & System.IO.Path.GetFileName(strInFile), strPriorPath & System.IO.Path.GetFileName(strInFile), True)
                    Else
                        My.Computer.FileSystem.MoveFile(strCurPath & System.IO.Path.GetFileName(strInFile), strInPath & System.IO.Path.GetFileName(strInFile), True)
                    End If
                End If
                Application.Exit()
            End If
        End If
    End Sub

    Private Sub txtOCR_MouseDown(sender As Object, e As MouseEventArgs) Handles txtOCR.MouseDown
        If e.Button = MouseButtons.Right Then
            Dim strST As String = StrConv(txtOCR.SelectedText, VbStrConv.Uppercase)
            If chkNTR.CheckState = CheckState.Checked Then Exit Sub
            If chkTitErr.CheckState = CheckState.Checked Then Exit Sub
            If Trim(strST) = vbNullString Then Exit Sub
            Dim tStr As String
            tStr = Trim(strST)
            If rbAuthor.Checked Then
                'If tStr.Length > 18 Then tStr = tStr.Substring(0, 18)
                If tStr.Length > 15 Then tStr = tStr.Substring(0, 15) & "."
                txtAuthor.Text = tStr
                txtAuthor.Focus()
                txtAuthor.SelectionStart = txtAuthor.Text.Length
                txtAuthor.SelectionLength = 0
            ElseIf rbVolume.Checked Then
                tStr = Trim(Regex.Replace(tStr, "[^a-zA-Z0-9]", String.Empty))
                If tStr.Length > 4 Then tStr = tStr.Substring(0, 4)
                'If Regex.Match(tStr, "^[a-zA-Z0-9]+$").Success Then
                txtVolume.Text = tStr
                'End If
                txtVolume.Focus()
                txtVolume.SelectionStart = txtVolume.Text.Length
                txtVolume.SelectionLength = 0
            ElseIf rbPage.Checked Then
                tStr = Split(tStr, "-")(0)
                tStr = Trim(Regex.Replace(tStr, "[^a-zA-Z0-9 ]", String.Empty))
                If tStr.Length > 5 Then tStr = tStr.Substring(0, 5)
                'If Regex.Match(tStr, "^[a-zA-Z0-9]+$").Success Then
                txtPage.Text = tStr
                'End If
                txtPage.Focus()
                txtPage.SelectionStart = txtPage.Text.Length
                txtPage.SelectionLength = 0
            ElseIf rbYear.Checked Then
                tStr = Trim(Regex.Replace(tStr, "[^0-9]", String.Empty))
                If tStr.Length > 4 Then tStr = tStr.Substring(0, 4)
                'If Regex.Match(tStr, "\d").Success Then
                If tStr <> "" Then
                    txtYear.Text = CInt(tStr)
                End If
                'End If
                txtYear.Focus()
                txtYear.SelectionStart = txtYear.Text.Length
                txtYear.SelectionLength = 0
            ElseIf rbTitle.Checked Then
                If tStr.Length > 256 Then tStr = tStr.Substring(0, 256)
                txtTitle.Text = tStr
                txtTitle.Focus()
                txtTitle.SelectionStart = txtTitle.Text.Length
                txtTitle.SelectionLength = 0
            ElseIf rbARTN1.Checked Then
                'tStr = Trim(Regex.Replace(tStr, "[^a-zA-Z0-9 ]", String.Empty))
                txtArtn1.Text = tStr
                txtArtn1.Focus()
                txtArtn1.SelectionStart = txtArtn1.Text.Length
                txtArtn1.SelectionLength = 0
            ElseIf rbARTN2.Checked Then
                'tStr = Trim(Regex.Replace(tStr, "[^a-zA-Z0-9 ]", String.Empty))
                txtArtn2.Text = tStr
                txtArtn2.Focus()
                txtArtn2.SelectionStart = txtArtn2.Text.Length
                txtArtn2.SelectionLength = 0
            ElseIf rbARTN3.Checked Then
                'tStr = Trim(Regex.Replace(tStr, "[^a-zA-Z0-9 ]", String.Empty))
                txtArtn3.Text = tStr
                txtArtn3.Focus()
                txtArtn3.SelectionStart = txtArtn3.Text.Length
                txtArtn3.SelectionLength = 0
            ElseIf rbARTN4.Checked Then
                'tStr = Trim(Regex.Replace(tStr, "[^a-zA-Z0-9 ]", String.Empty))
                txtArtn4.Text = tStr
                txtArtn4.Focus()
                txtArtn4.SelectionStart = txtArtn4.Text.Length
                txtArtn4.SelectionLength = 0
            End If
        End If
    End Sub

    Private Sub cmdHelp_Click(sender As Object, e As EventArgs) Handles cmdHelp.Click
        Help.ShowHelp(Me, strHelpPath & "ISIHELP.hlp.12-1-11.HLP")
    End Sub

    Private Sub txtAuthor_GotFocus(sender As Object, e As EventArgs) Handles txtAuthor.GotFocus
        rbAuthor.Checked = True
        cbField1.Checked = True
    End Sub

    Private Sub txtVolume_GotFocus(sender As Object, e As EventArgs) Handles txtVolume.GotFocus
        rbVolume.Checked = True
        cbField2.Checked = True
    End Sub

    Private Sub txtPage_GotFocus(sender As Object, e As EventArgs) Handles txtPage.GotFocus
        rbPage.Checked = True
        cbField3.Checked = True
    End Sub

    Private Sub txtYear_GotFocus(sender As Object, e As EventArgs) Handles txtYear.GotFocus
        rbYear.Checked = True
        cbField4.Checked = True
    End Sub

    Private Sub txtTitle_GotFocus(sender As Object, e As EventArgs) Handles txtTitle.GotFocus
        rbTitle.Checked = True
        cbField5.Checked = True
        txtTitle.SelectionStart = txtTitle.Text.Length
        txtTitle.SelectionLength = 0
    End Sub

    Private Sub chkNTR_CheckedChanged(sender As Object, e As EventArgs) Handles chkNTR.CheckedChanged
        If chkNTR.CheckState = CheckState.Checked Then
            'subClearFields()
            'txtTitle.Text = "Non traditional reference"
            disableFields()
            'bNTR(iNo) = True
        ElseIf chkNTR.CheckState = CheckState.Unchecked Then
            enableFields()
            'bNTR(iNo) = False
        End If
    End Sub

    Private Sub chkDCI_CheckedChanged(sender As Object, e As EventArgs) Handles chkDCI.CheckedChanged
        If chkDCI.CheckState = CheckState.Checked Then
            'subClearFields()
            'txtTitle.Text = "Non traditional reference"
            disableFields()
            'bNTR(iNo) = True
        ElseIf chkDCI.CheckState = CheckState.Unchecked Then
            enableFields()
            'bNTR(iNo) = False
        End If
    End Sub

    Private Sub chkTitErr_CheckedChanged(sender As Object, e As EventArgs) Handles chkTitErr.CheckedChanged
        If chkTitErr.CheckState = CheckState.Checked Then
            disableFields()
        ElseIf chkTitErr.CheckState = CheckState.Unchecked Then
            enableFields()
        End If
    End Sub

    Private Sub txtAuthor_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtAuthor.KeyPress
        If Asc(e.KeyChar) = Keys.Space Then
            If txtAuthor.SelectionStart = 0 Then Exit Sub
            If txtAuthor.Text.Chars(txtAuthor.SelectionStart - 1) = " " Then
                txtAuthor.SelectionStart = txtAuthor.SelectionStart - 1
            End If
            If Not txtAuthor.Text.Trim.StartsWith("*") Then
                If txtAuthor.Text.Trim.Contains(" ") Then
                    If txtAuthor.Text.Chars(txtAuthor.SelectionStart - 1) = " " Then
                        txtAuthor.SelectionStart = txtAuthor.SelectionStart '- 1
                    End If
                    e.Handled = True
                End If
            End If
        End If
        If Asc(e.KeyChar) = Keys.Return Then
            txtVolume.Focus()
            e.Handled = True
            Exit Sub
        ElseIf e.KeyChar = "-" Then
            txtAuthor.Clear()
            e.Handled = True
            Exit Sub
        End If
        If (Char.IsControl(e.KeyChar) = False) Then
            If Char.IsPunctuation(e.KeyChar) Then
                If e.KeyChar = "*" Or e.KeyChar = "&" Then
                    If txtAuthor.SelectionStart > 0 Then
                        MsgBox(" */& allowed only at beginning", MsgBoxStyle.Information, "Verify")
                        e.Handled = True
                        txtAuthor.Focus()
                        txtAuthor.SelectionLength = 0
                        Exit Sub
                    Else
                        If txtAuthor.SelectedText <> "" Then
                            If Regex.Match(txtAuthor.Text.Replace(txtAuthor.SelectedText, ""), "[*|&]").Success Then
                                MsgBox(" */& allowed one time only", MsgBoxStyle.Information, "Verify")
                                e.Handled = True
                                txtAuthor.Focus()
                                txtAuthor.SelectionLength = 0
                                Exit Sub
                                'ElseIf txtAuthor.Text.StartsWith("*") Or txtAuthor.Text.StartsWith("&") Then
                            End If
                        Else
                            If Regex.Match(txtAuthor.Text, "[*|&]").Success Then
                                MsgBox(" */& allowed one time only", MsgBoxStyle.Information, "Verify")
                                e.Handled = True
                                txtAuthor.Focus()
                                txtAuthor.SelectionLength = 0
                                Exit Sub
                            End If
                        End If
                    End If
                ElseIf e.KeyChar = "." Then
                    If txtAuthor.SelectionStart <> 15 Then
                        MsgBox("Dot Allowed only after 15 chars!", MsgBoxStyle.Information, "Verify")
                        e.Handled = True
                        'txtAuthor.Focus()
                        txtAuthor.SelectionStart = txtAuthor.Text.Length
                        txtAuthor.SelectionLength = 0
                        Exit Sub
                    End If
                Else
                    MsgBox("Illegal character", MsgBoxStyle.Information, "Verify")
                    e.Handled = True
                    'txtAuthor.Focus()
                    txtAuthor.SelectionStart = txtAuthor.Text.Length
                    txtAuthor.SelectionLength = 0
                    Exit Sub
                End If
            ElseIf Char.IsSymbol(e.KeyChar) Then
                MsgBox("Illegal character", MsgBoxStyle.Information, "Verify")
                e.Handled = True
                'txtAuthor.Focus()
                txtAuthor.SelectionStart = txtAuthor.Text.Length
                'txtAuthor.SelectionLength = 0
                Exit Sub
            End If
        End If
    End Sub

    Private Sub txtVolume_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtVolume.KeyPress
        If Asc(e.KeyChar) = 13 Then
            txtPage.Focus()
            e.Handled = True
            Exit Sub
        ElseIf e.KeyChar = "-" Then
            txtVolume.Clear()
            e.Handled = True
            Exit Sub
        End If
        If (Char.IsControl(e.KeyChar) = False) Then
            If Not Char.IsLetterOrDigit(e.KeyChar) Then
                MsgBox("Only Letters & Digits Allowed!!", MsgBoxStyle.Information, "Verify")
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub txtPage_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtPage.KeyPress
        If Asc(e.KeyChar) = 13 Then
            txtYear.Focus()
            e.Handled = True
            Exit Sub
        ElseIf e.KeyChar = "-" Then
            txtPage.Clear()
            e.Handled = True
            Exit Sub
        End If
        If (Char.IsControl(e.KeyChar) = False) Then
            If Not Char.IsLetterOrDigit(e.KeyChar) Then
                MsgBox("Only Letters & Digits Allowed!!", MsgBoxStyle.Information, "Verify")
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub txtYear_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtYear.KeyPress
        If Asc(e.KeyChar) = 13 Then
            txtTitle.Focus()
            e.Handled = True
            Exit Sub
        ElseIf e.KeyChar = "-" Then
            txtYear.Clear()
            e.Handled = True
            Exit Sub
        End If
        If (Char.IsControl(e.KeyChar) = False) Then
            If Not Char.IsDigit(e.KeyChar) Then
                MsgBox("Only Digits Allowed!!", MsgBoxStyle.Information, "Verify")
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub txtTitle_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtTitle.KeyPress
        If Asc(e.KeyChar) = Keys.Space Then
            If txtTitle.SelectionStart = 0 Then Exit Sub
            If Not txtTitle.Text.Trim.StartsWith("COMMUNICATION") Then
                If txtTitle.Text.Chars(txtTitle.SelectionStart - 1) = " " Then
                    txtTitle.SelectionStart = txtTitle.SelectionStart - 1
                End If
            End If
        End If
            If Asc(e.KeyChar) = Keys.Enter Then
            cmdNext.Focus()
            cmdNext.PerformClick()
            e.Handled = True
            Exit Sub
        ElseIf e.KeyChar = "-" Then
            txtTitle.Clear()
            e.Handled = True
            Exit Sub
        ElseIf Asc(e.KeyChar) = Keys.Tab Then
            txtArtn1.Focus()
            e.Handled = True
            Exit Sub
        End If

        If (Char.IsControl(e.KeyChar) = False) Then
            If e.KeyChar = "*" Then
            ElseIf Char.IsPunctuation(e.KeyChar) Then
                MsgBox("Punctuation not Allowed!!", MsgBoxStyle.Information, "Verify")
                e.Handled = True
            ElseIf (Char.GetUnicodeCategory(e.KeyChar) = Globalization.UnicodeCategory.MathSymbol) Then
                MsgBox("Symbol not Allowed!!", MsgBoxStyle.Information, "Verify")
                e.Handled = True
            End If
        End If

    End Sub

    Private Sub cmdAddArtn_Click(sender As Object, e As EventArgs) Handles cmdAddArtn.Click
        Dim i As Integer
        For i = 2 To 4
            If Me.Controls.Find("txtArtn" & i, True)(0).Visible = False Then
                Me.Controls.Find("txtArtn" & i, True)(0).Visible = True
                Me.Controls.Find("cmbArtn" & i, True)(0).Visible = True
                If i = 4 Then cmdAddArtn.Visible = False
                If cmdDelArtn.Visible = False Then cmdDelArtn.Visible = True
                subResizePB() '("Add")
                'pbImage.Height = pbImage.Height - 30
                'pbImage.Location = New Point(pbImage.Location.X, Me.Controls.Find("cmbArtn" & i, True)(0).Location.Y + 30)
                Exit For
            End If
        Next
    End Sub

    Private Sub cmdDelArtn_Click(sender As Object, e As EventArgs) Handles cmdDelArtn.Click
        Dim i As Integer
        For i = 4 To 2 Step -1
            If Me.Controls.Find("txtArtn" & i, True)(0).Visible = True Then
                Me.Controls.Find("txtArtn" & i, True)(0).Visible = False
                Me.Controls.Find("cmbArtn" & i, True)(0).Visible = False
                If cmdAddArtn.Visible = False Then cmdAddArtn.Visible = True
                If i = 2 Then cmdDelArtn.Visible = False
                subResizePB() '("Del")
                'pbImage.Height = pbImage.Height + 30
                'pbImage.Location = New Point(pbImage.Location.X, Me.Controls.Find("cmbArtn" & i, True)(0).Location.Y)
                Exit For
            End If
        Next
    End Sub

    Private Sub strFindString(strOCR As String)
        Dim bFound As Boolean = False, TextLine As String = ""

        If System.IO.File.Exists(policyFile) = True Then
            Dim objReader As New IO.StreamReader(policyFile)
            lblBlink.Text = ""
            lblBlink.BackColor = Color.WhiteSmoke
            Do While (objReader.Peek() <> -1 And bFound = False)
                TextLine = objReader.ReadLine()
                If InStr(strOCR, " " & Trim(TextLine) & " ", CompareMethod.Text) Then
                    lblBlink.Text = StrConv(Trim(TextLine), VbStrConv.Uppercase)
                    lblBlink.BackColor = Color.Red
                    Flash()
                    bFound = True
                    Exit Sub
                End If
            Loop
        End If
    End Sub

    Private Async Sub Flash()
        While True
            Await Task.Delay(1000)
            lblBlink.Visible = Not lblBlink.Visible
        End While
    End Sub

    Private Sub cmdDone_Click(sender As Object, e As EventArgs) Handles cmdDone.Click
        Log_Done("===========Done started==============")
        Log_Done("CheckDOI started")
        If bCheckDOI() = False Then Exit Sub
        Log_Done("CheckDOI completed")
        Log_Done("CheckDeleted started")
        CheckDeleted()
        Log_Done("CheckDeleted completed")
        If xnlCitations Is Nothing Then Exit Sub
        Log_Done("Author type started")
        If txtAuthor.Text.StartsWith("&") Then
            bIsPatent = True
        Else
            bIsPatent = False
        End If
        Log_Done("Author type completed")
        Log_Done("Saving started")
        If chkNTR.Checked Then
            SaveNTR()
        ElseIf chkDCI.Checked Then
            SaveDCI()
        ElseIf chkTitErr.Checked Then
            SaveTitleError()
        ElseIf bIsPatent Then
            SavePatent()
        Else
            subSaveRef()
        End If
        Log_Done("Saving completed")

        If FindEmptyTitle() = True Then
            Exit Sub
        End If
        If Not AllValid() Then
            Exit Sub
        End If
        If MsgBox("Do you want to done this item?", MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then

            Log_Done("Initiating done")
            If strInFile Is String.Empty Then
                MsgBox("Invalid input file. Could not save")
                Exit Sub
            End If

            RemoveEmptyNodes()
            RemoveBlankLines(xmlDoc.InnerXml)

            If strInFile.Contains(strPriorPath) And (FormMode.chosenTool = ToolMode.OP) Then
                xmlDoc.Save(strInFile.Replace("Priority\", "QC\Priority\"))
            Else
                xmlDoc.Save(strOutPath & System.IO.Path.GetFileName(strInFile))
            End If
            Log_Done("Saving output file done")

            On Error Resume Next
            My.Computer.FileSystem.DeleteFile(strTempPath & System.IO.Path.GetFileName(strInFile))
            My.Computer.FileSystem.DeleteFile(strCurPath & System.IO.Path.GetFileName(strInFile))
            Log_Done("Moving comp file done")
            Log_Done("Getfiles started")
            For Each file As String In My.Computer.FileSystem.GetFiles(strInPath)
                If file.Contains(IO.Path.GetFileNameWithoutExtension(strInFile)) Then
                    My.Computer.FileSystem.MoveFile(file, strOutPath & System.IO.Path.GetFileName(file), True)
                End If
            Next
            Log_Done("Getfiles finished")
            iCompItems = iCompItems + 1
            iCompRefs = iCompRefs + xnlCitations.Count
            swCurProd.Stop()
            swCurSusp.Stop()

            Dim TotRefs As Integer = xnlCitations.Count
            Dim DelRefs As Integer = xnlCitations(0).ParentNode.SelectNodes("CI_CITATION[@D='Y']").Count
            Dim InsRefs As Integer = xnlCitations(0).ParentNode.SelectNodes("CI_CITATION[@I='Y']").Count
            Log_Done("Logging started")
            LogEntry.References_Close = TotRefs + InsRefs - DelRefs
            LogEntry.References_Deleted = DelRefs
            LogEntry.References_Inserted = InsRefs
            LogEntry.Session_Production = swCurProd.Elapsed.ToString("hh\:mm\:ss")
            LogEntry.Session_Suspend = swCurSusp.Elapsed.ToString("hh\:mm\:ss")
            LogEntry.Session_End_Date = Format(Today, "dd-MM-yyyy")
            LogEntry.Session_End_Time = Now.ToShortTimeString
            LogEntry.Status = "CO"
            'MakeLogEntry(strLogPath & frmLogin.txtUserName.Text & ".XML")
            UpdateLogEntry(strLogPath & frmLogin.txtUserName.Text & ".XML", LogEntry)
            Log_Done("Logging finished")
            On Error GoTo 0
            lblCompItems.Text = iCompItems
            lblCompRefs.Text = iCompRefs
            subClearFields()
            Log_Done("Fields cleared")
            pbSource.Image = Nothing
            lblCurrent.Text = " of "
            xnlCitations = Nothing
            'System.IO.File.Move(strCurPath & System.IO.Path.GetFileName(strInFile), strCompPath & System.IO.Path.GetFileName(strInFile))

            If MsgBox("Do you want to load new item?", MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
                Log_Done("Getting new file")
                strInFile = fnGetXMLFile(strPriorPath)

                If strInFile = "" Then
                    bIsPrior = False
                    strInFile = fnGetXMLFile(strInPath)
                End If
                If strInFile = "" Then
                    MsgBox("No files for input", MsgBoxStyle.Information)
                Else
                    bIsPrior = False
                    Log_Done("Loading new file")
                    fnLoadFile(strInFile)
                    Log_Done("New file loaded")
                End If
            Else
                strInFile = ""
                lvlist.Items.Clear()
            End If
        End If
        Log_Done("Done finished")
    End Sub

    Private Function bCheckDOI() As Boolean
        Dim strARTN As String = String.Empty, bValid As Boolean = False
        Dim regex = New Regex("^(10[.][0-9]{4,5}/(\S+))\b")
        'Dim regex = New Regex("\b(10[.][0-9]{4,}/(\S+))\b")
        For i As Integer = 1 To 4
            If (Me.Controls.Find("txtArtn" & i, True)(0).Visible = True) And (Me.Controls.Find("cmbArtn" & i, True)(0).Text = "DOI") Then
                strARTN = Me.Controls.Find("txtArtn" & i, True)(0).Text
                If Not regex.Match(strARTN).Success Then
                    MsgBox("Invalid DOI format")
                    Return False
                    Exit Function
                End If
            End If
        Next
        Return True
    End Function

    Private Sub CheckDeleted()              ' This will remove FULL_CI_INFO tag from deleted nodes
        If xnlCitations Is Nothing Then Exit Sub
        For Each nod As XmlNode In xnlCitations
            If nod.Attributes("D") IsNot Nothing Then
                If nod.Attributes("D").Value = "Y" Then
                    Dim nodFCI As XmlNode
                    nodFCI = nod.SelectSingleNode("FULL_CI_INFO")
                    If nodFCI IsNot Nothing Then
                        nodFCI.ParentNode.RemoveChild(nodFCI)
                    End If
                End If
            End If
        Next
    End Sub

    Private Sub disableFields()
        txtAuthor.Enabled = False
        txtVolume.Enabled = False
        txtPage.Enabled = False
        txtYear.Enabled = False
        txtTitle.Enabled = False
    End Sub

    Private Sub enableFields()
        txtAuthor.Enabled = True
        txtVolume.Enabled = True
        txtPage.Enabled = True
        txtYear.Enabled = True
        txtTitle.Enabled = True
    End Sub

    Private Sub txtTitle_Leave(sender As Object, e As EventArgs) Handles txtTitle.Leave
        txtTitle.Text = txtTitle.Text.Trim
        If txtTitle.Text.StartsWith("COMMUNICATION") Then
            If Not Regex.Match(txtTitle.Text, "^(COMMUNICATION)\s{3}\d{4}$").Success Then
                tsslStatus.Text = "COMMUNICATION should be followed by 3 spaces and then by 4 digits (20 Chars)"
                txtTitle.BackColor = Color.Pink
            Else
                tsslStatus.Text = "COMMUNICATION Title valid"
                txtTitle.BackColor = Color.White
            End If
        End If
        On Error GoTo addtitle
        If xnlCitations Is Nothing Then Exit Sub
        If iNo = xnlCitations.Count Then Exit Sub
        'txtTitle.Text = bumpSpace(txtTitle.Text)
        If lvlist.Items.Count > iNo Then lvlist.Items(curRef).SubItems(5).Text = txtTitle.Text
        On Error GoTo 0
        Exit Sub
addtitle:
        For i As Integer = lvlist.Items(curRef).SubItems.Count To 6
            lvlist.Items(curRef).SubItems.Add("")
        Next
        lvlist.Items(curRef).SubItems(5).Text = txtTitle.Text
        Resume Next
    End Sub

    Private Sub txtTitle_TextChanged(sender As Object, e As EventArgs) Handles txtTitle.TextChanged
        Dim selst As Integer = txtTitle.SelectionStart
        txtTitle.Text = Regex.Replace(txtTitle.Text, "[^a-zA-Z0-9* ]+", String.Empty)
        If Not txtTitle.Text.Trim.StartsWith("COMMUNICATION") Then
            txtTitle.Text = bumpSpace(Regex.Replace(txtTitle.Text, "[^a-zA-Z0-9* ]+", String.Empty))
        Else
            txtTitle.Text = Regex.Replace(txtTitle.Text, "[^a-zA-Z0-9* ]+", String.Empty)
        End If
        txtTitle.SelectionStart = selst
        lblTitChars.Text = txtTitle.TextLength
    End Sub

    Private Sub txtAuthor_Leave(sender As Object, e As EventArgs) Handles txtAuthor.Leave
        If xnlCitations Is Nothing Then Exit Sub
        If iNo = xnlCitations.Count Then Exit Sub
        If txtAuthor.Text.Trim = String.Empty Then Exit Sub
        Dim tname() As String
        txtAuthor.Text = txtAuthor.Text.Trim
        If (Not txtAuthor.Text.StartsWith("&")) And (Not txtAuthor.Text.StartsWith("*")) Then
            If Regex.Match(txtAuthor.Text, "\d").Success Then
                MsgBox("Digits not allowed here", MsgBoxStyle.Information, "Verify")
                txtAuthor.Focus()
                Exit Sub
            End If
        End If
        If txtAuthor.Text.StartsWith("*") Then
            If txtAuthor.Text.Contains(".") Then
                MsgBox("Dot not allowed here", MsgBoxStyle.Information, "Verify")
                txtAuthor.Focus()
                Exit Sub
            Else
                If IsNumeric(txtAuthor.Text.Split(" ")(0).Replace("*", "")) Then
                    If txtAuthor.Text.Split(" ")(0).Replace("*", "").Length > 3 Then
                        MsgBox("Patent should start with &")
                        txtAuthor.Focus()
                        Exit Sub
                    End If
                End If
            End If
        ElseIf txtAuthor.Text.StartsWith("&") Then
            If Not IsNumeric(txtAuthor.Text.Split(" ")(0).Replace("&", "")) Then
                MsgBox("Patent should contain Numeric value only")
                txtAuthor.Focus()
                Exit Sub
            End If
            If txtAuthor.Text.Split(" ")(0).Replace("&", "").Length < 3 Then
                MsgBox("Patent number should be atleast 3 digits")
                txtAuthor.Focus()
                Exit Sub
            End If
        Else
            If Not Regex.Match(txtAuthor.Text, "^[a-zA-Z .]+$").Success Then
                MsgBox("Only Alphabets allowed", MsgBoxStyle.Information, "Verify")
                txtAuthor.Focus()
                Exit Sub
            End If
            If txtAuthor.Text.Length > 16 Then
                If txtAuthor.Text.Contains(".") Then
                    tname = Split(txtAuthor.Text, ".")
                    If tname(0).Length <> 15 Then
                        MsgBox("Name should be 15 chars", MsgBoxStyle.Information, "Verify")
                        txtAuthor.Focus()
                        Exit Sub
                    End If
                    If (tname.Length <> 1) And (tname.Length <> 2) Then
                        MsgBox("Only 1 DOT allowed", MsgBoxStyle.Information, "Verify")
                        txtAuthor.Focus()
                        Exit Sub
                    End If
                Else
                    If Not txtAuthor.Text.Trim.Substring(0, 16).Contains(" ") Then
                        MsgBox("Space or DOT expected", MsgBoxStyle.Information, "Verify")
                        txtAuthor.Focus()
                        Exit Sub
                    End If
                End If
            End If
            'If txtAuthor.Text.Trim.Contains(" ") Then
            '    tname = Split(txtAuthor.Text, " ")
            '    If tname.Last.Length > 3 Then
            '        MsgBox("Givenname must have atmost 3 chars", MsgBoxStyle.Information, "Verify")
            '        txtAuthor.Focus()
            '        Exit Sub
            '    End If
            'End If
        End If
        txtAuthor.Text = bumpSpace(txtAuthor.Text)

        On Error GoTo addtitle
        If lvlist.Items.Count > curRef Then lvlist.Items(curRef).SubItems(1).Text = txtAuthor.Text
        On Error GoTo 0
        Exit Sub
addtitle:
        For i As Integer = lvlist.Items(iNo).SubItems.Count To 2
            lvlist.Items(curRef).SubItems.Add("")
        Next
        lvlist.Items(curRef).SubItems(1).Text = txtAuthor.Text
        Resume Next
    End Sub

    Private Sub txtVolume_Leave(sender As Object, e As EventArgs) Handles txtVolume.Leave
        If xnlCitations Is Nothing Then Exit Sub
        If iNo = xnlCitations.Count Then Exit Sub
        If IsNumeric(txtVolume.Text) Then
            txtVolume.Text = CInt(txtVolume.Text)
        Else
            If Regex.IsMatch(txtVolume.Text, "\d") Then
                MsgBox("Volume must either be Alpha or numeric", MsgBoxStyle.Information, "Verify")
                txtVolume.Focus()
            End If
        End If
        txtVolume.Text = bumpSpace(txtVolume.Text)
        On Error GoTo addvol
        If lvlist.Items.Count > curRef Then lvlist.Items(curRef).SubItems(2).Text = txtVolume.Text
        On Error GoTo 0
        Exit Sub
addvol:
        For i As Integer = lvlist.Items(curRef).SubItems.Count To 3
            lvlist.Items(curRef).SubItems.Add("")
        Next
        lvlist.Items(curRef).SubItems(2).Text = txtVolume.Text
        Resume Next
    End Sub

    Private Sub txtYear_Leave(sender As Object, e As EventArgs) Handles txtYear.Leave
        txtYear.Text = txtYear.Text.Trim
        If txtYear.Text = String.Empty Then Exit Sub
        If xnlCitations Is Nothing Then Exit Sub
        If iNo = xnlCitations.Count Then Exit Sub
        If Not IsNumeric(txtYear.Text) Then
            MsgBox("Year must be numeric", MsgBoxStyle.Information, "Verify")
            txtYear.Focus()
            Exit Sub
        End If

        Dim CurYear As Integer = DateAndTime.Now.Year
        If txtYear.Text.Length > 4 Then
            MsgBox("Year must have atmost 4 digits", MsgBoxStyle.Information, "Verify")
            txtYear.Focus()
        ElseIf txtYear.Text.Length <> 4 Then
            If CInt(txtYear.Text) + 2000 <= CurYear Then
                txtYear.Text = CInt(txtYear.Text) + 2000
            ElseIf CInt(txtYear.Text) + 1900 <= CurYear Then
                txtYear.Text = CInt(txtYear.Text) + 1900
            Else
                MsgBox("Invalid Year", MsgBoxStyle.Information, "Verify")
                txtYear.Focus()
            End If
        Else
            If (CInt(txtYear.Text) < 1000) Or (CInt(txtYear.Text) > CurYear) Then
                MsgBox("Invalid Year", MsgBoxStyle.Information, "Verify")
                txtYear.Focus()
            End If
        End If

        'If CInt(txtYear.Text) <= 20 Then
        '    txtYear.Text = CInt(txtYear.Text) + 2000
        'ElseIf txtYear.Text.Length <= 2 Then
        '    txtYear.Text = CInt(txtYear.Text) + 1900
        'ElseIf txtYear.Text.Length <> 4 Then
        '    MsgBox("Year must have 1,2 or 4 digits", MsgBoxStyle.Information, "Verify")
        '    txtYear.Focus()
        'End If

        On Error GoTo addYear
        If lvlist.Items.Count > curRef Then lvlist.Items(curRef).SubItems(4).Text = txtYear.Text
        On Error GoTo 0
        Exit Sub
addYear:
        For i As Integer = lvlist.Items(curRef).SubItems.Count To 5
            lvlist.Items(curRef).SubItems.Add("")
        Next
        lvlist.Items(curRef).SubItems(4).Text = txtYear.Text
        Resume Next
    End Sub

    Private Sub txtPage_Leave(sender As Object, e As EventArgs) Handles txtPage.Leave
        txtPage.Text = txtPage.Text.Trim
        If txtPage.Text = String.Empty Then Exit Sub
        If xnlCitations Is Nothing Then Exit Sub
        If iNo = xnlCitations.Count Then Exit Sub
        Dim spaces As String, pText As String, NewStr As String
        pText = Split(txtPage.Text, "-")(0).Trim
        NewStr = pText
        'If pText.Length = 5 Then Exit Sub
        If pText.Length > 5 Then
            MsgBox("page must have atmost 5 chars", MsgBoxStyle.Information, "Verify")
            txtPage.Focus()
            Exit Sub
        End If
        If Not IsNumeric(NewStr) Then
            If Not Regex.Match(NewStr, "^[a-zA-Z0-9 ]+$").Success Then
                MsgBox("page must be alphanumeric", MsgBoxStyle.Information, "Verify")
                txtPage.Focus()
                Exit Sub
            End If
            Dim PI As Integer = 0, PS As String = ""
            For Each ch As Char In pText
                If IsNumeric(ch) Then
                    PI = PI * 10 + CInt(ch.ToString)
                Else
                    PS = PS & ch
                End If
            Next
            If (PI = 0) And (Not txtAuthor.Text.StartsWith("&")) Then
                MsgBox("Page number should not be zero", MsgBoxStyle.Information, "Verify")
                txtPage.Focus()
                Exit Sub
            End If
            If PI = 0 Then
                NewStr = PS
            Else
                NewStr = PS & PI.ToString.PadLeft(5 - PS.Length)
            End If

            'spaces = Space(5 - pText.Length)
            'For iCnt = 1 To pText.Length
            '    If Char.IsDigit((Mid(pText, iCnt, 1))) Then
            '        NewStr = Mid(pText, 1, iCnt - 1) & spaces & Mid(pText, iCnt, pText.Length - iCnt + 1)
            '        Exit For
            '    End If
            'Next
            txtPage.Text = Trim(NewStr)
        Else
            If CInt(NewStr) = 0 Then
                MsgBox("Page number should not be zero", MsgBoxStyle.Information, "Verify")
                txtPage.Focus()
                Exit Sub
            End If
            txtPage.Text = CInt(NewStr)
        End If

        On Error GoTo addtitle
        If lvlist.Items.Count > curRef Then lvlist.Items(curRef).SubItems(3).Text = txtPage.Text
        On Error GoTo 0
        Exit Sub
addtitle:
        For i As Integer = lvlist.Items(curRef).SubItems.Count To 4
            lvlist.Items(curRef).SubItems.Add("")
        Next
        lvlist.Items(curRef).SubItems(3).Text = txtPage.Text
        Resume Next
    End Sub

    Private Sub txtAuthor_TextChanged(sender As Object, e As EventArgs) Handles txtAuthor.TextChanged
        Dim selst As Integer = txtAuthor.SelectionStart

        txtAuthor.Text = bumpSpace(Regex.Replace(txtAuthor.Text, "[^a-zA-Z0-9.&* ]+", String.Empty))
        txtAuthor.SelectionStart = selst
        lblAuthorChars.Text = txtAuthor.Text.Length
        If txtAuthor.Text.Length >= 15 Then
            txtAuthor.BackColor = Color.Aqua
        Else
            txtAuthor.BackColor = Color.White
        End If
        If Not txtAuthor.Text.Trim.StartsWith("*") Then
            If txtAuthor.Text.Trim.Length - txtAuthor.Text.Trim.Replace(" ", String.Empty).Length > 1 Then
                txtAuthor.BackColor = Color.Red
            End If
        End If
        If txtAuthor.Text.Trim.Contains(".") Then
            If Split(txtAuthor.Text.Trim, ".").First.Length <> 15 Then
                MsgBox("Author name must contain 15 chars before DOT")
                txtAuthor.Text = txtAuthor.Text.Replace(".", "").Trim
            End If
        End If
        If txtAuthor.Text.StartsWith("&") Then
            lblAuthor.Text = "ID : "
            lblTitle.Text = "Assignee : "
        Else
            lblAuthor.Text = "Author : "
            lblTitle.Text = "Full Title : "
        End If
    End Sub

    Private Sub txtPage_TextChanged(sender As Object, e As EventArgs) Handles txtPage.TextChanged
        'txtPage.Text = Split(txtPage.Text, "-")(0)
        txtPage.Text = Regex.Replace(txtPage.Text, "[^a-zA-Z0-9 ]+", String.Empty)
    End Sub
    Private Sub SubClearFields()
        ClearCIInfoFields()
        pbImage.ImageLocation = Nothing
        'pbSource.ImageLocation = Nothing
        txtOCR.Clear()
        Me.Controls.Find("txtArtn1", True)(0).Text = String.Empty
        Me.Controls.Find("cmbArtn1", True)(0).Text = String.Empty
        For i As Integer = 2 To 4
            Me.Controls.Find("txtArtn" & i, True)(0).Text = String.Empty
            Me.Controls.Find("cmbArtn" & i, True)(0).Text = String.Empty
            Me.Controls.Find("txtArtn" & i, True)(0).Visible = False
            Me.Controls.Find("cmbArtn" & i, True)(0).Visible = False
        Next
        chkNTR.Checked = False
        chkTitErr.Checked = False
        chkDCI.Checked = False
        For i As Integer = 1 To 5
            CType(Me.Controls.Find("cbField" & i, True)(0), CheckBox).Checked = False
        Next
        'lvlist.Items.Clear()
    End Sub

    Private Sub ClearCIInfoFields()
        txtAuthor.Clear()
        txtVolume.Clear()
        txtPage.Clear()
        txtYear.Clear()
        txtTitle.Clear()
    End Sub

    Private Sub fnLoadFields(cits As XmlNodeList, iNum As Integer)
        Dim nod As XmlNode, newBlurb As String
        If cits Is Nothing Then
            MsgBox("Nothing to load")
            Exit Sub
        End If

        If iNum >= cits.Count Then Exit Sub
        nod = cits(iNum).Attributes("D")
        If nod IsNot Nothing Then
            If nod.Value = "Y" Then disableFields()
        Else
            enableFields()
        End If

        lblRepName.Text = ""
        newBlurb = ""
        On Error Resume Next
        newBlurb = cits(iNum).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_BLURB").InnerText
        On Error GoTo 0
        If newBlurb <> "" Then DCI_Check(cits(iNum), newBlurb)
        MakeTree(cits(iNum), "CI_CAPTURE/CI_CAPTURE_BLURB", newBlurb)
        'cits(iNum).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_BLURB").InnerText = newBlurb

        nod = cits(iNum).SelectSingleNode("CI_INFO/CI_JOURNAL")
        If nod IsNot Nothing Then
            checkNTR(nod)
            checkDCI(nod)
            checkTitleError(nod)
            On Error Resume Next
            Dim tstr As String = StrConv((cits(iNum).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_AUTHOR").InnerText), VbStrConv.Uppercase)
            If tstr.Contains(".") Then
                Dim atstr() As String = Split(tstr, ".")
                tstr = Regex.Replace(atstr(0), "[^A-Z0-9* ]+", String.Empty) & "." & Regex.Replace(atstr(1), "[^A-Z0-9* ]+", String.Empty)
            Else
                tstr = Regex.Replace(tstr, "[^A-Z0-9* ]+", String.Empty)
            End If
            txtAuthor.Text = tstr

            txtVolume.Text = Regex.Replace(StrConv(cits(iNum).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_VOLUME").InnerText, VbStrConv.Uppercase), "[^A-Z0-9 ]+", String.Empty)
            If IsNumeric(txtVolume.Text) Then
                txtVolume.Text = CInt(txtVolume.Text)
            End If

            txtPage.Text = Regex.Replace(StrConv(cits(iNum).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_PAGE").InnerText, VbStrConv.Uppercase), "[^A-Z0-9 ]+", String.Empty)
            If txtPage.Text.Contains("-") Then txtPage.Text = Split(txtPage.Text, "-")(0)
            If IsNumeric(txtPage.Text) Then
                txtPage.Text = CInt(txtPage.Text)
            End If

            txtYear.Text = StrConv(cits(iNum).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_YEAR").InnerText, VbStrConv.Uppercase)
            If IsNumeric(txtYear.Text) Then
                txtYear.Text = CInt(txtYear.Text)
            End If
            txtTitle.Text = StrConv(cits(iNum).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_TITLE").InnerText, VbStrConv.Uppercase)
            On Error GoTo 0
        Else
            nod = cits(iNum).SelectSingleNode("CI_INFO/CI_PATENT")
            If nod IsNot Nothing Then
                On Error Resume Next
                txtTitle.Text = Regex.Replace(StrConv((cits(iNum).SelectSingleNode("CI_INFO/CI_PATENT/PATENT_ASSIGNEE").InnerText), VbStrConv.Uppercase), "[^A-Z0-9 ]+", String.Empty)

                txtVolume.Text = Regex.Replace(StrConv(cits(iNum).SelectSingleNode("CI_INFO/CI_PATENT/PATENT_VOLUME").InnerText, VbStrConv.Uppercase), "[^A-Z0-9 ]+", String.Empty)
                If IsNumeric(txtVolume.Text) Then
                    txtVolume.Text = CInt(txtVolume.Text)
                End If

                txtPage.Text = Regex.Replace(StrConv(cits(iNum).SelectSingleNode("CI_INFO/CI_PATENT/PATENT_PAGE").InnerText, VbStrConv.Uppercase), "[^A-Z0-9 ]+", String.Empty)
                If txtPage.Text.Contains("-") Then txtPage.Text = Split(txtPage.Text, "-")(0)
                If IsNumeric(txtPage.Text) Then
                    txtPage.Text = CInt(txtPage.Text)
                End If

                txtYear.Text = StrConv(cits(iNum).SelectSingleNode("CI_INFO/CI_PATENT/PATENT_YEAR").InnerText, VbStrConv.Uppercase)
                If IsNumeric(txtYear.Text) Then
                    txtYear.Text = CInt(txtYear.Text)
                End If
                txtAuthor.Text = "&" & StrConv(cits(iNum).SelectSingleNode("CI_INFO/CI_PATENT").Attributes("ID").Value, VbStrConv.Uppercase)
                On Error GoTo 0
            End If
        End If

        Dim inClipName As String
        On Error Resume Next
        inClipName = xnlCitations(iNum).SelectSingleNode("CI_CAPTURE/CI_IMAGE_CLIP_NAME").InnerText
        On Error GoTo 0
        If inClipName = String.Empty Then
            MakeTree(xnlCitations(iNum), "CI_CAPTURE/CI_IMAGE_CLIP_NAME")
            inClipName = lblAccn.Text & CInt(xmlDoc.SelectSingleNode("//ITEM").Attributes("ITEMNO").Value).ToString("D3") & "C_" & xnlCitations(iNum).Attributes("seq").InnerText & ".TIF"
        Else
            inClipName = Replace(inClipName, Split(inClipName, "_").Last, xnlCitations(iNum).Attributes("seq").InnerText & ".TIF")
        End If

        xnlCitations(iNum).SelectSingleNode("CI_CAPTURE/CI_IMAGE_CLIP_NAME").InnerText = inClipName
        pbImage.ImageLocation = strInPath & Split(inClipName, "\").Last
        If My.Computer.FileSystem.FileExists(strInPath & Split(cits(iNum).SelectSingleNode("CI_CAPTURE/CI_IMAGE_CLIP_NAME").InnerText, "\").Last) Then
            pbSource.ImageLocation = strInPath & Split(cits(iNum).SelectSingleNode("CI_CAPTURE/CI_IMAGE_CLIP_NAME").InnerText, "\").Last
        End If
        txtOCR.Text = cits(iNum).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_BLURB").InnerText
        Dim strARTN As String
        For i As Integer = 1 To cits(iNum).SelectNodes("RI_CITATIONIDENTIFIER").Count
            Me.Controls.Find("txtArtn" & i, True)(0).Text = cits(iNum).SelectSingleNode("RI_CITATIONIDENTIFIER[@seq='" & i & "']").InnerText
            strARTN = cits(iNum).SelectSingleNode("RI_CITATIONIDENTIFIER[@seq='" & i & "']").Attributes("type").Value
            For Each itm In cmbArtn1.Items
                If itm.ToString = strARTN Then
                    Me.Controls.Find("cmbArtn" & i, True)(0).Text = strARTN
                    Exit For
                End If
            Next

            If Trim(Me.Controls.Find("txtArtn" & i, True)(0).Text) = String.Empty And (i <> 1) Then
                Me.Controls.Find("txtArtn" & i, True)(0).Visible = False
                Me.Controls.Find("cmbArtn" & i, True)(0).Visible = False
            Else
                Me.Controls.Find("txtArtn" & i, True)(0).Visible = True
                Me.Controls.Find("cmbArtn" & i, True)(0).Visible = True
            End If
        Next
        curRef = iNum
        subResizePB()
    End Sub

    Private Sub tmTimer_Tick(sender As Object, e As EventArgs) Handles tmTimer.Tick
        lblProdTime.Text = swProdTime.Elapsed.ToString("hh\:mm\:ss")
        lblSuspTime.Text = swSuspTime.Elapsed.ToString("hh\:mm\:ss")
    End Sub

    Private Sub cmdSuspend_Click(sender As Object, e As EventArgs) Handles cmdSuspend.Click
        Me.Enabled = False
        frmLogin.txtUserName.Text = lblUName.Text
        frmLogin.txtUserName.Enabled = False
        focusToBox()
        frmLogin.Show(Me)
    End Sub

    Private Sub ContentsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ContentsToolStripMenuItem.Click
        Help.ShowHelp(Me, strHelpPath & "ISIHELP.hlp.12-1-11.HLP")
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Application.Exit()
    End Sub

    Private Sub lvlist_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lvlist.SelectedIndexChanged
        If lvlist.SelectedIndices.Count Then
            iNo = lvlist.SelectedIndices(0)
            subViewRef(iNo)
        End If
    End Sub

    Private Sub subViewRef(iNum As Integer, Optional sBut As String = "")

        Dim i As Integer

        If xnlCitations Is Nothing Then Exit Sub
        If txtAuthor.Text.StartsWith("&") Then
            bIsPatent = True
        Else
            bIsPatent = False
        End If

        Dim cCit As XmlNode = xnlCitations(iNum)
        While cCit IsNot Nothing
            Dim stest As String = ""
            Dim itest As String = ""
            Try
                stest = cCit.Attributes("D").Value
                'itest = cCit.Attributes("I").Value
            Catch ex As Exception
            End Try
            Try
                itest = cCit.Attributes("I").Value
            Catch ex As Exception
            End Try
            If stest <> "Y" Then Exit While
            If itest = "Y" Then
                iNum = CInt(lblCurrent.Text.Split("of").First.Trim + 1)
            End If
            If sBut = "Prev" Then
                iNum = iNum - 1
            Else
                iNum = iNum + 1
            End If
            cCit = xnlCitations(iNum)
        End While

        If iNum <= xnlCitations.Count Then
            If chkNTR.Checked Then
                SaveNTR()
            ElseIf chkDCI.Checked Then
                SaveDCI()
            ElseIf chkTitErr.Checked Then
                SaveTitleError()
            ElseIf bIsPatent Then
                SavePatent()
            Else
                subSaveRef(iNo)   'curref
            End If
        End If
        If iNum < 0 Then
            MsgBox("No more reference to view")
            Exit Sub
        ElseIf iNum >= xnlCitations.Count Then
            MsgBox("No more reference to view")
            subClearFields()
            lblCurrent.Text = " " & xnlCitations.Count & " of " & xnlCitations.Count
            iNo = xnlCitations.Count
            Exit Sub
        End If

        iNo = iNum
        lblCurrent.Text = " " & iNum & " of " & xnlCitations.Count
        subClearFields()
        For i = 1 To 5
            CType(Me.Controls.Find("cbField" & i, True)(0), CheckBox).Checked = False
        Next
        fnLoadFields(xnlCitations, iNum)
        subResizePB() '("Add")
        strFindString(txtOCR.Text)
        txtAuthor.Focus()
        txtAuthor.SelectionStart = 0
        txtAuthor.SelectionLength = 0
        'chkNTR.CheckState = CheckState.Unchecked
        'checkNTR(xnlCitations(iNum))        
    End Sub

    Public Sub subSaveRef(Optional inum As Integer = -1)
        Dim nod As XmlNode = Nothing, stest As String = ""
        If inum = -1 Then inum = CInt(Split(lblCurrent.Text, "of").First.Trim)
        If xnlCitations(inum) Is Nothing Then Exit Sub
        If lblCurrent.Text.Split("of").First.Trim = lblCurrent.Text.Split("of").Last.Trim Then
            Exit Sub
        End If

        Try
            xnlCitations(inum).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_TITLE").InnerText = txtTitle.Text.Trim
        Catch ex As Exception
            MakeTree(xnlCitations(inum), "CI_CAPTURE/CI_CAPTURE_TITLE", "", InsertAfter:=xnlCitations(inum).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_BLURB"))
        End Try

        RemoveCIInfoChildren(xnlCitations(inum))
        MakeCIInfoJournal(xnlCitations(inum))
        'MakeCIInfoNode(xnlCitations(inum))
        '------------------------
        'nod = Nothing
        'nod = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_AUTHOR")
        'If nod IsNot Nothing Then
        '    nod.InnerText = txtAuthor.Text.Trim
        'Else
        '    If txtAuthor.Text.Trim <> "" Then
        '        MakeTree(xnlCitations(inum), "CI_INFO", "", InsertFirst:=True)
        '        MakeTree(xnlCitations(inum), "CI_INFO/CI_JOURNAL", "", InsertFirst:=True)
        '        MakeTree(xnlCitations(inum), "CI_INFO/CI_JOURNAL/CI_AUTHOR", txtAuthor.Text.Trim, InsertFirst:=True)
        '    End If
        'End If

        'nod = Nothing
        'nod = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_VOLUME")
        'If nod IsNot Nothing Then
        '    nod.InnerText = txtVolume.Text.Trim
        'Else
        '    If txtVolume.Text.Trim <> "" Then
        '        MakeTree(xnlCitations(inum), "CI_INFO", "", InsertFirst:=True)
        '        MakeTree(xnlCitations(inum), "CI_INFO/CI_JOURNAL", "", InsertFirst:=True)
        '        MakeTree(xnlCitations(inum), "CI_INFO/CI_JOURNAL/CI_VOLUME", txtVolume.Text.Trim, InsertAfter:=xnlCitations(inum).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_AUTHOR"))
        '    End If
        'End If

        'nod = Nothing
        'nod = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_PAGE")
        'If nod IsNot Nothing Then
        '    Try
        '        nod.InnerText = txtPage.Text  'ToString("D5")
        '    Catch ex As Exception
        '        Debug.Print(ex.Message)
        '    End Try
        'Else
        '    If txtPage.Text.Trim <> "" Then
        '        MakeTree(xnlCitations(inum), "CI_INFO", "", InsertFirst:=True)
        '        MakeTree(xnlCitations(inum), "CI_INFO/CI_JOURNAL", "", InsertFirst:=True)
        '        MakeTree(xnlCitations(inum), "CI_INFO/CI_JOURNAL/CI_PAGE", txtPage.Text, InsertAfter:=xnlCitations(inum).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_VOLUME"))
        '    End If
        'End If

        'nod = Nothing
        'nod = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_YEAR")
        'If nod IsNot Nothing Then
        '    nod.InnerText = txtYear.Text.Trim
        'Else
        '    If txtYear.Text.Trim <> "" Then
        '        MakeTree(xnlCitations(inum), "CI_INFO", "", InsertFirst:=True)
        '        MakeTree(xnlCitations(inum), "CI_INFO/CI_JOURNAL", "", InsertFirst:=True)
        '        MakeTree(xnlCitations(inum), "CI_INFO/CI_JOURNAL/CI_YEAR", txtYear.Text.Trim, InsertAfter:=xnlCitations(inum).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_PAGE"))
        '    End If
        'End If

        'nod = Nothing
        'Try
        '    nod = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_TITLE")
        '    'nod.ParentNode.RemoveChild(nod)
        '    nod.InnerText = ""
        '    Dim CData As XmlCDataSection
        '    CData = xmlDoc.CreateCDataSection("")
        '    nod.AppendChild(CData)
        'Catch ex As Exception
        '    Dim attrb As XmlAttribute = xnlCitations(inum).Attributes("D")
        '    If attrb IsNot Nothing Then
        '        MakeTree(xnlCitations(inum), "CI_INFO/CI_JOURNAL/CI_TITLE", "", InsertAfter:=xnlCitations(inum).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_YEAR"))
        '        Dim CData As XmlCDataSection
        '        CData = xmlDoc.CreateCDataSection("")
        '        nod = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_TITLE")
        '        nod.AppendChild(CData)
        '    End If
        'End Try
        '---------------------------------------------
        If Not ReviewARTN(xnlCitations(inum)) Then Exit Sub
        For i = 1 To 4
            If Me.Controls.Find("txtArtn" & i, True)(0).Visible = True Then
                If (Me.Controls.Find("txtArtn" & i, True)(0).Text.Trim = vbNullString) Xor (Me.Controls.Find("cmbArtn" & i, True)(0).Text = vbNullString) Then
                    MsgBox("Check Artn fields", MsgBoxStyle.Information, "Verify")
                    Exit Sub
                ElseIf (Me.Controls.Find("txtArtn" & i, True)(0).Text.Trim <> vbNullString) And (Me.Controls.Find("cmbArtn" & i, True)(0).Text <> vbNullString) Then
                    Dim nd As XmlNode = xnlCitations(inum).SelectSingleNode("RI_CITATIONIDENTIFIER[@seq='" & i & "']")
                    If nd Is Nothing Then
                        'makeTreeAttr(xnlCitations(inum), "RI_CITATIONIDENTIFIER", "seq", "type", Trim(Me.Controls.Find("txtArtn" & i, True)(0).Text), i, Me.Controls.Find("cmbArtn" & i, True)(0).Text)
                        MakeTreeAttr(xnlCitations(inum), "RI_CITATIONIDENTIFIER", "seq", "type", "", i, Me.Controls.Find("cmbArtn" & i, True)(0).Text, xnlCitations(inum).SelectSingleNode("CI_INFO"))
                    Else
                        xnlCitations(inum).SelectSingleNode("RI_CITATIONIDENTIFIER[@seq='" & i & "']").Attributes("type").Value = Me.Controls.Find("cmbArtn" & i, True)(0).Text
                        'xnlCitations(inum).SelectSingleNode("RI_CITATIONIDENTIFIER[@seq='" & i & "']").InnerText =  Me.Controls.Find("txtArtn" & i, True)(0).Text.Trim
                    End If
                    nd = xnlCitations(inum).SelectSingleNode("RI_CITATIONIDENTIFIER[@seq='" & i & "']")
                    Dim cd As XmlCDataSection, val As String
                    val = Me.Controls.Find("txtArtn" & i, True)(0).Text.Trim
                    If val = "" Then val = " "
                    cd = xmlDoc.CreateCDataSection(val)
                    nd.InnerText = ""
                    nd.AppendChild(cd)
                Else
                    Dim nd As XmlNode = xnlCitations(inum).SelectSingleNode("RI_CITATIONIDENTIFIER[@seq='" & i & "']")
                    Try
                        nd.ParentNode.RemoveChild(nd)
                    Catch ex As Exception
                    End Try
                End If
            Else
                Try
                    Dim tnod As XmlNode
                    tnod = xnlCitations(inum).SelectSingleNode("RI_CITATIONIDENTIFIER[@seq='" & i & "']")
                    tnod.ParentNode.RemoveChild(tnod)
                Catch ex As Exception
                End Try
            End If
        Next
        Dim inClipName As String = String.Empty
        Try
            inClipName = xnlCitations(inum).SelectSingleNode("CI_CAPTURE/CI_IMAGE_CLIP_NAME").InnerText
        Catch ex As Exception
        End Try
        If inClipName = String.Empty Then
            MakeTree(xnlCitations(inum), "CI_CAPTURE/CI_IMAGE_CLIP_NAME")
            inClipName = lblAccn.Text & CInt(xmlDoc.SelectSingleNode("//ITEM").Attributes("ITEMNO").Value).ToString("D3") & "_" & xnlCitations(inum).Attributes("seq").InnerText & ".TIF"
        Else
            inClipName = Replace(inClipName, Split(inClipName, "_").Last, xnlCitations(inum).Attributes("seq").InnerText & ".TIF")
        End If
        If Not inClipName.StartsWith(".\") Then inClipName = ".\" & inClipName
        xnlCitations(inum).SelectSingleNode("CI_CAPTURE/CI_IMAGE_CLIP_NAME").InnerText = inClipName

        nod = Nothing
        Try
            nod = xnlCitations(inum).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_BLURB")
            Dim CData As XmlCDataSection, val As String
            val = txtOCR.Text.Trim
            If val = "" Then val = " "
            CData = xmlDoc.CreateCDataSection(val)
            nod.InnerText = ""
            nod.AppendChild(CData)
            'nod.InnerText = txtOCR.Text
            'cits(inum).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_BLURB").InnerText()
        Catch ex As Exception
            MakeTree(xnlCitations(inum), "CI_CAPTURE/CI_CAPTURE_BLURB", "")
            nod = xnlCitations(inum).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_BLURB")
            'nod.InnerText = txtOCR.Text
            Dim CData As XmlCDataSection, val As String
            val = txtOCR.Text.Trim
            If val = "" Then val = " "
            CData = xmlDoc.CreateCDataSection(val)
            nod.InnerText = ""
            nod.AppendChild(CData)
        End Try
        nod = Nothing
        Try
            nod = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_PATENT")
            nod.ParentNode.RemoveChild(nod)
        Catch ex As Exception
        End Try
        Dim attr As XmlAttribute = xnlCitations(inum).Attributes("D")
        If attr IsNot Nothing Then
            Try
                xnlCitations(inum).RemoveChild(xnlCitations(inum).SelectSingleNode("FULL_CI_INFO"))
            Catch ex As Exception
            End Try
        End If
        'xmlDoc.Save(Split(xmlDoc.BaseURI, "///").Last)
        xmlDoc.Save(strTempPath & System.IO.Path.GetFileName(strInFile))
    End Sub

    Public Function fnGetXMLFile(strDir As String) As String
        Try
            'Dim fxml As String = System.IO.Directory.GetFiles(strDir, "*.XML", IO.SearchOption.TopDirectoryOnly)(0)
            Dim files() = New System.IO.DirectoryInfo(strDir).GetFiles("*.XML", IO.SearchOption.TopDirectoryOnly).OrderBy(Function(fi) fi.LastWriteTime).ToArray
            Dim fxml As String = files.First.ToString
            Dim ftmp As String = IO.Path.ChangeExtension(fxml, "tmp")
            My.Computer.FileSystem.RenameFile(strDir & fxml, ftmp)
            Return strDir & ftmp
        Catch e As Exception
            Return String.Empty
        End Try
    End Function

    Private Sub fnLoadFile(strFile As String)
        Log_Load("===========Load started==============")
        Log_Load("Input file " & strFile)
        iNo = 0
        'On Error Resume Next
        Log_Load("Moving " & strFile & " to " & strCurPath & System.IO.Path.GetFileNameWithoutExtension(strFile) & ".XML")

        If IO.File.Exists(strCurPath & System.IO.Path.GetFileNameWithoutExtension(strFile) & ".XML") Then
            FileSystem.Kill(strCurPath & System.IO.Path.GetFileNameWithoutExtension(strFile) & ".XML")
        End If
        My.Computer.FileSystem.MoveFile(strFile, strCurPath & System.IO.Path.GetFileNameWithoutExtension(strFile) & ".XML", True)

        strInFile = strInFile.Replace(".tmp", ".XML")
        strFile = strCurPath & System.IO.Path.GetFileNameWithoutExtension(strFile) & ".XML"
        Log_Load("Loading " & strFile)
        '--------------------
        On Error GoTo Error_Loadfile
        xmlDoc.Load(strFile)
        On Error GoTo 0
        '-----------------
        cbBackup.Items.Clear()
        frmImages.cbBackup.Items.Clear()
        swCurProd.Restart()
        swCurSusp.Reset()
        tsslStatus.Text = "Loaded file : " & strFile

        On Error Resume Next
        lblAccn.Text = xmlDoc.SelectSingleNode("//ID_ACCESSION").InnerText
        lblItem.Text = xmlDoc.SelectSingleNode("//ITEM").Attributes.GetNamedItem("ITEMNO").InnerText
        xnlCitations = xmlDoc.DocumentElement.SelectNodes("//CI_CITATION")
        lblTotalSeq.Text = xnlCitations.Count
        lblCurrent.Text = " 0 of " & xnlCitations.Count
        lvlist.Items.Clear()

        InitLog(LogEntry)
        LogEntry.Session_Start_Date = Format(Today, "dd-MM-yyyy")
        LogEntry.Session_Start_Time = Now.ToShortTimeString
        LogEntry.Item_Accession = lblAccn.Text
        LogEntry.Item_Itemno = lblItem.Text
        LogEntry.References_Open = lblTotalSeq.Text
        LogEntry.Status = "IP"
        MakeLogEntry(strLogPath & frmLogin.txtUserName.Text & ".XML")

        Dim nodej As XmlNode, nodep As XmlNode
        ReDim bNTR(0 To xnlCitations.Count - 1)
        'Try
        For i = 0 To xnlCitations.Count - 1
            bNTR(i) = False

            nodej = xnlCitations(i).SelectSingleNode("CI_INFO/CI_JOURNAL")
            nodep = xnlCitations(i).SelectSingleNode("CI_INFO/CI_PATENT")

            If (nodej Is Nothing) And (nodep Is Nothing) Then
                MakeTree(xnlCitations(i), "CI_INFO", InsertFirst:=True)
                MakeTree(xnlCitations(i), "CI_INFO/CI_JOURNAL", InsertFirst:=True)
                MakeTree(xnlCitations(i), "CI_INFO/CI_JOURNAL/CI_AUTHOR", InsertFirst:=True)
                MakeTree(xnlCitations(i), "CI_INFO/CI_JOURNAL/CI_VOLUME", InsertAfter:=xnlCitations(i).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_AUTHOR"))
                MakeTree(xnlCitations(i), "CI_INFO/CI_JOURNAL/CI_PAGE", InsertAfter:=xnlCitations(i).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_VOLUME"))
                MakeTree(xnlCitations(i), "CI_INFO/CI_JOURNAL/CI_YEAR", InsertAfter:=xnlCitations(i).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_PAGE"))
                MakeTree(xnlCitations(i), "CI_INFO/CI_JOURNAL/CI_TITLE", InsertAfter:=xnlCitations(i).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_YEAR"))
                MakeTree(xnlCitations(i), "CI_CAPTURE/CI_CAPTURE_TITLE")

                Dim auth As String = xnlCitations(i).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_AUTHOR").InnerText
                Dim vol As String = xnlCitations(i).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_VOLUME").InnerText
                Dim page As String = xnlCitations(i).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_PAGE").InnerText
                Dim year As String = xnlCitations(i).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_YEAR").InnerText
                Dim title As String = xnlCitations(i).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_TITLE").InnerText

                With lvlist.Items.Add(i + 1)
                    .SubItems.Add(auth)
                    .SubItems.Add(vol)
                    .SubItems.Add(page)
                    .SubItems.Add(year)
                    .SubItems.Add(title)
                End With
            Else
                If (nodep Is Nothing) Then
                    MakeTree(xnlCitations(i), "CI_INFO", InsertFirst:=True)
                    MakeTree(xnlCitations(i), "CI_INFO/CI_JOURNAL", InsertFirst:=True)
                    MakeTree(xnlCitations(i), "CI_INFO/CI_JOURNAL/CI_AUTHOR", InsertFirst:=True)
                    MakeTree(xnlCitations(i), "CI_INFO/CI_JOURNAL/CI_VOLUME", InsertAfter:=xnlCitations(i).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_AUTHOR"))
                    MakeTree(xnlCitations(i), "CI_INFO/CI_JOURNAL/CI_PAGE", InsertAfter:=xnlCitations(i).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_VOLUME"))
                    MakeTree(xnlCitations(i), "CI_INFO/CI_JOURNAL/CI_YEAR", InsertAfter:=xnlCitations(i).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_PAGE"))
                    MakeTree(xnlCitations(i), "CI_INFO/CI_JOURNAL/CI_TITLE", InsertAfter:=xnlCitations(i).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_YEAR"))
                    MakeTree(xnlCitations(i), "CI_CAPTURE/CI_CAPTURE_TITLE")

                    Dim auth As String = xnlCitations(i).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_AUTHOR").InnerText
                    Dim vol As String = xnlCitations(i).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_VOLUME").InnerText
                    Dim page As String = xnlCitations(i).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_PAGE").InnerText
                    Dim year As String = xnlCitations(i).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_YEAR").InnerText
                    Dim title As String = xnlCitations(i).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_TITLE").InnerText

                    With lvlist.Items.Add(i + 1)
                        .SubItems.Add(auth)
                        .SubItems.Add(xnlCitations(i).SelectSingleNode(vol).InnerText)
                        .SubItems.Add(xnlCitations(i).SelectSingleNode(page).InnerText)
                        .SubItems.Add(xnlCitations(i).SelectSingleNode(year).InnerText)
                        .SubItems.Add(xnlCitations(i).SelectSingleNode(title).InnerText)
                    End With
                Else
                    Dim auth As String = "&" & xnlCitations(i).SelectSingleNode("CI_INFO/CI_PATENT").Attributes("ID").Value
                    Dim vol As String = xnlCitations(i).SelectSingleNode("CI_INFO/CI_PATENT/PATENT_VOLUME").InnerText
                    Dim page As String = xnlCitations(i).SelectSingleNode("CI_INFO/CI_PATENT/PATENT_PAGE").InnerText
                    Dim year As String = xnlCitations(i).SelectSingleNode("CI_INFO/CI_PATENT/PATENT_YEAR").InnerText
                    Dim title As String = xnlCitations(i).SelectSingleNode("CI_INFO/CI_PATENT/PATENT_ASSIGNEE").InnerText

                    With lvlist.Items.Add(i + 1)
                        .SubItems.Add(auth)
                        .SubItems.Add(vol)
                        .SubItems.Add(page)
                        .SubItems.Add(year)
                        .SubItems.Add(title)
                    End With
                End If
            End If
        Next
        On Error GoTo 0

        cmdDelArtn.Visible = False
        Log_Load("Loading Fields")
        fnLoadFields(xnlCitations, iNo)
        Dim tempInPath As String

        'If bIsPrior Then
        '    tempInPath = strPriorPath
        'Else
        '    tempInPath = strInPath
        'End If
        tempInPath = strInPath

        For Each TIF As String In IO.Directory.GetFiles(tempInPath,
                                    System.IO.Path.GetFileNameWithoutExtension(strFile) & "_*.TIF", IO.SearchOption.TopDirectoryOnly)
            If FormMode.chosenTool = ToolMode.OP Then
                If Not TIF.Contains("BU") Then
                    If Not IO.File.Exists(Replace(TIF, ".TIF", "-BU.TIF", Compare:=CompareMethod.Text)) Then
                        IO.File.Copy(TIF, Replace(TIF, ".TIF", "-BU.TIF", Compare:=CompareMethod.Text))
                        cbBackup.Items.Add(IO.Path.GetFileNameWithoutExtension(TIF) & "-BU")
                    End If
                End If
                cbBackup.Items.Add(IO.Path.GetFileNameWithoutExtension(TIF))
            ElseIf FormMode.chosenTool = ToolMode.QC Then
                If TIF.Contains("BU") Then
                    cbBackup.Items.Add(Replace(IO.Path.GetFileNameWithoutExtension(TIF), "-BU", ""))
                End If
            End If
        Next


        lblSTime.Text = Now.ToShortTimeString
            strFindString(txtOCR.Text)
        txtAuthor.Focus()
        txtAuthor.SelectionStart = 0
        txtAuthor.SelectionLength = 0
        Log_Load("Load finished")
        Exit Sub
Error_Loadfile:
        MsgBox("Invalid XML File : " & IO.Path.GetFileName(strFile) & vbCrLf &
                   "Source files will be moved to Invalid\ folder" & vbCrLf & vbCrLf &
                   "More Details: " & Err.Description, MsgBoxStyle.Exclamation)
        On Error Resume Next
        IO.File.Move(strFile, Replace(strFile, strCurPath, strInvalidPath))
        On Error GoTo 0
        Dim FileLocation As IO.DirectoryInfo = New IO.DirectoryInfo(strInPath)
        Dim fi As IO.FileInfo() = FileLocation.GetFiles(IO.Path.GetFileNameWithoutExtension(strInFile) & "*.TIF")

        For Each file In fi
            If Not file.Name.Contains("BU") Then
                file.CopyTo(Replace(file.FullName, strInPath, strInvalidPath), True)
                On Error Resume Next
                file.Delete()
                On Error GoTo 0
            End If
        Next file
        If MsgBox("Do you want to load new item?", MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
            strInFile = fnGetXMLFile(strPriorPath)

            If strInFile = "" Then
                bIsPrior = False
                strInFile = fnGetXMLFile(strInPath)
            End If
            If strInFile = "" Then
                MsgBox("No files for input", MsgBoxStyle.Information)
            Else
                bIsPrior = False
                fnLoadFile(strInFile)
            End If
        End If
        Resume Next

    End Sub

    Private Sub cmdInter_Click(sender As Object, e As EventArgs) Handles cmdInter.Click
        If MsgBox("Do you want to interrupt this item?", MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
            xmlDoc.Save(strPriorPath & System.IO.Path.GetFileName(strInFile))
            subClearFields()
            Try
                My.Computer.FileSystem.DeleteFile(strCurPath & System.IO.Path.GetFileName(strInFile))
                My.Computer.FileSystem.DeleteFile(strTempPath & System.IO.Path.GetFileName(strInFile))
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub subResizePB()
        For i As Integer = 4 To 1 Step -1
            If Me.Controls.Find("txtArtn" & i, True)(0).Visible = True Then
                pnlDesImage.Location = New Point(pnlDesImage.Location.X, Me.Controls.Find("txtArtn" & i, True)(0).Location.Y + 560)
                pnlDesImage.Height = txtOCR.Top - pnlDesImage.Top - 10
                Exit For
            End If
        Next
    End Sub

    Private Function fnAllVisited() As Boolean
        For i As Integer = 1 To 5
            If Not CType(Me.Controls.Find("cbField" & i, True)(0), CheckBox).Checked Then
                Return False
            End If
        Next
        Return True
    End Function

    Private Sub cmdDelRef_Click(sender As Object, e As EventArgs) Handles cmdDelRef.Click
        If MsgBox("Do you want to Delete this reference?", MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
            DelRef(iNo)
        End If
    End Sub

    Private Sub cmdAddRef_Click(sender As Object, e As EventArgs) Handles cmdAddRef.Click
        If iNo = xnlCitations.Count Then
            subViewRef(iNo - 1)
        Else
            subSaveRef()
        End If

        If MsgBox("Do you want to Add reference?", MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
            AddRef()
            'iNo = xnlCitations.Count - 1
            Dim lvitem As New ListViewItem(xnlCitations.Count)
            'Try
            With lvitem
                Try
                    .SubItems.Add(xnlCitations(iNo).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_AUTHOR").InnerText)
                Catch ex As Exception
                End Try
                Try
                    .SubItems.Add(xnlCitations(iNo).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_VOLUME").InnerText)
                Catch ex As Exception
                End Try
                Try
                    .SubItems.Add(xnlCitations(iNo).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_PAGE").InnerText)
                Catch ex As Exception
                End Try
                Try
                    .SubItems.Add(xnlCitations(iNo).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_YEAR").InnerText)
                Catch ex As Exception
                End Try
                .SubItems.Add("")
                '.SubItems.Add(xnlCitations(iNo).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_TITLE").InnerText)
            End With
            'Catch ex As Exception
            'End Try
            'If xnlCitations.Count = iNo + 1 Then
            '    lvlist.Items.Add(lvitem)
            '    lvlist.Items(iNo).ForeColor = Color.White
            'Else
            '    lvlist.Items.Insert(iNo + 1, lvitem)
            '    lvlist.Items(iNo + 1).ForeColor = Color.White
            'End If
            lvlist.Items.Add(lvitem)
            lvlist.Items(lvlist.Items.Count - 1).ForeColor = Color.White

            'curRef = iNo + 1
            curRef = xnlCitations.Count - 1
            'lblCurrent.Text = CInt(lblCurrent.Text.Split("of").First.Trim) + 1 & " of " & xnlCitations.Count
            lblCurrent.Text = curRef & " of " & xnlCitations.Count
            txtTitle.Clear()
            txtAuthor.Focus()
            txtAuthor.SelectionStart = txtAuthor.Text.Length
            txtAuthor.SelectionLength = 0
            chkNTR.Checked = False
            chkTitErr.Checked = False
            subSaveRef(curRef)
            iNo = curRef
            'subViewRef(iNo)
        End If
    End Sub

    Private Sub DelRef(iNum As Integer)
        Dim xaAttr As XmlAttribute, attr As XmlAttribute
        If xnlCitations(iNum) Is Nothing Then Exit Sub
        attr = xnlCitations(iNum).Attributes("I")
        If attr Is Nothing Then
            xaAttr = xmlDoc.CreateAttribute("D")
            xaAttr.Value = "Y"
            xnlCitations(iNum).Attributes.Append(xaAttr)
            disableFields()
            lvlist.Items(iNum).ForeColor = Color.Silver
            subClearFields()
            Dim nod As XmlNode
            nod = xnlCitations(iNum).SelectSingleNode("CI_INFO/CI_JOURNAL")
            If nod Is Nothing Then
                nod = xnlCitations(iNum).SelectSingleNode("CI_INFO/CI_PATENT")
            End If
            If nod IsNot Nothing Then
                Try
                    nod.ParentNode.RemoveChild(nod)
                Catch ex As Exception
                    Debug.Print(ex.Message)
                End Try
            End If
            'Try
            '    If bIsPatent Then
            '        xnlCitations(iNum).SelectSingleNode("CI_INFO").RemoveChild(xnlCitations(iNum).SelectSingleNode("CI_INFO/CI_PATENT"))
            '    Else
            '        xnlCitations(iNum).SelectSingleNode("CI_INFO").RemoveChild(xnlCitations(iNum).SelectSingleNode("CI_INFO/CI_JOURNAL"))
            '    End If
            'Catch ex As Exception
            '    Debug.Print(ex.Message)
            'End Try

            'lvlist.Items(iNum).Remove()
            MsgBox("Successfully marked as Deleted")
            subSaveRef()
            'If chkNTR.Checked Then
            '    SaveNTR()
            'ElseIf chkDCI.Checked Then
            '    SaveDCI()
            'ElseIf chkTitErr.Checked Then
            '    SaveTitleError()
            'ElseIf bIsPatent Then
            '    SavePatent()
            'Else
            '    subSaveRef()
            'End If
            'If txtAuthor.Text.Trim.StartsWith("&") Then
            '    SavePatent()
            'Else
            '    subSaveRef()
            'End If

            lblCurrent.Text = CInt(lblCurrent.Text.Split("of").First.Trim) + 1 & " of " & xnlCitations.Count
            subViewRef(iNum + 1)
            iNo = iNum + 1
        Else
            xnlCitations(iNum).ParentNode.RemoveChild(xnlCitations(iNum))
            xnlCitations = xmlDoc.DocumentElement.SelectNodes("/ISSUE/ITEM/ITEM_CONTENT/CI_CITATION")
            'lvlist.Items(iNum).ForeColor = Color.Silver
            lvlist.Items(iNum).Remove()
            MsgBox("Added reference Successfully Deleted")
            lblCurrent.Text = CInt(lblCurrent.Text.Split("of").First.Trim) & " of " & xnlCitations.Count

            'subSaveRef()
            'subViewRef(iNum)
            subClearFields()
            fnLoadFields(xnlCitations, iNum)
            'iNo = iNum + 1
        End If

    End Sub

    'Private Sub AddRef()
    '    Dim cr As String = Environment.NewLine
    '    Dim newCitation As String
    '    Dim seqnum As Integer
    '    curRef = xnlCitations.Count - 1
    '    If iNo = xnlCitations.Count Then
    '        iNo = iNo - 1
    '    End If
    '    seqnum = xnlCitations(xnlCitations.Count - 1).Attributes("seq").Value + 1
    '    If seqnum < 10000 Then seqnum = seqnum + 10000
    '    Dim tnod As XmlNode
    '    tnod = xmlDoc.SelectSingleNode("//CI_CITATION[@seq='" & seqnum & "']")
    '    While tnod IsNot Nothing
    '        seqnum = seqnum + 1
    '        tnod = xmlDoc.SelectSingleNode("//CI_CITATION[@seq='" & seqnum & "']")
    '    End While
    '    newCitation = cr &
    '            "   <CI_CITATION seq='" & seqnum & "' I='Y'>" & cr &
    '            "       <CI_JOURNAL/>" & cr &
    '            "       <CI_INFO>" & cr &
    '            "           <CI_JOURNAL>" & cr &
    '            "               <CI_AUTHOR></CI_AUTHOR>" & cr &
    '            "               <CI_VOLUME></CI_VOLUME>" & cr &
    '            "               <CI_PAGE></CI_PAGE>" & cr &
    '            "               <CI_YEAR></CI_YEAR>" & cr &
    '            "               <CI_TITLE></CI_TITLE>" & cr &
    '            "           </CI_JOURNAL>" & cr &
    '            "       </CI_INFO>" & cr &
    '            "       <CI_CAPTURE>" & cr &
    '            "           <CI_CAPTURE_BLURB></CI_CAPTURE_BLURB>" & cr &
    '            "           <CI_CAPTURE_TITLE></CI_CAPTURE_TITLE>" & cr &
    '            "           <CI_CAPTURE_TITLE_CONF_IND></CI_CAPTURE_TITLE_CONF_IND>" & cr &
    '            "           <CI_IMAGE_CLIP_NAME>.\" & IO.Path.GetFileNameWithoutExtension(strInFile) & "_" & seqnum & ".TIF</CI_IMAGE_CLIP_NAME>" & cr &
    '            "       </CI_CAPTURE>" & cr &
    '            "   </CI_CITATION>"
    '    Try
    '        IO.File.Copy(strInFile.Replace(IO.Path.GetExtension(strInFile), "_" & xnlCitations(iNo).Attributes("seq").Value & ".TIF"), strInFile.Replace(IO.Path.GetExtension(strInFile), "_" & seqnum & ".TIF"))
    '    Catch ex As Exception
    '        MsgBox(ex.Message)
    '    End Try

    '    Dim docFrag As XmlDocumentFragment = xmlDoc.CreateDocumentFragment()
    '    docFrag.InnerXml = newCitation
    '    If xnlCitations(iNo) IsNot Nothing Then
    '        xnlCitations(iNo).ParentNode.InsertAfter(docFrag, xnlCitations(iNo))
    '    Else
    '        xnlCitations(0).ParentNode.InsertAfter(docFrag, xnlCitations(0).ParentNode.LastChild)
    '    End If
    '    xnlCitations = xmlDoc.DocumentElement.SelectNodes("/ISSUE/ITEM/ITEM_CONTENT/CI_CITATION")
    'End Sub

    Private Sub AddRef()
        Dim cr As String = Environment.NewLine
        Dim newCitation As String
        Dim seqnum As Integer
        curRef = xnlCitations.Count - 1
        If iNo = xnlCitations.Count Then
            iNo = iNo - 1
        End If
        seqnum = xnlCitations(xnlCitations.Count - 1).Attributes("seq").Value + 1
        If seqnum < 10000 Then seqnum = seqnum + 10000
        Dim tnod As XmlNode
        tnod = xmlDoc.SelectSingleNode("//CI_CITATION[@seq='" & seqnum & "']")
        While tnod IsNot Nothing
            seqnum = seqnum + 1
            tnod = xmlDoc.SelectSingleNode("//CI_CITATION[@seq='" & seqnum & "']")
        End While
        newCitation = cr &
                "   <CI_CITATION seq='" & seqnum & "' I='Y'>" & cr &
                "       <CI_INFO>" & cr &
                "           <CI_JOURNAL>" & cr &
                "               <CI_AUTHOR></CI_AUTHOR>" & cr &
                "               <CI_VOLUME></CI_VOLUME>" & cr &
                "               <CI_PAGE></CI_PAGE>" & cr &
                "               <CI_YEAR></CI_YEAR>" & cr &
                "               <CI_TITLE></CI_TITLE>" & cr &
                "           </CI_JOURNAL>" & cr &
                "       </CI_INFO>" & cr &
                "       <CI_CAPTURE>" & cr &
                "           <CI_CAPTURE_BLURB></CI_CAPTURE_BLURB>" & cr &
                "           <CI_CAPTURE_TITLE></CI_CAPTURE_TITLE>" & cr &
                "           <CI_IMAGE_CLIP_NAME>.\" & IO.Path.GetFileNameWithoutExtension(strInFile) & "_" & seqnum & ".TIF</CI_IMAGE_CLIP_NAME>" & cr &
                "       </CI_CAPTURE>" & cr &
                "   </CI_CITATION>"
        Try
            IO.File.Copy(strInFile.Replace(IO.Path.GetExtension(strInFile), "_" & xnlCitations(iNo).Attributes("seq").Value & ".TIF"), strInFile.Replace(IO.Path.GetExtension(strInFile), "_" & seqnum & ".TIF"))
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

        Dim docFrag As XmlDocumentFragment = xmlDoc.CreateDocumentFragment()
        docFrag.InnerXml = newCitation
        xnlCitations(0).ParentNode.AppendChild(docFrag)
        'If xnlCitations(iNo) IsNot Nothing Then
        '    xnlCitations(iNo).ParentNode.AppendChild(docFrag)
        'Else
        '    xnlCitations(0).ParentNode.AppendChild(docFrag)
        'End If
        xnlCitations = xmlDoc.DocumentElement.SelectNodes("/ISSUE/ITEM/ITEM_CONTENT/CI_CITATION")
    End Sub

    Private Sub AddRefToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AddRefToolStripMenuItem.Click
        cmdAddRef.PerformClick()
    End Sub

    Private Sub DeleteRefToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteRefToolStripMenuItem.Click
        cmdDelRef.PerformClick()
    End Sub

    Private Sub txtArtn2_GotFocus(sender As Object, e As EventArgs) Handles txtArtn2.GotFocus
        rbARTN2.Checked = True
    End Sub

    Private Sub txtArtn2_VisibleChanged(sender As Object, e As EventArgs) Handles txtArtn2.VisibleChanged
        cmdDelArtn.Visible = txtArtn2.Visible
        'If txtArtn2.Visible = False Then
        '    cmdDelArtn.Visible = False
        'Else
        '    cmdDelArtn.Visible = True
        'End If
    End Sub

    Private Sub SaveNTR()
        Dim ref As Integer
        ref = CInt(Split(lblCurrent.Text, "of").First.Trim)
        RemoveCIInfoChildren(xnlCitations(ref))
        ClearCIInfoFields()
        MakeCIInfoJournal(xnlCitations(ref))
        'MakeTree(xnlCitations(ref), "CI_INFO/CI_JOURNAL/CI_AUTHOR", "")
        'MakeTree(xnlCitations(ref), "CI_INFO/CI_JOURNAL/CI_VOLUME", "")
        'MakeTree(xnlCitations(ref), "CI_INFO/CI_JOURNAL/CI_PAGE", "")
        'MakeTree(xnlCitations(ref), "CI_INFO/CI_JOURNAL/CI_YEAR", "")
        'MakeTree(xnlCitations(ref), "CI_INFO/CI_JOURNAL/CI_TITLE", "")
        'Dim cdTitle As XmlCDataSection
        'cdTitle = xmlDoc.CreateCDataSection("**NON-TRADITIONAL**")
        'xnlCitations(ref).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_TITLE").AppendChild(cdTitle)
        Try
            xnlCitations(ref).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_TITLE").ChildNodes(0).Value = "**NON-TRADITIONAL**"
        Catch ex As Exception
            MakeTree(xnlCitations(ref), "CI_INFO/CI_JOURNAL/CI_TITLE", "<![CDATA[**NON-TRADITIONAL**]]>")
        End Try
        MakeTree(xnlCitations(ref), "CI_CAPTURE/CI_CAPTURE_TITLE", "NON TRADITIONAL REF")
        MakeTree(xnlCitations(ref), "CI_CAPTURE/CI_CAPTURE_BLURB", txtOCR.Text)
        xmlDoc.Save(strTempPath & System.IO.Path.GetFileName(strInFile))
    End Sub

    Private Sub SaveDCI()
        Dim ref As Integer
        ref = CInt(Split(lblCurrent.Text, "of").First.Trim)
        RemoveCIInfoChildren(xnlCitations(ref))
        ClearCIInfoFields()
        MakeCIInfoJournal(xnlCitations(ref))
        'MakeTree(xnlCitations(ref), "CI_INFO/CI_JOURNAL/CI_AUTHOR", "")
        'MakeTree(xnlCitations(ref), "CI_INFO/CI_JOURNAL/CI_VOLUME", "")
        'MakeTree(xnlCitations(ref), "CI_INFO/CI_JOURNAL/CI_PAGE", "")
        'MakeTree(xnlCitations(ref), "CI_INFO/CI_JOURNAL/CI_YEAR", "")
        'MakeTree(xnlCitations(ref), "CI_INFO/CI_JOURNAL/CI_TITLE", "")
        Dim cdTitle As XmlCDataSection
        'cdTitle = xmlDoc.CreateCDataSection("**DATA OBJECT**")
        'xnlCitations(ref).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_TITLE").AppendChild(cdTitle)
        Try
            xnlCitations(ref).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_TITLE").ChildNodes(0).Value = "**DATA OBJECT**"
        Catch ex As Exception
            MakeTree(xnlCitations(ref), "CI_INFO/CI_JOURNAL/CI_TITLE", "<![CDATA[**DATA OBJECT**]]>")
        End Try
        MakeTree(xnlCitations(ref), "CI_CAPTURE/CI_CAPTURE_TITLE", "")
        cdTitle = xmlDoc.CreateCDataSection("DATA OBJECT")
        xnlCitations(ref).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_TITLE").AppendChild(cdTitle)
        MakeTree(xnlCitations(ref), "CI_CAPTURE/CI_CAPTURE_BLURB", txtOCR.Text)
        xmlDoc.Save(strTempPath & System.IO.Path.GetFileName(strInFile))
    End Sub

    Private Sub SaveTitleError()
        Dim ref As Integer
        ref = CInt(Split(lblCurrent.Text, "of").First.Trim)
        RemoveCIInfoChildren(xnlCitations(ref))
        ClearCIInfoFields()
        MakeCIInfoJournal(xnlCitations(ref))
        'MakeTree(xnlCitations(ref), "CI_INFO/CI_JOURNAL/CI_AUTHOR", "")
        'MakeTree(xnlCitations(ref), "CI_INFO/CI_JOURNAL/CI_VOLUME", "")
        'MakeTree(xnlCitations(ref), "CI_INFO/CI_JOURNAL/CI_PAGE", "")
        'MakeTree(xnlCitations(ref), "CI_INFO/CI_JOURNAL/CI_YEAR", "")
        'MakeTree(xnlCitations(ref), "CI_INFO/CI_JOURNAL/CI_TITLE", "")
        'Dim cdTitle As XmlCDataSection
        'cdTitle = xmlDoc.CreateCDataSection("**TITLE-ERROR**")
        'xnlCitations(ref).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_TITLE").AppendChild(cdTitle)
        Try
            xnlCitations(ref).SelectSingleNode("CI_INFO/CI_JOURNAL/CI_TITLE").ChildNodes(0).Value = "**TITLE-ERROR**"
        Catch ex As Exception
            MakeTree(xnlCitations(ref), "CI_INFO/CI_JOURNAL/CI_TITLE", "**TITLE-ERROR**")
        End Try
        MakeTree(xnlCitations(ref), "CI_CAPTURE/CI_CAPTURE_BLURB", txtOCR.Text)
        MakeTree(xnlCitations(ref), "CI_CAPTURE/CI_CAPTURE_TITLE", "TITLE ERROR")
    End Sub

    Public Shared Sub MakeTree(xnNode As XmlNode, strTree As String, Optional value As String = "!@!@!",
                               Optional InsertFirst As Boolean = False, Optional InsertLast As Boolean = False,
                               Optional InsertAfter As XmlNode = Nothing, Optional InsertBefore As XmlNode = Nothing)
        Dim nod As XmlNode, nodnew As XmlNode, xeElement As XmlElement
        nod = Nothing
        nodnew = Nothing
        xeElement = Nothing

        nod = xnNode
        For Each str As String In Split(strTree, "/")
            If str.Trim <> String.Empty Then
                nodnew = nod.SelectSingleNode(str)
                If nodnew Is Nothing Then
                    xeElement = xnNode.OwnerDocument.CreateElement(str)
                    If str = Split(strTree, "/").Last Then
                        If InsertFirst Then
                            If nod.FirstChild IsNot Nothing Then
                                nod = nod.InsertBefore(xeElement, nod.FirstChild)
                            Else
                                nod = nod.AppendChild(xeElement)
                            End If
                        ElseIf InsertLast Then
                            nod = nod.AppendChild(xeElement)
                        ElseIf InsertAfter IsNot Nothing Then
                            nod = nod.InsertAfter(xeElement, InsertAfter)
                        ElseIf InsertBefore IsNot Nothing Then
                            nod = nod.InsertBefore(xeElement, InsertBefore)
                        Else
                            nod = nod.AppendChild(xeElement)
                        End If
                    Else
                        nod = nod.AppendChild(xeElement)
                    End If
                Else
                    nod = nodnew
                End If
            End If
        Next
        If value <> "!@!@!" Then
            nod.InnerText = value
        End If
    End Sub

    Private Shared Sub MakeTreeAttr(xnNode As XmlNode, strTree As String, strAttr1 As String, strAttr2 As String, Optional nodVal As String = "!@!@!", Optional attrVal1 As String = "!@!@!", Optional attrVal2 As String = "!@!@!", Optional InsertAfter As XmlNode = Nothing)
        Dim nod As XmlNode, nodnew As XmlNode, xeElement As XmlElement, xaAttr As XmlAttribute
        nod = xnNode
        For Each str As String In Split(strTree, "/")
            If Trim(str) <> String.Empty Then
                nodnew = nod.SelectSingleNode(str & "[" & strAttr1 & "='" & attrVal1 & "']")
                If nodnew Is Nothing Then
                    xeElement = xnNode.OwnerDocument.CreateElement(str)
                    xaAttr = xnNode.OwnerDocument.CreateAttribute(strAttr1)
                    If attrVal1 <> "!@!@!" Then
                        xaAttr.Value = attrVal1
                    End If
                    xeElement.Attributes.Append(xaAttr)
                    xaAttr = xnNode.OwnerDocument.CreateAttribute(strAttr2)
                    If attrVal2 <> "!@!@!" Then
                        xaAttr.Value = attrVal2
                    End If
                    xeElement.Attributes.Append(xaAttr)
                    If InsertAfter IsNot Nothing Then
                        nod = nod.InsertAfter(xeElement, InsertAfter)
                    Else
                        nod = nod.InsertBefore(xeElement, nod.SelectNodes("CI_CITATION")(0))    'todo: correct possition
                    End If
                Else
                    nod = nodnew
                End If
            End If
        Next
        If nodVal <> "!@!@!" Then
            nod.InnerText = nodVal
        End If
    End Sub

    Private Sub focusToBox()
        Select Case True
            Case rbAuthor.Checked
                frmLogin.lblLField.Text = "txtAuthor"
            Case rbVolume.Checked
                txtVolume.Focus()
                frmLogin.lblLField.Text = "txtVolume"
            Case rbPage.Checked
                txtPage.Focus()
                frmLogin.lblLField.Text = "txtPage"
            Case rbYear.Checked
                txtYear.Focus()
                frmLogin.lblLField.Text = "txtYear"
            Case rbTitle.Checked
                txtTitle.Focus()
                frmLogin.lblLField.Text = "txtTitle"
        End Select
    End Sub

    Private Sub cmdGoto_Click(sender As Object, e As EventArgs) Handles cmdGoto.Click
        If Not IsNumeric(txtGoto.Text) Then
            MsgBox("Input number only", MsgBoxStyle.Information, "Verify")
            Exit Sub
        Else
            If CInt(txtGoto.Text) < 1 Or CInt(txtGoto.Text) > xnlCitations.Count Then
                MsgBox("Out of range", MsgBoxStyle.Information, "Verify")
                Exit Sub
            Else
                subViewRef(CInt(txtGoto.Text) - 1)
            End If
        End If
    End Sub

    Private Sub txtGoto_KeyPress(sender As Object, e As KeyPressEventArgs) Handles txtGoto.KeyPress
        If Asc(e.KeyChar) = Keys.Return Then
            cmdGoto.PerformClick()
            e.Handled = True
        End If
    End Sub

    Private Sub cmdZoning_Click(sender As Object, e As EventArgs) Handles cmdZoning.Click
        pbSource.ImageLocation = strInPath & Split(xnlCitations(iNo).SelectSingleNode("CI_CAPTURE/CI_IMAGE_CLIP_NAME").InnerText, "\")(1)
        subToggleZoning()
    End Sub

    Private Sub pbSource_MouseDown(sender As Object, e As MouseEventArgs) Handles pbSource.MouseDown
        If pbSource.ImageLocation = String.Empty Then Exit Sub
        If chkFullWidth.Checked Then
            mRect = New Rectangle(0, e.Y, pbSource.Width, 0)
        Else
            mRect = New Rectangle(e.X, e.Y, 0, 0)
        End If
        pbSource.Invalidate()
    End Sub

    Private Sub pbSource_MouseMove(sender As Object, e As MouseEventArgs) Handles pbSource.MouseMove
        If pbSource.ImageLocation = String.Empty Then Exit Sub
        If e.Button = Windows.Forms.MouseButtons.Left Then
            If chkFullWidth.Checked Then
                mRect = New Rectangle(0, mRect.Top, pbSource.Width, e.Y - mRect.Top)
            Else
                mRect = New Rectangle(mRect.Left, mRect.Top, e.X - mRect.Left, e.Y - mRect.Top)
            End If
            pbSource.Invalidate()
        End If
    End Sub

    Private Sub pbSource_MouseUp(sender As Object, e As MouseEventArgs) Handles pbSource.MouseUp
        If pbSource.ImageLocation = String.Empty Then Exit Sub
        If mRect.IsEmpty Then Exit Sub
        CropImage()
    End Sub

    Private Sub pbSource_Paint(sender As Object, e As PaintEventArgs) Handles pbSource.Paint
        If mRect.IsEmpty Then Exit Sub
        Using pen As New Pen(Color.Red, 1)
            e.Graphics.DrawRectangle(pen, mRect)
        End Using
    End Sub

    'Protected Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean
    '    If keyData = Keys.Up Then
    '        If Me.pbSource.Bounds.Contains(Me.PointToClient(Cursor.Position)) Then
    '            mRect.Location = New Point(mRect.Location.X, mRect.Location.Y - 1)
    '            pbSource.Invalidate()
    '            CropImage()
    '            Return True
    '        End If
    '    End If
    '    If keyData = Keys.Down Then
    '        If Me.pbSource.Bounds.Contains(Me.PointToClient(Cursor.Position)) Then
    '            mRect.Location = New Point(mRect.Location.X, mRect.Location.Y + 1)
    '            pbSource.Invalidate()
    '            CropImage()
    '            Return True
    '        End If
    '    End If
    '    If keyData = Keys.Left Then
    '        If Me.pbSource.Bounds.Contains(Me.PointToClient(Cursor.Position)) Then
    '            mRect.Location = New Point(mRect.Location.X - 1, mRect.Location.Y)
    '            pbSource.Invalidate()
    '            CropImage()
    '            Return True
    '        End If
    '    End If
    '    If keyData = Keys.Right Then
    '        If Me.pbSource.Bounds.Contains(Me.PointToClient(Cursor.Position)) Then
    '            mRect.Location = New Point(mRect.Location.X + 1, mRect.Location.Y)
    '            pbSource.Invalidate()
    '            CropImage()
    '            Return True
    '        End If
    '    End If
    '    Return MyBase.ProcessCmdKey(msg, keyData)
    'End Function

    Private Sub CropImage()
        Dim fileName = pbSource.ImageLocation
        If fileName Is Nothing Then Exit Sub
        If mRect.IsEmpty Then Exit Sub
        If mRect.Width * mRect.Height = 0 Then Exit Sub
        Dim CropRect As New Rectangle(mRect.Left, mRect.Top, mRect.Width, mRect.Height)
        Dim OriginalImage = Image.FromFile(fileName)
        Dim CropImage As Bitmap = Nothing
        Try
            CropImage = New Bitmap(CropRect.Width, CropRect.Height)
        Catch ex As Exception
        End Try
        If CropImage Is Nothing Then Exit Sub

        Using grp = Graphics.FromImage(CropImage)
            grp.DrawImage(OriginalImage, New Rectangle(0, 0, CropRect.Width, CropRect.Height), CropRect, GraphicsUnit.Pixel)
            OriginalImage.Dispose()
            'CropImage.Save(IO.Path.GetDirectoryName(fileName) & "\temp.TIF")
            If chkMergeImg.Checked = True Then
                pbImage.Image = CombineImages(pbImage.Image, CropImage)
            Else
                pbImage.Image = CropImage
            End If
            pbImage.Image.Save(IO.Path.GetDirectoryName(fileName) & "\temp.TIF")
            If Not fileName.Contains("-BU.TIF") Then
                If Not IO.File.Exists(Replace(fileName, ".TIF", "-BU.TIF", Compare:=CompareMethod.Text)) Then
                    My.Computer.FileSystem.RenameFile(fileName, Replace(IO.Path.GetFileName(fileName), ".TIF", "-BU.TIF", Compare:=CompareMethod.Text))
                End If
                'pbSource.ImageLocation = IO.Path.GetDirectoryName(fileName) & "\" & Replace(IO.Path.GetFileName(fileName), ".TIF", "-BU.TIF", Compare:=CompareMethod.Text)
            End If

            If IO.File.Exists(IO.Path.GetDirectoryName(fileName) & "\" & Split(xnlCitations(iNo).SelectSingleNode("CI_CAPTURE/CI_IMAGE_CLIP_NAME").InnerText, "\").Last) Then
                IO.File.Delete(IO.Path.GetDirectoryName(fileName) & "\" & Split(xnlCitations(iNo).SelectSingleNode("CI_CAPTURE/CI_IMAGE_CLIP_NAME").InnerText, "\").Last)
            End If
            Try
                My.Computer.FileSystem.RenameFile(IO.Path.GetDirectoryName(fileName) & "\temp.TIF", Split(xnlCitations(iNo).SelectSingleNode("CI_CAPTURE/CI_IMAGE_CLIP_NAME").InnerText, "\").Last)
            Catch ex As Exception
                My.Computer.FileSystem.RenameFile(IO.Path.GetDirectoryName(fileName) & "\temp.TIF", lblAccn.Text & CInt(xmlDoc.SelectSingleNode("//ITEM").Attributes("ITEMNO").Value).ToString("D3") & "C_" & xnlCitations.Count & ".TIF")
            End Try
        End Using
    End Sub

    Private Sub subToggleZoning()
        pnlSource.Visible = Not pnlSource.Visible
        gbZoneTool.Visible = Not gbZoneTool.Visible
    End Sub

    'Private Sub cmdZClose_Click(sender As Object, e As EventArgs) Handles cmdZClose.Click
    '    subToggleZoning()
    '    pbSource.Image = Nothing
    'End Sub

    Shared Function getCitations() As XmlNodeList
        Return frmMain.xnlCitations
    End Function

    Shared Function getInPath() As String
        If frmMain.bIsPrior Then
            Return frmMain.strPriorPath
        Else
            Return frmMain.strInPath
        End If
    End Function

    Shared Function getInFile() As String
        Return frmMain.strInFile
    End Function

    Private Sub cmdViewAll_Click(sender As Object, e As EventArgs) Handles cmdViewAll.Click
        frmImages.Show()
    End Sub

    Private Sub cmdRefresh_Click(sender As Object, e As EventArgs) Handles cmdRefresh.Click
        pbImage.ImageLocation = pbImage.ImageLocation
    End Sub

    Private Sub txtArtn1_GotFocus(sender As Object, e As EventArgs) Handles txtArtn1.GotFocus
        rbARTN1.Checked = True
    End Sub

    Private Sub txtArtn3_GotFocus(sender As Object, e As EventArgs) Handles txtArtn3.GotFocus
        rbARTN3.Checked = True
    End Sub

    Private Sub txtArtn4_GotFocus(sender As Object, e As EventArgs) Handles txtArtn4.GotFocus
        rbARTN4.Checked = True
    End Sub

    Private Sub txtArtn4_VisibleChanged(sender As Object, e As EventArgs) Handles txtArtn4.VisibleChanged
        If txtArtn4.Visible = True Then
            cmdAddArtn.Visible = False
        Else
            cmdAddArtn.Visible = True
        End If
    End Sub

    Private Sub cmdLoadBackup_Click(sender As Object, e As EventArgs) Handles cmdLoadBackup.Click
        If cbBackup.SelectedItem = vbNullString Then
            MsgBox("Select file")
            Exit Sub
        Else
            Dim temppath As String
            'If bIsPrior Then
            '    temppath = strPriorPath
            'Else
            '    temppath = strInPath
            'End If
            temppath = strInPath

            If IO.File.Exists(temppath & cbBackup.SelectedItem & ".TIF") Then
                pbSource.ImageLocation = temppath & cbBackup.SelectedItem & ".TIF"
            Else
                MsgBox("Invalid file name")
                Exit Sub
            End If
        End If
    End Sub

    '    Private Sub SavePatent()

    '        Dim inum As Integer = CInt(Trim(Split(lblCurrent.Text, "of")(0)))
    '        'Dim nod As XmlNode
    '        'If Trim(txtTitle.Text) = vbNullString Then
    '        '    MsgBox("Title should not be empty", MsgBoxStyle.Information, "Verify")
    '        '    'Exit Sub
    '        'End If

    '        Dim root As XmlNode = xmlDoc.DocumentElement
    '        Dim elem As XmlElement = xmlDoc.CreateElement("CI_PATENT")
    '        Dim attr As XmlAttribute = xmlDoc.CreateAttribute("ID")

    '        attr.Value = txtAuthor.Text.Replace("&", String.Empty)
    '        elem.Attributes.Append(attr)
    '        Dim strInXMl As String = String.Empty

    '        If Trim(txtVolume.Text) <> vbNullString Then strInXMl = strInXMl & "<PATENT_VOLUME>" & txtVolume.Text & "</PATENT_VOLUME>" & vbNewLine
    '        If Trim(txtPage.Text) <> vbNullString Then strInXMl = strInXMl & "<PATENT_PAGE>" & txtPage.Text & "</PATENT_PAGE>" & vbNewLine
    '        If Trim(txtYear.Text) <> vbNullString Then strInXMl = strInXMl & "<PATENT_YEAR>" & txtYear.Text & "</PATENT_YEAR>" & vbNewLine
    '        If Trim(txtTitle.Text) <> vbNullString Then strInXMl = strInXMl & "<PATENT_ASSIGNEE>" & txtTitle.Text & "</PATENT_ASSIGNEE>" & vbNewLine

    '        elem.InnerXml = Replace(strInXMl, "&", "")
    '        'elem.InnerXml = strInXMl
    '        Dim node As XmlNode = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_JOURNAL")

    '        If node Is Nothing Then
    '            node = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_PATENT")
    '            If node Is Nothing Then
    '                xnlCitations(inum).SelectSingleNode("CI_INFO").AppendChild(elem)
    '            Else
    '                node.ParentNode.ReplaceChild(elem, node)
    '            End If
    '        Else
    '            node.ParentNode.ReplaceChild(elem, node)
    '        End If

    '        Dim inClipName As String = String.Empty
    '        Try
    '            inClipName = xnlCitations(inum).SelectSingleNode("CI_CAPTURE/CI_IMAGE_CLIP_NAME").InnerText
    '            inClipName = Replace(inClipName, Split(inClipName, "_").Last, xnlCitations(inum).Attributes("seq").InnerText & ".TIF")
    '        Catch ex As Exception
    '            MakeTree(xnlCitations(inum), "CI_CAPTURE/CI_IMAGE_CLIP_NAME")
    '            inClipName = lblAccn.Text & CInt(xmlDoc.SelectSingleNode("//ITEM").Attributes("ITEMNO").Value).ToString("D3") & "_" & xnlCitations(inum).Attributes("seq").InnerText & ".TIF"
    '        End Try

    '        'If inClipName = String.Empty Then
    '        '    makeTree(xnlCitations(inum), "CI_CAPTURE/CI_IMAGE_CLIP_NAME")
    '        '    inClipName = lblAccn.Text & CInt(xmlDoc.SelectSingleNode("//ITEM").Attributes("ITEMNO").Value).ToString("D3") & "_" & xnlCitations(inum).Attributes("seq").InnerText & ".TIF"
    '        'Else
    '        '    inClipName = Replace(inClipName, Split(inClipName, "_").Last, xnlCitations(inum).Attributes("seq").InnerText & ".TIF")
    '        'End If
    '        xnlCitations(inum).SelectSingleNode("CI_CAPTURE/CI_IMAGE_CLIP_NAME").InnerText = inClipName
    '        xnlCitations(inum).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_BLURB").InnerText = txtOCR.Text
    '        For i = 1 To 4
    '            If Me.Controls.Find("txtArtn" & i, True)(0).Visible = True Then
    '                If (Trim(Me.Controls.Find("txtArtn" & i, True)(0).Text) = vbNullString) Xor (Me.Controls.Find("cmbArtn" & i, True)(0).Text = vbNullString) Then
    '                    MsgBox("Check Artn fields", MsgBoxStyle.Information, "Verify")
    '                    Exit Sub
    '                ElseIf Trim(Me.Controls.Find("txtArtn" & i, True)(0).Text) <> vbNullString Then
    '16:                 Dim nd As XmlNode = xnlCitations(inum).SelectSingleNode("RI_CITATIONIDENTIFIER[@seq='" & i & "']")
    '                    If nd Is Nothing Then
    '                        MakeTreeAttr(xnlCitations(inum), "RI_CITATIONIDENTIFIER", "seq", "type", Trim(Me.Controls.Find("txtArtn" & i, True)(0).Text), i, Me.Controls.Find("cmbArtn" & i, True)(0).Text)
    '                    End If
    '                End If
    '            Else
    '                Exit For
    '            End If
    '        Next
    '        Try
    '            Dim nod As XmlNode
    '            nod = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_JOURNAL")
    '            nod.ParentNode.RemoveChild(nod)
    '        Catch ex As Exception
    '        End Try
    '        attr = xnlCitations(inum).Attributes("D")
    '        If attr IsNot Nothing Then
    '            Try
    '                xnlCitations(inum).RemoveChild(xnlCitations(inum).SelectSingleNode("FULL_CI_INFO"))
    '            Catch ex As Exception
    '            End Try
    '        End If
    '        xmlDoc.Save(strTempPath & System.IO.Path.GetFileName(strInFile))
    '        'Try
    '        '    xmlDoc.Save(Split(xmlDoc.BaseURI, "///").Last)
    '        'Catch ex As Exception
    '        '    MsgBox("URI: " & Split(xmlDoc.BaseURI, "///").Last & vbCrLf & ex.Message)
    '        'End Try

    '    End Sub

    Private Sub SavePatent()

        Dim inum As Integer = CInt(Trim(Split(lblCurrent.Text, "of")(0)))
        'Dim nod As XmlNode

        ''----------------------------
        'nod = Nothing
        'nod = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_JOURNAL")
        'If nod IsNot Nothing Then
        '    nod.ParentNode.RemoveChild(nod)
        'End If
        'MakeTree(xnlCitations(inum), "CI_INFO", "", InsertFirst:=True)
        'Dim attr As XmlAttribute
        'Try
        '    nod = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_PATENT").Attributes("ID")
        '    nod.Value = txtAuthor.Text.Replace("&", String.Empty)
        'Catch ex As Exception
        '    Dim root As XmlNode = xmlDoc.DocumentElement
        '    Dim elem As XmlElement = xmlDoc.CreateElement("CI_PATENT")
        '    attr = xmlDoc.CreateAttribute("ID")
        '    attr.Value = txtAuthor.Text.Replace("&", String.Empty)
        '    elem.Attributes.Append(attr)
        '    xnlCitations(inum).SelectSingleNode("CI_INFO").AppendChild(elem)
        'End Try

        'nod = Nothing
        'Try
        '    nod = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_PATENT/PATENT_VOLUME")
        '    nod.InnerText = CInt(txtVolume.Text.Trim).ToString("D4")
        'Catch ex As Exception
        '    MakeTree(xnlCitations(inum), "CI_INFO/CI_PATENT/PATENT_VOLUME", "", InsertFirst:=True)
        '    nod = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_PATENT/PATENT_VOLUME")
        '    nod.InnerText = txtVolume.Text.Trim
        'End Try

        'nod = Nothing
        'Try
        '    nod = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_PATENT/PATENT_PAGE")
        '    If IsNumeric(txtPage.Text.Trim) Then
        '        nod.InnerText = CInt(txtPage.Text.Trim).ToString("D5")
        '    Else
        '        nod.InnerText = txtPage.Text.Trim
        '    End If
        'Catch ex As Exception
        '    MakeTree(xnlCitations(inum), "CI_INFO/CI_PATENT/PATENT_PAGE", "", InsertAfter:=xnlCitations(inum).SelectSingleNode("CI_INFO/CI_PATENT/PATENT_VOLUME"))
        '    nod = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_PATENT/PATENT_PAGE")
        '    nod.InnerText = txtPage.Text.Trim
        'End Try

        'nod = Nothing
        'Try
        '    nod = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_PATENT/PATENT_YEAR")
        '    nod.InnerText = txtYear.Text.Trim
        'Catch ex As Exception
        '    MakeTree(xnlCitations(inum), "CI_INFO/CI_PATENT/PATENT_YEAR", "", InsertAfter:=xnlCitations(inum).SelectSingleNode("CI_INFO/CI_PATENT/PATENT_PAGE"))
        '    nod = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_PATENT/PATENT_YEAR")
        '    nod.InnerText = txtYear.Text.Trim
        'End Try

        'nod = Nothing
        'Try
        '    nod = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_PATENT/PATENT_ASSIGNEE")
        '    nod.InnerText = txtTitle.Text.Trim
        'Catch ex As Exception
        '    MakeTree(xnlCitations(inum), "CI_INFO/CI_PATENT/PATENT_ASSIGNEE", "", InsertAfter:=xnlCitations(inum).SelectSingleNode("CI_INFO/CI_PATENT/PATENT_YEAR"))
        '    nod = xnlCitations(inum).SelectSingleNode("CI_INFO/CI_PATENT/PATENT_ASSIGNEE")
        '    nod.InnerText = txtTitle.Text.Trim
        'End Try
        ''---------------------------
        RemoveCIInfoChildren(xnlCitations(inum))
        MakeCIInfoPatent(xnlCitations(inum))

        Dim inClipName As String = String.Empty
        Try
            inClipName = xnlCitations(inum).SelectSingleNode("CI_CAPTURE/CI_IMAGE_CLIP_NAME").InnerText
            inClipName = Replace(inClipName, Split(inClipName, "_").Last, xnlCitations(inum).Attributes("seq").InnerText & ".TIF")
        Catch ex As Exception
            MakeTree(xnlCitations(inum), "CI_CAPTURE/CI_IMAGE_CLIP_NAME")
            inClipName = lblAccn.Text & CInt(xmlDoc.SelectSingleNode("//ITEM").Attributes("ITEMNO").Value).ToString("D3") & "_" & xnlCitations(inum).Attributes("seq").InnerText & ".TIF"
        End Try

        'If inClipName = String.Empty Then
        '    makeTree(xnlCitations(inum), "CI_CAPTURE/CI_IMAGE_CLIP_NAME")
        '    inClipName = lblAccn.Text & CInt(xmlDoc.SelectSingleNode("//ITEM").Attributes("ITEMNO").Value).ToString("D3") & "_" & xnlCitations(inum).Attributes("seq").InnerText & ".TIF"
        'Else
        '    inClipName = Replace(inClipName, Split(inClipName, "_").Last, xnlCitations(inum).Attributes("seq").InnerText & ".TIF")
        'End If
        xnlCitations(inum).SelectSingleNode("CI_CAPTURE/CI_IMAGE_CLIP_NAME").InnerText = inClipName
        Try
            xnlCitations(inum).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_BLURB").InnerText = txtOCR.Text
        Catch ex As Exception
        End Try
        Try
            xnlCitations(inum).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_TITLE").InnerText = String.Empty
        Catch ex As Exception
            MakeTree(xnlCitations(inum), "CI_CAPTURE/CI_CAPTURE_TITLE", String.Empty)
        End Try


        If Not ReviewARTN(xnlCitations(inum)) Then Exit Sub
        For i = 1 To 4
            If Me.Controls.Find("txtArtn" & i, True)(0).Visible = True Then
                If (Trim(Me.Controls.Find("txtArtn" & i, True)(0).Text) = vbNullString) Xor (Me.Controls.Find("cmbArtn" & i, True)(0).Text = vbNullString) Then
                    MsgBox("Check Artn fields", MsgBoxStyle.Information, "Verify")
                    Exit Sub
                ElseIf Trim(Me.Controls.Find("txtArtn" & i, True)(0).Text) <> vbNullString Then
16:                 Dim nd As XmlNode = xnlCitations(inum).SelectSingleNode("RI_CITATIONIDENTIFIER[@seq='" & i & "']")
                    If nd Is Nothing Then
                        MakeTreeAttr(xnlCitations(inum), "RI_CITATIONIDENTIFIER", "seq", "type", Trim(Me.Controls.Find("txtArtn" & i, True)(0).Text), i, Me.Controls.Find("cmbArtn" & i, True)(0).Text)
                    End If
                End If
            Else
                Exit For
            End If
        Next

        Dim attr As XmlAttribute = xnlCitations(inum).Attributes("D")
        If attr IsNot Nothing Then
            Try
                xnlCitations(inum).RemoveChild(xnlCitations(inum).SelectSingleNode("FULL_CI_INFO"))
            Catch ex As Exception
            End Try
        End If
        xmlDoc.Save(strTempPath & System.IO.Path.GetFileName(strInFile))

    End Sub

    Public Shared Function CombineImages(ByVal img1 As Image, ByVal img2 As Image) As Image
        Dim bmp As New Bitmap(Math.Max(img1.Width, img2.Width), img1.Height + img2.Height)
        Dim g As Graphics = Graphics.FromImage(bmp)

        g.DrawImage(img1, 0, 0, img1.Width, img1.Height)
        g.DrawImage(img2, 0, img1.Height, img2.Width, img2.Height)
        g.Dispose()
        Return bmp
    End Function

    Private Sub ThemeColorToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ThemeColorToolStripMenuItem.Click
        If cdColorDlg.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Me.BackColor = cdColorDlg.Color
            Me.gbZoneTool.BackColor = Me.BackColor
            My.Settings.MainBackColor = Me.BackColor
            My.Settings.Save()
        End If
    End Sub

    Private Sub txtArtn1_KeyDown(sender As Object, e As KeyEventArgs) Handles txtArtn1.KeyDown
        If e.Modifiers = Keys.Control Then
            If e.KeyCode = Keys.Down Then
                Try
                    cmbArtn1.SelectedIndex = (cmbArtn1.SelectedIndex + 1)
                Catch ex As Exception
                    cmbArtn1.SelectedIndex = (0)
                End Try
                e.Handled = True
            ElseIf e.KeyCode = Keys.Up Then
                Try
                    cmbArtn1.SelectedIndex = (cmbArtn1.SelectedIndex - 1)
                Catch ex As Exception
                    cmbArtn1.SelectedIndex = (cmbArtn1.Items.Count - 1)
                End Try
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub txtArtn2_KeyDown(sender As Object, e As KeyEventArgs) Handles txtArtn2.KeyDown
        If e.Modifiers = Keys.Control Then
            If e.KeyCode = Keys.Down Then
                Try
                    cmbArtn2.SelectedIndex = (cmbArtn2.SelectedIndex + 1)
                Catch ex As Exception
                    cmbArtn2.SelectedIndex = (0)
                End Try
                e.Handled = True
            ElseIf e.KeyCode = Keys.Up Then
                Try
                    cmbArtn2.SelectedIndex = (cmbArtn2.SelectedIndex - 1)
                Catch ex As Exception
                    cmbArtn2.SelectedIndex = (cmbArtn2.Items.Count - 1)
                End Try
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub txtArtn3_KeyDown(sender As Object, e As KeyEventArgs) Handles txtArtn3.KeyDown
        If e.Modifiers = Keys.Control Then
            If e.KeyCode = Keys.Down Then
                Try
                    cmbArtn3.SelectedIndex = (cmbArtn3.SelectedIndex + 1)
                Catch ex As Exception
                    cmbArtn3.SelectedIndex = (0)
                End Try
                e.Handled = True
            ElseIf e.KeyCode = Keys.Up Then
                Try
                    cmbArtn3.SelectedIndex = (cmbArtn3.SelectedIndex - 1)
                Catch ex As Exception
                    cmbArtn3.SelectedIndex = (cmbArtn3.Items.Count - 1)
                End Try
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub txtArtn4_KeyDown(sender As Object, e As KeyEventArgs) Handles txtArtn4.KeyDown
        If e.Modifiers = Keys.Control Then
            If e.KeyCode = Keys.Down Then
                Try
                    cmbArtn4.SelectedIndex = (cmbArtn4.SelectedIndex + 1)
                Catch ex As Exception
                    cmbArtn4.SelectedIndex = (0)
                End Try
                e.Handled = True
            ElseIf e.KeyCode = Keys.Up Then
                Try
                    cmbArtn4.SelectedIndex = (cmbArtn4.SelectedIndex - 1)
                Catch ex As Exception
                    cmbArtn4.SelectedIndex = (cmbArtn4.Items.Count - 1)
                End Try
                e.Handled = True
            End If
        End If
    End Sub

    Private Sub QueryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles QueryToolStripMenuItem.Click
        If MsgBox("Do you want to reject this item?", MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
            If strInFile Is String.Empty Then Exit Sub

            'On Error Resume Next
            'My.Computer.FileSystem.DeleteFile(strQueryPath & System.IO.Path.GetFileName(strInFile))
            Try
                My.Computer.FileSystem.MoveFile(strCurPath & System.IO.Path.GetFileName(strInFile), strQueryPath & System.IO.Path.GetFileName(strInFile), True)
            Catch ex As Exception
            End Try

            For Each file As String In My.Computer.FileSystem.GetFiles(strInPath)
                If file.Contains(IO.Path.GetFileNameWithoutExtension(strInFile)) Then
                    Try
                        My.Computer.FileSystem.MoveFile(file, strQueryPath & System.IO.Path.GetFileName(file), True)
                    Catch ex As Exception
                    End Try
                End If
            Next

            'iCompItems = iCompItems + 1
            'iCompRefs = iCompRefs + xnlCitations.Count
            swCurProd.Stop()
            swCurSusp.Stop()
            'swLogFile.WriteLine(vbTab & xnlCitations.Count & vbTab & swCurProd.Elapsed.ToString("hh\:mm\:ss") & vbTab & vbTab & swCurSusp.Elapsed.ToString("hh\:mm\:ss"))
            'swLogFile.Close()

            'Dim TotRefs As Integer = xnlCitations.Count
            'Dim DelRefs As Integer = xnlCitations(0).ParentNode.SelectNodes("CI_CITATION[@D='Y']").Count
            'Dim InsRefs As Integer = xnlCitations(0).ParentNode.SelectNodes("CI_CITATION[@D='Y']").Count
            'LogEntry.References_Close = TotRefs + InsRefs - DelRefs
            'LogEntry.References_Deleted = DelRefs
            'LogEntry.References_Inserted = InsRefs
            LogEntry.Session_Production = swCurProd.Elapsed.ToString("hh\:mm\:ss")
            LogEntry.Session_Suspend = swCurSusp.Elapsed.ToString("hh\:mm\:ss")
            LogEntry.Session_End_Date = Format(Today, "dd-MM-yyyy")
            LogEntry.Session_End_Time = Now.ToShortTimeString
            LogEntry.Status = "QR"
            'MakeLogEntry(strLogPath & frmLogin.txtUserName.Text & ".XML")
            UpdateLogEntry(strLogPath & frmLogin.txtUserName.Text & ".XML", LogEntry)
            lblCompItems.Text = iCompItems
            lblCompRefs.Text = iCompRefs
            subClearFields()
            pbSource.Image = Nothing
            lblCurrent.Text = " of "

            'System.IO.File.Move(strCurPath & System.IO.Path.GetFileName(strInFile), strCompPath & System.IO.Path.GetFileName(strInFile))
            If MsgBox("Do you want to load new item?", MsgBoxStyle.YesNo, "Confirmation") = MsgBoxResult.Yes Then
                strInFile = fnGetXMLFile(strPriorPath)

                If strInFile = "" Then
                    bIsPrior = False
                    strInFile = fnGetXMLFile(strInPath)
                End If
                If strInFile = "" Then
                    MsgBox("No files for input", MsgBoxStyle.Information)
                Else
                    bIsPrior = False
                    fnLoadFile(strInFile)
                End If
            Else
                strInFile = ""
                lvlist.Items.Clear()
            End If
        End If
    End Sub

    Private Function bumpSpace(sTemp As String) As String

        Dim bBumped As Boolean = False
        While Not bBumped
            If sTemp.Contains("  ") Then
                sTemp = sTemp.Replace("  ", " ")
                bBumped = False
            Else
                bBumped = True
            End If
        End While
        Return sTemp
    End Function

    Private Sub txtArtn1_TextChanged(sender As Object, e As EventArgs) Handles txtArtn1.TextChanged
        txtArtn1.Text = txtArtn1.Text.Replace(" ", String.Empty).Trim
    End Sub

    Private Sub txtArtn2_TextChanged(sender As Object, e As EventArgs) Handles txtArtn2.TextChanged
        txtArtn2.Text = txtArtn2.Text.Replace(" ", String.Empty).Trim
    End Sub

    Private Sub txtArtn3_TextChanged(sender As Object, e As EventArgs) Handles txtArtn3.TextChanged
        txtArtn3.Text = txtArtn3.Text.Replace(" ", String.Empty).Trim
    End Sub

    Private Sub txtArtn4_TextChanged(sender As Object, e As EventArgs) Handles txtArtn4.TextChanged
        txtArtn4.Text = txtArtn4.Text.Replace(" ", String.Empty).Trim
    End Sub

    Private Function MakeDirectory(strDir As String) As Boolean
        Try
            If Not IO.Directory.Exists(strDir) Then
                IO.Directory.CreateDirectory(strDir)
            End If
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Sub DoneToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DoneToolStripMenuItem.Click
        cmdDone.PerformClick()
    End Sub

    Private Sub checkNTR(ndNode As XmlNode)
        Try
            If ndNode.SelectSingleNode("CI_TITLE").InnerText = "**NON-TRADITIONAL**" Then
                chkNTR.Checked = True
            Else
                chkNTR.Checked = False
            End If
        Catch ex As Exception
            chkNTR.Checked = False
        End Try
    End Sub

    Private Sub checkDCI(ndNode As XmlNode)
        Try
            If ndNode.SelectSingleNode("CI_TITLE").InnerText = "**DATA OBJECT**" Then
                chkDCI.Checked = True
            Else
                chkDCI.Checked = False
            End If
        Catch ex As Exception
            chkDCI.Checked = False
        End Try
    End Sub

    Private Sub checkTitleError(ndNode As XmlNode)
        Try
            If ndNode.SelectSingleNode("CI_TITLE").InnerText = "**TITLE-ERROR**" Then
                chkTitErr.Checked = True
            Else
                chkTitErr.Checked = False
            End If
        Catch ex As Exception
            chkTitErr.Checked = False
        End Try
    End Sub

    Private Sub MakeLogFile(filename As String)
        If Not IO.File.Exists(filename) Then
            Dim writer As New XmlTextWriter(filename, System.Text.Encoding.UTF8)
            writer.WriteStartDocument(True)
            writer.Formatting = Formatting.Indented
            writer.Indentation = 3
            writer.WriteStartElement("Logs")
            writer.WriteEndElement()                                    'Logs
            writer.WriteEndDocument()
            writer.Close()
        Else
            Dim tdoc As New XmlDocument
            Try
                tdoc.Load(filename)
            Catch ex As Exception
                Dim tsb As New System.Text.StringBuilder
                tsb.AppendLine("DB file is either corrupt or invalid")
                tsb.AppendLine("Also check whether DB file is open in some other programs. If so, close that and try again")
                tsb.AppendLine("In the case of corrupt, it will be backed up and New DB file will be created")
                MsgBox(tsb.ToString)
                Try
                    My.Computer.FileSystem.RenameFile(filename, IO.Path.GetFileName(filename) & ".backup")
                Catch ex1 As Exception
                    Dim i As Integer = 1
                    While (IO.File.Exists(IO.Path.GetFileName(filename) & ".backup" & i))
                        i = i + 1
                    End While
                    My.Computer.FileSystem.RenameFile(filename, IO.Path.GetFileName(filename) & ".backup" & i)
                End Try

                Dim writer As New XmlTextWriter(filename, System.Text.Encoding.UTF8)
                writer.WriteStartDocument(True)
                writer.Formatting = Formatting.Indented
                writer.Indentation = 3
                writer.WriteStartElement("Logs")
                writer.WriteEndElement()                                    'Logs
                writer.WriteEndDocument()
                writer.Close()
            End Try
        End If
        'MakeLogEntry(filename)
        tsslStatus.Text = "Created"
    End Sub

    Private Sub InitLog(ByRef LE As Log)
        LE.Session_Start_Date = ""
        LE.Session_Start_Time = ""
        LE.Session_End_Date = ""
        LE.Session_End_Time = ""
        LE.Session_Production = ""
        LE.Session_Suspend = ""
        LE.Item_Accession = ""
        LE.Item_Itemno = ""
        LE.Status = ""
        LE.References_Open = ""
        LE.References_Close = ""
        LE.References_Inserted = ""
        LE.References_Deleted = ""
    End Sub

    Private Sub MakeLogEntry(ByVal filename As String)
        Dim xdDoc As New XmlDocument
        Dim nod1 As XmlNode, nod2 As XmlNode, attr As XmlAttribute

        Try
            xdDoc.Load(strLogPath & frmLogin.txtUserName.Text & ".XML")
        Catch ex As Exception
            MsgBox("Could not open DB file")
            Exit Sub
        End Try

        nod1 = xdDoc.DocumentElement
        nod2 = xdDoc.CreateElement("Log")
        attr = xdDoc.CreateAttribute("seq")
        attr.Value = xdDoc.DocumentElement.SelectNodes("Log").Count + 1
        nod2.Attributes.Append(attr)
        nod1.AppendChild(nod2)
        nod1 = nod2

        nod2 = xdDoc.CreateElement("System")
        attr = xdDoc.CreateAttribute("type")
        attr.Value = LogEntry.System_Type
        nod2.Attributes.Append(attr)
        nod2.InnerText = LogEntry.System
        nod1.AppendChild(nod2)

        nod2 = xdDoc.CreateElement("Session")
        nod1.AppendChild(nod2)
        nod1 = nod2

        nod2 = xdDoc.CreateElement("Start_Time")
        nod1.AppendChild(nod2)
        nod1 = nod2
        nod2 = xdDoc.CreateElement("Date")
        nod2.InnerText = LogEntry.Session_Start_Date
        nod1.AppendChild(nod2)
        nod2 = xdDoc.CreateElement("Time")
        nod2.InnerText = LogEntry.Session_Start_Time
        nod1.AppendChild(nod2)

        nod1 = nod1.ParentNode
        nod2 = xdDoc.CreateElement("End_Time")
        nod1.AppendChild(nod2)
        nod1 = nod2
        nod2 = xdDoc.CreateElement("Date")
        nod2.InnerText = LogEntry.Session_End_Date
        nod1.AppendChild(nod2)
        nod2 = xdDoc.CreateElement("Time")
        nod2.InnerText = LogEntry.Session_End_Time
        nod1.AppendChild(nod2)
        'nod1.ParentNode.AppendChild(nod1)

        nod1 = nod1.ParentNode
        nod2 = xdDoc.CreateElement("Production")
        nod2.InnerText = LogEntry.Session_Production
        nod1.AppendChild(nod2)
        nod2 = xdDoc.CreateElement("Suspend")
        nod2.InnerText = LogEntry.Session_Suspend
        nod1.AppendChild(nod2)

        nod1 = nod1.ParentNode
        nod2 = xdDoc.CreateElement("Item")
        nod1.AppendChild(nod2)
        nod1 = nod2
        nod2 = xdDoc.CreateElement("Accession")
        nod2.InnerText = LogEntry.Item_Accession
        nod1.AppendChild(nod2)
        nod2 = xdDoc.CreateElement("ItemNo")
        nod2.InnerText = LogEntry.Item_Itemno
        nod1.AppendChild(nod2)
        'nod1.ParentNode.AppendChild(nod1)

        nod1 = nod1.ParentNode
        nod2 = xdDoc.CreateElement("Status")
        nod2.InnerText = LogEntry.Status
        nod1.AppendChild(nod2)

        nod2 = xdDoc.CreateElement("References")
        nod1.AppendChild(nod2)
        nod1 = nod2
        nod2 = xdDoc.CreateElement("Open")
        nod2.InnerText = LogEntry.References_Open
        nod1.AppendChild(nod2)
        nod2 = xdDoc.CreateElement("Close")
        nod2.InnerText = LogEntry.References_Close
        nod1.AppendChild(nod2)
        nod2 = xdDoc.CreateElement("Inserted")
        nod2.InnerText = LogEntry.References_Inserted
        nod1.AppendChild(nod2)
        nod2 = xdDoc.CreateElement("Deleted")
        nod2.InnerText = LogEntry.References_Deleted
        nod1.AppendChild(nod2)

        xdDoc.Save(filename)
    End Sub

    Private Sub UpdateLogEntry(ByVal filename As String, LE As Log)
        Dim xddoc As New XmlDocument, lChild As XmlNode
        Try
            xddoc.Load(filename)
        Catch ex As Exception
            MsgBox("Could not open DB file")
            Exit Sub
        End Try

        lChild = xddoc.DocumentElement.LastChild   '.SelectSingleNode(tagname).InnerText = newval
        If lChild Is Nothing Then
            MsgBox("Could not update. Check DB file")
            Exit Sub
        End If
        lChild.SelectSingleNode("System").InnerText = LE.System
        lChild.SelectSingleNode("System").Attributes("type").Value = LE.System_Type
        lChild.SelectSingleNode("Session/Start_Time/Date").InnerText = LE.Session_Start_Date
        lChild.SelectSingleNode("Session/Start_Time/Time").InnerText = LE.Session_Start_Time
        lChild.SelectSingleNode("Session/End_Time/Date").InnerText = LE.Session_End_Date
        lChild.SelectSingleNode("Session/End_Time/Time").InnerText = LE.Session_End_Time
        lChild.SelectSingleNode("Session/Production").InnerText = LE.Session_Production
        lChild.SelectSingleNode("Session/Suspend").InnerText = LE.Session_Suspend
        lChild.SelectSingleNode("Item/Accession").InnerText = LE.Item_Accession
        lChild.SelectSingleNode("Item/ItemNo").InnerText = LE.Item_Itemno
        lChild.SelectSingleNode("Status").InnerText = LE.Status
        lChild.SelectSingleNode("References/Open").InnerText = LE.References_Open
        lChild.SelectSingleNode("References/Close").InnerText = LE.References_Close
        lChild.SelectSingleNode("References/Inserted").InnerText = LE.References_Inserted
        lChild.SelectSingleNode("References/Deleted").InnerText = LE.References_Deleted
        xddoc.Save(filename)
    End Sub

    Private Sub Log_Load(strLog As String)
        If EnableLogToolStripMenuItem.Checked Then
            IO.File.AppendAllText(LogLoadFile, Now.ToString("hh:mm:ss") & " : " & strLog & Environment.NewLine)
        End If
    End Sub

    Private Sub Log_Done(strLog As String)
        If EnableLogToolStripMenuItem.Checked Then
            IO.File.AppendAllText(LogDoneFile, Now.ToString("hh:mm:ss") & " : " & strLog & Environment.NewLine)
        End If
    End Sub

    Private Sub DCI_Check(citation As XmlNode, ByRef new_blurb As String)
        Dim blurb As String, reps As XmlNodeList, URI As String = ""
        'Dim new_blurb As String = ""
        Dim rep_found As String = ""
        Dim var_found As String = ""

        blurb = citation.SelectSingleNode("CI_CAPTURE/CI_CAPTURE_BLURB").InnerText
        URI = ""
        URI = GetURI(blurb, "http://", new_blurb)
        If URI = "" Then
            URI = GetURI(blurb, "https://", new_blurb)
            If URI = "" Then
                URI = GetURI(blurb, "www.", new_blurb)
                If URI = "" Then
                    URI = GetURI(blurb, "doi:", new_blurb)
                    If URI = "" Then Exit Sub
                End If
            End If
        End If

        If System.IO.File.Exists(RepFile) = True Then
            Dim xmlRep As New XmlDocument
            xmlRep.Load(RepFile)
            reps = xmlRep.SelectNodes("DCI-Repository/Repository")
            If reps Is Nothing Then
                MsgBox("No repository to check")
                Exit Sub
            End If
            lblRepName.Text = ""
            For Each rep As XmlNode In reps
                For Each child As XmlNode In rep.ChildNodes
                    If child.Name = "Abbreviation" Then
                        'If URI.Contains(child.InnerText.Replace(" ", "")) Then
                        If FindTextInURI(URI, child.InnerText.Replace(" ", "")) Then
                            lblRepName.Text = "Repository Name : " & rep.SelectSingleNode("Name").InnerText
                        End If
                    ElseIf child.Name = "Name" Then
                        'If new_blurb.Contains(child.InnerText) Then
                        If FindText(new_blurb, child.InnerText) Then
                            If rep_found = "" Then rep_found = child.InnerText
                        ElseIf child.InnerText.Contains("&") Then
                            Dim srch As String = child.InnerText.Replace("&", " and ")
                            srch = srch.Replace("  ", " ")
                            If FindText(new_blurb, srch) Then
                                If rep_found = "" Then rep_found = srch 'child.InnerText
                            End If
                        ElseIf child.InnerText.Contains(" and ") Then
                            Dim srch As String = child.InnerText.Replace(" and ", " & ")
                            srch = srch.Replace("  ", " ")
                            If FindText(new_blurb, srch) Then
                                If rep_found = "" Then rep_found = srch 'child.InnerText
                            End If
                        End If

                    ElseIf child.Name = "Variant" Then
                        'If new_blurb.Contains(child.InnerText) Then
                        If FindText(new_blurb, child.InnerText) Then
                            If var_found = "" Then var_found = rep.SelectSingleNode("Name").InnerText
                        ElseIf child.InnerText.Contains("&") Then
                            Dim srch As String = child.InnerText.Replace("&", " and ")
                            srch = srch.Replace("  ", " ")
                            If FindText(new_blurb, srch) Then
                                If var_found = "" Then var_found = srch 'rep.SelectSingleNode("Name").InnerText
                            ElseIf child.InnerText.Contains(" and ") Then
                                srch = child.InnerText.Replace(" and ", " & ")
                                srch = srch.Replace("  ", " ")
                                If FindText(new_blurb, srch) Then
                                    If var_found = "" Then var_found = srch 'rep.SelectSingleNode("Name").InnerText
                                End If
                            End If
                        End If
                    End If
                Next
            Next
            If lblRepName.Text = "" Then
                If rep_found <> "" Then
                    lblRepName.Text = "Repository Name : " & rep_found
                ElseIf var_found <> "" Then
                    lblRepName.Text = "Repository Name : " & var_found
                End If
            End If
        Else
            MessageBox.Show("Repository File does not exist")
        End If
    End Sub

    Public Shared Function FindText(FullText As String, TextToSearch As String) As Boolean
        If Not FullText.ToLower.Contains(TextToSearch.ToLower) Then Return False
        If FullText.ToLower.Contains((" " & TextToSearch & " ").ToLower) Then Return True
        If FullText.ToLower.StartsWith((TextToSearch & " ").ToLower) Then Return True
        If FullText.ToLower.EndsWith((" " & TextToSearch).ToLower) Then Return True
        If FullText.ToLower.Contains((" " & TextToSearch & ".").ToLower) Then Return True
        If FullText.ToLower.Contains((" " & TextToSearch & ",").ToLower) Then Return True
        If FullText.ToLower.Contains(("/" & TextToSearch & "/").ToLower) Then Return True
        If FullText.ToLower.Contains(("/" & TextToSearch & ".").ToLower) Then Return True
        If FullText.ToLower.Contains(("." & TextToSearch & "/").ToLower) Then Return True
        If FullText.ToLower.Contains(("." & TextToSearch & ".").ToLower) Then Return True
        Return False
    End Function

    Public Shared Function FindTextInURI(FullURI As String, TextToSearch As String) As Boolean
        If Not FullURI.ToLower.Contains(TextToSearch.ToLower) Then Return False
        If FullURI.ToLower.Contains(("/" & TextToSearch & "/").ToLower) Then Return True
        If FullURI.ToLower.Contains(("/" & TextToSearch & ".").ToLower) Then Return True
        If FullURI.ToLower.Contains(("." & TextToSearch & "/").ToLower) Then Return True
        If FullURI.ToLower.Contains(("." & TextToSearch & ".").ToLower) Then Return True
        If FullURI.ToLower.StartsWith((TextToSearch & "/").ToLower) Then Return True
        If FullURI.ToLower.EndsWith("/" & TextToSearch.ToLower) Then Return True
        Return False
    End Function

    Private Function GetURI(blurb As String, beginwith As String, ByRef nb As String) As String
        Dim uri As String = beginwith, lastpos As Integer = blurb.Length
        Dim URIPos As Integer = blurb.IndexOf(beginwith)
        nb = blurb
        If URIPos = -1 Then
            Return ""
        End If
        For i As Integer = URIPos + beginwith.Length To blurb.Length - 1
            If blurb(i) = "-" Then
                uri = uri & blurb(i)
                lastpos = i
            ElseIf blurb(i) = " " Then
                If Regex.IsMatch(blurb(i - 1), "[a-zA-Z0-9]") Then
                    Exit For
                End If
            ElseIf blurb(i) = "." Then
                If Regex.IsMatch(blurb(i - 1), "[/]") Then
                    Exit For
                Else
                    uri = uri & blurb(i)
                    lastpos = i
                End If
            Else
                If Regex.IsMatch(blurb(i), "[a-zA-Z0-9./:?=*_#]") Then
                    uri = uri & blurb(i)
                    lastpos = i
                Else
                    Exit For
                End If
            End If
        Next
        While uri.EndsWith(".")
            uri = uri.Remove(uri.Length - 1)
            lastpos = lastpos - 1
        End While
        'Debug.Print(URIPos & " -:- " & lastpos & " -:- " & uri & " -:- " & blurb.Replace(blurb.Substring(URIPos, lastpos - URIPos + 1), uri))

        Try
            If uri <> "" Then nb = blurb.Replace(blurb.Substring(URIPos, lastpos - URIPos + 1), uri)
        Catch ex As Exception
        End Try
        Return uri
    End Function

    Private Sub txtAuthor_KeyDown(sender As Object, e As KeyEventArgs) Handles txtAuthor.KeyDown
        If e.KeyCode = Keys.Down Then
            txtVolume.Focus()
            e.Handled = True
        End If
    End Sub

    Private Sub txtVolume_KeyDown(sender As Object, e As KeyEventArgs) Handles txtVolume.KeyDown
        If e.KeyCode = Keys.Down Then
            txtPage.Focus()
            e.Handled = True
        ElseIf e.KeyCode = Keys.Up Then
            txtAuthor.Focus()
            e.Handled = True
        End If
    End Sub

    Private Sub txtPage_KeyDown(sender As Object, e As KeyEventArgs) Handles txtPage.KeyDown
        If e.KeyCode = Keys.Down Then
            txtYear.Focus()
            e.Handled = True
        ElseIf e.KeyCode = Keys.Up Then
            txtVolume.Focus()
            e.Handled = True
        End If
    End Sub

    Private Sub txtYear_KeyDown(sender As Object, e As KeyEventArgs) Handles txtYear.KeyDown
        If e.KeyCode = Keys.Down Then
            txtTitle.Focus()
            e.Handled = True
        ElseIf e.KeyCode = Keys.Up Then
            txtPage.Focus()
            e.Handled = True
        End If
    End Sub

    Private Sub txtTitle_KeyDown(sender As Object, e As KeyEventArgs) Handles txtTitle.KeyDown
        If e.KeyCode = Keys.Up Then
            txtYear.Focus()
            e.Handled = True
        End If
    End Sub

    Private Function bCheckAuthor() As Boolean
        If txtAuthor.Text.StartsWith("*") Then

        ElseIf txtAuthor.Text.StartsWith("&") Then

        Else
            If txtAuthor.Text.Contains(".") Then
                If txtAuthor.Text.Split(".").Count > 2 Then
                    MsgBox("Author name must contains atmost one DOT")
                    Return False
                ElseIf txtAuthor.Text.Split(".")(0).Length <> 15 Then
                    MsgBox("Author surname must contains 15 characters before DOT")
                    Return False
                End If
            End If
        End If
        Return True
    End Function

    Private Function AllValid() As Boolean
        If Not AuthorValid() Then Return False
        If Not VolumeValid() Then Return False
        If Not PageValid() Then Return False
        If Not YearValid() Then Return False
        If Not TitleValid() Then Return False
        Return True
    End Function

    Private Function AuthorValid() As Boolean
        Return True
    End Function

    Private Function VolumeValid() As Boolean
        Return True
    End Function

    Private Function PageValid() As Boolean
        Return True
    End Function

    Private Function YearValid() As Boolean
        Return True
    End Function

    Private Function TitleValid() As Boolean
        If txtTitle.Text.Trim = vbNullString Then
            Select Case True
                Case txtAuthor.Text.Trim.StartsWith("&"), chkDCI.Checked, chkNTR.Checked, chkTitErr.Checked
                    Return True
                Case Split(lblCurrent.Text, "of")(0).Trim = Split(lblCurrent.Text, "of")(1).Trim
                    Return True
                Case Else
                    MsgBox("Title should not be empty. Please capture the title.")
                    txtTitle.Focus()
                    Return False
            End Select
        End If
        Return True
    End Function

    Private Function FindEmptyTitle() As Boolean
        Dim refno As Integer = 0
        For Each cit As XmlNode In xnlCitations
            Dim delNode As XmlNode
            delNode = cit.Attributes("D")
            Dim nod As XmlNode
            nod = cit.SelectSingleNode("CI_INFO/CI_PATENT")
            If nod Is Nothing Then
                If delNode Is Nothing Then
                    If cit.SelectSingleNode("CI_CAPTURE/CI_CAPTURE_TITLE").InnerText.Trim = "" Then
                        Dim autnod As XmlNode = cit.SelectSingleNode("CI_INFO/CI_JOURNAL/CI_AUTHOR")
                        Dim patnod As XmlNode = cit.SelectSingleNode("CI_INFO/CI_PATENT")
                        If patnod IsNot Nothing Then
                            'Return False
                        ElseIf autnod IsNot Nothing Then
                            MsgBox("Title should be captured for non Patent author")
                            subViewRef(refno)
                            Return True
                        Else
                            MsgBox("Check Title and Author fields")
                            subViewRef(refno)
                            Return True
                        End If
                    End If
                End If
            End If
            refno = refno + 1
        Next
        Return False
    End Function

    Private Sub RemoveEmptyNodes()
        Try
            Dim EmptyNodes As XmlNodeList = xmlDoc.SelectNodes("//*[not(descendant::text()[normalize-space()])]")
            For Each emptyNode As XmlNode In EmptyNodes
                Select Case emptyNode.Name
                    Case "CI_TITLE", "CI_INFO", "CI_JOURNAL"

                    Case Else
                        emptyNode.ParentNode.RemoveChild(emptyNode)
                End Select

                'If emptyNode.Name <> "CI_TITLE" Then
                '    emptyNode.ParentNode.RemoveChild(emptyNode)
                'End If
            Next
        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try
    End Sub

    Private Sub RemoveBlankLines(FullText As String)
        FullText = Regex.Replace(FullText, "^\s*$\n", "")
        xmlDoc.LoadXml(FullText)
    End Sub

    Private Sub RemoveCIInfoChildren(ByRef cit As XmlNode)
        Dim cinode As XmlNode, XNode As XmlNode

        cinode = cit.SelectSingleNode("CI_INFO")
        cinode.ParentNode.RemoveChild(cinode)
        XNode = cit.SelectSingleNode("CI_INFO")
        If XNode Is Nothing Then
            XNode = cit.OwnerDocument.CreateElement("CI_INFO")
            cit.PrependChild(XNode)
        End If
    End Sub

    Private Sub MakeCIInfoNode(cit As XmlNode)
        If txtAuthor.Text.Trim.StartsWith("&") Then
            MakeCIInfoPatent(cit)
        Else
            MakeCIInfoJournal(cit)
        End If
    End Sub

    Private Sub MakeCIInfoJournal(cit As XmlNode)
        Dim xnode As XmlNode, xnode1 As XmlNode

        xnode = cit.SelectSingleNode("CI_INFO")
        If xnode Is Nothing Then
            xnode = cit.OwnerDocument.CreateElement("CI_INFO")
            cit.PrependChild(xnode)
        End If
        xnode = cit.OwnerDocument.CreateElement("CI_JOURNAL")

        txtAuthor.Text = txtAuthor.Text.Trim
        If txtAuthor.Text <> "" Then
            xnode1 = cit.OwnerDocument.CreateElement("CI_AUTHOR")
            xnode1.InnerText = txtAuthor.Text
            xnode.AppendChild(xnode1)
        End If

        txtVolume.Text = txtVolume.Text.Trim
        If txtVolume.Text <> "" Then
            xnode1 = cit.OwnerDocument.CreateElement("CI_VOLUME")
            If IsNumeric(txtVolume.Text) Then
                xnode1.InnerText = CInt(txtVolume.Text).ToString("D4")
            Else
                xnode1.InnerText = txtVolume.Text
            End If
            xnode.AppendChild(xnode1)
        End If

        txtPage.Text = txtPage.Text.Trim
        If txtPage.Text <> "" Then
            xnode1 = cit.OwnerDocument.CreateElement("CI_PAGE")
            If IsNumeric(txtPage.Text) Then
                xnode1.InnerText = CInt(txtPage.Text).ToString("D5")
            Else
                xnode1.InnerText = txtPage.Text
            End If
            xnode.AppendChild(xnode1)
        End If

        txtYear.Text = txtYear.Text.Trim
        If txtYear.Text <> "" Then
            xnode1 = cit.OwnerDocument.CreateElement("CI_YEAR")
            xnode1.InnerText = txtYear.Text
            xnode.AppendChild(xnode1)
        End If

        Dim cd As XmlCDataSection
        xnode1 = cit.OwnerDocument.CreateElement("CI_TITLE")
        cd = cit.OwnerDocument.CreateCDataSection(" ")
        xnode1.AppendChild(cd)
        xnode.AppendChild(xnode1)

        cit.SelectSingleNode("CI_INFO").AppendChild(xnode)
    End Sub

    Private Sub MakeCIInfoPatent(cit As XmlNode)
        Dim xnode As XmlNode, xnode1 As XmlNode

        Dim attr As XmlAttribute
        xnode = cit.SelectSingleNode("CI_INFO")
        If xnode Is Nothing Then
            xnode = cit.OwnerDocument.CreateElement("CI_INFO")
            cit.PrependChild(xnode)
        End If
        xnode = cit.OwnerDocument.CreateElement("CI_PATENT")
        attr = cit.OwnerDocument.CreateAttribute("ID")
        attr.Value = txtAuthor.Text.Replace("&", "")
        xnode.Attributes.Append(attr)

        txtVolume.Text = txtVolume.Text.Trim
        If txtVolume.Text <> "" Then
            xnode1 = cit.OwnerDocument.CreateElement("PATENT_VOLUME")
            If IsNumeric(txtVolume.Text) Then
                xnode1.InnerText = CInt(txtVolume.Text).ToString("D4")
            Else
                xnode1.InnerText = txtVolume.Text
            End If
            xnode.AppendChild(xnode1)
        End If

        txtPage.Text = txtPage.Text.Trim
        If txtPage.Text <> "" Then
            xnode1 = cit.OwnerDocument.CreateElement("PATENT_PAGE")
            If IsNumeric(txtPage.Text) Then
                xnode1.InnerText = CInt(txtPage.Text).ToString("D5")
            Else
                xnode1.InnerText = txtPage.Text
            End If
            xnode.AppendChild(xnode1)
        End If

        txtYear.Text = txtYear.Text.Trim
        If txtYear.Text <> "" Then
            xnode1 = cit.OwnerDocument.CreateElement("PATENT_YEAR")
            xnode1.InnerText = txtYear.Text
            xnode.AppendChild(xnode1)
        End If

        txtTitle.Text = txtTitle.Text.Trim
        If txtTitle.Text <> "" Then
            xnode1 = cit.OwnerDocument.CreateElement("PATENT_ASSIGNEE")
            xnode1.InnerText = txtTitle.Text
            xnode.AppendChild(xnode1)
        End If
        cit.SelectSingleNode("CI_INFO").AppendChild(xnode)
    End Sub

    Private Sub AboutToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AboutToolStripMenuItem.Click
        Dim sbAbout As New System.Text.StringBuilder
        sbAbout.AppendLine("DE Master for VIL")
        sbAbout.AppendLine("Version 12.4.5")
        sbAbout.AppendLine("Stamp 1908101431")
        MsgBox(sbAbout.ToString)
    End Sub

    Private Function ReviewARTN(cit As XmlNode) As Boolean
        Dim MaxFld As Integer = 0

        'Remove incomplete fields
        For i As Integer = 4 To 1 Step -1
            If (Me.Controls.Find("txtArtn" & i, True)(0).Text.Trim = vbNullString) Xor (Me.Controls.Find("cmbArtn" & i, True)(0).Text = vbNullString) Then
                Me.Controls.Find("txtArtn" & i, True)(0).Text = vbNullString
                Me.Controls.Find("cmbArtn" & i, True)(0).Text = vbNullString
            ElseIf Me.Controls.Find("txtArtn" & i, True)(0).Text.Trim <> vbNullString Then
                If MaxFld = 0 Then MaxFld = i
            End If
        Next
        If MaxFld = 0 Then Return True

        'Remove duplicates
        For i As Integer = MaxFld To 2 Step -1
            For j As Integer = MaxFld - 1 To 1 Step -1
                If i <> j Then
                    'If Me.Controls.Find("txtArtn" & i, True)(0).Text = Me.Controls.Find("txtArtn" & j, True)(0).Text Then
                    If Me.Controls.Find("cmbArtn" & i, True)(0).Text = Me.Controls.Find("cmbArtn" & j, True)(0).Text Then
                        'Me.Controls.Find("txtArtn" & i, True)(0).Text = vbNullString
                        'Me.Controls.Find("cmbArtn" & i, True)(0).Text = vbNullString
                        MsgBox("Duplicate Identifier type found")
                        Return False
                    End If
                    'End If
                End If
            Next
        Next

        'Remove empty fields
        For i As Integer = 1 To 4
            If Me.Controls.Find("txtArtn" & i, True)(0).Text.Trim = vbNullString Then
                If i < MaxFld Then
                    Me.Controls.Find("txtArtn" & i, True)(0).Text = Me.Controls.Find("txtArtn" & MaxFld, True)(0).Text
                    Me.Controls.Find("cmbArtn" & i, True)(0).Text = Me.Controls.Find("cmbArtn" & MaxFld, True)(0).Text
                    Me.Controls.Find("txtArtn" & MaxFld, True)(0).Text = vbNullString
                    Me.Controls.Find("cmbArtn" & MaxFld, True)(0).Text = vbNullString
                    While Me.Controls.Find("txtArtn" & MaxFld, True)(0).Text = vbNullString
                        Me.Controls.Find("txtArtn" & MaxFld, True)(0).Visible = False
                        Me.Controls.Find("cmbArtn" & MaxFld, True)(0).Visible = False
                        MaxFld -= 1
                    End While
                End If
            End If
        Next

        'Reset sequence number
        For i As Integer = 1 To cit.SelectNodes("RI_CITATIONIDENTIFIER").Count
            cit.SelectNodes("RI_CITATIONIDENTIFIER")(i - 1).Attributes("seq").Value = i
        Next

        'Make sure first field visible
        If MaxFld <= 1 Then
            Me.Controls.Find("txtArtn" & 1, True)(0).Visible = True
            Me.Controls.Find("cmbArtn" & 1, True)(0).Visible = True
            Return True
        End If
        Return True
    End Function
End Class