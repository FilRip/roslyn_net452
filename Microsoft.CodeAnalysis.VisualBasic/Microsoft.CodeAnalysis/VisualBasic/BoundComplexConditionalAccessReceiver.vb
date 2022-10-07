Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundComplexConditionalAccessReceiver
		Inherits BoundExpression
		Private ReadOnly _ValueTypeReceiver As BoundExpression

		Private ReadOnly _ReferenceTypeReceiver As BoundExpression

		Public ReadOnly Property ReferenceTypeReceiver As BoundExpression
			Get
				Return Me._ReferenceTypeReceiver
			End Get
		End Property

		Public ReadOnly Property ValueTypeReceiver As BoundExpression
			Get
				Return Me._ValueTypeReceiver
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal valueTypeReceiver As BoundExpression, ByVal referenceTypeReceiver As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ComplexConditionalAccessReceiver, syntax, type, If(hasErrors OrElse valueTypeReceiver.NonNullAndHasErrors(), True, referenceTypeReceiver.NonNullAndHasErrors()))
			Me._ValueTypeReceiver = valueTypeReceiver
			Me._ReferenceTypeReceiver = referenceTypeReceiver
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitComplexConditionalAccessReceiver(Me)
		End Function

		Public Function Update(ByVal valueTypeReceiver As BoundExpression, ByVal referenceTypeReceiver As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundComplexConditionalAccessReceiver
			Dim boundComplexConditionalAccessReceiver As Microsoft.CodeAnalysis.VisualBasic.BoundComplexConditionalAccessReceiver
			If (valueTypeReceiver <> Me.ValueTypeReceiver OrElse referenceTypeReceiver <> Me.ReferenceTypeReceiver OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundComplexConditionalAccessReceiver1 As Microsoft.CodeAnalysis.VisualBasic.BoundComplexConditionalAccessReceiver = New Microsoft.CodeAnalysis.VisualBasic.BoundComplexConditionalAccessReceiver(MyBase.Syntax, valueTypeReceiver, referenceTypeReceiver, type, MyBase.HasErrors)
				boundComplexConditionalAccessReceiver1.CopyAttributes(Me)
				boundComplexConditionalAccessReceiver = boundComplexConditionalAccessReceiver1
			Else
				boundComplexConditionalAccessReceiver = Me
			End If
			Return boundComplexConditionalAccessReceiver
		End Function
	End Class
End Namespace