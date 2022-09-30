using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class VarPatternSyntax : PatternSyntax
    {
        internal readonly SyntaxToken varKeyword;

        internal readonly VariableDesignationSyntax designation;

        public SyntaxToken VarKeyword => varKeyword;

        public VariableDesignationSyntax Designation => designation;

        public VarPatternSyntax(SyntaxKind kind, SyntaxToken varKeyword, VariableDesignationSyntax designation, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(varKeyword);
            this.varKeyword = varKeyword;
            AdjustFlagsAndWidth(designation);
            this.designation = designation;
        }

        public VarPatternSyntax(SyntaxKind kind, SyntaxToken varKeyword, VariableDesignationSyntax designation, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(varKeyword);
            this.varKeyword = varKeyword;
            AdjustFlagsAndWidth(designation);
            this.designation = designation;
        }

        public VarPatternSyntax(SyntaxKind kind, SyntaxToken varKeyword, VariableDesignationSyntax designation)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(varKeyword);
            this.varKeyword = varKeyword;
            AdjustFlagsAndWidth(designation);
            this.designation = designation;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => varKeyword,
                1 => designation,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.VarPatternSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitVarPattern(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitVarPattern(this);
        }

        public VarPatternSyntax Update(SyntaxToken varKeyword, VariableDesignationSyntax designation)
        {
            if (varKeyword != VarKeyword || designation != Designation)
            {
                VarPatternSyntax varPatternSyntax = SyntaxFactory.VarPattern(varKeyword, designation);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    varPatternSyntax = varPatternSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    varPatternSyntax = varPatternSyntax.WithAnnotationsGreen(annotations);
                }
                return varPatternSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new VarPatternSyntax(base.Kind, varKeyword, designation, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new VarPatternSyntax(base.Kind, varKeyword, designation, GetDiagnostics(), annotations);
        }

        public VarPatternSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            varKeyword = node;
            VariableDesignationSyntax node2 = (VariableDesignationSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            designation = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(varKeyword);
            writer.WriteValue(designation);
        }

        static VarPatternSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(VarPatternSyntax), (ObjectReader r) => new VarPatternSyntax(r));
        }
    }
}
