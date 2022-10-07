Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Reflection

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend MustInherit Class PEAssemblyBuilderBase
		Inherits PEModuleBuilder
		Implements IAssemblyReference
		Protected ReadOnly m_SourceAssembly As SourceAssemblySymbol

		Private ReadOnly _additionalTypes As ImmutableArray(Of NamedTypeSymbol)

		Private _lazyFiles As ImmutableArray(Of IFileReference)

		Private _lazyFilesWithoutManifestResources As ImmutableArray(Of IFileReference)

		Private ReadOnly _metadataName As String

		Public ReadOnly Property AssemblyVersionPattern As Version Implements IAssemblyReference.AssemblyVersionPattern
			Get
				Return Me.m_SourceAssembly.AssemblyVersionPattern
			End Get
		End Property

		Public ReadOnly Property Identity As AssemblyIdentity Implements IAssemblyReference.Identity
			Get
				Return Me.m_SourceAssembly.Identity
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._metadataName
			End Get
		End Property

		Public Overrides ReadOnly Property SourceAssemblyOpt As ISourceAssemblySymbolInternal
			Get
				Return Me.m_SourceAssembly
			End Get
		End Property

		Public Sub New(ByVal sourceAssembly As SourceAssemblySymbol, ByVal emitOptions As Microsoft.CodeAnalysis.Emit.EmitOptions, ByVal outputKind As Microsoft.CodeAnalysis.OutputKind, ByVal serializationProperties As ModulePropertiesForSerialization, ByVal manifestResources As IEnumerable(Of ResourceDescription), ByVal additionalTypes As ImmutableArray(Of NamedTypeSymbol))
			MyBase.New(DirectCast(sourceAssembly.Modules(0), SourceModuleSymbol), emitOptions, outputKind, serializationProperties, manifestResources)
			Me.m_SourceAssembly = sourceAssembly
			Me._additionalTypes = Microsoft.CodeAnalysis.ImmutableArrayExtensions.NullToEmpty(Of NamedTypeSymbol)(additionalTypes)
			Me._metadataName = If(emitOptions.OutputNameOverride Is Nothing, sourceAssembly.MetadataName, FileNameUtilities.ChangeExtension(emitOptions.OutputNameOverride, Nothing))
			Me.m_AssemblyOrModuleSymbolToModuleRefMap.Add(sourceAssembly, Me)
		End Sub

		Protected Overrides Sub AddEmbeddedResourcesFromAddedModules(ByVal builder As ArrayBuilder(Of ManagedResource), ByVal diagnostics As DiagnosticBag)
			Dim modules As ImmutableArray(Of ModuleSymbol) = Me.m_SourceAssembly.Modules
			Dim length As Integer = modules.Length - 1
			For i As Integer = 1 To length
				Dim fileReference As IFileReference = DirectCast(MyBase.Translate(modules(i), diagnostics), IFileReference)
				Try
					Dim embeddedResourcesOrThrow As ImmutableArray(Of EmbeddedResource) = DirectCast(modules(i), PEModuleSymbol).[Module].GetEmbeddedResourcesOrThrow()
					Dim enumerator As ImmutableArray(Of EmbeddedResource).Enumerator = embeddedResourcesOrThrow.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As EmbeddedResource = enumerator.Current
						builder.Add(New ManagedResource(current.Name, CInt((current.Attributes And ManifestResourceAttributes.[Public])) <> 0, Nothing, fileReference, current.Offset))
					End While
				Catch badImageFormatException As System.BadImageFormatException
					ProjectData.SetProjectError(badImageFormatException)
					diagnostics.Add(ERRID.ERR_UnsupportedModule1, NoLocation.Singleton, New [Object]() { modules(i) })
					ProjectData.ClearProjectError()
				End Try
			Next

		End Sub

		Private Shared Function Free(ByVal builder As ArrayBuilder(Of IFileReference)) As Boolean
			builder.Free()
			Return False
		End Function

		Public Overrides Function GetAdditionalTopLevelTypes() As ImmutableArray(Of NamedTypeSymbol)
			Return Me._additionalTypes
		End Function

		Public Overrides Function GetEmbeddedTypes(ByVal diagnostics As DiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Public NotOverridable Overrides Function GetFiles(ByVal context As EmitContext) As IEnumerable(Of IFileReference)
			Dim fileReferences As IEnumerable(Of IFileReference)
			fileReferences = If(context.IsRefAssembly, Me.GetFilesCore(context, Me._lazyFilesWithoutManifestResources), Me.GetFilesCore(context, Me._lazyFiles))
			Return fileReferences
		End Function

		Private Function GetFilesCore(ByVal context As EmitContext, ByRef lazyFiles As ImmutableArray(Of IFileReference)) As IEnumerable(Of IFileReference)
			Dim enumerator As IEnumerator(Of ResourceDescription) = Nothing
			If (lazyFiles.IsDefault) Then
				Dim instance As ArrayBuilder(Of IFileReference) = ArrayBuilder(Of IFileReference).GetInstance()
				Try
					Dim modules As ImmutableArray(Of ModuleSymbol) = Me.m_SourceAssembly.Modules
					Dim length As Integer = modules.Length - 1
					Dim num As Integer = 1
					Do
						instance.Add(DirectCast(MyBase.Translate(modules(num), context.Diagnostics), IFileReference))
						num = num + 1
					Loop While num <= length
					If (Not context.IsRefAssembly) Then
						Try
							enumerator = Me.ManifestResources.GetEnumerator()
							While enumerator.MoveNext()
								Dim current As ResourceDescription = enumerator.Current
								If (current.IsEmbedded) Then
									Continue While
								End If
								instance.Add(current)
							End While
						Finally
							If (enumerator IsNot Nothing) Then
								enumerator.Dispose()
							End If
						End Try
					End If
					If (ImmutableInterlocked.InterlockedInitialize(Of IFileReference)(lazyFiles, instance.ToImmutable()) AndAlso lazyFiles.Length > 0 AndAlso Not CryptographicHashProvider.IsSupportedAlgorithm(Me.m_SourceAssembly.HashAlgorithm)) Then
						context.Diagnostics.Add(New VBDiagnostic(ErrorFactory.ErrorInfo(ERRID.ERR_CryptoHashFailed), NoLocation.Singleton, False))
					End If
				Finally
					instance.Free()
				End Try
			End If
			Return DirectCast(lazyFiles, IEnumerable(Of IFileReference))
		End Function
	End Class
End Namespace