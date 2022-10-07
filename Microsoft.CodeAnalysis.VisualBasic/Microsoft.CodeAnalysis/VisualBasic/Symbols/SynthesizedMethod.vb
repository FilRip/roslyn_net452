Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Reflection
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SynthesizedMethod
		Inherits SynthesizedMethodBase
		Private ReadOnly _isShared As Boolean

		Private ReadOnly _name As String

		Private ReadOnly _syntaxNodeOpt As SyntaxNode

		Private ReadOnly Shared s_typeSubstitutionFactory As Func(Of Symbol, TypeSubstitution)

		Friend ReadOnly Shared CreateTypeParameter As Func(Of TypeParameterSymbol, Symbol, TypeParameterSymbol)

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.[Public]
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Dim syntax As SyntaxNode = Me.Syntax
				Dim lambdaExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaExpressionSyntax = TryCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.LambdaExpressionSyntax)
				If (lambdaExpressionSyntax Is Nothing) Then
					Dim methodBlockBaseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax = TryCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax)
					If (methodBlockBaseSyntax IsNot Nothing) Then
						syntax = methodBlockBaseSyntax.BlockStatement
					End If
				Else
					syntax = lambdaExpressionSyntax.SubOrFunctionHeader
				End If
				Return ImmutableArray.Create(Of SyntaxReference)(syntax.GetReference())
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ImplementationAttributes As MethodImplAttributes
			Get
				Return MethodImplAttributes.IL
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
				Return Me._isShared
			End Get
		End Property

		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				Return Me.ReturnType.IsVoidType()
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

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return ImmutableArray(Of ParameterSymbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me.ContainingAssembly.GetSpecialType(SpecialType.System_Void)
			End Get
		End Property

		Friend Overrides ReadOnly Property Syntax As SyntaxNode
			Get
				Return Me._syntaxNodeOpt
			End Get
		End Property

		Friend Overridable ReadOnly Property TypeMap As TypeSubstitution
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Shared Sub New()
			SynthesizedMethod.s_typeSubstitutionFactory = Function(container As Symbol) DirectCast(container, SynthesizedMethod).TypeMap
			SynthesizedMethod.CreateTypeParameter = Function(typeParameter As TypeParameterSymbol, container As Symbol) New SynthesizedClonedTypeParameterSymbol(typeParameter, container, typeParameter.Name, SynthesizedMethod.s_typeSubstitutionFactory)
		End Sub

		Friend Sub New(ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal containingSymbol As NamedTypeSymbol, ByVal name As String, ByVal isShared As Boolean)
			MyBase.New(containingSymbol)
			Me._syntaxNodeOpt = syntaxNode
			Me._isShared = isShared
			Me._name = name
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			Dim containingSymbol As SourceMemberContainerTypeSymbol = TryCast(MyBase.ContainingSymbol, SourceMemberContainerTypeSymbol)
			If (containingSymbol IsNot Nothing) Then
				Dim declaringCompilation As VisualBasicCompilation = containingSymbol.DeclaringCompilation
				Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
				Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
				Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor, typedConstants, keyValuePairs, False))
			End If
		End Sub

		Friend Shared Function WithNewContainerAndType(ByVal newContainer As Symbol, ByVal newType As TypeSymbol, ByVal origParameter As ParameterSymbol) As ParameterSymbol
			Dim sourceParameterFlag As SourceParameterFlags = 0
			sourceParameterFlag = If(Not origParameter.IsByRef, sourceParameterFlag Or SourceParameterFlags.[ByVal], sourceParameterFlag Or SourceParameterFlags.[ByRef])
			If (origParameter.IsParamArray) Then
				sourceParameterFlag = sourceParameterFlag Or SourceParameterFlags.[ParamArray]
			End If
			If (origParameter.IsOptional) Then
				sourceParameterFlag = sourceParameterFlag Or SourceParameterFlags.[Optional]
			End If
			Return SourceComplexParameterSymbol.Create(newContainer, origParameter.Name, origParameter.Ordinal, newType, System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of Location)(origParameter.Locations), Nothing, sourceParameterFlag, origParameter.ExplicitDefaultConstantValue)
		End Function
	End Class
End Namespace