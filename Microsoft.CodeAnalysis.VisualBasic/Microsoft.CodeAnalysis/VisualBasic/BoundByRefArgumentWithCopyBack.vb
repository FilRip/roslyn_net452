Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundByRefArgumentWithCopyBack
		Inherits BoundExpression
		Private ReadOnly _OriginalArgument As BoundExpression

		Private ReadOnly _InConversion As BoundExpression

		Private ReadOnly _InPlaceholder As BoundByRefArgumentPlaceholder

		Private ReadOnly _OutConversion As BoundExpression

		Private ReadOnly _OutPlaceholder As BoundRValuePlaceholder

		Public ReadOnly Property InConversion As BoundExpression
			Get
				Return Me._InConversion
			End Get
		End Property

		Public ReadOnly Property InPlaceholder As BoundByRefArgumentPlaceholder
			Get
				Return Me._InPlaceholder
			End Get
		End Property

		Public ReadOnly Property OriginalArgument As BoundExpression
			Get
				Return Me._OriginalArgument
			End Get
		End Property

		Public ReadOnly Property OutConversion As BoundExpression
			Get
				Return Me._OutConversion
			End Get
		End Property

		Public ReadOnly Property OutPlaceholder As BoundRValuePlaceholder
			Get
				Return Me._OutPlaceholder
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal originalArgument As BoundExpression, ByVal inConversion As BoundExpression, ByVal inPlaceholder As BoundByRefArgumentPlaceholder, ByVal outConversion As BoundExpression, ByVal outPlaceholder As BoundRValuePlaceholder, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ByRefArgumentWithCopyBack, syntax, type, If(hasErrors OrElse originalArgument.NonNullAndHasErrors() OrElse inConversion.NonNullAndHasErrors() OrElse inPlaceholder.NonNullAndHasErrors() OrElse outConversion.NonNullAndHasErrors(), True, outPlaceholder.NonNullAndHasErrors()))
			Me._OriginalArgument = originalArgument
			Me._InConversion = inConversion
			Me._InPlaceholder = inPlaceholder
			Me._OutConversion = outConversion
			Me._OutPlaceholder = outPlaceholder
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitByRefArgumentWithCopyBack(Me)
		End Function

		Public Function Update(ByVal originalArgument As BoundExpression, ByVal inConversion As BoundExpression, ByVal inPlaceholder As BoundByRefArgumentPlaceholder, ByVal outConversion As BoundExpression, ByVal outPlaceholder As BoundRValuePlaceholder, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundByRefArgumentWithCopyBack
			Dim boundByRefArgumentWithCopyBack As Microsoft.CodeAnalysis.VisualBasic.BoundByRefArgumentWithCopyBack
			If (originalArgument <> Me.OriginalArgument OrElse inConversion <> Me.InConversion OrElse inPlaceholder <> Me.InPlaceholder OrElse outConversion <> Me.OutConversion OrElse outPlaceholder <> Me.OutPlaceholder OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundByRefArgumentWithCopyBack1 As Microsoft.CodeAnalysis.VisualBasic.BoundByRefArgumentWithCopyBack = New Microsoft.CodeAnalysis.VisualBasic.BoundByRefArgumentWithCopyBack(MyBase.Syntax, originalArgument, inConversion, inPlaceholder, outConversion, outPlaceholder, type, MyBase.HasErrors)
				boundByRefArgumentWithCopyBack1.CopyAttributes(Me)
				boundByRefArgumentWithCopyBack = boundByRefArgumentWithCopyBack1
			Else
				boundByRefArgumentWithCopyBack = Me
			End If
			Return boundByRefArgumentWithCopyBack
		End Function
	End Class
End Namespace