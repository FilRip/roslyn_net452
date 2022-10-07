Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class UsingInfo
		Public ReadOnly PlaceholderInfo As Dictionary(Of TypeSymbol, ValueTuple(Of BoundRValuePlaceholder, BoundExpression, BoundExpression))

		Public ReadOnly UsingStatementSyntax As UsingBlockSyntax

		Public Sub New(ByVal usingStatementSyntax As UsingBlockSyntax, ByVal placeholderInfo As Dictionary(Of TypeSymbol, ValueTuple(Of BoundRValuePlaceholder, BoundExpression, BoundExpression)))
			MyBase.New()
			Me.PlaceholderInfo = placeholderInfo
			Me.UsingStatementSyntax = usingStatementSyntax
		End Sub
	End Class
End Namespace