using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ImplicitStackAllocArrayCreationExpressionSyntax : ExpressionSyntax
    {
        internal readonly SyntaxToken stackAllocKeyword;

        internal readonly SyntaxToken openBracketToken;

        internal readonly SyntaxToken closeBracketToken;

        internal readonly InitializerExpressionSyntax initializer;

        public SyntaxToken StackAllocKeyword => stackAllocKeyword;

        public SyntaxToken OpenBracketToken => openBracketToken;

        public SyntaxToken CloseBracketToken => closeBracketToken;

        public InitializerExpressionSyntax Initializer => initializer;

        public ImplicitStackAllocArrayCreationExpressionSyntax(SyntaxKind kind, SyntaxToken stackAllocKeyword, SyntaxToken openBracketToken, SyntaxToken closeBracketToken, InitializerExpressionSyntax initializer, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(stackAllocKeyword);
            this.stackAllocKeyword = stackAllocKeyword;
            AdjustFlagsAndWidth(openBracketToken);
            this.openBracketToken = openBracketToken;
            AdjustFlagsAndWidth(closeBracketToken);
            this.closeBracketToken = closeBracketToken;
            AdjustFlagsAndWidth(initializer);
            this.initializer = initializer;
        }

        public ImplicitStackAllocArrayCreationExpressionSyntax(SyntaxKind kind, SyntaxToken stackAllocKeyword, SyntaxToken openBracketToken, SyntaxToken closeBracketToken, InitializerExpressionSyntax initializer, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
            AdjustFlagsAndWidth(stackAllocKeyword);
            this.stackAllocKeyword = stackAllocKeyword;
            AdjustFlagsAndWidth(openBracketToken);
            this.openBracketToken = openBracketToken;
            AdjustFlagsAndWidth(closeBracketToken);
            this.closeBracketToken = closeBracketToken;
            AdjustFlagsAndWidth(initializer);
            this.initializer = initializer;
        }

        public ImplicitStackAllocArrayCreationExpressionSyntax(SyntaxKind kind, SyntaxToken stackAllocKeyword, SyntaxToken openBracketToken, SyntaxToken closeBracketToken, InitializerExpressionSyntax initializer)
            : base(kind)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(stackAllocKeyword);
            this.stackAllocKeyword = stackAllocKeyword;
            AdjustFlagsAndWidth(openBracketToken);
            this.openBracketToken = openBracketToken;
            AdjustFlagsAndWidth(closeBracketToken);
            this.closeBracketToken = closeBracketToken;
            AdjustFlagsAndWidth(initializer);
            this.initializer = initializer;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => stackAllocKeyword,
                1 => openBracketToken,
                2 => closeBracketToken,
                3 => initializer,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitStackAllocArrayCreationExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitImplicitStackAllocArrayCreationExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitImplicitStackAllocArrayCreationExpression(this);
        }

        public ImplicitStackAllocArrayCreationExpressionSyntax Update(SyntaxToken stackAllocKeyword, SyntaxToken openBracketToken, SyntaxToken closeBracketToken, InitializerExpressionSyntax initializer)
        {
            if (stackAllocKeyword != StackAllocKeyword || openBracketToken != OpenBracketToken || closeBracketToken != CloseBracketToken || initializer != Initializer)
            {
                ImplicitStackAllocArrayCreationExpressionSyntax implicitStackAllocArrayCreationExpressionSyntax = SyntaxFactory.ImplicitStackAllocArrayCreationExpression(stackAllocKeyword, openBracketToken, closeBracketToken, initializer);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    implicitStackAllocArrayCreationExpressionSyntax = implicitStackAllocArrayCreationExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    implicitStackAllocArrayCreationExpressionSyntax = implicitStackAllocArrayCreationExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return implicitStackAllocArrayCreationExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ImplicitStackAllocArrayCreationExpressionSyntax(base.Kind, stackAllocKeyword, openBracketToken, closeBracketToken, initializer, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ImplicitStackAllocArrayCreationExpressionSyntax(base.Kind, stackAllocKeyword, openBracketToken, closeBracketToken, initializer, GetDiagnostics(), annotations);
        }

        public ImplicitStackAllocArrayCreationExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 4;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            stackAllocKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            openBracketToken = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            closeBracketToken = node3;
            InitializerExpressionSyntax node4 = (InitializerExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            initializer = node4;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(stackAllocKeyword);
            writer.WriteValue(openBracketToken);
            writer.WriteValue(closeBracketToken);
            writer.WriteValue(initializer);
        }

        static ImplicitStackAllocArrayCreationExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ImplicitStackAllocArrayCreationExpressionSyntax), (ObjectReader r) => new ImplicitStackAllocArrayCreationExpressionSyntax(r));
        }
    }
}
