Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundGetType
		Inherits BoundExpression
		Private ReadOnly _SourceType As BoundTypeExpression

		Public ReadOnly Property SourceType As BoundTypeExpression
			Get
				Return Me._SourceType
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal sourceType As BoundTypeExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.[GetType], syntax, type, If(hasErrors, True, sourceType.NonNullAndHasErrors()))
			Me._SourceType = sourceType
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitGetType(Me)
		End Function

		Public Function Update(ByVal sourceType As BoundTypeExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundGetType
			Dim boundGetType As Microsoft.CodeAnalysis.VisualBasic.BoundGetType
			If (sourceType <> Me.SourceType OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundGetType1 As Microsoft.CodeAnalysis.VisualBasic.BoundGetType = New Microsoft.CodeAnalysis.VisualBasic.BoundGetType(MyBase.Syntax, sourceType, type, MyBase.HasErrors)
				boundGetType1.CopyAttributes(Me)
				boundGetType = boundGetType1
			Else
				boundGetType = Me
			End If
			Return boundGetType
		End Function
	End Class
End Namespace