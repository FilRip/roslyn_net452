Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend NotInheritable Class AssemblyReference
		Implements IAssemblyReference
		Private ReadOnly _targetAssembly As AssemblySymbol

		Public ReadOnly Property AssemblyVersionPattern As Version Implements IAssemblyReference.AssemblyVersionPattern
			Get
				Return Me._targetAssembly.AssemblyVersionPattern
			End Get
		End Property

		Public ReadOnly Property Identity As AssemblyIdentity Implements IAssemblyReference.Identity
			Get
				Return Me._targetAssembly.Identity
			End Get
		End Property

		ReadOnly Property INamedEntityName As String
			Get
				Return Me.Identity.Name
			End Get
		End Property

		Public Sub New(ByVal assemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol)
			MyBase.New()
			Me._targetAssembly = assemblySymbol
		End Sub

		Private Function IModuleReferenceGetContainingAssembly(ByVal context As EmitContext) As IAssemblyReference
			Return Me
		End Function

		Private Function IReferenceAsDefinition(ByVal context As EmitContext) As IDefinition
			Return Nothing
		End Function

		Private Function IReferenceAttributes(ByVal context As EmitContext) As IEnumerable(Of ICustomAttribute)
			Return SpecializedCollections.EmptyEnumerable(Of ICustomAttribute)()
		End Function

		Private Sub IReferenceDispatch(ByVal visitor As MetadataVisitor)
			visitor.Visit(Me)
		End Sub

		Private Function IReferenceGetInternalSymbol() As ISymbolInternal
			Return Nothing
		End Function

		Public Overrides Function ToString() As String
			Return Me._targetAssembly.ToString()
		End Function
	End Class
End Namespace