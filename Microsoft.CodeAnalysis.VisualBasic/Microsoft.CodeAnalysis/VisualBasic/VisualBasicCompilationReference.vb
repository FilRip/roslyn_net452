Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	<DebuggerDisplay("{GetDebuggerDisplay(), nq}")>
	Friend NotInheritable Class VisualBasicCompilationReference
		Inherits CompilationReference
		Private ReadOnly _compilation As VisualBasicCompilation

		Public Shadows ReadOnly Property Compilation As VisualBasicCompilation
			Get
				Return Me._compilation
			End Get
		End Property

		Friend Overrides ReadOnly Property CompilationCore As Microsoft.CodeAnalysis.Compilation
			Get
				Return Me._compilation
			End Get
		End Property

		Public Sub New(ByVal compilation As VisualBasicCompilation, Optional ByVal aliases As ImmutableArray(Of String) = Nothing, Optional ByVal embedInteropTypes As Boolean = False)
			MyBase.New(CompilationReference.GetProperties(compilation, aliases, embedInteropTypes))
			Me._compilation = compilation
		End Sub

		Private Sub New(ByVal compilation As VisualBasicCompilation, ByVal properties As MetadataReferenceProperties)
			MyBase.New(properties)
			Me._compilation = compilation
		End Sub

		Private Function GetDebuggerDisplay() As String
			Return [String].Concat(VBResources.CompilationVisualBasic, Me._compilation.AssemblyName)
		End Function

		Friend Overrides Function WithPropertiesImpl(ByVal properties As MetadataReferenceProperties) As CompilationReference
			Return New VisualBasicCompilationReference(Me._compilation, properties)
		End Function
	End Class
End Namespace