using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ForStatementSyntax : StatementSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly SyntaxToken forKeyword;

        internal readonly SyntaxToken openParenToken;

        internal readonly VariableDeclarationSyntax? declaration;

        internal readonly GreenNode? initializers;

        internal readonly SyntaxToken firstSemicolonToken;

        internal readonly ExpressionSyntax? condition;

        internal readonly SyntaxToken secondSemicolonToken;

        internal readonly GreenNode? incrementors;

        internal readonly SyntaxToken closeParenToken;

        internal readonly StatementSyntax statement;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public SyntaxToken ForKeyword => forKeyword;

        public SyntaxToken OpenParenToken => openParenToken;

        public VariableDeclarationSyntax? Declaration => declaration;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> Initializers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(initializers));

        public SyntaxToken FirstSemicolonToken => firstSemicolonToken;

        public ExpressionSyntax? Condition => condition;

        public SyntaxToken SecondSemicolonToken => secondSemicolonToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> Incrementors => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(incrementors));

        public SyntaxToken CloseParenToken => closeParenToken;

        public StatementSyntax Statement => statement;

        public ForStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken forKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax? declaration, GreenNode? initializers, SyntaxToken firstSemicolonToken, ExpressionSyntax? condition, SyntaxToken secondSemicolonToken, GreenNode? incrementors, SyntaxToken closeParenToken, StatementSyntax statement, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 11;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(forKeyword);
            this.forKeyword = forKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            if (declaration != null)
            {
                AdjustFlagsAndWidth(declaration);
                this.declaration = declaration;
            }
            if (initializers != null)
            {
                AdjustFlagsAndWidth(initializers);
                this.initializers = initializers;
            }
            AdjustFlagsAndWidth(firstSemicolonToken);
            this.firstSemicolonToken = firstSemicolonToken;
            if (condition != null)
            {
                AdjustFlagsAndWidth(condition);
                this.condition = condition;
            }
            AdjustFlagsAndWidth(secondSemicolonToken);
            this.secondSemicolonToken = secondSemicolonToken;
            if (incrementors != null)
            {
                AdjustFlagsAndWidth(incrementors);
                this.incrementors = incrementors;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
        }

        public ForStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken forKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax? declaration, GreenNode? initializers, SyntaxToken firstSemicolonToken, ExpressionSyntax? condition, SyntaxToken secondSemicolonToken, GreenNode? incrementors, SyntaxToken closeParenToken, StatementSyntax statement, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 11;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(forKeyword);
            this.forKeyword = forKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            if (declaration != null)
            {
                AdjustFlagsAndWidth(declaration);
                this.declaration = declaration;
            }
            if (initializers != null)
            {
                AdjustFlagsAndWidth(initializers);
                this.initializers = initializers;
            }
            AdjustFlagsAndWidth(firstSemicolonToken);
            this.firstSemicolonToken = firstSemicolonToken;
            if (condition != null)
            {
                AdjustFlagsAndWidth(condition);
                this.condition = condition;
            }
            AdjustFlagsAndWidth(secondSemicolonToken);
            this.secondSemicolonToken = secondSemicolonToken;
            if (incrementors != null)
            {
                AdjustFlagsAndWidth(incrementors);
                this.incrementors = incrementors;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
        }

        public ForStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken forKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax? declaration, GreenNode? initializers, SyntaxToken firstSemicolonToken, ExpressionSyntax? condition, SyntaxToken secondSemicolonToken, GreenNode? incrementors, SyntaxToken closeParenToken, StatementSyntax statement)
            : base(kind)
        {
            base.SlotCount = 11;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(forKeyword);
            this.forKeyword = forKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            if (declaration != null)
            {
                AdjustFlagsAndWidth(declaration);
                this.declaration = declaration;
            }
            if (initializers != null)
            {
                AdjustFlagsAndWidth(initializers);
                this.initializers = initializers;
            }
            AdjustFlagsAndWidth(firstSemicolonToken);
            this.firstSemicolonToken = firstSemicolonToken;
            if (condition != null)
            {
                AdjustFlagsAndWidth(condition);
                this.condition = condition;
            }
            AdjustFlagsAndWidth(secondSemicolonToken);
            this.secondSemicolonToken = secondSemicolonToken;
            if (incrementors != null)
            {
                AdjustFlagsAndWidth(incrementors);
                this.incrementors = incrementors;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => forKeyword,
                2 => openParenToken,
                3 => declaration,
                4 => initializers,
                5 => firstSemicolonToken,
                6 => condition,
                7 => secondSemicolonToken,
                8 => incrementors,
                9 => closeParenToken,
                10 => statement,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ForStatementSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitForStatement(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitForStatement(this);
        }

        public ForStatementSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken forKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax declaration, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> initializers, SyntaxToken firstSemicolonToken, ExpressionSyntax condition, SyntaxToken secondSemicolonToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> incrementors, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (!(attributeLists != AttributeLists) && forKeyword == ForKeyword && openParenToken == OpenParenToken && declaration == Declaration)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> right = Initializers;
                if (!(initializers != right) && firstSemicolonToken == FirstSemicolonToken && condition == Condition && secondSemicolonToken == SecondSemicolonToken)
                {
                    Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> right2 = Incrementors;
                    if (!(incrementors != right2) && closeParenToken == CloseParenToken && statement == Statement)
                    {
                        return this;
                    }
                }
            }
            ForStatementSyntax forStatementSyntax = SyntaxFactory.ForStatement(attributeLists, forKeyword, openParenToken, declaration, initializers, firstSemicolonToken, condition, secondSemicolonToken, incrementors, closeParenToken, statement);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                forStatementSyntax = forStatementSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                forStatementSyntax = forStatementSyntax.WithAnnotationsGreen(annotations);
            }
            return forStatementSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ForStatementSyntax(base.Kind, attributeLists, forKeyword, openParenToken, declaration, initializers, firstSemicolonToken, condition, secondSemicolonToken, incrementors, closeParenToken, statement, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ForStatementSyntax(base.Kind, attributeLists, forKeyword, openParenToken, declaration, initializers, firstSemicolonToken, condition, secondSemicolonToken, incrementors, closeParenToken, statement, GetDiagnostics(), annotations);
        }

        public ForStatementSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 11;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                attributeLists = greenNode;
            }
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            forKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            openParenToken = node2;
            VariableDeclarationSyntax variableDeclarationSyntax = (VariableDeclarationSyntax)reader.ReadValue();
            if (variableDeclarationSyntax != null)
            {
                AdjustFlagsAndWidth(variableDeclarationSyntax);
                declaration = variableDeclarationSyntax;
            }
            GreenNode greenNode2 = (GreenNode)reader.ReadValue();
            if (greenNode2 != null)
            {
                AdjustFlagsAndWidth(greenNode2);
                initializers = greenNode2;
            }
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            firstSemicolonToken = node3;
            ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
            if (expressionSyntax != null)
            {
                AdjustFlagsAndWidth(expressionSyntax);
                condition = expressionSyntax;
            }
            SyntaxToken node4 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            secondSemicolonToken = node4;
            GreenNode greenNode3 = (GreenNode)reader.ReadValue();
            if (greenNode3 != null)
            {
                AdjustFlagsAndWidth(greenNode3);
                incrementors = greenNode3;
            }
            SyntaxToken node5 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node5);
            closeParenToken = node5;
            StatementSyntax node6 = (StatementSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node6);
            statement = node6;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(forKeyword);
            writer.WriteValue(openParenToken);
            writer.WriteValue(declaration);
            writer.WriteValue(initializers);
            writer.WriteValue(firstSemicolonToken);
            writer.WriteValue(condition);
            writer.WriteValue(secondSemicolonToken);
            writer.WriteValue(incrementors);
            writer.WriteValue(closeParenToken);
            writer.WriteValue(statement);
        }

        static ForStatementSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ForStatementSyntax), (ObjectReader r) => new ForStatementSyntax(r));
        }
    }
}
