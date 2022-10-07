Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Friend Class VisualBasicLineDirectiveMap
		Inherits LineDirectiveMap(Of DirectiveTriviaSyntax)
		Public Sub New(ByVal tree As SyntaxTree)
			MyBase.New(tree)
		End Sub

		Protected Overrides Function GetEntry(ByVal directive As DirectiveTriviaSyntax, ByVal sourceText As Microsoft.CodeAnalysis.Text.SourceText, ByVal previous As LineDirectiveMap(Of DirectiveTriviaSyntax).LineMappingEntry) As LineDirectiveMap(Of DirectiveTriviaSyntax).LineMappingEntry
			Dim positionState As LineDirectiveMap(Of DirectiveTriviaSyntax).PositionState = 0
			Dim num As Integer = sourceText.Lines.IndexOf(directive.SpanStart) + 1
			Dim num1 As Integer = num
			Dim mappedLine As Integer = previous.MappedLine + num - previous.UnmappedLine
			Dim mappedPathOpt As String = previous.MappedPathOpt
			If (directive.Kind() = SyntaxKind.ExternalSourceDirectiveTrivia) Then
				Dim externalSourceDirectiveTriviaSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalSourceDirectiveTriviaSyntax = DirectCast(directive, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExternalSourceDirectiveTriviaSyntax)
				If (Not externalSourceDirectiveTriviaSyntax.LineStart.IsMissing AndAlso Not externalSourceDirectiveTriviaSyntax.ExternalSource.IsMissing) Then
					Dim lineStart As SyntaxToken = externalSourceDirectiveTriviaSyntax.LineStart
					mappedLine = CInt((Math.Min(Microsoft.VisualBasic.CompilerServices.Conversions.ToLong(lineStart.Value), CLng(2147483647)) - CLng(1)))
					lineStart = externalSourceDirectiveTriviaSyntax.ExternalSource
					mappedPathOpt = Microsoft.VisualBasic.CompilerServices.Conversions.ToString(lineStart.Value)
				End If
				positionState = If(previous.State <> LineDirectiveMap(Of TDirective).PositionState.Unknown, LineDirectiveMap(Of TDirective).PositionState.RemappedAfterHidden, LineDirectiveMap(Of TDirective).PositionState.RemappedAfterUnknown)
			ElseIf (directive.Kind() = SyntaxKind.EndExternalSourceDirectiveTrivia) Then
				mappedLine = num1
				mappedPathOpt = Nothing
				If (num1 <= previous.UnmappedLine + 1 OrElse previous.State <> LineDirectiveMap(Of TDirective).PositionState.RemappedAfterHidden AndAlso previous.State <> LineDirectiveMap(Of TDirective).PositionState.RemappedAfterUnknown) Then
					positionState = If(previous.State <> LineDirectiveMap(Of TDirective).PositionState.RemappedAfterHidden, LineDirectiveMap(Of TDirective).PositionState.Unknown, LineDirectiveMap(Of TDirective).PositionState.Hidden)
				Else
					positionState = LineDirectiveMap(Of TDirective).PositionState.Hidden
				End If
			End If
			Return New LineDirectiveMap(Of DirectiveTriviaSyntax).LineMappingEntry(num1, mappedLine, mappedPathOpt, DirectCast(positionState, LineDirectiveMap(Of !0).PositionState))
		End Function

		Public Overrides Function GetLineVisibility(ByVal sourceText As Microsoft.CodeAnalysis.Text.SourceText, ByVal position As Integer) As LineVisibility
			Dim linePosition As Microsoft.CodeAnalysis.Text.LinePosition = sourceText.Lines.GetLinePosition(position)
			Return Me.GetLineVisibility(MyBase.FindEntryIndex(linePosition.Line))
		End Function

		Private Function GetLineVisibility(ByVal index As Integer) As Microsoft.CodeAnalysis.LineVisibility
			Dim lineVisibility As Microsoft.CodeAnalysis.LineVisibility
			Dim entries As LineDirectiveMap(Of DirectiveTriviaSyntax).LineMappingEntry = Me.Entries(index)
			If (entries.State <> LineDirectiveMap(Of TDirective).PositionState.Unknown) Then
				lineVisibility = If(entries.State = LineDirectiveMap(Of TDirective).PositionState.Hidden, Microsoft.CodeAnalysis.LineVisibility.Hidden, Microsoft.CodeAnalysis.LineVisibility.Visible)
			ElseIf (CInt(Me.Entries.Length) < index + 3) Then
				lineVisibility = Microsoft.CodeAnalysis.LineVisibility.Visible
			ElseIf (Me.Entries(index + 1).State <> LineDirectiveMap(Of TDirective).PositionState.Unknown) Then
				Dim state As LineDirectiveMap(Of DirectiveTriviaSyntax).PositionState = Me.Entries(index + 2).State
				If (state <> LineDirectiveMap(Of TDirective).PositionState.Unknown) Then
					lineVisibility = If(state = LineDirectiveMap(Of TDirective).PositionState.Hidden, Microsoft.CodeAnalysis.LineVisibility.Hidden, Microsoft.CodeAnalysis.LineVisibility.Visible)
				Else
					lineVisibility = Me.GetLineVisibility(index + 2)
				End If
			Else
				lineVisibility = Me.GetLineVisibility(index + 1)
			End If
			Return lineVisibility
		End Function

		Protected Overrides Function InitializeFirstEntry() As LineDirectiveMap(Of DirectiveTriviaSyntax).LineMappingEntry
			Return New LineDirectiveMap(Of DirectiveTriviaSyntax).LineMappingEntry(0, 0, Nothing, LineDirectiveMap(Of TDirective).PositionState.Unknown)
		End Function

		Protected Overrides Function ShouldAddDirective(ByVal directive As DirectiveTriviaSyntax) As Boolean
			If (directive.Kind() = SyntaxKind.ExternalSourceDirectiveTrivia) Then
				Return True
			End If
			Return directive.Kind() = SyntaxKind.EndExternalSourceDirectiveTrivia
		End Function

		Friend Overrides Function TranslateSpanAndVisibility(ByVal sourceText As Microsoft.CodeAnalysis.Text.SourceText, ByVal treeFilePath As String, ByVal span As TextSpan, ByRef isHiddenPosition As Boolean) As FileLinePositionSpan
			Dim linePosition As Microsoft.CodeAnalysis.Text.LinePosition = sourceText.Lines.GetLinePosition(span.Start)
			Dim linePosition1 As Microsoft.CodeAnalysis.Text.LinePosition = sourceText.Lines.GetLinePosition(span.[End])
			Dim num As Integer = MyBase.FindEntryIndex(linePosition.Line)
			isHiddenPosition = Me.GetLineVisibility(num) = LineVisibility.Hidden
			Dim entries As LineDirectiveMap(Of DirectiveTriviaSyntax).LineMappingEntry = Me.Entries(num)
			Return MyBase.TranslateSpan(entries, treeFilePath, linePosition, linePosition1)
		End Function
	End Class
End Namespace