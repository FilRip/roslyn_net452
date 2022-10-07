Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class TypeParameterSingleConstraintClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax
		Friend ReadOnly _asKeyword As KeywordSyntax

		Friend ReadOnly _constraint As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property AsKeyword As KeywordSyntax
			Get
				Return Me._asKeyword
			End Get
		End Property

		Friend ReadOnly Property Constraint As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax
			Get
				Return Me._constraint
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal asKeyword As KeywordSyntax, ByVal constraint As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 2
			MyBase.AdjustFlagsAndWidth(asKeyword)
			Me._asKeyword = asKeyword
			MyBase.AdjustFlagsAndWidth(constraint)
			Me._constraint = constraint
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal asKeyword As KeywordSyntax, ByVal constraint As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 2
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(asKeyword)
			Me._asKeyword = asKeyword
			MyBase.AdjustFlagsAndWidth(constraint)
			Me._constraint = constraint
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal asKeyword As KeywordSyntax, ByVal constraint As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 2
			MyBase.AdjustFlagsAndWidth(asKeyword)
			Me._asKeyword = asKeyword
			MyBase.AdjustFlagsAndWidth(constraint)
			Me._constraint = constraint
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 2
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._asKeyword = keywordSyntax
			End If
			Dim constraintSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax)
			If (constraintSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(constraintSyntax)
				Me._constraint = constraintSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitTypeParameterSingleConstraintClause(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSingleConstraintClauseSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Dim num As Integer = i
			If (num = 0) Then
				greenNode = Me._asKeyword
			ElseIf (num = 1) Then
				greenNode = Me._constraint
			Else
				greenNode = Nothing
			End If
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._asKeyword, Me._constraint)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._asKeyword, Me._constraint)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._asKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._constraint, IObjectWritable))
		End Sub
	End Class
End Namespace