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
    public sealed class VariableDeclarationSyntax : CSharpSyntaxNode
    {
        internal readonly TypeSyntax type;

        internal readonly GreenNode? variables;

        public TypeSyntax Type => type;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDeclaratorSyntax> Variables => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDeclaratorSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(variables));

        public VariableDeclarationSyntax(SyntaxKind kind, TypeSyntax type, GreenNode? variables, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(type);
            this.type = type;
            if (variables != null)
            {
                AdjustFlagsAndWidth(variables);
                this.variables = variables;
            }
        }

        public VariableDeclarationSyntax(SyntaxKind kind, TypeSyntax type, GreenNode? variables, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(type);
            this.type = type;
            if (variables != null)
            {
                AdjustFlagsAndWidth(variables);
                this.variables = variables;
            }
        }

        public VariableDeclarationSyntax(SyntaxKind kind, TypeSyntax type, GreenNode? variables)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(type);
            this.type = type;
            if (variables != null)
            {
                AdjustFlagsAndWidth(variables);
                this.variables = variables;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => type,
                1 => variables,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitVariableDeclaration(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitVariableDeclaration(this);
        }

        public VariableDeclarationSyntax Update(TypeSyntax type, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDeclaratorSyntax> variables)
        {
            if (type == Type)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<VariableDeclaratorSyntax> right = Variables;
                if (!(variables != right))
                {
                    return this;
                }
            }
            VariableDeclarationSyntax variableDeclarationSyntax = SyntaxFactory.VariableDeclaration(type, variables);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                variableDeclarationSyntax = variableDeclarationSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                variableDeclarationSyntax = variableDeclarationSyntax.WithAnnotationsGreen(annotations);
            }
            return variableDeclarationSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new VariableDeclarationSyntax(base.Kind, type, variables, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new VariableDeclarationSyntax(base.Kind, type, variables, GetDiagnostics(), annotations);
        }

        public VariableDeclarationSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            TypeSyntax node = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            type = node;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                variables = greenNode;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(type);
            writer.WriteValue(variables);
        }

        static VariableDeclarationSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(VariableDeclarationSyntax), (ObjectReader r) => new VariableDeclarationSyntax(r));
        }
    }
}
