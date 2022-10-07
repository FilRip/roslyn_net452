Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend MustInherit Class TypeMemberReference
		Implements ITypeMemberReference
		ReadOnly Property INamedEntityName As String
			Get
				Return Me.UnderlyingSymbol.MetadataName
			End Get
		End Property

		Protected MustOverride ReadOnly Property UnderlyingSymbol As Symbol

		Protected Sub New()
			MyBase.New()
		End Sub

		Public MustOverride Sub Dispatch(ByVal visitor As MetadataVisitor)

		Public NotOverridable Overrides Function Equals(ByVal obj As Object) As Boolean
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overridable Function GetContainingType(ByVal context As EmitContext) As ITypeReference Implements ITypeMemberReference.GetContainingType
			Return DirectCast(context.[Module], PEModuleBuilder).Translate(Me.UnderlyingSymbol.ContainingType, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics, False, False)
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
			Return Me.UnderlyingSymbol
		End Function

		Public Overrides Function ToString() As String
			Return Me.UnderlyingSymbol.ToDisplayString(SymbolDisplayFormat.ILVisualizationFormat)
		End Function
	End Class
End Namespace