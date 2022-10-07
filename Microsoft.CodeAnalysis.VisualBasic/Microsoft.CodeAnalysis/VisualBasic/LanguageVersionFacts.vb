Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public Module LanguageVersionFacts
		Friend ReadOnly Property CurrentVersion As LanguageVersion
			Get
				Return LanguageVersion.VisualBasic16_9
			End Get
		End Property

		<Extension>
		Friend Function AllowNonTrailingNamedArguments(ByVal self As LanguageVersion) As Boolean
			Return self >= Feature.NonTrailingNamedArguments.GetLanguageVersion()
		End Function

		<Extension>
		Friend Function DisallowInferredTupleElementNames(ByVal self As LanguageVersion) As Boolean
			Return self < Feature.InferredTupleNames.GetLanguageVersion()
		End Function

		<Extension>
		Public Function MapSpecifiedToEffectiveVersion(ByVal version As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion) As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion
			Dim languageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion
			Dim languageVersion1 As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion = version
			If (languageVersion1 = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.[Default]) Then
				languageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic16
			Else
				languageVersion = If(languageVersion1 <> Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.Latest, version, Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic16_9)
			End If
			Return languageVersion
		End Function

		<Extension>
		Public Function ToDisplayString(ByVal version As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion) As String
			Dim str As String
			Dim languageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion = version
			If (languageVersion <= Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic15_3) Then
				If (languageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.[Default]) Then
					str = "default"
				Else
					Select Case languageVersion
						Case Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic9
							str = "9"
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic10
							str = "10"
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic11
							str = "11"
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic12
							str = "12"
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic9 Or Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic12
							Throw ExceptionUtilities.UnexpectedValue(version)
						Case Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic14
							str = "14"
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic15
							str = "15"
							Exit Select
						Case Else
							If (languageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic15_3) Then
								str = "15.3"
								Exit Select
							Else
								Throw ExceptionUtilities.UnexpectedValue(version)
							End If
					End Select
				End If
			ElseIf (languageVersion <= Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic16) Then
				If (languageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic15_5) Then
					str = "15.5"
				Else
					If (languageVersion <> Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic16) Then
						Throw ExceptionUtilities.UnexpectedValue(version)
					End If
					str = "16"
				End If
			ElseIf (languageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic16_9) Then
				str = "16.9"
			Else
				If (languageVersion <> Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.Latest) Then
					Throw ExceptionUtilities.UnexpectedValue(version)
				End If
				str = "latest"
			End If
			Return str
		End Function

		Public Function TryParse(ByVal version As String, ByRef result As LanguageVersion) As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.LanguageVersionFacts::TryParse(System.String,Microsoft.CodeAnalysis.VisualBasic.LanguageVersion&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean TryParse(System.String,Microsoft.CodeAnalysis.VisualBasic.LanguageVersion&)
			' 
			' L'index était hors limites. Il ne doit pas être négatif et doit être inférieur à la taille de la collection.
			' Nom du paramètre : index
			'    à System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
			'    à ..(Int32 ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:ligne 364
			'    à ..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:ligne 74
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:ligne 55
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function
	End Module
End Namespace