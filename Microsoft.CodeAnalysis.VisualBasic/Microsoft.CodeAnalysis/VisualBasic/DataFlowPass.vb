Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Text

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class DataFlowPass
		Inherits AbstractFlowPass(Of DataFlowPass.LocalState)
		Protected ReadOnly initiallyAssignedVariables As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)

		Private ReadOnly _trackStructsWithIntrinsicTypedFields As Boolean

		Private ReadOnly _unusedVariables As HashSet(Of LocalSymbol)

		Private ReadOnly _writtenVariables As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)

		Private ReadOnly _variableSlot As Dictionary(Of DataFlowPass.VariableIdentifier, Integer)

		Protected variableBySlot As DataFlowPass.VariableIdentifier()

		Protected nextVariableSlot As Integer

		Private _alreadyReported As BitVector

		Private _seenOnErrorOrResume As Boolean

		Protected _convertInsufficientExecutionStackExceptionToCancelledByStackGuardException As Boolean

		Private ReadOnly _isEmptyStructType As Dictionary(Of NamedTypeSymbol, Boolean)

		Private _typeToMembersCache As Dictionary(Of TypeSymbol, ImmutableArray(Of FieldSymbol))

		Private _tryState As Nullable(Of DataFlowPass.LocalState)

		Protected Overridable ReadOnly Property EnableBreakingFlowAnalysisFeatures As Boolean
			Get
				Return False
			End Get
		End Property

		Protected Overridable ReadOnly Property IgnoreOutSemantics As Boolean
			Get
				Return True
			End Get
		End Property

		Protected Overridable ReadOnly Property ProcessCompilerGeneratedLocals As Boolean
			Get
				Return False
			End Get
		End Property

		Protected Overrides ReadOnly Property SuppressRedimOperandRvalueOnPreserve As Boolean
			Get
				Return True
			End Get
		End Property

		Private ReadOnly Shared Property UnreachableBitsSet As BitVector
			Get
				Return BitVector.AllSet(1)
			End Get
		End Property

		Friend Sub New(ByVal info As FlowAnalysisInfo, ByVal suppressConstExpressionsSupport As Boolean, Optional ByVal trackStructsWithIntrinsicTypedFields As Boolean = False)
			MyBase.New(info, suppressConstExpressionsSupport)
			Me._unusedVariables = New HashSet(Of LocalSymbol)()
			Me._writtenVariables = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
			Me._variableSlot = New Dictionary(Of DataFlowPass.VariableIdentifier, Integer)()
			ReDim Me.variableBySlot(10)
			Me.nextVariableSlot = 2
			Me._convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = False
			Me._isEmptyStructType = New Dictionary(Of NamedTypeSymbol, Boolean)()
			Me._typeToMembersCache = Nothing
			Me.initiallyAssignedVariables = Nothing
			Me._trackStructsWithIntrinsicTypedFields = trackStructsWithIntrinsicTypedFields
		End Sub

		Friend Sub New(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo, ByVal suppressConstExpressionsSupport As Boolean, Optional ByVal initiallyAssignedVariables As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = Nothing, Optional ByVal trackUnassignments As Boolean = False, Optional ByVal trackStructsWithIntrinsicTypedFields As Boolean = False)
			MyBase.New(info, region, suppressConstExpressionsSupport, trackUnassignments)
			Me._unusedVariables = New HashSet(Of LocalSymbol)()
			Me._writtenVariables = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
			Me._variableSlot = New Dictionary(Of DataFlowPass.VariableIdentifier, Integer)()
			ReDim Me.variableBySlot(10)
			Me.nextVariableSlot = 2
			Me._convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = False
			Me._isEmptyStructType = New Dictionary(Of NamedTypeSymbol, Boolean)()
			Me._typeToMembersCache = Nothing
			Me.initiallyAssignedVariables = initiallyAssignedVariables
			Me._trackStructsWithIntrinsicTypedFields = trackStructsWithIntrinsicTypedFields
		End Sub

		Protected Overrides Function AllBitsSet() As DataFlowPass.LocalState
			Dim localState As DataFlowPass.LocalState = New DataFlowPass.LocalState(BitVector.AllSet(Me.nextVariableSlot))
			localState.Unassign(0)
			Return localState
		End Function

		Public Shared Sub Analyze(ByVal info As FlowAnalysisInfo, ByVal diagnostics As DiagnosticBag, ByVal suppressConstExpressionsSupport As Boolean)
			Dim dataFlowPass As Microsoft.CodeAnalysis.VisualBasic.DataFlowPass = New Microsoft.CodeAnalysis.VisualBasic.DataFlowPass(info, suppressConstExpressionsSupport, False)
			If (diagnostics IsNot Nothing) Then
				dataFlowPass._convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = True
			End If
			Try
				Try
					dataFlowPass.Analyze()
					If (diagnostics IsNot Nothing) Then
						diagnostics.AddRange(dataFlowPass.diagnostics)
					End If
				Catch cancelledByStackGuardException As BoundTreeVisitor.CancelledByStackGuardException When diagnostics IsNot Nothing
					cancelledByStackGuardException.AddAnError(diagnostics)
					ProjectData.ClearProjectError()
				End Try
			Finally
				dataFlowPass.Free()
			End Try
		End Sub

		Protected Sub AppendBitName(ByVal bit As Integer, ByVal builder As StringBuilder)
			Dim variableIdentifier As DataFlowPass.VariableIdentifier = Me.variableBySlot(bit)
			If (variableIdentifier.ContainingSlot > 0) Then
				Me.AppendBitName(variableIdentifier.ContainingSlot, builder)
				builder.Append(".")
			End If
			builder.Append(If(bit = 0, "*", variableIdentifier.Symbol.Name))
		End Sub

		Protected Sub AppendBitNames(ByVal a As BitVector, ByVal builder As StringBuilder)
			Dim enumerator As IEnumerator(Of Integer) = Nothing
			Using flag As Boolean = False
				enumerator = a.TrueBits().GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Integer = enumerator.Current
					If (flag) Then
						builder.Append(", ")
					End If
					flag = True
					Me.AppendBitName(current, builder)
				End While
			End Using
		End Sub

		Protected Overridable Sub Assign(ByVal node As BoundNode, ByVal value As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, Optional ByVal assigned As Boolean = True)
			Dim kind As BoundKind = node.Kind
			If (kind > BoundKind.LocalDeclaration) Then
				If (CByte(kind) - CByte(BoundKind.ForToStatement) <= CByte(BoundKind.OmittedArgument)) Then
					Dim orCreateSlot As Integer = Me.GetOrCreateSlot(DirectCast(node, BoundForStatement).DeclaredOrInferredLocalOpt, 0)
					Me.SetSlotState(orCreateSlot, If(assigned, True, Not Me.State.Reachable))
					Return
				End If
				If (kind = BoundKind.MeReference) Then
					Me.SetSlotState(Me.GetOrCreateSlot(Me.MeParameter, 0), assigned)
					If (assigned) Then
						Me.NoteWrite(Me.MeParameter, value)
						Return
					End If
				Else
					Select Case kind
						Case BoundKind.Local
							Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = DirectCast(node, BoundLocal).LocalSymbol
							If (localSymbol.IsCompilerGenerated AndAlso Not Me.ProcessCompilerGeneratedLocals) Then
								Exit Select
							End If
							Me.SetSlotState(Me.GetOrCreateSlot(localSymbol, 0), assigned)
							If (localSymbol.IsFunctionValue) Then
								Me.SetSlotState(1, assigned)
							End If
							If (Not assigned) Then
								Exit Select
							End If
							Me.NoteWrite(localSymbol, value)
							Return
						Case BoundKind.PseudoVariable
						Case BoundKind.ByRefArgumentPlaceholder
							Exit Select
						Case BoundKind.Parameter
							Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = DirectCast(node, BoundParameter).ParameterSymbol
							Me.SetSlotState(Me.GetOrCreateSlot(parameterSymbol, 0), assigned)
							If (Not assigned) Then
								Exit Select
							End If
							Me.NoteWrite(parameterSymbol, value)
							Return
						Case BoundKind.ByRefArgumentWithCopyBack
							Me.Assign(DirectCast(node, BoundByRefArgumentWithCopyBack).OriginalArgument, value, assigned)
							Exit Select
						Case Else
							Return
					End Select
				End If
			ElseIf (CByte(kind) - CByte(BoundKind.WithLValueExpressionPlaceholder) <= CByte(BoundKind.OmittedArgument)) Then
				Dim getPlaceholderSubstitute As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = MyBase.GetPlaceholderSubstitute(DirectCast(node, BoundValuePlaceholderBase))
				If (getPlaceholderSubstitute IsNot Nothing) Then
					Me.Assign(getPlaceholderSubstitute, value, assigned)
					Return
				End If
			ElseIf (CByte(kind) - CByte(BoundKind.FieldAccess) <= CByte(BoundKind.OmittedArgument)) Then
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Dim slotCollection As DataFlowPass.SlotCollection = Me.MakeSlotsForExpression(boundExpression)
				Dim count As Integer = slotCollection.Count - 1
				Dim num As Integer = 0
				Do
					Me.SetSlotState(slotCollection(num), assigned)
					num = num + 1
				Loop While num <= count
				slotCollection.Free()
				If (assigned) Then
					Me.NoteWrite(boundExpression, value)
					Return
				End If
			Else
				If (kind <> BoundKind.LocalDeclaration) Then
					Return
				End If
				Dim boundLocalDeclaration As Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclaration = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclaration)
				Dim localSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = boundLocalDeclaration.LocalSymbol
				Dim orCreateSlot1 As Integer = Me.GetOrCreateSlot(localSymbol1, 0)
				Me.SetSlotState(orCreateSlot1, If(assigned, True, Not Me.State.Reachable))
				If (assigned AndAlso (value IsNot Nothing OrElse boundLocalDeclaration.InitializedByAsNew)) Then
					Me.NoteWrite(localSymbol1, value)
					Return
				End If
			End If
		End Sub

		Friend Overridable Sub AssignLocalOnDeclaration(ByVal local As LocalSymbol, ByVal node As BoundLocalDeclaration)
			If (node.InitializerOpt IsNot Nothing OrElse node.InitializedByAsNew) Then
				Me.Assign(node, node.InitializerOpt, True)
			End If
		End Sub

		Protected Sub CheckAssigned(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal node As SyntaxNode, Optional ByVal rwContext As AbstractFlowPass(Of DataFlowPass.LocalState).ReadWriteContext = 0)
			If (symbol IsNot Nothing) Then
				Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
				If (localSymbol Is Nothing OrElse Not localSymbol.IsCompilerGenerated OrElse Me.ProcessCompilerGeneratedLocals) Then
					If (localSymbol IsNot Nothing AndAlso localSymbol.DeclarationKind = LocalDeclarationKind.AmbiguousLocals) Then
						Dim ambiguousLocalsPseudoSymbol As DataFlowPass.AmbiguousLocalsPseudoSymbol = DirectCast(localSymbol, DataFlowPass.AmbiguousLocalsPseudoSymbol)
						Me.VisitAmbiguousLocalSymbol(ambiguousLocalsPseudoSymbol)
						Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol).Enumerator = ambiguousLocalsPseudoSymbol.Locals.GetEnumerator()
						While enumerator.MoveNext()
							Me.NoteRead(enumerator.Current)
						End While
						Return
					End If
					Dim orCreateSlot As Integer = Me.GetOrCreateSlot(symbol, 0)
					If (orCreateSlot >= Me.State.Assigned.Capacity) Then
						Me.Normalize(Me.State)
					End If
					If (orCreateSlot >= 2 AndAlso Me.State.Reachable AndAlso Not Me.State.IsAssigned(orCreateSlot)) Then
						Me.ReportUnassigned(symbol, node, rwContext, -1, Nothing)
					End If
					Me.NoteRead(symbol)
				End If
			End If
		End Sub

		Private Sub CheckAssigned(ByVal fieldAccess As BoundFieldAccess, ByVal node As SyntaxNode, Optional ByVal rwContext As AbstractFlowPass(Of DataFlowPass.LocalState).ReadWriteContext = 0)
			Dim num As Integer = 0
			If (Me.State.Reachable AndAlso Not Me.IsAssigned(fieldAccess, num)) Then
				Me.ReportUnassigned(fieldAccess.FieldSymbol, node, rwContext, num, fieldAccess)
			End If
			Me.NoteRead(fieldAccess)
		End Sub

		Protected Sub CheckAssignedFromArgumentWrite(ByVal expr As BoundExpression, ByVal node As SyntaxNode)
			If (Me.State.Reachable) Then
				Dim kind As BoundKind = expr.Kind
				If (kind = BoundKind.FieldAccess) Then
					Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess)
					Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = boundFieldAccess.FieldSymbol
					If (AbstractFlowPass(Of DataFlowPass.LocalState).FieldAccessMayRequireTracking(boundFieldAccess)) Then
						Me.CheckAssigned(boundFieldAccess, node, AbstractFlowPass(Of LocalState).ReadWriteContext.ByRefArgument)
						Return
					End If
				Else
					If (kind = BoundKind.EventAccess) Then
						Throw ExceptionUtilities.UnexpectedValue(expr.Kind)
					End If
					Select Case kind
						Case BoundKind.MeReference
						Case BoundKind.MyBaseReference
						Case BoundKind.MyClassReference
							Me.CheckAssigned(Me.MeParameter, node, AbstractFlowPass(Of LocalState).ReadWriteContext.None)
							Exit Select
						Case BoundKind.ValueTypeMeReference
						Case BoundKind.PreviousSubmissionReference
						Case BoundKind.HostObjectMemberReference
						Case BoundKind.PseudoVariable
							Exit Select
						Case BoundKind.Local
							Me.CheckAssigned(DirectCast(expr, BoundLocal).LocalSymbol, node, AbstractFlowPass(Of LocalState).ReadWriteContext.None)
							Return
						Case BoundKind.Parameter
							Me.CheckAssigned(DirectCast(expr, BoundParameter).ParameterSymbol, node, AbstractFlowPass(Of LocalState).ReadWriteContext.None)
							Return
						Case Else
							Return
					End Select
				End If
			End If
		End Sub

		Private Sub CheckAssignedFunctionValue(ByVal local As LocalSymbol, ByVal node As SyntaxNode)
			If (Not Me.State.FunctionAssignedValue AndAlso Not Me._seenOnErrorOrResume) Then
				Dim type As TypeSymbol = local.Type
				If (Me.EnableBreakingFlowAnalysisFeatures OrElse Not type.IsValueType OrElse type.IsIntrinsicOrEnumType() OrElse Not Me.IsEmptyStructType(type) OrElse MyBase.MethodSymbol.MethodKind = MethodKind.EventAdd AndAlso DirectCast(MyBase.MethodSymbol.AssociatedSymbol, EventSymbol).IsWindowsRuntimeEvent) Then
					Me.ReportUnassignedFunctionValue(local, node)
				End If
			End If
		End Sub

		Protected Function ConsiderLocalInitiallyAssigned(ByVal variable As LocalSymbol) As Boolean
			If (Not Me.State.Reachable) Then
				Return True
			End If
			If (Me.initiallyAssignedVariables Is Nothing) Then
				Return False
			End If
			Return Me.initiallyAssignedVariables.Contains(variable)
		End Function

		Protected Overrides Function ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException() As Boolean
			Return Me._convertInsufficientExecutionStackExceptionToCancelledByStackGuardException
		End Function

		Protected Overridable Function CreateLocalSymbolForVariables(ByVal declarations As ImmutableArray(Of BoundLocalDeclaration)) As LocalSymbol
			Return declarations(0).LocalSymbol
		End Function

		Private Function DeclaredVariableIsAlwaysAssignedBeforeInitializer(ByVal syntax As SyntaxNode, ByVal boundInitializer As BoundExpression, <Out> ByRef placeholder As BoundValuePlaceholderBase) As Boolean
			Dim createTemporaryLocalForInitialization As Boolean
			placeholder = Nothing
			If (boundInitializer IsNot Nothing AndAlso (boundInitializer.Kind = BoundKind.ObjectCreationExpression OrElse boundInitializer.Kind = BoundKind.NewT)) Then
				Dim initializerOpt As BoundObjectInitializerExpressionBase = DirectCast(boundInitializer, BoundObjectCreationExpressionBase).InitializerOpt
				If (initializerOpt IsNot Nothing AndAlso initializerOpt.Kind = BoundKind.ObjectInitializerExpression) Then
					Dim boundObjectInitializerExpression As Microsoft.CodeAnalysis.VisualBasic.BoundObjectInitializerExpression = DirectCast(initializerOpt, Microsoft.CodeAnalysis.VisualBasic.BoundObjectInitializerExpression)
					If (syntax IsNot Nothing AndAlso syntax.Kind() = SyntaxKind.VariableDeclarator) Then
						Dim variableDeclaratorSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax = DirectCast(syntax, Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)
						If (variableDeclaratorSyntax.AsClause Is Nothing OrElse variableDeclaratorSyntax.AsClause.Kind() <> SyntaxKind.AsNewClause) Then
							createTemporaryLocalForInitialization = False
							Return createTemporaryLocalForInitialization
						End If
						placeholder = boundObjectInitializerExpression.PlaceholderOpt
						createTemporaryLocalForInitialization = Not boundObjectInitializerExpression.CreateTemporaryLocalForInitialization
						Return createTemporaryLocalForInitialization
					End If
				End If
			End If
			createTemporaryLocalForInitialization = False
			Return createTemporaryLocalForInitialization
		End Function

		Private Function DescendThroughTupleRestFields(ByRef symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal containingSlot As Integer, ByVal forceContainingSlotsToExist As Boolean) As Integer
			Dim num As Integer
			Dim tupleFieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleFieldSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.TupleFieldSymbol)
			If (tupleFieldSymbol IsNot Nothing) Then
				Dim underlyingNamedType As TypeSymbol = DirectCast(symbol.ContainingType, TupleTypeSymbol).UnderlyingNamedType
				symbol = tupleFieldSymbol.TupleUnderlyingField
				While Not TypeSymbol.Equals(underlyingNamedType, symbol.ContainingType, TypeCompareKind.ConsiderEverything)
					Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = TryCast(System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)(underlyingNamedType.GetMembers("Rest")), Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
					If (fieldSymbol IsNot Nothing) Then
						If (forceContainingSlotsToExist) Then
							containingSlot = Me.GetOrCreateSlot(fieldSymbol, containingSlot)
						ElseIf (Not Me._variableSlot.TryGetValue(New DataFlowPass.VariableIdentifier(fieldSymbol, containingSlot), containingSlot)) Then
							num = -1
							Return num
						End If
						underlyingNamedType = fieldSymbol.Type.GetTupleUnderlyingTypeOrSelf()
					Else
						num = -1
						Return num
					End If
				End While
			End If
			num = containingSlot
			Return num
		End Function

		Protected Overrides Function Dump(ByVal state As DataFlowPass.LocalState) As String
			Dim stringBuilder As System.Text.StringBuilder = New System.Text.StringBuilder()
			stringBuilder.Append("[assigned ")
			Me.AppendBitNames(state.Assigned, stringBuilder)
			stringBuilder.Append("]")
			Return stringBuilder.ToString()
		End Function

		Protected Overridable Sub EnterParameter(ByVal parameter As ParameterSymbol)
			Dim num As Integer = Me.VariableSlot(parameter, 0)
			If (num >= 2) Then
				Me.State.Assign(num)
			End If
			Me.NoteWrite(parameter, Nothing)
		End Sub

		Private Sub EnterParameters(ByVal parameters As ImmutableArray(Of ParameterSymbol))
			Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = parameters.GetEnumerator()
			While enumerator.MoveNext()
				Me.EnterParameter(enumerator.Current)
			End While
		End Sub

		Protected Overrides Sub Free()
			Me._alreadyReported = BitVector.Null
			MyBase.Free()
		End Sub

		Private Shared Function GetFunctionLocalName(ByVal method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal local As LocalSymbol) As String
			Dim text As String
			Dim methodKind As Microsoft.CodeAnalysis.MethodKind = method.MethodKind
			If (methodKind = Microsoft.CodeAnalysis.MethodKind.AnonymousFunction) Then
				text = "<anonymous method>"
			ElseIf (methodKind = Microsoft.CodeAnalysis.MethodKind.Conversion OrElse methodKind = Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator) Then
				Dim operatorToken As SyntaxToken = DirectCast(DirectCast(local.ContainingSymbol, SourceMemberMethodSymbol).BlockSyntax, OperatorBlockSyntax).OperatorStatement.OperatorToken
				text = operatorToken.Text
			Else
				text = If(local.Name, method.Name)
			End If
			Return text
		End Function

		Protected Function GetNodeSymbol(ByVal node As BoundNode) As Microsoft.CodeAnalysis.VisualBasic.Symbol
			Dim nodeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol
			While True
				If (node IsNot Nothing) Then
					Dim kind As BoundKind = node.Kind
					If (kind <= BoundKind.LocalDeclaration) Then
						If (kind <= BoundKind.FieldAccess) Then
							If (CByte(kind) - CByte(BoundKind.WithLValueExpressionPlaceholder) <= CByte(BoundKind.OmittedArgument)) Then
								Dim getPlaceholderSubstitute As BoundExpression = MyBase.GetPlaceholderSubstitute(DirectCast(node, BoundValuePlaceholderBase))
								If (getPlaceholderSubstitute Is Nothing) Then
									nodeSymbol = Nothing
									Exit While
								Else
									nodeSymbol = Me.GetNodeSymbol(getPlaceholderSubstitute)
									Exit While
								End If
							ElseIf (kind = BoundKind.FieldAccess) Then
								Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess)
								If (Not AbstractFlowPass(Of DataFlowPass.LocalState).FieldAccessMayRequireTracking(boundFieldAccess)) Then
									nodeSymbol = Nothing
									Exit While
								Else
									node = boundFieldAccess.ReceiverOpt
								End If
							Else
								nodeSymbol = Nothing
								Return nodeSymbol
							End If
						ElseIf (kind = BoundKind.PropertyAccess) Then
							node = DirectCast(node, BoundPropertyAccess).ReceiverOpt
						ElseIf (kind = BoundKind.LocalDeclaration) Then
							nodeSymbol = DirectCast(node, BoundLocalDeclaration).LocalSymbol
							Exit While
						Else
							nodeSymbol = Nothing
							Return nodeSymbol
						End If
					ElseIf (kind > BoundKind.MeReference) Then
						Select Case kind
							Case BoundKind.Local
								nodeSymbol = DirectCast(node, BoundLocal).LocalSymbol
								Return nodeSymbol
							Case BoundKind.PseudoVariable
							Case BoundKind.ByRefArgumentPlaceholder
								nodeSymbol = Nothing
								Return nodeSymbol
							Case BoundKind.Parameter
								nodeSymbol = DirectCast(node, BoundParameter).ParameterSymbol
								Return nodeSymbol
							Case BoundKind.ByRefArgumentWithCopyBack
								node = DirectCast(node, BoundByRefArgumentWithCopyBack).OriginalArgument
								Continue While
							Case Else
								If (kind = BoundKind.RangeVariable) Then
									Exit Select
								Else
									nodeSymbol = Nothing
									Return nodeSymbol
								End If
						End Select
						nodeSymbol = DirectCast(node, BoundRangeVariable).RangeVariable
						Exit While
					ElseIf (CByte(kind) - CByte(BoundKind.ForToStatement) <= CByte(BoundKind.OmittedArgument)) Then
						nodeSymbol = DirectCast(node, BoundForStatement).DeclaredOrInferredLocalOpt
						Exit While
					ElseIf (kind = BoundKind.MeReference) Then
						nodeSymbol = Me.MeParameter
						Exit While
					Else
						nodeSymbol = Nothing
						Return nodeSymbol
					End If
				Else
					nodeSymbol = Nothing
					Exit While
				End If
			End While
			Return nodeSymbol
		End Function

		Protected Function GetOrCreateSlot(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, Optional ByVal containingSlot As Integer = 0) As Integer
			Dim num As Integer
			Dim num1 As Integer
			containingSlot = Me.DescendThroughTupleRestFields(symbol, containingSlot, True)
			If (containingSlot <> -1) Then
				Dim variableIdentifier As DataFlowPass.VariableIdentifier = New DataFlowPass.VariableIdentifier(symbol, containingSlot)
				If (Not Me._variableSlot.TryGetValue(variableIdentifier, num1)) Then
					If (symbol.Kind = SymbolKind.Local) Then
						Me._unusedVariables.Add(DirectCast(symbol, LocalSymbol))
					End If
					If (Me.IsEmptyStructType(DataFlowPass.GetVariableType(symbol))) Then
						num = -1
						Return num
					End If
					num1 = Me.nextVariableSlot
					Me.nextVariableSlot = Me.nextVariableSlot + 1
					Me._variableSlot.Add(variableIdentifier, num1)
					If (num1 >= CInt(Me.variableBySlot.Length)) Then
						Array.Resize(Of DataFlowPass.VariableIdentifier)(Me.variableBySlot, num1 * 2)
					End If
					Me.variableBySlot(num1) = variableIdentifier
				End If
				Me.Normalize(Me.State)
				num = num1
			Else
				num = -1
			End If
			Return num
		End Function

		Private Function GetStructInstanceFields(ByVal type As TypeSymbol) As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
			Dim immutableAndFree As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol) = New ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)()
			If (Me._typeToMembersCache Is Nothing OrElse Not Me._typeToMembersCache.TryGetValue(type, immutableAndFree)) Then
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol).GetInstance()
				Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = type.GetMembersUnordered().GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator.Current
					If (current.Kind <> SymbolKind.Field) Then
						Continue While
					End If
					Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
					If (fieldSymbol.IsVirtualTupleField OrElse Me.ShouldIgnoreStructField(fieldSymbol)) Then
						Continue While
					End If
					instance.Add(fieldSymbol)
				End While
				immutableAndFree = instance.ToImmutableAndFree()
				If (Me._typeToMembersCache Is Nothing) Then
					Me._typeToMembersCache = New Dictionary(Of TypeSymbol, ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol))()
				End If
				Me._typeToMembersCache.Add(type, immutableAndFree)
			End If
			Return immutableAndFree
		End Function

		Private Function GetUnassignedSymbolFirstLocation(ByVal sym As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess) As Location
			Dim unassignedSymbolFirstLocation As Location
			Dim item As Location
			Dim kind As SymbolKind = sym.Kind
			If (kind > SymbolKind.Local) Then
				If (kind = SymbolKind.Parameter) Then
					unassignedSymbolFirstLocation = Nothing
				Else
					If (kind <> SymbolKind.RangeVariable) Then
						unassignedSymbolFirstLocation = Nothing
						Return unassignedSymbolFirstLocation
					End If
					unassignedSymbolFirstLocation = Nothing
				End If
			ElseIf (kind <> SymbolKind.Field) Then
				If (kind <> SymbolKind.Local) Then
					unassignedSymbolFirstLocation = Nothing
					Return unassignedSymbolFirstLocation
				End If
				Dim locations As ImmutableArray(Of Location) = sym.Locations
				If (locations.IsEmpty) Then
					item = Nothing
				Else
					item = locations(0)
				End If
				unassignedSymbolFirstLocation = item
			ElseIf (Not sym.IsShared) Then
				Dim receiverOpt As BoundExpression = boundFieldAccess.ReceiverOpt
				Dim boundKind As Microsoft.CodeAnalysis.VisualBasic.BoundKind = receiverOpt.Kind
				If (boundKind = Microsoft.CodeAnalysis.VisualBasic.BoundKind.FieldAccess) Then
					Dim boundFieldAccess1 As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = DirectCast(receiverOpt, Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess)
					unassignedSymbolFirstLocation = Me.GetUnassignedSymbolFirstLocation(boundFieldAccess1.FieldSymbol, boundFieldAccess1)
				ElseIf (boundKind <> Microsoft.CodeAnalysis.VisualBasic.BoundKind.Local) Then
					unassignedSymbolFirstLocation = Nothing
				Else
					unassignedSymbolFirstLocation = Me.GetUnassignedSymbolFirstLocation(DirectCast(receiverOpt, BoundLocal).LocalSymbol, Nothing)
				End If
			Else
				unassignedSymbolFirstLocation = Nothing
			End If
			Return unassignedSymbolFirstLocation
		End Function

		Private Shared Function GetVariableType(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol) As TypeSymbol
			Dim type As TypeSymbol
			Dim kind As SymbolKind = symbol.Kind
			If (kind <= SymbolKind.Local) Then
				If (kind = SymbolKind.Field) Then
					type = DirectCast(symbol, FieldSymbol).Type
				Else
					If (kind <> SymbolKind.Local) Then
						Throw ExceptionUtilities.UnexpectedValue(symbol.Kind)
					End If
					type = DirectCast(symbol, LocalSymbol).Type
				End If
			ElseIf (kind = SymbolKind.Parameter) Then
				type = DirectCast(symbol, ParameterSymbol).Type
			Else
				If (kind <> SymbolKind.RangeVariable) Then
					Throw ExceptionUtilities.UnexpectedValue(symbol.Kind)
				End If
				type = DirectCast(symbol, RangeVariableSymbol).Type
			End If
			Return type
		End Function

		Protected Overrides Sub InitForScan()
			Dim enumerator As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator()
			MyBase.InitForScan()
			Dim enumerator1 As ImmutableArray(Of ParameterSymbol).Enumerator = MyBase.MethodParameters.GetEnumerator()
			While enumerator1.MoveNext()
				Me.GetOrCreateSlot(enumerator1.Current, 0)
			End While
			Me._alreadyReported = BitVector.Empty
			Me.EnterParameters(MyBase.MethodParameters)
			Dim meParameter As ParameterSymbol = Me.MeParameter
			If (meParameter IsNot Nothing) Then
				Me.GetOrCreateSlot(meParameter, 0)
				Me.EnterParameter(meParameter)
			End If
			If (Me.initiallyAssignedVariables IsNot Nothing) Then
				Try
					enumerator = Me.initiallyAssignedVariables.GetEnumerator()
					While enumerator.MoveNext()
						Me.SetSlotAssigned(Me.GetOrCreateSlot(enumerator.Current, 0))
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
			End If
		End Sub

		Private Shared Function IntersectBitArrays(ByRef receiver As BitVector, ByVal other As BitVector) As Boolean
			Dim flag As Boolean
			If (other(0)) Then
				flag = False
			ElseIf (Not receiver(0)) Then
				flag = receiver.IntersectWith(other)
			Else
				receiver = other.Clone()
				flag = True
			End If
			Return flag
		End Function

		Protected Overrides Function IntersectWith(ByRef self As DataFlowPass.LocalState, ByRef other As DataFlowPass.LocalState) As Boolean
			Dim flag As Boolean
			If (self.Reachable = other.Reachable) Then
				If (self.Assigned.Capacity <> other.Assigned.Capacity) Then
					Me.Normalize(self)
					Me.Normalize(other)
				End If
				flag = DataFlowPass.IntersectBitArrays(self.Assigned, other.Assigned)
			ElseIf (self.Reachable) Then
				flag = False
			Else
				self = other.Clone()
				flag = True
			End If
			Return flag
		End Function

		Private Function IsAssigned(ByVal node As BoundExpression, ByRef unassignedSlot As Integer) As Boolean
			Dim flag As Boolean
			Dim kind As BoundKind
			Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess
			unassignedSlot = -1
			If (Not Me.IsEmptyStructType(node.Type)) Then
				kind = node.Kind
				If (kind > BoundKind.MeReference) Then
					If (kind = BoundKind.Local) Then
						Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = DirectCast(node, BoundLocal).LocalSymbol
						If (localSymbol.DeclarationKind = LocalDeclarationKind.AmbiguousLocals) Then
							unassignedSlot = -1
							Me.VisitAmbiguousLocalSymbol(DirectCast(localSymbol, DataFlowPass.AmbiguousLocalsPseudoSymbol))
						Else
							unassignedSlot = Me.VariableSlot(localSymbol, 0)
						End If
					Else
						If (kind <> BoundKind.Parameter) Then
							GoTo Label2
						End If
						unassignedSlot = Me.VariableSlot(DirectCast(node, BoundParameter).ParameterSymbol, 0)
					End If
				ElseIf (CByte(kind) - CByte(BoundKind.WithLValueExpressionPlaceholder) <= CByte(BoundKind.OmittedArgument)) Then
					Dim getPlaceholderSubstitute As BoundExpression = MyBase.GetPlaceholderSubstitute(DirectCast(node, BoundValuePlaceholderBase))
					If (getPlaceholderSubstitute IsNot Nothing) Then
						flag = Me.IsAssigned(getPlaceholderSubstitute, unassignedSlot)
						Return flag
					End If
					unassignedSlot = -1
				ElseIf (kind = BoundKind.FieldAccess) Then
					boundFieldAccess = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess)
					If (AbstractFlowPass(Of DataFlowPass.LocalState).FieldAccessMayRequireTracking(boundFieldAccess) AndAlso Not Me.IsAssigned(boundFieldAccess.ReceiverOpt, unassignedSlot)) Then
						GoTo Label4
					End If
					flag = True
					Return flag
				Else
					If (kind <> BoundKind.MeReference) Then
						GoTo Label1
					End If
					unassignedSlot = Me.VariableSlot(Me.MeParameter, 0)
				End If
			Label6:
				flag = Me.State.IsAssigned(unassignedSlot)
			Else
				flag = True
			End If
			Return flag
		Label1:
			unassignedSlot = -1
			GoTo Label6
		Label2:
			If (kind <> BoundKind.RangeVariable) Then
				GoTo Label1
			End If
			unassignedSlot = -1
			flag = True
			Return flag
		Label4:
			unassignedSlot = Me.GetOrCreateSlot(boundFieldAccess.FieldSymbol, unassignedSlot)
			GoTo Label6
		End Function

		Private Shared Function IsConvertedLambda(ByVal value As BoundExpression) As Boolean
			Dim flag As Boolean
			If (value IsNot Nothing) Then
				While True
					Dim kind As BoundKind = value.Kind
					If (kind <= BoundKind.Conversion) Then
						If (kind = BoundKind.Parenthesized) Then
							value = DirectCast(value, BoundParenthesized).Expression
						ElseIf (kind = BoundKind.Conversion) Then
							value = DirectCast(value, BoundConversion).Operand
						Else
							Exit While
						End If
					ElseIf (kind = BoundKind.[DirectCast]) Then
						value = DirectCast(value, BoundDirectCast).Operand
					ElseIf (kind = BoundKind.[TryCast]) Then
						value = DirectCast(value, BoundTryCast).Operand
					Else
						If (kind <> BoundKind.Lambda) Then
							Exit While
						End If
						flag = True
						Return flag
					End If
				End While
				flag = False
			Else
				flag = False
			End If
			Return flag
		End Function

		Protected Overridable Function IsEmptyStructType(ByVal type As TypeSymbol) As Boolean
			Dim flag As Boolean
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = TryCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
			If (namedTypeSymbol Is Nothing) Then
				flag = False
			ElseIf (DataFlowPass.IsTrackableStructType(namedTypeSymbol)) Then
				Dim flag1 As Boolean = False
				If (Not Me._isEmptyStructType.TryGetValue(namedTypeSymbol, flag1)) Then
					Me._isEmptyStructType(namedTypeSymbol) = True
					Dim enumerator As ImmutableArray(Of FieldSymbol).Enumerator = Me.GetStructInstanceFields(namedTypeSymbol).GetEnumerator()
					While enumerator.MoveNext()
						If (Me.IsEmptyStructType(enumerator.Current.Type)) Then
							Continue While
						End If
						Me._isEmptyStructType(namedTypeSymbol) = False
						flag = False
						Return flag
					End While
					Me._isEmptyStructType(namedTypeSymbol) = True
					flag = True
				Else
					flag = flag1
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function IsSlotAlreadyReported(ByVal symbolType As TypeSymbol, ByVal slot As Integer) As Boolean
			Dim flag As Boolean
			If (Me._alreadyReported(slot)) Then
				flag = True
			ElseIf (slot <= 1) Then
				flag = False
			ElseIf (DataFlowPass.IsTrackableStructType(symbolType)) Then
				Dim enumerator As ImmutableArray(Of FieldSymbol).Enumerator = Me.GetStructInstanceFields(symbolType).GetEnumerator()
				While enumerator.MoveNext()
					Dim current As FieldSymbol = enumerator.Current
					Dim num As Integer = Me.VariableSlot(current, slot)
					If (num = -1) Then
						flag = False
						Return flag
					Else
						If (Me.IsSlotAlreadyReported(current.Type, num)) Then
							Continue While
						End If
						flag = False
						Return flag
					End If
				End While
				Me._alreadyReported(slot) = True
				flag = True
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Shared Function IsTrackableStructType(ByVal symbol As TypeSymbol) As Boolean
			Dim flag As Boolean
			If (Not AbstractFlowPass(Of DataFlowPass.LocalState).IsNonPrimitiveValueType(symbol)) Then
				flag = False
			Else
				Dim originalDefinition As NamedTypeSymbol = TryCast(symbol.OriginalDefinition, NamedTypeSymbol)
				flag = If(originalDefinition Is Nothing, False, Not originalDefinition.KnownCircularStruct)
			End If
			Return flag
		End Function

		Private Sub LeaveParameter(ByVal parameter As ParameterSymbol)
			If (parameter.IsByRef) Then
				Dim num As Integer = Me.VariableSlot(parameter, 0)
				If (Not Me.State.IsAssigned(num)) Then
					Me.ReportUnassignedByRefParameter(parameter)
				End If
				Me.NoteRead(parameter)
			End If
		End Sub

		Private Sub LeaveParameters(ByVal parameters As ImmutableArray(Of ParameterSymbol))
			If (Me.State.Reachable) Then
				Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = parameters.GetEnumerator()
				While enumerator.MoveNext()
					Me.LeaveParameter(enumerator.Current)
				End While
			End If
		End Sub

		Protected Function MakeSlotsForExpression(ByVal node As BoundExpression) As DataFlowPass.SlotCollection
			Dim slotCollection As DataFlowPass.SlotCollection
			Dim orCreateSlot As DataFlowPass.SlotCollection = New DataFlowPass.SlotCollection()
			Dim kind As BoundKind = node.Kind
			If (kind > BoundKind.FieldAccess) Then
				Select Case kind
					Case BoundKind.MeReference
					Case BoundKind.MyBaseReference
					Case BoundKind.MyClassReference
						If (Me.MeParameter Is Nothing) Then
							Exit Select
						End If
						orCreateSlot.Append(Me.GetOrCreateSlot(Me.MeParameter, 0))
						Exit Select
					Case BoundKind.ValueTypeMeReference
					Case BoundKind.PreviousSubmissionReference
					Case BoundKind.HostObjectMemberReference
					Case BoundKind.PseudoVariable
						Exit Select
					Case BoundKind.Local
						Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = DirectCast(node, BoundLocal).LocalSymbol
						If (localSymbol.DeclarationKind <> LocalDeclarationKind.AmbiguousLocals) Then
							orCreateSlot.Append(Me.GetOrCreateSlot(localSymbol, 0))
							Exit Select
						Else
							Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol).Enumerator = DirectCast(localSymbol, DataFlowPass.AmbiguousLocalsPseudoSymbol).Locals.GetEnumerator()
							While enumerator.MoveNext()
								orCreateSlot.Append(Me.GetOrCreateSlot(enumerator.Current, 0))
							End While
							Exit Select
						End If
					Case BoundKind.Parameter
						orCreateSlot.Append(Me.GetOrCreateSlot(DirectCast(node, BoundParameter).ParameterSymbol, 0))
						Exit Select
					Case Else
						If (kind = BoundKind.RangeVariable) Then
							orCreateSlot.Append(Me.GetOrCreateSlot(DirectCast(node, BoundRangeVariable).RangeVariable, 0))
							Exit Select
						Else
							Exit Select
						End If
				End Select
			ElseIf (CByte(kind) - CByte(BoundKind.WithLValueExpressionPlaceholder) <= CByte(BoundKind.OmittedArgument)) Then
				Dim getPlaceholderSubstitute As BoundExpression = MyBase.GetPlaceholderSubstitute(DirectCast(node, BoundValuePlaceholderBase))
				If (getPlaceholderSubstitute Is Nothing) Then
					slotCollection = orCreateSlot
					Return slotCollection
				End If
				slotCollection = Me.MakeSlotsForExpression(getPlaceholderSubstitute)
				Return slotCollection
			ElseIf (kind = BoundKind.FieldAccess) Then
				Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess)
				Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = boundFieldAccess.FieldSymbol
				Dim receiverOpt As BoundExpression = boundFieldAccess.ReceiverOpt
				If (Not fieldSymbol.IsShared AndAlso receiverOpt IsNot Nothing AndAlso receiverOpt.Kind <> BoundKind.TypeExpression AndAlso receiverOpt.Type IsNot Nothing AndAlso receiverOpt.Type.TypeKind = Microsoft.CodeAnalysis.TypeKind.Struct) Then
					orCreateSlot = Me.MakeSlotsForExpression(receiverOpt)
					Dim count As Integer = orCreateSlot.Count - 1
					For i As Integer = 0 To count
						orCreateSlot(i) = Me.GetOrCreateSlot(boundFieldAccess.FieldSymbol, orCreateSlot(0))
					Next

				End If
			End If
			slotCollection = orCreateSlot
			Return slotCollection
		End Function

		Private Sub MarkSlotAsReported(ByVal symbolType As TypeSymbol, ByVal slot As Integer)
			If (Not Me._alreadyReported(slot)) Then
				Me._alreadyReported(slot) = True
				If (slot > 1 AndAlso DataFlowPass.IsTrackableStructType(symbolType)) Then
					Dim enumerator As ImmutableArray(Of FieldSymbol).Enumerator = Me.GetStructInstanceFields(symbolType).GetEnumerator()
					While enumerator.MoveNext()
						Dim current As FieldSymbol = enumerator.Current
						Dim num As Integer = Me.VariableSlot(current, slot)
						If (num = -1) Then
							Continue While
						End If
						Me.MarkSlotAsReported(current.Type, num)
					End While
				End If
			End If
		End Sub

		Protected Sub Normalize(ByRef _state As DataFlowPass.LocalState)
			Dim capacity As Integer = _state.Assigned.Capacity
			_state.Assigned.EnsureCapacity(Me.nextVariableSlot)
			Dim num As Integer = Me.nextVariableSlot - 1
			For i As Integer = capacity To num
				Dim variableIdentifier As DataFlowPass.VariableIdentifier = Me.variableBySlot(i)
				If (variableIdentifier.ContainingSlot >= 2 AndAlso _state.Assigned(variableIdentifier.ContainingSlot)) Then
					_state.Assign(i)
				End If
			Next

		End Sub

		Protected Overridable Sub NoteRead(ByVal variable As Microsoft.CodeAnalysis.VisualBasic.Symbol)
			If (variable IsNot Nothing AndAlso variable.Kind = SymbolKind.Local) Then
				Me._unusedVariables.Remove(DirectCast(variable, LocalSymbol))
			End If
		End Sub

		Protected Overridable Sub NoteRead(ByVal fieldAccess As BoundFieldAccess)
			Dim nodeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me.GetNodeSymbol(fieldAccess)
			If (nodeSymbol IsNot Nothing) Then
				If (nodeSymbol.Kind = SymbolKind.Local AndAlso DirectCast(nodeSymbol, LocalSymbol).DeclarationKind = LocalDeclarationKind.AmbiguousLocals) Then
					Dim enumerator As ImmutableArray(Of LocalSymbol).Enumerator = DirectCast(nodeSymbol, DataFlowPass.AmbiguousLocalsPseudoSymbol).Locals.GetEnumerator()
					While enumerator.MoveNext()
						Me.NoteRead(enumerator.Current)
					End While
					Return
				End If
				Me.NoteRead(nodeSymbol)
			End If
		End Sub

		Protected Overridable Sub NoteWrite(ByVal variable As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal value As BoundExpression)
			If (variable IsNot Nothing) Then
				Me._writtenVariables.Add(variable)
				Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = TryCast(variable, Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
				If (value IsNot Nothing AndAlso (localSymbol IsNot Nothing AndAlso Not localSymbol.IsConst AndAlso localSymbol.Type.IsReferenceType OrElse value.HasErrors)) Then
					Me._unusedVariables.Remove(localSymbol)
				End If
			End If
		End Sub

		Protected Overridable Sub NoteWrite(ByVal node As BoundExpression, ByVal value As BoundExpression)
			Dim nodeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me.GetNodeSymbol(node)
			If (nodeSymbol IsNot Nothing) Then
				If (nodeSymbol.Kind = SymbolKind.Local AndAlso DirectCast(nodeSymbol, LocalSymbol).DeclarationKind = LocalDeclarationKind.AmbiguousLocals) Then
					Dim enumerator As ImmutableArray(Of LocalSymbol).Enumerator = DirectCast(nodeSymbol, DataFlowPass.AmbiguousLocalsPseudoSymbol).Locals.GetEnumerator()
					While enumerator.MoveNext()
						Me.NoteWrite(enumerator.Current, value)
					End While
					Return
				End If
				Me.NoteWrite(nodeSymbol, value)
			End If
		End Sub

		Protected Overrides Function ReachableState() As DataFlowPass.LocalState
			Return New DataFlowPass.LocalState(BitVector.Empty)
		End Function

		Protected Overridable Sub ReportUnassigned(ByVal sym As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal node As SyntaxNode, ByVal rwContext As AbstractFlowPass(Of DataFlowPass.LocalState).ReadWriteContext, Optional ByVal slot As Integer = -1, Optional ByVal boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = Nothing)
			Dim type As TypeSymbol
			Dim flag As Boolean
			Dim isStatic As Boolean
			If (slot < 2) Then
				slot = Me.VariableSlot(sym, 0)
			End If
			If (slot >= Me._alreadyReported.Capacity) Then
				Me._alreadyReported.EnsureCapacity(Me.nextVariableSlot)
			End If
			If (sym.Kind <> SymbolKind.Parameter AndAlso sym.Kind <> SymbolKind.RangeVariable) Then
				Dim isImplicitlyDeclared As Boolean = False
				If (sym.Kind <> SymbolKind.Local) Then
					flag = False
					isStatic = False
					type = DirectCast(sym, FieldSymbol).Type
				Else
					Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = DirectCast(sym, Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
					type = localSymbol.Type
					flag = If(Not localSymbol.IsFunctionValue, False, Me.EnableBreakingFlowAnalysisFeatures)
					isStatic = localSymbol.IsStatic
					isImplicitlyDeclared = localSymbol.IsImplicitlyDeclared
				End If
				If (Not Me.IsSlotAlreadyReported(type, slot)) Then
					Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None
					Dim unassignedSymbolFirstLocation As Location = Me.GetUnassignedSymbolFirstLocation(sym, boundFieldAccess)
					If (isImplicitlyDeclared OrElse unassignedSymbolFirstLocation Is Nothing OrElse unassignedSymbolFirstLocation.SourceSpan.Start < node.SpanStart) Then
						If (type IsNot Nothing AndAlso Not isStatic) Then
							If (Not type.IsIntrinsicValueType() AndAlso Not type.IsReferenceType) Then
								If (type.IsValueType) Then
									eRRID = If(rwContext = AbstractFlowPass(Of LocalState).ReadWriteContext.ByRefArgument, Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgUseNullRefByRefStr, Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgUseNullRefStr)
								End If
							ElseIf (Not type.IsIntrinsicValueType() OrElse flag) Then
								eRRID = If(rwContext = AbstractFlowPass(Of LocalState).ReadWriteContext.ByRefArgument, Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgUseNullRefByRef, Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgUseNullRef)
							Else
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None
							End If
							If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
								Me.diagnostics.Add(eRRID, node.GetLocation(), New [Object]() { If(sym.Name, "dummy") })
							End If
						End If
						Me.MarkSlotAsReported(type, slot)
					End If
				End If
			End If
		End Sub

		Protected Overridable Sub ReportUnassignedByRefParameter(ByVal parameter As ParameterSymbol)
		End Sub

		Private Sub ReportUnassignedFunctionValue(ByVal local As LocalSymbol, ByVal node As SyntaxNode)
			If (Not Me._alreadyReported(1)) Then
				Dim returnType As TypeSymbol = Nothing
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = MyBase.MethodSymbol
				returnType = methodSymbol.ReturnType
				If (returnType IsNot Nothing AndAlso Not methodSymbol.IsIterator AndAlso (Not methodSymbol.IsAsync OrElse Not returnType.Equals(Me.compilation.GetWellKnownType(WellKnownType.System_Threading_Tasks_Task)))) Then
					Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None
					Dim functionLocalName As String = DataFlowPass.GetFunctionLocalName(methodSymbol, local)
					returnType = returnType.GetEnumUnderlyingTypeOrSelf()
					If (returnType.IsIntrinsicValueType()) Then
						Dim methodKind As Microsoft.CodeAnalysis.MethodKind = MyBase.MethodSymbol.MethodKind
						If (methodKind = Microsoft.CodeAnalysis.MethodKind.Conversion OrElse methodKind = Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator) Then
							eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValOpVal1
						Else
							eRRID = If(methodKind = Microsoft.CodeAnalysis.MethodKind.PropertyGet, Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValPropVal1, Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValFuncVal1)
						End If
					ElseIf (returnType.IsReferenceType) Then
						Dim methodKind1 As Microsoft.CodeAnalysis.MethodKind = MyBase.MethodSymbol.MethodKind
						If (methodKind1 = Microsoft.CodeAnalysis.MethodKind.Conversion OrElse methodKind1 = Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator) Then
							eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValOpRef1
						Else
							eRRID = If(methodKind1 = Microsoft.CodeAnalysis.MethodKind.PropertyGet, Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValPropRef1, Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValFuncRef1)
						End If
					ElseIf (returnType.TypeKind <> Microsoft.CodeAnalysis.TypeKind.TypeParameter) Then
						Dim methodKind2 As Microsoft.CodeAnalysis.MethodKind = MyBase.MethodSymbol.MethodKind
						If (methodKind2 <= Microsoft.CodeAnalysis.MethodKind.EventAdd) Then
							If (methodKind2 <> Microsoft.CodeAnalysis.MethodKind.Conversion) Then
								If (methodKind2 <> Microsoft.CodeAnalysis.MethodKind.EventAdd) Then
									GoTo Label1
								End If
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValWinRtEventVal1
								functionLocalName = MyBase.MethodSymbol.AssociatedSymbol.Name
								GoTo Label0
							Else
								GoTo Label0
							End If
						ElseIf (methodKind2 <> Microsoft.CodeAnalysis.MethodKind.UserDefinedOperator) Then
							If (methodKind2 <> Microsoft.CodeAnalysis.MethodKind.PropertyGet) Then
								GoTo Label1
							End If
							eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValPropRef1
							GoTo Label0
						Else
							GoTo Label0
						End If
					Label1:
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.WRN_DefAsgNoRetValFuncRef1
					End If
				Label0:
					If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
						Me.diagnostics.Add(eRRID, node.GetLocation(), New [Object]() { functionLocalName })
					End If
				End If
				Me._alreadyReported(1) = True
			End If
		End Sub

		Private Sub ReportUnused(ByVal local As LocalSymbol)
			If (Not local.IsFunctionValue AndAlso Not [String].IsNullOrEmpty(local.Name)) Then
				If (Not Me._writtenVariables.Contains(local)) Then
					Me.diagnostics.Add(ERRID.WRN_UnusedLocal, local.Locations(0), New [Object]() { If(local.Name, "dummy") })
				ElseIf (local.IsConst) Then
					Me.diagnostics.Add(ERRID.WRN_UnusedLocalConst, local.Locations(0), New [Object]() { If(local.Name, "dummy") })
					Return
				End If
			End If
		End Sub

		Protected Overrides Function Scan() As Boolean
			Dim flag As Boolean
			Dim enumerator As HashSet(Of LocalSymbol).Enumerator = New HashSet(Of LocalSymbol).Enumerator()
			If (MyBase.Scan()) Then
				If (Not Me._seenOnErrorOrResume) Then
					Try
						enumerator = Me._unusedVariables.GetEnumerator()
						While enumerator.MoveNext()
							Me.ReportUnused(enumerator.Current)
						End While
					Finally
						DirectCast(enumerator, IDisposable).Dispose()
					End Try
				End If
				If (MyBase.ShouldAnalyzeByRefParameters) Then
					Me.LeaveParameters(MyBase.MethodParameters)
				End If
				Dim meParameter As ParameterSymbol = Me.MeParameter
				If (meParameter IsNot Nothing) Then
					Me.LeaveParameter(meParameter)
				End If
				flag = True
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Sub SetSlotAssigned(ByVal slot As Integer, ByRef state As DataFlowPass.LocalState)
			Dim variableIdentifier As DataFlowPass.VariableIdentifier = Me.variableBySlot(slot)
			Dim variableType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = DataFlowPass.GetVariableType(variableIdentifier.Symbol)
			If (slot >= Me.State.Assigned.Capacity) Then
				Me.Normalize(Me.State)
			End If
			If (Not state.IsAssigned(slot)) Then
				state.Assign(slot)
				If (DataFlowPass.IsTrackableStructType(variableType)) Then
					Dim enumerator As ImmutableArray(Of FieldSymbol).Enumerator = Me.GetStructInstanceFields(variableType).GetEnumerator()
					While enumerator.MoveNext()
						Dim num As Integer = Me.VariableSlot(enumerator.Current, slot)
						If (num < 2) Then
							Continue While
						End If
						Me.SetSlotAssigned(num, state)
					End While
				End If
				While variableIdentifier.ContainingSlot > 0
					Dim containingSlot As Integer = variableIdentifier.ContainingSlot
					Dim variableIdentifier1 As DataFlowPass.VariableIdentifier = Me.variableBySlot(containingSlot)
					Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = variableIdentifier1.Symbol
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = DataFlowPass.GetVariableType(symbol)
					Dim enumerator1 As ImmutableArray(Of FieldSymbol).Enumerator = Me.GetStructInstanceFields(typeSymbol).GetEnumerator()
					While enumerator1.MoveNext()
						Dim orCreateSlot As Integer = Me.GetOrCreateSlot(enumerator1.Current, containingSlot)
						If (orCreateSlot <> -1 AndAlso Not state.IsAssigned(orCreateSlot)) Then
							Return
						End If
					End While
					state.Assign(containingSlot)
					If (symbol.Kind = SymbolKind.Local AndAlso DirectCast(symbol, LocalSymbol).IsFunctionValue) Then
						state.Assign(1)
					End If
					variableIdentifier = variableIdentifier1
				End While
			End If
		End Sub

		Private Sub SetSlotAssigned(ByVal slot As Integer)
			Me.SetSlotAssigned(slot, Me.State)
		End Sub

		Protected Sub SetSlotState(ByVal slot As Integer, ByVal assigned As Boolean)
			If (slot <> -1) Then
				If (slot = 1) Then
					If (assigned) Then
						Me.State.Assign(slot)
						Return
					End If
					Me.State.Unassign(slot)
					Return
				End If
				If (assigned) Then
					Me.SetSlotAssigned(slot)
					Return
				End If
				Me.SetSlotUnassigned(slot)
			End If
		End Sub

		Private Sub SetSlotUnassigned(ByVal slot As Integer)
			If (slot >= Me.State.Assigned.Capacity) Then
				Me.Normalize(Me.State)
			End If
			If (Me._tryState.HasValue) Then
				Dim value As DataFlowPass.LocalState = Me._tryState.Value
				Me.SetSlotUnassigned(slot, value)
				Me._tryState = New Nullable(Of DataFlowPass.LocalState)(value)
			End If
			Me.SetSlotUnassigned(slot, Me.State)
		End Sub

		Private Sub SetSlotUnassigned(ByVal slot As Integer, ByRef state As DataFlowPass.LocalState)
			Dim variableIdentifier As DataFlowPass.VariableIdentifier = Me.variableBySlot(slot)
			Dim variableType As TypeSymbol = DataFlowPass.GetVariableType(variableIdentifier.Symbol)
			state.Unassign(slot)
			If (DataFlowPass.IsTrackableStructType(variableType)) Then
				Dim enumerator As ImmutableArray(Of FieldSymbol).Enumerator = Me.GetStructInstanceFields(variableType).GetEnumerator()
				While enumerator.MoveNext()
					Dim num As Integer = Me.VariableSlot(enumerator.Current, slot)
					If (num < 2) Then
						Continue While
					End If
					Me.SetSlotUnassigned(num, state)
				End While
			End If
			While variableIdentifier.ContainingSlot > 0
				Dim containingSlot As Integer = variableIdentifier.ContainingSlot
				Dim variableIdentifier1 As DataFlowPass.VariableIdentifier = Me.variableBySlot(containingSlot)
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = variableIdentifier1.Symbol
				If (Not state.IsAssigned(containingSlot)) Then
					Exit While
				End If
				state.Unassign(containingSlot)
				If (symbol.Kind = SymbolKind.Local AndAlso DirectCast(symbol, LocalSymbol).IsFunctionValue) Then
					state.Unassign(1)
				End If
				variableIdentifier = variableIdentifier1
			End While
		End Sub

		Private Function ShouldIgnoreStructField(ByVal field As FieldSymbol) As Boolean
			Dim flag As Boolean
			If (field.IsShared) Then
				flag = True
			ElseIf (Not Me._trackStructsWithIntrinsicTypedFields) Then
				Dim type As TypeSymbol = field.Type
				If (type.IsIntrinsicValueType()) Then
					flag = True
				ElseIf (type.IsTypeParameter() AndAlso Not DirectCast(type, TypeParameterSymbol).IsReferenceType) Then
					flag = True
				ElseIf (field.DeclaredAccessibility = Accessibility.[Private]) Then
					If (field.Dangerous_IsFromSomeCompilationIncludingRetargeting) Then
						Dim associatedSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = field.AssociatedSymbol
						If (associatedSymbol IsNot Nothing AndAlso associatedSymbol.Kind = SymbolKind.[Event]) Then
							GoTo Label1
						End If
						flag = False
						Return flag
					End If
				Label1:
					flag = If(Not field.ContainingType.IsGenericType, True, False)
				Else
					flag = False
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Protected Overridable Function TreatTheLocalAsAssignedWithinTheLambda(ByVal local As LocalSymbol, ByVal right As BoundExpression) As Boolean
			Return DataFlowPass.IsConvertedLambda(right)
		End Function

		Private Shared Sub UnionBitArrays(ByRef receiver As BitVector, ByVal other As BitVector)
			If (Not receiver(0)) Then
				If (other(0)) Then
					receiver = DataFlowPass.UnreachableBitsSet
					Return
				End If
				receiver.UnionWith(other)
			End If
		End Sub

		Protected Overrides Sub UnionWith(ByRef self As DataFlowPass.LocalState, ByRef other As DataFlowPass.LocalState)
			If (self.Assigned.Capacity <> other.Assigned.Capacity) Then
				Me.Normalize(self)
				Me.Normalize(other)
			End If
			Dim num As Integer = 0
			Do
				If (other.Assigned(num)) Then
					self.Assigned(num) = True
				End If
				num = num + 1
			Loop While num <= 1
			Dim capacity As Integer = self.Assigned.Capacity - 1
			For i As Integer = 2 To capacity
				If (other.Assigned(i) AndAlso Not self.Assigned(i)) Then
					Me.SetSlotAssigned(i, self)
				End If
			Next

		End Sub

		Protected Overrides Function UnreachableState() As DataFlowPass.LocalState
			Return New DataFlowPass.LocalState(DataFlowPass.UnreachableBitsSet)
		End Function

		Protected Function VariableSlot(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, Optional ByVal containingSlot As Integer = 0) As Integer
			Dim num As Integer
			Dim num1 As Integer
			containingSlot = Me.DescendThroughTupleRestFields(symbol, containingSlot, False)
			If (symbol IsNot Nothing) Then
				num = If(Me._variableSlot.TryGetValue(New DataFlowPass.VariableIdentifier(symbol, containingSlot), num1), num1, -1)
			Else
				num = -1
			End If
			Return num
		End Function

		Public Overrides Function VisitAggregateClause(ByVal node As BoundAggregateClause) As BoundNode
			If (node.CapturedGroupOpt IsNot Nothing) Then
				MyBase.VisitRvalue(node.CapturedGroupOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			MyBase.VisitRvalue(node.UnderlyingExpression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Protected Overridable Sub VisitAmbiguousLocalSymbol(ByVal ambiguous As DataFlowPass.AmbiguousLocalsPseudoSymbol)
		End Sub

		Public Overrides Function VisitAnonymousTypeCreationExpression(ByVal node As BoundAnonymousTypeCreationExpression) As BoundNode
			MyBase.VisitAnonymousTypeCreationExpression(node)
			Return Nothing
		End Function

		Protected NotOverridable Overrides Sub VisitArgument(ByVal arg As BoundExpression, ByVal p As ParameterSymbol)
			If (Not p.IsByRef) Then
				MyBase.VisitRvalue(arg, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
				Return
			End If
			If (p.IsOut And Not Me.IgnoreOutSemantics) Then
				Me.VisitLvalue(arg, False)
				Return
			End If
			MyBase.VisitRvalue(arg, AbstractFlowPass(Of LocalState).ReadWriteContext.ByRefArgument, False)
		End Sub

		Public Overrides Function VisitAsNewLocalDeclarations(ByVal node As BoundAsNewLocalDeclarations) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundValuePlaceholderBase As Microsoft.CodeAnalysis.VisualBasic.BoundValuePlaceholderBase = Nothing
			Dim flag As Boolean = Me.DeclaredVariableIsAlwaysAssignedBeforeInitializer(node.Syntax, node.Initializer, boundValuePlaceholderBase)
			Dim localDeclarations As ImmutableArray(Of BoundLocalDeclaration) = node.LocalDeclarations
			If (Not localDeclarations.IsEmpty) Then
				Dim enumerator As ImmutableArray(Of BoundLocalDeclaration).Enumerator = localDeclarations.GetEnumerator()
				While enumerator.MoveNext()
					Me.GetOrCreateSlot(enumerator.Current.LocalSymbol, 0)
				End While
				Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Me.CreateLocalSymbolForVariables(localDeclarations)
				Dim item As BoundLocalDeclaration = localDeclarations(0)
				Me.Assign(item, node.Initializer, If(Me.ConsiderLocalInitiallyAssigned(item.LocalSymbol), True, flag))
				If (flag) Then
					MyBase.SetPlaceholderSubstitute(boundValuePlaceholderBase, New BoundLocal(item.Syntax, localSymbol, localSymbol.Type))
				End If
				MyBase.VisitRvalue(node.Initializer, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
				MyBase.Visit(item)
				Dim length As Integer = localDeclarations.Length - 1
				Dim num As Integer = 1
				Do
					item = localDeclarations(num)
					Me.Assign(item, node.Initializer, If(Me.ConsiderLocalInitiallyAssigned(item.LocalSymbol), True, flag))
					MyBase.Visit(item)
					num = num + 1
				Loop While num <= length
				If (flag) Then
					MyBase.RemovePlaceholderSubstitute(boundValuePlaceholderBase)
				End If
				boundNode = Nothing
			Else
				boundNode = Nothing
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitAssignmentOperator(ByVal node As BoundAssignmentOperator) As BoundNode
			Dim left As BoundExpression = node.Left
			Dim flag As Boolean = False
			If (left.Kind = BoundKind.Local) Then
				flag = Me.TreatTheLocalAsAssignedWithinTheLambda(DirectCast(left, BoundLocal).LocalSymbol, node.Right)
			End If
			If (flag) Then
				Me.Assign(left, node.Right, True)
			End If
			MyBase.VisitAssignmentOperator(node)
			If (Not flag) Then
				Me.Assign(left, node.Right, True)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitBlock(ByVal node As BoundBlock) As BoundNode
			Dim enumerator As ImmutableArray(Of LocalSymbol).Enumerator = node.Locals.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As LocalSymbol = enumerator.Current
				If (Not current.IsImplicitlyDeclared) Then
					Continue While
				End If
				Me.SetSlotState(Me.GetOrCreateSlot(current, 0), Me.ConsiderLocalInitiallyAssigned(current))
			End While
			Return MyBase.VisitBlock(node)
		End Function

		Protected Overrides Sub VisitCatchBlock(ByVal catchBlock As BoundCatchBlock, ByRef finallyState As DataFlowPass.LocalState)
			If (Not Me.TrackUnassignments) Then
				Me.VisitCatchBlockInternal(catchBlock, finallyState)
				Return
			End If
			Dim nullable As Nullable(Of DataFlowPass.LocalState) = Me._tryState
			Me._tryState = New Nullable(Of DataFlowPass.LocalState)(Me.AllBitsSet())
			Me.VisitCatchBlockInternal(catchBlock, finallyState)
			Dim value As DataFlowPass.LocalState = Me._tryState.Value
			Me.IntersectWith(finallyState, value)
			If (Not nullable.HasValue) Then
				Me._tryState = nullable
				Return
			End If
			Dim localState As DataFlowPass.LocalState = Me._tryState.Value
			value = nullable.Value
			Me.IntersectWith(localState, value)
			Me._tryState = New Nullable(Of DataFlowPass.LocalState)(localState)
		End Sub

		Private Sub VisitCatchBlockInternal(ByVal catchBlock As BoundCatchBlock, ByRef finallyState As DataFlowPass.LocalState)
			Dim localOpt As LocalSymbol = catchBlock.LocalOpt
			If (localOpt IsNot Nothing) Then
				Me.GetOrCreateSlot(localOpt, 0)
			End If
			Dim exceptionSourceOpt As BoundExpression = catchBlock.ExceptionSourceOpt
			If (exceptionSourceOpt IsNot Nothing) Then
				Me.Assign(exceptionSourceOpt, Nothing, True)
			End If
			MyBase.VisitCatchBlock(catchBlock, finallyState)
		End Sub

		Public Overrides Function VisitFieldAccess(ByVal node As BoundFieldAccess) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = MyBase.VisitFieldAccess(node)
			If (AbstractFlowPass(Of DataFlowPass.LocalState).FieldAccessMayRequireTracking(node)) Then
				Me.CheckAssigned(node, node.Syntax, AbstractFlowPass(Of LocalState).ReadWriteContext.None)
			End If
			Return boundNode
		End Function

		Protected Overrides Sub VisitFieldAccessInReadWriteContext(ByVal node As BoundFieldAccess, ByVal rwContext As AbstractFlowPass(Of DataFlowPass.LocalState).ReadWriteContext)
			MyBase.VisitFieldAccessInReadWriteContext(node, DirectCast(rwContext, AbstractFlowPass(Of LocalState).ReadWriteContext))
			If (AbstractFlowPass(Of DataFlowPass.LocalState).FieldAccessMayRequireTracking(node)) Then
				Me.CheckAssigned(node, node.Syntax, rwContext)
			End If
		End Sub

		Protected Overrides Sub VisitFinallyBlock(ByVal finallyBlock As BoundStatement, ByRef unsetInFinally As DataFlowPass.LocalState)
			If (Not Me.TrackUnassignments) Then
				MyBase.VisitFinallyBlock(finallyBlock, unsetInFinally)
				Return
			End If
			Dim nullable As Nullable(Of DataFlowPass.LocalState) = Me._tryState
			Me._tryState = New Nullable(Of DataFlowPass.LocalState)(Me.AllBitsSet())
			MyBase.VisitFinallyBlock(finallyBlock, unsetInFinally)
			Dim value As DataFlowPass.LocalState = Me._tryState.Value
			Me.IntersectWith(unsetInFinally, value)
			If (Not nullable.HasValue) Then
				Me._tryState = nullable
				Return
			End If
			Dim localState As DataFlowPass.LocalState = Me._tryState.Value
			value = nullable.Value
			Me.IntersectWith(localState, value)
			Me._tryState = New Nullable(Of DataFlowPass.LocalState)(localState)
		End Sub

		Protected Overrides Sub VisitForControlInitialization(ByVal node As BoundForToStatement)
			MyBase.VisitForControlInitialization(node)
			Me.Assign(node.ControlVariable, node.InitialValue, True)
		End Sub

		Protected Overrides Sub VisitForControlInitialization(ByVal node As BoundForEachStatement)
			MyBase.VisitForControlInitialization(node)
			Me.Assign(node.ControlVariable, Nothing, True)
		End Sub

		Protected Overrides Sub VisitForStatementVariableDeclaration(ByVal node As BoundForStatement)
			If (node.DeclaredOrInferredLocalOpt IsNot Nothing) Then
				Me.GetOrCreateSlot(node.DeclaredOrInferredLocalOpt, 0)
				Me.Assign(node, Nothing, Me.ConsiderLocalInitiallyAssigned(node.DeclaredOrInferredLocalOpt))
			End If
		End Sub

		Public Overrides Function VisitLambda(ByVal node As BoundLambda) As BoundNode
			Dim savedPending As AbstractFlowPass(Of DataFlowPass.LocalState).SavedPending = MyBase.SavePending()
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me.symbol
			Me.symbol = node.LambdaSymbol
			Dim state As DataFlowPass.LocalState = Me.State
			MyBase.SetState(If(state.Reachable, state.Clone(), Me.AllBitsSet()))
			Me.State.Assigned(1) = False
			Dim item As Boolean = Me._alreadyReported(1)
			Me._alreadyReported(1) = False
			Me.EnterParameters(node.LambdaSymbol.Parameters)
			Me.VisitBlock(node.Body)
			Me.LeaveParameters(node.LambdaSymbol.Parameters)
			Me.symbol = symbol
			Me._alreadyReported(1) = item
			Me.State.Assigned(1) = True
			MyBase.RestorePending(savedPending, False)
			Me.IntersectWith(state, Me.State)
			MyBase.SetState(state)
			Return Nothing
		End Function

		Protected Overrides Sub VisitLateBoundArgument(ByVal arg As BoundExpression, ByVal isByRef As Boolean)
			If (isByRef) Then
				MyBase.VisitRvalue(arg, AbstractFlowPass(Of LocalState).ReadWriteContext.ByRefArgument, False)
				Return
			End If
			MyBase.VisitRvalue(arg, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
		End Sub

		Public Overrides Function VisitLocal(ByVal node As BoundLocal) As BoundNode
			Me.CheckAssigned(node.LocalSymbol, node.Syntax, AbstractFlowPass(Of LocalState).ReadWriteContext.None)
			Return Nothing
		End Function

		Public Overrides Function VisitLocalDeclaration(ByVal node As BoundLocalDeclaration) As BoundNode
			Dim boundValuePlaceholderBase As Microsoft.CodeAnalysis.VisualBasic.BoundValuePlaceholderBase = Nothing
			Dim flag As Boolean = Me.DeclaredVariableIsAlwaysAssignedBeforeInitializer(node.Syntax.Parent, node.InitializerOpt, boundValuePlaceholderBase)
			Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = node.LocalSymbol
			If (flag) Then
				MyBase.SetPlaceholderSubstitute(boundValuePlaceholderBase, New BoundLocal(node.Syntax, localSymbol, localSymbol.Type))
			End If
			Me.GetOrCreateSlot(localSymbol, 0)
			Me.Assign(node, node.InitializerOpt, If(Me.ConsiderLocalInitiallyAssigned(localSymbol) OrElse flag, True, Me.TreatTheLocalAsAssignedWithinTheLambda(localSymbol, node.InitializerOpt)))
			If (node.InitializerOpt IsNot Nothing OrElse node.InitializedByAsNew) Then
				MyBase.VisitLocalDeclaration(node)
			End If
			Me.AssignLocalOnDeclaration(localSymbol, node)
			If (flag) Then
				MyBase.RemovePlaceholderSubstitute(boundValuePlaceholderBase)
			End If
			Return Nothing
		End Function

		Protected Overrides Sub VisitLocalInReadWriteContext(ByVal node As BoundLocal, ByVal rwContext As AbstractFlowPass(Of DataFlowPass.LocalState).ReadWriteContext)
			MyBase.VisitLocalInReadWriteContext(node, DirectCast(rwContext, AbstractFlowPass(Of LocalState).ReadWriteContext))
			Me.CheckAssigned(node.LocalSymbol, node.Syntax, rwContext)
		End Sub

		Protected Overrides Sub VisitLvalue(ByVal node As BoundExpression, Optional ByVal dontLeaveRegion As Boolean = False)
			MyBase.VisitLvalue(node, True)
			If (node.Kind = BoundKind.Local) Then
				Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = DirectCast(node, BoundLocal).LocalSymbol
				If (localSymbol.DeclarationKind = LocalDeclarationKind.AmbiguousLocals) Then
					Me.VisitAmbiguousLocalSymbol(DirectCast(localSymbol, DataFlowPass.AmbiguousLocalsPseudoSymbol))
				End If
			End If
			If (Not dontLeaveRegion AndAlso node = Me._lastInRegion AndAlso MyBase.IsInside) Then
				Me.LeaveRegion()
			End If
		End Sub

		Public Overrides Function VisitMeReference(ByVal node As BoundMeReference) As BoundNode
			Me.CheckAssigned(Me.MeParameter, node.Syntax, AbstractFlowPass(Of LocalState).ReadWriteContext.None)
			Return Nothing
		End Function

		Public Overrides Function VisitMyBaseReference(ByVal node As BoundMyBaseReference) As BoundNode
			Me.CheckAssigned(Me.MeParameter, node.Syntax, AbstractFlowPass(Of LocalState).ReadWriteContext.None)
			Return Nothing
		End Function

		Public Overrides Function VisitMyClassReference(ByVal node As BoundMyClassReference) As BoundNode
			Me.CheckAssigned(Me.MeParameter, node.Syntax, AbstractFlowPass(Of LocalState).ReadWriteContext.None)
			Return Nothing
		End Function

		Protected Overrides Sub VisitObjectCreationExpressionInitializer(ByVal node As BoundObjectInitializerExpressionBase)
			Dim flag As Boolean
			flag = If(node Is Nothing OrElse node.Kind <> BoundKind.ObjectInitializerExpression, False, DirectCast(node, BoundObjectInitializerExpression).CreateTemporaryLocalForInitialization)
			If (flag) Then
				MyBase.SetPlaceholderSubstitute(DirectCast(node, BoundObjectInitializerExpression).PlaceholderOpt, Nothing)
			End If
			MyBase.VisitObjectCreationExpressionInitializer(node)
			If (flag) Then
				MyBase.RemovePlaceholderSubstitute(DirectCast(node, BoundObjectInitializerExpression).PlaceholderOpt)
			End If
		End Sub

		Public Overrides Function VisitOnErrorStatement(ByVal node As BoundOnErrorStatement) As BoundNode
			Me._seenOnErrorOrResume = True
			Return MyBase.VisitOnErrorStatement(node)
		End Function

		Public Overrides Function VisitOrdering(ByVal node As BoundOrdering) As BoundNode
			MyBase.VisitRvalue(node.UnderlyingExpression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitParameter(ByVal node As BoundParameter) As BoundNode
			If (Not node.WasCompilerGenerated) Then
				Me.CheckAssigned(node.ParameterSymbol, node.Syntax, AbstractFlowPass(Of LocalState).ReadWriteContext.None)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitQueryableSource(ByVal node As BoundQueryableSource) As BoundNode
			MyBase.VisitRvalue(node.Source, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitQueryClause(ByVal node As BoundQueryClause) As BoundNode
			MyBase.VisitRvalue(node.UnderlyingExpression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitQueryExpression(ByVal node As BoundQueryExpression) As BoundNode
			MyBase.VisitRvalue(node.LastOperator, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitQueryLambda(ByVal node As BoundQueryLambda) As BoundNode
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me.symbol
			Me.symbol = node.LambdaSymbol
			Dim localState As DataFlowPass.LocalState = Me.State.Clone()
			MyBase.VisitRvalue(node.Expression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Me.symbol = symbol
			Me.IntersectWith(localState, Me.State)
			MyBase.SetState(localState)
			Return Nothing
		End Function

		Public Overrides Function VisitQuerySource(ByVal node As BoundQuerySource) As BoundNode
			MyBase.VisitRvalue(node.Expression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitRangeVariable(ByVal node As BoundRangeVariable) As BoundNode
			If (Not node.WasCompilerGenerated) Then
				Me.CheckAssigned(node.RangeVariable, node.Syntax, AbstractFlowPass(Of LocalState).ReadWriteContext.None)
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitRangeVariableAssignment(ByVal node As BoundRangeVariableAssignment) As BoundNode
			MyBase.VisitRvalue(node.Value, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitRedimClause(ByVal node As BoundRedimClause) As BoundNode
			MyBase.VisitRedimClause(node)
			Me.Assign(node.Operand, Nothing, True)
			Return Nothing
		End Function

		Public Overrides Function VisitReferenceAssignment(ByVal node As BoundReferenceAssignment) As BoundNode
			MyBase.VisitReferenceAssignment(node)
			Me.Assign(node.ByRefLocal, node.LValue, True)
			Return Nothing
		End Function

		Public Overrides Function VisitResumeStatement(ByVal node As BoundResumeStatement) As BoundNode
			Me._seenOnErrorOrResume = True
			Return MyBase.VisitResumeStatement(node)
		End Function

		Public Overrides Function VisitReturnStatement(ByVal node As BoundReturnStatement) As BoundNode
			If (node.IsEndOfMethodReturn()) Then
				Dim expressionOpt As BoundLocal = TryCast(node.ExpressionOpt, BoundLocal)
				If (expressionOpt IsNot Nothing) Then
					Me.CheckAssignedFunctionValue(expressionOpt.LocalSymbol, node.Syntax)
				End If
			Else
				Me.SetSlotState(1, True)
			End If
			MyBase.VisitReturnStatement(node)
			Return Nothing
		End Function

		Public Overrides Function VisitToQueryableCollectionConversion(ByVal node As BoundToQueryableCollectionConversion) As BoundNode
			MyBase.VisitRvalue(node.ConversionCall, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Protected Overrides Sub VisitTryBlock(ByVal tryBlock As BoundStatement, ByVal node As BoundTryStatement, ByRef _tryState As DataFlowPass.LocalState)
			If (Not Me.TrackUnassignments) Then
				MyBase.VisitTryBlock(tryBlock, node, _tryState)
				Return
			End If
			Dim nullable As Nullable(Of DataFlowPass.LocalState) = Me._tryState
			Me._tryState = New Nullable(Of DataFlowPass.LocalState)(Me.AllBitsSet())
			MyBase.VisitTryBlock(tryBlock, node, _tryState)
			Dim value As DataFlowPass.LocalState = Me._tryState.Value
			Me.IntersectWith(_tryState, value)
			If (Not nullable.HasValue) Then
				Me._tryState = nullable
				Return
			End If
			Dim localState As DataFlowPass.LocalState = Me._tryState.Value
			value = nullable.Value
			Me.IntersectWith(localState, value)
			Me._tryState = New Nullable(Of DataFlowPass.LocalState)(localState)
		End Sub

		Public Overrides Function VisitUsingStatement(ByVal node As BoundUsingStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (Not node.ResourceList.IsDefaultOrEmpty) Then
				Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclarationBase).Enumerator = node.ResourceList.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclarationBase = enumerator.Current
					If (current.Kind <> BoundKind.AsNewLocalDeclarations) Then
						Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = DirectCast(current, BoundLocalDeclaration).LocalSymbol
						Dim orCreateSlot As Integer = Me.GetOrCreateSlot(localSymbol, 0)
						If (orCreateSlot < 0) Then
							Continue While
						End If
						Me.SetSlotAssigned(orCreateSlot)
						Me.NoteWrite(localSymbol, Nothing)
					Else
						Dim enumerator1 As ImmutableArray(Of BoundLocalDeclaration).Enumerator = DirectCast(current, BoundAsNewLocalDeclarations).LocalDeclarations.GetEnumerator()
						While enumerator1.MoveNext()
							Dim localSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = enumerator1.Current.LocalSymbol
							Dim num As Integer = Me.GetOrCreateSlot(localSymbol1, 0)
							If (num < 0) Then
								Continue While
							End If
							Me.SetSlotAssigned(num)
							Me.NoteWrite(localSymbol1, Nothing)
						End While
					End If
				End While
				Dim boundNode1 As Microsoft.CodeAnalysis.VisualBasic.BoundNode = MyBase.VisitUsingStatement(node)
				Dim enumerator2 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclarationBase).Enumerator = node.ResourceList.GetEnumerator()
				While enumerator2.MoveNext()
					Dim boundLocalDeclarationBase As Microsoft.CodeAnalysis.VisualBasic.BoundLocalDeclarationBase = enumerator2.Current
					If (boundLocalDeclarationBase.Kind <> BoundKind.AsNewLocalDeclarations) Then
						Me.NoteRead(DirectCast(boundLocalDeclarationBase, BoundLocalDeclaration).LocalSymbol)
					Else
						Dim enumerator3 As ImmutableArray(Of BoundLocalDeclaration).Enumerator = DirectCast(boundLocalDeclarationBase, BoundAsNewLocalDeclarations).LocalDeclarations.GetEnumerator()
						While enumerator3.MoveNext()
							Me.NoteRead(enumerator3.Current.LocalSymbol)
						End While
					End If
				End While
				boundNode = boundNode1
			Else
				boundNode = MyBase.VisitUsingStatement(node)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitWithStatement(ByVal node As BoundWithStatement) As BoundNode
			MyBase.SetPlaceholderSubstitute(node.ExpressionPlaceholder, node.DraftPlaceholderSubstitute)
			MyBase.VisitWithStatement(node)
			MyBase.RemovePlaceholderSubstitute(node.ExpressionPlaceholder)
			Return Nothing
		End Function

		Protected Overrides Sub WriteArgument(ByVal arg As BoundExpression, ByVal isOut As Boolean)
			If (Not isOut) Then
				Me.CheckAssignedFromArgumentWrite(arg, arg.Syntax)
			End If
			Me.Assign(arg, Nothing, True)
			MyBase.WriteArgument(arg, isOut)
		End Sub

		Protected NotInheritable Class AmbiguousLocalsPseudoSymbol
			Inherits LocalSymbol
			Public ReadOnly Locals As ImmutableArray(Of LocalSymbol)

			Friend Overrides ReadOnly Property DeclarationKind As LocalDeclarationKind
				Get
					Return LocalDeclarationKind.AmbiguousLocals
				End Get
			End Property

			Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
				Get
					Return ImmutableArray(Of SyntaxReference).Empty
				End Get
			End Property

			Friend Overrides ReadOnly Property IdentifierLocation As Location
				Get
					Return NoLocation.Singleton
				End Get
			End Property

			Friend Overrides ReadOnly Property IdentifierToken As SyntaxToken
				Get
					Return New SyntaxToken()
				End Get
			End Property

			Friend Overrides ReadOnly Property IsByRef As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsFunctionValue As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
				Get
					Return ImmutableArray(Of Location).Empty
				End Get
			End Property

			Public Overrides ReadOnly Property Name As String
				Get
					Return Nothing
				End Get
			End Property

			Friend Overrides ReadOnly Property SynthesizedKind As SynthesizedLocalKind
				Get
					Return SynthesizedLocalKind.UserDefined
				End Get
			End Property

			Private Sub New(ByVal container As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal type As TypeSymbol, ByVal locals As ImmutableArray(Of LocalSymbol))
				MyBase.New(container, type)
				Me.Locals = locals
			End Sub

			Friend Shared Function Create(ByVal locals As ImmutableArray(Of LocalSymbol)) As LocalSymbol
				Dim item As LocalSymbol = locals(0)
				Return New DataFlowPass.AmbiguousLocalsPseudoSymbol(item.ContainingSymbol, item.Type, locals)
			End Function

			Friend Overrides Function GetDeclaratorSyntax() As SyntaxNode
				Throw ExceptionUtilities.Unreachable
			End Function
		End Class

		Friend Structure LocalState
			Implements AbstractFlowPass(Of DataFlowPass.LocalState).AbstractLocalState
			Friend Assigned As BitVector

			Public ReadOnly Property FunctionAssignedValue As Boolean
				Get
					Return Me.IsAssigned(1)
				End Get
			End Property

			Public ReadOnly Property Reachable As Boolean
				Get
					If (Me.Assigned.Capacity <= 0) Then
						Return True
					End If
					Return Not Me.IsAssigned(0)
				End Get
			End Property

			Friend Sub New(ByVal assigned As BitVector)
				Me = New DataFlowPass.LocalState() With
				{
					.Assigned = assigned
				}
			End Sub

			Public Sub Assign(ByVal slot As Integer)
				If (slot <> -1) Then
					Me.Assigned(slot) = True
				End If
			End Sub

			Public Function Clone() As DataFlowPass.LocalState Implements AbstractFlowPass(Of DataFlowPass.LocalState).AbstractLocalState.Clone
				Return New DataFlowPass.LocalState(Me.Assigned.Clone())
			End Function

			Public Function IsAssigned(ByVal slot As Integer) As Boolean
				If (slot = -1 OrElse Me.Assigned(0)) Then
					Return True
				End If
				Return Me.Assigned(slot)
			End Function

			Public Sub Unassign(ByVal slot As Integer)
				If (slot <> -1) Then
					Me.Assigned(slot) = False
				End If
			End Sub
		End Structure

		Protected Structure SlotCollection
			Private _singleValue As Integer

			Private _builder As ArrayBuilder(Of Integer)

			Public ReadOnly Property Count As Integer
				Get
					Dim num As Integer
					If (Me._builder Is Nothing) Then
						num = If(Me._singleValue = 0, 0, 1)
					Else
						num = Me._builder.Count
					End If
					Return num
				End Get
			End Property

			Default Public Property Item(ByVal index As Integer) As Integer
				Get
					Dim num As Integer
					num = If(Me._builder Is Nothing, Me._singleValue, Me._builder(index))
					Return num
				End Get
				Set(ByVal value As Integer)
					If (Me._builder IsNot Nothing) Then
						Me._builder(index) = value
					End If
					Me._singleValue = value
				End Set
			End Property

			Public Sub Append(ByVal slot As Integer)
				If (Me._builder IsNot Nothing) Then
					Me._builder.Add(slot)
					Return
				End If
				If (Me._singleValue = 0) Then
					Me._singleValue = slot
					Return
				End If
				Me._builder = ArrayBuilder(Of Integer).GetInstance()
				Me._builder.Add(Me._singleValue)
				Me._builder.Add(slot)
			End Sub

			Public Sub Free()
				If (Me._builder IsNot Nothing) Then
					Me._builder.Free()
					Me._builder = Nothing
				End If
				Me._singleValue = 0
			End Sub
		End Structure

		Public Enum SlotKind
			NotTracked = -1
			Unreachable = 0
			FunctionValue = 1
			FirstAvailable = 2
		End Enum

		Protected Structure VariableIdentifier
			Implements IEquatable(Of DataFlowPass.VariableIdentifier)
			Public Shared None As DataFlowPass.VariableIdentifier

			Public ReadOnly Symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol

			Public ReadOnly ContainingSlot As Integer

			Public ReadOnly Property Exists As Boolean
				Get
					Return CObj(Me.Symbol) <> CObj(Nothing)
				End Get
			End Property

			Shared Sub New()
				DataFlowPass.VariableIdentifier.None = New DataFlowPass.VariableIdentifier()
			End Sub

			Public Sub New(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal containingSlot As Integer)
				Me = New DataFlowPass.VariableIdentifier() With
				{
					.Symbol = symbol,
					.ContainingSlot = containingSlot
				}
			End Sub

			Public Function ExplicitEquals(ByVal obj As DataFlowPass.VariableIdentifier) As Boolean Implements IEquatable(Of DataFlowPass.VariableIdentifier).Equals
				If (Not Me.Symbol.Equals(obj.Symbol)) Then
					Return False
				End If
				Return Me.ContainingSlot = obj.ContainingSlot
			End Function

			Public Overrides Function Equals(ByVal obj As Object) As Boolean Implements IEquatable(Of DataFlowPass.VariableIdentifier).Equals
				Return Me.ExplicitEquals(DirectCast(obj, Nullable(Of DataFlowPass.VariableIdentifier)).Value)
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return Hash.Combine(Me.Symbol.GetHashCode(), Me.ContainingSlot.GetHashCode())
			End Function

			Public Shared Operator =(ByVal left As DataFlowPass.VariableIdentifier, ByVal right As DataFlowPass.VariableIdentifier) As Boolean
				Return left.ExplicitEquals(right)
			End Operator

			Public Shared Operator <>(ByVal left As DataFlowPass.VariableIdentifier, ByVal right As DataFlowPass.VariableIdentifier) As Boolean
				Return Not left.ExplicitEquals(right)
			End Operator
		End Structure
	End Class
End Namespace