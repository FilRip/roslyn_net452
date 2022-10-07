Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit
	Friend NotInheritable Class ParameterTypeInformation
		Implements IParameterTypeInformation
		Private ReadOnly _underlyingParameter As ParameterSymbol

		ReadOnly Property IParameterListEntryIndex As UShort
			Get
				Return CUShort(Me._underlyingParameter.Ordinal)
			End Get
		End Property

		ReadOnly Property IParameterTypeInformationCustomModifiers As ImmutableArray(Of ICustomModifier) Implements IParameterTypeInformation.CustomModifiers
			Get
				Return Me._underlyingParameter.CustomModifiers.[As](Of ICustomModifier)()
			End Get
		End Property

		ReadOnly Property IParameterTypeInformationIsByReference As Boolean Implements IParameterTypeInformation.IsByReference
			Get
				Return Me._underlyingParameter.IsByRef
			End Get
		End Property

		ReadOnly Property IParameterTypeInformationRefCustomModifiers As ImmutableArray(Of ICustomModifier) Implements IParameterTypeInformation.RefCustomModifiers
			Get
				Return Me._underlyingParameter.RefCustomModifiers.[As](Of ICustomModifier)()
			End Get
		End Property

		Public Sub New(ByVal underlyingParameter As ParameterSymbol)
			MyBase.New()
			Me._underlyingParameter = underlyingParameter
		End Sub

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overrides Function GetHashCode() As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Private Function IParameterTypeInformationGetType(ByVal context As EmitContext) As ITypeReference Implements IParameterTypeInformation.[GetType]
			Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
			Dim type As TypeSymbol = Me._underlyingParameter.Type
			Return [module].Translate(type, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics)
		End Function
	End Class
End Namespace