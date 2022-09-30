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
    public sealed class OrderByClauseSyntax : QueryClauseSyntax
    {
        internal readonly SyntaxToken orderByKeyword;

        internal readonly GreenNode? orderings;

        public SyntaxToken OrderByKeyword => orderByKeyword;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<OrderingSyntax> Orderings => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<OrderingSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(orderings));

        public OrderByClauseSyntax(SyntaxKind kind, SyntaxToken orderByKeyword, GreenNode? orderings, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(orderByKeyword);
            this.orderByKeyword = orderByKeyword;
            if (orderings != null)
            {
                AdjustFlagsAndWidth(orderings);
                this.orderings = orderings;
            }
        }

        public OrderByClauseSyntax(SyntaxKind kind, SyntaxToken orderByKeyword, GreenNode? orderings, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(orderByKeyword);
            this.orderByKeyword = orderByKeyword;
            if (orderings != null)
            {
                AdjustFlagsAndWidth(orderings);
                this.orderings = orderings;
            }
        }

        public OrderByClauseSyntax(SyntaxKind kind, SyntaxToken orderByKeyword, GreenNode? orderings)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(orderByKeyword);
            this.orderByKeyword = orderByKeyword;
            if (orderings != null)
            {
                AdjustFlagsAndWidth(orderings);
                this.orderings = orderings;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => orderByKeyword,
                1 => orderings,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.OrderByClauseSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitOrderByClause(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitOrderByClause(this);
        }

        public OrderByClauseSyntax Update(SyntaxToken orderByKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<OrderingSyntax> orderings)
        {
            if (orderByKeyword == OrderByKeyword)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<OrderingSyntax> right = Orderings;
                if (!(orderings != right))
                {
                    return this;
                }
            }
            OrderByClauseSyntax orderByClauseSyntax = SyntaxFactory.OrderByClause(orderByKeyword, orderings);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                orderByClauseSyntax = orderByClauseSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                orderByClauseSyntax = orderByClauseSyntax.WithAnnotationsGreen(annotations);
            }
            return orderByClauseSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new OrderByClauseSyntax(base.Kind, orderByKeyword, orderings, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new OrderByClauseSyntax(base.Kind, orderByKeyword, orderings, GetDiagnostics(), annotations);
        }

        public OrderByClauseSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            orderByKeyword = node;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                orderings = greenNode;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(orderByKeyword);
            writer.WriteValue(orderings);
        }

        static OrderByClauseSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(OrderByClauseSyntax), (ObjectReader r) => new OrderByClauseSyntax(r));
        }
    }
}
