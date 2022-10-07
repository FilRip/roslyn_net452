Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class PropertySymbol
		Inherits Symbol
		Implements IPropertyDefinition, IPropertySymbol
		Friend ReadOnly Property AdaptedPropertySymbol As PropertySymbol
			Get
				Return Me
			End Get
		End Property

		Friend MustOverride ReadOnly Property AssociatedField As FieldSymbol

		Friend MustOverride ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention

		Friend Overrides ReadOnly Property EmbeddedSymbolKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind
			Get
				Return Me.ContainingSymbol.EmbeddedSymbolKind
			End Get
		End Property

		Public MustOverride ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of PropertySymbol)

		Public MustOverride ReadOnly Property GetMethod As MethodSymbol

		Friend Overridable ReadOnly Property HasRuntimeSpecialName As Boolean
			Get
				Return False
			End Get
		End Property

		Friend ReadOnly Property HasSet As Boolean
			Get
				Return CObj(Me.GetMostDerivedSetMethod()) <> CObj(Nothing)
			End Get
		End Property

		Friend MustOverride ReadOnly Property HasSpecialName As Boolean

		Public NotOverridable Overrides ReadOnly Property HasUnsupportedMetadata As Boolean
			Get
				Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = Me.GetUseSiteInfo().DiagnosticInfo
				If (diagnosticInfo Is Nothing) Then
					Return False
				End If
				Return diagnosticInfo.Code = 30643
			End Get
		End Property

		Protected Overrides ReadOnly Property HighestPriorityUseSiteError As Integer
			Get
				Return 30643
			End Get
		End Property

		ReadOnly Property INamedEntityName As String
			Get
				Return Me.AdaptedPropertySymbol.MetadataName
			End Get
		End Property

		ReadOnly Property IPropertyDefinitionDefaultValue As MetadataConstant Implements IPropertyDefinition.DefaultValue
			Get
				Return Nothing
			End Get
		End Property

		ReadOnly Property IPropertyDefinitionGetter As IMethodReference Implements IPropertyDefinition.Getter
			Get
				Dim getMethod As MethodSymbol = Me.AdaptedPropertySymbol.GetMethod
				If (getMethod IsNot Nothing) Then
					Return getMethod.GetCciAdapter()
				End If
				Return Nothing
			End Get
		End Property

		ReadOnly Property IPropertyDefinitionHasDefaultValue As Boolean Implements IPropertyDefinition.HasDefaultValue
			Get
				Return False
			End Get
		End Property

		ReadOnly Property IPropertyDefinitionIsRuntimeSpecial As Boolean Implements IPropertyDefinition.IsRuntimeSpecial
			Get
				Return Me.AdaptedPropertySymbol.HasRuntimeSpecialName
			End Get
		End Property

		ReadOnly Property IPropertyDefinitionIsSpecialName As Boolean Implements IPropertyDefinition.IsSpecialName
			Get
				Return Me.AdaptedPropertySymbol.HasSpecialName
			End Get
		End Property

		ReadOnly Property IPropertyDefinitionParameters As ImmutableArray(Of IParameterDefinition) Implements IPropertyDefinition.Parameters
			Get
				Return StaticCast(Of IParameterDefinition).From(Of ParameterSymbol)(Me.AdaptedPropertySymbol.Parameters)
			End Get
		End Property

		ReadOnly Property IPropertyDefinitionSetter As IMethodReference Implements IPropertyDefinition.Setter
			Get
				Dim setMethod As MethodSymbol = Me.AdaptedPropertySymbol.SetMethod
				If (setMethod IsNot Nothing) Then
					Return setMethod.GetCciAdapter()
				End If
				Return Nothing
			End Get
		End Property

		ReadOnly Property IPropertySymbol_ByRefReturnIsReadonly As Boolean Implements IPropertySymbol.ReturnsByRefReadonly
			Get
				Return False
			End Get
		End Property

		ReadOnly Property IPropertySymbol_ExplicitInterfaceImplementations As ImmutableArray(Of IPropertySymbol) Implements IPropertySymbol.ExplicitInterfaceImplementations
			Get
				Return Me.ExplicitInterfaceImplementations.Cast(Of IPropertySymbol)()
			End Get
		End Property

		ReadOnly Property IPropertySymbol_GetMethod As IMethodSymbol Implements IPropertySymbol.GetMethod
			Get
				Return Me.GetMethod
			End Get
		End Property

		ReadOnly Property IPropertySymbol_NullableAnnotation As Microsoft.CodeAnalysis.NullableAnnotation Implements IPropertySymbol.NullableAnnotation
			Get
				Return Microsoft.CodeAnalysis.NullableAnnotation.None
			End Get
		End Property

		ReadOnly Property IPropertySymbol_OriginalDefinition As IPropertySymbol Implements IPropertySymbol.OriginalDefinition
			Get
				Return Me.OriginalDefinition
			End Get
		End Property

		ReadOnly Property IPropertySymbol_OverriddenProperty As IPropertySymbol Implements IPropertySymbol.OverriddenProperty
			Get
				Return Me.OverriddenProperty
			End Get
		End Property

		ReadOnly Property IPropertySymbol_Parameters As ImmutableArray(Of IParameterSymbol) Implements IPropertySymbol.Parameters
			Get
				Return StaticCast(Of IParameterSymbol).From(Of ParameterSymbol)(Me.Parameters)
			End Get
		End Property

		ReadOnly Property IPropertySymbol_RefCustomModifiers As ImmutableArray(Of CustomModifier) Implements IPropertySymbol.RefCustomModifiers
			Get
				Return Me.RefCustomModifiers
			End Get
		End Property

		ReadOnly Property IPropertySymbol_RefKind As Microsoft.CodeAnalysis.RefKind Implements IPropertySymbol.RefKind
			Get
				If (Not Me.ReturnsByRef) Then
					Return Microsoft.CodeAnalysis.RefKind.None
				End If
				Return Microsoft.CodeAnalysis.RefKind.Ref
			End Get
		End Property

		ReadOnly Property IPropertySymbol_ReturnsByRef As Boolean Implements IPropertySymbol.ReturnsByRef
			Get
				Return Me.ReturnsByRef
			End Get
		End Property

		ReadOnly Property IPropertySymbol_SetMethod As IMethodSymbol Implements IPropertySymbol.SetMethod
			Get
				Return Me.SetMethod
			End Get
		End Property

		ReadOnly Property IPropertySymbol_Type As ITypeSymbol Implements IPropertySymbol.Type
			Get
				Return Me.Type
			End Get
		End Property

		ReadOnly Property IPropertySymbol_TypeCustomModifiers As ImmutableArray(Of CustomModifier) Implements IPropertySymbol.TypeCustomModifiers
			Get
				Return Me.TypeCustomModifiers
			End Get
		End Property

		Public MustOverride ReadOnly Property IsDefault As Boolean Implements IPropertySymbol.IsIndexer

		Friend Overridable ReadOnly Property IsDirectlyExcludedFromCodeCoverage As Boolean
			Get
				Return False
			End Get
		End Property

		ReadOnly Property ISignatureCallingConvention As Microsoft.Cci.CallingConvention
			Get
				Return Me.AdaptedPropertySymbol.CallingConvention
			End Get
		End Property

		ReadOnly Property ISignatureParameterCount As UShort
			Get
				Return CUShort(Me.AdaptedPropertySymbol.ParameterCount)
			End Get
		End Property

		ReadOnly Property ISignatureRefCustomModifiers As ImmutableArray(Of ICustomModifier)
			Get
				Return Me.AdaptedPropertySymbol.RefCustomModifiers.[As](Of ICustomModifier)()
			End Get
		End Property

		ReadOnly Property ISignatureReturnValueCustomModifiers As ImmutableArray(Of ICustomModifier)
			Get
				Return Me.AdaptedPropertySymbol.TypeCustomModifiers.[As](Of ICustomModifier)()
			End Get
		End Property

		ReadOnly Property ISignatureReturnValueIsByRef As Boolean
			Get
				Return Me.AdaptedPropertySymbol.ReturnsByRef
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMyGroupCollectionProperty As Boolean

		Public MustOverride ReadOnly Property IsOverloads As Boolean

		Friend ReadOnly Property IsReadable As Boolean
			Get
				Return CObj(Me.GetMostDerivedGetMethod()) <> CObj(Nothing)
			End Get
		End Property

		Public Overridable ReadOnly Property IsReadOnly As Boolean Implements IPropertySymbol.IsReadOnly
			Get
				Return Me.SetMethod Is Nothing
			End Get
		End Property

		Public Overridable ReadOnly Property IsTupleProperty As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overridable ReadOnly Property IsWithEvents As Boolean Implements IPropertySymbol.IsWithEvents
			Get
				Dim overriddenProperty As PropertySymbol = Me.OverriddenProperty
				Return If(overriddenProperty IsNot Nothing, overriddenProperty.IsWithEvents, False)
			End Get
		End Property

		Public Overridable ReadOnly Property IsWriteOnly As Boolean Implements IPropertySymbol.IsWriteOnly
			Get
				Return Me.GetMethod Is Nothing
			End Get
		End Property

		ReadOnly Property ITypeDefinitionMemberContainingTypeDefinition As ITypeDefinition
			Get
				Return Me.AdaptedPropertySymbol.ContainingType.GetCciAdapter()
			End Get
		End Property

		ReadOnly Property ITypeDefinitionMemberVisibility As TypeMemberVisibility
			Get
				Return PEModuleBuilder.MemberVisibility(Me.AdaptedPropertySymbol)
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Kind As SymbolKind
			Get
				Return SymbolKind.[Property]
			End Get
		End Property

		Friend Overridable ReadOnly Property MeParameter As ParameterSymbol
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Shadows Overridable ReadOnly Property OriginalDefinition As PropertySymbol
			Get
				Return Me
			End Get
		End Property

		Protected NotOverridable Overrides ReadOnly Property OriginalSymbolDefinition As Symbol
			Get
				Return Me.OriginalDefinition
			End Get
		End Property

		Friend Overridable ReadOnly Property OverriddenMembers As OverriddenMembersResult(Of PropertySymbol)
			Get
				Return OverrideHidingHelper(Of PropertySymbol).MakeOverriddenMembers(Me)
			End Get
		End Property

		Public ReadOnly Property OverriddenProperty As PropertySymbol
			Get
				Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol
				If (Not Me.IsOverrides) Then
					propertySymbol = Nothing
				Else
					propertySymbol = If(Not MyBase.IsDefinition, OverriddenMembersResult(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol).GetOverriddenMember(Me, Me.OriginalDefinition.OverriddenProperty), Me.OverriddenMembers.OverriddenMember)
				End If
				Return propertySymbol
			End Get
		End Property

		Public Overridable ReadOnly Property ParameterCount As Integer
			Get
				Return Me.Parameters.Length
			End Get
		End Property

		Public MustOverride ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)

		Friend Overridable ReadOnly Property ReceiverType As TypeSymbol
			Get
				Return Me.ContainingType
			End Get
		End Property

		Friend Overridable ReadOnly Property ReducedFrom As PropertySymbol
			Get
				Return Nothing
			End Get
		End Property

		Friend Overridable ReadOnly Property ReducedFromDefinition As PropertySymbol
			Get
				Return Nothing
			End Get
		End Property

		Public MustOverride ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)

		Public MustOverride ReadOnly Property ReturnsByRef As Boolean

		Public MustOverride ReadOnly Property SetMethod As MethodSymbol

		Public Overridable ReadOnly Property TupleUnderlyingProperty As PropertySymbol
			Get
				Return Nothing
			End Get
		End Property

		Public MustOverride ReadOnly Property Type As TypeSymbol

		Public MustOverride ReadOnly Property TypeCustomModifiers As ImmutableArray(Of CustomModifier)

		Friend Sub New()
			MyBase.New()
		End Sub

		Friend Overrides Function Accept(Of TArgument, TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TArgument, TResult), ByVal arg As TArgument) As TResult
			Return visitor.VisitProperty(Me, arg)
		End Function

		Public Overrides Sub Accept(ByVal visitor As SymbolVisitor)
			visitor.VisitProperty(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As SymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitProperty(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As VisualBasicSymbolVisitor)
			visitor.VisitProperty(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitProperty(Me)
		End Function

		Friend Function CalculateUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol)
			Dim code As Boolean
			Dim flag As Boolean
			Dim code1 As Boolean
			Dim flag1 As Boolean
			Dim useSiteInfo1 As UseSiteInfo(Of AssemblySymbol) = MyBase.MergeUseSiteInfo(New UseSiteInfo(Of AssemblySymbol)(MyBase.PrimaryDependency), MyBase.DeriveUseSiteInfoFromType(Me.Type))
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = useSiteInfo1.DiagnosticInfo
			If (diagnosticInfo IsNot Nothing) Then
				code = diagnosticInfo.Code = 30643
			Else
				code = False
			End If
			If (Not code) Then
				Dim useSiteInfo2 As UseSiteInfo(Of AssemblySymbol) = MyBase.DeriveUseSiteInfoFromCustomModifiers(Me.RefCustomModifiers, False)
				Dim diagnosticInfo1 As Microsoft.CodeAnalysis.DiagnosticInfo = useSiteInfo2.DiagnosticInfo
				If (diagnosticInfo1 IsNot Nothing) Then
					flag = diagnosticInfo1.Code = 30643
				Else
					flag = False
				End If
				If (Not flag) Then
					Dim useSiteInfo3 As UseSiteInfo(Of AssemblySymbol) = MyBase.DeriveUseSiteInfoFromCustomModifiers(Me.TypeCustomModifiers, False)
					Dim diagnosticInfo2 As Microsoft.CodeAnalysis.DiagnosticInfo = useSiteInfo3.DiagnosticInfo
					If (diagnosticInfo2 IsNot Nothing) Then
						code1 = diagnosticInfo2.Code = 30643
					Else
						code1 = False
					End If
					If (Not code1) Then
						Dim useSiteInfo4 As UseSiteInfo(Of AssemblySymbol) = MyBase.DeriveUseSiteInfoFromParameters(Me.Parameters)
						Dim diagnosticInfo3 As Microsoft.CodeAnalysis.DiagnosticInfo = useSiteInfo4.DiagnosticInfo
						If (diagnosticInfo3 IsNot Nothing) Then
							flag1 = diagnosticInfo3.Code = 30643
						Else
							flag1 = False
						End If
						If (Not flag1) Then
							Dim unificationUseSiteDiagnosticRecursive As Microsoft.CodeAnalysis.DiagnosticInfo = If(useSiteInfo1.DiagnosticInfo, (If(useSiteInfo2.DiagnosticInfo, (If(useSiteInfo3.DiagnosticInfo, useSiteInfo4.DiagnosticInfo)))))
							If (unificationUseSiteDiagnosticRecursive Is Nothing AndAlso Me.ContainingModule.HasUnifiedReferences) Then
								Dim typeSymbols As HashSet(Of TypeSymbol) = Nothing
								unificationUseSiteDiagnosticRecursive = If(Me.Type.GetUnificationUseSiteDiagnosticRecursive(Me, typeSymbols), (If(Symbol.GetUnificationUseSiteDiagnosticRecursive(Me.RefCustomModifiers, Me, typeSymbols), (If(Symbol.GetUnificationUseSiteDiagnosticRecursive(Me.TypeCustomModifiers, Me, typeSymbols), Symbol.GetUnificationUseSiteDiagnosticRecursive(Me.Parameters, Me, typeSymbols))))))
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

		<Conditional("DEBUG")>
		Protected Friend Sub CheckDefinitionInvariantAllowEmbedded()
		End Sub

		Friend Sub CloneParameters(ByVal method As MethodSymbol, ByVal parameters As ArrayBuilder(Of ParameterSymbol))
			Dim parameterSymbols As ImmutableArray(Of ParameterSymbol) = Me.Parameters
			Dim length As Integer = parameterSymbols.Length - 1
			For i As Integer = 0 To length
				parameters.Add(parameterSymbols(i).ChangeOwner(method))
			Next

		End Sub

		Friend Function GetAccessorOverride(ByVal getter As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim overriddenProperty As PropertySymbol = Me.OverriddenProperty
			If (overriddenProperty Is Nothing) Then
				methodSymbol = Nothing
			Else
				methodSymbol = If(getter, overriddenProperty.GetMethod, overriddenProperty.SetMethod)
			End If
			Return methodSymbol
		End Function

		Friend Shadows Function GetCciAdapter() As PropertySymbol
			Return Me
		End Function

		Public Function GetFieldAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Dim associatedField As FieldSymbol = Me.AssociatedField
			If (associatedField Is Nothing) Then
				Return ImmutableArray(Of VisualBasicAttributeData).Empty
			End If
			Return associatedField.GetAttributes()
		End Function

		Friend Function GetMostDerivedGetMethod() As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim overriddenProperty As PropertySymbol = Me
			While True
				Dim getMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = overriddenProperty.GetMethod
				If (getMethod Is Nothing) Then
					overriddenProperty = overriddenProperty.OverriddenProperty
					If (overriddenProperty Is Nothing) Then
						methodSymbol = Nothing
						Exit While
					End If
				Else
					methodSymbol = getMethod
					Exit While
				End If
			End While
			Return methodSymbol
		End Function

		Friend Function GetMostDerivedSetMethod() As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim overriddenProperty As PropertySymbol = Me
			While True
				Dim setMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = overriddenProperty.SetMethod
				If (setMethod Is Nothing) Then
					overriddenProperty = overriddenProperty.OverriddenProperty
					If (overriddenProperty Is Nothing) Then
						methodSymbol = Nothing
						Exit While
					End If
				Else
					methodSymbol = setMethod
					Exit While
				End If
			End While
			Return methodSymbol
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol)
			useSiteInfo = If(Not MyBase.IsDefinition, Me.OriginalDefinition.GetUseSiteInfo(), New UseSiteInfo(Of AssemblySymbol)(MyBase.PrimaryDependency))
			Return useSiteInfo
		End Function

		Private Function IPropertyDefinitionAccessors(ByVal context As EmitContext) As IEnumerable(Of IMethodReference) Implements IPropertyDefinition.GetAccessors
			Return New PropertySymbol.VB$StateMachine_0_IPropertyDefinitionAccessors(-2) With
			{
				.$VB$Me = Me,
				.$P_context = context
			}
		End Function

		Friend NotOverridable Overrides Function IReferenceAsDefinition(ByVal context As EmitContext) As IDefinition
			Return Me
		End Function

		Friend NotOverridable Overrides Sub IReferenceDispatch(ByVal visitor As MetadataVisitor)
			visitor.Visit(Me)
		End Sub

		Private Function ISignatureGetParameters(ByVal context As EmitContext) As ImmutableArray(Of IParameterTypeInformation)
			Return StaticCast(Of IParameterTypeInformation).From(Of ParameterSymbol)(Me.AdaptedPropertySymbol.Parameters)
		End Function

		Private Function ISignatureGetType(ByVal context As EmitContext) As ITypeReference
			Return DirectCast(context.[Module], PEModuleBuilder).Translate(Me.AdaptedPropertySymbol.Type, DirectCast(context.SyntaxNode, VisualBasicSyntaxNode), context.Diagnostics)
		End Function

		Friend Function IsWritable(ByVal receiverOpt As BoundExpression, ByVal containingBinder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal isKnownTargetOfObjectMemberInitializer As Boolean) As Boolean
			Dim flag As Boolean
			Dim kind As BoundKind
			Dim flag1 As Boolean
			Dim methodKind As Boolean
			Dim expressionPlaceholder As BoundExpression
			Dim mostDerivedSetMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me.GetMostDerivedSetMethod()
			If (mostDerivedSetMethod Is Nothing) Then
				Dim sourcePropertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol = TryCast(Me, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol)
				Dim isShared As Boolean = Me.IsShared
				Dim containingMember As Microsoft.CodeAnalysis.VisualBasic.Symbol = containingBinder.ContainingMember
				If (sourcePropertySymbol Is Nothing OrElse containingMember Is Nothing OrElse Not sourcePropertySymbol.IsAutoProperty OrElse Not TypeSymbol.Equals(sourcePropertySymbol.ContainingType, containingMember.ContainingType, TypeCompareKind.ConsiderEverything) OrElse isShared <> containingMember.IsShared OrElse Not isShared AndAlso (receiverOpt Is Nothing OrElse receiverOpt.Kind <> BoundKind.MeReference)) Then
					flag1 = False
				Else
					flag1 = If(containingMember.Kind <> SymbolKind.Method OrElse Not DirectCast(containingMember, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).IsAnyConstructor(), TypeOf containingBinder Is DeclarationInitializerBinder, True)
				End If
				flag = flag1
			ElseIf (Not mostDerivedSetMethod.IsInitOnly) Then
				flag = True
			ElseIf (receiverOpt Is Nothing) Then
				flag = False
			ElseIf (Not isKnownTargetOfObjectMemberInitializer) Then
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = containingBinder.ContainingMember
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
				If (methodSymbol IsNot Nothing) Then
					methodKind = methodSymbol.MethodKind <> Microsoft.CodeAnalysis.MethodKind.Constructor
				Else
					methodKind = True
				End If
				If (Not methodKind) Then
					If (receiverOpt.Kind = BoundKind.WithLValueExpressionPlaceholder OrElse receiverOpt.Kind = BoundKind.WithRValueExpressionPlaceholder) Then
						Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = containingBinder
						While binder IsNot Nothing AndAlso CObj(binder.ContainingMember) = CObj(symbol)
							Dim withBlockBinder As Microsoft.CodeAnalysis.VisualBasic.WithBlockBinder = TryCast(binder, Microsoft.CodeAnalysis.VisualBasic.WithBlockBinder)
							If (withBlockBinder Is Nothing) Then
								binder = binder.ContainingBinder
							Else
								Dim info As Microsoft.CodeAnalysis.VisualBasic.WithBlockBinder.WithBlockInfo = withBlockBinder.Info
								If (info IsNot Nothing) Then
									expressionPlaceholder = info.ExpressionPlaceholder
								Else
									expressionPlaceholder = Nothing
								End If
								If (expressionPlaceholder <> receiverOpt) Then
									Exit While
								End If
								receiverOpt = withBlockBinder.Info.OriginalExpression
								Exit While
							End If
						End While
					End If
					While True
						kind = receiverOpt.Kind
						If (kind <> BoundKind.Parenthesized) Then
							Exit While
						End If
						receiverOpt = DirectCast(receiverOpt, BoundParenthesized).Expression
					End While
					flag = If(kind = BoundKind.MeReference OrElse CByte(kind) - CByte(BoundKind.MyBaseReference) <= CByte(BoundKind.OmittedArgument), True, False)
				Else
					flag = False
				End If
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Function ITypeMemberReferenceGetContainingType(ByVal context As EmitContext) As ITypeReference
			Return Me.AdaptedPropertySymbol.ContainingType.GetCciAdapter()
		End Function
	End Class
End Namespace