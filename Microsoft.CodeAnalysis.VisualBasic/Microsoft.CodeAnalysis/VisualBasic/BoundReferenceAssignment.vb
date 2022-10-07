Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundReferenceAssignment
		Inherits BoundExpression
		Private ReadOnly _ByRefLocal As BoundLocal

		Private ReadOnly _LValue As BoundExpression

		Private ReadOnly _IsLValue As Boolean

		Public ReadOnly Property ByRefLocal As BoundLocal
			Get
				Return Me._ByRefLocal
			End Get
		End Property

		Public Overrides ReadOnly Property IsLValue As Boolean
			Get
				Return Me._IsLValue
			End Get
		End Property

		Public ReadOnly Property LValue As BoundExpression
			Get
				Return Me._LValue
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal byRefLocal As BoundLocal, ByVal lValue As BoundExpression, ByVal isLValue As Boolean, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ReferenceAssignment, syntax, type, If(hasErrors OrElse byRefLocal.NonNullAndHasErrors(), True, lValue.NonNullAndHasErrors()))
			Me._ByRefLocal = byRefLocal
			Me._LValue = lValue
			Me._IsLValue = isLValue
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitReferenceAssignment(Me)
		End Function

		Public Shadows Function MakeRValue() As Microsoft.CodeAnalysis.VisualBasic.BoundReferenceAssignment
			Dim boundReferenceAssignment As Microsoft.CodeAnalysis.VisualBasic.BoundReferenceAssignment
			boundReferenceAssignment = If(Not Me._IsLValue, Me, Me.Update(Me.ByRefLocal, Me.LValue, False, MyBase.Type))
			Return boundReferenceAssignment
		End Function

		Protected Overrides Function MakeRValueImpl() As BoundExpression
			Return Me.MakeRValue()
		End Function

		Public Function Update(ByVal byRefLocal As BoundLocal, ByVal lValue As BoundExpression, ByVal isLValue As Boolean, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundReferenceAssignment
			Dim boundReferenceAssignment As Microsoft.CodeAnalysis.VisualBasic.BoundReferenceAssignment
			If (byRefLocal <> Me.ByRefLocal OrElse lValue <> Me.LValue OrElse isLValue <> Me.IsLValue OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundReferenceAssignment1 As Microsoft.CodeAnalysis.VisualBasic.BoundReferenceAssignment = New Microsoft.CodeAnalysis.VisualBasic.BoundReferenceAssignment(MyBase.Syntax, byRefLocal, lValue, isLValue, type, MyBase.HasErrors)
				boundReferenceAssignment1.CopyAttributes(Me)
				boundReferenceAssignment = boundReferenceAssignment1
			Else
				boundReferenceAssignment = Me
			End If
			Return boundReferenceAssignment
		End Function
	End Class
End Namespace