Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Module SourceSymbolHelpers
		Public Function GetAsClauseLocation(ByVal identifier As SyntaxToken, ByVal asClauseOpt As AsClauseSyntax) As Microsoft.CodeAnalysis.SyntaxNodeOrToken
			Dim syntaxNodeOrToken As Microsoft.CodeAnalysis.SyntaxNodeOrToken
			syntaxNodeOrToken = If(asClauseOpt Is Nothing OrElse asClauseOpt.Kind() = SyntaxKind.AsNewClause AndAlso DirectCast(asClauseOpt, AsNewClauseSyntax).NewExpression.Kind() = SyntaxKind.AnonymousObjectCreationExpression, identifier, asClauseOpt.Type())
			Return syntaxNodeOrToken
		End Function
	End Module
End Namespace