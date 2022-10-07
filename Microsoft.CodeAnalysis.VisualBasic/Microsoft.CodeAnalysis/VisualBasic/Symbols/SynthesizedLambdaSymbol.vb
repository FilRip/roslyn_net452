Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SynthesizedLambdaSymbol
		Inherits LambdaSymbol
		Private ReadOnly _kind As SynthesizedLambdaKind

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				If (Me._kind = SynthesizedLambdaKind.DelegateRelaxationStub) Then
					Return False
				End If
				Return Me._kind <> SynthesizedLambdaKind.LateBoundAddressOfLambda
			End Get
		End Property

		Public Overrides ReadOnly Property IsAsync As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property IsIterator As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property SynthesizedKind As SynthesizedLambdaKind
			Get
				Return Me._kind
			End Get
		End Property

		Public Sub New(ByVal kind As SynthesizedLambdaKind, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal parameters As ImmutableArray(Of BoundLambdaParameterSymbol), ByVal returnType As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder)
			MyBase.New(syntaxNode, parameters, returnType, binder)
			Me._kind = kind
		End Sub

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Return obj = Me
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return RuntimeHelpers.GetHashCode(Me)
		End Function

		Public Sub SetQueryLambdaReturnType(ByVal returnType As TypeSymbol)
			Me.m_ReturnType = returnType
		End Sub
	End Class
End Namespace