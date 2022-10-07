Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class Conversions
		Public ReadOnly Shared Identity As KeyValuePair(Of ConversionKind, MethodSymbol)

		Shared Sub New()
			Conversions.Identity = New KeyValuePair(Of ConversionKind, MethodSymbol)(ConversionKind.Identity, Nothing)
		End Sub

		Private Sub New()
			MyBase.New()
			Throw ExceptionUtilities.Unreachable
		End Sub

		Private Shared Function AddDelegateRelaxationInformationForADelegate(ByVal source As TypeSymbol, ByVal destination As TypeSymbol, ByVal convKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			If (Not source.IsDelegateType()) Then
				conversionKind = convKind
			Else
				convKind = convKind And (Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToNumericOverflow Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToIntegerOverflow Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToNumericOverflowMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToQueryLambdaBodyMismatch Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToArrayLiteralElementConversion Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Widening] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Narrowing] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Identity Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Numeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Nullable Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNullable Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNullable Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Reference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningReference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingReference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Array Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningArray Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingArray Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Value Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningValue Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingValue Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[String] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningString Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingString Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Boolean] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingBoolean Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNothingLiteral Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesNarrowingFromNumericConstant Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Lambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.VarianceConversionAmbiguity Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.MightSucceedAtRuntime Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.AnonymousDelegate Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NeedAStub Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.ConvertedToExpressionTree Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.UserDefined Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingDueToContraVarianceInDelegate Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InterpolatedString Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Tuple Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTuple Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTuple Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNullableTuple Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNullableTuple)
				If (Not Conversions.ConversionExists(convKind)) Then
					conversionKind = convKind Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelInvalid
				ElseIf (Not Conversions.IsWideningConversion(convKind)) Then
					conversionKind = convKind Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNarrowing
				ElseIf (Not Conversions.IsIdentityConversion(convKind)) Then
					conversionKind = If(Not destination.IsDelegateType() OrElse destination.IsStrictSupertypeOfConcreteDelegate(), convKind Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningToNonLambda, convKind Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWidening)
				Else
					conversionKind = convKind
				End If
			End If
			Return conversionKind
		End Function

		Private Shared Function ArrayElementBitSize(ByVal type As TypeSymbol) As Integer
			Dim num As Integer
			Select Case type.GetEnumUnderlyingTypeOrSelf().SpecialType
				Case SpecialType.System_Boolean
				Case SpecialType.System_SByte
				Case SpecialType.System_Byte
					num = 8
					Exit Select
				Case SpecialType.System_Char
				Label0:
					num = 0
					Exit Select
				Case SpecialType.System_Int16
				Case SpecialType.System_UInt16
					num = 16
					Exit Select
				Case SpecialType.System_Int32
				Case SpecialType.System_UInt32
					num = 32
					Exit Select
				Case SpecialType.System_Int64
				Case SpecialType.System_UInt64
					num = 64
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return num
		End Function

		Friend Shared Function Classify_Reference_Array_TypeParameterConversion(ByVal srcElem As TypeSymbol, ByVal dstElem As TypeSymbol, ByVal varianceCompatibilityClassificationDepth As Integer, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ConversionKind
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.ConversionKind Microsoft.CodeAnalysis.VisualBasic.Conversions::Classify_Reference_Array_TypeParameterConversion(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,System.Int32,Microsoft.CodeAnalysis.CompoundUseSiteInfo`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.ConversionKind Classify_Reference_Array_TypeParameterConversion(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,System.Int32,Microsoft.CodeAnalysis.CompoundUseSiteInfo<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Shared Function ClassifyAddressOfConversion(ByVal source As BoundAddressOfOperator, ByVal destination As TypeSymbol) As ConversionKind
			Return Binder.ClassifyAddressOfConversion(source, destination)
		End Function

		Private Shared Function ClassifyAnonymousDelegateConversion(ByVal source As TypeSymbol, ByVal destination As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			If (Not source.IsAnonymousType OrElse Not source.IsDelegateType() OrElse Not destination.IsDelegateType()) Then
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
			Else
				Dim delegateInvokeMethod As MethodSymbol = DirectCast(destination, NamedTypeSymbol).DelegateInvokeMethod
				If (delegateInvokeMethod Is Nothing OrElse delegateInvokeMethod.GetUseSiteInfo().DiagnosticInfo IsNot Nothing) Then
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
				Else
					Dim methodConversionKind As Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind = Conversions.ClassifyMethodConversionForLambdaOrAnonymousDelegate(delegateInvokeMethod, DirectCast(source, NamedTypeSymbol).DelegateInvokeMethod, useSiteInfo)
					If (Conversions.IsDelegateRelaxationSupportedFor(methodConversionKind)) Then
						Dim conversionKind1 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.DetermineDelegateRelaxationLevel(methodConversionKind)
						If (Conversions.IsStubRequiredForMethodConversion(methodConversionKind)) Then
							conversionKind1 = conversionKind1 Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NeedAStub
						End If
						If (Not Conversions.IsNarrowingMethodConversion(methodConversionKind, True)) Then
							conversionKind = 1048577 Or CInt(conversionKind1)
						Else
							conversionKind = 1048578 Or CInt(conversionKind1)
						End If
					Else
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
					End If
				End If
			End If
			Return conversionKind
		End Function

		Private Shared Function ClassifyArrayConversion(ByVal source As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal destination As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal varianceCompatibilityClassificationDepth As Integer, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			If (Not Conversions.IsArrayType(source) OrElse Not Conversions.IsArrayType(destination)) Then
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
			Else
				Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(source, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
				Dim arrayTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(destination, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
				If (arrayTypeSymbol.HasSameShapeAs(arrayTypeSymbol1)) Then
					Dim elementType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = arrayTypeSymbol.ElementType
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = arrayTypeSymbol1.ElementType
					conversionKind = If(elementType.Kind = SymbolKind.ErrorType OrElse typeSymbol.Kind = SymbolKind.ErrorType, Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone, Conversions.ClassifyArrayConversionBasedOnElementTypes(elementType, typeSymbol, varianceCompatibilityClassificationDepth, useSiteInfo))
				Else
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
				End If
			End If
			Return conversionKind
		End Function

		Private Shared Function ClassifyArrayConversionBasedOnElementTypes(ByVal srcElem As TypeSymbol, ByVal dstElem As TypeSymbol, ByVal varianceCompatibilityClassificationDepth As Integer, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ConversionKind
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.ConversionKind Microsoft.CodeAnalysis.VisualBasic.Conversions::ClassifyArrayConversionBasedOnElementTypes(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,System.Int32,Microsoft.CodeAnalysis.CompoundUseSiteInfo`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.ConversionKind ClassifyArrayConversionBasedOnElementTypes(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,System.Int32,Microsoft.CodeAnalysis.CompoundUseSiteInfo<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Shared Function ClassifyArrayElementConversion(ByVal srcElem As TypeSymbol, ByVal dstElem As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind1 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyIdentityConversion(srcElem, dstElem)
			conversionKind = If(Not Conversions.ConversionExists(conversionKind1), Conversions.ClassifyArrayConversionBasedOnElementTypes(srcElem, dstElem, 0, useSiteInfo), conversionKind1)
			Return conversionKind
		End Function

		Private Shared Function ClassifyArrayInitialization(ByVal source As BoundArrayInitialization, ByVal targetElementType As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ConversionKind
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.ConversionKind Microsoft.CodeAnalysis.VisualBasic.Conversions::ClassifyArrayInitialization(Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.CompoundUseSiteInfo`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.ConversionKind ClassifyArrayInitialization(Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.CompoundUseSiteInfo<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Shared Function ClassifyArrayLiteralConversion(ByVal source As BoundArrayLiteral, ByVal destination As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim inferredType As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = source.InferredType
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(destination, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			If (namedTypeSymbol IsNot Nothing) Then
				originalDefinition = namedTypeSymbol.OriginalDefinition
			Else
				originalDefinition = Nothing
			End If
			Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = originalDefinition
			Dim item As TypeSymbol = Nothing
			Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = TryCast(destination, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
			If (arrayTypeSymbol Is Nothing OrElse inferredType.Rank <> arrayTypeSymbol.Rank AndAlso Not source.IsEmptyArrayLiteral) Then
				If (inferredType.Rank <> 1 AndAlso Not source.IsEmptyArrayLiteral OrElse namedTypeSymbol1 Is Nothing OrElse namedTypeSymbol1.SpecialType <> SpecialType.System_Collections_Generic_IEnumerable_T AndAlso namedTypeSymbol1.SpecialType <> SpecialType.System_Collections_Generic_IList_T AndAlso namedTypeSymbol1.SpecialType <> SpecialType.System_Collections_Generic_ICollection_T AndAlso namedTypeSymbol1.SpecialType <> SpecialType.System_Collections_Generic_IReadOnlyList_T AndAlso namedTypeSymbol1.SpecialType <> SpecialType.System_Collections_Generic_IReadOnlyCollection_T) Then
					GoTo Label1
				End If
				item = namedTypeSymbol.TypeArgumentsWithDefinitionUseSiteDiagnostics(useSiteInfo)(0)
				conversionKind = Conversions.ClassifyArrayInitialization(source.Initializer, item, binder, useSiteInfo)
				Return conversionKind
			Else
				item = arrayTypeSymbol.ElementType
				conversionKind = Conversions.ClassifyArrayInitialization(source.Initializer, item, binder, useSiteInfo)
				Return conversionKind
			End If
			Return conversionKind
		Label1:
			Dim conversionKind1 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyStringConversion(inferredType, destination)
			If (Conversions.NoConversion(conversionKind1)) Then
				conversionKind1 = Conversions.ClassifyDirectCastConversion(inferredType, destination, useSiteInfo)
			End If
			If (Not Conversions.NoConversion(conversionKind1)) Then
				Dim conversionKind2 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyArrayInitialization(source.Initializer, inferredType.ElementType, binder, useSiteInfo)
				If (Conversions.NoConversion(conversionKind2)) Then
					conversionKind = conversionKind2
					Return conversionKind
				ElseIf (Not Conversions.IsWideningConversion(conversionKind1)) Then
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Narrowing]
					Return conversionKind
				Else
					conversionKind = conversionKind2
					Return conversionKind
				End If
			Else
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
				Return conversionKind
			End If
		End Function

		Private Shared Function ClassifyAsReferenceType(ByVal candidate As TypeSymbol, ByRef isClassType As Boolean, ByRef isDelegateType As Boolean, ByRef isInterfaceType As Boolean, ByRef isArrayType As Boolean) As Boolean
			Dim flag As Boolean
			Select Case candidate.TypeKind
				Case Microsoft.CodeAnalysis.TypeKind.Array
					isClassType = False
					isDelegateType = False
					isInterfaceType = False
					isArrayType = True
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.[Class]
				Case Microsoft.CodeAnalysis.TypeKind.[Module]
					isClassType = True
					isDelegateType = False
					isInterfaceType = False
					isArrayType = False
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.[Delegate]
					isClassType = True
					isDelegateType = True
					isInterfaceType = False
					isArrayType = False
					Exit Select
				Case Microsoft.CodeAnalysis.TypeKind.Dynamic
				Case Microsoft.CodeAnalysis.TypeKind.[Enum]
				Case Microsoft.CodeAnalysis.TypeKind.[Error]
					isClassType = False
					isDelegateType = False
					isInterfaceType = False
					isArrayType = False
					flag = False
					Return flag
				Case Microsoft.CodeAnalysis.TypeKind.[Interface]
					isClassType = False
					isDelegateType = False
					isInterfaceType = True
					isArrayType = False
					Exit Select
				Case Else
					isClassType = False
					isDelegateType = False
					isInterfaceType = False
					isArrayType = False
					flag = False
					Return flag
			End Select
			flag = True
			Return flag
		End Function

		Public Shared Function ClassifyConversion(ByVal source As TypeSymbol, ByVal destination As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.ConversionKind, MethodSymbol)
			Dim keyValuePair As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.ConversionKind, MethodSymbol)
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyPredefinedConversion(source, destination, useSiteInfo)
			keyValuePair = If(Not Conversions.ConversionExists(conversionKind), Conversions.ClassifyUserDefinedConversion(source, destination, useSiteInfo), New KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.ConversionKind, MethodSymbol)(conversionKind, Nothing))
			Return keyValuePair
		End Function

		Public Shared Function ClassifyConversion(ByVal source As BoundExpression, ByVal destination As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As KeyValuePair(Of ConversionKind, MethodSymbol)
			' 
			' Current member / type: System.Collections.Generic.KeyValuePair`2<Microsoft.CodeAnalysis.VisualBasic.ConversionKind,Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol> Microsoft.CodeAnalysis.VisualBasic.Conversions::ClassifyConversion(Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.CompoundUseSiteInfo`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Collections.Generic.KeyValuePair<Microsoft.CodeAnalysis.VisualBasic.ConversionKind,Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol> ClassifyConversion(Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.CompoundUseSiteInfo<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Shared Function ClassifyConversionFromTypeParameter(ByVal typeParameter As TypeParameterSymbol, ByVal destination As TypeSymbol, ByVal varianceCompatibilityClassificationDepth As Integer, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			If (destination.SpecialType <> SpecialType.System_Object) Then
				Dim typeParameterSymbols As ArrayBuilder(Of TypeParameterSymbol) = Nothing
				Dim conversionKind1 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyConversionFromTypeParameter(typeParameter, destination, typeParameterSymbols, varianceCompatibilityClassificationDepth, useSiteInfo)
				If (typeParameterSymbols IsNot Nothing) Then
					typeParameterSymbols.Free()
				End If
				conversionKind = conversionKind1
			Else
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTypeParameter
			End If
			Return conversionKind
		End Function

		Private Shared Function ClassifyConversionFromTypeParameter(ByVal typeParameter As TypeParameterSymbol, ByVal destination As TypeSymbol, <InAttribute> <Out> ByRef queue As ArrayBuilder(Of TypeParameterSymbol), ByVal varianceCompatibilityClassificationDepth As Integer, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim flag As Boolean = False
			Dim flag1 As Boolean = False
			Dim flag2 As Boolean = False
			Dim flag3 As Boolean = False
			Dim flag4 As Boolean = False
			Dim flag5 As Boolean = False
			Dim flag6 As Boolean = False
			Dim flag7 As Boolean = False
			Dim num As Integer = 0
			Dim flag8 As Boolean = False
			Dim toInterfaceConversionClassifier As Conversions.ToInterfaceConversionClassifier = New Conversions.ToInterfaceConversionClassifier()
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Nothing
			Conversions.ClassifyAsReferenceType(destination, flag, flag1, flag2, flag3)
			If (flag2) Then
				namedTypeSymbol = DirectCast(destination, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			End If
			While True
				If (Not flag8 AndAlso typeParameter.HasValueTypeConstraint) Then
					If (destination.SpecialType <> Microsoft.CodeAnalysis.SpecialType.System_ValueType) Then
						If (flag2) Then
							Dim specialType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = typeParameter.ContainingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_ValueType)
							If (specialType.Kind <> SymbolKind.ErrorType AndAlso toInterfaceConversionClassifier.AccumulateConversionClassificationToVariantCompatibleInterface(specialType, namedTypeSymbol, varianceCompatibilityClassificationDepth, useSiteInfo)) Then
								conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTypeParameter
								Exit While
							End If
						End If
						flag8 = True
					Else
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTypeParameter
						Exit While
					End If
				End If
				Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = typeParameter.ConstraintTypesWithDefinitionUseSiteDiagnostics(useSiteInfo).GetEnumerator()
				While enumerator.MoveNext()
					Dim current As TypeSymbol = enumerator.Current
					If (current.Kind = SymbolKind.ErrorType) Then
						Continue While
					End If
					If (current.IsSameTypeIgnoringAll(destination)) Then
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTypeParameter
						Return conversionKind
					ElseIf (current.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Enum] OrElse Not DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).EnumUnderlyingType.IsSameTypeIgnoringAll(destination)) Then
						Dim flag9 As Boolean = False
						If (Not Conversions.ClassifyAsReferenceType(current, flag4, flag5, flag6, flag7)) Then
							flag9 = Conversions.IsValueType(current)
						End If
						If (flag2) Then
							If (Not flag4 AndAlso Not flag6 AndAlso Not flag9) Then
								If (flag7) Then
									Dim anInterface As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyReferenceConversionFromArrayToAnInterface(current, destination, varianceCompatibilityClassificationDepth, useSiteInfo)
									If (Conversions.IsWideningConversion(anInterface)) Then
										conversionKind = 129 Or CInt((anInterface And Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions))
										Return conversionKind
									End If
								End If
							ElseIf (toInterfaceConversionClassifier.AccumulateConversionClassificationToVariantCompatibleInterface(DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol), namedTypeSymbol, varianceCompatibilityClassificationDepth, useSiteInfo)) Then
								conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTypeParameter
								Return conversionKind
							End If
						ElseIf (flag) Then
							If ((flag4 OrElse flag9 OrElse flag7) AndAlso Conversions.IsDerivedFrom(current, destination, useSiteInfo)) Then
								conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTypeParameter
								Return conversionKind
							End If
						ElseIf (flag3 AndAlso flag7) Then
							Dim conversionKind1 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyArrayConversion(current, destination, varianceCompatibilityClassificationDepth, useSiteInfo)
							If (Conversions.IsWideningConversion(conversionKind1)) Then
								conversionKind = 129 Or CInt((conversionKind1 And Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions))
								Return conversionKind
							End If
						End If
						If (current.Kind <> SymbolKind.TypeParameter) Then
							Continue While
						End If
						If (queue Is Nothing) Then
							queue = ArrayBuilder(Of TypeParameterSymbol).GetInstance()
						End If
						queue.Add(DirectCast(current, TypeParameterSymbol))
					Else
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Widening] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions
						Return conversionKind
					End If
				End While
				If (queue IsNot Nothing AndAlso num < queue.Count) Then
					typeParameter = queue(num)
					num = num + 1
				ElseIf (Not flag2) Then
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
					Exit While
				Else
					Dim result As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = toInterfaceConversionClassifier.Result
					If (Not Conversions.ConversionExists(result)) Then
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTypeParameter
						Exit While
					Else
						conversionKind = 128 Or CInt(result)
						Exit While
					End If
				End If
			End While
			Return conversionKind
		End Function

		Public Shared Function ClassifyConversionOfOperandOfConditionalBranch(ByVal operand As BoundExpression, ByVal booleanType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <Out> ByRef applyNullableIsTrueOperator As Boolean, <Out> ByRef isTrueOperator As OverloadResolution.OverloadResolutionResult, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As KeyValuePair(Of ConversionKind, MethodSymbol)
			Dim keyValuePair As KeyValuePair(Of ConversionKind, MethodSymbol)
			applyNullableIsTrueOperator = False
			isTrueOperator = New OverloadResolution.OverloadResolutionResult()
			Dim keyValuePair1 As KeyValuePair(Of ConversionKind, MethodSymbol) = Conversions.ClassifyConversion(operand, booleanType, binder, useSiteInfo)
			If (Not Conversions.IsWideningConversion(keyValuePair1.Key)) Then
				Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = operand.Type
				If (type IsNot Nothing AndAlso Not type.IsErrorType() AndAlso Not type.IsObjectType()) Then
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
					Dim identity As KeyValuePair(Of ConversionKind, MethodSymbol) = New KeyValuePair(Of ConversionKind, MethodSymbol)()
					If (Not type.IsNullableOfBoolean()) Then
						Dim specialType As NamedTypeSymbol = booleanType.ContainingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Nullable_T)
						If (Not specialType.IsErrorType() AndAlso (type.IsNullableType() OrElse type.CanContainUserDefinedOperators(useSiteInfo))) Then
							typeSymbol = specialType.Construct(ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)(booleanType))
							identity = Conversions.ClassifyConversion(operand, typeSymbol, binder, useSiteInfo)
						End If
					Else
						identity = Conversions.Identity
						typeSymbol = type
					End If
					If (Not Conversions.IsWideningConversion(identity.Key)) Then
						Dim overloadResolutionResult As OverloadResolution.OverloadResolutionResult = New OverloadResolution.OverloadResolutionResult()
						If (type.CanContainUserDefinedOperators(useSiteInfo)) Then
							overloadResolutionResult = OverloadResolution.ResolveIsTrueOperator(operand, binder, useSiteInfo)
						End If
						If (Not overloadResolutionResult.BestResult.HasValue) Then
							If (Not Conversions.IsNarrowingConversion(identity.Key) OrElse (identity.Key And (ConversionKind.Nullable Or ConversionKind.UserDefined)) = ConversionKind.UserDefined AndAlso identity.Value.ReturnType.IsBooleanType()) Then
								GoTo Label1
							End If
							applyNullableIsTrueOperator = True
							keyValuePair = identity
							Return keyValuePair
						Else
							isTrueOperator = overloadResolutionResult
							If (overloadResolutionResult.BestResult.Value.Candidate.IsLifted) Then
								applyNullableIsTrueOperator = True
							End If
							keyValuePair = New KeyValuePair(Of ConversionKind, MethodSymbol)(ConversionKind.[Widening], Nothing)
							Return keyValuePair
						End If
					Else
						applyNullableIsTrueOperator = True
						keyValuePair = identity
						Return keyValuePair
					End If
				End If
			Label1:
				keyValuePair = keyValuePair1
			Else
				keyValuePair = keyValuePair1
			End If
			Return keyValuePair
		End Function

		Private Shared Function ClassifyConversionToTypeParameter(ByVal source As TypeSymbol, ByVal typeParameter As TypeParameterSymbol, ByVal varianceCompatibilityClassificationDepth As Integer, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim flag As Boolean = False
			Dim flag1 As Boolean = False
			Dim flag2 As Boolean = False
			Dim flag3 As Boolean = False
			Dim flag4 As Boolean = False
			Dim flag5 As Boolean = False
			Dim flag6 As Boolean = False
			Dim flag7 As Boolean = False
			If (source.SpecialType <> Microsoft.CodeAnalysis.SpecialType.System_Object) Then
				If (typeParameter.HasValueTypeConstraint) Then
					If (source.SpecialType = Microsoft.CodeAnalysis.SpecialType.System_ValueType) Then
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTypeParameter
						Return conversionKind
					ElseIf (Conversions.IsClassType(source)) Then
						Dim specialType As NamedTypeSymbol = typeParameter.ContainingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_ValueType)
						If (specialType.Kind = SymbolKind.ErrorType OrElse Not Conversions.IsDerivedFrom(specialType, source, useSiteInfo)) Then
							GoTo Label1
						End If
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTypeParameter
						Return conversionKind
					End If
				End If
			Label1:
				Conversions.ClassifyAsReferenceType(source, flag, flag1, flag2, flag3)
				If (Not flag2) Then
					Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = typeParameter.ConstraintTypesWithDefinitionUseSiteDiagnostics(useSiteInfo).GetEnumerator()
					While enumerator.MoveNext()
						Dim current As TypeSymbol = enumerator.Current
						If (current.Kind = SymbolKind.ErrorType) Then
							Continue While
						End If
						If (current.IsSameTypeIgnoringAll(source)) Then
							conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTypeParameter
							Return conversionKind
						ElseIf (current.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Enum] OrElse Not DirectCast(current, NamedTypeSymbol).EnumUnderlyingType.IsSameTypeIgnoringAll(source)) Then
							Dim flag8 As Boolean = False
							If (Not Conversions.ClassifyAsReferenceType(current, flag4, flag5, flag6, flag7)) Then
								flag8 = Conversions.IsValueType(current)
							End If
							If (Not flag4 AndAlso Not flag8 AndAlso Not flag7) Then
								If (current.Kind <> SymbolKind.TypeParameter) Then
									Continue While
								End If
								Dim conversionKind1 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyTypeParameterConversion(source, current, varianceCompatibilityClassificationDepth, useSiteInfo)
								If (Not Conversions.IsNarrowingConversion(conversionKind1)) Then
									Continue While
								End If
								conversionKind = 130 Or CInt((conversionKind1 And Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions))
								Return conversionKind
							ElseIf (Not flag) Then
								If (Not flag3 OrElse Not flag7) Then
									Continue While
								End If
								Dim conversionKind2 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyArrayConversion(current, source, varianceCompatibilityClassificationDepth, useSiteInfo)
								If (Not Conversions.IsWideningConversion(conversionKind2)) Then
									Continue While
								End If
								conversionKind = 130 Or CInt((conversionKind2 And Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions))
								Return conversionKind
							Else
								If (Not Conversions.IsDerivedFrom(current, source, useSiteInfo)) Then
									Continue While
								End If
								conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTypeParameter
								Return conversionKind
							End If
						Else
							conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Narrowing] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions
							Return conversionKind
						End If
					End While
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
				Else
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTypeParameter
				End If
			Else
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTypeParameter
			End If
			Return conversionKind
		End Function

		Private Shared Function ClassifyConversionToVariantCompatibleDelegateType(ByVal source As NamedTypeSymbol, ByVal destination As NamedTypeSymbol, ByVal varianceCompatibilityClassificationDepth As Integer, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind1 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyImmediateVarianceCompatibility(source, destination, varianceCompatibilityClassificationDepth, useSiteInfo)
			If (Not Conversions.ConversionExists(conversionKind1)) Then
				Dim conversionKind2 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyImmediateVarianceCompatibility(destination, source, varianceCompatibilityClassificationDepth, useSiteInfo)
				conversionKind = If(Not Conversions.ConversionExists(conversionKind2), (conversionKind1 Or conversionKind2) And Microsoft.CodeAnalysis.VisualBasic.ConversionKind.MightSucceedAtRuntime, conversionKind2 And (Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToNumericOverflow Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToIntegerOverflow Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToNumericOverflowMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToQueryLambdaBodyMismatch Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToArrayLiteralElementConversion Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Narrowing] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Numeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Nullable Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNullable Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Reference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingReference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Array Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingArray Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Value Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingValue Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[String] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingString Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Boolean] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingBoolean Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesNarrowingFromNumericConstant Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Lambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWidening Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningDropReturnOrArgs Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningToNonLambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNarrowing Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelInvalid Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.VarianceConversionAmbiguity Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.MightSucceedAtRuntime Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.AnonymousDelegate Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NeedAStub Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.ConvertedToExpressionTree Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.UserDefined Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Tuple Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTuple Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNullableTuple) Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Narrowing])
			Else
				conversionKind = conversionKind1
			End If
			Return conversionKind
		End Function

		Public Shared Function ClassifyDirectCastConversion(ByVal source As TypeSymbol, ByVal destination As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind1 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyIdentityConversion(source, destination)
			If (Not Conversions.ConversionExists(conversionKind1)) Then
				If (source.IsIntegralType()) Then
					If (destination.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Enum] OrElse Not DirectCast(destination, NamedTypeSymbol).EnumUnderlyingType.Equals(source)) Then
						GoTo Label1
					End If
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Narrowing] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Numeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions
					Return conversionKind
				ElseIf (destination.IsIntegralType()) Then
					If (source.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Enum] OrElse Not DirectCast(source, NamedTypeSymbol).EnumUnderlyingType.Equals(destination)) Then
						GoTo Label1
					End If
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Widening] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Numeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions
					Return conversionKind
				ElseIf (source.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Enum] AndAlso destination.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Enum]) Then
					Dim enumUnderlyingType As NamedTypeSymbol = DirectCast(source, NamedTypeSymbol).EnumUnderlyingType
					If (Not enumUnderlyingType.IsIntegralType() OrElse Not enumUnderlyingType.Equals(DirectCast(destination, NamedTypeSymbol).EnumUnderlyingType)) Then
						GoTo Label1
					End If
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Narrowing] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Numeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions
					Return conversionKind
				End If
			Label1:
				conversionKind1 = Conversions.ClassifyReferenceConversion(source, destination, 0, useSiteInfo)
				If (Not Conversions.ConversionExists(conversionKind1)) Then
					conversionKind1 = Conversions.ClassifyArrayConversion(source, destination, 0, useSiteInfo)
					If (Not Conversions.ConversionExists(conversionKind1)) Then
						conversionKind1 = Conversions.ClassifyValueTypeConversion(source, destination, useSiteInfo)
						If (Not Conversions.ConversionExists(conversionKind1)) Then
							conversionKind1 = Conversions.ClassifyTypeParameterConversion(source, destination, 0, useSiteInfo)
							conversionKind = conversionKind1
						Else
							conversionKind = conversionKind1
						End If
					Else
						conversionKind = conversionKind1
					End If
				Else
					conversionKind = conversionKind1
				End If
			Else
				conversionKind = conversionKind1
			End If
			Return conversionKind
		End Function

		Public Shared Function ClassifyDirectCastConversion(ByVal source As BoundExpression, ByVal destination As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ConversionKind
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.ConversionKind Microsoft.CodeAnalysis.VisualBasic.Conversions::ClassifyDirectCastConversion(Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.CompoundUseSiteInfo`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.ConversionKind ClassifyDirectCastConversion(Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.CompoundUseSiteInfo<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Shared Function ClassifyExpressionReclassification(ByVal source As BoundExpression, ByVal destination As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim kind As BoundKind = source.Kind
			If (kind <= BoundKind.ArrayLiteral) Then
				If (kind <= BoundKind.AddressOfOperator) Then
					If (kind = BoundKind.Parenthesized) Then
						If (source.Type IsNot Nothing) Then
							conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
							Return conversionKind
						End If
						conversionKind = Conversions.ClassifyExpressionReclassification(DirectCast(source, BoundParenthesized).Expression, destination, binder, useSiteInfo)
					Else
						If (kind <> BoundKind.AddressOfOperator) Then
							conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
							Return conversionKind
						End If
						conversionKind = Conversions.ClassifyAddressOfConversion(DirectCast(source, BoundAddressOfOperator), destination)
					End If
				ElseIf (kind = BoundKind.TupleLiteral) Then
					conversionKind = Conversions.ClassifyTupleConversion(DirectCast(source, BoundTupleLiteral), destination, binder, useSiteInfo)
				Else
					If (kind <> BoundKind.ArrayLiteral) Then
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
						Return conversionKind
					End If
					conversionKind = Conversions.ClassifyArrayLiteralConversion(DirectCast(source, BoundArrayLiteral), destination, binder, useSiteInfo)
				End If
			ElseIf (kind <= BoundKind.QueryLambda) Then
				If (kind = BoundKind.UnboundLambda) Then
					conversionKind = Conversions.ClassifyUnboundLambdaConversion(DirectCast(source, UnboundLambda), destination)
				Else
					If (kind <> BoundKind.QueryLambda) Then
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
						Return conversionKind
					End If
					conversionKind = Conversions.ClassifyQueryLambdaConversion(DirectCast(source, BoundQueryLambda), destination, binder, useSiteInfo)
				End If
			ElseIf (kind = BoundKind.GroupTypeInferenceLambda) Then
				conversionKind = Conversions.ClassifyGroupTypeInferenceLambdaConversion(DirectCast(source, GroupTypeInferenceLambda), destination)
			Else
				If (kind <> BoundKind.InterpolatedStringExpression) Then
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
					Return conversionKind
				End If
				conversionKind = Conversions.ClassifyInterpolatedStringConversion(DirectCast(source, BoundInterpolatedStringExpression), destination, binder)
			End If
			Return conversionKind
		End Function

		Private Shared Function ClassifyGroupTypeInferenceLambdaConversion(ByVal source As GroupTypeInferenceLambda, ByVal destination As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = destination.DelegateOrExpressionDelegate(source.Binder)
			If (namedTypeSymbol IsNot Nothing) Then
				Dim delegateInvokeMethod As MethodSymbol = namedTypeSymbol.DelegateInvokeMethod
				If (delegateInvokeMethod Is Nothing OrElse delegateInvokeMethod.GetUseSiteInfo().DiagnosticInfo IsNot Nothing OrElse delegateInvokeMethod.IsSub) Then
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
				Else
					Dim parameters As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) = source.Parameters
					If (delegateInvokeMethod.ParameterCount = parameters.Length) Then
						Dim parameterSymbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) = delegateInvokeMethod.Parameters
						Dim length As Integer = parameters.Length - 1
						Dim num As Integer = 0
						While num <= length
							Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = parameters(num)
							Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = parameterSymbols(num)
							If (item.IsByRef <> parameterSymbol.IsByRef OrElse item.Type IsNot Nothing AndAlso Not item.Type.IsSameTypeIgnoringAll(parameterSymbol.Type)) Then
								conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
								Return conversionKind
							Else
								num = num + 1
							End If
						End While
						If (delegateInvokeMethod.ReturnType.IsAnonymousType) Then
							Dim returnType As AnonymousTypeManager.AnonymousTypePublicSymbol = DirectCast(DirectCast(delegateInvokeMethod.ReturnType, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol), AnonymousTypeManager.AnonymousTypePublicSymbol)
							conversionKind = If(returnType.Properties.Length <> 1 OrElse returnType.Properties(0).SetMethod IsNot Nothing OrElse Not returnType.Properties(0).Name.Equals("$VB$ItAnonymous") OrElse Not parameterSymbols(1).Type.IsSameTypeIgnoringAll(returnType.Properties(0).Type), Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone, Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Widening] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Lambda)
						Else
							conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
						End If
					Else
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
					End If
				End If
			Else
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
			End If
			Return conversionKind
		End Function

		Private Shared Function ClassifyIdentityConversion(ByVal source As TypeSymbol, ByVal destination As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			conversionKind = If(Not source.IsSameTypeIgnoringAll(destination), Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone, Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Identity)
			Return conversionKind
		End Function

		Private Shared Function ClassifyImmediateVarianceCompatibility(ByVal source As NamedTypeSymbol, ByVal destination As NamedTypeSymbol, ByVal varianceCompatibilityClassificationDepth As Integer, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ConversionKind
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.ConversionKind Microsoft.CodeAnalysis.VisualBasic.Conversions::ClassifyImmediateVarianceCompatibility(Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol,System.Int32,Microsoft.CodeAnalysis.CompoundUseSiteInfo`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.ConversionKind ClassifyImmediateVarianceCompatibility(Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol,System.Int32,Microsoft.CodeAnalysis.CompoundUseSiteInfo<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Shared Function ClassifyInterpolatedStringConversion(ByVal source As BoundInterpolatedStringExpression, ByVal destination As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			conversionKind = If(destination.Equals(binder.Compilation.GetWellKnownType(WellKnownType.System_FormattableString)) OrElse destination.Equals(binder.Compilation.GetWellKnownType(WellKnownType.System_IFormattable)), Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InterpolatedString, Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone)
			Return conversionKind
		End Function

		Public Shared Function ClassifyMethodConversionBasedOnArgumentConversion(ByVal conversion As ConversionKind, ByVal delegateParameterType As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind
			Dim methodConversionKind As Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind
			If (Conversions.NoConversion(conversion)) Then
				methodConversionKind = Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind.Error_OverloadResolution
			ElseIf (Conversions.IsNarrowingConversion(conversion)) Then
				methodConversionKind = Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind.OneArgumentIsNarrowing
			ElseIf (Conversions.IsIdentityConversion(conversion)) Then
				methodConversionKind = Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind.Identity
			Else
				methodConversionKind = If(Not Conversions.IsCLRPredefinedConversion(conversion) OrElse Not delegateParameterType.IsReferenceType, Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind.OneArgumentIsVbOrBoxWidening, Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind.OneArgumentIsClrWidening)
			End If
			Return methodConversionKind
		End Function

		Public Shared Function ClassifyMethodConversionBasedOnReturn(ByVal returnTypeOfConvertFromMethod As TypeSymbol, ByVal convertFromMethodIsByRef As Boolean, ByVal returnTypeOfConvertToMethod As TypeSymbol, ByVal convertToMethodIsByRef As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind
			Dim methodConversionKind As Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind
			methodConversionKind = If(convertToMethodIsByRef = convertFromMethodIsByRef, Conversions.ClassifyMethodConversionBasedOnReturnType(returnTypeOfConvertFromMethod, returnTypeOfConvertToMethod, convertFromMethodIsByRef, useSiteInfo), Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind.Error_ByRefByValMismatch)
			Return methodConversionKind
		End Function

		Public Shared Function ClassifyMethodConversionBasedOnReturnType(ByVal returnTypeOfConvertFromMethod As TypeSymbol, ByVal returnTypeOfConvertToMethod As TypeSymbol, ByVal isRefReturning As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As MethodConversionKind
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind Microsoft.CodeAnalysis.VisualBasic.Conversions::ClassifyMethodConversionBasedOnReturnType(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,System.Boolean,Microsoft.CodeAnalysis.CompoundUseSiteInfo`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind ClassifyMethodConversionBasedOnReturnType(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,System.Boolean,Microsoft.CodeAnalysis.CompoundUseSiteInfo<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Shared Function ClassifyMethodConversionForEventRaise(ByVal toDelegateInvokeMethod As MethodSymbol, ByVal parameters As ImmutableArray(Of ParameterSymbol), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As MethodConversionKind
			Return Conversions.ClassifyMethodConversionForLambdaOrAnonymousDelegateBasedOnParameters(New UnboundLambda.TargetSignature(toDelegateInvokeMethod), parameters, useSiteInfo)
		End Function

		Public Shared Function ClassifyMethodConversionForLambdaOrAnonymousDelegate(ByVal toMethod As MethodSymbol, ByVal lambdaOrDelegateInvokeSymbol As MethodSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As MethodConversionKind
			Return Conversions.ClassifyMethodConversionForLambdaOrAnonymousDelegate(New UnboundLambda.TargetSignature(toMethod), lambdaOrDelegateInvokeSymbol, useSiteInfo)
		End Function

		Public Shared Function ClassifyMethodConversionForLambdaOrAnonymousDelegate(ByVal toMethodSignature As UnboundLambda.TargetSignature, ByVal lambdaOrDelegateInvokeSymbol As MethodSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As MethodConversionKind
			Return Conversions.ClassifyMethodConversionBasedOnReturn(lambdaOrDelegateInvokeSymbol.ReturnType, lambdaOrDelegateInvokeSymbol.ReturnsByRef, toMethodSignature.ReturnType, toMethodSignature.ReturnsByRef, useSiteInfo) Or Conversions.ClassifyMethodConversionForLambdaOrAnonymousDelegateBasedOnParameters(toMethodSignature, lambdaOrDelegateInvokeSymbol.Parameters, useSiteInfo)
		End Function

		Private Shared Function ClassifyMethodConversionForLambdaOrAnonymousDelegateBasedOnParameters(ByVal toMethodSignature As UnboundLambda.TargetSignature, ByVal parameters As ImmutableArray(Of ParameterSymbol), <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind
			Dim methodConversionKind As Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind = 0
			If (parameters.Length = 0 AndAlso toMethodSignature.ParameterTypes.Length > 0) Then
				methodConversionKind = methodConversionKind Or Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind.AllArgumentsIgnored
			ElseIf (parameters.Length = toMethodSignature.ParameterTypes.Length) Then
				Dim length As Integer = parameters.Length - 1
				For i As Integer = 0 To length
					If (toMethodSignature.ParameterIsByRef(i) <> parameters(i).IsByRef) Then
						methodConversionKind = methodConversionKind Or Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind.Error_ByRefByValMismatch
					End If
					Dim item As TypeSymbol = toMethodSignature.ParameterTypes(i)
					Dim type As TypeSymbol = parameters(i).Type
					If (Not item.IsErrorType() AndAlso Not type.IsErrorType()) Then
						Dim keyValuePair As KeyValuePair(Of ConversionKind, MethodSymbol) = Conversions.ClassifyConversion(item, type, useSiteInfo)
						methodConversionKind = methodConversionKind Or Conversions.ClassifyMethodConversionBasedOnArgumentConversion(keyValuePair.Key, item)
						If (toMethodSignature.ParameterIsByRef(i)) Then
							keyValuePair = Conversions.ClassifyConversion(type, item, useSiteInfo)
							methodConversionKind = methodConversionKind Or Conversions.ClassifyMethodConversionBasedOnArgumentConversion(keyValuePair.Key, type)
						End If
					End If
				Next

			Else
				methodConversionKind = methodConversionKind Or Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind.Error_OverloadResolution
			End If
			Return methodConversionKind
		End Function

		Private Shared Function ClassifyNothingLiteralConversion(ByVal constantExpression As BoundExpression, ByVal destination As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			If (Not constantExpression.IsStrictNothingLiteral()) Then
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
			Else
				conversionKind = If(Not destination.IsObjectType() OrElse constantExpression.Type Is Nothing OrElse Not constantExpression.Type.IsObjectType(), Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNothingLiteral, Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Identity)
			End If
			Return conversionKind
		End Function

		Private Shared Function ClassifyNullableConversion(ByVal source As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal destination As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind1 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim flag As Boolean = source.IsNullableType()
			Dim flag1 As Boolean = destination.IsNullableType()
			If (flag OrElse flag1) Then
				Dim nullableUnderlyingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
				If (flag) Then
					nullableUnderlyingType = source.GetNullableUnderlyingType()
					If (nullableUnderlyingType.Kind <> SymbolKind.ErrorType AndAlso nullableUnderlyingType.IsValueType AndAlso Not nullableUnderlyingType.IsNullableType()) Then
						GoTo Label1
					End If
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
					Return conversionKind
				End If
			Label1:
				If (flag1) Then
					typeSymbol = destination.GetNullableUnderlyingType()
					If (typeSymbol.Kind <> SymbolKind.ErrorType AndAlso typeSymbol.IsValueType AndAlso Not typeSymbol.IsNullableType()) Then
						GoTo Label2
					End If
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
					Return conversionKind
				End If
				If (Not flag) Then
					If (Not source.IsSameTypeIgnoringAll(typeSymbol)) Then
						Dim conversionKind2 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyPredefinedConversion(source, typeSymbol, useSiteInfo)
						If (Not Conversions.IsWideningConversion(conversionKind2)) Then
							If (Not Conversions.IsNarrowingConversion(conversionKind2)) Then
								GoTo Label3
							End If
							conversionKind = 18 Or CInt((conversionKind2 And (Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWidening Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningDropReturnOrArgs Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningToNonLambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNarrowing Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelInvalid Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Tuple)))
							Return conversionKind
						Else
							conversionKind = 17 Or CInt((conversionKind2 And (Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWidening Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningDropReturnOrArgs Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningToNonLambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNarrowing Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelInvalid Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Tuple)))
							Return conversionKind
						End If
					Else
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNullable
						Return conversionKind
					End If
				ElseIf (flag1) Then
					conversionKind1 = Conversions.ClassifyPredefinedConversion(nullableUnderlyingType, typeSymbol, useSiteInfo)
					If (Not Conversions.IsWideningConversion(conversionKind1)) Then
						If (Not Conversions.IsNarrowingConversion(conversionKind1)) Then
							GoTo Label3
						End If
						conversionKind = 18 Or CInt((conversionKind1 And (Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWidening Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningDropReturnOrArgs Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningToNonLambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNarrowing Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelInvalid Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Tuple)))
						Return conversionKind
					Else
						conversionKind = 17 Or CInt((conversionKind1 And (Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWidening Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningDropReturnOrArgs Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningToNonLambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNarrowing Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelInvalid Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Tuple)))
						Return conversionKind
					End If
				ElseIf (Conversions.IsInterfaceType(destination)) Then
					conversionKind1 = Conversions.ClassifyDirectCastConversion(nullableUnderlyingType, destination, useSiteInfo)
					If (Not Conversions.IsWideningConversion(conversionKind1)) Then
						If (Not Conversions.IsNarrowingConversion(conversionKind1)) Then
							GoTo Label3
						End If
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNullable
						Return conversionKind
					Else
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNullable
						Return conversionKind
					End If
				ElseIf (Not nullableUnderlyingType.IsSameTypeIgnoringAll(destination)) Then
					conversionKind1 = Conversions.ClassifyPredefinedConversion(nullableUnderlyingType, destination, useSiteInfo)
					If (Not Conversions.ConversionExists(conversionKind1)) Then
						GoTo Label3
					End If
					conversionKind = 18 Or CInt((conversionKind1 And (Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWidening Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningDropReturnOrArgs Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningToNonLambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNarrowing Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelInvalid Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Tuple)))
					Return conversionKind
				Else
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNullable
					Return conversionKind
				End If
			Label3:
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
			Else
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
			End If
			Return conversionKind
		End Function

		Private Shared Function ClassifyNumericConstantConversion(ByVal constantExpression As BoundExpression, ByVal destination As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim nullable As Nullable(Of Microsoft.CodeAnalysis.VisualBasic.ConversionKind)
			Dim constantValueOpt As ConstantValue
			If (Not constantExpression.ConstantValueOpt.IsBad) Then
				Dim nullableUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = destination.GetNullableUnderlyingTypeOrSelf()
				If (Not constantExpression.IsIntegerZeroLiteral() OrElse Not nullableUnderlyingTypeOrSelf.IsEnumType() OrElse Not DirectCast(nullableUnderlyingTypeOrSelf, NamedTypeSymbol).EnumUnderlyingType.IsIntegralType()) Then
					Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = constantExpression.Type
					If (type Is Nothing) Then
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
					ElseIf (CObj(type) <> CObj(destination)) Then
						Dim value As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
						Dim flag As Boolean = False
						If (type.IsIntegralType()) Then
							If (Not nullableUnderlyingTypeOrSelf.IsIntegralType()) Then
								GoTo Label1
							End If
							nullable = Conversions.FastClassifyPredefinedConversion(type, destination)
							value = nullable.Value
							If (Conversions.IsNarrowingConversion(value)) Then
								value = value Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesNarrowingFromNumericConstant
								constantValueOpt = constantExpression.ConstantValueOpt
								Conversions.TryFoldConstantNumericOrBooleanConversion(constantValueOpt, type, nullableUnderlyingTypeOrSelf, flag)
								If (flag) Then
									If (Not binder.CheckOverflow) Then
										conversionKind = value
										Return conversionKind
									End If
									conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToIntegerOverflow
									Return conversionKind
								ElseIf (CObj(nullableUnderlyingTypeOrSelf) = CObj(destination)) Then
									value = value And (Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToNumericOverflow Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToIntegerOverflow Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToNumericOverflowMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToQueryLambdaBodyMismatch Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToArrayLiteralElementConversion Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Widening] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Identity Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Numeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Nullable Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNullable Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Reference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningReference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Array Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningArray Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Value Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningValue Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[String] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningString Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Boolean] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNothingLiteral Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesNarrowingFromNumericConstant Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Lambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWidening Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningDropReturnOrArgs Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningToNonLambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNarrowing Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelInvalid Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.VarianceConversionAmbiguity Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.MightSucceedAtRuntime Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.AnonymousDelegate Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NeedAStub Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.ConvertedToExpressionTree Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.UserDefined Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingDueToContraVarianceInDelegate Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InterpolatedString Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Tuple Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTuple Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNullableTuple) Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Widening]
								End If
							End If
							conversionKind = value
							Return conversionKind
						ElseIf (type.IsFloatingType() AndAlso nullableUnderlyingTypeOrSelf.IsFloatingType()) Then
							nullable = Conversions.FastClassifyPredefinedConversion(type, destination)
							value = nullable.Value
							If (Conversions.IsNarrowingConversion(value)) Then
								value = value Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesNarrowingFromNumericConstant
							End If
						End If
					Label1:
						If (Not Conversions.IsWideningConversion(value)) Then
							Dim enumUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = type.GetEnumUnderlyingTypeOrSelf()
							Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = destination.GetNullableUnderlyingTypeOrSelf().GetEnumUnderlyingTypeOrSelf()
							If (enumUnderlyingTypeOrSelf.IsNumericType() AndAlso typeSymbol.IsNumericType()) Then
								constantValueOpt = constantExpression.ConstantValueOpt
								If (Not Conversions.TryFoldConstantNumericOrBooleanConversion(constantValueOpt, enumUnderlyingTypeOrSelf, typeSymbol, flag).IsBad) Then
									If (Not flag OrElse Not binder.CheckOverflow) Then
										GoTo Label3
									End If
									conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToIntegerOverflow
									Return conversionKind
								Else
									value = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToNumericOverflow
								End If
							End If
						End If
					Label3:
						conversionKind = value
					Else
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Identity
					End If
				Else
					conversionKind = If(CObj(nullableUnderlyingTypeOrSelf) <> CObj(destination), Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Narrowing] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Nullable Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNullable Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesNarrowingFromNumericConstant, Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Widening] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Numeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions)
				End If
			Else
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
			End If
			Return conversionKind
		End Function

		Public Shared Function ClassifyPredefinedConversion(ByVal source As BoundExpression, ByVal destination As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ConversionKind
			Dim flag As Boolean = False
			Return Conversions.ClassifyPredefinedConversion(source, destination, binder, flag, useSiteInfo)
		End Function

		Private Shared Function ClassifyPredefinedConversion(ByVal source As BoundExpression, ByVal destination As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <Out> ByRef userDefinedConversionsMightStillBeApplicable As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim inferredType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			userDefinedConversionsMightStillBeApplicable = False
			Dim isConstant As Boolean = False
			If (source.Kind = BoundKind.FieldAccess) Then
				isConstant = If(DirectCast(source, BoundFieldAccess).FieldSymbol.GetConstantValue(binder.ConstantFieldsInProgress) Is Nothing, False, source.IsConstant)
			ElseIf (source.Kind <> BoundKind.Local) Then
				isConstant = source.IsConstant
			Else
				isConstant = If(DirectCast(source, BoundLocal).LocalSymbol.GetConstantValue(binder) Is Nothing, False, source.IsConstant)
			End If
			If (isConstant) Then
				Dim conversionKind1 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyNothingLiteralConversion(source, destination)
				If (Not Conversions.ConversionExists(conversionKind1)) Then
					conversionKind1 = Conversions.ClassifyNumericConstantConversion(source, destination, binder)
					If (Not Conversions.ConversionExists(conversionKind1) AndAlso Not Conversions.FailedDueToNumericOverflow(conversionKind1)) Then
						GoTo Label1
					End If
					conversionKind = conversionKind1
					Return conversionKind
				Else
					conversionKind = conversionKind1
					Return conversionKind
				End If
			End If
		Label1:
			If (source.IsValue()) Then
				If (source.Kind = BoundKind.TupleLiteral) Then
					inferredType = DirectCast(source, BoundTupleLiteral).InferredType
				Else
					inferredType = source.Type
				End If
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = inferredType
				If (typeSymbol IsNot Nothing) Then
					If (typeSymbol.Kind <> SymbolKind.ErrorType) Then
						Dim conversionKind2 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyPredefinedConversion(typeSymbol, destination, useSiteInfo)
						If (Conversions.ConversionExists(conversionKind2)) Then
							conversionKind = conversionKind2
							Return conversionKind
						End If
						userDefinedConversionsMightStillBeApplicable = True
					End If
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
				Else
					Dim kind As BoundKind = source.GetMostEnclosedParenthesizedExpression().Kind
					userDefinedConversionsMightStillBeApplicable = If(kind = BoundKind.ArrayLiteral, True, kind = BoundKind.TupleLiteral)
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
				End If
			Else
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
			End If
			Return conversionKind
		End Function

		Public Shared Function ClassifyPredefinedConversion(ByVal source As TypeSymbol, ByVal destination As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim nullable As Nullable(Of Microsoft.CodeAnalysis.VisualBasic.ConversionKind) = Conversions.FastClassifyPredefinedConversion(source, destination)
			conversionKind = If(Not nullable.HasValue, Conversions.ClassifyPredefinedConversionSlow(source, destination, useSiteInfo), nullable.Value)
			Return conversionKind
		End Function

		Private Shared Function ClassifyPredefinedConversionSlow(ByVal source As TypeSymbol, ByVal destination As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind1 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyIdentityConversion(source, destination)
			If (Not Conversions.ConversionExists(conversionKind1)) Then
				conversionKind1 = Conversions.ClassifyReferenceConversion(source, destination, 0, useSiteInfo)
				If (Not Conversions.ConversionExists(conversionKind1)) Then
					conversionKind1 = Conversions.ClassifyAnonymousDelegateConversion(source, destination, useSiteInfo)
					If (Not Conversions.ConversionExists(conversionKind1)) Then
						conversionKind1 = Conversions.ClassifyArrayConversion(source, destination, 0, useSiteInfo)
						If (Not Conversions.ConversionExists(conversionKind1)) Then
							conversionKind1 = Conversions.ClassifyTupleConversion(source, destination, useSiteInfo)
							If (Not Conversions.ConversionExists(conversionKind1)) Then
								conversionKind1 = Conversions.ClassifyValueTypeConversion(source, destination, useSiteInfo)
								If (Not Conversions.ConversionExists(conversionKind1)) Then
									conversionKind1 = Conversions.ClassifyNullableConversion(source, destination, useSiteInfo)
									If (Not Conversions.ConversionExists(conversionKind1)) Then
										conversionKind1 = Conversions.ClassifyStringConversion(source, destination)
										If (Not Conversions.ConversionExists(conversionKind1)) Then
											conversionKind1 = Conversions.ClassifyTypeParameterConversion(source, destination, 0, useSiteInfo)
											conversionKind = Conversions.AddDelegateRelaxationInformationForADelegate(source, destination, conversionKind1)
										Else
											conversionKind = conversionKind1
										End If
									Else
										conversionKind = conversionKind1
									End If
								Else
									conversionKind = conversionKind1
								End If
							Else
								conversionKind = conversionKind1
							End If
						Else
							conversionKind = conversionKind1
						End If
					Else
						conversionKind = conversionKind1
					End If
				Else
					conversionKind = Conversions.AddDelegateRelaxationInformationForADelegate(source, destination, conversionKind1)
				End If
			Else
				conversionKind = conversionKind1
			End If
			Return conversionKind
		End Function

		Private Shared Function ClassifyQueryLambdaConversion(ByVal source As BoundQueryLambda, ByVal destination As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim key As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim flag As Boolean = False
			Dim keyValuePair As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.ConversionKind, MethodSymbol)
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = destination.DelegateOrExpressionDelegate(binder, flag)
			If (namedTypeSymbol IsNot Nothing) Then
				Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = If(flag, Microsoft.CodeAnalysis.VisualBasic.ConversionKind.ConvertedToExpressionTree, Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone)
				Dim delegateInvokeMethod As MethodSymbol = namedTypeSymbol.DelegateInvokeMethod
				If (delegateInvokeMethod Is Nothing OrElse delegateInvokeMethod.GetUseSiteInfo().DiagnosticInfo IsNot Nothing OrElse delegateInvokeMethod.IsSub) Then
					key = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
				ElseIf (delegateInvokeMethod.ParameterCount = source.LambdaSymbol.ParameterCount) Then
					Dim parameters As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) = source.LambdaSymbol.Parameters
					Dim parameterSymbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) = delegateInvokeMethod.Parameters
					Dim length As Integer = parameters.Length - 1
					Dim num As Integer = 0
					While num <= length
						Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = parameters(num)
						Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = parameterSymbols(num)
						If (item.IsByRef <> parameterSymbol.IsByRef OrElse Not item.Type.IsSameTypeIgnoringAll(parameterSymbol.Type)) Then
							key = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
							Return key
						Else
							num = num + 1
						End If
					End While
					If (CObj(source.LambdaSymbol.ReturnType) <> CObj(LambdaSymbol.ReturnTypePendingDelegate)) Then
						If (Not delegateInvokeMethod.ReturnType.IsSameTypeIgnoringAll(source.LambdaSymbol.ReturnType)) Then
							GoTo Label1
						End If
						key = 16385 Or CInt(conversionKind)
						Return key
					Else
						If (delegateInvokeMethod.ReturnType.IsErrorType()) Then
							GoTo Label1
						End If
						If (Not source.ExprIsOperandOfConditionalBranch OrElse Not delegateInvokeMethod.ReturnType.IsBooleanType()) Then
							keyValuePair = Conversions.ClassifyConversion(source.Expression, delegateInvokeMethod.ReturnType, binder, useSiteInfo)
						Else
							Dim expression As BoundExpression = source.Expression
							Dim returnType As TypeSymbol = delegateInvokeMethod.ReturnType
							Dim flag1 As Boolean = False
							Dim overloadResolutionResult As OverloadResolution.OverloadResolutionResult = New OverloadResolution.OverloadResolutionResult()
							keyValuePair = Conversions.ClassifyConversionOfOperandOfConditionalBranch(expression, returnType, binder, flag1, overloadResolutionResult, useSiteInfo)
						End If
						If (Conversions.IsIdentityConversion(keyValuePair.Key)) Then
							key = keyValuePair.Key And (Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToNumericOverflow Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToIntegerOverflow Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToNumericOverflowMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToQueryLambdaBodyMismatch Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToArrayLiteralElementConversion Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Narrowing] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Numeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Nullable Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNullable Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Reference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingReference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Array Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingArray Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Value Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingValue Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[String] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingString Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Boolean] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingBoolean Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesNarrowingFromNumericConstant Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Lambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWidening Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningDropReturnOrArgs Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningToNonLambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNarrowing Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelInvalid Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.VarianceConversionAmbiguity Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.MightSucceedAtRuntime Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.AnonymousDelegate Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NeedAStub Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.ConvertedToExpressionTree Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.UserDefined Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingDueToContraVarianceInDelegate Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Tuple Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTuple Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNullableTuple) Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Widening] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Lambda Or conversionKind
							Return key
						ElseIf (Not Conversions.NoConversion(keyValuePair.Key)) Then
							key = keyValuePair.Key And (Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToNumericOverflow Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToIntegerOverflow Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToNumericOverflowMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToQueryLambdaBodyMismatch Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToArrayLiteralElementConversion Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Widening] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Narrowing] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Identity Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Numeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Reference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningReference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingReference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Array Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningArray Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingArray Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Value Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningValue Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingValue Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[String] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningString Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingString Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Boolean] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingBoolean Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNothingLiteral Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesNarrowingFromNumericConstant Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Lambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWidening Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningDropReturnOrArgs Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningToNonLambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNarrowing Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelInvalid Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.VarianceConversionAmbiguity Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.MightSucceedAtRuntime Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.AnonymousDelegate Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NeedAStub Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.ConvertedToExpressionTree Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingDueToContraVarianceInDelegate Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InterpolatedString) Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Lambda Or conversionKind
							Return key
						Else
							key = keyValuePair.Key Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToQueryLambdaBodyMismatch Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Lambda Or conversionKind
							Return key
						End If
					End If
				Label1:
					key = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
				Else
					key = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
				End If
			Else
				key = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
			End If
			Return key
		End Function

		Private Shared Function ClassifyReferenceConversion(ByVal source As TypeSymbol, ByVal destination As TypeSymbol, ByVal varianceCompatibilityClassificationDepth As Integer, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ConversionKind
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.ConversionKind Microsoft.CodeAnalysis.VisualBasic.Conversions::ClassifyReferenceConversion(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,System.Int32,Microsoft.CodeAnalysis.CompoundUseSiteInfo`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.ConversionKind ClassifyReferenceConversion(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,System.Int32,Microsoft.CodeAnalysis.CompoundUseSiteInfo<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Shared Function ClassifyReferenceConversionFromArrayToAnInterface(ByVal source As TypeSymbol, ByVal destination As TypeSymbol, ByVal varianceCompatibilityClassificationDepth As Integer, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = source.BaseTypeWithDefinitionUseSiteDiagnostics(useSiteInfo)
			If (namedTypeSymbol Is Nothing OrElse namedTypeSymbol.IsErrorType() OrElse namedTypeSymbol.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Class] OrElse Not Conversions.IsWideningConversion(Conversions.ClassifyDirectCastConversion(namedTypeSymbol, destination, useSiteInfo))) Then
				Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(source, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
				If (arrayTypeSymbol.IsSZArray) Then
					Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(destination.OriginalDefinition, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					If (originalDefinition = destination OrElse originalDefinition.Kind = SymbolKind.ErrorType) Then
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
					Else
						Dim specialType As Microsoft.CodeAnalysis.SpecialType = originalDefinition.SpecialType
						If (specialType = Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IList_T OrElse specialType = Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_ICollection_T OrElse specialType = Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IEnumerable_T OrElse specialType = Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IReadOnlyList_T OrElse specialType = Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IReadOnlyCollection_T) Then
							Dim item As TypeSymbol = DirectCast(destination, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).TypeArgumentsWithDefinitionUseSiteDiagnostics(useSiteInfo)(0)
							If (item.Kind <> SymbolKind.ErrorType) Then
								Dim elementType As TypeSymbol = arrayTypeSymbol.ElementType
								If (elementType.Kind = SymbolKind.ErrorType) Then
									conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
								ElseIf (Not elementType.IsSameTypeIgnoringAll(item)) Then
									Dim conversionKind1 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyArrayConversionBasedOnElementTypes(elementType, item, varianceCompatibilityClassificationDepth, useSiteInfo)
									If (Conversions.IsWideningConversion(conversionKind1)) Then
										conversionKind = 33 Or CInt((conversionKind1 And Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions))
									ElseIf (Not Conversions.IsNarrowingConversion(conversionKind1)) Then
										conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingReference
									Else
										conversionKind = 34 Or CInt((conversionKind1 And (Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.VarianceConversionAmbiguity)))
									End If
								Else
									conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningReference
								End If
							Else
								conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
							End If
						Else
							conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
						End If
					End If
				Else
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
				End If
			Else
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningReference
			End If
			Return conversionKind
		End Function

		Public Shared Function ClassifyStringConversion(ByVal source As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal destination As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol
			If (source.SpecialType <> SpecialType.System_String) Then
				If (destination.SpecialType = SpecialType.System_String) Then
					typeSymbol = source
					If (typeSymbol.Kind = SymbolKind.ArrayType) Then
						arrayTypeSymbol = DirectCast(typeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
						If (Not arrayTypeSymbol.IsSZArray OrElse arrayTypeSymbol.ElementType.SpecialType <> SpecialType.System_Char) Then
							conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
							Return conversionKind
						End If
						If (arrayTypeSymbol <> source) Then
							conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingString
							Return conversionKind
						Else
							conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningString
							Return conversionKind
						End If
					End If
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
					Return conversionKind
				End If
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
				Return conversionKind
			Else
				typeSymbol = destination
			End If
			If (typeSymbol.Kind = SymbolKind.ArrayType) Then
				arrayTypeSymbol = DirectCast(typeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
				If (Not arrayTypeSymbol.IsSZArray OrElse arrayTypeSymbol.ElementType.SpecialType <> SpecialType.System_Char) Then
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
					Return conversionKind
				End If
				If (arrayTypeSymbol <> source) Then
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingString
					Return conversionKind
				Else
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningString
					Return conversionKind
				End If
			End If
			conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
			Return conversionKind
		End Function

		Public Shared Function ClassifyTryCastConversion(ByVal source As TypeSymbol, ByVal destination As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind1 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyDirectCastConversion(source, destination, useSiteInfo)
			conversionKind = If(Not Conversions.ConversionExists(conversionKind1), Conversions.ClassifyTryCastConversionForTypeParameters(source, destination, useSiteInfo), conversionKind1)
			Return conversionKind
		End Function

		Public Shared Function ClassifyTryCastConversion(ByVal source As BoundExpression, ByVal destination As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ConversionKind
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.ConversionKind Microsoft.CodeAnalysis.VisualBasic.Conversions::ClassifyTryCastConversion(Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.CompoundUseSiteInfo`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.ConversionKind ClassifyTryCastConversion(Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.CompoundUseSiteInfo<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Shared Function ClassifyTryCastConversionForTypeParameters(ByVal source As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal destination As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim kind As Microsoft.CodeAnalysis.SymbolKind = source.Kind
			Dim symbolKind As Microsoft.CodeAnalysis.SymbolKind = destination.Kind
			If (kind = Microsoft.CodeAnalysis.SymbolKind.ArrayType AndAlso symbolKind = Microsoft.CodeAnalysis.SymbolKind.ArrayType) Then
				Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(source, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
				Dim arrayTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(destination, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
				Dim elementType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = arrayTypeSymbol.ElementType
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = arrayTypeSymbol1.ElementType
				If (Not elementType.IsReferenceType) Then
					If (Not elementType.IsValueType OrElse Not typeSymbol.IsReferenceType) Then
						GoTo Label1
					End If
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
					Return conversionKind
				Else
					If (Not typeSymbol.IsValueType) Then
						GoTo Label1
					End If
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
					Return conversionKind
				End If
			Label1:
				conversionKind = Conversions.ClassifyTryCastConversionForTypeParameters(elementType, typeSymbol, useSiteInfo)
			ElseIf (kind = Microsoft.CodeAnalysis.SymbolKind.TypeParameter OrElse symbolKind = Microsoft.CodeAnalysis.SymbolKind.TypeParameter) Then
				Dim nonInterfaceTypeConstraintOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Conversions.GetNonInterfaceTypeConstraintOrSelf(source, useSiteInfo)
				Dim nonInterfaceTypeConstraintOrSelf1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Conversions.GetNonInterfaceTypeConstraintOrSelf(destination, useSiteInfo)
				If (nonInterfaceTypeConstraintOrSelf Is Nothing OrElse nonInterfaceTypeConstraintOrSelf1 Is Nothing) Then
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Narrowing]
				Else
					Dim conversionKind1 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyDirectCastConversion(nonInterfaceTypeConstraintOrSelf, nonInterfaceTypeConstraintOrSelf1, useSiteInfo)
					If (Not Conversions.IsWideningConversion(conversionKind1)) Then
						conversionKind1 = Conversions.ClassifyDirectCastConversion(nonInterfaceTypeConstraintOrSelf1, nonInterfaceTypeConstraintOrSelf, useSiteInfo)
						If (Not Conversions.IsWideningConversion(conversionKind1)) Then
							conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
						ElseIf (kind <> Microsoft.CodeAnalysis.SymbolKind.TypeParameter OrElse nonInterfaceTypeConstraintOrSelf1.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Class] AndAlso Not DirectCast(nonInterfaceTypeConstraintOrSelf1, NamedTypeSymbol).IsNotInheritable OrElse Conversions.ClassOrBasesSatisfyConstraints(nonInterfaceTypeConstraintOrSelf1, DirectCast(source, TypeParameterSymbol), useSiteInfo)) Then
							conversionKind = 2 Or CInt((conversionKind1 And Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions))
						Else
							conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
						End If
					ElseIf (symbolKind <> Microsoft.CodeAnalysis.SymbolKind.TypeParameter OrElse nonInterfaceTypeConstraintOrSelf.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Class] AndAlso Not DirectCast(nonInterfaceTypeConstraintOrSelf, NamedTypeSymbol).IsNotInheritable OrElse Conversions.ClassOrBasesSatisfyConstraints(nonInterfaceTypeConstraintOrSelf, DirectCast(destination, TypeParameterSymbol), useSiteInfo)) Then
						conversionKind = 2 Or CInt((conversionKind1 And Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions))
					Else
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
					End If
				End If
			Else
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
			End If
			Return conversionKind
		End Function

		Public Shared Function ClassifyTupleConversion(ByVal source As BoundTupleLiteral, ByVal destination As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim flag As Boolean
			If (Not Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(source.Type, destination, TypeCompareKind.ConsiderEverything)) Then
				Dim arguments As ImmutableArray(Of BoundExpression) = source.Arguments
				Dim conversionKind1 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTuple
				Dim conversionKind2 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTuple
				If (destination.IsNullableType()) Then
					destination = destination.GetNullableUnderlyingType()
					conversionKind1 = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNullableTuple
					conversionKind2 = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNullableTuple
				End If
				Dim inferredType As TupleTypeSymbol = source.InferredType
				If (inferredType IsNot Nothing) Then
					flag = inferredType.IsSameTypeIgnoringAll(destination)
				Else
					flag = False
				End If
				If (flag) Then
					conversionKind = conversionKind1
				ElseIf (destination.IsTupleOrCompatibleWithTupleOfCardinality(arguments.Length)) Then
					Dim elementTypesOfTupleOrCompatible As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) = destination.GetElementTypesOfTupleOrCompatible()
					Dim conversionKind3 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = conversionKind1
					Dim conversionKind4 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
					Dim conversionKind5 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesNarrowingFromNumericConstant
					Dim conversionKind6 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
					Dim length As Integer = arguments.Length - 1
					Dim num As Integer = 0
					While num <= length
						Dim item As BoundExpression = arguments(num)
						Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = elementTypesOfTupleOrCompatible(num)
						If (item.HasErrors OrElse typeSymbol.IsErrorType()) Then
							conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
							Return conversionKind
						Else
							Dim keyValuePair As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.ConversionKind, MethodSymbol) = Conversions.ClassifyConversion(item, typeSymbol, binder, useSiteInfo)
							Dim key As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = keyValuePair.Key
							If (Not Conversions.NoConversion(key)) Then
								Dim conversionKind7 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = key And Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelMask
								If (conversionKind7 > conversionKind6) Then
									conversionKind6 = conversionKind7
								End If
								conversionKind4 = conversionKind4 Or key
								If (Conversions.IsNarrowingConversion(key)) Then
									conversionKind5 = conversionKind5 And key
									conversionKind3 = conversionKind2
								End If
								num = num + 1
							Else
								conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
								Return conversionKind
							End If
						End If
					End While
					conversionKind = conversionKind3 Or conversionKind4 And conversionKind5 Or conversionKind6
				Else
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
				End If
			Else
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Identity
			End If
			Return conversionKind
		End Function

		Private Shared Function ClassifyTupleConversion(ByVal source As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal destination As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			If (source.IsTupleType) Then
				Dim tupleElementTypes As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) = DirectCast(source, TupleTypeSymbol).TupleElementTypes
				If (destination.IsTupleOrCompatibleWithTupleOfCardinality(tupleElementTypes.Length)) Then
					Dim elementTypesOfTupleOrCompatible As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) = destination.GetElementTypesOfTupleOrCompatible()
					Dim conversionKind1 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTuple
					Dim conversionKind2 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
					Dim length As Integer = tupleElementTypes.Length - 1
					Dim num As Integer = 0
					While num <= length
						Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = tupleElementTypes(num)
						Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = elementTypesOfTupleOrCompatible(num)
						If (item.IsErrorType() OrElse typeSymbol.IsErrorType()) Then
							conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
							Return conversionKind
						Else
							Dim key As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ClassifyConversion(item, typeSymbol, useSiteInfo).Key
							If (Not Conversions.NoConversion(key)) Then
								Dim conversionKind3 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = key And Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelMask
								If (conversionKind3 > conversionKind2) Then
									conversionKind2 = conversionKind3
								End If
								If (Conversions.IsNarrowingConversion(key)) Then
									conversionKind1 = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTuple
								End If
								num = num + 1
							Else
								conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
								Return conversionKind
							End If
						End If
					End While
					conversionKind = conversionKind1 Or conversionKind2
				Else
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
				End If
			Else
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
			End If
			Return conversionKind
		End Function

		Private Shared Function ClassifyTypeParameterConversion(ByVal source As TypeSymbol, ByVal destination As TypeSymbol, ByVal varianceCompatibilityClassificationDepth As Integer, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim typeParameter As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			If (source.Kind = SymbolKind.TypeParameter) Then
				typeParameter = Conversions.ClassifyConversionFromTypeParameter(DirectCast(source, TypeParameterSymbol), destination, varianceCompatibilityClassificationDepth, useSiteInfo)
				If (Not Conversions.ConversionExists(typeParameter)) Then
					If (destination.Kind = SymbolKind.TypeParameter) Then
						typeParameter = Conversions.ClassifyConversionToTypeParameter(source, DirectCast(destination, TypeParameterSymbol), varianceCompatibilityClassificationDepth, useSiteInfo)
						If (Not Conversions.ConversionExists(typeParameter)) Then
							conversionKind = If(source.Kind = SymbolKind.TypeParameter OrElse destination.Kind = SymbolKind.TypeParameter, Microsoft.CodeAnalysis.VisualBasic.ConversionKind.MightSucceedAtRuntime, Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone)
							Return conversionKind
						End If
						conversionKind = typeParameter
						Return conversionKind
					End If
					conversionKind = If(source.Kind = SymbolKind.TypeParameter OrElse destination.Kind = SymbolKind.TypeParameter, Microsoft.CodeAnalysis.VisualBasic.ConversionKind.MightSucceedAtRuntime, Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone)
					Return conversionKind
				End If
				conversionKind = typeParameter
				Return conversionKind
			End If
			If (destination.Kind = SymbolKind.TypeParameter) Then
				typeParameter = Conversions.ClassifyConversionToTypeParameter(source, DirectCast(destination, TypeParameterSymbol), varianceCompatibilityClassificationDepth, useSiteInfo)
				If (Not Conversions.ConversionExists(typeParameter)) Then
					conversionKind = If(source.Kind = SymbolKind.TypeParameter OrElse destination.Kind = SymbolKind.TypeParameter, Microsoft.CodeAnalysis.VisualBasic.ConversionKind.MightSucceedAtRuntime, Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone)
					Return conversionKind
				End If
				conversionKind = typeParameter
				Return conversionKind
			End If
			conversionKind = If(source.Kind = SymbolKind.TypeParameter OrElse destination.Kind = SymbolKind.TypeParameter, Microsoft.CodeAnalysis.VisualBasic.ConversionKind.MightSucceedAtRuntime, Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone)
			Return conversionKind
		End Function

		Private Shared Function ClassifyUnboundLambdaConversion(ByVal source As UnboundLambda, ByVal destination As TypeSymbol) As ConversionKind
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.ConversionKind Microsoft.CodeAnalysis.VisualBasic.Conversions::ClassifyUnboundLambdaConversion(Microsoft.CodeAnalysis.VisualBasic.UnboundLambda,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.ConversionKind ClassifyUnboundLambdaConversion(Microsoft.CodeAnalysis.VisualBasic.UnboundLambda,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
			' 
			' File d'attente vide.
			'    à System.ThrowHelper.ThrowInvalidOperationException(ExceptionResource resource)
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.(ICollection`1 ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 525
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 445
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 363
			'    à Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\TypeInference\TypeInferer.cs:ligne 307
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 86
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Shared Function ClassifyUserDefinedConversion(ByVal source As TypeSymbol, ByVal destination As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As KeyValuePair(Of ConversionKind, MethodSymbol)
			Dim keyValuePair As KeyValuePair(Of ConversionKind, MethodSymbol)
			keyValuePair = If(Conversions.IsInterfaceType(source) OrElse Conversions.IsInterfaceType(destination) OrElse Not source.CanContainUserDefinedOperators(useSiteInfo) AndAlso Not destination.CanContainUserDefinedOperators(useSiteInfo), New KeyValuePair(Of ConversionKind, MethodSymbol)(), OverloadResolution.ResolveUserDefinedConversion(source, destination, useSiteInfo))
			Return keyValuePair
		End Function

		Private Shared Function ClassifyUserDefinedConversion(ByVal source As BoundExpression, ByVal destination As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As KeyValuePair(Of ConversionKind, MethodSymbol)
			' 
			' Current member / type: System.Collections.Generic.KeyValuePair`2<Microsoft.CodeAnalysis.VisualBasic.ConversionKind,Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol> Microsoft.CodeAnalysis.VisualBasic.Conversions::ClassifyUserDefinedConversion(Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.CompoundUseSiteInfo`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Collections.Generic.KeyValuePair<Microsoft.CodeAnalysis.VisualBasic.ConversionKind,Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol> ClassifyUserDefinedConversion(Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.CompoundUseSiteInfo<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Shared Function ClassifyValueTypeConversion(ByVal source As TypeSymbol, ByVal destination As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim current As NamedTypeSymbol
			If (source.SpecialType = SpecialType.System_Void OrElse destination.SpecialType = SpecialType.System_Void) Then
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
			Else
				If (Conversions.IsValueType(source)) Then
					If (Not source.IsRestrictedType()) Then
						If (destination.SpecialType = SpecialType.System_Object) Then
							conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningValue
							Return conversionKind
						ElseIf (Conversions.IsClassType(destination)) Then
							If (Not Conversions.IsDerivedFrom(source, destination, useSiteInfo)) Then
								GoTo Label1
							End If
							conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningValue
							Return conversionKind
						ElseIf (Conversions.IsInterfaceType(destination)) Then
							Dim variantCompatibleInterface As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Conversions.ToInterfaceConversionClassifier.ClassifyConversionToVariantCompatibleInterface(DirectCast(source, NamedTypeSymbol), DirectCast(destination, NamedTypeSymbol), 0, useSiteInfo)
							If (Not Conversions.ConversionExists(variantCompatibleInterface)) Then
								GoTo Label1
							End If
							conversionKind = variantCompatibleInterface Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Value
							Return conversionKind
						End If
					End If
				ElseIf (Conversions.IsValueType(destination)) Then
					If (source.SpecialType = SpecialType.System_Object) Then
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingValue
						Return conversionKind
					ElseIf (Conversions.IsClassType(source)) Then
						If (Not Conversions.IsDerivedFrom(destination, source, useSiteInfo)) Then
							GoTo Label1
						End If
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingValue
						Return conversionKind
					ElseIf (Conversions.IsInterfaceType(source)) Then
						Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = destination.AllInterfacesWithDefinitionUseSiteDiagnostics(useSiteInfo).GetEnumerator()
						Do
							If (Not enumerator.MoveNext()) Then
								GoTo Label1
							End If
							current = enumerator.Current
						Loop While current.IsErrorType() OrElse Not current.IsSameTypeIgnoringAll(source)
						conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingValue
						Return conversionKind
					End If
				End If
			Label1:
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
			End If
			Return conversionKind
		End Function

		Private Shared Function ClassOrBasesSatisfyConstraints(ByVal [class] As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByVal typeParam As TypeParameterSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Dim flag As Boolean
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = [class]
			While True
				If (typeSymbol Is Nothing) Then
					flag = False
					Exit While
				ElseIf (Not ConstraintsHelper.CheckConstraints(Nothing, Nothing, typeParam, typeSymbol, Nothing, useSiteInfo)) Then
					typeSymbol = typeSymbol.BaseTypeWithDefinitionUseSiteDiagnostics(useSiteInfo)
				Else
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		Public Shared Function CombineConversionRequirements(ByVal restriction1 As Microsoft.CodeAnalysis.VisualBasic.RequiredConversion, ByVal restriction2 As Microsoft.CodeAnalysis.VisualBasic.RequiredConversion) As Microsoft.CodeAnalysis.VisualBasic.RequiredConversion
			Dim requiredConversion As Microsoft.CodeAnalysis.VisualBasic.RequiredConversion
			If (restriction1 = restriction2) Then
				requiredConversion = restriction1
			ElseIf (restriction1 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.None) Then
				requiredConversion = restriction2
			ElseIf (restriction2 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.None) Then
				requiredConversion = restriction1
			ElseIf (restriction1 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Identity OrElse restriction2 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Identity) Then
				requiredConversion = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Identity
			ElseIf ((restriction1 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.AnyReverse OrElse restriction1 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.ReverseReference) AndAlso (restriction2 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.AnyReverse OrElse restriction2 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.ReverseReference)) Then
				requiredConversion = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.ReverseReference
			ElseIf ((restriction1 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Any OrElse restriction1 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.AnyReverse OrElse restriction1 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.AnyAndReverse) AndAlso (restriction2 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Any OrElse restriction2 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.AnyReverse OrElse restriction2 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.AnyAndReverse)) Then
				requiredConversion = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.AnyAndReverse
			ElseIf ((restriction1 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Any OrElse restriction1 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.ArrayElement) AndAlso (restriction2 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Any OrElse restriction2 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.ArrayElement)) Then
				requiredConversion = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.ArrayElement
			Else
				requiredConversion = If((restriction1 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Any OrElse restriction1 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.ArrayElement OrElse restriction1 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Reference) AndAlso (restriction2 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Any OrElse restriction2 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.ArrayElement OrElse restriction2 = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Reference), Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Reference, Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Identity)
			End If
			Return requiredConversion
		End Function

		Public Shared Function ConversionExists(ByVal conv As ConversionKind) As Boolean
			Return (conv And (ConversionKind.[Widening] Or ConversionKind.[Narrowing])) <> ConversionKind.DelegateRelaxationLevelNone
		End Function

		Public Shared Function DetermineDelegateRelaxationLevel(ByVal methodConversion As MethodConversionKind) As ConversionKind
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.ConversionKind Microsoft.CodeAnalysis.VisualBasic.Conversions::DetermineDelegateRelaxationLevel(Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.ConversionKind DetermineDelegateRelaxationLevel(Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Shared Function DetermineDelegateRelaxationLevelForLambdaReturn(ByVal expressionOpt As BoundExpression, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim methodConversionKind As Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind
			If (expressionOpt Is Nothing OrElse expressionOpt.Kind <> BoundKind.Conversion OrElse expressionOpt.HasErrors) Then
				conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
			Else
				Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundConversion = DirectCast(expressionOpt, Microsoft.CodeAnalysis.VisualBasic.BoundConversion)
				If (Not boundConversion.ExplicitCastInCode) Then
					Dim type As TypeSymbol = boundConversion.Operand.Type
					methodConversionKind = If(type IsNot Nothing, Conversions.ClassifyMethodConversionBasedOnReturnType(type, boundConversion.Type, False, useSiteInfo), Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind.Identity)
					conversionKind = Conversions.DetermineDelegateRelaxationLevel(methodConversionKind)
				Else
					conversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNone
				End If
			End If
			Return conversionKind
		End Function

		Public Shared Function FailedDueToNumericOverflow(ByVal conv As ConversionKind) As Boolean
			Return (conv And (ConversionKind.FailedDueToNumericOverflow Or ConversionKind.[Widening] Or ConversionKind.[Narrowing])) = ConversionKind.FailedDueToNumericOverflow
		End Function

		Public Shared Function FailedDueToQueryLambdaBodyMismatch(ByVal conv As ConversionKind) As Boolean
			Return (conv And (ConversionKind.FailedDueToQueryLambdaBodyMismatch Or ConversionKind.[Widening] Or ConversionKind.[Narrowing])) = ConversionKind.FailedDueToQueryLambdaBodyMismatch
		End Function

		Private Shared Function FastClassifyPredefinedConversion(ByVal source As TypeSymbol, ByVal target As TypeSymbol) As Nullable(Of ConversionKind)
			Return Conversions.ConversionEasyOut.ClassifyPredefinedConversion(source, target)
		End Function

		Private Shared Function GetNonErrorEnumUnderlyingType(ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			If (type.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Enum]) Then
				Dim enumUnderlyingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).EnumUnderlyingType
				If (enumUnderlyingType.Kind = SymbolKind.ErrorType) Then
					namedTypeSymbol = Nothing
					Return namedTypeSymbol
				End If
				namedTypeSymbol = enumUnderlyingType
				Return namedTypeSymbol
			End If
			namedTypeSymbol = Nothing
			Return namedTypeSymbol
		End Function

		Private Shared Function GetNonInterfaceTypeConstraintOrSelf(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim nonInterfaceConstraint As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			If (type.Kind <> SymbolKind.TypeParameter) Then
				nonInterfaceConstraint = type
			Else
				Dim typeParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeParameterSymbol)
				If (Not typeParameterSymbol.HasValueTypeConstraint) Then
					nonInterfaceConstraint = typeParameterSymbol.GetNonInterfaceConstraint(useSiteInfo)
				Else
					Dim specialType As NamedTypeSymbol = typeParameterSymbol.ContainingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_ValueType)
					If (specialType.Kind = SymbolKind.ErrorType) Then
						typeSymbol = Nothing
					Else
						typeSymbol = specialType
					End If
					nonInterfaceConstraint = typeSymbol
				End If
			End If
			Return nonInterfaceConstraint
		End Function

		Private Shared Function GetValueTypeConstraint(ByVal typeParam As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim nonInterfaceConstraint As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = DirectCast(typeParam, TypeParameterSymbol).GetNonInterfaceConstraint(useSiteInfo)
			If (nonInterfaceConstraint Is Nothing OrElse Not nonInterfaceConstraint.IsValueType) Then
				typeSymbol = Nothing
			Else
				typeSymbol = nonInterfaceConstraint
			End If
			Return typeSymbol
		End Function

		Public Shared Function HasWideningDirectCastConversionButNotEnumTypeConversion(ByVal source As TypeSymbol, ByVal destination As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Conversions::HasWideningDirectCastConversionButNotEnumTypeConversion(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.CompoundUseSiteInfo`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean HasWideningDirectCastConversionButNotEnumTypeConversion(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.CompoundUseSiteInfo<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Shared Function InvertConversionRequirement(ByVal restriction As Microsoft.CodeAnalysis.VisualBasic.RequiredConversion) As Microsoft.CodeAnalysis.VisualBasic.RequiredConversion
			Dim requiredConversion As Microsoft.CodeAnalysis.VisualBasic.RequiredConversion
			If (restriction = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.AnyReverse) Then
				requiredConversion = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Any
			ElseIf (restriction = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.ReverseReference) Then
				requiredConversion = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Reference
			ElseIf (restriction = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Any) Then
				requiredConversion = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.AnyReverse
			ElseIf (restriction <> Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.ArrayElement) Then
				requiredConversion = If(restriction <> Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Reference, restriction, Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.ReverseReference)
			Else
				requiredConversion = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.ReverseReference
			End If
			Return requiredConversion
		End Function

		Private Shared Function IsArrayType(ByVal type As TypeSymbol) As Boolean
			Return type.TypeKind = Microsoft.CodeAnalysis.TypeKind.Array
		End Function

		Private Shared Function IsClassType(ByVal type As TypeSymbol) As Boolean
			Dim typeKind As Microsoft.CodeAnalysis.TypeKind = type.TypeKind
			If (typeKind = Microsoft.CodeAnalysis.TypeKind.[Class] OrElse typeKind = Microsoft.CodeAnalysis.TypeKind.[Module]) Then
				Return True
			End If
			Return typeKind = Microsoft.CodeAnalysis.TypeKind.[Delegate]
		End Function

		Public Shared Function IsCLRPredefinedConversion(ByVal conversion As ConversionKind) As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Conversions::IsCLRPredefinedConversion(Microsoft.CodeAnalysis.VisualBasic.ConversionKind)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean IsCLRPredefinedConversion(Microsoft.CodeAnalysis.VisualBasic.ConversionKind)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Shared Function IsDelegateRelaxationSupportedFor(ByVal methodConversion As MethodConversionKind) As Boolean
			Return (methodConversion And MethodConversionKind.AllErrorReasons) = MethodConversionKind.Identity
		End Function

		Private Shared Function IsDelegateType(ByVal type As TypeSymbol) As Boolean
			Return type.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Delegate]
		End Function

		Public Shared Function IsDerivedFrom(ByVal derivedType As TypeSymbol, ByVal baseType As TypeSymbol, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
			Return baseType.IsBaseTypeOf(derivedType, useSiteInfo)
		End Function

		Public Shared Function IsIdentityConversion(ByVal conv As ConversionKind) As Boolean
			Return (conv And ConversionKind.Identity) = ConversionKind.Identity
		End Function

		Private Shared Function IsInterfaceType(ByVal type As TypeSymbol) As Boolean
			Return type.IsInterfaceType()
		End Function

		Public Shared Function IsNarrowingConversion(ByVal conv As ConversionKind) As Boolean
			Return (conv And ConversionKind.[Narrowing]) <> ConversionKind.DelegateRelaxationLevelNone
		End Function

		Public Shared Function IsNarrowingMethodConversion(ByVal methodConversion As Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind, ByVal isForAddressOf As Boolean) As Boolean
			Dim methodConversionKind As Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind
			methodConversionKind = If(Not isForAddressOf, Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind.OneArgumentIsNarrowing Or Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind.ReturnIsWidening, Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind.OneArgumentIsNarrowing Or Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind.ReturnIsWidening Or Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind.AllArgumentsIgnored)
			Return (methodConversion And methodConversionKind) <> Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind.Identity
		End Function

		Public Shared Function IsStubRequiredForMethodConversion(ByVal methodConversions As MethodConversionKind) As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Conversions::IsStubRequiredForMethodConversion(Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean IsStubRequiredForMethodConversion(Microsoft.CodeAnalysis.VisualBasic.MethodConversionKind)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Shared Function IsValueType(ByVal type As TypeSymbol) As Boolean
			Dim typeKind As Microsoft.CodeAnalysis.TypeKind = type.TypeKind
			If (typeKind = Microsoft.CodeAnalysis.TypeKind.[Enum]) Then
				Return True
			End If
			Return typeKind = Microsoft.CodeAnalysis.TypeKind.Struct
		End Function

		Public Shared Function IsWideningConversion(ByVal conv As ConversionKind) As Boolean
			Return (conv And ConversionKind.[Widening]) <> ConversionKind.DelegateRelaxationLevelNone
		End Function

		Public Shared Function NoConversion(ByVal conv As ConversionKind) As Boolean
			Return (conv And (ConversionKind.[Widening] Or ConversionKind.[Narrowing])) = ConversionKind.DelegateRelaxationLevelNone
		End Function

		Public Shared Function StrengthenConversionRequirementToReference(ByVal restriction As Microsoft.CodeAnalysis.VisualBasic.RequiredConversion) As Microsoft.CodeAnalysis.VisualBasic.RequiredConversion
			Dim requiredConversion As Microsoft.CodeAnalysis.VisualBasic.RequiredConversion
			If (restriction = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.AnyReverse) Then
				requiredConversion = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.ReverseReference
			ElseIf (restriction = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Any OrElse restriction = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.ArrayElement) Then
				requiredConversion = Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Reference
			Else
				requiredConversion = If(restriction <> Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.AnyAndReverse, restriction, Microsoft.CodeAnalysis.VisualBasic.RequiredConversion.Identity)
			End If
			Return requiredConversion
		End Function

		Public Shared Function TryFoldConstantConversion(ByVal source As BoundExpression, ByVal destination As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol, ByRef integerOverflow As Boolean) As Microsoft.CodeAnalysis.ConstantValue
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
			Dim chr As Char
			Dim stringValue As String
			integerOverflow = False
			Dim constantValueOpt As Microsoft.CodeAnalysis.ConstantValue = source.ConstantValueOpt
			If (constantValueOpt Is Nothing OrElse constantValueOpt.IsBad) Then
				constantValue = Nothing
			ElseIf (Not destination.AllowsCompileTimeConversions()) Then
				constantValue = Nothing
			ElseIf (Not source.IsNothingLiteral()) Then
				Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = source.Type
				If (type.AllowsCompileTimeConversions()) Then
					Dim enumUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = type.GetEnumUnderlyingTypeOrSelf()
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = destination.GetEnumUnderlyingTypeOrSelf()
					If (CObj(enumUnderlyingTypeOrSelf) <> CObj(typeSymbol)) Then
						If (enumUnderlyingTypeOrSelf.IsStringType()) Then
							If (Not typeSymbol.IsCharType()) Then
								GoTo Label1
							End If
							If (constantValueOpt.IsNothing) Then
								stringValue = Nothing
							Else
								stringValue = constantValueOpt.StringValue
							End If
							Dim str As String = stringValue
							chr = If(str Is Nothing OrElse str.Length = 0, Strings.ChrW(0), str(0))
							constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(chr)
							Return constantValue
						ElseIf (Not enumUnderlyingTypeOrSelf.IsCharType()) Then
							constantValue = Conversions.TryFoldConstantNumericOrBooleanConversion(constantValueOpt, enumUnderlyingTypeOrSelf, typeSymbol, integerOverflow)
							Return constantValue
						Else
							If (Not typeSymbol.IsStringType()) Then
								GoTo Label1
							End If
							constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(New [String](constantValueOpt.CharValue, 1))
							Return constantValue
						End If
					Label1:
						constantValue = Nothing
					Else
						constantValue = constantValueOpt
					End If
				Else
					constantValue = Nothing
				End If
			Else
				constantValue = If(Not destination.IsStringType(), Microsoft.CodeAnalysis.ConstantValue.[Default](destination.GetConstantValueTypeDiscriminator()), source.ConstantValueOpt)
			End If
			Return constantValue
		End Function

		Private Shared Function TryFoldConstantNumericOrBooleanConversion(ByRef sourceValue As Microsoft.CodeAnalysis.ConstantValue, ByVal sourceType As TypeSymbol, ByVal targetType As TypeSymbol, ByRef integerOverflow As Boolean) As Microsoft.CodeAnalysis.ConstantValue
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
			integerOverflow = False
			If (sourceType.IsIntegralType() OrElse sourceType.IsBooleanType()) Then
				If (Not targetType.IsNumericType() AndAlso Not targetType.IsBooleanType()) Then
					constantValue = Nothing
					Return constantValue
				End If
				Dim constantValueAsInt64 As Long = CompileTimeCalculations.GetConstantValueAsInt64(sourceValue)
				If (sourceType.IsBooleanType() AndAlso constantValueAsInt64 <> 0) Then
					If (Not targetType.IsUnsignedIntegralType()) Then
						constantValueAsInt64 = CLng(-1)
					Else
						Dim flag As Boolean = False
						constantValueAsInt64 = CompileTimeCalculations.NarrowIntegralResult(CLng(-1), sourceType, targetType, flag)
					End If
				End If
				constantValue = CompileTimeCalculations.ConvertIntegralValue(constantValueAsInt64, sourceValue.Discriminator, targetType.GetConstantValueTypeDiscriminator(), integerOverflow)
				Return constantValue
			ElseIf (Not sourceType.IsFloatingType()) Then
				If (Not sourceType.IsDecimalType() OrElse Not targetType.IsNumericType() AndAlso Not targetType.IsBooleanType()) Then
					constantValue = Nothing
					Return constantValue
				End If
				Dim constantValue1 As Microsoft.CodeAnalysis.ConstantValue = CompileTimeCalculations.ConvertDecimalValue(sourceValue.DecimalValue, targetType.GetConstantValueTypeDiscriminator(), integerOverflow)
				If (constantValue1.IsBad) Then
					integerOverflow = False
				End If
				constantValue = constantValue1
				Return constantValue
			Else
				If (Not targetType.IsNumericType() AndAlso Not targetType.IsBooleanType()) Then
					constantValue = Nothing
					Return constantValue
				End If
				Dim constantValue2 As Microsoft.CodeAnalysis.ConstantValue = CompileTimeCalculations.ConvertFloatingValue(If(sourceValue.Discriminator = ConstantValueTypeDiscriminator.[Double], sourceValue.DoubleValue, CDbl(sourceValue.SingleValue)), targetType.GetConstantValueTypeDiscriminator(), integerOverflow)
				If (constantValue2.IsBad) Then
					integerOverflow = False
				End If
				constantValue = constantValue2
				Return constantValue
			End If
			constantValue = Nothing
			Return constantValue
		End Function

		Public Shared Function TryFoldNothingReferenceConversion(ByVal source As BoundExpression, ByVal conversion As ConversionKind, ByVal targetType As TypeSymbol) As ConstantValue
			Return Conversions.TryFoldNothingReferenceConversion(source.ConstantValueOpt, conversion, targetType)
		End Function

		Friend Shared Function TryFoldNothingReferenceConversion(ByVal sourceValue As Microsoft.CodeAnalysis.ConstantValue, ByVal conversion As ConversionKind, ByVal targetType As TypeSymbol) As Microsoft.CodeAnalysis.ConstantValue
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
			If (sourceValue Is Nothing OrElse Not sourceValue.IsNothing OrElse targetType.IsTypeParameter() OrElse Not targetType.IsReferenceType) Then
				constantValue = Nothing
			ElseIf (conversion = ConversionKind.WideningNothingLiteral OrElse Conversions.IsIdentityConversion(conversion) OrElse (conversion And ConversionKind.WideningReference) = ConversionKind.WideningReference OrElse (conversion And ConversionKind.WideningArray) = ConversionKind.WideningArray) Then
				constantValue = sourceValue
			Else
				constantValue = Nothing
			End If
			Return constantValue
		End Function

		Friend Class ConversionEasyOut
			Private ReadOnly Shared s_convkind As Integer(,)

			Shared Sub New()
				Conversions.ConversionEasyOut.s_convkind = New Integer(,) { { 5, 34, 258, 258, 258, 258, 258, 258, 258, 258, 258, 258, 258, 258, 258, 258 }, { 33, 5, 514, 514, 514, 514, 514, 514, 514, 514, 514, 514, 514, 514, 514, 514 }, { 257, 514, 5, 0, 1026, 1026, 1026, 1026, 1026, 1026, 1026, 1026, 1026, 1026, 1026, 0 }, { 257, 513, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 257, 514, 1026, 0, 5, 9, 9, 9, 10, 10, 10, 10, 9, 9, 9, 0 }, { 257, 514, 1026, 0, 10, 5, 9, 9, 10, 10, 10, 10, 9, 9, 9, 0 }, { 257, 514, 1026, 0, 10, 10, 5, 9, 10, 10, 10, 10, 9, 9, 9, 0 }, { 257, 514, 1026, 0, 10, 10, 10, 5, 10, 10, 10, 10, 9, 9, 9, 0 }, { 257, 514, 1026, 0, 10, 9, 9, 9, 5, 9, 9, 9, 9, 9, 9, 0 }, { 257, 514, 1026, 0, 10, 10, 9, 9, 10, 5, 9, 9, 9, 9, 9, 0 }, { 257, 514, 1026, 0, 10, 10, 10, 9, 10, 10, 5, 9, 9, 9, 9, 0 }, { 257, 514, 1026, 0, 10, 10, 10, 10, 10, 10, 10, 5, 9, 9, 9, 0 }, { 257, 514, 1026, 0, 10, 10, 10, 10, 10, 10, 10, 10, 5, 9, 10, 0 }, { 257, 514, 1026, 0, 10, 10, 10, 10, 10, 10, 10, 10, 10, 5, 10, 0 }, { 257, 514, 1026, 0, 10, 10, 10, 10, 10, 10, 10, 10, 9, 9, 5, 0 }, { 257, 514, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5 } }
			End Sub

			Public Sub New()
				MyBase.New()
			End Sub

			Public Shared Function ClassifyPredefinedConversion(ByVal source As TypeSymbol, ByVal target As TypeSymbol) As Nullable(Of ConversionKind)
				' 
				' Current member / type: System.Nullable`1<Microsoft.CodeAnalysis.VisualBasic.ConversionKind> Microsoft.CodeAnalysis.VisualBasic.Conversions/ConversionEasyOut::ClassifyPredefinedConversion(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Nullable<Microsoft.CodeAnalysis.VisualBasic.ConversionKind> ClassifyPredefinedConversion(Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
				'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
				'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
				'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Function
		End Class

		Private Structure ToInterfaceConversionClassifier
			Private _conv As ConversionKind

			Private _match As NamedTypeSymbol

			Public ReadOnly Property Result As ConversionKind
				Get
					Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
					conversionKind = If(Not Conversions.IsIdentityConversion(Me._conv), Me._conv, Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Widening])
					Return conversionKind
				End Get
			End Property

			Public Function AccumulateConversionClassificationToVariantCompatibleInterface(ByVal source As NamedTypeSymbol, ByVal destination As NamedTypeSymbol, ByVal varianceCompatibilityClassificationDepth As Integer, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
				Dim flag As Boolean
				If (Not Conversions.IsIdentityConversion(Me._conv)) Then
					If (Conversions.IsInterfaceType(source)) Then
						Me.ClassifyInterfaceImmediateVarianceCompatibility(source, destination, varianceCompatibilityClassificationDepth, useSiteInfo)
					End If
					Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = source.AllInterfacesWithDefinitionUseSiteDiagnostics(useSiteInfo).GetEnumerator()
					While enumerator.MoveNext()
						Dim current As NamedTypeSymbol = enumerator.Current
						If (current.IsErrorType() OrElse Not Me.ClassifyInterfaceImmediateVarianceCompatibility(current, destination, varianceCompatibilityClassificationDepth, useSiteInfo)) Then
							Continue While
						End If
						flag = True
						Return flag
					End While
					flag = False
				Else
					flag = True
				End If
				Return flag
			End Function

			<Conditional("DEBUG")>
			Public Sub AssertFoundIdentity()
			End Sub

			Public Shared Function ClassifyConversionToVariantCompatibleInterface(ByVal source As NamedTypeSymbol, ByVal destination As NamedTypeSymbol, ByVal varianceCompatibilityClassificationDepth As Integer, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ConversionKind
				Dim toInterfaceConversionClassifier As Conversions.ToInterfaceConversionClassifier = New Conversions.ToInterfaceConversionClassifier()
				toInterfaceConversionClassifier.AccumulateConversionClassificationToVariantCompatibleInterface(source, destination, varianceCompatibilityClassificationDepth, useSiteInfo)
				Return toInterfaceConversionClassifier.Result
			End Function

			Private Function ClassifyInterfaceImmediateVarianceCompatibility(ByVal source As NamedTypeSymbol, ByVal destination As NamedTypeSymbol, ByVal varianceCompatibilityClassificationDepth As Integer, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As Boolean
				' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Conversions/ToInterfaceConversionClassifier::ClassifyInterfaceImmediateVarianceCompatibility(Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol,System.Int32,Microsoft.CodeAnalysis.CompoundUseSiteInfo`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean ClassifyInterfaceImmediateVarianceCompatibility(Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol,System.Int32,Microsoft.CodeAnalysis.CompoundUseSiteInfo<Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol>&)
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
				'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
				'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
				'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Function
		End Structure
	End Class
End Namespace