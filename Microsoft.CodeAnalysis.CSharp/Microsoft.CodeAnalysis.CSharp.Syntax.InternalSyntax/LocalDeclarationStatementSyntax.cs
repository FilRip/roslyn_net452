using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class LocalDeclarationStatementSyntax : StatementSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly SyntaxToken? awaitKeyword;

        internal readonly SyntaxToken? usingKeyword;

        internal readonly GreenNode? modifiers;

        internal readonly VariableDeclarationSyntax declaration;

        internal readonly SyntaxToken semicolonToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public SyntaxToken? AwaitKeyword => awaitKeyword;

        public SyntaxToken? UsingKeyword => usingKeyword;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(modifiers);

        public VariableDeclarationSyntax Declaration => declaration;

        public SyntaxToken SemicolonToken => semicolonToken;

        public LocalDeclarationStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken? awaitKeyword, SyntaxToken? usingKeyword, GreenNode? modifiers, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 6;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (awaitKeyword != null)
            {
                AdjustFlagsAndWidth(awaitKeyword);
                this.awaitKeyword = awaitKeyword;
            }
            if (usingKeyword != null)
            {
                AdjustFlagsAndWidth(usingKeyword);
                this.usingKeyword = usingKeyword;
            }
            if (modifiers != null)
            {
                AdjustFlagsAndWidth(modifiers);
                this.modifiers = modifiers;
            }
            AdjustFlagsAndWidth(declaration);
            this.declaration = declaration;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public LocalDeclarationStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken? awaitKeyword, SyntaxToken? usingKeyword, GreenNode? modifiers, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 6;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (awaitKeyword != null)
            {
                AdjustFlagsAndWidth(awaitKeyword);
                this.awaitKeyword = awaitKeyword;
            }
            if (usingKeyword != null)
            {
                AdjustFlagsAndWidth(usingKeyword);
                this.usingKeyword = usingKeyword;
            }
            if (modifiers != null)
            {
                AdjustFlagsAndWidth(modifiers);
                this.modifiers = modifiers;
            }
            AdjustFlagsAndWidth(declaration);
            this.declaration = declaration;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public LocalDeclarationStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, SyntaxToken? awaitKeyword, SyntaxToken? usingKeyword, GreenNode? modifiers, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
            : base(kind)
        {
            base.SlotCount = 6;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (awaitKeyword != null)
            {
                AdjustFlagsAndWidth(awaitKeyword);
                this.awaitKeyword = awaitKeyword;
            }
            if (usingKeyword != null)
            {
                AdjustFlagsAndWidth(usingKeyword);
                this.usingKeyword = usingKeyword;
            }
            if (modifiers != null)
            {
                AdjustFlagsAndWidth(modifiers);
                this.modifiers = modifiers;
            }
            AdjustFlagsAndWidth(declaration);
            this.declaration = declaration;
            AdjustFlagsAndWidth(semicolonToken);
            this.semicolonToken = semicolonToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => awaitKeyword,
                2 => usingKeyword,
                3 => modifiers,
                4 => declaration,
                5 => semicolonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.LocalDeclarationStatementSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitLocalDeclarationStatement(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitLocalDeclarationStatement(this);
        }

        public LocalDeclarationStatementSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken awaitKeyword, SyntaxToken usingKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> modifiers, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || awaitKeyword != AwaitKeyword || usingKeyword != UsingKeyword || modifiers != Modifiers || declaration != Declaration || semicolonToken != SemicolonToken)
            {
                LocalDeclarationStatementSyntax localDeclarationStatementSyntax = SyntaxFactory.LocalDeclarationStatement(attributeLists, awaitKeyword, usingKeyword, modifiers, declaration, semicolonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    localDeclarationStatementSyntax = localDeclarationStatementSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    localDeclarationStatementSyntax = localDeclarationStatementSyntax.WithAnnotationsGreen(annotations);
                }
                return localDeclarationStatementSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new LocalDeclarationStatementSyntax(base.Kind, attributeLists, awaitKeyword, usingKeyword, modifiers, declaration, semicolonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new LocalDeclarationStatementSyntax(base.Kind, attributeLists, awaitKeyword, usingKeyword, modifiers, declaration, semicolonToken, GetDiagnostics(), annotations);
        }

        public LocalDeclarationStatementSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 6;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                attributeLists = greenNode;
            }
            SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
            if (syntaxToken != null)
            {
                AdjustFlagsAndWidth(syntaxToken);
                awaitKeyword = syntaxToken;
            }
            SyntaxToken syntaxToken2 = (SyntaxToken)reader.ReadValue();
            if (syntaxToken2 != null)
            {
                AdjustFlagsAndWidth(syntaxToken2);
                usingKeyword = syntaxToken2;
            }
            GreenNode greenNode2 = (GreenNode)reader.ReadValue();
            if (greenNode2 != null)
            {
                AdjustFlagsAndWidth(greenNode2);
                modifiers = greenNode2;
            }
            VariableDeclarationSyntax node = (VariableDeclarationSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            declaration = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            semicolonToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(awaitKeyword);
            writer.WriteValue(usingKeyword);
            writer.WriteValue(modifiers);
            writer.WriteValue(declaration);
            writer.WriteValue(semicolonToken);
        }

        static LocalDeclarationStatementSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(LocalDeclarationStatementSyntax), (ObjectReader r) => new LocalDeclarationStatementSyntax(r));
        }
    }
}
