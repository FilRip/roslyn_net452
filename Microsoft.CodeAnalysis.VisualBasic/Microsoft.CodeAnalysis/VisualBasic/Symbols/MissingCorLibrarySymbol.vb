Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable
Imports System.Reflection

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class MissingCorLibrarySymbol
		Inherits MissingAssemblySymbol
		Friend ReadOnly Shared Instance As MissingCorLibrarySymbol

		Private _lazySpecialTypes As NamedTypeSymbol()

		Shared Sub New()
			MissingCorLibrarySymbol.Instance = New MissingCorLibrarySymbol()
		End Sub

		Private Sub New()
			MyBase.New(New AssemblyIdentity("<Missing Core Assembly>", Nothing, Nothing, New ImmutableArray(Of Byte)(), False, False, AssemblyContentType.[Default]))
			MyBase.SetCorLibrary(Me)
		End Sub

		Friend Overrides Function GetDeclaredSpecialType(ByVal type As SpecialType) As NamedTypeSymbol
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol Microsoft.CodeAnalysis.VisualBasic.Symbols.MissingCorLibrarySymbol::GetDeclaredSpecialType(Microsoft.CodeAnalysis.SpecialType)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol GetDeclaredSpecialType(Microsoft.CodeAnalysis.SpecialType)
			' 
			' The unary opperator AddressReference is not supported in VisualBasic
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\DetermineNotSupportedVBCodeStep.cs:ligne 22
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function
	End Class
End Namespace