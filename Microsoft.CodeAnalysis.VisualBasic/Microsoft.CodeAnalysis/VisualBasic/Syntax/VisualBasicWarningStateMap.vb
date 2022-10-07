Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Syntax
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Friend Class VisualBasicWarningStateMap
		Inherits AbstractWarningStateMap(Of ReportDiagnostic)
		Public Sub New(ByVal tree As SyntaxTree)
			MyBase.New(tree)
		End Sub

		Private Shared Function CreateWarningStateEntries(ByVal directiveList As ImmutableArray(Of DirectiveTriviaSyntax)) As AbstractWarningStateMap(Of Microsoft.CodeAnalysis.ReportDiagnostic).WarningStateMapEntry()
			Dim reportDiagnostic As Microsoft.CodeAnalysis.ReportDiagnostic = 0
			Dim errorCodes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax) = New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)()
			Dim warningStateMapEntry(directiveList.Length + 1 - 1) As AbstractWarningStateMap(Of Microsoft.CodeAnalysis.ReportDiagnostic).WarningStateMapEntry
			Dim num As Integer = 0
			warningStateMapEntry(num) = New AbstractWarningStateMap(Of Microsoft.CodeAnalysis.ReportDiagnostic).WarningStateMapEntry(0, Microsoft.CodeAnalysis.ReportDiagnostic.[Default], Nothing)
			Dim reportDiagnostic1 As Microsoft.CodeAnalysis.ReportDiagnostic = Microsoft.CodeAnalysis.ReportDiagnostic.[Default]
			Dim strs As ImmutableDictionary(Of String, Microsoft.CodeAnalysis.ReportDiagnostic) = ImmutableDictionary.Create(Of String, Microsoft.CodeAnalysis.ReportDiagnostic)(CaseInsensitiveComparison.Comparer)
			While num < directiveList.Length
				Dim item As DirectiveTriviaSyntax = directiveList(num)
				If (item.IsKind(SyntaxKind.EnableWarningDirectiveTrivia)) Then
					reportDiagnostic = Microsoft.CodeAnalysis.ReportDiagnostic.[Default]
					errorCodes = DirectCast(item, EnableWarningDirectiveTriviaSyntax).ErrorCodes
				ElseIf (item.IsKind(SyntaxKind.DisableWarningDirectiveTrivia)) Then
					reportDiagnostic = Microsoft.CodeAnalysis.ReportDiagnostic.Suppress
					errorCodes = DirectCast(item, DisableWarningDirectiveTriviaSyntax).ErrorCodes
				End If
				If (errorCodes.Count <> 0) Then
					Dim count As Integer = errorCodes.Count - 1
					For i As Integer = 0 To count
						Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax = errorCodes(i)
						If (Not identifierNameSyntax.IsMissing AndAlso Not identifierNameSyntax.ContainsDiagnostics) Then
							strs = strs.SetItem(identifierNameSyntax.Identifier.ValueText, reportDiagnostic)
						End If
					Next

				Else
					reportDiagnostic1 = reportDiagnostic
					strs = ImmutableDictionary.Create(Of String, Microsoft.CodeAnalysis.ReportDiagnostic)(CaseInsensitiveComparison.Comparer)
				End If
				num = num + 1
				Dim sourceSpan As TextSpan = item.GetLocation().SourceSpan
				warningStateMapEntry(num) = New AbstractWarningStateMap(Of Microsoft.CodeAnalysis.ReportDiagnostic).WarningStateMapEntry(sourceSpan.[End], reportDiagnostic1, strs)
			End While
			Return warningStateMapEntry
		End Function

		Protected Overrides Function CreateWarningStateMapEntries(ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree) As AbstractWarningStateMap(Of ReportDiagnostic).WarningStateMapEntry()
			Dim instance As ArrayBuilder(Of DirectiveTriviaSyntax) = ArrayBuilder(Of DirectiveTriviaSyntax).GetInstance()
			VisualBasicWarningStateMap.GetAllWarningDirectives(syntaxTree, instance)
			Return VisualBasicWarningStateMap.CreateWarningStateEntries(instance.ToImmutableAndFree())
		End Function

		Private Shared Sub GetAllWarningDirectives(ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree, ByVal directiveList As ArrayBuilder(Of DirectiveTriviaSyntax))
			Dim enumerator As IEnumerator(Of DirectiveTriviaSyntax) = Nothing
			Try
				Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
				enumerator = syntaxTree.GetRoot(cancellationToken).GetDirectives(Nothing).GetEnumerator()
				While enumerator.MoveNext()
					Dim current As DirectiveTriviaSyntax = enumerator.Current
					If (Not current.IsKind(SyntaxKind.EnableWarningDirectiveTrivia)) Then
						If (Not current.IsKind(SyntaxKind.DisableWarningDirectiveTrivia)) Then
							Continue While
						End If
						Dim disableWarningDirectiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.DisableWarningDirectiveTriviaSyntax = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Syntax.DisableWarningDirectiveTriviaSyntax)
						If (disableWarningDirectiveTriviaSyntax.DisableKeyword.IsMissing OrElse disableWarningDirectiveTriviaSyntax.DisableKeyword.ContainsDiagnostics OrElse disableWarningDirectiveTriviaSyntax.WarningKeyword.IsMissing OrElse disableWarningDirectiveTriviaSyntax.WarningKeyword.ContainsDiagnostics) Then
							Continue While
						End If
						directiveList.Add(disableWarningDirectiveTriviaSyntax)
					Else
						Dim enableWarningDirectiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EnableWarningDirectiveTriviaSyntax = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Syntax.EnableWarningDirectiveTriviaSyntax)
						If (enableWarningDirectiveTriviaSyntax.EnableKeyword.IsMissing OrElse enableWarningDirectiveTriviaSyntax.EnableKeyword.ContainsDiagnostics OrElse enableWarningDirectiveTriviaSyntax.WarningKeyword.IsMissing OrElse enableWarningDirectiveTriviaSyntax.WarningKeyword.ContainsDiagnostics) Then
							Continue While
						End If
						directiveList.Add(enableWarningDirectiveTriviaSyntax)
					End If
				End While
			Finally
				If (enumerator IsNot Nothing) Then
					enumerator.Dispose()
				End If
			End Try
		End Sub
	End Class
End Namespace