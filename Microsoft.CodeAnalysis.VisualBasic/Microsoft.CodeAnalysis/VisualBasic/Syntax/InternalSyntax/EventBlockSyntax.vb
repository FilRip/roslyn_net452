Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class EventBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclarationStatementSyntax
		Friend ReadOnly _eventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax

		Friend ReadOnly _accessors As GreenNode

		Friend ReadOnly _endEventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Accessors As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax)(Me._accessors)
			End Get
		End Property

		Friend ReadOnly Property EndEventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Get
				Return Me._endEventStatement
			End Get
		End Property

		Friend ReadOnly Property EventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax
			Get
				Return Me._eventStatement
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal eventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax, ByVal accessors As GreenNode, ByVal endEventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(eventStatement)
			Me._eventStatement = eventStatement
			If (accessors IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(accessors)
				Me._accessors = accessors
			End If
			MyBase.AdjustFlagsAndWidth(endEventStatement)
			Me._endEventStatement = endEventStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal eventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax, ByVal accessors As GreenNode, ByVal endEventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(eventStatement)
			Me._eventStatement = eventStatement
			If (accessors IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(accessors)
				Me._accessors = accessors
			End If
			MyBase.AdjustFlagsAndWidth(endEventStatement)
			Me._endEventStatement = endEventStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal eventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax, ByVal accessors As GreenNode, ByVal endEventStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(eventStatement)
			Me._eventStatement = eventStatement
			If (accessors IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(accessors)
				Me._accessors = accessors
			End If
			MyBase.AdjustFlagsAndWidth(endEventStatement)
			Me._endEventStatement = endEventStatement
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim eventStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax)
			If (eventStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(eventStatementSyntax)
				Me._eventStatement = eventStatementSyntax
			End If
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._accessors = greenNode
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (endBlockStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(endBlockStatementSyntax)
				Me._endEventStatement = endBlockStatementSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitEventBlock(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.EventBlockSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._eventStatement
					Exit Select
				Case 1
					greenNode = Me._accessors
					Exit Select
				Case 2
					greenNode = Me._endEventStatement
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._eventStatement, Me._accessors, Me._endEventStatement)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._eventStatement, Me._accessors, Me._endEventStatement)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._eventStatement, IObjectWritable))
			writer.WriteValue(DirectCast(Me._accessors, IObjectWritable))
			writer.WriteValue(DirectCast(Me._endEventStatement, IObjectWritable))
		End Sub
	End Class
End Namespace