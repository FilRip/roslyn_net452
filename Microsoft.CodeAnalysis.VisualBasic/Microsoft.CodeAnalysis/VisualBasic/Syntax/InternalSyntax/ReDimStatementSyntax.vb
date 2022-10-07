Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class ReDimStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax
		Friend ReadOnly _reDimKeyword As KeywordSyntax

		Friend ReadOnly _preserveKeyword As KeywordSyntax

		Friend ReadOnly _clauses As GreenNode

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Clauses As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax)(Me._clauses))
			End Get
		End Property

		Friend ReadOnly Property PreserveKeyword As KeywordSyntax
			Get
				Return Me._preserveKeyword
			End Get
		End Property

		Friend ReadOnly Property ReDimKeyword As KeywordSyntax
			Get
				Return Me._reDimKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal reDimKeyword As KeywordSyntax, ByVal preserveKeyword As KeywordSyntax, ByVal clauses As GreenNode)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(reDimKeyword)
			Me._reDimKeyword = reDimKeyword
			If (preserveKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(preserveKeyword)
				Me._preserveKeyword = preserveKeyword
			End If
			If (clauses IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(clauses)
				Me._clauses = clauses
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal reDimKeyword As KeywordSyntax, ByVal preserveKeyword As KeywordSyntax, ByVal clauses As GreenNode, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(reDimKeyword)
			Me._reDimKeyword = reDimKeyword
			If (preserveKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(preserveKeyword)
				Me._preserveKeyword = preserveKeyword
			End If
			If (clauses IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(clauses)
				Me._clauses = clauses
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal reDimKeyword As KeywordSyntax, ByVal preserveKeyword As KeywordSyntax, ByVal clauses As GreenNode)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(reDimKeyword)
			Me._reDimKeyword = reDimKeyword
			If (preserveKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(preserveKeyword)
				Me._preserveKeyword = preserveKeyword
			End If
			If (clauses IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(clauses)
				Me._clauses = clauses
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._reDimKeyword = keywordSyntax
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax1)
				Me._preserveKeyword = keywordSyntax1
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._clauses = greenNode
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitReDimStatement(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.ReDimStatementSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._reDimKeyword
					Exit Select
				Case 1
					greenNode = Me._preserveKeyword
					Exit Select
				Case 2
					greenNode = Me._clauses
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._reDimKeyword, Me._preserveKeyword, Me._clauses)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._reDimKeyword, Me._preserveKeyword, Me._clauses)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._reDimKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._preserveKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._clauses, IObjectWritable))
		End Sub
	End Class
End Namespace