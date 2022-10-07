Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundTupleLiteral
		Inherits BoundTupleExpression
		Private ReadOnly _InferredType As TupleTypeSymbol

		Private ReadOnly _ArgumentNamesOpt As ImmutableArray(Of String)

		Private ReadOnly _InferredNamesOpt As ImmutableArray(Of Boolean)

		Public ReadOnly Property ArgumentNamesOpt As ImmutableArray(Of String)
			Get
				Return Me._ArgumentNamesOpt
			End Get
		End Property

		Public ReadOnly Property InferredNamesOpt As ImmutableArray(Of Boolean)
			Get
				Return Me._InferredNamesOpt
			End Get
		End Property

		Public ReadOnly Property InferredType As TupleTypeSymbol
			Get
				Return Me._InferredType
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal inferredType As TupleTypeSymbol, ByVal argumentNamesOpt As ImmutableArray(Of String), ByVal inferredNamesOpt As ImmutableArray(Of Boolean), ByVal arguments As ImmutableArray(Of BoundExpression), ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.TupleLiteral, syntax, arguments, type, If(hasErrors, True, arguments.NonNullAndHasErrors()))
			Me._InferredType = inferredType
			Me._ArgumentNamesOpt = argumentNamesOpt
			Me._InferredNamesOpt = inferredNamesOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitTupleLiteral(Me)
		End Function

		Public Function Update(ByVal inferredType As TupleTypeSymbol, ByVal argumentNamesOpt As ImmutableArray(Of String), ByVal inferredNamesOpt As ImmutableArray(Of Boolean), ByVal arguments As ImmutableArray(Of BoundExpression), ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundTupleLiteral
			Dim boundTupleLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundTupleLiteral
			If (CObj(inferredType) <> CObj(Me.InferredType) OrElse argumentNamesOpt <> Me.ArgumentNamesOpt OrElse inferredNamesOpt <> Me.InferredNamesOpt OrElse arguments <> MyBase.Arguments OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundTupleLiteral1 As Microsoft.CodeAnalysis.VisualBasic.BoundTupleLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundTupleLiteral(MyBase.Syntax, inferredType, argumentNamesOpt, inferredNamesOpt, arguments, type, MyBase.HasErrors)
				boundTupleLiteral1.CopyAttributes(Me)
				boundTupleLiteral = boundTupleLiteral1
			Else
				boundTupleLiteral = Me
			End If
			Return boundTupleLiteral
		End Function
	End Class
End Namespace