Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class FromClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax
		Friend ReadOnly _fromKeyword As KeywordSyntax

		Friend ReadOnly _variables As GreenNode

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property FromKeyword As KeywordSyntax
			Get
				Return Me._fromKeyword
			End Get
		End Property

		Friend ReadOnly Property Variables As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax)(Me._variables))
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal fromKeyword As KeywordSyntax, ByVal variables As GreenNode)
			MyBase.New(kind)
			MyBase._slotCount = 2
			MyBase.AdjustFlagsAndWidth(fromKeyword)
			Me._fromKeyword = fromKeyword
			If (variables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(variables)
				Me._variables = variables
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal fromKeyword As KeywordSyntax, ByVal variables As GreenNode, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 2
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(fromKeyword)
			Me._fromKeyword = fromKeyword
			If (variables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(variables)
				Me._variables = variables
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal fromKeyword As KeywordSyntax, ByVal variables As GreenNode)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 2
			MyBase.AdjustFlagsAndWidth(fromKeyword)
			Me._fromKeyword = fromKeyword
			If (variables IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(variables)
				Me._variables = variables
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 2
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._fromKeyword = keywordSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._variables = greenNode
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitFromClause(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.FromClauseSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Dim num As Integer = i
			If (num = 0) Then
				greenNode = Me._fromKeyword
			ElseIf (num = 1) Then
				greenNode = Me._variables
			Else
				greenNode = Nothing
			End If
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._fromKeyword, Me._variables)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._fromKeyword, Me._variables)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._fromKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._variables, IObjectWritable))
		End Sub
	End Class
End Namespace