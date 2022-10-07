Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class ThrowStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax
		Friend ReadOnly _throwKeyword As KeywordSyntax

		Friend ReadOnly _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Get
				Return Me._expression
			End Get
		End Property

		Friend ReadOnly Property ThrowKeyword As KeywordSyntax
			Get
				Return Me._throwKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal throwKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 2
			MyBase.AdjustFlagsAndWidth(throwKeyword)
			Me._throwKeyword = throwKeyword
			If (expression IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expression)
				Me._expression = expression
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal throwKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 2
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(throwKeyword)
			Me._throwKeyword = throwKeyword
			If (expression IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expression)
				Me._expression = expression
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal throwKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 2
			MyBase.AdjustFlagsAndWidth(throwKeyword)
			Me._throwKeyword = throwKeyword
			If (expression IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expression)
				Me._expression = expression
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 2
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._throwKeyword = keywordSyntax
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (expressionSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expressionSyntax)
				Me._expression = expressionSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitThrowStatement(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.ThrowStatementSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Dim num As Integer = i
			If (num = 0) Then
				greenNode = Me._throwKeyword
			ElseIf (num = 1) Then
				greenNode = Me._expression
			Else
				greenNode = Nothing
			End If
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._throwKeyword, Me._expression)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._throwKeyword, Me._expression)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._throwKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._expression, IObjectWritable))
		End Sub
	End Class
End Namespace