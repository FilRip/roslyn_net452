using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class RecursivePatternSyntax : PatternSyntax
    {
        internal readonly TypeSyntax? type;

        internal readonly PositionalPatternClauseSyntax? positionalPatternClause;

        internal readonly PropertyPatternClauseSyntax? propertyPatternClause;

        internal readonly VariableDesignationSyntax? designation;

        public TypeSyntax? Type => type;

        public PositionalPatternClauseSyntax? PositionalPatternClause => positionalPatternClause;

        public PropertyPatternClauseSyntax? PropertyPatternClause => propertyPatternClause;

        public VariableDesignationSyntax? Designation => designation;

        public RecursivePatternSyntax(SyntaxKind kind, TypeSyntax? type, PositionalPatternClauseSyntax? positionalPatternClause, PropertyPatternClauseSyntax? propertyPatternClause, VariableDesignationSyntax? designation, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
            if (type != null)
            {
                AdjustFlagsAndWidth(type);
                this.type = type;
            }
            if (positionalPatternClause != null)
            {
                AdjustFlagsAndWidth(positionalPatternClause);
                this.positionalPatternClause = positionalPatternClause;
            }
            if (propertyPatternClause != null)
            {
                AdjustFlagsAndWidth(propertyPatternClause);
                this.propertyPatternClause = propertyPatternClause;
            }
            if (designation != null)
            {
                AdjustFlagsAndWidth(designation);
                this.designation = designation;
            }
        }

        public RecursivePatternSyntax(SyntaxKind kind, TypeSyntax? type, PositionalPatternClauseSyntax? positionalPatternClause, PropertyPatternClauseSyntax? propertyPatternClause, VariableDesignationSyntax? designation, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
            if (type != null)
            {
                AdjustFlagsAndWidth(type);
                this.type = type;
            }
            if (positionalPatternClause != null)
            {
                AdjustFlagsAndWidth(positionalPatternClause);
                this.positionalPatternClause = positionalPatternClause;
            }
            if (propertyPatternClause != null)
            {
                AdjustFlagsAndWidth(propertyPatternClause);
                this.propertyPatternClause = propertyPatternClause;
            }
            if (designation != null)
            {
                AdjustFlagsAndWidth(designation);
                this.designation = designation;
            }
        }

        public RecursivePatternSyntax(SyntaxKind kind, TypeSyntax? type, PositionalPatternClauseSyntax? positionalPatternClause, PropertyPatternClauseSyntax? propertyPatternClause, VariableDesignationSyntax? designation)
            : base(kind)
        {
            base.SlotCount = 4;
            if (type != null)
            {
                AdjustFlagsAndWidth(type);
                this.type = type;
            }
            if (positionalPatternClause != null)
            {
                AdjustFlagsAndWidth(positionalPatternClause);
                this.positionalPatternClause = positionalPatternClause;
            }
            if (propertyPatternClause != null)
            {
                AdjustFlagsAndWidth(propertyPatternClause);
                this.propertyPatternClause = propertyPatternClause;
            }
            if (designation != null)
            {
                AdjustFlagsAndWidth(designation);
                this.designation = designation;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => type,
                1 => positionalPatternClause,
                2 => propertyPatternClause,
                3 => designation,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.RecursivePatternSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitRecursivePattern(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitRecursivePattern(this);
        }

        public RecursivePatternSyntax Update(TypeSyntax type, PositionalPatternClauseSyntax positionalPatternClause, PropertyPatternClauseSyntax propertyPatternClause, VariableDesignationSyntax designation)
        {
            if (type != Type || positionalPatternClause != PositionalPatternClause || propertyPatternClause != PropertyPatternClause || designation != Designation)
            {
                RecursivePatternSyntax recursivePatternSyntax = SyntaxFactory.RecursivePattern(type, positionalPatternClause, propertyPatternClause, designation);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    recursivePatternSyntax = recursivePatternSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    recursivePatternSyntax = recursivePatternSyntax.WithAnnotationsGreen(annotations);
                }
                return recursivePatternSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new RecursivePatternSyntax(base.Kind, type, positionalPatternClause, propertyPatternClause, designation, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new RecursivePatternSyntax(base.Kind, type, positionalPatternClause, propertyPatternClause, designation, GetDiagnostics(), annotations);
        }

        public RecursivePatternSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 4;
            TypeSyntax typeSyntax = (TypeSyntax)reader.ReadValue();
            if (typeSyntax != null)
            {
                AdjustFlagsAndWidth(typeSyntax);
                type = typeSyntax;
            }
            PositionalPatternClauseSyntax positionalPatternClauseSyntax = (PositionalPatternClauseSyntax)reader.ReadValue();
            if (positionalPatternClauseSyntax != null)
            {
                AdjustFlagsAndWidth(positionalPatternClauseSyntax);
                positionalPatternClause = positionalPatternClauseSyntax;
            }
            PropertyPatternClauseSyntax propertyPatternClauseSyntax = (PropertyPatternClauseSyntax)reader.ReadValue();
            if (propertyPatternClauseSyntax != null)
            {
                AdjustFlagsAndWidth(propertyPatternClauseSyntax);
                propertyPatternClause = propertyPatternClauseSyntax;
            }
            VariableDesignationSyntax variableDesignationSyntax = (VariableDesignationSyntax)reader.ReadValue();
            if (variableDesignationSyntax != null)
            {
                AdjustFlagsAndWidth(variableDesignationSyntax);
                designation = variableDesignationSyntax;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(type);
            writer.WriteValue(positionalPatternClause);
            writer.WriteValue(propertyPatternClause);
            writer.WriteValue(designation);
        }

        static RecursivePatternSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(RecursivePatternSyntax), (ObjectReader r) => new RecursivePatternSyntax(r));
        }
    }
}
