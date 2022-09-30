using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ConversionOperatorDeclarationSyntax : BaseMethodDeclarationSyntax
    {
        internal readonly GreenNode? attributeLists;

        internal readonly GreenNode? modifiers;

        internal readonly SyntaxToken implicitOrExplicitKeyword;

        internal readonly SyntaxToken operatorKeyword;

        internal readonly TypeSyntax type;

        internal readonly ParameterListSyntax parameterList;

        internal readonly BlockSyntax? body;

        internal readonly ArrowExpressionClauseSyntax? expressionBody;

        internal readonly SyntaxToken? semicolonToken;

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> AttributeLists => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>(attributeLists);

        public override Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> Modifiers => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>(modifiers);

        public SyntaxToken ImplicitOrExplicitKeyword => implicitOrExplicitKeyword;

        public SyntaxToken OperatorKeyword => operatorKeyword;

        public TypeSyntax Type => type;

        public override ParameterListSyntax ParameterList => parameterList;

        public override BlockSyntax? Body => body;

        public override ArrowExpressionClauseSyntax? ExpressionBody => expressionBody;

        public override SyntaxToken? SemicolonToken => semicolonToken;

        public ConversionOperatorDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken implicitOrExplicitKeyword, SyntaxToken operatorKeyword, TypeSyntax type, ParameterListSyntax parameterList, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken? semicolonToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
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
            AdjustFlagsAndWidth(implicitOrExplicitKeyword);
            this.implicitOrExplicitKeyword = implicitOrExplicitKeyword;
            AdjustFlagsAndWidth(operatorKeyword);
            this.operatorKeyword = operatorKeyword;
            AdjustFlagsAndWidth(type);
            this.type = type;
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

        public ConversionOperatorDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken implicitOrExplicitKeyword, SyntaxToken operatorKeyword, TypeSyntax type, ParameterListSyntax parameterList, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken? semicolonToken, SyntaxFactoryContext context)
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
            AdjustFlagsAndWidth(implicitOrExplicitKeyword);
            this.implicitOrExplicitKeyword = implicitOrExplicitKeyword;
            AdjustFlagsAndWidth(operatorKeyword);
            this.operatorKeyword = operatorKeyword;
            AdjustFlagsAndWidth(type);
            this.type = type;
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

        public ConversionOperatorDeclarationSyntax(SyntaxKind kind, GreenNode? attributeLists, GreenNode? modifiers, SyntaxToken implicitOrExplicitKeyword, SyntaxToken operatorKeyword, TypeSyntax type, ParameterListSyntax parameterList, BlockSyntax? body, ArrowExpressionClauseSyntax? expressionBody, SyntaxToken? semicolonToken)
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
            AdjustFlagsAndWidth(implicitOrExplicitKeyword);
            this.implicitOrExplicitKeyword = implicitOrExplicitKeyword;
            AdjustFlagsAndWidth(operatorKeyword);
            this.operatorKeyword = operatorKeyword;
            AdjustFlagsAndWidth(type);
            this.type = type;
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
                2 => implicitOrExplicitKeyword,
                3 => operatorKeyword,
                4 => type,
                5 => parameterList,
                6 => body,
                7 => expressionBody,
                8 => semicolonToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ConversionOperatorDeclarationSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitConversionOperatorDeclaration(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitConversionOperatorDeclaration(this);
        }

        public ConversionOperatorDeclarationSyntax Update(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> modifiers, SyntaxToken implicitOrExplicitKeyword, SyntaxToken operatorKeyword, TypeSyntax type, ParameterListSyntax parameterList, BlockSyntax body, ArrowExpressionClauseSyntax expressionBody, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || implicitOrExplicitKeyword != ImplicitOrExplicitKeyword || operatorKeyword != OperatorKeyword || type != Type || parameterList != ParameterList || body != Body || expressionBody != ExpressionBody || semicolonToken != SemicolonToken)
            {
                ConversionOperatorDeclarationSyntax conversionOperatorDeclarationSyntax = SyntaxFactory.ConversionOperatorDeclaration(attributeLists, modifiers, implicitOrExplicitKeyword, operatorKeyword, type, parameterList, body, expressionBody, semicolonToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    conversionOperatorDeclarationSyntax = conversionOperatorDeclarationSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    conversionOperatorDeclarationSyntax = conversionOperatorDeclarationSyntax.WithAnnotationsGreen(annotations);
                }
                return conversionOperatorDeclarationSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ConversionOperatorDeclarationSyntax(base.Kind, attributeLists, modifiers, implicitOrExplicitKeyword, operatorKeyword, type, parameterList, body, expressionBody, semicolonToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ConversionOperatorDeclarationSyntax(base.Kind, attributeLists, modifiers, implicitOrExplicitKeyword, operatorKeyword, type, parameterList, body, expressionBody, semicolonToken, GetDiagnostics(), annotations);
        }

        public ConversionOperatorDeclarationSyntax(ObjectReader reader)
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
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            implicitOrExplicitKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            operatorKeyword = node2;
            TypeSyntax node3 = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            type = node3;
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
            writer.WriteValue(implicitOrExplicitKeyword);
            writer.WriteValue(operatorKeyword);
            writer.WriteValue(type);
            writer.WriteValue(parameterList);
            writer.WriteValue(body);
            writer.WriteValue(expressionBody);
            writer.WriteValue(semicolonToken);
        }

        static ConversionOperatorDeclarationSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ConversionOperatorDeclarationSyntax), (ObjectReader r) => new ConversionOperatorDeclarationSyntax(r));
        }
    }
}
