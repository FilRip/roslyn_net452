Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class WithExpressionRewriter
		Private ReadOnly _withSyntax As WithStatementSyntax

		Friend Sub New(ByVal withSyntax As WithStatementSyntax)
			MyBase.New()
			Me._withSyntax = withSyntax
		End Sub

		Public Function AnalyzeWithExpression(ByVal containingMember As Symbol, ByVal value As BoundExpression, ByVal doNotUseByRefLocal As Boolean, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, Optional ByVal preserveIdentityOfLValues As Boolean = False) As WithExpressionRewriter.Result
			Dim state As WithExpressionRewriter.State = New WithExpressionRewriter.State(containingMember, doNotUseByRefLocal, binder, preserveIdentityOfLValues)
			Return state.CreateResult(Me.CaptureWithExpression(value, state))
		End Function

		Private Function CaptureArrayAccess(ByVal value As BoundArrayAccess, ByVal state As WithExpressionRewriter.State) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.CaptureInATemp(value.Expression, state).MakeRValue()
			Dim length As Integer = value.Indices.Length
			Dim boundExpressionArray(length - 1 + 1 - 1) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim num As Integer = length - 1
			Dim num1 As Integer = 0
			Do
				Dim indices As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression) = value.Indices
				boundExpressionArray(num1) = Me.CaptureRValue(indices(num1), state)
				num1 = num1 + 1
			Loop While num1 <= num
			Return value.Update(boundExpression, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)(boundExpressionArray), value.IsLValue, value.Type)
		End Function

		Private Function CaptureExpression(ByVal value As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal state As WithExpressionRewriter.State) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (value.IsLValue) Then
				Dim kind As BoundKind = value.Kind
				If (kind <= BoundKind.ArrayAccess) Then
					If (kind = BoundKind.WithLValueExpressionPlaceholder) Then
						Dim withStatementPlaceholderSubstitute As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = state.Binder.GetWithStatementPlaceholderSubstitute(DirectCast(value, BoundValuePlaceholderBase))
						boundExpression = If(withStatementPlaceholderSubstitute IsNot Nothing, Me.CaptureExpression(withStatementPlaceholderSubstitute, state), value)
						Return boundExpression
					Else
						If (kind <> BoundKind.ArrayAccess) Then
							GoTo Label3
						End If
						boundExpression = Me.CaptureArrayAccess(DirectCast(value, BoundArrayAccess), state)
						Return boundExpression
					End If
				ElseIf (kind = BoundKind.FieldAccess) Then
					boundExpression = Me.CaptureFieldAccess(DirectCast(value, BoundFieldAccess), state)
					Return boundExpression
				Else
					If (kind <> BoundKind.Local AndAlso kind <> BoundKind.Parameter) Then
						GoTo Label3
					End If
					boundExpression = value
					Return boundExpression
				End If
			Label3:
				boundExpression = Me.CaptureRValue(value, state)
			Else
				boundExpression = Me.CaptureRValue(value, state)
			End If
			Return boundExpression
		End Function

		Private Function CaptureFieldAccess(ByVal value As BoundFieldAccess, ByVal state As WithExpressionRewriter.State) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim fieldSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.FieldSymbol = value.FieldSymbol
			If (fieldSymbol.IsShared AndAlso value.ReceiverOpt IsNot Nothing) Then
				boundExpression = value.Update(Nothing, fieldSymbol, value.IsLValue, value.SuppressVirtualCalls, value.ConstantsInProgressOpt, value.Type)
			ElseIf (value.ReceiverOpt IsNot Nothing) Then
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Me.CaptureReceiver(value.ReceiverOpt, state)
				boundExpression = value.Update(boundExpression1, fieldSymbol, value.IsLValue, value.SuppressVirtualCalls, value.ConstantsInProgressOpt, value.Type)
			Else
				boundExpression = value
			End If
			Return boundExpression
		End Function

		Private Function CaptureInAByRefTemp(ByVal value As BoundExpression, ByVal state As WithExpressionRewriter.State) As BoundExpression
			Dim type As TypeSymbol = value.Type
			Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(state.ContainingMember, type, SynthesizedLocalKind.[With], Me._withSyntax, True)
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = (New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(value.Syntax, synthesizedLocal, True, type)).MakeCompilerGenerated()
			state.AddLocal(synthesizedLocal, (New BoundReferenceAssignment(value.Syntax, boundLocal, value, True, type, False)).MakeCompilerGenerated())
			Return boundLocal
		End Function

		Private Function CaptureInATemp(ByVal value As BoundExpression, ByVal state As WithExpressionRewriter.State) As Microsoft.CodeAnalysis.VisualBasic.BoundLocal
			Dim type As TypeSymbol = value.Type
			Dim synthesizedLocal As Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedLocal(state.ContainingMember, type, SynthesizedLocalKind.[With], Me._withSyntax, False)
			Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = (New Microsoft.CodeAnalysis.VisualBasic.BoundLocal(value.Syntax, synthesizedLocal, True, type)).MakeCompilerGenerated()
			If (value.IsLValue) Then
				If (Not state.PreserveIdentityOfLValues) Then
					value = value.MakeRValue()
				Else
					value = (New BoundLValueToRValueWrapper(value.Syntax, value, value.Type, False)).MakeCompilerGenerated()
				End If
			End If
			state.AddLocal(synthesizedLocal, (New BoundAssignmentOperator(value.Syntax, boundLocal, value, True, type, False)).MakeCompilerGenerated())
			Return boundLocal
		End Function

		Private Function CaptureReceiver(ByVal value As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal state As WithExpressionRewriter.State) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (Not value.IsLValue OrElse Not value.Type.IsReferenceType) Then
				boundExpression = Me.CaptureExpression(value, state)
			Else
				boundExpression = Me.CaptureInATemp(value, state)
			End If
			Return boundExpression
		End Function

		Private Function CaptureRValue(ByVal value As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal state As WithExpressionRewriter.State) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim kind As BoundKind = value.Kind
			If (kind = BoundKind.BadVariable OrElse CByte(kind) - CByte(BoundKind.Literal) <= CByte(BoundKind.OmittedArgument) OrElse CByte(kind) - CByte(BoundKind.MyBaseReference) <= CByte(BoundKind.OmittedArgument)) Then
				boundExpression = value
			Else
				If (Not value.IsValue() OrElse value.Type Is Nothing OrElse value.Type.IsVoidType()) Then
					Throw ExceptionUtilities.Unreachable
				End If
				If (value.ConstantValueOpt Is Nothing) Then
					boundExpression = Me.CaptureInATemp(value, state).MakeRValue()
				Else
					boundExpression = value
				End If
			End If
			Return boundExpression
		End Function

		Private Function CaptureWithExpression(ByVal value As Microsoft.CodeAnalysis.VisualBasic.BoundExpression, ByVal state As WithExpressionRewriter.State) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim type As TypeSymbol = value.Type
			Dim kind As Microsoft.CodeAnalysis.VisualBasic.BoundKind = value.Kind
			If (kind = Microsoft.CodeAnalysis.VisualBasic.BoundKind.MeReference OrElse kind = Microsoft.CodeAnalysis.VisualBasic.BoundKind.MyClassReference OrElse kind = Microsoft.CodeAnalysis.VisualBasic.BoundKind.MyBaseReference) Then
				boundExpression = value
			ElseIf (type.IsReferenceType AndAlso Not type.IsTypeParameter()) Then
				Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = Me.CaptureInATemp(value, state)
				If (Not value.IsLValue) Then
					boundLocal = boundLocal.MakeRValue()
				End If
				boundExpression = boundLocal
			ElseIf (Not value.IsLValue) Then
				boundExpression = Me.CaptureInATemp(value, state).MakeRValue()
			ElseIf (kind = Microsoft.CodeAnalysis.VisualBasic.BoundKind.Local OrElse kind = Microsoft.CodeAnalysis.VisualBasic.BoundKind.Parameter) Then
				boundExpression = value
			ElseIf (state.DoNotUseByRefLocal OrElse value.Kind = Microsoft.CodeAnalysis.VisualBasic.BoundKind.ArrayAccess AndAlso value.Type.Kind = SymbolKind.TypeParameter) Then
				Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
				Dim boundKind As Microsoft.CodeAnalysis.VisualBasic.BoundKind = value.Kind
				If (boundKind = Microsoft.CodeAnalysis.VisualBasic.BoundKind.ArrayAccess) Then
					boundExpression1 = Me.CaptureArrayAccess(DirectCast(value, BoundArrayAccess), state)
				Else
					If (boundKind <> Microsoft.CodeAnalysis.VisualBasic.BoundKind.FieldAccess) Then
						Throw ExceptionUtilities.UnexpectedValue(value.Kind)
					End If
					boundExpression1 = Me.CaptureFieldAccess(DirectCast(value, BoundFieldAccess), state)
				End If
				boundExpression = boundExpression1
			Else
				boundExpression = Me.CaptureInAByRefTemp(value, state)
			End If
			Return boundExpression
		End Function

		Public Structure Result
			Public ReadOnly Expression As BoundExpression

			Public ReadOnly Locals As ImmutableArray(Of LocalSymbol)

			Public ReadOnly Initializers As ImmutableArray(Of BoundExpression)

			Public Sub New(ByVal expression As BoundExpression, ByVal locals As ImmutableArray(Of LocalSymbol), ByVal initializers As ImmutableArray(Of BoundExpression))
				Me = New WithExpressionRewriter.Result() With
				{
					.Expression = expression,
					.Locals = locals,
					.Initializers = initializers
				}
			End Sub
		End Structure

		Private Class State
			Public ReadOnly ContainingMember As Symbol

			Public ReadOnly DoNotUseByRefLocal As Boolean

			Public ReadOnly Binder As Binder

			Public ReadOnly PreserveIdentityOfLValues As Boolean

			Private _locals As ArrayBuilder(Of LocalSymbol)

			Private _initializers As ArrayBuilder(Of BoundExpression)

			Public Sub New(ByVal containingMember As Symbol, ByVal doNotUseByRefLocal As Boolean, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal preserveIdentityOfLValues As Boolean)
				MyBase.New()
				Me._locals = Nothing
				Me._initializers = Nothing
				Me.ContainingMember = containingMember
				Me.DoNotUseByRefLocal = doNotUseByRefLocal
				Me.Binder = binder
				Me.PreserveIdentityOfLValues = preserveIdentityOfLValues
			End Sub

			Public Sub AddLocal(ByVal local As LocalSymbol, ByVal initializer As BoundExpression)
				If (Me._locals Is Nothing) Then
					Me._locals = ArrayBuilder(Of LocalSymbol).GetInstance()
					Me._initializers = ArrayBuilder(Of BoundExpression).GetInstance()
				End If
				Me._locals.Add(local)
				Me._initializers.Add(initializer)
			End Sub

			Public Function CreateResult(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As WithExpressionRewriter.Result
				Dim empty As ImmutableArray(Of LocalSymbol)
				Dim immutableAndFree As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression)
				Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = expression
				If (Me._locals Is Nothing) Then
					empty = ImmutableArray(Of LocalSymbol).Empty
				Else
					empty = Me._locals.ToImmutableAndFree()
				End If
				If (Me._initializers Is Nothing) Then
					immutableAndFree = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.BoundExpression).Empty
				Else
					immutableAndFree = Me._initializers.ToImmutableAndFree()
				End If
				Return New WithExpressionRewriter.Result(boundExpression, empty, immutableAndFree)
			End Function
		End Class
	End Class
End Namespace