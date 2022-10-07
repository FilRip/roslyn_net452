Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class TypeSubstitution
		Private ReadOnly _pairs As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers))

		Private ReadOnly _targetGenericDefinition As Symbol

		Private ReadOnly _parent As TypeSubstitution

		Private ReadOnly Shared s_withoutModifiers As Func(Of TypeSymbol, TypeWithModifiers)

		Public ReadOnly Property Pairs As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers))
			Get
				Return Me._pairs
			End Get
		End Property

		Public ReadOnly Property PairsIncludingParent As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers))
			Get
				Dim immutableAndFree As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers))
				If (Me._parent IsNot Nothing) Then
					Dim instance As ArrayBuilder(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)) = ArrayBuilder(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)).GetInstance()
					Me.AddPairsIncludingParentToBuilder(instance)
					immutableAndFree = instance.ToImmutableAndFree()
				Else
					immutableAndFree = Me.Pairs
				End If
				Return immutableAndFree
			End Get
		End Property

		Public ReadOnly Property Parent As TypeSubstitution
			Get
				Return Me._parent
			End Get
		End Property

		Public ReadOnly Property TargetGenericDefinition As Symbol
			Get
				Return Me._targetGenericDefinition
			End Get
		End Property

		Shared Sub New()
			TypeSubstitution.s_withoutModifiers = Function(arg As TypeSymbol) New TypeWithModifiers(arg)
		End Sub

		Private Sub New(ByVal targetGenericDefinition As Symbol, ByVal pairs As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)), ByVal parent As TypeSubstitution)
			MyBase.New()
			Me._pairs = pairs
			Me._parent = parent
			Me._targetGenericDefinition = targetGenericDefinition
		End Sub

		Private Sub AddPairsIncludingParentToBuilder(ByVal pairBuilder As ArrayBuilder(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)))
			If (Me._parent IsNot Nothing) Then
				Me._parent.AddPairsIncludingParentToBuilder(pairBuilder)
			End If
			pairBuilder.AddRange(Me._pairs)
		End Sub

		Public Shared Function AdjustForConstruct(ByVal adjustedParent As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution, ByVal oldConstructSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution, ByVal additionalSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Dim instance As ArrayBuilder(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)) = ArrayBuilder(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)).GetInstance()
			Dim flag As Boolean = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.PrivateAdjustForConstruct(instance, oldConstructSubstitution, additionalSubstitution)
			If (Not flag AndAlso oldConstructSubstitution.Parent = adjustedParent) Then
				typeSubstitution = oldConstructSubstitution
			ElseIf (instance.Count <> 0 OrElse adjustedParent IsNot Nothing) Then
				typeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Concat(adjustedParent, oldConstructSubstitution.TargetGenericDefinition, If(flag, instance.ToImmutable(), oldConstructSubstitution.Pairs))
			Else
				typeSubstitution = Nothing
			End If
			instance.Free()
			Return typeSubstitution
		End Function

		Public Shared Function Concat(ByVal targetGenericDefinition As Symbol, ByVal sub1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution, ByVal sub2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			If (sub1 Is Nothing) Then
				typeSubstitution = sub2
			ElseIf (sub2 IsNot Nothing) Then
				typeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.ConcatNotNulls(sub1, sub2)
			Else
				typeSubstitution = If(CObj(targetGenericDefinition) <> CObj(sub1.TargetGenericDefinition), Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Concat(sub1, targetGenericDefinition, ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)).Empty), sub1)
			End If
			Return typeSubstitution
		End Function

		Private Shared Function Concat(ByVal parent As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution, ByVal targetGenericDefinition As Symbol, ByVal pairs As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers))) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			If (parent Is Nothing OrElse parent.TargetGenericDefinition = targetGenericDefinition.ContainingType) Then
				typeSubstitution = New Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution(targetGenericDefinition, pairs, parent)
			Else
				Dim containingType As NamedTypeSymbol = targetGenericDefinition.ContainingType
				typeSubstitution = New Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution(targetGenericDefinition, pairs, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Concat(parent, containingType, ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)).Empty))
			End If
			Return typeSubstitution
		End Function

		Private Shared Function ConcatNotNulls(ByVal sub1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution, ByVal sub2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			typeSubstitution = If(sub2.Parent IsNot Nothing, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Concat(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.ConcatNotNulls(sub1, sub2.Parent), sub2.TargetGenericDefinition, sub2.Pairs), Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Concat(sub1, sub2.TargetGenericDefinition, sub2.Pairs))
			Return typeSubstitution
		End Function

		Public Shared Function Create(ByVal targetGenericDefinition As Symbol, ByVal params As TypeParameterSymbol(), ByVal args As TypeWithModifiers(), Optional ByVal allowAlphaRenamedTypeParametersAsArguments As Boolean = False) As TypeSubstitution
			Return TypeSubstitution.Create(targetGenericDefinition, ImmutableArrayExtensions.AsImmutableOrNull(Of TypeParameterSymbol)(params), ImmutableArrayExtensions.AsImmutableOrNull(Of TypeWithModifiers)(args), allowAlphaRenamedTypeParametersAsArguments)
		End Function

		Public Shared Function Create(ByVal targetGenericDefinition As Symbol, ByVal params As TypeParameterSymbol(), ByVal args As TypeSymbol(), Optional ByVal allowAlphaRenamedTypeParametersAsArguments As Boolean = False) As TypeSubstitution
			Return TypeSubstitution.Create(targetGenericDefinition, ImmutableArrayExtensions.AsImmutableOrNull(Of TypeParameterSymbol)(params), ImmutableArrayExtensions.AsImmutableOrNull(Of TypeSymbol)(args), allowAlphaRenamedTypeParametersAsArguments)
		End Function

		Public Shared Function Create(ByVal targetGenericDefinition As Symbol, ByVal params As ImmutableArray(Of TypeParameterSymbol), ByVal args As ImmutableArray(Of TypeWithModifiers), Optional ByVal allowAlphaRenamedTypeParametersAsArguments As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			If (params.Length <> args.Length) Then
				Throw New ArgumentException(VBResources.NumberOfTypeParametersAndArgumentsMustMatch)
			End If
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Nothing
			Dim containingSymbol As Symbol = Nothing
			Dim instance As ArrayBuilder(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)) = ArrayBuilder(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)).GetInstance()
			Try
				Dim length As Integer = params.Length - 1
				Dim num As Integer = 0
				Do
					Dim item As TypeParameterSymbol = params(num)
					Dim typeWithModifier As TypeWithModifiers = args(num)
					If (CObj(containingSymbol) <> CObj(item.ContainingSymbol)) Then
						If (instance.Count > 0) Then
							typeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Concat(typeSubstitution, containingSymbol, instance.ToImmutable())
							instance.Clear()
						End If
						containingSymbol = item.ContainingSymbol
					End If
					If (Not typeWithModifier.[Is](item)) Then
						If (Not allowAlphaRenamedTypeParametersAsArguments AndAlso typeWithModifier.Type.IsTypeParameter() AndAlso Not typeWithModifier.Type.IsDefinition) Then
							Throw New ArgumentException()
						End If
						instance.Add(New KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)(item, typeWithModifier))
					End If
					num = num + 1
				Loop While num <= length
				If (instance.Count > 0) Then
					typeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Concat(typeSubstitution, containingSymbol, instance.ToImmutable())
				End If
			Finally
				instance.Free()
			End Try
			If (typeSubstitution IsNot Nothing AndAlso CObj(typeSubstitution.TargetGenericDefinition) <> CObj(targetGenericDefinition)) Then
				typeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Concat(typeSubstitution, targetGenericDefinition, ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)).Empty)
			End If
			Return typeSubstitution
		End Function

		Public Shared Function Create(ByVal targetGenericDefinition As Symbol, ByVal params As ImmutableArray(Of TypeParameterSymbol), ByVal args As ImmutableArray(Of TypeSymbol), Optional ByVal allowAlphaRenamedTypeParametersAsArguments As Boolean = False) As TypeSubstitution
			Return TypeSubstitution.Create(targetGenericDefinition, params, ImmutableArrayExtensions.SelectAsArray(Of TypeSymbol, TypeWithModifiers)(args, TypeSubstitution.s_withoutModifiers), allowAlphaRenamedTypeParametersAsArguments)
		End Function

		Public Shared Function Create(ByVal parent As TypeSubstitution, ByVal targetGenericDefinition As Symbol, ByVal args As ImmutableArray(Of TypeSymbol), Optional ByVal allowAlphaRenamedTypeParametersAsArguments As Boolean = False) As TypeSubstitution
			Return TypeSubstitution.Create(parent, targetGenericDefinition, ImmutableArrayExtensions.SelectAsArray(Of TypeSymbol, TypeWithModifiers)(args, TypeSubstitution.s_withoutModifiers), allowAlphaRenamedTypeParametersAsArguments)
		End Function

		Public Shared Function Create(ByVal parent As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution, ByVal targetGenericDefinition As Symbol, ByVal args As ImmutableArray(Of TypeWithModifiers), Optional ByVal allowAlphaRenamedTypeParametersAsArguments As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Dim typeParameterSymbols As ImmutableArray(Of TypeParameterSymbol)
			typeParameterSymbols = If(targetGenericDefinition.Kind <> SymbolKind.Method, DirectCast(targetGenericDefinition, NamedTypeSymbol).TypeParameters, DirectCast(targetGenericDefinition, MethodSymbol).TypeParameters)
			Dim length As Integer = typeParameterSymbols.Length
			If (args.Length <> length) Then
				Throw New ArgumentException(VBResources.NumberOfTypeParametersAndArgumentsMustMatch)
			End If
			Dim num As Integer = 0
			Dim num1 As Integer = length - 1
			Dim num2 As Integer = 0
			Do
				Dim item As TypeWithModifiers = args(num2)
				If (Not item.[Is](typeParameterSymbols(num2))) Then
					num = num + 1
				End If
				If (Not allowAlphaRenamedTypeParametersAsArguments AndAlso item.Type.IsTypeParameter() AndAlso Not item.Type.IsDefinition) Then
					Throw New ArgumentException()
				End If
				num2 = num2 + 1
			Loop While num2 <= num1
			If (num <> 0) Then
				Dim num3 As Integer = 0
				Dim keyValuePair(num - 1 + 1 - 1) As KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)
				Dim num4 As Integer = length - 1
				Dim num5 As Integer = 0
				Do
					If (Not args(num5).[Is](typeParameterSymbols(num5))) Then
						keyValuePair(num3) = New KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)(typeParameterSymbols(num5), args(num5))
						num3 = num3 + 1
					End If
					num5 = num5 + 1
				Loop While num5 <= num4
				typeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Concat(parent, targetGenericDefinition, ImmutableArrayExtensions.AsImmutableOrNull(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers))(keyValuePair))
			Else
				typeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Concat(targetGenericDefinition, parent, Nothing)
			End If
			Return typeSubstitution
		End Function

		Public Shared Function CreateAdditionalMethodTypeParameterSubstitution(ByVal targetMethod As MethodSymbol, ByVal typeArguments As ImmutableArray(Of TypeWithModifiers)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Dim item As TypeWithModifiers
			Dim typeParameters As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol) = targetMethod.TypeParameters
			Dim num As Integer = 0
			Dim length As Integer = typeArguments.Length - 1
			Dim num1 As Integer = 0
			Do
				item = typeArguments(num1)
				If (item.Type.IsTypeParameter()) Then
					Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol = DirectCast(item.Type, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol)
					If (type.Ordinal = num1 AndAlso type.ContainingSymbol = targetMethod AndAlso item.CustomModifiers.IsDefaultOrEmpty) Then
						GoTo Label0
					End If
				End If
				num = num + 1
			Label0:
				num1 = num1 + 1
			Loop While num1 <= length
			If (num <> 0) Then
				Dim keyValuePair(num - 1 + 1 - 1) As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol, TypeWithModifiers)
				num = 0
				Dim length1 As Integer = typeArguments.Length - 1
				Dim num2 As Integer = 0
				Do
					item = typeArguments(num2)
					If (item.Type.IsTypeParameter()) Then
						Dim typeParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol = DirectCast(item.Type, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol)
						If (typeParameterSymbol.Ordinal = num2 AndAlso typeParameterSymbol.ContainingSymbol = targetMethod AndAlso item.CustomModifiers.IsDefaultOrEmpty) Then
							GoTo Label1
						End If
					End If
					keyValuePair(num) = New KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol, TypeWithModifiers)(typeParameters(num2), item)
					num = num + 1
				Label1:
					num2 = num2 + 1
				Loop While num2 <= length1
				typeSubstitution = New Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution(targetMethod, ImmutableArrayExtensions.AsImmutableOrNull(Of KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol, TypeWithModifiers))(keyValuePair), Nothing)
			Else
				typeSubstitution = Nothing
			End If
			Return typeSubstitution
		End Function

		Public Shared Function CreateForAlphaRename(ByVal parent As TypeSubstitution, ByVal alphaRenamedTypeParameters As ImmutableArray(Of SubstitutedTypeParameterSymbol)) As TypeSubstitution
			Dim typeParameterSymbols As ImmutableArray(Of TypeParameterSymbol)
			Dim containingSymbol As Symbol = alphaRenamedTypeParameters(0).OriginalDefinition.ContainingSymbol
			typeParameterSymbols = If(containingSymbol.Kind <> SymbolKind.Method, DirectCast(containingSymbol, NamedTypeSymbol).TypeParameters, DirectCast(containingSymbol, MethodSymbol).TypeParameters)
			Dim keyValuePair(typeParameterSymbols.Length - 1 + 1 - 1) As KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)
			Dim length As Integer = typeParameterSymbols.Length - 1
			Dim num As Integer = 0
			Do
				keyValuePair(num) = New KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)(typeParameterSymbols(num), New TypeWithModifiers(alphaRenamedTypeParameters(num)))
				num = num + 1
			Loop While num <= length
			Return TypeSubstitution.Concat(parent, containingSymbol, ImmutableArrayExtensions.AsImmutableOrNull(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers))(keyValuePair))
		End Function

		Public Function GetSubstitutionFor(ByVal tp As TypeParameterSymbol) As TypeWithModifiers
			Dim typeWithModifier As TypeWithModifiers
			Dim containingSymbol As Symbol = tp.ContainingSymbol
			Dim parent As TypeSubstitution = Me
			While True
				If (CObj(parent.TargetGenericDefinition) <> CObj(containingSymbol)) Then
					parent = parent.Parent
					If (parent Is Nothing) Then
						typeWithModifier = New TypeWithModifiers(tp, ImmutableArray(Of CustomModifier).Empty)
						Exit While
					End If
				Else
					Dim enumerator As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)).Enumerator = parent.Pairs.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers) = enumerator.Current
						If (Not current.Key.Equals(tp)) Then
							Continue While
						End If
						typeWithModifier = current.Value
						Return typeWithModifier
					End While
					typeWithModifier = New TypeWithModifiers(tp, ImmutableArray(Of CustomModifier).Empty)
					Exit While
				End If
			End While
			Return typeWithModifier
		End Function

		Public Function GetSubstitutionForGenericDefinition(ByVal targetGenericDefinition As Symbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Dim parent As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Me
			While True
				If (CObj(parent.TargetGenericDefinition) <> CObj(targetGenericDefinition)) Then
					parent = parent.Parent
					If (parent Is Nothing) Then
						typeSubstitution = Nothing
						Exit While
					End If
				Else
					typeSubstitution = parent
					Exit While
				End If
			End While
			Return typeSubstitution
		End Function

		Public Function GetSubstitutionForGenericDefinitionOrContainers(ByVal targetGenericDefinition As Symbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Dim parent As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Me
			While True
				If (Not parent.IsValidToApplyTo(targetGenericDefinition)) Then
					parent = parent.Parent
					If (parent Is Nothing) Then
						typeSubstitution = Nothing
						Exit While
					End If
				Else
					typeSubstitution = parent
					Exit While
				End If
			End While
			Return typeSubstitution
		End Function

		Public Function GetTypeArgumentsCustomModifiersFor(ByVal originalDefinition As TypeParameterSymbol) As ImmutableArray(Of CustomModifier)
			Dim customModifiers As ImmutableArray(Of CustomModifier)
			Dim parent As TypeSubstitution = Me
			Do
				If (CObj(parent.TargetGenericDefinition) <> CObj(originalDefinition.ContainingSymbol)) Then
					parent = parent.Parent
				Else
					Dim enumerator As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)).Enumerator = parent.Pairs.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers) = enumerator.Current
						If (current.Key.Ordinal <> originalDefinition.Ordinal) Then
							Continue While
						End If
						customModifiers = current.Value.CustomModifiers
						Return customModifiers
					End While
					Exit Do
				End If
			Loop While parent IsNot Nothing
			customModifiers = ImmutableArray(Of CustomModifier).Empty
			Return customModifiers
		End Function

		Public Function GetTypeArgumentsFor(ByVal originalDefinition As NamedTypeSymbol, <Out> ByRef hasTypeArgumentsCustomModifiers As Boolean) As ImmutableArray(Of TypeSymbol)
			Dim parent As TypeSubstitution = Me
			Dim instance As ArrayBuilder(Of TypeSymbol) = ArrayBuilder(Of TypeSymbol).GetInstance(originalDefinition.Arity, Nothing)
			hasTypeArgumentsCustomModifiers = False
			Do
				If (parent.TargetGenericDefinition <> originalDefinition) Then
					parent = parent.Parent
				Else
					Dim enumerator As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)).Enumerator = parent.Pairs.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers) = enumerator.Current
						instance(current.Key.Ordinal) = current.Value.Type
						If (current.Value.CustomModifiers.IsDefaultOrEmpty) Then
							Continue While
						End If
						hasTypeArgumentsCustomModifiers = True
					End While
					Exit Do
				End If
			Loop While parent IsNot Nothing
			Dim count As Integer = instance.Count - 1
			Dim num As Integer = 0
			Do
				If (instance(num) Is Nothing) Then
					instance(num) = originalDefinition.TypeParameters(num)
				End If
				num = num + 1
			Loop While num <= count
			Return instance.ToImmutableAndFree()
		End Function

		Public Function HasTypeArgumentsCustomModifiersFor(ByVal originalDefinition As NamedTypeSymbol) As Boolean
			Dim flag As Boolean
			Dim parent As TypeSubstitution = Me
			Do
				If (parent.TargetGenericDefinition <> originalDefinition) Then
					parent = parent.Parent
				Else
					Dim enumerator As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)).Enumerator = parent.Pairs.GetEnumerator()
					While enumerator.MoveNext()
						If (enumerator.Current.Value.CustomModifiers.IsDefaultOrEmpty) Then
							Continue While
						End If
						flag = True
						Return flag
					End While
					Exit Do
				End If
			Loop While parent IsNot Nothing
			flag = False
			Return flag
		End Function

		Public Function IsValidToApplyTo(ByVal genericDefinition As Symbol) As Boolean
			Dim flag As Boolean
			Dim containingType As Symbol = genericDefinition
			While True
				If (CObj(containingType) <> CObj(Me.TargetGenericDefinition)) Then
					containingType = containingType.ContainingType
					If (containingType Is Nothing) Then
						flag = False
						Exit While
					End If
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Shared Function PrivateAdjustForConstruct(ByVal pairs As ArrayBuilder(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)), ByVal oldConstructSubstitution As TypeSubstitution, ByVal additionalSubstitution As TypeSubstitution) As Boolean
			Dim num As Integer
			Dim flag As Boolean = False
			Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)) = oldConstructSubstitution.Pairs
			Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
			Dim targetGenericDefinition As Symbol = oldConstructSubstitution.TargetGenericDefinition
			If (keyValuePairs.Length > 0) Then
				num = If(targetGenericDefinition.Kind <> SymbolKind.Method, DirectCast(targetGenericDefinition, NamedTypeSymbol).Arity, DirectCast(targetGenericDefinition, MethodSymbol).Arity)
				bitVector = Microsoft.CodeAnalysis.BitVector.Create(num)
			End If
			Dim length As Integer = keyValuePairs.Length - 1
			Dim num1 As Integer = 0
			Do
				Dim item As KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers) = keyValuePairs(num1)
				Dim typeWithModifier As TypeWithModifiers = item.Value.InternalSubstituteTypeParameters(additionalSubstitution)
				item = keyValuePairs(num1)
				bitVector(item.Key.Ordinal) = True
				If (Not typeWithModifier.ExplicitEquals(keyValuePairs(num1).Value)) Then
					flag = True
				End If
				If (Not typeWithModifier.[Is](keyValuePairs(num1).Key)) Then
					item = keyValuePairs(num1)
					pairs.Add(New KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)(item.Key, typeWithModifier))
				End If
				num1 = num1 + 1
			Loop While num1 <= length
			Dim substitutionForGenericDefinition As TypeSubstitution = additionalSubstitution.GetSubstitutionForGenericDefinition(targetGenericDefinition)
			If (substitutionForGenericDefinition IsNot Nothing) Then
				Dim enumerator As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)).Enumerator = substitutionForGenericDefinition.Pairs.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers) = enumerator.Current
					If (Not bitVector.IsNull AndAlso bitVector(current.Key.Ordinal)) Then
						Continue While
					End If
					flag = True
					pairs.Add(current)
				End While
			End If
			Return flag
		End Function

		Public Function SubstituteCustomModifiers(ByVal type As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier)) As ImmutableArray(Of CustomModifier)
			Dim customModifiers1 As ImmutableArray(Of CustomModifier)
			If (Not type.IsTypeParameter()) Then
				customModifiers1 = Me.SubstituteCustomModifiers(customModifiers)
			Else
				Dim typeWithModifier As TypeWithModifiers = New TypeWithModifiers(type, customModifiers)
				customModifiers1 = typeWithModifier.InternalSubstituteTypeParameters(Me).CustomModifiers
			End If
			Return customModifiers1
		End Function

		Public Function SubstituteCustomModifiers(ByVal customModifiers As ImmutableArray(Of CustomModifier)) As ImmutableArray(Of CustomModifier)
			Dim immutableAndFree As ImmutableArray(Of CustomModifier)
			If (Not customModifiers.IsDefaultOrEmpty) Then
				Dim length As Integer = customModifiers.Length - 1
				Dim num As Integer = 0
				While num <= length
					Dim modifier As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(customModifiers(num).Modifier, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					Dim typeWithModifier As TypeWithModifiers = modifier.InternalSubstituteTypeParameters(Me)
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(typeWithModifier.AsTypeSymbolOnly(), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					If (TypeSymbol.Equals(modifier, namedTypeSymbol, TypeCompareKind.ConsiderEverything)) Then
						num = num + 1
					Else
						Dim instance As ArrayBuilder(Of CustomModifier) = ArrayBuilder(Of CustomModifier).GetInstance(customModifiers.Length)
						instance.AddRange(customModifiers, num)
						instance.Add(If(customModifiers(num).IsOptional, VisualBasicCustomModifier.CreateOptional(namedTypeSymbol), VisualBasicCustomModifier.CreateRequired(namedTypeSymbol)))
						Dim length1 As Integer = customModifiers.Length - 1
						Dim num1 As Integer = num + 1
						Do
							Dim modifier1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(customModifiers(num1).Modifier, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
							typeWithModifier = modifier1.InternalSubstituteTypeParameters(Me)
							namedTypeSymbol = DirectCast(typeWithModifier.AsTypeSymbolOnly(), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
							If (TypeSymbol.Equals(modifier1, namedTypeSymbol, TypeCompareKind.ConsiderEverything)) Then
								instance.Add(customModifiers(num1))
							Else
								instance.Add(If(customModifiers(num1).IsOptional, VisualBasicCustomModifier.CreateOptional(namedTypeSymbol), VisualBasicCustomModifier.CreateRequired(namedTypeSymbol)))
							End If
							num1 = num1 + 1
						Loop While num1 <= length1
						immutableAndFree = instance.ToImmutableAndFree()
						Return immutableAndFree
					End If
				End While
				immutableAndFree = customModifiers
			Else
				immutableAndFree = customModifiers
			End If
			Return immutableAndFree
		End Function

		Public Sub ThrowIfSubstitutingToAlphaRenamedTypeParameter()
			Dim parent As TypeSubstitution = Me
			Do
				Dim enumerator As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)).Enumerator = parent.Pairs.GetEnumerator()
				While enumerator.MoveNext()
					Dim type As TypeSymbol = enumerator.Current.Value.Type
					If (Not type.IsTypeParameter() OrElse type.IsDefinition) Then
						Continue While
					End If
					Throw New ArgumentException()
				End While
				parent = parent.Parent
			Loop While parent IsNot Nothing
		End Sub

		Public Overrides Function ToString() As String
			Dim stringBuilder As System.Text.StringBuilder = New System.Text.StringBuilder()
			stringBuilder.AppendFormat("{0} : ", Me.TargetGenericDefinition)
			Me.ToString(stringBuilder)
			Return stringBuilder.ToString()
		End Function

		Private Sub ToString(ByVal builder As StringBuilder)
			If (Me._parent IsNot Nothing) Then
				Me._parent.ToString(builder)
				builder.Append(", ")
			End If
			builder.Append("{"C)
			Dim length As Integer = Me._pairs.Length - 1
			Dim num As Integer = 0
			Do
				If (num <> 0) Then
					builder.Append(", ")
				End If
				Dim item As KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers) = Me._pairs(num)
				Dim str As String = item.Key.ToString()
				item = Me._pairs(num)
				builder.AppendFormat("{0}->{1}", CObj(str), item.Value.Type.ToString())
				num = num + 1
			Loop While num <= length
			builder.Append("}"C)
		End Sub

		Public Function WasConstructedForModifiers() As Boolean
			Dim flag As Boolean
			Dim enumerator As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)).Enumerator = Me._pairs.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers) = enumerator.Current
					If (Not current.Key.Equals(current.Value.Type.OriginalDefinition)) Then
						flag = False
						Exit While
					End If
				Else
					flag = If(Me._parent Is Nothing, True, Me._parent.WasConstructedForModifiers())
					Exit While
				End If
			End While
			Return flag
		End Function
	End Class
End Namespace