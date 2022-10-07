Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend NotInheritable Class GenericNestedTypeInstanceReference
		Inherits GenericTypeInstanceReference
		Implements INestedTypeReference
		Public Overrides ReadOnly Property AsGenericTypeInstanceReference As IGenericTypeInstanceReference
			Get
				Return Me
			End Get
		End Property

		Public Overrides ReadOnly Property AsNamespaceTypeReference As INamespaceTypeReference
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property AsNestedTypeReference As INestedTypeReference
			Get
				Return Me
			End Get
		End Property

		Public Overrides ReadOnly Property AsSpecializedNestedTypeReference As ISpecializedNestedTypeReference
			Get
				Return Nothing
			End Get
		End Property

		Public Sub New(ByVal underlyingNamedType As NamedTypeSymbol)
			MyBase.New(underlyingNamedType)
		End Sub

		Private Function ITypeMemberReferenceGetContainingType(ByVal context As EmitContext) As ITypeReference
			Return DirectCast(context.[Module], PEModuleBuilder).Translate(Me.m_UnderlyingNamedType.ContainingType, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics, False, False)
		End Function
	End Class
End Namespace