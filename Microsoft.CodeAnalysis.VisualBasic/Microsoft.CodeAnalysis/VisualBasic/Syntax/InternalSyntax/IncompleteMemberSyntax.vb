Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class IncompleteMemberSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclarationStatementSyntax
		Friend ReadOnly _attributeLists As GreenNode

		Friend ReadOnly _modifiers As GreenNode

		Friend ReadOnly _missingIdentifier As IdentifierTokenSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property AttributeLists As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax)(Me._attributeLists)
			End Get
		End Property

		Friend ReadOnly Property MissingIdentifier As IdentifierTokenSyntax
			Get
				Return Me._missingIdentifier
			End Get
		End Property

		Friend ReadOnly Property Modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)(Me._modifiers)
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal missingIdentifier As IdentifierTokenSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 3
			If (attributeLists IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(attributeLists)
				Me._attributeLists = attributeLists
			End If
			If (modifiers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(modifiers)
				Me._modifiers = modifiers
			End If
			If (missingIdentifier IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(missingIdentifier)
				Me._missingIdentifier = missingIdentifier
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal missingIdentifier As IdentifierTokenSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			If (attributeLists IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(attributeLists)
				Me._attributeLists = attributeLists
			End If
			If (modifiers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(modifiers)
				Me._modifiers = modifiers
			End If
			If (missingIdentifier IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(missingIdentifier)
				Me._missingIdentifier = missingIdentifier
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal missingIdentifier As IdentifierTokenSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			If (attributeLists IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(attributeLists)
				Me._attributeLists = attributeLists
			End If
			If (modifiers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(modifiers)
				Me._modifiers = modifiers
			End If
			If (missingIdentifier IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(missingIdentifier)
				Me._missingIdentifier = missingIdentifier
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
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
				Me._missingIdentifier = identifierTokenSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitIncompleteMember(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.IncompleteMemberSyntax(Me, parent, startLocation)
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
					greenNode = Me._missingIdentifier
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._attributeLists, Me._modifiers, Me._missingIdentifier)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._attributeLists, Me._modifiers, Me._missingIdentifier)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._attributeLists, IObjectWritable))
			writer.WriteValue(DirectCast(Me._modifiers, IObjectWritable))
			writer.WriteValue(DirectCast(Me._missingIdentifier, IObjectWritable))
		End Sub
	End Class
End Namespace