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

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class FromClauseSyntax : QueryClauseSyntax
    {
        internal readonly SyntaxToken fromKeyword;

        internal readonly TypeSyntax? type;

        internal readonly SyntaxToken identifier;

        internal readonly SyntaxToken inKeyword;

        internal readonly ExpressionSyntax expression;

        public SyntaxToken FromKeyword => fromKeyword;

        public TypeSyntax? Type => type;

        public SyntaxToken Identifier => identifier;

        public SyntaxToken InKeyword => inKeyword;

        public ExpressionSyntax Expression => expression;

        public FromClauseSyntax(SyntaxKind kind, SyntaxToken fromKeyword, TypeSyntax? type, SyntaxToken identifier, SyntaxToken inKeyword, ExpressionSyntax expression, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 5;
            AdjustFlagsAndWidth(fromKeyword);
            this.fromKeyword = fromKeyword;
            if (type != null)
            {
                AdjustFlagsAndWidth(type);
                this.type = type;
            }
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(inKeyword);
            this.inKeyword = inKeyword;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public FromClauseSyntax(SyntaxKind kind, SyntaxToken fromKeyword, TypeSyntax? type, SyntaxToken identifier, SyntaxToken inKeyword, ExpressionSyntax expression, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 5;
            AdjustFlagsAndWidth(fromKeyword);
            this.fromKeyword = fromKeyword;
            if (type != null)
            {
                AdjustFlagsAndWidth(type);
                this.type = type;
            }
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(inKeyword);
            this.inKeyword = inKeyword;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public FromClauseSyntax(SyntaxKind kind, SyntaxToken fromKeyword, TypeSyntax? type, SyntaxToken identifier, SyntaxToken inKeyword, ExpressionSyntax expression)
            : base(kind)
        {
            base.SlotCount = 5;
            AdjustFlagsAndWidth(fromKeyword);
            this.fromKeyword = fromKeyword;
            if (type != null)
            {
                AdjustFlagsAndWidth(type);
                this.type = type;
            }
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(inKeyword);
            this.inKeyword = inKeyword;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => fromKeyword,
                1 => type,
                2 => identifier,
                3 => inKeyword,
                4 => expression,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.FromClauseSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitFromClause(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitFromClause(this);
        }

        public FromClauseSyntax Update(SyntaxToken fromKeyword, TypeSyntax type, SyntaxToken identifier, SyntaxToken inKeyword, ExpressionSyntax expression)
        {
            if (fromKeyword != FromKeyword || type != Type || identifier != Identifier || inKeyword != InKeyword || expression != Expression)
            {
                FromClauseSyntax fromClauseSyntax = SyntaxFactory.FromClause(fromKeyword, type, identifier, inKeyword, expression);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    fromClauseSyntax = fromClauseSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    fromClauseSyntax = fromClauseSyntax.WithAnnotationsGreen(annotations);
                }
                return fromClauseSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new FromClauseSyntax(base.Kind, fromKeyword, type, identifier, inKeyword, expression, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new FromClauseSyntax(base.Kind, fromKeyword, type, identifier, inKeyword, expression, GetDiagnostics(), annotations);
        }

        public FromClauseSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 5;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            fromKeyword = node;
            TypeSyntax typeSyntax = (TypeSyntax)reader.ReadValue();
            if (typeSyntax != null)
            {
                AdjustFlagsAndWidth(typeSyntax);
                type = typeSyntax;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            identifier = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            inKeyword = node3;
            ExpressionSyntax node4 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            expression = node4;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(fromKeyword);
            writer.WriteValue(type);
            writer.WriteValue(identifier);
            writer.WriteValue(inKeyword);
            writer.WriteValue(expression);
        }

        static FromClauseSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(FromClauseSyntax), (ObjectReader r) => new FromClauseSyntax(r));
        }
    }
}
