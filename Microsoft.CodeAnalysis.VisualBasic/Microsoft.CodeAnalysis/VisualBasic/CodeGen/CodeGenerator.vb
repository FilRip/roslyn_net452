Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Diagnostics
Imports System.Linq
Imports System.Reflection
Imports System.Reflection.Metadata
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.CodeGen
	Friend NotInheritable Class CodeGenerator
		Private ReadOnly _method As MethodSymbol

		Private ReadOnly _block As BoundStatement

		Private ReadOnly _builder As ILBuilder

		Private ReadOnly _module As PEModuleBuilder

		Private ReadOnly _diagnostics As DiagnosticBag

		Private ReadOnly _ilEmitStyle As ILEmitStyle

		Private ReadOnly _emitPdbSequencePoints As Boolean

		Private ReadOnly _stackLocals As HashSet(Of LocalSymbol)

		Private _tryNestingLevel As Integer

		Private _currentCatchBlock As BoundCatchBlock

		Private ReadOnly _synthesizedLocalOrdinals As SynthesizedLocalOrdinalsDispenser

		Private _uniqueNameId As Integer

		Private ReadOnly Shared s_returnLabel As Object

		Private _unhandledReturn As Boolean

		Private _checkCallsForUnsafeJITOptimization As Boolean

		Private _asyncCatchHandlerOffset As Integer

		Private _asyncYieldPoints As ArrayBuilder(Of Integer)

		Private _asyncResumePoints As ArrayBuilder(Of Integer)

		Private _recursionDepth As Integer

		Private ReadOnly Shared s_compOpCodes As ILOpCode()

		Private Const s_IL_OP_CODE_ROW_LENGTH As Integer = 4

		Private ReadOnly Shared s_condJumpOpCodes As ILOpCode()

		Shared Sub New()
			CodeGenerator.s_returnLabel = RuntimeHelpers.GetObjectValue(New [Object]())
			CodeGenerator.s_compOpCodes = New ILOpCode() { GetType(<PrivateImplementationDetails>).GetField("836F7F350F6523EDCBE94991CEDFDC6FFC6591EC9E6EA14AADC93E75220DB435").FieldHandle }
			CodeGenerator.s_condJumpOpCodes = New ILOpCode() { GetType(<PrivateImplementationDetails>).GetField("95746B76CA7FB81DB12FDC08E025796B0DE2CC336E8711AD7DFE059826AA2F63").FieldHandle }
		End Sub

		Public Sub New(ByVal method As MethodSymbol, ByVal boundBody As BoundStatement, ByVal builder As ILBuilder, ByVal moduleBuilder As PEModuleBuilder, ByVal diagnostics As DiagnosticBag, ByVal optimizations As OptimizationLevel, ByVal emittingPdb As Boolean)
			MyBase.New()
			Me._stackLocals = Nothing
			Me._tryNestingLevel = 0
			Me._currentCatchBlock = Nothing
			Me._synthesizedLocalOrdinals = New SynthesizedLocalOrdinalsDispenser()
			Me._asyncCatchHandlerOffset = -1
			Me._asyncYieldPoints = Nothing
			Me._asyncResumePoints = Nothing
			Me._method = method
			Me._block = boundBody
			Me._builder = builder
			Me._module = moduleBuilder
			Me._diagnostics = diagnostics
			If (Not method.GenerateDebugInfo) Then
				Me._ilEmitStyle = ILEmitStyle.Release
			ElseIf (optimizations <> OptimizationLevel.Debug) Then
				Me._ilEmitStyle = If(Me.IsDebugPlus(), ILEmitStyle.DebugFriendlyRelease, ILEmitStyle.Release)
			Else
				Me._ilEmitStyle = ILEmitStyle.Debug
			End If
			Me._emitPdbSequencePoints = If(Not emittingPdb, False, method.GenerateDebugInfo)
			Try
				Me._block = Optimizer.Optimize(method, boundBody, Me._ilEmitStyle <> ILEmitStyle.Release, Me._stackLocals)
			Catch cancelledByStackGuardException As BoundTreeVisitor.CancelledByStackGuardException
				ProjectData.SetProjectError(cancelledByStackGuardException)
				cancelledByStackGuardException.AddAnError(diagnostics)
				Me._block = boundBody
				ProjectData.ClearProjectError()
			End Try
			Me._checkCallsForUnsafeJITOptimization = (Me._method.ImplementationAttributes And (MethodImplAttributes.NoInlining Or MethodImplAttributes.NoOptimization)) <> (MethodImplAttributes.NoInlining Or MethodImplAttributes.NoOptimization)
		End Sub

		Private Function AllocateTemp(ByVal type As TypeSymbol, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode) As LocalDefinition
			Dim localSlotManager As Microsoft.CodeAnalysis.CodeGen.LocalSlotManager = Me._builder.LocalSlotManager
			Dim typeReference As ITypeReference = Me._module.Translate(type, syntaxNode, Me._diagnostics)
			Dim flags As ImmutableArray(Of Boolean) = New ImmutableArray(Of Boolean)()
			Return localSlotManager.AllocateSlot(typeReference, LocalSlotConstraints.None, flags, New ImmutableArray(Of String)())
		End Function

		Private Function AllowedToTakeRef(ByVal expression As BoundExpression, ByVal addressKind As CodeGenerator.AddressKind) As Boolean
			Dim isReference As Boolean
			If (expression.Kind = BoundKind.ConditionalAccessReceiverPlaceholder OrElse expression.Kind = BoundKind.ComplexConditionalAccessReceiver) Then
				isReference = If(addressKind = CodeGenerator.AddressKind.[ReadOnly], True, addressKind = CodeGenerator.AddressKind.Immutable)
			Else
				If (addressKind <> CodeGenerator.AddressKind.Immutable) Then
					Dim kind As BoundKind = expression.Kind
					If (kind <= BoundKind.FieldAccess) Then
						If (kind = BoundKind.Dup) Then
							isReference = DirectCast(expression, BoundDup).IsReference
							Return isReference
						Else
							If (kind <> BoundKind.FieldAccess) Then
								GoTo Label2
							End If
							isReference = Me.AllowedToTakeRef(DirectCast(expression, BoundFieldAccess), addressKind)
							Return isReference
						End If
					ElseIf (kind = BoundKind.Sequence) Then
						Dim valueOpt As BoundExpression = DirectCast(expression, BoundSequence).ValueOpt
						isReference = If(valueOpt Is Nothing, False, Me.AllowedToTakeRef(valueOpt, addressKind))
						Return isReference
					Else
						Select Case kind
							Case BoundKind.MeReference
							Case BoundKind.MyClassReference
								isReference = addressKind <> CodeGenerator.AddressKind.Writeable
								Return isReference
							Case BoundKind.Local
								isReference = Me.AllowedToTakeRef(DirectCast(expression, BoundLocal), addressKind)
								Return isReference
							Case BoundKind.PseudoVariable
								isReference = True
								Return isReference
							Case BoundKind.Parameter
								isReference = True
								Return isReference
						End Select
					End If
				End If
			Label2:
				isReference = Me.HasHome(expression)
			End If
			Return isReference
		End Function

		Private Function AllowedToTakeRef(ByVal boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal, ByVal addressKind As CodeGenerator.AddressKind) As Boolean
			Dim flag As Boolean
			If (addressKind = CodeGenerator.AddressKind.Writeable AndAlso boundLocal.LocalSymbol.IsReadOnly AndAlso Not boundLocal.IsLValue) Then
				flag = False
			ElseIf (Me.HasHome(boundLocal)) Then
				If (boundLocal.IsConstant) Then
					Dim type As TypeSymbol = boundLocal.Type
					If (type.IsDecimalType() OrElse type.IsDateTimeType()) Then
						GoTo Label1
					End If
					flag = False
					Return flag
				End If
			Label1:
				flag = True
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function AllowedToTakeRef(ByVal fieldAccess As BoundFieldAccess, ByVal addressKind As CodeGenerator.AddressKind) As Boolean
			Dim flag As Boolean
			If (addressKind <> CodeGenerator.AddressKind.Immutable) Then
				If (Not Me.HasHome(fieldAccess)) Then
					flag = False
					Return flag
				ElseIf (fieldAccess.FieldSymbol.ContainingType.IsValueType) Then
					Dim receiverOpt As BoundExpression = fieldAccess.ReceiverOpt
					If (receiverOpt Is Nothing OrElse Me.AllowedToTakeRef(receiverOpt, CodeGenerator.AddressKind.[ReadOnly])) Then
						flag = Me.HasHome(fieldAccess)
						Return flag
					End If
					If (Me.HasHome(receiverOpt)) Then
						flag = False
						Return flag
					Else
						flag = True
						Return flag
					End If
				End If
			End If
			flag = Me.HasHome(fieldAccess)
			Return flag
		End Function

		Private Function AsConstOrDefault(ByVal init As BoundExpression) As Microsoft.CodeAnalysis.ConstantValue
			Dim constantValue As Microsoft.CodeAnalysis.ConstantValue
			Dim constantValueOpt As Microsoft.CodeAnalysis.ConstantValue = init.ConstantValueOpt
			constantValue = If(constantValueOpt Is Nothing, Microsoft.CodeAnalysis.ConstantValue.[Default](init.Type.GetEnumUnderlyingTypeOrSelf().SpecialType), constantValueOpt)
			Return constantValue
		End Function

		Private Function CanPassToBrfalse(ByVal ts As TypeSymbol) As Boolean
			Dim flag As Boolean
			If (Not ts.IsEnumType()) Then
				Dim primitiveTypeCode As Microsoft.Cci.PrimitiveTypeCode = ts.PrimitiveTypeCode
				If (CInt(primitiveTypeCode) - CInt(Microsoft.Cci.PrimitiveTypeCode.Float32) <= CInt(Microsoft.Cci.PrimitiveTypeCode.[Char])) Then
					flag = False
				Else
					flag = If(primitiveTypeCode = Microsoft.Cci.PrimitiveTypeCode.NotPrimitive, ts.IsReferenceType, True)
				End If
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Function CanUseCallOnRefTypeReceiver(ByVal receiver As BoundExpression) As Boolean
			Dim isCapturedFrame As Boolean
			If (Not receiver.Type.IsTypeParameter()) Then
				Dim constantValueOpt As ConstantValue = receiver.ConstantValueOpt
				If (constantValueOpt Is Nothing) Then
					Dim kind As BoundKind = receiver.Kind
					If (kind > BoundKind.ObjectCreationExpression) Then
						If (kind > BoundKind.MeReference) Then
							If (CByte(kind) - CByte(BoundKind.MyBaseReference) <= CByte(BoundKind.OmittedArgument)) Then
								isCapturedFrame = True
								Return isCapturedFrame
							End If
							If (kind <> BoundKind.ConditionalAccessReceiverPlaceholder AndAlso kind <> BoundKind.ComplexConditionalAccessReceiver) Then
								GoTo Label2
							End If
							isCapturedFrame = True
							Return isCapturedFrame
						Else
							Select Case kind
								Case BoundKind.DelegateCreationExpression
									isCapturedFrame = True
									Return isCapturedFrame
								Case BoundKind.ArrayCreation
									isCapturedFrame = True
									Return isCapturedFrame
								Case BoundKind.ArrayLiteral
								Case BoundKind.ArrayInitialization
									GoTo Label2
								Case BoundKind.FieldAccess
									isCapturedFrame = DirectCast(receiver, BoundFieldAccess).FieldSymbol.IsCapturedFrame
									Return isCapturedFrame
								Case Else
									If (kind = BoundKind.Sequence) Then
										Dim valueOpt As BoundExpression = DirectCast(receiver, BoundSequence).ValueOpt
										isCapturedFrame = If(valueOpt Is Nothing, False, Me.CanUseCallOnRefTypeReceiver(valueOpt))
										Return isCapturedFrame
									Else
										If (kind = BoundKind.MeReference) Then
											Exit Select
										End If
										GoTo Label2
									End If
							End Select
						End If
						isCapturedFrame = True
						Return isCapturedFrame
					ElseIf (kind > BoundKind.AssignmentOperator) Then
						If (kind = BoundKind.AddressOfOperator) Then
							isCapturedFrame = True
							Return isCapturedFrame
						End If
						If (kind = BoundKind.[DirectCast]) Then
							Dim operand As BoundExpression = DirectCast(receiver, BoundDirectCast).Operand
							If (operand.Type.IsVerifierReference()) Then
								isCapturedFrame = Me.CanUseCallOnRefTypeReceiver(operand)
								Return isCapturedFrame
							Else
								isCapturedFrame = True
								Return isCapturedFrame
							End If
						Else
							If (kind <> BoundKind.ObjectCreationExpression) Then
								GoTo Label2
							End If
							isCapturedFrame = True
							Return isCapturedFrame
						End If
					ElseIf (kind = BoundKind.[GetType]) Then
						isCapturedFrame = True
						Return isCapturedFrame
					Else
						If (kind <> BoundKind.AssignmentOperator) Then
							GoTo Label2
						End If
						isCapturedFrame = Me.CanUseCallOnRefTypeReceiver(DirectCast(receiver, BoundAssignmentOperator).Right)
						Return isCapturedFrame
					End If
				Label2:
					isCapturedFrame = False
				Else
					isCapturedFrame = Not constantValueOpt.IsNothing
				End If
			Else
				isCapturedFrame = False
			End If
			Return isCapturedFrame
		End Function

		Private Function CodeForJump(ByVal expression As BoundBinaryOperator, ByVal sense As Boolean, <Out> ByRef revOpCode As ILOpCode) As ILOpCode
			Dim sCondJumpOpCodes As ILOpCode
			Dim num As Integer
			Dim operatorKind As BinaryOperatorKind = expression.OperatorKind And BinaryOperatorKind.OpMask
			Dim type As TypeSymbol = expression.Left.Type
			If (type IsNot Nothing AndAlso type.IsBooleanType()) Then
				Select Case operatorKind
					Case BinaryOperatorKind.LessThanOrEqual
						operatorKind = BinaryOperatorKind.GreaterThanOrEqual
						Exit Select
					Case BinaryOperatorKind.GreaterThanOrEqual
						operatorKind = BinaryOperatorKind.LessThanOrEqual
						Exit Select
					Case BinaryOperatorKind.LessThan
						operatorKind = BinaryOperatorKind.GreaterThan
						Exit Select
					Case BinaryOperatorKind.GreaterThan
						operatorKind = BinaryOperatorKind.LessThan
						Exit Select
				End Select
			End If
			Select Case operatorKind
				Case BinaryOperatorKind.Equals
					sCondJumpOpCodes = If(sense, ILOpCode.Beq, ILOpCode.Bne_un)
					Return sCondJumpOpCodes
				Case BinaryOperatorKind.NotEquals
					sCondJumpOpCodes = If(sense, ILOpCode.Bne_un, ILOpCode.Beq)
					Return sCondJumpOpCodes
				Case BinaryOperatorKind.LessThanOrEqual
					num = 1
					Exit Select
				Case BinaryOperatorKind.GreaterThanOrEqual
					num = 3
					Exit Select
				Case BinaryOperatorKind.LessThan
					num = 0
					Exit Select
				Case BinaryOperatorKind.GreaterThan
					num = 2
					Exit Select
				Case Else
					If (operatorKind = BinaryOperatorKind.[Is]) Then
						sCondJumpOpCodes = If(sense, ILOpCode.Beq, ILOpCode.Bne_un)
						Return sCondJumpOpCodes
					Else
						If (operatorKind <> BinaryOperatorKind.[IsNot]) Then
							Throw ExceptionUtilities.UnexpectedValue(operatorKind)
						End If
						sCondJumpOpCodes = If(sense, ILOpCode.Bne_un, ILOpCode.Beq)
						Return sCondJumpOpCodes
					End If
			End Select
			If (type IsNot Nothing) Then
				If (type.IsUnsignedIntegralType()) Then
					num += 8
				ElseIf (type.IsFloatingType()) Then
					num += 16
				End If
			End If
			Dim num1 As Integer = num
			If (sense) Then
				num1 += 4
			Else
				num += 4
			End If
			revOpCode = CodeGenerator.s_condJumpOpCodes(num1)
			sCondJumpOpCodes = CodeGenerator.s_condJumpOpCodes(num)
			Return sCondJumpOpCodes
		End Function

		Private Function ConversionHasSideEffects(ByVal conversion As BoundConversion) As Boolean
			Return True
		End Function

		Private Function ConversionHasSideEffects(ByVal conversion As BoundDirectCast) As Boolean
			Return True
		End Function

		Private Function ConversionHasSideEffects(ByVal conversion As BoundTryCast) As Boolean
			Return False
		End Function

		Private Function CreateCaseBlockLabels(ByVal caseBlocks As ImmutableArray(Of BoundCaseBlock)) As ImmutableArray(Of GeneratedLabelSymbol)
			Dim instance As ArrayBuilder(Of GeneratedLabelSymbol) = ArrayBuilder(Of GeneratedLabelSymbol).GetInstance(caseBlocks.Length)
			Dim num As Integer = 0
			Dim enumerator As ImmutableArray(Of BoundCaseBlock).Enumerator = caseBlocks.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As BoundCaseBlock = enumerator.Current
				num = num + 1
				instance.Add(New GeneratedLabelSymbol([String].Concat("Case Block ", num.ToString())))
			End While
			Return instance.ToImmutableAndFree()
		End Function

		Private Function DefineLocal(ByVal local As LocalSymbol, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode) As Microsoft.CodeAnalysis.CodeGen.LocalDefinition
			Dim localDefinition As Microsoft.CodeAnalysis.CodeGen.LocalDefinition
			Dim empty As ImmutableArray(Of String)
			Dim flags As ImmutableArray(Of Boolean) = ImmutableArray(Of Boolean).Empty
			If (local.IsCompilerGenerated OrElse Not local.Type.ContainsTupleNames()) Then
				empty = ImmutableArray(Of String).Empty
			Else
				empty = VisualBasicCompilation.TupleNamesEncoder.Encode(local.Type)
			End If
			Dim strs As ImmutableArray(Of String) = empty
			If (local.HasConstantValue) Then
				Dim metadataConstant As Microsoft.CodeAnalysis.CodeGen.MetadataConstant = Me._module.CreateConstant(local.Type, RuntimeHelpers.GetObjectValue(local.ConstantValue), syntaxNode, Me._diagnostics)
				Dim localConstantDefinition As Microsoft.CodeAnalysis.CodeGen.LocalConstantDefinition = New Microsoft.CodeAnalysis.CodeGen.LocalConstantDefinition(local.Name, If(System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of Location)(local.Locations), Location.None), metadataConstant, flags, strs)
				Me._builder.AddLocalConstantToScope(localConstantDefinition)
				localDefinition = Nothing
			ElseIf (Not Me.IsStackLocal(local)) Then
				Dim typeReference As ITypeReference = Me._module.Translate(local.Type, syntaxNode, Me._diagnostics)
				Me._module.GetFakeSymbolTokenForIL(typeReference, syntaxNode, Me._diagnostics)
				Dim localSlotConstraint As LocalSlotConstraints = If(local.IsByRef, 1, 0) Or If(local.IsPinned, 2, 0)
				Dim localDebugId As Microsoft.CodeAnalysis.CodeGen.LocalDebugId = New Microsoft.CodeAnalysis.CodeGen.LocalDebugId()
				Dim localDebugName As String = Me.GetLocalDebugName(local, localDebugId)
				Dim synthesizedKind As SynthesizedLocalKind = local.SynthesizedKind
				Dim localDefinition1 As Microsoft.CodeAnalysis.CodeGen.LocalDefinition = Me._builder.LocalSlotManager.DeclareLocal(typeReference, local, localDebugName, synthesizedKind, localDebugId, synthesizedKind.PdbAttributes(), localSlotConstraint, flags, strs, synthesizedKind.IsSlotReusable(Me._ilEmitStyle <> ILEmitStyle.Release))
				If (localDefinition1.Name IsNot Nothing) Then
					Me._builder.AddLocalToScope(localDefinition1)
				End If
				localDefinition = localDefinition1
			Else
				localDefinition = Nothing
			End If
			Return localDefinition
		End Function

		Private Sub DefineUserDefinedStateMachineHoistedLocal(ByVal field As StateMachineFieldSymbol)
			If (Me._module.DebugInformationFormat <> DebugInformationFormat.Pdb) Then
				Me._builder.DefineUserDefinedStateMachineHoistedLocal(field.SlotIndex)
				Return
			End If
			Dim lBuilder As ILBuilder = Me._builder
			Dim name As String = field.Name
			Dim slotIndex As Integer = field.SlotIndex
			Dim localDebugId As Microsoft.CodeAnalysis.CodeGen.LocalDebugId = New Microsoft.CodeAnalysis.CodeGen.LocalDebugId()
			Dim flags As ImmutableArray(Of Boolean) = New ImmutableArray(Of Boolean)()
			Dim strs As ImmutableArray(Of String) = New ImmutableArray(Of String)()
			lBuilder.AddLocalToScope(New LocalDefinition(Nothing, name, Nothing, slotIndex, SynthesizedLocalKind.EmitterTemp, localDebugId, LocalVariableAttributes.None, LocalSlotConstraints.None, flags, strs))
		End Sub

		Private Function DigForLocal(ByVal value As BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundLocal
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal
			Dim kind As BoundKind = value.Kind
			If (kind = BoundKind.FieldAccess) Then
				Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = DirectCast(value, Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess)
				If (boundFieldAccess.FieldSymbol.IsShared) Then
					boundLocal = Nothing
					Return boundLocal
				End If
				boundLocal = Me.DigForLocal(boundFieldAccess.ReceiverOpt)
				Return boundLocal
			ElseIf (kind = BoundKind.Sequence) Then
				boundLocal = Me.DigForLocal(DirectCast(value, BoundSequence).ValueOpt)
				Return boundLocal
			ElseIf (kind = BoundKind.Local) Then
				Dim boundLocal1 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = DirectCast(value, Microsoft.CodeAnalysis.VisualBasic.BoundLocal)
				If (boundLocal1.LocalSymbol.IsByRef) Then
					boundLocal = Nothing
					Return boundLocal
				End If
				boundLocal = boundLocal1
				Return boundLocal
			End If
			boundLocal = Nothing
			Return boundLocal
		End Function

		Private Sub DowncastResultOfArithmeticOperation(ByVal targetPrimitiveType As Microsoft.Cci.PrimitiveTypeCode, ByVal isChecked As Boolean)
			If (targetPrimitiveType = Microsoft.Cci.PrimitiveTypeCode.Int8 OrElse targetPrimitiveType = Microsoft.Cci.PrimitiveTypeCode.UInt8 OrElse targetPrimitiveType = Microsoft.Cci.PrimitiveTypeCode.Int16 OrElse targetPrimitiveType = Microsoft.Cci.PrimitiveTypeCode.UInt16) Then
				Me._builder.EmitNumericConversion(If(targetPrimitiveType.IsUnsigned(), Microsoft.Cci.PrimitiveTypeCode.UInt32, Microsoft.Cci.PrimitiveTypeCode.Int32), targetPrimitiveType, isChecked)
			End If
		End Sub

		Private Function EmitAddress(ByVal expression As BoundExpression, ByVal addressKind As CodeGenerator.AddressKind) As Microsoft.CodeAnalysis.CodeGen.LocalDefinition
			Dim localDefinition As Microsoft.CodeAnalysis.CodeGen.LocalDefinition
			Dim kind As BoundKind = expression.Kind
			Dim localDefinition1 As Microsoft.CodeAnalysis.CodeGen.LocalDefinition = Nothing
			If (Me.AllowedToTakeRef(expression, addressKind)) Then
				If (kind <= BoundKind.SequencePointExpression) Then
					If (kind <= BoundKind.Parenthesized) Then
						If (kind = BoundKind.Dup) Then
							Me._builder.EmitOpCode(ILOpCode.Dup)
						Else
							If (kind = BoundKind.Parenthesized) Then
								GoTo Label3
							End If
							Throw ExceptionUtilities.UnexpectedValue(kind)
						End If
					ElseIf (kind = BoundKind.ArrayAccess) Then
						Me.EmitArrayElementAddress(DirectCast(expression, BoundArrayAccess), addressKind)
					ElseIf (kind = BoundKind.ReferenceAssignment) Then
						Me.EmitReferenceAssignment(DirectCast(expression, BoundReferenceAssignment), True, True)
					Else
						If (kind <> BoundKind.SequencePointExpression) Then
							Throw ExceptionUtilities.UnexpectedValue(kind)
						End If
						Me.EmitSequencePointExpressionAddress(DirectCast(expression, BoundSequencePointExpression), addressKind)
					End If
				ElseIf (kind > BoundKind.Sequence) Then
					Select Case kind
						Case BoundKind.MeReference
						Case BoundKind.MyClassReference
							Me._builder.EmitOpCode(ILOpCode.Ldarg_0)
							Exit Select
						Case BoundKind.ValueTypeMeReference
							Me._builder.EmitOpCode(ILOpCode.Ldarg_0)
							Exit Select
						Case BoundKind.MyBaseReference
							Exit Select
						Case BoundKind.PreviousSubmissionReference
						Case BoundKind.HostObjectMemberReference
							Throw ExceptionUtilities.UnexpectedValue(kind)
						Case BoundKind.Local
							Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = DirectCast(expression, Microsoft.CodeAnalysis.VisualBasic.BoundLocal)
							If (Me.IsStackLocal(boundLocal.LocalSymbol)) Then
								Exit Select
							End If
							Dim local As Microsoft.CodeAnalysis.CodeGen.LocalDefinition = Me.GetLocal(boundLocal)
							Me._builder.EmitLocalAddress(local)
							Exit Select
						Case BoundKind.PseudoVariable
							Me.EmitPseudoVariableAddress(DirectCast(expression, BoundPseudoVariable))
							Exit Select
						Case BoundKind.Parameter
							Me.EmitParameterAddress(DirectCast(expression, BoundParameter))
							Exit Select
						Case Else
							If (kind = BoundKind.ConditionalAccessReceiverPlaceholder) Then
								Exit Select
							End If
							If (kind = BoundKind.ComplexConditionalAccessReceiver) Then
								Me.EmitComplexConditionalAccessReceiverAddress(DirectCast(expression, BoundComplexConditionalAccessReceiver))
								Exit Select
							Else
								Throw ExceptionUtilities.UnexpectedValue(kind)
							End If
					End Select
				ElseIf (kind = BoundKind.[Call]) Then
					Me.EmitCallExpression(DirectCast(expression, BoundCall), CodeGenerator.UseKind.UsedAsAddress)
				ElseIf (kind = BoundKind.FieldAccess) Then
					localDefinition1 = Me.EmitFieldAddress(DirectCast(expression, BoundFieldAccess), addressKind)
				Else
					If (kind <> BoundKind.Sequence) Then
						Throw ExceptionUtilities.UnexpectedValue(kind)
					End If
					localDefinition1 = Me.EmitSequenceAddress(DirectCast(expression, BoundSequence), addressKind)
				End If
			Label3:
				localDefinition = localDefinition1
			Else
				localDefinition = Me.EmitAddressOfTempClone(expression)
			End If
			Return localDefinition
		End Function

		Private Function EmitAddressOfTempClone(ByVal expression As BoundExpression) As Microsoft.CodeAnalysis.CodeGen.LocalDefinition
			Me.EmitExpression(expression, True)
			Dim localDefinition As Microsoft.CodeAnalysis.CodeGen.LocalDefinition = Me.AllocateTemp(expression.Type, expression.Syntax)
			Me._builder.EmitLocalStore(localDefinition)
			Me._builder.EmitLocalAddress(localDefinition)
			Return localDefinition
		End Function

		Private Sub EmitAllElementInitializersRecursive(ByVal arrayType As ArrayTypeSymbol, ByVal indices As ArrayBuilder(Of CodeGenerator.IndexDesc), ByVal includeConstants As Boolean)
			Dim initializers As ImmutableArray(Of BoundExpression) = indices.Peek().Initializers
			If (Not Me.IsMultidimensionalInitializer(initializers)) Then
				Dim length As Integer = initializers.Length - 1
				For i As Integer = 0 To length
					Dim item As BoundExpression = initializers(i)
					If (CodeGenerator.ShouldEmitInitExpression(includeConstants, item)) Then
						Me._builder.EmitOpCode(ILOpCode.Dup)
						Dim enumerator As ArrayBuilder(Of CodeGenerator.IndexDesc).Enumerator = indices.GetEnumerator()
						While enumerator.MoveNext()
							Dim current As CodeGenerator.IndexDesc = enumerator.Current
							Me._builder.EmitIntConstant(current.Index)
						End While
						Me._builder.EmitIntConstant(i)
						Me.EmitExpression(initializers(i), True)
						Me.EmitArrayElementStore(arrayType, item.Syntax)
					End If
				Next

			Else
				Dim num As Integer = initializers.Length - 1
				For j As Integer = 0 To num
					indices.Push(New CodeGenerator.IndexDesc(j, DirectCast(initializers(j), BoundArrayInitialization).Initializers))
					Me.EmitAllElementInitializersRecursive(arrayType, indices, includeConstants)
				Next

			End If
			indices.Pop()
		End Sub

		Private Sub EmitArguments(ByVal arguments As ImmutableArray(Of BoundExpression), ByVal parameters As ImmutableArray(Of ParameterSymbol))
			Dim length As Integer = arguments.Length - 1
			For i As Integer = 0 To length
				Dim item As BoundExpression = arguments(i)
				If (Not parameters(i).IsByRef) Then
					Me.EmitExpression(item, True)
				Else
					Me.EmitAddress(item, CodeGenerator.AddressKind.Writeable)
				End If
			Next

		End Sub

		Private Sub EmitArrayCreationExpression(ByVal expression As BoundArrayCreation, ByVal used As Boolean)
			Dim type As ArrayTypeSymbol = DirectCast(expression.Type, ArrayTypeSymbol)
			Me.EmitExpressions(expression.Bounds, True)
			If (Not type.IsSZArray) Then
				Me._builder.EmitArrayCreation(Me._module.Translate(type), expression.Syntax, Me._diagnostics)
			Else
				Me._builder.EmitOpCode(ILOpCode.Newarr)
				Me.EmitSymbolToken(type.ElementType, expression.Syntax)
			End If
			If (expression.InitializerOpt IsNot Nothing) Then
				Me.EmitArrayInitializers(type, expression.InitializerOpt)
			End If
			Me.EmitPopIfUnused(used)
		End Sub

		Private Sub EmitArrayElementAddress(ByVal arrayAccess As BoundArrayAccess, ByVal addressKind As CodeGenerator.AddressKind)
			Me.EmitExpression(arrayAccess.Expression, True)
			Me.EmitExpressions(arrayAccess.Indices, True)
			Dim type As TypeSymbol = arrayAccess.Type
			If (addressKind <> CodeGenerator.AddressKind.Writeable AndAlso type.IsTypeParameter()) Then
				Me._builder.EmitOpCode(ILOpCode.[Readonly])
			End If
			If (DirectCast(arrayAccess.Expression.Type, ArrayTypeSymbol).IsSZArray) Then
				Me._builder.EmitOpCode(ILOpCode.Ldelema)
				Me.EmitSymbolToken(type, arrayAccess.Syntax)
				Return
			End If
			Me._builder.EmitArrayElementAddress(Me._module.Translate(DirectCast(arrayAccess.Expression.Type, ArrayTypeSymbol)), arrayAccess.Syntax, Me._diagnostics)
		End Sub

		Private Sub EmitArrayElementLoad(ByVal arrayAccess As BoundArrayAccess, ByVal used As Boolean)
			Me.EmitExpression(arrayAccess.Expression, True)
			Me.EmitExpressions(arrayAccess.Indices, True)
			If (Not DirectCast(arrayAccess.Expression.Type, ArrayTypeSymbol).IsSZArray) Then
				Me._builder.EmitArrayElementLoad(Me._module.Translate(DirectCast(arrayAccess.Expression.Type, ArrayTypeSymbol)), arrayAccess.Expression.Syntax, Me._diagnostics)
			Else
				Dim type As TypeSymbol = arrayAccess.Type
				If (type.IsEnumType()) Then
					type = DirectCast(type, NamedTypeSymbol).EnumUnderlyingType
				End If
				Select Case type.PrimitiveTypeCode
					Case Microsoft.Cci.PrimitiveTypeCode.[Boolean]
					Case Microsoft.Cci.PrimitiveTypeCode.UInt8
						Me._builder.EmitOpCode(ILOpCode.Ldelem_u1)
						Exit Select
					Case Microsoft.Cci.PrimitiveTypeCode.[Char]
					Case Microsoft.Cci.PrimitiveTypeCode.UInt16
						Me._builder.EmitOpCode(ILOpCode.Ldelem_u2)
						Exit Select
					Case Microsoft.Cci.PrimitiveTypeCode.Int8
						Me._builder.EmitOpCode(ILOpCode.Ldelem_i1)
						Exit Select
					Case Microsoft.Cci.PrimitiveTypeCode.Float32
						Me._builder.EmitOpCode(ILOpCode.Ldelem_r4)
						Exit Select
					Case Microsoft.Cci.PrimitiveTypeCode.Float64
						Me._builder.EmitOpCode(ILOpCode.Ldelem_r8)
						Exit Select
					Case Microsoft.Cci.PrimitiveTypeCode.Int16
						Me._builder.EmitOpCode(ILOpCode.Ldelem_i2)
						Exit Select
					Case Microsoft.Cci.PrimitiveTypeCode.Int32
						Me._builder.EmitOpCode(ILOpCode.Ldelem_i4)
						Exit Select
					Case Microsoft.Cci.PrimitiveTypeCode.Int64
					Case Microsoft.Cci.PrimitiveTypeCode.UInt64
						Me._builder.EmitOpCode(ILOpCode.Ldelem_i8)
						Exit Select
					Case Microsoft.Cci.PrimitiveTypeCode.IntPtr
					Case Microsoft.Cci.PrimitiveTypeCode.Pointer
					Case Microsoft.Cci.PrimitiveTypeCode.UIntPtr
						Me._builder.EmitOpCode(ILOpCode.Ldelem_i)
						Exit Select
					Case Microsoft.Cci.PrimitiveTypeCode.Reference
					Case Microsoft.Cci.PrimitiveTypeCode.[String]
					Label0:
						If (Not type.IsVerifierReference()) Then
							If (Not used) Then
								If (type.TypeKind = Microsoft.CodeAnalysis.TypeKind.TypeParameter) Then
									Me._builder.EmitOpCode(ILOpCode.[Readonly])
								End If
								Me._builder.EmitOpCode(ILOpCode.Ldelema)
							Else
								Me._builder.EmitOpCode(ILOpCode.Ldelem)
							End If
							Me.EmitSymbolToken(type, arrayAccess.Expression.Syntax)
							Exit Select
						Else
							Me._builder.EmitOpCode(ILOpCode.Ldelem_ref)
							Exit Select
						End If
					Case Microsoft.Cci.PrimitiveTypeCode.UInt32
						Me._builder.EmitOpCode(ILOpCode.Ldelem_u4)
						Exit Select
					Case Else
						GoTo Label0
				End Select
			End If
			Me.EmitPopIfUnused(used)
		End Sub

		Private Sub EmitArrayElementStore(ByVal arrayType As ArrayTypeSymbol, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode)
			If (arrayType.IsSZArray) Then
				Me.EmitVectorElementStore(arrayType, syntaxNode)
				Return
			End If
			Me._builder.EmitArrayElementStore(Me._module.Translate(arrayType), syntaxNode, Me._diagnostics)
		End Sub

		Private Sub EmitArrayInitializers(ByVal arrayType As ArrayTypeSymbol, ByVal inits As BoundArrayInitialization)
			Dim initializers As ImmutableArray(Of BoundExpression) = inits.Initializers
			Dim arrayInitializerStyle As CodeGenerator.ArrayInitializerStyle = Me.ShouldEmitBlockInitializer(arrayType.ElementType, initializers)
			If (arrayInitializerStyle = CodeGenerator.ArrayInitializerStyle.Element) Then
				Me.EmitElementInitializers(arrayType, initializers, True)
				Return
			End If
			Me._builder.EmitArrayBlockInitializer(Me.GetRawData(initializers), inits.Syntax, Me._diagnostics)
			If (arrayInitializerStyle = CodeGenerator.ArrayInitializerStyle.Mixed) Then
				Me.EmitElementInitializers(arrayType, initializers, False)
			End If
		End Sub

		Private Sub EmitArrayLengthExpression(ByVal expression As BoundArrayLength, ByVal used As Boolean)
			Me.EmitExpression(expression.Expression, True)
			Me._builder.EmitOpCode(ILOpCode.Ldlen)
			Dim primitiveTypeCode As Microsoft.Cci.PrimitiveTypeCode = expression.Type.PrimitiveTypeCode
			Me._builder.EmitNumericConversion(If(primitiveTypeCode.IsUnsigned(), Microsoft.Cci.PrimitiveTypeCode.UIntPtr, Microsoft.Cci.PrimitiveTypeCode.IntPtr), primitiveTypeCode, False)
			Me.EmitPopIfUnused(used)
		End Sub

		Private Function EmitAssignmentDuplication(ByVal assignmentOperator As BoundAssignmentOperator, ByVal used As Boolean, ByVal lhsUsesStack As Boolean) As Microsoft.CodeAnalysis.CodeGen.LocalDefinition
			Dim localDefinition As Microsoft.CodeAnalysis.CodeGen.LocalDefinition = Nothing
			If (used) Then
				Me._builder.EmitOpCode(ILOpCode.Dup)
				If (lhsUsesStack) Then
					localDefinition = Me.AllocateTemp(assignmentOperator.Left.Type, assignmentOperator.Left.Syntax)
					Me._builder.EmitLocalStore(localDefinition)
				End If
			End If
			Return localDefinition
		End Function

		Private Sub EmitAssignmentExpression(ByVal assignmentOperator As BoundAssignmentOperator, ByVal used As Boolean)
			If (Not Me.TryEmitAssignmentInPlace(assignmentOperator, used)) Then
				Dim flag As Boolean = Me.EmitAssignmentPreamble(assignmentOperator.Left)
				Me.EmitExpression(assignmentOperator.Right, True)
				Dim localDefinition As Microsoft.CodeAnalysis.CodeGen.LocalDefinition = Me.EmitAssignmentDuplication(assignmentOperator, used, flag)
				Me.EmitStore(assignmentOperator.Left)
				Me.EmitAssignmentPostfix(localDefinition)
			End If
		End Sub

		Private Sub EmitAssignmentPostfix(ByVal temp As LocalDefinition)
			If (temp IsNot Nothing) Then
				Me._builder.EmitLocalLoad(temp)
				Me.FreeTemp(temp)
			End If
		End Sub

		Private Function EmitAssignmentPreamble(ByVal assignmentTarget As BoundExpression) As Boolean
			Dim flag As Boolean = False
			Dim kind As BoundKind = assignmentTarget.Kind
			If (kind <= BoundKind.[Call]) Then
				If (kind <= BoundKind.ModuleVersionId) Then
					If (kind = BoundKind.ArrayAccess) Then
						Dim boundArrayAccess As Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess = DirectCast(assignmentTarget, Microsoft.CodeAnalysis.VisualBasic.BoundArrayAccess)
						Me.EmitExpression(boundArrayAccess.Expression, True)
						Me.EmitExpressions(boundArrayAccess.Indices, True)
						flag = True
					Else
						If (CByte(kind) - CByte(BoundKind.InstrumentationPayloadRoot) <= CByte(BoundKind.OmittedArgument)) Then
							Return flag
						End If
						Throw ExceptionUtilities.UnexpectedValue(assignmentTarget.Kind)
					End If
				ElseIf (kind = BoundKind.ReferenceAssignment) Then
					Me.EmitReferenceAssignment(DirectCast(assignmentTarget, BoundReferenceAssignment), True, True)
					flag = True
				Else
					If (kind <> BoundKind.[Call]) Then
						Throw ExceptionUtilities.UnexpectedValue(assignmentTarget.Kind)
					End If
					Me.EmitCallExpression(DirectCast(assignmentTarget, BoundCall), CodeGenerator.UseKind.UsedAsAddress)
					flag = True
				End If
			ElseIf (kind <= BoundKind.Sequence) Then
				If (kind = BoundKind.FieldAccess) Then
					Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = DirectCast(assignmentTarget, Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess)
					If (Not boundFieldAccess.FieldSymbol.IsShared) Then
						Me.EmitReceiverRef(boundFieldAccess.ReceiverOpt, False, CodeGenerator.AddressKind.[ReadOnly])
						flag = True
					End If
				Else
					If (kind <> BoundKind.Sequence) Then
						Throw ExceptionUtilities.UnexpectedValue(assignmentTarget.Kind)
					End If
					Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSequence = DirectCast(assignmentTarget, Microsoft.CodeAnalysis.VisualBasic.BoundSequence)
					If (Not boundSequence.Locals.IsEmpty) Then
						Me._builder.OpenLocalScope(ScopeType.Variable, Nothing)
						Dim enumerator As ImmutableArray(Of LocalSymbol).Enumerator = boundSequence.Locals.GetEnumerator()
						While enumerator.MoveNext()
							Me.DefineLocal(enumerator.Current, boundSequence.Syntax)
						End While
					End If
					Me.EmitSideEffects(boundSequence.SideEffects)
					flag = Me.EmitAssignmentPreamble(boundSequence.ValueOpt)
				End If
			ElseIf (kind = BoundKind.MeReference) Then
				Me.EmitAddress(DirectCast(assignmentTarget, BoundMeReference), CodeGenerator.AddressKind.Writeable)
				flag = True
			Else
				Select Case kind
					Case BoundKind.Local
						Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = DirectCast(assignmentTarget, Microsoft.CodeAnalysis.VisualBasic.BoundLocal)
						If (Not boundLocal.LocalSymbol.IsByRef) Then
							Exit Select
						End If
						If (Not Me.IsStackLocal(boundLocal.LocalSymbol)) Then
							Me._builder.EmitLocalLoad(Me.GetLocal(boundLocal))
						End If
						flag = True
						Exit Select
					Case BoundKind.PseudoVariable
						Me.EmitPseudoVariableAddress(DirectCast(assignmentTarget, BoundPseudoVariable))
						flag = True
						Exit Select
					Case BoundKind.Parameter
						Dim boundParameter As Microsoft.CodeAnalysis.VisualBasic.BoundParameter = DirectCast(assignmentTarget, Microsoft.CodeAnalysis.VisualBasic.BoundParameter)
						If (Not boundParameter.ParameterSymbol.IsByRef) Then
							Exit Select
						End If
						Me._builder.EmitLoadArgumentOpcode(Me.ParameterSlot(boundParameter))
						flag = True
						Exit Select
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(assignmentTarget.Kind)
				End Select
			End If
			Return flag
		End Function

		Private Sub EmitBinaryArithOperator(ByVal expression As BoundBinaryOperator)
			Me.EmitExpression(expression.Left, True)
			Me.EmitExpression(expression.Right, True)
			Me.EmitBinaryArithOperatorInstructionAndDowncast(expression)
		End Sub

		Private Sub EmitBinaryArithOperatorInstructionAndDowncast(ByVal expression As BoundBinaryOperator)
			Dim primitiveTypeCode As Microsoft.Cci.PrimitiveTypeCode = expression.Type.PrimitiveTypeCode
			Dim operatorKind As BinaryOperatorKind = expression.OperatorKind And BinaryOperatorKind.OpMask
			If (operatorKind <> BinaryOperatorKind.Add) Then
				Select Case operatorKind
					Case BinaryOperatorKind.Subtract
						If (expression.Checked AndAlso (primitiveTypeCode = Microsoft.Cci.PrimitiveTypeCode.Int32 OrElse primitiveTypeCode = Microsoft.Cci.PrimitiveTypeCode.Int64)) Then
							Me._builder.EmitOpCode(ILOpCode.Sub_ovf)
							Exit Select
						ElseIf (Not expression.Checked OrElse primitiveTypeCode <> Microsoft.Cci.PrimitiveTypeCode.UInt32 AndAlso primitiveTypeCode <> Microsoft.Cci.PrimitiveTypeCode.UInt64) Then
							Me._builder.EmitOpCode(ILOpCode.[Sub])
							Exit Select
						Else
							Me._builder.EmitOpCode(ILOpCode.Sub_ovf_un)
							Exit Select
						End If
					Case BinaryOperatorKind.Multiply
						If (expression.Checked AndAlso (primitiveTypeCode = Microsoft.Cci.PrimitiveTypeCode.Int32 OrElse primitiveTypeCode = Microsoft.Cci.PrimitiveTypeCode.Int64)) Then
							Me._builder.EmitOpCode(ILOpCode.Mul_ovf)
							Exit Select
						ElseIf (Not expression.Checked OrElse primitiveTypeCode <> Microsoft.Cci.PrimitiveTypeCode.UInt32 AndAlso primitiveTypeCode <> Microsoft.Cci.PrimitiveTypeCode.UInt64) Then
							Me._builder.EmitOpCode(ILOpCode.Mul)
							Exit Select
						Else
							Me._builder.EmitOpCode(ILOpCode.Mul_ovf_un)
							Exit Select
						End If
					Case BinaryOperatorKind.Power
						Throw ExceptionUtilities.UnexpectedValue(operatorKind)
					Case BinaryOperatorKind.Divide
					Case BinaryOperatorKind.IntegerDivide
						If (Not primitiveTypeCode.IsUnsigned()) Then
							Me._builder.EmitOpCode(ILOpCode.Div)
							Exit Select
						Else
							Me._builder.EmitOpCode(ILOpCode.Div_un)
							Exit Select
						End If
					Case BinaryOperatorKind.Modulo
						If (Not primitiveTypeCode.IsUnsigned()) Then
							Me._builder.EmitOpCode(ILOpCode.[Rem])
							Exit Select
						Else
							Me._builder.EmitOpCode(ILOpCode.Rem_un)
							Exit Select
						End If
					Case BinaryOperatorKind.LeftShift
						Dim shiftSizeMask As Integer = CodeGenerator.GetShiftSizeMask(expression.Left.Type)
						Dim constantValueOpt As Microsoft.CodeAnalysis.ConstantValue = expression.Right.ConstantValueOpt
						If (constantValueOpt Is Nothing OrElse CULng(constantValueOpt.UInt32Value) > CLng(shiftSizeMask)) Then
							Me._builder.EmitConstantValue(Microsoft.CodeAnalysis.ConstantValue.Create(shiftSizeMask))
							Me._builder.EmitOpCode(ILOpCode.[And])
						End If
						Me._builder.EmitOpCode(ILOpCode.Shl)
						Exit Select
					Case BinaryOperatorKind.RightShift
						Dim num As Integer = CodeGenerator.GetShiftSizeMask(expression.Left.Type)
						Dim constantValue As Microsoft.CodeAnalysis.ConstantValue = expression.Right.ConstantValueOpt
						If (constantValue Is Nothing OrElse CULng(constantValue.UInt32Value) > CLng(num)) Then
							Me._builder.EmitConstantValue(Microsoft.CodeAnalysis.ConstantValue.Create(num))
							Me._builder.EmitOpCode(ILOpCode.[And])
						End If
						If (Not primitiveTypeCode.IsUnsigned()) Then
							Me._builder.EmitOpCode(ILOpCode.Shr)
							Exit Select
						Else
							Me._builder.EmitOpCode(ILOpCode.Shr_un)
							Exit Select
						End If
					Case Else
						Throw ExceptionUtilities.UnexpectedValue(operatorKind)
				End Select
			ElseIf (expression.Checked AndAlso (primitiveTypeCode = Microsoft.Cci.PrimitiveTypeCode.Int32 OrElse primitiveTypeCode = Microsoft.Cci.PrimitiveTypeCode.Int64)) Then
				Me._builder.EmitOpCode(ILOpCode.Add_ovf)
			ElseIf (Not expression.Checked OrElse primitiveTypeCode <> Microsoft.Cci.PrimitiveTypeCode.UInt32 AndAlso primitiveTypeCode <> Microsoft.Cci.PrimitiveTypeCode.UInt64) Then
				Me._builder.EmitOpCode(ILOpCode.Add)
			Else
				Me._builder.EmitOpCode(ILOpCode.Add_ovf_un)
			End If
			Me.DowncastResultOfArithmeticOperation(primitiveTypeCode, If(Not expression.Checked OrElse operatorKind = BinaryOperatorKind.LeftShift, False, operatorKind <> BinaryOperatorKind.RightShift))
		End Sub

		Private Sub EmitBinaryConditionalExpression(ByVal expr As BoundBinaryConditionalExpression, ByVal used As Boolean)
			Me.EmitExpression(expr.TestExpression, True)
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.StackMergeType(expr.TestExpression)
			If (used) Then
				If (CodeGenerator.IsVarianceCast(expr.Type, type)) Then
					Me.EmitStaticCast(expr.Type, expr.Syntax)
					type = expr.Type
				End If
				Me._builder.EmitOpCode(ILOpCode.Dup)
			End If
			If (expr.Type.IsTypeParameter()) Then
				Me.EmitBox(expr.Type, expr.TestExpression.Syntax)
			End If
			Dim objectValue As Object = RuntimeHelpers.GetObjectValue(New [Object]())
			Me._builder.EmitBranch(ILOpCode.Brtrue, RuntimeHelpers.GetObjectValue(objectValue), ILOpCode.Nop)
			If (used) Then
				Me._builder.EmitOpCode(ILOpCode.Pop)
			End If
			Me.EmitExpression(expr.ElseExpression, used)
			If (used) Then
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.StackMergeType(expr.ElseExpression)
				If (CodeGenerator.IsVarianceCast(expr.Type, typeSymbol)) Then
					Me.EmitStaticCast(expr.Type, expr.Syntax)
					typeSymbol = expr.Type
				ElseIf (expr.Type.IsInterfaceType() AndAlso Not Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(expr.Type, type, TypeCompareKind.ConsiderEverything) AndAlso Not Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(expr.Type, typeSymbol, TypeCompareKind.ConsiderEverything)) Then
					Me.EmitStaticCast(expr.Type, expr.Syntax)
				End If
			End If
			Me._builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue))
		End Sub

		Private Sub EmitBinaryCondOperator(ByVal binOp As BoundBinaryOperator, ByVal sense As Boolean)
			Dim num As Integer
			Dim flag As Boolean = sense
			Dim operatorKind As BinaryOperatorKind = binOp.OperatorKind And BinaryOperatorKind.OpMask
			Dim type As TypeSymbol = binOp.Left.Type
			If (type IsNot Nothing AndAlso type.IsBooleanType()) Then
				Select Case operatorKind
					Case BinaryOperatorKind.LessThanOrEqual
						operatorKind = BinaryOperatorKind.GreaterThanOrEqual
						Exit Select
					Case BinaryOperatorKind.GreaterThanOrEqual
						operatorKind = BinaryOperatorKind.LessThanOrEqual
						Exit Select
					Case BinaryOperatorKind.LessThan
						operatorKind = BinaryOperatorKind.GreaterThan
						Exit Select
					Case BinaryOperatorKind.GreaterThan
						operatorKind = BinaryOperatorKind.LessThan
						Exit Select
				End Select
			End If
			Select Case operatorKind
				Case BinaryOperatorKind.Equals
				Case BinaryOperatorKind.[Is]
				Label0:
					Dim constantValueOpt As ConstantValue = binOp.Left.ConstantValueOpt
					Dim right As BoundExpression = binOp.Right
					If (constantValueOpt Is Nothing) Then
						constantValueOpt = right.ConstantValueOpt
						right = binOp.Left
					End If
					If (constantValueOpt IsNot Nothing) Then
						If (constantValueOpt.IsDefaultValue) Then
							If (Not constantValueOpt.IsFloating) Then
								If (sense) Then
									Me.EmitIsNullOrZero(right, constantValueOpt)
									Return
								End If
								Me.EmitIsNotNullOrZero(right, constantValueOpt)
								Return
							End If
						ElseIf (constantValueOpt.IsBoolean) Then
							Me.EmitExpression(right, True)
							Me.EmitIsSense(sense)
							Return
						End If
					End If
					Me.EmitBinaryCondOperatorHelper(ILOpCode.Ceq, binOp.Left, binOp.Right, sense)
					Return
				Case BinaryOperatorKind.NotEquals
				Case BinaryOperatorKind.[IsNot]
					sense = Not sense
					GoTo Label0
				Case BinaryOperatorKind.LessThanOrEqual
					num = 1
					sense = Not sense
					Exit Select
				Case BinaryOperatorKind.GreaterThanOrEqual
					num = 3
					sense = Not sense
					Exit Select
				Case BinaryOperatorKind.LessThan
					num = 0
					Exit Select
				Case BinaryOperatorKind.GreaterThan
					num = 2
					Exit Select
				Case BinaryOperatorKind.Subtract
				Case BinaryOperatorKind.Multiply
				Case BinaryOperatorKind.Power
				Case BinaryOperatorKind.Divide
				Case BinaryOperatorKind.Modulo
				Case BinaryOperatorKind.IntegerDivide
				Case BinaryOperatorKind.LeftShift
				Case BinaryOperatorKind.RightShift
					Throw ExceptionUtilities.UnexpectedValue(operatorKind)
				Case BinaryOperatorKind.[Xor]
					If (sense) Then
						Me.EmitBinaryCondOperatorHelper(ILOpCode.[Xor], binOp.Left, binOp.Right, True)
						Return
					End If
					Me.EmitBinaryCondOperatorHelper(ILOpCode.Ceq, binOp.Left, binOp.Right, True)
					Return
				Case BinaryOperatorKind.[Or]
					Me.EmitBinaryCondOperatorHelper(ILOpCode.[Or], binOp.Left, binOp.Right, sense)
					Return
				Case BinaryOperatorKind.[OrElse]
					flag = Not flag
					If (Not flag) Then
						Me.EmitShortCircuitingOperator(binOp, sense, sense, True)
						Return
					End If
					Me.EmitShortCircuitingOperator(binOp, sense, Not sense, False)
					Return
				Case BinaryOperatorKind.[And]
					Me.EmitBinaryCondOperatorHelper(ILOpCode.[And], binOp.Left, binOp.Right, sense)
					Return
				Case BinaryOperatorKind.[AndAlso]
					If (Not flag) Then
						Me.EmitShortCircuitingOperator(binOp, sense, sense, True)
						Return
					End If
					Me.EmitShortCircuitingOperator(binOp, sense, Not sense, False)
					Return
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(operatorKind)
			End Select
			If (type IsNot Nothing) Then
				If (type.IsUnsignedIntegralType()) Then
					num += 4
				ElseIf (type.IsFloatingType()) Then
					num += 8
				End If
			End If
			Me.EmitBinaryCondOperatorHelper(CodeGenerator.s_compOpCodes(num), binOp.Left, binOp.Right, sense)
		End Sub

		Private Sub EmitBinaryCondOperatorHelper(ByVal opCode As ILOpCode, ByVal left As BoundExpression, ByVal right As BoundExpression, ByVal sense As Boolean)
			Me.EmitExpression(left, True)
			Me.EmitExpression(right, True)
			Me._builder.EmitOpCode(opCode)
			Me.EmitIsSense(sense)
		End Sub

		Private Sub EmitBinaryOperator(ByVal expression As BoundBinaryOperator)
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.CodeGen.CodeGenerator::EmitBinaryOperator(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void EmitBinaryOperator(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
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
			'    à ..(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 49
			'    à ..Visit(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 276
			'    à ..( ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 416
			'    à ..(ICodeNode ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:ligne 65
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

		End Sub

		Private Sub EmitBinaryOperatorExpression(ByVal expression As BoundBinaryOperator, ByVal used As Boolean)
			Dim operatorKind As BinaryOperatorKind = expression.OperatorKind And BinaryOperatorKind.OpMask
			Dim flag As Boolean = If(operatorKind = BinaryOperatorKind.[AndAlso], True, operatorKind = BinaryOperatorKind.[OrElse])
			If (Not used AndAlso Not flag AndAlso Not Me.OperatorHasSideEffects(expression)) Then
				Me.EmitExpression(expression.Left, False)
				Me.EmitExpression(expression.Right, False)
				Return
			End If
			If (Not Me.IsCondOperator(operatorKind)) Then
				Me.EmitBinaryOperator(expression)
			Else
				Me.EmitBinaryCondOperator(expression, True)
			End If
			Me.EmitPopIfUnused(used)
		End Sub

		Private Sub EmitBinaryOperatorSimple(ByVal expression As BoundBinaryOperator)
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.CodeGen.CodeGenerator::EmitBinaryOperatorSimple(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void EmitBinaryOperatorSimple(Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
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

		End Sub

		Private Sub EmitBlock(ByVal scope As BoundBlock)
			Dim syntax As SyntaxNode
			Dim isEmpty As Boolean = Not scope.Locals.IsEmpty
			If (isEmpty) Then
				Me._builder.OpenLocalScope(ScopeType.Variable, Nothing)
				Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol).Enumerator = scope.Locals.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = enumerator.Current
					Dim declaringSyntaxReferences As ImmutableArray(Of SyntaxReference) = current.DeclaringSyntaxReferences
					Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = current
					If (declaringSyntaxReferences.IsEmpty) Then
						syntax = scope.Syntax
					Else
						syntax = declaringSyntaxReferences(0).GetVisualBasicSyntax(New CancellationToken())
					End If
					Me.DefineLocal(localSymbol, syntax)
				End While
			End If
			Dim enumerator1 As ImmutableArray(Of BoundStatement).Enumerator = scope.Statements.GetEnumerator()
			While enumerator1.MoveNext()
				Me.EmitStatement(enumerator1.Current)
			End While
			If (isEmpty) Then
				Me._builder.CloseLocalScope()
			End If
		End Sub

		Private Sub EmitBox(ByVal type As TypeSymbol, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode)
			Me._builder.EmitOpCode(ILOpCode.Box)
			Me.EmitSymbolToken(type, syntaxNode)
		End Sub

		Private Sub EmitCallExpression(ByVal [call] As BoundCall, ByVal useKind As CodeGenerator.UseKind)
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.CodeGen.CodeGenerator::EmitCallExpression(Microsoft.CodeAnalysis.VisualBasic.BoundCall,Microsoft.CodeAnalysis.VisualBasic.CodeGen.CodeGenerator/UseKind)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void EmitCallExpression(Microsoft.CodeAnalysis.VisualBasic.BoundCall,Microsoft.CodeAnalysis.VisualBasic.CodeGen.CodeGenerator/UseKind)
			' 
			' L'index était hors limites. Il ne doit pas être négatif et doit être inférieur à la taille de la collection.
			' Nom du paramètre : index
			'    à System.ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
			'    à ..(Int32 ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:ligne 364
			'    à ..() dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:ligne 74
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RemoveCompilerOptimizationsStep.cs:ligne 55
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Sub

		Private Sub EmitCaseBlocks(ByVal caseBlocks As ImmutableArray(Of BoundCaseBlock), ByVal caseBlockLabels As ImmutableArray(Of GeneratedLabelSymbol), ByVal exitLabel As LabelSymbol)
			Dim num As Integer = 0
			Dim enumerator As ImmutableArray(Of BoundCaseBlock).Enumerator = caseBlocks.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As BoundCaseBlock = enumerator.Current
				Me._builder.MarkLabel(caseBlockLabels(num))
				num = num + 1
				Dim caseStatement As BoundCaseStatement = current.CaseStatement
				If (Not caseStatement.WasCompilerGenerated) Then
					If (Me._emitPdbSequencePoints) Then
						Me.EmitSequencePoint(caseStatement.Syntax)
					End If
					If (Me._ilEmitStyle = ILEmitStyle.Debug) Then
						Me._builder.EmitOpCode(ILOpCode.Nop)
					End If
				End If
				Me.EmitBlock(current.Body)
				Me._builder.EmitBranch(ILOpCode.Br, exitLabel, ILOpCode.Nop)
			End While
		End Sub

		Private Sub EmitCatchBlock(ByVal catchBlock As Microsoft.CodeAnalysis.VisualBasic.BoundCatchBlock)
			Dim typeReference As ITypeReference
			Dim syntax As SyntaxNode
			Dim boundCatchBlock As Microsoft.CodeAnalysis.VisualBasic.BoundCatchBlock = Me._currentCatchBlock
			Me._currentCatchBlock = catchBlock
			Dim objectValue As Object = Nothing
			Dim exceptionSourceOpt As BoundExpression = catchBlock.ExceptionSourceOpt
			If (exceptionSourceOpt Is Nothing) Then
				typeReference = Me._module.Translate(Me._module.Compilation.GetWellKnownType(WellKnownType.System_Exception), catchBlock.Syntax, Me._diagnostics, False, False)
			Else
				typeReference = Me._module.Translate(exceptionSourceOpt.Type, exceptionSourceOpt.Syntax, Me._diagnostics)
			End If
			Me._builder.AdjustStack(1)
			If (catchBlock.ExceptionFilterOpt Is Nothing OrElse catchBlock.ExceptionFilterOpt.Kind <> BoundKind.UnstructuredExceptionHandlingCatchFilter) Then
				If (catchBlock.ExceptionFilterOpt IsNot Nothing) Then
					Me._builder.OpenLocalScope(ScopeType.Filter, Nothing)
					Dim obj As Object = RuntimeHelpers.GetObjectValue(New [Object]())
					objectValue = RuntimeHelpers.GetObjectValue(New [Object]())
					Me._builder.EmitOpCode(ILOpCode.Isinst)
					Me._builder.EmitToken(typeReference, catchBlock.Syntax, Me._diagnostics, False)
					Me._builder.EmitOpCode(ILOpCode.Dup)
					Me._builder.EmitBranch(ILOpCode.Brtrue, RuntimeHelpers.GetObjectValue(obj), ILOpCode.Nop)
					Me._builder.EmitOpCode(ILOpCode.Pop)
					Me._builder.EmitIntConstant(0)
					Me._builder.EmitBranch(ILOpCode.Br, RuntimeHelpers.GetObjectValue(objectValue), ILOpCode.Nop)
					Me._builder.MarkLabel(RuntimeHelpers.GetObjectValue(obj))
				Else
					Me._builder.OpenLocalScope(ScopeType.[Catch], typeReference)
					If (catchBlock.IsSynthesizedAsyncCatchAll) Then
						Me._asyncCatchHandlerOffset = Me._builder.AllocateILMarker()
					End If
				End If
				Dim localOpt As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = catchBlock.LocalOpt
				If (localOpt IsNot Nothing) Then
					Dim declaringSyntaxReferences As ImmutableArray(Of SyntaxReference) = localOpt.DeclaringSyntaxReferences
					Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = localOpt
					If (Not declaringSyntaxReferences.IsEmpty) Then
						syntax = DirectCast(declaringSyntaxReferences(0).GetSyntax(New CancellationToken()), VisualBasicSyntaxNode)
					Else
						syntax = catchBlock.Syntax
					End If
					Me.DefineLocal(localSymbol, syntax)
				End If
				If (exceptionSourceOpt IsNot Nothing) Then
					If (Me.ShouldNoteProjectErrors()) Then
						Me._builder.EmitOpCode(ILOpCode.Dup)
						Me.EmitSetProjectError(catchBlock.Syntax, catchBlock.ErrorLineNumberOpt)
					End If
					If (exceptionSourceOpt.Type.IsTypeParameter()) Then
						Me._builder.EmitOpCode(ILOpCode.Unbox_any)
						Me.EmitSymbolToken(exceptionSourceOpt.Type, exceptionSourceOpt.Syntax)
					End If
					While exceptionSourceOpt.Kind = BoundKind.Sequence
						Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSequence = DirectCast(exceptionSourceOpt, Microsoft.CodeAnalysis.VisualBasic.BoundSequence)
						Me.EmitSideEffects(boundSequence.SideEffects)
						If (boundSequence.ValueOpt Is Nothing) Then
							Exit While
						End If
						exceptionSourceOpt = boundSequence.ValueOpt
					End While
					Dim kind As BoundKind = exceptionSourceOpt.Kind
					If (kind = BoundKind.FieldAccess) Then
						Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = DirectCast(exceptionSourceOpt, Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess)
						If (Not boundFieldAccess.FieldSymbol.IsShared) Then
							Dim fieldSymbol As StateMachineFieldSymbol = TryCast(boundFieldAccess.FieldSymbol, StateMachineFieldSymbol)
							If (fieldSymbol IsNot Nothing AndAlso fieldSymbol.SlotIndex >= 0) Then
								Me.DefineUserDefinedStateMachineHoistedLocal(fieldSymbol)
							End If
							Dim localDefinition As Microsoft.CodeAnalysis.CodeGen.LocalDefinition = Me.AllocateTemp(exceptionSourceOpt.Type, exceptionSourceOpt.Syntax)
							Me._builder.EmitLocalStore(localDefinition)
							Me.EmitReceiverRef(boundFieldAccess.ReceiverOpt, False, CodeGenerator.AddressKind.[ReadOnly])
							Me._builder.EmitLocalLoad(localDefinition)
						End If
						Me.EmitFieldStore(boundFieldAccess)
					ElseIf (kind = BoundKind.Local) Then
						Me._builder.EmitLocalStore(Me.GetLocal(DirectCast(exceptionSourceOpt, BoundLocal)))
					Else
						If (kind <> BoundKind.Parameter) Then
							Throw ExceptionUtilities.UnexpectedValue(exceptionSourceOpt.Kind)
						End If
						Dim boundParameter As Microsoft.CodeAnalysis.VisualBasic.BoundParameter = DirectCast(exceptionSourceOpt, Microsoft.CodeAnalysis.VisualBasic.BoundParameter)
						If (boundParameter.ParameterSymbol.IsByRef) Then
							Dim localDefinition1 As Microsoft.CodeAnalysis.CodeGen.LocalDefinition = Me.AllocateTemp(exceptionSourceOpt.Type, exceptionSourceOpt.Syntax)
							Me._builder.EmitLocalStore(localDefinition1)
							Me._builder.EmitLoadArgumentOpcode(Me.ParameterSlot(boundParameter))
							Me._builder.EmitLocalLoad(localDefinition1)
							Me.FreeTemp(localDefinition1)
						End If
						Me.EmitParameterStore(boundParameter)
					End If
				ElseIf (Not Me.ShouldNoteProjectErrors()) Then
					Me._builder.EmitOpCode(ILOpCode.Pop)
				Else
					Me.EmitSetProjectError(catchBlock.Syntax, catchBlock.ErrorLineNumberOpt)
				End If
				If (catchBlock.ExceptionFilterOpt IsNot Nothing) Then
					Me.EmitCondExpr(catchBlock.ExceptionFilterOpt, True)
					Me._builder.EmitIntConstant(0)
					Me._builder.EmitOpCode(ILOpCode.Cgt_un)
					Me._builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue))
					Me._builder.MarkFilterConditionEnd()
					Me._builder.EmitOpCode(ILOpCode.Pop)
				End If
			Else
				Dim exceptionFilterOpt As BoundUnstructuredExceptionHandlingCatchFilter = DirectCast(catchBlock.ExceptionFilterOpt, BoundUnstructuredExceptionHandlingCatchFilter)
				Me._builder.OpenLocalScope(ScopeType.Filter, Nothing)
				Me._builder.EmitOpCode(ILOpCode.Isinst)
				Me._builder.EmitToken(typeReference, catchBlock.Syntax, Me._diagnostics, False)
				Me._builder.EmitOpCode(ILOpCode.Ldnull)
				Me._builder.EmitOpCode(ILOpCode.Cgt_un)
				Me.EmitLocalLoad(exceptionFilterOpt.ActiveHandlerLocal, True)
				Me._builder.EmitIntConstant(0)
				Me._builder.EmitOpCode(ILOpCode.Cgt_un)
				Me._builder.EmitOpCode(ILOpCode.[And])
				Me.EmitLocalLoad(exceptionFilterOpt.ResumeTargetLocal, True)
				Me._builder.EmitIntConstant(0)
				Me._builder.EmitOpCode(ILOpCode.Ceq)
				Me._builder.EmitOpCode(ILOpCode.[And])
				Me._builder.MarkFilterConditionEnd()
				Me._builder.EmitOpCode(ILOpCode.Castclass)
				Me._builder.EmitToken(typeReference, catchBlock.Syntax, Me._diagnostics, False)
				If (Not Me.ShouldNoteProjectErrors()) Then
					Me._builder.EmitOpCode(ILOpCode.Pop)
				Else
					Me.EmitSetProjectError(catchBlock.Syntax, catchBlock.ErrorLineNumberOpt)
				End If
			End If
			Me.EmitBlock(catchBlock.Body)
			If (Me.ShouldNoteProjectErrors() AndAlso (catchBlock.ExceptionFilterOpt Is Nothing OrElse catchBlock.ExceptionFilterOpt.Kind <> BoundKind.UnstructuredExceptionHandlingCatchFilter)) Then
				Me.EmitClearProjectError(catchBlock.Syntax)
			End If
			Me._builder.CloseLocalScope()
			Me._currentCatchBlock = boundCatchBlock
		End Sub

		Private Sub EmitClearProjectError(ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode)
			Dim wellKnownTypeMember As MethodSymbol = DirectCast(Me._module.Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__ClearProjectError), MethodSymbol)
			Me._builder.EmitOpCode(ILOpCode.[Call], 0)
			Me.EmitSymbolToken(wellKnownTypeMember, syntaxNode, False)
		End Sub

		Private Sub EmitComplexConditionalAccessReceiver(ByVal expression As BoundComplexConditionalAccessReceiver, ByVal used As Boolean)
			Dim type As TypeSymbol = expression.Type
			Dim objectValue As Object = RuntimeHelpers.GetObjectValue(New [Object]())
			Dim obj As Object = RuntimeHelpers.GetObjectValue(New [Object]())
			Me.EmitInitObj(type, True, expression.Syntax)
			Me.EmitBox(type, expression.Syntax)
			Me._builder.EmitBranch(ILOpCode.Brtrue, RuntimeHelpers.GetObjectValue(objectValue), ILOpCode.Nop)
			Me.EmitExpression(expression.ReferenceTypeReceiver, used)
			Me._builder.EmitBranch(ILOpCode.Br, RuntimeHelpers.GetObjectValue(obj), ILOpCode.Nop)
			Me._builder.AdjustStack(-1)
			Me._builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue))
			Me.EmitExpression(expression.ValueTypeReceiver, used)
			Me._builder.MarkLabel(RuntimeHelpers.GetObjectValue(obj))
		End Sub

		Private Sub EmitComplexConditionalAccessReceiverAddress(ByVal expression As BoundComplexConditionalAccessReceiver)
			Dim type As TypeSymbol = expression.Type
			Dim objectValue As Object = RuntimeHelpers.GetObjectValue(New [Object]())
			Dim obj As Object = RuntimeHelpers.GetObjectValue(New [Object]())
			Me.EmitInitObj(type, True, expression.Syntax)
			Me.EmitBox(type, expression.Syntax)
			Me._builder.EmitBranch(ILOpCode.Brtrue, RuntimeHelpers.GetObjectValue(objectValue), ILOpCode.Nop)
			Me.EmitAddress(expression.ReferenceTypeReceiver, CodeGenerator.AddressKind.[ReadOnly])
			Me._builder.EmitBranch(ILOpCode.Br, RuntimeHelpers.GetObjectValue(obj), ILOpCode.Nop)
			Me._builder.AdjustStack(-1)
			Me._builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue))
			Me.EmitReceiverRef(expression.ValueTypeReceiver, True, CodeGenerator.AddressKind.[ReadOnly])
			Me._builder.MarkLabel(RuntimeHelpers.GetObjectValue(obj))
		End Sub

		Private Sub EmitCondBranch(ByVal condition As BoundExpression, ByRef lazyDest As Object, ByVal sense As Boolean)
			Me._recursionDepth = Me._recursionDepth + 1
			If (Me._recursionDepth <= 1) Then
				Me.EmitCondBranchCoreWithStackGuard(condition, lazyDest, sense)
			Else
				StackGuard.EnsureSufficientExecutionStack(Me._recursionDepth)
				Me.EmitCondBranchCore(condition, lazyDest, sense)
			End If
			Me._recursionDepth = Me._recursionDepth - 1
		End Sub

		Private Sub EmitCondBranchCore(ByVal condition As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByRef lazyDest As Object, ByVal sense As Boolean)
			Dim lOpCode As ILOpCode
			Dim boundBinaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator
			Dim lOpCode1 As ILOpCode
			While True
				Dim constantValueOpt As ConstantValue = condition.ConstantValueOpt
				If (constantValueOpt Is Nothing) Then
					Dim kind As BoundKind = condition.Kind
					If (kind > BoundKind.BinaryOperator) Then
						If (kind = BoundKind.[TypeOf]) Then
							Dim boundTypeOf As Microsoft.CodeAnalysis.VisualBasic.BoundTypeOf = DirectCast(condition, Microsoft.CodeAnalysis.VisualBasic.BoundTypeOf)
							Me.EmitTypeOfExpression(boundTypeOf, True, True)
							If (boundTypeOf.IsTypeOfIsNotExpression) Then
								sense = Not sense
							End If
							lOpCode = If(sense, ILOpCode.Brtrue, ILOpCode.Brfalse)
							lazyDest = RuntimeHelpers.GetObjectValue(If(lazyDest, New [Object]()))
							Me._builder.EmitBranch(lOpCode, RuntimeHelpers.GetObjectValue(lazyDest), ILOpCode.Nop)
							Return
						End If
						If (kind = BoundKind.Sequence) Then
							Me.EmitSequenceCondBranch(DirectCast(condition, BoundSequence), lazyDest, sense)
							Return
						End If
					ElseIf (kind = BoundKind.UnaryOperator) Then
						Dim boundUnaryOperator As Microsoft.CodeAnalysis.VisualBasic.BoundUnaryOperator = DirectCast(condition, Microsoft.CodeAnalysis.VisualBasic.BoundUnaryOperator)
						If (boundUnaryOperator.OperatorKind <> UnaryOperatorKind.[Not]) Then
							GoTo Label1
						End If
						sense = Not sense
						condition = boundUnaryOperator.Operand
						Continue While
					ElseIf (kind = BoundKind.BinaryOperator) Then
						boundBinaryOperator = DirectCast(condition, Microsoft.CodeAnalysis.VisualBasic.BoundBinaryOperator)
						Dim flag As Boolean = sense
						Dim operatorKind As BinaryOperatorKind = boundBinaryOperator.OperatorKind And BinaryOperatorKind.OpMask
						If (CInt(operatorKind) - CInt(BinaryOperatorKind.Equals) > CInt(BinaryOperatorKind.Add)) Then
							If (CInt(operatorKind) - CInt(BinaryOperatorKind.LessThanOrEqual) <= CInt(BinaryOperatorKind.[Like])) Then
								Exit While
							End If
							Select Case operatorKind
								Case BinaryOperatorKind.[OrElse]
									flag = Not flag
									GoTo Label2
								Case BinaryOperatorKind.[AndAlso]
								Label2:
									If (Not flag) Then
										Me.EmitCondBranch(boundBinaryOperator.Left, lazyDest, sense)
										condition = boundBinaryOperator.Right
										Continue While
									Else
										Dim objectValue As Object = RuntimeHelpers.GetObjectValue(New [Object]())
										Me.EmitCondBranch(boundBinaryOperator.Left, objectValue, Not sense)
										Me.EmitCondBranch(boundBinaryOperator.Right, lazyDest, sense)
										If (objectValue Is Nothing) Then
											Return
										End If
										Me._builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue))
										Return
									End If
								Case BinaryOperatorKind.[Is]
								Case BinaryOperatorKind.[IsNot]
									Exit Select
								Case Else
									GoTo Label1
							End Select
						End If
						Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.TryReduce(boundBinaryOperator, sense)
						If (boundExpression Is Nothing) Then
							Exit While
						End If
						condition = boundExpression
						Continue While
					End If
				Label1:
					Me.EmitExpression(condition, True)
					Dim type As TypeSymbol = condition.Type
					If (Not type.IsValueType AndAlso Not type.IsVerifierReference()) Then
						Me.EmitBox(type, condition.Syntax)
					End If
					lOpCode = If(sense, ILOpCode.Brtrue, ILOpCode.Brfalse)
					lazyDest = RuntimeHelpers.GetObjectValue(If(lazyDest, New [Object]()))
					Me._builder.EmitBranch(lOpCode, RuntimeHelpers.GetObjectValue(lazyDest), ILOpCode.Nop)
				ElseIf (constantValueOpt.IsDefaultValue <> sense) Then
					lazyDest = RuntimeHelpers.GetObjectValue(If(lazyDest, New [Object]()))
					Me._builder.EmitBranch(ILOpCode.Br, RuntimeHelpers.GetObjectValue(lazyDest), ILOpCode.Nop)
					Return
				End If
				Return
			End While
			Me.EmitExpression(boundBinaryOperator.Left, True)
			Me.EmitExpression(boundBinaryOperator.Right, True)
			lOpCode = Me.CodeForJump(boundBinaryOperator, sense, lOpCode1)
			lazyDest = RuntimeHelpers.GetObjectValue(If(lazyDest, New [Object]()))
			Me._builder.EmitBranch(lOpCode, RuntimeHelpers.GetObjectValue(lazyDest), lOpCode1)
		End Sub

		Private Sub EmitCondBranchCoreWithStackGuard(ByVal condition As BoundExpression, ByRef lazyDest As Object, ByVal sense As Boolean)
			Try
				Me.EmitCondBranchCore(condition, lazyDest, sense)
			Catch insufficientExecutionStackException As System.InsufficientExecutionStackException
				ProjectData.SetProjectError(insufficientExecutionStackException)
				Me._diagnostics.Add(ERRID.ERR_TooLongOrComplexExpression, BoundTreeVisitor.CancelledByStackGuardException.GetTooLongOrComplexExpressionErrorLocation(condition))
				Throw New CodeGenerator.EmitCancelledException()
			End Try
		End Sub

		Private Function EmitCondExpr(ByVal condition As BoundExpression, ByVal sense As Boolean) As CodeGenerator.ConstResKind
			Dim constResKind As CodeGenerator.ConstResKind
			While condition.Kind = BoundKind.UnaryOperator
				condition = DirectCast(condition, BoundUnaryOperator).Operand
				sense = Not sense
			End While
			If (Me._ilEmitStyle = ILEmitStyle.Release AndAlso condition.IsConstant) Then
				Dim booleanValue As Boolean = condition.ConstantValueOpt.BooleanValue
				Me._builder.EmitBoolConstant(booleanValue = sense)
				constResKind = If(booleanValue = sense, CodeGenerator.ConstResKind.ConstTrue, CodeGenerator.ConstResKind.ConstFalse)
			ElseIf (condition.Kind <> BoundKind.BinaryOperator) Then
				Me.EmitExpression(condition, True)
				Me.EmitIsSense(sense)
				constResKind = CodeGenerator.ConstResKind.NotAConst
			Else
				Me.EmitBinaryCondOperator(DirectCast(condition, BoundBinaryOperator), sense)
				constResKind = CodeGenerator.ConstResKind.NotAConst
			End If
			Return constResKind
		End Function

		Private Sub EmitConditionalAccess(ByVal conditional As BoundLoweredConditionalAccess, ByVal used As Boolean)
			Dim flag As Boolean
			If (conditional.ReceiverOrCondition.Type.IsBooleanType()) Then
				Dim objectValue As Object = RuntimeHelpers.GetObjectValue(New [Object]())
				Dim obj As Object = RuntimeHelpers.GetObjectValue(New [Object]())
				Me.EmitCondBranch(conditional.ReceiverOrCondition, obj, True)
				If (conditional.WhenNullOpt IsNot Nothing) Then
					Me.EmitExpression(conditional.WhenNullOpt, used)
				End If
				Me._builder.EmitBranch(ILOpCode.Br, RuntimeHelpers.GetObjectValue(objectValue), ILOpCode.Nop)
				If (used) Then
					Me._builder.AdjustStack(-1)
				End If
				Me._builder.MarkLabel(RuntimeHelpers.GetObjectValue(obj))
				Me.EmitExpression(conditional.WhenNotNull, used)
				Me._builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue))
				Return
			End If
			Dim localDefinition As Microsoft.CodeAnalysis.CodeGen.LocalDefinition = Nothing
			Dim localDefinition1 As Microsoft.CodeAnalysis.CodeGen.LocalDefinition = Nothing
			Dim objectValue1 As Object = RuntimeHelpers.GetObjectValue(New [Object]())
			Dim obj1 As Object = RuntimeHelpers.GetObjectValue(New [Object]())
			Dim receiverOrCondition As BoundExpression = conditional.ReceiverOrCondition
			Dim type As TypeSymbol = receiverOrCondition.Type
			If (conditional.CaptureReceiver) Then
				flag = True
			Else
				flag = If(Not type.IsReferenceType, False, type.TypeKind = Microsoft.CodeAnalysis.TypeKind.TypeParameter)
			End If
			If (Not flag) Then
				Me.EmitExpression(receiverOrCondition, True)
				If (Not type.IsReferenceType) Then
					Me.EmitBox(type, receiverOrCondition.Syntax)
				End If
			Else
				localDefinition = Me.EmitReceiverRef(receiverOrCondition, Not type.IsReferenceType, CodeGenerator.AddressKind.[ReadOnly])
				If (type.IsReferenceType) Then
					Me._builder.EmitOpCode(ILOpCode.Dup)
				ElseIf (localDefinition IsNot Nothing) Then
					Me._builder.EmitOpCode(ILOpCode.Dup)
					Me.EmitLoadIndirect(type, receiverOrCondition.Syntax)
					Me.EmitBox(type, receiverOrCondition.Syntax)
				Else
					Me.EmitInitObj(type, True, receiverOrCondition.Syntax)
					Me.EmitBox(type, receiverOrCondition.Syntax)
					Me._builder.EmitBranch(ILOpCode.Brtrue, RuntimeHelpers.GetObjectValue(objectValue1), ILOpCode.Nop)
					Me.EmitLoadIndirect(type, receiverOrCondition.Syntax)
					localDefinition1 = Me.AllocateTemp(type, receiverOrCondition.Syntax)
					Me._builder.EmitLocalStore(localDefinition1)
					Me._builder.EmitLocalAddress(localDefinition1)
					Me._builder.EmitLocalLoad(localDefinition1)
					Me.EmitBox(type, receiverOrCondition.Syntax)
				End If
			End If
			Me._builder.EmitBranch(ILOpCode.Brtrue, RuntimeHelpers.GetObjectValue(objectValue1), ILOpCode.Nop)
			If (flag) Then
				Me._builder.EmitOpCode(ILOpCode.Pop)
			End If
			If (conditional.WhenNullOpt IsNot Nothing) Then
				Me.EmitExpression(conditional.WhenNullOpt, used)
			End If
			Me._builder.EmitBranch(ILOpCode.Br, RuntimeHelpers.GetObjectValue(obj1), ILOpCode.Nop)
			If (used) Then
				Me._builder.AdjustStack(-1)
			End If
			If (flag) Then
				Me._builder.AdjustStack(1)
			End If
			Me._builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue1))
			If (Not flag) Then
				localDefinition = Me.EmitReceiverRef(receiverOrCondition, Not type.IsReferenceType, CodeGenerator.AddressKind.[ReadOnly])
			End If
			Me.EmitExpression(conditional.WhenNotNull, used)
			Me._builder.MarkLabel(RuntimeHelpers.GetObjectValue(obj1))
			If (localDefinition1 IsNot Nothing) Then
				Me.FreeTemp(localDefinition1)
			End If
			If (localDefinition IsNot Nothing) Then
				Me.FreeTemp(localDefinition)
			End If
		End Sub

		Private Sub EmitConditionalAccessReceiverPlaceholder(ByVal expression As BoundConditionalAccessReceiverPlaceholder, ByVal used As Boolean)
			If (used AndAlso Not expression.Type.IsReferenceType) Then
				Me.EmitLoadIndirect(expression.Type, expression.Syntax)
			End If
			Me.EmitPopIfUnused(used)
		End Sub

		Private Sub EmitConditionalGoto(ByVal boundConditionalGoto As Microsoft.CodeAnalysis.VisualBasic.BoundConditionalGoto)
			Dim label As Object = boundConditionalGoto.Label
			Me.EmitCondBranch(boundConditionalGoto.Condition, label, boundConditionalGoto.JumpIfTrue)
		End Sub

		Private Sub EmitConstantExpression(ByVal type As TypeSymbol, ByVal constantValue As Microsoft.CodeAnalysis.ConstantValue, ByVal used As Boolean, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode)
			If (used) Then
				If (type IsNot Nothing AndAlso type.TypeKind = Microsoft.CodeAnalysis.TypeKind.TypeParameter AndAlso constantValue.IsNull) Then
					Me.EmitInitObj(type, used, syntaxNode)
					Return
				End If
				Me._builder.EmitConstantValue(constantValue)
			End If
		End Sub

		Private Sub EmitConstantExpression(ByVal expression As BoundExpression)
			Me._builder.EmitConstantValue(expression.ConstantValueOpt)
		End Sub

		Private Sub EmitConversionExpression(ByVal conversion As BoundConversion, ByVal used As Boolean)
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.CodeGen.CodeGenerator::EmitConversionExpression(Microsoft.CodeAnalysis.VisualBasic.BoundConversion,System.Boolean)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void EmitConversionExpression(Microsoft.CodeAnalysis.VisualBasic.BoundConversion,System.Boolean)
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

		End Sub

		Private Sub EmitConvertIntrinsic(ByVal conversion As BoundConversion, ByVal underlyingFrom As Microsoft.Cci.PrimitiveTypeCode, ByVal underlyingTo As Microsoft.Cci.PrimitiveTypeCode)
			Me.EmitExpression(conversion.Operand, True)
			If (underlyingFrom <> underlyingTo OrElse conversion.ExplicitCastInCode OrElse underlyingFrom = Microsoft.Cci.PrimitiveTypeCode.Float32 OrElse underlyingFrom = Microsoft.Cci.PrimitiveTypeCode.Float64) Then
				If (underlyingTo = Microsoft.Cci.PrimitiveTypeCode.[Boolean]) Then
					Me._builder.EmitConstantValue(ConstantValue.[Default](underlyingFrom.GetConstantValueTypeDiscriminator()))
					If (Not underlyingFrom.IsFloatingPoint()) Then
						Me._builder.EmitOpCode(ILOpCode.Cgt_un)
						Return
					End If
					Me._builder.EmitOpCode(ILOpCode.Ceq)
					Me._builder.EmitOpCode(ILOpCode.Ldc_i4_0)
					Me._builder.EmitOpCode(ILOpCode.Ceq)
					Return
				End If
				If (underlyingFrom <> Microsoft.Cci.PrimitiveTypeCode.[Boolean]) Then
					If (underlyingFrom = Microsoft.Cci.PrimitiveTypeCode.Float32 AndAlso CodeGenerator.IsIntegral(underlyingTo)) Then
						Dim kind As BoundKind = conversion.Operand.Kind
						If (kind <> BoundKind.UnaryOperator) Then
							If (kind = BoundKind.BinaryOperator) Then
								Dim operatorKind As BinaryOperatorKind = DirectCast(conversion.Operand, BoundBinaryOperator).OperatorKind And BinaryOperatorKind.OpMask
								If (operatorKind = BinaryOperatorKind.Add OrElse CInt(operatorKind) - CInt(BinaryOperatorKind.Subtract) <= CInt(BinaryOperatorKind.Equals)) Then
									Me._builder.EmitOpCode(ILOpCode.Conv_r4)
								End If
							End If
						ElseIf (CInt((DirectCast(conversion.Operand, BoundUnaryOperator).OperatorKind And UnaryOperatorKind.[Not])) - CInt(UnaryOperatorKind.Plus) <= CInt(UnaryOperatorKind.Plus)) Then
							Me._builder.EmitOpCode(ILOpCode.Conv_r4)
						End If
					End If
					Me.EmitConvertSimpleNumeric(conversion, underlyingFrom, underlyingTo, conversion.Checked)
				Else
					Me._builder.EmitOpCode(ILOpCode.Ldc_i4_0)
					Me._builder.EmitOpCode(ILOpCode.Cgt_un)
					Me._builder.EmitOpCode(ILOpCode.Neg)
					If (underlyingTo <> Microsoft.Cci.PrimitiveTypeCode.Int32) Then
						Me._builder.EmitNumericConversion(Microsoft.Cci.PrimitiveTypeCode.Int32, underlyingTo, False)
						Return
					End If
				End If
			End If
		End Sub

		Private Sub EmitConvertSimpleNumeric(ByVal conversion As BoundConversion, ByVal typeFrom As Microsoft.Cci.PrimitiveTypeCode, ByVal typeTo As Microsoft.Cci.PrimitiveTypeCode, ByVal checked As Boolean)
			Me._builder.EmitNumericConversion(typeFrom, typeTo, checked)
		End Sub

		Private Sub EmitDelegateCreation(ByVal receiver As BoundExpression, ByVal method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal delegateType As TypeSymbol, ByVal used As Boolean, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode)
			Dim flag As Boolean = If(receiver Is Nothing, True, method.IsShared)
			If (used) Then
				If (Not flag) Then
					Me.EmitExpression(receiver, True)
					If (Not receiver.Type.IsVerifierReference()) Then
						Me.EmitBox(receiver.Type, receiver.Syntax)
					End If
				Else
					Me._builder.EmitNullConstant()
				End If
				If (Not method.IsMetadataVirtual() OrElse method.ContainingType.IsDelegateType() OrElse receiver.SuppressVirtualCalls) Then
					Me._builder.EmitOpCode(ILOpCode.Ldftn)
				Else
					Me._builder.EmitOpCode(ILOpCode.Dup)
					Me._builder.EmitOpCode(ILOpCode.Ldvirtftn)
				End If
				Dim callsiteReducedFromMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = If(method.CallsiteReducedFromMethod, method)
				If (Not flag AndAlso callsiteReducedFromMethod.ContainingType.IsNullableType()) Then
					callsiteReducedFromMethod = method.OverriddenMethod
				End If
				Me.EmitSymbolToken(callsiteReducedFromMethod, syntaxNode, False)
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(delegateType.GetMembers(".ctor").[Single](), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
				Me._builder.EmitOpCode(ILOpCode.Newobj, -1)
				Me.EmitSymbolToken(methodSymbol, syntaxNode, False)
			ElseIf (Not flag) Then
				Me.EmitExpression(receiver, False)
				Return
			End If
		End Sub

		Private Sub EmitDelegateCreationExpression(ByVal expression As BoundDelegateCreationExpression, ByVal used As Boolean)
			Dim method As MethodSymbol = expression.Method
			Me.EmitDelegateCreation(expression.ReceiverOpt, method, expression.Type, used, expression.Syntax)
		End Sub

		Private Sub EmitDirectCastExpression(ByVal conversion As BoundDirectCast, ByVal used As Boolean)
			If (Not used AndAlso Not Me.ConversionHasSideEffects(conversion)) Then
				Me.EmitExpression(conversion.Operand, False)
				Return
			End If
			If (Not conversion.Operand.IsNothingLiteral()) Then
				Me.EmitExpression(conversion.Operand, True)
				If (Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(conversion.ConversionKind)) Then
					Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = conversion.Type
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = conversion.Operand.Type
					If (Not typeSymbol.GetEnumUnderlyingTypeOrSelf().IsSameTypeIgnoringAll(type.GetEnumUnderlyingTypeOrSelf())) Then
						If (typeSymbol.IsTypeParameter()) Then
							Me.EmitBox(typeSymbol, conversion.Operand.Syntax)
							If (type.SpecialType <> SpecialType.System_Object) Then
								If (type.IsTypeParameter()) Then
									Me._builder.EmitOpCode(ILOpCode.Unbox_any)
									Me.EmitSymbolToken(type, conversion.Syntax)
								ElseIf (Not type.IsReferenceType) Then
									Me.EmitUnboxAny(type, conversion.Syntax)
								Else
									Me._builder.EmitOpCode(ILOpCode.Castclass)
									Me.EmitSymbolToken(type, conversion.Syntax)
								End If
							End If
						ElseIf (type.IsTypeParameter()) Then
							If (typeSymbol.IsValueType) Then
								Me.EmitBox(typeSymbol, conversion.Operand.Syntax)
							End If
							Me._builder.EmitOpCode(ILOpCode.Unbox_any)
							Me.EmitSymbolToken(type, conversion.Syntax)
						ElseIf (typeSymbol.IsValueType) Then
							Me.EmitBox(typeSymbol, conversion.Operand.Syntax)
							If (type.IsInterfaceType()) Then
								Me._builder.EmitOpCode(ILOpCode.Castclass)
								Me.EmitSymbolToken(type, conversion.Syntax)
							End If
						ElseIf (Not type.IsReferenceType) Then
							Me.EmitUnboxAny(type, conversion.Syntax)
						Else
							Dim flag As Boolean = True
							If (Microsoft.CodeAnalysis.VisualBasic.Conversions.IsWideningConversion(conversion.ConversionKind)) Then
								flag = False
								If (typeSymbol.IsArrayType()) Then
									Dim elementType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = DirectCast(typeSymbol, ArrayTypeSymbol).ElementType
									If (type.IsArrayType() AndAlso (elementType.IsTypeParameter() OrElse DirectCast(type, ArrayTypeSymbol).ElementType.IsTypeParameter())) Then
										flag = True
									ElseIf (elementType.IsTypeParameter() AndAlso type.IsInterfaceType()) Then
										Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = DirectCast(type, Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol)
										If (namedTypeSymbol.Arity = 1 AndAlso Not namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics(0).IsSameTypeIgnoringAll(elementType)) Then
											flag = True
										End If
									End If
								End If
							End If
							If (flag) Then
								Me._builder.EmitOpCode(ILOpCode.Castclass)
								Me.EmitSymbolToken(type, conversion.Syntax)
							End If
						End If
					End If
				End If
			Else
				If (conversion.Type.IsTypeParameter()) Then
					Me.EmitLoadDefaultValueOfTypeParameter(conversion.Type, used, conversion.Syntax)
					Return
				End If
				Me.EmitExpression(conversion.Operand, True)
				If (conversion.Type.IsValueType) Then
					Me.EmitUnboxAny(conversion.Type, conversion.Syntax)
				End If
			End If
			Me.EmitPopIfUnused(used)
		End Sub

		Private Sub EmitDupExpression(ByVal dupExpression As BoundDup, ByVal used As Boolean)
			If (dupExpression.IsReference) Then
				Me._builder.EmitOpCode(ILOpCode.Dup)
				Me.EmitLoadIndirect(dupExpression.Type, dupExpression.Syntax)
				Me.EmitPopIfUnused(used)
			ElseIf (used) Then
				Me._builder.EmitOpCode(ILOpCode.Dup)
				Return
			End If
		End Sub

		Private Sub EmitElementInitializers(ByVal arrayType As ArrayTypeSymbol, ByVal inits As ImmutableArray(Of BoundExpression), ByVal includeConstants As Boolean)
			If (Not Me.IsMultidimensionalInitializer(inits)) Then
				Me.EmitOnedimensionalElementInitializers(arrayType, inits, includeConstants)
				Return
			End If
			Me.EmitMultidimensionalElementInitializers(arrayType, inits, includeConstants)
		End Sub

		Private Sub EmitExpression(ByVal expression As BoundExpression, ByVal used As Boolean)
			If (expression IsNot Nothing) Then
				Dim constantValueOpt As ConstantValue = expression.ConstantValueOpt
				If (constantValueOpt IsNot Nothing) Then
					If (Not used) Then
						Return
					End If
					If (Not constantValueOpt.IsDecimal AndAlso Not constantValueOpt.IsDateTime) Then
						Me.EmitConstantExpression(expression.Type, constantValueOpt, used, expression.Syntax)
						Return
					End If
				End If
				Me._recursionDepth = Me._recursionDepth + 1
				If (Me._recursionDepth <= 1) Then
					Me.EmitExpressionCoreWithStackGuard(expression, used)
				Else
					StackGuard.EnsureSufficientExecutionStack(Me._recursionDepth)
					Me.EmitExpressionCore(expression, used)
				End If
				Me._recursionDepth = Me._recursionDepth - 1
			End If
		End Sub

		Private Sub EmitExpressionCore(ByVal expression As BoundExpression, ByVal used As Boolean)
			Dim kind As BoundKind = expression.Kind
			If (kind <= BoundKind.ObjectCreationExpression) Then
				Select Case kind
					Case BoundKind.Dup
						Me.EmitDupExpression(DirectCast(expression, BoundDup), used)
						Return
					Case BoundKind.BadExpression
					Case BoundKind.BadStatement
					Case BoundKind.Parenthesized
					Case BoundKind.BadVariable
					Case BoundKind.TypeExpression
					Case BoundKind.TypeOrValueExpression
					Case BoundKind.NamespaceExpression
					Case BoundKind.UserDefinedUnaryOperator
					Case BoundKind.NullableIsTrueOperator
					Case BoundKind.UserDefinedBinaryOperator
					Case BoundKind.UserDefinedShortCircuitingOperator
					Case BoundKind.CompoundAssignmentTargetPlaceholder
					Case BoundKind.AddressOfOperator
					Case BoundKind.RelaxationLambda
					Case BoundKind.ConvertedTupleElements
					Case BoundKind.UserDefinedConversion
					Case BoundKind.SequencePoint
						Exit Select
					Case BoundKind.ArrayAccess
						Me.EmitArrayElementLoad(DirectCast(expression, BoundArrayAccess), used)
						Return
					Case BoundKind.ArrayLength
						Me.EmitArrayLengthExpression(DirectCast(expression, BoundArrayLength), used)
						Return
					Case BoundKind.[GetType]
						Me.EmitGetType(DirectCast(expression, BoundGetType), used)
						Return
					Case BoundKind.FieldInfo
						Me.EmitFieldInfoExpression(DirectCast(expression, BoundFieldInfo), used)
						Return
					Case BoundKind.MethodInfo
						Me.EmitMethodInfoExpression(DirectCast(expression, BoundMethodInfo), used)
						Return
					Case BoundKind.MethodDefIndex
						Me.EmitMethodDefIndexExpression(DirectCast(expression, BoundMethodDefIndex))
						Return
					Case BoundKind.MaximumMethodDefIndex
						Me.EmitMaximumMethodDefIndexExpression(DirectCast(expression, BoundMaximumMethodDefIndex))
						Return
					Case BoundKind.InstrumentationPayloadRoot
						Me.EmitInstrumentationPayloadRootLoad(DirectCast(expression, BoundInstrumentationPayloadRoot))
						Return
					Case BoundKind.ModuleVersionId
						Me.EmitModuleVersionIdLoad(DirectCast(expression, BoundModuleVersionId))
						Return
					Case BoundKind.ModuleVersionIdString
						Me.EmitModuleVersionIdStringLoad(DirectCast(expression, BoundModuleVersionIdString))
						Return
					Case BoundKind.SourceDocumentIndex
						Me.EmitSourceDocumentIndex(DirectCast(expression, BoundSourceDocumentIndex))
						Return
					Case BoundKind.UnaryOperator
						Me.EmitUnaryOperatorExpression(DirectCast(expression, BoundUnaryOperator), used)
						Return
					Case BoundKind.BinaryOperator
						Me.EmitBinaryOperatorExpression(DirectCast(expression, BoundBinaryOperator), used)
						Return
					Case BoundKind.AssignmentOperator
						Me.EmitAssignmentExpression(DirectCast(expression, BoundAssignmentOperator), used)
						Return
					Case BoundKind.ReferenceAssignment
						Me.EmitReferenceAssignment(DirectCast(expression, BoundReferenceAssignment), used, False)
						Return
					Case BoundKind.TernaryConditionalExpression
						Me.EmitTernaryConditionalExpression(DirectCast(expression, BoundTernaryConditionalExpression), used)
						Return
					Case BoundKind.BinaryConditionalExpression
						Me.EmitBinaryConditionalExpression(DirectCast(expression, BoundBinaryConditionalExpression), used)
						Return
					Case BoundKind.Conversion
						Me.EmitConversionExpression(DirectCast(expression, BoundConversion), used)
						Return
					Case BoundKind.[DirectCast]
						Me.EmitDirectCastExpression(DirectCast(expression, BoundDirectCast), used)
						Return
					Case BoundKind.[TryCast]
						Me.EmitTryCastExpression(DirectCast(expression, BoundTryCast), used)
						Return
					Case BoundKind.[TypeOf]
						Me.EmitTypeOfExpression(DirectCast(expression, BoundTypeOf), used, False)
						Return
					Case BoundKind.SequencePointExpression
						Me.EmitSequencePointExpression(DirectCast(expression, BoundSequencePointExpression), used)
						Return
					Case Else
						If (kind = BoundKind.[Call]) Then
							Me.EmitCallExpression(DirectCast(expression, BoundCall), If(used, CodeGenerator.UseKind.UsedAsValue, CodeGenerator.UseKind.Unused))
							Return
						End If
						If (kind = BoundKind.ObjectCreationExpression) Then
							Me.EmitObjectCreationExpression(DirectCast(expression, BoundObjectCreationExpression), used)
							Return
						End If
						Exit Select
				End Select
			ElseIf (kind > BoundKind.Sequence) Then
				Select Case kind
					Case BoundKind.MeReference
					Case BoundKind.MyClassReference
						If (Not used) Then
							Exit Select
						End If
						Me.EmitMeOrMyClassReferenceExpression(expression)
						Return
					Case BoundKind.ValueTypeMeReference
						Throw ExceptionUtilities.UnexpectedValue(expression.Kind)
					Case BoundKind.MyBaseReference
						If (Not used) Then
							Exit Select
						End If
						Me._builder.EmitOpCode(ILOpCode.Ldarg_0)
						Return
					Case BoundKind.PreviousSubmissionReference
					Case BoundKind.HostObjectMemberReference
						Throw ExceptionUtilities.UnexpectedValue(expression.Kind)
					Case BoundKind.Local
						Me.EmitLocalLoad(DirectCast(expression, BoundLocal), used)
						Return
					Case BoundKind.PseudoVariable
						Me.EmitPseudoVariableValue(DirectCast(expression, BoundPseudoVariable), used)
						Return
					Case BoundKind.Parameter
						If (Not used) Then
							Exit Select
						End If
						Me.EmitParameterLoad(DirectCast(expression, BoundParameter))
						Return
					Case Else
						Select Case kind
							Case BoundKind.ConditionalAccessReceiverPlaceholder
								Me.EmitConditionalAccessReceiverPlaceholder(DirectCast(expression, BoundConditionalAccessReceiverPlaceholder), used)
								Return
							Case BoundKind.LoweredConditionalAccess
								Me.EmitConditionalAccess(DirectCast(expression, BoundLoweredConditionalAccess), used)
								Return
							Case BoundKind.ComplexConditionalAccessReceiver
								Me.EmitComplexConditionalAccessReceiver(DirectCast(expression, BoundComplexConditionalAccessReceiver), used)
								Return
							Case Else
								Throw ExceptionUtilities.UnexpectedValue(expression.Kind)
						End Select

				End Select
				Return
			Else
				Select Case kind
					Case BoundKind.DelegateCreationExpression
						Me.EmitDelegateCreationExpression(DirectCast(expression, BoundDelegateCreationExpression), used)
						Return
					Case BoundKind.ArrayCreation
						Me.EmitArrayCreationExpression(DirectCast(expression, BoundArrayCreation), used)
						Return
					Case BoundKind.ArrayLiteral
					Case BoundKind.ArrayInitialization
						Exit Select
					Case BoundKind.FieldAccess
						Me.EmitFieldLoad(DirectCast(expression, BoundFieldAccess), used)
						Return
					Case Else
						If (kind = BoundKind.Sequence) Then
							Me.EmitSequenceExpression(DirectCast(expression, BoundSequence), used)
							Return
						End If
						Exit Select
				End Select
			End If
			Throw ExceptionUtilities.UnexpectedValue(expression.Kind)
		End Sub

		Private Sub EmitExpressionCoreWithStackGuard(ByVal expression As BoundExpression, ByVal used As Boolean)
			Try
				Me.EmitExpressionCore(expression, used)
			Catch insufficientExecutionStackException As System.InsufficientExecutionStackException
				ProjectData.SetProjectError(insufficientExecutionStackException)
				Me._diagnostics.Add(ERRID.ERR_TooLongOrComplexExpression, BoundTreeVisitor.CancelledByStackGuardException.GetTooLongOrComplexExpressionErrorLocation(expression))
				Throw New CodeGenerator.EmitCancelledException()
			End Try
		End Sub

		Private Sub EmitExpressions(ByVal expressions As ImmutableArray(Of BoundExpression), ByVal used As Boolean)
			For i As Integer = 0 To expressions.Length
				Me.EmitExpression(expressions(i), used)
			Next

		End Sub

		Private Sub EmitFieldAccess(ByVal fieldAccess As BoundFieldAccess)
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = fieldAccess.FieldSymbol
			If (Not fieldSymbol.IsShared) Then
				Me.EmitExpression(fieldAccess.ReceiverOpt, True)
			End If
			If (Not fieldSymbol.IsShared) Then
				Me._builder.EmitOpCode(ILOpCode.Ldfld)
			Else
				Me._builder.EmitOpCode(ILOpCode.Ldsfld)
			End If
			Me.EmitSymbolToken(fieldSymbol, fieldAccess.Syntax)
		End Sub

		Private Function EmitFieldAddress(ByVal fieldAccess As BoundFieldAccess, ByVal addressKind As CodeGenerator.AddressKind) As Microsoft.CodeAnalysis.CodeGen.LocalDefinition
			Dim localDefinition As Microsoft.CodeAnalysis.CodeGen.LocalDefinition
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = fieldAccess.FieldSymbol
			If (Not fieldAccess.FieldSymbol.IsShared) Then
				localDefinition = Me.EmitInstanceFieldAddress(fieldAccess, addressKind)
			Else
				Me.EmitStaticFieldAddress(fieldSymbol, fieldAccess.Syntax)
				localDefinition = Nothing
			End If
			Return localDefinition
		End Function

		Private Sub EmitFieldInfoExpression(ByVal node As BoundFieldInfo, ByVal used As Boolean)
			Dim wellKnownTypeMember As MethodSymbol
			Me._builder.EmitOpCode(ILOpCode.Ldtoken)
			Me.EmitSymbolToken(node.Field, node.Syntax)
			If (node.Field.ContainingType.IsGenericType) Then
				Me._builder.EmitOpCode(ILOpCode.Ldtoken)
				Me.EmitSymbolToken(node.Field.ContainingType, node.Syntax)
				Me._builder.EmitOpCode(ILOpCode.[Call], -1)
				wellKnownTypeMember = DirectCast(Me._module.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Reflection_FieldInfo__GetFieldFromHandle2), MethodSymbol)
			Else
				Me._builder.EmitOpCode(ILOpCode.[Call], 0)
				wellKnownTypeMember = DirectCast(Me._module.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Reflection_FieldInfo__GetFieldFromHandle), MethodSymbol)
			End If
			Me.EmitSymbolToken(wellKnownTypeMember, node.Syntax, False)
			If (Not TypeSymbol.Equals(node.Type, wellKnownTypeMember.ReturnType, TypeCompareKind.ConsiderEverything)) Then
				Me._builder.EmitOpCode(ILOpCode.Castclass)
				Me.EmitSymbolToken(node.Type, node.Syntax)
			End If
			Me.EmitPopIfUnused(used)
		End Sub

		Private Sub EmitFieldLoad(ByVal fieldAccess As BoundFieldAccess, ByVal used As Boolean)
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = fieldAccess.FieldSymbol
			If (Not used AndAlso Not fieldSymbol.IsShared AndAlso fieldAccess.ReceiverOpt.Type.IsVerifierValue()) Then
				Me.EmitExpression(fieldAccess.ReceiverOpt, False)
				Return
			End If
			Dim specialType As Microsoft.CodeAnalysis.SpecialType = fieldSymbol.Type.SpecialType
			If (fieldSymbol.IsConst AndAlso specialType <> Microsoft.CodeAnalysis.SpecialType.System_Decimal AndAlso specialType <> Microsoft.CodeAnalysis.SpecialType.System_DateTime) Then
				Throw ExceptionUtilities.Unreachable
			End If
			If (Not fieldSymbol.IsShared) Then
				Me.EmitInstanceFieldLoad(fieldAccess, used)
				Return
			End If
			Me.EmitStaticFieldLoad(fieldSymbol, used, fieldAccess.Syntax)
		End Sub

		Private Function EmitFieldLoadReceiverAddress(ByVal receiver As BoundExpression) As Boolean
			Dim flag As Boolean
			If (receiver Is Nothing OrElse receiver.Type.IsReferenceType) Then
				flag = False
			ElseIf (receiver.Kind <> BoundKind.[DirectCast] OrElse Not Me.IsUnboxingDirectCast(DirectCast(receiver, BoundDirectCast))) Then
				If (receiver.Kind = BoundKind.FieldAccess) Then
					Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = DirectCast(receiver, Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess)
					Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = boundFieldAccess.FieldSymbol
					If (fieldSymbol.IsShared OrElse Not Me.EmitFieldLoadReceiverAddress(boundFieldAccess.ReceiverOpt)) Then
						GoTo Label1
					End If
					Me._builder.EmitOpCode(ILOpCode.Ldflda)
					Me.EmitSymbolToken(fieldSymbol, boundFieldAccess.Syntax)
					flag = True
					Return flag
				End If
			Label1:
				flag = False
			Else
				Me.EmitExpression(DirectCast(receiver, BoundDirectCast).Operand, True)
				Me._builder.EmitOpCode(ILOpCode.Unbox)
				Me.EmitSymbolToken(receiver.Type, receiver.Syntax)
				flag = True
			End If
			Return flag
		End Function

		Private Sub EmitFieldStore(ByVal fieldAccess As BoundFieldAccess)
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = fieldAccess.FieldSymbol
			If (Not fieldSymbol.IsShared) Then
				Me._builder.EmitOpCode(ILOpCode.Stfld)
			Else
				Me._builder.EmitOpCode(ILOpCode.Stsfld)
			End If
			Me.EmitSymbolToken(fieldSymbol, fieldAccess.Syntax)
		End Sub

		Private Sub EmitGetType(ByVal boundTypeOfOperator As BoundGetType, ByVal used As Boolean)
			Dim type As TypeSymbol = boundTypeOfOperator.SourceType.Type
			Me._builder.EmitOpCode(ILOpCode.Ldtoken)
			Me.EmitSymbolToken(type, boundTypeOfOperator.SourceType.Syntax)
			Me._builder.EmitOpCode(ILOpCode.[Call], 0)
			Dim wellKnownTypeMember As MethodSymbol = DirectCast(Me._module.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Type__GetTypeFromHandle), MethodSymbol)
			Me.EmitSymbolToken(wellKnownTypeMember, boundTypeOfOperator.Syntax, False)
			Me.EmitPopIfUnused(used)
		End Sub

		Private Sub EmitGotoStatement(ByVal boundGotoStatement As Microsoft.CodeAnalysis.VisualBasic.BoundGotoStatement)
			If (Me.ShouldNoteProjectErrors() AndAlso Me._currentCatchBlock IsNot Nothing AndAlso (Me._currentCatchBlock.ExceptionFilterOpt Is Nothing OrElse Me._currentCatchBlock.ExceptionFilterOpt.Kind <> BoundKind.UnstructuredExceptionHandlingCatchFilter) AndAlso Not CodeGenerator.LabelFinder.NodeContainsLabel(Me._currentCatchBlock, boundGotoStatement.Label)) Then
				Me.EmitClearProjectError(boundGotoStatement.Syntax)
			End If
			Me._builder.EmitBranch(ILOpCode.Br, boundGotoStatement.Label, ILOpCode.Nop)
		End Sub

		Private Sub EmitHiddenSequencePoint()
			Me._builder.DefineHiddenSequencePoint()
		End Sub

		Private Sub EmitInitObj(ByVal type As TypeSymbol, ByVal used As Boolean, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode)
			If (used) Then
				Dim localDefinition As Microsoft.CodeAnalysis.CodeGen.LocalDefinition = Me.AllocateTemp(type, syntaxNode)
				Me._builder.EmitLocalAddress(localDefinition)
				Me._builder.EmitOpCode(ILOpCode.Initobj)
				Me.EmitSymbolToken(type, syntaxNode)
				Me._builder.EmitLocalLoad(localDefinition)
				Me.FreeTemp(localDefinition)
			End If
		End Sub

		Private Sub EmitInitObjOnTarget(ByVal target As BoundExpression)
			If (target.Kind = BoundKind.Local AndAlso Me.IsStackLocal(DirectCast(target, BoundLocal).LocalSymbol)) Then
				Me.EmitInitObj(target.Type, True, target.Syntax)
				Return
			End If
			Me.EmitAddress(target, CodeGenerator.AddressKind.Immutable)
			Me._builder.EmitOpCode(ILOpCode.Initobj)
			Me.EmitSymbolToken(target.Type, target.Syntax)
		End Sub

		Private Function EmitInstanceFieldAddress(ByVal fieldAccess As BoundFieldAccess, ByVal addressKind As CodeGenerator.AddressKind) As Microsoft.CodeAnalysis.CodeGen.LocalDefinition
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = fieldAccess.FieldSymbol
			If (addressKind = CodeGenerator.AddressKind.Writeable AndAlso Me.IsMeReceiver(fieldAccess.ReceiverOpt)) Then
				addressKind = CodeGenerator.AddressKind.[ReadOnly]
			End If
			Dim localDefinition As Microsoft.CodeAnalysis.CodeGen.LocalDefinition = Me.EmitReceiverRef(fieldAccess.ReceiverOpt, False, addressKind)
			Me._builder.EmitOpCode(ILOpCode.Ldflda)
			Me.EmitSymbolToken(fieldSymbol, fieldAccess.Syntax)
			Return localDefinition
		End Function

		Private Sub EmitInstanceFieldLoad(ByVal fieldAccess As BoundFieldAccess, ByVal used As Boolean)
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = fieldAccess.FieldSymbol
			Dim receiverOpt As BoundExpression = fieldAccess.ReceiverOpt
			If (Not CodeGenerator.FieldLoadMustUseRef(receiverOpt) AndAlso Not Me.FieldLoadPrefersRef(receiverOpt)) Then
				Me.EmitExpression(receiverOpt, True)
			ElseIf (Not Me.EmitFieldLoadReceiverAddress(receiverOpt)) Then
				Me.EmitReceiverRef(receiverOpt, False, CodeGenerator.AddressKind.Immutable)
			End If
			Me._builder.EmitOpCode(ILOpCode.Ldfld)
			Me.EmitSymbolToken(fieldSymbol, fieldAccess.Syntax)
			Me.EmitPopIfUnused(used)
		End Sub

		Private Sub EmitInstrumentationPayloadRootLoad(ByVal node As BoundInstrumentationPayloadRoot)
			Me._builder.EmitOpCode(ILOpCode.Ldsfld)
			Me.EmitInstrumentationPayloadRootToken(node)
		End Sub

		Private Sub EmitInstrumentationPayloadRootStore(ByVal node As BoundInstrumentationPayloadRoot)
			Me._builder.EmitOpCode(ILOpCode.Stsfld)
			Me.EmitInstrumentationPayloadRootToken(node)
		End Sub

		Private Sub EmitInstrumentationPayloadRootToken(ByVal node As BoundInstrumentationPayloadRoot)
			Me._builder.EmitToken(Me._module.GetInstrumentationPayloadRoot(node.AnalysisKind, Me._module.Translate(node.Type, node.Syntax, Me._diagnostics), node.Syntax, Me._diagnostics), node.Syntax, Me._diagnostics, False)
		End Sub

		Private Sub EmitIsNotNullOrZero(ByVal comparand As BoundExpression, ByVal nullOrZero As ConstantValue)
			Me.EmitExpression(comparand, True)
			Dim type As TypeSymbol = comparand.Type
			If (type.IsReferenceType AndAlso Not type.IsVerifierReference()) Then
				Me.EmitBox(type, comparand.Syntax)
			End If
			Me._builder.EmitConstantValue(nullOrZero)
			Me._builder.EmitOpCode(ILOpCode.Cgt_un)
		End Sub

		Private Sub EmitIsNullOrZero(ByVal comparand As BoundExpression, ByVal nullOrZero As ConstantValue)
			Me.EmitExpression(comparand, True)
			Dim type As TypeSymbol = comparand.Type
			If (type.IsReferenceType AndAlso Not type.IsVerifierReference()) Then
				Me.EmitBox(type, comparand.Syntax)
			End If
			Me._builder.EmitConstantValue(nullOrZero)
			Me._builder.EmitOpCode(ILOpCode.Ceq)
		End Sub

		Private Sub EmitIsSense(ByVal sense As Boolean)
			If (Not sense) Then
				Me._builder.EmitOpCode(ILOpCode.Ldc_i4_0)
				Me._builder.EmitOpCode(ILOpCode.Ceq)
			End If
		End Sub

		Private Sub EmitLabelStatement(ByVal boundLabelStatement As Microsoft.CodeAnalysis.VisualBasic.BoundLabelStatement)
			Me._builder.MarkLabel(boundLabelStatement.Label)
		End Sub

		Private Sub EmitLoadDefaultValueOfTypeFromNothingLiteral(ByVal type As TypeSymbol, ByVal used As Boolean, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode)
			Me.EmitInitObj(type, used, syntaxNode)
		End Sub

		Private Sub EmitLoadDefaultValueOfTypeParameter(ByVal type As TypeSymbol, ByVal used As Boolean, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode)
			Me.EmitLoadDefaultValueOfTypeFromNothingLiteral(type, used, syntaxNode)
		End Sub

		Private Sub EmitLoadIndirect(ByVal type As TypeSymbol, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode)
			If (type.IsEnumType()) Then
				type = DirectCast(type, NamedTypeSymbol).EnumUnderlyingType
			End If
			Select Case type.PrimitiveTypeCode
				Case Microsoft.Cci.PrimitiveTypeCode.[Boolean]
				Case Microsoft.Cci.PrimitiveTypeCode.UInt8
					Me._builder.EmitOpCode(ILOpCode.Ldind_u1)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.[Char]
				Case Microsoft.Cci.PrimitiveTypeCode.UInt16
					Me._builder.EmitOpCode(ILOpCode.Ldind_u2)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.Int8
					Me._builder.EmitOpCode(ILOpCode.Ldind_i1)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.Float32
					Me._builder.EmitOpCode(ILOpCode.Ldind_r4)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.Float64
					Me._builder.EmitOpCode(ILOpCode.Ldind_r8)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.Int16
					Me._builder.EmitOpCode(ILOpCode.Ldind_i2)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.Int32
					Me._builder.EmitOpCode(ILOpCode.Ldind_i4)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.Int64
				Case Microsoft.Cci.PrimitiveTypeCode.UInt64
					Me._builder.EmitOpCode(ILOpCode.Ldind_i8)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.IntPtr
				Case Microsoft.Cci.PrimitiveTypeCode.Pointer
				Case Microsoft.Cci.PrimitiveTypeCode.UIntPtr
					Me._builder.EmitOpCode(ILOpCode.Ldind_i)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.Reference
				Case Microsoft.Cci.PrimitiveTypeCode.[String]
					If (type.IsVerifierReference()) Then
						Me._builder.EmitOpCode(ILOpCode.Ldind_ref)
						Return
					End If
					Me._builder.EmitOpCode(ILOpCode.Ldobj)
					Me.EmitSymbolToken(type, syntaxNode)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.UInt32
					Me._builder.EmitOpCode(ILOpCode.Ldind_u4)
					Return
				Case Else
					If (type.IsVerifierReference()) Then
						Me._builder.EmitOpCode(ILOpCode.Ldind_ref)
						Return
					End If
					Me._builder.EmitOpCode(ILOpCode.Ldobj)
					Me.EmitSymbolToken(type, syntaxNode)
					Return
			End Select
		End Sub

		Private Sub EmitLocalLoad(ByVal local As BoundLocal, ByVal used As Boolean)
			If (Not Me.IsStackLocal(local.LocalSymbol)) Then
				If (Not used) Then
					Return
				End If
				Me._builder.EmitLocalLoad(Me.GetLocal(local))
			Else
				Me.EmitPopIfUnused(used)
			End If
			If (used AndAlso local.LocalSymbol.IsByRef) Then
				Me.EmitLoadIndirect(local.Type, local.Syntax)
			End If
		End Sub

		Private Sub EmitLocalStore(ByVal local As BoundLocal)
			Dim localDefinition As Microsoft.CodeAnalysis.CodeGen.LocalDefinition = Me.GetLocal(local)
			Me._builder.EmitLocalStore(localDefinition)
		End Sub

		Private Sub EmitMaximumMethodDefIndexExpression(ByVal node As BoundMaximumMethodDefIndex)
			Me._builder.EmitOpCode(ILOpCode.Ldtoken)
			Me._builder.EmitGreatestMethodToken()
		End Sub

		Private Sub EmitMeOrMyClassReferenceExpression(ByVal thisRef As BoundExpression)
			Dim type As TypeSymbol = thisRef.Type
			Me._builder.EmitOpCode(ILOpCode.Ldarg_0)
			If (type.IsValueType) Then
				Me._builder.EmitOpCode(ILOpCode.Ldobj)
				Me.EmitSymbolToken(thisRef.Type, thisRef.Syntax)
			End If
		End Sub

		Private Sub EmitMeStore(ByVal thisRef As BoundMeReference)
			Me._builder.EmitOpCode(ILOpCode.Stobj)
			Me.EmitSymbolToken(thisRef.Type, thisRef.Syntax)
		End Sub

		Private Sub EmitMethodDefIndexExpression(ByVal node As BoundMethodDefIndex)
			Me._builder.EmitOpCode(ILOpCode.Ldtoken)
			Me.EmitSymbolToken(If(node.Method.PartialDefinitionPart, node.Method), node.Syntax, True)
		End Sub

		Private Sub EmitMethodInfoExpression(ByVal node As BoundMethodInfo, ByVal used As Boolean)
			Dim wellKnownTypeMember As MethodSymbol
			Dim method As MethodSymbol = node.Method
			If (method.IsTupleMethod) Then
				method = method.TupleUnderlyingMethod
			End If
			Me._builder.EmitOpCode(ILOpCode.Ldtoken)
			Me.EmitSymbolToken(method, node.Syntax, False)
			If (method.ContainingType.IsGenericType OrElse method.ContainingType.IsAnonymousType) Then
				Me._builder.EmitOpCode(ILOpCode.Ldtoken)
				Me.EmitSymbolToken(method.ContainingType, node.Syntax)
				Me._builder.EmitOpCode(ILOpCode.[Call], -1)
				wellKnownTypeMember = DirectCast(Me._module.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Reflection_MethodBase__GetMethodFromHandle2), MethodSymbol)
			Else
				Me._builder.EmitOpCode(ILOpCode.[Call], 0)
				wellKnownTypeMember = DirectCast(Me._module.Compilation.GetWellKnownTypeMember(WellKnownMember.System_Reflection_MethodBase__GetMethodFromHandle), MethodSymbol)
			End If
			Me.EmitSymbolToken(wellKnownTypeMember, node.Syntax, False)
			If (Not TypeSymbol.Equals(node.Type, wellKnownTypeMember.ReturnType, TypeCompareKind.ConsiderEverything)) Then
				Me._builder.EmitOpCode(ILOpCode.Castclass)
				Me.EmitSymbolToken(node.Type, node.Syntax)
			End If
			Me.EmitPopIfUnused(used)
		End Sub

		Private Sub EmitModuleVersionIdLoad(ByVal node As BoundModuleVersionId)
			Me._builder.EmitOpCode(ILOpCode.Ldsfld)
			Me.EmitModuleVersionIdToken(node)
		End Sub

		Private Sub EmitModuleVersionIdStore(ByVal node As BoundModuleVersionId)
			Me._builder.EmitOpCode(ILOpCode.Stsfld)
			Me.EmitModuleVersionIdToken(node)
		End Sub

		Private Sub EmitModuleVersionIdStringLoad(ByVal node As BoundModuleVersionIdString)
			Me._builder.EmitOpCode(ILOpCode.Ldstr)
			Me._builder.EmitModuleVersionIdStringToken()
		End Sub

		Private Sub EmitModuleVersionIdToken(ByVal node As BoundModuleVersionId)
			Me._builder.EmitToken(Me._module.GetModuleVersionId(Me._module.Translate(node.Type, node.Syntax, Me._diagnostics), node.Syntax, Me._diagnostics), node.Syntax, Me._diagnostics, False)
		End Sub

		Private Sub EmitMultidimensionalElementInitializers(ByVal arrayType As ArrayTypeSymbol, ByVal inits As ImmutableArray(Of BoundExpression), ByVal includeConstants As Boolean)
			Dim indexDescs As ArrayBuilder(Of CodeGenerator.IndexDesc) = New ArrayBuilder(Of CodeGenerator.IndexDesc)()
			Dim length As Integer = inits.Length - 1
			For i As Integer = 0 To length
				indexDescs.Push(New CodeGenerator.IndexDesc(i, DirectCast(inits(i), BoundArrayInitialization).Initializers))
				Me.EmitAllElementInitializersRecursive(arrayType, indexDescs, includeConstants)
			Next

		End Sub

		Private Sub EmitNewObj(ByVal constructor As MethodSymbol, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal used As Boolean, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode)
			Me.EmitArguments(arguments, constructor.Parameters)
			Me._builder.EmitOpCode(ILOpCode.Newobj, ILOpCode.Newobj.StackPushCount() - arguments.Length)
			Me.EmitSymbolToken(constructor, syntaxNode, False)
			Me.EmitPopIfUnused(used)
		End Sub

		Private Sub EmitNoOpStatement(ByVal statement As BoundNoOpStatement)
			Select Case statement.Flavor
				Case NoOpStatementFlavor.[Default]
					If (Me._ilEmitStyle <> ILEmitStyle.Debug) Then
						Return
					End If
					Me._builder.EmitOpCode(ILOpCode.Nop)
					Return
				Case NoOpStatementFlavor.AwaitYieldPoint
					If (Me._asyncYieldPoints Is Nothing) Then
						Me._asyncYieldPoints = ArrayBuilder(Of Integer).GetInstance()
						Me._asyncResumePoints = ArrayBuilder(Of Integer).GetInstance()
					End If
					Me._asyncYieldPoints.Add(Me._builder.AllocateILMarker())
					Return
				Case NoOpStatementFlavor.AwaitResumePoint
					Me._asyncResumePoints.Add(Me._builder.AllocateILMarker())
					Return
			End Select
			Throw ExceptionUtilities.UnexpectedValue(statement.Flavor)
		End Sub

		Private Sub EmitObjectCreationExpression(ByVal expression As BoundObjectCreationExpression, ByVal used As Boolean)
			If (expression.IsDefaultValue()) Then
				Me.EmitInitObj(expression.Type, used, expression.Syntax)
				Return
			End If
			Dim constructorOpt As MethodSymbol = expression.ConstructorOpt
			Me.EmitNewObj(expression.ConstructorOpt, expression.Arguments, used, expression.Syntax)
		End Sub

		Private Sub EmitOnedimensionalElementInitializers(ByVal arrayType As ArrayTypeSymbol, ByVal inits As ImmutableArray(Of BoundExpression), ByVal includeConstants As Boolean)
			Dim length As Integer = inits.Length - 1
			For i As Integer = 0 To length
				Dim item As BoundExpression = inits(i)
				If (CodeGenerator.ShouldEmitInitExpression(includeConstants, item)) Then
					Me._builder.EmitOpCode(ILOpCode.Dup)
					Me._builder.EmitIntConstant(i)
					Me.EmitExpression(item, True)
					Me.EmitArrayElementStore(arrayType, item.Syntax)
				End If
			Next

		End Sub

		Private Sub EmitParameterAddress(ByVal parameter As BoundParameter)
			Dim num As Integer = Me.ParameterSlot(parameter)
			If (Not parameter.ParameterSymbol.IsByRef) Then
				Me._builder.EmitLoadArgumentAddrOpcode(num)
				Return
			End If
			Me._builder.EmitLoadArgumentOpcode(num)
		End Sub

		Private Sub EmitParameterLoad(ByVal parameter As BoundParameter)
			Dim num As Integer = Me.ParameterSlot(parameter)
			Me._builder.EmitLoadArgumentOpcode(num)
			If (parameter.ParameterSymbol.IsByRef) Then
				Me.EmitLoadIndirect(parameter.ParameterSymbol.Type, parameter.Syntax)
			End If
		End Sub

		Private Sub EmitParameterStore(ByVal parameter As BoundParameter)
			If (parameter.ParameterSymbol.IsByRef) Then
				Me.EmitStoreIndirect(parameter.ParameterSymbol.Type, parameter.Syntax)
				Return
			End If
			Dim num As Integer = Me.ParameterSlot(parameter)
			Me._builder.EmitStoreArgumentOpcode(num)
		End Sub

		Private Sub EmitPopIfUnused(ByVal used As Boolean)
			If (Not used) Then
				Me._builder.EmitOpCode(ILOpCode.Pop)
			End If
		End Sub

		Private Sub EmitPseudoVariableAddress(ByVal expression As BoundPseudoVariable)
			Me.EmitExpression(expression.EmitExpressions.GetAddress(expression), True)
		End Sub

		Private Sub EmitPseudoVariableValue(ByVal expression As BoundPseudoVariable, ByVal used As Boolean)
			Me.EmitExpression(expression.EmitExpressions.GetValue(expression, Me._diagnostics), used)
		End Sub

		Private Function EmitReceiverRef(ByVal receiver As BoundExpression, ByVal isAccessConstrained As Boolean, ByVal addressKind As CodeGenerator.AddressKind) As Microsoft.CodeAnalysis.CodeGen.LocalDefinition
			Dim localDefinition As Microsoft.CodeAnalysis.CodeGen.LocalDefinition
			Dim type As TypeSymbol = receiver.Type
			If (type.IsVerifierReference()) Then
				Me.EmitExpression(receiver, True)
				localDefinition = Nothing
			ElseIf (type.TypeKind <> Microsoft.CodeAnalysis.TypeKind.TypeParameter) Then
				localDefinition = Me.EmitAddress(receiver, addressKind)
			ElseIf (Not isAccessConstrained) Then
				Me.EmitExpression(receiver, True)
				If (receiver.Kind <> BoundKind.ConditionalAccessReceiverPlaceholder) Then
					Me.EmitBox(type, receiver.Syntax)
				End If
				localDefinition = Nothing
			Else
				localDefinition = Me.EmitAddress(receiver, CodeGenerator.AddressKind.[ReadOnly])
			End If
			Return localDefinition
		End Function

		Private Sub EmitReferenceAssignment(ByVal capture As BoundReferenceAssignment, ByVal used As Boolean, Optional ByVal needReference As Boolean = False)
			Me.EmitAddress(capture.LValue, CodeGenerator.AddressKind.Writeable)
			If (used) Then
				Me._builder.EmitOpCode(ILOpCode.Dup)
			End If
			Dim byRefLocal As BoundLocal = capture.ByRefLocal
			If (Not Me.IsStackLocal(byRefLocal.LocalSymbol)) Then
				Dim local As LocalDefinition = Me.GetLocal(byRefLocal)
				Me._builder.EmitLocalStore(local)
			End If
			If (used AndAlso Not needReference) Then
				Me.EmitLoadIndirect(capture.Type, capture.Syntax)
			End If
		End Sub

		Private Sub EmitReturnStatement(ByVal boundReturnStatement As Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement)
			Me.EmitExpression(boundReturnStatement.ExpressionOpt, True)
			Me._builder.EmitRet(boundReturnStatement.ExpressionOpt Is Nothing)
		End Sub

		Private Sub EmitSelectStatement(ByVal boundSelectStatement As Microsoft.CodeAnalysis.VisualBasic.BoundSelectStatement)
			Dim expression As BoundExpression = boundSelectStatement.ExpressionStatement.Expression
			Dim caseBlocks As ImmutableArray(Of BoundCaseBlock) = boundSelectStatement.CaseBlocks
			Dim exitLabel As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol = boundSelectStatement.ExitLabel
			Dim labelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LabelSymbol = exitLabel
			Dim generatedLabelSymbols As ImmutableArray(Of GeneratedLabelSymbol) = Me.CreateCaseBlockLabels(caseBlocks)
			Dim caseLabelsForEmitSwitchHeader As KeyValuePair(Of ConstantValue, Object)() = Me.GetCaseLabelsForEmitSwitchHeader(caseBlocks, generatedLabelSymbols, labelSymbol)
			Me.EmitSwitchTableHeader(expression, caseLabelsForEmitSwitchHeader, labelSymbol)
			Me.EmitCaseBlocks(caseBlocks, generatedLabelSymbols, exitLabel)
			Me._builder.MarkLabel(exitLabel)
		End Sub

		Private Function EmitSequenceAddress(ByVal sequence As BoundSequence, ByVal addressKind As CodeGenerator.AddressKind) As LocalDefinition
			Dim isEmpty As Boolean = Not sequence.Locals.IsEmpty
			If (isEmpty) Then
				Me._builder.OpenLocalScope(ScopeType.Variable, Nothing)
				Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol).Enumerator = sequence.Locals.GetEnumerator()
				While enumerator.MoveNext()
					Me.DefineLocal(enumerator.Current, sequence.Syntax)
				End While
			End If
			Me.EmitSideEffects(sequence.SideEffects)
			Dim local As LocalDefinition = Me.EmitAddress(sequence.ValueOpt, addressKind)
			Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Nothing
			If (local Is Nothing) Then
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Me.DigForLocal(sequence.ValueOpt)
				If (boundLocal IsNot Nothing) Then
					localSymbol = boundLocal.LocalSymbol
				End If
			End If
			If (isEmpty) Then
				Me._builder.CloseLocalScope()
				Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol).Enumerator = sequence.Locals.GetEnumerator()
				While enumerator1.MoveNext()
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = enumerator1.Current
					If (CObj(current) = CObj(localSymbol)) Then
						local = Me.GetLocal(localSymbol)
					Else
						Me.FreeLocal(current)
					End If
				End While
			End If
			Return local
		End Function

		Private Sub EmitSequenceCondBranch(ByVal sequence As BoundSequence, ByRef lazyDest As Object, ByVal sense As Boolean)
			Dim isEmpty As Boolean = Not sequence.Locals.IsEmpty
			If (isEmpty) Then
				Me._builder.OpenLocalScope(ScopeType.Variable, Nothing)
				Dim enumerator As ImmutableArray(Of LocalSymbol).Enumerator = sequence.Locals.GetEnumerator()
				While enumerator.MoveNext()
					Me.DefineLocal(enumerator.Current, sequence.Syntax)
				End While
			End If
			Me.EmitSideEffects(sequence.SideEffects)
			Me.EmitCondBranch(sequence.ValueOpt, lazyDest, sense)
			If (isEmpty) Then
				Me._builder.CloseLocalScope()
				Dim enumerator1 As ImmutableArray(Of LocalSymbol).Enumerator = sequence.Locals.GetEnumerator()
				While enumerator1.MoveNext()
					Me.FreeLocal(enumerator1.Current)
				End While
			End If
		End Sub

		Private Sub EmitSequenceExpression(ByVal sequence As BoundSequence, ByVal used As Boolean)
			Dim isEmpty As Boolean = Not sequence.Locals.IsEmpty
			If (isEmpty) Then
				Me._builder.OpenLocalScope(ScopeType.Variable, Nothing)
				Dim enumerator As ImmutableArray(Of LocalSymbol).Enumerator = sequence.Locals.GetEnumerator()
				While enumerator.MoveNext()
					Me.DefineLocal(enumerator.Current, sequence.Syntax)
				End While
			End If
			Me.EmitSideEffects(sequence.SideEffects)
			Me.EmitExpression(sequence.ValueOpt, used)
			If (isEmpty) Then
				Me._builder.CloseLocalScope()
				Dim enumerator1 As ImmutableArray(Of LocalSymbol).Enumerator = sequence.Locals.GetEnumerator()
				While enumerator1.MoveNext()
					Me.FreeLocal(enumerator1.Current)
				End While
			End If
		End Sub

		Private Sub EmitSequencePoint(ByVal syntax As SyntaxNode)
			Me.EmitSequencePoint(syntax.SyntaxTree, syntax.Span)
		End Sub

		Private Function EmitSequencePoint(ByVal tree As SyntaxTree, ByVal span As TextSpan) As TextSpan
			Me._builder.DefineSequencePoint(tree, span)
			Return span
		End Function

		Private Sub EmitSequencePointExpression(ByVal node As BoundSequencePointExpression, ByVal used As Boolean)
			Dim syntax As SyntaxNode = node.Syntax
			If (Me._emitPdbSequencePoints) Then
				If (syntax IsNot Nothing) Then
					Me.EmitSequencePoint(syntax)
				Else
					Me.EmitHiddenSequencePoint()
				End If
			End If
			Me.EmitExpression(node.Expression, True)
			Me.EmitPopIfUnused(used)
		End Sub

		Private Sub EmitSequencePointExpressionAddress(ByVal node As BoundSequencePointExpression, ByVal addressKind As CodeGenerator.AddressKind)
			Dim syntax As SyntaxNode = node.Syntax
			If (Me._emitPdbSequencePoints) Then
				If (syntax IsNot Nothing) Then
					Me.EmitSequencePoint(syntax)
				Else
					Me.EmitHiddenSequencePoint()
				End If
			End If
			Me.EmitAddress(node.Expression, addressKind)
		End Sub

		Private Sub EmitSequencePointStatement(ByVal node As BoundSequencePoint)
			Dim syntax As SyntaxNode = node.Syntax
			If (Me._emitPdbSequencePoints) Then
				If (syntax IsNot Nothing) Then
					Me.EmitSequencePoint(syntax)
				Else
					Me.EmitHiddenSequencePoint()
				End If
			End If
			Dim statementOpt As BoundStatement = node.StatementOpt
			Dim num As Integer = 0
			If (statementOpt IsNot Nothing) Then
				num = Me.EmitStatementAndCountInstructions(statementOpt)
			End If
			If (num = 0 AndAlso syntax IsNot Nothing AndAlso Me._ilEmitStyle = ILEmitStyle.Debug) Then
				Me._builder.EmitOpCode(ILOpCode.Nop)
			End If
		End Sub

		Private Sub EmitSequencePointStatement(ByVal node As BoundSequencePointWithSpan)
			Dim span As Microsoft.CodeAnalysis.Text.TextSpan = node.Span
			Dim textSpan As Microsoft.CodeAnalysis.Text.TextSpan = New Microsoft.CodeAnalysis.Text.TextSpan()
			If (span <> textSpan AndAlso Me._emitPdbSequencePoints) Then
				Me.EmitSequencePoint(node.SyntaxTree, span)
			End If
			Dim statementOpt As BoundStatement = node.StatementOpt
			Dim num As Integer = 0
			If (statementOpt IsNot Nothing) Then
				num = Me.EmitStatementAndCountInstructions(statementOpt)
			End If
			If (num = 0) Then
				textSpan = New Microsoft.CodeAnalysis.Text.TextSpan()
				If (span <> textSpan AndAlso Me._ilEmitStyle = ILEmitStyle.Debug) Then
					Me._builder.EmitOpCode(ILOpCode.Nop)
				End If
			End If
		End Sub

		Private Sub EmitSetProjectError(ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal errorLineNumberOpt As BoundExpression)
			Dim wellKnownTypeMember As MethodSymbol
			If (errorLineNumberOpt IsNot Nothing) Then
				wellKnownTypeMember = DirectCast(Me._module.Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__SetProjectError_Int32), MethodSymbol)
				Me.EmitExpression(errorLineNumberOpt, True)
				Me._builder.EmitOpCode(ILOpCode.[Call], -2)
			Else
				wellKnownTypeMember = DirectCast(Me._module.Compilation.GetWellKnownTypeMember(WellKnownMember.Microsoft_VisualBasic_CompilerServices_ProjectData__SetProjectError), MethodSymbol)
				Me._builder.EmitOpCode(ILOpCode.[Call], -1)
			End If
			Me.EmitSymbolToken(wellKnownTypeMember, syntaxNode, False)
		End Sub

		Private Sub EmitShortCircuitingOperator(ByVal condition As BoundBinaryOperator, ByVal sense As Boolean, ByVal stopSense As Boolean, ByVal stopValue As Boolean)
			Dim obj As Object = Nothing
			Me.EmitCondBranch(condition.Left, obj, stopSense)
			Me.EmitCondExpr(condition.Right, sense)
			If (obj IsNot Nothing) Then
				Dim objectValue As Object = RuntimeHelpers.GetObjectValue(New [Object]())
				Me._builder.EmitBranch(ILOpCode.Br, RuntimeHelpers.GetObjectValue(objectValue), ILOpCode.Nop)
				Me._builder.AdjustStack(-1)
				Me._builder.MarkLabel(RuntimeHelpers.GetObjectValue(obj))
				Me._builder.EmitBoolConstant(stopValue)
				Me._builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue))
			End If
		End Sub

		Private Sub EmitSideEffects(ByVal sideEffects As ImmutableArray(Of BoundExpression))
			If (Not sideEffects.IsDefaultOrEmpty) Then
				Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = sideEffects.GetEnumerator()
				While enumerator.MoveNext()
					Me.EmitExpression(enumerator.Current, False)
				End While
			End If
		End Sub

		Private Sub EmitSourceDocumentIndex(ByVal node As BoundSourceDocumentIndex)
			Me._builder.EmitOpCode(ILOpCode.Ldtoken)
			Me._builder.EmitSourceDocumentIndexToken(node.Document)
		End Sub

		Private Sub EmitStateMachineScope(ByVal scope As BoundStateMachineScope)
			Me._builder.OpenLocalScope(ScopeType.StateMachineVariable, Nothing)
			Dim enumerator As ImmutableArray(Of FieldSymbol).Enumerator = scope.Fields.GetEnumerator()
			While enumerator.MoveNext()
				Me.DefineUserDefinedStateMachineHoistedLocal(DirectCast(enumerator.Current, StateMachineFieldSymbol))
			End While
			Me.EmitStatement(scope.Statement)
			Me._builder.CloseLocalScope()
		End Sub

		Private Sub EmitStatement(ByVal statement As BoundStatement)
			Dim kind As BoundKind = statement.Kind
			If (kind <= BoundKind.ExpressionStatement) Then
				If (kind > BoundKind.Block) Then
					If (kind = BoundKind.StateMachineScope) Then
						Me.EmitStateMachineScope(DirectCast(statement, BoundStateMachineScope))
						Return
					End If
					If (kind = BoundKind.ExpressionStatement) Then
						Me.EmitExpression(DirectCast(statement, BoundExpressionStatement).Expression, False)
						Return
					End If
				Else
					Select Case kind
						Case BoundKind.SequencePoint
							Me.EmitSequencePointStatement(DirectCast(statement, BoundSequencePoint))
							Return
						Case BoundKind.SequencePointExpression
						Case BoundKind.MethodGroup
						Case BoundKind.PropertyGroup
						Case BoundKind.YieldStatement
							Exit Select
						Case BoundKind.SequencePointWithSpan
							Me.EmitSequencePointStatement(DirectCast(statement, BoundSequencePointWithSpan))
							Return
						Case BoundKind.NoOpStatement
							Me.EmitNoOpStatement(DirectCast(statement, BoundNoOpStatement))
							Return
						Case BoundKind.ReturnStatement
							Me.EmitReturnStatement(DirectCast(statement, BoundReturnStatement))
							Return
						Case BoundKind.ThrowStatement
							Me.EmitThrowStatement(DirectCast(statement, BoundThrowStatement))
							Return
						Case Else
							If (kind = BoundKind.Block) Then
								Me.EmitBlock(DirectCast(statement, BoundBlock))
								Return
							End If
							Exit Select
					End Select
				End If
			ElseIf (kind > BoundKind.TryStatement) Then
				Select Case kind
					Case BoundKind.LabelStatement
						Me.EmitLabelStatement(DirectCast(statement, BoundLabelStatement))
						Return
					Case BoundKind.Label
						Exit Select
					Case BoundKind.GotoStatement
						Me.EmitGotoStatement(DirectCast(statement, BoundGotoStatement))
						Return
					Case BoundKind.StatementList
						Dim boundStatementList As Microsoft.CodeAnalysis.VisualBasic.BoundStatementList = DirectCast(statement, Microsoft.CodeAnalysis.VisualBasic.BoundStatementList)
						Dim statements As ImmutableArray(Of BoundStatement) = boundStatementList.Statements
						Dim length As Integer = statements.Length - 1
						Dim num As Integer = 0
						Do
							statements = boundStatementList.Statements
							Me.EmitStatement(statements(num))
							num = num + 1
						Loop While num <= length
						Return
					Case BoundKind.ConditionalGoto
						Me.EmitConditionalGoto(DirectCast(statement, BoundConditionalGoto))
						Return
					Case Else
						If (kind = BoundKind.UnstructuredExceptionOnErrorSwitch) Then
							Me.EmitUnstructuredExceptionOnErrorSwitch(DirectCast(statement, BoundUnstructuredExceptionOnErrorSwitch))
							Return
						End If
						If (kind = BoundKind.UnstructuredExceptionResumeSwitch) Then
							Me.EmitUnstructuredExceptionResumeSwitch(DirectCast(statement, BoundUnstructuredExceptionResumeSwitch))
							Return
						End If
						Exit Select
				End Select
			Else
				If (kind = BoundKind.SelectStatement) Then
					Me.EmitSelectStatement(DirectCast(statement, BoundSelectStatement))
					Return
				End If
				If (kind = BoundKind.TryStatement) Then
					Me.EmitTryStatement(DirectCast(statement, BoundTryStatement), False)
					Return
				End If
			End If
			Throw ExceptionUtilities.UnexpectedValue(statement.Kind)
		End Sub

		Private Function EmitStatementAndCountInstructions(ByVal statement As BoundStatement) As Integer
			Dim instructionsEmitted As Integer = Me._builder.InstructionsEmitted
			Me.EmitStatement(statement)
			Return Me._builder.InstructionsEmitted - instructionsEmitted
		End Function

		Private Sub EmitStaticCast(ByVal toType As TypeSymbol, ByVal syntax As SyntaxNode)
			Dim localDefinition As Microsoft.CodeAnalysis.CodeGen.LocalDefinition = Me.AllocateTemp(toType, syntax)
			Me._builder.EmitLocalStore(localDefinition)
			Me._builder.EmitLocalLoad(localDefinition)
			Me.FreeTemp(localDefinition)
		End Sub

		Private Sub EmitStaticFieldAddress(ByVal field As FieldSymbol, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode)
			Me._builder.EmitOpCode(ILOpCode.Ldsflda)
			Me.EmitSymbolToken(field, syntaxNode)
		End Sub

		Private Sub EmitStaticFieldLoad(ByVal field As FieldSymbol, ByVal used As Boolean, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode)
			Me._builder.EmitOpCode(ILOpCode.Ldsfld)
			Me.EmitSymbolToken(field, syntaxNode)
			Me.EmitPopIfUnused(used)
		End Sub

		Private Sub EmitStore(ByVal expression As BoundExpression)
			Dim kind As BoundKind = expression.Kind
			If (kind <= BoundKind.ReferenceAssignment) Then
				If (kind > BoundKind.InstrumentationPayloadRoot) Then
					If (kind = BoundKind.ModuleVersionId) Then
						Me.EmitModuleVersionIdStore(DirectCast(expression, BoundModuleVersionId))
						Return
					End If
					If (kind = BoundKind.ReferenceAssignment) Then
						Me.EmitStoreIndirect(expression.Type, expression.Syntax)
						Return
					End If
				Else
					If (kind = BoundKind.ArrayAccess) Then
						Dim type As ArrayTypeSymbol = DirectCast(DirectCast(expression, BoundArrayAccess).Expression.Type, ArrayTypeSymbol)
						Me.EmitArrayElementStore(type, expression.Syntax)
						Return
					End If
					If (kind = BoundKind.InstrumentationPayloadRoot) Then
						Me.EmitInstrumentationPayloadRootStore(DirectCast(expression, BoundInstrumentationPayloadRoot))
						Return
					End If
				End If
			ElseIf (kind > BoundKind.FieldAccess) Then
				If (kind = BoundKind.Sequence) Then
					Dim boundSequence As Microsoft.CodeAnalysis.VisualBasic.BoundSequence = DirectCast(expression, Microsoft.CodeAnalysis.VisualBasic.BoundSequence)
					Me.EmitStore(boundSequence.ValueOpt)
					If (Not boundSequence.Locals.IsEmpty) Then
						Me._builder.CloseLocalScope()
						Dim enumerator As ImmutableArray(Of LocalSymbol).Enumerator = boundSequence.Locals.GetEnumerator()
						While enumerator.MoveNext()
							Me.FreeLocal(enumerator.Current)
						End While
						Return
					End If
				Else
					If (kind = BoundKind.MeReference) Then
						Me.EmitMeStore(DirectCast(expression, BoundMeReference))
						Return
					End If
					Select Case kind
						Case BoundKind.Local
							Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = DirectCast(expression, Microsoft.CodeAnalysis.VisualBasic.BoundLocal)
							If (boundLocal.LocalSymbol.IsByRef) Then
								Me.EmitStoreIndirect(boundLocal.LocalSymbol.Type, expression.Syntax)
								Return
							End If
							If (Me.IsStackLocal(boundLocal.LocalSymbol)) Then
								Exit Select
							End If
							Dim local As LocalDefinition = Me.GetLocal(boundLocal)
							Me._builder.EmitLocalStore(local)
							Return
						Case BoundKind.PseudoVariable
							Me.EmitStoreIndirect(expression.Type, expression.Syntax)
							Return
						Case BoundKind.Parameter
							Me.EmitParameterStore(DirectCast(expression, BoundParameter))
							Return
						Case Else
							Throw ExceptionUtilities.UnexpectedValue(expression.Kind)
					End Select
				End If
				Return
			Else
				If (kind = BoundKind.[Call]) Then
					Me.EmitStoreIndirect(expression.Type, expression.Syntax)
					Return
				End If
				If (kind = BoundKind.FieldAccess) Then
					Me.EmitFieldStore(DirectCast(expression, BoundFieldAccess))
					Return
				End If
			End If
			Throw ExceptionUtilities.UnexpectedValue(expression.Kind)
		End Sub

		Private Sub EmitStoreIndirect(ByVal type As TypeSymbol, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode)
			If (type.IsEnumType()) Then
				type = DirectCast(type, NamedTypeSymbol).EnumUnderlyingType
			End If
			Select Case type.PrimitiveTypeCode
				Case Microsoft.Cci.PrimitiveTypeCode.[Boolean]
				Case Microsoft.Cci.PrimitiveTypeCode.Int8
				Case Microsoft.Cci.PrimitiveTypeCode.UInt8
					Me._builder.EmitOpCode(ILOpCode.Stind_i1)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.[Char]
				Case Microsoft.Cci.PrimitiveTypeCode.Int16
				Case Microsoft.Cci.PrimitiveTypeCode.UInt16
					Me._builder.EmitOpCode(ILOpCode.Stind_i2)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.Float32
					Me._builder.EmitOpCode(ILOpCode.Stind_r4)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.Float64
					Me._builder.EmitOpCode(ILOpCode.Stind_r8)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.Int32
				Case Microsoft.Cci.PrimitiveTypeCode.UInt32
					Me._builder.EmitOpCode(ILOpCode.Stind_i4)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.Int64
				Case Microsoft.Cci.PrimitiveTypeCode.UInt64
					Me._builder.EmitOpCode(ILOpCode.Stind_i8)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.IntPtr
				Case Microsoft.Cci.PrimitiveTypeCode.Pointer
				Case Microsoft.Cci.PrimitiveTypeCode.UIntPtr
					Me._builder.EmitOpCode(ILOpCode.Stind_i)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.Reference
				Case Microsoft.Cci.PrimitiveTypeCode.[String]
					If (type.IsVerifierReference()) Then
						Me._builder.EmitOpCode(ILOpCode.Stind_ref)
						Return
					End If
					Me._builder.EmitOpCode(ILOpCode.Stobj)
					Me.EmitSymbolToken(type, syntaxNode)
					Return
				Case Else
					If (type.IsVerifierReference()) Then
						Me._builder.EmitOpCode(ILOpCode.Stind_ref)
						Return
					End If
					Me._builder.EmitOpCode(ILOpCode.Stobj)
					Me.EmitSymbolToken(type, syntaxNode)
					Return
			End Select
		End Sub

		Private Sub EmitStringCompareAndBranch(ByVal key As LocalOrParameter, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal stringConstant As ConstantValue, ByVal targetLabel As Object, ByVal stringCompareMethodRef As IReference)
			Me._builder.EmitLoad(key)
			Me._builder.EmitConstantValue(stringConstant)
			Me._builder.EmitConstantValue(ConstantValue.[False])
			Me._builder.EmitOpCode(ILOpCode.[Call], -2)
			Me._builder.EmitToken(stringCompareMethodRef, syntaxNode, Me._diagnostics, False)
			Me._builder.EmitBranch(ILOpCode.Brfalse, RuntimeHelpers.GetObjectValue(targetLabel), ILOpCode.Brtrue)
		End Sub

		Private Sub EmitStringSwitchJumpTable(ByVal caseLabels As KeyValuePair(Of ConstantValue, Object)(), ByVal fallThroughLabel As LabelSymbol, ByVal key As Microsoft.CodeAnalysis.CodeGen.LocalDefinition, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode)
			Dim localDefinition As Microsoft.CodeAnalysis.CodeGen.LocalDefinition = Nothing
			If (SwitchStringJumpTableEmitter.ShouldGenerateHashTableSwitch(Me._module, CInt(caseLabels.Length))) Then
				Dim method As IReference = Me._module.GetPrivateImplClass(syntaxNode, Me._diagnostics).GetMethod("ComputeStringHash")
				Me._builder.EmitLocalLoad(key)
				Me._builder.EmitOpCode(ILOpCode.[Call], 0)
				Me._builder.EmitToken(method, syntaxNode, Me._diagnostics, False)
				Dim internalSymbol As TypeSymbol = DirectCast(Me._module.GetSpecialType(SpecialType.System_UInt32, syntaxNode, Me._diagnostics).GetInternalSymbol(), TypeSymbol)
				localDefinition = Me.AllocateTemp(internalSymbol, syntaxNode)
				Me._builder.EmitLocalStore(localDefinition)
			End If
			Dim wellKnownType As NamedTypeSymbol = Me._module.Compilation.GetWellKnownType(Microsoft.CodeAnalysis.WellKnownType.Microsoft_VisualBasic_CompilerServices_EmbeddedOperators)
			Dim wellKnownTypeMember As MethodSymbol = DirectCast(Me._module.Compilation.GetWellKnownTypeMember(If(Not wellKnownType.IsErrorType() OrElse Not TypeOf wellKnownType Is MissingMetadataTypeSymbol, WellKnownMember.Microsoft_VisualBasic_CompilerServices_EmbeddedOperators__CompareStringStringStringBoolean, WellKnownMember.Microsoft_VisualBasic_CompilerServices_Operators__CompareStringStringStringBoolean)), MethodSymbol)
			Dim reference As IReference = Me._module.Translate(wellKnownTypeMember, syntaxNode, Me._diagnostics, False)
			Dim emitStringCompareAndBranch As SwitchStringJumpTableEmitter.EmitStringCompareAndBranch = Sub(keyArg As LocalOrParameter, stringConstant As ConstantValue, targetLabel As Object) Me.EmitStringCompareAndBranch(keyArg, syntaxNode, stringConstant, RuntimeHelpers.GetObjectValue(targetLabel), reference)
			Me._builder.EmitStringSwitchJumpTable(caseLabels, fallThroughLabel, key, localDefinition, emitStringCompareAndBranch, New SwitchStringJumpTableEmitter.GetStringHashCode(AddressOf SynthesizedStringSwitchHashMethod.ComputeStringHash))
			If (localDefinition IsNot Nothing) Then
				Me.FreeTemp(localDefinition)
			End If
		End Sub

		Private Sub EmitStructConstructorCallOnTarget(ByVal constructor As MethodSymbol, ByVal arguments As ImmutableArray(Of BoundExpression), ByVal target As BoundExpression, ByVal syntaxNode As VisualBasicSyntaxNode)
			If (target.Kind = BoundKind.Local AndAlso Me.IsStackLocal(DirectCast(target, BoundLocal).LocalSymbol)) Then
				Me.EmitNewObj(constructor, arguments, True, syntaxNode)
				Return
			End If
			Me.EmitAddress(target, CodeGenerator.AddressKind.Immutable)
			Dim parameterCount As Integer = -constructor.ParameterCount - 1
			Me.EmitArguments(arguments, constructor.Parameters)
			Me._builder.EmitOpCode(ILOpCode.[Call], parameterCount)
			Me.EmitSymbolToken(constructor, syntaxNode, False)
		End Sub

		Private Sub EmitSwitch(ByVal jumps As ImmutableArray(Of BoundGotoStatement))
			Dim label(jumps.Length - 1 + 1 - 1) As [Object]
			Dim length As Integer = jumps.Length - 1
			Dim num As Integer = 0
			Do
				label(num) = jumps(num).Label
				num = num + 1
			Loop While num <= length
			Me._builder.EmitSwitch(label)
		End Sub

		Private Sub EmitSwitchTableHeader(ByVal selectExpression As BoundExpression, ByVal caseLabels As KeyValuePair(Of ConstantValue, Object)(), ByVal fallThroughLabel As LabelSymbol)
			If (Not caseLabels.Any()) Then
				Me._builder.EmitBranch(ILOpCode.Br, fallThroughLabel, ILOpCode.Nop)
				Return
			End If
			Dim type As TypeSymbol = selectExpression.Type
			Dim localDefinition As Microsoft.CodeAnalysis.CodeGen.LocalDefinition = Nothing
			If (type.SpecialType = SpecialType.System_String) Then
				If (selectExpression.Kind <> BoundKind.Local OrElse DirectCast(selectExpression, BoundLocal).LocalSymbol.IsByRef) Then
					Me.EmitExpression(selectExpression, True)
					localDefinition = Me.AllocateTemp(type, selectExpression.Syntax)
					Me._builder.EmitLocalStore(localDefinition)
					Me.EmitStringSwitchJumpTable(caseLabels, fallThroughLabel, localDefinition, selectExpression.Syntax)
				Else
					Me.EmitStringSwitchJumpTable(caseLabels, fallThroughLabel, Me.GetLocal(DirectCast(selectExpression, BoundLocal)), selectExpression.Syntax)
				End If
			ElseIf (selectExpression.Kind = BoundKind.Local AndAlso Not DirectCast(selectExpression, BoundLocal).LocalSymbol.IsByRef) Then
				Me._builder.EmitIntegerSwitchJumpTable(caseLabels, fallThroughLabel, Me.GetLocal(DirectCast(selectExpression, BoundLocal)), type.GetEnumUnderlyingTypeOrSelf().PrimitiveTypeCode)
			ElseIf (selectExpression.Kind <> BoundKind.Parameter OrElse DirectCast(selectExpression, BoundParameter).ParameterSymbol.IsByRef) Then
				Me.EmitExpression(selectExpression, True)
				localDefinition = Me.AllocateTemp(type, selectExpression.Syntax)
				Me._builder.EmitLocalStore(localDefinition)
				Me._builder.EmitIntegerSwitchJumpTable(caseLabels, fallThroughLabel, localDefinition, type.GetEnumUnderlyingTypeOrSelf().PrimitiveTypeCode)
			Else
				Me._builder.EmitIntegerSwitchJumpTable(caseLabels, fallThroughLabel, Me.ParameterSlot(DirectCast(selectExpression, BoundParameter)), type.GetEnumUnderlyingTypeOrSelf().PrimitiveTypeCode)
			End If
			If (localDefinition IsNot Nothing) Then
				Me.FreeTemp(localDefinition)
			End If
		End Sub

		Private Sub EmitSymbolToken(ByVal symbol As FieldSymbol, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode)
			Me._builder.EmitToken(Me._module.Translate(symbol, syntaxNode, Me._diagnostics, False), syntaxNode, Me._diagnostics, False)
		End Sub

		Private Sub EmitSymbolToken(ByVal symbol As MethodSymbol, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, Optional ByVal encodeAsRawDefinitionToken As Boolean = False)
			Me._builder.EmitToken(Me._module.Translate(symbol, syntaxNode, Me._diagnostics, encodeAsRawDefinitionToken), syntaxNode, Me._diagnostics, encodeAsRawDefinitionToken)
		End Sub

		Private Sub EmitSymbolToken(ByVal symbol As TypeSymbol, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode)
			Me._builder.EmitToken(Me._module.Translate(symbol, syntaxNode, Me._diagnostics), syntaxNode, Me._diagnostics, False)
		End Sub

		Private Sub EmitTernaryConditionalExpression(ByVal expr As BoundTernaryConditionalExpression, ByVal used As Boolean)
			Dim objectValue As Object = RuntimeHelpers.GetObjectValue(New [Object]())
			Dim obj As Object = RuntimeHelpers.GetObjectValue(New [Object]())
			Me.EmitCondBranch(expr.Condition, objectValue, True)
			Me.EmitExpression(expr.WhenFalse, used)
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.StackMergeType(expr.WhenFalse)
			If (used AndAlso CodeGenerator.IsVarianceCast(expr.Type, type)) Then
				Me.EmitStaticCast(expr.Type, expr.Syntax)
				type = expr.Type
			End If
			Me._builder.EmitBranch(ILOpCode.Br, RuntimeHelpers.GetObjectValue(obj), ILOpCode.Nop)
			If (used) Then
				Me._builder.AdjustStack(-1)
			End If
			Me._builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue))
			Me.EmitExpression(expr.WhenTrue, used)
			If (used) Then
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.StackMergeType(expr.WhenTrue)
				If (CodeGenerator.IsVarianceCast(expr.Type, typeSymbol)) Then
					Me.EmitStaticCast(expr.Type, expr.Syntax)
					typeSymbol = expr.Type
				ElseIf (expr.Type.IsInterfaceType() AndAlso Not Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(expr.Type, type, TypeCompareKind.ConsiderEverything) AndAlso Not Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(expr.Type, typeSymbol, TypeCompareKind.ConsiderEverything)) Then
					Me.EmitStaticCast(expr.Type, expr.Syntax)
				End If
			End If
			Me._builder.MarkLabel(RuntimeHelpers.GetObjectValue(obj))
		End Sub

		Private Sub EmitThrowStatement(ByVal boundThrowStatement As Microsoft.CodeAnalysis.VisualBasic.BoundThrowStatement)
			Dim expressionOpt As BoundExpression = boundThrowStatement.ExpressionOpt
			If (expressionOpt IsNot Nothing) Then
				Me.EmitExpression(expressionOpt, True)
				Dim type As TypeSymbol = expressionOpt.Type
				If (type IsNot Nothing AndAlso type.TypeKind = Microsoft.CodeAnalysis.TypeKind.TypeParameter) Then
					Me.EmitBox(type, expressionOpt.Syntax)
				End If
			End If
			Me._builder.EmitThrow(expressionOpt Is Nothing)
		End Sub

		Private Sub EmitTryCastExpression(ByVal conversion As BoundTryCast, ByVal used As Boolean)
			If (Not used AndAlso Not Me.ConversionHasSideEffects(conversion)) Then
				Me.EmitExpression(conversion.Operand, False)
				Return
			End If
			If (Not conversion.Operand.IsNothingLiteral()) Then
				Me.EmitExpression(conversion.Operand, True)
				If (Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(conversion.ConversionKind)) Then
					Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = conversion.Type
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = conversion.Operand.Type
					If (typeSymbol.IsReferenceType OrElse typeSymbol.IsTypeParameter() OrElse type.IsTypeParameter()) Then
						If (Not typeSymbol.IsVerifierReference()) Then
							Me.EmitBox(typeSymbol, conversion.Operand.Syntax)
						End If
						Me._builder.EmitOpCode(ILOpCode.Isinst)
						Me.EmitSymbolToken(type, conversion.Syntax)
						If (Not type.IsVerifierReference()) Then
							Me._builder.EmitOpCode(ILOpCode.Unbox_any)
							Me.EmitSymbolToken(type, conversion.Syntax)
						End If
					Else
						Me.EmitBox(typeSymbol, conversion.Operand.Syntax)
					End If
				End If
			ElseIf (Not conversion.Type.IsTypeParameter()) Then
				Me.EmitExpression(conversion.Operand, True)
			Else
				Me.EmitLoadDefaultValueOfTypeParameter(conversion.Type, True, conversion.Syntax)
			End If
			Me.EmitPopIfUnused(used)
		End Sub

		Private Sub EmitTryStatement(ByVal statement As BoundTryStatement, Optional ByVal emitCatchesOnly As Boolean = False)
			Dim flag As Boolean
			flag = If(emitCatchesOnly OrElse statement.CatchBlocks.Length <= 0, False, statement.FinallyBlockOpt IsNot Nothing)
			Me._builder.OpenLocalScope(ScopeType.TryCatchFinally, Nothing)
			Me._builder.OpenLocalScope(ScopeType.[Try], Nothing)
			Me._tryNestingLevel = Me._tryNestingLevel + 1
			If (Not flag) Then
				Me.EmitBlock(statement.TryBlock)
			Else
				Me.EmitTryStatement(statement, True)
			End If
			Me._tryNestingLevel = Me._tryNestingLevel - 1
			Me._builder.CloseLocalScope()
			If (Not flag) Then
				Dim enumerator As ImmutableArray(Of BoundCatchBlock).Enumerator = statement.CatchBlocks.GetEnumerator()
				While enumerator.MoveNext()
					Me.EmitCatchBlock(enumerator.Current)
				End While
			End If
			If (Not emitCatchesOnly AndAlso statement.FinallyBlockOpt IsNot Nothing) Then
				Me._builder.OpenLocalScope(ScopeType.[Finally], Nothing)
				Me.EmitBlock(statement.FinallyBlockOpt)
				Me._builder.CloseLocalScope()
			End If
			Me._builder.CloseLocalScope()
			If (Not emitCatchesOnly AndAlso statement.ExitLabelOpt IsNot Nothing) Then
				Me._builder.MarkLabel(statement.ExitLabelOpt)
			End If
		End Sub

		Private Sub EmitTypeOfExpression(ByVal expression As BoundTypeOf, ByVal used As Boolean, Optional ByVal optimize As Boolean = False)
			Dim operand As BoundExpression = expression.Operand
			Me.EmitExpression(operand, True)
			If (used) Then
				Dim type As TypeSymbol = operand.Type
				Dim targetType As TypeSymbol = expression.TargetType
				Me._builder.EmitOpCode(ILOpCode.Isinst)
				Me.EmitSymbolToken(targetType, expression.Syntax)
				If (Not optimize) Then
					Me._builder.EmitOpCode(ILOpCode.Ldnull)
					If (Not expression.IsTypeOfIsNotExpression) Then
						Me._builder.EmitOpCode(ILOpCode.Cgt_un)
					Else
						Me._builder.EmitOpCode(ILOpCode.Ceq)
					End If
				End If
			End If
			Me.EmitPopIfUnused(used)
		End Sub

		Private Sub EmitUnaryOperatorExpression(ByVal expression As BoundUnaryOperator, ByVal used As Boolean)
			Dim flag As Boolean
			If (Not used AndAlso Not CodeGenerator.OperatorHasSideEffects(expression)) Then
				Me.EmitExpression(expression.Operand, False)
				Return
			End If
			Select Case expression.OperatorKind
				Case UnaryOperatorKind.Plus
					Me.EmitExpression(expression.Operand, True)
					Exit Select
				Case UnaryOperatorKind.Minus
					Dim primitiveTypeCode As Microsoft.Cci.PrimitiveTypeCode = expression.Type.PrimitiveTypeCode
					If (Not expression.Checked) Then
						flag = False
					Else
						flag = If(primitiveTypeCode = Microsoft.Cci.PrimitiveTypeCode.Int32, True, primitiveTypeCode = Microsoft.Cci.PrimitiveTypeCode.Int64)
					End If
					If (flag) Then
						Me._builder.EmitOpCode(ILOpCode.Ldc_i4_0)
						If (primitiveTypeCode = Microsoft.Cci.PrimitiveTypeCode.Int64) Then
							Me._builder.EmitOpCode(ILOpCode.Conv_i8)
						End If
					End If
					Me.EmitExpression(expression.Operand, True)
					If (Not flag) Then
						Me._builder.EmitOpCode(ILOpCode.Neg)
					Else
						Me._builder.EmitOpCode(ILOpCode.Sub_ovf)
					End If
					Me.DowncastResultOfArithmeticOperation(primitiveTypeCode, expression.Checked)
					Exit Select
				Case UnaryOperatorKind.[Not]
					If (Not expression.Type.IsBooleanType()) Then
						Me.EmitExpression(expression.Operand, True)
						Me._builder.EmitOpCode(ILOpCode.[Not])
						Dim primitiveTypeCode1 As Microsoft.Cci.PrimitiveTypeCode = expression.Type.PrimitiveTypeCode
						If (primitiveTypeCode1 <> Microsoft.Cci.PrimitiveTypeCode.UInt8 AndAlso primitiveTypeCode1 <> Microsoft.Cci.PrimitiveTypeCode.UInt16) Then
							Exit Select
						End If
						Me._builder.EmitNumericConversion(Microsoft.Cci.PrimitiveTypeCode.UInt32, primitiveTypeCode1, False)
						Exit Select
					Else
						Me.EmitCondExpr(expression.Operand, False)
						Exit Select
					End If
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(expression.OperatorKind)
			End Select
			Me.EmitPopIfUnused(used)
		End Sub

		Private Sub EmitUnboxAny(ByVal type As TypeSymbol, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode)
			Me._builder.EmitOpCode(ILOpCode.Unbox_any)
			Me.EmitSymbolToken(type, syntaxNode)
		End Sub

		Private Sub EmitUnstructuredExceptionOnErrorSwitch(ByVal node As BoundUnstructuredExceptionOnErrorSwitch)
			Me.EmitExpression(node.Value, True)
			Me.EmitSwitch(node.Jumps)
		End Sub

		Private Sub EmitUnstructuredExceptionResumeSwitch(ByVal node As BoundUnstructuredExceptionResumeSwitch)
			Me.EmitLabelStatement(node.ResumeLabel)
			Me.EmitExpression(node.ResumeTargetTemporary, True)
			Dim objectValue As Object = RuntimeHelpers.GetObjectValue(New [Object]())
			Me._builder.EmitBranch(ILOpCode.Br_s, RuntimeHelpers.GetObjectValue(objectValue), ILOpCode.Nop)
			Me._builder.AdjustStack(-1)
			Me.EmitLabelStatement(node.ResumeNextLabel)
			Me.EmitExpression(node.ResumeTargetTemporary, True)
			Me._builder.EmitIntConstant(1)
			Me._builder.EmitOpCode(ILOpCode.Add)
			Me._builder.MarkLabel(RuntimeHelpers.GetObjectValue(objectValue))
			Me._builder.EmitIntConstant(0)
			Me._builder.EmitLocalStore(Me.GetLocal(node.ResumeTargetTemporary))
			Me.EmitSwitch(node.Jumps)
		End Sub

		Private Sub EmitVectorElementStore(ByVal arrayType As ArrayTypeSymbol, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode)
			Dim elementType As TypeSymbol = arrayType.ElementType
			If (elementType.IsEnumType()) Then
				elementType = DirectCast(elementType, NamedTypeSymbol).EnumUnderlyingType
			End If
			Select Case elementType.PrimitiveTypeCode
				Case Microsoft.Cci.PrimitiveTypeCode.[Boolean]
				Case Microsoft.Cci.PrimitiveTypeCode.Int8
				Case Microsoft.Cci.PrimitiveTypeCode.UInt8
					Me._builder.EmitOpCode(ILOpCode.Stelem_i1)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.[Char]
				Case Microsoft.Cci.PrimitiveTypeCode.Int16
				Case Microsoft.Cci.PrimitiveTypeCode.UInt16
					Me._builder.EmitOpCode(ILOpCode.Stelem_i2)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.Float32
					Me._builder.EmitOpCode(ILOpCode.Stelem_r4)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.Float64
					Me._builder.EmitOpCode(ILOpCode.Stelem_r8)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.Int32
				Case Microsoft.Cci.PrimitiveTypeCode.UInt32
					Me._builder.EmitOpCode(ILOpCode.Stelem_i4)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.Int64
				Case Microsoft.Cci.PrimitiveTypeCode.UInt64
					Me._builder.EmitOpCode(ILOpCode.Stelem_i8)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.IntPtr
				Case Microsoft.Cci.PrimitiveTypeCode.Pointer
				Case Microsoft.Cci.PrimitiveTypeCode.UIntPtr
					Me._builder.EmitOpCode(ILOpCode.Stelem_i)
					Return
				Case Microsoft.Cci.PrimitiveTypeCode.Reference
				Case Microsoft.Cci.PrimitiveTypeCode.[String]
					If (elementType.IsVerifierReference()) Then
						Me._builder.EmitOpCode(ILOpCode.Stelem_ref)
						Return
					End If
					Me._builder.EmitOpCode(ILOpCode.Stelem)
					Me.EmitSymbolToken(elementType, syntaxNode)
					Return
				Case Else
					If (elementType.IsVerifierReference()) Then
						Me._builder.EmitOpCode(ILOpCode.Stelem_ref)
						Return
					End If
					Me._builder.EmitOpCode(ILOpCode.Stelem)
					Me.EmitSymbolToken(elementType, syntaxNode)
					Return
			End Select
		End Sub

		Friend Shared Function FieldLoadMustUseRef(ByVal expr As BoundExpression) As Boolean
			Dim flag As Boolean
			flag = If(Not expr.Type.IsEnumType(), expr.Type.IsTypeParameter(), True)
			Return flag
		End Function

		Private Function FieldLoadPrefersRef(ByVal receiver As BoundExpression) As Boolean
			Dim isByRef As Boolean
			If (Not receiver.Type.IsVerifierValue()) Then
				isByRef = True
			ElseIf (receiver.Kind = BoundKind.[DirectCast] AndAlso Me.IsUnboxingDirectCast(DirectCast(receiver, BoundDirectCast))) Then
				isByRef = True
			ElseIf (Me.HasHome(receiver)) Then
				Dim kind As BoundKind = receiver.Kind
				If (kind <= BoundKind.Sequence) Then
					If (kind = BoundKind.FieldAccess) Then
						Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = DirectCast(receiver, Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess)
						isByRef = If(boundFieldAccess.FieldSymbol.IsShared, True, Me.FieldLoadPrefersRef(boundFieldAccess.ReceiverOpt))
						Return isByRef
					Else
						If (kind <> BoundKind.Sequence) Then
							GoTo Label1
						End If
						isByRef = Me.FieldLoadPrefersRef(DirectCast(receiver, BoundSequence).ValueOpt)
						Return isByRef
					End If
				ElseIf (kind = BoundKind.Local) Then
					isByRef = DirectCast(receiver, BoundLocal).LocalSymbol.IsByRef
					Return isByRef
				Else
					If (kind <> BoundKind.Parameter) Then
						GoTo Label1
					End If
					isByRef = DirectCast(receiver, BoundParameter).ParameterSymbol.IsByRef
					Return isByRef
				End If
			Label1:
				isByRef = True
			Else
				isByRef = False
			End If
			Return isByRef
		End Function

		Private Sub FreeLocal(ByVal local As LocalSymbol)
			If (local.Name Is Nothing AndAlso Me.IsSlotReusable(local) AndAlso Not Me.IsStackLocal(local)) Then
				Me._builder.LocalSlotManager.FreeLocal(local)
			End If
		End Sub

		Private Sub FreeOptTemp(ByVal temp As LocalDefinition)
			If (temp IsNot Nothing) Then
				Me.FreeTemp(temp)
			End If
		End Sub

		Private Sub FreeTemp(ByVal temp As LocalDefinition)
			Me._builder.LocalSlotManager.FreeSlot(temp)
		End Sub

		Public Sub Generate()
			Me.GenerateImpl()
		End Sub

		Public Sub Generate(<Out> ByRef asyncCatchHandlerOffset As Integer, <Out> ByRef asyncYieldPoints As ImmutableArray(Of Integer), <Out> ByRef asyncResumePoints As ImmutableArray(Of Integer))
			Me.GenerateImpl()
			asyncCatchHandlerOffset = Me._builder.GetILOffsetFromMarker(Me._asyncCatchHandlerOffset)
			Dim nums As ArrayBuilder(Of Integer) = Me._asyncYieldPoints
			Dim nums1 As ArrayBuilder(Of Integer) = Me._asyncResumePoints
			If (nums Is Nothing) Then
				asyncYieldPoints = ImmutableArray(Of Integer).Empty
				asyncResumePoints = ImmutableArray(Of Integer).Empty
				Return
			End If
			Dim instance As ArrayBuilder(Of Integer) = ArrayBuilder(Of Integer).GetInstance()
			Dim instance1 As ArrayBuilder(Of Integer) = ArrayBuilder(Of Integer).GetInstance()
			Dim count As Integer = nums.Count - 1
			Dim num As Integer = 0
			Do
				Dim lOffsetFromMarker As Integer = Me._builder.GetILOffsetFromMarker(nums(num))
				Dim lOffsetFromMarker1 As Integer = Me._builder.GetILOffsetFromMarker(nums1(num))
				If (lOffsetFromMarker > 0) Then
					instance.Add(lOffsetFromMarker)
					instance1.Add(lOffsetFromMarker1)
				End If
				num = num + 1
			Loop While num <= count
			asyncYieldPoints = instance.ToImmutableAndFree()
			asyncResumePoints = instance1.ToImmutableAndFree()
			nums.Free()
			nums1.Free()
		End Sub

		Private Sub GenerateImpl()
			Me.SetInitialDebugDocument()
			If (Me._emitPdbSequencePoints AndAlso Me._method.IsImplicitlyDeclared) Then
				Me._builder.DefineInitialHiddenSequencePoint()
			End If
			Try
				Me.EmitStatement(Me._block)
				If (Me._unhandledReturn) Then
					Me.HandleReturn()
				End If
				If (Not Me._diagnostics.HasAnyErrors()) Then
					Me._builder.Realize()
				End If
			Catch emitCancelledException As CodeGenerator.EmitCancelledException
				ProjectData.SetProjectError(emitCancelledException)
				ProjectData.ClearProjectError()
			End Try
			Me._synthesizedLocalOrdinals.Free()
		End Sub

		Private Function GetCaseLabelsForEmitSwitchHeader(ByVal caseBlocks As ImmutableArray(Of BoundCaseBlock), ByVal caseBlockLabels As ImmutableArray(Of GeneratedLabelSymbol), ByRef fallThroughLabel As LabelSymbol) As KeyValuePair(Of ConstantValue, Object)()
			Dim constantValueOpt As ConstantValue
			Dim instance As ArrayBuilder(Of KeyValuePair(Of ConstantValue, Object)) = ArrayBuilder(Of KeyValuePair(Of ConstantValue, Object)).GetInstance()
			Dim constantValues As HashSet(Of ConstantValue) = New HashSet(Of ConstantValue)(New SwitchConstantValueHelper.SwitchLabelsComparer())
			Dim num As Integer = 0
			Dim enumerator As ImmutableArray(Of BoundCaseBlock).Enumerator = caseBlocks.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As BoundCaseBlock = enumerator.Current
				Dim item As GeneratedLabelSymbol = caseBlockLabels(num)
				Dim caseClauses As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundCaseClause) = current.CaseStatement.CaseClauses
				If (Not System.Linq.ImmutableArrayExtensions.Any(Of Microsoft.CodeAnalysis.VisualBasic.BoundCaseClause)(caseClauses)) Then
					fallThroughLabel = item
				Else
					Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundCaseClause).Enumerator = caseClauses.GetEnumerator()
					While enumerator1.MoveNext()
						Dim boundCaseClause As Microsoft.CodeAnalysis.VisualBasic.BoundCaseClause = enumerator1.Current
						Select Case boundCaseClause.Kind
							Case BoundKind.SimpleCaseClause
								constantValueOpt = DirectCast(boundCaseClause, BoundSimpleCaseClause).ValueOpt.ConstantValueOpt
								Exit Select
							Case BoundKind.RangeCaseClause
								Throw ExceptionUtilities.UnexpectedValue(boundCaseClause.Kind)
							Case BoundKind.RelationalCaseClause
								constantValueOpt = DirectCast(boundCaseClause, BoundRelationalCaseClause).ValueOpt.ConstantValueOpt
								Exit Select
							Case Else
								Throw ExceptionUtilities.UnexpectedValue(boundCaseClause.Kind)
						End Select
						If (constantValues.Contains(constantValueOpt)) Then
							Continue While
						End If
						instance.Add(New KeyValuePair(Of ConstantValue, Object)(constantValueOpt, item))
						constantValues.Add(constantValueOpt)
					End While
				End If
				num = num + 1
			End While
			Return instance.ToArrayAndFree()
		End Function

		Private Function GetLocal(ByVal localExpression As BoundLocal) As LocalDefinition
			Return Me.GetLocal(localExpression.LocalSymbol)
		End Function

		Private Function GetLocal(ByVal symbol As LocalSymbol) As LocalDefinition
			Return Me._builder.LocalSlotManager.GetLocal(symbol)
		End Function

		Private Function GetLocalDebugName(ByVal local As LocalSymbol, <Out> ByRef localId As LocalDebugId) As String
			Dim name As String
			localId = LocalDebugId.None
			If (local.IsImportedFromMetadata) Then
				name = local.Name
			ElseIf (local.DeclarationKind <> LocalDeclarationKind.FunctionValue OrElse Not TypeOf Me._method Is SynthesizedStateMachineMethod) Then
				Dim synthesizedKind As SynthesizedLocalKind = local.SynthesizedKind
				If (synthesizedKind.IsLongLived()) Then
					If (Me._ilEmitStyle = ILEmitStyle.Debug) Then
						Dim declaratorSyntax As SyntaxNode = local.GetDeclaratorSyntax()
						Dim num As Integer = Me._method.CalculateLocalSyntaxOffset(declaratorSyntax.SpanStart, declaratorSyntax.SyntaxTree)
						Dim num1 As Integer = Me._synthesizedLocalOrdinals.AssignLocalOrdinal(synthesizedKind, num)
						localId = New LocalDebugId(num, num1)
					End If
					name = If(local.Name Is Nothing, GeneratedNames.MakeSynthesizedLocalName(synthesizedKind, Me._uniqueNameId), local.Name)
				Else
					name = Nothing
				End If
			Else
				name = Nothing
			End If
			Return name
		End Function

		Private Function GetParameterlessValueTypeConstructor(ByVal typeTo As NamedTypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).Enumerator = typeTo.InstanceConstructors.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = enumerator.Current
				If (current.ParameterCount <> 0) Then
					Continue While
				End If
				Dim containingType As NamedTypeSymbol = Me._method.ContainingType
				Dim discarded As CompoundUseSiteInfo(Of AssemblySymbol) = CompoundUseSiteInfo(Of AssemblySymbol).Discarded
				Dim basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved = New Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved()
				If (Not AccessCheck.IsSymbolAccessible(current, containingType, typeTo, discarded, basesBeingResolved)) Then
					methodSymbol = Nothing
				Else
					methodSymbol = current
				End If
				Return methodSymbol
			End While
			Throw ExceptionUtilities.Unreachable
		End Function

		Private Function GetRawData(ByVal initializers As ImmutableArray(Of BoundExpression)) As ImmutableArray(Of Byte)
			Dim instance As Microsoft.Cci.PooledBlobBuilder = Microsoft.Cci.PooledBlobBuilder.GetInstance(initializers.Length * 4)
			Me.SerializeArrayRecursive(instance, initializers)
			Dim immutableArray As ImmutableArray(Of Byte) = instance.ToImmutableArray()
			instance.Free()
			Return immutableArray
		End Function

		Public Shared Function GetShiftSizeMask(ByVal leftOperandType As TypeSymbol) As Integer
			Return leftOperandType.GetEnumUnderlyingTypeOrSelf().SpecialType.GetShiftSizeMask()
		End Function

		Private Sub HandleReturn()
			Me._builder.MarkLabel(RuntimeHelpers.GetObjectValue(CodeGenerator.s_returnLabel))
			Me._builder.EmitRet(True)
			Me._unhandledReturn = False
		End Sub

		Private Function HasHome(ByVal expression As BoundExpression) As Boolean
			Dim isReference As Boolean
			Dim kind As BoundKind = expression.Kind
			If (kind <= BoundKind.ReferenceAssignment) Then
				If (kind <> BoundKind.Dup) Then
					If (kind = BoundKind.ArrayAccess OrElse kind = BoundKind.ReferenceAssignment) Then
						isReference = True
						Return isReference
					End If
					isReference = False
					Return isReference
				End If
				isReference = DirectCast(expression, BoundDup).IsReference
			ElseIf (kind <= BoundKind.FieldAccess) Then
				If (kind = BoundKind.[Call]) Then
					isReference = DirectCast(expression, BoundCall).Method.ReturnsByRef
				Else
					If (kind <> BoundKind.FieldAccess) Then
						isReference = False
						Return isReference
					End If
					isReference = Me.HasHome(DirectCast(expression, BoundFieldAccess))
				End If
			ElseIf (kind = BoundKind.Sequence) Then
				Dim valueOpt As BoundExpression = DirectCast(expression, BoundSequence).ValueOpt
				isReference = If(valueOpt Is Nothing, False, Me.HasHome(valueOpt))
			Else
				Select Case kind
					Case BoundKind.MeReference
					Case BoundKind.MyBaseReference
					Case BoundKind.Parameter
						isReference = True
						Return isReference
					Case BoundKind.ValueTypeMeReference
						isReference = True
						Exit Select
					Case BoundKind.MyClassReference
					Case BoundKind.PreviousSubmissionReference
					Case BoundKind.HostObjectMemberReference
					Case BoundKind.PseudoVariable
						isReference = False
						Return isReference
					Case BoundKind.Local
						Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = DirectCast(expression, BoundLocal).LocalSymbol
						isReference = If(Not Me.IsStackLocal(localSymbol), True, localSymbol.IsByRef)
						Exit Select
					Case Else
						isReference = False
						Return isReference
				End Select
			End If
			Return isReference
		End Function

		Private Function HasHome(ByVal fieldAccess As BoundFieldAccess) As Boolean
			Dim methodKind As Boolean
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = fieldAccess.FieldSymbol
			If (fieldSymbol.IsConst AndAlso Not fieldSymbol.IsConstButNotMetadataConstant) Then
				methodKind = False
			ElseIf (Not fieldSymbol.IsReadOnly) Then
				methodKind = True
			ElseIf (Not TypeSymbol.Equals(fieldSymbol.ContainingType, Me._method.ContainingType, TypeCompareKind.ConsiderEverything)) Then
				methodKind = False
			ElseIf (Not fieldSymbol.IsShared) Then
				methodKind = If(Me._method.MethodKind <> Microsoft.CodeAnalysis.MethodKind.Constructor, False, fieldAccess.ReceiverOpt.Kind = BoundKind.MeReference)
			Else
				methodKind = Me._method.MethodKind = Microsoft.CodeAnalysis.MethodKind.StaticConstructor
			End If
			Return methodKind
		End Function

		Private Sub InitializerCountRecursive(ByVal inits As ImmutableArray(Of BoundExpression), ByRef initCount As Integer, ByRef constInits As Integer)
			If (inits.Length <> 0) Then
				Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = inits.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As BoundExpression = enumerator.Current
					Dim boundArrayInitialization As Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization = TryCast(current, Microsoft.CodeAnalysis.VisualBasic.BoundArrayInitialization)
					If (boundArrayInitialization Is Nothing) Then
						If (current.IsDefaultValue()) Then
							Continue While
						End If
						initCount = initCount + 1
						If (current.ConstantValueOpt Is Nothing) Then
							Continue While
						End If
						constInits = constInits + 1
					Else
						Me.InitializerCountRecursive(boundArrayInitialization.Initializers, initCount, constInits)
					End If
				End While
			End If
		End Sub

		Private Sub InPlaceCtorCall(ByVal target As BoundExpression, ByVal objCreation As BoundObjectCreationExpression, ByVal used As Boolean)
			Me.EmitAddress(target, CodeGenerator.AddressKind.Writeable)
			Dim constructorOpt As MethodSymbol = objCreation.ConstructorOpt
			Me.EmitArguments(objCreation.Arguments, constructorOpt.Parameters)
			Dim parameterCount As Integer = constructorOpt.ParameterCount + 1
			Me._builder.EmitOpCode(ILOpCode.[Call], -parameterCount)
			Me.EmitSymbolToken(constructorOpt, objCreation.Syntax, False)
			If (used) Then
				Me.EmitExpression(target, used)
			End If
		End Sub

		Private Sub InPlaceInit(ByVal target As BoundExpression, ByVal used As Boolean)
			Me.EmitAddress(target, CodeGenerator.AddressKind.Writeable)
			Me._builder.EmitOpCode(ILOpCode.Initobj)
			Me.EmitSymbolToken(target.Type, target.Syntax)
			If (used) Then
				Me.EmitExpression(target, used)
			End If
		End Sub

		Private Function IsCondOperator(ByVal operationKind As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind) As Boolean
			Dim flag As Boolean
			Dim binaryOperatorKind As Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind = operationKind And Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.OpMask
			flag = If(CInt(binaryOperatorKind) - CInt(Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Equals) <= CInt(Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.NotEquals) OrElse binaryOperatorKind = Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[OrElse] OrElse CInt(binaryOperatorKind) - CInt(Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.[AndAlso]) <= CInt(Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Concatenate), True, False)
			Return flag
		End Function

		Private Function IsDebugPlus() As Boolean
			Return Me._module.Compilation.Options.DebugPlusMode
		End Function

		Private Shared Function IsIntegral(ByVal type As Microsoft.Cci.PrimitiveTypeCode) As Boolean
			Dim flag As Boolean = False
			Select Case type
				Case Microsoft.Cci.PrimitiveTypeCode.Int8
				Case Microsoft.Cci.PrimitiveTypeCode.Int16
				Case Microsoft.Cci.PrimitiveTypeCode.Int32
				Case Microsoft.Cci.PrimitiveTypeCode.Int64
				Case Microsoft.Cci.PrimitiveTypeCode.UInt8
				Case Microsoft.Cci.PrimitiveTypeCode.UInt16
				Case Microsoft.Cci.PrimitiveTypeCode.UInt32
				Case Microsoft.Cci.PrimitiveTypeCode.UInt64
					flag = True
					Return flag
				Case Microsoft.Cci.PrimitiveTypeCode.Float32
				Case Microsoft.Cci.PrimitiveTypeCode.Float64
				Case Microsoft.Cci.PrimitiveTypeCode.IntPtr
				Case Microsoft.Cci.PrimitiveTypeCode.Pointer
				Case Microsoft.Cci.PrimitiveTypeCode.Reference
				Case Microsoft.Cci.PrimitiveTypeCode.[String]
					Return flag
				Case Else
					Return flag
			End Select
		End Function

		Private Function IsMeReceiver(ByVal receiver As BoundExpression) As Boolean
			Dim flag As Boolean
			Dim kind As BoundKind = receiver.Kind
			If (kind = BoundKind.Sequence) Then
				flag = Me.IsMeReceiver(DirectCast(receiver, BoundSequence).ValueOpt)
			Else
				flag = If(kind = BoundKind.MeReference OrElse kind = BoundKind.MyClassReference, True, False)
			End If
			Return flag
		End Function

		Private Function IsMultidimensionalInitializer(ByVal inits As ImmutableArray(Of BoundExpression)) As Boolean
			If (inits.Length = 0) Then
				Return False
			End If
			Return inits(0).Kind = BoundKind.ArrayInitialization
		End Function

		Private Shared Function IsSimpleType(ByVal type As Microsoft.Cci.PrimitiveTypeCode) As Boolean
			Dim flag As Boolean = False
			Select Case type
				Case Microsoft.Cci.PrimitiveTypeCode.[Boolean]
				Case Microsoft.Cci.PrimitiveTypeCode.Int8
				Case Microsoft.Cci.PrimitiveTypeCode.Float32
				Case Microsoft.Cci.PrimitiveTypeCode.Float64
				Case Microsoft.Cci.PrimitiveTypeCode.Int16
				Case Microsoft.Cci.PrimitiveTypeCode.Int32
				Case Microsoft.Cci.PrimitiveTypeCode.Int64
				Case Microsoft.Cci.PrimitiveTypeCode.UInt8
				Case Microsoft.Cci.PrimitiveTypeCode.UInt16
				Case Microsoft.Cci.PrimitiveTypeCode.UInt32
				Case Microsoft.Cci.PrimitiveTypeCode.UInt64
					flag = True
					Return flag
				Case Microsoft.Cci.PrimitiveTypeCode.[Char]
				Case Microsoft.Cci.PrimitiveTypeCode.IntPtr
				Case Microsoft.Cci.PrimitiveTypeCode.Pointer
				Case Microsoft.Cci.PrimitiveTypeCode.Reference
				Case Microsoft.Cci.PrimitiveTypeCode.[String]
					Return flag
				Case Else
					Return flag
			End Select
		End Function

		Private Function IsSlotReusable(ByVal local As LocalSymbol) As Boolean
			Return local.SynthesizedKind.IsSlotReusable(Me._ilEmitStyle <> ILEmitStyle.Release)
		End Function

		Private Function IsStackLocal(ByVal local As LocalSymbol) As Boolean
			If (Me._stackLocals Is Nothing) Then
				Return False
			End If
			Return Me._stackLocals.Contains(local)
		End Function

		Private Function IsUnboxingDirectCast(ByVal conversion As BoundDirectCast) As Boolean
			Dim type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = conversion.Type
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = conversion.Operand.Type
			If (conversion.Operand.IsNothingLiteral() OrElse Microsoft.CodeAnalysis.VisualBasic.Conversions.IsIdentityConversion(conversion.ConversionKind) OrElse typeSymbol.GetEnumUnderlyingTypeOrSelf().IsSameTypeIgnoringAll(type.GetEnumUnderlyingTypeOrSelf()) OrElse typeSymbol.IsTypeParameter() OrElse typeSymbol.IsValueType) Then
				Return False
			End If
			Return Not type.IsReferenceType
		End Function

		Private Shared Function IsVarianceCast(ByVal toType As TypeSymbol, ByVal fromType As TypeSymbol) As Boolean
			Dim flag As Boolean
			Dim flag1 As Boolean
			If (TypeSymbol.Equals(toType, fromType, TypeCompareKind.ConsiderEverything)) Then
				flag = False
			ElseIf (fromType Is Nothing) Then
				flag = True
			ElseIf (Not toType.IsArrayType()) Then
				If (Not toType.IsDelegateType() OrElse TypeSymbol.Equals(toType, fromType, TypeCompareKind.ConsiderEverything)) Then
					flag1 = If(Not toType.IsInterfaceType() OrElse Not fromType.IsInterfaceType(), False, Not fromType.InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.ContainsKey(DirectCast(toType, NamedTypeSymbol)))
				Else
					flag1 = True
				End If
				flag = flag1
			Else
				flag = CodeGenerator.IsVarianceCast(DirectCast(toType, ArrayTypeSymbol).ElementType, DirectCast(fromType, ArrayTypeSymbol).ElementType)
			End If
			Return flag
		End Function

		Private Function MayUseCallForStructMethod(ByVal method As MethodSymbol) As Boolean
			Dim flag As Boolean
			If (method.IsMetadataVirtual()) Then
				Dim overriddenMethod As MethodSymbol = method.OverriddenMethod
				If (overriddenMethod Is Nothing OrElse overriddenMethod.IsMustOverride) Then
					flag = True
				Else
					Dim containingType As NamedTypeSymbol = method.ContainingType
					flag = If(containingType.IsIntrinsicType(), True, containingType.IsRestrictedType())
				End If
			Else
				flag = True
			End If
			Return flag
		End Function

		Private Shared Function OperatorHasSideEffects(ByVal expression As BoundUnaryOperator) As Boolean
			Dim flag As Boolean
			flag = If(Not expression.Checked OrElse expression.OperatorKind <> UnaryOperatorKind.Minus OrElse Not expression.Type.IsIntegralType(), False, True)
			Return flag
		End Function

		Private Function OperatorHasSideEffects(ByVal expression As BoundBinaryOperator) As Boolean
			Dim flag As Boolean
			Dim operatorKind As BinaryOperatorKind = expression.OperatorKind And BinaryOperatorKind.OpMask
			If (operatorKind = BinaryOperatorKind.Add OrElse CInt(operatorKind) - CInt(BinaryOperatorKind.Subtract) <= CInt(BinaryOperatorKind.Add)) Then
				flag = If(Not expression.Checked, False, expression.Type.IsIntegralType())
			Else
				flag = If(CInt(operatorKind) - CInt(BinaryOperatorKind.Divide) > CInt(BinaryOperatorKind.Concatenate), False, True)
			End If
			Return flag
		End Function

		Private Function ParameterSlot(ByVal parameter As BoundParameter) As Integer
			Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = parameter.ParameterSymbol
			Dim ordinal As Integer = parameterSymbol.Ordinal
			If (Not parameterSymbol.ContainingSymbol.IsShared) Then
				ordinal = ordinal + 1
			End If
			Return ordinal
		End Function

		Private Function PartialCtorResultCannotEscape(ByVal left As BoundExpression) As Boolean
			If (Me._tryNestingLevel <> 0) Then
				Return False
			End If
			Return Me.TargetIsNotOnHeap(left)
		End Function

		Private Function SafeToGetWriteableReference(ByVal left As BoundExpression) As Boolean
			If (Not Me.AllowedToTakeRef(left, CodeGenerator.AddressKind.Writeable)) Then
				Return False
			End If
			If (left.Kind <> BoundKind.ArrayAccess) Then
				Return True
			End If
			Return left.Type.TypeKind <> Microsoft.CodeAnalysis.TypeKind.TypeParameter
		End Function

		Private Sub SerializeArrayRecursive(ByVal bw As BlobBuilder, ByVal inits As ImmutableArray(Of BoundExpression))
			If (inits.Length <> 0) Then
				If (inits(0).Kind = BoundKind.ArrayInitialization) Then
					Dim enumerator As ImmutableArray(Of BoundExpression).Enumerator = inits.GetEnumerator()
					While enumerator.MoveNext()
						Me.SerializeArrayRecursive(bw, DirectCast(enumerator.Current, BoundArrayInitialization).Initializers)
					End While
					Return
				End If
				Dim enumerator1 As ImmutableArray(Of BoundExpression).Enumerator = inits.GetEnumerator()
				While enumerator1.MoveNext()
					Me.AsConstOrDefault(enumerator1.Current).Serialize(bw)
				End While
			End If
		End Sub

		Private Sub SetInitialDebugDocument()
			Dim syntax As SyntaxNode = Me._method.Syntax
			If (Me._emitPdbSequencePoints AndAlso syntax IsNot Nothing) Then
				Me._builder.SetInitialDebugDocument(syntax.SyntaxTree)
			End If
		End Sub

		Private Function ShouldEmitBlockInitializer(ByVal elementType As TypeSymbol, ByVal inits As ImmutableArray(Of BoundExpression)) As CodeGenerator.ArrayInitializerStyle
			Dim arrayInitializerStyle As CodeGenerator.ArrayInitializerStyle
			If (Me._module.SupportsPrivateImplClass) Then
				If (elementType.IsEnumType()) Then
					If (Not Me._module.Compilation.EnableEnumArrayBlockInitialization) Then
						arrayInitializerStyle = CodeGenerator.ArrayInitializerStyle.Element
						Return arrayInitializerStyle
					End If
					elementType = DirectCast(elementType, NamedTypeSymbol).EnumUnderlyingType
				End If
				If (elementType.SpecialType.IsBlittable()) Then
					If (Me._module.GetInitArrayHelper() IsNot Nothing) Then
						Dim num As Integer = 0
						Dim num1 As Integer = 0
						Me.InitializerCountRecursive(inits, num, num1)
						If (num > 2) Then
							If (num <> num1) Then
								If (num1 < Math.Max(3, num / 3)) Then
									GoTo Label2
								End If
								arrayInitializerStyle = CodeGenerator.ArrayInitializerStyle.Mixed
								Return arrayInitializerStyle
							Else
								arrayInitializerStyle = CodeGenerator.ArrayInitializerStyle.Block
								Return arrayInitializerStyle
							End If
						End If
					Else
						arrayInitializerStyle = CodeGenerator.ArrayInitializerStyle.Element
						Return arrayInitializerStyle
					End If
				End If
			Label2:
				arrayInitializerStyle = CodeGenerator.ArrayInitializerStyle.Element
			Else
				arrayInitializerStyle = CodeGenerator.ArrayInitializerStyle.Element
			End If
			Return arrayInitializerStyle
		End Function

		Private Shared Function ShouldEmitInitExpression(ByVal includeConstants As Boolean, ByVal init As BoundExpression) As Boolean
			If (init.ConstantValueOpt Is Nothing) Then
				Return True
			End If
			If (Not includeConstants) Then
				Return False
			End If
			Return Not init.ConstantValueOpt.IsDefaultValue
		End Function

		Private Function ShouldNoteProjectErrors() As Boolean
			Return Not Me._module.SourceModule.ContainingSourceAssembly.IsVbRuntime
		End Function

		Private Function StackMergeType(ByVal expr As BoundExpression) As TypeSymbol
			Dim type As TypeSymbol
			If (expr.Type.IsArrayType() OrElse expr.Type.IsInterfaceType() OrElse expr.Type.IsDelegateType()) Then
				Dim kind As BoundKind = expr.Kind
				If (kind <= BoundKind.[DirectCast]) Then
					If (kind = BoundKind.Dup) Then
						type = Nothing
						Return type
					ElseIf (kind = BoundKind.AssignmentOperator) Then
						type = Me.StackMergeType(DirectCast(expr, BoundAssignmentOperator).Right)
						Return type
					ElseIf (kind = BoundKind.[DirectCast]) Then
						Dim boundDirectCast As Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast = DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.BoundDirectCast)
						If (Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsWideningConversion(boundDirectCast.ConversionKind)) Then
							GoTo Label1
						End If
						type = Me.StackMergeType(boundDirectCast.Operand)
						Return type
					End If
				ElseIf (kind = BoundKind.[TryCast]) Then
					Dim boundTryCast As Microsoft.CodeAnalysis.VisualBasic.BoundTryCast = DirectCast(expr, Microsoft.CodeAnalysis.VisualBasic.BoundTryCast)
					If (Not Microsoft.CodeAnalysis.VisualBasic.Conversions.IsWideningConversion(boundTryCast.ConversionKind)) Then
						GoTo Label1
					End If
					type = Me.StackMergeType(boundTryCast.Operand)
					Return type
				ElseIf (kind = BoundKind.Sequence) Then
					type = Me.StackMergeType(DirectCast(expr, BoundSequence).ValueOpt)
					Return type
				ElseIf (kind = BoundKind.Local) Then
					If (Not Me.IsStackLocal(DirectCast(expr, BoundLocal).LocalSymbol)) Then
						GoTo Label1
					End If
					type = Nothing
					Return type
				End If
			Label1:
				type = expr.Type
			Else
				type = expr.Type
			End If
			Return type
		End Function

		Private Function TargetIsNotOnHeap(ByVal left As BoundExpression) As Boolean
			Dim isByRef As Boolean
			Dim kind As BoundKind = left.Kind
			If (kind = BoundKind.ReferenceAssignment) Then
				isByRef = False
			ElseIf (kind = BoundKind.Local) Then
				isByRef = Not DirectCast(left, BoundLocal).LocalSymbol.IsByRef
			Else
				isByRef = If(kind = BoundKind.Parameter, Not DirectCast(left, BoundParameter).ParameterSymbol.IsByRef, False)
			End If
			Return isByRef
		End Function

		Private Function TryEmitAssignmentInPlace(ByVal assignmentOperator As BoundAssignmentOperator, ByVal used As Boolean) As Boolean
			Dim flag As Boolean
			Dim left As BoundExpression = assignmentOperator.Left
			If (used AndAlso Not Me.TargetIsNotOnHeap(left)) Then
				flag = False
			ElseIf (Me.SafeToGetWriteableReference(left)) Then
				Dim right As BoundExpression = assignmentOperator.Right
				Dim type As TypeSymbol = right.Type
				If (Not type.IsTypeParameter() AndAlso (type.IsReferenceType OrElse right.ConstantValueOpt IsNot Nothing AndAlso type.SpecialType <> SpecialType.System_Decimal)) Then
					flag = False
				ElseIf (right.IsDefaultValue()) Then
					Me.InPlaceInit(left, used)
					flag = True
				ElseIf (right.Kind <> BoundKind.ObjectCreationExpression OrElse Not Me.PartialCtorResultCannotEscape(left)) Then
					flag = False
				Else
					Me.InPlaceCtorCall(left, DirectCast(right, BoundObjectCreationExpression), used)
					flag = True
				End If
			Else
				flag = False
			End If
			Return flag
		End Function

		Private Function TryReduce(ByVal condition As BoundBinaryOperator, ByRef sense As Boolean) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim left As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim flag As Boolean
			Dim operatorKind As BinaryOperatorKind = condition.OperatorKind And BinaryOperatorKind.OpMask
			Dim constantValueOpt As ConstantValue = condition.Left.ConstantValueOpt
			If (constantValueOpt Is Nothing) Then
				constantValueOpt = condition.Right.ConstantValueOpt
				If (constantValueOpt Is Nothing) Then
					boundExpression = Nothing
					Return boundExpression
				End If
				left = condition.Left
			Else
				left = condition.Right
			End If
			Dim type As TypeSymbol = left.Type
			If (type Is Nothing OrElse Me.CanPassToBrfalse(type)) Then
				flag = If(type Is Nothing, False, type.PrimitiveTypeCode = Microsoft.Cci.PrimitiveTypeCode.[Boolean])
				Dim isDefaultValue As Boolean = constantValueOpt.IsDefaultValue
				If (flag OrElse isDefaultValue) Then
					If (isDefaultValue) Then
						sense = Not sense
					End If
					If (operatorKind = BinaryOperatorKind.NotEquals OrElse operatorKind = BinaryOperatorKind.[IsNot]) Then
						sense = Not sense
					End If
					boundExpression = left
				Else
					boundExpression = Nothing
				End If
			Else
				boundExpression = Nothing
			End If
			Return boundExpression
		End Function

		<Conditional("DEBUG")>
		Private Sub ValidateReferenceEqualityOperands(ByVal binOp As BoundBinaryOperator)
		End Sub

		Private Enum AddressKind
			Writeable
			[ReadOnly]
			Immutable
		End Enum

		Private Enum ArrayInitializerStyle
			Element
			Block
			Mixed
		End Enum

		Private Enum CallKind
			[Call]
			CallVirt
			ConstrainedCallVirt
		End Enum

		Private Enum ConstResKind
			ConstFalse
			ConstTrue
			NotAConst
		End Enum

		Private Class EmitCancelledException
			Inherits Exception
			Public Sub New()
				MyBase.New()
			End Sub
		End Class

		Private Structure IndexDesc
			Public ReadOnly Index As Integer

			Public ReadOnly Initializers As ImmutableArray(Of BoundExpression)

			Public Sub New(ByVal Index As Integer, ByVal Initializers As ImmutableArray(Of BoundExpression))
				Me = New CodeGenerator.IndexDesc() With
				{
					.Index = Index,
					.Initializers = Initializers
				}
			End Sub
		End Structure

		Private Class LabelFinder
			Inherits StatementWalker
			Private ReadOnly _label As LabelSymbol

			Private _found As Boolean

			Private Sub New(ByVal label As LabelSymbol)
				MyBase.New()
				Me._found = False
				Me._label = label
			End Sub

			Public Shared Function NodeContainsLabel(ByVal node As BoundNode, ByVal label As LabelSymbol) As Boolean
				Dim labelFinder As CodeGenerator.LabelFinder = New CodeGenerator.LabelFinder(label)
				labelFinder.Visit(node)
				Return labelFinder._found
			End Function

			Public Overrides Function Visit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
				If (Me._found OrElse TypeOf node Is BoundExpression) Then
					boundNode = Nothing
				Else
					boundNode = MyBase.Visit(node)
				End If
				Return boundNode
			End Function

			Public Overrides Function VisitLabelStatement(ByVal node As BoundLabelStatement) As BoundNode
				If (CObj(node.Label) = CObj(Me._label)) Then
					Me._found = True
				End If
				Return MyBase.VisitLabelStatement(node)
			End Function
		End Class

		Private Enum UseKind
			Unused
			UsedAsValue
			UsedAsAddress
		End Enum
	End Class
End Namespace