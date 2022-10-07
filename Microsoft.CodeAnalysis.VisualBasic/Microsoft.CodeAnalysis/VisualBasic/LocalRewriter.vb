Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.Emit.NoPia
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.RuntimeMembers
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Globalization
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Text

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class LocalRewriter
		Inherits BoundTreeRewriterWithStackGuard
		Private ReadOnly _topMethod As MethodSymbol

		Private ReadOnly _emitModule As PEModuleBuilder

		Private ReadOnly _compilationState As TypeCompilationState

		Private ReadOnly _previousSubmissionFields As SynthesizedSubmissionFields

		Private ReadOnly _diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag

		Private ReadOnly _instrumenterOpt As Instrumenter

		Private _symbolsCapturedWithoutCopyCtor As ISet(Of Symbol)

		Private _currentMethodOrLambda As MethodSymbol

		Private _rangeVariableMap As Dictionary(Of RangeVariableSymbol, BoundExpression)

		Private _placeholderReplacementMapDoNotUseDirectly As Dictionary(Of BoundValuePlaceholderBase, BoundExpression)

		Private _hasLambdas As Boolean

		Private _inExpressionLambda As Boolean

		Private _staticLocalMap As Dictionary(Of LocalSymbol, KeyValuePair(Of SynthesizedStaticLocalBackingField, SynthesizedStaticLocalBackingField))

		Private _xmlFixupData As LocalRewriter.XmlLiteralFixupData

		Private _xmlImportedNamespaces As ImmutableArray(Of KeyValuePair(Of String, String))

		Private _unstructuredExceptionHandling As LocalRewriter.UnstructuredExceptionHandlingState

		Private _currentLineTemporary As LocalSymbol

		Private _instrumentTopLevelNonCompilerGeneratedExpressionsInQuery As Boolean

		Private _conditionalAccessReceiverPlaceholderId As Integer

		Private ReadOnly _flags As LocalRewriter.RewritingFlags

		Private Const s_activeHandler_None As Integer = 0

		Private Const s_activeHandler_ResumeNext As Integer = 1

		Private Const s_activeHandler_FirstNonReservedOnErrorGotoIndex As Integer = 2

		Private Const s_activeHandler_FirstOnErrorResumeNextIndex As Integer = -2

		Private ReadOnly _valueTypesCleanUpCache As Dictionary(Of TypeSymbol, Boolean)

		Private ReadOnly Property Compilation As VisualBasicCompilation
			Get
				Return Me._topMethod.DeclaringCompilation
			End Get
		End Property

		Private ReadOnly Property ContainingAssembly As SourceAssemblySymbol
			Get
				Return DirectCast(Me._topMethod.ContainingAssembly, SourceAssemblySymbol)
			End Get
		End Property

		Private ReadOnly Property Instrument As Boolean
			Get
				If (Me._instrumenterOpt Is Nothing) Then
					Return False
				End If
				Return Not Me._inExpressionLambda
			End Get
		End Property

		Private ReadOnly Property Instrument(ByVal original As BoundNode, ByVal rewritten As BoundNode) As Boolean
			Get
				If (Not Me.Instrument OrElse rewritten Is Nothing OrElse original.WasCompilerGenerated) Then
					Return False
				End If
				Return original.Syntax IsNot Nothing
			End Get
		End Property

		Private ReadOnly Property Instrument(ByVal original As BoundNode) As Boolean
			Get
				If (Not Me.Instrument OrElse original.WasCompilerGenerated) Then
					Return False
				End If
				Return original.Syntax IsNot Nothing
			End Get
		End Property

		Public ReadOnly Property OptimizationLevelIsDebug As Boolean
			Get
				Return Me.Compilation.Options.OptimizationLevel = OptimizationLevel.Debug
			End Get
		End Property

		Private ReadOnly Property PlaceholderReplacement(ByVal placeholder As BoundValuePlaceholderBase) As BoundExpression
			Get
				Return Me._placeholderReplacementMapDoNotUseDirectly(placeholder)
			End Get
		End Property

		Private Sub New(ByVal topMethod As MethodSymbol, ByVal currentMethod As MethodSymbol, ByVal compilationState As TypeCompilationState, ByVal previousSubmissionFields As SynthesizedSubmissionFields, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal flags As LocalRewriter.RewritingFlags, ByVal instrumenterOpt As Instrumenter, ByVal recursionDepth As Integer)
			MyBase.New(recursionDepth)
			Me._xmlFixupData = New LocalRewriter.XmlLiteralFixupData()
			Me._valueTypesCleanUpCache = New Dictionary(Of TypeSymbol, Boolean)()
			Me._topMethod = topMethod
			Me._currentMethodOrLambda = currentMethod
			Me._emitModule = compilationState.ModuleBuilderOpt
			Me._compilationState = compilationState
			Me._previousSubmissionFields = previousSubmissionFields
			Me._diagnostics = diagnostics
			Me._flags = flags
			Me._instrumenterOpt = instrumenterOpt
		End Sub

		Private Sub AddPlaceholderReplacement(ByVal placeholder As BoundValuePlaceholderBase, ByVal value As BoundExpression)
			If (Me._placeholderReplacementMapDoNotUseDirectly Is Nothing) Then
				Me._placeholderReplacementMapDoNotUseDirectly = New Dictionary(Of BoundValuePlaceholderBase, BoundExpression)()
			End If
			Me._placeholderReplacementMapDoNotUseDirectly.Add(placeholder, value)
		End Sub

		Private Function AddResumeTargetLabel(ByVal syntax As SyntaxNode) As BoundLabelStatement
			Dim generatedUnstructuredExceptionHandlingResumeLabel As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedUnstructuredExceptionHandlingResumeLabel = New Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedUnstructuredExceptionHandlingResumeLabel(Me._unstructuredExceptionHandling.Context.ResumeWithoutLabelOpt)
			Me._unstructuredExceptionHandling.ResumeTargets.Add(New BoundGotoStatement(syntax, generatedUnstructuredExceptionHandlingResumeLabel, Nothing, False))
			Return New BoundLabelStatement(syntax, generatedUnstructuredExceptionHandlingResumeLabel)
		End Function

		Private Sub AddResumeTargetLabelAndUpdateCurrentStatementTemporary(ByVal syntax As SyntaxNode, ByVal canThrow As Boolean, ByVal statements As ArrayBuilder(Of BoundStatement))
			statements.Add(Me.AddResumeTargetLabel(syntax))
			If (canThrow) Then
				Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = New Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory(Me._topMethod, Me._currentMethodOrLambda, syntax, Me._compilationState, Me._diagnostics)
				statements.Add(syntheticBoundNodeFactory.AssignmentExpression(syntheticBoundNodeFactory.Local(Me._unstructuredExceptionHandling.CurrentStatementTemporary, True), syntheticBoundNodeFactory.Literal(Me._unstructuredExceptionHandling.ResumeTargets.Count)).ToStatement())
			End If
		End Sub

		Private Shared Function AdjustIfOptimizableForConditionalBranch(ByVal operand As BoundExpression, <Out> ByRef optimizableForConditionalBranch As Boolean) As BoundExpression
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundExpression Microsoft.CodeAnalysis.VisualBasic.LocalRewriter::AdjustIfOptimizableForConditionalBranch(Microsoft.CodeAnalysis.VisualBasic.BoundExpression,System.Boolean&)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundExpression AdjustIfOptimizableForConditionalBranch(Microsoft.CodeAnalysis.VisualBasic.BoundExpression,System.Boolean&)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Shared Function AppendToBlock(ByVal block As Microsoft.CodeAnalysis.VisualBasic.BoundBlock, ByVal additionOpt As BoundStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			If (additionOpt IsNot Nothing) Then
				Dim statements As ImmutableArray(Of BoundStatement) = block.Statements
				Dim item(statements.Length + 1 - 1) As BoundStatement
				statements = block.Statements
				Dim length As Integer = statements.Length - 1
				Dim num As Integer = 0
				Do
					statements = block.Statements
					item(num) = statements(num)
					num = num + 1
				Loop While num <= length
				statements = block.Statements
				item(statements.Length) = additionOpt
				boundBlock = block.Update(block.StatementListSyntax, block.Locals, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundStatement)(item))
			Else
				boundBlock = block
			End If
			Return boundBlock
		End Function

		Private Function ApplyUnliftedBinaryOp(ByVal originalOperator As BoundBinaryOperator, ByVal left As BoundExpression, ByVal right As BoundExpression) As BoundExpression
			Dim operatorKind As BinaryOperatorKind = originalOperator.OperatorKind And (BinaryOperatorKind.Add Or BinaryOperatorKind.Concatenate Or BinaryOperatorKind.[Like] Or BinaryOperatorKind.Equals Or BinaryOperatorKind.NotEquals Or BinaryOperatorKind.LessThanOrEqual Or BinaryOperatorKind.GreaterThanOrEqual Or BinaryOperatorKind.LessThan Or BinaryOperatorKind.GreaterThan Or BinaryOperatorKind.Subtract Or BinaryOperatorKind.Multiply Or BinaryOperatorKind.Power Or BinaryOperatorKind.Divide Or BinaryOperatorKind.Modulo Or BinaryOperatorKind.IntegerDivide Or BinaryOperatorKind.LeftShift Or BinaryOperatorKind.RightShift Or BinaryOperatorKind.[Xor] Or BinaryOperatorKind.[Or] Or BinaryOperatorKind.[OrElse] Or BinaryOperatorKind.[And] Or BinaryOperatorKind.[AndAlso] Or BinaryOperatorKind.[Is] Or BinaryOperatorKind.[IsNot] Or BinaryOperatorKind.OpMask Or BinaryOperatorKind.CompareText Or BinaryOperatorKind.UserDefined Or BinaryOperatorKind.[Error] Or BinaryOperatorKind.IsOperandOfConditionalBranch Or BinaryOperatorKind.OptimizableForConditionalBranch)
			Return Me.MakeBinaryExpression(originalOperator.Syntax, operatorKind, left, right, originalOperator.Checked, originalOperator.Type.GetNullableUnderlyingType())
		End Function

		Private Function ApplyUnliftedUnaryOp(ByVal originalOperator As BoundUnaryOperator, ByVal operandValue As BoundExpression) As BoundExpression
			Dim operatorKind As UnaryOperatorKind = originalOperator.OperatorKind And (UnaryOperatorKind.Plus Or UnaryOperatorKind.Minus Or UnaryOperatorKind.[Not] Or UnaryOperatorKind.IntrinsicOpMask Or UnaryOperatorKind.UserDefined Or UnaryOperatorKind.Implicit Or UnaryOperatorKind.Explicit Or UnaryOperatorKind.IsTrue Or UnaryOperatorKind.IsFalse Or UnaryOperatorKind.OpMask Or UnaryOperatorKind.[Error])
			Return Me.RewriteUnaryOperator(New BoundUnaryOperator(originalOperator.Syntax, operatorKind, operandValue, originalOperator.Checked, originalOperator.Type.GetNullableUnderlyingType(), False))
		End Function

		<Conditional("DEBUG")>
		Private Shared Sub AssertIsWriteableFromMember(ByVal node As BoundPropertyAccess, ByVal fromMember As Symbol)
			Dim receiverOpt As BoundExpression = node.ReceiverOpt
			Dim propertySymbol As SourcePropertySymbol = DirectCast(node.PropertySymbol, SourcePropertySymbol)
			Dim isShared As Boolean = node.PropertySymbol.IsShared
		End Sub

		<Conditional("DEBUG")>
		Private Shared Sub AssertPlaceholderReplacement(ByVal placeholder As BoundValuePlaceholderBase, ByVal value As BoundExpression)
			If (placeholder.IsLValue) Then
				Dim kind As BoundKind = value.Kind
			End If
		End Sub

		Private Function BindXmlNamespace(ByVal syntax As SyntaxNode, ByVal [namespace] As BoundExpression) As BoundExpression
			Dim wellKnownTypeMember As MethodSymbol = DirectCast(Me.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Xml_Linq_XNamespace__Get), MethodSymbol)
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)([namespace])
			Dim returnType As TypeSymbol = wellKnownTypeMember.ReturnType
			Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
			Return (New BoundCall(syntax, wellKnownTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)).MakeCompilerGenerated()
		End Function

		Private Shared Function BuildDelegateRelaxationLambda(ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal targetType As NamedTypeSymbol, ByVal boundMember As BoundLateMemberAccess, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim delegateInvokeMethod As MethodSymbol = targetType.DelegateInvokeMethod
			Dim returnType As TypeSymbol = delegateInvokeMethod.ReturnType
			Dim parameters As ImmutableArray(Of ParameterSymbol) = delegateInvokeMethod.Parameters
			Dim length As Integer = parameters.Length
			Dim boundLambdaParameterSymbol(length - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.Symbols.BoundLambdaParameterSymbol
			Dim location As Microsoft.CodeAnalysis.Location = syntaxNode.GetLocation()
			Dim num As Integer = length - 1
			Dim num1 As Integer = 0
			Do
				Dim item As ParameterSymbol = parameters(num1)
				boundLambdaParameterSymbol(num1) = New Microsoft.CodeAnalysis.VisualBasic.Symbols.BoundLambdaParameterSymbol(GeneratedNames.MakeDelegateRelaxationParameterName(num1), item.Ordinal, item.Type, item.IsByRef, syntaxNode, location)
				num1 = num1 + 1
			Loop While num1 <= num
			Dim synthesizedLambdaSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLambdaSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLambdaSymbol(SynthesizedLambdaKind.LateBoundAddressOfLambda, syntaxNode, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.BoundLambdaParameterSymbol)(boundLambdaParameterSymbol), returnType, binder)
			Dim boundExpressionArray(length - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim length1 As Integer = CInt(boundLambdaParameterSymbol.Length) - 1
			Dim num2 As Integer = 0
			Do
				Dim boundLambdaParameterSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.BoundLambdaParameterSymbol = boundLambdaParameterSymbol(num2)
				Dim boundParameter As Microsoft.CodeAnalysis.VisualBasic.BoundParameter = New Microsoft.CodeAnalysis.VisualBasic.BoundParameter(syntaxNode, boundLambdaParameterSymbol1, boundLambdaParameterSymbol1.Type)
				boundParameter.SetWasCompilerGenerated()
				boundExpressionArray(num2) = boundParameter
				num2 = num2 + 1
			Loop While num2 <= length1
			Dim lambdaBodyBinder As Microsoft.CodeAnalysis.VisualBasic.LambdaBodyBinder = New Microsoft.CodeAnalysis.VisualBasic.LambdaBodyBinder(synthesizedLambdaSymbol, binder)
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpressionArray)
			Dim strs As ImmutableArray(Of String) = New ImmutableArray(Of String)()
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = lambdaBodyBinder.BindLateBoundInvocation(syntaxNode, Nothing, boundMember, boundExpressions, strs, diagnostics, True)
			boundExpression.SetWasCompilerGenerated()
			Dim boundStatements As ImmutableArray(Of BoundStatement) = New ImmutableArray(Of BoundStatement)()
			If (Not synthesizedLambdaSymbol.IsSub) Then
				boundExpression = lambdaBodyBinder.ApplyImplicitConversion(syntaxNode, returnType, boundExpression, diagnostics, False)
				Dim boundReturnStatement As Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement(syntaxNode, boundExpression, Nothing, Nothing, False)
				boundReturnStatement.SetWasCompilerGenerated()
				boundStatements = ImmutableArray.Create(Of BoundStatement)(boundReturnStatement)
			Else
				If (boundExpression.IsLateBound()) Then
					boundExpression = boundExpression.SetLateBoundAccessKind(LateBoundAccessKind.[Call])
				End If
				Dim boundStatementArray(1) As BoundStatement
				Dim boundExpressionStatement As BoundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement(syntaxNode, boundExpression, False)
				boundExpressionStatement.SetWasCompilerGenerated()
				boundStatementArray(0) = boundExpressionStatement
				boundExpressionStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement(syntaxNode, Nothing, Nothing, Nothing, False)
				boundExpressionStatement.SetWasCompilerGenerated()
				boundStatementArray(1) = boundExpressionStatement
				boundStatements = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundStatement)(boundStatementArray)
			End If
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(syntaxNode, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, boundStatements, False)
			boundBlock.SetWasCompilerGenerated()
			Dim boundLambda As Microsoft.CodeAnalysis.VisualBasic.BoundLambda = New Microsoft.CodeAnalysis.VisualBasic.BoundLambda(syntaxNode, synthesizedLambdaSymbol, boundBlock, ImmutableBindingDiagnostic(Of AssemblySymbol).Empty, Nothing, ConversionKind.DelegateRelaxationLevelWidening, MethodConversionKind.Identity, False)
			boundLambda.SetWasCompilerGenerated()
			Return New BoundDirectCast(syntaxNode, boundLambda, ConversionKind.DelegateRelaxationLevelWidening, targetType, False)
		End Function

		Private Shared Function CacheToLocalIfNotConst(ByVal container As Symbol, ByVal value As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal locals As ArrayBuilder(Of LocalSymbol), ByVal expressions As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression), ByVal kind As SynthesizedLocalKind, ByVal syntaxOpt As StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal
			Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator
			Dim constantValueOpt As ConstantValue = value.ConstantValueOpt
			If (constantValueOpt IsNot Nothing) Then
				If (value.Type.IsDecimalType()) Then
					Dim decimalValue As [Decimal] = constantValueOpt.DecimalValue
					If ([Decimal].Compare(decimalValue, [Decimal].MinusOne) <> 0 AndAlso [Decimal].Compare(decimalValue, [Decimal].Zero) <> 0 AndAlso [Decimal].Compare(decimalValue, [Decimal].One) <> 0) Then
						synthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(container, value.Type, kind, syntaxOpt, False)
						locals.Add(synthesizedLocal)
						boundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(value.Syntax, synthesizedLocal, synthesizedLocal.Type)
						boundAssignmentOperator = (New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(value.Syntax, boundLocal, value, True, boundLocal.Type, False)).MakeCompilerGenerated()
						expressions.Add(boundAssignmentOperator)
						boundExpression = boundLocal.MakeRValue()
						Return boundExpression
					End If
					boundExpression = value
					Return boundExpression
				Else
					boundExpression = value
					Return boundExpression
				End If
			End If
			synthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(container, value.Type, kind, syntaxOpt, False)
			locals.Add(synthesizedLocal)
			boundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(value.Syntax, synthesizedLocal, synthesizedLocal.Type)
			boundAssignmentOperator = (New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(value.Syntax, boundLocal, value, True, boundLocal.Type, False)).MakeCompilerGenerated()
			expressions.Add(boundAssignmentOperator)
			boundExpression = boundLocal.MakeRValue()
			Return boundExpression
		End Function

		Private Function CaptureNullableIfNeeded(ByVal operand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, <Out> ByRef temp As SynthesizedLocal, <Out> ByRef init As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal doNotCaptureLocals As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			temp = Nothing
			init = Nothing
			If (Not operand.IsConstant) Then
				If (doNotCaptureLocals) Then
					If (operand.Kind <> BoundKind.Local OrElse DirectCast(operand, BoundLocal).LocalSymbol.IsByRef) Then
						If (operand.Kind <> BoundKind.Parameter OrElse DirectCast(operand, BoundParameter).ParameterSymbol.IsByRef) Then
							GoTo Label1
						End If
						boundExpression = operand
						Return boundExpression
					Else
						boundExpression = operand
						Return boundExpression
					End If
				End If
			Label1:
				boundExpression = Me.CaptureOperand(operand, temp, init)
			Else
				boundExpression = operand
			End If
			Return boundExpression
		End Function

		Private Function CaptureNullableIfNeeded(ByVal operand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, <InAttribute> <Out> ByRef temps As ArrayBuilder(Of LocalSymbol), <InAttribute> <Out> ByRef inits As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression), ByVal doNotCaptureLocals As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = Nothing
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.CaptureNullableIfNeeded(operand, synthesizedLocal, boundExpression, doNotCaptureLocals)
			If (synthesizedLocal IsNot Nothing) Then
				temps = If(temps, ArrayBuilder(Of LocalSymbol).GetInstance())
				temps.Add(synthesizedLocal)
				inits = If(inits, ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance())
				inits.Add(boundExpression)
			End If
			Return boundExpression1
		End Function

		Private Function CaptureOperand(ByVal operand As BoundExpression, <Out> ByRef temp As SynthesizedLocal, <Out> ByRef init As BoundExpression) As BoundExpression
			temp = New SynthesizedLocal(Me._currentMethodOrLambda, operand.Type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(operand.Syntax, temp, True, temp.Type)
			init = New BoundAssignmentOperator(operand.Syntax, boundLocal, operand, True, operand.Type, False)
			Return boundLocal.MakeRValue()
		End Function

		Private Shared Function Concat(ByVal statement As BoundStatement, ByVal additionOpt As BoundStatement) As BoundStatement
			Dim boundStatementList As BoundStatement
			If (additionOpt IsNot Nothing) Then
				Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = TryCast(statement, Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
				If (boundBlock Is Nothing) Then
					Dim boundStatementArray() As BoundStatement = { statement, additionOpt }
					boundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(statement.Syntax, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundStatement)(boundStatementArray), False)
				Else
					Dim statements As ImmutableArray(Of BoundStatement) = boundBlock.Statements
					Dim item(statements.Length + 1 - 1) As BoundStatement
					statements = boundBlock.Statements
					Dim length As Integer = statements.Length - 1
					Dim num As Integer = 0
					Do
						statements = boundBlock.Statements
						item(num) = statements(num)
						num = num + 1
					Loop While num <= length
					statements = boundBlock.Statements
					item(statements.Length) = additionOpt
					boundStatementList = boundBlock.Update(boundBlock.StatementListSyntax, boundBlock.Locals, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundStatement)(item))
				End If
			Else
				boundStatementList = statement
			End If
			Return boundStatementList
		End Function

		Private Function Convert(ByVal factory As SyntheticBoundNodeFactory, ByVal type As TypeSymbol, ByVal expr As BoundExpression) As BoundExpression
			Return Me.TransformRewrittenConversion(factory.Convert(type, expr, False))
		End Function

		Private Function CouldPossiblyBeNothing(ByVal F As SyntheticBoundNodeFactory, ByVal node As BoundExpression) As Boolean
			Dim flag As Boolean
			Dim kind As BoundKind = node.Kind
			If (kind <= BoundKind.Conversion) Then
				If (kind = BoundKind.TernaryConditionalExpression) Then
					Dim boundTernaryConditionalExpression As Microsoft.CodeAnalysis.VisualBasic.BoundTernaryConditionalExpression = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundTernaryConditionalExpression)
					flag = If(Me.CouldPossiblyBeNothing(F, boundTernaryConditionalExpression.WhenTrue), True, Me.CouldPossiblyBeNothing(F, boundTernaryConditionalExpression.WhenFalse))
				Else
					If (kind <> BoundKind.Conversion) Then
						flag = True
						Return flag
					End If
					flag = Me.CouldPossiblyBeNothing(F, DirectCast(node, BoundConversion).Operand)
				End If
			ElseIf (kind = BoundKind.[Call]) Then
				Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundCall)
				flag = If(boundCall.Method = F.WellKnownMember(Of MethodSymbol)(WellKnownMember.System_Delegate__CreateDelegate, True) OrElse boundCall.Method = F.WellKnownMember(Of MethodSymbol)(WellKnownMember.System_Delegate__CreateDelegate4, True), False, Not (boundCall.Method = F.WellKnownMember(Of MethodSymbol)(WellKnownMember.System_Reflection_MethodInfo__CreateDelegate, True)))
			Else
				If (kind <> BoundKind.Lambda) Then
					flag = True
					Return flag
				End If
				flag = False
			End If
			Return flag
		End Function

		Private Function CreateBackingFieldsForStaticLocal(ByVal localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, ByVal hasInitializer As Boolean) As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedStaticLocalBackingField, Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedStaticLocalBackingField)
			Dim synthesizedStaticLocalBackingField As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedStaticLocalBackingField
			If (Me._staticLocalMap Is Nothing) Then
				Me._staticLocalMap = New Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedStaticLocalBackingField, Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedStaticLocalBackingField))(ReferenceEqualityComparer.Instance)
			End If
			Dim synthesizedStaticLocalBackingField1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedStaticLocalBackingField = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedStaticLocalBackingField(localSymbol, True, Not hasInitializer)
			If (hasInitializer) Then
				synthesizedStaticLocalBackingField = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedStaticLocalBackingField(localSymbol, False, True)
			Else
				synthesizedStaticLocalBackingField = Nothing
			End If
			Dim keyValuePair As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedStaticLocalBackingField, Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedStaticLocalBackingField) = New KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedStaticLocalBackingField, Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedStaticLocalBackingField)(synthesizedStaticLocalBackingField1, synthesizedStaticLocalBackingField)
			If (Me._emitModule IsNot Nothing) Then
				Me._emitModule.AddSynthesizedDefinition(Me._topMethod.ContainingType, keyValuePair.Key.GetCciAdapter())
				If (keyValuePair.Value IsNot Nothing) Then
					Me._emitModule.AddSynthesizedDefinition(Me._topMethod.ContainingType, keyValuePair.Value.GetCciAdapter())
				End If
			End If
			Me._staticLocalMap.Add(localSymbol, keyValuePair)
			Return keyValuePair
		End Function

		Private Function CreateCompilerGeneratedArray(ByVal syntax As SyntaxNode, ByVal arrayType As TypeSymbol, ByVal items As ImmutableArray(Of BoundExpression)) As BoundExpression
			Dim boundArrayCreation As BoundExpression
			If (items.Length <> 0) Then
				Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = (New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntax, ConstantValue.Create(items.Length), Me.GetSpecialType(SpecialType.System_Int32))).MakeCompilerGenerated()
				Dim boundArrayInitialization As Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization = (New Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization(syntax, items, arrayType, False)).MakeCompilerGenerated()
				boundArrayCreation = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation(syntax, ImmutableArray.Create(Of BoundExpression)(boundLiteral), boundArrayInitialization, arrayType, False)
			Else
				boundArrayCreation = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntax, ConstantValue.[Nothing], arrayType)
			End If
			boundArrayCreation.SetWasCompilerGenerated()
			Return boundArrayCreation
		End Function

		Private Function CreateCompilerGeneratedXmlNamespace(ByVal syntax As SyntaxNode, ByVal [namespace] As String) As BoundExpression
			Return Me.BindXmlNamespace(syntax, Me.CreateStringLiteral(syntax, [namespace]))
		End Function

		Private Function CreateCompilerGeneratedXmlnsPrefix(ByVal syntax As SyntaxNode, ByVal prefix As String) As BoundExpression
			Return Me.CreateStringLiteral(syntax, If(EmbeddedOperators.CompareString(prefix, "", False) = 0, "xmlns", prefix))
		End Function

		Private Function CreateIndexIncrement(ByVal syntaxNode As VisualBasicSyntaxNode, ByVal boundIndex As BoundLocal) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim type As TypeSymbol = boundIndex.Type
			Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator(syntaxNode, BinaryOperatorKind.Add, boundIndex.MakeRValue(), New BoundLiteral(syntaxNode, ConstantValue.Create(1), type), True, type, False)
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = (New BoundAssignmentOperator(syntaxNode, boundIndex, boundBinaryOperator, False, type, False)).ToStatement().MakeCompilerGenerated()
			boundStatement = DirectCast(Me.Visit(boundStatement), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			If (Me.Instrument) Then
				boundStatement = SyntheticBoundNodeFactory.HiddenSequencePoint(boundStatement)
			End If
			Return boundStatement
		End Function

		Private Function CreateLocalAndAssignment(ByVal syntaxNode As StatementSyntax, ByVal initExpression As BoundExpression, <Out> ByRef boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal, ByVal locals As ArrayBuilder(Of LocalSymbol), ByVal kind As SynthesizedLocalKind) As BoundStatement
			Dim type As TypeSymbol = initExpression.Type
			Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, type, kind, syntaxNode, False)
			locals.Add(synthesizedLocal)
			boundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntaxNode, synthesizedLocal, type)
			Dim statement As BoundExpressionStatement = (New BoundAssignmentOperator(syntaxNode, boundLocal, Me.VisitAndGenerateObjectCloneIfNeeded(initExpression, False), True, type, False)).ToStatement()
			statement.SetWasCompilerGenerated()
			Return statement
		End Function

		Private Function CreateLoweredWhileStatements(ByVal forEachStatement As BoundForEachStatement, ByVal limit As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal index As BoundLocal, ByVal currentAssignment As Microsoft.CodeAnalysis.VisualBasic.BoundStatement, ByVal incrementAssignment As Microsoft.CodeAnalysis.VisualBasic.BoundStatement, ByVal generateUnstructuredExceptionHandlingResumeCode As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundStatementList
			Dim empty As ImmutableArray(Of LocalSymbol)
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(forEachStatement.Body), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = forEachStatement.Syntax
			Dim boundStatementList As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Nothing
			If (generateUnstructuredExceptionHandlingResumeCode) Then
				boundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(syntax, Me.RegisterUnstructuredExceptionHandlingResumeTarget(syntax, True), False)
			End If
			If (Me(forEachStatement)) Then
				boundStatementList = Me._instrumenterOpt.InstrumentForEachLoopEpilogue(forEachStatement, boundStatementList)
			End If
			If (boundStatementList IsNot Nothing) Then
				incrementAssignment = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(syntax, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(boundStatementList, incrementAssignment), False)
			End If
			Dim boundStatements As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(currentAssignment, boundStatement, New BoundLabelStatement(syntax, forEachStatement.ContinueLabel), incrementAssignment)
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = syntax
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
			If (forEachStatement.DeclaredOrInferredLocalOpt IsNot Nothing) Then
				empty = ImmutableArray.Create(Of LocalSymbol)(forEachStatement.DeclaredOrInferredLocalOpt)
			Else
				empty = ImmutableArray(Of LocalSymbol).Empty
			End If
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(syntaxNode, statementSyntaxes, empty, boundStatements, False)
			Dim specialTypeWithUseSiteDiagnostics As NamedTypeSymbol = Me.GetSpecialTypeWithUseSiteDiagnostics(SpecialType.System_Boolean, syntax)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.TransformRewrittenBinaryOperator(New BoundBinaryOperator(syntax, BinaryOperatorKind.LessThan, index.MakeRValue(), limit, False, specialTypeWithUseSiteDiagnostics, False))
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(boundExpression)
			Dim generatedLabelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol("postIncrement")
			Dim exitLabel As LabelSymbol = forEachStatement.ExitLabel
			Dim boundStatements1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)()
			Return DirectCast(Me.RewriteWhileStatement(forEachStatement, boundExpression1, boundBlock, generatedLabelSymbol, exitLabel, True, Nothing, boundStatements1, Nothing), Microsoft.CodeAnalysis.VisualBasic.BoundStatementList)
		End Function

		Private Sub CreatePrefixesAndNamespacesArrays(ByVal rewriterInfo As BoundXmlContainerRewriterInfo, ByVal syntax As SyntaxNode, <Out> ByRef prefixes As BoundExpression, <Out> ByRef namespaces As BoundExpression)
			Dim instance As ArrayBuilder(Of BoundExpression) = ArrayBuilder(Of BoundExpression).GetInstance()
			Dim boundExpressions As ArrayBuilder(Of BoundExpression) = ArrayBuilder(Of BoundExpression).GetInstance()
			Dim enumerator As ImmutableArray(Of KeyValuePair(Of String, String)).Enumerator = Me._xmlImportedNamespaces.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As KeyValuePair(Of String, String) = enumerator.Current
				instance.Add(Me.CreateCompilerGeneratedXmlnsPrefix(syntax, current.Key))
				boundExpressions.Add(Me.CreateCompilerGeneratedXmlNamespace(syntax, current.Value))
			End While
			Dim enumerator1 As ImmutableArray(Of KeyValuePair(Of String, String)).Enumerator = rewriterInfo.InScopeXmlNamespaces.GetEnumerator()
			While enumerator1.MoveNext()
				Dim keyValuePair As KeyValuePair(Of String, String) = enumerator1.Current
				instance.Add(Me.CreateCompilerGeneratedXmlnsPrefix(syntax, keyValuePair.Key))
				boundExpressions.Add(Me.CreateCompilerGeneratedXmlNamespace(syntax, keyValuePair.Value))
			End While
			prefixes = Me.VisitExpressionNode(Me.CreateCompilerGeneratedArray(syntax, rewriterInfo.PrefixesPlaceholder.Type, instance.ToImmutableAndFree()))
			namespaces = Me.VisitExpressionNode(Me.CreateCompilerGeneratedArray(syntax, rewriterInfo.NamespacesPlaceholder.Type, boundExpressions.ToImmutableAndFree()))
		End Sub

		Friend Shared Function CreateReturnStatementForQueryLambdaBody(ByVal rewrittenBody As BoundExpression, ByVal originalNode As BoundQueryLambda, Optional ByVal hasErrors As Boolean = False) As BoundStatement
			Return (New BoundReturnStatement(originalNode.Syntax, rewrittenBody, Nothing, Nothing, hasErrors)).MakeCompilerGenerated()
		End Function

		Private Function CreateStringLiteral(ByVal syntax As SyntaxNode, ByVal str As String) As BoundLiteral
			Return (New BoundLiteral(syntax, ConstantValue.Create(str), Me.GetSpecialType(SpecialType.System_String))).MakeCompilerGenerated()
		End Function

		Private Function CreateTempLocal(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, ByVal expr As BoundExpression, ByVal sideEffects As ArrayBuilder(Of BoundExpression)) As Microsoft.CodeAnalysis.VisualBasic.BoundLocal
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, New SynthesizedLocal(Me._currentMethodOrLambda, type, SynthesizedLocalKind.LoweringTemp, Nothing, False), type)
			sideEffects.Add(New BoundAssignmentOperator(syntax, boundLocal, expr, True, type, False))
			Return boundLocal.MakeRValue()
		End Function

		Private Function CreateTempLocalInExpressionLambda(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol, ByVal expr As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundLocal
			Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._topMethod, type, SynthesizedLocalKind.XmlInExpressionLambda, Me._currentMethodOrLambda.Syntax, False)
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, synthesizedLocal, type)
			Me._xmlFixupData.AddLocal(synthesizedLocal, New BoundAssignmentOperator(syntax, boundLocal, expr, True, type, False))
			Return boundLocal.MakeRValue()
		End Function

		Private Function EnforceStaticLocalInitializationSemantics(ByVal staticLocalBackingFields As KeyValuePair(Of SynthesizedStaticLocalBackingField, SynthesizedStaticLocalBackingField), ByVal rewrittenInitialization As Microsoft.CodeAnalysis.VisualBasic.BoundStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim empty As ImmutableArray(Of LocalSymbol)
			Dim boundStatements As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Dim boundMeReference As BoundExpression
			Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = rewrittenInitialization.Syntax
			Dim specialTypeWithUseSiteDiagnostics As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.GetSpecialTypeWithUseSiteDiagnostics(SpecialType.System_Object, syntax)
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.GetSpecialTypeWithUseSiteDiagnostics(SpecialType.System_Boolean, syntax)
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = Nothing
			Dim methodSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			If (Not (If(specialTypeWithUseSiteDiagnostics.IsErrorType(), True, namedTypeSymbol.IsErrorType()) Or Not Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, WellKnownMember.Microsoft_VisualBasic_CompilerServices_StaticLocalInitFlag__ctor, syntax, False) Or Not Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol1, WellKnownMember.System_Threading_Interlocked__CompareExchange_T, syntax, False) Or Not Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)(fieldSymbol, WellKnownMember.Microsoft_VisualBasic_CompilerServices_StaticLocalInitFlag__State, syntax, False) Or Not Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol2, WellKnownMember.Microsoft_VisualBasic_CompilerServices_IncompleteInitialization__ctor, syntax, False))) Then
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement).GetInstance()
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = syntax
				If (Me._topMethod.IsShared) Then
					boundMeReference = Nothing
				Else
					boundMeReference = New Microsoft.CodeAnalysis.VisualBasic.BoundMeReference(syntax, Me._topMethod.ContainingType)
				End If
				Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(syntaxNode, boundMeReference, staticLocalBackingFields.Value, True, staticLocalBackingFields.Value.Type, False)
				Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me.GetNewCompoundUseSiteInfo()
				Dim boundDirectCast As Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast = New Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast(syntax, boundFieldAccess.MakeRValue(), Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(boundFieldAccess.Type, specialTypeWithUseSiteDiagnostics, newCompoundUseSiteInfo), specialTypeWithUseSiteDiagnostics, False)
				Me._diagnostics.Add(syntax, newCompoundUseSiteInfo)
				Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator(syntax, BinaryOperatorKind.[Is], boundDirectCast, New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntax, ConstantValue.[Nothing], specialTypeWithUseSiteDiagnostics), False, namedTypeSymbol, False)
				Dim u00210s As ImmutableArray(Of !0) = ImmutableArray(Of BoundExpression).Empty
				Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = boundFieldAccess.Type
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundObjectCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression(syntax, methodSymbol, u00210s, Nothing, type, False, bitVector)
				Dim methodSymbol3 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = methodSymbol1.Construct(New Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol() { boundFieldAccess.Type })
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(boundFieldAccess, boundObjectCreationExpression, New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntax, ConstantValue.[Nothing], boundFieldAccess.Type))
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = boundFieldAccess.Type
				bitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, methodSymbol3, Nothing, Nothing, boundExpressions, Nothing, typeSymbol, False, False, bitVector)
				Dim statement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = boundCall.ToStatement()
				Dim boundStatements1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)()
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.RewriteIfStatement(syntax, boundBinaryOperator, statement, Nothing, Nothing, boundStatements1)
				instance.Add(boundStatement)
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Nothing
				Dim boundStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Nothing
				Dim boundStatement2 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.GenerateMonitorEnter(syntax, boundDirectCast, boundLocal, boundStatement1)
				Dim boundFieldAccess1 As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(syntax, boundFieldAccess, fieldSymbol, True, fieldSymbol.Type, False)
				Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntax, ConstantValue.Create(CShort(2)), boundFieldAccess1.Type)
				Dim boundBinaryOperator1 As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator(syntax, BinaryOperatorKind.Equals, boundFieldAccess1.MakeRValue(), New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntax, ConstantValue.[Default](ConstantValueTypeDiscriminator.Int16), boundFieldAccess1.Type), False, namedTypeSymbol, False)
				Dim boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = (New BoundAssignmentOperator(syntax, boundFieldAccess1, boundLiteral, True, False)).ToStatement()
				Dim boundBinaryOperator2 As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator(syntax, BinaryOperatorKind.Equals, boundFieldAccess1.MakeRValue(), boundLiteral, False, namedTypeSymbol, False)
				Dim empty1 As ImmutableArray(Of !0) = ImmutableArray(Of BoundExpression).Empty
				Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = methodSymbol2.ContainingType
				bitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundThrowStatement As Microsoft.CodeAnalysis.VisualBasic.BoundThrowStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundThrowStatement(syntax, New Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression(syntax, methodSymbol2, empty1, Nothing, containingType, False, bitVector), False)
				Dim boundStatementList As Microsoft.CodeAnalysis.VisualBasic.BoundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(syntax, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(boundExpressionStatement, rewrittenInitialization), False)
				boundStatements1 = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)()
				Dim boundStatement3 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.RewriteIfStatement(syntax, boundBinaryOperator2, boundThrowStatement, Nothing, Nothing, boundStatements1)
				boundStatements1 = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)()
				Dim boundStatement4 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.RewriteIfStatement(syntax, boundBinaryOperator1, boundStatementList, boundStatement3, Nothing, boundStatements1)
				If (boundLocal Is Nothing) Then
					empty = ImmutableArray(Of LocalSymbol).Empty
					instance.Add(boundStatement2)
					boundStatements = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(boundStatement4)
				Else
					empty = ImmutableArray.Create(Of LocalSymbol)(boundLocal.LocalSymbol)
					instance.Add(boundStatement1)
					boundStatements = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(boundStatement2, boundStatement4)
				End If
				Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
				Dim boundBlock1 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, boundStatements, False)
				Dim statement1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = (New BoundAssignmentOperator(syntax, boundFieldAccess1, New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntax, ConstantValue.Create(CShort(1)), boundFieldAccess1.Type), True, False)).ToStatement()
				Dim boundStatement5 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.GenerateMonitorExit(syntax, boundDirectCast, boundLocal)
				statementSyntaxes = New SyntaxList(Of StatementSyntax)()
				Dim boundBlock2 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(statement1, boundStatement5), False)
				Dim boundTryStatement As Microsoft.CodeAnalysis.VisualBasic.BoundTryStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundTryStatement(syntax, boundBlock1, ImmutableArray(Of BoundCatchBlock).Empty, boundBlock2, Nothing, False)
				instance.Add(boundTryStatement)
				statementSyntaxes = New SyntaxList(Of StatementSyntax)()
				boundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(syntax, statementSyntaxes, empty, instance.ToImmutableAndFree(), False)
			Else
				boundBlock = rewrittenInitialization
			End If
			Return boundBlock
		End Function

		Private Sub EnsureStringHashFunction(ByVal node As BoundSelectStatement)
			Dim expression As BoundExpression = node.ExpressionStatement.Expression
			Dim wellKnownType As NamedTypeSymbol = Me.Compilation.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.Microsoft_VisualBasic_CompilerServices_EmbeddedOperators)
			Dim wellKnownMember As Microsoft.CodeAnalysis.WellKnownMember = If(Not wellKnownType.IsErrorType() OrElse Not TypeOf wellKnownType Is MissingMetadataTypeSymbol, Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_EmbeddedOperators__CompareStringStringStringBoolean, Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__CompareStringStringStringBoolean)
			Dim wellKnownTypeMember As MethodSymbol = DirectCast(Me.Compilation.GetWellKnownTypeMember(wellKnownMember), MethodSymbol)
			Me.ReportMissingOrBadRuntimeHelper(expression, wellKnownMember, wellKnownTypeMember)
			Dim specialTypeMember As MethodSymbol = DirectCast(Me.ContainingAssembly.GetSpecialTypeMember(SpecialMember.System_String__Chars), MethodSymbol)
			Me.ReportMissingOrBadRuntimeHelper(expression, SpecialMember.System_String__Chars, specialTypeMember)
			Me.ReportBadType(expression, Me.Compilation.GetSpecialType(SpecialType.System_Int32))
			Me.ReportBadType(expression, Me.Compilation.GetSpecialType(SpecialType.System_UInt32))
			Me.ReportBadType(expression, Me.Compilation.GetSpecialType(SpecialType.System_String))
			If (Me._emitModule IsNot Nothing AndAlso LocalRewriter.ShouldGenerateHashTableSwitch(Me._emitModule, node)) Then
				Dim privateImplClass As PrivateImplementationDetails = Me._emitModule.GetPrivateImplClass(node.Syntax, Me._diagnostics.DiagnosticBag)
				If (privateImplClass.GetMethod("ComputeStringHash") Is Nothing) Then
					Dim synthesizedStringSwitchHashMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedStringSwitchHashMethod = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedStringSwitchHashMethod(Me._emitModule.SourceModule, privateImplClass)
					privateImplClass.TryAddSynthesizedMethod(synthesizedStringSwitchHashMethod.GetCciAdapter())
				End If
			End If
		End Sub

		Private Function EvaluateOperandAndReturnFalse(ByVal node As BoundBinaryOperator, ByVal operand As BoundExpression, ByVal operandHasValue As Boolean) As BoundExpression
			Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(node.Syntax, ConstantValue.[False], node.Type.GetNullableUnderlyingType())
			Return New BoundSequence(node.Syntax, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundExpression)(If(operandHasValue, Me.NullableValueOrDefault(operand), operand)), boundLiteral, boundLiteral.Type, False)
		End Function

		Private Shared Function EventReceiverNeedsTemp(ByVal expression As BoundExpression) As Boolean
			Dim flag As Boolean
			Select Case expression.Kind
				Case BoundKind.Literal
					flag = If(expression.Type Is Nothing, False, Not expression.Type.SpecialType.IsClrInteger())
					Exit Select
				Case BoundKind.MeReference
				Case BoundKind.MyBaseReference
				Case BoundKind.MyClassReference
					flag = False
					Exit Select
				Case BoundKind.ValueTypeMeReference
				Case BoundKind.PreviousSubmissionReference
				Case BoundKind.HostObjectMemberReference
				Case BoundKind.PseudoVariable
				Label0:
					flag = True
					Exit Select
				Case BoundKind.Local
				Case BoundKind.Parameter
					flag = False
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return flag
		End Function

		Private Function FinishNonObjectForLoop(ByVal forStatement As BoundForToStatement, ByVal rewrittenControlVariable As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal rewrittenInitialValue As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal rewrittenLimit As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal rewrittenStep As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As BoundBlock
			Dim syntax As ForBlockSyntax = DirectCast(forStatement.Syntax, ForBlockSyntax)
			Dim flag As Boolean = Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(forStatement)
			Dim boundStatements As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)()
			If (flag) Then
				boundStatements = Me.RegisterUnstructuredExceptionHandlingResumeTarget(syntax, True)
			End If
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
			Dim flag1 As Boolean = LocalRewriter.WillDoAtLeastOneIteration(rewrittenInitialValue, rewrittenLimit, rewrittenStep)
			Dim localSymbols As ArrayBuilder(Of LocalSymbol) = ArrayBuilder(Of LocalSymbol).GetInstance()
			rewrittenInitialValue = LocalRewriter.CacheToLocalIfNotConst(Me._currentMethodOrLambda, rewrittenInitialValue, localSymbols, instance, SynthesizedLocalKind.ForInitialValue, syntax)
			rewrittenLimit = LocalRewriter.CacheToLocalIfNotConst(Me._currentMethodOrLambda, rewrittenLimit, localSymbols, instance, SynthesizedLocalKind.ForLimit, syntax)
			rewrittenStep = LocalRewriter.CacheToLocalIfNotConst(Me._currentMethodOrLambda, rewrittenStep, localSymbols, instance, SynthesizedLocalKind.ForStep, syntax)
			Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = Nothing
			If (forStatement.OperatorsOpt IsNot Nothing) Then
				Me.AddPlaceholderReplacement(forStatement.OperatorsOpt.LeftOperandPlaceholder, rewrittenStep)
				Me.AddPlaceholderReplacement(forStatement.OperatorsOpt.RightOperandPlaceholder, rewrittenStep)
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(forStatement.OperatorsOpt.Subtraction)
				Me.UpdatePlaceholderReplacement(forStatement.OperatorsOpt.RightOperandPlaceholder, boundExpression)
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(forStatement.OperatorsOpt.GreaterThanOrEqual)
				synthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, boundExpression1.Type, SynthesizedLocalKind.ForDirection, syntax, False)
				localSymbols.Add(synthesizedLocal)
				instance.Add(New BoundAssignmentOperator(forStatement.OperatorsOpt.Syntax, New BoundLocal(forStatement.OperatorsOpt.Syntax, synthesizedLocal, synthesizedLocal.Type), boundExpression1, True, synthesizedLocal.Type, False))
				Me.RemovePlaceholderReplacement(forStatement.OperatorsOpt.LeftOperandPlaceholder)
				Me.RemovePlaceholderReplacement(forStatement.OperatorsOpt.RightOperandPlaceholder)
			ElseIf (rewrittenStep.ConstantValueOpt Is Nothing AndAlso Not rewrittenStep.Type.GetEnumUnderlyingTypeOrSelf().IsSignedIntegralType() AndAlso Not rewrittenStep.Type.GetEnumUnderlyingTypeOrSelf().IsUnsignedIntegralType()) Then
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = rewrittenStep
				Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				If (boundExpression2.Type.IsNullableType()) Then
					boundExpression3 = Me.NullableHasValue(boundExpression2)
					boundExpression2 = Me.NullableValueOrDefault(boundExpression2)
				End If
				If (Not boundExpression2.Type.GetEnumUnderlyingTypeOrSelf().IsNumericType()) Then
					Throw ExceptionUtilities.Unreachable
				End If
				Dim enumUnderlyingTypeOrSelf As TypeSymbol = boundExpression2.Type.GetEnumUnderlyingTypeOrSelf()
				Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(rewrittenStep.Syntax, ConstantValue.[Default](enumUnderlyingTypeOrSelf.SpecialType), boundExpression2.Type)
				If (enumUnderlyingTypeOrSelf.IsDecimalType()) Then
					boundLiteral = LocalRewriter.RewriteDecimalConstant(boundLiteral, boundLiteral.ConstantValueOpt, Me._topMethod, Me._diagnostics)
				End If
				Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.TransformRewrittenBinaryOperator(New BoundBinaryOperator(rewrittenStep.Syntax, BinaryOperatorKind.GreaterThanOrEqual, boundExpression2, boundLiteral, False, Me.GetSpecialType(SpecialType.System_Boolean), False))
				If (boundExpression3 IsNot Nothing) Then
					boundExpression4 = Me.MakeBooleanBinaryExpression(boundExpression4.Syntax, BinaryOperatorKind.[AndAlso], boundExpression3, boundExpression4)
				End If
				synthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, boundExpression4.Type, SynthesizedLocalKind.ForDirection, syntax, False)
				localSymbols.Add(synthesizedLocal)
				instance.Add(New BoundAssignmentOperator(boundExpression4.Syntax, New BoundLocal(boundExpression4.Syntax, synthesizedLocal, synthesizedLocal.Type), boundExpression4, True, synthesizedLocal.Type, False))
			End If
			If (instance.Count > 0) Then
				rewrittenInitialValue = New BoundSequence(rewrittenInitialValue.Syntax, ImmutableArray(Of LocalSymbol).Empty, instance.ToImmutable(), rewrittenInitialValue, rewrittenInitialValue.Type, False)
			End If
			instance.Free()
			Dim boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement(rewrittenInitialValue.Syntax, New BoundAssignmentOperator(rewrittenInitialValue.Syntax, rewrittenControlVariable, rewrittenInitialValue, True, rewrittenInitialValue.Type, False), False)
			If (Not boundStatements.IsDefaultOrEmpty) Then
				boundExpressionStatement = New BoundStatementList(boundExpressionStatement.Syntax, boundStatements.Add(boundExpressionStatement), False)
			End If
			Dim instrument As Boolean = Me(forStatement)
			If (instrument) Then
				boundExpressionStatement = Me._instrumenterOpt.InstrumentForLoopInitialization(forStatement, boundExpressionStatement)
			End If
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(forStatement.Body), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Dim boundStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.RewriteForLoopIncrement(rewrittenControlVariable, rewrittenStep, forStatement.Checked, forStatement.OperatorsOpt)
			If (flag) Then
				boundStatement1 = Me.RegisterUnstructuredExceptionHandlingResumeTarget(syntax, boundStatement1, True)
			End If
			If (instrument) Then
				boundStatement1 = Me._instrumenterOpt.InstrumentForLoopIncrement(forStatement, boundStatement1)
			End If
			Dim boundExpression5 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.RewriteForLoopCondition(rewrittenControlVariable, rewrittenLimit, rewrittenStep, forStatement.OperatorsOpt, synthesizedLocal)
			Dim labelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol = LocalRewriter.GenerateLabel("start")
			Dim boundConditionalGoto As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundConditionalGoto(syntax, boundExpression5, True, labelSymbol, False)
			Dim generatedLabelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = Nothing
			Dim boundGotoStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Nothing
			If (Not flag1) Then
				generatedLabelSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol("PostIncrement")
				Dim boundLabelStatement As Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(syntax, generatedLabelSymbol)
				boundGotoStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundGotoStatement(syntax, generatedLabelSymbol, Nothing, False)
				If (instrument) Then
					boundGotoStatement = SyntheticBoundNodeFactory.HiddenSequencePoint(boundGotoStatement)
				End If
			End If
			Dim instance1 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement).GetInstance()
			instance1.Add(boundExpressionStatement)
			If (boundGotoStatement IsNot Nothing) Then
				instance1.Add(boundGotoStatement)
			End If
			instance1.Add(New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(syntax, labelSymbol))
			instance1.Add(boundStatement)
			instance1.Add(New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(syntax, forStatement.ContinueLabel))
			instance1.Add(boundStatement1)
			If (generatedLabelSymbol IsNot Nothing) Then
				Dim boundLabelStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(syntax, generatedLabelSymbol)
				If (instrument) Then
					boundGotoStatement = SyntheticBoundNodeFactory.HiddenSequencePoint(boundLabelStatement1)
				End If
				instance1.Add(boundLabelStatement1)
			End If
			If (instrument) Then
				boundConditionalGoto = SyntheticBoundNodeFactory.HiddenSequencePoint(boundConditionalGoto)
			End If
			instance1.Add(boundConditionalGoto)
			instance1.Add(New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(syntax, forStatement.ExitLabel))
			Dim declaredOrInferredLocalOpt As LocalSymbol = forStatement.DeclaredOrInferredLocalOpt
			If (declaredOrInferredLocalOpt IsNot Nothing) Then
				localSymbols.Add(declaredOrInferredLocalOpt)
			End If
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
			Return New BoundBlock(syntax, statementSyntaxes, localSymbols.ToImmutableAndFree(), instance1.ToImmutableAndFree(), False)
		End Function

		Private Function FinishObjectForLoop(ByVal forStatement As BoundForToStatement, ByVal rewrittenControlVariable As BoundExpression, ByVal rewrittenInitialValue As BoundExpression, ByVal rewrittenLimit As BoundExpression, ByVal rewrittenStep As BoundExpression) As BoundBlock
			Dim boundBadExpression As BoundExpression
			Dim boundCall As BoundExpression
			Dim bitVector As Microsoft.CodeAnalysis.BitVector
			Dim instance As ArrayBuilder(Of LocalSymbol) = ArrayBuilder(Of LocalSymbol).GetInstance()
			Dim syntax As ForBlockSyntax = DirectCast(forStatement.Syntax, ForBlockSyntax)
			Dim flag As Boolean = Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(forStatement)
			Dim type As TypeSymbol = rewrittenControlVariable.Type
			Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, type, SynthesizedLocalKind.ForInitialValue, syntax, False)
			instance.Add(synthesizedLocal)
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, synthesizedLocal, True, synthesizedLocal.Type)
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(New BoundExpression() { rewrittenControlVariable.MakeRValue(), rewrittenInitialValue, rewrittenLimit, rewrittenStep, boundLocal, rewrittenControlVariable })
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			If (Not Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, WellKnownMember.Microsoft_VisualBasic_CompilerServices_ObjectFlowControl_ForLoopControl__ForLoopInitObj, syntax, False)) Then
				boundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(rewrittenLimit.Syntax, LookupResultKind.NotReferencable, ImmutableArray(Of Symbol).Empty, boundExpressions, Me.Compilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean), True)
			Else
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = rewrittenLimit.Syntax
				Dim specialType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.Compilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean)
				bitVector = New Microsoft.CodeAnalysis.BitVector()
				boundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntaxNode, methodSymbol, Nothing, Nothing, boundExpressions, Nothing, specialType, True, False, bitVector)
			End If
			Dim instrument As Boolean = Me(forStatement)
			If (instrument) Then
				boundBadExpression = Me._instrumenterOpt.InstrumentObjectForLoopInitCondition(forStatement, boundBadExpression, Me._currentMethodOrLambda)
			End If
			Dim boundConditionalGoto As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundConditionalGoto(syntax, boundBadExpression, False, forStatement.ExitLabel, False)
			If (flag) Then
				boundConditionalGoto = Me.RegisterUnstructuredExceptionHandlingResumeTarget(syntax, boundConditionalGoto, True)
			End If
			If (instrument) Then
				boundConditionalGoto = Me._instrumenterOpt.InstrumentForLoopInitialization(forStatement, boundConditionalGoto)
			End If
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(forStatement.Body), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			boundExpressions = ImmutableArray.Create(Of BoundExpression)(rewrittenControlVariable.MakeRValue(), boundLocal.MakeRValue(), rewrittenControlVariable)
			Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			If (Not Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol1, WellKnownMember.Microsoft_VisualBasic_CompilerServices_ObjectFlowControl_ForLoopControl__ForNextCheckObj, syntax, False)) Then
				boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(rewrittenLimit.Syntax, LookupResultKind.NotReferencable, ImmutableArray(Of Symbol).Empty, boundExpressions, Me.Compilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean), True)
			Else
				Dim syntax1 As Microsoft.CodeAnalysis.SyntaxNode = rewrittenLimit.Syntax
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.Compilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean)
				bitVector = New Microsoft.CodeAnalysis.BitVector()
				boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax1, methodSymbol1, Nothing, Nothing, boundExpressions, Nothing, namedTypeSymbol, True, False, bitVector)
			End If
			Dim labelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol = LocalRewriter.GenerateLabel("start")
			If (instrument) Then
				boundCall = Me._instrumenterOpt.InstrumentObjectForLoopCondition(forStatement, boundCall, Me._currentMethodOrLambda)
			End If
			Dim boundConditionalGoto1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundConditionalGoto(syntax, boundCall, True, labelSymbol, False)
			If (flag) Then
				boundConditionalGoto1 = Me.RegisterUnstructuredExceptionHandlingResumeTarget(syntax, boundConditionalGoto1, True)
			End If
			If (instrument) Then
				boundConditionalGoto1 = Me._instrumenterOpt.InstrumentForLoopIncrement(forStatement, boundConditionalGoto1)
			End If
			Dim boundLabelStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(syntax, forStatement.ContinueLabel)
			If (instrument) Then
				boundLabelStatement = SyntheticBoundNodeFactory.HiddenSequencePoint(boundLabelStatement)
				boundConditionalGoto1 = SyntheticBoundNodeFactory.HiddenSequencePoint(boundConditionalGoto1)
			End If
			Dim boundStatements As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { boundConditionalGoto, New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(syntax, labelSymbol), boundStatement, boundLabelStatement, boundConditionalGoto1, New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(syntax, forStatement.ExitLabel) })
			Dim declaredOrInferredLocalOpt As LocalSymbol = forStatement.DeclaredOrInferredLocalOpt
			If (declaredOrInferredLocalOpt IsNot Nothing) Then
				instance.Add(declaredOrInferredLocalOpt)
			End If
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
			Return New BoundBlock(syntax, statementSyntaxes, instance.ToImmutableAndFree(), boundStatements, False)
		End Function

		Private Function FinishRewriteNullableConversion(ByVal node As BoundConversion, ByVal resultType As TypeSymbol, ByVal operand As BoundExpression, ByVal operandHasValue As BoundExpression, ByVal temps As ArrayBuilder(Of LocalSymbol), ByVal inits As ArrayBuilder(Of BoundExpression)) As BoundExpression
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundExpression Microsoft.CodeAnalysis.VisualBasic.LocalRewriter::FinishRewriteNullableConversion(Microsoft.CodeAnalysis.VisualBasic.BoundConversion,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.PooledObjects.ArrayBuilder`1<Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol>,Microsoft.CodeAnalysis.PooledObjects.ArrayBuilder`1<Microsoft.CodeAnalysis.VisualBasic.BoundExpression>)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundExpression FinishRewriteNullableConversion(Microsoft.CodeAnalysis.VisualBasic.BoundConversion,Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol,Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.PooledObjects.ArrayBuilder<Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol>,Microsoft.CodeAnalysis.PooledObjects.ArrayBuilder<Microsoft.CodeAnalysis.VisualBasic.BoundExpression>)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Function FinishRewriteOfLiftedIntrinsicBinaryOperator(ByVal node As BoundBinaryOperator, ByVal left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal right As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal optimizeForConditionalBranch As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim flag As Boolean = LocalRewriter.HasValue(left)
			Dim flag1 As Boolean = LocalRewriter.HasValue(right)
			Dim flag2 As Boolean = LocalRewriter.HasNoValue(left)
			Dim flag3 As Boolean = LocalRewriter.HasNoValue(right)
			If (optimizeForConditionalBranch AndAlso node.Type.IsNullableOfBoolean() AndAlso left.Type.IsNullableOfBoolean() AndAlso right.Type.IsNullableOfBoolean() AndAlso (flag OrElse Not Me._inExpressionLambda OrElse (node.OperatorKind And BinaryOperatorKind.OpMask) = BinaryOperatorKind.[OrElse])) Then
				boundExpression = Me.RewriteAndOptimizeLiftedIntrinsicLogicalShortCircuitingOperator(node, left, right, flag2, flag, flag3, flag1)
			ElseIf (Me._inExpressionLambda) Then
				boundExpression = node.Update(node.OperatorKind, left, right, node.Checked, node.ConstantValueOpt, node.Type)
			ElseIf (flag2 And flag3) Then
				boundExpression = LocalRewriter.NullableNull(left, node.Type)
			ElseIf (flag And flag1) Then
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.ApplyUnliftedBinaryOp(node, Me.NullableValueOrDefault(left), Me.NullableValueOrDefault(right))
				boundExpression = Me.WrapInNullable(boundExpression2, node.Type)
			ElseIf (node.Left.Type.IsNullableOfBoolean() AndAlso CInt((node.OperatorKind And BinaryOperatorKind.OpMask)) - CInt(BinaryOperatorKind.[Or]) <= CInt(BinaryOperatorKind.[Like])) Then
				boundExpression = Me.RewriteLiftedBooleanBinaryOperator(node, left, right, flag2, flag3, flag, flag1)
			ElseIf (Not (flag2 Or flag3)) Then
				If (flag1) Then
					Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
					Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
					If (LocalRewriter.IsConditionalAccess(left, boundExpression3, boundExpression4)) Then
						Dim boundExpression5 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.NullableValueOrDefault(right)
						If (Not boundExpression5.IsConstant AndAlso boundExpression5.Kind <> BoundKind.Local AndAlso boundExpression5.Kind <> BoundKind.Parameter OrElse Not LocalRewriter.HasValue(boundExpression3) OrElse Not LocalRewriter.HasNoValue(boundExpression4)) Then
							GoTo Label1
						End If
						boundExpression = LocalRewriter.UpdateConditionalAccess(left, Me.WrapInNullable(Me.ApplyUnliftedBinaryOp(node, Me.NullableValueOrDefault(boundExpression3), boundExpression5), node.Type), LocalRewriter.NullableNull(boundExpression4, node.Type))
						Return boundExpression
					End If
				End If
			Label1:
				Dim localSymbols As ArrayBuilder(Of LocalSymbol) = Nothing
				Dim boundExpressions As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Nothing
				Dim boundExpression6 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim boundExpression7 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim boundExpression8 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.ProcessNullableOperand(left, boundExpression6, localSymbols, boundExpressions, LocalRewriter.RightCantChangeLeftLocal(left, right), flag)
				Dim boundExpression9 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.ProcessNullableOperand(right, boundExpression7, localSymbols, boundExpressions, True, flag1)
				Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim boundExpression10 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.MakeBooleanBinaryExpression(node.Syntax, BinaryOperatorKind.[And], boundExpression6, boundExpression7)
				Dim boundExpression11 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.ApplyUnliftedBinaryOp(node, boundExpression8, boundExpression9)
				boundSequence = Me.MakeTernaryConditionalExpression(node.Syntax, boundExpression10, Me.WrapInNullable(boundExpression11, node.Type), LocalRewriter.NullableNull(node.Syntax, node.Type))
				If (localSymbols IsNot Nothing) Then
					boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(node.Syntax, localSymbols.ToImmutableAndFree(), boundExpressions.ToImmutableAndFree(), boundSequence, boundSequence.Type, False)
				End If
				boundExpression = boundSequence
			Else
				boundExpression1 = If(flag2, right, left)
				Dim boundExpression12 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = LocalRewriter.NullableNull(If(flag2, left, right), node.Type)
				boundExpression = LocalRewriter.MakeSequence(boundExpression1, boundExpression12)
			End If
			Return boundExpression
		End Function

		Private Sub FlattenConcatArg(ByVal lowered As BoundExpression, ByVal flattened As ArrayBuilder(Of BoundExpression))
			Dim kind As BoundKind = lowered.Kind
			If (kind = BoundKind.BinaryConditionalExpression) Then
				Dim boundBinaryConditionalExpression As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryConditionalExpression = DirectCast(lowered, Microsoft.CodeAnalysis.VisualBasic.BoundBinaryConditionalExpression)
				If (boundBinaryConditionalExpression.ConvertedTestExpression Is Nothing) Then
					Dim elseExpression As BoundExpression = boundBinaryConditionalExpression.ElseExpression
					If (elseExpression.ConstantValueOpt IsNot Nothing AndAlso EmbeddedOperators.CompareString(elseExpression.ConstantValueOpt.StringValue, "", False) = 0) Then
						flattened.AddRange(New BoundExpression() { boundBinaryConditionalExpression.TestExpression })
						Return
					End If
				End If
			ElseIf (kind = BoundKind.[Call]) Then
				Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = DirectCast(lowered, Microsoft.CodeAnalysis.VisualBasic.BoundCall)
				Dim method As MethodSymbol = boundCall.Method
				If (method.IsShared AndAlso method.ContainingType.SpecialType = SpecialType.System_String) Then
					If (method = Me.Compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringString) OrElse method = Me.Compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringStringString) OrElse method = Me.Compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringStringStringString)) Then
						flattened.AddRange(boundCall.Arguments)
						Return
					End If
					If (method = Me.Compilation.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringArray)) Then
						Dim item As BoundArrayCreation = TryCast(boundCall.Arguments(0), BoundArrayCreation)
						If (item IsNot Nothing) Then
							Dim initializerOpt As BoundArrayInitialization = item.InitializerOpt
							If (initializerOpt IsNot Nothing) Then
								flattened.AddRange(initializerOpt.Initializers)
								Return
							End If
						End If
					End If
				End If
			End If
			flattened.Add(lowered)
		End Sub

		Public Function GenerateDisposeCallForForeachAndUsing(ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal rewrittenBoundLocal As BoundLocal, ByVal rewrittenCondition As BoundExpression, ByVal IsOrInheritsFromOrImplementsIDisposable As Boolean, ByVal rewrittenDisposeConversion As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim statement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim bitVector As Microsoft.CodeAnalysis.BitVector
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			If (Me.TryGetSpecialMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, SpecialMember.System_IDisposable__Dispose, syntaxNode)) Then
				Dim specialTypeWithUseSiteDiagnostics As NamedTypeSymbol = Me.GetSpecialTypeWithUseSiteDiagnostics(SpecialType.System_Void, syntaxNode)
				Dim type As TypeSymbol = rewrittenBoundLocal.Type
				If (Not IsOrInheritsFromOrImplementsIDisposable OrElse Not type.IsValueType AndAlso Not type.IsTypeParameter()) Then
					Dim empty As ImmutableArray(Of !0) = ImmutableArray(Of BoundExpression).Empty
					bitVector = New Microsoft.CodeAnalysis.BitVector()
					boundStatement = (New BoundCall(syntaxNode, methodSymbol, Nothing, rewrittenDisposeConversion, empty, Nothing, specialTypeWithUseSiteDiagnostics, False, False, bitVector)).ToStatement()
					boundStatement.SetWasCompilerGenerated()
				Else
					Dim u00210s As ImmutableArray(Of !0) = ImmutableArray(Of BoundExpression).Empty
					bitVector = New Microsoft.CodeAnalysis.BitVector()
					boundStatement = (New BoundCall(syntaxNode, methodSymbol, Nothing, rewrittenBoundLocal, u00210s, Nothing, specialTypeWithUseSiteDiagnostics, False, False, bitVector)).ToStatement()
					boundStatement.SetWasCompilerGenerated()
					If (Not type.IsValueType) Then
						GoTo Label1
					End If
					statement = boundStatement
					Return statement
				End If
			Label1:
				Dim boundStatements As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)()
				statement = Me.RewriteIfStatement(syntaxNode, rewrittenCondition, boundStatement, Nothing, Nothing, boundStatements)
			Else
				statement = (New BoundBadExpression(syntaxNode, LookupResultKind.NotReferencable, ImmutableArray(Of Symbol).Empty, If(rewrittenCondition IsNot Nothing, ImmutableArray.Create(Of BoundExpression)(rewrittenBoundLocal, rewrittenCondition), ImmutableArray.Create(Of BoundExpression)(rewrittenBoundLocal)), ErrorTypeSymbol.UnknownResultType, True)).ToStatement()
			End If
			Return statement
		End Function

		Private Shared Function GenerateLabel(ByVal baseName As String) As LabelSymbol
			Return New GeneratedLabelSymbol(baseName)
		End Function

		Private Function GenerateMonitorEnter(ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal boundLockObject As BoundExpression, <Out> ByRef boundLockTakenLocal As BoundLocal, <Out> ByRef boundLockTakenInitialization As BoundStatement) As BoundStatement
			Dim statement As BoundStatement
			Dim boundExpressions As ImmutableArray(Of BoundExpression)
			Dim synthesizedLocal As LocalSymbol
			Dim parameters As ImmutableArray(Of ParameterSymbol)
			boundLockTakenLocal = Nothing
			boundLockTakenInitialization = Nothing
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			If (Not Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, WellKnownMember.System_Threading_Monitor__Enter2, syntaxNode, True)) Then
				Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, WellKnownMember.System_Threading_Monitor__Enter, syntaxNode, False)
				boundExpressions = ImmutableArray.Create(Of BoundExpression)(boundLockObject)
			Else
				If (syntaxNode.Parent.Kind() <> SyntaxKind.SyncLockStatement) Then
					Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me._currentMethodOrLambda
					parameters = methodSymbol.Parameters
					synthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(methodSymbol1, parameters(1).Type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
				Else
					Dim methodSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me._currentMethodOrLambda
					parameters = methodSymbol.Parameters
					synthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(methodSymbol2, parameters(1).Type, SynthesizedLocalKind.LockTaken, DirectCast(syntaxNode.Parent, SyncLockStatementSyntax), False)
				End If
				boundLockTakenLocal = New BoundLocal(syntaxNode, synthesizedLocal, synthesizedLocal.Type)
				boundLockTakenInitialization = (New BoundAssignmentOperator(syntaxNode, boundLockTakenLocal, New BoundLiteral(syntaxNode, ConstantValue.[False], boundLockTakenLocal.Type), True, boundLockTakenLocal.Type, False)).ToStatement()
				boundLockTakenInitialization.SetWasCompilerGenerated()
				boundExpressions = ImmutableArray.Create(Of BoundExpression)(boundLockObject, boundLockTakenLocal)
				boundLockTakenLocal = boundLockTakenLocal.MakeRValue()
			End If
			If (methodSymbol Is Nothing) Then
				statement = (New BoundBadExpression(syntaxNode, LookupResultKind.NotReferencable, ImmutableArray(Of Symbol).Empty, boundExpressions, ErrorTypeSymbol.UnknownResultType, True)).ToStatement()
			Else
				Dim returnType As TypeSymbol = methodSymbol.ReturnType
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = (New BoundCall(syntaxNode, methodSymbol, Nothing, Nothing, boundExpressions, Nothing, returnType, True, False, bitVector)).ToStatement()
				boundExpressionStatement.SetWasCompilerGenerated()
				statement = boundExpressionStatement
			End If
			Return statement
		End Function

		Private Function GenerateMonitorExit(ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal boundLockObject As BoundExpression, ByVal boundLockTakenLocal As BoundLocal) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundBadExpression As BoundExpression
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			If (Not Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, WellKnownMember.System_Threading_Monitor__Exit, syntaxNode, False)) Then
				boundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(syntaxNode, LookupResultKind.NotReferencable, ImmutableArray(Of Symbol).Empty, ImmutableArray.Create(Of BoundExpression)(boundLockObject), ErrorTypeSymbol.UnknownResultType, True)
			Else
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(boundLockObject)
				Dim returnType As TypeSymbol = methodSymbol.ReturnType
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				boundBadExpression = New BoundCall(syntaxNode, methodSymbol, Nothing, Nothing, boundExpressions, Nothing, returnType, True, False, bitVector)
			End If
			Dim statement As BoundExpressionStatement = boundBadExpression.ToStatement()
			statement.SetWasCompilerGenerated()
			If (boundLockTakenLocal Is Nothing) Then
				boundStatement = statement
			Else
				Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator(syntaxNode, BinaryOperatorKind.Equals, boundLockTakenLocal, New BoundLiteral(syntaxNode, ConstantValue.[True], boundLockTakenLocal.Type), False, boundLockTakenLocal.Type, False)
				Dim boundStatements As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)()
				boundStatement = Me.RewriteIfStatement(syntaxNode, boundBinaryOperator, statement, Nothing, Nothing, boundStatements)
			End If
			Return boundStatement
		End Function

		Private Function GenerateObjectCloneIfNeeded(ByVal generatedExpression As BoundExpression) As BoundExpression
			Return Me.GenerateObjectCloneIfNeeded(generatedExpression, generatedExpression)
		End Function

		Private Function GenerateObjectCloneIfNeeded(ByVal expression As BoundExpression, ByVal rewrittenExpression As BoundExpression) As BoundExpression
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundExpression Microsoft.CodeAnalysis.VisualBasic.LocalRewriter::GenerateObjectCloneIfNeeded(Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundExpression GenerateObjectCloneIfNeeded(Microsoft.CodeAnalysis.VisualBasic.BoundExpression,Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Friend Shared Function GenerateSequenceValueSideEffects(ByVal container As Symbol, ByVal value As BoundExpression, ByVal temporaries As ImmutableArray(Of LocalSymbol), ByVal sideEffects As ImmutableArray(Of BoundExpression)) As BoundExpression
			Dim localIfNotConst As BoundExpression
			Dim syntax As SyntaxNode = value.Syntax
			Dim type As TypeSymbol = value.Type
			Dim instance As ArrayBuilder(Of LocalSymbol) = ArrayBuilder(Of LocalSymbol).GetInstance()
			If (Not temporaries.IsEmpty) Then
				instance.AddRange(temporaries)
			End If
			Dim boundExpressions As ArrayBuilder(Of BoundExpression) = ArrayBuilder(Of BoundExpression).GetInstance()
			If (type.SpecialType <> SpecialType.System_Void) Then
				localIfNotConst = LocalRewriter.CacheToLocalIfNotConst(container, value, instance, boundExpressions, SynthesizedLocalKind.LoweringTemp, Nothing)
			Else
				boundExpressions.Add(value)
				localIfNotConst = Nothing
			End If
			If (Not sideEffects.IsDefaultOrEmpty) Then
				boundExpressions.AddRange(sideEffects)
			End If
			Return New BoundSequence(syntax, instance.ToImmutableAndFree(), boundExpressions.ToImmutableAndFree(), localIfNotConst, type, False)
		End Function

		Private Shared Function GetBoundObjectInitializerFromInitializer(ByVal initializer As BoundExpression) As BoundObjectInitializerExpression
			Dim initializerOpt As BoundObjectInitializerExpression
			If (initializer Is Nothing OrElse initializer.Kind <> BoundKind.ObjectCreationExpression AndAlso initializer.Kind <> BoundKind.NewT) Then
				initializerOpt = Nothing
			Else
				initializerOpt = TryCast(DirectCast(initializer, BoundObjectCreationExpressionBase).InitializerOpt, BoundObjectInitializerExpression)
			End If
			Return initializerOpt
		End Function

		Private Function GetEventAccessReceiver(ByVal unwrappedEventAccess As BoundEventAccess) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (unwrappedEventAccess.ReceiverOpt IsNot Nothing) Then
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(unwrappedEventAccess.ReceiverOpt)
				If (unwrappedEventAccess.EventSymbol.IsShared) Then
					boundExpression1 = Nothing
				Else
					boundExpression1 = boundExpression2
				End If
				boundExpression = boundExpression1
			Else
				boundExpression = Nothing
			End If
			Return boundExpression
		End Function

		Private Shared Function GetLeftOperand(ByVal binary As BoundBinaryOperator, ByRef optimizeForConditionalBranch As Boolean) As BoundExpression
			If (optimizeForConditionalBranch AndAlso (binary.OperatorKind And BinaryOperatorKind.OpMask) <> BinaryOperatorKind.[OrElse]) Then
				optimizeForConditionalBranch = False
			End If
			Return binary.Left.GetMostEnclosedParenthesizedExpression()
		End Function

		Private Function GetNewCompoundUseSiteInfo() As CompoundUseSiteInfo(Of AssemblySymbol)
			Return New CompoundUseSiteInfo(Of AssemblySymbol)(Me._diagnostics, Me.Compilation.Assembly)
		End Function

		Private Function GetNullableMethod(ByVal syntax As SyntaxNode, ByVal nullableType As TypeSymbol, ByVal member As SpecialMember) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim memberForDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			If (Not Me.TryGetSpecialMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, member, syntax)) Then
				memberForDefinition = Nothing
			Else
				memberForDefinition = DirectCast(DirectCast(nullableType, SubstitutedNamedType).GetMemberForDefinition(methodSymbol), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			End If
			Return memberForDefinition
		End Function

		Private Shared Function GetRightOperand(ByVal binary As BoundBinaryOperator, ByVal adjustIfOptimizableForConditionalBranch As Boolean) As BoundExpression
			Dim right As BoundExpression
			If (Not adjustIfOptimizableForConditionalBranch) Then
				right = binary.Right
			Else
				Dim flag As Boolean = False
				right = LocalRewriter.AdjustIfOptimizableForConditionalBranch(binary.Right, flag)
			End If
			Return right
		End Function

		Private Shared Function GetSideeffects(ByVal operand As BoundExpression) As BoundExpression
			Dim sideeffects As BoundExpression
			If (Not operand.IsConstant) Then
				Dim kind As BoundKind = operand.Kind
				If (kind = BoundKind.ObjectCreationExpression) Then
					If (Not operand.Type.IsNullableType()) Then
						GoTo Label1
					End If
					Dim arguments As ImmutableArray(Of BoundExpression) = DirectCast(operand, BoundObjectCreationExpression).Arguments
					If (arguments.Length <> 0) Then
						sideeffects = LocalRewriter.GetSideeffects(arguments(0))
						Return sideeffects
					Else
						sideeffects = Nothing
						Return sideeffects
					End If
				Else
					If (kind <> BoundKind.Local AndAlso kind <> BoundKind.Parameter) Then
						GoTo Label1
					End If
					sideeffects = Nothing
					Return sideeffects
				End If
			Label1:
				sideeffects = operand
			Else
				sideeffects = Nothing
			End If
			Return sideeffects
		End Function

		Private Function GetSpecialType(ByVal specialType As Microsoft.CodeAnalysis.SpecialType) As NamedTypeSymbol
			Return Me._topMethod.ContainingAssembly.GetSpecialType(specialType)
		End Function

		Private Function GetSpecialTypeMember(ByVal specialMember As Microsoft.CodeAnalysis.SpecialMember) As Symbol
			Return Me._topMethod.ContainingAssembly.GetSpecialTypeMember(specialMember)
		End Function

		Private Function GetSpecialTypeWithUseSiteDiagnostics(ByVal specialType As Microsoft.CodeAnalysis.SpecialType, ByVal syntax As SyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me._topMethod.ContainingAssembly.GetSpecialType(specialType)
			Dim useSiteInfoForSpecialType As UseSiteInfo(Of AssemblySymbol) = Binder.GetUseSiteInfoForSpecialType(namedTypeSymbol, False)
			Binder.ReportUseSite(Me._diagnostics, syntax, useSiteInfoForSpecialType)
			Return namedTypeSymbol
		End Function

		Private Function GetWindowsRuntimeEventReceiver(ByVal syntax As SyntaxNode, ByVal rewrittenReceiver As BoundExpression) As BoundExpression
			Dim boundBadExpression As BoundExpression
			Dim type As NamedTypeSymbol = DirectCast(rewrittenReceiver.Type, NamedTypeSymbol)
			Dim wellKnownTypeMember As MethodSymbol = DirectCast(Me.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__GetOrCreateEventRegistrationTokenTable), MethodSymbol)
			wellKnownTypeMember = wellKnownTypeMember.AsMember(type)
			Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = Nothing
			If (Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)(propertySymbol, WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__InvocationList, syntax, False)) Then
				Dim getMethod As MethodSymbol = propertySymbol.GetMethod
				If (getMethod IsNot Nothing) Then
					getMethod = getMethod.AsMember(type)
					Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = (New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, wellKnownTypeMember, Nothing, Nothing, ImmutableArray.Create(Of BoundExpression)(rewrittenReceiver), Nothing, False, False, wellKnownTypeMember.ReturnType, False)).MakeCompilerGenerated()
					boundBadExpression = (New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, getMethod, Nothing, boundCall, ImmutableArray(Of BoundExpression).Empty, Nothing, False, False, getMethod.ReturnType, False)).MakeCompilerGenerated()
					Return boundBadExpression
				End If
				Dim descriptor As MemberDescriptor = WellKnownMembers.GetDescriptor(WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__InvocationList)
				Dim accessorName As String = Binder.GetAccessorName(propertySymbol.Name, MethodKind.PropertyGet, False)
				Dim diagnosticForMissingRuntimeHelper As DiagnosticInfo = MissingRuntimeMemberDiagnosticHelper.GetDiagnosticForMissingRuntimeHelper(descriptor.DeclaringTypeMetadataName, accessorName, Me._compilationState.Compilation.Options.EmbedVbCoreRuntime)
				Me._diagnostics.Add(diagnosticForMissingRuntimeHelper, syntax.GetLocation())
			End If
			boundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(syntax, LookupResultKind.NotReferencable, ImmutableArray(Of Symbol).Empty, ImmutableArray.Create(Of BoundExpression)(rewrittenReceiver), ErrorTypeSymbol.UnknownResultType, True)
			Return boundBadExpression
		End Function

		Private Shared Function HasNoValue(ByVal expr As BoundExpression) As Boolean
			Dim flag As Boolean
			flag = If(expr.Kind <> BoundKind.ObjectCreationExpression, False, DirectCast(expr, BoundObjectCreationExpression).Arguments.Length = 0)
			Return flag
		End Function

		Private Shared Function HasSideEffects(ByVal statement As BoundStatement) As Boolean
			Dim flag As Boolean
			If (statement IsNot Nothing) Then
				Dim kind As BoundKind = statement.Kind
				Select Case kind
					Case BoundKind.SequencePoint
						flag = LocalRewriter.HasSideEffects(DirectCast(statement, BoundSequencePoint).StatementOpt)
						Exit Select
					Case BoundKind.SequencePointExpression
					Label0:
						flag = True
						Exit Select
					Case BoundKind.SequencePointWithSpan
						flag = LocalRewriter.HasSideEffects(DirectCast(statement, BoundSequencePointWithSpan).StatementOpt)
						Exit Select
					Case BoundKind.NoOpStatement
						flag = False
						Exit Select
					Case Else
						If (kind = BoundKind.Block) Then
							Dim enumerator As ImmutableArray(Of BoundStatement).Enumerator = DirectCast(statement, BoundBlock).Statements.GetEnumerator()
							While enumerator.MoveNext()
								If (Not LocalRewriter.HasSideEffects(enumerator.Current)) Then
									Continue While
								End If
								flag = True
								Return flag
							End While
							flag = False
							Exit Select
						Else
							GoTo Label0
						End If
				End Select
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Shared Function HasValue(ByVal expr As BoundExpression) As Boolean
			Dim length As Boolean
			Dim kind As BoundKind = expr.Kind
			If (kind = BoundKind.Conversion) Then
				If (Not LocalRewriter.IsConversionFromUnderlyingToNullable(DirectCast(expr, BoundConversion))) Then
					length = False
					Return length
				End If
				length = True
				Return length
			Else
				If (kind <> BoundKind.ObjectCreationExpression) Then
					length = False
					Return length
				End If
				length = DirectCast(expr, BoundObjectCreationExpression).Arguments.Length <> 0
				Return length
			End If
			length = False
			Return length
		End Function

		Private Shared Function InsertXmlLiteralsPreamble(ByVal node As BoundNode, ByVal fixups As ImmutableArray(Of LocalRewriter.XmlLiteralFixupData.LocalWithInitialization)) As BoundBlock
			Dim length As Integer = fixups.Length
			Dim local(length - 1 + 1 - 1) As LocalSymbol
			Dim boundExpressionStatement(length + 1 - 1) As BoundStatement
			Dim num As Integer = length - 1
			Dim num1 As Integer = 0
			Do
				Dim item As LocalRewriter.XmlLiteralFixupData.LocalWithInitialization = fixups(num1)
				local(num1) = item.Local
				Dim initialization As BoundExpression = item.Initialization
				boundExpressionStatement(num1) = New Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement(initialization.Syntax, initialization, False)
				num1 = num1 + 1
			Loop While num1 <= num
			boundExpressionStatement(length) = DirectCast(node, BoundStatement)
			Dim syntax As SyntaxNode = node.Syntax
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
			Return New BoundBlock(syntax, statementSyntaxes, local.AsImmutable(Of LocalSymbol)(), Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundStatement)(boundExpressionStatement), False)
		End Function

		Private Function InsideValidUnstructuredExceptionHandlingOnErrorContext() As Boolean
			If (CObj(Me._currentMethodOrLambda) <> CObj(Me._topMethod) OrElse Me._unstructuredExceptionHandling.Context Is Nothing) Then
				Return False
			End If
			Return Me._unstructuredExceptionHandling.Context.ContainsOnError
		End Function

		Private Function InsideValidUnstructuredExceptionHandlingResumeContext() As Boolean
			If (Me._unstructuredExceptionHandling.Context Is Nothing OrElse Me._unstructuredExceptionHandling.CurrentStatementTemporary Is Nothing) Then
				Return False
			End If
			Return CObj(Me._currentMethodOrLambda) = CObj(Me._topMethod)
		End Function

		Private Function InvokeInterpolatedStringFactory(ByVal node As BoundInterpolatedStringExpression, ByVal factoryType As TypeSymbol, ByVal factoryMethodName As String, ByVal targetType As TypeSymbol, ByVal factory As SyntheticBoundNodeFactory) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim name As [Object]()
			Dim boundExpressionArray As Microsoft.CodeAnalysis.VisualBasic.BoundExpression()
			Dim flag As Boolean = False
			If (Not factoryType.IsErrorType()) Then
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = node.Binder
				Dim instance As LookupResult = LookupResult.GetInstance()
				Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me.GetNewCompoundUseSiteInfo()
				binder.LookupMember(instance, factoryType, factoryMethodName, 0, LookupOptions.MustNotBeInstance Or LookupOptions.AllMethodsOfAnyArity Or LookupOptions.MethodsOnly, newCompoundUseSiteInfo)
				Me._diagnostics.Add(node, newCompoundUseSiteInfo)
				If (instance.Kind <> LookupResultKind.Inaccessible) Then
					If (instance.IsGood) Then
						GoTo Label2
					End If
					instance.Free()
					name = New [Object]() { factoryType.Name, factoryMethodName }
					LocalRewriter.ReportDiagnostic(node, ErrorFactory.ErrorInfo(ERRID.ERR_InterpolatedStringFactoryError, name), Me._diagnostics)
					boundExpressionArray = New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { DirectCast(MyBase.VisitInterpolatedStringExpression(node), Microsoft.CodeAnalysis.VisualBasic.BoundExpression) }
					boundExpression = factory.Convert(targetType, factory.BadExpression(boundExpressionArray), False)
					Return boundExpression
				Else
					flag = True
				End If
			Label2:
				Dim boundMethodGroup As Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup = (New Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup(node.Syntax, Nothing, instance.Symbols.ToDowncastedImmutable(Of MethodSymbol)(), instance.Kind, Nothing, QualificationKind.QualifiedViaTypeName, False)).MakeCompilerGenerated()
				instance.Free()
				Dim pooledStringBuilder As Microsoft.CodeAnalysis.PooledObjects.PooledStringBuilder = Microsoft.CodeAnalysis.PooledObjects.PooledStringBuilder.GetInstance()
				Dim boundExpressions As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
				Dim num As Integer = -1
				boundExpressions.Add(Nothing)
				Dim enumerator As ImmutableArray(Of BoundNode).Enumerator = node.Contents.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As BoundNode = enumerator.Current
					Dim kind As BoundKind = current.Kind
					If (kind = BoundKind.Literal) Then
						pooledStringBuilder.Builder.Append(DirectCast(current, BoundLiteral).Value.StringValue)
					Else
						If (kind <> BoundKind.Interpolation) Then
							Throw ExceptionUtilities.Unreachable
						End If
						num = num + 1
						Dim boundInterpolation As Microsoft.CodeAnalysis.VisualBasic.BoundInterpolation = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.BoundInterpolation)
						Dim builder As StringBuilder = pooledStringBuilder.Builder
						builder.Append("{"C)
						builder.Append(num.ToString(CultureInfo.InvariantCulture))
						If (boundInterpolation.AlignmentOpt IsNot Nothing) Then
							builder.Append(","C)
							Dim int64Value As Long = boundInterpolation.AlignmentOpt.ConstantValueOpt.Int64Value
							builder.Append(int64Value.ToString(CultureInfo.InvariantCulture))
						End If
						If (boundInterpolation.FormatStringOpt IsNot Nothing) Then
							builder.Append(":")
							builder.Append(boundInterpolation.FormatStringOpt.Value.StringValue)
						End If
						builder.Append("}"C)
						builder = Nothing
						boundExpressions.Add(boundInterpolation.Expression)
					End If
				End While
				boundExpressions(0) = factory.StringLiteral(ConstantValue.Create(pooledStringBuilder.ToStringAndFree())).MakeCompilerGenerated()
				Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim immutableAndFree As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = boundExpressions.ToImmutableAndFree()
				Dim strs As ImmutableArray(Of String) = New ImmutableArray(Of String)()
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = binder.MakeRValue(binder.BindInvocationExpression(syntax, syntaxNode, TypeCharacter.None, boundMethodGroup, immutableAndFree, strs, Me._diagnostics, Nothing, False, False, False, Nothing, True), Me._diagnostics).MakeCompilerGenerated()
				If (Not boundExpression1.Type.Equals(targetType)) Then
					boundExpression1 = binder.ApplyImplicitConversion(node.Syntax, targetType, boundExpression1, Me._diagnostics, False).MakeCompilerGenerated()
				End If
				If (flag OrElse boundExpression1.HasErrors) Then
					name = New [Object]() { factoryType.Name, factoryMethodName }
					LocalRewriter.ReportDiagnostic(node, ErrorFactory.ErrorInfo(ERRID.ERR_InterpolatedStringFactoryError, name), Me._diagnostics)
					boundExpressionArray = New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { DirectCast(MyBase.VisitInterpolatedStringExpression(node), Microsoft.CodeAnalysis.VisualBasic.BoundExpression) }
					boundExpression = factory.Convert(targetType, factory.BadExpression(boundExpressionArray), False)
					Return boundExpression
				End If
				boundExpression1 = Me.VisitExpression(boundExpression1)
				boundExpression = boundExpression1
				Return boundExpression
			End If
			name = New [Object]() { factoryType.Name, factoryMethodName }
			LocalRewriter.ReportDiagnostic(node, ErrorFactory.ErrorInfo(ERRID.ERR_InterpolatedStringFactoryError, name), Me._diagnostics)
			boundExpressionArray = New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { DirectCast(MyBase.VisitInterpolatedStringExpression(node), Microsoft.CodeAnalysis.VisualBasic.BoundExpression) }
			boundExpression = factory.Convert(targetType, factory.BadExpression(boundExpressionArray), False)
			Return boundExpression
		End Function

		Private Shared Function IsCompoundVariableName(ByVal name As String) As Boolean
			If (name.Equals("$VB$It", StringComparison.Ordinal) OrElse name.Equals("$VB$It1", StringComparison.Ordinal)) Then
				Return True
			End If
			Return name.Equals("$VB$It2", StringComparison.Ordinal)
		End Function

		Private Shared Function IsConditionalAccess(ByVal operand As BoundExpression, <Out> ByRef whenNotNull As BoundExpression, <Out> ByRef whenNull As BoundExpression) As Boolean
			Dim flag As Boolean
			If (operand.Kind = BoundKind.Sequence) Then
				Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSequence = DirectCast(operand, Microsoft.CodeAnalysis.VisualBasic.BoundSequence)
				If (boundSequence.ValueOpt Is Nothing) Then
					whenNotNull = Nothing
					whenNull = Nothing
					flag = False
					Return flag
				End If
				operand = boundSequence.ValueOpt
			End If
			If (operand.Kind <> BoundKind.LoweredConditionalAccess) Then
				whenNotNull = Nothing
				whenNull = Nothing
				flag = False
			Else
				Dim boundLoweredConditionalAccess As Microsoft.CodeAnalysis.VisualBasic.BoundLoweredConditionalAccess = DirectCast(operand, Microsoft.CodeAnalysis.VisualBasic.BoundLoweredConditionalAccess)
				whenNotNull = boundLoweredConditionalAccess.WhenNotNull
				whenNull = boundLoweredConditionalAccess.WhenNullOpt
				flag = True
			End If
			Return flag
		End Function

		Private Shared Function IsConversionFromUnderlyingToNullable(ByVal conversion As BoundConversion) As Boolean
			If ((conversion.ConversionKind And (ConversionKind.[Widening] Or ConversionKind.Nullable Or ConversionKind.WideningNullable Or ConversionKind.UserDefined)) <> ConversionKind.WideningNullable) Then
				Return False
			End If
			Return conversion.Type.GetNullableUnderlyingType().Equals(conversion.Operand.Type, TypeCompareKind.AllIgnoreOptionsForVB)
		End Function

		Private Shared Function IsFloatingPointExpressionOfUnknownPrecision(ByVal rewrittenNode As BoundExpression) As Boolean
			Dim flag As Boolean
			If (rewrittenNode IsNot Nothing) Then
				Dim specialType As Microsoft.CodeAnalysis.SpecialType = rewrittenNode.Type.SpecialType
				If (specialType = Microsoft.CodeAnalysis.SpecialType.System_Double OrElse specialType = Microsoft.CodeAnalysis.SpecialType.System_Single) Then
					Dim kind As BoundKind = rewrittenNode.Kind
					If (kind = BoundKind.Conversion) Then
						Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundConversion = DirectCast(rewrittenNode, Microsoft.CodeAnalysis.VisualBasic.BoundConversion)
						flag = If(boundConversion.ConversionKind <> ConversionKind.Identity, False, Not boundConversion.ExplicitCastInCode)
					Else
						flag = If(kind <> BoundKind.Sequence, True, LocalRewriter.IsFloatingPointExpressionOfUnknownPrecision(DirectCast(rewrittenNode, BoundSequence).ValueOpt))
					End If
				Else
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function IsFloatingTruncation(ByVal node As BoundCall) As Boolean
			Dim method As Boolean
			Dim name As String = node.Method.Name
			If (Not "Fix".Equals(name)) Then
				method = If(Not "Truncate".Equals(name), False, node.Method = Me.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Math__TruncateDouble))
			Else
				Dim specialType As Microsoft.CodeAnalysis.SpecialType = node.Type.SpecialType
				If (specialType = Microsoft.CodeAnalysis.SpecialType.System_Single) Then
					method = node.Method = Me.Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Conversion__FixSingle)
				Else
					method = If(specialType = Microsoft.CodeAnalysis.SpecialType.System_Double, node.Method = Me.Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Conversion__FixDouble), False)
				End If
			End If
			Return method
		End Function

		Private Shared Function IsNullOrEmptyStringConstant(ByVal operand As BoundExpression) As Boolean
			If (operand.ConstantValueOpt IsNot Nothing AndAlso [String].IsNullOrEmpty(operand.ConstantValueOpt.StringValue)) Then
				Return True
			End If
			Return operand.IsDefaultValueConstant()
		End Function

		Private Function IsOmittedBoundCall(ByVal expression As BoundExpression) As Boolean
			Dim flag As Boolean
			If ((Me._flags And LocalRewriter.RewritingFlags.AllowOmissionOfConditionalCalls) = LocalRewriter.RewritingFlags.AllowOmissionOfConditionalCalls) Then
				Dim kind As BoundKind = expression.Kind
				If (kind = BoundKind.[Call]) Then
					flag = DirectCast(expression, BoundCall).Method.CallsAreOmitted(expression.Syntax, expression.SyntaxTree)
					Return flag
				Else
					If (kind <> BoundKind.ConditionalAccess) Then
						flag = False
						Return flag
					End If
					flag = Me.IsOmittedBoundCall(DirectCast(expression, BoundConditionalAccess).AccessExpression)
					Return flag
				End If
			End If
			flag = False
			Return flag
		End Function

		Private Shared Function IsPropertyAssignment(ByVal node As BoundAssignmentOperator) As Boolean
			Dim returnsByRef As Boolean
			Dim kind As BoundKind = node.Left.Kind
			If (kind = BoundKind.PropertyAccess) Then
				returnsByRef = Not DirectCast(node.Left, BoundPropertyAccess).PropertySymbol.ReturnsByRef
			Else
				returnsByRef = If(kind = BoundKind.XmlMemberAccess, True, False)
			End If
			Return returnsByRef
		End Function

		Private Shared Function LateAssignToArrayElement(ByVal node As SyntaxNode, ByVal arrayRef As BoundExpression, ByVal index As Integer, ByVal value As BoundExpression, ByVal intType As TypeSymbol) As BoundExpression
			Dim boundLiteral As BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(node, ConstantValue.Create(index), intType)
			Dim boundArrayAccess As BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess(node, arrayRef, ImmutableArray.Create(Of BoundExpression)(boundLiteral), value.Type, False)
			Return New BoundAssignmentOperator(node, boundArrayAccess, value, True, False)
		End Function

		Private Function LateCallOrGet(ByVal memberAccess As BoundLateMemberAccess, ByVal receiverExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal argExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression), ByVal assignmentArguments As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression), ByVal argNames As ImmutableArray(Of String), ByVal useLateCall As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim syntax As SyntaxNode = memberAccess.Syntax
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			If (Not useLateCall) Then
				If (Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateGet, syntax, False)) Then
					GoTo Label1
				End If
				boundExpression = memberAccess
				Return boundExpression
			Else
				If (Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateCall, syntax, False)) Then
					GoTo Label1
				End If
				boundExpression = memberAccess
				Return boundExpression
			End If
		Label1:
			Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = Nothing
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Nothing
			Dim synthesizedLocal1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = Nothing
			Dim boundLocal1 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Nothing
			Dim parameters As ImmutableArray(Of ParameterSymbol) = methodSymbol.Parameters
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.LateMakeArgumentArrayArgument(syntax, argExpressions, argNames, parameters(3).Type)
			Dim flags As ImmutableArray(Of Boolean) = New ImmutableArray(Of Boolean)()
			parameters = methodSymbol.Parameters
			Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.LateMakeCopyBackArray(syntax, flags, parameters(6).Type)
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Nothing
			If (Not assignmentArguments.IsDefaultOrEmpty) Then
				Dim num As Integer = 0
				If (Not argNames.IsDefaultOrEmpty) Then
					Dim enumerator As ImmutableArray(Of String).Enumerator = argNames.GetEnumerator()
					While enumerator.MoveNext()
						If (enumerator.Current Is Nothing) Then
							Continue While
						End If
						num = num + 1
					End While
				End If
				Dim length As Integer = assignmentArguments.Length - num
				Dim flagArray As Boolean() = Nothing
				Dim length1 As Integer = assignmentArguments.Length - 1
				Dim num1 As Integer = 0
				Do
					Dim item As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = assignmentArguments(num1)
					If (item.IsSupportingAssignment()) Then
						If (synthesizedLocal1 Is Nothing) Then
							synthesizedLocal1 = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, boundExpression3.Type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
							boundLocal1 = (New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, synthesizedLocal1, synthesizedLocal1.Type)).MakeRValue()
							synthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, boundExpression2.Type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
							boundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, synthesizedLocal, synthesizedLocal.Type)
							boundExpression2 = (New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(syntax, boundLocal, boundExpression2, True, False)).MakeRValue()
							boundLocal = boundLocal.MakeRValue()
							instance = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance(assignmentArguments.Length)
							ReDim flagArray(assignmentArguments.Length - 1 + 1 - 1)
						End If
						Dim num2 As Integer = If(num1 < length, num + num1, num1 - length)
						flagArray(num2) = True
						instance.Add(Me.LateMakeConditionalCopyback(item, boundLocal, boundLocal1, num2))
					End If
					num1 = num1 + 1
				Loop While num1 <= length1
				If (synthesizedLocal1 IsNot Nothing) Then
					boundExpression3 = (New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(syntax, New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, synthesizedLocal1, synthesizedLocal1.Type), Me.LateMakeCopyBackArray(syntax, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Boolean)(flagArray), synthesizedLocal1.Type), True, False)).MakeRValue()
				End If
			End If
			If (receiverExpression Is Nothing) Then
				boundExpression1 = Nothing
			Else
				boundExpression1 = receiverExpression.MakeRValue()
			End If
			Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundExpression1
			parameters = methodSymbol.Parameters
			Dim boundExpression5 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.LateMakeReceiverArgument(syntax, boundExpression4, parameters(0).Type)
			Dim containerTypeOpt As TypeSymbol = memberAccess.ContainerTypeOpt
			parameters = methodSymbol.Parameters
			Dim boundExpression6 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.LateMakeContainerArgument(syntax, receiverExpression, containerTypeOpt, parameters(1).Type)
			Dim nameOpt As String = memberAccess.NameOpt
			parameters = methodSymbol.Parameters
			Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = LocalRewriter.MakeStringLiteral(syntax, nameOpt, parameters(2).Type)
			Dim boundExpression7 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundExpression2
			parameters = methodSymbol.Parameters
			Dim boundExpression8 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.LateMakeArgumentNameArrayArgument(syntax, argNames, parameters(4).Type)
			Dim typeArgumentsOpt As BoundTypeArguments = memberAccess.TypeArgumentsOpt
			parameters = methodSymbol.Parameters
			Dim boundExpression9 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.LateMakeTypeArgumentArrayArgument(syntax, typeArgumentsOpt, parameters(5).Type)
			Dim boundExpression10 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundExpression3
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression5, boundExpression6, boundLiteral, boundExpression7, boundExpression8, boundExpression9, boundExpression10 })
			If (useLateCall) Then
				parameters = methodSymbol.Parameters
				Dim boundExpression11 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = LocalRewriter.MakeBooleanLiteral(syntax, True, parameters(7).Type)
				boundExpressions = boundExpressions.Add(boundExpression11)
			End If
			Dim returnType As TypeSymbol = methodSymbol.ReturnType
			Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
			Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, methodSymbol, Nothing, Nothing, boundExpressions, Nothing, returnType, True, False, bitVector)
			If (synthesizedLocal1 IsNot Nothing) Then
				Dim synthesizedLocal2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, boundCall.Type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
				Dim boundLocal2 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, synthesizedLocal2, synthesizedLocal2.Type)
				Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(syntax, boundLocal2, boundCall, True, False)
				boundCall = New BoundSequence(syntax, ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal, synthesizedLocal1, synthesizedLocal2), Microsoft.CodeAnalysis.ImmutableArrayExtensions.Concat(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundAssignmentOperator), instance.ToImmutableAndFree()), boundLocal2.MakeRValue(), boundLocal2.Type, False)
			End If
			boundExpression = boundCall
			Return boundExpression
		End Function

		Private Sub LateCaptureArgsComplex(ByRef temps As ArrayBuilder(Of SynthesizedLocal), ByRef arguments As ImmutableArray(Of BoundExpression), <Out> ByRef writeTargets As ImmutableArray(Of BoundExpression))
			Dim second As BoundExpression
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me._currentMethodOrLambda
			If (temps Is Nothing) Then
				temps = ArrayBuilder(Of SynthesizedLocal).GetInstance()
			End If
			If (Not arguments.IsDefaultOrEmpty) Then
				Dim instance As ArrayBuilder(Of BoundExpression) = ArrayBuilder(Of BoundExpression).GetInstance()
				Dim boundExpressions As ArrayBuilder(Of BoundExpression) = ArrayBuilder(Of BoundExpression).GetInstance()
				Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = arguments.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As BoundExpression = enumerator.Current
					If (current.IsSupportingAssignment()) Then
						Dim boundLateBoundArgumentSupportingAssignmentWithCapture As Microsoft.CodeAnalysis.VisualBasic.BoundLateBoundArgumentSupportingAssignmentWithCapture = Nothing
						If (current.Kind = BoundKind.LateBoundArgumentSupportingAssignmentWithCapture) Then
							boundLateBoundArgumentSupportingAssignmentWithCapture = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.BoundLateBoundArgumentSupportingAssignmentWithCapture)
							current = boundLateBoundArgumentSupportingAssignmentWithCapture.OriginalArgument
						End If
						Dim result As UseTwiceRewriter.Result = UseTwiceRewriter.UseTwice(methodSymbol, current, temps)
						If (current.IsPropertyOrXmlPropertyAccess()) Then
							current = result.First.SetAccessKind(PropertyAccessKind.[Get])
							second = result.Second.SetAccessKind(PropertyAccessKind.[Set])
						ElseIf (Not current.IsLateBound()) Then
							current = result.First.MakeRValue()
							second = result.Second
						Else
							current = result.First.SetLateBoundAccessKind(LateBoundAccessKind.[Get])
							second = result.Second.SetLateBoundAccessKind(LateBoundAccessKind.[Set])
						End If
						If (boundLateBoundArgumentSupportingAssignmentWithCapture IsNot Nothing) Then
							current = New BoundAssignmentOperator(boundLateBoundArgumentSupportingAssignmentWithCapture.Syntax, New BoundLocal(boundLateBoundArgumentSupportingAssignmentWithCapture.Syntax, boundLateBoundArgumentSupportingAssignmentWithCapture.LocalSymbol, boundLateBoundArgumentSupportingAssignmentWithCapture.LocalSymbol.Type), current, True, boundLateBoundArgumentSupportingAssignmentWithCapture.Type, False)
						End If
					Else
						second = Nothing
					End If
					instance.Add(Me.VisitExpressionNode(current))
					boundExpressions.Add(second)
				End While
				arguments = instance.ToImmutableAndFree()
				writeTargets = boundExpressions.ToImmutableAndFree()
			End If
		End Sub

		Private Function LateIndexGet(ByVal node As BoundLateInvocation, ByVal receiverExpr As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal argExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			If (Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateIndexGet, syntax, False)) Then
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = receiverExpr.MakeRValue()
				Dim parameters As ImmutableArray(Of ParameterSymbol) = methodSymbol.Parameters
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.LateMakeReceiverArgument(syntax, boundExpression, parameters(0).Type)
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim argumentNamesOpt As ImmutableArray(Of String) = node.ArgumentNamesOpt
				parameters = methodSymbol.Parameters
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.LateMakeArgumentArrayArgument(syntaxNode, argExpressions, argumentNamesOpt, parameters(1).Type)
				Dim strs As ImmutableArray(Of String) = node.ArgumentNamesOpt
				parameters = methodSymbol.Parameters
				Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.LateMakeArgumentNameArrayArgument(syntax, strs, parameters(2).Type)
				Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpression1, boundExpression2, boundExpression3)
				Dim returnType As TypeSymbol = methodSymbol.ReturnType
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, methodSymbol, Nothing, Nothing, boundExpressions, Nothing, returnType, True, False, bitVector)
			Else
				boundCall = node
			End If
			Return boundCall
		End Function

		Private Function LateIndexSet(ByVal syntax As SyntaxNode, ByVal invocation As BoundLateInvocation, ByVal assignmentValue As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal isCopyBack As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim flag As Boolean = If(invocation.Member Is Nothing, False, Not invocation.Member.IsLValue)
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			Dim flag1 As Boolean = If(isCopyBack, True, flag)
			If (Not flag1) Then
				If (Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateIndexSet, syntax, False)) Then
					GoTo Label1
				End If
				boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(syntax, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(invocation), Nothing, Me.GetSpecialType(SpecialType.System_Void), False)
				Return boundSequence
			Else
				If (Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateIndexSetComplex, syntax, False)) Then
					GoTo Label1
				End If
				boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(syntax, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(invocation), Nothing, Me.GetSpecialType(SpecialType.System_Void), False)
				Return boundSequence
			End If
		Label1:
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = invocation.Member.MakeRValue()
			Dim parameters As ImmutableArray(Of ParameterSymbol) = methodSymbol.Parameters
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.LateMakeReceiverArgument(syntax, boundExpression, parameters(0).Type)
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = assignmentValue.MakeRValue()
			Dim argumentsOpt As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = invocation.ArgumentsOpt
			Dim argumentNamesOpt As ImmutableArray(Of String) = invocation.ArgumentNamesOpt
			parameters = methodSymbol.Parameters
			Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.LateMakeSetArgumentArrayArgument(syntax, boundExpression2, argumentsOpt, argumentNamesOpt, parameters(1).Type)
			Dim strs As ImmutableArray(Of String) = invocation.ArgumentNamesOpt
			parameters = methodSymbol.Parameters
			Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.LateMakeArgumentNameArrayArgument(syntax, strs, parameters(2).Type)
			If (flag1) Then
				parameters = methodSymbol.Parameters
				Dim boundExpression5 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = LocalRewriter.MakeBooleanLiteral(syntax, isCopyBack, parameters(3).Type)
				parameters = methodSymbol.Parameters
				Dim boundExpression6 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = LocalRewriter.MakeBooleanLiteral(syntax, flag, parameters(4).Type)
				boundExpressions = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression1, boundExpression3, boundExpression4, boundExpression5, boundExpression6 })
			Else
				boundExpressions = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpression1, boundExpression3, boundExpression4)
			End If
			Dim returnType As TypeSymbol = methodSymbol.ReturnType
			Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
			boundSequence = New BoundCall(syntax, methodSymbol, Nothing, Nothing, boundExpressions, Nothing, returnType, True, False, bitVector)
			Return boundSequence
		End Function

		Private Function LateMakeArgumentArrayArgument(ByVal node As SyntaxNode, ByVal rewrittenArguments As ImmutableArray(Of BoundExpression), ByVal argumentNames As ImmutableArray(Of String), ByVal objectArrayType As TypeSymbol) As BoundExpression
			Dim boundSequence As BoundExpression
			If (Not argumentNames.IsDefaultOrEmpty) Then
				Dim num As Integer = 0
				Dim enumerator As ImmutableArray(Of String).Enumerator = argumentNames.GetEnumerator()
				While enumerator.MoveNext()
					If (enumerator.Current Is Nothing) Then
						Continue While
					End If
					num = num + 1
				End While
				Dim length As Integer = rewrittenArguments.Length - num
				Dim elementType As TypeSymbol = DirectCast(objectArrayType, ArrayTypeSymbol).ElementType
				Dim specialType As NamedTypeSymbol = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32)
				Dim boundLiteral As BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(node, ConstantValue.Create(rewrittenArguments.Length), specialType)
				Dim boundArrayCreation As Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation(node, ImmutableArray.Create(Of BoundExpression)(boundLiteral), Nothing, objectArrayType, False)
				Dim synthesizedLocal As LocalSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, boundArrayCreation.Type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(node, synthesizedLocal, synthesizedLocal.Type)
				Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(node, boundLocal, boundArrayCreation, True, False)
				Dim instance As ArrayBuilder(Of BoundExpression) = ArrayBuilder(Of BoundExpression).GetInstance()
				instance.Add(boundAssignmentOperator)
				boundLocal = boundLocal.MakeRValue()
				Dim length1 As Integer = rewrittenArguments.Length - 1
				Dim num1 As Integer = 0
				Do
					Dim item As BoundExpression = rewrittenArguments(num1)
					item = item.MakeRValue()
					If (Not item.Type.IsObjectType()) Then
						Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me.GetNewCompoundUseSiteInfo()
						Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(item.Type, elementType, newCompoundUseSiteInfo)
						Me._diagnostics.Add(node, newCompoundUseSiteInfo)
						item = New BoundDirectCast(node, item, conversionKind, elementType, False)
					End If
					Dim num2 As Integer = If(num1 < length, num + num1, num1 - length)
					Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(node, ConstantValue.Create(num2), specialType))
					Dim boundArrayAccess As BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess(node, boundLocal, boundExpressions, elementType, False)
					instance.Add(New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(node, boundArrayAccess, item, True, False))
					num1 = num1 + 1
				Loop While num1 <= length1
				boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(node, ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal), instance.ToImmutableAndFree(), boundLocal, boundLocal.Type, False)
			Else
				boundSequence = Me.LateMakeArgumentArrayArgumentNoNamed(node, rewrittenArguments, objectArrayType)
			End If
			Return boundSequence
		End Function

		Private Function LateMakeArgumentArrayArgumentNoNamed(ByVal node As SyntaxNode, ByVal rewrittenArguments As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression), ByVal objectArrayType As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundArrayCreation As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim elementType As TypeSymbol = DirectCast(objectArrayType, ArrayTypeSymbol).ElementType
			Dim specialType As NamedTypeSymbol = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32)
			If (Not rewrittenArguments.IsDefaultOrEmpty) Then
				Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(node, ConstantValue.Create(rewrittenArguments.Length), specialType)
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
				Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Enumerator = rewrittenArguments.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = enumerator.Current
					current = current.MakeRValue()
					If (Not current.Type.IsObjectType()) Then
						Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me.GetNewCompoundUseSiteInfo()
						Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(current.Type, elementType, newCompoundUseSiteInfo)
						Me._diagnostics.Add(current, newCompoundUseSiteInfo)
						current = New BoundDirectCast(node, current, conversionKind, elementType, False)
					End If
					instance.Add(current)
				End While
				Dim boundArrayInitialization As Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization(node, instance.ToImmutableAndFree(), Nothing, False)
				boundArrayCreation = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation(node, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundLiteral), boundArrayInitialization, objectArrayType, False)
			Else
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(node, ConstantValue.[Default](ConstantValueTypeDiscriminator.Int32), specialType)
				boundArrayCreation = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation(node, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpression), Nothing, objectArrayType, False)
			End If
			Return boundArrayCreation
		End Function

		Private Function LateMakeArgumentNameArrayArgument(ByVal node As SyntaxNode, ByVal argumentNames As ImmutableArray(Of String), ByVal stringArrayType As TypeSymbol) As BoundExpression
			Dim boundArrayCreation As BoundExpression
			Dim elementType As TypeSymbol = DirectCast(stringArrayType, ArrayTypeSymbol).ElementType
			If (Not argumentNames.IsDefaultOrEmpty) Then
				Dim instance As ArrayBuilder(Of BoundExpression) = ArrayBuilder(Of BoundExpression).GetInstance()
				Dim enumerator As ImmutableArray(Of String).Enumerator = argumentNames.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As String = enumerator.Current
					If (current Is Nothing) Then
						Continue While
					End If
					instance.Add(LocalRewriter.MakeStringLiteral(node, current, elementType))
				End While
				Dim boundArrayInitialization As Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization(node, instance.ToImmutableAndFree(), Nothing, False)
				Dim specialType As NamedTypeSymbol = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32)
				Dim initializers As ImmutableArray(Of BoundExpression) = boundArrayInitialization.Initializers
				Dim boundLiteral As BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(node, ConstantValue.Create(initializers.Length), specialType)
				boundArrayCreation = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation(node, ImmutableArray.Create(Of BoundExpression)(boundLiteral), boundArrayInitialization, stringArrayType, False)
			Else
				boundArrayCreation = LocalRewriter.MakeNullLiteral(node, stringArrayType)
			End If
			Return boundArrayCreation
		End Function

		Private Function LateMakeConditionalCopyback(ByVal assignmentTarget As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal valueArrayRef As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal copyBackArrayRef As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal argNum As Integer) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim syntax As SyntaxNode = assignmentTarget.Syntax
			Dim specialType As NamedTypeSymbol = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32)
			Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntax, ConstantValue.Create(argNum), specialType)
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundLiteral)
			Dim elementType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = DirectCast(copyBackArrayRef.Type, ArrayTypeSymbol).ElementType
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = (New BoundArrayAccess(syntax, copyBackArrayRef, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundLiteral), elementType, False)).MakeRValue()
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = DirectCast(valueArrayRef.Type, ArrayTypeSymbol).ElementType
			Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = (New BoundArrayAccess(syntax, valueArrayRef, boundExpressions, typeSymbol, False)).MakeRValue()
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = assignmentTarget.Type
			If (Not type.IsSameTypeIgnoringAll(typeSymbol)) Then
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
				If (Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ChangeType, syntax, False)) Then
					Dim boundTypeExpression As Microsoft.CodeAnalysis.VisualBasic.BoundTypeExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundTypeExpression(syntax, type, False)
					Dim parameters As ImmutableArray(Of ParameterSymbol) = methodSymbol.Parameters
					Dim boundGetType As Microsoft.CodeAnalysis.VisualBasic.BoundGetType = New Microsoft.CodeAnalysis.VisualBasic.BoundGetType(syntax, boundTypeExpression, parameters(1).Type, False)
					Dim boundExpressions1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundCall, boundGetType)
					Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
					boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, methodSymbol, Nothing, Nothing, boundExpressions1, Nothing, typeSymbol, False, False, bitVector)
				End If
				Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
				Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(typeSymbol, type, discarded)
				boundCall = New BoundDirectCast(syntax, boundCall, conversionKind, type, False)
			End If
			Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(syntax, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty, Nothing, Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Void), False)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.LateMakeCopyback(syntax, assignmentTarget, boundCall)
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.MakeTernaryConditionalExpression(syntax, boundExpression, boundExpression1, boundSequence)
			Return Me.VisitExpressionNode(boundExpression2)
		End Function

		Private Function LateMakeContainerArgument(ByVal node As SyntaxNode, ByVal receiver As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal containerType As TypeSymbol, ByVal typeType As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (receiver Is Nothing) Then
				boundExpression = LocalRewriter.MakeGetTypeExpression(node, containerType, typeType)
			Else
				boundExpression = LocalRewriter.MakeNullLiteral(node, typeType)
			End If
			Return boundExpression
		End Function

		Private Function LateMakeCopyback(ByVal syntax As SyntaxNode, ByVal assignmentTarget As BoundExpression, ByVal convertedValue As BoundExpression) As BoundExpression
			Dim boundSequence As BoundExpression
			If (assignmentTarget.Kind = BoundKind.LateMemberAccess) Then
				Dim boundLateMemberAccess As Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess = DirectCast(assignmentTarget, Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess)
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = New ImmutableArray(Of BoundExpression)()
				Dim strs As ImmutableArray(Of String) = New ImmutableArray(Of String)()
				boundSequence = Me.LateSet(syntax, boundLateMemberAccess, convertedValue, boundExpressions, strs, True)
			ElseIf (assignmentTarget.Kind <> BoundKind.LateInvocation) Then
				Dim boundAssignmentOperator As BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(syntax, assignmentTarget, Me.GenerateObjectCloneIfNeeded(convertedValue), True, False)
				boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(syntax, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundExpression)(boundAssignmentOperator), Nothing, Me.GetSpecialType(SpecialType.System_Void), False)
			Else
				Dim boundLateInvocation As Microsoft.CodeAnalysis.VisualBasic.BoundLateInvocation = DirectCast(assignmentTarget, Microsoft.CodeAnalysis.VisualBasic.BoundLateInvocation)
				If (boundLateInvocation.Member.Kind <> BoundKind.LateMemberAccess) Then
					boundSequence = Me.LateIndexSet(syntax, boundLateInvocation, convertedValue, True)
				Else
					Dim member As Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess = DirectCast(boundLateInvocation.Member, Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess)
					boundSequence = Me.LateSet(syntax, member, convertedValue, boundLateInvocation.ArgumentsOpt, boundLateInvocation.ArgumentNamesOpt, True)
				End If
			End If
			Return boundSequence
		End Function

		Private Function LateMakeCopyBackArray(ByVal node As SyntaxNode, ByVal flags As ImmutableArray(Of Boolean), ByVal booleanArrayType As TypeSymbol) As BoundExpression
			Dim boundArrayCreation As BoundExpression
			Dim elementType As TypeSymbol = DirectCast(booleanArrayType, ArrayTypeSymbol).ElementType
			If (Not flags.IsDefaultOrEmpty) Then
				Dim specialType As NamedTypeSymbol = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32)
				Dim boundLiteral As BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(node, ConstantValue.Create(flags.Length), specialType)
				Dim instance As ArrayBuilder(Of BoundExpression) = ArrayBuilder(Of BoundExpression).GetInstance()
				Dim enumerator As ImmutableArray(Of Boolean).Enumerator = flags.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Boolean = enumerator.Current
					instance.Add(LocalRewriter.MakeBooleanLiteral(node, current, elementType))
				End While
				Dim boundArrayInitialization As Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization(node, instance.ToImmutableAndFree(), Nothing, False)
				boundArrayCreation = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation(node, ImmutableArray.Create(Of BoundExpression)(boundLiteral), boundArrayInitialization, booleanArrayType, False)
			Else
				boundArrayCreation = LocalRewriter.MakeNullLiteral(node, booleanArrayType)
			End If
			Return boundArrayCreation
		End Function

		Private Function LateMakeReceiverArgument(ByVal node As SyntaxNode, ByVal rewrittenReceiver As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal objectType As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (rewrittenReceiver IsNot Nothing) Then
				If (Not rewrittenReceiver.Type.IsObjectType()) Then
					Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me.GetNewCompoundUseSiteInfo()
					Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(rewrittenReceiver.Type, objectType, newCompoundUseSiteInfo)
					Me._diagnostics.Add(node, newCompoundUseSiteInfo)
					rewrittenReceiver = New BoundDirectCast(node, rewrittenReceiver, conversionKind, objectType, False)
				End If
				boundExpression = rewrittenReceiver
			Else
				boundExpression = LocalRewriter.MakeNullLiteral(node, objectType)
			End If
			Return boundExpression
		End Function

		Private Function LateMakeSetArgumentArrayArgument(ByVal node As SyntaxNode, ByVal rewrittenValue As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal rewrittenArguments As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression), ByVal argumentNames As ImmutableArray(Of String), ByVal objectArrayType As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim elementType As TypeSymbol = DirectCast(objectArrayType, ArrayTypeSymbol).ElementType
			Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me.GetNewCompoundUseSiteInfo()
			If (Not rewrittenValue.Type.IsObjectType()) Then
				Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(rewrittenValue.Type, elementType, newCompoundUseSiteInfo)
				Me._diagnostics.Add(node, newCompoundUseSiteInfo)
				rewrittenValue = New BoundDirectCast(node, rewrittenValue, conversionKind, elementType, False)
			End If
			If (Not argumentNames.IsDefaultOrEmpty) Then
				Dim num As Integer = 0
				Dim enumerator As ImmutableArray(Of String).Enumerator = argumentNames.GetEnumerator()
				While enumerator.MoveNext()
					If (enumerator.Current Is Nothing) Then
						Continue While
					End If
					num = num + 1
				End While
				Dim length As Integer = rewrittenArguments.Length - num
				Dim specialType As NamedTypeSymbol = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32)
				Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(node, ConstantValue.Create(rewrittenArguments.Length + 1), specialType)
				Dim boundArrayCreation As Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation(node, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundLiteral), Nothing, objectArrayType, False)
				Dim synthesizedLocal As LocalSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, boundArrayCreation.Type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(node, synthesizedLocal, synthesizedLocal.Type)
				Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(node, boundLocal, boundArrayCreation, True, False)
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
				instance.Add(boundAssignmentOperator)
				boundLocal = boundLocal.MakeRValue()
				Dim length1 As Integer = rewrittenArguments.Length - 1
				Dim num1 As Integer = 0
				Do
					Dim item As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = rewrittenArguments(num1)
					item = item.MakeRValue()
					If (Not item.Type.IsObjectType()) Then
						Dim conversionKind1 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(item.Type, elementType, newCompoundUseSiteInfo)
						Me._diagnostics.Add(item, newCompoundUseSiteInfo)
						item = New BoundDirectCast(node, item, conversionKind1, elementType, False)
					End If
					Dim arrayElement As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = LocalRewriter.LateAssignToArrayElement(node, boundLocal, If(num1 < length, num + num1, num1 - length), item, specialType)
					instance.Add(arrayElement)
					num1 = num1 + 1
				Loop While num1 <= length1
				If (Not rewrittenValue.Type.IsObjectType()) Then
					Dim conversionKind2 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(rewrittenValue.Type, elementType, newCompoundUseSiteInfo)
					Me._diagnostics.Add(rewrittenValue, newCompoundUseSiteInfo)
					rewrittenValue = New BoundDirectCast(node, rewrittenValue, conversionKind2, elementType, False)
				End If
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = LocalRewriter.LateAssignToArrayElement(node, boundLocal, rewrittenArguments.Length, rewrittenValue, specialType)
				instance.Add(boundExpression)
				boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(node, ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal), instance.ToImmutableAndFree(), boundLocal, boundLocal.Type, False)
			Else
				If (rewrittenArguments.IsDefaultOrEmpty) Then
					rewrittenArguments = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(rewrittenValue)
				ElseIf (argumentNames.IsDefaultOrEmpty) Then
					rewrittenArguments = rewrittenArguments.Add(rewrittenValue)
				End If
				boundSequence = Me.LateMakeArgumentArrayArgumentNoNamed(node, rewrittenArguments, objectArrayType)
			End If
			Return boundSequence
		End Function

		Private Function LateMakeTypeArgumentArrayArgument(ByVal node As SyntaxNode, ByVal arguments As BoundTypeArguments, ByVal typeArrayType As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (arguments IsNot Nothing) Then
				boundExpression = Me.MakeArrayOfGetTypeExpressions(node, arguments.Arguments, typeArrayType)
			Else
				boundExpression = LocalRewriter.MakeNullLiteral(node, typeArrayType)
			End If
			Return boundExpression
		End Function

		Private Function LateSet(ByVal syntax As Microsoft.CodeAnalysis.SyntaxNode, ByVal memberAccess As BoundLateMemberAccess, ByVal assignmentValue As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal argExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression), ByVal argNames As ImmutableArray(Of String), ByVal isCopyBack As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim flag As Boolean = If(memberAccess.ReceiverOpt Is Nothing, False, Not memberAccess.ReceiverOpt.IsLValue)
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			Dim flag1 As Boolean = If(isCopyBack, True, flag)
			If (Not flag1) Then
				If (Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateSet, syntax, False)) Then
					GoTo Label1
				End If
				boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(syntax, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(memberAccess), Nothing, Me.GetSpecialType(SpecialType.System_Void), False)
				Return boundSequence
			Else
				If (Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, WellKnownMember.Microsoft_VisualBasic_CompilerServices_NewLateBinding__LateSetComplex, syntax, False)) Then
					GoTo Label1
				End If
				boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(syntax, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(memberAccess), Nothing, Me.GetSpecialType(SpecialType.System_Void), False)
				Return boundSequence
			End If
		Label1:
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = syntax
			If (memberAccess.ReceiverOpt IsNot Nothing) Then
				boundExpression = memberAccess.ReceiverOpt.MakeRValue()
			Else
				boundExpression = Nothing
			End If
			Dim parameters As ImmutableArray(Of ParameterSymbol) = methodSymbol.Parameters
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.LateMakeReceiverArgument(syntaxNode, boundExpression, parameters(0).Type)
			Dim receiverOpt As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = memberAccess.ReceiverOpt
			Dim containerTypeOpt As TypeSymbol = memberAccess.ContainerTypeOpt
			parameters = methodSymbol.Parameters
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.LateMakeContainerArgument(syntax, receiverOpt, containerTypeOpt, parameters(1).Type)
			Dim nameOpt As String = memberAccess.NameOpt
			parameters = methodSymbol.Parameters
			Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = LocalRewriter.MakeStringLiteral(syntax, nameOpt, parameters(2).Type)
			parameters = methodSymbol.Parameters
			Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.LateMakeSetArgumentArrayArgument(syntax, assignmentValue, argExpressions, argNames, parameters(3).Type)
			parameters = methodSymbol.Parameters
			Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.LateMakeArgumentNameArrayArgument(syntax, argNames, parameters(4).Type)
			Dim typeArgumentsOpt As BoundTypeArguments = memberAccess.TypeArgumentsOpt
			parameters = methodSymbol.Parameters
			Dim boundExpression5 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.LateMakeTypeArgumentArrayArgument(syntax, typeArgumentsOpt, parameters(5).Type)
			If (flag1) Then
				parameters = methodSymbol.Parameters
				Dim boundExpression6 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = LocalRewriter.MakeBooleanLiteral(syntax, isCopyBack, parameters(6).Type)
				parameters = methodSymbol.Parameters
				Dim boundExpression7 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = LocalRewriter.MakeBooleanLiteral(syntax, flag, parameters(7).Type)
				boundExpressions = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression1, boundExpression2, boundLiteral, boundExpression3, boundExpression4, boundExpression5, boundExpression6, boundExpression7 })
			Else
				boundExpressions = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression1, boundExpression2, boundLiteral, boundExpression3, boundExpression4, boundExpression5 })
			End If
			Dim returnType As TypeSymbol = methodSymbol.ReturnType
			Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
			boundSequence = New BoundCall(syntax, methodSymbol, Nothing, Nothing, boundExpressions, Nothing, returnType, True, False, bitVector)
			Return boundSequence
		End Function

		Private Function LeaveUnstructuredExceptionHandlingContext(ByVal node As BoundNode) As LocalRewriter.UnstructuredExceptionHandlingContext
			Dim context As LocalRewriter.UnstructuredExceptionHandlingContext = New LocalRewriter.UnstructuredExceptionHandlingContext()
			context.Context = Me._unstructuredExceptionHandling.Context
			Me._unstructuredExceptionHandling.Context = Nothing
			Return context
		End Function

		Private Function LocalOrFieldNeedsToBeCleanedUp(ByVal currentType As TypeSymbol) As Boolean
			Dim flag As Boolean
			Dim flag1 As Boolean
			If (currentType.IsReferenceType OrElse currentType.IsTypeParameter()) Then
				flag = True
			ElseIf (currentType.IsIntrinsicOrEnumType()) Then
				flag = False
			ElseIf (Not Me._valueTypesCleanUpCache.TryGetValue(currentType, flag1)) Then
				Me._valueTypesCleanUpCache(currentType) = False
				Dim enumerator As ImmutableArray(Of Symbol).Enumerator = currentType.GetMembers().GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Symbol = enumerator.Current
					If (current.Kind <> SymbolKind.Field) Then
						Continue While
					End If
					Dim type As TypeSymbol = DirectCast(current, FieldSymbol).Type
					If (CObj(type) = CObj(currentType) OrElse Not Me.LocalOrFieldNeedsToBeCleanedUp(type)) Then
						Continue While
					End If
					Me._valueTypesCleanUpCache(currentType) = True
					flag = True
					Return flag
				End While
				flag = False
			Else
				flag = flag1
			End If
			Return flag
		End Function

		Private Function MakeArrayOfGetTypeExpressions(ByVal node As SyntaxNode, ByVal types As ImmutableArray(Of TypeSymbol), ByVal typeArrayType As TypeSymbol) As BoundArrayCreation
			Dim specialType As NamedTypeSymbol = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Int32)
			Dim boundLiteral As BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(node, ConstantValue.Create(types.Length), specialType)
			Dim elementType As TypeSymbol = DirectCast(typeArrayType, ArrayTypeSymbol).ElementType
			Dim instance As ArrayBuilder(Of BoundExpression) = ArrayBuilder(Of BoundExpression).GetInstance()
			Dim enumerator As ImmutableArray(Of TypeSymbol).Enumerator = types.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As TypeSymbol = enumerator.Current
				instance.Add(LocalRewriter.MakeGetTypeExpression(node, current, elementType))
			End While
			Dim boundArrayInitialization As Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization(node, instance.ToImmutableAndFree(), Nothing, False)
			Return New BoundArrayCreation(node, ImmutableArray.Create(Of BoundExpression)(boundLiteral), boundArrayInitialization, typeArrayType, False)
		End Function

		Private Shared Function MakeBadFieldAccess(ByVal syntax As SyntaxNode, ByVal tupleField As FieldSymbol, ByVal rewrittenReceiver As BoundExpression) As BoundBadExpression
			Return New BoundBadExpression(syntax, LookupResultKind.Empty, ImmutableArray.Create(Of Symbol)(tupleField), ImmutableArray.Create(Of BoundExpression)(rewrittenReceiver), tupleField.Type, True)
		End Function

		Private Function MakeBinaryExpression(ByVal syntax As SyntaxNode, ByVal binaryOpKind As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind, ByVal left As BoundExpression, ByVal right As BoundExpression, ByVal isChecked As Boolean, ByVal resultType As TypeSymbol) As BoundExpression
			Dim boundLiteral As BoundExpression
			Dim flag As Boolean = False
			Dim flag1 As Boolean = False
			Dim flag2 As Boolean = False
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = OverloadResolution.TryFoldConstantBinaryOperator(binaryOpKind, left, right, resultType, flag, flag1, flag2)
			If (constantValue Is Nothing OrElse flag1 OrElse flag And isChecked OrElse flag2) Then
				Dim binaryOperatorKind As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind = binaryOpKind
				If (binaryOperatorKind > Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Subtract) Then
					If (binaryOperatorKind <> Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Multiply) Then
						If (CInt(binaryOperatorKind) - CInt(Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[Or]) <= CInt(Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Add)) Then
							GoTo Label0
						End If
						If (CInt(binaryOperatorKind) - CInt(Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[And]) <= CInt(Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Add)) Then
							GoTo Label3
						End If
						GoTo Label2
					End If
					If (left.IsDefaultValueConstant()) Then
						boundLiteral = LocalRewriter.MakeSequence(right, left)
						Return boundLiteral
					ElseIf (right.IsDefaultValueConstant()) Then
						boundLiteral = LocalRewriter.MakeSequence(left, right)
						Return boundLiteral
					ElseIf (Not left.IsTrueConstant()) Then
						If (Not right.IsTrueConstant()) Then
							GoTo Label2
						End If
						boundLiteral = left
						Return boundLiteral
					Else
						boundLiteral = right
						Return boundLiteral
					End If
				Else
					Select Case binaryOperatorKind
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Add
							GoTo Label0
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Concatenate
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[Like]
							Exit Select
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Equals
							If (Not left.IsTrueConstant()) Then
								If (Not right.IsTrueConstant()) Then
									Exit Select
								End If
								boundLiteral = left
								Return boundLiteral
							Else
								boundLiteral = right
								Return boundLiteral
							End If
						Case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.NotEquals
							If (Not left.IsFalseConstant()) Then
								If (Not right.IsFalseConstant()) Then
									Exit Select
								End If
								boundLiteral = left
								Return boundLiteral
							Else
								boundLiteral = right
								Return boundLiteral
							End If
						Case Else
							If (binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Subtract) Then
								If (Not right.IsDefaultValueConstant()) Then
									Exit Select
								End If
								boundLiteral = left
								Return boundLiteral
							Else
								Exit Select
							End If
					End Select
				End If
			Label2:
				boundLiteral = Me.TransformRewrittenBinaryOperator(New BoundBinaryOperator(syntax, binaryOpKind, left, right, isChecked, resultType, False))
			Else
				boundLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntax, constantValue, resultType)
			End If
			Return boundLiteral
			If (left.IsDefaultValueConstant()) Then
				boundLiteral = right
				Return boundLiteral
			ElseIf (right.IsDefaultValueConstant()) Then
				boundLiteral = left
				Return boundLiteral
			ElseIf (Not left.IsTrueConstant()) Then
				If (Not right.IsTrueConstant()) Then
					GoTo Label2
				End If
				boundLiteral = LocalRewriter.MakeSequence(left, right)
				Return boundLiteral
			Else
				boundLiteral = LocalRewriter.MakeSequence(right, left)
				Return boundLiteral
			End If
		End Function

		Private Function MakeBooleanBinaryExpression(ByVal syntax As SyntaxNode, ByVal binaryOpKind As BinaryOperatorKind, ByVal left As BoundExpression, ByVal right As BoundExpression) As BoundExpression
			Return Me.MakeBinaryExpression(syntax, binaryOpKind, left, right, False, left.Type)
		End Function

		Private Shared Function MakeBooleanLiteral(ByVal node As SyntaxNode, ByVal value As Boolean, ByVal booleanType As TypeSymbol) As BoundLiteral
			Return New BoundLiteral(node, ConstantValue.Create(value), booleanType)
		End Function

		Private Function MakeEventAccessorCall(ByVal node As BoundAddRemoveHandlerStatement, ByVal unwrappedEventAccess As BoundEventAccess, ByVal accessorSymbol As MethodSymbol) As BoundStatement
			Dim current As VisualBasicAttributeData
			Dim bitVector As Microsoft.CodeAnalysis.BitVector
			Dim syntax As SyntaxNode
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim returnType As TypeSymbol
			Dim eventAccessReceiver As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.GetEventAccessReceiver(unwrappedEventAccess)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.Handler)
			Dim eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol = unwrappedEventAccess.EventSymbol
			Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
			If (eventAccessReceiver IsNot Nothing AndAlso eventSymbol.ContainingAssembly.IsLinked AndAlso eventSymbol.ContainingType.IsInterfaceType()) Then
				Dim containingType As NamedTypeSymbol = eventSymbol.ContainingType
				Dim enumerator As ImmutableArray(Of VisualBasicAttributeData).Enumerator = containingType.GetAttributes().GetEnumerator()
				Do
					If (Not enumerator.MoveNext()) Then
						If (boundCall Is Nothing) Then
							syntax = node.Syntax
							boundExpressions = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpression)
							returnType = accessorSymbol.ReturnType
							bitVector = New Microsoft.CodeAnalysis.BitVector()
							boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, accessorSymbol, Nothing, eventAccessReceiver, boundExpressions, Nothing, returnType, False, False, bitVector)
						End If
						Return New BoundExpressionStatement(node.Syntax, boundCall, False)
					End If
					current = enumerator.Current
				Loop While Not current.IsTargetAttribute(containingType, AttributeDescription.ComEventInterfaceAttribute) OrElse current.CommonConstructorArguments.Length <> 2
				boundCall = Me.RewriteNoPiaAddRemoveHandler(node, eventAccessReceiver, eventSymbol, boundExpression)
			End If
			If (boundCall Is Nothing) Then
				syntax = node.Syntax
				boundExpressions = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpression)
				returnType = accessorSymbol.ReturnType
				bitVector = New Microsoft.CodeAnalysis.BitVector()
				boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, accessorSymbol, Nothing, eventAccessReceiver, boundExpressions, Nothing, returnType, False, False, bitVector)
			End If
			Return New BoundExpressionStatement(node.Syntax, boundCall, False)
		End Function

		Private Shared Function MakeGetTypeExpression(ByVal node As SyntaxNode, ByVal type As TypeSymbol, ByVal typeType As TypeSymbol) As BoundGetType
			Dim boundTypeExpression As Microsoft.CodeAnalysis.VisualBasic.BoundTypeExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundTypeExpression(node, type, False)
			Return New BoundGetType(node, boundTypeExpression, typeType, False)
		End Function

		Private Shared Function MakeNullLiteral(ByVal syntax As SyntaxNode, ByVal type As TypeSymbol) As BoundLiteral
			Return New BoundLiteral(syntax, ConstantValue.[Nothing], type)
		End Function

		Private Function MakeResultFromNonNullLeft(ByVal rewrittenLeft As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal convertedTestExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal testExpressionPlaceholder As BoundRValuePlaceholder) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (convertedTestExpression IsNot Nothing) Then
				boundExpression = If(Not rewrittenLeft.Type.IsSameTypeIgnoringAll(convertedTestExpression.Type), Me.VisitExpressionNode(convertedTestExpression, testExpressionPlaceholder, Me.NullableValueOrDefault(rewrittenLeft)), rewrittenLeft)
			Else
				boundExpression = Me.NullableValueOrDefault(rewrittenLeft)
			End If
			Return boundExpression
		End Function

		Private Shared Function MakeSequence(ByVal first As BoundExpression, ByVal second As BoundExpression) As BoundExpression
			Return LocalRewriter.MakeSequence(second.Syntax, first, second)
		End Function

		Private Shared Function MakeSequence(ByVal syntax As SyntaxNode, ByVal first As BoundExpression, ByVal second As BoundExpression) As BoundExpression
			Dim boundSequence As BoundExpression
			Dim sideeffects As BoundExpression = LocalRewriter.GetSideeffects(first)
			If (sideeffects IsNot Nothing) Then
				boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(syntax, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundExpression)(sideeffects), second, second.Type, False)
			Else
				boundSequence = second
			End If
			Return boundSequence
		End Function

		Private Shared Function MakeStringLiteral(ByVal node As SyntaxNode, ByVal value As String, ByVal stringType As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral
			Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral
			boundLiteral = If(value IsNot Nothing, New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(node, ConstantValue.Create(value), stringType), LocalRewriter.MakeNullLiteral(node, stringType))
			Return boundLiteral
		End Function

		Private Function MakeTernaryConditionalExpression(ByVal syntax As SyntaxNode, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal whenTrue As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal whenFalse As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim constantValueOpt As ConstantValue = condition.ConstantValueOpt
			boundExpression = If(constantValueOpt Is Nothing, LocalRewriter.TransformRewrittenTernaryConditionalExpression(New BoundTernaryConditionalExpression(syntax, condition, whenTrue, whenFalse, Nothing, whenTrue.Type, False)), LocalRewriter.MakeSequence(syntax, condition, If(CObj(constantValueOpt) = CObj(ConstantValue.[True]), whenTrue, whenFalse)))
			Return boundExpression
		End Function

		Private Function MakeTupleConversion(ByVal syntax As SyntaxNode, ByVal rewrittenOperand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal destinationType As TypeSymbol, ByVal convertedElements As BoundConvertedTupleElements) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim tupleTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol
			If (Not destinationType.IsSameTypeIgnoringAll(rewrittenOperand.Type)) Then
				Dim length As Integer = destinationType.GetElementTypesOfTupleOrCompatible().Length
				Dim type As TypeSymbol = rewrittenOperand.Type
				tupleTypeSymbol = If(Not type.IsTupleType, Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol.Create(DirectCast(type, NamedTypeSymbol)), DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleTypeSymbol))
				Dim tupleElements As ImmutableArray(Of FieldSymbol) = tupleTypeSymbol.TupleElements
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance(length)
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = Nothing
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.CaptureOperand(rewrittenOperand, synthesizedLocal, boundExpression1)
				Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = New Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory(Me._topMethod, Me._currentMethodOrLambda, syntax, Me._compilationState, Me._diagnostics)
				Dim num As Integer = length - 1
				Dim num1 As Integer = 0
				Do
					Dim item As FieldSymbol = tupleElements(num1)
					Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = item.CalculateUseSiteInfo()
					LocalRewriter.ReportUseSite(rewrittenOperand, useSiteInfo, Me._diagnostics)
					Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.MakeTupleFieldAccess(syntax, item, boundExpression2, Nothing, False)
					Dim elementPlaceholders As ImmutableArray(Of BoundRValuePlaceholder) = convertedElements.ElementPlaceholders
					Me.AddPlaceholderReplacement(elementPlaceholders(num1), boundExpression3)
					Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = convertedElements.ConvertedElements
					instance.Add(Me.VisitExpression(boundExpressions(num1)))
					elementPlaceholders = convertedElements.ElementPlaceholders
					Me.RemovePlaceholderReplacement(elementPlaceholders(num1))
					num1 = num1 + 1
				Loop While num1 <= num
				Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.MakeTupleCreationExpression(syntax, DirectCast(destinationType, NamedTypeSymbol), instance.ToImmutableAndFree())
				boundExpression = syntheticBoundNodeFactory.Sequence(synthesizedLocal, New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression1, boundExpression4 })
			Else
				boundExpression = rewrittenOperand
			End If
			Return boundExpression
		End Function

		Private Function MakeTupleCreationExpression(ByVal syntax As SyntaxNode, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol, ByVal rewrittenArguments As ImmutableArray(Of BoundExpression)) As BoundExpression
			Dim boundBadExpression As BoundExpression
			Dim tupleUnderlyingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = If(type.TupleUnderlyingType, type)
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol).GetInstance()
			TupleTypeSymbol.GetUnderlyingTypeChain(tupleUnderlyingType, instance)
			Try
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = instance.Pop()
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(rewrittenArguments, instance.Count * 7, namedTypeSymbol.Arity)
				Dim wellKnownMemberInType As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(TupleTypeSymbol.GetWellKnownMemberInType(namedTypeSymbol.OriginalDefinition, TupleTypeSymbol.GetTupleCtor(namedTypeSymbol.Arity), Me._diagnostics, syntax), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
				If (wellKnownMemberInType IsNot Nothing) Then
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = wellKnownMemberInType.AsMember(namedTypeSymbol)
					Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
					Dim boundObjectCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression(syntax, methodSymbol, boundExpressions, Nothing, namedTypeSymbol, False, bitVector)
					If (instance.Count > 0) Then
						Dim wellKnownMemberInType1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(TupleTypeSymbol.GetWellKnownMemberInType(instance.Peek().OriginalDefinition, TupleTypeSymbol.GetTupleCtor(8), Me._diagnostics, syntax), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
						If (wellKnownMemberInType1 IsNot Nothing) Then
							Do
								Dim boundExpressions1 As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(rewrittenArguments, (instance.Count - 1) * 7, 7)
								Dim boundExpressions2 As ImmutableArray(Of BoundExpression) = boundExpressions1.Add(boundObjectCreationExpression)
								Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = wellKnownMemberInType1.AsMember(instance.Pop())
								Dim containingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = methodSymbol1.ContainingType
								bitVector = New Microsoft.CodeAnalysis.BitVector()
								boundObjectCreationExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression(syntax, methodSymbol1, boundExpressions2, Nothing, containingType, False, bitVector)
							Loop While instance.Count > 0
						Else
							boundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(syntax, LookupResultKind.Empty, ImmutableArray(Of Symbol).Empty, rewrittenArguments, type, True)
							Return boundBadExpression
						End If
					End If
					boundObjectCreationExpression = boundObjectCreationExpression.Update(boundObjectCreationExpression.ConstructorOpt, Nothing, boundObjectCreationExpression.Arguments, boundObjectCreationExpression.DefaultArguments, boundObjectCreationExpression.InitializerOpt, type)
					boundBadExpression = boundObjectCreationExpression
				Else
					boundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(syntax, LookupResultKind.Empty, ImmutableArray(Of Symbol).Empty, rewrittenArguments, type, True)
				End If
			Finally
				instance.Free()
			End Try
			Return boundBadExpression
		End Function

		Private Function MakeTupleFieldAccess(ByVal syntax As SyntaxNode, ByVal tupleField As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol, ByVal rewrittenReceiver As BoundExpression, ByVal constantValueOpt As ConstantValue, ByVal isLValue As Boolean) As BoundExpression
			Dim boundFieldAccess As BoundExpression
			Dim tupleUnderlyingType As NamedTypeSymbol = tupleField.ContainingType.TupleUnderlyingType
			Dim tupleUnderlyingField As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = tupleField.TupleUnderlyingField
			If (tupleUnderlyingField IsNot Nothing) Then
				If (Not TypeSymbol.Equals(tupleUnderlyingField.ContainingType, tupleUnderlyingType, TypeCompareKind.ConsiderEverything)) Then
					Dim tupleTypeMember As WellKnownMember = TupleTypeSymbol.GetTupleTypeMember(8, 8)
					Dim wellKnownMemberInType As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = DirectCast(TupleTypeSymbol.GetWellKnownMemberInType(tupleUnderlyingType.OriginalDefinition, tupleTypeMember, Me._diagnostics, syntax), Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
					If (wellKnownMemberInType Is Nothing) Then
						boundFieldAccess = LocalRewriter.MakeBadFieldAccess(syntax, tupleField, rewrittenReceiver)
						Return boundFieldAccess
					End If
					Do
						Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = wellKnownMemberInType.AsMember(tupleUnderlyingType)
						rewrittenReceiver = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(syntax, rewrittenReceiver, fieldSymbol, isLValue, fieldSymbol.Type, False)
						tupleUnderlyingType = tupleUnderlyingType.TypeArgumentsNoUseSiteDiagnostics(7).TupleUnderlyingType
					Loop While Not TypeSymbol.Equals(tupleUnderlyingField.ContainingType, tupleUnderlyingType, TypeCompareKind.ConsiderEverything)
				End If
				boundFieldAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(syntax, rewrittenReceiver, tupleUnderlyingField, isLValue, tupleUnderlyingField.Type, False)
			Else
				boundFieldAccess = LocalRewriter.MakeBadFieldAccess(syntax, tupleField, rewrittenReceiver)
			End If
			Return boundFieldAccess
		End Function

		Private Function NegateIfStepNegative(ByVal value As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal [step] As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim primitiveType As NamedTypeSymbol = [step].Type.ContainingAssembly.GetPrimitiveType(PrimitiveTypeCode.Int32)
			Dim shiftBits As Integer = [step].Type.GetEnumUnderlyingTypeOrSelf().SpecialType.VBForToShiftBits()
			Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(value.Syntax, ConstantValue.Create(shiftBits), primitiveType)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.TransformRewrittenBinaryOperator(New BoundBinaryOperator(value.Syntax, BinaryOperatorKind.RightShift, [step], boundLiteral, False, [step].Type, False))
			Return Me.TransformRewrittenBinaryOperator(New BoundBinaryOperator(value.Syntax, BinaryOperatorKind.[Xor], boundExpression, value, False, value.Type, False))
		End Function

		Private Shared Function NoParameterRelaxation(ByVal from As BoundExpression, ByVal toLambda As Microsoft.CodeAnalysis.VisualBasic.Symbols.LambdaSymbol) As Boolean
			Dim lambdaSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LambdaSymbol
			Dim boundLambda As Microsoft.CodeAnalysis.VisualBasic.BoundLambda = TryCast(from, Microsoft.CodeAnalysis.VisualBasic.BoundLambda)
			If (boundLambda IsNot Nothing) Then
				lambdaSymbol = boundLambda.LambdaSymbol
			Else
				lambdaSymbol = Nothing
			End If
			Dim lambdaSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.LambdaSymbol = lambdaSymbol
			If (lambdaSymbol1 Is Nothing OrElse lambdaSymbol1.IsSub OrElse Not toLambda.IsSub) Then
				Return False
			End If
			Return MethodSignatureComparer.HaveSameParameterTypes(lambdaSymbol1.Parameters, Nothing, toLambda.Parameters, Nothing, True, False, False)
		End Function

		Private Function NullableFalse(ByVal syntax As SyntaxNode, ByVal nullableOfBoolean As TypeSymbol) As BoundExpression
			Dim nullableUnderlyingType As TypeSymbol = nullableOfBoolean.GetNullableUnderlyingType()
			Return Me.WrapInNullable(New BoundLiteral(syntax, ConstantValue.[False], nullableUnderlyingType), nullableOfBoolean)
		End Function

		Private Function NullableHasValue(ByVal expr As BoundExpression) As BoundExpression
			Dim boundBadExpression As BoundExpression
			Dim nullableMethod As MethodSymbol = Me.GetNullableMethod(expr.Syntax, expr.Type, SpecialMember.System_Nullable_T_get_HasValue)
			If (nullableMethod Is Nothing) Then
				boundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(expr.Syntax, LookupResultKind.NotReferencable, ImmutableArray(Of Symbol).Empty, ImmutableArray.Create(Of BoundExpression)(expr), Me.Compilation.GetSpecialType(SpecialType.System_Boolean), True)
			Else
				boundBadExpression = New BoundCall(expr.Syntax, nullableMethod, Nothing, expr, ImmutableArray(Of BoundExpression).Empty, Nothing, False, True, nullableMethod.ReturnType, False)
			End If
			Return boundBadExpression
		End Function

		Private Shared Function NullableNull(ByVal syntax As SyntaxNode, ByVal nullableType As TypeSymbol) As BoundExpression
			Dim empty As ImmutableArray(Of !0) = ImmutableArray(Of BoundExpression).Empty
			Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
			Return New BoundObjectCreationExpression(syntax, Nothing, empty, Nothing, nullableType, False, bitVector)
		End Function

		Private Shared Function NullableNull(ByVal candidateNullExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			boundExpression = If(Not type.IsSameTypeIgnoringAll(candidateNullExpression.Type) OrElse candidateNullExpression.Kind <> BoundKind.ObjectCreationExpression, LocalRewriter.NullableNull(candidateNullExpression.Syntax, type), candidateNullExpression)
			Return boundExpression
		End Function

		Private Function NullableOfBooleanValue(ByVal syntax As SyntaxNode, ByVal isTrue As Boolean, ByVal nullableOfBoolean As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			boundExpression = If(Not isTrue, Me.NullableFalse(syntax, nullableOfBoolean), Me.NullableTrue(syntax, nullableOfBoolean))
			Return boundExpression
		End Function

		Private Function NullableTrue(ByVal syntax As SyntaxNode, ByVal nullableOfBoolean As TypeSymbol) As BoundExpression
			Dim nullableUnderlyingType As TypeSymbol = nullableOfBoolean.GetNullableUnderlyingType()
			Return Me.WrapInNullable(New BoundLiteral(syntax, ConstantValue.[True], nullableUnderlyingType), nullableOfBoolean)
		End Function

		Private Function NullableValue(ByVal expr As BoundExpression) As BoundExpression
			Dim boundBadExpression As BoundExpression
			If (Not LocalRewriter.HasValue(expr)) Then
				Dim nullableMethod As MethodSymbol = Me.GetNullableMethod(expr.Syntax, expr.Type, SpecialMember.System_Nullable_T_get_Value)
				If (nullableMethod Is Nothing) Then
					boundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(expr.Syntax, LookupResultKind.NotReferencable, ImmutableArray(Of Symbol).Empty, ImmutableArray.Create(Of BoundExpression)(expr), expr.Type.GetNullableUnderlyingType(), True)
				Else
					boundBadExpression = New BoundCall(expr.Syntax, nullableMethod, Nothing, expr, ImmutableArray(Of BoundExpression).Empty, Nothing, False, True, nullableMethod.ReturnType, False)
				End If
			Else
				boundBadExpression = Me.NullableValueOrDefault(expr)
			End If
			Return boundBadExpression
		End Function

		Private Function NullableValueOrDefault(ByVal operand As BoundExpression, ByVal operandHasValue As Boolean) As BoundExpression
			Dim boundNullableIsTrueOperator As BoundExpression
			If (Not Me._inExpressionLambda OrElse operandHasValue) Then
				boundNullableIsTrueOperator = Me.NullableValueOrDefault(operand)
			Else
				boundNullableIsTrueOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundNullableIsTrueOperator(operand.Syntax, operand, operand.Type.GetNullableUnderlyingType(), False)
			End If
			Return boundNullableIsTrueOperator
		End Function

		Private Function NullableValueOrDefault(ByVal expr As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundBadExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim nullableMethod As MethodSymbol
			Dim kind As BoundKind = expr.Kind
			If (kind = BoundKind.Conversion) Then
				Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundConversion = DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.BoundConversion)
				If (Not LocalRewriter.IsConversionFromUnderlyingToNullable(boundConversion)) Then
					nullableMethod = Me.GetNullableMethod(expr.Syntax, expr.Type, SpecialMember.System_Nullable_T_GetValueOrDefault)
					If (nullableMethod Is Nothing) Then
						boundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(expr.Syntax, LookupResultKind.NotReferencable, ImmutableArray(Of Symbol).Empty, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(expr), expr.Type.GetNullableUnderlyingType(), True)
					Else
						boundBadExpression = New BoundCall(expr.Syntax, nullableMethod, Nothing, expr, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty, Nothing, False, True, nullableMethod.ReturnType, False)
					End If
					Return boundBadExpression
				End If
				boundBadExpression = boundConversion.Operand
				Return boundBadExpression
			ElseIf (kind = BoundKind.ObjectCreationExpression) Then
				Dim boundObjectCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression = DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression)
				If (boundObjectCreationExpression.Arguments.Length <> 1) Then
					nullableMethod = Me.GetNullableMethod(expr.Syntax, expr.Type, SpecialMember.System_Nullable_T_GetValueOrDefault)
					If (nullableMethod Is Nothing) Then
						boundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(expr.Syntax, LookupResultKind.NotReferencable, ImmutableArray(Of Symbol).Empty, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(expr), expr.Type.GetNullableUnderlyingType(), True)
					Else
						boundBadExpression = New BoundCall(expr.Syntax, nullableMethod, Nothing, expr, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty, Nothing, False, True, nullableMethod.ReturnType, False)
					End If
					Return boundBadExpression
				End If
				boundBadExpression = boundObjectCreationExpression.Arguments(0)
				Return boundBadExpression
			ElseIf (Not Me._inExpressionLambda AndAlso expr.Type.IsNullableOfBoolean()) Then
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				If (Not LocalRewriter.IsConditionalAccess(expr, boundExpression, boundExpression1) OrElse Not LocalRewriter.HasNoValue(boundExpression1)) Then
					nullableMethod = Me.GetNullableMethod(expr.Syntax, expr.Type, SpecialMember.System_Nullable_T_GetValueOrDefault)
					If (nullableMethod Is Nothing) Then
						boundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(expr.Syntax, LookupResultKind.NotReferencable, ImmutableArray(Of Symbol).Empty, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(expr), expr.Type.GetNullableUnderlyingType(), True)
					Else
						boundBadExpression = New BoundCall(expr.Syntax, nullableMethod, Nothing, expr, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty, Nothing, False, True, nullableMethod.ReturnType, False)
					End If
					Return boundBadExpression
				End If
				boundBadExpression = LocalRewriter.UpdateConditionalAccess(expr, Me.NullableValueOrDefault(boundExpression), New BoundLiteral(expr.Syntax, ConstantValue.[False], expr.Type.GetNullableUnderlyingType()))
				Return boundBadExpression
			End If
			nullableMethod = Me.GetNullableMethod(expr.Syntax, expr.Type, SpecialMember.System_Nullable_T_GetValueOrDefault)
			If (nullableMethod Is Nothing) Then
				boundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(expr.Syntax, LookupResultKind.NotReferencable, ImmutableArray(Of Symbol).Empty, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(expr), expr.Type.GetNullableUnderlyingType(), True)
			Else
				boundBadExpression = New BoundCall(expr.Syntax, nullableMethod, Nothing, expr, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty, Nothing, False, True, nullableMethod.ReturnType, False)
			End If
			Return boundBadExpression
		End Function

		Private Function PassArgAsTempClone(ByVal argument As BoundExpression, ByVal rewrittenArgument As BoundExpression, ByRef tempsArray As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal)) As BoundExpression
			If (tempsArray Is Nothing) Then
				tempsArray = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal).GetInstance()
			End If
			Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, rewrittenArgument.Type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
			tempsArray.Add(synthesizedLocal)
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(rewrittenArgument.Syntax, synthesizedLocal, synthesizedLocal.Type)
			Dim boundAssignmentOperator As BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(rewrittenArgument.Syntax, boundLocal, Me.GenerateObjectCloneIfNeeded(argument, rewrittenArgument), True, rewrittenArgument.Type, False)
			Return New BoundSequence(rewrittenArgument.Syntax, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundExpression)(boundAssignmentOperator), boundLocal, rewrittenArgument.Type, False)
		End Function

		Private Shared Sub PopulateRangeVariableMapForAnonymousType(ByVal syntax As SyntaxNode, ByVal anonymousTypeInstance As BoundExpression, ByVal rangeVariables As ImmutableArray(Of RangeVariableSymbol), ByRef firstUnmappedRangeVariable As Integer, ByVal rangeVariableMap As Dictionary(Of RangeVariableSymbol, BoundExpression), ByVal inExpressionLambda As Boolean)
			Dim enumerator As ImmutableArray(Of AnonymousTypeManager.AnonymousTypePropertyPublicSymbol).Enumerator = DirectCast(anonymousTypeInstance.Type, AnonymousTypeManager.AnonymousTypePublicSymbol).Properties.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As PropertySymbol = enumerator.Current
				Dim boundCall As BoundExpression = Nothing
				If (Not inExpressionLambda) Then
					Dim getMethod As MethodSymbol = current.GetMethod
					Dim empty As ImmutableArray(Of !0) = ImmutableArray(Of BoundExpression).Empty
					Dim returnType As TypeSymbol = getMethod.ReturnType
					Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
					boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, getMethod, Nothing, anonymousTypeInstance, empty, Nothing, returnType, False, False, bitVector)
				Else
					boundCall = New BoundPropertyAccess(syntax, current, Nothing, PropertyAccessKind.[Get], False, False, anonymousTypeInstance, ImmutableArray(Of BoundExpression).Empty, Microsoft.CodeAnalysis.BitVector.Null, current.Type, False)
				End If
				Dim name As String = current.Name
				If (Not name.StartsWith("$", StringComparison.Ordinal) OrElse Not LocalRewriter.IsCompoundVariableName(name)) Then
					rangeVariableMap.Add(rangeVariables(firstUnmappedRangeVariable), boundCall)
					firstUnmappedRangeVariable = firstUnmappedRangeVariable + 1
				Else
					LocalRewriter.PopulateRangeVariableMapForAnonymousType(syntax, boundCall.MakeCompilerGenerated(), rangeVariables, firstUnmappedRangeVariable, rangeVariableMap, inExpressionLambda)
				End If
			End While
		End Sub

		Friend Shared Sub PopulateRangeVariableMapForQueryLambdaRewrite(ByVal node As BoundQueryLambda, ByRef rangeVariableMap As Dictionary(Of RangeVariableSymbol, BoundExpression), ByVal inExpressionLambda As Boolean)
			Dim rangeVariables As ImmutableArray(Of RangeVariableSymbol) = node.RangeVariables
			If (rangeVariables.Length > 0) Then
				If (rangeVariableMap Is Nothing) Then
					rangeVariableMap = New Dictionary(Of RangeVariableSymbol, BoundExpression)()
				End If
				Dim num As Integer = 0
				Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = node.LambdaSymbol.Parameters.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As ParameterSymbol = enumerator.Current
					Dim name As String = current.Name
					Dim flag As Boolean = name.StartsWith("$", StringComparison.Ordinal)
					If (flag AndAlso [String].Equals(name, "$VB$ItAnonymous", StringComparison.Ordinal)) Then
						Continue While
					End If
					Dim boundParameter As Microsoft.CodeAnalysis.VisualBasic.BoundParameter = New Microsoft.CodeAnalysis.VisualBasic.BoundParameter(node.Syntax, current, False, current.Type)
					If (Not flag OrElse Not LocalRewriter.IsCompoundVariableName(name)) Then
						rangeVariableMap.Add(rangeVariables(num), boundParameter)
						num = num + 1
					Else
						If (current.Type.IsErrorType()) Then
							Exit While
						End If
						LocalRewriter.PopulateRangeVariableMapForAnonymousType(node.Syntax, boundParameter.MakeCompilerGenerated(), rangeVariables, num, rangeVariableMap, inExpressionLambda)
					End If
				End While
			End If
		End Sub

		Private Shared Function PrependWithPrologue(ByVal statement As BoundStatement, ByVal prologueOpt As BoundStatement) As BoundStatement
			Dim boundStatementList As BoundStatement
			If (prologueOpt IsNot Nothing) Then
				boundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(statement.Syntax, ImmutableArray.Create(Of BoundStatement)(prologueOpt, statement), False)
			Else
				boundStatementList = statement
			End If
			Return boundStatementList
		End Function

		Private Shared Function PrependWithPrologue(ByVal block As Microsoft.CodeAnalysis.VisualBasic.BoundBlock, ByVal prologueOpt As BoundStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			If (prologueOpt IsNot Nothing) Then
				Dim syntax As SyntaxNode = block.Syntax
				Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
				boundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundStatement)(prologueOpt, block), False)
			Else
				boundBlock = block
			End If
			Return boundBlock
		End Function

		Private Function ProcessNullableOperand(ByVal operand As BoundExpression, <Out> ByRef hasValueExpr As BoundExpression, ByRef temps As ArrayBuilder(Of LocalSymbol), ByRef inits As ArrayBuilder(Of BoundExpression), ByVal doNotCaptureLocals As Boolean) As BoundExpression
			Return Me.ProcessNullableOperand(operand, hasValueExpr, temps, inits, doNotCaptureLocals, LocalRewriter.HasValue(operand))
		End Function

		Private Function ProcessNullableOperand(ByVal operand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, <Out> ByRef hasValueExpr As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByRef temps As ArrayBuilder(Of LocalSymbol), ByRef inits As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression), ByVal doNotCaptureLocals As Boolean, ByVal operandHasValue As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (operandHasValue) Then
				operand = Me.NullableValueOrDefault(operand)
			End If
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.CaptureNullableIfNeeded(operand, temps, inits, doNotCaptureLocals)
			If (Not operandHasValue) Then
				hasValueExpr = Me.NullableHasValue(boundExpression1)
				boundExpression = Me.NullableValueOrDefault(boundExpression1)
			Else
				hasValueExpr = New BoundLiteral(operand.Syntax, ConstantValue.[True], Me.GetSpecialType(SpecialType.System_Boolean))
				boundExpression = boundExpression1
			End If
			Return boundExpression
		End Function

		Private Function RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(ByVal syntax As SyntaxNode) As BoundLabelStatement
			Return Me.AddResumeTargetLabel(syntax)
		End Function

		Private Sub RegisterUnstructuredExceptionHandlingResumeTarget(ByVal syntax As SyntaxNode, ByVal canThrow As Boolean, ByVal statements As ArrayBuilder(Of BoundStatement))
			Me.AddResumeTargetLabelAndUpdateCurrentStatementTemporary(syntax, canThrow, statements)
		End Sub

		Private Function RegisterUnstructuredExceptionHandlingResumeTarget(ByVal syntax As SyntaxNode, ByVal node As BoundStatement, ByVal canThrow As Boolean) As BoundStatement
			Dim instance As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
			Me.AddResumeTargetLabelAndUpdateCurrentStatementTemporary(syntax, canThrow, instance)
			instance.Add(node)
			Return New BoundStatementList(syntax, instance.ToImmutableAndFree(), False)
		End Function

		Private Function RegisterUnstructuredExceptionHandlingResumeTarget(ByVal syntax As SyntaxNode, ByVal canThrow As Boolean) As ImmutableArray(Of BoundStatement)
			Dim instance As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
			Me.AddResumeTargetLabelAndUpdateCurrentStatementTemporary(syntax, canThrow, instance)
			Return instance.ToImmutableAndFree()
		End Function

		Private Sub RemovePlaceholderReplacement(ByVal placeholder As BoundValuePlaceholderBase)
			Me._placeholderReplacementMapDoNotUseDirectly.Remove(placeholder)
		End Sub

		Friend Shared Sub RemoveRangeVariables(ByVal originalNode As BoundQueryLambda, ByVal rangeVariableMap As Dictionary(Of RangeVariableSymbol, BoundExpression))
			Dim enumerator As ImmutableArray(Of RangeVariableSymbol).Enumerator = originalNode.RangeVariables.GetEnumerator()
			While enumerator.MoveNext()
				rangeVariableMap.Remove(enumerator.Current)
			End While
		End Sub

		Private Function ReplaceMyGroupCollectionPropertyGetWithUnderlyingField(ByVal operand As BoundExpression) As BoundExpression
			Dim boundFieldAccess As BoundExpression
			If (Not operand.HasErrors) Then
				Dim kind As BoundKind = operand.Kind
				If (kind <= BoundKind.[DirectCast]) Then
					If (kind = BoundKind.Conversion) Then
						Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundConversion = DirectCast(operand, Microsoft.CodeAnalysis.VisualBasic.BoundConversion)
						boundFieldAccess = boundConversion.Update(Me.ReplaceMyGroupCollectionPropertyGetWithUnderlyingField(boundConversion.Operand), boundConversion.ConversionKind, boundConversion.Checked, boundConversion.ExplicitCastInCode, boundConversion.ConstantValueOpt, boundConversion.ExtendedInfoOpt, boundConversion.Type)
						Return boundFieldAccess
					Else
						If (kind <> BoundKind.[DirectCast]) Then
							GoTo Label1
						End If
						Dim boundDirectCast As Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast = DirectCast(operand, Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast)
						boundFieldAccess = boundDirectCast.Update(Me.ReplaceMyGroupCollectionPropertyGetWithUnderlyingField(boundDirectCast.Operand), boundDirectCast.ConversionKind, boundDirectCast.SuppressVirtualCalls, boundDirectCast.ConstantValueOpt, boundDirectCast.RelaxationLambdaOpt, boundDirectCast.Type)
						Return boundFieldAccess
					End If
				ElseIf (kind = BoundKind.[Call]) Then
					Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = DirectCast(operand, Microsoft.CodeAnalysis.VisualBasic.BoundCall)
					If (boundCall.Method.MethodKind <> MethodKind.PropertyGet OrElse boundCall.Method.AssociatedSymbol Is Nothing OrElse Not boundCall.Method.AssociatedSymbol.IsMyGroupCollectionProperty) Then
						GoTo Label1
					End If
					boundFieldAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(boundCall.Syntax, boundCall.ReceiverOpt, DirectCast(boundCall.Method.AssociatedSymbol, PropertySymbol).AssociatedField, False, boundCall.Type, False)
					Return boundFieldAccess
				ElseIf (kind = BoundKind.PropertyAccess) Then
					Dim boundPropertyAccess As Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess = DirectCast(operand, Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess)
					If (boundPropertyAccess.AccessKind <> PropertyAccessKind.[Get] OrElse Not boundPropertyAccess.PropertySymbol.IsMyGroupCollectionProperty) Then
						GoTo Label1
					End If
					boundFieldAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(boundPropertyAccess.Syntax, boundPropertyAccess.ReceiverOpt, boundPropertyAccess.PropertySymbol.AssociatedField, False, boundPropertyAccess.Type, False)
					Return boundFieldAccess
				End If
			Label1:
				boundFieldAccess = operand
			Else
				boundFieldAccess = operand
			End If
			Return boundFieldAccess
		End Function

		Private Function ReplaceObjectOrCollectionInitializer(ByVal rewrittenObjectCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal rewrittenInitializer As BoundObjectInitializerExpressionBase) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim kind As BoundKind = rewrittenObjectCreationExpression.Kind
			If (kind = BoundKind.ObjectCreationExpression) Then
				Dim boundObjectCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression = DirectCast(rewrittenObjectCreationExpression, Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression)
				boundExpression = boundObjectCreationExpression.Update(boundObjectCreationExpression.ConstructorOpt, boundObjectCreationExpression.Arguments, boundObjectCreationExpression.DefaultArguments, rewrittenInitializer, boundObjectCreationExpression.Type)
			ElseIf (kind = BoundKind.NewT) Then
				Dim boundNewT As Microsoft.CodeAnalysis.VisualBasic.BoundNewT = DirectCast(rewrittenObjectCreationExpression, Microsoft.CodeAnalysis.VisualBasic.BoundNewT)
				boundExpression = boundNewT.Update(rewrittenInitializer, boundNewT.Type)
			Else
				If (kind <> BoundKind.Sequence) Then
					Throw ExceptionUtilities.UnexpectedValue(rewrittenObjectCreationExpression.Kind)
				End If
				Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSequence = DirectCast(rewrittenObjectCreationExpression, Microsoft.CodeAnalysis.VisualBasic.BoundSequence)
				boundExpression = boundSequence.Update(boundSequence.Locals, boundSequence.SideEffects, Me.ReplaceObjectOrCollectionInitializer(boundSequence.ValueOpt, rewrittenInitializer), boundSequence.Type)
			End If
			Return boundExpression
		End Function

		Private Sub ReportBadType(ByVal node As BoundNode, ByVal typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)
			LocalRewriter.ReportUseSite(node, typeSymbol.GetUseSiteInfo(), Me._diagnostics)
		End Sub

		Private Shared Sub ReportDiagnostic(ByVal node As BoundNode, ByVal diagnostic As DiagnosticInfo, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			diagnostics.Add(New VBDiagnostic(diagnostic, node.Syntax.GetLocation(), False))
		End Sub

		Private Sub ReportErrorsOnCatchBlockHelpers(ByVal node As BoundCatchBlock)
			Dim wellKnownMember As Microsoft.CodeAnalysis.WellKnownMember = If(node.ErrorLineNumberOpt Is Nothing, Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__SetProjectError, Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__SetProjectError_Int32)
			Dim wellKnownTypeMember As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(Me.Compilation.GetWellKnownTypeMember(wellKnownMember), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			Me.ReportMissingOrBadRuntimeHelper(node, wellKnownMember, wellKnownTypeMember)
			If (node.ExceptionFilterOpt Is Nothing OrElse node.ExceptionFilterOpt.Kind <> BoundKind.UnstructuredExceptionHandlingCatchFilter) Then
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(Me.Compilation.GetWellKnownTypeMember(Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__ClearProjectError), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
				Me.ReportMissingOrBadRuntimeHelper(node, Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__ClearProjectError, methodSymbol)
			End If
		End Sub

		Private Function ReportMissingOrBadRuntimeHelper(ByVal node As BoundNode, ByVal specialMember As Microsoft.CodeAnalysis.SpecialMember, ByVal memberSymbol As Symbol) As Boolean
			Return LocalRewriter.ReportMissingOrBadRuntimeHelper(node, specialMember, memberSymbol, Me._diagnostics, Me._compilationState.Compilation.Options.EmbedVbCoreRuntime)
		End Function

		Friend Shared Function ReportMissingOrBadRuntimeHelper(ByVal node As BoundNode, ByVal specialMember As Microsoft.CodeAnalysis.SpecialMember, ByVal memberSymbol As Symbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByVal embedVBCoreRuntime As Boolean = False) As Boolean
			Dim flag As Boolean
			If (memberSymbol IsNot Nothing) Then
				flag = LocalRewriter.ReportUseSite(node, Binder.GetUseSiteInfoForMemberAndContainingType(memberSymbol), diagnostics)
			Else
				LocalRewriter.ReportMissingRuntimeHelper(node, specialMember, diagnostics, embedVBCoreRuntime)
				flag = True
			End If
			Return flag
		End Function

		Private Function ReportMissingOrBadRuntimeHelper(ByVal node As BoundNode, ByVal wellKnownMember As Microsoft.CodeAnalysis.WellKnownMember, ByVal memberSymbol As Symbol) As Boolean
			Return LocalRewriter.ReportMissingOrBadRuntimeHelper(node, wellKnownMember, memberSymbol, Me._diagnostics, Me._compilationState.Compilation.Options.EmbedVbCoreRuntime)
		End Function

		Friend Shared Function ReportMissingOrBadRuntimeHelper(ByVal node As BoundNode, ByVal wellKnownMember As Microsoft.CodeAnalysis.WellKnownMember, ByVal memberSymbol As Symbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal embedVBCoreRuntime As Boolean) As Boolean
			Dim flag As Boolean
			If (memberSymbol IsNot Nothing) Then
				flag = LocalRewriter.ReportUseSite(node, Binder.GetUseSiteInfoForMemberAndContainingType(memberSymbol), diagnostics)
			Else
				LocalRewriter.ReportMissingRuntimeHelper(node, wellKnownMember, diagnostics, embedVBCoreRuntime)
				flag = True
			End If
			Return flag
		End Function

		Private Shared Sub ReportMissingRuntimeHelper(ByVal node As BoundNode, ByVal specialMember As Microsoft.CodeAnalysis.SpecialMember, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByVal embedVBCoreRuntime As Boolean = False)
			Dim descriptor As MemberDescriptor = SpecialMembers.GetDescriptor(specialMember)
			Dim declaringTypeMetadataName As String = descriptor.DeclaringTypeMetadataName
			LocalRewriter.ReportMissingRuntimeHelper(node, declaringTypeMetadataName, descriptor.Name, diagnostics, embedVBCoreRuntime)
		End Sub

		Private Shared Sub ReportMissingRuntimeHelper(ByVal node As BoundNode, ByVal wellKnownMember As Microsoft.CodeAnalysis.WellKnownMember, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal embedVBCoreRuntime As Boolean)
			Dim descriptor As MemberDescriptor = WellKnownMembers.GetDescriptor(wellKnownMember)
			Dim declaringTypeMetadataName As String = descriptor.DeclaringTypeMetadataName
			LocalRewriter.ReportMissingRuntimeHelper(node, declaringTypeMetadataName, descriptor.Name, diagnostics, embedVBCoreRuntime)
		End Sub

		Private Shared Sub ReportMissingRuntimeHelper(ByVal node As BoundNode, ByVal typeName As String, ByVal memberName As String, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal embedVBCoreRuntime As Boolean)
			If (memberName.Equals(".ctor") OrElse memberName.Equals(".cctor")) Then
				memberName = "New"
			End If
			Dim diagnosticForMissingRuntimeHelper As DiagnosticInfo = MissingRuntimeMemberDiagnosticHelper.GetDiagnosticForMissingRuntimeHelper(typeName, memberName, embedVBCoreRuntime)
			LocalRewriter.ReportDiagnostic(node, diagnosticForMissingRuntimeHelper, diagnostics)
		End Sub

		Private Shared Function ReportUseSite(ByVal node As BoundNode, ByVal useSiteInfo As UseSiteInfo(Of AssemblySymbol), ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Boolean
			Return diagnostics.Add(useSiteInfo, node.Syntax.GetLocation())
		End Function

		Private Sub RestoreUnstructuredExceptionHandlingContext(ByVal node As BoundNode, ByVal saved As LocalRewriter.UnstructuredExceptionHandlingContext)
			Me._unstructuredExceptionHandling.Context = saved.Context
		End Sub

		Private Function ReturnsWholeNumberDouble(ByVal node As BoundCall) As Boolean
			Dim method As Boolean
			Dim name As String = node.Method.Name
			If ("Ceiling".Equals(name)) Then
				method = node.Method = Me.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Math__CeilingDouble)
			ElseIf ("Floor".Equals(name)) Then
				method = node.Method = Me.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Math__FloorDouble)
			ElseIf ("Round".Equals(name)) Then
				method = node.Method = Me.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Math__RoundDouble)
			ElseIf (Not "Int".Equals(name)) Then
				method = False
			Else
				Dim specialType As Microsoft.CodeAnalysis.SpecialType = node.Type.SpecialType
				If (specialType = Microsoft.CodeAnalysis.SpecialType.System_Single) Then
					method = node.Method = Me.Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Conversion__IntSingle)
				Else
					method = If(specialType = Microsoft.CodeAnalysis.SpecialType.System_Double, node.Method = Me.Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_Conversion__IntDouble), False)
				End If
			End If
			Return method
		End Function

		Public Shared Function Rewrite(ByVal node As BoundBlock, ByVal topMethod As MethodSymbol, ByVal compilationState As TypeCompilationState, ByVal previousSubmissionFields As SynthesizedSubmissionFields, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, <Out> ByRef rewrittenNodes As HashSet(Of BoundNode), <Out> ByRef hasLambdas As Boolean, <Out> ByRef symbolsCapturedWithoutCopyCtor As ISet(Of Symbol), ByVal flags As LocalRewriter.RewritingFlags, ByVal instrumenterOpt As Instrumenter, ByVal currentMethod As MethodSymbol) As BoundBlock
			Return DirectCast(LocalRewriter.RewriteNode(node, topMethod, If(currentMethod, topMethod), compilationState, previousSubmissionFields, diagnostics, rewrittenNodes, hasLambdas, symbolsCapturedWithoutCopyCtor, flags, instrumenterOpt, 0), BoundBlock)
		End Function

		Private Function RewriteAddRemoveHandler(ByVal node As BoundAddRemoveHandlerStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundEventAccess As Microsoft.CodeAnalysis.VisualBasic.BoundEventAccess = Me.UnwrapEventAccess(node.EventAccess)
			Dim eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol = boundEventAccess.EventSymbol
			Dim unstructuredExceptionHandlingContext As LocalRewriter.UnstructuredExceptionHandlingContext = Me.LeaveUnstructuredExceptionHandlingContext(node)
			boundStatement = If(Not eventSymbol.IsWindowsRuntimeEvent, Me.MakeEventAccessorCall(node, boundEventAccess, If(node.Kind = BoundKind.AddHandlerStatement, eventSymbol.AddMethod, eventSymbol.RemoveMethod)), Me.RewriteWinRtEvent(node, boundEventAccess, node.Kind = BoundKind.AddHandlerStatement))
			Me.RestoreUnstructuredExceptionHandlingContext(node, unstructuredExceptionHandlingContext)
			If (Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)) Then
				boundStatement = Me.RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, boundStatement, True)
			End If
			Return boundStatement
		End Function

		Private Function RewriteAndOptimizeLiftedIntrinsicLogicalShortCircuitingOperator(ByVal node As BoundBinaryOperator, ByVal left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal right As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal leftHasNoValue As Boolean, ByVal leftHasValue As Boolean, ByVal rightHasNoValue As Boolean, ByVal rightHasValue As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
			If (Not Me._inExpressionLambda) Then
				If (leftHasNoValue And rightHasNoValue) Then
					boundExpression = LocalRewriter.NullableNull(left, node.Type)
					Return boundExpression
				ElseIf ((node.OperatorKind And BinaryOperatorKind.OpMask) = BinaryOperatorKind.[OrElse]) Then
					If (Not leftHasNoValue) Then
						If (Not rightHasNoValue) Then
							If (boundSequence Is Nothing) Then
								boundSequence = Me.ApplyUnliftedBinaryOp(node, Me.NullableValueOrDefault(left, leftHasValue), Me.NullableValueOrDefault(right, rightHasValue))
							End If
							boundExpression = Me.WrapInNullable(boundSequence, node.Type)
							Return boundExpression
						End If
						boundExpression = left
						Return boundExpression
					Else
						boundExpression = right
						Return boundExpression
					End If
				ElseIf (leftHasNoValue) Then
					boundSequence = Me.EvaluateOperandAndReturnFalse(node, right, rightHasValue)
				ElseIf (rightHasNoValue) Then
					boundSequence = Me.EvaluateOperandAndReturnFalse(node, left, leftHasValue)
				ElseIf (Not leftHasValue) Then
					Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = Nothing
					Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
					Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.CaptureNullableIfNeeded(left, synthesizedLocal, boundExpression1, LocalRewriter.RightCantChangeLeftLocal(left, right))
					boundSequence = Me.MakeBooleanBinaryExpression(node.Syntax, BinaryOperatorKind.[AndAlso], Me.MakeBooleanBinaryExpression(node.Syntax, BinaryOperatorKind.[OrElse], New BoundUnaryOperator(node.Syntax, UnaryOperatorKind.[Not], Me.NullableHasValue(boundExpression2), False, node.Type.GetNullableUnderlyingType(), False), Me.NullableValueOrDefault(boundExpression2)), Me.MakeBooleanBinaryExpression(node.Syntax, BinaryOperatorKind.[AndAlso], Me.NullableValueOrDefault(right), Me.NullableHasValue(boundExpression2)))
					If (synthesizedLocal IsNot Nothing) Then
						boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(node.Syntax, ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal), ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpression1), boundSequence, boundSequence.Type, False)
					End If
				End If
			End If
			If (boundSequence Is Nothing) Then
				boundSequence = Me.ApplyUnliftedBinaryOp(node, Me.NullableValueOrDefault(left, leftHasValue), Me.NullableValueOrDefault(right, rightHasValue))
			End If
			boundExpression = Me.WrapInNullable(boundSequence, node.Type)
			Return boundExpression
		End Function

		Private Function RewriteAnonymousDelegateConversion(ByVal node As BoundConversion) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim lambda As BoundLambda
			Dim receiverPlaceholderOpt As BoundRValuePlaceholder
			Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = New Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory(Me._topMethod, Me._currentMethodOrLambda, node.Syntax, Me._compilationState, Me._diagnostics)
			If (Not node.Operand.IsDefaultValueConstant()) Then
				If (node.ExtendedInfoOpt Is Nothing) Then
					lambda = Nothing
					receiverPlaceholderOpt = Nothing
				Else
					Dim extendedInfoOpt As BoundRelaxationLambda = DirectCast(node.ExtendedInfoOpt, BoundRelaxationLambda)
					lambda = extendedInfoOpt.Lambda
					receiverPlaceholderOpt = extendedInfoOpt.ReceiverPlaceholderOpt
				End If
				If (Me._inExpressionLambda OrElse Not Me.CouldPossiblyBeNothing(syntheticBoundNodeFactory, node.Operand)) Then
					Dim boundDelegateCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression(node.Syntax, node.Operand, DirectCast(node.Operand.Type, NamedTypeSymbol).DelegateInvokeMethod, lambda, receiverPlaceholderOpt, Nothing, node.Type, False)
					boundNode = Me.VisitExpression(boundDelegateCreationExpression)
				Else
					Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = syntheticBoundNodeFactory.SynthesizedLocal(node.Operand.Type, SynthesizedLocalKind.LoweringTemp, Nothing)
					Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = syntheticBoundNodeFactory.ReferenceIsNothing(syntheticBoundNodeFactory.Local(localSymbol, False))
					Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = syntheticBoundNodeFactory.Null(node.Type)
					Dim boundDelegateCreationExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression(node.Syntax, syntheticBoundNodeFactory.Local(localSymbol, False), DirectCast(node.Operand.Type, NamedTypeSymbol).DelegateInvokeMethod, lambda, receiverPlaceholderOpt, Nothing, node.Type, False)
					Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = syntheticBoundNodeFactory.TernaryConditionalExpression(boundBinaryOperator, boundExpression, boundDelegateCreationExpression1)
					boundNode = syntheticBoundNodeFactory.Sequence(localSymbol, New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { syntheticBoundNodeFactory.AssignmentExpression(syntheticBoundNodeFactory.Local(localSymbol, True), Me.VisitExpression(node.Operand)), Me.VisitExpression(boundExpression1) })
				End If
			Else
				boundNode = syntheticBoundNodeFactory.Null(node.Type)
			End If
			Return boundNode
		End Function

		Private Shared Function RewriteAsDirectCast(ByVal node As BoundConversion) As BoundExpression
			Return New BoundDirectCast(node.Syntax, node.Operand, node.ConversionKind, node.Type, False)
		End Function

		Private Function RewriteBinaryConditionalExpressionInExpressionLambda(ByVal node As BoundBinaryConditionalExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim testExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.TestExpression
			Dim type As TypeSymbol = testExpression.Type
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(testExpression)
			Dim boundConversion1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
			Dim convertedTestExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.ConvertedTestExpression
			If (convertedTestExpression IsNot Nothing) Then
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = convertedTestExpression
				Dim testExpressionPlaceholder As BoundRValuePlaceholder = node.TestExpressionPlaceholder
				If (type.IsNullableType()) Then
					boundConversion = New Microsoft.CodeAnalysis.VisualBasic.BoundConversion(boundExpression.Syntax, boundExpression, ConversionKind.WideningNullable, False, False, type.GetNullableUnderlyingTypeOrSelf(), False)
				Else
					boundConversion = boundExpression
				End If
				boundConversion1 = Me.VisitExpressionNode(boundExpression1, testExpressionPlaceholder, boundConversion)
			ElseIf (Not type.IsNullableOfBoolean() AndAlso type.IsNullableType()) Then
				boundConversion1 = New Microsoft.CodeAnalysis.VisualBasic.BoundConversion(boundExpression.Syntax, boundExpression, ConversionKind.WideningNullable, False, False, type.GetNullableUnderlyingTypeOrSelf(), False)
			End If
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(node.ElseExpression)
			Return node.Update(boundExpression, boundConversion1, Nothing, boundExpression2, node.ConstantValueOpt, node.Type)
		End Function

		Private Function RewriteBinaryOperatorSimple(ByVal node As BoundBinaryOperator, ByVal optimizeForConditionalBranch As Boolean) As BoundNode
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundNode Microsoft.CodeAnalysis.VisualBasic.LocalRewriter::RewriteBinaryOperatorSimple(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator,System.Boolean)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundNode RewriteBinaryOperatorSimple(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator,System.Boolean)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Function RewriteByRefArgumentWithCopyBack(ByVal argument As BoundByRefArgumentWithCopyBack, ByRef tempsArray As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal), ByRef copyBackArray As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim second As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim originalArgument As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = argument.OriginalArgument
			If (originalArgument.IsPropertyOrXmlPropertyAccess()) Then
				originalArgument = originalArgument.SetAccessKind(PropertyAccessKind.Unknown)
			ElseIf (originalArgument.IsLateBound()) Then
				originalArgument = originalArgument.SetLateBoundAccessKind(LateBoundAccessKind.Unknown)
			End If
			If (Not Me._inExpressionLambda) Then
				If (tempsArray Is Nothing) Then
					tempsArray = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal).GetInstance()
				End If
				If (copyBackArray Is Nothing) Then
					copyBackArray = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
				End If
				Dim result As UseTwiceRewriter.Result = UseTwiceRewriter.UseTwice(Me._currentMethodOrLambda, originalArgument, tempsArray)
				If (originalArgument.IsPropertyOrXmlPropertyAccess()) Then
					boundExpression = result.First.SetAccessKind(PropertyAccessKind.[Get]).MakeRValue()
					second = result.Second.SetAccessKind(If(originalArgument.IsPropertyReturnsByRef(), PropertyAccessKind.[Get], PropertyAccessKind.[Set]))
				ElseIf (Not originalArgument.IsLateBound()) Then
					boundExpression = result.First.MakeRValue()
					second = result.Second
				Else
					boundExpression = result.First.SetLateBoundAccessKind(LateBoundAccessKind.[Get])
					second = result.Second.SetLateBoundAccessKind(LateBoundAccessKind.[Set])
				End If
				Me.AddPlaceholderReplacement(argument.InPlaceholder, Me.VisitExpressionNode(boundExpression))
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitAndGenerateObjectCloneIfNeeded(argument.InConversion, False)
				Me.RemovePlaceholderReplacement(argument.InPlaceholder)
				Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, argument.Type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
				tempsArray.Add(synthesizedLocal)
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(argument.Syntax, synthesizedLocal, synthesizedLocal.Type)
				Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(argument.Syntax, boundLocal, boundExpression2, True, argument.Type, False)
				Me.AddPlaceholderReplacement(argument.OutPlaceholder, boundLocal.MakeRValue())
				If (originalArgument.IsLateBound()) Then
					Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(argument.OutConversion)
					Me.RemovePlaceholderReplacement(argument.OutPlaceholder)
					If (second.Kind <> BoundKind.LateMemberAccess) Then
						Dim boundLateInvocation As Microsoft.CodeAnalysis.VisualBasic.BoundLateInvocation = DirectCast(second, Microsoft.CodeAnalysis.VisualBasic.BoundLateInvocation)
						If (boundLateInvocation.Member.Kind <> BoundKind.LateMemberAccess) Then
							boundLateInvocation = boundLateInvocation.Update(Me.VisitExpressionNode(boundLateInvocation.Member), Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundLateInvocation.ArgumentsOpt), boundLateInvocation.ArgumentNamesOpt, boundLateInvocation.AccessKind, boundLateInvocation.MethodOrPropertyGroupOpt, boundLateInvocation.Type)
							boundExpression1 = Me.LateIndexSet(boundLateInvocation.Syntax, boundLateInvocation, boundExpression3, True)
						Else
							boundExpression1 = Me.LateSet(boundLateInvocation.Syntax, DirectCast(MyBase.VisitLateMemberAccess(DirectCast(boundLateInvocation.Member, Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess)), Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess), boundExpression3, Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundLateInvocation.ArgumentsOpt), boundLateInvocation.ArgumentNamesOpt, True)
						End If
					Else
						Dim syntax As SyntaxNode = second.Syntax
						Dim boundLateMemberAccess As Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess = DirectCast(MyBase.VisitLateMemberAccess(DirectCast(second, Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess)), Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess)
						Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)()
						Dim strs As ImmutableArray(Of String) = New ImmutableArray(Of String)()
						boundExpression1 = Me.LateSet(syntax, boundLateMemberAccess, boundExpression3, boundExpressions, strs, True)
					End If
				Else
					boundExpression1 = DirectCast(Me.VisitAssignmentOperator(New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(argument.Syntax, second, argument.OutConversion, False, False)), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
					Me.RemovePlaceholderReplacement(argument.OutPlaceholder)
				End If
				copyBackArray.Add(boundExpression1)
				boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(argument.Syntax, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundAssignmentOperator), boundLocal, argument.Type, False)
			Else
				If (originalArgument.IsPropertyOrXmlPropertyAccess()) Then
					originalArgument = originalArgument.SetAccessKind(PropertyAccessKind.[Get])
				ElseIf (originalArgument.IsLateBound()) Then
					originalArgument = originalArgument.SetLateBoundAccessKind(LateBoundAccessKind.[Get])
				End If
				If (originalArgument.IsLValue) Then
					originalArgument = originalArgument.MakeRValue()
				End If
				Me.AddPlaceholderReplacement(argument.InPlaceholder, Me.VisitExpressionNode(originalArgument))
				Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(argument.InConversion)
				Me.RemovePlaceholderReplacement(argument.InPlaceholder)
				boundSequence = boundExpression4
			End If
			Return boundSequence
		End Function

		Private Function RewriteCallArguments(ByVal arguments As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression), ByVal parameters As ImmutableArray(Of ParameterSymbol), <Out> ByRef temporaries As ImmutableArray(Of SynthesizedLocal), <Out> ByRef copyBack As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression), ByVal suppressObjectClone As Boolean) As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			temporaries = New ImmutableArray(Of SynthesizedLocal)()
			copyBack = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)()
			If (Not arguments.IsEmpty) Then
				Dim synthesizedLocals As ArrayBuilder(Of SynthesizedLocal) = Nothing
				Dim boundExpressions1 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Nothing
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
				Dim flag As Boolean = False
				Dim num As Integer = 0
				Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Enumerator = arguments.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = enumerator.Current
					If (current.Kind <> BoundKind.ByRefArgumentWithCopyBack) Then
						boundExpression = Me.VisitExpressionNode(current)
						If (parameters(num).IsByRef AndAlso Not current.IsLValue AndAlso Not Me._inExpressionLambda) Then
							boundExpression = Me.PassArgAsTempClone(current, boundExpression, synthesizedLocals)
						End If
					Else
						boundExpression = Me.RewriteByRefArgumentWithCopyBack(DirectCast(current, BoundByRefArgumentWithCopyBack), synthesizedLocals, boundExpressions1)
					End If
					If (Not suppressObjectClone AndAlso (Not parameters(num).IsByRef OrElse Not boundExpression.IsLValue)) Then
						boundExpression = Me.GenerateObjectCloneIfNeeded(current, boundExpression)
					End If
					If (boundExpression <> current) Then
						flag = True
					End If
					instance.Add(boundExpression)
					num = num + 1
				End While
				If (flag) Then
					arguments = instance.ToImmutable()
				End If
				instance.Free()
				If (synthesizedLocals IsNot Nothing) Then
					temporaries = synthesizedLocals.ToImmutableAndFree()
				End If
				If (boundExpressions1 IsNot Nothing) Then
					boundExpressions1.ReverseContents()
					copyBack = boundExpressions1.ToImmutableAndFree()
				End If
				boundExpressions = arguments
			Else
				boundExpressions = arguments
			End If
			Return boundExpressions
		End Function

		Private Function RewriteCaseBlocksRecursive(ByVal selectStatement As BoundSelectStatement, ByVal generateUnstructuredExceptionHandlingResumeCode As Boolean, ByVal caseBlocks As ImmutableArray(Of BoundCaseBlock), ByVal startFrom As Integer, ByRef lazyConditionalBranchLocal As LocalSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundStatement2 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			If (startFrom <> caseBlocks.Length) Then
				Dim item As BoundCaseBlock = caseBlocks(startFrom)
				Dim boundStatements As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)()
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.RewriteCaseStatement(generateUnstructuredExceptionHandlingResumeCode, item.CaseStatement, boundStatements)
				Dim block As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.VisitBlock(item.Body), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
				If (generateUnstructuredExceptionHandlingResumeCode AndAlso startFrom < caseBlocks.Length - 1) Then
					block = LocalRewriter.AppendToBlock(block, Me.RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(item.Syntax))
				End If
				Dim instrument As Boolean = Me(selectStatement)
				If (boundExpression IsNot Nothing) Then
					If (instrument) Then
						boundExpression = Me._instrumenterOpt.InstrumentSelectStatementCaseCondition(selectStatement, boundExpression, Me._currentMethodOrLambda, lazyConditionalBranchLocal)
					End If
					Dim syntax As SyntaxNode = item.Syntax
					Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundExpression
					Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = block
					Dim boundStatement3 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.RewriteCaseBlocksRecursive(selectStatement, generateUnstructuredExceptionHandlingResumeCode, caseBlocks, startFrom + 1, lazyConditionalBranchLocal)
					If (instrument) Then
						boundStatement2 = item
					Else
						boundStatement2 = Nothing
					End If
					boundStatement1 = Me.RewriteIfStatement(syntax, boundExpression1, boundBlock, boundStatement3, boundStatement2, boundStatements)
				ElseIf (Not instrument) Then
					boundStatement1 = block
				Else
					boundStatement1 = Me._instrumenterOpt.InstrumentCaseElseBlock(item, block)
				End If
				boundStatement = boundStatement1
			Else
				boundStatement = Nothing
			End If
			Return boundStatement
		End Function

		Private Function RewriteCaseStatement(ByVal generateUnstructuredExceptionHandlingResumeCode As Boolean, ByVal node As BoundCaseStatement, <Out> ByRef unstructuredExceptionHandlingResumeTarget As ImmutableArray(Of BoundStatement)) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (Not System.Linq.ImmutableArrayExtensions.Any(Of BoundCaseClause)(node.CaseClauses)) Then
				unstructuredExceptionHandlingResumeTarget = New ImmutableArray(Of BoundStatement)()
				boundExpression = Nothing
			Else
				unstructuredExceptionHandlingResumeTarget = If(generateUnstructuredExceptionHandlingResumeCode, Me.RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, True), New ImmutableArray(Of BoundStatement)())
				boundExpression = Me.VisitExpressionNode(node.ConditionOpt)
			End If
			Return boundExpression
		End Function

		Public Function RewriteCollectionInitializerExpression(ByVal node As BoundCollectionInitializerExpression, ByVal objectCreationExpression As BoundExpression, ByVal rewrittenObjectCreationExpression As BoundExpression) As BoundNode
			Dim boundSequence As BoundNode
			Dim synthesizedLocal As LocalSymbol
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal
			Dim boundWithLValueExpressionPlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundWithLValueExpressionPlaceholder
			Dim type As TypeSymbol = node.Type
			Dim syntax As SyntaxNode = node.Syntax
			Dim instance As ArrayBuilder(Of BoundExpression) = ArrayBuilder(Of BoundExpression).GetInstance()
			If (Not Me._inExpressionLambda) Then
				synthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
				boundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, synthesizedLocal, type)
				Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(syntax, boundLocal, Me.GenerateObjectCloneIfNeeded(objectCreationExpression, rewrittenObjectCreationExpression), True, type, False)
				instance.Add(boundAssignmentOperator)
				boundWithLValueExpressionPlaceholder = Nothing
				Me.AddPlaceholderReplacement(node.PlaceholderOpt, boundLocal)
			Else
				synthesizedLocal = Nothing
				boundLocal = Nothing
				boundWithLValueExpressionPlaceholder = New Microsoft.CodeAnalysis.VisualBasic.BoundWithLValueExpressionPlaceholder(node.PlaceholderOpt.Syntax, node.PlaceholderOpt.Type)
				Me.AddPlaceholderReplacement(node.PlaceholderOpt, boundWithLValueExpressionPlaceholder)
			End If
			Dim length As Integer = node.Initializers.Length - 1
			Dim num As Integer = 0
			Do
				Dim item As BoundExpression = node.Initializers(num)
				If (Not Me.IsOmittedBoundCall(item)) Then
					instance.Add(Me.VisitExpressionNode(item))
				End If
				num = num + 1
			Loop While num <= length
			Me.RemovePlaceholderReplacement(node.PlaceholderOpt)
			If (Not Me._inExpressionLambda) Then
				boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(syntax, ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal), instance.ToImmutableAndFree(), boundLocal.MakeRValue(), type, False)
			Else
				boundSequence = Me.ReplaceObjectOrCollectionInitializer(rewrittenObjectCreationExpression, node.Update(boundWithLValueExpressionPlaceholder, instance.ToImmutableAndFree(), node.Type))
			End If
			Return boundSequence
		End Function

		Private Function RewriteConcatenateOperator(ByVal node As BoundBinaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim syntax As SyntaxNode = node.Syntax
			Dim left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Left
			Dim right As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Right
			Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = New Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory(Me._topMethod, Me._currentMethodOrLambda, syntax, Me._compilationState, Me._diagnostics)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.TryFoldTwoConcatOperands(syntheticBoundNodeFactory, left, right)
			If (boundExpression1 Is Nothing) Then
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
				Dim boundExpressions As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
				Me.FlattenConcatArg(left, instance)
				Me.FlattenConcatArg(right, boundExpressions)
				If (instance.Any() AndAlso boundExpressions.Any()) Then
					boundExpression1 = Me.TryFoldTwoConcatOperands(syntheticBoundNodeFactory, instance.Last(), boundExpressions.First())
					If (boundExpression1 IsNot Nothing) Then
						boundExpressions(0) = boundExpression1
						instance.RemoveLast()
					End If
				End If
				instance.AddRange(boundExpressions)
				boundExpressions.Free()
				Select Case instance.Count
					Case 0
						instance.Free()
						boundExpression = syntheticBoundNodeFactory.StringLiteral(ConstantValue.Create(""))
						Exit Select
					Case 1
						Dim item As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = instance(0)
						instance.Free()
						boundExpression = item
						Exit Select
					Case 2
						Dim item1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = instance(0)
						Dim item2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = instance(1)
						instance.Free()
						boundExpression = Me.RewriteStringConcatenationTwoExprs(node, syntheticBoundNodeFactory, item1, item2)
						Exit Select
					Case 3
						Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = instance(0)
						Dim item3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = instance(1)
						Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = instance(2)
						instance.Free()
						boundExpression = Me.RewriteStringConcatenationThreeExprs(node, syntheticBoundNodeFactory, boundExpression2, item3, boundExpression3)
						Exit Select
					Case 4
						Dim item4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = instance(0)
						Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = instance(1)
						Dim item5 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = instance(2)
						Dim boundExpression5 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = instance(3)
						instance.Free()
						boundExpression = Me.RewriteStringConcatenationFourExprs(node, syntheticBoundNodeFactory, item4, boundExpression4, item5, boundExpression5)
						Exit Select
					Case Else
						boundExpression = Me.RewriteStringConcatenationManyExprs(node, syntheticBoundNodeFactory, instance.ToImmutableAndFree())
						Exit Select
				End Select
			Else
				boundExpression = boundExpression1
			End If
			Return boundExpression
		End Function

		Private Function RewriteConstant(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal constantValue As Microsoft.CodeAnalysis.ConstantValue) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (Not Me._inExpressionLambda AndAlso Not node.HasErrors) Then
				If (constantValue.Discriminator <> ConstantValueTypeDiscriminator.[Decimal]) Then
					If (constantValue.Discriminator <> ConstantValueTypeDiscriminator.DateTime) Then
						If (node.Kind = BoundKind.Literal) Then
							boundLiteral = node
						Else
							boundLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(node.Syntax, constantValue, node.Type, constantValue.IsBad)
						End If
						boundExpression = boundLiteral
						Return boundExpression
					End If
					boundExpression = LocalRewriter.RewriteDateConstant(node, constantValue, Me._topMethod, Me._diagnostics)
					Return boundExpression
				Else
					boundExpression = LocalRewriter.RewriteDecimalConstant(node, constantValue, Me._topMethod, Me._diagnostics)
					Return boundExpression
				End If
			End If
			If (node.Kind = BoundKind.Literal) Then
				boundLiteral = node
			Else
				boundLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(node.Syntax, constantValue, node.Type, constantValue.IsBad)
			End If
			boundExpression = boundLiteral
			Return boundExpression
		End Function

		Private Function RewriteDateComparisonOperator(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (Not Me._inExpressionLambda) Then
				Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node
				Dim left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Left
				Dim right As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Right
				If (left.Type.IsDateTimeType() AndAlso right.Type.IsDateTimeType()) Then
					Dim specialTypeMember As MethodSymbol = DirectCast(Me.ContainingAssembly.GetSpecialTypeMember(SpecialMember.System_DateTime__CompareDateTimeDateTime), MethodSymbol)
					If (Not Me.ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_DateTime__CompareDateTimeDateTime, specialTypeMember)) Then
						Dim syntax As SyntaxNode = node.Syntax
						Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(left, right)
						Dim returnType As TypeSymbol = specialTypeMember.ReturnType
						Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
						Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, specialTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)
						boundBinaryOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator(node.Syntax, node.OperatorKind And BinaryOperatorKind.OpMask, boundCall, New BoundLiteral(node.Syntax, ConstantValue.Create(0), specialTypeMember.ReturnType), False, node.Type, False)
					End If
				End If
				boundExpression = boundBinaryOperator
			Else
				boundExpression = node
			End If
			Return boundExpression
		End Function

		Private Shared Function RewriteDateConstant(ByVal node As BoundExpression, ByVal nodeValue As Microsoft.CodeAnalysis.ConstantValue, ByVal currentMethod As MethodSymbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As BoundExpression
			Dim boundObjectCreationExpression As BoundExpression
			Dim specialTypeMember As MethodSymbol
			Dim parameters As ImmutableArray(Of ParameterSymbol)
			Dim bitVector As Microsoft.CodeAnalysis.BitVector
			Dim syntax As Microsoft.CodeAnalysis.SyntaxNode
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
			Dim boundLiteral As BoundExpression()
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim boundExpressions As ImmutableArray(Of BoundExpression)
			Dim type As TypeSymbol
			Dim containingAssembly As AssemblySymbol = currentMethod.ContainingAssembly
			Dim dateTimeValue As DateTime = nodeValue.DateTimeValue
			If (DateTime.Compare(dateTimeValue, DateTime.MinValue) = 0 AndAlso (currentMethod.MethodKind <> MethodKind.StaticConstructor OrElse currentMethod.ContainingType.SpecialType <> SpecialType.System_DateTime)) Then
				Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = DirectCast(containingAssembly.GetSpecialTypeMember(SpecialMember.System_DateTime__MinValue), Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
				If (fieldSymbol IsNot Nothing) Then
					Dim useSiteInfoForMemberAndContainingType As UseSiteInfo(Of AssemblySymbol) = Binder.GetUseSiteInfoForMemberAndContainingType(fieldSymbol)
					If (useSiteInfoForMemberAndContainingType.DiagnosticInfo IsNot Nothing) Then
						specialTypeMember = DirectCast(containingAssembly.GetSpecialTypeMember(SpecialMember.System_DateTime__CtorInt64), MethodSymbol)
						If (LocalRewriter.ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_DateTime__CtorInt64, specialTypeMember, diagnostics, False)) Then
							boundObjectCreationExpression = node
						Else
							syntaxNode = node.Syntax
							ReDim boundLiteral(0)
							syntax = node.Syntax
							constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(dateTimeValue.Ticks)
							parameters = specialTypeMember.Parameters
							boundLiteral(0) = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntax, constantValue, parameters(0).Type)
							boundExpressions = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(boundLiteral)
							type = node.Type
							bitVector = New Microsoft.CodeAnalysis.BitVector()
							boundObjectCreationExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression(syntaxNode, specialTypeMember, boundExpressions, Nothing, type, False, bitVector)
						End If
						Return boundObjectCreationExpression
					End If
					diagnostics.AddDependencies(useSiteInfoForMemberAndContainingType)
					boundObjectCreationExpression = New BoundFieldAccess(node.Syntax, Nothing, fieldSymbol, False, fieldSymbol.Type, False)
					Return boundObjectCreationExpression
				End If
			End If
			specialTypeMember = DirectCast(containingAssembly.GetSpecialTypeMember(SpecialMember.System_DateTime__CtorInt64), MethodSymbol)
			If (LocalRewriter.ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_DateTime__CtorInt64, specialTypeMember, diagnostics, False)) Then
				boundObjectCreationExpression = node
			Else
				syntaxNode = node.Syntax
				ReDim boundLiteral(0)
				syntax = node.Syntax
				constantValue = Microsoft.CodeAnalysis.ConstantValue.Create(dateTimeValue.Ticks)
				parameters = specialTypeMember.Parameters
				boundLiteral(0) = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntax, constantValue, parameters(0).Type)
				boundExpressions = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(boundLiteral)
				type = node.Type
				bitVector = New Microsoft.CodeAnalysis.BitVector()
				boundObjectCreationExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression(syntaxNode, specialTypeMember, boundExpressions, Nothing, type, False, bitVector)
			End If
			Return boundObjectCreationExpression
		End Function

		Private Function RewriteDecimalBinaryOperator(ByVal node As BoundBinaryOperator, ByVal member As SpecialMember) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (Not Me._inExpressionLambda) Then
				Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node
				Dim left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Left
				Dim right As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Right
				If (left.Type.IsDecimalType() AndAlso right.Type.IsDecimalType()) Then
					Dim specialTypeMember As MethodSymbol = DirectCast(Me.ContainingAssembly.GetSpecialTypeMember(member), MethodSymbol)
					If (Not Me.ReportMissingOrBadRuntimeHelper(node, member, specialTypeMember)) Then
						Dim syntax As SyntaxNode = node.Syntax
						Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(left, right)
						Dim returnType As TypeSymbol = specialTypeMember.ReturnType
						Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
						boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, specialTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)
					End If
				End If
				boundExpression = boundCall
			Else
				boundExpression = node
			End If
			Return boundExpression
		End Function

		Private Function RewriteDecimalComparisonOperator(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (Not Me._inExpressionLambda) Then
				Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node
				Dim left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Left
				Dim right As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Right
				If (left.Type.IsDecimalType() AndAlso right.Type.IsDecimalType()) Then
					Dim specialTypeMember As MethodSymbol = DirectCast(Me.ContainingAssembly.GetSpecialTypeMember(SpecialMember.System_Decimal__CompareDecimalDecimal), MethodSymbol)
					If (Not Me.ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_Decimal__CompareDecimalDecimal, specialTypeMember)) Then
						Dim syntax As SyntaxNode = node.Syntax
						Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(left, right)
						Dim returnType As TypeSymbol = specialTypeMember.ReturnType
						Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
						Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, specialTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)
						boundBinaryOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator(node.Syntax, node.OperatorKind And BinaryOperatorKind.OpMask, boundCall, New BoundLiteral(node.Syntax, ConstantValue.Create(0), specialTypeMember.ReturnType), False, node.Type, False)
					End If
				End If
				boundExpression = boundBinaryOperator
			Else
				boundExpression = node
			End If
			Return boundExpression
		End Function

		Private Shared Function RewriteDecimalConstant(ByVal node As BoundExpression, ByVal nodeValue As Microsoft.CodeAnalysis.ConstantValue, ByVal currentMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As BoundExpression
			Dim boundFieldAccess As BoundExpression
			Dim flag As Boolean
			Dim num As Byte
			Dim num1 As UInteger
			Dim num2 As UInteger
			Dim num3 As UInteger
			Dim parameters As ImmutableArray(Of ParameterSymbol)
			Dim bitVector As Microsoft.CodeAnalysis.BitVector
			Dim containingAssembly As AssemblySymbol = currentMethod.ContainingAssembly
			nodeValue.DecimalValue.GetBits(flag, num, num1, num2, num3)
			If (num = 0 AndAlso CULng(num3) = 0 AndAlso CULng(num2) = 0) Then
				If (currentMethod.MethodKind <> MethodKind.StaticConstructor OrElse currentMethod.ContainingType.SpecialType <> SpecialType.System_Decimal) Then
					Dim specialTypeMember As Symbol = Nothing
					If (CULng(num1) = 0) Then
						specialTypeMember = containingAssembly.GetSpecialTypeMember(SpecialMember.System_Decimal__Zero)
					ElseIf (CULng(num1) = CLng(1)) Then
						specialTypeMember = If(Not flag, containingAssembly.GetSpecialTypeMember(SpecialMember.System_Decimal__One), containingAssembly.GetSpecialTypeMember(SpecialMember.System_Decimal__MinusOne))
					End If
					If (specialTypeMember IsNot Nothing) Then
						Dim useSiteInfoForMemberAndContainingType As UseSiteInfo(Of AssemblySymbol) = Binder.GetUseSiteInfoForMemberAndContainingType(specialTypeMember)
						If (useSiteInfoForMemberAndContainingType.DiagnosticInfo IsNot Nothing) Then
							GoTo Label1
						End If
						Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = DirectCast(specialTypeMember, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
						diagnostics.AddDependencies(useSiteInfoForMemberAndContainingType)
						boundFieldAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(node.Syntax, Nothing, fieldSymbol, False, fieldSymbol.Type, False)
						Return boundFieldAccess
					End If
				End If
			Label1:
				Dim num4 As Long = CLng(num1)
				If (flag) Then
					num4 = -num4
				End If
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(containingAssembly.GetSpecialTypeMember(SpecialMember.System_Decimal__CtorInt64), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
				If (methodSymbol IsNot Nothing) Then
					Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = Binder.GetUseSiteInfoForMemberAndContainingType(methodSymbol)
					If (useSiteInfo.DiagnosticInfo IsNot Nothing) Then
						GoTo Label2
					End If
					diagnostics.AddDependencies(useSiteInfo)
					Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
					Dim boundLiteral(0) As BoundExpression
					Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
					Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = Microsoft.CodeAnalysis.ConstantValue.Create(num4)
					parameters = methodSymbol.Parameters
					boundLiteral(0) = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntaxNode, constantValue, parameters(0).Type)
					Dim boundExpressions As ImmutableArray(Of BoundExpression) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(boundLiteral)
					Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = node.Type
					bitVector = New Microsoft.CodeAnalysis.BitVector()
					boundFieldAccess = New BoundObjectCreationExpression(syntax, methodSymbol, boundExpressions, Nothing, type, False, bitVector)
					Return boundFieldAccess
				End If
			End If
		Label2:
			Dim specialTypeMember1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			specialTypeMember1 = DirectCast(containingAssembly.GetSpecialTypeMember(SpecialMember.System_Decimal__CtorInt32Int32Int32BooleanByte), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			If (LocalRewriter.ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_Decimal__CtorInt32Int32Int32BooleanByte, specialTypeMember1, diagnostics, False)) Then
				boundFieldAccess = node
			Else
				Dim syntax1 As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim boundExpressionArray(4) As BoundExpression
				Dim syntaxNode1 As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim constantValue1 As Microsoft.CodeAnalysis.ConstantValue = Microsoft.CodeAnalysis.ConstantValue.Create(CompileTimeCalculations.UncheckedCInt(num1))
				parameters = specialTypeMember1.Parameters
				boundExpressionArray(0) = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntaxNode1, constantValue1, parameters(0).Type)
				Dim syntax2 As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim constantValue2 As Microsoft.CodeAnalysis.ConstantValue = Microsoft.CodeAnalysis.ConstantValue.Create(CompileTimeCalculations.UncheckedCInt(num2))
				parameters = specialTypeMember1.Parameters
				boundExpressionArray(1) = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntax2, constantValue2, parameters(1).Type)
				Dim syntaxNode2 As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim constantValue3 As Microsoft.CodeAnalysis.ConstantValue = Microsoft.CodeAnalysis.ConstantValue.Create(CompileTimeCalculations.UncheckedCInt(num3))
				parameters = specialTypeMember1.Parameters
				boundExpressionArray(2) = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntaxNode2, constantValue3, parameters(2).Type)
				Dim syntax3 As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim constantValue4 As Microsoft.CodeAnalysis.ConstantValue = Microsoft.CodeAnalysis.ConstantValue.Create(flag)
				parameters = specialTypeMember1.Parameters
				boundExpressionArray(3) = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntax3, constantValue4, parameters(3).Type)
				Dim syntaxNode3 As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim constantValue5 As Microsoft.CodeAnalysis.ConstantValue = Microsoft.CodeAnalysis.ConstantValue.Create(num)
				parameters = specialTypeMember1.Parameters
				boundExpressionArray(4) = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(syntaxNode3, constantValue5, parameters(4).Type)
				Dim boundExpressions1 As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(boundExpressionArray)
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = node.Type
				bitVector = New Microsoft.CodeAnalysis.BitVector()
				boundFieldAccess = New BoundObjectCreationExpression(syntax1, specialTypeMember1, boundExpressions1, Nothing, typeSymbol, False, bitVector)
			End If
			Return boundFieldAccess
		End Function

		Private Function RewriteDecimalToNumericOrBooleanConversion(ByVal node As BoundConversion, ByVal typeFrom As TypeSymbol, ByVal underlyingTypeTo As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim wellKnownMember As Microsoft.CodeAnalysis.WellKnownMember
			Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node
			Select Case underlyingTypeTo.SpecialType
				Case SpecialType.System_Boolean
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToBooleanDecimal
					Exit Select
				Case SpecialType.System_Char
				Case SpecialType.System_Decimal
					boundExpression = boundCall
					Return boundExpression
				Case SpecialType.System_SByte
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToSByteDecimal
					Exit Select
				Case SpecialType.System_Byte
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToByteDecimal
					Exit Select
				Case SpecialType.System_Int16
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToInt16Decimal
					Exit Select
				Case SpecialType.System_UInt16
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToUInt16Decimal
					Exit Select
				Case SpecialType.System_Int32
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToInt32Decimal
					Exit Select
				Case SpecialType.System_UInt32
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToUInt32Decimal
					Exit Select
				Case SpecialType.System_Int64
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToInt64Decimal
					Exit Select
				Case SpecialType.System_UInt64
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToUInt64Decimal
					Exit Select
				Case SpecialType.System_Single
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToSingleDecimal
					Exit Select
				Case SpecialType.System_Double
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Convert__ToDoubleDecimal
					Exit Select
				Case Else
					boundExpression = boundCall
					Return boundExpression
			End Select
			Dim wellKnownTypeMember As MethodSymbol = DirectCast(Me.Compilation.GetWellKnownTypeMember(wellKnownMember), MethodSymbol)
			If (Not Me.ReportMissingOrBadRuntimeHelper(node, wellKnownMember, wellKnownTypeMember)) Then
				Dim operand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Operand
				Dim syntax As SyntaxNode = node.Syntax
				Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(operand)
				Dim returnType As TypeSymbol = wellKnownTypeMember.ReturnType
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, wellKnownTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)
				Dim type As TypeSymbol = node.Type
				If (CObj(type) <> CObj(wellKnownTypeMember.ReturnType)) Then
					Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Narrowing] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Numeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions
					boundCall = New BoundConversion(node.Syntax, boundCall, conversionKind, node.Checked, node.ExplicitCastInCode, type, False)
				End If
			End If
			boundExpression = boundCall
			Return boundExpression
		End Function

		Private Function RewriteDecimalUnaryOperator(ByVal node As BoundUnaryOperator) As BoundExpression
			Dim boundCall As BoundExpression = node
			If ((node.OperatorKind And UnaryOperatorKind.[Not]) <> UnaryOperatorKind.Plus) Then
				Dim specialTypeMember As MethodSymbol = DirectCast(Me.ContainingAssembly.GetSpecialTypeMember(SpecialMember.System_Decimal__NegateDecimal), MethodSymbol)
				If (Not Me.ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_Decimal__NegateDecimal, specialTypeMember)) Then
					Dim syntax As SyntaxNode = node.Syntax
					Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(node.Operand)
					Dim returnType As TypeSymbol = specialTypeMember.ReturnType
					Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
					boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, specialTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)
				End If
			Else
				boundCall = node.Operand
			End If
			Return boundCall
		End Function

		Public Shared Function RewriteExpressionTree(ByVal node As BoundExpression, ByVal method As MethodSymbol, ByVal compilationState As TypeCompilationState, ByVal previousSubmissionFields As SynthesizedSubmissionFields, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal rewrittenNodes As HashSet(Of BoundNode), ByVal recursionDepth As Integer) As BoundExpression
			Dim flag As Boolean = False
			Dim symbols As ISet(Of Symbol) = SpecializedCollections.EmptySet(Of Symbol)()
			Return DirectCast(LocalRewriter.RewriteNode(node, method, method, compilationState, previousSubmissionFields, diagnostics, rewrittenNodes, flag, symbols, LocalRewriter.RewritingFlags.[Default], Nothing, recursionDepth), BoundExpression)
		End Function

		Private Function RewriteFinallyBlock(ByVal tryStatement As BoundTryStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim finallyBlockOpt As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = tryStatement.FinallyBlockOpt
			If (finallyBlockOpt IsNot Nothing) Then
				Dim boundBlock1 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.Visit(finallyBlockOpt), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
				If (Me(tryStatement) AndAlso TypeOf finallyBlockOpt.Syntax Is FinallyBlockSyntax) Then
					boundBlock1 = LocalRewriter.PrependWithPrologue(boundBlock1, Me._instrumenterOpt.CreateFinallyBlockPrologue(tryStatement))
				End If
				boundBlock = boundBlock1
			Else
				boundBlock = finallyBlockOpt
			End If
			Return boundBlock
		End Function

		Private Function RewriteFloatingToIntegralConversion(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundConversion, ByVal typeFrom As TypeSymbol, ByVal underlyingTypeTo As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node
			Dim operand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Operand
			If (operand.Kind = BoundKind.[Call]) Then
				Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = DirectCast(operand, Microsoft.CodeAnalysis.VisualBasic.BoundCall)
				If (Not Me.IsFloatingTruncation(boundCall)) Then
					If (Not Me.ReturnsWholeNumberDouble(boundCall)) Then
						GoTo Label1
					End If
					boundConversion = node
					Return boundConversion
				Else
					Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
					Dim arguments As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = boundCall.Arguments
					boundConversion = New Microsoft.CodeAnalysis.VisualBasic.BoundConversion(syntax, arguments(0), node.ConversionKind, node.Checked, node.ExplicitCastInCode, node.Type, False)
					Return boundConversion
				End If
			End If
		Label1:
			Dim wellKnownTypeMember As MethodSymbol = DirectCast(Me.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Math__RoundDouble), MethodSymbol)
			If (Not Me.ReportMissingOrBadRuntimeHelper(node, WellKnownMember.System_Math__RoundDouble, wellKnownTypeMember)) Then
				Dim parameters As ImmutableArray(Of ParameterSymbol) = wellKnownTypeMember.Parameters
				If (CObj(typeFrom) <> CObj(parameters(0).Type)) Then
					Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
					Dim checked As Boolean = node.Checked
					Dim explicitCastInCode As Boolean = node.ExplicitCastInCode
					parameters = wellKnownTypeMember.Parameters
					operand = New Microsoft.CodeAnalysis.VisualBasic.BoundConversion(syntaxNode, operand, ConversionKind.WideningNumeric, checked, explicitCastInCode, parameters(0).Type, False)
				End If
				Dim syntax1 As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(operand)
				Dim returnType As TypeSymbol = wellKnownTypeMember.ReturnType
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundCall1 As Microsoft.CodeAnalysis.VisualBasic.BoundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax1, wellKnownTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)
				boundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundConversion(node.Syntax, boundCall1, node.ConversionKind, node.Checked, node.ExplicitCastInCode, node.Type, False)
			End If
			boundConversion = boundExpression
			Return boundConversion
		End Function

		Private Sub RewriteForEachArrayOrString(ByVal node As BoundForEachStatement, ByVal statements As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement), ByVal locals As ArrayBuilder(Of LocalSymbol), ByVal isArray As Boolean, ByVal collectionExpression As BoundExpression)
			Dim boundBadExpression As BoundExpression
			Dim specialType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim bitVector As Microsoft.CodeAnalysis.BitVector
			Dim syntax As ForEachBlockSyntax = DirectCast(node.Syntax, ForEachBlockSyntax)
			Dim flag As Boolean = Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)
			Dim boundStatements As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)()
			If (flag) Then
				boundStatements = Me.RegisterUnstructuredExceptionHandlingResumeTarget(syntax, True)
			End If
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = node.ControlVariable.Type
			Dim enumeratorInfo As ForEachEnumeratorInfo = node.EnumeratorInfo
			If (collectionExpression.Kind = BoundKind.Conversion) Then
				Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundConversion = DirectCast(collectionExpression, Microsoft.CodeAnalysis.VisualBasic.BoundConversion)
				If (Not boundConversion.ExplicitCastInCode AndAlso boundConversion.Operand.Type.IsArrayType()) Then
					collectionExpression = boundConversion.Operand
				End If
			End If
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = collectionExpression.Type
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Nothing
			Dim boundStatementList As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.CreateLocalAndAssignment(syntax.ForEachStatement, collectionExpression.MakeRValue(), boundLocal, locals, SynthesizedLocalKind.ForEachArray)
			If (Not boundStatements.IsDefaultOrEmpty) Then
				boundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(boundStatementList.Syntax, boundStatements.Add(boundStatementList), False)
			End If
			If (Me(node)) Then
				boundStatementList = Me._instrumenterOpt.InstrumentForEachLoopInitialization(node, boundStatementList)
			End If
			statements.Add(boundStatementList)
			Dim boundLocal1 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Nothing
			Dim specialTypeWithUseSiteDiagnostics As NamedTypeSymbol = Me.GetSpecialTypeWithUseSiteDiagnostics(Microsoft.CodeAnalysis.SpecialType.System_Int32, syntax)
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.CreateLocalAndAssignment(syntax.ForEachStatement, New BoundLiteral(syntax, ConstantValue.[Default](Microsoft.CodeAnalysis.SpecialType.System_Int32), specialTypeWithUseSiteDiagnostics), boundLocal1, locals, SynthesizedLocalKind.ForEachArrayIndex)
			statements.Add(boundStatement)
			Dim boundCall As BoundExpression = Nothing
			If (Not isArray) Then
				Dim specialTypeMember As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(Me.GetSpecialTypeMember(SpecialMember.System_String__Length), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
				Dim empty As ImmutableArray(Of !0) = ImmutableArray(Of BoundExpression).Empty
				bitVector = New Microsoft.CodeAnalysis.BitVector()
				boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, specialTypeMember, Nothing, boundLocal, empty, Nothing, specialTypeWithUseSiteDiagnostics, False, False, bitVector)
			Else
				boundCall = New BoundArrayLength(syntax, boundLocal, specialTypeWithUseSiteDiagnostics, False)
			End If
			If (Not isArray) Then
				specialType = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Char)
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
				If (Not Me.TryGetSpecialMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, SpecialMember.System_String__Chars, syntax)) Then
					boundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(syntax, LookupResultKind.NotReferencable, ImmutableArray(Of Symbol).Empty, ImmutableArray.Create(Of BoundExpression)(boundLocal1.MakeRValue()), specialType, True)
				Else
					Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(boundLocal1.MakeRValue())
					bitVector = New Microsoft.CodeAnalysis.BitVector()
					boundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, methodSymbol, Nothing, boundLocal, boundExpressions, Nothing, specialType, False, False, bitVector)
				End If
			Else
				specialType = DirectCast(typeSymbol, ArrayTypeSymbol).ElementType
				boundBadExpression = New BoundArrayAccess(syntax, boundLocal.MakeRValue(), ImmutableArray.Create(Of BoundExpression)(boundLocal1.MakeRValue()), False, specialType, False)
			End If
			If (enumeratorInfo.CurrentPlaceholder IsNot Nothing) Then
				Me.AddPlaceholderReplacement(enumeratorInfo.CurrentPlaceholder, boundBadExpression)
			End If
			Dim statement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = (New BoundAssignmentOperator(syntax, node.ControlVariable, enumeratorInfo.CurrentConversion, False, node.ControlVariable.Type, False)).ToStatement()
			statement.SetWasCompilerGenerated()
			statement = DirectCast(Me.Visit(statement), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Dim boundStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.CreateIndexIncrement(syntax, boundLocal1)
			Dim boundStatementList1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatementList = Me.CreateLoweredWhileStatements(node, boundCall, boundLocal1, statement, boundStatement1, flag)
			statements.AddRange(boundStatementList1.Statements)
			If (enumeratorInfo.CurrentPlaceholder IsNot Nothing) Then
				Me.RemovePlaceholderReplacement(enumeratorInfo.CurrentPlaceholder)
			End If
		End Sub

		Private Sub RewriteForEachIEnumerable(ByVal node As BoundForEachStatement, ByVal statements As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement), ByVal locals As ArrayBuilder(Of LocalSymbol))
			Dim empty As ImmutableArray(Of LocalSymbol)
			Dim syntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachBlockSyntax = DirectCast(node.Syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachBlockSyntax)
			Dim enumeratorInfo As ForEachEnumeratorInfo = node.EnumeratorInfo
			Dim flag As Boolean = If(Not enumeratorInfo.NeedToDispose, False, Not Me.InsideValidUnstructuredExceptionHandlingOnErrorContext())
			Dim unstructuredExceptionHandlingContext As LocalRewriter.UnstructuredExceptionHandlingContext = New LocalRewriter.UnstructuredExceptionHandlingContext()
			If (flag) Then
				unstructuredExceptionHandlingContext = Me.LeaveUnstructuredExceptionHandlingContext(node)
			End If
			Dim flag1 As Boolean = Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)
			Dim boundStatements As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)()
			If (flag1) Then
				boundStatements = Me.RegisterUnstructuredExceptionHandlingResumeTarget(syntax, True)
			End If
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Nothing
			Dim boundStatementList As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.CreateLocalAndAssignment(syntax.ForEachStatement, enumeratorInfo.GetEnumerator, boundLocal, locals, SynthesizedLocalKind.ForEachEnumerator)
			If (Not boundStatements.IsDefaultOrEmpty) Then
				boundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(boundStatementList.Syntax, boundStatements.Add(boundStatementList), False)
			End If
			If (Me(node)) Then
				boundStatementList = Me._instrumenterOpt.InstrumentForEachLoopInitialization(node, boundStatementList)
			End If
			Me.AddPlaceholderReplacement(enumeratorInfo.EnumeratorPlaceholder, boundLocal)
			If (enumeratorInfo.CurrentPlaceholder IsNot Nothing) Then
				Me.AddPlaceholderReplacement(enumeratorInfo.CurrentPlaceholder, Me.VisitExpressionNode(enumeratorInfo.Current))
			End If
			Dim statement As BoundExpressionStatement = (New BoundAssignmentOperator(syntax, node.ControlVariable, enumeratorInfo.CurrentConversion, False, node.ControlVariable.Type, False)).ToStatement()
			statement.SetWasCompilerGenerated()
			Dim boundStatements1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(DirectCast(Me.Visit(statement), Microsoft.CodeAnalysis.VisualBasic.BoundStatement), DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundStatement))
			Dim forEachBlockSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ForEachBlockSyntax = syntax
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
			Dim statementSyntaxes1 As SyntaxList(Of StatementSyntax) = statementSyntaxes
			If (node.DeclaredOrInferredLocalOpt IsNot Nothing) Then
				empty = ImmutableArray.Create(Of LocalSymbol)(node.DeclaredOrInferredLocalOpt)
			Else
				empty = ImmutableArray(Of LocalSymbol).Empty
			End If
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(forEachBlockSyntax, statementSyntaxes1, empty, boundStatements1, False)
			Dim boundLabelStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(syntax, node.ContinueLabel)
			If (flag1) Then
				boundLabelStatement = LocalRewriter.Concat(boundLabelStatement, New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(syntax, Me.RegisterUnstructuredExceptionHandlingResumeTarget(syntax, True), False))
			End If
			If (Me(node)) Then
				boundLabelStatement = Me._instrumenterOpt.InstrumentForEachLoopEpilogue(node, boundLabelStatement)
			End If
			boundBlock = LocalRewriter.AppendToBlock(boundBlock, boundLabelStatement)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(enumeratorInfo.MoveNext)
			Dim generatedLabelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol("MoveNextLabel")
			Dim exitLabel As LabelSymbol = node.ExitLabel
			Dim boundStatements2 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)()
			Dim boundStatementList1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatementList = DirectCast(Me.RewriteWhileStatement(node, boundExpression, boundBlock, generatedLabelSymbol, exitLabel, True, Nothing, boundStatements2, Nothing), Microsoft.CodeAnalysis.VisualBasic.BoundStatementList)
			If (enumeratorInfo.CurrentPlaceholder IsNot Nothing) Then
				Me.RemovePlaceholderReplacement(enumeratorInfo.CurrentPlaceholder)
			End If
			If (Not enumeratorInfo.NeedToDispose) Then
				statements.Add(boundStatementList)
				statements.AddRange(New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { boundStatementList1 })
			Else
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.GenerateDisposeCallForForeachAndUsing(node.Syntax, boundLocal, Me.VisitExpressionNode(enumeratorInfo.DisposeCondition), enumeratorInfo.IsOrInheritsFromOrImplementsIDisposable, Me.VisitExpressionNode(enumeratorInfo.DisposeCast))
				If (flag) Then
					statementSyntaxes = New SyntaxList(Of StatementSyntax)()
					Dim boundBlock1 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(boundStatementList, boundStatementList1), False)
					Dim u00210s As ImmutableArray(Of !0) = ImmutableArray(Of BoundCatchBlock).Empty
					statementSyntaxes = New SyntaxList(Of StatementSyntax)()
					Dim boundTryStatement As Microsoft.CodeAnalysis.VisualBasic.BoundTryStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundTryStatement(syntax, boundBlock1, u00210s, New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(boundStatement), False), Nothing, False)
					boundTryStatement.SetWasCompilerGenerated()
					statements.Add(boundTryStatement)
				Else
					statements.Add(boundStatementList)
					statements.Add(boundStatementList1)
					If (flag1) Then
						Me.RegisterUnstructuredExceptionHandlingResumeTarget(syntax, True, statements)
					End If
					statements.Add(boundStatement)
				End If
			End If
			If (flag) Then
				Me.RestoreUnstructuredExceptionHandlingContext(node, unstructuredExceptionHandlingContext)
			End If
			Me.RemovePlaceholderReplacement(enumeratorInfo.EnumeratorPlaceholder)
		End Sub

		Private Function RewriteForLoopCondition(ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal limit As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal stepValue As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal operatorsOpt As BoundForToUserDefinedOperators, ByVal positiveFlag As SynthesizedLocal) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim binaryOperatorKind As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind
			If (operatorsOpt Is Nothing) Then
				Dim specialType As NamedTypeSymbol = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Boolean)
				If (Not stepValue.Type.GetEnumUnderlyingTypeOrSelf().IsUnsignedIntegralType()) Then
					Dim constantValueOpt As ConstantValue = stepValue.ConstantValueOpt
					If (constantValueOpt IsNot Nothing) Then
						If (Not constantValueOpt.IsNegativeNumeric) Then
							If (Not constantValueOpt.IsNumeric) Then
								Throw ExceptionUtilities.UnexpectedValue(constantValueOpt)
							End If
							binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LessThanOrEqual
						Else
							binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.GreaterThanOrEqual
						End If
						boundExpression = Me.TransformRewrittenBinaryOperator(New BoundBinaryOperator(limit.Syntax, binaryOperatorKind, controlVariable.MakeRValue(), limit, False, specialType, False))
					ElseIf (Not stepValue.Type.GetEnumUnderlyingTypeOrSelf().IsSignedIntegralType()) Then
						Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
						If (controlVariable.Type.IsNullableType()) Then
							boundExpression1 = Me.MakeBooleanBinaryExpression(controlVariable.Syntax, Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[And], Me.NullableHasValue(limit), Me.NullableHasValue(controlVariable))
							controlVariable = Me.NullableValueOrDefault(controlVariable)
							limit = Me.NullableValueOrDefault(limit)
						End If
						If (positiveFlag Is Nothing) Then
							Throw ExceptionUtilities.Unreachable
						End If
						Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.TransformRewrittenBinaryOperator(New BoundBinaryOperator(limit.Syntax, Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LessThanOrEqual, controlVariable.MakeRValue(), limit, False, specialType, False))
						Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.TransformRewrittenBinaryOperator(New BoundBinaryOperator(limit.Syntax, Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.GreaterThanOrEqual, controlVariable.MakeRValue(), limit, False, specialType, False))
						Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(limit.Syntax, positiveFlag, False, positiveFlag.Type)
						Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.MakeTernaryConditionalExpression(limit.Syntax, boundLocal, boundExpression2, boundExpression3)
						If (boundExpression1 IsNot Nothing) Then
							boundExpression4 = Me.MakeBooleanBinaryExpression(boundExpression4.Syntax, Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[AndAlso], boundExpression1, boundExpression4)
						End If
						boundExpression = boundExpression4
					Else
						boundExpression = Me.TransformRewrittenBinaryOperator(New BoundBinaryOperator(stepValue.Syntax, Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LessThanOrEqual, Me.NegateIfStepNegative(controlVariable.MakeRValue(), stepValue), Me.NegateIfStepNegative(limit, stepValue), False, specialType, False))
					End If
				Else
					boundExpression = Me.TransformRewrittenBinaryOperator(New BoundBinaryOperator(limit.Syntax, Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LessThanOrEqual, controlVariable.MakeRValue(), limit, False, specialType, False))
				End If
			Else
				Me.AddPlaceholderReplacement(operatorsOpt.LeftOperandPlaceholder, controlVariable.MakeRValue())
				Me.AddPlaceholderReplacement(operatorsOpt.RightOperandPlaceholder, limit)
				Dim boundExpression5 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.MakeTernaryConditionalExpression(operatorsOpt.Syntax, New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(operatorsOpt.Syntax, positiveFlag, False, positiveFlag.Type), Me.VisitExpressionNode(operatorsOpt.LessThanOrEqual), Me.VisitExpressionNode(operatorsOpt.GreaterThanOrEqual))
				Me.RemovePlaceholderReplacement(operatorsOpt.LeftOperandPlaceholder)
				Me.RemovePlaceholderReplacement(operatorsOpt.RightOperandPlaceholder)
				boundExpression = boundExpression5
			End If
			Return boundExpression
		End Function

		Private Function RewriteForLoopIncrement(ByVal controlVariable As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal stepValue As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal isChecked As Boolean, ByVal operatorsOpt As BoundForToUserDefinedOperators) As BoundStatement
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (operatorsOpt IsNot Nothing) Then
				Me.AddPlaceholderReplacement(operatorsOpt.LeftOperandPlaceholder, controlVariable.MakeRValue())
				Me.AddPlaceholderReplacement(operatorsOpt.RightOperandPlaceholder, stepValue)
				boundExpression = Me.VisitExpressionNode(operatorsOpt.Addition)
				Me.RemovePlaceholderReplacement(operatorsOpt.LeftOperandPlaceholder)
				Me.RemovePlaceholderReplacement(operatorsOpt.RightOperandPlaceholder)
			Else
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = controlVariable
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				If (controlVariable.Type.IsNullableType()) Then
					boundExpression2 = Me.MakeBooleanBinaryExpression(controlVariable.Syntax, BinaryOperatorKind.[And], Me.NullableHasValue(stepValue), Me.NullableHasValue(controlVariable))
					boundExpression1 = Me.NullableValueOrDefault(controlVariable)
					stepValue = Me.NullableValueOrDefault(stepValue)
				End If
				boundExpression = Me.TransformRewrittenBinaryOperator(New BoundBinaryOperator(stepValue.Syntax, BinaryOperatorKind.Add, boundExpression1.MakeRValue(), stepValue, isChecked, boundExpression1.Type, False))
				If (controlVariable.Type.IsNullableType()) Then
					boundExpression = Me.MakeTernaryConditionalExpression(boundExpression.Syntax, boundExpression2, Me.WrapInNullable(boundExpression, controlVariable.Type), LocalRewriter.NullableNull(controlVariable.Syntax, controlVariable.Type))
				End If
			End If
			Return New BoundExpressionStatement(stepValue.Syntax, New BoundAssignmentOperator(stepValue.Syntax, controlVariable, boundExpression, True, controlVariable.Type, False), False)
		End Function

		Private Function RewriteFromObjectConversion(ByVal node As BoundConversion, ByVal typeFrom As TypeSymbol, ByVal underlyingTypeTo As TypeSymbol) As BoundExpression
			Dim boundCall As BoundExpression = node
			Dim wellKnownMember As Microsoft.CodeAnalysis.WellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Count
			Select Case underlyingTypeTo.SpecialType
				Case SpecialType.System_Boolean
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToBooleanObject
					Exit Select
				Case SpecialType.System_Char
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharObject
					Exit Select
				Case SpecialType.System_SByte
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSByteObject
					Exit Select
				Case SpecialType.System_Byte
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToByteObject
					Exit Select
				Case SpecialType.System_Int16
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToShortObject
					Exit Select
				Case SpecialType.System_UInt16
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUShortObject
					Exit Select
				Case SpecialType.System_Int32
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToIntegerObject
					Exit Select
				Case SpecialType.System_UInt32
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUIntegerObject
					Exit Select
				Case SpecialType.System_Int64
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToLongObject
					Exit Select
				Case SpecialType.System_UInt64
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToULongObject
					Exit Select
				Case SpecialType.System_Decimal
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDecimalObject
					Exit Select
				Case SpecialType.System_Single
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSingleObject
					Exit Select
				Case SpecialType.System_Double
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDoubleObject
					Exit Select
				Case SpecialType.System_String
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringObject
					Exit Select
				Case SpecialType.System_IntPtr
				Case SpecialType.System_UIntPtr
				Case SpecialType.System_Array
				Case SpecialType.System_Collections_IEnumerable
				Case SpecialType.System_Collections_Generic_IEnumerable_T
				Case SpecialType.System_Collections_Generic_IList_T
				Case SpecialType.System_Collections_Generic_ICollection_T
				Case SpecialType.System_Collections_IEnumerator
				Case SpecialType.System_Collections_Generic_IEnumerator_T
				Case SpecialType.System_Collections_Generic_IReadOnlyList_T
				Case SpecialType.System_Collections_Generic_IReadOnlyCollection_T
				Case SpecialType.System_Nullable_T
				Label0:
					If (Not underlyingTypeTo.IsTypeParameter()) Then
						Exit Select
					End If
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToGenericParameter_T_Object
					Exit Select
				Case SpecialType.System_DateTime
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDateObject
					Exit Select
				Case Else
					GoTo Label0
			End Select
			If (wellKnownMember <> Microsoft.CodeAnalysis.WellKnownMember.Count) Then
				Dim wellKnownTypeMember As MethodSymbol = DirectCast(Me.Compilation.GetWellKnownTypeMember(wellKnownMember), MethodSymbol)
				If (Not Me.ReportMissingOrBadRuntimeHelper(node, wellKnownMember, wellKnownTypeMember)) Then
					If (wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToGenericParameter_T_Object) Then
						wellKnownTypeMember = wellKnownTypeMember.Construct(New TypeSymbol() { underlyingTypeTo })
					End If
					Dim operand As BoundExpression = node.Operand
					If (Not operand.Type.IsObjectType()) Then
						Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me.GetNewCompoundUseSiteInfo()
						operand = New BoundDirectCast(operand.Syntax, operand, Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(operand.Type, typeFrom, newCompoundUseSiteInfo), typeFrom, False)
						Me._diagnostics.Add(node, newCompoundUseSiteInfo)
					End If
					Dim syntax As SyntaxNode = node.Syntax
					Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(operand)
					Dim returnType As TypeSymbol = wellKnownTypeMember.ReturnType
					Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
					boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, wellKnownTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)
					Dim type As TypeSymbol = node.Type
					If (Not type.IsSameTypeIgnoringAll(wellKnownTypeMember.ReturnType)) Then
						Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Narrowing] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Numeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions
						boundCall = New BoundConversion(node.Syntax, boundCall, conversionKind, node.Checked, node.ExplicitCastInCode, type, False)
					End If
				End If
			End If
			Return boundCall
		End Function

		Private Function RewriteFromStringConversion(ByVal node As BoundConversion, ByVal typeFrom As TypeSymbol, ByVal underlyingTypeTo As TypeSymbol) As BoundExpression
			Dim boundCall As BoundExpression = node
			Dim wellKnownMember As Microsoft.CodeAnalysis.WellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Count
			Select Case underlyingTypeTo.SpecialType
				Case SpecialType.System_Boolean
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToBooleanString
					Exit Select
				Case SpecialType.System_Char
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharString
					Exit Select
				Case SpecialType.System_SByte
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSByteString
					Exit Select
				Case SpecialType.System_Byte
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToByteString
					Exit Select
				Case SpecialType.System_Int16
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToShortString
					Exit Select
				Case SpecialType.System_UInt16
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUShortString
					Exit Select
				Case SpecialType.System_Int32
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToIntegerString
					Exit Select
				Case SpecialType.System_UInt32
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToUIntegerString
					Exit Select
				Case SpecialType.System_Int64
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToLongString
					Exit Select
				Case SpecialType.System_UInt64
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToULongString
					Exit Select
				Case SpecialType.System_Decimal
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDecimalString
					Exit Select
				Case SpecialType.System_Single
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToSingleString
					Exit Select
				Case SpecialType.System_Double
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDoubleString
					Exit Select
				Case SpecialType.System_String
				Case SpecialType.System_IntPtr
				Case SpecialType.System_UIntPtr
				Case SpecialType.System_Array
				Case SpecialType.System_Collections_IEnumerable
				Case SpecialType.System_Collections_Generic_IEnumerable_T
				Case SpecialType.System_Collections_Generic_IList_T
				Case SpecialType.System_Collections_Generic_ICollection_T
				Case SpecialType.System_Collections_IEnumerator
				Case SpecialType.System_Collections_Generic_IEnumerator_T
				Case SpecialType.System_Collections_Generic_IReadOnlyList_T
				Case SpecialType.System_Collections_Generic_IReadOnlyCollection_T
				Case SpecialType.System_Nullable_T
				Label0:
					If (Not underlyingTypeTo.IsCharSZArray()) Then
						Exit Select
					End If
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharArrayRankOneString
					Exit Select
				Case SpecialType.System_DateTime
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDateString
					Exit Select
				Case Else
					GoTo Label0
			End Select
			If (wellKnownMember <> Microsoft.CodeAnalysis.WellKnownMember.Count) Then
				Dim wellKnownTypeMember As MethodSymbol = DirectCast(Me.Compilation.GetWellKnownTypeMember(wellKnownMember), MethodSymbol)
				If (Not Me.ReportMissingOrBadRuntimeHelper(node, wellKnownMember, wellKnownTypeMember)) Then
					Dim operand As BoundExpression = node.Operand
					Dim syntax As SyntaxNode = node.Syntax
					Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(operand)
					Dim returnType As TypeSymbol = wellKnownTypeMember.ReturnType
					Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
					boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, wellKnownTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)
					Dim type As TypeSymbol = node.Type
					If (Not type.IsSameTypeIgnoringAll(wellKnownTypeMember.ReturnType)) Then
						Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Narrowing] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Numeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions
						boundCall = New BoundConversion(node.Syntax, boundCall, conversionKind, node.Checked, node.ExplicitCastInCode, type, False)
					End If
				End If
			End If
			Return boundCall
		End Function

		Private Function RewriteIfStatement(ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal rewrittenCondition As BoundExpression, ByVal rewrittenConsequence As Microsoft.CodeAnalysis.VisualBasic.BoundStatement, ByVal rewrittenAlternative As Microsoft.CodeAnalysis.VisualBasic.BoundStatement, ByVal instrumentationTargetOpt As Microsoft.CodeAnalysis.VisualBasic.BoundStatement, Optional ByVal unstructuredExceptionHandlingResumeTarget As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = Nothing) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundStatementList As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim labelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol = LocalRewriter.GenerateLabel("afterif")
			Dim boundLabelStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(syntaxNode, labelSymbol)
			If (rewrittenAlternative IsNot Nothing) Then
				Dim labelSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol = LocalRewriter.GenerateLabel("alternative")
				Dim boundConditionalGoto As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundConditionalGoto(syntaxNode, rewrittenCondition, False, labelSymbol1, False)
				If (Not unstructuredExceptionHandlingResumeTarget.IsDefaultOrEmpty) Then
					boundConditionalGoto = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(boundConditionalGoto.Syntax, unstructuredExceptionHandlingResumeTarget.Add(boundConditionalGoto), False)
				End If
				If (instrumentationTargetOpt IsNot Nothing) Then
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = instrumentationTargetOpt.Syntax.Kind()
					If (syntaxKind > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineIfBlock) Then
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfBlock) Then
							GoTo Label2
						End If
						If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseBlock) Then
							Throw ExceptionUtilities.UnexpectedValue(instrumentationTargetOpt.Syntax.Kind())
						End If
						boundConditionalGoto = Me._instrumenterOpt.InstrumentCaseBlockConditionalGoto(DirectCast(instrumentationTargetOpt, BoundCaseBlock), boundConditionalGoto)
						GoTo Label0
					Else
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineIfBlock) Then
							GoTo Label2
						End If
						Throw ExceptionUtilities.UnexpectedValue(instrumentationTargetOpt.Syntax.Kind())
					End If
				Label2:
					boundConditionalGoto = Me._instrumenterOpt.InstrumentIfStatementConditionalGoto(DirectCast(instrumentationTargetOpt, BoundIfStatement), boundConditionalGoto)
				End If
			Label0:
				Dim boundStatementArray() As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = { boundConditionalGoto, rewrittenConsequence, New BoundGotoStatement(syntaxNode, labelSymbol, Nothing, False), New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(syntaxNode, labelSymbol1), rewrittenAlternative, boundLabelStatement }
				boundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(syntaxNode, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(boundStatementArray), False)
			Else
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundConditionalGoto(syntaxNode, rewrittenCondition, False, labelSymbol, False)
				If (Not unstructuredExceptionHandlingResumeTarget.IsDefaultOrEmpty) Then
					boundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(boundStatement.Syntax, unstructuredExceptionHandlingResumeTarget.Add(boundStatement), False)
				End If
				If (instrumentationTargetOpt IsNot Nothing) Then
					Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = instrumentationTargetOpt.Syntax.Kind()
					If (syntaxKind1 > Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineIfBlock) Then
						If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfBlock) Then
							GoTo Label6
						End If
						If (syntaxKind1 <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseBlock) Then
							Throw ExceptionUtilities.UnexpectedValue(instrumentationTargetOpt.Syntax.Kind())
						End If
						boundStatement = Me._instrumenterOpt.InstrumentCaseBlockConditionalGoto(DirectCast(instrumentationTargetOpt, BoundCaseBlock), boundStatement)
						GoTo Label4
					Else
						If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement OrElse syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineIfBlock) Then
							GoTo Label6
						End If
						Throw ExceptionUtilities.UnexpectedValue(instrumentationTargetOpt.Syntax.Kind())
					End If
				Label6:
					boundStatement = Me._instrumenterOpt.InstrumentIfStatementConditionalGoto(DirectCast(instrumentationTargetOpt, BoundIfStatement), boundStatement)
				Label4:
					boundLabelStatement = If(instrumentationTargetOpt.Syntax.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineIfBlock, SyntheticBoundNodeFactory.HiddenSequencePoint(boundLabelStatement), Me._instrumenterOpt.InstrumentIfStatementAfterIfStatement(DirectCast(instrumentationTargetOpt, BoundIfStatement), boundLabelStatement))
				End If
				boundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(syntaxNode, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(boundStatement, rewrittenConsequence, boundLabelStatement), False)
			End If
			Return boundStatementList
		End Function

		Private Function RewriteInterpolatedStringConversion(ByVal conversion As BoundConversion) As BoundExpression
			Dim type As TypeSymbol = conversion.Type
			Dim operand As BoundInterpolatedStringExpression = DirectCast(conversion.Operand, BoundInterpolatedStringExpression)
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = operand.Binder
			Return Me.InvokeInterpolatedStringFactory(operand, binder.GetWellKnownType(WellKnownType.System_Runtime_CompilerServices_FormattableStringFactory, conversion.Syntax, Me._diagnostics), "Create", conversion.Type, New SyntheticBoundNodeFactory(Me._topMethod, Me._currentMethodOrLambda, operand.Syntax, Me._compilationState, Me._diagnostics))
		End Function

		Private Function RewriteLambdaRelaxationConversion(ByVal node As BoundConversion) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim lambda As BoundLambda = DirectCast(node.ExtendedInfoOpt, BoundRelaxationLambda).Lambda
			If (Not Me._inExpressionLambda OrElse Not LocalRewriter.NoParameterRelaxation(node.Operand, lambda.LambdaSymbol)) Then
				boundNode = node.Update(Me.VisitExpressionNode(lambda), node.ConversionKind, node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, Nothing, node.Type)
			Else
				boundNode = MyBase.VisitConversion(node.Update(node.Operand, node.ConversionKind, node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, Nothing, node.Type))
				boundNode = Me.TransformRewrittenConversion(DirectCast(boundNode, BoundConversion))
			End If
			Return boundNode
		End Function

		Private Function RewriteLambdaRelaxationConversion(ByVal node As BoundDirectCast) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (Not Me._inExpressionLambda OrElse Not LocalRewriter.NoParameterRelaxation(node.Operand, node.RelaxationLambdaOpt.LambdaSymbol)) Then
				boundNode = node.Update(Me.VisitExpressionNode(node.RelaxationLambdaOpt), node.ConversionKind, node.SuppressVirtualCalls, node.ConstantValueOpt, Nothing, node.Type)
			Else
				boundNode = MyBase.VisitDirectCast(node.Update(node.Operand, node.ConversionKind, node.SuppressVirtualCalls, node.ConstantValueOpt, Nothing, node.Type))
			End If
			Return boundNode
		End Function

		Private Function RewriteLambdaRelaxationConversion(ByVal node As BoundTryCast) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (Not Me._inExpressionLambda OrElse Not LocalRewriter.NoParameterRelaxation(node.Operand, node.RelaxationLambdaOpt.LambdaSymbol)) Then
				boundNode = node.Update(Me.VisitExpressionNode(node.RelaxationLambdaOpt), node.ConversionKind, node.ConstantValueOpt, Nothing, node.Type)
			Else
				boundNode = MyBase.VisitTryCast(node.Update(node.Operand, node.ConversionKind, node.ConstantValueOpt, Nothing, node.Type))
			End If
			Return boundNode
		End Function

		Private Function RewriteLateBoundAssignment(ByVal node As BoundAssignmentOperator) As BoundNode
			Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Left
			Dim empty As ImmutableArray(Of SynthesizedLocal) = ImmutableArray(Of SynthesizedLocal).Empty
			If (node.LeftOnTheRightOpt IsNot Nothing) Then
				left = left.SetLateBoundAccessKind(LateBoundAccessKind.Unknown)
				Dim instance As ArrayBuilder(Of SynthesizedLocal) = ArrayBuilder(Of SynthesizedLocal).GetInstance()
				Dim result As UseTwiceRewriter.Result = UseTwiceRewriter.UseTwice(Me._currentMethodOrLambda, left, instance)
				empty = instance.ToImmutableAndFree()
				left = result.First.SetLateBoundAccessKind(LateBoundAccessKind.[Set])
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = result.Second.SetLateBoundAccessKind(LateBoundAccessKind.[Get])
				Me.AddPlaceholderReplacement(node.LeftOnTheRightOpt, Me.VisitExpressionNode(boundExpression))
			End If
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.Right)
			If (node.LeftOnTheRightOpt IsNot Nothing) Then
				Me.RemovePlaceholderReplacement(node.LeftOnTheRightOpt)
			End If
			If (left.Kind <> BoundKind.LateMemberAccess) Then
				Dim boundLateInvocation As Microsoft.CodeAnalysis.VisualBasic.BoundLateInvocation = DirectCast(left, Microsoft.CodeAnalysis.VisualBasic.BoundLateInvocation)
				If (boundLateInvocation.Member.Kind <> BoundKind.LateMemberAccess) Then
					boundLateInvocation = boundLateInvocation.Update(Me.VisitExpressionNode(boundLateInvocation.Member), Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundLateInvocation.ArgumentsOpt), boundLateInvocation.ArgumentNamesOpt, boundLateInvocation.AccessKind, boundLateInvocation.MethodOrPropertyGroupOpt, boundLateInvocation.Type)
					boundSequence = Me.LateIndexSet(node.Syntax, boundLateInvocation, boundExpression1, False)
				Else
					boundSequence = Me.LateSet(node.Syntax, DirectCast(MyBase.VisitLateMemberAccess(DirectCast(boundLateInvocation.Member, Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess)), Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess), boundExpression1, Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundLateInvocation.ArgumentsOpt), boundLateInvocation.ArgumentNamesOpt, False)
				End If
			Else
				Dim syntax As SyntaxNode = node.Syntax
				Dim boundLateMemberAccess As Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess = DirectCast(MyBase.VisitLateMemberAccess(DirectCast(left, Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess)), Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess)
				Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)()
				Dim strs As ImmutableArray(Of String) = New ImmutableArray(Of String)()
				boundSequence = Me.LateSet(syntax, boundLateMemberAccess, boundExpression1, boundExpressions, strs, False)
			End If
			If (empty.Length > 0) Then
				boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(node.Syntax, StaticCast(Of LocalSymbol).From(Of SynthesizedLocal)(empty), ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundSequence), Nothing, boundSequence.Type, False)
			End If
			Return boundSequence
		End Function

		Private Function RewriteLateBoundIndexInvocation(ByVal invocation As BoundLateInvocation, ByVal receiverExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal argExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(invocation.Member)
			Return Me.LateIndexGet(invocation, boundExpression, Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(argExpressions))
		End Function

		Private Function RewriteLateBoundMemberInvocation(ByVal memberAccess As BoundLateMemberAccess, ByVal receiverExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal argExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression), ByVal argNames As ImmutableArray(Of String), ByVal useLateCall As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim synthesizedLocals As ArrayBuilder(Of SynthesizedLocal) = Nothing
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(receiverExpression)
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)()
			Me.LateCaptureArgsComplex(synthesizedLocals, argExpressions, boundExpressions)
			Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.LateCallOrGet(memberAccess, boundExpression, argExpressions, boundExpressions, argNames, useLateCall)
			Dim immutableAndFree As ImmutableArray(Of SynthesizedLocal) = New ImmutableArray(Of SynthesizedLocal)()
			If (synthesizedLocals IsNot Nothing) Then
				immutableAndFree = synthesizedLocals.ToImmutableAndFree()
				If (Not immutableAndFree.IsEmpty) Then
					boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(memberAccess.Syntax, StaticCast(Of LocalSymbol).From(Of SynthesizedLocal)(immutableAndFree), ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty, boundSequence, boundSequence.Type, False)
				End If
			End If
			Return boundSequence
		End Function

		Private Function RewriteLiftedBooleanBinaryOperator(ByVal node As BoundBinaryOperator, ByVal left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal right As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal leftHasNoValue As Boolean, ByVal rightHasNoValue As Boolean, ByVal leftHasValue As Boolean, ByVal rightHasValue As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim flag As Boolean
			Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundUnaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim type As TypeSymbol = node.Type
			Dim nullableUnderlyingType As TypeSymbol = type.GetNullableUnderlyingType()
			Dim operatorKind As BinaryOperatorKind = node.OperatorKind And BinaryOperatorKind.OpMask
			Dim flag1 As Boolean = If(operatorKind = BinaryOperatorKind.[OrElse], True, operatorKind = BinaryOperatorKind.[Or])
			If (Not (leftHasNoValue Or rightHasNoValue)) Then
				Dim flag2 As Boolean = operatorKind = BinaryOperatorKind.[AndAlso] Or operatorKind = BinaryOperatorKind.[OrElse]
				Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = Nothing
				Dim boundExpression5 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim boundExpression6 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = left
				If (Not leftHasValue) Then
					boundExpression6 = Me.CaptureNullableIfNeeded(left, synthesizedLocal, boundExpression5, LocalRewriter.RightCantChangeLeftLocal(left, right))
				End If
				Dim synthesizedLocal1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = Nothing
				Dim boundExpression7 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim boundExpression8 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = right
				If (Not rightHasValue AndAlso (Not leftHasValue OrElse Not flag2)) Then
					boundExpression8 = Me.CaptureNullableIfNeeded(boundExpression8, synthesizedLocal1, boundExpression7, True)
				End If
				Dim boundExpression9 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = If(leftHasValue, boundExpression8, boundExpression6)
				Dim boundExpression10 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.NullableOfBooleanValue(node.Syntax, flag1, type)
				Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.MakeTernaryConditionalExpression(node.Syntax, If(leftHasValue, Me.NullableValueOrDefault(boundExpression6), Me.NullableValueOrDefault(boundExpression8)), If(flag1, boundExpression10, boundExpression9), If(flag1, boundExpression9, boundExpression10))
				If (Not leftHasValue) Then
					If (Not rightHasValue) Then
						If (Not flag2) Then
							boundExpression3 = boundExpression8
						Else
							boundExpression3 = If(boundExpression7, boundExpression8)
							boundExpression7 = Nothing
						End If
						boundSequence = Me.MakeTernaryConditionalExpression(node.Syntax, Me.NullableHasValue(boundExpression3), boundSequence, LocalRewriter.NullableNull(node.Syntax, type))
					End If
					If (Not rightHasValue OrElse flag2) Then
						Dim boundExpression11 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.NullableValueOrDefault(boundExpression6)
						If (boundExpression7 IsNot Nothing OrElse boundExpression5 Is Nothing) Then
							boundExpression4 = boundExpression6
						Else
							boundExpression4 = boundExpression5
							boundExpression5 = Nothing
						End If
						Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
						Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
						Dim boundExpression12 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.NullableHasValue(boundExpression4)
						If (flag1) Then
							boundUnaryOperator = boundExpression11
						Else
							boundUnaryOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundUnaryOperator(node.Syntax, UnaryOperatorKind.[Not], boundExpression11, False, nullableUnderlyingType, False)
						End If
						boundSequence = Me.MakeTernaryConditionalExpression(syntax, Me.MakeBooleanBinaryExpression(syntaxNode, BinaryOperatorKind.[AndAlso], boundExpression12, boundUnaryOperator), Me.NullableOfBooleanValue(node.Syntax, flag1, type), boundSequence)
					End If
				End If
				If (synthesizedLocal IsNot Nothing OrElse synthesizedLocal1 IsNot Nothing) Then
					Dim instance As ArrayBuilder(Of LocalSymbol) = ArrayBuilder(Of LocalSymbol).GetInstance()
					Dim boundExpressions As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
					If (synthesizedLocal IsNot Nothing) Then
						instance.Add(synthesizedLocal)
						If (boundExpression5 IsNot Nothing) Then
							boundExpressions.Add(boundExpression5)
						End If
					End If
					If (synthesizedLocal1 IsNot Nothing) Then
						instance.Add(synthesizedLocal1)
						If (boundExpression7 IsNot Nothing) Then
							boundExpressions.Add(boundExpression7)
						End If
					End If
					boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(node.Syntax, instance.ToImmutableAndFree(), boundExpressions.ToImmutableAndFree(), boundSequence, boundSequence.Type, False)
				End If
				boundExpression = boundSequence
			Else
				If (Not rightHasNoValue) Then
					boundExpression1 = right
					boundExpression2 = left
					flag = rightHasValue
				Else
					boundExpression1 = left
					boundExpression2 = right
					flag = leftHasValue
				End If
				If (Not flag) Then
					Dim synthesizedLocal2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = Nothing
					Dim boundExpression13 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
					Dim boundExpression14 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.CaptureNullableIfNeeded(boundExpression1, synthesizedLocal2, boundExpression13, True)
					Dim syntax1 As Microsoft.CodeAnalysis.SyntaxNode = boundExpression1.Syntax
					Me.NullableValueOrDefault(boundExpression14)
					Dim boundExpression15 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = If(flag1, Me.NullableValueOrDefault(boundExpression14), Me.MakeBooleanBinaryExpression(syntax1, BinaryOperatorKind.[And], Me.NullableHasValue(boundExpression14), New Microsoft.CodeAnalysis.VisualBasic.BoundUnaryOperator(syntax1, UnaryOperatorKind.[Not], Me.NullableValueOrDefault(boundExpression14), False, nullableUnderlyingType, False)))
					Dim boundSequence1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.MakeTernaryConditionalExpression(node.Syntax, boundExpression15, boundExpression14, LocalRewriter.NullableNull(boundExpression2, type))
					If (synthesizedLocal2 IsNot Nothing) Then
						boundSequence1 = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(node.Syntax, ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal2), ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpression13), boundSequence1, boundSequence1.Type, False)
					End If
					boundExpression = boundSequence1
				Else
					Dim syntaxNode1 As Microsoft.CodeAnalysis.SyntaxNode = boundExpression1.Syntax
					Dim boundExpression16 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.NullableValueOrDefault(boundExpression1)
					boundExpression = Me.MakeTernaryConditionalExpression(node.Syntax, boundExpression16, If(flag1, Me.NullableTrue(syntaxNode1, type), LocalRewriter.NullableNull(boundExpression2, type)), If(flag1, LocalRewriter.NullableNull(boundExpression2, type), Me.NullableFalse(syntaxNode1, type)))
				End If
			End If
			Return boundExpression
		End Function

		Private Function RewriteLiftedIntrinsicBinaryOperatorSimple(ByVal node As BoundBinaryOperator, ByVal optimizeForConditionalBranch As Boolean) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.Left)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(LocalRewriter.GetRightOperand(node, optimizeForConditionalBranch))
			Return Me.FinishRewriteOfLiftedIntrinsicBinaryOperator(node, boundExpression, boundExpression1, optimizeForConditionalBranch)
		End Function

		Private Function RewriteLiftedUnaryOperator(ByVal node As BoundUnaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.Operand)
			If (LocalRewriter.HasNoValue(boundExpression)) Then
				boundNode = LocalRewriter.NullableNull(boundExpression, node.Type)
			ElseIf (Not LocalRewriter.HasValue(boundExpression)) Then
				Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = Nothing
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.CaptureNullableIfNeeded(boundExpression, synthesizedLocal, boundExpression1, True)
				Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.ApplyUnliftedUnaryOp(node, Me.NullableValueOrDefault(boundExpression2))
				Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.MakeTernaryConditionalExpression(node.Syntax, Me.NullableHasValue(boundExpression2), Me.WrapInNullable(boundExpression3, node.Type), boundExpression2)
				If (synthesizedLocal IsNot Nothing) Then
					boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(node.Syntax, ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal), ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpression1), boundSequence, boundSequence.Type, False)
				End If
				boundNode = boundSequence
			Else
				Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.ApplyUnliftedUnaryOp(node, Me.NullableValueOrDefault(boundExpression))
				boundNode = Me.WrapInNullable(boundExpression4, node.Type)
			End If
			Return boundNode
		End Function

		Private Function RewriteLiftedUserDefinedBinaryOperator(ByVal node As BoundUserDefinedBinaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.Left)
			Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.Right)
			Dim [call] As BoundCall = node.[Call]
			Dim type As TypeSymbol = [call].Type
			Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = LocalRewriter.NullableNull(node.Syntax, type)
			Dim flag As Boolean = LocalRewriter.HasNoValue(boundExpression2)
			Dim flag1 As Boolean = LocalRewriter.HasNoValue(boundExpression3)
			If (flag And flag1) Then
				boundNode = boundExpression4
			ElseIf (Not (flag Or flag1)) Then
				Dim localSymbols As ArrayBuilder(Of LocalSymbol) = Nothing
				Dim boundExpressions As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Nothing
				Dim flag2 As Boolean = LocalRewriter.HasValue(boundExpression2)
				Dim flag3 As Boolean = LocalRewriter.HasValue(boundExpression3)
				Dim boundExpression5 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				If (flag2) Then
					boundExpression = Me.NullableValueOrDefault(boundExpression2)
					If (Not flag3) Then
						boundExpression = Me.CaptureNullableIfNeeded(boundExpression, localSymbols, boundExpressions, True)
						boundExpression1 = Me.ProcessNullableOperand(boundExpression3, boundExpression5, localSymbols, boundExpressions, True)
					Else
						boundExpression1 = Me.NullableValueOrDefault(boundExpression3)
					End If
				ElseIf (Not flag3) Then
					Dim boundExpression6 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
					Dim boundExpression7 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
					boundExpression = Me.ProcessNullableOperand(boundExpression2, boundExpression6, localSymbols, boundExpressions, True)
					boundExpression1 = Me.ProcessNullableOperand(boundExpression3, boundExpression7, localSymbols, boundExpressions, True)
					boundExpression5 = Me.MakeBooleanBinaryExpression(node.Syntax, BinaryOperatorKind.[And], boundExpression6, boundExpression7)
				Else
					boundExpression = Me.ProcessNullableOperand(boundExpression2, boundExpression5, localSymbols, boundExpressions, True)
					boundExpression1 = Me.NullableValueOrDefault(boundExpression3)
					boundExpression1 = Me.CaptureNullableIfNeeded(boundExpression1, localSymbols, boundExpressions, True)
				End If
				Dim method As MethodSymbol = [call].Method
				Dim receiverOpt As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = [call].ReceiverOpt
				Dim boundExpressions1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpression, boundExpression1)
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundExpression8 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = [call].Update(method, Nothing, receiverOpt, boundExpressions1, bitVector, [call].ConstantValueOpt, [call].IsLValue, [call].SuppressObjectClone, [call].Method.ReturnType)
				If (Not boundExpression8.Type.IsSameTypeIgnoringAll(type)) Then
					boundExpression8 = Me.WrapInNullable(boundExpression8, type)
				End If
				If (Not (flag2 And flag3)) Then
					Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.MakeTernaryConditionalExpression(node.Syntax, boundExpression5, boundExpression8, boundExpression4)
					If (localSymbols IsNot Nothing) Then
						boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(node.Syntax, localSymbols.ToImmutableAndFree(), boundExpressions.ToImmutableAndFree(), boundSequence, boundSequence.Type, False)
					End If
					boundNode = boundSequence
				Else
					boundNode = boundExpression8
				End If
			Else
				boundNode = LocalRewriter.MakeSequence(If(flag, boundExpression3, boundExpression2), boundExpression4)
			End If
			Return boundNode
		End Function

		Private Function RewriteLiftedUserDefinedUnaryOperator(ByVal node As BoundUserDefinedUnaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.Operand)
			Dim [call] As BoundCall = node.[Call]
			Dim type As TypeSymbol = [call].Type
			Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = LocalRewriter.NullableNull(node.Syntax, type)
			If (Not LocalRewriter.HasNoValue(boundExpression2)) Then
				Dim localSymbols As ArrayBuilder(Of LocalSymbol) = Nothing
				Dim boundExpressions As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Nothing
				Dim flag As Boolean = LocalRewriter.HasValue(boundExpression2)
				Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				boundExpression1 = If(Not flag, Me.ProcessNullableOperand(boundExpression2, boundExpression4, localSymbols, boundExpressions, True), Me.NullableValueOrDefault(boundExpression2))
				Dim method As MethodSymbol = [call].Method
				Dim receiverOpt As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = [call].ReceiverOpt
				Dim boundExpressions1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpression1)
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundExpression5 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = [call].Update(method, Nothing, receiverOpt, boundExpressions1, bitVector, [call].ConstantValueOpt, [call].IsLValue, [call].SuppressObjectClone, [call].Method.ReturnType)
				If (Not boundExpression5.Type.IsSameTypeIgnoringAll(type)) Then
					boundExpression5 = Me.WrapInNullable(boundExpression5, type)
				End If
				If (Not flag) Then
					Dim boundExpression6 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundExpression4
					Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.MakeTernaryConditionalExpression(node.Syntax, boundExpression6, boundExpression5, boundExpression3)
					If (localSymbols IsNot Nothing) Then
						boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(node.Syntax, localSymbols.ToImmutableAndFree(), boundExpressions.ToImmutableAndFree(), boundSequence, boundSequence.Type, False)
					End If
					boundExpression = boundSequence
				Else
					boundExpression = boundExpression5
				End If
			Else
				boundExpression = boundExpression3
			End If
			Return boundExpression
		End Function

		Private Function RewriteLikeOperator(ByVal node As BoundBinaryOperator, ByVal member As WellKnownMember) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Left
			Dim right As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Right
			Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node
			Dim operatorKind As Boolean = CInt((node.OperatorKind And BinaryOperatorKind.CompareText)) <> 0
			Dim wellKnownTypeMember As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(Me.Compilation.GetWellKnownTypeMember(member), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			If (Not Me.ReportMissingOrBadRuntimeHelper(node, member, wellKnownTypeMember)) Then
				Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = wellKnownTypeMember
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = left
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = right
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = Microsoft.CodeAnalysis.ConstantValue.Create(If(operatorKind, 1, 0))
				Dim parameters As ImmutableArray(Of ParameterSymbol) = wellKnownTypeMember.Parameters
				Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpression, boundExpression1, New BoundLiteral(syntaxNode, constantValue, parameters(2).Type))
				Dim returnType As TypeSymbol = wellKnownTypeMember.ReturnType
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, methodSymbol, Nothing, Nothing, boundExpressions, Nothing, returnType, True, False, bitVector)
			End If
			Return boundCall
		End Function

		Private Function RewriteLocalDeclarationAsInitializer(ByVal node As BoundLocalDeclaration, ByVal rewrittenInitializer As BoundExpression, ByVal staticLocalBackingFields As KeyValuePair(Of SynthesizedStaticLocalBackingField, SynthesizedStaticLocalBackingField), Optional ByVal objectInitializerNeedsTemporary As Boolean = True) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim unstructuredExceptionHandlingContext As LocalRewriter.UnstructuredExceptionHandlingContext = Me.LeaveUnstructuredExceptionHandlingContext(node)
			boundStatement = If(objectInitializerNeedsTemporary, New BoundExpressionStatement(rewrittenInitializer.Syntax, New BoundAssignmentOperator(rewrittenInitializer.Syntax, Me.VisitExpressionNode(New BoundLocal(node.Syntax, node.LocalSymbol, node.LocalSymbol.Type)), rewrittenInitializer, True, node.LocalSymbol.Type, False), False), New BoundExpressionStatement(rewrittenInitializer.Syntax, rewrittenInitializer, False))
			If (node.LocalSymbol.IsStatic) Then
				boundStatement = Me.EnforceStaticLocalInitializationSemantics(staticLocalBackingFields, boundStatement)
			End If
			Me.RestoreUnstructuredExceptionHandlingContext(node, unstructuredExceptionHandlingContext)
			If (Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)) Then
				boundStatement = Me.RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, boundStatement, True)
			End If
			If (Me(node)) Then
				boundStatement = Me._instrumenterOpt.InstrumentLocalInitialization(node, boundStatement)
			End If
			Return boundStatement
		End Function

		Private Shared Function RewriteNode(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode, ByVal topMethod As MethodSymbol, ByVal currentMethod As MethodSymbol, ByVal compilationState As TypeCompilationState, ByVal previousSubmissionFields As SynthesizedSubmissionFields, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, <InAttribute> <Out> ByRef rewrittenNodes As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.BoundNode), <Out> ByRef hasLambdas As Boolean, <Out> ByRef symbolsCapturedWithoutCtor As ISet(Of Symbol), ByVal flags As Microsoft.CodeAnalysis.VisualBasic.LocalRewriter.RewritingFlags, ByVal instrumenterOpt As Instrumenter, ByVal recursionDepth As Integer) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim localRewriter As Microsoft.CodeAnalysis.VisualBasic.LocalRewriter = New Microsoft.CodeAnalysis.VisualBasic.LocalRewriter(topMethod, currentMethod, compilationState, previousSubmissionFields, diagnostics, flags, instrumenterOpt, recursionDepth)
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = localRewriter.Visit(node)
			If (Not localRewriter._xmlFixupData.IsEmpty) Then
				boundNode = Microsoft.CodeAnalysis.VisualBasic.LocalRewriter.InsertXmlLiteralsPreamble(boundNode, localRewriter._xmlFixupData.MaterializeAndFree())
			End If
			hasLambdas = localRewriter._hasLambdas
			symbolsCapturedWithoutCtor = localRewriter._symbolsCapturedWithoutCopyCtor
			Return boundNode
		End Function

		Private Function RewriteNoPiaAddRemoveHandler(ByVal node As BoundAddRemoveHandlerStatement, ByVal receiver As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal [event] As EventSymbol, ByVal handler As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundBadExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = New Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory(Me._topMethod, Me._currentMethodOrLambda, node.Syntax, Me._compilationState, Me._diagnostics)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = syntheticBoundNodeFactory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(WellKnownMember.System_Runtime_InteropServices_ComAwareEventInfo__ctor, False)
			If (methodSymbol IsNot Nothing) Then
				Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = syntheticBoundNodeFactory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(If(node.Kind = BoundKind.AddHandlerStatement, WellKnownMember.System_Runtime_InteropServices_ComAwareEventInfo__AddEventHandler, WellKnownMember.System_Runtime_InteropServices_ComAwareEventInfo__RemoveEventHandler), False)
				If (methodSymbol1 IsNot Nothing) Then
					Dim boundObjectCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression = syntheticBoundNodeFactory.[New](methodSymbol, New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { syntheticBoundNodeFactory.[Typeof]([event].ContainingType), syntheticBoundNodeFactory.Literal([event].MetadataName) })
					Dim boundExpressionArray(1) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
					Dim parameters As ImmutableArray(Of ParameterSymbol) = methodSymbol1.Parameters
					boundExpressionArray(0) = Me.Convert(syntheticBoundNodeFactory, parameters(0).Type, receiver.MakeRValue())
					parameters = methodSymbol1.Parameters
					boundExpressionArray(1) = Me.Convert(syntheticBoundNodeFactory, parameters(1).Type, handler)
					boundExpression = syntheticBoundNodeFactory.[Call](boundObjectCreationExpression, methodSymbol1, boundExpressionArray)
				End If
			End If
			If (Me._emitModule IsNot Nothing) Then
				Me._emitModule.EmbeddedTypesManagerOpt.EmbedEventIfNeedTo([event].GetCciAdapter(), node.Syntax, Me._diagnostics.DiagnosticBag, True)
			End If
			If (boundExpression Is Nothing) Then
				boundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(node.Syntax, LookupResultKind.NotCreatable, ImmutableArray.Create(Of Symbol)([event]), ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(receiver, handler), ErrorTypeSymbol.UnknownResultType, True)
			Else
				boundBadExpression = boundExpression
			End If
			Return boundBadExpression
		End Function

		Private Function RewriteNullableBinaryConditionalExpression(ByVal node As BoundBinaryConditionalExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.TestExpression)
			If (Not LocalRewriter.HasValue(boundExpression1)) Then
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.ElseExpression)
				If (Not LocalRewriter.HasNoValue(boundExpression1)) Then
					Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
					Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
					If (LocalRewriter.IsConditionalAccess(boundExpression1, boundExpression3, boundExpression4) AndAlso LocalRewriter.HasNoValue(boundExpression4) AndAlso LocalRewriter.HasValue(boundExpression3)) Then
						boundNode = LocalRewriter.UpdateConditionalAccess(boundExpression1, Me.MakeResultFromNonNullLeft(boundExpression3, node.ConvertedTestExpression, node.TestExpressionPlaceholder), boundExpression2)
					ElseIf (Not boundExpression1.Type.IsNullableType() OrElse Not boundExpression2.IsDefaultValue() OrElse Not boundExpression2.Type.IsSameTypeIgnoringAll(boundExpression1.Type.GetNullableUnderlyingType())) Then
						Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = Nothing
						Dim boundExpression5 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
						Dim boundExpression6 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.CaptureNullableIfNeeded(boundExpression1, synthesizedLocal, boundExpression5, True)
						Dim boundExpression7 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.NullableHasValue(If(boundExpression5, boundExpression6))
						If (node.ConvertedTestExpression IsNot Nothing) Then
							boundExpression = If(Not boundExpression6.Type.IsSameTypeIgnoringAll(node.ConvertedTestExpression.Type), Me.VisitExpressionNode(node.ConvertedTestExpression, node.TestExpressionPlaceholder, Me.NullableValueOrDefault(boundExpression6)), boundExpression6)
						Else
							boundExpression = Me.NullableValueOrDefault(boundExpression6)
						End If
						Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.MakeTernaryConditionalExpression(node.Syntax, boundExpression7, boundExpression, boundExpression2)
						If (synthesizedLocal IsNot Nothing) Then
							boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(node.Syntax, ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal), ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty, boundSequence, boundSequence.Type, False)
						End If
						boundNode = boundSequence
					Else
						boundNode = Me.NullableValueOrDefault(boundExpression1)
					End If
				Else
					boundNode = boundExpression2
				End If
			Else
				boundNode = Me.MakeResultFromNonNullLeft(boundExpression1, node.ConvertedTestExpression, node.TestExpressionPlaceholder)
			End If
			Return boundNode
		End Function

		Private Function RewriteNullableConversion(ByVal node As BoundConversion) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Operand), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			boundExpression = If(Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(node.ConversionKind), Me.RewriteNullableConversion(node, boundExpression1), boundExpression1)
			Return boundExpression
		End Function

		Private Function RewriteNullableConversion(ByVal node As BoundConversion, ByVal rewrittenOperand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundDirectCast As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = node.Type
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = rewrittenOperand.Type
			If (CObj(rewrittenOperand.ConstantValueOpt) = CObj(ConstantValue.[Nothing])) Then
				boundDirectCast = LocalRewriter.NullableNull(rewrittenOperand.Syntax, type)
			ElseIf (typeSymbol.IsReferenceType OrElse type.IsReferenceType) Then
				If (type.IsStringType()) Then
					rewrittenOperand = Me.NullableValue(rewrittenOperand)
				ElseIf (typeSymbol.IsStringType()) Then
					Dim nullableUnderlyingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = type.GetNullableUnderlyingType()
					Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me.GetNewCompoundUseSiteInfo()
					Dim keyValuePair As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.ConversionKind, MethodSymbol) = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyConversion(rewrittenOperand.Type, nullableUnderlyingType, newCompoundUseSiteInfo)
					Dim key As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = keyValuePair.Key
					Me._diagnostics.Add(node, newCompoundUseSiteInfo)
					boundDirectCast = Me.WrapInNullable(Me.TransformRewrittenConversion(node.Update(rewrittenOperand, key, node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, node.ExtendedInfoOpt, type.GetNullableUnderlyingType())), type)
					Return boundDirectCast
				ElseIf (typeSymbol.IsNullableType()) Then
					If (Not LocalRewriter.HasNoValue(rewrittenOperand)) Then
						If (Not LocalRewriter.HasValue(rewrittenOperand)) Then
							GoTo Label1
						End If
						Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.NullableValueOrDefault(rewrittenOperand)
						Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me.GetNewCompoundUseSiteInfo()
						Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(boundExpression.Type, type, compoundUseSiteInfo)
						Me._diagnostics.Add(node, compoundUseSiteInfo)
						boundDirectCast = New Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast(node.Syntax, boundExpression, conversionKind, type, False)
						Return boundDirectCast
					Else
						boundDirectCast = New Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast(node.Syntax, LocalRewriter.MakeNullLiteral(rewrittenOperand.Syntax, type), Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNothingLiteral, type, False)
						Return boundDirectCast
					End If
				End If
			Label1:
				boundDirectCast = Me.TransformRewrittenConversion(node.Update(rewrittenOperand, node.ConversionKind And (Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToNumericOverflow Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToIntegerOverflow Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToNumericOverflowMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToQueryLambdaBodyMismatch Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToArrayLiteralElementConversion Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Widening] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Narrowing] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Identity Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Numeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Reference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningReference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingReference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Array Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningArray Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingArray Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Value Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningValue Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingValue Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[String] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningString Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingString Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Boolean] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingBoolean Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNothingLiteral Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesNarrowingFromNumericConstant Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Lambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWidening Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningDropReturnOrArgs Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningToNonLambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNarrowing Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelInvalid Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.VarianceConversionAmbiguity Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.MightSucceedAtRuntime Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.AnonymousDelegate Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NeedAStub Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.ConvertedToExpressionTree Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.UserDefined Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingDueToContraVarianceInDelegate Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InterpolatedString Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Tuple Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTuple Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTuple), node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, node.ExtendedInfoOpt, type))
			Else
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = rewrittenOperand
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim localSymbols As ArrayBuilder(Of LocalSymbol) = Nothing
				Dim boundExpressions As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Nothing
				If (typeSymbol.IsNullableType()) Then
					If (Not type.IsNullableType()) Then
						boundExpression1 = Me.NullableValue(rewrittenOperand)
					ElseIf (LocalRewriter.HasValue(rewrittenOperand)) Then
						boundExpression1 = Me.NullableValueOrDefault(rewrittenOperand)
					ElseIf (Not LocalRewriter.HasNoValue(rewrittenOperand)) Then
						Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
						Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
						If (LocalRewriter.IsConditionalAccess(rewrittenOperand, boundExpression3, boundExpression4) AndAlso LocalRewriter.HasValue(boundExpression3) AndAlso LocalRewriter.HasNoValue(boundExpression4)) Then
							boundDirectCast = LocalRewriter.UpdateConditionalAccess(rewrittenOperand, Me.FinishRewriteNullableConversion(node, type, Me.NullableValueOrDefault(boundExpression3), Nothing, Nothing, Nothing), LocalRewriter.NullableNull(boundExpression1.Syntax, type))
							Return boundDirectCast
						End If
						boundExpression1 = Me.ProcessNullableOperand(rewrittenOperand, boundExpression2, localSymbols, boundExpressions, True)
					Else
						boundDirectCast = LocalRewriter.NullableNull(boundExpression1.Syntax, type)
						Return boundDirectCast
					End If
				End If
				boundDirectCast = Me.FinishRewriteNullableConversion(node, type, boundExpression1, boundExpression2, localSymbols, boundExpressions)
			End If
			Return boundDirectCast
		End Function

		Private Function RewriteNullableIsOrIsNotOperator(ByVal node As BoundBinaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Left
			Dim right As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Right
			If (Not Me._inExpressionLambda) Then
				boundExpression = Me.RewriteNullableIsOrIsNotOperator((node.OperatorKind And BinaryOperatorKind.OpMask) = BinaryOperatorKind.[Is], If(left.IsNothingLiteral(), right, left), node.Type)
			Else
				boundExpression = node
			End If
			Return boundExpression
		End Function

		Private Function RewriteNullableIsOrIsNotOperator(ByVal isIs As Boolean, ByVal operand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal resultType As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundLiteral As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (LocalRewriter.HasNoValue(operand)) Then
				boundLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(operand.Syntax, If(isIs, ConstantValue.[True], ConstantValue.[False]), resultType)
			ElseIf (Not LocalRewriter.HasValue(operand)) Then
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				If (Not LocalRewriter.IsConditionalAccess(operand, boundExpression, boundExpression1) OrElse Not LocalRewriter.HasNoValue(boundExpression1)) Then
					Dim boundUnaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.NullableHasValue(operand)
					If (isIs) Then
						boundUnaryOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundUnaryOperator(boundUnaryOperator.Syntax, UnaryOperatorKind.[Not], boundUnaryOperator, False, resultType, False)
					End If
					boundLiteral = boundUnaryOperator
				Else
					boundLiteral = LocalRewriter.UpdateConditionalAccess(operand, Me.RewriteNullableIsOrIsNotOperator(isIs, boundExpression, resultType), Me.RewriteNullableIsOrIsNotOperator(isIs, boundExpression1, resultType))
				End If
			Else
				boundLiteral = LocalRewriter.MakeSequence(operand, New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(operand.Syntax, If(isIs, ConstantValue.[False], ConstantValue.[True]), resultType))
			End If
			Return boundLiteral
		End Function

		Private Function RewriteNullableReferenceConversion(ByVal node As BoundConversion, ByVal rewrittenOperand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundDirectCast As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = node.Type
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = rewrittenOperand.Type
			If (typeSymbol.IsStringType()) Then
				Dim nullableUnderlyingType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = type.GetNullableUnderlyingType()
				Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me.GetNewCompoundUseSiteInfo()
				Dim keyValuePair As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.ConversionKind, MethodSymbol) = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyConversion(typeSymbol, nullableUnderlyingType, newCompoundUseSiteInfo)
				Dim key As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = keyValuePair.Key
				Me._diagnostics.Add(node, newCompoundUseSiteInfo)
				boundDirectCast = Me.WrapInNullable(Me.TransformRewrittenConversion(node.Update(rewrittenOperand, key, node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, node.ExtendedInfoOpt, type.GetNullableUnderlyingType())), type)
			ElseIf (Not type.IsStringType()) Then
				If (typeSymbol.IsNullableType()) Then
					If (Not LocalRewriter.HasNoValue(rewrittenOperand)) Then
						If (Not LocalRewriter.HasValue(rewrittenOperand)) Then
							GoTo Label1
						End If
						Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.NullableValueOrDefault(rewrittenOperand)
						Dim compoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me.GetNewCompoundUseSiteInfo()
						Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(boundExpression.Type, type, compoundUseSiteInfo)
						Me._diagnostics.Add(node, compoundUseSiteInfo)
						boundDirectCast = New Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast(node.Syntax, boundExpression, conversionKind, type, False)
						Return boundDirectCast
					Else
						boundDirectCast = New Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast(node.Syntax, LocalRewriter.MakeNullLiteral(rewrittenOperand.Syntax, type), Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNothingLiteral, type, False)
						Return boundDirectCast
					End If
				End If
			Label1:
				If (Not type.IsNullableType()) Then
					Throw ExceptionUtilities.Unreachable
				End If
				Dim newCompoundUseSiteInfo1 As CompoundUseSiteInfo(Of AssemblySymbol) = Me.GetNewCompoundUseSiteInfo()
				Dim conversionKind1 As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(rewrittenOperand.Type, type, newCompoundUseSiteInfo1)
				Me._diagnostics.Add(node, newCompoundUseSiteInfo1)
				boundDirectCast = New Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast(node.Syntax, rewrittenOperand, conversionKind1, type, False)
			Else
				rewrittenOperand = Me.NullableValue(rewrittenOperand)
				Dim compoundUseSiteInfo1 As CompoundUseSiteInfo(Of AssemblySymbol) = Me.GetNewCompoundUseSiteInfo()
				Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(rewrittenOperand.Type, type, compoundUseSiteInfo1)
				Me._diagnostics.Add(node, compoundUseSiteInfo1)
				boundDirectCast = Me.TransformRewrittenConversion(node.Update(rewrittenOperand, node.ConversionKind And (Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToNumericOverflow Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToIntegerOverflow Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToNumericOverflowMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToQueryLambdaBodyMismatch Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.FailedDueToArrayLiteralElementConversion Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Widening] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Narrowing] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Identity Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Numeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Reference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningReference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingReference Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Array Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningArray Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingArray Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.TypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTypeParameter Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Value Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningValue Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingValue Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[String] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningString Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingString Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Boolean] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingBoolean Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNothingLiteral Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesNarrowingFromNumericConstant Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Lambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWidening Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningDropReturnOrArgs Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelWideningToNonLambda Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelNarrowing Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelInvalid Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.DelegateRelaxationLevelMask Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.VarianceConversionAmbiguity Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.MightSucceedAtRuntime Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.AnonymousDelegate Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NeedAStub Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.ConvertedToExpressionTree Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.UserDefined Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingDueToContraVarianceInDelegate Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InterpolatedString Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Tuple Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningTuple Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.NarrowingTuple), node.Checked, node.ExplicitCastInCode, node.ConstantValueOpt, node.ExtendedInfoOpt, type))
			End If
			Return boundDirectCast
		End Function

		Private Function RewriteNullableUserDefinedConversion(ByVal node As BoundUserDefinedConversion) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim operand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Operand
			Dim inConversionOpt As BoundConversion = node.InConversionOpt
			Dim [call] As BoundCall = node.[Call]
			Dim outConversionOpt As BoundConversion = node.OutConversionOpt
			Dim type As TypeSymbol = outConversionOpt.Type
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(operand)
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = LocalRewriter.NullableNull(node.Syntax, type)
			If (Not LocalRewriter.HasNoValue(boundExpression1)) Then
				Dim flag As Boolean = LocalRewriter.HasValue(boundExpression1)
				Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = Nothing
				If (Not flag) Then
					Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
					Dim boundExpression5 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.CaptureNullableIfNeeded(boundExpression1, synthesizedLocal, boundExpression4, True)
					boundExpression3 = Me.NullableHasValue(If(boundExpression4, boundExpression5))
					boundExpression = Me.WrapInNullable(Me.NullableValueOrDefault(boundExpression5), boundExpression5.Type)
				Else
					boundExpression = boundExpression1
				End If
				boundExpression = Me.RewriteNullableConversion(inConversionOpt, boundExpression)
				Dim method As MethodSymbol = [call].Method
				Dim receiverOpt As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = [call].ReceiverOpt
				Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpression)
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundExpression6 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = [call].Update(method, Nothing, receiverOpt, boundExpressions, bitVector, [call].ConstantValueOpt, [call].IsLValue, [call].SuppressObjectClone, [call].Type)
				boundExpression6 = Me.RewriteNullableConversion(outConversionOpt, boundExpression6)
				If (Not flag) Then
					Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.MakeTernaryConditionalExpression(node.Syntax, boundExpression3, boundExpression6, boundExpression2)
					If (synthesizedLocal IsNot Nothing) Then
						boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(node.Syntax, ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal), ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty, boundSequence, boundSequence.Type, False)
					End If
					boundNode = boundSequence
				Else
					boundNode = boundExpression6
				End If
			Else
				boundNode = boundExpression2
			End If
			Return boundNode
		End Function

		Private Function RewriteNumericOrBooleanToDecimalConversion(ByVal node As BoundConversion, ByVal underlyingTypeFrom As TypeSymbol, ByVal typeTo As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim specialTypeMember As MethodSymbol
			Dim specialMember As Microsoft.CodeAnalysis.SpecialMember
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim bitVector As Microsoft.CodeAnalysis.BitVector
			Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node
			If (Not underlyingTypeFrom.IsBooleanType()) Then
				Select Case underlyingTypeFrom.SpecialType
					Case SpecialType.System_SByte
					Case SpecialType.System_Byte
					Case SpecialType.System_Int16
					Case SpecialType.System_UInt16
					Case SpecialType.System_Int32
						specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__CtorInt32
						Exit Select
					Case SpecialType.System_UInt32
						specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__CtorUInt32
						Exit Select
					Case SpecialType.System_Int64
						specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__CtorInt64
						Exit Select
					Case SpecialType.System_UInt64
						specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__CtorUInt64
						Exit Select
					Case SpecialType.System_Decimal
						boundExpression = boundCall
						Return boundExpression
					Case SpecialType.System_Single
						specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__CtorSingle
						Exit Select
					Case SpecialType.System_Double
						specialMember = Microsoft.CodeAnalysis.SpecialMember.System_Decimal__CtorDouble
						Exit Select
					Case Else
						boundExpression = boundCall
						Return boundExpression
				End Select
				specialTypeMember = DirectCast(Me.ContainingAssembly.GetSpecialTypeMember(specialMember), MethodSymbol)
				If (Me.ReportMissingOrBadRuntimeHelper(node, specialMember, specialTypeMember)) Then
					specialTypeMember = Nothing
				End If
			Else
				specialTypeMember = DirectCast(Me.Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDecimalBoolean), MethodSymbol)
				If (Me.ReportMissingOrBadRuntimeHelper(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToDecimalBoolean, specialTypeMember)) Then
					specialTypeMember = Nothing
				End If
			End If
			If (specialTypeMember IsNot Nothing) Then
				Dim operand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Operand
				Dim type As TypeSymbol = operand.Type
				Dim parameters As ImmutableArray(Of ParameterSymbol) = specialTypeMember.Parameters
				If (CObj(type) <> CObj(parameters(0).Type)) Then
					conversionKind = If(Not type.IsEnumType(), Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNumeric, Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Widening] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Numeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions)
					Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
					Dim checked As Boolean = node.Checked
					Dim explicitCastInCode As Boolean = node.ExplicitCastInCode
					parameters = specialTypeMember.Parameters
					operand = New BoundConversion(syntax, operand, conversionKind, checked, explicitCastInCode, parameters(0).Type, False)
				End If
				If (specialTypeMember.MethodKind <> MethodKind.Constructor) Then
					Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
					Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(operand)
					Dim returnType As TypeSymbol = specialTypeMember.ReturnType
					bitVector = New Microsoft.CodeAnalysis.BitVector()
					boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntaxNode, specialTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)
				Else
					Dim syntax1 As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
					Dim boundExpressions1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(operand)
					bitVector = New Microsoft.CodeAnalysis.BitVector()
					boundCall = New BoundObjectCreationExpression(syntax1, specialTypeMember, boundExpressions1, Nothing, typeTo, False, bitVector)
				End If
			End If
			boundExpression = boundCall
			Return boundExpression
		End Function

		Private Function RewriteObjectBinaryOperator(ByVal node As BoundBinaryOperator, ByVal member As WellKnownMember) As BoundExpression
			Dim left As BoundExpression = node.Left
			Dim right As BoundExpression = node.Right
			Dim boundCall As BoundExpression = node
			Dim wellKnownTypeMember As MethodSymbol = DirectCast(Me.Compilation.GetWellKnownTypeMember(member), MethodSymbol)
			If (Not Me.ReportMissingOrBadRuntimeHelper(node, member, wellKnownTypeMember)) Then
				Dim syntax As SyntaxNode = node.Syntax
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(left, right)
				Dim returnType As TypeSymbol = wellKnownTypeMember.ReturnType
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, wellKnownTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, True, False, bitVector)
			End If
			Return boundCall
		End Function

		Private Function RewriteObjectComparisonOperator(ByVal node As BoundBinaryOperator, ByVal member As WellKnownMember) As BoundExpression
			Dim boundCall As BoundExpression = node
			Dim left As BoundExpression = node.Left
			Dim right As BoundExpression = node.Right
			Dim operatorKind As Boolean = CInt((node.OperatorKind And BinaryOperatorKind.CompareText)) <> 0
			Dim wellKnownTypeMember As MethodSymbol = DirectCast(Me.Compilation.GetWellKnownTypeMember(member), MethodSymbol)
			If (Not Me.ReportMissingOrBadRuntimeHelper(node, member, wellKnownTypeMember)) Then
				Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = Microsoft.CodeAnalysis.ConstantValue.Create(operatorKind)
				Dim parameters As ImmutableArray(Of ParameterSymbol) = wellKnownTypeMember.Parameters
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(left, right, New BoundLiteral(syntaxNode, constantValue, parameters(2).Type))
				Dim returnType As TypeSymbol = wellKnownTypeMember.ReturnType
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, wellKnownTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, True, False, bitVector)
				If (Me._inExpressionLambda AndAlso wellKnownTypeMember.ReturnType.IsObjectType() AndAlso node.Type.IsBooleanType()) Then
					boundCall = New BoundConversion(node.Syntax, boundCall, ConversionKind.NarrowingBoolean, node.Checked, False, node.Type, False)
				End If
			End If
			Return boundCall
		End Function

		Public Function RewriteObjectInitializerExpression(ByVal node As BoundObjectInitializerExpression, ByVal objectCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal rewrittenObjectCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As BoundNode
			Dim boundSequence As BoundNode
			Dim placeholderReplacement As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim specialType As TypeSymbol
			Dim empty As ImmutableArray(Of LocalSymbol)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim initializers As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim placeholderOpt As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim type As TypeSymbol = node.Type
			Dim length As Integer = node.Initializers.Length
			Dim syntax As SyntaxNode = node.Syntax
			If (Not node.CreateTemporaryLocalForInitialization) Then
				placeholderReplacement = Me(node.PlaceholderOpt)
				specialType = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Void)
				empty = ImmutableArray(Of LocalSymbol).Empty
				boundExpression = Nothing
			Else
				Dim synthesizedLocal As LocalSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
				specialType = type
				empty = ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal)
				If (Me._inExpressionLambda) Then
					placeholderOpt = node.PlaceholderOpt
				Else
					placeholderOpt = New BoundLocal(syntax, synthesizedLocal, type)
				End If
				placeholderReplacement = placeholderOpt
				boundExpression = placeholderReplacement.MakeRValue()
				Me.AddPlaceholderReplacement(node.PlaceholderOpt, placeholderReplacement)
			End If
			Dim boundAssignmentOperator(length + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			boundAssignmentOperator(0) = New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(syntax, placeholderReplacement, Me.GenerateObjectCloneIfNeeded(objectCreationExpression, rewrittenObjectCreationExpression), True, type, False)
			Dim num As Integer = length - 1
			Dim num1 As Integer = 0
			Do
				If (Not Me._inExpressionLambda) Then
					initializers = node.Initializers
					boundAssignmentOperator(num1 + 1) = Me.VisitExpressionNode(initializers(num1))
				Else
					initializers = node.Initializers
					Dim item As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator = DirectCast(initializers(num1), Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator)
					boundAssignmentOperator(num1 + 1) = item.Update(item.Left, item.LeftOnTheRightOpt, Me.VisitExpressionNode(item.Right), True, item.Type)
				End If
				num1 = num1 + 1
			Loop While num1 <= num
			If (node.CreateTemporaryLocalForInitialization) Then
				Me.RemovePlaceholderReplacement(node.PlaceholderOpt)
			End If
			If (Not Me._inExpressionLambda) Then
				boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(syntax, empty, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundAssignmentOperator), boundExpression, specialType, False)
			Else
				Dim boundExpressionArray(length - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim num2 As Integer = length - 1
				Dim num3 As Integer = 0
				Do
					boundExpressionArray(num3) = boundAssignmentOperator(num3 + 1)
					num3 = num3 + 1
				Loop While num3 <= num2
				boundSequence = Me.ReplaceObjectOrCollectionInitializer(rewrittenObjectCreationExpression, node.Update(node.CreateTemporaryLocalForInitialization, node.Binder, node.PlaceholderOpt, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpressionArray), node.Type))
			End If
			Return boundSequence
		End Function

		Private Function RewriteObjectShortCircuitOperator(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator) As BoundExpression
			Dim bitVector As Microsoft.CodeAnalysis.BitVector
			Dim boundDirectCast As BoundExpression = node
			Dim left As BoundExpression = node.Left
			Dim right As BoundExpression = node.Right
			If (Not left.Type.IsObjectType() OrElse Not right.Type.IsObjectType()) Then
				Throw ExceptionUtilities.Unreachable
			End If
			Dim operand As BoundExpression = left
			Dim boundCall As BoundExpression = right
			If (operand.Kind = BoundKind.[DirectCast]) Then
				Dim boundDirectCast1 As Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast = DirectCast(operand, Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast)
				If (boundDirectCast1.Operand.Type.IsBooleanType()) Then
					operand = boundDirectCast1.Operand
				End If
			End If
			If (boundCall.Kind = BoundKind.[DirectCast]) Then
				Dim boundDirectCast2 As Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast = DirectCast(boundCall, Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast)
				If (boundDirectCast2.Operand.Type.IsBooleanType()) Then
					boundCall = boundDirectCast2.Operand
				End If
			End If
			If (operand = left OrElse boundCall = right) Then
				Dim wellKnownTypeMember As MethodSymbol = DirectCast(Me.Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToBooleanObject), MethodSymbol)
				If (Not Me.ReportMissingOrBadRuntimeHelper(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToBooleanObject, wellKnownTypeMember)) Then
					If (operand = left) Then
						Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
						Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(operand)
						Dim returnType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = wellKnownTypeMember.ReturnType
						bitVector = New Microsoft.CodeAnalysis.BitVector()
						operand = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, wellKnownTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)
					End If
					If (boundCall = right) Then
						Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
						Dim boundExpressions1 As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(boundCall)
						Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = wellKnownTypeMember.ReturnType
						bitVector = New Microsoft.CodeAnalysis.BitVector()
						boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntaxNode, wellKnownTypeMember, Nothing, Nothing, boundExpressions1, Nothing, typeSymbol, False, False, bitVector)
					End If
				End If
			End If
			If (operand <> left AndAlso boundCall <> right) Then
				Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator(node.Syntax, node.OperatorKind And BinaryOperatorKind.OpMask, operand, boundCall, False, operand.Type, False)
				boundDirectCast = New Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast(node.Syntax, boundBinaryOperator, ConversionKind.WideningValue, node.Type, False)
			End If
			Return boundDirectCast
		End Function

		Private Function RewriteObjectUnaryOperator(ByVal node As BoundUnaryOperator) As BoundExpression
			Dim wellKnownMember As Microsoft.CodeAnalysis.WellKnownMember
			Dim boundCall As BoundExpression = node
			Dim operatorKind As UnaryOperatorKind = node.OperatorKind And UnaryOperatorKind.[Not]
			If (operatorKind <> UnaryOperatorKind.Plus) Then
				wellKnownMember = If(operatorKind <> UnaryOperatorKind.Minus, Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__NotObjectObject, Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__NegateObjectObject)
			Else
				wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__PlusObjectObject
			End If
			Dim wellKnownTypeMember As MethodSymbol = DirectCast(Me.Compilation.GetWellKnownTypeMember(wellKnownMember), MethodSymbol)
			If (Not Me.ReportMissingOrBadRuntimeHelper(node, wellKnownMember, wellKnownTypeMember)) Then
				Dim syntax As SyntaxNode = node.Syntax
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(node.Operand)
				Dim returnType As TypeSymbol = wellKnownTypeMember.ReturnType
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, wellKnownTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)
			End If
			Return boundCall
		End Function

		Private Function RewritePowOperator(ByVal node As BoundBinaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (Not Me._inExpressionLambda) Then
				Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node
				Dim left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Left
				Dim right As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Right
				If (node.Type.IsDoubleType() AndAlso left.Type.IsDoubleType() AndAlso right.Type.IsDoubleType()) Then
					Dim wellKnownTypeMember As MethodSymbol = DirectCast(Me.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Math__PowDoubleDouble), MethodSymbol)
					If (Not Me.ReportMissingOrBadRuntimeHelper(node, WellKnownMember.System_Math__PowDoubleDouble, wellKnownTypeMember)) Then
						Dim syntax As SyntaxNode = node.Syntax
						Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(left, right)
						Dim returnType As TypeSymbol = wellKnownTypeMember.ReturnType
						Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
						boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, wellKnownTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)
					End If
				End If
				boundExpression = boundCall
			Else
				boundExpression = node
			End If
			Return boundExpression
		End Function

		Private Function RewritePropertyAssignmentAsSetCall(ByVal node As BoundAssignmentOperator, ByVal setNode As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim kind As BoundKind = setNode.Kind
			If (kind = BoundKind.PropertyAccess) Then
				boundExpression = Me.RewritePropertyAssignmentAsSetCall(node, DirectCast(setNode, BoundPropertyAccess))
			Else
				If (kind <> BoundKind.XmlMemberAccess) Then
					Throw ExceptionUtilities.UnexpectedValue(setNode.Kind)
				End If
				boundExpression = Me.RewritePropertyAssignmentAsSetCall(node, DirectCast(setNode, BoundXmlMemberAccess).MemberAccess)
			End If
			Return boundExpression
		End Function

		Private Function RewritePropertyAssignmentAsSetCall(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator, ByVal setNode As BoundPropertyAccess) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = setNode.PropertySymbol
			Dim mostDerivedSetMethod As MethodSymbol = propertySymbol.GetMostDerivedSetMethod()
			If (mostDerivedSetMethod IsNot Nothing) Then
				boundAssignmentOperator = Me.RewriteReceiverArgumentsAndGenerateAccessorCall(node.Syntax, mostDerivedSetMethod, setNode.ReceiverOpt, Microsoft.CodeAnalysis.ImmutableArrayExtensions.Concat(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(setNode.Arguments, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(node.Right)), node.ConstantValueOpt, False, False, mostDerivedSetMethod.ReturnType)
			Else
				Dim associatedField As FieldSymbol = propertySymbol.AssociatedField
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(setNode.ReceiverOpt)
				Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(setNode.Syntax, boundExpression, associatedField, True, associatedField.Type, False)
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(node.Right)
				boundAssignmentOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(node.Syntax, boundFieldAccess, boundExpression1, node.SuppressObjectClone, node.Type, False)
			End If
			Return boundAssignmentOperator
		End Function

		Friend Shared Function RewriteQueryLambda(ByVal rewrittenBody As BoundStatement, ByVal originalNode As BoundQueryLambda) As Microsoft.CodeAnalysis.VisualBasic.BoundLambda
			Dim syntax As SyntaxNode = originalNode.Syntax
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = (New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundStatement)(rewrittenBody), False)).MakeCompilerGenerated()
			Dim boundLambda As Microsoft.CodeAnalysis.VisualBasic.BoundLambda = New Microsoft.CodeAnalysis.VisualBasic.BoundLambda(originalNode.Syntax, originalNode.LambdaSymbol, boundBlock, ImmutableBindingDiagnostic(Of AssemblySymbol).Empty, Nothing, ConversionKind.DelegateRelaxationLevelNone, MethodConversionKind.Identity, False)
			boundLambda.MakeCompilerGenerated()
			Return boundLambda
		End Function

		Private Function RewriteReceiverArgumentsAndGenerateAccessorCall(ByVal syntax As SyntaxNode, ByVal methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal receiverOpt As BoundExpression, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal constantValueOpt As ConstantValue, ByVal isLValue As Boolean, ByVal suppressObjectClone As Boolean, ByVal type As TypeSymbol) As BoundExpression
			LocalRewriter.UpdateMethodAndArgumentsIfReducedFromMethod(methodSymbol, receiverOpt, arguments)
			Dim synthesizedLocals As ImmutableArray(Of SynthesizedLocal) = New ImmutableArray(Of SynthesizedLocal)()
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = New ImmutableArray(Of BoundExpression)()
			receiverOpt = Me.VisitExpressionNode(receiverOpt)
			arguments = Me.RewriteCallArguments(arguments, methodSymbol.Parameters, synthesizedLocals, boundExpressions, False)
			Dim boundCall As BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, methodSymbol, Nothing, receiverOpt, arguments, constantValueOpt, isLValue, suppressObjectClone, type, False)
			If (Not synthesizedLocals.IsDefault) Then
				boundCall = If(Not methodSymbol.IsSub, New BoundSequence(syntax, StaticCast(Of LocalSymbol).From(Of SynthesizedLocal)(synthesizedLocals), ImmutableArray(Of BoundExpression).Empty, boundCall, boundCall.Type, False), New BoundSequence(syntax, StaticCast(Of LocalSymbol).From(Of SynthesizedLocal)(synthesizedLocals), ImmutableArray.Create(Of BoundExpression)(boundCall), Nothing, boundCall.Type, False))
			End If
			Return boundCall
		End Function

		Private Function RewriteReferenceTypeToCharArrayRankOneConversion(ByVal node As BoundConversion, ByVal typeFrom As TypeSymbol, ByVal typeTo As TypeSymbol) As BoundExpression
			Dim boundCall As BoundExpression = node
			Dim wellKnownTypeMember As MethodSymbol = DirectCast(Me.Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharArrayRankOneObject), MethodSymbol)
			If (Not Me.ReportMissingOrBadRuntimeHelper(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToCharArrayRankOneObject, wellKnownTypeMember)) Then
				Dim operand As BoundExpression = node.Operand
				If (Not operand.Type.IsObjectType()) Then
					Dim type As TypeSymbol = wellKnownTypeMember.Parameters(0).Type
					Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me.GetNewCompoundUseSiteInfo()
					operand = New BoundDirectCast(operand.Syntax, operand, Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(operand.Type, type, newCompoundUseSiteInfo), type, False)
					Me._diagnostics.Add(node, newCompoundUseSiteInfo)
				End If
				Dim syntax As SyntaxNode = node.Syntax
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(operand)
				Dim returnType As TypeSymbol = wellKnownTypeMember.ReturnType
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, wellKnownTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)
			End If
			Return boundCall
		End Function

		Private Function RewriteReturnStatement(ByVal node As BoundReturnStatement) As BoundStatement
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundStatement Microsoft.CodeAnalysis.VisualBasic.LocalRewriter::RewriteReturnStatement(Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundStatement RewriteReturnStatement(Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Function RewriteSelectExpression(ByVal generateUnstructuredExceptionHandlingResumeCode As Boolean, ByVal selectExpressionStmt As BoundExpressionStatement, <Out> ByRef rewrittenSelectExpression As BoundExpression, <Out> ByRef tempLocals As ImmutableArray(Of LocalSymbol), ByVal statementBuilder As ArrayBuilder(Of BoundStatement), ByVal caseBlocks As ImmutableArray(Of BoundCaseBlock), ByVal recommendSwitchTable As Boolean, <Out> ByRef endSelectResumeLabel As BoundLabelStatement) As BoundExpressionStatement
			Dim flag As Boolean
			Dim syntax As SyntaxNode = selectExpressionStmt.Syntax
			If (Not generateUnstructuredExceptionHandlingResumeCode) Then
				endSelectResumeLabel = Nothing
			Else
				Me.RegisterUnstructuredExceptionHandlingResumeTarget(syntax, True, statementBuilder)
				endSelectResumeLabel = Me.RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(syntax)
			End If
			rewrittenSelectExpression = Me.VisitExpressionNode(selectExpressionStmt.Expression)
			If (Not System.Linq.ImmutableArrayExtensions.Any(Of BoundCaseBlock)(caseBlocks)) Then
				flag = False
			Else
				flag = If(Not recommendSwitchTable, True, rewrittenSelectExpression.Kind <> BoundKind.Local)
			End If
			If (Not flag) Then
				tempLocals = ImmutableArray(Of LocalSymbol).Empty
			Else
				Dim type As TypeSymbol = rewrittenSelectExpression.Type
				Dim selectStatement As SelectStatementSyntax = DirectCast(syntax.Parent, SelectBlockSyntax).SelectStatement
				Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, type, SynthesizedLocalKind.SelectCaseValue, selectStatement, False)
				tempLocals = ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal)
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(rewrittenSelectExpression.Syntax, synthesizedLocal, type)
				statementBuilder.Add((New BoundAssignmentOperator(syntax, boundLocal, rewrittenSelectExpression, True, type, False)).ToStatement().MakeCompilerGenerated())
				rewrittenSelectExpression = boundLocal.MakeRValue()
			End If
			Return selectExpressionStmt.Update(rewrittenSelectExpression)
		End Function

		Protected Function RewriteSelectStatement(ByVal node As BoundSelectStatement, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal selectExpressionStmt As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement, ByVal exprPlaceholderOpt As BoundRValuePlaceholder, ByVal caseBlocks As ImmutableArray(Of BoundCaseBlock), ByVal recommendSwitchTable As Boolean, ByVal exitLabel As LabelSymbol) As BoundNode
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement).GetInstance()
			Dim instrument As Boolean = Me(node)
			If (instrument) Then
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me._instrumenterOpt.CreateSelectStatementPrologue(node)
				If (boundStatement IsNot Nothing) Then
					instance.Add(boundStatement)
				End If
			End If
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
			Dim localSymbols As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)()
			Dim boundLabelStatement As Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement = Nothing
			Dim flag As Boolean = Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)
			Dim boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = Me.RewriteSelectExpression(flag, selectExpressionStmt, boundExpression, localSymbols, instance, caseBlocks, recommendSwitchTable, boundLabelStatement)
			If (exprPlaceholderOpt IsNot Nothing) Then
				Me.AddPlaceholderReplacement(exprPlaceholderOpt, boundExpression)
			End If
			If (Not System.Linq.ImmutableArrayExtensions.Any(Of BoundCaseBlock)(caseBlocks)) Then
				instance.Add(boundExpressionStatement)
			ElseIf (Not recommendSwitchTable) Then
				Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Nothing
				instance.Add(Me.RewriteCaseBlocksRecursive(node, flag, caseBlocks, 0, localSymbol))
				If (localSymbol IsNot Nothing) Then
					localSymbols = localSymbols.Add(localSymbol)
				End If
				instance.Add(New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(syntaxNode, exitLabel))
			Else
				If (boundExpression.Type.IsStringType()) Then
					Me.EnsureStringHashFunction(node)
				End If
				instance.Add(node.Update(boundExpressionStatement, exprPlaceholderOpt, Me.VisitList(Of BoundCaseBlock)(caseBlocks), True, exitLabel))
			End If
			If (exprPlaceholderOpt IsNot Nothing) Then
				Me.RemovePlaceholderReplacement(exprPlaceholderOpt)
			End If
			Dim boundStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = boundLabelStatement
			If (instrument) Then
				boundStatement1 = Me._instrumenterOpt.InstrumentSelectStatementEpilogue(node, boundStatement1)
			End If
			If (boundStatement1 IsNot Nothing) Then
				instance.Add(boundStatement1)
			End If
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
			Return (New BoundBlock(syntaxNode, statementSyntaxes, localSymbols, instance.ToImmutableAndFree(), False)).MakeCompilerGenerated()
		End Function

		Private Function RewriteSingleUsingToTryFinally(ByVal node As BoundUsingStatement, ByVal resourceIndex As Integer, ByVal localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, ByVal initializationExpression As BoundExpression, ByRef placeholderInfo As ValueTuple(Of BoundRValuePlaceholder, BoundExpression, BoundExpression), ByVal currentBody As Microsoft.CodeAnalysis.VisualBasic.BoundBlock) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim boundStatements As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Dim syntax As UsingBlockSyntax = DirectCast(node.Syntax, UsingBlockSyntax)
			Dim type As TypeSymbol = localSymbol.Type
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, localSymbol, True, type)
			Dim item1 As BoundRValuePlaceholder = placeholderInfo.Item1
			Dim item2 As BoundExpression = placeholderInfo.Item2
			Dim item3 As BoundExpression = placeholderInfo.Item3
			Me.AddPlaceholderReplacement(item1, boundLocal.MakeRValue())
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(LocalRewriter.Concat(currentBody, SyntheticBoundNodeFactory.HiddenSequencePoint(Nothing)), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
			Dim statement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = (New BoundAssignmentOperator(syntax, boundLocal, Me.VisitAndGenerateObjectCloneIfNeeded(initializationExpression, True), True, type, False)).ToStatement()
			Dim instrument As Boolean = Me(node)
			If (instrument) Then
				statement = Me._instrumenterOpt.InstrumentUsingStatementResourceCapture(node, resourceIndex, statement)
			End If
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.GenerateDisposeCallForForeachAndUsing(syntax, boundLocal, Me.VisitExpressionNode(item3), True, Me.VisitExpressionNode(item2))
			Dim boundStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Nothing
			If (instrument) Then
				boundStatement1 = Me._instrumenterOpt.CreateUsingStatementDisposePrologue(node)
			End If
			boundStatements = If(boundStatement1 Is Nothing, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(boundStatement), ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(boundStatement1, boundStatement))
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
			Dim boundBlock1 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol).Empty, boundStatements, False)
			Dim boundStatement2 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.RewriteTryStatement(syntax, boundBlock, ImmutableArray(Of BoundCatchBlock).Empty, boundBlock1, Nothing)
			statementSyntaxes = New SyntaxList(Of StatementSyntax)()
			boundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol).Empty, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(statement, boundStatement2), False)
			Me.RemovePlaceholderReplacement(item1)
			Return boundBlock
		End Function

		Private Function RewriteStringComparisonOperator(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator) As BoundExpression
			Dim boundBinaryOperator As BoundExpression = node
			Dim left As BoundExpression = node.Left
			Dim right As BoundExpression = node.Right
			Dim operatorKind As Boolean = CInt((node.OperatorKind And BinaryOperatorKind.CompareText)) <> 0
			Dim wellKnownType As NamedTypeSymbol = Me.Compilation.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.Microsoft_VisualBasic_CompilerServices_EmbeddedOperators)
			Dim wellKnownMember As Microsoft.CodeAnalysis.WellKnownMember = If(Not wellKnownType.IsErrorType() OrElse Not TypeOf wellKnownType Is MissingMetadataTypeSymbol, Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_EmbeddedOperators__CompareStringStringStringBoolean, Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__CompareStringStringStringBoolean)
			Dim wellKnownTypeMember As MethodSymbol = DirectCast(Me.Compilation.GetWellKnownTypeMember(wellKnownMember), MethodSymbol)
			If (Not Me.ReportMissingOrBadRuntimeHelper(node, wellKnownMember, wellKnownTypeMember)) Then
				Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = Microsoft.CodeAnalysis.ConstantValue.Create(operatorKind)
				Dim parameters As ImmutableArray(Of ParameterSymbol) = wellKnownTypeMember.Parameters
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(left, right, New BoundLiteral(syntaxNode, constantValue, parameters(2).Type))
				Dim returnType As TypeSymbol = wellKnownTypeMember.ReturnType
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, wellKnownTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)
				boundBinaryOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator(node.Syntax, node.OperatorKind And BinaryOperatorKind.OpMask, boundCall, New BoundLiteral(node.Syntax, Microsoft.CodeAnalysis.ConstantValue.Create(0), wellKnownTypeMember.ReturnType), False, node.Type, False)
			End If
			Return boundBinaryOperator
		End Function

		Private Function RewriteStringConcatenationFourExprs(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal factory As SyntheticBoundNodeFactory, ByVal loweredFirst As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal loweredSecond As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal loweredThird As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal loweredFourth As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim specialTypeMember As MethodSymbol = DirectCast(Me.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringStringStringString), MethodSymbol)
			If (Me.ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_String__ConcatStringStringStringString, specialTypeMember)) Then
				boundExpression = node
			Else
				boundExpression = factory.[Call](Nothing, specialTypeMember, New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { loweredFirst, loweredSecond, loweredThird, loweredFourth })
			End If
			Return boundExpression
		End Function

		Private Function RewriteStringConcatenationManyExprs(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal factory As SyntheticBoundNodeFactory, ByVal loweredArgs As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim specialTypeMember As MethodSymbol = DirectCast(Me.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringArray), MethodSymbol)
			If (Me.ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_String__ConcatStringArray, specialTypeMember)) Then
				boundExpression = node
			Else
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = factory.Array(node.Type, loweredArgs)
				boundExpression = factory.[Call](Nothing, specialTypeMember, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpression1))
			End If
			Return boundExpression
		End Function

		Private Shared Function RewriteStringConcatenationOneExpr(ByVal factory As SyntheticBoundNodeFactory, ByVal loweredOperand As BoundExpression) As BoundExpression
			Return factory.BinaryConditional(loweredOperand, factory.Literal(""))
		End Function

		Private Function RewriteStringConcatenationThreeExprs(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal factory As SyntheticBoundNodeFactory, ByVal loweredFirst As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal loweredSecond As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal loweredThird As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim specialTypeMember As MethodSymbol = DirectCast(Me.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringStringString), MethodSymbol)
			If (Me.ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_String__ConcatStringStringString, specialTypeMember)) Then
				boundExpression = node
			Else
				boundExpression = factory.[Call](Nothing, specialTypeMember, New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { loweredFirst, loweredSecond, loweredThird })
			End If
			Return boundExpression
		End Function

		Private Function RewriteStringConcatenationTwoExprs(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal factory As SyntheticBoundNodeFactory, ByVal loweredLeft As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal loweredRight As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim specialTypeMember As MethodSymbol = DirectCast(Me.GetSpecialTypeMember(SpecialMember.System_String__ConcatStringString), MethodSymbol)
			If (Me.ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_String__ConcatStringString, specialTypeMember)) Then
				boundExpression = node
			Else
				boundExpression = factory.[Call](Nothing, specialTypeMember, New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { loweredLeft, loweredRight })
			End If
			Return boundExpression
		End Function

		Private Function RewriteToStringConversion(ByVal node As BoundConversion, ByVal underlyingTypeFrom As TypeSymbol, ByVal typeTo As TypeSymbol) As BoundExpression
			Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind
			Dim bitVector As Microsoft.CodeAnalysis.BitVector
			Dim boundCall As BoundExpression = node
			Dim wellKnownTypeMember As MethodSymbol = Nothing
			If (Not underlyingTypeFrom.IsCharSZArray()) Then
				Dim wellKnownMember As Microsoft.CodeAnalysis.WellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Count
				Select Case underlyingTypeFrom.SpecialType
					Case SpecialType.System_Boolean
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringBoolean
						GoTo Label0
					Case SpecialType.System_Char
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringChar
						GoTo Label0
					Case SpecialType.System_SByte
					Case SpecialType.System_Int16
					Case SpecialType.System_Int32
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringInt32
						GoTo Label0
					Case SpecialType.System_Byte
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringByte
						GoTo Label0
					Case SpecialType.System_UInt16
					Case SpecialType.System_UInt32
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringUInt32
						GoTo Label0
					Case SpecialType.System_Int64
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringInt64
						GoTo Label0
					Case SpecialType.System_UInt64
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringUInt64
						GoTo Label0
					Case SpecialType.System_Decimal
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringDecimal
						GoTo Label0
					Case SpecialType.System_Single
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringSingle
						GoTo Label0
					Case SpecialType.System_Double
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringDouble
						GoTo Label0
					Case SpecialType.System_String
					Case SpecialType.System_IntPtr
					Case SpecialType.System_UIntPtr
					Case SpecialType.System_Array
					Case SpecialType.System_Collections_IEnumerable
					Case SpecialType.System_Collections_Generic_IEnumerable_T
					Case SpecialType.System_Collections_Generic_IList_T
					Case SpecialType.System_Collections_Generic_ICollection_T
					Case SpecialType.System_Collections_IEnumerator
					Case SpecialType.System_Collections_Generic_IEnumerator_T
					Case SpecialType.System_Collections_Generic_IReadOnlyList_T
					Case SpecialType.System_Collections_Generic_IReadOnlyCollection_T
					Case SpecialType.System_Nullable_T
					Label0:
						If (wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Count) Then
							Exit Select
						End If
						wellKnownTypeMember = DirectCast(Me.Compilation.GetWellKnownTypeMember(wellKnownMember), MethodSymbol)
						If (Not Me.ReportMissingOrBadRuntimeHelper(node, wellKnownMember, wellKnownTypeMember)) Then
							Exit Select
						End If
						wellKnownTypeMember = Nothing
						Exit Select
					Case SpecialType.System_DateTime
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Conversions__ToStringDateTime
						GoTo Label0
					Case Else
						GoTo Label0
				End Select
			Else
				wellKnownTypeMember = DirectCast(Me.ContainingAssembly.GetSpecialTypeMember(SpecialMember.System_String__CtorSZArrayChar), MethodSymbol)
				If (Me.ReportMissingOrBadRuntimeHelper(node, SpecialMember.System_String__CtorSZArrayChar, wellKnownTypeMember)) Then
					wellKnownTypeMember = Nothing
				End If
			End If
			If (wellKnownTypeMember IsNot Nothing) Then
				Dim operand As BoundExpression = node.Operand
				Dim type As TypeSymbol = operand.Type
				If (Not type.IsSameTypeIgnoringAll(wellKnownTypeMember.Parameters(0).Type)) Then
					conversionKind = If(Not type.IsEnumType(), Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNumeric, Microsoft.CodeAnalysis.VisualBasic.ConversionKind.[Widening] Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.Numeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.WideningNumeric Or Microsoft.CodeAnalysis.VisualBasic.ConversionKind.InvolvesEnumTypeConversions)
					Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
					Dim checked As Boolean = node.Checked
					Dim explicitCastInCode As Boolean = node.ExplicitCastInCode
					Dim parameters As ImmutableArray(Of ParameterSymbol) = wellKnownTypeMember.Parameters
					operand = New BoundConversion(syntax, operand, conversionKind, checked, explicitCastInCode, parameters(0).Type, False)
				End If
				If (wellKnownTypeMember.MethodKind <> MethodKind.Constructor) Then
					Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
					Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(operand)
					Dim returnType As TypeSymbol = wellKnownTypeMember.ReturnType
					bitVector = New Microsoft.CodeAnalysis.BitVector()
					boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntaxNode, wellKnownTypeMember, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)
				Else
					Dim syntax1 As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
					Dim boundExpressions1 As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(operand)
					bitVector = New Microsoft.CodeAnalysis.BitVector()
					boundCall = New BoundObjectCreationExpression(syntax1, wellKnownTypeMember, boundExpressions1, Nothing, typeTo, False, bitVector)
				End If
			End If
			Return boundCall
		End Function

		Private Function RewriteTrivialMidAssignment(ByVal node As BoundAssignmentOperator) As BoundExpression
			Dim boundCall As BoundExpression
			Dim right As BoundMidResult = DirectCast(node.Right, BoundMidResult)
			Dim wellKnownTypeMember As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			wellKnownTypeMember = DirectCast(Me.Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_CompilerServices_StringType__MidStmtStr), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			If (Not Me.ReportMissingOrBadRuntimeHelper(node, WellKnownMember.Microsoft_VisualBasic_CompilerServices_StringType__MidStmtStr, wellKnownTypeMember)) Then
				Dim synthesizedLocals As ImmutableArray(Of SynthesizedLocal) = New ImmutableArray(Of SynthesizedLocal)()
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = New ImmutableArray(Of BoundExpression)()
				Dim syntax As SyntaxNode = node.Syntax
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = wellKnownTypeMember
				Dim left As BoundExpression = node.Left
				Dim start As BoundExpression = right.Start
				Dim lengthOpt As BoundExpression = right.LengthOpt
				If (lengthOpt Is Nothing) Then
					lengthOpt = New BoundLiteral(node.Syntax, ConstantValue.Create(2147483647), right.Start.Type)
				End If
				Dim boundExpressions1 As ImmutableArray(Of BoundExpression) = Me.RewriteCallArguments(ImmutableArray.Create(Of BoundExpression)(left, start, lengthOpt, right.Source), wellKnownTypeMember.Parameters, synthesizedLocals, boundExpressions, False)
				Dim returnType As TypeSymbol = wellKnownTypeMember.ReturnType
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, methodSymbol, Nothing, Nothing, boundExpressions1, Nothing, returnType, False, False, bitVector)
			Else
				boundCall = right.Update(Me.VisitExpressionNode(node.Left), Me.VisitExpressionNode(right.Start), Me.VisitExpressionNode(right.LengthOpt), Me.VisitExpressionNode(right.Source), node.Type)
			End If
			Return boundCall
		End Function

		Private Function RewriteTryBlock(ByVal tryStatement As BoundTryStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim tryBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = tryStatement.TryBlock
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.Visit(tryBlock), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
			If (Me(tryStatement) AndAlso TypeOf tryBlock.Syntax Is TryBlockSyntax) Then
				boundBlock = LocalRewriter.PrependWithPrologue(boundBlock, Me._instrumenterOpt.CreateTryBlockPrologue(tryStatement))
			End If
			Return boundBlock
		End Function

		Public Function RewriteTryStatement(ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal tryBlock As BoundBlock, ByVal catchBlocks As ImmutableArray(Of BoundCatchBlock), ByVal finallyBlockOpt As BoundBlock, ByVal exitLabelOpt As LabelSymbol) As BoundStatement
			Dim boundStatementList As BoundStatement
			Dim boundTryStatement As BoundStatement
			Dim enumerator As ImmutableArray(Of BoundCatchBlock).Enumerator
			Dim current As BoundCatchBlock
			If (Not Me.OptimizationLevelIsDebug) Then
				If (Not LocalRewriter.HasSideEffects(tryBlock)) Then
					catchBlocks = ImmutableArray(Of BoundCatchBlock).Empty
				End If
				If (Not LocalRewriter.HasSideEffects(finallyBlockOpt)) Then
					finallyBlockOpt = Nothing
				End If
				If (Not catchBlocks.IsDefaultOrEmpty OrElse finallyBlockOpt IsNot Nothing) Then
					boundTryStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundTryStatement(syntaxNode, tryBlock, catchBlocks, finallyBlockOpt, exitLabelOpt, False)
					enumerator = catchBlocks.GetEnumerator()
					While enumerator.MoveNext()
						current = enumerator.Current
						Me.ReportErrorsOnCatchBlockHelpers(current)
					End While
					boundStatementList = boundTryStatement
					Return boundStatementList
				End If
				If (exitLabelOpt IsNot Nothing) Then
					boundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(syntaxNode, ImmutableArray.Create(Of BoundStatement)(tryBlock, New BoundLabelStatement(syntaxNode, exitLabelOpt)), False)
					Return boundStatementList
				Else
					boundStatementList = tryBlock
					Return boundStatementList
				End If
			End If
			boundTryStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundTryStatement(syntaxNode, tryBlock, catchBlocks, finallyBlockOpt, exitLabelOpt, False)
			enumerator = catchBlocks.GetEnumerator()
			While enumerator.MoveNext()
				current = enumerator.Current
				Me.ReportErrorsOnCatchBlockHelpers(current)
			End While
			boundStatementList = boundTryStatement
			Return boundStatementList
		End Function

		Private Function RewriteTupleConversion(ByVal node As BoundConversion) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim syntax As SyntaxNode = node.Syntax
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(node.Operand)
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(Me.VisitType(node.Type), Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			Return Me.MakeTupleConversion(syntax, boundExpression, namedTypeSymbol, DirectCast(node.ExtendedInfoOpt, BoundConvertedTupleElements))
		End Function

		Private Function RewriteTupleCreationExpression(ByVal node As BoundTupleExpression, ByVal rewrittenArguments As ImmutableArray(Of BoundExpression)) As BoundExpression
			Return Me.MakeTupleCreationExpression(node.Syntax, DirectCast(node.Type, NamedTypeSymbol), rewrittenArguments)
		End Function

		Private Function RewriteUnaryOperator(ByVal node As BoundUnaryOperator) As BoundExpression
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundExpression Microsoft.CodeAnalysis.VisualBasic.LocalRewriter::RewriteUnaryOperator(Microsoft.CodeAnalysis.VisualBasic.BoundUnaryOperator)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundExpression RewriteUnaryOperator(Microsoft.CodeAnalysis.VisualBasic.BoundUnaryOperator)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Function RewriteUnstructuredExceptionHandlingStatementIntoBlock(ByVal node As BoundUnstructuredExceptionHandlingStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim bitVector As Microsoft.CodeAnalysis.BitVector
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim resumeNextLabel As LabelSymbol
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim instance As ArrayBuilder(Of LocalSymbol) = ArrayBuilder(Of LocalSymbol).GetInstance()
			Me._unstructuredExceptionHandling.Context = node
			Me._unstructuredExceptionHandling.ExceptionHandlers = ArrayBuilder(Of BoundGotoStatement).GetInstance()
			Me._unstructuredExceptionHandling.OnErrorResumeNextCount = 0
			Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = New Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory(Me._topMethod, Me._currentMethodOrLambda, node.Syntax, Me._compilationState, Me._diagnostics)
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = syntheticBoundNodeFactory.SpecialType(SpecialType.System_Int32)
			Dim namedTypeSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = syntheticBoundNodeFactory.SpecialType(SpecialType.System_Boolean)
			Me._unstructuredExceptionHandling.ActiveHandlerTemporary = New SynthesizedLocal(Me._topMethod, namedTypeSymbol, SynthesizedLocalKind.OnErrorActiveHandler, DirectCast(syntheticBoundNodeFactory.Syntax, StatementSyntax), False)
			instance.Add(Me._unstructuredExceptionHandling.ActiveHandlerTemporary)
			Me._unstructuredExceptionHandling.ResumeTargetTemporary = New SynthesizedLocal(Me._topMethod, namedTypeSymbol, SynthesizedLocalKind.OnErrorResumeTarget, DirectCast(syntheticBoundNodeFactory.Syntax, StatementSyntax), False)
			instance.Add(Me._unstructuredExceptionHandling.ResumeTargetTemporary)
			If (node.ResumeWithoutLabelOpt IsNot Nothing) Then
				Me._unstructuredExceptionHandling.CurrentStatementTemporary = New SynthesizedLocal(Me._topMethod, namedTypeSymbol, SynthesizedLocalKind.OnErrorCurrentStatement, DirectCast(syntheticBoundNodeFactory.Syntax, StatementSyntax), False)
				instance.Add(Me._unstructuredExceptionHandling.CurrentStatementTemporary)
				Me._unstructuredExceptionHandling.ResumeNextLabel = New Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol("$VB$UnstructuredExceptionHandling_ResumeNext")
				Me._unstructuredExceptionHandling.ResumeLabel = New Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol("$VB$UnstructuredExceptionHandling_Resume")
				Me._unstructuredExceptionHandling.ResumeTargets = ArrayBuilder(Of BoundGotoStatement).GetInstance()
			End If
			Dim boundStatements As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
			boundStatements.Add(DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundBlock))
			If (Me.Instrument) Then
				boundStatements.Add(Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory.HiddenSequencePoint(Nothing))
			End If
			If (Me._unstructuredExceptionHandling.CurrentStatementTemporary IsNot Nothing) Then
				Me.RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, False, boundStatements)
			End If
			Dim generatedLabelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol("$VB$UnstructuredExceptionHandling_Done")
			boundStatements.Add(syntheticBoundNodeFactory.[Goto](generatedLabelSymbol, True))
			Dim generatedLabelSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol("$VB$UnstructuredExceptionHandling_OnErrorFailure")
			If (node.ResumeWithoutLabelOpt IsNot Nothing) Then
				Dim generatedLabelSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol("$VB$UnstructuredExceptionHandling_ResumeSwitchFallThrough")
				Dim item(1 + Me._unstructuredExceptionHandling.ResumeTargets.Count - 1 + 1 - 1) As BoundGotoStatement
				item(0) = syntheticBoundNodeFactory.[Goto](generatedLabelSymbol2, True)
				Dim count As Integer = Me._unstructuredExceptionHandling.ResumeTargets.Count - 1
				Dim num As Integer = 0
				Do
					item(num + 1) = Me._unstructuredExceptionHandling.ResumeTargets(num)
					num = num + 1
				Loop While num <= count
				boundStatements.Add(New BoundUnstructuredExceptionResumeSwitch(node.Syntax, syntheticBoundNodeFactory.Local(Me._unstructuredExceptionHandling.ResumeTargetTemporary, False), syntheticBoundNodeFactory.Label(Me._unstructuredExceptionHandling.ResumeLabel), syntheticBoundNodeFactory.Label(Me._unstructuredExceptionHandling.ResumeNextLabel), Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundGotoStatement)(item), False))
				boundStatements.Add(syntheticBoundNodeFactory.Label(generatedLabelSymbol2))
				boundStatements.Add(syntheticBoundNodeFactory.[Goto](generatedLabelSymbol1, True))
			End If
			Dim generatedLabelSymbol3 As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol("$VB$UnstructuredExceptionHandling_OnError")
			boundStatements.Add(syntheticBoundNodeFactory.Label(generatedLabelSymbol3))
			Dim boundStatements1 As ArrayBuilder(Of BoundStatement) = boundStatements
			Dim syntheticBoundNodeFactory1 As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = syntheticBoundNodeFactory
			Dim boundLocal1 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = syntheticBoundNodeFactory.Local(Me._unstructuredExceptionHandling.ResumeTargetTemporary, True)
			If (Me._unstructuredExceptionHandling.CurrentStatementTemporary Is Nothing) Then
				boundExpression = syntheticBoundNodeFactory.Literal(-1)
			Else
				boundExpression = syntheticBoundNodeFactory.Local(Me._unstructuredExceptionHandling.CurrentStatementTemporary, False)
			End If
			boundStatements1.Add(syntheticBoundNodeFactory1.AssignmentExpression(boundLocal1, boundExpression).ToStatement())
			Dim generatedLabelSymbol4 As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol("$VB$UnstructuredExceptionHandling_OnErrorSwitchFallThrough")
			Dim boundGotoStatementArray(2 + Me._unstructuredExceptionHandling.ExceptionHandlers.Count - 1 + 1 - 1) As BoundGotoStatement
			boundGotoStatementArray(0) = syntheticBoundNodeFactory.[Goto](generatedLabelSymbol4, True)
			Dim boundGotoStatementArray1 As BoundGotoStatement() = boundGotoStatementArray
			Dim syntheticBoundNodeFactory2 As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = syntheticBoundNodeFactory
			If (node.ResumeWithoutLabelOpt IsNot Nothing) Then
				resumeNextLabel = Me._unstructuredExceptionHandling.ResumeNextLabel
			Else
				resumeNextLabel = generatedLabelSymbol4
			End If
			boundGotoStatementArray1(1) = syntheticBoundNodeFactory2.[Goto](resumeNextLabel, True)
			Dim count1 As Integer = Me._unstructuredExceptionHandling.ExceptionHandlers.Count - 1
			Dim num1 As Integer = 0
			Do
				boundGotoStatementArray(2 + num1) = Me._unstructuredExceptionHandling.ExceptionHandlers(num1)
				num1 = num1 + 1
			Loop While num1 <= count1
			Dim boundStatements2 As ArrayBuilder(Of BoundStatement) = boundStatements
			Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
			If (node.ResumeWithoutLabelOpt Is Nothing OrElse Not Me.OptimizationLevelIsDebug) Then
				boundExpression1 = syntheticBoundNodeFactory.Local(Me._unstructuredExceptionHandling.ActiveHandlerTemporary, False)
			Else
				boundExpression1 = syntheticBoundNodeFactory.Conditional(syntheticBoundNodeFactory.Binary(BinaryOperatorKind.GreaterThan, namedTypeSymbol1, syntheticBoundNodeFactory.Local(Me._unstructuredExceptionHandling.ActiveHandlerTemporary, False), syntheticBoundNodeFactory.Literal(-2)), syntheticBoundNodeFactory.Local(Me._unstructuredExceptionHandling.ActiveHandlerTemporary, False), syntheticBoundNodeFactory.Literal(1), namedTypeSymbol)
			End If
			boundStatements2.Add(New BoundUnstructuredExceptionOnErrorSwitch(syntax, boundExpression1, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundGotoStatement)(boundGotoStatementArray), False))
			boundStatements.Add(syntheticBoundNodeFactory.Label(generatedLabelSymbol4))
			boundStatements.Add(syntheticBoundNodeFactory.[Goto](generatedLabelSymbol1, True))
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = syntheticBoundNodeFactory.Block(boundStatements.ToImmutable())
			boundStatements.Clear()
			Dim boundStatements3 As ArrayBuilder(Of BoundStatement) = boundStatements
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
			Dim boundBlock1 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = boundBlock
			Dim syntax1 As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
			If (Me._currentLineTemporary IsNot Nothing) Then
				boundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(node.Syntax, Me._currentLineTemporary, False, Me._currentLineTemporary.Type)
			Else
				boundLocal = Nothing
			End If
			boundStatements3.Add(Me.RewriteTryStatement(syntaxNode, boundBlock1, ImmutableArray.Create(Of BoundCatchBlock)(New BoundCatchBlock(syntax1, Nothing, Nothing, boundLocal, New BoundUnstructuredExceptionHandlingCatchFilter(node.Syntax, syntheticBoundNodeFactory.Local(Me._unstructuredExceptionHandling.ActiveHandlerTemporary, False), syntheticBoundNodeFactory.Local(Me._unstructuredExceptionHandling.ResumeTargetTemporary, False), namedTypeSymbol1, False), syntheticBoundNodeFactory.Block(ImmutableArray.Create(Of BoundStatement)(syntheticBoundNodeFactory.[Goto](generatedLabelSymbol3, True))), False, False)), Nothing, Nothing))
			boundStatements.Add(syntheticBoundNodeFactory.Label(generatedLabelSymbol1))
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = syntheticBoundNodeFactory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__CreateProjectError, False)
			If (methodSymbol IsNot Nothing) Then
				Dim syntaxNode1 As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(syntheticBoundNodeFactory.Literal(-2146828237))
				Dim returnType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = methodSymbol.ReturnType
				bitVector = New Microsoft.CodeAnalysis.BitVector()
				boundStatements.Add(syntheticBoundNodeFactory.[Throw](New BoundCall(syntaxNode1, methodSymbol, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)))
			End If
			boundStatements.Add(syntheticBoundNodeFactory.Label(generatedLabelSymbol))
			Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = syntheticBoundNodeFactory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__ClearProjectError, False)
			If (methodSymbol1 IsNot Nothing) Then
				Dim syntax2 As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = syntheticBoundNodeFactory.Binary(BinaryOperatorKind.NotEquals, namedTypeSymbol1, syntheticBoundNodeFactory.Local(Me._unstructuredExceptionHandling.ResumeTargetTemporary, False), syntheticBoundNodeFactory.Literal(0))
				Dim syntaxNode2 As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim empty As ImmutableArray(Of !0) = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = methodSymbol1.ReturnType
				bitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim statement As BoundExpressionStatement = (New BoundCall(syntaxNode2, methodSymbol1, Nothing, Nothing, empty, Nothing, typeSymbol, False, False, bitVector)).ToStatement()
				Dim boundStatements4 As ImmutableArray(Of BoundStatement) = New ImmutableArray(Of BoundStatement)()
				boundStatements.Add(Me.RewriteIfStatement(syntax2, boundBinaryOperator, statement, Nothing, Nothing, boundStatements4))
			End If
			Me._unstructuredExceptionHandling.Context = Nothing
			Me._unstructuredExceptionHandling.ExceptionHandlers.Free()
			Me._unstructuredExceptionHandling.ExceptionHandlers = Nothing
			If (Me._unstructuredExceptionHandling.ResumeTargets IsNot Nothing) Then
				Me._unstructuredExceptionHandling.ResumeTargets.Free()
				Me._unstructuredExceptionHandling.ResumeTargets = Nothing
			End If
			Me._unstructuredExceptionHandling.ActiveHandlerTemporary = Nothing
			Me._unstructuredExceptionHandling.ResumeTargetTemporary = Nothing
			Me._unstructuredExceptionHandling.CurrentStatementTemporary = Nothing
			Me._unstructuredExceptionHandling.ResumeNextLabel = Nothing
			Me._unstructuredExceptionHandling.ResumeLabel = Nothing
			Me._unstructuredExceptionHandling.OnErrorResumeNextCount = 0
			Return syntheticBoundNodeFactory.Block(instance.ToImmutableAndFree(), boundStatements.ToImmutableAndFree())
		End Function

		Protected Function RewriteWhileStatement(ByVal statement As BoundStatement, ByVal rewrittenCondition As BoundExpression, ByVal rewrittenBody As BoundStatement, ByVal continueLabel As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol, ByVal exitLabel As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol, Optional ByVal loopIfTrue As Boolean = True, Optional ByVal loopResumeLabelOpt As BoundLabelStatement = Nothing, Optional ByVal conditionResumeTargetOpt As ImmutableArray(Of BoundStatement) = Nothing, Optional ByVal afterBodyResumeTargetOpt As BoundStatement = Nothing) As BoundNode
			Dim labelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol = LocalRewriter.GenerateLabel("start")
			Dim syntax As SyntaxNode = statement.Syntax
			Dim instrument As Boolean = Me(statement)
			If (instrument) Then
				Select Case statement.Kind
					Case BoundKind.DoLoopStatement
						afterBodyResumeTargetOpt = Me._instrumenterOpt.InstrumentDoLoopEpilogue(DirectCast(statement, BoundDoLoopStatement), afterBodyResumeTargetOpt)
						Exit Select
					Case BoundKind.WhileStatement
						afterBodyResumeTargetOpt = Me._instrumenterOpt.InstrumentWhileEpilogue(DirectCast(statement, BoundWhileStatement), afterBodyResumeTargetOpt)
						Exit Select
					Case BoundKind.ForToUserDefinedOperators
					Case BoundKind.ForToStatement
						Throw ExceptionUtilities.UnexpectedValue(statement.Kind)
					Case BoundKind.ForEachStatement
						Exit Select
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(statement.Kind)
				End Select
			End If
			rewrittenBody = LocalRewriter.Concat(rewrittenBody, afterBodyResumeTargetOpt)
			If (rewrittenCondition IsNot Nothing AndAlso instrument) Then
				Select Case statement.Kind
					Case BoundKind.DoLoopStatement
						rewrittenCondition = Me._instrumenterOpt.InstrumentDoLoopStatementCondition(DirectCast(statement, BoundDoLoopStatement), rewrittenCondition, Me._currentMethodOrLambda)
						Exit Select
					Case BoundKind.WhileStatement
						rewrittenCondition = Me._instrumenterOpt.InstrumentWhileStatementCondition(DirectCast(statement, BoundWhileStatement), rewrittenCondition, Me._currentMethodOrLambda)
						Exit Select
					Case BoundKind.ForToUserDefinedOperators
					Case BoundKind.ForToStatement
						Throw ExceptionUtilities.UnexpectedValue(statement.Kind)
					Case BoundKind.ForEachStatement
						rewrittenCondition = Me._instrumenterOpt.InstrumentForEachStatementCondition(DirectCast(statement, BoundForEachStatement), rewrittenCondition, Me._currentMethodOrLambda)
						Exit Select
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(statement.Kind)
				End Select
			End If
			Dim boundConditionalGoto As BoundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundConditionalGoto(syntax, rewrittenCondition, loopIfTrue, labelSymbol, False)
			If (Not conditionResumeTargetOpt.IsDefaultOrEmpty) Then
				boundConditionalGoto = New BoundStatementList(boundConditionalGoto.Syntax, conditionResumeTargetOpt.Add(boundConditionalGoto), False)
			End If
			If (instrument) Then
				Select Case statement.Kind
					Case BoundKind.DoLoopStatement
						boundConditionalGoto = Me._instrumenterOpt.InstrumentDoLoopStatementEntryOrConditionalGotoStart(DirectCast(statement, BoundDoLoopStatement), boundConditionalGoto)
						Exit Select
					Case BoundKind.WhileStatement
						boundConditionalGoto = Me._instrumenterOpt.InstrumentWhileStatementConditionalGotoStart(DirectCast(statement, BoundWhileStatement), boundConditionalGoto)
						Exit Select
					Case BoundKind.ForToUserDefinedOperators
					Case BoundKind.ForToStatement
						Throw ExceptionUtilities.UnexpectedValue(statement.Kind)
					Case BoundKind.ForEachStatement
						boundConditionalGoto = Me._instrumenterOpt.InstrumentForEachStatementConditionalGotoStart(DirectCast(statement, BoundForEachStatement), boundConditionalGoto)
						Exit Select
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(statement.Kind)
				End Select
			End If
			Dim boundGotoStatement As BoundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundGotoStatement(syntax, continueLabel, Nothing, False)
			If (loopResumeLabelOpt IsNot Nothing) Then
				boundGotoStatement = LocalRewriter.Concat(loopResumeLabelOpt, boundGotoStatement)
			End If
			If (instrument) Then
				boundGotoStatement = SyntheticBoundNodeFactory.HiddenSequencePoint(boundGotoStatement)
			End If
			Return New BoundStatementList(syntax, ImmutableArray.Create(Of BoundStatement)(New BoundStatement() { boundGotoStatement, New BoundLabelStatement(syntax, labelSymbol), rewrittenBody, New BoundLabelStatement(syntax, continueLabel), boundConditionalGoto, New BoundLabelStatement(syntax, exitLabel) }), False)
		End Function

		Private Function RewriteWinRtEvent(ByVal node As BoundAddRemoveHandlerStatement, ByVal unwrappedEventAccess As BoundEventAccess, ByVal isAddition As Boolean) As BoundStatement
			Dim boundBlock As BoundStatement
			Dim wellKnownMember As Microsoft.CodeAnalysis.WellKnownMember
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim syntax As SyntaxNode = node.Syntax
			Dim eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol = unwrappedEventAccess.EventSymbol
			Dim eventAccessReceiver As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.GetEventAccessReceiver(unwrappedEventAccess)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.Handler)
			Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator = Nothing
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Nothing
			If (Not eventSymbol.IsShared AndAlso LocalRewriter.EventReceiverNeedsTemp(eventAccessReceiver)) Then
				Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = eventAccessReceiver.Type
				boundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, New SynthesizedLocal(Me._currentMethodOrLambda, type, SynthesizedLocalKind.LoweringTemp, Nothing, False), type)
				boundAssignmentOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(syntax, boundLocal, Me.GenerateObjectCloneIfNeeded(unwrappedEventAccess.ReceiverOpt, eventAccessReceiver.MakeRValue()), True, False)
			End If
			Dim wellKnownType As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.Compilation.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationToken)
			Me.Compilation.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal)
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.Compilation.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Action_T)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = eventSymbol.Type
			namedTypeSymbol = namedTypeSymbol.Construct(New Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol() { wellKnownType })
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundLocal
			If (boundExpression1 Is Nothing) Then
				boundExpression1 = eventAccessReceiver
				If (boundExpression1 Is Nothing) Then
					boundExpression1 = (New BoundTypeExpression(syntax, typeSymbol, False)).MakeCompilerGenerated()
				End If
			End If
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundExpression1.MakeRValue()
			Dim boundDelegateCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression(syntax, boundExpression2, eventSymbol.RemoveMethod, Nothing, Nothing, Nothing, namedTypeSymbol, False)
			If (Not isAddition) Then
				wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal__RemoveEventHandler_T
				boundExpressions = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundDelegateCreationExpression, boundExpression)
			Else
				Dim wellKnownType1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.Compilation.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.System_Func_T2)
				wellKnownType1 = wellKnownType1.Construct(New Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol() { typeSymbol, wellKnownType })
				wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_WindowsRuntimeMarshal__AddEventHandler_T
				boundExpressions = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(New Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression(syntax, boundExpression2, eventSymbol.AddMethod, Nothing, Nothing, Nothing, wellKnownType1, False), boundDelegateCreationExpression, boundExpression)
			End If
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			If (Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, wellKnownMember, syntax, False)) Then
				methodSymbol = methodSymbol.Construct(New Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol() { typeSymbol })
				Dim returnType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = methodSymbol.ReturnType
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, methodSymbol, Nothing, Nothing, boundExpressions, Nothing, returnType, True, False, bitVector)
				If (boundLocal IsNot Nothing) Then
					Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
					boundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(syntax, statementSyntaxes, ImmutableArray.Create(Of LocalSymbol)(boundLocal.LocalSymbol), ImmutableArray.Create(Of BoundStatement)(New BoundExpressionStatement(syntax, boundAssignmentOperator, False), New BoundExpressionStatement(syntax, boundCall, False)), False)
				Else
					boundBlock = New BoundExpressionStatement(syntax, boundCall, False)
				End If
			Else
				boundBlock = New BoundExpressionStatement(syntax, New BoundBadExpression(syntax, LookupResultKind.Empty, ImmutableArray.Create(Of Symbol)(eventSymbol), ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty, ErrorTypeSymbol.UnknownResultType, True), False)
			End If
			Return boundBlock
		End Function

		Private Function RewriteWithBlockStatements(ByVal node As BoundWithStatement, ByVal generateUnstructuredExceptionHandlingResumeCode As Boolean, ByVal locals As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol), ByVal initializers As ImmutableArray(Of BoundExpression), ByVal placeholder As BoundValuePlaceholderBase, ByVal replaceWith As BoundExpression) As BoundBlock
			Dim body As BoundBlock = node.Body
			Dim syntax As SyntaxNode = node.Syntax
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement).GetInstance()
			Dim flag As Boolean = If(Not Me(node), False, syntax.Kind() = SyntaxKind.WithBlock)
			If (flag) Then
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me._instrumenterOpt.CreateWithStatementPrologue(node)
				If (boundStatement IsNot Nothing) Then
					instance.Add(boundStatement)
				End If
			End If
			If (generateUnstructuredExceptionHandlingResumeCode) Then
				Me.RegisterUnstructuredExceptionHandlingResumeTarget(syntax, True, instance)
			End If
			Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = initializers.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As BoundExpression = enumerator.Current
				instance.Add((New BoundExpressionStatement(syntax, current, False)).MakeCompilerGenerated())
			End While
			Me.AddPlaceholderReplacement(placeholder, replaceWith)
			instance.Add(DirectCast(Me.Visit(body), Microsoft.CodeAnalysis.VisualBasic.BoundStatement))
			Me.RemovePlaceholderReplacement(placeholder)
			If (flag) Then
				Dim boundStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me._instrumenterOpt.CreateWithStatementEpilogue(node)
				If (boundStatement1 IsNot Nothing) Then
					instance.Add(boundStatement1)
				End If
			End If
			If (generateUnstructuredExceptionHandlingResumeCode) Then
				instance.Add(Me.RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(syntax))
			End If
			If (Not node.Binder.ExpressionIsAccessedFromNestedLambda) Then
				Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol).Enumerator = locals.GetEnumerator()
				While enumerator1.MoveNext()
					Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = enumerator1.Current
					Dim type As TypeSymbol = localSymbol.Type
					If (localSymbol.IsByRef OrElse Not Me.LocalOrFieldNeedsToBeCleanedUp(type)) Then
						Continue While
					End If
					instance.Add((New BoundExpressionStatement(syntax, Me.VisitExpression((New BoundAssignmentOperator(syntax, (New BoundLocal(syntax, localSymbol, True, type)).MakeCompilerGenerated(), (New BoundConversion(syntax, (New BoundLiteral(syntax, ConstantValue.[Nothing], Nothing)).MakeCompilerGenerated(), ConversionKind.WideningNothingLiteral, False, False, type, False)).MakeCompilerGenerated(), True, type, False)).MakeCompilerGenerated()), False)).MakeCompilerGenerated())
				End While
			End If
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
			Return New BoundBlock(syntax, statementSyntaxes, locals, instance.ToImmutableAndFree(), False)
		End Function

		Private Shared Function RightCantChangeLeftLocal(ByVal left As BoundExpression, ByVal right As BoundExpression) As Boolean
			If (right.Kind = BoundKind.Local) Then
				Return True
			End If
			Return right.Kind = BoundKind.Parameter
		End Function

		Private Shared Function ShouldCaptureConditionalAccessReceiver(ByVal receiver As BoundExpression) As Boolean
			Dim isByRef As Boolean
			Dim kind As BoundKind = receiver.Kind
			If (kind = BoundKind.MeReference) Then
				isByRef = False
			ElseIf (kind = BoundKind.Local) Then
				isByRef = DirectCast(receiver, BoundLocal).LocalSymbol.IsByRef
			Else
				isByRef = If(kind = BoundKind.Parameter, DirectCast(receiver, BoundParameter).ParameterSymbol.IsByRef, Not receiver.IsDefaultValue())
			End If
			Return isByRef
		End Function

		Private Shared Function ShouldGenerateHashTableSwitch(ByVal [module] As PEModuleBuilder, ByVal node As BoundSelectStatement) As Boolean
			Dim flag As Boolean
			If ([module].SupportsPrivateImplClass) Then
				Dim constantValues As HashSet(Of ConstantValue) = New HashSet(Of ConstantValue)()
				Dim enumerator As ImmutableArray(Of BoundCaseBlock).Enumerator = node.CaseBlocks.GetEnumerator()
				While enumerator.MoveNext()
					Dim enumerator1 As ImmutableArray(Of BoundCaseClause).Enumerator = enumerator.Current.CaseStatement.CaseClauses.GetEnumerator()
					While enumerator1.MoveNext()
						Dim current As BoundCaseClause = enumerator1.Current
						Dim constantValueOpt As ConstantValue = Nothing
						Dim kind As BoundKind = current.Kind
						If (kind = BoundKind.SimpleCaseClause) Then
							constantValueOpt = DirectCast(current, BoundSimpleCaseClause).ValueOpt.ConstantValueOpt
						Else
							If (kind <> BoundKind.RelationalCaseClause) Then
								Throw ExceptionUtilities.UnexpectedValue(current.Kind)
							End If
							constantValueOpt = DirectCast(current, BoundRelationalCaseClause).ValueOpt.ConstantValueOpt
						End If
						constantValues.Add(constantValueOpt)
					End While
				End While
				flag = SwitchStringJumpTableEmitter.ShouldGenerateHashTableSwitch([module], constantValues.Count)
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function ShouldGenerateUnstructuredExceptionHandlingResumeCode(ByVal statement As BoundStatement) As Boolean
			Dim flag As Boolean
			If (statement.WasCompilerGenerated) Then
				flag = False
			ElseIf (Me.InsideValidUnstructuredExceptionHandlingResumeContext()) Then
				If (Not TypeOf statement.Syntax Is StatementSyntax AndAlso (statement.Syntax.Parent Is Nothing OrElse statement.Syntax.Parent.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.EraseStatement)) Then
					Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = statement.Syntax.Kind()
					If (syntaxKind <= Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfBlock) Then
						If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ModifiedIdentifier) Then
							If (statement.Kind = BoundKind.LocalDeclaration AndAlso statement.Syntax.Parent IsNot Nothing AndAlso statement.Syntax.Parent.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.VariableDeclarator AndAlso statement.Syntax.Parent.Parent IsNot Nothing AndAlso statement.Syntax.Parent.Parent.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.LocalDeclarationStatement) Then
								GoTo Label1
							End If
							flag = False
							Return flag
						Else
							If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfBlock) Then
								flag = False
								Return flag
							End If
							If (statement.Kind = BoundKind.IfStatement) Then
								GoTo Label1
							End If
							flag = False
							Return flag
						End If
					ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseBlock OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.CaseElseBlock) Then
						If (statement.Kind = BoundKind.CaseBlock) Then
							GoTo Label1
						End If
						flag = False
						Return flag
					Else
						If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.RedimClause) Then
							flag = False
							Return flag
						End If
						If (statement.Kind = BoundKind.ExpressionStatement AndAlso TypeOf statement.Syntax.Parent Is ReDimStatementSyntax) Then
							GoTo Label1
						End If
						flag = False
						Return flag
					End If
					flag = False
					Return flag
				End If
			Label1:
				flag = If(Not TypeOf statement.Syntax Is DeclarationStatementSyntax, True, False)
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Shared Function TransformReferenceOrUnconstrainedRewrittenBinaryConditionalExpression(ByVal node As BoundNode) As BoundNode
			If (node.Kind <> BoundKind.BinaryConditionalExpression) Then
				Return node
			End If
			Return LocalRewriter.TransformReferenceOrUnconstrainedRewrittenBinaryConditionalExpression(DirectCast(node, BoundBinaryConditionalExpression))
		End Function

		Private Shared Function TransformReferenceOrUnconstrainedRewrittenBinaryConditionalExpression(ByVal node As BoundBinaryConditionalExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (Not node.HasErrors) Then
				Dim testExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.TestExpression
				Dim elseExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.ElseExpression
				If (Not testExpression.IsConstant OrElse Not TypeSymbol.Equals(testExpression.Type, elseExpression.Type, TypeCompareKind.ConsiderEverything)) Then
					boundExpression = node
				Else
					boundExpression = If(Not testExpression.ConstantValueOpt.IsNothing, testExpression, node.ElseExpression)
				End If
			Else
				boundExpression = node
			End If
			Return boundExpression
		End Function

		Private Function TransformRewrittenBinaryOperator(ByVal node As BoundBinaryOperator) As BoundExpression
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundExpression Microsoft.CodeAnalysis.VisualBasic.LocalRewriter::TransformRewrittenBinaryOperator(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundExpression TransformRewrittenBinaryOperator(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 84
			'    à ..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 73
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixSwitchConditionStep.cs:ligne 27
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CombinedTransformerStep.cs:ligne 198
			'    à ..(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 79
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 276
			'    à ..Visit[,]( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 286
			'    à ..Visit( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 317
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 337
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\CombinedTransformerStep.cs:ligne 44
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Private Function TransformRewrittenConversion(ByVal rewrittenConversion As BoundConversion) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (rewrittenConversion.HasErrors OrElse Me._inExpressionLambda) Then
				boundExpression = rewrittenConversion
			Else
				Dim integralConversion As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = rewrittenConversion
				Dim enumUnderlyingTypeOrSelf As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = rewrittenConversion.Type.GetEnumUnderlyingTypeOrSelf()
				Dim operand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = rewrittenConversion.Operand
				If (Not operand.IsNothingLiteral()) Then
					If (operand.Kind = BoundKind.Lambda) Then
						boundExpression = rewrittenConversion
						Return boundExpression
					End If
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = operand.Type.GetEnumUnderlyingTypeOrSelf()
					If (typeSymbol.IsFloatingType() AndAlso enumUnderlyingTypeOrSelf.IsIntegralType()) Then
						integralConversion = Me.RewriteFloatingToIntegralConversion(rewrittenConversion, typeSymbol, enumUnderlyingTypeOrSelf)
					ElseIf (typeSymbol.IsDecimalType() AndAlso (enumUnderlyingTypeOrSelf.IsBooleanType() OrElse enumUnderlyingTypeOrSelf.IsIntegralType() OrElse enumUnderlyingTypeOrSelf.IsFloatingType())) Then
						integralConversion = Me.RewriteDecimalToNumericOrBooleanConversion(rewrittenConversion, typeSymbol, enumUnderlyingTypeOrSelf)
					ElseIf (enumUnderlyingTypeOrSelf.IsDecimalType() AndAlso (typeSymbol.IsBooleanType() OrElse typeSymbol.IsIntegralType() OrElse typeSymbol.IsFloatingType())) Then
						integralConversion = Me.RewriteNumericOrBooleanToDecimalConversion(rewrittenConversion, typeSymbol, enumUnderlyingTypeOrSelf)
					ElseIf (Not typeSymbol.IsNullableType() AndAlso Not enumUnderlyingTypeOrSelf.IsNullableType()) Then
						If (typeSymbol.IsObjectType() AndAlso (enumUnderlyingTypeOrSelf.IsTypeParameter() OrElse enumUnderlyingTypeOrSelf.IsIntrinsicType())) Then
							integralConversion = Me.RewriteFromObjectConversion(rewrittenConversion, typeSymbol, enumUnderlyingTypeOrSelf)
						ElseIf (typeSymbol.IsTypeParameter()) Then
							integralConversion = LocalRewriter.RewriteAsDirectCast(rewrittenConversion)
						ElseIf (enumUnderlyingTypeOrSelf.IsTypeParameter()) Then
							integralConversion = LocalRewriter.RewriteAsDirectCast(rewrittenConversion)
						ElseIf (typeSymbol.IsStringType() AndAlso (enumUnderlyingTypeOrSelf.IsCharSZArray() OrElse enumUnderlyingTypeOrSelf.IsIntrinsicValueType())) Then
							integralConversion = Me.RewriteFromStringConversion(rewrittenConversion, typeSymbol, enumUnderlyingTypeOrSelf)
						ElseIf (enumUnderlyingTypeOrSelf.IsStringType() AndAlso (typeSymbol.IsCharSZArray() OrElse typeSymbol.IsIntrinsicValueType())) Then
							integralConversion = Me.RewriteToStringConversion(rewrittenConversion, typeSymbol, enumUnderlyingTypeOrSelf)
						ElseIf (typeSymbol.IsReferenceType AndAlso enumUnderlyingTypeOrSelf.IsCharSZArray()) Then
							integralConversion = Me.RewriteReferenceTypeToCharArrayRankOneConversion(rewrittenConversion, typeSymbol, enumUnderlyingTypeOrSelf)
						ElseIf (enumUnderlyingTypeOrSelf.IsReferenceType) Then
							integralConversion = LocalRewriter.RewriteAsDirectCast(rewrittenConversion)
						ElseIf (typeSymbol.IsReferenceType AndAlso enumUnderlyingTypeOrSelf.IsIntrinsicValueType()) Then
							integralConversion = Me.RewriteFromObjectConversion(rewrittenConversion, Me.Compilation.GetSpecialType(SpecialType.System_Object), enumUnderlyingTypeOrSelf)
						End If
					End If
				ElseIf (enumUnderlyingTypeOrSelf.IsTypeParameter() OrElse enumUnderlyingTypeOrSelf.IsReferenceType) Then
					integralConversion = LocalRewriter.RewriteAsDirectCast(rewrittenConversion)
				End If
				boundExpression = integralConversion
			End If
			Return boundExpression
		End Function

		Private Shared Function TransformRewrittenTernaryConditionalExpression(ByVal node As BoundTernaryConditionalExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (Not node.Condition.IsConstant OrElse Not node.WhenTrue.IsConstant OrElse Not node.WhenFalse.IsConstant) Then
				boundExpression = node
			Else
				boundExpression = If(If(node.Condition.ConstantValueOpt.IsBoolean, node.Condition.ConstantValueOpt.BooleanValue, node.Condition.ConstantValueOpt.IsString), node.WhenTrue, node.WhenFalse)
			End If
			Return boundExpression
		End Function

		Private Shared Function TryFoldTwoConcatConsts(ByVal leftConst As Microsoft.CodeAnalysis.ConstantValue, ByVal rightConst As Microsoft.CodeAnalysis.ConstantValue) As Microsoft.CodeAnalysis.ConstantValue
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
			Dim stringValue As String = leftConst.StringValue
			Dim str As String = rightConst.StringValue
			If (leftConst.IsDefaultValue OrElse rightConst.IsDefaultValue OrElse stringValue.Length + str.Length >= 0) Then
				constantValue = Microsoft.CodeAnalysis.ConstantValue.Create([String].Concat(stringValue, str))
			Else
				constantValue = Nothing
			End If
			Return constantValue
		End Function

		Private Function TryFoldTwoConcatOperands(ByVal factory As SyntheticBoundNodeFactory, ByVal loweredLeft As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal loweredRight As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim constantValueOpt As Microsoft.CodeAnalysis.ConstantValue = loweredLeft.ConstantValueOpt
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = loweredRight.ConstantValueOpt
			If (constantValueOpt IsNot Nothing AndAlso constantValue IsNot Nothing) Then
				Dim constantValue1 As Microsoft.CodeAnalysis.ConstantValue = LocalRewriter.TryFoldTwoConcatConsts(constantValueOpt, constantValue)
				If (constantValue1 Is Nothing) Then
					GoTo Label1
				End If
				boundExpression = factory.StringLiteral(constantValue1)
				Return boundExpression
			End If
			If (Not LocalRewriter.IsNullOrEmptyStringConstant(loweredLeft)) Then
				If (Me._inExpressionLambda OrElse Not LocalRewriter.IsNullOrEmptyStringConstant(loweredRight)) Then
					boundExpression = Nothing
					Return boundExpression
				End If
				boundExpression = LocalRewriter.RewriteStringConcatenationOneExpr(factory, loweredLeft)
				Return boundExpression
			ElseIf (Not LocalRewriter.IsNullOrEmptyStringConstant(loweredRight)) Then
				If (Me._inExpressionLambda) Then
					boundExpression = Nothing
					Return boundExpression
				End If
				boundExpression = LocalRewriter.RewriteStringConcatenationOneExpr(factory, loweredRight)
				Return boundExpression
			Else
				boundExpression = factory.Literal("")
				Return boundExpression
			End If
			boundExpression = Nothing
			Return boundExpression
		End Function

		Private Function TryGetSpecialMember(Of T As Symbol)(<Out> ByRef result As T, ByVal memberId As SpecialMember, ByVal syntax As SyntaxNode) As Boolean
			Dim flag As Boolean
			result = Nothing
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = New UseSiteInfo(Of AssemblySymbol)()
			Dim specialTypeMember As Symbol = Binder.GetSpecialTypeMember(Me._topMethod.ContainingAssembly, memberId, useSiteInfo)
			If (Not Binder.ReportUseSite(Me._diagnostics, syntax.GetLocation(), useSiteInfo)) Then
				result = DirectCast(specialTypeMember, T)
				flag = True
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function TryGetWellknownMember(Of T As Symbol)(<Out> ByRef result As T, ByVal memberId As WellKnownMember, ByVal syntax As SyntaxNode, Optional ByVal isOptional As Boolean = False) As Boolean
			Dim flag As Boolean
			result = Nothing
			Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = New UseSiteInfo(Of AssemblySymbol)()
			Dim wellKnownTypeMember As Symbol = Binder.GetWellKnownTypeMember(Me.Compilation, memberId, useSiteInfo)
			If (useSiteInfo.DiagnosticInfo Is Nothing) Then
				Me._diagnostics.AddDependencies(useSiteInfo)
				result = DirectCast(wellKnownTypeMember, T)
				flag = True
			Else
				If (Not isOptional) Then
					Binder.ReportUseSite(Me._diagnostics, syntax.GetLocation(), useSiteInfo)
				End If
				flag = False
			End If
			Return flag
		End Function

		Private Function UnwrapEventAccess(ByVal node As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundEventAccess
			Dim boundEventAccess As Microsoft.CodeAnalysis.VisualBasic.BoundEventAccess
			boundEventAccess = If(node.Kind <> BoundKind.EventAccess, Me.UnwrapEventAccess(DirectCast(node, BoundParenthesized).Expression), DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundEventAccess))
			Return boundEventAccess
		End Function

		Private Shared Function UpdateConditionalAccess(ByVal operand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal whenNotNull As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal whenNull As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSequence
			If (operand.Kind <> BoundKind.Sequence) Then
				boundSequence = Nothing
			Else
				boundSequence = DirectCast(operand, Microsoft.CodeAnalysis.VisualBasic.BoundSequence)
				operand = boundSequence.ValueOpt
			End If
			Dim boundLoweredConditionalAccess As Microsoft.CodeAnalysis.VisualBasic.BoundLoweredConditionalAccess = DirectCast(operand, Microsoft.CodeAnalysis.VisualBasic.BoundLoweredConditionalAccess)
			operand = boundLoweredConditionalAccess.Update(boundLoweredConditionalAccess.ReceiverOrCondition, boundLoweredConditionalAccess.CaptureReceiver, boundLoweredConditionalAccess.PlaceholderId, whenNotNull, whenNull, whenNotNull.Type)
			If (boundSequence IsNot Nothing) Then
				boundExpression = boundSequence.Update(boundSequence.Locals, boundSequence.SideEffects, operand, operand.Type)
			Else
				boundExpression = operand
			End If
			Return boundExpression
		End Function

		Private Shared Sub UpdateMethodAndArgumentsIfReducedFromMethod(ByRef method As MethodSymbol, ByRef receiver As BoundExpression, ByRef arguments As ImmutableArray(Of BoundExpression))
			If (receiver IsNot Nothing) Then
				Dim callsiteReducedFromMethod As MethodSymbol = method.CallsiteReducedFromMethod
				If (callsiteReducedFromMethod IsNot Nothing) Then
					If (Not arguments.IsEmpty) Then
						Dim boundExpressionArray(arguments.Length + 1 - 1) As BoundExpression
						boundExpressionArray(0) = receiver
						arguments.CopyTo(boundExpressionArray, 1)
						arguments = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(boundExpressionArray)
					Else
						arguments = ImmutableArray.Create(Of BoundExpression)(receiver)
					End If
					receiver = Nothing
					method = callsiteReducedFromMethod
				End If
			End If
		End Sub

		Private Sub UpdatePlaceholderReplacement(ByVal placeholder As BoundValuePlaceholderBase, ByVal value As BoundExpression)
			Me._placeholderReplacementMapDoNotUseDirectly(placeholder) = value
		End Sub

		Public Overrides Function Visit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			If (boundExpression Is Nothing) Then
				boundNode = MyBase.Visit(node)
			Else
				boundNode = Me.VisitExpression(boundExpression)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitAddHandlerStatement(ByVal node As BoundAddHandlerStatement) As BoundNode
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.RewriteAddRemoveHandler(node)
			If (Me(node, boundStatement)) Then
				boundStatement = Me._instrumenterOpt.InstrumentAddHandlerStatement(node, boundStatement)
			End If
			Return boundStatement
		End Function

		Public Overrides Function VisitAggregateClause(ByVal node As BoundAggregateClause) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (node.CapturedGroupOpt Is Nothing) Then
				boundNode = Me.Visit(node.UnderlyingExpression)
			Else
				Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, node.CapturedGroupOpt.Type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
				Me.AddPlaceholderReplacement(node.GroupPlaceholderOpt, New BoundLocal(node.Syntax, synthesizedLocal, False, synthesizedLocal.Type))
				Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(node.Syntax, ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal), ImmutableArray.Create(Of BoundExpression)(New BoundAssignmentOperator(node.Syntax, New BoundLocal(node.Syntax, synthesizedLocal, True, synthesizedLocal.Type), Me.VisitExpressionNode(node.CapturedGroupOpt), True, synthesizedLocal.Type, False)), Me.VisitExpressionNode(node.UnderlyingExpression), node.Type, False)
				Me.RemovePlaceholderReplacement(node.GroupPlaceholderOpt)
				boundNode = boundSequence
			End If
			Return boundNode
		End Function

		Private Function VisitAndGenerateObjectCloneIfNeeded(ByVal right As BoundExpression, Optional ByVal suppressObjectClone As Boolean = False) As BoundExpression
			If (suppressObjectClone OrElse right.HasErrors) Then
				Return Me.VisitExpression(right)
			End If
			Return Me.GenerateObjectCloneIfNeeded(right, Me.VisitExpression(right))
		End Function

		Public Overrides Function VisitAnonymousTypeCreationExpression(ByVal node As BoundAnonymousTypeCreationExpression) As BoundNode
			Dim anonymousTypePropertyLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol
			Dim length As Integer = node.Arguments.Length
			Dim item(length - 1 + 1 - 1) As BoundExpression
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol) = Nothing
			Dim num As Integer = length - 1
			Dim num1 As Integer = 0
			Do
				item(num1) = node.Arguments(num1)
				If (node.BinderOpt IsNot Nothing) Then
					anonymousTypePropertyLocal = node.BinderOpt.GetAnonymousTypePropertyLocal(num1)
				Else
					anonymousTypePropertyLocal = Nothing
				End If
				Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = anonymousTypePropertyLocal
				If (localSymbol IsNot Nothing) Then
					If (instance Is Nothing) Then
						instance = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol).GetInstance()
					End If
					instance.Add(localSymbol)
					Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(item(num1).Syntax, localSymbol, True, localSymbol.Type)
					item(num1) = New BoundAssignmentOperator(item(num1).Syntax, boundLocal, item(num1), True, localSymbol.Type, False)
				End If
				item(num1) = Me.VisitExpression(item(num1))
				num1 = num1 + 1
			Loop While num1 <= num
			Dim syntax As SyntaxNode = node.Syntax
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(node.Type, NamedTypeSymbol).InstanceConstructors(0)
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(item)
			Dim type As TypeSymbol = node.Type
			Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
			Dim boundObjectCreationExpression As BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression(syntax, methodSymbol, boundExpressions, Nothing, type, False, bitVector)
			If (instance IsNot Nothing) Then
				boundObjectCreationExpression = New BoundSequence(node.Syntax, instance.ToImmutableAndFree(), ImmutableArray(Of BoundExpression).Empty, boundObjectCreationExpression, node.Type, False)
			End If
			Return boundObjectCreationExpression
		End Function

		Public Overrides Function VisitAnonymousTypeFieldInitializer(ByVal node As BoundAnonymousTypeFieldInitializer) As BoundNode
			Return Me.Visit(node.Value)
		End Function

		Public Overrides Function VisitAnonymousTypePropertyAccess(ByVal node As BoundAnonymousTypePropertyAccess) As BoundNode
			Dim anonymousTypePropertyLocal As LocalSymbol = node.Binder.GetAnonymousTypePropertyLocal(node.PropertyIndex)
			Return New BoundLocal(node.Syntax, anonymousTypePropertyLocal, False, Me.VisitType(anonymousTypePropertyLocal.Type))
		End Function

		Public Overrides Function VisitArrayCreation(ByVal node As BoundArrayCreation) As BoundNode
			Return MyBase.VisitArrayCreation(node.Update(node.IsParamArrayArgument, node.Bounds, node.InitializerOpt, Nothing, ConversionKind.DelegateRelaxationLevelNone, node.Type))
		End Function

		Public Overrides Function VisitAsNewLocalDeclarations(ByVal node As BoundAsNewLocalDeclarations) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim instance As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
			Dim boundObjectInitializerFromInitializer As BoundObjectInitializerExpression = LocalRewriter.GetBoundObjectInitializerFromInitializer(node.Initializer)
			Dim localDeclarations As ImmutableArray(Of BoundLocalDeclaration) = node.LocalDeclarations
			Dim length As Integer = localDeclarations.Length - 1
			Dim num As Integer = 0
			Do
				Dim item As BoundLocalDeclaration = localDeclarations(num)
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Nothing
				Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = item.LocalSymbol
				Dim keyValuePair As KeyValuePair(Of SynthesizedStaticLocalBackingField, SynthesizedStaticLocalBackingField) = New KeyValuePair(Of SynthesizedStaticLocalBackingField, SynthesizedStaticLocalBackingField)()
				If (localSymbol.IsStatic) Then
					keyValuePair = Me.CreateBackingFieldsForStaticLocal(localSymbol, True)
				End If
				If (boundObjectInitializerFromInitializer Is Nothing) Then
					boundNode = Me.VisitAndGenerateObjectCloneIfNeeded(node.Initializer, False)
				Else
					Dim initializer As BoundExpression = node.Initializer
					Dim placeholderOpt As BoundWithLValueExpressionPlaceholder = boundObjectInitializerFromInitializer.PlaceholderOpt
					If (num > 0) Then
						Dim asClause As AsNewClauseSyntax = DirectCast(DirectCast(node.Syntax, VariableDeclaratorSyntax).AsClause, AsNewClauseSyntax)
						Dim newExpression As ObjectCreationExpressionSyntax = DirectCast(asClause.NewExpression, ObjectCreationExpressionSyntax)
						Dim type As TypeSymbol = localSymbol.Type
						placeholderOpt = New BoundWithLValueExpressionPlaceholder(asClause, type)
						placeholderOpt.SetWasCompilerGenerated()
						initializer = boundObjectInitializerFromInitializer.Binder.BindObjectCreationExpression(asClause.Type(), newExpression.ArgumentList, type, newExpression, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.Discarded, placeholderOpt)
					End If
					If (Not boundObjectInitializerFromInitializer.CreateTemporaryLocalForInitialization) Then
						Me.AddPlaceholderReplacement(placeholderOpt, Me.VisitExpressionNode(New BoundLocal(item.Syntax, localSymbol, localSymbol.Type)))
					End If
					boundNode = Me.VisitAndGenerateObjectCloneIfNeeded(initializer, False)
					If (Not boundObjectInitializerFromInitializer.CreateTemporaryLocalForInitialization) Then
						Me.RemovePlaceholderReplacement(placeholderOpt)
					End If
				End If
				instance.Add(Me.RewriteLocalDeclarationAsInitializer(item, DirectCast(boundNode, BoundExpression), keyValuePair, If(boundObjectInitializerFromInitializer Is Nothing, True, boundObjectInitializerFromInitializer.CreateTemporaryLocalForInitialization)))
				num = num + 1
			Loop While num <= length
			Return New BoundStatementList(node.Syntax, instance.ToImmutableAndFree(), False)
		End Function

		Private Function VisitAssignmentLeftExpression(ByVal node As BoundAssignmentOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Left
			If (left.Kind = BoundKind.FieldAccess) Then
				Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = DirectCast(left, Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess)
				If (Not boundFieldAccess.IsConstant) Then
					boundExpression = Me.VisitExpressionNode(left)
					Return boundExpression
				End If
				boundExpression = DirectCast(MyBase.VisitFieldAccess(boundFieldAccess), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Return boundExpression
			End If
			boundExpression = Me.VisitExpressionNode(left)
			Return boundExpression
		End Function

		Public Overrides Function VisitAssignmentOperator(ByVal node As BoundAssignmentOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim first As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Left
			If (left.IsLateBound()) Then
				boundNode = Me.RewriteLateBoundAssignment(node)
			ElseIf (node.Right.Kind <> BoundKind.MidResult OrElse Not left.IsLValue) Then
				If (LocalRewriter.IsPropertyAssignment(node)) Then
					boundExpression2 = left
				Else
					boundExpression2 = Nothing
				End If
				Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundExpression2
				If (boundExpression3 IsNot Nothing OrElse node.LeftOnTheRightOpt IsNot Nothing) Then
					Dim empty As ImmutableArray(Of SynthesizedLocal) = ImmutableArray(Of SynthesizedLocal).Empty
					If (node.LeftOnTheRightOpt Is Nothing) Then
						first = left
					Else
						first = If(boundExpression3 Is Nothing, left, boundExpression3.SetAccessKind(PropertyAccessKind.Unknown))
						Dim instance As ArrayBuilder(Of SynthesizedLocal) = ArrayBuilder(Of SynthesizedLocal).GetInstance()
						Dim result As UseTwiceRewriter.Result = UseTwiceRewriter.UseTwice(Me._currentMethodOrLambda, first, instance)
						empty = instance.ToImmutableAndFree()
						If (boundExpression3 Is Nothing) Then
							first = result.First
							boundExpression1 = result.Second.MakeRValue()
						Else
							boundExpression3 = result.First.SetAccessKind(PropertyAccessKind.[Set])
							first = boundExpression3
							boundExpression1 = result.Second.SetAccessKind(PropertyAccessKind.[Get])
						End If
						Me.AddPlaceholderReplacement(node.LeftOnTheRightOpt, Me.VisitExpressionNode(boundExpression1))
					End If
					If (boundExpression3 Is Nothing) Then
						boundExpression = node.Update(Me.VisitExpressionNode(first), Nothing, Me.VisitAndGenerateObjectCloneIfNeeded(node.Right, node.SuppressObjectClone), True, node.Type)
					Else
						boundExpression = Me.RewritePropertyAssignmentAsSetCall(node, boundExpression3)
					End If
					If (empty.Length > 0) Then
						boundExpression = If(Not boundExpression.Type.IsVoidType(), New BoundSequence(node.Syntax, StaticCast(Of LocalSymbol).From(Of SynthesizedLocal)(empty), ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty, boundExpression, boundExpression.Type, False), New BoundSequence(node.Syntax, StaticCast(Of LocalSymbol).From(Of SynthesizedLocal)(empty), ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpression), Nothing, boundExpression.Type, False))
					End If
					If (node.LeftOnTheRightOpt IsNot Nothing) Then
						Me.RemovePlaceholderReplacement(node.LeftOnTheRightOpt)
					End If
					boundNode = boundExpression
				Else
					boundNode = Me.VisitAssignmentOperatorSimple(node)
				End If
			Else
				boundNode = Me.RewriteTrivialMidAssignment(node)
			End If
			Return boundNode
		End Function

		Private Function VisitAssignmentOperatorSimple(ByVal node As BoundAssignmentOperator) As BoundExpression
			Return node.Update(Me.VisitAssignmentLeftExpression(node), Nothing, Me.VisitAndGenerateObjectCloneIfNeeded(node.Right, node.SuppressObjectClone), True, node.Type)
		End Function

		Public Overrides Function VisitAwaitOperator(ByVal node As BoundAwaitOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (Me._inExpressionLambda) Then
				boundNode = node
			Else
				Dim awaiterInstancePlaceholder As BoundLValuePlaceholder = node.AwaiterInstancePlaceholder
				Dim awaitableInstancePlaceholder As BoundRValuePlaceholder = node.AwaitableInstancePlaceholder
				Me.AddPlaceholderReplacement(awaiterInstancePlaceholder, awaiterInstancePlaceholder)
				Me.AddPlaceholderReplacement(awaitableInstancePlaceholder, awaitableInstancePlaceholder)
				Dim boundNode1 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = MyBase.VisitAwaitOperator(node)
				Me.RemovePlaceholderReplacement(awaiterInstancePlaceholder)
				Me.RemovePlaceholderReplacement(awaitableInstancePlaceholder)
				boundNode = boundNode1
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitBadExpression(ByVal node As BoundBadExpression) As BoundNode
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overrides Function VisitBinaryConditionalExpression(ByVal node As BoundBinaryConditionalExpression) As BoundNode
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundNode Microsoft.CodeAnalysis.VisualBasic.LocalRewriter::VisitBinaryConditionalExpression(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryConditionalExpression)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundNode VisitBinaryConditionalExpression(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryConditionalExpression)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Overrides Function VisitBinaryOperator(ByVal node As BoundBinaryOperator) As BoundNode
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundNode Microsoft.CodeAnalysis.VisualBasic.LocalRewriter::VisitBinaryOperator(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundNode VisitBinaryOperator(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Overrides Function VisitBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundBlock) As BoundNode
			Dim boundBlock As BoundNode
			Dim boundBlock1 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = node
			If (Not node.Locals.IsEmpty) Then
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol) = Nothing
				Dim locals As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol) = node.Locals
				Dim length As Integer = locals.Length - 1
				Dim num As Integer = 0
				While num <= length
					If (Not node.Locals(num).IsStatic) Then
						num = num + 1
					Else
						instance = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol).GetInstance()
						Exit While
					End If
				End While
				If (instance IsNot Nothing) Then
					instance.AddRange(node.Locals, num)
					locals = node.Locals
					Dim length1 As Integer = locals.Length - 1
					num = num + 1
					Do
						If (Not node.Locals(num).IsStatic) Then
							locals = node.Locals
							instance.Add(locals(num))
						End If
						num = num + 1
					Loop While num <= length1
					node = node.Update(node.StatementListSyntax, instance.ToImmutableAndFree(), node.Statements)
				End If
			End If
			If (Not Me.Instrument) Then
				boundBlock = MyBase.VisitBlock(node)
			Else
				Dim boundStatements As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement).GetInstance()
				Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement).Enumerator = node.Statements.GetEnumerator()
				While enumerator.MoveNext()
					Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = TryCast(Me.Visit(enumerator.Current), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
					If (boundStatement Is Nothing) Then
						Continue While
					End If
					boundStatements.Add(boundStatement)
				End While
				Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Nothing
				Dim boundStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me._instrumenterOpt.CreateBlockPrologue(boundBlock1, node, localSymbol)
				If (boundStatement1 IsNot Nothing) Then
					boundStatements.Insert(0, boundStatement1)
				End If
				boundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(node.Syntax, node.StatementListSyntax, If(localSymbol Is Nothing, node.Locals, node.Locals.Add(localSymbol)), boundStatements.ToImmutableAndFree(), False)
			End If
			Return boundBlock
		End Function

		Private Function VisitBottomConditionLoop(ByVal node As BoundDoLoopStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim syntax As DoLoopBlockSyntax = DirectCast(node.Syntax, DoLoopBlockSyntax)
			Dim flag As Boolean = Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)
			Dim boundLabelStatement As Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement = Nothing
			If (flag) Then
				boundLabelStatement = Me.RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(syntax.DoStatement)
			End If
			Dim labelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol = LocalRewriter.GenerateLabel("start")
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(syntax.DoStatement, labelSymbol)
			If (boundLabelStatement IsNot Nothing) Then
				boundStatement = LocalRewriter.Concat(boundLabelStatement, boundStatement)
			End If
			Dim boundStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Dim instrument As Boolean = Me(node)
			If (instrument AndAlso syntax.LoopStatement IsNot Nothing) Then
				boundStatement1 = LocalRewriter.Concat(boundStatement1, Me._instrumenterOpt.InstrumentDoLoopEpilogue(node, Nothing))
			End If
			Dim boundStatements As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)()
			If (flag) Then
				boundStatements = Me.RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, True)
			End If
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.ConditionOpt)
			If (boundExpression IsNot Nothing AndAlso instrument) Then
				boundExpression = Me._instrumenterOpt.InstrumentDoLoopStatementCondition(node, boundExpression, Me._currentMethodOrLambda)
			End If
			Dim boundConditionalGoto As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundConditionalGoto(syntax.DoStatement, boundExpression, Not node.ConditionIsUntil, labelSymbol, False)
			If (Not boundStatements.IsDefaultOrEmpty) Then
				boundConditionalGoto = New BoundStatementList(boundConditionalGoto.Syntax, boundStatements.Add(boundConditionalGoto), False)
			End If
			boundNode = If(Not instrument, New BoundStatementList(node.Syntax, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { boundStatement, boundStatement1, New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(node.Syntax, node.ContinueLabel), boundConditionalGoto, New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(node.Syntax, node.ExitLabel) }), False), New BoundStatementList(node.Syntax, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { boundStatement, Me._instrumenterOpt.InstrumentDoLoopStatementEntryOrConditionalGotoStart(node, Nothing), boundStatement1, New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(syntax.DoStatement, node.ContinueLabel), boundConditionalGoto, New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(syntax.DoStatement, node.ExitLabel) }), False))
			Return boundNode
		End Function

		Public Overrides Function VisitByRefArgumentPlaceholder(ByVal node As BoundByRefArgumentPlaceholder) As BoundNode
			Return Me(node)
		End Function

		Public Overrides Function VisitByRefArgumentWithCopyBack(ByVal node As BoundByRefArgumentWithCopyBack) As BoundNode
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overrides Function VisitCall(ByVal node As BoundCall) As BoundNode
			Dim boundConversion As BoundNode
			Dim receiverOpt As BoundExpression = node.ReceiverOpt
			Dim method As MethodSymbol = node.Method
			Dim arguments As ImmutableArray(Of BoundExpression) = node.Arguments
			If (method <> Me.Compilation.GetWellKnownTypeMember(Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Strings__AscWCharInt32)) Then
				Dim wellKnownMember As Microsoft.CodeAnalysis.WellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Count
				If (method = Me.Compilation.GetWellKnownTypeMember(Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Interaction__CallByName)) Then
					wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Versioned__CallByName
				ElseIf (method.ContainingSymbol = Me.Compilation.GetWellKnownType(WellKnownType.Microsoft_VisualBasic_Information)) Then
					If (method = Me.Compilation.GetWellKnownTypeMember(Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Information__IsNumeric)) Then
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Versioned__IsNumeric
					ElseIf (method = Me.Compilation.GetWellKnownTypeMember(Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Information__SystemTypeName)) Then
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Versioned__SystemTypeName
					ElseIf (method = Me.Compilation.GetWellKnownTypeMember(Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Information__TypeName)) Then
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Versioned__TypeName
					ElseIf (method = Me.Compilation.GetWellKnownTypeMember(Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_Information__VbTypeName)) Then
						wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.Microsoft_VisualBasic_CompilerServices_Versioned__VbTypeName
					End If
				End If
				If (wellKnownMember <> Microsoft.CodeAnalysis.WellKnownMember.Count) Then
					Dim wellKnownTypeMember As MethodSymbol = DirectCast(Me.Compilation.GetWellKnownTypeMember(wellKnownMember), MethodSymbol)
					If (wellKnownTypeMember IsNot Nothing AndAlso Not Me.ReportMissingOrBadRuntimeHelper(node, wellKnownMember, wellKnownTypeMember)) Then
						method = wellKnownTypeMember
					End If
				End If
				LocalRewriter.UpdateMethodAndArgumentsIfReducedFromMethod(method, receiverOpt, arguments)
				Dim synthesizedLocals As ImmutableArray(Of SynthesizedLocal) = New ImmutableArray(Of SynthesizedLocal)()
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = New ImmutableArray(Of BoundExpression)()
				Dim flag As Boolean = If(node.SuppressObjectClone, True, method = Me.Compilation.GetWellKnownTypeMember(Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_CompilerServices_RuntimeHelpers__GetObjectValueObject))
				receiverOpt = Me.VisitExpressionNode(receiverOpt)
				node = node.Update(method, Nothing, receiverOpt, Me.RewriteCallArguments(arguments, method.Parameters, synthesizedLocals, boundExpressions, flag), node.DefaultArguments, Nothing, node.IsLValue, True, node.Type)
				If (Not boundExpressions.IsDefault) Then
					boundConversion = LocalRewriter.GenerateSequenceValueSideEffects(Me._currentMethodOrLambda, node, StaticCast(Of LocalSymbol).From(Of SynthesizedLocal)(synthesizedLocals), boundExpressions)
				ElseIf (synthesizedLocals.IsDefault) Then
					boundConversion = node
				Else
					boundConversion = If(Not method.IsSub, New BoundSequence(node.Syntax, StaticCast(Of LocalSymbol).From(Of SynthesizedLocal)(synthesizedLocals), ImmutableArray(Of BoundExpression).Empty, node, node.Type, False), New BoundSequence(node.Syntax, StaticCast(Of LocalSymbol).From(Of SynthesizedLocal)(synthesizedLocals), ImmutableArray.Create(Of BoundExpression)(node), Nothing, node.Type, False))
				End If
			Else
				boundConversion = New Microsoft.CodeAnalysis.VisualBasic.BoundConversion(node.Syntax, Me.VisitExpressionNode(arguments(0)), ConversionKind.WideningNumeric, False, True, node.Type, False)
			End If
			Return boundConversion
		End Function

		Public Overrides Function VisitCaseBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundCaseBlock) As BoundNode
			Dim boundCaseBlock As Microsoft.CodeAnalysis.VisualBasic.BoundCaseBlock = DirectCast(MyBase.VisitCaseBlock(node), Microsoft.CodeAnalysis.VisualBasic.BoundCaseBlock)
			Dim body As BoundBlock = boundCaseBlock.Body
			If (Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)) Then
				body = LocalRewriter.AppendToBlock(body, Me.RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(node.Syntax))
			End If
			Return boundCaseBlock.Update(boundCaseBlock.CaseStatement, body)
		End Function

		Public Overrides Function VisitCatchBlock(ByVal node As BoundCatchBlock) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.ExceptionSourceOpt)
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.ExceptionFilterOpt)
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
			If (Me(node) AndAlso TypeOf node.Syntax Is CatchBlockSyntax) Then
				If (boundExpression1 Is Nothing) Then
					boundBlock = LocalRewriter.PrependWithPrologue(boundBlock, Me._instrumenterOpt.CreateCatchBlockPrologue(node))
				Else
					boundExpression1 = Me._instrumenterOpt.InstrumentCatchBlockFilter(node, boundExpression1, Me._currentMethodOrLambda)
				End If
			End If
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
			If (node.ErrorLineNumberOpt IsNot Nothing) Then
				boundLocal = Me.VisitExpressionNode(node.ErrorLineNumberOpt)
			ElseIf (Me._currentLineTemporary IsNot Nothing AndAlso CObj(Me._currentMethodOrLambda) = CObj(Me._topMethod)) Then
				boundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(node.Syntax, Me._currentLineTemporary, False, Me._currentLineTemporary.Type)
			End If
			Return node.Update(node.LocalOpt, boundExpression, boundLocal, boundExpression1, boundBlock, node.IsSynthesizedAsyncCatchAll)
		End Function

		Public Overrides Function VisitCompoundAssignmentTargetPlaceholder(ByVal node As BoundCompoundAssignmentTargetPlaceholder) As BoundNode
			Return Me(node)
		End Function

		Public Overrides Function VisitConditionalAccess(ByVal node As BoundConditionalAccess) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundConditionalAccessReceiverPlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim flag As Boolean
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundLoweredConditionalAccess As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression4 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.Receiver)
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = boundExpression4.Type
			Dim num As Integer = 0
			Dim synthesizedLocal As LocalSymbol = Nothing
			Dim boundExpression5 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
			Dim flag1 As Boolean = True
			Dim flag2 As Boolean = True
			Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = New Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory(Me._topMethod, Me._currentMethodOrLambda, node.Syntax, Me._compilationState, Me._diagnostics)
			If (type.IsNullableType()) Then
				If (LocalRewriter.HasNoValue(boundExpression4)) Then
					boundExpression = Nothing
					flag1 = False
					boundConditionalAccessReceiverPlaceholder = Nothing
				ElseIf (Not LocalRewriter.HasValue(boundExpression4)) Then
					If (Not LocalRewriter.ShouldCaptureConditionalAccessReceiver(boundExpression4)) Then
						boundExpression3 = boundExpression4
						boundConditionalAccessReceiverPlaceholder = boundExpression4
					Else
						synthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
						boundExpression5 = syntheticBoundNodeFactory.AssignmentExpression(syntheticBoundNodeFactory.Local(synthesizedLocal, True), boundExpression4.MakeRValue())
						boundExpression3 = syntheticBoundNodeFactory.Local(synthesizedLocal, True)
						boundConditionalAccessReceiverPlaceholder = syntheticBoundNodeFactory.Local(synthesizedLocal, True)
					End If
					boundExpression = Me.NullableHasValue(boundExpression3)
					boundConditionalAccessReceiverPlaceholder = Me.NullableValueOrDefault(boundConditionalAccessReceiverPlaceholder)
				Else
					boundExpression = Nothing
					flag2 = False
					boundConditionalAccessReceiverPlaceholder = Me.NullableValueOrDefault(boundExpression4)
				End If
				flag = False
			ElseIf (Not boundExpression4.IsConstant) Then
				boundExpression = boundExpression4
				flag = LocalRewriter.ShouldCaptureConditionalAccessReceiver(boundExpression4)
				Me._conditionalAccessReceiverPlaceholderId = Me._conditionalAccessReceiverPlaceholderId + 1
				num = Me._conditionalAccessReceiverPlaceholderId
				boundConditionalAccessReceiverPlaceholder = New Microsoft.CodeAnalysis.VisualBasic.BoundConditionalAccessReceiverPlaceholder(node.Placeholder.Syntax, num, node.Placeholder.Type)
			Else
				boundExpression = Nothing
				flag = False
				If (Not boundExpression4.ConstantValueOpt.IsNothing) Then
					boundConditionalAccessReceiverPlaceholder = boundExpression4.MakeRValue()
					flag2 = False
				Else
					boundConditionalAccessReceiverPlaceholder = Nothing
					flag1 = False
				End If
			End If
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = node.AccessExpression.Type
			If (Not flag1) Then
				boundExpression1 = Nothing
			Else
				Me.AddPlaceholderReplacement(node.Placeholder, boundConditionalAccessReceiverPlaceholder)
				boundExpression1 = Me.VisitExpressionNode(node.AccessExpression)
				Me.RemovePlaceholderReplacement(node.Placeholder)
			End If
			If (Not node.Type.IsVoidType()) Then
				If (flag1 AndAlso Not typeSymbol.IsNullableType() AndAlso typeSymbol.IsValueType) Then
					boundExpression1 = Me.WrapInNullable(boundExpression1, node.Type)
				End If
				If (Not flag2) Then
					boundExpression2 = Nothing
				Else
					boundExpression2 = If(node.Type.IsNullableType(), LocalRewriter.NullableNull(node.Syntax, node.Type), syntheticBoundNodeFactory.Null(node.Type))
				End If
			Else
				boundExpression2 = Nothing
			End If
			If (flag1) Then
				If (Not flag2) Then
					boundLoweredConditionalAccess = boundExpression1
				Else
					boundLoweredConditionalAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundLoweredConditionalAccess(node.Syntax, boundExpression, flag, num, boundExpression1, boundExpression2, node.Type, False)
				End If
			ElseIf (boundExpression2 Is Nothing) Then
				boundLoweredConditionalAccess = New BoundSequence(node.Syntax, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty, Nothing, node.Type, False)
			Else
				boundLoweredConditionalAccess = boundExpression2
			End If
			If (synthesizedLocal IsNot Nothing) Then
				boundLoweredConditionalAccess = If(Not boundLoweredConditionalAccess.Type.IsVoidType(), New BoundSequence(node.Syntax, ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal), ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpression5), boundLoweredConditionalAccess, boundLoweredConditionalAccess.Type, False), New BoundSequence(node.Syntax, ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal), ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpression5, boundLoweredConditionalAccess), Nothing, boundLoweredConditionalAccess.Type, False))
			End If
			Return boundLoweredConditionalAccess
		End Function

		Public Overrides Function VisitContinueStatement(ByVal node As BoundContinueStatement) As BoundNode
			Dim boundGotoStatement As BoundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundGotoStatement(node.Syntax, node.Label, Nothing, False)
			If (Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)) Then
				boundGotoStatement = LocalRewriter.Concat(Me.RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(node.Syntax), boundGotoStatement)
			End If
			If (Me(node, boundGotoStatement)) Then
				boundGotoStatement = Me._instrumenterOpt.InstrumentContinueStatement(node, boundGotoStatement)
			End If
			Return boundGotoStatement
		End Function

		Public Overrides Function VisitConversion(ByVal node As BoundConversion) As BoundNode
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundNode Microsoft.CodeAnalysis.VisualBasic.LocalRewriter::VisitConversion(Microsoft.CodeAnalysis.VisualBasic.BoundConversion)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundNode VisitConversion(Microsoft.CodeAnalysis.VisualBasic.BoundConversion)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Overrides Function VisitConvertedTupleLiteral(ByVal node As BoundConvertedTupleLiteral) As BoundNode
			Return Me.VisitTupleExpression(node)
		End Function

		Public Overrides Function VisitDelegateCreationExpression(ByVal node As BoundDelegateCreationExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (node.RelaxationLambdaOpt IsNot Nothing) Then
				Dim relaxationReceiverPlaceholderOpt As BoundRValuePlaceholder = node.RelaxationReceiverPlaceholderOpt
				Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = Nothing
				If (relaxationReceiverPlaceholderOpt IsNot Nothing) Then
					If (Not Me._inExpressionLambda) Then
						synthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, relaxationReceiverPlaceholderOpt.Type, SynthesizedLocalKind.DelegateRelaxationReceiver, relaxationReceiverPlaceholderOpt.Syntax, False)
						Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = (New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(relaxationReceiverPlaceholderOpt.Syntax, synthesizedLocal, synthesizedLocal.Type)).MakeRValue()
						Me.AddPlaceholderReplacement(relaxationReceiverPlaceholderOpt, boundLocal)
					Else
						Me.AddPlaceholderReplacement(relaxationReceiverPlaceholderOpt, Me.VisitExpression(node.ReceiverOpt))
					End If
				End If
				Dim boundLambda As Microsoft.CodeAnalysis.VisualBasic.BoundLambda = DirectCast(Me.Visit(node.RelaxationLambdaOpt), Microsoft.CodeAnalysis.VisualBasic.BoundLambda)
				If (relaxationReceiverPlaceholderOpt IsNot Nothing) Then
					Me.RemovePlaceholderReplacement(relaxationReceiverPlaceholderOpt)
				End If
				Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundConversion(boundLambda.Syntax, boundLambda, ConversionKind.[Widening] Or ConversionKind.Lambda, False, False, node.Type, node.HasErrors)
				If (synthesizedLocal IsNot Nothing) Then
					Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.ReceiverOpt)
					Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(boundExpression.Syntax, New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(boundExpression.Syntax, synthesizedLocal, synthesizedLocal.Type), boundExpression.MakeRValue(), True, synthesizedLocal.Type, False)
					boundConversion = New BoundSequence(boundConversion.Syntax, ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal), ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundAssignmentOperator), boundConversion, boundConversion.Type, False)
				End If
				boundNode = boundConversion
			Else
				boundNode = MyBase.VisitDelegateCreationExpression(node)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitDimStatement(ByVal node As BoundDimStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundStatementList As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim instance As ArrayBuilder(Of BoundStatement) = Nothing
			Dim enumerator As ImmutableArray(Of BoundLocalDeclarationBase).Enumerator = node.LocalDeclarations.GetEnumerator()
			While enumerator.MoveNext()
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Me.Visit(enumerator.Current)
				If (boundNode Is Nothing) Then
					Continue While
				End If
				If (instance Is Nothing) Then
					instance = ArrayBuilder(Of BoundStatement).GetInstance()
				End If
				instance.Add(DirectCast(boundNode, BoundStatement))
			End While
			If (instance Is Nothing) Then
				boundStatementList = Nothing
			Else
				boundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(node.Syntax, instance.ToImmutableAndFree(), False)
			End If
			Return boundStatementList
		End Function

		Public Overrides Function VisitDirectCast(ByVal node As BoundDirectCast) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode1 As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (Me._inExpressionLambda OrElse Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(node.ConversionKind)) Then
				Dim flag As Boolean = Me._inExpressionLambda
				If ((node.ConversionKind And (ConversionKind.Lambda Or ConversionKind.ConvertedToExpressionTree)) = (ConversionKind.Lambda Or ConversionKind.ConvertedToExpressionTree)) Then
					Me._inExpressionLambda = True
				End If
				boundNode1 = If(node.RelaxationLambdaOpt IsNot Nothing, Me.RewriteLambdaRelaxationConversion(node), MyBase.VisitDirectCast(node))
				Me._inExpressionLambda = flag
				boundNode = boundNode1
			Else
				boundNode = Me.VisitExpressionNode(node.Operand)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitDoLoopStatement(ByVal node As BoundDoLoopStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (node.ConditionOpt Is Nothing) Then
				boundNode = Me.VisitInfiniteLoop(node)
			Else
				boundNode = If(Not node.ConditionIsTop, Me.VisitBottomConditionLoop(node), Me.VisitTopConditionLoop(node))
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitEndStatement(ByVal node As BoundEndStatement) As BoundNode
			Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = New Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory(Me._topMethod, Me._currentMethodOrLambda, node.Syntax, Me._compilationState, Me._diagnostics)
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = syntheticBoundNodeFactory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__EndApp, False)
			Dim statement As BoundStatement = node
			If (methodSymbol IsNot Nothing) Then
				statement = syntheticBoundNodeFactory.[Call](Nothing, methodSymbol, ImmutableArray(Of BoundExpression).Empty).ToStatement()
			End If
			If (Me(node, statement)) Then
				statement = Me._instrumenterOpt.InstrumentEndStatement(node, statement)
			End If
			Return statement
		End Function

		Public Overrides Function VisitEraseStatement(ByVal node As BoundEraseStatement) As BoundNode
			Dim boundStatementList As BoundNode
			If (node.Clauses.Length <> 1) Then
				Dim instance As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
				Dim enumerator As ImmutableArray(Of BoundAssignmentOperator).Enumerator = node.Clauses.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As BoundAssignmentOperator = enumerator.Current
					instance.Add(DirectCast(Me.Visit(New BoundExpressionStatement(current.Syntax, current, False)), BoundStatement))
				End While
				boundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(node.Syntax, instance.ToImmutableAndFree(), False)
			Else
				Dim item As BoundAssignmentOperator = node.Clauses(0)
				boundStatementList = Me.Visit(New BoundExpressionStatement(item.Syntax, item, False))
			End If
			Return boundStatementList
		End Function

		Public Overrides Function VisitEventAccess(ByVal node As BoundEventAccess) As BoundNode
			Return MyBase.VisitEventAccess(node)
		End Function

		Public Overrides Function VisitExitStatement(ByVal node As BoundExitStatement) As BoundNode
			Dim boundGotoStatement As BoundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundGotoStatement(node.Syntax, node.Label, Nothing, False)
			If (Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)) Then
				boundGotoStatement = LocalRewriter.Concat(Me.RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(node.Syntax), boundGotoStatement)
			End If
			If (Me(node, boundGotoStatement)) Then
				boundGotoStatement = Me._instrumenterOpt.InstrumentExitStatement(node, boundGotoStatement)
			End If
			Return boundGotoStatement
		End Function

		Private Function VisitExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim flag As Boolean
			Dim constantValueOpt As ConstantValue = node.ConstantValueOpt
			If (Not Me._instrumentTopLevelNonCompilerGeneratedExpressionsInQuery OrElse Not Me.Instrument OrElse node.WasCompilerGenerated OrElse node.Syntax.Kind() = SyntaxKind.GroupAggregation) Then
				flag = False
			Else
				flag = If(node.Syntax.Kind() <> SyntaxKind.SimpleAsClause OrElse node.Syntax.Parent.Kind() <> SyntaxKind.CollectionRangeVariable, TypeOf node.Syntax Is ExpressionSyntax, True)
			End If
			If (flag) Then
				Me._instrumentTopLevelNonCompilerGeneratedExpressionsInQuery = False
			End If
			boundExpression = If(constantValueOpt Is Nothing, MyBase.VisitExpressionWithStackGuard(node), Me.RewriteConstant(node, constantValueOpt))
			If (flag) Then
				Me._instrumentTopLevelNonCompilerGeneratedExpressionsInQuery = True
				boundExpression = Me._instrumenterOpt.InstrumentTopLevelExpressionInQuery(node, boundExpression)
			End If
			Return boundExpression
		End Function

		Private Function VisitExpressionNode(ByVal expression As BoundExpression) As BoundExpression
			Return DirectCast(Me.Visit(expression), BoundExpression)
		End Function

		Private Function VisitExpressionNode(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal placeholder As BoundValuePlaceholderBase, ByVal placeholderSubstitute As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (placeholder IsNot Nothing) Then
				Me.AddPlaceholderReplacement(placeholder, placeholderSubstitute)
			End If
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node)
			If (placeholder IsNot Nothing) Then
				Me.RemovePlaceholderReplacement(placeholder)
			End If
			Return boundExpression
		End Function

		Public Overrides Function VisitExpressionStatement(ByVal node As BoundExpressionStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (Not Me.IsOmittedBoundCall(node.Expression)) Then
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(MyBase.VisitExpressionStatement(node), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
				If (Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)) Then
					boundStatement = Me.RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, boundStatement, True)
				End If
				If (Me(node, boundStatement)) Then
					boundStatement = Me._instrumenterOpt.InstrumentExpressionStatement(node, boundStatement)
				End If
				boundNode = boundStatement
			Else
				boundNode = Nothing
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitFieldAccess(ByVal node As BoundFieldAccess) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (node.FieldSymbol.IsShared) Then
				boundExpression = Nothing
			Else
				boundExpression = Me.VisitExpressionNode(node.ReceiverOpt)
			End If
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundExpression
			If (Not node.FieldSymbol.IsTupleField) Then
				boundNode = node.Update(boundExpression1, node.FieldSymbol, node.IsLValue, node.SuppressVirtualCalls, Nothing, node.Type)
			Else
				boundNode = Me.MakeTupleFieldAccess(node.Syntax, node.FieldSymbol, boundExpression1, node.ConstantValueOpt, node.IsLValue)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitFieldInitializer(ByVal node As BoundFieldInitializer) As BoundNode
			Return Me.VisitFieldOrPropertyInitializer(node, ImmutableArray(Of Symbol).CastUp(Of FieldSymbol)(node.InitializedFields))
		End Function

		Private Function VisitFieldOrPropertyInitializer(ByVal node As BoundFieldOrPropertyInitializer, ByVal initializedSymbols As ImmutableArray(Of Symbol)) As BoundNode
			Dim memberAccessExpressionOpt As BoundExpression
			Dim statement As BoundStatement
			Dim syntax As SyntaxNode = node.Syntax
			Dim instance As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance(initializedSymbols.Length)
			Dim boundMeReference As BoundExpression = Nothing
			If (Not initializedSymbols.First().IsShared) Then
				boundMeReference = New Microsoft.CodeAnalysis.VisualBasic.BoundMeReference(syntax, Me._currentMethodOrLambda.ContainingType)
				boundMeReference.SetWasCompilerGenerated()
			End If
			Dim initializerOpt As BoundObjectInitializerExpression = Nothing
			Dim createTemporaryLocalForInitialization As Boolean = True
			If (node.InitialValue.Kind = BoundKind.ObjectCreationExpression OrElse node.InitialValue.Kind = BoundKind.NewT) Then
				Dim initialValue As BoundObjectCreationExpressionBase = DirectCast(node.InitialValue, BoundObjectCreationExpressionBase)
				If (initialValue.InitializerOpt IsNot Nothing AndAlso initialValue.InitializerOpt.Kind = BoundKind.ObjectInitializerExpression) Then
					initializerOpt = DirectCast(initialValue.InitializerOpt, BoundObjectInitializerExpression)
					createTemporaryLocalForInitialization = initializerOpt.CreateTemporaryLocalForInitialization
				End If
			End If
			Dim instrument As Boolean = Me(node)
			Dim length As Integer = initializedSymbols.Length - 1
			Dim num As Integer = 0
			Do
				Dim item As Symbol = initializedSymbols(num)
				If (initializedSymbols.Length <= 1) Then
					memberAccessExpressionOpt = node.MemberAccessExpressionOpt
				ElseIf (item.Kind <> SymbolKind.Field) Then
					Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
					Dim hasSet As Boolean = propertySymbol.HasSet
					Dim empty As ImmutableArray(Of !0) = ImmutableArray(Of BoundExpression).Empty
					Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
					memberAccessExpressionOpt = New BoundPropertyAccess(syntax, propertySymbol, Nothing, PropertyAccessKind.[Set], hasSet, boundMeReference, empty, bitVector, False)
				Else
					Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
					memberAccessExpressionOpt = New BoundFieldAccess(syntax, boundMeReference, fieldSymbol, True, fieldSymbol.Type, False)
				End If
				If (createTemporaryLocalForInitialization) Then
					statement = Me.VisitExpression(New BoundAssignmentOperator(syntax, memberAccessExpressionOpt, node.InitialValue, False, False)).ToStatement()
				Else
					Me.AddPlaceholderReplacement(initializerOpt.PlaceholderOpt, memberAccessExpressionOpt)
					statement = Me.VisitExpressionNode(node.InitialValue).ToStatement()
					Me.RemovePlaceholderReplacement(initializerOpt.PlaceholderOpt)
				End If
				If (instrument) Then
					statement = Me._instrumenterOpt.InstrumentFieldOrPropertyInitializer(node, statement, num, createTemporaryLocalForInitialization)
				End If
				instance.Add(statement)
				num = num + 1
			Loop While num <= length
			Return New BoundStatementList(node.Syntax, instance.ToImmutableAndFree(), False)
		End Function

		Public Overrides Function VisitForEachStatement(ByVal node As BoundForEachStatement) As BoundNode
			Dim collection As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim instance As ArrayBuilder(Of LocalSymbol) = ArrayBuilder(Of LocalSymbol).GetInstance()
			Dim boundStatements As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
			If (node.Collection.Kind <> BoundKind.Conversion) Then
				collection = node.Collection
			Else
				Dim boundConversion As Microsoft.CodeAnalysis.VisualBasic.BoundConversion = DirectCast(node.Collection, Microsoft.CodeAnalysis.VisualBasic.BoundConversion)
				Dim operand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundConversion.Operand
				collection = If(boundConversion.ExplicitCastInCode OrElse operand.IsNothingLiteral() OrElse Not operand.Type.IsArrayType() AndAlso Not operand.Type.IsStringType(), node.Collection, operand)
			End If
			Dim type As TypeSymbol = collection.Type
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = collection
			If (node.DeclaredOrInferredLocalOpt IsNot Nothing) Then
				Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, node.ControlVariable.Type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(node.Syntax, synthesizedLocal, node.ControlVariable.Type)
				Dim flag As Boolean = False
				boundExpression = DirectCast(LocalRewriter.LocalVariableSubstituter.Replace(collection, node.DeclaredOrInferredLocalOpt, synthesizedLocal, MyBase.RecursionDepth, flag), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				If (flag) Then
					instance.Add(synthesizedLocal)
					If (Me._symbolsCapturedWithoutCopyCtor Is Nothing) Then
						Me._symbolsCapturedWithoutCopyCtor = New HashSet(Of Symbol)()
					End If
					Me._symbolsCapturedWithoutCopyCtor.Add(synthesizedLocal)
				End If
			End If
			If (node.EnumeratorInfo.CollectionPlaceholder IsNot Nothing) Then
				Me.AddPlaceholderReplacement(node.EnumeratorInfo.CollectionPlaceholder, Me.VisitExpressionNode(boundExpression).MakeRValue())
			End If
			If (type.IsArrayType() AndAlso DirectCast(type, ArrayTypeSymbol).IsSZArray) Then
				Me.RewriteForEachArrayOrString(node, boundStatements, instance, True, boundExpression)
			ElseIf (type.IsStringType()) Then
				Me.RewriteForEachArrayOrString(node, boundStatements, instance, False, boundExpression)
			ElseIf (Not node.Collection.HasErrors) Then
				Me.RewriteForEachIEnumerable(node, boundStatements, instance)
			End If
			If (node.EnumeratorInfo.CollectionPlaceholder IsNot Nothing) Then
				Me.RemovePlaceholderReplacement(node.EnumeratorInfo.CollectionPlaceholder)
			End If
			Dim syntax As SyntaxNode = node.Syntax
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
			Return New BoundBlock(syntax, statementSyntaxes, instance.ToImmutableAndFree(), boundStatements.ToImmutableAndFree(), False)
		End Function

		Public Overrides Function VisitForToStatement(ByVal node As BoundForToStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.ControlVariable)
			Dim flag As Boolean = boundExpression.Type.IsObjectType()
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.InitialValue)
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.LimitValue)
			Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.StepValue)
			boundNode = If(flag, Me.FinishObjectForLoop(node, boundExpression, boundExpression1, boundExpression2, boundExpression3), Me.FinishNonObjectForLoop(node, boundExpression, boundExpression1, boundExpression2, boundExpression3))
			Return boundNode
		End Function

		Public Overrides Function VisitGetType(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundGetType) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundGetType As Microsoft.CodeAnalysis.VisualBasic.BoundGetType = DirectCast(MyBase.VisitGetType(node), Microsoft.CodeAnalysis.VisualBasic.BoundGetType)
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			boundNode = If(Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, WellKnownMember.System_Type__GetTypeFromHandle, node.Syntax, False), boundGetType, New Microsoft.CodeAnalysis.VisualBasic.BoundGetType(boundGetType.Syntax, boundGetType.SourceType, boundGetType.Type, True))
			Return boundNode
		End Function

		Public Overrides Function VisitGotoStatement(ByVal node As BoundGotoStatement) As BoundNode
			If (node.LabelExpressionOpt IsNot Nothing) Then
				node = node.Update(node.Label, Nothing)
			End If
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(MyBase.VisitGotoStatement(node), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			If (Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)) Then
				boundStatement = LocalRewriter.Concat(Me.RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(node.Syntax), boundStatement)
			End If
			If (Me(node, boundStatement)) Then
				boundStatement = Me._instrumenterOpt.InstrumentGotoStatement(node, boundStatement)
			End If
			Return boundStatement
		End Function

		Public Overrides Function VisitGroupAggregation(ByVal node As BoundGroupAggregation) As BoundNode
			Return Me.Visit(node.Group)
		End Function

		Public Overrides Function VisitHostObjectMemberReference(ByVal node As BoundHostObjectMemberReference) As BoundNode
			Dim syntax As SyntaxNode = node.Syntax
			Dim hostObjectField As FieldSymbol = Me._previousSubmissionFields.GetHostObjectField()
			Dim boundMeReference As Microsoft.CodeAnalysis.VisualBasic.BoundMeReference = New Microsoft.CodeAnalysis.VisualBasic.BoundMeReference(syntax, Me._topMethod.ContainingType)
			Return New BoundFieldAccess(syntax, boundMeReference, hostObjectField, False, hostObjectField.Type, False)
		End Function

		Public Overrides Function VisitIfStatement(ByVal node As BoundIfStatement) As BoundNode
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
			Dim flag As Boolean = Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)
			Dim boundStatements As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)()
			If (flag) Then
				boundStatements = Me.RegisterUnstructuredExceptionHandlingResumeTarget(syntax, True)
			End If
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.Condition)
			Dim boundStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(node.Consequence), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Dim alternativeOpt As Boolean = node.AlternativeOpt IsNot Nothing
			Dim instrument As Boolean = Me(node)
			If (instrument) Then
				Dim boundStatement2 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Nothing
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = syntax.Kind()
				If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement) Then
					If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MultiLineIfBlock AndAlso syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.ElseIfBlock) Then
						Throw ExceptionUtilities.UnexpectedValue(syntax.Kind())
					End If
					If (flag AndAlso (Me.OptimizationLevelIsDebug OrElse alternativeOpt)) Then
						boundStatement2 = Me.RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(boundStatement1.Syntax)
					End If
					alternativeOpt = False
				End If
				boundStatement1 = LocalRewriter.Concat(boundStatement1, Me._instrumenterOpt.InstrumentIfStatementConsequenceEpilogue(node, boundStatement2))
			End If
			If (flag AndAlso alternativeOpt) Then
				boundStatement1 = LocalRewriter.Concat(boundStatement1, Me.RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(boundStatement1.Syntax))
			End If
			Dim boundStatement3 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(node.AlternativeOpt), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			If (instrument AndAlso boundStatement3 IsNot Nothing) Then
				If (syntax.Kind() <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.SingleLineIfStatement) Then
					If (TypeOf node.AlternativeOpt.Syntax Is ElseBlockSyntax) Then
						Dim boundStatement4 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Nothing
						If (flag AndAlso Me.OptimizationLevelIsDebug) Then
							boundStatement4 = Me.RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(boundStatement3.Syntax)
						End If
						boundStatement3 = LocalRewriter.Concat(boundStatement3, Me._instrumenterOpt.InstrumentIfStatementAlternativeEpilogue(node, boundStatement4))
						boundStatement3 = LocalRewriter.PrependWithPrologue(boundStatement3, Me._instrumenterOpt.CreateIfStatementAlternativePrologue(node))
					End If
				ElseIf (TypeOf node.AlternativeOpt.Syntax Is SingleLineElseClauseSyntax) Then
					boundStatement3 = LocalRewriter.PrependWithPrologue(boundStatement3, Me._instrumenterOpt.CreateIfStatementAlternativePrologue(node))
				End If
			End If
			If (instrument) Then
				boundExpression = Me._instrumenterOpt.InstrumentIfStatementCondition(node, boundExpression, Me._currentMethodOrLambda)
			End If
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundExpression
			Dim boundStatement5 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = boundStatement1
			Dim boundStatement6 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = boundStatement3
			If (instrument) Then
				boundStatement = node
			Else
				boundStatement = Nothing
			End If
			Return Me.RewriteIfStatement(syntaxNode, boundExpression1, boundStatement5, boundStatement6, boundStatement, boundStatements)
		End Function

		Private Function VisitInfiniteLoop(ByVal node As BoundDoLoopStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim syntax As DoLoopBlockSyntax = DirectCast(node.Syntax, DoLoopBlockSyntax)
			Dim flag As Boolean = Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)
			Dim boundLabelStatement As Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement = Nothing
			If (flag) Then
				boundLabelStatement = Me.RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(syntax.DoStatement)
			End If
			Dim labelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol = LocalRewriter.GenerateLabel("start")
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(syntax.DoStatement, labelSymbol)
			If (boundLabelStatement IsNot Nothing) Then
				boundStatement = LocalRewriter.Concat(boundLabelStatement, boundStatement)
			End If
			Dim boundStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Dim boundStatement2 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Nothing
			If (flag) Then
				boundStatement2 = Me.RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(syntax)
			End If
			Dim instrument As Boolean = Me(node)
			If (instrument AndAlso syntax.LoopStatement IsNot Nothing) Then
				boundStatement2 = Me._instrumenterOpt.InstrumentDoLoopEpilogue(node, boundStatement2)
			End If
			boundStatement1 = LocalRewriter.Concat(boundStatement1, boundStatement2)
			boundNode = If(Not instrument, New BoundStatementList(syntax, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { boundStatement, boundStatement1, New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(node.Syntax, node.ContinueLabel), New BoundGotoStatement(node.Syntax, labelSymbol, Nothing, False), New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(node.Syntax, node.ExitLabel) }), False), New BoundStatementList(syntax, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(New Microsoft.CodeAnalysis.VisualBasic.BoundStatement() { boundStatement, Me._instrumenterOpt.InstrumentDoLoopStatementEntryOrConditionalGotoStart(node, Nothing), boundStatement1, New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(syntax.DoStatement, node.ContinueLabel), New BoundGotoStatement(syntax.DoStatement, labelSymbol, Nothing, False), New Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement(syntax.DoStatement, node.ExitLabel) }), False))
			Return boundNode
		End Function

		Public Overrides Function VisitInterpolatedStringExpression(ByVal node As BoundInterpolatedStringExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = New Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory(Me._topMethod, Me._currentMethodOrLambda, node.Syntax, Me._compilationState, Me._diagnostics)
			If (node.IsEmpty) Then
				boundNode = syntheticBoundNodeFactory.StringLiteral(ConstantValue.Create([String].Empty))
			ElseIf (node.HasInterpolations) Then
				boundNode = Me.InvokeInterpolatedStringFactory(node, node.Type, "Format", node.Type, syntheticBoundNodeFactory)
			Else
				Dim stringValue As String = DirectCast(node.Contents(0), BoundLiteral).Value.StringValue
				boundNode = syntheticBoundNodeFactory.StringLiteral(ConstantValue.Create(stringValue.Replace("{{", "{").Replace("}}", "}")))
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitLabelStatement(ByVal node As BoundLabelStatement) As BoundNode
			Dim boundStatementList As BoundStatement = DirectCast(MyBase.VisitLabelStatement(node), BoundStatement)
			If (Me._currentLineTemporary IsNot Nothing AndAlso CObj(Me._currentMethodOrLambda) = CObj(Me._topMethod) AndAlso Not node.WasCompilerGenerated AndAlso node.Syntax.Kind() = SyntaxKind.LabelStatement) Then
				Dim syntax As LabelStatementSyntax = DirectCast(node.Syntax, LabelStatementSyntax)
				If (syntax.LabelToken.Kind() = SyntaxKind.IntegerLiteralToken) Then
					Dim num As Integer = 0
					Dim labelToken As SyntaxToken = syntax.LabelToken
					Int32.TryParse(labelToken.ValueText, NumberStyles.None, CultureInfo.InvariantCulture, num)
					Dim statement As BoundStatement = (New BoundAssignmentOperator(node.Syntax, New BoundLocal(node.Syntax, Me._currentLineTemporary, Me._currentLineTemporary.Type), New BoundLiteral(node.Syntax, ConstantValue.Create(num), Me._currentLineTemporary.Type), True, False)).ToStatement()
					If (Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)) Then
						statement = Me.RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, statement, False)
					End If
					boundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(node.Syntax, ImmutableArray.Create(Of BoundStatement)(boundStatementList, statement), False)
				End If
			End If
			If (node.Label.IsFromCompilation(Me._compilationState.Compilation) AndAlso Me(node, boundStatementList)) Then
				boundStatementList = Me._instrumenterOpt.InstrumentLabelStatement(node, boundStatementList)
			End If
			Return boundStatementList
		End Function

		Public Overrides Function VisitLambda(ByVal node As BoundLambda) As BoundNode
			Me._hasLambdas = True
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me._currentMethodOrLambda
			Me._currentMethodOrLambda = node.LambdaSymbol
			Me._currentMethodOrLambda = methodSymbol
			Return MyBase.VisitLambda(node)
		End Function

		Public Overrides Function VisitLateAddressOfOperator(ByVal node As BoundLateAddressOfOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (Not Me._inExpressionLambda) Then
				Dim type As NamedTypeSymbol = DirectCast(node.Type, NamedTypeSymbol)
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = LocalRewriter.BuildDelegateRelaxationLambda(node.Syntax, type, node.MemberAccess, node.Binder, Me._diagnostics)
				boundNode = Me.VisitExpressionNode(boundExpression)
			Else
				boundNode = MyBase.VisitLateAddressOfOperator(node)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitLateInvocation(ByVal node As BoundLateInvocation) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (Me._inExpressionLambda) Then
				boundNode = MyBase.VisitLateInvocation(node)
			ElseIf (node.Member.Kind <> BoundKind.LateMemberAccess) Then
				boundNode = Me.RewriteLateBoundIndexInvocation(node, node.Member, node.ArgumentsOpt)
			Else
				Dim member As BoundLateMemberAccess = DirectCast(node.Member, BoundLateMemberAccess)
				boundNode = Me.RewriteLateBoundMemberInvocation(member, member.ReceiverOpt, node.ArgumentsOpt, node.ArgumentNamesOpt, node.AccessKind = LateBoundAccessKind.[Call])
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitLateMemberAccess(ByVal memberAccess As BoundLateMemberAccess) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (Not Me._inExpressionLambda) Then
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(memberAccess.ReceiverOpt)
				Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)()
				Dim boundExpressions1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = boundExpressions
				boundExpressions = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)()
				Dim strs As ImmutableArray(Of String) = New ImmutableArray(Of String)()
				boundNode = Me.LateCallOrGet(memberAccess, boundExpression, boundExpressions1, boundExpressions, strs, memberAccess.AccessKind = LateBoundAccessKind.[Call])
			Else
				boundNode = MyBase.VisitLateMemberAccess(memberAccess)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitLocal(ByVal node As BoundLocal) As BoundNode
			Dim boundFieldAccess As BoundNode
			Dim boundMeReference As BoundExpression
			If (Not node.LocalSymbol.IsStatic) Then
				boundFieldAccess = MyBase.VisitLocal(node)
			Else
				Dim key As SynthesizedStaticLocalBackingField = Me._staticLocalMap(node.LocalSymbol).Key
				Dim syntax As SyntaxNode = node.Syntax
				If (Me._topMethod.IsShared) Then
					boundMeReference = Nothing
				Else
					boundMeReference = New Microsoft.CodeAnalysis.VisualBasic.BoundMeReference(node.Syntax, Me._topMethod.ContainingType)
				End If
				boundFieldAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(syntax, boundMeReference, key, node.IsLValue, key.Type, False)
			End If
			Return boundFieldAccess
		End Function

		Public Overrides Function VisitLocalDeclaration(ByVal node As BoundLocalDeclaration) As BoundNode
			Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = node.LocalSymbol
			Dim keyValuePair As KeyValuePair(Of SynthesizedStaticLocalBackingField, SynthesizedStaticLocalBackingField) = New KeyValuePair(Of SynthesizedStaticLocalBackingField, SynthesizedStaticLocalBackingField)()
			Dim initializerOpt As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.InitializerOpt
			Dim flag As Boolean = initializerOpt IsNot Nothing
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Nothing
			If (localSymbol.IsStatic) Then
				keyValuePair = Me.CreateBackingFieldsForStaticLocal(localSymbol, flag)
			End If
			If (flag) Then
				Dim placeholderOpt As BoundWithLValueExpressionPlaceholder = Nothing
				If (initializerOpt.Kind = BoundKind.ObjectCreationExpression OrElse initializerOpt.Kind = BoundKind.NewT) Then
					Dim boundObjectCreationExpressionBase As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpressionBase = DirectCast(initializerOpt, Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpressionBase)
					If (boundObjectCreationExpressionBase.InitializerOpt IsNot Nothing AndAlso boundObjectCreationExpressionBase.InitializerOpt.Kind = BoundKind.ObjectInitializerExpression) Then
						Dim boundObjectInitializerExpression As Microsoft.CodeAnalysis.VisualBasic.BoundObjectInitializerExpression = DirectCast(boundObjectCreationExpressionBase.InitializerOpt, Microsoft.CodeAnalysis.VisualBasic.BoundObjectInitializerExpression)
						If (Not boundObjectInitializerExpression.CreateTemporaryLocalForInitialization) Then
							placeholderOpt = boundObjectInitializerExpression.PlaceholderOpt
							Me.AddPlaceholderReplacement(placeholderOpt, Me.VisitExpressionNode(New BoundLocal(node.Syntax, localSymbol, localSymbol.Type)))
						End If
					End If
				End If
				If (Not localSymbol.IsConst) Then
					Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitAndGenerateObjectCloneIfNeeded(initializerOpt, False)
					boundStatement = Me.RewriteLocalDeclarationAsInitializer(node, boundExpression, keyValuePair, placeholderOpt Is Nothing)
				End If
				If (placeholderOpt IsNot Nothing) Then
					Me.RemovePlaceholderReplacement(placeholderOpt)
				End If
			End If
			Return boundStatement
		End Function

		Public Overrides Function VisitLValuePlaceholder(ByVal node As BoundLValuePlaceholder) As BoundNode
			Return Me(node)
		End Function

		Public Overrides Function VisitLValueToRValueWrapper(ByVal node As BoundLValueToRValueWrapper) As BoundNode
			Return Me.VisitExpressionNode(node.UnderlyingLValue).MakeRValue()
		End Function

		Public Overrides Function VisitMethodGroup(ByVal node As BoundMethodGroup) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitMidResult(ByVal node As BoundMidResult) As BoundNode
			Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, node.Type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(node.Syntax, synthesizedLocal, node.Type)
			Dim boundCompoundAssignmentTargetPlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundCompoundAssignmentTargetPlaceholder = New Microsoft.CodeAnalysis.VisualBasic.BoundCompoundAssignmentTargetPlaceholder(node.Syntax, node.Type)
			Return New BoundSequence(node.Syntax, ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal), ImmutableArray.Create(Of BoundExpression)(New BoundAssignmentOperator(node.Syntax, boundLocal, Me.VisitExpressionNode(node.Original), True, False), Me.RewriteTrivialMidAssignment(New BoundAssignmentOperator(node.Syntax, boundLocal, boundCompoundAssignmentTargetPlaceholder, node.Update(boundCompoundAssignmentTargetPlaceholder, node.Start, node.LengthOpt, node.Source, node.Type), False, False))), boundLocal.MakeRValue(), node.Type, False)
		End Function

		Public Overrides Function VisitNewT(ByVal node As BoundNewT) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundBadExpression As BoundExpression
			If (Not Me._inExpressionLambda) Then
				Dim syntax As SyntaxNode = node.Syntax
				Dim type As TypeParameterSymbol = DirectCast(node.Type, TypeParameterSymbol)
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
				If (Not Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, WellKnownMember.System_Activator__CreateInstance_T, syntax, False)) Then
					boundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(syntax, LookupResultKind.NotReferencable, ImmutableArray(Of Symbol).Empty, ImmutableArray(Of BoundExpression).Empty, type, True)
				Else
					methodSymbol = methodSymbol.Construct(ImmutableArray.Create(Of TypeSymbol)(type))
					boundBadExpression = New BoundCall(syntax, methodSymbol, Nothing, Nothing, ImmutableArray(Of BoundExpression).Empty, Nothing, False, False, type, False)
				End If
				If (node.InitializerOpt Is Nothing) Then
					boundNode = boundBadExpression
				Else
					boundNode = Me.VisitObjectCreationInitializer(node.InitializerOpt, boundBadExpression, boundBadExpression)
				End If
			ElseIf (node.InitializerOpt Is Nothing) Then
				boundNode = node
			Else
				boundNode = Me.VisitObjectCreationInitializer(node.InitializerOpt, node, node)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitNoPiaObjectCreationExpression(ByVal node As BoundNoPiaObjectCreationExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundBadExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundDirectCast As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = New Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory(Me._topMethod, Me._currentMethodOrLambda, node.Syntax, Me._compilationState, Me._diagnostics)
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = syntheticBoundNodeFactory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(WellKnownMember.System_Guid__ctor, False)
			If (methodSymbol Is Nothing) Then
				boundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(node.Syntax, LookupResultKind.NotCreatable, ImmutableArray(Of Symbol).Empty, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty, ErrorTypeSymbol.UnknownResultType, True)
			Else
				boundBadExpression = syntheticBoundNodeFactory.[New](methodSymbol, New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { syntheticBoundNodeFactory.Literal(node.GuidString) })
			End If
			Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = If(syntheticBoundNodeFactory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(WellKnownMember.System_Runtime_InteropServices_Marshal__GetTypeFromCLSID, True), syntheticBoundNodeFactory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(WellKnownMember.System_Type__GetTypeFromCLSID, False))
			If (methodSymbol1 Is Nothing) Then
				boundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(node.Syntax, LookupResultKind.OverloadResolutionFailure, ImmutableArray(Of Symbol).Empty, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty, ErrorTypeSymbol.UnknownResultType, True)
			Else
				boundExpression = syntheticBoundNodeFactory.[Call](Nothing, methodSymbol1, New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundBadExpression })
			End If
			Dim methodSymbol2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = syntheticBoundNodeFactory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(WellKnownMember.System_Activator__CreateInstance, False)
			If (methodSymbol2 Is Nothing OrElse methodSymbol2.ReturnType.IsErrorType()) Then
				boundDirectCast = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(node.Syntax, LookupResultKind.OverloadResolutionFailure, ImmutableArray(Of Symbol).Empty, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty, node.Type, True)
			Else
				Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me.GetNewCompoundUseSiteInfo()
				Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(methodSymbol2.ReturnType, node.Type, newCompoundUseSiteInfo)
				Me._diagnostics.Add(node, newCompoundUseSiteInfo)
				boundDirectCast = New Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast(node.Syntax, syntheticBoundNodeFactory.[Call](Nothing, methodSymbol2, New Microsoft.CodeAnalysis.VisualBasic.BoundExpression() { boundExpression }), conversionKind, node.Type, False)
			End If
			If (node.InitializerOpt Is Nothing OrElse node.InitializerOpt.HasErrors) Then
				boundNode = boundDirectCast
			Else
				boundNode = Me.VisitObjectCreationInitializer(node.InitializerOpt, node, boundDirectCast)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitNullableIsTrueOperator(ByVal node As BoundNullableIsTrueOperator) As BoundNode
			Dim boundLiteral As BoundNode
			Dim flag As Boolean = False
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(LocalRewriter.AdjustIfOptimizableForConditionalBranch(node.Operand, flag))
			If (flag AndAlso LocalRewriter.HasValue(boundExpression)) Then
				boundLiteral = Me.NullableValueOrDefault(boundExpression)
			ElseIf (Me._inExpressionLambda) Then
				boundLiteral = node.Update(boundExpression, node.Type)
			ElseIf (Not LocalRewriter.HasNoValue(boundExpression)) Then
				boundLiteral = Me.NullableValueOrDefault(boundExpression)
			Else
				boundLiteral = New Microsoft.CodeAnalysis.VisualBasic.BoundLiteral(node.Syntax, ConstantValue.[False], node.Type)
			End If
			Return boundLiteral
		End Function

		Public Overrides Function VisitObjectCreationExpression(ByVal node As BoundObjectCreationExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim initializerOpt As BoundObjectInitializerExpressionBase = node.InitializerOpt
			node = node.Update(node.ConstructorOpt, node.Arguments, node.DefaultArguments, Nothing, node.Type)
			Dim constructorOpt As MethodSymbol = node.ConstructorOpt
			Dim boundDirectCast As BoundExpression = node
			If (constructorOpt IsNot Nothing) Then
				Dim synthesizedLocals As ImmutableArray(Of SynthesizedLocal) = New ImmutableArray(Of SynthesizedLocal)()
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = New ImmutableArray(Of BoundExpression)()
				boundDirectCast = node.Update(constructorOpt, Me.RewriteCallArguments(node.Arguments, constructorOpt.Parameters, synthesizedLocals, boundExpressions, False), node.DefaultArguments, Nothing, constructorOpt.ContainingType)
				If (Not synthesizedLocals.IsDefault) Then
					boundDirectCast = LocalRewriter.GenerateSequenceValueSideEffects(Me._currentMethodOrLambda, boundDirectCast, StaticCast(Of LocalSymbol).From(Of SynthesizedLocal)(synthesizedLocals), boundExpressions)
				End If
				If (node.Type.IsInterfaceType()) Then
					Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me.GetNewCompoundUseSiteInfo()
					Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(boundDirectCast.Type, node.Type, newCompoundUseSiteInfo)
					Me._diagnostics.Add(boundDirectCast, newCompoundUseSiteInfo)
					boundDirectCast = New Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast(node.Syntax, boundDirectCast, conversionKind, node.Type, False)
				End If
			End If
			If (initializerOpt Is Nothing) Then
				boundNode = boundDirectCast
			Else
				boundNode = Me.VisitObjectCreationInitializer(initializerOpt, node, boundDirectCast)
			End If
			Return boundNode
		End Function

		Private Function VisitObjectCreationInitializer(ByVal objectInitializer As BoundObjectInitializerExpressionBase, ByVal objectCreationExpression As BoundExpression, ByVal rewrittenObjectCreationExpression As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			boundNode = If(objectInitializer.Kind <> BoundKind.CollectionInitializerExpression, Me.RewriteObjectInitializerExpression(DirectCast(objectInitializer, BoundObjectInitializerExpression), objectCreationExpression, rewrittenObjectCreationExpression), Me.RewriteCollectionInitializerExpression(DirectCast(objectInitializer, BoundCollectionInitializerExpression), objectCreationExpression, rewrittenObjectCreationExpression))
			Return boundNode
		End Function

		Public Overrides Function VisitOmittedArgument(ByVal node As BoundOmittedArgument) As BoundNode
			Dim boundDirectCast As BoundNode
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = Nothing
			If (Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)(fieldSymbol, WellKnownMember.System_Reflection_Missing__Value, node.Syntax, False)) Then
				Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(node.Syntax, Nothing, fieldSymbol, False, fieldSymbol.Type, False)
				Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me.GetNewCompoundUseSiteInfo()
				Dim conversionKind As Microsoft.CodeAnalysis.VisualBasic.ConversionKind = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(boundFieldAccess.Type, node.Type, newCompoundUseSiteInfo)
				Me._diagnostics.Add(node, newCompoundUseSiteInfo)
				boundDirectCast = New Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast(node.Syntax, boundFieldAccess, conversionKind, node.Type, False)
			Else
				boundDirectCast = node
			End If
			Return boundDirectCast
		End Function

		Public Overrides Function VisitOnErrorStatement(ByVal node As BoundOnErrorStatement) As BoundNode
			Dim count As Integer
			Dim boundStatementList As BoundStatement
			Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = New Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory(Me._topMethod, Me._currentMethodOrLambda, node.Syntax, Me._compilationState, Me._diagnostics)
			Dim instance As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
			If (Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)) Then
				Me.RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, False, instance)
			End If
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = syntheticBoundNodeFactory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__ClearProjectError, False)
			If (methodSymbol IsNot Nothing) Then
				Dim syntax As SyntaxNode = node.Syntax
				Dim empty As ImmutableArray(Of !0) = ImmutableArray(Of BoundExpression).Empty
				Dim returnType As TypeSymbol = methodSymbol.ReturnType
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				instance.Add((New BoundCall(syntax, methodSymbol, Nothing, Nothing, empty, Nothing, returnType, False, False, bitVector)).ToStatement())
			End If
			Select Case node.OnErrorKind
				Case OnErrorStatementKind.GoToMinusOne
					instance.Add(syntheticBoundNodeFactory.AssignmentExpression(syntheticBoundNodeFactory.Local(Me._unstructuredExceptionHandling.ResumeTargetTemporary, True), syntheticBoundNodeFactory.Literal(0)).ToStatement())
					boundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(node.Syntax, instance.ToImmutableAndFree(), False)
					If (Me(node, boundStatementList)) Then
						boundStatementList = Me._instrumenterOpt.InstrumentOnErrorStatement(node, boundStatementList)
					End If
					Return boundStatementList
				Case OnErrorStatementKind.GoToLabel
					count = 2 + Me._unstructuredExceptionHandling.ExceptionHandlers.Count
					Me._unstructuredExceptionHandling.ExceptionHandlers.Add(syntheticBoundNodeFactory.[Goto](node.LabelOpt, False))
					Exit Select
				Case OnErrorStatementKind.ResumeNext
					count = If(Not Me.OptimizationLevelIsDebug, 1, -2 - Me._unstructuredExceptionHandling.OnErrorResumeNextCount)
					Me._unstructuredExceptionHandling.OnErrorResumeNextCount = Me._unstructuredExceptionHandling.OnErrorResumeNextCount + 1
					Exit Select
				Case Else
					count = 0
					Exit Select
			End Select
			instance.Add(syntheticBoundNodeFactory.AssignmentExpression(syntheticBoundNodeFactory.Local(Me._unstructuredExceptionHandling.ActiveHandlerTemporary, True), syntheticBoundNodeFactory.Literal(count)).ToStatement())
			boundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(node.Syntax, instance.ToImmutableAndFree(), False)
			If (Me(node, boundStatementList)) Then
				boundStatementList = Me._instrumenterOpt.InstrumentOnErrorStatement(node, boundStatementList)
			End If
			Return boundStatementList
		End Function

		Public Overrides Function VisitOrdering(ByVal node As BoundOrdering) As BoundNode
			Return Me.Visit(node.UnderlyingExpression)
		End Function

		Public Overrides Function VisitParenthesized(ByVal node As BoundParenthesized) As BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.Expression)
			If (boundExpression.IsLValue) Then
				boundExpression = boundExpression.MakeRValue()
			End If
			Return boundExpression
		End Function

		Public Overrides Function VisitPreviousSubmissionReference(ByVal node As BoundPreviousSubmissionReference) As BoundNode
			Dim type As ImplicitNamedTypeSymbol = DirectCast(node.Type, ImplicitNamedTypeSymbol)
			Dim syntax As SyntaxNode = node.Syntax
			Dim orMakeField As FieldSymbol = Me._previousSubmissionFields.GetOrMakeField(type)
			Dim boundMeReference As Microsoft.CodeAnalysis.VisualBasic.BoundMeReference = New Microsoft.CodeAnalysis.VisualBasic.BoundMeReference(syntax, Me._topMethod.ContainingType)
			Return New BoundFieldAccess(syntax, boundMeReference, orMakeField, False, orMakeField.Type, False)
		End Function

		Public Overrides Function VisitPropertyAccess(ByVal node As BoundPropertyAccess) As BoundNode
			Dim boundArrayLength As BoundNode
			Dim flag As Boolean
			Dim receiverOpt As BoundExpression = node.ReceiverOpt
			If (receiverOpt Is Nothing OrElse Not receiverOpt.Type.IsArrayType() OrElse Not DirectCast(receiverOpt.Type, ArrayTypeSymbol).IsSZArray OrElse node.PropertySymbol <> Me.GetSpecialTypeMember(SpecialMember.System_Array__Length) AndAlso node.PropertySymbol <> Me.GetSpecialTypeMember(SpecialMember.System_Array__LongLength)) Then
				Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = node.PropertySymbol
				If (receiverOpt Is Nothing) Then
					flag = False
				Else
					flag = If(receiverOpt.IsMyClassReference(), True, receiverOpt.IsMyBaseReference())
				End If
				Dim flag1 As Boolean = flag
				If (Not Me._inExpressionLambda OrElse propertySymbol.ParameterCount <> 0 OrElse propertySymbol.ReducedFrom IsNot Nothing OrElse flag1) Then
					Dim mostDerivedGetMethod As MethodSymbol = propertySymbol.GetMostDerivedGetMethod()
					boundArrayLength = Me.RewriteReceiverArgumentsAndGenerateAccessorCall(node.Syntax, mostDerivedGetMethod, receiverOpt, node.Arguments, node.ConstantValueOpt, node.IsLValue, False, mostDerivedGetMethod.ReturnType)
				Else
					boundArrayLength = MyBase.VisitPropertyAccess(node)
				End If
			Else
				boundArrayLength = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayLength(node.Syntax, Me.VisitExpressionNode(receiverOpt), node.Type, False)
			End If
			Return boundArrayLength
		End Function

		Public Overrides Function VisitPropertyInitializer(ByVal node As BoundPropertyInitializer) As BoundNode
			Return Me.VisitFieldOrPropertyInitializer(node, ImmutableArray(Of Symbol).CastUp(Of PropertySymbol)(node.InitializedProperties))
		End Function

		Public Overrides Function VisitQueryableSource(ByVal node As BoundQueryableSource) As BoundNode
			Return Me.Visit(node.Source)
		End Function

		Public Overrides Function VisitQueryClause(ByVal node As BoundQueryClause) As BoundNode
			Return Me.Visit(node.UnderlyingExpression)
		End Function

		Public Overrides Function VisitQueryExpression(ByVal node As BoundQueryExpression) As BoundNode
			Return Me.Visit(node.LastOperator)
		End Function

		Public Overrides Function VisitQueryLambda(ByVal node As BoundQueryLambda) As BoundNode
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me._currentMethodOrLambda
			Me._currentMethodOrLambda = node.LambdaSymbol
			LocalRewriter.PopulateRangeVariableMapForQueryLambdaRewrite(node, Me._rangeVariableMap, Me._inExpressionLambda)
			Dim flag As Boolean = Me._instrumentTopLevelNonCompilerGeneratedExpressionsInQuery
			Dim synthesizedKind As SynthesizedLambdaKind = node.LambdaSymbol.SynthesizedKind
			Dim flag1 As Boolean = If(synthesizedKind = SynthesizedLambdaKind.AggregateQueryLambda, True, synthesizedKind = SynthesizedLambdaKind.LetVariableQueryLambda)
			Me._instrumentTopLevelNonCompilerGeneratedExpressionsInQuery = Not flag1
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = LocalRewriter.CreateReturnStatementForQueryLambdaBody(Me.VisitExpressionNode(node.Expression), node, False)
			If (flag1 AndAlso Me.Instrument) Then
				boundStatement = Me._instrumenterOpt.InstrumentQueryLambdaBody(node, boundStatement)
			End If
			LocalRewriter.RemoveRangeVariables(node, Me._rangeVariableMap)
			Me._instrumentTopLevelNonCompilerGeneratedExpressionsInQuery = flag
			Me._hasLambdas = True
			Me._currentMethodOrLambda = methodSymbol
			Return LocalRewriter.RewriteQueryLambda(boundStatement, node)
		End Function

		Public Overrides Function VisitQuerySource(ByVal node As BoundQuerySource) As BoundNode
			Return Me.Visit(node.Expression)
		End Function

		Public Overrides Function VisitRaiseEventStatement(ByVal node As BoundRaiseEventStatement) As BoundNode
			Dim boundExpressionStatement As BoundStatement
			Dim syntax As SyntaxNode = node.Syntax
			Dim unstructuredExceptionHandlingContext As LocalRewriter.UnstructuredExceptionHandlingContext = Me.LeaveUnstructuredExceptionHandlingContext(node)
			Dim eventInvocation As BoundCall = DirectCast(node.EventInvocation, BoundCall)
			Dim receiverOpt As BoundExpression = eventInvocation.ReceiverOpt
			If (receiverOpt Is Nothing OrElse receiverOpt.IsMeReference()) Then
				boundExpressionStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement(syntax, Me.VisitExpressionNode(eventInvocation), False)
			Else
				If (node.EventSymbol.IsWindowsRuntimeEvent) Then
					receiverOpt = Me.GetWindowsRuntimeEventReceiver(syntax, receiverOpt)
				End If
				Dim synthesizedLocal As LocalSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, receiverOpt.Type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = (New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, synthesizedLocal, synthesizedLocal.Type)).MakeCompilerGenerated()
				Dim boundExpressionStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = (New Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement(syntax, New BoundAssignmentOperator(syntax, boundLocal, receiverOpt, True, receiverOpt.Type, False), False)).MakeCompilerGenerated()
				eventInvocation = eventInvocation.Update(eventInvocation.Method, eventInvocation.MethodGroupOpt, boundLocal, eventInvocation.Arguments, eventInvocation.DefaultArguments, eventInvocation.ConstantValueOpt, eventInvocation.IsLValue, eventInvocation.SuppressObjectClone, eventInvocation.Type)
				Dim boundExpressionStatement2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement(syntax, Me.VisitExpressionNode(eventInvocation), False)
				Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = (New Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator(syntax, BinaryOperatorKind.[Is], boundLocal.MakeRValue(), New BoundLiteral(syntax, ConstantValue.[Nothing], Me.Compilation.GetSpecialType(SpecialType.System_Object)), False, Me.Compilation.GetSpecialType(SpecialType.System_Boolean), False)).MakeCompilerGenerated()
				Dim generatedLabelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol("skipEventRaise")
				Dim boundConditionalGoto As Microsoft.CodeAnalysis.VisualBasic.BoundConditionalGoto = (New Microsoft.CodeAnalysis.VisualBasic.BoundConditionalGoto(syntax, boundBinaryOperator, True, generatedLabelSymbol, False)).MakeCompilerGenerated()
				Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
				boundExpressionStatement = New BoundBlock(syntax, statementSyntaxes, ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal), ImmutableArray.Create(Of BoundStatement)(boundExpressionStatement1, boundConditionalGoto, boundExpressionStatement2, New BoundLabelStatement(syntax, generatedLabelSymbol)), False)
			End If
			Me.RestoreUnstructuredExceptionHandlingContext(node, unstructuredExceptionHandlingContext)
			If (Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)) Then
				boundExpressionStatement = Me.RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, boundExpressionStatement, True)
			End If
			If (Me(node, boundExpressionStatement)) Then
				boundExpressionStatement = Me._instrumenterOpt.InstrumentRaiseEventStatement(node, boundExpressionStatement)
			End If
			Return boundExpressionStatement
		End Function

		Public Overrides Function VisitRangeVariable(ByVal node As BoundRangeVariable) As BoundNode
			Return Me._rangeVariableMap(node.RangeVariable)
		End Function

		Public Overrides Function VisitRangeVariableAssignment(ByVal node As BoundRangeVariableAssignment) As BoundNode
			Return Me.Visit(node.Value)
		End Function

		Public Overrides Function VisitRedimClause(ByVal node As BoundRedimClause) As BoundNode
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim boundArrayCreation As BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation(node.Syntax, node.Indices, Nothing, node.ArrayTypeOpt, False)
			Dim instance As ArrayBuilder(Of SynthesizedLocal) = Nothing
			Dim operand As BoundExpression = node.Operand
			Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me.GetNewCompoundUseSiteInfo()
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			If (node.Preserve AndAlso Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Utils__CopyArray, node.Syntax, False)) Then
				instance = ArrayBuilder(Of SynthesizedLocal).GetInstance()
				Dim result As UseTwiceRewriter.Result = UseTwiceRewriter.UseTwice(Me._currentMethodOrLambda, operand, instance)
				operand = result.First
				Dim second As BoundExpression = result.Second
				If (second.Kind <> BoundKind.PropertyAccess) Then
					second = If(Not second.IsLateBound(), second.MakeRValue(), second.SetLateBoundAccessKind(LateBoundAccessKind.[Get]))
				Else
					second = DirectCast(second, BoundPropertyAccess).SetAccessKind(PropertyAccessKind.[Get])
				End If
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = methodSymbol.Parameters(0).Type
				second = New BoundDirectCast(node.Syntax, second, Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(second.Type, typeSymbol, newCompoundUseSiteInfo), typeSymbol, False)
				boundArrayCreation = New BoundDirectCast(node.Syntax, boundArrayCreation, Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(boundArrayCreation.Type, typeSymbol, newCompoundUseSiteInfo), typeSymbol, False)
				Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(second, boundArrayCreation)
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				boundArrayCreation = New BoundCall(syntax, methodSymbol, Nothing, Nothing, boundExpressions, Nothing, typeSymbol, False, False, bitVector)
			End If
			boundArrayCreation = New BoundDirectCast(node.Syntax, boundArrayCreation, Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyDirectCastConversion(boundArrayCreation.Type, operand.Type, newCompoundUseSiteInfo), operand.Type, False)
			Me._diagnostics.Add(node, newCompoundUseSiteInfo)
			If (operand.Kind = BoundKind.PropertyAccess) Then
				operand = DirectCast(operand, BoundPropertyAccess).SetAccessKind(PropertyAccessKind.[Set])
			ElseIf (operand.IsLateBound()) Then
				operand = operand.SetLateBoundAccessKind(LateBoundAccessKind.[Set])
			End If
			Dim boundAssignmentOperator As BoundExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(node.Syntax, operand, boundArrayCreation, True, False)
			If (instance IsNot Nothing) Then
				If (instance.Count <= 0) Then
					instance.Free()
				Else
					Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
					Dim localSymbols As ImmutableArray(Of LocalSymbol) = StaticCast(Of LocalSymbol).From(Of SynthesizedLocal)(instance.ToImmutableAndFree())
					Dim boundExpressions1 As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(boundAssignmentOperator)
					If (boundAssignmentOperator.Type.IsVoidType()) Then
						type = boundAssignmentOperator.Type
					Else
						type = Me.Compilation.GetSpecialType(SpecialType.System_Void)
					End If
					boundAssignmentOperator = New BoundSequence(syntaxNode, localSymbols, boundExpressions1, Nothing, type, False)
				End If
			End If
			Return Me.Visit(New BoundExpressionStatement(node.Syntax, boundAssignmentOperator, False))
		End Function

		Public Overrides Function VisitRedimStatement(ByVal node As BoundRedimStatement) As BoundNode
			Dim boundStatementList As BoundNode
			Dim clauses As ImmutableArray(Of BoundRedimClause)
			If (node.Clauses.Length <> 1) Then
				clauses = node.Clauses
				Dim boundStatementArray(clauses.Length - 1 + 1 - 1) As BoundStatement
				clauses = node.Clauses
				Dim length As Integer = clauses.Length - 1
				Dim num As Integer = 0
				Do
					clauses = node.Clauses
					boundStatementArray(num) = DirectCast(Me.Visit(clauses(num)), BoundStatement)
					num = num + 1
				Loop While num <= length
				boundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(node.Syntax, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundStatement)(boundStatementArray), False)
			Else
				clauses = node.Clauses
				boundStatementList = Me.Visit(clauses(0))
			End If
			Return boundStatementList
		End Function

		Public Overrides Function VisitRemoveHandlerStatement(ByVal node As BoundRemoveHandlerStatement) As BoundNode
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.RewriteAddRemoveHandler(node)
			If (Me(node, boundStatement)) Then
				boundStatement = Me._instrumenterOpt.InstrumentRemoveHandlerStatement(node, boundStatement)
			End If
			Return boundStatement
		End Function

		Public Overrides Function VisitResumeStatement(ByVal node As BoundResumeStatement) As BoundNode
			Dim bitVector As Microsoft.CodeAnalysis.BitVector
			Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = New Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory(Me._topMethod, Me._currentMethodOrLambda, node.Syntax, Me._compilationState, Me._diagnostics)
			Dim instance As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
			Dim flag As Boolean = Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)
			If (flag) Then
				Me.RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, True, instance)
			End If
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = syntheticBoundNodeFactory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__ClearProjectError, False)
			If (methodSymbol IsNot Nothing) Then
				Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim empty As ImmutableArray(Of !0) = ImmutableArray(Of BoundExpression).Empty
				Dim returnType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = methodSymbol.ReturnType
				bitVector = New Microsoft.CodeAnalysis.BitVector()
				instance.Add((New BoundCall(syntax, methodSymbol, Nothing, Nothing, empty, Nothing, returnType, False, False, bitVector)).ToStatement())
			End If
			Dim methodSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = syntheticBoundNodeFactory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__CreateProjectError, False)
			If (methodSymbol1 IsNot Nothing) Then
				Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = syntheticBoundNodeFactory.Binary(BinaryOperatorKind.Equals, syntheticBoundNodeFactory.SpecialType(SpecialType.System_Boolean), syntheticBoundNodeFactory.Local(Me._unstructuredExceptionHandling.ResumeTargetTemporary, False), syntheticBoundNodeFactory.Literal(0))
				Dim syntax1 As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(syntheticBoundNodeFactory.Literal(-2146828268))
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = methodSymbol1.ReturnType
				bitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundThrowStatement As Microsoft.CodeAnalysis.VisualBasic.BoundThrowStatement = syntheticBoundNodeFactory.[Throw](New BoundCall(syntax1, methodSymbol1, Nothing, Nothing, boundExpressions, Nothing, typeSymbol, False, False, bitVector))
				Dim boundStatements As ImmutableArray(Of BoundStatement) = New ImmutableArray(Of BoundStatement)()
				instance.Add(Me.RewriteIfStatement(syntaxNode, boundBinaryOperator, boundThrowStatement, Nothing, Nothing, boundStatements))
			End If
			Dim resumeKind As ResumeStatementKind = node.ResumeKind
			If (resumeKind = ResumeStatementKind.[Next]) Then
				instance.Add(syntheticBoundNodeFactory.[Goto](Me._unstructuredExceptionHandling.ResumeNextLabel, True))
			ElseIf (resumeKind <> ResumeStatementKind.Label) Then
				instance.Add(syntheticBoundNodeFactory.[Goto](Me._unstructuredExceptionHandling.ResumeLabel, True))
			Else
				instance.Add(syntheticBoundNodeFactory.AssignmentExpression(syntheticBoundNodeFactory.Local(Me._unstructuredExceptionHandling.ResumeTargetTemporary, True), syntheticBoundNodeFactory.Literal(0)).ToStatement())
				If (flag) Then
					Me.RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, False, instance)
				End If
				instance.Add(syntheticBoundNodeFactory.[Goto](node.LabelOpt, False))
			End If
			Dim boundStatementList As BoundStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(node.Syntax, instance.ToImmutableAndFree(), False)
			If (Me(node, boundStatementList)) Then
				boundStatementList = Me._instrumenterOpt.InstrumentResumeStatement(node, boundStatementList)
			End If
			Return boundStatementList
		End Function

		Public Overrides Function VisitReturnStatement(ByVal node As BoundReturnStatement) As BoundNode
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.RewriteReturnStatement(node)
			If (Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)) Then
				boundStatement = Me.RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, boundStatement, node.ExpressionOpt IsNot Nothing)
			End If
			If (Me(node, boundStatement) OrElse node.ExpressionOpt IsNot Nothing AndAlso Me(node.ExpressionOpt)) Then
				boundStatement = Me._instrumenterOpt.InstrumentReturnStatement(node, boundStatement)
			End If
			Return boundStatement
		End Function

		Public Overrides Function VisitRValuePlaceholder(ByVal node As BoundRValuePlaceholder) As BoundNode
			Return Me(node)
		End Function

		Public Overrides Function VisitSelectStatement(ByVal node As BoundSelectStatement) As BoundNode
			Return Me.RewriteSelectStatement(node, node.Syntax, node.ExpressionStatement, node.ExprPlaceholderOpt, node.CaseBlocks, node.RecommendSwitchTable, node.ExitLabel)
		End Function

		Public Overrides Function VisitSequencePoint(ByVal node As BoundSequencePoint) As BoundNode
			Return node.Update(DirectCast(Me.Visit(node.StatementOpt), BoundStatement))
		End Function

		Public Overrides Function VisitSequencePointWithSpan(ByVal node As BoundSequencePointWithSpan) As BoundNode
			Return node.Update(DirectCast(Me.Visit(node.StatementOpt), BoundStatement), node.Span)
		End Function

		Public Overrides Function VisitStopStatement(ByVal node As BoundStopStatement) As BoundNode
			Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = New Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory(Me._topMethod, Me._currentMethodOrLambda, node.Syntax, Me._compilationState, Me._diagnostics)
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = syntheticBoundNodeFactory.WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(WellKnownMember.System_Diagnostics_Debugger__Break, False)
			Dim statement As BoundStatement = node
			If (methodSymbol IsNot Nothing) Then
				statement = (New BoundCall(syntheticBoundNodeFactory.Syntax, methodSymbol, Nothing, Nothing, ImmutableArray(Of BoundExpression).Empty, Nothing, False, True, methodSymbol.ReturnType, False)).ToStatement()
			End If
			If (Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)) Then
				statement = Me.RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, statement, True)
			End If
			If (Me(node, statement)) Then
				statement = Me._instrumenterOpt.InstrumentStopStatement(node, statement)
			End If
			Return statement
		End Function

		Public Overrides Function VisitSyncLockStatement(ByVal node As BoundSyncLockStatement) As BoundNode
			Dim localSymbols As ImmutableArray(Of LocalSymbol)
			Dim boundStatements As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Dim flag As Boolean = False
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement).GetInstance()
			Dim syntax As SyncLockBlockSyntax = DirectCast(node.Syntax, SyncLockBlockSyntax)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.LockExpression)
			Dim specialType As NamedTypeSymbol = Me.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_Object)
			Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = Me.GetNewCompoundUseSiteInfo()
			Dim keyValuePair As KeyValuePair(Of ConversionKind, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) = Microsoft.CodeAnalysis.VisualBasic.Conversions.ClassifyConversion(boundExpression.Type, specialType, newCompoundUseSiteInfo)
			Dim key As ConversionKind = keyValuePair.Key
			Me._diagnostics.Add(node, newCompoundUseSiteInfo)
			If (Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(key)) Then
				Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = Microsoft.CodeAnalysis.VisualBasic.Conversions.TryFoldConstantConversion(boundExpression, specialType, flag)
				boundExpression = Me.TransformRewrittenConversion(New BoundConversion(node.LockExpression.Syntax, boundExpression, key, False, False, constantValue, specialType, False))
			End If
			Dim synthesizedLocal As LocalSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, specialType, SynthesizedLocalKind.Lock, syntax.SyncLockStatement, False)
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(syntax, synthesizedLocal, specialType)
			Dim instrument As Boolean = Me(node)
			If (instrument) Then
				Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me._instrumenterOpt.CreateSyncLockStatementPrologue(node)
				If (boundStatement IsNot Nothing) Then
					instance.Add(boundStatement)
				End If
			End If
			Dim statement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = (New BoundAssignmentOperator(syntax, boundLocal, boundExpression, True, specialType, False)).ToStatement()
			boundLocal = boundLocal.MakeRValue()
			If (Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)) Then
				statement = Me.RegisterUnstructuredExceptionHandlingResumeTarget(syntax, statement, True)
			End If
			Dim unstructuredExceptionHandlingContext As LocalRewriter.UnstructuredExceptionHandlingContext = Me.LeaveUnstructuredExceptionHandlingContext(node)
			If (instrument) Then
				statement = Me._instrumenterOpt.InstrumentSyncLockObjectCapture(node, statement)
			End If
			instance.Add(statement)
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Nothing
			If (node.LockExpression.Type.IsObjectType() AndAlso Me.TryGetWellknownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(methodSymbol, WellKnownMember.Microsoft_VisualBasic_CompilerServices_ObjectFlowControl__CheckForSyncLockOnValueType, syntax, True)) Then
				Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundLocal)
				Dim returnType As TypeSymbol = methodSymbol.ReturnType
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = (New BoundCall(syntax, methodSymbol, Nothing, Nothing, boundExpressions, Nothing, returnType, True, False, bitVector)).ToStatement()
				boundExpressionStatement.SetWasCompilerGenerated()
				instance.Add(boundExpressionStatement)
			End If
			Dim boundLocal1 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Nothing
			Dim boundStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Nothing
			Dim boundStatement2 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.GenerateMonitorEnter(node.LockExpression.Syntax, boundLocal, boundLocal1, boundStatement1)
			If (boundLocal1 Is Nothing) Then
				localSymbols = ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal)
				instance.Add(boundStatement2)
				boundStatements = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundBlock))
			Else
				localSymbols = ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal, boundLocal1.LocalSymbol)
				instance.Add(boundStatement1)
				boundStatements = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(boundStatement2, DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundBlock))
			End If
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, boundStatements, False)
			Dim boundStatement3 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.GenerateMonitorExit(syntax, boundLocal, boundLocal1)
			statementSyntaxes = New SyntaxList(Of StatementSyntax)()
			Dim boundBlock1 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(boundStatement3), False)
			If (instrument) Then
				boundBlock1 = DirectCast(LocalRewriter.Concat(boundBlock1, Me._instrumenterOpt.CreateSyncLockExitDueToExceptionEpilogue(node)), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
			End If
			Dim boundStatement4 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.RewriteTryStatement(syntax, boundBlock, ImmutableArray(Of BoundCatchBlock).Empty, boundBlock1, Nothing)
			instance.Add(boundStatement4)
			If (instrument) Then
				Dim boundStatement5 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me._instrumenterOpt.CreateSyncLockExitNormallyEpilogue(node)
				If (boundStatement5 IsNot Nothing) Then
					instance.Add(boundStatement5)
				End If
			End If
			Me.RestoreUnstructuredExceptionHandlingContext(node, unstructuredExceptionHandlingContext)
			statementSyntaxes = New SyntaxList(Of StatementSyntax)()
			Return New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(syntax, statementSyntaxes, localSymbols, instance.ToImmutableAndFree(), False)
		End Function

		Public Overrides Function VisitTernaryConditionalExpression(ByVal node As BoundTernaryConditionalExpression) As BoundNode
			Return LocalRewriter.TransformRewrittenTernaryConditionalExpression(DirectCast(MyBase.VisitTernaryConditionalExpression(node), BoundTernaryConditionalExpression))
		End Function

		Public Overrides Function VisitThrowStatement(ByVal node As BoundThrowStatement) As BoundNode
			Dim expressionOpt As BoundExpression = node.ExpressionOpt
			If (expressionOpt IsNot Nothing) Then
				expressionOpt = Me.VisitExpressionNode(expressionOpt)
				If (expressionOpt.Type.SpecialType = SpecialType.System_Int32) Then
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = (New SyntheticBoundNodeFactory(Me._topMethod, Me._currentMethodOrLambda, node.Syntax, Me._compilationState, Me._diagnostics)).WellKnownMember(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__CreateProjectError, False)
					If (methodSymbol IsNot Nothing) Then
						Dim syntax As SyntaxNode = node.Syntax
						Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(expressionOpt)
						Dim returnType As TypeSymbol = methodSymbol.ReturnType
						Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
						expressionOpt = New BoundCall(syntax, methodSymbol, Nothing, Nothing, boundExpressions, Nothing, returnType, False, False, bitVector)
					End If
				End If
			End If
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = node.Update(expressionOpt)
			If (Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)) Then
				boundStatement = Me.RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, boundStatement, True)
			End If
			If (Me(node, boundStatement)) Then
				boundStatement = Me._instrumenterOpt.InstrumentThrowStatement(node, boundStatement)
			End If
			Return boundStatement
		End Function

		Private Function VisitTopConditionLoop(ByVal node As BoundDoLoopStatement) As BoundNode
			Dim flag As Boolean = Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)
			Dim boundLabelStatement As Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement = Nothing
			Dim boundStatements As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)()
			If (flag) Then
				boundLabelStatement = Me.RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(node.Syntax)
				boundStatements = Me.RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, True)
			End If
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Dim boundLabelStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement = Nothing
			If (flag) Then
				boundLabelStatement1 = Me.RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(node.Syntax)
			End If
			Dim syntax As DoLoopBlockSyntax = DirectCast(node.Syntax, DoLoopBlockSyntax)
			Return Me.RewriteWhileStatement(node, Me.VisitExpressionNode(node.ConditionOpt), boundStatement, node.ContinueLabel, node.ExitLabel, Not node.ConditionIsUntil, boundLabelStatement, boundStatements, boundLabelStatement1)
		End Function

		Public Overrides Function VisitToQueryableCollectionConversion(ByVal node As BoundToQueryableCollectionConversion) As BoundNode
			Return Me.Visit(node.ConversionCall)
		End Function

		Public Overrides Function VisitTryCast(ByVal node As BoundTryCast) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundDirectCast As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (Me._inExpressionLambda OrElse Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(node.ConversionKind)) Then
				Dim flag As Boolean = Me._inExpressionLambda
				If ((node.ConversionKind And (ConversionKind.Lambda Or ConversionKind.ConvertedToExpressionTree)) = (ConversionKind.Lambda Or ConversionKind.ConvertedToExpressionTree)) Then
					Me._inExpressionLambda = True
				End If
				If (node.RelaxationLambdaOpt IsNot Nothing) Then
					boundDirectCast = Me.RewriteLambdaRelaxationConversion(node)
				Else
					boundDirectCast = Nothing
					If (Microsoft.CodeAnalysis.VisualBasic.Conversions.IsWideningConversion(node.ConversionKind) AndAlso Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(node.ConversionKind)) Then
						Dim operand As BoundExpression = node.Operand
						If (operand.Kind <> BoundKind.Lambda) Then
							Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = operand.Type
							Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = node.Type
							If (Not typeSymbol.IsTypeParameter() AndAlso typeSymbol.IsReferenceType AndAlso Not type.IsTypeParameter() AndAlso type.IsReferenceType) Then
								boundDirectCast = New Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast(node.Syntax, DirectCast(Me.Visit(operand), BoundExpression), node.ConversionKind, typeSymbol, False)
							End If
						End If
					End If
					If (boundDirectCast Is Nothing) Then
						boundDirectCast = MyBase.VisitTryCast(node)
					End If
				End If
				Me._inExpressionLambda = flag
				boundNode = boundDirectCast
			Else
				boundNode = Me.Visit(node.Operand)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitTryStatement(ByVal node As BoundTryStatement) As BoundNode
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = Me.RewriteTryBlock(node)
			Dim boundCatchBlocks As ImmutableArray(Of BoundCatchBlock) = Me.VisitList(Of BoundCatchBlock)(node.CatchBlocks)
			Dim boundBlock1 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = Me.RewriteFinallyBlock(node)
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Me.RewriteTryStatement(node.Syntax, boundBlock, boundCatchBlocks, boundBlock1, node.ExitLabelOpt)
			If (Me(node) AndAlso TypeOf node.Syntax Is TryBlockSyntax) Then
				boundStatement = Me._instrumenterOpt.InstrumentTryStatement(node, boundStatement)
			End If
			Return boundStatement
		End Function

		Private Function VisitTupleExpression(ByVal node As BoundTupleExpression) As BoundNode
			Dim boundExpressions As ImmutableArray(Of BoundExpression) = Me.VisitList(Of BoundExpression)(node.Arguments)
			Return Me.RewriteTupleCreationExpression(node, boundExpressions)
		End Function

		Public Overrides Function VisitTupleLiteral(ByVal node As BoundTupleLiteral) As BoundNode
			Return Me.VisitTupleExpression(node)
		End Function

		Public Overrides Function VisitUnaryOperator(ByVal node As BoundUnaryOperator) As BoundNode
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundNode Microsoft.CodeAnalysis.VisualBasic.LocalRewriter::VisitUnaryOperator(Microsoft.CodeAnalysis.VisualBasic.BoundUnaryOperator)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundNode VisitUnaryOperator(Microsoft.CodeAnalysis.VisualBasic.BoundUnaryOperator)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Overrides Function VisitUnstructuredExceptionHandlingStatement(ByVal node As BoundUnstructuredExceptionHandlingStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			If (node.TrackLineNumber) Then
				Dim syntheticBoundNodeFactory As Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory = New Microsoft.CodeAnalysis.VisualBasic.SyntheticBoundNodeFactory(Me._topMethod, Me._currentMethodOrLambda, node.Syntax, Me._compilationState, Me._diagnostics)
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = syntheticBoundNodeFactory.SpecialType(SpecialType.System_Int32)
				Me._currentLineTemporary = New SynthesizedLocal(Me._topMethod, namedTypeSymbol, SynthesizedLocalKind.OnErrorCurrentLine, DirectCast(syntheticBoundNodeFactory.Syntax, StatementSyntax), False)
				boundBlock = If(node.ContainsOnError OrElse node.ContainsResume, Me.RewriteUnstructuredExceptionHandlingStatementIntoBlock(node), DirectCast(Me.VisitBlock(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundBlock))
				boundBlock = boundBlock.Update(boundBlock.StatementListSyntax, If(boundBlock.Locals.IsEmpty, ImmutableArray.Create(Of LocalSymbol)(Me._currentLineTemporary), boundBlock.Locals.Add(Me._currentLineTemporary)), boundBlock.Statements)
				Me._currentLineTemporary = Nothing
				boundNode = boundBlock
			Else
				boundNode = Me.RewriteUnstructuredExceptionHandlingStatementIntoBlock(node)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitUserDefinedBinaryOperator(ByVal node As BoundUserDefinedBinaryOperator) As BoundNode
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundNode Microsoft.CodeAnalysis.VisualBasic.LocalRewriter::VisitUserDefinedBinaryOperator(Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundNode VisitUserDefinedBinaryOperator(Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Overrides Function VisitUserDefinedShortCircuitingOperator(ByVal node As BoundUserDefinedShortCircuitingOperator) As BoundNode
			Dim boundSequence As BoundNode
			If (Not Me._inExpressionLambda) Then
				Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, node.LeftOperand.Type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(node.Syntax, synthesizedLocal, True, synthesizedLocal.Type)
				Me.AddPlaceholderReplacement(node.LeftOperandPlaceholder, New BoundAssignmentOperator(node.Syntax, boundLocal, Me.VisitExpressionNode(node.LeftOperand), True, synthesizedLocal.Type, False))
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.LeftTest)
				boundLocal = boundLocal.MakeRValue()
				Me.UpdatePlaceholderReplacement(node.LeftOperandPlaceholder, boundLocal)
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.BitwiseOperator)
				Me.RemovePlaceholderReplacement(node.LeftOperandPlaceholder)
				boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(node.Syntax, ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal), ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty, Me.MakeTernaryConditionalExpression(node.Syntax, boundExpression, boundLocal, boundExpression1), synthesizedLocal.Type, False)
			Else
				Dim leftOperandPlaceholder As BoundRValuePlaceholder = node.LeftOperandPlaceholder
				Dim leftOperand As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.LeftOperand
				If (leftOperandPlaceholder IsNot Nothing) Then
					Me.AddPlaceholderReplacement(leftOperandPlaceholder, Me.VisitExpression(leftOperand))
				End If
				Dim boundUserDefinedBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator = DirectCast(Me.VisitExpression(node.BitwiseOperator), Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator)
				If (leftOperandPlaceholder IsNot Nothing) Then
					Me.RemovePlaceholderReplacement(leftOperandPlaceholder)
				End If
				boundSequence = node.Update(node.LeftOperand, node.LeftOperandPlaceholder, node.LeftTest, boundUserDefinedBinaryOperator, node.Type)
			End If
			Return boundSequence
		End Function

		Public Overrides Function VisitUserDefinedUnaryOperator(ByVal node As BoundUserDefinedUnaryOperator) As BoundNode
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundNode Microsoft.CodeAnalysis.VisualBasic.LocalRewriter::VisitUserDefinedUnaryOperator(Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedUnaryOperator)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundNode VisitUserDefinedUnaryOperator(Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedUnaryOperator)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Public Overrides Function VisitUsingStatement(ByVal node As BoundUsingStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim item As ValueTuple(Of BoundRValuePlaceholder, BoundExpression, BoundExpression)
			Dim unstructuredExceptionHandlingContext As LocalRewriter.UnstructuredExceptionHandlingContext = Me.LeaveUnstructuredExceptionHandlingContext(node)
			Dim syntax As UsingBlockSyntax = DirectCast(node.Syntax, UsingBlockSyntax)
			Dim tryFinally As BoundBlock = DirectCast(Me.Visit(node.Body), BoundBlock)
			Dim locals As ImmutableArray(Of LocalSymbol) = node.Locals
			If (node.ResourceList.IsDefault) Then
				Dim resourceExpressionOpt As BoundExpression = node.ResourceExpressionOpt
				item = node.UsingInfo.PlaceholderInfo(resourceExpressionOpt.Type)
				Dim synthesizedLocal As LocalSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(Me._currentMethodOrLambda, resourceExpressionOpt.Type, SynthesizedLocalKind.[Using], syntax.UsingStatement, False)
				tryFinally = Me.RewriteSingleUsingToTryFinally(node, 0, synthesizedLocal, resourceExpressionOpt, item, tryFinally)
				locals = locals.Add(synthesizedLocal)
			Else
				For i As Integer = node.ResourceList.Length - 1 To 0 Step -1
					Dim boundLocalDeclarationBase As Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclarationBase = node.ResourceList(i)
					If (boundLocalDeclarationBase.Kind <> BoundKind.LocalDeclaration) Then
						Dim boundAsNewLocalDeclaration As BoundAsNewLocalDeclarations = DirectCast(boundLocalDeclarationBase, BoundAsNewLocalDeclarations)
						Dim length As Integer = boundAsNewLocalDeclaration.LocalDeclarations.Length
						item = node.UsingInfo.PlaceholderInfo(boundAsNewLocalDeclaration.LocalDeclarations.First().LocalSymbol.Type)
						Dim localDeclarations As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclaration) = boundAsNewLocalDeclaration.LocalDeclarations
						For j As Integer = localDeclarations.Length - 1 To 0 Step -1
							localDeclarations = boundAsNewLocalDeclaration.LocalDeclarations
							tryFinally = Me.RewriteSingleUsingToTryFinally(node, i, localDeclarations(j).LocalSymbol, boundAsNewLocalDeclaration.Initializer, item, tryFinally)
						Next

					Else
						Dim boundLocalDeclaration As Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclaration = DirectCast(boundLocalDeclarationBase, Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclaration)
						item = node.UsingInfo.PlaceholderInfo(boundLocalDeclaration.LocalSymbol.Type)
						tryFinally = Me.RewriteSingleUsingToTryFinally(node, i, boundLocalDeclaration.LocalSymbol, boundLocalDeclaration.InitializerOpt, item, tryFinally)
					End If
				Next

			End If
			Me.RestoreUnstructuredExceptionHandlingContext(node, unstructuredExceptionHandlingContext)
			Dim statements As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = tryFinally.Statements
			If (Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)) Then
				statements = Microsoft.CodeAnalysis.ImmutableArrayExtensions.Concat(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(Me.RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, True), statements)
			End If
			tryFinally = New BoundBlock(node.Syntax, tryFinally.StatementListSyntax, locals, statements, False)
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = Nothing
			If (Me(node)) Then
				boundStatement = Me._instrumenterOpt.CreateUsingStatementPrologue(node)
			End If
			boundNode = If(boundStatement Is Nothing, New BoundStatementList(node.UsingInfo.UsingStatementSyntax, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(tryFinally), False), New BoundStatementList(node.UsingInfo.UsingStatementSyntax, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)(boundStatement, tryFinally), False))
			Return boundNode
		End Function

		Public Overrides Function VisitWhileStatement(ByVal node As BoundWhileStatement) As BoundNode
			Dim flag As Boolean = Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node)
			Dim boundLabelStatement As Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement = Nothing
			Dim boundStatements As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement)()
			If (flag) Then
				boundLabelStatement = Me.RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(node.Syntax)
				boundStatements = Me.RegisterUnstructuredExceptionHandlingResumeTarget(node.Syntax, True)
			End If
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			Dim boundLabelStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement = Nothing
			If (flag) Then
				boundLabelStatement1 = Me.RegisterUnstructuredExceptionHandlingNonThrowingResumeTarget(node.Syntax)
			End If
			Return Me.RewriteWhileStatement(node, Me.VisitExpressionNode(node.Condition), boundStatement, node.ContinueLabel, node.ExitLabel, True, boundLabelStatement, boundStatements, boundLabelStatement1)
		End Function

		Public Overrides Function VisitWithLValueExpressionPlaceholder(ByVal node As BoundWithLValueExpressionPlaceholder) As BoundNode
			Return Me(node)
		End Function

		Public Overrides Function VisitWithRValueExpressionPlaceholder(ByVal node As BoundWithRValueExpressionPlaceholder) As BoundNode
			Return Me(node)
		End Function

		Public Overrides Function VisitWithStatement(ByVal node As BoundWithStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (Not node.HasErrors) Then
				Dim unstructuredExceptionHandlingContext As LocalRewriter.UnstructuredExceptionHandlingContext = Me.LeaveUnstructuredExceptionHandlingContext(node)
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(node.OriginalExpression)
				Dim type As TypeSymbol = boundExpression.Type
				Dim withStatement As WithStatementSyntax = DirectCast(node.Syntax, WithBlockSyntax).WithStatement
				Dim flag As Boolean = If(Me._currentMethodOrLambda.IsIterator OrElse Me._currentMethodOrLambda.IsAsync, True, node.Binder.ExpressionIsAccessedFromNestedLambda)
				Dim result As WithExpressionRewriter.Result = (New WithExpressionRewriter(withStatement)).AnalyzeWithExpression(Me._currentMethodOrLambda, boundExpression, flag, Nothing, False)
				Me.RestoreUnstructuredExceptionHandlingContext(node, unstructuredExceptionHandlingContext)
				boundNode = Me.RewriteWithBlockStatements(node, Me.ShouldGenerateUnstructuredExceptionHandlingResumeCode(node), result.Locals, result.Initializers, node.ExpressionPlaceholder, result.Expression)
			Else
				boundNode = node
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitXmlAttribute(ByVal node As BoundXmlAttribute) As BoundNode
			Return Me.Visit(node.ObjectCreation)
		End Function

		Public Overrides Function VisitXmlCData(ByVal node As BoundXmlCData) As BoundNode
			Return Me.Visit(node.ObjectCreation)
		End Function

		Public Overrides Function VisitXmlComment(ByVal node As BoundXmlComment) As BoundNode
			Return Me.Visit(node.ObjectCreation)
		End Function

		Private Function VisitXmlContainer(ByVal rewriterInfo As BoundXmlContainerRewriterInfo) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpressionNode(rewriterInfo.ObjectCreation)
			If (rewriterInfo.SideEffects.Length <> 0) Then
				Dim syntax As SyntaxNode = boundExpression.Syntax
				Dim type As TypeSymbol = boundExpression.Type
				Dim instance As ArrayBuilder(Of LocalSymbol) = ArrayBuilder(Of LocalSymbol).GetInstance()
				Dim boundExpressions As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Me.CreateTempLocal(syntax, type, boundExpression, boundExpressions)
				instance.Add(boundLocal.LocalSymbol)
				Me.AddPlaceholderReplacement(rewriterInfo.Placeholder, boundLocal)
				Dim boundLocal1 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Nothing
				If (rewriterInfo.XmlnsAttributesPlaceholder IsNot Nothing) Then
					boundLocal1 = Me.CreateTempLocal(syntax, rewriterInfo.XmlnsAttributesPlaceholder.Type, Me.VisitExpressionNode(rewriterInfo.XmlnsAttributes), boundExpressions)
					instance.Add(boundLocal1.LocalSymbol)
					Me.AddPlaceholderReplacement(rewriterInfo.XmlnsAttributesPlaceholder, boundLocal1)
				End If
				If (rewriterInfo.PrefixesPlaceholder IsNot Nothing) Then
					Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
					Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
					Me.CreatePrefixesAndNamespacesArrays(rewriterInfo, syntax, boundExpression1, boundExpression2)
					Dim boundLocal2 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Me.CreateTempLocal(syntax, rewriterInfo.PrefixesPlaceholder.Type, boundExpression1, boundExpressions)
					instance.Add(boundLocal2.LocalSymbol)
					Me.AddPlaceholderReplacement(rewriterInfo.PrefixesPlaceholder, boundLocal2)
					Dim boundLocal3 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Me.CreateTempLocal(syntax, rewriterInfo.NamespacesPlaceholder.Type, boundExpression2, boundExpressions)
					instance.Add(boundLocal3.LocalSymbol)
					Me.AddPlaceholderReplacement(rewriterInfo.NamespacesPlaceholder, boundLocal3)
				End If
				MyBase.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(rewriterInfo.SideEffects, boundExpressions)
				If (rewriterInfo.PrefixesPlaceholder IsNot Nothing) Then
					Me.RemovePlaceholderReplacement(rewriterInfo.PrefixesPlaceholder)
					Me.RemovePlaceholderReplacement(rewriterInfo.NamespacesPlaceholder)
				End If
				If (rewriterInfo.XmlnsAttributesPlaceholder IsNot Nothing) Then
					Me.RemovePlaceholderReplacement(rewriterInfo.XmlnsAttributesPlaceholder)
				End If
				Me.RemovePlaceholderReplacement(rewriterInfo.Placeholder)
				boundSequence = New Microsoft.CodeAnalysis.VisualBasic.BoundSequence(syntax, instance.ToImmutableAndFree(), boundExpressions.ToImmutableAndFree(), boundLocal, type, False)
			Else
				boundSequence = boundExpression
			End If
			Return boundSequence
		End Function

		Private Function VisitXmlContainerInExpressionLambda(ByVal rewriterInfo As BoundXmlContainerRewriterInfo) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundArrayCreation As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim sideEffects As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = rewriterInfo.SideEffects
			Dim objectCreation As BoundObjectCreationExpression = DirectCast(rewriterInfo.ObjectCreation, BoundObjectCreationExpression)
			If (sideEffects.Length <> 0) Then
				Dim item As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = objectCreation.Arguments(0)
				Dim constructorOpt As MethodSymbol = Nothing
				If (objectCreation.Arguments.Length <> 1) Then
					constructorOpt = objectCreation.ConstructorOpt
				Else
					constructorOpt = DirectCast(Me.Compilation.GetWellKnownTypeMember(If(sideEffects.Length = 1, WellKnownMember.System_Xml_Linq_XElement__ctor, WellKnownMember.System_Xml_Linq_XElement__ctor2)), MethodSymbol)
					If (Not Me.ReportMissingOrBadRuntimeHelper(objectCreation, WellKnownMember.System_Xml_Linq_XElement__ctor2, constructorOpt)) Then
						GoTo Label1
					End If
					boundExpression = Me.VisitExpressionNode(objectCreation)
					Return boundExpression
				End If
			Label1:
				Dim syntax As SyntaxNode = objectCreation.Syntax
				Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = objectCreation.Type
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Nothing
				If (rewriterInfo.XmlnsAttributesPlaceholder IsNot Nothing) Then
					boundLocal = Me.CreateTempLocalInExpressionLambda(syntax, rewriterInfo.XmlnsAttributesPlaceholder.Type, Me.VisitExpressionNode(rewriterInfo.XmlnsAttributes))
					Me.AddPlaceholderReplacement(rewriterInfo.XmlnsAttributesPlaceholder, boundLocal)
				End If
				If (rewriterInfo.PrefixesPlaceholder IsNot Nothing) Then
					Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
					Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
					Me.CreatePrefixesAndNamespacesArrays(rewriterInfo, syntax, boundExpression1, boundExpression2)
					Dim boundLocal1 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Me.CreateTempLocalInExpressionLambda(syntax, rewriterInfo.PrefixesPlaceholder.Type, boundExpression1)
					Me.AddPlaceholderReplacement(rewriterInfo.PrefixesPlaceholder, boundLocal1)
					Dim boundLocal2 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Me.CreateTempLocalInExpressionLambda(syntax, rewriterInfo.NamespacesPlaceholder.Type, boundExpression2)
					Me.AddPlaceholderReplacement(rewriterInfo.NamespacesPlaceholder, boundLocal2)
				End If
				Dim boundExpressionArray(sideEffects.Length - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim length As Integer = sideEffects.Length - 1
				Dim num As Integer = 0
				Do
					Dim arguments As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = DirectCast(sideEffects(num), BoundCall).Arguments
					boundExpressionArray(num) = Me.VisitExpressionNode(arguments(0))
					num = num + 1
				Loop While num <= length
				If (rewriterInfo.PrefixesPlaceholder IsNot Nothing) Then
					Me.RemovePlaceholderReplacement(rewriterInfo.PrefixesPlaceholder)
					Me.RemovePlaceholderReplacement(rewriterInfo.NamespacesPlaceholder)
				End If
				If (rewriterInfo.XmlnsAttributesPlaceholder IsNot Nothing) Then
					Me.RemovePlaceholderReplacement(rewriterInfo.XmlnsAttributesPlaceholder)
				End If
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = constructorOpt.Parameters(1).Type
				If (Not typeSymbol.IsArrayType()) Then
					boundArrayCreation = boundExpressionArray(0)
				Else
					Dim arrayTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol = DirectCast(typeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.ArrayTypeSymbol)
					boundArrayCreation = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayCreation(objectCreation.Syntax, ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(New BoundLiteral(objectCreation.Syntax, ConstantValue.Create(CInt(boundExpressionArray.Length)), Me.GetSpecialType(SpecialType.System_Int32))), New BoundArrayInitialization(objectCreation.Syntax, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpressionArray), arrayTypeSymbol, False), arrayTypeSymbol, False)
				End If
				Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(Me.VisitExpression(item), boundArrayCreation)
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				boundExpression = objectCreation.Update(constructorOpt, boundExpressions, bitVector, Nothing, objectCreation.Type)
			Else
				boundExpression = Me.VisitExpressionNode(objectCreation)
			End If
			Return boundExpression
		End Function

		Public Overrides Function VisitXmlDeclaration(ByVal node As BoundXmlDeclaration) As BoundNode
			Return Me.Visit(node.ObjectCreation)
		End Function

		Public Overrides Function VisitXmlDocument(ByVal node As BoundXmlDocument) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			boundNode = If(Not Me._inExpressionLambda OrElse node.HasErrors, Me.VisitXmlContainer(node.RewriterInfo), Me.VisitXmlContainerInExpressionLambda(node.RewriterInfo))
			Return boundNode
		End Function

		Public Overrides Function VisitXmlElement(ByVal node As BoundXmlElement) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim rewriterInfo As BoundXmlContainerRewriterInfo = node.RewriterInfo
			Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of String, String)) = Me._xmlImportedNamespaces
			If (rewriterInfo.IsRoot) Then
				Me._xmlImportedNamespaces = rewriterInfo.ImportedNamespaces
			End If
			boundNode = If(Not Me._inExpressionLambda OrElse node.HasErrors, Me.VisitXmlContainer(rewriterInfo), Me.VisitXmlContainerInExpressionLambda(rewriterInfo))
			If (rewriterInfo.IsRoot) Then
				Me._xmlImportedNamespaces = keyValuePairs
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitXmlEmbeddedExpression(ByVal node As BoundXmlEmbeddedExpression) As BoundNode
			Return Me.Visit(node.Expression)
		End Function

		Public Overrides Function VisitXmlMemberAccess(ByVal node As BoundXmlMemberAccess) As BoundNode
			Return Me.Visit(node.MemberAccess)
		End Function

		Public Overrides Function VisitXmlName(ByVal node As BoundXmlName) As BoundNode
			Return Me.Visit(node.ObjectCreation)
		End Function

		Public Overrides Function VisitXmlNamespace(ByVal node As BoundXmlNamespace) As BoundNode
			Return Me.Visit(node.ObjectCreation)
		End Function

		Public Overrides Function VisitXmlProcessingInstruction(ByVal node As BoundXmlProcessingInstruction) As BoundNode
			Return Me.Visit(node.ObjectCreation)
		End Function

		Private Shared Function WillDoAtLeastOneIteration(ByVal rewrittenInitialValue As BoundExpression, ByVal rewrittenLimit As BoundExpression, ByVal rewrittenStep As BoundExpression) As Boolean
			Dim flag As Boolean
			Dim isNegativeNumeric As Boolean
			Dim constantValueOpt As Microsoft.CodeAnalysis.ConstantValue = rewrittenInitialValue.ConstantValueOpt
			If (constantValueOpt IsNot Nothing) Then
				Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = rewrittenLimit.ConstantValueOpt
				If (constantValue IsNot Nothing) Then
					Dim constantValueOpt1 As Microsoft.CodeAnalysis.ConstantValue = rewrittenStep.ConstantValueOpt
					If (constantValueOpt1 IsNot Nothing) Then
						isNegativeNumeric = constantValueOpt1.IsNegativeNumeric
					Else
						If (rewrittenStep.Type.GetEnumUnderlyingTypeOrSelf().IsUnsignedIntegralType()) Then
							GoTo Label1
						End If
						flag = False
						Return flag
					End If
					If (constantValueOpt.IsUnsigned) Then
						Dim uInt64Value As ULong = constantValueOpt.UInt64Value
						Dim num As ULong = constantValue.UInt64Value
						flag = If(isNegativeNumeric, uInt64Value >= num, uInt64Value <= num)
					ElseIf (constantValueOpt.IsIntegral) Then
						Dim int64Value As Long = constantValueOpt.Int64Value
						Dim int64Value1 As Long = constantValue.Int64Value
						flag = If(isNegativeNumeric, int64Value >= int64Value1, int64Value <= int64Value1)
					ElseIf (Not constantValueOpt.IsDecimal) Then
						Dim doubleValue As Double = constantValueOpt.DoubleValue
						Dim doubleValue1 As Double = constantValue.DoubleValue
						flag = If(isNegativeNumeric, doubleValue >= doubleValue1, doubleValue <= doubleValue1)
					Else
						Dim decimalValue As [Decimal] = constantValueOpt.DecimalValue
						Dim decimalValue1 As [Decimal] = constantValue.DecimalValue
						flag = If(isNegativeNumeric, [Decimal].Compare(decimalValue, decimalValue1) >= 0, [Decimal].Compare(decimalValue, decimalValue1) <= 0)
					End If
				Else
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		Label1:
			isNegativeNumeric = False
			GoTo Label2
		End Function

		Private Function WrapInNullable(ByVal expr As BoundExpression, ByVal nullableType As TypeSymbol) As BoundExpression
			Dim boundBadExpression As BoundExpression
			Dim nullableMethod As MethodSymbol = Me.GetNullableMethod(expr.Syntax, nullableType, SpecialMember.System_Nullable_T__ctor)
			If (nullableMethod Is Nothing) Then
				boundBadExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundBadExpression(expr.Syntax, LookupResultKind.NotReferencable, ImmutableArray(Of Symbol).Empty, ImmutableArray.Create(Of BoundExpression)(expr), nullableType, True)
			Else
				Dim syntax As SyntaxNode = expr.Syntax
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = ImmutableArray.Create(Of BoundExpression)(expr)
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				boundBadExpression = New BoundObjectCreationExpression(syntax, nullableMethod, boundExpressions, Nothing, nullableType, False, bitVector)
			End If
			Return boundBadExpression
		End Function

		Private NotInheritable Class LocalVariableSubstituter
			Inherits BoundTreeRewriterWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
			Private ReadOnly _original As LocalSymbol

			Private ReadOnly _replacement As LocalSymbol

			Private _replacedNode As Boolean

			Private ReadOnly Property ReplacedNode As Boolean
				Get
					Return Me._replacedNode
				End Get
			End Property

			Private Sub New(ByVal original As LocalSymbol, ByVal replacement As LocalSymbol, ByVal recursionDepth As Integer)
				MyBase.New(recursionDepth)
				Me._replacedNode = False
				Me._original = original
				Me._replacement = replacement
			End Sub

			Public Shared Function Replace(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode, ByVal original As LocalSymbol, ByVal replacement As LocalSymbol, ByVal recursionDepth As Integer, ByRef replacedNode As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim localVariableSubstituter As LocalRewriter.LocalVariableSubstituter = New LocalRewriter.LocalVariableSubstituter(original, replacement, recursionDepth)
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = localVariableSubstituter.Visit(node)
				replacedNode = localVariableSubstituter.ReplacedNode
				Return boundNode
			End Function

			Public Overrides Function VisitLocal(ByVal node As BoundLocal) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				If (CObj(node.LocalSymbol) <> CObj(Me._original)) Then
					boundNode = node
				Else
					Me._replacedNode = True
					boundNode = node.Update(Me._replacement, node.IsLValue, node.Type)
				End If
				Return boundNode
			End Function
		End Class

		<Flags>
		Friend Enum RewritingFlags As Byte
			[Default] = 0
			AllowSequencePoints = 1
			AllowEndOfMethodReturnWithExpression = 2
			AllowCatchWithErrorLineNumberReference = 4
			AllowOmissionOfConditionalCalls = 8
		End Enum

		Private Structure UnstructuredExceptionHandlingContext
			Public Context As BoundUnstructuredExceptionHandlingStatement
		End Structure

		Private Structure UnstructuredExceptionHandlingState
			Public Context As BoundUnstructuredExceptionHandlingStatement

			Public ExceptionHandlers As ArrayBuilder(Of BoundGotoStatement)

			Public ResumeTargets As ArrayBuilder(Of BoundGotoStatement)

			Public OnErrorResumeNextCount As Integer

			Public ActiveHandlerTemporary As LocalSymbol

			Public ResumeTargetTemporary As LocalSymbol

			Public CurrentStatementTemporary As LocalSymbol

			Public ResumeNextLabel As LabelSymbol

			Public ResumeLabel As LabelSymbol
		End Structure

		Private Structure XmlLiteralFixupData
			Private _locals As ArrayBuilder(Of LocalRewriter.XmlLiteralFixupData.LocalWithInitialization)

			Public ReadOnly Property IsEmpty As Boolean
				Get
					Return Me._locals Is Nothing
				End Get
			End Property

			Public Sub AddLocal(ByVal local As LocalSymbol, ByVal initialization As BoundExpression)
				If (Me._locals Is Nothing) Then
					Me._locals = ArrayBuilder(Of LocalRewriter.XmlLiteralFixupData.LocalWithInitialization).GetInstance()
				End If
				Me._locals.Add(New LocalRewriter.XmlLiteralFixupData.LocalWithInitialization(local, initialization))
			End Sub

			Public Function MaterializeAndFree() As ImmutableArray(Of LocalRewriter.XmlLiteralFixupData.LocalWithInitialization)
				Dim immutableAndFree As ImmutableArray(Of LocalRewriter.XmlLiteralFixupData.LocalWithInitialization) = Me._locals.ToImmutableAndFree()
				Me._locals = Nothing
				Return immutableAndFree
			End Function

			Public Structure LocalWithInitialization
				Public ReadOnly Local As LocalSymbol

				Public ReadOnly Initialization As BoundExpression

				Public Sub New(ByVal local As LocalSymbol, ByVal initialization As BoundExpression)
					Me = New LocalRewriter.XmlLiteralFixupData.LocalWithInitialization() With
					{
						.Local = local,
						.Initialization = initialization
					}
				End Sub
			End Structure
		End Structure
	End Class
End Namespace