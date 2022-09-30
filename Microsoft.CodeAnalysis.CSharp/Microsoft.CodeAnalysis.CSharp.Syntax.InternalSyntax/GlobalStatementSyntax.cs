using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class GlobalStatementSyntax : MemberDeclarationSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly GreenNode? modifiers;

        internal readonly StatementSyntax statement;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(modifiers);

        public StatementSyntax Statement => statement;

        public GlobalStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, StatementSyntax statement, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (modifiers != null)
            {
                AdjustFlagsAndWidth(modifiers);
                this.modifiers = modifiers;
            }
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
        }

        public GlobalStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, StatementSyntax statement, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (modifiers != null)
            {
                AdjustFlagsAndWidth(modifiers);
                this.modifiers = modifiers;
            }
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
        }

        public GlobalStatementSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, StatementSyntax statement)
            : base(kind)
        {
            base.SlotCount = 3;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (modifiers != null)
            {
                AdjustFlagsAndWidth(modifiers);
                this.modifiers = modifiers;
            }
            AdjustFlagsAndWidth(statement);
            this.statement = statement;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => modifiers,
                2 => statement,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitGlobalStatement(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitGlobalStatement(this);
        }

        public GlobalStatementSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> modifiers, StatementSyntax statement)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || statement != Statement)
            {
                GlobalStatementSyntax globalStatementSyntax = SyntaxFactory.GlobalStatement(attributeLists, modifiers, statement);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    globalStatementSyntax = globalStatementSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    globalStatementSyntax = globalStatementSyntax.WithAnnotationsGreen(annotations);
                }
                return globalStatementSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new GlobalStatementSyntax(base.Kind, attributeLists, modifiers, statement, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new GlobalStatementSyntax(base.Kind, attributeLists, modifiers, statement, GetDiagnostics(), annotations);
        }

        public GlobalStatementSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                attributeLists = greenNode;
            }
            GreenNode greenNode2 = (GreenNode)reader.ReadValue();
            if (greenNode2 != null)
            {
                AdjustFlagsAndWidth(greenNode2);
                modifiers = greenNode2;
            }
            StatementSyntax node = (StatementSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            statement = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(modifiers);
            writer.WriteValue(statement);
        }

        static GlobalStatementSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(GlobalStatementSyntax), (ObjectReader r) => new GlobalStatementSyntax(r));
        }
    }
}
