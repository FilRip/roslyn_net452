Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Module SynthesizedLambdaKindExtensions
		<Extension>
		Friend Function IsQueryLambda(ByVal kind As SynthesizedLambdaKind) As Boolean
			If (kind < SynthesizedLambdaKind.FilterConditionQueryLambda) Then
				Return False
			End If
			Return kind <= SynthesizedLambdaKind.ConversionNonUserCodeQueryLambda
		End Function
	End Module
End Namespace