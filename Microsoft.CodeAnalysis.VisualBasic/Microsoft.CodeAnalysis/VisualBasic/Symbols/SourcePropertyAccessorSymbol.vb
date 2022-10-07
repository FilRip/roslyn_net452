Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SourcePropertyAccessorSymbol
		Inherits SourceMethodSymbol
		Protected ReadOnly m_property As SourcePropertySymbol

		Private ReadOnly _name As String

		Private _lazyMetadataName As String

		Private _lazyExplicitImplementations As ImmutableArray(Of MethodSymbol)

		Private _lazyParameters As ImmutableArray(Of ParameterSymbol)

		Private _lazyReturnType As TypeSymbol

		Private ReadOnly Shared s_checkParameterModifierCallback As Binder.CheckParameterModifierDelegate

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return Me.m_property
			End Get
		End Property

		Protected Overrides ReadOnly Property BoundReturnTypeAttributesSource As SourcePropertySymbol
			Get
				If (Me.MethodKind <> Microsoft.CodeAnalysis.MethodKind.PropertyGet) Then
					Return Nothing
				End If
				Return Me.m_property
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Dim localAccessibility As Accessibility = Me.LocalAccessibility
				Return If(localAccessibility = Accessibility.NotApplicable, Me.m_property.DeclaredAccessibility, localAccessibility)
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				If (Not Me.m_property.IsCustomProperty) Then
					Return ImmutableArray(Of SyntaxReference).Empty
				End If
				Return MyBase.DeclaringSyntaxReferences
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
			Get
				If (Me._lazyExplicitImplementations.IsDefault) Then
					Dim accessorImplementations As ImmutableArray(Of MethodSymbol) = Me.m_property.GetAccessorImplementations(Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.PropertyGet)
					Dim methodSymbols As ImmutableArray(Of MethodSymbol) = New ImmutableArray(Of MethodSymbol)()
					ImmutableInterlocked.InterlockedCompareExchange(Of MethodSymbol)(Me._lazyExplicitImplementations, accessorImplementations, methodSymbols)
				End If
				Return Me._lazyExplicitImplementations
			End Get
		End Property

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				If (Me.m_property.IsAutoProperty) Then
					Return False
				End If
				Return MyBase.GenerateDebugInfoImpl
			End Get
		End Property

		Friend ReadOnly Property HasDebuggerHiddenAttribute As Boolean
			Get
				Dim decodedWellKnownAttributeData As MethodWellKnownAttributeData = MyBase.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return decodedWellKnownAttributeData.IsPropertyAccessorWithDebuggerHiddenAttribute
			End Get
		End Property

		Public Overrides ReadOnly Property IsExtensionMethod As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				If (Not Me.m_property.IsCustomProperty) Then
					Return True
				End If
				Return MyBase.IsImplicitlyDeclared
			End Get
		End Property

		Friend ReadOnly Property LocalAccessibility As Accessibility
			Get
				Return MyBase.DeclaredAccessibility
			End Get
		End Property

		Friend Overrides ReadOnly Property MayBeReducibleExtensionMethod As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				If (Me._lazyMetadataName Is Nothing) Then
					Dim overriddenMethod As MethodSymbol = Me.OverriddenMethod
					If (overriddenMethod Is Nothing) Then
						Interlocked.CompareExchange(Of String)(Me._lazyMetadataName, Me._name, Nothing)
					Else
						Interlocked.CompareExchange(Of String)(Me._lazyMetadataName, overriddenMethod.MetadataName, Nothing)
					End If
				End If
				Return Me._lazyMetadataName
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend Overrides ReadOnly Property OverriddenMembers As OverriddenMembersResult(Of MethodSymbol)
			Get
				Return OverriddenMembersResult(Of MethodSymbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property OverriddenMethod As MethodSymbol
			Get
				Return Me.m_property.GetAccessorOverride(Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.PropertyGet)
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Dim parameterSymbols As ImmutableArray(Of ParameterSymbol) = Me._lazyParameters
				If (parameterSymbols.IsDefault) Then
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
					Dim containingSourceModule As SourceModuleSymbol = MyBase.ContainingSourceModule
					parameterSymbols = Me.GetParameters(containingSourceModule, instance)
					Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = parameterSymbols.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As ParameterSymbol = enumerator.Current
						If (current.Locations.Length <= 0) Then
							Continue While
						End If
						Dim type As TypeSymbol = current.Type
						Dim locations As ImmutableArray(Of Location) = current.Locations
						type.CheckAllConstraints(locations(0), instance, New CompoundUseSiteInfo(Of AssemblySymbol)(instance, containingSourceModule.ContainingAssembly))
					End While
					containingSourceModule.AtomicStoreArrayAndDiagnostics(Of ParameterSymbol)(Me._lazyParameters, parameterSymbols, instance)
					instance.Free()
					parameterSymbols = Me._lazyParameters
				End If
				Return parameterSymbols
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me._lazyReturnType
				If (typeSymbol Is Nothing) Then
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
					Dim containingSourceModule As SourceModuleSymbol = MyBase.ContainingSourceModule
					Dim syntaxNodeOrToken As Microsoft.CodeAnalysis.SyntaxNodeOrToken = New Microsoft.CodeAnalysis.SyntaxNodeOrToken()
					typeSymbol = Me.GetReturnType(containingSourceModule, syntaxNodeOrToken, instance)
					If (Not syntaxNodeOrToken.IsKind(SyntaxKind.None)) Then
						Dim typeParameterDiagnosticInfos As ArrayBuilder(Of TypeParameterDiagnosticInfo) = ArrayBuilder(Of TypeParameterDiagnosticInfo).GetInstance()
						Dim typeParameterDiagnosticInfos1 As ArrayBuilder(Of TypeParameterDiagnosticInfo) = Nothing
						typeSymbol.CheckAllConstraints(typeParameterDiagnosticInfos, typeParameterDiagnosticInfos1, New CompoundUseSiteInfo(Of AssemblySymbol)(instance, containingSourceModule.ContainingAssembly))
						If (typeParameterDiagnosticInfos1 IsNot Nothing) Then
							typeParameterDiagnosticInfos.AddRange(typeParameterDiagnosticInfos1)
						End If
						Dim enumerator As ArrayBuilder(Of TypeParameterDiagnosticInfo).Enumerator = typeParameterDiagnosticInfos.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As TypeParameterDiagnosticInfo = enumerator.Current
							instance.Add(current.UseSiteInfo, syntaxNodeOrToken.GetLocation())
						End While
						typeParameterDiagnosticInfos.Free()
					End If
					containingSourceModule.AtomicStoreReferenceAndDiagnostics(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)(Me._lazyReturnType, typeSymbol, instance, Nothing)
					instance.Free()
					typeSymbol = Me._lazyReturnType
				End If
				Return typeSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnTypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Dim customModifiers As ImmutableArray(Of CustomModifier)
				Dim empty As ImmutableArray(Of CustomModifier)
				Dim overriddenMethod As MethodSymbol = Me.OverriddenMethod
				If (overriddenMethod Is Nothing) Then
					If (Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.PropertySet) Then
						empty = ImmutableArray(Of CustomModifier).Empty
					Else
						empty = Me.m_property.TypeCustomModifiers
					End If
					customModifiers = empty
				Else
					customModifiers = overriddenMethod.ReturnTypeCustomModifiers
				End If
				Return customModifiers
			End Get
		End Property

		Friend Overrides ReadOnly Property ShadowsExplicitly As Boolean
			Get
				Return Me.m_property.ShadowsExplicitly
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Return ImmutableArray(Of TypeParameterSymbol).Empty
			End Get
		End Property

		Shared Sub New()
			SourcePropertyAccessorSymbol.s_checkParameterModifierCallback = New Binder.CheckParameterModifierDelegate(AddressOf SourcePropertyAccessorSymbol.CheckParameterModifier)
		End Sub

		Friend Sub New(ByVal propertySymbol As SourcePropertySymbol, ByVal name As String, ByVal flags As SourceMemberFlags, ByVal syntaxRef As SyntaxReference, ByVal locations As ImmutableArray(Of Location))
			MyBase.New(propertySymbol.ContainingSourceType, If(flags.ToMethodKind() = Microsoft.CodeAnalysis.MethodKind.PropertyGet, flags, flags And SourceMemberFlags.AccessibilityPrivate Or SourceMemberFlags.AccessibilityProtected Or SourceMemberFlags.AccessibilityFriend Or SourceMemberFlags.AccessibilityProtectedFriend Or SourceMemberFlags.AccessibilityPrivateProtected Or SourceMemberFlags.AccessibilityPublic Or SourceMemberFlags.AccessibilityMask Or SourceMemberFlags.[Private] Or SourceMemberFlags.[Protected] Or SourceMemberFlags.[Friend] Or SourceMemberFlags.[Public] Or SourceMemberFlags.AllAccessibilityModifiers Or SourceMemberFlags.[Shared] Or SourceMemberFlags.[ReadOnly] Or SourceMemberFlags.[WriteOnly] Or SourceMemberFlags.AllWriteabilityModifiers Or SourceMemberFlags.[Overrides] Or SourceMemberFlags.[Overridable] Or SourceMemberFlags.[MustOverride] Or SourceMemberFlags.[NotOverridable] Or SourceMemberFlags.AllOverrideModifiers Or SourceMemberFlags.PrivateOverridableModifiers Or SourceMemberFlags.PrivateMustOverrideModifiers Or SourceMemberFlags.PrivateNotOverridableModifiers Or SourceMemberFlags.[Overloads] Or SourceMemberFlags.[Shadows] Or SourceMemberFlags.AllShadowingModifiers Or SourceMemberFlags.ShadowsAndOverrides Or SourceMemberFlags.[Default] Or SourceMemberFlags.[WithEvents] Or SourceMemberFlags.[Widening] Or SourceMemberFlags.[Narrowing] Or SourceMemberFlags.AllConversionModifiers Or SourceMemberFlags.InvalidInNotInheritableClass Or SourceMemberFlags.InvalidInNotInheritableOtherPartialClass Or SourceMemberFlags.InvalidInModule Or SourceMemberFlags.InvalidIfShared Or SourceMemberFlags.InvalidIfDefault Or SourceMemberFlags.[Partial] Or SourceMemberFlags.[MustInherit] Or SourceMemberFlags.[NotInheritable] Or SourceMemberFlags.TypeInheritModifiers Or SourceMemberFlags.Async Or SourceMemberFlags.[Dim] Or SourceMemberFlags.[Const] Or SourceMemberFlags.[Static] Or SourceMemberFlags.DeclarationModifierFlagShift Or SourceMemberFlags.InferredFieldType Or SourceMemberFlags.FirstFieldDeclarationOfType Or SourceMemberFlags.MethodIsSub Or SourceMemberFlags.MethodHandlesEvents Or SourceMemberFlags.MethodKindOrdinary Or SourceMemberFlags.MethodKindConstructor Or SourceMemberFlags.MethodKindSharedConstructor Or SourceMemberFlags.MethodKindDelegateInvoke Or SourceMemberFlags.MethodKindOperator Or SourceMemberFlags.MethodKindConversion Or SourceMemberFlags.MethodKindPropertyGet Or SourceMemberFlags.MethodKindPropertySet Or SourceMemberFlags.MethodKindEventAdd Or SourceMemberFlags.MethodKindEventRemove Or SourceMemberFlags.MethodKindEventRaise Or SourceMemberFlags.MethodKindDeclare Or SourceMemberFlags.MethodKindMask Or SourceMemberFlags.MethodKindShift), syntaxRef, locations)
			Me.m_property = propertySymbol
			Me._name = name
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			If (Me.m_property.IsAutoProperty) Then
				Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
				Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
				Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
				Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor, typedConstants, keyValuePairs, False))
			End If
		End Sub

		Private Shared Function BindParameters(ByVal propertySymbol As SourcePropertySymbol, ByVal method As SourcePropertyAccessorSymbol, ByVal location As Microsoft.CodeAnalysis.Location, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal parameterListOpt As ParameterListSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)
			Dim length As Integer = propertySymbol.Parameters.Length
			Dim methodKind As Boolean = method.MethodKind = Microsoft.CodeAnalysis.MethodKind.PropertySet
			Dim parameterSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax) = If(parameterListOpt Is Nothing OrElse Not methodKind, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax)(), parameterListOpt.Parameters)
			Dim flag As Boolean = If(Not methodKind, False, parameterSyntaxes.Count = 0)
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol).GetInstance(length + parameterSyntaxes.Count + If(flag, 1, 0))
			propertySymbol.CloneParametersForAccessor(method, instance)
			If (parameterSyntaxes.Count > 0) Then
				binder.DecodeParameterList(method, False, SourceMemberFlags.None, parameterSyntaxes, instance, SourcePropertyAccessorSymbol.s_checkParameterModifierCallback, diagnostics)
				Dim item As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = instance(length)
				If (Not CaseInsensitiveComparison.Equals(item.Name, "Value")) Then
					Dim parameterSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax = parameterSyntaxes(0)
					Microsoft.CodeAnalysis.VisualBasic.Binder.CheckParameterNameNotDuplicate(instance, length, parameterSyntax, item, diagnostics)
				End If
				If (parameterSyntaxes.Count <> 1) Then
					diagnostics.Add(ERRID.ERR_SetHasOnlyOneParam, location)
				Else
					Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = propertySymbol.Type
					Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = instance(instance.Count - 1)
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = parameterSymbol.Type
					If (type.IsSameTypeIgnoringAll(typeSymbol)) Then
						Dim overriddenMethod As MethodSymbol = method.OverriddenMethod
						If (overriddenMethod IsNot Nothing) Then
							Dim item1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = overriddenMethod.Parameters(instance.Count - 1)
							If (item1.Type.IsSameTypeIgnoringAll(typeSymbol) AndAlso CustomModifierUtils.CopyParameterCustomModifiers(item1, parameterSymbol)) Then
								instance(instance.Count - 1) = parameterSymbol
							End If
						End If
					ElseIf (Not type.IsErrorType() AndAlso Not typeSymbol.IsErrorType()) Then
						Dim locations As ImmutableArray(Of Microsoft.CodeAnalysis.Location) = parameterSymbol.Locations
						diagnostics.Add(ERRID.ERR_SetValueNotPropertyType, locations(0))
					End If
				End If
			ElseIf (flag) Then
				instance.Add(SynthesizedParameterSymbol.CreateSetAccessorValueParameter(method, propertySymbol, "Value"))
			End If
			Return instance.ToImmutableAndFree()
		End Function

		Private Shared Function CheckParameterModifier(ByVal container As Symbol, ByVal token As SyntaxToken, ByVal flag As SourceParameterFlags, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As SourceParameterFlags
			Dim sourceParameterFlag As SourceParameterFlags
			If (flag = SourceParameterFlags.[ByVal]) Then
				sourceParameterFlag = SourceParameterFlags.[ByVal]
			Else
				Dim location As Microsoft.CodeAnalysis.Location = token.GetLocation()
				diagnostics.Add(ERRID.ERR_SetHasToBeByVal1, location, New [Object]() { token.ToString() })
				sourceParameterFlag = flag And SourceParameterFlags.[ByVal]
			End If
			Return sourceParameterFlag
		End Function

		Friend Shared Function CreatePropertyAccessor(ByVal propertySymbol As SourcePropertySymbol, ByVal kindFlags As SourceMemberFlags, ByVal propertyFlags As SourceMemberFlags, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal blockSyntax As AccessorBlockSyntax, ByVal diagnostics As DiagnosticBag) As SourcePropertyAccessorSymbol
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertyAccessorSymbol Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertyAccessorSymbol::CreatePropertyAccessor(Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol,Microsoft.CodeAnalysis.VisualBasic.SourceMemberFlags,Microsoft.CodeAnalysis.VisualBasic.SourceMemberFlags,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax,Microsoft.CodeAnalysis.DiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertyAccessorSymbol CreatePropertyAccessor(Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol,Microsoft.CodeAnalysis.VisualBasic.SourceMemberFlags,Microsoft.CodeAnalysis.VisualBasic.SourceMemberFlags,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.VisualBasic.Syntax.AccessorBlockSyntax,Microsoft.CodeAnalysis.DiagnosticBag)
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

		Friend Overrides Sub DecodeWellKnownAttribute(ByRef arguments As DecodeWellKnownAttributeArguments(Of AttributeSyntax, VisualBasicAttributeData, AttributeLocation))
			If (arguments.SymbolPart = AttributeLocation.None AndAlso arguments.Attribute.IsTargetAttribute(Me, AttributeDescription.DebuggerHiddenAttribute)) Then
				arguments.GetOrCreateData(Of MethodWellKnownAttributeData)().IsPropertyAccessorWithDebuggerHiddenAttribute = True
			End If
			MyBase.DecodeWellKnownAttribute(arguments)
		End Sub

		Protected Overrides Function GetAttributeDeclarations() As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
			Dim oneOrMany As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
			oneOrMany = If(Not Me.m_property.IsCustomProperty, New OneOrMany(Of SyntaxList(Of AttributeListSyntax))(), Roslyn.Utilities.OneOrMany.Create(Of SyntaxList(Of AttributeListSyntax))(MyBase.AttributeDeclarationSyntaxList))
			Return oneOrMany
		End Function

		Friend Overrides Function GetBoundMethodBody(ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByRef methodBodyBinder As Binder = Nothing) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			boundBlock = If(Not Me.m_property.IsAutoProperty, MyBase.GetBoundMethodBody(compilationState, diagnostics, methodBodyBinder), SynthesizedPropertyAccessorHelper.GetBoundMethodBody(Me, Me.m_property.AssociatedField, methodBodyBinder))
			Return boundBlock
		End Function

		Friend Overrides Function GetLexicalSortKey() As LexicalSortKey
			If (Me.m_property.IsCustomProperty) Then
				Return MyBase.GetLexicalSortKey()
			End If
			Return Me.m_property.GetLexicalSortKey()
		End Function

		Private Function GetParameters(ByVal sourceModule As SourceModuleSymbol, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of ParameterSymbol)
			Dim parameterSymbols As ImmutableArray(Of ParameterSymbol)
			If (Not Me.m_property.IsCustomProperty) Then
				parameterSymbols = If(Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.PropertyGet, SourcePropertyAccessorSymbol.SynthesizeAutoGetterParameters(Me, Me.m_property), SourcePropertyAccessorSymbol.SynthesizeAutoSetterParameters(Me, Me.m_property))
			Else
				Dim locationSpecificBinder As Binder = BinderBuilder.CreateBinderForType(sourceModule, MyBase.SyntaxTree, Me.m_property.ContainingSourceType)
				locationSpecificBinder = New Microsoft.CodeAnalysis.VisualBasic.LocationSpecificBinder(BindingLocation.PropertyAccessorSignature, Me, locationSpecificBinder)
				parameterSymbols = SourcePropertyAccessorSymbol.BindParameters(Me.m_property, Me, System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of Location)(Me.Locations), locationSpecificBinder, MyBase.BlockSyntax.BlockStatement.ParameterList, diagBag)
			End If
			Return parameterSymbols
		End Function

		Private Function GetReturnType(ByVal sourceModule As SourceModuleSymbol, ByRef errorLocation As SyntaxNodeOrToken, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As TypeSymbol
			Dim specialType As TypeSymbol
			Dim methodKind As Microsoft.CodeAnalysis.MethodKind = Me.MethodKind
			If (methodKind = Microsoft.CodeAnalysis.MethodKind.PropertyGet) Then
				Dim type As TypeSymbol = DirectCast(Me.AssociatedSymbol, PropertySymbol).Type
				Dim overriddenMethod As MethodSymbol = Me.OverriddenMethod
				If (overriddenMethod IsNot Nothing AndAlso overriddenMethod.ReturnType.IsSameTypeIgnoringAll(type)) Then
					type = overriddenMethod.ReturnType
				End If
				specialType = type
			Else
				If (methodKind <> Microsoft.CodeAnalysis.MethodKind.PropertySet) Then
					Throw ExceptionUtilities.Unreachable
				End If
				specialType = BinderBuilder.CreateBinderForType(sourceModule, MyBase.SyntaxTree, Me.m_property.ContainingSourceType).GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Void, MyBase.DeclarationSyntax, diagBag)
			End If
			Return specialType
		End Function

		Protected Overrides Function GetReturnTypeAttributeDeclarations() As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
			Return New OneOrMany(Of SyntaxList(Of AttributeListSyntax))()
		End Function

		Private Shared Function SynthesizeAutoGetterParameters(ByVal getter As SourcePropertyAccessorSymbol, ByVal propertySymbol As SourcePropertySymbol) As ImmutableArray(Of ParameterSymbol)
			Dim immutableAndFree As ImmutableArray(Of ParameterSymbol)
			If (propertySymbol.ParameterCount <> 0) Then
				Dim instance As ArrayBuilder(Of ParameterSymbol) = ArrayBuilder(Of ParameterSymbol).GetInstance(propertySymbol.ParameterCount)
				propertySymbol.CloneParametersForAccessor(getter, instance)
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = ImmutableArray(Of ParameterSymbol).Empty
			End If
			Return immutableAndFree
		End Function

		Private Shared Function SynthesizeAutoSetterParameters(ByVal setter As SourcePropertyAccessorSymbol, ByVal propertySymbol As SourcePropertySymbol) As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)
			Dim immutableAndFree As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)
			Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = SynthesizedParameterSymbol.CreateSetAccessorValueParameter(setter, propertySymbol, If(propertySymbol.IsAutoProperty, "AutoPropertyValue", "Value"))
			If (propertySymbol.ParameterCount <> 0) Then
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol).GetInstance(propertySymbol.ParameterCount + 1)
				propertySymbol.CloneParametersForAccessor(setter, instance)
				instance.Add(parameterSymbol)
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)(parameterSymbol)
			End If
			Return immutableAndFree
		End Function
	End Class
End Namespace