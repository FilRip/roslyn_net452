Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class MemberAccessExpressionSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
		Friend ReadOnly _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax

		Friend ReadOnly _operatorToken As PunctuationSyntax

		Friend ReadOnly _name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Get
				Return Me._expression
			End Get
		End Property

		Friend ReadOnly Property Name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax
			Get
				Return Me._name
			End Get
		End Property

		Friend ReadOnly Property OperatorToken As PunctuationSyntax
			Get
				Return Me._operatorToken
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 3
			If (expression IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expression)
				Me._expression = expression
			End If
			MyBase.AdjustFlagsAndWidth(operatorToken)
			Me._operatorToken = operatorToken
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			If (expression IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expression)
				Me._expression = expression
			End If
			MyBase.AdjustFlagsAndWidth(operatorToken)
			Me._operatorToken = operatorToken
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal operatorToken As PunctuationSyntax, ByVal name As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			If (expression IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expression)
				Me._expression = expression
			End If
			MyBase.AdjustFlagsAndWidth(operatorToken)
			Me._operatorToken = operatorToken
			MyBase.AdjustFlagsAndWidth(name)
			Me._name = name
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (expressionSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expressionSyntax)
				Me._expression = expressionSyntax
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._operatorToken = punctuationSyntax
			End If
			Dim simpleNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax)
			If (simpleNameSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(simpleNameSyntax)
				Me._name = simpleNameSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitMemberAccessExpression(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._expression
					Exit Select
				Case 1
					greenNode = Me._operatorToken
					Exit Select
				Case 2
					greenNode = Me._name
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._expression, Me._operatorToken, Me._name)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._expression, Me._operatorToken, Me._name)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._expression, IObjectWritable))
			writer.WriteValue(DirectCast(Me._operatorToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._name, IObjectWritable))
		End Sub
	End Class
End Namespace