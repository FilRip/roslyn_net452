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
    public sealed class InterpolatedStringExpressionSyntax : ExpressionSyntax
    {
        internal readonly SyntaxToken stringStartToken;

        internal readonly GreenNode? contents;

        internal readonly SyntaxToken stringEndToken;

        public SyntaxToken StringStartToken => stringStartToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<InterpolatedStringContentSyntax> Contents => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<InterpolatedStringContentSyntax>(contents);

        public SyntaxToken StringEndToken => stringEndToken;

        public InterpolatedStringExpressionSyntax(SyntaxKind kind, SyntaxToken stringStartToken, GreenNode? contents, SyntaxToken stringEndToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(stringStartToken);
            this.stringStartToken = stringStartToken;
            if (contents != null)
            {
                AdjustFlagsAndWidth(contents);
                this.contents = contents;
            }
            AdjustFlagsAndWidth(stringEndToken);
            this.stringEndToken = stringEndToken;
        }

        public InterpolatedStringExpressionSyntax(SyntaxKind kind, SyntaxToken stringStartToken, GreenNode? contents, SyntaxToken stringEndToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(stringStartToken);
            this.stringStartToken = stringStartToken;
            if (contents != null)
            {
                AdjustFlagsAndWidth(contents);
                this.contents = contents;
            }
            AdjustFlagsAndWidth(stringEndToken);
            this.stringEndToken = stringEndToken;
        }

        public InterpolatedStringExpressionSyntax(SyntaxKind kind, SyntaxToken stringStartToken, GreenNode? contents, SyntaxToken stringEndToken)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(stringStartToken);
            this.stringStartToken = stringStartToken;
            if (contents != null)
            {
                AdjustFlagsAndWidth(contents);
                this.contents = contents;
            }
            AdjustFlagsAndWidth(stringEndToken);
            this.stringEndToken = stringEndToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => stringStartToken,
                1 => contents,
                2 => stringEndToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.InterpolatedStringExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitInterpolatedStringExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitInterpolatedStringExpression(this);
        }

        public InterpolatedStringExpressionSyntax Update(SyntaxToken stringStartToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<InterpolatedStringContentSyntax> contents, SyntaxToken stringEndToken)
        {
            if (stringStartToken != StringStartToken || contents != Contents || stringEndToken != StringEndToken)
            {
                InterpolatedStringExpressionSyntax interpolatedStringExpressionSyntax = SyntaxFactory.InterpolatedStringExpression(stringStartToken, contents, stringEndToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    interpolatedStringExpressionSyntax = interpolatedStringExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    interpolatedStringExpressionSyntax = interpolatedStringExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return interpolatedStringExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new InterpolatedStringExpressionSyntax(base.Kind, stringStartToken, contents, stringEndToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new InterpolatedStringExpressionSyntax(base.Kind, stringStartToken, contents, stringEndToken, GetDiagnostics(), annotations);
        }

        public InterpolatedStringExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            stringStartToken = node;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                contents = greenNode;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            stringEndToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(stringStartToken);
            writer.WriteValue(contents);
            writer.WriteValue(stringEndToken);
        }

        static InterpolatedStringExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(InterpolatedStringExpressionSyntax), (ObjectReader r) => new InterpolatedStringExpressionSyntax(r));
        }
    }
}
