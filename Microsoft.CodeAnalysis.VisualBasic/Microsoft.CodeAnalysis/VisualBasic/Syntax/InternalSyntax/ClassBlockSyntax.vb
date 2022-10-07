Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class ClassBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeBlockSyntax
		Friend ReadOnly _classStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax

		Friend ReadOnly _endClassStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Public Overrides ReadOnly Property BlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax
			Get
				Return Me.ClassStatement
			End Get
		End Property

		Friend ReadOnly Property ClassStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax
			Get
				Return Me._classStatement
			End Get
		End Property

		Public Overrides ReadOnly Property EndBlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Get
				Return Me.EndClassStatement
			End Get
		End Property

		Friend ReadOnly Property EndClassStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Get
				Return Me._endClassStatement
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassBlockSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassBlockSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassBlockSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassBlockSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal classStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax, ByVal [inherits] As GreenNode, ByVal [implements] As GreenNode, ByVal members As GreenNode, ByVal endClassStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.New(kind, [inherits], [implements], members)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(classStatement)
			Me._classStatement = classStatement
			MyBase.AdjustFlagsAndWidth(endClassStatement)
			Me._endClassStatement = endClassStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal classStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax, ByVal [inherits] As GreenNode, ByVal [implements] As GreenNode, ByVal members As GreenNode, ByVal endClassStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, [inherits], [implements], members)
			MyBase._slotCount = 5
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(classStatement)
			Me._classStatement = classStatement
			MyBase.AdjustFlagsAndWidth(endClassStatement)
			Me._endClassStatement = endClassStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal classStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax, ByVal [inherits] As GreenNode, ByVal [implements] As GreenNode, ByVal members As GreenNode, ByVal endClassStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.New(kind, errors, annotations, [inherits], [implements], members)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(classStatement)
			Me._classStatement = classStatement
			MyBase.AdjustFlagsAndWidth(endClassStatement)
			Me._endClassStatement = endClassStatement
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 5
			Dim classStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax)
			If (classStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(classStatementSyntax)
				Me._classStatement = classStatementSyntax
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (endBlockStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(endBlockStatementSyntax)
				Me._endClassStatement = endBlockStatementSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitClassBlock(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.ClassBlockSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._classStatement
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
					greenNode = Me._endClassStatement
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassBlockSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._classStatement, Me._inherits, Me._implements, Me._members, Me._endClassStatement)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassBlockSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._classStatement, Me._inherits, Me._implements, Me._members, Me._endClassStatement)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._classStatement, IObjectWritable))
			writer.WriteValue(DirectCast(Me._endClassStatement, IObjectWritable))
		End Sub
	End Class
End Namespace