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
    public sealed class TypeParameterConstraintClauseSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken whereKeyword;

        internal readonly IdentifierNameSyntax name;

        internal readonly SyntaxToken colonToken;

        internal readonly GreenNode? constraints;

        public SyntaxToken WhereKeyword => whereKeyword;

        public IdentifierNameSyntax Name => name;

        public SyntaxToken ColonToken => colonToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeParameterConstraintSyntax> Constraints => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeParameterConstraintSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(constraints));

        public TypeParameterConstraintClauseSyntax(SyntaxKind kind, SyntaxToken whereKeyword, IdentifierNameSyntax name, SyntaxToken colonToken, GreenNode? constraints, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(whereKeyword);
            this.whereKeyword = whereKeyword;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
            if (constraints != null)
            {
                AdjustFlagsAndWidth(constraints);
                this.constraints = constraints;
            }
        }

        public TypeParameterConstraintClauseSyntax(SyntaxKind kind, SyntaxToken whereKeyword, IdentifierNameSyntax name, SyntaxToken colonToken, GreenNode? constraints, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
            AdjustFlagsAndWidth(whereKeyword);
            this.whereKeyword = whereKeyword;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
            if (constraints != null)
            {
                AdjustFlagsAndWidth(constraints);
                this.constraints = constraints;
            }
        }

        public TypeParameterConstraintClauseSyntax(SyntaxKind kind, SyntaxToken whereKeyword, IdentifierNameSyntax name, SyntaxToken colonToken, GreenNode? constraints)
            : base(kind)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(whereKeyword);
            this.whereKeyword = whereKeyword;
            AdjustFlagsAndWidth(name);
            this.name = name;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
            if (constraints != null)
            {
                AdjustFlagsAndWidth(constraints);
                this.constraints = constraints;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => whereKeyword,
                1 => name,
                2 => colonToken,
                3 => constraints,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterConstraintClauseSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitTypeParameterConstraintClause(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitTypeParameterConstraintClause(this);
        }

        public TypeParameterConstraintClauseSyntax Update(SyntaxToken whereKeyword, IdentifierNameSyntax name, SyntaxToken colonToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeParameterConstraintSyntax> constraints)
        {
            if (whereKeyword == WhereKeyword && name == Name && colonToken == ColonToken)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeParameterConstraintSyntax> right = Constraints;
                if (!(constraints != right))
                {
                    return this;
                }
            }
            TypeParameterConstraintClauseSyntax typeParameterConstraintClauseSyntax = SyntaxFactory.TypeParameterConstraintClause(whereKeyword, name, colonToken, constraints);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                typeParameterConstraintClauseSyntax = typeParameterConstraintClauseSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                typeParameterConstraintClauseSyntax = typeParameterConstraintClauseSyntax.WithAnnotationsGreen(annotations);
            }
            return typeParameterConstraintClauseSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new TypeParameterConstraintClauseSyntax(base.Kind, whereKeyword, name, colonToken, constraints, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new TypeParameterConstraintClauseSyntax(base.Kind, whereKeyword, name, colonToken, constraints, GetDiagnostics(), annotations);
        }

        public TypeParameterConstraintClauseSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 4;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            whereKeyword = node;
            IdentifierNameSyntax node2 = (IdentifierNameSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            name = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            colonToken = node3;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                constraints = greenNode;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(whereKeyword);
            writer.WriteValue(name);
            writer.WriteValue(colonToken);
            writer.WriteValue(constraints);
        }

        static TypeParameterConstraintClauseSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(TypeParameterConstraintClauseSyntax), (ObjectReader r) => new TypeParameterConstraintClauseSyntax(r));
        }
    }
}
