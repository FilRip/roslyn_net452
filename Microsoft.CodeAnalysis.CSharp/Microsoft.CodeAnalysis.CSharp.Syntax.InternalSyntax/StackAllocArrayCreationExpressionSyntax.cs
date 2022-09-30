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

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class StackAllocArrayCreationExpressionSyntax : ExpressionSyntax
    {
        internal readonly SyntaxToken stackAllocKeyword;

        internal readonly TypeSyntax type;

        internal readonly InitializerExpressionSyntax? initializer;

        public SyntaxToken StackAllocKeyword => stackAllocKeyword;

        public TypeSyntax Type => type;

        public InitializerExpressionSyntax? Initializer => initializer;

        public StackAllocArrayCreationExpressionSyntax(SyntaxKind kind, SyntaxToken stackAllocKeyword, TypeSyntax type, InitializerExpressionSyntax? initializer, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(stackAllocKeyword);
            this.stackAllocKeyword = stackAllocKeyword;
            AdjustFlagsAndWidth(type);
            this.type = type;
            if (initializer != null)
            {
                AdjustFlagsAndWidth(initializer);
                this.initializer = initializer;
            }
        }

        public StackAllocArrayCreationExpressionSyntax(SyntaxKind kind, SyntaxToken stackAllocKeyword, TypeSyntax type, InitializerExpressionSyntax? initializer, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(stackAllocKeyword);
            this.stackAllocKeyword = stackAllocKeyword;
            AdjustFlagsAndWidth(type);
            this.type = type;
            if (initializer != null)
            {
                AdjustFlagsAndWidth(initializer);
                this.initializer = initializer;
            }
        }

        public StackAllocArrayCreationExpressionSyntax(SyntaxKind kind, SyntaxToken stackAllocKeyword, TypeSyntax type, InitializerExpressionSyntax? initializer)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(stackAllocKeyword);
            this.stackAllocKeyword = stackAllocKeyword;
            AdjustFlagsAndWidth(type);
            this.type = type;
            if (initializer != null)
            {
                AdjustFlagsAndWidth(initializer);
                this.initializer = initializer;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => stackAllocKeyword,
                1 => type,
                2 => initializer,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.StackAllocArrayCreationExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitStackAllocArrayCreationExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitStackAllocArrayCreationExpression(this);
        }

        public StackAllocArrayCreationExpressionSyntax Update(SyntaxToken stackAllocKeyword, TypeSyntax type, InitializerExpressionSyntax initializer)
        {
            if (stackAllocKeyword != StackAllocKeyword || type != Type || initializer != Initializer)
            {
                StackAllocArrayCreationExpressionSyntax stackAllocArrayCreationExpressionSyntax = SyntaxFactory.StackAllocArrayCreationExpression(stackAllocKeyword, type, initializer);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    stackAllocArrayCreationExpressionSyntax = stackAllocArrayCreationExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    stackAllocArrayCreationExpressionSyntax = stackAllocArrayCreationExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return stackAllocArrayCreationExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new StackAllocArrayCreationExpressionSyntax(base.Kind, stackAllocKeyword, type, initializer, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new StackAllocArrayCreationExpressionSyntax(base.Kind, stackAllocKeyword, type, initializer, GetDiagnostics(), annotations);
        }

        public StackAllocArrayCreationExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            stackAllocKeyword = node;
            TypeSyntax node2 = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            type = node2;
            InitializerExpressionSyntax initializerExpressionSyntax = (InitializerExpressionSyntax)reader.ReadValue();
            if (initializerExpressionSyntax != null)
            {
                AdjustFlagsAndWidth(initializerExpressionSyntax);
                initializer = initializerExpressionSyntax;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(stackAllocKeyword);
            writer.WriteValue(type);
            writer.WriteValue(initializer);
        }

        static StackAllocArrayCreationExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(StackAllocArrayCreationExpressionSyntax), (ObjectReader r) => new StackAllocArrayCreationExpressionSyntax(r));
        }
    }
}
