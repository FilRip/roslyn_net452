Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SynthesizedConstructorSymbol
		Inherits SynthesizedConstructorBase
		Private ReadOnly _debuggable As Boolean

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return Me._debuggable
			End Get
		End Property

		Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
			Get
				Return ImmutableArray(Of ParameterSymbol).Empty
			End Get
		End Property

		Friend Sub New(ByVal syntaxReference As Microsoft.CodeAnalysis.SyntaxReference, ByVal container As NamedTypeSymbol, ByVal isShared As Boolean, ByVal isDebuggable As Boolean, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			MyBase.New(syntaxReference, container, isShared, binder, diagnostics)
			Me._debuggable = isDebuggable
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
		End Sub

		Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Return DirectCast(MyBase.ContainingType, SourceMemberContainerTypeSymbol).CalculateSyntaxOffsetInSynthesizedConstructor(localPosition, localTree, MyBase.IsShared)
		End Function

		Friend Overrides Function GetBoundMethodBody(ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByRef methodBodyBinder As Binder = Nothing) As BoundBlock
			methodBodyBinder = Nothing
			Dim boundReturnStatement As Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement(MyBase.Syntax, Nothing, Nothing, Nothing, False)
			boundReturnStatement.SetWasCompilerGenerated()
			Dim syntax As SyntaxNode = MyBase.Syntax
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
			Return New BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundStatement)(boundReturnStatement), False)
		End Function
	End Class
End Namespace