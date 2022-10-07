Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module OptionsValidator
		Friend Function IsValidNamespaceName(ByVal name As String) As Boolean
			Dim flag As Boolean
			Dim num As Integer = 0
			While True
				Dim num1 As Integer = name.IndexOf("."C, num)
				If (Not OptionsValidator.IsValidRootNamespaceComponent(name, num, If(num1 < 0, name.Length, num1), True)) Then
					flag = False
					Exit While
				ElseIf (num1 < 0) Then
					flag = True
					Exit While
				Else
					num = num1 + 1
				End If
			End While
			Return flag
		End Function

		Private Function IsValidRootNamespaceComponent(ByVal name As String, ByVal start As Integer, ByVal [end] As Integer, ByVal allowEscaping As Boolean) As Boolean
			Dim flag As Boolean
			If (start <> [end]) Then
				Dim num As Integer = [end] - 1
				If (allowEscaping AndAlso SyntaxFacts.ReturnFullWidthOrSelf(name(start)) = "［"C) Then
					flag = If(SyntaxFacts.ReturnFullWidthOrSelf(name(num)) = "］"C, OptionsValidator.IsValidRootNamespaceComponent(name, start + 1, num, False), False)
				ElseIf (Not SyntaxFacts.IsIdentifierStartCharacter(name(start))) Then
					flag = False
				ElseIf ([end] - start <> 1 OrElse SyntaxFacts.ReturnFullWidthOrSelf(name(start)) <> Strings.ChrW(65343)) Then
					Dim num1 As Integer = num
					Dim num2 As Integer = start + 1
					While num2 <= num1
						If (SyntaxFacts.IsIdentifierPartCharacter(name(num2))) Then
							num2 = num2 + 1
						Else
							flag = False
							Return flag
						End If
					End While
					flag = True
				Else
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Friend Function ParseImports(ByVal importsClauses As IEnumerable(Of String), ByVal diagnostics As DiagnosticBag) As GlobalImport()
			Dim array As GlobalImport()
			Dim variable As OptionsValidator._Closure$__0-0 = Nothing
			Dim func As Func(Of String, String)
			Dim func1 As Func(Of String, String, String)
			Dim severity As Func(Of Diagnostic, Boolean)
			Dim strArray As String() = importsClauses.[Select](Of String)(New Func(Of String, String)(AddressOf StringExtensions.Unquote)).ToArray()
			If (CInt(strArray.Length) <= 0) Then
				array = System.Array.Empty(Of GlobalImport)()
			Else
				Dim strArray1 As String() = strArray
				If (OptionsValidator._Closure$__.$I0-0 Is Nothing) Then
					func = Function(name As String) [String].Concat("Imports ", name, "" & VbCrLf & "" & VbCrLf & "")
					OptionsValidator._Closure$__.$I0-0 = func
				Else
					func = OptionsValidator._Closure$__.$I0-0
				End If
				Dim strs As IEnumerable(Of String) = strArray1.[Select](Of String)(func)
				If (OptionsValidator._Closure$__.$I0-1 Is Nothing) Then
					func1 = Function(a As String, b As String) [String].Concat(a, b)
					OptionsValidator._Closure$__.$I0-1 = func1
				Else
					func1 = OptionsValidator._Closure$__.$I0-1
				End If
				Dim sourceText As Microsoft.CodeAnalysis.Text.SourceText = Microsoft.CodeAnalysis.Text.SourceText.From(strs.Aggregate(func1), Nothing, SourceHashAlgorithm.Sha1)
				Dim [default] As VisualBasicParseOptions = VisualBasicParseOptions.[Default]
				Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
				Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = VisualBasicSyntaxTree.ParseText(sourceText, [default], "", DirectCast(Nothing, ImmutableDictionary(Of String, ReportDiagnostic)), cancellationToken)
				Dim globalImports As List(Of GlobalImport) = New List(Of GlobalImport)()
				Dim [imports] As SyntaxList(Of ImportsStatementSyntax) = syntaxTree.GetCompilationUnitRoot().[Imports]
				Dim count As Integer = [imports].Count - 1
				Dim num As Integer = 0
				Do
					Dim importsClauseSyntaxes As SeparatedSyntaxList(Of ImportsClauseSyntax) = [imports](num).ImportsClauses
					If (importsClauses.Count() > 0) Then
						variable = New OptionsValidator._Closure$__0-0(variable)
						Dim item As ImportsClauseSyntax = importsClauseSyntaxes(0)
						Dim syntaxErrors As IEnumerable(Of Diagnostic) = item.GetSyntaxErrors(syntaxTree)
						If (importsClauseSyntaxes.Count > 1) Then
							syntaxErrors = syntaxErrors.Concat(New VBDiagnostic(New DiagnosticInfo(MessageProvider.Instance, 30205), importsClauseSyntaxes(1).GetLocation(), False))
						End If
						variable.$VB$Local_import = New GlobalImport(item, strArray(num))
						Dim diagnostics1 As IEnumerable(Of Diagnostic) = syntaxErrors.[Select](Of Diagnostic)(Function(diag As Diagnostic) Me.$VB$Local_import.MapDiagnostic(diag))
						diagnostics.AddRange(diagnostics1)
						Dim diagnostics2 As IEnumerable(Of Diagnostic) = diagnostics1
						If (OptionsValidator._Closure$__.$I0-3 Is Nothing) Then
							severity = Function(diag As Diagnostic) diag.Severity = DiagnosticSeverity.[Error]
							OptionsValidator._Closure$__.$I0-3 = severity
						Else
							severity = OptionsValidator._Closure$__.$I0-3
						End If
						If (Not diagnostics2.Any(severity)) Then
							globalImports.Add(variable.$VB$Local_import)
						End If
					End If
					num = num + 1
				Loop While num <= count
				array = globalImports.ToArray()
			End If
			Return array
		End Function
	End Module
End Namespace