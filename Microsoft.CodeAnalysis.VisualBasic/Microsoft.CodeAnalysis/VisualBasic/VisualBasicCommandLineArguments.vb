Imports Microsoft.CodeAnalysis
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.IO
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public NotInheritable Class VisualBasicCommandLineArguments
		Inherits CommandLineArguments
		Friend OutputLevel As Microsoft.CodeAnalysis.VisualBasic.OutputLevel

		Public Shadows Property CompilationOptions As VisualBasicCompilationOptions

		Protected Overrides ReadOnly Property CompilationOptionsCore As Microsoft.CodeAnalysis.CompilationOptions
			Get
				Return Me.CompilationOptions
			End Get
		End Property

		Friend Property DefaultCoreLibraryReference As Nullable(Of CommandLineReference)

		Public Shadows Property ParseOptions As VisualBasicParseOptions

		Protected Overrides ReadOnly Property ParseOptionsCore As Microsoft.CodeAnalysis.ParseOptions
			Get
				Return Me.ParseOptions
			End Get
		End Property

		Friend Sub New()
			MyBase.New()
		End Sub

		Friend Overrides Function ResolveMetadataReferences(ByVal metadataResolver As MetadataReferenceResolver, ByVal diagnostics As List(Of DiagnosticInfo), ByVal messageProvider As CommonMessageProvider, ByVal resolved As List(Of MetadataReference)) As Boolean
			Dim flag As Boolean
			Dim enumerator As List(Of MetadataReference).Enumerator = New List(Of MetadataReference).Enumerator()
			Dim flag1 As Boolean = MyBase.ResolveMetadataReferences(metadataResolver, diagnostics, messageProvider, resolved)
			If (Not Me.DefaultCoreLibraryReference.HasValue OrElse resolved.Count <= 0) Then
				flag = flag1
			Else
				Try
					enumerator = resolved.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As MetadataReference = enumerator.Current
						If (current.IsUnresolved) Then
							Continue While
						End If
						Dim properties As MetadataReferenceProperties = current.Properties
						If (properties.EmbedInteropTypes OrElse properties.Kind <> MetadataImageKind.Assembly) Then
							Continue While
						End If
						Try
							Dim metadataNoCopy As AssemblyMetadata = TryCast(DirectCast(current, Microsoft.CodeAnalysis.PortableExecutableReference).GetMetadataNoCopy(), AssemblyMetadata)
							If (metadataNoCopy Is Nothing OrElse Not metadataNoCopy.IsValidAssembly()) Then
								flag = flag1
								Return flag
							Else
								Dim assembly As PEAssembly = metadataNoCopy.GetAssembly()
								If (assembly.AssemblyReferences.Length = 0 AndAlso Not assembly.ContainsNoPiaLocalTypes() AndAlso assembly.DeclaresTheObjectClass) Then
									flag = flag1
									Return flag
								End If
							End If
						Catch badImageFormatException As System.BadImageFormatException
							ProjectData.SetProjectError(badImageFormatException)
							flag = flag1
							ProjectData.ClearProjectError()
							Return flag
						Catch oException As IOException
							ProjectData.SetProjectError(oException)
							flag = flag1
							ProjectData.ClearProjectError()
							Return flag
						End Try
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
				Dim defaultCoreLibraryReference As Nullable(Of CommandLineReference) = Me.DefaultCoreLibraryReference
				Dim portableExecutableReference As Microsoft.CodeAnalysis.PortableExecutableReference = System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of Microsoft.CodeAnalysis.PortableExecutableReference)(CommandLineArguments.ResolveMetadataReference(defaultCoreLibraryReference.Value, metadataResolver, diagnostics, messageProvider))
				If (portableExecutableReference Is Nothing OrElse portableExecutableReference.IsUnresolved) Then
					flag = False
				Else
					resolved.Insert(0, portableExecutableReference)
					flag = flag1
				End If
			End If
			Return flag
		End Function
	End Class
End Namespace