Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Linq
Imports System.Reflection
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SourcePropertySymbol
		Inherits PropertySymbol
		Implements IAttributeTargetSymbol
		Private ReadOnly _containingType As SourceMemberContainerTypeSymbol

		Private ReadOnly _name As String

		Private _lazyMetadataName As String

		Private ReadOnly _syntaxRef As Microsoft.CodeAnalysis.SyntaxReference

		Private ReadOnly _blockRef As Microsoft.CodeAnalysis.SyntaxReference

		Private ReadOnly _location As Location

		Private ReadOnly _flags As SourceMemberFlags

		Private _lazyType As TypeSymbol

		Private _lazyParameters As ImmutableArray(Of ParameterSymbol)

		Private _getMethod As MethodSymbol

		Private _setMethod As MethodSymbol

		Private _backingField As FieldSymbol

		Private _lazyDocComment As String

		Private _lazyExpandedDocComment As String

		Private _lazyMeParameter As ParameterSymbol

		Private _lazyCustomAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData)

		Private _lazyReturnTypeCustomAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData)

		Private _lazyImplementedProperties As ImmutableArray(Of PropertySymbol)

		Private _lazyOverriddenProperties As OverriddenMembersResult(Of PropertySymbol)

		Private _lazyState As Integer

		Private ReadOnly Shared s_overridableModifierKinds As SyntaxKind()

		Private ReadOnly Shared s_accessibilityModifierKinds As SyntaxKind()

		Friend Overrides ReadOnly Property AssociatedField As FieldSymbol
			Get
				Return Me._backingField
			End Get
		End Property

		Friend ReadOnly Property BlockSyntaxReference As Microsoft.CodeAnalysis.SyntaxReference
			Get
				Return Me._blockRef
			End Get
		End Property

		Friend Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
			Get
				If (Not Me.IsShared) Then
					Return Microsoft.Cci.CallingConvention.HasThis
				End If
				Return Microsoft.Cci.CallingConvention.[Default]
			End Get
		End Property

		Public ReadOnly Property ContainingSourceType As SourceMemberContainerTypeSymbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return Me._containingType
			End Get
		End Property

		Friend ReadOnly Property DeclarationSyntax As DeclarationStatementSyntax
			Get
				Dim parent As DeclarationStatementSyntax
				Dim visualBasicSyntax As VisualBasicSyntaxNode = Me._syntaxRef.GetVisualBasicSyntax(New CancellationToken())
				If (visualBasicSyntax.Kind() <> SyntaxKind.PropertyStatement) Then
					parent = DirectCast(visualBasicSyntax.Parent.Parent, FieldDeclarationSyntax)
				Else
					parent = DirectCast(visualBasicSyntax, PropertyStatementSyntax)
				End If
				Return parent
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return DirectCast((Me._flags And SourceMemberFlags.AccessibilityMask), Accessibility)
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of Microsoft.CodeAnalysis.SyntaxReference)
			Get
				Return Symbol.GetDeclaringSyntaxReferenceHelper(Me._syntaxRef)
			End Get
		End Property

		Public ReadOnly Property DefaultAttributeLocation As AttributeLocation Implements IAttributeTargetSymbol.DefaultAttributeLocation
			Get
				Return AttributeLocation.[Property]
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of PropertySymbol)
			Get
				If (Me._lazyImplementedProperties.IsDefault) Then
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
					DirectCast(Me.ContainingModule, SourceModuleSymbol).AtomicStoreArrayAndDiagnostics(Of PropertySymbol)(Me._lazyImplementedProperties, Me.ComputeExplicitInterfaceImplementations(instance), instance)
					instance.Free()
				End If
				Return Me._lazyImplementedProperties
			End Get
		End Property

		Public Overrides ReadOnly Property GetMethod As MethodSymbol
			Get
				Return Me._getMethod
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Dim decodedWellKnownAttributeData As CommonPropertyWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return decodedWellKnownAttributeData.HasSpecialNameAttribute
			End Get
		End Property

		Friend ReadOnly Property IsAutoProperty As Boolean
			Get
				If (Me.IsWithEvents) Then
					Return False
				End If
				Return CObj(Me._backingField) <> CObj(Nothing)
			End Get
		End Property

		Friend ReadOnly Property IsCustomProperty As Boolean
			Get
				If (Me._backingField IsNot Nothing) Then
					Return False
				End If
				Return Not Me.IsMustOverride
			End Get
		End Property

		Public Overrides ReadOnly Property IsDefault As Boolean
			Get
				Return (Me._flags And SourceMemberFlags.[Default]) <> SourceMemberFlags.None
			End Get
		End Property

		Friend Overrides ReadOnly Property IsDirectlyExcludedFromCodeCoverage As Boolean
			Get
				Dim decodedWellKnownAttributeData As CommonPropertyWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return decodedWellKnownAttributeData.HasExcludeFromCodeCoverageAttribute
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me._containingType.AreMembersImplicitlyDeclared
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return (Me._flags And SourceMemberFlags.[MustOverride]) <> SourceMemberFlags.None
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMyGroupCollectionProperty As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return (Me._flags And SourceMemberFlags.[NotOverridable]) <> SourceMemberFlags.None
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverloads As Boolean
			Get
				' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol::get_IsOverloads()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean get_IsOverloads()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
				'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à ..(MethodBody , & ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\PropertyDecompiler.cs:ligne 345
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return (Me._flags And SourceMemberFlags.[Overridable]) <> SourceMemberFlags.None
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return (Me._flags And SourceMemberFlags.[Overrides]) <> SourceMemberFlags.None
			End Get
		End Property

		Public Overrides ReadOnly Property IsReadOnly As Boolean
			Get
				Return (Me._flags And SourceMemberFlags.[ReadOnly]) <> SourceMemberFlags.None
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return (Me._flags And SourceMemberFlags.[Shared]) <> SourceMemberFlags.None
			End Get
		End Property

		Public Overrides ReadOnly Property IsWithEvents As Boolean
			Get
				Return (Me._flags And SourceMemberFlags.[WithEvents]) <> SourceMemberFlags.None
			End Get
		End Property

		Public Overrides ReadOnly Property IsWriteOnly As Boolean
			Get
				Return (Me._flags And SourceMemberFlags.[WriteOnly]) <> SourceMemberFlags.None
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return ImmutableArray.Create(Of Location)(Me._location)
			End Get
		End Property

		Friend Overrides ReadOnly Property MeParameter As ParameterSymbol
			Get
				Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol
				If (Not Me.IsShared) Then
					If (Me._lazyMeParameter Is Nothing) Then
						Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)(Me._lazyMeParameter, New MeParameterSymbol(Me), Nothing)
					End If
					parameterSymbol = Me._lazyMeParameter
				Else
					parameterSymbol = Nothing
				End If
				Return parameterSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				If (Me._lazyMetadataName Is Nothing) Then
					OverloadingHelper.SetMetadataNameForAllOverloads(Me._name, SymbolKind.[Property], Me._containingType)
				End If
				Return Me._lazyMetadataName
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Dim uninitialized As Microsoft.CodeAnalysis.ObsoleteAttributeData
				Dim obsoleteAttributeDatum As Microsoft.CodeAnalysis.ObsoleteAttributeData
				If (Me._containingType.AnyMemberHasAttributes) Then
					Dim customAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me._lazyCustomAttributesBag
					If (customAttributesBag Is Nothing OrElse Not customAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed) Then
						uninitialized = Microsoft.CodeAnalysis.ObsoleteAttributeData.Uninitialized
					Else
						Dim earlyDecodedWellKnownAttributeData As CommonPropertyEarlyWellKnownAttributeData = DirectCast(Me._lazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData, CommonPropertyEarlyWellKnownAttributeData)
						If (earlyDecodedWellKnownAttributeData IsNot Nothing) Then
							obsoleteAttributeDatum = earlyDecodedWellKnownAttributeData.ObsoleteAttributeData
						Else
							obsoleteAttributeDatum = Nothing
						End If
						uninitialized = obsoleteAttributeDatum
					End If
				Else
					uninitialized = Nothing
				End If
				Return uninitialized
			End Get
		End Property

		Friend ReadOnly Property OverloadsExplicitly As Boolean
			Get
				Return (Me._flags And SourceMemberFlags.[Overloads]) <> SourceMemberFlags.None
			End Get
		End Property

		Friend Overrides ReadOnly Property OverriddenMembers As OverriddenMembersResult(Of PropertySymbol)
			Get
				Me.EnsureSignature()
				Return Me._lazyOverriddenProperties
			End Get
		End Property

		Friend ReadOnly Property OverridesExplicitly As Boolean
			Get
				Return (Me._flags And SourceMemberFlags.[Overrides]) <> SourceMemberFlags.None
			End Get
		End Property

		Public Overrides ReadOnly Property ParameterCount As Integer
			Get
				Dim length As Integer
				If (Me._lazyParameters.IsDefault) Then
					Dim declarationSyntax As DeclarationStatementSyntax = Me.DeclarationSyntax
					If (declarationSyntax.Kind() <> SyntaxKind.PropertyStatement) Then
						length = MyBase.ParameterCount
					Else
						Dim parameterList As ParameterListSyntax = DirectCast(declarationSyntax, PropertyStatementSyntax).ParameterList
						length = If(parameterList Is Nothing, 0, parameterList.Parameters.Count)
					End If
				Else
					length = Me._lazyParameters.Length
				End If
				Return length
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Me.EnsureSignature()
				Return Me._lazyParameters
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnsByRef As Boolean
			Get
				Return False
			End Get
		End Property

		Friend ReadOnly Property ReturnTypeMarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Dim decodedReturnTypeWellKnownAttributeData As CommonReturnTypeWellKnownAttributeData = Me.GetDecodedReturnTypeWellKnownAttributeData()
				If (decodedReturnTypeWellKnownAttributeData Is Nothing) Then
					Return Nothing
				End If
				Return decodedReturnTypeWellKnownAttributeData.MarshallingInformation
			End Get
		End Property

		Public Overrides ReadOnly Property SetMethod As MethodSymbol
			Get
				Return Me._setMethod
			End Get
		End Property

		Friend Overrides ReadOnly Property ShadowsExplicitly As Boolean
			Get
				Return (Me._flags And SourceMemberFlags.[Shadows]) <> SourceMemberFlags.None
			End Get
		End Property

		Friend ReadOnly Property Syntax As VisualBasicSyntaxNode
			Get
				If (Me._syntaxRef Is Nothing) Then
					Return Nothing
				End If
				Return Me._syntaxRef.GetVisualBasicSyntax(New CancellationToken())
			End Get
		End Property

		Friend ReadOnly Property SyntaxReference As Microsoft.CodeAnalysis.SyntaxReference
			Get
				Return Me._syntaxRef
			End Get
		End Property

		Friend ReadOnly Property SyntaxTree As Microsoft.CodeAnalysis.SyntaxTree
			Get
				Return Me._syntaxRef.SyntaxTree
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Me.EnsureSignature()
				Return Me._lazyType
			End Get
		End Property

		Public Overrides ReadOnly Property TypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Dim empty As ImmutableArray(Of CustomModifier)
				Dim overriddenProperty As PropertySymbol = MyBase.OverriddenProperty
				If (overriddenProperty IsNot Nothing) Then
					empty = overriddenProperty.TypeCustomModifiers
				Else
					empty = ImmutableArray(Of CustomModifier).Empty
				End If
				Return empty
			End Get
		End Property

		Shared Sub New()
			SourcePropertySymbol.s_overridableModifierKinds = New SyntaxKind() { SyntaxKind.OverridableKeyword }
			SourcePropertySymbol.s_accessibilityModifierKinds = New SyntaxKind() { GetType(<PrivateImplementationDetails>).GetField("FE50B78E155B99376440C95B439D133DF3AA9C3CA740A0DC83D4F274D62315F7").FieldHandle }
		End Sub

		Private Sub New(ByVal container As SourceMemberContainerTypeSymbol, ByVal name As String, ByVal flags As SourceMemberFlags, ByVal syntaxRef As Microsoft.CodeAnalysis.SyntaxReference, ByVal blockRef As Microsoft.CodeAnalysis.SyntaxReference, ByVal location As Microsoft.CodeAnalysis.Location)
			MyBase.New()
			Me._containingType = container
			Me._name = name
			Me._syntaxRef = syntaxRef
			Me._blockRef = blockRef
			Me._location = location
			Me._flags = flags
			Me._lazyState = 0
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			If (Me.Type.ContainsTupleNames()) Then
				Symbol.AddSynthesizedAttribute(attributes, Me.DeclaringCompilation.SynthesizeTupleNamesAttribute(Me.Type))
			End If
		End Sub

		Private Shared Function BindImplementsClause(ByVal containingType As SourceMemberContainerTypeSymbol, ByVal bodyBinder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal prop As SourcePropertySymbol, ByVal syntax As PropertyStatementSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of PropertySymbol)
			Dim empty As ImmutableArray(Of PropertySymbol)
			If (syntax.ImplementsClause IsNot Nothing) Then
				If (prop.IsShared And Not containingType.IsModuleType()) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, syntax.Modifiers.First(SyntaxKind.SharedKeyword), ERRID.ERR_SharedOnProcThatImpl, New [Object]() { syntax.Identifier.ToString() })
					empty = ImmutableArray(Of PropertySymbol).Empty
					Return empty
				End If
				empty = ImplementsHelper.ProcessImplementsClause(Of PropertySymbol)(syntax.ImplementsClause, prop, containingType, bodyBinder, diagnostics)
				Return empty
			End If
			empty = ImmutableArray(Of PropertySymbol).Empty
			Return empty
		End Function

		Friend Sub CloneParametersForAccessor(ByVal method As MethodSymbol, ByVal parameterBuilder As ArrayBuilder(Of ParameterSymbol))
			Dim overriddenMethod As MethodSymbol = method.OverriddenMethod
			Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = Me.Parameters.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As ParameterSymbol = enumerator.Current
				Dim sourceClonedParameterSymbol As ParameterSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceClonedParameterSymbol(DirectCast(current, SourceParameterSymbol), method, current.Ordinal)
				If (overriddenMethod IsNot Nothing) Then
					Dim parameters As ImmutableArray(Of ParameterSymbol) = overriddenMethod.Parameters
					CustomModifierUtils.CopyParameterCustomModifiers(parameters(current.Ordinal), sourceClonedParameterSymbol)
				End If
				parameterBuilder.Add(sourceClonedParameterSymbol)
			End While
		End Sub

		Private Function ComputeExplicitInterfaceImplementations(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of PropertySymbol)
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.CreateBinderForTypeDeclaration()
			Dim syntax As PropertyStatementSyntax = DirectCast(Me._syntaxRef.GetSyntax(New CancellationToken()), PropertyStatementSyntax)
			Return SourcePropertySymbol.BindImplementsClause(Me._containingType, binder, Me, syntax, diagnostics)
		End Function

		Private Function ComputeParameters(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of ParameterSymbol)
			Dim empty As ImmutableArray(Of ParameterSymbol)
			If (Not Me.IsWithEvents) Then
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.CreateBinderForTypeDeclaration()
				Dim syntax As PropertyStatementSyntax = DirectCast(Me._syntaxRef.GetSyntax(New CancellationToken()), PropertyStatementSyntax)
				Dim parameterSymbols As ImmutableArray(Of ParameterSymbol) = binder.DecodePropertyParameterList(Me, syntax.ParameterList, diagnostics)
				If (Me.IsDefault) Then
					If (Not SourcePropertySymbol.HasRequiredParameters(parameterSymbols)) Then
						diagnostics.Add(ERRID.ERR_DefaultPropertyWithNoParams, Me._location)
					End If
					binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Reflection_DefaultMemberAttribute__ctor, syntax, diagnostics)
				End If
				empty = parameterSymbols
			Else
				empty = ImmutableArray(Of ParameterSymbol).Empty
			End If
			Return empty
		End Function

		Private Function ComputeType(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim unknownResultType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim cancellationToken As System.Threading.CancellationToken
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.CreateBinderForTypeDeclaration()
			If (Not Me.IsWithEvents) Then
				Dim syntaxReference As Microsoft.CodeAnalysis.SyntaxReference = Me._syntaxRef
				cancellationToken = New System.Threading.CancellationToken()
				Dim syntax As PropertyStatementSyntax = DirectCast(syntaxReference.GetSyntax(cancellationToken), PropertyStatementSyntax)
				Dim asClause As AsClauseSyntax = syntax.AsClause
				If (asClause Is Nothing OrElse asClause.Kind() <> SyntaxKind.AsNewClause OrElse DirectCast(asClause, AsNewClauseSyntax).NewExpression.Kind() <> SyntaxKind.AnonymousObjectCreationExpression) Then
					Dim getErrorInfoERRStrictDisallowsImplicitProc As Func(Of DiagnosticInfo) = Nothing
					If (Not [String].IsNullOrEmpty(Me._name)) Then
						If (binder.OptionStrict = OptionStrict.[On]) Then
							getErrorInfoERRStrictDisallowsImplicitProc = ErrorFactory.GetErrorInfo_ERR_StrictDisallowsImplicitProc
						ElseIf (binder.OptionStrict = OptionStrict.Custom) Then
							getErrorInfoERRStrictDisallowsImplicitProc = ErrorFactory.GetErrorInfo_WRN_ObjectAssumedProperty1_WRN_MissingAsClauseinProperty
						End If
					End If
					Dim identifier As SyntaxToken = syntax.Identifier
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = binder.DecodeIdentifierType(identifier, asClause, getErrorInfoERRStrictDisallowsImplicitProc, diagnostics)
					If (Not typeSymbol.IsErrorType()) Then
						Dim asClauseLocation As SyntaxNodeOrToken = SourceSymbolHelpers.GetAsClauseLocation(identifier, asClause)
						AccessCheck.VerifyAccessExposureForMemberType(Me, asClauseLocation, typeSymbol, diagnostics, False)
						Dim typeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Nothing
						If (typeSymbol.IsRestrictedTypeOrArrayType(typeSymbol1)) Then
							Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, asClauseLocation, ERRID.ERR_RestrictedType1, New [Object]() { typeSymbol1 })
						End If
						Dim getMethod As MethodSymbol = Me.GetMethod
						If (getMethod IsNot Nothing AndAlso getMethod.IsIterator) Then
							Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = typeSymbol.OriginalDefinition
							If (originalDefinition.SpecialType <> SpecialType.System_Collections_Generic_IEnumerable_T AndAlso originalDefinition.SpecialType <> SpecialType.System_Collections_Generic_IEnumerator_T AndAlso typeSymbol.SpecialType <> SpecialType.System_Collections_IEnumerable AndAlso typeSymbol.SpecialType <> SpecialType.System_Collections_IEnumerator) Then
								Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, asClauseLocation, ERRID.ERR_BadIteratorReturn)
							End If
						End If
					End If
					unknownResultType = typeSymbol
				Else
					unknownResultType = ErrorTypeSymbol.UnknownResultType
				End If
			Else
				Dim syntaxReference1 As Microsoft.CodeAnalysis.SyntaxReference = Me._syntaxRef
				cancellationToken = New System.Threading.CancellationToken()
				Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax = DirectCast(syntaxReference1.GetSyntax(cancellationToken), Microsoft.CodeAnalysis.VisualBasic.Syntax.ModifiedIdentifierSyntax)
				unknownResultType = SourceMemberFieldSymbol.ComputeWithEventsFieldType(Me, modifiedIdentifierSyntax, binder, (Me._flags And SourceMemberFlags.FirstFieldDeclarationOfType) = SourceMemberFlags.None, diagnostics)
			End If
			Return unknownResultType
		End Function

		Friend Shared Function Create(ByVal containingType As SourceMemberContainerTypeSymbol, ByVal bodyBinder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal syntax As PropertyStatementSyntax, ByVal blockSyntaxOpt As PropertyBlockSyntax, ByVal diagnostics As DiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol
			Dim locations As ImmutableArray(Of Microsoft.CodeAnalysis.Location)
			Dim syntaxReference As Microsoft.CodeAnalysis.SyntaxReference
			Dim memberModifier As MemberModifiers = Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol.DecodeModifiers(syntax.Modifiers, containingType, bodyBinder, diagnostics)
			Dim identifier As SyntaxToken = syntax.Identifier
			Dim valueText As String = identifier.ValueText
			[String].IsNullOrEmpty(valueText)
			Dim location As Microsoft.CodeAnalysis.Location = identifier.GetLocation()
			Dim syntaxReference1 As Microsoft.CodeAnalysis.SyntaxReference = bodyBinder.GetSyntaxReference(syntax)
			If (blockSyntaxOpt Is Nothing) Then
				syntaxReference = Nothing
			Else
				syntaxReference = bodyBinder.GetSyntaxReference(blockSyntaxOpt)
			End If
			Dim syntaxReference2 As Microsoft.CodeAnalysis.SyntaxReference = syntaxReference
			Dim sourcePropertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol(containingType, valueText, memberModifier.AllFlags, syntaxReference1, syntaxReference2, location)
			bodyBinder = New LocationSpecificBinder(BindingLocation.PropertySignature, sourcePropertySymbol, bodyBinder)
			If (syntax.AttributeLists.Count = 0) Then
				sourcePropertySymbol.SetCustomAttributeData(CustomAttributesBag(Of VisualBasicAttributeData).Empty)
			End If
			Dim allFlags As SourceMemberFlags = memberModifier.AllFlags And (SourceMemberFlags.[Private] Or SourceMemberFlags.[Protected] Or SourceMemberFlags.[Friend] Or SourceMemberFlags.[Public] Or SourceMemberFlags.AllAccessibilityModifiers Or SourceMemberFlags.[Shared] Or SourceMemberFlags.[ReadOnly] Or SourceMemberFlags.[WriteOnly] Or SourceMemberFlags.AllWriteabilityModifiers Or SourceMemberFlags.[Overrides] Or SourceMemberFlags.[Overridable] Or SourceMemberFlags.[MustOverride] Or SourceMemberFlags.[NotOverridable] Or SourceMemberFlags.AllOverrideModifiers Or SourceMemberFlags.PrivateOverridableModifiers Or SourceMemberFlags.PrivateMustOverrideModifiers Or SourceMemberFlags.PrivateNotOverridableModifiers Or SourceMemberFlags.[Overloads] Or SourceMemberFlags.[Shadows] Or SourceMemberFlags.AllShadowingModifiers Or SourceMemberFlags.ShadowsAndOverrides Or SourceMemberFlags.[Default] Or SourceMemberFlags.[WithEvents] Or SourceMemberFlags.[Widening] Or SourceMemberFlags.[Narrowing] Or SourceMemberFlags.AllConversionModifiers Or SourceMemberFlags.InvalidInNotInheritableClass Or SourceMemberFlags.InvalidInNotInheritableOtherPartialClass Or SourceMemberFlags.InvalidInModule Or SourceMemberFlags.InvalidInInterface Or SourceMemberFlags.InvalidIfShared Or SourceMemberFlags.InvalidIfDefault Or SourceMemberFlags.[Partial] Or SourceMemberFlags.[MustInherit] Or SourceMemberFlags.[NotInheritable] Or SourceMemberFlags.TypeInheritModifiers Or SourceMemberFlags.Async Or SourceMemberFlags.Iterator Or SourceMemberFlags.[Dim] Or SourceMemberFlags.[Const] Or SourceMemberFlags.[Static] Or SourceMemberFlags.DeclarationModifierFlagMask Or SourceMemberFlags.InferredFieldType Or SourceMemberFlags.FirstFieldDeclarationOfType Or SourceMemberFlags.MethodIsSub Or SourceMemberFlags.MethodHandlesEvents Or SourceMemberFlags.MethodKindOrdinary Or SourceMemberFlags.MethodKindConstructor Or SourceMemberFlags.MethodKindSharedConstructor Or SourceMemberFlags.MethodKindDelegateInvoke Or SourceMemberFlags.MethodKindOperator Or SourceMemberFlags.MethodKindConversion Or SourceMemberFlags.MethodKindPropertyGet Or SourceMemberFlags.MethodKindPropertySet Or SourceMemberFlags.MethodKindEventAdd Or SourceMemberFlags.MethodKindEventRemove Or SourceMemberFlags.MethodKindEventRaise Or SourceMemberFlags.MethodKindDeclare)
			Dim sourcePropertyAccessorSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertyAccessorSymbol = Nothing
			Dim sourcePropertyAccessorSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertyAccessorSymbol = Nothing
			If (blockSyntaxOpt IsNot Nothing) Then
				Dim enumerator As SyntaxList(Of AccessorBlockSyntax).Enumerator = blockSyntaxOpt.Accessors.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As AccessorBlockSyntax = enumerator.Current
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = current.BlockStatement.Kind()
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement) Then
						If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement) Then
							Continue While
						End If
						Dim sourcePropertyAccessorSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertyAccessorSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol.CreateAccessor(sourcePropertySymbol, SourceMemberFlags.MethodKindPropertySet, allFlags, bodyBinder, current, diagnostics)
						If (sourcePropertyAccessorSymbol1 IsNot Nothing) Then
							locations = sourcePropertyAccessorSymbol2.Locations
							diagnostics.Add(ERRID.ERR_DuplicatePropertySet, locations(0))
						Else
							sourcePropertyAccessorSymbol1 = sourcePropertyAccessorSymbol2
						End If
					Else
						Dim sourcePropertyAccessorSymbol3 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertyAccessorSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol.CreateAccessor(sourcePropertySymbol, SourceMemberFlags.MethodKindPropertyGet, allFlags, bodyBinder, current, diagnostics)
						If (sourcePropertyAccessorSymbol IsNot Nothing) Then
							locations = sourcePropertyAccessorSymbol3.Locations
							diagnostics.Add(ERRID.ERR_DuplicatePropertyGet, locations(0))
						Else
							sourcePropertyAccessorSymbol = sourcePropertyAccessorSymbol3
						End If
					End If
				End While
			End If
			Dim foundFlags As Boolean = (memberModifier.FoundFlags And SourceMemberFlags.[ReadOnly]) <> SourceMemberFlags.None
			Dim flag As Boolean = (memberModifier.FoundFlags And SourceMemberFlags.[WriteOnly]) <> SourceMemberFlags.None
			If (Not sourcePropertySymbol.IsMustOverride) Then
				If (foundFlags) Then
					If (sourcePropertyAccessorSymbol IsNot Nothing) Then
						If (sourcePropertyAccessorSymbol.LocalAccessibility <> Accessibility.NotApplicable) Then
							diagnostics.Add(ERRID.ERR_ReadOnlyNoAccessorFlag, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol.GetAccessorBlockBeginLocation(sourcePropertyAccessorSymbol))
						End If
					ElseIf (blockSyntaxOpt IsNot Nothing) Then
						diagnostics.Add(ERRID.ERR_ReadOnlyHasNoGet, location)
					End If
					If (sourcePropertyAccessorSymbol1 IsNot Nothing) Then
						locations = sourcePropertyAccessorSymbol1.Locations
						diagnostics.Add(ERRID.ERR_ReadOnlyHasSet, locations(0))
					End If
				End If
				If (flag) Then
					If (sourcePropertyAccessorSymbol1 IsNot Nothing) Then
						If (sourcePropertyAccessorSymbol1.LocalAccessibility <> Accessibility.NotApplicable) Then
							diagnostics.Add(ERRID.ERR_WriteOnlyNoAccessorFlag, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol.GetAccessorBlockBeginLocation(sourcePropertyAccessorSymbol1))
						End If
					ElseIf (blockSyntaxOpt IsNot Nothing) Then
						diagnostics.Add(ERRID.ERR_WriteOnlyHasNoWrite, location)
					End If
					If (sourcePropertyAccessorSymbol IsNot Nothing) Then
						locations = sourcePropertyAccessorSymbol.Locations
						diagnostics.Add(ERRID.ERR_WriteOnlyHasGet, locations(0))
					End If
				End If
				If (sourcePropertyAccessorSymbol IsNot Nothing AndAlso sourcePropertyAccessorSymbol1 IsNot Nothing) Then
					If (sourcePropertyAccessorSymbol.LocalAccessibility <> Accessibility.NotApplicable AndAlso sourcePropertyAccessorSymbol1.LocalAccessibility <> Accessibility.NotApplicable) Then
						Dim sourcePropertyAccessorSymbol4 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertyAccessorSymbol = If(sourcePropertyAccessorSymbol.Locations(0).SourceSpan.Start < sourcePropertyAccessorSymbol1.Locations(0).SourceSpan.Start, sourcePropertyAccessorSymbol1, sourcePropertyAccessorSymbol)
						diagnostics.Add(ERRID.ERR_OnlyOneAccessorForGetSet, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol.GetAccessorBlockBeginLocation(sourcePropertyAccessorSymbol4))
					ElseIf (sourcePropertySymbol.IsOverridable AndAlso (sourcePropertyAccessorSymbol.LocalAccessibility = Accessibility.[Private] OrElse sourcePropertyAccessorSymbol1.LocalAccessibility = Accessibility.[Private])) Then
						bodyBinder.ReportModifierError(syntax.Modifiers, ERRID.ERR_BadPropertyAccessorFlags3, diagnostics, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol.s_overridableModifierKinds)
					End If
				End If
				If (Not foundFlags AndAlso Not flag AndAlso (sourcePropertyAccessorSymbol Is Nothing OrElse sourcePropertyAccessorSymbol1 Is Nothing) AndAlso blockSyntaxOpt IsNot Nothing AndAlso Not sourcePropertySymbol.IsMustOverride) Then
					diagnostics.Add(ERRID.ERR_PropMustHaveGetSet, location)
				End If
			End If
			If (blockSyntaxOpt IsNot Nothing) Then
				sourcePropertySymbol._getMethod = sourcePropertyAccessorSymbol
				sourcePropertySymbol._setMethod = sourcePropertyAccessorSymbol1
			Else
				If (Not sourcePropertySymbol.IsMustOverride) Then
					If (flag) Then
						diagnostics.Add(ERRID.ERR_AutoPropertyCantBeWriteOnly, location)
					End If
					Dim str As String = [String].Concat("_", sourcePropertySymbol._name)
					sourcePropertySymbol._backingField = New SynthesizedPropertyBackingFieldSymbol(sourcePropertySymbol, str, sourcePropertySymbol.IsShared)
				End If
				Dim sourceMemberFlag As SourceMemberFlags = sourcePropertySymbol._flags And (SourceMemberFlags.[Friend] Or SourceMemberFlags.[Public] Or SourceMemberFlags.[Shared] Or SourceMemberFlags.[ReadOnly] Or SourceMemberFlags.[WriteOnly] Or SourceMemberFlags.AllWriteabilityModifiers Or SourceMemberFlags.[Overrides] Or SourceMemberFlags.[Overridable] Or SourceMemberFlags.[MustOverride] Or SourceMemberFlags.[NotOverridable] Or SourceMemberFlags.AllOverrideModifiers Or SourceMemberFlags.[Overloads] Or SourceMemberFlags.[Shadows] Or SourceMemberFlags.AllShadowingModifiers Or SourceMemberFlags.ShadowsAndOverrides Or SourceMemberFlags.[Default] Or SourceMemberFlags.[WithEvents] Or SourceMemberFlags.[Widening] Or SourceMemberFlags.[Narrowing] Or SourceMemberFlags.AllConversionModifiers Or SourceMemberFlags.InvalidInNotInheritableClass Or SourceMemberFlags.InvalidInNotInheritableOtherPartialClass Or SourceMemberFlags.InvalidIfShared Or SourceMemberFlags.[Partial] Or SourceMemberFlags.[MustInherit] Or SourceMemberFlags.[NotInheritable] Or SourceMemberFlags.TypeInheritModifiers Or SourceMemberFlags.Async Or SourceMemberFlags.Iterator Or SourceMemberFlags.[Dim] Or SourceMemberFlags.[Const] Or SourceMemberFlags.[Static] Or SourceMemberFlags.InferredFieldType Or SourceMemberFlags.FirstFieldDeclarationOfType Or SourceMemberFlags.MethodIsSub Or SourceMemberFlags.MethodHandlesEvents Or SourceMemberFlags.MethodKindOrdinary Or SourceMemberFlags.MethodKindConstructor Or SourceMemberFlags.MethodKindSharedConstructor Or SourceMemberFlags.MethodKindDelegateInvoke Or SourceMemberFlags.MethodKindOperator Or SourceMemberFlags.MethodKindConversion Or SourceMemberFlags.MethodKindPropertyGet Or SourceMemberFlags.MethodKindPropertySet Or SourceMemberFlags.MethodKindEventAdd Or SourceMemberFlags.MethodKindEventRemove Or SourceMemberFlags.MethodKindEventRaise Or SourceMemberFlags.MethodKindDeclare)
				If (Not flag) Then
					sourcePropertySymbol._getMethod = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertyAccessorSymbol(sourcePropertySymbol, Microsoft.CodeAnalysis.VisualBasic.Binder.GetAccessorName(sourcePropertySymbol.Name, MethodKind.PropertyGet, False), sourceMemberFlag Or SourceMemberFlags.MethodKindPropertyGet, sourcePropertySymbol._syntaxRef, sourcePropertySymbol.Locations)
				End If
				If (Not foundFlags) Then
					sourcePropertySymbol._setMethod = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertyAccessorSymbol(sourcePropertySymbol, Microsoft.CodeAnalysis.VisualBasic.Binder.GetAccessorName(sourcePropertySymbol.Name, MethodKind.PropertySet, sourcePropertySymbol.IsCompilationOutputWinMdObj()), sourceMemberFlag Or SourceMemberFlags.MethodKindPropertySet Or SourceMemberFlags.[Dim], sourcePropertySymbol._syntaxRef, sourcePropertySymbol.Locations)
				End If
			End If
			Return sourcePropertySymbol
		End Function

		Private Shared Function CreateAccessor(ByVal [property] As SourcePropertySymbol, ByVal kindFlags As SourceMemberFlags, ByVal propertyFlags As SourceMemberFlags, ByVal bodyBinder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal syntax As AccessorBlockSyntax, ByVal diagnostics As DiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertyAccessorSymbol
			Dim sourcePropertyAccessorSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertyAccessorSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertyAccessorSymbol.CreatePropertyAccessor([property], kindFlags, propertyFlags, bodyBinder, syntax, diagnostics)
			Dim localAccessibility As Accessibility = sourcePropertyAccessorSymbol.LocalAccessibility
			If (Not SourcePropertySymbol.IsAccessibilityMoreRestrictive([property].DeclaredAccessibility, localAccessibility)) Then
				SourcePropertySymbol.ReportAccessorAccessibilityError(bodyBinder, syntax, ERRID.ERR_BadPropertyAccessorFlagsRestrict, diagnostics)
				Return sourcePropertyAccessorSymbol
			End If
			If ([property].IsNotOverridable AndAlso localAccessibility = Accessibility.[Private]) Then
				SourcePropertySymbol.ReportAccessorAccessibilityError(bodyBinder, syntax, ERRID.ERR_BadPropertyAccessorFlags1, diagnostics)
				Return sourcePropertyAccessorSymbol
			End If
			If ([property].IsDefault AndAlso localAccessibility = Accessibility.[Private]) Then
				SourcePropertySymbol.ReportAccessorAccessibilityError(bodyBinder, syntax, ERRID.ERR_BadPropertyAccessorFlags2, diagnostics)
			End If
			Return sourcePropertyAccessorSymbol
		End Function

		Private Function CreateBinderForTypeDeclaration() As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForType(DirectCast(Me.ContainingModule, SourceModuleSymbol), Me._syntaxRef.SyntaxTree, Me._containingType)
			Return New LocationSpecificBinder(BindingLocation.PropertySignature, Me, binder)
		End Function

		Friend Shared Function CreateWithEvents(ByVal containingType As SourceMemberContainerTypeSymbol, ByVal bodyBinder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal identifier As SyntaxToken, ByVal syntaxRef As Microsoft.CodeAnalysis.SyntaxReference, ByVal modifiers As MemberModifiers, ByVal firstFieldDeclarationOfType As Boolean, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As SourcePropertySymbol
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol::CreateWithEvents(Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.SyntaxToken,Microsoft.CodeAnalysis.SyntaxReference,Microsoft.CodeAnalysis.VisualBasic.MemberModifiers,System.Boolean,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol CreateWithEvents(Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.SyntaxToken,Microsoft.CodeAnalysis.SyntaxReference,Microsoft.CodeAnalysis.VisualBasic.MemberModifiers,System.Boolean,Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
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

		Private Shared Function DecodeModifiers(ByVal modifiers As SyntaxTokenList, ByVal container As SourceMemberContainerTypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As DiagnosticBag) As MemberModifiers
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.MemberModifiers Microsoft.CodeAnalysis.VisualBasic.Symbols.SourcePropertySymbol::DecodeModifiers(Microsoft.CodeAnalysis.SyntaxTokenList,Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.DiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.MemberModifiers DecodeModifiers(Microsoft.CodeAnalysis.SyntaxTokenList,Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.DiagnosticBag)
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
			Dim attribute As VisualBasicAttributeData = arguments.Attribute
			Dim diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = DirectCast(arguments.Diagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (attribute.IsTargetAttribute(Me, AttributeDescription.TupleElementNamesAttribute)) Then
				diagnostics.Add(ERRID.ERR_ExplicitTupleElementNamesAttribute, arguments.AttributeSyntaxOpt.Location)
			End If
			If (arguments.SymbolPart <> AttributeLocation.[Return]) Then
				If (attribute.IsTargetAttribute(Me, AttributeDescription.SpecialNameAttribute)) Then
					arguments.GetOrCreateData(Of CommonPropertyWellKnownAttributeData)().HasSpecialNameAttribute = True
					Return
				End If
				If (attribute.IsTargetAttribute(Me, AttributeDescription.ExcludeFromCodeCoverageAttribute)) Then
					arguments.GetOrCreateData(Of CommonPropertyWellKnownAttributeData)().HasExcludeFromCodeCoverageAttribute = True
					Return
				End If
				If (Me.IsWithEvents OrElse Not attribute.IsTargetAttribute(Me, AttributeDescription.DebuggerHiddenAttribute)) Then
					MyBase.DecodeWellKnownAttribute(arguments)
					Return
				End If
				If ((Me._getMethod Is Nothing OrElse Not DirectCast(Me._getMethod, SourcePropertyAccessorSymbol).HasDebuggerHiddenAttribute) AndAlso (Me._setMethod Is Nothing OrElse Not DirectCast(Me._setMethod, SourcePropertyAccessorSymbol).HasDebuggerHiddenAttribute)) Then
					diagnostics.Add(ERRID.WRN_DebuggerHiddenIgnoredOnProperties, arguments.AttributeSyntaxOpt.GetLocation())
					Return
				Else
					Return
				End If
			Else
				Dim flag As Boolean = attribute.IsTargetAttribute(Me, AttributeDescription.MarshalAsAttribute)
				If (Me._getMethod Is Nothing AndAlso Me._setMethod IsNot Nothing AndAlso (Not flag OrElse Not SynthesizedParameterSimpleSymbol.IsMarshalAsAttributeApplicable(Me._setMethod))) Then
					diagnostics.Add(ERRID.WRN_ReturnTypeAttributeOnWriteOnlyProperty, arguments.AttributeSyntaxOpt.GetLocation())
					Return
				End If
				If (flag) Then
					MarshalAsAttributeDecoder(Of CommonReturnTypeWellKnownAttributeData, AttributeSyntax, VisualBasicAttributeData, AttributeLocation).Decode(arguments, AttributeTargets.Field, MessageProvider.Instance)
					Return
				End If
			End If
			MyBase.DecodeWellKnownAttribute(arguments)
		End Sub

		Friend Overrides Function EarlyDecodeWellKnownAttribute(ByRef arguments As EarlyDecodeWellKnownAttributeArguments(Of EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation)) As VisualBasicAttributeData
			Dim visualBasicAttributeDatum As VisualBasicAttributeData
			Dim visualBasicAttributeDatum1 As VisualBasicAttributeData = Nothing
			Dim obsoleteAttributeDatum As Microsoft.CodeAnalysis.ObsoleteAttributeData = Nothing
			If (Not MyBase.EarlyDecodeDeprecatedOrExperimentalOrObsoleteAttribute(arguments, visualBasicAttributeDatum1, obsoleteAttributeDatum)) Then
				visualBasicAttributeDatum = MyBase.EarlyDecodeWellKnownAttribute(arguments)
			Else
				If (obsoleteAttributeDatum IsNot Nothing) Then
					arguments.GetOrCreateData(Of CommonPropertyEarlyWellKnownAttributeData)().ObsoleteAttributeData = obsoleteAttributeDatum
				End If
				visualBasicAttributeDatum = visualBasicAttributeDatum1
			End If
			Return visualBasicAttributeDatum
		End Function

		Private Sub EnsureSignature()
			Dim empty As OverriddenMembersResult(Of PropertySymbol)
			If (Me._lazyParameters.IsDefault) Then
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
				Dim containingModule As SourceModuleSymbol = DirectCast(Me.ContainingModule, SourceModuleSymbol)
				Dim parameterSymbols As ImmutableArray(Of ParameterSymbol) = Me.ComputeParameters(instance)
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.ComputeType(instance)
				If (Not Me.IsOverrides OrElse Not OverrideHidingHelper.CanOverrideOrHide(Me)) Then
					empty = OverriddenMembersResult(Of PropertySymbol).Empty
				Else
					Dim instance1 As ArrayBuilder(Of ParameterSymbol) = ArrayBuilder(Of ParameterSymbol).GetInstance(parameterSymbols.Length)
					Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = parameterSymbols.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As ParameterSymbol = enumerator.Current
						instance1.Add(New SignatureOnlyParameterSymbol(current.Type, ImmutableArray(Of CustomModifier).Empty, ImmutableArray(Of CustomModifier).Empty, Nothing, False, current.IsByRef, False, current.IsOptional))
					End While
					empty = OverrideHidingHelper(Of PropertySymbol).MakeOverriddenMembers(New SignatureOnlyPropertySymbol(Me.Name, Me._containingType, Me.IsReadOnly, Me.IsWriteOnly, instance1.ToImmutableAndFree(), False, typeSymbol, ImmutableArray(Of CustomModifier).Empty, ImmutableArray(Of CustomModifier).Empty, True, Me.IsWithEvents))
				End If
				Dim overriddenMember As PropertySymbol = empty.OverriddenMember
				If (overriddenMember IsNot Nothing) Then
					Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = overriddenMember.Type
					If (typeSymbol.IsSameType(type, TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds)) Then
						typeSymbol = CustomModifierUtils.CopyTypeCustomModifiers(type, typeSymbol)
					End If
					parameterSymbols = CustomModifierUtils.CopyParameterCustomModifiers(overriddenMember.Parameters, parameterSymbols)
				End If
				Interlocked.CompareExchange(Of OverriddenMembersResult(Of PropertySymbol))(Me._lazyOverriddenProperties, empty, Nothing)
				Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)(Me._lazyType, typeSymbol, Nothing)
				containingModule.AtomicStoreArrayAndDiagnostics(Of ParameterSymbol)(Me._lazyParameters, parameterSymbols, instance)
				instance.Free()
			End If
		End Sub

		Friend Overrides Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
			MyBase.GenerateDeclarationErrors(cancellationToken)
			Dim type As TypeSymbol = Me.Type
			Dim parameters As ImmutableArray(Of ParameterSymbol) = Me.Parameters
			Me.GetReturnTypeAttributesBag()
			Dim explicitInterfaceImplementations As ImmutableArray(Of PropertySymbol) = Me.ExplicitInterfaceImplementations
			If (Me.DeclaringCompilation.EventQueue IsNot Nothing) Then
				DirectCast(Me.ContainingModule, SourceModuleSymbol).AtomicSetFlagAndRaiseSymbolDeclaredEvent(Me._lazyState, 1, 0, Me)
			End If
		End Sub

		Private Shared Function GetAccessorBlockBeginLocation(ByVal accessor As SourcePropertyAccessorSymbol) As Location
			Return accessor.SyntaxTree.GetLocation(DirectCast(accessor.BlockSyntax, AccessorBlockSyntax).BlockStatement.Span)
		End Function

		Friend Function GetAccessorImplementations(ByVal getter As Boolean) As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			Dim immutableAndFree As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			Dim explicitInterfaceImplementations As ImmutableArray(Of PropertySymbol) = Me.ExplicitInterfaceImplementations
			If (Not explicitInterfaceImplementations.IsEmpty) Then
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).GetInstance()
				Dim enumerator As ImmutableArray(Of PropertySymbol).Enumerator = explicitInterfaceImplementations.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As PropertySymbol = enumerator.Current
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = If(getter, current.GetMethod, current.SetMethod)
					If (methodSymbol Is Nothing OrElse Not methodSymbol.RequiresImplementation()) Then
						Continue While
					End If
					instance.Add(methodSymbol)
				End While
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).Empty
			End If
			Return immutableAndFree
		End Function

		Private Function GetAttributeDeclarations() As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
			Dim oneOrMany As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
			If (Not Me.IsWithEvents) Then
				Dim syntaxReference As Microsoft.CodeAnalysis.SyntaxReference = Me._syntaxRef
				Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
				oneOrMany = Roslyn.Utilities.OneOrMany.Create(Of SyntaxList(Of AttributeListSyntax))(DirectCast(syntaxReference.GetSyntax(cancellationToken), PropertyStatementSyntax).AttributeLists)
			Else
				oneOrMany = New OneOrMany(Of SyntaxList(Of AttributeListSyntax))()
			End If
			Return oneOrMany
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.GetAttributesBag().Attributes
		End Function

		Friend Function GetAttributesBag() As CustomAttributesBag(Of VisualBasicAttributeData)
			If (Me._lazyCustomAttributesBag Is Nothing OrElse Not Me._lazyCustomAttributesBag.IsSealed) Then
				MyBase.LoadAndValidateAttributes(Me.GetAttributeDeclarations(), Me._lazyCustomAttributesBag, AttributeLocation.None)
			End If
			Return Me._lazyCustomAttributesBag
		End Function

		Private Function GetDecodedReturnTypeWellKnownAttributeData() As CommonReturnTypeWellKnownAttributeData
			Dim returnTypeAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me._lazyReturnTypeCustomAttributesBag
			If (returnTypeAttributesBag Is Nothing OrElse Not returnTypeAttributesBag.IsDecodedWellKnownAttributeDataComputed) Then
				returnTypeAttributesBag = Me.GetReturnTypeAttributesBag()
			End If
			Return DirectCast(returnTypeAttributesBag.DecodedWellKnownAttributeData, CommonReturnTypeWellKnownAttributeData)
		End Function

		Private Function GetDecodedWellKnownAttributeData() As CommonPropertyWellKnownAttributeData
			Dim attributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me._lazyCustomAttributesBag
			If (attributesBag Is Nothing OrElse Not attributesBag.IsDecodedWellKnownAttributeDataComputed) Then
				attributesBag = Me.GetAttributesBag()
			End If
			Return DirectCast(attributesBag.DecodedWellKnownAttributeData, CommonPropertyWellKnownAttributeData)
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Dim str As String
			str = If(Not expandIncludes, SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(Me, preferredCulture, expandIncludes, Me._lazyDocComment, cancellationToken), SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(Me, preferredCulture, expandIncludes, Me._lazyExpandedDocComment, cancellationToken))
			Return str
		End Function

		Friend Function GetImplementingLocation(ByVal implementedProperty As PropertySymbol) As Microsoft.CodeAnalysis.Location
			Dim location As Microsoft.CodeAnalysis.Location
			Dim syntax As PropertyStatementSyntax = TryCast(Me._syntaxRef.GetSyntax(New CancellationToken()), PropertyStatementSyntax)
			If (syntax Is Nothing OrElse syntax.ImplementsClause Is Nothing) Then
				location = If(System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of Microsoft.CodeAnalysis.Location)(Me.Locations), NoLocation.Singleton)
			Else
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.CreateBinderForTypeDeclaration()
				location = ImplementsHelper.FindImplementingSyntax(Of PropertySymbol)(syntax.ImplementsClause, Me, implementedProperty, Me._containingType, binder).GetLocation()
			End If
			Return location
		End Function

		Friend Overrides Function GetLexicalSortKey() As LexicalSortKey
			Return New LexicalSortKey(Me._location, Me.DeclaringCompilation)
		End Function

		Private Function GetReturnTypeAttributeDeclarations() As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
			Dim oneOrMany As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
			If (Not Me.IsWithEvents) Then
				Dim asClause As AsClauseSyntax = DirectCast(Me.Syntax, PropertyStatementSyntax).AsClause
				oneOrMany = If(asClause IsNot Nothing, Roslyn.Utilities.OneOrMany.Create(Of SyntaxList(Of AttributeListSyntax))(asClause.Attributes()), New OneOrMany(Of SyntaxList(Of AttributeListSyntax))())
			Else
				oneOrMany = New OneOrMany(Of SyntaxList(Of AttributeListSyntax))()
			End If
			Return oneOrMany
		End Function

		Friend Function GetReturnTypeAttributesBag() As CustomAttributesBag(Of VisualBasicAttributeData)
			If (Me._lazyReturnTypeCustomAttributesBag Is Nothing OrElse Not Me._lazyReturnTypeCustomAttributesBag.IsSealed) Then
				MyBase.LoadAndValidateAttributes(Me.GetReturnTypeAttributeDeclarations(), Me._lazyReturnTypeCustomAttributesBag, AttributeLocation.[Return])
			End If
			Return Me._lazyReturnTypeCustomAttributesBag
		End Function

		Private Shared Function HasRequiredParameters(ByVal parameters As ImmutableArray(Of ParameterSymbol)) As Boolean
			Dim flag As Boolean
			Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = parameters.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As ParameterSymbol = enumerator.Current
					If (Not current.IsOptional AndAlso Not current.IsParamArray) Then
						flag = True
						Exit While
					End If
				Else
					flag = False
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Shared Function IsAccessibilityMoreRestrictive(ByVal [property] As Accessibility, ByVal accessor As Accessibility) As Boolean
			Dim flag As Boolean
			Dim flag1 As Boolean
			If (accessor <> Accessibility.NotApplicable) Then
				If (accessor >= [property]) Then
					flag1 = False
				Else
					flag1 = If(accessor <> Accessibility.[Protected], True, [property] <> Accessibility.Internal)
				End If
				flag = flag1
			Else
				flag = True
			End If
			Return flag
		End Function

		Friend Overrides Function IsDefinedInSourceTree(ByVal tree As Microsoft.CodeAnalysis.SyntaxTree, ByVal definedWithinSpan As Nullable(Of TextSpan), Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Boolean
			Dim syntax As VisualBasicSyntaxNode = Me.Syntax
			If (syntax Is Nothing) Then
				Return False
			End If
			Return Symbol.IsDefinedInSourceTree(syntax.Parent, tree, definedWithinSpan, cancellationToken)
		End Function

		Private Shared Sub ReportAccessorAccessibilityError(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal syntax As AccessorBlockSyntax, ByVal errorId As ERRID, ByVal diagnostics As DiagnosticBag)
			binder.ReportModifierError(syntax.BlockStatement.Modifiers, errorId, diagnostics, SourcePropertySymbol.s_accessibilityModifierKinds)
		End Sub

		Private Sub SetCustomAttributeData(ByVal attributeData As CustomAttributesBag(Of VisualBasicAttributeData))
			Me._lazyCustomAttributesBag = attributeData
		End Sub

		Friend Overrides Sub SetMetadataName(ByVal metadataName As String)
			Interlocked.CompareExchange(Of String)(Me._lazyMetadataName, metadataName, Nothing)
		End Sub

		<Flags>
		Private Enum StateFlags
			SymbolDeclaredEvent = 1
		End Enum
	End Class
End Namespace