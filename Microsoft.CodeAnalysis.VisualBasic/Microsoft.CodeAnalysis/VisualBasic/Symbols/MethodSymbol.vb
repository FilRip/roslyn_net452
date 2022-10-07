Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Reflection
Imports System.Reflection.Metadata
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class MethodSymbol
		Inherits Symbol
		Implements ITypeMemberReference, IMethodReference, IGenericMethodInstanceReference, ISpecializedMethodReference, ITypeDefinitionMember, IMethodDefinition, IMethodSymbol, IMethodSymbolInternal
		Public Const DisableJITOptimizationFlags As MethodImplAttributes = MethodImplAttributes.NoInlining Or MethodImplAttributes.NoOptimization

		Friend ReadOnly Property AdaptedMethodSymbol As MethodSymbol
			Get
				Return Me
			End Get
		End Property

		Public MustOverride ReadOnly Property Arity As Integer

		Public Overridable ReadOnly Property AssociatedAnonymousDelegate As NamedTypeSymbol
			Get
				Return Nothing
			End Get
		End Property

		Public MustOverride ReadOnly Property AssociatedSymbol As Symbol

		Friend MustOverride ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention

		Friend Overridable ReadOnly Property CallsiteReducedFromMethod As MethodSymbol
			Get
				Return Nothing
			End Get
		End Property

		Friend Overridable ReadOnly Property CanConstruct As Boolean
			Get
				If (Not MyBase.IsDefinition) Then
					Return False
				End If
				Return Me.Arity > 0
			End Get
		End Property

		Public Overridable ReadOnly Property ConstructedFrom As MethodSymbol
			Get
				Return Me
			End Get
		End Property

		Friend Overrides ReadOnly Property EmbeddedSymbolKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind
			Get
				If (Me.ContainingSymbol Is Nothing) Then
					Return Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind.None
				End If
				Return Me.ContainingSymbol.EmbeddedSymbolKind
			End Get
		End Property

		Public MustOverride ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)

		Friend Overridable ReadOnly Property FixedTypeParameters As ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeSymbol))
			Get
				Return ImmutableArray(Of KeyValuePair(Of TypeParameterSymbol, TypeSymbol)).Empty
			End Get
		End Property

		Friend ReadOnly Property GenerateDebugInfo As Boolean
			Get
				If (Not Me.GenerateDebugInfoImpl) Then
					Return False
				End If
				Return Not MyBase.IsEmbedded
			End Get
		End Property

		Friend MustOverride ReadOnly Property GenerateDebugInfoImpl As Boolean

		Public Overridable ReadOnly Property HandledEvents As ImmutableArray(Of HandledEvent)
			Get
				Return ImmutableArray(Of HandledEvent).Empty
			End Get
		End Property

		Friend MustOverride ReadOnly Property HasDeclarativeSecurity As Boolean

		Friend Overridable ReadOnly Property HasRuntimeSpecialName As Boolean
			Get
				If (Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.Constructor) Then
					Return True
				End If
				Return Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.StaticConstructor
			End Get
		End Property

		Friend MustOverride ReadOnly Property HasSpecialName As Boolean

		Public NotOverridable Overrides ReadOnly Property HasUnsupportedMetadata As Boolean
			Get
				Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = Me.GetUseSiteInfo().DiagnosticInfo
				If (diagnosticInfo Is Nothing) Then
					Return False
				End If
				Return diagnosticInfo.Code = 30657
			End Get
		End Property

		Protected Overrides ReadOnly Property HighestPriorityUseSiteError As Integer
			Get
				Return 30657
			End Get
		End Property

		ReadOnly Property IMethodDefinition_ContainingNamespace As INamespace Implements IMethodDefinition.ContainingNamespace
			Get
				Return Me.AdaptedMethodSymbol.ContainingNamespace.GetCciAdapter()
			End Get
		End Property

		ReadOnly Property IMethodDefinitionGenericParameters As IEnumerable(Of IGenericMethodParameter) Implements IMethodDefinition.GenericParameters
			Get
				Return DirectCast(Me.AdaptedMethodSymbol.TypeParameters, IEnumerable(Of IGenericMethodParameter))
			End Get
		End Property

		ReadOnly Property IMethodDefinitionHasDeclarativeSecurity As Boolean Implements IMethodDefinition.HasDeclarativeSecurity
			Get
				Return Me.AdaptedMethodSymbol.HasDeclarativeSecurity
			End Get
		End Property

		ReadOnly Property IMethodDefinitionIsAbstract As Boolean Implements IMethodDefinition.IsAbstract
			Get
				Return Me.AdaptedMethodSymbol.IsMustOverride
			End Get
		End Property

		ReadOnly Property IMethodDefinitionIsAccessCheckedOnOverride As Boolean Implements IMethodDefinition.IsAccessCheckedOnOverride
			Get
				Return Me.AdaptedMethodSymbol.IsAccessCheckedOnOverride
			End Get
		End Property

		ReadOnly Property IMethodDefinitionIsConstructor As Boolean Implements IMethodDefinition.IsConstructor
			Get
				Return Me.AdaptedMethodSymbol.MethodKind = Microsoft.CodeAnalysis.MethodKind.Constructor
			End Get
		End Property

		ReadOnly Property IMethodDefinitionIsExternal As Boolean Implements IMethodDefinition.IsExternal
			Get
				Return Me.AdaptedMethodSymbol.IsExternal
			End Get
		End Property

		ReadOnly Property IMethodDefinitionIsHiddenBySignature As Boolean Implements IMethodDefinition.IsHiddenBySignature
			Get
				Return Me.AdaptedMethodSymbol.IsHiddenBySignature
			End Get
		End Property

		ReadOnly Property IMethodDefinitionIsNewSlot As Boolean Implements IMethodDefinition.IsNewSlot
			Get
				Return Me.AdaptedMethodSymbol.IsMetadataNewSlot(False)
			End Get
		End Property

		ReadOnly Property IMethodDefinitionIsPlatformInvoke As Boolean Implements IMethodDefinition.IsPlatformInvoke
			Get
				Return Me.AdaptedMethodSymbol.GetDllImportData() IsNot Nothing
			End Get
		End Property

		ReadOnly Property IMethodDefinitionIsRuntimeSpecial As Boolean Implements IMethodDefinition.IsRuntimeSpecial
			Get
				Return Me.AdaptedMethodSymbol.HasRuntimeSpecialName
			End Get
		End Property

		ReadOnly Property IMethodDefinitionIsSealed As Boolean Implements IMethodDefinition.IsSealed
			Get
				Return Me.AdaptedMethodSymbol.IsMetadataFinal
			End Get
		End Property

		ReadOnly Property IMethodDefinitionIsSpecialName As Boolean Implements IMethodDefinition.IsSpecialName
			Get
				Return Me.AdaptedMethodSymbol.HasSpecialName
			End Get
		End Property

		ReadOnly Property IMethodDefinitionIsStatic As Boolean Implements IMethodDefinition.IsStatic
			Get
				Return Me.AdaptedMethodSymbol.IsShared
			End Get
		End Property

		ReadOnly Property IMethodDefinitionIsVirtual As Boolean Implements IMethodDefinition.IsVirtual
			Get
				Return Me.AdaptedMethodSymbol.IsMetadataVirtual()
			End Get
		End Property

		ReadOnly Property IMethodDefinitionParameters As ImmutableArray(Of IParameterDefinition) Implements IMethodDefinition.Parameters
			Get
				Return StaticCast(Of IParameterDefinition).From(Of ParameterSymbol)(Me.AdaptedMethodSymbol.Parameters)
			End Get
		End Property

		ReadOnly Property IMethodDefinitionPlatformInvokeData As IPlatformInvokeInformation Implements IMethodDefinition.PlatformInvokeData
			Get
				Return Me.AdaptedMethodSymbol.GetDllImportData()
			End Get
		End Property

		ReadOnly Property IMethodDefinitionRequiresSecurityObject As Boolean Implements IMethodDefinition.RequiresSecurityObject
			Get
				Return False
			End Get
		End Property

		ReadOnly Property IMethodDefinitionReturnValueIsMarshalledExplicitly As Boolean Implements IMethodDefinition.ReturnValueIsMarshalledExplicitly
			Get
				Return Me.AdaptedMethodSymbol.ReturnValueIsMarshalledExplicitly
			End Get
		End Property

		ReadOnly Property IMethodDefinitionReturnValueMarshallingDescriptor As ImmutableArray(Of Byte) Implements IMethodDefinition.ReturnValueMarshallingDescriptor
			Get
				Return Me.AdaptedMethodSymbol.ReturnValueMarshallingDescriptor
			End Get
		End Property

		ReadOnly Property IMethodDefinitionReturnValueMarshallingInformation As IMarshallingInformation Implements IMethodDefinition.ReturnValueMarshallingInformation
			Get
				Return Me.AdaptedMethodSymbol.ReturnTypeMarshallingInformation
			End Get
		End Property

		ReadOnly Property IMethodDefinitionSecurityAttributes As IEnumerable(Of SecurityAttribute) Implements IMethodDefinition.SecurityAttributes
			Get
				Return Me.AdaptedMethodSymbol.GetSecurityInformation()
			End Get
		End Property

		ReadOnly Property IMethodReferenceAcceptsExtraArguments As Boolean Implements IMethodReference.AcceptsExtraArguments
			Get
				Return Me.AdaptedMethodSymbol.IsVararg
			End Get
		End Property

		ReadOnly Property IMethodReferenceAsGenericMethodInstanceReference As IGenericMethodInstanceReference Implements IMethodReference.AsGenericMethodInstanceReference
			Get
				Dim genericMethodInstanceReference As IGenericMethodInstanceReference
				If (Me.AdaptedMethodSymbol.IsDefinition OrElse Not Me.AdaptedMethodSymbol.IsGenericMethod OrElse CObj(Me.AdaptedMethodSymbol) = CObj(Me.AdaptedMethodSymbol.ConstructedFrom)) Then
					genericMethodInstanceReference = Nothing
				Else
					genericMethodInstanceReference = Me
				End If
				Return genericMethodInstanceReference
			End Get
		End Property

		ReadOnly Property IMethodReferenceAsSpecializedMethodReference As ISpecializedMethodReference Implements IMethodReference.AsSpecializedMethodReference
			Get
				Dim specializedMethodReference As ISpecializedMethodReference
				If (Me.AdaptedMethodSymbol.IsDefinition OrElse Me.AdaptedMethodSymbol.IsGenericMethod AndAlso CObj(Me.AdaptedMethodSymbol) <> CObj(Me.AdaptedMethodSymbol.ConstructedFrom)) Then
					specializedMethodReference = Nothing
				Else
					specializedMethodReference = Me
				End If
				Return specializedMethodReference
			End Get
		End Property

		ReadOnly Property IMethodReferenceExtraParameters As ImmutableArray(Of IParameterTypeInformation) Implements IMethodReference.ExtraParameters
			Get
				Return ImmutableArray(Of IParameterTypeInformation).Empty
			End Get
		End Property

		ReadOnly Property IMethodReferenceGenericParameterCount As UShort Implements IMethodReference.GenericParameterCount
			Get
				Return CUShort(Me.AdaptedMethodSymbol.Arity)
			End Get
		End Property

		ReadOnly Property IMethodReferenceIsGeneric As Boolean Implements IMethodReference.IsGeneric
			Get
				Return Me.AdaptedMethodSymbol.IsGenericMethod
			End Get
		End Property

		ReadOnly Property IMethodReferenceParameterCount As UShort
			Get
				Return CUShort(Me.AdaptedMethodSymbol.ParameterCount)
			End Get
		End Property

		ReadOnly Property IMethodSymbol_Arity As Integer Implements IMethodSymbol.Arity
			Get
				Return Me.Arity
			End Get
		End Property

		ReadOnly Property IMethodSymbol_AssociatedAnonymousDelegate As INamedTypeSymbol Implements IMethodSymbol.AssociatedAnonymousDelegate
			Get
				Return Me.AssociatedAnonymousDelegate
			End Get
		End Property

		ReadOnly Property IMethodSymbol_AssociatedSymbol As ISymbol Implements IMethodSymbol.AssociatedSymbol
			Get
				Return Me.AssociatedSymbol
			End Get
		End Property

		ReadOnly Property IMethodSymbol_CallingConvention As SignatureCallingConvention Implements IMethodSymbol.CallingConvention
			Get
				Return Me.CallingConvention.ToSignatureConvention()
			End Get
		End Property

		ReadOnly Property IMethodSymbol_ConstructedFrom As IMethodSymbol Implements IMethodSymbol.ConstructedFrom
			Get
				Return Me.ConstructedFrom
			End Get
		End Property

		ReadOnly Property IMethodSymbol_ExplicitInterfaceImplementations As ImmutableArray(Of IMethodSymbol) Implements IMethodSymbol.ExplicitInterfaceImplementations
			Get
				Return Me.ExplicitInterfaceImplementations.Cast(Of IMethodSymbol)()
			End Get
		End Property

		ReadOnly Property IMethodSymbol_HidesBaseMethodsByName As Boolean Implements IMethodSymbol.HidesBaseMethodsByName
			Get
				Return True
			End Get
		End Property

		ReadOnly Property IMethodSymbol_IsAsync As Boolean Implements IMethodSymbol.IsAsync, IMethodSymbolInternal.IsAsync
			Get
				Return Me.IsAsync
			End Get
		End Property

		ReadOnly Property IMethodSymbol_IsExtensionMethod As Boolean Implements IMethodSymbol.IsExtensionMethod
			Get
				Return Me.IsExtensionMethod
			End Get
		End Property

		ReadOnly Property IMethodSymbol_IsGenericMethod As Boolean Implements IMethodSymbol.IsGenericMethod
			Get
				Return Me.IsGenericMethod
			End Get
		End Property

		ReadOnly Property IMethodSymbol_IsInitOnly As Boolean Implements IMethodSymbol.IsInitOnly
			Get
				Return Me.IsInitOnly
			End Get
		End Property

		ReadOnly Property IMethodSymbol_IsPartialDefinition As Boolean Implements IMethodSymbol.IsPartialDefinition
			Get
				Dim sourceMemberMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol = TryCast(Me, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol)
				If (sourceMemberMethodSymbol IsNot Nothing) Then
					Return sourceMemberMethodSymbol.IsPartialDefinition
				End If
				Return False
			End Get
		End Property

		ReadOnly Property IMethodSymbol_IsReadOnly As Boolean Implements IMethodSymbol.IsReadOnly
			Get
				Return False
			End Get
		End Property

		ReadOnly Property IMethodSymbol_MethodImplementationFlags As MethodImplAttributes Implements IMethodSymbol.MethodImplementationFlags
			Get
				Return Me.ImplementationAttributes
			End Get
		End Property

		ReadOnly Property IMethodSymbol_MethodKind As Microsoft.CodeAnalysis.MethodKind Implements IMethodSymbol.MethodKind
			Get
				Return Me.MethodKind
			End Get
		End Property

		ReadOnly Property IMethodSymbol_OriginalDefinition As IMethodSymbol Implements IMethodSymbol.OriginalDefinition
			Get
				Return Me.OriginalDefinition
			End Get
		End Property

		ReadOnly Property IMethodSymbol_OverriddenMethod As IMethodSymbol Implements IMethodSymbol.OverriddenMethod
			Get
				Return Me.OverriddenMethod
			End Get
		End Property

		ReadOnly Property IMethodSymbol_Parameters As ImmutableArray(Of IParameterSymbol) Implements IMethodSymbol.Parameters
			Get
				Return ImmutableArray(Of IParameterSymbol).CastUp(Of ParameterSymbol)(Me.Parameters)
			End Get
		End Property

		ReadOnly Property IMethodSymbol_PartialDefinitionPart As IMethodSymbol Implements IMethodSymbol.PartialDefinitionPart
			Get
				Return Me.PartialDefinitionPart
			End Get
		End Property

		ReadOnly Property IMethodSymbol_PartialImplementationPart As IMethodSymbol Implements IMethodSymbol.PartialImplementationPart
			Get
				Return Me.PartialImplementationPart
			End Get
		End Property

		ReadOnly Property IMethodSymbol_ReceiverNullableAnnotation As NullableAnnotation Implements IMethodSymbol.ReceiverNullableAnnotation
			Get
				Return NullableAnnotation.None
			End Get
		End Property

		ReadOnly Property IMethodSymbol_ReceiverType As ITypeSymbol Implements IMethodSymbol.ReceiverType
			Get
				Return Me.ReceiverType
			End Get
		End Property

		ReadOnly Property IMethodSymbol_ReducedFrom As IMethodSymbol Implements IMethodSymbol.ReducedFrom
			Get
				Return Me.ReducedFrom
			End Get
		End Property

		ReadOnly Property IMethodSymbol_RefCustomModifiers As ImmutableArray(Of CustomModifier) Implements IMethodSymbol.RefCustomModifiers
			Get
				Return Me.RefCustomModifiers
			End Get
		End Property

		ReadOnly Property IMethodSymbol_RefKind As Microsoft.CodeAnalysis.RefKind Implements IMethodSymbol.RefKind
			Get
				If (Not Me.ReturnsByRef) Then
					Return Microsoft.CodeAnalysis.RefKind.None
				End If
				Return Microsoft.CodeAnalysis.RefKind.Ref
			End Get
		End Property

		ReadOnly Property IMethodSymbol_ReturnNullableAnnotation As NullableAnnotation Implements IMethodSymbol.ReturnNullableAnnotation
			Get
				Return NullableAnnotation.None
			End Get
		End Property

		ReadOnly Property IMethodSymbol_ReturnsByReadonlyRef As Boolean Implements IMethodSymbol.ReturnsByRefReadonly
			Get
				Return False
			End Get
		End Property

		ReadOnly Property IMethodSymbol_ReturnsByRef As Boolean Implements IMethodSymbol.ReturnsByRef
			Get
				Return Me.ReturnsByRef
			End Get
		End Property

		ReadOnly Property IMethodSymbol_ReturnsVoid As Boolean Implements IMethodSymbol.ReturnsVoid
			Get
				Return Me.IsSub
			End Get
		End Property

		ReadOnly Property IMethodSymbol_ReturnType As ITypeSymbol Implements IMethodSymbol.ReturnType
			Get
				Return Me.ReturnType
			End Get
		End Property

		ReadOnly Property IMethodSymbol_ReturnTypeCustomModifiers As ImmutableArray(Of CustomModifier) Implements IMethodSymbol.ReturnTypeCustomModifiers
			Get
				Return Me.ReturnTypeCustomModifiers
			End Get
		End Property

		ReadOnly Property IMethodSymbol_TypeArguments As ImmutableArray(Of ITypeSymbol) Implements IMethodSymbol.TypeArguments
			Get
				Return StaticCast(Of ITypeSymbol).From(Of TypeSymbol)(Me.TypeArguments)
			End Get
		End Property

		ReadOnly Property IMethodSymbol_TypeArgumentsNullableAnnotation As ImmutableArray(Of NullableAnnotation) Implements IMethodSymbol.TypeArgumentNullableAnnotations
			Get
				Dim func As Func(Of TypeSymbol, NullableAnnotation)
				Dim typeArguments As ImmutableArray(Of TypeSymbol) = Me.TypeArguments
				If (MethodSymbol._Closure$__.$I317-0 Is Nothing) Then
					func = Function(t As TypeSymbol) NullableAnnotation.None
					MethodSymbol._Closure$__.$I317-0 = func
				Else
					func = MethodSymbol._Closure$__.$I317-0
				End If
				Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of TypeSymbol, NullableAnnotation)(typeArguments, func)
			End Get
		End Property

		ReadOnly Property IMethodSymbol_TypeParameters As ImmutableArray(Of ITypeParameterSymbol) Implements IMethodSymbol.TypeParameters
			Get
				Return StaticCast(Of ITypeParameterSymbol).From(Of TypeParameterSymbol)(Me.TypeParameters)
			End Get
		End Property

		ReadOnly Property IMethodSymbol_UnmanagedCallingConventionTypes As ImmutableArray(Of INamedTypeSymbol) Implements IMethodSymbol.UnmanagedCallingConventionTypes
			Get
				Return ImmutableArray(Of INamedTypeSymbol).Empty
			End Get
		End Property

		ReadOnly Property IMethodSymbolInternal_IsIterator As Boolean Implements IMethodSymbolInternal.IsIterator
			Get
				Return Me.IsIterator
			End Get
		End Property

		Friend MustOverride ReadOnly Property ImplementationAttributes As MethodImplAttributes

		Friend Overrides ReadOnly Property ImplicitlyDefinedBy(ByVal membersInProgress As Dictionary(Of String, ArrayBuilder(Of Symbol))) As Symbol
			Get
				Return Me.AssociatedSymbol
			End Get
		End Property

		ReadOnly Property INamedEntityName As String
			Get
				Return Me.AdaptedMethodSymbol.MetadataName
			End Get
		End Property

		Friend Overridable ReadOnly Property IsAccessCheckedOnOverride As Boolean
			Get
				Return Me.IsMetadataVirtual()
			End Get
		End Property

		Public MustOverride ReadOnly Property IsAsync As Boolean

		Public Overridable ReadOnly Property IsCheckedBuiltin As Boolean Implements IMethodSymbol.IsCheckedBuiltin
			Get
				Return False
			End Get
		End Property

		Public ReadOnly Property IsConditional As Boolean Implements IMethodSymbol.IsConditional
			Get
				Return System.Linq.ImmutableArrayExtensions.Any(Of String)(Me.GetAppliedConditionalSymbols())
			End Get
		End Property

		Friend Overridable ReadOnly Property IsDirectlyExcludedFromCodeCoverage As Boolean
			Get
				Return False
			End Get
		End Property

		Friend ReadOnly Property IsEntryPointCandidate As Boolean
			Get
				Dim flag As Boolean
				If (Me.ContainingType.IsEmbedded) Then
					flag = False
				ElseIf (Not Me.IsSubmissionConstructor) Then
					flag = If(Not Me.IsImplicitlyDeclared, [String].Equals(Me.Name, "Main", StringComparison.OrdinalIgnoreCase), False)
				Else
					flag = False
				End If
				Return flag
			End Get
		End Property

		Public MustOverride ReadOnly Property IsExtensionMethod As Boolean

		Friend Overridable ReadOnly Property IsExternal As Boolean
			Get
				Return Me.IsExternalMethod
			End Get
		End Property

		Public MustOverride ReadOnly Property IsExternalMethod As Boolean

		Public Overridable ReadOnly Property IsGenericMethod As Boolean
			Get
				Return Me.Arity <> 0
			End Get
		End Property

		Friend Overridable ReadOnly Property IsHiddenBySignature As Boolean
			Get
				Return Me.IsOverloads
			End Get
		End Property

		ReadOnly Property ISignatureCallingConvention As Microsoft.Cci.CallingConvention
			Get
				Return Me.AdaptedMethodSymbol.CallingConvention
			End Get
		End Property

		ReadOnly Property ISignatureRefCustomModifiers As ImmutableArray(Of ICustomModifier)
			Get
				Return Me.AdaptedMethodSymbol.RefCustomModifiers.[As](Of ICustomModifier)()
			End Get
		End Property

		ReadOnly Property ISignatureReturnValueCustomModifiers As ImmutableArray(Of ICustomModifier)
			Get
				Return Me.AdaptedMethodSymbol.ReturnTypeCustomModifiers.[As](Of ICustomModifier)()
			End Get
		End Property

		ReadOnly Property ISignatureReturnValueIsByRef As Boolean
			Get
				Return Me.AdaptedMethodSymbol.ReturnsByRef
			End Get
		End Property

		Public MustOverride ReadOnly Property IsInitOnly As Boolean

		Public MustOverride ReadOnly Property IsIterator As Boolean

		Friend Overridable ReadOnly Property IsMetadataFinal As Boolean
			Get
				If (Me.IsNotOverridable) Then
					Return True
				End If
				If (Not Me.IsMetadataVirtual()) Then
					Return False
				End If
				If (Me.IsOverridable OrElse Me.IsMustOverride) Then
					Return False
				End If
				Return Not Me.IsOverrides
			End Get
		End Property

		Friend MustOverride ReadOnly Property IsMethodKindBasedOnSyntax As Boolean

		Public MustOverride ReadOnly Property IsOverloads As Boolean

		ReadOnly Property ISpecializedMethodReferenceUnspecializedVersion As IMethodReference Implements ISpecializedMethodReference.UnspecializedVersion
			Get
				Return Me.AdaptedMethodSymbol.OriginalDefinition.GetCciAdapter()
			End Get
		End Property

		Friend ReadOnly Property IsReducedExtensionMethod As Boolean
			Get
				Return CObj(Me.ReducedFrom) <> CObj(Nothing)
			End Get
		End Property

		Friend ReadOnly Property IsRuntimeImplemented As Boolean
			Get
				Return (Me.ImplementationAttributes And MethodImplAttributes.CodeTypeMask) <> MethodImplAttributes.IL
			End Get
		End Property

		Friend ReadOnly Property IsScriptConstructor As Boolean
			Get
				If (Me.MethodKind <> Microsoft.CodeAnalysis.MethodKind.Constructor) Then
					Return False
				End If
				Return Me.ContainingType.IsScriptClass
			End Get
		End Property

		Friend Overridable ReadOnly Property IsScriptInitializer As Boolean
			Get
				Return False
			End Get
		End Property

		Public MustOverride ReadOnly Property IsSub As Boolean

		Friend ReadOnly Property IsSubmissionConstructor As Boolean
			Get
				If (Not Me.IsScriptConstructor) Then
					Return False
				End If
				Return Me.ContainingAssembly.IsInteractive
			End Get
		End Property

		Public Overridable ReadOnly Property IsTupleMethod As Boolean
			Get
				Return False
			End Get
		End Property

		Public MustOverride ReadOnly Property IsVararg As Boolean Implements IMethodSymbol.IsVararg

		Friend ReadOnly Property IsViableMainMethod As Boolean
			Get
				If (Not Me.IsShared OrElse Not Me.IsAccessibleEntryPoint()) Then
					Return False
				End If
				Return Me.HasEntryPointSignature()
			End Get
		End Property

		ReadOnly Property ISymbol_IsExtern As Boolean
			Get
				Return Me.IsExternalMethod
			End Get
		End Property

		ReadOnly Property ITypeDefinitionMemberContainingTypeDefinition As ITypeDefinition Implements ITypeDefinitionMember.ContainingTypeDefinition
			Get
				Dim cciAdapter As ITypeDefinition
				Dim adaptedMethodSymbol As SynthesizedGlobalMethodBase = TryCast(Me.AdaptedMethodSymbol, SynthesizedGlobalMethodBase)
				If (adaptedMethodSymbol Is Nothing) Then
					cciAdapter = Me.AdaptedMethodSymbol.ContainingType.GetCciAdapter()
				Else
					cciAdapter = adaptedMethodSymbol.ContainingPrivateImplementationDetailsType
				End If
				Return cciAdapter
			End Get
		End Property

		ReadOnly Property ITypeDefinitionMemberVisibility As TypeMemberVisibility Implements ITypeDefinitionMember.Visibility
			Get
				Return PEModuleBuilder.MemberVisibility(Me.AdaptedMethodSymbol)
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Kind As SymbolKind
			Get
				Return SymbolKind.Method
			End Get
		End Property

		Friend Overridable ReadOnly Property MayBeReducibleExtensionMethod As Boolean
			Get
				If (Not Me.IsExtensionMethod) Then
					Return False
				End If
				Return Me.MethodKind <> Microsoft.CodeAnalysis.MethodKind.ReducedExtension
			End Get
		End Property

		Friend ReadOnly Property MeParameter As ParameterSymbol
			Get
				Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = Nothing
				If (Not Me.TryGetMeParameter(parameterSymbol)) Then
					Throw ExceptionUtilities.Unreachable
				End If
				Return parameterSymbol
			End Get
		End Property

		Public MustOverride ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind

		Public Shadows Overridable ReadOnly Property OriginalDefinition As MethodSymbol
			Get
				Return Me
			End Get
		End Property

		Protected NotOverridable Overrides ReadOnly Property OriginalSymbolDefinition As Symbol
			Get
				Return Me.OriginalDefinition
			End Get
		End Property

		Friend Overridable ReadOnly Property OverriddenMembers As OverriddenMembersResult(Of MethodSymbol)
			Get
				Return OverrideHidingHelper(Of MethodSymbol).MakeOverriddenMembers(Me)
			End Get
		End Property

		Public Overridable ReadOnly Property OverriddenMethod As MethodSymbol
			Get
				Dim accessorOverride As MethodSymbol
				If (Me.IsAccessor() AndAlso Me.AssociatedSymbol.Kind = SymbolKind.[Property]) Then
					accessorOverride = DirectCast(Me.AssociatedSymbol, PropertySymbol).GetAccessorOverride(Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.PropertyGet)
				ElseIf (Not Me.IsOverrides OrElse CObj(Me.ConstructedFrom) <> CObj(Me)) Then
					accessorOverride = Nothing
				Else
					accessorOverride = If(Not MyBase.IsDefinition, OverriddenMembersResult(Of MethodSymbol).GetOverriddenMember(Me, Me.OriginalDefinition.OverriddenMethod), Me.OverriddenMembers.OverriddenMember)
				End If
				Return accessorOverride
			End Get
		End Property

		Friend Overridable ReadOnly Property ParameterCount As Integer
			Get
				Return Me.Parameters.Length
			End Get
		End Property

		Public MustOverride ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)

		Public Overridable ReadOnly Property PartialDefinitionPart As MethodSymbol
			Get
				Return Nothing
			End Get
		End Property

		Public Overridable ReadOnly Property PartialImplementationPart As MethodSymbol
			Get
				Return Nothing
			End Get
		End Property

		Friend Overridable ReadOnly Property PreserveOriginalLocals As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overridable ReadOnly Property Proximity As Integer
			Get
				Return 0
			End Get
		End Property

		Public Overridable ReadOnly Property ReceiverType As TypeSymbol
			Get
				Return Me.ContainingType
			End Get
		End Property

		Public Overridable ReadOnly Property ReducedFrom As MethodSymbol
			Get
				Return Nothing
			End Get
		End Property

		Public MustOverride ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)

		Public MustOverride ReadOnly Property ReturnsByRef As Boolean

		Public MustOverride ReadOnly Property ReturnType As TypeSymbol

		Public MustOverride ReadOnly Property ReturnTypeCustomModifiers As ImmutableArray(Of CustomModifier)

		Friend MustOverride ReadOnly Property ReturnTypeMarshallingInformation As MarshalPseudoCustomAttributeData

		Friend Overridable ReadOnly Property ReturnValueIsMarshalledExplicitly As Boolean
			Get
				Return Me.ReturnTypeMarshallingInformation IsNot Nothing
			End Get
		End Property

		Friend Overridable ReadOnly Property ReturnValueMarshallingDescriptor As ImmutableArray(Of Byte)
			Get
				Return New ImmutableArray(Of Byte)()
			End Get
		End Property

		Friend MustOverride ReadOnly Property Syntax As SyntaxNode

		Public Overridable ReadOnly Property TupleUnderlyingMethod As MethodSymbol
			Get
				Return Nothing
			End Get
		End Property

		Public MustOverride ReadOnly Property TypeArguments As ImmutableArray(Of TypeSymbol)

		Public MustOverride ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)

		Friend Sub New()
			MyBase.New()
		End Sub

		Friend Overrides Function Accept(Of TArgument, TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TArgument, TResult), ByVal arg As TArgument) As TResult
			Return visitor.VisitMethod(Me, arg)
		End Function

		Public Overrides Sub Accept(ByVal visitor As SymbolVisitor)
			visitor.VisitMethod(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As SymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitMethod(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As VisualBasicSymbolVisitor)
			visitor.VisitMethod(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitMethod(Me)
		End Function

		Friend Overridable Sub AddSynthesizedReturnTypeAttributes(ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
		End Sub

		Friend MustOverride Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer

		Friend Function CalculateUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol)
			Dim code As Boolean
			Dim flag As Boolean
			Dim code1 As Boolean
			Dim flag1 As Boolean
			Dim useSiteInfo1 As UseSiteInfo(Of AssemblySymbol) = MyBase.MergeUseSiteInfo(New UseSiteInfo(Of AssemblySymbol)(MyBase.PrimaryDependency), MyBase.DeriveUseSiteInfoFromType(Me.ReturnType))
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = useSiteInfo1.DiagnosticInfo
			If (diagnosticInfo IsNot Nothing) Then
				code = diagnosticInfo.Code = 30657
			Else
				code = False
			End If
			If (Not code) Then
				Dim useSiteInfo2 As UseSiteInfo(Of AssemblySymbol) = MyBase.DeriveUseSiteInfoFromCustomModifiers(Me.RefCustomModifiers, False)
				Dim diagnosticInfo1 As Microsoft.CodeAnalysis.DiagnosticInfo = useSiteInfo2.DiagnosticInfo
				If (diagnosticInfo1 IsNot Nothing) Then
					flag = diagnosticInfo1.Code = 30657
				Else
					flag = False
				End If
				If (Not flag) Then
					Dim useSiteInfo3 As UseSiteInfo(Of AssemblySymbol) = MyBase.DeriveUseSiteInfoFromCustomModifiers(Me.ReturnTypeCustomModifiers, Me.IsInitOnly)
					Dim diagnosticInfo2 As Microsoft.CodeAnalysis.DiagnosticInfo = useSiteInfo3.DiagnosticInfo
					If (diagnosticInfo2 IsNot Nothing) Then
						code1 = diagnosticInfo2.Code = 30657
					Else
						code1 = False
					End If
					If (Not code1) Then
						Dim useSiteInfo4 As UseSiteInfo(Of AssemblySymbol) = MyBase.DeriveUseSiteInfoFromParameters(Me.Parameters)
						Dim diagnosticInfo3 As Microsoft.CodeAnalysis.DiagnosticInfo = useSiteInfo4.DiagnosticInfo
						If (diagnosticInfo3 IsNot Nothing) Then
							flag1 = diagnosticInfo3.Code = 30657
						Else
							flag1 = False
						End If
						If (Not flag1) Then
							Dim unificationUseSiteDiagnosticRecursive As Microsoft.CodeAnalysis.DiagnosticInfo = If(useSiteInfo1.DiagnosticInfo, (If(useSiteInfo2.DiagnosticInfo, (If(useSiteInfo3.DiagnosticInfo, useSiteInfo4.DiagnosticInfo)))))
							If (unificationUseSiteDiagnosticRecursive Is Nothing AndAlso Me.ContainingModule.HasUnifiedReferences) Then
								Dim typeSymbols As HashSet(Of TypeSymbol) = Nothing
								unificationUseSiteDiagnosticRecursive = If(Me.ReturnType.GetUnificationUseSiteDiagnosticRecursive(Me, typeSymbols), (If(Symbol.GetUnificationUseSiteDiagnosticRecursive(Me.RefCustomModifiers, Me, typeSymbols), (If(Symbol.GetUnificationUseSiteDiagnosticRecursive(Me.ReturnTypeCustomModifiers, Me, typeSymbols), (If(Symbol.GetUnificationUseSiteDiagnosticRecursive(Me.Parameters, Me, typeSymbols), Symbol.GetUnificationUseSiteDiagnosticRecursive(Me.TypeParameters, Me, typeSymbols))))))))
							End If
							If (unificationUseSiteDiagnosticRecursive Is Nothing) Then
								Dim primaryDependency As AssemblySymbol = useSiteInfo1.PrimaryDependency
								Dim secondaryDependencies As ImmutableHashSet(Of AssemblySymbol) = useSiteInfo1.SecondaryDependencies
								useSiteInfo2.MergeDependencies(primaryDependency, secondaryDependencies)
								useSiteInfo3.MergeDependencies(primaryDependency, secondaryDependencies)
								useSiteInfo4.MergeDependencies(primaryDependency, secondaryDependencies)
								useSiteInfo = New UseSiteInfo(Of AssemblySymbol)(Nothing, primaryDependency, secondaryDependencies)
							Else
								useSiteInfo = New UseSiteInfo(Of AssemblySymbol)(unificationUseSiteDiagnosticRecursive)
							End If
						Else
							useSiteInfo = useSiteInfo4
						End If
					Else
						useSiteInfo = useSiteInfo3
					End If
				Else
					useSiteInfo = useSiteInfo2
				End If
			Else
				useSiteInfo = useSiteInfo1
			End If
			Return useSiteInfo
		End Function

		Private Function CallsAreConditionallyOmitted(ByVal atNode As SyntaxNodeOrToken, ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree) As Boolean
			Dim flag As Boolean
			Dim containingType As NamedTypeSymbol = Me.ContainingType
			If (Not Me.IsConditional OrElse Not Me.IsSub OrElse Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.PropertySet OrElse containingType IsNot Nothing AndAlso containingType.IsInterfaceType()) Then
				flag = False
			Else
				flag = If(Not syntaxTree.IsAnyPreprocessorSymbolDefined(DirectCast(Me.GetAppliedConditionalSymbols(), IEnumerable(Of String)), atNode), True, False)
			End If
			Return flag
		End Function

		Friend Overridable Function CallsAreOmitted(ByVal atNode As SyntaxNodeOrToken, ByVal syntaxTree As Microsoft.CodeAnalysis.SyntaxTree) As Boolean
			If (Me.IsPartialWithoutImplementation()) Then
				Return True
			End If
			If (syntaxTree Is Nothing) Then
				Return False
			End If
			Return Me.CallsAreConditionallyOmitted(atNode, syntaxTree)
		End Function

		Protected Sub CheckCanConstructAndTypeArguments(ByVal typeArguments As ImmutableArray(Of TypeSymbol))
			If (Not Me.CanConstruct OrElse CObj(Me) <> CObj(Me.ConstructedFrom)) Then
				Throw New InvalidOperationException()
			End If
			typeArguments.CheckTypeArguments(Me.Arity)
		End Sub

		Public Overridable Function Construct(ByVal typeArguments As ImmutableArray(Of TypeSymbol)) As MethodSymbol
			Dim constructedNotSpecializedGenericMethod As MethodSymbol
			Me.CheckCanConstructAndTypeArguments(typeArguments)
			Dim typeSubstitution As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution = Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSubstitution.Create(Me, Me.TypeParameters, typeArguments, True)
			If (typeSubstitution IsNot Nothing) Then
				constructedNotSpecializedGenericMethod = New SubstitutedMethodSymbol.ConstructedNotSpecializedGenericMethod(typeSubstitution, typeArguments)
			Else
				constructedNotSpecializedGenericMethod = Me
			End If
			Return constructedNotSpecializedGenericMethod
		End Function

		Public Function Construct(ByVal ParamArray typeArguments As TypeSymbol()) As MethodSymbol
			Return Me.Construct(ImmutableArray.Create(Of TypeSymbol)(typeArguments))
		End Function

		Private Function EnumerateDefinitionParameters() As ImmutableArray(Of IParameterTypeInformation)
			Return StaticCast(Of IParameterTypeInformation).From(Of ParameterSymbol)(Me.AdaptedMethodSymbol.Parameters)
		End Function

		Friend MustOverride Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)

		Friend Overridable Function GetBoundMethodBody(ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, <Out> Optional ByRef methodBodyBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing) As BoundBlock
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Shadows Function GetCciAdapter() As MethodSymbol
			Return Me
		End Function

		Public MustOverride Function GetDllImportData() As DllImportData Implements IMethodSymbol.GetDllImportData

		Public Overridable Function GetReturnTypeAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return ImmutableArray(Of VisualBasicAttributeData).Empty
		End Function

		Friend MustOverride Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)

		Public Overridable Function GetTypeInferredDuringReduction(ByVal reducedFromTypeParameter As TypeParameterSymbol) As TypeSymbol
			Throw New InvalidOperationException()
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol)
			useSiteInfo = If(Not MyBase.IsDefinition, Me.OriginalDefinition.GetUseSiteInfo(), New UseSiteInfo(Of AssemblySymbol)(MyBase.PrimaryDependency))
			Return useSiteInfo
		End Function

		Friend Function HasEntryPointSignature() As Boolean
			Dim flag As Boolean
			Dim returnType As TypeSymbol = Me.ReturnType
			If (returnType.SpecialType <> SpecialType.System_Int32 AndAlso returnType.SpecialType <> SpecialType.System_Void) Then
				flag = False
			ElseIf (Me.Parameters.Length = 0) Then
				flag = True
			ElseIf (Me.Parameters.Length > 1) Then
				flag = False
			ElseIf (Not Me.Parameters(0).IsByRef) Then
				Dim type As TypeSymbol = Me.Parameters(0).Type
				If (type.TypeKind = Microsoft.CodeAnalysis.TypeKind.Array) Then
					Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
					flag = If(Not arrayTypeSymbol.IsSZArray, False, arrayTypeSymbol.ElementType.SpecialType = SpecialType.System_String)
				Else
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function IGenericMethodInstanceReferenceGetGenericArguments(ByVal context As EmitContext) As IEnumerable(Of ITypeReference) Implements IGenericMethodInstanceReference.GetGenericArguments
			Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
			Return Me.AdaptedMethodSymbol.TypeArguments.[Select](Of ITypeReference)(Function(arg As TypeSymbol) [module].Translate(arg, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics))
		End Function

		Private Function IGenericMethodInstanceReferenceGetGenericMethod(ByVal context As EmitContext) As IMethodReference Implements IGenericMethodInstanceReference.GetGenericMethod
			Dim specializedMethodReference As IMethodReference
			If (Me.AdaptedMethodSymbol.ContainingType.IsOrInGenericType()) Then
				specializedMethodReference = New Microsoft.CodeAnalysis.VisualBasic.Emit.SpecializedMethodReference(Me.AdaptedMethodSymbol.ConstructedFrom)
			Else
				specializedMethodReference = DirectCast(context.[Module], PEModuleBuilder).Translate(Me.AdaptedMethodSymbol.OriginalDefinition, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics, True)
			End If
			Return specializedMethodReference
		End Function

		Private Function IMethodDefinitionGetBody(ByVal context As EmitContext) As IMethodBody Implements IMethodDefinition.GetBody
			Return DirectCast(context.[Module], PEModuleBuilder).GetMethodBody(Me.AdaptedMethodSymbol)
		End Function

		Private Function IMethodDefinitionGetImplementationOptions(ByVal context As EmitContext) As MethodImplAttributes Implements IMethodDefinition.GetImplementationAttributes
			Return Me.AdaptedMethodSymbol.ImplementationAttributes Or If(DirectCast(context.[Module], PEModuleBuilder).JITOptimizationIsDisabled(Me.AdaptedMethodSymbol), MethodImplAttributes.NoInlining Or MethodImplAttributes.NoOptimization, MethodImplAttributes.IL)
		End Function

		Private Function IMethodDefinitionGetReturnValueAttributes(ByVal context As EmitContext) As IEnumerable(Of ICustomAttribute) Implements IMethodDefinition.GetReturnValueAttributes
			Dim synthesizedAttributeDatas As ArrayBuilder(Of SynthesizedAttributeData) = Nothing
			Dim returnTypeAttributes As ImmutableArray(Of VisualBasicAttributeData) = Me.AdaptedMethodSymbol.GetReturnTypeAttributes()
			Me.AdaptedMethodSymbol.AddSynthesizedReturnTypeAttributes(synthesizedAttributeDatas)
			Return Me.AdaptedMethodSymbol.GetCustomAttributesToEmit(returnTypeAttributes, synthesizedAttributeDatas, True, False)
		End Function

		Private Function IMethodReferenceGetResolvedMethod(ByVal context As EmitContext) As IMethodDefinition Implements IMethodReference.GetResolvedMethod
			Return Me.ResolvedMethodImpl(DirectCast(context.[Module], PEModuleBuilder))
		End Function

		Private Function ExplicitIMethodSymbol_Construct(ByVal ParamArray typeArguments As ITypeSymbol()) As IMethodSymbol Implements IMethodSymbol.Construct
			Return Me.Construct(Symbol.ConstructTypeArguments(typeArguments))
		End Function

		Private Function ExplicitIMethodSymbol_Construct(ByVal typeArguments As ImmutableArray(Of ITypeSymbol), ByVal typeArgumentNullableAnnotations As ImmutableArray(Of NullableAnnotation)) As IMethodSymbol Implements IMethodSymbol.Construct
			Return Me.Construct(Symbol.ConstructTypeArguments(typeArguments, typeArgumentNullableAnnotations))
		End Function

		Private Function IMethodSymbol_GetReturnTypeAttributes() As ImmutableArray(Of AttributeData) Implements IMethodSymbol.GetReturnTypeAttributes
			Return Me.GetReturnTypeAttributes().Cast(Of AttributeData)()
		End Function

		Private Function IMethodSymbol_GetTypeInferredDuringReduction(ByVal reducedFromTypeParameter As ITypeParameterSymbol) As ITypeSymbol Implements IMethodSymbol.GetTypeInferredDuringReduction
			Return Me.GetTypeInferredDuringReduction(reducedFromTypeParameter.EnsureVbSymbolOrNothing(Of TypeParameterSymbol)("reducedFromTypeParameter"))
		End Function

		Private Function IMethodSymbol_ReduceExtensionMethod(ByVal receiverType As ITypeSymbol) As IMethodSymbol Implements IMethodSymbol.ReduceExtensionMethod
			If (receiverType Is Nothing) Then
				Throw New ArgumentNullException("receiverType")
			End If
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = receiverType.EnsureVbSymbolOrNothing(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)("receiverType")
			Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
			Return Me.ReduceExtensionMethod(typeSymbol, discarded)
		End Function

		Private Function IMethodSymbolInternal_CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer Implements IMethodSymbolInternal.CalculateLocalSyntaxOffset
			Return Me.CalculateLocalSyntaxOffset(localPosition, localTree)
		End Function

		Private Function IMethodSymbolInternal_Construct(ByVal ParamArray typeArguments As ITypeSymbolInternal()) As IMethodSymbolInternal Implements IMethodSymbolInternal.Construct
			Return Me.Construct(DirectCast(typeArguments, TypeSymbol()))
		End Function

		Friend NotOverridable Overrides Function IReferenceAsDefinition(ByVal context As EmitContext) As IDefinition
			Return Me.ResolvedMethodImpl(DirectCast(context.[Module], PEModuleBuilder))
		End Function

		Friend NotOverridable Overrides Sub IReferenceDispatch(ByVal visitor As MetadataVisitor)
			If (Not Me.AdaptedMethodSymbol.IsDefinition) Then
				If (Me.AdaptedMethodSymbol.IsGenericMethod AndAlso CObj(Me.AdaptedMethodSymbol) <> CObj(Me.AdaptedMethodSymbol.ConstructedFrom)) Then
					visitor.Visit(DirectCast(Me, IGenericMethodInstanceReference))
					Return
				End If
				visitor.Visit(DirectCast(Me, IMethodReference))
				Return
			End If
			Dim [module] As PEModuleBuilder = DirectCast(visitor.Context.[Module], PEModuleBuilder)
			If (Me.AdaptedMethodSymbol.ContainingModule = [module].SourceModule) Then
				visitor.Visit(DirectCast(Me, IMethodDefinition))
				Return
			End If
			visitor.Visit(DirectCast(Me, IMethodReference))
		End Sub

		Private Function IsAccessibleEntryPoint() As Boolean
			Dim flag As Boolean
			If (Me.DeclaredAccessibility <> Accessibility.[Private]) Then
				Dim containingType As NamedTypeSymbol = Me.ContainingType
				While containingType IsNot Nothing
					If (containingType.DeclaredAccessibility <> Accessibility.[Private]) Then
						containingType = containingType.ContainingType
					Else
						flag = False
						Return flag
					End If
				End While
				flag = True
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function ISignatureGetParameters(ByVal context As EmitContext) As ImmutableArray(Of IParameterTypeInformation)
			Dim parameterTypeInformations As ImmutableArray(Of IParameterTypeInformation)
			Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
			parameterTypeInformations = If(Not Me.AdaptedMethodSymbol.IsDefinition OrElse Not (Me.AdaptedMethodSymbol.ContainingModule = [module].SourceModule), [module].Translate(Me.AdaptedMethodSymbol.Parameters), Me.EnumerateDefinitionParameters())
			Return parameterTypeInformations
		End Function

		Private Function ISignatureGetType(ByVal context As EmitContext) As ITypeReference
			Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
			Dim returnType As TypeSymbol = Me.AdaptedMethodSymbol.ReturnType
			Return [module].Translate(returnType, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics)
		End Function

		Friend Overridable Function IsMetadataNewSlot(Optional ByVal ignoreInterfaceImplementationChanges As Boolean = False) As Boolean
			Dim flag As Boolean
			flag = If(Not Me.IsOverrides, Me.IsMetadataVirtual(), OverrideHidingHelper.RequiresExplicitOverride(Me))
			Return flag
		End Function

		Friend Overridable Function IsParameterlessConstructor() As Boolean
			If (Me.ParameterCount <> 0) Then
				Return False
			End If
			Return Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.Constructor
		End Function

		Private Function ITypeMemberReferenceGetContainingType(ByVal context As EmitContext) As ITypeReference Implements ITypeMemberReference.GetContainingType
			Dim privateImplClass As ITypeReference
			Dim [module] As PEModuleBuilder = DirectCast(context.[Module], PEModuleBuilder)
			If (Not Me.AdaptedMethodSymbol.IsDefinition) Then
				privateImplClass = [module].Translate(Me.AdaptedMethodSymbol.ContainingType, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics, False, False)
			ElseIf (Not TypeOf Me.AdaptedMethodSymbol Is SynthesizedGlobalMethodBase) Then
				privateImplClass = [module].Translate(Me.AdaptedMethodSymbol.ContainingType, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics, False, True)
			Else
				privateImplClass = [module].GetPrivateImplClass(DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics)
			End If
			Return privateImplClass
		End Function

		Friend Function ReduceExtensionMethod(ByVal instanceType As TypeSymbol, ByVal proximity As Integer, ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As MethodSymbol
			Return ReducedExtensionMethodSymbol.Create(instanceType, Me, proximity, useSiteInfo)
		End Function

		Public Function ReduceExtensionMethod(ByVal instanceType As TypeSymbol, ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As MethodSymbol
			Return Me.ReduceExtensionMethod(instanceType, 0, useSiteInfo)
		End Function

		Private Function ResolvedMethodImpl(ByVal moduleBeingBuilt As PEModuleBuilder) As IMethodDefinition
			Dim methodDefinition As IMethodDefinition
			If (Not Me.AdaptedMethodSymbol.IsDefinition OrElse Not (Me.AdaptedMethodSymbol.ContainingModule = moduleBeingBuilt.SourceModule)) Then
				methodDefinition = Nothing
			Else
				methodDefinition = Me
			End If
			Return methodDefinition
		End Function

		Friend Overridable Function TryGetMeParameter(<Out> ByRef meParameter As ParameterSymbol) As Boolean
			meParameter = Nothing
			Return False
		End Function

		Protected Function ValidateGenericConstraintsOnExtensionMethodDefinition() As Boolean
			Dim flag As Boolean
			Dim enumerator As HashSet(Of TypeParameterSymbol).Enumerator = New HashSet(Of TypeParameterSymbol).Enumerator()
			If (Me.Arity <> 0) Then
				Dim item As ParameterSymbol = Me.Parameters(0)
				Dim typeParameterSymbols As HashSet(Of TypeParameterSymbol) = New HashSet(Of TypeParameterSymbol)()
				item.Type.CollectReferencedTypeParameters(typeParameterSymbols)
				If (typeParameterSymbols.Count > 0) Then
					Try
						enumerator = typeParameterSymbols.GetEnumerator()
						While enumerator.MoveNext()
							Dim enumerator1 As ImmutableArray(Of TypeSymbol).Enumerator = enumerator.Current.ConstraintTypesNoUseSiteDiagnostics.GetEnumerator()
							While enumerator1.MoveNext()
								If (Not enumerator1.Current.ReferencesTypeParameterNotInTheSet(typeParameterSymbols)) Then
									Continue While
								End If
								flag = False
								Return flag
							End While
						End While
					Finally
						DirectCast(enumerator, IDisposable).Dispose()
					End Try
				End If
				flag = True
			Else
				flag = True
			End If
			Return flag
		End Function
	End Class
End Namespace