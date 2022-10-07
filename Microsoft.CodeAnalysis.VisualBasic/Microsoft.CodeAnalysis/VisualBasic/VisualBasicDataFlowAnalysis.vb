Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class VisualBasicDataFlowAnalysis
		Inherits DataFlowAnalysis
		Private ReadOnly _context As RegionAnalysisContext

		Private _variablesDeclared As ImmutableArray(Of ISymbol)

		Private _unassignedVariables As HashSet(Of Symbol)

		Private _dataFlowsIn As ImmutableArray(Of ISymbol)

		Private _definitelyAssignedOnEntry As ImmutableArray(Of ISymbol)

		Private _definitelyAssignedOnExit As ImmutableArray(Of ISymbol)

		Private _dataFlowsOut As ImmutableArray(Of ISymbol)

		Private _alwaysAssigned As ImmutableArray(Of ISymbol)

		Private _readInside As ImmutableArray(Of ISymbol)

		Private _writtenInside As ImmutableArray(Of ISymbol)

		Private _readOutside As ImmutableArray(Of ISymbol)

		Private _writtenOutside As ImmutableArray(Of ISymbol)

		Private _captured As ImmutableArray(Of ISymbol)

		Private _capturedInside As ImmutableArray(Of ISymbol)

		Private _capturedOutside As ImmutableArray(Of ISymbol)

		Private _succeeded As Nullable(Of Boolean)

		Private _invalidRegionDetected As Boolean

		Public Overrides ReadOnly Property AlwaysAssigned As ImmutableArray(Of ISymbol)
			Get
				Dim empty As ImmutableArray(Of ISymbol)
				If (Me._alwaysAssigned.IsDefault) Then
					If (Me._context.Failed) Then
						empty = ImmutableArray(Of ISymbol).Empty
					Else
						Dim analysisInfo As FlowAnalysisInfo = Me._context.AnalysisInfo
						Dim regionAnalysisContext As Microsoft.CodeAnalysis.VisualBasic.RegionAnalysisContext = Me._context
						empty = Me.Normalize(AlwaysAssignedWalker.Analyze(analysisInfo, regionAnalysisContext.RegionInfo))
					End If
					Dim symbols As ImmutableArray(Of ISymbol) = empty
					Dim symbols1 As ImmutableArray(Of ISymbol) = New ImmutableArray(Of ISymbol)()
					ImmutableInterlocked.InterlockedCompareExchange(Of ISymbol)(Me._alwaysAssigned, symbols, symbols1)
				End If
				Return Me._alwaysAssigned
			End Get
		End Property

		Public Overrides ReadOnly Property Captured As ImmutableArray(Of ISymbol)
			Get
				If (Me._captured.IsDefault) Then
					Me.AnalyzeReadWrite()
				End If
				Return Me._captured
			End Get
		End Property

		Public Overrides ReadOnly Property CapturedInside As ImmutableArray(Of ISymbol)
			Get
				If (Me._capturedInside.IsDefault) Then
					Me.AnalyzeReadWrite()
				End If
				Return Me._capturedInside
			End Get
		End Property

		Public Overrides ReadOnly Property CapturedOutside As ImmutableArray(Of ISymbol)
			Get
				If (Me._capturedOutside.IsDefault) Then
					Me.AnalyzeReadWrite()
				End If
				Return Me._capturedOutside
			End Get
		End Property

		Public Overrides ReadOnly Property DataFlowsIn As ImmutableArray(Of ISymbol)
			Get
				Dim empty As ImmutableArray(Of ISymbol)
				If (Me._dataFlowsIn.IsDefault) Then
					Me._succeeded = New Nullable(Of Boolean)(Not Me._context.Failed)
					If (Me._context.Failed) Then
						empty = ImmutableArray(Of ISymbol).Empty
					Else
						Dim analysisInfo As FlowAnalysisInfo = Me._context.AnalysisInfo
						Dim regionAnalysisContext As Microsoft.CodeAnalysis.VisualBasic.RegionAnalysisContext = Me._context
						empty = Me.Normalize(DataFlowsInWalker.Analyze(analysisInfo, regionAnalysisContext.RegionInfo, Me.UnassignedVariables, Me._succeeded, Me._invalidRegionDetected))
					End If
					Dim symbols As ImmutableArray(Of ISymbol) = empty
					Dim symbols1 As ImmutableArray(Of ISymbol) = New ImmutableArray(Of ISymbol)()
					ImmutableInterlocked.InterlockedCompareExchange(Of ISymbol)(Me._dataFlowsIn, symbols, symbols1)
				End If
				Return Me._dataFlowsIn
			End Get
		End Property

		Public Overrides ReadOnly Property DataFlowsOut As ImmutableArray(Of ISymbol)
			Get
				Dim empty As ImmutableArray(Of ISymbol)
				Dim dataFlowsIn As ImmutableArray(Of ISymbol) = Me.DataFlowsIn
				If (Me._dataFlowsOut.IsDefault) Then
					If (Me._context.Failed) Then
						empty = ImmutableArray(Of ISymbol).Empty
					Else
						Dim analysisInfo As FlowAnalysisInfo = Me._context.AnalysisInfo
						Dim regionAnalysisContext As Microsoft.CodeAnalysis.VisualBasic.RegionAnalysisContext = Me._context
						empty = Me.Normalize(DataFlowsOutWalker.Analyze(analysisInfo, regionAnalysisContext.RegionInfo, Me.UnassignedVariables, Me._dataFlowsIn))
					End If
					Dim symbols As ImmutableArray(Of ISymbol) = empty
					Dim symbols1 As ImmutableArray(Of ISymbol) = New ImmutableArray(Of ISymbol)()
					ImmutableInterlocked.InterlockedCompareExchange(Of ISymbol)(Me._dataFlowsOut, symbols, symbols1)
				End If
				Return Me._dataFlowsOut
			End Get
		End Property

		Public Overrides ReadOnly Property DefinitelyAssignedOnEntry As ImmutableArray(Of ISymbol)
			Get
				Return Me.ComputeDefinitelyAssignedValues().Item1
			End Get
		End Property

		Public Overrides ReadOnly Property DefinitelyAssignedOnExit As ImmutableArray(Of ISymbol)
			Get
				Return Me.ComputeDefinitelyAssignedValues().Item2
			End Get
		End Property

		Friend ReadOnly Property InvalidRegionDetectedInternal As Boolean
			Get
				If (Me.Succeeded) Then
					Return False
				End If
				Return Me._invalidRegionDetected
			End Get
		End Property

		Public Overrides ReadOnly Property ReadInside As ImmutableArray(Of ISymbol)
			Get
				If (Me._readInside.IsDefault) Then
					Me.AnalyzeReadWrite()
				End If
				Return Me._readInside
			End Get
		End Property

		Public Overrides ReadOnly Property ReadOutside As ImmutableArray(Of ISymbol)
			Get
				If (Me._readOutside.IsDefault) Then
					Me.AnalyzeReadWrite()
				End If
				Return Me._readOutside
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Succeeded As Boolean
			Get
				If (Not Me._succeeded.HasValue) Then
					Dim dataFlowsIn As ImmutableArray(Of ISymbol) = Me.DataFlowsIn
				End If
				Return Me._succeeded.Value
			End Get
		End Property

		Private ReadOnly Property UnassignedVariables As HashSet(Of Symbol)
			Get
				Dim symbols As HashSet(Of Symbol)
				If (Me._unassignedVariables Is Nothing) Then
					symbols = If(Me._context.Failed, New HashSet(Of Symbol)(), UnassignedVariablesWalker.Analyze(Me._context.AnalysisInfo))
					Interlocked.CompareExchange(Of HashSet(Of Symbol))(Me._unassignedVariables, symbols, Nothing)
				End If
				Return Me._unassignedVariables
			End Get
		End Property

		Public Overrides ReadOnly Property UnsafeAddressTaken As ImmutableArray(Of ISymbol)
			Get
				Return ImmutableArray(Of ISymbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property UsedLocalFunctions As ImmutableArray(Of IMethodSymbol)
			Get
				Return ImmutableArray(Of IMethodSymbol).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property VariablesDeclared As ImmutableArray(Of ISymbol)
			Get
				Dim empty As ImmutableArray(Of ISymbol)
				If (Me._variablesDeclared.IsDefault) Then
					If (Me._context.Failed) Then
						empty = ImmutableArray(Of ISymbol).Empty
					Else
						Dim analysisInfo As FlowAnalysisInfo = Me._context.AnalysisInfo
						Dim regionAnalysisContext As Microsoft.CodeAnalysis.VisualBasic.RegionAnalysisContext = Me._context
						empty = Me.Normalize(VariablesDeclaredWalker.Analyze(analysisInfo, regionAnalysisContext.RegionInfo))
					End If
					Dim symbols As ImmutableArray(Of ISymbol) = empty
					Dim symbols1 As ImmutableArray(Of ISymbol) = New ImmutableArray(Of ISymbol)()
					ImmutableInterlocked.InterlockedCompareExchange(Of ISymbol)(Me._variablesDeclared, symbols, symbols1)
				End If
				Return Me._variablesDeclared
			End Get
		End Property

		Public Overrides ReadOnly Property WrittenInside As ImmutableArray(Of ISymbol)
			Get
				If (Me._writtenInside.IsDefault) Then
					Me.AnalyzeReadWrite()
				End If
				Return Me._writtenInside
			End Get
		End Property

		Public Overrides ReadOnly Property WrittenOutside As ImmutableArray(Of ISymbol)
			Get
				If (Me._writtenOutside.IsDefault) Then
					Me.AnalyzeReadWrite()
				End If
				Return Me._writtenOutside
			End Get
		End Property

		Friend Sub New(ByVal _context As RegionAnalysisContext)
			MyBase.New()
			Me._context = _context
		End Sub

		Private Sub AnalyzeReadWrite()
			Dim symbols As IEnumerable(Of Symbol) = Nothing
			Dim symbols1 As IEnumerable(Of Symbol) = Nothing
			Dim symbols2 As IEnumerable(Of Symbol) = Nothing
			Dim symbols3 As IEnumerable(Of Symbol) = Nothing
			Dim symbols4 As IEnumerable(Of Symbol) = Nothing
			Dim symbols5 As IEnumerable(Of Symbol) = Nothing
			Dim symbols6 As IEnumerable(Of Symbol) = Nothing
			If (Me.Succeeded) Then
				Dim analysisInfo As FlowAnalysisInfo = Me._context.AnalysisInfo
				Dim regionAnalysisContext As Microsoft.CodeAnalysis.VisualBasic.RegionAnalysisContext = Me._context
				ReadWriteWalker.Analyze(analysisInfo, regionAnalysisContext.RegionInfo, symbols, symbols1, symbols2, symbols3, symbols4, symbols5, symbols6)
			Else
				symbols = Enumerable.Empty(Of Symbol)()
				symbols1 = symbols
				symbols2 = symbols
				symbols3 = symbols
				symbols4 = symbols
			End If
			Dim symbols7 As ImmutableArray(Of ISymbol) = Me.Normalize(symbols)
			Dim symbols8 As ImmutableArray(Of ISymbol) = New ImmutableArray(Of ISymbol)()
			ImmutableInterlocked.InterlockedCompareExchange(Of ISymbol)(Me._readInside, symbols7, symbols8)
			Dim symbols9 As ImmutableArray(Of ISymbol) = Me.Normalize(symbols1)
			symbols8 = New ImmutableArray(Of ISymbol)()
			ImmutableInterlocked.InterlockedCompareExchange(Of ISymbol)(Me._writtenInside, symbols9, symbols8)
			Dim symbols10 As ImmutableArray(Of ISymbol) = Me.Normalize(symbols2)
			symbols8 = New ImmutableArray(Of ISymbol)()
			ImmutableInterlocked.InterlockedCompareExchange(Of ISymbol)(Me._readOutside, symbols10, symbols8)
			Dim symbols11 As ImmutableArray(Of ISymbol) = Me.Normalize(symbols3)
			symbols8 = New ImmutableArray(Of ISymbol)()
			ImmutableInterlocked.InterlockedCompareExchange(Of ISymbol)(Me._writtenOutside, symbols11, symbols8)
			Dim symbols12 As ImmutableArray(Of ISymbol) = Me.Normalize(symbols4)
			symbols8 = New ImmutableArray(Of ISymbol)()
			ImmutableInterlocked.InterlockedCompareExchange(Of ISymbol)(Me._captured, symbols12, symbols8)
			Dim symbols13 As ImmutableArray(Of ISymbol) = Me.Normalize(symbols5)
			symbols8 = New ImmutableArray(Of ISymbol)()
			ImmutableInterlocked.InterlockedCompareExchange(Of ISymbol)(Me._capturedInside, symbols13, symbols8)
			Dim symbols14 As ImmutableArray(Of ISymbol) = Me.Normalize(symbols6)
			symbols8 = New ImmutableArray(Of ISymbol)()
			ImmutableInterlocked.InterlockedCompareExchange(Of ISymbol)(Me._capturedOutside, symbols14, symbols8)
		End Sub

		Private Function ComputeDefinitelyAssignedValues() As <TupleElementNames(New String() { "onEntry", "onExit" })> ValueTuple(Of ImmutableArray(Of ISymbol), ImmutableArray(Of ISymbol))
			If (Me._definitelyAssignedOnExit.IsDefault) Then
				Dim empty As ImmutableArray(Of ISymbol) = ImmutableArray(Of ISymbol).Empty
				Dim symbols As ImmutableArray(Of ISymbol) = ImmutableArray(Of ISymbol).Empty
				Dim dataFlowsIn As ImmutableArray(Of ISymbol) = Me.DataFlowsIn
				If (Not Me._context.Failed) Then
					Dim valueTuple As ValueTuple(Of HashSet(Of Symbol), HashSet(Of Symbol)) = DefinitelyAssignedWalker.Analyze(Me._context.AnalysisInfo, Me._context.RegionInfo)
					empty = Me.Normalize(valueTuple.Item1)
					symbols = Me.Normalize(valueTuple.Item2)
				End If
				ImmutableInterlocked.InterlockedInitialize(Of ISymbol)(Me._definitelyAssignedOnEntry, empty)
				ImmutableInterlocked.InterlockedInitialize(Of ISymbol)(Me._definitelyAssignedOnExit, symbols)
			End If
			Return New ValueTuple(Of ImmutableArray(Of ISymbol), ImmutableArray(Of ISymbol))(Me._definitelyAssignedOnEntry, Me._definitelyAssignedOnExit)
		End Function

		Friend Function Normalize(ByVal data As IEnumerable(Of Symbol)) As ImmutableArray(Of ISymbol)
			Dim canBeReferencedByName As Func(Of Symbol, Boolean)
			Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
			Dim symbols As IEnumerable(Of Symbol) = data
			If (VisualBasicDataFlowAnalysis._Closure$__.$I56-0 Is Nothing) Then
				canBeReferencedByName = Function(s As Symbol) s.CanBeReferencedByName
				VisualBasicDataFlowAnalysis._Closure$__.$I56-0 = canBeReferencedByName
			Else
				canBeReferencedByName = VisualBasicDataFlowAnalysis._Closure$__.$I56-0
			End If
			instance.AddRange(symbols.Where(canBeReferencedByName))
			instance.Sort(LexicalOrderSymbolComparer.Instance)
			Return instance.ToImmutableAndFree().[As](Of ISymbol)()
		End Function
	End Class
End Namespace