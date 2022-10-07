Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Reflection
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SynthesizedGlobalMethodBase
		Inherits MethodSymbol
		Protected ReadOnly m_privateImplType As PrivateImplementationDetails

		Protected ReadOnly m_containingModule As SourceModuleSymbol

		Protected ReadOnly m_name As String

		Public NotOverridable Overrides ReadOnly Property Arity As Integer
			Get
				Return 0
			End Get
		End Property

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return Nothing
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
			Get
				If (Not Me.IsShared) Then
					Return Microsoft.Cci.CallingConvention.HasThis
				End If
				Return Microsoft.Cci.CallingConvention.[Default]
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Return Me.m_containingModule.ContainingAssembly
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingModule As ModuleSymbol
			Get
				Return Me.m_containingModule
			End Get
		End Property

		Public ReadOnly Property ContainingPrivateImplementationDetailsType As PrivateImplementationDetails
			Get
				Return Me.m_privateImplType
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.Internal
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
			Get
				Return ImmutableArray(Of MethodSymbol).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ImplementationAttributes As MethodImplAttributes
			Get
				Return MethodImplAttributes.IL
			End Get
		End Property

		Public Overrides ReadOnly Property IsAsync As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsExtensionMethod As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsExternalMethod As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return True
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsInitOnly As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsIterator As Boolean
			Get
				Return False
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsMethodKindBasedOnSyntax As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverloads As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsShared As Boolean
			Get
				Return True
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsVararg As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return ImmutableArray(Of Location).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind
			Get
				Return Microsoft.CodeAnalysis.MethodKind.Ordinary
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Name As String
			Get
				Return Me.m_name
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property OverriddenMethod As MethodSymbol
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return ImmutableArray(Of ParameterSymbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnTypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ReturnTypeMarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property Syntax As SyntaxNode
			Get
				Return VisualBasicSyntaxTree.Dummy.GetRoot(New CancellationToken())
			End Get
		End Property

		Public Overrides ReadOnly Property TypeArguments As ImmutableArray(Of TypeSymbol)
			Get
				Return ImmutableArray(Of TypeSymbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Return ImmutableArray(Of TypeParameterSymbol).Empty
			End Get
		End Property

		Protected Sub New(ByVal containingModule As SourceModuleSymbol, ByVal name As String, ByVal privateImplType As PrivateImplementationDetails)
			MyBase.New()
			Me.m_containingModule = containingModule
			Me.m_name = name
			Me.m_privateImplType = privateImplType
		End Sub

		Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			Return ImmutableArray(Of String).Empty
		End Function

		Public NotOverridable Overrides Function GetDllImportData() As DllImportData
			Return Nothing
		End Function

		Friend Overrides Function GetLexicalSortKey() As LexicalSortKey
			Return LexicalSortKey.NotInSource
		End Function

		Public Overrides Function GetReturnTypeAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return ImmutableArray(Of VisualBasicAttributeData).Empty
		End Function

		Friend NotOverridable Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
			Throw ExceptionUtilities.Unreachable
		End Function
	End Class
End Namespace