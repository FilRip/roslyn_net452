Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class SimpleAsClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax
		Friend ReadOnly _attributeLists As GreenNode

		Friend ReadOnly _type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property AttributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(Me._attributeLists)
			End Get
		End Property

		Friend ReadOnly Property Type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax
			Get
				Return Me._type
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal asKeyword As KeywordSyntax, ByVal attributeLists As GreenNode, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			MyBase.New(kind, asKeyword)
			MyBase._slotCount = 3
			If (attributeLists IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(attributeLists)
				Me._attributeLists = attributeLists
			End If
			MyBase.AdjustFlagsAndWidth(type)
			Me._type = type
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal asKeyword As KeywordSyntax, ByVal attributeLists As GreenNode, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, asKeyword)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			If (attributeLists IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(attributeLists)
				Me._attributeLists = attributeLists
			End If
			MyBase.AdjustFlagsAndWidth(type)
			Me._type = type
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal asKeyword As KeywordSyntax, ByVal attributeLists As GreenNode, ByVal type As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			MyBase.New(kind, errors, annotations, asKeyword)
			MyBase._slotCount = 3
			If (attributeLists IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(attributeLists)
				Me._attributeLists = attributeLists
			End If
			MyBase.AdjustFlagsAndWidth(type)
			Me._type = type
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._attributeLists = greenNode
			End If
			Dim typeSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax)
			If (typeSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(typeSyntax)
				Me._type = typeSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitSimpleAsClause(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleAsClauseSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._asKeyword
					Exit Select
				Case 1
					greenNode = Me._attributeLists
					Exit Select
				Case 2
					greenNode = Me._type
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._asKeyword, Me._attributeLists, Me._type)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._asKeyword, Me._attributeLists, Me._type)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._attributeLists, IObjectWritable))
			writer.WriteValue(DirectCast(Me._type, IObjectWritable))
		End Sub
	End Class
End Namespace