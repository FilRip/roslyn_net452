Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Reflection.Metadata
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
	Friend Structure TupleTypeDecoder
		Private ReadOnly _elementNames As ImmutableArray(Of String)

		Private _namesIndex As Integer

		Private _foundUsableErrorType As Boolean

		Private _decodingFailed As Boolean

		Private Sub New(ByVal elementNames As ImmutableArray(Of String))
			Me = New TupleTypeDecoder() With
			{
				._elementNames = elementNames,
				._namesIndex = If(elementNames.IsDefault, 0, elementNames.Length),
				._foundUsableErrorType = False,
				._decodingFailed = False
			}
		End Sub

		Private Shared Function Construct(ByVal type As NamedTypeSymbol, ByVal newTypeArgs As ImmutableArray(Of TypeWithModifiers)) As NamedTypeSymbol
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Dim typeSubstitution1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution
			Dim originalDefinition As NamedTypeSymbol = type.OriginalDefinition
			Dim containingType As NamedTypeSymbol = type.ConstructedFrom.ContainingType
			If (containingType IsNot Nothing) Then
				typeSubstitution1 = containingType.TypeSubstitution
			Else
				typeSubstitution1 = Nothing
			End If
			Dim typeSubstitution2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = typeSubstitution1
			typeSubstitution = If(typeSubstitution2 Is Nothing, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(originalDefinition, originalDefinition.TypeParameters, newTypeArgs, False), Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(typeSubstitution2, originalDefinition, newTypeArgs, False))
			Return originalDefinition.Construct(typeSubstitution)
		End Function

		Private Function DecodeArrayType(ByVal type As ArrayTypeSymbol) As ArrayTypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.DecodeType(type.ElementType)
			If (CObj(typeSymbol) = CObj(type.ElementType)) Then
				Return type
			End If
			Return type.WithElementType(typeSymbol)
		End Function

		Private Function DecodeNamedType(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim num As Integer
			Dim typeWithModifier As Func(Of TypeSymbol, Integer, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers)
			Dim typeWithModifiers As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers)
			Dim func As Func(Of TypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers)
			Dim typeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol) = type.TypeArgumentsNoUseSiteDiagnostics
			Dim typeSymbols As ImmutableArray(Of TypeSymbol) = Me.DecodeTypeArguments(typeArgumentsNoUseSiteDiagnostics)
			Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = type
			Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = type.ContainingType
			Dim namedTypeSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
			namedTypeSymbol2 = If(containingType Is Nothing OrElse Not containingType.IsGenericType, containingType, Me.DecodeNamedType(containingType))
			Dim flag As Boolean = CObj(namedTypeSymbol2) <> CObj(containingType)
			If (typeArgumentsNoUseSiteDiagnostics <> typeSymbols OrElse flag) Then
				If (type.HasTypeArgumentsCustomModifiers) Then
					Dim typeSymbols1 As ImmutableArray(Of TypeSymbol) = typeSymbols
					If (TupleTypeDecoder._Closure$__.$I10-0 Is Nothing) Then
						typeWithModifier = Function(t As TypeSymbol, i As Integer, m As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) New Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers(t, m.GetTypeArgumentCustomModifiers(i))
						TupleTypeDecoder._Closure$__.$I10-0 = typeWithModifier
					Else
						typeWithModifier = TupleTypeDecoder._Closure$__.$I10-0
					End If
					typeWithModifiers = Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of TypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers)(typeSymbols1, typeWithModifier, type)
				Else
					Dim typeSymbols2 As ImmutableArray(Of TypeSymbol) = typeSymbols
					If (TupleTypeDecoder._Closure$__.$I10-1 Is Nothing) Then
						func = Function(t As TypeSymbol) New Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers(t, New ImmutableArray(Of CustomModifier)())
						TupleTypeDecoder._Closure$__.$I10-1 = func
					Else
						func = TupleTypeDecoder._Closure$__.$I10-1
					End If
					typeWithModifiers = Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of TypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers)(typeSymbols2, func)
				End If
				Dim typeWithModifiers1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeWithModifiers) = typeWithModifiers
				If (flag) Then
					namedTypeSymbol1 = namedTypeSymbol1.OriginalDefinition.AsMember(namedTypeSymbol2)
					namedTypeSymbol = If(namedTypeSymbol1.TypeParameters.IsEmpty, namedTypeSymbol1, TupleTypeDecoder.Construct(namedTypeSymbol1, typeWithModifiers1))
					Return namedTypeSymbol
				End If
				namedTypeSymbol1 = TupleTypeDecoder.Construct(type, typeWithModifiers1)
			End If
			If (namedTypeSymbol1.IsTupleCompatible(num)) Then
				namedTypeSymbol1 = TupleTypeSymbol.Create(namedTypeSymbol1, Me.EatElementNamesIfAvailable(num))
			End If
			namedTypeSymbol = namedTypeSymbol1
			Return namedTypeSymbol
		End Function

		Public Shared Function DecodeTupleTypesIfApplicable(ByVal metadataType As TypeSymbol, ByVal targetSymbolToken As EntityHandle, ByVal containingModule As PEModuleSymbol) As TypeSymbol
			Dim unsupportedMetadataTypeSymbol As TypeSymbol
			Dim strs As ImmutableArray(Of String) = New ImmutableArray(Of String)()
			Dim flag As Boolean = containingModule.[Module].HasTupleElementNamesAttribute(targetSymbolToken, strs)
			If (Not flag OrElse Not strs.IsDefaultOrEmpty) Then
				unsupportedMetadataTypeSymbol = TupleTypeDecoder.DecodeTupleTypesInternal(metadataType, strs, flag)
			Else
				unsupportedMetadataTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.UnsupportedMetadataTypeSymbol(DirectCast(Nothing, BadImageFormatException))
			End If
			Return unsupportedMetadataTypeSymbol
		End Function

		Public Shared Function DecodeTupleTypesIfApplicable(ByVal metadataType As TypeSymbol, ByVal elementNames As ImmutableArray(Of String)) As TypeSymbol
			Return TupleTypeDecoder.DecodeTupleTypesInternal(metadataType, elementNames, Not elementNames.IsDefaultOrEmpty)
		End Function

		Private Shared Function DecodeTupleTypesInternal(ByVal metadataType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal elementNames As ImmutableArray(Of String), ByVal hasTupleElementNamesAttribute As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim unsupportedMetadataTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim tupleTypeDecoder As Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.TupleTypeDecoder = New Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE.TupleTypeDecoder(elementNames)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = tupleTypeDecoder.DecodeType(metadataType)
			If (Not tupleTypeDecoder._decodingFailed AndAlso (Not hasTupleElementNamesAttribute OrElse tupleTypeDecoder._namesIndex = 0)) Then
				unsupportedMetadataTypeSymbol = typeSymbol
			ElseIf (Not tupleTypeDecoder._foundUsableErrorType) Then
				unsupportedMetadataTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.UnsupportedMetadataTypeSymbol(DirectCast(Nothing, BadImageFormatException))
			Else
				unsupportedMetadataTypeSymbol = metadataType
			End If
			Return unsupportedMetadataTypeSymbol
		End Function

		Private Function DecodeType(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim kind As SymbolKind = type.Kind
			If (kind <= SymbolKind.NamedType) Then
				Select Case kind
					Case SymbolKind.ArrayType
						typeSymbol = Me.DecodeArrayType(DirectCast(type, ArrayTypeSymbol))
						Return typeSymbol
					Case SymbolKind.Assembly
						Exit Select
					Case SymbolKind.DynamicType
						typeSymbol = type
						Return typeSymbol
					Case SymbolKind.ErrorType
						Me._foundUsableErrorType = True
						typeSymbol = type
						Return typeSymbol
					Case Else
						If (kind = SymbolKind.NamedType) Then
							typeSymbol = If(type.IsTupleType, Me.DecodeNamedType(type.TupleUnderlyingType), Me.DecodeNamedType(DirectCast(type, NamedTypeSymbol)))
							Return typeSymbol
						Else
							Exit Select
						End If
				End Select
			ElseIf (kind = SymbolKind.PointerType OrElse kind = SymbolKind.TypeParameter) Then
				typeSymbol = type
				Return typeSymbol
			End If
			Throw ExceptionUtilities.UnexpectedValue(type.TypeKind)
		End Function

		Private Function DecodeTypeArguments(ByVal typeArgs As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)) As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
			Dim immutableAndFree As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
			If (Not typeArgs.IsEmpty) Then
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol).GetInstance(typeArgs.Length)
				Dim flag As Boolean = False
				For i As Integer = typeArgs.Length - 1 To 0 Step -1
					Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = typeArgs(i)
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.DecodeType(item)
					flag = flag Or CObj(typeSymbol) <> CObj(item)
					instance.Add(typeSymbol)
				Next

				If (flag) Then
					instance.ReverseContents()
					immutableAndFree = instance.ToImmutableAndFree()
				Else
					instance.Free()
					immutableAndFree = typeArgs
				End If
			Else
				immutableAndFree = typeArgs
			End If
			Return immutableAndFree
		End Function

		Private Function EatElementNamesIfAvailable(ByVal numberOfElements As Integer) As ImmutableArray(Of String)
			Dim immutableAndFree As ImmutableArray(Of String)
			If (Me._elementNames.IsDefault) Then
				immutableAndFree = Me._elementNames
			ElseIf (numberOfElements <= Me._namesIndex) Then
				Dim num As Integer = Me._namesIndex - numberOfElements
				Dim flag As Boolean = True
				Me._namesIndex = num
				Dim num1 As Integer = numberOfElements - 1
				Dim num2 As Integer = 0
				While num2 <= num1
					If (Me._elementNames(num + num2) Is Nothing) Then
						num2 = num2 + 1
					Else
						flag = False
						Exit While
					End If
				End While
				If (Not flag) Then
					Dim instance As ArrayBuilder(Of String) = ArrayBuilder(Of String).GetInstance(numberOfElements)
					Dim num3 As Integer = numberOfElements - 1
					Dim num4 As Integer = 0
					Do
						instance.Add(Me._elementNames(num + num4))
						num4 = num4 + 1
					Loop While num4 <= num3
					immutableAndFree = instance.ToImmutableAndFree()
				Else
					immutableAndFree = New ImmutableArray(Of String)()
				End If
			Else
				Me._namesIndex = 0
				Me._decodingFailed = True
				immutableAndFree = New ImmutableArray(Of String)()
			End If
			Return immutableAndFree
		End Function
	End Structure
End Namespace