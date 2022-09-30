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
    public sealed class CatchDeclarationSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken openParenToken;

        internal readonly TypeSyntax type;

        internal readonly SyntaxToken? identifier;

        internal readonly SyntaxToken closeParenToken;

        public SyntaxToken OpenParenToken => openParenToken;

        public TypeSyntax Type => type;

        public SyntaxToken? Identifier => identifier;

        public SyntaxToken CloseParenToken => closeParenToken;

        public CatchDeclarationSyntax(SyntaxKind kind, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken? identifier, SyntaxToken closeParenToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(type);
            this.type = type;
            if (identifier != null)
            {
                AdjustFlagsAndWidth(identifier);
                this.identifier = identifier;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public CatchDeclarationSyntax(SyntaxKind kind, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken? identifier, SyntaxToken closeParenToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(type);
            this.type = type;
            if (identifier != null)
            {
                AdjustFlagsAndWidth(identifier);
                this.identifier = identifier;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public CatchDeclarationSyntax(SyntaxKind kind, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken? identifier, SyntaxToken closeParenToken)
            : base(kind)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(type);
            this.type = type;
            if (identifier != null)
            {
                AdjustFlagsAndWidth(identifier);
                this.identifier = identifier;
            }
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => openParenToken,
                1 => type,
                2 => identifier,
                3 => closeParenToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.CatchDeclarationSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitCatchDeclaration(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitCatchDeclaration(this);
        }

        public CatchDeclarationSyntax Update(SyntaxToken openParenToken, TypeSyntax type, SyntaxToken identifier, SyntaxToken closeParenToken)
        {
            if (openParenToken != OpenParenToken || type != Type || identifier != Identifier || closeParenToken != CloseParenToken)
            {
                CatchDeclarationSyntax catchDeclarationSyntax = SyntaxFactory.CatchDeclaration(openParenToken, type, identifier, closeParenToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    catchDeclarationSyntax = catchDeclarationSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    catchDeclarationSyntax = catchDeclarationSyntax.WithAnnotationsGreen(annotations);
                }
                return catchDeclarationSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new CatchDeclarationSyntax(base.Kind, openParenToken, type, identifier, closeParenToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new CatchDeclarationSyntax(base.Kind, openParenToken, type, identifier, closeParenToken, GetDiagnostics(), annotations);
        }

        public CatchDeclarationSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 4;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            openParenToken = node;
            TypeSyntax node2 = (TypeSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            type = node2;
            SyntaxToken syntaxToken = (SyntaxToken)reader.ReadValue();
            if (syntaxToken != null)
            {
                AdjustFlagsAndWidth(syntaxToken);
                identifier = syntaxToken;
            }
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            closeParenToken = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(openParenToken);
            writer.WriteValue(type);
            writer.WriteValue(identifier);
            writer.WriteValue(closeParenToken);
        }

        static CatchDeclarationSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(CatchDeclarationSyntax), (ObjectReader r) => new CatchDeclarationSyntax(r));
        }
    }
}
