Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class ForBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForOrForEachBlockSyntax
		Friend ReadOnly _forStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property ForStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax
			Get
				Return Me._forStatement
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal forStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax, ByVal statements As GreenNode, ByVal nextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax)
			MyBase.New(kind, statements, nextStatement)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(forStatement)
			Me._forStatement = forStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal forStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax, ByVal statements As GreenNode, ByVal nextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, statements, nextStatement)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(forStatement)
			Me._forStatement = forStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal forStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax, ByVal statements As GreenNode, ByVal nextStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax)
			MyBase.New(kind, errors, annotations, statements, nextStatement)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(forStatement)
			Me._forStatement = forStatement
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim forStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax)
			If (forStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(forStatementSyntax)
				Me._forStatement = forStatementSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitForBlock(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.ForBlockSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._forStatement
					Exit Select
				Case 1
					greenNode = Me._statements
					Exit Select
				Case 2
					greenNode = Me._nextStatement
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._forStatement, Me._statements, Me._nextStatement)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._forStatement, Me._statements, Me._nextStatement)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._forStatement, IObjectWritable))
		End Sub
	End Class
End Namespace