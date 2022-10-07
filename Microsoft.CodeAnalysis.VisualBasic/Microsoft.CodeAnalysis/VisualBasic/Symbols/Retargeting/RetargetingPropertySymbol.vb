Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
	Friend NotInheritable Class RetargetingPropertySymbol
		Inherits PropertySymbol
		Private ReadOnly _retargetingModule As RetargetingModuleSymbol

		Private ReadOnly _underlyingProperty As PropertySymbol

		Private _lazyParameters As ImmutableArray(Of ParameterSymbol)

		Private _lazyCustomModifiers As Microsoft.CodeAnalysis.CustomModifiersTuple

		Private _lazyCustomAttributes As ImmutableArray(Of VisualBasicAttributeData)

		Private _lazyExplicitInterfaceImplementations As ImmutableArray(Of PropertySymbol)

		Private _lazyCachedUseSiteInfo As CachedUseSiteInfo(Of AssemblySymbol)

		Friend Overrides ReadOnly Property AssociatedField As FieldSymbol
			Get
				If (Me._underlyingProperty.AssociatedField Is Nothing) Then
					Return Nothing
				End If
				Return Me.RetargetingTranslator.Retarget(Me._underlyingProperty.AssociatedField)
			End Get
		End Property

		Friend Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
			Get
				Return Me._underlyingProperty.CallingConvention
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me.RetargetingTranslator.Retarget(Me._underlyingProperty.ContainingSymbol)
			End Get
		End Property

		Private ReadOnly Property CustomModifiersTuple As Microsoft.CodeAnalysis.CustomModifiersTuple
			Get
				Return Me.RetargetingTranslator.RetargetModifiers(Me._underlyingProperty.TypeCustomModifiers, Me._underlyingProperty.RefCustomModifiers, Me._lazyCustomModifiers)
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Me._underlyingProperty.DeclaredAccessibility
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me._underlyingProperty.DeclaringSyntaxReferences
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of PropertySymbol)
			Get
				If (Me._lazyExplicitInterfaceImplementations.IsDefault) Then
					Dim propertySymbols As ImmutableArray(Of PropertySymbol) = Me.RetargetExplicitInterfaceImplementations()
					Dim propertySymbols1 As ImmutableArray(Of PropertySymbol) = New ImmutableArray(Of PropertySymbol)()
					ImmutableInterlocked.InterlockedCompareExchange(Of PropertySymbol)(Me._lazyExplicitInterfaceImplementations, propertySymbols, propertySymbols1)
				End If
				Return Me._lazyExplicitInterfaceImplementations
			End Get
		End Property

		Public Overrides ReadOnly Property GetMethod As MethodSymbol
			Get
				If (Me._underlyingProperty.GetMethod Is Nothing) Then
					Return Nothing
				End If
				Return Me.RetargetingTranslator.Retarget(Me._underlyingProperty.GetMethod)
			End Get
		End Property

		Friend Overrides ReadOnly Property HasRuntimeSpecialName As Boolean
			Get
				Return Me._underlyingProperty.HasRuntimeSpecialName
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return Me._underlyingProperty.HasSpecialName
			End Get
		End Property

		Public Overrides ReadOnly Property IsDefault As Boolean
			Get
				Return Me._underlyingProperty.IsDefault
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me._underlyingProperty.IsImplicitlyDeclared
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return Me._underlyingProperty.IsMustOverride
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMyGroupCollectionProperty As Boolean
			Get
				Return Me._underlyingProperty.IsMyGroupCollectionProperty
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return Me._underlyingProperty.IsNotOverridable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverloads As Boolean
			Get
				Return Me._underlyingProperty.IsOverloads
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return Me._underlyingProperty.IsOverridable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return Me._underlyingProperty.IsOverrides
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return Me._underlyingProperty.IsShared
			End Get
		End Property

		Public Overrides ReadOnly Property IsWithEvents As Boolean
			Get
				Return Me._underlyingProperty.IsWithEvents
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._underlyingProperty.Locations
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				Return Me._underlyingProperty.MetadataName
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._underlyingProperty.Name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Me._underlyingProperty.ObsoleteAttributeData
			End Get
		End Property

		Public Overrides ReadOnly Property ParameterCount As Integer
			Get
				Return Me._underlyingProperty.ParameterCount
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
				Return Me._underlyingProperty.ReturnsByRef
			End Get
		End Property

		Public Overrides ReadOnly Property SetMethod As MethodSymbol
			Get
				If (Me._underlyingProperty.SetMethod Is Nothing) Then
					Return Nothing
				End If
				Return Me.RetargetingTranslator.Retarget(Me._underlyingProperty.SetMethod)
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me.RetargetingTranslator.Retarget(Me._underlyingProperty.Type, RetargetOptions.RetargetPrimitiveTypesByTypeCode)
			End Get
		End Property

		Public Overrides ReadOnly Property TypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me.CustomModifiersTuple.TypeCustomModifiers
			End Get
		End Property

		Public ReadOnly Property UnderlyingProperty As PropertySymbol
			Get
				Return Me._underlyingProperty
			End Get
		End Property

		Public Sub New(ByVal retargetingModule As RetargetingModuleSymbol, ByVal underlyingProperty As PropertySymbol)
			MyBase.New()
			Me._lazyCachedUseSiteInfo = CachedUseSiteInfo(Of AssemblySymbol).Uninitialized
			If (TypeOf underlyingProperty Is RetargetingPropertySymbol) Then
				Throw New ArgumentException()
			End If
			Me._retargetingModule = retargetingModule
			Me._underlyingProperty = underlyingProperty
		End Sub

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.RetargetingTranslator.GetRetargetedAttributes(Me._underlyingProperty, Me._lazyCustomAttributes, False)
		End Function

		Friend Overrides Function GetCustomAttributesToEmit(ByVal compilationState As ModuleCompilationState) As IEnumerable(Of VisualBasicAttributeData)
			Return Me.RetargetingTranslator.RetargetAttributes(Me._underlyingProperty.GetCustomAttributesToEmit(compilationState))
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._underlyingProperty.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim primaryDependency As AssemblySymbol = MyBase.PrimaryDependency
			If (Not Me._lazyCachedUseSiteInfo.IsInitialized) Then
				Me._lazyCachedUseSiteInfo.Initialize(primaryDependency, MyBase.CalculateUseSiteInfo())
			End If
			Return Me._lazyCachedUseSiteInfo.ToUseSiteInfo(primaryDependency)
		End Function

		Private Function RetargetExplicitInterfaceImplementations() As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
			Dim immutableAndFree As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
			Dim explicitInterfaceImplementations As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol) = Me.UnderlyingProperty.ExplicitInterfaceImplementations
			If (Not explicitInterfaceImplementations.IsEmpty) Then
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol).GetInstance()
				Dim length As Integer = explicitInterfaceImplementations.Length - 1
				Dim num As Integer = 0
				Do
					Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = Me.RetargetingModule.RetargetingTranslator.Retarget(explicitInterfaceImplementations(num), PropertySignatureComparer.RetargetedExplicitPropertyImplementationComparer)
					If (propertySymbol IsNot Nothing) Then
						instance.Add(propertySymbol)
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
			Dim parameters As ImmutableArray(Of ParameterSymbol) = Me._underlyingProperty.Parameters
			Dim length As Integer = parameters.Length
			If (length <> 0) Then
				Dim parameterSymbolArray(length - 1 + 1 - 1) As ParameterSymbol
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				Do
					parameterSymbolArray(num1) = RetargetingParameterSymbol.CreatePropertyParameter(Me, parameters(num1))
					num1 = num1 + 1
				Loop While num1 <= num
				empty = ImmutableArrayExtensions.AsImmutableOrNull(Of ParameterSymbol)(parameterSymbolArray)
			Else
				empty = ImmutableArray(Of ParameterSymbol).Empty
			End If
			Return empty
		End Function
	End Class
End Namespace