Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class TypeParameterMultipleConstraintClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax
		Friend ReadOnly _asKeyword As KeywordSyntax

		Friend ReadOnly _openBraceToken As PunctuationSyntax

		Friend ReadOnly _constraints As GreenNode

		Friend ReadOnly _closeBraceToken As PunctuationSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property AsKeyword As KeywordSyntax
			Get
				Return Me._asKeyword
			End Get
		End Property

		Friend ReadOnly Property CloseBraceToken As PunctuationSyntax
			Get
				Return Me._closeBraceToken
			End Get
		End Property

		Friend ReadOnly Property Constraints As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax)(Me._constraints))
			End Get
		End Property

		Friend ReadOnly Property OpenBraceToken As PunctuationSyntax
			Get
				Return Me._openBraceToken
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal asKeyword As KeywordSyntax, ByVal openBraceToken As PunctuationSyntax, ByVal constraints As GreenNode, ByVal closeBraceToken As PunctuationSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(asKeyword)
			Me._asKeyword = asKeyword
			MyBase.AdjustFlagsAndWidth(openBraceToken)
			Me._openBraceToken = openBraceToken
			If (constraints IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(constraints)
				Me._constraints = constraints
			End If
			MyBase.AdjustFlagsAndWidth(closeBraceToken)
			Me._closeBraceToken = closeBraceToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal asKeyword As KeywordSyntax, ByVal openBraceToken As PunctuationSyntax, ByVal constraints As GreenNode, ByVal closeBraceToken As PunctuationSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(asKeyword)
			Me._asKeyword = asKeyword
			MyBase.AdjustFlagsAndWidth(openBraceToken)
			Me._openBraceToken = openBraceToken
			If (constraints IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(constraints)
				Me._constraints = constraints
			End If
			MyBase.AdjustFlagsAndWidth(closeBraceToken)
			Me._closeBraceToken = closeBraceToken
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal asKeyword As KeywordSyntax, ByVal openBraceToken As PunctuationSyntax, ByVal constraints As GreenNode, ByVal closeBraceToken As PunctuationSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(asKeyword)
			Me._asKeyword = asKeyword
			MyBase.AdjustFlagsAndWidth(openBraceToken)
			Me._openBraceToken = openBraceToken
			If (constraints IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(constraints)
				Me._constraints = constraints
			End If
			MyBase.AdjustFlagsAndWidth(closeBraceToken)
			Me._closeBraceToken = closeBraceToken
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 4
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._asKeyword = keywordSyntax
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._openBraceToken = punctuationSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._constraints = greenNode
			End If
			Dim punctuationSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax1)
				Me._closeBraceToken = punctuationSyntax1
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitTypeParameterMultipleConstraintClause(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterMultipleConstraintClauseSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._asKeyword
					Exit Select
				Case 1
					greenNode = Me._openBraceToken
					Exit Select
				Case 2
					greenNode = Me._constraints
					Exit Select
				Case 3
					greenNode = Me._closeBraceToken
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._asKeyword, Me._openBraceToken, Me._constraints, Me._closeBraceToken)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._asKeyword, Me._openBraceToken, Me._constraints, Me._closeBraceToken)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._asKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._openBraceToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._constraints, IObjectWritable))
			writer.WriteValue(DirectCast(Me._closeBraceToken, IObjectWritable))
		End Sub
	End Class
End Namespace