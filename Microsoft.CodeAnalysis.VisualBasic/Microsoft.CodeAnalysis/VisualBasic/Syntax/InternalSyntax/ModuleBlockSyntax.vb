Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class ModuleBlockSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeBlockSyntax
		Friend ReadOnly _moduleStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax

		Friend ReadOnly _endModuleStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Public Overrides ReadOnly Property BlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax
			Get
				Return Me.ModuleStatement
			End Get
		End Property

		Public Overrides ReadOnly Property EndBlockStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Get
				Return Me.EndModuleStatement
			End Get
		End Property

		Friend ReadOnly Property EndModuleStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax
			Get
				Return Me._endModuleStatement
			End Get
		End Property

		Friend ReadOnly Property ModuleStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax
			Get
				Return Me._moduleStatement
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleBlockSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleBlockSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleBlockSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleBlockSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal moduleStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax, ByVal [inherits] As GreenNode, ByVal [implements] As GreenNode, ByVal members As GreenNode, ByVal endModuleStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.New(kind, [inherits], [implements], members)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(moduleStatement)
			Me._moduleStatement = moduleStatement
			MyBase.AdjustFlagsAndWidth(endModuleStatement)
			Me._endModuleStatement = endModuleStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal moduleStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax, ByVal [inherits] As GreenNode, ByVal [implements] As GreenNode, ByVal members As GreenNode, ByVal endModuleStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, [inherits], [implements], members)
			MyBase._slotCount = 5
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(moduleStatement)
			Me._moduleStatement = moduleStatement
			MyBase.AdjustFlagsAndWidth(endModuleStatement)
			Me._endModuleStatement = endModuleStatement
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal moduleStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax, ByVal [inherits] As GreenNode, ByVal [implements] As GreenNode, ByVal members As GreenNode, ByVal endModuleStatement As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			MyBase.New(kind, errors, annotations, [inherits], [implements], members)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(moduleStatement)
			Me._moduleStatement = moduleStatement
			MyBase.AdjustFlagsAndWidth(endModuleStatement)
			Me._endModuleStatement = endModuleStatement
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 5
			Dim moduleStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax)
			If (moduleStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(moduleStatementSyntax)
				Me._moduleStatement = moduleStatementSyntax
			End If
			Dim endBlockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax)
			If (endBlockStatementSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(endBlockStatementSyntax)
				Me._endModuleStatement = endBlockStatementSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitModuleBlock(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.ModuleBlockSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._moduleStatement
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
					greenNode = Me._endModuleStatement
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleBlockSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._moduleStatement, Me._inherits, Me._implements, Me._members, Me._endModuleStatement)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleBlockSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._moduleStatement, Me._inherits, Me._implements, Me._members, Me._endModuleStatement)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._moduleStatement, IObjectWritable))
			writer.WriteValue(DirectCast(Me._endModuleStatement, IObjectWritable))
		End Sub
	End Class
End Namespace