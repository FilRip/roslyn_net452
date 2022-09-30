using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class BracketedParameterListSyntax : BaseParameterListSyntax
    {
        internal readonly SyntaxToken openBracketToken;

        internal readonly GreenNode? parameters;

        internal readonly SyntaxToken closeBracketToken;

        public SyntaxToken OpenBracketToken => openBracketToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax> Parameters => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(parameters));

        public SyntaxToken CloseBracketToken => closeBracketToken;

        public BracketedParameterListSyntax(SyntaxKind kind, SyntaxToken openBracketToken, GreenNode? parameters, SyntaxToken closeBracketToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openBracketToken);
            this.openBracketToken = openBracketToken;
            if (parameters != null)
            {
                AdjustFlagsAndWidth(parameters);
                this.parameters = parameters;
            }
            AdjustFlagsAndWidth(closeBracketToken);
            this.closeBracketToken = closeBracketToken;
        }

        public BracketedParameterListSyntax(SyntaxKind kind, SyntaxToken openBracketToken, GreenNode? parameters, SyntaxToken closeBracketToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openBracketToken);
            this.openBracketToken = openBracketToken;
            if (parameters != null)
            {
                AdjustFlagsAndWidth(parameters);
                this.parameters = parameters;
            }
            AdjustFlagsAndWidth(closeBracketToken);
            this.closeBracketToken = closeBracketToken;
        }

        public BracketedParameterListSyntax(SyntaxKind kind, SyntaxToken openBracketToken, GreenNode? parameters, SyntaxToken closeBracketToken)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openBracketToken);
            this.openBracketToken = openBracketToken;
            if (parameters != null)
            {
                AdjustFlagsAndWidth(parameters);
                this.parameters = parameters;
            }
            AdjustFlagsAndWidth(closeBracketToken);
            this.closeBracketToken = closeBracketToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => openBracketToken,
                1 => parameters,
                2 => closeBracketToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.BracketedParameterListSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitBracketedParameterList(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitBracketedParameterList(this);
        }

        public BracketedParameterListSyntax Update(SyntaxToken openBracketToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax> parameters, SyntaxToken closeBracketToken)
        {
            if (openBracketToken == OpenBracketToken)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax> right = Parameters;
                if (!(parameters != right) && closeBracketToken == CloseBracketToken)
                {
                    return this;
                }
            }
            BracketedParameterListSyntax bracketedParameterListSyntax = SyntaxFactory.BracketedParameterList(openBracketToken, parameters, closeBracketToken);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                bracketedParameterListSyntax = bracketedParameterListSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                bracketedParameterListSyntax = bracketedParameterListSyntax.WithAnnotationsGreen(annotations);
            }
            return bracketedParameterListSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new BracketedParameterListSyntax(base.Kind, openBracketToken, parameters, closeBracketToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new BracketedParameterListSyntax(base.Kind, openBracketToken, parameters, closeBracketToken, GetDiagnostics(), annotations);
        }

        public BracketedParameterListSyntax(ObjectReader reader)
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
                parameters = greenNode;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            closeBracketToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(openBracketToken);
            writer.WriteValue(parameters);
            writer.WriteValue(closeBracketToken);
        }

        static BracketedParameterListSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(BracketedParameterListSyntax), (ObjectReader r) => new BracketedParameterListSyntax(r));
        }
    }
}
