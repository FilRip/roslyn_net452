Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Reflection.Metadata
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class NamedTypeSymbol
		Inherits TypeSymbol
		Implements ITypeReference, ITypeDefinition, INamedTypeReference, INamedTypeDefinition, INamespaceTypeReference, INamespaceTypeDefinition, INestedTypeReference, INestedTypeDefinition, IGenericTypeInstanceReference, ISpecializedNestedTypeReference, INamedTypeSymbol, INamedTypeSymbolInternal
		Private _lazyDeclaredBase As NamedTypeSymbol

		Private _lazyDeclaredInterfaces As ImmutableArray(Of NamedTypeSymbol)

		Private _lazyBaseType As NamedTypeSymbol

		Private _lazyInterfaces As ImmutableArray(Of NamedTypeSymbol)

		Friend ReadOnly Property AdaptedNamedTypeSymbol As NamedTypeSymbol
			Get
				Return Me
			End Get
		End Property

		Friend Overridable ReadOnly Property AreMembersImplicitlyDeclared As Boolean
			Get
				Return False
			End Get
		End Property

		Public MustOverride ReadOnly Property Arity As Integer

		Public Overridable ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return Nothing
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property BaseTypeNoUseSiteDiagnostics As NamedTypeSymbol
			Get
				If (Me._lazyBaseType = ErrorTypeSymbol.UnknownResultType) Then
					If (Me.ContainingType IsNot Nothing) Then
						Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.ContainingType.BaseTypeNoUseSiteDiagnostics
					End If
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
					Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.MakeAcyclicBaseType(instance)
					Me.AtomicStoreReferenceAndDiagnostics(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)(Me._lazyBaseType, namedTypeSymbol1, instance, ErrorTypeSymbol.UnknownResultType)
					instance.Free()
				End If
				Return Me._lazyBaseType
			End Get
		End Property

		Friend MustOverride ReadOnly Property CanConstruct As Boolean

		Friend MustOverride ReadOnly Property CoClassType As TypeSymbol

		Public MustOverride ReadOnly Property ConstructedFrom As NamedTypeSymbol

		Public ReadOnly Property Constructors As ImmutableArray(Of MethodSymbol)
			Get
				Return Me.GetConstructors(Of MethodSymbol)(True, True)
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return TryCast(Me.ContainingSymbol, NamedTypeSymbol)
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility

		Protected ReadOnly Property DefaultMarshallingCharSet As CharSet
			Get
				Dim effectiveDefaultMarshallingCharSet As Nullable(Of CharSet) = MyBase.EffectiveDefaultMarshallingCharSet
				Dim nullable As Nullable(Of CharSet) = effectiveDefaultMarshallingCharSet
				If (Not effectiveDefaultMarshallingCharSet.HasValue) Then
					Return CharSet.Ansi
				End If
				Return nullable.GetValueOrDefault()
			End Get
		End Property

		Friend MustOverride ReadOnly Property DefaultPropertyName As String

		Public Overridable ReadOnly Property DelegateInvokeMethod As MethodSymbol
			Get
				Dim item As MethodSymbol
				If (Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Delegate]) Then
					Dim members As ImmutableArray(Of Symbol) = Me.GetMembers("Invoke")
					If (members.Length = 1) Then
						item = TryCast(members(0), MethodSymbol)
					Else
						item = Nothing
					End If
				Else
					item = Nothing
				End If
				Return item
			End Get
		End Property

		Public Overridable ReadOnly Property EnumUnderlyingType As NamedTypeSymbol
			Get
				Return Nothing
			End Get
		End Property

		Private ReadOnly Property GenericParameterCountImpl As UShort
			Get
				Return CUShort(Me.AdaptedNamedTypeSymbol.Arity)
			End Get
		End Property

		Private ReadOnly Property GenericTypeImpl(ByVal context As EmitContext) As INamedTypeReference
			Get
				Return DirectCast(context.[Module], PEModuleBuilder).Translate(Me.AdaptedNamedTypeSymbol.OriginalDefinition, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics, False, True)
			End Get
		End Property

		Friend MustOverride ReadOnly Property HasCodeAnalysisEmbeddedAttribute As Boolean

		Friend MustOverride ReadOnly Property HasDeclarativeSecurity As Boolean

		Friend MustOverride ReadOnly Property HasSpecialName As Boolean

		Friend MustOverride ReadOnly Property HasTypeArgumentsCustomModifiers As Boolean

		Friend MustOverride ReadOnly Property HasVisualBasicEmbeddedAttribute As Boolean

		ReadOnly Property INamedEntityName As String
			Get
				Return Me.AdaptedNamedTypeSymbol.Name
			End Get
		End Property

		ReadOnly Property INamedTypeReferenceGenericParameterCount As UShort Implements INamedTypeReference.GenericParameterCount
			Get
				Return Me.GenericParameterCountImpl
			End Get
		End Property

		ReadOnly Property INamedTypeReferenceMangleName As Boolean Implements INamedTypeReference.MangleName
			Get
				Return Me.AdaptedNamedTypeSymbol.MangleName
			End Get
		End Property

		ReadOnly Property INamedTypeSymbol_Arity As Integer Implements INamedTypeSymbol.Arity
			Get
				Return Me.Arity
			End Get
		End Property

		ReadOnly Property INamedTypeSymbol_AssociatedSymbol As ISymbol Implements INamedTypeSymbol.AssociatedSymbol
			Get
				Return Me.AssociatedSymbol
			End Get
		End Property

		ReadOnly Property INamedTypeSymbol_ConstructedFrom As INamedTypeSymbol Implements INamedTypeSymbol.ConstructedFrom
			Get
				Return Me.ConstructedFrom
			End Get
		End Property

		ReadOnly Property INamedTypeSymbol_Constructors As ImmutableArray(Of IMethodSymbol) Implements INamedTypeSymbol.Constructors
			Get
				Return Me.GetConstructors(Of IMethodSymbol)(True, True)
			End Get
		End Property

		ReadOnly Property INamedTypeSymbol_DelegateInvokeMethod As IMethodSymbol Implements INamedTypeSymbol.DelegateInvokeMethod
			Get
				Return Me.DelegateInvokeMethod
			End Get
		End Property

		ReadOnly Property INamedTypeSymbol_EnumUnderlyingType As INamedTypeSymbol Implements INamedTypeSymbol.EnumUnderlyingType
			Get
				Return Me.EnumUnderlyingType
			End Get
		End Property

		ReadOnly Property INamedTypeSymbol_InstanceConstructors As ImmutableArray(Of IMethodSymbol) Implements INamedTypeSymbol.InstanceConstructors
			Get
				Return Me.GetConstructors(Of IMethodSymbol)(True, False)
			End Get
		End Property

		ReadOnly Property INamedTypeSymbol_IsComImport As Boolean Implements INamedTypeSymbol.IsComImport
			Get
				Return Me.IsComImport
			End Get
		End Property

		ReadOnly Property INamedTypeSymbol_IsImplicitClass As Boolean Implements INamedTypeSymbol.IsImplicitClass
			Get
				Return Me.IsImplicitClass
			End Get
		End Property

		ReadOnly Property INamedTypeSymbol_IsScriptClass As Boolean Implements INamedTypeSymbol.IsScriptClass
			Get
				Return Me.IsScriptClass
			End Get
		End Property

		ReadOnly Property INamedTypeSymbol_IsUnboundGenericType As Boolean Implements INamedTypeSymbol.IsUnboundGenericType
			Get
				Return Me.IsUnboundGenericType
			End Get
		End Property

		ReadOnly Property INamedTypeSymbol_MemberNames As IEnumerable(Of String) Implements INamedTypeSymbol.MemberNames
			Get
				Return Me.MemberNames
			End Get
		End Property

		ReadOnly Property INamedTypeSymbol_NativeIntegerUnderlyingType As INamedTypeSymbol Implements INamedTypeSymbol.NativeIntegerUnderlyingType
			Get
				Return Nothing
			End Get
		End Property

		ReadOnly Property INamedTypeSymbol_OriginalDefinition As INamedTypeSymbol Implements INamedTypeSymbol.OriginalDefinition
			Get
				Return Me.OriginalDefinition
			End Get
		End Property

		ReadOnly Property INamedTypeSymbol_StaticConstructors As ImmutableArray(Of IMethodSymbol) Implements INamedTypeSymbol.StaticConstructors
			Get
				Return Me.GetConstructors(Of IMethodSymbol)(False, True)
			End Get
		End Property

		ReadOnly Property INamedTypeSymbol_TupleElements As ImmutableArray(Of IFieldSymbol) Implements INamedTypeSymbol.TupleElements
			Get
				Return StaticCast(Of IFieldSymbol).From(Of FieldSymbol)(Me.TupleElements)
			End Get
		End Property

		ReadOnly Property INamedTypeSymbol_TupleUnderlyingType As INamedTypeSymbol Implements INamedTypeSymbol.TupleUnderlyingType
			Get
				Return Me.TupleUnderlyingType
			End Get
		End Property

		ReadOnly Property INamedTypeSymbol_TypeArguments As ImmutableArray(Of ITypeSymbol) Implements INamedTypeSymbol.TypeArguments
			Get
				Return StaticCast(Of ITypeSymbol).From(Of TypeSymbol)(Me.TypeArgumentsNoUseSiteDiagnostics)
			End Get
		End Property

		ReadOnly Property INamedTypeSymbol_TypeParameters As ImmutableArray(Of ITypeParameterSymbol) Implements INamedTypeSymbol.TypeParameters
			Get
				Return StaticCast(Of ITypeParameterSymbol).From(Of TypeParameterSymbol)(Me.TypeParameters)
			End Get
		End Property

		ReadOnly Property INamedTypeSymbolInternal_EnumUnderlyingType As INamedTypeSymbolInternal Implements INamedTypeSymbolInternal.EnumUnderlyingType
			Get
				Return Me.EnumUnderlyingType
			End Get
		End Property

		ReadOnly Property INamespaceTypeDefinitionIsPublic As Boolean Implements INamespaceTypeDefinition.IsPublic
			Get
				Return PEModuleBuilder.MemberVisibility(Me.AdaptedNamedTypeSymbol) = TypeMemberVisibility.[Public]
			End Get
		End Property

		ReadOnly Property INamespaceTypeReferenceNamespaceName As String Implements INamespaceTypeReference.NamespaceName
			Get
				Return If(Me.AdaptedNamedTypeSymbol.GetEmittedNamespaceName(), Me.AdaptedNamedTypeSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat))
			End Get
		End Property

		Public ReadOnly Property InstanceConstructors As ImmutableArray(Of MethodSymbol)
			Get
				Return Me.GetConstructors(Of MethodSymbol)(True, False)
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property InterfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol)
			Get
				If (Me._lazyInterfaces.IsDefault) Then
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
					Dim namedTypeSymbols As ImmutableArray(Of NamedTypeSymbol) = Me.MakeAcyclicInterfaces(instance)
					Me.AtomicStoreArrayAndDiagnostics(Of NamedTypeSymbol)(Me._lazyInterfaces, namedTypeSymbols, instance)
					instance.Free()
				End If
				Return Me._lazyInterfaces
			End Get
		End Property

		Friend MustOverride ReadOnly Property IsComImport As Boolean

		Friend ReadOnly Property IsConditional As Boolean
			Get
				Return System.Linq.ImmutableArrayExtensions.Any(Of String)(Me.GetAppliedConditionalSymbols())
			End Get
		End Property

		Friend Overridable ReadOnly Property IsDirectlyExcludedFromCodeCoverage As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overridable ReadOnly Property IsExplicitDefinitionOfNoPiaLocalType As Boolean
			Get
				Return False
			End Get
		End Property

		Friend MustOverride ReadOnly Property IsExtensibleInterfaceNoUseSiteDiagnostics As Boolean

		Public ReadOnly Property IsGenericType As Boolean Implements INamedTypeSymbol.IsGenericType
			Get
				Dim flag As Boolean
				Dim containingType As NamedTypeSymbol = Me
				While True
					If (containingType Is Nothing) Then
						flag = False
						Exit While
					ElseIf (containingType.Arity = 0) Then
						containingType = containingType.ContainingType
					Else
						flag = True
						Exit While
					End If
				End While
				Return flag
			End Get
		End Property

		Public Overridable ReadOnly Property IsImplicitClass As Boolean
			Get
				Return False
			End Get
		End Property

		Friend MustOverride ReadOnly Property IsInterface As Boolean

		Friend Overridable ReadOnly Property IsMetadataAbstract As Boolean
			Get
				If (Me.IsMustInherit) Then
					Return True
				End If
				Return Me.IsInterface
			End Get
		End Property

		Friend Overridable ReadOnly Property IsMetadataSealed As Boolean
			Get
				Dim flag As Boolean
				If (Not Me.IsNotInheritable) Then
					Dim typeKind As Microsoft.CodeAnalysis.TypeKind = Me.TypeKind
					flag = If(typeKind = Microsoft.CodeAnalysis.TypeKind.[Enum] OrElse typeKind = Microsoft.CodeAnalysis.TypeKind.[Module] OrElse typeKind = Microsoft.CodeAnalysis.TypeKind.Struct, True, False)
				Else
					flag = True
				End If
				Return flag
			End Get
		End Property

		Public MustOverride ReadOnly Property IsMustInherit As Boolean

		Public MustOverride ReadOnly Property IsNotInheritable As Boolean

		Public Overrides ReadOnly Property IsReferenceType As Boolean
			Get
				If (Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Enum] OrElse Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.Struct) Then
					Return False
				End If
				Return Me.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Error]
			End Get
		End Property

		Public Overridable ReadOnly Property IsScriptClass As Boolean
			Get
				Return False
			End Get
		End Property

		Public MustOverride ReadOnly Property IsSerializable As Boolean Implements INamedTypeSymbol.IsSerializable

		Public ReadOnly Property IsSubmissionClass As Boolean
			Get
				Return Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.Submission
			End Get
		End Property

		Public Overridable ReadOnly Property IsUnboundGenericType As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsValueType As Boolean
			Get
				If (Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Enum]) Then
					Return True
				End If
				Return Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.Struct
			End Get
		End Property

		Friend MustOverride ReadOnly Property IsWindowsRuntimeImport As Boolean

		Protected Overrides ReadOnly Property ISymbol_IsAbstract As Boolean
			Get
				Return Me.IsMustInherit
			End Get
		End Property

		Protected Overrides ReadOnly Property ISymbol_IsSealed As Boolean
			Get
				Return Me.IsNotInheritable
			End Get
		End Property

		ReadOnly Property ITypeDefinitionAlignment As UShort Implements ITypeDefinition.Alignment
			Get
				Return CUShort(Me.AdaptedNamedTypeSymbol.Layout.Alignment)
			End Get
		End Property

		ReadOnly Property ITypeDefinitionGenericParameterCount As UShort Implements ITypeDefinition.GenericParameterCount
			Get
				Return Me.GenericParameterCountImpl
			End Get
		End Property

		ReadOnly Property ITypeDefinitionGenericParameters As IEnumerable(Of IGenericTypeParameter) Implements ITypeDefinition.GenericParameters
			Get
				Return DirectCast(Me.AdaptedNamedTypeSymbol.TypeParameters, IEnumerable(Of IGenericTypeParameter))
			End Get
		End Property

		ReadOnly Property ITypeDefinitionHasDeclarativeSecurity As Boolean Implements ITypeDefinition.HasDeclarativeSecurity
			Get
				Return Me.AdaptedNamedTypeSymbol.HasDeclarativeSecurity
			End Get
		End Property

		ReadOnly Property ITypeDefinitionIsAbstract As Boolean Implements ITypeDefinition.IsAbstract
			Get
				Return Me.AdaptedNamedTypeSymbol.IsMetadataAbstract
			End Get
		End Property

		ReadOnly Property ITypeDefinitionIsBeforeFieldInit As Boolean Implements ITypeDefinition.IsBeforeFieldInit
			Get
				Dim beInjectedIntoConstructor As Boolean
				Select Case Me.AdaptedNamedTypeSymbol.TypeKind
					Case Microsoft.CodeAnalysis.TypeKind.[Delegate]
					Case Microsoft.CodeAnalysis.TypeKind.[Enum]
					Case Microsoft.CodeAnalysis.TypeKind.[Interface]
						beInjectedIntoConstructor = False
						Exit Select
					Case Microsoft.CodeAnalysis.TypeKind.Dynamic
					Case Microsoft.CodeAnalysis.TypeKind.[Error]
					Label1:
						Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(Me.AdaptedNamedTypeSymbol.SharedConstructors)
						If (methodSymbol Is Nothing) Then
							Dim adaptedNamedTypeSymbol As SourceMemberContainerTypeSymbol = TryCast(Me.AdaptedNamedTypeSymbol, SourceMemberContainerTypeSymbol)
							If (adaptedNamedTypeSymbol Is Nothing OrElse adaptedNamedTypeSymbol.StaticInitializers.IsDefaultOrEmpty) Then
								beInjectedIntoConstructor = False
								Exit Select
							Else
								beInjectedIntoConstructor = adaptedNamedTypeSymbol.AnyInitializerToBeInjectedIntoConstructor(DirectCast(adaptedNamedTypeSymbol.StaticInitializers, IEnumerable(Of ImmutableArray(Of FieldOrPropertyInitializer))), True)
								Exit Select
							End If
						ElseIf (methodSymbol.IsImplicitlyDeclared) Then
							Dim enumerator As ImmutableArray(Of Symbol).Enumerator = Me.AdaptedNamedTypeSymbol.GetMembers().GetEnumerator()
							While enumerator.MoveNext()
								Dim current As Symbol = enumerator.Current
								If (current.Kind <> SymbolKind.Method) Then
									Continue While
								End If
								Dim handledEvents As ImmutableArray(Of HandledEvent) = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).HandledEvents
								If (handledEvents.IsEmpty) Then
									Continue While
								End If
								Dim enumerator1 As ImmutableArray(Of HandledEvent).Enumerator = handledEvents.GetEnumerator()
								While enumerator1.MoveNext()
									If (enumerator1.Current.hookupMethod.MethodKind <> MethodKind.StaticConstructor) Then
										Continue While
									End If
									beInjectedIntoConstructor = False
									Return beInjectedIntoConstructor
								End While
							End While
							beInjectedIntoConstructor = True
							Exit Select
						Else
							beInjectedIntoConstructor = False
							Exit Select
						End If
					Case Else
						GoTo Label1
				End Select
				Return beInjectedIntoConstructor
			End Get
		End Property

		ReadOnly Property ITypeDefinitionIsComObject As Boolean Implements ITypeDefinition.IsComObject
			Get
				Return Me.AdaptedNamedTypeSymbol.IsComImport
			End Get
		End Property

		ReadOnly Property ITypeDefinitionIsDelegate As Boolean Implements ITypeDefinition.IsDelegate
			Get
				Return Me.AdaptedNamedTypeSymbol.IsDelegateType()
			End Get
		End Property

		ReadOnly Property ITypeDefinitionIsGeneric As Boolean Implements ITypeDefinition.IsGeneric
			Get
				Return Me.AdaptedNamedTypeSymbol.Arity <> 0
			End Get
		End Property

		ReadOnly Property ITypeDefinitionIsInterface As Boolean Implements ITypeDefinition.IsInterface
			Get
				Return Me.AdaptedNamedTypeSymbol.IsInterface
			End Get
		End Property

		ReadOnly Property ITypeDefinitionIsRuntimeSpecial As Boolean Implements ITypeDefinition.IsRuntimeSpecial
			Get
				Return False
			End Get
		End Property

		ReadOnly Property ITypeDefinitionIsSealed As Boolean Implements ITypeDefinition.IsSealed
			Get
				Return Me.AdaptedNamedTypeSymbol.IsMetadataSealed
			End Get
		End Property

		ReadOnly Property ITypeDefinitionIsSerializable As Boolean Implements ITypeDefinition.IsSerializable
			Get
				Return Me.AdaptedNamedTypeSymbol.IsSerializable
			End Get
		End Property

		ReadOnly Property ITypeDefinitionIsSpecialName As Boolean Implements ITypeDefinition.IsSpecialName
			Get
				Return Me.AdaptedNamedTypeSymbol.HasSpecialName
			End Get
		End Property

		ReadOnly Property ITypeDefinitionIsWindowsRuntimeImport As Boolean Implements ITypeDefinition.IsWindowsRuntimeImport
			Get
				Return Me.AdaptedNamedTypeSymbol.IsWindowsRuntimeImport
			End Get
		End Property

		ReadOnly Property ITypeDefinitionLayout As LayoutKind Implements ITypeDefinition.Layout
			Get
				Return Me.AdaptedNamedTypeSymbol.Layout.Kind
			End Get
		End Property

		ReadOnly Property ITypeDefinitionMemberContainingTypeDefinition As ITypeDefinition
			Get
				Return Me.AdaptedNamedTypeSymbol.ContainingType.GetCciAdapter()
			End Get
		End Property

		ReadOnly Property ITypeDefinitionMemberVisibility As TypeMemberVisibility
			Get
				Return PEModuleBuilder.MemberVisibility(Me.AdaptedNamedTypeSymbol)
			End Get
		End Property

		ReadOnly Property ITypeDefinitionSecurityAttributes As IEnumerable(Of SecurityAttribute) Implements ITypeDefinition.SecurityAttributes
			Get
				Return Me.AdaptedNamedTypeSymbol.GetSecurityInformation()
			End Get
		End Property

		ReadOnly Property ITypeDefinitionSizeOf As UInteger Implements ITypeDefinition.SizeOf
			Get
				Return CUInt(Me.AdaptedNamedTypeSymbol.Layout.Size)
			End Get
		End Property

		ReadOnly Property ITypeDefinitionStringFormat As CharSet Implements ITypeDefinition.StringFormat
			Get
				Return Me.AdaptedNamedTypeSymbol.MarshallingCharSet
			End Get
		End Property

		ReadOnly Property ITypeReferenceAsGenericMethodParameterReference As IGenericMethodParameterReference Implements ITypeReference.AsGenericMethodParameterReference
			Get
				Return Nothing
			End Get
		End Property

		ReadOnly Property ITypeReferenceAsGenericTypeInstanceReference As IGenericTypeInstanceReference Implements ITypeReference.AsGenericTypeInstanceReference
			Get
				Dim genericTypeInstanceReference As IGenericTypeInstanceReference
				If (Me.AdaptedNamedTypeSymbol.IsDefinition OrElse Me.AdaptedNamedTypeSymbol.Arity <= 0 OrElse CObj(Me.AdaptedNamedTypeSymbol.ConstructedFrom) = CObj(Me.AdaptedNamedTypeSymbol)) Then
					genericTypeInstanceReference = Nothing
				Else
					genericTypeInstanceReference = Me
				End If
				Return genericTypeInstanceReference
			End Get
		End Property

		ReadOnly Property ITypeReferenceAsGenericTypeParameterReference As IGenericTypeParameterReference Implements ITypeReference.AsGenericTypeParameterReference
			Get
				Return Nothing
			End Get
		End Property

		ReadOnly Property ITypeReferenceAsNamespaceTypeReference As INamespaceTypeReference Implements ITypeReference.AsNamespaceTypeReference
			Get
				Dim namespaceTypeReference As INamespaceTypeReference
				If (Not Me.AdaptedNamedTypeSymbol.IsDefinition OrElse Me.AdaptedNamedTypeSymbol.ContainingType IsNot Nothing) Then
					namespaceTypeReference = Nothing
				Else
					namespaceTypeReference = Me
				End If
				Return namespaceTypeReference
			End Get
		End Property

		ReadOnly Property ITypeReferenceAsNestedTypeReference As INestedTypeReference Implements ITypeReference.AsNestedTypeReference
			Get
				Dim nestedTypeReference As INestedTypeReference
				If (Me.AdaptedNamedTypeSymbol.ContainingType Is Nothing) Then
					nestedTypeReference = Nothing
				Else
					nestedTypeReference = Me
				End If
				Return nestedTypeReference
			End Get
		End Property

		ReadOnly Property ITypeReferenceAsSpecializedNestedTypeReference As ISpecializedNestedTypeReference Implements ITypeReference.AsSpecializedNestedTypeReference
			Get
				Dim specializedNestedTypeReference As ISpecializedNestedTypeReference
				If (Me.AdaptedNamedTypeSymbol.IsDefinition OrElse Me.AdaptedNamedTypeSymbol.Arity <> 0 AndAlso CObj(Me.AdaptedNamedTypeSymbol.ConstructedFrom) <> CObj(Me.AdaptedNamedTypeSymbol)) Then
					specializedNestedTypeReference = Nothing
				Else
					specializedNestedTypeReference = Me
				End If
				Return specializedNestedTypeReference
			End Get
		End Property

		ReadOnly Property ITypeReferenceIsEnum As Boolean Implements ITypeReference.IsEnum
			Get
				Return Me.AdaptedNamedTypeSymbol.TypeKind = Microsoft.CodeAnalysis.TypeKind.[Enum]
			End Get
		End Property

		ReadOnly Property ITypeReferenceIsValueType As Boolean Implements ITypeReference.IsValueType
			Get
				Return Me.AdaptedNamedTypeSymbol.IsValueType
			End Get
		End Property

		ReadOnly Property ITypeReferenceTypeCode As Microsoft.Cci.PrimitiveTypeCode Implements ITypeReference.TypeCode
			Get
				Dim primitiveTypeCode As Microsoft.Cci.PrimitiveTypeCode
				primitiveTypeCode = If(Not Me.AdaptedNamedTypeSymbol.IsDefinition, Microsoft.Cci.PrimitiveTypeCode.NotPrimitive, Me.AdaptedNamedTypeSymbol.PrimitiveTypeCode)
				Return primitiveTypeCode
			End Get
		End Property

		ReadOnly Property ITypeReferenceTypeDef As TypeDefinitionHandle Implements ITypeReference.TypeDef
			Get
				Dim adaptedNamedTypeSymbol As PENamedTypeSymbol = TryCast(Me.AdaptedNamedTypeSymbol, PENamedTypeSymbol)
				Return If(adaptedNamedTypeSymbol Is Nothing, New TypeDefinitionHandle(), adaptedNamedTypeSymbol.Handle)
			End Get
		End Property

		Public Overrides ReadOnly Property Kind As SymbolKind
			Get
				Return SymbolKind.NamedType
			End Get
		End Property

		Friend Overridable ReadOnly Property KnownCircularStruct As Boolean
			Get
				Return False
			End Get
		End Property

		Friend MustOverride ReadOnly Property Layout As Microsoft.CodeAnalysis.TypeLayout

		Friend MustOverride ReadOnly Property MangleName As Boolean

		Friend MustOverride ReadOnly Property MarshallingCharSet As CharSet

		Public MustOverride ReadOnly Property MemberNames As IEnumerable(Of String)

		Public Overrides ReadOnly Property MetadataName As String
			Get
				If (Not Me.MangleName) Then
					Return Me.Name
				End If
				Return MetadataHelpers.ComposeAritySuffixedMetadataName(Me.Name, Me.Arity)
			End Get
		End Property

		Public MustOverride ReadOnly Property MightContainExtensionMethods As Boolean Implements INamedTypeSymbol.MightContainExtensionMethods

		Public Overrides ReadOnly Property Name As String

		Public Shadows Overridable ReadOnly Property OriginalDefinition As NamedTypeSymbol
			Get
				Return Me
			End Get
		End Property

		Protected NotOverridable Overrides ReadOnly Property OriginalTypeSymbolDefinition As TypeSymbol
			Get
				Return Me.OriginalDefinition
			End Get
		End Property

		Public ReadOnly Property SharedConstructors As ImmutableArray(Of MethodSymbol)
			Get
				Return Me.GetConstructors(Of MethodSymbol)(False, True)
			End Get
		End Property

		Friend MustOverride ReadOnly Property ShouldAddWinRTMembers As Boolean

		ReadOnly Property TypeArgumentNullableAnnotations As ImmutableArray(Of Microsoft.CodeAnalysis.NullableAnnotation) Implements INamedTypeSymbol.TypeArgumentNullableAnnotations
			Get
				Dim func As Func(Of TypeSymbol, Microsoft.CodeAnalysis.NullableAnnotation)
				Dim typeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol) = Me.TypeArgumentsNoUseSiteDiagnostics
				If (NamedTypeSymbol._Closure$__.$I296-0 Is Nothing) Then
					func = Function(t As TypeSymbol) Microsoft.CodeAnalysis.NullableAnnotation.None
					NamedTypeSymbol._Closure$__.$I296-0 = func
				Else
					func = NamedTypeSymbol._Closure$__.$I296-0
				End If
				Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of TypeSymbol, Microsoft.CodeAnalysis.NullableAnnotation)(typeArgumentsNoUseSiteDiagnostics, func)
			End Get
		End Property

		Friend MustOverride ReadOnly Property TypeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)

		Public MustOverride ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)

		Friend MustOverride ReadOnly Property TypeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution

		Friend Sub New()
			MyBase.New()
			Me._lazyDeclaredBase = ErrorTypeSymbol.UnknownResultType
			Me._lazyDeclaredInterfaces = New ImmutableArray(Of NamedTypeSymbol)()
			Me._lazyBaseType = ErrorTypeSymbol.UnknownResultType
		End Sub

		Friend Overrides Function Accept(Of TArgument, TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TArgument, TResult), ByVal arg As TArgument) As TResult
			Return visitor.VisitNamedType(Me, arg)
		End Function

		Public Overrides Sub Accept(ByVal visitor As SymbolVisitor)
			visitor.VisitNamedType(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As SymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitNamedType(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As VisualBasicSymbolVisitor)
			visitor.VisitNamedType(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitNamedType(Me)
		End Function

		Private Sub AddExplicitImplementations(ByVal context As EmitContext, ByVal implementingMethod As MethodSymbol, ByVal explicitImplements As ArrayBuilder(Of Microsoft.Cci.MethodImplementation), ByVal sourceNamedType As SourceNamedTypeSymbol, ByVal moduleBeingBuilt As PEModuleBuilder)
			Dim enumerator As ImmutableArray(Of MethodSymbol).Enumerator = implementingMethod.ExplicitInterfaceImplementations.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As MethodSymbol = enumerator.Current
				If (Not MethodSignatureComparer.CustomModifiersAndParametersAndReturnTypeSignatureComparer.Equals(implementingMethod, current)) Then
					Continue While
				End If
				explicitImplements.Add(New Microsoft.Cci.MethodImplementation(implementingMethod.GetCciAdapter(), moduleBeingBuilt.TranslateOverriddenMethodReference(current, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics)))
			End While
			If (OverrideHidingHelper.RequiresExplicitOverride(implementingMethod)) Then
				explicitImplements.Add(New Microsoft.Cci.MethodImplementation(implementingMethod.GetCciAdapter(), moduleBeingBuilt.TranslateOverriddenMethodReference(implementingMethod.OverriddenMethod, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics)))
			End If
			If (sourceNamedType IsNot Nothing) Then
				Dim correspondingComClassInterfaceMethod As MethodSymbol = sourceNamedType.GetCorrespondingComClassInterfaceMethod(implementingMethod)
				If (correspondingComClassInterfaceMethod IsNot Nothing) Then
					explicitImplements.Add(New Microsoft.Cci.MethodImplementation(implementingMethod.GetCciAdapter(), moduleBeingBuilt.TranslateOverriddenMethodReference(correspondingComClassInterfaceMethod, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics)))
				End If
			End If
		End Sub

		Friend Overridable Sub AddExtensionMethodLookupSymbolsInfo(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
			Me.AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder, Me)
		End Sub

		Friend Overridable Sub AddExtensionMethodLookupSymbolsInfo(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder, ByVal appendThrough As NamedTypeSymbol)
			If (Me.MightContainExtensionMethods) Then
				appendThrough.AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder, Me.MemberNames.[Select](Of KeyValuePair(Of String, ImmutableArray(Of Symbol)))(Function(name As String) New KeyValuePair(Of String, ImmutableArray(Of Symbol))(name, Me.GetMembers(name))))
			End If
		End Sub

		Friend Overridable Sub AppendProbableExtensionMethods(ByVal name As String, ByVal methods As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol))
			If (Me.MightContainExtensionMethods) Then
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = Me.GetMembers(name).GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Symbol = enumerator.Current
					If (current.Kind <> SymbolKind.Method) Then
						Continue While
					End If
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
					If (Not methodSymbol.MayBeReducibleExtensionMethod) Then
						Continue While
					End If
					methods.Add(methodSymbol)
				End While
			End If
		End Sub

		Private Function AsNestedTypeDefinitionImpl(ByVal moduleBeingBuilt As PEModuleBuilder) As INestedTypeDefinition
			Dim nestedTypeDefinition As INestedTypeDefinition
			If (Me.AdaptedNamedTypeSymbol.ContainingType Is Nothing OrElse Not Me.AdaptedNamedTypeSymbol.IsDefinition OrElse Not Me.AdaptedNamedTypeSymbol.ContainingModule.Equals(moduleBeingBuilt.SourceModule)) Then
				nestedTypeDefinition = Nothing
			Else
				nestedTypeDefinition = Me
			End If
			Return nestedTypeDefinition
		End Function

		Private Function AsTypeDefinitionImpl(ByVal moduleBeingBuilt As PEModuleBuilder) As ITypeDefinition
			Dim typeDefinition As ITypeDefinition
			If (Not Me.AdaptedNamedTypeSymbol.IsDefinition OrElse Not Me.AdaptedNamedTypeSymbol.ContainingModule.Equals(moduleBeingBuilt.SourceModule)) Then
				typeDefinition = Nothing
			Else
				typeDefinition = Me
			End If
			Return typeDefinition
		End Function

		Friend Sub AtomicStoreArrayAndDiagnostics(Of T)(ByRef variable As ImmutableArray(Of T), ByVal value As ImmutableArray(Of T), ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (diagBag Is Nothing OrElse diagBag.IsEmpty) Then
				ImmutableInterlocked.InterlockedCompareExchange(Of T)(variable, value, New ImmutableArray(Of T)())
				Return
			End If
			Dim containingModule As SourceModuleSymbol = TryCast(Me.ContainingModule, SourceModuleSymbol)
			If (containingModule IsNot Nothing) Then
				containingModule.AtomicStoreArrayAndDiagnostics(Of T)(variable, value, diagBag)
			End If
		End Sub

		Private Sub AtomicStoreReferenceAndDiagnostics(Of T As Class)(ByRef variable As T, ByVal value As T, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByVal comparand As T = Nothing)
			If (diagBag Is Nothing OrElse diagBag.IsEmpty) Then
				Interlocked.CompareExchange(Of T)(variable, value, comparand)
				Return
			End If
			Dim containingModule As SourceModuleSymbol = TryCast(Me.ContainingModule, SourceModuleSymbol)
			If (containingModule IsNot Nothing) Then
				containingModule.AtomicStoreReferenceAndDiagnostics(Of T)(variable, value, diagBag, comparand)
			End If
		End Sub

		Friend Overridable Sub BuildExtensionMethodsMap(ByVal map As Dictionary(Of String, ArrayBuilder(Of MethodSymbol)), ByVal appendThrough As NamespaceSymbol)
			If (Me.MightContainExtensionMethods) Then
				appendThrough.BuildExtensionMethodsMap(map, Me.MemberNames.[Select](Of KeyValuePair(Of String, ImmutableArray(Of Symbol)))(Function(name As String) New KeyValuePair(Of String, ImmutableArray(Of Symbol))(name, Me.GetMembers(name))))
			End If
		End Sub

		Protected Sub CheckCanConstructAndTypeArguments(ByVal typeArguments As ImmutableArray(Of TypeSymbol))
			If (Not Me.CanConstruct OrElse CObj(Me) <> CObj(Me.ConstructedFrom)) Then
				Throw New InvalidOperationException()
			End If
			typeArguments.CheckTypeArguments(Me.Arity)
		End Sub

		Public Function Construct(ByVal ParamArray typeArguments As TypeSymbol()) As NamedTypeSymbol
			Return Me.Construct(Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeSymbol)(typeArguments))
		End Function

		Public Function Construct(ByVal typeArguments As IEnumerable(Of TypeSymbol)) As NamedTypeSymbol
			Return Me.Construct(Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeSymbol)(typeArguments))
		End Function

		Public MustOverride Function Construct(ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As NamedTypeSymbol

		Friend Function Construct(ByVal substitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			If (substitution IsNot Nothing) Then
				substitution.ThrowIfSubstitutingToAlphaRenamedTypeParameter()
				namedTypeSymbol = DirectCast(Me.InternalSubstituteTypeParameters(substitution).AsTypeSymbolOnly(), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			Else
				namedTypeSymbol = Me
			End If
			Return namedTypeSymbol
		End Function

		Public Function ConstructUnboundGenericType() As NamedTypeSymbol
			Return Me.AsUnboundGenericType()
		End Function

		Private Function DeriveUseSiteInfoFromTypeArguments() As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo1 As UseSiteInfo(Of AssemblySymbol) = New UseSiteInfo(Of AssemblySymbol)()
			Dim containingType As NamedTypeSymbol = Me
		Label0:
			Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = containingType.TypeArgumentsNoUseSiteDiagnostics.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As TypeSymbol = enumerator.Current
					If (Symbol.MergeUseSiteInfo(useSiteInfo1, MyBase.DeriveUseSiteInfoFromType(current), 30649)) Then
						useSiteInfo = useSiteInfo1
						Exit While
					End If
				Else
					If (containingType.HasTypeArgumentsCustomModifiers) Then
						Dim arity As Integer = Me.Arity - 1
						Dim num As Integer = 0
						While num <= arity
							If (Not Symbol.MergeUseSiteInfo(useSiteInfo1, MyBase.DeriveUseSiteInfoFromCustomModifiers(Me.GetTypeArgumentCustomModifiers(num), False), 30649)) Then
								num = num + 1
							Else
								useSiteInfo = useSiteInfo1
								Return useSiteInfo
							End If
						End While
					End If
					containingType = containingType.ContainingType
					If (containingType IsNot Nothing AndAlso Not containingType.IsDefinition) Then
						GoTo Label0
					End If
					useSiteInfo = useSiteInfo1
					Exit While
				End If
			End While
			Return useSiteInfo
		End Function

		Friend Overrides MustOverride Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)

		Friend MustOverride Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)

		Friend MustOverride Function GetAttributeUsageInfo() As AttributeUsageInfo

		Friend Function GetBestKnownBaseType() As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me._lazyBaseType
			namedTypeSymbol = If(namedTypeSymbol1 = ErrorTypeSymbol.UnknownResultType, Me.GetDeclaredBase(New BasesBeingResolved()), namedTypeSymbol1)
			Return namedTypeSymbol
		End Function

		Friend Function GetBestKnownInterfacesNoUseSiteDiagnostics() As ImmutableArray(Of NamedTypeSymbol)
			Dim namedTypeSymbols As ImmutableArray(Of NamedTypeSymbol)
			Dim namedTypeSymbols1 As ImmutableArray(Of NamedTypeSymbol) = Me._lazyInterfaces
			namedTypeSymbols = If(namedTypeSymbols1.IsDefault, Me.GetDeclaredInterfacesNoUseSiteDiagnostics(New BasesBeingResolved()), namedTypeSymbols1)
			Return namedTypeSymbols
		End Function

		Friend Shadows Function GetCciAdapter() As NamedTypeSymbol
			Return Me
		End Function

		Private Function GetConstructors(Of TMethodSymbol As {Class, IMethodSymbol})(ByVal includeInstance As Boolean, ByVal includeShared As Boolean) As ImmutableArray(Of TMethodSymbol)
			Dim immutableAndFree As ImmutableArray(Of TMethodSymbol)
			Dim members As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim empty As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)
			If (includeInstance) Then
				members = Me.GetMembers(".ctor")
			Else
				members = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Empty
			End If
			Dim symbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = members
			If (includeShared) Then
				empty = Me.GetMembers(".cctor")
			Else
				empty = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Empty
			End If
			Dim symbols1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = empty
			If (Not symbols.IsEmpty OrElse Not symbols1.IsEmpty) Then
				Dim instance As ArrayBuilder(Of TMethodSymbol) = ArrayBuilder(Of TMethodSymbol).GetInstance()
				Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = symbols.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator.Current
					If (current.Kind <> SymbolKind.Method) Then
						Continue While
					End If
					instance.Add(DirectCast(TryCast(current, TMethodSymbol), TMethodSymbol))
				End While
				Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = symbols1.GetEnumerator()
				While enumerator1.MoveNext()
					Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator1.Current
					If (symbol.Kind <> SymbolKind.Method) Then
						Continue While
					End If
					instance.Add(DirectCast(TryCast(symbol, TMethodSymbol), TMethodSymbol))
				End While
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = ImmutableArray(Of TMethodSymbol).Empty
			End If
			Return immutableAndFree
		End Function

		Friend Overridable Function GetDeclaredBase(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved) As NamedTypeSymbol
			If (Me._lazyDeclaredBase = ErrorTypeSymbol.UnknownResultType) Then
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
				Me.AtomicStoreReferenceAndDiagnostics(Of NamedTypeSymbol)(Me._lazyDeclaredBase, Me.MakeDeclaredBase(basesBeingResolved, instance), instance, ErrorTypeSymbol.UnknownResultType)
				instance.Free()
			End If
			Return Me._lazyDeclaredBase
		End Function

		Friend Overridable Function GetDeclaredBaseInterfacesSafe(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved) As ImmutableArray(Of NamedTypeSymbol)
			Dim namedTypeSymbols As ImmutableArray(Of NamedTypeSymbol)
			namedTypeSymbols = If(Not basesBeingResolved.InheritsBeingResolvedOpt.Contains(Me), Me.GetDeclaredInterfacesNoUseSiteDiagnostics(basesBeingResolved.PrependInheritsBeingResolved(Me)), New ImmutableArray(Of NamedTypeSymbol)())
			Return namedTypeSymbols
		End Function

		Friend Overridable Function GetDeclaredInterfacesNoUseSiteDiagnostics(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved) As ImmutableArray(Of NamedTypeSymbol)
			If (Me._lazyDeclaredInterfaces.IsDefault) Then
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
				Me.AtomicStoreArrayAndDiagnostics(Of NamedTypeSymbol)(Me._lazyDeclaredInterfaces, Me.MakeDeclaredInterfaces(basesBeingResolved, instance), instance)
				instance.Free()
			End If
			Return Me._lazyDeclaredInterfaces
		End Function

		Friend Function GetDeclaredInterfacesWithDefinitionUseSiteDiagnostics(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ImmutableArray(Of NamedTypeSymbol)
			Dim declaredInterfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol) = Me.GetDeclaredInterfacesNoUseSiteDiagnostics(basesBeingResolved)
			Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = declaredInterfacesNoUseSiteDiagnostics.GetEnumerator()
			While enumerator.MoveNext()
				enumerator.Current.OriginalDefinition.AddUseSiteInfo(useSiteInfo)
			End While
			Return declaredInterfacesNoUseSiteDiagnostics
		End Function

		Friend Function GetDirectBaseInterfacesNoUseSiteDiagnostics(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved) As ImmutableArray(Of NamedTypeSymbol)
			Dim empty As ImmutableArray(Of NamedTypeSymbol)
			If (Me.TypeKind <> Microsoft.CodeAnalysis.TypeKind.[Interface]) Then
				empty = ImmutableArray(Of NamedTypeSymbol).Empty
			Else
				empty = If(basesBeingResolved.InheritsBeingResolvedOpt IsNot Nothing, Me.GetDeclaredBaseInterfacesSafe(basesBeingResolved), Me.InterfacesNoUseSiteDiagnostics)
			End If
			Return empty
		End Function

		Friend Overridable Function GetEmittedNamespaceName() As String
			Return Nothing
		End Function

		Friend Function GetEmptyTypeArgumentCustomModifiers(ByVal ordinal As Integer) As ImmutableArray(Of CustomModifier)
			If (ordinal < 0 OrElse ordinal >= Me.Arity) Then
				Throw New IndexOutOfRangeException()
			End If
			Return ImmutableArray(Of CustomModifier).Empty
		End Function

		Friend Overridable Function GetEventsToEmit() As IEnumerable(Of EventSymbol)
			Return New NamedTypeSymbol.VB$StateMachine_102_GetEventsToEmit(-2) With
			{
				.$VB$Me = Me
			}
		End Function

		Friend Overridable Sub GetExtensionMethods(ByVal methods As ArrayBuilder(Of MethodSymbol), ByVal appendThrough As NamespaceSymbol, ByVal Name As String)
			If (Me.MightContainExtensionMethods) Then
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = Me.GetSimpleNonTypeMembers(Name).GetEnumerator()
				While enumerator.MoveNext()
					appendThrough.AddMemberIfExtension(methods, enumerator.Current)
				End While
			End If
		End Sub

		Friend MustOverride Function GetFieldsToEmit() As IEnumerable(Of FieldSymbol)

		Friend Overridable Function GetGuidString(ByRef guidString As String) As Boolean
			Return MyBase.GetGuidStringDefaultImplementation(guidString)
		End Function

		Friend Overridable Function GetInterfacesToEmit() As IEnumerable(Of NamedTypeSymbol)
			Dim namedTypeSymbols As IEnumerable(Of NamedTypeSymbol)
			Dim synthesizedImplements As IEnumerable(Of NamedTypeSymbol) = Me.GetSynthesizedImplements()
			Dim interfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol) = Me.InterfacesNoUseSiteDiagnostics
			If (Not interfacesNoUseSiteDiagnostics.IsEmpty) Then
				Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol = Me.BaseTypeNoUseSiteDiagnostics
				Dim namedTypeSymbols1 As IEnumerable(Of NamedTypeSymbol) = interfacesNoUseSiteDiagnostics.Where(Function(sym As NamedTypeSymbol)
					If (baseTypeNoUseSiteDiagnostics IsNot Nothing) Then
						Dim u0024VBu0024LocalBase As NamedTypeSymbol = baseTypeNoUseSiteDiagnostics
						Dim instanceCLRSignatureCompare As EqualsIgnoringComparer = EqualsIgnoringComparer.InstanceCLRSignatureCompare
						Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
						If (u0024VBu0024LocalBase.ImplementsInterface(sym, instanceCLRSignatureCompare, discarded)) Then
							Return Me.ImplementsAllMembersOfInterface(sym)
						End If
					End If
					Return True
				End Function)
				namedTypeSymbols = If(synthesizedImplements Is Nothing, namedTypeSymbols1, synthesizedImplements.Concat(namedTypeSymbols1))
			Else
				namedTypeSymbols = If(synthesizedImplements, SpecializedCollections.EmptyEnumerable(Of NamedTypeSymbol)())
			End If
			Return namedTypeSymbols
		End Function

		Public Overrides MustOverride Function GetMembers() As ImmutableArray(Of Symbol)

		Public Overrides MustOverride Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)

		Friend Overridable Function GetMembersForCci() As ImmutableArray(Of Symbol)
			Return Me.GetMembers()
		End Function

		Friend Overridable Function GetMethodsToEmit() As IEnumerable(Of MethodSymbol)
			Return New NamedTypeSymbol.VB$StateMachine_111_GetMethodsToEmit(-2) With
			{
				.$VB$Me = Me
			}
		End Function

		Friend Overridable Function GetPropertiesToEmit() As IEnumerable(Of PropertySymbol)
			Return New NamedTypeSymbol.VB$StateMachine_112_GetPropertiesToEmit(-2) With
			{
				.$VB$Me = Me
			}
		End Function

		Friend Function GetScriptConstructor() As SynthesizedConstructorBase
			Return DirectCast(Me.InstanceConstructors.[Single](), SynthesizedConstructorBase)
		End Function

		Friend Function GetScriptEntryPoint() As SynthesizedEntryPointSymbol
			Return DirectCast(Me.GetMembers(If(Me.TypeKind = Microsoft.CodeAnalysis.TypeKind.Submission, "<Factory>", "<Main>")).[Single](), SynthesizedEntryPointSymbol)
		End Function

		Friend Function GetScriptInitializer() As SynthesizedInteractiveInitializerMethod
			Return DirectCast(Me.GetMembers("<Initialize>").[Single](), SynthesizedInteractiveInitializerMethod)
		End Function

		Friend MustOverride Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)

		Friend Overridable Function GetSimpleNonTypeMembers(ByVal name As String) As ImmutableArray(Of Symbol)
			Return Me.GetMembers(name)
		End Function

		Friend Overridable Function GetSynthesizedImplements() As IEnumerable(Of NamedTypeSymbol)
			Return Nothing
		End Function

		Friend Overridable Function GetSynthesizedNestedTypes() As IEnumerable(Of INestedTypeDefinition)
			Return Nothing
		End Function

		Friend MustOverride Function GetSynthesizedWithEventsOverrides() As IEnumerable(Of PropertySymbol)

		Public MustOverride Function GetTypeArgumentCustomModifiers(ByVal ordinal As Integer) As ImmutableArray(Of CustomModifier)

		Public Overrides MustOverride Function GetTypeMembers() As ImmutableArray(Of NamedTypeSymbol)

		Public Overrides MustOverride Function GetTypeMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)

		Public Overrides MustOverride Function GetTypeMembers(ByVal name As String, ByVal arity As Integer) As ImmutableArray(Of NamedTypeSymbol)

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol)
			Dim code As Boolean
			If (Not MyBase.IsDefinition) Then
				Dim useSiteInfo1 As UseSiteInfo(Of AssemblySymbol) = MyBase.DeriveUseSiteInfoFromType(Me.OriginalDefinition)
				Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = useSiteInfo1.DiagnosticInfo
				If (diagnosticInfo IsNot Nothing) Then
					code = diagnosticInfo.Code = 30649
				Else
					code = False
				End If
				useSiteInfo = If(Not code, MyBase.MergeUseSiteInfo(useSiteInfo1, Me.DeriveUseSiteInfoFromTypeArguments()), useSiteInfo1)
			Else
				useSiteInfo = New UseSiteInfo(Of AssemblySymbol)(MyBase.PrimaryDependency)
			End If
			Return useSiteInfo
		End Function

		Private Function IGenericTypeInstanceReferenceGetGenericArguments(ByVal context As EmitContext) As ImmutableArray(Of ITypeReference) Implements IGenericTypeInstanceReference.GetGenericArguments
			Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
			Dim hasTypeArgumentsCustomModifiers As Boolean = Me.AdaptedNamedTypeSymbol.HasTypeArgumentsCustomModifiers
			Dim instance As ArrayBuilder(Of ITypeReference) = ArrayBuilder(Of ITypeReference).GetInstance()
			Dim typeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol) = Me.AdaptedNamedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics
			Dim length As Integer = typeArgumentsNoUseSiteDiagnostics.Length - 1
			Dim num As Integer = 0
			Do
				Dim modifiedTypeReference As ITypeReference = [module].Translate(typeArgumentsNoUseSiteDiagnostics(num), DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics)
				If (hasTypeArgumentsCustomModifiers) Then
					Dim typeArgumentCustomModifiers As ImmutableArray(Of CustomModifier) = Me.AdaptedNamedTypeSymbol.GetTypeArgumentCustomModifiers(num)
					If (Not typeArgumentCustomModifiers.IsDefaultOrEmpty) Then
						modifiedTypeReference = New Microsoft.Cci.ModifiedTypeReference(modifiedTypeReference, typeArgumentCustomModifiers.[As](Of ICustomModifier)())
					End If
				End If
				instance.Add(modifiedTypeReference)
				num = num + 1
			Loop While num <= length
			Return instance.ToImmutableAndFree()
		End Function

		Private Function IGenericTypeInstanceReferenceGetGenericType(ByVal context As EmitContext) As INamedTypeReference Implements IGenericTypeInstanceReference.GetGenericType
			Return Me(context)
		End Function

		Friend Function ImplementsAllMembersOfInterface(ByVal iface As NamedTypeSymbol) As Boolean
			Dim flag As Boolean
			Dim explicitInterfaceImplementationMap As MultiDictionary(Of Symbol, Symbol) = Me.ExplicitInterfaceImplementationMap
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = iface.GetMembersUnordered().GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As Symbol = enumerator.Current
					If (current.RequiresImplementation() AndAlso Not explicitInterfaceImplementationMap.ContainsKey(current)) Then
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

		Private Function ExplicitINamedTypeSymbol_Construct(ByVal ParamArray typeArguments As ITypeSymbol()) As INamedTypeSymbol Implements INamedTypeSymbol.Construct
			Return Me.Construct(Symbol.ConstructTypeArguments(typeArguments))
		End Function

		Private Function ExplicitINamedTypeSymbol_Construct(ByVal typeArguments As ImmutableArray(Of ITypeSymbol), ByVal typeArgumentNullableAnnotations As ImmutableArray(Of Microsoft.CodeAnalysis.NullableAnnotation)) As INamedTypeSymbol Implements INamedTypeSymbol.Construct
			Return Me.Construct(Symbol.ConstructTypeArguments(typeArguments, typeArgumentNullableAnnotations))
		End Function

		Private Function INamedTypeSymbol_ConstructUnboundGenericType() As INamedTypeSymbol Implements INamedTypeSymbol.ConstructUnboundGenericType
			Return Me.ConstructUnboundGenericType()
		End Function

		Private Function INamedTypeSymbol_GetTypeArgumentCustomModifiers(ByVal ordinal As Integer) As ImmutableArray(Of CustomModifier) Implements INamedTypeSymbol.GetTypeArgumentCustomModifiers
			Return Me.GetTypeArgumentCustomModifiers(ordinal)
		End Function

		Private Function INamespaceTypeReferenceGetUnit(ByVal context As EmitContext) As IUnitReference Implements INamespaceTypeReference.GetUnit
			Return DirectCast(context.[Module], PEModuleBuilder).Translate(Me.AdaptedNamedTypeSymbol.ContainingModule, context.Diagnostics)
		End Function

		Friend NotOverridable Overrides Function IReferenceAsDefinition(ByVal context As EmitContext) As IDefinition
			Return Me.AsTypeDefinitionImpl(DirectCast(context.[Module], PEModuleBuilder))
		End Function

		Friend NotOverridable Overrides Sub IReferenceDispatch(ByVal visitor As MetadataVisitor)
			If (Not Me.AdaptedNamedTypeSymbol.IsDefinition) Then
				If (Me.AdaptedNamedTypeSymbol.Arity > 0 AndAlso CObj(Me.AdaptedNamedTypeSymbol.ConstructedFrom) <> CObj(Me.AdaptedNamedTypeSymbol)) Then
					visitor.Visit(DirectCast(Me, IGenericTypeInstanceReference))
					Return
				End If
				visitor.Visit(DirectCast(Me, INestedTypeReference))
				Return
			End If
			Dim [module] As PEModuleBuilder = DirectCast(visitor.Context.[Module], PEModuleBuilder)
			Dim flag As Boolean = Me.AdaptedNamedTypeSymbol.ContainingModule.Equals([module].SourceModule)
			If (Me.AdaptedNamedTypeSymbol.ContainingType Is Nothing) Then
				If (flag) Then
					visitor.Visit(DirectCast(Me, INamespaceTypeDefinition))
					Return
				End If
				visitor.Visit(DirectCast(Me, INamespaceTypeReference))
				Return
			End If
			If (flag) Then
				visitor.Visit(DirectCast(Me, INestedTypeDefinition))
				Return
			End If
			visitor.Visit(DirectCast(Me, INestedTypeReference))
		End Sub

		Private Function ISpecializedNestedTypeReferenceGetUnspecializedVersion(ByVal context As EmitContext) As INestedTypeReference Implements ISpecializedNestedTypeReference.GetUnspecializedVersion
			Return Me(context).AsNestedTypeReference
		End Function

		Public NotOverridable Overrides Function IsTupleCompatible(<Out> ByRef tupleCardinality As Integer) As Boolean
			Dim flag As Boolean
			Dim typeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)
			Dim kind As Boolean
			Dim isGlobalNamespace As Boolean
			Dim arity As Integer
			If (Not Me.IsTupleType) Then
				If (Not Me.IsUnboundGenericType) Then
					Dim containingSymbol As Symbol = Me.ContainingSymbol
					If (containingSymbol IsNot Nothing) Then
						kind = containingSymbol.Kind = SymbolKind.[Namespace]
					Else
						kind = False
					End If
					If (kind) Then
						Dim containingNamespace As NamespaceSymbol = MyBase.ContainingNamespace.ContainingNamespace
						If (containingNamespace IsNot Nothing) Then
							isGlobalNamespace = containingNamespace.IsGlobalNamespace
						Else
							isGlobalNamespace = False
						End If
						If (isGlobalNamespace AndAlso EmbeddedOperators.CompareString(Me.Name, "ValueTuple", False) = 0 AndAlso EmbeddedOperators.CompareString(MyBase.ContainingNamespace.Name, "System", False) = 0) Then
							Dim num As Integer = Me.Arity
							If (num > 0 AndAlso num < 8) Then
								tupleCardinality = num
								flag = True
								Return flag
							ElseIf (num = 8 AndAlso Not MyBase.IsDefinition) Then
								Dim item As TypeSymbol = Me
								Dim num1 As Integer = 0
								Do
									num1 = num1 + 1
									typeArgumentsNoUseSiteDiagnostics = DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).TypeArgumentsNoUseSiteDiagnostics
									item = typeArgumentsNoUseSiteDiagnostics(7)
								Loop While TypeSymbol.Equals(item.OriginalDefinition, Me.OriginalDefinition, TypeCompareKind.ConsiderEverything) AndAlso Not item.IsDefinition
								If (Not item.IsTupleType) Then
									Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(item, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
									If (namedTypeSymbol IsNot Nothing) Then
										arity = namedTypeSymbol.Arity
									Else
										arity = 0
									End If
									num = arity
									If (num <= 0 OrElse num >= 8 OrElse Not item.IsTupleCompatible(tupleCardinality)) Then
										GoTo Label1
									End If
									tupleCardinality = tupleCardinality + 7 * num1
									flag = True
									Return flag
								Else
									Dim tupleUnderlyingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = item.TupleUnderlyingType
									If (tupleUnderlyingType.Arity <> 8 OrElse TypeSymbol.Equals(tupleUnderlyingType.OriginalDefinition, Me.OriginalDefinition, TypeCompareKind.ConsiderEverything)) Then
										typeArgumentsNoUseSiteDiagnostics = item.TupleElementTypes
										tupleCardinality = 7 * num1 + typeArgumentsNoUseSiteDiagnostics.Length
										flag = True
										Return flag
									Else
										tupleCardinality = 0
										flag = False
										Return flag
									End If
								End If
							End If
						End If
					End If
				End If
			Label1:
				tupleCardinality = 0
				flag = False
			Else
				tupleCardinality = 0
				flag = False
			End If
			Return flag
		End Function

		Private Function IsWithEventsField(ByVal field As FieldSymbol) As Boolean
			Return TypeOf field Is SourceWithEventsBackingFieldSymbol
		End Function

		Private Function ITypeDefinitionEvents(ByVal context As EmitContext) As IEnumerable(Of IEventDefinition) Implements ITypeDefinition.GetEvents
			Return New NamedTypeSymbol.VB$StateMachine_31_ITypeDefinitionEvents(-2) With
			{
				.$VB$Me = Me,
				.$P_context = context
			}
		End Function

		Private Function ITypeDefinitionGetBaseClass(ByVal context As EmitContext) As ITypeReference Implements ITypeDefinition.GetBaseClass
			Dim typeReference As ITypeReference
			Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
			Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol = Me.AdaptedNamedTypeSymbol.BaseTypeNoUseSiteDiagnostics
			If (Me.AdaptedNamedTypeSymbol.TypeKind = Microsoft.CodeAnalysis.TypeKind.Submission) Then
				baseTypeNoUseSiteDiagnostics = Me.AdaptedNamedTypeSymbol.ContainingAssembly.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Object)
			End If
			If (baseTypeNoUseSiteDiagnostics Is Nothing) Then
				typeReference = Nothing
			Else
				typeReference = [module].Translate(baseTypeNoUseSiteDiagnostics, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics, False, False)
			End If
			Return typeReference
		End Function

		Private Function ITypeDefinitionGetExplicitImplementationOverrides(ByVal context As EmitContext) As IEnumerable(Of Microsoft.Cci.MethodImplementation) Implements ITypeDefinition.GetExplicitImplementationOverrides
			Dim immutableAndFree As IEnumerable(Of Microsoft.Cci.MethodImplementation)
			Dim enumerator As IEnumerator(Of IMethodDefinition) = Nothing
			If (Not Me.AdaptedNamedTypeSymbol.IsInterface) Then
				Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
				Dim adaptedNamedTypeSymbol As SourceNamedTypeSymbol = TryCast(Me.AdaptedNamedTypeSymbol, SourceNamedTypeSymbol)
				Dim instance As ArrayBuilder(Of Microsoft.Cci.MethodImplementation) = ArrayBuilder(Of Microsoft.Cci.MethodImplementation).GetInstance()
				Dim enumerator1 As ImmutableArray(Of Symbol).Enumerator = Me.AdaptedNamedTypeSymbol.GetMembersForCci().GetEnumerator()
				While enumerator1.MoveNext()
					Dim current As Symbol = enumerator1.Current
					If (current.Kind <> SymbolKind.Method) Then
						Continue While
					End If
					Me.AddExplicitImplementations(context, DirectCast(current, MethodSymbol), instance, adaptedNamedTypeSymbol, [module])
				End While
				Dim synthesizedMethods As IEnumerable(Of IMethodDefinition) = [module].GetSynthesizedMethods(Me.AdaptedNamedTypeSymbol)
				If (synthesizedMethods IsNot Nothing) Then
					Try
						enumerator = synthesizedMethods.GetEnumerator()
						While enumerator.MoveNext()
							Dim internalSymbol As MethodSymbol = TryCast(enumerator.Current.GetInternalSymbol(), MethodSymbol)
							If (internalSymbol Is Nothing) Then
								Continue While
							End If
							Me.AddExplicitImplementations(context, internalSymbol, instance, adaptedNamedTypeSymbol, [module])
						End While
					Finally
						If (enumerator IsNot Nothing) Then
							enumerator.Dispose()
						End If
					End Try
				End If
				immutableAndFree = DirectCast(instance.ToImmutableAndFree(), IEnumerable(Of Microsoft.Cci.MethodImplementation))
			Else
				immutableAndFree = SpecializedCollections.EmptyEnumerable(Of Microsoft.Cci.MethodImplementation)()
			End If
			Return immutableAndFree
		End Function

		Private Function ITypeDefinitionGetFields(ByVal context As EmitContext) As IEnumerable(Of IFieldDefinition) Implements ITypeDefinition.GetFields
			Return New NamedTypeSymbol.VB$StateMachine_34_ITypeDefinitionGetFields(-2) With
			{
				.$VB$Me = Me,
				.$P_context = context
			}
		End Function

		Private Function ITypeDefinitionGetMethods(ByVal context As EmitContext) As IEnumerable(Of IMethodDefinition) Implements ITypeDefinition.GetMethods
			Return New NamedTypeSymbol.VB$StateMachine_69_ITypeDefinitionGetMethods(-2) With
			{
				.$VB$Me = Me,
				.$P_context = context
			}
		End Function

		Private Function ITypeDefinitionGetNestedTypes(ByVal context As EmitContext) As IEnumerable(Of INestedTypeDefinition) Implements ITypeDefinition.GetNestedTypes
			Dim nestedTypeDefinitions As IEnumerable(Of INestedTypeDefinition)
			Dim namedTypeSymbols As IEnumerable(Of NamedTypeSymbol)
			Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
			Dim typeMembers As ImmutableArray(Of NamedTypeSymbol) = Me.AdaptedNamedTypeSymbol.GetTypeMembers()
			If (typeMembers.Length <> 0) Then
				namedTypeSymbols = If(Not Me.AdaptedNamedTypeSymbol.IsEmbedded, DirectCast(typeMembers, IEnumerable(Of NamedTypeSymbol)), typeMembers.Where([module].SourceModule.ContainingSourceAssembly.DeclaringCompilation.EmbeddedSymbolManager.IsReferencedPredicate))
				nestedTypeDefinitions = namedTypeSymbols
			Else
				nestedTypeDefinitions = SpecializedCollections.EmptyEnumerable(Of INestedTypeDefinition)()
			End If
			Dim synthesizedTypes As IEnumerable(Of INestedTypeDefinition) = [module].GetSynthesizedTypes(Me.AdaptedNamedTypeSymbol)
			If (synthesizedTypes IsNot Nothing) Then
				nestedTypeDefinitions = nestedTypeDefinitions.Concat(synthesizedTypes)
			End If
			Return nestedTypeDefinitions
		End Function

		Private Function ITypeDefinitionGetProperties(ByVal context As EmitContext) As IEnumerable(Of IPropertyDefinition) Implements ITypeDefinition.GetProperties
			Return New NamedTypeSymbol.VB$StateMachine_71_ITypeDefinitionGetProperties(-2) With
			{
				.$VB$Me = Me,
				.$P_context = context
			}
		End Function

		Private Function ITypeDefinitionInterfaces(ByVal context As EmitContext) As IEnumerable(Of TypeReferenceWithAttributes) Implements ITypeDefinition.Interfaces
			Return New NamedTypeSymbol.VB$StateMachine_44_ITypeDefinitionInterfaces(-2) With
			{
				.$VB$Me = Me,
				.$P_context = context
			}
		End Function

		Private Function ITypeMemberReferenceGetContainingType(ByVal context As EmitContext) As ITypeReference
			Return DirectCast(context.[Module], PEModuleBuilder).Translate(Me.AdaptedNamedTypeSymbol.ContainingType, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics, False, Me.AdaptedNamedTypeSymbol.IsDefinition)
		End Function

		Private Function ITypeReferenceAsNamespaceTypeDefinition(ByVal context As EmitContext) As INamespaceTypeDefinition Implements ITypeReference.AsNamespaceTypeDefinition
			Dim namespaceTypeDefinition As INamespaceTypeDefinition
			Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
			If (Me.AdaptedNamedTypeSymbol.ContainingType IsNot Nothing OrElse Not Me.AdaptedNamedTypeSymbol.IsDefinition OrElse Not Me.AdaptedNamedTypeSymbol.ContainingModule.Equals([module].SourceModule)) Then
				namespaceTypeDefinition = Nothing
			Else
				namespaceTypeDefinition = Me
			End If
			Return namespaceTypeDefinition
		End Function

		Private Function ITypeReferenceAsNestedTypeDefinition(ByVal context As EmitContext) As INestedTypeDefinition Implements ITypeReference.AsNestedTypeDefinition
			Return Me.AsNestedTypeDefinitionImpl(DirectCast(context.[Module], PEModuleBuilder))
		End Function

		Private Function ITypeReferenceAsTypeDefinition(ByVal context As EmitContext) As ITypeDefinition Implements ITypeReference.AsTypeDefinition
			Return Me.AsTypeDefinitionImpl(DirectCast(context.[Module], PEModuleBuilder))
		End Function

		Private Function ITypeReferenceGetResolvedType(ByVal context As EmitContext) As ITypeDefinition Implements ITypeReference.GetResolvedType
			Return Me.AsTypeDefinitionImpl(DirectCast(context.[Module], PEModuleBuilder))
		End Function

		Friend MustOverride Function MakeAcyclicBaseType(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol

		Friend MustOverride Function MakeAcyclicInterfaces(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)

		Friend MustOverride Function MakeDeclaredBase(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol

		Friend MustOverride Function MakeDeclaredInterfaces(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)

		Friend Function TypeArgumentsWithDefinitionUseSiteDiagnostics(<InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ImmutableArray(Of TypeSymbol)
			Dim typeArgumentsNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol) = Me.TypeArgumentsNoUseSiteDiagnostics
			Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = typeArgumentsNoUseSiteDiagnostics.GetEnumerator()
			While enumerator.MoveNext()
				enumerator.Current.OriginalDefinition.AddUseSiteInfo(useSiteInfo)
			End While
			Return typeArgumentsNoUseSiteDiagnostics
		End Function

		Friend Function TypeArgumentWithDefinitionUseSiteDiagnostics(ByVal index As Integer, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As TypeSymbol
			Dim item As TypeSymbol = Me.TypeArgumentsNoUseSiteDiagnostics(index)
			item.OriginalDefinition.AddUseSiteInfo(useSiteInfo)
			Return item
		End Function
	End Class
End Namespace