using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class DeclarationExpressionSyntax : ExpressionSyntax
    {
        internal readonly TypeSyntax type;

        internal readonly VariableDesignationSyntax designation;

        public TypeSyntax Type => type;

        public VariableDesignationSyntax Designation => designation;

        public DeclarationExpressionSyntax(SyntaxKind kind, TypeSyntax type, VariableDesignationSyntax designation, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(type);
            this.type = type;
            AdjustFlagsAndWidth(designation);
            this.designation = designation;
        }

        public DeclarationExpressionSyntax(SyntaxKind kind, TypeSyntax type, VariableDesignationSyntax designation, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(type);
            this.type = type;
            AdjustFlagsAndWidth(designation);
            this.designation = designation;
        }

        public DeclarationExpressionSyntax(SyntaxKind kind, TypeSyntax type, VariableDesignationSyntax designation)
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
            return new Microsoft.CodeAnalysis.CSharp.Syntax.DeclarationExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitDeclarationExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitDeclarationExpression(this);
        }

        public DeclarationExpressionSyntax Update(TypeSyntax type, VariableDesignationSyntax designation)
        {
            if (type != Type || designation != Designation)
            {
                DeclarationExpressionSyntax declarationExpressionSyntax = SyntaxFactory.DeclarationExpression(type, designation);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    declarationExpressionSyntax = declarationExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    declarationExpressionSyntax = declarationExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return declarationExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new DeclarationExpressionSyntax(base.Kind, type, designation, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new DeclarationExpressionSyntax(base.Kind, type, designation, GetDiagnostics(), annotations);
        }

        public DeclarationExpressionSyntax(ObjectReader reader)
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

        static DeclarationExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(DeclarationExpressionSyntax), (ObjectReader r) => new DeclarationExpressionSyntax(r));
        }
    }
}
