Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class TypeOfExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
		Friend ReadOnly _typeOfKeyword As KeywordSyntax

		Friend ReadOnly _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax

		Friend ReadOnly _operatorToken As KeywordSyntax

		Friend ReadOnly _type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Get
				Return Me._expression
			End Get
		End Property

		Friend ReadOnly Property OperatorToken As KeywordSyntax
			Get
				Return Me._operatorToken
			End Get
		End Property

		Friend ReadOnly Property Type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax
			Get
				Return Me._type
			End Get
		End Property

		Friend ReadOnly Property TypeOfKeyword As KeywordSyntax
			Get
				Return Me._typeOfKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal typeOfKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As KeywordSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(typeOfKeyword)
			Me._typeOfKeyword = typeOfKeyword
			MyBase.AdjustFlagsAndWidth(expression)
			Me._expression = expression
			MyBase.AdjustFlagsAndWidth(operatorToken)
			Me._operatorToken = operatorToken
			MyBase.AdjustFlagsAndWidth(type)
			Me._type = type
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal typeOfKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As KeywordSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(typeOfKeyword)
			Me._typeOfKeyword = typeOfKeyword
			MyBase.AdjustFlagsAndWidth(expression)
			Me._expression = expression
			MyBase.AdjustFlagsAndWidth(operatorToken)
			Me._operatorToken = operatorToken
			MyBase.AdjustFlagsAndWidth(type)
			Me._type = type
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal typeOfKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As KeywordSyntax, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(typeOfKeyword)
			Me._typeOfKeyword = typeOfKeyword
			MyBase.AdjustFlagsAndWidth(expression)
			Me._expression = expression
			MyBase.AdjustFlagsAndWidth(operatorToken)
			Me._operatorToken = operatorToken
			MyBase.AdjustFlagsAndWidth(type)
			Me._type = type
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 4
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._typeOfKeyword = keywordSyntax
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (expressionSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expressionSyntax)
				Me._expression = expressionSyntax
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax1)
				Me._operatorToken = keywordSyntax1
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (typeSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(typeSyntax)
				Me._type = typeSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitTypeOfExpression(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeOfExpressionSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._typeOfKeyword
					Exit Select
				Case 1
					greenNode = Me._expression
					Exit Select
				Case 2
					greenNode = Me._operatorToken
					Exit Select
				Case 3
					greenNode = Me._type
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._typeOfKeyword, Me._expression, Me._operatorToken, Me._type)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._typeOfKeyword, Me._expression, Me._operatorToken, Me._type)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._typeOfKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._expression, IObjectWritable))
			writer.WriteValue(DirectCast(Me._operatorToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._type, IObjectWritable))
		End Sub
	End Class
End Namespace