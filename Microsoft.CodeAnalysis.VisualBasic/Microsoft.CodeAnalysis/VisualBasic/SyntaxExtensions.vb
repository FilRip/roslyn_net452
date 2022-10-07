Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public Module SyntaxExtensions
		<Extension>
		Public Function Attributes(ByVal asClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax) As SyntaxList(Of AttributeListSyntax)
			Dim attributeLists As SyntaxList(Of AttributeListSyntax)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = asClauseSyntax.Kind()
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleAsClause) Then
				attributeLists = DirectCast(asClauseSyntax, SimpleAsClauseSyntax).AttributeLists
			Else
				If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsNewClause) Then
					Throw ExceptionUtilities.UnexpectedValue(asClauseSyntax.Kind())
				End If
				attributeLists = DirectCast(asClauseSyntax, AsNewClauseSyntax).NewExpression.AttributeLists
			End If
			Return attributeLists
		End Function

		<Extension>
		Public Function NormalizeWhitespace(Of TNode As SyntaxNode)(ByVal node As TNode, ByVal useDefaultCasing As Boolean, ByVal indentation As String, ByVal elasticTrivia As Boolean) As TNode
			Return DirectCast(SyntaxNormalizer.Normalize(Of TNode)(node, indentation, "" & VbCrLf & "", elasticTrivia, useDefaultCasing), TNode)
		End Function

		<Extension>
		Public Function NormalizeWhitespace(Of TNode As SyntaxNode)(ByVal node As TNode, ByVal useDefaultCasing As Boolean, Optional ByVal indentation As String = "    ", Optional ByVal eol As String = "" & VbCrLf & "", Optional ByVal elasticTrivia As Boolean = False) As TNode
			Return DirectCast(SyntaxNormalizer.Normalize(Of TNode)(node, indentation, eol, elasticTrivia, useDefaultCasing), TNode)
		End Function

		<Extension>
		Public Function NormalizeWhitespace(ByVal token As SyntaxToken, ByVal indentation As String, ByVal elasticTrivia As Boolean) As SyntaxToken
			Return SyntaxNormalizer.Normalize(token, indentation, "" & VbCrLf & "", elasticTrivia, False)
		End Function

		<Extension>
		Public Function NormalizeWhitespace(ByVal token As SyntaxToken, Optional ByVal indentation As String = "    ", Optional ByVal eol As String = "" & VbCrLf & "", Optional ByVal elasticTrivia As Boolean = False, Optional ByVal useDefaultCasing As Boolean = False) As SyntaxToken
			Return SyntaxNormalizer.Normalize(token, indentation, eol, elasticTrivia, useDefaultCasing)
		End Function

		<Extension>
		Public Function NormalizeWhitespace(ByVal trivia As SyntaxTriviaList, Optional ByVal indentation As String = "    ", Optional ByVal eol As String = "" & VbCrLf & "", Optional ByVal elasticTrivia As Boolean = False, Optional ByVal useDefaultCasing As Boolean = False) As SyntaxTriviaList
			Return SyntaxNormalizer.Normalize(trivia, indentation, eol, elasticTrivia, useDefaultCasing)
		End Function

		<Extension>
		Friend Function ReportDocumentationCommentDiagnostics(ByVal tree As SyntaxTree) As Boolean
			Return tree.Options.DocumentationMode >= DocumentationMode.Diagnose
		End Function

		<Extension>
		Public Function ToSyntaxTriviaList(ByVal sequence As IEnumerable(Of SyntaxTrivia)) As SyntaxTriviaList
			Return SyntaxFactory.TriviaList(sequence)
		End Function

		<Extension>
		Public Function TryGetInferredMemberName(ByVal syntax As SyntaxNode) As String
			Dim str As String
			Dim valueText As String
			If (syntax IsNot Nothing) Then
				Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax = TryCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
				If (expressionSyntax IsNot Nothing) Then
					Dim xmlNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax = Nothing
					Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = expressionSyntax.ExtractAnonymousTypeMemberName(xmlNameSyntax)
					If (syntaxToken.Kind() = SyntaxKind.IdentifierToken) Then
						valueText = syntaxToken.ValueText
					Else
						valueText = Nothing
					End If
					str = valueText
				Else
					str = Nothing
				End If
			Else
				str = Nothing
			End If
			Return str
		End Function

		<Extension>
		Public Function Type(ByVal newExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.NewExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
			Select Case newExpressionSyntax.Kind()
				Case SyntaxKind.ObjectCreationExpression
					typeSyntax = DirectCast(newExpressionSyntax, ObjectCreationExpressionSyntax).Type
					Exit Select
				Case SyntaxKind.AnonymousObjectCreationExpression
					typeSyntax = Nothing
					Exit Select
				Case SyntaxKind.List Or SyntaxKind.EmptyStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndInterfaceStatement Or SyntaxKind.OptionStatement Or SyntaxKind.ImportsStatement Or SyntaxKind.RightShiftAssignmentStatement Or SyntaxKind.ConcatenateAssignmentStatement Or SyntaxKind.RaiseEventStatement Or SyntaxKind.WithStatement Or SyntaxKind.ReDimStatement Or SyntaxKind.ReDimPreserveStatement Or SyntaxKind.GetXmlNamespaceExpression Or SyntaxKind.SimpleMemberAccessExpression Or SyntaxKind.InvocationExpression Or SyntaxKind.ObjectCreationExpression Or SyntaxKind.AnonymousObjectCreationExpression
				Case SyntaxKind.EndSelectStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.MidExpression Or SyntaxKind.RaiseEventStatement Or SyntaxKind.DictionaryAccessExpression Or SyntaxKind.InvocationExpression
					Throw ExceptionUtilities.UnexpectedValue(newExpressionSyntax.Kind())
				Case SyntaxKind.ArrayCreationExpression
					typeSyntax = DirectCast(newExpressionSyntax, ArrayCreationExpressionSyntax).Type
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(newExpressionSyntax.Kind())
			End Select
			Return typeSyntax
		End Function

		<Extension>
		Public Function Type(ByVal asClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeSyntax
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = asClauseSyntax.Kind()
			If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleAsClause) Then
				typeSyntax = DirectCast(asClauseSyntax, SimpleAsClauseSyntax).Type
			Else
				If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsNewClause) Then
					Throw ExceptionUtilities.UnexpectedValue(asClauseSyntax.Kind())
				End If
				typeSyntax = DirectCast(asClauseSyntax, AsNewClauseSyntax).NewExpression.Type()
			End If
			Return typeSyntax
		End Function

		<Extension>
		Public Function WithIdentifier(ByVal simpleName As SimpleNameSyntax, ByVal identifier As SyntaxToken) As SimpleNameSyntax
			If (simpleName.Kind() <> SyntaxKind.IdentifierName) Then
				Return DirectCast(simpleName, GenericNameSyntax).WithIdentifier(identifier)
			End If
			Return DirectCast(simpleName, IdentifierNameSyntax).WithIdentifier(identifier)
		End Function
	End Module
End Namespace