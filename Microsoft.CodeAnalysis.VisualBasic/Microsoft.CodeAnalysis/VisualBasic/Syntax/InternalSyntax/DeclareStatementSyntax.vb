Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class DeclareStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBaseSyntax
		Friend ReadOnly _declareKeyword As KeywordSyntax

		Friend ReadOnly _charsetKeyword As KeywordSyntax

		Friend ReadOnly _subOrFunctionKeyword As KeywordSyntax

		Friend ReadOnly _identifier As IdentifierTokenSyntax

		Friend ReadOnly _libKeyword As KeywordSyntax

		Friend ReadOnly _libraryName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax

		Friend ReadOnly _aliasKeyword As KeywordSyntax

		Friend ReadOnly _aliasName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax

		Friend ReadOnly _asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property AliasKeyword As KeywordSyntax
			Get
				Return Me._aliasKeyword
			End Get
		End Property

		Friend ReadOnly Property AliasName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Get
				Return Me._aliasName
			End Get
		End Property

		Friend ReadOnly Property AsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax
			Get
				Return Me._asClause
			End Get
		End Property

		Friend ReadOnly Property CharsetKeyword As KeywordSyntax
			Get
				Return Me._charsetKeyword
			End Get
		End Property

		Friend ReadOnly Property DeclareKeyword As KeywordSyntax
			Get
				Return Me._declareKeyword
			End Get
		End Property

		Friend ReadOnly Property Identifier As IdentifierTokenSyntax
			Get
				Return Me._identifier
			End Get
		End Property

		Friend ReadOnly Property LibKeyword As KeywordSyntax
			Get
				Return Me._libKeyword
			End Get
		End Property

		Friend ReadOnly Property LibraryName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax
			Get
				Return Me._libraryName
			End Get
		End Property

		Friend ReadOnly Property SubOrFunctionKeyword As KeywordSyntax
			Get
				Return Me._subOrFunctionKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal declareKeyword As KeywordSyntax, ByVal charsetKeyword As KeywordSyntax, ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal libKeyword As KeywordSyntax, ByVal libraryName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax, ByVal aliasKeyword As KeywordSyntax, ByVal aliasName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			MyBase.New(kind, attributeLists, modifiers, parameterList)
			MyBase._slotCount = 12
			MyBase.AdjustFlagsAndWidth(declareKeyword)
			Me._declareKeyword = declareKeyword
			If (charsetKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(charsetKeyword)
				Me._charsetKeyword = charsetKeyword
			End If
			MyBase.AdjustFlagsAndWidth(subOrFunctionKeyword)
			Me._subOrFunctionKeyword = subOrFunctionKeyword
			MyBase.AdjustFlagsAndWidth(identifier)
			Me._identifier = identifier
			MyBase.AdjustFlagsAndWidth(libKeyword)
			Me._libKeyword = libKeyword
			MyBase.AdjustFlagsAndWidth(libraryName)
			Me._libraryName = libraryName
			If (aliasKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(aliasKeyword)
				Me._aliasKeyword = aliasKeyword
			End If
			If (aliasName IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(aliasName)
				Me._aliasName = aliasName
			End If
			If (asClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(asClause)
				Me._asClause = asClause
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal declareKeyword As KeywordSyntax, ByVal charsetKeyword As KeywordSyntax, ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal libKeyword As KeywordSyntax, ByVal libraryName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax, ByVal aliasKeyword As KeywordSyntax, ByVal aliasName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, attributeLists, modifiers, parameterList)
			MyBase._slotCount = 12
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(declareKeyword)
			Me._declareKeyword = declareKeyword
			If (charsetKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(charsetKeyword)
				Me._charsetKeyword = charsetKeyword
			End If
			MyBase.AdjustFlagsAndWidth(subOrFunctionKeyword)
			Me._subOrFunctionKeyword = subOrFunctionKeyword
			MyBase.AdjustFlagsAndWidth(identifier)
			Me._identifier = identifier
			MyBase.AdjustFlagsAndWidth(libKeyword)
			Me._libKeyword = libKeyword
			MyBase.AdjustFlagsAndWidth(libraryName)
			Me._libraryName = libraryName
			If (aliasKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(aliasKeyword)
				Me._aliasKeyword = aliasKeyword
			End If
			If (aliasName IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(aliasName)
				Me._aliasName = aliasName
			End If
			If (asClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(asClause)
				Me._asClause = asClause
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal declareKeyword As KeywordSyntax, ByVal charsetKeyword As KeywordSyntax, ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal libKeyword As KeywordSyntax, ByVal libraryName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax, ByVal aliasKeyword As KeywordSyntax, ByVal aliasName As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			MyBase.New(kind, errors, annotations, attributeLists, modifiers, parameterList)
			MyBase._slotCount = 12
			MyBase.AdjustFlagsAndWidth(declareKeyword)
			Me._declareKeyword = declareKeyword
			If (charsetKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(charsetKeyword)
				Me._charsetKeyword = charsetKeyword
			End If
			MyBase.AdjustFlagsAndWidth(subOrFunctionKeyword)
			Me._subOrFunctionKeyword = subOrFunctionKeyword
			MyBase.AdjustFlagsAndWidth(identifier)
			Me._identifier = identifier
			MyBase.AdjustFlagsAndWidth(libKeyword)
			Me._libKeyword = libKeyword
			MyBase.AdjustFlagsAndWidth(libraryName)
			Me._libraryName = libraryName
			If (aliasKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(aliasKeyword)
				Me._aliasKeyword = aliasKeyword
			End If
			If (aliasName IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(aliasName)
				Me._aliasName = aliasName
			End If
			If (asClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(asClause)
				Me._asClause = asClause
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 12
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._declareKeyword = keywordSyntax
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax1)
				Me._charsetKeyword = keywordSyntax1
			End If
			Dim keywordSyntax2 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax2 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax2)
				Me._subOrFunctionKeyword = keywordSyntax2
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (identifierTokenSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(identifierTokenSyntax)
				Me._identifier = identifierTokenSyntax
			End If
			Dim keywordSyntax3 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax3 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax3)
				Me._libKeyword = keywordSyntax3
			End If
			Dim literalExpressionSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			If (literalExpressionSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(literalExpressionSyntax)
				Me._libraryName = literalExpressionSyntax
			End If
			Dim keywordSyntax4 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax4 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax4)
				Me._aliasKeyword = keywordSyntax4
			End If
			Dim literalExpressionSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax)
			If (literalExpressionSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(literalExpressionSyntax1)
				Me._aliasName = literalExpressionSyntax1
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			If (simpleAsClauseSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(simpleAsClauseSyntax)
				Me._asClause = simpleAsClauseSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitDeclareStatement(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.DeclareStatementSyntax(Me, parent, startLocation)
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
					greenNode = Me._declareKeyword
					Exit Select
				Case 3
					greenNode = Me._charsetKeyword
					Exit Select
				Case 4
					greenNode = Me._subOrFunctionKeyword
					Exit Select
				Case 5
					greenNode = Me._identifier
					Exit Select
				Case 6
					greenNode = Me._libKeyword
					Exit Select
				Case 7
					greenNode = Me._libraryName
					Exit Select
				Case 8
					greenNode = Me._aliasKeyword
					Exit Select
				Case 9
					greenNode = Me._aliasName
					Exit Select
				Case 10
					greenNode = Me._parameterList
					Exit Select
				Case 11
					greenNode = Me._asClause
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._attributeLists, Me._modifiers, Me._declareKeyword, Me._charsetKeyword, Me._subOrFunctionKeyword, Me._identifier, Me._libKeyword, Me._libraryName, Me._aliasKeyword, Me._aliasName, Me._parameterList, Me._asClause)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._attributeLists, Me._modifiers, Me._declareKeyword, Me._charsetKeyword, Me._subOrFunctionKeyword, Me._identifier, Me._libKeyword, Me._libraryName, Me._aliasKeyword, Me._aliasName, Me._parameterList, Me._asClause)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._declareKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._charsetKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._subOrFunctionKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._identifier, IObjectWritable))
			writer.WriteValue(DirectCast(Me._libKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._libraryName, IObjectWritable))
			writer.WriteValue(DirectCast(Me._aliasKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._aliasName, IObjectWritable))
			writer.WriteValue(DirectCast(Me._asClause, IObjectWritable))
		End Sub
	End Class
End Namespace