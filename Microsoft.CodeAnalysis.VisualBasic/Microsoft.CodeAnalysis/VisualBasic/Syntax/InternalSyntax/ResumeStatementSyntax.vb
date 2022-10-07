Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class ResumeStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax
		Friend ReadOnly _resumeKeyword As KeywordSyntax

		Friend ReadOnly _label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax
			Get
				Return Me._label
			End Get
		End Property

		Friend ReadOnly Property ResumeKeyword As KeywordSyntax
			Get
				Return Me._resumeKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal resumeKeyword As KeywordSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 2
			MyBase.AdjustFlagsAndWidth(resumeKeyword)
			Me._resumeKeyword = resumeKeyword
			If (label IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(label)
				Me._label = label
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal resumeKeyword As KeywordSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 2
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(resumeKeyword)
			Me._resumeKeyword = resumeKeyword
			If (label IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(label)
				Me._label = label
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal resumeKeyword As KeywordSyntax, ByVal label As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 2
			MyBase.AdjustFlagsAndWidth(resumeKeyword)
			Me._resumeKeyword = resumeKeyword
			If (label IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(label)
				Me._label = label
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 2
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._resumeKeyword = keywordSyntax
			End If
			Dim labelSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax)
			If (labelSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(labelSyntax)
				Me._label = labelSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitResumeStatement(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.ResumeStatementSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Dim num As Integer = i
			If (num = 0) Then
				greenNode = Me._resumeKeyword
			ElseIf (num = 1) Then
				greenNode = Me._label
			Else
				greenNode = Nothing
			End If
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._resumeKeyword, Me._label)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._resumeKeyword, Me._label)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._resumeKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._label, IObjectWritable))
		End Sub
	End Class
End Namespace