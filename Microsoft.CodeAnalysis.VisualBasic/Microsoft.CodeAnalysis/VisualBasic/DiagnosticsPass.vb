Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class DiagnosticsPass
		Inherits BoundTreeWalkerWithStackGuardWithoutRecursionOnTheLeftOfBinaryOperator
		Private ReadOnly _diagnostics As DiagnosticBag

		Private ReadOnly _compilation As VisualBasicCompilation

		Private _containingSymbol As MethodSymbol

		Private _withExpressionPlaceholderMap As Dictionary(Of BoundValuePlaceholderBase, BoundWithStatement)

		Private _expressionsBeingVisited As Stack(Of BoundExpression)

		Private _insideNameof As Boolean

		Private _inExpressionLambda As Boolean

		Private ReadOnly _expressionTreePlaceholders As HashSet(Of BoundNode)

		Private ReadOnly Property IsInExpressionLambda As Boolean
			Get
				Return Me._inExpressionLambda
			End Get
		End Property

		Private Sub New(ByVal compilation As VisualBasicCompilation, ByVal diagnostics As DiagnosticBag, ByVal containingSymbol As MethodSymbol)
			MyBase.New()
			Me._insideNameof = False
			Me._expressionTreePlaceholders = New HashSet(Of BoundNode)(ReferenceEqualityComparer.Instance)
			Me._compilation = compilation
			Me._diagnostics = diagnostics
			Me._containingSymbol = containingSymbol
			Me._inExpressionLambda = False
		End Sub

		Private Function CheckLambdaForByRefParameters(ByVal lambda As BoundLambda) As Boolean
			Dim flag As Boolean
			Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = lambda.LambdaSymbol.Parameters.GetEnumerator()
			While True
				If (Not enumerator.MoveNext()) Then
					flag = False
					Exit While
				ElseIf (enumerator.Current.IsByRef) Then
					Me.GenerateDiagnostic(ERRID.ERR_ByRefParamInExpressionTree, lambda)
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Sub CheckMeAccessInWithExpression(ByVal node As BoundValuePlaceholderBase)
			Dim enumerator As Stack(Of BoundExpression).Enumerator = New Stack(Of BoundExpression).Enumerator()
			Dim boundWithStatement As Microsoft.CodeAnalysis.VisualBasic.BoundWithStatement = Nothing
			If (Me._withExpressionPlaceholderMap IsNot Nothing AndAlso Me._withExpressionPlaceholderMap.TryGetValue(node, boundWithStatement)) Then
				Dim binder As WithBlockBinder = boundWithStatement.Binder
				If (binder.ContainingMember <> Me._containingSymbol AndAlso Not binder.IsInLambda) Then
					Dim containingType As NamedTypeSymbol = binder.ContainingType
					If (containingType IsNot Nothing AndAlso containingType.IsValueType AndAlso binder.Info.ExpressionHasByRefMeReference(MyBase.RecursionDepth)) Then
						Dim meAccessError As ERRID = Me.GetMeAccessError()
						If (meAccessError <> ERRID.ERR_None) Then
							Dim syntax As SyntaxNode = node.Syntax
							Try
								enumerator = Me._expressionsBeingVisited.GetEnumerator()
								While enumerator.MoveNext()
									Dim current As BoundExpression = enumerator.Current
									If (current.Syntax = syntax) Then
										Continue While
									End If
									syntax = current.Syntax
									GoTo Label0
								End While
							Finally
								DirectCast(enumerator, IDisposable).Dispose()
							End Try
						Label0:
							Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(Me._diagnostics, syntax, meAccessError)
						End If
					End If
				End If
			End If
		End Sub

		Private Sub CheckRefReturningPropertyAccess(ByVal node As BoundPropertyAccess)
			If (Me.IsInExpressionLambda AndAlso node.PropertySymbol.ReturnsByRef) Then
				Me.GenerateDiagnostic(ERRID.ERR_RefReturningCallInExpressionTree, node)
			End If
		End Sub

		Private Sub GenerateDiagnostic(ByVal code As ERRID, ByVal node As BoundNode)
			Me._diagnostics.Add(New VBDiagnostic(ErrorFactory.ErrorInfo(code), node.Syntax.GetLocation(), False))
		End Sub

		Private Sub GenerateExpressionTreeNotSupportedDiagnostic(ByVal node As BoundNode)
			Me.GenerateDiagnostic(ERRID.ERR_ExpressionTreeNotSupported, node)
		End Sub

		Private Function GetMeAccessError() As Microsoft.CodeAnalysis.VisualBasic.ERRID
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID
			Dim meParameter As ParameterSymbol = Me._containingSymbol.MeParameter
			If (meParameter Is Nothing OrElse Not meParameter.IsByRef OrElse Me._containingSymbol.MethodKind <> MethodKind.AnonymousFunction) Then
				eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None
			Else
				eRRID = If(Not Binder.IsTopMostEnclosingLambdaAQueryLambda(Me._containingSymbol, Nothing), Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_CannotLiftStructureMeLambda, Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_CannotLiftStructureMeQuery)
			End If
			Return eRRID
		End Function

		Public Shared Sub IssueDiagnostics(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode, ByVal diagnostics As DiagnosticBag, ByVal containingSymbol As MethodSymbol)
			Try
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = (New DiagnosticsPass(containingSymbol.DeclaringCompilation, diagnostics, containingSymbol)).Visit(node)
			Catch cancelledByStackGuardException As BoundTreeVisitor.CancelledByStackGuardException
				ProjectData.SetProjectError(cancelledByStackGuardException)
				cancelledByStackGuardException.AddAnError(diagnostics)
				ProjectData.ClearProjectError()
			End Try
		End Sub

		Public Overrides Function Visit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim flag As Boolean = False
			If (Me._withExpressionPlaceholderMap IsNot Nothing AndAlso Me._withExpressionPlaceholderMap.Count > 0) Then
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				If (boundExpression IsNot Nothing) Then
					Me._expressionsBeingVisited.Push(boundExpression)
					flag = True
				End If
			End If
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = MyBase.Visit(node)
			If (flag) Then
				Me._expressionsBeingVisited.Pop()
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitAnonymousTypeCreationExpression(ByVal node As BoundAnonymousTypeCreationExpression) As BoundNode
			Me.VisitList(Of BoundExpression)(node.Arguments)
			Return Nothing
		End Function

		Public Overrides Function VisitAnonymousTypePropertyAccess(ByVal node As BoundAnonymousTypePropertyAccess) As BoundNode
			If (Me.IsInExpressionLambda) Then
				Me.GenerateDiagnostic(ERRID.ERR_BadAnonymousTypeForExprTree, node)
			End If
			Return MyBase.VisitAnonymousTypePropertyAccess(node)
		End Function

		Public Overrides Function VisitArrayCreation(ByVal node As BoundArrayCreation) As BoundNode
			If (Me.IsInExpressionLambda AndAlso Not DirectCast(node.Type, ArrayTypeSymbol).IsSZArray) Then
				Dim initializerOpt As BoundArrayInitialization = node.InitializerOpt
				If (initializerOpt IsNot Nothing AndAlso Not initializerOpt.Initializers.IsEmpty) Then
					Me.GenerateDiagnostic(ERRID.ERR_ExprTreeNoMultiDimArrayCreation, node)
				End If
			End If
			Return MyBase.VisitArrayCreation(node)
		End Function

		Public Overrides Function VisitAssignmentOperator(ByVal node As BoundAssignmentOperator) As BoundNode
			If (Me.IsInExpressionLambda AndAlso (node.Left.Kind <> BoundKind.PropertyAccess OrElse node.LeftOnTheRightOpt IsNot Nothing)) Then
				Me.GenerateExpressionTreeNotSupportedDiagnostic(node)
			End If
			Return MyBase.VisitAssignmentOperator(node)
		End Function

		Public Overrides Function VisitCall(ByVal node As BoundCall) As BoundNode
			Dim method As MethodSymbol = node.Method
			If (Not method.IsShared) Then
				Me.Visit(node.ReceiverOpt)
			End If
			If (Me.IsInExpressionLambda And method.ReturnsByRef) Then
				Me.GenerateDiagnostic(ERRID.ERR_RefReturningCallInExpressionTree, node)
			End If
			Me.VisitList(Of BoundExpression)(node.Arguments)
			Return Nothing
		End Function

		Public Overrides Function VisitConditionalAccess(ByVal node As BoundConditionalAccess) As BoundNode
			If (Me.IsInExpressionLambda) Then
				Me.GenerateDiagnostic(ERRID.ERR_NullPropagatingOpInExpressionTree, node)
			End If
			Return MyBase.VisitConditionalAccess(node)
		End Function

		Public Overrides Function VisitConversion(ByVal node As BoundConversion) As BoundNode
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundNode Microsoft.CodeAnalysis.VisualBasic.DiagnosticsPass::VisitConversion(Microsoft.CodeAnalysis.VisualBasic.BoundConversion)
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

		Public Overrides Function VisitDirectCast(ByVal node As BoundDirectCast) As BoundNode
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundNode Microsoft.CodeAnalysis.VisualBasic.DiagnosticsPass::VisitDirectCast(Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundNode VisitDirectCast(Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast)
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

		Public Overrides Function VisitEventAccess(ByVal node As BoundEventAccess) As BoundNode
			If (Not node.EventSymbol.IsShared) Then
				Me.Visit(node.ReceiverOpt)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitFieldAccess(ByVal node As BoundFieldAccess) As BoundNode
			If (Not node.FieldSymbol.IsShared) Then
				Me.Visit(node.ReceiverOpt)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitLambda(ByVal node As BoundLambda) As BoundNode
			Dim flag As Boolean
			Dim kind As BoundKind
			If (Me.IsInExpressionLambda) Then
				Dim lambdaSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LambdaSymbol = node.LambdaSymbol
				If (lambdaSymbol.IsAsync OrElse lambdaSymbol.IsIterator) Then
					Me.GenerateDiagnostic(ERRID.ERR_ResumableLambdaInExpressionTree, node)
				ElseIf (node.WasCompilerGenerated OrElse node.IsSingleLine) Then
					Select Case lambdaSymbol.Syntax.Kind()
						Case SyntaxKind.SingleLineFunctionLambdaExpression
						Case SyntaxKind.SingleLineSubLambdaExpression
							flag = True
							Dim body As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = node.Body
							If (body.Statements.Length = 1 OrElse body.Statements.Length = 2 AndAlso body.Statements(1).Kind = BoundKind.ReturnStatement AndAlso DirectCast(body.Statements(1), BoundReturnStatement).ExpressionOpt Is Nothing OrElse body.Statements.Length = 3 AndAlso body.Statements(1).Kind = BoundKind.LabelStatement AndAlso body.Statements(2).Kind = BoundKind.ReturnStatement) Then
								Dim item As BoundStatement = body.Statements(0)
								While True
									kind = item.Kind
									If (kind > BoundKind.Block) Then
										GoTo Label0
									End If
									If (kind = BoundKind.ReturnStatement) Then
										Exit While
									End If
									If (kind <> BoundKind.Block) Then
										GoTo Label2
									End If
									Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(item, Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
									If (Not boundBlock.Locals.IsEmpty OrElse boundBlock.Statements.Length <> 1) Then
										GoTo Label2
									End If
									item = boundBlock.Statements(0)
								End While
								If (DirectCast(item, BoundReturnStatement).ExpressionOpt IsNot Nothing) Then
									flag = False
								End If
							End If
						Label2:
							If (Not flag) Then
								Exit Select
							End If
							Me.GenerateDiagnostic(ERRID.ERR_StatementLambdaInExpressionTree, node)
							Exit Select
						Case SyntaxKind.MultiLineFunctionLambdaExpression
						Case SyntaxKind.MultiLineSubLambdaExpression
							Me.GenerateDiagnostic(ERRID.ERR_StatementLambdaInExpressionTree, node)
							Exit Select
					End Select
				Else
					Me.GenerateDiagnostic(ERRID.ERR_StatementLambdaInExpressionTree, node)
				End If
			End If
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me._containingSymbol
			Me._containingSymbol = node.LambdaSymbol
			Me.Visit(node.Body)
			Me._containingSymbol = methodSymbol
			Return Nothing
		Label0:
			If (kind = BoundKind.ExpressionStatement OrElse CByte(kind) - CByte(BoundKind.AddHandlerStatement) <= CByte(BoundKind.OmittedArgument)) Then
				flag = False
				GoTo Label2
			Else
				GoTo Label2
			End If
		End Function

		Private Sub VisitLambdaConversion(ByVal operand As BoundExpression, ByVal relaxationLambda As BoundLambda)
			If (operand.Kind = BoundKind.Lambda AndAlso Not Me.CheckLambdaForByRefParameters(DirectCast(operand, BoundLambda)) AndAlso relaxationLambda IsNot Nothing) Then
				Me.CheckLambdaForByRefParameters(relaxationLambda)
			End If
			Me.Visit(operand)
		End Sub

		Public Overrides Function VisitLateInvocation(ByVal node As BoundLateInvocation) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (Me.IsInExpressionLambda) Then
				Me.GenerateDiagnostic(ERRID.ERR_ExprTreeNoLateBind, node)
				If (node.Member.Kind <> BoundKind.LateMemberAccess) Then
					Me.Visit(node.Member)
				End If
				Me.VisitList(Of BoundExpression)(node.ArgumentsOpt)
				boundNode = Nothing
			Else
				boundNode = MyBase.VisitLateInvocation(node)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitLateMemberAccess(ByVal node As BoundLateMemberAccess) As BoundNode
			If (Me.IsInExpressionLambda) Then
				Me.GenerateDiagnostic(ERRID.ERR_ExprTreeNoLateBind, node)
			End If
			Return MyBase.VisitLateMemberAccess(node)
		End Function

		Public Overrides Function VisitMeReference(ByVal node As BoundMeReference) As BoundNode
			Dim meAccessError As ERRID = Me.GetMeAccessError()
			If (meAccessError <> ERRID.ERR_None) Then
				Binder.ReportDiagnostic(Me._diagnostics, node.Syntax, meAccessError)
			End If
			Return MyBase.VisitMeReference(node)
		End Function

		Public Overrides Function VisitMyClassReference(ByVal node As BoundMyClassReference) As BoundNode
			Dim meAccessError As ERRID = Me.GetMeAccessError()
			If (meAccessError <> ERRID.ERR_None) Then
				Binder.ReportDiagnostic(Me._diagnostics, node.Syntax, meAccessError)
			End If
			Return MyBase.VisitMyClassReference(node)
		End Function

		Public Overrides Function VisitNameOfOperator(ByVal node As BoundNameOfOperator) As BoundNode
			Me._insideNameof = True
			Me._insideNameof = False
			Return MyBase.VisitNameOfOperator(node)
		End Function

		Public Overrides Function VisitObjectCreationExpression(ByVal node As BoundObjectCreationExpression) As BoundNode
			If (Me.IsInExpressionLambda) Then
				Dim initializerOpt As BoundObjectInitializerExpressionBase = node.InitializerOpt
				If (initializerOpt IsNot Nothing AndAlso initializerOpt.Kind = BoundKind.ObjectInitializerExpression AndAlso node.ConstantValueOpt Is Nothing AndAlso initializerOpt.Type.IsValueType AndAlso node.ConstructorOpt IsNot Nothing AndAlso node.Arguments.Length > 0) Then
					Me.GenerateExpressionTreeNotSupportedDiagnostic(initializerOpt)
				End If
			End If
			Return MyBase.VisitObjectCreationExpression(node)
		End Function

		Public Overrides Function VisitObjectInitializerExpression(ByVal node As BoundObjectInitializerExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (Me.IsInExpressionLambda) Then
				Dim placeholderOpt As BoundWithLValueExpressionPlaceholder = node.PlaceholderOpt
				Me.Visit(placeholderOpt)
				Me._expressionTreePlaceholders.Add(placeholderOpt)
				Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = node.Initializers.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As BoundAssignmentOperator = DirectCast(enumerator.Current, BoundAssignmentOperator)
					Dim left As BoundPropertyAccess = TryCast(current.Left, BoundPropertyAccess)
					If (left IsNot Nothing) Then
						Me.CheckRefReturningPropertyAccess(left)
					End If
					Me.Visit(current.Right)
				End While
				Me._expressionTreePlaceholders.Remove(placeholderOpt)
				boundNode = Nothing
			Else
				boundNode = MyBase.VisitObjectInitializerExpression(node)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitParameter(ByVal node As BoundParameter) As BoundNode
			Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = node.ParameterSymbol
			If (parameterSymbol.IsByRef) Then
				Dim containingSymbol As Symbol = parameterSymbol.ContainingSymbol
				If (Me._containingSymbol <> containingSymbol AndAlso Not Me._insideNameof) Then
					If (Not Binder.IsTopMostEnclosingLambdaAQueryLambda(Me._containingSymbol, containingSymbol)) Then
						Binder.ReportDiagnostic(Me._diagnostics, node.Syntax, ERRID.ERR_CannotLiftByRefParamLambda1, New [Object]() { parameterSymbol.Name })
					Else
						Binder.ReportDiagnostic(Me._diagnostics, node.Syntax, ERRID.ERR_CannotLiftByRefParamQuery1, New [Object]() { parameterSymbol.Name })
					End If
				End If
			End If
			Return MyBase.VisitParameter(node)
		End Function

		Public Overrides Function VisitPropertyAccess(ByVal node As BoundPropertyAccess) As BoundNode
			If (Not node.PropertySymbol.IsShared) Then
				Me.Visit(node.ReceiverOpt)
			End If
			Me.CheckRefReturningPropertyAccess(node)
			Me.VisitList(Of BoundExpression)(node.Arguments)
			Return Nothing
		End Function

		Public Overrides Function VisitQueryLambda(ByVal node As BoundQueryLambda) As BoundNode
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me._containingSymbol
			Me._containingSymbol = node.LambdaSymbol
			Me.Visit(node.Expression)
			Me._containingSymbol = methodSymbol
			Return Nothing
		End Function

		Public Overrides Function VisitSequence(ByVal node As BoundSequence) As BoundNode
			If (Not node.Locals.IsEmpty AndAlso Me.IsInExpressionLambda) Then
				Me.GenerateExpressionTreeNotSupportedDiagnostic(node)
			End If
			Return MyBase.VisitSequence(node)
		End Function

		Public Overrides Function VisitTryCast(ByVal node As BoundTryCast) As BoundNode
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundNode Microsoft.CodeAnalysis.VisualBasic.DiagnosticsPass::VisitTryCast(Microsoft.CodeAnalysis.VisualBasic.BoundTryCast)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.BoundNode VisitTryCast(Microsoft.CodeAnalysis.VisualBasic.BoundTryCast)
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

		Public Overrides Function VisitUserDefinedBinaryOperator(ByVal node As BoundUserDefinedBinaryOperator) As BoundNode
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.BoundNode Microsoft.CodeAnalysis.VisualBasic.DiagnosticsPass::VisitUserDefinedBinaryOperator(Microsoft.CodeAnalysis.VisualBasic.BoundUserDefinedBinaryOperator)
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

		Public Overrides Function VisitUserDefinedUnaryOperator(ByVal node As BoundUserDefinedUnaryOperator) As BoundNode
			If (Me.IsInExpressionLambda) Then
				Dim operatorKind As UnaryOperatorKind = node.OperatorKind And UnaryOperatorKind.OpMask
				Dim flag As Boolean = CInt((node.OperatorKind And UnaryOperatorKind.Lifted)) <> 0
				If (CInt(operatorKind) - CInt(UnaryOperatorKind.Plus) <= CInt(UnaryOperatorKind.Minus) AndAlso flag AndAlso node.[Call].Method.ReturnType.IsNullableType()) Then
					Me.GenerateExpressionTreeNotSupportedDiagnostic(node)
				End If
			End If
			Return MyBase.VisitUserDefinedUnaryOperator(node)
		End Function

		Public Overrides Function VisitWithLValueExpressionPlaceholder(ByVal node As BoundWithLValueExpressionPlaceholder) As BoundNode
			If (Me._expressionTreePlaceholders.Contains(node)) Then
				Me.GenerateExpressionTreeNotSupportedDiagnostic(node)
			End If
			Me.CheckMeAccessInWithExpression(node)
			Return MyBase.VisitWithLValueExpressionPlaceholder(node)
		End Function

		Public Overrides Function VisitWithRValueExpressionPlaceholder(ByVal node As BoundWithRValueExpressionPlaceholder) As BoundNode
			Me.CheckMeAccessInWithExpression(node)
			Return MyBase.VisitWithRValueExpressionPlaceholder(node)
		End Function

		Public Overrides Function VisitWithStatement(ByVal node As BoundWithStatement) As BoundNode
			Me.Visit(node.OriginalExpression)
			Dim info As WithBlockBinder.WithBlockInfo = node.Binder.Info
			If (info Is Nothing OrElse Not info.ExpressionIsAccessedFromNestedLambda) Then
				info = Nothing
			Else
				If (Me._withExpressionPlaceholderMap Is Nothing) Then
					Me._withExpressionPlaceholderMap = New Dictionary(Of BoundValuePlaceholderBase, BoundWithStatement)()
					Me._expressionsBeingVisited = New Stack(Of BoundExpression)()
				End If
				Me._withExpressionPlaceholderMap.Add(info.ExpressionPlaceholder, node)
			End If
			Me.Visit(node.Body)
			If (info IsNot Nothing) Then
				Me._withExpressionPlaceholderMap.Remove(info.ExpressionPlaceholder)
			End If
			Return Nothing
		End Function
	End Class
End Namespace