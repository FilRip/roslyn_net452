Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Globalization
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	<DebuggerDisplay("{GetDebuggerDisplay(), nq}")>
	Friend MustInherit Class Symbol
		Implements IReference, ISymbol, ISymbolInternal, IFormattable
		Friend ReadOnly Property AdaptedSymbol As Symbol
			Get
				Return Me
			End Get
		End Property

		Public ReadOnly Property CanBeReferencedByName As Boolean
			Get
				Dim length As Boolean
				Select Case Me.Kind
					Case SymbolKind.[Alias]
					Case SymbolKind.Label
					Case SymbolKind.Local
						length = Me.Name.Length > 0
						Exit Select
					Case SymbolKind.ArrayType
					Case SymbolKind.Assembly
					Case SymbolKind.NetModule
						length = False
						Exit Select
					Case SymbolKind.DynamicType
					Case SymbolKind.PointerType
						Throw ExceptionUtilities.UnexpectedValue(Me.Kind)
					Case SymbolKind.ErrorType
					Case SymbolKind.[Event]
					Case SymbolKind.Field
					Case SymbolKind.[Namespace]
					Case SymbolKind.Parameter
					Case SymbolKind.[Property]
					Case SymbolKind.RangeVariable
					Case SymbolKind.TypeParameter
					Label0:
						If (Not Me.Dangerous_IsFromSomeCompilationIncludingRetargeting) Then
							length = SyntaxFacts.IsValidIdentifier(Me.Name)
							Exit Select
						Else
							length = If([String].IsNullOrEmpty(Me.Name), False, SyntaxFacts.IsIdentifierStartCharacter(Me.Name(0)))
							Exit Select
						End If
					Case SymbolKind.Method
						Dim methodKind As Microsoft.CodeAnalysis.MethodKind = DirectCast(Me, MethodSymbol).MethodKind
						If (CInt(methodKind) - CInt(Microsoft.CodeAnalysis.MethodKind.Conversion) > CInt(Microsoft.CodeAnalysis.MethodKind.Constructor)) Then
							Select Case methodKind
								Case Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator
									Exit Select
								Case Microsoft.CodeAnalysis.MethodKind.Ordinary
								Case Microsoft.CodeAnalysis.MethodKind.ReducedExtension
									GoTo Label0
								Case Microsoft.CodeAnalysis.MethodKind.PropertyGet
								Case Microsoft.CodeAnalysis.MethodKind.PropertySet
									length = False
									Return length
								Case Else
									If (methodKind = Microsoft.CodeAnalysis.MethodKind.DeclareMethod) Then
										GoTo Label0
									End If
									length = False
									Return length
							End Select
						End If
						length = True
						Exit Select
					Case SymbolKind.NamedType
						If (Not DirectCast(Me, NamedTypeSymbol).IsSubmissionClass) Then
							GoTo Label0
						End If
						length = False
						Exit Select
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(Me.Kind)
				End Select
				Return length
			End Get
		End Property

		Friend ReadOnly Property CanBeReferencedByNameIgnoringIllegalCharacters As Boolean
			Get
				Dim flag As Boolean
				If (Me.Kind <> SymbolKind.Method) Then
					flag = True
				Else
					Dim methodKind As Microsoft.CodeAnalysis.MethodKind = DirectCast(Me, MethodSymbol).MethodKind
					If (methodKind <= Microsoft.CodeAnalysis.MethodKind.Ordinary) Then
						If (CInt(methodKind) - CInt(Microsoft.CodeAnalysis.MethodKind.Conversion) <= CInt(Microsoft.CodeAnalysis.MethodKind.Constructor) OrElse CInt(methodKind) - CInt(Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator) <= CInt(Microsoft.CodeAnalysis.MethodKind.Constructor)) Then
							GoTo Label1
						End If
						flag = False
						Return flag
					ElseIf (methodKind <> Microsoft.CodeAnalysis.MethodKind.ReducedExtension AndAlso methodKind <> Microsoft.CodeAnalysis.MethodKind.DeclareMethod) Then
						flag = False
						Return flag
					End If
				Label1:
					flag = True
				End If
				Return flag
			End Get
		End Property

		Public Overridable ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Dim assemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol
				Dim containingSymbol As Symbol = Me.ContainingSymbol
				If (containingSymbol Is Nothing) Then
					assemblySymbol = Nothing
				Else
					assemblySymbol = containingSymbol.ContainingAssembly
				End If
				Return assemblySymbol
			End Get
		End Property

		Public Overridable ReadOnly Property ContainingModule As ModuleSymbol
			Get
				Dim moduleSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ModuleSymbol
				Dim containingSymbol As Symbol = Me.ContainingSymbol
				If (containingSymbol Is Nothing) Then
					moduleSymbol = Nothing
				Else
					moduleSymbol = containingSymbol.ContainingModule
				End If
				Return moduleSymbol
			End Get
		End Property

		Public ReadOnly Property ContainingNamespace As NamespaceSymbol
			Get
				Dim namespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol
				Dim containingSymbol As Symbol = Me.ContainingSymbol
				While True
					If (containingSymbol IsNot Nothing) Then
						Dim namespaceSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol = TryCast(containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceSymbol)
						If (namespaceSymbol1 Is Nothing) Then
							containingSymbol = containingSymbol.ContainingSymbol
						Else
							namespaceSymbol = namespaceSymbol1
							Exit While
						End If
					Else
						namespaceSymbol = Nothing
						Exit While
					End If
				End While
				Return namespaceSymbol
			End Get
		End Property

		Friend ReadOnly Property ContainingNamespaceOrType As NamespaceOrTypeSymbol
			Get
				Dim containingSymbol As NamespaceOrTypeSymbol
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me.ContainingSymbol
				If (symbol IsNot Nothing) Then
					Dim kind As SymbolKind = symbol.Kind
					If (kind = SymbolKind.NamedType) Then
						containingSymbol = DirectCast(Me.ContainingSymbol, NamedTypeSymbol)
						Return containingSymbol
					Else
						If (kind <> SymbolKind.[Namespace]) Then
							containingSymbol = Nothing
							Return containingSymbol
						End If
						containingSymbol = DirectCast(Me.ContainingSymbol, NamespaceSymbol)
						Return containingSymbol
					End If
				End If
				containingSymbol = Nothing
				Return containingSymbol
			End Get
		End Property

		Public MustOverride ReadOnly Property ContainingSymbol As Symbol

		Public Overridable ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Dim containingSymbol As Symbol = Me.ContainingSymbol
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(containingSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
				Return If(namedTypeSymbol <> containingSymbol, containingSymbol.ContainingType, namedTypeSymbol)
			End Get
		End Property

		Friend ReadOnly Property Dangerous_IsFromSomeCompilationIncludingRetargeting As Boolean
			Get
				Dim flag As Boolean
				Dim containingModule As Object
				If (Me.DeclaringCompilation IsNot Nothing) Then
					flag = True
				ElseIf (Me.Kind <> SymbolKind.Assembly) Then
					If (Me.Kind = SymbolKind.NetModule) Then
						containingModule = Me
					Else
						containingModule = Me.ContainingModule
					End If
					Dim retargetingModuleSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting.RetargetingModuleSymbol = TryCast(containingModule, Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting.RetargetingModuleSymbol)
					flag = If(retargetingModuleSymbol Is Nothing, False, retargetingModuleSymbol.UnderlyingModule.DeclaringCompilation IsNot Nothing)
				Else
					Dim retargetingAssemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting.RetargetingAssemblySymbol = TryCast(Me, Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting.RetargetingAssemblySymbol)
					flag = If(retargetingAssemblySymbol Is Nothing, False, retargetingAssemblySymbol.UnderlyingAssembly.DeclaringCompilation IsNot Nothing)
				End If
				Return flag
			End Get
		End Property

		Public MustOverride ReadOnly Property DeclaredAccessibility As Accessibility

		Friend Overridable ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Dim visualBasicCompilation As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation
				Dim visualBasicCompilation1 As Microsoft.CodeAnalysis.VisualBasic.VisualBasicCompilation
				Dim kind As SymbolKind = Me.Kind
				If (kind = SymbolKind.Assembly) Then
					visualBasicCompilation = Nothing
				ElseIf (kind = SymbolKind.ErrorType) Then
					visualBasicCompilation = Nothing
				ElseIf (kind = SymbolKind.NetModule) Then
					visualBasicCompilation = Nothing
				Else
					Dim containingModule As SourceModuleSymbol = TryCast(Me.ContainingModule, SourceModuleSymbol)
					If (containingModule Is Nothing) Then
						visualBasicCompilation1 = Nothing
					Else
						visualBasicCompilation1 = containingModule.DeclaringCompilation
					End If
					visualBasicCompilation = visualBasicCompilation1
				End If
				Return visualBasicCompilation
			End Get
		End Property

		Public MustOverride ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)

		Friend ReadOnly Property EffectiveDefaultMarshallingCharSet As Nullable(Of CharSet)
			Get
				If (Not Me.IsEmbedded) Then
					Return Me.ContainingModule.DefaultMarshallingCharSet
				End If
				Return Nothing
			End Get
		End Property

		Friend Overridable ReadOnly Property EmbeddedSymbolKind As Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind
			Get
				Return Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind.None
			End Get
		End Property

		Public Overridable ReadOnly Property HasUnsupportedMetadata As Boolean Implements ISymbol.HasUnsupportedMetadata
			Get
				Return False
			End Get
		End Property

		Protected Overridable ReadOnly Property HighestPriorityUseSiteError As Integer
			Get
				Return 2147483647
			End Get
		End Property

		Friend Overridable ReadOnly Property ImplicitlyDefinedBy(ByVal membersInProgress As Dictionary(Of String, ArrayBuilder(Of Symbol))) As Symbol
			Get
				Return Nothing
			End Get
		End Property

		Public ReadOnly Property IsDefinition As Boolean
			Get
				Return CObj(Me.OriginalDefinition) = CObj(Me)
			End Get
		End Property

		Friend ReadOnly Property IsEmbedded As Boolean
			Get
				Return Me.EmbeddedSymbolKind <> Microsoft.CodeAnalysis.VisualBasic.Symbols.EmbeddedSymbolKind.None
			End Get
		End Property

		Public Overridable ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overridable ReadOnly Property IsLambdaMethod As Boolean
			Get
				Return False
			End Get
		End Property

		Public MustOverride ReadOnly Property IsMustOverride As Boolean

		Friend Overridable ReadOnly Property IsMyGroupCollectionProperty As Boolean
			Get
				Return False
			End Get
		End Property

		Public MustOverride ReadOnly Property IsNotOverridable As Boolean

		Public MustOverride ReadOnly Property IsOverridable As Boolean

		Public MustOverride ReadOnly Property IsOverrides As Boolean

		Friend Overridable ReadOnly Property IsQueryLambdaMethod As Boolean
			Get
				Return False
			End Get
		End Property

		Public MustOverride ReadOnly Property IsShared As Boolean

		ReadOnly Property ISymbol_CanBeReferencedByName As Boolean Implements ISymbol.CanBeReferencedByName
			Get
				Return Me.CanBeReferencedByName
			End Get
		End Property

		ReadOnly Property ISymbol_ContainingAssembly As IAssemblySymbol Implements ISymbol.ContainingAssembly
			Get
				Return Me.ContainingAssembly
			End Get
		End Property

		ReadOnly Property ISymbol_ContainingModule As IModuleSymbol Implements ISymbol.ContainingModule
			Get
				Return Me.ContainingModule
			End Get
		End Property

		ReadOnly Property ISymbol_ContainingNamespace As INamespaceSymbol Implements ISymbol.ContainingNamespace
			Get
				Return Me.ContainingNamespace
			End Get
		End Property

		ReadOnly Property ISymbol_ContainingSymbol As ISymbol Implements ISymbol.ContainingSymbol
			Get
				Return Me.ContainingSymbol
			End Get
		End Property

		ReadOnly Property ISymbol_ContainingType As INamedTypeSymbol Implements ISymbol.ContainingType
			Get
				Return Me.ContainingType
			End Get
		End Property

		ReadOnly Property ISymbol_DeclaredAccessibility As Accessibility Implements ISymbol.DeclaredAccessibility, ISymbolInternal.DeclaredAccessibility
			Get
				Return Me.DeclaredAccessibility
			End Get
		End Property

		ReadOnly Property ISymbol_DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference) Implements ISymbol.DeclaringSyntaxReferences
			Get
				Return Me.DeclaringSyntaxReferences
			End Get
		End Property

		Protected Overridable ReadOnly Property ISymbol_IsAbstract As Boolean Implements ISymbol.IsAbstract, ISymbolInternal.IsAbstract
			Get
				Return Me.IsMustOverride
			End Get
		End Property

		ReadOnly Property ISymbol_IsDefinition As Boolean Implements ISymbol.IsDefinition, ISymbolInternal.IsDefinition
			Get
				Return Me.IsDefinition
			End Get
		End Property

		ReadOnly Property ISymbol_IsExtern As Boolean Implements ISymbol.IsExtern
			Get
				Return False
			End Get
		End Property

		ReadOnly Property ISymbol_IsImplicitlyDeclared As Boolean Implements ISymbol.IsImplicitlyDeclared, ISymbolInternal.IsImplicitlyDeclared
			Get
				Return Me.IsImplicitlyDeclared
			End Get
		End Property

		ReadOnly Property ISymbol_IsOverride As Boolean Implements ISymbol.IsOverride, ISymbolInternal.IsOverride
			Get
				Return Me.IsOverrides
			End Get
		End Property

		Protected Overridable ReadOnly Property ISymbol_IsSealed As Boolean Implements ISymbol.IsSealed
			Get
				Return Me.IsNotOverridable
			End Get
		End Property

		Protected Overridable ReadOnly Property ISymbol_IsStatic As Boolean Implements ISymbol.IsStatic, ISymbolInternal.IsStatic
			Get
				Return Me.IsShared
			End Get
		End Property

		ReadOnly Property ISymbol_IsVirtual As Boolean Implements ISymbol.IsVirtual, ISymbolInternal.IsVirtual
			Get
				Return Me.IsOverridable
			End Get
		End Property

		ReadOnly Property ISymbol_Kind As SymbolKind Implements ISymbol.Kind, ISymbolInternal.Kind
			Get
				Return Me.Kind
			End Get
		End Property

		ReadOnly Property ISymbol_Locations As ImmutableArray(Of Location) Implements ISymbol.Locations, ISymbolInternal.Locations
			Get
				Return Me.Locations
			End Get
		End Property

		ReadOnly Property ISymbol_Name As String Implements ISymbol.Name, ISymbolInternal.Name
			Get
				Return Me.Name
			End Get
		End Property

		ReadOnly Property ISymbol_OriginalDefinition As ISymbol Implements ISymbol.OriginalDefinition
			Get
				Return Me.OriginalDefinition
			End Get
		End Property

		ReadOnly Property ISymbolInternal_ContainingAssembly As IAssemblySymbolInternal Implements ISymbolInternal.ContainingAssembly
			Get
				Return Me.ContainingAssembly
			End Get
		End Property

		ReadOnly Property ISymbolInternal_ContainingModule As IModuleSymbolInternal Implements ISymbolInternal.ContainingModule
			Get
				Return Me.ContainingModule
			End Get
		End Property

		ReadOnly Property ISymbolInternal_ContainingNamespace As INamespaceSymbolInternal Implements ISymbolInternal.ContainingNamespace
			Get
				Return Me.ContainingNamespace
			End Get
		End Property

		ReadOnly Property ISymbolInternal_ContainingSymbol As ISymbolInternal Implements ISymbolInternal.ContainingSymbol
			Get
				Return Me.ContainingSymbol
			End Get
		End Property

		ReadOnly Property ISymbolInternal_ContainingType As INamedTypeSymbolInternal Implements ISymbolInternal.ContainingType
			Get
				Return Me.ContainingType
			End Get
		End Property

		Public ReadOnly Property ISymbolInternal_DeclaringCompilation As Compilation Implements ISymbolInternal.DeclaringCompilation
			Get
				Return Me.DeclaringCompilation
			End Get
		End Property

		Public MustOverride ReadOnly Property Kind As SymbolKind

		Public ReadOnly Property Language As String Implements ISymbol.Language
			Get
				Return "Visual Basic"
			End Get
		End Property

		Public MustOverride ReadOnly Property Locations As ImmutableArray(Of Location)

		Public Overridable ReadOnly Property MetadataName As String Implements ISymbol.MetadataName, ISymbolInternal.MetadataName
			Get
				Return Me.Name
			End Get
		End Property

		Public Overridable ReadOnly Property Name As String
			Get
				Return [String].Empty
			End Get
		End Property

		Friend MustOverride ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData

		Friend ReadOnly Property ObsoleteKind As ObsoleteAttributeKind
			Get
				Dim obsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData = Me.ObsoleteAttributeData
				If (obsoleteAttributeData Is Nothing) Then
					Return ObsoleteAttributeKind.None
				End If
				Return obsoleteAttributeData.Kind
			End Get
		End Property

		Friend ReadOnly Property ObsoleteState As ThreeState
			Get
				Dim threeState As Microsoft.CodeAnalysis.ThreeState
				Select Case Me.ObsoleteKind
					Case ObsoleteAttributeKind.None
					Case ObsoleteAttributeKind.Experimental
						threeState = Microsoft.CodeAnalysis.ThreeState.[False]
						Exit Select
					Case ObsoleteAttributeKind.Uninitialized
						threeState = Microsoft.CodeAnalysis.ThreeState.Unknown
						Exit Select
					Case ObsoleteAttributeKind.Obsolete
					Case ObsoleteAttributeKind.Deprecated
					Label0:
						threeState = Microsoft.CodeAnalysis.ThreeState.[True]
						Exit Select
					Case Else
						GoTo Label0
				End Select
				Return threeState
			End Get
		End Property

		Public ReadOnly Property OriginalDefinition As Symbol
			Get
				Return Me.OriginalSymbolDefinition
			End Get
		End Property

		Protected Overridable ReadOnly Property OriginalSymbolDefinition As Symbol
			Get
				Return Me
			End Get
		End Property

		Friend ReadOnly Property PrimaryDependency As AssemblySymbol
			Get
				Dim assemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol
				Dim containingAssembly As Microsoft.CodeAnalysis.VisualBasic.Symbols.AssemblySymbol = Me.ContainingAssembly
				If (containingAssembly Is Nothing OrElse Not (containingAssembly.CorLibrary = containingAssembly)) Then
					assemblySymbol = containingAssembly
				Else
					assemblySymbol = Nothing
				End If
				Return assemblySymbol
			End Get
		End Property

		Friend Overridable ReadOnly Property ShadowsExplicitly As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Sub New()
			MyBase.New()
		End Sub

		Friend MustOverride Function Accept(Of TArgument, TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TArgument, TResult), ByVal arg As TArgument) As TResult

		Public MustOverride Sub ExplicitAccept(ByVal visitor As SymbolVisitor) Implements ISymbol.Accept

		Public MustOverride Function ExplicitAccept(Of TResult)(ByVal visitor As SymbolVisitor(Of TResult)) As TResult Implements ISymbol.Accept

		Public MustOverride Sub Accept(ByVal visitor As VisualBasicSymbolVisitor)

		Public MustOverride Function Accept(Of TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TResult)) As TResult

		Friend Shared Sub AddSynthesizedAttribute(ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData), ByVal attribute As SynthesizedAttributeData)
			If (attribute IsNot Nothing) Then
				If (attributes Is Nothing) Then
					attributes = ArrayBuilder(Of SynthesizedAttributeData).GetInstance(4)
				End If
				attributes.Add(attribute)
			End If
		End Sub

		Friend Overridable Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
		End Sub

		<Conditional("DEBUG")>
		Protected Friend Sub CheckDefinitionInvariant()
		End Sub

		Protected Shared Function ConstructTypeArguments(ByVal ParamArray typeArguments As ITypeSymbol()) As ImmutableArray(Of TypeSymbol)
			Dim instance As ArrayBuilder(Of TypeSymbol) = ArrayBuilder(Of TypeSymbol).GetInstance(CInt(typeArguments.Length))
			Dim typeSymbolArray As ITypeSymbol() = typeArguments
			Dim num As Integer = 0
			Do
				instance.Add(typeSymbolArray(num).EnsureVbSymbolOrNothing(Of TypeSymbol)("typeArguments"))
				num = num + 1
			Loop While num < CInt(typeSymbolArray.Length)
			Return instance.ToImmutableAndFree()
		End Function

		Protected Shared Function ConstructTypeArguments(ByVal typeArguments As ImmutableArray(Of ITypeSymbol), ByVal typeArgumentNullableAnnotations As ImmutableArray(Of NullableAnnotation)) As ImmutableArray(Of TypeSymbol)
			Dim func As Func(Of ITypeSymbol, TypeSymbol)
			If (typeArguments.IsDefault) Then
				Throw New ArgumentException("typeArguments")
			End If
			Dim length As Integer = typeArguments.Length
			If (Not typeArgumentNullableAnnotations.IsDefault AndAlso typeArgumentNullableAnnotations.Length <> length) Then
				Throw New ArgumentException("typeArgumentNullableAnnotations")
			End If
			Dim typeSymbols As ImmutableArray(Of ITypeSymbol) = typeArguments
			If (Symbol._Closure$__.$I201-0 Is Nothing) Then
				func = Function(typeArg As ITypeSymbol) typeArg.EnsureVbSymbolOrNothing(Of TypeSymbol)("typeArguments")
				Symbol._Closure$__.$I201-0 = func
			Else
				func = Symbol._Closure$__.$I201-0
			End If
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of ITypeSymbol, TypeSymbol)(typeSymbols, func)
		End Function

		Friend Overridable Sub DecodeWellKnownAttribute(ByRef arguments As DecodeWellKnownAttributeArguments(Of AttributeSyntax, VisualBasicAttributeData, AttributeLocation))
			Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
			Me.MarkEmbeddedAttributeTypeReference(arguments.Attribute, arguments.AttributeSyntaxOpt, declaringCompilation)
			Me.ReportExtensionAttributeUseSiteInfo(arguments.Attribute, arguments.AttributeSyntaxOpt, declaringCompilation, DirectCast(arguments.Diagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag))
			If (arguments.Attribute.IsTargetAttribute(Me, AttributeDescription.SkipLocalsInitAttribute)) Then
				DirectCast(arguments.Diagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag).Add(ERRID.WRN_AttributeNotSupportedInVB, arguments.AttributeSyntaxOpt.Location, New [Object]() { AttributeDescription.SkipLocalsInitAttribute.FullName })
			End If
		End Sub

		Friend Function DeriveUseSiteInfoFromCustomModifiers(ByVal customModifiers As ImmutableArray(Of CustomModifier), Optional ByVal allowIsExternalInit As Boolean = False) As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo1 As UseSiteInfo(Of AssemblySymbol) = New UseSiteInfo(Of AssemblySymbol)()
			Dim highestPriorityUseSiteError As Integer = Me.HighestPriorityUseSiteError
			Dim enumerator As ImmutableArray(Of CustomModifier).Enumerator = customModifiers.GetEnumerator()
			Do
				If (Not enumerator.MoveNext()) Then
					Exit Do
				End If
				Dim current As CustomModifier = enumerator.Current
				If (Not current.IsOptional AndAlso (Not allowIsExternalInit OrElse Not DirectCast(current, VisualBasicCustomModifier).ModifierSymbol.IsWellKnownTypeIsExternalInit())) Then
					useSiteInfo = New UseSiteInfo(Of AssemblySymbol)(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedType1, New [Object]() { [String].Empty }))
					Me.GetSymbolSpecificUnsupportedMetadataUseSiteErrorInfo(useSiteInfo)
					If (Symbol.MergeUseSiteInfo(useSiteInfo1, useSiteInfo, highestPriorityUseSiteError)) Then
						Exit Do
					End If
				End If
				useSiteInfo = Me.DeriveUseSiteInfoFromType(DirectCast(current, VisualBasicCustomModifier).ModifierSymbol)
			Loop While Not Symbol.MergeUseSiteInfo(useSiteInfo1, useSiteInfo, highestPriorityUseSiteError)
			Return useSiteInfo1
		End Function

		Friend Function DeriveUseSiteInfoFromParameter(ByVal param As ParameterSymbol, ByVal highestPriorityUseSiteError As Integer) As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol)
			Dim code As Boolean
			Dim flag As Boolean
			Dim code1 As Boolean
			Dim useSiteInfo1 As UseSiteInfo(Of AssemblySymbol) = Me.DeriveUseSiteInfoFromType(param.Type)
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = useSiteInfo1.DiagnosticInfo
			If (diagnosticInfo IsNot Nothing) Then
				code = diagnosticInfo.Code = highestPriorityUseSiteError
			Else
				code = False
			End If
			If (Not code) Then
				Dim useSiteInfo2 As UseSiteInfo(Of AssemblySymbol) = Me.DeriveUseSiteInfoFromCustomModifiers(param.RefCustomModifiers, False)
				Dim diagnosticInfo1 As Microsoft.CodeAnalysis.DiagnosticInfo = useSiteInfo2.DiagnosticInfo
				If (diagnosticInfo1 IsNot Nothing) Then
					flag = diagnosticInfo1.Code = highestPriorityUseSiteError
				Else
					flag = False
				End If
				If (Not flag) Then
					Dim useSiteInfo3 As UseSiteInfo(Of AssemblySymbol) = Me.DeriveUseSiteInfoFromCustomModifiers(param.CustomModifiers, False)
					Dim diagnosticInfo2 As Microsoft.CodeAnalysis.DiagnosticInfo = useSiteInfo3.DiagnosticInfo
					If (diagnosticInfo2 IsNot Nothing) Then
						code1 = diagnosticInfo2.Code = highestPriorityUseSiteError
					Else
						code1 = False
					End If
					If (Not code1) Then
						Dim diagnosticInfo3 As Microsoft.CodeAnalysis.DiagnosticInfo = If(useSiteInfo1.DiagnosticInfo, (If(useSiteInfo2.DiagnosticInfo, useSiteInfo3.DiagnosticInfo)))
						If (diagnosticInfo3 Is Nothing) Then
							Dim primaryDependency As AssemblySymbol = useSiteInfo1.PrimaryDependency
							Dim secondaryDependencies As ImmutableHashSet(Of AssemblySymbol) = useSiteInfo1.SecondaryDependencies
							useSiteInfo2.MergeDependencies(primaryDependency, secondaryDependencies)
							useSiteInfo3.MergeDependencies(primaryDependency, secondaryDependencies)
							useSiteInfo = New UseSiteInfo(Of AssemblySymbol)(Nothing, primaryDependency, secondaryDependencies)
						Else
							useSiteInfo = New UseSiteInfo(Of AssemblySymbol)(diagnosticInfo3)
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

		Friend Function DeriveUseSiteInfoFromParameters(ByVal parameters As ImmutableArray(Of ParameterSymbol)) As UseSiteInfo(Of AssemblySymbol)
			Dim current As ParameterSymbol
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = New UseSiteInfo(Of AssemblySymbol)()
			Dim highestPriorityUseSiteError As Integer = Me.HighestPriorityUseSiteError
			Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = parameters.GetEnumerator()
			Do
				If (Not enumerator.MoveNext()) Then
					Exit Do
				End If
				current = enumerator.Current
			Loop While Not Symbol.MergeUseSiteInfo(useSiteInfo, Me.DeriveUseSiteInfoFromParameter(current, highestPriorityUseSiteError), highestPriorityUseSiteError)
			Return useSiteInfo
		End Function

		Friend Function DeriveUseSiteInfoFromType(ByVal type As TypeSymbol) As UseSiteInfo(Of AssemblySymbol)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = type.GetUseSiteInfo()
			If (useSiteInfo.DiagnosticInfo IsNot Nothing AndAlso useSiteInfo.DiagnosticInfo.Code = 30649) Then
				Me.GetSymbolSpecificUnsupportedMetadataUseSiteErrorInfo(useSiteInfo)
			End If
			Return useSiteInfo
		End Function

		Friend Function EarlyDecodeDeprecatedOrExperimentalOrObsoleteAttribute(ByRef arguments As EarlyDecodeWellKnownAttributeArguments(Of EarlyWellKnownAttributeBinder, NamedTypeSymbol, Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax, AttributeLocation), <Out> ByRef boundAttribute As VisualBasicAttributeData, <Out> ByRef obsoleteData As Microsoft.CodeAnalysis.ObsoleteAttributeData) As Boolean
			Dim flag As Boolean
			Dim obsoleteAttributeKind As Microsoft.CodeAnalysis.ObsoleteAttributeKind
			Dim flag1 As Boolean
			Dim attributeType As NamedTypeSymbol = arguments.AttributeType
			Dim attributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax = arguments.AttributeSyntax
			If (VisualBasicAttributeData.IsTargetEarlyAttribute(attributeType, attributeSyntax, AttributeDescription.ObsoleteAttribute)) Then
				obsoleteAttributeKind = Microsoft.CodeAnalysis.ObsoleteAttributeKind.Obsolete
			ElseIf (Not VisualBasicAttributeData.IsTargetEarlyAttribute(attributeType, attributeSyntax, AttributeDescription.DeprecatedAttribute)) Then
				If (VisualBasicAttributeData.IsTargetEarlyAttribute(attributeType, attributeSyntax, AttributeDescription.ExperimentalAttribute)) Then
					obsoleteAttributeKind = Microsoft.CodeAnalysis.ObsoleteAttributeKind.Experimental
					flag1 = False
					boundAttribute = arguments.Binder.GetAttribute(attributeSyntax, attributeType, flag1)
					If (boundAttribute.HasErrors) Then
						obsoleteData = Nothing
						boundAttribute = Nothing
					Else
						obsoleteData = boundAttribute.DecodeObsoleteAttribute(obsoleteAttributeKind)
						If (flag1) Then
							boundAttribute = Nothing
						End If
					End If
					flag = True
					Return flag
				End If
				boundAttribute = Nothing
				obsoleteData = Nothing
				flag = False
				Return flag
			Else
				obsoleteAttributeKind = Microsoft.CodeAnalysis.ObsoleteAttributeKind.Deprecated
			End If
			flag1 = False
			boundAttribute = arguments.Binder.GetAttribute(attributeSyntax, attributeType, flag1)
			If (boundAttribute.HasErrors) Then
				obsoleteData = Nothing
				boundAttribute = Nothing
			Else
				obsoleteData = boundAttribute.DecodeObsoleteAttribute(obsoleteAttributeKind)
				If (flag1) Then
					boundAttribute = Nothing
				End If
			End If
			flag = True
			Return flag
		End Function

		Friend Overridable Function EarlyDecodeWellKnownAttribute(ByRef arguments As EarlyDecodeWellKnownAttributeArguments(Of EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation)) As VisualBasicAttributeData
			Return Nothing
		End Function

		Private Function EarlyDecodeWellKnownAttributes(ByVal binders As ImmutableArray(Of Binder), ByVal boundAttributeTypes As ImmutableArray(Of NamedTypeSymbol), ByVal attributesToBind As ImmutableArray(Of AttributeSyntax), ByVal attributeBuilder As VisualBasicAttributeData(), ByVal symbolPart As AttributeLocation) As EarlyWellKnownAttributeData
			Dim earlyWellKnownAttributeBinder As EarlyDecodeWellKnownAttributeArguments(Of Microsoft.CodeAnalysis.VisualBasic.EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation) = New EarlyDecodeWellKnownAttributeArguments(Of Microsoft.CodeAnalysis.VisualBasic.EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation)() With
			{
				.SymbolPart = symbolPart
			}
			Dim length As Integer = boundAttributeTypes.Length - 1
			Dim num As Integer = 0
			Do
				Dim item As NamedTypeSymbol = boundAttributeTypes(num)
				If (Not item.IsErrorType()) Then
					earlyWellKnownAttributeBinder.Binder = New Microsoft.CodeAnalysis.VisualBasic.EarlyWellKnownAttributeBinder(Me, binders(num))
					earlyWellKnownAttributeBinder.AttributeType = item
					earlyWellKnownAttributeBinder.AttributeSyntax = attributesToBind(num)
					attributeBuilder(num) = Me.EarlyDecodeWellKnownAttribute(earlyWellKnownAttributeBinder)
				End If
				num = num + 1
			Loop While num <= length
			If (Not earlyWellKnownAttributeBinder.HasDecodedData) Then
				Return Nothing
			End If
			Return earlyWellKnownAttributeBinder.DecodedData
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return Me = obj
		End Function

		Public Overridable Function Equals(ByVal other As Symbol, ByVal compareKind As TypeCompareKind) As Boolean
			Return Me.Equals(other)
		End Function

		Friend Sub ForceCompleteObsoleteAttribute()
			If (Me.ObsoleteState = ThreeState.Unknown) Then
				Me.GetAttributes()
			End If
		End Sub

		Friend Overridable Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
		End Sub

		Friend Function GetAttributeBinder(ByVal syntaxList As SyntaxList(Of AttributeListSyntax), ByVal sourceModule As SourceModuleSymbol) As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = syntaxList.Node.SyntaxTree
			Dim parent As SyntaxNode = syntaxList.Node.Parent
			If (Not parent.IsKind(SyntaxKind.AttributesStatement) OrElse Not parent.Parent.IsKind(SyntaxKind.CompilationUnit)) Then
				binder = BinderBuilder.CreateBinderForAttribute(sourceModule, syntaxTree, Me)
			Else
				binder = BinderBuilder.CreateBinderForProjectLevelNamespace(sourceModule, syntaxTree)
			End If
			Return binder
		End Function

		Public Overridable Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return ImmutableArray(Of VisualBasicAttributeData).Empty
		End Function

		Private Function GetAttributesToBind(ByVal attributeDeclarationSyntaxLists As OneOrMany(Of SyntaxList(Of AttributeListSyntax)), ByVal symbolPart As AttributeLocation, ByVal compilation As VisualBasicCompilation, <Out> ByRef binders As ImmutableArray(Of Binder)) As ImmutableArray(Of AttributeSyntax)
			Dim empty As ImmutableArray(Of AttributeSyntax)
			Dim attributeTargetSymbol As IAttributeTargetSymbol = DirectCast(Me, IAttributeTargetSymbol)
			Dim sourceModule As SourceModuleSymbol = DirectCast(compilation.SourceModule, SourceModuleSymbol)
			Dim attributeSyntaxes As ArrayBuilder(Of AttributeSyntax) = Nothing
			Dim binders1 As ArrayBuilder(Of Binder) = Nothing
			Dim num As Integer = 0
			Dim count As Integer = attributeDeclarationSyntaxLists.Count - 1
			Dim num1 As Integer = 0
			Do
				Dim item As SyntaxList(Of AttributeListSyntax) = attributeDeclarationSyntaxLists(num1)
				If (item.Any()) Then
					Dim num2 As Integer = num
					Dim enumerator As SyntaxList(Of AttributeListSyntax).Enumerator = item.GetEnumerator()
					While enumerator.MoveNext()
						Dim enumerator1 As SeparatedSyntaxList(Of AttributeSyntax).Enumerator = enumerator.Current.Attributes.GetEnumerator()
						While enumerator1.MoveNext()
							Dim current As AttributeSyntax = enumerator1.Current
							If (Not Symbol.MatchAttributeTarget(attributeTargetSymbol, symbolPart, current.Target)) Then
								Continue While
							End If
							If (attributeSyntaxes Is Nothing) Then
								attributeSyntaxes = New ArrayBuilder(Of AttributeSyntax)()
								binders1 = New ArrayBuilder(Of Binder)()
							End If
							attributeSyntaxes.Add(current)
							num = num + 1
						End While
					End While
					If (num <> num2) Then
						Dim attributeBinder As Binder = Me.GetAttributeBinder(item, sourceModule)
						Dim num3 As Integer = num - num2 - 1
						For i As Integer = 0 To num3
							binders1.Add(attributeBinder)
						Next

					End If
				End If
				num1 = num1 + 1
			Loop While num1 <= count
			If (attributeSyntaxes Is Nothing) Then
				binders = ImmutableArray(Of Binder).Empty
				empty = ImmutableArray(Of AttributeSyntax).Empty
			Else
				binders = binders1.ToImmutableAndFree()
				empty = attributeSyntaxes.ToImmutableAndFree()
			End If
			Return empty
		End Function

		Private Shared Function GetAttributesToBind(ByVal attributeBlockSyntaxList As SyntaxList(Of AttributeListSyntax)) As ImmutableArray(Of AttributeSyntax)
			Dim attributeSyntaxes As ArrayBuilder(Of AttributeSyntax) = Nothing
			Symbol.GetAttributesToBind(attributeBlockSyntaxList, attributeSyntaxes)
			If (attributeSyntaxes Is Nothing) Then
				Return ImmutableArray(Of AttributeSyntax).Empty
			End If
			Return attributeSyntaxes.ToImmutableAndFree()
		End Function

		Friend Shared Sub GetAttributesToBind(ByVal attributeBlockSyntaxList As SyntaxList(Of AttributeListSyntax), ByRef attributeSyntaxBuilder As ArrayBuilder(Of AttributeSyntax))
			If (attributeBlockSyntaxList.Count > 0) Then
				If (attributeSyntaxBuilder Is Nothing) Then
					attributeSyntaxBuilder = ArrayBuilder(Of AttributeSyntax).GetInstance()
				End If
				Dim enumerator As SyntaxList(Of AttributeListSyntax).Enumerator = attributeBlockSyntaxList.GetEnumerator()
				While enumerator.MoveNext()
					attributeSyntaxBuilder.AddRange(DirectCast(enumerator.Current.Attributes, IEnumerable(Of AttributeSyntax)))
				End While
			End If
		End Sub

		Friend Function GetAttributeTarget() As AttributeTargets
			Dim attributeTarget As AttributeTargets
			Select Case Me.Kind
				Case SymbolKind.Assembly
					attributeTarget = AttributeTargets.Assembly
					Exit Select
				Case SymbolKind.DynamicType
				Case SymbolKind.ErrorType
				Case SymbolKind.Label
				Case SymbolKind.Local
				Case SymbolKind.[Namespace]
				Case SymbolKind.PointerType
				Case SymbolKind.RangeVariable
				Label0:
					attributeTarget = 0
					Exit Select
				Case SymbolKind.[Event]
					attributeTarget = AttributeTargets.[Event]
					Exit Select
				Case SymbolKind.Field
					attributeTarget = AttributeTargets.Field
					Exit Select
				Case SymbolKind.Method
					Select Case DirectCast(Me, MethodSymbol).MethodKind
						Case MethodKind.Constructor
						Case MethodKind.StaticConstructor
							attributeTarget = AttributeTargets.Constructor

						Case MethodKind.Conversion
						Case MethodKind.DelegateInvoke
						Case MethodKind.EventAdd
						Case MethodKind.EventRaise
						Case MethodKind.EventRemove
						Case MethodKind.UserDefinedOperator
						Case MethodKind.Ordinary
						Case MethodKind.PropertyGet
						Case MethodKind.PropertySet
						Case MethodKind.DeclareMethod
							attributeTarget = AttributeTargets.Method

						Case Else
							GoTo Label0
					End Select

				Case SymbolKind.NetModule
					attributeTarget = AttributeTargets.[Module]
					Exit Select
				Case SymbolKind.NamedType
					Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(Me, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
					Select Case namedTypeSymbol.TypeKind
						Case Microsoft.CodeAnalysis.TypeKind.[Class]
						Case Microsoft.CodeAnalysis.TypeKind.[Module]
							attributeTarget = AttributeTargets.[Class]

						Case Microsoft.CodeAnalysis.TypeKind.[Delegate]
							attributeTarget = AttributeTargets.[Delegate]

						Case Microsoft.CodeAnalysis.TypeKind.[Enum]
							attributeTarget = AttributeTargets.Struct Or AttributeTargets.[Enum]

						Case Microsoft.CodeAnalysis.TypeKind.[Interface]
							attributeTarget = AttributeTargets.[Interface]

						Case Microsoft.CodeAnalysis.TypeKind.Struct
							attributeTarget = AttributeTargets.Struct

						Case Microsoft.CodeAnalysis.TypeKind.Submission
							Throw ExceptionUtilities.UnexpectedValue(namedTypeSymbol.TypeKind)
						Case Else
							GoTo Label0
					End Select

				Case SymbolKind.Parameter
					attributeTarget = AttributeTargets.Parameter
					Exit Select
				Case SymbolKind.[Property]
					attributeTarget = AttributeTargets.[Property]
					Exit Select
				Case SymbolKind.TypeParameter
					attributeTarget = AttributeTargets.GenericParameter
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return attributeTarget
		End Function

		Friend Function GetCciAdapter() As Symbol
			Return Me
		End Function

		Friend Overridable Function GetCustomAttributesToEmit(ByVal compilationState As ModuleCompilationState) As IEnumerable(Of VisualBasicAttributeData)
			Return Me.GetCustomAttributesToEmit(compilationState, False)
		End Function

		Friend Function GetCustomAttributesToEmit(ByVal compilationState As ModuleCompilationState, ByVal emittingAssemblyAttributesInNetModule As Boolean) As IEnumerable(Of VisualBasicAttributeData)
			Dim synthesizedAttributeDatas As ArrayBuilder(Of SynthesizedAttributeData) = Nothing
			Me.AddSynthesizedAttributes(compilationState, synthesizedAttributeDatas)
			Return Me.GetCustomAttributesToEmit(Me.GetAttributes(), synthesizedAttributeDatas, False, emittingAssemblyAttributesInNetModule)
		End Function

		Friend Function GetCustomAttributesToEmit(ByVal userDefined As ImmutableArray(Of VisualBasicAttributeData), ByVal synthesized As ArrayBuilder(Of SynthesizedAttributeData), ByVal isReturnType As Boolean, ByVal emittingAssemblyAttributesInNetModule As Boolean) As IEnumerable(Of VisualBasicAttributeData)
			Dim visualBasicAttributeDatas As IEnumerable(Of VisualBasicAttributeData)
			visualBasicAttributeDatas = If(Not userDefined.IsEmpty OrElse synthesized IsNot Nothing, Me.GetCustomAttributesToEmitIterator(userDefined, synthesized, isReturnType, emittingAssemblyAttributesInNetModule), SpecializedCollections.EmptyEnumerable(Of VisualBasicAttributeData)())
			Return visualBasicAttributeDatas
		End Function

		Private Function GetCustomAttributesToEmitIterator(ByVal userDefined As ImmutableArray(Of VisualBasicAttributeData), ByVal synthesized As ArrayBuilder(Of SynthesizedAttributeData), ByVal isReturnType As Boolean, ByVal emittingAssemblyAttributesInNetModule As Boolean) As IEnumerable(Of VisualBasicAttributeData)
			Return New Symbol.VB$StateMachine_12_GetCustomAttributesToEmitIterator(-2) With
			{
				.$VB$Me = Me,
				.$P_userDefined = userDefined,
				.$P_synthesized = synthesized,
				.$P_isReturnType = isReturnType,
				.$P_emittingAssemblyAttributesInNetModule = emittingAssemblyAttributesInNetModule
			}
		End Function

		Private Function GetDebuggerDisplay() As String
			Return [String].Format("{0} {1}", Me.Kind, Me.ToDisplayString(SymbolDisplayFormat.TestFormat))
		End Function

		Friend Shared Function GetDeclaringSyntaxNodeHelper(Of TNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)(ByVal locations As ImmutableArray(Of Location)) As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)
			Dim immutableAndFree As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)
			If (Not locations.IsEmpty) Then
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode).GetInstance()
				Dim enumerator As ImmutableArray(Of Location).Enumerator = locations.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Location = enumerator.Current
					If (Not current.IsInSource OrElse current.SourceTree Is Nothing) Then
						Continue While
					End If
					Dim sourceTree As SyntaxTree = current.SourceTree
					Dim cancellationToken As System.Threading.CancellationToken = New System.Threading.CancellationToken()
					Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken = sourceTree.GetRoot(cancellationToken).FindToken(current.SourceSpan.Start, False)
					If (syntaxToken.Kind() = SyntaxKind.None) Then
						Continue While
					End If
					Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = DirectCast(syntaxToken.Parent.FirstAncestorOrSelf(Of TNode)(Nothing, True), Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)
					If (visualBasicSyntaxNode Is Nothing) Then
						Continue While
					End If
					instance.Add(visualBasicSyntaxNode)
				End While
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode).Empty
			End If
			Return immutableAndFree
		End Function

		Friend Shared Function GetDeclaringSyntaxReferenceHelper(Of TNode As VisualBasicSyntaxNode)(ByVal locations As ImmutableArray(Of Location)) As ImmutableArray(Of SyntaxReference)
			Dim immutableAndFree As ImmutableArray(Of SyntaxReference)
			Dim declaringSyntaxNodeHelper As ImmutableArray(Of VisualBasicSyntaxNode) = Symbol.GetDeclaringSyntaxNodeHelper(Of TNode)(locations)
			If (Not declaringSyntaxNodeHelper.IsEmpty) Then
				Dim instance As ArrayBuilder(Of SyntaxReference) = ArrayBuilder(Of SyntaxReference).GetInstance()
				Dim enumerator As ImmutableArray(Of VisualBasicSyntaxNode).Enumerator = declaringSyntaxNodeHelper.GetEnumerator()
				While enumerator.MoveNext()
					instance.Add(enumerator.Current.GetReference())
				End While
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = ImmutableArray(Of SyntaxReference).Empty
			End If
			Return immutableAndFree
		End Function

		Friend Shared Function GetDeclaringSyntaxReferenceHelper(ByVal references As ImmutableArray(Of SyntaxReference)) As ImmutableArray(Of SyntaxReference)
			Dim immutableAndFree As ImmutableArray(Of SyntaxReference)
			If (references.Length <> 1) Then
				Dim instance As ArrayBuilder(Of SyntaxReference) = ArrayBuilder(Of SyntaxReference).GetInstance()
				Dim enumerator As ImmutableArray(Of SyntaxReference).Enumerator = references.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As SyntaxReference = enumerator.Current
					If (current.SyntaxTree.IsEmbeddedOrMyTemplateTree()) Then
						Continue While
					End If
					instance.Add(New BeginOfBlockSyntaxReference(current))
				End While
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = Symbol.GetDeclaringSyntaxReferenceHelper(references(0))
			End If
			Return immutableAndFree
		End Function

		Friend Shared Function GetDeclaringSyntaxReferenceHelper(ByVal reference As SyntaxReference) As ImmutableArray(Of SyntaxReference)
			Dim empty As ImmutableArray(Of SyntaxReference)
			If (reference Is Nothing OrElse reference.SyntaxTree.IsEmbeddedOrMyTemplateTree()) Then
				empty = ImmutableArray(Of SyntaxReference).Empty
			Else
				empty = ImmutableArray.Create(Of SyntaxReference)(New BeginOfBlockSyntaxReference(reference))
			End If
			Return empty
		End Function

		Public Overridable Function GetDocumentationCommentId() As String Implements ISymbol.GetDocumentationCommentId
			Dim instance As PooledStringBuilder = PooledStringBuilder.GetInstance()
			DocumentationCommentIdVisitor.Instance.Visit(Me, instance.Builder)
			Dim stringAndFree As String = instance.ToStringAndFree()
			If (stringAndFree.Length <> 0) Then
				Return stringAndFree
			End If
			Return Nothing
		End Function

		Public Overridable Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String Implements ISymbol.GetDocumentationCommentXml
			Return ""
		End Function

		Friend Function GetGuidStringDefaultImplementation(<Out> ByRef guidString As String) As Boolean
			Dim flag As Boolean
			Dim enumerator As ImmutableArray(Of VisualBasicAttributeData).Enumerator = Me.GetAttributes().GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As VisualBasicAttributeData = enumerator.Current
					If (current.IsTargetAttribute(Me, AttributeDescription.GuidAttribute) AndAlso CommonAttributeDataExtensions.TryGetGuidAttributeValue(current, guidString)) Then
						flag = True
						Exit While
					End If
				Else
					guidString = Nothing
					flag = False
					Exit While
				End If
			End While
			Return flag
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return RuntimeHelpers.GetHashCode(Me)
		End Function

		Friend Overridable Function GetLexicalSortKey() As LexicalSortKey
			Dim locations As ImmutableArray(Of Location) = Me.Locations
			Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
			If (locations.Length <= 0) Then
				Return LexicalSortKey.NotInSource
			End If
			Return New LexicalSortKey(locations(0), declaringCompilation)
		End Function

		Private Sub GetSymbolSpecificUnsupportedMetadataUseSiteErrorInfo(ByRef useSiteInfo As UseSiteInfo(Of AssemblySymbol))
			Dim kind As SymbolKind = Me.Kind
			If (kind = SymbolKind.Field) Then
				useSiteInfo = New UseSiteInfo(Of AssemblySymbol)(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedField1, New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(Me) }))
				Return
			End If
			If (kind = SymbolKind.Method) Then
				useSiteInfo = New UseSiteInfo(Of AssemblySymbol)(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedMethod1, New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(Me) }))
				Return
			End If
			If (kind <> SymbolKind.[Property]) Then
				Return
			End If
			useSiteInfo = New UseSiteInfo(Of AssemblySymbol)(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedProperty1, New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(Me) }))
		End Sub

		Friend Shared Function GetUnificationUseSiteDiagnosticRecursive(Of T As TypeSymbol)(ByVal types As ImmutableArray(Of T), ByVal owner As Symbol, ByRef checkedTypes As HashSet(Of TypeSymbol)) As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim enumerator As ImmutableArray(Of T).Enumerator = types.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim unificationUseSiteDiagnosticRecursive As Microsoft.CodeAnalysis.DiagnosticInfo = enumerator.Current.GetUnificationUseSiteDiagnosticRecursive(owner, checkedTypes)
					If (unificationUseSiteDiagnosticRecursive IsNot Nothing) Then
						diagnosticInfo = unificationUseSiteDiagnosticRecursive
						Exit While
					End If
				Else
					diagnosticInfo = Nothing
					Exit While
				End If
			End While
			Return diagnosticInfo
		End Function

		Friend Shared Function GetUnificationUseSiteDiagnosticRecursive(ByVal modifiers As ImmutableArray(Of CustomModifier), ByVal owner As Symbol, ByRef checkedTypes As HashSet(Of TypeSymbol)) As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim enumerator As ImmutableArray(Of CustomModifier).Enumerator = modifiers.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim unificationUseSiteDiagnosticRecursive As Microsoft.CodeAnalysis.DiagnosticInfo = DirectCast(enumerator.Current.Modifier, TypeSymbol).GetUnificationUseSiteDiagnosticRecursive(owner, checkedTypes)
					If (unificationUseSiteDiagnosticRecursive IsNot Nothing) Then
						diagnosticInfo = unificationUseSiteDiagnosticRecursive
						Exit While
					End If
				Else
					diagnosticInfo = Nothing
					Exit While
				End If
			End While
			Return diagnosticInfo
		End Function

		Friend Shared Function GetUnificationUseSiteDiagnosticRecursive(ByVal parameters As ImmutableArray(Of ParameterSymbol), ByVal owner As Symbol, ByRef checkedTypes As HashSet(Of TypeSymbol)) As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = parameters.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As ParameterSymbol = enumerator.Current
					Dim unificationUseSiteDiagnosticRecursive As Microsoft.CodeAnalysis.DiagnosticInfo = If(current.Type.GetUnificationUseSiteDiagnosticRecursive(owner, checkedTypes), (If(Symbol.GetUnificationUseSiteDiagnosticRecursive(current.RefCustomModifiers, owner, checkedTypes), Symbol.GetUnificationUseSiteDiagnosticRecursive(current.CustomModifiers, owner, checkedTypes))))
					If (unificationUseSiteDiagnosticRecursive IsNot Nothing) Then
						diagnosticInfo = unificationUseSiteDiagnosticRecursive
						Exit While
					End If
				Else
					diagnosticInfo = Nothing
					Exit While
				End If
			End While
			Return diagnosticInfo
		End Function

		Friend Shared Function GetUnificationUseSiteDiagnosticRecursive(ByVal typeParameters As ImmutableArray(Of TypeParameterSymbol), ByVal owner As Symbol, ByRef checkedTypes As HashSet(Of TypeSymbol)) As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo
			Dim enumerator As ImmutableArray(Of TypeParameterSymbol).Enumerator = typeParameters.GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim unificationUseSiteDiagnosticRecursive As Microsoft.CodeAnalysis.DiagnosticInfo = Symbol.GetUnificationUseSiteDiagnosticRecursive(Of TypeSymbol)(enumerator.Current.ConstraintTypesNoUseSiteDiagnostics, owner, checkedTypes)
					If (unificationUseSiteDiagnosticRecursive IsNot Nothing) Then
						diagnosticInfo = unificationUseSiteDiagnosticRecursive
						Exit While
					End If
				Else
					diagnosticInfo = Nothing
					Exit While
				End If
			End While
			Return diagnosticInfo
		End Function

		Friend Overridable Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Return New UseSiteInfo(Of AssemblySymbol)()
		End Function

		Friend Shared Function HaveSameSignature(ByVal method1 As MethodSymbol, ByVal method2 As MethodSymbol) As Boolean
			Return CInt(MethodSignatureComparer.DetailedCompare(method1, method2, SymbolComparisonResults.NameMismatch Or SymbolComparisonResults.ArityMismatch Or SymbolComparisonResults.RequiredExtraParameterMismatch Or SymbolComparisonResults.RequiredParameterTypeMismatch Or SymbolComparisonResults.OptionalParameterTypeMismatch Or SymbolComparisonResults.PropertyInitOnlyMismatch Or SymbolComparisonResults.VarargMismatch Or SymbolComparisonResults.TotalParameterCountMismatch, 0)) = 0
		End Function

		Friend Shared Function HaveSameSignatureAndConstraintsAndReturnType(ByVal method1 As MethodSymbol, ByVal method2 As MethodSymbol) As Boolean
			Return MethodSignatureComparer.VisualBasicSignatureAndConstraintsAndReturnTypeComparer.Equals(method1, method2)
		End Function

		Private Function IEquatable_Equals(ByVal other As ISymbol) As Boolean
			Return Me.Equals(TryCast(other, Symbol), SymbolEqualityComparer.[Default].CompareKind)
		End Function

		Private Function IFormattable_ToString(ByVal format As String, ByVal formatProvider As IFormatProvider) As String Implements IFormattable.ToString
			Return Me.ToString()
		End Function

		Friend Overridable Function IReferenceAsDefinition(ByVal context As EmitContext) As IDefinition Implements IReference.AsDefinition
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overridable Sub IReferenceDispatch(ByVal visitor As MetadataVisitor) Implements IReference.Dispatch
			Throw ExceptionUtilities.Unreachable
		End Sub

		Private Function IReferenceGetAttributes(ByVal context As EmitContext) As IEnumerable(Of ICustomAttribute) Implements IReference.GetAttributes
			Return Me.AdaptedSymbol.GetCustomAttributesToEmit(DirectCast(context.[Module], PEModuleBuilder).CompilationState)
		End Function

		Private Function IReferenceGetInternalSymbol() As ISymbolInternal Implements IReference.GetInternalSymbol
			Return Me.AdaptedSymbol
		End Function

		Friend Overridable Function IsDefinedInSourceTree(ByVal tree As SyntaxTree, ByVal definedWithinSpan As Nullable(Of TextSpan), Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Boolean
			Dim flag As Boolean
			Dim declaringSyntaxReferences As ImmutableArray(Of SyntaxReference) = Me.DeclaringSyntaxReferences
			If (Not Me.IsImplicitlyDeclared OrElse declaringSyntaxReferences.Length <> 0) Then
				Dim enumerator As ImmutableArray(Of SyntaxReference).Enumerator = declaringSyntaxReferences.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As SyntaxReference = enumerator.Current
					cancellationToken.ThrowIfCancellationRequested()
					If (current.SyntaxTree <> tree OrElse definedWithinSpan.HasValue AndAlso Not current.Span.IntersectsWith(definedWithinSpan.Value)) Then
						Continue While
					End If
					flag = True
					Return flag
				End While
				flag = False
			Else
				flag = Me.ContainingSymbol.IsDefinedInSourceTree(tree, definedWithinSpan, cancellationToken)
			End If
			Return flag
		End Function

		Friend Shared Function IsDefinedInSourceTree(ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal tree As SyntaxTree, ByVal definedWithinSpan As Nullable(Of TextSpan), Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Boolean
			If (syntaxNode Is Nothing OrElse syntaxNode.SyntaxTree <> tree) Then
				Return False
			End If
			If (Not definedWithinSpan.HasValue) Then
				Return True
			End If
			Return definedWithinSpan.Value.IntersectsWith(syntaxNode.FullSpan)
		End Function

		Friend Function IsDefinitionOrDistinct() As Boolean
			If (Me.IsDefinition) Then
				Return True
			End If
			Return Not Me.Equals(Me.OriginalDefinition)
		End Function

		Friend Function IsFromCompilation(ByVal compilation As VisualBasicCompilation) As Boolean
			Return compilation = Me.DeclaringCompilation
		End Function

		Public Shared Function IsSymbolAccessible(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal within As NamedTypeSymbol, Optional ByVal throughTypeOpt As NamedTypeSymbol = Nothing) As Boolean
			If (symbol Is Nothing) Then
				Throw New ArgumentNullException("symbol")
			End If
			If (within Is Nothing) Then
				Throw New ArgumentNullException("within")
			End If
			Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
			Dim basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
			Return AccessCheck.IsSymbolAccessible(symbol, within, throughTypeOpt, discarded, basesBeingResolved)
		End Function

		Public Shared Function IsSymbolAccessible(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal within As AssemblySymbol) As Boolean
			If (symbol Is Nothing) Then
				Throw New ArgumentNullException("symbol")
			End If
			If (within Is Nothing) Then
				Throw New ArgumentNullException("within")
			End If
			Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
			Dim basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
			Return AccessCheck.IsSymbolAccessible(symbol, within, discarded, basesBeingResolved)
		End Function

		Private Function ISymbol_Equals(ByVal other As ISymbol, ByVal equalityComparer As SymbolEqualityComparer) As Boolean Implements ISymbol.Equals
			Return Me.Equals(TryCast(other, Symbol), equalityComparer.CompareKind)
		End Function

		Private Function ISymbol_GetAttributes() As ImmutableArray(Of AttributeData) Implements ISymbol.GetAttributes
			Return StaticCast(Of AttributeData).From(Of VisualBasicAttributeData)(Me.GetAttributes())
		End Function

		Private Function ISymbol_ToDisplayParts(Optional ByVal format As SymbolDisplayFormat = Nothing) As ImmutableArray(Of SymbolDisplayPart) Implements ISymbol.ToDisplayParts
			Return Microsoft.CodeAnalysis.VisualBasic.SymbolDisplay.ToDisplayParts(Me, format)
		End Function

		Private Function ISymbol_ToDisplayString(Optional ByVal format As SymbolDisplayFormat = Nothing) As String Implements ISymbol.ToDisplayString
			Return Microsoft.CodeAnalysis.VisualBasic.SymbolDisplay.ToDisplayString(Me, format)
		End Function

		Private Function ISymbol_ToMinimalDisplayParts(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal position As Integer, Optional ByVal format As SymbolDisplayFormat = Nothing) As ImmutableArray(Of SymbolDisplayPart) Implements ISymbol.ToMinimalDisplayParts
			Return Microsoft.CodeAnalysis.VisualBasic.SymbolDisplay.ToMinimalDisplayParts(Me, semanticModel, position, format)
		End Function

		Private Function ISymbol_ToMinimalDisplayString(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal position As Integer, Optional ByVal format As SymbolDisplayFormat = Nothing) As String Implements ISymbol.ToMinimalDisplayString
			Return Microsoft.CodeAnalysis.VisualBasic.SymbolDisplay.ToMinimalDisplayString(Me, semanticModel, position, format)
		End Function

		Private Function ISymbolInternal_Equals(ByVal other As ISymbolInternal, ByVal compareKind As TypeCompareKind) As Boolean Implements ISymbolInternal.Equals
			Return Me.Equals(TryCast(other, Symbol), compareKind)
		End Function

		Private Function ISymbolInternal_GetISymbol() As ISymbol Implements ISymbolInternal.GetISymbol
			Return Me
		End Function

		Private Function ISymbolInternalGetCciAdapter() As IReference Implements ISymbolInternal.GetCciAdapter
			Return Me.GetCciAdapter()
		End Function

		Friend Sub LoadAndValidateAttributes(ByVal attributeBlockSyntaxList As OneOrMany(Of SyntaxList(Of AttributeListSyntax)), ByRef lazyCustomAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData), Optional ByVal symbolPart As AttributeLocation = 0)
			Dim empty As ImmutableArray(Of VisualBasicAttributeData)
			Dim wellKnownAttributeDatum As WellKnownAttributeData
			Dim containingAssembly As Object
			Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
			If (Me.Kind = SymbolKind.Assembly) Then
				containingAssembly = Me
			Else
				containingAssembly = Me.ContainingAssembly
			End If
			Dim sourceAssemblySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceAssemblySymbol = DirectCast(containingAssembly, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceAssemblySymbol)
			Dim sourceModule As SourceModuleSymbol = sourceAssemblySymbol.SourceModule
			Dim declaringCompilation As VisualBasicCompilation = sourceAssemblySymbol.DeclaringCompilation
			Dim binders As ImmutableArray(Of Binder) = New ImmutableArray(Of Binder)()
			Dim attributesToBind As ImmutableArray(Of AttributeSyntax) = Me.GetAttributesToBind(attributeBlockSyntaxList, symbolPart, declaringCompilation, binders)
			If (Not System.Linq.ImmutableArrayExtensions.Any(Of AttributeSyntax)(attributesToBind)) Then
				empty = ImmutableArray(Of VisualBasicAttributeData).Empty
				wellKnownAttributeDatum = Nothing
				Interlocked.CompareExchange(Of CustomAttributesBag(Of VisualBasicAttributeData))(lazyCustomAttributesBag, CustomAttributesBag(Of VisualBasicAttributeData).WithEmptyData(), Nothing)
			Else
				If (lazyCustomAttributesBag Is Nothing) Then
					Interlocked.CompareExchange(Of CustomAttributesBag(Of VisualBasicAttributeData))(lazyCustomAttributesBag, New CustomAttributesBag(Of VisualBasicAttributeData)(), Nothing)
				End If
				Dim namedTypeSymbols As ImmutableArray(Of NamedTypeSymbol) = Binder.BindAttributeTypes(binders, attributesToBind, Me, instance)
				Dim visualBasicAttributeDataArray(namedTypeSymbols.Length - 1 + 1 - 1) As VisualBasicAttributeData
				Dim earlyWellKnownAttributeDatum As EarlyWellKnownAttributeData = Me.EarlyDecodeWellKnownAttributes(binders, namedTypeSymbols, attributesToBind, visualBasicAttributeDataArray, symbolPart)
				lazyCustomAttributesBag.SetEarlyDecodedWellKnownAttributeData(earlyWellKnownAttributeDatum)
				Binder.GetAttributes(binders, attributesToBind, namedTypeSymbols, visualBasicAttributeDataArray, Me, instance)
				empty = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of VisualBasicAttributeData)(visualBasicAttributeDataArray)
				wellKnownAttributeDatum = Me.ValidateAttributeUsageAndDecodeWellKnownAttributes(binders, attributesToBind, empty, instance, symbolPart)
				lazyCustomAttributesBag.SetDecodedWellKnownAttributeData(wellKnownAttributeDatum)
			End If
			Me.PostDecodeWellKnownAttributes(empty, attributesToBind, instance, symbolPart, wellKnownAttributeDatum)
			sourceModule.AtomicStoreAttributesAndDiagnostics(lazyCustomAttributesBag, empty, instance)
			instance.Free()
		End Sub

		Private Sub MarkEmbeddedAttributeTypeReference(ByVal attribute As VisualBasicAttributeData, ByVal nodeOpt As AttributeSyntax, ByVal compilation As VisualBasicCompilation)
			If (Not Me.IsEmbedded AndAlso attribute.AttributeClass.IsEmbedded AndAlso nodeOpt IsNot Nothing AndAlso compilation.ContainsSyntaxTree(nodeOpt.SyntaxTree)) Then
				compilation.EmbeddedSymbolManager.MarkSymbolAsReferenced(attribute.AttributeClass)
			End If
		End Sub

		Private Shared Function MatchAttributeTarget(ByVal attributeTarget As IAttributeTargetSymbol, ByVal symbolPart As Microsoft.CodeAnalysis.VisualBasic.Symbols.AttributeLocation, ByVal targetOpt As AttributeTargetSyntax) As Boolean
			Dim flag As Boolean
			Dim attributeLocation As Microsoft.CodeAnalysis.VisualBasic.Symbols.AttributeLocation
			If (targetOpt IsNot Nothing) Then
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = targetOpt.AttributeModifier.Kind()
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModuleKeyword) Then
					attributeLocation = Microsoft.CodeAnalysis.VisualBasic.Symbols.AttributeLocation.[Module]
				Else
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.AssemblyKeyword) Then
						Throw ExceptionUtilities.UnexpectedValue(targetOpt.AttributeModifier.Kind())
					End If
					attributeLocation = Microsoft.CodeAnalysis.VisualBasic.Symbols.AttributeLocation.Assembly
				End If
				flag = If(symbolPart <> Microsoft.CodeAnalysis.VisualBasic.Symbols.AttributeLocation.None, attributeLocation = symbolPart, attributeLocation = attributeTarget.DefaultAttributeLocation)
			Else
				flag = True
			End If
			Return flag
		End Function

		Friend Function MergeUseSiteInfo(ByVal first As UseSiteInfo(Of AssemblySymbol), ByVal second As UseSiteInfo(Of AssemblySymbol)) As UseSiteInfo(Of AssemblySymbol)
			Symbol.MergeUseSiteInfo(first, second, Me.HighestPriorityUseSiteError)
			Return first
		End Function

		Friend Shared Function MergeUseSiteInfo(ByRef result As UseSiteInfo(Of AssemblySymbol), ByVal other As UseSiteInfo(Of AssemblySymbol), ByVal highestPriorityUseSiteError As Integer) As Boolean
			Dim code As Boolean
			Dim flag As Boolean
			Dim diagnosticInfo As Microsoft.CodeAnalysis.DiagnosticInfo = other.DiagnosticInfo
			If (diagnosticInfo IsNot Nothing) Then
				flag = diagnosticInfo.Code = highestPriorityUseSiteError
			Else
				flag = False
			End If
			If (flag) Then
				result = other
				code = True
			ElseIf (result.DiagnosticInfo IsNot Nothing) Then
				code = result.DiagnosticInfo.Code = highestPriorityUseSiteError
			Else
				If (other.DiagnosticInfo Is Nothing) Then
					Dim primaryDependency As AssemblySymbol = result.PrimaryDependency
					Dim secondaryDependencies As ImmutableHashSet(Of AssemblySymbol) = result.SecondaryDependencies
					other.MergeDependencies(primaryDependency, secondaryDependencies)
					result = New UseSiteInfo(Of AssemblySymbol)(Nothing, primaryDependency, secondaryDependencies)
				Else
					result = other
				End If
				code = False
			End If
			Return code
		End Function

		Public Shared Operator =(ByVal left As Symbol, ByVal right As Symbol) As Boolean
			Dim flag As Boolean
			If (right IsNot Nothing) Then
				flag = If(CObj(left) = CObj(right), True, right.Equals(left))
			Else
				flag = left Is Nothing
			End If
			Return flag
		End Operator

		Public Shared Operator <>(ByVal left As Symbol, ByVal right As Symbol) As Boolean
			Dim flag As Boolean
			If (right IsNot Nothing) Then
				flag = If(CObj(left) = CObj(right), False, Not right.Equals(left))
			Else
				flag = CObj(left) <> CObj(Nothing)
			End If
			Return flag
		End Operator

		Friend Overridable Sub PostDecodeWellKnownAttributes(ByVal boundAttributes As ImmutableArray(Of VisualBasicAttributeData), ByVal allAttributeSyntaxNodes As ImmutableArray(Of AttributeSyntax), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal symbolPart As AttributeLocation, ByVal decodedData As WellKnownAttributeData)
		End Sub

		Private Sub ReportExtensionAttributeUseSiteInfo(ByVal attribute As VisualBasicAttributeData, ByVal nodeOpt As AttributeSyntax, ByVal compilation As VisualBasicCompilation, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = New UseSiteInfo(Of AssemblySymbol)()
			If (attribute.AttributeConstructor IsNot Nothing AndAlso CObj(attribute.AttributeConstructor) = CObj(compilation.GetExtensionAttributeConstructor(useSiteInfo))) Then
				diagnostics.Add(useSiteInfo, If(nodeOpt IsNot Nothing, nodeOpt.GetLocation(), NoLocation.Singleton))
			End If
		End Sub

		Friend Overridable Sub SetMetadataName(ByVal metadataName As String)
			Throw ExceptionUtilities.Unreachable
		End Sub

		Public Function ToDisplayParts(Optional ByVal format As SymbolDisplayFormat = Nothing) As ImmutableArray(Of SymbolDisplayPart)
			Return Microsoft.CodeAnalysis.VisualBasic.SymbolDisplay.ToDisplayParts(Me, format)
		End Function

		Public Function ToDisplayString(Optional ByVal format As SymbolDisplayFormat = Nothing) As String
			Return Microsoft.CodeAnalysis.VisualBasic.SymbolDisplay.ToDisplayString(Me, format)
		End Function

		Public Function ToMinimalDisplayParts(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal position As Integer, Optional ByVal format As SymbolDisplayFormat = Nothing) As ImmutableArray(Of SymbolDisplayPart)
			Return Microsoft.CodeAnalysis.VisualBasic.SymbolDisplay.ToMinimalDisplayParts(Me, semanticModel, position, format)
		End Function

		Public Function ToMinimalDisplayString(ByVal semanticModel As Microsoft.CodeAnalysis.SemanticModel, ByVal position As Integer, Optional ByVal format As SymbolDisplayFormat = Nothing) As String
			Return Microsoft.CodeAnalysis.VisualBasic.SymbolDisplay.ToMinimalDisplayString(Me, semanticModel, position, format)
		End Function

		Public NotOverridable Overrides Function ToString() As String
			Return Me.ToDisplayString(SymbolDisplayFormat.VisualBasicErrorMessageFormat)
		End Function

		Private Function ValidateAttributeUsage(ByVal attribute As VisualBasicAttributeData, ByVal node As AttributeSyntax, ByVal compilation As VisualBasicCompilation, ByVal symbolPart As AttributeLocation, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal uniqueAttributeTypes As HashSet(Of NamedTypeSymbol)) As Boolean
			Dim flag As Boolean
			Dim attributeTarget As AttributeTargets
			Dim flag1 As Boolean
			Dim str As String
			Dim attributeClass As NamedTypeSymbol = attribute.AttributeClass
			Dim attributeUsageInfo As Microsoft.CodeAnalysis.AttributeUsageInfo = attributeClass.GetAttributeUsageInfo()
			If (uniqueAttributeTypes.Add(attributeClass) OrElse attributeUsageInfo.AllowMultiple) Then
				attributeTarget = If(symbolPart <> AttributeLocation.[Return], Me.GetAttributeTarget(), AttributeTargets.ReturnValue)
				If (CObj(attributeClass) <> CObj(compilation.GetWellKnownType(WellKnownType.System_NonSerializedAttribute)) OrElse Me.Kind <> SymbolKind.[Event] OrElse DirectCast(Me, SourceEventSymbol).AssociatedField Is Nothing) Then
					Dim validTargets As AttributeTargets = attributeUsageInfo.ValidTargets
					flag1 = If(CInt(attributeTarget) = 0, False, CInt((validTargets And attributeTarget)) <> 0)
				Else
					flag1 = True
				End If
				If (flag1) Then
					If (attribute.IsSecurityAttribute(compilation)) Then
						Dim kind As SymbolKind = Me.Kind
						If (kind = SymbolKind.Assembly OrElse kind = SymbolKind.Method OrElse kind = SymbolKind.NamedType) Then
							GoTo Label1
						End If
						diagnostics.Add(ERRID.ERR_SecurityAttributeInvalidTarget, node.Name.GetLocation(), New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(attributeClass) })
						flag = False
						Return flag
					End If
				Label1:
					flag = True
				Else
					If (attributeTarget <= AttributeTargets.[Module]) Then
						If (attributeTarget = AttributeTargets.Assembly) Then
							diagnostics.Add(ERRID.ERR_InvalidAssemblyAttribute1, node.Name.GetLocation(), New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(attributeClass) })
						Else
							If (attributeTarget <> AttributeTargets.[Module]) Then
								GoTo Label2
							End If
							diagnostics.Add(ERRID.ERR_InvalidModuleAttribute1, node.Name.GetLocation(), New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(attributeClass) })
						End If
					ElseIf (attributeTarget = AttributeTargets.Method) Then
						If (Me.Kind = SymbolKind.Method) Then
							Dim sourceMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol = DirectCast(Me, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMethodSymbol)
							Dim str1 As String = sourceMethodSymbol.MethodKind.TryGetAccessorDisplayName()
							If (str1 Is Nothing) Then
								GoTo Label4
							End If
							diagnostics.Add(ERRID.ERR_InvalidAttributeUsageOnAccessor, node.Name.GetLocation(), New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(attributeClass), str1, CustomSymbolDisplayFormatter.ShortErrorName(sourceMethodSymbol.AssociatedSymbol) })
							GoTo Label3
						End If
					Label4:
						Dim location As Microsoft.CodeAnalysis.Location = node.Name.GetLocation()
						diagnostics.Add(ERRID.ERR_InvalidAttributeUsage2, location, New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(attributeClass), CustomSymbolDisplayFormatter.ShortErrorName(Me).ToString() })
					ElseIf (attributeTarget = AttributeTargets.Field) Then
						Dim sourceWithEventsBackingFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceWithEventsBackingFieldSymbol = TryCast(Me, Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceWithEventsBackingFieldSymbol)
						str = If(sourceWithEventsBackingFieldSymbol Is Nothing, CustomSymbolDisplayFormatter.ShortErrorName(Me).ToString(), CustomSymbolDisplayFormatter.ShortErrorName(sourceWithEventsBackingFieldSymbol.AssociatedSymbol).ToString())
						diagnostics.Add(ERRID.ERR_InvalidAttributeUsage2, node.Name.GetLocation(), New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(attributeClass), str })
					Else
						If (attributeTarget <> AttributeTargets.ReturnValue) Then
							GoTo Label2
						End If
						diagnostics.Add(ERRID.ERR_InvalidAttributeUsage2, node.Name.GetLocation(), New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(attributeClass), New LocalizableErrorArgument(ERRID.IDS_FunctionReturnType) })
					End If
				Label3:
					flag = False
				End If
			Else
				diagnostics.Add(ERRID.ERR_InvalidMultipleAttributeUsage1, node.GetLocation(), New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(attributeClass) })
				flag = False
			End If
			Return flag
		Label2:
			Dim location1 As Microsoft.CodeAnalysis.Location = node.Name.GetLocation()
			diagnostics.Add(ERRID.ERR_InvalidAttributeUsage2, location1, New [Object]() { CustomSymbolDisplayFormatter.ShortErrorName(attributeClass), CustomSymbolDisplayFormatter.ShortErrorName(Me).ToString() })
			GoTo Label3
		End Function

		Friend Function ValidateAttributeUsageAndDecodeWellKnownAttributes(ByVal binders As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Binder), ByVal attributeSyntaxList As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax), ByVal boundAttributes As ImmutableArray(Of VisualBasicAttributeData), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal symbolPart As AttributeLocation) As WellKnownAttributeData
			Dim length As Integer = boundAttributes.Length
			Dim namedTypeSymbols As HashSet(Of NamedTypeSymbol) = New HashSet(Of NamedTypeSymbol)()
			Dim decodeWellKnownAttributeArgument As DecodeWellKnownAttributeArguments(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax, VisualBasicAttributeData, AttributeLocation) = New DecodeWellKnownAttributeArguments(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax, VisualBasicAttributeData, AttributeLocation)() With
			{
				.AttributesCount = length,
				.Diagnostics = diagnostics,
				.SymbolPart = symbolPart
			}
			Dim num As Integer = length - 1
			Dim num1 As Integer = 0
			Do
				Dim item As VisualBasicAttributeData = boundAttributes(num1)
				Dim attributeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AttributeSyntax = attributeSyntaxList(num1)
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = binders(num1)
				If (Not item.HasErrors AndAlso Me.ValidateAttributeUsage(item, attributeSyntax, binder.Compilation, symbolPart, diagnostics, namedTypeSymbols)) Then
					decodeWellKnownAttributeArgument.Attribute = item
					decodeWellKnownAttributeArgument.AttributeSyntaxOpt = attributeSyntax
					decodeWellKnownAttributeArgument.Index = num1
					Me.DecodeWellKnownAttribute(decodeWellKnownAttributeArgument)
				End If
				num1 = num1 + 1
			Loop While num1 <= num
			If (Not decodeWellKnownAttributeArgument.HasDecodedData) Then
				Return Nothing
			End If
			Return decodeWellKnownAttributeArgument.DecodedData
		End Function
	End Class
End Namespace