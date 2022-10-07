Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Linq
Imports System.Reflection
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SourceMethodSymbol
		Inherits MethodSymbol
		Implements IAttributeTargetSymbol
		Protected ReadOnly m_flags As SourceMemberFlags

		Protected ReadOnly m_containingType As NamedTypeSymbol

		Private _lazyMeParameter As ParameterSymbol

		Protected m_lazyCustomAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData)

		Protected m_lazyReturnTypeCustomAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData)

		Protected ReadOnly m_syntaxReferenceOpt As SyntaxReference

		Private _lazyLocations As ImmutableArray(Of Location)

		Private _lazyDocComment As String

		Private _lazyExpandedDocComment As String

		Private _cachedDiagnostics As ImmutableArray(Of Diagnostic)

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return Me.TypeParameters.Length
			End Get
		End Property

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return Nothing
			End Get
		End Property

		Protected ReadOnly Property AttributeDeclarationSyntaxList As SyntaxList(Of AttributeListSyntax)
			Get
				If (Me.m_syntaxReferenceOpt Is Nothing) Then
					Return New SyntaxList(Of AttributeListSyntax)()
				End If
				Return Me.DeclarationSyntax.AttributeLists
			End Get
		End Property

		Friend ReadOnly Property BlockSyntax As MethodBlockBaseSyntax
			Get
				Dim parent As MethodBlockBaseSyntax
				If (Me.m_syntaxReferenceOpt IsNot Nothing) Then
					parent = TryCast(Me.m_syntaxReferenceOpt.GetSyntax(New CancellationToken()).Parent, MethodBlockBaseSyntax)
				Else
					parent = Nothing
				End If
				Return parent
			End Get
		End Property

		Protected Overridable ReadOnly Property BoundAttributesSource As SourceMethodSymbol
			Get
				Return Nothing
			End Get
		End Property

		Protected Overridable ReadOnly Property BoundReturnTypeAttributesSource As SourcePropertySymbol
			Get
				Return Nothing
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
			Get
				Return DirectCast((If(Me.IsShared, 0, 32) Or If(Me.IsGenericMethod, 16, 0)), Microsoft.Cci.CallingConvention)
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ConstructedFrom As MethodSymbol
			Get
				Return Me
			End Get
		End Property

		Public ReadOnly Property ContainingSourceModule As SourceModuleSymbol
			Get
				Return DirectCast(Me.ContainingModule, SourceModuleSymbol)
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me.m_containingType
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return Me.m_containingType
			End Get
		End Property

		Friend ReadOnly Property DeclarationSyntax As MethodBaseSyntax
			Get
				If (Me.m_syntaxReferenceOpt Is Nothing) Then
					Return Nothing
				End If
				Return DirectCast(Me.m_syntaxReferenceOpt.GetSyntax(New CancellationToken()), MethodBaseSyntax)
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return DirectCast((Me.m_flags And SourceMemberFlags.AccessibilityMask), Accessibility)
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Symbol.GetDeclaringSyntaxReferenceHelper(Me.m_syntaxReferenceOpt)
			End Get
		End Property

		Public ReadOnly Property DefaultAttributeLocation As AttributeLocation Implements IAttributeTargetSymbol.DefaultAttributeLocation
			Get
				Return AttributeLocation.Method
			End Get
		End Property

		Friend ReadOnly Property Diagnostics As ImmutableArray(Of Diagnostic)
			Get
				Return Me._cachedDiagnostics
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
			Get
				Return ImmutableArray(Of MethodSymbol).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return True
			End Get
		End Property

		Friend ReadOnly Property HandlesEvents As Boolean
			Get
				Return (Me.m_flags And SourceMemberFlags.[Const]) <> SourceMemberFlags.None
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
			Get
				Dim decodedWellKnownAttributeData As MethodWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return decodedWellKnownAttributeData.HasDeclarativeSecurity
			End Get
		End Property

		Friend Overridable ReadOnly Property HasEmptyBody As Boolean
			Get
				Dim blockSyntax As MethodBlockBaseSyntax = Me.BlockSyntax
				If (blockSyntax Is Nothing) Then
					Return True
				End If
				Return Not blockSyntax.Statements.Any()
			End Get
		End Property

		Friend Overrides ReadOnly Property HasRuntimeSpecialName As Boolean
			Get
				If (MyBase.HasRuntimeSpecialName) Then
					Return True
				End If
				Return Me.IsVtableGapInterfaceMethod()
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Dim flag As Boolean
				Select Case Me.MethodKind
					Case Microsoft.CodeAnalysis.MethodKind.Constructor
					Case Microsoft.CodeAnalysis.MethodKind.Conversion
					Case Microsoft.CodeAnalysis.MethodKind.EventAdd
					Case Microsoft.CodeAnalysis.MethodKind.EventRaise
					Case Microsoft.CodeAnalysis.MethodKind.EventRemove
					Case Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator
					Case Microsoft.CodeAnalysis.MethodKind.PropertyGet
					Case Microsoft.CodeAnalysis.MethodKind.PropertySet
					Case Microsoft.CodeAnalysis.MethodKind.StaticConstructor
						flag = True
						Exit Select
					Case Microsoft.CodeAnalysis.MethodKind.DelegateInvoke
					Case Microsoft.CodeAnalysis.MethodKind.Destructor
					Case Microsoft.CodeAnalysis.MethodKind.ExplicitInterfaceImplementation
					Case Microsoft.CodeAnalysis.MethodKind.Ordinary
					Case Microsoft.CodeAnalysis.MethodKind.ReducedExtension
					Label0:
						If (Not Me.IsVtableGapInterfaceMethod()) Then
							Dim decodedWellKnownAttributeData As MethodWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
							flag = If(decodedWellKnownAttributeData Is Nothing, False, decodedWellKnownAttributeData.HasSpecialNameAttribute)
							Exit Select
						Else
							flag = True
							Exit Select
						End If
					Case Else
						GoTo Label0
				End Select
				Return flag
			End Get
		End Property

		Private ReadOnly Property HasSTAThreadOrMTAThreadAttribute As Boolean
			Get
				Dim decodedWellKnownAttributeData As MethodWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				If (decodedWellKnownAttributeData.HasSTAThreadAttribute) Then
					Return True
				End If
				Return decodedWellKnownAttributeData.HasMTAThreadAttribute
			End Get
		End Property

		Friend Overrides ReadOnly Property ImplementationAttributes As MethodImplAttributes
			Get
				Dim methodImplAttribute As MethodImplAttributes
				If (Not Me.ContainingType.IsComImport OrElse Me.ContainingType.IsInterface) Then
					Dim decodedWellKnownAttributeData As MethodWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
					methodImplAttribute = If(decodedWellKnownAttributeData IsNot Nothing, decodedWellKnownAttributeData.MethodImplAttributes, MethodImplAttributes.IL)
				Else
					methodImplAttribute = MethodImplAttributes.CodeTypeMask Or MethodImplAttributes.Native Or MethodImplAttributes.OPTIL Or MethodImplAttributes.Runtime Or MethodImplAttributes.InternalCall
				End If
				Return methodImplAttribute
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsAsync As Boolean
			Get
				Return (Me.m_flags And SourceMemberFlags.Async) <> SourceMemberFlags.None
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsDirectlyExcludedFromCodeCoverage As Boolean
			Get
				Dim decodedWellKnownAttributeData As MethodWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return decodedWellKnownAttributeData.HasExcludeFromCodeCoverageAttribute
			End Get
		End Property

		Public Overrides ReadOnly Property IsExtensionMethod As Boolean
			Get
				Dim earlyDecodedWellKnownAttributeData As MethodEarlyWellKnownAttributeData = Me.GetEarlyDecodedWellKnownAttributeData()
				If (earlyDecodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return earlyDecodedWellKnownAttributeData.IsExtensionMethod
			End Get
		End Property

		Public Overrides ReadOnly Property IsExternalMethod As Boolean
			Get
				' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol::get_IsExternalMethod()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean get_IsExternalMethod()
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

		Public NotOverridable Overrides ReadOnly Property IsGenericMethod As Boolean
			Get
				Return Me.Arity <> 0
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me.m_containingType.AreMembersImplicitlyDeclared
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsInitOnly As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsIterator As Boolean
			Get
				Return (Me.m_flags And SourceMemberFlags.Iterator) <> SourceMemberFlags.None
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsMethodKindBasedOnSyntax As Boolean
			Get
				Return True
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return (Me.m_flags And SourceMemberFlags.[MustOverride]) <> SourceMemberFlags.None
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return (Me.m_flags And SourceMemberFlags.[NotOverridable]) <> SourceMemberFlags.None
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsOverloads As Boolean
			Get
				' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol::get_IsOverloads()
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

		Public NotOverridable Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return (Me.m_flags And SourceMemberFlags.[Overridable]) <> SourceMemberFlags.None
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return (Me.m_flags And SourceMemberFlags.[Overrides]) <> SourceMemberFlags.None
			End Get
		End Property

		Friend ReadOnly Property IsPartial As Boolean
			Get
				Return (Me.m_flags And SourceMemberFlags.[Partial]) <> SourceMemberFlags.None
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsShared As Boolean
			Get
				Return (Me.m_flags And SourceMemberFlags.[Shared]) <> SourceMemberFlags.None
			End Get
		End Property

		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				Return (Me.m_flags And SourceMemberFlags.[Dim]) <> SourceMemberFlags.None
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsVararg As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Dim empty As ImmutableArray(Of Location)
				If (Me._lazyLocations.IsDefault) Then
					Dim nonMergedLocation As Location = Me.NonMergedLocation
					If (nonMergedLocation Is Nothing) Then
						empty = ImmutableArray(Of Location).Empty
					Else
						empty = ImmutableArray.Create(Of Location)(nonMergedLocation)
					End If
					Dim locations1 As ImmutableArray(Of Location) = New ImmutableArray(Of Location)()
					ImmutableInterlocked.InterlockedCompareExchange(Of Location)(Me._lazyLocations, empty, locations1)
				End If
				Return Me._lazyLocations
			End Get
		End Property

		Friend Overrides ReadOnly Property MayBeReducibleExtensionMethod As Boolean

		Public Overrides ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind
			Get
				Return Me.m_flags.ToMethodKind()
			End Get
		End Property

		Friend ReadOnly Property NonMergedLocation As Location
			Get
				If (Me.m_syntaxReferenceOpt Is Nothing) Then
					Return Nothing
				End If
				Return Me.GetSymbolLocation(Me.m_syntaxReferenceOpt)
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Dim uninitialized As Microsoft.CodeAnalysis.ObsoleteAttributeData
				Dim obsoleteAttributeDatum As Microsoft.CodeAnalysis.ObsoleteAttributeData
				Dim mContainingType As SourceMemberContainerTypeSymbol = TryCast(Me.m_containingType, SourceMemberContainerTypeSymbol)
				If (mContainingType Is Nothing OrElse Not mContainingType.AnyMemberHasAttributes) Then
					uninitialized = Nothing
				Else
					Dim mLazyCustomAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me.m_lazyCustomAttributesBag
					If (mLazyCustomAttributesBag IsNot Nothing AndAlso mLazyCustomAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed) Then
						Dim earlyDecodedWellKnownAttributeData As MethodEarlyWellKnownAttributeData = DirectCast(Me.m_lazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData, MethodEarlyWellKnownAttributeData)
						If (earlyDecodedWellKnownAttributeData IsNot Nothing) Then
							obsoleteAttributeDatum = earlyDecodedWellKnownAttributeData.ObsoleteAttributeData
						Else
							obsoleteAttributeDatum = Nothing
						End If
						uninitialized = obsoleteAttributeDatum
					ElseIf (Not Me.DeclaringSyntaxReferences.IsEmpty) Then
						uninitialized = Microsoft.CodeAnalysis.ObsoleteAttributeData.Uninitialized
					Else
						uninitialized = Nothing
					End If
				End If
				Return uninitialized
			End Get
		End Property

		Friend ReadOnly Property OverloadsExplicitly As Boolean
			Get
				Return (Me.m_flags And SourceMemberFlags.[Overloads]) <> SourceMemberFlags.None
			End Get
		End Property

		Friend Overrides ReadOnly Property OverriddenMembers As OverriddenMembersResult(Of MethodSymbol)

		Friend ReadOnly Property OverridesExplicitly As Boolean
			Get
				Return (Me.m_flags And SourceMemberFlags.[Overrides]) <> SourceMemberFlags.None
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)

		Public NotOverridable Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ReturnsByRef As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol

		Protected ReadOnly Property ReturnTypeAttributeDeclarationSyntaxList As SyntaxList(Of AttributeListSyntax)
			Get
				Dim attributeListSyntaxes As SyntaxList(Of AttributeListSyntax)
				Dim declarationSyntax As MethodBaseSyntax = Me.DeclarationSyntax
				If (declarationSyntax IsNot Nothing) Then
					Dim asClauseInternal As AsClauseSyntax = declarationSyntax.AsClauseInternal
					If (asClauseInternal Is Nothing) Then
						attributeListSyntaxes = New SyntaxList(Of AttributeListSyntax)()
						Return attributeListSyntaxes
					End If
					attributeListSyntaxes = asClauseInternal.Attributes()
					Return attributeListSyntaxes
				End If
				attributeListSyntaxes = New SyntaxList(Of AttributeListSyntax)()
				Return attributeListSyntaxes
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnTypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Dim empty As ImmutableArray(Of CustomModifier)
				Dim overriddenMethod As MethodSymbol = Me.OverriddenMethod
				If (overriddenMethod IsNot Nothing) Then
					empty = overriddenMethod.ConstructIfGeneric(Me.TypeArguments).ReturnTypeCustomModifiers
				Else
					empty = ImmutableArray(Of CustomModifier).Empty
				End If
				Return empty
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ReturnTypeMarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Dim decodedReturnTypeWellKnownAttributeData As CommonReturnTypeWellKnownAttributeData = Me.GetDecodedReturnTypeWellKnownAttributeData()
				If (decodedReturnTypeWellKnownAttributeData Is Nothing) Then
					Return Nothing
				End If
				Return decodedReturnTypeWellKnownAttributeData.MarshallingInformation
			End Get
		End Property

		Friend Overrides ReadOnly Property ShadowsExplicitly As Boolean
			Get
				Return (Me.m_flags And SourceMemberFlags.[Shadows]) <> SourceMemberFlags.None
			End Get
		End Property

		Friend Overrides ReadOnly Property Syntax As SyntaxNode
			Get
				Dim visualBasicSyntax As SyntaxNode
				If (Me.m_syntaxReferenceOpt IsNot Nothing) Then
					Dim blockSyntax As MethodBlockBaseSyntax = Me.BlockSyntax
					If (blockSyntax Is Nothing) Then
						visualBasicSyntax = Me.m_syntaxReferenceOpt.GetVisualBasicSyntax(New CancellationToken())
					Else
						visualBasicSyntax = blockSyntax
					End If
				Else
					visualBasicSyntax = Nothing
				End If
				Return visualBasicSyntax
			End Get
		End Property

		Public ReadOnly Property SyntaxTree As Microsoft.CodeAnalysis.SyntaxTree
			Get
				Dim syntaxTree1 As Microsoft.CodeAnalysis.SyntaxTree
				If (Me.m_syntaxReferenceOpt Is Nothing) Then
					syntaxTree1 = Nothing
				Else
					syntaxTree1 = Me.m_syntaxReferenceOpt.SyntaxTree
				End If
				Return syntaxTree1
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property TypeArguments As ImmutableArray(Of TypeSymbol)
			Get
				Return StaticCast(Of TypeSymbol).From(Of TypeParameterSymbol)(Me.TypeParameters)
			End Get
		End Property

		Protected Sub New(ByVal containingType As NamedTypeSymbol, ByVal flags As SourceMemberFlags, ByVal syntaxRef As SyntaxReference, Optional ByVal locations As ImmutableArray(Of Location) = Nothing)
			MyBase.New()
			Me.m_containingType = containingType
			Me.m_flags = flags
			Me.m_syntaxReferenceOpt = syntaxRef
			Me._lazyLocations = locations
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
			If (Me = declaringCompilation.GetEntryPoint(CancellationToken.None) AndAlso Not Me.HasSTAThreadOrMTAThreadAttribute) Then
				Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
				Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
				Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_STAThreadAttribute__ctor, typedConstants, keyValuePairs, False))
			End If
		End Sub

		Friend Overrides Sub AddSynthesizedReturnTypeAttributes(ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedReturnTypeAttributes(attributes)
			If (Me.ReturnType.ContainsTupleNames()) Then
				Symbol.AddSynthesizedAttribute(attributes, Me.DeclaringCompilation.SynthesizeTupleNamesAttribute(Me.ReturnType))
			End If
		End Sub

		Friend Function BindTypeParameterConstraints(ByVal syntax As TypeParameterSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of TypeParameterConstraint)
			Dim locationSpecificBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForType(Me.ContainingSourceModule, Me.SyntaxTree, Me.m_containingType)
			locationSpecificBinder = BinderBuilder.CreateBinderForGenericMethodDeclaration(Me, locationSpecificBinder)
			If (syntax.VarianceKeyword.Kind() <> SyntaxKind.None) Then
				Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, syntax.VarianceKeyword, ERRID.ERR_VarianceDisallowedHere)
			End If
			locationSpecificBinder = New Microsoft.CodeAnalysis.VisualBasic.LocationSpecificBinder(BindingLocation.GenericConstraintsClause, Me, locationSpecificBinder)
			Return locationSpecificBinder.BindTypeParameterConstraintClause(Me, syntax.TypeParameterConstraintClause, diagnostics)
		End Function

		Friend NotOverridable Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As Microsoft.CodeAnalysis.SyntaxTree) As Integer
			Dim num As Integer
			Dim num1 As Integer = 0
			Dim blockSyntax As MethodBlockBaseSyntax = Me.BlockSyntax
			If (blockSyntax IsNot Nothing AndAlso localTree = blockSyntax.SyntaxTree) Then
				If (localPosition <> blockSyntax.BlockStatement.SpanStart) Then
					Dim span As TextSpan = blockSyntax.Statements.Span
					If (Not span.Contains(localPosition)) Then
						If (Not DirectCast(Me.ContainingType, SourceNamedTypeSymbol).TryCalculateSyntaxOffsetOfPositionInInitializer(localPosition, localTree, Me.IsShared, num1)) Then
							Throw ExceptionUtilities.Unreachable
						End If
						num = num1
						Return num
					End If
					num = localPosition - span.Start
					Return num
				Else
					num = -1
					Return num
				End If
			End If
			If (Not DirectCast(Me.ContainingType, SourceNamedTypeSymbol).TryCalculateSyntaxOffsetOfPositionInInitializer(localPosition, localTree, Me.IsShared, num1)) Then
				Throw ExceptionUtilities.Unreachable
			End If
			num = num1
			Return num
		End Function

		Friend Shared Function CreateConstructor(ByVal container As SourceMemberContainerTypeSymbol, ByVal syntax As SubNewStatementSyntax, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As DiagnosticBag) As SourceMethodSymbol
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol::CreateConstructor(Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.DiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol CreateConstructor(Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.DiagnosticBag)
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

		Friend Shared Function CreateDeclareMethod(ByVal container As SourceMemberContainerTypeSymbol, ByVal syntax As DeclareStatementSyntax, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As DiagnosticBag) As SourceMethodSymbol
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol::CreateDeclareMethod(Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.DiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol CreateDeclareMethod(Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.DiagnosticBag)
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

		Friend Shared Function CreateOperator(ByVal container As SourceMemberContainerTypeSymbol, ByVal syntax As OperatorStatementSyntax, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As DiagnosticBag) As SourceMethodSymbol
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID
			Dim allFlags As SourceMemberFlags = SourceMethodSymbol.DecodeOperatorModifiers(syntax, binder, diagBag).AllFlags
			Dim memberNameFromSyntax As String = SourceMethodSymbol.GetMemberNameFromSyntax(syntax)
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = syntax.OperatorToken.Kind()
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword) Then
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeKeyword) Then
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LikeKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModKeyword) Then
						GoTo Label3
					End If
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword) Then
						GoTo Label0
					End If
					Throw ExceptionUtilities.UnexpectedValue(syntax.OperatorToken.Kind())
				Else
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword) Then
						GoTo Label3
					End If
					If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeKeyword) Then
						GoTo Label0
					End If
					Throw ExceptionUtilities.UnexpectedValue(syntax.OperatorToken.Kind())
				End If
			ElseIf (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword) Then
				If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
					GoTo Label0
				End If
				Select Case syntaxKind
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsteriskToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanEqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanGreaterThanToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanEqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BackslashToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaretToken
						Exit Select
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSyncLockStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterMultipleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeywordEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsPropertyEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClauseItem Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IncompleteMember Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableDeclarator Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleAsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsNewClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectMemberInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCollectionInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferredFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionalKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverloadsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReadOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.REMKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShadowsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StaticKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThrowKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ToKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryCastKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UIntegerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ULongKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GosubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariantKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WendKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AllKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnsiKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AssemblyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AutoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BinaryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompareKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CustomKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DescendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DisableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DistinctKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExplicitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalSourceKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalChecksumKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FromKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OffKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrderKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OutKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PreserveKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StrictKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TextKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnicodeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UntilKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WarningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhereKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsyncKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AwaitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IteratorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.YieldKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclamationToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AtToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HashToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseParenToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SemicolonToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GoToStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
						Throw ExceptionUtilities.UnexpectedValue(syntax.OperatorToken.Kind())
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken
					Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusToken
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_OneOrTwoParametersRequired1
						GoTo Label1
					Case Else
						If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanLessThanToken) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
							Exit Select
						End If
						Throw ExceptionUtilities.UnexpectedValue(syntax.OperatorToken.Kind())
				End Select
			Else
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword) Then
					GoTo Label3
				End If
				Throw ExceptionUtilities.UnexpectedValue(syntax.OperatorToken.Kind())
			End If
		Label3:
			eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_TwoParametersRequired1
		Label1:
			Select Case eRRID
				Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_OneParameterRequired1
					If (syntax.ParameterList.Parameters.Count <> 1) Then
						Exit Select
					End If
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_TwoParametersRequired1
					If (syntax.ParameterList.Parameters.Count <> 2) Then
						Exit Select
					End If
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_OneOrTwoParametersRequired1
					If (syntax.ParameterList.Parameters.Count <> 1 AndAlso 2 <> syntax.ParameterList.Parameters.Count) Then
						Exit Select
					End If
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(eRRID)
			End Select
			If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
				Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, syntax.OperatorToken, eRRID, New [Object]() { SyntaxFacts.GetText(syntax.OperatorToken.Kind()) })
			End If
			allFlags = allFlags Or If(syntax.OperatorToken.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeKeyword, SourceMemberFlags.MethodKindConversion, SourceMemberFlags.MethodKindOperator)
			Dim handledEvents As ImmutableArray(Of HandledEvent) = New ImmutableArray(Of HandledEvent)()
			Return New SourceMemberMethodSymbol(container, memberNameFromSyntax, allFlags, binder, syntax, 0, handledEvents)
		Label0:
			eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_OneParameterRequired1
			GoTo Label1
		End Function

		Friend Shared Function CreateRegularMethod(ByVal container As SourceMemberContainerTypeSymbol, ByVal syntax As MethodStatementSyntax, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As DiagnosticBag) As SourceMethodSymbol
			Dim empty As ImmutableArray(Of HandledEvent)
			Dim memberModifier As MemberModifiers = SourceMethodSymbol.DecodeMethodModifiers(syntax.Modifiers, container, binder, diagBag)
			Dim allFlags As SourceMemberFlags = memberModifier.AllFlags Or SourceMemberFlags.MethodKindOrdinary
			If (syntax.Kind() = SyntaxKind.SubStatement) Then
				allFlags = allFlags Or SourceMemberFlags.[Dim]
			End If
			If (syntax.HandlesClause IsNot Nothing) Then
				allFlags = allFlags Or SourceMemberFlags.[Const]
			End If
			Dim valueText As String = syntax.Identifier.ValueText
			If (syntax.HandlesClause Is Nothing) Then
				empty = ImmutableArray(Of HandledEvent).Empty
			Else
				If (container.TypeKind = Microsoft.CodeAnalysis.TypeKind.Struct) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, syntax.Identifier, ERRID.ERR_StructsCannotHandleEvents)
				ElseIf (container.IsInterface) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, syntax.HandlesClause, ERRID.ERR_BadInterfaceMethodFlags1, New [Object]() { syntax.HandlesClause.HandlesKeyword.ToString() })
				ElseIf (SourceMethodSymbol.GetTypeParameterListSyntax(syntax) IsNot Nothing) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, syntax.Identifier, ERRID.ERR_HandlesInvalidOnGenericMethod)
				End If
				empty = New ImmutableArray(Of HandledEvent)()
			End If
			Dim num As Integer = If(syntax.TypeParameterList Is Nothing, 0, syntax.TypeParameterList.Parameters.Count)
			Dim sourceMemberMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol(container, valueText, allFlags, binder, syntax, num, empty)
			If (sourceMemberMethodSymbol.IsPartial AndAlso sourceMemberMethodSymbol.IsSub) Then
				If (sourceMemberMethodSymbol.IsAsync) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, syntax.Identifier, ERRID.ERR_PartialMethodsMustNotBeAsync1, New [Object]() { valueText })
				End If
				SourceMethodSymbol.ReportPartialMethodErrors(syntax.Modifiers, binder, diagBag)
			End If
			Return sourceMemberMethodSymbol
		End Function

		Friend Shared Function DecodeConstructorModifiers(ByVal modifiers As SyntaxTokenList, ByVal container As SourceMemberContainerTypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As DiagnosticBag) As MemberModifiers
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.MemberModifiers Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol::DecodeConstructorModifiers(Microsoft.CodeAnalysis.SyntaxTokenList,Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.DiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.MemberModifiers DecodeConstructorModifiers(Microsoft.CodeAnalysis.SyntaxTokenList,Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberContainerTypeSymbol,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.DiagnosticBag)
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

		Private Shared Function DecodeMethodModifiers(ByVal modifiers As SyntaxTokenList, ByVal container As SourceMemberContainerTypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As DiagnosticBag) As MemberModifiers
			Dim memberModifier As MemberModifiers = binder.DecodeModifiers(modifiers, SourceMemberFlags.[Private] Or SourceMemberFlags.[Protected] Or SourceMemberFlags.[Friend] Or SourceMemberFlags.[Public] Or SourceMemberFlags.AllAccessibilityModifiers Or SourceMemberFlags.[Shared] Or SourceMemberFlags.[Overrides] Or SourceMemberFlags.[Overridable] Or SourceMemberFlags.[MustOverride] Or SourceMemberFlags.[NotOverridable] Or SourceMemberFlags.AllOverrideModifiers Or SourceMemberFlags.PrivateOverridableModifiers Or SourceMemberFlags.PrivateMustOverrideModifiers Or SourceMemberFlags.PrivateNotOverridableModifiers Or SourceMemberFlags.[Overloads] Or SourceMemberFlags.[Shadows] Or SourceMemberFlags.AllShadowingModifiers Or SourceMemberFlags.ShadowsAndOverrides Or SourceMemberFlags.InvalidInNotInheritableClass Or SourceMemberFlags.InvalidInNotInheritableOtherPartialClass Or SourceMemberFlags.InvalidIfDefault Or SourceMemberFlags.[Partial] Or SourceMemberFlags.Async Or SourceMemberFlags.Iterator, ERRID.ERR_BadMethodFlags1, Accessibility.[Public], diagBag)
			memberModifier = binder.ValidateSharedPropertyAndMethodModifiers(modifiers, memberModifier, False, container, diagBag)
			If ((memberModifier.FoundFlags And (SourceMemberFlags.Async Or SourceMemberFlags.Iterator)) = (SourceMemberFlags.Async Or SourceMemberFlags.Iterator)) Then
				binder.ReportModifierError(modifiers, ERRID.ERR_InvalidAsyncIteratorModifiers, diagBag, InvalidModifiers.InvalidAsyncIterator)
			End If
			Return memberModifier
		End Function

		Private Shared Function DecodeOperatorModifiers(ByVal syntax As OperatorStatementSyntax, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As DiagnosticBag) As MemberModifiers
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.MemberModifiers Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol::DecodeOperatorModifiers(Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.DiagnosticBag)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.MemberModifiers DecodeOperatorModifiers(Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax,Microsoft.CodeAnalysis.VisualBasic.Binder,Microsoft.CodeAnalysis.DiagnosticBag)
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
			If (attribute.IsTargetAttribute(Me, AttributeDescription.TupleElementNamesAttribute)) Then
				DirectCast(arguments.Diagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag).Add(ERRID.ERR_ExplicitTupleElementNamesAttribute, arguments.AttributeSyntaxOpt.Location)
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.UnmanagedCallersOnlyAttribute)) Then
				DirectCast(arguments.Diagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag).Add(ERRID.ERR_UnmanagedCallersOnlyNotSupported, arguments.AttributeSyntaxOpt.Location)
			End If
			If (arguments.SymbolPart <> AttributeLocation.[Return]) Then
				Me.DecodeWellKnownAttributeAppliedToMethod(arguments)
			Else
				Me.DecodeWellKnownAttributeAppliedToReturnValue(arguments)
			End If
			MyBase.DecodeWellKnownAttribute(arguments)
		End Sub

		Private Sub DecodeWellKnownAttributeAppliedToMethod(ByRef arguments As DecodeWellKnownAttributeArguments(Of AttributeSyntax, VisualBasicAttributeData, AttributeLocation))
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol::DecodeWellKnownAttributeAppliedToMethod(Microsoft.CodeAnalysis.DecodeWellKnownAttributeArguments`3<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax,Microsoft.CodeAnalysis.VisualBasic.Symbols.VisualBasicAttributeData,Microsoft.CodeAnalysis.VisualBasic.Symbols.AttributeLocation>&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void DecodeWellKnownAttributeAppliedToMethod(Microsoft.CodeAnalysis.DecodeWellKnownAttributeArguments<Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax,Microsoft.CodeAnalysis.VisualBasic.Symbols.VisualBasicAttributeData,Microsoft.CodeAnalysis.VisualBasic.Symbols.AttributeLocation>&)
			' 
			' L'index était hors limites. Il ne doit pas être négatif et doit être inférieur à la taille de la collection.
			' Nom du paramètre : index
			'    à System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
			'    à ..(Int32 ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:ligne 364
			'    à ..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:ligne 74
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:ligne 55
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Sub

		Private Sub DecodeWellKnownAttributeAppliedToReturnValue(ByRef arguments As DecodeWellKnownAttributeArguments(Of AttributeSyntax, VisualBasicAttributeData, AttributeLocation))
			If (arguments.Attribute.IsTargetAttribute(Me, AttributeDescription.MarshalAsAttribute)) Then
				MarshalAsAttributeDecoder(Of CommonReturnTypeWellKnownAttributeData, AttributeSyntax, VisualBasicAttributeData, AttributeLocation).Decode(arguments, AttributeTargets.ReturnValue, MessageProvider.Instance)
			End If
		End Sub

		Friend Overrides Function EarlyDecodeWellKnownAttribute(ByRef arguments As EarlyDecodeWellKnownAttributeArguments(Of EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation)) As VisualBasicAttributeData
			Dim visualBasicAttributeDatum As VisualBasicAttributeData
			Dim visualBasicAttributeDatum1 As VisualBasicAttributeData
			Dim visualBasicAttributeDatum2 As VisualBasicAttributeData
			Dim flag As Boolean = False
			If (arguments.SymbolPart = AttributeLocation.[Return]) Then
				visualBasicAttributeDatum = MyBase.EarlyDecodeWellKnownAttribute(arguments)
				Return visualBasicAttributeDatum
			ElseIf (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.CaseInsensitiveExtensionAttribute)) Then
				Dim mightContainExtensionMethods As Boolean = False
				If ((Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.Ordinary OrElse Me.MethodKind = Microsoft.CodeAnalysis.MethodKind.DeclareMethod) AndAlso Me.m_containingType.AllowsExtensionMethods() AndAlso Me.ParameterCount <> 0) Then
					Dim item As ParameterSymbol = Me.Parameters(0)
					If (Not item.IsOptional AndAlso Not item.IsParamArray AndAlso MyBase.ValidateGenericConstraintsOnExtensionMethodDefinition()) Then
						mightContainExtensionMethods = Me.m_containingType.MightContainExtensionMethods
					End If
				End If
				If (mightContainExtensionMethods) Then
					Dim attribute As SourceAttributeData = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, flag)
					If (attribute.HasErrors) Then
						GoTo Label2
					End If
					arguments.GetOrCreateData(Of MethodEarlyWellKnownAttributeData)().IsExtensionMethod = True
					If (Not flag) Then
						visualBasicAttributeDatum2 = attribute
					Else
						visualBasicAttributeDatum2 = Nothing
					End If
					visualBasicAttributeDatum = visualBasicAttributeDatum2
					Return visualBasicAttributeDatum
				End If
			Label2:
				visualBasicAttributeDatum = Nothing
			ElseIf (Not VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.ConditionalAttribute)) Then
				Dim visualBasicAttributeDatum3 As VisualBasicAttributeData = Nothing
				Dim obsoleteAttributeDatum As Microsoft.CodeAnalysis.ObsoleteAttributeData = Nothing
				If (Not MyBase.EarlyDecodeDeprecatedOrExperimentalOrObsoleteAttribute(arguments, visualBasicAttributeDatum3, obsoleteAttributeDatum)) Then
					visualBasicAttributeDatum = MyBase.EarlyDecodeWellKnownAttribute(arguments)
					Return visualBasicAttributeDatum
				End If
				If (obsoleteAttributeDatum IsNot Nothing) Then
					arguments.GetOrCreateData(Of MethodEarlyWellKnownAttributeData)().ObsoleteAttributeData = obsoleteAttributeDatum
				End If
				visualBasicAttributeDatum = visualBasicAttributeDatum3
			Else
				Dim sourceAttributeDatum As SourceAttributeData = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, flag)
				If (sourceAttributeDatum.HasErrors) Then
					visualBasicAttributeDatum = Nothing
				Else
					Dim constructorArgument As String = sourceAttributeDatum.GetConstructorArgument(Of String)(0, SpecialType.System_String)
					arguments.GetOrCreateData(Of MethodEarlyWellKnownAttributeData)().AddConditionalSymbol(constructorArgument)
					If (Not flag) Then
						visualBasicAttributeDatum1 = sourceAttributeDatum
					Else
						visualBasicAttributeDatum1 = Nothing
					End If
					visualBasicAttributeDatum = visualBasicAttributeDatum1
				End If
			End If
			Return visualBasicAttributeDatum
		End Function

		Friend Shared Function FindSymbolFromSyntax(ByVal syntax As MethodBaseSyntax, ByVal tree As Microsoft.CodeAnalysis.SyntaxTree, ByVal container As NamedTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Select Case syntax.Kind()
				Case SyntaxKind.DelegateSubStatement
				Case SyntaxKind.DelegateFunctionStatement
					Dim identifier As Microsoft.CodeAnalysis.SyntaxToken = DirectCast(syntax, DelegateStatementSyntax).Identifier
					symbol = container.FindMember(identifier.ValueText, SymbolKind.NamedType, identifier.Span, tree)
					Exit Select
				Case 100
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement
				Case SyntaxKind.OperatorStatement
				Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.OptionStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement
				Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement
				Label1:
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(container.FindMember(SourceMethodSymbol.GetMemberNameFromSyntax(syntax), SymbolKind.Method, SourceMethodSymbol.GetMethodLocationFromSyntax(syntax), tree), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
					If (methodSymbol IsNot Nothing) Then
						Dim partialImplementationPart As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = methodSymbol.PartialImplementationPart
						If (partialImplementationPart IsNot Nothing AndAlso partialImplementationPart.Syntax = syntax.Parent) Then
							methodSymbol = partialImplementationPart
						End If
					End If
					symbol = methodSymbol
					Exit Select
				Case SyntaxKind.EventStatement
					Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = DirectCast(syntax, EventStatementSyntax).Identifier
					symbol = container.FindMember(syntaxToken.ValueText, SymbolKind.[Event], syntaxToken.Span, tree)
					Exit Select
				Case SyntaxKind.PropertyStatement
					Dim identifier1 As Microsoft.CodeAnalysis.SyntaxToken = DirectCast(syntax, PropertyStatementSyntax).Identifier
					symbol = container.FindMember(identifier1.ValueText, SymbolKind.[Property], identifier1.Span, tree)
					Exit Select
				Case SyntaxKind.GetAccessorStatement
				Case SyntaxKind.SetAccessorStatement
					Dim parent As PropertyBlockSyntax = TryCast(syntax.Parent.Parent, PropertyBlockSyntax)
					If (parent Is Nothing) Then
						symbol = Nothing
						Exit Select
					Else
						Dim syntaxToken1 As Microsoft.CodeAnalysis.SyntaxToken = parent.PropertyStatement.Identifier
						Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(container.FindMember(syntaxToken1.ValueText, SymbolKind.[Property], syntaxToken1.Span, tree), Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
						If (propertySymbol IsNot Nothing) Then
							Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = If(syntax.Kind() = SyntaxKind.GetAccessorStatement, propertySymbol.GetMethod, propertySymbol.SetMethod)
							If (methodSymbol1.Syntax <> syntax.Parent) Then
								symbol = Nothing
								Exit Select
							Else
								symbol = methodSymbol1
								Exit Select
							End If
						Else
							symbol = Nothing
							Exit Select
						End If
					End If
				Case SyntaxKind.AddHandlerAccessorStatement
				Case SyntaxKind.RemoveHandlerAccessorStatement
				Case SyntaxKind.RaiseEventAccessorStatement
					Dim eventBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax = TryCast(syntax.Parent.Parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax)
					If (eventBlockSyntax Is Nothing) Then
						symbol = Nothing
						Exit Select
					Else
						Dim identifier2 As Microsoft.CodeAnalysis.SyntaxToken = eventBlockSyntax.EventStatement.Identifier
						Dim eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol = DirectCast(container.FindMember(identifier2.ValueText, SymbolKind.[Event], identifier2.Span, tree), Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol)
						If (eventSymbol IsNot Nothing) Then
							Dim addMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
							Select Case syntax.Kind()
								Case SyntaxKind.AddHandlerAccessorStatement
									addMethod = eventSymbol.AddMethod
									GoTo Label0
								Case SyntaxKind.RemoveHandlerAccessorStatement
									addMethod = eventSymbol.RemoveMethod
									GoTo Label0
								Case SyntaxKind.List Or SyntaxKind.EndIfStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndStructureStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndModuleStatement Or SyntaxKind.OptionStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.XmlNamespaceImportsClause Or SyntaxKind.NewConstraint Or SyntaxKind.ClassConstraint Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DeclareFunctionStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.GetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement
								Case SyntaxKind.EmptyStatement Or SyntaxKind.EndUsingStatement Or SyntaxKind.EndSelectStatement Or SyntaxKind.EndEnumStatement Or SyntaxKind.EndClassStatement Or SyntaxKind.EndNamespaceStatement Or SyntaxKind.CompilationUnit Or SyntaxKind.ImportsStatement Or SyntaxKind.SimpleImportsClause Or SyntaxKind.TypeParameterList Or SyntaxKind.TypeParameterSingleConstraintClause Or SyntaxKind.NewConstraint Or SyntaxKind.StructureConstraint Or SyntaxKind.EnumMemberDeclaration Or SyntaxKind.DeclareSubStatement Or SyntaxKind.DelegateSubStatement Or SyntaxKind.EventStatement Or SyntaxKind.PropertyStatement Or SyntaxKind.SetAccessorStatement Or SyntaxKind.RemoveHandlerAccessorStatement
								Label0:
									If (addMethod Is Nothing OrElse addMethod.Syntax <> syntax.Parent) Then
										symbol = Nothing
									Else
										symbol = addMethod
									End If

								Case SyntaxKind.RaiseEventAccessorStatement
									addMethod = eventSymbol.RaiseMethod
									GoTo Label0
								Case Else
									GoTo Label0
							End Select
						Else
							symbol = Nothing
							Exit Select
						End If
					End If

				Case Else
					GoTo Label1
			End Select
			Return symbol
		End Function

		Friend Overrides Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
			MyBase.GenerateDeclarationErrors(cancellationToken)
			Dim returnType As TypeSymbol = Me.ReturnType
			Me.GetReturnTypeAttributes()
			Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = Me.Parameters.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As ParameterSymbol = enumerator.Current
				current.GetAttributes()
				If (Not current.HasExplicitDefaultValue) Then
					Continue While
				End If
				Dim explicitDefaultConstantValue As ConstantValue = current.ExplicitDefaultConstantValue
			End While
			Dim enumerator1 As ImmutableArray(Of TypeParameterSymbol).Enumerator = Me.TypeParameters.GetEnumerator()
			While enumerator1.MoveNext()
				Dim constraintTypesNoUseSiteDiagnostics As ImmutableArray(Of TypeSymbol) = enumerator1.Current.ConstraintTypesNoUseSiteDiagnostics
			End While
			Dim handledEvents As ImmutableArray(Of HandledEvent) = Me.HandledEvents
		End Sub

		Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			Dim earlyDecodedWellKnownAttributeData As MethodEarlyWellKnownAttributeData = Me.GetEarlyDecodedWellKnownAttributeData()
			If (earlyDecodedWellKnownAttributeData Is Nothing) Then
				Return ImmutableArray(Of String).Empty
			End If
			Return earlyDecodedWellKnownAttributeData.ConditionalSymbols
		End Function

		Protected Overridable Function GetAttributeDeclarations() As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
			Return OneOrMany.Create(Of SyntaxList(Of AttributeListSyntax))(Me.AttributeDeclarationSyntaxList)
		End Function

		Public NotOverridable Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.GetAttributesBag().Attributes
		End Function

		Private Function GetAttributesBag() As CustomAttributesBag(Of VisualBasicAttributeData)
			Return Me.GetAttributesBag(Me.m_lazyCustomAttributesBag, False)
		End Function

		Private Function GetAttributesBag(ByRef lazyCustomAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData), ByVal forReturnType As Boolean) As CustomAttributesBag(Of VisualBasicAttributeData)
			If (lazyCustomAttributesBag Is Nothing OrElse Not lazyCustomAttributesBag.IsSealed) Then
				If (Not forReturnType) Then
					Dim boundAttributesSource As SourceMethodSymbol = Me.BoundAttributesSource
					If (boundAttributesSource Is Nothing) Then
						MyBase.LoadAndValidateAttributes(Me.GetAttributeDeclarations(), lazyCustomAttributesBag, AttributeLocation.None)
					Else
						Interlocked.CompareExchange(Of CustomAttributesBag(Of VisualBasicAttributeData))(lazyCustomAttributesBag, boundAttributesSource.GetAttributesBag(), Nothing)
					End If
				Else
					Dim boundReturnTypeAttributesSource As SourcePropertySymbol = Me.BoundReturnTypeAttributesSource
					If (boundReturnTypeAttributesSource Is Nothing) Then
						MyBase.LoadAndValidateAttributes(Me.GetReturnTypeAttributeDeclarations(), lazyCustomAttributesBag, AttributeLocation.[Return])
					Else
						Interlocked.CompareExchange(Of CustomAttributesBag(Of VisualBasicAttributeData))(lazyCustomAttributesBag, boundReturnTypeAttributesSource.GetReturnTypeAttributesBag(), Nothing)
					End If
				End If
			End If
			Return lazyCustomAttributesBag
		End Function

		Friend Overrides Function GetBoundMethodBody(ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByRef methodBodyBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = Me.SyntaxTree
			Dim blockSyntax As MethodBlockBaseSyntax = Me.BlockSyntax
			methodBodyBinder = BinderBuilder.CreateBinderForMethodBody(Me.ContainingSourceModule, syntaxTree, Me)
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = methodBodyBinder.BindStatement(blockSyntax, diagnostics)
			boundBlock = If(boundStatement.Kind <> BoundKind.Block, New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(blockSyntax, blockSyntax.Statements, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(boundStatement), False), DirectCast(boundStatement, Microsoft.CodeAnalysis.VisualBasic.BoundBlock))
			Return boundBlock
		End Function

		Private Function GetDecodedReturnTypeWellKnownAttributeData() As CommonReturnTypeWellKnownAttributeData
			Dim mLazyReturnTypeCustomAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me.m_lazyReturnTypeCustomAttributesBag
			If (mLazyReturnTypeCustomAttributesBag Is Nothing OrElse Not mLazyReturnTypeCustomAttributesBag.IsDecodedWellKnownAttributeDataComputed) Then
				mLazyReturnTypeCustomAttributesBag = Me.GetReturnTypeAttributesBag()
			End If
			Return DirectCast(mLazyReturnTypeCustomAttributesBag.DecodedWellKnownAttributeData, CommonReturnTypeWellKnownAttributeData)
		End Function

		Protected Function GetDecodedWellKnownAttributeData() As MethodWellKnownAttributeData
			Dim mLazyCustomAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me.m_lazyCustomAttributesBag
			If (mLazyCustomAttributesBag Is Nothing OrElse Not mLazyCustomAttributesBag.IsDecodedWellKnownAttributeDataComputed) Then
				mLazyCustomAttributesBag = Me.GetAttributesBag()
			End If
			Return DirectCast(mLazyCustomAttributesBag.DecodedWellKnownAttributeData, MethodWellKnownAttributeData)
		End Function

		Public Overrides Function GetDllImportData() As DllImportData
			Dim decodedWellKnownAttributeData As MethodWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
			If (decodedWellKnownAttributeData Is Nothing) Then
				Return Nothing
			End If
			Return decodedWellKnownAttributeData.DllImportPlatformInvokeData
		End Function

		Public NotOverridable Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Dim str As String
			str = If(Not expandIncludes, SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(Me, preferredCulture, expandIncludes, Me._lazyDocComment, cancellationToken), SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(Me, preferredCulture, expandIncludes, Me._lazyExpandedDocComment, cancellationToken))
			Return str
		End Function

		Private Function GetEarlyDecodedWellKnownAttributeData() As MethodEarlyWellKnownAttributeData
			Dim mLazyCustomAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me.m_lazyCustomAttributesBag
			If (mLazyCustomAttributesBag Is Nothing OrElse Not mLazyCustomAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed) Then
				mLazyCustomAttributesBag = Me.GetAttributesBag()
			End If
			Return DirectCast(mLazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData, MethodEarlyWellKnownAttributeData)
		End Function

		Friend Function GetImplementingLocation(ByVal implementedMethod As MethodSymbol) As Microsoft.CodeAnalysis.Location
			Dim location As Microsoft.CodeAnalysis.Location
			Dim syntax As MethodStatementSyntax = Nothing
			Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = Nothing
			Dim mContainingType As SourceMemberContainerTypeSymbol = TryCast(Me.m_containingType, SourceMemberContainerTypeSymbol)
			If (Me.m_syntaxReferenceOpt IsNot Nothing) Then
				syntax = TryCast(Me.m_syntaxReferenceOpt.GetSyntax(New CancellationToken()), MethodStatementSyntax)
				syntaxTree = Me.m_syntaxReferenceOpt.SyntaxTree
			End If
			If (syntax Is Nothing OrElse syntax.ImplementsClause Is Nothing OrElse mContainingType Is Nothing) Then
				location = If(System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of Microsoft.CodeAnalysis.Location)(Me.Locations), NoLocation.Singleton)
			Else
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForType(mContainingType.ContainingSourceModule, syntaxTree, mContainingType)
				location = ImplementsHelper.FindImplementingSyntax(Of MethodSymbol)(syntax.ImplementsClause, Me, implementedMethod, mContainingType, binder).GetLocation()
			End If
			Return location
		End Function

		Friend Overrides Function GetLexicalSortKey() As LexicalSortKey
			If (Me.m_syntaxReferenceOpt Is Nothing) Then
				Return LexicalSortKey.NotInSource
			End If
			Return New LexicalSortKey(Me.m_syntaxReferenceOpt, Me.DeclaringCompilation)
		End Function

		Friend Shared Function GetMemberNameFromSyntax(ByVal node As MethodBaseSyntax) As String
			Dim valueText As String
			Select Case node.Kind()
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement
					valueText = DirectCast(node, MethodStatementSyntax).Identifier.ValueText
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement
					Dim flag As Boolean = False
					Dim enumerator As SyntaxTokenList.Enumerator = node.Modifiers.GetEnumerator()
					While enumerator.MoveNext()
						If (enumerator.Current.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword) Then
							Continue While
						End If
						flag = True
					End While
					If (node.Parent IsNot Nothing AndAlso (node.Parent.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock OrElse node.Parent.Parent IsNot Nothing AndAlso node.Parent.Parent.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock)) Then
						flag = True
					End If
					valueText = If(flag, ".cctor", ".ctor")
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
					valueText = DirectCast(node, DeclareStatementSyntax).Identifier.ValueText
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement
				Case 100
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement
					Throw ExceptionUtilities.UnexpectedValue(node.Kind())
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement
					Dim operatorStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.Syntax.OperatorStatementSyntax)
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = operatorStatementSyntax.OperatorToken.Kind()
					If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword) Then
						If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LikeKeyword) Then
							If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AndKeyword) Then
								valueText = "op_BitwiseAnd"
								Exit Select
							ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CTypeKeyword) Then
								Dim enumerator1 As SyntaxTokenList.Enumerator = operatorStatementSyntax.Modifiers.GetEnumerator()
								While enumerator1.MoveNext()
									Dim sourceMemberFlag As SourceMemberFlags = Microsoft.CodeAnalysis.VisualBasic.Binder.MapKeywordToFlag(enumerator1.Current)
									If (sourceMemberFlag <> SourceMemberFlags.[Widening]) Then
										If (sourceMemberFlag <> SourceMemberFlags.[Narrowing]) Then
											Continue While
										End If
										valueText = "op_Explicit"
										Return valueText
									Else
										valueText = "op_Implicit"
										Return valueText
									End If
								End While
								valueText = "op_Explicit"
								Exit Select
							ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LikeKeyword) Then
								valueText = "op_Like"
								Exit Select
							End If
						ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModKeyword) Then
							valueText = "op_Modulus"
							Exit Select
						ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword) Then
							valueText = "op_OnesComplement"
							Exit Select
						ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword) Then
							valueText = "op_BitwiseOr"
							Exit Select
						End If
					ElseIf (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword) Then
						Select Case syntaxKind
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
								valueText = "op_Concatenate"
								Return valueText
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndUsingStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWithStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSelectStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndStructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndInterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndNamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndPropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndAddHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRemoveHandlerStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndRaiseEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndWhileStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndTryStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndSyncLockStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompilationUnit Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImportsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlNamespaceImportsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamespaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InheritsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InterfaceStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterSingleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeParameterMultipleConstraintClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NewConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ClassConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeConstraint Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnumMemberDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ConstructorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParameterList Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AddHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventAccessorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ImplementsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeywordEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsPropertyEventContainer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HandlesClauseItem Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IncompleteMember Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FieldDeclaration Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableDeclarator Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SimpleAsClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsNewClause Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectMemberInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectCollectionInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferredFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ObjectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OptionalKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrElseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverloadsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RaiseEventKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReadOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReDimKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.REMKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RemoveHandlerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ResumeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ReturnKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SByteKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SelectKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SetKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShadowsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SharedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StaticKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StepKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StopKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StringKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StructureKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ThrowKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ToKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TryCastKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeOfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UIntegerKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ULongKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UShortKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhenKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhileKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WideningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WriteOnlyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GosubKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariantKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WendKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AggregateKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AllKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnsiKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AscendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AssemblyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AutoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BinaryKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ByKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CompareKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CustomKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DescendingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DisableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DistinctKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EnableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExplicitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalSourceKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExternalChecksumKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FromKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GroupKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.InferKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IntoKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.JoinKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MidKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OffKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OrderKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OutKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PreserveKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RegionKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SkipKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.StrictKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TakeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TextKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnicodeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UntilKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WarningKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WhereKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.TypeKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XmlKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsyncKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AwaitKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IteratorKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.YieldKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ExclamationToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AtToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CommaToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.HashToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AmpersandToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenParenToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseParenToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SemicolonToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DotToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ColonToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndFunctionStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndGetStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndOperatorStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndEventStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NamedFieldInitializer Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsValue Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.Parameter Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UsingBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SyncLockBlock Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LabelStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GoToStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NothingKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotInheritableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ParamArrayKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleQuoteToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OpenBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CloseBraceToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AsteriskToken
								valueText = "op_Multiply"
								Return valueText
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PlusToken
								If (operatorStatementSyntax.ParameterList.Parameters.Count > 1) Then
									valueText = "op_Addition"
									Return valueText
								Else
									valueText = "op_UnaryPlus"
									Return valueText
								End If
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MinusToken
								If (operatorStatementSyntax.ParameterList.Parameters.Count > 1) Then
									valueText = "op_Subtraction"
									Return valueText
								Else
									valueText = "op_UnaryNegation"
									Return valueText
								End If
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SlashToken
								valueText = "op_Division"
								Return valueText
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanToken
								valueText = "op_LessThan"
								Return valueText
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanEqualsToken
								valueText = "op_LessThanOrEqual"
								Return valueText
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanGreaterThanToken
								valueText = "op_Inequality"
								Return valueText
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EqualsToken
								valueText = "op_Equality"
								Return valueText
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanToken
								valueText = "op_GreaterThan"
								Return valueText
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanEqualsToken
								valueText = "op_GreaterThanOrEqual"
								Return valueText
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.BackslashToken
								valueText = "op_IntegerDivision"
								Return valueText
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaretToken
								valueText = "op_Exponent"
								Return valueText
							Case Else
								If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LessThanLessThanToken) Then
									valueText = "op_LeftShift"
									Return valueText
								ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.GreaterThanGreaterThanToken) Then
									valueText = "op_RightShift"
									Return valueText
								Else
									Exit Select
								End If
						End Select
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.XorKeyword) Then
						valueText = "op_ExclusiveOr"
						Exit Select
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsFalseKeyword) Then
						valueText = "op_False"
						Exit Select
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.IsTrueKeyword) Then
						valueText = "op_True"
						Exit Select
					End If
					Throw ExceptionUtilities.UnexpectedValue(operatorStatementSyntax.OperatorToken.Kind())
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PropertyStatement
					valueText = DirectCast(node, PropertyStatementSyntax).Identifier.ValueText
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(node.Kind())
			End Select
			Return valueText
		End Function

		Private Shared Function GetMethodLocationFromSyntax(ByVal node As VisualBasicSyntaxNode) As TextSpan
			Dim span As TextSpan
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = node.Kind()
			Select Case syntaxKind
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FunctionStatement
					span = DirectCast(node, MethodStatementSyntax).Identifier.Span
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SubNewStatement
					span = DirectCast(node, SubNewStatementSyntax).NewKeyword.Span
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
					span = DirectCast(node, DeclareStatementSyntax).Identifier.Span
					Exit Select
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateSubStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DelegateFunctionStatement
				Case 100
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EndIfStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareSubStatement Or Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.DeclareFunctionStatement
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EventStatement
					Throw ExceptionUtilities.UnexpectedValue(node.Kind())
				Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OperatorStatement
					span = DirectCast(node, OperatorStatementSyntax).OperatorToken.Span
					Exit Select
				Case Else
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineFunctionLambdaExpression AndAlso CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineSubLambdaExpression) > CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EmptyStatement)) Then
						Throw ExceptionUtilities.UnexpectedValue(node.Kind())
					End If
					span = DirectCast(node, LambdaExpressionSyntax).SubOrFunctionHeader.Span
					Exit Select
			End Select
			Return span
		End Function

		Private Shared Function GetPInvokeAttributes(ByVal syntax As DeclareStatementSyntax) As MethodImportAttributes
			Dim methodImportAttribute As MethodImportAttributes = 0
			Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = syntax.CharsetKeyword.Kind()
			If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnsiKeyword) Then
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.None OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AnsiKeyword) Then
					methodImportAttribute = MethodImportAttributes.ExactSpelling Or MethodImportAttributes.CharSetAnsi
				End If
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AutoKeyword) Then
				methodImportAttribute = MethodImportAttributes.CharSetAuto
			ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.UnicodeKeyword) Then
				methodImportAttribute = MethodImportAttributes.ExactSpelling Or MethodImportAttributes.CharSetUnicode
			End If
			Return methodImportAttribute Or MethodImportAttributes.CallingConventionWinApi Or MethodImportAttributes.SetLastError
		End Function

		Protected Overridable Function GetReturnTypeAttributeDeclarations() As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
			Return OneOrMany.Create(Of SyntaxList(Of AttributeListSyntax))(Me.ReturnTypeAttributeDeclarationSyntaxList)
		End Function

		Public NotOverridable Overrides Function GetReturnTypeAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.GetReturnTypeAttributesBag().Attributes
		End Function

		Private Function GetReturnTypeAttributesBag() As CustomAttributesBag(Of VisualBasicAttributeData)
			Return Me.GetAttributesBag(Me.m_lazyReturnTypeCustomAttributesBag, True)
		End Function

		Friend NotOverridable Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
			Dim securityAttributes As IEnumerable(Of SecurityAttribute)
			Dim attributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me.GetAttributesBag()
			Dim decodedWellKnownAttributeData As MethodWellKnownAttributeData = DirectCast(attributesBag.DecodedWellKnownAttributeData, MethodWellKnownAttributeData)
			If (decodedWellKnownAttributeData IsNot Nothing) Then
				Dim securityInformation As SecurityWellKnownAttributeData = decodedWellKnownAttributeData.SecurityInformation
				If (securityInformation Is Nothing) Then
					securityAttributes = SpecializedCollections.EmptyEnumerable(Of SecurityAttribute)()
					Return securityAttributes
				End If
				securityAttributes = securityInformation.GetSecurityAttributes(Of VisualBasicAttributeData)(attributesBag.Attributes)
				Return securityAttributes
			End If
			securityAttributes = SpecializedCollections.EmptyEnumerable(Of SecurityAttribute)()
			Return securityAttributes
		End Function

		Private Function GetSymbolLocation(ByVal syntaxRef As SyntaxReference) As Location
			Dim visualBasicSyntax As VisualBasicSyntaxNode = syntaxRef.GetVisualBasicSyntax(New CancellationToken())
			Return syntaxRef.SyntaxTree.GetLocation(SourceMethodSymbol.GetMethodLocationFromSyntax(visualBasicSyntax))
		End Function

		Friend Shared Function GetTypeParameterListSyntax(ByVal methodSyntax As MethodBaseSyntax) As TypeParameterListSyntax
			Dim typeParameterList As TypeParameterListSyntax
			If (methodSyntax.Kind() = SyntaxKind.SubStatement OrElse methodSyntax.Kind() = SyntaxKind.FunctionStatement) Then
				typeParameterList = DirectCast(methodSyntax, MethodStatementSyntax).TypeParameterList
			Else
				typeParameterList = Nothing
			End If
			Return typeParameterList
		End Function

		Friend NotOverridable Overrides Function IsDefinedInSourceTree(ByVal tree As Microsoft.CodeAnalysis.SyntaxTree, ByVal definedWithinSpan As Nullable(Of TextSpan), Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Boolean
			Return Symbol.IsDefinedInSourceTree(Me.Syntax, tree, definedWithinSpan, cancellationToken)
		End Function

		Private Function IsDllImportAttributeAllowed(ByVal syntax As AttributeSyntax, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Boolean
			Dim flag As Boolean
			Dim partialImplementationPart As Object
			Dim methodKind As Microsoft.CodeAnalysis.MethodKind = Me.MethodKind
			If (CInt(methodKind) - CInt(Microsoft.CodeAnalysis.MethodKind.EventAdd) <= CInt(Microsoft.CodeAnalysis.MethodKind.Conversion)) Then
				diagnostics.Add(ERRID.ERR_DllImportNotLegalOnEventMethod, syntax.Name.GetLocation())
				flag = False
			ElseIf (CInt(methodKind) - CInt(Microsoft.CodeAnalysis.MethodKind.PropertyGet) <= CInt(Microsoft.CodeAnalysis.MethodKind.Constructor)) Then
				diagnostics.Add(ERRID.ERR_DllImportNotLegalOnGetOrSet, syntax.Name.GetLocation())
				flag = False
			ElseIf (methodKind = Microsoft.CodeAnalysis.MethodKind.DeclareMethod) Then
				diagnostics.Add(ERRID.ERR_DllImportNotLegalOnDeclare, syntax.Name.GetLocation())
				flag = False
			ElseIf (Me.ContainingType IsNot Nothing AndAlso Me.ContainingType.IsInterface) Then
				diagnostics.Add(ERRID.ERR_DllImportOnInterfaceMethod, syntax.Name.GetLocation())
				flag = False
			ElseIf (Me.IsGenericMethod OrElse Me.ContainingType IsNot Nothing AndAlso Me.ContainingType.IsGenericType) Then
				diagnostics.Add(ERRID.ERR_DllImportOnGenericSubOrFunction, syntax.Name.GetLocation())
				flag = False
			ElseIf (Me.IsShared) Then
				If (Me.IsPartial) Then
					partialImplementationPart = Me.PartialImplementationPart
				Else
					partialImplementationPart = Me
				End If
				Dim sourceMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol = TryCast(partialImplementationPart, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol)
				If (sourceMethodSymbol IsNot Nothing AndAlso (sourceMethodSymbol.IsAsync OrElse sourceMethodSymbol.IsIterator) AndAlso Not sourceMethodSymbol.ContainingType.IsInterfaceType()) Then
					Dim nonMergedLocation As Location = sourceMethodSymbol.NonMergedLocation
					If (nonMergedLocation Is Nothing) Then
						GoTo Label1
					End If
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, nonMergedLocation, ERRID.ERR_DllImportOnResumableMethod)
					flag = False
					Return flag
				End If
			Label1:
				If (Me.HasEmptyBody) Then
					flag = True
				Else
					diagnostics.Add(ERRID.ERR_DllImportOnNonEmptySubOrFunction, syntax.Name.GetLocation())
					flag = False
				End If
			Else
				diagnostics.Add(ERRID.ERR_DllImportOnInstanceMethod, syntax.Name.GetLocation())
				flag = False
			End If
			Return flag
		End Function

		Private Function IsVtableGapInterfaceMethod() As Boolean
			If (Not Me.ContainingType.IsInterface) Then
				Return False
			End If
			Return ModuleExtensions.GetVTableGapSize(Me.MetadataName) > 0
		End Function

		Friend Overrides Sub PostDecodeWellKnownAttributes(ByVal boundAttributes As ImmutableArray(Of VisualBasicAttributeData), ByVal allAttributeSyntaxNodes As ImmutableArray(Of AttributeSyntax), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal symbolPart As AttributeLocation, ByVal decodedData As WellKnownAttributeData)
			If (symbolPart <> AttributeLocation.[Return]) Then
				Dim methodWellKnownAttributeDatum As MethodWellKnownAttributeData = DirectCast(decodedData, MethodWellKnownAttributeData)
				If (methodWellKnownAttributeDatum IsNot Nothing AndAlso methodWellKnownAttributeDatum.HasSTAThreadAttribute AndAlso methodWellKnownAttributeDatum.HasMTAThreadAttribute) Then
					diagnostics.Add(ERRID.ERR_STAThreadAndMTAThread0, Me.NonMergedLocation)
				End If
			End If
			MyBase.PostDecodeWellKnownAttributes(boundAttributes, allAttributeSyntaxNodes, diagnostics, symbolPart, decodedData)
		End Sub

		Private Shared Sub ReportPartialMethodErrors(ByVal modifiers As SyntaxTokenList, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As DiagnosticBag)
			Dim item As Microsoft.CodeAnalysis.SyntaxToken
			Dim flag As Boolean = True
			Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = New Microsoft.CodeAnalysis.SyntaxToken()
			Dim list As List(Of Microsoft.CodeAnalysis.SyntaxToken) = DirectCast(modifiers, IEnumerable(Of Microsoft.CodeAnalysis.SyntaxToken)).ToList()
			Dim count As Integer = list.Count - 1
			Dim num As Integer = 0
		Label3:
			While num <= count
				item = list(num)
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = item.Kind()
				If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MustOverrideKeyword) Then
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.NotOverridableKeyword) Then
						Select Case syntaxKind
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridableKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.OverridesKeyword
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PublicKeyword
								Exit Select
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PartialKeyword
								syntaxToken = item
								GoTo Label0
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.PrivateKeyword
								flag = False
								GoTo Label0
							Case Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword
								If (num >= list.Count - 1 OrElse list(num + 1).Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FriendKeyword) Then
									Exit Select
								Else
									GoTo Label1
								End If
							Case Else
								GoTo Label0
						End Select
					End If
				ElseIf (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.FriendKeyword) Then
					If (CUShort(syntaxKind) - CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MustInheritKeyword) <= CUShort(Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.List)) Then
						GoTo Label2
					End If
					GoTo Label0
				ElseIf (num < list.Count - 1) Then
					If (list(num + 1).Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ProtectedKeyword) Then
						GoTo Label1
					End If
				End If
			Label2:
				Dim syntaxNodeOrToken As Microsoft.CodeAnalysis.SyntaxNodeOrToken = item
				Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, syntaxNodeOrToken, ERRID.ERR_OnlyPrivatePartialMethods1, New [Object]() { SyntaxFacts.GetText(item.Kind()) })
				flag = False
				GoTo Label0
			End While
			If (flag) Then
				Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, syntaxToken, ERRID.ERR_PartialMethodsMustBePrivate)
			End If
			Return
		Label0:
			num = num + 1
			GoTo Label3
		Label1:
			num = num + 1
			Dim item1 As Microsoft.CodeAnalysis.SyntaxToken = list(num)
			Dim num1 As Integer = Math.Min(item.SpanStart, item1.SpanStart)
			Dim span As TextSpan = item.Span
			Dim [end] As Integer = span.[End]
			span = item1.Span
			Dim num2 As Integer = Math.Max([end], span.[End])
			Dim location As Microsoft.CodeAnalysis.Location = binder.SyntaxTree.GetLocation(New TextSpan(num1, num2 - num1))
			Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, location, ERRID.ERR_OnlyPrivatePartialMethods1, New [Object]() { [String].Concat(item.Kind().GetText(), " ", item1.Kind().GetText()) })
			flag = False
			GoTo Label0
		End Sub

		Friend Function SetDiagnostics(ByVal diags As ImmutableArray(Of Diagnostic)) As Boolean
			Return ImmutableInterlocked.InterlockedInitialize(Of Diagnostic)(Me._cachedDiagnostics, diags)
		End Function

		Friend NotOverridable Overrides Function TryGetMeParameter(<Out> ByRef meParameter As ParameterSymbol) As Boolean
			If (Not Me.IsShared) Then
				If (Me._lazyMeParameter Is Nothing) Then
					Interlocked.CompareExchange(Of ParameterSymbol)(Me._lazyMeParameter, New MeParameterSymbol(Me), Nothing)
				End If
				meParameter = Me._lazyMeParameter
			Else
				meParameter = Nothing
			End If
			Return True
		End Function

		Private Function VerifyObsoleteAttributeAppliedToMethod(ByRef arguments As DecodeWellKnownAttributeArguments(Of AttributeSyntax, VisualBasicAttributeData, AttributeLocation), ByVal description As AttributeDescription) As Boolean
			Dim flag As Boolean
			If (Not arguments.Attribute.IsTargetAttribute(Me, description)) Then
				flag = False
			Else
				If (Me.IsAccessor() AndAlso Me.AssociatedSymbol.Kind = SymbolKind.[Event]) Then
					DirectCast(arguments.Diagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag).Add(ERRID.ERR_ObsoleteInvalidOnEventMember, Me.Locations(0), New [Object]() { description.FullName })
				End If
				flag = True
			End If
			Return flag
		End Function
	End Class
End Namespace