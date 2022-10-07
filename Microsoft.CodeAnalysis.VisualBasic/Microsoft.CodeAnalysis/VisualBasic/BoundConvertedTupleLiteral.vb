Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundConvertedTupleLiteral
		Inherits BoundTupleExpression
		Private ReadOnly _NaturalTypeOpt As TypeSymbol

		Public ReadOnly Property NaturalTypeOpt As TypeSymbol
			Get
				Return Me._NaturalTypeOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal naturalTypeOpt As TypeSymbol, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ConvertedTupleLiteral, syntax, arguments, type, If(hasErrors, True, arguments.NonNullAndHasErrors()))
			Me._NaturalTypeOpt = naturalTypeOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitConvertedTupleLiteral(Me)
		End Function

		Public Function Update(ByVal naturalTypeOpt As TypeSymbol, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundConvertedTupleLiteral
			Dim boundConvertedTupleLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundConvertedTupleLiteral
			If (CObj(naturalTypeOpt) <> CObj(Me.NaturalTypeOpt) OrElse arguments <> MyBase.Arguments OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundConvertedTupleLiteral1 As Microsoft.CodeAnalysis.VisualBasic.BoundConvertedTupleLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundConvertedTupleLiteral(MyBase.Syntax, naturalTypeOpt, arguments, type, MyBase.HasErrors)
				boundConvertedTupleLiteral1.CopyAttributes(Me)
				boundConvertedTupleLiteral = boundConvertedTupleLiteral1
			Else
				boundConvertedTupleLiteral = Me
			End If
			Return boundConvertedTupleLiteral
		End Function
	End Class
End Namespace