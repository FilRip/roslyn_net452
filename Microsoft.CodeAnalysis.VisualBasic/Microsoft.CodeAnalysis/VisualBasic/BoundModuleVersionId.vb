Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundModuleVersionId
		Inherits BoundExpression
		Private ReadOnly _IsLValue As Boolean

		Public Overrides ReadOnly Property IsLValue As Boolean
			Get
				Return Me._IsLValue
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal isLValue As Boolean, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.ModuleVersionId, syntax, type, hasErrors)
			Me._IsLValue = isLValue
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal isLValue As Boolean, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.ModuleVersionId, syntax, type)
			Me._IsLValue = isLValue
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitModuleVersionId(Me)
		End Function

		Public Function Update(ByVal isLValue As Boolean, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundModuleVersionId
			Dim boundModuleVersionId As Microsoft.CodeAnalysis.VisualBasic.BoundModuleVersionId
			If (isLValue <> Me.IsLValue OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundModuleVersionId1 As Microsoft.CodeAnalysis.VisualBasic.BoundModuleVersionId = New Microsoft.CodeAnalysis.VisualBasic.BoundModuleVersionId(MyBase.Syntax, isLValue, type, MyBase.HasErrors)
				boundModuleVersionId1.CopyAttributes(Me)
				boundModuleVersionId = boundModuleVersionId1
			Else
				boundModuleVersionId = Me
			End If
			Return boundModuleVersionId
		End Function
	End Class
End Namespace