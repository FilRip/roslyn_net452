Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend NotInheritable Class GenericMethodInstanceReference
		Inherits MethodReference
		Implements IGenericMethodInstanceReference
		Public Overrides ReadOnly Property AsGenericMethodInstanceReference As IGenericMethodInstanceReference
			Get
				Return Me
			End Get
		End Property

		Public Sub New(ByVal underlyingMethod As MethodSymbol)
			MyBase.New(underlyingMethod)
		End Sub

		Public Overrides Sub Dispatch(ByVal visitor As MetadataVisitor)
			visitor.Visit(Me)
		End Sub

		Private Function IGenericMethodInstanceReferenceGetGenericArguments(ByVal context As EmitContext) As IEnumerable(Of ITypeReference) Implements IGenericMethodInstanceReference.GetGenericArguments
			Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
			Return Me.m_UnderlyingMethod.TypeArguments.[Select](Of ITypeReference)(Function(arg As TypeSymbol) [module].Translate(arg, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics))
		End Function

		Private Function IGenericMethodInstanceReferenceGetGenericMethod(ByVal context As EmitContext) As IMethodReference Implements IGenericMethodInstanceReference.GetGenericMethod
			Return DirectCast(context.[Module], PEModuleBuilder).Translate(Me.m_UnderlyingMethod.OriginalDefinition, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics, True)
		End Function
	End Class
End Namespace