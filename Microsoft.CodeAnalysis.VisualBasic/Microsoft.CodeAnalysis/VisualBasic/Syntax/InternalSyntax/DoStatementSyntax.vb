Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class DoStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax
		Friend ReadOnly _doKeyword As KeywordSyntax

		Friend ReadOnly _whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property DoKeyword As KeywordSyntax
			Get
				Return Me._doKeyword
			End Get
		End Property

		Friend ReadOnly Property WhileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax
			Get
				Return Me._whileOrUntilClause
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal doKeyword As KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 2
			MyBase.AdjustFlagsAndWidth(doKeyword)
			Me._doKeyword = doKeyword
			If (whileOrUntilClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(whileOrUntilClause)
				Me._whileOrUntilClause = whileOrUntilClause
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal doKeyword As KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 2
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(doKeyword)
			Me._doKeyword = doKeyword
			If (whileOrUntilClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(whileOrUntilClause)
				Me._whileOrUntilClause = whileOrUntilClause
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal doKeyword As KeywordSyntax, ByVal whileOrUntilClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 2
			MyBase.AdjustFlagsAndWidth(doKeyword)
			Me._doKeyword = doKeyword
			If (whileOrUntilClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(whileOrUntilClause)
				Me._whileOrUntilClause = whileOrUntilClause
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 2
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._doKeyword = keywordSyntax
			End If
			Dim whileOrUntilClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax)
			If (whileOrUntilClauseSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(whileOrUntilClauseSyntax)
				Me._whileOrUntilClause = whileOrUntilClauseSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitDoStatement(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.DoStatementSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Dim num As Integer = i
			If (num = 0) Then
				greenNode = Me._doKeyword
			ElseIf (num = 1) Then
				greenNode = Me._whileOrUntilClause
			Else
				greenNode = Nothing
			End If
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._doKeyword, Me._whileOrUntilClause)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._doKeyword, Me._whileOrUntilClause)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._doKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._whileOrUntilClause, IObjectWritable))
		End Sub
	End Class
End Namespace