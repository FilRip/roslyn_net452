Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.CodeGen
	Friend Class StackScheduler
		Public Sub New()
			MyBase.New()
		End Sub

		Private Shared Function FilterValidStackLocals(ByVal info As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, StackScheduler.LocalDefUseInfo)) As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, StackScheduler.LocalDefUseInfo)
			Dim localSymbols As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, StackScheduler.LocalDefUseInfo)
			Dim enumerator As List(Of StackScheduler.LocalDefUseInfo).Enumerator = New List(Of StackScheduler.LocalDefUseInfo).Enumerator()
			Dim enumerator1 As List(Of StackScheduler.LocalDefUseSpan).Enumerator = New List(Of StackScheduler.LocalDefUseSpan).Enumerator()
			Dim enumerator2 As IEnumerator(Of KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, StackScheduler.LocalDefUseInfo)) = Nothing
			Dim enumerator3 As List(Of StackScheduler.LocalDefUseSpan).Enumerator = New List(Of StackScheduler.LocalDefUseSpan).Enumerator()
			Dim count As Func(Of KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, StackScheduler.LocalDefUseInfo), Boolean)
			Dim func As Func(Of KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, StackScheduler.LocalDefUseInfo), Integer)
			Dim start As Func(Of KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, StackScheduler.LocalDefUseInfo), Integer)
			Dim func1 As Func(Of KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, StackScheduler.LocalDefUseInfo), KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, StackScheduler.LocalDefUseInfo))
			Dim localDefUseInfos As List(Of StackScheduler.LocalDefUseInfo) = New List(Of StackScheduler.LocalDefUseInfo)()
			Dim array As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol() = info.Keys.ToArray()
			Dim num As Integer = 0
			Do
				Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = array(num)
				Dim item As StackScheduler.LocalDefUseInfo = info(localSymbol)
				If (TypeOf localSymbol Is StackScheduler.DummyLocal) Then
					localDefUseInfos.Add(item)
					info.Remove(localSymbol)
				ElseIf (item.CannotSchedule) Then
					info.Remove(localSymbol)
				End If
				num = num + 1
			Loop While num < CInt(array.Length)
			If (info.Count <> 0) Then
				Dim localDefUseSpans As List(Of StackScheduler.LocalDefUseSpan) = New List(Of StackScheduler.LocalDefUseSpan)()
				Try
					enumerator = localDefUseInfos.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As StackScheduler.LocalDefUseInfo = enumerator.Current
						Try
							enumerator1 = current.localDefs.GetEnumerator()
							While enumerator1.MoveNext()
								Dim localDefUseSpan As StackScheduler.LocalDefUseSpan = enumerator1.Current
								If (localDefUseSpan.Start >= localDefUseSpan.[End]) Then
									Continue While
								End If
								localDefUseSpans.Add(localDefUseSpan)
							End While
						Finally
							DirectCast(enumerator1, IDisposable).Dispose()
						End Try
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
				Using count1 As Integer = localDefUseSpans.Count
					Dim localSymbols1 As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, StackScheduler.LocalDefUseInfo) = info
					If (StackScheduler._Closure$__.$I8-0 Is Nothing) Then
						count = Function(i As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, StackScheduler.LocalDefUseInfo)) i.Value.localDefs.Count > 0
						StackScheduler._Closure$__.$I8-0 = count
					Else
						count = StackScheduler._Closure$__.$I8-0
					End If
					Dim keyValuePairs As IEnumerable(Of KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, StackScheduler.LocalDefUseInfo)) = localSymbols1.Where(count)
					If (StackScheduler._Closure$__.$I8-1 Is Nothing) Then
						func = Function(i As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, StackScheduler.LocalDefUseInfo)) i.Value.localDefs.Count
						StackScheduler._Closure$__.$I8-1 = func
					Else
						func = StackScheduler._Closure$__.$I8-1
					End If
					Dim keyValuePairs1 As IOrderedEnumerable(Of KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, StackScheduler.LocalDefUseInfo)) = keyValuePairs.OrderByDescending(Of Integer)(func)
					If (StackScheduler._Closure$__.$I8-2 Is Nothing) Then
						start = Function(i As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, StackScheduler.LocalDefUseInfo)) i.Value.localDefs(0).Start
						StackScheduler._Closure$__.$I8-2 = start
					Else
						start = StackScheduler._Closure$__.$I8-2
					End If
					Dim keyValuePairs2 As IOrderedEnumerable(Of KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, StackScheduler.LocalDefUseInfo)) = keyValuePairs1.ThenByDescending(Of Integer)(start)
					If (StackScheduler._Closure$__.$I8-3 Is Nothing) Then
						func1 = Function(i As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, StackScheduler.LocalDefUseInfo)) i
						StackScheduler._Closure$__.$I8-3 = func1
					Else
						func1 = StackScheduler._Closure$__.$I8-3
					End If
					enumerator2 = keyValuePairs2.[Select](Of KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, StackScheduler.LocalDefUseInfo))(func1).GetEnumerator()
					While enumerator2.MoveNext()
						Dim keyValuePair As KeyValuePair(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, StackScheduler.LocalDefUseInfo) = enumerator2.Current
						If (Not info.ContainsKey(keyValuePair.Key)) Then
							Continue While
						End If
						Dim flag As Boolean = False
						Dim instance As ArrayBuilder(Of StackScheduler.LocalDefUseSpan) = ArrayBuilder(Of StackScheduler.LocalDefUseSpan).GetInstance()
						Try
							enumerator3 = keyValuePair.Value.localDefs.GetEnumerator()
							While enumerator3.MoveNext()
								Dim current1 As StackScheduler.LocalDefUseSpan = enumerator3.Current
								Dim num1 As Integer = count1 - 1
								Dim num2 As Integer = 0
								While num2 <= num1
									If (Not current1.ConflictsWithDummy(localDefUseSpans(num2))) Then
										num2 = num2 + 1
									Else
										flag = True
										Exit While
									End If
								End While
								If (Not flag) Then
									Dim count2 As Integer = localDefUseSpans.Count - 1
									Dim num3 As Integer = count1
									While num3 <= count2
										If (Not current1.ConflictsWith(localDefUseSpans(num3))) Then
											num3 = num3 + 1
										Else
											flag = True
											Exit While
										End If
									End While
								End If
								If (Not flag) Then
									instance.Add(current1)
								Else
									info.Remove(keyValuePair.Key)
									GoTo Label0
								End If
							End While
						Finally
							DirectCast(enumerator3, IDisposable).Dispose()
						End Try
					Label0:
						If (Not flag) Then
							localDefUseSpans.AddRange(instance)
						End If
						instance.Free()
					End While
				End Using
				localSymbols = info
			Else
				localSymbols = info
			End If
			Return localSymbols
		End Function

		Public Shared Function OptimizeLocalsOut(ByVal container As Symbol, ByVal src As Microsoft.CodeAnalysis.VisualBasic.BoundStatement, ByVal debugFriendly As Boolean, <Out> ByRef stackLocals As HashSet(Of LocalSymbol)) As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement
			Dim localSymbols As Dictionary(Of LocalSymbol, StackScheduler.LocalDefUseInfo) = Nothing
			src = DirectCast(StackScheduler.Analyzer.Analyze(container, src, debugFriendly, localSymbols), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
			localSymbols = StackScheduler.FilterValidStackLocals(localSymbols)
			If (localSymbols.Count <> 0) Then
				stackLocals = New HashSet(Of LocalSymbol)(localSymbols.Keys)
				boundStatement = StackScheduler.Rewriter.Rewrite(src, localSymbols)
			Else
				stackLocals = New HashSet(Of LocalSymbol)()
				boundStatement = src
			End If
			Return boundStatement
		End Function

		Private NotInheritable Class Analyzer
			Inherits BoundTreeRewriter
			Private ReadOnly _container As Symbol

			Private _counter As Integer

			<TupleElementNames(New String() { "expression", "context" })>
			Private ReadOnly _evalStack As ArrayBuilder(Of ValueTuple(Of BoundExpression, StackScheduler.ExprContext))

			Private ReadOnly _debugFriendly As Boolean

			Private _context As StackScheduler.ExprContext

			Private _assignmentLocal As BoundLocal

			Private ReadOnly _locals As Dictionary(Of LocalSymbol, StackScheduler.LocalDefUseInfo)

			Private ReadOnly _empty As StackScheduler.DummyLocal

			Private ReadOnly _dummyVariables As Dictionary(Of Object, StackScheduler.DummyLocal)

			Private _recursionDepth As Integer

			Private Sub New(ByVal container As Symbol, ByVal evalStack As ArrayBuilder(Of ValueTuple(Of BoundExpression, StackScheduler.ExprContext)), ByVal debugFriendly As Boolean)
				MyBase.New()
				Me._counter = 0
				Me._context = StackScheduler.ExprContext.None
				Me._assignmentLocal = Nothing
				Me._locals = New Dictionary(Of LocalSymbol, StackScheduler.LocalDefUseInfo)()
				Me._dummyVariables = New Dictionary(Of Object, StackScheduler.DummyLocal)()
				Me._container = container
				Me._evalStack = evalStack
				Me._debugFriendly = debugFriendly
				Me._empty = New StackScheduler.DummyLocal(container)
				Me.DeclareLocal(Me._empty, 0)
				Me.RecordDummyWrite(Me._empty)
			End Sub

			Public Shared Function Analyze(ByVal container As Symbol, ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode, ByVal debugFriendly As Boolean, <Out> ByRef locals As Dictionary(Of LocalSymbol, StackScheduler.LocalDefUseInfo)) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim instance As ArrayBuilder(Of ValueTuple(Of BoundExpression, StackScheduler.ExprContext)) = ArrayBuilder(Of ValueTuple(Of BoundExpression, StackScheduler.ExprContext)).GetInstance()
				Dim analyzer As StackScheduler.Analyzer = New StackScheduler.Analyzer(container, instance, debugFriendly)
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = analyzer.Visit(node)
				instance.Free()
				locals = analyzer._locals
				Return boundNode
			End Function

			Private Function CanScheduleToStack(ByVal local As LocalSymbol) As Boolean
				If (Not local.CanScheduleToStack) Then
					Return False
				End If
				If (Not Me._debugFriendly) Then
					Return True
				End If
				Return Not local.SynthesizedKind.IsLongLived()
			End Function

			Private Sub ClearEvalStack()
				Me._evalStack.Clear()
			End Sub

			Private Sub DeclareLocal(ByVal local As LocalSymbol, ByVal stack As Integer)
				If (local IsNot Nothing AndAlso Me.CanScheduleToStack(local)) Then
					Me._locals.Add(local, New StackScheduler.LocalDefUseInfo(stack))
				End If
			End Sub

			Private Sub DeclareLocals(ByVal locals As ImmutableArray(Of LocalSymbol), ByVal stack As Integer)
				Dim enumerator As ImmutableArray(Of LocalSymbol).Enumerator = locals.GetEnumerator()
				While enumerator.MoveNext()
					Me.DeclareLocal(enumerator.Current, stack)
				End While
			End Sub

			Private Sub EnsureOnlyEvalStack()
				Me.RecordVarRead(Me._empty)
			End Sub

			Private Sub EnsureStackState(ByVal cookie As Object)
				Me.RecordVarRead(Me._dummyVariables(RuntimeHelpers.GetObjectValue(cookie)))
			End Sub

			Private Function EvalStackHasLocal(ByVal local As LocalSymbol) As Boolean
				' 
				' Current member / type: System.Boolean Microsoft.CodeAnalysis.VisualBasic.CodeGen.StackScheduler/Analyzer::EvalStackHasLocal(Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: System.Boolean EvalStackHasLocal(Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
				' 
				' La méthode spécifiée n'est pas prise en charge.
				'    à Telerik.JustDecompiler.Common.Extensions.(TypeReference , TypeReference& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Common\Extensions.cs:ligne 113
				'    à ..(Expression , TypeReference& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 240
				'    à ..(BinaryExpression ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 221
				'    à ..(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 97
				'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 276
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 44
				'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
				'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
				'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Function

			Private Function EvalStackIsEmpty() As Boolean
				Return Me.StackDepth() = 0
			End Function

			Private Function GetStackStateCookie() As Object
				Dim dummyLocal As StackScheduler.DummyLocal = New StackScheduler.DummyLocal(Me._container)
				Me._dummyVariables.Add(dummyLocal, dummyLocal)
				Me._locals.Add(dummyLocal, New StackScheduler.LocalDefUseInfo(Me.StackDepth()))
				Me.RecordDummyWrite(dummyLocal)
				Return dummyLocal
			End Function

			Private Shared Function IsByRefVariable(ByVal node As BoundExpression) As Boolean
				Dim returnsByRef As Boolean
				Dim kind As BoundKind = node.Kind
				If (kind <= BoundKind.[Call]) Then
					If (kind <= BoundKind.ModuleVersionId) Then
						If (kind = BoundKind.ArrayAccess) Then
							returnsByRef = False
							Return returnsByRef
						End If
						If (CByte(kind) - CByte(BoundKind.InstrumentationPayloadRoot) > CByte(BoundKind.OmittedArgument)) Then
							Throw ExceptionUtilities.UnexpectedValue(node.Kind)
						End If
						returnsByRef = False
					ElseIf (kind = BoundKind.ReferenceAssignment) Then
						returnsByRef = True
					Else
						If (kind <> BoundKind.[Call]) Then
							Throw ExceptionUtilities.UnexpectedValue(node.Kind)
						End If
						returnsByRef = DirectCast(node, BoundCall).Method.ReturnsByRef
					End If
				ElseIf (kind <= BoundKind.Sequence) Then
					If (kind = BoundKind.FieldAccess) Then
						returnsByRef = False
						Return returnsByRef
					End If
					If (kind <> BoundKind.Sequence) Then
						Throw ExceptionUtilities.UnexpectedValue(node.Kind)
					End If
					returnsByRef = False
				ElseIf (kind = BoundKind.ValueTypeMeReference) Then
					returnsByRef = True
				Else
					Select Case kind
						Case BoundKind.Local
							returnsByRef = DirectCast(node, BoundLocal).LocalSymbol.IsByRef
							Exit Select
						Case BoundKind.PseudoVariable
							returnsByRef = True
							Exit Select
						Case BoundKind.Parameter
							returnsByRef = DirectCast(node, BoundParameter).ParameterSymbol.IsByRef
							Exit Select
						Case Else
							Throw ExceptionUtilities.UnexpectedValue(node.Kind)
					End Select
				End If
				Return returnsByRef
			End Function

			Private Shared Function IsIndirectAssignment(ByVal node As BoundAssignmentOperator) As Boolean
				Return StackScheduler.Analyzer.IsByRefVariable(node.Left)
			End Function

			Private Shared Function IsVerifierRef(ByVal type As TypeSymbol) As Boolean
				If (type.TypeKind = Microsoft.CodeAnalysis.TypeKind.TypeParameter) Then
					Return False
				End If
				Return type.IsReferenceType
			End Function

			Private Shared Function IsVerifierVal(ByVal type As TypeSymbol) As Boolean
				If (type.TypeKind = Microsoft.CodeAnalysis.TypeKind.TypeParameter) Then
					Return False
				End If
				Return type.IsValueType
			End Function

			Private Function LhsUsesStackWhenAssignedTo(ByVal node As BoundNode, ByVal context As StackScheduler.ExprContext) As Boolean
				Dim isShared As Boolean
				If (node IsNot Nothing) Then
					Dim kind As BoundKind = node.Kind
					If (kind > BoundKind.Sequence) Then
						If (kind <> BoundKind.Local AndAlso kind <> BoundKind.Parameter) Then
							GoTo Label1
						End If
						isShared = False
						Return isShared
					ElseIf (kind = BoundKind.FieldAccess) Then
						isShared = Not DirectCast(node, BoundFieldAccess).FieldSymbol.IsShared
						Return isShared
					Else
						If (kind <> BoundKind.Sequence) Then
							GoTo Label1
						End If
						isShared = Me.LhsUsesStackWhenAssignedTo(DirectCast(node, BoundSequence).ValueOpt, context)
						Return isShared
					End If
				Label1:
					isShared = True
				Else
					isShared = False
				End If
				Return isShared
			End Function

			Private Sub PopEvalStack()
				Me.SetStackDepth(Me._evalStack.Count - 1)
			End Sub

			Private Sub PushEvalStack(ByVal result As BoundExpression, ByVal context As StackScheduler.ExprContext)
				Me._evalStack.Add(New ValueTuple(Of BoundExpression, StackScheduler.ExprContext)(result, context))
			End Sub

			Private Sub RecordBranch(ByVal label As LabelSymbol)
				Dim dummyLocal As StackScheduler.DummyLocal = Nothing
				If (Me._dummyVariables.TryGetValue(label, dummyLocal)) Then
					Me.RecordVarRead(dummyLocal)
					Return
				End If
				dummyLocal = New StackScheduler.DummyLocal(Me._container)
				Me._dummyVariables.Add(label, dummyLocal)
				Me._locals.Add(dummyLocal, New StackScheduler.LocalDefUseInfo(Me.StackDepth()))
				Me.RecordDummyWrite(dummyLocal)
			End Sub

			Private Sub RecordDummyWrite(ByVal local As LocalSymbol)
				Dim item As StackScheduler.LocalDefUseInfo = Me._locals(local)
				Dim localDefUseSpan As StackScheduler.LocalDefUseSpan = New StackScheduler.LocalDefUseSpan(Me._counter)
				item.localDefs.Add(localDefUseSpan)
			End Sub

			Private Sub RecordLabel(ByVal label As LabelSymbol)
				Dim dummyLocal As StackScheduler.DummyLocal = Nothing
				If (Me._dummyVariables.TryGetValue(label, dummyLocal)) Then
					Me.RecordVarRead(dummyLocal)
					Return
				End If
				dummyLocal = Me._empty
				Me._dummyVariables.Add(label, dummyLocal)
				Me.RecordVarRead(dummyLocal)
			End Sub

			Private Sub RecordVarRead(ByVal local As LocalSymbol)
				If (Me.CanScheduleToStack(local)) Then
					Dim item As StackScheduler.LocalDefUseInfo = Me._locals(local)
					If (Not item.CannotSchedule) Then
						If (item.localDefs.Count = 0) Then
							item.ShouldNotSchedule()
							Return
						End If
						If (Not TypeOf local Is StackScheduler.DummyLocal AndAlso item.StackAtDeclaration <> Me.StackDepth() AndAlso Not Me.EvalStackHasLocal(local)) Then
							item.ShouldNotSchedule()
							Return
						End If
						item.localDefs.Last().SetEnd(Me._counter)
						Dim localDefUseSpan As StackScheduler.LocalDefUseSpan = New StackScheduler.LocalDefUseSpan(Me._counter)
						item.localDefs.Add(localDefUseSpan)
					End If
				End If
			End Sub

			Private Sub RecordVarRef(ByVal local As LocalSymbol)
				If (Me.CanScheduleToStack(local)) Then
					Me.ShouldNotSchedule(local)
				End If
			End Sub

			Private Sub RecordVarWrite(ByVal local As LocalSymbol)
				If (Me.CanScheduleToStack(local)) Then
					Dim item As StackScheduler.LocalDefUseInfo = Me._locals(local)
					If (Not item.CannotSchedule) Then
						If (item.StackAtDeclaration <> Me.StackDepth() - 1) Then
							item.ShouldNotSchedule()
							Return
						End If
						Dim localDefUseSpan As StackScheduler.LocalDefUseSpan = New StackScheduler.LocalDefUseSpan(Me._counter)
						item.localDefs.Add(localDefUseSpan)
					End If
				End If
			End Sub

			Private Sub SetStackDepth(ByVal depth As Integer)
				Me._evalStack.Clip(depth)
			End Sub

			Private Sub ShouldNotSchedule(ByVal local As LocalSymbol)
				Dim localDefUseInfo As StackScheduler.LocalDefUseInfo = Nothing
				If (Me._locals.TryGetValue(local, localDefUseInfo)) Then
					localDefUseInfo.ShouldNotSchedule()
				End If
			End Sub

			Private Function StackDepth() As Integer
				Return Me._evalStack.Count
			End Function

			Public Overrides Function Visit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				If (boundExpression Is Nothing) Then
					boundNode = Me.VisitStatement(node)
				Else
					boundNode = Me.VisitExpression(boundExpression, StackScheduler.ExprContext.Value)
				End If
				Return boundNode
			End Function

			Private Function VisitArguments(ByVal arguments As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression), ByVal parameters As ImmutableArray(Of ParameterSymbol)) As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Nothing
				Dim length As Integer = arguments.Length - 1
				Dim num As Integer = 0
				Do
					Dim exprContext As StackScheduler.ExprContext = If(num = parameters.Length OrElse Not parameters(num).IsByRef, StackScheduler.ExprContext.Value, StackScheduler.ExprContext.Address)
					Dim item As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = arguments(num)
					Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(item, exprContext)
					If (instance Is Nothing AndAlso item <> boundExpression) Then
						instance = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
						instance.AddRange(arguments, num)
					End If
					If (instance IsNot Nothing) Then
						instance.Add(boundExpression)
					End If
					num = num + 1
				Loop While num <= length
				If (instance Is Nothing) Then
					Return arguments
				End If
				Return instance.ToImmutableAndFree()
			End Function

			Public Overrides Function VisitArrayAccess(ByVal node As BoundArrayAccess) As BoundNode
				Dim exprContext As StackScheduler.ExprContext = Me._context
				Me._context = StackScheduler.ExprContext.Value
				Me._context = exprContext
				Return MyBase.VisitArrayAccess(node)
			End Function

			Public Overrides Function VisitArrayInitialization(ByVal node As BoundArrayInitialization) As BoundNode
				Me.EnsureOnlyEvalStack()
				Dim initializers As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = node.Initializers
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Nothing
				If (Not initializers.IsDefault) Then
					Dim length As Integer = initializers.Length - 1
					For i As Integer = 0 To length
						Me.EnsureOnlyEvalStack()
						Dim item As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = initializers(i)
						Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(item, StackScheduler.ExprContext.Value)
						If (instance Is Nothing AndAlso boundExpression <> item) Then
							instance = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
							instance.AddRange(initializers, i)
						End If
						If (instance IsNot Nothing) Then
							instance.Add(boundExpression)
						End If
					Next

				End If
				Return node.Update(If(instance IsNot Nothing, instance.ToImmutableAndFree(), initializers), node.Type)
			End Function

			Public Overrides Function VisitAssignmentOperator(ByVal node As BoundAssignmentOperator) As BoundNode
				Dim exprContext As StackScheduler.ExprContext
				Dim flag As Boolean = StackScheduler.Analyzer.IsIndirectAssignment(node)
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(node.Left, If(flag, StackScheduler.ExprContext.Address, StackScheduler.ExprContext.AssignmentTarget))
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Me._assignmentLocal
				Me._assignmentLocal = Nothing
				exprContext = If(Me._context <> StackScheduler.ExprContext.Address, StackScheduler.ExprContext.Value, StackScheduler.ExprContext.Address)
				Dim right As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Right
				Dim type As TypeSymbol = boundExpression.Type
				Dim flag1 As Boolean = False
				If (right.Kind = BoundKind.ObjectCreationExpression) Then
					Dim constructorOpt As MethodSymbol = DirectCast(right, BoundObjectCreationExpression).ConstructorOpt
					If (constructorOpt IsNot Nothing AndAlso constructorOpt.ParameterCount <> 0) Then
						flag1 = True
					End If
				End If
				If (flag1) Then
					Me.PushEvalStack(Nothing, StackScheduler.ExprContext.None)
				End If
				right = Me.VisitExpression(node.Right, exprContext)
				If (flag1) Then
					Me.PopEvalStack()
				End If
				If (boundLocal IsNot Nothing) Then
					Me.RecordVarWrite(boundLocal.LocalSymbol)
				End If
				Return node.Update(boundExpression, Nothing, right, node.SuppressObjectClone, node.Type)
			End Function

			Public Overrides Function VisitBinaryConditionalExpression(ByVal node As BoundBinaryConditionalExpression) As BoundNode
				Dim num As Integer = Me.StackDepth()
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.TestExpression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Dim objectValue As Object = RuntimeHelpers.GetObjectValue(Me.GetStackStateCookie())
				Me.SetStackDepth(num)
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ElseExpression), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Me.EnsureStackState(RuntimeHelpers.GetObjectValue(objectValue))
				Return node.Update(boundExpression, Nothing, Nothing, boundExpression1, node.ConstantValueOpt, node.Type)
			End Function

			Public Overrides Function VisitBinaryOperator(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim flag As Boolean
				Dim left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Left
				If (left.Kind <> BoundKind.BinaryOperator OrElse left.ConstantValueOpt IsNot Nothing) Then
					boundNode = Me.VisitBinaryOperatorSimple(node)
				Else
					Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator).GetInstance()
					instance.Push(node)
					Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = DirectCast(left, Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
					While True
						instance.Push(boundBinaryOperator)
						left = boundBinaryOperator.Left
						If (left.Kind <> BoundKind.BinaryOperator OrElse left.ConstantValueOpt IsNot Nothing) Then
							Exit While
						End If
						boundBinaryOperator = DirectCast(left, Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
					End While
					Dim num As Integer = Me.StackDepth()
					Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(left), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
					While True
						boundBinaryOperator = instance.Pop()
						Dim objectValue As Object = Nothing
						Dim operatorKind As BinaryOperatorKind = boundBinaryOperator.OperatorKind And BinaryOperatorKind.OpMask
						If (operatorKind = BinaryOperatorKind.[OrElse] OrElse operatorKind = BinaryOperatorKind.[AndAlso]) Then
							flag = True
							objectValue = RuntimeHelpers.GetObjectValue(Me.GetStackStateCookie())
							Me.SetStackDepth(num)
						Else
							flag = False
						End If
						Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(boundBinaryOperator.Right), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
						If (flag) Then
							Me.EnsureStackState(RuntimeHelpers.GetObjectValue(objectValue))
						End If
						Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(boundBinaryOperator.Type)
						boundExpression = boundBinaryOperator.Update(boundBinaryOperator.OperatorKind, boundExpression, boundExpression1, boundBinaryOperator.Checked, boundBinaryOperator.ConstantValueOpt, typeSymbol)
						If (instance.Count = 0) Then
							Exit While
						End If
						Me._counter = Me._counter + 1
						Me.SetStackDepth(num)
						Me.PushEvalStack(node, StackScheduler.ExprContext.Value)
					End While
					instance.Free()
					boundNode = boundExpression
				End If
				Return boundNode
			End Function

			Private Function VisitBinaryOperatorSimple(ByVal node As BoundBinaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim operatorKind As BinaryOperatorKind = node.OperatorKind And BinaryOperatorKind.OpMask
				If (operatorKind = BinaryOperatorKind.[OrElse] OrElse operatorKind = BinaryOperatorKind.[AndAlso]) Then
					Dim num As Integer = Me.StackDepth()
					Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Left), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
					Dim objectValue As Object = RuntimeHelpers.GetObjectValue(Me.GetStackStateCookie())
					Me.SetStackDepth(num)
					Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Right), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
					Me.EnsureStackState(RuntimeHelpers.GetObjectValue(objectValue))
					boundNode = node.Update(node.OperatorKind, boundExpression, boundExpression1, node.Checked, node.ConstantValueOpt, node.Type)
				Else
					boundNode = MyBase.VisitBinaryOperator(node)
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitBlock(ByVal node As BoundBlock) As BoundNode
				Me.DeclareLocals(node.Locals, 0)
				Return MyBase.VisitBlock(node)
			End Function

			Public Overrides Function VisitCall(ByVal node As BoundCall) As BoundNode
				Dim exprContext As StackScheduler.ExprContext
				Dim receiverOpt As BoundExpression = node.ReceiverOpt
				If (node.Method.IsShared) Then
					Me._counter = Me._counter + 1
				Else
					Dim type As TypeSymbol = receiverOpt.Type
					If (Not type.IsReferenceType) Then
						exprContext = StackScheduler.ExprContext.Address
					Else
						exprContext = If(Not type.IsTypeParameter(), StackScheduler.ExprContext.Value, StackScheduler.ExprContext.Box)
					End If
					receiverOpt = Me.VisitExpression(receiverOpt, exprContext)
				End If
				Dim method As MethodSymbol = node.Method
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = Me.VisitArguments(node.Arguments, method.Parameters)
				Return node.Update(method, node.MethodGroupOpt, receiverOpt, boundExpressions, node.DefaultArguments, node.ConstantValueOpt, node.IsLValue, node.SuppressObjectClone, node.Type)
			End Function

			Public Overrides Function VisitCaseBlock(ByVal node As BoundCaseBlock) As BoundNode
				Me.EnsureOnlyEvalStack()
				Dim boundCaseStatement As Microsoft.CodeAnalysis.VisualBasic.BoundCaseStatement = DirectCast(Me.Visit(node.CaseStatement), Microsoft.CodeAnalysis.VisualBasic.BoundCaseStatement)
				Me.EnsureOnlyEvalStack()
				Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
				Me.EnsureOnlyEvalStack()
				Return node.Update(boundCaseStatement, boundBlock)
			End Function

			Public Overrides Function VisitCatchBlock(ByVal node As BoundCatchBlock) As BoundNode
				Dim num As Integer = Me.StackDepth()
				Me.DeclareLocal(node.LocalOpt, num)
				Me.EnsureOnlyEvalStack()
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(node.ExceptionSourceOpt, StackScheduler.ExprContext.Value)
				Me.SetStackDepth(num)
				Me.EnsureOnlyEvalStack()
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(node.ErrorLineNumberOpt, StackScheduler.ExprContext.Value)
				Me.SetStackDepth(num)
				Me.EnsureOnlyEvalStack()
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(node.ExceptionFilterOpt, StackScheduler.ExprContext.Value)
				Me.SetStackDepth(num)
				Me.EnsureOnlyEvalStack()
				Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.Visit(node.Body), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
				Me.EnsureOnlyEvalStack()
				Return node.Update(node.LocalOpt, boundExpression, boundExpression1, boundExpression2, boundBlock, node.IsSynthesizedAsyncCatchAll)
			End Function

			Public Overrides Function VisitComplexConditionalAccessReceiver(ByVal node As BoundComplexConditionalAccessReceiver) As BoundNode
				Me.EnsureOnlyEvalStack()
				Dim num As Integer = Me.StackDepth()
				Me.PushEvalStack(Nothing, StackScheduler.ExprContext.None)
				Dim objectValue As Object = RuntimeHelpers.GetObjectValue(Me.GetStackStateCookie())
				Me.SetStackDepth(num)
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ValueTypeReceiver), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Me.EnsureStackState(RuntimeHelpers.GetObjectValue(objectValue))
				Me.SetStackDepth(num)
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ReferenceTypeReceiver), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Me.EnsureStackState(RuntimeHelpers.GetObjectValue(objectValue))
				Return node.Update(boundExpression, boundExpression1, node.Type)
			End Function

			Public Overrides Function VisitConditionalAccessReceiverPlaceholder(ByVal node As BoundConditionalAccessReceiverPlaceholder) As BoundNode
				Return MyBase.VisitConditionalAccessReceiverPlaceholder(node)
			End Function

			Public Overrides Function VisitConditionalGoto(ByVal node As BoundConditionalGoto) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = MyBase.VisitConditionalGoto(node)
				Me.PopEvalStack()
				Me.RecordBranch(node.Label)
				Return boundNode
			End Function

			Private Function VisitExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal context As StackScheduler.ExprContext) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Me._recursionDepth = Me._recursionDepth + 1
				If (Me._recursionDepth <= 1) Then
					boundExpression = Me.VisitExpressionCoreWithStackGuard(node, context)
				Else
					StackGuard.EnsureSufficientExecutionStack(Me._recursionDepth)
					boundExpression = Me.VisitExpressionCore(node, context)
				End If
				Me._recursionDepth = Me._recursionDepth - 1
				Return boundExpression
			End Function

			Private Function VisitExpressionCore(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal context As StackScheduler.ExprContext) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				If (node IsNot Nothing) Then
					Dim exprContext As StackScheduler.ExprContext = Me._context
					Dim num As Integer = Me.StackDepth()
					Me._context = context
					Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = If(node.ConstantValueOpt Is Nothing, DirectCast(MyBase.Visit(node), Microsoft.CodeAnalysis.VisualBasic.BoundExpression), node)
					Me._context = exprContext
					Me._counter = Me._counter + 1
					Select Case context
						Case StackScheduler.ExprContext.Sideeffects
							Me.SetStackDepth(num)
							boundExpression = boundExpression1
							Return boundExpression
						Case StackScheduler.ExprContext.Value
						Case StackScheduler.ExprContext.Address
						Case StackScheduler.ExprContext.Box
							Me.SetStackDepth(num)
							Me.PushEvalStack(node, context)
							boundExpression = boundExpression1
							Return boundExpression
						Case StackScheduler.ExprContext.AssignmentTarget
							boundExpression = boundExpression1
							Return boundExpression
					End Select
					Throw ExceptionUtilities.UnexpectedValue(context)
				Else
					Me._counter = Me._counter + 1
					boundExpression = node
				End If
				Return boundExpression
			End Function

			Private Function VisitExpressionCoreWithStackGuard(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal context As StackScheduler.ExprContext) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Try
					boundExpression = Me.VisitExpressionCore(node, context)
				Catch insufficientExecutionStackException As System.InsufficientExecutionStackException
					ProjectData.SetProjectError(insufficientExecutionStackException)
					Throw New BoundTreeVisitor.CancelledByStackGuardException(insufficientExecutionStackException, node)
				End Try
				Return boundExpression
			End Function

			Public Overrides Function VisitExpressionStatement(ByVal node As BoundExpressionStatement) As BoundNode
				Return node.Update(Me.VisitExpression(node.Expression, StackScheduler.ExprContext.Sideeffects))
			End Function

			Protected Overrides Function VisitExpressionWithoutStackGuard(ByVal node As BoundExpression) As BoundExpression
				Throw ExceptionUtilities.Unreachable
			End Function

			Public Overrides Function VisitFieldAccess(ByVal node As BoundFieldAccess) As BoundNode
				Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = node.FieldSymbol
				Dim receiverOpt As BoundExpression = node.ReceiverOpt
				If (fieldSymbol.IsShared) Then
					Me._counter = Me._counter + 1
					receiverOpt = Nothing
				ElseIf (Not receiverOpt.Type.IsTypeParameter()) Then
					receiverOpt = If(Not receiverOpt.Type.IsValueType OrElse Me._context <> StackScheduler.ExprContext.AssignmentTarget AndAlso Me._context <> StackScheduler.ExprContext.Address AndAlso Not CodeGenerator.FieldLoadMustUseRef(receiverOpt), Me.VisitExpression(receiverOpt, StackScheduler.ExprContext.Value), Me.VisitExpression(receiverOpt, StackScheduler.ExprContext.Address))
				Else
					receiverOpt = Me.VisitExpression(receiverOpt, StackScheduler.ExprContext.Box)
				End If
				Return node.Update(receiverOpt, fieldSymbol, node.IsLValue, node.SuppressVirtualCalls, Nothing, node.Type)
			End Function

			Public Overrides Function VisitGotoStatement(ByVal node As BoundGotoStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = MyBase.VisitGotoStatement(node)
				Me.RecordBranch(node.Label)
				Return boundNode
			End Function

			Public Overrides Function VisitLabelStatement(ByVal node As BoundLabelStatement) As BoundNode
				Me.RecordLabel(node.Label)
				Return MyBase.VisitLabelStatement(node)
			End Function

			Public Overrides Function VisitLocal(ByVal node As BoundLocal) As BoundNode
				If (node.ConstantValueOpt Is Nothing) Then
					Select Case Me._context
						Case StackScheduler.ExprContext.Value
						Case StackScheduler.ExprContext.Box
							Me.RecordVarRead(node.LocalSymbol)
							Exit Select
						Case StackScheduler.ExprContext.Address
							If (Not node.LocalSymbol.IsByRef) Then
								Me.RecordVarRef(node.LocalSymbol)
								Exit Select
							Else
								Me.RecordVarRead(node.LocalSymbol)
								Exit Select
							End If
						Case StackScheduler.ExprContext.AssignmentTarget
							Me._assignmentLocal = node
							Exit Select
					End Select
				End If
				Return MyBase.VisitLocal(node)
			End Function

			Public Overrides Function VisitLoweredConditionalAccess(ByVal node As BoundLoweredConditionalAccess) As BoundNode
				If (Not node.ReceiverOrCondition.Type.IsBooleanType()) Then
					Me.EnsureOnlyEvalStack()
				End If
				Dim num As Integer = Me.StackDepth()
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ReceiverOrCondition), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Dim objectValue As Object = RuntimeHelpers.GetObjectValue(Me.GetStackStateCookie())
				Me.SetStackDepth(num)
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.WhenNotNull), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Me.EnsureStackState(RuntimeHelpers.GetObjectValue(objectValue))
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				If (node.WhenNullOpt IsNot Nothing) Then
					Me.SetStackDepth(num)
					boundExpression2 = DirectCast(Me.Visit(node.WhenNullOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
					Me.EnsureStackState(RuntimeHelpers.GetObjectValue(objectValue))
				End If
				Return node.Update(boundExpression, node.CaptureReceiver, node.PlaceholderId, boundExpression1, boundExpression2, node.Type)
			End Function

			Public Overrides Function VisitObjectCreationExpression(ByVal node As BoundObjectCreationExpression) As BoundNode
				Dim constructorOpt As MethodSymbol = node.ConstructorOpt
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = If(constructorOpt Is Nothing, node.Arguments, Me.VisitArguments(node.Arguments, constructorOpt.Parameters))
				Me._counter = Me._counter + 1
				Return node.Update(constructorOpt, boundExpressions, node.DefaultArguments, Nothing, node.Type)
			End Function

			Public Overrides Function VisitReferenceAssignment(ByVal node As BoundReferenceAssignment) As BoundNode
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = DirectCast(Me.VisitExpression(node.ByRefLocal, StackScheduler.ExprContext.AssignmentTarget), Microsoft.CodeAnalysis.VisualBasic.BoundLocal)
				Dim boundLocal1 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Me._assignmentLocal
				Me._assignmentLocal = Nothing
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(node.LValue, StackScheduler.ExprContext.Address)
				Me.RecordVarWrite(boundLocal1.LocalSymbol)
				Return node.Update(boundLocal, boundExpression, node.IsLValue, node.Type)
			End Function

			Public Overrides Function VisitReturnStatement(ByVal node As BoundReturnStatement) As BoundNode
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = TryCast(Me.Visit(node.ExpressionOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Me.EnsureOnlyEvalStack()
				Return node.Update(boundExpression, node.FunctionLocalOpt, node.ExitLabelOpt)
			End Function

			Public Overrides Function VisitSelectStatement(ByVal node As BoundSelectStatement) As BoundNode
				Me.EnsureOnlyEvalStack()
				Dim boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = DirectCast(Me.Visit(node.ExpressionStatement), Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement)
				If (boundExpressionStatement.Expression.Kind = BoundKind.Local) Then
					Me.ShouldNotSchedule(DirectCast(boundExpressionStatement.Expression, BoundLocal).LocalSymbol)
				End If
				Dim num As Integer = Me.StackDepth()
				Me.EnsureOnlyEvalStack()
				Dim boundRValuePlaceholder As Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder = DirectCast(Me.Visit(node.ExprPlaceholderOpt), Microsoft.CodeAnalysis.VisualBasic.BoundRValuePlaceholder)
				Me.SetStackDepth(num)
				Me.EnsureOnlyEvalStack()
				Dim boundCaseBlocks As ImmutableArray(Of BoundCaseBlock) = Me.VisitList(Of BoundCaseBlock)(node.CaseBlocks)
				Dim exitLabel As LabelSymbol = node.ExitLabel
				If (exitLabel IsNot Nothing) Then
					Me.RecordLabel(exitLabel)
				End If
				Me.EnsureOnlyEvalStack()
				Return node.Update(boundExpressionStatement, boundRValuePlaceholder, boundCaseBlocks, node.RecommendSwitchTable, exitLabel)
			End Function

			Public Overrides Function VisitSequence(ByVal node As BoundSequence) As BoundNode
				Dim num As Integer = Me.StackDepth()
				Dim locals As ImmutableArray(Of LocalSymbol) = node.Locals
				If (Not locals.IsEmpty) Then
					If (Me._context <> StackScheduler.ExprContext.Sideeffects) Then
						Me.DeclareLocals(locals, num)
					Else
						Me.DeclareLocals(locals, num)
					End If
				End If
				Dim exprContext As StackScheduler.ExprContext = Me._context
				Dim sideEffects As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = node.SideEffects
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Nothing
				If (Not sideEffects.IsDefault) Then
					Dim length As Integer = sideEffects.Length - 1
					For i As Integer = 0 To length
						Dim item As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = sideEffects(i)
						Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(item, StackScheduler.ExprContext.Sideeffects)
						If (instance Is Nothing AndAlso boundExpression <> item) Then
							instance = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).GetInstance()
							instance.AddRange(sideEffects, i)
						End If
						If (instance IsNot Nothing) Then
							instance.Add(boundExpression)
						End If
					Next

				End If
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.VisitExpression(node.ValueOpt, exprContext)
				Return node.Update(node.Locals, If(instance IsNot Nothing, instance.ToImmutableAndFree(), sideEffects), boundExpression1, node.Type)
			End Function

			Public Overrides Function VisitSpillSequence(ByVal node As BoundSpillSequence) As BoundNode
				Throw ExceptionUtilities.Unreachable
			End Function

			Private Function VisitStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim num As Integer = Me.StackDepth()
				Dim exprContext As StackScheduler.ExprContext = Me._context
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = MyBase.Visit(node)
				If (Me._debugFriendly) Then
					Me.EnsureOnlyEvalStack()
				End If
				Me._context = exprContext
				Me.SetStackDepth(num)
				Me._counter = Me._counter + 1
				Return boundNode
			End Function

			Public Overrides Function VisitTernaryConditionalExpression(ByVal node As BoundTernaryConditionalExpression) As BoundNode
				Dim num As Integer = Me.StackDepth()
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Condition), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Dim objectValue As Object = RuntimeHelpers.GetObjectValue(Me.GetStackStateCookie())
				Me.SetStackDepth(num)
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.WhenTrue), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Me.EnsureStackState(RuntimeHelpers.GetObjectValue(objectValue))
				Me.SetStackDepth(num)
				Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.WhenFalse), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Me.EnsureStackState(RuntimeHelpers.GetObjectValue(objectValue))
				Return node.Update(boundExpression, boundExpression1, boundExpression2, node.ConstantValueOpt, node.Type)
			End Function

			Public Overrides Function VisitTryStatement(ByVal node As BoundTryStatement) As BoundNode
				Me.EnsureOnlyEvalStack()
				Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.Visit(node.TryBlock), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
				Me.EnsureOnlyEvalStack()
				Dim boundCatchBlocks As ImmutableArray(Of BoundCatchBlock) = Me.VisitList(Of BoundCatchBlock)(node.CatchBlocks)
				Me.EnsureOnlyEvalStack()
				Dim boundBlock1 As Microsoft.CodeAnalysis.VisualBasic.BoundBlock = DirectCast(Me.Visit(node.FinallyBlockOpt), Microsoft.CodeAnalysis.VisualBasic.BoundBlock)
				Me.EnsureOnlyEvalStack()
				If (node.ExitLabelOpt IsNot Nothing) Then
					Me.RecordLabel(node.ExitLabelOpt)
				End If
				Me.EnsureOnlyEvalStack()
				Return node.Update(boundBlock, boundCatchBlocks, boundBlock1, node.ExitLabelOpt)
			End Function

			Public Overrides Function VisitUnaryOperator(ByVal node As BoundUnaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				If (Not node.Checked OrElse (node.OperatorKind And UnaryOperatorKind.OpMask) <> UnaryOperatorKind.Minus) Then
					boundNode = MyBase.VisitUnaryOperator(node)
				Else
					Dim num As Integer = Me.StackDepth()
					Me.PushEvalStack(Nothing, StackScheduler.ExprContext.None)
					Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Operand), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
					Me.SetStackDepth(num)
					boundNode = node.Update(node.OperatorKind, boundExpression, node.Checked, node.ConstantValueOpt, node.Type)
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitUnstructuredExceptionOnErrorSwitch(ByVal node As BoundUnstructuredExceptionOnErrorSwitch) As BoundNode
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Value), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Me.PopEvalStack()
				Return node.Update(boundExpression, Me.VisitList(Of BoundGotoStatement)(node.Jumps))
			End Function

			Public Overrides Function VisitUnstructuredExceptionResumeSwitch(ByVal node As BoundUnstructuredExceptionResumeSwitch) As BoundNode
				Dim boundLabelStatement As Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement = DirectCast(Me.Visit(node.ResumeLabel), Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement)
				Dim boundLabelStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement = DirectCast(Me.Visit(node.ResumeNextLabel), Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement)
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = DirectCast(Me.Visit(node.ResumeTargetTemporary), Microsoft.CodeAnalysis.VisualBasic.BoundLocal)
				Me.PopEvalStack()
				Return node.Update(boundLocal, boundLabelStatement, boundLabelStatement1, node.Jumps)
			End Function
		End Class

		Private Class DummyLocal
			Inherits SynthesizedLocal
			Public Sub New(ByVal container As Symbol)
				MyBase.New(container, Nothing, SynthesizedLocalKind.OptimizerTemp, Nothing, False)
			End Sub

			Friend Overrides Function ComputeType(Optional ByVal containingBinder As Binder = Nothing) As TypeSymbol
				Throw ExceptionUtilities.Unreachable
			End Function
		End Class

		Private Enum ExprContext
			None
			Sideeffects
			Value
			Address
			AssignmentTarget
			Box
		End Enum

		Private Class LocalDefUseInfo
			Public ReadOnly StackAtDeclaration As Integer

			Public ReadOnly localDefs As List(Of StackScheduler.LocalDefUseSpan)

			Private _cannotSchedule As Boolean

			Public ReadOnly Property CannotSchedule As Boolean
				Get
					Return Me._cannotSchedule
				End Get
			End Property

			Public Sub New(ByVal stackAtDeclaration As Integer)
				MyBase.New()
				Me.localDefs = New List(Of StackScheduler.LocalDefUseSpan)(8)
				Me._cannotSchedule = False
				Me.StackAtDeclaration = stackAtDeclaration
			End Sub

			Public Sub ShouldNotSchedule()
				Me._cannotSchedule = True
			End Sub
		End Class

		Private Class LocalDefUseSpan
			Public ReadOnly Start As Integer

			Private _end As Integer

			Public ReadOnly Property [End] As Integer
				Get
					Return Me._end
				End Get
			End Property

			Public Sub New(ByVal assigned As Integer)
				MyBase.New()
				Me.Start = assigned
				Me._end = assigned
			End Sub

			Public Function ConflictsWith(ByVal other As StackScheduler.LocalDefUseSpan) As Boolean
				Return Me.Contains(other.Start) Xor Me.Contains(other.[End])
			End Function

			Public Function ConflictsWithDummy(ByVal dummy As StackScheduler.LocalDefUseSpan) As Boolean
				Return Me.Includes(dummy.Start) Xor Me.Includes(dummy.[End])
			End Function

			Private Function Contains(ByVal val As Integer) As Boolean
				If (Me.Start >= val) Then
					Return False
				End If
				Return Me.[End] > val
			End Function

			Private Function Includes(ByVal val As Integer) As Boolean
				If (Me.Start > val) Then
					Return False
				End If
				Return Me.[End] >= val
			End Function

			Public Sub SetEnd(ByVal newEnd As Integer)
				Me._end = newEnd
			End Sub

			Public Overrides Function ToString() As String
				Dim str() As [String] = { "[", Me.Start.ToString(), ", ", Nothing, Nothing }
				str(3) = Me.[End].ToString()
				str(4) = ")"
				Return [String].Concat(str)
			End Function
		End Class

		Private NotInheritable Class Rewriter
			Inherits BoundTreeRewriterWithStackGuard
			Private _nodeCounter As Integer

			Private ReadOnly _info As Dictionary(Of LocalSymbol, StackScheduler.LocalDefUseInfo)

			Private Sub New(ByVal info As Dictionary(Of LocalSymbol, StackScheduler.LocalDefUseInfo))
				MyBase.New()
				Me._nodeCounter = 0
				Me._info = Nothing
				Me._info = info
			End Sub

			Private Shared Function IsLastAccess(ByVal locInfo As StackScheduler.LocalDefUseInfo, ByVal counter As Integer) As Boolean
				Return locInfo.localDefs.Any(Function(d As StackScheduler.LocalDefUseSpan)
					If (counter <> d.Start) Then
						Return False
					End If
					Return counter = d.[End]
				End Function)
			End Function

			Public Shared Function Rewrite(ByVal src As BoundStatement, ByVal info As Dictionary(Of LocalSymbol, StackScheduler.LocalDefUseInfo)) As BoundStatement
				Return DirectCast((New StackScheduler.Rewriter(info)).Visit(src), BoundStatement)
			End Function

			Public Overrides Function Visit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Nothing
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				boundNode = If(boundExpression Is Nothing OrElse boundExpression.ConstantValueOpt Is Nothing, MyBase.Visit(node), node)
				Me._nodeCounter = Me._nodeCounter + 1
				Return boundNode
			End Function

			Public Overrides Function VisitAssignmentOperator(ByVal node As BoundAssignmentOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim localDefUseInfo As StackScheduler.LocalDefUseInfo = Nothing
				Dim left As BoundLocal = TryCast(node.Left, BoundLocal)
				If (left Is Nothing OrElse Not Me._info.TryGetValue(left.LocalSymbol, localDefUseInfo)) Then
					boundNode = Me.VisitAssignmentOperatorDefault(node)
				ElseIf (Not left.LocalSymbol.IsByRef) Then
					Me._nodeCounter = Me._nodeCounter + 1
					Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Right), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
					If (Not StackScheduler.Rewriter.IsLastAccess(localDefUseInfo, Me._nodeCounter)) Then
						boundNode = node.Update(left, node.LeftOnTheRightOpt, boundExpression, node.SuppressObjectClone, node.Type)
					Else
						boundNode = boundExpression
					End If
				Else
					boundNode = Me.VisitAssignmentOperatorDefault(node)
				End If
				Return boundNode
			End Function

			Private Function VisitAssignmentOperatorDefault(ByVal node As BoundAssignmentOperator) As BoundNode
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Left), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.Right), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Return node.Update(boundExpression, Nothing, boundExpression1, node.SuppressObjectClone, node.Type)
			End Function

			Public Overrides Function VisitBinaryOperator(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.Left
				If (left.Kind <> BoundKind.BinaryOperator OrElse left.ConstantValueOpt IsNot Nothing) Then
					boundNode = Me.VisitBinaryOperatorSimple(node)
				Else
					Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator).GetInstance()
					instance.Push(node)
					Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = DirectCast(left, Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
					While True
						instance.Push(boundBinaryOperator)
						left = boundBinaryOperator.Left
						If (left.Kind <> BoundKind.BinaryOperator OrElse left.ConstantValueOpt IsNot Nothing) Then
							Exit While
						End If
						boundBinaryOperator = DirectCast(left, Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
					End While
					Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(left), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
					While True
						boundBinaryOperator = instance.Pop()
						Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(boundBinaryOperator.Right), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
						Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(boundBinaryOperator.Type)
						boundExpression = boundBinaryOperator.Update(boundBinaryOperator.OperatorKind, boundExpression, boundExpression1, boundBinaryOperator.Checked, boundBinaryOperator.ConstantValueOpt, typeSymbol)
						If (instance.Count = 0) Then
							Exit While
						End If
						Me._nodeCounter = Me._nodeCounter + 1
					End While
					instance.Free()
					boundNode = boundExpression
				End If
				Return boundNode
			End Function

			Private Function VisitBinaryOperatorSimple(ByVal node As BoundBinaryOperator) As BoundNode
				Return MyBase.VisitBinaryOperator(node)
			End Function

			Public Overrides Function VisitLocal(ByVal node As BoundLocal) As BoundNode
				Dim boundDup As BoundNode
				Dim localDefUseInfo As StackScheduler.LocalDefUseInfo = Nothing
				If (Not Me._info.TryGetValue(node.LocalSymbol, localDefUseInfo)) Then
					boundDup = MyBase.VisitLocal(node)
				ElseIf (StackScheduler.Rewriter.IsLastAccess(localDefUseInfo, Me._nodeCounter)) Then
					boundDup = MyBase.VisitLocal(node)
				Else
					boundDup = New Microsoft.CodeAnalysis.VisualBasic.BoundDup(node.Syntax, node.LocalSymbol.IsByRef, node.Type)
				End If
				Return boundDup
			End Function

			Public Overrides Function VisitLoweredConditionalAccess(ByVal node As BoundLoweredConditionalAccess) As BoundNode
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ReceiverOrCondition), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.WhenNotNull), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Dim whenNullOpt As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.WhenNullOpt
				If (whenNullOpt IsNot Nothing) Then
					whenNullOpt = DirectCast(Me.Visit(whenNullOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				End If
				Return node.Update(boundExpression, node.CaptureReceiver, node.PlaceholderId, boundExpression1, whenNullOpt, node.Type)
			End Function

			Public Overrides Function VisitReferenceAssignment(ByVal node As BoundReferenceAssignment) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim localDefUseInfo As StackScheduler.LocalDefUseInfo = Nothing
				Dim byRefLocal As BoundLocal = node.ByRefLocal
				If (Me._info.TryGetValue(byRefLocal.LocalSymbol, localDefUseInfo)) Then
					Me._nodeCounter = Me._nodeCounter + 1
					Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.LValue), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
					If (Not StackScheduler.Rewriter.IsLastAccess(localDefUseInfo, Me._nodeCounter)) Then
						boundNode = node.Update(byRefLocal, boundExpression, node.IsLValue, node.Type)
					Else
						boundNode = boundExpression
					End If
				Else
					boundNode = MyBase.VisitReferenceAssignment(node)
				End If
				Return boundNode
			End Function
		End Class
	End Class
End Namespace