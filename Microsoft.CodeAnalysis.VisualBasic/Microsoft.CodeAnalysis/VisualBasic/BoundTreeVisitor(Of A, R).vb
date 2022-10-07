Imports System
Imports System.Diagnostics
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundTreeVisitor(Of A, R)
		Protected Sub New()
			MyBase.New()
		End Sub

		Public Overridable Function DefaultVisit(ByVal node As BoundNode, ByVal arg As A) As R
			Return Nothing
		End Function

		Public Overridable Function Visit(ByVal node As BoundNode, ByVal arg As A) As R
			Dim statement As R
			If (node IsNot Nothing) Then
				Dim kind As BoundKind = node.Kind
				If (kind > BoundKind.NamespaceExpression) Then
					If (kind <= BoundKind.ObjectCreationExpression) Then
						Select Case kind
							Case BoundKind.UnaryOperator
								statement = Me.VisitUnaryOperator(DirectCast(node, BoundUnaryOperator), arg)
								Return statement
							Case BoundKind.UserDefinedUnaryOperator
							Case BoundKind.NullableIsTrueOperator
							Case BoundKind.UserDefinedBinaryOperator
							Case BoundKind.UserDefinedShortCircuitingOperator
							Case BoundKind.CompoundAssignmentTargetPlaceholder
							Case BoundKind.ReferenceAssignment
							Case BoundKind.AddressOfOperator
							Case BoundKind.RelaxationLambda
							Case BoundKind.ConvertedTupleElements
							Case BoundKind.UserDefinedConversion
							Case BoundKind.SequencePointExpression
								Exit Select
							Case BoundKind.BinaryOperator
								statement = Me.VisitBinaryOperator(DirectCast(node, BoundBinaryOperator), arg)
								Return statement
							Case BoundKind.AssignmentOperator
								statement = Me.VisitAssignmentOperator(DirectCast(node, BoundAssignmentOperator), arg)
								Return statement
							Case BoundKind.TernaryConditionalExpression
								statement = Me.VisitTernaryConditionalExpression(DirectCast(node, BoundTernaryConditionalExpression), arg)
								Return statement
							Case BoundKind.BinaryConditionalExpression
								statement = Me.VisitBinaryConditionalExpression(DirectCast(node, BoundBinaryConditionalExpression), arg)
								Return statement
							Case BoundKind.Conversion
								statement = Me.VisitConversion(DirectCast(node, BoundConversion), arg)
								Return statement
							Case BoundKind.[DirectCast]
								statement = Me.VisitDirectCast(DirectCast(node, BoundDirectCast), arg)
								Return statement
							Case BoundKind.[TryCast]
								statement = Me.VisitTryCast(DirectCast(node, BoundTryCast), arg)
								Return statement
							Case BoundKind.[TypeOf]
								statement = Me.VisitTypeOf(DirectCast(node, BoundTypeOf), arg)
								Return statement
							Case BoundKind.SequencePoint
								statement = Me.VisitSequencePoint(DirectCast(node, BoundSequencePoint), arg)
								Return statement
							Case BoundKind.SequencePointWithSpan
								statement = Me.VisitSequencePointWithSpan(DirectCast(node, BoundSequencePointWithSpan), arg)
								Return statement
							Case BoundKind.NoOpStatement
								statement = Me.VisitNoOpStatement(DirectCast(node, BoundNoOpStatement), arg)
								Return statement
							Case BoundKind.MethodGroup
								statement = Me.VisitMethodGroup(DirectCast(node, BoundMethodGroup), arg)
								Return statement
							Case BoundKind.PropertyGroup
								statement = Me.VisitPropertyGroup(DirectCast(node, BoundPropertyGroup), arg)
								Return statement
							Case BoundKind.ReturnStatement
								statement = Me.VisitReturnStatement(DirectCast(node, BoundReturnStatement), arg)
								Return statement
							Case Else
								If (kind = BoundKind.[Call]) Then
									statement = Me.VisitCall(DirectCast(node, BoundCall), arg)
									Return statement
								ElseIf (kind = BoundKind.ObjectCreationExpression) Then
									statement = Me.VisitObjectCreationExpression(DirectCast(node, BoundObjectCreationExpression), arg)
									Return statement
								Else
									Exit Select
								End If
						End Select
					ElseIf (kind = BoundKind.DelegateCreationExpression) Then
						statement = Me.VisitDelegateCreationExpression(DirectCast(node, BoundDelegateCreationExpression), arg)
						Return statement
					Else
						Select Case kind
							Case BoundKind.FieldAccess
								statement = Me.VisitFieldAccess(DirectCast(node, BoundFieldAccess), arg)
								Return statement
							Case BoundKind.PropertyAccess
								statement = Me.VisitPropertyAccess(DirectCast(node, BoundPropertyAccess), arg)
								Return statement
							Case BoundKind.EventAccess
							Case BoundKind.StateMachineScope
							Case BoundKind.AsNewLocalDeclarations
							Case BoundKind.DimStatement
							Case BoundKind.Initializer
							Case BoundKind.ParameterEqualsValue
							Case BoundKind.GlobalStatementInitializer
								Exit Select
							Case BoundKind.Block
								statement = Me.VisitBlock(DirectCast(node, BoundBlock), arg)
								Return statement
							Case BoundKind.LocalDeclaration
								statement = Me.VisitLocalDeclaration(DirectCast(node, BoundLocalDeclaration), arg)
								Return statement
							Case BoundKind.FieldInitializer
								statement = Me.VisitFieldInitializer(DirectCast(node, BoundFieldInitializer), arg)
								Return statement
							Case BoundKind.PropertyInitializer
								statement = Me.VisitPropertyInitializer(DirectCast(node, BoundPropertyInitializer), arg)
								Return statement
							Case BoundKind.Sequence
								statement = Me.VisitSequence(DirectCast(node, BoundSequence), arg)
								Return statement
							Case BoundKind.ExpressionStatement
								statement = Me.VisitExpressionStatement(DirectCast(node, BoundExpressionStatement), arg)
								Return statement
							Case BoundKind.IfStatement
								statement = Me.VisitIfStatement(DirectCast(node, BoundIfStatement), arg)
								Return statement
							Case Else
								Select Case kind
									Case BoundKind.ForToStatement
										statement = Me.VisitForToStatement(DirectCast(node, BoundForToStatement), arg)
										Return statement
									Case BoundKind.ExitStatement
										statement = Me.VisitExitStatement(DirectCast(node, BoundExitStatement), arg)
										Return statement
									Case BoundKind.ContinueStatement
										statement = Me.VisitContinueStatement(DirectCast(node, BoundContinueStatement), arg)
										Return statement
									Case BoundKind.TryStatement
										statement = Me.VisitTryStatement(DirectCast(node, BoundTryStatement), arg)
										Return statement
									Case BoundKind.CatchBlock
										statement = Me.VisitCatchBlock(DirectCast(node, BoundCatchBlock), arg)
										Return statement
									Case BoundKind.Literal
										statement = Me.VisitLiteral(DirectCast(node, BoundLiteral), arg)
										Return statement
									Case BoundKind.MeReference
										statement = Me.VisitMeReference(DirectCast(node, BoundMeReference), arg)
										Return statement
									Case BoundKind.Local
										statement = Me.VisitLocal(DirectCast(node, BoundLocal), arg)
										Return statement
									Case BoundKind.Parameter
										statement = Me.VisitParameter(DirectCast(node, BoundParameter), arg)
										Return statement
									Case BoundKind.ByRefArgumentPlaceholder
										statement = Me.VisitByRefArgumentPlaceholder(DirectCast(node, BoundByRefArgumentPlaceholder), arg)
										Return statement
									Case BoundKind.ByRefArgumentWithCopyBack
										statement = Me.VisitByRefArgumentWithCopyBack(DirectCast(node, BoundByRefArgumentWithCopyBack), arg)
										Return statement
									Case BoundKind.LabelStatement
										statement = Me.VisitLabelStatement(DirectCast(node, BoundLabelStatement), arg)
										Return statement
									Case BoundKind.GotoStatement
										statement = Me.VisitGotoStatement(DirectCast(node, BoundGotoStatement), arg)
										Return statement
									Case BoundKind.StatementList
										statement = Me.VisitStatementList(DirectCast(node, BoundStatementList), arg)
										Return statement
									Case BoundKind.ConditionalGoto
										statement = Me.VisitConditionalGoto(DirectCast(node, BoundConditionalGoto), arg)
										Return statement
									Case BoundKind.Lambda
										statement = Me.VisitLambda(DirectCast(node, BoundLambda), arg)
										Return statement
								End Select

						End Select
					End If
				ElseIf (kind <= BoundKind.Parenthesized) Then
					If (kind = BoundKind.OmittedArgument) Then
						statement = Me.VisitOmittedArgument(DirectCast(node, BoundOmittedArgument), arg)
						Return statement
					Else
						If (kind <> BoundKind.Parenthesized) Then
							GoTo Label3
						End If
						statement = Me.VisitParenthesized(DirectCast(node, BoundParenthesized), arg)
						Return statement
					End If
				ElseIf (kind = BoundKind.ArrayAccess) Then
					statement = Me.VisitArrayAccess(DirectCast(node, BoundArrayAccess), arg)
					Return statement
				ElseIf (kind = BoundKind.TypeExpression) Then
					statement = Me.VisitTypeExpression(DirectCast(node, BoundTypeExpression), arg)
					Return statement
				Else
					If (kind <> BoundKind.NamespaceExpression) Then
						GoTo Label3
					End If
					statement = Me.VisitNamespaceExpression(DirectCast(node, BoundNamespaceExpression), arg)
					Return statement
				End If
			Label3:
				statement = Me.VisitInternal(node, arg)
			Else
				statement = Nothing
			End If
			Return statement
		End Function

		Public Overridable Function VisitAddHandlerStatement(ByVal node As BoundAddHandlerStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitAddressOfOperator(ByVal node As BoundAddressOfOperator, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitAggregateClause(ByVal node As BoundAggregateClause, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitAnonymousTypeCreationExpression(ByVal node As BoundAnonymousTypeCreationExpression, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitAnonymousTypeFieldInitializer(ByVal node As BoundAnonymousTypeFieldInitializer, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitAnonymousTypePropertyAccess(ByVal node As BoundAnonymousTypePropertyAccess, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitArrayAccess(ByVal node As BoundArrayAccess, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitArrayCreation(ByVal node As BoundArrayCreation, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitArrayInitialization(ByVal node As BoundArrayInitialization, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitArrayLength(ByVal node As BoundArrayLength, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitArrayLiteral(ByVal node As BoundArrayLiteral, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitAsNewLocalDeclarations(ByVal node As BoundAsNewLocalDeclarations, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitAssignmentOperator(ByVal node As BoundAssignmentOperator, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitAttribute(ByVal node As BoundAttribute, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitAwaitOperator(ByVal node As BoundAwaitOperator, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitBadExpression(ByVal node As BoundBadExpression, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitBadStatement(ByVal node As BoundBadStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitBadVariable(ByVal node As BoundBadVariable, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitBinaryConditionalExpression(ByVal node As BoundBinaryConditionalExpression, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitBinaryOperator(ByVal node As BoundBinaryOperator, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitBlock(ByVal node As BoundBlock, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitByRefArgumentPlaceholder(ByVal node As BoundByRefArgumentPlaceholder, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitByRefArgumentWithCopyBack(ByVal node As BoundByRefArgumentWithCopyBack, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitCall(ByVal node As BoundCall, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitCaseBlock(ByVal node As BoundCaseBlock, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitCaseStatement(ByVal node As BoundCaseStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitCatchBlock(ByVal node As BoundCatchBlock, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitCollectionInitializerExpression(ByVal node As BoundCollectionInitializerExpression, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitComplexConditionalAccessReceiver(ByVal node As BoundComplexConditionalAccessReceiver, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitCompoundAssignmentTargetPlaceholder(ByVal node As BoundCompoundAssignmentTargetPlaceholder, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitConditionalAccess(ByVal node As BoundConditionalAccess, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitConditionalAccessReceiverPlaceholder(ByVal node As BoundConditionalAccessReceiverPlaceholder, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitConditionalGoto(ByVal node As BoundConditionalGoto, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitContinueStatement(ByVal node As BoundContinueStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitConversion(ByVal node As BoundConversion, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitConvertedTupleElements(ByVal node As BoundConvertedTupleElements, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitConvertedTupleLiteral(ByVal node As BoundConvertedTupleLiteral, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitDelegateCreationExpression(ByVal node As BoundDelegateCreationExpression, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitDimStatement(ByVal node As BoundDimStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitDirectCast(ByVal node As BoundDirectCast, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitDoLoopStatement(ByVal node As BoundDoLoopStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitDup(ByVal node As BoundDup, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitEndStatement(ByVal node As BoundEndStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitEraseStatement(ByVal node As BoundEraseStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitEventAccess(ByVal node As BoundEventAccess, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitExitStatement(ByVal node As BoundExitStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitExpressionStatement(ByVal node As BoundExpressionStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitFieldAccess(ByVal node As BoundFieldAccess, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitFieldInfo(ByVal node As BoundFieldInfo, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitFieldInitializer(ByVal node As BoundFieldInitializer, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitForEachStatement(ByVal node As BoundForEachStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitForToStatement(ByVal node As BoundForToStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitForToUserDefinedOperators(ByVal node As BoundForToUserDefinedOperators, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitGetType(ByVal node As BoundGetType, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitGlobalStatementInitializer(ByVal node As BoundGlobalStatementInitializer, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitGotoStatement(ByVal node As BoundGotoStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitGroupAggregation(ByVal node As BoundGroupAggregation, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitGroupTypeInferenceLambda(ByVal node As GroupTypeInferenceLambda, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitHostObjectMemberReference(ByVal node As BoundHostObjectMemberReference, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitIfStatement(ByVal node As BoundIfStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitInitializer(ByVal node As BoundInitializer, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitInstrumentationPayloadRoot(ByVal node As BoundInstrumentationPayloadRoot, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		<DebuggerStepThrough>
		<MethodImpl(8)>
		Friend Function VisitInternal(ByVal node As BoundNode, ByVal arg As A) As R
			Dim rValueWrapper As R
			Select Case node.Kind
				Case BoundKind.TypeArguments
					rValueWrapper = Me.VisitTypeArguments(DirectCast(node, BoundTypeArguments), arg)
					Exit Select
				Case BoundKind.OmittedArgument
					rValueWrapper = Me.VisitOmittedArgument(DirectCast(node, BoundOmittedArgument), arg)
					Exit Select
				Case BoundKind.LValueToRValueWrapper
					rValueWrapper = Me.VisitLValueToRValueWrapper(DirectCast(node, BoundLValueToRValueWrapper), arg)
					Exit Select
				Case BoundKind.WithLValueExpressionPlaceholder
					rValueWrapper = Me.VisitWithLValueExpressionPlaceholder(DirectCast(node, BoundWithLValueExpressionPlaceholder), arg)
					Exit Select
				Case BoundKind.WithRValueExpressionPlaceholder
					rValueWrapper = Me.VisitWithRValueExpressionPlaceholder(DirectCast(node, BoundWithRValueExpressionPlaceholder), arg)
					Exit Select
				Case BoundKind.RValuePlaceholder
					rValueWrapper = Me.VisitRValuePlaceholder(DirectCast(node, BoundRValuePlaceholder), arg)
					Exit Select
				Case BoundKind.LValuePlaceholder
					rValueWrapper = Me.VisitLValuePlaceholder(DirectCast(node, BoundLValuePlaceholder), arg)
					Exit Select
				Case BoundKind.Dup
					rValueWrapper = Me.VisitDup(DirectCast(node, BoundDup), arg)
					Exit Select
				Case BoundKind.BadExpression
					rValueWrapper = Me.VisitBadExpression(DirectCast(node, BoundBadExpression), arg)
					Exit Select
				Case BoundKind.BadStatement
					rValueWrapper = Me.VisitBadStatement(DirectCast(node, BoundBadStatement), arg)
					Exit Select
				Case BoundKind.Parenthesized
					rValueWrapper = Me.VisitParenthesized(DirectCast(node, BoundParenthesized), arg)
					Exit Select
				Case BoundKind.BadVariable
					rValueWrapper = Me.VisitBadVariable(DirectCast(node, BoundBadVariable), arg)
					Exit Select
				Case BoundKind.ArrayAccess
					rValueWrapper = Me.VisitArrayAccess(DirectCast(node, BoundArrayAccess), arg)
					Exit Select
				Case BoundKind.ArrayLength
					rValueWrapper = Me.VisitArrayLength(DirectCast(node, BoundArrayLength), arg)
					Exit Select
				Case BoundKind.[GetType]
					rValueWrapper = Me.VisitGetType(DirectCast(node, BoundGetType), arg)
					Exit Select
				Case BoundKind.FieldInfo
					rValueWrapper = Me.VisitFieldInfo(DirectCast(node, BoundFieldInfo), arg)
					Exit Select
				Case BoundKind.MethodInfo
					rValueWrapper = Me.VisitMethodInfo(DirectCast(node, BoundMethodInfo), arg)
					Exit Select
				Case BoundKind.TypeExpression
					rValueWrapper = Me.VisitTypeExpression(DirectCast(node, BoundTypeExpression), arg)
					Exit Select
				Case BoundKind.TypeOrValueExpression
					rValueWrapper = Me.VisitTypeOrValueExpression(DirectCast(node, BoundTypeOrValueExpression), arg)
					Exit Select
				Case BoundKind.NamespaceExpression
					rValueWrapper = Me.VisitNamespaceExpression(DirectCast(node, BoundNamespaceExpression), arg)
					Exit Select
				Case BoundKind.MethodDefIndex
					rValueWrapper = Me.VisitMethodDefIndex(DirectCast(node, BoundMethodDefIndex), arg)
					Exit Select
				Case BoundKind.MaximumMethodDefIndex
					rValueWrapper = Me.VisitMaximumMethodDefIndex(DirectCast(node, BoundMaximumMethodDefIndex), arg)
					Exit Select
				Case BoundKind.InstrumentationPayloadRoot
					rValueWrapper = Me.VisitInstrumentationPayloadRoot(DirectCast(node, BoundInstrumentationPayloadRoot), arg)
					Exit Select
				Case BoundKind.ModuleVersionId
					rValueWrapper = Me.VisitModuleVersionId(DirectCast(node, BoundModuleVersionId), arg)
					Exit Select
				Case BoundKind.ModuleVersionIdString
					rValueWrapper = Me.VisitModuleVersionIdString(DirectCast(node, BoundModuleVersionIdString), arg)
					Exit Select
				Case BoundKind.SourceDocumentIndex
					rValueWrapper = Me.VisitSourceDocumentIndex(DirectCast(node, BoundSourceDocumentIndex), arg)
					Exit Select
				Case BoundKind.UnaryOperator
					rValueWrapper = Me.VisitUnaryOperator(DirectCast(node, BoundUnaryOperator), arg)
					Exit Select
				Case BoundKind.UserDefinedUnaryOperator
					rValueWrapper = Me.VisitUserDefinedUnaryOperator(DirectCast(node, BoundUserDefinedUnaryOperator), arg)
					Exit Select
				Case BoundKind.NullableIsTrueOperator
					rValueWrapper = Me.VisitNullableIsTrueOperator(DirectCast(node, BoundNullableIsTrueOperator), arg)
					Exit Select
				Case BoundKind.BinaryOperator
					rValueWrapper = Me.VisitBinaryOperator(DirectCast(node, BoundBinaryOperator), arg)
					Exit Select
				Case BoundKind.UserDefinedBinaryOperator
					rValueWrapper = Me.VisitUserDefinedBinaryOperator(DirectCast(node, BoundUserDefinedBinaryOperator), arg)
					Exit Select
				Case BoundKind.UserDefinedShortCircuitingOperator
					rValueWrapper = Me.VisitUserDefinedShortCircuitingOperator(DirectCast(node, BoundUserDefinedShortCircuitingOperator), arg)
					Exit Select
				Case BoundKind.CompoundAssignmentTargetPlaceholder
					rValueWrapper = Me.VisitCompoundAssignmentTargetPlaceholder(DirectCast(node, BoundCompoundAssignmentTargetPlaceholder), arg)
					Exit Select
				Case BoundKind.AssignmentOperator
					rValueWrapper = Me.VisitAssignmentOperator(DirectCast(node, BoundAssignmentOperator), arg)
					Exit Select
				Case BoundKind.ReferenceAssignment
					rValueWrapper = Me.VisitReferenceAssignment(DirectCast(node, BoundReferenceAssignment), arg)
					Exit Select
				Case BoundKind.AddressOfOperator
					rValueWrapper = Me.VisitAddressOfOperator(DirectCast(node, BoundAddressOfOperator), arg)
					Exit Select
				Case BoundKind.TernaryConditionalExpression
					rValueWrapper = Me.VisitTernaryConditionalExpression(DirectCast(node, BoundTernaryConditionalExpression), arg)
					Exit Select
				Case BoundKind.BinaryConditionalExpression
					rValueWrapper = Me.VisitBinaryConditionalExpression(DirectCast(node, BoundBinaryConditionalExpression), arg)
					Exit Select
				Case BoundKind.Conversion
					rValueWrapper = Me.VisitConversion(DirectCast(node, BoundConversion), arg)
					Exit Select
				Case BoundKind.RelaxationLambda
					rValueWrapper = Me.VisitRelaxationLambda(DirectCast(node, BoundRelaxationLambda), arg)
					Exit Select
				Case BoundKind.ConvertedTupleElements
					rValueWrapper = Me.VisitConvertedTupleElements(DirectCast(node, BoundConvertedTupleElements), arg)
					Exit Select
				Case BoundKind.UserDefinedConversion
					rValueWrapper = Me.VisitUserDefinedConversion(DirectCast(node, BoundUserDefinedConversion), arg)
					Exit Select
				Case BoundKind.[DirectCast]
					rValueWrapper = Me.VisitDirectCast(DirectCast(node, BoundDirectCast), arg)
					Exit Select
				Case BoundKind.[TryCast]
					rValueWrapper = Me.VisitTryCast(DirectCast(node, BoundTryCast), arg)
					Exit Select
				Case BoundKind.[TypeOf]
					rValueWrapper = Me.VisitTypeOf(DirectCast(node, BoundTypeOf), arg)
					Exit Select
				Case BoundKind.SequencePoint
					rValueWrapper = Me.VisitSequencePoint(DirectCast(node, BoundSequencePoint), arg)
					Exit Select
				Case BoundKind.SequencePointExpression
					rValueWrapper = Me.VisitSequencePointExpression(DirectCast(node, BoundSequencePointExpression), arg)
					Exit Select
				Case BoundKind.SequencePointWithSpan
					rValueWrapper = Me.VisitSequencePointWithSpan(DirectCast(node, BoundSequencePointWithSpan), arg)
					Exit Select
				Case BoundKind.NoOpStatement
					rValueWrapper = Me.VisitNoOpStatement(DirectCast(node, BoundNoOpStatement), arg)
					Exit Select
				Case BoundKind.MethodGroup
					rValueWrapper = Me.VisitMethodGroup(DirectCast(node, BoundMethodGroup), arg)
					Exit Select
				Case BoundKind.PropertyGroup
					rValueWrapper = Me.VisitPropertyGroup(DirectCast(node, BoundPropertyGroup), arg)
					Exit Select
				Case BoundKind.ReturnStatement
					rValueWrapper = Me.VisitReturnStatement(DirectCast(node, BoundReturnStatement), arg)
					Exit Select
				Case BoundKind.YieldStatement
					rValueWrapper = Me.VisitYieldStatement(DirectCast(node, BoundYieldStatement), arg)
					Exit Select
				Case BoundKind.ThrowStatement
					rValueWrapper = Me.VisitThrowStatement(DirectCast(node, BoundThrowStatement), arg)
					Exit Select
				Case BoundKind.RedimStatement
					rValueWrapper = Me.VisitRedimStatement(DirectCast(node, BoundRedimStatement), arg)
					Exit Select
				Case BoundKind.RedimClause
					rValueWrapper = Me.VisitRedimClause(DirectCast(node, BoundRedimClause), arg)
					Exit Select
				Case BoundKind.EraseStatement
					rValueWrapper = Me.VisitEraseStatement(DirectCast(node, BoundEraseStatement), arg)
					Exit Select
				Case BoundKind.[Call]
					rValueWrapper = Me.VisitCall(DirectCast(node, BoundCall), arg)
					Exit Select
				Case BoundKind.Attribute
					rValueWrapper = Me.VisitAttribute(DirectCast(node, BoundAttribute), arg)
					Exit Select
				Case BoundKind.LateMemberAccess
					rValueWrapper = Me.VisitLateMemberAccess(DirectCast(node, BoundLateMemberAccess), arg)
					Exit Select
				Case BoundKind.LateInvocation
					rValueWrapper = Me.VisitLateInvocation(DirectCast(node, BoundLateInvocation), arg)
					Exit Select
				Case BoundKind.LateAddressOfOperator
					rValueWrapper = Me.VisitLateAddressOfOperator(DirectCast(node, BoundLateAddressOfOperator), arg)
					Exit Select
				Case BoundKind.TupleLiteral
					rValueWrapper = Me.VisitTupleLiteral(DirectCast(node, BoundTupleLiteral), arg)
					Exit Select
				Case BoundKind.ConvertedTupleLiteral
					rValueWrapper = Me.VisitConvertedTupleLiteral(DirectCast(node, BoundConvertedTupleLiteral), arg)
					Exit Select
				Case BoundKind.ObjectCreationExpression
					rValueWrapper = Me.VisitObjectCreationExpression(DirectCast(node, BoundObjectCreationExpression), arg)
					Exit Select
				Case BoundKind.NoPiaObjectCreationExpression
					rValueWrapper = Me.VisitNoPiaObjectCreationExpression(DirectCast(node, BoundNoPiaObjectCreationExpression), arg)
					Exit Select
				Case BoundKind.AnonymousTypeCreationExpression
					rValueWrapper = Me.VisitAnonymousTypeCreationExpression(DirectCast(node, BoundAnonymousTypeCreationExpression), arg)
					Exit Select
				Case BoundKind.AnonymousTypePropertyAccess
					rValueWrapper = Me.VisitAnonymousTypePropertyAccess(DirectCast(node, BoundAnonymousTypePropertyAccess), arg)
					Exit Select
				Case BoundKind.AnonymousTypeFieldInitializer
					rValueWrapper = Me.VisitAnonymousTypeFieldInitializer(DirectCast(node, BoundAnonymousTypeFieldInitializer), arg)
					Exit Select
				Case BoundKind.ObjectInitializerExpression
					rValueWrapper = Me.VisitObjectInitializerExpression(DirectCast(node, BoundObjectInitializerExpression), arg)
					Exit Select
				Case BoundKind.CollectionInitializerExpression
					rValueWrapper = Me.VisitCollectionInitializerExpression(DirectCast(node, BoundCollectionInitializerExpression), arg)
					Exit Select
				Case BoundKind.NewT
					rValueWrapper = Me.VisitNewT(DirectCast(node, BoundNewT), arg)
					Exit Select
				Case BoundKind.DelegateCreationExpression
					rValueWrapper = Me.VisitDelegateCreationExpression(DirectCast(node, BoundDelegateCreationExpression), arg)
					Exit Select
				Case BoundKind.ArrayCreation
					rValueWrapper = Me.VisitArrayCreation(DirectCast(node, BoundArrayCreation), arg)
					Exit Select
				Case BoundKind.ArrayLiteral
					rValueWrapper = Me.VisitArrayLiteral(DirectCast(node, BoundArrayLiteral), arg)
					Exit Select
				Case BoundKind.ArrayInitialization
					rValueWrapper = Me.VisitArrayInitialization(DirectCast(node, BoundArrayInitialization), arg)
					Exit Select
				Case BoundKind.FieldAccess
					rValueWrapper = Me.VisitFieldAccess(DirectCast(node, BoundFieldAccess), arg)
					Exit Select
				Case BoundKind.PropertyAccess
					rValueWrapper = Me.VisitPropertyAccess(DirectCast(node, BoundPropertyAccess), arg)
					Exit Select
				Case BoundKind.EventAccess
					rValueWrapper = Me.VisitEventAccess(DirectCast(node, BoundEventAccess), arg)
					Exit Select
				Case BoundKind.Block
					rValueWrapper = Me.VisitBlock(DirectCast(node, BoundBlock), arg)
					Exit Select
				Case BoundKind.StateMachineScope
					rValueWrapper = Me.VisitStateMachineScope(DirectCast(node, BoundStateMachineScope), arg)
					Exit Select
				Case BoundKind.LocalDeclaration
					rValueWrapper = Me.VisitLocalDeclaration(DirectCast(node, BoundLocalDeclaration), arg)
					Exit Select
				Case BoundKind.AsNewLocalDeclarations
					rValueWrapper = Me.VisitAsNewLocalDeclarations(DirectCast(node, BoundAsNewLocalDeclarations), arg)
					Exit Select
				Case BoundKind.DimStatement
					rValueWrapper = Me.VisitDimStatement(DirectCast(node, BoundDimStatement), arg)
					Exit Select
				Case BoundKind.Initializer
					rValueWrapper = Me.VisitInitializer(DirectCast(node, BoundInitializer), arg)
					Exit Select
				Case BoundKind.FieldInitializer
					rValueWrapper = Me.VisitFieldInitializer(DirectCast(node, BoundFieldInitializer), arg)
					Exit Select
				Case BoundKind.PropertyInitializer
					rValueWrapper = Me.VisitPropertyInitializer(DirectCast(node, BoundPropertyInitializer), arg)
					Exit Select
				Case BoundKind.ParameterEqualsValue
					rValueWrapper = Me.VisitParameterEqualsValue(DirectCast(node, BoundParameterEqualsValue), arg)
					Exit Select
				Case BoundKind.GlobalStatementInitializer
					rValueWrapper = Me.VisitGlobalStatementInitializer(DirectCast(node, BoundGlobalStatementInitializer), arg)
					Exit Select
				Case BoundKind.Sequence
					rValueWrapper = Me.VisitSequence(DirectCast(node, BoundSequence), arg)
					Exit Select
				Case BoundKind.ExpressionStatement
					rValueWrapper = Me.VisitExpressionStatement(DirectCast(node, BoundExpressionStatement), arg)
					Exit Select
				Case BoundKind.IfStatement
					rValueWrapper = Me.VisitIfStatement(DirectCast(node, BoundIfStatement), arg)
					Exit Select
				Case BoundKind.SelectStatement
					rValueWrapper = Me.VisitSelectStatement(DirectCast(node, BoundSelectStatement), arg)
					Exit Select
				Case BoundKind.CaseBlock
					rValueWrapper = Me.VisitCaseBlock(DirectCast(node, BoundCaseBlock), arg)
					Exit Select
				Case BoundKind.CaseStatement
					rValueWrapper = Me.VisitCaseStatement(DirectCast(node, BoundCaseStatement), arg)
					Exit Select
				Case BoundKind.SimpleCaseClause
					rValueWrapper = Me.VisitSimpleCaseClause(DirectCast(node, BoundSimpleCaseClause), arg)
					Exit Select
				Case BoundKind.RangeCaseClause
					rValueWrapper = Me.VisitRangeCaseClause(DirectCast(node, BoundRangeCaseClause), arg)
					Exit Select
				Case BoundKind.RelationalCaseClause
					rValueWrapper = Me.VisitRelationalCaseClause(DirectCast(node, BoundRelationalCaseClause), arg)
					Exit Select
				Case BoundKind.DoLoopStatement
					rValueWrapper = Me.VisitDoLoopStatement(DirectCast(node, BoundDoLoopStatement), arg)
					Exit Select
				Case BoundKind.WhileStatement
					rValueWrapper = Me.VisitWhileStatement(DirectCast(node, BoundWhileStatement), arg)
					Exit Select
				Case BoundKind.ForToUserDefinedOperators
					rValueWrapper = Me.VisitForToUserDefinedOperators(DirectCast(node, BoundForToUserDefinedOperators), arg)
					Exit Select
				Case BoundKind.ForToStatement
					rValueWrapper = Me.VisitForToStatement(DirectCast(node, BoundForToStatement), arg)
					Exit Select
				Case BoundKind.ForEachStatement
					rValueWrapper = Me.VisitForEachStatement(DirectCast(node, BoundForEachStatement), arg)
					Exit Select
				Case BoundKind.ExitStatement
					rValueWrapper = Me.VisitExitStatement(DirectCast(node, BoundExitStatement), arg)
					Exit Select
				Case BoundKind.ContinueStatement
					rValueWrapper = Me.VisitContinueStatement(DirectCast(node, BoundContinueStatement), arg)
					Exit Select
				Case BoundKind.TryStatement
					rValueWrapper = Me.VisitTryStatement(DirectCast(node, BoundTryStatement), arg)
					Exit Select
				Case BoundKind.CatchBlock
					rValueWrapper = Me.VisitCatchBlock(DirectCast(node, BoundCatchBlock), arg)
					Exit Select
				Case BoundKind.Literal
					rValueWrapper = Me.VisitLiteral(DirectCast(node, BoundLiteral), arg)
					Exit Select
				Case BoundKind.MeReference
					rValueWrapper = Me.VisitMeReference(DirectCast(node, BoundMeReference), arg)
					Exit Select
				Case BoundKind.ValueTypeMeReference
					rValueWrapper = Me.VisitValueTypeMeReference(DirectCast(node, BoundValueTypeMeReference), arg)
					Exit Select
				Case BoundKind.MyBaseReference
					rValueWrapper = Me.VisitMyBaseReference(DirectCast(node, BoundMyBaseReference), arg)
					Exit Select
				Case BoundKind.MyClassReference
					rValueWrapper = Me.VisitMyClassReference(DirectCast(node, BoundMyClassReference), arg)
					Exit Select
				Case BoundKind.PreviousSubmissionReference
					rValueWrapper = Me.VisitPreviousSubmissionReference(DirectCast(node, BoundPreviousSubmissionReference), arg)
					Exit Select
				Case BoundKind.HostObjectMemberReference
					rValueWrapper = Me.VisitHostObjectMemberReference(DirectCast(node, BoundHostObjectMemberReference), arg)
					Exit Select
				Case BoundKind.Local
					rValueWrapper = Me.VisitLocal(DirectCast(node, BoundLocal), arg)
					Exit Select
				Case BoundKind.PseudoVariable
					rValueWrapper = Me.VisitPseudoVariable(DirectCast(node, BoundPseudoVariable), arg)
					Exit Select
				Case BoundKind.Parameter
					rValueWrapper = Me.VisitParameter(DirectCast(node, BoundParameter), arg)
					Exit Select
				Case BoundKind.ByRefArgumentPlaceholder
					rValueWrapper = Me.VisitByRefArgumentPlaceholder(DirectCast(node, BoundByRefArgumentPlaceholder), arg)
					Exit Select
				Case BoundKind.ByRefArgumentWithCopyBack
					rValueWrapper = Me.VisitByRefArgumentWithCopyBack(DirectCast(node, BoundByRefArgumentWithCopyBack), arg)
					Exit Select
				Case BoundKind.LateBoundArgumentSupportingAssignmentWithCapture
					rValueWrapper = Me.VisitLateBoundArgumentSupportingAssignmentWithCapture(DirectCast(node, BoundLateBoundArgumentSupportingAssignmentWithCapture), arg)
					Exit Select
				Case BoundKind.LabelStatement
					rValueWrapper = Me.VisitLabelStatement(DirectCast(node, BoundLabelStatement), arg)
					Exit Select
				Case BoundKind.Label
					rValueWrapper = Me.VisitLabel(DirectCast(node, BoundLabel), arg)
					Exit Select
				Case BoundKind.GotoStatement
					rValueWrapper = Me.VisitGotoStatement(DirectCast(node, BoundGotoStatement), arg)
					Exit Select
				Case BoundKind.StatementList
					rValueWrapper = Me.VisitStatementList(DirectCast(node, BoundStatementList), arg)
					Exit Select
				Case BoundKind.ConditionalGoto
					rValueWrapper = Me.VisitConditionalGoto(DirectCast(node, BoundConditionalGoto), arg)
					Exit Select
				Case BoundKind.WithStatement
					rValueWrapper = Me.VisitWithStatement(DirectCast(node, BoundWithStatement), arg)
					Exit Select
				Case BoundKind.UnboundLambda
					rValueWrapper = Me.VisitUnboundLambda(DirectCast(node, UnboundLambda), arg)
					Exit Select
				Case BoundKind.Lambda
					rValueWrapper = Me.VisitLambda(DirectCast(node, BoundLambda), arg)
					Exit Select
				Case BoundKind.QueryExpression
					rValueWrapper = Me.VisitQueryExpression(DirectCast(node, BoundQueryExpression), arg)
					Exit Select
				Case BoundKind.QuerySource
					rValueWrapper = Me.VisitQuerySource(DirectCast(node, BoundQuerySource), arg)
					Exit Select
				Case BoundKind.ToQueryableCollectionConversion
					rValueWrapper = Me.VisitToQueryableCollectionConversion(DirectCast(node, BoundToQueryableCollectionConversion), arg)
					Exit Select
				Case BoundKind.QueryableSource
					rValueWrapper = Me.VisitQueryableSource(DirectCast(node, BoundQueryableSource), arg)
					Exit Select
				Case BoundKind.QueryClause
					rValueWrapper = Me.VisitQueryClause(DirectCast(node, BoundQueryClause), arg)
					Exit Select
				Case BoundKind.Ordering
					rValueWrapper = Me.VisitOrdering(DirectCast(node, BoundOrdering), arg)
					Exit Select
				Case BoundKind.QueryLambda
					rValueWrapper = Me.VisitQueryLambda(DirectCast(node, BoundQueryLambda), arg)
					Exit Select
				Case BoundKind.RangeVariableAssignment
					rValueWrapper = Me.VisitRangeVariableAssignment(DirectCast(node, BoundRangeVariableAssignment), arg)
					Exit Select
				Case BoundKind.GroupTypeInferenceLambda
					rValueWrapper = Me.VisitGroupTypeInferenceLambda(DirectCast(node, GroupTypeInferenceLambda), arg)
					Exit Select
				Case BoundKind.AggregateClause
					rValueWrapper = Me.VisitAggregateClause(DirectCast(node, BoundAggregateClause), arg)
					Exit Select
				Case BoundKind.GroupAggregation
					rValueWrapper = Me.VisitGroupAggregation(DirectCast(node, BoundGroupAggregation), arg)
					Exit Select
				Case BoundKind.RangeVariable
					rValueWrapper = Me.VisitRangeVariable(DirectCast(node, BoundRangeVariable), arg)
					Exit Select
				Case BoundKind.AddHandlerStatement
					rValueWrapper = Me.VisitAddHandlerStatement(DirectCast(node, BoundAddHandlerStatement), arg)
					Exit Select
				Case BoundKind.RemoveHandlerStatement
					rValueWrapper = Me.VisitRemoveHandlerStatement(DirectCast(node, BoundRemoveHandlerStatement), arg)
					Exit Select
				Case BoundKind.RaiseEventStatement
					rValueWrapper = Me.VisitRaiseEventStatement(DirectCast(node, BoundRaiseEventStatement), arg)
					Exit Select
				Case BoundKind.UsingStatement
					rValueWrapper = Me.VisitUsingStatement(DirectCast(node, BoundUsingStatement), arg)
					Exit Select
				Case BoundKind.SyncLockStatement
					rValueWrapper = Me.VisitSyncLockStatement(DirectCast(node, BoundSyncLockStatement), arg)
					Exit Select
				Case BoundKind.XmlName
					rValueWrapper = Me.VisitXmlName(DirectCast(node, BoundXmlName), arg)
					Exit Select
				Case BoundKind.XmlNamespace
					rValueWrapper = Me.VisitXmlNamespace(DirectCast(node, BoundXmlNamespace), arg)
					Exit Select
				Case BoundKind.XmlDocument
					rValueWrapper = Me.VisitXmlDocument(DirectCast(node, BoundXmlDocument), arg)
					Exit Select
				Case BoundKind.XmlDeclaration
					rValueWrapper = Me.VisitXmlDeclaration(DirectCast(node, BoundXmlDeclaration), arg)
					Exit Select
				Case BoundKind.XmlProcessingInstruction
					rValueWrapper = Me.VisitXmlProcessingInstruction(DirectCast(node, BoundXmlProcessingInstruction), arg)
					Exit Select
				Case BoundKind.XmlComment
					rValueWrapper = Me.VisitXmlComment(DirectCast(node, BoundXmlComment), arg)
					Exit Select
				Case BoundKind.XmlAttribute
					rValueWrapper = Me.VisitXmlAttribute(DirectCast(node, BoundXmlAttribute), arg)
					Exit Select
				Case BoundKind.XmlElement
					rValueWrapper = Me.VisitXmlElement(DirectCast(node, BoundXmlElement), arg)
					Exit Select
				Case BoundKind.XmlMemberAccess
					rValueWrapper = Me.VisitXmlMemberAccess(DirectCast(node, BoundXmlMemberAccess), arg)
					Exit Select
				Case BoundKind.XmlEmbeddedExpression
					rValueWrapper = Me.VisitXmlEmbeddedExpression(DirectCast(node, BoundXmlEmbeddedExpression), arg)
					Exit Select
				Case BoundKind.XmlCData
					rValueWrapper = Me.VisitXmlCData(DirectCast(node, BoundXmlCData), arg)
					Exit Select
				Case BoundKind.ResumeStatement
					rValueWrapper = Me.VisitResumeStatement(DirectCast(node, BoundResumeStatement), arg)
					Exit Select
				Case BoundKind.OnErrorStatement
					rValueWrapper = Me.VisitOnErrorStatement(DirectCast(node, BoundOnErrorStatement), arg)
					Exit Select
				Case BoundKind.UnstructuredExceptionHandlingStatement
					rValueWrapper = Me.VisitUnstructuredExceptionHandlingStatement(DirectCast(node, BoundUnstructuredExceptionHandlingStatement), arg)
					Exit Select
				Case BoundKind.UnstructuredExceptionHandlingCatchFilter
					rValueWrapper = Me.VisitUnstructuredExceptionHandlingCatchFilter(DirectCast(node, BoundUnstructuredExceptionHandlingCatchFilter), arg)
					Exit Select
				Case BoundKind.UnstructuredExceptionOnErrorSwitch
					rValueWrapper = Me.VisitUnstructuredExceptionOnErrorSwitch(DirectCast(node, BoundUnstructuredExceptionOnErrorSwitch), arg)
					Exit Select
				Case BoundKind.UnstructuredExceptionResumeSwitch
					rValueWrapper = Me.VisitUnstructuredExceptionResumeSwitch(DirectCast(node, BoundUnstructuredExceptionResumeSwitch), arg)
					Exit Select
				Case BoundKind.AwaitOperator
					rValueWrapper = Me.VisitAwaitOperator(DirectCast(node, BoundAwaitOperator), arg)
					Exit Select
				Case BoundKind.SpillSequence
					rValueWrapper = Me.VisitSpillSequence(DirectCast(node, BoundSpillSequence), arg)
					Exit Select
				Case BoundKind.StopStatement
					rValueWrapper = Me.VisitStopStatement(DirectCast(node, BoundStopStatement), arg)
					Exit Select
				Case BoundKind.EndStatement
					rValueWrapper = Me.VisitEndStatement(DirectCast(node, BoundEndStatement), arg)
					Exit Select
				Case BoundKind.MidResult
					rValueWrapper = Me.VisitMidResult(DirectCast(node, BoundMidResult), arg)
					Exit Select
				Case BoundKind.ConditionalAccess
					rValueWrapper = Me.VisitConditionalAccess(DirectCast(node, BoundConditionalAccess), arg)
					Exit Select
				Case BoundKind.ConditionalAccessReceiverPlaceholder
					rValueWrapper = Me.VisitConditionalAccessReceiverPlaceholder(DirectCast(node, BoundConditionalAccessReceiverPlaceholder), arg)
					Exit Select
				Case BoundKind.LoweredConditionalAccess
					rValueWrapper = Me.VisitLoweredConditionalAccess(DirectCast(node, BoundLoweredConditionalAccess), arg)
					Exit Select
				Case BoundKind.ComplexConditionalAccessReceiver
					rValueWrapper = Me.VisitComplexConditionalAccessReceiver(DirectCast(node, BoundComplexConditionalAccessReceiver), arg)
					Exit Select
				Case BoundKind.NameOfOperator
					rValueWrapper = Me.VisitNameOfOperator(DirectCast(node, BoundNameOfOperator), arg)
					Exit Select
				Case BoundKind.TypeAsValueExpression
					rValueWrapper = Me.VisitTypeAsValueExpression(DirectCast(node, BoundTypeAsValueExpression), arg)
					Exit Select
				Case BoundKind.InterpolatedStringExpression
					rValueWrapper = Me.VisitInterpolatedStringExpression(DirectCast(node, BoundInterpolatedStringExpression), arg)
					Exit Select
				Case BoundKind.Interpolation
					rValueWrapper = Me.VisitInterpolation(DirectCast(node, BoundInterpolation), arg)
					Exit Select
				Case Else
					rValueWrapper = Me.DefaultVisit(node, arg)
					Exit Select
			End Select
			Return rValueWrapper
		End Function

		Public Overridable Function VisitInterpolatedStringExpression(ByVal node As BoundInterpolatedStringExpression, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitInterpolation(ByVal node As BoundInterpolation, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitLabel(ByVal node As BoundLabel, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitLabelStatement(ByVal node As BoundLabelStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitLambda(ByVal node As BoundLambda, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitLateAddressOfOperator(ByVal node As BoundLateAddressOfOperator, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitLateBoundArgumentSupportingAssignmentWithCapture(ByVal node As BoundLateBoundArgumentSupportingAssignmentWithCapture, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitLateInvocation(ByVal node As BoundLateInvocation, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitLateMemberAccess(ByVal node As BoundLateMemberAccess, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitLiteral(ByVal node As BoundLiteral, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitLocal(ByVal node As BoundLocal, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitLocalDeclaration(ByVal node As BoundLocalDeclaration, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitLoweredConditionalAccess(ByVal node As BoundLoweredConditionalAccess, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitLValuePlaceholder(ByVal node As BoundLValuePlaceholder, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitLValueToRValueWrapper(ByVal node As BoundLValueToRValueWrapper, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitMaximumMethodDefIndex(ByVal node As BoundMaximumMethodDefIndex, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitMeReference(ByVal node As BoundMeReference, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitMethodDefIndex(ByVal node As BoundMethodDefIndex, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitMethodGroup(ByVal node As BoundMethodGroup, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitMethodInfo(ByVal node As BoundMethodInfo, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitMidResult(ByVal node As BoundMidResult, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitModuleVersionId(ByVal node As BoundModuleVersionId, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitModuleVersionIdString(ByVal node As BoundModuleVersionIdString, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitMyBaseReference(ByVal node As BoundMyBaseReference, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitMyClassReference(ByVal node As BoundMyClassReference, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitNameOfOperator(ByVal node As BoundNameOfOperator, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitNamespaceExpression(ByVal node As BoundNamespaceExpression, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitNewT(ByVal node As BoundNewT, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitNoOpStatement(ByVal node As BoundNoOpStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitNoPiaObjectCreationExpression(ByVal node As BoundNoPiaObjectCreationExpression, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitNullableIsTrueOperator(ByVal node As BoundNullableIsTrueOperator, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitObjectCreationExpression(ByVal node As BoundObjectCreationExpression, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitObjectInitializerExpression(ByVal node As BoundObjectInitializerExpression, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitOmittedArgument(ByVal node As BoundOmittedArgument, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitOnErrorStatement(ByVal node As BoundOnErrorStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitOrdering(ByVal node As BoundOrdering, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitParameter(ByVal node As BoundParameter, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitParameterEqualsValue(ByVal node As BoundParameterEqualsValue, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitParenthesized(ByVal node As BoundParenthesized, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitPreviousSubmissionReference(ByVal node As BoundPreviousSubmissionReference, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitPropertyAccess(ByVal node As BoundPropertyAccess, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitPropertyGroup(ByVal node As BoundPropertyGroup, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitPropertyInitializer(ByVal node As BoundPropertyInitializer, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitPseudoVariable(ByVal node As BoundPseudoVariable, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitQueryableSource(ByVal node As BoundQueryableSource, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitQueryClause(ByVal node As BoundQueryClause, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitQueryExpression(ByVal node As BoundQueryExpression, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitQueryLambda(ByVal node As BoundQueryLambda, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitQuerySource(ByVal node As BoundQuerySource, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitRaiseEventStatement(ByVal node As BoundRaiseEventStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitRangeCaseClause(ByVal node As BoundRangeCaseClause, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitRangeVariable(ByVal node As BoundRangeVariable, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitRangeVariableAssignment(ByVal node As BoundRangeVariableAssignment, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitRedimClause(ByVal node As BoundRedimClause, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitRedimStatement(ByVal node As BoundRedimStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitReferenceAssignment(ByVal node As BoundReferenceAssignment, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitRelationalCaseClause(ByVal node As BoundRelationalCaseClause, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitRelaxationLambda(ByVal node As BoundRelaxationLambda, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitRemoveHandlerStatement(ByVal node As BoundRemoveHandlerStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitResumeStatement(ByVal node As BoundResumeStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitReturnStatement(ByVal node As BoundReturnStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitRValuePlaceholder(ByVal node As BoundRValuePlaceholder, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitSelectStatement(ByVal node As BoundSelectStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitSequence(ByVal node As BoundSequence, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitSequencePoint(ByVal node As BoundSequencePoint, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitSequencePointExpression(ByVal node As BoundSequencePointExpression, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitSequencePointWithSpan(ByVal node As BoundSequencePointWithSpan, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitSimpleCaseClause(ByVal node As BoundSimpleCaseClause, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitSourceDocumentIndex(ByVal node As BoundSourceDocumentIndex, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitSpillSequence(ByVal node As BoundSpillSequence, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitStateMachineScope(ByVal node As BoundStateMachineScope, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitStatementList(ByVal node As BoundStatementList, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitStopStatement(ByVal node As BoundStopStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitSyncLockStatement(ByVal node As BoundSyncLockStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitTernaryConditionalExpression(ByVal node As BoundTernaryConditionalExpression, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitThrowStatement(ByVal node As BoundThrowStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitToQueryableCollectionConversion(ByVal node As BoundToQueryableCollectionConversion, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitTryCast(ByVal node As BoundTryCast, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitTryStatement(ByVal node As BoundTryStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitTupleLiteral(ByVal node As BoundTupleLiteral, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitTypeArguments(ByVal node As BoundTypeArguments, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitTypeAsValueExpression(ByVal node As BoundTypeAsValueExpression, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitTypeExpression(ByVal node As BoundTypeExpression, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitTypeOf(ByVal node As BoundTypeOf, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitTypeOrValueExpression(ByVal node As BoundTypeOrValueExpression, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitUnaryOperator(ByVal node As BoundUnaryOperator, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitUnboundLambda(ByVal node As UnboundLambda, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitUnstructuredExceptionHandlingCatchFilter(ByVal node As BoundUnstructuredExceptionHandlingCatchFilter, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitUnstructuredExceptionHandlingStatement(ByVal node As BoundUnstructuredExceptionHandlingStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitUnstructuredExceptionOnErrorSwitch(ByVal node As BoundUnstructuredExceptionOnErrorSwitch, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitUnstructuredExceptionResumeSwitch(ByVal node As BoundUnstructuredExceptionResumeSwitch, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitUserDefinedBinaryOperator(ByVal node As BoundUserDefinedBinaryOperator, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitUserDefinedConversion(ByVal node As BoundUserDefinedConversion, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitUserDefinedShortCircuitingOperator(ByVal node As BoundUserDefinedShortCircuitingOperator, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitUserDefinedUnaryOperator(ByVal node As BoundUserDefinedUnaryOperator, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitUsingStatement(ByVal node As BoundUsingStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitValueTypeMeReference(ByVal node As BoundValueTypeMeReference, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitWhileStatement(ByVal node As BoundWhileStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitWithLValueExpressionPlaceholder(ByVal node As BoundWithLValueExpressionPlaceholder, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitWithRValueExpressionPlaceholder(ByVal node As BoundWithRValueExpressionPlaceholder, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitWithStatement(ByVal node As BoundWithStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitXmlAttribute(ByVal node As BoundXmlAttribute, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitXmlCData(ByVal node As BoundXmlCData, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitXmlComment(ByVal node As BoundXmlComment, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitXmlDeclaration(ByVal node As BoundXmlDeclaration, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitXmlDocument(ByVal node As BoundXmlDocument, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitXmlElement(ByVal node As BoundXmlElement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitXmlEmbeddedExpression(ByVal node As BoundXmlEmbeddedExpression, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitXmlMemberAccess(ByVal node As BoundXmlMemberAccess, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitXmlName(ByVal node As BoundXmlName, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitXmlNamespace(ByVal node As BoundXmlNamespace, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitXmlProcessingInstruction(ByVal node As BoundXmlProcessingInstruction, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function

		Public Overridable Function VisitYieldStatement(ByVal node As BoundYieldStatement, ByVal arg As A) As R
			Return Me.DefaultVisit(node, arg)
		End Function
	End Class
End Namespace