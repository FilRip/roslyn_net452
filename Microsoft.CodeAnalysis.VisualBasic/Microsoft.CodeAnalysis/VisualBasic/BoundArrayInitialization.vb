Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundArrayInitialization
		Inherits BoundExpression
		Private ReadOnly _Initializers As ImmutableArray(Of BoundExpression)

		Public ReadOnly Property Initializers As ImmutableArray(Of BoundExpression)
			Get
				Return Me._Initializers
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal initializers As ImmutableArray(Of BoundExpression), ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ArrayInitialization, syntax, type, If(hasErrors, True, initializers.NonNullAndHasErrors()))
			Me._Initializers = initializers
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitArrayInitialization(Me)
		End Function

		Public Function Update(ByVal initializers As ImmutableArray(Of BoundExpression), ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization
			Dim boundArrayInitialization As Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization
			If (initializers <> Me.Initializers OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundArrayInitialization1 As Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization(MyBase.Syntax, initializers, type, MyBase.HasErrors)
				boundArrayInitialization1.CopyAttributes(Me)
				boundArrayInitialization = boundArrayInitialization1
			Else
				boundArrayInitialization = Me
			End If
			Return boundArrayInitialization
		End Function
	End Class
End Namespace