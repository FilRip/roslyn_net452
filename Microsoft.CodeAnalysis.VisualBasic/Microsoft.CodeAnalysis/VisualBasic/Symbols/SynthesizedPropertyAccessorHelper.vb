Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections.Immutable
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Module SynthesizedPropertyAccessorHelper
		Friend Function GetBoundMethodBody(ByVal accessor As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol, ByVal backingField As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol, Optional ByRef methodBodyBinder As Binder = Nothing) As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim boundBlock As Microsoft.CodeAnalysis.VisualBasic.BoundBlock
			Dim localSymbols As ImmutableArray(Of LocalSymbol)
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal
			Dim statementSyntaxes As SyntaxList(Of StatementSyntax)
			Dim bitVector As Microsoft.CodeAnalysis.BitVector
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundAssignmentOperator As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim empty As ImmutableArray(Of LocalSymbol)
			Dim boundExpression2 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression3 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			methodBodyBinder = Nothing
			Dim associatedSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(accessor.AssociatedSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
			Dim root As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = VisualBasicSyntaxTree.Dummy.GetRoot(New CancellationToken())
			If (Not associatedSymbol.Type.IsVoidType()) Then
				Dim meParameter As ParameterSymbol = Nothing
				Dim boundMeReference As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				If (Not accessor.IsShared) Then
					meParameter = accessor.MeParameter
					boundMeReference = New Microsoft.CodeAnalysis.VisualBasic.BoundMeReference(root, meParameter.Type)
				End If
				Dim flag As Boolean = If(Not associatedSymbol.IsWithEvents, False, associatedSymbol.IsOverrides)
				Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = Nothing
				Dim boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = Nothing
				Dim boundMyBaseReference As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim boundCall As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				If (Not flag) Then
					fieldSymbol = backingField
					boundFieldAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess(root, boundMeReference, fieldSymbol, True, fieldSymbol.Type, False)
				Else
					boundMyBaseReference = New Microsoft.CodeAnalysis.VisualBasic.BoundMyBaseReference(root, meParameter.Type)
					Dim overriddenMethod As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = associatedSymbol.GetMethod.OverriddenMethod
					Dim u00210s As ImmutableArray(Of !0) = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty
					Dim returnType As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = overriddenMethod.ReturnType
					bitVector = New Microsoft.CodeAnalysis.BitVector()
					boundCall = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(root, overriddenMethod, Nothing, boundMyBaseReference, u00210s, Nothing, returnType, True, False, bitVector)
				End If
				Dim generatedLabelSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.GeneratedLabelSymbol("exit")
				Dim instance As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
				If (accessor.MethodKind <> MethodKind.PropertyGet) Then
					Dim item As ParameterSymbol = accessor.Parameters(accessor.ParameterCount - 1)
					Dim boundParameter As Microsoft.CodeAnalysis.VisualBasic.BoundParameter = New Microsoft.CodeAnalysis.VisualBasic.BoundParameter(root, item, False, item.Type)
					Dim valueTuples As ArrayBuilder(Of ValueTuple(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)) = Nothing
					Dim instance1 As ArrayBuilder(Of LocalSymbol) = Nothing
					Dim boundLocals As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundLocal) = Nothing
					If (associatedSymbol.IsWithEvents) Then
						Dim enumerator As ImmutableArray(Of Symbol).Enumerator = accessor.ContainingType.GetMembers().GetEnumerator()
						While enumerator.MoveNext()
							Dim current As Symbol = enumerator.Current
							If (current.Kind <> SymbolKind.Method) Then
								Continue While
							End If
							Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol)
							Dim handledEvents As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.HandledEvent) = methodSymbol.HandledEvents
							If (methodSymbol.IsPartial()) Then
								Dim partialImplementationPart As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = methodSymbol.PartialImplementationPart
								If (partialImplementationPart Is Nothing) Then
									Continue While
								End If
								handledEvents = ImmutableArrayExtensions.Concat(Of Microsoft.CodeAnalysis.VisualBasic.HandledEvent)(handledEvents, partialImplementationPart.HandledEvents)
							End If
							If (handledEvents.IsEmpty) Then
								Continue While
							End If
							Dim enumerator1 As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.HandledEvent).Enumerator = handledEvents.GetEnumerator()
							While enumerator1.MoveNext()
								Dim handledEvent As Microsoft.CodeAnalysis.VisualBasic.HandledEvent = enumerator1.Current
								If (handledEvent.hookupMethod <> accessor) Then
									Continue While
								End If
								If (valueTuples Is Nothing) Then
									valueTuples = ArrayBuilder(Of ValueTuple(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)).GetInstance()
									instance1 = ArrayBuilder(Of LocalSymbol).GetInstance()
									boundLocals = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.BoundLocal).GetInstance()
								End If
								valueTuples.Add(New ValueTuple(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)(DirectCast(handledEvent.EventSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol), DirectCast(handledEvent.WithEventsSourceProperty, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)))
								Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(accessor, handledEvent.delegateCreation.Type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
								instance1.Add(synthesizedLocal)
								Dim boundLocal1 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(root, synthesizedLocal, synthesizedLocal.Type)
								boundLocals.Add(boundLocal1.MakeRValue())
								Dim boundExpressionStatement As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement(root, New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(root, boundLocal1, handledEvent.delegateCreation, False, boundLocal1.Type, False), False)
								instance.Add(boundExpressionStatement)
							End While
						End While
					End If
					Dim boundLocal2 As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Nothing
					Dim boundExpressionStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement = Nothing
					If (valueTuples IsNot Nothing) Then
						If (Not flag) Then
							boundExpression1 = boundFieldAccess.MakeRValue()
						Else
							boundExpression1 = boundCall
						End If
						Dim synthesizedLocal1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(accessor, boundExpression1.Type, SynthesizedLocalKind.LoweringTemp, Nothing, False)
						instance1.Add(synthesizedLocal1)
						boundLocal2 = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(root, synthesizedLocal1, synthesizedLocal1.Type)
						boundExpressionStatement1 = New Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement(root, New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(root, boundLocal2, boundExpression1, True, synthesizedLocal1.Type, False), False)
						instance.Add(boundExpressionStatement1)
						Dim boundStatements As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
						Dim count As Integer = valueTuples.Count - 1
						Dim num As Integer = 0
						Do
							Dim item1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol = valueTuples(num).Item1
							Dim boundPropertyAccess As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundLocal2
							Dim item2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = valueTuples(num).Item2
							If (item2 IsNot Nothing) Then
								Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = root
								Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = item2
								If (item2.IsShared) Then
									boundExpression3 = Nothing
								Else
									boundExpression3 = boundLocal2
								End If
								Dim empty1 As ImmutableArray(Of !0) = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty
								bitVector = New Microsoft.CodeAnalysis.BitVector()
								boundPropertyAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess(visualBasicSyntaxNode, propertySymbol, Nothing, PropertyAccessKind.[Get], False, boundExpression3, empty1, bitVector, False)
							End If
							boundStatements.Add(New BoundRemoveHandlerStatement(root, New BoundEventAccess(root, boundPropertyAccess, item1, item1.Type, False), boundLocals(num), False))
							num = num + 1
						Loop While num <= count
						Dim boundStatementList As Microsoft.CodeAnalysis.VisualBasic.BoundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(root, boundStatements.ToImmutableAndFree(), False)
						Dim boundIfStatement As Microsoft.CodeAnalysis.VisualBasic.BoundIfStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundIfStatement(root, (New BoundBinaryOperator(root, BinaryOperatorKind.[IsNot], boundLocal2.MakeRValue(), New BoundLiteral(root, ConstantValue.[Nothing], accessor.ContainingAssembly.GetSpecialType(SpecialType.System_Object)), False, accessor.ContainingAssembly.GetSpecialType(SpecialType.System_Boolean), False)).MakeCompilerGenerated(), boundStatementList, Nothing, False)
						instance.Add(boundIfStatement.MakeCompilerGenerated())
					End If
					If (Not flag) Then
						boundAssignmentOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundAssignmentOperator(root, boundFieldAccess, boundParameter, False, associatedSymbol.Type, False)
					Else
						Dim overriddenMethod1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = accessor.OverriddenMethod
						Dim boundExpressions As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = ImmutableArray.Create(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundParameter)
						Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = overriddenMethod1.ReturnType
						bitVector = New Microsoft.CodeAnalysis.BitVector()
						boundAssignmentOperator = New Microsoft.CodeAnalysis.VisualBasic.BoundCall(root, overriddenMethod1, Nothing, boundMyBaseReference, boundExpressions, Nothing, typeSymbol, True, False, bitVector)
					End If
					instance.Add((New Microsoft.CodeAnalysis.VisualBasic.BoundExpressionStatement(root, boundAssignmentOperator, False)).MakeCompilerGenerated())
					If (valueTuples IsNot Nothing) Then
						instance.Add(boundExpressionStatement1)
						Dim boundStatements1 As ArrayBuilder(Of BoundStatement) = ArrayBuilder(Of BoundStatement).GetInstance()
						Dim count1 As Integer = valueTuples.Count - 1
						Dim num1 As Integer = 0
						Do
							Dim eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol = valueTuples(num1).Item1
							Dim boundPropertyAccess1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = boundLocal2
							Dim item21 As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = valueTuples(num1).Item2
							If (item21 IsNot Nothing) Then
								Dim visualBasicSyntaxNode1 As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = root
								Dim propertySymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = item21
								If (item21.IsShared) Then
									boundExpression2 = Nothing
								Else
									boundExpression2 = boundLocal2
								End If
								Dim u00210s1 As ImmutableArray(Of !0) = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty
								bitVector = New Microsoft.CodeAnalysis.BitVector()
								boundPropertyAccess1 = New Microsoft.CodeAnalysis.VisualBasic.BoundPropertyAccess(visualBasicSyntaxNode1, propertySymbol1, Nothing, PropertyAccessKind.[Get], False, boundExpression2, u00210s1, bitVector, False)
							End If
							boundStatements1.Add(New BoundAddHandlerStatement(root, New BoundEventAccess(root, boundPropertyAccess1, eventSymbol, eventSymbol.Type, False), boundLocals(num1), False))
							num1 = num1 + 1
						Loop While num1 <= count1
						Dim boundStatementList1 As Microsoft.CodeAnalysis.VisualBasic.BoundStatementList = New Microsoft.CodeAnalysis.VisualBasic.BoundStatementList(root, boundStatements1.ToImmutableAndFree(), False)
						Dim boundIfStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundIfStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundIfStatement(root, (New BoundBinaryOperator(root, BinaryOperatorKind.[IsNot], boundLocal2.MakeRValue(), New BoundLiteral(root, ConstantValue.[Nothing], accessor.ContainingAssembly.GetSpecialType(SpecialType.System_Object)), False, accessor.ContainingAssembly.GetSpecialType(SpecialType.System_Boolean), False)).MakeCompilerGenerated(), boundStatementList1, Nothing, False)
						instance.Add(boundIfStatement1.MakeCompilerGenerated())
					End If
					If (instance1 Is Nothing) Then
						empty = ImmutableArray(Of LocalSymbol).Empty
					Else
						empty = instance1.ToImmutableAndFree()
					End If
					localSymbols = empty
					boundLocal = Nothing
					If (valueTuples IsNot Nothing) Then
						valueTuples.Free()
						boundLocals.Free()
					End If
				Else
					Dim synthesizedLocal2 As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(accessor, accessor.ReturnType, SynthesizedLocalKind.LoweringTemp, Nothing, False)
					If (Not flag) Then
						boundExpression = boundFieldAccess.MakeRValue()
					Else
						boundExpression = boundCall
					End If
					instance.Add((New BoundReturnStatement(root, boundExpression, synthesizedLocal2, generatedLabelSymbol, False)).MakeCompilerGenerated())
					localSymbols = ImmutableArray.Create(Of LocalSymbol)(synthesizedLocal2)
					boundLocal = New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(root, synthesizedLocal2, False, synthesizedLocal2.Type)
				End If
				instance.Add((New BoundLabelStatement(root, generatedLabelSymbol)).MakeCompilerGenerated())
				instance.Add((New BoundReturnStatement(root, boundLocal, Nothing, Nothing, False)).MakeCompilerGenerated())
				statementSyntaxes = New SyntaxList(Of StatementSyntax)()
				boundBlock = (New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(root, statementSyntaxes, localSymbols, instance.ToImmutableAndFree(), False)).MakeCompilerGenerated()
			Else
				statementSyntaxes = New SyntaxList(Of StatementSyntax)()
				boundBlock = (New Microsoft.CodeAnalysis.VisualBasic.BoundBlock(root, statementSyntaxes, ImmutableArray(Of LocalSymbol).Empty, ImmutableArray(Of BoundStatement).Empty, True)).MakeCompilerGenerated()
			End If
			Return boundBlock
		End Function
	End Module
End Namespace