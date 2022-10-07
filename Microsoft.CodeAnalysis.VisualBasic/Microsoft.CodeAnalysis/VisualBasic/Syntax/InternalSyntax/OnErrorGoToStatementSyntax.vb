Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class OnErrorGoToStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax
		Friend ReadOnly _onKeyword As KeywordSyntax

		Friend ReadOnly _errorKeyword As KeywordSyntax

		Friend ReadOnly _goToKeyword As KeywordSyntax

		Friend ReadOnly _minus As PunctuationSyntax

		Friend ReadOnly _label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property ErrorKeyword As KeywordSyntax
			Get
				Return Me._errorKeyword
			End Get
		End Property

		Friend ReadOnly Property GoToKeyword As KeywordSyntax
			Get
				Return Me._goToKeyword
			End Get
		End Property

		Friend ReadOnly Property Label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			Get
				Return Me._label
			End Get
		End Property

		Friend ReadOnly Property Minus As PunctuationSyntax
			Get
				Return Me._minus
			End Get
		End Property

		Friend ReadOnly Property OnKeyword As KeywordSyntax
			Get
				Return Me._onKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal onKeyword As KeywordSyntax, ByVal errorKeyword As KeywordSyntax, ByVal goToKeyword As KeywordSyntax, ByVal minus As PunctuationSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(onKeyword)
			Me._onKeyword = onKeyword
			MyBase.AdjustFlagsAndWidth(errorKeyword)
			Me._errorKeyword = errorKeyword
			MyBase.AdjustFlagsAndWidth(goToKeyword)
			Me._goToKeyword = goToKeyword
			If (minus IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(minus)
				Me._minus = minus
			End If
			MyBase.AdjustFlagsAndWidth(label)
			Me._label = label
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal onKeyword As KeywordSyntax, ByVal errorKeyword As KeywordSyntax, ByVal goToKeyword As KeywordSyntax, ByVal minus As PunctuationSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 5
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(onKeyword)
			Me._onKeyword = onKeyword
			MyBase.AdjustFlagsAndWidth(errorKeyword)
			Me._errorKeyword = errorKeyword
			MyBase.AdjustFlagsAndWidth(goToKeyword)
			Me._goToKeyword = goToKeyword
			If (minus IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(minus)
				Me._minus = minus
			End If
			MyBase.AdjustFlagsAndWidth(label)
			Me._label = label
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal onKeyword As KeywordSyntax, ByVal errorKeyword As KeywordSyntax, ByVal goToKeyword As KeywordSyntax, ByVal minus As PunctuationSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(onKeyword)
			Me._onKeyword = onKeyword
			MyBase.AdjustFlagsAndWidth(errorKeyword)
			Me._errorKeyword = errorKeyword
			MyBase.AdjustFlagsAndWidth(goToKeyword)
			Me._goToKeyword = goToKeyword
			If (minus IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(minus)
				Me._minus = minus
			End If
			MyBase.AdjustFlagsAndWidth(label)
			Me._label = label
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 5
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._onKeyword = keywordSyntax
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax1)
				Me._errorKeyword = keywordSyntax1
			End If
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax2)
				Me._goToKeyword = keywordSyntax2
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._minus = punctuationSyntax
			End If
			Dim labelSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)
			If (labelSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(labelSyntax)
				Me._label = labelSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitOnErrorGoToStatement(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorGoToStatementSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._onKeyword
					Exit Select
				Case 1
					greenNode = Me._errorKeyword
					Exit Select
				Case 2
					greenNode = Me._goToKeyword
					Exit Select
				Case 3
					greenNode = Me._minus
					Exit Select
				Case 4
					greenNode = Me._label
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._onKeyword, Me._errorKeyword, Me._goToKeyword, Me._minus, Me._label)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._onKeyword, Me._errorKeyword, Me._goToKeyword, Me._minus, Me._label)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._onKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._errorKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._goToKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._minus, IObjectWritable))
			writer.WriteValue(DirectCast(Me._label, IObjectWritable))
		End Sub
	End Class
End Namespace