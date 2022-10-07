Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend MustInherit Class RangeVariableSymbol
		Inherits Symbol
		Implements IRangeVariableSymbol
		Friend ReadOnly m_Binder As Binder

		Private ReadOnly _type As TypeSymbol

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me.m_Binder.ContainingMember
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.NotApplicable
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Kind As SymbolKind
			Get
				Return SymbolKind.RangeVariable
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)

		Public Overrides ReadOnly Property Name As String

		Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Public MustOverride ReadOnly Property Syntax As VisualBasicSyntaxNode

		Public Overridable ReadOnly Property Type As TypeSymbol
			Get
				Return Me._type
			End Get
		End Property

		Private Sub New(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal type As TypeSymbol)
			MyBase.New()
			Me.m_Binder = binder
			Me._type = type
		End Sub

		Friend Overrides Function Accept(Of TArgument, TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TArgument, TResult), ByVal arg As TArgument) As TResult
			Return visitor.VisitRangeVariable(Me, arg)
		End Function

		Public Overrides Sub Accept(ByVal visitor As SymbolVisitor)
			visitor.VisitRangeVariable(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As SymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitRangeVariable(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As VisualBasicSymbolVisitor)
			visitor.VisitRangeVariable(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitRangeVariable(Me)
		End Function

		Friend Shared Function Create(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal declaringIdentifier As SyntaxToken, ByVal type As TypeSymbol) As RangeVariableSymbol
			Return New RangeVariableSymbol.WithIdentifierToken(binder, declaringIdentifier, type)
		End Function

		Friend Shared Function CreateCompilerGenerated(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal syntax As VisualBasicSyntaxNode, ByVal name As String, ByVal type As TypeSymbol) As RangeVariableSymbol
			Return New RangeVariableSymbol.CompilerGenerated(binder, syntax, name, type)
		End Function

		Friend Shared Function CreateForErrorRecovery(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal syntax As VisualBasicSyntaxNode, ByVal type As TypeSymbol) As RangeVariableSymbol
			Return New RangeVariableSymbol.ForErrorRecovery(binder, syntax, type)
		End Function

		Private Class CompilerGenerated
			Inherits RangeVariableSymbol.ForErrorRecovery
			Private ReadOnly _name As String

			Public Overrides ReadOnly Property Name As String
				Get
					Return Me._name
				End Get
			End Property

			Public Sub New(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal syntax As VisualBasicSyntaxNode, ByVal name As String, ByVal type As TypeSymbol)
				MyBase.New(binder, syntax, type)
				Me._name = name
			End Sub
		End Class

		Private Class ForErrorRecovery
			Inherits RangeVariableSymbol
			Private ReadOnly _syntax As VisualBasicSyntaxNode

			Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
				Get
					Return ImmutableArray(Of SyntaxReference).Empty
				End Get
			End Property

			Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
				Get
					Return ImmutableArray.Create(Of Location)(Me._syntax.GetLocation())
				End Get
			End Property

			Public Overrides ReadOnly Property Name As String
				Get
					Dim position As Integer = Me._syntax.Position
					Return [String].Concat("$", position.ToString())
				End Get
			End Property

			Public Overrides ReadOnly Property Syntax As VisualBasicSyntaxNode
				Get
					Return Me._syntax
				End Get
			End Property

			Public Sub New(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal syntax As VisualBasicSyntaxNode, ByVal type As TypeSymbol)
				MyBase.New(binder, type)
				Me._syntax = syntax
			End Sub
		End Class

		Private Class WithIdentifierToken
			Inherits RangeVariableSymbol
			Private ReadOnly _identifierToken As SyntaxToken

			Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
				Get
					Dim empty As ImmutableArray(Of SyntaxReference)
					Dim parent As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = Nothing
					Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = Nothing
					Dim parent1 As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode = DirectCast(Me._identifierToken.Parent, Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode)
					If (parent1 IsNot Nothing) Then
						parent = parent1.Parent
					End If
					If (parent IsNot Nothing) Then
						visualBasicSyntaxNode = parent.Parent
					End If
					Dim collectionRangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax = TryCast(parent, Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)
					If (collectionRangeVariableSyntax Is Nothing OrElse Not (Me._identifierToken = collectionRangeVariableSyntax.Identifier.Identifier)) Then
						Dim expressionRangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax = TryCast(visualBasicSyntaxNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)
						If (expressionRangeVariableSyntax Is Nothing OrElse expressionRangeVariableSyntax.NameEquals Is Nothing OrElse Not (expressionRangeVariableSyntax.NameEquals.Identifier.Identifier = Me._identifierToken)) Then
							Dim aggregationRangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax = TryCast(visualBasicSyntaxNode, Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregationRangeVariableSyntax)
							If (aggregationRangeVariableSyntax Is Nothing OrElse aggregationRangeVariableSyntax.NameEquals Is Nothing OrElse Not (aggregationRangeVariableSyntax.NameEquals.Identifier.Identifier = Me._identifierToken)) Then
								empty = ImmutableArray(Of SyntaxReference).Empty
							Else
								empty = ImmutableArray.Create(Of SyntaxReference)(aggregationRangeVariableSyntax.GetReference())
							End If
						Else
							empty = ImmutableArray.Create(Of SyntaxReference)(expressionRangeVariableSyntax.GetReference())
						End If
					Else
						empty = ImmutableArray.Create(Of SyntaxReference)(collectionRangeVariableSyntax.GetReference())
					End If
					Return empty
				End Get
			End Property

			Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
				Get
					Return ImmutableArray.Create(Of Location)(Me._identifierToken.GetLocation())
				End Get
			End Property

			Public Overrides ReadOnly Property Name As String
				Get
					Return Me._identifierToken.GetIdentifierText()
				End Get
			End Property

			Public Overrides ReadOnly Property Syntax As VisualBasicSyntaxNode
				Get
					Return DirectCast(Me._identifierToken.Parent, VisualBasicSyntaxNode)
				End Get
			End Property

			Public Sub New(ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal declaringIdentifier As SyntaxToken, ByVal type As TypeSymbol)
				MyBase.New(binder, type)
				Me._identifierToken = declaringIdentifier
			End Sub

			Public Overrides Function Equals(ByVal obj As Object) As Boolean
				Dim flag As Boolean
				Dim withIdentifierToken As RangeVariableSymbol.WithIdentifierToken = TryCast(obj, RangeVariableSymbol.WithIdentifierToken)
				If (CObj(Me) <> CObj(withIdentifierToken)) Then
					flag = If(withIdentifierToken Is Nothing, False, withIdentifierToken._identifierToken.Equals(Me._identifierToken))
				Else
					flag = True
				End If
				Return flag
			End Function

			Public Overrides Function GetHashCode() As Integer
				Return Me._identifierToken.GetHashCode()
			End Function
		End Class
	End Class
End Namespace