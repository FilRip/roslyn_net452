using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class QualifiedCrefSyntax : CrefSyntax
    {
        internal readonly TypeSyntax container;

        internal readonly SyntaxToken dotToken;

        internal readonly MemberCrefSyntax member;

        public TypeSyntax Container => container;

        public SyntaxToken DotToken => dotToken;

        public MemberCrefSyntax Member => member;

        public QualifiedCrefSyntax(SyntaxKind kind, TypeSyntax container, SyntaxToken dotToken, MemberCrefSyntax member, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(container);
            this.container = container;
            AdjustFlagsAndWidth(dotToken);
            this.dotToken = dotToken;
            AdjustFlagsAndWidth(member);
            this.member = member;
        }

        public QualifiedCrefSyntax(SyntaxKind kind, TypeSyntax container, SyntaxToken dotToken, MemberCrefSyntax member, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(container);
            this.container = container;
            AdjustFlagsAndWidth(dotToken);
            this.dotToken = dotToken;
            AdjustFlagsAndWidth(member);
            this.member = member;
        }

        public QualifiedCrefSyntax(SyntaxKind kind, TypeSyntax container, SyntaxToken dotToken, MemberCrefSyntax member)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(container);
            this.container = container;
            AdjustFlagsAndWidth(dotToken);
            this.dotToken = dotToken;
            AdjustFlagsAndWidth(member);
            this.member = member;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => container,
                1 => dotToken,
                2 => member,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedCrefSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitQualifiedCref(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitQualifiedCref(this);
        }

        public QualifiedCrefSyntax Update(TypeSyntax container, SyntaxToken dotToken, MemberCrefSyntax member)
        {
            if (container != Container || dotToken != DotToken || member != Member)
            {
                QualifiedCrefSyntax qualifiedCrefSyntax = SyntaxFactory.QualifiedCref(container, dotToken, member);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    qualifiedCrefSyntax = qualifiedCrefSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    qualifiedCrefSyntax = qualifiedCrefSyntax.WithAnnotationsGreen(annotations);
                }
                return qualifiedCrefSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new QualifiedCrefSyntax(base.Kind, container, dotToken, member, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new QualifiedCrefSyntax(base.Kind, container, dotToken, member, GetDiagnostics(), annotations);
        }

        public QualifiedCrefSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            TypeSyntax node = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            container = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            dotToken = node2;
            MemberCrefSyntax node3 = (MemberCrefSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            member = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(container);
            writer.WriteValue(dotToken);
            writer.WriteValue(member);
        }

        static QualifiedCrefSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(QualifiedCrefSyntax), (ObjectReader r) => new QualifiedCrefSyntax(r));
        }
    }
}
