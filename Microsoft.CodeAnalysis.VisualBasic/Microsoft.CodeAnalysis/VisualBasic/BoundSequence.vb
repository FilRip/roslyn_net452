Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundSequence
		Inherits BoundExpression
		Private ReadOnly _Locals As ImmutableArray(Of LocalSymbol)

		Private ReadOnly _SideEffects As ImmutableArray(Of BoundExpression)

		Private ReadOnly _ValueOpt As BoundExpression

		Public Overrides ReadOnly Property IsLValue As Boolean
			Get
				If (Me.ValueOpt Is Nothing) Then
					Return False
				End If
				Return Me.ValueOpt.IsLValue
			End Get
		End Property

		Public ReadOnly Property Locals As ImmutableArray(Of LocalSymbol)
			Get
				Return Me._Locals
			End Get
		End Property

		Public ReadOnly Property SideEffects As ImmutableArray(Of BoundExpression)
			Get
				Return Me._SideEffects
			End Get
		End Property

		Public ReadOnly Property ValueOpt As BoundExpression
			Get
				Return Me._ValueOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal locals As ImmutableArray(Of LocalSymbol), ByVal sideEffects As ImmutableArray(Of BoundExpression), ByVal valueOpt As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.Sequence, syntax, type, If(hasErrors OrElse sideEffects.NonNullAndHasErrors(), True, valueOpt.NonNullAndHasErrors()))
			Me._Locals = locals
			Me._SideEffects = sideEffects
			Me._ValueOpt = valueOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitSequence(Me)
		End Function

		Public Shadows Function MakeRValue() As Microsoft.CodeAnalysis.VisualBasic.BoundSequence
			Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSequence
			boundSequence = If(Not Me.IsLValue, Me, Me.Update(Me._Locals, Me._SideEffects, Me.ValueOpt.MakeRValue(), MyBase.Type))
			Return boundSequence
		End Function

		Protected Overrides Function MakeRValueImpl() As BoundExpression
			Return Me.MakeRValue()
		End Function

		Public Function Update(ByVal locals As ImmutableArray(Of LocalSymbol), ByVal sideEffects As ImmutableArray(Of BoundExpression), ByVal valueOpt As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundSequence
			Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSequence
			If (locals <> Me.Locals OrElse sideEffects <> Me.SideEffects OrElse valueOpt <> Me.ValueOpt OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundSequence1 As Microsoft.CodeAnalysis.VisualBasic.BoundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(MyBase.Syntax, locals, sideEffects, valueOpt, type, MyBase.HasErrors)
				boundSequence1.CopyAttributes(Me)
				boundSequence = boundSequence1
			Else
				boundSequence = Me
			End If
			Return boundSequence
		End Function
	End Class
End Namespace