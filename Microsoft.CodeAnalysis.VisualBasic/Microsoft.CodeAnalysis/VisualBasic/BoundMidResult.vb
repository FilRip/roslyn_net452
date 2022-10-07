Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundMidResult
		Inherits BoundExpression
		Private ReadOnly _Original As BoundExpression

		Private ReadOnly _Start As BoundExpression

		Private ReadOnly _LengthOpt As BoundExpression

		Private ReadOnly _Source As BoundExpression

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.Original, Me.Start, Me.LengthOpt, Me.Source)
			End Get
		End Property

		Public ReadOnly Property LengthOpt As BoundExpression
			Get
				Return Me._LengthOpt
			End Get
		End Property

		Public ReadOnly Property Original As BoundExpression
			Get
				Return Me._Original
			End Get
		End Property

		Public ReadOnly Property Source As BoundExpression
			Get
				Return Me._Source
			End Get
		End Property

		Public ReadOnly Property Start As BoundExpression
			Get
				Return Me._Start
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal original As BoundExpression, ByVal start As BoundExpression, ByVal lengthOpt As BoundExpression, ByVal source As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.MidResult, syntax, type, If(hasErrors OrElse original.NonNullAndHasErrors() OrElse start.NonNullAndHasErrors() OrElse lengthOpt.NonNullAndHasErrors(), True, source.NonNullAndHasErrors()))
			Me._Original = original
			Me._Start = start
			Me._LengthOpt = lengthOpt
			Me._Source = source
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitMidResult(Me)
		End Function

		Public Function Update(ByVal original As BoundExpression, ByVal start As BoundExpression, ByVal lengthOpt As BoundExpression, ByVal source As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundMidResult
			Dim boundMidResult As Microsoft.CodeAnalysis.VisualBasic.BoundMidResult
			If (original <> Me.Original OrElse start <> Me.Start OrElse lengthOpt <> Me.LengthOpt OrElse source <> Me.Source OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundMidResult1 As Microsoft.CodeAnalysis.VisualBasic.BoundMidResult = New Microsoft.CodeAnalysis.VisualBasic.BoundMidResult(MyBase.Syntax, original, start, lengthOpt, source, type, MyBase.HasErrors)
				boundMidResult1.CopyAttributes(Me)
				boundMidResult = boundMidResult1
			Else
				boundMidResult = Me
			End If
			Return boundMidResult
		End Function
	End Class
End Namespace