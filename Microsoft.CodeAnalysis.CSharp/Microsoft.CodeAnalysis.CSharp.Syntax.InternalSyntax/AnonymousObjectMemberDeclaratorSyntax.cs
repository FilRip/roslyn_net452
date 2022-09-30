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
    public sealed class AnonymousObjectMemberDeclaratorSyntax : CSharpSyntaxNode
    {
        internal readonly NameEqualsSyntax? nameEquals;

        internal readonly ExpressionSyntax expression;

        public NameEqualsSyntax? NameEquals => nameEquals;

        public ExpressionSyntax Expression => expression;

        public AnonymousObjectMemberDeclaratorSyntax(SyntaxKind kind, NameEqualsSyntax? nameEquals, ExpressionSyntax expression, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            if (nameEquals != null)
            {
                AdjustFlagsAndWidth(nameEquals);
                this.nameEquals = nameEquals;
            }
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public AnonymousObjectMemberDeclaratorSyntax(SyntaxKind kind, NameEqualsSyntax? nameEquals, ExpressionSyntax expression, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            if (nameEquals != null)
            {
                AdjustFlagsAndWidth(nameEquals);
                this.nameEquals = nameEquals;
            }
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public AnonymousObjectMemberDeclaratorSyntax(SyntaxKind kind, NameEqualsSyntax? nameEquals, ExpressionSyntax expression)
            : base(kind)
        {
            base.SlotCount = 2;
            if (nameEquals != null)
            {
                AdjustFlagsAndWidth(nameEquals);
                this.nameEquals = nameEquals;
            }
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => nameEquals,
                1 => expression,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.AnonymousObjectMemberDeclaratorSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitAnonymousObjectMemberDeclarator(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitAnonymousObjectMemberDeclarator(this);
        }

        public AnonymousObjectMemberDeclaratorSyntax Update(NameEqualsSyntax nameEquals, ExpressionSyntax expression)
        {
            if (nameEquals != NameEquals || expression != Expression)
            {
                AnonymousObjectMemberDeclaratorSyntax anonymousObjectMemberDeclaratorSyntax = SyntaxFactory.AnonymousObjectMemberDeclarator(nameEquals, expression);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    anonymousObjectMemberDeclaratorSyntax = anonymousObjectMemberDeclaratorSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    anonymousObjectMemberDeclaratorSyntax = anonymousObjectMemberDeclaratorSyntax.WithAnnotationsGreen(annotations);
                }
                return anonymousObjectMemberDeclaratorSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new AnonymousObjectMemberDeclaratorSyntax(base.Kind, nameEquals, expression, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new AnonymousObjectMemberDeclaratorSyntax(base.Kind, nameEquals, expression, GetDiagnostics(), annotations);
        }

        public AnonymousObjectMemberDeclaratorSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            NameEqualsSyntax nameEqualsSyntax = (NameEqualsSyntax)reader.ReadValue();
            if (nameEqualsSyntax != null)
            {
                AdjustFlagsAndWidth(nameEqualsSyntax);
                nameEquals = nameEqualsSyntax;
            }
            ExpressionSyntax node = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            expression = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(nameEquals);
            writer.WriteValue(expression);
        }

        static AnonymousObjectMemberDeclaratorSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(AnonymousObjectMemberDeclaratorSyntax), (ObjectReader r) => new AnonymousObjectMemberDeclaratorSyntax(r));
        }
    }
}
