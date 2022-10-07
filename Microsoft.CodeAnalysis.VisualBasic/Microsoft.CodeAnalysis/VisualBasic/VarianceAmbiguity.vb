Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class VarianceAmbiguity
		Public Sub New()
			MyBase.New()
		End Sub

		Private Shared Sub CheckCorrespondingTypeArguments(ByVal containingType As NamedTypeSymbol, ByVal variance As VarianceKind, ByVal typeArgument1 As TypeSymbol, ByVal typeArgument2 As TypeSymbol, ByRef causesAmbiguity As Boolean, ByRef preventsAmbiguity As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			If (Not typeArgument1.IsSameTypeIgnoringAll(typeArgument2)) Then
				Select Case variance
					Case VarianceKind.None
						If (TypeUnification.CanUnify(containingType, typeArgument1, typeArgument2)) Then
							Return
						End If
						preventsAmbiguity = True
						Return
					Case VarianceKind.Out
						If (typeArgument1.SpecialType <> SpecialType.System_Object AndAlso typeArgument2.SpecialType <> SpecialType.System_Object AndAlso Not typeArgument1.IsValueType AndAlso Not typeArgument2.IsValueType) Then
							causesAmbiguity = True
							Return
						End If
						If (Not typeArgument1.IsValueType AndAlso Not typeArgument2.IsValueType OrElse TypeUnification.CanUnify(containingType, typeArgument1, typeArgument2)) Then
							Return
						End If
						preventsAmbiguity = True
						Return
					Case VarianceKind.[In]
						Dim flag As Boolean = If(Not typeArgument1.IsClassType(), False, typeArgument2.IsClassType())
						Dim flag1 As Boolean = If(Not flag, False, (Conversions.ClassifyDirectCastConversion(typeArgument1, typeArgument2, useSiteInfo) And ConversionKind.Reference) <> ConversionKind.DelegateRelaxationLevelNone)
						If (Not typeArgument1.IsNotInheritable() AndAlso Not typeArgument2.IsNotInheritable() AndAlso (Not flag OrElse flag1)) Then
							causesAmbiguity = True
							Return
						End If
						If (Not typeArgument1.IsValueType AndAlso Not typeArgument2.IsValueType AndAlso (Not flag OrElse flag1) OrElse TypeUnification.CanUnify(containingType, typeArgument1, typeArgument2)) Then
							Return
						End If
						preventsAmbiguity = True
						Return
				End Select
				Throw ExceptionUtilities.UnexpectedValue(variance)
			End If
		End Sub

		Public Shared Function HasVarianceAmbiguity(ByVal containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal i1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal i2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = i1
			Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = i2
			Dim flag As Boolean = False
			Dim flag1 As Boolean = False
			Do
				Dim arity As Integer = namedTypeSymbol.Arity - 1
				Dim num As Integer = 0
				Do
					Dim typeParameters As ImmutableArray(Of TypeParameterSymbol) = namedTypeSymbol.TypeParameters
					VarianceAmbiguity.CheckCorrespondingTypeArguments(containingType, typeParameters(num).Variance, namedTypeSymbol.TypeArgumentWithDefinitionUseSiteDiagnostics(num, useSiteInfo), namedTypeSymbol1.TypeArgumentWithDefinitionUseSiteDiagnostics(num, useSiteInfo), flag, flag1, useSiteInfo)
					num = num + 1
				Loop While num <= arity
				namedTypeSymbol = namedTypeSymbol.ContainingType
				namedTypeSymbol1 = namedTypeSymbol1.ContainingType
			Loop While namedTypeSymbol IsNot Nothing
			Return flag And Not flag1
		End Function
	End Class
End Namespace