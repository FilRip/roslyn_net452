using System.ComponentModel;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class CrefParameterSyntax : CSharpSyntaxNode
    {
        private TypeSyntax? type;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public SyntaxToken RefOrOutKeyword => RefKindKeyword;

        public SyntaxToken RefKindKeyword
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken refKindKeyword = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CrefParameterSyntax)base.Green).refKindKeyword;
                if (refKindKeyword == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, refKindKeyword, base.Position, 0);
            }
        }

        public TypeSyntax Type => GetRed(ref type, 1);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public CrefParameterSyntax WithRefOrOutKeyword(SyntaxToken refOrOutKeyword)
        {
            return Update(refOrOutKeyword, Type);
        }

        internal CrefParameterSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref type, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return type;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitCrefParameter(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCrefParameter(this);

        public CrefParameterSyntax Update(SyntaxToken refKindKeyword, TypeSyntax type)
        {
            if (refKindKeyword != RefKindKeyword || type != Type)
            {
                CrefParameterSyntax crefParameterSyntax = SyntaxFactory.CrefParameter(refKindKeyword, type);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return crefParameterSyntax;
                }
                return crefParameterSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public CrefParameterSyntax WithRefKindKeyword(SyntaxToken refKindKeyword)
        {
            return Update(refKindKeyword, Type);
        }

        public CrefParameterSyntax WithType(TypeSyntax type)
        {
            return Update(RefKindKeyword, type);
        }
    }
}
