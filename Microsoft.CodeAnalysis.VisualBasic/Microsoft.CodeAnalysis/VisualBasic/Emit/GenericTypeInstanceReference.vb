Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend MustInherit Class GenericTypeInstanceReference
		Inherits NamedTypeReference
		Implements IGenericTypeInstanceReference
		Public Sub New(ByVal underlyingNamedType As NamedTypeSymbol)
			MyBase.New(underlyingNamedType)
		End Sub

		Public NotOverridable Overrides Sub Dispatch(ByVal visitor As MetadataVisitor)
			visitor.Visit(Me)
		End Sub

		Private Function IGenericTypeInstanceReferenceGetGenericArguments(ByVal context As EmitContext) As ImmutableArray(Of ITypeReference) Implements IGenericTypeInstanceReference.GetGenericArguments
			Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
			Dim instance As ArrayBuilder(Of ITypeReference) = ArrayBuilder(Of ITypeReference).GetInstance()
			Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = Me.m_UnderlyingNamedType.TypeArgumentsNoUseSiteDiagnostics.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As TypeSymbol = enumerator.Current
				instance.Add([module].Translate(current, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics))
			End While
			Return instance.ToImmutableAndFree()
		End Function

		Private Function IGenericTypeInstanceReferenceGetGenericType(ByVal context As EmitContext) As INamedTypeReference Implements IGenericTypeInstanceReference.GetGenericType
			Return DirectCast(context.[Module], PEModuleBuilder).Translate(Me.m_UnderlyingNamedType.OriginalDefinition, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics, False, True)
		End Function
	End Class
End Namespace