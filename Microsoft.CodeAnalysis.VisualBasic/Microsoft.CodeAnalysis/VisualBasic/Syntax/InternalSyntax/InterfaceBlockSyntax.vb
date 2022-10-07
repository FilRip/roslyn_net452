Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class InterfaceBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeBlockSyntax
		Friend ReadOnly _interfaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax

		Friend ReadOnly _endInterfaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Public Overrides ReadOnly Property BlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax
			Get
				Return Me.InterfaceStatement
			End Get
		End Property

		Public Overrides ReadOnly Property EndBlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Get
				Return Me.EndInterfaceStatement
			End Get
		End Property

		Friend ReadOnly Property EndInterfaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Get
				Return Me._endInterfaceStatement
			End Get
		End Property

		Friend ReadOnly Property InterfaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax
			Get
				Return Me._interfaceStatement
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceBlockSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceBlockSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceBlockSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceBlockSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal interfaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax, ByVal [inherits] As GreenNode, ByVal [implements] As GreenNode, ByVal members As GreenNode, ByVal endInterfaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.New(kind, [inherits], [implements], members)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(interfaceStatement)
			Me._interfaceStatement = interfaceStatement
			MyBase.AdjustFlagsAndWidth(endInterfaceStatement)
			Me._endInterfaceStatement = endInterfaceStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal interfaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax, ByVal [inherits] As GreenNode, ByVal [implements] As GreenNode, ByVal members As GreenNode, ByVal endInterfaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, [inherits], [implements], members)
			MyBase._slotCount = 5
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(interfaceStatement)
			Me._interfaceStatement = interfaceStatement
			MyBase.AdjustFlagsAndWidth(endInterfaceStatement)
			Me._endInterfaceStatement = endInterfaceStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal interfaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax, ByVal [inherits] As GreenNode, ByVal [implements] As GreenNode, ByVal members As GreenNode, ByVal endInterfaceStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.New(kind, errors, annotations, [inherits], [implements], members)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(interfaceStatement)
			Me._interfaceStatement = interfaceStatement
			MyBase.AdjustFlagsAndWidth(endInterfaceStatement)
			Me._endInterfaceStatement = endInterfaceStatement
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 5
			Dim interfaceStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax)
			If (interfaceStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(interfaceStatementSyntax)
				Me._interfaceStatement = interfaceStatementSyntax
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (endBlockStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(endBlockStatementSyntax)
				Me._endInterfaceStatement = endBlockStatementSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitInterfaceBlock(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InterfaceBlockSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._interfaceStatement
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
					greenNode = Me._endInterfaceStatement
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceBlockSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._interfaceStatement, Me._inherits, Me._implements, Me._members, Me._endInterfaceStatement)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceBlockSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._interfaceStatement, Me._inherits, Me._implements, Me._members, Me._endInterfaceStatement)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._interfaceStatement, IObjectWritable))
			writer.WriteValue(DirectCast(Me._endInterfaceStatement, IObjectWritable))
		End Sub
	End Class
End Namespace