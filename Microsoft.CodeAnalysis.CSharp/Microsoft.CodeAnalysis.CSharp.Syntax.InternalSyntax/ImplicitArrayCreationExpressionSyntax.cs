using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ImplicitArrayCreationExpressionSyntax : ExpressionSyntax
    {
        internal readonly SyntaxToken newKeyword;

        internal readonly SyntaxToken openBracketToken;

        internal readonly GreenNode? commas;

        internal readonly SyntaxToken closeBracketToken;

        internal readonly InitializerExpressionSyntax initializer;

        public SyntaxToken NewKeyword => newKeyword;

        public SyntaxToken OpenBracketToken => openBracketToken;

        public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Commas => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(commas);

        public SyntaxToken CloseBracketToken => closeBracketToken;

        public InitializerExpressionSyntax Initializer => initializer;

        public ImplicitArrayCreationExpressionSyntax(SyntaxKind kind, SyntaxToken newKeyword, SyntaxToken openBracketToken, GreenNode? commas, SyntaxToken closeBracketToken, InitializerExpressionSyntax initializer, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 5;
            AdjustFlagsAndWidth(newKeyword);
            this.newKeyword = newKeyword;
            AdjustFlagsAndWidth(openBracketToken);
            this.openBracketToken = openBracketToken;
            if (commas != null)
            {
                AdjustFlagsAndWidth(commas);
                this.commas = commas;
            }
            AdjustFlagsAndWidth(closeBracketToken);
            this.closeBracketToken = closeBracketToken;
            AdjustFlagsAndWidth(initializer);
            this.initializer = initializer;
        }

        public ImplicitArrayCreationExpressionSyntax(SyntaxKind kind, SyntaxToken newKeyword, SyntaxToken openBracketToken, GreenNode? commas, SyntaxToken closeBracketToken, InitializerExpressionSyntax initializer, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 5;
            AdjustFlagsAndWidth(newKeyword);
            this.newKeyword = newKeyword;
            AdjustFlagsAndWidth(openBracketToken);
            this.openBracketToken = openBracketToken;
            if (commas != null)
            {
                AdjustFlagsAndWidth(commas);
                this.commas = commas;
            }
            AdjustFlagsAndWidth(closeBracketToken);
            this.closeBracketToken = closeBracketToken;
            AdjustFlagsAndWidth(initializer);
            this.initializer = initializer;
        }

        public ImplicitArrayCreationExpressionSyntax(SyntaxKind kind, SyntaxToken newKeyword, SyntaxToken openBracketToken, GreenNode? commas, SyntaxToken closeBracketToken, InitializerExpressionSyntax initializer)
            : base(kind)
        {
            base.SlotCount = 5;
            AdjustFlagsAndWidth(newKeyword);
            this.newKeyword = newKeyword;
            AdjustFlagsAndWidth(openBracketToken);
            this.openBracketToken = openBracketToken;
            if (commas != null)
            {
                AdjustFlagsAndWidth(commas);
                this.commas = commas;
            }
            AdjustFlagsAndWidth(closeBracketToken);
            this.closeBracketToken = closeBracketToken;
            AdjustFlagsAndWidth(initializer);
            this.initializer = initializer;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => newKeyword,
                1 => openBracketToken,
                2 => commas,
                3 => closeBracketToken,
                4 => initializer,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ImplicitArrayCreationExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitImplicitArrayCreationExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitImplicitArrayCreationExpression(this);
        }

        public ImplicitArrayCreationExpressionSyntax Update(SyntaxToken newKeyword, SyntaxToken openBracketToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> commas, SyntaxToken closeBracketToken, InitializerExpressionSyntax initializer)
        {
            if (newKeyword != NewKeyword || openBracketToken != OpenBracketToken || commas != Commas || closeBracketToken != CloseBracketToken || initializer != Initializer)
            {
                ImplicitArrayCreationExpressionSyntax implicitArrayCreationExpressionSyntax = SyntaxFactory.ImplicitArrayCreationExpression(newKeyword, openBracketToken, commas, closeBracketToken, initializer);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    implicitArrayCreationExpressionSyntax = implicitArrayCreationExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    implicitArrayCreationExpressionSyntax = implicitArrayCreationExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return implicitArrayCreationExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ImplicitArrayCreationExpressionSyntax(base.Kind, newKeyword, openBracketToken, commas, closeBracketToken, initializer, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ImplicitArrayCreationExpressionSyntax(base.Kind, newKeyword, openBracketToken, commas, closeBracketToken, initializer, GetDiagnostics(), annotations);
        }

        public ImplicitArrayCreationExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 5;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            newKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            openBracketToken = node2;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                commas = greenNode;
            }
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
            writer.WriteValue(newKeyword);
            writer.WriteValue(openBracketToken);
            writer.WriteValue(commas);
            writer.WriteValue(closeBracketToken);
            writer.WriteValue(initializer);
        }

        static ImplicitArrayCreationExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ImplicitArrayCreationExpressionSyntax), (ObjectReader r) => new ImplicitArrayCreationExpressionSyntax(r));
        }
    }
}
