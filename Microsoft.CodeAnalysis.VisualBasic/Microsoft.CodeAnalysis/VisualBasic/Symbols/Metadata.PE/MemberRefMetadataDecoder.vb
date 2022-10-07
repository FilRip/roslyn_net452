Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections.Immutable
Imports System.Reflection.Metadata

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
	Friend NotInheritable Class MemberRefMetadataDecoder
		Inherits MetadataDecoder
		Private ReadOnly _containingType As TypeSymbol

		Public Sub New(ByVal moduleSymbol As PEModuleSymbol, ByVal containingType As TypeSymbol)
			MyBase.New(moduleSymbol, TryCast(containingType, PENamedTypeSymbol))
			Me._containingType = containingType
		End Sub

		Private Shared Function CustomModifiersMatch(ByVal candidateReturnTypeCustomModifiers As ImmutableArray(Of Microsoft.CodeAnalysis.CustomModifier), ByVal targetReturnTypeCustomModifiers As ImmutableArray(Of ModifierInfo(Of TypeSymbol))) As Boolean
			Dim flag As Boolean
			If (targetReturnTypeCustomModifiers.IsDefault OrElse targetReturnTypeCustomModifiers.IsEmpty) Then
				flag = If(candidateReturnTypeCustomModifiers.IsDefault, True, candidateReturnTypeCustomModifiers.IsEmpty)
			ElseIf (Not candidateReturnTypeCustomModifiers.IsDefault) Then
				Dim length As Integer = candidateReturnTypeCustomModifiers.Length
				If (targetReturnTypeCustomModifiers.Length = length) Then
					Dim num As Integer = length - 1
					Dim num1 As Integer = 0
					While num1 <= num
						Dim item As ModifierInfo(Of TypeSymbol) = targetReturnTypeCustomModifiers(num1)
						Dim customModifier As Microsoft.CodeAnalysis.CustomModifier = candidateReturnTypeCustomModifiers(num1)
						If (item.IsOptional <> customModifier.IsOptional OrElse Not [Object].Equals(item.Modifier, customModifier.Modifier)) Then
							flag = False
							Return flag
						Else
							num1 = num1 + 1
						End If
					End While
					flag = True
				Else
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Shared Function FindFieldBySignature(ByVal targetTypeSymbol As TypeSymbol, ByVal targetMemberName As String, ByVal customModifiers As ImmutableArray(Of ModifierInfo(Of TypeSymbol)), ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = targetTypeSymbol.GetMembers(targetMemberName).GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = TryCast(enumerator.Current, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
					If (current IsNot Nothing AndAlso TypeSymbol.Equals(current.Type, type, TypeCompareKind.AllIgnoreOptionsForVB) AndAlso MemberRefMetadataDecoder.CustomModifiersMatch(current.CustomModifiers, customModifiers)) Then
						fieldSymbol = current
						Exit While
					End If
				Else
					fieldSymbol = Nothing
					Exit While
				End If
			End While
			Return fieldSymbol
		End Function

		Friend Function FindMember(ByVal targetTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal memberRef As MemberReferenceHandle, ByVal methodsOnly As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim signatureHeader As System.Reflection.Metadata.SignatureHeader
			Dim num As Integer
			If (targetTypeSymbol IsNot Nothing) Then
				Try
					Dim memberRefNameOrThrow As String = Me.[Module].GetMemberRefNameOrThrow(memberRef)
					Dim signatureOrThrow As BlobHandle = Me.[Module].GetSignatureOrThrow(memberRef)
					Dim blobReader As System.Reflection.Metadata.BlobReader = MyBase.DecodeSignatureHeaderOrThrow(signatureOrThrow, signatureHeader)
					Dim rawValue As Byte = signatureHeader.RawValue And 15
					If (rawValue = 0 OrElse rawValue = 5) Then
						Dim paramInfoArray As ParamInfo(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)() = MyBase.DecodeSignatureParametersOrThrow(blobReader, signatureHeader, num, True, False)
						symbol = MemberRefMetadataDecoder.FindMethodBySignature(targetTypeSymbol, memberRefNameOrThrow, signatureHeader, num, paramInfoArray)
					ElseIf (rawValue <> 6) Then
						symbol = Nothing
					ElseIf (Not methodsOnly) Then
						Dim modifierInfos As ImmutableArray(Of ModifierInfo(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)) = New ImmutableArray(Of ModifierInfo(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol))()
						Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = MyBase.DecodeFieldSignature(blobReader, modifierInfos)
						symbol = MemberRefMetadataDecoder.FindFieldBySignature(targetTypeSymbol, memberRefNameOrThrow, modifierInfos, typeSymbol)
					Else
						symbol = Nothing
					End If
				Catch badImageFormatException As System.BadImageFormatException
					ProjectData.SetProjectError(badImageFormatException)
					symbol = Nothing
					ProjectData.ClearProjectError()
				End Try
			Else
				symbol = Nothing
			End If
			Return symbol
		End Function

		Private Shared Function FindMethodBySignature(ByVal targetTypeSymbol As TypeSymbol, ByVal targetMemberName As String, ByVal targetMemberSignatureHeader As SignatureHeader, ByVal targetMemberTypeParamCount As Integer, ByVal targetParamInfo As ParamInfo(Of TypeSymbol)()) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = targetTypeSymbol.GetMembers(targetMemberName).GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = TryCast(enumerator.Current, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
					If (current IsNot Nothing AndAlso CByte(current.CallingConvention) = targetMemberSignatureHeader.RawValue AndAlso targetMemberTypeParamCount = current.Arity AndAlso MemberRefMetadataDecoder.MethodSymbolMatchesParamInfo(current, targetParamInfo)) Then
						methodSymbol = current
						Exit While
					End If
				Else
					methodSymbol = Nothing
					Exit While
				End If
			End While
			Return methodSymbol
		End Function

		Protected Overrides Function GetGenericMethodTypeParamSymbol(ByVal position As Integer) As TypeSymbol
			Return IndexedTypeParameterSymbol.GetTypeParameter(position)
		End Function

		Private Shared Sub GetGenericTypeArgumentSymbol(ByVal position As Integer, ByVal namedType As NamedTypeSymbol, ByRef cumulativeArity As Integer, ByRef typeArgument As TypeSymbol)
			Dim num As Integer = 0
			cumulativeArity = namedType.Arity
			typeArgument = Nothing
			Dim num1 As Integer = 0
			Dim containingType As NamedTypeSymbol = namedType.ContainingType
			If (containingType IsNot Nothing) Then
				MemberRefMetadataDecoder.GetGenericTypeArgumentSymbol(position, containingType, num, typeArgument)
				cumulativeArity += num
				num1 = num
			End If
			If (num1 <= position AndAlso position < cumulativeArity) Then
				typeArgument = namedType.TypeArgumentsNoUseSiteDiagnostics(position - num1)
			End If
		End Sub

		Protected Overrides Function GetGenericTypeParamSymbol(ByVal position As Integer) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim unsupportedMetadataTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim num As Integer = 0
			Dim containingSymbol As PENamedTypeSymbol = TryCast(Me._containingType, PENamedTypeSymbol)
			If (containingSymbol Is Nothing) Then
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(Me._containingType, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				If (namedTypeSymbol Is Nothing) Then
					unsupportedMetadataTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.UnsupportedMetadataTypeSymbol(VBResources.AssociatedTypeDoesNotHaveTypeParameters)
				Else
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
					MemberRefMetadataDecoder.GetGenericTypeArgumentSymbol(position, namedTypeSymbol, num, typeSymbol)
					If (typeSymbol Is Nothing) Then
						unsupportedMetadataTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.UnsupportedMetadataTypeSymbol(VBResources.PositionOfTypeParameterTooLarge)
					Else
						unsupportedMetadataTypeSymbol = typeSymbol
					End If
				End If
			Else
				While containingSymbol IsNot Nothing AndAlso containingSymbol.MetadataArity - containingSymbol.Arity > position
					containingSymbol = TryCast(containingSymbol.ContainingSymbol, PENamedTypeSymbol)
				End While
				If (containingSymbol Is Nothing OrElse containingSymbol.MetadataArity <= position) Then
					unsupportedMetadataTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.UnsupportedMetadataTypeSymbol(VBResources.PositionOfTypeParameterTooLarge)
				Else
					position = position - (containingSymbol.MetadataArity - containingSymbol.Arity)
					unsupportedMetadataTypeSymbol = containingSymbol.TypeArgumentsNoUseSiteDiagnostics(position)
				End If
			End If
			Return unsupportedMetadataTypeSymbol
		End Function

		Private Shared Function MethodSymbolMatchesParamInfo(ByVal candidateMethod As MethodSymbol, ByVal targetParamInfo As ParamInfo(Of TypeSymbol)()) As Boolean
			Dim flag As Boolean
			Dim length As Integer = CInt(targetParamInfo.Length) - 1
			If (candidateMethod.ParameterCount = length) Then
				If (candidateMethod.Arity > 0) Then
					candidateMethod = candidateMethod.Construct(StaticCast(Of TypeSymbol).From(Of TypeParameterSymbol)(IndexedTypeParameterSymbol.Take(candidateMethod.Arity)))
				End If
				If (MemberRefMetadataDecoder.ReturnTypesMatch(candidateMethod, targetParamInfo(0))) Then
					Dim num As Integer = length - 1
					Dim num1 As Integer = 0
					While num1 <= num
						If (MemberRefMetadataDecoder.ParametersMatch(candidateMethod.Parameters(num1), targetParamInfo(num1 + 1))) Then
							num1 = num1 + 1
						Else
							flag = False
							Return flag
						End If
					End While
					flag = True
				Else
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Shared Function ParametersMatch(ByVal candidateParam As ParameterSymbol, ByRef targetParam As ParamInfo(Of TypeSymbol)) As Boolean
			Dim flag As Boolean
			If (candidateParam.IsByRef <> targetParam.IsByRef) Then
				flag = False
			ElseIf (TypeSymbol.Equals(candidateParam.Type, targetParam.Type, TypeCompareKind.AllIgnoreOptionsForVB)) Then
				flag = If(Not MemberRefMetadataDecoder.CustomModifiersMatch(candidateParam.CustomModifiers, targetParam.CustomModifiers) OrElse Not MemberRefMetadataDecoder.CustomModifiersMatch(candidateParam.RefCustomModifiers, targetParam.RefCustomModifiers), False, True)
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Shared Function ReturnTypesMatch(ByVal candidateMethod As MethodSymbol, ByRef targetReturnParam As ParamInfo(Of TypeSymbol)) As Boolean
			Dim flag As Boolean
			If (Not TypeSymbol.Equals(candidateMethod.ReturnType, targetReturnParam.Type, TypeCompareKind.AllIgnoreOptionsForVB) OrElse candidateMethod.ReturnsByRef <> targetReturnParam.IsByRef) Then
				flag = False
			Else
				flag = If(Not MemberRefMetadataDecoder.CustomModifiersMatch(candidateMethod.ReturnTypeCustomModifiers, targetReturnParam.CustomModifiers) OrElse Not MemberRefMetadataDecoder.CustomModifiersMatch(candidateMethod.RefCustomModifiers, targetReturnParam.RefCustomModifiers), False, True)
			End If
			Return flag
		End Function
	End Class
End Namespace