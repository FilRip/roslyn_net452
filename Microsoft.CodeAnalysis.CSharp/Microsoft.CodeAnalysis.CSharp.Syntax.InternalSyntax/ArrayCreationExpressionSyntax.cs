using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ArrayCreationExpressionSyntax : ExpressionSyntax
    {
        internal readonly SyntaxToken newKeyword;

        internal readonly ArrayTypeSyntax type;

        internal readonly InitializerExpressionSyntax? initializer;

        public SyntaxToken NewKeyword => newKeyword;

        public ArrayTypeSyntax Type => type;

        public InitializerExpressionSyntax? Initializer => initializer;

        public ArrayCreationExpressionSyntax(SyntaxKind kind, SyntaxToken newKeyword, ArrayTypeSyntax type, InitializerExpressionSyntax? initializer, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(newKeyword);
            this.newKeyword = newKeyword;
            AdjustFlagsAndWidth(type);
            this.type = type;
            if (initializer != null)
            {
                AdjustFlagsAndWidth(initializer);
                this.initializer = initializer;
            }
        }

        public ArrayCreationExpressionSyntax(SyntaxKind kind, SyntaxToken newKeyword, ArrayTypeSyntax type, InitializerExpressionSyntax? initializer, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(newKeyword);
            this.newKeyword = newKeyword;
            AdjustFlagsAndWidth(type);
            this.type = type;
            if (initializer != null)
            {
                AdjustFlagsAndWidth(initializer);
                this.initializer = initializer;
            }
        }

        public ArrayCreationExpressionSyntax(SyntaxKind kind, SyntaxToken newKeyword, ArrayTypeSyntax type, InitializerExpressionSyntax? initializer)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(newKeyword);
            this.newKeyword = newKeyword;
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
                0 => newKeyword,
                1 => type,
                2 => initializer,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ArrayCreationExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitArrayCreationExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitArrayCreationExpression(this);
        }

        public ArrayCreationExpressionSyntax Update(SyntaxToken newKeyword, ArrayTypeSyntax type, InitializerExpressionSyntax initializer)
        {
            if (newKeyword != NewKeyword || type != Type || initializer != Initializer)
            {
                ArrayCreationExpressionSyntax arrayCreationExpressionSyntax = SyntaxFactory.ArrayCreationExpression(newKeyword, type, initializer);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    arrayCreationExpressionSyntax = arrayCreationExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    arrayCreationExpressionSyntax = arrayCreationExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return arrayCreationExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ArrayCreationExpressionSyntax(base.Kind, newKeyword, type, initializer, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ArrayCreationExpressionSyntax(base.Kind, newKeyword, type, initializer, GetDiagnostics(), annotations);
        }

        public ArrayCreationExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            newKeyword = node;
            ArrayTypeSyntax node2 = (ArrayTypeSyntax)reader.ReadValue();
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
            writer.WriteValue(newKeyword);
            writer.WriteValue(type);
            writer.WriteValue(initializer);
        }

        static ArrayCreationExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ArrayCreationExpressionSyntax), (ObjectReader r) => new ArrayCreationExpressionSyntax(r));
        }
    }
}
