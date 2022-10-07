Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundByRefArgumentPlaceholder
		Inherits BoundValuePlaceholderBase
		Private ReadOnly _IsOut As Boolean

		Public ReadOnly Property IsOut As Boolean
			Get
				Return Me._IsOut
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal isOut As Boolean, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.ByRefArgumentPlaceholder, syntax, type, hasErrors)
			Me._IsOut = isOut
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal isOut As Boolean, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.ByRefArgumentPlaceholder, syntax, type)
			Me._IsOut = isOut
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitByRefArgumentPlaceholder(Me)
		End Function

		Public Function Update(ByVal isOut As Boolean, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundByRefArgumentPlaceholder
			Dim boundByRefArgumentPlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundByRefArgumentPlaceholder
			If (isOut <> Me.IsOut OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundByRefArgumentPlaceholder1 As Microsoft.CodeAnalysis.VisualBasic.BoundByRefArgumentPlaceholder = New Microsoft.CodeAnalysis.VisualBasic.BoundByRefArgumentPlaceholder(MyBase.Syntax, isOut, type, MyBase.HasErrors)
				boundByRefArgumentPlaceholder1.CopyAttributes(Me)
				boundByRefArgumentPlaceholder = boundByRefArgumentPlaceholder1
			Else
				boundByRefArgumentPlaceholder = Me
			End If
			Return boundByRefArgumentPlaceholder
		End Function
	End Class
End Namespace