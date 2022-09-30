using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ArgumentSyntax : CSharpSyntaxNode
    {
        internal readonly NameColonSyntax? nameColon;

        internal readonly SyntaxToken? refKindKeyword;

        internal readonly ExpressionSyntax expression;

        public NameColonSyntax? NameColon => nameColon;

        public SyntaxToken? RefKindKeyword => refKindKeyword;

        public ExpressionSyntax Expression => expression;

        public ArgumentSyntax(SyntaxKind kind, NameColonSyntax? nameColon, SyntaxToken? refKindKeyword, ExpressionSyntax expression, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            if (nameColon != null)
            {
                AdjustFlagsAndWidth(nameColon);
                this.nameColon = nameColon;
            }
            if (refKindKeyword != null)
            {
                AdjustFlagsAndWidth(refKindKeyword);
                this.refKindKeyword = refKindKeyword;
            }
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public ArgumentSyntax(SyntaxKind kind, NameColonSyntax? nameColon, SyntaxToken? refKindKeyword, ExpressionSyntax expression, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            if (nameColon != null)
            {
                AdjustFlagsAndWidth(nameColon);
                this.nameColon = nameColon;
            }
            if (refKindKeyword != null)
            {
                AdjustFlagsAndWidth(refKindKeyword);
                this.refKindKeyword = refKindKeyword;
            }
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public ArgumentSyntax(SyntaxKind kind, NameColonSyntax? nameColon, SyntaxToken? refKindKeyword, ExpressionSyntax expression)
            : base(kind)
        {
            base.SlotCount = 3;
            if (nameColon != null)
            {
                AdjustFlagsAndWidth(nameColon);
                this.nameColon = nameColon;
            }
            if (refKindKeyword != null)
            {
                AdjustFlagsAndWidth(refKindKeyword);
                this.refKindKeyword = refKindKeyword;
            }
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => nameColon,
                1 => refKindKeyword,
                2 => expression,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitArgument(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitArgument(this);
        }

        public ArgumentSyntax Update(NameColonSyntax nameColon, SyntaxToken refKindKeyword, ExpressionSyntax expression)
        {
            if (nameColon != NameColon || refKindKeyword != RefKindKeyword || expression != Expression)
            {
                ArgumentSyntax argumentSyntax = SyntaxFactory.Argument(nameColon, refKindKeyword, expression);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    argumentSyntax = argumentSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    argumentSyntax = argumentSyntax.WithAnnotationsGreen(annotations);
                }
                return argumentSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ArgumentSyntax(base.Kind, nameColon, refKindKeyword, expression, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ArgumentSyntax(base.Kind, nameColon, refKindKeyword, expression, GetDiagnostics(), annotations);
        }

        public ArgumentSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            NameColonSyntax nameColonSyntax = (NameColonSyntax)reader.ReadValue();
            if (nameColonSyntax != null)
            {
                AdjustFlagsAndWidth(nameColonSyntax);
                nameColon = nameColonSyntax;
            }
            SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
            if (syntaxToken != null)
            {
                AdjustFlagsAndWidth(syntaxToken);
                refKindKeyword = syntaxToken;
            }
            ExpressionSyntax node = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            expression = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(nameColon);
            writer.WriteValue(refKindKeyword);
            writer.WriteValue(expression);
        }

        static ArgumentSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ArgumentSyntax), (ObjectReader r) => new ArgumentSyntax(r));
        }
    }
}
