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

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class FixedStatementSyntax : StatementSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly SyntaxToken fixedKeyword;

        internal readonly SyntaxToken openParenToken;

        internal readonly VariableDeclarationSyntax declaration;

        internal readonly SyntaxToken closeParenToken;

        internal readonly StatementSyntax statement;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public SyntaxToken FixedKeyword => fixedKeyword;

        public SyntaxToken OpenParenToken => openParenToken;

        public VariableDeclarationSyntax Declaration => declaration;

        public SyntaxToken CloseParenToken => closeParenToken;

        public StatementSyntax Statement => statement;

        public FixedStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken fixedKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax declaration, SyntaxToken closeParenToken, StatementSyntax statement, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 6;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(fixedKeyword);
            this.fixedKeyword = fixedKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(declaration);
            this.declaration = declaration;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
        }

        public FixedStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken fixedKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax declaration, SyntaxToken closeParenToken, StatementSyntax statement, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 6;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(fixedKeyword);
            this.fixedKeyword = fixedKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(declaration);
            this.declaration = declaration;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
        }

        public FixedStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken fixedKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax declaration, SyntaxToken closeParenToken, StatementSyntax statement)
            : base(kind)
        {
            base.SlotCount = 6;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            AdjustFlagsAndWidth(fixedKeyword);
            this.fixedKeyword = fixedKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(declaration);
            this.declaration = declaration;
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
                1 => fixedKeyword,
                2 => openParenToken,
                3 => declaration,
                4 => closeParenToken,
                5 => statement,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.FixedStatementSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitFixedStatement(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitFixedStatement(this);
        }

        public FixedStatementSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken fixedKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax declaration, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (attributeLists != AttributeLists || fixedKeyword != FixedKeyword || openParenToken != OpenParenToken || declaration != Declaration || closeParenToken != CloseParenToken || statement != Statement)
            {
                FixedStatementSyntax fixedStatementSyntax = SyntaxFactory.FixedStatement(attributeLists, fixedKeyword, openParenToken, declaration, closeParenToken, statement);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    fixedStatementSyntax = fixedStatementSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    fixedStatementSyntax = fixedStatementSyntax.WithAnnotationsGreen(annotations);
                }
                return fixedStatementSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new FixedStatementSyntax(base.Kind, attributeLists, fixedKeyword, openParenToken, declaration, closeParenToken, statement, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new FixedStatementSyntax(base.Kind, attributeLists, fixedKeyword, openParenToken, declaration, closeParenToken, statement, GetDiagnostics(), annotations);
        }

        public FixedStatementSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 6;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                attributeLists = greenNode;
            }
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            fixedKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            openParenToken = node2;
            VariableDeclarationSyntax node3 = (VariableDeclarationSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            declaration = node3;
            SyntaxToken node4 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            closeParenToken = node4;
            StatementSyntax node5 = (StatementSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node5);
            statement = node5;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(fixedKeyword);
            writer.WriteValue(openParenToken);
            writer.WriteValue(declaration);
            writer.WriteValue(closeParenToken);
            writer.WriteValue(statement);
        }

        static FixedStatementSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(FixedStatementSyntax), (ObjectReader r) => new FixedStatementSyntax(r));
        }
    }
}
