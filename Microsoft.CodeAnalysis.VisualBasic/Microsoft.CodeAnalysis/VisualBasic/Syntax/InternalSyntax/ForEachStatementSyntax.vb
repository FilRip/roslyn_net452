Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class ForEachStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForOrForEachStatementSyntax
		Friend ReadOnly _eachKeyword As KeywordSyntax

		Friend ReadOnly _inKeyword As KeywordSyntax

		Friend ReadOnly _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property EachKeyword As KeywordSyntax
			Get
				Return Me._eachKeyword
			End Get
		End Property

		Friend ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Get
				Return Me._expression
			End Get
		End Property

		Friend ReadOnly Property InKeyword As KeywordSyntax
			Get
				Return Me._inKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal forKeyword As KeywordSyntax, ByVal eachKeyword As KeywordSyntax, ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByVal inKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			MyBase.New(kind, forKeyword, controlVariable)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(eachKeyword)
			Me._eachKeyword = eachKeyword
			MyBase.AdjustFlagsAndWidth(inKeyword)
			Me._inKeyword = inKeyword
			MyBase.AdjustFlagsAndWidth(expression)
			Me._expression = expression
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal forKeyword As KeywordSyntax, ByVal eachKeyword As KeywordSyntax, ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByVal inKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, forKeyword, controlVariable)
			MyBase._slotCount = 5
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(eachKeyword)
			Me._eachKeyword = eachKeyword
			MyBase.AdjustFlagsAndWidth(inKeyword)
			Me._inKeyword = inKeyword
			MyBase.AdjustFlagsAndWidth(expression)
			Me._expression = expression
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal forKeyword As KeywordSyntax, ByVal eachKeyword As KeywordSyntax, ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode, ByVal inKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			MyBase.New(kind, errors, annotations, forKeyword, controlVariable)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(eachKeyword)
			Me._eachKeyword = eachKeyword
			MyBase.AdjustFlagsAndWidth(inKeyword)
			Me._inKeyword = inKeyword
			MyBase.AdjustFlagsAndWidth(expression)
			Me._expression = expression
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 5
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._eachKeyword = keywordSyntax
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax1)
				Me._inKeyword = keywordSyntax1
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (expressionSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expressionSyntax)
				Me._expression = expressionSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitForEachStatement(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachStatementSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._forKeyword
					Exit Select
				Case 1
					greenNode = Me._eachKeyword
					Exit Select
				Case 2
					greenNode = Me._controlVariable
					Exit Select
				Case 3
					greenNode = Me._inKeyword
					Exit Select
				Case 4
					greenNode = Me._expression
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._forKeyword, Me._eachKeyword, Me._controlVariable, Me._inKeyword, Me._expression)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._forKeyword, Me._eachKeyword, Me._controlVariable, Me._inKeyword, Me._expression)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._eachKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._inKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._expression, IObjectWritable))
		End Sub
	End Class
End Namespace