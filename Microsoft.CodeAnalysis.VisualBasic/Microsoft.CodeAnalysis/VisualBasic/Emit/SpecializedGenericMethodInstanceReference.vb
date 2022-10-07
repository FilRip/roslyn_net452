Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend NotInheritable Class SpecializedGenericMethodInstanceReference
		Inherits SpecializedMethodReference
		Implements IGenericMethodInstanceReference
		Private ReadOnly _genericMethod As SpecializedMethodReference

		Public Overrides ReadOnly Property AsGenericMethodInstanceReference As IGenericMethodInstanceReference
			Get
				Return Me
			End Get
		End Property

		Public Sub New(ByVal underlyingMethod As MethodSymbol)
			MyBase.New(underlyingMethod)
			Me._genericMethod = New SpecializedMethodReference(underlyingMethod)
		End Sub

		Public Overrides Sub Dispatch(ByVal visitor As MetadataVisitor)
			visitor.Visit(Me)
		End Sub

		Public Function GetGenericArguments(ByVal context As EmitContext) As IEnumerable(Of ITypeReference) Implements IGenericMethodInstanceReference.GetGenericArguments
			Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
			Return Me.m_UnderlyingMethod.TypeArguments.[Select](Of ITypeReference)(Function(arg As TypeSymbol) [module].Translate(arg, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics))
		End Function

		Public Function GetGenericMethod(ByVal context As EmitContext) As IMethodReference Implements IGenericMethodInstanceReference.GetGenericMethod
			Return Me._genericMethod
		End Function
	End Class
End Namespace