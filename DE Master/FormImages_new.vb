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
    Dim panelCount As Integer

    Private Sub frmImages_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        If Not IsNumeric(frmMain.lblTotalSeq.Text) Then Exit Sub

        cits = frmMain.getCitations()
        totSeqs = cits.Count
        If totSeqs <= 0 Then Exit Sub
        chkFullWidth.Checked = True
        Dim inClipName As String
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
        Dim imgCount As Integer

        If totSeqs < 100 Then imgCount = totSeqs Else imgCount = 100

        ReDim pnlImg(0 To imgCount - 1)
        ReDim pbImages(0 To imgCount - 1)
        ReDim txtOCR(0 To imgCount - 1)
        ReDim lblImages(0 To imgCount - 1)
        ReDim cmdToCrop(0 To imgCount - 1)
        ReDim cmdFromCrop(0 To imgCount - 1)

        For i As Integer = 0 To imgCount - 1
            If pnlImg(i) Is Nothing Then
                pnlImg(i) = New Panel
                pnlImg(i).Name = "pnlImg-" & i
                pnlImg(i).AutoSize = True
            End If

            If pbImages(i) Is Nothing Then
                pbImages(i) = New PictureBox
                pbImages(i).Name = "pbImages-" & i
                pbImages(i).SizeMode = PictureBoxSizeMode.Zoom
                pbImages(i).Height = 100
                pbImages(i).Width = 200
            End If

            If txtOCR(i) Is Nothing Then
                txtOCR(i) = New TextBox
                txtOCR(i).Name = "txtOCR-" & i
                txtOCR(i).Multiline = True
                txtOCR(i).Height = 50
                txtOCR(i).Width = 200
                txtOCR(i).Font = New Font("Times New Roman", 10)
                txtOCR(i).ScrollBars = ScrollBars.Vertical
            End If

            If lblImages(i) Is Nothing Then
                lblImages(i) = New Label
                lblImages(i).Name = "lblImages-" & i
                lblImages(i).AutoSize = True
            End If

            If cmdToCrop(i) Is Nothing Then
                cmdToCrop(i) = New Button
                cmdToCrop(i).Name = "cmdToCrop-" & i
                cmdToCrop(i).Text = "To crop"
            End If

            If cmdFromCrop(i) Is Nothing Then
                cmdFromCrop(i) = New Button
                cmdFromCrop(i).Name = "cmdFromCrop-" & i
                cmdFromCrop(i).Text = "From crop"
            End If

            'pnlImg(i).AutoScroll = True

            inClipName = vbNullString
            Dim RefNo As Integer
            Try
                RefNo = (CInt(lblCurrent.Text) - 1) * 100 + i
            Catch ex As Exception
                RefNo = i
            End Try

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

            pbImages(i).ImageLocation = loc
            lblImages(i).Text = IO.Path.GetFileNameWithoutExtension(loc)
            Try
                txtOCR(i).Text = cits(RefNo).SelectSingleNode("CI_CAPTURE/CI_CAPTURE_BLURB").InnerText
            Catch ex As Exception
            End Try

            pnlImg(i).Controls.Add(pbImages(i))
            pnlImg(i).Controls.Add(lblImages(i))
            pnlImg(i).Controls.Add(txtOCR(i))

            pnlImg(i).BorderStyle = BorderStyle.FixedSingle
            pbImages(i).Location = New Point(5, lblImages(i).Location.Y + lblImages(i).Height + 5)
            txtOCR(i).Location = New Point(5, pbImages(i).Location.Y + pbImages(i).Height + 5)
            pnlImages.Controls.Add(pnlImg(i))
            pnlImages.Controls.Add(cmdToCrop(i))
            pnlImages.Controls.Add(cmdFromCrop(i))

            AddHandler cmdToCrop(i).Click, AddressOf Me.cmdToCrop_Click
            AddHandler cmdFromCrop(i).Click, AddressOf Me.cmdFromCrop_Click

            If i = 0 Then
                pnlImg(i).Location = New Point(0, 0)
            Else
                pnlImg(i).Top = pnlImg(i - 1).Bottom + 10
            End If
            cmdToCrop(i).Left = pnlImg(i).Right + 10
            cmdToCrop(i).Top = pnlImg(i).Top + 50
            cmdFromCrop(i).Left = pnlImg(i).Right + 10
            cmdFromCrop(i).Top = pnlImg(i).Top + 100
        Next
        pnlImages.BringToFront()
        lblCurrent.Text = "1"
        lblTotal.Text = " / " & CInt(Math.Ceiling(totSeqs / 100))

        For Each TIF As String In IO.Directory.GetFiles(frmMain.strInPath, IO.Path.GetFileNameWithoutExtension(frmMain.getInFile) & "_*.TIF", IO.SearchOption.TopDirectoryOnly)
            cbBackup.Items.Add(IO.Path.GetFileNameWithoutExtension(TIF))
        Next
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
            If chkFullWidth.Checked Then
                mRect = New Rectangle(0, e.Y, pbSource.Width, 0)
            Else
                mRect = New Rectangle(e.X, e.Y, 0, 0)
            End If
            pbSource.Invalidate()
            lblTest.Text = "Mouse Down : " & e.X & " , " & e.Y
        End If
    End Sub

    Private Sub pbSource_MouseMove(sender As Object, e As MouseEventArgs) Handles pbSource.MouseMove
        If (pbSource.ImageLocation = String.Empty) And (pbSource.Image Is Nothing) Then Exit Sub
        If e.Button = Windows.Forms.MouseButtons.Left Then
            If chkFullWidth.Checked Then
                mRect = New Rectangle(0, mRect.Top, pbSource.Width, e.Y - mRect.Top)
            Else
                mRect = New Rectangle(mRect.Left, mRect.Top, e.X - mRect.Left, e.Y - mRect.Top)
            End If
            pbSource.Invalidate()
            lblTest.Text = "Mouse Move : " & e.X & " , " & e.Y
        End If
    End Sub

    Private Sub pbSource_MouseUp(sender As Object, e As MouseEventArgs) Handles pbSource.MouseUp
        If (pbSource.ImageLocation = String.Empty) And (pbSource.Image Is Nothing) Then Exit Sub
        If e.Button = Windows.Forms.MouseButtons.Left Then
            lblTest.Text = "Mouse Up : " & e.X & " , " & e.Y
        End If
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
        For i As Integer = 0 To totSeqs - 1
            If pbImages(i).Image IsNot Nothing Then
                If pbImages(i).Image.Width > 100 Then
                    Dim strPath As String = frmMain.strInPath & lblImages(i).Text & ".TIF"
                    If My.Computer.FileSystem.FileExists(strPath) Then
                        My.Computer.FileSystem.DeleteFile(strPath)
                    End If
                    pbImages(i).Image.Save(strPath)
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
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
        End If
    End Sub

    Private Sub lblFirst_Click(sender As Object, e As EventArgs) Handles lblFirst.Click
        'todo pnlImages.First.BringToFront()
        lblCurrent.Text = "1"
    End Sub

    Private Sub lblLast_Click(sender As Object, e As EventArgs) Handles lblLast.Click
        'todo pnlImages.Last.BringToFront()
        lblCurrent.Text = CInt(Math.Ceiling(totSeqs / 100))
    End Sub

    Private Sub lblPre_Click(sender As Object, e As EventArgs) Handles lblPre.Click
        If lblCurrent.Text = "1" Then
            Exit Sub
        End If
        'todo pnlImages(CInt(lblCurrent.Text - 2)).BringToFront()
        lblCurrent.Text = CInt(lblCurrent.Text) - 1
    End Sub

    Private Sub lblNext_Click(sender As Object, e As EventArgs) Handles lblNext.Click
        If lblCurrent.Text = Convert.ToString(CInt(Math.Ceiling(totSeqs / 100))) Then
            Exit Sub
        End If
        'todo pnlImages(CInt(lblCurrent.Text)).BringToFront()
        lblCurrent.Text = CInt(lblCurrent.Text) + 1
    End Sub

    Private Sub pbSource_MouseLeave(sender As Object, e As EventArgs) Handles pbSource.MouseLeave
        lblTest.Text = "Mouse Leave : " & e.ToString & " , " & sender.ToString
    End Sub
End Class