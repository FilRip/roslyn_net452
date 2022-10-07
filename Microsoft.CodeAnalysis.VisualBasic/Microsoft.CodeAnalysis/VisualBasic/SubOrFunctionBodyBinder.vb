Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class SubOrFunctionBodyBinder
		Inherits ExecutableCodeBinder
		Private ReadOnly _methodSymbol As MethodSymbol

		Protected ReadOnly _parameterMap As Dictionary(Of String, Symbol)

		Public Overrides ReadOnly Property AdditionalContainingMembers As ImmutableArray(Of Symbol)
			Get
				Return ImmutableArray(Of Symbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingMember As Symbol
			Get
				Return Me._methodSymbol
			End Get
		End Property

		Public Sub New(ByVal methodOrLambdaSymbol As MethodSymbol, ByVal root As SyntaxNode, ByVal containingBinder As Binder)
			MyBase.New(root, containingBinder)
			Me._methodSymbol = methodOrLambdaSymbol
			Dim parameters As ImmutableArray(Of ParameterSymbol) = methodOrLambdaSymbol.Parameters
			Dim length As Integer = parameters.Length
			Dim num As Integer = length
			If (Not methodOrLambdaSymbol.IsSub) Then
				num = num + 1
			End If
			Me._parameterMap = New Dictionary(Of String, Symbol)(num, CaseInsensitiveComparison.Comparer)
			Dim num1 As Integer = length - 1
			For i As Integer = 0 To num1
				Dim item As ParameterSymbol = parameters(i)
				If (Not Me._parameterMap.ContainsKey(item.Name)) Then
					Me._parameterMap(item.Name) = item
				End If
			Next

		End Sub

		Friend Overrides Sub AddLookupSymbolsInfoInSingleBinder(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.SubOrFunctionBodyBinder::AddLookupSymbolsInfoInSingleBinder(Microsoft.CodeAnalysis.VisualBasic.LookupSymbolsInfo,Microsoft.CodeAnalysis.VisualBasic.LookupOptions,Microsoft.CodeAnalysis.VisualBasic.Binder)
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

		Public Overrides MustOverride Function GetLocalForFunctionValue() As LocalSymbol

		Friend Overrides Sub LookupInSingleBinder(ByVal lookupResult As Microsoft.CodeAnalysis.VisualBasic.LookupResult, ByVal name As String, ByVal arity As Integer, ByVal options As LookupOptions, ByVal originalBinder As Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.SubOrFunctionBodyBinder::LookupInSingleBinder(Microsoft.CodeAnalysis.VisualBasic.LookupResult,System.String,System.Int32,Microsoft.CodeAnalysis.VisualBasic.LookupOptions,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.CompoundUseSiteInfo`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
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