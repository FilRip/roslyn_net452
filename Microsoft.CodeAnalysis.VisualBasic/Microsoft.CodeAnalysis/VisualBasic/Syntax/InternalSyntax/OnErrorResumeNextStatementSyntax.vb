Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class OnErrorResumeNextStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax
		Friend ReadOnly _onKeyword As KeywordSyntax

		Friend ReadOnly _errorKeyword As KeywordSyntax

		Friend ReadOnly _resumeKeyword As KeywordSyntax

		Friend ReadOnly _nextKeyword As KeywordSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property ErrorKeyword As KeywordSyntax
			Get
				Return Me._errorKeyword
			End Get
		End Property

		Friend ReadOnly Property NextKeyword As KeywordSyntax
			Get
				Return Me._nextKeyword
			End Get
		End Property

		Friend ReadOnly Property OnKeyword As KeywordSyntax
			Get
				Return Me._onKeyword
			End Get
		End Property

		Friend ReadOnly Property ResumeKeyword As KeywordSyntax
			Get
				Return Me._resumeKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal onKeyword As KeywordSyntax, ByVal errorKeyword As KeywordSyntax, ByVal resumeKeyword As KeywordSyntax, ByVal nextKeyword As KeywordSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(onKeyword)
			Me._onKeyword = onKeyword
			MyBase.AdjustFlagsAndWidth(errorKeyword)
			Me._errorKeyword = errorKeyword
			MyBase.AdjustFlagsAndWidth(resumeKeyword)
			Me._resumeKeyword = resumeKeyword
			MyBase.AdjustFlagsAndWidth(nextKeyword)
			Me._nextKeyword = nextKeyword
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal onKeyword As KeywordSyntax, ByVal errorKeyword As KeywordSyntax, ByVal resumeKeyword As KeywordSyntax, ByVal nextKeyword As KeywordSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 4
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(onKeyword)
			Me._onKeyword = onKeyword
			MyBase.AdjustFlagsAndWidth(errorKeyword)
			Me._errorKeyword = errorKeyword
			MyBase.AdjustFlagsAndWidth(resumeKeyword)
			Me._resumeKeyword = resumeKeyword
			MyBase.AdjustFlagsAndWidth(nextKeyword)
			Me._nextKeyword = nextKeyword
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal onKeyword As KeywordSyntax, ByVal errorKeyword As KeywordSyntax, ByVal resumeKeyword As KeywordSyntax, ByVal nextKeyword As KeywordSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 4
			MyBase.AdjustFlagsAndWidth(onKeyword)
			Me._onKeyword = onKeyword
			MyBase.AdjustFlagsAndWidth(errorKeyword)
			Me._errorKeyword = errorKeyword
			MyBase.AdjustFlagsAndWidth(resumeKeyword)
			Me._resumeKeyword = resumeKeyword
			MyBase.AdjustFlagsAndWidth(nextKeyword)
			Me._nextKeyword = nextKeyword
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 4
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._onKeyword = keywordSyntax
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax1)
				Me._errorKeyword = keywordSyntax1
			End If
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax2)
				Me._resumeKeyword = keywordSyntax2
			End If
			Dim keywordSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax3 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax3)
				Me._nextKeyword = keywordSyntax3
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitOnErrorResumeNextStatement(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.OnErrorResumeNextStatementSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._onKeyword
					Exit Select
				Case 1
					greenNode = Me._errorKeyword
					Exit Select
				Case 2
					greenNode = Me._resumeKeyword
					Exit Select
				Case 3
					greenNode = Me._nextKeyword
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._onKeyword, Me._errorKeyword, Me._resumeKeyword, Me._nextKeyword)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._onKeyword, Me._errorKeyword, Me._resumeKeyword, Me._nextKeyword)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._onKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._errorKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._resumeKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._nextKeyword, IObjectWritable))
		End Sub
	End Class
End Namespace