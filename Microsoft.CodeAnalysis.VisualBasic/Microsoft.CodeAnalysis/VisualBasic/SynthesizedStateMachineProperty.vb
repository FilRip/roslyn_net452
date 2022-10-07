Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class SynthesizedStateMachineProperty
		Inherits SynthesizedPropertyBase
		Implements ISynthesizedMethodBodyImplementationSymbol
		Private ReadOnly _getter As SynthesizedStateMachineMethod

		Private ReadOnly _name As String

		Friend Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
			Get
				Return Me._getter.CallingConvention
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._getter.ContainingSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Me._getter.DeclaredAccessibility
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of PropertySymbol)
			Get
				Return ImmutableArray.Create(Of PropertySymbol)(Me.ImplementedProperty)
			End Get
		End Property

		Public Overrides ReadOnly Property GetMethod As MethodSymbol
			Get
				Return Me._getter
			End Get
		End Property

		Public ReadOnly Property HasMethodBodyDependency As Boolean Implements ISynthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency
			Get
				Return Me._getter.HasMethodBodyDependency
			End Get
		End Property

		Private ReadOnly Property ImplementedProperty As PropertySymbol
			Get
				Return DirectCast(Me._getter.ExplicitInterfaceImplementations(0).AssociatedSymbol, PropertySymbol)
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return Me._getter.IsShared
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return ImmutableArray(Of Location).Empty
			End Get
		End Property

		Public ReadOnly Property Method As IMethodSymbolInternal Implements ISynthesizedMethodBodyImplementationSymbol.Method
			Get
				Return DirectCast(Me.ContainingSymbol, ISynthesizedMethodBodyImplementationSymbol).Method
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Public Overrides ReadOnly Property ParameterCount As Integer
			Get
				Return Me._getter.ParameterCount
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return Me._getter.Parameters
			End Get
		End Property

		Public Overrides ReadOnly Property SetMethod As MethodSymbol
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				Return Me._getter.ReturnType
			End Get
		End Property

		Public Overrides ReadOnly Property TypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return Me._getter.ReturnTypeCustomModifiers
			End Get
		End Property

		Friend Sub New(ByVal stateMachineType As StateMachineTypeSymbol, ByVal name As String, ByVal interfacePropertyGetter As MethodSymbol, ByVal syntax As SyntaxNode, ByVal declaredAccessibility As Accessibility)
			MyBase.New()
			Dim str As String
			Me._name = name
			str = If(name.Length <> 7, "IEnumerator.get_Current", "get_Current")
			Me._getter = New SynthesizedStateMachineDebuggerNonUserCodeMethod(stateMachineType, str, interfacePropertyGetter, syntax, declaredAccessibility, False, Me)
		End Sub
	End Class
End Namespace