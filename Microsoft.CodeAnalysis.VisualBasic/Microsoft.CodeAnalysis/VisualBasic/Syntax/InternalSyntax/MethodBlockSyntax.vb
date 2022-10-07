Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class MethodBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockBaseSyntax
		Friend ReadOnly _subOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax

		Friend ReadOnly _endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Public Overrides ReadOnly Property Begin As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBaseSyntax
			Get
				Return Me.SubOrFunctionStatement
			End Get
		End Property

		Public Overrides ReadOnly Property [End] As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Get
				Return Me.EndSubOrFunctionStatement
			End Get
		End Property

		Friend ReadOnly Property EndSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Get
				Return Me._endSubOrFunctionStatement
			End Get
		End Property

		Friend ReadOnly Property SubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax
			Get
				Return Me._subOrFunctionStatement
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal subOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax, ByVal statements As GreenNode, ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.New(kind, statements)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(subOrFunctionStatement)
			Me._subOrFunctionStatement = subOrFunctionStatement
			MyBase.AdjustFlagsAndWidth(endSubOrFunctionStatement)
			Me._endSubOrFunctionStatement = endSubOrFunctionStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal subOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax, ByVal statements As GreenNode, ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, statements)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(subOrFunctionStatement)
			Me._subOrFunctionStatement = subOrFunctionStatement
			MyBase.AdjustFlagsAndWidth(endSubOrFunctionStatement)
			Me._endSubOrFunctionStatement = endSubOrFunctionStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal subOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax, ByVal statements As GreenNode, ByVal endSubOrFunctionStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.New(kind, errors, annotations, statements)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(subOrFunctionStatement)
			Me._subOrFunctionStatement = subOrFunctionStatement
			MyBase.AdjustFlagsAndWidth(endSubOrFunctionStatement)
			Me._endSubOrFunctionStatement = endSubOrFunctionStatement
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim methodStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax)
			If (methodStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(methodStatementSyntax)
				Me._subOrFunctionStatement = methodStatementSyntax
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (endBlockStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(endBlockStatementSyntax)
				Me._endSubOrFunctionStatement = endBlockStatementSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitMethodBlock(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._subOrFunctionStatement
					Exit Select
				Case 1
					greenNode = Me._statements
					Exit Select
				Case 2
					greenNode = Me._endSubOrFunctionStatement
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._subOrFunctionStatement, Me._statements, Me._endSubOrFunctionStatement)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._subOrFunctionStatement, Me._statements, Me._endSubOrFunctionStatement)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._subOrFunctionStatement, IObjectWritable))
			writer.WriteValue(DirectCast(Me._endSubOrFunctionStatement, IObjectWritable))
		End Sub
	End Class
End Namespace