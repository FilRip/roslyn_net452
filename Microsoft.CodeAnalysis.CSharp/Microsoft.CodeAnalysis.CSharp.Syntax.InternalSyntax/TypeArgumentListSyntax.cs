using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class TypeArgumentListSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken lessThanToken;

        internal readonly GreenNode? arguments;

        internal readonly SyntaxToken greaterThanToken;

        public SyntaxToken LessThanToken => lessThanToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeSyntax> Arguments => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(arguments));

        public SyntaxToken GreaterThanToken => greaterThanToken;

        public TypeArgumentListSyntax(SyntaxKind kind, SyntaxToken lessThanToken, GreenNode? arguments, SyntaxToken greaterThanToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(lessThanToken);
            this.lessThanToken = lessThanToken;
            if (arguments != null)
            {
                AdjustFlagsAndWidth(arguments);
                this.arguments = arguments;
            }
            AdjustFlagsAndWidth(greaterThanToken);
            this.greaterThanToken = greaterThanToken;
        }

        public TypeArgumentListSyntax(SyntaxKind kind, SyntaxToken lessThanToken, GreenNode? arguments, SyntaxToken greaterThanToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(lessThanToken);
            this.lessThanToken = lessThanToken;
            if (arguments != null)
            {
                AdjustFlagsAndWidth(arguments);
                this.arguments = arguments;
            }
            AdjustFlagsAndWidth(greaterThanToken);
            this.greaterThanToken = greaterThanToken;
        }

        public TypeArgumentListSyntax(SyntaxKind kind, SyntaxToken lessThanToken, GreenNode? arguments, SyntaxToken greaterThanToken)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(lessThanToken);
            this.lessThanToken = lessThanToken;
            if (arguments != null)
            {
                AdjustFlagsAndWidth(arguments);
                this.arguments = arguments;
            }
            AdjustFlagsAndWidth(greaterThanToken);
            this.greaterThanToken = greaterThanToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => lessThanToken,
                1 => arguments,
                2 => greaterThanToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.TypeArgumentListSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitTypeArgumentList(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitTypeArgumentList(this);
        }

        public TypeArgumentListSyntax Update(SyntaxToken lessThanToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeSyntax> arguments, SyntaxToken greaterThanToken)
        {
            if (lessThanToken == LessThanToken)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<TypeSyntax> right = Arguments;
                if (!(arguments != right) && greaterThanToken == GreaterThanToken)
                {
                    return this;
                }
            }
            TypeArgumentListSyntax typeArgumentListSyntax = SyntaxFactory.TypeArgumentList(lessThanToken, arguments, greaterThanToken);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                typeArgumentListSyntax = typeArgumentListSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                typeArgumentListSyntax = typeArgumentListSyntax.WithAnnotationsGreen(annotations);
            }
            return typeArgumentListSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new TypeArgumentListSyntax(base.Kind, lessThanToken, arguments, greaterThanToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new TypeArgumentListSyntax(base.Kind, lessThanToken, arguments, greaterThanToken, GetDiagnostics(), annotations);
        }

        public TypeArgumentListSyntax(ObjectReader reader)
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
                arguments = greenNode;
            }
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            greaterThanToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(lessThanToken);
            writer.WriteValue(arguments);
            writer.WriteValue(greaterThanToken);
        }

        static TypeArgumentListSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(TypeArgumentListSyntax), (ObjectReader r) => new TypeArgumentListSyntax(r));
        }
    }
}
