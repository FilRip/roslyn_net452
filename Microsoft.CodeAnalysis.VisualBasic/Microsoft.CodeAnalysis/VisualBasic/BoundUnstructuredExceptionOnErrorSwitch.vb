Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundUnstructuredExceptionOnErrorSwitch
		Inherits BoundStatement
		Private ReadOnly _Value As BoundExpression

		Private ReadOnly _Jumps As ImmutableArray(Of BoundGotoStatement)

		Public ReadOnly Property Jumps As ImmutableArray(Of BoundGotoStatement)
			Get
				Return Me._Jumps
			End Get
		End Property

		Public ReadOnly Property Value As BoundExpression
			Get
				Return Me._Value
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal value As BoundExpression, ByVal jumps As ImmutableArray(Of BoundGotoStatement), Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.UnstructuredExceptionOnErrorSwitch, syntax, If(hasErrors OrElse value.NonNullAndHasErrors(), True, jumps.NonNullAndHasErrors()))
			Me._Value = value
			Me._Jumps = jumps
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitUnstructuredExceptionOnErrorSwitch(Me)
		End Function

		Public Function Update(ByVal value As BoundExpression, ByVal jumps As ImmutableArray(Of BoundGotoStatement)) As Microsoft.CodeAnalysis.VisualBasic.BoundUnstructuredExceptionOnErrorSwitch
			Dim boundUnstructuredExceptionOnErrorSwitch As Microsoft.CodeAnalysis.VisualBasic.BoundUnstructuredExceptionOnErrorSwitch
			If (value <> Me.Value OrElse jumps <> Me.Jumps) Then
				Dim boundUnstructuredExceptionOnErrorSwitch1 As Microsoft.CodeAnalysis.VisualBasic.BoundUnstructuredExceptionOnErrorSwitch = New Microsoft.CodeAnalysis.VisualBasic.BoundUnstructuredExceptionOnErrorSwitch(MyBase.Syntax, value, jumps, MyBase.HasErrors)
				boundUnstructuredExceptionOnErrorSwitch1.CopyAttributes(Me)
				boundUnstructuredExceptionOnErrorSwitch = boundUnstructuredExceptionOnErrorSwitch1
			Else
				boundUnstructuredExceptionOnErrorSwitch = Me
			End If
			Return boundUnstructuredExceptionOnErrorSwitch
		End Function
	End Class
End Namespace