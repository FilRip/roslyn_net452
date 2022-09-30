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

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ObjectCreationExpressionSyntax : BaseObjectCreationExpressionSyntax
    {
        internal readonly SyntaxToken newKeyword;

        internal readonly TypeSyntax type;

        internal readonly ArgumentListSyntax? argumentList;

        internal readonly InitializerExpressionSyntax? initializer;

        public override SyntaxToken NewKeyword => newKeyword;

        public TypeSyntax Type => type;

        public override ArgumentListSyntax? ArgumentList => argumentList;

        public override InitializerExpressionSyntax? Initializer => initializer;

        public ObjectCreationExpressionSyntax(SyntaxKind kind, SyntaxToken newKeyword, TypeSyntax type, ArgumentListSyntax? argumentList, InitializerExpressionSyntax? initializer, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(newKeyword);
            this.newKeyword = newKeyword;
            AdjustFlagsAndWidth(type);
            this.type = type;
            if (argumentList != null)
            {
                AdjustFlagsAndWidth(argumentList);
                this.argumentList = argumentList;
            }
            if (initializer != null)
            {
                AdjustFlagsAndWidth(initializer);
                this.initializer = initializer;
            }
        }

        public ObjectCreationExpressionSyntax(SyntaxKind kind, SyntaxToken newKeyword, TypeSyntax type, ArgumentListSyntax? argumentList, InitializerExpressionSyntax? initializer, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
            AdjustFlagsAndWidth(newKeyword);
            this.newKeyword = newKeyword;
            AdjustFlagsAndWidth(type);
            this.type = type;
            if (argumentList != null)
            {
                AdjustFlagsAndWidth(argumentList);
                this.argumentList = argumentList;
            }
            if (initializer != null)
            {
                AdjustFlagsAndWidth(initializer);
                this.initializer = initializer;
            }
        }

        public ObjectCreationExpressionSyntax(SyntaxKind kind, SyntaxToken newKeyword, TypeSyntax type, ArgumentListSyntax? argumentList, InitializerExpressionSyntax? initializer)
            : base(kind)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(newKeyword);
            this.newKeyword = newKeyword;
            AdjustFlagsAndWidth(type);
            this.type = type;
            if (argumentList != null)
            {
                AdjustFlagsAndWidth(argumentList);
                this.argumentList = argumentList;
            }
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
                2 => argumentList,
                3 => initializer,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ObjectCreationExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitObjectCreationExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitObjectCreationExpression(this);
        }

        public ObjectCreationExpressionSyntax Update(SyntaxToken newKeyword, TypeSyntax type, ArgumentListSyntax argumentList, InitializerExpressionSyntax initializer)
        {
            if (newKeyword != NewKeyword || type != Type || argumentList != ArgumentList || initializer != Initializer)
            {
                ObjectCreationExpressionSyntax objectCreationExpressionSyntax = SyntaxFactory.ObjectCreationExpression(newKeyword, type, argumentList, initializer);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    objectCreationExpressionSyntax = objectCreationExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    objectCreationExpressionSyntax = objectCreationExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return objectCreationExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ObjectCreationExpressionSyntax(base.Kind, newKeyword, type, argumentList, initializer, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ObjectCreationExpressionSyntax(base.Kind, newKeyword, type, argumentList, initializer, GetDiagnostics(), annotations);
        }

        public ObjectCreationExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 4;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            newKeyword = node;
            TypeSyntax node2 = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            type = node2;
            ArgumentListSyntax argumentListSyntax = (ArgumentListSyntax)reader.ReadValue();
            if (argumentListSyntax != null)
            {
                AdjustFlagsAndWidth(argumentListSyntax);
                argumentList = argumentListSyntax;
            }
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
            writer.WriteValue(argumentList);
            writer.WriteValue(initializer);
        }

        static ObjectCreationExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ObjectCreationExpressionSyntax), (ObjectReader r) => new ObjectCreationExpressionSyntax(r));
        }
    }
}
