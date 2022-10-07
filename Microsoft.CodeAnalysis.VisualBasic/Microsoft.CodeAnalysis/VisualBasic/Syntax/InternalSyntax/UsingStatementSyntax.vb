Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class UsingStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
		Friend ReadOnly _usingKeyword As KeywordSyntax

		Friend ReadOnly _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax

		Friend ReadOnly _variables As GreenNode

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Get
				Return Me._expression
			End Get
		End Property

		Friend ReadOnly Property UsingKeyword As KeywordSyntax
			Get
				Return Me._usingKeyword
			End Get
		End Property

		Friend ReadOnly Property Variables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax)(Me._variables))
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal usingKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal variables As GreenNode)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(usingKeyword)
			Me._usingKeyword = usingKeyword
			If (expression IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expression)
				Me._expression = expression
			End If
			If (variables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(variables)
				Me._variables = variables
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal usingKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal variables As GreenNode, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(usingKeyword)
			Me._usingKeyword = usingKeyword
			If (expression IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expression)
				Me._expression = expression
			End If
			If (variables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(variables)
				Me._variables = variables
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal usingKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal variables As GreenNode)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(usingKeyword)
			Me._usingKeyword = usingKeyword
			If (expression IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expression)
				Me._expression = expression
			End If
			If (variables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(variables)
				Me._variables = variables
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._usingKeyword = keywordSyntax
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (expressionSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expressionSyntax)
				Me._expression = expressionSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._variables = greenNode
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitUsingStatement(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._usingKeyword
					Exit Select
				Case 1
					greenNode = Me._expression
					Exit Select
				Case 2
					greenNode = Me._variables
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._usingKeyword, Me._expression, Me._variables)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._usingKeyword, Me._expression, Me._variables)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._usingKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._expression, IObjectWritable))
			writer.WriteValue(DirectCast(Me._variables, IObjectWritable))
		End Sub
	End Class
End Namespace