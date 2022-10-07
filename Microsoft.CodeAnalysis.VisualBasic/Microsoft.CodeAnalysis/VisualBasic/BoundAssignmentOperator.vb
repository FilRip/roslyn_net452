Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundAssignmentOperator
		Inherits BoundExpression
		Private ReadOnly _Left As BoundExpression

		Private ReadOnly _LeftOnTheRightOpt As BoundCompoundAssignmentTargetPlaceholder

		Private ReadOnly _Right As BoundExpression

		Private ReadOnly _SuppressObjectClone As Boolean

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.Left, Me.Right)
			End Get
		End Property

		Public ReadOnly Property Left As BoundExpression
			Get
				Return Me._Left
			End Get
		End Property

		Public ReadOnly Property LeftOnTheRightOpt As BoundCompoundAssignmentTargetPlaceholder
			Get
				Return Me._LeftOnTheRightOpt
			End Get
		End Property

		Public ReadOnly Property Right As BoundExpression
			Get
				Return Me._Right
			End Get
		End Property

		Public ReadOnly Property SuppressObjectClone As Boolean
			Get
				Return Me._SuppressObjectClone
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal left As BoundExpression, ByVal right As BoundExpression, ByVal suppressObjectClone As Boolean, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, left, Nothing, right, suppressObjectClone, type, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal left As BoundExpression, ByVal right As BoundExpression, ByVal suppressObjectClone As Boolean, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, left, Nothing, right, suppressObjectClone, hasErrors)
		End Sub

		Public Sub New(ByVal syntax As Microsoft.CodeAnalysis.SyntaxNode, ByVal left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal leftOnTheRightOpt As Microsoft.CodeAnalysis.VisualBasic.BoundCompoundAssignmentTargetPlaceholder, ByVal right As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal suppressObjectClone As Boolean, Optional ByVal hasErrors As Boolean = False)
			MyClass.New(syntax, left, leftOnTheRightOpt, right, suppressObjectClone, If(left.IsPropertyOrXmlPropertyAccess(), left.GetPropertyOrXmlProperty().ContainingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Void), If(left.IsLateBound(), left.Type.ContainingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Void), left.Type)), hasErrors)
		End Sub

		Public Sub New(ByVal syntax As SyntaxNode, ByVal left As BoundExpression, ByVal leftOnTheRightOpt As BoundCompoundAssignmentTargetPlaceholder, ByVal right As BoundExpression, ByVal suppressObjectClone As Boolean, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.AssignmentOperator, syntax, type, If(hasErrors OrElse left.NonNullAndHasErrors() OrElse leftOnTheRightOpt.NonNullAndHasErrors(), True, right.NonNullAndHasErrors()))
			Me._Left = left
			Me._LeftOnTheRightOpt = leftOnTheRightOpt
			Me._Right = right
			Me._SuppressObjectClone = suppressObjectClone
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitAssignmentOperator(Me)
		End Function

		Public Function Update(ByVal left As BoundExpression, ByVal leftOnTheRightOpt As BoundCompoundAssignmentTargetPlaceholder, ByVal right As BoundExpression, ByVal suppressObjectClone As Boolean, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator
			Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator
			If (left <> Me.Left OrElse leftOnTheRightOpt <> Me.LeftOnTheRightOpt OrElse right <> Me.Right OrElse suppressObjectClone <> Me.SuppressObjectClone OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundAssignmentOperator1 As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(MyBase.Syntax, left, leftOnTheRightOpt, right, suppressObjectClone, type, MyBase.HasErrors)
				boundAssignmentOperator1.CopyAttributes(Me)
				boundAssignmentOperator = boundAssignmentOperator1
			Else
				boundAssignmentOperator = Me
			End If
			Return boundAssignmentOperator
		End Function
	End Class
End Namespace