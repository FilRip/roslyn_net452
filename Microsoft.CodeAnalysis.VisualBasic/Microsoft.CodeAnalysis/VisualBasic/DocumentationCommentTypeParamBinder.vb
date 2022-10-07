Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class DocumentationCommentTypeParamBinder
		Inherits DocumentationCommentBinder
		Protected ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Dim empty As ImmutableArray(Of TypeParameterSymbol)
				If (Me.CommentedSymbol IsNot Nothing) Then
					Dim kind As SymbolKind = Me.CommentedSymbol.Kind
					If (kind = SymbolKind.Method) Then
						empty = DirectCast(Me.CommentedSymbol, MethodSymbol).TypeParameters
						Return empty
					Else
						If (kind <> SymbolKind.NamedType) Then
							empty = ImmutableArray(Of TypeParameterSymbol).Empty
							Return empty
						End If
						empty = DirectCast(Me.CommentedSymbol, NamedTypeSymbol).TypeParameters
						Return empty
					End If
				End If
				empty = ImmutableArray(Of TypeParameterSymbol).Empty
				Return empty
			End Get
		End Property

		Public Sub New(ByVal containingBinder As Binder, ByVal commentedSymbol As Symbol)
			MyBase.New(containingBinder, commentedSymbol)
		End Sub

		Friend Overrides Sub AddLookupSymbolsInfoInSingleBinder(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.DocumentationCommentTypeParamBinder::AddLookupSymbolsInfoInSingleBinder(Microsoft.CodeAnalysis.VisualBasic.LookupSymbolsInfo,Microsoft.CodeAnalysis.VisualBasic.LookupOptions,Microsoft.CodeAnalysis.VisualBasic.Binder)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void AddLookupSymbolsInfoInSingleBinder(Microsoft.CodeAnalysis.VisualBasic.LookupSymbolsInfo,Microsoft.CodeAnalysis.VisualBasic.LookupOptions,Microsoft.CodeAnalysis.VisualBasic.Binder)
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

		End Sub

		Friend Overrides Function BindXmlNameAttributeValue(ByVal identifier As IdentifierNameSyntax, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ImmutableArray(Of Symbol)
			Dim empty As ImmutableArray(Of Symbol)
			If (Me.CommentedSymbol IsNot Nothing) Then
				Dim valueText As String = identifier.Identifier.ValueText
				If (Not [String].IsNullOrEmpty(valueText)) Then
					empty = DocumentationCommentBinder.FindSymbolInSymbolArray(Of TypeParameterSymbol)(valueText, Me.TypeParameters)
				Else
					empty = ImmutableArray(Of Symbol).Empty
				End If
			Else
				empty = ImmutableArray(Of Symbol).Empty
			End If
			Return empty
		End Function

		Friend Overrides Sub LookupInSingleBinder(ByVal lookupResult As Microsoft.CodeAnalysis.VisualBasic.LookupResult, ByVal name As String, ByVal arity As Integer, ByVal options As LookupOptions, ByVal originalBinder As Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.DocumentationCommentTypeParamBinder::LookupInSingleBinder(Microsoft.CodeAnalysis.VisualBasic.LookupResult,System.String,System.Int32,Microsoft.CodeAnalysis.VisualBasic.LookupOptions,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.CompoundUseSiteInfo`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void LookupInSingleBinder(Microsoft.CodeAnalysis.VisualBasic.LookupResult,System.String,System.Int32,Microsoft.CodeAnalysis.VisualBasic.LookupOptions,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.CompoundUseSiteInfo<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
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

		End Sub
	End Class
End Namespace