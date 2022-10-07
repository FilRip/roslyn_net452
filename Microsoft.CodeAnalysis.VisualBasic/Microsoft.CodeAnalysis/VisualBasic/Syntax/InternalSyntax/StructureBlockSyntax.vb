Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class StructureBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeBlockSyntax
		Friend ReadOnly _structureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax

		Friend ReadOnly _endStructureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Public Overrides ReadOnly Property BlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax
			Get
				Return Me.StructureStatement
			End Get
		End Property

		Public Overrides ReadOnly Property EndBlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Get
				Return Me.EndStructureStatement
			End Get
		End Property

		Friend ReadOnly Property EndStructureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Get
				Return Me._endStructureStatement
			End Get
		End Property

		Friend ReadOnly Property StructureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax
			Get
				Return Me._structureStatement
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureBlockSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureBlockSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureBlockSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureBlockSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal structureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax, ByVal [inherits] As GreenNode, ByVal [implements] As GreenNode, ByVal members As GreenNode, ByVal endStructureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.New(kind, [inherits], [implements], members)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(structureStatement)
			Me._structureStatement = structureStatement
			MyBase.AdjustFlagsAndWidth(endStructureStatement)
			Me._endStructureStatement = endStructureStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal structureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax, ByVal [inherits] As GreenNode, ByVal [implements] As GreenNode, ByVal members As GreenNode, ByVal endStructureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, [inherits], [implements], members)
			MyBase._slotCount = 5
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(structureStatement)
			Me._structureStatement = structureStatement
			MyBase.AdjustFlagsAndWidth(endStructureStatement)
			Me._endStructureStatement = endStructureStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal structureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax, ByVal [inherits] As GreenNode, ByVal [implements] As GreenNode, ByVal members As GreenNode, ByVal endStructureStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.New(kind, errors, annotations, [inherits], [implements], members)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(structureStatement)
			Me._structureStatement = structureStatement
			MyBase.AdjustFlagsAndWidth(endStructureStatement)
			Me._endStructureStatement = endStructureStatement
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 5
			Dim structureStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax)
			If (structureStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(structureStatementSyntax)
				Me._structureStatement = structureStatementSyntax
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (endBlockStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(endBlockStatementSyntax)
				Me._endStructureStatement = endBlockStatementSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitStructureBlock(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.StructureBlockSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._structureStatement
					Exit Select
				Case 1
					greenNode = Me._inherits
					Exit Select
				Case 2
					greenNode = Me._implements
					Exit Select
				Case 3
					greenNode = Me._members
					Exit Select
				Case 4
					greenNode = Me._endStructureStatement
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureBlockSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._structureStatement, Me._inherits, Me._implements, Me._members, Me._endStructureStatement)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureBlockSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._structureStatement, Me._inherits, Me._implements, Me._members, Me._endStructureStatement)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._structureStatement, IObjectWritable))
			writer.WriteValue(DirectCast(Me._endStructureStatement, IObjectWritable))
		End Sub
	End Class
End Namespace