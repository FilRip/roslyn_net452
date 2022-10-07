Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class MethodStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBaseSyntax
		Friend ReadOnly _subOrFunctionKeyword As KeywordSyntax

		Friend ReadOnly _identifier As IdentifierTokenSyntax

		Friend ReadOnly _typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax

		Friend ReadOnly _asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax

		Friend ReadOnly _handlesClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax

		Friend ReadOnly _implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property AsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax
			Get
				Return Me._asClause
			End Get
		End Property

		Friend ReadOnly Property HandlesClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax
			Get
				Return Me._handlesClause
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

		Friend ReadOnly Property SubOrFunctionKeyword As KeywordSyntax
			Get
				Return Me._subOrFunctionKeyword
			End Get
		End Property

		Friend ReadOnly Property TypeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax
			Get
				Return Me._typeParameterList
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal handlesClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)
			MyBase.New(kind, attributeLists, modifiers, parameterList)
			MyBase._slotCount = 9
			MyBase.AdjustFlagsAndWidth(subOrFunctionKeyword)
			Me._subOrFunctionKeyword = subOrFunctionKeyword
			MyBase.AdjustFlagsAndWidth(identifier)
			Me._identifier = identifier
			If (typeParameterList IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(typeParameterList)
				Me._typeParameterList = typeParameterList
			End If
			If (asClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(asClause)
				Me._asClause = asClause
			End If
			If (handlesClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(handlesClause)
				Me._handlesClause = handlesClause
			End If
			If (implementsClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(implementsClause)
				Me._implementsClause = implementsClause
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal handlesClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, attributeLists, modifiers, parameterList)
			MyBase._slotCount = 9
			MyBase.SetFactoryContext(context)
			MyBase.AdjustFlagsAndWidth(subOrFunctionKeyword)
			Me._subOrFunctionKeyword = subOrFunctionKeyword
			MyBase.AdjustFlagsAndWidth(identifier)
			Me._identifier = identifier
			If (typeParameterList IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(typeParameterList)
				Me._typeParameterList = typeParameterList
			End If
			If (asClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(asClause)
				Me._asClause = asClause
			End If
			If (handlesClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(handlesClause)
				Me._handlesClause = handlesClause
			End If
			If (implementsClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(implementsClause)
				Me._implementsClause = implementsClause
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal attributeLists As GreenNode, ByVal modifiers As GreenNode, ByVal subOrFunctionKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax, ByVal parameterList As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax, ByVal asClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax, ByVal handlesClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax, ByVal implementsClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)
			MyBase.New(kind, errors, annotations, attributeLists, modifiers, parameterList)
			MyBase._slotCount = 9
			MyBase.AdjustFlagsAndWidth(subOrFunctionKeyword)
			Me._subOrFunctionKeyword = subOrFunctionKeyword
			MyBase.AdjustFlagsAndWidth(identifier)
			Me._identifier = identifier
			If (typeParameterList IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(typeParameterList)
				Me._typeParameterList = typeParameterList
			End If
			If (asClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(asClause)
				Me._asClause = asClause
			End If
			If (handlesClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(handlesClause)
				Me._handlesClause = handlesClause
			End If
			If (implementsClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(implementsClause)
				Me._implementsClause = implementsClause
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 9
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._subOrFunctionKeyword = keywordSyntax
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (identifierTokenSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(identifierTokenSyntax)
				Me._identifier = identifierTokenSyntax
			End If
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax)
			If (typeParameterListSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(typeParameterListSyntax)
				Me._typeParameterList = typeParameterListSyntax
			End If
			Dim simpleAsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax)
			If (simpleAsClauseSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(simpleAsClauseSyntax)
				Me._asClause = simpleAsClauseSyntax
			End If
			Dim handlesClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax)
			If (handlesClauseSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(handlesClauseSyntax)
				Me._handlesClause = handlesClauseSyntax
			End If
			Dim implementsClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax)
			If (implementsClauseSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(implementsClauseSyntax)
				Me._implementsClause = implementsClauseSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitMethodStatement(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodStatementSyntax(Me, parent, startLocation)
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
					greenNode = Me._subOrFunctionKeyword
					Exit Select
				Case 3
					greenNode = Me._identifier
					Exit Select
				Case 4
					greenNode = Me._typeParameterList
					Exit Select
				Case 5
					greenNode = Me._parameterList
					Exit Select
				Case 6
					greenNode = Me._asClause
					Exit Select
				Case 7
					greenNode = Me._handlesClause
					Exit Select
				Case 8
					greenNode = Me._implementsClause
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._attributeLists, Me._modifiers, Me._subOrFunctionKeyword, Me._identifier, Me._typeParameterList, Me._parameterList, Me._asClause, Me._handlesClause, Me._implementsClause)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._attributeLists, Me._modifiers, Me._subOrFunctionKeyword, Me._identifier, Me._typeParameterList, Me._parameterList, Me._asClause, Me._handlesClause, Me._implementsClause)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._subOrFunctionKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._identifier, IObjectWritable))
			writer.WriteValue(DirectCast(Me._typeParameterList, IObjectWritable))
			writer.WriteValue(DirectCast(Me._asClause, IObjectWritable))
			writer.WriteValue(DirectCast(Me._handlesClause, IObjectWritable))
			writer.WriteValue(DirectCast(Me._implementsClause, IObjectWritable))
		End Sub
	End Class
End Namespace