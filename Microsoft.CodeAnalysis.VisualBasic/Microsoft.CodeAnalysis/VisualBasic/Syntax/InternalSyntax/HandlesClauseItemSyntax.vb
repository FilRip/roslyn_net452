Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class HandlesClauseItemSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
		Friend ReadOnly _eventContainer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventContainerSyntax

		Friend ReadOnly _dotToken As PunctuationSyntax

		Friend ReadOnly _eventMember As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property DotToken As PunctuationSyntax
			Get
				Return Me._dotToken
			End Get
		End Property

		Friend ReadOnly Property EventContainer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventContainerSyntax
			Get
				Return Me._eventContainer
			End Get
		End Property

		Friend ReadOnly Property EventMember As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax
			Get
				Return Me._eventMember
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal eventContainer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventContainerSyntax, ByVal dotToken As PunctuationSyntax, ByVal eventMember As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(eventContainer)
			Me._eventContainer = eventContainer
			MyBase.AdjustFlagsAndWidth(dotToken)
			Me._dotToken = dotToken
			MyBase.AdjustFlagsAndWidth(eventMember)
			Me._eventMember = eventMember
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal eventContainer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventContainerSyntax, ByVal dotToken As PunctuationSyntax, ByVal eventMember As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(eventContainer)
			Me._eventContainer = eventContainer
			MyBase.AdjustFlagsAndWidth(dotToken)
			Me._dotToken = dotToken
			MyBase.AdjustFlagsAndWidth(eventMember)
			Me._eventMember = eventMember
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal eventContainer As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventContainerSyntax, ByVal dotToken As PunctuationSyntax, ByVal eventMember As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(eventContainer)
			Me._eventContainer = eventContainer
			MyBase.AdjustFlagsAndWidth(dotToken)
			Me._dotToken = dotToken
			MyBase.AdjustFlagsAndWidth(eventMember)
			Me._eventMember = eventMember
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim eventContainerSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventContainerSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventContainerSyntax)
			If (eventContainerSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(eventContainerSyntax)
				Me._eventContainer = eventContainerSyntax
			End If
			Dim punctuationSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PunctuationSyntax)
			If (punctuationSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(punctuationSyntax)
				Me._dotToken = punctuationSyntax
			End If
			Dim identifierNameSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax)
			If (identifierNameSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(identifierNameSyntax)
				Me._eventMember = identifierNameSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitHandlesClauseItem(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.HandlesClauseItemSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._eventContainer
					Exit Select
				Case 1
					greenNode = Me._dotToken
					Exit Select
				Case 2
					greenNode = Me._eventMember
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._eventContainer, Me._dotToken, Me._eventMember)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._eventContainer, Me._dotToken, Me._eventMember)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._eventContainer, IObjectWritable))
			writer.WriteValue(DirectCast(Me._dotToken, IObjectWritable))
			writer.WriteValue(DirectCast(Me._eventMember, IObjectWritable))
		End Sub
	End Class
End Namespace