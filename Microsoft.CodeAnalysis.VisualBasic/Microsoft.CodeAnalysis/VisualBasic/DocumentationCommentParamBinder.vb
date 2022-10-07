Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class DocumentationCommentParamBinder
		Inherits DocumentationCommentBinder
		Private Const s_invalidLookupOptions As LookupOptions = LookupOptions.NamespacesOrTypesOnly Or LookupOptions.LabelsOnly Or LookupOptions.MustBeInstance Or LookupOptions.MustNotBeInstance Or LookupOptions.AttributeTypeOnly Or LookupOptions.MustNotBeLocalOrParameter

		Private ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Dim delegateParameters As ImmutableArray(Of ParameterSymbol)
				If (Me.CommentedSymbol IsNot Nothing) Then
					Dim kind As SymbolKind = Me.CommentedSymbol.Kind
					If (kind <= SymbolKind.Method) Then
						If (kind = SymbolKind.[Event]) Then
							delegateParameters = DirectCast(Me.CommentedSymbol, EventSymbol).DelegateParameters
							Return delegateParameters
						Else
							If (kind <> SymbolKind.Method) Then
								delegateParameters = ImmutableArray(Of ParameterSymbol).Empty
								Return delegateParameters
							End If
							delegateParameters = DirectCast(Me.CommentedSymbol, MethodSymbol).Parameters
							Return delegateParameters
						End If
					ElseIf (kind = SymbolKind.NamedType) Then
						Dim commentedSymbol As NamedTypeSymbol = DirectCast(Me.CommentedSymbol, NamedTypeSymbol)
						If (commentedSymbol.TypeKind = TypeKind.[Delegate]) Then
							Dim delegateInvokeMethod As MethodSymbol = commentedSymbol.DelegateInvokeMethod
							If (delegateInvokeMethod Is Nothing) Then
								delegateParameters = ImmutableArray(Of ParameterSymbol).Empty
								Return delegateParameters
							End If
							delegateParameters = delegateInvokeMethod.Parameters
							Return delegateParameters
						End If
					Else
						If (kind <> SymbolKind.[Property]) Then
							delegateParameters = ImmutableArray(Of ParameterSymbol).Empty
							Return delegateParameters
						End If
						delegateParameters = DirectCast(Me.CommentedSymbol, PropertySymbol).Parameters
						Return delegateParameters
					End If
				End If
				delegateParameters = ImmutableArray(Of ParameterSymbol).Empty
				Return delegateParameters
			End Get
		End Property

		Public Sub New(ByVal containingBinder As Binder, ByVal commentedSymbol As Symbol)
			MyBase.New(containingBinder, commentedSymbol)
		End Sub

		Friend Overrides Sub AddLookupSymbolsInfoInSingleBinder(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.DocumentationCommentParamBinder::AddLookupSymbolsInfoInSingleBinder(Microsoft.CodeAnalysis.VisualBasic.LookupSymbolsInfo,Microsoft.CodeAnalysis.VisualBasic.LookupOptions,Microsoft.CodeAnalysis.VisualBasic.Binder)
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
					empty = DocumentationCommentBinder.FindSymbolInSymbolArray(Of ParameterSymbol)(valueText, Me.Parameters)
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
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.DocumentationCommentParamBinder::LookupInSingleBinder(Microsoft.CodeAnalysis.VisualBasic.LookupResult,System.String,System.Int32,Microsoft.CodeAnalysis.VisualBasic.LookupOptions,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.CompoundUseSiteInfo`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
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