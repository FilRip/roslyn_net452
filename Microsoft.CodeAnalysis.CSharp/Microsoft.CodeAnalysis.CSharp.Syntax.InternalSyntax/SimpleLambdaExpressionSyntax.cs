using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class SimpleLambdaExpressionSyntax : LambdaExpressionSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly GreenNode? modifiers;

        internal readonly ParameterSyntax parameter;

        internal readonly SyntaxToken arrowToken;

        internal readonly BlockSyntax? block;

        internal readonly ExpressionSyntax? expressionBody;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(modifiers);

        public ParameterSyntax Parameter => parameter;

        public override SyntaxToken ArrowToken => arrowToken;

        public override BlockSyntax? Block => block;

        public override ExpressionSyntax? ExpressionBody => expressionBody;

        public SimpleLambdaExpressionSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, ParameterSyntax parameter, SyntaxToken arrowToken, BlockSyntax? block, ExpressionSyntax? expressionBody, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 6;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (modifiers != null)
            {
                AdjustFlagsAndWidth(modifiers);
                this.modifiers = modifiers;
            }
            AdjustFlagsAndWidth(parameter);
            this.parameter = parameter;
            AdjustFlagsAndWidth(arrowToken);
            this.arrowToken = arrowToken;
            if (block != null)
            {
                AdjustFlagsAndWidth(block);
                this.block = block;
            }
            if (expressionBody != null)
            {
                AdjustFlagsAndWidth(expressionBody);
                this.expressionBody = expressionBody;
            }
        }

        public SimpleLambdaExpressionSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, ParameterSyntax parameter, SyntaxToken arrowToken, BlockSyntax? block, ExpressionSyntax? expressionBody, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 6;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (modifiers != null)
            {
                AdjustFlagsAndWidth(modifiers);
                this.modifiers = modifiers;
            }
            AdjustFlagsAndWidth(parameter);
            this.parameter = parameter;
            AdjustFlagsAndWidth(arrowToken);
            this.arrowToken = arrowToken;
            if (block != null)
            {
                AdjustFlagsAndWidth(block);
                this.block = block;
            }
            if (expressionBody != null)
            {
                AdjustFlagsAndWidth(expressionBody);
                this.expressionBody = expressionBody;
            }
        }

        public SimpleLambdaExpressionSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, ParameterSyntax parameter, SyntaxToken arrowToken, BlockSyntax? block, ExpressionSyntax? expressionBody)
            : base(kind)
        {
            base.SlotCount = 6;
            if (attributeLists != null)
            {
                AdjustFlagsAndWidth(attributeLists);
                this.attributeLists = attributeLists;
            }
            if (modifiers != null)
            {
                AdjustFlagsAndWidth(modifiers);
                this.modifiers = modifiers;
            }
            AdjustFlagsAndWidth(parameter);
            this.parameter = parameter;
            AdjustFlagsAndWidth(arrowToken);
            this.arrowToken = arrowToken;
            if (block != null)
            {
                AdjustFlagsAndWidth(block);
                this.block = block;
            }
            if (expressionBody != null)
            {
                AdjustFlagsAndWidth(expressionBody);
                this.expressionBody = expressionBody;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => modifiers,
                2 => parameter,
                3 => arrowToken,
                4 => block,
                5 => expressionBody,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.SimpleLambdaExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitSimpleLambdaExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitSimpleLambdaExpression(this);
        }

        public SimpleLambdaExpressionSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> modifiers, ParameterSyntax parameter, SyntaxToken arrowToken, BlockSyntax block, ExpressionSyntax expressionBody)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || parameter != Parameter || arrowToken != ArrowToken || block != Block || expressionBody != ExpressionBody)
            {
                SimpleLambdaExpressionSyntax simpleLambdaExpressionSyntax = SyntaxFactory.SimpleLambdaExpression(attributeLists, modifiers, parameter, arrowToken, block, expressionBody);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    simpleLambdaExpressionSyntax = simpleLambdaExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    simpleLambdaExpressionSyntax = simpleLambdaExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return simpleLambdaExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new SimpleLambdaExpressionSyntax(base.Kind, attributeLists, modifiers, parameter, arrowToken, block, expressionBody, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new SimpleLambdaExpressionSyntax(base.Kind, attributeLists, modifiers, parameter, arrowToken, block, expressionBody, GetDiagnostics(), annotations);
        }

        public SimpleLambdaExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 6;
            GreenNode greenNode = (GreenNode)reader.ReadValue();
            if (greenNode != null)
            {
                AdjustFlagsAndWidth(greenNode);
                attributeLists = greenNode;
            }
            GreenNode greenNode2 = (GreenNode)reader.ReadValue();
            if (greenNode2 != null)
            {
                AdjustFlagsAndWidth(greenNode2);
                modifiers = greenNode2;
            }
            ParameterSyntax node = (ParameterSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            parameter = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            arrowToken = node2;
            BlockSyntax blockSyntax = (BlockSyntax)reader.ReadValue();
            if (blockSyntax != null)
            {
                AdjustFlagsAndWidth(blockSyntax);
                block = blockSyntax;
            }
            ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
            if (expressionSyntax != null)
            {
                AdjustFlagsAndWidth(expressionSyntax);
                expressionBody = expressionSyntax;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(modifiers);
            writer.WriteValue(parameter);
            writer.WriteValue(arrowToken);
            writer.WriteValue(block);
            writer.WriteValue(expressionBody);
        }

        static SimpleLambdaExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(SimpleLambdaExpressionSyntax), (ObjectReader r) => new SimpleLambdaExpressionSyntax(r));
        }
    }
}
