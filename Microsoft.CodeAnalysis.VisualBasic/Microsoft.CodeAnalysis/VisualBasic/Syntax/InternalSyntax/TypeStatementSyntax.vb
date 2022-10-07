Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class TypeStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclarationStatementSyntax
		Friend ReadOnly _attributeLists As GreenNode

		Friend ReadOnly _modifiers As GreenNode

		Friend ReadOnly _identifier As IdentifierTokenSyntax

		Friend ReadOnly _typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax

		Friend ReadOnly Property AttributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(Me._attributeLists)
			End Get
		End Property

		Friend ReadOnly Property Identifier As IdentifierTokenSyntax
			Get
				Return Me._identifier
			End Get
		End Property

		Friend ReadOnly Property Modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)(Me._modifiers)
			End Get
		End Property

		Friend ReadOnly Property TypeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax
			Get
				Return Me._typeParameterList
			End Get
		End Property

		Friend Sub New(ByVal kind As SyntaxKind, ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)
			MyBase.New(kind)
			If (attributeLists IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(attributeLists)
				Me._attributeLists = attributeLists
			End If
			If (modifiers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(modifiers)
				Me._modifiers = modifiers
			End If
			MyBase.AdjustFlagsAndWidth(identifier)
			Me._identifier = identifier
			If (typeParameterList IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(typeParameterList)
				Me._typeParameterList = typeParameterList
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase.SetFactoryContext(context)
			If (attributeLists IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(attributeLists)
				Me._attributeLists = attributeLists
			End If
			If (modifiers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(modifiers)
				Me._modifiers = modifiers
			End If
			MyBase.AdjustFlagsAndWidth(identifier)
			Me._identifier = identifier
			If (typeParameterList IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(typeParameterList)
				Me._typeParameterList = typeParameterList
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)
			MyBase.New(kind, errors, annotations)
			If (attributeLists IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(attributeLists)
				Me._attributeLists = attributeLists
			End If
			If (modifiers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(modifiers)
				Me._modifiers = modifiers
			End If
			MyBase.AdjustFlagsAndWidth(identifier)
			Me._identifier = identifier
			If (typeParameterList IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(typeParameterList)
				Me._typeParameterList = typeParameterList
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._attributeLists = greenNode
			End If
			Dim greenNode1 As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode1)
				Me._modifiers = greenNode1
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (identifierTokenSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(identifierTokenSyntax)
				Me._identifier = identifierTokenSyntax
			End If
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)
			If (typeParameterListSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(typeParameterListSyntax)
				Me._typeParameterList = typeParameterListSyntax
			End If
		End Sub

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._attributeLists, IObjectWritable))
			writer.WriteValue(DirectCast(Me._modifiers, IObjectWritable))
			writer.WriteValue(DirectCast(Me._identifier, IObjectWritable))
			writer.WriteValue(DirectCast(Me._typeParameterList, IObjectWritable))
		End Sub
	End Class
End Namespace