Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Diagnostics
Imports System
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public Class VisualBasicGeneratorDriver
		Inherits GeneratorDriver
		Friend Overrides ReadOnly Property MessageProvider As CommonMessageProvider
			Get
				Return Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance
			End Get
		End Property

		Private Sub New(ByVal state As GeneratorDriverState)
			MyBase.New(state)
		End Sub

		Friend Sub New(ByVal parseOptions As VisualBasicParseOptions, ByVal generators As ImmutableArray(Of ISourceGenerator), ByVal optionsProvider As AnalyzerConfigOptionsProvider, ByVal additionalTexts As ImmutableArray(Of AdditionalText))
			MyBase.New(parseOptions, generators, optionsProvider, additionalTexts)
		End Sub

		Public Shared Function Create(ByVal generators As ImmutableArray(Of ISourceGenerator), Optional ByVal additionalTexts As ImmutableArray(Of AdditionalText) = Nothing, Optional ByVal parseOptions As VisualBasicParseOptions = Nothing, Optional ByVal analyzerConfigOptionsProvider As Microsoft.CodeAnalysis.Diagnostics.AnalyzerConfigOptionsProvider = Nothing) As VisualBasicGeneratorDriver
			Return New VisualBasicGeneratorDriver(parseOptions, generators, analyzerConfigOptionsProvider, additionalTexts)
		End Function

		Friend Overrides Function CreateSourcesCollection() As AdditionalSourcesCollection
			Return New AdditionalSourcesCollection(".vb")
		End Function

		Friend Overrides Function FromState(ByVal state As GeneratorDriverState) As GeneratorDriver
			Return New VisualBasicGeneratorDriver(state)
		End Function

		Friend Overrides Function ParseGeneratedSourceText(ByVal input As GeneratedSourceText, ByVal fileName As String, ByVal cancellationToken As System.Threading.CancellationToken) As SyntaxTree
			Return VisualBasicSyntaxTree.ParseTextLazy(input.Text, DirectCast(Me._state.ParseOptions, VisualBasicParseOptions), fileName)
		End Function
	End Class
End Namespace