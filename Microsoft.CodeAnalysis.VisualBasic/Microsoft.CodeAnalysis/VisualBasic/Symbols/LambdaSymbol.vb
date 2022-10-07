Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Reflection
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class LambdaSymbol
		Inherits MethodSymbol
		Friend ReadOnly Shared ReturnTypeIsBeingInferred As TypeSymbol

		Friend ReadOnly Shared ReturnTypeIsUnknown As TypeSymbol

		Friend ReadOnly Shared ReturnTypePendingDelegate As TypeSymbol

		Friend ReadOnly Shared ReturnTypeVoidReplacement As TypeSymbol

		Friend ReadOnly Shared ErrorRecoveryInferenceError As TypeSymbol

		Private ReadOnly _syntaxNode As SyntaxNode

		Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

		Protected m_ReturnType As TypeSymbol

		Private ReadOnly _binder As Microsoft.CodeAnalysis.VisualBasic.Binder

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return 0
			End Get
		End Property

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property CallingConvention As Microsoft.Cci.CallingConvention
			Get
				Return Microsoft.Cci.CallingConvention.[Default]
			End Get
		End Property

		Friend ReadOnly Property ContainingBinder As Microsoft.CodeAnalysis.VisualBasic.Binder
			Get
				Return Me._binder
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._binder.ContainingMember
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.[Private]
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray.Create(Of SyntaxReference)(Me._syntaxNode.GetReference())
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
			Get
				Return ImmutableArray(Of MethodSymbol).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return True
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

		Friend Overrides ReadOnly Property ImplementationAttributes As MethodImplAttributes
			Get
				Return MethodImplAttributes.IL
			End Get
		End Property

		Public Overrides ReadOnly Property IsExtensionMethod As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsExternalMethod As Boolean
			Get
				Return False
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property IsInitOnly As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property IsLambdaMethod As Boolean
			Get
				Return True
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsMethodKindBasedOnSyntax As Boolean
			Get
				Return True
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

		Friend NotOverridable Overrides ReadOnly Property IsQueryLambdaMethod As Boolean
			Get
				Return Me.SynthesizedKind.IsQueryLambda()
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Dim flag As Boolean
				Dim containingSymbol As Symbol = Me.ContainingSymbol
				Dim kind As SymbolKind = containingSymbol.Kind
				flag = If(kind = SymbolKind.Field OrElse kind = SymbolKind.Method OrElse kind = SymbolKind.[Property], containingSymbol.IsShared, True)
				Return flag
			End Get
		End Property

		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				Return Me.m_ReturnType.IsVoidType()
			End Get
		End Property

		Public Overrides ReadOnly Property IsVararg As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return ImmutableArray.Create(Of Location)(Me._syntaxNode.GetLocation())
			End Get
		End Property

		Public Overrides ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind
			Get
				Return Microsoft.CodeAnalysis.MethodKind.AnonymousFunction
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return Me._parameters
			End Get
		End Property

		Public Overrides ReadOnly Property RefCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnsByRef As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me.m_ReturnType
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnTypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property ReturnTypeMarshallingInformation As MarshalPseudoCustomAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Friend Overrides ReadOnly Property Syntax As SyntaxNode
			Get
				Return Me._syntaxNode
			End Get
		End Property

		Public MustOverride ReadOnly Property SynthesizedKind As SynthesizedLambdaKind

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

		Shared Sub New()
			LambdaSymbol.ReturnTypeIsBeingInferred = New ErrorTypeSymbol()
			LambdaSymbol.ReturnTypeIsUnknown = New ErrorTypeSymbol()
			LambdaSymbol.ReturnTypePendingDelegate = New ErrorTypeSymbol()
			LambdaSymbol.ReturnTypeVoidReplacement = New ErrorTypeSymbol()
			LambdaSymbol.ErrorRecoveryInferenceError = New ErrorTypeSymbol()
		End Sub

		Protected Sub New(ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal parameters As ImmutableArray(Of BoundLambdaParameterSymbol), ByVal returnType As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder)
			MyBase.New()
			Me._syntaxNode = syntaxNode
			Me._parameters = StaticCast(Of ParameterSymbol).From(Of BoundLambdaParameterSymbol)(parameters)
			Me.m_ReturnType = returnType
			Me._binder = binder
			Dim enumerator As ImmutableArray(Of BoundLambdaParameterSymbol).Enumerator = parameters.GetEnumerator()
			While enumerator.MoveNext()
				enumerator.Current.SetLambdaSymbol(Me)
			End While
		End Sub

		Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Dim flag As Boolean
			If (obj <> Me) Then
				Dim lambdaSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LambdaSymbol = TryCast(obj, Microsoft.CodeAnalysis.VisualBasic.Symbols.LambdaSymbol)
				flag = If(lambdaSymbol Is Nothing OrElse lambdaSymbol._syntaxNode <> Me._syntaxNode OrElse Not [Object].Equals(lambdaSymbol.ContainingSymbol, Me.ContainingSymbol), False, MethodSignatureComparer.AllAspectsSignatureComparer.Equals(lambdaSymbol, Me))
			Else
				flag = True
			End If
			Return flag
		End Function

		Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			Return ImmutableArray(Of String).Empty
		End Function

		Public Overrides Function GetDllImportData() As DllImportData
			Return Nothing
		End Function

		Public Overrides Function GetHashCode() As Integer
			Dim hashCode As Integer = Me.Syntax.GetHashCode()
			Dim parameterSymbols As ImmutableArray(Of ParameterSymbol) = Me._parameters
			Dim num As Integer = Hash.Combine(hashCode, parameterSymbols.Length)
			num = Hash.Combine(num, Me.ReturnType.GetHashCode())
			Dim length As Integer = Me._parameters.Length - 1
			Dim num1 As Integer = 0
			Do
				parameterSymbols = Me._parameters
				num = Hash.Combine(num, parameterSymbols(num1).Type.GetHashCode())
				num1 = num1 + 1
			Loop While num1 <= length
			Return num
		End Function

		Friend NotOverridable Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function IsMetadataNewSlot(Optional ByVal ignoreInterfaceImplementationChanges As Boolean = False) As Boolean
			Return False
		End Function

		Friend Overrides Function TryGetMeParameter(<Out> ByRef meParameter As ParameterSymbol) As Boolean
			Dim kind As SymbolKind = Me.ContainingSymbol.Kind
			If (kind = SymbolKind.Field) Then
				meParameter = DirectCast(Me.ContainingSymbol, FieldSymbol).MeParameter
			ElseIf (kind = SymbolKind.Method) Then
				meParameter = DirectCast(Me.ContainingSymbol, MethodSymbol).MeParameter
			ElseIf (kind = SymbolKind.[Property]) Then
				meParameter = DirectCast(Me.ContainingSymbol, PropertySymbol).MeParameter
			Else
				meParameter = Nothing
			End If
			Return True
		End Function
	End Class
End Namespace