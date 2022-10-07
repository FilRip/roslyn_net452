Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class OptionStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclarationStatementSyntax
		Friend ReadOnly _optionKeyword As KeywordSyntax

		Friend ReadOnly _nameKeyword As KeywordSyntax

		Friend ReadOnly _valueKeyword As KeywordSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property NameKeyword As KeywordSyntax
			Get
				Return Me._nameKeyword
			End Get
		End Property

		Friend ReadOnly Property OptionKeyword As KeywordSyntax
			Get
				Return Me._optionKeyword
			End Get
		End Property

		Friend ReadOnly Property ValueKeyword As KeywordSyntax
			Get
				Return Me._valueKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal optionKeyword As KeywordSyntax, ByVal nameKeyword As KeywordSyntax, ByVal valueKeyword As KeywordSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(optionKeyword)
			Me._optionKeyword = optionKeyword
			MyBase.AdjustFlagsAndWidth(nameKeyword)
			Me._nameKeyword = nameKeyword
			If (valueKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(valueKeyword)
				Me._valueKeyword = valueKeyword
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal optionKeyword As KeywordSyntax, ByVal nameKeyword As KeywordSyntax, ByVal valueKeyword As KeywordSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(optionKeyword)
			Me._optionKeyword = optionKeyword
			MyBase.AdjustFlagsAndWidth(nameKeyword)
			Me._nameKeyword = nameKeyword
			If (valueKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(valueKeyword)
				Me._valueKeyword = valueKeyword
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal optionKeyword As KeywordSyntax, ByVal nameKeyword As KeywordSyntax, ByVal valueKeyword As KeywordSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			MyBase.AdjustFlagsAndWidth(optionKeyword)
			Me._optionKeyword = optionKeyword
			MyBase.AdjustFlagsAndWidth(nameKeyword)
			Me._nameKeyword = nameKeyword
			If (valueKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(valueKeyword)
				Me._valueKeyword = valueKeyword
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._optionKeyword = keywordSyntax
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax1)
				Me._nameKeyword = keywordSyntax1
			End If
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax2)
				Me._valueKeyword = keywordSyntax2
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitOptionStatement(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.OptionStatementSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._optionKeyword
					Exit Select
				Case 1
					greenNode = Me._nameKeyword
					Exit Select
				Case 2
					greenNode = Me._valueKeyword
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._optionKeyword, Me._nameKeyword, Me._valueKeyword)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._optionKeyword, Me._nameKeyword, Me._valueKeyword)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._optionKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._nameKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._valueKeyword, IObjectWritable))
		End Sub
	End Class
End Namespace