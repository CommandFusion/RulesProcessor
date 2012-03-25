Imports Noesis.Javascript
Imports Newtonsoft.Json

Namespace Rules

    Public Class Rule
        Public name As String
        Public searchString As String
        Public macroName As String

        Public Sub New(ByVal name As String, ByVal searchString As String, ByVal macroName As String)
            Me.name = name
            Me.searchString = searchString
            Me.macroName = macroName
        End Sub
    End Class

    Public Class RuleMacro
        Public name As String
        Public actions As New List(Of RuleMacroAction)

        Public Sub New(ByVal name As String)
            Me.name = name
        End Sub
    End Class

    Public Class RuleMacroAction
        Public delay As Integer
        Public command As String

        Public Sub New(ByVal delay As Integer, ByVal command As String)
            Me.delay = delay
            Me.command = command
        End Sub
    End Class

    Public Class RulesCollection
        Public macros As New List(Of RuleMacro)
        Public rules As New List(Of Rule)
    End Class

    Public Class RulesProcessor
        Private rulesJS As String

        Public Sub New()
            Dim asm As System.Reflection.Assembly = System.Reflection.Assembly.GetExecutingAssembly()
            ' Open file with the Using statement.
            If asm.GetManifestResourceStream("CommandFusion.rules.js") Is Nothing Then
                MsgBox("RulesProcessor JavaScript Resource could not be loaded!")
                Exit Sub
            End If

            Using sr As New IO.StreamReader(asm.GetManifestResourceStream("CommandFusion.rules.js"))
                rulesJS = sr.ReadToEnd()
            End Using

        End Sub

        Public Function getBytes(ByVal Rules As RulesCollection) As Array
            Dim bytes As String = ""

            Dim JS As New JavascriptContext
            JS.SetParameter("console", New MessageBox)
            JS.SetParameter("Rules", JsonConvert.SerializeObject(Rules))
            JS.SetParameter("bytes", bytes)

            Try
                JS.Run(rulesJS & Environment.NewLine & "bytes = RulesProcessor.getBytes(Rules);")
            Catch ex As Exception
                Debug.WriteLine(ex, ToString)
                Return Nothing
            End Try

            Return JS.GetParameter("bytes")
        End Function

        Public Function fromBytes(ByVal bytes As String) As RulesCollection

            Dim JS As New JavascriptContext

            JS.SetParameter("console", New MessageBox)
            JS.SetParameter("rulesJSON", "")
            JS.SetParameter("bytes", bytes)

            Try
                JS.Run(rulesJS & Environment.NewLine & "rulesJSON = RulesProcessor.fromBytes(bytes, true);")
            Catch ex As Exception
                Debug.WriteLine(ex, ToString)
                Return Nothing
            End Try

            My.Computer.Clipboard.SetText(JS.GetParameter("rulesJSON"))
            Dim rules As Object = JsonConvert.DeserializeObject(JS.GetParameter("rulesJSON"))

            ' Our new RulesCollection object that all the returned rules data will be stored in
            Dim newRulesCol As New RulesCollection

            ' Get the object properties, ready to recreate the RulesCollection
            For i As Integer = 0 To rules("macros").Count - 1
                Dim newMacro As New RuleMacro(rules("macros")(i)("name").Value)
                For j As Integer = 0 To rules("macros")(i)("actions").Count - 1
                    newMacro.actions.Add(New RuleMacroAction(rules("macros")(i)("actions")(j)("delay").Value, rules("macros")(i)("actions")(j)("command").Value))
                Next
                newRulesCol.macros.Add(newMacro)
            Next

            For i As Integer = 0 To rules("rules").Count - 1
                newRulesCol.rules.Add(New Rule(rules("rules")(i)("name").Value, rules("rules")(i)("searchString").Value, rules("rules")(i)("macroName").Value))
            Next

            Return newRulesCol

        End Function
    End Class

    Public Class MessageBox
        Public Sub log(ByVal msg As String)
            My.Computer.Clipboard.SetText(msg)
            MsgBox(msg)
        End Sub
    End Class

End Namespace
