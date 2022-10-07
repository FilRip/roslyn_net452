Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class TypeBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclarationStatementSyntax
		Friend ReadOnly _inherits As GreenNode

		Friend ReadOnly _implements As GreenNode

		Friend ReadOnly _members As GreenNode

		Public MustOverride ReadOnly Property BlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax

		Public MustOverride ReadOnly Property EndBlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax

		Friend ReadOnly Property [Implements] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax)(Me._implements)
			End Get
		End Property

		Friend ReadOnly Property [Inherits] As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax)(Me._inherits)
			End Get
		End Property

		Friend ReadOnly Property Members As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax)(Me._members)
			End Get
		End Property

		Friend Sub New(ByVal kind As SyntaxKind, ByVal [inherits] As GreenNode, ByVal [implements] As GreenNode, ByVal members As GreenNode)
			MyBase.New(kind)
			If ([inherits] IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth([inherits])
				Me._inherits = [inherits]
			End If
			If ([implements] IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth([implements])
				Me._implements = [implements]
			End If
			If (members IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(members)
				Me._members = members
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal [inherits] As GreenNode, ByVal [implements] As GreenNode, ByVal members As GreenNode, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase.SetFactoryContext(context)
			If ([inherits] IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth([inherits])
				Me._inherits = [inherits]
			End If
			If ([implements] IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth([implements])
				Me._implements = [implements]
			End If
			If (members IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(members)
				Me._members = members
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal [inherits] As GreenNode, ByVal [implements] As GreenNode, ByVal members As GreenNode)
			MyBase.New(kind, errors, annotations)
			If ([inherits] IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth([inherits])
				Me._inherits = [inherits]
			End If
			If ([implements] IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth([implements])
				Me._implements = [implements]
			End If
			If (members IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(members)
				Me._members = members
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._inherits = greenNode
			End If
			Dim greenNode1 As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode1)
				Me._implements = greenNode1
			End If
			Dim greenNode2 As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode2)
				Me._members = greenNode2
			End If
		End Sub

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._inherits, IObjectWritable))
			writer.WriteValue(DirectCast(Me._implements, IObjectWritable))
			writer.WriteValue(DirectCast(Me._members, IObjectWritable))
		End Sub
	End Class
End Namespace