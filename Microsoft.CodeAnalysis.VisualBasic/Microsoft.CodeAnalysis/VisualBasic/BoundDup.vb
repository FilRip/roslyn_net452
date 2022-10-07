Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundDup
		Inherits BoundExpression
		Private ReadOnly _IsReference As Boolean

		Public ReadOnly Property IsReference As Boolean
			Get
				Return Me._IsReference
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal isReference As Boolean, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.Dup, syntax, type, hasErrors)
			Me._IsReference = isReference
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal isReference As Boolean, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.Dup, syntax, type)
			Me._IsReference = isReference
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitDup(Me)
		End Function

		Public Function Update(ByVal isReference As Boolean, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundDup
			Dim boundDup As Microsoft.CodeAnalysis.VisualBasic.BoundDup
			If (isReference <> Me.IsReference OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundDup1 As Microsoft.CodeAnalysis.VisualBasic.BoundDup = New Microsoft.CodeAnalysis.VisualBasic.BoundDup(MyBase.Syntax, isReference, type, MyBase.HasErrors)
				boundDup1.CopyAttributes(Me)
				boundDup = boundDup1
			Else
				boundDup = Me
			End If
			Return boundDup
		End Function
	End Class
End Namespace