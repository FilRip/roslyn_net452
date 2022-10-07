Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module LanguageVersionEnumBounds
		<Extension>
		Friend Function GetErrorName(ByVal value As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion) As String
			Dim str As String
			Dim languageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion = value
			If (languageVersion <= Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic15_3) Then
				Select Case languageVersion
					Case Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic9
						str = "9.0"
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic10
						str = "10.0"
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic11
						str = "11.0"
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic12
						str = "12.0"
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic9 Or Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic12
						Throw ExceptionUtilities.UnexpectedValue(value)
					Case Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic14
						str = "14.0"
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic15
						str = "15.0"
						Exit Select
					Case Else
						If (languageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic15_3) Then
							str = "15.3"
							Exit Select
						Else
							Throw ExceptionUtilities.UnexpectedValue(value)
						End If
				End Select
			ElseIf (languageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic15_5) Then
				str = "15.5"
			ElseIf (languageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic16) Then
				str = "16"
			Else
				If (languageVersion <> Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic16_9) Then
					Throw ExceptionUtilities.UnexpectedValue(value)
				End If
				str = "16.9"
			End If
			Return str
		End Function

		<Extension>
		Friend Function IsValid(ByVal value As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion) As Boolean
			Dim flag As Boolean
			Dim languageVersion As Microsoft.CodeAnalysis.VisualBasic.LanguageVersion = value
			If (languageVersion <= Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic15_3) Then
				If (CInt(languageVersion) - CInt(Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic9) <= 3 OrElse CInt(languageVersion) - CInt(Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic14) <= 1 OrElse languageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic15_3) Then
					flag = True
					Return flag
				End If
				flag = False
				Return flag
			ElseIf (languageVersion <> Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic15_5 AndAlso languageVersion <> Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic16 AndAlso languageVersion <> Microsoft.CodeAnalysis.VisualBasic.LanguageVersion.VisualBasic16_9) Then
				flag = False
				Return flag
			End If
			flag = True
			Return flag
		End Function
	End Module
End Namespace