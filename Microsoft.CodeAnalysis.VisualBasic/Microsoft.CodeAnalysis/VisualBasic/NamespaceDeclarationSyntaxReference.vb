Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class NamespaceDeclarationSyntaxReference
		Inherits TranslationSyntaxReference
		Public Sub New(ByVal reference As SyntaxReference)
			MyBase.New(reference)
		End Sub

		Protected Overrides Function Translate(ByVal reference As SyntaxReference, ByVal cancellationToken As System.Threading.CancellationToken) As SyntaxNode
			Dim syntax As SyntaxNode = reference.GetSyntax(cancellationToken)
			While TypeOf syntax Is NameSyntax
				syntax = syntax.Parent
			End While
			Return syntax
		End Function
	End Class
End Namespace