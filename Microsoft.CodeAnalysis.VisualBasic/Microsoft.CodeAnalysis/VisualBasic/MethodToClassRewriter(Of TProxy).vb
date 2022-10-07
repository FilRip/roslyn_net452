Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeGen
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class MethodToClassRewriter(Of TProxy)
		Inherits BoundTreeRewriterWithStackGuard
		Protected ReadOnly Proxies As Dictionary(Of Symbol, TProxy)

		Protected ReadOnly LocalMap As Dictionary(Of LocalSymbol, LocalSymbol)

		Protected ReadOnly ParameterMap As Dictionary(Of ParameterSymbol, ParameterSymbol)

		Protected ReadOnly PlaceholderReplacementMap As Dictionary(Of BoundValuePlaceholderBase, BoundExpression)

		Protected ReadOnly CompilationState As TypeCompilationState

		Protected ReadOnly Diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag

		Protected ReadOnly SlotAllocatorOpt As VariableSlotAllocator

		Protected ReadOnly PreserveOriginalLocals As Boolean

		Protected MustOverride ReadOnly Property CurrentMethod As MethodSymbol

		Protected MustOverride ReadOnly Property IsInExpressionLambda As Boolean

		Protected MustOverride ReadOnly Property TopLevelMethod As MethodSymbol

		Protected MustOverride ReadOnly Property TypeMap As TypeSubstitution

		Protected Sub New(ByVal slotAllocatorOpt As VariableSlotAllocator, ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, ByVal preserveOriginalLocals As Boolean)
			MyBase.New()
			Me.Proxies = New Dictionary(Of Symbol, TProxy)()
			Me.LocalMap = New Dictionary(Of LocalSymbol, LocalSymbol)()
			Me.ParameterMap = New Dictionary(Of ParameterSymbol, ParameterSymbol)()
			Me.PlaceholderReplacementMap = New Dictionary(Of BoundValuePlaceholderBase, BoundExpression)()
			Me.CompilationState = compilationState
			Me.Diagnostics = diagnostics
			Me.SlotAllocatorOpt = slotAllocatorOpt
			Me.PreserveOriginalLocals = preserveOriginalLocals
		End Sub

		Protected Shared Function CreateReplacementLocalOrReturnSelf(ByVal originalLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol, ByVal newType As TypeSymbol, Optional ByVal onlyReplaceIfFunctionValue As Boolean = False, <Out> Optional ByRef wasReplaced As Boolean = False) As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol
			Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol
			If (Not onlyReplaceIfFunctionValue OrElse originalLocal.IsFunctionValue) Then
				wasReplaced = True
				localSymbol = Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol.Create(originalLocal, newType)
			Else
				wasReplaced = False
				localSymbol = originalLocal
			End If
			Return localSymbol
		End Function

		Friend MustOverride Function FramePointer(ByVal syntax As SyntaxNode, ByVal frameClass As NamedTypeSymbol) As BoundExpression

		Private Function GetOrCreateMyBaseOrMyClassWrapperFunction(ByVal receiver As BoundExpression, ByVal method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim boundReturnStatement As BoundStatement
			method = method.ConstructedFrom
			Dim methodWrapper As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me.CompilationState.GetMethodWrapper(method)
			If (methodWrapper Is Nothing) Then
				Dim containingType As NamedTypeSymbol = Me.TopLevelMethod.ContainingType
				Dim flag As Boolean = Not method.ContainingType.Equals(containingType)
				Dim syntax As SyntaxNode = Me.CurrentMethod.Syntax
				Dim str As String = GeneratedNames.MakeBaseMethodWrapperName(method.Name, flag)
				Dim synthesizedWrapperMethod As MethodToClassRewriter(Of TProxy).SynthesizedWrapperMethod = New MethodToClassRewriter(Of TProxy).SynthesizedWrapperMethod(DirectCast(containingType, InstanceTypeSymbol), method, str, syntax)
				If (Me.CompilationState.ModuleBuilderOpt IsNot Nothing) Then
					Me.CompilationState.ModuleBuilderOpt.AddSynthesizedDefinition(containingType, synthesizedWrapperMethod.GetCciAdapter())
				End If
				Dim wrappedMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = synthesizedWrapperMethod.WrappedMethod
				Dim boundParameter(synthesizedWrapperMethod.ParameterCount - 1 + 1 - 1) As BoundExpression
				Dim parameterCount As Integer = synthesizedWrapperMethod.ParameterCount - 1
				Dim num As Integer = 0
				Do
					Dim item As ParameterSymbol = synthesizedWrapperMethod.Parameters(num)
					boundParameter(num) = New Microsoft.CodeAnalysis.VisualBasic.BoundParameter(syntax, item, item.IsByRef, item.Type)
					num = num + 1
				Loop While num <= parameterCount
				Dim meParameter As ParameterSymbol = synthesizedWrapperMethod.MeParameter
				Dim boundMyClassReference As BoundExpression = Nothing
				If (Not flag) Then
					boundMyClassReference = New Microsoft.CodeAnalysis.VisualBasic.BoundMyClassReference(syntax, meParameter.Type)
				Else
					boundMyClassReference = New BoundMyBaseReference(syntax, meParameter.Type)
				End If
				Dim boundExpressions As ImmutableArray(Of BoundExpression) = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of BoundExpression)(boundParameter)
				Dim returnType As TypeSymbol = wrappedMethod.ReturnType
				Dim bitVector As Microsoft.CodeAnalysis.BitVector = New Microsoft.CodeAnalysis.BitVector()
				Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(syntax, wrappedMethod, Nothing, boundMyClassReference, boundExpressions, Nothing, returnType, False, False, bitVector)
				If (Not wrappedMethod.ReturnType.IsVoidType()) Then
					boundReturnStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement(syntax, boundCall, Nothing, Nothing, False)
				Else
					Dim statementSyntaxes As SyntaxList(Of StatementSyntax) = New SyntaxList(Of StatementSyntax)()
					boundReturnStatement = New BoundBlock(syntax, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray.Create(Of BoundStatement)(New BoundExpressionStatement(syntax, boundCall, False), New Microsoft.CodeAnalysis.VisualBasic.BoundReturnStatement(syntax, Nothing, Nothing, Nothing, False)), False)
				End If
				Me.CompilationState.AddMethodWrapper(method, synthesizedWrapperMethod, boundReturnStatement)
				methodSymbol = synthesizedWrapperMethod
			Else
				methodSymbol = methodWrapper
			End If
			Return methodSymbol
		End Function

		Protected MustOverride Function MaterializeProxy(ByVal origExpression As BoundExpression, ByVal proxy As TProxy) As BoundNode

		Protected Function RewriteBlock(ByVal node As BoundBlock, ByVal prologue As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression), ByVal newLocals As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)) As BoundBlock
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol).Enumerator = node.Locals.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = enumerator.Current
				If (Not Me.PreserveOriginalLocals AndAlso Me.Proxies.ContainsKey(current)) Then
					Continue While
				End If
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(current.Type)
				If (Not Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(typeSymbol, current.Type, TypeCompareKind.ConsiderEverything)) Then
					Dim flag As Boolean = False
					Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = MethodToClassRewriter(Of TProxy).CreateReplacementLocalOrReturnSelf(current, typeSymbol, False, flag)
					newLocals.Add(localSymbol)
					Me.LocalMap.Add(current, localSymbol)
				Else
					Dim localSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Nothing
					Dim flag1 As Boolean = False
					If (Not Me.LocalMap.TryGetValue(current, localSymbol1)) Then
						localSymbol1 = MethodToClassRewriter(Of TProxy).CreateReplacementLocalOrReturnSelf(current, typeSymbol, True, flag1)
					End If
					If (flag1) Then
						Me.LocalMap.Add(current, localSymbol1)
					End If
					newLocals.Add(localSymbol1)
				End If
			End While
			Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement).GetInstance()
			Dim num As Integer = 0
			Dim statements As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundStatement) = node.Statements
			If (prologue.Count > 0) Then
				If (statements.Length > 0 AndAlso statements(0).Syntax IsNot Nothing) Then
					Dim statementOpt As Boolean = False
					Dim kind As BoundKind = statements(0).Kind
					If (kind = BoundKind.SequencePoint) Then
						statementOpt = DirectCast(statements(0), BoundSequencePoint).StatementOpt Is Nothing
					ElseIf (kind = BoundKind.SequencePointWithSpan) Then
						statementOpt = DirectCast(statements(0), BoundSequencePointWithSpan).StatementOpt Is Nothing
					End If
					If (statementOpt) Then
						Dim boundStatement As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(statements(0)), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
						If (boundStatement IsNot Nothing) Then
							instance.Add(boundStatement)
						End If
						num = 1
					End If
				End If
				instance.Add((New BoundSequencePoint(Nothing, Nothing, False)).MakeCompilerGenerated())
			End If
			Dim enumerator1 As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Enumerator = prologue.GetEnumerator()
			While enumerator1.MoveNext()
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = enumerator1.Current
				instance.Add(New BoundExpressionStatement(boundExpression.Syntax, boundExpression, False))
			End While
			prologue.Free()
			Dim length As Integer = statements.Length - 1
			Dim num1 As Integer = num
			Do
				Dim boundStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatement = DirectCast(Me.Visit(statements(num1)), Microsoft.CodeAnalysis.VisualBasic.BoundStatement)
				If (boundStatement1 IsNot Nothing) Then
					instance.Add(boundStatement1)
				End If
				num1 = num1 + 1
			Loop While num1 <= length
			Return node.Update(node.StatementListSyntax, newLocals.ToImmutableAndFree(), instance.ToImmutableAndFree())
		End Function

		Protected Function RewriteBlock(ByVal node As BoundBlock) As BoundBlock
			Dim instance As ArrayBuilder(Of BoundExpression) = ArrayBuilder(Of BoundExpression).GetInstance()
			Return Me.RewriteBlock(node, instance, ArrayBuilder(Of LocalSymbol).GetInstance())
		End Function

		Protected Function RewriteSequence(ByVal node As BoundSequence) As BoundSequence
			Dim instance As ArrayBuilder(Of BoundExpression) = ArrayBuilder(Of BoundExpression).GetInstance()
			Return Me.RewriteSequence(node, instance, ArrayBuilder(Of LocalSymbol).GetInstance())
		End Function

		Protected Function RewriteSequence(ByVal node As BoundSequence, ByVal prologue As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression), ByVal newLocals As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)) As BoundSequence
			Dim enumerator As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol).Enumerator = node.Locals.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = enumerator.Current
				If (Me.Proxies.ContainsKey(current)) Then
					Continue While
				End If
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(current.Type)
				If (Not Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol.Equals(typeSymbol, current.Type, TypeCompareKind.ConsiderEverything)) Then
					Dim flag As Boolean = False
					Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = MethodToClassRewriter(Of TProxy).CreateReplacementLocalOrReturnSelf(current, typeSymbol, False, flag)
					newLocals.Add(localSymbol)
					Me.LocalMap.Add(current, localSymbol)
				Else
					newLocals.Add(current)
				End If
			End While
			Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Enumerator = node.SideEffects.GetEnumerator()
			While enumerator1.MoveNext()
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(enumerator1.Current), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				If (boundExpression Is Nothing) Then
					Continue While
				End If
				prologue.Add(boundExpression)
			End While
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ValueOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Return node.Update(newLocals.ToImmutableAndFree(), prologue.ToImmutableAndFree(), boundExpression1, If(boundExpression1 Is Nothing, node.Type, boundExpression1.Type))
		End Function

		Private Function ShouldRewriteMethodSymbol(ByVal originalReceiver As BoundExpression, ByVal rewrittenReceiverOpt As BoundExpression, ByVal newMethod As MethodSymbol) As Boolean
			If (originalReceiver <> rewrittenReceiverOpt OrElse Not newMethod.IsDefinition OrElse Me.TypeMap IsNot Nothing AndAlso Me.TypeMap.TargetGenericDefinition.Equals(newMethod)) Then
				Return True
			End If
			If (Not Me.IsInExpressionLambda OrElse rewrittenReceiverOpt Is Nothing) Then
				Return False
			End If
			If (rewrittenReceiverOpt.IsMyClassReference()) Then
				Return True
			End If
			Return rewrittenReceiverOpt.IsMyBaseReference()
		End Function

		Private Function SubstituteMethodForMyBaseOrMyClassCall(ByVal receiverOpt As BoundExpression, ByVal originalMethodBeingCalled As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			If ((originalMethodBeingCalled.IsMetadataVirtual() OrElse Me.IsInExpressionLambda) AndAlso receiverOpt IsNot Nothing AndAlso (receiverOpt.Kind = BoundKind.MyBaseReference OrElse receiverOpt.Kind = BoundKind.MyClassReference) AndAlso (Me.CurrentMethod.ContainingType <> Me.TopLevelMethod.ContainingType OrElse Me.IsInExpressionLambda)) Then
				Dim orCreateMyBaseOrMyClassWrapperFunction As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me.GetOrCreateMyBaseOrMyClassWrapperFunction(receiverOpt, originalMethodBeingCalled)
				If (orCreateMyBaseOrMyClassWrapperFunction.IsGenericMethod) Then
					Dim typeArguments As ImmutableArray(Of TypeSymbol) = originalMethodBeingCalled.TypeArguments
					Dim typeSymbolArray(typeArguments.Length - 1 + 1 - 1) As TypeSymbol
					Dim length As Integer = typeArguments.Length - 1
					Dim num As Integer = 0
					Do
						typeSymbolArray(num) = Me.VisitType(typeArguments(num))
						num = num + 1
					Loop While num <= length
					orCreateMyBaseOrMyClassWrapperFunction = orCreateMyBaseOrMyClassWrapperFunction.Construct(Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeSymbol)(typeSymbolArray))
				End If
				methodSymbol = orCreateMyBaseOrMyClassWrapperFunction
			Else
				methodSymbol = originalMethodBeingCalled
			End If
			Return methodSymbol
		End Function

		Public Overrides Function VisitAwaitOperator(ByVal node As BoundAwaitOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim awaitableInstancePlaceholder As BoundRValuePlaceholder = node.AwaitableInstancePlaceholder
			Me.PlaceholderReplacementMap.Add(awaitableInstancePlaceholder, awaitableInstancePlaceholder.Update(Me.VisitType(awaitableInstancePlaceholder.Type)))
			Dim awaiterInstancePlaceholder As BoundLValuePlaceholder = node.AwaiterInstancePlaceholder
			Me.PlaceholderReplacementMap.Add(awaiterInstancePlaceholder, awaiterInstancePlaceholder.Update(Me.VisitType(awaiterInstancePlaceholder.Type)))
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = MyBase.VisitAwaitOperator(node)
			Me.PlaceholderReplacementMap.Remove(awaitableInstancePlaceholder)
			Me.PlaceholderReplacementMap.Remove(awaiterInstancePlaceholder)
			Return boundNode
		End Function

		Public Overrides Function VisitBlock(ByVal node As BoundBlock) As BoundNode
			Return Me.RewriteBlock(node)
		End Function

		Public Overrides Function VisitCall(ByVal node As BoundCall) As BoundNode
			Dim receiverOpt As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = node.ReceiverOpt
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(receiverOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = node.Method
			Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = Me.VisitList(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(node.Arguments)
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.VisitType(node.Type)
			If (Me.ShouldRewriteMethodSymbol(receiverOpt, boundExpression, method)) Then
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me.SubstituteMethodForMyBaseOrMyClassCall(receiverOpt, node.Method)
				method = Me.VisitMethodSymbol(methodSymbol)
			End If
			Return node.Update(method, Nothing, boundExpression, boundExpressions, node.DefaultArguments, node.ConstantValueOpt, node.IsLValue, node.SuppressObjectClone, typeSymbol)
		End Function

		Public Overrides MustOverride Function VisitCatchBlock(ByVal node As BoundCatchBlock) As BoundNode

		Public Overrides Function VisitDelegateCreationExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression) As BoundNode
			Dim boundDelegateCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression = DirectCast(MyBase.VisitDelegateCreationExpression(node), Microsoft.CodeAnalysis.VisualBasic.BoundDelegateCreationExpression)
			Dim method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = boundDelegateCreationExpression.Method
			Dim receiverOpt As BoundExpression = boundDelegateCreationExpression.ReceiverOpt
			If (Me.ShouldRewriteMethodSymbol(node.ReceiverOpt, receiverOpt, method)) Then
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me.SubstituteMethodForMyBaseOrMyClassCall(node.ReceiverOpt, node.Method)
				method = Me.VisitMethodSymbol(methodSymbol)
			End If
			Return node.Update(receiverOpt, method, boundDelegateCreationExpression.RelaxationLambdaOpt, boundDelegateCreationExpression.RelaxationReceiverPlaceholderOpt, Nothing, boundDelegateCreationExpression.Type)
		End Function

		Public Overrides Function VisitFieldAccess(ByVal node As BoundFieldAccess) As BoundNode
			Return node.Update(DirectCast(Me.Visit(node.ReceiverOpt), BoundExpression), Me.VisitFieldSymbol(node.FieldSymbol), node.IsLValue, node.SuppressVirtualCalls, Nothing, Me.VisitType(node.Type))
		End Function

		Public Overrides Function VisitFieldInfo(ByVal node As BoundFieldInfo) As BoundNode
			Return node.Update(Me.VisitFieldSymbol(node.Field), Me.VisitType(node.Type))
		End Function

		Private Function VisitFieldSymbol(ByVal field As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol
			Dim typeMap As TypeSubstitution = Me.TypeMap
			If (typeMap Is Nothing) Then
				fieldSymbol = field
			Else
				Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = field.OriginalDefinition
				Dim typeWithModifier As TypeWithModifiers = field.ContainingType.InternalSubstituteTypeParameters(typeMap)
				Dim substitutedNamedType As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedNamedType = TryCast(typeWithModifier.AsTypeSymbolOnly(), Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedNamedType)
				If (substitutedNamedType IsNot Nothing) Then
					originalDefinition = DirectCast(substitutedNamedType.GetMemberForDefinition(originalDefinition), Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol)
				End If
				fieldSymbol = originalDefinition
			End If
			Return fieldSymbol
		End Function

		Public NotOverridable Overrides Function VisitLocal(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundLocal) As BoundNode
			Dim boundLocal As BoundNode
			Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = node.LocalSymbol
			If (Not localSymbol.IsConst) Then
				Dim tProxy As TProxy = Nothing
				If (Not Me.Proxies.TryGetValue(localSymbol, tProxy)) Then
					Dim localSymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = Nothing
					If (Not Me.LocalMap.TryGetValue(localSymbol, localSymbol1)) Then
						boundLocal = MyBase.VisitLocal(node)
					Else
						boundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(node.Syntax, localSymbol1, node.IsLValue, localSymbol1.Type, node.HasErrors)
					End If
				Else
					boundLocal = Me.MaterializeProxy(node, tProxy)
				End If
			Else
				boundLocal = MyBase.VisitLocal(node)
			End If
			Return boundLocal
		End Function

		Public Overrides Function VisitLocalDeclaration(ByVal node As BoundLocalDeclaration) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = node.LocalSymbol
			Dim tProxy As TProxy = Nothing
			If (Not Me.Proxies.TryGetValue(localSymbol, tProxy)) Then
				boundNode = node
			Else
				boundNode = Nothing
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitLValuePlaceholder(ByVal node As BoundLValuePlaceholder) As BoundNode
			Return Me.PlaceholderReplacementMap(node)
		End Function

		Public NotOverridable Overrides Function VisitMethodInfo(ByVal node As BoundMethodInfo) As BoundNode
			Return node.Update(Me.VisitMethodSymbol(node.Method), Me.VisitType(node.Type))
		End Function

		Private Function VisitMethodSymbol(ByVal method As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol
			Dim typeMap As TypeSubstitution = Me.TypeMap
			If (typeMap Is Nothing) Then
				methodSymbol = method
			Else
				Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = method.OriginalDefinition
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = method.ContainingType.InternalSubstituteTypeParameters(typeMap).AsTypeSymbolOnly()
				Dim substitutedNamedType As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedNamedType = TryCast(typeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedNamedType)
				If (substitutedNamedType Is Nothing) Then
					Dim anonymousTypeOrDelegatePublicSymbol As AnonymousTypeManager.AnonymousTypeOrDelegatePublicSymbol = TryCast(typeSymbol, AnonymousTypeManager.AnonymousTypeOrDelegatePublicSymbol)
					If (anonymousTypeOrDelegatePublicSymbol IsNot Nothing) Then
						originalDefinition = anonymousTypeOrDelegatePublicSymbol.FindSubstitutedMethodSymbol(originalDefinition)
					End If
				Else
					originalDefinition = DirectCast(substitutedNamedType.GetMemberForDefinition(originalDefinition), Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
				End If
				If (originalDefinition.IsGenericMethod) Then
					Dim typeArguments As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) = method.TypeArguments
					Dim typeSymbolArray(typeArguments.Length - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
					Dim length As Integer = typeArguments.Length - 1
					Dim num As Integer = 0
					Do
						typeSymbolArray(num) = Me.VisitType(typeArguments(num))
						num = num + 1
					Loop While num <= length
					originalDefinition = originalDefinition.Construct(typeSymbolArray)
				End If
				methodSymbol = originalDefinition
			End If
			Return methodSymbol
		End Function

		Public Overrides Function VisitObjectCreationExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression) As BoundNode
			Dim boundObjectCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression = DirectCast(MyBase.VisitObjectCreationExpression(node), Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression)
			Dim constructorOpt As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = boundObjectCreationExpression.ConstructorOpt
			If (constructorOpt IsNot Nothing AndAlso (CObj(node.Type) <> CObj(boundObjectCreationExpression.Type) OrElse Not constructorOpt.IsDefinition)) Then
				Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = Me.VisitMethodSymbol(constructorOpt)
				boundObjectCreationExpression = node.Update(methodSymbol, boundObjectCreationExpression.Arguments, boundObjectCreationExpression.DefaultArguments, boundObjectCreationExpression.InitializerOpt, boundObjectCreationExpression.Type)
			End If
			Return boundObjectCreationExpression
		End Function

		Public NotOverridable Overrides Function VisitParameter(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundParameter) As BoundNode
			Dim boundParameter As BoundNode
			Dim tProxy As TProxy = Nothing
			If (Not Me.Proxies.TryGetValue(node.ParameterSymbol, tProxy)) Then
				Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = Nothing
				If (Not Me.ParameterMap.TryGetValue(node.ParameterSymbol, parameterSymbol)) Then
					boundParameter = MyBase.VisitParameter(node)
				Else
					boundParameter = New Microsoft.CodeAnalysis.VisualBasic.BoundParameter(node.Syntax, parameterSymbol, node.IsLValue, parameterSymbol.Type, node.HasErrors)
				End If
			Else
				boundParameter = Me.MaterializeProxy(node, tProxy)
			End If
			Return boundParameter
		End Function

		Public Overrides Function VisitPropertyAccess(ByVal node As BoundPropertyAccess) As BoundNode
			Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = Me.VisitPropertySymbol(node.PropertySymbol)
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = DirectCast(Me.Visit(node.ReceiverOpt), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
			Dim arguments As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = node.Arguments
			Dim boundExpressionArray(arguments.Length - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim length As Integer = arguments.Length - 1
			Dim num As Integer = 0
			Do
				boundExpressionArray(num) = DirectCast(Me.Visit(arguments(num)), Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				num = num + 1
			Loop While num <= length
			Return node.Update(propertySymbol, Nothing, node.AccessKind, node.IsWriteable, node.IsLValue, boundExpression, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpressionArray), node.DefaultArguments, Me.VisitType(node.Type))
		End Function

		Private Function VisitPropertySymbol(ByVal [property] As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol
			Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol
			Dim typeMap As TypeSubstitution = Me.TypeMap
			If (typeMap Is Nothing) Then
				propertySymbol = [property]
			Else
				Dim originalDefinition As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = [property].OriginalDefinition
				Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = [property].ContainingType.InternalSubstituteTypeParameters(typeMap).AsTypeSymbolOnly()
				Dim substitutedNamedType As Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedNamedType = TryCast(typeSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.SubstitutedNamedType)
				If (substitutedNamedType Is Nothing) Then
					Dim anonymousTypePublicSymbol As AnonymousTypeManager.AnonymousTypePublicSymbol = TryCast(typeSymbol, AnonymousTypeManager.AnonymousTypePublicSymbol)
					If (anonymousTypePublicSymbol IsNot Nothing) Then
						originalDefinition = anonymousTypePublicSymbol.Properties(TryCast(originalDefinition, AnonymousTypeManager.AnonymousTypePropertyPublicSymbol).PropertyIndex)
					End If
				Else
					originalDefinition = DirectCast(substitutedNamedType.GetMemberForDefinition(originalDefinition), Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
				End If
				propertySymbol = originalDefinition
			End If
			Return propertySymbol
		End Function

		Public Overrides Function VisitRValuePlaceholder(ByVal node As BoundRValuePlaceholder) As BoundNode
			Return Me.PlaceholderReplacementMap(node)
		End Function

		Public Overrides Function VisitSelectStatement(ByVal node As BoundSelectStatement) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim exprPlaceholderOpt As BoundRValuePlaceholder = node.ExprPlaceholderOpt
			If (exprPlaceholderOpt IsNot Nothing) Then
				Me.PlaceholderReplacementMap.Add(exprPlaceholderOpt, exprPlaceholderOpt.Update(Me.VisitType(exprPlaceholderOpt.Type)))
			End If
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = MyBase.VisitSelectStatement(node)
			If (exprPlaceholderOpt IsNot Nothing) Then
				Me.PlaceholderReplacementMap.Remove(exprPlaceholderOpt)
			End If
			Return boundNode
		End Function

		Public Overrides Function VisitSequence(ByVal node As BoundSequence) As BoundNode
			Return Me.RewriteSequence(node)
		End Function

		Public NotOverridable Overrides Function VisitType(ByVal type As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol
			typeSymbol = If(type IsNot Nothing, type.InternalSubstituteTypeParameters(Me.TypeMap).Type, type)
			Return typeSymbol
		End Function

		Public Overrides Function VisitUserDefinedShortCircuitingOperator(ByVal node As BoundUserDefinedShortCircuitingOperator) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim leftOperandPlaceholder As BoundRValuePlaceholder = node.LeftOperandPlaceholder
			Me.PlaceholderReplacementMap.Add(leftOperandPlaceholder, leftOperandPlaceholder.Update(Me.VisitType(leftOperandPlaceholder.Type)))
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = MyBase.VisitUserDefinedShortCircuitingOperator(node)
			Me.PlaceholderReplacementMap.Remove(leftOperandPlaceholder)
			Return boundNode
		End Function

		Friend NotInheritable Class SynthesizedWrapperMethod
			Inherits SynthesizedMethod
			Private ReadOnly _wrappedMethod As MethodSymbol

			Private ReadOnly _typeMap As TypeSubstitution

			Private ReadOnly _typeParameters As ImmutableArray(Of TypeParameterSymbol)

			Private ReadOnly _parameters As ImmutableArray(Of ParameterSymbol)

			Private ReadOnly _returnType As TypeSymbol

			Private ReadOnly _locations As ImmutableArray(Of Location)

			Public Overrides ReadOnly Property Arity As Integer
				Get
					Return Me._typeParameters.Length
				End Get
			End Property

			Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
				Get
					Return Accessibility.[Private]
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

			Public Overrides ReadOnly Property IsShared As Boolean
				Get
					Return False
				End Get
			End Property

			Public Overrides ReadOnly Property IsSub As Boolean
				Get
					Return Me._wrappedMethod.IsSub
				End Get
			End Property

			Public Overrides ReadOnly Property IsVararg As Boolean
				Get
					Return Me._wrappedMethod.IsVararg
				End Get
			End Property

			Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
				Get
					Return Me._locations
				End Get
			End Property

			Friend Overrides ReadOnly Property ParameterCount As Integer
				Get
					Return Me._parameters.Length
				End Get
			End Property

			Public Overrides ReadOnly Property Parameters As ImmutableArray(Of ParameterSymbol)
				Get
					Return Me._parameters
				End Get
			End Property

			Public Overrides ReadOnly Property ReturnType As TypeSymbol
				Get
					Return Me._returnType
				End Get
			End Property

			Public Overrides ReadOnly Property TypeArguments As ImmutableArray(Of TypeSymbol)
				Get
					Dim empty As ImmutableArray(Of TypeSymbol)
					If (Me.Arity <= 0) Then
						empty = ImmutableArray(Of TypeSymbol).Empty
					Else
						empty = StaticCast(Of TypeSymbol).From(Of TypeParameterSymbol)(Me.TypeParameters)
					End If
					Return empty
				End Get
			End Property

			Friend Overrides ReadOnly Property TypeMap As TypeSubstitution
				Get
					Return Me._typeMap
				End Get
			End Property

			Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
				Get
					Return Me._typeParameters
				End Get
			End Property

			Public ReadOnly Property WrappedMethod As MethodSymbol
				Get
					Return Me._wrappedMethod
				End Get
			End Property

			Friend Sub New(ByVal containingType As InstanceTypeSymbol, ByVal methodToWrap As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal wrapperName As String, ByVal syntax As SyntaxNode)
				MyBase.New(syntax, containingType, wrapperName, False)
				Me._locations = ImmutableArray.Create(Of Location)(syntax.GetLocation())
				Me._typeMap = Nothing
				If (methodToWrap.IsGenericMethod) Then
					Me._typeParameters = SynthesizedClonedTypeParameterSymbol.MakeTypeParameters(methodToWrap.OriginalDefinition.TypeParameters, Me, SynthesizedMethod.CreateTypeParameter)
					Dim item(Me._typeParameters.Length - 1 + 1 - 1) As TypeSymbol
					Dim length As Integer = Me._typeParameters.Length - 1
					Dim num As Integer = 0
					Do
						item(num) = Me._typeParameters(num)
						num = num + 1
					Loop While num <= length
					Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = methodToWrap.Construct(Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeSymbol)(item))
					Me._typeMap = TypeSubstitution.Create(methodSymbol.OriginalDefinition, methodSymbol.OriginalDefinition.TypeParameters, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeSymbol)(item), False)
					Me._wrappedMethod = methodSymbol
				Else
					Me._typeParameters = ImmutableArray(Of TypeParameterSymbol).Empty
					Me._wrappedMethod = methodToWrap
				End If
				Dim parameterSymbolArray(Me._wrappedMethod.ParameterCount - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol
				Dim length1 As Integer = CInt(parameterSymbolArray.Length) - 1
				Dim num1 As Integer = 0
				Do
					Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = Me._wrappedMethod.Parameters(num1)
					parameterSymbolArray(num1) = SynthesizedMethod.WithNewContainerAndType(Me, parameterSymbol.Type.InternalSubstituteTypeParameters(Me._typeMap).Type, parameterSymbol)
					num1 = num1 + 1
				Loop While num1 <= length1
				Me._parameters = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)(parameterSymbolArray)
				Me._returnType = Me._wrappedMethod.ReturnType.InternalSubstituteTypeParameters(Me._typeMap).Type
			End Sub

			Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
				MyBase.AddSynthesizedAttributes(compilationState, attributes)
				Dim declaringCompilation As VisualBasicCompilation = Me.DeclaringCompilation
				Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
				Dim keyValuePairs As ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant)) = New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))()
				Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor, typedConstants, keyValuePairs, False))
				Symbol.AddSynthesizedAttribute(attributes, declaringCompilation.SynthesizeDebuggerHiddenAttribute())
			End Sub

			Friend Overrides Function CalculateLocalSyntaxOffset(ByVal localPosition As Integer, ByVal localTree As SyntaxTree) As Integer
				Throw ExceptionUtilities.Unreachable
			End Function

			Friend Overrides Function IsMetadataNewSlot(Optional ByVal ignoreInterfaceImplementationChanges As Boolean = False) As Boolean
				Return False
			End Function
		End Class
	End Class
End Namespace