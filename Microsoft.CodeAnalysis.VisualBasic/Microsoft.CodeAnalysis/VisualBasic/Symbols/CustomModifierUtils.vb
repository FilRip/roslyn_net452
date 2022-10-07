Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class CustomModifierUtils
		Public Sub New()
			MyBase.New()
		End Sub

		Friend Shared Sub CopyMethodCustomModifiers(ByVal sourceMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal destinationTypeParameters As ImmutableArray(Of TypeSymbol), <InAttribute> <Out> ByRef destinationReturnType As TypeSymbol, <InAttribute> <Out> ByRef parameters As ImmutableArray(Of ParameterSymbol))
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = sourceMethod.ConstructIfGeneric(destinationTypeParameters)
			parameters = CustomModifierUtils.CopyParameterCustomModifiers(methodSymbol.Parameters, parameters)
			Dim returnType As TypeSymbol = methodSymbol.ReturnType
			If (destinationReturnType.IsSameType(returnType, TypeCompareKind.AllIgnoreOptionsForVB)) Then
				destinationReturnType = CustomModifierUtils.CopyTypeCustomModifiers(returnType, destinationReturnType)
			End If
		End Sub

		Public Shared Function CopyParameterCustomModifiers(ByVal overriddenMemberParameters As ImmutableArray(Of ParameterSymbol), ByVal parameters As ImmutableArray(Of ParameterSymbol)) As ImmutableArray(Of ParameterSymbol)
			Dim instance As ArrayBuilder(Of ParameterSymbol) = Nothing
			Dim length As Integer = parameters.Length - 1
			Dim num As Integer = 0
			Do
				Dim item As ParameterSymbol = parameters(num)
				If (CustomModifierUtils.CopyParameterCustomModifiers(overriddenMemberParameters(num), item)) Then
					If (instance Is Nothing) Then
						instance = ArrayBuilder(Of ParameterSymbol).GetInstance()
						instance.AddRange(parameters, num)
					End If
					instance.Add(item)
				ElseIf (instance IsNot Nothing) Then
					instance.Add(item)
				End If
				num = num + 1
			Loop While num <= length
			If (instance Is Nothing) Then
				Return parameters
			End If
			Return instance.ToImmutableAndFree()
		End Function

		Public Shared Function CopyParameterCustomModifiers(ByVal overriddenParam As ParameterSymbol, <InAttribute> <Out> ByRef thisParam As ParameterSymbol) As Boolean
			Dim flag As Boolean
			Dim refCustomModifiers As ImmutableArray(Of CustomModifier)
			If (Not System.Linq.ImmutableArrayExtensions.SequenceEqual(Of CustomModifier, CustomModifier)(overriddenParam.CustomModifiers, thisParam.CustomModifiers, DirectCast(Nothing, IEqualityComparer(Of CustomModifier))) OrElse overriddenParam.IsByRef AndAlso thisParam.IsByRef AndAlso Not System.Linq.ImmutableArrayExtensions.SequenceEqual(Of CustomModifier, CustomModifier)(overriddenParam.RefCustomModifiers, thisParam.RefCustomModifiers, DirectCast(Nothing, IEqualityComparer(Of CustomModifier))) OrElse Not thisParam.Type.IsSameType(overriddenParam.Type, TypeCompareKind.IgnoreTupleNames)) Then
				Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = thisParam.Type
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = overriddenParam.Type
				If (type.ContainsTuple() AndAlso Not overriddenParam.Type.IsSameType(type, TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds)) Then
					typeSymbol = TupleTypeDecoder.DecodeTupleTypesIfApplicable(typeSymbol, VisualBasicCompilation.TupleNamesEncoder.Encode(type))
				End If
				Dim sourceParameterSymbolBase As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceParameterSymbolBase = DirectCast(thisParam, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceParameterSymbolBase)
				Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = typeSymbol
				Dim customModifiers As ImmutableArray(Of CustomModifier) = overriddenParam.CustomModifiers
				If (thisParam.IsByRef) Then
					refCustomModifiers = overriddenParam.RefCustomModifiers
				Else
					refCustomModifiers = ImmutableArray(Of CustomModifier).Empty
				End If
				thisParam = sourceParameterSymbolBase.WithTypeAndCustomModifiers(typeSymbol1, customModifiers, refCustomModifiers)
				flag = True
			Else
				flag = False
			End If
			Return flag
		End Function

		Friend Shared Function CopyTypeCustomModifiers(ByVal sourceType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal destinationType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			typeSymbol = If(Not destinationType.ContainsTuple() OrElse sourceType.IsSameType(destinationType, TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds), sourceType, TupleTypeDecoder.DecodeTupleTypesIfApplicable(sourceType, VisualBasicCompilation.TupleNamesEncoder.Encode(destinationType)))
			Return typeSymbol
		End Function

		Friend Shared Function HasIsExternalInitModifier(ByVal modifiers As ImmutableArray(Of CustomModifier)) As Boolean
			Dim isOptional As Func(Of CustomModifier, Boolean)
			Dim customModifiers As ImmutableArray(Of CustomModifier) = modifiers
			If (CustomModifierUtils._Closure$__.$I5-0 Is Nothing) Then
				isOptional = Function(modifier As CustomModifier)
					If (modifier.IsOptional) Then
						Return False
					End If
					Return DirectCast(modifier, VisualBasicCustomModifier).ModifierSymbol.IsWellKnownTypeIsExternalInit()
				End Function
				CustomModifierUtils._Closure$__.$I5-0 = isOptional
			Else
				isOptional = CustomModifierUtils._Closure$__.$I5-0
			End If
			Return customModifiers.Any(isOptional)
		End Function
	End Class
End Namespace