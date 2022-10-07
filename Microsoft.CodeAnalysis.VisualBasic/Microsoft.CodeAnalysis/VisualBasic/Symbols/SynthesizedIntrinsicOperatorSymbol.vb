Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SynthesizedIntrinsicOperatorSymbol
		Inherits SynthesizedMethodBase
		Private ReadOnly _name As String

		Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

		Private ReadOnly _returnType As TypeSymbol

		Private ReadOnly _isCheckedBuiltin As Boolean

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.[Public]
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property IsCheckedBuiltin As Boolean
			Get
				Return Me._isCheckedBuiltin
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

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property IsSub As Boolean
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
				Return Microsoft.CodeAnalysis.MethodKind.BuiltinOperator
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
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

		Public Sub New(ByVal container As NamedTypeSymbol, ByVal name As String, ByVal rightType As TypeSymbol, ByVal returnType As TypeSymbol, ByVal isCheckedBuiltin As Boolean)
			MyBase.New(container)
			Me._name = name
			Me._returnType = returnType
			Me._parameters = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of ParameterSymbol)(New ParameterSymbol() { New SynthesizedIntrinsicOperatorSymbol.SynthesizedOperatorParameterSymbol(Me, container, 0, "left"), New SynthesizedIntrinsicOperatorSymbol.SynthesizedOperatorParameterSymbol(Me, rightType, 1, "right") })
			Me._isCheckedBuiltin = isCheckedBuiltin
		End Sub

		Public Sub New(ByVal container As NamedTypeSymbol, ByVal name As String, ByVal returnType As TypeSymbol, ByVal isCheckedBuiltin As Boolean)
			MyBase.New(container)
			Me._name = name
			Me._returnType = returnType
			Me._parameters = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of ParameterSymbol)(New ParameterSymbol() { New SynthesizedIntrinsicOperatorSymbol.SynthesizedOperatorParameterSymbol(Me, container, 0, "value") })
			Me._isCheckedBuiltin = isCheckedBuiltin
		End Sub

		Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Dim flag As Boolean
			If (obj <> Me) Then
				Dim synthesizedIntrinsicOperatorSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedIntrinsicOperatorSymbol = TryCast(obj, Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedIntrinsicOperatorSymbol)
				If (synthesizedIntrinsicOperatorSymbol Is Nothing) Then
					flag = False
				ElseIf (Me._isCheckedBuiltin <> synthesizedIntrinsicOperatorSymbol._isCheckedBuiltin OrElse Me._parameters.Length <> synthesizedIntrinsicOperatorSymbol._parameters.Length OrElse Not [String].Equals(Me._name, synthesizedIntrinsicOperatorSymbol._name, StringComparison.Ordinal) OrElse Not TypeSymbol.Equals(Me.m_containingType, synthesizedIntrinsicOperatorSymbol.m_containingType, TypeCompareKind.ConsiderEverything) OrElse Not TypeSymbol.Equals(Me._returnType, synthesizedIntrinsicOperatorSymbol._returnType, TypeCompareKind.ConsiderEverything)) Then
					flag = False
				Else
					Dim length As Integer = Me._parameters.Length - 1
					Dim num As Integer = 0
					While num <= length
						If (TypeSymbol.Equals(Me._parameters(num).Type, synthesizedIntrinsicOperatorSymbol._parameters(num).Type, TypeCompareKind.ConsiderEverything)) Then
							num = num + 1
						Else
							flag = False
							Return flag
						End If
					End While
					flag = True
				End If
			Else
				flag = True
			End If
			Return flag
		End Function

		Public Overrides Function GetDocumentationCommentId() As String
			Return Nothing
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim str As String = Me._name
			Dim mContainingType As NamedTypeSymbol = Me.m_containingType
			Dim parameterSymbols As ImmutableArray(Of ParameterSymbol) = Me._parameters
			Return Hash.Combine(Of String)(str, Hash.Combine(Of NamedTypeSymbol)(mContainingType, parameterSymbols.Length))
		End Function

		Friend Overrides Function IsMetadataNewSlot(Optional ByVal ignoreInterfaceImplementationChanges As Boolean = False) As Boolean
			Return False
		End Function

		Private NotInheritable Class SynthesizedOperatorParameterSymbol
			Inherits SynthesizedParameterSimpleSymbol
			Public Sub New(ByVal container As MethodSymbol, ByVal type As TypeSymbol, ByVal ordinal As Integer, ByVal name As String)
				MyBase.New(container, type, ordinal, name)
			End Sub

			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				Dim flag As Boolean
				If (obj <> Me) Then
					Dim synthesizedOperatorParameterSymbol As SynthesizedIntrinsicOperatorSymbol.SynthesizedOperatorParameterSymbol = TryCast(obj, SynthesizedIntrinsicOperatorSymbol.SynthesizedOperatorParameterSymbol)
					If (synthesizedOperatorParameterSymbol IsNot Nothing) Then
						flag = If(MyBase.Ordinal <> synthesizedOperatorParameterSymbol.Ordinal, False, MyBase.ContainingSymbol = synthesizedOperatorParameterSymbol.ContainingSymbol)
					Else
						flag = False
					End If
				Else
					flag = True
				End If
				Return flag
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return Hash.Combine(Of Symbol)(MyBase.ContainingSymbol, MyBase.Ordinal.GetHashCode())
			End Function
		End Class
	End Class
End Namespace