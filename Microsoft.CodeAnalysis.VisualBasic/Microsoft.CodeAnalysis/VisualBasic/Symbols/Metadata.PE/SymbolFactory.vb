Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
	Friend NotInheritable Class SymbolFactory
		Inherits SymbolFactory(Of PEModuleSymbol, TypeSymbol)
		Friend ReadOnly Shared Instance As SymbolFactory

		Shared Sub New()
			SymbolFactory.Instance = New SymbolFactory()
		End Sub

		Public Sub New()
			MyBase.New()
		End Sub

		Friend Overrides Function GetEnumUnderlyingType(ByVal moduleSymbol As PEModuleSymbol, ByVal type As TypeSymbol) As TypeSymbol
			Return type.GetEnumUnderlyingType()
		End Function

		Friend Overrides Function GetMDArrayTypeSymbol(ByVal moduleSymbol As PEModuleSymbol, ByVal rank As Integer, ByVal elementType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal customModifiers As ImmutableArray(Of ModifierInfo(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)), ByVal sizes As ImmutableArray(Of Integer), ByVal lowerBounds As ImmutableArray(Of Integer)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			If (Not TypeOf elementType Is UnsupportedMetadataTypeSymbol) Then
				typeSymbol = ArrayTypeSymbol.CreateMDArray(elementType, VisualBasicCustomModifier.Convert(customModifiers), rank, sizes, lowerBounds, moduleSymbol.ContainingAssembly)
			Else
				typeSymbol = elementType
			End If
			Return typeSymbol
		End Function

		Friend Overrides Function GetPrimitiveTypeCode(ByVal moduleSymbol As PEModuleSymbol, ByVal type As TypeSymbol) As PrimitiveTypeCode
			Return type.PrimitiveTypeCode
		End Function

		Friend Overrides Function GetSpecialType(ByVal moduleSymbol As PEModuleSymbol, ByVal specialType As Microsoft.CodeAnalysis.SpecialType) As TypeSymbol
			Return moduleSymbol.ContainingAssembly.GetSpecialType(specialType)
		End Function

		Friend Overrides Function GetSystemTypeSymbol(ByVal moduleSymbol As PEModuleSymbol) As TypeSymbol
			Return moduleSymbol.SystemTypeSymbol
		End Function

		Friend Overrides Function GetSZArrayTypeSymbol(ByVal moduleSymbol As PEModuleSymbol, ByVal elementType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal customModifiers As ImmutableArray(Of ModifierInfo(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol))) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			If (Not TypeOf elementType Is UnsupportedMetadataTypeSymbol) Then
				typeSymbol = ArrayTypeSymbol.CreateSZArray(elementType, VisualBasicCustomModifier.Convert(customModifiers), moduleSymbol.ContainingAssembly)
			Else
				typeSymbol = elementType
			End If
			Return typeSymbol
		End Function

		Friend Overrides Function GetUnsupportedMetadataTypeSymbol(ByVal moduleSymbol As PEModuleSymbol, ByVal exception As BadImageFormatException) As TypeSymbol
			Return New UnsupportedMetadataTypeSymbol(exception)
		End Function

		Friend Overrides Function MakeFunctionPointerTypeSymbol(ByVal callingConvention As Microsoft.Cci.CallingConvention, ByVal retAndParamTypes As ImmutableArray(Of ParamInfo(Of TypeSymbol))) As TypeSymbol
			Return New UnsupportedMetadataTypeSymbol(DirectCast(Nothing, BadImageFormatException))
		End Function

		Friend Overrides Function MakePointerTypeSymbol(ByVal moduleSymbol As PEModuleSymbol, ByVal type As TypeSymbol, ByVal customModifiers As ImmutableArray(Of ModifierInfo(Of TypeSymbol))) As TypeSymbol
			Return New PointerTypeSymbol(type, VisualBasicCustomModifier.Convert(customModifiers))
		End Function

		Friend Overrides Function MakeUnboundIfGeneric(ByVal moduleSymbol As PEModuleSymbol, ByVal type As TypeSymbol) As TypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			If (namedTypeSymbol Is Nothing OrElse Not namedTypeSymbol.IsGenericType) Then
				Return type
			End If
			Return UnboundGenericType.Create(namedTypeSymbol)
		End Function

		Friend Overrides Function SubstituteTypeParameters(ByVal moduleSymbol As PEModuleSymbol, ByVal genericTypeDef As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal arguments As ImmutableArray(Of KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ImmutableArray(Of ModifierInfo(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)))), ByVal refersToNoPiaLocalType As ImmutableArray(Of Boolean)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim unsupportedMetadataTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeWithModifier As Func(Of KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ImmutableArray(Of ModifierInfo(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol))), TypeWithModifiers)
			If (Not TypeOf genericTypeDef Is Microsoft.CodeAnalysis.VisualBasic.Symbols.UnsupportedMetadataTypeSymbol) Then
				Dim enumerator As ImmutableArray(Of KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ImmutableArray(Of ModifierInfo(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)))).Enumerator = arguments.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ImmutableArray(Of ModifierInfo(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol))) = enumerator.Current
					If (current.Key.Kind <> SymbolKind.ErrorType OrElse Not TypeOf current.Key Is Microsoft.CodeAnalysis.VisualBasic.Symbols.UnsupportedMetadataTypeSymbol) Then
						Continue While
					End If
					unsupportedMetadataTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.UnsupportedMetadataTypeSymbol(DirectCast(Nothing, BadImageFormatException))
					Return unsupportedMetadataTypeSymbol
				End While
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(genericTypeDef, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				Dim linkedReferencedAssemblies As ImmutableArray(Of AssemblySymbol) = moduleSymbol.ContainingAssembly.GetLinkedReferencedAssemblies()
				Dim flag As Boolean = False
				If (Not linkedReferencedAssemblies.IsDefaultOrEmpty OrElse moduleSymbol.[Module].ContainsNoPiaLocalTypes()) Then
					Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = namedTypeSymbol
					Dim length As Integer = refersToNoPiaLocalType.Length - 1
					Do
						If (Not containingType.IsInterface) Then
							Exit Do
						End If
						length -= containingType.Arity
						containingType = containingType.ContainingType
					Loop While containingType IsNot Nothing
					Dim num As Integer = length
					While num >= 0
						If (refersToNoPiaLocalType(num) OrElse Not linkedReferencedAssemblies.IsDefaultOrEmpty AndAlso MetadataDecoder.IsOrClosedOverATypeFromAssemblies(arguments(num).Key, linkedReferencedAssemblies)) Then
							flag = True
							Exit While
						Else
							num += -1
						End If
					End While
				End If
				Dim allTypeParameters As ImmutableArray(Of TypeParameterSymbol) = namedTypeSymbol.GetAllTypeParameters()
				If (allTypeParameters.Length = arguments.Length) Then
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = genericTypeDef
					Dim typeParameterSymbols As ImmutableArray(Of TypeParameterSymbol) = allTypeParameters
					Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ImmutableArray(Of ModifierInfo(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)))) = arguments
					If (SymbolFactory._Closure$__.$I11-0 Is Nothing) Then
						typeWithModifier = Function(pair As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ImmutableArray(Of ModifierInfo(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)))) New TypeWithModifiers(pair.Key, VisualBasicCustomModifier.Convert(pair.Value))
						SymbolFactory._Closure$__.$I11-0 = typeWithModifier
					Else
						typeWithModifier = SymbolFactory._Closure$__.$I11-0
					End If
					Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(typeSymbol, typeParameterSymbols, ImmutableArrayExtensions.SelectAsArray(Of KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ImmutableArray(Of ModifierInfo(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol))), TypeWithModifiers)(keyValuePairs, typeWithModifier), False)
					If (typeSubstitution IsNot Nothing) Then
						Dim noPiaIllegalGenericInstantiationSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = namedTypeSymbol.Construct(typeSubstitution)
						If (flag) Then
							noPiaIllegalGenericInstantiationSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.NoPiaIllegalGenericInstantiationSymbol(noPiaIllegalGenericInstantiationSymbol)
						End If
						unsupportedMetadataTypeSymbol = noPiaIllegalGenericInstantiationSymbol
					Else
						unsupportedMetadataTypeSymbol = genericTypeDef
					End If
				Else
					unsupportedMetadataTypeSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.UnsupportedMetadataTypeSymbol(DirectCast(Nothing, BadImageFormatException))
				End If
			Else
				unsupportedMetadataTypeSymbol = genericTypeDef
			End If
			Return unsupportedMetadataTypeSymbol
		End Function
	End Class
End Namespace