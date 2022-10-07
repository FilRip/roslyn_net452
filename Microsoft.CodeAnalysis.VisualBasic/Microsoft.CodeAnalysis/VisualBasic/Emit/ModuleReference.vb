Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Reflection

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend NotInheritable Class ModuleReference
		Implements IModuleReference, IFileReference
		Private ReadOnly _moduleBeingBuilt As PEModuleBuilder

		Private ReadOnly _underlyingModule As ModuleSymbol

		ReadOnly Property IFileReferenceFileName As String Implements IFileReference.FileName
			Get
				Return Me._underlyingModule.Name
			End Get
		End Property

		ReadOnly Property IFileReferenceHasMetadata As Boolean Implements IFileReference.HasMetadata
			Get
				Return True
			End Get
		End Property

		ReadOnly Property INamedEntityName As String
			Get
				Return Me._underlyingModule.Name
			End Get
		End Property

		Friend Sub New(ByVal moduleBeingBuilt As PEModuleBuilder, ByVal underlyingModule As ModuleSymbol)
			MyBase.New()
			Me._moduleBeingBuilt = moduleBeingBuilt
			Me._underlyingModule = underlyingModule
		End Sub

		Private Function IFileReferenceGetHashValue(ByVal algorithmId As AssemblyHashAlgorithm) As ImmutableArray(Of Byte) Implements IFileReference.GetHashValue
			Return Me._underlyingModule.GetHash(algorithmId)
		End Function

		Private Function IModuleReferenceGetContainingAssembly(ByVal context As EmitContext) As IAssemblyReference Implements IModuleReference.GetContainingAssembly
			Dim assemblyReference As IAssemblyReference
			If (Not Me._moduleBeingBuilt.OutputKind.IsNetModule() OrElse CObj(Me._moduleBeingBuilt.SourceModule.ContainingAssembly) <> CObj(Me._underlyingModule.ContainingAssembly)) Then
				assemblyReference = Me._moduleBeingBuilt.Translate(Me._underlyingModule.ContainingAssembly, context.Diagnostics)
			Else
				assemblyReference = Nothing
			End If
			Return assemblyReference
		End Function

		Private Function IReferenceAsDefinition(ByVal context As EmitContext) As IDefinition
			Return Nothing
		End Function

		Private Function IReferenceAttributes(ByVal context As EmitContext) As IEnumerable(Of ICustomAttribute)
			Return SpecializedCollections.EmptyEnumerable(Of ICustomAttribute)()
		End Function

		Private Sub IReferenceDispatch(ByVal visitor As MetadataVisitor)
			visitor.Visit(DirectCast(Me, IModuleReference))
		End Sub

		Private Function IReferenceGetInternalSymbol() As ISymbolInternal
			Return Nothing
		End Function

		Public Overrides Function ToString() As String
			Return Me._underlyingModule.ToString()
		End Function
	End Class
End Namespace