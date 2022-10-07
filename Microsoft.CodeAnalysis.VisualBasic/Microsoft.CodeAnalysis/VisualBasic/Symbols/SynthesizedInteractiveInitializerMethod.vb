Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SynthesizedInteractiveInitializerMethod
		Inherits SynthesizedMethodBase
		Friend Const InitializerName As String = "<Initialize>"

		Friend ReadOnly ResultType As TypeSymbol

		Friend ReadOnly FunctionLocal As LocalSymbol

		Friend ReadOnly ExitLabel As LabelSymbol

		Private ReadOnly _syntaxReference As SyntaxReference

		Private ReadOnly _returnType As TypeSymbol

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.Internal
			End Get
		End Property

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return True
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property IsAsync As Boolean
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

		Friend Overrides ReadOnly Property IsScriptInitializer As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				Return Me._returnType.SpecialType = SpecialType.System_Void
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me.m_containingType.Locations
			End Get
		End Property

		Public Overrides ReadOnly Property MethodKind As Microsoft.CodeAnalysis.MethodKind
			Get
				Return Microsoft.CodeAnalysis.MethodKind.Ordinary
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return "<Initialize>"
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me._returnType
			End Get
		End Property

		Friend Overrides ReadOnly Property Syntax As SyntaxNode
			Get
				Return DirectCast(Me._syntaxReference.GetSyntax(New CancellationToken()), VisualBasicSyntaxNode)
			End Get
		End Property

		Friend Sub New(ByVal syntaxReference As Microsoft.CodeAnalysis.SyntaxReference, ByVal containingType As SourceMemberContainerTypeSymbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			MyBase.New(containingType)
			Me._syntaxReference = syntaxReference
			SynthesizedInteractiveInitializerMethod.CalculateReturnType(containingType.DeclaringCompilation, diagnostics, Me.ResultType, Me._returnType)
			Me.FunctionLocal = New SynthesizedLocal(Me, Me.ResultType, SynthesizedLocalKind.FunctionReturnValue, Me.Syntax, False)
			Me.ExitLabel = New GeneratedLabelSymbol("exit")
		End Sub

		Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Return DirectCast(Me.m_containingType, SourceMemberContainerTypeSymbol).CalculateSyntaxOffsetInSynthesizedConstructor(localPosition, localTree, False)
		End Function

		Private Shared Sub CalculateReturnType(ByVal compilation As VisualBasicCompilation, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByRef resultType As TypeSymbol, ByRef returnType As TypeSymbol)
			Dim returnTypeOpt As Type = Nothing
			If (compilation.ScriptCompilationInfo IsNot Nothing) Then
				returnTypeOpt = compilation.ScriptCompilationInfo.ReturnTypeOpt
			End If
			Dim wellKnownType As NamedTypeSymbol = compilation.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Threading_Tasks_Task_T)
			diagnostics.Add(wellKnownType.GetUseSiteInfo(), NoLocation.Singleton)
			If (returnTypeOpt IsNot Nothing) Then
				resultType = compilation.GetTypeByReflectionType(returnTypeOpt)
			Else
				resultType = compilation.GetSpecialType(SpecialType.System_Object)
			End If
			returnType = wellKnownType.Construct(New TypeSymbol() { resultType })
		End Sub

		Friend Overrides Function GetBoundMethodBody(ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByRef methodBodyBinder As Binder = Nothing) As BoundBlock
			Dim syntax As SyntaxNode = Me.Syntax
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
			Return New BoundBlock(syntax, statementSyntaxes, ImmutableArray.Create(Of LocalSymbol)(Me.FunctionLocal), ImmutableArray.Create(Of BoundStatement)(New BoundLabelStatement(syntax, Me.ExitLabel)), False)
		End Function
	End Class
End Namespace