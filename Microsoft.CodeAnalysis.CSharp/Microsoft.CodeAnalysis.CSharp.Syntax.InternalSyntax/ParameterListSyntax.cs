using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ParameterListSyntax : BaseParameterListSyntax
    {
        internal readonly SyntaxToken openParenToken;

        internal readonly GreenNode? parameters;

        internal readonly SyntaxToken closeParenToken;

        public SyntaxToken OpenParenToken => openParenToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax> Parameters => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(parameters));

        public SyntaxToken CloseParenToken => closeParenToken;

        public ParameterListSyntax(SyntaxKind kind, SyntaxToken openParenToken, GreenNode? parameters, SyntaxToken closeParenToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
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

        public ParameterListSyntax(SyntaxKind kind, SyntaxToken openParenToken, GreenNode? parameters, SyntaxToken closeParenToken, SyntaxFactoryContext context)
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

        public ParameterListSyntax(SyntaxKind kind, SyntaxToken openParenToken, GreenNode? parameters, SyntaxToken closeParenToken)
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
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitParameterList(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitParameterList(this);
        }

        public ParameterListSyntax Update(SyntaxToken openParenToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax> parameters, SyntaxToken closeParenToken)
        {
            if (openParenToken == OpenParenToken)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax> right = Parameters;
                if (!(parameters != right) && closeParenToken == CloseParenToken)
                {
                    return this;
                }
            }
            ParameterListSyntax parameterListSyntax = SyntaxFactory.ParameterList(openParenToken, parameters, closeParenToken);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                parameterListSyntax = parameterListSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                parameterListSyntax = parameterListSyntax.WithAnnotationsGreen(annotations);
            }
            return parameterListSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ParameterListSyntax(base.Kind, openParenToken, parameters, closeParenToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ParameterListSyntax(base.Kind, openParenToken, parameters, closeParenToken, GetDiagnostics(), annotations);
        }

        public ParameterListSyntax(ObjectReader reader)
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

        static ParameterListSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ParameterListSyntax), (ObjectReader r) => new ParameterListSyntax(r));
        }
    }
}
