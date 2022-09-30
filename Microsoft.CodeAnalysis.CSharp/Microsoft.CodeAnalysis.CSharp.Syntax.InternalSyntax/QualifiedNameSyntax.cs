using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class QualifiedNameSyntax : NameSyntax
    {
        internal readonly NameSyntax left;

        internal readonly SyntaxToken dotToken;

        internal readonly SimpleNameSyntax right;

        public NameSyntax Left => left;

        public SyntaxToken DotToken => dotToken;

        public SimpleNameSyntax Right => right;

        public QualifiedNameSyntax(SyntaxKind kind, NameSyntax left, SyntaxToken dotToken, SimpleNameSyntax right, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(left);
            this.left = left;
            AdjustFlagsAndWidth(dotToken);
            this.dotToken = dotToken;
            AdjustFlagsAndWidth(right);
            this.right = right;
        }

        public QualifiedNameSyntax(SyntaxKind kind, NameSyntax left, SyntaxToken dotToken, SimpleNameSyntax right, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(left);
            this.left = left;
            AdjustFlagsAndWidth(dotToken);
            this.dotToken = dotToken;
            AdjustFlagsAndWidth(right);
            this.right = right;
        }

        public QualifiedNameSyntax(SyntaxKind kind, NameSyntax left, SyntaxToken dotToken, SimpleNameSyntax right)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(left);
            this.left = left;
            AdjustFlagsAndWidth(dotToken);
            this.dotToken = dotToken;
            AdjustFlagsAndWidth(right);
            this.right = right;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => left,
                1 => dotToken,
                2 => right,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitQualifiedName(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitQualifiedName(this);
        }

        public QualifiedNameSyntax Update(NameSyntax left, SyntaxToken dotToken, SimpleNameSyntax right)
        {
            if (left != Left || dotToken != DotToken || right != Right)
            {
                QualifiedNameSyntax qualifiedNameSyntax = SyntaxFactory.QualifiedName(left, dotToken, right);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    qualifiedNameSyntax = qualifiedNameSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    qualifiedNameSyntax = qualifiedNameSyntax.WithAnnotationsGreen(annotations);
                }
                return qualifiedNameSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new QualifiedNameSyntax(base.Kind, left, dotToken, right, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new QualifiedNameSyntax(base.Kind, left, dotToken, right, GetDiagnostics(), annotations);
        }

        public QualifiedNameSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            NameSyntax node = (NameSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            left = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            dotToken = node2;
            SimpleNameSyntax node3 = (SimpleNameSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            right = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(left);
            writer.WriteValue(dotToken);
            writer.WriteValue(right);
        }

        static QualifiedNameSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(QualifiedNameSyntax), (ObjectReader r) => new QualifiedNameSyntax(r));
        }
    }
}
