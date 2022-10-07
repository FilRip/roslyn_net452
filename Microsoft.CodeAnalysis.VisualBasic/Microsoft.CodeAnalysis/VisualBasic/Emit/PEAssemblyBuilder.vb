Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend NotInheritable Class PEAssemblyBuilder
		Inherits PEAssemblyBuilderBase
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

		Public Sub New(ByVal sourceAssembly As SourceAssemblySymbol, ByVal emitOptions As Microsoft.CodeAnalysis.Emit.EmitOptions, ByVal outputKind As Microsoft.CodeAnalysis.OutputKind, ByVal serializationProperties As ModulePropertiesForSerialization, ByVal manifestResources As IEnumerable(Of ResourceDescription), Optional ByVal additionalTypes As ImmutableArray(Of NamedTypeSymbol) = Nothing)
			MyBase.New(sourceAssembly, emitOptions, outputKind, serializationProperties, manifestResources, additionalTypes)
		End Sub
	End Class
End Namespace