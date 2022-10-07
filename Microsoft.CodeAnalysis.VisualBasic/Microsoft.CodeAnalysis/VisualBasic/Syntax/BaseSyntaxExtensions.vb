Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Friend Module BaseSyntaxExtensions
		<Extension>
		Friend Function ToGreen(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return node
		End Function

		<Extension>
		Friend Function ToGreen(ByVal node As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			If (node Is Nothing) Then
				Return Nothing
			End If
			Return node.VbGreen
		End Function

		<Extension>
		Friend Function ToRed(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (node IsNot Nothing) Then
				syntaxNode = node.CreateRed(Nothing, 0)
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		<Extension>
		Friend Function ToRed(ByVal node As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
			Return node
		End Function
	End Module
End Namespace