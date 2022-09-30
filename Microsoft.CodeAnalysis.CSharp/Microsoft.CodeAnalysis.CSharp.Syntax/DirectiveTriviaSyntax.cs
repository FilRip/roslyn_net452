using System;
using System.Collections.Generic;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public abstract class DirectiveTriviaSyntax : StructuredTriviaSyntax
    {
        private static readonly Func<SyntaxToken, bool> s_hasDirectivesFunction = (SyntaxToken t) => t.ContainsDirectives;

        public SyntaxToken DirectiveNameToken => Kind() switch
        {
            SyntaxKind.IfDirectiveTrivia => ((IfDirectiveTriviaSyntax)this).IfKeyword,
            SyntaxKind.ElifDirectiveTrivia => ((ElifDirectiveTriviaSyntax)this).ElifKeyword,
            SyntaxKind.ElseDirectiveTrivia => ((ElseDirectiveTriviaSyntax)this).ElseKeyword,
            SyntaxKind.EndIfDirectiveTrivia => ((EndIfDirectiveTriviaSyntax)this).EndIfKeyword,
            SyntaxKind.RegionDirectiveTrivia => ((RegionDirectiveTriviaSyntax)this).RegionKeyword,
            SyntaxKind.EndRegionDirectiveTrivia => ((EndRegionDirectiveTriviaSyntax)this).EndRegionKeyword,
            SyntaxKind.ErrorDirectiveTrivia => ((ErrorDirectiveTriviaSyntax)this).ErrorKeyword,
            SyntaxKind.WarningDirectiveTrivia => ((WarningDirectiveTriviaSyntax)this).WarningKeyword,
            SyntaxKind.BadDirectiveTrivia => ((BadDirectiveTriviaSyntax)this).Identifier,
            SyntaxKind.DefineDirectiveTrivia => ((DefineDirectiveTriviaSyntax)this).DefineKeyword,
            SyntaxKind.UndefDirectiveTrivia => ((UndefDirectiveTriviaSyntax)this).UndefKeyword,
            SyntaxKind.LineDirectiveTrivia => ((LineDirectiveTriviaSyntax)this).LineKeyword,
            SyntaxKind.PragmaWarningDirectiveTrivia => ((PragmaWarningDirectiveTriviaSyntax)this).PragmaKeyword,
            SyntaxKind.PragmaChecksumDirectiveTrivia => ((PragmaChecksumDirectiveTriviaSyntax)this).PragmaKeyword,
            SyntaxKind.ReferenceDirectiveTrivia => ((ReferenceDirectiveTriviaSyntax)this).ReferenceKeyword,
            SyntaxKind.LoadDirectiveTrivia => ((LoadDirectiveTriviaSyntax)this).LoadKeyword,
            SyntaxKind.ShebangDirectiveTrivia => ((ShebangDirectiveTriviaSyntax)this).ExclamationToken,
            SyntaxKind.NullableDirectiveTrivia => ((NullableDirectiveTriviaSyntax)this).NullableKeyword,
            _ => throw ExceptionUtilities.UnexpectedValue(Kind()),
        };

        public abstract SyntaxToken HashToken { get; }

        public abstract SyntaxToken EndOfDirectiveToken { get; }

        public abstract bool IsActive { get; }

        public DirectiveTriviaSyntax? GetNextDirective(Func<DirectiveTriviaSyntax, bool>? predicate = null)
        {
            SyntaxToken token = ParentTrivia.Token;
            bool flag = false;
            while (token.Kind() != 0)
            {
                SyntaxTriviaList.Enumerator enumerator = token.LeadingTrivia.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxTrivia current = enumerator.Current;
                    if (flag)
                    {
                        if (current.IsDirective)
                        {
                            DirectiveTriviaSyntax directiveTriviaSyntax = (DirectiveTriviaSyntax)current.GetStructure();
                            if (predicate == null || predicate!(directiveTriviaSyntax))
                            {
                                return directiveTriviaSyntax;
                            }
                        }
                    }
                    else if (current.UnderlyingNode == base.Green)
                    {
                        flag = true;
                    }
                }
                token = token.GetNextToken(s_hasDirectivesFunction);
            }
            return null;
        }

        public DirectiveTriviaSyntax? GetPreviousDirective(Func<DirectiveTriviaSyntax, bool>? predicate = null)
        {
            SyntaxToken token = ParentTrivia.Token;
            bool flag = false;
            while (token.Kind() != 0)
            {
                SyntaxTriviaList.Reversed.Enumerator enumerator = token.LeadingTrivia.Reverse().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxTrivia current = enumerator.Current;
                    if (flag)
                    {
                        if (current.IsDirective)
                        {
                            DirectiveTriviaSyntax directiveTriviaSyntax = (DirectiveTriviaSyntax)current.GetStructure();
                            if (predicate == null || predicate!(directiveTriviaSyntax))
                            {
                                return directiveTriviaSyntax;
                            }
                        }
                    }
                    else if (current.UnderlyingNode == base.Green)
                    {
                        flag = true;
                    }
                }
                token = token.GetPreviousToken(s_hasDirectivesFunction);
            }
            return null;
        }

        public List<DirectiveTriviaSyntax> GetRelatedDirectives()
        {
            List<DirectiveTriviaSyntax> list = new List<DirectiveTriviaSyntax>();
            GetRelatedDirectives(list);
            return list;
        }

        private void GetRelatedDirectives(List<DirectiveTriviaSyntax> list)
        {
            list.Clear();
            for (DirectiveTriviaSyntax previousRelatedDirective = GetPreviousRelatedDirective(); previousRelatedDirective != null; previousRelatedDirective = previousRelatedDirective.GetPreviousRelatedDirective())
            {
                list.Add(previousRelatedDirective);
            }
            list.Reverse();
            list.Add(this);
            for (DirectiveTriviaSyntax nextRelatedDirective = GetNextRelatedDirective(); nextRelatedDirective != null; nextRelatedDirective = nextRelatedDirective.GetNextRelatedDirective())
            {
                list.Add(nextRelatedDirective);
            }
        }

        private DirectiveTriviaSyntax? GetNextRelatedDirective()
        {
            DirectiveTriviaSyntax directiveTriviaSyntax = this;
            switch (directiveTriviaSyntax.Kind())
            {
                case SyntaxKind.IfDirectiveTrivia:
                    while (directiveTriviaSyntax != null)
                    {
                        SyntaxKind syntaxKind = directiveTriviaSyntax.Kind();
                        if (syntaxKind - 8549 <= (SyntaxKind)2)
                        {
                            return directiveTriviaSyntax;
                        }
                        directiveTriviaSyntax = directiveTriviaSyntax.GetNextPossiblyRelatedDirective();
                    }
                    break;
                case SyntaxKind.ElifDirectiveTrivia:
                    for (directiveTriviaSyntax = directiveTriviaSyntax.GetNextPossiblyRelatedDirective(); directiveTriviaSyntax != null; directiveTriviaSyntax = directiveTriviaSyntax.GetNextPossiblyRelatedDirective())
                    {
                        SyntaxKind syntaxKind = directiveTriviaSyntax.Kind();
                        if (syntaxKind - 8549 <= (SyntaxKind)2)
                        {
                            return directiveTriviaSyntax;
                        }
                    }
                    break;
                case SyntaxKind.ElseDirectiveTrivia:
                    while (directiveTriviaSyntax != null)
                    {
                        if (directiveTriviaSyntax.Kind() == SyntaxKind.EndIfDirectiveTrivia)
                        {
                            return directiveTriviaSyntax;
                        }
                        directiveTriviaSyntax = directiveTriviaSyntax.GetNextPossiblyRelatedDirective();
                    }
                    break;
                case SyntaxKind.RegionDirectiveTrivia:
                    while (directiveTriviaSyntax != null)
                    {
                        if (directiveTriviaSyntax.Kind() == SyntaxKind.EndRegionDirectiveTrivia)
                        {
                            return directiveTriviaSyntax;
                        }
                        directiveTriviaSyntax = directiveTriviaSyntax.GetNextPossiblyRelatedDirective();
                    }
                    break;
            }
            return null;
        }

        private DirectiveTriviaSyntax? GetNextPossiblyRelatedDirective()
        {
            DirectiveTriviaSyntax directiveTriviaSyntax = this;
            while (directiveTriviaSyntax != null)
            {
                directiveTriviaSyntax = directiveTriviaSyntax.GetNextDirective();
                if (directiveTriviaSyntax != null)
                {
                    SyntaxKind syntaxKind = directiveTriviaSyntax.Kind();
                    if (syntaxKind == SyntaxKind.IfDirectiveTrivia)
                    {
                        while (directiveTriviaSyntax != null && directiveTriviaSyntax.Kind() != SyntaxKind.EndIfDirectiveTrivia)
                        {
                            directiveTriviaSyntax = directiveTriviaSyntax.GetNextRelatedDirective();
                        }
                        continue;
                    }
                    if (syntaxKind == SyntaxKind.RegionDirectiveTrivia)
                    {
                        while (directiveTriviaSyntax != null && directiveTriviaSyntax.Kind() != SyntaxKind.EndRegionDirectiveTrivia)
                        {
                            directiveTriviaSyntax = directiveTriviaSyntax.GetNextRelatedDirective();
                        }
                        continue;
                    }
                }
                return directiveTriviaSyntax;
            }
            return null;
        }

        private DirectiveTriviaSyntax? GetPreviousRelatedDirective()
        {
            DirectiveTriviaSyntax directiveTriviaSyntax = this;
            switch (directiveTriviaSyntax.Kind())
            {
                case SyntaxKind.EndIfDirectiveTrivia:
                    while (directiveTriviaSyntax != null)
                    {
                        SyntaxKind syntaxKind = directiveTriviaSyntax.Kind();
                        if (syntaxKind - 8548 <= (SyntaxKind)2)
                        {
                            return directiveTriviaSyntax;
                        }
                        directiveTriviaSyntax = directiveTriviaSyntax.GetPreviousPossiblyRelatedDirective();
                    }
                    break;
                case SyntaxKind.ElifDirectiveTrivia:
                    for (directiveTriviaSyntax = directiveTriviaSyntax.GetPreviousPossiblyRelatedDirective(); directiveTriviaSyntax != null; directiveTriviaSyntax = directiveTriviaSyntax.GetPreviousPossiblyRelatedDirective())
                    {
                        SyntaxKind syntaxKind = directiveTriviaSyntax.Kind();
                        if (syntaxKind - 8548 <= SyntaxKind.List)
                        {
                            return directiveTriviaSyntax;
                        }
                    }
                    break;
                case SyntaxKind.ElseDirectiveTrivia:
                    while (directiveTriviaSyntax != null)
                    {
                        SyntaxKind syntaxKind = directiveTriviaSyntax.Kind();
                        if (syntaxKind - 8548 <= SyntaxKind.List)
                        {
                            return directiveTriviaSyntax;
                        }
                        directiveTriviaSyntax = directiveTriviaSyntax.GetPreviousPossiblyRelatedDirective();
                    }
                    break;
                case SyntaxKind.EndRegionDirectiveTrivia:
                    while (directiveTriviaSyntax != null)
                    {
                        if (directiveTriviaSyntax.Kind() == SyntaxKind.RegionDirectiveTrivia)
                        {
                            return directiveTriviaSyntax;
                        }
                        directiveTriviaSyntax = directiveTriviaSyntax.GetPreviousPossiblyRelatedDirective();
                    }
                    break;
            }
            return null;
        }

        private DirectiveTriviaSyntax? GetPreviousPossiblyRelatedDirective()
        {
            DirectiveTriviaSyntax directiveTriviaSyntax = this;
            while (directiveTriviaSyntax != null)
            {
                directiveTriviaSyntax = directiveTriviaSyntax.GetPreviousDirective();
                if (directiveTriviaSyntax != null)
                {
                    SyntaxKind syntaxKind = directiveTriviaSyntax.Kind();
                    if (syntaxKind == SyntaxKind.EndIfDirectiveTrivia)
                    {
                        while (directiveTriviaSyntax != null && directiveTriviaSyntax.Kind() != SyntaxKind.IfDirectiveTrivia)
                        {
                            directiveTriviaSyntax = directiveTriviaSyntax.GetPreviousRelatedDirective();
                        }
                        continue;
                    }
                    if (syntaxKind == SyntaxKind.EndRegionDirectiveTrivia)
                    {
                        while (directiveTriviaSyntax != null && directiveTriviaSyntax.Kind() != SyntaxKind.RegionDirectiveTrivia)
                        {
                            directiveTriviaSyntax = directiveTriviaSyntax.GetPreviousRelatedDirective();
                        }
                        continue;
                    }
                }
                return directiveTriviaSyntax;
            }
            return null;
        }

        internal DirectiveTriviaSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public DirectiveTriviaSyntax WithHashToken(SyntaxToken hashToken)
        {
            return WithHashTokenCore(hashToken);
        }

        internal abstract DirectiveTriviaSyntax WithHashTokenCore(SyntaxToken hashToken);

        public DirectiveTriviaSyntax WithEndOfDirectiveToken(SyntaxToken endOfDirectiveToken)
        {
            return WithEndOfDirectiveTokenCore(endOfDirectiveToken);
        }

        internal abstract DirectiveTriviaSyntax WithEndOfDirectiveTokenCore(SyntaxToken endOfDirectiveToken);
    }
}
