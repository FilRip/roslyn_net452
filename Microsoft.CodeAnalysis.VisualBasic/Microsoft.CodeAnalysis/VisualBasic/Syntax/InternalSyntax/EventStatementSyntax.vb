Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class EventStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBaseSyntax
		Friend ReadOnly _customKeyword As KeywordSyntax

		Friend ReadOnly _eventKeyword As KeywordSyntax

		Friend ReadOnly _identifier As IdentifierTokenSyntax

		Friend ReadOnly _asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax

		Friend ReadOnly _implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property AsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax
			Get
				Return Me._asClause
			End Get
		End Property

		Friend ReadOnly Property CustomKeyword As KeywordSyntax
			Get
				Return Me._customKeyword
			End Get
		End Property

		Friend ReadOnly Property EventKeyword As KeywordSyntax
			Get
				Return Me._eventKeyword
			End Get
		End Property

		Friend ReadOnly Property Identifier As IdentifierTokenSyntax
			Get
				Return Me._identifier
			End Get
		End Property

		Friend ReadOnly Property ImplementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax
			Get
				Return Me._implementsClause
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal customKeyword As KeywordSyntax, ByVal eventKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)
			MyBase.New(kind, attributeLists, modifiers, parameterList)
			MyBase._slotCount = 8
			If (customKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(customKeyword)
				Me._customKeyword = customKeyword
			End If
			MyBase.AdjustFlagsAndWidth(eventKeyword)
			Me._eventKeyword = eventKeyword
			MyBase.AdjustFlagsAndWidth(identifier)
			Me._identifier = identifier
			If (asClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(asClause)
				Me._asClause = asClause
			End If
			If (implementsClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(implementsClause)
				Me._implementsClause = implementsClause
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal customKeyword As KeywordSyntax, ByVal eventKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, attributeLists, modifiers, parameterList)
			MyBase._slotCount = 8
			MyBase.SetFactoryContext(context)
			If (customKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(customKeyword)
				Me._customKeyword = customKeyword
			End If
			MyBase.AdjustFlagsAndWidth(eventKeyword)
			Me._eventKeyword = eventKeyword
			MyBase.AdjustFlagsAndWidth(identifier)
			Me._identifier = identifier
			If (asClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(asClause)
				Me._asClause = asClause
			End If
			If (implementsClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(implementsClause)
				Me._implementsClause = implementsClause
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal customKeyword As KeywordSyntax, ByVal eventKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)
			MyBase.New(kind, errors, annotations, attributeLists, modifiers, parameterList)
			MyBase._slotCount = 8
			If (customKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(customKeyword)
				Me._customKeyword = customKeyword
			End If
			MyBase.AdjustFlagsAndWidth(eventKeyword)
			Me._eventKeyword = eventKeyword
			MyBase.AdjustFlagsAndWidth(identifier)
			Me._identifier = identifier
			If (asClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(asClause)
				Me._asClause = asClause
			End If
			If (implementsClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(implementsClause)
				Me._implementsClause = implementsClause
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 8
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._customKeyword = keywordSyntax
			End If
			Dim keywordSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax1 IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax1)
				Me._eventKeyword = keywordSyntax1
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (identifierTokenSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(identifierTokenSyntax)
				Me._identifier = identifierTokenSyntax
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			If (simpleAsClauseSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(simpleAsClauseSyntax)
				Me._asClause = simpleAsClauseSyntax
			End If
			Dim implementsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)
			If (implementsClauseSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(implementsClauseSyntax)
				Me._implementsClause = implementsClauseSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitEventStatement(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.EventStatementSyntax(Me, parent, startLocation)
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
					greenNode = Me._customKeyword
					Exit Select
				Case 3
					greenNode = Me._eventKeyword
					Exit Select
				Case 4
					greenNode = Me._identifier
					Exit Select
				Case 5
					greenNode = Me._parameterList
					Exit Select
				Case 6
					greenNode = Me._asClause
					Exit Select
				Case 7
					greenNode = Me._implementsClause
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._attributeLists, Me._modifiers, Me._customKeyword, Me._eventKeyword, Me._identifier, Me._parameterList, Me._asClause, Me._implementsClause)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._attributeLists, Me._modifiers, Me._customKeyword, Me._eventKeyword, Me._identifier, Me._parameterList, Me._asClause, Me._implementsClause)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._customKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._eventKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._identifier, IObjectWritable))
			writer.WriteValue(DirectCast(Me._asClause, IObjectWritable))
			writer.WriteValue(DirectCast(Me._implementsClause, IObjectWritable))
		End Sub
	End Class
End Namespace