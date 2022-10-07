Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Reflection
Imports System.Reflection.Metadata

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SignatureOnlyMethodSymbol
		Inherits MethodSymbol
		Private ReadOnly _name As String

		Private ReadOnly _containingType As TypeSymbol

		Private ReadOnly _methodKind As Microsoft.CodeAnalysis.MethodKind

		Private ReadOnly _callingConvention As Microsoft.Cci.CallingConvention

		Private ReadOnly _typeParameters As ImmutableArray(Of TypeParameterSymbol)

		Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

		Private ReadOnly _returnsByRef As Boolean

		Private ReadOnly _returnType As TypeSymbol

		Private ReadOnly _returnTypeCustomModifiers As ImmutableArray(Of CustomModifier)

		Private ReadOnly _refCustomModifiers As ImmutableArray(Of CustomModifier)

		Private ReadOnly _explicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)

		Private ReadOnly _isOverrides As Boolean

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return Me._typeParameters.Length
			End Get
		End Property

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
			Get
				Return Me._callingConvention
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
			Get
				Return Me._explicitInterfaceImplementations
			End Get
		End Property

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend Overrides ReadOnly Property ImplementationAttributes As MethodImplAttributes
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property IsAsync As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsExtensionMethod As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property IsExternalMethod As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property IsGenericMethod As Boolean
			Get
				Return Me.Arity > 0
			End Get
		End Property

		Public Overrides ReadOnly Property IsInitOnly As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsIterator As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMethodKindBasedOnSyntax As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverloads As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return Me._isOverrides
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				Return Me.ReturnType.SpecialType = SpecialType.System_Void
			End Get
		End Property

		Public Overrides ReadOnly Property IsVararg As Boolean
			Get
				Return (New SignatureHeader(CByte(Me._callingConvention))).CallingConvention = SignatureCallingConvention.VarArgs
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind
			Get
				Return Me._methodKind
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property OverriddenMembers As OverriddenMembersResult(Of MethodSymbol)
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property OverriddenMethod As MethodSymbol
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return Me._parameters
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._refCustomModifiers
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnsByRef As Boolean
			Get
				Return Me._returnsByRef
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me._returnType
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnTypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._returnTypeCustomModifiers
			End Get
		End Property

		Friend Overrides ReadOnly Property ReturnTypeMarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Friend Overrides ReadOnly Property Syntax As SyntaxNode
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property TypeArguments As ImmutableArray(Of TypeSymbol)
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Return Me._typeParameters
			End Get
		End Property

		Public Sub New(ByVal name As String, ByVal m_containingType As TypeSymbol, ByVal methodKind As Microsoft.CodeAnalysis.MethodKind, ByVal callingConvention As Microsoft.Cci.CallingConvention, ByVal typeParameters As ImmutableArray(Of TypeParameterSymbol), ByVal parameters As ImmutableArray(Of ParameterSymbol), ByVal returnsByRef As Boolean, ByVal returnType As TypeSymbol, ByVal returnTypeCustomModifiers As ImmutableArray(Of CustomModifier), ByVal refCustomModifiers As ImmutableArray(Of CustomModifier), ByVal explicitInterfaceImplementations As ImmutableArray(Of MethodSymbol), Optional ByVal isOverrides As Boolean = False)
			MyBase.New()
			Me._callingConvention = callingConvention
			Me._typeParameters = typeParameters
			Me._returnsByRef = returnsByRef
			Me._returnType = returnType
			Me._returnTypeCustomModifiers = returnTypeCustomModifiers
			Me._refCustomModifiers = refCustomModifiers
			Me._parameters = parameters
			Me._explicitInterfaceImplementations = Microsoft.CodeAnalysis.ImmutableArrayExtensions.NullToEmpty(Of MethodSymbol)(explicitInterfaceImplementations)
			Me._containingType = m_containingType
			Me._methodKind = methodKind
			Me._name = name
			Me._isOverrides = isOverrides
		End Sub

		Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overrides Function GetDllImportData() As DllImportData
			Return Nothing
		End Function

		Friend Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function IsMetadataNewSlot(Optional ByVal ignoreInterfaceImplementationChanges As Boolean = False) As Boolean
			Throw ExceptionUtilities.Unreachable
		End Function
	End Class
End Namespace