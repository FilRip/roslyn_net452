Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax
Imports System
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class BeginOfBlockSyntaxReference
		Inherits TranslationSyntaxReference
		Public Sub New(ByVal reference As SyntaxReference)
			MyBase.New(reference)
		End Sub

		Protected Overrides Function Translate(ByVal reference As SyntaxReference, ByVal cancellationToken As System.Threading.CancellationToken) As SyntaxNode
			Return SyntaxFacts.BeginOfBlockStatementIfAny(reference.GetSyntax(cancellationToken))
		End Function
	End Class
End Namespace