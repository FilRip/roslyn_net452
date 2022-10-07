Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class SynthesizedEntryPointSymbol
		Inherits SynthesizedMethodBase
		Friend Const MainName As String = "<Main>"

		Friend Const FactoryName As String = "<Factory>"

		Private ReadOnly _containingType As NamedTypeSymbol

		Private ReadOnly _returnType As TypeSymbol

		Public Overrides ReadOnly Property AssociatedSymbol As Symbol
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.[Private]
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
			Get
				Return ImmutableArray(Of MethodSymbol).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				Return False
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return False
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
				Return Me._returnType.SpecialType = SpecialType.System_Void
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

		Public Overrides ReadOnly Property ReturnType As TypeSymbol
			Get
				Return Me._returnType
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnTypeCustomModifiers As ImmutableArray(Of CustomModifier)
			Get
				Return ImmutableArray(Of CustomModifier).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property Syntax As SyntaxNode
			Get
				Return Nothing
			End Get
		End Property

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

		Private Sub New(ByVal containingType As NamedTypeSymbol, ByVal returnType As TypeSymbol)
			MyBase.New(containingType)
			Me._containingType = containingType
			Me._returnType = returnType
		End Sub

		Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
			Throw ExceptionUtilities.Unreachable
		End Function

		Friend Shared Function Create(ByVal initializerMethod As SynthesizedInteractiveInitializerMethod, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As SynthesizedEntryPointSymbol
			Dim scriptEntryPoint As SynthesizedEntryPointSymbol
			Dim requiredMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim containingType As NamedTypeSymbol = initializerMethod.ContainingType
			Dim declaringCompilation As VisualBasicCompilation = containingType.DeclaringCompilation
			If (Not declaringCompilation.IsSubmission) Then
				Dim wellKnownType As NamedTypeSymbol = declaringCompilation.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Threading_Tasks_Task)
				SynthesizedEntryPointSymbol.ReportUseSiteInfo(wellKnownType, diagnostics)
				If (wellKnownType.IsErrorType()) Then
					requiredMethod = Nothing
				Else
					requiredMethod = SynthesizedEntryPointSymbol.GetRequiredMethod(wellKnownType, "GetAwaiter", diagnostics)
				End If
				Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = requiredMethod
				If (methodSymbol1 Is Nothing) Then
					methodSymbol = Nothing
				Else
					methodSymbol = SynthesizedEntryPointSymbol.GetRequiredMethod(methodSymbol1.ReturnType, "GetResult", diagnostics)
				End If
				Dim methodSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = methodSymbol
				scriptEntryPoint = New SynthesizedEntryPointSymbol.ScriptEntryPoint(containingType, declaringCompilation.GetSpecialType(SpecialType.System_Void), methodSymbol1, methodSymbol2)
			Else
				Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = declaringCompilation.CreateArrayTypeSymbol(declaringCompilation.GetSpecialType(SpecialType.System_Object), 1)
				SynthesizedEntryPointSymbol.ReportUseSiteInfo(arrayTypeSymbol, diagnostics)
				scriptEntryPoint = New SynthesizedEntryPointSymbol.SubmissionEntryPoint(containingType, initializerMethod.ReturnType, arrayTypeSymbol)
			End If
			Return scriptEntryPoint
		End Function

		Friend MustOverride Function CreateBody() As BoundBlock

		Private Shared Function CreateParameterlessCall(ByVal syntax As VisualBasicSyntaxNode, ByVal receiver As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal method As MethodSymbol) As BoundCall
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = receiver.MakeRValue()
			Dim empty As ImmutableArray(Of !0) = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty
			Dim returnType As TypeSymbol = method.ReturnType
			Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
			Return (New BoundCall(syntax, method, Nothing, boundExpression, empty, Nothing, returnType, False, False, bitVector)).MakeCompilerGenerated()
		End Function

		Friend Overrides Function GetLexicalSortKey() As LexicalSortKey
			Return LexicalSortKey.NotInSource
		End Function

		Private Shared Function GetRequiredMethod(ByVal type As TypeSymbol, ByVal methodName As String, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = TryCast(System.Linq.ImmutableArrayExtensions.SingleOrDefault(Of Symbol)(type.GetMembers(methodName)), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			If (methodSymbol Is Nothing) Then
				diagnostics.Add(ErrorFactory.ErrorInfo(ERRID.ERR_MissingRuntimeHelper, New [Object]() { [String].Concat(type.MetadataName, ".", methodName) }), NoLocation.Singleton)
			End If
			Return methodSymbol
		End Function

		Private Function GetSyntax() As VisualBasicSyntaxNode
			Return VisualBasicSyntaxTree.Dummy.GetRoot(New CancellationToken())
		End Function

		Friend Overrides Function IsMetadataNewSlot(Optional ByVal ignoreInterfaceImplementationChanges As Boolean = False) As Boolean
			Return False
		End Function

		Private Shared Sub ReportUseSiteInfo(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			diagnostics.Add(symbol.GetUseSiteInfo(), NoLocation.Singleton)
		End Sub

		Private NotInheritable Class ScriptEntryPoint
			Inherits SynthesizedEntryPointSymbol
			Private ReadOnly _getAwaiterMethod As MethodSymbol

			Private ReadOnly _getResultMethod As MethodSymbol

			Public Overrides ReadOnly Property Name As String
				Get
					Return "<Main>"
				End Get
			End Property

			Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
				Get
					Return ImmutableArray(Of ParameterSymbol).Empty
				End Get
			End Property

			Friend Sub New(ByVal containingType As NamedTypeSymbol, ByVal returnType As TypeSymbol, ByVal getAwaiterMethod As MethodSymbol, ByVal getResultMethod As MethodSymbol)
				MyBase.New(containingType, returnType)
				Me._getAwaiterMethod = getAwaiterMethod
				Me._getResultMethod = getResultMethod
			End Sub

			Friend Overrides Function CreateBody() As BoundBlock
				Dim syntax As VisualBasicSyntaxNode = MyBase.GetSyntax()
				Dim scriptConstructor As SynthesizedConstructorBase = Me._containingType.GetScriptConstructor()
				Dim scriptInitializer As SynthesizedInteractiveInitializerMethod = Me._containingType.GetScriptInitializer()
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = (New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, New SynthesizedLocal(Me, Me._containingType, SynthesizedLocalKind.LoweringTemp, Nothing, False), Me._containingType)).MakeCompilerGenerated()
				Dim empty As ImmutableArray(Of !0) = ImmutableArray(Of BoundExpression).Empty
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me._containingType
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = (New Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement(syntax, (New BoundAssignmentOperator(syntax, boundLocal, (New BoundObjectCreationExpression(syntax, scriptConstructor, empty, Nothing, namedTypeSymbol, False, bitVector)).MakeCompilerGenerated(), False, False)).MakeCompilerGenerated(), False)).MakeCompilerGenerated()
				Dim boundExpressionStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = (New Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement(syntax, SynthesizedEntryPointSymbol.CreateParameterlessCall(syntax, SynthesizedEntryPointSymbol.CreateParameterlessCall(syntax, SynthesizedEntryPointSymbol.CreateParameterlessCall(syntax, boundLocal, scriptInitializer), Me._getAwaiterMethod), Me._getResultMethod), False)).MakeCompilerGenerated()
				Dim boundReturnStatement As Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement = (New Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement(syntax, Nothing, Nothing, Nothing, False)).MakeCompilerGenerated()
				Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
				Return (New BoundBlock(syntax, statementSyntaxes, ImmutableArray.Create(Of LocalSymbol)(boundLocal.LocalSymbol), ImmutableArray.Create(Of BoundStatement)(boundExpressionStatement, boundExpressionStatement1, boundReturnStatement), False)).MakeCompilerGenerated()
			End Function
		End Class

		Private NotInheritable Class SubmissionEntryPoint
			Inherits SynthesizedEntryPointSymbol
			Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

			Public Overrides ReadOnly Property Name As String
				Get
					Return "<Factory>"
				End Get
			End Property

			Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
				Get
					Return Me._parameters
				End Get
			End Property

			Friend Sub New(ByVal containingType As NamedTypeSymbol, ByVal returnType As TypeSymbol, ByVal submissionArrayType As TypeSymbol)
				MyBase.New(containingType, returnType)
				Me._parameters = ImmutableArray.Create(Of ParameterSymbol)(New SynthesizedParameterSymbol(Me, submissionArrayType, 0, False, "submissionArray"))
			End Sub

			Friend Overrides Function CreateBody() As BoundBlock
				Dim syntax As VisualBasicSyntaxNode = MyBase.GetSyntax()
				Dim scriptConstructor As SynthesizedConstructorBase = Me._containingType.GetScriptConstructor()
				Dim scriptInitializer As SynthesizedInteractiveInitializerMethod = Me._containingType.GetScriptInitializer()
				Dim item As ParameterSymbol = Me._parameters(0)
				Dim boundParameter As Microsoft.CodeAnalysis.VisualBasic.BoundParameter = (New Microsoft.CodeAnalysis.VisualBasic.BoundParameter(syntax, item, False, item.Type)).MakeCompilerGenerated()
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = (New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, New SynthesizedLocal(Me, Me._containingType, SynthesizedLocalKind.LoweringTemp, Nothing, False), Me._containingType)).MakeCompilerGenerated()
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(boundParameter)
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me._containingType
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = (New Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement(syntax, (New BoundAssignmentOperator(syntax, boundLocal, (New BoundObjectCreationExpression(syntax, scriptConstructor, boundExpressions, Nothing, namedTypeSymbol, False, bitVector)).MakeCompilerGenerated(), False, False)).MakeCompilerGenerated(), False)).MakeCompilerGenerated()
				Dim boundReturnStatement As Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement = (New Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement(syntax, SynthesizedEntryPointSymbol.CreateParameterlessCall(syntax, boundLocal, scriptInitializer).MakeRValue(), Nothing, Nothing, False)).MakeCompilerGenerated()
				Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
				Return (New BoundBlock(syntax, statementSyntaxes, ImmutableArray.Create(Of LocalSymbol)(boundLocal.LocalSymbol), ImmutableArray.Create(Of BoundStatement)(boundExpressionStatement, boundReturnStatement), False)).MakeCompilerGenerated()
			End Function
		End Class
	End Class
End Namespace