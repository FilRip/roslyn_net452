Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class AbstractFlowPass(Of LocalState As AbstractFlowPass(Of LocalState).AbstractLocalState)
		Inherits BoundTreeVisitor
		Protected ReadOnly _firstInRegion As BoundNode

		Protected ReadOnly _lastInRegion As BoundNode

		Protected ReadOnly _region As TextSpan

		Protected _regionPlace As AbstractFlowPass(Of LocalState).RegionPlace

		Private ReadOnly _loopHeadState As Dictionary(Of BoundLoopStatement, LocalState)

		Private _invalidRegion As Boolean

		Protected ReadOnly compilation As VisualBasicCompilation

		Public symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol

		Private ReadOnly _methodOrInitializerMainNode As BoundNode

		Private ReadOnly _labels As Dictionary(Of LabelSymbol, AbstractFlowPass(Of LocalState).LabelStateAndNesting)

		Private _labelsSeen As HashSet(Of LabelSymbol)

		Private _placeholderReplacementMap As Dictionary(Of BoundValuePlaceholderBase, BoundExpression)

		Friend backwardBranchChanged As Boolean

		Private _pendingBranches As ArrayBuilder(Of AbstractFlowPass(Of LocalState).PendingBranch)

		Protected State As LocalState

		Protected StateWhenTrue As LocalState

		Protected StateWhenFalse As LocalState

		Protected IsConditionalState As Boolean

		Protected ReadOnly MeParameter As ParameterSymbol

		Protected ReadOnly TrackUnassignments As Boolean

		Private _nesting As ArrayBuilder(Of Integer)

		Protected ReadOnly diagnostics As DiagnosticBag

		Private ReadOnly _suppressConstantExpressions As Boolean

		Protected _recursionDepth As Integer

		Protected ReadOnly Property GetPlaceholderSubstitute(ByVal placeholder As BoundValuePlaceholderBase) As BoundExpression
			Get
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				If (Me._placeholderReplacementMap Is Nothing OrElse Not Me._placeholderReplacementMap.TryGetValue(placeholder, boundExpression1)) Then
					boundExpression = Nothing
				Else
					boundExpression = boundExpression1
				End If
				Return boundExpression
			End Get
		End Property

		Protected ReadOnly Property InvalidRegionDetected As Boolean
			Get
				Return Me._invalidRegion
			End Get
		End Property

		Protected ReadOnly Property IsInside As Boolean
			Get
				Return Me._regionPlace = AbstractFlowPass(Of LocalState).RegionPlace.Inside
			End Get
		End Property

		Protected ReadOnly Property MethodParameters As ImmutableArray(Of ParameterSymbol)
			Get
				If (Me.symbol.Kind <> SymbolKind.Method) Then
					Return ImmutableArray(Of ParameterSymbol).Empty
				End If
				Return DirectCast(Me.symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).Parameters
			End Get
		End Property

		Protected ReadOnly Property MethodReturnType As TypeSymbol
			Get
				If (Me.symbol.Kind <> SymbolKind.Method) Then
					Return Nothing
				End If
				Return DirectCast(Me.symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).ReturnType
			End Get
		End Property

		Protected ReadOnly Property MethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Get
				If (Me.symbol.Kind <> SymbolKind.Method) Then
					Return Nothing
				End If
				Return DirectCast(Me.symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			End Get
		End Property

		Protected ReadOnly Property PendingBranches As ImmutableArray(Of AbstractFlowPass(Of LocalState).PendingBranch)
			Get
				Return Me._pendingBranches.ToImmutable()
			End Get
		End Property

		Protected ReadOnly Property ShouldAnalyzeByRefParameters As Boolean
			Get
				If (Me.symbol.Kind <> SymbolKind.Method) Then
					Return False
				End If
				Return DirectCast(Me.symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).Locations.Length = 1
			End Get
		End Property

		Protected Overridable ReadOnly Property SuppressRedimOperandRvalueOnPreserve As Boolean
			Get
				Return False
			End Get
		End Property

		Protected Sub New(ByVal _info As FlowAnalysisInfo, ByVal suppressConstExpressionsSupport As Boolean)
			MyClass.New(_info, New FlowAnalysisRegionInfo(), suppressConstExpressionsSupport, False)
		End Sub

		Protected Sub New(ByVal _info As FlowAnalysisInfo, ByVal _region As FlowAnalysisRegionInfo, ByVal suppressConstExpressionsSupport As Boolean, ByVal trackUnassignments As Boolean)
			MyBase.New()
			Dim boundLoopStatements As Dictionary(Of BoundLoopStatement, LocalState)
			Me._invalidRegion = False
			Me._labels = New Dictionary(Of LabelSymbol, AbstractFlowPass(Of LocalState).LabelStateAndNesting)()
			Me._labelsSeen = New HashSet(Of LabelSymbol)()
			Me.backwardBranchChanged = False
			Me._pendingBranches = ArrayBuilder(Of AbstractFlowPass(Of LocalState).PendingBranch).GetInstance()
			Me.diagnostics = DiagnosticBag.GetInstance()
			Me.compilation = _info.Compilation
			Me.symbol = _info.Symbol
			Me.MeParameter = Me.symbol.GetMeParameter()
			Me._methodOrInitializerMainNode = _info.Node
			Me._firstInRegion = _region.FirstInRegion
			Me._lastInRegion = _region.LastInRegion
			Me._region = _region.Region
			Me.TrackUnassignments = trackUnassignments
			If (trackUnassignments) Then
				boundLoopStatements = New Dictionary(Of BoundLoopStatement, LocalState)()
			Else
				boundLoopStatements = Nothing
			End If
			Me._loopHeadState = boundLoopStatements
			Me._suppressConstantExpressions = suppressConstExpressionsSupport
		End Sub

		Private Sub AdjustConditionalState(ByVal node As BoundExpression)
			If (Me.IsConstantTrue(node)) Then
				Me.Unsplit()
				Me.SetConditionalState(Me.State, Me.UnreachableState())
				Return
			End If
			If (Not Me.IsConstantFalse(node)) Then
				Me.Split()
				Return
			End If
			Me.Unsplit()
			Me.SetConditionalState(Me.UnreachableState(), Me.State)
		End Sub

		Protected Overridable Function AllBitsSet() As LocalState
			Return Nothing
		End Function

		Protected Overridable Function Analyze() As Boolean
			Dim flag As Boolean
			While True
				If (Not Me.Scan()) Then
					flag = False
					Exit While
				ElseIf (Not Me.backwardBranchChanged) Then
					flag = True
					Exit While
				End If
			End While
			Return flag
		End Function

		Private Function BothBranchAndLabelArePrefixedByNesting(ByVal branch As AbstractFlowPass(Of LocalState).PendingBranch, Optional ByVal labelsFilter As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol) = Nothing, Optional ByVal ignoreLast As Boolean = False, <Out> Optional ByRef labelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol = Nothing, <Out> Optional ByRef labelAndNesting As AbstractFlowPass(Of LocalState).LabelStateAndNesting = Nothing) As Boolean
			Dim flag As Boolean
			Dim flag1 As Boolean
			Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = branch.Branch
			If (boundStatement IsNot Nothing AndAlso branch.Nesting.IsPrefixedBy(Me._nesting, ignoreLast)) Then
				labelSymbol = AbstractFlowPass(Of LocalState).GetBranchTargetLabel(boundStatement, True)
				If (labelSymbol Is Nothing OrElse labelsFilter IsNot Nothing AndAlso Not labelsFilter.Contains(labelSymbol)) Then
					flag = False
					Return flag
				End If
				If (Not Me._labels.TryGetValue(labelSymbol, labelAndNesting)) Then
					flag1 = False
				Else
					Dim nesting As AbstractFlowPass(Of LocalState).BlockNesting = labelAndNesting.Nesting
					flag1 = nesting.IsPrefixedBy(Me._nesting, ignoreLast)
				End If
				flag = flag1
				Return flag
			End If
			flag = False
			Return flag
		End Function

		Protected Overrides Function ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException() As Boolean
			Return False
		End Function

		Public Overrides Function DefaultVisit(ByVal node As BoundNode) As BoundNode
			Return Me.Unimplemented(node, "flow analysis")
		End Function

		Protected MustOverride Function Dump(ByVal state As LocalState) As String

		Protected Overridable Sub EnterRegion()
			Me._regionPlace = AbstractFlowPass(Of LocalState).RegionPlace.Inside
		End Sub

		Protected Shared Function FieldAccessMayRequireTracking(ByVal fieldAccess As BoundFieldAccess) As Boolean
			Dim flag As Boolean
			If (Not fieldAccess.FieldSymbol.IsShared) Then
				Dim receiverOpt As BoundExpression = fieldAccess.ReceiverOpt
				flag = If(receiverOpt IsNot Nothing, AbstractFlowPass(Of LocalState).IsNonPrimitiveValueType(receiverOpt.Type), False)
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Sub FinalizeBlock(ByVal level As Integer)
			Me._nesting.RemoveAt(level)
		End Sub

		Protected Overridable Sub Free()
			If (Me._nesting IsNot Nothing) Then
				Me._nesting.Free()
			End If
			Me.diagnostics.Free()
			Me._pendingBranches.Free()
		End Sub

		Private Shared Function GetBranchTargetLabel(ByVal branch As BoundStatement, ByVal gotoOnly As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol
			Dim label As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol
			Dim exitLabelOpt As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol
			Dim labelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol
			Dim kind As BoundKind = branch.Kind
			If (kind <= BoundKind.ExitStatement) Then
				If (kind = BoundKind.ReturnStatement) Then
					If (gotoOnly) Then
						exitLabelOpt = Nothing
					Else
						exitLabelOpt = DirectCast(branch, BoundReturnStatement).ExitLabelOpt
					End If
					label = exitLabelOpt
				ElseIf (kind = BoundKind.YieldStatement) Then
					label = Nothing
				Else
					If (kind <> BoundKind.ExitStatement) Then
						Throw ExceptionUtilities.UnexpectedValue(branch.Kind)
					End If
					If (gotoOnly) Then
						labelSymbol = Nothing
					Else
						labelSymbol = DirectCast(branch, BoundExitStatement).Label
					End If
					label = labelSymbol
				End If
			ElseIf (kind = BoundKind.ContinueStatement) Then
				label = Nothing
			ElseIf (kind = BoundKind.GotoStatement) Then
				label = DirectCast(branch, BoundGotoStatement).Label
			Else
				If (kind <> BoundKind.ConditionalGoto) Then
					Throw ExceptionUtilities.UnexpectedValue(branch.Kind)
				End If
				label = DirectCast(branch, BoundConditionalGoto).Label
			End If
			Return label
		End Function

		Protected Overridable Sub InitForScan()
		End Sub

		Private Sub InitializeBlockStatement(ByVal level As Integer, ByRef index As Integer)
			Me._nesting(level) = index
			index = index + 1
		End Sub

		Protected MustOverride Function IntersectWith(ByRef self As LocalState, ByRef other As LocalState) As Boolean

		Private Function IntroduceBlock() As Integer
			Dim count As Integer = Me._nesting.Count
			Me._nesting.Add(0)
			Return count
		End Function

		Private Function IsConstantFalse(ByVal node As BoundExpression) As Boolean
			Dim flag As Boolean
			If (Me._suppressConstantExpressions) Then
				flag = False
			ElseIf (node.IsConstant) Then
				Dim constantValueOpt As ConstantValue = node.ConstantValueOpt
				flag = If(constantValueOpt.Discriminator = ConstantValueTypeDiscriminator.[Boolean], Not constantValueOpt.BooleanValue, False)
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function IsConstantNull(ByVal node As BoundExpression) As Boolean
			Dim flag As Boolean
			If (Not Me._suppressConstantExpressions) Then
				flag = If(node.IsConstant, node.ConstantValueOpt.IsNull, False)
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function IsConstantTrue(ByVal node As BoundExpression) As Boolean
			Dim flag As Boolean
			If (Me._suppressConstantExpressions) Then
				flag = False
			ElseIf (node.IsConstant) Then
				Dim constantValueOpt As ConstantValue = node.ConstantValueOpt
				flag = If(constantValueOpt.Discriminator = ConstantValueTypeDiscriminator.[Boolean], constantValueOpt.BooleanValue, False)
			Else
				flag = False
			End If
			Return flag
		End Function

		Protected Function IsInsideRegion(ByVal span As TextSpan) As Boolean
			Dim flag As Boolean
			flag = If(span.Length <> 0, Me._region.Contains(span), Me._region.Contains(span.Start))
			Return flag
		End Function

		Protected Shared Function IsNonPrimitiveValueType(ByVal type As TypeSymbol) As Boolean
			Dim flag As Boolean
			If (type.IsValueType) Then
				Dim specialType As Microsoft.CodeAnalysis.SpecialType = type.SpecialType
				flag = If(specialType = Microsoft.CodeAnalysis.SpecialType.None OrElse CSByte(specialType) - CSByte(Microsoft.CodeAnalysis.SpecialType.System_IntPtr) <= CSByte(Microsoft.CodeAnalysis.SpecialType.System_Object) OrElse specialType = Microsoft.CodeAnalysis.SpecialType.System_Nullable_T, True, False)
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function LabelState(ByVal label As LabelSymbol) As LocalState
			Dim localState As LocalState
			Dim labelStateAndNesting As AbstractFlowPass(Of LocalState).LabelStateAndNesting = New AbstractFlowPass(Of LocalState).LabelStateAndNesting()
			localState = If(Not Me._labels.TryGetValue(label, labelStateAndNesting), Me.UnreachableState(), labelStateAndNesting.State)
			Return localState
		End Function

		Protected Overridable Sub LeaveRegion()
			Me._regionPlace = AbstractFlowPass(Of LocalState).RegionPlace.After
		End Sub

		Private Sub LoopHead(ByVal node As BoundLoopStatement)
			Dim localState As LocalState
			If (Me.TrackUnassignments) Then
				If (Me._loopHeadState.TryGetValue(node, localState)) Then
					Me.IntersectWith(Me.State, localState)
				End If
				Me._loopHeadState(node) = Me.State.Clone()
			End If
		End Sub

		Private Sub LoopTail(ByVal node As BoundLoopStatement)
			If (Me.TrackUnassignments) Then
				Dim item As LocalState = Me._loopHeadState(node)
				If (Me.IntersectWith(item, Me.State)) Then
					Me._loopHeadState(node) = item
					Me.backwardBranchChanged = True
				End If
			End If
		End Sub

		Protected Overridable Sub NoteBranch(ByVal pending As AbstractFlowPass(Of LocalState).PendingBranch, ByVal stmt As BoundStatement, ByVal labelStmt As BoundLabelStatement)
		End Sub

		Protected MustOverride Function ReachableState() As LocalState

		Protected Sub RemovePlaceholderSubstitute(ByVal placeholder As BoundValuePlaceholderBase)
			Me._placeholderReplacementMap.Remove(placeholder)
		End Sub

		Private Sub ResetPendingBranches(ByVal newPendingBranches As ArrayBuilder(Of AbstractFlowPass(Of LocalState).PendingBranch))
			Me._pendingBranches.Free()
			Me._pendingBranches = newPendingBranches
		End Sub

		Protected Overridable Sub ResolveBranch(ByVal pending As AbstractFlowPass(Of LocalState).PendingBranch, ByVal label As LabelSymbol, ByVal target As BoundLabelStatement, ByRef labelStateChanged As Boolean)
			Dim localState As LocalState = Me.LabelState(target.Label)
			Me.NoteBranch(pending, pending.Branch, target)
			If (Me.IntersectWith(localState, pending.State)) Then
				labelStateChanged = True
				Me._labels(target.Label) = New AbstractFlowPass(Of LocalState).LabelStateAndNesting(target, localState, Me._nesting)
			End If
		End Sub

		Private Function ResolveBranches(ByVal target As BoundLabelStatement) As Boolean
			Dim flag As Boolean = False
			If (Me.PendingBranches.Length > 0) Then
				Dim instance As ArrayBuilder(Of AbstractFlowPass(Of LocalState).PendingBranch) = ArrayBuilder(Of AbstractFlowPass(Of LocalState).PendingBranch).GetInstance()
				Dim enumerator As ImmutableArray(Of AbstractFlowPass(Of LocalState).PendingBranch).Enumerator = Me.PendingBranches.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As AbstractFlowPass(Of LocalState).PendingBranch = enumerator.Current
					Dim branchTargetLabel As LabelSymbol = AbstractFlowPass(Of LocalState).GetBranchTargetLabel(current.Branch, False)
					If (branchTargetLabel Is Nothing OrElse Not (branchTargetLabel = target.Label)) Then
						instance.Add(current)
					Else
						Me.ResolveBranch(current, branchTargetLabel, target, flag)
					End If
				End While
				Me.ResetPendingBranches(instance)
			End If
			Return flag
		End Function

		Private Function ResolveBranches(ByVal labelsFilter As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol)) As Boolean
			Dim flag As Boolean = False
			Dim flag1 As Boolean = False
			If (Me.PendingBranches.Length > 0) Then
				Dim instance As ArrayBuilder(Of AbstractFlowPass(Of LocalState).PendingBranch) = ArrayBuilder(Of AbstractFlowPass(Of LocalState).PendingBranch).GetInstance()
				Dim enumerator As ImmutableArray(Of AbstractFlowPass(Of LocalState).PendingBranch).Enumerator = Me.PendingBranches.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As AbstractFlowPass(Of LocalState).PendingBranch = enumerator.Current
					Dim labelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol = Nothing
					Dim labelStateAndNesting As AbstractFlowPass(Of LocalState).LabelStateAndNesting = New AbstractFlowPass(Of LocalState).LabelStateAndNesting()
					If (Not Me.BothBranchAndLabelArePrefixedByNesting(current, labelsFilter, False, labelSymbol, labelStateAndNesting)) Then
						instance.Add(current)
					Else
						Me.ResolveBranch(current, labelSymbol, labelStateAndNesting.Target, flag)
						If (Not flag) Then
							Continue While
						End If
						flag1 = True
					End If
				End While
				Me.ResetPendingBranches(instance)
			End If
			Return flag1
		End Function

		Private Sub ResolveBreaks(ByVal breakState As LocalState, ByVal breakLabel As LabelSymbol)
			Dim instance As ArrayBuilder(Of AbstractFlowPass(Of LocalState).PendingBranch) = ArrayBuilder(Of AbstractFlowPass(Of LocalState).PendingBranch).GetInstance()
			Dim enumerator As ImmutableArray(Of AbstractFlowPass(Of LocalState).PendingBranch).Enumerator = Me.PendingBranches.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As AbstractFlowPass(Of LocalState).PendingBranch = enumerator.Current
				If (current.Branch.Kind <> BoundKind.ExitStatement) Then
					instance.Add(current)
				ElseIf (TryCast(current.Branch, BoundExitStatement).Label <> breakLabel) Then
					instance.Add(current)
				Else
					Me.IntersectWith(breakState, current.State)
				End If
			End While
			Me.ResetPendingBranches(instance)
			Me.SetState(breakState)
		End Sub

		Private Sub ResolveContinues(ByVal continueLabel As LabelSymbol)
			Dim instance As ArrayBuilder(Of AbstractFlowPass(Of LocalState).PendingBranch) = ArrayBuilder(Of AbstractFlowPass(Of LocalState).PendingBranch).GetInstance()
			Dim enumerator As ImmutableArray(Of AbstractFlowPass(Of LocalState).PendingBranch).Enumerator = Me.PendingBranches.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As AbstractFlowPass(Of LocalState).PendingBranch = enumerator.Current
				If (current.Branch.Kind <> BoundKind.ContinueStatement) Then
					instance.Add(current)
				ElseIf (TryCast(current.Branch, BoundContinueStatement).Label <> continueLabel) Then
					instance.Add(current)
				Else
					Me.IntersectWith(Me.State, current.State)
				End If
			End While
			Me.ResetPendingBranches(instance)
		End Sub

		Protected Sub RestorePending(ByVal oldPending As AbstractFlowPass(Of LocalState).SavedPending, Optional ByVal mergeLabelsSeen As Boolean = False)
			If (Me.ResolveBranches(Me._labelsSeen)) Then
				Me.backwardBranchChanged = True
			End If
			oldPending.PendingBranches.AddRange(Me.PendingBranches)
			Me.ResetPendingBranches(oldPending.PendingBranches)
			If (Not mergeLabelsSeen) Then
				Me._labelsSeen = oldPending.LabelsSeen
				Return
			End If
			Me._labelsSeen.AddAll(oldPending.LabelsSeen)
		End Sub

		Protected Function SavePending() As AbstractFlowPass(Of LocalState).SavedPending
			' 
			' Current member / type: Microsoft.CodeAnalysis.VisualBasic.AbstractFlowPass`1/SavedPending<LocalState> Microsoft.CodeAnalysis.VisualBasic.AbstractFlowPass`1::SavePending()
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: Microsoft.CodeAnalysis.VisualBasic.AbstractFlowPass<LocalState>/SavedPending<LocalState> SavePending()
			' 
			' The unary opperator AddressReference is not supported in VisualBasic
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\DetermineNotSupportedVBCodeStep.cs:ligne 22
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Function

		Protected Overridable Function Scan() As Boolean
			Me.diagnostics.Clear()
			Me._regionPlace = AbstractFlowPass(Of LocalState).RegionPlace.Before
			Me.SetState(Me.ReachableState())
			Me.backwardBranchChanged = False
			If (Me._nesting IsNot Nothing) Then
				Me._nesting.Free()
			End If
			Me._nesting = ArrayBuilder(Of Integer).GetInstance()
			Me.InitForScan()
			Dim savedPending As AbstractFlowPass(Of LocalState).SavedPending = Me.SavePending()
			Me.Visit(Me._methodOrInitializerMainNode)
			Me.RestorePending(savedPending, False)
			Me._labelsSeen.Clear()
			If (Me._firstInRegion Is Nothing) Then
				Return True
			End If
			Return Me._regionPlace = AbstractFlowPass(Of LocalState).RegionPlace.After
		End Function

		Private Sub SetConditionalState(ByVal _whenTrue As LocalState, ByVal _whenFalse As LocalState)
			Me.State = Nothing
			Me.StateWhenTrue = _whenTrue
			Me.StateWhenFalse = _whenFalse
			Me.IsConditionalState = True
		End Sub

		Protected Sub SetInvalidRegion()
			Me._invalidRegion = True
		End Sub

		Protected Sub SetPlaceholderSubstitute(ByVal placeholder As BoundValuePlaceholderBase, ByVal newSubstitute As BoundExpression)
			If (Me._placeholderReplacementMap Is Nothing) Then
				Me._placeholderReplacementMap = New Dictionary(Of BoundValuePlaceholderBase, BoundExpression)()
			End If
			Me._placeholderReplacementMap(placeholder) = newSubstitute
		End Sub

		Protected Sub SetState(ByVal _state As LocalState)
			Me.State = _state
			If (Me.IsConditionalState) Then
				Me.StateWhenTrue = Nothing
				Me.StateWhenFalse = Nothing
				Me.IsConditionalState = False
			End If
		End Sub

		Protected Sub SetUnreachable()
			Me.SetState(Me.UnreachableState())
		End Sub

		Protected Sub Split()
			If (Not Me.IsConditionalState) Then
				Me.SetConditionalState(Me.State, Me.State.Clone())
			End If
		End Sub

		Protected Overridable Function Unimplemented(ByVal node As BoundNode, ByVal feature As String) As BoundNode
			Return Nothing
		End Function

		Protected MustOverride Sub UnionWith(ByRef self As LocalState, ByRef other As LocalState)

		Protected MustOverride Function UnreachableState() As LocalState

		Protected Sub Unsplit()
			If (Me.IsConditionalState) Then
				Me.IntersectWith(Me.StateWhenTrue, Me.StateWhenFalse)
				Me.SetState(Me.StateWhenTrue)
			End If
		End Sub

		Public NotOverridable Overrides Function Visit(ByVal node As BoundNode) As BoundNode
			Me.Visit(node, False)
			Return Nothing
		End Function

		Protected Overridable Sub Visit(ByVal node As BoundNode, ByVal dontLeaveRegion As Boolean)
			Me.VisitAlways(node, dontLeaveRegion)
		End Sub

		Public Overrides Function VisitAddHandlerStatement(ByVal node As BoundAddHandlerStatement) As BoundNode
			Return Me.VisitAddRemoveHandlerStatement(node)
		End Function

		Private Function VisitAddRemoveHandlerStatement(ByVal node As BoundAddRemoveHandlerStatement) As BoundNode
			Me.VisitRvalue(node.EventAccess, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Me.VisitRvalue(node.Handler, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitAddressOfOperator(ByVal node As BoundAddressOfOperator) As BoundNode
			Me.Visit(node.MethodGroup)
			Return Nothing
		End Function

		Protected Sub VisitAlways(ByVal node As BoundNode, Optional ByVal dontLeaveRegion As Boolean = False)
			If (Me._firstInRegion Is Nothing) Then
				Me.VisitWithStackGuard(node)
				Return
			End If
			If (node = Me._firstInRegion AndAlso Me._regionPlace = AbstractFlowPass(Of LocalState).RegionPlace.Before) Then
				Me.EnterRegion()
			End If
			Me.VisitWithStackGuard(node)
			If (Not dontLeaveRegion AndAlso node = Me._lastInRegion AndAlso Me.IsInside) Then
				Me.LeaveRegion()
			End If
		End Sub

		Public Overrides Function VisitAnonymousTypeCreationExpression(ByVal node As BoundAnonymousTypeCreationExpression) As BoundNode
			Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = node.Arguments.GetEnumerator()
			While enumerator.MoveNext()
				Me.VisitRvalue(enumerator.Current, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End While
			Return Nothing
		End Function

		Public Overrides Function VisitAnonymousTypeFieldInitializer(ByVal node As BoundAnonymousTypeFieldInitializer) As BoundNode
			Me.VisitRvalue(node.Value, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Protected Overridable Sub VisitArgument(ByVal arg As BoundExpression, ByVal p As ParameterSymbol)
			If (p.IsByRef) Then
				Me.VisitLvalue(arg, False)
				Return
			End If
			Me.VisitRvalue(arg, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
		End Sub

		Private Sub VisitArguments(ByVal arguments As ImmutableArray(Of BoundExpression), ByVal parameters As ImmutableArray(Of ParameterSymbol))
			If (parameters.IsDefault) Then
				Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = arguments.GetEnumerator()
				While enumerator.MoveNext()
					Me.VisitRvalue(enumerator.Current, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
				End While
				Return
			End If
			Dim num As Integer = Math.Min(parameters.Length, arguments.Length)
			Dim num1 As Integer = num - 1
			Dim num2 As Integer = 0
			Do
				Me.VisitArgument(arguments(num2), parameters(num2))
				num2 = num2 + 1
			Loop While num2 <= num1
			Dim num3 As Integer = num - 1
			For i As Integer = 0 To num3
				If (parameters(i).IsByRef) Then
					Me.WriteArgument(arguments(i), parameters(i).IsOut)
				End If
			Next

		End Sub

		Public Overrides Function VisitArrayAccess(ByVal node As BoundArrayAccess) As BoundNode
			Me.VisitRvalue(node.Expression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = node.Indices.GetEnumerator()
			While enumerator.MoveNext()
				Me.VisitRvalue(enumerator.Current, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End While
			Return Nothing
		End Function

		Public Overrides Function VisitArrayCreation(ByVal node As BoundArrayCreation) As BoundNode
			Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = node.Bounds.GetEnumerator()
			While enumerator.MoveNext()
				Me.VisitRvalue(enumerator.Current, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End While
			If (node.InitializerOpt IsNot Nothing AndAlso Not node.InitializerOpt.Initializers.IsDefault) Then
				Dim enumerator1 As ImmutableArray(Of BoundExpression).Enumerator = node.InitializerOpt.Initializers.GetEnumerator()
				While enumerator1.MoveNext()
					Me.VisitRvalue(enumerator1.Current, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
				End While
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitArrayInitialization(ByVal node As BoundArrayInitialization) As BoundNode
			Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = node.Initializers.GetEnumerator()
			While enumerator.MoveNext()
				Me.VisitRvalue(enumerator.Current, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End While
			Return Nothing
		End Function

		Public Overrides Function VisitArrayLength(ByVal node As BoundArrayLength) As BoundNode
			Me.VisitRvalue(node.Expression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitArrayLiteral(ByVal node As BoundArrayLiteral) As BoundNode
			Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = node.Bounds.GetEnumerator()
			While enumerator.MoveNext()
				Me.VisitRvalue(enumerator.Current, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End While
			Me.VisitRvalue(node.Initializer, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitAsNewLocalDeclarations(ByVal node As BoundAsNewLocalDeclarations) As BoundNode
			Me.VisitRvalue(node.Initializer, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Dim enumerator As ImmutableArray(Of BoundLocalDeclaration).Enumerator = node.LocalDeclarations.GetEnumerator()
			While enumerator.MoveNext()
				Me.Visit(enumerator.Current)
			End While
			Return Nothing
		End Function

		Public Overrides Function VisitAssignmentOperator(ByVal node As BoundAssignmentOperator) As BoundNode
			If (node.LeftOnTheRightOpt IsNot Nothing) Then
				Me.SetPlaceholderSubstitute(node.LeftOnTheRightOpt, node.Left)
			Else
				Me.VisitLvalue(node.Left, False)
			End If
			Me.VisitRvalue(node.Right, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			If (node.LeftOnTheRightOpt IsNot Nothing) Then
				Me.RemovePlaceholderSubstitute(node.LeftOnTheRightOpt)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitAwaitOperator(ByVal node As BoundAwaitOperator) As BoundNode
			Me.VisitRvalue(node.Operand, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitBadExpression(ByVal node As BoundBadExpression) As BoundNode
			Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = node.ChildBoundNodes.GetEnumerator()
			While enumerator.MoveNext()
				Me.VisitRvalue(enumerator.Current, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End While
			Return Nothing
		End Function

		Public Overrides Function VisitBadStatement(ByVal node As BoundBadStatement) As BoundNode
			Dim enumerator As ImmutableArray(Of BoundNode).Enumerator = node.ChildBoundNodes.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As BoundNode = enumerator.Current
				If (Not TypeOf current Is BoundStatement) Then
					Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = TryCast(current, Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
					If (boundExpression Is Nothing) Then
						Me.Visit(current)
					Else
						Me.VisitExpressionAsStatement(boundExpression)
					End If
				Else
					Me.VisitStatement(TryCast(current, BoundStatement))
				End If
			End While
			Return Nothing
		End Function

		Public Overrides Function VisitBadVariable(ByVal node As BoundBadVariable) As BoundNode
			If (Not node.IsLValue) Then
				Me.VisitRvalue(node.Expression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Else
				Me.VisitLvalue(node.Expression, False)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitBinaryConditionalExpression(ByVal node As BoundBinaryConditionalExpression) As BoundNode
			Me.VisitRvalue(node.TestExpression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			If (Not node.TestExpression.IsConstant OrElse Not node.TestExpression.ConstantValueOpt.IsNothing) Then
				Dim localState As LocalState = Me.State.Clone()
				Me.VisitRvalue(node.ElseExpression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
				Me.SetState(localState)
			Else
				Me.VisitRvalue(node.ElseExpression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			Return Nothing
		End Function

		Public NotOverridable Overrides Function VisitBinaryOperator(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator) As BoundNode
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator).GetInstance()
			Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator = node
			Dim left As BoundExpression = node.Left
			While left.Kind = BoundKind.BinaryOperator
				instance.Push(boundBinaryOperator)
				boundBinaryOperator = DirectCast(left, Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
				left = boundBinaryOperator.Left
			End While
			If (Me._regionPlace = AbstractFlowPass(Of LocalState).RegionPlace.Before AndAlso instance.Count > 0) Then
				Dim count As Integer = instance.Count - 1
				Dim num As Integer = 1
				While num <= count
					If (instance(num) <> Me._firstInRegion) Then
						num = num + 1
					Else
						Me.EnterRegion()
						GoTo Label0
					End If
				End While
				If (boundBinaryOperator = Me._firstInRegion) Then
					Me.EnterRegion()
				End If
			End If
		Label0:
			Dim operatorKind As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind = boundBinaryOperator.OperatorKind And Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.OpMask
			If (operatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[OrElse] OrElse operatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[AndAlso]) Then
				Me.VisitCondition(left)
			Else
				Me.VisitRvalue(left, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			While True
				Dim binaryOperatorKind As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind = boundBinaryOperator.OperatorKind And Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.OpMask
				If (binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[OrElse]) Then
					Dim stateWhenTrue As LocalState = Me.StateWhenTrue
					Me.SetState(Me.StateWhenFalse)
					Me.VisitCondition(boundBinaryOperator.Right)
					Dim localState As LocalState = Me.StateWhenTrue
					Me.IntersectWith(localState, stateWhenTrue)
					Me.SetConditionalState(localState, Me.StateWhenFalse)
				ElseIf (binaryOperatorKind <> Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[AndAlso]) Then
					Me.VisitRvalue(boundBinaryOperator.Right, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
				Else
					Dim stateWhenTrue1 As LocalState = Me.StateWhenTrue
					Dim stateWhenFalse As LocalState = Me.StateWhenFalse
					Me.SetState(stateWhenTrue1)
					Me.VisitCondition(boundBinaryOperator.Right)
					Dim localState1 As LocalState = Me.StateWhenTrue
					Dim localState2 As LocalState = stateWhenFalse
					Me.IntersectWith(localState2, Me.StateWhenFalse)
					Me.SetConditionalState(localState1, localState2)
				End If
				If (instance.Count <= 0) Then
					Exit While
				End If
				left = boundBinaryOperator
				boundBinaryOperator = instance.Pop()
				Dim operatorKind1 As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind = boundBinaryOperator.OperatorKind And Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.OpMask
				If (operatorKind1 = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[OrElse] OrElse operatorKind1 = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[AndAlso]) Then
					Me.AdjustConditionalState(left)
				Else
					Me.Unsplit()
				End If
				If (left = Me._lastInRegion AndAlso Me.IsInside) Then
					Me.LeaveRegion()
				End If
			End While
			instance.Free()
			Return Nothing
		End Function

		Public Overrides Function VisitBlock(ByVal node As BoundBlock) As BoundNode
			Dim num As Integer = Me.IntroduceBlock()
			Dim num1 As Integer = 0
			Dim enumerator As ImmutableArray(Of BoundStatement).Enumerator = node.Statements.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As BoundStatement = enumerator.Current
				Me.InitializeBlockStatement(num, num1)
				Me.VisitStatement(current)
			End While
			Me.FinalizeBlock(num)
			Return Nothing
		End Function

		Public Overrides Function VisitByRefArgumentPlaceholder(ByVal node As BoundByRefArgumentPlaceholder) As BoundNode
			Dim getPlaceholderSubstitute As BoundExpression = Me(node)
			If (getPlaceholderSubstitute IsNot Nothing) Then
				Me.VisitRvalue(getPlaceholderSubstitute, AbstractFlowPass(Of LocalState).ReadWriteContext.ByRefArgument, False)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitByRefArgumentWithCopyBack(ByVal node As BoundByRefArgumentWithCopyBack) As BoundNode
			Me.SetPlaceholderSubstitute(node.InPlaceholder, node.OriginalArgument)
			Me.VisitRvalue(node.InConversion, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Me.RemovePlaceholderSubstitute(node.InPlaceholder)
			Return Nothing
		End Function

		Public Overrides Function VisitCall(ByVal node As BoundCall) As BoundNode
			Dim receiverOpt As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim flag As Boolean = node.Method.CallsAreOmitted(node.Syntax, node.SyntaxTree)
			Dim localState As LocalState = Nothing
			If (flag) Then
				localState = Me.State.Clone()
				Me.SetUnreachable()
			End If
			Dim methodGroupOpt As BoundMethodGroup = node.MethodGroupOpt
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.ReceiverOpt
			Dim method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = node.Method
			If (methodGroupOpt IsNot Nothing AndAlso Me._firstInRegion = methodGroupOpt AndAlso Me._regionPlace = AbstractFlowPass(Of LocalState).RegionPlace.Before) Then
				Me.EnterRegion()
			End If
			If (boundExpression Is Nothing) Then
				If (methodGroupOpt IsNot Nothing) Then
					receiverOpt = methodGroupOpt.ReceiverOpt
				Else
					receiverOpt = Nothing
				End If
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = receiverOpt
				If (boundExpression1 IsNot Nothing AndAlso Not boundExpression1.WasCompilerGenerated) Then
					Dim kind As BoundKind = boundExpression1.Kind
					If (kind <> BoundKind.TypeExpression AndAlso kind <> BoundKind.NamespaceExpression AndAlso kind <> BoundKind.TypeOrValueExpression) Then
						Me.VisitUnreachableReceiver(boundExpression1)
					End If
				End If
			Else
				Me.VisitCallReceiver(boundExpression, method)
			End If
			If (methodGroupOpt IsNot Nothing AndAlso Me._lastInRegion = methodGroupOpt AndAlso Me.IsInside) Then
				Me.LeaveRegion()
			End If
			Me.VisitArguments(node.Arguments, method.Parameters)
			If (boundExpression IsNot Nothing AndAlso boundExpression.IsLValue) Then
				Me.WriteLValueCallReceiver(boundExpression, method)
			End If
			If (flag) Then
				Me.SetState(localState)
			End If
			Return Nothing
		End Function

		Private Sub VisitCallReceiver(ByVal receiver As BoundExpression, ByVal method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			If (Not method.IsReducedExtensionMethod OrElse Not receiver.IsValue()) Then
				Me.VisitRvalue(receiver, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
				Return
			End If
			Me.VisitArgument(receiver, method.CallsiteReducedFromMethod.Parameters(0))
		End Sub

		Public Overrides Function VisitCaseBlock(ByVal node As BoundCaseBlock) As BoundNode
			Throw ExceptionUtilities.Unreachable
		End Function

		Private Sub VisitCaseBlocks(ByVal caseBlocks As ImmutableArray(Of BoundCaseBlock))
			Dim instance As ArrayBuilder(Of LocalState) = ArrayBuilder(Of LocalState).GetInstance(caseBlocks.Length)
			Dim flag As Boolean = False
			Dim num As Integer = 0
			Dim length As Integer = caseBlocks.Length - 1
			Dim enumerator As ImmutableArray(Of BoundCaseBlock).Enumerator = caseBlocks.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As BoundCaseBlock = enumerator.Current
				Me.VisitStatement(current.CaseStatement)
				Dim localState As LocalState = Me.State.Clone()
				Me.VisitStatement(current.Body)
				flag = If(flag, True, current.Syntax.Kind() = SyntaxKind.CaseElseBlock)
				If (num <> length OrElse Not flag) Then
					instance.Add(Me.State.Clone())
					Me.SetState(localState)
				End If
				num = num + 1
			End While
			Dim enumerator1 As ArrayBuilder(Of LocalState).Enumerator = instance.GetEnumerator()
			While enumerator1.MoveNext()
				Dim current1 As LocalState = enumerator1.Current
				Me.IntersectWith(Me.State, current1)
			End While
			instance.Free()
		End Sub

		Public Overrides Function VisitCaseStatement(ByVal node As BoundCaseStatement) As BoundNode
			If (node.ConditionOpt Is Nothing) Then
				Dim enumerator As ImmutableArray(Of BoundCaseClause).Enumerator = node.CaseClauses.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As BoundCaseClause = enumerator.Current
					Select Case current.Kind
						Case BoundKind.SimpleCaseClause
							Me.VisitSimpleCaseClause(DirectCast(current, BoundSimpleCaseClause))
							Continue While
						Case BoundKind.RangeCaseClause
							Me.VisitRangeCaseClause(DirectCast(current, BoundRangeCaseClause))
							Continue While
						Case BoundKind.RelationalCaseClause
							Me.VisitRelationalCaseClause(DirectCast(current, BoundRelationalCaseClause))
							Continue While
					End Select
					Throw ExceptionUtilities.UnexpectedValue(current.Kind)
				End While
			Else
				Me.VisitRvalue(node.ConditionOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			Return Nothing
		End Function

		Protected Overridable Sub VisitCatchBlock(ByVal catchBlock As BoundCatchBlock, ByRef finallyState As LocalState)
			If (catchBlock.ExceptionSourceOpt IsNot Nothing) Then
				Me.VisitLvalue(catchBlock.ExceptionSourceOpt, False)
			End If
			If (catchBlock.ErrorLineNumberOpt IsNot Nothing) Then
				Me.VisitRvalue(catchBlock.ErrorLineNumberOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			If (catchBlock.ExceptionFilterOpt IsNot Nothing) Then
				Me.VisitRvalue(catchBlock.ExceptionFilterOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			Me.VisitBlock(catchBlock.Body)
		End Sub

		Public Overrides Function VisitCollectionInitializerExpression(ByVal node As BoundCollectionInitializerExpression) As BoundNode
			Return Me.VisitObjectInitializerExpressionBase(node)
		End Function

		Public Overrides Function VisitComplexConditionalAccessReceiver(ByVal node As BoundComplexConditionalAccessReceiver) As BoundNode
			Dim localState As LocalState = Me.State.Clone()
			Me.VisitLvalue(node.ValueTypeReceiver, False)
			Me.IntersectWith(Me.State, localState)
			localState = Me.State.Clone()
			Me.VisitRvalue(node.ReferenceTypeReceiver, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Me.IntersectWith(Me.State, localState)
			Return Nothing
		End Function

		Public Overrides Function VisitCompoundAssignmentTargetPlaceholder(ByVal node As BoundCompoundAssignmentTargetPlaceholder) As BoundNode
			Dim getPlaceholderSubstitute As BoundExpression = Me(node)
			If (getPlaceholderSubstitute IsNot Nothing) Then
				Me.VisitRvalue(getPlaceholderSubstitute, AbstractFlowPass(Of LocalState).ReadWriteContext.CompoundAssignmentTarget, False)
			End If
			Return Nothing
		End Function

		Protected Sub VisitCondition(ByVal node As BoundExpression)
			If (node = Me._firstInRegion AndAlso Me._regionPlace = AbstractFlowPass(Of LocalState).RegionPlace.Before) Then
				Me.EnterRegion()
			End If
			Me.Visit(node, True)
			Me.AdjustConditionalState(node)
			If (node = Me._lastInRegion AndAlso Me.IsInside) Then
				Me.LeaveRegion()
			End If
		End Sub

		Public Overrides Function VisitConditionalAccess(ByVal node As BoundConditionalAccess) As BoundNode
			Me.VisitRvalue(node.Receiver, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			If (Not node.Receiver.IsConstant) Then
				Dim localState As LocalState = Me.State.Clone()
				Me.VisitRvalue(node.AccessExpression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
				Me.IntersectWith(Me.State, localState)
			ElseIf (Not node.Receiver.ConstantValueOpt.IsNothing) Then
				Me.VisitRvalue(node.AccessExpression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Else
				Dim localState1 As LocalState = Me.State.Clone()
				Me.SetUnreachable()
				Me.VisitRvalue(node.AccessExpression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
				Me.SetState(localState1)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitConditionalAccessReceiverPlaceholder(ByVal node As BoundConditionalAccessReceiverPlaceholder) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitConditionalGoto(ByVal node As BoundConditionalGoto) As BoundNode
			Me.VisitCondition(node.Condition)
			If (Not node.JumpIfTrue) Then
				Me._pendingBranches.Add(New AbstractFlowPass(Of LocalState).PendingBranch(node, Me.StateWhenFalse, Me._nesting))
				Me.SetState(Me.StateWhenTrue)
			Else
				Me._pendingBranches.Add(New AbstractFlowPass(Of LocalState).PendingBranch(node, Me.StateWhenTrue, Me._nesting))
				Me.SetState(Me.StateWhenFalse)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitContinueStatement(ByVal node As BoundContinueStatement) As BoundNode
			Me._pendingBranches.Add(New AbstractFlowPass(Of LocalState).PendingBranch(node, Me.State, Me._nesting))
			Me.SetUnreachable()
			Return Nothing
		End Function

		Public Overrides Function VisitConversion(ByVal node As BoundConversion) As BoundNode
			Me.VisitRvalue(node.Operand, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitConvertedTupleElements(ByVal node As BoundConvertedTupleElements) As BoundNode
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overrides Function VisitConvertedTupleLiteral(ByVal node As BoundConvertedTupleLiteral) As BoundNode
			Return Me.VisitTupleExpression(node)
		End Function

		Public Overrides Function VisitDelegateCreationExpression(ByVal node As BoundDelegateCreationExpression) As BoundNode
			Dim receiverOpt As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim methodGroupOpt As BoundMethodGroup = node.MethodGroupOpt
			If (methodGroupOpt IsNot Nothing AndAlso Me._firstInRegion = methodGroupOpt AndAlso Me._regionPlace = AbstractFlowPass(Of LocalState).RegionPlace.Before) Then
				Me.EnterRegion()
			End If
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.ReceiverOpt
			Dim method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = node.Method
			If (boundExpression Is Nothing) Then
				If (methodGroupOpt IsNot Nothing) Then
					receiverOpt = methodGroupOpt.ReceiverOpt
				Else
					receiverOpt = Nothing
				End If
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = receiverOpt
				If (boundExpression1 IsNot Nothing AndAlso Not boundExpression1.WasCompilerGenerated) Then
					Dim kind As BoundKind = boundExpression1.Kind
					If (kind <> BoundKind.TypeExpression AndAlso kind <> BoundKind.NamespaceExpression AndAlso kind <> BoundKind.TypeOrValueExpression) Then
						Me.VisitUnreachableReceiver(boundExpression1)
					End If
				End If
			ElseIf (Not method.IsReducedExtensionMethod OrElse Not boundExpression.IsValue()) Then
				Me.VisitRvalue(boundExpression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Else
				Me.VisitArgument(boundExpression, method.CallsiteReducedFromMethod.Parameters(0))
			End If
			If (methodGroupOpt IsNot Nothing AndAlso Me._lastInRegion = methodGroupOpt AndAlso Me.IsInside) Then
				Me.LeaveRegion()
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitDimStatement(ByVal node As BoundDimStatement) As BoundNode
			Dim enumerator As ImmutableArray(Of BoundLocalDeclarationBase).Enumerator = node.LocalDeclarations.GetEnumerator()
			While enumerator.MoveNext()
				Me.VisitStatement(enumerator.Current)
			End While
			Return Nothing
		End Function

		Public Overrides Function VisitDirectCast(ByVal node As BoundDirectCast) As BoundNode
			Me.VisitRvalue(node.Operand, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Sub VisitDoLoopBottomConditionStatement(ByVal node As BoundDoLoopStatement)
			Dim stateWhenFalse As LocalState
			Me.LoopHead(node)
			Me.VisitStatement(node.Body)
			Me.ResolveContinues(node.ContinueLabel)
			Me.VisitCondition(node.ConditionOpt)
			If (Not node.ConditionIsUntil) Then
				stateWhenFalse = Me.StateWhenFalse
				Me.SetState(Me.StateWhenTrue)
			Else
				stateWhenFalse = Me.StateWhenTrue
				Me.SetState(Me.StateWhenFalse)
			End If
			Me.LoopTail(node)
			Me.ResolveBreaks(stateWhenFalse, node.ExitLabel)
		End Sub

		Public Overrides Function VisitDoLoopStatement(ByVal node As BoundDoLoopStatement) As BoundNode
			If (node.ConditionOpt Is Nothing) Then
				Me.VisitUnconditionalDoLoopStatement(node)
			ElseIf (Not node.ConditionIsTop) Then
				Me.VisitDoLoopBottomConditionStatement(node)
			Else
				Me.VisitDoLoopTopConditionStatement(node)
			End If
			Return Nothing
		End Function

		Public Sub VisitDoLoopTopConditionStatement(ByVal node As BoundDoLoopStatement)
			Dim stateWhenFalse As LocalState
			Me.LoopHead(node)
			Me.VisitCondition(node.ConditionOpt)
			If (Not node.ConditionIsUntil) Then
				stateWhenFalse = Me.StateWhenFalse
				Me.SetState(Me.StateWhenTrue)
			Else
				stateWhenFalse = Me.StateWhenTrue
				Me.SetState(Me.StateWhenFalse)
			End If
			Me.VisitStatement(node.Body)
			Me.ResolveContinues(node.ContinueLabel)
			Me.LoopTail(node)
			Me.ResolveBreaks(stateWhenFalse, node.ExitLabel)
		End Sub

		Public Overrides Function VisitEndStatement(ByVal node As BoundEndStatement) As BoundNode
			Me.SetUnreachable()
			Return Nothing
		End Function

		Public Overrides Function VisitEraseStatement(ByVal node As BoundEraseStatement) As BoundNode
			Dim enumerator As ImmutableArray(Of BoundAssignmentOperator).Enumerator = node.Clauses.GetEnumerator()
			While enumerator.MoveNext()
				Me.Visit(enumerator.Current)
			End While
			Return Nothing
		End Function

		Public Overrides Function VisitEventAccess(ByVal node As BoundEventAccess) As BoundNode
			If (node.ReceiverOpt IsNot Nothing) Then
				Me.VisitRvalue(node.ReceiverOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitExitStatement(ByVal node As BoundExitStatement) As BoundNode
			Me._pendingBranches.Add(New AbstractFlowPass(Of LocalState).PendingBranch(node, Me.State, Me._nesting))
			Me.SetUnreachable()
			Return Nothing
		End Function

		Private Sub VisitExpressionAsStatement(ByVal node As BoundExpression)
			Me.VisitRvalue(node, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
		End Sub

		Public Overrides Function VisitExpressionStatement(ByVal node As BoundExpressionStatement) As BoundNode
			Me.VisitExpressionAsStatement(node.Expression)
			Return Nothing
		End Function

		<DebuggerStepThrough>
		Protected Overrides Function VisitExpressionWithoutStackGuard(ByVal node As BoundExpression) As BoundExpression
			Return DirectCast(MyBase.Visit(node), BoundExpression)
		End Function

		Public Overrides Function VisitFieldAccess(ByVal node As BoundFieldAccess) As BoundNode
			Me.VisitFieldAccessInternal(node)
			Return Nothing
		End Function

		Protected Overridable Sub VisitFieldAccessInReadWriteContext(ByVal node As BoundFieldAccess, ByVal rwContext As AbstractFlowPass(Of LocalState).ReadWriteContext)
			Me.VisitFieldAccessInternal(node)
		End Sub

		Private Function VisitFieldAccessInternal(ByVal node As BoundFieldAccess) As BoundNode
			Dim receiverOpt As BoundExpression = node.ReceiverOpt
			If (AbstractFlowPass(Of LocalState).FieldAccessMayRequireTracking(node)) Then
				Me.VisitLvalue(receiverOpt, False)
			ElseIf (Not node.FieldSymbol.IsShared) Then
				Me.VisitRvalue(receiverOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Else
				Me.VisitUnreachableReceiver(receiverOpt)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitFieldInitializer(ByVal node As BoundFieldInitializer) As BoundNode
			Me.VisitRvalue(node.InitialValue, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Protected Overridable Sub VisitFinallyBlock(ByVal finallyBlock As BoundStatement, ByRef unsetInFinally As LocalState)
			Me.VisitStatement(finallyBlock)
		End Sub

		Protected Overridable Sub VisitForControlInitialization(ByVal node As BoundForToStatement)
			Me.VisitLvalue(node.ControlVariable, False)
		End Sub

		Protected Overridable Sub VisitForControlInitialization(ByVal node As BoundForEachStatement)
			Me.VisitLvalue(node.ControlVariable, False)
		End Sub

		Public Overrides Function VisitForEachStatement(ByVal node As BoundForEachStatement) As BoundNode
			Me.VisitForStatementVariableDeclaration(node)
			Me.VisitRvalue(node.Collection, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Me.LoopHead(node)
			Me.Split()
			Dim stateWhenTrue As LocalState = Me.StateWhenTrue
			Dim stateWhenFalse As LocalState = Me.StateWhenFalse
			Me.SetState(stateWhenTrue)
			Me.VisitForControlInitialization(node)
			Me.VisitStatement(node.Body)
			Me.ResolveContinues(node.ContinueLabel)
			Me.LoopTail(node)
			Me.ResolveBreaks(stateWhenFalse, node.ExitLabel)
			Return Nothing
		End Function

		Protected Overridable Sub VisitForInitValues(ByVal node As BoundForToStatement)
			Me.VisitRvalue(node.InitialValue, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Me.VisitRvalue(node.LimitValue, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Me.VisitRvalue(node.StepValue, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
		End Sub

		Protected Overridable Sub VisitForStatementVariableDeclaration(ByVal node As BoundForStatement)
		End Sub

		Public Overrides Function VisitForToStatement(ByVal node As BoundForToStatement) As BoundNode
			Me.VisitForStatementVariableDeclaration(node)
			Me.VisitForInitValues(node)
			Me.VisitForControlInitialization(node)
			Me.LoopHead(node)
			Me.Split()
			Dim stateWhenTrue As LocalState = Me.StateWhenTrue
			Dim stateWhenFalse As LocalState = Me.StateWhenFalse
			Me.SetState(stateWhenTrue)
			Me.VisitStatement(node.Body)
			Me.ResolveContinues(node.ContinueLabel)
			Me.LoopTail(node)
			Me.ResolveBreaks(stateWhenFalse, node.ExitLabel)
			Return Nothing
		End Function

		Public Overrides Function VisitGetType(ByVal node As BoundGetType) As BoundNode
			Me.VisitTypeExpression(node.SourceType)
			Return Nothing
		End Function

		Public Overrides Function VisitGotoStatement(ByVal node As BoundGotoStatement) As BoundNode
			Me._pendingBranches.Add(New AbstractFlowPass(Of LocalState).PendingBranch(node, Me.State, Me._nesting))
			Me.SetUnreachable()
			Return Nothing
		End Function

		Public Overrides Function VisitIfStatement(ByVal node As BoundIfStatement) As BoundNode
			Me.VisitCondition(node.Condition)
			Dim stateWhenTrue As LocalState = Me.StateWhenTrue
			Dim stateWhenFalse As LocalState = Me.StateWhenFalse
			Me.SetState(stateWhenTrue)
			Me.VisitStatement(node.Consequence)
			stateWhenTrue = Me.State
			Me.SetState(stateWhenFalse)
			If (node.AlternativeOpt IsNot Nothing) Then
				Me.VisitStatement(node.AlternativeOpt)
			End If
			Me.IntersectWith(Me.State, stateWhenTrue)
			Return Nothing
		End Function

		Public Overrides Function VisitInstrumentationPayloadRoot(ByVal node As BoundInstrumentationPayloadRoot) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitInterpolatedStringExpression(ByVal node As BoundInterpolatedStringExpression) As BoundNode
			Dim enumerator As ImmutableArray(Of BoundNode).Enumerator = node.Contents.GetEnumerator()
			While enumerator.MoveNext()
				Me.Visit(enumerator.Current)
			End While
			Return Nothing
		End Function

		Public Overrides Function VisitInterpolation(ByVal node As BoundInterpolation) As BoundNode
			Me.VisitRvalue(node.Expression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Me.VisitRvalue(node.AlignmentOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Me.VisitRvalue(node.FormatStringOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitLabelStatement(ByVal node As BoundLabelStatement) As BoundNode
			If (Me.ResolveBranches(node)) Then
				Me.backwardBranchChanged = True
			End If
			Dim label As LabelSymbol = node.Label
			Dim localState As LocalState = Me.LabelState(label)
			Me.IntersectWith(Me.State, localState)
			Me._labels(label) = New AbstractFlowPass(Of LocalState).LabelStateAndNesting(node, Me.State.Clone(), Me._nesting)
			Me._labelsSeen.Add(label)
			Return Nothing
		End Function

		Public Overrides Function VisitLambda(ByVal node As BoundLambda) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitLateAddressOfOperator(ByVal node As BoundLateAddressOfOperator) As BoundNode
			Me.Visit(node.MemberAccess)
			Return Nothing
		End Function

		Protected Overridable Sub VisitLateBoundArgument(ByVal arg As BoundExpression, ByVal isByRef As Boolean)
			If (isByRef) Then
				Me.VisitLvalue(arg, False)
				Return
			End If
			Me.VisitRvalue(arg, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
		End Sub

		Private Sub VisitLateBoundArguments(ByVal arguments As ImmutableArray(Of BoundExpression), ByVal isByRef As Boolean)
			Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = arguments.GetEnumerator()
			While enumerator.MoveNext()
				Me.VisitLateBoundArgument(enumerator.Current, isByRef)
			End While
			If (isByRef) Then
				Dim enumerator1 As ImmutableArray(Of BoundExpression).Enumerator = arguments.GetEnumerator()
				While enumerator1.MoveNext()
					Me.WriteArgument(enumerator1.Current, False)
				End While
			End If
		End Sub

		Public Overrides Function VisitLateInvocation(ByVal node As BoundLateInvocation) As BoundNode
			Dim flag As Boolean
			Dim member As BoundExpression = node.Member
			Me.Visit(node.Member)
			Dim argumentsOpt As ImmutableArray(Of BoundExpression) = node.ArgumentsOpt
			If (Not argumentsOpt.IsEmpty) Then
				flag = If(member.Kind = BoundKind.LateMemberAccess, True, False)
				Me.VisitLateBoundArguments(argumentsOpt, flag)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitLateMemberAccess(ByVal node As BoundLateMemberAccess) As BoundNode
			Me.VisitRvalue(node.ReceiverOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitLiteral(ByVal node As BoundLiteral) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitLocal(ByVal node As BoundLocal) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitLocalDeclaration(ByVal node As BoundLocalDeclaration) As BoundNode
			If (node.InitializerOpt IsNot Nothing) Then
				Me.VisitRvalue(node.InitializerOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			Return Nothing
		End Function

		Protected Overridable Sub VisitLocalInReadWriteContext(ByVal node As BoundLocal, ByVal rwContext As AbstractFlowPass(Of LocalState).ReadWriteContext)
		End Sub

		Public Overrides Function VisitLoweredConditionalAccess(ByVal node As BoundLoweredConditionalAccess) As BoundNode
			Me.VisitRvalue(node.ReceiverOrCondition, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Dim localState As LocalState = Me.State.Clone()
			Me.VisitRvalue(node.WhenNotNull, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Me.IntersectWith(Me.State, localState)
			If (node.WhenNullOpt IsNot Nothing) Then
				localState = Me.State.Clone()
				Me.VisitRvalue(node.WhenNullOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
				Me.IntersectWith(Me.State, localState)
			End If
			Return Nothing
		End Function

		Protected Overridable Sub VisitLvalue(ByVal node As BoundExpression, Optional ByVal dontLeaveRegion As Boolean = False)
			If (node = Me._firstInRegion AndAlso Me._regionPlace = AbstractFlowPass(Of LocalState).RegionPlace.Before) Then
				Me.EnterRegion()
			End If
			Dim kind As BoundKind = node.Kind
			If (CByte(kind) - CByte(BoundKind.WithLValueExpressionPlaceholder) <= CByte(BoundKind.WithLValueExpressionPlaceholder)) Then
				Dim getPlaceholderSubstitute As BoundExpression = Me(DirectCast(node, BoundValuePlaceholderBase))
				If (getPlaceholderSubstitute Is Nothing) Then
					Me.VisitRvalue(node, AbstractFlowPass(Of LocalState).ReadWriteContext.None, True)
				Else
					Me.VisitLvalue(getPlaceholderSubstitute, False)
				End If
			ElseIf (kind = BoundKind.FieldAccess) Then
				Me.VisitFieldAccessInternal(DirectCast(node, BoundFieldAccess))
			Else
				Select Case kind
					Case BoundKind.MeReference
					Case BoundKind.MyBaseReference
					Case BoundKind.MyClassReference
					Case BoundKind.Parameter
						Exit Select
					Case BoundKind.ValueTypeMeReference
					Case BoundKind.PreviousSubmissionReference
					Case BoundKind.HostObjectMemberReference
					Case BoundKind.PseudoVariable
					Label0:
						Me.VisitRvalue(node, AbstractFlowPass(Of LocalState).ReadWriteContext.None, True)
						Exit Select
					Case BoundKind.Local
						Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundLocal)
						If (Not boundLocal.LocalSymbol.IsByRef) Then
							Exit Select
						End If
						Me.VisitRvalue(boundLocal, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
						Exit Select
					Case Else
						GoTo Label0
				End Select
			End If
			If (Not dontLeaveRegion AndAlso node = Me._lastInRegion AndAlso Me.IsInside) Then
				Me.LeaveRegion()
			End If
		End Sub

		Public Overrides Function VisitLValueToRValueWrapper(ByVal node As BoundLValueToRValueWrapper) As BoundNode
			Me.Visit(node.UnderlyingLValue)
			Return Nothing
		End Function

		Public Overrides Function VisitMaximumMethodDefIndex(ByVal node As BoundMaximumMethodDefIndex) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitMeReference(ByVal node As BoundMeReference) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitMethodDefIndex(ByVal node As BoundMethodDefIndex) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitMethodGroup(ByVal node As BoundMethodGroup) As BoundNode
			Me.VisitRvalue(node.ReceiverOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitMidResult(ByVal node As BoundMidResult) As BoundNode
			Me.VisitRvalue(node.Original, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Me.VisitRvalue(node.Start, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			If (node.LengthOpt IsNot Nothing) Then
				Me.VisitRvalue(node.LengthOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			Me.VisitRvalue(node.Source, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitModuleVersionId(ByVal node As BoundModuleVersionId) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitModuleVersionIdString(ByVal node As BoundModuleVersionIdString) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitMyBaseReference(ByVal node As BoundMyBaseReference) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitNameOfOperator(ByVal node As BoundNameOfOperator) As BoundNode
			Dim localState As LocalState = Me.State.Clone()
			Me.SetUnreachable()
			Me.VisitRvalue(node.Argument, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Me.SetState(localState)
			Return Nothing
		End Function

		Public Overrides Function VisitNamespaceExpression(ByVal node As BoundNamespaceExpression) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitNewT(ByVal node As BoundNewT) As BoundNode
			Me.Visit(node.InitializerOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitNoOpStatement(ByVal node As BoundNoOpStatement) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitNoPiaObjectCreationExpression(ByVal node As BoundNoPiaObjectCreationExpression) As BoundNode
			Me.VisitObjectCreationExpressionInitializer(node.InitializerOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitNullableIsTrueOperator(ByVal node As BoundNullableIsTrueOperator) As BoundNode
			Me.VisitRvalue(node.Operand, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitObjectCreationExpression(ByVal node As BoundObjectCreationExpression) As BoundNode
			Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = node.Arguments.GetEnumerator()
			While enumerator.MoveNext()
				Me.VisitRvalue(enumerator.Current, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End While
			Me.VisitObjectCreationExpressionInitializer(node.InitializerOpt)
			Return Nothing
		End Function

		Protected Overridable Sub VisitObjectCreationExpressionInitializer(ByVal node As BoundObjectInitializerExpressionBase)
			Me.Visit(node)
		End Sub

		Public Overrides Function VisitObjectInitializerExpression(ByVal node As BoundObjectInitializerExpression) As BoundNode
			Return Me.VisitObjectInitializerExpressionBase(node)
		End Function

		Private Function VisitObjectInitializerExpressionBase(ByVal node As BoundObjectInitializerExpressionBase) As BoundNode
			Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = node.Initializers.GetEnumerator()
			While enumerator.MoveNext()
				Me.VisitExpressionAsStatement(enumerator.Current)
			End While
			Return Nothing
		End Function

		Public Overrides Function VisitParameter(ByVal node As BoundParameter) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitParameterEqualsValue(ByVal node As BoundParameterEqualsValue) As BoundNode
			Me.VisitRvalue(node.Value, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitParenthesized(ByVal node As BoundParenthesized) As BoundNode
			Me.VisitRvalue(node.Expression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitPropertyAccess(ByVal node As BoundPropertyAccess) As BoundNode
			Dim receiverOpt As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim propertyGroupOpt As BoundPropertyGroup = node.PropertyGroupOpt
			If (propertyGroupOpt IsNot Nothing AndAlso Me._firstInRegion = propertyGroupOpt AndAlso Me._regionPlace = AbstractFlowPass(Of LocalState).RegionPlace.Before) Then
				Me.EnterRegion()
			End If
			If (node.ReceiverOpt Is Nothing) Then
				If (propertyGroupOpt IsNot Nothing) Then
					receiverOpt = propertyGroupOpt.ReceiverOpt
				Else
					receiverOpt = Nothing
				End If
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = receiverOpt
				If (boundExpression IsNot Nothing AndAlso Not boundExpression.WasCompilerGenerated) Then
					Dim kind As BoundKind = boundExpression.Kind
					If (kind <> BoundKind.TypeExpression AndAlso kind <> BoundKind.NamespaceExpression AndAlso kind <> BoundKind.TypeOrValueExpression) Then
						Me.VisitUnreachableReceiver(boundExpression)
					End If
				End If
			Else
				Me.VisitRvalue(node.ReceiverOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			If (propertyGroupOpt IsNot Nothing AndAlso Me._lastInRegion = propertyGroupOpt AndAlso Me.IsInside) Then
				Me.LeaveRegion()
			End If
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Enumerator = node.Arguments.GetEnumerator()
			While enumerator.MoveNext()
				Me.VisitRvalue(enumerator.Current, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End While
			Return Nothing
		End Function

		Public Overrides Function VisitPropertyGroup(ByVal node As BoundPropertyGroup) As BoundNode
			Me.VisitRvalue(node.ReceiverOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitPropertyInitializer(ByVal node As BoundPropertyInitializer) As BoundNode
			Me.VisitRvalue(node.InitialValue, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitQueryExpression(ByVal node As BoundQueryExpression) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitRaiseEventStatement(ByVal node As BoundRaiseEventStatement) As BoundNode
			Me.VisitExpressionAsStatement(node.EventInvocation)
			Return Nothing
		End Function

		Public Overrides Function VisitRangeCaseClause(ByVal node As BoundRangeCaseClause) As BoundNode
			If (node.LowerBoundOpt Is Nothing) Then
				Me.VisitRvalue(node.LowerBoundConditionOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Else
				Me.VisitRvalue(node.LowerBoundOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			If (node.UpperBoundOpt Is Nothing) Then
				Me.VisitRvalue(node.UpperBoundConditionOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Else
				Me.VisitRvalue(node.UpperBoundOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitRangeVariable(ByVal node As BoundRangeVariable) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitRedimClause(ByVal node As BoundRedimClause) As BoundNode
			If (node.Preserve AndAlso Not Me.SuppressRedimOperandRvalueOnPreserve) Then
				Me.VisitRvalue(node.Operand, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			Me.VisitLvalue(node.Operand, False)
			Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = node.Indices.GetEnumerator()
			While enumerator.MoveNext()
				Me.VisitRvalue(enumerator.Current, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End While
			Return Nothing
		End Function

		Public Overrides Function VisitRedimStatement(ByVal node As BoundRedimStatement) As BoundNode
			Dim enumerator As ImmutableArray(Of BoundRedimClause).Enumerator = node.Clauses.GetEnumerator()
			While enumerator.MoveNext()
				Me.Visit(enumerator.Current)
			End While
			Return Nothing
		End Function

		Public Overrides Function VisitReferenceAssignment(ByVal node As BoundReferenceAssignment) As BoundNode
			Me.VisitRvalue(node.ByRefLocal, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Me.VisitRvalue(node.LValue, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitRelationalCaseClause(ByVal node As BoundRelationalCaseClause) As BoundNode
			If (node.ValueOpt Is Nothing) Then
				Me.VisitRvalue(node.ConditionOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Else
				Me.VisitRvalue(node.ValueOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitRelaxationLambda(ByVal node As BoundRelaxationLambda) As BoundNode
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overrides Function VisitRemoveHandlerStatement(ByVal node As BoundRemoveHandlerStatement) As BoundNode
			Return Me.VisitAddRemoveHandlerStatement(node)
		End Function

		Public Overrides Function VisitReturnStatement(ByVal node As BoundReturnStatement) As BoundNode
			If (Not node.IsEndOfMethodReturn()) Then
				Me.VisitRvalue(node.ExpressionOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
				Me._pendingBranches.Add(New AbstractFlowPass(Of LocalState).PendingBranch(node, Me.State, Me._nesting))
				Me.SetUnreachable()
			End If
			Return Nothing
		End Function

		Protected Sub VisitRvalue(ByVal node As BoundExpression, Optional ByVal rwContext As AbstractFlowPass(Of LocalState).ReadWriteContext = 0, Optional ByVal dontLeaveRegion As Boolean = False)
			If (node = Me._firstInRegion AndAlso Me._regionPlace = AbstractFlowPass(Of LocalState).RegionPlace.Before) Then
				Me.EnterRegion()
			End If
			If (rwContext <> AbstractFlowPass(Of LocalState).ReadWriteContext.None) Then
				Dim kind As BoundKind = node.Kind
				If (kind = BoundKind.FieldAccess) Then
					Me.VisitFieldAccessInReadWriteContext(DirectCast(node, BoundFieldAccess), rwContext)
					Me.Unsplit()
					If (Not dontLeaveRegion AndAlso node = Me._lastInRegion AndAlso Me.IsInside) Then
						Me.LeaveRegion()
					End If
					Return
				Else
					If (kind <> BoundKind.Local) Then
						Me.Visit(node, True)
						Me.Unsplit()
						If (Not dontLeaveRegion AndAlso node = Me._lastInRegion AndAlso Me.IsInside) Then
							Me.LeaveRegion()
						End If
						Return
					End If
					Me.VisitLocalInReadWriteContext(DirectCast(node, BoundLocal), rwContext)
					Me.Unsplit()
					If (Not dontLeaveRegion AndAlso node = Me._lastInRegion AndAlso Me.IsInside) Then
						Me.LeaveRegion()
					End If
					Return
				End If
			End If
			Me.Visit(node, True)
			Me.Unsplit()
			If (Not dontLeaveRegion AndAlso node = Me._lastInRegion AndAlso Me.IsInside) Then
				Me.LeaveRegion()
			End If
		End Sub

		Public Overrides Function VisitSelectStatement(ByVal node As BoundSelectStatement) As BoundNode
			Me.VisitStatement(node.ExpressionStatement)
			Dim caseBlocks As ImmutableArray(Of BoundCaseBlock) = node.CaseBlocks
			If (System.Linq.ImmutableArrayExtensions.Any(Of BoundCaseBlock)(caseBlocks)) Then
				Me.VisitCaseBlocks(caseBlocks)
				Me.ResolveBreaks(Me.State, node.ExitLabel)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitSequence(ByVal node As BoundSequence) As BoundNode
			If (Not node.SideEffects.IsEmpty) Then
				Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = node.SideEffects.GetEnumerator()
				While enumerator.MoveNext()
					Me.VisitExpressionAsStatement(enumerator.Current)
				End While
			End If
			If (node.ValueOpt IsNot Nothing) Then
				Me.VisitRvalue(node.ValueOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitSequencePoint(ByVal node As BoundSequencePoint) As BoundNode
			Me.VisitStatement(node.StatementOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitSequencePointWithSpan(ByVal node As BoundSequencePointWithSpan) As BoundNode
			Me.VisitStatement(node.StatementOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitSimpleCaseClause(ByVal node As BoundSimpleCaseClause) As BoundNode
			If (node.ValueOpt Is Nothing) Then
				Me.VisitRvalue(node.ConditionOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Else
				Me.VisitRvalue(node.ValueOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitSourceDocumentIndex(ByVal node As BoundSourceDocumentIndex) As BoundNode
			Return Nothing
		End Function

		Protected Overridable Sub VisitStatement(ByVal statement As BoundStatement)
			Me.Visit(statement)
		End Sub

		Public Overrides Function VisitStatementList(ByVal node As BoundStatementList) As BoundNode
			Dim enumerator As ImmutableArray(Of BoundStatement).Enumerator = node.Statements.GetEnumerator()
			While enumerator.MoveNext()
				Me.Visit(enumerator.Current)
			End While
			Return Nothing
		End Function

		Public Overrides Function VisitStopStatement(ByVal node As BoundStopStatement) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitSyncLockStatement(ByVal node As BoundSyncLockStatement) As BoundNode
			Me.VisitRvalue(node.LockExpression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Me.VisitStatement(node.Body)
			Return Nothing
		End Function

		Public Overrides Function VisitTernaryConditionalExpression(ByVal node As BoundTernaryConditionalExpression) As BoundNode
			Me.VisitCondition(node.Condition)
			Dim stateWhenTrue As LocalState = Me.StateWhenTrue
			Dim stateWhenFalse As LocalState = Me.StateWhenFalse
			If (Me.IsConstantTrue(node.Condition)) Then
				Me.SetState(stateWhenFalse)
				Me.VisitRvalue(node.WhenFalse, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
				Me.SetState(stateWhenTrue)
				Me.VisitRvalue(node.WhenTrue, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			ElseIf (Not Me.IsConstantFalse(node.Condition)) Then
				Me.SetState(stateWhenTrue)
				Me.VisitRvalue(node.WhenTrue, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
				Me.Unsplit()
				stateWhenTrue = Me.State
				Me.SetState(stateWhenFalse)
				Me.VisitRvalue(node.WhenFalse, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
				Me.Unsplit()
				Me.IntersectWith(Me.State, stateWhenTrue)
			Else
				Me.SetState(stateWhenTrue)
				Me.VisitRvalue(node.WhenTrue, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
				Me.SetState(stateWhenFalse)
				Me.VisitRvalue(node.WhenFalse, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitThrowStatement(ByVal node As BoundThrowStatement) As BoundNode
			Dim expressionOpt As BoundExpression = node.ExpressionOpt
			If (expressionOpt IsNot Nothing) Then
				Me.VisitRvalue(expressionOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			Me.SetUnreachable()
			Return Nothing
		End Function

		Protected Overridable Sub VisitTryBlock(ByVal tryBlock As BoundStatement, ByVal node As BoundTryStatement, ByRef tryState As LocalState)
			Me.VisitStatement(tryBlock)
		End Sub

		Public Overrides Function VisitTryCast(ByVal node As BoundTryCast) As BoundNode
			Me.VisitRvalue(node.Operand, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitTryStatement(ByVal node As BoundTryStatement) As BoundNode
			Dim savedPending As AbstractFlowPass(Of LocalState).SavedPending = Me.SavePending()
			Dim num As Integer = Me.IntroduceBlock()
			Dim num1 As Integer = 0
			Dim localState As LocalState = Me.State.Clone()
			Me.InitializeBlockStatement(num, num1)
			Me.VisitTryBlock(node.TryBlock, node, localState)
			Dim localState1 As LocalState = localState.Clone()
			Dim state As LocalState = Me.State
			Dim enumerator As ImmutableArray(Of BoundCatchBlock).Enumerator = node.CatchBlocks.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As BoundCatchBlock = enumerator.Current
				Me.SetState(localState.Clone())
				Me.InitializeBlockStatement(num, num1)
				Me.VisitCatchBlock(current, localState1)
				Me.IntersectWith(state, Me.State)
			End While
			If (node.FinallyBlockOpt IsNot Nothing) Then
				Dim savedPending1 As AbstractFlowPass(Of LocalState).SavedPending = Me.SavePending()
				Me.SetState(localState1)
				Dim localState2 As LocalState = Me.AllBitsSet()
				Me.InitializeBlockStatement(num, num1)
				Me.VisitFinallyBlock(node.FinallyBlockOpt, localState2)
				Dim enumerator1 As ArrayBuilder(Of AbstractFlowPass(Of LocalState).PendingBranch).Enumerator = savedPending1.PendingBranches.GetEnumerator()
				While enumerator1.MoveNext()
					Dim pendingBranch As AbstractFlowPass(Of LocalState).PendingBranch = enumerator1.Current
					Dim kind As Boolean = pendingBranch.Branch.Kind <> BoundKind.YieldStatement
					If (kind) Then
						Dim labelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol = Nothing
						Dim labelStateAndNesting As AbstractFlowPass(Of LocalState).LabelStateAndNesting = New AbstractFlowPass(Of LocalState).LabelStateAndNesting()
						If (Me.BothBranchAndLabelArePrefixedByNesting(pendingBranch, Nothing, True, labelSymbol, labelStateAndNesting)) Then
							kind = False
						End If
					End If
					If (Not kind) Then
						Continue While
					End If
					Me.UnionWith(pendingBranch.State, Me.State)
					If (Not Me.TrackUnassignments) Then
						Continue While
					End If
					Me.IntersectWith(pendingBranch.State, localState2)
				End While
				Me.RestorePending(savedPending1, False)
				Me.UnionWith(state, Me.State)
				If (Me.TrackUnassignments) Then
					Me.IntersectWith(state, localState2)
				End If
			End If
			Me.SetState(state)
			Me.FinalizeBlock(num)
			If (node.ExitLabelOpt IsNot Nothing) Then
				Me.ResolveBreaks(state, node.ExitLabelOpt)
			End If
			Me.RestorePending(savedPending, False)
			Return Nothing
		End Function

		Private Function VisitTupleExpression(ByVal node As BoundTupleExpression) As BoundNode
			Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = node.Arguments.GetEnumerator()
			While enumerator.MoveNext()
				Me.VisitRvalue(enumerator.Current, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End While
			Return Nothing
		End Function

		Public Overrides Function VisitTupleLiteral(ByVal node As BoundTupleLiteral) As BoundNode
			Return Me.VisitTupleExpression(node)
		End Function

		Public Overrides Function VisitTypeAsValueExpression(ByVal node As BoundTypeAsValueExpression) As BoundNode
			Me.Visit(node.Expression)
			Return Nothing
		End Function

		Public Overrides Function VisitTypeExpression(ByVal node As BoundTypeExpression) As BoundNode
			Me.VisitUnreachableReceiver(node.UnevaluatedReceiverOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitTypeOf(ByVal node As BoundTypeOf) As BoundNode
			Me.VisitRvalue(node.Operand, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitUnaryOperator(ByVal node As BoundUnaryOperator) As BoundNode
			If (node.OperatorKind <> UnaryOperatorKind.[Not]) Then
				Me.VisitRvalue(node.Operand, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Else
				Me.VisitCondition(node.Operand)
				Me.SetConditionalState(Me.StateWhenFalse, Me.StateWhenTrue)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitUnboundLambda(ByVal node As UnboundLambda) As BoundNode
			Dim boundLambda As Microsoft.CodeAnalysis.VisualBasic.BoundLambda = node.BindForErrorRecovery()
			If (boundLambda = Me._firstInRegion AndAlso Me._regionPlace = AbstractFlowPass(Of LocalState).RegionPlace.Before) Then
				Me.EnterRegion()
			End If
			Me.VisitLambda(node.BindForErrorRecovery())
			If (boundLambda = Me._lastInRegion AndAlso Me.IsInside) Then
				Me.LeaveRegion()
			End If
			Return Nothing
		End Function

		Private Sub VisitUnconditionalDoLoopStatement(ByVal node As BoundDoLoopStatement)
			Me.LoopHead(node)
			Me.VisitStatement(node.Body)
			Me.ResolveContinues(node.ContinueLabel)
			Dim localState As LocalState = Me.UnreachableState()
			Me.LoopTail(node)
			Me.ResolveBreaks(localState, node.ExitLabel)
		End Sub

		Private Sub VisitUnreachableReceiver(ByVal receiver As BoundExpression)
			If (receiver IsNot Nothing) Then
				Dim localState As LocalState = Me.State.Clone()
				Me.SetUnreachable()
				Me.VisitRvalue(receiver, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
				Me.SetState(localState)
			End If
		End Sub

		Public Overrides Function VisitUnstructuredExceptionHandlingStatement(ByVal node As BoundUnstructuredExceptionHandlingStatement) As BoundNode
			Me.Visit(node.Body)
			Return Nothing
		End Function

		Public Overrides Function VisitUserDefinedBinaryOperator(ByVal node As BoundUserDefinedBinaryOperator) As BoundNode
			Me.VisitRvalue(node.UnderlyingExpression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitUserDefinedConversion(ByVal node As BoundUserDefinedConversion) As BoundNode
			Me.VisitRvalue(node.UnderlyingExpression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitUserDefinedShortCircuitingOperator(ByVal node As BoundUserDefinedShortCircuitingOperator) As BoundNode
			If (node.LeftOperand IsNot Nothing) Then
				Me.VisitRvalue(node.LeftOperand, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			Me.VisitRvalue(node.BitwiseOperator, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitUserDefinedUnaryOperator(ByVal node As BoundUserDefinedUnaryOperator) As BoundNode
			Me.VisitRvalue(node.UnderlyingExpression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitUsingStatement(ByVal node As BoundUsingStatement) As BoundNode
			If (node.ResourceExpressionOpt Is Nothing) Then
				Dim enumerator As ImmutableArray(Of BoundLocalDeclarationBase).Enumerator = node.ResourceList.GetEnumerator()
				While enumerator.MoveNext()
					Me.Visit(enumerator.Current)
				End While
			Else
				Me.VisitRvalue(node.ResourceExpressionOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			Me.VisitStatement(node.Body)
			Return Nothing
		End Function

		Public Overrides Function VisitWhileStatement(ByVal node As BoundWhileStatement) As BoundNode
			Me.LoopHead(node)
			Me.VisitCondition(node.Condition)
			Dim stateWhenTrue As LocalState = Me.StateWhenTrue
			Dim stateWhenFalse As LocalState = Me.StateWhenFalse
			Me.SetState(stateWhenTrue)
			Me.VisitStatement(node.Body)
			Me.ResolveContinues(node.ContinueLabel)
			Me.LoopTail(node)
			Me.ResolveBreaks(stateWhenFalse, node.ExitLabel)
			Return Nothing
		End Function

		Public Overrides Function VisitWithLValueExpressionPlaceholder(ByVal node As BoundWithLValueExpressionPlaceholder) As BoundNode
			Me.Visit(Me(node))
			Return Nothing
		End Function

		<DebuggerStepThrough>
		Private Function VisitWithStackGuard(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = TryCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			If (boundExpression Is Nothing) Then
				boundNode = MyBase.Visit(node)
			Else
				boundNode = MyBase.VisitExpressionWithStackGuard(Me._recursionDepth, boundExpression)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitWithStatement(ByVal node As BoundWithStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim right As BoundExpression
			Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator
			Dim current As BoundExpression
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim flag As Boolean = False
			Dim flag1 As Boolean = False
			If (Me._firstInRegion IsNot Nothing AndAlso Me._regionPlace = AbstractFlowPass(Of LocalState).RegionPlace.Before AndAlso BoundNodeFinder.ContainsNode(node.OriginalExpression, Me._firstInRegion, Me._recursionDepth, Me.ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException())) Then
				flag = True
				Dim enumerator1 As ImmutableArray(Of BoundExpression).Enumerator = node.DraftInitializers.GetEnumerator()
				Do
					If (Not enumerator1.MoveNext()) Then
						If (flag AndAlso flag1) Then
							Me.EnterRegion()
						End If
						enumerator = node.DraftInitializers.GetEnumerator()
						While enumerator.MoveNext()
							current = enumerator.Current
							Me.VisitRvalue(current, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
						End While
						If (flag AndAlso flag1) Then
							Me.LeaveRegion()
						End If
						boundNode = Me.VisitBlock(node.Body)
						If (flag AndAlso Me._regionPlace <> AbstractFlowPass(Of LocalState).RegionPlace.After) Then
							Me.SetInvalidRegion()
						End If
						Return Nothing
					End If
					right = DirectCast(enumerator1.Current, BoundAssignmentOperator).Right
					If (right.Kind = BoundKind.LValueToRValueWrapper) Then
						right = DirectCast(right, BoundLValueToRValueWrapper).UnderlyingLValue
					End If
					flag1 = True
				Loop While Me._firstInRegion <> right AndAlso BoundNodeFinder.ContainsNode(Me._firstInRegion, right, Me._recursionDepth, Me.ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException())
				flag1 = False
			End If
			If (flag AndAlso flag1) Then
				Me.EnterRegion()
			End If
			enumerator = node.DraftInitializers.GetEnumerator()
			While enumerator.MoveNext()
				current = enumerator.Current
				Me.VisitRvalue(current, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End While
			If (flag AndAlso flag1) Then
				Me.LeaveRegion()
			End If
			boundNode = Me.VisitBlock(node.Body)
			If (flag AndAlso Me._regionPlace <> AbstractFlowPass(Of LocalState).RegionPlace.After) Then
				Me.SetInvalidRegion()
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitXmlAttribute(ByVal node As BoundXmlAttribute) As BoundNode
			Me.VisitRvalue(node.Name, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Me.VisitRvalue(node.Value, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitXmlDocument(ByVal node As BoundXmlDocument) As BoundNode
			Me.VisitRvalue(node.Declaration, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = node.ChildNodes.GetEnumerator()
			While enumerator.MoveNext()
				Me.VisitRvalue(enumerator.Current, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End While
			Return Nothing
		End Function

		Public Overrides Function VisitXmlElement(ByVal node As BoundXmlElement) As BoundNode
			Me.VisitRvalue(node.Argument, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = node.ChildNodes.GetEnumerator()
			While enumerator.MoveNext()
				Me.VisitRvalue(enumerator.Current, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End While
			Return Nothing
		End Function

		Public Overrides Function VisitXmlEmbeddedExpression(ByVal node As BoundXmlEmbeddedExpression) As BoundNode
			Me.VisitRvalue(node.Expression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitXmlMemberAccess(ByVal node As BoundXmlMemberAccess) As BoundNode
			Me.VisitRvalue(node.MemberAccess, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitXmlName(ByVal node As BoundXmlName) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitXmlNamespace(ByVal node As BoundXmlNamespace) As BoundNode
			Me.VisitRvalue(node.ObjectCreation, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitYieldStatement(ByVal node As BoundYieldStatement) As BoundNode
			Me.VisitRvalue(node.Expression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Me._pendingBranches.Add(New AbstractFlowPass(Of LocalState).PendingBranch(node, Me.State, Me._nesting))
			Return Nothing
		End Function

		Protected Overridable Sub WriteArgument(ByVal arg As BoundExpression, ByVal isOut As Boolean)
		End Sub

		Private Sub WriteLValueCallReceiver(ByVal receiver As BoundExpression, ByVal method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
			If (receiver.Type.IsReferenceType) Then
				Dim callsiteReducedFromMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = method.CallsiteReducedFromMethod
				If (callsiteReducedFromMethod Is Nothing OrElse callsiteReducedFromMethod.ParameterCount = 0 OrElse Not callsiteReducedFromMethod.Parameters(0).IsByRef) Then
					Return
				End If
			End If
			Me.WriteArgument(receiver, False)
		End Sub

		Friend Interface AbstractLocalState
			Function Clone() As LocalState
		End Interface

		Friend Structure BlockNesting
			Private ReadOnly _path As ImmutableArray(Of Integer)

			Private Sub New(ByVal builder As ArrayBuilder(Of Integer))
				Me = New AbstractFlowPass(Of LocalState).BlockNesting() With
				{
					._path = builder.ToImmutable()
				}
			End Sub

			Public Function IsPrefixedBy(ByVal other As ArrayBuilder(Of Integer), ByVal ignoreLast As Boolean) As Boolean
				Dim flag As Boolean
				Dim count As Integer = other.Count
				If (ignoreLast) Then
					count = count - 1
				End If
				If (count > Me._path.Length) Then
					flag = False
				Else
					Dim num As Integer = count - 1
					Dim num1 As Integer = 0
					While num1 <= num
						If (Me._path(num1) = other(num1)) Then
							num1 = num1 + 1
						Else
							flag = False
							Return flag
						End If
					End While
					flag = True
				End If
				Return flag
			End Function

			Public Shared Widening Operator CType(ByVal builder As ArrayBuilder(Of Integer)) As AbstractFlowPass(Of LocalState).BlockNesting
				Return New AbstractFlowPass(Of LocalState).BlockNesting(builder)
			End Operator
		End Structure

		Protected Structure LabelStateAndNesting
			Public ReadOnly Target As BoundLabelStatement

			Public ReadOnly State As LocalState

			Public ReadOnly Nesting As AbstractFlowPass(Of LocalState).BlockNesting

			Public Sub New(ByVal target As BoundLabelStatement, ByVal state As LocalState, ByVal nesting As AbstractFlowPass(Of LocalState).BlockNesting)
				Me = New AbstractFlowPass(Of LocalState).LabelStateAndNesting() With
				{
					.Target = target,
					.State = state,
					.Nesting = nesting
				}
			End Sub
		End Structure

		Friend Class PendingBranch
			Public ReadOnly Branch As BoundStatement

			Public State As LocalState

			Public Nesting As AbstractFlowPass(Of LocalState).BlockNesting

			Public ReadOnly Property Label As LabelSymbol
				Get
					Dim labelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol
					Dim kind As BoundKind = Me.Branch.Kind
					If (kind <= BoundKind.ContinueStatement) Then
						If (kind = BoundKind.ExitStatement) Then
							labelSymbol = DirectCast(Me.Branch, BoundExitStatement).Label
						Else
							If (kind <> BoundKind.ContinueStatement) Then
								labelSymbol = Nothing
								Return labelSymbol
							End If
							labelSymbol = DirectCast(Me.Branch, BoundContinueStatement).Label
						End If
					ElseIf (kind = BoundKind.GotoStatement) Then
						labelSymbol = DirectCast(Me.Branch, BoundGotoStatement).Label
					Else
						If (kind <> BoundKind.ConditionalGoto) Then
							labelSymbol = Nothing
							Return labelSymbol
						End If
						labelSymbol = DirectCast(Me.Branch, BoundConditionalGoto).Label
					End If
					Return labelSymbol
				End Get
			End Property

			Public Sub New(ByVal branch As BoundStatement, ByVal state As LocalState, ByVal nesting As AbstractFlowPass(Of LocalState).BlockNesting)
				MyBase.New()
				Me.Branch = branch
				Me.State = state.Clone()
				Me.Nesting = nesting
			End Sub
		End Class

		Protected Enum ReadWriteContext
			None
			CompoundAssignmentTarget
			ByRefArgument
		End Enum

		Protected Enum RegionPlace
			Before
			Inside
			After
		End Enum

		Protected Class SavedPending
			Public ReadOnly PendingBranches As ArrayBuilder(Of AbstractFlowPass(Of LocalState).PendingBranch)

			Public ReadOnly LabelsSeen As HashSet(Of LabelSymbol)

			Public Sub New(ByRef _pendingBranches As ArrayBuilder(Of AbstractFlowPass(Of LocalState).PendingBranch), ByRef _labelsSeen As HashSet(Of LabelSymbol))
				MyBase.New()
				Me.PendingBranches = _pendingBranches
				Me.LabelsSeen = _labelsSeen
				_pendingBranches = ArrayBuilder(Of AbstractFlowPass(Of LocalState).PendingBranch).GetInstance()
				_labelsSeen = New HashSet(Of LabelSymbol)()
			End Sub
		End Class
	End Class
End Namespace