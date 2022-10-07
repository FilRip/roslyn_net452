Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Reflection.Metadata

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend MustInherit Class NamedTypeReference
		Implements INamedTypeReference
		Protected ReadOnly m_UnderlyingNamedType As NamedTypeSymbol

		Public MustOverride ReadOnly Property AsGenericTypeInstanceReference As IGenericTypeInstanceReference

		Public MustOverride ReadOnly Property AsNamespaceTypeReference As INamespaceTypeReference

		Public MustOverride ReadOnly Property AsNestedTypeReference As INestedTypeReference

		Public MustOverride ReadOnly Property AsSpecializedNestedTypeReference As ISpecializedNestedTypeReference

		ReadOnly Property INamedEntityName As String
			Get
				Return Me.m_UnderlyingNamedType.Name
			End Get
		End Property

		ReadOnly Property INamedTypeReferenceGenericParameterCount As UShort Implements INamedTypeReference.GenericParameterCount
			Get
				Return CUShort(Me.m_UnderlyingNamedType.Arity)
			End Get
		End Property

		ReadOnly Property INamedTypeReferenceMangleName As Boolean Implements INamedTypeReference.MangleName
			Get
				Return Me.m_UnderlyingNamedType.MangleName
			End Get
		End Property

		ReadOnly Property ITypeReferenceAsGenericMethodParameterReference As IGenericMethodParameterReference
			Get
				Return Nothing
			End Get
		End Property

		ReadOnly Property ITypeReferenceAsGenericTypeParameterReference As IGenericTypeParameterReference
			Get
				Return Nothing
			End Get
		End Property

		ReadOnly Property ITypeReferenceIsEnum As Boolean
			Get
				Return Me.m_UnderlyingNamedType.TypeKind = TypeKind.[Enum]
			End Get
		End Property

		ReadOnly Property ITypeReferenceIsValueType As Boolean
			Get
				Return Me.m_UnderlyingNamedType.IsValueType
			End Get
		End Property

		ReadOnly Property ITypeReferenceTypeCode As Microsoft.Cci.PrimitiveTypeCode
			Get
				Return Microsoft.Cci.PrimitiveTypeCode.NotPrimitive
			End Get
		End Property

		ReadOnly Property ITypeReferenceTypeDef As TypeDefinitionHandle
			Get
				Return New TypeDefinitionHandle()
			End Get
		End Property

		Public Sub New(ByVal underlyingNamedType As NamedTypeSymbol)
			MyBase.New()
			Me.m_UnderlyingNamedType = underlyingNamedType
		End Sub

		Public MustOverride Sub Dispatch(ByVal visitor As MetadataVisitor)

		Public NotOverridable Overrides Function Equals(ByVal obj As Object) As Boolean
			Throw ExceptionUtilities.Unreachable
		End Function

		Public NotOverridable Overrides Function GetHashCode() As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Private Function IReferenceAsDefinition(ByVal context As EmitContext) As IDefinition
			Return Nothing
		End Function

		Private Function IReferenceAttributes(ByVal context As EmitContext) As IEnumerable(Of ICustomAttribute)
			Return SpecializedCollections.EmptyEnumerable(Of ICustomAttribute)()
		End Function

		Private Function IReferenceGetInternalSymbol() As ISymbolInternal
			Return Me.m_UnderlyingNamedType
		End Function

		Private Function ITypeReferenceAsNamespaceTypeDefinition(ByVal context As EmitContext) As INamespaceTypeDefinition
			Return Nothing
		End Function

		Private Function ITypeReferenceAsNestedTypeDefinition(ByVal context As EmitContext) As INestedTypeDefinition
			Return Nothing
		End Function

		Private Function ITypeReferenceAsTypeDefinition(ByVal context As EmitContext) As ITypeDefinition
			Return Nothing
		End Function

		Private Function ITypeReferenceGetResolvedType(ByVal context As EmitContext) As ITypeDefinition
			Return Nothing
		End Function

		Public Overrides Function ToString() As String
			Return Me.m_UnderlyingNamedType.ToString()
		End Function
	End Class
End Namespace