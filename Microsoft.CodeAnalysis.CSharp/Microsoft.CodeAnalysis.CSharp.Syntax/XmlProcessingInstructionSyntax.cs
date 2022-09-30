#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class XmlProcessingInstructionSyntax : XmlNodeSyntax
    {
        private XmlNameSyntax? name;

        public SyntaxToken StartProcessingInstructionToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlProcessingInstructionSyntax)base.Green).startProcessingInstructionToken, base.Position, 0);

        public XmlNameSyntax Name => GetRed(ref name, 1);

        public SyntaxTokenList TextTokens
        {
            get
            {
                GreenNode slot = base.Green.GetSlot(2);
                if (slot == null)
                {
                    return default(SyntaxTokenList);
                }
                return new SyntaxTokenList(this, slot, GetChildPosition(2), GetChildIndex(2));
            }
        }

        public SyntaxToken EndProcessingInstructionToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.XmlProcessingInstructionSyntax)base.Green).endProcessingInstructionToken, GetChildPosition(3), GetChildIndex(3));

        internal XmlProcessingInstructionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref name, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return name;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitXmlProcessingInstruction(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitXmlProcessingInstruction(this);

        public XmlProcessingInstructionSyntax Update(SyntaxToken startProcessingInstructionToken, XmlNameSyntax name, SyntaxTokenList textTokens, SyntaxToken endProcessingInstructionToken)
        {
            if (startProcessingInstructionToken != StartProcessingInstructionToken || name != Name || textTokens != TextTokens || endProcessingInstructionToken != EndProcessingInstructionToken)
            {
                XmlProcessingInstructionSyntax xmlProcessingInstructionSyntax = SyntaxFactory.XmlProcessingInstruction(startProcessingInstructionToken, name, textTokens, endProcessingInstructionToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return xmlProcessingInstructionSyntax;
                }
                return xmlProcessingInstructionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public XmlProcessingInstructionSyntax WithStartProcessingInstructionToken(SyntaxToken startProcessingInstructionToken)
        {
            return Update(startProcessingInstructionToken, Name, TextTokens, EndProcessingInstructionToken);
        }

        public XmlProcessingInstructionSyntax WithName(XmlNameSyntax name)
        {
            return Update(StartProcessingInstructionToken, name, TextTokens, EndProcessingInstructionToken);
        }

        public XmlProcessingInstructionSyntax WithTextTokens(SyntaxTokenList textTokens)
        {
            return Update(StartProcessingInstructionToken, Name, textTokens, EndProcessingInstructionToken);
        }

        public XmlProcessingInstructionSyntax WithEndProcessingInstructionToken(SyntaxToken endProcessingInstructionToken)
        {
            return Update(StartProcessingInstructionToken, Name, TextTokens, endProcessingInstructionToken);
        }

        public XmlProcessingInstructionSyntax AddTextTokens(params SyntaxToken[] items)
        {
            return WithTextTokens(TextTokens.AddRange(items));
        }
    }
}
