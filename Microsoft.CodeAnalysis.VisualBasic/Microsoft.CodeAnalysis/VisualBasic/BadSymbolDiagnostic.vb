Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BadSymbolDiagnostic
		Inherits DiagnosticInfo
		Implements IDiagnosticInfoWithSymbols
		Private ReadOnly _badSymbol As Symbol

		Public Overrides ReadOnly Property AdditionalLocations As IReadOnlyList(Of Location)
			Get
				Return DirectCast(Me._badSymbol.Locations, IReadOnlyList(Of Location))
			End Get
		End Property

		Public ReadOnly Property BadSymbol As Symbol
			Get
				Return Me._badSymbol
			End Get
		End Property

		Friend Sub New(ByVal badSymbol As Symbol, ByVal errid As Microsoft.CodeAnalysis.VisualBasic.ERRID)
			MyClass.New(badSymbol, errid, New [Object]() { badSymbol })
		End Sub

		Friend Sub New(ByVal badSymbol As Symbol, ByVal errid As Microsoft.CodeAnalysis.VisualBasic.ERRID, ByVal ParamArray additionalArgs As Object())
			MyBase.New(MessageProvider.Instance, errid, additionalArgs)
			Me._badSymbol = badSymbol
		End Sub

		Private Sub GetAssociatedSymbols(ByVal builder As ArrayBuilder(Of Symbol)) Implements IDiagnosticInfoWithSymbols.GetAssociatedSymbols
			builder.Add(Me._badSymbol)
		End Sub
	End Class
End Namespace