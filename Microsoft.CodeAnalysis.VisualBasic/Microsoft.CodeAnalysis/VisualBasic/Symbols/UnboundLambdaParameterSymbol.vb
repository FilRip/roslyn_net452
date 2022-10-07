Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class UnboundLambdaParameterSymbol
		Inherits LambdaParameterSymbol
		Private ReadOnly _identifierSyntax As ModifiedIdentifierSyntax

		Private ReadOnly _typeSyntax As SyntaxNodeOrToken

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Nothing
			End Get
		End Property

		Public ReadOnly Property IdentifierSyntax As SyntaxToken
			Get
				Return Me._identifierSyntax.Identifier
			End Get
		End Property

		Public ReadOnly Property Syntax As ModifiedIdentifierSyntax
			Get
				Return Me._identifierSyntax
			End Get
		End Property

		Public ReadOnly Property TypeSyntax As SyntaxNodeOrToken
			Get
				Return Me._typeSyntax
			End Get
		End Property

		Private Sub New(ByVal name As String, ByVal ordinal As Integer, ByVal type As TypeSymbol, ByVal location As Microsoft.CodeAnalysis.Location, ByVal flags As SourceParameterFlags, ByVal identifierSyntax As ModifiedIdentifierSyntax, ByVal typeSyntax As SyntaxNodeOrToken)
			MyBase.New(name, ordinal, type, flags And SourceParameterFlags.[ByRef] <> 0, location)
			Me._identifierSyntax = identifierSyntax
			Me._typeSyntax = typeSyntax
		End Sub

		Friend Shared Function CreateFromSyntax(ByVal syntax As ParameterSyntax, ByVal name As String, ByVal flags As SourceParameterFlags, ByVal ordinal As Integer, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ParameterSymbol
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol Microsoft.CodeAnalysis.VisualBasic.Symbols.UnboundLambdaParameterSymbol::CreateFromSyntax(Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax,System.String,Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceParameterFlags,System.Int32,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol CreateFromSyntax(Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax,System.String,Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceParameterFlags,System.Int32,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Shared Function GetModifierToken(ByVal modifiers As SyntaxTokenList, ByVal tokenKind As SyntaxKind) As SyntaxToken
			Dim enumerator As SyntaxTokenList.Enumerator = modifiers.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As SyntaxToken = enumerator.Current
				If (current.Kind() <> tokenKind) Then
					Continue While
				End If
				Return current
			End While
			Throw ExceptionUtilities.Unreachable
		End Function
	End Class
End Namespace