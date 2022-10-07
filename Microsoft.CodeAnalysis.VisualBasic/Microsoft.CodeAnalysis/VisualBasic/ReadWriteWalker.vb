Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Linq

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class ReadWriteWalker
		Inherits AbstractRegionDataFlowPass
		Private ReadOnly _readInside As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)

		Private ReadOnly _writtenInside As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)

		Private ReadOnly _readOutside As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)

		Private ReadOnly _writtenOutside As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)

		Private ReadOnly _captured As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)

		Private ReadOnly _capturedInside As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)

		Private ReadOnly _capturedOutside As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)

		Private _currentMethodOrLambda As Microsoft.CodeAnalysis.VisualBasic.Symbol

		Private _currentQueryLambda As BoundQueryLambda

		Private Sub New(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo)
			MyBase.New(info, region, Nothing, False, False)
			Me._readInside = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
			Me._writtenInside = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
			Me._readOutside = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
			Me._writtenOutside = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
			Me._captured = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
			Me._capturedInside = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
			Me._capturedOutside = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
			Me._currentMethodOrLambda = Me.symbol
		End Sub

		Friend Shared Sub Analyze(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo, ByRef readInside As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), ByRef writtenInside As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), ByRef readOutside As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), ByRef writtenOutside As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), ByRef captured As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), ByRef capturedInside As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), ByRef capturedOutside As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbol))
			Dim readWriteWalker As Microsoft.CodeAnalysis.VisualBasic.ReadWriteWalker = New Microsoft.CodeAnalysis.VisualBasic.ReadWriteWalker(info, region)
			Try
				If (Not readWriteWalker.Analyze()) Then
					readInside = Enumerable.Empty(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
					writtenInside = readInside
					readOutside = readInside
					writtenOutside = readInside
					captured = readInside
					capturedInside = readInside
					capturedOutside = readInside
				Else
					readInside = readWriteWalker._readInside
					writtenInside = readWriteWalker._writtenInside
					readOutside = readWriteWalker._readOutside
					writtenOutside = readWriteWalker._writtenOutside
					captured = readWriteWalker._captured
					capturedInside = readWriteWalker._capturedInside
					capturedOutside = readWriteWalker._capturedOutside
				End If
			Finally
				readWriteWalker.Free()
			End Try
		End Sub

		Private Shadows Function Analyze() As Boolean
			Return Me.Scan()
		End Function

		Private Sub CheckCaptured(ByVal variable As Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim kind As SymbolKind = variable.Kind
			If (kind = SymbolKind.Local) Then
				Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = DirectCast(variable, Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol)
				If (Not localSymbol.IsConst AndAlso Me._currentMethodOrLambda <> localSymbol.ContainingSymbol) Then
					Me.NoteCaptured(localSymbol)
					Return
				End If
			ElseIf (kind = SymbolKind.Parameter) Then
				Dim parameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol = DirectCast(variable, Microsoft.CodeAnalysis.VisualBasic.Symbols.ParameterSymbol)
				If (Me._currentMethodOrLambda <> parameterSymbol.ContainingSymbol) Then
					Me.NoteCaptured(parameterSymbol)
					Return
				End If
			Else
				If (kind <> SymbolKind.RangeVariable) Then
					Return
				End If
				Dim rangeVariableSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.RangeVariableSymbol = DirectCast(variable, Microsoft.CodeAnalysis.VisualBasic.Symbols.RangeVariableSymbol)
				If (Me._currentMethodOrLambda <> rangeVariableSymbol.ContainingSymbol AndAlso Me._currentQueryLambda IsNot Nothing AndAlso (Me._currentMethodOrLambda <> Me._currentQueryLambda.LambdaSymbol OrElse Not Me._currentQueryLambda.RangeVariables.Contains(rangeVariableSymbol))) Then
					Me.NoteCaptured(rangeVariableSymbol)
				End If
			End If
		End Sub

		Private Shared Function IsCompilerGeneratedTempLocal(ByVal variable As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
			Return TypeOf variable Is SynthesizedLocal
		End Function

		Private Sub NoteCaptured(ByVal variable As Microsoft.CodeAnalysis.VisualBasic.Symbol)
			If (Me._regionPlace = AbstractFlowPass(Of LocalState).RegionPlace.Inside) Then
				Me._capturedInside.Add(variable)
				Me._captured.Add(variable)
				Return
			End If
			If (variable.Kind <> SymbolKind.RangeVariable) Then
				Me._capturedOutside.Add(variable)
				Me._captured.Add(variable)
			End If
		End Sub

		Protected Overrides Sub NoteRead(ByVal variable As Microsoft.CodeAnalysis.VisualBasic.Symbol)
			If (ReadWriteWalker.IsCompilerGeneratedTempLocal(variable)) Then
				MyBase.NoteRead(variable)
				Return
			End If
			Select Case Me._regionPlace
				Case DirectCast(AbstractFlowPass(Of LocalState).RegionPlace.Before, AbstractFlowPass(Of LocalState).RegionPlace)
				Case DirectCast(AbstractFlowPass(Of LocalState).RegionPlace.After, AbstractFlowPass(Of LocalState).RegionPlace)
					Me._readOutside.Add(variable)
					Exit Select
				Case DirectCast(AbstractFlowPass(Of LocalState).RegionPlace.Inside, AbstractFlowPass(Of LocalState).RegionPlace)
					Me._readInside.Add(variable)
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(DirectCast(Me._regionPlace, AbstractFlowPass(Of DataFlowPass.LocalState).RegionPlace))
			End Select
			MyBase.NoteRead(variable)
			Me.CheckCaptured(variable)
		End Sub

		Protected Overrides Sub NoteRead(ByVal fieldAccess As BoundFieldAccess)
			MyBase.NoteRead(fieldAccess)
			If (Me._regionPlace <> AbstractFlowPass(Of LocalState).RegionPlace.Inside AndAlso fieldAccess.Syntax.Span.Contains(Me._region)) Then
				Me.NoteReceiverRead(fieldAccess)
			End If
		End Sub

		Private Sub NoteReceiverRead(ByVal fieldAccess As BoundFieldAccess)
			Me.NoteReceiverReadOrWritten(fieldAccess, Me._readInside)
		End Sub

		Private Sub NoteReceiverReadOrWritten(ByVal fieldAccess As BoundFieldAccess, ByVal readOrWritten As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol))
			If (Not fieldAccess.FieldSymbol.IsShared AndAlso Not fieldAccess.FieldSymbol.ContainingType.IsReferenceType) Then
				Dim receiverOpt As BoundExpression = fieldAccess.ReceiverOpt
				If (receiverOpt IsNot Nothing) Then
					Dim syntax As SyntaxNode = receiverOpt.Syntax
					If (syntax IsNot Nothing) Then
						Dim kind As BoundKind = receiverOpt.Kind
						If (kind <= BoundKind.MyBaseReference) Then
							If (kind = BoundKind.FieldAccess) Then
								If (receiverOpt.Type.IsStructureType() AndAlso syntax.Span.OverlapsWith(Me._region)) Then
									Me.NoteReceiverReadOrWritten(DirectCast(receiverOpt, BoundFieldAccess), readOrWritten)
								End If
							ElseIf (kind <> BoundKind.MeReference) Then
								If (kind <> BoundKind.MyBaseReference) Then
									Return
								End If
								If (Me._region.Contains(syntax.Span)) Then
									readOrWritten.Add(Me.MeParameter)
									Return
								End If
							ElseIf (Me._region.Contains(syntax.Span)) Then
								readOrWritten.Add(Me.MeParameter)
								Return
							End If
						ElseIf (kind = BoundKind.Local) Then
							If (Me._region.Contains(syntax.Span)) Then
								readOrWritten.Add(DirectCast(receiverOpt, BoundLocal).LocalSymbol)
								Return
							End If
						ElseIf (kind <> BoundKind.Parameter) Then
							If (kind <> BoundKind.RangeVariable) Then
								Return
							End If
							If (Me._region.Contains(syntax.Span)) Then
								readOrWritten.Add(DirectCast(receiverOpt, BoundRangeVariable).RangeVariable)
								Return
							End If
						ElseIf (Me._region.Contains(syntax.Span)) Then
							readOrWritten.Add(DirectCast(receiverOpt, BoundParameter).ParameterSymbol)
							Return
						End If
					End If
				End If
			End If
		End Sub

		Private Sub NoteReceiverWritten(ByVal fieldAccess As BoundFieldAccess)
			Me.NoteReceiverReadOrWritten(fieldAccess, Me._writtenInside)
		End Sub

		Protected Overrides Sub NoteWrite(ByVal variable As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal value As BoundExpression)
			If (ReadWriteWalker.IsCompilerGeneratedTempLocal(variable)) Then
				MyBase.NoteWrite(variable, value)
				Return
			End If
			Select Case Me._regionPlace
				Case DirectCast(AbstractFlowPass(Of LocalState).RegionPlace.Before, AbstractFlowPass(Of LocalState).RegionPlace)
				Case DirectCast(AbstractFlowPass(Of LocalState).RegionPlace.After, AbstractFlowPass(Of LocalState).RegionPlace)
					Me._writtenOutside.Add(variable)
					Exit Select
				Case DirectCast(AbstractFlowPass(Of LocalState).RegionPlace.Inside, AbstractFlowPass(Of LocalState).RegionPlace)
					Me._writtenInside.Add(variable)
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(DirectCast(Me._regionPlace, AbstractFlowPass(Of DataFlowPass.LocalState).RegionPlace))
			End Select
			MyBase.NoteWrite(variable, value)
			Me.CheckCaptured(variable)
		End Sub

		Protected Overrides Sub NoteWrite(ByVal node As BoundExpression, ByVal value As BoundExpression)
			MyBase.NoteWrite(node, value)
			If (node.Kind = BoundKind.FieldAccess) Then
				Me.NoteReceiverWritten(DirectCast(node, BoundFieldAccess))
			End If
		End Sub

		Public Overrides Function VisitLambda(ByVal node As BoundLambda) As BoundNode
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me._currentMethodOrLambda
			Me._currentMethodOrLambda = node.LambdaSymbol
			Me._currentMethodOrLambda = symbol
			Return MyBase.VisitLambda(node)
		End Function

		Public Overrides Function VisitQueryableSource(ByVal node As BoundQueryableSource) As BoundNode
			If (Not node.WasCompilerGenerated AndAlso node.RangeVariableOpt IsNot Nothing) Then
				Me.NoteWrite(node.RangeVariableOpt, Nothing)
			End If
			MyBase.VisitRvalue(node.Source, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitQueryLambda(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundQueryLambda) As BoundNode
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me._currentMethodOrLambda
			Me._currentMethodOrLambda = node.LambdaSymbol
			Dim boundQueryLambda As Microsoft.CodeAnalysis.VisualBasic.BoundQueryLambda = Me._currentQueryLambda
			Me._currentQueryLambda = node
			Me._currentMethodOrLambda = symbol
			Me._currentQueryLambda = boundQueryLambda
			Return MyBase.VisitQueryLambda(node)
		End Function

		Public Overrides Function VisitRangeVariableAssignment(ByVal node As BoundRangeVariableAssignment) As BoundNode
			If (Not node.WasCompilerGenerated) Then
				Me.NoteWrite(node.RangeVariable, Nothing)
			End If
			MyBase.VisitRvalue(node.Value, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function
	End Class
End Namespace