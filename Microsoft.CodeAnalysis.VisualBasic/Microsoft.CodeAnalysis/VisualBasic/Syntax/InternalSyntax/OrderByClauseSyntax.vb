Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class OrderByClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax
		Friend ReadOnly _orderKeyword As KeywordSyntax

		Friend ReadOnly _byKeyword As KeywordSyntax

		Friend ReadOnly _orderings As GreenNode

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property ByKeyword As KeywordSyntax
			Get
				Return Me._byKeyword
			End Get
		End Property

		Friend ReadOnly Property Orderings As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax)(Me._orderings))
			End Get
		End Property

		Friend ReadOnly Property OrderKeyword As KeywordSyntax
			Get
				Return Me._orderKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal orderKeyword As KeywordSyntax, ByVal byKeyword As KeywordSyntax, ByVal orderings As GreenNode)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(orderKeyword)
			Me._orderKeyword = orderKeyword
			MyBase.AdjustFlagsAndWidth(byKeyword)
			Me._byKeyword = byKeyword
			If (orderings IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(orderings)
				Me._orderings = orderings
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal orderKeyword As KeywordSyntax, ByVal byKeyword As KeywordSyntax, ByVal orderings As GreenNode, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(orderKeyword)
			Me._orderKeyword = orderKeyword
			MyBase.AdjustFlagsAndWidth(byKeyword)
			Me._byKeyword = byKeyword
			If (orderings IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(orderings)
				Me._orderings = orderings
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal orderKeyword As KeywordSyntax, ByVal byKeyword As KeywordSyntax, ByVal orderings As GreenNode)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(orderKeyword)
			Me._orderKeyword = orderKeyword
			MyBase.AdjustFlagsAndWidth(byKeyword)
			Me._byKeyword = byKeyword
			If (orderings IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(orderings)
				Me._orderings = orderings
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._orderKeyword = keywordSyntax
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax1)
				Me._byKeyword = keywordSyntax1
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._orderings = greenNode
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitOrderByClause(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderByClauseSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._orderKeyword
					Exit Select
				Case 1
					greenNode = Me._byKeyword
					Exit Select
				Case 2
					greenNode = Me._orderings
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._orderKeyword, Me._byKeyword, Me._orderings)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._orderKeyword, Me._byKeyword, Me._orderings)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._orderKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._byKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._orderings, IObjectWritable))
		End Sub
	End Class
End Namespace