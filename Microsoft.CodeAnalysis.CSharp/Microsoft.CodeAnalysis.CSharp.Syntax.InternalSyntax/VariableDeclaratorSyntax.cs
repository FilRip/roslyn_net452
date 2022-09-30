using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class VariableDeclaratorSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken identifier;

        internal readonly BracketedArgumentListSyntax? argumentList;

        internal readonly EqualsValueClauseSyntax? initializer;

        public SyntaxToken Identifier => identifier;

        public BracketedArgumentListSyntax? ArgumentList => argumentList;

        public EqualsValueClauseSyntax? Initializer => initializer;

        public VariableDeclaratorSyntax(SyntaxKind kind, SyntaxToken identifier, BracketedArgumentListSyntax? argumentList, EqualsValueClauseSyntax? initializer, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
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

        public VariableDeclaratorSyntax(SyntaxKind kind, SyntaxToken identifier, BracketedArgumentListSyntax? argumentList, EqualsValueClauseSyntax? initializer, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
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

        public VariableDeclaratorSyntax(SyntaxKind kind, SyntaxToken identifier, BracketedArgumentListSyntax? argumentList, EqualsValueClauseSyntax? initializer)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
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
                0 => identifier,
                1 => argumentList,
                2 => initializer,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitVariableDeclarator(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitVariableDeclarator(this);
        }

        public VariableDeclaratorSyntax Update(SyntaxToken identifier, BracketedArgumentListSyntax argumentList, EqualsValueClauseSyntax initializer)
        {
            if (identifier != Identifier || argumentList != ArgumentList || initializer != Initializer)
            {
                VariableDeclaratorSyntax variableDeclaratorSyntax = SyntaxFactory.VariableDeclarator(identifier, argumentList, initializer);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    variableDeclaratorSyntax = variableDeclaratorSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    variableDeclaratorSyntax = variableDeclaratorSyntax.WithAnnotationsGreen(annotations);
                }
                return variableDeclaratorSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new VariableDeclaratorSyntax(base.Kind, identifier, argumentList, initializer, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new VariableDeclaratorSyntax(base.Kind, identifier, argumentList, initializer, GetDiagnostics(), annotations);
        }

        public VariableDeclaratorSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            identifier = node;
            BracketedArgumentListSyntax bracketedArgumentListSyntax = (BracketedArgumentListSyntax)reader.ReadValue();
            if (bracketedArgumentListSyntax != null)
            {
                AdjustFlagsAndWidth(bracketedArgumentListSyntax);
                argumentList = bracketedArgumentListSyntax;
            }
            EqualsValueClauseSyntax equalsValueClauseSyntax = (EqualsValueClauseSyntax)reader.ReadValue();
            if (equalsValueClauseSyntax != null)
            {
                AdjustFlagsAndWidth(equalsValueClauseSyntax);
                initializer = equalsValueClauseSyntax;
            }
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(identifier);
            writer.WriteValue(argumentList);
            writer.WriteValue(initializer);
        }

        static VariableDeclaratorSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(VariableDeclaratorSyntax), (ObjectReader r) => new VariableDeclaratorSyntax(r));
        }
    }
}
