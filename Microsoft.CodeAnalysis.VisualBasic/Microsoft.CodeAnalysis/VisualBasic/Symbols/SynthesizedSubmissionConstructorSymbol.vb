Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SynthesizedSubmissionConstructorSymbol
		Inherits SynthesizedConstructorBase
		Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return Me._parameters
			End Get
		End Property

		Friend Sub New(ByVal syntaxReference As Microsoft.CodeAnalysis.SyntaxReference, ByVal container As NamedTypeSymbol, ByVal isShared As Boolean, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			MyBase.New(syntaxReference, container, isShared, binder, diagnostics)
			Dim declaringCompilation As VisualBasicCompilation = container.DeclaringCompilation
			Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = declaringCompilation.CreateArrayTypeSymbol(declaringCompilation.GetSpecialType(SpecialType.System_Object), 1)
			diagnostics.Add(arrayTypeSymbol.GetUseSiteInfo(), NoLocation.Singleton)
			Me._parameters = ImmutableArray.Create(Of ParameterSymbol)(New SynthesizedParameterSymbol(Me, arrayTypeSymbol, 0, False, "submissionArray"))
		End Sub

		Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Return DirectCast(MyBase.ContainingType, SourceMemberContainerTypeSymbol).CalculateSyntaxOffsetInSynthesizedConstructor(localPosition, localTree, MyBase.IsShared)
		End Function

		Friend Overrides Function GetBoundMethodBody(ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByRef methodBodyBinder As Binder = Nothing) As BoundBlock
			Dim syntax As SyntaxNode = MyBase.Syntax
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
			Return New BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundStatement)(New BoundReturnStatement(syntax, Nothing, Nothing, Nothing, False)), False)
		End Function

		Friend Shared Function MakeSubmissionInitialization(ByVal syntax As SyntaxNode, ByVal constructor As MethodSymbol, ByVal synthesizedFields As SynthesizedSubmissionFields, ByVal compilation As VisualBasicCompilation) As ImmutableArray(Of BoundStatement)
			Dim enumerator As IEnumerator(Of FieldSymbol) = Nothing
			Dim boundStatements As List(Of BoundStatement) = New List(Of BoundStatement)()
			Dim item As ParameterSymbol = constructor.Parameters(0)
			Dim parameters As ImmutableArray(Of ParameterSymbol) = constructor.Parameters
			Dim boundParameter As Microsoft.CodeAnalysis.VisualBasic.BoundParameter = New Microsoft.CodeAnalysis.VisualBasic.BoundParameter(syntax, item, False, parameters(0).Type)
			compilation.CreateArrayTypeSymbol(compilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Object), 1)
			Dim specialType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = compilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32)
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = compilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Object)
			Dim boundMeReference As Microsoft.CodeAnalysis.VisualBasic.BoundMeReference = New Microsoft.CodeAnalysis.VisualBasic.BoundMeReference(syntax, constructor.ContainingType)
			Dim submissionSlotIndex As Integer = compilation.GetSubmissionSlotIndex()
			boundStatements.Add((New BoundExpressionStatement(syntax, New BoundAssignmentOperator(syntax, New BoundArrayAccess(syntax, boundParameter, ImmutableArray.Create(Of BoundExpression)(New BoundLiteral(syntax, ConstantValue.Create(submissionSlotIndex), specialType)), True, namedTypeSymbol, False), New BoundDirectCast(syntax, boundMeReference, ConversionKind.Reference, namedTypeSymbol, False), True, namedTypeSymbol, False), False)).MakeCompilerGenerated())
			Dim hostObjectField As FieldSymbol = synthesizedFields.GetHostObjectField()
			If (hostObjectField IsNot Nothing) Then
				boundStatements.Add(New BoundExpressionStatement(syntax, (New BoundAssignmentOperator(syntax, New BoundFieldAccess(syntax, boundMeReference, hostObjectField, True, hostObjectField.Type, False), New BoundDirectCast(syntax, New BoundArrayAccess(syntax, boundParameter, ImmutableArray.Create(Of BoundExpression)(New BoundLiteral(syntax, ConstantValue.Create(0), specialType)), False, namedTypeSymbol, False), ConversionKind.Reference, hostObjectField.Type, False), True, hostObjectField.Type, False)).MakeCompilerGenerated(), False))
			End If
			Using enumerator
				enumerator = synthesizedFields.FieldSymbols.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As FieldSymbol = enumerator.Current
					Dim type As ImplicitNamedTypeSymbol = DirectCast(current.Type, ImplicitNamedTypeSymbol)
					Dim num As Integer = type.DeclaringCompilation.GetSubmissionSlotIndex()
					boundStatements.Add((New BoundExpressionStatement(syntax, New BoundAssignmentOperator(syntax, New BoundFieldAccess(syntax, boundMeReference, current, True, type, False), New BoundDirectCast(syntax, New BoundArrayAccess(syntax, boundParameter, ImmutableArray.Create(Of BoundExpression)(New BoundLiteral(syntax, ConstantValue.Create(num), specialType)), False, namedTypeSymbol, False), ConversionKind.Reference, type, False), True, type, False), False)).MakeCompilerGenerated())
				End While
			End Using
			Return ImmutableArrayExtensions.AsImmutableOrNull(Of BoundStatement)(boundStatements)
		End Function
	End Class
End Namespace