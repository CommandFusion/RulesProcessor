<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
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
        Me.btnRun = New System.Windows.Forms.Button
        Me.dlgSaveFile = New System.Windows.Forms.SaveFileDialog
        Me.SuspendLayout()
        '
        'btnRun
        '
        Me.btnRun.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnRun.Location = New System.Drawing.Point(13, 13)
        Me.btnRun.Name = "btnRun"
        Me.btnRun.Size = New System.Drawing.Size(251, 23)
        Me.btnRun.TabIndex = 0
        Me.btnRun.Text = "Save To File"
        Me.btnRun.UseVisualStyleBackColor = True
        '
        'dlgSaveFile
        '
        Me.dlgSaveFile.DefaultExt = "cfrls"
        Me.dlgSaveFile.Filter = "Rules Files|*.cfrls"
        Me.dlgSaveFile.Title = "Save Rules to..."
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(276, 96)
        Me.Controls.Add(Me.btnRun)
        Me.Name = "frmMain"
        Me.Text = "Rules Processor Example"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents btnRun As System.Windows.Forms.Button
    Friend WithEvents dlgSaveFile As System.Windows.Forms.SaveFileDialog

End Class
