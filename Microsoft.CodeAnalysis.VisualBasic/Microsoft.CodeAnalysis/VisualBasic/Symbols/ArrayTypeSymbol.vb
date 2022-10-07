Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Reflection.Metadata
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class ArrayTypeSymbol
		Inherits TypeSymbol
		Implements IArrayTypeReference, IArrayTypeSymbol
		Friend ReadOnly Property AdaptedArrayTypeSymbol As ArrayTypeSymbol
			Get
				Return Me
			End Get
		End Property

		Friend Overrides ReadOnly Property BaseTypeNoUseSiteDiagnostics As NamedTypeSymbol

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Nothing
			End Get
		End Property

		Public MustOverride ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.NotApplicable
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Public MustOverride ReadOnly Property ElementType As TypeSymbol

		Friend MustOverride ReadOnly Property HasDefaultSizesAndLowerBounds As Boolean

		ReadOnly Property IArrayTypeReferenceIsSZArray As Boolean Implements IArrayTypeReference.IsSZArray
			Get
				Return Me.AdaptedArrayTypeSymbol.IsSZArray
			End Get
		End Property

		ReadOnly Property IArrayTypeReferenceLowerBounds As ImmutableArray(Of Integer) Implements IArrayTypeReference.LowerBounds
			Get
				Return Me.AdaptedArrayTypeSymbol.LowerBounds
			End Get
		End Property

		ReadOnly Property IArrayTypeReferenceRank As Integer Implements IArrayTypeReference.Rank
			Get
				Return Me.AdaptedArrayTypeSymbol.Rank
			End Get
		End Property

		ReadOnly Property IArrayTypeReferenceSizes As ImmutableArray(Of Integer) Implements IArrayTypeReference.Sizes
			Get
				Return Me.AdaptedArrayTypeSymbol.Sizes
			End Get
		End Property

		ReadOnly Property IArrayTypeSymbol_CustomModifiers As ImmutableArray(Of CustomModifier) Implements IArrayTypeSymbol.CustomModifiers
			Get
				Return Me.CustomModifiers
			End Get
		End Property

		ReadOnly Property IArrayTypeSymbol_ElementNullableAnnotation As Microsoft.CodeAnalysis.NullableAnnotation Implements IArrayTypeSymbol.ElementNullableAnnotation
			Get
				Return Microsoft.CodeAnalysis.NullableAnnotation.None
			End Get
		End Property

		ReadOnly Property IArrayTypeSymbol_ElementType As ITypeSymbol Implements IArrayTypeSymbol.ElementType
			Get
				Return Me.ElementType
			End Get
		End Property

		ReadOnly Property IArrayTypeSymbol_IsSZArray As Boolean Implements IArrayTypeSymbol.IsSZArray
			Get
				Return Me.IsSZArray
			End Get
		End Property

		ReadOnly Property IArrayTypeSymbol_LowerBounds As ImmutableArray(Of Integer) Implements IArrayTypeSymbol.LowerBounds
			Get
				Return Me.LowerBounds
			End Get
		End Property

		ReadOnly Property IArrayTypeSymbol_Rank As Integer Implements IArrayTypeSymbol.Rank
			Get
				Return Me.Rank
			End Get
		End Property

		ReadOnly Property IArrayTypeSymbol_Sizes As ImmutableArray(Of Integer) Implements IArrayTypeSymbol.Sizes
			Get
				Return Me.Sizes
			End Get
		End Property

		Public Overrides ReadOnly Property IsReferenceType As Boolean
			Get
				Return True
			End Get
		End Property

		Friend MustOverride ReadOnly Property IsSZArray As Boolean

		Public Overrides ReadOnly Property IsValueType As Boolean
			Get
				Return False
			End Get
		End Property

		ReadOnly Property ITypeReferenceAsGenericMethodParameterReference As IGenericMethodParameterReference
			Get
				Return Nothing
			End Get
		End Property

		ReadOnly Property ITypeReferenceAsGenericTypeInstanceReference As IGenericTypeInstanceReference
			Get
				Return Nothing
			End Get
		End Property

		ReadOnly Property ITypeReferenceAsGenericTypeParameterReference As IGenericTypeParameterReference
			Get
				Return Nothing
			End Get
		End Property

		ReadOnly Property ITypeReferenceAsNamespaceTypeReference As INamespaceTypeReference
			Get
				Return Nothing
			End Get
		End Property

		ReadOnly Property ITypeReferenceAsNestedTypeReference As INestedTypeReference
			Get
				Return Nothing
			End Get
		End Property

		ReadOnly Property ITypeReferenceAsSpecializedNestedTypeReference As ISpecializedNestedTypeReference
			Get
				Return Nothing
			End Get
		End Property

		ReadOnly Property ITypeReferenceIsEnum As Boolean
			Get
				Return False
			End Get
		End Property

		ReadOnly Property ITypeReferenceIsValueType As Boolean
			Get
				Return False
			End Get
		End Property

		ReadOnly Property ITypeReferenceTypeCode As Microsoft.Cci.PrimitiveTypeCode
			Get
				Return Microsoft.Cci.PrimitiveTypeCode.NotPrimitive
			End Get
		End Property

		ReadOnly Property ITypeReferenceTypeDef As TypeDefinitionHandle
			Get
				Return New TypeDefinitionHandle()
			End Get
		End Property

		Public Overrides ReadOnly Property Kind As SymbolKind
			Get
				Return SymbolKind.ArrayType
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return ImmutableArray(Of Location).Empty
			End Get
		End Property

		Friend Overridable ReadOnly Property LowerBounds As ImmutableArray(Of Integer)
			Get
				Return New ImmutableArray(Of Integer)()
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Public MustOverride ReadOnly Property Rank As Integer

		Friend Overridable ReadOnly Property Sizes As ImmutableArray(Of Integer)
			Get
				Return ImmutableArray(Of Integer).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind
			Get
				Return Microsoft.CodeAnalysis.TypeKind.Array
			End Get
		End Property

		Protected Sub New()
			MyBase.New()
		End Sub

		Friend Overrides Function Accept(Of TArgument, TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TArgument, TResult), ByVal arg As TArgument) As TResult
			Return visitor.VisitArrayType(Me, arg)
		End Function

		Public Overrides Sub Accept(ByVal visitor As SymbolVisitor)
			visitor.VisitArrayType(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As SymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitArrayType(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As VisualBasicSymbolVisitor)
			visitor.VisitArrayType(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitArrayType(Me)
		End Function

		Friend Shared Function CreateMDArray(ByVal elementType As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal rank As Integer, ByVal sizes As ImmutableArray(Of Integer), ByVal lowerBounds As ImmutableArray(Of Integer), ByVal declaringAssembly As AssemblySymbol) As ArrayTypeSymbol
			Dim mDArrayWithSizesAndBound As ArrayTypeSymbol
			Dim specialType As NamedTypeSymbol = declaringAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Array)
			If (Not sizes.IsDefaultOrEmpty OrElse Not lowerBounds.IsDefault) Then
				mDArrayWithSizesAndBound = New ArrayTypeSymbol.MDArrayWithSizesAndBounds(elementType, customModifiers, rank, sizes, lowerBounds, specialType)
			Else
				mDArrayWithSizesAndBound = New ArrayTypeSymbol.MDArrayNoSizesOrBounds(elementType, customModifiers, rank, specialType)
			End If
			Return mDArrayWithSizesAndBound
		End Function

		Friend Shared Function CreateSZArray(ByVal elementType As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal compilation As VisualBasicCompilation) As ArrayTypeSymbol
			Return ArrayTypeSymbol.CreateSZArray(elementType, customModifiers, compilation.Assembly)
		End Function

		Friend Shared Function CreateSZArray(ByVal elementType As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal declaringAssembly As AssemblySymbol) As ArrayTypeSymbol
			Return New ArrayTypeSymbol.SZArray(elementType, customModifiers, declaringAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Array), ArrayTypeSymbol.GetSZArrayInterfaces(elementType, declaringAssembly))
		End Function

		Friend Shared Function CreateVBArray(ByVal elementType As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal rank As Integer, ByVal compilation As VisualBasicCompilation) As ArrayTypeSymbol
			Return ArrayTypeSymbol.CreateVBArray(elementType, customModifiers, rank, compilation.Assembly)
		End Function

		Friend Shared Function CreateVBArray(ByVal elementType As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal rank As Integer, ByVal declaringAssembly As AssemblySymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol
			Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol
			If (rank <> 1) Then
				Dim nums As ImmutableArray(Of Integer) = New ImmutableArray(Of Integer)()
				Dim nums1 As ImmutableArray(Of Integer) = nums
				nums = New ImmutableArray(Of Integer)()
				arrayTypeSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol.CreateMDArray(elementType, customModifiers, rank, nums1, nums, declaringAssembly)
			Else
				arrayTypeSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol.CreateSZArray(elementType, customModifiers, declaringAssembly)
			End If
			Return arrayTypeSymbol
		End Function

		Public NotOverridable Overrides Function Equals(ByVal other As TypeSymbol, ByVal comparison As TypeCompareKind) As Boolean
			Return Me.Equals(TryCast(other, ArrayTypeSymbol), comparison)
		End Function

		Public Function Equals(ByVal other As ArrayTypeSymbol, ByVal compareKind As TypeCompareKind) As Boolean
			' 
			' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol::Equals(Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol,Microsoft.CodeAnalysis.TypeCompareKind)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Boolean Equals(Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol,Microsoft.CodeAnalysis.TypeCompareKind)
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

		Friend Shadows Function GetCciAdapter() As ArrayTypeSymbol
			Return Me
		End Function

		Public NotOverridable Overrides Function GetHashCode() As Integer
			Dim num As Integer = 0
			Dim elementType As TypeSymbol = Me
			While elementType.TypeKind = Microsoft.CodeAnalysis.TypeKind.Array
				Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(elementType, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
				num = Hash.Combine(arrayTypeSymbol.Rank, num)
				elementType = arrayTypeSymbol.ElementType
			End While
			Return Hash.Combine(Of TypeSymbol)(elementType, num)
		End Function

		Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
			Return ImmutableArray(Of Symbol).Empty
		End Function

		Public Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
			Return ImmutableArray(Of Symbol).Empty
		End Function

		Private Shared Function GetSZArrayInterfaces(ByVal elementType As TypeSymbol, ByVal declaringAssembly As AssemblySymbol) As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			Dim empty As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			Dim specialType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = declaringAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IList_T)
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = declaringAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Collections_Generic_IReadOnlyList_T)
			If (Not specialType.IsErrorType()) Then
				empty = If(Not namedTypeSymbol.IsErrorType(), ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)(specialType.Construct(New TypeSymbol() { elementType }), namedTypeSymbol.Construct(New TypeSymbol() { elementType })), ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)(specialType.Construct(New TypeSymbol() { elementType })))
			ElseIf (namedTypeSymbol.IsErrorType()) Then
				empty = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).Empty
			Else
				empty = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)(namedTypeSymbol.Construct(New TypeSymbol() { elementType }))
			End If
			Return empty
		End Function

		Public Overrides Function GetTypeMembers() As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String, ByVal arity As Integer) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Friend Overrides Function GetUnificationUseSiteDiagnosticRecursive(ByVal owner As Symbol, ByRef checkedTypes As HashSet(Of TypeSymbol)) As DiagnosticInfo
			Dim unificationUseSiteDiagnosticRecursive As Object = Me.ElementType.GetUnificationUseSiteDiagnosticRecursive(owner, checkedTypes)
			If (unificationUseSiteDiagnosticRecursive Is Nothing) Then
				If (Me.BaseTypeNoUseSiteDiagnostics IsNot Nothing) Then
					unificationUseSiteDiagnosticRecursive = Me.BaseTypeNoUseSiteDiagnostics.GetUnificationUseSiteDiagnosticRecursive(owner, checkedTypes)
				Else
					unificationUseSiteDiagnosticRecursive = Nothing
				End If
				If (unificationUseSiteDiagnosticRecursive Is Nothing) Then
					unificationUseSiteDiagnosticRecursive = If(Symbol.GetUnificationUseSiteDiagnosticRecursive(Of NamedTypeSymbol)(Me.InterfacesNoUseSiteDiagnostics, owner, checkedTypes), Symbol.GetUnificationUseSiteDiagnosticRecursive(Me.CustomModifiers, owner, checkedTypes))
				End If
			End If
			Return unificationUseSiteDiagnosticRecursive
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol)
			Dim code As Boolean
			Dim useSiteInfo1 As UseSiteInfo(Of AssemblySymbol) = MyBase.DeriveUseSiteInfoFromType(Me.ElementType)
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = useSiteInfo1.DiagnosticInfo
			If (diagnosticInfo IsNot Nothing) Then
				code = diagnosticInfo.Code = 30649
			Else
				code = False
			End If
			If (Not code) Then
				Dim useSiteInfo2 As UseSiteInfo(Of AssemblySymbol) = MyBase.DeriveUseSiteInfoFromCustomModifiers(Me.CustomModifiers, False)
				useSiteInfo = MyBase.MergeUseSiteInfo(useSiteInfo1, useSiteInfo2)
			Else
				useSiteInfo = useSiteInfo1
			End If
			Return useSiteInfo
		End Function

		Friend Function HasSameShapeAs(ByVal other As ArrayTypeSymbol) As Boolean
			If (Me.Rank <> other.Rank) Then
				Return False
			End If
			Return Me.IsSZArray = other.IsSZArray
		End Function

		Friend Function HasSameSizesAndLowerBoundsAs(ByVal other As ArrayTypeSymbol) As Boolean
			Dim isDefault As Boolean
			If (Not System.Linq.ImmutableArrayExtensions.SequenceEqual(Of Integer, Integer)(Me.Sizes, other.Sizes, DirectCast(Nothing, IEqualityComparer(Of Integer)))) Then
				isDefault = False
			Else
				Dim lowerBounds As ImmutableArray(Of Integer) = Me.LowerBounds
				If (Not lowerBounds.IsDefault) Then
					Dim nums As ImmutableArray(Of Integer) = other.LowerBounds
					isDefault = If(nums.IsDefault, False, System.Linq.ImmutableArrayExtensions.SequenceEqual(Of Integer, Integer)(lowerBounds, nums, DirectCast(Nothing, IEqualityComparer(Of Integer))))
				Else
					isDefault = other.LowerBounds.IsDefault
				End If
			End If
			Return isDefault
		End Function

		Private Function IArrayTypeReferenceGetElementType(ByVal context As EmitContext) As ITypeReference Implements IArrayTypeReference.GetElementType
			Dim modifiedTypeReference As ITypeReference
			Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
			Dim customModifiers As ImmutableArray(Of CustomModifier) = Me.AdaptedArrayTypeSymbol.CustomModifiers
			Dim typeReference As ITypeReference = [module].Translate(Me.AdaptedArrayTypeSymbol.ElementType, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics)
			If (customModifiers.Length <> 0) Then
				modifiedTypeReference = New Microsoft.Cci.ModifiedTypeReference(typeReference, customModifiers.[As](Of ICustomModifier)())
			Else
				modifiedTypeReference = typeReference
			End If
			Return modifiedTypeReference
		End Function

		Private Function IArrayTypeSymbol_Equals(ByVal symbol As IArrayTypeSymbol) As Boolean Implements IArrayTypeSymbol.Equals
			Return MyBase.Equals(TryCast(symbol, ArrayTypeSymbol))
		End Function

		Friend Overrides MustOverride Function InternalSubstituteTypeParameters(ByVal substitution As TypeSubstitution) As TypeWithModifiers

		Friend NotOverridable Overrides Function IReferenceAsDefinition(ByVal context As EmitContext) As IDefinition
			Return Nothing
		End Function

		Friend NotOverridable Overrides Sub IReferenceDispatch(ByVal visitor As MetadataVisitor)
			visitor.Visit(Me)
		End Sub

		Private Function ITypeReferenceAsNamespaceTypeDefinition(ByVal context As EmitContext) As INamespaceTypeDefinition
			Return Nothing
		End Function

		Private Function ITypeReferenceAsNestedTypeDefinition(ByVal context As EmitContext) As INestedTypeDefinition
			Return Nothing
		End Function

		Private Function ITypeReferenceAsTypeDefinition(ByVal context As EmitContext) As ITypeDefinition
			Return Nothing
		End Function

		Private Function ITypeReferenceGetResolvedType(ByVal context As EmitContext) As ITypeDefinition
			Return Nothing
		End Function

		Friend MustOverride Function WithElementType(ByVal elementType As TypeSymbol) As ArrayTypeSymbol

		Private MustInherit Class MDArray
			Inherits ArrayTypeSymbol.SZOrMDArray
			Private ReadOnly _rank As Integer

			Friend NotOverridable Overrides ReadOnly Property InterfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol)
				Get
					Return ImmutableArray(Of NamedTypeSymbol).Empty
				End Get
			End Property

			Friend NotOverridable Overrides ReadOnly Property IsSZArray As Boolean
				Get
					Return False
				End Get
			End Property

			Public NotOverridable Overrides ReadOnly Property Rank As Integer
				Get
					Return Me._rank
				End Get
			End Property

			Public Sub New(ByVal elementType As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal rank As Integer, ByVal systemArray As NamedTypeSymbol)
				MyBase.New(elementType, customModifiers, systemArray)
				Me._rank = rank
			End Sub
		End Class

		Private NotInheritable Class MDArrayNoSizesOrBounds
			Inherits ArrayTypeSymbol.MDArray
			Friend Overrides ReadOnly Property HasDefaultSizesAndLowerBounds As Boolean
				Get
					Return True
				End Get
			End Property

			Public Sub New(ByVal elementType As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal rank As Integer, ByVal systemArray As NamedTypeSymbol)
				MyBase.New(elementType, customModifiers, rank, systemArray)
			End Sub

			Friend Overrides Function WithElementType(ByVal newElementType As TypeSymbol) As ArrayTypeSymbol
				Return New ArrayTypeSymbol.MDArrayNoSizesOrBounds(newElementType, MyBase.CustomModifiers, MyBase.Rank, MyBase.BaseTypeNoUseSiteDiagnostics)
			End Function
		End Class

		Private NotInheritable Class MDArrayWithSizesAndBounds
			Inherits ArrayTypeSymbol.MDArray
			Private ReadOnly _sizes As ImmutableArray(Of Integer)

			Private ReadOnly _lowerBounds As ImmutableArray(Of Integer)

			Friend Overrides ReadOnly Property HasDefaultSizesAndLowerBounds As Boolean
				Get
					Return False
				End Get
			End Property

			Friend Overrides ReadOnly Property LowerBounds As ImmutableArray(Of Integer)
				Get
					Return Me._lowerBounds
				End Get
			End Property

			Friend Overrides ReadOnly Property Sizes As ImmutableArray(Of Integer)
				Get
					Return Me._sizes
				End Get
			End Property

			Public Sub New(ByVal elementType As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal rank As Integer, ByVal sizes As ImmutableArray(Of Integer), ByVal lowerBounds As ImmutableArray(Of Integer), ByVal systemArray As NamedTypeSymbol)
				MyBase.New(elementType, customModifiers, rank, systemArray)
				Me._sizes = Microsoft.CodeAnalysis.ImmutableArrayExtensions.NullToEmpty(Of Integer)(sizes)
				Me._lowerBounds = lowerBounds
			End Sub

			Friend Overrides Function WithElementType(ByVal newElementType As TypeSymbol) As ArrayTypeSymbol
				Return New ArrayTypeSymbol.MDArrayWithSizesAndBounds(newElementType, MyBase.CustomModifiers, MyBase.Rank, Me.Sizes, Me.LowerBounds, MyBase.BaseTypeNoUseSiteDiagnostics)
			End Function
		End Class

		Private NotInheritable Class SZArray
			Inherits ArrayTypeSymbol.SZOrMDArray
			Private ReadOnly _interfaces As ImmutableArray(Of NamedTypeSymbol)

			Friend Overrides ReadOnly Property HasDefaultSizesAndLowerBounds As Boolean
				Get
					Return True
				End Get
			End Property

			Friend Overrides ReadOnly Property InterfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol)
				Get
					Return Me._interfaces
				End Get
			End Property

			Friend Overrides ReadOnly Property IsSZArray As Boolean
				Get
					Return True
				End Get
			End Property

			Public Overrides ReadOnly Property Rank As Integer
				Get
					Return 1
				End Get
			End Property

			Public Sub New(ByVal elementType As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal systemArray As NamedTypeSymbol, ByVal interfaces As ImmutableArray(Of NamedTypeSymbol))
				MyBase.New(elementType, customModifiers, systemArray)
				Me._interfaces = interfaces
			End Sub

			Friend Overrides Function WithElementType(ByVal newElementType As TypeSymbol) As ArrayTypeSymbol
				Dim namedTypeSymbols As ImmutableArray(Of NamedTypeSymbol) = Me._interfaces
				If (namedTypeSymbols.Length > 0) Then
					namedTypeSymbols = Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of NamedTypeSymbol, NamedTypeSymbol)(namedTypeSymbols, Function(i As NamedTypeSymbol) i.OriginalDefinition.Construct(New TypeSymbol() { newElementType }))
				End If
				Return New ArrayTypeSymbol.SZArray(newElementType, MyBase.CustomModifiers, MyBase.BaseTypeNoUseSiteDiagnostics, namedTypeSymbols)
			End Function
		End Class

		Private MustInherit Class SZOrMDArray
			Inherits ArrayTypeSymbol
			Private ReadOnly _elementType As TypeSymbol

			Private ReadOnly _customModifiers As ImmutableArray(Of CustomModifier)

			Private ReadOnly _systemArray As NamedTypeSymbol

			Friend NotOverridable Overrides ReadOnly Property BaseTypeNoUseSiteDiagnostics As NamedTypeSymbol
				Get
					Return Me._systemArray
				End Get
			End Property

			Public NotOverridable Overrides ReadOnly Property CustomModifiers As ImmutableArray(Of CustomModifier)
				Get
					Return Me._customModifiers
				End Get
			End Property

			Public NotOverridable Overrides ReadOnly Property ElementType As TypeSymbol
				Get
					Return Me._elementType
				End Get
			End Property

			Public Sub New(ByVal elementType As TypeSymbol, ByVal customModifiers As ImmutableArray(Of CustomModifier), ByVal systemArray As NamedTypeSymbol)
				MyBase.New()
				Me._elementType = elementType
				Me._systemArray = systemArray
				Me._customModifiers = Microsoft.CodeAnalysis.ImmutableArrayExtensions.NullToEmpty(Of CustomModifier)(customModifiers)
			End Sub

			Friend Overrides Function InternalSubstituteTypeParameters(ByVal substitution As TypeSubstitution) As TypeWithModifiers
				Dim typeWithModifier As TypeWithModifiers
				Dim sZArray As ArrayTypeSymbol
				Dim func As Func(Of NamedTypeSymbol, TypeSubstitution, NamedTypeSymbol)
				Dim typeWithModifier1 As TypeWithModifiers = New TypeWithModifiers(Me._elementType, Me._customModifiers)
				Dim typeWithModifier2 As TypeWithModifiers = typeWithModifier1.InternalSubstituteTypeParameters(substitution)
				If (typeWithModifier2 = typeWithModifier1) Then
					typeWithModifier = New TypeWithModifiers(Me)
				Else
					If (Me.IsSZArray) Then
						Dim interfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol) = Me.InterfacesNoUseSiteDiagnostics
						If (interfacesNoUseSiteDiagnostics.Length > 0) Then
							Dim namedTypeSymbols As ImmutableArray(Of NamedTypeSymbol) = interfacesNoUseSiteDiagnostics
							If (ArrayTypeSymbol.SZOrMDArray._Closure$__.$I10-0 Is Nothing) Then
								func = Function([interface] As NamedTypeSymbol, map As TypeSubstitution) DirectCast([interface].InternalSubstituteTypeParameters(map).AsTypeSymbolOnly(), NamedTypeSymbol)
								ArrayTypeSymbol.SZOrMDArray._Closure$__.$I10-0 = func
							Else
								func = ArrayTypeSymbol.SZOrMDArray._Closure$__.$I10-0
							End If
							interfacesNoUseSiteDiagnostics = Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of NamedTypeSymbol, TypeSubstitution, NamedTypeSymbol)(namedTypeSymbols, func, substitution)
						End If
						sZArray = New ArrayTypeSymbol.SZArray(typeWithModifier2.Type, typeWithModifier2.CustomModifiers, Me._systemArray, interfacesNoUseSiteDiagnostics)
					ElseIf (Not Me.HasDefaultSizesAndLowerBounds) Then
						sZArray = New ArrayTypeSymbol.MDArrayWithSizesAndBounds(typeWithModifier2.Type, typeWithModifier2.CustomModifiers, Me.Rank, Me.Sizes, Me.LowerBounds, Me._systemArray)
					Else
						sZArray = New ArrayTypeSymbol.MDArrayNoSizesOrBounds(typeWithModifier2.Type, typeWithModifier2.CustomModifiers, Me.Rank, Me._systemArray)
					End If
					typeWithModifier = New TypeWithModifiers(sZArray)
				End If
				Return typeWithModifier
			End Function
		End Class
	End Class
End Namespace