Imports Microsoft.CodeAnalysis
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public Module PredefinedPreprocessorSymbols
		Friend ReadOnly Property CurrentVersionNumber As Double
			Get
				Return [Double].Parse(LanguageVersion.Latest.MapSpecifiedToEffectiveVersion().GetErrorName(), CultureInfo.InvariantCulture)
			End Get
		End Property

		Public Function AddPredefinedPreprocessorSymbols(ByVal kind As OutputKind, ByVal symbols As IEnumerable(Of KeyValuePair(Of String, Object))) As ImmutableArray(Of KeyValuePair(Of String, Object))
			Return PredefinedPreprocessorSymbols.AddPredefinedPreprocessorSymbols(kind, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of KeyValuePair(Of String, Object))(symbols))
		End Function

		Public Function AddPredefinedPreprocessorSymbols(ByVal kind As OutputKind, ByVal ParamArray symbols As KeyValuePair(Of String, Object)()) As ImmutableArray(Of KeyValuePair(Of String, Object))
			Return PredefinedPreprocessorSymbols.AddPredefinedPreprocessorSymbols(kind, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of KeyValuePair(Of String, Object))(symbols))
		End Function

		Public Function AddPredefinedPreprocessorSymbols(ByVal kind As OutputKind, ByVal symbols As ImmutableArray(Of KeyValuePair(Of String, Object))) As ImmutableArray(Of KeyValuePair(Of String, Object))
			Dim func As Func(Of KeyValuePair(Of String, Object), Boolean)
			Dim func1 As Func(Of KeyValuePair(Of String, Object), Boolean)
			If (Not kind.IsValid()) Then
				Throw New ArgumentOutOfRangeException("kind")
			End If
			If (symbols.IsDefault) Then
				symbols = ImmutableArray(Of KeyValuePair(Of String, Object)).Empty
			End If
			Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of String, Object)) = symbols
			If (PredefinedPreprocessorSymbols._Closure$__.$I4-0 Is Nothing) Then
				func = Function(entry As KeyValuePair(Of String, Object)) CaseInsensitiveComparison.Equals(entry.Key, "VBC_VER")
				PredefinedPreprocessorSymbols._Closure$__.$I4-0 = func
			Else
				func = PredefinedPreprocessorSymbols._Closure$__.$I4-0
			End If
			If (System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of KeyValuePair(Of String, Object))(keyValuePairs, func).Key Is Nothing) Then
				symbols = symbols.Add(New KeyValuePair(Of String, Object)("VBC_VER", PredefinedPreprocessorSymbols.CurrentVersionNumber))
			End If
			Dim keyValuePairs1 As ImmutableArray(Of KeyValuePair(Of String, Object)) = symbols
			If (PredefinedPreprocessorSymbols._Closure$__.$I4-1 Is Nothing) Then
				func1 = Function(entry As KeyValuePair(Of String, Object)) CaseInsensitiveComparison.Equals(entry.Key, "TARGET")
				PredefinedPreprocessorSymbols._Closure$__.$I4-1 = func1
			Else
				func1 = PredefinedPreprocessorSymbols._Closure$__.$I4-1
			End If
			If (System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of KeyValuePair(Of String, Object))(keyValuePairs1, func1).Key Is Nothing) Then
				symbols = symbols.Add(New KeyValuePair(Of String, Object)("TARGET", PredefinedPreprocessorSymbols.GetTargetString(kind)))
			End If
			Return symbols
		End Function

		Friend Function GetTargetString(ByVal kind As OutputKind) As String
			Dim str As String
			Select Case kind
				Case OutputKind.ConsoleApplication
					str = "exe"
					Exit Select
				Case OutputKind.WindowsApplication
					str = "winexe"
					Exit Select
				Case OutputKind.DynamicallyLinkedLibrary
					str = "library"
					Exit Select
				Case OutputKind.NetModule
					str = "module"
					Exit Select
				Case OutputKind.WindowsRuntimeMetadata
					str = "winmdobj"
					Exit Select
				Case OutputKind.WindowsRuntimeApplication
					str = "appcontainerexe"
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(kind)
			End Select
			Return str
		End Function
	End Module
End Namespace