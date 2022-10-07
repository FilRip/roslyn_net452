Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class AddRemoveHandlerStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax
		Friend ReadOnly _addHandlerOrRemoveHandlerKeyword As KeywordSyntax

		Friend ReadOnly _eventExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax

		Friend ReadOnly _commaToken As PunctuationSyntax

		Friend ReadOnly _delegateExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property AddHandlerOrRemoveHandlerKeyword As KeywordSyntax
			Get
				Return Me._addHandlerOrRemoveHandlerKeyword
			End Get
		End Property

		Friend ReadOnly Property CommaToken As PunctuationSyntax
			Get
				Return Me._commaToken
			End Get
		End Property

		Friend ReadOnly Property DelegateExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Get
				Return Me._delegateExpression
			End Get
		End Property

		Friend ReadOnly Property EventExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax
			Get
				Return Me._eventExpression
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal addHandlerOrRemoveHandlerKeyword As KeywordSyntax, ByVal eventExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal delegateExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(addHandlerOrRemoveHandlerKeyword)
			Me._addHandlerOrRemoveHandlerKeyword = addHandlerOrRemoveHandlerKeyword
			MyBase.AdjustFlagsAndWidth(eventExpression)
			Me._eventExpression = eventExpression
			MyBase.AdjustFlagsAndWidth(commaToken)
			Me._commaToken = commaToken
			MyBase.AdjustFlagsAndWidth(delegateExpression)
			Me._delegateExpression = delegateExpression
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal addHandlerOrRemoveHandlerKeyword As KeywordSyntax, ByVal eventExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal delegateExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(addHandlerOrRemoveHandlerKeyword)
			Me._addHandlerOrRemoveHandlerKeyword = addHandlerOrRemoveHandlerKeyword
			MyBase.AdjustFlagsAndWidth(eventExpression)
			Me._eventExpression = eventExpression
			MyBase.AdjustFlagsAndWidth(commaToken)
			Me._commaToken = commaToken
			MyBase.AdjustFlagsAndWidth(delegateExpression)
			Me._delegateExpression = delegateExpression
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal addHandlerOrRemoveHandlerKeyword As KeywordSyntax, ByVal eventExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax, ByVal commaToken As PunctuationSyntax, ByVal delegateExpression As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(addHandlerOrRemoveHandlerKeyword)
			Me._addHandlerOrRemoveHandlerKeyword = addHandlerOrRemoveHandlerKeyword
			MyBase.AdjustFlagsAndWidth(eventExpression)
			Me._eventExpression = eventExpression
			MyBase.AdjustFlagsAndWidth(commaToken)
			Me._commaToken = commaToken
			MyBase.AdjustFlagsAndWidth(delegateExpression)
			Me._delegateExpression = delegateExpression
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 4
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._addHandlerOrRemoveHandlerKeyword = keywordSyntax
			End If
			Dim expressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (expressionSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expressionSyntax)
				Me._eventExpression = expressionSyntax
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._commaToken = punctuationSyntax
			End If
			Dim expressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)
			If (expressionSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(expressionSyntax1)
				Me._delegateExpression = expressionSyntax1
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitAddRemoveHandlerStatement(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.AddRemoveHandlerStatementSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._addHandlerOrRemoveHandlerKeyword
					Exit Select
				Case 1
					greenNode = Me._eventExpression
					Exit Select
				Case 2
					greenNode = Me._commaToken
					Exit Select
				Case 3
					greenNode = Me._delegateExpression
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._addHandlerOrRemoveHandlerKeyword, Me._eventExpression, Me._commaToken, Me._delegateExpression)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._addHandlerOrRemoveHandlerKeyword, Me._eventExpression, Me._commaToken, Me._delegateExpression)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._addHandlerOrRemoveHandlerKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._eventExpression, IObjectWritable))
			writer.WriteValue(DirectCast(Me._commaToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._delegateExpression, IObjectWritable))
		End Sub
	End Class
End Namespace