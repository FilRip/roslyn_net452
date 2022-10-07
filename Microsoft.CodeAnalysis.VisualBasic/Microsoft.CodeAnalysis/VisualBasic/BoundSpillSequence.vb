Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundSpillSequence
		Inherits BoundExpression
		Private ReadOnly _Locals As ImmutableArray(Of LocalSymbol)

		Private ReadOnly _SpillFields As ImmutableArray(Of FieldSymbol)

		Private ReadOnly _Statements As ImmutableArray(Of BoundStatement)

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

		Public ReadOnly Property SpillFields As ImmutableArray(Of FieldSymbol)
			Get
				Return Me._SpillFields
			End Get
		End Property

		Public ReadOnly Property Statements As ImmutableArray(Of BoundStatement)
			Get
				Return Me._Statements
			End Get
		End Property

		Public ReadOnly Property ValueOpt As BoundExpression
			Get
				Return Me._ValueOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal locals As ImmutableArray(Of LocalSymbol), ByVal spillFields As ImmutableArray(Of FieldSymbol), ByVal statements As ImmutableArray(Of BoundStatement), ByVal valueOpt As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.SpillSequence, syntax, type, If(hasErrors OrElse statements.NonNullAndHasErrors(), True, valueOpt.NonNullAndHasErrors()))
			Me._Locals = locals
			Me._SpillFields = spillFields
			Me._Statements = statements
			Me._ValueOpt = valueOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitSpillSequence(Me)
		End Function

		Public Shadows Function MakeRValue() As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence
			Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence
			boundSpillSequence = If(Not Me.IsLValue, Me, Me.Update(Me.Locals, Me.SpillFields, Me.Statements, Me.ValueOpt.MakeRValue(), MyBase.Type))
			Return boundSpillSequence
		End Function

		Protected Overrides Function MakeRValueImpl() As BoundExpression
			Return Me.MakeRValue()
		End Function

		Public Function Update(ByVal locals As ImmutableArray(Of LocalSymbol), ByVal spillFields As ImmutableArray(Of FieldSymbol), ByVal statements As ImmutableArray(Of BoundStatement), ByVal valueOpt As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence
			Dim boundSpillSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence
			If (locals <> Me.Locals OrElse spillFields <> Me.SpillFields OrElse statements <> Me.Statements OrElse valueOpt <> Me.ValueOpt OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundSpillSequence1 As Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSpillSequence(MyBase.Syntax, locals, spillFields, statements, valueOpt, type, MyBase.HasErrors)
				boundSpillSequence1.CopyAttributes(Me)
				boundSpillSequence = boundSpillSequence1
			Else
				boundSpillSequence = Me
			End If
			Return boundSpillSequence
		End Function
	End Class
End Namespace