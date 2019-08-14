Imports System.ComponentModel

Public Class frmImages
    Dim cits As Xml.XmlNodeList
    Dim pnlImg As Panel()
    'Dim pnlImages As Panel()
    Dim pbImages As PictureBox()
    Dim lblImages As Label()
    Dim cmdToCrop As Button()
    Dim cmdFromCrop As Button()
    Dim txtOCR As TextBox()
    Dim mRect As Rectangle
    Dim totSeqs As Integer
    Dim imgCount As Integer
    Dim PanelImgCount As Integer
    Dim panelCount As Integer
    Dim inClipName As String
    Private SrcX As Integer
    Private SrcY As Integer
    Private bReadClick As Boolean
    Private StartX As Integer
    Private StartY As Integer
    Private EndX As Integer
    Private EndY As Integer

    Private Sub frmImages_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Not IsNumeric(frmMain.lblTotalSeq.Text) Then Exit Sub

        cits = frmMain.getCitations()
        totSeqs = cits.Count
        If totSeqs <= 0 Then Exit Sub
        chkFullWidth.Checked = True

        'panelCount = CInt(Math.Ceiling(totSeqs / 100))
        'ReDim pnlImages(0 To panelCount - 1)
        'For i = 0 To panelCount - 1
        '    pnlImages(i) = New Panel
        '    pnlImages(i).Name = "pnlImages-" & Convert.ToString(i)
        '    pnlImages(i).AutoScroll = True
        '    Controls.Add(pnlImages(i))
        '    pnlImages(i).Location = New Point(5, 30)
        '    pnlImages(i).Width = 350
        '    pnlImages(i).Height = Me.Height - 100
        'Next

        If totSeqs < 100 Then imgCount = totSeqs Else imgCount = 100

        ReDim pnlImg(0 To imgCount - 1)
        ReDim pbImages(0 To imgCount - 1)
        ReDim txtOCR(0 To imgCount - 1)
        ReDim lblImages(0 To imgCount - 1)
        ReDim cmdToCrop(0 To imgCount - 1)
        ReDim cmdFromCrop(0 To imgCount - 1)

        LoadPanel(0, imgCount - 1)
        'pnlImages.BringToFront()
        lblCurrent.Text = "1"
        lblTotal.Text = " / " & CInt(Math.Ceiling(totSeqs / 100))

        For Each TIF As String In IO.Directory.GetFiles(frmMain.strInPath, IO.Path.GetFileNameWithoutExtension(frmMain.getInFile) & "_*.TIF", IO.SearchOption.TopDirectoryOnly)
            cbBackup.Items.Add(IO.Path.GetFileNameWithoutExtension(TIF))
        Next
    End Sub

    Private Sub LoadPanel(first As Integer, last As Integer)
        Dim ctr As Integer = 0
        Me.SuspendLayout()
        For i As Integer = 0 To Math.Min(last, 99)
            If pnlImg(i) IsNot Nothing Then pnlImg(i).Visible = False
            If cmdToCrop(i) IsNot Nothing Then cmdToCrop(i).Visible = False
            If cmdFromCrop(i) IsNot Nothing Then cmdFromCrop(i).Visible = False
            Try
                pbImages(i).ImageLocation = ""
                pbImages(i).SizeMode = PictureBoxSizeMode.Zoom
            Catch ex As Exception
            End Try
        Next
        'Me.PerformLayout()
        Try
            If pnlImg IsNot Nothing Then pnlImages.ScrollControlIntoView(pnlImg.First)
        Catch ex As Exception
        End Try
        PanelImgCount = 0
        'Me.SuspendLayout()
        For i As Integer = first To last
            PanelImgCount += 1
            If pnlImg(ctr) Is Nothing Then
                pnlImg(ctr) = New Panel
                pnlImg(ctr).Name = "pnlImg-" & i
                pnlImg(ctr).AutoSize = True
            End If
            pnlImg(ctr).Visible = True
            If pbImages(ctr) Is Nothing Then
                pbImages(ctr) = New PictureBox
                pbImages(ctr).Name = "pbImages-" & i
                pbImages(ctr).SizeMode = PictureBoxSizeMode.Zoom
                pbImages(ctr).Height = 100
                pbImages(ctr).Width = 200
            End If

            If txtOCR(ctr) Is Nothing Then
                txtOCR(ctr) = New TextBox
                txtOCR(ctr).Name = "txtOCR-" & i
                txtOCR(ctr).Multiline = True
                txtOCR(ctr).Height = 50
                txtOCR(ctr).Width = 200
                txtOCR(ctr).Font = New Font("Times New Roman", 10)
                txtOCR(ctr).ScrollBars = ScrollBars.Vertical
            End If

            If lblImages(ctr) Is Nothing Then
                lblImages(ctr) = New Label
                lblImages(ctr).Name = "lblImages-" & i
                lblImages(ctr).AutoSize = True
            End If

            If cmdToCrop(ctr) Is Nothing Then
                cmdToCrop(ctr) = New Button
                cmdToCrop(ctr).Name = "cmdToCrop-" & i
                cmdToCrop(ctr).Text = "To crop"
            End If

            If cmdFromCrop(ctr) Is Nothing Then
                cmdFromCrop(ctr) = New Button
                cmdFromCrop(ctr).Name = "cmdFromCrop-" & i
                cmdFromCrop(ctr).Text = "From crop"
            End If

            inClipName = vbNullString
            Dim RefNo As Integer
            Try
                RefNo = (CInt(lblCurrent.Text) - 1) * 100 + i
            Catch ex As Exception
                RefNo = i
            End Try
            RefNo = i
            Try
                inClipName = cits(RefNo).SelectSingleNode("CI_CAPTURE/CI_IMAGE_CLIP_NAME").InnerText
            Catch ex As Exception
            End Try

            If inClipName = vbNullString Then
                frmMain.MakeTree(cits(RefNo), "CI_CAPTURE/CI_IMAGE_CLIP_NAME", ".\" & StrConv(cits(RefNo).OwnerDocument.SelectSingleNode("//ID_ACCESSION").InnerText _
                             & cits(RefNo).OwnerDocument.SelectSingleNode("//ITEM").Attributes("ITEMNO").Value & "C_" & cits(RefNo).Attributes("seq").InnerText & ".TIF", VbStrConv.Uppercase), InsertAfter:=cits(RefNo).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_TITLE"))
                inClipName = cits(RefNo).SelectSingleNode("CI_CAPTURE/CI_IMAGE_CLIP_NAME").InnerText
            End If
            inClipName = Replace(inClipName, Split(inClipName, "_").Last, cits(RefNo).Attributes("seq").InnerText & ".TIF")
            cits(RefNo).SelectSingleNode("CI_CAPTURE/CI_IMAGE_CLIP_NAME").InnerText = inClipName
            'Dim loc As String = frmMain.getInPath() & Split(inClipName, "\").Last
            Dim loc As String = frmMain.strInPath & Split(inClipName, "\").Last

            pbImages(ctr).ImageLocation = loc
            lblImages(ctr).Text = IO.Path.GetFileNameWithoutExtension(loc)
            Try
                txtOCR(ctr).Text = cits(RefNo).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_BLURB").InnerText
            Catch ex As Exception
            End Try

            pnlImg(ctr).Controls.Add(pbImages(ctr))
            pnlImg(ctr).Controls.Add(lblImages(ctr))
            pnlImg(ctr).Controls.Add(txtOCR(ctr))

            pnlImg(ctr).BorderStyle = BorderStyle.FixedSingle
            pbImages(ctr).Location = New Point(5, lblImages(ctr).Location.Y + lblImages(ctr).Height + 5)
            txtOCR(ctr).Location = New Point(5, pbImages(ctr).Location.Y + pbImages(ctr).Height + 5)
            pnlImages.Controls.Add(pnlImg(ctr))
            pnlImages.Controls.Add(cmdToCrop(ctr))
            pnlImages.Controls.Add(cmdFromCrop(ctr))


            AddHandler cmdToCrop(ctr).Click, AddressOf Me.cmdToCrop_Click
            AddHandler cmdFromCrop(ctr).Click, AddressOf Me.cmdFromCrop_Click

            If ctr = 0 Then
                pnlImg(ctr).Location = New Point(0, 0)
            Else
                pnlImg(ctr).Top = pnlImg(ctr - 1).Bottom + 10
            End If
            cmdToCrop(ctr).Left = pnlImg(ctr).Right + 10
            cmdToCrop(ctr).Top = pnlImg(ctr).Top + 50
            cmdFromCrop(ctr).Left = pnlImg(ctr).Right + 10
            cmdFromCrop(ctr).Top = pnlImg(ctr).Top + 100

            cmdToCrop(ctr).Visible = True
            cmdFromCrop(ctr).Visible = True

            ctr += 1
        Next
        Me.PerformLayout()
    End Sub

    Private Sub cmdToCrop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim sender_Num As Integer = Split(sender.name, "-")(1)
        If Not My.Computer.FileSystem.FileExists(pbImages(sender_Num).ImageLocation) Then Exit Sub
        pbSource.ImageLocation = pbImages(sender_Num).ImageLocation
    End Sub

    Private Sub cmdFromCrop_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If pbOut.Image Is Nothing Then Exit Sub
        Dim sender_Num As Integer = Split(sender.name, "-")(1)
        pbImages(sender_Num).Image = pbOut.Image
        'If pbOut.Width < 200 And pbOut.Height < 200 Then
        '    pbImages(sender_Num).SizeMode = PictureBoxSizeMode.Normal
        'Else
        '    pbImages(sender_Num).SizeMode = PictureBoxSizeMode.StretchImage
        'End If
        pbImages(sender_Num).SizeMode = PictureBoxSizeMode.Normal
    End Sub

    Private Sub pbSource_MouseDown(sender As Object, e As MouseEventArgs) Handles pbSource.MouseDown
        If (pbSource.ImageLocation = String.Empty) And (pbSource.Image Is Nothing) Then Exit Sub
        If e.Button = Windows.Forms.MouseButtons.Left Then
            SrcX = e.X
            SrcY = e.Y
            If bReadClick Then
                ToolTip1.Hide(pbSource)
                If (StartX = -1) Or (StartY = -1) Then
                    StartX = e.X
                    StartY = e.Y
                    LBSetStart.Enabled = False
                    LBSetEnd.Enabled = True
                    LBCut.Enabled = False
                    bReadClick = False
                    mRect = New Rectangle(e.X, e.Y, 5, 5)
                    pbSource.Invalidate()
                Else
                    EndX = e.X
                    EndY = e.Y
                    LBSetStart.Enabled = False
                    LBSetEnd.Enabled = False
                    LBCut.Enabled = True
                    bReadClick = False
                    mRect = New Rectangle(e.X, e.Y, 5, 5)
                    pbSource.Invalidate()
                End If
            Else
                If chkFullWidth.Checked Then
                    mRect = New Rectangle(0, e.Y, pbSource.Width, 0)
                Else
                    mRect = New Rectangle(e.X, e.Y, 0, 0)
                End If
                pbSource.Invalidate()
            End If
            'lblTest.Text = "Mouse Down : " & e.X & " , " & e.Y
        End If
    End Sub

    Private Sub pbSource_MouseMove(sender As Object, e As MouseEventArgs) Handles pbSource.MouseMove
        If (pbSource.ImageLocation = String.Empty) And (pbSource.Image Is Nothing) Then Exit Sub
        If e.Button = Windows.Forms.MouseButtons.Left Then
            Dim L As Integer, T As Integer, W As Integer, H As Integer

            T = Math.Min(SrcY, e.Y)
            H = Math.Abs(SrcY - e.Y)

            If chkFullWidth.Checked Then
                mRect = New Rectangle(0, T, pbSource.Width, H)
                'mRect = New Rectangle(0, mRect.Top, pbSource.Width, e.Y - mRect.Top)
            Else
                L = Math.Min(SrcX, e.X)
                W = Math.Abs(SrcX - e.X)
                mRect = New Rectangle(L, T, W, H)
                'mRect = New Rectangle(mRect.Left, mRect.Top, e.X - mRect.Left, e.Y - mRect.Top)
            End If
            pbSource.Invalidate()
            'lblTest.Text = "Mouse Move : " & e.X & " , " & e.Y & " , " & Me.Left & " , " & Me.Top & " , " & Me.Height & " , " & Me.Width
        End If
    End Sub

    Private Sub pbSource_MouseUp(sender As Object, e As MouseEventArgs) Handles pbSource.MouseUp
        If (pbSource.ImageLocation = String.Empty) And (pbSource.Image Is Nothing) Then Exit Sub
        If mRect.IsEmpty Then Exit Sub
        cmdCrop.PerformClick()
    End Sub

    Private Sub pbSource_Paint(sender As Object, e As PaintEventArgs) Handles pbSource.Paint
        If mRect.IsEmpty Then Exit Sub
        Using pen As New Pen(Color.Red, 1)
            e.Graphics.DrawRectangle(pen, mRect)
        End Using
    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef msg As Message, ByVal keyData As Keys) As Boolean
        If keyData = Keys.Up Then
            mRect.Location = New Point(mRect.Location.X, mRect.Location.Y - 1)
            pbSource.Invalidate()
            cmdCrop.PerformClick()
            Return True
        End If
        If keyData = Keys.Down Then
            mRect.Location = New Point(mRect.Location.X, mRect.Location.Y + 1)
            pbSource.Invalidate()
            cmdCrop.PerformClick()
            Return True
        End If
        If keyData = Keys.Left Then
            mRect.Location = New Point(mRect.Location.X - 1, mRect.Location.Y)
            pbSource.Invalidate()
            cmdCrop.PerformClick()
            Return True
        End If
        If keyData = Keys.Right Then
            mRect.Location = New Point(mRect.Location.X + 1, mRect.Location.Y)
            pbSource.Invalidate()
            cmdCrop.PerformClick()
            Return True
        End If
        Return MyBase.ProcessCmdKey(msg, keyData)
    End Function

    Private Sub cmdCrop_Click(sender As Object, e As EventArgs) Handles cmdCrop.Click
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
            pbOut.Image = CropImage
        End Using
    End Sub

    Private Sub cmdSave_Click(sender As Object, e As EventArgs) Handles cmdSave.Click
        For i As Integer = 0 To PanelImgCount - 1
            If pbImages(i).Image IsNot Nothing Then
                If pbImages(i).Image.Width > 100 Then
                    Dim strPath As String = frmMain.strInPath & lblImages(i).Text & ".TIF"
                    If My.Computer.FileSystem.FileExists(strPath) Then
                        Try
                            My.Computer.FileSystem.DeleteFile(strPath)
                        Catch ex As Exception
                            MsgBox("Could not delete img: " & strPath)
                        End Try
                    End If

                    Try
                        pbImages(i).Image.Save(strPath)
                    Catch ex As Exception
                        MsgBox("Could not save img: " & strPath)
                    End Try
                End If
            End If
        Next
        MsgBox("Images saved")
    End Sub

    Private Sub cmdUseCrop_Click(sender As Object, e As EventArgs) Handles cmdUseCrop.Click
        frmMain.pbImage.Image = pbOut.Image
    End Sub

    Private Sub cmdLoadBUp_Click(sender As Object, e As EventArgs) Handles cmdLoadBUp.Click
        If cbBackup.SelectedItem = vbNullString Then
            MsgBox("Select file")
            Exit Sub
        Else
            Try
                pbSource.ImageLocation = frmMain.strInPath & cbBackup.SelectedItem & ".TIF"
                spCropper.Panel1.AutoScrollPosition = New Point(0, 0)
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try

        End If
    End Sub

    Private Sub lblFirst_Click(sender As Object, e As EventArgs) Handles lblFirst.Click
        'pnlImages.First.BringToFront()
        cmdSave.PerformClick()
        If totSeqs < 100 Then imgCount = totSeqs Else imgCount = 100
        LoadPanel(0, imgCount - 1)
        lblCurrent.Text = "1"
    End Sub

    Private Sub lblLast_Click(sender As Object, e As EventArgs) Handles lblLast.Click
        'pnlImages.Last.BringToFront()
        cmdSave.PerformClick()
        lblCurrent.Text = Convert.ToString(Math.Ceiling(totSeqs / 100))
        LoadPanel(CInt(lblCurrent.Text - 1) * 100, totSeqs - 1)
    End Sub

    Private Sub lblPre_Click(sender As Object, e As EventArgs) Handles lblPre.Click
        cmdSave.PerformClick()
        If lblCurrent.Text = "1" Then
            Exit Sub
        End If
        'pnlImages(CInt(lblCurrent.Text - 2)).BringToFront()
        LoadPanel(CInt(lblCurrent.Text - 2) * 100, CInt(lblCurrent.Text - 2) * 100 + 99)
        lblCurrent.Text = CInt(lblCurrent.Text) - 1
    End Sub

    Private Sub lblNext_Click(sender As Object, e As EventArgs) Handles lblNext.Click
        cmdSave.PerformClick()
        If lblCurrent.Text = Convert.ToString(Math.Ceiling(totSeqs / 100)) Then
            Exit Sub
        End If
        'pnlImages(CInt(lblCurrent.Text)).BringToFront()
        If CInt(lblCurrent.Text) * 100 + 99 <= totSeqs Then
            LoadPanel(CInt(lblCurrent.Text) * 100, CInt(lblCurrent.Text) * 100 + 99)
        Else
            LoadPanel(CInt(lblCurrent.Text) * 100, totSeqs - 1)
        End If

        lblCurrent.Text = CInt(lblCurrent.Text) + 1
    End Sub

    Private Sub ChkMark_CheckedChanged(sender As Object, e As EventArgs) Handles ChkMark.CheckedChanged
        If ChkMark.Checked Then
            LBSetStart.Visible = True
            LBSetEnd.Visible = True
            LBCut.Visible = True
            LBSetStart.Enabled = True
            LBSetEnd.Enabled = False
            LBCut.Enabled = False
        Else
            LBSetStart.Visible = False
            LBSetEnd.Visible = False
            LBCut.Visible = False
        End If
    End Sub

    Private Sub LBSetStart_Click(sender As Object, e As EventArgs) Handles LBSetStart.Click
        ToolTip1.Show("Now click on Image to Set Start position", pbSource)
        bReadClick = True
        LBSetStart.Enabled = False
        LBSetEnd.Enabled = False
    End Sub

    Private Sub LBSetEnd_Click(sender As Object, e As EventArgs) Handles LBSetEnd.Click
        ToolTip1.Show("Now click on Image to Set End position", pbSource)
        bReadClick = True
        LBSetStart.Enabled = False
        LBSetEnd.Enabled = False
    End Sub

    Private Sub LBCut_Click(sender As Object, e As EventArgs) Handles LBCut.Click
        If StartX = -1 Or StartY = -1 Or EndX = -1 Or EndY = -1 Then
            MsgBox("Invalid values. Mark again")
            Exit Sub
        End If
        Dim L As Integer, T As Integer, W As Integer, H As Integer
        T = Math.Min(StartY, EndY)
        H = Math.Abs(StartY - EndY)
        L = Math.Min(StartX, EndX)
        W = Math.Abs(StartX - EndX)
        If chkFullWidth.Checked Then
            mRect = New Rectangle(0, T, pbSource.Width, H)
        Else
            mRect = New Rectangle(L, T, W, H)
        End If
        pbSource.Invalidate()
        cmdCrop.PerformClick()
        LBSetStart.Enabled = True
        LBSetEnd.Enabled = False
        LBCut.Enabled = False
        StartX = -1 : StartY = -1 : EndX = -1 : EndY = -1
    End Sub

    Private Sub frmImages_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If FormMode.chosenTool = ToolMode.OP Then
            frmMain.cbBackup.Items.Clear()
            For Each TIF As String In IO.Directory.GetFiles(frmMain.strInPath, IO.Path.GetFileNameWithoutExtension(frmMain.getInFile) & "_*.TIF", IO.SearchOption.TopDirectoryOnly)
                frmMain.cbBackup.Items.Add(IO.Path.GetFileNameWithoutExtension(TIF))
            Next
        End If
    End Sub
End Class