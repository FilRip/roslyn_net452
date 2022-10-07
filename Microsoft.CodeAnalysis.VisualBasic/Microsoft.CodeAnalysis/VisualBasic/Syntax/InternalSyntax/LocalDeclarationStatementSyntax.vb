Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Syntax.InternalSyntax
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class LocalDeclarationStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax
		Friend ReadOnly _modifiers As GreenNode

		Friend ReadOnly _declarators As GreenNode

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Declarators As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax)(New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax)(Me._declarators))
			End Get
		End Property

		Friend ReadOnly Property Modifiers As Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of KeywordSyntax)
			Get
				Return New Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList(Of GreenNode)(Me._modifiers)
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal modifiers As GreenNode, ByVal declarators As GreenNode)
			MyBase.New(kind)
			MyBase._slotCount = 2
			If (modifiers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(modifiers)
				Me._modifiers = modifiers
			End If
			If (declarators IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(declarators)
				Me._declarators = declarators
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal modifiers As GreenNode, ByVal declarators As GreenNode, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 2
			MyBase.SetFactoryContext(context)
			If (modifiers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(modifiers)
				Me._modifiers = modifiers
			End If
			If (declarators IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(declarators)
				Me._declarators = declarators
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal modifiers As GreenNode, ByVal declarators As GreenNode)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 2
			If (modifiers IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(modifiers)
				Me._modifiers = modifiers
			End If
			If (declarators IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(declarators)
				Me._declarators = declarators
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 2
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode)
				Me._modifiers = greenNode
			End If
			Dim greenNode1 As Microsoft.CodeAnalysis.GreenNode = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.GreenNode)
			If (greenNode1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(greenNode1)
				Me._declarators = greenNode1
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitLocalDeclarationStatement(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.LocalDeclarationStatementSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Dim num As Integer = i
			If (num = 0) Then
				greenNode = Me._modifiers
			ElseIf (num = 1) Then
				greenNode = Me._declarators
			Else
				greenNode = Nothing
			End If
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._modifiers, Me._declarators)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._modifiers, Me._declarators)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._modifiers, IObjectWritable))
			writer.WriteValue(DirectCast(Me._declarators, IObjectWritable))
		End Sub
	End Class
End Namespace