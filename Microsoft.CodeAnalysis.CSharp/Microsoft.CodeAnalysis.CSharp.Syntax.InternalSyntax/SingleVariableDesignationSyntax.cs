using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class SingleVariableDesignationSyntax : VariableDesignationSyntax
    {
        internal readonly SyntaxToken identifier;

        public SyntaxToken Identifier => identifier;

        public SingleVariableDesignationSyntax(SyntaxKind kind, SyntaxToken identifier, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
        }

        public SingleVariableDesignationSyntax(SyntaxKind kind, SyntaxToken identifier, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 1;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
        }

        public SingleVariableDesignationSyntax(SyntaxKind kind, SyntaxToken identifier)
            : base(kind)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
        }

        public override GreenNode? GetSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return identifier;
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.SingleVariableDesignationSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitSingleVariableDesignation(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitSingleVariableDesignation(this);
        }

        public SingleVariableDesignationSyntax Update(SyntaxToken identifier)
        {
            if (identifier != Identifier)
            {
                SingleVariableDesignationSyntax singleVariableDesignationSyntax = SyntaxFactory.SingleVariableDesignation(identifier);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    singleVariableDesignationSyntax = singleVariableDesignationSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    singleVariableDesignationSyntax = singleVariableDesignationSyntax.WithAnnotationsGreen(annotations);
                }
                return singleVariableDesignationSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new SingleVariableDesignationSyntax(base.Kind, identifier, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new SingleVariableDesignationSyntax(base.Kind, identifier, GetDiagnostics(), annotations);
        }

        public SingleVariableDesignationSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 1;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            identifier = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(identifier);
        }

        static SingleVariableDesignationSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(SingleVariableDesignationSyntax), (ObjectReader r) => new SingleVariableDesignationSyntax(r));
        }
    }
}
