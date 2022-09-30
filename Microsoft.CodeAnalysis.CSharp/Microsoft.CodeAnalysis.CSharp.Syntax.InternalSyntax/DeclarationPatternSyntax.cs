using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class DeclarationPatternSyntax : PatternSyntax
    {
        internal readonly TypeSyntax type;

        internal readonly VariableDesignationSyntax designation;

        public TypeSyntax Type => type;

        public VariableDesignationSyntax Designation => designation;

        public DeclarationPatternSyntax(SyntaxKind kind, TypeSyntax type, VariableDesignationSyntax designation, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(type);
            this.type = type;
            AdjustFlagsAndWidth(designation);
            this.designation = designation;
        }

        public DeclarationPatternSyntax(SyntaxKind kind, TypeSyntax type, VariableDesignationSyntax designation, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(type);
            this.type = type;
            AdjustFlagsAndWidth(designation);
            this.designation = designation;
        }

        public DeclarationPatternSyntax(SyntaxKind kind, TypeSyntax type, VariableDesignationSyntax designation)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(type);
            this.type = type;
            AdjustFlagsAndWidth(designation);
            this.designation = designation;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => type,
                1 => designation,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationPatternSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitDeclarationPattern(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitDeclarationPattern(this);
        }

        public DeclarationPatternSyntax Update(TypeSyntax type, VariableDesignationSyntax designation)
        {
            if (type != Type || designation != Designation)
            {
                DeclarationPatternSyntax declarationPatternSyntax = SyntaxFactory.DeclarationPattern(type, designation);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    declarationPatternSyntax = declarationPatternSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    declarationPatternSyntax = declarationPatternSyntax.WithAnnotationsGreen(annotations);
                }
                return declarationPatternSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new DeclarationPatternSyntax(base.Kind, type, designation, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new DeclarationPatternSyntax(base.Kind, type, designation, GetDiagnostics(), annotations);
        }

        public DeclarationPatternSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            TypeSyntax node = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            type = node;
            VariableDesignationSyntax node2 = (VariableDesignationSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            designation = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(type);
            writer.WriteValue(designation);
        }

        static DeclarationPatternSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(DeclarationPatternSyntax), (ObjectReader r) => new DeclarationPatternSyntax(r));
        }
    }
}
