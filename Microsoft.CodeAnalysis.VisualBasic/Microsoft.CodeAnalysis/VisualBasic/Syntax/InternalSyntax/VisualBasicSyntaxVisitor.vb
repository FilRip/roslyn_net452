Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend MustInherit Class VisualBasicSyntaxVisitor
		Protected Sub New()
			MyBase.New()
		End Sub

		Public Overridable Function Visit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Dim visualBasicSyntaxNode As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			If (node Is Nothing) Then
				visualBasicSyntaxNode = Nothing
			Else
				visualBasicSyntaxNode = node.Accept(Me)
			End If
			Return visualBasicSyntaxNode
		End Function

		Public Overridable Function VisitAccessorBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitMethodBlockBase(node)
		End Function

		Public Overridable Function VisitAccessorStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AccessorStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitMethodBase(node)
		End Function

		Public Overridable Function VisitAddRemoveHandlerStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AddRemoveHandlerStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitAggregateClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregateClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitQueryClause(node)
		End Function

		Public Overridable Function VisitAggregation(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitAggregationRangeVariable(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AggregationRangeVariableSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitAnonymousObjectCreationExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitNewExpression(node)
		End Function

		Public Overridable Function VisitArgument(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitArgumentList(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitArrayCreationExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayCreationExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitNewExpression(node)
		End Function

		Public Overridable Function VisitArrayRankSpecifier(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayRankSpecifierSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitArrayType(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArrayTypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitType(node)
		End Function

		Public Overridable Function VisitAsClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitAsNewClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AsNewClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitAsClause(node)
		End Function

		Public Overridable Function VisitAssignmentStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AssignmentStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitAttribute(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitAttributeList(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitAttributesStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributesStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDeclarationStatement(node)
		End Function

		Public Overridable Function VisitAttributeTarget(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AttributeTargetSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitAwaitExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.AwaitExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitBadDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BadDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDirectiveTrivia(node)
		End Function

		Public Overridable Function VisitBaseXmlAttribute(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BaseXmlAttributeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitXmlNode(node)
		End Function

		Public Overridable Function VisitBinaryConditionalExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryConditionalExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitBinaryExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.BinaryExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitCallStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CallStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitCaseBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitCaseClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitCaseStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CaseStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStatement(node)
		End Function

		Public Overridable Function VisitCastExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CastExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitCatchBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitCatchFilterClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchFilterClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitCatchStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CatchStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStatement(node)
		End Function

		Public Overridable Function VisitClassBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitTypeBlock(node)
		End Function

		Public Overridable Function VisitClassStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ClassStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitTypeStatement(node)
		End Function

		Public Overridable Function VisitCollectionInitializer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitCollectionRangeVariable(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CollectionRangeVariableSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitCompilationUnit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitConditionalAccessExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConditionalAccessExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitConstDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDirectiveTrivia(node)
		End Function

		Public Overridable Function VisitConstraint(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstraintSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitConstructorBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ConstructorBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitMethodBlockBase(node)
		End Function

		Public Overridable Function VisitContinueStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ContinueStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitCrefOperatorReference(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefOperatorReferenceSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitName(node)
		End Function

		Public Overridable Function VisitCrefReference(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefReferenceSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitCrefSignature(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignatureSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitCrefSignaturePart(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CrefSignaturePartSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitCTypeExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CTypeExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitCastExpression(node)
		End Function

		Public Overridable Function VisitDeclarationStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclarationStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStatement(node)
		End Function

		Public Overridable Function VisitDeclareStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DeclareStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitMethodBase(node)
		End Function

		Public Overridable Function VisitDelegateStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DelegateStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitMethodBase(node)
		End Function

		Public Overridable Function VisitDirectCastExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectCastExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitCastExpression(node)
		End Function

		Public Overridable Function VisitDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStructuredTrivia(node)
		End Function

		Public Overridable Function VisitDisableWarningDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DisableWarningDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDirectiveTrivia(node)
		End Function

		Public Overridable Function VisitDistinctClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DistinctClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitQueryClause(node)
		End Function

		Public Overridable Function VisitDocumentationCommentTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DocumentationCommentTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStructuredTrivia(node)
		End Function

		Public Overridable Function VisitDoLoopBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoLoopBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitDoStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DoStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStatement(node)
		End Function

		Public Overridable Function VisitElseBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitElseCaseClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseCaseClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitCaseClause(node)
		End Function

		Public Overridable Function VisitElseDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDirectiveTrivia(node)
		End Function

		Public Overridable Function VisitElseIfBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitElseIfStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseIfStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStatement(node)
		End Function

		Public Overridable Function VisitElseStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ElseStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStatement(node)
		End Function

		Public Overridable Function VisitEmptyStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStatement(node)
		End Function

		Public Overridable Function VisitEnableWarningDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnableWarningDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDirectiveTrivia(node)
		End Function

		Public Overridable Function VisitEndBlockStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndBlockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDeclarationStatement(node)
		End Function

		Public Overridable Function VisitEndExternalSourceDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndExternalSourceDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDirectiveTrivia(node)
		End Function

		Public Overridable Function VisitEndIfDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndIfDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDirectiveTrivia(node)
		End Function

		Public Overridable Function VisitEndRegionDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EndRegionDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDirectiveTrivia(node)
		End Function

		Public Overridable Function VisitEnumBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDeclarationStatement(node)
		End Function

		Public Overridable Function VisitEnumMemberDeclaration(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumMemberDeclarationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDeclarationStatement(node)
		End Function

		Public Overridable Function VisitEnumStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EnumStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDeclarationStatement(node)
		End Function

		Public Overridable Function VisitEqualsValue(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EqualsValueSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitEraseStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EraseStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitErrorStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ErrorStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitEventBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDeclarationStatement(node)
		End Function

		Public Overridable Function VisitEventContainer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventContainerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitEventStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EventStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitMethodBase(node)
		End Function

		Public Overridable Function VisitExecutableStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExecutableStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStatement(node)
		End Function

		Public Overridable Function VisitExitStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExitStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitExpressionRangeVariable(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitExpressionStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitExternalChecksumDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalChecksumDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDirectiveTrivia(node)
		End Function

		Public Overridable Function VisitExternalSourceDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExternalSourceDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDirectiveTrivia(node)
		End Function

		Public Overridable Function VisitFieldDeclaration(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldDeclarationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDeclarationStatement(node)
		End Function

		Public Overridable Function VisitFieldInitializer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FieldInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitFinallyBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitFinallyStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStatement(node)
		End Function

		Public Overridable Function VisitForBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitForOrForEachBlock(node)
		End Function

		Public Overridable Function VisitForEachBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitForOrForEachBlock(node)
		End Function

		Public Overridable Function VisitForEachStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForEachStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitForOrForEachStatement(node)
		End Function

		Public Overridable Function VisitForOrForEachBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForOrForEachBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitForOrForEachStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForOrForEachStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStatement(node)
		End Function

		Public Overridable Function VisitForStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitForOrForEachStatement(node)
		End Function

		Public Overridable Function VisitForStepClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ForStepClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitFromClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FromClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitQueryClause(node)
		End Function

		Public Overridable Function VisitFunctionAggregation(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FunctionAggregationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitAggregation(node)
		End Function

		Public Overridable Function VisitGenericName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GenericNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitSimpleName(node)
		End Function

		Public Overridable Function VisitGetTypeExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetTypeExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitGetXmlNamespaceExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GetXmlNamespaceExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitGlobalName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GlobalNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitName(node)
		End Function

		Public Overridable Function VisitGoToStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GoToStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitGroupAggregation(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupAggregationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitAggregation(node)
		End Function

		Public Overridable Function VisitGroupByClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupByClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitQueryClause(node)
		End Function

		Public Overridable Function VisitGroupJoinClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.GroupJoinClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitJoinClause(node)
		End Function

		Public Overridable Function VisitHandlesClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitHandlesClauseItem(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.HandlesClauseItemSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitIdentifierName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IdentifierNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitSimpleName(node)
		End Function

		Public Overridable Function VisitIfDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDirectiveTrivia(node)
		End Function

		Public Overridable Function VisitIfStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStatement(node)
		End Function

		Public Overridable Function VisitImplementsClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitImplementsStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImplementsStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitInheritsOrImplementsStatement(node)
		End Function

		Public Overridable Function VisitImportAliasClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportAliasClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitImportsClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitImportsStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ImportsStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDeclarationStatement(node)
		End Function

		Public Overridable Function VisitIncompleteMember(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IncompleteMemberSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDeclarationStatement(node)
		End Function

		Public Overridable Function VisitInferredFieldInitializer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InferredFieldInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitFieldInitializer(node)
		End Function

		Public Overridable Function VisitInheritsOrImplementsStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsOrImplementsStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDeclarationStatement(node)
		End Function

		Public Overridable Function VisitInheritsStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InheritsStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitInheritsOrImplementsStatement(node)
		End Function

		Public Overridable Function VisitInstanceExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InstanceExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitInterfaceBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitTypeBlock(node)
		End Function

		Public Overridable Function VisitInterfaceStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterfaceStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitTypeStatement(node)
		End Function

		Public Overridable Function VisitInterpolatedStringContent(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringContentSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitInterpolatedStringExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitInterpolatedStringText(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolatedStringTextSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitInterpolatedStringContent(node)
		End Function

		Public Overridable Function VisitInterpolation(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitInterpolatedStringContent(node)
		End Function

		Public Overridable Function VisitInterpolationAlignmentClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationAlignmentClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitInterpolationFormatClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InterpolationFormatClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitInvocationExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.InvocationExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitJoinClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitQueryClause(node)
		End Function

		Public Overridable Function VisitJoinCondition(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.JoinConditionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitKeywordEventContainer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordEventContainerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitEventContainer(node)
		End Function

		Public Overridable Function VisitLabel(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitLabelStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LabelStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitLambdaExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitLambdaHeader(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LambdaHeaderSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitMethodBase(node)
		End Function

		Public Overridable Function VisitLetClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LetClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitQueryClause(node)
		End Function

		Public Overridable Function VisitLiteralExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LiteralExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitLocalDeclarationStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LocalDeclarationStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitLoopStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.LoopStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStatement(node)
		End Function

		Public Overridable Function VisitMeExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MeExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitInstanceExpression(node)
		End Function

		Public Overridable Function VisitMemberAccessExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MemberAccessExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitMethodBase(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBaseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDeclarationStatement(node)
		End Function

		Public Overridable Function VisitMethodBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitMethodBlockBase(node)
		End Function

		Public Overridable Function VisitMethodBlockBase(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodBlockBaseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDeclarationStatement(node)
		End Function

		Public Overridable Function VisitMethodStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MethodStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitMethodBase(node)
		End Function

		Public Overridable Function VisitMidExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MidExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitModifiedIdentifier(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModifiedIdentifierSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitModuleBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitTypeBlock(node)
		End Function

		Public Overridable Function VisitModuleStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ModuleStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitTypeStatement(node)
		End Function

		Public Overridable Function VisitMultiLineIfBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineIfBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitMultiLineLambdaExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MultiLineLambdaExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitLambdaExpression(node)
		End Function

		Public Overridable Function VisitMyBaseExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyBaseExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitInstanceExpression(node)
		End Function

		Public Overridable Function VisitMyClassExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.MyClassExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitInstanceExpression(node)
		End Function

		Public Overridable Function VisitName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitType(node)
		End Function

		Public Overridable Function VisitNameColonEquals(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameColonEqualsSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitNamedFieldInitializer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedFieldInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitFieldInitializer(node)
		End Function

		Public Overridable Function VisitNamedTupleElement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamedTupleElementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitTupleElement(node)
		End Function

		Public Overridable Function VisitNameOfExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NameOfExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitNamespaceBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDeclarationStatement(node)
		End Function

		Public Overridable Function VisitNamespaceStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NamespaceStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDeclarationStatement(node)
		End Function

		Public Overridable Function VisitNewExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NewExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitNextStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NextStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStatement(node)
		End Function

		Public Overridable Function VisitNullableType(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.NullableTypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitType(node)
		End Function

		Public Overridable Function VisitObjectCollectionInitializer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCollectionInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitObjectCreationInitializer(node)
		End Function

		Public Overridable Function VisitObjectCreationExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitNewExpression(node)
		End Function

		Public Overridable Function VisitObjectCreationInitializer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectCreationInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitObjectMemberInitializer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ObjectMemberInitializerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitObjectCreationInitializer(node)
		End Function

		Public Overridable Function VisitOmittedArgument(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OmittedArgumentSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitArgument(node)
		End Function

		Public Overridable Function VisitOnErrorGoToStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorGoToStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitOnErrorResumeNextStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OnErrorResumeNextStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitOperatorBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitMethodBlockBase(node)
		End Function

		Public Overridable Function VisitOperatorStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OperatorStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitMethodBase(node)
		End Function

		Public Overridable Function VisitOptionStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OptionStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDeclarationStatement(node)
		End Function

		Public Overridable Function VisitOrderByClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderByClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitQueryClause(node)
		End Function

		Public Overridable Function VisitOrdering(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.OrderingSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitParameter(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitParameterList(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitParenthesizedExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ParenthesizedExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitPartitionClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitQueryClause(node)
		End Function

		Public Overridable Function VisitPartitionWhileClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PartitionWhileClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitQueryClause(node)
		End Function

		Public Overridable Function VisitPredefinedCastExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedCastExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitPredefinedType(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PredefinedTypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitType(node)
		End Function

		Public Overridable Function VisitPrintStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitPropertyBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDeclarationStatement(node)
		End Function

		Public Overridable Function VisitPropertyStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PropertyStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitMethodBase(node)
		End Function

		Public Overridable Function VisitQualifiedCrefOperatorReference(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedCrefOperatorReferenceSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitName(node)
		End Function

		Public Overridable Function VisitQualifiedName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QualifiedNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitName(node)
		End Function

		Public Overridable Function VisitQueryClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitQueryExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.QueryExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitRaiseEventStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RaiseEventStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitRangeArgument(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeArgumentSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitArgument(node)
		End Function

		Public Overridable Function VisitRangeCaseClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RangeCaseClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitCaseClause(node)
		End Function

		Public Overridable Function VisitRedimClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitReDimStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReDimStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitReferenceDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReferenceDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDirectiveTrivia(node)
		End Function

		Public Overridable Function VisitRegionDirectiveTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RegionDirectiveTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDirectiveTrivia(node)
		End Function

		Public Overridable Function VisitRelationalCaseClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RelationalCaseClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitCaseClause(node)
		End Function

		Public Overridable Function VisitResumeStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ResumeStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitReturnStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ReturnStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitSelectBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitSelectClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitQueryClause(node)
		End Function

		Public Overridable Function VisitSelectStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SelectStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStatement(node)
		End Function

		Public Overridable Function VisitSimpleArgument(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleArgumentSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitArgument(node)
		End Function

		Public Overridable Function VisitSimpleAsClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleAsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitAsClause(node)
		End Function

		Public Overridable Function VisitSimpleCaseClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleCaseClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitCaseClause(node)
		End Function

		Public Overridable Function VisitSimpleImportsClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleImportsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitImportsClause(node)
		End Function

		Public Overridable Function VisitSimpleJoinClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleJoinClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitJoinClause(node)
		End Function

		Public Overridable Function VisitSimpleName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SimpleNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitName(node)
		End Function

		Public Overridable Function VisitSingleLineElseClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitSingleLineIfStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineIfStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitSingleLineLambdaExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineLambdaExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitLambdaExpression(node)
		End Function

		Public Overridable Function VisitSkippedTokensTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SkippedTokensTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStructuredTrivia(node)
		End Function

		Public Overridable Function VisitSpecialConstraint(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SpecialConstraintSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitConstraint(node)
		End Function

		Public Overridable Function VisitStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitStopOrEndStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StopOrEndStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitStructureBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitTypeBlock(node)
		End Function

		Public Overridable Function VisitStructuredTrivia(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructuredTriviaSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitStructureStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.StructureStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitTypeStatement(node)
		End Function

		Public Overridable Function VisitSubNewStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SubNewStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitMethodBase(node)
		End Function

		Public Overridable Function VisitSyncLockBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitSyncLockStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStatement(node)
		End Function

		Public Overridable Function VisitSyntaxToken(ByVal token As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
			Return token
		End Function

		Public Overridable Function VisitSyntaxTrivia(ByVal trivia As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxTrivia
			Return trivia
		End Function

		Public Overridable Function VisitTernaryConditionalExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TernaryConditionalExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitThrowStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ThrowStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitTryBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitTryCastExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryCastExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitCastExpression(node)
		End Function

		Public Overridable Function VisitTryStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStatement(node)
		End Function

		Public Overridable Function VisitTupleElement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleElementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitTupleExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitTupleType(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TupleTypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitType(node)
		End Function

		Public Overridable Function VisitType(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitTypeArgumentList(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitTypeBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDeclarationStatement(node)
		End Function

		Public Overridable Function VisitTypeConstraint(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeConstraintSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitConstraint(node)
		End Function

		Public Overridable Function VisitTypedTupleElement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypedTupleElementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitTupleElement(node)
		End Function

		Public Overridable Function VisitTypeOfExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeOfExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitTypeParameter(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitTypeParameterConstraintClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitTypeParameterList(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitTypeParameterMultipleConstraintClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterMultipleConstraintClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitTypeParameterConstraintClause(node)
		End Function

		Public Overridable Function VisitTypeParameterSingleConstraintClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeParameterSingleConstraintClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitTypeParameterConstraintClause(node)
		End Function

		Public Overridable Function VisitTypeStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TypeStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitDeclarationStatement(node)
		End Function

		Public Overridable Function VisitUnaryExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UnaryExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitUsingBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitUsingStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStatement(node)
		End Function

		Public Overridable Function VisitVariableDeclarator(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableDeclaratorSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitVariableNameEquals(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitVisualBasicSyntaxNode(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return node
		End Function

		Public Overridable Function VisitWhereClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhereClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitQueryClause(node)
		End Function

		Public Overridable Function VisitWhileBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitWhileOrUntilClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileOrUntilClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitWhileStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WhileStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStatement(node)
		End Function

		Public Overridable Function VisitWithBlock(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithBlockSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function

		Public Overridable Function VisitWithEventsEventContainer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsEventContainerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitEventContainer(node)
		End Function

		Public Overridable Function VisitWithEventsPropertyEventContainer(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithEventsPropertyEventContainerSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitEventContainer(node)
		End Function

		Public Overridable Function VisitWithStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.WithStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitStatement(node)
		End Function

		Public Overridable Function VisitXmlAttribute(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlAttributeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitBaseXmlAttribute(node)
		End Function

		Public Overridable Function VisitXmlBracketedName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlBracketedNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitXmlNode(node)
		End Function

		Public Overridable Function VisitXmlCDataSection(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCDataSectionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitXmlNode(node)
		End Function

		Public Overridable Function VisitXmlComment(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCommentSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitXmlNode(node)
		End Function

		Public Overridable Function VisitXmlCrefAttribute(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlCrefAttributeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitBaseXmlAttribute(node)
		End Function

		Public Overridable Function VisitXmlDeclaration(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitXmlDeclarationOption(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDeclarationOptionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitXmlDocument(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlDocumentSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitXmlNode(node)
		End Function

		Public Overridable Function VisitXmlElement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitXmlNode(node)
		End Function

		Public Overridable Function VisitXmlElementEndTag(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementEndTagSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitXmlNode(node)
		End Function

		Public Overridable Function VisitXmlElementStartTag(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlElementStartTagSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitXmlNode(node)
		End Function

		Public Overridable Function VisitXmlEmbeddedExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmbeddedExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitXmlNode(node)
		End Function

		Public Overridable Function VisitXmlEmptyElement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlEmptyElementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitXmlNode(node)
		End Function

		Public Overridable Function VisitXmlMemberAccessExpression(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlMemberAccessExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitXmlName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitXmlNode(node)
		End Function

		Public Overridable Function VisitXmlNameAttribute(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNameAttributeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitBaseXmlAttribute(node)
		End Function

		Public Overridable Function VisitXmlNamespaceImportsClause(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNamespaceImportsClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitImportsClause(node)
		End Function

		Public Overridable Function VisitXmlNode(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlNodeSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExpression(node)
		End Function

		Public Overridable Function VisitXmlPrefix(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitVisualBasicSyntaxNode(node)
		End Function

		Public Overridable Function VisitXmlPrefixName(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlPrefixNameSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitXmlNode(node)
		End Function

		Public Overridable Function VisitXmlProcessingInstruction(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlProcessingInstructionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitXmlNode(node)
		End Function

		Public Overridable Function VisitXmlString(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlStringSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitXmlNode(node)
		End Function

		Public Overridable Function VisitXmlText(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.XmlTextSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitXmlNode(node)
		End Function

		Public Overridable Function VisitYieldStatement(ByVal node As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.YieldStatementSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode
			Return Me.VisitExecutableStatement(node)
		End Function
	End Class
End Namespace