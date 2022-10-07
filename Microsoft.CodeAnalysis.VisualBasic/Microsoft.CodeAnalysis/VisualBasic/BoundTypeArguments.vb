Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundTypeArguments
		Inherits BoundExpression
		Private ReadOnly _Arguments As ImmutableArray(Of TypeSymbol)

		Public ReadOnly Property Arguments As ImmutableArray(Of TypeSymbol)
			Get
				Return Me._Arguments
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal arguments As ImmutableArray(Of TypeSymbol), ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.TypeArguments, syntax, Nothing, hasErrors)
			Me._Arguments = arguments
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal arguments As ImmutableArray(Of TypeSymbol))
			MyBase.New(BoundKind.TypeArguments, syntax, Nothing)
			Me._Arguments = arguments
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitTypeArguments(Me)
		End Function

		Public Function Update(ByVal arguments As ImmutableArray(Of TypeSymbol)) As BoundTypeArguments
			Dim boundTypeArgument As BoundTypeArguments
			If (arguments = Me.Arguments) Then
				boundTypeArgument = Me
			Else
				Dim boundTypeArgument1 As BoundTypeArguments = New BoundTypeArguments(MyBase.Syntax, arguments, MyBase.HasErrors)
				boundTypeArgument1.CopyAttributes(Me)
				boundTypeArgument = boundTypeArgument1
			End If
			Return boundTypeArgument
		End Function
	End Class
End Namespace