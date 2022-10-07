Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class AggregationRangeVariableSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _nameEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax

		Friend _aggregation As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationSyntax

		Public ReadOnly Property Aggregation As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationSyntax)(Me._aggregation, 1)
			End Get
		End Property

		Public ReadOnly Property NameEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax)(Me._nameEquals)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal nameEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax, ByVal aggregation As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax(kind, errors, annotations, If(nameEquals IsNot Nothing, DirectCast(nameEquals.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax), Nothing), DirectCast(aggregation.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitAggregationRangeVariable(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitAggregationRangeVariable(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._nameEquals
			ElseIf (num = 1) Then
				syntaxNode = Me._aggregation
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim nameEquals As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				nameEquals = Me.NameEquals
			ElseIf (num = 1) Then
				nameEquals = Me.Aggregation
			Else
				nameEquals = Nothing
			End If
			Return nameEquals
		End Function

		Public Function Update(ByVal nameEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax, ByVal aggregation As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax
			Dim aggregationRangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax
			If (nameEquals <> Me.NameEquals OrElse aggregation <> Me.Aggregation) Then
				Dim aggregationRangeVariableSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.AggregationRangeVariable(nameEquals, aggregation)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				aggregationRangeVariableSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, aggregationRangeVariableSyntax1, aggregationRangeVariableSyntax1.WithAnnotations(annotations))
			Else
				aggregationRangeVariableSyntax = Me
			End If
			Return aggregationRangeVariableSyntax
		End Function

		Public Function WithAggregation(ByVal aggregation As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax
			Return Me.Update(Me.NameEquals, aggregation)
		End Function

		Public Function WithNameEquals(ByVal nameEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax
			Return Me.Update(nameEquals, Me.Aggregation)
		End Function
	End Class
End Namespace