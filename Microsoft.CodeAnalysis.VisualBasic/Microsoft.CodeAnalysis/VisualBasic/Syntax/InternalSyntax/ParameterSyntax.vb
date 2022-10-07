Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class ParameterSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
		Friend ReadOnly _attributeLists As GreenNode

		Friend ReadOnly _modifiers As GreenNode

		Friend ReadOnly _identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax

		Friend ReadOnly _asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax

		Friend ReadOnly _default As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property AsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax
			Get
				Return Me._asClause
			End Get
		End Property

		Friend ReadOnly Property AttributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(Me._attributeLists)
			End Get
		End Property

		Friend ReadOnly Property [Default] As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax
			Get
				Return Me._default
			End Get
		End Property

		Friend ReadOnly Property Identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax
			Get
				Return Me._identifier
			End Get
		End Property

		Friend ReadOnly Property Modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)(Me._modifiers)
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal [default] As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 5
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
			If (asClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(asClause)
				Me._asClause = asClause
			End If
			If ([default] IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth([default])
				Me._default = [default]
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal [default] As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 5
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
			If (asClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(asClause)
				Me._asClause = asClause
			End If
			If ([default] IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth([default])
				Me._default = [default]
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal identifier As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal [default] As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 5
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
			If (asClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(asClause)
				Me._asClause = asClause
			End If
			If ([default] IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth([default])
				Me._default = [default]
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 5
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
			Dim modifiedIdentifierSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax)
			If (modifiedIdentifierSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(modifiedIdentifierSyntax)
				Me._identifier = modifiedIdentifierSyntax
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			If (simpleAsClauseSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(simpleAsClauseSyntax)
				Me._asClause = simpleAsClauseSyntax
			End If
			Dim equalsValueSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax)
			If (equalsValueSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(equalsValueSyntax)
				Me._default = equalsValueSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitParameter(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.ParameterSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._attributeLists
					Exit Select
				Case 1
					greenNode = Me._modifiers
					Exit Select
				Case 2
					greenNode = Me._identifier
					Exit Select
				Case 3
					greenNode = Me._asClause
					Exit Select
				Case 4
					greenNode = Me._default
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._attributeLists, Me._modifiers, Me._identifier, Me._asClause, Me._default)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._attributeLists, Me._modifiers, Me._identifier, Me._asClause, Me._default)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._attributeLists, IObjectWritable))
			writer.WriteValue(DirectCast(Me._modifiers, IObjectWritable))
			writer.WriteValue(DirectCast(Me._identifier, IObjectWritable))
			writer.WriteValue(DirectCast(Me._asClause, IObjectWritable))
			writer.WriteValue(DirectCast(Me._default, IObjectWritable))
		End Sub
	End Class
End Namespace