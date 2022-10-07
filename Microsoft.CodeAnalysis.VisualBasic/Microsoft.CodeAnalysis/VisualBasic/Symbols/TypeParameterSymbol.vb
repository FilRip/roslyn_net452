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
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Reflection.Metadata
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class TypeParameterSymbol
		Inherits TypeSymbol
		Implements IGenericParameterReference, IGenericMethodParameterReference, IGenericTypeParameterReference, IGenericParameter, IGenericMethodParameter, IGenericTypeParameter, ITypeParameterSymbol, ITypeParameterSymbolInternal
		Private ReadOnly Shared s_substituteFunc As Func(Of TypeSymbol, TypeSubstitution, TypeSymbol)

		Friend ReadOnly Property AdaptedTypeParameterSymbol As TypeParameterSymbol
			Get
				Return Me
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property BaseTypeNoUseSiteDiagnostics As NamedTypeSymbol
			Get
				Return Nothing
			End Get
		End Property

		Friend MustOverride ReadOnly Property ConstraintTypesNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol)

		Public NotOverridable Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.NotApplicable
			End Get
		End Property

		Public ReadOnly Property DeclaringMethod As MethodSymbol
			Get
				Return TryCast(Me.ContainingSymbol, MethodSymbol)
			End Get
		End Property

		Public ReadOnly Property DeclaringType As NamedTypeSymbol
			Get
				Return TryCast(Me.ContainingSymbol, NamedTypeSymbol)
			End Get
		End Property

		Friend Overrides ReadOnly Property EmbeddedSymbolKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind
			Get
				Return Me.ContainingSymbol.EmbeddedSymbolKind
			End Get
		End Property

		Public MustOverride ReadOnly Property HasConstructorConstraint As Boolean Implements ITypeParameterSymbol.HasConstructorConstraint

		ReadOnly Property HasNotNullConstraint As Boolean Implements ITypeParameterSymbol.HasNotNullConstraint
			Get
				Return False
			End Get
		End Property

		Public MustOverride ReadOnly Property HasReferenceTypeConstraint As Boolean Implements ITypeParameterSymbol.HasReferenceTypeConstraint

		ReadOnly Property HasUnmanagedTypeConstraint As Boolean Implements ITypeParameterSymbol.HasUnmanagedTypeConstraint
			Get
				Return False
			End Get
		End Property

		Public MustOverride ReadOnly Property HasValueTypeConstraint As Boolean Implements ITypeParameterSymbol.HasValueTypeConstraint

		ReadOnly Property IGenericMethodParameterDefiningMethod As IMethodDefinition Implements IGenericMethodParameter.DefiningMethod
			Get
				Return DirectCast(Me.AdaptedTypeParameterSymbol.ContainingSymbol, MethodSymbol).GetCciAdapter()
			End Get
		End Property

		ReadOnly Property IGenericMethodParameterReferenceDefiningMethod As IMethodReference Implements IGenericMethodParameterReference.DefiningMethod
			Get
				Return DirectCast(Me.AdaptedTypeParameterSymbol.ContainingSymbol, MethodSymbol).GetCciAdapter()
			End Get
		End Property

		ReadOnly Property IGenericParameterAsGenericMethodParameter As IGenericMethodParameter Implements IGenericParameter.AsGenericMethodParameter
			Get
				Dim genericMethodParameter As IGenericMethodParameter
				If (Me.AdaptedTypeParameterSymbol.ContainingSymbol.Kind <> SymbolKind.Method) Then
					genericMethodParameter = Nothing
				Else
					genericMethodParameter = Me
				End If
				Return genericMethodParameter
			End Get
		End Property

		ReadOnly Property IGenericParameterAsGenericTypeParameter As IGenericTypeParameter Implements IGenericParameter.AsGenericTypeParameter
			Get
				Dim genericTypeParameter As IGenericTypeParameter
				If (Me.AdaptedTypeParameterSymbol.ContainingSymbol.Kind <> SymbolKind.NamedType) Then
					genericTypeParameter = Nothing
				Else
					genericTypeParameter = Me
				End If
				Return genericTypeParameter
			End Get
		End Property

		ReadOnly Property IGenericParameterMustBeReferenceType As Boolean Implements IGenericParameter.MustBeReferenceType
			Get
				Return Me.AdaptedTypeParameterSymbol.HasReferenceTypeConstraint
			End Get
		End Property

		ReadOnly Property IGenericParameterMustBeValueType As Boolean Implements IGenericParameter.MustBeValueType
			Get
				Return Me.AdaptedTypeParameterSymbol.HasValueTypeConstraint
			End Get
		End Property

		ReadOnly Property IGenericParameterMustHaveDefaultConstructor As Boolean Implements IGenericParameter.MustHaveDefaultConstructor
			Get
				If (Me.AdaptedTypeParameterSymbol.HasConstructorConstraint) Then
					Return True
				End If
				Return Me.AdaptedTypeParameterSymbol.HasValueTypeConstraint
			End Get
		End Property

		ReadOnly Property IGenericParameterVariance As TypeParameterVariance Implements IGenericParameter.Variance
			Get
				Dim typeParameterVariance As Microsoft.Cci.TypeParameterVariance
				Select Case Me.AdaptedTypeParameterSymbol.Variance
					Case VarianceKind.None
						typeParameterVariance = Microsoft.Cci.TypeParameterVariance.NonVariant
						Exit Select
					Case VarianceKind.Out
						typeParameterVariance = Microsoft.Cci.TypeParameterVariance.Covariant
						Exit Select
					Case VarianceKind.[In]
						typeParameterVariance = Microsoft.Cci.TypeParameterVariance.Contravariant
						Exit Select
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(Me.AdaptedTypeParameterSymbol.Variance)
				End Select
				Return typeParameterVariance
			End Get
		End Property

		ReadOnly Property IGenericTypeParameterDefiningType As ITypeDefinition Implements IGenericTypeParameter.DefiningType
			Get
				Return DirectCast(Me.AdaptedTypeParameterSymbol.ContainingSymbol, NamedTypeSymbol).GetCciAdapter()
			End Get
		End Property

		ReadOnly Property IGenericTypeParameterReferenceDefiningType As ITypeReference Implements IGenericTypeParameterReference.DefiningType
			Get
				Return DirectCast(Me.AdaptedTypeParameterSymbol.ContainingSymbol, NamedTypeSymbol).GetCciAdapter()
			End Get
		End Property

		ReadOnly Property INamedEntityName As String
			Get
				Return Me.AdaptedTypeParameterSymbol.MetadataName
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property InterfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol)
			Get
				Return ImmutableArray(Of NamedTypeSymbol).Empty
			End Get
		End Property

		ReadOnly Property IParameterListEntryIndex As UShort
			Get
				Return CUShort(Me.AdaptedTypeParameterSymbol.Ordinal)
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsReferenceType As Boolean
			Get
				Return If(Not Me.HasReferenceTypeConstraint, Me.IsReferenceTypeIgnoringIsClass(), True)
			End Get
		End Property

		Public Overrides ReadOnly Property IsValueType As Boolean
			Get
				Dim flag As Boolean
				If (Not Me.HasValueTypeConstraint) Then
					Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = Me.ConstraintTypesNoUseSiteDiagnostics.GetEnumerator()
					While enumerator.MoveNext()
						If (Not enumerator.Current.IsValueType) Then
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
			End Get
		End Property

		ReadOnly Property ITypeParameterSymbol_ConstraintNullableAnnotations As ImmutableArray(Of Microsoft.CodeAnalysis.NullableAnnotation) Implements ITypeParameterSymbol.ConstraintNullableAnnotations
			Get
				Dim func As Func(Of TypeSymbol, Microsoft.CodeAnalysis.NullableAnnotation)
				Dim constraintTypesNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol) = Me.ConstraintTypesNoUseSiteDiagnostics
				If (TypeParameterSymbol._Closure$__.$I133-0 Is Nothing) Then
					func = Function(t As TypeSymbol) Microsoft.CodeAnalysis.NullableAnnotation.None
					TypeParameterSymbol._Closure$__.$I133-0 = func
				Else
					func = TypeParameterSymbol._Closure$__.$I133-0
				End If
				Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of TypeSymbol, Microsoft.CodeAnalysis.NullableAnnotation)(constraintTypesNoUseSiteDiagnostics, func)
			End Get
		End Property

		ReadOnly Property ITypeParameterSymbol_ConstraintTypes As ImmutableArray(Of ITypeSymbol) Implements ITypeParameterSymbol.ConstraintTypes
			Get
				Return StaticCast(Of ITypeSymbol).From(Of TypeSymbol)(Me.ConstraintTypesNoUseSiteDiagnostics)
			End Get
		End Property

		ReadOnly Property ITypeParameterSymbol_DeclaringMethod As IMethodSymbol Implements ITypeParameterSymbol.DeclaringMethod
			Get
				Return Me.DeclaringMethod
			End Get
		End Property

		ReadOnly Property ITypeParameterSymbol_DeclaringType As INamedTypeSymbol Implements ITypeParameterSymbol.DeclaringType
			Get
				Return Me.DeclaringType
			End Get
		End Property

		ReadOnly Property ITypeParameterSymbol_Ordinal As Integer Implements ITypeParameterSymbol.Ordinal
			Get
				Return Me.Ordinal
			End Get
		End Property

		ReadOnly Property ITypeParameterSymbol_OriginalDefinition As ITypeParameterSymbol Implements ITypeParameterSymbol.OriginalDefinition
			Get
				Return Me.OriginalDefinition
			End Get
		End Property

		ReadOnly Property ITypeParameterSymbol_ReducedFrom As ITypeParameterSymbol Implements ITypeParameterSymbol.ReducedFrom
			Get
				Return Me.ReducedFrom
			End Get
		End Property

		ReadOnly Property ITypeParameterSymbol_ReferenceTypeConstraintNullableAnnotation As Microsoft.CodeAnalysis.NullableAnnotation Implements ITypeParameterSymbol.ReferenceTypeConstraintNullableAnnotation
			Get
				Return Microsoft.CodeAnalysis.NullableAnnotation.None
			End Get
		End Property

		ReadOnly Property ITypeReferenceAsGenericMethodParameterReference As IGenericMethodParameterReference
			Get
				Dim genericMethodParameterReference As IGenericMethodParameterReference
				If (Me.AdaptedTypeParameterSymbol.ContainingSymbol.Kind <> SymbolKind.Method) Then
					genericMethodParameterReference = Nothing
				Else
					genericMethodParameterReference = Me
				End If
				Return genericMethodParameterReference
			End Get
		End Property

		ReadOnly Property ITypeReferenceAsGenericTypeInstanceReference As IGenericTypeInstanceReference
			Get
				Return Nothing
			End Get
		End Property

		ReadOnly Property ITypeReferenceAsGenericTypeParameterReference As IGenericTypeParameterReference
			Get
				Dim genericTypeParameterReference As IGenericTypeParameterReference
				If (Me.AdaptedTypeParameterSymbol.ContainingSymbol.Kind <> SymbolKind.NamedType) Then
					genericTypeParameterReference = Nothing
				Else
					genericTypeParameterReference = Me
				End If
				Return genericTypeParameterReference
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

		Public NotOverridable Overrides ReadOnly Property Kind As SymbolKind
			Get
				Return SymbolKind.TypeParameter
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Public MustOverride ReadOnly Property Ordinal As Integer

		Public Shadows Overridable ReadOnly Property OriginalDefinition As TypeParameterSymbol
			Get
				Return Me
			End Get
		End Property

		Protected NotOverridable Overrides ReadOnly Property OriginalTypeSymbolDefinition As TypeSymbol
			Get
				Return Me.OriginalDefinition
			End Get
		End Property

		Public Overridable ReadOnly Property ReducedFrom As TypeParameterSymbol
			Get
				Return Nothing
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind
			Get
				Return Microsoft.CodeAnalysis.TypeKind.TypeParameter
			End Get
		End Property

		Public MustOverride ReadOnly Property TypeParameterKind As Microsoft.CodeAnalysis.TypeParameterKind Implements ITypeParameterSymbol.TypeParameterKind

		Public MustOverride ReadOnly Property Variance As VarianceKind Implements ITypeParameterSymbol.Variance

		Shared Sub New()
			TypeParameterSymbol.s_substituteFunc = Function(type As TypeSymbol, substitution As TypeSubstitution) type.InternalSubstituteTypeParameters(substitution).Type
		End Sub

		Friend Sub New()
			MyBase.New()
		End Sub

		Friend Overrides Function Accept(Of TArgument, TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TArgument, TResult), ByVal arg As TArgument) As TResult
			Return visitor.VisitTypeParameter(Me, arg)
		End Function

		Public Overrides Sub Accept(ByVal visitor As SymbolVisitor)
			visitor.VisitTypeParameter(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As SymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitTypeParameter(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As VisualBasicSymbolVisitor)
			visitor.VisitTypeParameter(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitTypeParameter(Me)
		End Function

		Private Shared Function ConstraintImpliesReferenceType(ByVal constraint As TypeSymbol) As Boolean
			Dim flag As Boolean
			If (constraint.TypeKind = Microsoft.CodeAnalysis.TypeKind.TypeParameter) Then
				flag = DirectCast(constraint, TypeParameterSymbol).IsReferenceTypeIgnoringIsClass()
			ElseIf (Not constraint.IsReferenceType) Then
				flag = False
			ElseIf (Not constraint.IsInterfaceType()) Then
				Dim specialType As Microsoft.CodeAnalysis.SpecialType = constraint.SpecialType
				flag = If(CSByte(specialType) - CSByte(Microsoft.CodeAnalysis.SpecialType.System_Object) <= CSByte(Microsoft.CodeAnalysis.SpecialType.System_Object) OrElse specialType = Microsoft.CodeAnalysis.SpecialType.System_ValueType, False, True)
			Else
				flag = False
			End If
			Return flag
		End Function

		Friend Function ConstraintTypesWithDefinitionUseSiteDiagnostics(<InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol)) As ImmutableArray(Of TypeSymbol)
			Dim constraintTypesNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol) = Me.ConstraintTypesNoUseSiteDiagnostics
			Me.AddConstraintsUseSiteInfo(useSiteInfo)
			Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = constraintTypesNoUseSiteDiagnostics.GetEnumerator()
			While enumerator.MoveNext()
				enumerator.Current.OriginalDefinition.AddUseSiteInfo(useSiteInfo)
			End While
			Return constraintTypesNoUseSiteDiagnostics
		End Function

		Friend MustOverride Sub EnsureAllConstraintsAreResolved()

		Friend Shared Sub EnsureAllConstraintsAreResolved(ByVal typeParameters As ImmutableArray(Of TypeParameterSymbol))
			Dim enumerator As ImmutableArray(Of TypeParameterSymbol).Enumerator = typeParameters.GetEnumerator()
			While enumerator.MoveNext()
				enumerator.Current.ResolveConstraints(ConsList(Of TypeParameterSymbol).Empty)
			End While
		End Sub

		Friend Shadows Function GetCciAdapter() As TypeParameterSymbol
			Return Me
		End Function

		Friend Sub GetConstraints(ByVal constraintsBuilder As ArrayBuilder(Of TypeParameterConstraint))
			constraintsBuilder.AddRange(Me.GetConstraints())
		End Sub

		Friend Overridable Function GetConstraints() As ImmutableArray(Of TypeParameterConstraint)
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overridable Function GetConstraintsUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Return New UseSiteInfo(Of AssemblySymbol)()
		End Function

		Friend Shared Function GetConstraintTypesOnly(ByVal constraints As ImmutableArray(Of TypeParameterConstraint)) As ImmutableArray(Of TypeSymbol)
			Dim immutableAndFree As ImmutableArray(Of TypeSymbol)
			If (Not constraints.IsEmpty) Then
				Dim instance As ArrayBuilder(Of TypeSymbol) = ArrayBuilder(Of TypeSymbol).GetInstance()
				Dim enumerator As ImmutableArray(Of TypeParameterConstraint).Enumerator = constraints.GetEnumerator()
				While enumerator.MoveNext()
					Dim typeConstraint As TypeSymbol = enumerator.Current.TypeConstraint
					If (typeConstraint Is Nothing) Then
						Continue While
					End If
					instance.Add(typeConstraint)
				End While
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = ImmutableArray(Of TypeSymbol).Empty
			End If
			Return immutableAndFree
		End Function

		Public NotOverridable Overrides Function GetMembers() As ImmutableArray(Of Symbol)
			Return ImmutableArray(Of Symbol).Empty
		End Function

		Public NotOverridable Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
			Return ImmutableArray(Of Symbol).Empty
		End Function

		Public NotOverridable Overrides Function GetTypeMembers() As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Public NotOverridable Overrides Function GetTypeMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Public NotOverridable Overrides Function GetTypeMembers(ByVal name As String, ByVal arity As Integer) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Friend Overrides Function GetUnificationUseSiteDiagnosticRecursive(ByVal owner As Symbol, ByRef checkedTypes As HashSet(Of TypeSymbol)) As DiagnosticInfo
			Return Nothing
		End Function

		Private Function IGenericParameterGetConstraints(ByVal context As EmitContext) As IEnumerable(Of TypeReferenceWithAttributes) Implements IGenericParameter.GetConstraints
			Return New TypeParameterSymbol.VB$StateMachine_39_IGenericParameterGetConstraints(-2) With
			{
				.$VB$Me = Me,
				.$P_context = context
			}
		End Function

		Friend Overrides Function InternalSubstituteTypeParameters(ByVal substitution As TypeSubstitution) As TypeWithModifiers
			Dim typeWithModifier As TypeWithModifiers
			typeWithModifier = If(substitution Is Nothing, New TypeWithModifiers(Me), substitution.GetSubstitutionFor(Me))
			Return typeWithModifier
		End Function

		Friend Shared Function InternalSubstituteTypeParametersDistinct(ByVal substitution As TypeSubstitution, ByVal types As ImmutableArray(Of TypeSymbol)) As ImmutableArray(Of TypeSymbol)
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.Distinct(Of TypeSymbol)(Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of TypeSymbol, TypeSubstitution, TypeSymbol)(types, TypeParameterSymbol.s_substituteFunc, substitution), Nothing)
		End Function

		Friend NotOverridable Overrides Function IReferenceAsDefinition(ByVal context As EmitContext) As IDefinition
			Return Nothing
		End Function

		Friend NotOverridable Overrides Sub IReferenceDispatch(ByVal visitor As MetadataVisitor)
			Dim kind As SymbolKind = Me.AdaptedTypeParameterSymbol.ContainingSymbol.Kind
			If (DirectCast(visitor.Context.[Module], PEModuleBuilder).SourceModule = Me.AdaptedTypeParameterSymbol.ContainingModule) Then
				If (kind = SymbolKind.NamedType) Then
					visitor.Visit(DirectCast(Me, IGenericTypeParameter))
					Return
				End If
				If (kind <> SymbolKind.Method) Then
					Throw ExceptionUtilities.UnexpectedValue(kind)
				End If
				visitor.Visit(DirectCast(Me, IGenericMethodParameter))
				Return
			End If
			If (kind = SymbolKind.NamedType) Then
				visitor.Visit(DirectCast(Me, IGenericTypeParameterReference))
				Return
			End If
			If (kind <> SymbolKind.Method) Then
				Throw ExceptionUtilities.UnexpectedValue(kind)
			End If
			visitor.Visit(DirectCast(Me, IGenericMethodParameterReference))
		End Sub

		Private Function IsReferenceTypeIgnoringIsClass() As Boolean
			Dim flag As Boolean
			Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = Me.ConstraintTypesNoUseSiteDiagnostics.GetEnumerator()
			While True
				If (Not enumerator.MoveNext()) Then
					flag = False
					Exit While
				ElseIf (TypeParameterSymbol.ConstraintImpliesReferenceType(enumerator.Current)) Then
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

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

		Friend Overridable Sub ResolveConstraints(ByVal inProgress As ConsList(Of TypeParameterSymbol))
			Throw ExceptionUtilities.Unreachable
		End Sub
	End Class
End Namespace