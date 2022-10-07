Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Collections
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class IteratorAndAsyncCaptureWalker
		Inherits DataFlowPass
		Private ReadOnly _variablesToHoist As OrderedSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)

		Private ReadOnly _byRefLocalsInitializers As Dictionary(Of LocalSymbol, BoundExpression)

		Private _lazyDisallowedCaptures As MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, SyntaxNode)

		Protected Overrides ReadOnly Property EnableBreakingFlowAnalysisFeatures As Boolean
			Get
				Return True
			End Get
		End Property

		Protected Overrides ReadOnly Property IgnoreOutSemantics As Boolean
			Get
				Return False
			End Get
		End Property

		Protected Overrides ReadOnly Property ProcessCompilerGeneratedLocals As Boolean
			Get
				Return True
			End Get
		End Property

		Public Sub New(ByVal info As FlowAnalysisInfo)
			MyBase.New(info, New FlowAnalysisRegionInfo(), False, Nothing, True, True)
			Me._variablesToHoist = New OrderedSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
			Me._byRefLocalsInitializers = New Dictionary(Of LocalSymbol, BoundExpression)()
		End Sub

		Public Shared Function Analyze(ByVal info As FlowAnalysisInfo, ByVal diagnostics As DiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.IteratorAndAsyncCaptureWalker.Result
			Dim enumerator As Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, Microsoft.CodeAnalysis.SyntaxNode).ValueSet).KeyCollection.Enumerator = New Dictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, Microsoft.CodeAnalysis.SyntaxNode).ValueSet).KeyCollection.Enumerator()
			Dim enumerator1 As MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, Microsoft.CodeAnalysis.SyntaxNode).ValueSet.Enumerator = New MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, Microsoft.CodeAnalysis.SyntaxNode).ValueSet.Enumerator()
			Dim iteratorAndAsyncCaptureWalker As Microsoft.CodeAnalysis.VisualBasic.IteratorAndAsyncCaptureWalker = New Microsoft.CodeAnalysis.VisualBasic.IteratorAndAsyncCaptureWalker(info) With
			{
				._convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = True
			}
			iteratorAndAsyncCaptureWalker.Analyze()
			Dim symbols As OrderedSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = iteratorAndAsyncCaptureWalker._variablesToHoist
			Dim variableIdentifierArray As DataFlowPass.VariableIdentifier() = iteratorAndAsyncCaptureWalker.variableBySlot
			Dim localSymbols As Dictionary(Of LocalSymbol, BoundExpression) = iteratorAndAsyncCaptureWalker._byRefLocalsInitializers
			Dim symbols1 As MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, Microsoft.CodeAnalysis.SyntaxNode) = iteratorAndAsyncCaptureWalker._lazyDisallowedCaptures
			iteratorAndAsyncCaptureWalker.Free()
			If (symbols1 IsNot Nothing) Then
				Try
					enumerator = symbols1.Keys.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator.Current
						Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = If(current.Kind = SymbolKind.Local, TryCast(current, LocalSymbol).Type, TryCast(current, ParameterSymbol).Type)
						Try
							enumerator1 = symbols1(current).GetEnumerator()
							While enumerator1.MoveNext()
								Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = enumerator1.Current
								diagnostics.Add(ERRID.ERR_CannotLiftRestrictedTypeResumable1, syntaxNode.GetLocation(), New [Object]() { typeSymbol })
							End While
						Finally
							DirectCast(enumerator1, IDisposable).Dispose()
						End Try
					End While
				Finally
					DirectCast(enumerator, IDisposable).Dispose()
				End Try
			End If
			If (info.Compilation.Options.OptimizationLevel <> OptimizationLevel.Release) Then
				Dim isIterator As Boolean = DirectCast(info.Symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol).IsIterator
				Dim variableIdentifierArray1 As DataFlowPass.VariableIdentifier() = variableIdentifierArray
				For i As Integer = 0 To CInt(variableIdentifierArray1.Length) Step 1
					Dim variableIdentifier As DataFlowPass.VariableIdentifier = variableIdentifierArray1(i)
					If (variableIdentifier.Symbol IsNot Nothing AndAlso Microsoft.CodeAnalysis.VisualBasic.IteratorAndAsyncCaptureWalker.HoistInDebugBuild(variableIdentifier.Symbol, isIterator)) Then
						symbols.Add(variableIdentifier.Symbol)
					End If
				Next

			End If
			Return New Microsoft.CodeAnalysis.VisualBasic.IteratorAndAsyncCaptureWalker.Result(symbols, localSymbols)
		End Function

		Private Sub CaptureVariable(ByVal variable As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal syntax As SyntaxNode)
			If (If(variable.Kind = SymbolKind.Local, TryCast(variable, LocalSymbol).Type, TryCast(variable, ParameterSymbol).Type).IsRestrictedType()) Then
				If (Not TypeOf variable Is SynthesizedLocal) Then
					If (Me._lazyDisallowedCaptures Is Nothing) Then
						Me._lazyDisallowedCaptures = New MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, SyntaxNode)()
					End If
					Me._lazyDisallowedCaptures.Add(variable, syntax)
					Return
				End If
			ElseIf (Me.compilation.Options.OptimizationLevel = OptimizationLevel.Release) Then
				Me._variablesToHoist.Add(variable)
			End If
		End Sub

		Protected Overrides Sub EnterParameter(ByVal parameter As ParameterSymbol)
			MyBase.GetOrCreateSlot(parameter, 0)
			Me.CaptureVariable(parameter, Nothing)
		End Sub

		Private Shared Function HoistInDebugBuild(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal skipByRefLocals As Boolean) As Boolean
			Dim flag As Boolean
			If (symbol.Kind = SymbolKind.Parameter) Then
				flag = Not TryCast(symbol, ParameterSymbol).Type.IsRestrictedType()
			ElseIf (symbol.Kind <> SymbolKind.Local) Then
				flag = False
			Else
				Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = TryCast(symbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
				If (localSymbol.IsConst) Then
					flag = False
				ElseIf (Not skipByRefLocals OrElse Not localSymbol.IsByRef) Then
					flag = If(localSymbol.SynthesizedKind <> SynthesizedLocalKind.UserDefined, localSymbol.SynthesizedKind <> SynthesizedLocalKind.ConditionalBranchDiscriminator, Not localSymbol.Type.IsRestrictedType())
				Else
					flag = False
				End If
			End If
			Return flag
		End Function

		Protected Overrides Function IsEmptyStructType(ByVal type As TypeSymbol) As Boolean
			Return False
		End Function

		Private Sub MarkLocalsUnassigned()
			Dim num As Integer = Me.nextVariableSlot - 1
			For i As Integer = 2 To num
				Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me.variableBySlot(i).Symbol
				Dim kind As SymbolKind = symbol.Kind
				If (kind <> SymbolKind.Local) Then
					If (kind = SymbolKind.Parameter) Then
						MyBase.SetSlotState(i, False)
					End If
				ElseIf (Not DirectCast(symbol, LocalSymbol).IsConst) Then
					MyBase.SetSlotState(i, False)
				End If
			Next

		End Sub

		Protected Overrides Sub ReportUnassigned(ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal node As SyntaxNode, ByVal rwContext As AbstractFlowPass(Of DataFlowPass.LocalState).ReadWriteContext, Optional ByVal slot As Integer = -1, Optional ByVal boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = Nothing)
			If (symbol.Kind = SymbolKind.Field) Then
				Dim nodeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = MyBase.GetNodeSymbol(boundFieldAccess)
				If (nodeSymbol IsNot Nothing) Then
					Me.CaptureVariable(nodeSymbol, node)
					Return
				End If
			ElseIf (symbol.Kind = SymbolKind.Parameter OrElse symbol.Kind = SymbolKind.Local) Then
				Me.CaptureVariable(symbol, node)
			End If
		End Sub

		Protected Overrides Function Scan() As Boolean
			Me._variablesToHoist.Clear()
			Me._byRefLocalsInitializers.Clear()
			Dim symbols As MultiDictionary(Of Microsoft.CodeAnalysis.VisualBasic.Symbol, SyntaxNode) = Me._lazyDisallowedCaptures
			If (symbols IsNot Nothing) Then
				symbols.Clear()
			Else
			End If
			Return MyBase.Scan()
		End Function

		Protected Overrides Function TreatTheLocalAsAssignedWithinTheLambda(ByVal local As LocalSymbol, ByVal right As BoundExpression) As Boolean
			Dim flag As Boolean
			If (right.Kind = BoundKind.ObjectCreationExpression) Then
				Dim boundObjectCreationExpression As Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression = DirectCast(right, Microsoft.CodeAnalysis.VisualBasic.BoundObjectCreationExpression)
				If (TypeOf boundObjectCreationExpression.Type Is LambdaFrame AndAlso boundObjectCreationExpression.Arguments.Length = 1) Then
					Dim item As BoundExpression = boundObjectCreationExpression.Arguments(0)
					If (item.Kind <> BoundKind.Local OrElse CObj(DirectCast(item, BoundLocal).LocalSymbol) <> CObj(local)) Then
						flag = False
						Return flag
					End If
					flag = True
					Return flag
				End If
			End If
			flag = False
			Return flag
		End Function

		Public Overrides Function VisitAwaitOperator(ByVal node As BoundAwaitOperator) As BoundNode
			MyBase.VisitAwaitOperator(node)
			Me.MarkLocalsUnassigned()
			Return Nothing
		End Function

		Public Overrides Function VisitReferenceAssignment(ByVal node As BoundReferenceAssignment) As BoundNode
			Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = node.ByRefLocal.LocalSymbol
			Me._byRefLocalsInitializers.Add(localSymbol, node.LValue)
			Return MyBase.VisitReferenceAssignment(node)
		End Function

		Public Overrides Function VisitSequence(ByVal node As BoundSequence) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode = Nothing
			Dim enumerator As ImmutableArray(Of LocalSymbol).Enumerator = node.Locals.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As LocalSymbol = enumerator.Current
				MyBase.SetSlotState(MyBase.GetOrCreateSlot(current, 0), True)
			End While
			boundNode = MyBase.VisitSequence(node)
			Dim enumerator1 As ImmutableArray(Of LocalSymbol).Enumerator = node.Locals.GetEnumerator()
			While enumerator1.MoveNext()
				MyBase.CheckAssigned(enumerator1.Current, node.Syntax, AbstractFlowPass(Of LocalState).ReadWriteContext.None)
			End While
			Return boundNode
		End Function

		Public Overrides Function VisitYieldStatement(ByVal node As BoundYieldStatement) As BoundNode
			MyBase.VisitYieldStatement(node)
			Me.MarkLocalsUnassigned()
			Return Nothing
		End Function

		Public Structure Result
			Public ReadOnly CapturedLocals As OrderedSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)

			Public ReadOnly ByRefLocalsInitializers As Dictionary(Of LocalSymbol, BoundExpression)

			Friend Sub New(ByVal cl As OrderedSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), ByVal initializers As Dictionary(Of LocalSymbol, BoundExpression))
				Me = New IteratorAndAsyncCaptureWalker.Result() With
				{
					.CapturedLocals = cl,
					.ByRefLocalsInitializers = initializers
				}
			End Sub
		End Structure
	End Class
End Namespace