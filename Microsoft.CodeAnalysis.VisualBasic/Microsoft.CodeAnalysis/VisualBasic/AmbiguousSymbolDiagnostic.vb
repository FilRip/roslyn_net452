Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class AmbiguousSymbolDiagnostic
		Inherits DiagnosticInfo
		Implements IDiagnosticInfoWithSymbols
		Private _symbols As ImmutableArray(Of Symbol)

		Public Overrides ReadOnly Property AdditionalLocations As IReadOnlyList(Of Location)
			Get
				Dim instance As ArrayBuilder(Of Location) = ArrayBuilder(Of Location).GetInstance()
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = Me._symbols.GetEnumerator()
				While enumerator.MoveNext()
					Dim enumerator1 As ImmutableArray(Of Location).Enumerator = enumerator.Current.Locations.GetEnumerator()
					While enumerator1.MoveNext()
						instance.Add(enumerator1.Current)
					End While
				End While
				Return DirectCast(instance.ToImmutableAndFree(), IReadOnlyList(Of Location))
			End Get
		End Property

		Public ReadOnly Property AmbiguousSymbols As ImmutableArray(Of Symbol)
			Get
				Return Me._symbols
			End Get
		End Property

		Friend Sub New(ByVal errid As Microsoft.CodeAnalysis.VisualBasic.ERRID, ByVal symbols As ImmutableArray(Of Symbol), ByVal ParamArray args As Object())
			MyBase.New(MessageProvider.Instance, errid, args)
			Me._symbols = symbols
		End Sub

		Private Sub GetAssociatedSymbols(ByVal builder As ArrayBuilder(Of Symbol)) Implements IDiagnosticInfoWithSymbols.GetAssociatedSymbols
			builder.AddRange(Me._symbols)
		End Sub
	End Class
End Namespace