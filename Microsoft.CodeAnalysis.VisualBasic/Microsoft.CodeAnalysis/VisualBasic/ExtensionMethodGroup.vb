Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class ExtensionMethodGroup
		Private ReadOnly _lookupBinder As Binder

		Private ReadOnly _lookupOptions As LookupOptions

		Private ReadOnly _withDependencies As Boolean

		Private _lazyMethods As ImmutableArray(Of MethodSymbol)

		Private _lazyUseSiteDiagnostics As IReadOnlyCollection(Of DiagnosticInfo)

		Private _lazyUseSiteDependencies As IReadOnlyCollection(Of AssemblySymbol)

		Public Sub New(ByVal lookupBinder As Binder, ByVal lookupOptions As Microsoft.CodeAnalysis.VisualBasic.LookupOptions, ByVal withDependencies As Boolean)
			MyBase.New()
			Me._lookupBinder = lookupBinder
			Me._lookupOptions = lookupOptions
			Me._withDependencies = withDependencies
		End Sub

		Public Function LazyLookupAdditionalExtensionMethods(ByVal group As BoundMethodGroup, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ImmutableArray(Of MethodSymbol)
			Dim methods As ImmutableArray(Of MethodSymbol)
			Dim dependencies As IReadOnlyCollection(Of AssemblySymbol)
			If (Me._lazyMethods.IsDefault) Then
				Dim receiverOpt As BoundExpression = group.ReceiverOpt
				Dim empty As ImmutableArray(Of MethodSymbol) = ImmutableArray(Of MethodSymbol).Empty
				Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = If(Me._withDependencies, New CompoundUseSiteInfo(Of AssemblySymbol)(Me._lookupBinder.Compilation.Assembly), CompoundUseSiteInfo(Of AssemblySymbol).DiscardedDependencies)
				If (receiverOpt IsNot Nothing AndAlso receiverOpt.Type IsNot Nothing) Then
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.LookupResult = Microsoft.CodeAnalysis.VisualBasic.LookupResult.GetInstance()
					Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me._lookupBinder
					Dim lookupResult As Microsoft.CodeAnalysis.VisualBasic.LookupResult = instance
					Dim type As TypeSymbol = receiverOpt.Type
					methods = group.Methods
					binder.LookupExtensionMethods(lookupResult, type, methods(0).Name, If(group.TypeArgumentsOpt Is Nothing, 0, group.TypeArgumentsOpt.Arguments.Length), Me._lookupOptions, compoundUseSiteInfo)
					If (instance.IsGood) Then
						empty = instance.Symbols.ToDowncastedImmutable(Of MethodSymbol)()
					End If
					instance.Free()
				End If
				Interlocked.CompareExchange(Of IReadOnlyCollection(Of DiagnosticInfo))(Me._lazyUseSiteDiagnostics, compoundUseSiteInfo.Diagnostics, Nothing)
				If (compoundUseSiteInfo.AccumulatesDependencies) Then
					dependencies = compoundUseSiteInfo.Dependencies
				Else
					dependencies = Nothing
				End If
				Interlocked.CompareExchange(Of IReadOnlyCollection(Of AssemblySymbol))(Me._lazyUseSiteDependencies, dependencies, Nothing)
				methods = New ImmutableArray(Of MethodSymbol)()
				ImmutableInterlocked.InterlockedCompareExchange(Of MethodSymbol)(Me._lazyMethods, empty, methods)
			End If
			useSiteInfo.AddDiagnostics(Volatile.Read(Of IReadOnlyCollection(Of DiagnosticInfo))(Me._lazyUseSiteDiagnostics))
			useSiteInfo.AddDependencies(Volatile.Read(Of IReadOnlyCollection(Of AssemblySymbol))(Me._lazyUseSiteDependencies))
			Return Me._lazyMethods
		End Function
	End Class
End Namespace