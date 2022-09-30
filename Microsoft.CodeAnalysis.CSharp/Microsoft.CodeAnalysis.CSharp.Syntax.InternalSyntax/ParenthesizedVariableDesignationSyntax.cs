using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ParenthesizedVariableDesignationSyntax : VariableDesignationSyntax
    {
        internal readonly SyntaxToken openParenToken;

        internal readonly GreenNode? variables;

        internal readonly SyntaxToken closeParenToken;

        public SyntaxToken OpenParenToken => openParenToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDesignationSyntax> Variables => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDesignationSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(variables));

        public SyntaxToken CloseParenToken => closeParenToken;

        public ParenthesizedVariableDesignationSyntax(SyntaxKind kind, SyntaxToken openParenToken, GreenNode? variables, SyntaxToken closeParenToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            if (variables != null)
            {
                AdjustFlagsAndWidth(variables);
                this.variables = variables;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public ParenthesizedVariableDesignationSyntax(SyntaxKind kind, SyntaxToken openParenToken, GreenNode? variables, SyntaxToken closeParenToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            if (variables != null)
            {
                AdjustFlagsAndWidth(variables);
                this.variables = variables;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public ParenthesizedVariableDesignationSyntax(SyntaxKind kind, SyntaxToken openParenToken, GreenNode? variables, SyntaxToken closeParenToken)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            if (variables != null)
            {
                AdjustFlagsAndWidth(variables);
                this.variables = variables;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => openParenToken,
                1 => variables,
                2 => closeParenToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedVariableDesignationSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitParenthesizedVariableDesignation(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitParenthesizedVariableDesignation(this);
        }

        public ParenthesizedVariableDesignationSyntax Update(SyntaxToken openParenToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDesignationSyntax> variables, SyntaxToken closeParenToken)
        {
            if (openParenToken == OpenParenToken)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDesignationSyntax> right = Variables;
                if (!(variables != right) && closeParenToken == CloseParenToken)
                {
                    return this;
                }
            }
            ParenthesizedVariableDesignationSyntax parenthesizedVariableDesignationSyntax = SyntaxFactory.ParenthesizedVariableDesignation(openParenToken, variables, closeParenToken);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                parenthesizedVariableDesignationSyntax = parenthesizedVariableDesignationSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                parenthesizedVariableDesignationSyntax = parenthesizedVariableDesignationSyntax.WithAnnotationsGreen(annotations);
            }
            return parenthesizedVariableDesignationSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ParenthesizedVariableDesignationSyntax(base.Kind, openParenToken, variables, closeParenToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ParenthesizedVariableDesignationSyntax(base.Kind, openParenToken, variables, closeParenToken, GetDiagnostics(), annotations);
        }

        public ParenthesizedVariableDesignationSyntax(ObjectReader reader)
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
                variables = greenNode;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            closeParenToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(openParenToken);
            writer.WriteValue(variables);
            writer.WriteValue(closeParenToken);
        }

        static ParenthesizedVariableDesignationSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ParenthesizedVariableDesignationSyntax), (ObjectReader r) => new ParenthesizedVariableDesignationSyntax(r));
        }
    }
}
