using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class TypeParameterListSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken lessThanToken;

        internal readonly GreenNode? parameters;

        internal readonly SyntaxToken greaterThanToken;

        public SyntaxToken LessThanToken => lessThanToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeParameterSyntax> Parameters => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeParameterSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(parameters));

        public SyntaxToken GreaterThanToken => greaterThanToken;

        public TypeParameterListSyntax(SyntaxKind kind, SyntaxToken lessThanToken, GreenNode? parameters, SyntaxToken greaterThanToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(lessThanToken);
            this.lessThanToken = lessThanToken;
            if (parameters != null)
            {
                AdjustFlagsAndWidth(parameters);
                this.parameters = parameters;
            }
            AdjustFlagsAndWidth(greaterThanToken);
            this.greaterThanToken = greaterThanToken;
        }

        public TypeParameterListSyntax(SyntaxKind kind, SyntaxToken lessThanToken, GreenNode? parameters, SyntaxToken greaterThanToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(lessThanToken);
            this.lessThanToken = lessThanToken;
            if (parameters != null)
            {
                AdjustFlagsAndWidth(parameters);
                this.parameters = parameters;
            }
            AdjustFlagsAndWidth(greaterThanToken);
            this.greaterThanToken = greaterThanToken;
        }

        public TypeParameterListSyntax(SyntaxKind kind, SyntaxToken lessThanToken, GreenNode? parameters, SyntaxToken greaterThanToken)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(lessThanToken);
            this.lessThanToken = lessThanToken;
            if (parameters != null)
            {
                AdjustFlagsAndWidth(parameters);
                this.parameters = parameters;
            }
            AdjustFlagsAndWidth(greaterThanToken);
            this.greaterThanToken = greaterThanToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => lessThanToken,
                1 => parameters,
                2 => greaterThanToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.TypeParameterListSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitTypeParameterList(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitTypeParameterList(this);
        }

        public TypeParameterListSyntax Update(SyntaxToken lessThanToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeParameterSyntax> parameters, SyntaxToken greaterThanToken)
        {
            if (lessThanToken == LessThanToken)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeParameterSyntax> right = Parameters;
                if (!(parameters != right) && greaterThanToken == GreaterThanToken)
                {
                    return this;
                }
            }
            TypeParameterListSyntax typeParameterListSyntax = SyntaxFactory.TypeParameterList(lessThanToken, parameters, greaterThanToken);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                typeParameterListSyntax = typeParameterListSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                typeParameterListSyntax = typeParameterListSyntax.WithAnnotationsGreen(annotations);
            }
            return typeParameterListSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new TypeParameterListSyntax(base.Kind, lessThanToken, parameters, greaterThanToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new TypeParameterListSyntax(base.Kind, lessThanToken, parameters, greaterThanToken, GetDiagnostics(), annotations);
        }

        public TypeParameterListSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            lessThanToken = node;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                parameters = greenNode;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            greaterThanToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(lessThanToken);
            writer.WriteValue(parameters);
            writer.WriteValue(greaterThanToken);
        }

        static TypeParameterListSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(TypeParameterListSyntax), (ObjectReader r) => new TypeParameterListSyntax(r));
        }
    }
}
