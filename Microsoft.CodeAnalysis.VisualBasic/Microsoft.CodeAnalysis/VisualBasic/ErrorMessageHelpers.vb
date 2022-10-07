Imports Microsoft.CodeAnalysis
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Module ErrorMessageHelpers
		<Extension>
		Public Function ToDisplay(ByVal access As Accessibility) As String
			Dim str As String
			Select Case access
				Case Accessibility.NotApplicable
					str = ""
					Exit Select
				Case Accessibility.[Private]
					str = "Private"
					Exit Select
				Case Accessibility.ProtectedAndInternal
					str = "Private Protected"
					Exit Select
				Case Accessibility.[Protected]
					str = "Protected"
					Exit Select
				Case Accessibility.Internal
					str = "Friend"
					Exit Select
				Case Accessibility.ProtectedOrInternal
					str = "Protected Friend"
					Exit Select
				Case Accessibility.[Public]
					str = "Public"
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(access)
			End Select
			Return str
		End Function
	End Module
End Namespace