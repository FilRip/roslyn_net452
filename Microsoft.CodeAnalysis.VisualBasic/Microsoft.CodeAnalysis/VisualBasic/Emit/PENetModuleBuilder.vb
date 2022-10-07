Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend NotInheritable Class PENetModuleBuilder
		Inherits PEModuleBuilder
		Friend Overrides ReadOnly Property AllowOmissionOfConditionalCalls As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property CurrentGenerationOrdinal As Integer
			Get
				Return 0
			End Get
		End Property

		Public Overrides ReadOnly Property SourceAssemblyOpt As ISourceAssemblySymbolInternal
			Get
				Return Nothing
			End Get
		End Property

		Friend Sub New(ByVal sourceModule As SourceModuleSymbol, ByVal emitOptions As Microsoft.CodeAnalysis.Emit.EmitOptions, ByVal serializationProperties As ModulePropertiesForSerialization, ByVal manifestResources As IEnumerable(Of ResourceDescription))
			MyBase.New(sourceModule, emitOptions, OutputKind.NetModule, serializationProperties, manifestResources)
		End Sub

		Protected Overrides Sub AddEmbeddedResourcesFromAddedModules(ByVal builder As ArrayBuilder(Of ManagedResource), ByVal diagnostics As DiagnosticBag)
			Throw ExceptionUtilities.Unreachable
		End Sub

		Public Overrides Function GetFiles(ByVal context As EmitContext) As IEnumerable(Of IFileReference)
			Return SpecializedCollections.EmptyEnumerable(Of IFileReference)()
		End Function
	End Class
End Namespace