Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SynthesizedSimpleMethodSymbol
		Inherits SynthesizedRegularMethodBase
		Private _parameters As ImmutableArray(Of ParameterSymbol)

		Private ReadOnly _overriddenMethod As MethodSymbol

		Private ReadOnly _interfaceMethods As ImmutableArray(Of MethodSymbol)

		Private ReadOnly _isOverloads As Boolean

		Private ReadOnly _returnType As TypeSymbol

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.[Public]
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
			Get
				Return Me._interfaceMethods
			End Get
		End Property

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverloads As Boolean
			Get
				Return Me._isOverloads
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return CObj(Me._overriddenMethod) <> CObj(Nothing)
			End Get
		End Property

		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				Return Me._returnType.IsVoidType()
			End Get
		End Property

		Public Overrides ReadOnly Property OverriddenMethod As MethodSymbol
			Get
				Return Me._overriddenMethod
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
				Return Me._returnType
			End Get
		End Property

		Public Sub New(ByVal container As NamedTypeSymbol, ByVal name As String, ByVal returnType As TypeSymbol, Optional ByVal overriddenMethod As MethodSymbol = Nothing, Optional ByVal interfaceMethod As MethodSymbol = Nothing, Optional ByVal isOverloads As Boolean = False)
			MyBase.New(VisualBasicSyntaxTree.Dummy.GetRoot(New CancellationToken()), container, name, False)
			Dim empty As ImmutableArray(Of MethodSymbol)
			Me._returnType = returnType
			Me._overriddenMethod = overriddenMethod
			Me._isOverloads = isOverloads
			If (interfaceMethod Is Nothing) Then
				empty = ImmutableArray(Of MethodSymbol).Empty
			Else
				empty = ImmutableArray.Create(Of MethodSymbol)(interfaceMethod)
			End If
			Me._interfaceMethods = empty
		End Sub

		Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Sub SetParameters(ByVal parameters As ImmutableArray(Of ParameterSymbol))
			Me._parameters = parameters
		End Sub
	End Class
End Namespace