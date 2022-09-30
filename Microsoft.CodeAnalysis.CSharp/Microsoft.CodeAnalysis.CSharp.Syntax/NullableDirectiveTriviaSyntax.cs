#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class NullableDirectiveTriviaSyntax : DirectiveTriviaSyntax
    {
        public override SyntaxToken HashToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NullableDirectiveTriviaSyntax)base.Green).hashToken, base.Position, 0);

        public SyntaxToken NullableKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NullableDirectiveTriviaSyntax)base.Green).nullableKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken SettingToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NullableDirectiveTriviaSyntax)base.Green).settingToken, GetChildPosition(2), GetChildIndex(2));

        public SyntaxToken TargetToken
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken targetToken = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NullableDirectiveTriviaSyntax)base.Green).targetToken;
                if (targetToken == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, targetToken, GetChildPosition(3), GetChildIndex(3));
            }
        }

        public override SyntaxToken EndOfDirectiveToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NullableDirectiveTriviaSyntax)base.Green).endOfDirectiveToken, GetChildPosition(4), GetChildIndex(4));

        public override bool IsActive => ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NullableDirectiveTriviaSyntax)base.Green).IsActive;

        internal NullableDirectiveTriviaSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return null;
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return null;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitNullableDirectiveTrivia(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitNullableDirectiveTrivia(this);

        public NullableDirectiveTriviaSyntax Update(SyntaxToken hashToken, SyntaxToken nullableKeyword, SyntaxToken settingToken, SyntaxToken targetToken, SyntaxToken endOfDirectiveToken, bool isActive)
        {
            if (hashToken != HashToken || nullableKeyword != NullableKeyword || settingToken != SettingToken || targetToken != TargetToken || endOfDirectiveToken != EndOfDirectiveToken)
            {
                NullableDirectiveTriviaSyntax nullableDirectiveTriviaSyntax = SyntaxFactory.NullableDirectiveTrivia(hashToken, nullableKeyword, settingToken, targetToken, endOfDirectiveToken, isActive);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return nullableDirectiveTriviaSyntax;
                }
                return nullableDirectiveTriviaSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken)
        {
            return WithHashToken(hashToken);
        }

        public new NullableDirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
        {
            return Update(hashToken, NullableKeyword, SettingToken, TargetToken, EndOfDirectiveToken, IsActive);
        }

        public NullableDirectiveTriviaSyntax WithNullableKeyword(SyntaxToken nullableKeyword)
        {
            return Update(HashToken, nullableKeyword, SettingToken, TargetToken, EndOfDirectiveToken, IsActive);
        }

        public NullableDirectiveTriviaSyntax WithSettingToken(SyntaxToken settingToken)
        {
            return Update(HashToken, NullableKeyword, settingToken, TargetToken, EndOfDirectiveToken, IsActive);
        }

        public NullableDirectiveTriviaSyntax WithTargetToken(SyntaxToken targetToken)
        {
            return Update(HashToken, NullableKeyword, SettingToken, targetToken, EndOfDirectiveToken, IsActive);
        }

        internal override DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken)
        {
            return WithEndOfDirectiveToken(endOfDirectiveToken);
        }

        public new NullableDirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken)
        {
            return Update(HashToken, NullableKeyword, SettingToken, TargetToken, endOfDirectiveToken, IsActive);
        }

        public NullableDirectiveTriviaSyntax WithIsActive(bool isActive)
        {
            return Update(HashToken, NullableKeyword, SettingToken, TargetToken, EndOfDirectiveToken, isActive);
        }
    }
}
