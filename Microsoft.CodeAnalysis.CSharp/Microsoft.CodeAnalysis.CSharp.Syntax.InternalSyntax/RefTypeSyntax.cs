using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class RefTypeSyntax : TypeSyntax
    {
        internal readonly SyntaxToken refKeyword;

        internal readonly SyntaxToken? readOnlyKeyword;

        internal readonly TypeSyntax type;

        public SyntaxToken RefKeyword => refKeyword;

        public SyntaxToken? ReadOnlyKeyword => readOnlyKeyword;

        public TypeSyntax Type => type;

        public RefTypeSyntax(SyntaxKind kind, SyntaxToken refKeyword, SyntaxToken? readOnlyKeyword, TypeSyntax type, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(refKeyword);
            this.refKeyword = refKeyword;
            if (readOnlyKeyword != null)
            {
                AdjustFlagsAndWidth(readOnlyKeyword);
                this.readOnlyKeyword = readOnlyKeyword;
            }
            AdjustFlagsAndWidth(type);
            this.type = type;
        }

        public RefTypeSyntax(SyntaxKind kind, SyntaxToken refKeyword, SyntaxToken? readOnlyKeyword, TypeSyntax type, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(refKeyword);
            this.refKeyword = refKeyword;
            if (readOnlyKeyword != null)
            {
                AdjustFlagsAndWidth(readOnlyKeyword);
                this.readOnlyKeyword = readOnlyKeyword;
            }
            AdjustFlagsAndWidth(type);
            this.type = type;
        }

        public RefTypeSyntax(SyntaxKind kind, SyntaxToken refKeyword, SyntaxToken? readOnlyKeyword, TypeSyntax type)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(refKeyword);
            this.refKeyword = refKeyword;
            if (readOnlyKeyword != null)
            {
                AdjustFlagsAndWidth(readOnlyKeyword);
                this.readOnlyKeyword = readOnlyKeyword;
            }
            AdjustFlagsAndWidth(type);
            this.type = type;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => refKeyword,
                1 => readOnlyKeyword,
                2 => type,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.RefTypeSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitRefType(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitRefType(this);
        }

        public RefTypeSyntax Update(SyntaxToken refKeyword, SyntaxToken readOnlyKeyword, TypeSyntax type)
        {
            if (refKeyword != RefKeyword || readOnlyKeyword != ReadOnlyKeyword || type != Type)
            {
                RefTypeSyntax refTypeSyntax = SyntaxFactory.RefType(refKeyword, readOnlyKeyword, type);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    refTypeSyntax = refTypeSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    refTypeSyntax = refTypeSyntax.WithAnnotationsGreen(annotations);
                }
                return refTypeSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new RefTypeSyntax(base.Kind, refKeyword, readOnlyKeyword, type, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new RefTypeSyntax(base.Kind, refKeyword, readOnlyKeyword, type, GetDiagnostics(), annotations);
        }

        public RefTypeSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            refKeyword = node;
            SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
            if (syntaxToken != null)
            {
                AdjustFlagsAndWidth(syntaxToken);
                readOnlyKeyword = syntaxToken;
            }
            TypeSyntax node2 = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            type = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(refKeyword);
            writer.WriteValue(readOnlyKeyword);
            writer.WriteValue(type);
        }

        static RefTypeSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(RefTypeSyntax), (ObjectReader r) => new RefTypeSyntax(r));
        }
    }
}
