Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Module TypeSymbolExtensions
		Private ReadOnly s_addIfTypeParameterFunc As Func(Of TypeSymbol, HashSet(Of TypeParameterSymbol), Boolean)

		Private ReadOnly s_isTypeParameterNotInSetFunc As Func(Of TypeSymbol, HashSet(Of TypeParameterSymbol), Boolean)

		Private ReadOnly s_isMethodTypeParameterFunc As Func(Of TypeSymbol, MethodSymbol, Boolean)

		Private ReadOnly s_isTypeParameterFunc As Func(Of TypeSymbol, Object, Boolean)

		Private ReadOnly s_isTupleTypeFunc As Func(Of TypeSymbol, Object, Boolean)

		Private ReadOnly s_hasTupleNamesFunc As Func(Of TypeSymbol, Object, Boolean)

		Sub New()
			TypeSymbolExtensions.s_addIfTypeParameterFunc = New Func(Of TypeSymbol, HashSet(Of TypeParameterSymbol), Boolean)(AddressOf TypeSymbolExtensions.AddIfTypeParameter)
			TypeSymbolExtensions.s_isTypeParameterNotInSetFunc = New Func(Of TypeSymbol, HashSet(Of TypeParameterSymbol), Boolean)(AddressOf TypeSymbolExtensions.IsTypeParameterNotInSet)
			TypeSymbolExtensions.s_isMethodTypeParameterFunc = New Func(Of TypeSymbol, MethodSymbol, Boolean)(AddressOf TypeSymbolExtensions.IsMethodTypeParameter)
			TypeSymbolExtensions.s_isTypeParameterFunc = Function(type As TypeSymbol, arg As Object) type.TypeKind = Microsoft.CodeAnalysis.TypeKind.TypeParameter
			TypeSymbolExtensions.s_isTupleTypeFunc = Function(type As TypeSymbol, arg As Object) type.IsTupleType
			TypeSymbolExtensions.s_hasTupleNamesFunc = Function(type As TypeSymbol, arg As Object) Not type.TupleElementNames.IsDefault
		End Sub

		<Extension>
		Public Sub AddConstraintsUseSiteInfo(ByVal type As TypeParameterSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			useSiteInfo.Add(type.GetConstraintsUseSiteInfo())
		End Sub

		Private Function AddIfTypeParameter(ByVal type As TypeSymbol, ByVal typeParameters As HashSet(Of TypeParameterSymbol)) As Boolean
			If (type.TypeKind = Microsoft.CodeAnalysis.TypeKind.TypeParameter) Then
				typeParameters.Add(DirectCast(type, TypeParameterSymbol))
			End If
			Return False
		End Function

		<Extension>
		Public Sub AddUseSiteDiagnosticsForBaseDefinitions(ByVal source As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = source
			Do
				typeSymbol = typeSymbol.BaseTypeWithDefinitionUseSiteDiagnostics(useSiteInfo)
			Loop While typeSymbol IsNot Nothing
		End Sub

		<Extension>
		Public Sub AddUseSiteInfo(ByVal type As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol))
			If (useSiteInfo.AccumulatesDiagnostics) Then
				useSiteInfo.Add(type.GetUseSiteInfo())
			End If
		End Sub

		<Extension>
		Public Function AllowsCompileTimeConversions(ByVal this As TypeSymbol) As Boolean
			Return CompileTimeCalculations.TypeAllowsCompileTimeConversions(this.GetConstantValueTypeDiscriminator())
		End Function

		<Extension>
		Public Function AllowsCompileTimeOperations(ByVal this As TypeSymbol) As Boolean
			Return CompileTimeCalculations.TypeAllowsCompileTimeOperations(this.GetConstantValueTypeDiscriminator())
		End Function

		<Extension>
		Friend Function AreSameCustomModifiers(ByVal [mod] As ImmutableArray(Of CustomModifier), ByVal otherMod As ImmutableArray(Of CustomModifier)) As Boolean
			Dim flag As Boolean
			flag = If([mod].Length = otherMod.Length, System.Linq.ImmutableArrayExtensions.SequenceEqual(Of CustomModifier, CustomModifier)([mod], otherMod, DirectCast(Nothing, IEqualityComparer(Of CustomModifier))), False)
			Return flag
		End Function

		<Extension>
		Public Function CanContainUserDefinedOperators(ByVal this As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			If (this.Kind = SymbolKind.TypeParameter) Then
				Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = DirectCast(this, TypeParameterSymbol).ConstraintTypesWithDefinitionUseSiteDiagnostics(useSiteInfo).GetEnumerator()
				While enumerator.MoveNext()
					If (Not enumerator.Current.CanContainUserDefinedOperators(useSiteInfo)) Then
						Continue While
					End If
					flag = True
					Return flag
				End While
			ElseIf (this.Kind = SymbolKind.NamedType AndAlso Not DirectCast(this, NamedTypeSymbol).IsInterface) Then
				Dim enumUnderlyingTypeOrSelf As TypeSymbol = this.GetNullableUnderlyingTypeOrSelf().GetEnumUnderlyingTypeOrSelf()
				If (enumUnderlyingTypeOrSelf.IsIntrinsicType() OrElse enumUnderlyingTypeOrSelf.IsObjectType()) Then
					flag = False
					Return flag
				End If
				flag = True
				Return flag
			End If
			flag = False
			Return flag
		End Function

		<Extension>
		Friend Sub CheckTypeArguments(ByVal typeArguments As ImmutableArray(Of TypeSymbol), ByVal expectedCount As Integer)
			If (typeArguments.IsDefault) Then
				Throw New ArgumentNullException("typeArguments")
			End If
			Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = typeArguments.GetEnumerator()
			While enumerator.MoveNext()
				If (enumerator.Current IsNot Nothing) Then
					Continue While
				End If
				Throw New ArgumentException(VBResources.TypeArgumentCannotBeNothing, "typeArguments")
			End While
			If (typeArguments.Length = 0 OrElse typeArguments.Length <> expectedCount) Then
				Throw New ArgumentException(VBResources.WrongNumberOfTypeArguments, "typeArguments")
			End If
		End Sub

		<Extension>
		Friend Sub CollectReferencedTypeParameters(ByVal this As TypeSymbol, ByVal typeParameters As HashSet(Of TypeParameterSymbol))
			this.VisitType(Of HashSet(Of TypeParameterSymbol))(TypeSymbolExtensions.s_addIfTypeParameterFunc, typeParameters)
		End Sub

		<Extension>
		Friend Function ContainsTuple(ByVal type As TypeSymbol) As Boolean
			Return CObj(type.VisitType(Of Object)(TypeSymbolExtensions.s_isTupleTypeFunc, Nothing)) <> CObj(Nothing)
		End Function

		<Extension>
		Friend Function ContainsTupleNames(ByVal type As TypeSymbol) As Boolean
			Return CObj(type.VisitType(Of Object)(TypeSymbolExtensions.s_hasTupleNamesFunc, Nothing)) <> CObj(Nothing)
		End Function

		<Extension>
		Public Function DelegateOrExpressionDelegate(ByVal type As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			namedTypeSymbol = If(type.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Delegate], type.ExpressionTargetDelegate(binder.Compilation), DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol))
			Return namedTypeSymbol
		End Function

		<Extension>
		Public Function DelegateOrExpressionDelegate(ByVal type As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByRef wasExpression As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			If (type.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Delegate]) Then
				Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = type.ExpressionTargetDelegate(binder.Compilation)
				wasExpression = CObj(namedTypeSymbol1) <> CObj(Nothing)
				namedTypeSymbol = namedTypeSymbol1
			Else
				wasExpression = False
				namedTypeSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			End If
			Return namedTypeSymbol
		End Function

		<Extension>
		Public Function DigThroughArrayType(ByVal possiblyArrayType As TypeSymbol) As TypeSymbol
			While possiblyArrayType.Kind = SymbolKind.ArrayType
				possiblyArrayType = DirectCast(possiblyArrayType, ArrayTypeSymbol).ElementType
			End While
			Return possiblyArrayType
		End Function

		<Extension>
		Public Function ExpressionTargetDelegate(ByVal type As TypeSymbol, ByVal compilation As VisualBasicCompilation) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			If (type.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Class]) Then
				Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				If (namedTypeSymbol1.Arity = 1 AndAlso TypeSymbol.Equals(namedTypeSymbol1.OriginalDefinition, compilation.GetWellKnownType(WellKnownType.System_Linq_Expressions_Expression_T), TypeCompareKind.ConsiderEverything)) Then
					Dim item As TypeSymbol = namedTypeSymbol1.TypeArgumentsNoUseSiteDiagnostics(0)
					If (item.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Delegate]) Then
						namedTypeSymbol = Nothing
						Return namedTypeSymbol
					End If
					namedTypeSymbol = DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					Return namedTypeSymbol
				End If
			End If
			namedTypeSymbol = Nothing
			Return namedTypeSymbol
		End Function

		<Extension>
		Public Function GetAllTypeArguments(ByVal type As NamedTypeSymbol) As ImmutableArray(Of TypeSymbol)
			Dim typeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol) = type.TypeArgumentsNoUseSiteDiagnostics
			While True
				type = type.ContainingType
				If (type Is Nothing) Then
					Exit While
				End If
				typeArgumentsNoUseSiteDiagnostics = Microsoft.CodeAnalysis.ImmutableArrayExtensions.Concat(Of TypeSymbol)(type.TypeArgumentsNoUseSiteDiagnostics, typeArgumentsNoUseSiteDiagnostics)
			End While
			Return typeArgumentsNoUseSiteDiagnostics
		End Function

		<Extension>
		Public Function GetAllTypeArgumentsWithModifiers(ByVal type As NamedTypeSymbol) As ImmutableArray(Of TypeWithModifiers)
			Dim instance As ArrayBuilder(Of TypeWithModifiers) = ArrayBuilder(Of TypeWithModifiers).GetInstance()
			Do
				Dim typeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol) = type.TypeArgumentsNoUseSiteDiagnostics
				For i As Integer = typeArgumentsNoUseSiteDiagnostics.Length - 1 To 0 Step -1
					instance.Add(New TypeWithModifiers(typeArgumentsNoUseSiteDiagnostics(i), type.GetTypeArgumentCustomModifiers(i)))
				Next

				type = type.ContainingType
			Loop While type IsNot Nothing
			instance.ReverseContents()
			Return instance.ToImmutableAndFree()
		End Function

		<Extension>
		Public Function GetAllTypeParameters(ByVal type As NamedTypeSymbol) As ImmutableArray(Of TypeParameterSymbol)
			Dim immutableAndFree As ImmutableArray(Of TypeParameterSymbol)
			If (type.ContainingType IsNot Nothing) Then
				Dim instance As ArrayBuilder(Of TypeParameterSymbol) = ArrayBuilder(Of TypeParameterSymbol).GetInstance()
				type.GetAllTypeParameters(instance)
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = type.TypeParameters
			End If
			Return immutableAndFree
		End Function

		<Extension>
		Public Sub GetAllTypeParameters(ByVal type As NamedTypeSymbol, ByVal builder As ArrayBuilder(Of TypeParameterSymbol))
			Dim containingType As NamedTypeSymbol = type.ContainingType
			If (containingType IsNot Nothing) Then
				containingType.GetAllTypeParameters(builder)
			End If
			builder.AddRange(type.TypeParameters)
		End Sub

		<Extension>
		Public Function GetConstantValueTypeDiscriminator(ByVal this As TypeSymbol) As Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator
			Dim constantValueTypeDiscriminator As Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator
			If (this IsNot Nothing) Then
				this = this.GetEnumUnderlyingTypeOrSelf()
				Select Case this.SpecialType
					Case SpecialType.System_Boolean
						constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[Boolean]
						Exit Select
					Case SpecialType.System_Char
						constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[Char]
						Exit Select
					Case SpecialType.System_SByte
						constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[SByte]
						Exit Select
					Case SpecialType.System_Byte
						constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[Byte]
						Exit Select
					Case SpecialType.System_Int16
						constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.Int16
						Exit Select
					Case SpecialType.System_UInt16
						constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.UInt16
						Exit Select
					Case SpecialType.System_Int32
						constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.Int32
						Exit Select
					Case SpecialType.System_UInt32
						constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.UInt32
						Exit Select
					Case SpecialType.System_Int64
						constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.Int64
						Exit Select
					Case SpecialType.System_UInt64
						constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.UInt64
						Exit Select
					Case SpecialType.System_Decimal
						constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[Decimal]
						Exit Select
					Case SpecialType.System_Single
						constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[Single]
						Exit Select
					Case SpecialType.System_Double
						constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[Double]
						Exit Select
					Case SpecialType.System_String
						constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[String]
						Exit Select
					Case SpecialType.System_IntPtr
					Case SpecialType.System_UIntPtr
					Case SpecialType.System_Array
					Case SpecialType.System_Collections_IEnumerable
					Case SpecialType.System_Collections_Generic_IEnumerable_T
					Case SpecialType.System_Collections_Generic_IList_T
					Case SpecialType.System_Collections_Generic_ICollection_T
					Case SpecialType.System_Collections_IEnumerator
					Case SpecialType.System_Collections_Generic_IEnumerator_T
					Case SpecialType.System_Collections_Generic_IReadOnlyList_T
					Case SpecialType.System_Collections_Generic_IReadOnlyCollection_T
					Case SpecialType.System_Nullable_T
					Label0:
						If (this.IsTypeParameter() OrElse Not this.IsReferenceType) Then
							constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.Bad
							Exit Select
						Else
							constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[Nothing]
							Exit Select
						End If
					Case SpecialType.System_DateTime
						constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.DateTime
						Exit Select
					Case Else
						GoTo Label0
				End Select
			Else
				constantValueTypeDiscriminator = Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[Nothing]
			End If
			Return constantValueTypeDiscriminator
		End Function

		<Extension>
		Public Function GetElementTypesOfTupleOrCompatible(ByVal Type As TypeSymbol) As ImmutableArray(Of TypeSymbol)
			Dim immutableAndFree As ImmutableArray(Of TypeSymbol)
			If (Not Type.IsTupleType) Then
				Dim instance As ArrayBuilder(Of TypeSymbol) = ArrayBuilder(Of TypeSymbol).GetInstance()
				TupleTypeSymbol.AddElementTypes(DirectCast(Type, NamedTypeSymbol), instance)
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = DirectCast(Type, TupleTypeSymbol).TupleElementTypes
			End If
			Return immutableAndFree
		End Function

		<Extension>
		Public Function GetEnumUnderlyingType(ByVal type As TypeSymbol) As TypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			If (namedTypeSymbol IsNot Nothing) Then
				Return namedTypeSymbol.EnumUnderlyingType
			End If
			Return Nothing
		End Function

		<Extension>
		Public Function GetEnumUnderlyingTypeOrSelf(ByVal type As TypeSymbol) As TypeSymbol
			Return If(type.GetEnumUnderlyingType(), type)
		End Function

		<Extension>
		Public Function GetNativeCompilerVType(ByVal type As TypeSymbol) As String
			Dim nativeCompilerVType As String = type.SpecialType.GetNativeCompilerVType()
			If (nativeCompilerVType Is Nothing) Then
				If (Not type.IsTypeParameter()) Then
					If (type.IsArrayType()) Then
						Return "t_array"
					End If
					If (Not type.IsValueType) Then
						Return "t_ref"
					End If
					Return "t_struct"
				End If
				nativeCompilerVType = "t_generic"
			End If
			Return nativeCompilerVType
		End Function

		<Extension>
		Public Function GetNullableUnderlyingType(ByVal type As TypeSymbol) As TypeSymbol
			Return DirectCast(type, NamedTypeSymbol).TypeArgumentsNoUseSiteDiagnostics(0)
		End Function

		<Extension>
		Public Function GetNullableUnderlyingTypeOrSelf(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			typeSymbol = If(Not type.IsNullableType(), type, type.GetNullableUnderlyingType())
			Return typeSymbol
		End Function

		<Extension>
		Public Function GetSpecialTypeSafe(ByVal this As TypeSymbol) As SpecialType
			If (this Is Nothing) Then
				Return SpecialType.None
			End If
			Return this.SpecialType
		End Function

		<Extension>
		Public Function GetTupleUnderlyingType(ByVal type As TypeSymbol) As TypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			If (namedTypeSymbol IsNot Nothing) Then
				Return namedTypeSymbol.TupleUnderlyingType
			End If
			Return Nothing
		End Function

		<Extension>
		Public Function GetTupleUnderlyingTypeOrSelf(ByVal type As TypeSymbol) As TypeSymbol
			Return If(type.GetTupleUnderlyingType(), type)
		End Function

		<Extension>
		Friend Function GetTypeRefWithAttributes(ByVal type As TypeSymbol, ByVal declaringCompilation As VisualBasicCompilation, ByVal typeRef As ITypeReference) As TypeReferenceWithAttributes
			Dim typeReferenceWithAttribute As TypeReferenceWithAttributes
			Dim customAttributes As ImmutableArray(Of ICustomAttribute)
			If (type.ContainsTupleNames()) Then
				Dim synthesizedAttributeDatum As SynthesizedAttributeData = declaringCompilation.SynthesizeTupleNamesAttribute(type)
				If (synthesizedAttributeDatum Is Nothing) Then
					customAttributes = New ImmutableArray(Of ICustomAttribute)()
					typeReferenceWithAttribute = New TypeReferenceWithAttributes(typeRef, customAttributes)
					Return typeReferenceWithAttribute
				End If
				typeReferenceWithAttribute = New TypeReferenceWithAttributes(typeRef, ImmutableArray.Create(Of ICustomAttribute)(synthesizedAttributeDatum))
				Return typeReferenceWithAttribute
			End If
			customAttributes = New ImmutableArray(Of ICustomAttribute)()
			typeReferenceWithAttribute = New TypeReferenceWithAttributes(typeRef, customAttributes)
			Return typeReferenceWithAttribute
		End Function

		<Extension>
		Friend Function HasNameQualifier(ByVal type As NamedTypeSymbol, ByVal qualifiedName As String, ByVal comparison As StringComparison) As Boolean
			Dim flag As Boolean
			Dim containingSymbol As Symbol = type.ContainingSymbol
			If (containingSymbol.Kind = SymbolKind.[Namespace]) Then
				Dim emittedNamespaceName As String = type.GetEmittedNamespaceName()
				If (emittedNamespaceName Is Nothing) Then
					Dim namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = DirectCast(containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)
					flag = If(Not namespaceSymbol.IsGlobalNamespace, TypeSymbolExtensions.HasNamespaceName(namespaceSymbol, qualifiedName, comparison, qualifiedName.Length), qualifiedName.Length = 0)
				Else
					flag = [String].Equals(qualifiedName, emittedNamespaceName, comparison)
				End If
			Else
				flag = [String].Equals(containingSymbol.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat), qualifiedName, comparison)
			End If
			Return flag
		End Function

		Private Function HasNamespaceName(ByVal [namespace] As NamespaceSymbol, ByVal namespaceName As String, ByVal comparison As StringComparison, ByVal length As Integer) As Boolean
			Dim flag As Boolean
			If (length <> 0) Then
				Dim containingNamespace As NamespaceSymbol = [namespace].ContainingNamespace
				Dim num As Integer = namespaceName.LastIndexOf("."C, length - 1, length)
				Dim num1 As Integer = 0
				If (num < 0) Then
					If (containingNamespace.IsGlobalNamespace) Then
						GoTo Label1
					End If
					flag = False
					Return flag
				ElseIf (Not containingNamespace.IsGlobalNamespace) Then
					If (Not TypeSymbolExtensions.HasNamespaceName(containingNamespace, namespaceName, comparison, num)) Then
						flag = False
						Return flag
					End If
					Dim num2 As Integer = num + 1
					num1 = num2
					length -= num2
				Else
					flag = False
					Return flag
				End If
			Label1:
				Dim name As String = [namespace].Name
				flag = If(name.Length <> length, False, [String].Compare(name, 0, namespaceName, num1, length, comparison) = 0)
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function HasSameTupleNames(ByVal t1 As TypeSymbol, ByVal t2 As TypeSymbol) As Boolean
			Dim flag As Boolean
			Dim tupleElementNames As ImmutableArray(Of String) = t1.TupleElementNames
			Dim strs As ImmutableArray(Of String) = t2.TupleElementNames
			If (Not tupleElementNames.IsDefault OrElse Not strs.IsDefault) Then
				flag = If(tupleElementNames.IsDefault OrElse strs.IsDefault, False, tupleElementNames.SequenceEqual(Of String)(strs, New Func(Of String, String, Boolean)(AddressOf CaseInsensitiveComparison.Equals)))
			Else
				flag = True
			End If
			Return flag
		End Function

		Friend Function HasSameTypeArgumentCustomModifiers(ByVal type1 As NamedTypeSymbol, ByVal type2 As NamedTypeSymbol) As Boolean
			Dim flag As Boolean
			Dim hasTypeArgumentsCustomModifiers As Boolean = type1.HasTypeArgumentsCustomModifiers
			Dim hasTypeArgumentsCustomModifiers1 As Boolean = type2.HasTypeArgumentsCustomModifiers
			If (Not hasTypeArgumentsCustomModifiers AndAlso Not hasTypeArgumentsCustomModifiers1) Then
				flag = True
			ElseIf (Not hasTypeArgumentsCustomModifiers OrElse Not hasTypeArgumentsCustomModifiers1) Then
				flag = False
			Else
				Dim arity As Integer = type1.Arity - 1
				Dim num As Integer = 0
				While num <= arity
					If (type1.GetTypeArgumentCustomModifiers(num).AreSameCustomModifiers(type2.GetTypeArgumentCustomModifiers(num))) Then
						num = num + 1
					Else
						flag = False
						Return flag
					End If
				End While
				flag = True
			End If
			Return flag
		End Function

		<Extension>
		Public Function ImplementsInterface(ByVal subType As TypeSymbol, ByVal superInterface As TypeSymbol, ByVal comparer As EqualityComparer(Of TypeSymbol), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			If (comparer Is Nothing) Then
				comparer = EqualityComparer(Of TypeSymbol).[Default]
			End If
			Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = subType.AllInterfacesWithDefinitionUseSiteDiagnostics(useSiteInfo).GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As NamedTypeSymbol = enumerator.Current
					If (current.IsInterface AndAlso comparer.Equals(current, superInterface)) Then
						flag = True
						Exit While
					End If
				Else
					flag = False
					Exit While
				End If
			End While
			Return flag
		End Function

		<Extension>
		Friend Function IsArrayType(ByVal type As TypeSymbol) As Boolean
			Return type.Kind = SymbolKind.ArrayType
		End Function

		<Extension>
		Public Function IsBaseTypeOf(ByVal superType As TypeSymbol, ByVal subType As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			Dim baseTypeNoUseSiteDiagnostics As TypeSymbol = subType
			While True
				If (baseTypeNoUseSiteDiagnostics IsNot Nothing) Then
					If (CObj(baseTypeNoUseSiteDiagnostics) <> CObj(subType)) Then
						baseTypeNoUseSiteDiagnostics.OriginalDefinition.AddUseSiteInfo(useSiteInfo)
					End If
					If (Not baseTypeNoUseSiteDiagnostics.IsSameTypeIgnoringAll(superType)) Then
						baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics
					Else
						flag = True
						Exit While
					End If
				Else
					flag = False
					Exit While
				End If
			End While
			Return flag
		End Function

		<Extension>
		Public Function IsBaseTypeOrInterfaceOf(ByVal superType As TypeSymbol, ByVal subType As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			flag = If(Not superType.IsInterfaceType(), superType.IsBaseTypeOf(subType, useSiteInfo), subType.ImplementsInterface(superType, EqualsIgnoringComparer.InstanceCLRSignatureCompare, useSiteInfo))
			Return flag
		End Function

		<Extension>
		Public Function IsBooleanType(ByVal this As TypeSymbol) As Boolean
			Return this.SpecialType = SpecialType.System_Boolean
		End Function

		<Extension>
		Friend Function IsCharSZArray(ByVal type As TypeSymbol) As Boolean
			Dim flag As Boolean
			If (type.IsArrayType()) Then
				Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
				If (Not arrayTypeSymbol.IsSZArray OrElse arrayTypeSymbol.ElementType.SpecialType <> SpecialType.System_Char) Then
					flag = False
					Return flag
				End If
				flag = True
				Return flag
			End If
			flag = False
			Return flag
		End Function

		<Extension>
		Public Function IsCharType(ByVal this As TypeSymbol) As Boolean
			Return this.SpecialType = SpecialType.System_Char
		End Function

		<Extension>
		Friend Function IsClassOrInterfaceType(ByVal type As TypeSymbol) As Boolean
			If (type.IsClassType()) Then
				Return True
			End If
			Return type.IsInterfaceType()
		End Function

		<Extension>
		Friend Function IsClassType(ByVal type As TypeSymbol) As Boolean
			Return type.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Class]
		End Function

		<Extension>
		Public Function IsCompatibleWithGenericIEnumerableOfType(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal typeArgument As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			Dim enumerator As HashSet(Of NamedTypeSymbol).Enumerator = New HashSet(Of NamedTypeSymbol).Enumerator()
			If (Not typeArgument.IsErrorType()) Then
				Dim elementType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = typeArgument
				While elementType.IsArrayType()
					elementType = DirectCast(elementType, ArrayTypeSymbol).ElementType
				End While
				If (Not elementType.IsErrorType()) Then
					Dim specialType As NamedTypeSymbol = elementType.ContainingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IEnumerable_T)
					Dim namedTypeSymbols As HashSet(Of NamedTypeSymbol) = New HashSet(Of NamedTypeSymbol)()
					If (Binder.IsOrInheritsFromOrImplementsInterface(type, specialType, useSiteInfo, namedTypeSymbols) AndAlso namedTypeSymbols.Count > 0) Then
						Try
							enumerator = namedTypeSymbols.GetEnumerator()
							While enumerator.MoveNext()
								Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = enumerator.Current.TypeArgumentWithDefinitionUseSiteDiagnostics(0, useSiteInfo)
								If (Not typeSymbol.IsErrorType()) Then
									If (Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsWideningConversion(Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(typeSymbol, typeArgument, useSiteInfo))) Then
										Continue While
									End If
									flag = True
									Return flag
								Else
									flag = False
									Return flag
								End If
							End While
						Finally
							DirectCast(enumerator, IDisposable).Dispose()
						End Try
					End If
					flag = False
				Else
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		<Extension>
		Friend Function IsCompilerServicesTopLevelType(ByVal typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) As Boolean
			If (typeSymbol.ContainingType IsNot Nothing) Then
				Return False
			End If
			Return typeSymbol.IsContainedInNamespace("System", "Runtime", "CompilerServices")
		End Function

		<Extension>
		Private Function IsContainedInNamespace(ByVal typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal outerNS As String, ByVal midNS As String, ByVal innerNS As String) As Boolean
			Dim flag As Boolean
			Dim name As String
			Dim str As String
			Dim name1 As String
			Dim containingNamespace As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = typeSymbol.ContainingNamespace
			If (containingNamespace IsNot Nothing) Then
				name = containingNamespace.Name
			Else
				name = Nothing
			End If
			If ([String].Equals(name, innerNS)) Then
				Dim namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = containingNamespace.ContainingNamespace
				If (namespaceSymbol IsNot Nothing) Then
					str = namespaceSymbol.Name
				Else
					str = Nothing
				End If
				If ([String].Equals(str, midNS)) Then
					Dim containingNamespace1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = namespaceSymbol.ContainingNamespace
					If (containingNamespace1 IsNot Nothing) Then
						name1 = containingNamespace1.Name
					Else
						name1 = Nothing
					End If
					If ([String].Equals(name1, outerNS)) Then
						Dim namespaceSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = containingNamespace1.ContainingNamespace
						flag = If(namespaceSymbol1 Is Nothing, False, namespaceSymbol1.IsGlobalNamespace)
					Else
						flag = False
					End If
				Else
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		<Extension>
		Public Function IsDateTimeType(ByVal this As TypeSymbol) As Boolean
			Return this.SpecialType = SpecialType.System_DateTime
		End Function

		<Extension>
		Friend Function IsDBNullType(ByVal type As TypeSymbol) As Boolean
			Dim flag As Boolean
			flag = If(type.SpecialType <> SpecialType.None OrElse type.Kind <> SymbolKind.NamedType OrElse Not [String].Equals(type.Name, "DBNull", StringComparison.Ordinal) OrElse Not DirectCast(type, NamedTypeSymbol).HasNameQualifier("System", StringComparison.Ordinal), False, True)
			Return flag
		End Function

		<Extension>
		Public Function IsDecimalType(ByVal this As TypeSymbol) As Boolean
			Return this.SpecialType = SpecialType.System_Decimal
		End Function

		<Extension>
		Friend Function IsDelegateType(ByVal type As TypeSymbol) As Boolean
			Return type.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Delegate]
		End Function

		<Extension>
		Public Function IsDoubleType(ByVal this As TypeSymbol) As Boolean
			Return this.SpecialType = SpecialType.System_Double
		End Function

		<Extension>
		Friend Function IsEnumType(ByVal type As TypeSymbol) As Boolean
			Return type.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Enum]
		End Function

		<Extension>
		Friend Function IsErrorType(ByVal type As TypeSymbol) As Boolean
			Return type.Kind = SymbolKind.ErrorType
		End Function

		<Extension>
		Public Function IsExpressionTree(ByVal type As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Boolean
			Return CObj(type.ExpressionTargetDelegate(binder.Compilation)) <> CObj(Nothing)
		End Function

		<Extension>
		Public Function IsExtensibleInterfaceNoUseSiteDiagnostics(ByVal type As TypeSymbol) As Boolean
			If (Not type.IsInterfaceType()) Then
				Return False
			End If
			Return DirectCast(type, NamedTypeSymbol).IsExtensibleInterfaceNoUseSiteDiagnostics
		End Function

		<Extension>
		Public Function IsFloatingType(ByVal this As TypeSymbol) As Boolean
			Return this.SpecialType.IsFloatingType()
		End Function

		<Extension>
		Public Function IsIntegralType(ByVal this As TypeSymbol) As Boolean
			Return this.SpecialType.IsIntegralType()
		End Function

		<Extension>
		Friend Function IsInterfaceType(ByVal type As TypeSymbol) As Boolean
			If (type.Kind <> SymbolKind.NamedType) Then
				Return False
			End If
			Return DirectCast(type, NamedTypeSymbol).IsInterface
		End Function

		<Extension>
		Public Function IsIntrinsicOrEnumType(ByVal type As TypeSymbol) As Boolean
			If (type Is Nothing) Then
				Return False
			End If
			Return type.GetEnumUnderlyingTypeOrSelf().IsIntrinsicType()
		End Function

		<Extension>
		Public Function IsIntrinsicType(ByVal this As TypeSymbol) As Boolean
			Return this.SpecialType.IsIntrinsicType()
		End Function

		<Extension>
		Public Function IsIntrinsicValueType(ByVal this As TypeSymbol) As Boolean
			Return this.SpecialType.IsIntrinsicValueType()
		End Function

		Private Function IsMethodTypeParameter(ByVal type As TypeSymbol, ByVal method As MethodSymbol) As Boolean
			If (type.TypeKind <> Microsoft.CodeAnalysis.TypeKind.TypeParameter) Then
				Return False
			End If
			Return type.ContainingSymbol.Equals(method)
		End Function

		<Extension>
		Friend Function IsMicrosoftVisualBasicCollection(ByVal type As TypeSymbol) As Boolean
			Dim flag As Boolean
			flag = If(type.SpecialType <> SpecialType.None OrElse type.Kind <> SymbolKind.NamedType OrElse Not [String].Equals(type.Name, "Collection", StringComparison.Ordinal) OrElse Not DirectCast(type, NamedTypeSymbol).HasNameQualifier("Microsoft.VisualBasic", StringComparison.Ordinal), False, True)
			Return flag
		End Function

		<Extension>
		Friend Function IsModuleType(ByVal type As TypeSymbol) As Boolean
			Return type.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Module]
		End Function

		<Extension>
		Public Function IsNotInheritable(ByVal this As TypeSymbol) As Boolean
			Dim flag As Boolean
			Select Case this.TypeKind
				Case Microsoft.CodeAnalysis.TypeKind.Unknown
				Case Microsoft.CodeAnalysis.TypeKind.[Interface]
				Case Microsoft.CodeAnalysis.TypeKind.TypeParameter
					flag = False
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.Array
				Case Microsoft.CodeAnalysis.TypeKind.[Delegate]
				Case Microsoft.CodeAnalysis.TypeKind.[Enum]
				Case Microsoft.CodeAnalysis.TypeKind.[Module]
				Case Microsoft.CodeAnalysis.TypeKind.Struct
					flag = True
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.[Class]
				Case Microsoft.CodeAnalysis.TypeKind.[Error]
				Case Microsoft.CodeAnalysis.TypeKind.Submission
					flag = DirectCast(this, NamedTypeSymbol).IsNotInheritable
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.Dynamic
				Case Microsoft.CodeAnalysis.TypeKind.Pointer
					Throw ExceptionUtilities.UnexpectedValue(this.TypeKind)
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(this.TypeKind)
			End Select
			Return flag
		End Function

		<Extension>
		Public Function IsNullableOfBoolean(ByVal this As TypeSymbol) As Boolean
			If (Not this.IsNullableType()) Then
				Return False
			End If
			Return this.GetNullableUnderlyingType().IsBooleanType()
		End Function

		<Extension>
		Public Function IsNullableType(ByVal this As TypeSymbol) As Boolean
			Return this.OriginalDefinition.SpecialType = SpecialType.System_Nullable_T
		End Function

		<Extension>
		Public Function IsNumericType(ByVal this As TypeSymbol) As Boolean
			Return this.SpecialType.IsNumericType()
		End Function

		<Extension>
		Public Function IsObjectType(ByVal this As TypeSymbol) As Boolean
			Return this.SpecialType = SpecialType.System_Object
		End Function

		<Extension>
		Public Function IsOrDerivedFrom(ByVal derivedType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal baseType As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = derivedType
			While True
				If (namedTypeSymbol Is Nothing) Then
					flag = False
					Exit While
				ElseIf (Not namedTypeSymbol.IsSameTypeIgnoringAll(baseType)) Then
					namedTypeSymbol = namedTypeSymbol.BaseTypeWithDefinitionUseSiteDiagnostics(useSiteInfo)
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		<Extension>
		Public Function IsOrDerivedFrom(ByVal derivedType As TypeSymbol, ByVal baseType As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			While True
				If (derivedType IsNot Nothing) Then
					Dim typeKind As Microsoft.CodeAnalysis.TypeKind = derivedType.TypeKind
					If (typeKind = Microsoft.CodeAnalysis.TypeKind.Array) Then
						derivedType = derivedType.BaseTypeWithDefinitionUseSiteDiagnostics(useSiteInfo)
					ElseIf (typeKind = Microsoft.CodeAnalysis.TypeKind.TypeParameter) Then
						derivedType = DirectCast(derivedType, TypeParameterSymbol).GetNonInterfaceConstraint(useSiteInfo)
					Else
						flag = DirectCast(derivedType, NamedTypeSymbol).IsOrDerivedFrom(baseType, useSiteInfo)
						Exit While
					End If
				Else
					flag = False
					Exit While
				End If
			End While
			Return flag
		End Function

		<Extension>
		Public Function IsOrDerivedFromWellKnownClass(ByVal derivedType As TypeSymbol, ByVal wellKnownBaseType As WellKnownType, ByVal compilation As VisualBasicCompilation, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Return derivedType.IsOrDerivedFrom(compilation.GetWellKnownType(wellKnownBaseType), useSiteInfo)
		End Function

		<Extension>
		Public Function IsOrImplementsIEnumerableOfXElement(ByVal type As TypeSymbol, ByVal compilation As VisualBasicCompilation, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Return type.IsCompatibleWithGenericIEnumerableOfType(compilation.GetWellKnownType(WellKnownType.System_Xml_Linq_XElement), useSiteInfo)
		End Function

		<Extension>
		Friend Function IsOrRefersToTypeParameter(ByVal this As TypeSymbol) As Boolean
			Return CObj(this.VisitType(Of Object)(TypeSymbolExtensions.s_isTypeParameterFunc, Nothing)) <> CObj(Nothing)
		End Function

		<Extension>
		Public Function IsPrimitiveType(ByVal t As TypeSymbol) As Boolean
			Return t.SpecialType.IsPrimitiveType()
		End Function

		<Extension>
		Public Function IsRestrictedArrayType(ByVal this As TypeSymbol, <Out> ByRef restrictedType As TypeSymbol) As Boolean
			Dim flag As Boolean
			If (this.Kind <> SymbolKind.ArrayType) Then
				restrictedType = Nothing
				flag = False
			Else
				flag = this.IsRestrictedTypeOrArrayType(restrictedType)
			End If
			Return flag
		End Function

		<Extension>
		Public Function IsRestrictedType(ByVal this As TypeSymbol) As Boolean
			Return this.SpecialType.IsRestrictedType()
		End Function

		<Extension>
		Public Function IsRestrictedTypeOrArrayType(ByVal this As TypeSymbol, <Out> ByRef restrictedType As TypeSymbol) As Boolean
			Dim flag As Boolean
			While this.Kind = SymbolKind.ArrayType
				this = DirectCast(this, ArrayTypeSymbol).ElementType
			End While
			If (Not this.IsRestrictedType()) Then
				restrictedType = Nothing
				flag = False
			Else
				restrictedType = this
				flag = True
			End If
			Return flag
		End Function

		<Extension>
		Public Function IsSameOrNestedWithin(ByVal inner As NamedTypeSymbol, ByVal outer As NamedTypeSymbol) As Boolean
			Dim flag As Boolean
			While True
				If (Not TypeSymbol.Equals(inner, outer, TypeCompareKind.ConsiderEverything)) Then
					inner = inner.ContainingType
					If (inner Is Nothing) Then
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

		<Extension>
		Friend Function IsSameType(ByVal t1 As TypeSymbol, ByVal t2 As TypeSymbol, ByVal compareKind As TypeCompareKind) As Boolean
			Return TypeSymbol.Equals(t1, t2, compareKind)
		End Function

		<Extension>
		Friend Function IsSameTypeIgnoringAll(ByVal t1 As TypeSymbol, ByVal t2 As TypeSymbol) As Boolean
			Return t1.IsSameType(t2, TypeCompareKind.AllIgnoreOptionsForVB)
		End Function

		<Extension>
		Public Function IsSignedIntegralType(ByVal this As TypeSymbol) As Boolean
			Return this.SpecialType.IsSignedIntegralType()
		End Function

		<Extension>
		Public Function IsSingleType(ByVal this As TypeSymbol) As Boolean
			Return this.SpecialType = SpecialType.System_Single
		End Function

		<Extension>
		Public Function IsStrictSupertypeOfConcreteDelegate(ByVal this As TypeSymbol) As Boolean
			Return this.SpecialType.IsStrictSupertypeOfConcreteDelegate()
		End Function

		<Extension>
		Public Function IsStringType(ByVal this As TypeSymbol) As Boolean
			Return this.SpecialType = SpecialType.System_String
		End Function

		<Extension>
		Friend Function IsStructureType(ByVal type As TypeSymbol) As Boolean
			Return type.TypeKind = Microsoft.CodeAnalysis.TypeKind.Struct
		End Function

		<Extension>
		Public Function IsTopLevelType(ByVal type As NamedTypeSymbol) As Boolean
			Return type.ContainingType Is Nothing
		End Function

		<Extension>
		Friend Function IsTypeParameter(ByVal type As TypeSymbol) As Boolean
			Return type.Kind = SymbolKind.TypeParameter
		End Function

		Private Function IsTypeParameterNotInSet(ByVal type As TypeSymbol, ByVal typeParameters As HashSet(Of TypeParameterSymbol)) As Boolean
			If (type.TypeKind <> Microsoft.CodeAnalysis.TypeKind.TypeParameter) Then
				Return False
			End If
			Return Not typeParameters.Contains(DirectCast(type, TypeParameterSymbol))
		End Function

		<Extension>
		Public Function IsUnboundGenericType(ByVal this As TypeSymbol) As Boolean
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(this, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			If (namedTypeSymbol Is Nothing) Then
				Return False
			End If
			Return namedTypeSymbol.IsUnboundGenericType
		End Function

		<Extension>
		Public Function IsUnsignedIntegralType(ByVal this As TypeSymbol) As Boolean
			Return this.SpecialType.IsUnsignedIntegralType()
		End Function

		<Extension>
		Friend Function IsValidEnumUnderlyingType(ByVal type As TypeSymbol) As Boolean
			Return type.SpecialType.IsValidEnumUnderlyingType()
		End Function

		<Extension>
		Public Function IsValidForConstantValue(ByVal this As TypeSymbol, ByVal value As ConstantValue) As Boolean
			Dim constantValueTypeDiscriminator As Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator = this.GetConstantValueTypeDiscriminator()
			If (constantValueTypeDiscriminator <> Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.Bad AndAlso constantValueTypeDiscriminator = value.Discriminator) Then
				Return True
			End If
			If (value.Discriminator <> Microsoft.CodeAnalysis.ConstantValueTypeDiscriminator.[Nothing]) Then
				Return False
			End If
			Return this.IsStringType()
		End Function

		<Extension>
		Public Function IsValidTypeForAttributeArgument(ByVal type As TypeSymbol, ByVal compilation As VisualBasicCompilation) As Boolean
			Dim flag As Boolean
			If (type IsNot Nothing) Then
				If (type.IsArrayType()) Then
					Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
					If (Not arrayTypeSymbol.IsSZArray) Then
						flag = False
						Return flag
					End If
					type = arrayTypeSymbol.ElementType
				End If
				flag = If(type.GetEnumUnderlyingTypeOrSelf().SpecialType.IsValidTypeForAttributeArgument(), True, TypeSymbol.Equals(type, compilation.GetWellKnownType(WellKnownType.System_Type), TypeCompareKind.ConsiderEverything))
			Else
				flag = False
			End If
			Return flag
		End Function

		<Extension>
		Friend Function IsValidTypeForConstField(ByVal fieldType As TypeSymbol) As Boolean
			If (fieldType.IsIntrinsicType() OrElse fieldType.SpecialType = SpecialType.System_Object) Then
				Return True
			End If
			Return fieldType.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Enum]
		End Function

		<Extension>
		Public Function IsValidTypeForSwitchTable(ByVal type As TypeSymbol) As Boolean
			type = type.GetNullableUnderlyingTypeOrSelf()
			type = type.GetEnumUnderlyingTypeOrSelf()
			Return type.SpecialType.IsValidTypeForSwitchTable()
		End Function

		<Extension>
		Public Function IsVerifierReference(ByVal type As TypeSymbol) As Boolean
			Return If(type.TypeKind <> Microsoft.CodeAnalysis.TypeKind.TypeParameter, type.IsReferenceType, False)
		End Function

		<Extension>
		Public Function IsVerifierValue(ByVal type As TypeSymbol) As Boolean
			Return If(type.TypeKind <> Microsoft.CodeAnalysis.TypeKind.TypeParameter, type.IsValueType, False)
		End Function

		<Extension>
		Public Function IsVoidType(ByVal this As TypeSymbol) As Boolean
			Return this.SpecialType = SpecialType.System_Void
		End Function

		<Extension>
		Private Function IsWellKnownCompilerServicesTopLevelType(ByVal typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal name As String) As Boolean
			Dim flag As Boolean
			flag = If([String].Equals(typeSymbol.Name, name), typeSymbol.IsCompilerServicesTopLevelType(), False)
			Return flag
		End Function

		<Extension>
		Friend Function IsWellKnownTypeIsExternalInit(ByVal typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) As Boolean
			Return typeSymbol.IsWellKnownCompilerServicesTopLevelType("IsExternalInit")
		End Function

		<Extension>
		Public Function MarkCheckedIfNecessary(ByVal type As TypeSymbol, ByRef checkedTypes As HashSet(Of TypeSymbol)) As Boolean
			If (checkedTypes Is Nothing) Then
				checkedTypes = New HashSet(Of TypeSymbol)()
			End If
			Return checkedTypes.Add(type)
		End Function

		<Extension>
		Friend Function ReferencesMethodsTypeParameter(ByVal this As TypeSymbol, ByVal method As MethodSymbol) As Boolean
			Return CObj(this.VisitType(Of MethodSymbol)(TypeSymbolExtensions.s_isMethodTypeParameterFunc, method)) <> CObj(Nothing)
		End Function

		<Extension>
		Friend Function ReferencesTypeParameterNotInTheSet(ByVal this As TypeSymbol, ByVal typeParameters As HashSet(Of TypeParameterSymbol)) As Boolean
			Return CObj(this.VisitType(Of HashSet(Of TypeParameterSymbol))(TypeSymbolExtensions.s_isTypeParameterNotInSetFunc, typeParameters)) <> CObj(Nothing)
		End Function

		<Extension>
		Friend Function TransformToCanonicalFormFor(ByVal typeArguments As ImmutableArray(Of TypeSymbol), ByVal genericType As SubstitutedNamedType.SpecializedGenericType) As ImmutableArray(Of TypeSymbol)
			Return TypeSymbolExtensions.TransformToCanonicalFormFor(typeArguments, genericType, genericType.TypeParameters)
		End Function

		<Extension>
		Friend Function TransformToCanonicalFormFor(ByVal typeArguments As ImmutableArray(Of TypeSymbol), ByVal genericMethod As SubstitutedMethodSymbol.SpecializedGenericMethod) As ImmutableArray(Of TypeSymbol)
			Return TypeSymbolExtensions.TransformToCanonicalFormFor(typeArguments, genericMethod, genericMethod.TypeParameters)
		End Function

		Private Function TransformToCanonicalFormFor(ByVal typeArguments As ImmutableArray(Of TypeSymbol), ByVal specializedGenericTypeOrMethod As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal specializedTypeParameters As ImmutableArray(Of TypeParameterSymbol)) As ImmutableArray(Of TypeSymbol)
			Dim typeSymbols As ImmutableArray(Of TypeSymbol)
			Dim item As TypeSymbol
			Dim array As TypeSymbol() = Nothing
			Dim num As Integer = 0
			Do
				item = typeArguments(num)
				If (item.IsTypeParameter() AndAlso Not item.IsDefinition) Then
					Dim containingSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = item.ContainingSymbol
					If (CObj(containingSymbol) <> CObj(specializedGenericTypeOrMethod) AndAlso containingSymbol.Equals(specializedGenericTypeOrMethod)) Then
						array = typeArguments.ToArray()
						Exit Do
					End If
				End If
				num = num + 1
			Loop While num < typeArguments.Length
			If (array IsNot Nothing) Then
				array(num) = specializedTypeParameters(DirectCast(item, TypeParameterSymbol).Ordinal)
				num = num + 1
				Do
					item = typeArguments(num)
					If (item.IsTypeParameter() AndAlso Not item.IsDefinition) Then
						Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = item.ContainingSymbol
						If (CObj(symbol) <> CObj(specializedGenericTypeOrMethod) AndAlso symbol.Equals(specializedGenericTypeOrMethod)) Then
							array(num) = specializedTypeParameters(DirectCast(item, TypeParameterSymbol).Ordinal)
						End If
					End If
					num = num + 1
				Loop While num < typeArguments.Length
				typeArguments = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeSymbol)(array)
			End If
			Dim length As Integer = specializedTypeParameters.Length - 1
			num = 0
			While True
				If (num > length) Then
					typeSymbols = New ImmutableArray(Of TypeSymbol)()
					Exit While
				ElseIf (specializedTypeParameters(num) = typeArguments(num)) Then
					num = num + 1
				Else
					typeSymbols = typeArguments
					Exit While
				End If
			End While
			Return typeSymbols
		End Function

		<Extension>
		Public Function TryGetElementTypesIfTupleOrCompatible(ByVal type As TypeSymbol, <Out> ByRef elementTypes As ImmutableArray(Of TypeSymbol)) As Boolean
			Dim flag As Boolean
			Dim num As Integer
			If (type.IsTupleType) Then
				elementTypes = DirectCast(type, TupleTypeSymbol).TupleElementTypes
				flag = True
			ElseIf (type.IsTupleCompatible(num)) Then
				Dim instance As ArrayBuilder(Of TypeSymbol) = ArrayBuilder(Of TypeSymbol).GetInstance(num)
				TupleTypeSymbol.AddElementTypes(DirectCast(type, NamedTypeSymbol), instance)
				elementTypes = instance.ToImmutableAndFree()
				flag = True
			Else
				elementTypes = New ImmutableArray(Of TypeSymbol)()
				flag = False
			End If
			Return flag
		End Function

		<Extension>
		Public Function TypeToIndex(ByVal type As TypeSymbol) As Nullable(Of Integer)
			Return type.SpecialType.TypeToIndex()
		End Function

		<Extension>
		Friend Function VisitType(Of T)(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal predicate As Func(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, T, Boolean), ByVal arg As T) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim elementType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = type
			While True
				Select Case elementType.TypeKind
					Case Microsoft.CodeAnalysis.TypeKind.[Class]
					Case Microsoft.CodeAnalysis.TypeKind.[Delegate]
					Case Microsoft.CodeAnalysis.TypeKind.[Enum]
					Case Microsoft.CodeAnalysis.TypeKind.[Interface]
					Case Microsoft.CodeAnalysis.TypeKind.Struct
						Dim containingType As NamedTypeSymbol = elementType.ContainingType
						If (containingType Is Nothing) Then
							GoTo Label0
						End If
						Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = containingType.VisitType(Of T)(predicate, arg)
						If (typeSymbol1 Is Nothing) Then
							GoTo Label0
						End If
						typeSymbol = typeSymbol1

					Case Microsoft.CodeAnalysis.TypeKind.Dynamic
					Case Microsoft.CodeAnalysis.TypeKind.[Error]
					Case Microsoft.CodeAnalysis.TypeKind.[Module]
					Case Microsoft.CodeAnalysis.TypeKind.Pointer
					Case Microsoft.CodeAnalysis.TypeKind.TypeParameter
					Case Microsoft.CodeAnalysis.TypeKind.Submission
					Label0:
						If (Not predicate(elementType, arg)) Then
							Select Case elementType.TypeKind
								Case Microsoft.CodeAnalysis.TypeKind.Array
									elementType = DirectCast(elementType, ArrayTypeSymbol).ElementType
									Continue While
								Case Microsoft.CodeAnalysis.TypeKind.[Class]
								Case Microsoft.CodeAnalysis.TypeKind.[Delegate]
								Case Microsoft.CodeAnalysis.TypeKind.[Error]
								Case Microsoft.CodeAnalysis.TypeKind.[Interface]
								Case Microsoft.CodeAnalysis.TypeKind.Struct
									If (elementType.IsTupleType) Then
										elementType = elementType.TupleUnderlyingType
									End If
									Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol).Enumerator = DirectCast(elementType, NamedTypeSymbol).TypeArgumentsNoUseSiteDiagnostics.GetEnumerator()
									While enumerator.MoveNext()
										Dim typeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = enumerator.Current.VisitType(Of T)(predicate, arg)
										If (typeSymbol2 Is Nothing) Then
											Continue While
										End If
										typeSymbol = typeSymbol2
										Return typeSymbol
									End While
									typeSymbol = Nothing

								Case Microsoft.CodeAnalysis.TypeKind.Dynamic
								Case Microsoft.CodeAnalysis.TypeKind.[Enum]
								Case Microsoft.CodeAnalysis.TypeKind.[Module]
								Case Microsoft.CodeAnalysis.TypeKind.TypeParameter
								Case Microsoft.CodeAnalysis.TypeKind.Submission
									typeSymbol = Nothing

								Case Microsoft.CodeAnalysis.TypeKind.Pointer
									Throw ExceptionUtilities.UnexpectedValue(elementType.TypeKind)
								Case Else
									Throw ExceptionUtilities.UnexpectedValue(elementType.TypeKind)
							End Select
						Else
							typeSymbol = elementType
						End If

					Case Else
						GoTo Label0
				End Select
			End While
			Return typeSymbol
		End Function
	End Module
End Namespace