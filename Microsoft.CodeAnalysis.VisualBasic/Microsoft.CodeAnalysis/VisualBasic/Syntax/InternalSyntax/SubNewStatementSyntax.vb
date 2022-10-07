Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class SubNewStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBaseSyntax
		Friend ReadOnly _subKeyword As KeywordSyntax

		Friend ReadOnly _newKeyword As KeywordSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property NewKeyword As KeywordSyntax
			Get
				Return Me._newKeyword
			End Get
		End Property

		Friend ReadOnly Property SubKeyword As KeywordSyntax
			Get
				Return Me._subKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal subKeyword As KeywordSyntax, ByVal newKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)
			MyBase.New(kind, attributeLists, modifiers, parameterList)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(subKeyword)
			Me._subKeyword = subKeyword
			MyBase.AdjustFlagsAndWidth(newKeyword)
			Me._newKeyword = newKeyword
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal subKeyword As KeywordSyntax, ByVal newKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, attributeLists, modifiers, parameterList)
			MyBase._slotCount = 5
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(subKeyword)
			Me._subKeyword = subKeyword
			MyBase.AdjustFlagsAndWidth(newKeyword)
			Me._newKeyword = newKeyword
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal subKeyword As KeywordSyntax, ByVal newKeyword As KeywordSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax)
			MyBase.New(kind, errors, annotations, attributeLists, modifiers, parameterList)
			MyBase._slotCount = 5
			MyBase.AdjustFlagsAndWidth(subKeyword)
			Me._subKeyword = subKeyword
			MyBase.AdjustFlagsAndWidth(newKeyword)
			Me._newKeyword = newKeyword
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 5
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._subKeyword = keywordSyntax
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax1)
				Me._newKeyword = keywordSyntax1
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitSubNewStatement(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.SubNewStatementSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._attributeLists
					Exit Select
				Case 1
					greenNode = Me._modifiers
					Exit Select
				Case 2
					greenNode = Me._subKeyword
					Exit Select
				Case 3
					greenNode = Me._newKeyword
					Exit Select
				Case 4
					greenNode = Me._parameterList
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._attributeLists, Me._modifiers, Me._subKeyword, Me._newKeyword, Me._parameterList)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._attributeLists, Me._modifiers, Me._subKeyword, Me._newKeyword, Me._parameterList)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._subKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._newKeyword, IObjectWritable))
		End Sub
	End Class
End Namespace