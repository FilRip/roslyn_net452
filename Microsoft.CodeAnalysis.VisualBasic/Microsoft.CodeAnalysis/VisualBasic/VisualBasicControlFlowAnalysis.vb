Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class VisualBasicControlFlowAnalysis
		Inherits ControlFlowAnalysis
		Private ReadOnly _context As RegionAnalysisContext

		Private _entryPoints As ImmutableArray(Of SyntaxNode)

		Private _exitPoints As ImmutableArray(Of SyntaxNode)

		Private _regionStartPointIsReachable As Object

		Private _regionEndPointIsReachable As Object

		Private _returnStatements As ImmutableArray(Of SyntaxNode)

		Private _succeeded As Nullable(Of Boolean)

		Public NotOverridable Overrides ReadOnly Property EndPointIsReachable As Boolean
			Get
				If (Me._regionStartPointIsReachable Is Nothing) Then
					Me.ComputeReachability()
				End If
				Return CBool(Me._regionEndPointIsReachable)
			End Get
		End Property

		Public Overrides ReadOnly Property EntryPoints As ImmutableArray(Of SyntaxNode)
			Get
				Dim empty As ImmutableArray(Of SyntaxNode)
				If (Me._entryPoints.IsDefault) Then
					Me._succeeded = New Nullable(Of Boolean)(Not Me._context.Failed)
					If (Me._context.Failed) Then
						empty = ImmutableArray(Of SyntaxNode).Empty
					Else
						Dim analysisInfo As FlowAnalysisInfo = Me._context.AnalysisInfo
						Dim regionAnalysisContext As Microsoft.CodeAnalysis.VisualBasic.RegionAnalysisContext = Me._context
						empty = EntryPointsWalker.Analyze(analysisInfo, regionAnalysisContext.RegionInfo, Me._succeeded).ToImmutableArray()
					End If
					Dim syntaxNodes As ImmutableArray(Of SyntaxNode) = empty
					Dim syntaxNodes1 As ImmutableArray(Of SyntaxNode) = New ImmutableArray(Of SyntaxNode)()
					ImmutableInterlocked.InterlockedCompareExchange(Of SyntaxNode)(Me._entryPoints, syntaxNodes, syntaxNodes1)
				End If
				Return Me._entryPoints
			End Get
		End Property

		Public Overrides ReadOnly Property ExitPoints As ImmutableArray(Of SyntaxNode)
			Get
				Dim empty As ImmutableArray(Of SyntaxNode)
				If (Me._exitPoints.IsDefault) Then
					If (Me._context.Failed) Then
						empty = ImmutableArray(Of SyntaxNode).Empty
					Else
						Dim analysisInfo As FlowAnalysisInfo = Me._context.AnalysisInfo
						Dim regionAnalysisContext As Microsoft.CodeAnalysis.VisualBasic.RegionAnalysisContext = Me._context
						empty = ExitPointsWalker.Analyze(analysisInfo, regionAnalysisContext.RegionInfo).ToImmutableArray()
					End If
					Dim syntaxNodes As ImmutableArray(Of SyntaxNode) = empty
					Dim syntaxNodes1 As ImmutableArray(Of SyntaxNode) = New ImmutableArray(Of SyntaxNode)()
					ImmutableInterlocked.InterlockedCompareExchange(Of SyntaxNode)(Me._exitPoints, syntaxNodes, syntaxNodes1)
				End If
				Return Me._exitPoints
			End Get
		End Property

		Public Overrides ReadOnly Property ReturnStatements As ImmutableArray(Of SyntaxNode)
			Get
				Dim func As Func(Of SyntaxNode, Boolean)
				Dim exitPoints As ImmutableArray(Of SyntaxNode) = Me.ExitPoints
				If (VisualBasicControlFlowAnalysis._Closure$__.$I18-0 Is Nothing) Then
					func = Function(s As SyntaxNode) s.IsKind(SyntaxKind.ReturnStatement) Or s.IsKind(SyntaxKind.ExitSubStatement) Or s.IsKind(SyntaxKind.ExitFunctionStatement) Or s.IsKind(SyntaxKind.ExitOperatorStatement) Or s.IsKind(SyntaxKind.ExitPropertyStatement)
					VisualBasicControlFlowAnalysis._Closure$__.$I18-0 = func
				Else
					func = VisualBasicControlFlowAnalysis._Closure$__.$I18-0
				End If
				Return exitPoints.WhereAsArray(func)
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property StartPointIsReachable As Boolean
			Get
				If (Me._regionStartPointIsReachable Is Nothing) Then
					Me.ComputeReachability()
				End If
				Return CBool(Me._regionStartPointIsReachable)
			End Get
		End Property

		Public NotOverridable Overrides ReadOnly Property Succeeded As Boolean
			Get
				If (Not Me._succeeded.HasValue) Then
					Dim entryPoints As ImmutableArray(Of SyntaxNode) = Me.EntryPoints
				End If
				Return Me._succeeded.Value
			End Get
		End Property

		Friend Sub New(ByVal _context As RegionAnalysisContext)
			MyBase.New()
			Me._context = _context
		End Sub

		Private Sub ComputeReachability()
			Dim flag As Boolean = False
			Dim flag1 As Boolean = False
			If (Not Me._context.Failed) Then
				Dim analysisInfo As FlowAnalysisInfo = Me._context.AnalysisInfo
				Dim regionAnalysisContext As Microsoft.CodeAnalysis.VisualBasic.RegionAnalysisContext = Me._context
				RegionReachableWalker.Analyze(analysisInfo, regionAnalysisContext.RegionInfo, flag, flag1)
			Else
				flag = True
				flag1 = True
			End If
			Interlocked.CompareExchange(Me._regionStartPointIsReachable, flag, Nothing)
			Interlocked.CompareExchange(Me._regionEndPointIsReachable, flag1, Nothing)
		End Sub
	End Class
End Namespace