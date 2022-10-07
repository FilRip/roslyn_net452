Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class SynthesizedStateMachineMethod
		Inherits SynthesizedMethod
		Implements ISynthesizedMethodBodyImplementationSymbol
		Private ReadOnly _interfaceMethod As MethodSymbol

		Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

		Private ReadOnly _locations As ImmutableArray(Of Location)

		Private ReadOnly _accessibility As Accessibility

		Private ReadOnly _generateDebugInfo As Boolean

		Private ReadOnly _hasMethodBodyDependency As Boolean

		Private ReadOnly _associatedProperty As PropertySymbol

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return 0
			End Get
		End Property

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return Me._associatedProperty
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Me._accessibility
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
			Get
				Return ImmutableArray.Create(Of MethodSymbol)(Me._interfaceMethod)
			End Get
		End Property

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return Me._generateDebugInfo
			End Get
		End Property

		Public ReadOnly Property HasMethodBodyDependency As Boolean Implements ISynthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency
			Get
				Return Me._hasMethodBodyDependency
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				Return Me._interfaceMethod.IsSub
			End Get
		End Property

		Public Overrides ReadOnly Property IsVararg As Boolean
			Get
				Return Me._interfaceMethod.IsVararg
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._locations
			End Get
		End Property

		Public ReadOnly Property Method As IMethodSymbolInternal Implements ISynthesizedMethodBodyImplementationSymbol.Method
			Get
				Return Me.StateMachineType.KickoffMethod
			End Get
		End Property

		Friend Overrides ReadOnly Property ParameterCount As Integer
			Get
				Return Me._parameters.Length
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return Me._parameters
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me._interfaceMethod.ReturnType
			End Get
		End Property

		Public ReadOnly Property StateMachineType As StateMachineTypeSymbol
			Get
				Return DirectCast(MyBase.ContainingSymbol, StateMachineTypeSymbol)
			End Get
		End Property

		Public Overrides ReadOnly Property TypeArguments As ImmutableArray(Of TypeSymbol)
			Get
				Return ImmutableArray(Of TypeSymbol).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property TypeMap As TypeSubstitution
			Get
				Return Nothing
			End Get
		End Property

		Protected Sub New(ByVal stateMachineType As StateMachineTypeSymbol, ByVal name As String, ByVal interfaceMethod As MethodSymbol, ByVal syntax As SyntaxNode, ByVal declaredAccessibility As Accessibility, ByVal generateDebugInfo As Boolean, ByVal hasMethodBodyDependency As Boolean, Optional ByVal associatedProperty As PropertySymbol = Nothing)
			MyBase.New(syntax, stateMachineType, name, False)
			Me._locations = ImmutableArray.Create(Of Location)(syntax.GetLocation())
			Me._accessibility = declaredAccessibility
			Me._generateDebugInfo = generateDebugInfo
			Me._hasMethodBodyDependency = hasMethodBodyDependency
			Me._interfaceMethod = interfaceMethod
			Dim parameterSymbolArray(Me._interfaceMethod.ParameterCount - 1 + 1 - 1) As ParameterSymbol
			Dim length As Integer = CInt(parameterSymbolArray.Length) - 1
			Dim num As Integer = 0
			Do
				Dim item As ParameterSymbol = Me._interfaceMethod.Parameters(num)
				parameterSymbolArray(num) = SynthesizedMethod.WithNewContainerAndType(Me, item.Type, item)
				num = num + 1
			Loop While num <= length
			Me._parameters = ImmutableArrayExtensions.AsImmutableOrNull(Of ParameterSymbol)(parameterSymbolArray)
			Me._associatedProperty = associatedProperty
		End Sub

		Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Return Me.StateMachineType.KickoffMethod.CalculateLocalSyntaxOffset(localPosition, localTree)
		End Function

		Friend Overrides Function IsMetadataNewSlot(Optional ByVal ignoreInterfaceImplementationChanges As Boolean = False) As Boolean
			Return True
		End Function
	End Class
End Namespace