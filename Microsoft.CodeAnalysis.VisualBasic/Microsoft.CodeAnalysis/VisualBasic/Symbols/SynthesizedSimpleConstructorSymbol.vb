Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SynthesizedSimpleConstructorSymbol
		Inherits SynthesizedConstructorBase
		Implements ISynthesizedMethodBodyImplementationSymbol
		Private _parameters As ImmutableArray(Of ParameterSymbol)

		Friend NotOverridable Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return False
			End Get
		End Property

		Public ReadOnly Property HasMethodBodyDependency As Boolean Implements ISynthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency
			Get
				Return False
			End Get
		End Property

		Public ReadOnly Property Method As IMethodSymbolInternal Implements ISynthesizedMethodBodyImplementationSymbol.Method
			Get
				Return DirectCast(MyBase.ContainingSymbol, ISynthesizedMethodBodyImplementationSymbol).Method
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ParameterCount As Integer
			Get
				Return Me._parameters.Length
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return Me._parameters
			End Get
		End Property

		Public Sub New(ByVal container As NamedTypeSymbol)
			MyBase.New(VisualBasicSyntaxTree.DummyReference, container, False, Nothing, Nothing)
		End Sub

		Friend NotOverridable Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Sub SetParameters(ByVal parameters As ImmutableArray(Of ParameterSymbol))
			Me._parameters = parameters
		End Sub
	End Class
End Namespace