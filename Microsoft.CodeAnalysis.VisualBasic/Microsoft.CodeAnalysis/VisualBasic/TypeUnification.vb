Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class TypeUnification
		Public Sub New()
			MyBase.New()
		End Sub

		Private Shared Sub AddSubstitution(ByRef substitution As TypeSubstitution, ByVal targetGenericType As NamedTypeSymbol, ByVal tp As TypeParameterSymbol, ByVal typeArgument As TypeWithModifiers)
			If (substitution Is Nothing) Then
				substitution = TypeSubstitution.Create(targetGenericType, New TypeParameterSymbol() { tp }, New TypeWithModifiers() { typeArgument }, False)
				Return
			End If
			Dim instance As ArrayBuilder(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)) = ArrayBuilder(Of KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)).GetInstance()
			instance.AddRange(substitution.PairsIncludingParent)
			Dim count As Integer = instance.Count
			Dim num As Integer = 0
			While num <= count
				If (num > instance.Count - 1 OrElse instance(num).Key.ContainingType.IsSameOrNestedWithin(tp.ContainingType)) Then
					instance.Insert(num, New KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers)(tp, typeArgument))
					Exit While
				Else
					num = num + 1
				End If
			End While
			Dim count1 As Integer = instance.Count
			Dim key(count1 - 1 + 1 - 1) As TypeParameterSymbol
			Dim value(count1 - 1 + 1 - 1) As TypeWithModifiers
			Dim num1 As Integer = count1 - 1
			Dim num2 As Integer = 0
			Do
				Dim item As KeyValuePair(Of TypeParameterSymbol, TypeWithModifiers) = instance(num2)
				key(num2) = item.Key
				item = instance(num2)
				value(num2) = item.Value
				num2 = num2 + 1
			Loop While num2 <= num1
			instance.Free()
			substitution = TypeSubstitution.Create(targetGenericType, key, value, False)
		End Sub

		Public Shared Function CanUnify(ByVal containingGenericType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal t1 As TypeSymbol, ByVal t2 As TypeSymbol) As Boolean
			Dim flag As Boolean
			Dim typeWithModifier As TypeWithModifiers
			Dim typeWithModifier1 As TypeWithModifiers
			Dim typeWithModifier2 As TypeWithModifiers
			If (Not containingGenericType.IsGenericType) Then
				flag = False
			ElseIf (Not TypeSymbol.Equals(t1, t2, TypeCompareKind.ConsiderEverything)) Then
				Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Nothing
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = containingGenericType
				If (t1 Is Nothing) Then
					typeWithModifier = New TypeWithModifiers()
					typeWithModifier1 = typeWithModifier
				Else
					typeWithModifier1 = New TypeWithModifiers(t1)
				End If
				If (t2 Is Nothing) Then
					typeWithModifier = New TypeWithModifiers()
					typeWithModifier2 = typeWithModifier
				Else
					typeWithModifier2 = New TypeWithModifiers(t2)
				End If
				flag = TypeUnification.CanUnifyHelper(namedTypeSymbol, typeWithModifier1, typeWithModifier2, typeSubstitution)
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Shared Function CanUnifyHelper(ByVal containingGenericType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal t1 As TypeWithModifiers, ByVal t2 As TypeWithModifiers, ByRef substitution As TypeSubstitution) As Boolean
			Dim flag As Boolean
			Dim kind As SymbolKind
			Dim customModifiers As ImmutableArray(Of CustomModifier)
			Dim typeArgumentCustomModifiers As ImmutableArray(Of CustomModifier)
			Dim typeArgumentCustomModifiers1 As ImmutableArray(Of CustomModifier)
			If (t1 = t2) Then
				flag = True
			ElseIf (t1.Type Is Nothing OrElse t2.Type Is Nothing) Then
				flag = False
			Else
				If (substitution IsNot Nothing) Then
					t1 = t1.InternalSubstituteTypeParameters(substitution)
					t2 = t2.InternalSubstituteTypeParameters(substitution)
				End If
				If (t1 <> t2) Then
					If (Not t1.Type.IsTypeParameter() AndAlso t2.Type.IsTypeParameter()) Then
						Dim typeWithModifier As TypeWithModifiers = t1
						t1 = t2
						t2 = typeWithModifier
					End If
					kind = t1.Type.Kind
					If (kind > SymbolKind.ErrorType) Then
						If (kind = SymbolKind.NamedType) Then
							GoTo Label0
						End If
						If (kind <> SymbolKind.TypeParameter) Then
							GoTo Label6
						End If
						If (t2.Type.SpecialType <> SpecialType.System_Void) Then
							Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol = DirectCast(t1.Type, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol)
							If (Not TypeUnification.Contains(t2.Type, type)) Then
								customModifiers = t1.CustomModifiers
								If (customModifiers.IsDefaultOrEmpty) Then
									TypeUnification.AddSubstitution(substitution, containingGenericType, type, t2)
									flag = True
									Return flag
								ElseIf (Not System.Linq.ImmutableArrayExtensions.SequenceEqual(Of CustomModifier, CustomModifier)(t1.CustomModifiers, t2.CustomModifiers, DirectCast(Nothing, IEqualityComparer(Of CustomModifier)))) Then
									customModifiers = t1.CustomModifiers
									Dim length As Integer = customModifiers.Length
									customModifiers = t2.CustomModifiers
									If (length < customModifiers.Length) Then
										Dim customModifiers1 As ImmutableArray(Of CustomModifier) = t1.CustomModifiers
										Dim customModifiers2 As IEnumerable(Of CustomModifier) = DirectCast(t2.CustomModifiers, IEnumerable(Of CustomModifier))
										customModifiers = t1.CustomModifiers
										If (Not customModifiers1.SequenceEqual(Of CustomModifier)(customModifiers2.Take(customModifiers.Length), Nothing)) Then
											GoTo Label3
										End If
										Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = t2.Type
										Dim customModifiers3 As ImmutableArray(Of CustomModifier) = t2.CustomModifiers
										customModifiers = t1.CustomModifiers
										Dim num As Integer = customModifiers.Length
										customModifiers = t2.CustomModifiers
										Dim length1 As Integer = customModifiers.Length
										customModifiers = t1.CustomModifiers
										TypeUnification.AddSubstitution(substitution, containingGenericType, type, New TypeWithModifiers(typeSymbol, ImmutableArray.Create(Of CustomModifier)(customModifiers3, num, length1 - customModifiers.Length)))
										flag = True
										Return flag
									End If
								Label3:
									If (t2.Type.IsTypeParameter()) Then
										Dim typeParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol = DirectCast(t2.Type, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol)
										customModifiers = t2.CustomModifiers
										If (Not customModifiers.IsDefaultOrEmpty) Then
											customModifiers = t2.CustomModifiers
											Dim num1 As Integer = customModifiers.Length
											customModifiers = t1.CustomModifiers
											If (num1 < customModifiers.Length) Then
												Dim customModifiers4 As ImmutableArray(Of CustomModifier) = t2.CustomModifiers
												Dim customModifiers5 As IEnumerable(Of CustomModifier) = DirectCast(t1.CustomModifiers, IEnumerable(Of CustomModifier))
												customModifiers = t2.CustomModifiers
												If (Not customModifiers4.SequenceEqual(Of CustomModifier)(customModifiers5.Take(customModifiers.Length), Nothing)) Then
													flag = False
													Return flag
												End If
												Dim type1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = t1.Type
												Dim customModifiers6 As ImmutableArray(Of CustomModifier) = t1.CustomModifiers
												customModifiers = t2.CustomModifiers
												Dim length2 As Integer = customModifiers.Length
												customModifiers = t1.CustomModifiers
												Dim num2 As Integer = customModifiers.Length
												customModifiers = t2.CustomModifiers
												TypeUnification.AddSubstitution(substitution, containingGenericType, typeParameterSymbol, New TypeWithModifiers(type1, ImmutableArray.Create(Of CustomModifier)(customModifiers6, length2, num2 - customModifiers.Length)))
												flag = True
												Return flag
											End If
										Else
											TypeUnification.AddSubstitution(substitution, containingGenericType, typeParameterSymbol, t1)
											flag = True
											Return flag
										End If
									End If
									flag = False
									Return flag
								Else
									TypeUnification.AddSubstitution(substitution, containingGenericType, type, New TypeWithModifiers(t2.Type))
									flag = True
									Return flag
								End If
							Else
								flag = False
								Return flag
							End If
						Else
							flag = False
							Return flag
						End If
					Else
						If (kind <> SymbolKind.ArrayType) Then
							GoTo Label5
						End If
						If (t2.Type.TypeKind <> t1.Type.TypeKind OrElse Not System.Linq.ImmutableArrayExtensions.SequenceEqual(Of CustomModifier, CustomModifier)(t1.CustomModifiers, t2.CustomModifiers, DirectCast(Nothing, IEqualityComparer(Of CustomModifier)))) Then
							flag = False
							Return flag
						Else
							Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(t1.Type, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
							Dim arrayTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(t2.Type, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
							If (arrayTypeSymbol.HasSameShapeAs(arrayTypeSymbol1)) Then
								flag = TypeUnification.CanUnifyHelper(containingGenericType, New TypeWithModifiers(arrayTypeSymbol.ElementType, arrayTypeSymbol.CustomModifiers), New TypeWithModifiers(arrayTypeSymbol1.ElementType, arrayTypeSymbol1.CustomModifiers), substitution)
								Return flag
							Else
								flag = False
								Return flag
							End If
						End If
					End If
				Label6:
					flag = t1 = t2
				Else
					flag = True
				End If
			End If
			Return flag
		Label0:
			If (t2.Type.TypeKind <> t1.Type.TypeKind OrElse Not System.Linq.ImmutableArrayExtensions.SequenceEqual(Of CustomModifier, CustomModifier)(t1.CustomModifiers, t2.CustomModifiers, DirectCast(Nothing, IEqualityComparer(Of CustomModifier)))) Then
				flag = False
				Return flag
			Else
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(t1.Type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(t2.Type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				If (namedTypeSymbol.IsTupleType OrElse namedTypeSymbol1.IsTupleType) Then
					flag = TypeUnification.CanUnifyHelper(containingGenericType, New TypeWithModifiers(namedTypeSymbol.GetTupleUnderlyingTypeOrSelf()), New TypeWithModifiers(namedTypeSymbol1.GetTupleUnderlyingTypeOrSelf()), substitution)
					Return flag
				ElseIf (Not namedTypeSymbol.IsGenericType) Then
					flag = If(namedTypeSymbol1.IsGenericType, False, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(namedTypeSymbol, namedTypeSymbol1, TypeCompareKind.ConsiderEverything))
					Return flag
				ElseIf (namedTypeSymbol1.IsGenericType) Then
					Dim arity As Integer = namedTypeSymbol.Arity
					If (namedTypeSymbol1.Arity <> arity OrElse Not Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(namedTypeSymbol1.OriginalDefinition, namedTypeSymbol.OriginalDefinition, TypeCompareKind.ConsiderEverything)) Then
						flag = False
						Return flag
					Else
						Dim typeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) = namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics
						Dim typeSymbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) = namedTypeSymbol1.TypeArgumentsNoUseSiteDiagnostics
						Dim hasTypeArgumentsCustomModifiers As Boolean = namedTypeSymbol.HasTypeArgumentsCustomModifiers
						Dim hasTypeArgumentsCustomModifiers1 As Boolean = namedTypeSymbol1.HasTypeArgumentsCustomModifiers
						Dim num3 As Integer = arity - 1
						Dim num4 As Integer = 0
						While num4 <= num3
							Dim namedTypeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = containingGenericType
							Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = typeArgumentsNoUseSiteDiagnostics(num4)
							If (hasTypeArgumentsCustomModifiers) Then
								typeArgumentCustomModifiers = namedTypeSymbol.GetTypeArgumentCustomModifiers(num4)
							Else
								customModifiers = New ImmutableArray(Of CustomModifier)()
								typeArgumentCustomModifiers = customModifiers
							End If
							Dim typeWithModifier1 As TypeWithModifiers = New TypeWithModifiers(item, typeArgumentCustomModifiers)
							Dim item1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = typeSymbols(num4)
							If (hasTypeArgumentsCustomModifiers1) Then
								typeArgumentCustomModifiers1 = namedTypeSymbol1.GetTypeArgumentCustomModifiers(num4)
							Else
								customModifiers = New ImmutableArray(Of CustomModifier)()
								typeArgumentCustomModifiers1 = customModifiers
							End If
							If (TypeUnification.CanUnifyHelper(namedTypeSymbol2, typeWithModifier1, New TypeWithModifiers(item1, typeArgumentCustomModifiers1), substitution)) Then
								num4 = num4 + 1
							Else
								flag = False
								Return flag
							End If
						End While
						flag = If(namedTypeSymbol.ContainingType Is Nothing, True, TypeUnification.CanUnifyHelper(containingGenericType, New TypeWithModifiers(namedTypeSymbol.ContainingType), New TypeWithModifiers(namedTypeSymbol1.ContainingType), substitution))
						Return flag
					End If
				Else
					flag = False
					Return flag
				End If
			End If
		Label5:
			If (kind = SymbolKind.ErrorType) Then
				GoTo Label0
			End If
			GoTo Label6
		End Function

		Private Shared Function Contains(ByVal type As TypeSymbol, ByVal typeParam As TypeParameterSymbol) As Boolean
			Dim flag As Boolean
			Dim containingType As NamedTypeSymbol
			Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator
			Dim typeSymbols As ImmutableArray(Of TypeSymbol)
			Dim kind As SymbolKind = type.Kind
			If (kind > SymbolKind.ErrorType) Then
				If (kind = SymbolKind.NamedType) Then
					containingType = DirectCast(type, NamedTypeSymbol)
					While containingType IsNot Nothing
						typeSymbols = If(containingType.IsTupleType, containingType.TupleElementTypes, containingType.TypeArgumentsNoUseSiteDiagnostics)
						enumerator = typeSymbols.GetEnumerator()
						While enumerator.MoveNext()
							If (Not TypeUnification.Contains(enumerator.Current, typeParam)) Then
								Continue While
							End If
							flag = True
							Return flag
						End While
						containingType = containingType.ContainingType
					End While
					flag = False
					Return flag
				End If
				If (kind <> SymbolKind.TypeParameter) Then
					flag = False
					Return flag
				End If
				flag = TypeSymbol.Equals(type, typeParam, TypeCompareKind.ConsiderEverything)
				Return flag
			Else
				If (kind <> SymbolKind.ArrayType) Then
					GoTo Label3
				End If
				flag = TypeUnification.Contains(DirectCast(type, ArrayTypeSymbol).ElementType, typeParam)
				Return flag
			End If
			flag = False
			Return flag
		Label3:
			If (kind = SymbolKind.ErrorType) Then
				containingType = DirectCast(type, NamedTypeSymbol)
				While containingType IsNot Nothing
					typeSymbols = If(containingType.IsTupleType, containingType.TupleElementTypes, containingType.TypeArgumentsNoUseSiteDiagnostics)
					enumerator = typeSymbols.GetEnumerator()
					While enumerator.MoveNext()
						If (Not TypeUnification.Contains(enumerator.Current, typeParam)) Then
							Continue While
						End If
						flag = True
						Return flag
					End While
					containingType = containingType.ContainingType
				End While
				flag = False
				Return flag
			End If
			flag = False
			Return flag
		End Function
	End Class
End Namespace