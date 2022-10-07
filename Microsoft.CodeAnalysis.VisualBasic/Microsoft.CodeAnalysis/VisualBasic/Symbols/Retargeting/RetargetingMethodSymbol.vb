Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Reflection
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
	Friend NotInheritable Class RetargetingMethodSymbol
		Inherits MethodSymbol
		Private ReadOnly _retargetingModule As RetargetingModuleSymbol

		Private ReadOnly _underlyingMethod As MethodSymbol

		Private _lazyTypeParameters As ImmutableArray(Of TypeParameterSymbol)

		Private _lazyParameters As ImmutableArray(Of ParameterSymbol)

		Private _lazyCustomModifiers As Microsoft.CodeAnalysis.CustomModifiersTuple

		Private _lazyCustomAttributes As ImmutableArray(Of VisualBasicAttributeData)

		Private _lazyReturnTypeCustomAttributes As ImmutableArray(Of VisualBasicAttributeData)

		Private _lazyExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)

		Private _lazyCachedUseSiteInfo As CachedUseSiteInfo(Of AssemblySymbol)

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return Me._underlyingMethod.Arity
			End Get
		End Property

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me._underlyingMethod.AssociatedSymbol
				If (symbol Is Nothing) Then
					Return Nothing
				End If
				Return Me.RetargetingTranslator.Retarget(symbol)
			End Get
		End Property

		Friend Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
			Get
				Return Me._underlyingMethod.CallingConvention
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Return Me._retargetingModule.ContainingAssembly
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingModule As ModuleSymbol
			Get
				Return Me._retargetingModule
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me.RetargetingTranslator.Retarget(Me._underlyingMethod.ContainingSymbol)
			End Get
		End Property

		Private ReadOnly Property CustomModifiersTuple As Microsoft.CodeAnalysis.CustomModifiersTuple
			Get
				Return Me.RetargetingTranslator.RetargetModifiers(Me._underlyingMethod.ReturnTypeCustomModifiers, Me._underlyingMethod.RefCustomModifiers, Me._lazyCustomModifiers)
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Me._underlyingMethod.DeclaredAccessibility
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me._underlyingMethod.DeclaringSyntaxReferences
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
			Get
				If (Me._lazyExplicitInterfaceImplementations.IsDefault) Then
					Dim methodSymbols As ImmutableArray(Of MethodSymbol) = Me.RetargetExplicitInterfaceImplementations()
					Dim methodSymbols1 As ImmutableArray(Of MethodSymbol) = New ImmutableArray(Of MethodSymbol)()
					ImmutableInterlocked.InterlockedCompareExchange(Of MethodSymbol)(Me._lazyExplicitInterfaceImplementations, methodSymbols, methodSymbols1)
				End If
				Return Me._lazyExplicitInterfaceImplementations
			End Get
		End Property

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
			Get
				Return Me._underlyingMethod.HasDeclarativeSecurity
			End Get
		End Property

		Friend Overrides ReadOnly Property HasRuntimeSpecialName As Boolean
			Get
				Return Me._underlyingMethod.HasRuntimeSpecialName
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return Me._underlyingMethod.HasSpecialName
			End Get
		End Property

		Friend Overrides ReadOnly Property ImplementationAttributes As MethodImplAttributes
			Get
				Return Me._underlyingMethod.ImplementationAttributes
			End Get
		End Property

		Friend Overrides ReadOnly Property IsAccessCheckedOnOverride As Boolean
			Get
				Return Me._underlyingMethod.IsAccessCheckedOnOverride
			End Get
		End Property

		Public Overrides ReadOnly Property IsAsync As Boolean
			Get
				Return Me._underlyingMethod.IsAsync
			End Get
		End Property

		Public Overrides ReadOnly Property IsExtensionMethod As Boolean
			Get
				Return Me._underlyingMethod.IsExtensionMethod
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExternal As Boolean
			Get
				Return Me._underlyingMethod.IsExternal
			End Get
		End Property

		Public Overrides ReadOnly Property IsExternalMethod As Boolean
			Get
				Return Me._underlyingMethod.IsExternalMethod
			End Get
		End Property

		Public Overrides ReadOnly Property IsGenericMethod As Boolean
			Get
				Return Me._underlyingMethod.IsGenericMethod
			End Get
		End Property

		Friend Overrides ReadOnly Property IsHiddenBySignature As Boolean
			Get
				Return Me._underlyingMethod.IsHiddenBySignature
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me._underlyingMethod.IsImplicitlyDeclared
			End Get
		End Property

		Public Overrides ReadOnly Property IsInitOnly As Boolean
			Get
				Return Me._underlyingMethod.IsInitOnly
			End Get
		End Property

		Public Overrides ReadOnly Property IsIterator As Boolean
			Get
				Return Me._underlyingMethod.IsIterator
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataFinal As Boolean
			Get
				Return Me._underlyingMethod.IsMetadataFinal
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMethodKindBasedOnSyntax As Boolean
			Get
				Return Me._underlyingMethod.IsMethodKindBasedOnSyntax
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return Me._underlyingMethod.IsMustOverride
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return Me._underlyingMethod.IsNotOverridable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverloads As Boolean
			Get
				Return Me._underlyingMethod.IsOverloads
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return Me._underlyingMethod.IsOverridable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return Me._underlyingMethod.IsOverrides
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return Me._underlyingMethod.IsShared
			End Get
		End Property

		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				Return Me._underlyingMethod.IsSub
			End Get
		End Property

		Public Overrides ReadOnly Property IsVararg As Boolean
			Get
				Return Me._underlyingMethod.IsVararg
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._underlyingMethod.Locations
			End Get
		End Property

		Friend Overrides ReadOnly Property MayBeReducibleExtensionMethod As Boolean
			Get
				Return Me._underlyingMethod.MayBeReducibleExtensionMethod
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				Return Me._underlyingMethod.MetadataName
			End Get
		End Property

		Public Overrides ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind
			Get
				Return Me._underlyingMethod.MethodKind
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._underlyingMethod.Name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Me._underlyingMethod.ObsoleteAttributeData
			End Get
		End Property

		Friend Overrides ReadOnly Property ParameterCount As Integer
			Get
				Return Me._underlyingMethod.ParameterCount
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				If (Me._lazyParameters.IsDefault) Then
					Dim parameterSymbols As ImmutableArray(Of ParameterSymbol) = Me.RetargetParameters()
					Dim parameterSymbols1 As ImmutableArray(Of ParameterSymbol) = New ImmutableArray(Of ParameterSymbol)()
					ImmutableInterlocked.InterlockedCompareExchange(Of ParameterSymbol)(Me._lazyParameters, parameterSymbols, parameterSymbols1)
				End If
				Return Me._lazyParameters
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me.CustomModifiersTuple.RefCustomModifiers
			End Get
		End Property

		Public ReadOnly Property RetargetingModule As RetargetingModuleSymbol
			Get
				Return Me._retargetingModule
			End Get
		End Property

		Private ReadOnly Property RetargetingTranslator As RetargetingModuleSymbol.RetargetingSymbolTranslator
			Get
				Return Me._retargetingModule.RetargetingTranslator
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnsByRef As Boolean
			Get
				Return Me._underlyingMethod.ReturnsByRef
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me.RetargetingTranslator.Retarget(Me._underlyingMethod.ReturnType, RetargetOptions.RetargetPrimitiveTypesByTypeCode)
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnTypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me.CustomModifiersTuple.TypeCustomModifiers
			End Get
		End Property

		Friend Overrides ReadOnly Property ReturnTypeMarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Me.RetargetingTranslator.Retarget(Me._underlyingMethod.ReturnTypeMarshallingInformation)
			End Get
		End Property

		Friend Overrides ReadOnly Property ReturnValueIsMarshalledExplicitly As Boolean
			Get
				Return Me._underlyingMethod.ReturnValueIsMarshalledExplicitly
			End Get
		End Property

		Friend Overrides ReadOnly Property ReturnValueMarshallingDescriptor As ImmutableArray(Of Byte)
			Get
				Return Me._underlyingMethod.ReturnValueMarshallingDescriptor
			End Get
		End Property

		Friend Overrides ReadOnly Property Syntax As SyntaxNode
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property TypeArguments As ImmutableArray(Of TypeSymbol)
			Get
				Dim empty As ImmutableArray(Of TypeSymbol)
				If (Not Me.IsGenericMethod) Then
					empty = ImmutableArray(Of TypeSymbol).Empty
				Else
					empty = StaticCast(Of TypeSymbol).From(Of TypeParameterSymbol)(Me.TypeParameters)
				End If
				Return empty
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				If (Me._lazyTypeParameters.IsDefault) Then
					If (Me.IsGenericMethod) Then
						Dim typeParameterSymbols As ImmutableArray(Of TypeParameterSymbol) = Me.RetargetingTranslator.Retarget(Me._underlyingMethod.TypeParameters)
						Dim typeParameterSymbols1 As ImmutableArray(Of TypeParameterSymbol) = New ImmutableArray(Of TypeParameterSymbol)()
						ImmutableInterlocked.InterlockedCompareExchange(Of TypeParameterSymbol)(Me._lazyTypeParameters, typeParameterSymbols, typeParameterSymbols1)
					Else
						Me._lazyTypeParameters = ImmutableArray(Of TypeParameterSymbol).Empty
					End If
				End If
				Return Me._lazyTypeParameters
			End Get
		End Property

		Public ReadOnly Property UnderlyingMethod As MethodSymbol
			Get
				Return Me._underlyingMethod
			End Get
		End Property

		Public Sub New(ByVal retargetingModule As RetargetingModuleSymbol, ByVal underlyingMethod As MethodSymbol)
			MyBase.New()
			Me._lazyCachedUseSiteInfo = CachedUseSiteInfo(Of AssemblySymbol).Uninitialized
			If (TypeOf underlyingMethod Is RetargetingMethodSymbol) Then
				Throw New ArgumentException()
			End If
			Me._retargetingModule = retargetingModule
			Me._underlyingMethod = underlyingMethod
		End Sub

		Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			Return Me._underlyingMethod.GetAppliedConditionalSymbols()
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.RetargetingTranslator.GetRetargetedAttributes(Me._underlyingMethod, Me._lazyCustomAttributes, False)
		End Function

		Friend Overrides Function GetCustomAttributesToEmit(ByVal compilationState As ModuleCompilationState) As IEnumerable(Of VisualBasicAttributeData)
			Return Me.RetargetingTranslator.RetargetAttributes(Me._underlyingMethod.GetCustomAttributesToEmit(compilationState))
		End Function

		Public Overrides Function GetDllImportData() As DllImportData
			Return Me._underlyingMethod.GetDllImportData()
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._underlyingMethod.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function

		Public Overrides Function GetReturnTypeAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.RetargetingTranslator.GetRetargetedAttributes(Me._underlyingMethod, Me._lazyReturnTypeCustomAttributes, True)
		End Function

		Friend Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
			Return Me._underlyingMethod.GetSecurityInformation()
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim primaryDependency As AssemblySymbol = MyBase.PrimaryDependency
			If (Not Me._lazyCachedUseSiteInfo.IsInitialized) Then
				Me._lazyCachedUseSiteInfo.Initialize(primaryDependency, MyBase.CalculateUseSiteInfo())
			End If
			Return Me._lazyCachedUseSiteInfo.ToUseSiteInfo(primaryDependency)
		End Function

		Friend Overrides Function IsMetadataNewSlot(Optional ByVal ignoreInterfaceImplementationChanges As Boolean = False) As Boolean
			Return Me._underlyingMethod.IsMetadataNewSlot(ignoreInterfaceImplementationChanges)
		End Function

		Private Function RetargetExplicitInterfaceImplementations() As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			Dim immutableAndFree As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			Dim explicitInterfaceImplementations As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) = Me.UnderlyingMethod.ExplicitInterfaceImplementations
			If (Not explicitInterfaceImplementations.IsEmpty) Then
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).GetInstance()
				Dim length As Integer = explicitInterfaceImplementations.Length - 1
				Dim num As Integer = 0
				Do
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me.RetargetingTranslator.Retarget(explicitInterfaceImplementations(num), MethodSignatureComparer.RetargetedExplicitMethodImplementationComparer)
					If (methodSymbol IsNot Nothing) Then
						instance.Add(methodSymbol)
					End If
					num = num + 1
				Loop While num <= length
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = explicitInterfaceImplementations
			End If
			Return immutableAndFree
		End Function

		Private Function RetargetParameters() As ImmutableArray(Of ParameterSymbol)
			Dim empty As ImmutableArray(Of ParameterSymbol)
			Dim parameters As ImmutableArray(Of ParameterSymbol) = Me._underlyingMethod.Parameters
			Dim length As Integer = parameters.Length
			If (length <> 0) Then
				Dim parameterSymbolArray(length - 1 + 1 - 1) As ParameterSymbol
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				Do
					parameterSymbolArray(num1) = RetargetingParameterSymbol.CreateMethodParameter(Me, parameters(num1))
					num1 = num1 + 1
				Loop While num1 <= num
				empty = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of ParameterSymbol)(parameterSymbolArray)
			Else
				empty = ImmutableArray(Of ParameterSymbol).Empty
			End If
			Return empty
		End Function
	End Class
End Namespace