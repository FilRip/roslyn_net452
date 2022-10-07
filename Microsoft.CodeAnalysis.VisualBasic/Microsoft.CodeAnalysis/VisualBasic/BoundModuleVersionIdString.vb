Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundModuleVersionIdString
		Inherits BoundExpression
		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, ByVal hasErrors As Boolean)
			MyBase.New(BoundKind.ModuleVersionIdString, syntax, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol)
			MyBase.New(BoundKind.ModuleVersionIdString, syntax, type)
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitModuleVersionIdString(Me)
		End Function

		Public Function Update(ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundModuleVersionIdString
			Dim boundModuleVersionIdString As Microsoft.CodeAnalysis.VisualBasic.BoundModuleVersionIdString
			If (CObj(type) = CObj(MyBase.Type)) Then
				boundModuleVersionIdString = Me
			Else
				Dim boundModuleVersionIdString1 As Microsoft.CodeAnalysis.VisualBasic.BoundModuleVersionIdString = New Microsoft.CodeAnalysis.VisualBasic.BoundModuleVersionIdString(MyBase.Syntax, type, MyBase.HasErrors)
				boundModuleVersionIdString1.CopyAttributes(Me)
				boundModuleVersionIdString = boundModuleVersionIdString1
			End If
			Return boundModuleVersionIdString
		End Function
	End Class
End Namespace