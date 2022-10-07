Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class TypeParameterSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
		Friend ReadOnly _varianceKeyword As KeywordSyntax

		Friend ReadOnly _identifier As IdentifierTokenSyntax

		Friend ReadOnly _typeParameterConstraintClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax

		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend ReadOnly Property Identifier As IdentifierTokenSyntax
			Get
				Return Me._identifier
			End Get
		End Property

		Friend ReadOnly Property TypeParameterConstraintClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax
			Get
				Return Me._typeParameterConstraintClause
			End Get
		End Property

		Friend ReadOnly Property VarianceKeyword As KeywordSyntax
			Get
				Return Me._varianceKeyword
			End Get
		End Property

		Shared Sub New()
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax.CreateInstance = Function(o As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax), Function(r As ObjectReader) New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal varianceKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterConstraintClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax)
			MyBase.New(kind)
			MyBase._slotCount = 3
			If (varianceKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(varianceKeyword)
				Me._varianceKeyword = varianceKeyword
			End If
			MyBase.AdjustFlagsAndWidth(identifier)
			Me._identifier = identifier
			If (typeParameterConstraintClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(typeParameterConstraintClause)
				Me._typeParameterConstraintClause = typeParameterConstraintClause
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal varianceKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterConstraintClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind)
			MyBase._slotCount = 3
			MyBase.SetFactoryContext(context)
			If (varianceKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(varianceKeyword)
				Me._varianceKeyword = varianceKeyword
			End If
			MyBase.AdjustFlagsAndWidth(identifier)
			Me._identifier = identifier
			If (typeParameterConstraintClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(typeParameterConstraintClause)
				Me._typeParameterConstraintClause = typeParameterConstraintClause
			End If
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal varianceKeyword As KeywordSyntax, ByVal identifier As IdentifierTokenSyntax, ByVal typeParameterConstraintClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax)
			MyBase.New(kind, errors, annotations)
			MyBase._slotCount = 3
			If (varianceKeyword IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(varianceKeyword)
				Me._varianceKeyword = varianceKeyword
			End If
			MyBase.AdjustFlagsAndWidth(identifier)
			Me._identifier = identifier
			If (typeParameterConstraintClause IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(typeParameterConstraintClause)
				Me._typeParameterConstraintClause = typeParameterConstraintClause
			End If
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			MyBase._slotCount = 3
			Dim keywordSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax)
			If (keywordSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(keywordSyntax)
				Me._varianceKeyword = keywordSyntax
			End If
			Dim identifierTokenSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierTokenSyntax)
			If (identifierTokenSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(identifierTokenSyntax)
				Me._identifier = identifierTokenSyntax
			End If
			Dim typeParameterConstraintClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax = DirectCast(reader.ReadValue(), Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax)
			If (typeParameterConstraintClauseSyntax IsNot Nothing) Then
				MyBase.AdjustFlagsAndWidth(typeParameterConstraintClauseSyntax)
				Me._typeParameterConstraintClause = typeParameterConstraintClauseSyntax
			End If
		End Sub

		Public Overrides Function Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxVisitor) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return visitor.VisitTypeParameter(Me)
		End Function

		Friend Overrides Function CreateRed(ByVal parent As SyntaxNode, ByVal startLocation As Integer) As SyntaxNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterSyntax(Me, parent, startLocation)
		End Function

		Friend Overrides Function GetSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.GreenNode
			Dim greenNode As Microsoft.CodeAnalysis.GreenNode
			Select Case i
				Case 0
					greenNode = Me._varianceKeyword
					Exit Select
				Case 1
					greenNode = Me._identifier
					Exit Select
				Case 2
					greenNode = Me._typeParameterConstraintClause
					Exit Select
				Case Else
					greenNode = Nothing
					Exit Select
			End Select
			Return greenNode
		End Function

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, Me._varianceKeyword, Me._identifier, Me._typeParameterConstraintClause)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), Me._varianceKeyword, Me._identifier, Me._typeParameterConstraintClause)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(DirectCast(Me._varianceKeyword, IObjectWritable))
			writer.WriteValue(DirectCast(Me._identifier, IObjectWritable))
			writer.WriteValue(DirectCast(Me._typeParameterConstraintClause, IObjectWritable))
		End Sub
	End Class
End Namespace