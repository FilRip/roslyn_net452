Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SourceTypeParameterOnTypeSymbol
		Inherits SourceTypeParameterSymbol
		Private ReadOnly _container As SourceNamedTypeSymbol

		Private ReadOnly _syntaxRefs As ImmutableArray(Of SyntaxReference)

		Private _lazyVariance As VarianceKind

		Protected Overrides ReadOnly Property ContainerTypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Return Me._container.TypeParameters
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._container
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Symbol.GetDeclaringSyntaxReferenceHelper(Me._syntaxRefs)
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Dim instance As ArrayBuilder(Of Location) = ArrayBuilder(Of Location).GetInstance()
				Dim enumerator As ImmutableArray(Of SyntaxReference).Enumerator = Me._syntaxRefs.GetEnumerator()
				While enumerator.MoveNext()
					instance.Add(SourceTypeParameterSymbol.GetSymbolLocation(enumerator.Current))
				End While
				Return instance.ToImmutableAndFree()
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameterKind As Microsoft.CodeAnalysis.TypeParameterKind
			Get
				Return Microsoft.CodeAnalysis.TypeParameterKind.Type
			End Get
		End Property

		Public Overrides ReadOnly Property Variance As VarianceKind
			Get
				Me.EnsureAllConstraintsAreResolved()
				Return Me._lazyVariance
			End Get
		End Property

		Public Sub New(ByVal container As SourceNamedTypeSymbol, ByVal ordinal As Integer, ByVal name As String, ByVal syntaxRefs As ImmutableArray(Of SyntaxReference))
			MyBase.New(ordinal, name)
			Me._container = container
			Me._syntaxRefs = syntaxRefs
		End Sub

		Protected Overrides Function GetDeclaredConstraints(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of TypeParameterConstraint)
			Dim varianceKind As Microsoft.CodeAnalysis.VarianceKind
			Dim typeParameterConstraints As ImmutableArray(Of TypeParameterConstraint) = New ImmutableArray(Of TypeParameterConstraint)()
			Me._container.BindTypeParameterConstraints(Me, varianceKind, typeParameterConstraints, diagnostics)
			Me._lazyVariance = varianceKind
			Return typeParameterConstraints
		End Function

		Protected Overrides Function ReportRedundantConstraints() As Boolean
			Return True
		End Function
	End Class
End Namespace