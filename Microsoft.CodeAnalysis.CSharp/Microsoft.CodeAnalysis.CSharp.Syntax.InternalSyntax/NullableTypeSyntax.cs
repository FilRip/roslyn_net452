using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class NullableTypeSyntax : TypeSyntax
    {
        internal readonly TypeSyntax elementType;

        internal readonly SyntaxToken questionToken;

        public TypeSyntax ElementType => elementType;

        public SyntaxToken QuestionToken => questionToken;

        public NullableTypeSyntax(SyntaxKind kind, TypeSyntax elementType, SyntaxToken questionToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(elementType);
            this.elementType = elementType;
            AdjustFlagsAndWidth(questionToken);
            this.questionToken = questionToken;
        }

        public NullableTypeSyntax(SyntaxKind kind, TypeSyntax elementType, SyntaxToken questionToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(elementType);
            this.elementType = elementType;
            AdjustFlagsAndWidth(questionToken);
            this.questionToken = questionToken;
        }

        public NullableTypeSyntax(SyntaxKind kind, TypeSyntax elementType, SyntaxToken questionToken)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(elementType);
            this.elementType = elementType;
            AdjustFlagsAndWidth(questionToken);
            this.questionToken = questionToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => elementType,
                1 => questionToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.NullableTypeSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitNullableType(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitNullableType(this);
        }

        public NullableTypeSyntax Update(TypeSyntax elementType, SyntaxToken questionToken)
        {
            if (elementType != ElementType || questionToken != QuestionToken)
            {
                NullableTypeSyntax nullableTypeSyntax = SyntaxFactory.NullableType(elementType, questionToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    nullableTypeSyntax = nullableTypeSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    nullableTypeSyntax = nullableTypeSyntax.WithAnnotationsGreen(annotations);
                }
                return nullableTypeSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new NullableTypeSyntax(base.Kind, elementType, questionToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new NullableTypeSyntax(base.Kind, elementType, questionToken, GetDiagnostics(), annotations);
        }

        public NullableTypeSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            TypeSyntax node = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            elementType = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            questionToken = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(elementType);
            writer.WriteValue(questionToken);
        }

        static NullableTypeSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(NullableTypeSyntax), (ObjectReader r) => new NullableTypeSyntax(r));
        }
    }
}
