using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class OperatorDeclarationSyntax : BaseMethodDeclarationSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly GreenNode? modifiers;

        internal readonly TypeSyntax returnType;

        internal readonly SyntaxToken operatorKeyword;

        internal readonly SyntaxToken operatorToken;

        internal readonly ParameterListSyntax parameterList;

        internal readonly BlockSyntax? body;

        internal readonly ArrowExpressionClauseSyntax? expressionBody;

        internal readonly SyntaxToken? semicolonToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(modifiers);

        public TypeSyntax ReturnType => returnType;

        public SyntaxToken OperatorKeyword => operatorKeyword;

        public SyntaxToken OperatorToken => operatorToken;

        public override ParameterListSyntax ParameterList => parameterList;

        public override BlockSyntax? Body => body;

        public override ArrowExpressionClauseSyntax? ExpressionBody => expressionBody;

        public override SyntaxToken? SemicolonToken => semicolonToken;

        public OperatorDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, TypeSyntax returnType, SyntaxToken operatorKeyword, SyntaxToken operatorToken, ParameterListSyntax parameterList, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken? semicolonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 9;
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
            AdjustFlagsAndWidth(returnType);
            this.returnType = returnType;
            AdjustFlagsAndWidth(operatorKeyword);
            this.operatorKeyword = operatorKeyword;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(parameterList);
            this.parameterList = parameterList;
            if (body != null)
            {
                AdjustFlagsAndWidth(body);
                this.body = body;
            }
            if (expressionBody != null)
            {
                AdjustFlagsAndWidth(expressionBody);
                this.expressionBody = expressionBody;
            }
            if (semicolonToken != null)
            {
                AdjustFlagsAndWidth(semicolonToken);
                this.semicolonToken = semicolonToken;
            }
        }

        public OperatorDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, TypeSyntax returnType, SyntaxToken operatorKeyword, SyntaxToken operatorToken, ParameterListSyntax parameterList, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken? semicolonToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 9;
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
            AdjustFlagsAndWidth(returnType);
            this.returnType = returnType;
            AdjustFlagsAndWidth(operatorKeyword);
            this.operatorKeyword = operatorKeyword;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(parameterList);
            this.parameterList = parameterList;
            if (body != null)
            {
                AdjustFlagsAndWidth(body);
                this.body = body;
            }
            if (expressionBody != null)
            {
                AdjustFlagsAndWidth(expressionBody);
                this.expressionBody = expressionBody;
            }
            if (semicolonToken != null)
            {
                AdjustFlagsAndWidth(semicolonToken);
                this.semicolonToken = semicolonToken;
            }
        }

        public OperatorDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, TypeSyntax returnType, SyntaxToken operatorKeyword, SyntaxToken operatorToken, ParameterListSyntax parameterList, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken? semicolonToken)
            : base(kind)
        {
            base.SlotCount = 9;
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
            AdjustFlagsAndWidth(returnType);
            this.returnType = returnType;
            AdjustFlagsAndWidth(operatorKeyword);
            this.operatorKeyword = operatorKeyword;
            AdjustFlagsAndWidth(operatorToken);
            this.operatorToken = operatorToken;
            AdjustFlagsAndWidth(parameterList);
            this.parameterList = parameterList;
            if (body != null)
            {
                AdjustFlagsAndWidth(body);
                this.body = body;
            }
            if (expressionBody != null)
            {
                AdjustFlagsAndWidth(expressionBody);
                this.expressionBody = expressionBody;
            }
            if (semicolonToken != null)
            {
                AdjustFlagsAndWidth(semicolonToken);
                this.semicolonToken = semicolonToken;
            }
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => modifiers,
                2 => returnType,
                3 => operatorKeyword,
                4 => operatorToken,
                5 => parameterList,
                6 => body,
                7 => expressionBody,
                8 => semicolonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.OperatorDeclarationSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitOperatorDeclaration(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitOperatorDeclaration(this);
        }

        public OperatorDeclarationSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> modifiers, TypeSyntax returnType, SyntaxToken operatorKeyword, SyntaxToken operatorToken, ParameterListSyntax parameterList, BlockSyntax body, ArrowExpressionClauseSyntax expressionBody, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || returnType != ReturnType || operatorKeyword != OperatorKeyword || operatorToken != OperatorToken || parameterList != ParameterList || body != Body || expressionBody != ExpressionBody || semicolonToken != SemicolonToken)
            {
                OperatorDeclarationSyntax operatorDeclarationSyntax = SyntaxFactory.OperatorDeclaration(attributeLists, modifiers, returnType, operatorKeyword, operatorToken, parameterList, body, expressionBody, semicolonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    operatorDeclarationSyntax = operatorDeclarationSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    operatorDeclarationSyntax = operatorDeclarationSyntax.WithAnnotationsGreen(annotations);
                }
                return operatorDeclarationSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new OperatorDeclarationSyntax(base.Kind, attributeLists, modifiers, returnType, operatorKeyword, operatorToken, parameterList, body, expressionBody, semicolonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new OperatorDeclarationSyntax(base.Kind, attributeLists, modifiers, returnType, operatorKeyword, operatorToken, parameterList, body, expressionBody, semicolonToken, GetDiagnostics(), annotations);
        }

        public OperatorDeclarationSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 9;
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
            TypeSyntax node = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            returnType = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            operatorKeyword = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            operatorToken = node3;
            ParameterListSyntax node4 = (ParameterListSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            parameterList = node4;
            BlockSyntax blockSyntax = (BlockSyntax)reader.ReadValue();
            if (blockSyntax != null)
            {
                AdjustFlagsAndWidth(blockSyntax);
                body = blockSyntax;
            }
            ArrowExpressionClauseSyntax arrowExpressionClauseSyntax = (ArrowExpressionClauseSyntax)reader.ReadValue();
            if (arrowExpressionClauseSyntax != null)
            {
                AdjustFlagsAndWidth(arrowExpressionClauseSyntax);
                expressionBody = arrowExpressionClauseSyntax;
            }
            SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
            if (syntaxToken != null)
            {
                AdjustFlagsAndWidth(syntaxToken);
                semicolonToken = syntaxToken;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(attributeLists);
            writer.WriteValue(modifiers);
            writer.WriteValue(returnType);
            writer.WriteValue(operatorKeyword);
            writer.WriteValue(operatorToken);
            writer.WriteValue(parameterList);
            writer.WriteValue(body);
            writer.WriteValue(expressionBody);
            writer.WriteValue(semicolonToken);
        }

        static OperatorDeclarationSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(OperatorDeclarationSyntax), (ObjectReader r) => new OperatorDeclarationSyntax(r));
        }
    }
}
