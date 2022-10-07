Imports Microsoft.CodeAnalysis
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class VisualBasicRequiredLanguageVersion
		Inherits RequiredLanguageVersion
		Friend ReadOnly Property Version As LanguageVersion

		Friend Sub New(ByVal version As LanguageVersion)
			MyBase.New()
			Me.Version = version
		End Sub

		Public Overrides Function ToString() As String
			Return Me.Version.ToDisplayString()
		End Function
	End Class
End Namespace