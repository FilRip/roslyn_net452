using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ConstructorInitializerSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken colonToken;

        internal readonly SyntaxToken thisOrBaseKeyword;

        internal readonly ArgumentListSyntax argumentList;

        public SyntaxToken ColonToken => colonToken;

        public SyntaxToken ThisOrBaseKeyword => thisOrBaseKeyword;

        public ArgumentListSyntax ArgumentList => argumentList;

        public ConstructorInitializerSyntax(SyntaxKind kind, SyntaxToken colonToken, SyntaxToken thisOrBaseKeyword, ArgumentListSyntax argumentList, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
            AdjustFlagsAndWidth(thisOrBaseKeyword);
            this.thisOrBaseKeyword = thisOrBaseKeyword;
            AdjustFlagsAndWidth(argumentList);
            this.argumentList = argumentList;
        }

        public ConstructorInitializerSyntax(SyntaxKind kind, SyntaxToken colonToken, SyntaxToken thisOrBaseKeyword, ArgumentListSyntax argumentList, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 3;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
            AdjustFlagsAndWidth(thisOrBaseKeyword);
            this.thisOrBaseKeyword = thisOrBaseKeyword;
            AdjustFlagsAndWidth(argumentList);
            this.argumentList = argumentList;
        }

        public ConstructorInitializerSyntax(SyntaxKind kind, SyntaxToken colonToken, SyntaxToken thisOrBaseKeyword, ArgumentListSyntax argumentList)
            : base(kind)
        {
            base.SlotCount = 3;
            AdjustFlagsAndWidth(colonToken);
            this.colonToken = colonToken;
            AdjustFlagsAndWidth(thisOrBaseKeyword);
            this.thisOrBaseKeyword = thisOrBaseKeyword;
            AdjustFlagsAndWidth(argumentList);
            this.argumentList = argumentList;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => colonToken,
                1 => thisOrBaseKeyword,
                2 => argumentList,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorInitializerSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitConstructorInitializer(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitConstructorInitializer(this);
        }

        public ConstructorInitializerSyntax Update(SyntaxToken colonToken, SyntaxToken thisOrBaseKeyword, ArgumentListSyntax argumentList)
        {
            if (colonToken != ColonToken || thisOrBaseKeyword != ThisOrBaseKeyword || argumentList != ArgumentList)
            {
                ConstructorInitializerSyntax constructorInitializerSyntax = SyntaxFactory.ConstructorInitializer(base.Kind, colonToken, thisOrBaseKeyword, argumentList);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    constructorInitializerSyntax = constructorInitializerSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    constructorInitializerSyntax = constructorInitializerSyntax.WithAnnotationsGreen(annotations);
                }
                return constructorInitializerSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ConstructorInitializerSyntax(base.Kind, colonToken, thisOrBaseKeyword, argumentList, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ConstructorInitializerSyntax(base.Kind, colonToken, thisOrBaseKeyword, argumentList, GetDiagnostics(), annotations);
        }

        public ConstructorInitializerSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 3;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            colonToken = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            thisOrBaseKeyword = node2;
            ArgumentListSyntax node3 = (ArgumentListSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            argumentList = node3;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(colonToken);
            writer.WriteValue(thisOrBaseKeyword);
            writer.WriteValue(argumentList);
        }

        static ConstructorInitializerSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ConstructorInitializerSyntax), (ObjectReader r) => new ConstructorInitializerSyntax(r));
        }
    }
}
