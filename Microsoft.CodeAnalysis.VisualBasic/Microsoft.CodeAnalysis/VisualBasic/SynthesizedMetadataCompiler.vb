Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class SynthesizedMetadataCompiler
		Inherits VisualBasicSymbolVisitor
		Private ReadOnly _moduleBeingBuilt As PEModuleBuilder

		Private ReadOnly _cancellationToken As CancellationToken

		Private Sub New(ByVal moduleBeingBuilt As PEModuleBuilder, ByVal cancellationToken As System.Threading.CancellationToken)
			MyBase.New()
			Me._moduleBeingBuilt = moduleBeingBuilt
			Me._cancellationToken = cancellationToken
		End Sub

		Friend Shared Sub ProcessSynthesizedMembers(ByVal compilation As VisualBasicCompilation, ByVal moduleBeingBuilt As PEModuleBuilder, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing)
			Dim synthesizedMetadataCompiler As Microsoft.CodeAnalysis.VisualBasic.SynthesizedMetadataCompiler = New Microsoft.CodeAnalysis.VisualBasic.SynthesizedMetadataCompiler(moduleBeingBuilt, cancellationToken)
			compilation.SourceModule.GlobalNamespace.Accept(synthesizedMetadataCompiler)
		End Sub

		Public Overrides Sub VisitNamedType(ByVal symbol As NamedTypeSymbol)
			Me._cancellationToken.ThrowIfCancellationRequested()
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = symbol.GetMembers().GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator.Current
				If (current.Kind <> SymbolKind.NamedType) Then
					Continue While
				End If
				current.Accept(Me)
			End While
		End Sub

		Public Overrides Sub VisitNamespace(ByVal symbol As NamespaceSymbol)
			Me._cancellationToken.ThrowIfCancellationRequested()
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = symbol.GetMembers().GetEnumerator()
			While enumerator.MoveNext()
				enumerator.Current.Accept(Me)
			End While
		End Sub
	End Class
End Namespace