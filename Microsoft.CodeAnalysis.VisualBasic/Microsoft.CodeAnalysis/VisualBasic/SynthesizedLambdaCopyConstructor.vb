Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class SynthesizedLambdaCopyConstructor
		Inherits SynthesizedLambdaConstructor
		Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return Me._parameters
			End Get
		End Property

		Friend Sub New(ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal containingType As LambdaFrame)
			MyBase.New(syntaxNode, containingType)
			Me._parameters = ImmutableArray.Create(Of ParameterSymbol)(New SourceSimpleParameterSymbol(Me, "arg0", 0, containingType, Nothing))
		End Sub
	End Class
End Namespace