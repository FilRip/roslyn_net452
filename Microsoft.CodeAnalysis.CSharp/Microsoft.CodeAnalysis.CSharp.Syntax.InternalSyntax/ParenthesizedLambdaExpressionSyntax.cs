using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ParenthesizedLambdaExpressionSyntax : LambdaExpressionSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly GreenNode? modifiers;

        internal readonly ParameterListSyntax parameterList;

        internal readonly SyntaxToken arrowToken;

        internal readonly BlockSyntax? block;

        internal readonly ExpressionSyntax? expressionBody;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(modifiers);

        public ParameterListSyntax ParameterList => parameterList;

        public override SyntaxToken ArrowToken => arrowToken;

        public override BlockSyntax? Block => block;

        public override ExpressionSyntax? ExpressionBody => expressionBody;

        public ParenthesizedLambdaExpressionSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, ParameterListSyntax parameterList, SyntaxToken arrowToken, BlockSyntax? block, ExpressionSyntax? expressionBody, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
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
            AdjustFlagsAndWidth(parameterList);
            this.parameterList = parameterList;
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

        public ParenthesizedLambdaExpressionSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, ParameterListSyntax parameterList, SyntaxToken arrowToken, BlockSyntax? block, ExpressionSyntax? expressionBody, SyntaxFactoryContext context)
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
            AdjustFlagsAndWidth(parameterList);
            this.parameterList = parameterList;
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

        public ParenthesizedLambdaExpressionSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, ParameterListSyntax parameterList, SyntaxToken arrowToken, BlockSyntax? block, ExpressionSyntax? expressionBody)
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
            AdjustFlagsAndWidth(parameterList);
            this.parameterList = parameterList;
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
                2 => parameterList,
                3 => arrowToken,
                4 => block,
                5 => expressionBody,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ParenthesizedLambdaExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitParenthesizedLambdaExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitParenthesizedLambdaExpression(this);
        }

        public ParenthesizedLambdaExpressionSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> modifiers, ParameterListSyntax parameterList, SyntaxToken arrowToken, BlockSyntax block, ExpressionSyntax expressionBody)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || parameterList != ParameterList || arrowToken != ArrowToken || block != Block || expressionBody != ExpressionBody)
            {
                ParenthesizedLambdaExpressionSyntax parenthesizedLambdaExpressionSyntax = SyntaxFactory.ParenthesizedLambdaExpression(attributeLists, modifiers, parameterList, arrowToken, block, expressionBody);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    parenthesizedLambdaExpressionSyntax = parenthesizedLambdaExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    parenthesizedLambdaExpressionSyntax = parenthesizedLambdaExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return parenthesizedLambdaExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ParenthesizedLambdaExpressionSyntax(base.Kind, attributeLists, modifiers, parameterList, arrowToken, block, expressionBody, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ParenthesizedLambdaExpressionSyntax(base.Kind, attributeLists, modifiers, parameterList, arrowToken, block, expressionBody, GetDiagnostics(), annotations);
        }

        public ParenthesizedLambdaExpressionSyntax(ObjectReader reader)
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
            ParameterListSyntax node = (ParameterListSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            parameterList = node;
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
            writer.WriteValue(parameterList);
            writer.WriteValue(arrowToken);
            writer.WriteValue(block);
            writer.WriteValue(expressionBody);
        }

        static ParenthesizedLambdaExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ParenthesizedLambdaExpressionSyntax), (ObjectReader r) => new ParenthesizedLambdaExpressionSyntax(r));
        }
    }
}
