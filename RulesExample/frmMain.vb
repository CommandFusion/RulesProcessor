Imports CommandFusion.Rules

Public Class frmMain

    Private rulesProcess As New RulesProcessor
    Private rulesCol As New RulesCollection

    Private Sub btnTestGet_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRun.Click

        ' Create a few dummy rules and macros


        ' DINMOD4 RULES

        'Dim macro1 = New RuleMacro("CF Mini relay 1 pulse then relay 2 pulse")
        'macro1.actions.Add(New RuleMacroAction(0, "\xF2\x20\xF3TRLYSET\xF4P01:P:00010\xF5\xF5"))
        'macro1.actions.Add(New RuleMacroAction(2000, "\xF2\x20\xF3TRLYSET\xF4P02:P:00010\xF5\xF5"))

        'Dim macro2 = New RuleMacro("CF Mini relay 3 toggle twice")
        'macro2.actions.Add(New RuleMacroAction(0, "\xF2\x20\xF3TRLYSET\xF4P03:T\xF5\xF5"))
        'macro2.actions.Add(New RuleMacroAction(2000, "\xF2\x20\xF3TRLYSET\xF4P03:T\xF5\xF5"))

        'Dim macro3 = New RuleMacro("Send To TCP Server")
        'macro3.actions.Add(New RuleMacroAction(500, "\xF2\x02\xF3TLANSND\xF412:Starting macro...\x0D\xF5\xF5"))
        'macro3.actions.Add(New RuleMacroAction(3000, "\xF2\x02\xF3TLANSND\xF412:Data 3 seconds later\x0D\xF5\xF5"))
        'macro3.actions.Add(New RuleMacroAction(500, "\xF2\x02\xF3TLANSND\xF412:Data 0.5 seconds later\x0D\xF5\xF5"))
        'macro3.actions.Add(New RuleMacroAction(500, "\xF2\x02\xF3TLANSND\xF412:Data 0.5 seconds later\x0D\xF5\xF5"))
        'macro3.actions.Add(New RuleMacroAction(500, "\xF2\x02\xF3TLANSND\xF412:Data 0.5 seconds later\x0D\xF5\xF5"))
        'macro3.actions.Add(New RuleMacroAction(500, "\xF2\x02\xF3TLANSND\xF412:Data 0.5 seconds later\x0D\xF5\xF5"))
        'macro3.actions.Add(New RuleMacroAction(500, "\xF2\x02\xF3TLANSND\xF412:Data 0.5 seconds later\x0D\xF5\xF5"))
        'macro3.actions.Add(New RuleMacroAction(500, "\xF2\x02\xF3TLANSND\xF412:Finished macro\x0D\xF5\xF5"))

        'rulesCol.macros.Add(macro1)
        'rulesCol.macros.Add(macro2)
        'rulesCol.macros.Add(macro3)

        'rulesCol.rules.Add(New Rule("Relay 1 triggered Module 3", "\xF2\x11\xF3*RLYSTA*M3*P01:1*", macro1.name))
        'rulesCol.rules.Add(New Rule("Relay 2 triggered Module 1", "\xF2\x11\xF3.RLYSTA.M1.*P02:1*", macro2.name))
        'rulesCol.rules.Add(New Rule("Input 1 pressed Module 2", "\xF2\x11\xF3*CHA*M2*P01:1*", macro3.name))

        ' LANBRIDGE RULES

        Dim macro1 = New RuleMacro("CF Mini relay 1 pulse then relay 2 pulse")
        macro1.actions.Add(New RuleMacroAction(0, "\xF2\x20\xF3TRLYSET\xF4P01:P:00010\xF5\xF5"))
        macro1.actions.Add(New RuleMacroAction(2000, "\xF2\x20\xF3TRLYSET\xF4P02:P:00010\xF5\xF5"))

        Dim macro2 = New RuleMacro("CF Mini relay 3 toggle twice")
        macro2.actions.Add(New RuleMacroAction(0, "\xF2\x20\xF3TRLYSET\xF4P03:T\xF5\xF5"))
        macro2.actions.Add(New RuleMacroAction(2000, "\xF2\x20\xF3TRLYSET\xF4P03:T\xF5\xF5"))

        Dim macro3 = New RuleMacro("Send To TCP Server")
        macro3.actions.Add(New RuleMacroAction(500, "\xF2\x02\xF3TLANSND\xF412:Starting macro...\x0D\xF5\xF5"))
        macro3.actions.Add(New RuleMacroAction(3000, "\xF2\x02\xF3TLANSND\xF412:Data 3 seconds later\x0D\xF5\xF5"))
        macro3.actions.Add(New RuleMacroAction(500, "\xF2\x02\xF3TLANSND\xF412:Data 0.5 seconds later\x0D\xF5\xF5"))
        macro3.actions.Add(New RuleMacroAction(500, "\xF2\x02\xF3TLANSND\xF412:Data 0.5 seconds later\x0D\xF5\xF5"))
        macro3.actions.Add(New RuleMacroAction(500, "\xF2\x02\xF3TLANSND\xF412:Data 0.5 seconds later\x0D\xF5\xF5"))
        macro3.actions.Add(New RuleMacroAction(500, "\xF2\x02\xF3TLANSND\xF412:Data 0.5 seconds later\x0D\xF5\xF5"))
        macro3.actions.Add(New RuleMacroAction(500, "\xF2\x02\xF3TLANSND\xF412:Data 0.5 seconds later\x0D\xF5\xF5"))
        macro3.actions.Add(New RuleMacroAction(500, "\xF2\x02\xF3TLANSND\xF412:Finished macro\x0D\xF5\xF5"))

        rulesCol.macros.Add(macro1)
        rulesCol.macros.Add(macro2)
        rulesCol.macros.Add(macro3)

        rulesCol.rules.Add(New Rule("Data received over CFLink", "\xF2\x22\xF3*CHA*P01:1*", macro1.name))
        rulesCol.rules.Add(New Rule("Data received via UDP Broadcast", "*UDP TESTING*", macro2.name))
        rulesCol.rules.Add(New Rule("Data received via TCP Client", "*Test.2\x0D*", macro3.name))

        ' Now get a byte array from the RulesCollection
        ' bytes array now is split up in 264 byte blocks, ready to send to the hardware where rules will be stored.
        Dim bytes As Array = rulesProcess.getBytes(rulesCol)

        ' So that concludes converting TO bytes...

        If bytes Is Nothing Then
            MsgBox("Error during conversion TO bytes")
            Exit Sub
        End If

        ' Now lets reverse the process and create an Object FROM bytes returned from the hardware

        ' Concatenate the array into a single string, then convert it back to a RulesCollection object
        Dim reverseTest As String = ""
        For Each aString As String In bytes
            reverseTest &= aString
        Next

        If dlgSaveFile.ShowDialog = Windows.Forms.DialogResult.OK Then
            Try
                Using sw As New IO.StreamWriter(dlgSaveFile.FileName, False, System.Text.Encoding.GetEncoding(1252))
                    sw.Write(reverseTest)
                    sw.Close()
                End Using

            Catch ex As Exception
                MsgBox("Could not save to file. Ensure file is not in use.")
            End Try
        End If

        MsgBox("RulesCollection object converted into " & (bytes.Length * 264) & " bytes." & Environment.NewLine & _
                "Bytes copied to clipboard.")

        My.Computer.Clipboard.SetText(reverseTest)

        ' newRulesCol is a RulesCollection that will be filled with all the rule information taken from the byte stream
        Dim newRulesCol As RulesCollection = rulesProcess.fromBytes(reverseTest)

        If newRulesCol IsNot Nothing Then
            MsgBox("Bytes converted back to RuleCollection:" & Environment.NewLine & _
               "Macros: " & newRulesCol.macros.Count & Environment.NewLine & _
               "Rules: " & newRulesCol.rules.Count)
        Else
            MsgBox("Error during conversion FROM bytes")
            Exit Sub
        End If

    End Sub
End Class
