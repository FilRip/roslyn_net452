Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class DominantTypeData
		Public ResultType As TypeSymbol

		Public InferenceRestrictions As RequiredConversion

		Public IsStrictCandidate As Boolean

		Public IsUnstrictCandidate As Boolean

		Public Sub New()
			MyBase.New()
			Me.ResultType = Nothing
			Me.InferenceRestrictions = RequiredConversion.Any
			Me.IsStrictCandidate = False
			Me.IsUnstrictCandidate = False
		End Sub
	End Class
End Namespace