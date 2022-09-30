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
    public sealed class BaseListSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken colonToken;

        internal readonly GreenNode? types;

        public SyntaxToken ColonToken => colonToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<BaseTypeSyntax> Types => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<BaseTypeSyntax>(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<CSharpSyntaxNode>(types));

        public BaseListSyntax(SyntaxKind kind, SyntaxToken colonToken, GreenNode? types, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
            if (types != null)
            {
                AdjustFlagsAndWidth(types);
                this.types = types;
            }
        }

        public BaseListSyntax(SyntaxKind kind, SyntaxToken colonToken, GreenNode? types, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
            if (types != null)
            {
                AdjustFlagsAndWidth(types);
                this.types = types;
            }
        }

        public BaseListSyntax(SyntaxKind kind, SyntaxToken colonToken, GreenNode? types)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
            if (types != null)
            {
                AdjustFlagsAndWidth(types);
                this.types = types;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => colonToken,
                1 => types,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.BaseListSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitBaseList(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitBaseList(this);
        }

        public BaseListSyntax Update(SyntaxToken colonToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<BaseTypeSyntax> types)
        {
            if (colonToken == ColonToken)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<BaseTypeSyntax> right = Types;
                if (!(types != right))
                {
                    return this;
                }
            }
            BaseListSyntax baseListSyntax = SyntaxFactory.BaseList(colonToken, types);
            DiagnosticInfo[] diagnostics = GetDiagnostics();
            if (diagnostics != null && diagnostics.Length != 0)
            {
                baseListSyntax = baseListSyntax.WithDiagnosticsGreen(diagnostics);
            }
            SyntaxAnnotation[] annotations = GetAnnotations();
            if (annotations != null && annotations.Length != 0)
            {
                baseListSyntax = baseListSyntax.WithAnnotationsGreen(annotations);
            }
            return baseListSyntax;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new BaseListSyntax(base.Kind, colonToken, types, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new BaseListSyntax(base.Kind, colonToken, types, GetDiagnostics(), annotations);
        }

        public BaseListSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            colonToken = node;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                types = greenNode;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(colonToken);
            writer.WriteValue(types);
        }

        static BaseListSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(BaseListSyntax), (ObjectReader r) => new BaseListSyntax(r));
        }
    }
}
