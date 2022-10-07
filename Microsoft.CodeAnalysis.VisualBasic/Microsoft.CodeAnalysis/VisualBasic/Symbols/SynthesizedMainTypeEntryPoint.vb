Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SynthesizedMainTypeEntryPoint
		Inherits SynthesizedRegularMethodBase
		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.[Public]
			End Get
		End Property

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsSub As Boolean
			Get
				Return True
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me.ContainingAssembly.GetSpecialType(SpecialType.System_Void)
			End Get
		End Property

		Public Sub New(ByVal syntaxNode As VisualBasicSyntaxNode, ByVal container As SourceNamedTypeSymbol)
			MyBase.New(syntaxNode, container, "Main", True)
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
			Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
			Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
			Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_STAThreadAttribute__ctor, typedConstants, keyValuePairs, False))
		End Sub

		Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Overrides Function GetBoundMethodBody(ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, <Out> Optional ByRef methodBodyBinder As Microsoft.CodeAnalysis.VisualBasic.Binder = Nothing) As BoundBlock
			Dim boundBadStatement As BoundStatement
			methodBodyBinder = Nothing
			Dim syntax As SyntaxNode = MyBase.Syntax
			Dim containingSymbol As SourceNamedTypeSymbol = DirectCast(MyBase.ContainingSymbol, SourceNamedTypeSymbol)
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForType(containingSymbol.ContainingSourceModule, syntax.SyntaxTree, containingSymbol)
			Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance(False, diagnostics.AccumulatesDependencies)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = binder.TryDefaultInstanceProperty(New BoundTypeExpression(syntax, containingSymbol, False), instance)
			If (boundExpression IsNot Nothing) Then
				diagnostics.AddDependencies(instance, False)
			Else
				boundExpression = binder.BindObjectCreationExpression(syntax, containingSymbol, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty, diagnostics)
			End If
			instance.Free()
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = New UseSiteInfo(Of AssemblySymbol)()
			Dim wellKnownTypeMember As MethodSymbol = DirectCast(Microsoft.CodeAnalysis.VisualBasic.Binder.GetWellKnownTypeMember(containingSymbol.DeclaringCompilation, WellKnownMember.System_Windows_Forms_Application__RunForm, useSiteInfo), MethodSymbol)
			If (Microsoft.CodeAnalysis.VisualBasic.Binder.ReportUseSite(diagnostics, syntax, useSiteInfo)) Then
				boundBadStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundBadStatement(syntax, ImmutableArray(Of BoundNode).Empty, True)
			Else
				Dim boundMethodGroup As Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup = New Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup(syntax, Nothing, ImmutableArray.Create(Of MethodSymbol)(wellKnownTypeMember), LookupResultKind.Good, Nothing, QualificationKind.QualifiedViaTypeName, False)
				Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpression)
				Dim strs As ImmutableArray(Of String) = New ImmutableArray(Of String)()
				boundBadStatement = binder.BindInvocationExpression(syntax, syntax, TypeCharacter.None, boundMethodGroup, boundExpressions, strs, diagnostics, Nothing, False, False, False, Nothing, False).ToStatement()
			End If
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
			Return New BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundStatement)(boundBadStatement, New BoundReturnStatement(syntax, Nothing, Nothing, Nothing, False)), False)
		End Function
	End Class
End Namespace