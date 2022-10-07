Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundXmlProcessingInstruction
		Inherits BoundExpression
		Private ReadOnly _Target As BoundExpression

		Private ReadOnly _Data As BoundExpression

		Private ReadOnly _ObjectCreation As BoundExpression

		Public ReadOnly Property Data As BoundExpression
			Get
				Return Me._Data
			End Get
		End Property

		Public ReadOnly Property ObjectCreation As BoundExpression
			Get
				Return Me._ObjectCreation
			End Get
		End Property

		Public ReadOnly Property Target As BoundExpression
			Get
				Return Me._Target
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal target As BoundExpression, ByVal data As BoundExpression, ByVal objectCreation As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.XmlProcessingInstruction, syntax, type, If(hasErrors OrElse target.NonNullAndHasErrors() OrElse data.NonNullAndHasErrors(), True, objectCreation.NonNullAndHasErrors()))
			Me._Target = target
			Me._Data = data
			Me._ObjectCreation = objectCreation
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitXmlProcessingInstruction(Me)
		End Function

		Public Function Update(ByVal target As BoundExpression, ByVal data As BoundExpression, ByVal objectCreation As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundXmlProcessingInstruction
			Dim boundXmlProcessingInstruction As Microsoft.CodeAnalysis.VisualBasic.BoundXmlProcessingInstruction
			If (target <> Me.Target OrElse data <> Me.Data OrElse objectCreation <> Me.ObjectCreation OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundXmlProcessingInstruction1 As Microsoft.CodeAnalysis.VisualBasic.BoundXmlProcessingInstruction = New Microsoft.CodeAnalysis.VisualBasic.BoundXmlProcessingInstruction(MyBase.Syntax, target, data, objectCreation, type, MyBase.HasErrors)
				boundXmlProcessingInstruction1.CopyAttributes(Me)
				boundXmlProcessingInstruction = boundXmlProcessingInstruction1
			Else
				boundXmlProcessingInstruction = Me
			End If
			Return boundXmlProcessingInstruction
		End Function
	End Class
End Namespace