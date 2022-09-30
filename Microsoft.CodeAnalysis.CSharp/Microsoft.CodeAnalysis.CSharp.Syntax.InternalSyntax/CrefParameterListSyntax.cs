using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class CrefParameterListSyntax : BaseCrefParameterListSyntax
    {
        internal readonly SyntaxToken openParenToken;

        internal readonly GreenNode? parameters;

        internal readonly SyntaxToken closeParenToken;

        public SyntaxToken OpenParenToken => openParenToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CrefParameterSyntax> Parameters => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CrefParameterSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(parameters));

        public SyntaxToken CloseParenToken => closeParenToken;

        public CrefParameterListSyntax(SyntaxKind kind, SyntaxToken openParenToken, GreenNode? parameters, SyntaxToken closeParenToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            if (parameters != null)
            {
                AdjustFlagsAndWidth(parameters);
                this.parameters = parameters;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public CrefParameterListSyntax(SyntaxKind kind, SyntaxToken openParenToken, GreenNode? parameters, SyntaxToken closeParenToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            if (parameters != null)
            {
                AdjustFlagsAndWidth(parameters);
                this.parameters = parameters;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public CrefParameterListSyntax(SyntaxKind kind, SyntaxToken openParenToken, GreenNode? parameters, SyntaxToken closeParenToken)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            if (parameters != null)
            {
                AdjustFlagsAndWidth(parameters);
                this.parameters = parameters;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => openParenToken,
                1 => parameters,
                2 => closeParenToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.CrefParameterListSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitCrefParameterList(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitCrefParameterList(this);
        }

        public CrefParameterListSyntax Update(SyntaxToken openParenToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CrefParameterSyntax> parameters, SyntaxToken closeParenToken)
        {
            if (openParenToken == OpenParenToken)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<CrefParameterSyntax> right = Parameters;
                if (!(parameters != right) && closeParenToken == CloseParenToken)
                {
                    return this;
                }
            }
            CrefParameterListSyntax crefParameterListSyntax = SyntaxFactory.CrefParameterList(openParenToken, parameters, closeParenToken);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                crefParameterListSyntax = crefParameterListSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                crefParameterListSyntax = crefParameterListSyntax.WithAnnotationsGreen(annotations);
            }
            return crefParameterListSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new CrefParameterListSyntax(base.Kind, openParenToken, parameters, closeParenToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new CrefParameterListSyntax(base.Kind, openParenToken, parameters, closeParenToken, GetDiagnostics(), annotations);
        }

        public CrefParameterListSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            openParenToken = node;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                parameters = greenNode;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            closeParenToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(openParenToken);
            writer.WriteValue(parameters);
            writer.WriteValue(closeParenToken);
        }

        static CrefParameterListSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(CrefParameterListSyntax), (ObjectReader r) => new CrefParameterListSyntax(r));
        }
    }
}
