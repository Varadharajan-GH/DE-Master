<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmImages
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmImages))
        Me.pnlImages = New System.Windows.Forms.Panel()
        Me.pnlCropper = New System.Windows.Forms.Panel()
        Me.spCropper = New System.Windows.Forms.SplitContainer()
        Me.pbSource = New System.Windows.Forms.PictureBox()
        Me.pbOut = New System.Windows.Forms.PictureBox()
        Me.cmdCrop = New System.Windows.Forms.Button()
        Me.chkFullWidth = New System.Windows.Forms.CheckBox()
        Me.cmdSave = New System.Windows.Forms.Button()
        Me.cmdUseCrop = New System.Windows.Forms.Button()
        Me.cbBackup = New System.Windows.Forms.ComboBox()
        Me.cmdLoadBUp = New System.Windows.Forms.Button()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.lblTotal = New System.Windows.Forms.Label()
        Me.lblLast = New System.Windows.Forms.Label()
        Me.lblNext = New System.Windows.Forms.Label()
        Me.lblCurrent = New System.Windows.Forms.Label()
        Me.lblPre = New System.Windows.Forms.Label()
        Me.lblFirst = New System.Windows.Forms.Label()
        Me.ChkMark = New System.Windows.Forms.CheckBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.LBSetStart = New System.Windows.Forms.Label()
        Me.LBSetEnd = New System.Windows.Forms.Label()
        Me.LBCut = New System.Windows.Forms.Label()
        Me.pnlCropper.SuspendLayout()
        CType(Me.spCropper, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.spCropper.Panel1.SuspendLayout()
        Me.spCropper.Panel2.SuspendLayout()
        Me.spCropper.SuspendLayout()
        CType(Me.pbSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.pbOut, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.Panel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlImages
        '
        Me.pnlImages.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.pnlImages.AutoScroll = True
        Me.pnlImages.Location = New System.Drawing.Point(5, 30)
        Me.pnlImages.Name = "pnlImages"
        Me.pnlImages.Size = New System.Drawing.Size(350, 475)
        Me.pnlImages.TabIndex = 0
        '
        'pnlCropper
        '
        Me.pnlCropper.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.pnlCropper.AutoScroll = True
        Me.pnlCropper.Controls.Add(Me.spCropper)
        Me.pnlCropper.Location = New System.Drawing.Point(361, 4)
        Me.pnlCropper.Name = "pnlCropper"
        Me.pnlCropper.Size = New System.Drawing.Size(409, 500)
        Me.pnlCropper.TabIndex = 1
        '
        'spCropper
        '
        Me.spCropper.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.spCropper.Location = New System.Drawing.Point(3, 6)
        Me.spCropper.Name = "spCropper"
        Me.spCropper.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'spCropper.Panel1
        '
        Me.spCropper.Panel1.AutoScroll = True
        Me.spCropper.Panel1.Controls.Add(Me.pbSource)
        '
        'spCropper.Panel2
        '
        Me.spCropper.Panel2.AutoScroll = True
        Me.spCropper.Panel2.Controls.Add(Me.pbOut)
        Me.spCropper.Size = New System.Drawing.Size(403, 489)
        Me.spCropper.SplitterDistance = 347
        Me.spCropper.TabIndex = 0
        '
        'pbSource
        '
        Me.pbSource.Location = New System.Drawing.Point(3, 3)
        Me.pbSource.Name = "pbSource"
        Me.pbSource.Size = New System.Drawing.Size(100, 50)
        Me.pbSource.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.pbSource.TabIndex = 0
        Me.pbSource.TabStop = False
        '
        'pbOut
        '
        Me.pbOut.Location = New System.Drawing.Point(3, 3)
        Me.pbOut.Name = "pbOut"
        Me.pbOut.Size = New System.Drawing.Size(100, 50)
        Me.pbOut.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.pbOut.TabIndex = 0
        Me.pbOut.TabStop = False
        '
        'cmdCrop
        '
        Me.cmdCrop.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdCrop.Location = New System.Drawing.Point(780, 30)
        Me.cmdCrop.Name = "cmdCrop"
        Me.cmdCrop.Size = New System.Drawing.Size(75, 23)
        Me.cmdCrop.TabIndex = 2
        Me.cmdCrop.Text = "Crop"
        Me.cmdCrop.UseVisualStyleBackColor = True
        '
        'chkFullWidth
        '
        Me.chkFullWidth.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.chkFullWidth.AutoSize = True
        Me.chkFullWidth.Location = New System.Drawing.Point(780, 69)
        Me.chkFullWidth.Name = "chkFullWidth"
        Me.chkFullWidth.Size = New System.Drawing.Size(73, 17)
        Me.chkFullWidth.TabIndex = 3
        Me.chkFullWidth.Text = "Full Width"
        Me.chkFullWidth.UseVisualStyleBackColor = True
        '
        'cmdSave
        '
        Me.cmdSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdSave.Location = New System.Drawing.Point(780, 149)
        Me.cmdSave.Name = "cmdSave"
        Me.cmdSave.Size = New System.Drawing.Size(75, 34)
        Me.cmdSave.TabIndex = 4
        Me.cmdSave.Text = "&Save Images"
        Me.cmdSave.UseVisualStyleBackColor = True
        '
        'cmdUseCrop
        '
        Me.cmdUseCrop.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdUseCrop.Location = New System.Drawing.Point(780, 199)
        Me.cmdUseCrop.Name = "cmdUseCrop"
        Me.cmdUseCrop.Size = New System.Drawing.Size(75, 56)
        Me.cmdUseCrop.TabIndex = 5
        Me.cmdUseCrop.Text = "Use Cropped Image"
        Me.cmdUseCrop.UseVisualStyleBackColor = True
        '
        'cbBackup
        '
        Me.cbBackup.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cbBackup.FormattingEnabled = True
        Me.cbBackup.Location = New System.Drawing.Point(779, 271)
        Me.cbBackup.Name = "cbBackup"
        Me.cbBackup.Size = New System.Drawing.Size(120, 21)
        Me.cbBackup.TabIndex = 6
        '
        'cmdLoadBUp
        '
        Me.cmdLoadBUp.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdLoadBUp.Location = New System.Drawing.Point(780, 303)
        Me.cmdLoadBUp.Name = "cmdLoadBUp"
        Me.cmdLoadBUp.Size = New System.Drawing.Size(75, 25)
        Me.cmdLoadBUp.TabIndex = 7
        Me.cmdLoadBUp.Text = "Load Image"
        Me.cmdLoadBUp.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.lblTotal)
        Me.Panel1.Controls.Add(Me.lblLast)
        Me.Panel1.Controls.Add(Me.lblNext)
        Me.Panel1.Controls.Add(Me.lblCurrent)
        Me.Panel1.Controls.Add(Me.lblPre)
        Me.Panel1.Controls.Add(Me.lblFirst)
        Me.Panel1.Location = New System.Drawing.Point(83, 3)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(187, 25)
        Me.Panel1.TabIndex = 8
        '
        'lblTotal
        '
        Me.lblTotal.AutoSize = True
        Me.lblTotal.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTotal.Location = New System.Drawing.Point(92, 5)
        Me.lblTotal.Name = "lblTotal"
        Me.lblTotal.Size = New System.Drawing.Size(25, 13)
        Me.lblTotal.TabIndex = 5
        Me.lblTotal.Text = "/ #"
        '
        'lblLast
        '
        Me.lblLast.AutoSize = True
        Me.lblLast.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblLast.Location = New System.Drawing.Point(161, 5)
        Me.lblLast.Name = "lblLast"
        Me.lblLast.Size = New System.Drawing.Size(21, 13)
        Me.lblLast.TabIndex = 4
        Me.lblLast.Text = ">>"
        '
        'lblNext
        '
        Me.lblNext.AutoSize = True
        Me.lblNext.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblNext.Location = New System.Drawing.Point(127, 5)
        Me.lblNext.Name = "lblNext"
        Me.lblNext.Size = New System.Drawing.Size(14, 13)
        Me.lblNext.TabIndex = 3
        Me.lblNext.Text = ">"
        '
        'lblCurrent
        '
        Me.lblCurrent.AutoSize = True
        Me.lblCurrent.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCurrent.Location = New System.Drawing.Point(69, 5)
        Me.lblCurrent.Name = "lblCurrent"
        Me.lblCurrent.Size = New System.Drawing.Size(15, 13)
        Me.lblCurrent.TabIndex = 2
        Me.lblCurrent.Text = "#"
        '
        'lblPre
        '
        Me.lblPre.AutoSize = True
        Me.lblPre.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPre.Location = New System.Drawing.Point(44, 5)
        Me.lblPre.Name = "lblPre"
        Me.lblPre.Size = New System.Drawing.Size(14, 13)
        Me.lblPre.TabIndex = 1
        Me.lblPre.Text = "<"
        '
        'lblFirst
        '
        Me.lblFirst.AutoSize = True
        Me.lblFirst.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblFirst.Location = New System.Drawing.Point(3, 5)
        Me.lblFirst.Name = "lblFirst"
        Me.lblFirst.Size = New System.Drawing.Size(21, 13)
        Me.lblFirst.TabIndex = 0
        Me.lblFirst.Text = "<<"
        '
        'ChkMark
        '
        Me.ChkMark.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ChkMark.AutoSize = True
        Me.ChkMark.Location = New System.Drawing.Point(781, 98)
        Me.ChkMark.Name = "ChkMark"
        Me.ChkMark.Size = New System.Drawing.Size(50, 17)
        Me.ChkMark.TabIndex = 10
        Me.ChkMark.Text = "Mark"
        Me.ToolTip1.SetToolTip(Me.ChkMark, resources.GetString("ChkMark.ToolTip"))
        Me.ChkMark.UseVisualStyleBackColor = True
        '
        'LBSetStart
        '
        Me.LBSetStart.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LBSetStart.AutoSize = True
        Me.LBSetStart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LBSetStart.Location = New System.Drawing.Point(783, 124)
        Me.LBSetStart.Name = "LBSetStart"
        Me.LBSetStart.Size = New System.Drawing.Size(12, 15)
        Me.LBSetStart.TabIndex = 11
        Me.LBSetStart.Text = "["
        Me.ToolTip1.SetToolTip(Me.LBSetStart, "Set Start")
        Me.LBSetStart.Visible = False
        '
        'LBSetEnd
        '
        Me.LBSetEnd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LBSetEnd.AutoSize = True
        Me.LBSetEnd.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LBSetEnd.Location = New System.Drawing.Point(801, 124)
        Me.LBSetEnd.Name = "LBSetEnd"
        Me.LBSetEnd.Size = New System.Drawing.Size(12, 15)
        Me.LBSetEnd.TabIndex = 12
        Me.LBSetEnd.Text = "]"
        Me.ToolTip1.SetToolTip(Me.LBSetEnd, "Set End")
        Me.LBSetEnd.Visible = False
        '
        'LBCut
        '
        Me.LBCut.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LBCut.AutoSize = True
        Me.LBCut.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LBCut.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LBCut.Location = New System.Drawing.Point(819, 124)
        Me.LBCut.Name = "LBCut"
        Me.LBCut.Size = New System.Drawing.Size(17, 15)
        Me.LBCut.TabIndex = 13
        Me.LBCut.Text = "X"
        Me.ToolTip1.SetToolTip(Me.LBCut, "Crop selected portion")
        Me.LBCut.Visible = False
        '
        'frmImages
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(911, 511)
        Me.Controls.Add(Me.LBCut)
        Me.Controls.Add(Me.LBSetEnd)
        Me.Controls.Add(Me.LBSetStart)
        Me.Controls.Add(Me.ChkMark)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.cmdLoadBUp)
        Me.Controls.Add(Me.cbBackup)
        Me.Controls.Add(Me.cmdUseCrop)
        Me.Controls.Add(Me.cmdSave)
        Me.Controls.Add(Me.chkFullWidth)
        Me.Controls.Add(Me.cmdCrop)
        Me.Controls.Add(Me.pnlCropper)
        Me.Controls.Add(Me.pnlImages)
        Me.Name = "frmImages"
        Me.Text = "All Images"
        Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
        Me.pnlCropper.ResumeLayout(False)
        Me.spCropper.Panel1.ResumeLayout(False)
        Me.spCropper.Panel1.PerformLayout()
        Me.spCropper.Panel2.ResumeLayout(False)
        Me.spCropper.Panel2.PerformLayout()
        CType(Me.spCropper, System.ComponentModel.ISupportInitialize).EndInit()
        Me.spCropper.ResumeLayout(False)
        CType(Me.pbSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.pbOut, System.ComponentModel.ISupportInitialize).EndInit()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents pnlImages As System.Windows.Forms.Panel
    Friend WithEvents pnlCropper As System.Windows.Forms.Panel
    Friend WithEvents pbOut As System.Windows.Forms.PictureBox
    Friend WithEvents pbSource As System.Windows.Forms.PictureBox
    Friend WithEvents spCropper As System.Windows.Forms.SplitContainer
    Friend WithEvents cmdCrop As System.Windows.Forms.Button
    Friend WithEvents chkFullWidth As System.Windows.Forms.CheckBox
    Friend WithEvents cmdSave As System.Windows.Forms.Button
    Friend WithEvents cmdUseCrop As System.Windows.Forms.Button
    Friend WithEvents cbBackup As System.Windows.Forms.ComboBox
    Friend WithEvents cmdLoadBUp As System.Windows.Forms.Button
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents lblLast As System.Windows.Forms.Label
    Friend WithEvents lblNext As System.Windows.Forms.Label
    Friend WithEvents lblCurrent As System.Windows.Forms.Label
    Friend WithEvents lblPre As System.Windows.Forms.Label
    Friend WithEvents lblFirst As System.Windows.Forms.Label
    Friend WithEvents lblTotal As System.Windows.Forms.Label
    Friend WithEvents ChkMark As CheckBox
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents LBSetStart As Label
    Friend WithEvents LBSetEnd As Label
    Friend WithEvents LBCut As Label
End Class
