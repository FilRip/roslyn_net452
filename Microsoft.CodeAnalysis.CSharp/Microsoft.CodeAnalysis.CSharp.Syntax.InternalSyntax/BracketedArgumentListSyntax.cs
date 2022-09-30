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
    public sealed class BracketedArgumentListSyntax : BaseArgumentListSyntax
    {
        internal readonly SyntaxToken openBracketToken;

        internal readonly GreenNode? arguments;

        internal readonly SyntaxToken closeBracketToken;

        public SyntaxToken OpenBracketToken => openBracketToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax> Arguments => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(arguments));

        public SyntaxToken CloseBracketToken => closeBracketToken;

        public BracketedArgumentListSyntax(SyntaxKind kind, SyntaxToken openBracketToken, GreenNode? arguments, SyntaxToken closeBracketToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openBracketToken);
            this.openBracketToken = openBracketToken;
            if (arguments != null)
            {
                AdjustFlagsAndWidth(arguments);
                this.arguments = arguments;
            }
            AdjustFlagsAndWidth(closeBracketToken);
            this.closeBracketToken = closeBracketToken;
        }

        public BracketedArgumentListSyntax(SyntaxKind kind, SyntaxToken openBracketToken, GreenNode? arguments, SyntaxToken closeBracketToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openBracketToken);
            this.openBracketToken = openBracketToken;
            if (arguments != null)
            {
                AdjustFlagsAndWidth(arguments);
                this.arguments = arguments;
            }
            AdjustFlagsAndWidth(closeBracketToken);
            this.closeBracketToken = closeBracketToken;
        }

        public BracketedArgumentListSyntax(SyntaxKind kind, SyntaxToken openBracketToken, GreenNode? arguments, SyntaxToken closeBracketToken)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openBracketToken);
            this.openBracketToken = openBracketToken;
            if (arguments != null)
            {
                AdjustFlagsAndWidth(arguments);
                this.arguments = arguments;
            }
            AdjustFlagsAndWidth(closeBracketToken);
            this.closeBracketToken = closeBracketToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => openBracketToken,
                1 => arguments,
                2 => closeBracketToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.BracketedArgumentListSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitBracketedArgumentList(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitBracketedArgumentList(this);
        }

        public BracketedArgumentListSyntax Update(SyntaxToken openBracketToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax> arguments, SyntaxToken closeBracketToken)
        {
            if (openBracketToken == OpenBracketToken)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax> right = Arguments;
                if (!(arguments != right) && closeBracketToken == CloseBracketToken)
                {
                    return this;
                }
            }
            BracketedArgumentListSyntax bracketedArgumentListSyntax = SyntaxFactory.BracketedArgumentList(openBracketToken, arguments, closeBracketToken);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                bracketedArgumentListSyntax = bracketedArgumentListSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                bracketedArgumentListSyntax = bracketedArgumentListSyntax.WithAnnotationsGreen(annotations);
            }
            return bracketedArgumentListSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new BracketedArgumentListSyntax(base.Kind, openBracketToken, arguments, closeBracketToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new BracketedArgumentListSyntax(base.Kind, openBracketToken, arguments, closeBracketToken, GetDiagnostics(), annotations);
        }

        public BracketedArgumentListSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            openBracketToken = node;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                arguments = greenNode;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            closeBracketToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(openBracketToken);
            writer.WriteValue(arguments);
            writer.WriteValue(closeBracketToken);
        }

        static BracketedArgumentListSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(BracketedArgumentListSyntax), (ObjectReader r) => new BracketedArgumentListSyntax(r));
        }
    }
}
