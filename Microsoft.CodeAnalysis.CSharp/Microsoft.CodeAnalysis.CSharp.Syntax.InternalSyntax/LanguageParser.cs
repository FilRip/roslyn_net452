using System;
using System.Collections.Generic;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public class LanguageParser : SyntaxParser
    {
        [Flags()]
        internal enum TerminatorState
        {
            EndOfFile = 0,
            IsNamespaceMemberStartOrStop = 1,
            IsAttributeDeclarationTerminator = 2,
            IsPossibleAggregateClauseStartOrStop = 4,
            IsPossibleMemberStartOrStop = 8,
            IsEndOfReturnType = 0x10,
            IsEndOfParameterList = 0x20,
            IsEndOfFieldDeclaration = 0x40,
            IsPossibleEndOfVariableDeclaration = 0x80,
            IsEndOfTypeArgumentList = 0x100,
            IsPossibleStatementStartOrStop = 0x200,
            IsEndOfFixedStatement = 0x400,
            IsEndOfTryBlock = 0x800,
            IsEndOfCatchClause = 0x1000,
            IsEndOfFilterClause = 0x2000,
            IsEndOfCatchBlock = 0x4000,
            IsEndOfDoWhileExpression = 0x8000,
            IsEndOfForStatementArgument = 0x10000,
            IsEndOfDeclarationClause = 0x20000,
            IsEndOfArgumentList = 0x40000,
            IsSwitchSectionStart = 0x80000,
            IsEndOfTypeParameterList = 0x100000,
            IsEndOfMethodSignature = 0x200000,
            IsEndOfNameInExplicitInterface = 0x400000,
            IsEndOfFunctionPointerParameterList = 0x800000,
            IsEndOfFunctionPointerParameterListErrored = 0x1000000,
            IsEndOfFunctionPointerCallingConvention = 0x2000000,
            IsEndOfRecordSignature = 0x4000000
        }

        private struct NamespaceBodyBuilder
        {
            public SyntaxListBuilder<ExternAliasDirectiveSyntax> Externs;

            public SyntaxListBuilder<UsingDirectiveSyntax> Usings;

            public SyntaxListBuilder<AttributeListSyntax> Attributes;

            public SyntaxListBuilder<MemberDeclarationSyntax> Members;

            public NamespaceBodyBuilder(SyntaxListPool pool)
            {
                Externs = pool.Allocate<ExternAliasDirectiveSyntax>();
                Usings = pool.Allocate<UsingDirectiveSyntax>();
                Attributes = pool.Allocate<AttributeListSyntax>();
                Members = pool.Allocate<MemberDeclarationSyntax>();
            }

            internal void Free(SyntaxListPool pool)
            {
                pool.Free(Members);
                pool.Free(Attributes);
                pool.Free(Usings);
                pool.Free(Externs);
            }
        }

        private enum NamespaceParts
        {
            None,
            ExternAliases,
            Usings,
            GlobalAttributes,
            MembersAndStatements,
            TypesAndNamespaces,
            TopLevelStatementsAfterTypesAndNamespaces
        }

        private enum PostSkipAction
        {
            Continue,
            Abort
        }

        [Flags()]
        private enum VariableFlags
        {
            Fixed = 1,
            Const = 2,
            Local = 4
        }

        [Flags()]
        private enum NameOptions
        {
            None = 0,
            InExpression = 1,
            InTypeList = 2,
            PossiblePattern = 4,
            AfterIs = 8,
            DefinitePattern = 0x10,
            AfterOut = 0x20,
            AfterTupleComma = 0x40,
            FirstElementOfPossibleTupleLiteral = 0x80
        }

        private enum ScanTypeArgumentListKind
        {
            NotTypeArgumentList,
            PossibleTypeArgumentList,
            DefiniteTypeArgumentList
        }

        private enum ScanTypeFlags
        {
            NotType,
            MustBeType,
            GenericTypeOrMethod,
            GenericTypeOrExpression,
            NonGenericTypeOrExpression,
            AliasQualifiedName,
            NullableType,
            PointerOrMultiplication,
            TupleType
        }

        private enum ParseTypeMode
        {
            Normal,
            Parameter,
            AfterIs,
            DefinitePattern,
            AfterOut,
            AfterRef,
            AfterTupleComma,
            AsExpression,
            NewExpression,
            FirstElementOfPossibleTupleLiteral
        }

        private enum Precedence : uint
        {
            Expression = 0u,
            Assignment = 0u,
            Lambda = 0u,
            Conditional = 1u,
            Coalescing = 2u,
            ConditionalOr = 3u,
            ConditionalAnd = 4u,
            LogicalOr = 5u,
            LogicalXor = 6u,
            LogicalAnd = 7u,
            Equality = 8u,
            Relational = 9u,
            Shift = 10u,
            Additive = 11u,
            Mutiplicative = 12u,
            Switch = 13u,
            Range = 14u,
            Unary = 15u,
            Cast = 16u,
            PointerIndirection = 17u,
            AddressOf = 18u,
            Primary = 19u
        }

        private new struct ResetPoint
        {
            internal SyntaxParser.ResetPoint BaseResetPoint;

            internal readonly TerminatorState TerminatorState;

            internal readonly bool IsInTry;

            internal readonly bool IsInAsync;

            internal readonly int QueryDepth;

            internal ResetPoint(SyntaxParser.ResetPoint resetPoint, TerminatorState terminatorState, bool isInTry, bool isInAsync, int queryDepth)
            {
                BaseResetPoint = resetPoint;
                TerminatorState = terminatorState;
                IsInTry = isInTry;
                IsInAsync = isInAsync;
                QueryDepth = queryDepth;
            }
        }

        private readonly SyntaxListPool _pool = new SyntaxListPool();

        private readonly SyntaxFactoryContext _syntaxFactoryContext;

        private readonly ContextAwareSyntax _syntaxFactory;

        private int _recursionDepth;

        private TerminatorState _termState;

        private bool _isInTry;

        private bool _checkedTopLevelStatementsFeatureAvailability;

        private const int LastTerminatorState = 67108864;

        private bool IsCurrentTokenQueryContextualKeyword => IsTokenQueryContextualKeyword(base.CurrentToken);

        [Obsolete("Use IsIncrementalAndFactoryContextMatches")]
        private new bool IsIncremental
        {
            get
            {
                throw new Exception("Use IsIncrementalAndFactoryContextMatches");
            }
        }

        private bool IsIncrementalAndFactoryContextMatches
        {
            get
            {
                if (!base.IsIncremental)
                {
                    return false;
                }
                Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode currentNode = base.CurrentNode;
                if (currentNode != null)
                {
                    return MatchesFactoryContext(currentNode.Green, _syntaxFactoryContext);
                }
                return false;
            }
        }

        private bool IsInAsync
        {
            get
            {
                return _syntaxFactoryContext.IsInAsync;
            }
            set
            {
                _syntaxFactoryContext.IsInAsync = value;
            }
        }

        private bool IsInQuery => _syntaxFactoryContext.IsInQuery;

        public LanguageParser(Lexer lexer, Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode oldTree, IEnumerable<TextChangeRange> changes, LexerMode lexerMode = LexerMode.Syntax, CancellationToken cancellationToken = default(CancellationToken))
            : base(lexer, lexerMode, oldTree, changes, allowModeReset: false, preLexIfNotIncremental: true, cancellationToken)
        {
            _syntaxFactoryContext = new SyntaxFactoryContext();
            _syntaxFactory = new ContextAwareSyntax(_syntaxFactoryContext);
        }

        private static bool IsSomeWord(SyntaxKind kind)
        {
            if (kind != SyntaxKind.IdentifierToken)
            {
                return SyntaxFacts.IsKeywordKind(kind);
            }
            return true;
        }

        private bool IsTerminator()
        {
            if (base.CurrentToken.Kind == SyntaxKind.EndOfFileToken)
            {
                return true;
            }
            for (int num = 1; num <= 67108864; num <<= 1)
            {
                switch ((uint)_termState & (uint)num)
                {
                    case 1u:
                        if (!IsNamespaceMemberStartOrStop())
                        {
                            continue;
                        }
                        break;
                    case 2u:
                        if (!IsAttributeDeclarationTerminator())
                        {
                            continue;
                        }
                        break;
                    case 4u:
                        if (!IsPossibleAggregateClauseStartOrStop())
                        {
                            continue;
                        }
                        break;
                    case 8u:
                        if (!IsPossibleMemberStartOrStop())
                        {
                            continue;
                        }
                        break;
                    case 16u:
                        if (!IsEndOfReturnType())
                        {
                            continue;
                        }
                        break;
                    case 32u:
                        if (!IsEndOfParameterList())
                        {
                            continue;
                        }
                        break;
                    case 64u:
                        if (!IsEndOfFieldDeclaration())
                        {
                            continue;
                        }
                        break;
                    case 128u:
                        if (!IsPossibleEndOfVariableDeclaration())
                        {
                            continue;
                        }
                        break;
                    case 256u:
                        if (!IsEndOfTypeArgumentList())
                        {
                            continue;
                        }
                        break;
                    case 512u:
                        if (!IsPossibleStatementStartOrStop())
                        {
                            continue;
                        }
                        break;
                    case 1024u:
                        if (!IsEndOfFixedStatement())
                        {
                            continue;
                        }
                        break;
                    case 2048u:
                        if (!IsEndOfTryBlock())
                        {
                            continue;
                        }
                        break;
                    case 4096u:
                        if (!IsEndOfCatchClause())
                        {
                            continue;
                        }
                        break;
                    case 8192u:
                        if (!IsEndOfFilterClause())
                        {
                            continue;
                        }
                        break;
                    case 16384u:
                        if (!IsEndOfCatchBlock())
                        {
                            continue;
                        }
                        break;
                    case 32768u:
                        if (!IsEndOfDoWhileExpression())
                        {
                            continue;
                        }
                        break;
                    case 65536u:
                        if (!IsEndOfForStatementArgument())
                        {
                            continue;
                        }
                        break;
                    case 131072u:
                        if (!IsEndOfDeclarationClause())
                        {
                            continue;
                        }
                        break;
                    case 262144u:
                        if (!IsEndOfArgumentList())
                        {
                            continue;
                        }
                        break;
                    case 524288u:
                        if (!IsPossibleSwitchSection())
                        {
                            continue;
                        }
                        break;
                    case 1048576u:
                        if (!IsEndOfTypeParameterList())
                        {
                            continue;
                        }
                        break;
                    case 2097152u:
                        if (!IsEndOfMethodSignature())
                        {
                            continue;
                        }
                        break;
                    case 4194304u:
                        if (!IsEndOfNameInExplicitInterface())
                        {
                            continue;
                        }
                        break;
                    case 8388608u:
                        if (!IsEndOfFunctionPointerParameterList(errored: false))
                        {
                            continue;
                        }
                        break;
                    case 16777216u:
                        if (!IsEndOfFunctionPointerParameterList(errored: true))
                        {
                            continue;
                        }
                        break;
                    case 33554432u:
                        if (!IsEndOfFunctionPointerCallingConvention())
                        {
                            continue;
                        }
                        break;
                    case 67108864u:
                        if (!IsEndOfRecordSignature())
                        {
                            continue;
                        }
                        break;
                    default:
                        continue;
                }
                return true;
            }
            return false;
        }

        private static Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode GetOldParent(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode node)
        {
            return node?.Parent;
        }

        internal CompilationUnitSyntax ParseCompilationUnit()
        {
            return ParseWithStackGuard<CompilationUnitSyntax>(ParseCompilationUnitCore, () => SyntaxFactory.CompilationUnit(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ExternAliasDirectiveSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<UsingDirectiveSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<MemberDeclarationSyntax>), SyntaxFactory.Token(SyntaxKind.EndOfFileToken)));
        }

        internal CompilationUnitSyntax ParseCompilationUnitCore()
        {
            SyntaxToken openBrace = null;
            SyntaxListBuilder initialBadNodes = null;
            NamespaceBodyBuilder body = new NamespaceBodyBuilder(_pool);
            try
            {
                ParseNamespaceBody(ref openBrace, ref body, ref initialBadNodes, SyntaxKind.CompilationUnit);
                SyntaxToken endOfFileToken = EatToken(SyntaxKind.EndOfFileToken);
                CompilationUnitSyntax compilationUnitSyntax = _syntaxFactory.CompilationUnit(body.Externs, body.Usings, body.Attributes, body.Members, endOfFileToken);
                if (initialBadNodes != null)
                {
                    compilationUnitSyntax = AddLeadingSkippedSyntax(compilationUnitSyntax, initialBadNodes.ToListNode());
                    _pool.Free(initialBadNodes);
                }
                return compilationUnitSyntax;
            }
            finally
            {
                body.Free(_pool);
            }
        }

        internal TNode ParseWithStackGuard<TNode>(Func<TNode> parseFunc, Func<TNode> createEmptyNodeFunc) where TNode : CSharpSyntaxNode
        {
            try
            {
                return parseFunc();
            }
            catch (InsufficientExecutionStackException)
            {
                return CreateForGlobalFailure(lexer.TextWindow.Position, createEmptyNodeFunc());
            }
        }

        private TNode CreateForGlobalFailure<TNode>(int position, TNode node) where TNode : CSharpSyntaxNode
        {
            SyntaxListBuilder syntaxListBuilder = new SyntaxListBuilder(1);
            syntaxListBuilder.Add(SyntaxFactory.BadToken(null, lexer.TextWindow.Text.ToString(), null));
            SkippedTokensTriviaSyntax skippedSyntax = _syntaxFactory.SkippedTokensTrivia(syntaxListBuilder.ToList<SyntaxToken>());
            node = AddLeadingSkippedSyntax(node, skippedSyntax);
            ForceEndOfFile();
            return AddError(node, position, 0, ErrorCode.ERR_InsufficientStack);
        }

        private NamespaceDeclarationSyntax ParseNamespaceDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, SyntaxListBuilder modifiers)
        {
            _recursionDepth++;
            StackGuard.EnsureSufficientExecutionStack(_recursionDepth);
            NamespaceDeclarationSyntax result = ParseNamespaceDeclarationCore(attributeLists, modifiers);
            _recursionDepth--;
            return result;
        }

        private NamespaceDeclarationSyntax ParseNamespaceDeclarationCore(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists, SyntaxListBuilder modifiers)
        {
            SyntaxToken syntaxToken = EatToken(SyntaxKind.NamespaceKeyword);
            if (base.IsScript)
            {
                syntaxToken = AddError(syntaxToken, ErrorCode.ERR_NamespaceNotAllowedInScript);
            }
            NameSyntax name = ParseQualifiedName();
            SyntaxToken openBrace;
            if (base.CurrentToken.Kind == SyntaxKind.OpenBraceToken || IsPossibleNamespaceMemberDeclaration())
            {
                openBrace = EatToken(SyntaxKind.OpenBraceToken);
            }
            else
            {
                openBrace = EatTokenWithPrejudice(SyntaxKind.OpenBraceToken);
                openBrace = ConvertToMissingWithTrailingTrivia(openBrace, SyntaxKind.OpenBraceToken);
            }
            NamespaceBodyBuilder body = new NamespaceBodyBuilder(_pool);
            SyntaxListBuilder initialBadNodes = null;
            try
            {
                ParseNamespaceBody(ref openBrace, ref body, ref initialBadNodes, SyntaxKind.NamespaceDeclaration);
                SyntaxToken closeBraceToken = EatToken(SyntaxKind.CloseBraceToken);
                SyntaxToken semicolonToken = TryEatToken(SyntaxKind.SemicolonToken);
                return _syntaxFactory.NamespaceDeclaration(attributeLists, modifiers.ToList(), syntaxToken, name, openBrace, body.Externs, body.Usings, body.Members, closeBraceToken, semicolonToken);
            }
            finally
            {
                body.Free(_pool);
            }
        }

        private static bool IsPossibleStartOfTypeDeclaration(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.OpenBracketToken:
                case SyntaxKind.PublicKeyword:
                case SyntaxKind.PrivateKeyword:
                case SyntaxKind.InternalKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.StaticKeyword:
                case SyntaxKind.SealedKeyword:
                case SyntaxKind.NewKeyword:
                case SyntaxKind.AbstractKeyword:
                case SyntaxKind.ClassKeyword:
                case SyntaxKind.StructKeyword:
                case SyntaxKind.InterfaceKeyword:
                case SyntaxKind.EnumKeyword:
                case SyntaxKind.DelegateKeyword:
                case SyntaxKind.UnsafeKeyword:
                    return true;
                default:
                    return false;
            }
        }

        private void AddSkippedNamespaceText(ref SyntaxToken openBrace, ref NamespaceBodyBuilder body, ref SyntaxListBuilder initialBadNodes, CSharpSyntaxNode skippedSyntax)
        {
            if (body.Members.Count > 0)
            {
                AddTrailingSkippedSyntax(body.Members, skippedSyntax);
                return;
            }
            if (body.Attributes.Count > 0)
            {
                AddTrailingSkippedSyntax(body.Attributes, skippedSyntax);
                return;
            }
            if (body.Usings.Count > 0)
            {
                AddTrailingSkippedSyntax(body.Usings, skippedSyntax);
                return;
            }
            if (body.Externs.Count > 0)
            {
                AddTrailingSkippedSyntax(body.Externs, skippedSyntax);
                return;
            }
            if (openBrace != null)
            {
                openBrace = AddTrailingSkippedSyntax(openBrace, skippedSyntax);
                return;
            }
            if (initialBadNodes == null)
            {
                initialBadNodes = _pool.Allocate();
            }
            initialBadNodes.AddRange(skippedSyntax);
        }

        private void ParseNamespaceBody(ref SyntaxToken openBrace, ref NamespaceBodyBuilder body, ref SyntaxListBuilder initialBadNodes, SyntaxKind parentKind)
        {
            bool flag = openBrace == null;
            TerminatorState termState = _termState;
            _termState |= TerminatorState.IsNamespaceMemberStartOrStop;
            NamespaceParts seen2 = NamespaceParts.None;
            SyntaxListBuilder<MemberDeclarationSyntax> incompleteMembers = _pool.Allocate<MemberDeclarationSyntax>();
            bool flag2 = true;
            try
            {
                while (true)
                {
                    switch (base.CurrentToken.Kind)
                    {
                        case SyntaxKind.NamespaceKeyword:
                            {
                                AddIncompleteMembers(ref incompleteMembers, ref body);
                                SyntaxListBuilder<AttributeListSyntax> syntaxListBuilder = _pool.Allocate<AttributeListSyntax>();
                                SyntaxListBuilder syntaxListBuilder2 = _pool.Allocate();
                                body.Members.Add(adjustStateAndReportStatementOutOfOrder(ref seen2, ParseNamespaceDeclaration(syntaxListBuilder, syntaxListBuilder2)));
                                _pool.Free(syntaxListBuilder);
                                _pool.Free(syntaxListBuilder2);
                                flag2 = true;
                                continue;
                            }
                        case SyntaxKind.CloseBraceToken:
                            if (flag)
                            {
                                ReduceIncompleteMembers(ref incompleteMembers, ref openBrace, ref body, ref initialBadNodes);
                                SyntaxToken node2 = EatToken();
                                node2 = AddError(node2, base.IsScript ? ErrorCode.ERR_GlobalDefinitionOrStatementExpected : ErrorCode.ERR_EOFExpected);
                                AddSkippedNamespaceText(ref openBrace, ref body, ref initialBadNodes, node2);
                                flag2 = true;
                                continue;
                            }
                            return;
                        case SyntaxKind.EndOfFileToken:
                            return;
                        case SyntaxKind.ExternKeyword:
                            if (!flag || ScanExternAliasDirective())
                            {
                                ReduceIncompleteMembers(ref incompleteMembers, ref openBrace, ref body, ref initialBadNodes);
                                ExternAliasDirectiveSyntax node = ParseExternAliasDirective();
                                if (seen2 > NamespaceParts.ExternAliases)
                                {
                                    node = AddErrorToFirstToken(node, ErrorCode.ERR_ExternAfterElements);
                                    AddSkippedNamespaceText(ref openBrace, ref body, ref initialBadNodes, node);
                                }
                                else
                                {
                                    body.Externs.Add(node);
                                    seen2 = NamespaceParts.ExternAliases;
                                }
                                flag2 = true;
                                continue;
                            }
                            break;
                        case SyntaxKind.UsingKeyword:
                            if (!flag || (PeekToken(1).Kind != SyntaxKind.OpenParenToken && (base.IsScript || !IsPossibleTopLevelUsingLocalDeclarationStatement())))
                            {
                                parseUsingDirective(ref openBrace, ref body, ref initialBadNodes, ref seen2, ref incompleteMembers);
                                flag2 = true;
                                continue;
                            }
                            break;
                        case SyntaxKind.IdentifierToken:
                            if (base.CurrentToken.ContextualKind == SyntaxKind.GlobalKeyword && PeekToken(1).Kind == SyntaxKind.UsingKeyword)
                            {
                                parseUsingDirective(ref openBrace, ref body, ref initialBadNodes, ref seen2, ref incompleteMembers);
                                flag2 = true;
                                continue;
                            }
                            break;
                        case SyntaxKind.OpenBracketToken:
                            if (IsPossibleGlobalAttributeDeclaration())
                            {
                                ReduceIncompleteMembers(ref incompleteMembers, ref openBrace, ref body, ref initialBadNodes);
                                AttributeListSyntax attributeListSyntax = ParseAttributeDeclaration();
                                if (!flag || seen2 > NamespaceParts.GlobalAttributes)
                                {
                                    attributeListSyntax = AddError(attributeListSyntax, attributeListSyntax.Target!.Identifier, ErrorCode.ERR_GlobalAttributesNotFirst);
                                    AddSkippedNamespaceText(ref openBrace, ref body, ref initialBadNodes, attributeListSyntax);
                                }
                                else
                                {
                                    body.Attributes.Add(attributeListSyntax);
                                    seen2 = NamespaceParts.GlobalAttributes;
                                }
                                flag2 = true;
                                continue;
                            }
                            break;
                    }
                    MemberDeclarationSyntax memberDeclarationSyntax = (flag ? ParseMemberDeclarationOrStatement(parentKind) : ParseMemberDeclaration(parentKind));
                    if (memberDeclarationSyntax == null)
                    {
                        ReduceIncompleteMembers(ref incompleteMembers, ref openBrace, ref body, ref initialBadNodes);
                        SyntaxToken syntaxToken = EatToken();
                        if (flag2 && !syntaxToken.ContainsDiagnostics)
                        {
                            syntaxToken = AddError(syntaxToken, base.IsScript ? ErrorCode.ERR_GlobalDefinitionOrStatementExpected : ErrorCode.ERR_EOFExpected);
                            flag2 = false;
                        }
                        AddSkippedNamespaceText(ref openBrace, ref body, ref initialBadNodes, syntaxToken);
                    }
                    else if (memberDeclarationSyntax.Kind == SyntaxKind.IncompleteMember && seen2 < NamespaceParts.MembersAndStatements)
                    {
                        incompleteMembers.Add(memberDeclarationSyntax);
                        flag2 = true;
                    }
                    else
                    {
                        AddIncompleteMembers(ref incompleteMembers, ref body);
                        body.Members.Add(adjustStateAndReportStatementOutOfOrder(ref seen2, memberDeclarationSyntax));
                        flag2 = true;
                    }
                }
            }
            finally
            {
                _termState = termState;
                AddIncompleteMembers(ref incompleteMembers, ref body);
                _pool.Free(incompleteMembers);
            }
            MemberDeclarationSyntax adjustStateAndReportStatementOutOfOrder(ref NamespaceParts seen, MemberDeclarationSyntax memberOrStatement)
            {
                switch (memberOrStatement.Kind)
                {
                    case SyntaxKind.GlobalStatement:
                        if (seen < NamespaceParts.MembersAndStatements)
                        {
                            seen = NamespaceParts.MembersAndStatements;
                        }
                        else if (seen == NamespaceParts.TypesAndNamespaces)
                        {
                            seen = NamespaceParts.TopLevelStatementsAfterTypesAndNamespaces;
                            if (!base.IsScript)
                            {
                                memberOrStatement = AddError(memberOrStatement, ErrorCode.ERR_TopLevelStatementAfterNamespaceOrType);
                            }
                        }
                        break;
                    case SyntaxKind.NamespaceDeclaration:
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.StructDeclaration:
                    case SyntaxKind.InterfaceDeclaration:
                    case SyntaxKind.EnumDeclaration:
                    case SyntaxKind.DelegateDeclaration:
                    case SyntaxKind.RecordDeclaration:
                    case SyntaxKind.RecordStructDeclaration:
                        if (seen < NamespaceParts.TypesAndNamespaces)
                        {
                            seen = NamespaceParts.TypesAndNamespaces;
                        }
                        break;
                    default:
                        if (seen < NamespaceParts.MembersAndStatements)
                        {
                            seen = NamespaceParts.MembersAndStatements;
                        }
                        break;
                }
                return memberOrStatement;
            }
            void parseUsingDirective(ref SyntaxToken openBrace, ref NamespaceBodyBuilder body, ref SyntaxListBuilder initialBadNodes, ref NamespaceParts seen, ref SyntaxListBuilder<MemberDeclarationSyntax> pendingIncompleteMembers)
            {
                ReduceIncompleteMembers(ref pendingIncompleteMembers, ref openBrace, ref body, ref initialBadNodes);
                UsingDirectiveSyntax node3 = ParseUsingDirective();
                if (seen > NamespaceParts.Usings)
                {
                    node3 = AddError(node3, ErrorCode.ERR_UsingAfterElements);
                    AddSkippedNamespaceText(ref openBrace, ref body, ref initialBadNodes, node3);
                }
                else
                {
                    body.Usings.Add(node3);
                    seen = NamespaceParts.Usings;
                }
            }
        }

        private GlobalStatementSyntax CheckTopLevelStatementsFeatureAvailability(GlobalStatementSyntax globalStatementSyntax)
        {
            if (base.IsScript || _checkedTopLevelStatementsFeatureAvailability)
            {
                return globalStatementSyntax;
            }
            _checkedTopLevelStatementsFeatureAvailability = true;
            return CheckFeatureAvailability(globalStatementSyntax, MessageID.IDS_TopLevelStatements);
        }

        private static void AddIncompleteMembers(ref SyntaxListBuilder<MemberDeclarationSyntax> incompleteMembers, ref NamespaceBodyBuilder body)
        {
            if (incompleteMembers.Count > 0)
            {
                body.Members.AddRange(incompleteMembers);
                incompleteMembers.Clear();
            }
        }

        private void ReduceIncompleteMembers(ref SyntaxListBuilder<MemberDeclarationSyntax> incompleteMembers, ref SyntaxToken openBrace, ref NamespaceBodyBuilder body, ref SyntaxListBuilder initialBadNodes)
        {
            for (int i = 0; i < incompleteMembers.Count; i++)
            {
                AddSkippedNamespaceText(ref openBrace, ref body, ref initialBadNodes, incompleteMembers[i]);
            }
            incompleteMembers.Clear();
        }

        private bool IsPossibleNamespaceMemberDeclaration()
        {
            switch (base.CurrentToken.Kind)
            {
                case SyntaxKind.ExternKeyword:
                case SyntaxKind.NamespaceKeyword:
                case SyntaxKind.UsingKeyword:
                    return true;
                case SyntaxKind.IdentifierToken:
                    return IsPartialInNamespaceMemberDeclaration();
                default:
                    return IsPossibleStartOfTypeDeclaration(base.CurrentToken.Kind);
            }
        }

        private bool IsPartialInNamespaceMemberDeclaration()
        {
            if (base.CurrentToken.ContextualKind == SyntaxKind.PartialKeyword)
            {
                if (IsPartialType())
                {
                    return true;
                }
                if (PeekToken(1).Kind == SyntaxKind.NamespaceKeyword)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsEndOfNamespace()
        {
            return base.CurrentToken.Kind == SyntaxKind.CloseBraceToken;
        }

        public bool IsGobalAttributesTerminator()
        {
            if (!IsEndOfNamespace())
            {
                return IsPossibleNamespaceMemberDeclaration();
            }
            return true;
        }

        private bool IsNamespaceMemberStartOrStop()
        {
            if (!IsEndOfNamespace())
            {
                return IsPossibleNamespaceMemberDeclaration();
            }
            return true;
        }

        private bool ScanExternAliasDirective()
        {
            if (base.CurrentToken.Kind == SyntaxKind.ExternKeyword && PeekToken(1).Kind == SyntaxKind.IdentifierToken && PeekToken(1).ContextualKind == SyntaxKind.AliasKeyword && PeekToken(2).Kind == SyntaxKind.IdentifierToken)
            {
                return PeekToken(3).Kind == SyntaxKind.SemicolonToken;
            }
            return false;
        }

        private ExternAliasDirectiveSyntax ParseExternAliasDirective()
        {
            if (IsIncrementalAndFactoryContextMatches && base.CurrentNodeKind == SyntaxKind.ExternAliasDirective)
            {
                return (ExternAliasDirectiveSyntax)EatNode();
            }
            SyntaxToken node = EatToken(SyntaxKind.ExternKeyword);
            SyntaxToken aliasKeyword = EatContextualToken(SyntaxKind.AliasKeyword);
            node = CheckFeatureAvailability(node, MessageID.IDS_FeatureExternAlias);
            SyntaxToken identifier = ParseIdentifierToken();
            SyntaxToken semicolonToken = EatToken(SyntaxKind.SemicolonToken);
            return _syntaxFactory.ExternAliasDirective(node, aliasKeyword, identifier, semicolonToken);
        }

        private NameEqualsSyntax ParseNameEquals()
        {
            return _syntaxFactory.NameEquals(_syntaxFactory.IdentifierName(ParseIdentifierToken()), EatToken(SyntaxKind.EqualsToken));
        }

        private UsingDirectiveSyntax ParseUsingDirective()
        {
            if (IsIncrementalAndFactoryContextMatches && base.CurrentNodeKind == SyntaxKind.UsingDirective)
            {
                return (UsingDirectiveSyntax)EatNode();
            }
            SyntaxToken syntaxToken = null;
            if (base.CurrentToken.ContextualKind == SyntaxKind.GlobalKeyword)
            {
                syntaxToken = SyntaxParser.ConvertToKeyword(EatToken());
            }
            SyntaxToken usingKeyword = EatToken(SyntaxKind.UsingKeyword);
            SyntaxToken syntaxToken2 = TryEatToken(SyntaxKind.StaticKeyword);
            NameEqualsSyntax alias = (IsNamedAssignment() ? ParseNameEquals() : null);
            NameSyntax nameSyntax;
            SyntaxToken semicolonToken;
            if (IsPossibleNamespaceMemberDeclaration())
            {
                nameSyntax = WithAdditionalDiagnostics(CreateMissingIdentifierName(), GetExpectedTokenError(SyntaxKind.IdentifierToken, base.CurrentToken.Kind));
                semicolonToken = SyntaxFactory.MissingToken(SyntaxKind.SemicolonToken);
            }
            else
            {
                nameSyntax = ParseQualifiedName();
                if (nameSyntax.IsMissing && PeekToken(1).Kind == SyntaxKind.SemicolonToken)
                {
                    nameSyntax = AddTrailingSkippedSyntax(nameSyntax, EatToken());
                }
                semicolonToken = EatToken(SyntaxKind.SemicolonToken);
            }
            UsingDirectiveSyntax usingDirectiveSyntax = _syntaxFactory.UsingDirective(syntaxToken, usingKeyword, syntaxToken2, alias, nameSyntax, semicolonToken);
            if (syntaxToken2 != null)
            {
                usingDirectiveSyntax = CheckFeatureAvailability(usingDirectiveSyntax, MessageID.IDS_FeatureUsingStatic);
            }
            if (syntaxToken != null)
            {
                usingDirectiveSyntax = CheckFeatureAvailability(usingDirectiveSyntax, MessageID.IDS_FeatureGlobalUsing);
            }
            return usingDirectiveSyntax;
        }

        private bool IsPossibleGlobalAttributeDeclaration()
        {
            if (base.CurrentToken.Kind == SyntaxKind.OpenBracketToken && IsGlobalAttributeTarget(PeekToken(1)))
            {
                return PeekToken(2).Kind == SyntaxKind.ColonToken;
            }
            return false;
        }

        private static bool IsGlobalAttributeTarget(SyntaxToken token)
        {
            AttributeLocation attributeLocation = token.ToAttributeLocation();
            if ((uint)(attributeLocation - 1) <= 1u)
            {
                return true;
            }
            return false;
        }

        private bool IsPossibleAttributeDeclaration()
        {
            return base.CurrentToken.Kind == SyntaxKind.OpenBracketToken;
        }

        private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> ParseAttributeDeclarations()
        {
            SyntaxListBuilder<AttributeListSyntax> syntaxListBuilder = _pool.Allocate<AttributeListSyntax>();
            try
            {
                TerminatorState termState = _termState;
                _termState |= TerminatorState.IsAttributeDeclarationTerminator;
                while (IsPossibleAttributeDeclaration())
                {
                    AttributeListSyntax node = ParseAttributeDeclaration();
                    syntaxListBuilder.Add(node);
                }
                _termState = termState;
                return syntaxListBuilder.ToList();
            }
            finally
            {
                _pool.Free(syntaxListBuilder);
            }
        }

        private bool IsAttributeDeclarationTerminator()
        {
            if (base.CurrentToken.Kind != SyntaxKind.CloseBracketToken)
            {
                return IsPossibleAttributeDeclaration();
            }
            return true;
        }

        private AttributeListSyntax ParseAttributeDeclaration()
        {
            if (IsIncrementalAndFactoryContextMatches && base.CurrentNodeKind == SyntaxKind.AttributeList)
            {
                return (AttributeListSyntax)EatNode();
            }
            SyntaxToken openBracketToken = EatToken(SyntaxKind.OpenBracketToken);
            AttributeTargetSpecifierSyntax attributeTargetSpecifierSyntax = null;
            if (IsSomeWord(base.CurrentToken.Kind) && PeekToken(1).Kind == SyntaxKind.ColonToken)
            {
                SyntaxToken identifier = SyntaxParser.ConvertToKeyword(EatToken());
                SyntaxToken colonToken = EatToken(SyntaxKind.ColonToken);
                attributeTargetSpecifierSyntax = _syntaxFactory.AttributeTargetSpecifier(identifier, colonToken);
            }
            SeparatedSyntaxListBuilder<AttributeSyntax> builder = _pool.AllocateSeparated<AttributeSyntax>();
            try
            {
                if (attributeTargetSpecifierSyntax != null && attributeTargetSpecifierSyntax.Identifier.ToAttributeLocation() == AttributeLocation.Module)
                {
                    attributeTargetSpecifierSyntax = CheckFeatureAvailability(attributeTargetSpecifierSyntax, MessageID.IDS_FeatureModuleAttrLoc);
                }
                ParseAttributes(builder);
                SyntaxToken closeBracketToken = EatToken(SyntaxKind.CloseBracketToken);
                return _syntaxFactory.AttributeList(openBracketToken, attributeTargetSpecifierSyntax, builder, closeBracketToken);
            }
            finally
            {
                _pool.Free(in builder);
            }
        }

        private void ParseAttributes(SeparatedSyntaxListBuilder<AttributeSyntax> nodes)
        {
            nodes.Add(ParseAttribute());
            while (base.CurrentToken.Kind != SyntaxKind.CloseBracketToken)
            {
                if (base.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    nodes.AddSeparator(EatToken());
                    if (base.CurrentToken.Kind == SyntaxKind.CloseBracketToken)
                    {
                        break;
                    }
                    nodes.Add(ParseAttribute());
                }
                else if (IsPossibleAttribute())
                {
                    nodes.AddSeparator(EatToken(SyntaxKind.CommaToken));
                    nodes.Add(ParseAttribute());
                }
                else if (SkipBadAttributeListTokens(nodes, SyntaxKind.IdentifierToken) == PostSkipAction.Abort)
                {
                    break;
                }
            }
        }

        private PostSkipAction SkipBadAttributeListTokens(SeparatedSyntaxListBuilder<AttributeSyntax> list, SyntaxKind expected)
        {
            SyntaxToken startToken = null;
            return SkipBadSeparatedListTokensWithExpectedKind(ref startToken, list, (LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleAttribute(), (LanguageParser p) => p.CurrentToken.Kind == SyntaxKind.CloseBracketToken || p.IsTerminator(), expected);
        }

        private bool IsPossibleAttribute()
        {
            return IsTrueIdentifier();
        }

        private AttributeSyntax ParseAttribute()
        {
            if (IsIncrementalAndFactoryContextMatches && base.CurrentNodeKind == SyntaxKind.Attribute)
            {
                return (AttributeSyntax)EatNode();
            }
            NameSyntax name = ParseQualifiedName();
            AttributeArgumentListSyntax argumentList = ParseAttributeArgumentList();
            return _syntaxFactory.Attribute(name, argumentList);
        }

        internal AttributeArgumentListSyntax ParseAttributeArgumentList()
        {
            if (IsIncrementalAndFactoryContextMatches && base.CurrentNodeKind == SyntaxKind.AttributeArgumentList)
            {
                return (AttributeArgumentListSyntax)EatNode();
            }
            AttributeArgumentListSyntax result = null;
            if (base.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                SyntaxToken openParen = EatToken(SyntaxKind.OpenParenToken);
                SeparatedSyntaxListBuilder<AttributeArgumentSyntax> builder = _pool.AllocateSeparated<AttributeArgumentSyntax>();
                try
                {
                    while (base.CurrentToken.Kind != SyntaxKind.CloseParenToken)
                    {
                        if (IsPossibleAttributeArgument() || base.CurrentToken.Kind == SyntaxKind.CommaToken)
                        {
                            builder.Add(ParseAttributeArgument());
                            int lastTokenPosition = -1;
                            while (IsMakingProgress(ref lastTokenPosition) && base.CurrentToken.Kind != SyntaxKind.CloseParenToken)
                            {
                                if (base.CurrentToken.Kind == SyntaxKind.CommaToken || IsPossibleAttributeArgument())
                                {
                                    builder.AddSeparator(EatToken(SyntaxKind.CommaToken));
                                    builder.Add(ParseAttributeArgument());
                                }
                                else if (SkipBadAttributeArgumentTokens(ref openParen, builder, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                                {
                                    break;
                                }
                            }
                            break;
                        }
                        if (SkipBadAttributeArgumentTokens(ref openParen, builder, SyntaxKind.IdentifierToken) != 0)
                        {
                            break;
                        }
                    }
                    SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
                    return _syntaxFactory.AttributeArgumentList(openParen, builder, closeParenToken);
                }
                finally
                {
                    _pool.Free(in builder);
                }
            }
            return result;
        }

        private PostSkipAction SkipBadAttributeArgumentTokens(ref SyntaxToken openParen, SeparatedSyntaxListBuilder<AttributeArgumentSyntax> list, SyntaxKind expected)
        {
            return SkipBadSeparatedListTokensWithExpectedKind(ref openParen, list, (LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleAttributeArgument(), (LanguageParser p) => p.CurrentToken.Kind == SyntaxKind.CloseParenToken || p.IsTerminator(), expected);
        }

        private bool IsPossibleAttributeArgument()
        {
            return IsPossibleExpression();
        }

        private AttributeArgumentSyntax ParseAttributeArgument()
        {
            NameEqualsSyntax nameEquals = null;
            NameColonSyntax nameColon = null;
            if (base.CurrentToken.Kind == SyntaxKind.IdentifierToken)
            {
                switch (PeekToken(1).Kind)
                {
                    case SyntaxKind.EqualsToken:
                        {
                            SyntaxToken identifier = ParseIdentifierToken();
                            SyntaxToken equalsToken = EatToken(SyntaxKind.EqualsToken);
                            nameEquals = _syntaxFactory.NameEquals(_syntaxFactory.IdentifierName(identifier), equalsToken);
                            break;
                        }
                    case SyntaxKind.ColonToken:
                        {
                            IdentifierNameSyntax name = ParseIdentifierName();
                            SyntaxToken colonToken = EatToken(SyntaxKind.ColonToken);
                            nameColon = _syntaxFactory.NameColon(name, colonToken);
                            nameColon = CheckFeatureAvailability(nameColon, MessageID.IDS_FeatureNamedArgument);
                            break;
                        }
                }
            }
            return _syntaxFactory.AttributeArgument(nameEquals, nameColon, ParseExpressionCore());
        }

        private static DeclarationModifiers GetModifier(SyntaxToken token)
        {
            return GetModifier(token.Kind, token.ContextualKind);
        }

        internal static DeclarationModifiers GetModifier(SyntaxKind kind, SyntaxKind contextualKind)
        {
            switch (kind)
            {
                case SyntaxKind.PublicKeyword:
                    return DeclarationModifiers.Public;
                case SyntaxKind.InternalKeyword:
                    return DeclarationModifiers.Internal;
                case SyntaxKind.ProtectedKeyword:
                    return DeclarationModifiers.Protected;
                case SyntaxKind.PrivateKeyword:
                    return DeclarationModifiers.Private;
                case SyntaxKind.SealedKeyword:
                    return DeclarationModifiers.Sealed;
                case SyntaxKind.AbstractKeyword:
                    return DeclarationModifiers.Abstract;
                case SyntaxKind.StaticKeyword:
                    return DeclarationModifiers.Static;
                case SyntaxKind.VirtualKeyword:
                    return DeclarationModifiers.Virtual;
                case SyntaxKind.ExternKeyword:
                    return DeclarationModifiers.Extern;
                case SyntaxKind.NewKeyword:
                    return DeclarationModifiers.New;
                case SyntaxKind.OverrideKeyword:
                    return DeclarationModifiers.Override;
                case SyntaxKind.ReadOnlyKeyword:
                    return DeclarationModifiers.ReadOnly;
                case SyntaxKind.VolatileKeyword:
                    return DeclarationModifiers.Volatile;
                case SyntaxKind.UnsafeKeyword:
                    return DeclarationModifiers.Unsafe;
                case SyntaxKind.PartialKeyword:
                    return DeclarationModifiers.Partial;
                case SyntaxKind.AsyncKeyword:
                    return DeclarationModifiers.Async;
                case SyntaxKind.RefKeyword:
                    return DeclarationModifiers.Ref;
                case SyntaxKind.DataKeyword:
                    return DeclarationModifiers.Data;
                case SyntaxKind.IdentifierToken:
                    switch (contextualKind)
                    {
                        case SyntaxKind.PartialKeyword:
                            return DeclarationModifiers.Partial;
                        case SyntaxKind.AsyncKeyword:
                            return DeclarationModifiers.Async;
                        case SyntaxKind.DataKeyword:
                            return DeclarationModifiers.Data;
                    }
                    break;
            }
            return DeclarationModifiers.None;
        }

        private void ParseModifiers(SyntaxListBuilder tokens, bool forAccessors)
        {
            while (true)
            {
                SyntaxToken item;
                switch (GetModifier(base.CurrentToken))
                {
                    case DeclarationModifiers.Partial:
                        {
                            SyntaxToken syntaxToken = PeekToken(1);
                            bool flag = IsPartialType();
                            bool flag2 = IsPartialMember();
                            if (flag || flag2)
                            {
                                item = SyntaxParser.ConvertToKeyword(EatToken());
                                item = CheckFeatureAvailability(item, flag ? MessageID.IDS_FeaturePartialTypes : MessageID.IDS_FeaturePartialMethod);
                                break;
                            }
                            if (syntaxToken.Kind == SyntaxKind.NamespaceKeyword)
                            {
                                item = SyntaxParser.ConvertToKeyword(EatToken());
                                break;
                            }
                            if (syntaxToken.Kind == SyntaxKind.EnumKeyword || syntaxToken.Kind == SyntaxKind.DelegateKeyword || (IsPossibleStartOfTypeDeclaration(syntaxToken.Kind) && GetModifier(syntaxToken) != 0))
                            {
                                item = AddError(SyntaxParser.ConvertToKeyword(EatToken()), ErrorCode.ERR_PartialMisplaced);
                                break;
                            }
                            return;
                        }
                    case DeclarationModifiers.Ref:
                        {
                            SyntaxToken syntaxToken2 = PeekToken(1);
                            if (isStructOrRecordKeyword(syntaxToken2) || (syntaxToken2.ContextualKind == SyntaxKind.PartialKeyword && isStructOrRecordKeyword(PeekToken(2))))
                            {
                                item = EatToken();
                                item = CheckFeatureAvailability(item, MessageID.IDS_FeatureRefStructs);
                                break;
                            }
                            if (forAccessors && IsPossibleAccessorModifier())
                            {
                                item = EatToken();
                                break;
                            }
                            return;
                        }
                    case DeclarationModifiers.Async:
                        if (!ShouldAsyncBeTreatedAsModifier(parsingStatementNotDeclaration: false))
                        {
                            return;
                        }
                        item = SyntaxParser.ConvertToKeyword(EatToken());
                        item = CheckFeatureAvailability(item, MessageID.IDS_FeatureAsync);
                        break;
                    case DeclarationModifiers.None:
                    case DeclarationModifiers.Data:
                        return;
                    default:
                        item = EatToken();
                        break;
                }
                tokens.Add(item);
            }
            bool isStructOrRecordKeyword(SyntaxToken token)
            {
                if (token.Kind == SyntaxKind.StructKeyword)
                {
                    return true;
                }
                if (token.ContextualKind == SyntaxKind.RecordKeyword)
                {
                    return IsFeatureEnabled(MessageID.IDS_FeatureRecords);
                }
                return false;
            }
        }

        private bool ShouldAsyncBeTreatedAsModifier(bool parsingStatementNotDeclaration)
        {
            if (IsNonContextualModifier(PeekToken(1)))
            {
                return true;
            }
            ResetPoint state = GetResetPoint();
            try
            {
                EatToken();
                if (!parsingStatementNotDeclaration && base.CurrentToken.ContextualKind == SyntaxKind.PartialKeyword)
                {
                    EatToken();
                }
                if (!parsingStatementNotDeclaration)
                {
                    SyntaxKind kind = base.CurrentToken.Kind;
                    if (IsPossibleStartOfTypeDeclaration(kind) || kind == SyntaxKind.EventKeyword || ((kind == SyntaxKind.ExplicitKeyword || kind == SyntaxKind.ImplicitKeyword) && PeekToken(1).Kind == SyntaxKind.OperatorKeyword))
                    {
                        return true;
                    }
                }
                if (ScanType() != 0)
                {
                    if (IsPossibleMemberName())
                    {
                        return true;
                    }
                    SyntaxKind kind2 = base.CurrentToken.Kind;
                    switch (kind2)
                    {
                        case SyntaxKind.EndOfFileToken:
                            return true;
                        case SyntaxKind.CloseBraceToken:
                            return true;
                        default:
                            if (SyntaxFacts.IsPredefinedType(base.CurrentToken.Kind))
                            {
                                return true;
                            }
                            if (IsNonContextualModifier(base.CurrentToken))
                            {
                                return true;
                            }
                            if (IsTypeDeclarationStart())
                            {
                                return true;
                            }
                            if (kind2 == SyntaxKind.NamespaceKeyword)
                            {
                                return true;
                            }
                            if (!parsingStatementNotDeclaration && kind2 == SyntaxKind.OperatorKeyword)
                            {
                                return true;
                            }
                            break;
                    }
                }
            }
            finally
            {
                Reset(ref state);
                Release(ref state);
            }
            return false;
        }

        private static bool IsNonContextualModifier(SyntaxToken nextToken)
        {
            if (GetModifier(nextToken) != 0)
            {
                return !SyntaxFacts.IsContextualKeyword(nextToken.ContextualKind);
            }
            return false;
        }

        private bool IsPartialType()
        {
            SyntaxToken syntaxToken = PeekToken(1);
            SyntaxKind kind = syntaxToken.Kind;
            if (kind - 8374 <= (SyntaxKind)2)
            {
                return true;
            }
            if (syntaxToken.ContextualKind == SyntaxKind.RecordKeyword)
            {
                return IsFeatureEnabled(MessageID.IDS_FeatureRecords);
            }
            return false;
        }

        private bool IsPartialMember()
        {
            ResetPoint state = GetResetPoint();
            try
            {
                EatToken();
                if (ScanType() == ScanTypeFlags.NotType)
                {
                    return false;
                }
                return IsPossibleMemberName();
            }
            finally
            {
                Reset(ref state);
                Release(ref state);
            }
        }

        private bool IsPossibleMemberName()
        {
            switch (base.CurrentToken.Kind)
            {
                case SyntaxKind.IdentifierToken:
                    if (base.CurrentToken.ContextualKind == SyntaxKind.GlobalKeyword && PeekToken(1).Kind == SyntaxKind.UsingKeyword)
                    {
                        return false;
                    }
                    return true;
                case SyntaxKind.ThisKeyword:
                    return true;
                default:
                    return false;
            }
        }

        private MemberDeclarationSyntax ParseTypeDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers)
        {
            CancellationToken cancellationToken = base.cancellationToken;
            cancellationToken.ThrowIfCancellationRequested();
            switch (base.CurrentToken.Kind)
            {
                case SyntaxKind.ClassKeyword:
                    CheckForVersionSpecificModifiers(modifiers, SyntaxKind.StaticKeyword, MessageID.IDS_FeatureStaticClasses);
                    return ParseClassOrStructOrInterfaceDeclaration(attributes, modifiers);
                case SyntaxKind.StructKeyword:
                    CheckForVersionSpecificModifiers(modifiers, SyntaxKind.ReadOnlyKeyword, MessageID.IDS_FeatureReadOnlyStructs);
                    return ParseClassOrStructOrInterfaceDeclaration(attributes, modifiers);
                case SyntaxKind.InterfaceKeyword:
                    return ParseClassOrStructOrInterfaceDeclaration(attributes, modifiers);
                case SyntaxKind.DelegateKeyword:
                    return ParseDelegateDeclaration(attributes, modifiers);
                case SyntaxKind.EnumKeyword:
                    return ParseEnumDeclaration(attributes, modifiers);
                case SyntaxKind.IdentifierToken:
                    return ParseClassOrStructOrInterfaceDeclaration(attributes, modifiers);
                default:
                    throw ExceptionUtilities.UnexpectedValue(base.CurrentToken.Kind);
            }
        }

        private void CheckForVersionSpecificModifiers(SyntaxListBuilder modifiers, SyntaxKind kind, MessageID feature)
        {
            int i = 0;
            for (int count = modifiers.Count; i < count; i++)
            {
                if (modifiers[i]!.RawKind == (int)kind)
                {
                    modifiers[i] = CheckFeatureAvailability(modifiers[i], feature);
                }
            }
        }

        private TypeDeclarationSyntax ParseClassOrStructOrInterfaceDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers)
        {
            SyntaxToken syntaxToken = SyntaxParser.ConvertToKeyword(EatToken());
            TerminatorState termState = _termState;
            SyntaxToken recordModifier2 = null;
            if (syntaxToken.Kind == SyntaxKind.RecordKeyword)
            {
                _termState |= TerminatorState.IsEndOfRecordSignature;
                recordModifier2 = eatRecordModifierIfAvailable();
            }
            TerminatorState termState2 = _termState;
            _termState |= TerminatorState.IsPossibleAggregateClauseStartOrStop;
            SyntaxToken syntaxToken2 = ParseIdentifierToken();
            TypeParameterListSyntax typeParameters2 = ParseTypeParameterList();
            ParameterListSyntax paramList2 = ((syntaxToken.Kind == SyntaxKind.RecordKeyword && base.CurrentToken.Kind == SyntaxKind.OpenParenToken) ? ParseParenthesizedParameterList() : null);
            BaseListSyntax baseList2 = ParseBaseList();
            _termState = termState2;
            bool flag = true;
            SyntaxListBuilder<MemberDeclarationSyntax> syntaxListBuilder = default(SyntaxListBuilder<MemberDeclarationSyntax>);
            SyntaxListBuilder<TypeParameterConstraintClauseSyntax> syntaxListBuilder2 = default(SyntaxListBuilder<TypeParameterConstraintClauseSyntax>);
            try
            {
                if (base.CurrentToken.ContextualKind == SyntaxKind.WhereKeyword)
                {
                    syntaxListBuilder2 = _pool.Allocate<TypeParameterConstraintClauseSyntax>();
                    ParseTypeParameterConstraintClauses(syntaxListBuilder2);
                }
                _termState = termState;
                SyntaxToken openBrace2;
                SyntaxToken node;
                SyntaxToken semicolon2;
                if (syntaxToken.Kind != SyntaxKind.RecordKeyword || base.CurrentToken.Kind != SyntaxKind.SemicolonToken)
                {
                    openBrace2 = EatToken(SyntaxKind.OpenBraceToken);
                    if (syntaxToken2.IsMissing || openBrace2.IsMissing)
                    {
                        flag = false;
                    }
                    if (flag)
                    {
                        syntaxListBuilder = _pool.Allocate<MemberDeclarationSyntax>();
                        while (true)
                        {
                            SyntaxKind kind = base.CurrentToken.Kind;
                            if (CanStartMember(kind))
                            {
                                TerminatorState termState3 = _termState;
                                _termState |= TerminatorState.IsPossibleMemberStartOrStop;
                                MemberDeclarationSyntax memberDeclarationSyntax = ParseMemberDeclaration(syntaxToken.Kind);
                                if (memberDeclarationSyntax != null)
                                {
                                    syntaxListBuilder.Add(memberDeclarationSyntax);
                                }
                                else
                                {
                                    SkipBadMemberListTokens(ref openBrace2, syntaxListBuilder);
                                }
                                _termState = termState3;
                            }
                            else
                            {
                                if (kind == SyntaxKind.CloseBraceToken || kind == SyntaxKind.EndOfFileToken || IsTerminator())
                                {
                                    break;
                                }
                                SkipBadMemberListTokens(ref openBrace2, syntaxListBuilder);
                            }
                        }
                    }
                    if (openBrace2.IsMissing)
                    {
                        node = SyntaxFactory.MissingToken(SyntaxKind.CloseBraceToken);
                        node = WithAdditionalDiagnostics(node, GetExpectedTokenError(SyntaxKind.CloseBraceToken, base.CurrentToken.Kind));
                    }
                    else
                    {
                        node = EatToken(SyntaxKind.CloseBraceToken);
                    }
                    semicolon2 = TryEatToken(SyntaxKind.SemicolonToken);
                }
                else
                {
                    semicolon2 = CheckFeatureAvailability(EatToken(SyntaxKind.SemicolonToken), MessageID.IDS_FeatureRecords);
                    openBrace2 = null;
                    node = null;
                }
                return constructTypeDeclaration(_syntaxFactory, attributes, modifiers, syntaxToken, recordModifier2, syntaxToken2, typeParameters2, paramList2, baseList2, syntaxListBuilder2, openBrace2, syntaxListBuilder, node, semicolon2);
            }
            finally
            {
                if (!syntaxListBuilder.IsNull)
                {
                    _pool.Free(syntaxListBuilder);
                }
                if (!syntaxListBuilder2.IsNull)
                {
                    _pool.Free(syntaxListBuilder2);
                }
            }
            static TypeDeclarationSyntax constructTypeDeclaration(ContextAwareSyntax syntaxFactory, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers, SyntaxToken keyword, SyntaxToken? recordModifier, SyntaxToken name, TypeParameterListSyntax typeParameters, ParameterListSyntax? paramList, BaseListSyntax baseList, SyntaxListBuilder<TypeParameterConstraintClauseSyntax> constraints, SyntaxToken? openBrace, SyntaxListBuilder<MemberDeclarationSyntax> members, SyntaxToken? closeBrace, SyntaxToken semicolon)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> modifiers2 = modifiers.ToList();
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<MemberDeclarationSyntax> members2 = members;
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses = constraints;
                switch (keyword.Kind)
                {
                    case SyntaxKind.ClassKeyword:
                        return syntaxFactory.ClassDeclaration(attributes, modifiers2, keyword, name, typeParameters, baseList, constraintClauses, openBrace, members2, closeBrace, semicolon);
                    case SyntaxKind.StructKeyword:
                        return syntaxFactory.StructDeclaration(attributes, modifiers2, keyword, name, typeParameters, baseList, constraintClauses, openBrace, members2, closeBrace, semicolon);
                    case SyntaxKind.InterfaceKeyword:
                        return syntaxFactory.InterfaceDeclaration(attributes, modifiers2, keyword, name, typeParameters, baseList, constraintClauses, openBrace, members2, closeBrace, semicolon);
                    case SyntaxKind.RecordKeyword:
                        {
                            SyntaxKind kind2 = ((recordModifier != null && recordModifier!.Kind == SyntaxKind.StructKeyword) ? SyntaxKind.RecordStructDeclaration : SyntaxKind.RecordDeclaration);
                            return syntaxFactory.RecordDeclaration(kind2, attributes, modifiers.ToList(), keyword, recordModifier, name, typeParameters, paramList, baseList, constraints, openBrace, members, closeBrace, semicolon);
                        }
                    default:
                        throw ExceptionUtilities.UnexpectedValue(keyword.Kind);
                }
            }
            SyntaxToken? eatRecordModifierIfAvailable()
            {
                SyntaxKind kind3 = base.CurrentToken.Kind;
                if (kind3 == SyntaxKind.ClassKeyword || kind3 == SyntaxKind.StructKeyword)
                {
                    SyntaxToken node2 = EatToken();
                    return CheckFeatureAvailability(node2, MessageID.IDS_FeatureRecordStructs);
                }
                return null;
            }
        }

        private void SkipBadMemberListTokens(ref SyntaxToken openBrace, SyntaxListBuilder members)
        {
            if (members.Count > 0)
            {
                GreenNode previousNode = members[members.Count - 1];
                SkipBadMemberListTokens(ref previousNode);
                members[members.Count - 1] = previousNode;
            }
            else
            {
                GreenNode previousNode2 = openBrace;
                SkipBadMemberListTokens(ref previousNode2);
                openBrace = (SyntaxToken)previousNode2;
            }
        }

        private void SkipBadMemberListTokens(ref GreenNode previousNode)
        {
            int num = 0;
            SyntaxListBuilder syntaxListBuilder = _pool.Allocate();
            try
            {
                bool flag = false;
                SyntaxToken syntaxToken = EatToken();
                syntaxToken = AddError(syntaxToken, ErrorCode.ERR_InvalidMemberDecl, syntaxToken.Text);
                syntaxListBuilder.Add(syntaxToken);
                while (!flag)
                {
                    SyntaxKind kind = base.CurrentToken.Kind;
                    if (CanStartMember(kind) && (kind != SyntaxKind.DelegateKeyword || (PeekToken(1).Kind != SyntaxKind.OpenBraceToken && PeekToken(1).Kind != SyntaxKind.OpenParenToken)))
                    {
                        flag = true;
                        continue;
                    }
                    switch (kind)
                    {
                        case SyntaxKind.OpenBraceToken:
                            num++;
                            break;
                        case SyntaxKind.CloseBraceToken:
                            if (num-- == 0)
                            {
                                flag = true;
                                continue;
                            }
                            break;
                        case SyntaxKind.EndOfFileToken:
                            flag = true;
                            continue;
                    }
                    syntaxListBuilder.Add(EatToken());
                }
                previousNode = AddTrailingSkippedSyntax((CSharpSyntaxNode)previousNode, syntaxListBuilder.ToListNode());
            }
            finally
            {
                _pool.Free(syntaxListBuilder);
            }
        }

        private bool IsPossibleMemberStartOrStop()
        {
            if (!IsPossibleMemberStart())
            {
                return base.CurrentToken.Kind == SyntaxKind.CloseBraceToken;
            }
            return true;
        }

        private bool IsPossibleAggregateClauseStartOrStop()
        {
            if (base.CurrentToken.Kind != SyntaxKind.ColonToken && base.CurrentToken.Kind != SyntaxKind.OpenBraceToken)
            {
                return IsCurrentTokenWhereOfConstraintClause();
            }
            return true;
        }

        private BaseListSyntax ParseBaseList()
        {
            if (base.CurrentToken.Kind != SyntaxKind.ColonToken)
            {
                return null;
            }
            SyntaxToken colon = EatToken();
            SeparatedSyntaxListBuilder<BaseTypeSyntax> item = _pool.AllocateSeparated<BaseTypeSyntax>();
            try
            {
                TypeSyntax type = ParseType();
                ArgumentListSyntax argumentListSyntax = null;
                if (base.CurrentToken.Kind == SyntaxKind.OpenParenToken)
                {
                    argumentListSyntax = ParseParenthesizedArgumentList();
                }
                item.Add((argumentListSyntax != null) ? _syntaxFactory.PrimaryConstructorBaseType(type, argumentListSyntax) : _syntaxFactory.SimpleBaseType(type));
                while (base.CurrentToken.Kind != SyntaxKind.OpenBraceToken && ((_termState & TerminatorState.IsEndOfRecordSignature) == 0 || base.CurrentToken.Kind != SyntaxKind.SemicolonToken) && !IsCurrentTokenWhereOfConstraintClause())
                {
                    if (base.CurrentToken.Kind == SyntaxKind.CommaToken || IsPossibleType())
                    {
                        item.AddSeparator(EatToken(SyntaxKind.CommaToken));
                        item.Add(_syntaxFactory.SimpleBaseType(ParseType()));
                    }
                    else if (SkipBadBaseListTokens(ref colon, item, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                    {
                        break;
                    }
                }
                return _syntaxFactory.BaseList(colon, item);
            }
            finally
            {
                _pool.Free(in item);
            }
        }

        private PostSkipAction SkipBadBaseListTokens(ref SyntaxToken colon, SeparatedSyntaxListBuilder<BaseTypeSyntax> list, SyntaxKind expected)
        {
            return SkipBadSeparatedListTokensWithExpectedKind(ref colon, list, (LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleAttribute(), (LanguageParser p) => p.CurrentToken.Kind == SyntaxKind.OpenBraceToken || p.IsCurrentTokenWhereOfConstraintClause() || p.IsTerminator(), expected);
        }

        private bool IsCurrentTokenWhereOfConstraintClause()
        {
            if (base.CurrentToken.ContextualKind == SyntaxKind.WhereKeyword && PeekToken(1).Kind == SyntaxKind.IdentifierToken)
            {
                return PeekToken(2).Kind == SyntaxKind.ColonToken;
            }
            return false;
        }

        private void ParseTypeParameterConstraintClauses(SyntaxListBuilder list)
        {
            while (base.CurrentToken.ContextualKind == SyntaxKind.WhereKeyword)
            {
                list.Add(ParseTypeParameterConstraintClause());
            }
        }

        private TypeParameterConstraintClauseSyntax ParseTypeParameterConstraintClause()
        {
            SyntaxToken whereKeyword = EatContextualToken(SyntaxKind.WhereKeyword);
            IdentifierNameSyntax name = ((!IsTrueIdentifier()) ? AddError(CreateMissingIdentifierName(), ErrorCode.ERR_IdentifierExpected) : ParseIdentifierName());
            SyntaxToken colonToken = EatToken(SyntaxKind.ColonToken);
            SeparatedSyntaxListBuilder<TypeParameterConstraintSyntax> builder = _pool.AllocateSeparated<TypeParameterConstraintSyntax>();
            try
            {
                if (base.CurrentToken.Kind == SyntaxKind.OpenBraceToken || IsCurrentTokenWhereOfConstraintClause())
                {
                    builder.Add(_syntaxFactory.TypeConstraint(AddError(CreateMissingIdentifierName(), ErrorCode.ERR_TypeExpected)));
                }
                else
                {
                    builder.Add(ParseTypeParameterConstraint());
                    while (base.CurrentToken.Kind != SyntaxKind.OpenBraceToken && ((_termState & TerminatorState.IsEndOfRecordSignature) == 0 || base.CurrentToken.Kind != SyntaxKind.SemicolonToken) && base.CurrentToken.Kind != SyntaxKind.EqualsGreaterThanToken && base.CurrentToken.ContextualKind != SyntaxKind.WhereKeyword)
                    {
                        if (base.CurrentToken.Kind == SyntaxKind.CommaToken || IsPossibleTypeParameterConstraint())
                        {
                            builder.AddSeparator(EatToken(SyntaxKind.CommaToken));
                            if (IsCurrentTokenWhereOfConstraintClause())
                            {
                                builder.Add(_syntaxFactory.TypeConstraint(AddError(CreateMissingIdentifierName(), ErrorCode.ERR_TypeExpected)));
                                break;
                            }
                            builder.Add(ParseTypeParameterConstraint());
                        }
                        else if (SkipBadTypeParameterConstraintTokens(builder, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                        {
                            break;
                        }
                    }
                }
                return _syntaxFactory.TypeParameterConstraintClause(whereKeyword, name, colonToken, builder);
            }
            finally
            {
                _pool.Free(in builder);
            }
        }

        private bool IsPossibleTypeParameterConstraint()
        {
            switch (base.CurrentToken.Kind)
            {
                case SyntaxKind.DefaultKeyword:
                case SyntaxKind.NewKeyword:
                case SyntaxKind.ClassKeyword:
                case SyntaxKind.StructKeyword:
                    return true;
                case SyntaxKind.IdentifierToken:
                    return IsTrueIdentifier();
                default:
                    return IsPredefinedType(base.CurrentToken.Kind);
            }
        }

        private TypeParameterConstraintSyntax ParseTypeParameterConstraint()
        {
            SyntaxToken questionToken = null;
            _ = base.CurrentToken.Kind;
            switch (base.CurrentToken.Kind)
            {
                case SyntaxKind.NewKeyword:
                    {
                        SyntaxToken newKeyword = EatToken();
                        SyntaxToken openParenToken = EatToken(SyntaxKind.OpenParenToken);
                        SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
                        return _syntaxFactory.ConstructorConstraint(newKeyword, openParenToken, closeParenToken);
                    }
                case SyntaxKind.StructKeyword:
                    {
                        SyntaxToken classOrStructKeyword2 = EatToken();
                        if (base.CurrentToken.Kind == SyntaxKind.QuestionToken)
                        {
                            questionToken = EatToken();
                            questionToken = AddError(questionToken, ErrorCode.ERR_UnexpectedToken, questionToken.Text);
                        }
                        return _syntaxFactory.ClassOrStructConstraint(SyntaxKind.StructConstraint, classOrStructKeyword2, questionToken);
                    }
                case SyntaxKind.ClassKeyword:
                    {
                        SyntaxToken classOrStructKeyword = EatToken();
                        questionToken = TryEatToken(SyntaxKind.QuestionToken);
                        return _syntaxFactory.ClassOrStructConstraint(SyntaxKind.ClassConstraint, classOrStructKeyword, questionToken);
                    }
                case SyntaxKind.DefaultKeyword:
                    {
                        SyntaxToken defaultKeyword = EatToken();
                        return CheckFeatureAvailability(_syntaxFactory.DefaultConstraint(defaultKeyword), MessageID.IDS_FeatureDefaultTypeParameterConstraint);
                    }
                default:
                    {
                        TypeSyntax type = ParseType();
                        return _syntaxFactory.TypeConstraint(type);
                    }
            }
        }

        private PostSkipAction SkipBadTypeParameterConstraintTokens(SeparatedSyntaxListBuilder<TypeParameterConstraintSyntax> list, SyntaxKind expected)
        {
            CSharpSyntaxNode startToken = null;
            return SkipBadSeparatedListTokensWithExpectedKind(ref startToken, list, (LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleTypeParameterConstraint(), (LanguageParser p) => p.CurrentToken.Kind == SyntaxKind.OpenBraceToken || p.IsCurrentTokenWhereOfConstraintClause() || p.IsTerminator(), expected);
        }

        private bool IsPossibleMemberStart()
        {
            return CanStartMember(base.CurrentToken.Kind);
        }

        private static bool CanStartMember(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.TildeToken:
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.OpenBracketToken:
                case SyntaxKind.BoolKeyword:
                case SyntaxKind.ByteKeyword:
                case SyntaxKind.SByteKeyword:
                case SyntaxKind.ShortKeyword:
                case SyntaxKind.UShortKeyword:
                case SyntaxKind.IntKeyword:
                case SyntaxKind.UIntKeyword:
                case SyntaxKind.LongKeyword:
                case SyntaxKind.ULongKeyword:
                case SyntaxKind.DoubleKeyword:
                case SyntaxKind.FloatKeyword:
                case SyntaxKind.DecimalKeyword:
                case SyntaxKind.StringKeyword:
                case SyntaxKind.CharKeyword:
                case SyntaxKind.VoidKeyword:
                case SyntaxKind.ObjectKeyword:
                case SyntaxKind.PublicKeyword:
                case SyntaxKind.PrivateKeyword:
                case SyntaxKind.InternalKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.StaticKeyword:
                case SyntaxKind.ReadOnlyKeyword:
                case SyntaxKind.SealedKeyword:
                case SyntaxKind.ConstKeyword:
                case SyntaxKind.FixedKeyword:
                case SyntaxKind.VolatileKeyword:
                case SyntaxKind.NewKeyword:
                case SyntaxKind.OverrideKeyword:
                case SyntaxKind.AbstractKeyword:
                case SyntaxKind.VirtualKeyword:
                case SyntaxKind.EventKeyword:
                case SyntaxKind.ExternKeyword:
                case SyntaxKind.RefKeyword:
                case SyntaxKind.ClassKeyword:
                case SyntaxKind.StructKeyword:
                case SyntaxKind.InterfaceKeyword:
                case SyntaxKind.EnumKeyword:
                case SyntaxKind.DelegateKeyword:
                case SyntaxKind.UnsafeKeyword:
                case SyntaxKind.ExplicitKeyword:
                case SyntaxKind.ImplicitKeyword:
                case SyntaxKind.IdentifierToken:
                    return true;
                default:
                    return false;
            }
        }

        private bool IsTypeDeclarationStart()
        {
            SyntaxKind kind = base.CurrentToken.Kind;
            if (kind - 8374 > (SyntaxKind)3)
            {
                if (kind != SyntaxKind.DelegateKeyword)
                {
                    if (kind == SyntaxKind.IdentifierToken)
                    {
                        if (base.CurrentToken.ContextualKind == SyntaxKind.RecordKeyword)
                        {
                            return IsFeatureEnabled(MessageID.IDS_FeatureRecords);
                        }
                        return false;
                    }
                }
                else if (!IsFunctionPointerStart())
                {
                    goto IL_0030;
                }
                return false;
            }
            goto IL_0030;
        IL_0030:
            return true;
        }

        private bool CanReuseMemberDeclaration(SyntaxKind kind, bool isGlobal)
        {
            switch (kind)
            {
                case SyntaxKind.NamespaceDeclaration:
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.DelegateDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.DestructorDeclaration:
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.IndexerDeclaration:
                case SyntaxKind.RecordDeclaration:
                case SyntaxKind.RecordStructDeclaration:
                    return true;
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.MethodDeclaration:
                    if (!isGlobal || base.IsScript)
                    {
                        return true;
                    }
                    return base.CurrentNode.Parent is Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax;
                case SyntaxKind.GlobalStatement:
                    return isGlobal;
                default:
                    return false;
            }
        }

        public MemberDeclarationSyntax ParseMemberDeclaration()
        {
            return ParseWithStackGuard(() => ParseMemberDeclaration(SyntaxKind.StructDeclaration), () => createEmptyNodeFunc());
            MemberDeclarationSyntax createEmptyNodeFunc()
            {
                return _syntaxFactory.IncompleteMember(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>), CreateMissingIdentifierName());
            }
        }

        internal MemberDeclarationSyntax ParseMemberDeclarationOrStatement(SyntaxKind parentKind)
        {
            _recursionDepth++;
            StackGuard.EnsureSufficientExecutionStack(_recursionDepth);
            MemberDeclarationSyntax result = ParseMemberDeclarationOrStatementCore(parentKind);
            _recursionDepth--;
            return result;
        }

        private MemberDeclarationSyntax ParseMemberDeclarationOrStatementCore(SyntaxKind parentKind)
        {
            CancellationToken cancellationToken = base.cancellationToken;
            cancellationToken.ThrowIfCancellationRequested();
            if (IsIncrementalAndFactoryContextMatches && CanReuseMemberDeclaration(base.CurrentNodeKind, isGlobal: true))
            {
                return (MemberDeclarationSyntax)EatNode();
            }
            TerminatorState termState = _termState;
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = ParseAttributeDeclarations();
            bool flag = syntaxList.Count > 0;
            ResetPoint startPoint2 = GetResetPoint();
            SyntaxListBuilder modifiers = _pool.Allocate();
            try
            {
                if (!flag || !base.IsScript)
                {
                    bool isInAsync = IsInAsync;
                    if (!base.IsScript)
                    {
                        IsInAsync = true;
                    }
                    try
                    {
                        switch (base.CurrentToken.Kind)
                        {
                            case SyntaxKind.UnsafeKeyword:
                                if (PeekToken(1).Kind == SyntaxKind.OpenBraceToken)
                                {
                                    return CheckTopLevelStatementsFeatureAvailability(_syntaxFactory.GlobalStatement(ParseUnsafeStatement(syntaxList)));
                                }
                                break;
                            case SyntaxKind.FixedKeyword:
                                if (PeekToken(1).Kind == SyntaxKind.OpenParenToken)
                                {
                                    return CheckTopLevelStatementsFeatureAvailability(_syntaxFactory.GlobalStatement(ParseFixedStatement(syntaxList)));
                                }
                                break;
                            case SyntaxKind.DelegateKeyword:
                                {
                                    SyntaxKind kind = PeekToken(1).Kind;
                                    if (kind != SyntaxKind.OpenParenToken && kind != SyntaxKind.OpenBraceToken)
                                    {
                                        break;
                                    }
                                    return CheckTopLevelStatementsFeatureAvailability(_syntaxFactory.GlobalStatement(ParseExpressionStatement(syntaxList)));
                                }
                            case SyntaxKind.NewKeyword:
                                if (IsPossibleNewExpression())
                                {
                                    return CheckTopLevelStatementsFeatureAvailability(_syntaxFactory.GlobalStatement(ParseExpressionStatement(syntaxList)));
                                }
                                break;
                        }
                    }
                    finally
                    {
                        IsInAsync = isInAsync;
                    }
                }
                ParseModifiers(modifiers, forAccessors: false);
                bool flag2 = modifiers.Count > 0;
                MemberDeclarationSyntax result2;
                if (base.CurrentToken.Kind == SyntaxKind.IdentifierToken && PeekToken(1).Kind == SyntaxKind.OpenParenToken && (flag || flag2))
                {
                    SyntaxToken node = SyntaxFactory.MissingToken(SyntaxKind.VoidKeyword);
                    node = AddError(node, ErrorCode.ERR_MemberNeedsType);
                    PredefinedTypeSyntax type = _syntaxFactory.PredefinedType(node);
                    if (base.IsScript)
                    {
                        SyntaxToken identifier = EatToken();
                        return ParseMethodDeclaration(syntaxList, modifiers, type, null, identifier, null);
                    }
                    if (tryParseLocalDeclarationStatementFromStartPoint<LocalFunctionStatementSyntax>(syntaxList, ref startPoint2, out result2))
                    {
                        return result2;
                    }
                }
                if (base.CurrentToken.Kind == SyntaxKind.ConstKeyword)
                {
                    if (!base.IsScript && tryParseLocalDeclarationStatementFromStartPoint<LocalDeclarationStatementSyntax>(syntaxList, ref startPoint2, out result2))
                    {
                        return result2;
                    }
                    return ParseConstantFieldDeclaration(syntaxList, modifiers, parentKind);
                }
                if (base.CurrentToken.Kind == SyntaxKind.EventKeyword)
                {
                    return ParseEventDeclaration(syntaxList, modifiers, parentKind);
                }
                if (base.CurrentToken.Kind == SyntaxKind.FixedKeyword)
                {
                    return ParseFixedSizeBufferDeclaration(syntaxList, modifiers, parentKind);
                }
                if (base.CurrentToken.Kind == SyntaxKind.ExplicitKeyword || base.CurrentToken.Kind == SyntaxKind.ImplicitKeyword || (base.CurrentToken.Kind == SyntaxKind.OperatorKeyword && !SyntaxFacts.IsAnyOverloadableOperator(PeekToken(1).Kind)))
                {
                    return ParseConversionOperatorDeclaration(syntaxList, modifiers);
                }
                if (base.CurrentToken.Kind == SyntaxKind.NamespaceKeyword)
                {
                    return ParseNamespaceDeclaration(syntaxList, modifiers);
                }
                if (IsTypeDeclarationStart())
                {
                    return ParseTypeDeclaration(syntaxList, modifiers);
                }
                TypeSyntax type2 = ParseReturnType();
                ResetPoint state = GetResetPoint();
                try
                {
                    if ((!flag || !base.IsScript) && !flag2 && (type2.Kind == SyntaxKind.RefType || !IsOperatorKeyword()))
                    {
                        Reset(ref startPoint2);
                        if (base.CurrentToken.Kind != SyntaxKind.CloseBraceToken && base.CurrentToken.Kind != SyntaxKind.EndOfFileToken && IsPossibleStatement(acceptAccessibilityMods: true))
                        {
                            TerminatorState termState2 = _termState;
                            _termState |= TerminatorState.IsPossibleStatementStartOrStop;
                            bool isInAsync2 = IsInAsync;
                            if (!base.IsScript)
                            {
                                IsInAsync = true;
                            }
                            StatementSyntax statement2 = ParseStatementCore(syntaxList, isGlobal: true);
                            IsInAsync = isInAsync2;
                            _termState = termState2;
                            if (isAcceptableNonDeclarationStatement(statement2, base.IsScript))
                            {
                                return CheckTopLevelStatementsFeatureAvailability(_syntaxFactory.GlobalStatement(statement2));
                            }
                        }
                        Reset(ref state);
                    }
                    if (IsMisplacedModifier(modifiers, syntaxList, type2, out result2))
                    {
                        return result2;
                    }
                    ExplicitInterfaceSpecifierSyntax explicitInterfaceOpt;
                    SyntaxToken identifierOrThisOpt;
                    TypeParameterListSyntax typeParameterListOpt;
                    do
                    {
                        bool isRef = type2.IsRef;
                        if (!isRef && IsOperatorKeyword())
                        {
                            return ParseOperatorDeclaration(syntaxList, modifiers, type2);
                        }
                        if ((!isRef || !base.IsScript) && IsFieldDeclaration(isEvent: false))
                        {
                            TerminatorState termState3 = _termState;
                            if ((!flag && !flag2) || !base.IsScript)
                            {
                                _termState |= TerminatorState.IsPossibleStatementStartOrStop;
                                if (!base.IsScript)
                                {
                                    Reset(ref startPoint2);
                                    if (tryParseLocalDeclarationStatement<LocalDeclarationStatementSyntax>(syntaxList, out result2))
                                    {
                                        return result2;
                                    }
                                    Reset(ref state);
                                }
                            }
                            if (!isRef)
                            {
                                return ParseNormalFieldDeclaration(syntaxList, modifiers, type2, parentKind);
                            }
                            _termState = termState3;
                        }
                        ParseMemberName(out explicitInterfaceOpt, out identifierOrThisOpt, out typeParameterListOpt, isEvent: false);
                        if (IsNoneOrIncompleteMember(parentKind, syntaxList, modifiers, type2, explicitInterfaceOpt, identifierOrThisOpt, typeParameterListOpt, out result2))
                        {
                            return result2;
                        }
                    }
                    while (ReconsideredTypeAsAsyncModifier(ref modifiers, ref type2, ref state, ref explicitInterfaceOpt, ref identifierOrThisOpt, ref typeParameterListOpt));
                    CheckForVersionSpecificModifiers(modifiers, SyntaxKind.ReadOnlyKeyword, MessageID.IDS_FeatureReadOnlyMembers);
                    if (TryParseIndexerOrPropertyDeclaration(syntaxList, modifiers, type2, explicitInterfaceOpt, identifierOrThisOpt, typeParameterListOpt, out result2))
                    {
                        return result2;
                    }
                    if (!base.IsScript && explicitInterfaceOpt == null && tryParseLocalDeclarationStatementFromStartPoint<LocalFunctionStatementSyntax>(syntaxList, ref startPoint2, out result2))
                    {
                        return result2;
                    }
                    return ParseMethodDeclaration(syntaxList, modifiers, type2, explicitInterfaceOpt, identifierOrThisOpt, typeParameterListOpt);
                }
                finally
                {
                    Release(ref state);
                }
            }
            finally
            {
                _pool.Free(modifiers);
                _termState = termState;
                Release(ref startPoint2);
            }
            static bool isAcceptableNonDeclarationStatement(StatementSyntax statement, bool isScript)
            {
                SyntaxKind? syntaxKind = statement?.Kind;
                if (syntaxKind.HasValue)
                {
                    SyntaxKind valueOrDefault = syntaxKind.GetValueOrDefault();
                    if (valueOrDefault == SyntaxKind.LocalDeclarationStatement)
                    {
                        if (!isScript)
                        {
                            return ((LocalDeclarationStatementSyntax)statement).UsingKeyword != null;
                        }
                        return false;
                    }
                    if (valueOrDefault != SyntaxKind.ExpressionStatement)
                    {
                        if (valueOrDefault == SyntaxKind.LocalFunctionStatement)
                        {
                            goto IL_006e;
                        }
                    }
                    else if (!isScript)
                    {
                        ExpressionStatementSyntax expressionStatementSyntax = (ExpressionStatementSyntax)statement;
                        if (expressionStatementSyntax.Expression.Kind == SyntaxKind.IdentifierName && expressionStatementSyntax.SemicolonToken.IsMissing)
                        {
                            goto IL_006e;
                        }
                    }
                    return true;
                }
                goto IL_006e;
            IL_006e:
                return false;
            }
            bool tryParseLocalDeclarationStatement<DeclarationSyntax>(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, out MemberDeclarationSyntax result) where DeclarationSyntax : StatementSyntax
            {
                bool isInAsync3 = IsInAsync;
                IsInAsync = true;
                int lastTokenPosition = -1;
                IsMakingProgress(ref lastTokenPosition);
                StatementSyntax statementSyntax = ParseLocalDeclarationStatement(attributes);
                IsInAsync = isInAsync3;
                if (statementSyntax is DeclarationSyntax statement3 && IsMakingProgress(ref lastTokenPosition, assertIfFalse: false))
                {
                    result = CheckTopLevelStatementsFeatureAvailability(_syntaxFactory.GlobalStatement(statement3));
                    return true;
                }
                result = null;
                return false;
            }
            bool tryParseLocalDeclarationStatementFromStartPoint<DeclarationSyntax>(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, ref ResetPoint startPoint, out MemberDeclarationSyntax result) where DeclarationSyntax : StatementSyntax
            {
                ResetPoint state2 = GetResetPoint();
                try
                {
                    Reset(ref startPoint);
                    if (tryParseLocalDeclarationStatement<DeclarationSyntax>(attributes, out result))
                    {
                        return true;
                    }
                    Reset(ref state2);
                    return false;
                }
                finally
                {
                    Release(ref state2);
                }
            }
        }

        private bool IsMisplacedModifier(SyntaxListBuilder modifiers, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, TypeSyntax type, out MemberDeclarationSyntax result)
        {
            if (GetModifier(base.CurrentToken) != 0 && base.CurrentToken.ContextualKind != SyntaxKind.PartialKeyword && base.CurrentToken.ContextualKind != SyntaxKind.AsyncKeyword && base.CurrentToken.ContextualKind != SyntaxKind.DataKeyword && IsComplete(type))
            {
                SyntaxToken currentToken = base.CurrentToken;
                type = AddError(type, type.FullWidth + currentToken.GetLeadingTriviaWidth(), currentToken.Width, ErrorCode.ERR_BadModifierLocation, currentToken.Text);
                result = _syntaxFactory.IncompleteMember(attributes, modifiers.ToList(), type);
                return true;
            }
            result = null;
            return false;
        }

        private bool IsNoneOrIncompleteMember(SyntaxKind parentKind, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers, TypeSyntax type, ExplicitInterfaceSpecifierSyntax explicitInterfaceOpt, SyntaxToken identifierOrThisOpt, TypeParameterListSyntax typeParameterListOpt, out MemberDeclarationSyntax result)
        {
            if (explicitInterfaceOpt == null && identifierOrThisOpt == null && typeParameterListOpt == null)
            {
                if (attributes.Count == 0 && modifiers.Count == 0 && type.IsMissing && type.Kind != SyntaxKind.RefType)
                {
                    result = null;
                    return true;
                }
                IncompleteMemberSyntax incompleteMemberSyntax = _syntaxFactory.IncompleteMember(attributes, modifiers.ToList(), type.IsMissing ? null : type);
                if (incompleteMemberSyntax.ContainsDiagnostics)
                {
                    result = incompleteMemberSyntax;
                }
                else if (parentKind == SyntaxKind.NamespaceDeclaration || (parentKind == SyntaxKind.CompilationUnit && !base.IsScript))
                {
                    result = AddErrorToLastToken(incompleteMemberSyntax, ErrorCode.ERR_NamespaceUnexpected);
                }
                else
                {
                    result = AddError(incompleteMemberSyntax, incompleteMemberSyntax.FullWidth + base.CurrentToken.GetLeadingTriviaWidth(), base.CurrentToken.Width, ErrorCode.ERR_InvalidMemberDecl, base.CurrentToken.Text);
                }
                return true;
            }
            result = null;
            return false;
        }

        private bool ReconsideredTypeAsAsyncModifier(ref SyntaxListBuilder modifiers, ref TypeSyntax type, ref ResetPoint afterTypeResetPoint, ref ExplicitInterfaceSpecifierSyntax explicitInterfaceOpt, ref SyntaxToken identifierOrThisOpt, ref TypeParameterListSyntax typeParameterListOpt)
        {
            if (type.Kind != SyntaxKind.RefType && identifierOrThisOpt != null && ((typeParameterListOpt != null && typeParameterListOpt.ContainsDiagnostics) || (base.CurrentToken.Kind != SyntaxKind.OpenParenToken && base.CurrentToken.Kind != SyntaxKind.OpenBraceToken && base.CurrentToken.Kind != SyntaxKind.EqualsGreaterThanToken)) && ReconsiderTypeAsAsyncModifier(ref modifiers, type, identifierOrThisOpt))
            {
                Reset(ref afterTypeResetPoint);
                explicitInterfaceOpt = null;
                identifierOrThisOpt = null;
                typeParameterListOpt = null;
                Release(ref afterTypeResetPoint);
                type = ParseReturnType();
                afterTypeResetPoint = GetResetPoint();
                return true;
            }
            return false;
        }

        private bool TryParseIndexerOrPropertyDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers, TypeSyntax type, ExplicitInterfaceSpecifierSyntax explicitInterfaceOpt, SyntaxToken identifierOrThisOpt, TypeParameterListSyntax typeParameterListOpt, out MemberDeclarationSyntax result)
        {
            if (identifierOrThisOpt.Kind == SyntaxKind.ThisKeyword)
            {
                result = ParseIndexerDeclaration(attributes, modifiers, type, explicitInterfaceOpt, identifierOrThisOpt, typeParameterListOpt);
                return true;
            }
            SyntaxKind kind = base.CurrentToken.Kind;
            if (kind == SyntaxKind.OpenBraceToken || kind == SyntaxKind.EqualsGreaterThanToken)
            {
                result = ParsePropertyDeclaration(attributes, modifiers, type, explicitInterfaceOpt, identifierOrThisOpt, typeParameterListOpt);
                return true;
            }
            result = null;
            return false;
        }

        internal MemberDeclarationSyntax ParseMemberDeclaration(SyntaxKind parentKind)
        {
            _recursionDepth++;
            StackGuard.EnsureSufficientExecutionStack(_recursionDepth);
            MemberDeclarationSyntax result = ParseMemberDeclarationCore(parentKind);
            _recursionDepth--;
            return result;
        }

        private MemberDeclarationSyntax ParseMemberDeclarationCore(SyntaxKind parentKind)
        {
            CancellationToken cancellationToken = base.cancellationToken;
            cancellationToken.ThrowIfCancellationRequested();
            if (IsIncrementalAndFactoryContextMatches && CanReuseMemberDeclaration(base.CurrentNodeKind, isGlobal: false))
            {
                return (MemberDeclarationSyntax)EatNode();
            }
            SyntaxListBuilder modifiers = _pool.Allocate();
            TerminatorState termState = _termState;
            try
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes = ParseAttributeDeclarations();
                ParseModifiers(modifiers, forAccessors: false);
                if (base.CurrentToken.Kind == SyntaxKind.IdentifierToken && PeekToken(1).Kind == SyntaxKind.OpenParenToken)
                {
                    return ParseConstructorDeclaration(attributes, modifiers);
                }
                if (base.CurrentToken.Kind == SyntaxKind.TildeToken)
                {
                    return ParseDestructorDeclaration(attributes, modifiers);
                }
                if (base.CurrentToken.Kind == SyntaxKind.ConstKeyword)
                {
                    return ParseConstantFieldDeclaration(attributes, modifiers, parentKind);
                }
                if (base.CurrentToken.Kind == SyntaxKind.EventKeyword)
                {
                    return ParseEventDeclaration(attributes, modifiers, parentKind);
                }
                if (base.CurrentToken.Kind == SyntaxKind.FixedKeyword)
                {
                    return ParseFixedSizeBufferDeclaration(attributes, modifiers, parentKind);
                }
                if (base.CurrentToken.Kind == SyntaxKind.ExplicitKeyword || base.CurrentToken.Kind == SyntaxKind.ImplicitKeyword || (base.CurrentToken.Kind == SyntaxKind.OperatorKeyword && !SyntaxFacts.IsAnyOverloadableOperator(PeekToken(1).Kind)))
                {
                    return ParseConversionOperatorDeclaration(attributes, modifiers);
                }
                if (IsTypeDeclarationStart())
                {
                    return ParseTypeDeclaration(attributes, modifiers);
                }
                TypeSyntax type = ParseReturnType();
                ResetPoint afterTypeResetPoint = GetResetPoint();
                try
                {
                    if (IsMisplacedModifier(modifiers, attributes, type, out var result))
                    {
                        return result;
                    }
                    ExplicitInterfaceSpecifierSyntax explicitInterfaceOpt;
                    SyntaxToken identifierOrThisOpt;
                    TypeParameterListSyntax typeParameterListOpt;
                    do
                    {
                        if (type.Kind != SyntaxKind.RefType)
                        {
                            if (IsOperatorKeyword())
                            {
                                return ParseOperatorDeclaration(attributes, modifiers, type);
                            }
                            if (IsFieldDeclaration(isEvent: false))
                            {
                                return ParseNormalFieldDeclaration(attributes, modifiers, type, parentKind);
                            }
                        }
                        ParseMemberName(out explicitInterfaceOpt, out identifierOrThisOpt, out typeParameterListOpt, isEvent: false);
                        if (IsNoneOrIncompleteMember(parentKind, attributes, modifiers, type, explicitInterfaceOpt, identifierOrThisOpt, typeParameterListOpt, out result))
                        {
                            return result;
                        }
                    }
                    while (ReconsideredTypeAsAsyncModifier(ref modifiers, ref type, ref afterTypeResetPoint, ref explicitInterfaceOpt, ref identifierOrThisOpt, ref typeParameterListOpt));
                    CheckForVersionSpecificModifiers(modifiers, SyntaxKind.ReadOnlyKeyword, MessageID.IDS_FeatureReadOnlyMembers);
                    if (TryParseIndexerOrPropertyDeclaration(attributes, modifiers, type, explicitInterfaceOpt, identifierOrThisOpt, typeParameterListOpt, out result))
                    {
                        return result;
                    }
                    return ParseMethodDeclaration(attributes, modifiers, type, explicitInterfaceOpt, identifierOrThisOpt, typeParameterListOpt);
                }
                finally
                {
                    Release(ref afterTypeResetPoint);
                }
            }
            finally
            {
                _pool.Free(modifiers);
                _termState = termState;
            }
        }

        private bool ReconsiderTypeAsAsyncModifier(ref SyntaxListBuilder modifiers, TypeSyntax type, SyntaxToken identifierOrThisOpt)
        {
            if (type.Kind != SyntaxKind.IdentifierName)
            {
                return false;
            }
            if (identifierOrThisOpt.Kind != SyntaxKind.IdentifierToken)
            {
                return false;
            }
            SyntaxToken identifier = ((IdentifierNameSyntax)type).Identifier;
            SyntaxKind contextualKind = identifier.ContextualKind;
            if (contextualKind != SyntaxKind.AsyncKeyword || modifiers.Any((int)contextualKind))
            {
                return false;
            }
            modifiers.Add(SyntaxParser.ConvertToKeyword(identifier));
            return true;
        }

        private bool IsFieldDeclaration(bool isEvent)
        {
            if (base.CurrentToken.Kind != SyntaxKind.IdentifierToken)
            {
                return false;
            }
            if (base.CurrentToken.ContextualKind == SyntaxKind.GlobalKeyword && PeekToken(1).Kind == SyntaxKind.UsingKeyword)
            {
                return false;
            }
            switch (PeekToken(1).Kind)
            {
                case SyntaxKind.OpenBraceToken:
                case SyntaxKind.LessThanToken:
                case SyntaxKind.DotToken:
                case SyntaxKind.ColonColonToken:
                case SyntaxKind.EqualsGreaterThanToken:
                    return false;
                case SyntaxKind.OpenParenToken:
                    return isEvent;
                default:
                    return true;
            }
        }

        private bool IsOperatorKeyword()
        {
            if (base.CurrentToken.Kind != SyntaxKind.ImplicitKeyword && base.CurrentToken.Kind != SyntaxKind.ExplicitKeyword)
            {
                return base.CurrentToken.Kind == SyntaxKind.OperatorKeyword;
            }
            return true;
        }

        public static bool IsComplete(CSharpSyntaxNode node)
        {
            if (node == null)
            {
                return false;
            }
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.ChildSyntaxList.Reversed.Enumerator enumerator = node.ChildNodesAndTokens().Reverse().GetEnumerator();
            while (enumerator.MoveNext())
            {
                GreenNode current = enumerator.Current;
                if (!(current is SyntaxToken syntaxToken))
                {
                    return IsComplete((CSharpSyntaxNode)current);
                }
                if (syntaxToken.IsMissing)
                {
                    return false;
                }
                if (syntaxToken.Kind != 0)
                {
                    return true;
                }
            }
            return true;
        }

        private ConstructorDeclarationSyntax ParseConstructorDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers)
        {
            SyntaxToken identifier = ParseIdentifierToken();
            TerminatorState termState = _termState;
            _termState |= TerminatorState.IsEndOfMethodSignature;
            try
            {
                ParameterListSyntax parameterList = ParseParenthesizedParameterList();
                ConstructorInitializerSyntax initializer = ((base.CurrentToken.Kind == SyntaxKind.ColonToken) ? ParseConstructorInitializer() : null);
                ParseBlockAndExpressionBodiesWithSemicolon(out var blockBody, out var expressionBody, out var semicolon, parseSemicolonAfterBlock: true, MessageID.IDS_FeatureExpressionBodiedDeOrConstructor);
                return _syntaxFactory.ConstructorDeclaration(attributes, modifiers.ToList(), identifier, parameterList, initializer, blockBody, expressionBody, semicolon);
            }
            finally
            {
                _termState = termState;
            }
        }

        private ConstructorInitializerSyntax ParseConstructorInitializer()
        {
            SyntaxToken colonToken = EatToken(SyntaxKind.ColonToken);
            bool reportError = true;
            SyntaxKind kind = ((base.CurrentToken.Kind == SyntaxKind.BaseKeyword) ? SyntaxKind.BaseConstructorInitializer : SyntaxKind.ThisConstructorInitializer);
            SyntaxToken thisOrBaseKeyword;
            if (base.CurrentToken.Kind == SyntaxKind.BaseKeyword || base.CurrentToken.Kind == SyntaxKind.ThisKeyword)
            {
                thisOrBaseKeyword = EatToken();
            }
            else
            {
                thisOrBaseKeyword = EatToken(SyntaxKind.ThisKeyword, ErrorCode.ERR_ThisOrBaseExpected);
                reportError = false;
            }
            ArgumentListSyntax argumentList;
            if (base.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                argumentList = ParseParenthesizedArgumentList();
            }
            else
            {
                SyntaxToken openParenToken = EatToken(SyntaxKind.OpenParenToken, reportError);
                SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken, reportError);
                argumentList = _syntaxFactory.ArgumentList(openParenToken, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax>), closeParenToken);
            }
            return _syntaxFactory.ConstructorInitializer(kind, colonToken, thisOrBaseKeyword, argumentList);
        }

        private DestructorDeclarationSyntax ParseDestructorDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers)
        {
            SyntaxToken tildeToken = EatToken(SyntaxKind.TildeToken);
            SyntaxToken identifier = ParseIdentifierToken();
            SyntaxToken openParenToken = EatToken(SyntaxKind.OpenParenToken);
            SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
            ParseBlockAndExpressionBodiesWithSemicolon(out var blockBody, out var expressionBody, out var semicolon, parseSemicolonAfterBlock: true, MessageID.IDS_FeatureExpressionBodiedDeOrConstructor);
            ParameterListSyntax parameterList = _syntaxFactory.ParameterList(openParenToken, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax>), closeParenToken);
            return _syntaxFactory.DestructorDeclaration(attributes, modifiers.ToList(), tildeToken, identifier, parameterList, blockBody, expressionBody, semicolon);
        }

        private void ParseBlockAndExpressionBodiesWithSemicolon(out BlockSyntax blockBody, out ArrowExpressionClauseSyntax expressionBody, out SyntaxToken semicolon, bool parseSemicolonAfterBlock = true, MessageID requestedExpressionBodyFeature = MessageID.IDS_FeatureExpressionBodiedMethod)
        {
            if (base.CurrentToken.Kind == SyntaxKind.SemicolonToken)
            {
                blockBody = null;
                expressionBody = null;
                semicolon = EatToken(SyntaxKind.SemicolonToken);
                return;
            }
            blockBody = null;
            expressionBody = null;
            if (base.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
            {
                blockBody = ParseMethodOrAccessorBodyBlock(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), isAccessorBody: false);
            }
            if (base.CurrentToken.Kind == SyntaxKind.EqualsGreaterThanToken)
            {
                expressionBody = ParseArrowExpressionClause();
                expressionBody = CheckFeatureAvailability(expressionBody, requestedExpressionBodyFeature);
            }
            semicolon = null;
            if (expressionBody != null || blockBody == null)
            {
                semicolon = EatToken(SyntaxKind.SemicolonToken);
            }
            else if (parseSemicolonAfterBlock && base.CurrentToken.Kind == SyntaxKind.SemicolonToken)
            {
                semicolon = EatTokenWithPrejudice(ErrorCode.ERR_UnexpectedSemicolon);
            }
        }

        private bool IsEndOfTypeParameterList()
        {
            if (base.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                return true;
            }
            if (base.CurrentToken.Kind == SyntaxKind.ColonToken)
            {
                return true;
            }
            if (base.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
            {
                return true;
            }
            if (IsCurrentTokenWhereOfConstraintClause())
            {
                return true;
            }
            return false;
        }

        private bool IsEndOfMethodSignature()
        {
            if (base.CurrentToken.Kind != SyntaxKind.SemicolonToken)
            {
                return base.CurrentToken.Kind == SyntaxKind.OpenBraceToken;
            }
            return true;
        }

        private bool IsEndOfRecordSignature()
        {
            if (base.CurrentToken.Kind != SyntaxKind.SemicolonToken)
            {
                return base.CurrentToken.Kind == SyntaxKind.OpenBraceToken;
            }
            return true;
        }

        private bool IsEndOfNameInExplicitInterface()
        {
            if (base.CurrentToken.Kind != SyntaxKind.DotToken)
            {
                return base.CurrentToken.Kind == SyntaxKind.ColonColonToken;
            }
            return true;
        }

        private bool IsEndOfFunctionPointerParameterList(bool errored)
        {
            return (int)base.CurrentToken.Kind == (errored ? 8201 : 8217);
        }

        private bool IsEndOfFunctionPointerCallingConvention()
        {
            return base.CurrentToken.Kind == SyntaxKind.CloseBracketToken;
        }

        private MethodDeclarationSyntax ParseMethodDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers, TypeSyntax type, ExplicitInterfaceSpecifierSyntax explicitInterfaceOpt, SyntaxToken identifier, TypeParameterListSyntax typeParameterList)
        {
            TerminatorState termState = _termState;
            _termState |= TerminatorState.IsEndOfMethodSignature;
            ParameterListSyntax parameterListSyntax = ParseParenthesizedParameterList();
            SyntaxListBuilder<TypeParameterConstraintClauseSyntax> syntaxListBuilder = default(SyntaxListBuilder<TypeParameterConstraintClauseSyntax>);
            try
            {
                if (base.CurrentToken.ContextualKind == SyntaxKind.WhereKeyword)
                {
                    syntaxListBuilder = _pool.Allocate<TypeParameterConstraintClauseSyntax>();
                    ParseTypeParameterConstraintClauses(syntaxListBuilder);
                }
                else if (base.CurrentToken.Kind == SyntaxKind.ColonToken)
                {
                    SyntaxToken currentToken = base.CurrentToken;
                    ConstructorInitializerSyntax node = ParseConstructorInitializer();
                    node = AddErrorToFirstToken(node, ErrorCode.ERR_UnexpectedToken, currentToken.Text);
                    parameterListSyntax = AddTrailingSkippedSyntax(parameterListSyntax, node);
                }
                _termState = termState;
                IsInAsync = modifiers.Any(8435);
                ParseBlockAndExpressionBodiesWithSemicolon(out var blockBody, out var expressionBody, out var semicolon);
                IsInAsync = false;
                return _syntaxFactory.MethodDeclaration(attributes, modifiers.ToList(), type, explicitInterfaceOpt, identifier, typeParameterList, parameterListSyntax, syntaxListBuilder, blockBody, expressionBody, semicolon);
            }
            finally
            {
                if (!syntaxListBuilder.IsNull)
                {
                    _pool.Free(syntaxListBuilder);
                }
            }
        }

        private TypeSyntax ParseReturnType()
        {
            TerminatorState termState = _termState;
            _termState |= TerminatorState.IsEndOfReturnType;
            TypeSyntax result = ParseTypeOrVoid();
            _termState = termState;
            return result;
        }

        private bool IsEndOfReturnType()
        {
            SyntaxKind kind = base.CurrentToken.Kind;
            if (kind == SyntaxKind.OpenParenToken || kind == SyntaxKind.OpenBraceToken || kind == SyntaxKind.SemicolonToken)
            {
                return true;
            }
            return false;
        }

        private ConversionOperatorDeclarationSyntax ParseConversionOperatorDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers)
        {
            SyntaxToken implicitOrExplicitKeyword = ((base.CurrentToken.Kind != SyntaxKind.ImplicitKeyword && base.CurrentToken.Kind != SyntaxKind.ExplicitKeyword) ? EatToken(SyntaxKind.ExplicitKeyword) : EatToken());
            SyntaxToken operatorKeyword = EatToken(SyntaxKind.OperatorKeyword);
            TypeSyntax type = ParseType();
            ParameterListSyntax parameterList = ParseParenthesizedParameterList();
            ParseBlockAndExpressionBodiesWithSemicolon(out var blockBody, out var expressionBody, out var semicolon);
            return _syntaxFactory.ConversionOperatorDeclaration(attributes, modifiers.ToList(), implicitOrExplicitKeyword, operatorKeyword, type, parameterList, blockBody, expressionBody, semicolon);
        }

        private OperatorDeclarationSyntax ParseOperatorDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers, TypeSyntax type)
        {
            SyntaxToken operatorKeyword = EatToken(SyntaxKind.OperatorKeyword);
            SyntaxToken syntaxToken;
            int offset;
            int width;
            if (SyntaxFacts.IsAnyOverloadableOperator(base.CurrentToken.Kind))
            {
                syntaxToken = EatToken();
                offset = syntaxToken.GetLeadingTriviaWidth();
                width = syntaxToken.Width;
            }
            else if (base.CurrentToken.Kind == SyntaxKind.ImplicitKeyword || base.CurrentToken.Kind == SyntaxKind.ExplicitKeyword)
            {
                GetDiagnosticSpanForMissingToken(out offset, out width);
                syntaxToken = ConvertToMissingWithTrailingTrivia(EatToken(), SyntaxKind.PlusToken);
                if (type.IsMissing)
                {
                    SyntaxDiagnosticInfo syntaxDiagnosticInfo = SyntaxParser.MakeError(offset, width, ErrorCode.ERR_BadOperatorSyntax, SyntaxFacts.GetText(SyntaxKind.PlusToken));
                    syntaxToken = WithAdditionalDiagnostics(syntaxToken, syntaxDiagnosticInfo);
                }
                else
                {
                    type = AddError(type, ErrorCode.ERR_BadOperatorSyntax, SyntaxFacts.GetText(SyntaxKind.PlusToken));
                }
            }
            else
            {
                syntaxToken = EatToken();
                offset = syntaxToken.GetLeadingTriviaWidth();
                width = syntaxToken.Width;
            }
            SyntaxKind kind = syntaxToken.Kind;
            SyntaxToken currentToken = base.CurrentToken;
            if (syntaxToken.Kind == SyntaxKind.GreaterThanToken && currentToken.Kind == SyntaxKind.GreaterThanToken && syntaxToken.GetTrailingTriviaWidth() == 0 && currentToken.GetLeadingTriviaWidth() == 0)
            {
                SyntaxToken syntaxToken2 = EatToken();
                syntaxToken = SyntaxFactory.Token(syntaxToken.GetLeadingTrivia(), SyntaxKind.GreaterThanGreaterThanToken, syntaxToken2.GetTrailingTrivia());
            }
            ParameterListSyntax parameterListSyntax = ParseParenthesizedParameterList();
            switch (parameterListSyntax.Parameters.Count)
            {
                case 1:
                    if (syntaxToken.IsMissing || !SyntaxFacts.IsOverloadableUnaryOperator(kind))
                    {
                        SyntaxDiagnosticInfo syntaxDiagnosticInfo4 = SyntaxParser.MakeError(offset, width, ErrorCode.ERR_OvlUnaryOperatorExpected);
                        syntaxToken = WithAdditionalDiagnostics(syntaxToken, syntaxDiagnosticInfo4);
                    }
                    break;
                case 2:
                    if (syntaxToken.IsMissing || !SyntaxFacts.IsOverloadableBinaryOperator(kind))
                    {
                        SyntaxDiagnosticInfo syntaxDiagnosticInfo3 = SyntaxParser.MakeError(offset, width, ErrorCode.ERR_OvlBinaryOperatorExpected);
                        syntaxToken = WithAdditionalDiagnostics(syntaxToken, syntaxDiagnosticInfo3);
                    }
                    break;
                default:
                    if (syntaxToken.IsMissing)
                    {
                        SyntaxDiagnosticInfo syntaxDiagnosticInfo2 = SyntaxParser.MakeError(offset, width, ErrorCode.ERR_OvlOperatorExpected);
                        syntaxToken = WithAdditionalDiagnostics(syntaxToken, syntaxDiagnosticInfo2);
                    }
                    else
                    {
                        syntaxToken = ((!SyntaxFacts.IsOverloadableBinaryOperator(kind)) ? ((!SyntaxFacts.IsOverloadableUnaryOperator(kind)) ? AddError(syntaxToken, ErrorCode.ERR_OvlOperatorExpected) : AddError(syntaxToken, ErrorCode.ERR_BadUnOpArgs, SyntaxFacts.GetText(kind))) : AddError(syntaxToken, ErrorCode.ERR_BadBinOpArgs, SyntaxFacts.GetText(kind)));
                    }
                    break;
            }
            ParseBlockAndExpressionBodiesWithSemicolon(out var blockBody, out var expressionBody, out var semicolon);
            if (kind != SyntaxKind.IsKeyword && !SyntaxFacts.IsOverloadableUnaryOperator(kind) && !SyntaxFacts.IsOverloadableBinaryOperator(kind))
            {
                syntaxToken = ConvertToMissingWithTrailingTrivia(syntaxToken, SyntaxKind.PlusToken);
            }
            return _syntaxFactory.OperatorDeclaration(attributes, modifiers.ToList(), type, operatorKeyword, syntaxToken, parameterListSyntax, blockBody, expressionBody, semicolon);
        }

        private IndexerDeclarationSyntax ParseIndexerDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers, TypeSyntax type, ExplicitInterfaceSpecifierSyntax explicitInterfaceOpt, SyntaxToken thisKeyword, TypeParameterListSyntax typeParameterList)
        {
            if (typeParameterList != null)
            {
                thisKeyword = AddTrailingSkippedSyntax(thisKeyword, typeParameterList);
                thisKeyword = AddError(thisKeyword, ErrorCode.ERR_UnexpectedGenericName);
            }
            BracketedParameterListSyntax parameterList = ParseBracketedParameterList();
            AccessorListSyntax accessorList = null;
            ArrowExpressionClauseSyntax expressionBody = null;
            SyntaxToken syntaxToken = null;
            if (base.CurrentToken.Kind == SyntaxKind.EqualsGreaterThanToken)
            {
                expressionBody = ParseArrowExpressionClause();
                expressionBody = CheckFeatureAvailability(expressionBody, MessageID.IDS_FeatureExpressionBodiedIndexer);
                syntaxToken = EatToken(SyntaxKind.SemicolonToken);
            }
            else
            {
                accessorList = ParseAccessorList(isEvent: false);
                if (base.CurrentToken.Kind == SyntaxKind.SemicolonToken)
                {
                    syntaxToken = EatTokenWithPrejudice(ErrorCode.ERR_UnexpectedSemicolon);
                }
            }
            if (base.CurrentToken.Kind == SyntaxKind.EqualsGreaterThanToken && syntaxToken == null)
            {
                expressionBody = ParseArrowExpressionClause();
                expressionBody = CheckFeatureAvailability(expressionBody, MessageID.IDS_FeatureExpressionBodiedIndexer);
                syntaxToken = EatToken(SyntaxKind.SemicolonToken);
            }
            return _syntaxFactory.IndexerDeclaration(attributes, modifiers.ToList(), type, explicitInterfaceOpt, thisKeyword, parameterList, accessorList, expressionBody, syntaxToken);
        }

        private PropertyDeclarationSyntax ParsePropertyDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers, TypeSyntax type, ExplicitInterfaceSpecifierSyntax explicitInterfaceOpt, SyntaxToken identifier, TypeParameterListSyntax typeParameterList)
        {
            if (typeParameterList != null)
            {
                identifier = AddTrailingSkippedSyntax(identifier, typeParameterList);
                identifier = AddError(identifier, ErrorCode.ERR_UnexpectedGenericName);
            }
            AccessorListSyntax accessorList = null;
            if (base.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
            {
                accessorList = ParseAccessorList(isEvent: false);
            }
            ArrowExpressionClauseSyntax arrowExpressionClauseSyntax = null;
            EqualsValueClauseSyntax equalsValueClauseSyntax = null;
            if (base.CurrentToken.Kind == SyntaxKind.EqualsGreaterThanToken)
            {
                arrowExpressionClauseSyntax = ParseArrowExpressionClause();
                arrowExpressionClauseSyntax = CheckFeatureAvailability(arrowExpressionClauseSyntax, MessageID.IDS_FeatureExpressionBodiedProperty);
            }
            else if (base.CurrentToken.Kind == SyntaxKind.EqualsToken)
            {
                SyntaxToken equalsToken = EatToken(SyntaxKind.EqualsToken);
                ExpressionSyntax value = ParseVariableInitializer();
                equalsValueClauseSyntax = _syntaxFactory.EqualsValueClause(equalsToken, value);
                equalsValueClauseSyntax = CheckFeatureAvailability(equalsValueClauseSyntax, MessageID.IDS_FeatureAutoPropertyInitializer);
            }
            SyntaxToken semicolonToken = null;
            if (arrowExpressionClauseSyntax != null || equalsValueClauseSyntax != null)
            {
                semicolonToken = EatToken(SyntaxKind.SemicolonToken);
            }
            else if (base.CurrentToken.Kind == SyntaxKind.SemicolonToken)
            {
                semicolonToken = EatTokenWithPrejudice(ErrorCode.ERR_UnexpectedSemicolon);
            }
            return _syntaxFactory.PropertyDeclaration(attributes, modifiers.ToList(), type, explicitInterfaceOpt, identifier, accessorList, arrowExpressionClauseSyntax, equalsValueClauseSyntax, semicolonToken);
        }

        private AccessorListSyntax ParseAccessorList(bool isEvent)
        {
            SyntaxToken openBrace = EatToken(SyntaxKind.OpenBraceToken);
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AccessorDeclarationSyntax> accessors = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AccessorDeclarationSyntax>);
            if (!openBrace.IsMissing || !IsTerminator())
            {
                SyntaxListBuilder<AccessorDeclarationSyntax> syntaxListBuilder = _pool.Allocate<AccessorDeclarationSyntax>();
                try
                {
                    while (base.CurrentToken.Kind != SyntaxKind.CloseBraceToken)
                    {
                        if (IsPossibleAccessor())
                        {
                            AccessorDeclarationSyntax node = ParseAccessorDeclaration(isEvent);
                            syntaxListBuilder.Add(node);
                        }
                        else if (SkipBadAccessorListTokens(ref openBrace, syntaxListBuilder, isEvent ? ErrorCode.ERR_AddOrRemoveExpected : ErrorCode.ERR_GetOrSetExpected) == PostSkipAction.Abort)
                        {
                            break;
                        }
                    }
                    accessors = syntaxListBuilder.ToList();
                }
                finally
                {
                    _pool.Free(syntaxListBuilder);
                }
            }
            SyntaxToken closeBraceToken = EatToken(SyntaxKind.CloseBraceToken);
            return _syntaxFactory.AccessorList(openBrace, accessors, closeBraceToken);
        }

        private ArrowExpressionClauseSyntax ParseArrowExpressionClause()
        {
            SyntaxToken arrowToken = EatToken(SyntaxKind.EqualsGreaterThanToken);
            return _syntaxFactory.ArrowExpressionClause(arrowToken, ParsePossibleRefExpression());
        }

        private ExpressionSyntax ParsePossibleRefExpression()
        {
            SyntaxToken syntaxToken = null;
            if (base.CurrentToken.Kind == SyntaxKind.RefKeyword)
            {
                syntaxToken = EatToken();
                syntaxToken = CheckFeatureAvailability(syntaxToken, MessageID.IDS_FeatureRefLocalsReturns);
            }
            ExpressionSyntax expressionSyntax = ParseExpressionCore();
            if (syntaxToken != null)
            {
                expressionSyntax = _syntaxFactory.RefExpression(syntaxToken, expressionSyntax);
            }
            return expressionSyntax;
        }

        private PostSkipAction SkipBadAccessorListTokens(ref SyntaxToken openBrace, SyntaxListBuilder<AccessorDeclarationSyntax> list, ErrorCode error)
        {
            return SkipBadListTokensWithErrorCode(ref openBrace, list, (LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CloseBraceToken && !p.IsPossibleAccessor(), (LanguageParser p) => p.IsTerminator(), error);
        }

        private bool IsPossibleAccessor()
        {
            if (base.CurrentToken.Kind != SyntaxKind.IdentifierToken && !IsPossibleAttributeDeclaration() && SyntaxFacts.GetAccessorDeclarationKind(base.CurrentToken.ContextualKind) == SyntaxKind.None && base.CurrentToken.Kind != SyntaxKind.OpenBraceToken && base.CurrentToken.Kind != SyntaxKind.SemicolonToken)
            {
                return IsPossibleAccessorModifier();
            }
            return true;
        }

        private bool IsPossibleAccessorModifier()
        {
            if (GetModifier(base.CurrentToken) == DeclarationModifiers.None)
            {
                return false;
            }
            int i;
            for (i = 1; GetModifier(PeekToken(i)) != 0; i++)
            {
            }
            SyntaxToken syntaxToken = PeekToken(i);
            if (syntaxToken.Kind == SyntaxKind.CloseBraceToken || syntaxToken.Kind == SyntaxKind.EndOfFileToken)
            {
                return true;
            }
            SyntaxKind contextualKind = syntaxToken.ContextualKind;
            if (contextualKind - 8417 <= (SyntaxKind)3 || contextualKind == SyntaxKind.InitKeyword)
            {
                return true;
            }
            return false;
        }

        private PostSkipAction SkipBadSeparatedListTokensWithExpectedKind<T, TNode>(ref T startToken, SeparatedSyntaxListBuilder<TNode> list, Func<LanguageParser, bool> isNotExpectedFunction, Func<LanguageParser, bool> abortFunction, SyntaxKind expected) where T : CSharpSyntaxNode where TNode : CSharpSyntaxNode
        {
            PostSkipAction result = SkipBadListTokensWithExpectedKindHelper(list.UnderlyingBuilder, isNotExpectedFunction, abortFunction, expected, out GreenNode trailingTrivia);
            if (trailingTrivia != null)
            {
                startToken = AddTrailingSkippedSyntax(startToken, trailingTrivia);
            }
            return result;
        }

        private PostSkipAction SkipBadListTokensWithErrorCode<T, TNode>(ref T startToken, SyntaxListBuilder<TNode> list, Func<LanguageParser, bool> isNotExpectedFunction, Func<LanguageParser, bool> abortFunction, ErrorCode error) where T : CSharpSyntaxNode where TNode : CSharpSyntaxNode
        {
            PostSkipAction result = SkipBadListTokensWithErrorCodeHelper(list, isNotExpectedFunction, abortFunction, error, out GreenNode trailingTrivia);
            if (trailingTrivia != null)
            {
                startToken = AddTrailingSkippedSyntax(startToken, trailingTrivia);
            }
            return result;
        }

        private PostSkipAction SkipBadListTokensWithExpectedKindHelper(SyntaxListBuilder list, Func<LanguageParser, bool> isNotExpectedFunction, Func<LanguageParser, bool> abortFunction, SyntaxKind expected, out GreenNode trailingTrivia)
        {
            if (list.Count == 0)
            {
                return SkipBadTokensWithExpectedKind(isNotExpectedFunction, abortFunction, expected, out trailingTrivia);
            }
            PostSkipAction result = SkipBadTokensWithExpectedKind(isNotExpectedFunction, abortFunction, expected, out GreenNode trailingTrivia2);
            if (trailingTrivia2 != null)
            {
                AddTrailingSkippedSyntax(list, trailingTrivia2);
            }
            trailingTrivia = null;
            return result;
        }

        private PostSkipAction SkipBadListTokensWithErrorCodeHelper<TNode>(SyntaxListBuilder<TNode> list, Func<LanguageParser, bool> isNotExpectedFunction, Func<LanguageParser, bool> abortFunction, ErrorCode error, out GreenNode trailingTrivia) where TNode : CSharpSyntaxNode
        {
            if (list.Count == 0)
            {
                return SkipBadTokensWithErrorCode(isNotExpectedFunction, abortFunction, error, out trailingTrivia);
            }
            PostSkipAction result = SkipBadTokensWithErrorCode(isNotExpectedFunction, abortFunction, error, out GreenNode trailingTrivia2);
            if (trailingTrivia2 != null)
            {
                AddTrailingSkippedSyntax(list, trailingTrivia2);
            }
            trailingTrivia = null;
            return result;
        }

        private PostSkipAction SkipBadTokensWithExpectedKind(Func<LanguageParser, bool> isNotExpectedFunction, Func<LanguageParser, bool> abortFunction, SyntaxKind expected, out GreenNode trailingTrivia)
        {
            SyntaxListBuilder syntaxListBuilder = _pool.Allocate();
            try
            {
                bool flag = true;
                PostSkipAction result = PostSkipAction.Continue;
                while (isNotExpectedFunction(this))
                {
                    if (abortFunction(this))
                    {
                        result = PostSkipAction.Abort;
                        break;
                    }
                    SyntaxToken item = ((flag && !base.CurrentToken.ContainsDiagnostics) ? EatTokenWithPrejudice(expected) : EatToken());
                    flag = false;
                    syntaxListBuilder.Add(item);
                }
                trailingTrivia = ((syntaxListBuilder.Count > 0) ? syntaxListBuilder.ToListNode() : null);
                return result;
            }
            finally
            {
                _pool.Free(syntaxListBuilder);
            }
        }

        private PostSkipAction SkipBadTokensWithErrorCode(Func<LanguageParser, bool> isNotExpectedFunction, Func<LanguageParser, bool> abortFunction, ErrorCode errorCode, out GreenNode trailingTrivia)
        {
            SyntaxListBuilder syntaxListBuilder = _pool.Allocate();
            try
            {
                bool flag = true;
                PostSkipAction result = PostSkipAction.Continue;
                while (isNotExpectedFunction(this))
                {
                    if (abortFunction(this))
                    {
                        result = PostSkipAction.Abort;
                        break;
                    }
                    SyntaxToken item = ((flag && !base.CurrentToken.ContainsDiagnostics) ? EatTokenWithPrejudice(errorCode) : EatToken());
                    flag = false;
                    syntaxListBuilder.Add(item);
                }
                trailingTrivia = ((syntaxListBuilder.Count > 0) ? syntaxListBuilder.ToListNode() : null);
                return result;
            }
            finally
            {
                _pool.Free(syntaxListBuilder);
            }
        }

        private AccessorDeclarationSyntax ParseAccessorDeclaration(bool isEvent)
        {
            if (IsIncrementalAndFactoryContextMatches && SyntaxFacts.IsAccessorDeclaration(base.CurrentNodeKind))
            {
                return (AccessorDeclarationSyntax)EatNode();
            }
            SyntaxListBuilder syntaxListBuilder = _pool.Allocate();
            try
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists = ParseAttributeDeclarations();
                ParseModifiers(syntaxListBuilder, forAccessors: true);
                CheckForVersionSpecificModifiers(syntaxListBuilder, SyntaxKind.ReadOnlyKeyword, MessageID.IDS_FeatureReadOnlyMembers);
                if (!isEvent && syntaxListBuilder != null && syntaxListBuilder.Count > 0)
                {
                    syntaxListBuilder[0] = CheckFeatureAvailability(syntaxListBuilder[0], MessageID.IDS_FeaturePropertyAccessorMods);
                }
                SyntaxToken syntaxToken = EatToken(SyntaxKind.IdentifierToken, isEvent ? ErrorCode.ERR_AddOrRemoveExpected : ErrorCode.ERR_GetOrSetExpected);
                SyntaxKind accessorKind = GetAccessorKind(syntaxToken);
                if (accessorKind == SyntaxKind.UnknownAccessorDeclaration)
                {
                    if (!syntaxToken.IsMissing)
                    {
                        syntaxToken = AddError(syntaxToken, isEvent ? ErrorCode.ERR_AddOrRemoveExpected : ErrorCode.ERR_GetOrSetExpected);
                    }
                }
                else
                {
                    syntaxToken = SyntaxParser.ConvertToKeyword(syntaxToken);
                }
                BlockSyntax blockBody = null;
                ArrowExpressionClauseSyntax expressionBody = null;
                SyntaxToken semicolon = null;
                bool flag = base.CurrentToken.Kind == SyntaxKind.SemicolonToken;
                bool flag2 = base.CurrentToken.Kind == SyntaxKind.EqualsGreaterThanToken;
                if (base.CurrentToken.Kind == SyntaxKind.OpenBraceToken || flag2)
                {
                    ParseBlockAndExpressionBodiesWithSemicolon(out blockBody, out expressionBody, out semicolon, parseSemicolonAfterBlock: true, MessageID.IDS_FeatureExpressionBodiedAccessor);
                }
                else if (flag)
                {
                    semicolon = EatAccessorSemicolon();
                }
                else if (accessorKind != SyntaxKind.UnknownAccessorDeclaration)
                {
                    if (!IsTerminator())
                    {
                        blockBody = ParseMethodOrAccessorBodyBlock(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), isAccessorBody: true);
                    }
                    else
                    {
                        semicolon = EatAccessorSemicolon();
                    }
                }
                return _syntaxFactory.AccessorDeclaration(accessorKind, attributeLists, syntaxListBuilder.ToList(), syntaxToken, blockBody, expressionBody, semicolon);
            }
            finally
            {
                _pool.Free(syntaxListBuilder);
            }
        }

        private SyntaxToken EatAccessorSemicolon()
        {
            return EatToken(SyntaxKind.SemicolonToken, IsFeatureEnabled(MessageID.IDS_FeatureExpressionBodiedAccessor) ? ErrorCode.ERR_SemiOrLBraceOrArrowExpected : ErrorCode.ERR_SemiOrLBraceExpected);
        }

        private SyntaxKind GetAccessorKind(SyntaxToken accessorName)
        {
            return accessorName.ContextualKind switch
            {
                SyntaxKind.GetKeyword => SyntaxKind.GetAccessorDeclaration,
                SyntaxKind.SetKeyword => SyntaxKind.SetAccessorDeclaration,
                SyntaxKind.InitKeyword => SyntaxKind.InitAccessorDeclaration,
                SyntaxKind.AddKeyword => SyntaxKind.AddAccessorDeclaration,
                SyntaxKind.RemoveKeyword => SyntaxKind.RemoveAccessorDeclaration,
                _ => SyntaxKind.UnknownAccessorDeclaration,
            };
        }

        internal ParameterListSyntax ParseParenthesizedParameterList()
        {
            if (IsIncrementalAndFactoryContextMatches && CanReuseParameterList(base.CurrentNode as Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax))
            {
                return (ParameterListSyntax)EatNode();
            }
            SeparatedSyntaxListBuilder<ParameterSyntax> builder = _pool.AllocateSeparated<ParameterSyntax>();
            try
            {
                SyntaxKind openKind = SyntaxKind.OpenParenToken;
                SyntaxKind closeKind = SyntaxKind.CloseParenToken;
                ParseParameterList(out var open, builder, out var close, openKind, closeKind);
                return _syntaxFactory.ParameterList(open, builder, close);
            }
            finally
            {
                _pool.Free(in builder);
            }
        }

        internal BracketedParameterListSyntax ParseBracketedParameterList()
        {
            if (IsIncrementalAndFactoryContextMatches && CanReuseBracketedParameterList(base.CurrentNode as Microsoft.CodeAnalysis.CSharp.Syntax.BracketedParameterListSyntax))
            {
                return (BracketedParameterListSyntax)EatNode();
            }
            SeparatedSyntaxListBuilder<ParameterSyntax> builder = _pool.AllocateSeparated<ParameterSyntax>();
            try
            {
                SyntaxKind openKind = SyntaxKind.OpenBracketToken;
                SyntaxKind closeKind = SyntaxKind.CloseBracketToken;
                ParseParameterList(out var open, builder, out var close, openKind, closeKind);
                return _syntaxFactory.BracketedParameterList(open, builder, close);
            }
            finally
            {
                _pool.Free(in builder);
            }
        }

        private static bool CanReuseParameterList(Microsoft.CodeAnalysis.CSharp.Syntax.ParameterListSyntax list)
        {
            if (list == null)
            {
                return false;
            }
            if (list.OpenParenToken.IsMissing)
            {
                return false;
            }
            if (list.CloseParenToken.IsMissing)
            {
                return false;
            }
            SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax>.Enumerator enumerator = list.Parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!CanReuseParameter(enumerator.Current))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool CanReuseBracketedParameterList(Microsoft.CodeAnalysis.CSharp.Syntax.BracketedParameterListSyntax list)
        {
            if (list == null)
            {
                return false;
            }
            if (list.OpenBracketToken.IsMissing)
            {
                return false;
            }
            if (list.CloseBracketToken.IsMissing)
            {
                return false;
            }
            SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax>.Enumerator enumerator = list.Parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!CanReuseParameter(enumerator.Current))
                {
                    return false;
                }
            }
            return true;
        }

        private void ParseParameterList(out SyntaxToken open, SeparatedSyntaxListBuilder<ParameterSyntax> nodes, out SyntaxToken close, SyntaxKind openKind, SyntaxKind closeKind)
        {
            open = EatToken(openKind);
            TerminatorState termState = _termState;
            _termState |= TerminatorState.IsEndOfParameterList;
            if (base.CurrentToken.Kind != closeKind)
            {
                do
                {
                    if (!IsPossibleParameter() && base.CurrentToken.Kind != SyntaxKind.CommaToken)
                    {
                        continue;
                    }
                    ParameterSyntax node = ParseParameter();
                    nodes.Add(node);
                    while (base.CurrentToken.Kind != closeKind)
                    {
                        if (base.CurrentToken.Kind == SyntaxKind.CommaToken || IsPossibleParameter())
                        {
                            nodes.AddSeparator(EatToken(SyntaxKind.CommaToken));
                            node = ParseParameter();
                            if (node.IsMissing && IsPossibleParameter())
                            {
                                node = AddTrailingSkippedSyntax(node, EatToken());
                            }
                            nodes.Add(node);
                        }
                        else if (SkipBadParameterListTokens(ref open, nodes, SyntaxKind.CommaToken, closeKind) == PostSkipAction.Abort)
                        {
                            break;
                        }
                    }
                    break;
                }
                while (SkipBadParameterListTokens(ref open, nodes, SyntaxKind.IdentifierToken, closeKind) == PostSkipAction.Continue);
            }
            _termState = termState;
            close = EatToken(closeKind);
        }

        private bool IsEndOfParameterList()
        {
            if (base.CurrentToken.Kind != SyntaxKind.CloseParenToken)
            {
                return base.CurrentToken.Kind == SyntaxKind.CloseBracketToken;
            }
            return true;
        }

        private PostSkipAction SkipBadParameterListTokens(ref SyntaxToken open, SeparatedSyntaxListBuilder<ParameterSyntax> list, SyntaxKind expected, SyntaxKind closeKind)
        {
            return SkipBadSeparatedListTokensWithExpectedKind(ref open, list, (LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleParameter(), (LanguageParser p) => p.CurrentToken.Kind == closeKind || p.IsTerminator(), expected);
        }

        private bool IsPossibleParameter()
        {
            SyntaxKind kind = base.CurrentToken.Kind;
            if (kind <= SyntaxKind.OpenBracketToken)
            {
                if (kind == SyntaxKind.OpenParenToken || kind == SyntaxKind.OpenBracketToken)
                {
                    goto IL_0048;
                }
            }
            else
            {
                if (kind == SyntaxKind.ArgListKeyword)
                {
                    goto IL_0048;
                }
                if (kind != SyntaxKind.DelegateKeyword)
                {
                    if (kind == SyntaxKind.IdentifierToken)
                    {
                        return IsTrueIdentifier();
                    }
                }
                else if (IsFunctionPointerStart())
                {
                    goto IL_0048;
                }
            }
            if (!IsParameterModifier(base.CurrentToken.Kind))
            {
                return IsPredefinedType(base.CurrentToken.Kind);
            }
            return true;
        IL_0048:
            return true;
        }

        private static bool CanReuseParameter(Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax parameter)
        {
            if (parameter == null)
            {
                return false;
            }
            if (parameter.Default != null)
            {
                return false;
            }
            Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode parent = parameter.Parent;
            if (parent != null)
            {
                if (parent.Kind() == SyntaxKind.SimpleLambdaExpression)
                {
                    return false;
                }
                Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode parent2 = parent.Parent;
                if (parent2 != null && parent2.Kind() == SyntaxKind.ParenthesizedLambdaExpression)
                {
                    return false;
                }
            }
            return true;
        }

        private ParameterSyntax ParseParameter()
        {
            if (IsIncrementalAndFactoryContextMatches && CanReuseParameter(base.CurrentNode as Microsoft.CodeAnalysis.CSharp.Syntax.ParameterSyntax))
            {
                return (ParameterSyntax)EatNode();
            }
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists = ParseAttributeDeclarations();
            SyntaxListBuilder syntaxListBuilder = _pool.Allocate();
            try
            {
                ParseParameterModifiers(syntaxListBuilder);
                TypeSyntax type;
                SyntaxToken syntaxToken;
                if (base.CurrentToken.Kind != SyntaxKind.ArgListKeyword)
                {
                    type = ParseType(ParseTypeMode.Parameter);
                    syntaxToken = ParseIdentifierToken();
                    if (base.CurrentToken.Kind == SyntaxKind.OpenBracketToken && PeekToken(1).Kind == SyntaxKind.CloseBracketToken)
                    {
                        SyntaxToken node = EatToken();
                        SyntaxToken child = EatToken();
                        node = AddError(node, ErrorCode.ERR_BadArraySyntax);
                        syntaxToken = AddTrailingSkippedSyntax(syntaxToken, SyntaxList.List(node, child));
                    }
                }
                else
                {
                    type = null;
                    syntaxToken = EatToken(SyntaxKind.ArgListKeyword);
                }
                EqualsValueClauseSyntax @default = null;
                if (base.CurrentToken.Kind == SyntaxKind.EqualsToken)
                {
                    SyntaxToken equalsToken = EatToken(SyntaxKind.EqualsToken);
                    ExpressionSyntax value = ParseExpressionCore();
                    @default = _syntaxFactory.EqualsValueClause(equalsToken, value);
                    @default = CheckFeatureAvailability(@default, MessageID.IDS_FeatureOptionalParameter);
                }
                return _syntaxFactory.Parameter(attributeLists, syntaxListBuilder.ToList(), type, syntaxToken, @default);
            }
            finally
            {
                _pool.Free(syntaxListBuilder);
            }
        }

        private static bool IsParameterModifier(SyntaxKind kind, bool isFunctionPointerParameter = false)
        {
            SyntaxKind syntaxKind = kind;
            if (syntaxKind <= SyntaxKind.InKeyword)
            {
                if (syntaxKind != SyntaxKind.ReadOnlyKeyword)
                {
                    if (syntaxKind - 8360 <= (SyntaxKind)2)
                    {
                        goto IL_0033;
                    }
                }
                else if (isFunctionPointerParameter)
                {
                    goto IL_0033;
                }
            }
            else if (syntaxKind == SyntaxKind.ParamsKeyword || syntaxKind == SyntaxKind.ThisKeyword)
            {
                goto IL_0033;
            }
            return false;
        IL_0033:
            return true;
        }

        private void ParseParameterModifiers(SyntaxListBuilder modifiers, bool isFunctionPointerParameter = false)
        {
            while (IsParameterModifier(base.CurrentToken.Kind, isFunctionPointerParameter))
            {
                SyntaxToken syntaxToken = EatToken();
                switch (syntaxToken.Kind)
                {
                    case SyntaxKind.ThisKeyword:
                        syntaxToken = CheckFeatureAvailability(syntaxToken, MessageID.IDS_FeatureExtensionMethod);
                        if (base.CurrentToken.Kind == SyntaxKind.RefKeyword || base.CurrentToken.Kind == SyntaxKind.InKeyword)
                        {
                            syntaxToken = CheckFeatureAvailability(syntaxToken, MessageID.IDS_FeatureRefExtensionMethods);
                        }
                        break;
                    case SyntaxKind.RefKeyword:
                        if (base.CurrentToken.Kind == SyntaxKind.ThisKeyword)
                        {
                            syntaxToken = CheckFeatureAvailability(syntaxToken, MessageID.IDS_FeatureRefExtensionMethods);
                        }
                        break;
                    case SyntaxKind.InKeyword:
                        syntaxToken = CheckFeatureAvailability(syntaxToken, MessageID.IDS_FeatureReadOnlyReferences);
                        if (base.CurrentToken.Kind == SyntaxKind.ThisKeyword)
                        {
                            syntaxToken = CheckFeatureAvailability(syntaxToken, MessageID.IDS_FeatureRefExtensionMethods);
                        }
                        break;
                }
                modifiers.Add(syntaxToken);
            }
        }

        private FieldDeclarationSyntax ParseFixedSizeBufferDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers, SyntaxKind parentKind)
        {
            SyntaxToken node = EatToken();
            node = CheckFeatureAvailability(node, MessageID.IDS_FeatureFixedBuffer);
            modifiers.Add(node);
            TypeSyntax type = ParseType();
            TerminatorState termState = _termState;
            _termState |= TerminatorState.IsEndOfFieldDeclaration;
            SeparatedSyntaxListBuilder<VariableDeclaratorSyntax> builder = _pool.AllocateSeparated<VariableDeclaratorSyntax>();
            try
            {
                ParseVariableDeclarators(type, VariableFlags.Fixed, builder, parentKind);
                SyntaxToken semicolonToken = EatToken(SyntaxKind.SemicolonToken);
                return _syntaxFactory.FieldDeclaration(attributes, modifiers.ToList(), _syntaxFactory.VariableDeclaration(type, builder), semicolonToken);
            }
            finally
            {
                _termState = termState;
                _pool.Free(in builder);
            }
        }

        private MemberDeclarationSyntax ParseEventDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers, SyntaxKind parentKind)
        {
            SyntaxToken eventToken = EatToken();
            TypeSyntax type = ParseType();
            if (IsFieldDeclaration(isEvent: true))
            {
                return ParseEventFieldDeclaration(attributes, modifiers, eventToken, type, parentKind);
            }
            return ParseEventDeclarationWithAccessors(attributes, modifiers, eventToken, type);
        }

        private EventDeclarationSyntax ParseEventDeclarationWithAccessors(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers, SyntaxToken eventToken, TypeSyntax type)
        {
            ParseMemberName(out var explicitInterfaceOpt, out var identifierOrThisOpt, out var typeParameterListOpt, isEvent: true);
            CheckForVersionSpecificModifiers(modifiers, SyntaxKind.ReadOnlyKeyword, MessageID.IDS_FeatureReadOnlyMembers);
            if (explicitInterfaceOpt != null && base.CurrentToken.Kind != SyntaxKind.OpenBraceToken && base.CurrentToken.Kind != SyntaxKind.SemicolonToken)
            {
                SyntaxToken identifier = ((identifierOrThisOpt == null) ? CreateMissingIdentifierToken() : identifierOrThisOpt);
                AccessorListSyntax accessorList = _syntaxFactory.AccessorList(SyntaxFactory.MissingToken(SyntaxKind.OpenBraceToken), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AccessorDeclarationSyntax>), SyntaxFactory.MissingToken(SyntaxKind.CloseBraceToken));
                return _syntaxFactory.EventDeclaration(attributes, modifiers.ToList(), eventToken, type, explicitInterfaceOpt, identifier, accessorList, null);
            }
            SyntaxToken syntaxToken = ((identifierOrThisOpt == null) ? CreateMissingIdentifierToken() : ((identifierOrThisOpt.Kind == SyntaxKind.IdentifierToken) ? identifierOrThisOpt : ConvertToMissingWithTrailingTrivia(identifierOrThisOpt, SyntaxKind.IdentifierToken)));
            if (syntaxToken.IsMissing && !type.IsMissing)
            {
                syntaxToken = AddError(syntaxToken, ErrorCode.ERR_IdentifierExpected);
            }
            if (typeParameterListOpt != null)
            {
                syntaxToken = AddTrailingSkippedSyntax(syntaxToken, typeParameterListOpt);
                syntaxToken = AddError(syntaxToken, ErrorCode.ERR_UnexpectedGenericName);
            }
            AccessorListSyntax accessorList2 = null;
            SyntaxToken semicolonToken = null;
            if (explicitInterfaceOpt != null && base.CurrentToken.Kind == SyntaxKind.SemicolonToken)
            {
                semicolonToken = EatToken(SyntaxKind.SemicolonToken);
            }
            else
            {
                accessorList2 = ParseAccessorList(isEvent: true);
            }
            EventDeclarationSyntax decl = _syntaxFactory.EventDeclaration(attributes, modifiers.ToList(), eventToken, type, explicitInterfaceOpt, syntaxToken, accessorList2, semicolonToken);
            return EatUnexpectedTrailingSemicolon(decl);
        }

        private TNode EatUnexpectedTrailingSemicolon<TNode>(TNode decl) where TNode : CSharpSyntaxNode
        {
            if (base.CurrentToken.Kind == SyntaxKind.SemicolonToken)
            {
                SyntaxToken node = EatToken();
                node = AddError(node, ErrorCode.ERR_UnexpectedSemicolon);
                decl = AddTrailingSkippedSyntax(decl, node);
            }
            return decl;
        }

        private FieldDeclarationSyntax ParseNormalFieldDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers, TypeSyntax type, SyntaxKind parentKind)
        {
            TerminatorState termState = _termState;
            _termState |= TerminatorState.IsEndOfFieldDeclaration;
            SeparatedSyntaxListBuilder<VariableDeclaratorSyntax> builder = _pool.AllocateSeparated<VariableDeclaratorSyntax>();
            try
            {
                ParseVariableDeclarators(type, 0, builder, parentKind);
                SyntaxToken semicolonToken = EatToken(SyntaxKind.SemicolonToken);
                return _syntaxFactory.FieldDeclaration(attributes, modifiers.ToList(), _syntaxFactory.VariableDeclaration(type, builder), semicolonToken);
            }
            finally
            {
                _termState = termState;
                _pool.Free(in builder);
            }
        }

        private EventFieldDeclarationSyntax ParseEventFieldDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers, SyntaxToken eventToken, TypeSyntax type, SyntaxKind parentKind)
        {
            TerminatorState termState = _termState;
            _termState |= TerminatorState.IsEndOfFieldDeclaration;
            SeparatedSyntaxListBuilder<VariableDeclaratorSyntax> builder = _pool.AllocateSeparated<VariableDeclaratorSyntax>();
            try
            {
                ParseVariableDeclarators(type, 0, builder, parentKind);
                if (base.CurrentToken.Kind == SyntaxKind.DotToken)
                {
                    eventToken = AddError(eventToken, ErrorCode.ERR_ExplicitEventFieldImpl);
                }
                SyntaxToken semicolonToken = EatToken(SyntaxKind.SemicolonToken);
                return _syntaxFactory.EventFieldDeclaration(attributes, modifiers.ToList(), eventToken, _syntaxFactory.VariableDeclaration(type, builder), semicolonToken);
            }
            finally
            {
                _termState = termState;
                _pool.Free(in builder);
            }
        }

        private bool IsEndOfFieldDeclaration()
        {
            return base.CurrentToken.Kind == SyntaxKind.SemicolonToken;
        }

        private void ParseVariableDeclarators(TypeSyntax type, VariableFlags flags, SeparatedSyntaxListBuilder<VariableDeclaratorSyntax> variables, SyntaxKind parentKind)
        {
            ParseVariableDeclarators(type, flags, variables, parentKind switch
            {
                SyntaxKind.CompilationUnit => base.IsScript,
                SyntaxKind.NamespaceDeclaration => false,
                _ => true,
            }, allowLocalFunctions: false, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>), out var _);
        }

        private void ParseVariableDeclarators(TypeSyntax type, VariableFlags flags, SeparatedSyntaxListBuilder<VariableDeclaratorSyntax> variables, bool variableDeclarationsExpected, bool allowLocalFunctions, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> mods, out LocalFunctionStatementSyntax localFunction)
        {
            variables.Add(ParseVariableDeclarator(type, flags, isFirst: true, allowLocalFunctions, attributes, mods, out localFunction));
            if (localFunction != null)
            {
                return;
            }
            while (base.CurrentToken.Kind != SyntaxKind.SemicolonToken)
            {
                if (base.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    variables.AddSeparator(EatToken(SyntaxKind.CommaToken));
                    variables.Add(ParseVariableDeclarator(type, flags, isFirst: false, allowLocalFunctions: false, attributes, mods, out localFunction));
                }
                else if (!variableDeclarationsExpected || SkipBadVariableListTokens(variables, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                {
                    break;
                }
            }
        }

        private PostSkipAction SkipBadVariableListTokens(SeparatedSyntaxListBuilder<VariableDeclaratorSyntax> list, SyntaxKind expected)
        {
            CSharpSyntaxNode startToken = null;
            return SkipBadSeparatedListTokensWithExpectedKind(ref startToken, list, (LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CommaToken, (LanguageParser p) => p.CurrentToken.Kind == SyntaxKind.SemicolonToken || p.IsTerminator(), expected);
        }

        private static SyntaxTokenList GetOriginalModifiers(Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode decl)
        {
            if (decl != null)
            {
                switch (decl.Kind())
                {
                    case SyntaxKind.FieldDeclaration:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.FieldDeclarationSyntax)decl).Modifiers;
                    case SyntaxKind.MethodDeclaration:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.MethodDeclarationSyntax)decl).Modifiers;
                    case SyntaxKind.ConstructorDeclaration:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.ConstructorDeclarationSyntax)decl).Modifiers;
                    case SyntaxKind.DestructorDeclaration:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.DestructorDeclarationSyntax)decl).Modifiers;
                    case SyntaxKind.PropertyDeclaration:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.PropertyDeclarationSyntax)decl).Modifiers;
                    case SyntaxKind.EventFieldDeclaration:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.EventFieldDeclarationSyntax)decl).Modifiers;
                    case SyntaxKind.GetAccessorDeclaration:
                    case SyntaxKind.SetAccessorDeclaration:
                    case SyntaxKind.AddAccessorDeclaration:
                    case SyntaxKind.RemoveAccessorDeclaration:
                    case SyntaxKind.InitAccessorDeclaration:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.AccessorDeclarationSyntax)decl).Modifiers;
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.StructDeclaration:
                    case SyntaxKind.InterfaceDeclaration:
                    case SyntaxKind.RecordDeclaration:
                    case SyntaxKind.RecordStructDeclaration:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax)decl).Modifiers;
                    case SyntaxKind.DelegateDeclaration:
                        return ((Microsoft.CodeAnalysis.CSharp.Syntax.DelegateDeclarationSyntax)decl).Modifiers;
                }
            }
            return default(SyntaxTokenList);
        }

        private static bool WasFirstVariable(Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax variable)
        {
            if (GetOldParent(variable) is Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclarationSyntax variableDeclarationSyntax)
            {
                return variableDeclarationSyntax.Variables[0] == variable;
            }
            return false;
        }

        private static VariableFlags GetOriginalVariableFlags(Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax old)
        {
            Microsoft.CodeAnalysis.CSharp.CSharpSyntaxNode oldParent = GetOldParent(old);
            SyntaxTokenList originalModifiers = GetOriginalModifiers(oldParent);
            VariableFlags variableFlags = 0;
            if (originalModifiers.Any(SyntaxKind.FixedKeyword))
            {
                variableFlags |= VariableFlags.Fixed;
            }
            if (originalModifiers.Any(SyntaxKind.ConstKeyword))
            {
                variableFlags |= VariableFlags.Const;
            }
            if (oldParent != null && (oldParent.Kind() == SyntaxKind.VariableDeclaration || oldParent.Kind() == SyntaxKind.LocalDeclarationStatement))
            {
                variableFlags |= VariableFlags.Local;
            }
            return variableFlags;
        }

        private static bool CanReuseVariableDeclarator(Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax old, VariableFlags flags, bool isFirst)
        {
            if (old == null)
            {
                return false;
            }
            SyntaxKind syntaxKind;
            if (flags == GetOriginalVariableFlags(old) && isFirst == WasFirstVariable(old) && old.Initializer == null && (syntaxKind = GetOldParent(old).Kind()) != SyntaxKind.VariableDeclaration)
            {
                return syntaxKind != SyntaxKind.LocalDeclarationStatement;
            }
            return false;
        }

        private VariableDeclaratorSyntax ParseVariableDeclarator(TypeSyntax parentType, VariableFlags flags, bool isFirst, bool allowLocalFunctions, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> mods, out LocalFunctionStatementSyntax localFunction, bool isExpressionContext = false)
        {
            if (IsIncrementalAndFactoryContextMatches && CanReuseVariableDeclarator(base.CurrentNode as Microsoft.CodeAnalysis.CSharp.Syntax.VariableDeclaratorSyntax, flags, isFirst))
            {
                localFunction = null;
                return (VariableDeclaratorSyntax)EatNode();
            }
            if (!isExpressionContext)
            {
                ResetPoint state = GetResetPoint();
                try
                {
                    SyntaxKind kind = base.CurrentToken.Kind;
                    if (kind == SyntaxKind.IdentifierToken && !parentType.IsMissing && parentType.GetLastToken().TrailingTrivia.Any(8539))
                    {
                        GetDiagnosticSpanForMissingToken(out var offset, out var width);
                        EatToken();
                        kind = base.CurrentToken.Kind;
                        bool flag = kind != SyntaxKind.EqualsToken && SyntaxFacts.IsBinaryExpressionOperatorToken(kind);
                        if ((kind == SyntaxKind.DotToken || kind == SyntaxKind.OpenParenToken || kind == SyntaxKind.MinusGreaterThanToken || flag) && ((kind != SyntaxKind.OpenParenToken && kind != SyntaxKind.LessThanToken) || !IsLocalFunctionAfterIdentifier()))
                        {
                            SyntaxToken node = CreateMissingIdentifierToken();
                            node = AddError(node, offset, width, ErrorCode.ERR_IdentifierExpected);
                            localFunction = null;
                            return _syntaxFactory.VariableDeclarator(node, null, null);
                        }
                    }
                }
                finally
                {
                    Reset(ref state);
                    Release(ref state);
                }
            }
            SyntaxToken syntaxToken = ParseIdentifierToken();
            BracketedArgumentListSyntax bracketedArgumentListSyntax = null;
            EqualsValueClauseSyntax initializer = null;
            TerminatorState termState = _termState;
            bool flag2 = (flags & VariableFlags.Fixed) != 0;
            bool flag3 = (flags & VariableFlags.Const) != 0;
            bool flag4 = (flags & VariableFlags.Local) != 0;
            if (!isFirst && IsTrueIdentifier())
            {
                syntaxToken = AddError(syntaxToken, ErrorCode.ERR_MultiTypeInDeclaration);
            }
            SyntaxKind kind2 = base.CurrentToken.Kind;
            if (kind2 <= SyntaxKind.EqualsToken)
            {
                if (kind2 == SyntaxKind.OpenParenToken)
                {
                    if (allowLocalFunctions && isFirst)
                    {
                        localFunction = TryParseLocalFunctionStatementBody(attributes, mods, parentType, syntaxToken);
                        if (localFunction != null)
                        {
                            return null;
                        }
                    }
                    _termState |= TerminatorState.IsPossibleEndOfVariableDeclaration;
                    bracketedArgumentListSyntax = ParseBracketedArgumentList();
                    _termState = termState;
                    bracketedArgumentListSyntax = AddError(bracketedArgumentListSyntax, ErrorCode.ERR_BadVarDecl);
                    goto IL_0415;
                }
                if (kind2 == SyntaxKind.EqualsToken)
                {
                    goto IL_01cf;
                }
            }
            else
            {
                if (kind2 == SyntaxKind.OpenBracketToken)
                {
                    goto IL_02b5;
                }
                if (kind2 == SyntaxKind.LessThanToken && allowLocalFunctions && isFirst)
                {
                    localFunction = TryParseLocalFunctionStatementBody(attributes, mods, parentType, syntaxToken);
                    if (localFunction != null)
                    {
                        return null;
                    }
                }
            }
            goto IL_03e1;
        IL_02b5:
            _termState |= TerminatorState.IsPossibleEndOfVariableDeclaration;
            ArrayRankSpecifierSyntax arrayRankSpecifierSyntax = ParseArrayRankSpecifier(out bool sawNonOmittedSize);
            _termState = termState;
            SyntaxToken openBracketToken = arrayRankSpecifierSyntax.OpenBracketToken;
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ExpressionSyntax> sizes = arrayRankSpecifierSyntax.Sizes;
            SyntaxToken syntaxToken2 = arrayRankSpecifierSyntax.CloseBracketToken;
            if (flag2 && !sawNonOmittedSize)
            {
                syntaxToken2 = AddError(syntaxToken2, ErrorCode.ERR_ValueExpected);
            }
            SeparatedSyntaxListBuilder<ArgumentSyntax> builder = _pool.AllocateSeparated<ArgumentSyntax>();
            try
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode>.Enumerator enumerator = sizes.GetWithSeparators().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    GreenNode current = enumerator.Current;
                    ExpressionSyntax expressionSyntax = current as ExpressionSyntax;
                    if (expressionSyntax != null)
                    {
                        bool flag5 = expressionSyntax.Kind == SyntaxKind.OmittedArraySizeExpression;
                        if (!flag2 && !flag5)
                        {
                            expressionSyntax = AddError(expressionSyntax, ErrorCode.ERR_ArraySizeInDeclaration);
                        }
                        builder.Add(_syntaxFactory.Argument(null, null, expressionSyntax));
                    }
                    else
                    {
                        builder.AddSeparator((SyntaxToken)current);
                    }
                }
                bracketedArgumentListSyntax = _syntaxFactory.BracketedArgumentList(openBracketToken, builder, syntaxToken2);
                if (!flag2)
                {
                    bracketedArgumentListSyntax = AddError(bracketedArgumentListSyntax, ErrorCode.ERR_CStyleArray);
                    if (base.CurrentToken.Kind == SyntaxKind.EqualsToken)
                    {
                        goto IL_01cf;
                    }
                }
            }
            finally
            {
                _pool.Free(in builder);
            }
            goto IL_0415;
        IL_03e1:
            if (flag3)
            {
                syntaxToken = AddError(syntaxToken, ErrorCode.ERR_ConstValueRequired);
            }
            else if (flag2)
            {
                if (parentType.Kind != SyntaxKind.ArrayType)
                {
                    goto IL_02b5;
                }
                syntaxToken = AddError(syntaxToken, ErrorCode.ERR_FixedDimsRequired);
            }
            goto IL_0415;
        IL_0415:
            localFunction = null;
            return _syntaxFactory.VariableDeclarator(syntaxToken, bracketedArgumentListSyntax, initializer);
        IL_01cf:
            if (flag2)
            {
                goto IL_03e1;
            }
            SyntaxToken equalsToken = EatToken();
            SyntaxToken syntaxToken3 = null;
            if (flag4 && !flag3 && base.CurrentToken.Kind == SyntaxKind.RefKeyword)
            {
                syntaxToken3 = EatToken();
                syntaxToken3 = CheckFeatureAvailability(syntaxToken3, MessageID.IDS_FeatureRefLocalsReturns);
            }
            ExpressionSyntax expressionSyntax2 = ParseVariableInitializer();
            if (syntaxToken3 != null)
            {
                expressionSyntax2 = _syntaxFactory.RefExpression(syntaxToken3, expressionSyntax2);
            }
            initializer = _syntaxFactory.EqualsValueClause(equalsToken, expressionSyntax2);
            goto IL_0415;
        }

        private bool IsLocalFunctionAfterIdentifier()
        {
            ResetPoint state = GetResetPoint();
            try
            {
                ParseTypeParameterList();
                if (!ParseParenthesizedParameterList().IsMissing && (base.CurrentToken.Kind == SyntaxKind.OpenBraceToken || base.CurrentToken.Kind == SyntaxKind.EqualsGreaterThanToken || base.CurrentToken.ContextualKind == SyntaxKind.WhereKeyword))
                {
                    return true;
                }
                return false;
            }
            finally
            {
                Reset(ref state);
                Release(ref state);
            }
        }

        private bool IsPossibleEndOfVariableDeclaration()
        {
            SyntaxKind kind = base.CurrentToken.Kind;
            if (kind == SyntaxKind.SemicolonToken || kind == SyntaxKind.CommaToken)
            {
                return true;
            }
            return false;
        }

        private ExpressionSyntax ParseVariableInitializer()
        {
            if (base.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
            {
                return ParseArrayInitializer();
            }
            return ParseExpressionCore();
        }

        private bool IsPossibleVariableInitializer()
        {
            if (base.CurrentToken.Kind != SyntaxKind.OpenBraceToken)
            {
                return IsPossibleExpression();
            }
            return true;
        }

        private FieldDeclarationSyntax ParseConstantFieldDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers, SyntaxKind parentKind)
        {
            SyntaxToken item = EatToken(SyntaxKind.ConstKeyword);
            modifiers.Add(item);
            TypeSyntax type = ParseType();
            SeparatedSyntaxListBuilder<VariableDeclaratorSyntax> builder = _pool.AllocateSeparated<VariableDeclaratorSyntax>();
            try
            {
                ParseVariableDeclarators(type, VariableFlags.Const, builder, parentKind);
                SyntaxToken semicolonToken = EatToken(SyntaxKind.SemicolonToken);
                return _syntaxFactory.FieldDeclaration(attributes, modifiers.ToList(), _syntaxFactory.VariableDeclaration(type, builder), semicolonToken);
            }
            finally
            {
                _pool.Free(in builder);
            }
        }

        private DelegateDeclarationSyntax ParseDelegateDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers)
        {
            SyntaxToken delegateKeyword = EatToken(SyntaxKind.DelegateKeyword);
            TypeSyntax returnType = ParseReturnType();
            TerminatorState termState = _termState;
            _termState |= TerminatorState.IsEndOfMethodSignature;
            SyntaxToken identifier = ParseIdentifierToken();
            TypeParameterListSyntax typeParameterList = ParseTypeParameterList();
            ParameterListSyntax parameterList = ParseParenthesizedParameterList();
            SyntaxListBuilder<TypeParameterConstraintClauseSyntax> syntaxListBuilder = default(SyntaxListBuilder<TypeParameterConstraintClauseSyntax>);
            try
            {
                if (base.CurrentToken.ContextualKind == SyntaxKind.WhereKeyword)
                {
                    syntaxListBuilder = _pool.Allocate<TypeParameterConstraintClauseSyntax>();
                    ParseTypeParameterConstraintClauses(syntaxListBuilder);
                }
                _termState = termState;
                SyntaxToken semicolonToken = EatToken(SyntaxKind.SemicolonToken);
                return _syntaxFactory.DelegateDeclaration(attributes, modifiers.ToList(), delegateKeyword, returnType, identifier, typeParameterList, parameterList, syntaxListBuilder, semicolonToken);
            }
            finally
            {
                if (!syntaxListBuilder.IsNull)
                {
                    _pool.Free(syntaxListBuilder);
                }
            }
        }

        private EnumDeclarationSyntax ParseEnumDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxListBuilder modifiers)
        {
            SyntaxToken enumKeyword = EatToken(SyntaxKind.EnumKeyword);
            SyntaxToken syntaxToken = ParseIdentifierToken();
            TypeParameterListSyntax typeParameterListSyntax = ParseTypeParameterList();
            if (typeParameterListSyntax != null)
            {
                syntaxToken = AddTrailingSkippedSyntax(syntaxToken, typeParameterListSyntax);
                syntaxToken = AddError(syntaxToken, ErrorCode.ERR_UnexpectedGenericName);
            }
            BaseListSyntax baseList = null;
            if (base.CurrentToken.Kind == SyntaxKind.ColonToken)
            {
                SyntaxToken colonToken = EatToken(SyntaxKind.ColonToken);
                TypeSyntax type = ParseType();
                SeparatedSyntaxListBuilder<BaseTypeSyntax> builder = _pool.AllocateSeparated<BaseTypeSyntax>();
                builder.Add(_syntaxFactory.SimpleBaseType(type));
                baseList = _syntaxFactory.BaseList(colonToken, builder);
                _pool.Free(in builder);
            }
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<EnumMemberDeclarationSyntax> members = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<EnumMemberDeclarationSyntax>);
            SyntaxToken openBrace = EatToken(SyntaxKind.OpenBraceToken);
            if (!openBrace.IsMissing)
            {
                SeparatedSyntaxListBuilder<EnumMemberDeclarationSyntax> item = _pool.AllocateSeparated<EnumMemberDeclarationSyntax>();
                try
                {
                    ParseEnumMemberDeclarations(ref openBrace, item);
                    members = item.ToList();
                }
                finally
                {
                    _pool.Free(in item);
                }
            }
            SyntaxToken closeBraceToken = EatToken(SyntaxKind.CloseBraceToken);
            SyntaxToken semicolonToken = TryEatToken(SyntaxKind.SemicolonToken);
            return _syntaxFactory.EnumDeclaration(attributes, modifiers.ToList(), enumKeyword, syntaxToken, baseList, openBrace, members, closeBraceToken, semicolonToken);
        }

        private void ParseEnumMemberDeclarations(ref SyntaxToken openBrace, SeparatedSyntaxListBuilder<EnumMemberDeclarationSyntax> members)
        {
            if (base.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
            {
                return;
            }
            while (true)
            {
                if (IsPossibleEnumMemberDeclaration() || base.CurrentToken.Kind == SyntaxKind.CommaToken || base.CurrentToken.Kind == SyntaxKind.SemicolonToken)
                {
                    members.Add(ParseEnumMemberDeclaration());
                    while (true)
                    {
                        if (base.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                        {
                            return;
                        }
                        if (base.CurrentToken.Kind == SyntaxKind.CommaToken || base.CurrentToken.Kind == SyntaxKind.SemicolonToken || IsPossibleEnumMemberDeclaration())
                        {
                            if (base.CurrentToken.Kind == SyntaxKind.SemicolonToken)
                            {
                                members.AddSeparator(EatTokenWithPrejudice(SyntaxKind.CommaToken));
                            }
                            else
                            {
                                members.AddSeparator(EatToken(SyntaxKind.CommaToken));
                            }
                            if (base.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                            {
                                return;
                            }
                            if (!IsPossibleEnumMemberDeclaration())
                            {
                                break;
                            }
                            members.Add(ParseEnumMemberDeclaration());
                        }
                        else if (SkipBadEnumMemberListTokens(ref openBrace, members, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                        {
                            return;
                        }
                    }
                }
                else if (SkipBadEnumMemberListTokens(ref openBrace, members, SyntaxKind.IdentifierToken) != 0)
                {
                    break;
                }
            }
        }

        private PostSkipAction SkipBadEnumMemberListTokens(ref SyntaxToken openBrace, SeparatedSyntaxListBuilder<EnumMemberDeclarationSyntax> list, SyntaxKind expected)
        {
            return SkipBadSeparatedListTokensWithExpectedKind(ref openBrace, list, (LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CommaToken && p.CurrentToken.Kind != SyntaxKind.SemicolonToken && !p.IsPossibleEnumMemberDeclaration(), (LanguageParser p) => p.CurrentToken.Kind == SyntaxKind.CloseBraceToken || p.IsTerminator(), expected);
        }

        private EnumMemberDeclarationSyntax ParseEnumMemberDeclaration()
        {
            if (IsIncrementalAndFactoryContextMatches && base.CurrentNodeKind == SyntaxKind.EnumMemberDeclaration)
            {
                return (EnumMemberDeclarationSyntax)EatNode();
            }
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists = ParseAttributeDeclarations();
            SyntaxToken identifier = ParseIdentifierToken();
            EqualsValueClauseSyntax equalsValue = null;
            if (base.CurrentToken.Kind == SyntaxKind.EqualsToken)
            {
                SyntaxToken equalsToken = EatToken(SyntaxKind.EqualsToken);
                ExpressionSyntax value = ((base.CurrentToken.Kind != SyntaxKind.CommaToken && base.CurrentToken.Kind != SyntaxKind.CloseBraceToken) ? ParseExpressionCore() : ParseIdentifierName(ErrorCode.ERR_ConstantExpected));
                equalsValue = _syntaxFactory.EqualsValueClause(equalsToken, value);
            }
            return _syntaxFactory.EnumMemberDeclaration(attributeLists, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>), identifier, equalsValue);
        }

        private bool IsPossibleEnumMemberDeclaration()
        {
            if (base.CurrentToken.Kind != SyntaxKind.OpenBracketToken)
            {
                return IsTrueIdentifier();
            }
            return true;
        }

        private bool IsDotOrColonColon()
        {
            if (base.CurrentToken.Kind != SyntaxKind.DotToken)
            {
                return base.CurrentToken.Kind == SyntaxKind.ColonColonToken;
            }
            return true;
        }

        public NameSyntax ParseName()
        {
            return ParseQualifiedName();
        }

        private IdentifierNameSyntax CreateMissingIdentifierName()
        {
            return _syntaxFactory.IdentifierName(CreateMissingIdentifierToken());
        }

        private static SyntaxToken CreateMissingIdentifierToken()
        {
            return SyntaxFactory.MissingToken(SyntaxKind.IdentifierToken);
        }

        private bool IsTrueIdentifier()
        {
            if (base.CurrentToken.Kind == SyntaxKind.IdentifierToken && !IsCurrentTokenPartialKeywordOfPartialMethodOrType() && !IsCurrentTokenQueryKeywordInQuery() && !IsCurrentTokenWhereOfConstraintClause())
            {
                return true;
            }
            return false;
        }

        private bool IsTrueIdentifier(SyntaxToken token)
        {
            if (token.Kind == SyntaxKind.IdentifierToken)
            {
                if (IsInQuery)
                {
                    return !IsTokenQueryContextualKeyword(token);
                }
                return true;
            }
            return false;
        }

        private IdentifierNameSyntax ParseIdentifierName(ErrorCode code = ErrorCode.ERR_IdentifierExpected)
        {
            if (IsIncrementalAndFactoryContextMatches && base.CurrentNodeKind == SyntaxKind.IdentifierName && !SyntaxFacts.IsContextualKeyword(((Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax)base.CurrentNode).Identifier.Kind()))
            {
                return (IdentifierNameSyntax)EatNode();
            }
            return SyntaxFactory.IdentifierName(ParseIdentifierToken(code));
        }

        private SyntaxToken ParseIdentifierToken(ErrorCode code = ErrorCode.ERR_IdentifierExpected)
        {
            if (base.CurrentToken.Kind == SyntaxKind.IdentifierToken)
            {
                if (IsCurrentTokenPartialKeywordOfPartialMethodOrType() || IsCurrentTokenQueryKeywordInQuery())
                {
                    SyntaxToken node = CreateMissingIdentifierToken();
                    return AddError(node, ErrorCode.ERR_InvalidExprTerm, base.CurrentToken.Text);
                }
                SyntaxToken syntaxToken = EatToken();
                if (IsInAsync && syntaxToken.ContextualKind == SyntaxKind.AwaitKeyword)
                {
                    syntaxToken = AddError(syntaxToken, ErrorCode.ERR_BadAwaitAsIdentifier);
                }
                return syntaxToken;
            }
            SyntaxToken node2 = CreateMissingIdentifierToken();
            return AddError(node2, code);
        }

        private bool IsCurrentTokenQueryKeywordInQuery()
        {
            if (IsInQuery)
            {
                return IsCurrentTokenQueryContextualKeyword;
            }
            return false;
        }

        private bool IsCurrentTokenPartialKeywordOfPartialMethodOrType()
        {
            if (base.CurrentToken.ContextualKind == SyntaxKind.PartialKeyword && (IsPartialType() || IsPartialMember()))
            {
                return true;
            }
            return false;
        }

        private TypeParameterListSyntax ParseTypeParameterList()
        {
            if (base.CurrentToken.Kind != SyntaxKind.LessThanToken)
            {
                return null;
            }
            TerminatorState termState = _termState;
            _termState |= TerminatorState.IsEndOfTypeParameterList;
            try
            {
                SeparatedSyntaxListBuilder<TypeParameterSyntax> builder = _pool.AllocateSeparated<TypeParameterSyntax>();
                SyntaxToken node = EatToken(SyntaxKind.LessThanToken);
                node = CheckFeatureAvailability(node, MessageID.IDS_FeatureGenerics);
                builder.Add(ParseTypeParameter());
                while (base.CurrentToken.Kind != SyntaxKind.GreaterThanToken && !IsCurrentTokenWhereOfConstraintClause())
                {
                    if (base.CurrentToken.Kind == SyntaxKind.CommaToken)
                    {
                        builder.AddSeparator(EatToken(SyntaxKind.CommaToken));
                        builder.Add(ParseTypeParameter());
                    }
                    else if (SkipBadTypeParameterListTokens(builder, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                    {
                        break;
                    }
                }
                SyntaxToken greaterThanToken = EatToken(SyntaxKind.GreaterThanToken);
                return _syntaxFactory.TypeParameterList(node, builder, greaterThanToken);
            }
            finally
            {
                _termState = termState;
            }
        }

        private PostSkipAction SkipBadTypeParameterListTokens(SeparatedSyntaxListBuilder<TypeParameterSyntax> list, SyntaxKind expected)
        {
            CSharpSyntaxNode startToken = null;
            return SkipBadSeparatedListTokensWithExpectedKind(ref startToken, list, (LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CommaToken, (LanguageParser p) => p.CurrentToken.Kind == SyntaxKind.GreaterThanToken || p.IsTerminator(), expected);
        }

        private TypeParameterSyntax ParseTypeParameter()
        {
            if (IsCurrentTokenWhereOfConstraintClause())
            {
                return _syntaxFactory.TypeParameter(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), null, AddError(CreateMissingIdentifierToken(), ErrorCode.ERR_IdentifierExpected));
            }
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>);
            if (base.CurrentToken.Kind == SyntaxKind.OpenBracketToken && PeekToken(1).Kind != SyntaxKind.CloseBracketToken)
            {
                TerminatorState termState = _termState;
                _termState = TerminatorState.IsEndOfTypeArgumentList;
                attributeLists = ParseAttributeDeclarations();
                _termState = termState;
            }
            SyntaxToken varianceKeyword = null;
            if (base.CurrentToken.Kind == SyntaxKind.InKeyword || base.CurrentToken.Kind == SyntaxKind.OutKeyword)
            {
                varianceKeyword = CheckFeatureAvailability(EatToken(), MessageID.IDS_FeatureTypeVariance);
            }
            return _syntaxFactory.TypeParameter(attributeLists, varianceKeyword, ParseIdentifierToken());
        }

        private SimpleNameSyntax ParseSimpleName(NameOptions options = NameOptions.None)
        {
            IdentifierNameSyntax identifierNameSyntax = ParseIdentifierName();
            if (identifierNameSyntax.Identifier.IsMissing)
            {
                return identifierNameSyntax;
            }
            SimpleNameSyntax result = identifierNameSyntax;
            if (base.CurrentToken.Kind == SyntaxKind.LessThanToken)
            {
                ResetPoint state = GetResetPoint();
                ScanTypeArgumentListKind scanTypeArgumentListKind = ScanTypeArgumentList(options);
                Reset(ref state);
                Release(ref state);
                if (scanTypeArgumentListKind == ScanTypeArgumentListKind.DefiniteTypeArgumentList || (scanTypeArgumentListKind == ScanTypeArgumentListKind.PossibleTypeArgumentList && (options & NameOptions.InTypeList) != 0))
                {
                    SeparatedSyntaxListBuilder<TypeSyntax> builder = _pool.AllocateSeparated<TypeSyntax>();
                    ParseTypeArgumentList(out var open, builder, out var close);
                    result = _syntaxFactory.GenericName(identifierNameSyntax.Identifier, _syntaxFactory.TypeArgumentList(open, builder, close));
                    _pool.Free(in builder);
                }
            }
            return result;
        }

        private ScanTypeArgumentListKind ScanTypeArgumentList(NameOptions options)
        {
            if (base.CurrentToken.Kind != SyntaxKind.LessThanToken)
            {
                return ScanTypeArgumentListKind.NotTypeArgumentList;
            }
            if ((options & NameOptions.InExpression) == 0)
            {
                return ScanTypeArgumentListKind.DefiniteTypeArgumentList;
            }
            SyntaxToken lastTokenOfList = null;
            if (ScanPossibleTypeArgumentList(ref lastTokenOfList, out var isDefinitelyTypeArgumentList) == ScanTypeFlags.NotType)
            {
                return ScanTypeArgumentListKind.NotTypeArgumentList;
            }
            if (isDefinitelyTypeArgumentList)
            {
                return ScanTypeArgumentListKind.DefiniteTypeArgumentList;
            }
            switch (base.CurrentToken.Kind)
            {
                case SyntaxKind.CaretToken:
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.CloseParenToken:
                case SyntaxKind.CloseBraceToken:
                case SyntaxKind.CloseBracketToken:
                case SyntaxKind.BarToken:
                case SyntaxKind.ColonToken:
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.CommaToken:
                case SyntaxKind.DotToken:
                case SyntaxKind.QuestionToken:
                case SyntaxKind.ExclamationEqualsToken:
                case SyntaxKind.EqualsEqualsToken:
                    return ScanTypeArgumentListKind.DefiniteTypeArgumentList;
                case SyntaxKind.AmpersandToken:
                case SyntaxKind.OpenBracketToken:
                case SyntaxKind.LessThanToken:
                case SyntaxKind.BarBarToken:
                case SyntaxKind.AmpersandAmpersandToken:
                case SyntaxKind.LessThanEqualsToken:
                case SyntaxKind.GreaterThanEqualsToken:
                case SyntaxKind.IsKeyword:
                case SyntaxKind.AsKeyword:
                    return ScanTypeArgumentListKind.DefiniteTypeArgumentList;
                case SyntaxKind.OpenBraceToken:
                    return ScanTypeArgumentListKind.DefiniteTypeArgumentList;
                case SyntaxKind.GreaterThanToken:
                    if ((options & NameOptions.AfterIs) != 0 && PeekToken(1).Kind != SyntaxKind.GreaterThanToken)
                    {
                        return ScanTypeArgumentListKind.DefiniteTypeArgumentList;
                    }
                    break;
                case SyntaxKind.IdentifierToken:
                    if ((options & (NameOptions.AfterIs | NameOptions.DefinitePattern | NameOptions.AfterOut)) != 0 || ((options & NameOptions.AfterTupleComma) != 0 && (PeekToken(1).Kind == SyntaxKind.CommaToken || PeekToken(1).Kind == SyntaxKind.CloseParenToken)) || ((options & NameOptions.FirstElementOfPossibleTupleLiteral) != 0 && PeekToken(1).Kind == SyntaxKind.CommaToken))
                    {
                        return ScanTypeArgumentListKind.DefiniteTypeArgumentList;
                    }
                    return ScanTypeArgumentListKind.PossibleTypeArgumentList;
                case SyntaxKind.EndOfFileToken:
                    return ScanTypeArgumentListKind.DefiniteTypeArgumentList;
                case SyntaxKind.EqualsGreaterThanToken:
                    return ScanTypeArgumentListKind.DefiniteTypeArgumentList;
            }
            return ScanTypeArgumentListKind.PossibleTypeArgumentList;
        }

        private ScanTypeFlags ScanPossibleTypeArgumentList(ref SyntaxToken lastTokenOfList, out bool isDefinitelyTypeArgumentList)
        {
            isDefinitelyTypeArgumentList = false;
            if (base.CurrentToken.Kind == SyntaxKind.LessThanToken)
            {
                ScanTypeFlags result = ScanTypeFlags.GenericTypeOrExpression;
                do
                {
                    lastTokenOfList = EatToken();
                    if (base.CurrentToken.Kind == SyntaxKind.OpenBracketToken)
                    {
                        lastTokenOfList = null;
                        return ScanTypeFlags.NotType;
                    }
                    if (base.CurrentToken.Kind == SyntaxKind.GreaterThanToken)
                    {
                        lastTokenOfList = EatToken();
                        return result;
                    }
                    switch (ScanType(out lastTokenOfList))
                    {
                        case ScanTypeFlags.NotType:
                            lastTokenOfList = null;
                            return ScanTypeFlags.NotType;
                        case ScanTypeFlags.MustBeType:
                            isDefinitelyTypeArgumentList = DetermineIfDefinitelyTypeArgumentList(isDefinitelyTypeArgumentList);
                            result = ScanTypeFlags.GenericTypeOrMethod;
                            break;
                        case ScanTypeFlags.NullableType:
                            isDefinitelyTypeArgumentList = DetermineIfDefinitelyTypeArgumentList(isDefinitelyTypeArgumentList);
                            if (isDefinitelyTypeArgumentList)
                            {
                                result = ScanTypeFlags.GenericTypeOrMethod;
                            }
                            break;
                        case ScanTypeFlags.GenericTypeOrExpression:
                            if (!isDefinitelyTypeArgumentList)
                            {
                                isDefinitelyTypeArgumentList = base.CurrentToken.Kind == SyntaxKind.CommaToken;
                                result = ScanTypeFlags.GenericTypeOrMethod;
                            }
                            break;
                        case ScanTypeFlags.GenericTypeOrMethod:
                            result = ScanTypeFlags.GenericTypeOrMethod;
                            break;
                    }
                }
                while (base.CurrentToken.Kind == SyntaxKind.CommaToken);
                if (base.CurrentToken.Kind != SyntaxKind.GreaterThanToken)
                {
                    lastTokenOfList = null;
                    return ScanTypeFlags.NotType;
                }
                lastTokenOfList = EatToken();
                return result;
            }
            return ScanTypeFlags.NonGenericTypeOrExpression;
        }

        private bool DetermineIfDefinitelyTypeArgumentList(bool isDefinitelyTypeArgumentList)
        {
            if (!isDefinitelyTypeArgumentList)
            {
                isDefinitelyTypeArgumentList = base.CurrentToken.Kind == SyntaxKind.CommaToken || base.CurrentToken.Kind == SyntaxKind.GreaterThanToken;
            }
            return isDefinitelyTypeArgumentList;
        }

        private void ParseTypeArgumentList(out SyntaxToken open, SeparatedSyntaxListBuilder<TypeSyntax> types, out SyntaxToken close)
        {
            open = EatToken(SyntaxKind.LessThanToken);
            open = CheckFeatureAvailability(open, MessageID.IDS_FeatureGenerics);
            if (IsOpenName())
            {
                OmittedTypeArgumentSyntax node = _syntaxFactory.OmittedTypeArgument(SyntaxFactory.Token(SyntaxKind.OmittedTypeArgumentToken));
                types.Add(node);
                while (base.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    types.AddSeparator(EatToken(SyntaxKind.CommaToken));
                    types.Add(node);
                }
                close = EatToken(SyntaxKind.GreaterThanToken);
                return;
            }
            types.Add(ParseTypeArgument());
            while (base.CurrentToken.Kind != SyntaxKind.GreaterThanToken)
            {
                if (base.CurrentToken.Kind == SyntaxKind.CommaToken || IsPossibleType())
                {
                    types.AddSeparator(EatToken(SyntaxKind.CommaToken));
                    types.Add(ParseTypeArgument());
                }
                else if (SkipBadTypeArgumentListTokens(types, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                {
                    break;
                }
            }
            close = EatToken(SyntaxKind.GreaterThanToken);
        }

        private PostSkipAction SkipBadTypeArgumentListTokens(SeparatedSyntaxListBuilder<TypeSyntax> list, SyntaxKind expected)
        {
            CSharpSyntaxNode startToken = null;
            return SkipBadSeparatedListTokensWithExpectedKind(ref startToken, list, (LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleType(), (LanguageParser p) => p.CurrentToken.Kind == SyntaxKind.GreaterThanToken || p.IsTerminator(), expected);
        }

        private TypeSyntax ParseTypeArgument()
        {
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> syntaxList = default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>);
            if (base.CurrentToken.Kind == SyntaxKind.OpenBracketToken && PeekToken(1).Kind != SyntaxKind.CloseBracketToken)
            {
                TerminatorState termState = _termState;
                _termState = TerminatorState.IsEndOfTypeArgumentList;
                syntaxList = ParseAttributeDeclarations();
                _termState = termState;
            }
            SyntaxToken syntaxToken = null;
            if (base.CurrentToken.Kind == SyntaxKind.InKeyword || base.CurrentToken.Kind == SyntaxKind.OutKeyword)
            {
                syntaxToken = EatToken();
                syntaxToken = CheckFeatureAvailability(syntaxToken, MessageID.IDS_FeatureTypeVariance);
                syntaxToken = AddError(syntaxToken, ErrorCode.ERR_IllegalVarianceSyntax);
            }
            TypeSyntax typeSyntax = ParseType();
            SyntaxKind syntaxKind = SyntaxKind.None;
            if (typeSyntax.IsMissing && base.CurrentToken.Kind != SyntaxKind.CommaToken && base.CurrentToken.Kind != SyntaxKind.GreaterThanToken && ((syntaxKind = PeekToken(1).Kind) == SyntaxKind.CommaToken || syntaxKind == SyntaxKind.GreaterThanToken))
            {
                typeSyntax = AddTrailingSkippedSyntax(typeSyntax, EatToken());
            }
            if (syntaxToken != null)
            {
                typeSyntax = AddLeadingSkippedSyntax(typeSyntax, syntaxToken);
            }
            if (syntaxList.Count > 0)
            {
                typeSyntax = AddLeadingSkippedSyntax(typeSyntax, syntaxList.Node);
                typeSyntax = AddError(typeSyntax, ErrorCode.ERR_TypeExpected);
            }
            return typeSyntax;
        }

        private bool IsEndOfTypeArgumentList()
        {
            return base.CurrentToken.Kind == SyntaxKind.GreaterThanToken;
        }

        private bool IsOpenName()
        {
            bool result = true;
            int i;
            for (i = 0; PeekToken(i).Kind == SyntaxKind.CommaToken; i++)
            {
            }
            if (PeekToken(i).Kind != SyntaxKind.GreaterThanToken)
            {
                result = false;
            }
            return result;
        }

        private void ParseMemberName(out ExplicitInterfaceSpecifierSyntax explicitInterfaceOpt, out SyntaxToken identifierOrThisOpt, out TypeParameterListSyntax typeParameterListOpt, bool isEvent)
        {
            identifierOrThisOpt = null;
            explicitInterfaceOpt = null;
            typeParameterListOpt = null;
            if (!IsPossibleMemberName())
            {
                return;
            }
            NameSyntax nameSyntax = null;
            SyntaxToken syntaxToken = null;
            ResetPoint state = default(ResetPoint);
            bool flag = false;
            try
            {
                while (true)
                {
                    if (base.CurrentToken.Kind == SyntaxKind.ThisKeyword)
                    {
                        state = GetResetPoint();
                        flag = true;
                        identifierOrThisOpt = EatToken();
                        typeParameterListOpt = ParseTypeParameterList();
                        break;
                    }
                    ResetPoint state2 = GetResetPoint();
                    bool flag2;
                    try
                    {
                        ScanNamedTypePart();
                        flag2 = !IsDotOrColonColon();
                    }
                    finally
                    {
                        Reset(ref state2);
                        Release(ref state2);
                    }
                    if (flag2)
                    {
                        state = GetResetPoint();
                        flag = true;
                        if (syntaxToken != null && syntaxToken.Kind == SyntaxKind.ColonColonToken)
                        {
                            syntaxToken = AddError(syntaxToken, ErrorCode.ERR_AliasQualAsExpression);
                            syntaxToken = ConvertToMissingWithTrailingTrivia(syntaxToken, SyntaxKind.DotToken);
                        }
                        identifierOrThisOpt = ParseIdentifierToken();
                        typeParameterListOpt = ParseTypeParameterList();
                        break;
                    }
                    TerminatorState termState = _termState;
                    _termState |= TerminatorState.IsEndOfNameInExplicitInterface;
                    if (nameSyntax == null)
                    {
                        nameSyntax = ParseSimpleName(NameOptions.InTypeList);
                        syntaxToken = ((base.CurrentToken.Kind == SyntaxKind.ColonColonToken) ? EatToken() : EatToken(SyntaxKind.DotToken));
                    }
                    else
                    {
                        nameSyntax = ParseQualifiedNameRight(NameOptions.InTypeList, nameSyntax, syntaxToken);
                        syntaxToken = ((base.CurrentToken.Kind == SyntaxKind.ColonColonToken) ? ConvertToMissingWithTrailingTrivia(EatToken(), SyntaxKind.DotToken) : EatToken(SyntaxKind.DotToken));
                    }
                    _termState = termState;
                }
                if (nameSyntax == null)
                {
                    return;
                }
                if (syntaxToken.Kind != SyntaxKind.DotToken)
                {
                    syntaxToken = WithAdditionalDiagnostics(syntaxToken, GetExpectedTokenError(SyntaxKind.DotToken, syntaxToken.Kind, syntaxToken.GetLeadingTriviaWidth(), syntaxToken.Width));
                    syntaxToken = ConvertToMissingWithTrailingTrivia(syntaxToken, SyntaxKind.DotToken);
                }
                if (isEvent && base.CurrentToken.Kind != SyntaxKind.OpenBraceToken && base.CurrentToken.Kind != SyntaxKind.SemicolonToken)
                {
                    explicitInterfaceOpt = _syntaxFactory.ExplicitInterfaceSpecifier(nameSyntax, AddError(syntaxToken, ErrorCode.ERR_ExplicitEventFieldImpl));
                    if (syntaxToken.TrailingTrivia.Any(8539))
                    {
                        Reset(ref state);
                        identifierOrThisOpt = null;
                        typeParameterListOpt = null;
                    }
                }
                else
                {
                    explicitInterfaceOpt = _syntaxFactory.ExplicitInterfaceSpecifier(nameSyntax, syntaxToken);
                }
            }
            finally
            {
                if (flag)
                {
                    Release(ref state);
                }
            }
        }

        private NameSyntax ParseAliasQualifiedName(NameOptions allowedParts = NameOptions.None)
        {
            NameSyntax nameSyntax = ParseSimpleName(allowedParts);
            if (base.CurrentToken.Kind == SyntaxKind.ColonColonToken)
            {
                SyntaxToken separator = EatToken();
                nameSyntax = ParseQualifiedNameRight(allowedParts, nameSyntax, separator);
            }
            return nameSyntax;
        }

        private NameSyntax ParseQualifiedName(NameOptions options = NameOptions.None)
        {
            NameSyntax nameSyntax = ParseAliasQualifiedName(options);
            while ((IsDotOrColonColon() || base.CurrentToken.Kind == SyntaxKind.DotDotToken) && PeekToken(1).Kind != SyntaxKind.ThisKeyword)
            {
                SyntaxToken separator = EatToken();
                nameSyntax = ParseQualifiedNameRight(options, nameSyntax, separator);
            }
            return nameSyntax;
        }

        private NameSyntax ParseQualifiedNameRight(NameOptions options, NameSyntax left, SyntaxToken separator)
        {
            SimpleNameSyntax simpleNameSyntax = ParseSimpleName(options);
            switch (separator.Kind)
            {
                case SyntaxKind.DotToken:
                    return _syntaxFactory.QualifiedName(left, separator, simpleNameSyntax);
                case SyntaxKind.DotDotToken:
                    {
                        SyntaxToken dotToken = SyntaxFactory.Token(separator.LeadingTrivia.Node, SyntaxKind.DotToken, null);
                        IdentifierNameSyntax right = AddError(CreateMissingIdentifierName(), ErrorCode.ERR_IdentifierExpected);
                        SyntaxToken dotToken2 = SyntaxFactory.Token(null, SyntaxKind.DotToken, separator.TrailingTrivia.Node);
                        return _syntaxFactory.QualifiedName(_syntaxFactory.QualifiedName(left, dotToken, right), dotToken2, simpleNameSyntax);
                    }
                case SyntaxKind.ColonColonToken:
                    {
                        if (left.Kind != SyntaxKind.IdentifierName)
                        {
                            separator = AddError(separator, ErrorCode.ERR_UnexpectedAliasedName, separator.ToString());
                        }
                        IdentifierNameSyntax identifierNameSyntax = left as IdentifierNameSyntax;
                        if (identifierNameSyntax == null)
                        {
                            separator = ConvertToMissingWithTrailingTrivia(separator, SyntaxKind.DotToken);
                            return _syntaxFactory.QualifiedName(left, separator, simpleNameSyntax);
                        }
                        if (identifierNameSyntax.Identifier.ContextualKind == SyntaxKind.GlobalKeyword)
                        {
                            identifierNameSyntax = _syntaxFactory.IdentifierName(SyntaxParser.ConvertToKeyword(identifierNameSyntax.Identifier));
                        }
                        identifierNameSyntax = CheckFeatureAvailability(identifierNameSyntax, MessageID.IDS_FeatureGlobalNamespace);
                        return WithAdditionalDiagnostics(_syntaxFactory.AliasQualifiedName(identifierNameSyntax, separator, simpleNameSyntax), left.GetDiagnostics());
                    }
                default:
                    throw ExceptionUtilities.Unreachable;
            }
        }

        private SyntaxToken ConvertToMissingWithTrailingTrivia(SyntaxToken token, SyntaxKind expectedKind)
        {
            SyntaxToken node = SyntaxFactory.MissingToken(expectedKind);
            return AddTrailingSkippedSyntax(node, token);
        }

        private bool IsPossibleType()
        {
            if (!IsPredefinedType(base.CurrentToken.Kind))
            {
                return IsTrueIdentifier();
            }
            return true;
        }

        private ScanTypeFlags ScanType(bool forPattern = false)
        {
            return ScanType(out SyntaxToken lastTokenOfType, forPattern);
        }

        private ScanTypeFlags ScanType(out SyntaxToken lastTokenOfType, bool forPattern = false)
        {
            return ScanType(forPattern ? ParseTypeMode.DefinitePattern : ParseTypeMode.Normal, out lastTokenOfType);
        }

        private void ScanNamedTypePart()
        {
            ScanNamedTypePart(out var _);
        }

        private ScanTypeFlags ScanNamedTypePart(out SyntaxToken lastTokenOfType)
        {
            if (base.CurrentToken.Kind != SyntaxKind.IdentifierToken || !IsTrueIdentifier())
            {
                lastTokenOfType = null;
                return ScanTypeFlags.NotType;
            }
            lastTokenOfType = EatToken();
            if (base.CurrentToken.Kind == SyntaxKind.LessThanToken)
            {
                return ScanPossibleTypeArgumentList(ref lastTokenOfType, out bool isDefinitelyTypeArgumentList);
            }
            return ScanTypeFlags.NonGenericTypeOrExpression;
        }

        private ScanTypeFlags ScanType(ParseTypeMode mode, out SyntaxToken lastTokenOfType)
        {
            if (base.CurrentToken.Kind == SyntaxKind.RefKeyword)
            {
                EatToken();
                if (base.CurrentToken.Kind == SyntaxKind.ReadOnlyKeyword)
                {
                    EatToken();
                }
            }
            SyntaxKind kind = base.CurrentToken.Kind;
            ScanTypeFlags scanTypeFlags;
            if (kind == SyntaxKind.IdentifierToken || kind == SyntaxKind.ColonColonToken)
            {
                bool flag;
                if (base.CurrentToken.Kind == SyntaxKind.ColonColonToken)
                {
                    scanTypeFlags = ScanTypeFlags.NonGenericTypeOrExpression;
                    flag = true;
                    lastTokenOfType = null;
                }
                else
                {
                    flag = PeekToken(1).Kind == SyntaxKind.ColonColonToken;
                    scanTypeFlags = ScanNamedTypePart(out lastTokenOfType);
                    if (scanTypeFlags == ScanTypeFlags.NotType)
                    {
                        return ScanTypeFlags.NotType;
                    }
                }
                bool flag2 = true;
                while (IsDotOrColonColon())
                {
                    if (!flag2)
                    {
                        flag = false;
                    }
                    EatToken();
                    scanTypeFlags = ScanNamedTypePart(out lastTokenOfType);
                    if (scanTypeFlags == ScanTypeFlags.NotType)
                    {
                        return ScanTypeFlags.NotType;
                    }
                    flag2 = false;
                }
                if (flag)
                {
                    scanTypeFlags = ScanTypeFlags.AliasQualifiedName;
                }
            }
            else if (IsPredefinedType(base.CurrentToken.Kind))
            {
                lastTokenOfType = EatToken();
                scanTypeFlags = ScanTypeFlags.MustBeType;
            }
            else if (base.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                lastTokenOfType = EatToken();
                scanTypeFlags = ScanTupleType(out lastTokenOfType);
                if (scanTypeFlags == ScanTypeFlags.NotType || (mode == ParseTypeMode.DefinitePattern && base.CurrentToken.Kind != SyntaxKind.OpenBracketToken))
                {
                    return ScanTypeFlags.NotType;
                }
            }
            else
            {
                if (!IsFunctionPointerStart())
                {
                    lastTokenOfType = null;
                    return ScanTypeFlags.NotType;
                }
                scanTypeFlags = ScanFunctionPointerType(out lastTokenOfType);
            }
            int lastTokenPosition = -1;
            while (IsMakingProgress(ref lastTokenPosition))
            {
                switch (base.CurrentToken.Kind)
                {
                    case SyntaxKind.QuestionToken:
                        if (lastTokenOfType.Kind != SyntaxKind.QuestionToken && lastTokenOfType.Kind != SyntaxKind.AsteriskToken)
                        {
                            lastTokenOfType = EatToken();
                            scanTypeFlags = ScanTypeFlags.NullableType;
                            continue;
                        }
                        break;
                    case SyntaxKind.AsteriskToken:
                        if (lastTokenOfType.Kind != SyntaxKind.CloseBracketToken && mode != ParseTypeMode.DefinitePattern && ((mode != ParseTypeMode.AfterTupleComma && mode != ParseTypeMode.FirstElementOfPossibleTupleLiteral) || PointerTypeModsFollowedByRankAndDimensionSpecifier()))
                        {
                            lastTokenOfType = EatToken();
                            switch (scanTypeFlags)
                            {
                                case ScanTypeFlags.GenericTypeOrExpression:
                                case ScanTypeFlags.NonGenericTypeOrExpression:
                                    scanTypeFlags = ScanTypeFlags.PointerOrMultiplication;
                                    break;
                                case ScanTypeFlags.GenericTypeOrMethod:
                                    scanTypeFlags = ScanTypeFlags.MustBeType;
                                    break;
                            }
                            continue;
                        }
                        break;
                    case SyntaxKind.OpenBracketToken:
                        EatToken();
                        while (base.CurrentToken.Kind == SyntaxKind.CommaToken)
                        {
                            EatToken();
                        }
                        if (base.CurrentToken.Kind != SyntaxKind.CloseBracketToken)
                        {
                            lastTokenOfType = null;
                            return ScanTypeFlags.NotType;
                        }
                        lastTokenOfType = EatToken();
                        scanTypeFlags = ScanTypeFlags.MustBeType;
                        continue;
                }
                break;
            }
            return scanTypeFlags;
        }

        private ScanTypeFlags ScanTupleType(out SyntaxToken lastTokenOfType)
        {
            if (ScanType(out lastTokenOfType) != 0)
            {
                if (IsTrueIdentifier())
                {
                    lastTokenOfType = EatToken();
                }
                if (base.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    do
                    {
                        lastTokenOfType = EatToken();
                        if (ScanType(out lastTokenOfType) == ScanTypeFlags.NotType)
                        {
                            lastTokenOfType = EatToken();
                            return ScanTypeFlags.NotType;
                        }
                        if (IsTrueIdentifier())
                        {
                            lastTokenOfType = EatToken();
                        }
                    }
                    while (base.CurrentToken.Kind == SyntaxKind.CommaToken);
                    if (base.CurrentToken.Kind == SyntaxKind.CloseParenToken)
                    {
                        lastTokenOfType = EatToken();
                        return ScanTypeFlags.TupleType;
                    }
                }
            }
            lastTokenOfType = null;
            return ScanTypeFlags.NotType;
        }

        private ScanTypeFlags ScanFunctionPointerType(out SyntaxToken lastTokenOfType)
        {
            EatToken(SyntaxKind.DelegateKeyword);
            lastTokenOfType = EatToken(SyntaxKind.AsteriskToken);
            SyntaxToken lastTokenOfType2;
            if (base.CurrentToken.Kind == SyntaxKind.IdentifierToken)
            {
                SyntaxToken syntaxToken = PeekToken(1);
                lastTokenOfType2 = base.CurrentToken;
                if (lastTokenOfType2 != null)
                {
                    SyntaxKind contextualKind = lastTokenOfType2.ContextualKind;
                    if (contextualKind - 8445 <= SyntaxKind.List)
                    {
                        goto IL_006b;
                    }
                }
                if (!IsPossibleFunctionPointerParameterListStart(syntaxToken) && syntaxToken.Kind != SyntaxKind.OpenBracketToken)
                {
                    return ScanTypeFlags.MustBeType;
                }
                goto IL_006b;
            }
            goto IL_00f2;
        IL_006b:
            lastTokenOfType = EatToken();
            TerminatorState termState;
            if (base.CurrentToken.Kind == SyntaxKind.OpenBracketToken)
            {
                lastTokenOfType = EatToken(SyntaxKind.OpenBracketToken);
                termState = _termState;
                _termState |= TerminatorState.IsEndOfFunctionPointerCallingConvention;
                try
                {
                    while (true)
                    {
                        lastTokenOfType = TryEatToken(SyntaxKind.IdentifierToken) ?? lastTokenOfType;
                        if (skipBadFunctionPointerTokens() == PostSkipAction.Abort)
                        {
                            break;
                        }
                        lastTokenOfType = EatToken();
                    }
                    lastTokenOfType = TryEatToken(SyntaxKind.CloseBracketToken) ?? lastTokenOfType;
                }
                finally
                {
                    _termState = termState;
                }
            }
            goto IL_00f2;
        IL_00f2:
            if (!IsPossibleFunctionPointerParameterListStart(base.CurrentToken))
            {
                return ScanTypeFlags.MustBeType;
            }
            bool flag = EatToken().Kind == SyntaxKind.LessThanToken;
            termState = _termState;
            _termState |= (TerminatorState)(flag ? 8388608 : 16777216);
            SyntaxListBuilder<SyntaxToken> syntaxListBuilder = _pool.Allocate<SyntaxToken>();
            try
            {
                while (true)
                {
                    ParseParameterModifiers(syntaxListBuilder, isFunctionPointerParameter: true);
                    syntaxListBuilder.Clear();
                    ScanType(out lastTokenOfType2);
                    if (skipBadFunctionPointerTokens() == PostSkipAction.Abort)
                    {
                        break;
                    }
                    EatToken(SyntaxKind.CommaToken);
                }
            }
            finally
            {
                _termState = termState;
                _pool.Free(syntaxListBuilder);
            }
            if (!flag && base.CurrentToken.Kind == SyntaxKind.CloseParenToken)
            {
                lastTokenOfType = EatTokenAsKind(SyntaxKind.GreaterThanToken);
            }
            else
            {
                lastTokenOfType = EatToken(SyntaxKind.GreaterThanToken);
            }
            return ScanTypeFlags.MustBeType;
            PostSkipAction skipBadFunctionPointerTokens()
            {
                return SkipBadTokensWithExpectedKind((LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CommaToken, (LanguageParser p) => p.IsTerminator(), SyntaxKind.CommaToken, out GreenNode trailingTrivia);
            }
        }

        private static bool IsPredefinedType(SyntaxKind keyword)
        {
            return SyntaxFacts.IsPredefinedType(keyword);
        }

        public TypeSyntax ParseTypeName()
        {
            return ParseType();
        }

        private TypeSyntax ParseTypeOrVoid()
        {
            if (base.CurrentToken.Kind == SyntaxKind.VoidKeyword && PeekToken(1).Kind != SyntaxKind.AsteriskToken)
            {
                return _syntaxFactory.PredefinedType(EatToken());
            }
            return ParseType();
        }

        private TypeSyntax ParseType(ParseTypeMode mode = ParseTypeMode.Normal)
        {
            if (base.CurrentToken.Kind == SyntaxKind.RefKeyword)
            {
                SyntaxToken node = EatToken();
                node = CheckFeatureAvailability(node, MessageID.IDS_FeatureRefLocalsReturns);
                SyntaxToken readOnlyKeyword = null;
                if (base.CurrentToken.Kind == SyntaxKind.ReadOnlyKeyword)
                {
                    readOnlyKeyword = EatToken();
                    readOnlyKeyword = CheckFeatureAvailability(readOnlyKeyword, MessageID.IDS_FeatureReadOnlyReferences);
                }
                TypeSyntax type = ParseTypeCore(ParseTypeMode.AfterRef);
                return _syntaxFactory.RefType(node, readOnlyKeyword, type);
            }
            return ParseTypeCore(mode);
        }

        private TypeSyntax ParseTypeCore(ParseTypeMode mode)
        {
            NameOptions options;
            switch (mode)
            {
                case ParseTypeMode.AfterIs:
                    options = NameOptions.InExpression | NameOptions.PossiblePattern | NameOptions.AfterIs;
                    break;
                case ParseTypeMode.DefinitePattern:
                    options = NameOptions.InExpression | NameOptions.PossiblePattern | NameOptions.DefinitePattern;
                    break;
                case ParseTypeMode.AfterOut:
                    options = NameOptions.InExpression | NameOptions.AfterOut;
                    break;
                case ParseTypeMode.AfterTupleComma:
                    options = NameOptions.InExpression | NameOptions.AfterTupleComma;
                    break;
                case ParseTypeMode.FirstElementOfPossibleTupleLiteral:
                    options = NameOptions.InExpression | NameOptions.FirstElementOfPossibleTupleLiteral;
                    break;
                case ParseTypeMode.Normal:
                case ParseTypeMode.Parameter:
                case ParseTypeMode.AfterRef:
                case ParseTypeMode.AsExpression:
                case ParseTypeMode.NewExpression:
                    options = NameOptions.None;
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(mode);
            }
            TypeSyntax type = ParseUnderlyingType(mode, options);
            int lastTokenPosition = -1;
            while (IsMakingProgress(ref lastTokenPosition))
            {
                switch (base.CurrentToken.Kind)
                {
                    case SyntaxKind.QuestionToken:
                        if (canBeNullableType())
                        {
                            SyntaxToken syntaxToken = EatNullableQualifierIfApplicable(mode);
                            if (syntaxToken != null)
                            {
                                type = _syntaxFactory.NullableType(type, syntaxToken);
                                continue;
                            }
                        }
                        break;
                    case SyntaxKind.AsteriskToken:
                        if (type.Kind == SyntaxKind.ArrayType)
                        {
                            break;
                        }
                        switch (mode)
                        {
                            case ParseTypeMode.AfterIs:
                            case ParseTypeMode.DefinitePattern:
                            case ParseTypeMode.AfterTupleComma:
                            case ParseTypeMode.FirstElementOfPossibleTupleLiteral:
                                if (PointerTypeModsFollowedByRankAndDimensionSpecifier())
                                {
                                    type = ParsePointerTypeMods(type);
                                    continue;
                                }
                                break;
                            case ParseTypeMode.Normal:
                            case ParseTypeMode.Parameter:
                            case ParseTypeMode.AfterOut:
                            case ParseTypeMode.AfterRef:
                            case ParseTypeMode.AsExpression:
                            case ParseTypeMode.NewExpression:
                                type = ParsePointerTypeMods(type);
                                continue;
                        }
                        break;
                    case SyntaxKind.OpenBracketToken:
                        {
                            SyntaxListBuilder<ArrayRankSpecifierSyntax> syntaxListBuilder = _pool.Allocate<ArrayRankSpecifierSyntax>();
                            try
                            {
                                while (base.CurrentToken.Kind == SyntaxKind.OpenBracketToken)
                                {
                                    ArrayRankSpecifierSyntax node = ParseArrayRankSpecifier(out bool sawNonOmittedSize);
                                    syntaxListBuilder.Add(node);
                                }
                                type = _syntaxFactory.ArrayType(type, syntaxListBuilder);
                            }
                            finally
                            {
                                _pool.Free(syntaxListBuilder);
                            }
                            continue;
                        }
                }
                break;
            }
            return type;
            bool canBeNullableType()
            {
                if (type.Kind == SyntaxKind.NullableType || type.Kind == SyntaxKind.PointerType || type.Kind == SyntaxKind.FunctionPointerType)
                {
                    return false;
                }
                if (PeekToken(1).Kind == SyntaxKind.OpenBracketToken)
                {
                    return true;
                }
                if (mode == ParseTypeMode.DefinitePattern)
                {
                    return true;
                }
                if (mode == ParseTypeMode.NewExpression && type.Kind == SyntaxKind.TupleType && PeekToken(1).Kind != SyntaxKind.OpenParenToken && PeekToken(1).Kind != SyntaxKind.OpenBraceToken)
                {
                    return false;
                }
                return true;
            }
        }

        private SyntaxToken EatNullableQualifierIfApplicable(ParseTypeMode mode)
        {
            ResetPoint state = GetResetPoint();
            try
            {
                SyntaxToken node = EatToken();
                if (!canFollowNullableType())
                {
                    Reset(ref state);
                    return null;
                }
                return CheckFeatureAvailability(node, MessageID.IDS_FeatureNullable);
            }
            finally
            {
                Release(ref state);
            }
            bool canFollowNullableType()
            {
                switch (mode)
                {
                    case ParseTypeMode.AfterIs:
                    case ParseTypeMode.DefinitePattern:
                    case ParseTypeMode.AsExpression:
                        return !CanStartExpression();
                    case ParseTypeMode.NewExpression:
                        if (base.CurrentToken.Kind != SyntaxKind.OpenParenToken && base.CurrentToken.Kind != SyntaxKind.OpenBracketToken)
                        {
                            return base.CurrentToken.Kind == SyntaxKind.OpenBraceToken;
                        }
                        return true;
                    default:
                        return true;
                }
            }
        }

        private bool PointerTypeModsFollowedByRankAndDimensionSpecifier()
        {
            int num = 0;
            while (true)
            {
                switch (PeekToken(num).Kind)
                {
                    case SyntaxKind.OpenBracketToken:
                        return true;
                    default:
                        return false;
                    case SyntaxKind.AsteriskToken:
                        break;
                }
                num++;
            }
        }

        private ArrayRankSpecifierSyntax ParseArrayRankSpecifier(out bool sawNonOmittedSize)
        {
            sawNonOmittedSize = false;
            bool flag = false;
            SyntaxToken openBracket = EatToken(SyntaxKind.OpenBracketToken);
            SeparatedSyntaxListBuilder<ExpressionSyntax> builder = _pool.AllocateSeparated<ExpressionSyntax>();
            try
            {
                OmittedArraySizeExpressionSyntax node = _syntaxFactory.OmittedArraySizeExpression(SyntaxFactory.Token(SyntaxKind.OmittedArraySizeExpressionToken));
                int lastTokenPosition = -1;
                while (IsMakingProgress(ref lastTokenPosition) && base.CurrentToken.Kind != SyntaxKind.CloseBracketToken)
                {
                    if (base.CurrentToken.Kind == SyntaxKind.CommaToken)
                    {
                        flag = true;
                        builder.Add(node);
                        builder.AddSeparator(EatToken());
                    }
                    else if (IsPossibleExpression())
                    {
                        ExpressionSyntax node2 = ParseExpressionCore();
                        sawNonOmittedSize = true;
                        builder.Add(node2);
                        if (base.CurrentToken.Kind != SyntaxKind.CloseBracketToken)
                        {
                            builder.AddSeparator(EatToken(SyntaxKind.CommaToken));
                        }
                    }
                    else if (SkipBadArrayRankSpecifierTokens(ref openBracket, builder, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                    {
                        break;
                    }
                }
                if ((builder.Count & 1) == 0)
                {
                    flag = true;
                    builder.Add(node);
                }
                if (flag & sawNonOmittedSize)
                {
                    for (int i = 0; i < builder.Count; i++)
                    {
                        if (builder[i]!.RawKind == 8654)
                        {
                            int width = builder[i]!.Width;
                            int leadingTriviaWidth = builder[i]!.GetLeadingTriviaWidth();
                            builder[i] = AddError(CreateMissingIdentifierName(), leadingTriviaWidth, width, ErrorCode.ERR_ValueExpected);
                        }
                    }
                }
                SyntaxToken closeBracketToken = EatToken(SyntaxKind.CloseBracketToken);
                return _syntaxFactory.ArrayRankSpecifier(openBracket, builder, closeBracketToken);
            }
            finally
            {
                _pool.Free(in builder);
            }
        }

        private TupleTypeSyntax ParseTupleType()
        {
            SyntaxToken openParenToken = EatToken(SyntaxKind.OpenParenToken);
            SeparatedSyntaxListBuilder<TupleElementSyntax> item = _pool.AllocateSeparated<TupleElementSyntax>();
            try
            {
                if (base.CurrentToken.Kind != SyntaxKind.CloseParenToken)
                {
                    TupleElementSyntax node = ParseTupleElement();
                    item.Add(node);
                    while (base.CurrentToken.Kind == SyntaxKind.CommaToken)
                    {
                        SyntaxToken separatorToken = EatToken(SyntaxKind.CommaToken);
                        item.AddSeparator(separatorToken);
                        node = ParseTupleElement();
                        item.Add(node);
                    }
                }
                if (item.Count < 2)
                {
                    if (item.Count < 1)
                    {
                        item.Add(_syntaxFactory.TupleElement(CreateMissingIdentifierName(), null));
                    }
                    item.AddSeparator(SyntaxFactory.MissingToken(SyntaxKind.CommaToken));
                    IdentifierNameSyntax type = AddError(CreateMissingIdentifierName(), ErrorCode.ERR_TupleTooFewElements);
                    item.Add(_syntaxFactory.TupleElement(type, null));
                }
                SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
                TupleTypeSyntax node2 = _syntaxFactory.TupleType(openParenToken, item, closeParenToken);
                return CheckFeatureAvailability(node2, MessageID.IDS_FeatureTuples);
            }
            finally
            {
                _pool.Free(in item);
            }
        }

        private TupleElementSyntax ParseTupleElement()
        {
            TypeSyntax type = ParseType();
            SyntaxToken identifier = null;
            if (IsTrueIdentifier())
            {
                identifier = ParseIdentifierToken();
            }
            return _syntaxFactory.TupleElement(type, identifier);
        }

        private PostSkipAction SkipBadArrayRankSpecifierTokens(ref SyntaxToken openBracket, SeparatedSyntaxListBuilder<ExpressionSyntax> list, SyntaxKind expected)
        {
            return SkipBadSeparatedListTokensWithExpectedKind(ref openBracket, list, (LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleExpression(), (LanguageParser p) => p.CurrentToken.Kind == SyntaxKind.CloseBracketToken || p.IsTerminator(), expected);
        }

        private TypeSyntax ParseUnderlyingType(ParseTypeMode mode, NameOptions options = NameOptions.None)
        {
            if (IsPredefinedType(base.CurrentToken.Kind))
            {
                SyntaxToken syntaxToken = EatToken();
                if (syntaxToken.Kind == SyntaxKind.VoidKeyword && base.CurrentToken.Kind != SyntaxKind.AsteriskToken)
                {
                    syntaxToken = AddError(syntaxToken, (mode == ParseTypeMode.Parameter) ? ErrorCode.ERR_NoVoidParameter : ErrorCode.ERR_NoVoidHere);
                }
                return _syntaxFactory.PredefinedType(syntaxToken);
            }
            if (IsTrueIdentifier() || base.CurrentToken.Kind == SyntaxKind.ColonColonToken)
            {
                return ParseQualifiedName(options);
            }
            if (base.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                return ParseTupleType();
            }
            if (IsFunctionPointerStart())
            {
                return ParseFunctionPointerTypeSyntax();
            }
            return AddError(CreateMissingIdentifierName(), (mode == ParseTypeMode.NewExpression) ? ErrorCode.ERR_BadNewExpr : ErrorCode.ERR_TypeExpected);
        }

        private FunctionPointerTypeSyntax ParseFunctionPointerTypeSyntax()
        {
            SyntaxToken delegateKeyword = EatToken(SyntaxKind.DelegateKeyword);
            SyntaxToken asteriskToken = EatToken(SyntaxKind.AsteriskToken);
            FunctionPointerCallingConventionSyntax callingConvention = parseCallingConvention();
            if (!IsPossibleFunctionPointerParameterListStart(base.CurrentToken))
            {
                SyntaxToken lessThanToken = WithAdditionalDiagnostics(SyntaxFactory.MissingToken(SyntaxKind.LessThanToken), GetExpectedTokenError(SyntaxKind.LessThanToken, SyntaxKind.None));
                SeparatedSyntaxListBuilder<FunctionPointerParameterSyntax> builder = _pool.AllocateSeparated<FunctionPointerParameterSyntax>();
                IdentifierNameSyntax type = CreateMissingIdentifierName();
                FunctionPointerParameterSyntax node = SyntaxFactory.FunctionPointerParameter(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>), type);
                builder.Add(node);
                FunctionPointerParameterListSyntax parameterList = SyntaxFactory.FunctionPointerParameterList(greaterThanToken: TryEatToken(SyntaxKind.GreaterThanToken) ?? SyntaxFactory.MissingToken(SyntaxKind.GreaterThanToken), lessThanToken: lessThanToken, parameters: builder);
                FunctionPointerTypeSyntax result = SyntaxFactory.FunctionPointerType(delegateKeyword, asteriskToken, callingConvention, parameterList);
                _pool.Free(in builder);
                return result;
            }
            SyntaxToken syntaxToken = EatTokenAsKind(SyntaxKind.LessThanToken);
            TerminatorState termState = _termState;
            _termState |= (TerminatorState)(syntaxToken.IsMissing ? 16777216 : 8388608);
            SeparatedSyntaxListBuilder<FunctionPointerParameterSyntax> item = _pool.AllocateSeparated<FunctionPointerParameterSyntax>();
            try
            {
                while (true)
                {
                    SyntaxListBuilder<SyntaxToken> syntaxListBuilder = _pool.Allocate<SyntaxToken>();
                    try
                    {
                        ParseParameterModifiers(syntaxListBuilder, isFunctionPointerParameter: true);
                        TypeSyntax type2 = ParseTypeOrVoid();
                        item.Add(SyntaxFactory.FunctionPointerParameter(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), syntaxListBuilder, type2));
                        if (skipBadFunctionPointerTokens<FunctionPointerParameterSyntax>(item) == PostSkipAction.Abort)
                        {
                            break;
                        }
                        item.AddSeparator(EatToken(SyntaxKind.CommaToken));
                        continue;
                    }
                    finally
                    {
                        _pool.Free(syntaxListBuilder);
                    }
                }
                FunctionPointerTypeSyntax node2 = SyntaxFactory.FunctionPointerType(delegateKeyword, asteriskToken, callingConvention, SyntaxFactory.FunctionPointerParameterList(greaterThanToken: (!syntaxToken.IsMissing || base.CurrentToken.Kind != SyntaxKind.CloseParenToken) ? EatToken(SyntaxKind.GreaterThanToken) : EatTokenAsKind(SyntaxKind.GreaterThanToken), lessThanToken: syntaxToken, parameters: item));
                return CheckFeatureAvailability(node2, MessageID.IDS_FeatureFunctionPointers);
            }
            finally
            {
                _termState = termState;
                _pool.Free(in item);
            }
            FunctionPointerCallingConventionSyntax? parseCallingConvention()
            {
                if (base.CurrentToken.Kind != SyntaxKind.IdentifierToken)
                {
                    return null;
                }
                SyntaxToken syntaxToken2 = PeekToken(1);
                SyntaxToken currentToken = base.CurrentToken;
                SyntaxToken syntaxToken3;
                if (currentToken != null)
                {
                    SyntaxKind contextualKind = currentToken.ContextualKind;
                    if (contextualKind - 8445 <= SyntaxKind.List)
                    {
                        syntaxToken3 = EatContextualToken(base.CurrentToken.ContextualKind);
                        goto IL_0082;
                    }
                }
                if (IsPossibleFunctionPointerParameterListStart(syntaxToken2))
                {
                    syntaxToken3 = EatTokenAsKind(SyntaxKind.ManagedKeyword);
                }
                else
                {
                    if (syntaxToken2.Kind != SyntaxKind.OpenBracketToken)
                    {
                        return null;
                    }
                    syntaxToken3 = EatTokenAsKind(SyntaxKind.UnmanagedKeyword);
                }
                goto IL_0082;
            IL_0082:
                FunctionPointerUnmanagedCallingConventionListSyntax functionPointerUnmanagedCallingConventionListSyntax = null;
                if (base.CurrentToken.Kind == SyntaxKind.OpenBracketToken)
                {
                    SyntaxToken openBracketToken = EatToken(SyntaxKind.OpenBracketToken);
                    SeparatedSyntaxListBuilder<FunctionPointerUnmanagedCallingConventionSyntax> builder2 = _pool.AllocateSeparated<FunctionPointerUnmanagedCallingConventionSyntax>();
                    TerminatorState termState2 = _termState;
                    _termState |= TerminatorState.IsEndOfFunctionPointerCallingConvention;
                    try
                    {
                        while (true)
                        {
                            builder2.Add(SyntaxFactory.FunctionPointerUnmanagedCallingConvention(EatToken(SyntaxKind.IdentifierToken)));
                            if (skipBadFunctionPointerTokens<FunctionPointerUnmanagedCallingConventionSyntax>(builder2) == PostSkipAction.Abort)
                            {
                                break;
                            }
                            builder2.AddSeparator(EatToken(SyntaxKind.CommaToken));
                        }
                        SyntaxToken closeBracketToken = EatToken(SyntaxKind.CloseBracketToken);
                        functionPointerUnmanagedCallingConventionListSyntax = SyntaxFactory.FunctionPointerUnmanagedCallingConventionList(openBracketToken, builder2, closeBracketToken);
                    }
                    finally
                    {
                        _termState = termState2;
                        _pool.Free(in builder2);
                    }
                }
                if (syntaxToken3.Kind == SyntaxKind.ManagedKeyword && functionPointerUnmanagedCallingConventionListSyntax != null)
                {
                    functionPointerUnmanagedCallingConventionListSyntax = AddError(functionPointerUnmanagedCallingConventionListSyntax, ErrorCode.ERR_CannotSpecifyManagedWithUnmanagedSpecifiers);
                }
                return SyntaxFactory.FunctionPointerCallingConvention(syntaxToken3, functionPointerUnmanagedCallingConventionListSyntax);
            }
            PostSkipAction skipBadFunctionPointerTokens<T>(SeparatedSyntaxListBuilder<T> list) where T : CSharpSyntaxNode
            {
                CSharpSyntaxNode startToken = null;
                return SkipBadSeparatedListTokensWithExpectedKind(ref startToken, list, (LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CommaToken, (LanguageParser p) => p.IsTerminator(), SyntaxKind.CommaToken);
            }
        }

        private bool IsFunctionPointerStart()
        {
            if (base.CurrentToken.Kind == SyntaxKind.DelegateKeyword)
            {
                return PeekToken(1).Kind == SyntaxKind.AsteriskToken;
            }
            return false;
        }

        private static bool IsPossibleFunctionPointerParameterListStart(SyntaxToken token)
        {
            if (token.Kind != SyntaxKind.LessThanToken)
            {
                return token.Kind == SyntaxKind.OpenParenToken;
            }
            return true;
        }

        private TypeSyntax ParsePointerTypeMods(TypeSyntax type)
        {
            while (base.CurrentToken.Kind == SyntaxKind.AsteriskToken)
            {
                type = _syntaxFactory.PointerType(type, EatToken());
            }
            return type;
        }

        public StatementSyntax ParseStatement()
        {
            return ParseWithStackGuard(() => ParsePossiblyAttributedStatement() ?? ParseExpressionStatement(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>)), () => SyntaxFactory.EmptyStatement(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), SyntaxFactory.MissingToken(SyntaxKind.SemicolonToken)));
        }

        private StatementSyntax ParsePossiblyAttributedStatement()
        {
            return ParseStatementCore(ParseAttributeDeclarations(), isGlobal: false);
        }

        private StatementSyntax ParseStatementCore(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, bool isGlobal)
        {
            if (canReuseStatement(attributes, isGlobal))
            {
                return (StatementSyntax)EatNode();
            }
            ResetPoint resetPointBeforeStatement = GetResetPoint();
            try
            {
                _recursionDepth++;
                StackGuard.EnsureSufficientExecutionStack(_recursionDepth);
                switch (base.CurrentToken.Kind)
                {
                    case SyntaxKind.FixedKeyword:
                        return ParseFixedStatement(attributes);
                    case SyntaxKind.BreakKeyword:
                        return ParseBreakStatement(attributes);
                    case SyntaxKind.ContinueKeyword:
                        return ParseContinueStatement(attributes);
                    case SyntaxKind.TryKeyword:
                    case SyntaxKind.CatchKeyword:
                    case SyntaxKind.FinallyKeyword:
                        return ParseTryStatement(attributes);
                    case SyntaxKind.CheckedKeyword:
                    case SyntaxKind.UncheckedKeyword:
                        return ParseCheckedStatement(attributes);
                    case SyntaxKind.DoKeyword:
                        return ParseDoStatement(attributes);
                    case SyntaxKind.ForKeyword:
                        return ParseForOrForEachStatement(attributes);
                    case SyntaxKind.ForEachKeyword:
                        return ParseForEachStatement(attributes, null);
                    case SyntaxKind.GotoKeyword:
                        return ParseGotoStatement(attributes);
                    case SyntaxKind.IfKeyword:
                        return ParseIfStatement(attributes);
                    case SyntaxKind.ElseKeyword:
                        return ParseMisplacedElse(attributes);
                    case SyntaxKind.LockKeyword:
                        return ParseLockStatement(attributes);
                    case SyntaxKind.ReturnKeyword:
                        return ParseReturnStatement(attributes);
                    case SyntaxKind.SwitchKeyword:
                        return ParseSwitchStatement(attributes);
                    case SyntaxKind.ThrowKeyword:
                        return ParseThrowStatement(attributes);
                    case SyntaxKind.UnsafeKeyword:
                        {
                            StatementSyntax statementSyntax = TryParseStatementStartingWithUnsafe(attributes);
                            if (statementSyntax != null)
                            {
                                return statementSyntax;
                            }
                            break;
                        }
                    case SyntaxKind.UsingKeyword:
                        return ParseStatementStartingWithUsing(attributes);
                    case SyntaxKind.WhileKeyword:
                        return ParseWhileStatement(attributes);
                    case SyntaxKind.OpenBraceToken:
                        return ParseBlock(attributes);
                    case SyntaxKind.SemicolonToken:
                        return _syntaxFactory.EmptyStatement(attributes, EatToken());
                    case SyntaxKind.IdentifierToken:
                        {
                            StatementSyntax statementSyntax = TryParseStatementStartingWithIdentifier(attributes, isGlobal);
                            if (statementSyntax != null)
                            {
                                return statementSyntax;
                            }
                            break;
                        }
                }
                return ParseStatementCoreRest(attributes, isGlobal, ref resetPointBeforeStatement);
            }
            finally
            {
                _recursionDepth--;
                Release(ref resetPointBeforeStatement);
            }
            bool canReuseStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, bool isGlobal)
            {
                if (IsIncrementalAndFactoryContextMatches && base.CurrentNode is Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax && !isGlobal)
                {
                    return attributes.Count == 0;
                }
                return false;
            }
        }

        private StatementSyntax ParseStatementCoreRest(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, bool isGlobal, ref ResetPoint resetPointBeforeStatement)
        {
            isGlobal = isGlobal && base.IsScript;
            if (!IsPossibleLocalDeclarationStatement(isGlobal))
            {
                return ParseExpressionStatement(attributes);
            }
            if (isGlobal)
            {
                return null;
            }
            bool flag = base.CurrentToken.ContextualKind == SyntaxKind.AwaitKeyword;
            StatementSyntax statementSyntax = ParseLocalDeclarationStatement(attributes);
            if (statementSyntax == null)
            {
                Reset(ref resetPointBeforeStatement);
                return null;
            }
            if (statementSyntax.ContainsDiagnostics && flag && !IsInAsync)
            {
                Reset(ref resetPointBeforeStatement);
                IsInAsync = true;
                statementSyntax = ParseExpressionStatement(attributes);
                IsInAsync = false;
            }
            return statementSyntax;
        }

        private StatementSyntax TryParseStatementStartingWithIdentifier(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, bool isGlobal)
        {
            if (base.CurrentToken.ContextualKind == SyntaxKind.AwaitKeyword && PeekToken(1).Kind == SyntaxKind.ForEachKeyword)
            {
                return ParseForEachStatement(attributes, ParseAwaitKeyword(MessageID.IDS_FeatureAsyncStreams));
            }
            if (IsPossibleAwaitUsing())
            {
                if (PeekToken(2).Kind == SyntaxKind.OpenParenToken)
                {
                    return ParseUsingStatement(attributes, ParseAwaitKeyword(MessageID.IDS_FeatureAsyncUsing));
                }
            }
            else
            {
                if (IsPossibleLabeledStatement())
                {
                    return ParseLabeledStatement(attributes);
                }
                if (IsPossibleYieldStatement())
                {
                    return ParseYieldStatement(attributes);
                }
                if (IsPossibleAwaitExpressionStatement())
                {
                    return ParseExpressionStatement(attributes);
                }
                if (IsQueryExpression(mayBeVariableDeclaration: true, isGlobal && base.IsScript))
                {
                    return ParseExpressionStatement(attributes, ParseQueryExpression(Precedence.Expression));
                }
            }
            return null;
        }

        private StatementSyntax ParseStatementStartingWithUsing(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            if (PeekToken(1).Kind != SyntaxKind.OpenParenToken)
            {
                return ParseLocalDeclarationStatement(attributes);
            }
            return ParseUsingStatement(attributes);
        }

        private StatementSyntax TryParseStatementStartingWithUnsafe(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            if (!IsPossibleUnsafeStatement())
            {
                return null;
            }
            return ParseUnsafeStatement(attributes);
        }

        private SyntaxToken ParseAwaitKeyword(MessageID feature)
        {
            SyntaxToken syntaxToken = EatContextualToken(SyntaxKind.AwaitKeyword);
            if (feature == MessageID.None)
            {
                return syntaxToken;
            }
            return CheckFeatureAvailability(syntaxToken, feature);
        }

        private bool IsPossibleAwaitUsing()
        {
            if (base.CurrentToken.ContextualKind == SyntaxKind.AwaitKeyword)
            {
                return PeekToken(1).Kind == SyntaxKind.UsingKeyword;
            }
            return false;
        }

        private bool IsPossibleLabeledStatement()
        {
            if (PeekToken(1).Kind == SyntaxKind.ColonToken)
            {
                return IsTrueIdentifier();
            }
            return false;
        }

        private bool IsPossibleUnsafeStatement()
        {
            return PeekToken(1).Kind == SyntaxKind.OpenBraceToken;
        }

        private bool IsPossibleYieldStatement()
        {
            if (base.CurrentToken.ContextualKind == SyntaxKind.YieldKeyword)
            {
                if (PeekToken(1).Kind != SyntaxKind.ReturnKeyword)
                {
                    return PeekToken(1).Kind == SyntaxKind.BreakKeyword;
                }
                return true;
            }
            return false;
        }

        private bool IsPossibleLocalDeclarationStatement(bool isGlobalScriptLevel)
        {
            SyntaxKind kind = base.CurrentToken.Kind;
            if (kind == SyntaxKind.RefKeyword || IsDeclarationModifier(kind) || (SyntaxFacts.IsPredefinedType(kind) && PeekToken(1).Kind != SyntaxKind.DotToken && PeekToken(1).Kind != SyntaxKind.OpenParenToken))
            {
                return true;
            }
            if (kind == SyntaxKind.UsingKeyword)
            {
                return true;
            }
            if (IsPossibleAwaitUsing())
            {
                return true;
            }
            kind = base.CurrentToken.ContextualKind;
            if ((IsAdditionalLocalFunctionModifier(kind) || kind == SyntaxKind.OpenBracketToken) && (kind != SyntaxKind.AsyncKeyword || ShouldAsyncBeTreatedAsModifier(parsingStatementNotDeclaration: true)))
            {
                return true;
            }
            return IsPossibleFirstTypedIdentifierInLocaDeclarationStatement(isGlobalScriptLevel);
        }

        private bool IsPossibleFirstTypedIdentifierInLocaDeclarationStatement(bool isGlobalScriptLevel)
        {
            bool? flag = IsPossibleTypedIdentifierStart(base.CurrentToken, PeekToken(1), allowThisKeyword: false);
            if (flag.HasValue)
            {
                return flag.Value;
            }
            if (base.CurrentToken.ContextualKind == SyntaxKind.IdentifierToken)
            {
                SyntaxToken syntaxToken = PeekToken(1);
                if (syntaxToken.Kind == SyntaxKind.DotToken && syntaxToken.TrailingTrivia.Any(8539) && PeekToken(2).Kind == SyntaxKind.IdentifierToken && PeekToken(3).Kind == SyntaxKind.IdentifierToken)
                {
                    SyntaxKind kind = PeekToken(4).Kind;
                    if (kind != SyntaxKind.SemicolonToken && kind != SyntaxKind.EqualsToken && kind != SyntaxKind.CommaToken && kind != SyntaxKind.OpenParenToken && kind != SyntaxKind.LessThanToken)
                    {
                        return false;
                    }
                }
            }
            ResetPoint state = GetResetPoint();
            try
            {
                ScanTypeFlags scanTypeFlags = ScanType();
                if (scanTypeFlags == ScanTypeFlags.MustBeType && base.CurrentToken.Kind != SyntaxKind.DotToken && base.CurrentToken.Kind != SyntaxKind.OpenParenToken)
                {
                    return true;
                }
                if (scanTypeFlags == ScanTypeFlags.NotType || base.CurrentToken.Kind != SyntaxKind.IdentifierToken)
                {
                    return false;
                }
                if (isGlobalScriptLevel)
                {
                    switch (scanTypeFlags)
                    {
                        case ScanTypeFlags.PointerOrMultiplication:
                            return false;
                        case ScanTypeFlags.NullableType:
                            return IsPossibleDeclarationStatementFollowingNullableType();
                    }
                }
                return true;
            }
            finally
            {
                Reset(ref state);
                Release(ref state);
            }
        }

        private bool IsPossibleTopLevelUsingLocalDeclarationStatement()
        {
            if (base.CurrentToken.Kind != SyntaxKind.UsingKeyword)
            {
                return false;
            }
            SyntaxKind kind = PeekToken(1).Kind;
            if (kind == SyntaxKind.RefKeyword)
            {
                return true;
            }
            if (IsDeclarationModifier(kind))
            {
                if (kind != SyntaxKind.StaticKeyword)
                {
                    return true;
                }
            }
            else if (SyntaxFacts.IsPredefinedType(kind))
            {
                return true;
            }
            ResetPoint state = GetResetPoint();
            try
            {
                EatToken();
                if (kind == SyntaxKind.StaticKeyword)
                {
                    EatToken();
                }
                return IsPossibleFirstTypedIdentifierInLocaDeclarationStatement(isGlobalScriptLevel: false);
            }
            finally
            {
                Reset(ref state);
                Release(ref state);
            }
        }

        private bool IsPossibleDeclarationStatementFollowingNullableType()
        {
            if (IsFieldDeclaration(isEvent: false))
            {
                return IsPossibleFieldDeclarationFollowingNullableType();
            }
            ParseMemberName(out var explicitInterfaceOpt, out var identifierOrThisOpt, out var typeParameterListOpt, isEvent: false);
            if (explicitInterfaceOpt == null && identifierOrThisOpt == null && typeParameterListOpt == null)
            {
                return false;
            }
            if (base.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
            {
                return true;
            }
            if (identifierOrThisOpt.Kind == SyntaxKind.ThisKeyword)
            {
                return false;
            }
            return IsPossibleMethodDeclarationFollowingNullableType();
        }

        private bool IsPossibleFieldDeclarationFollowingNullableType()
        {
            if (base.CurrentToken.Kind != SyntaxKind.IdentifierToken)
            {
                return false;
            }
            EatToken();
            if (base.CurrentToken.Kind == SyntaxKind.EqualsToken)
            {
                TerminatorState termState = _termState;
                _termState |= TerminatorState.IsEndOfFieldDeclaration;
                EatToken();
                ParseVariableInitializer();
                _termState = termState;
            }
            if (base.CurrentToken.Kind != SyntaxKind.CommaToken)
            {
                return base.CurrentToken.Kind == SyntaxKind.SemicolonToken;
            }
            return true;
        }

        private bool IsPossibleMethodDeclarationFollowingNullableType()
        {
            TerminatorState termState = _termState;
            _termState |= TerminatorState.IsEndOfMethodSignature;
            ParameterListSyntax parameterListSyntax = ParseParenthesizedParameterList();
            _termState = termState;
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> withSeparators = parameterListSyntax.Parameters.GetWithSeparators();
            if (!parameterListSyntax.CloseParenToken.IsMissing)
            {
                if (base.CurrentToken.Kind == SyntaxKind.OpenBraceToken || base.CurrentToken.ContextualKind == SyntaxKind.WhereKeyword)
                {
                    return true;
                }
                if (base.CurrentToken.Kind == SyntaxKind.ColonToken)
                {
                    return false;
                }
            }
            if (withSeparators.Count == 0)
            {
                return false;
            }
            ParameterSyntax parameterSyntax = (ParameterSyntax)withSeparators[0];
            if (parameterSyntax.AttributeLists.Count > 0)
            {
                return true;
            }
            for (int i = 0; i < parameterSyntax.Modifiers.Count; i++)
            {
                if (parameterSyntax.Modifiers[i]!.Kind == SyntaxKind.ParamsKeyword)
                {
                    return true;
                }
            }
            if (parameterSyntax.Type == null)
            {
                if (parameterSyntax.Identifier.Kind == SyntaxKind.ArgListKeyword)
                {
                    return true;
                }
            }
            else if (parameterSyntax.Type!.Kind == SyntaxKind.NullableType)
            {
                if (parameterSyntax.Modifiers.Count > 0)
                {
                    return true;
                }
                if (!parameterSyntax.Identifier.IsMissing && ((withSeparators.Count >= 2 && !withSeparators[1]!.IsMissing) || (withSeparators.Count == 1 && !parameterListSyntax.CloseParenToken.IsMissing)))
                {
                    return true;
                }
            }
            else
            {
                if (parameterSyntax.Type!.Kind == SyntaxKind.IdentifierName && ((IdentifierNameSyntax)parameterSyntax.Type).Identifier.ContextualKind == SyntaxKind.FromKeyword)
                {
                    return false;
                }
                if (!parameterSyntax.Identifier.IsMissing)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsPossibleNewExpression()
        {
            SyntaxToken syntaxToken = PeekToken(1);
            SyntaxKind kind = syntaxToken.Kind;
            if (kind == SyntaxKind.OpenBraceToken || kind == SyntaxKind.OpenBracketToken)
            {
                return true;
            }
            if (SyntaxFacts.GetBaseTypeDeclarationKind(syntaxToken.Kind) != 0)
            {
                return false;
            }
            switch (GetModifier(syntaxToken))
            {
                case DeclarationModifiers.Partial:
                    if (SyntaxFacts.IsPredefinedType(PeekToken(2).Kind))
                    {
                        return false;
                    }
                    if (IsPossibleStartOfTypeDeclaration(PeekToken(2).Kind))
                    {
                        return false;
                    }
                    break;
                default:
                    return false;
                case DeclarationModifiers.None:
                    break;
            }
            bool? flag = IsPossibleTypedIdentifierStart(syntaxToken, PeekToken(2), allowThisKeyword: true);
            if (flag.HasValue)
            {
                return !flag.Value;
            }
            ResetPoint state = GetResetPoint();
            try
            {
                EatToken();
                ScanTypeFlags scanTypeFlags = ScanType();
                return !IsPossibleMemberName() || scanTypeFlags == ScanTypeFlags.NotType;
            }
            finally
            {
                Reset(ref state);
                Release(ref state);
            }
        }

        private bool? IsPossibleTypedIdentifierStart(SyntaxToken current, SyntaxToken next, bool allowThisKeyword)
        {
            if (IsTrueIdentifier(current))
            {
                switch (next.Kind)
                {
                    case SyntaxKind.AsteriskToken:
                    case SyntaxKind.OpenBracketToken:
                    case SyntaxKind.LessThanToken:
                    case SyntaxKind.DotToken:
                    case SyntaxKind.QuestionToken:
                    case SyntaxKind.ColonColonToken:
                        return null;
                    case SyntaxKind.OpenParenToken:
                        if (current.IsIdentifierVar())
                        {
                            return null;
                        }
                        return false;
                    case SyntaxKind.IdentifierToken:
                        return IsTrueIdentifier(next);
                    case SyntaxKind.ThisKeyword:
                        return allowThisKeyword;
                    default:
                        return false;
                }
            }
            return null;
        }

        private BlockSyntax ParsePossiblyAttributedBlock()
        {
            return ParseBlock(ParseAttributeDeclarations());
        }

        private BlockSyntax ParseMethodOrAccessorBodyBlock(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, bool isAccessorBody)
        {
            if (IsIncrementalAndFactoryContextMatches && base.CurrentNodeKind == SyntaxKind.Block && attributes.Count == 0)
            {
                return (BlockSyntax)EatNode();
            }
            CSharpSyntaxNode previousNode = ((isAccessorBody && base.CurrentToken.Kind != SyntaxKind.OpenBraceToken) ? AddError(SyntaxFactory.MissingToken(SyntaxKind.OpenBraceToken), IsFeatureEnabled(MessageID.IDS_FeatureExpressionBodiedAccessor) ? ErrorCode.ERR_SemiOrLBraceOrArrowExpected : ErrorCode.ERR_SemiOrLBraceExpected) : EatToken(SyntaxKind.OpenBraceToken));
            SyntaxListBuilder<StatementSyntax> syntaxListBuilder = _pool.Allocate<StatementSyntax>();
            ParseStatements(ref previousNode, syntaxListBuilder, stopOnSwitchSections: false);
            BlockSyntax result = _syntaxFactory.Block(attributes, (SyntaxToken)previousNode, IsLargeEnoughNonEmptyStatementList(syntaxListBuilder) ? new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>(SyntaxList.List(((SyntaxListBuilder)syntaxListBuilder).ToArray())) : syntaxListBuilder, EatToken(SyntaxKind.CloseBraceToken));
            _pool.Free(syntaxListBuilder);
            return result;
        }

        private BlockSyntax ParseBlock(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            if (IsIncrementalAndFactoryContextMatches && base.CurrentNodeKind == SyntaxKind.Block)
            {
                return (BlockSyntax)EatNode();
            }
            CSharpSyntaxNode previousNode = EatToken(SyntaxKind.OpenBraceToken);
            SyntaxListBuilder<StatementSyntax> syntaxListBuilder = _pool.Allocate<StatementSyntax>();
            ParseStatements(ref previousNode, syntaxListBuilder, stopOnSwitchSections: false);
            BlockSyntax result = _syntaxFactory.Block(attributes, (SyntaxToken)previousNode, syntaxListBuilder, EatToken(SyntaxKind.CloseBraceToken));
            _pool.Free(syntaxListBuilder);
            return result;
        }

        private static bool IsLargeEnoughNonEmptyStatementList(SyntaxListBuilder<StatementSyntax> statements)
        {
            if (statements.Count == 0)
            {
                return false;
            }
            if (statements.Count == 1)
            {
                return statements[0]!.Width > 60;
            }
            return true;
        }

        private void ParseStatements(ref CSharpSyntaxNode previousNode, SyntaxListBuilder<StatementSyntax> statements, bool stopOnSwitchSections)
        {
            TerminatorState termState = _termState;
            _termState |= TerminatorState.IsPossibleStatementStartOrStop;
            if (stopOnSwitchSections)
            {
                _termState |= TerminatorState.IsSwitchSectionStart;
            }
            int lastTokenPosition = -1;
            while (base.CurrentToken.Kind != SyntaxKind.CloseBraceToken && base.CurrentToken.Kind != SyntaxKind.EndOfFileToken && (!stopOnSwitchSections || !IsPossibleSwitchSection()) && IsMakingProgress(ref lastTokenPosition))
            {
                if (IsPossibleStatement(acceptAccessibilityMods: true))
                {
                    StatementSyntax statementSyntax = ParsePossiblyAttributedStatement();
                    if (statementSyntax != null)
                    {
                        statements.Add(statementSyntax);
                        continue;
                    }
                }
                PostSkipAction num = SkipBadStatementListTokens(statements, SyntaxKind.CloseBraceToken, out GreenNode trailingTrivia);
                if (trailingTrivia != null)
                {
                    previousNode = AddTrailingSkippedSyntax(previousNode, trailingTrivia);
                }
                if (num == PostSkipAction.Abort)
                {
                    break;
                }
            }
            _termState = termState;
        }

        private bool IsPossibleStatementStartOrStop()
        {
            if (base.CurrentToken.Kind != SyntaxKind.SemicolonToken)
            {
                return IsPossibleStatement(acceptAccessibilityMods: true);
            }
            return true;
        }

        private PostSkipAction SkipBadStatementListTokens(SyntaxListBuilder<StatementSyntax> statements, SyntaxKind expected, out GreenNode trailingTrivia)
        {
            return SkipBadListTokensWithExpectedKindHelper(statements, (LanguageParser p) => !p.IsPossibleStatement(acceptAccessibilityMods: false), (LanguageParser p) => p.CurrentToken.Kind == SyntaxKind.CloseBraceToken || p.IsTerminator(), expected, out trailingTrivia);
        }

        private bool IsPossibleStatement(bool acceptAccessibilityMods)
        {
            SyntaxKind kind = base.CurrentToken.Kind;
            switch (kind)
            {
                case SyntaxKind.OpenBraceToken:
                case SyntaxKind.OpenBracketToken:
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.IfKeyword:
                case SyntaxKind.ElseKeyword:
                case SyntaxKind.WhileKeyword:
                case SyntaxKind.ForKeyword:
                case SyntaxKind.ForEachKeyword:
                case SyntaxKind.DoKeyword:
                case SyntaxKind.SwitchKeyword:
                case SyntaxKind.TryKeyword:
                case SyntaxKind.LockKeyword:
                case SyntaxKind.GotoKeyword:
                case SyntaxKind.BreakKeyword:
                case SyntaxKind.ContinueKeyword:
                case SyntaxKind.ReturnKeyword:
                case SyntaxKind.ThrowKeyword:
                case SyntaxKind.StaticKeyword:
                case SyntaxKind.ReadOnlyKeyword:
                case SyntaxKind.ConstKeyword:
                case SyntaxKind.FixedKeyword:
                case SyntaxKind.VolatileKeyword:
                case SyntaxKind.ExternKeyword:
                case SyntaxKind.RefKeyword:
                case SyntaxKind.UsingKeyword:
                case SyntaxKind.CheckedKeyword:
                case SyntaxKind.UncheckedKeyword:
                case SyntaxKind.UnsafeKeyword:
                    return true;
                case SyntaxKind.IdentifierToken:
                    return IsTrueIdentifier();
                case SyntaxKind.CatchKeyword:
                case SyntaxKind.FinallyKeyword:
                    return !_isInTry;
                case SyntaxKind.PublicKeyword:
                case SyntaxKind.PrivateKeyword:
                case SyntaxKind.InternalKeyword:
                case SyntaxKind.ProtectedKeyword:
                    return acceptAccessibilityMods;
                default:
                    if (!IsPredefinedType(kind))
                    {
                        return IsPossibleExpression();
                    }
                    return true;
            }
        }

        private FixedStatementSyntax ParseFixedStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            SyntaxToken fixedKeyword = EatToken(SyntaxKind.FixedKeyword);
            SyntaxToken openParenToken = EatToken(SyntaxKind.OpenParenToken);
            TerminatorState termState = _termState;
            _termState |= TerminatorState.IsEndOfFixedStatement;
            VariableDeclarationSyntax declaration = ParseVariableDeclaration();
            _termState = termState;
            SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
            StatementSyntax statement = ParseEmbeddedStatement();
            return _syntaxFactory.FixedStatement(attributes, fixedKeyword, openParenToken, declaration, closeParenToken, statement);
        }

        private bool IsEndOfFixedStatement()
        {
            if (base.CurrentToken.Kind != SyntaxKind.CloseParenToken && base.CurrentToken.Kind != SyntaxKind.OpenBraceToken)
            {
                return base.CurrentToken.Kind == SyntaxKind.SemicolonToken;
            }
            return true;
        }

        private StatementSyntax ParseEmbeddedStatement()
        {
            return parseEmbeddedStatementRest(ParsePossiblyAttributedStatement());
            StatementSyntax parseEmbeddedStatementRest(StatementSyntax statement)
            {
                if (statement == null)
                {
                    return SyntaxFactory.EmptyStatement(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), EatToken(SyntaxKind.SemicolonToken));
                }
                if (statement.Kind == SyntaxKind.ExpressionStatement && base.IsScript)
                {
                    ExpressionStatementSyntax expressionStatementSyntax = (ExpressionStatementSyntax)statement;
                    SyntaxToken semicolonToken = expressionStatementSyntax.SemicolonToken;
                    if (semicolonToken.IsMissing && !semicolonToken.GetDiagnostics().Contains((DiagnosticInfo diagnosticInfo) => diagnosticInfo.Code == 1002))
                    {
                        semicolonToken = AddError(semicolonToken, ErrorCode.ERR_SemicolonExpected);
                        return expressionStatementSyntax.Update(expressionStatementSyntax.AttributeLists, expressionStatementSyntax.Expression, semicolonToken);
                    }
                }
                return statement;
            }
        }

        private BreakStatementSyntax ParseBreakStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            SyntaxToken breakKeyword = EatToken(SyntaxKind.BreakKeyword);
            SyntaxToken semicolonToken = EatToken(SyntaxKind.SemicolonToken);
            return _syntaxFactory.BreakStatement(attributes, breakKeyword, semicolonToken);
        }

        private ContinueStatementSyntax ParseContinueStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            SyntaxToken continueKeyword = EatToken(SyntaxKind.ContinueKeyword);
            SyntaxToken semicolonToken = EatToken(SyntaxKind.SemicolonToken);
            return _syntaxFactory.ContinueStatement(attributes, continueKeyword, semicolonToken);
        }

        private TryStatementSyntax ParseTryStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            bool isInTry = _isInTry;
            _isInTry = true;
            SyntaxToken syntaxToken = EatToken(SyntaxKind.TryKeyword);
            BlockSyntax blockSyntax;
            if (syntaxToken.IsMissing)
            {
                blockSyntax = _syntaxFactory.Block(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), EatToken(SyntaxKind.OpenBraceToken), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>), EatToken(SyntaxKind.CloseBraceToken));
            }
            else
            {
                TerminatorState termState = _termState;
                _termState |= TerminatorState.IsEndOfTryBlock;
                blockSyntax = ParsePossiblyAttributedBlock();
                _termState = termState;
            }
            SyntaxListBuilder<CatchClauseSyntax> syntaxListBuilder = default(SyntaxListBuilder<CatchClauseSyntax>);
            FinallyClauseSyntax @finally = null;
            try
            {
                bool flag = false;
                if (base.CurrentToken.Kind == SyntaxKind.CatchKeyword)
                {
                    flag = true;
                    syntaxListBuilder = _pool.Allocate<CatchClauseSyntax>();
                    while (base.CurrentToken.Kind == SyntaxKind.CatchKeyword)
                    {
                        syntaxListBuilder.Add(ParseCatchClause());
                    }
                }
                if (base.CurrentToken.Kind == SyntaxKind.FinallyKeyword)
                {
                    flag = true;
                    SyntaxToken finallyKeyword = EatToken();
                    BlockSyntax block = ParsePossiblyAttributedBlock();
                    @finally = _syntaxFactory.FinallyClause(finallyKeyword, block);
                }
                if (!flag)
                {
                    blockSyntax = AddErrorToLastToken(blockSyntax, ErrorCode.ERR_ExpectedEndTry);
                    @finally = _syntaxFactory.FinallyClause(SyntaxToken.CreateMissing(SyntaxKind.FinallyKeyword, null, null), _syntaxFactory.Block(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), SyntaxToken.CreateMissing(SyntaxKind.OpenBraceToken, null, null), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>), SyntaxToken.CreateMissing(SyntaxKind.CloseBraceToken, null, null)));
                }
                _isInTry = isInTry;
                return _syntaxFactory.TryStatement(attributes, syntaxToken, blockSyntax, syntaxListBuilder, @finally);
            }
            finally
            {
                if (!syntaxListBuilder.IsNull)
                {
                    _pool.Free(syntaxListBuilder);
                }
            }
        }

        private bool IsEndOfTryBlock()
        {
            if (base.CurrentToken.Kind != SyntaxKind.CloseBraceToken && base.CurrentToken.Kind != SyntaxKind.CatchKeyword)
            {
                return base.CurrentToken.Kind == SyntaxKind.FinallyKeyword;
            }
            return true;
        }

        private CatchClauseSyntax ParseCatchClause()
        {
            SyntaxToken catchKeyword = EatToken();
            CatchDeclarationSyntax declaration = null;
            TerminatorState termState = _termState;
            if (base.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                SyntaxToken openParenToken = EatToken();
                _termState |= TerminatorState.IsEndOfCatchClause;
                TypeSyntax type = ParseType();
                SyntaxToken identifier = null;
                if (IsTrueIdentifier())
                {
                    identifier = ParseIdentifierToken();
                }
                _termState = termState;
                SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
                declaration = _syntaxFactory.CatchDeclaration(openParenToken, type, identifier, closeParenToken);
            }
            CatchFilterClauseSyntax filter = null;
            SyntaxKind contextualKind = base.CurrentToken.ContextualKind;
            if (contextualKind == SyntaxKind.WhenKeyword || contextualKind == SyntaxKind.IfKeyword)
            {
                SyntaxToken node = EatContextualToken(SyntaxKind.WhenKeyword);
                if (contextualKind == SyntaxKind.IfKeyword)
                {
                    node = AddTrailingSkippedSyntax(node, EatToken());
                }
                node = CheckFeatureAvailability(node, MessageID.IDS_FeatureExceptionFilter);
                _termState |= TerminatorState.IsEndOfFilterClause;
                SyntaxToken openParenToken2 = EatToken(SyntaxKind.OpenParenToken);
                ExpressionSyntax filterExpression = ParseExpressionCore();
                _termState = termState;
                SyntaxToken closeParenToken2 = EatToken(SyntaxKind.CloseParenToken);
                filter = _syntaxFactory.CatchFilterClause(node, openParenToken2, filterExpression, closeParenToken2);
            }
            _termState |= TerminatorState.IsEndOfCatchBlock;
            BlockSyntax block = ParsePossiblyAttributedBlock();
            _termState = termState;
            return _syntaxFactory.CatchClause(catchKeyword, declaration, filter, block);
        }

        private bool IsEndOfCatchClause()
        {
            if (base.CurrentToken.Kind != SyntaxKind.CloseParenToken && base.CurrentToken.Kind != SyntaxKind.OpenBraceToken && base.CurrentToken.Kind != SyntaxKind.CloseBraceToken && base.CurrentToken.Kind != SyntaxKind.CatchKeyword)
            {
                return base.CurrentToken.Kind == SyntaxKind.FinallyKeyword;
            }
            return true;
        }

        private bool IsEndOfFilterClause()
        {
            if (base.CurrentToken.Kind != SyntaxKind.CloseParenToken && base.CurrentToken.Kind != SyntaxKind.OpenBraceToken && base.CurrentToken.Kind != SyntaxKind.CloseBraceToken && base.CurrentToken.Kind != SyntaxKind.CatchKeyword)
            {
                return base.CurrentToken.Kind == SyntaxKind.FinallyKeyword;
            }
            return true;
        }

        private bool IsEndOfCatchBlock()
        {
            if (base.CurrentToken.Kind != SyntaxKind.CloseBraceToken && base.CurrentToken.Kind != SyntaxKind.CatchKeyword)
            {
                return base.CurrentToken.Kind == SyntaxKind.FinallyKeyword;
            }
            return true;
        }

        private StatementSyntax ParseCheckedStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            if (PeekToken(1).Kind == SyntaxKind.OpenParenToken)
            {
                return ParseExpressionStatement(attributes);
            }
            SyntaxToken syntaxToken = EatToken();
            BlockSyntax block = ParsePossiblyAttributedBlock();
            return _syntaxFactory.CheckedStatement(SyntaxFacts.GetCheckStatement(syntaxToken.Kind), attributes, syntaxToken, block);
        }

        private DoStatementSyntax ParseDoStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            SyntaxToken doKeyword = EatToken(SyntaxKind.DoKeyword);
            StatementSyntax statement = ParseEmbeddedStatement();
            SyntaxToken whileKeyword = EatToken(SyntaxKind.WhileKeyword);
            SyntaxToken openParenToken = EatToken(SyntaxKind.OpenParenToken);
            TerminatorState termState = _termState;
            _termState |= TerminatorState.IsEndOfDoWhileExpression;
            ExpressionSyntax condition = ParseExpressionCore();
            _termState = termState;
            SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
            SyntaxToken semicolonToken = EatToken(SyntaxKind.SemicolonToken);
            return _syntaxFactory.DoStatement(attributes, doKeyword, statement, whileKeyword, openParenToken, condition, closeParenToken, semicolonToken);
        }

        private bool IsEndOfDoWhileExpression()
        {
            if (base.CurrentToken.Kind != SyntaxKind.CloseParenToken)
            {
                return base.CurrentToken.Kind == SyntaxKind.SemicolonToken;
            }
            return true;
        }

        private StatementSyntax ParseForOrForEachStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            ResetPoint state = GetResetPoint();
            try
            {
                EatToken();
                if (EatToken().Kind == SyntaxKind.OpenParenToken && ScanType() != 0 && EatToken().Kind == SyntaxKind.IdentifierToken && EatToken().Kind == SyntaxKind.InKeyword)
                {
                    Reset(ref state);
                    return ParseForEachStatement(attributes, null);
                }
                Reset(ref state);
                return ParseForStatement(attributes);
            }
            finally
            {
                Release(ref state);
            }
        }

        private ForStatementSyntax ParseForStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            SyntaxToken forKeyword = EatToken(SyntaxKind.ForKeyword);
            SyntaxToken startToken = EatToken(SyntaxKind.OpenParenToken);
            TerminatorState termState = _termState;
            _termState |= TerminatorState.IsEndOfForStatementArgument;
            ResetPoint state = GetResetPoint();
            SeparatedSyntaxListBuilder<ExpressionSyntax> builder = _pool.AllocateSeparated<ExpressionSyntax>();
            SeparatedSyntaxListBuilder<ExpressionSyntax> builder2 = _pool.AllocateSeparated<ExpressionSyntax>();
            try
            {
                VariableDeclarationSyntax variableDeclarationSyntax = null;
                bool flag = false;
                if (base.CurrentToken.Kind == SyntaxKind.RefKeyword)
                {
                    flag = true;
                }
                else
                {
                    flag = !IsQueryExpression(mayBeVariableDeclaration: true, mayBeMemberDeclaration: false) && ScanType() != 0 && IsTrueIdentifier();
                    Reset(ref state);
                }
                if (flag)
                {
                    variableDeclarationSyntax = ParseVariableDeclaration();
                    if (variableDeclarationSyntax.Type.Kind == SyntaxKind.RefType)
                    {
                        variableDeclarationSyntax = variableDeclarationSyntax.Update(CheckFeatureAvailability(variableDeclarationSyntax.Type, MessageID.IDS_FeatureRefFor), variableDeclarationSyntax.Variables);
                    }
                }
                else if (base.CurrentToken.Kind != SyntaxKind.SemicolonToken)
                {
                    ParseForStatementExpressionList(ref startToken, builder);
                }
                SyntaxToken firstSemicolonToken = EatToken(SyntaxKind.SemicolonToken);
                ExpressionSyntax condition = null;
                if (base.CurrentToken.Kind != SyntaxKind.SemicolonToken)
                {
                    condition = ParseExpressionCore();
                }
                SyntaxToken startToken2 = EatToken(SyntaxKind.SemicolonToken);
                if (base.CurrentToken.Kind != SyntaxKind.CloseParenToken)
                {
                    ParseForStatementExpressionList(ref startToken2, builder2);
                }
                SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
                StatementSyntax statement = ParseEmbeddedStatement();
                return _syntaxFactory.ForStatement(attributes, forKeyword, startToken, variableDeclarationSyntax, builder, firstSemicolonToken, condition, startToken2, builder2, closeParenToken, statement);
            }
            finally
            {
                _termState = termState;
                Release(ref state);
                _pool.Free(in builder2);
                _pool.Free(in builder);
            }
        }

        private bool IsEndOfForStatementArgument()
        {
            if (base.CurrentToken.Kind != SyntaxKind.SemicolonToken && base.CurrentToken.Kind != SyntaxKind.CloseParenToken)
            {
                return base.CurrentToken.Kind == SyntaxKind.OpenBraceToken;
            }
            return true;
        }

        private void ParseForStatementExpressionList(ref SyntaxToken startToken, SeparatedSyntaxListBuilder<ExpressionSyntax> list)
        {
            if (base.CurrentToken.Kind == SyntaxKind.CloseParenToken || base.CurrentToken.Kind == SyntaxKind.SemicolonToken)
            {
                return;
            }
            do
            {
                if (!IsPossibleExpression() && base.CurrentToken.Kind != SyntaxKind.CommaToken)
                {
                    continue;
                }
                list.Add(ParseExpressionCore());
                int lastTokenPosition = -1;
                while (IsMakingProgress(ref lastTokenPosition) && base.CurrentToken.Kind != SyntaxKind.CloseParenToken && base.CurrentToken.Kind != SyntaxKind.SemicolonToken)
                {
                    if (base.CurrentToken.Kind == SyntaxKind.CommaToken || IsPossibleExpression())
                    {
                        list.AddSeparator(EatToken(SyntaxKind.CommaToken));
                        list.Add(ParseExpressionCore());
                    }
                    else if (SkipBadForStatementExpressionListTokens(ref startToken, list, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                    {
                        break;
                    }
                }
                break;
            }
            while (SkipBadForStatementExpressionListTokens(ref startToken, list, SyntaxKind.IdentifierToken) == PostSkipAction.Continue);
        }

        private PostSkipAction SkipBadForStatementExpressionListTokens(ref SyntaxToken startToken, SeparatedSyntaxListBuilder<ExpressionSyntax> list, SyntaxKind expected)
        {
            return SkipBadSeparatedListTokensWithExpectedKind(ref startToken, list, (LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleExpression(), (LanguageParser p) => p.CurrentToken.Kind == SyntaxKind.CloseParenToken || p.CurrentToken.Kind == SyntaxKind.SemicolonToken || p.IsTerminator(), expected);
        }

        private CommonForEachStatementSyntax ParseForEachStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxToken awaitTokenOpt)
        {
            SyntaxToken forEachKeyword;
            if (base.CurrentToken.Kind == SyntaxKind.ForKeyword)
            {
                SyntaxToken node = EatToken();
                node = AddError(node, ErrorCode.ERR_SyntaxError, SyntaxFacts.GetText(SyntaxKind.ForEachKeyword), SyntaxFacts.GetText(SyntaxKind.ForKeyword));
                forEachKeyword = ConvertToMissingWithTrailingTrivia(node, SyntaxKind.ForEachKeyword);
            }
            else
            {
                forEachKeyword = EatToken(SyntaxKind.ForEachKeyword);
            }
            SyntaxToken openParenToken = EatToken(SyntaxKind.OpenParenToken);
            ExpressionSyntax expressionSyntax = ParseExpressionOrDeclaration(ParseTypeMode.Normal, MessageID.IDS_FeatureTuples, permitTupleDesignation: true);
            SyntaxToken syntaxToken = EatToken(SyntaxKind.InKeyword, ErrorCode.ERR_InExpected);
            if (!IsValidForeachVariable(expressionSyntax))
            {
                syntaxToken = AddError(syntaxToken, ErrorCode.ERR_BadForeachDecl);
            }
            ExpressionSyntax expression = ParseExpressionCore();
            SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
            StatementSyntax statement = ParseEmbeddedStatement();
            DeclarationExpressionSyntax declarationExpressionSyntax = expressionSyntax as DeclarationExpressionSyntax;
            if (declarationExpressionSyntax != null)
            {
                if (declarationExpressionSyntax.Type.Kind == SyntaxKind.RefType)
                {
                    declarationExpressionSyntax = declarationExpressionSyntax.Update(CheckFeatureAvailability(declarationExpressionSyntax.Type, MessageID.IDS_FeatureRefForEach), declarationExpressionSyntax.Designation);
                }
                if (declarationExpressionSyntax.designation.Kind != SyntaxKind.ParenthesizedVariableDesignation)
                {
                    SyntaxToken identifier;
                    switch (declarationExpressionSyntax.designation.Kind)
                    {
                        case SyntaxKind.SingleVariableDesignation:
                            identifier = ((SingleVariableDesignationSyntax)declarationExpressionSyntax.designation).identifier;
                            break;
                        case SyntaxKind.DiscardDesignation:
                            {
                                SyntaxToken underscoreToken = ((DiscardDesignationSyntax)declarationExpressionSyntax.designation).underscoreToken;
                                identifier = SyntaxToken.WithValue(SyntaxKind.IdentifierToken, underscoreToken.LeadingTrivia.Node, underscoreToken.Text, underscoreToken.ValueText, underscoreToken.TrailingTrivia.Node);
                                break;
                            }
                        default:
                            throw ExceptionUtilities.UnexpectedValue(declarationExpressionSyntax.designation.Kind);
                    }
                    return _syntaxFactory.ForEachStatement(attributes, awaitTokenOpt, forEachKeyword, openParenToken, declarationExpressionSyntax.Type, identifier, syntaxToken, expression, closeParenToken, statement);
                }
            }
            return _syntaxFactory.ForEachVariableStatement(attributes, awaitTokenOpt, forEachKeyword, openParenToken, expressionSyntax, syntaxToken, expression, closeParenToken, statement);
        }

        private ExpressionSyntax ParseExpressionOrDeclaration(ParseTypeMode mode, MessageID feature, bool permitTupleDesignation)
        {
            if (!IsPossibleDeclarationExpression(mode, permitTupleDesignation))
            {
                return ParseSubExpression(Precedence.Expression);
            }
            return ParseDeclarationExpression(mode, feature);
        }

        private bool IsPossibleDeclarationExpression(ParseTypeMode mode, bool permitTupleDesignation)
        {
            if (IsInAsync && base.CurrentToken.ContextualKind == SyntaxKind.AwaitKeyword)
            {
                return false;
            }
            ResetPoint state = GetResetPoint();
            try
            {
                bool flag = IsVarType();
                if (ScanType(mode, out var lastTokenOfType) == ScanTypeFlags.NotType)
                {
                    return false;
                }
                if (!ScanDesignation(permitTupleDesignation && (flag || IsPredefinedType(lastTokenOfType.Kind))))
                {
                    return false;
                }
                return mode switch
                {
                    ParseTypeMode.FirstElementOfPossibleTupleLiteral => base.CurrentToken.Kind == SyntaxKind.CommaToken,
                    ParseTypeMode.AfterTupleComma => base.CurrentToken.Kind == SyntaxKind.CommaToken || base.CurrentToken.Kind == SyntaxKind.CloseParenToken,
                    _ => true,
                };
            }
            finally
            {
                Reset(ref state);
                Release(ref state);
            }
        }

        private bool IsVarType()
        {
            if (!base.CurrentToken.IsIdentifierVar())
            {
                return false;
            }
            switch (PeekToken(1).Kind)
            {
                case SyntaxKind.AsteriskToken:
                case SyntaxKind.OpenBracketToken:
                case SyntaxKind.LessThanToken:
                case SyntaxKind.DotToken:
                case SyntaxKind.QuestionToken:
                case SyntaxKind.ColonColonToken:
                    return false;
                default:
                    return true;
            }
        }

        private static bool IsValidForeachVariable(ExpressionSyntax variable)
        {
            return variable.Kind switch
            {
                SyntaxKind.DeclarationExpression => true,
                SyntaxKind.TupleExpression => true,
                SyntaxKind.IdentifierName => ((IdentifierNameSyntax)variable).Identifier.ContextualKind == SyntaxKind.UnderscoreToken,
                _ => false,
            };
        }

        private GotoStatementSyntax ParseGotoStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            SyntaxToken gotoKeyword = EatToken(SyntaxKind.GotoKeyword);
            SyntaxToken syntaxToken = null;
            ExpressionSyntax expression = null;
            SyntaxKind kind;
            if (base.CurrentToken.Kind == SyntaxKind.CaseKeyword || base.CurrentToken.Kind == SyntaxKind.DefaultKeyword)
            {
                syntaxToken = EatToken();
                if (syntaxToken.Kind == SyntaxKind.CaseKeyword)
                {
                    kind = SyntaxKind.GotoCaseStatement;
                    expression = ParseExpressionCore();
                }
                else
                {
                    kind = SyntaxKind.GotoDefaultStatement;
                }
            }
            else
            {
                kind = SyntaxKind.GotoStatement;
                expression = ParseIdentifierName();
            }
            SyntaxToken semicolonToken = EatToken(SyntaxKind.SemicolonToken);
            return _syntaxFactory.GotoStatement(kind, attributes, gotoKeyword, syntaxToken, expression, semicolonToken);
        }

        private IfStatementSyntax ParseIfStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            return _syntaxFactory.IfStatement(attributes, EatToken(SyntaxKind.IfKeyword), EatToken(SyntaxKind.OpenParenToken), ParseExpressionCore(), EatToken(SyntaxKind.CloseParenToken), ParseEmbeddedStatement(), ParseElseClauseOpt());
        }

        private IfStatementSyntax ParseMisplacedElse(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            return _syntaxFactory.IfStatement(attributes, EatToken(SyntaxKind.IfKeyword, ErrorCode.ERR_ElseCannotStartStatement), EatToken(SyntaxKind.OpenParenToken), ParseExpressionCore(), EatToken(SyntaxKind.CloseParenToken), ParseExpressionStatement(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>)), ParseElseClauseOpt());
        }

        private ElseClauseSyntax ParseElseClauseOpt()
        {
            if (base.CurrentToken.Kind != SyntaxKind.ElseKeyword)
            {
                return null;
            }
            SyntaxToken elseKeyword = EatToken(SyntaxKind.ElseKeyword);
            StatementSyntax statement = ParseEmbeddedStatement();
            return _syntaxFactory.ElseClause(elseKeyword, statement);
        }

        private LockStatementSyntax ParseLockStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            SyntaxToken lockKeyword = EatToken(SyntaxKind.LockKeyword);
            SyntaxToken openParenToken = EatToken(SyntaxKind.OpenParenToken);
            ExpressionSyntax expression = ParseExpressionCore();
            SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
            StatementSyntax statement = ParseEmbeddedStatement();
            return _syntaxFactory.LockStatement(attributes, lockKeyword, openParenToken, expression, closeParenToken, statement);
        }

        private ReturnStatementSyntax ParseReturnStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            SyntaxToken returnKeyword = EatToken(SyntaxKind.ReturnKeyword);
            ExpressionSyntax expression = null;
            if (base.CurrentToken.Kind != SyntaxKind.SemicolonToken)
            {
                expression = ParsePossibleRefExpression();
            }
            SyntaxToken semicolonToken = EatToken(SyntaxKind.SemicolonToken);
            return _syntaxFactory.ReturnStatement(attributes, returnKeyword, expression, semicolonToken);
        }

        private YieldStatementSyntax ParseYieldStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            SyntaxToken node = SyntaxParser.ConvertToKeyword(EatToken());
            ExpressionSyntax expression = null;
            node = CheckFeatureAvailability(node, MessageID.IDS_FeatureIterators);
            SyntaxKind kind;
            SyntaxToken syntaxToken;
            if (base.CurrentToken.Kind == SyntaxKind.BreakKeyword)
            {
                kind = SyntaxKind.YieldBreakStatement;
                syntaxToken = EatToken();
            }
            else
            {
                kind = SyntaxKind.YieldReturnStatement;
                syntaxToken = EatToken(SyntaxKind.ReturnKeyword);
                if (base.CurrentToken.Kind == SyntaxKind.SemicolonToken)
                {
                    syntaxToken = AddError(syntaxToken, ErrorCode.ERR_EmptyYield);
                }
                else
                {
                    expression = ParseExpressionCore();
                }
            }
            SyntaxToken semicolonToken = EatToken(SyntaxKind.SemicolonToken);
            return _syntaxFactory.YieldStatement(kind, attributes, node, syntaxToken, expression, semicolonToken);
        }

        private SwitchStatementSyntax ParseSwitchStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            SyntaxToken switchKeyword = EatToken(SyntaxKind.SwitchKeyword);
            ExpressionSyntax expressionSyntax = ParseExpressionCore();
            SyntaxToken openParenToken;
            SyntaxToken closeParenToken;
            if (expressionSyntax.Kind == SyntaxKind.ParenthesizedExpression)
            {
                ParenthesizedExpressionSyntax obj = (ParenthesizedExpressionSyntax)expressionSyntax;
                openParenToken = obj.OpenParenToken;
                expressionSyntax = obj.Expression;
                closeParenToken = obj.CloseParenToken;
            }
            else if (expressionSyntax.Kind == SyntaxKind.TupleExpression)
            {
                openParenToken = (closeParenToken = null);
            }
            else
            {
                openParenToken = SyntaxFactory.MissingToken(SyntaxKind.OpenParenToken);
                expressionSyntax = AddError(expressionSyntax, ErrorCode.ERR_SwitchGoverningExpressionRequiresParens);
                closeParenToken = SyntaxFactory.MissingToken(SyntaxKind.CloseParenToken);
            }
            SyntaxToken openBraceToken = EatToken(SyntaxKind.OpenBraceToken);
            SyntaxListBuilder<SwitchSectionSyntax> syntaxListBuilder = _pool.Allocate<SwitchSectionSyntax>();
            try
            {
                while (IsPossibleSwitchSection())
                {
                    SwitchSectionSyntax node = ParseSwitchSection();
                    syntaxListBuilder.Add(node);
                }
                SyntaxToken closeBraceToken = EatToken(SyntaxKind.CloseBraceToken);
                return _syntaxFactory.SwitchStatement(attributes, switchKeyword, openParenToken, expressionSyntax, closeParenToken, openBraceToken, syntaxListBuilder, closeBraceToken);
            }
            finally
            {
                _pool.Free(syntaxListBuilder);
            }
        }

        private bool IsPossibleSwitchSection()
        {
            if (base.CurrentToken.Kind != SyntaxKind.CaseKeyword)
            {
                if (base.CurrentToken.Kind == SyntaxKind.DefaultKeyword)
                {
                    return PeekToken(1).Kind != SyntaxKind.OpenParenToken;
                }
                return false;
            }
            return true;
        }

        private SwitchSectionSyntax ParseSwitchSection()
        {
            SyntaxListBuilder<SwitchLabelSyntax> syntaxListBuilder = _pool.Allocate<SwitchLabelSyntax>();
            SyntaxListBuilder<StatementSyntax> syntaxListBuilder2 = _pool.Allocate<StatementSyntax>();
            try
            {
                do
                {
                    SwitchLabelSyntax node;
                    if (base.CurrentToken.Kind == SyntaxKind.CaseKeyword)
                    {
                        SyntaxToken keyword = EatToken();
                        if (base.CurrentToken.Kind == SyntaxKind.ColonToken)
                        {
                            ExpressionSyntax value = ParseIdentifierName(ErrorCode.ERR_ConstantExpected);
                            SyntaxToken colonToken = EatToken(SyntaxKind.ColonToken);
                            node = _syntaxFactory.CaseSwitchLabel(keyword, value, colonToken);
                        }
                        else
                        {
                            CSharpSyntaxNode cSharpSyntaxNode = ParseExpressionOrPatternForSwitchStatement();
                            if (base.CurrentToken.ContextualKind == SyntaxKind.WhenKeyword && cSharpSyntaxNode is ExpressionSyntax expression)
                            {
                                cSharpSyntaxNode = _syntaxFactory.ConstantPattern(expression);
                            }
                            if (cSharpSyntaxNode.Kind == SyntaxKind.DiscardPattern)
                            {
                                cSharpSyntaxNode = AddError(cSharpSyntaxNode, ErrorCode.ERR_DiscardPatternInSwitchStatement);
                            }
                            if (cSharpSyntaxNode is PatternSyntax pattern)
                            {
                                WhenClauseSyntax whenClause = ParseWhenClause(Precedence.Expression);
                                SyntaxToken colonToken = EatToken(SyntaxKind.ColonToken);
                                node = _syntaxFactory.CasePatternSwitchLabel(keyword, pattern, whenClause, colonToken);
                                node = CheckFeatureAvailability(node, MessageID.IDS_FeaturePatternMatching);
                            }
                            else
                            {
                                SyntaxToken colonToken = EatToken(SyntaxKind.ColonToken);
                                node = _syntaxFactory.CaseSwitchLabel(keyword, (ExpressionSyntax)cSharpSyntaxNode, colonToken);
                            }
                        }
                    }
                    else
                    {
                        SyntaxToken keyword = EatToken(SyntaxKind.DefaultKeyword);
                        SyntaxToken colonToken = EatToken(SyntaxKind.ColonToken);
                        node = _syntaxFactory.DefaultSwitchLabel(keyword, colonToken);
                    }
                    syntaxListBuilder.Add(node);
                }
                while (IsPossibleSwitchSection());
                CSharpSyntaxNode previousNode = syntaxListBuilder[syntaxListBuilder.Count - 1];
                ParseStatements(ref previousNode, syntaxListBuilder2, stopOnSwitchSections: true);
                syntaxListBuilder[syntaxListBuilder.Count - 1] = (SwitchLabelSyntax)previousNode;
                return _syntaxFactory.SwitchSection(syntaxListBuilder, syntaxListBuilder2);
            }
            finally
            {
                _pool.Free(syntaxListBuilder2);
                _pool.Free(syntaxListBuilder);
            }
        }

        private ThrowStatementSyntax ParseThrowStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            SyntaxToken throwKeyword = EatToken(SyntaxKind.ThrowKeyword);
            ExpressionSyntax expression = null;
            if (base.CurrentToken.Kind != SyntaxKind.SemicolonToken)
            {
                expression = ParseExpressionCore();
            }
            SyntaxToken semicolonToken = EatToken(SyntaxKind.SemicolonToken);
            return _syntaxFactory.ThrowStatement(attributes, throwKeyword, expression, semicolonToken);
        }

        private UnsafeStatementSyntax ParseUnsafeStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            SyntaxToken unsafeKeyword = EatToken(SyntaxKind.UnsafeKeyword);
            BlockSyntax block = ParsePossiblyAttributedBlock();
            return _syntaxFactory.UnsafeStatement(attributes, unsafeKeyword, block);
        }

        private UsingStatementSyntax ParseUsingStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, SyntaxToken awaitTokenOpt = null)
        {
            SyntaxToken usingKeyword = EatToken(SyntaxKind.UsingKeyword);
            SyntaxToken openParenToken = EatToken(SyntaxKind.OpenParenToken);
            VariableDeclarationSyntax declaration = null;
            ExpressionSyntax expression = null;
            ResetPoint resetPoint = GetResetPoint();
            ParseUsingExpression(ref declaration, ref expression, ref resetPoint);
            Release(ref resetPoint);
            SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
            StatementSyntax statement = ParseEmbeddedStatement();
            return _syntaxFactory.UsingStatement(attributes, awaitTokenOpt, usingKeyword, openParenToken, declaration, expression, closeParenToken, statement);
        }

        private void ParseUsingExpression(ref VariableDeclarationSyntax declaration, ref ExpressionSyntax expression, ref ResetPoint resetPoint)
        {
            if (IsAwaitExpression())
            {
                expression = ParseExpressionCore();
                return;
            }
            ScanTypeFlags scanTypeFlags = ((!IsQueryExpression(mayBeVariableDeclaration: true, mayBeMemberDeclaration: false)) ? ScanType() : ScanTypeFlags.NotType);
            if (scanTypeFlags == ScanTypeFlags.NullableType)
            {
                if (base.CurrentToken.Kind != SyntaxKind.IdentifierToken)
                {
                    Reset(ref resetPoint);
                    expression = ParseExpressionCore();
                    return;
                }
                switch (PeekToken(1).Kind)
                {
                    default:
                        Reset(ref resetPoint);
                        expression = ParseExpressionCore();
                        break;
                    case SyntaxKind.CloseParenToken:
                    case SyntaxKind.CommaToken:
                        Reset(ref resetPoint);
                        declaration = ParseVariableDeclaration();
                        break;
                    case SyntaxKind.EqualsToken:
                        Reset(ref resetPoint);
                        declaration = ParseVariableDeclaration();
                        if (base.CurrentToken.Kind == SyntaxKind.ColonToken && declaration.Type.Kind == SyntaxKind.NullableType && SyntaxFacts.IsName(((NullableTypeSyntax)declaration.Type).ElementType.Kind) && declaration.Variables.Count == 1)
                        {
                            Reset(ref resetPoint);
                            declaration = null;
                            expression = ParseExpressionCore();
                        }
                        break;
                }
            }
            else if (IsUsingStatementVariableDeclaration(scanTypeFlags))
            {
                Reset(ref resetPoint);
                declaration = ParseVariableDeclaration();
            }
            else
            {
                Reset(ref resetPoint);
                expression = ParseExpressionCore();
            }
        }

        private bool IsUsingStatementVariableDeclaration(ScanTypeFlags st)
        {
            bool num = st == ScanTypeFlags.MustBeType && base.CurrentToken.Kind != SyntaxKind.DotToken;
            bool flag = st != 0 && base.CurrentToken.Kind == SyntaxKind.IdentifierToken;
            bool flag2 = st == ScanTypeFlags.NonGenericTypeOrExpression || PeekToken(1).Kind == SyntaxKind.EqualsToken;
            if (!num)
            {
                return flag && flag2;
            }
            return true;
        }

        private WhileStatementSyntax ParseWhileStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            SyntaxToken whileKeyword = EatToken(SyntaxKind.WhileKeyword);
            SyntaxToken openParenToken = EatToken(SyntaxKind.OpenParenToken);
            ExpressionSyntax condition = ParseExpressionCore();
            SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
            StatementSyntax statement = ParseEmbeddedStatement();
            return _syntaxFactory.WhileStatement(attributes, whileKeyword, openParenToken, condition, closeParenToken, statement);
        }

        private LabeledStatementSyntax ParseLabeledStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            SyntaxToken identifier = ParseIdentifierToken();
            SyntaxToken colonToken = EatToken(SyntaxKind.ColonToken);
            StatementSyntax statement = ParsePossiblyAttributedStatement() ?? SyntaxFactory.EmptyStatement(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), EatToken(SyntaxKind.SemicolonToken));
            return _syntaxFactory.LabeledStatement(attributes, identifier, colonToken, statement);
        }

        private StatementSyntax ParseLocalDeclarationStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            bool flag = false;
            SyntaxToken awaitKeyword;
            SyntaxToken syntaxToken;
            if (IsPossibleAwaitUsing())
            {
                awaitKeyword = ParseAwaitKeyword(MessageID.None);
                syntaxToken = EatToken();
            }
            else if (base.CurrentToken.Kind == SyntaxKind.UsingKeyword)
            {
                awaitKeyword = null;
                syntaxToken = EatToken();
            }
            else
            {
                awaitKeyword = null;
                syntaxToken = null;
                flag = true;
            }
            if (syntaxToken != null)
            {
                syntaxToken = CheckFeatureAvailability(syntaxToken, MessageID.IDS_FeatureUsingDeclarations);
            }
            SyntaxListBuilder syntaxListBuilder = _pool.Allocate();
            ParseDeclarationModifiers(syntaxListBuilder);
            SeparatedSyntaxListBuilder<VariableDeclaratorSyntax> item = _pool.AllocateSeparated<VariableDeclaratorSyntax>();
            try
            {
                ParseLocalDeclaration(item, flag, attributes, syntaxListBuilder.ToList(), out var type, out var localFunction);
                if (localFunction != null)
                {
                    return localFunction;
                }
                if (flag && attributes.Count == 0 && syntaxListBuilder.Count > 0 && IsAccessibilityModifier(((SyntaxToken)syntaxListBuilder[0]).ContextualKind))
                {
                    return null;
                }
                for (int i = 0; i < syntaxListBuilder.Count; i++)
                {
                    SyntaxToken syntaxToken2 = (SyntaxToken)syntaxListBuilder[i];
                    if (IsAdditionalLocalFunctionModifier(syntaxToken2.ContextualKind))
                    {
                        syntaxListBuilder[i] = AddError(syntaxToken2, ErrorCode.ERR_BadMemberFlag, syntaxToken2.Text);
                    }
                }
                SyntaxToken semicolonToken = EatToken(SyntaxKind.SemicolonToken);
                return _syntaxFactory.LocalDeclarationStatement(attributes, awaitKeyword, syntaxToken, syntaxListBuilder.ToList(), _syntaxFactory.VariableDeclaration(type, item), semicolonToken);
            }
            finally
            {
                _pool.Free(in item);
                _pool.Free(syntaxListBuilder);
            }
        }

        private VariableDesignationSyntax ParseDesignation(bool forPattern)
        {
            VariableDesignationSyntax result;
            if (base.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                SyntaxToken openParenToken = EatToken(SyntaxKind.OpenParenToken);
                SeparatedSyntaxListBuilder<VariableDesignationSyntax> item = _pool.AllocateSeparated<VariableDesignationSyntax>();
                bool flag = false;
                if (forPattern)
                {
                    flag = base.CurrentToken.Kind == SyntaxKind.CloseParenToken;
                }
                else
                {
                    item.Add(ParseDesignation(forPattern));
                    item.AddSeparator(EatToken(SyntaxKind.CommaToken));
                }
                if (!flag)
                {
                    while (true)
                    {
                        item.Add(ParseDesignation(forPattern));
                        if (base.CurrentToken.Kind != SyntaxKind.CommaToken)
                        {
                            break;
                        }
                        item.AddSeparator(EatToken(SyntaxKind.CommaToken));
                    }
                }
                SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
                result = _syntaxFactory.ParenthesizedVariableDesignation(openParenToken, item, closeParenToken);
                _pool.Free(in item);
            }
            else
            {
                result = ParseSimpleDesignation();
            }
            return result;
        }

        private VariableDesignationSyntax ParseSimpleDesignation()
        {
            if (base.CurrentToken.ContextualKind == SyntaxKind.UnderscoreToken)
            {
                SyntaxToken underscoreToken = EatContextualToken(SyntaxKind.UnderscoreToken);
                return _syntaxFactory.DiscardDesignation(underscoreToken);
            }
            SyntaxToken identifier = EatToken(SyntaxKind.IdentifierToken);
            return _syntaxFactory.SingleVariableDesignation(identifier);
        }

        private WhenClauseSyntax ParseWhenClause(Precedence precedence)
        {
            if (base.CurrentToken.ContextualKind != SyntaxKind.WhenKeyword)
            {
                return null;
            }
            SyntaxToken whenKeyword = EatContextualToken(SyntaxKind.WhenKeyword);
            ExpressionSyntax condition = ParseSubExpression(precedence);
            return _syntaxFactory.WhenClause(whenKeyword, condition);
        }

        private VariableDeclarationSyntax ParseVariableDeclaration()
        {
            SeparatedSyntaxListBuilder<VariableDeclaratorSyntax> builder = _pool.AllocateSeparated<VariableDeclaratorSyntax>();
            ParseLocalDeclaration(builder, allowLocalFunctions: false, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>), out var type, out var _);
            VariableDeclarationSyntax result = _syntaxFactory.VariableDeclaration(type, builder);
            _pool.Free(in builder);
            return result;
        }

        private void ParseLocalDeclaration(SeparatedSyntaxListBuilder<VariableDeclaratorSyntax> variables, bool allowLocalFunctions, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> mods, out TypeSyntax type, out LocalFunctionStatementSyntax localFunction)
        {
            type = (allowLocalFunctions ? ParseReturnType() : ParseType());
            VariableFlags variableFlags = VariableFlags.Local;
            if (mods.Any(8350))
            {
                variableFlags |= VariableFlags.Const;
            }
            TerminatorState termState = _termState;
            _termState |= TerminatorState.IsEndOfDeclarationClause;
            ParseVariableDeclarators(type, variableFlags, variables, variableDeclarationsExpected: true, allowLocalFunctions, attributes, mods, out localFunction);
            _termState = termState;
            if (allowLocalFunctions && localFunction == null)
            {
                PredefinedTypeSyntax obj = type as PredefinedTypeSyntax;
                if (obj != null && obj.Keyword.Kind == SyntaxKind.VoidKeyword)
                {
                    type = AddError(type, ErrorCode.ERR_NoVoidHere);
                }
            }
        }

        private bool IsEndOfDeclarationClause()
        {
            SyntaxKind kind = base.CurrentToken.Kind;
            if (kind == SyntaxKind.CloseParenToken || kind - 8211 <= SyntaxKind.List)
            {
                return true;
            }
            return false;
        }

        private void ParseDeclarationModifiers(SyntaxListBuilder list)
        {
            SyntaxKind contextualKind;
            while (IsDeclarationModifier(contextualKind = base.CurrentToken.ContextualKind) || IsAdditionalLocalFunctionModifier(contextualKind))
            {
                SyntaxToken syntaxToken;
                if (contextualKind == SyntaxKind.AsyncKeyword)
                {
                    ResetPoint state = GetResetPoint();
                    bool num = !IsPossibleStartOfTypeDeclaration(EatToken().Kind) && !IsDeclarationModifier(base.CurrentToken.Kind) && !IsAdditionalLocalFunctionModifier(base.CurrentToken.Kind) && (ScanType() == ScanTypeFlags.NotType || base.CurrentToken.Kind != SyntaxKind.IdentifierToken);
                    Reset(ref state);
                    Release(ref state);
                    if (num)
                    {
                        break;
                    }
                    syntaxToken = EatContextualToken(contextualKind);
                    if (contextualKind == SyntaxKind.AsyncKeyword)
                    {
                        syntaxToken = CheckFeatureAvailability(syntaxToken, MessageID.IDS_FeatureAsync);
                    }
                }
                else
                {
                    syntaxToken = EatToken();
                }
                if (contextualKind == SyntaxKind.ReadOnlyKeyword || contextualKind == SyntaxKind.VolatileKeyword)
                {
                    syntaxToken = AddError(syntaxToken, ErrorCode.ERR_BadMemberFlag, syntaxToken.Text);
                }
                else if (list.Any(syntaxToken.RawKind))
                {
                    syntaxToken = AddError(syntaxToken, ErrorCode.ERR_TypeExpected, syntaxToken.Text);
                }
                list.Add(syntaxToken);
            }
        }

        private static bool IsDeclarationModifier(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.StaticKeyword:
                case SyntaxKind.ReadOnlyKeyword:
                case SyntaxKind.ConstKeyword:
                case SyntaxKind.VolatileKeyword:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsAdditionalLocalFunctionModifier(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PublicKeyword:
                case SyntaxKind.PrivateKeyword:
                case SyntaxKind.InternalKeyword:
                case SyntaxKind.ProtectedKeyword:
                case SyntaxKind.StaticKeyword:
                case SyntaxKind.ExternKeyword:
                case SyntaxKind.UnsafeKeyword:
                case SyntaxKind.AsyncKeyword:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsAccessibilityModifier(SyntaxKind kind)
        {
            if (kind - 8343 <= (SyntaxKind)3)
            {
                return true;
            }
            return false;
        }

        private LocalFunctionStatementSyntax TryParseLocalFunctionStatementBody(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> modifiers, TypeSyntax type, SyntaxToken identifier)
        {
            ResetPoint state = GetResetPoint();
            bool flag = true;
            if (type.Kind == SyntaxKind.IdentifierName)
            {
                flag = ((IdentifierNameSyntax)type).Identifier.ContextualKind != SyntaxKind.AwaitKeyword;
            }
            bool isInAsync = IsInAsync;
            IsInAsync = false;
            SyntaxListBuilder syntaxListBuilder = null;
            for (int i = 0; i < modifiers.Count; i++)
            {
                SyntaxToken syntaxToken = modifiers[i];
                SyntaxKind contextualKind = syntaxToken.ContextualKind;
                if (contextualKind <= SyntaxKind.VolatileKeyword)
                {
                    if (contextualKind != SyntaxKind.StaticKeyword)
                    {
                        if (contextualKind == SyntaxKind.ReadOnlyKeyword || contextualKind == SyntaxKind.VolatileKeyword)
                        {
                            continue;
                        }
                        goto IL_00f1;
                    }
                    syntaxToken = CheckFeatureAvailability(syntaxToken, MessageID.IDS_FeatureStaticLocalFunctions);
                    if (syntaxToken == modifiers[i])
                    {
                        continue;
                    }
                }
                else
                {
                    if (contextualKind != SyntaxKind.ExternKeyword)
                    {
                        switch (contextualKind)
                        {
                            case SyntaxKind.AsyncKeyword:
                                IsInAsync = true;
                                flag = true;
                                continue;
                            case SyntaxKind.UnsafeKeyword:
                                flag = true;
                                continue;
                        }
                        goto IL_00f1;
                    }
                    syntaxToken = CheckFeatureAvailability(syntaxToken, MessageID.IDS_FeatureExternLocalFunctions);
                    if (syntaxToken == modifiers[i])
                    {
                        continue;
                    }
                }
                goto IL_010d;
            IL_00f1:
                syntaxToken = AddError(syntaxToken, ErrorCode.ERR_BadMemberFlag, syntaxToken.Text);
                goto IL_010d;
            IL_010d:
                if (syntaxListBuilder == null)
                {
                    syntaxListBuilder = _pool.Allocate();
                    syntaxListBuilder.AddRange(modifiers);
                }
                syntaxListBuilder[i] = syntaxToken;
            }
            if (syntaxListBuilder != null)
            {
                modifiers = syntaxListBuilder.ToList();
                _pool.Free(syntaxListBuilder);
            }
            TypeParameterListSyntax typeParameterList = ParseTypeParameterList();
            ParameterListSyntax parameterListSyntax = ParseParenthesizedParameterList();
            if (!flag)
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ParameterSyntax> parameters = parameterListSyntax.Parameters;
                for (int j = 0; j < parameters.Count; j++)
                {
                    flag |= !parameters[j]!.ContainsDiagnostics;
                    if (flag)
                    {
                        break;
                    }
                }
            }
            SyntaxListBuilder<TypeParameterConstraintClauseSyntax> syntaxListBuilder2 = default(SyntaxListBuilder<TypeParameterConstraintClauseSyntax>);
            if (base.CurrentToken.ContextualKind == SyntaxKind.WhereKeyword)
            {
                syntaxListBuilder2 = _pool.Allocate<TypeParameterConstraintClauseSyntax>();
                ParseTypeParameterConstraintClauses(syntaxListBuilder2);
                flag = true;
            }
            ParseBlockAndExpressionBodiesWithSemicolon(out var blockBody, out var expressionBody, out var semicolon, parseSemicolonAfterBlock: false);
            IsInAsync = isInAsync;
            if (!flag && blockBody == null && expressionBody == null)
            {
                Reset(ref state);
                Release(ref state);
                return null;
            }
            Release(ref state);
            identifier = CheckFeatureAvailability(identifier, MessageID.IDS_FeatureLocalFunctions);
            return _syntaxFactory.LocalFunctionStatement(attributes, modifiers, type, identifier, typeParameterList, parameterListSyntax, syntaxListBuilder2, blockBody, expressionBody, semicolon);
        }

        private ExpressionStatementSyntax ParseExpressionStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes)
        {
            return ParseExpressionStatement(attributes, ParseExpressionCore());
        }

        private ExpressionStatementSyntax ParseExpressionStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes, ExpressionSyntax expression)
        {
            SyntaxToken semicolonToken = ((!base.IsScript || base.CurrentToken.Kind != SyntaxKind.EndOfFileToken) ? EatToken(SyntaxKind.SemicolonToken) : SyntaxFactory.MissingToken(SyntaxKind.SemicolonToken));
            return _syntaxFactory.ExpressionStatement(attributes, expression, semicolonToken);
        }

        public ExpressionSyntax ParseExpression()
        {
            return ParseWithStackGuard(ParseExpressionCore, CreateMissingIdentifierName);
        }

        private ExpressionSyntax ParseExpressionCore()
        {
            return ParseSubExpression(Precedence.Expression);
        }

        private bool CanStartExpression()
        {
            return IsPossibleExpression(allowBinaryExpressions: false, allowAssignmentExpressions: false, allowAttributes: false);
        }

        private bool IsPossibleExpression()
        {
            return IsPossibleExpression(allowBinaryExpressions: true, allowAssignmentExpressions: true, allowAttributes: true);
        }

        private bool IsPossibleExpression(bool allowBinaryExpressions, bool allowAssignmentExpressions, bool allowAttributes)
        {
            SyntaxKind kind = base.CurrentToken.Kind;
            switch (kind)
            {
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.DotDotToken:
                case SyntaxKind.ColonColonToken:
                case SyntaxKind.TypeOfKeyword:
                case SyntaxKind.SizeOfKeyword:
                case SyntaxKind.NullKeyword:
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.DefaultKeyword:
                case SyntaxKind.ThrowKeyword:
                case SyntaxKind.StackAllocKeyword:
                case SyntaxKind.NewKeyword:
                case SyntaxKind.RefKeyword:
                case SyntaxKind.ArgListKeyword:
                case SyntaxKind.MakeRefKeyword:
                case SyntaxKind.RefTypeKeyword:
                case SyntaxKind.RefValueKeyword:
                case SyntaxKind.ThisKeyword:
                case SyntaxKind.BaseKeyword:
                case SyntaxKind.DelegateKeyword:
                case SyntaxKind.CheckedKeyword:
                case SyntaxKind.UncheckedKeyword:
                case SyntaxKind.InterpolatedStringStartToken:
                case SyntaxKind.NumericLiteralToken:
                case SyntaxKind.CharacterLiteralToken:
                case SyntaxKind.StringLiteralToken:
                case SyntaxKind.InterpolatedStringToken:
                    return true;
                case SyntaxKind.StaticKeyword:
                    if (!IsPossibleAnonymousMethodExpression())
                    {
                        return IsPossibleLambdaExpression(Precedence.Expression);
                    }
                    return true;
                case SyntaxKind.OpenBracketToken:
                    if (allowAttributes)
                    {
                        return IsPossibleLambdaExpression(Precedence.Expression);
                    }
                    return false;
                case SyntaxKind.IdentifierToken:
                    if (!IsTrueIdentifier())
                    {
                        return base.CurrentToken.ContextualKind == SyntaxKind.FromKeyword;
                    }
                    return true;
                default:
                    if ((!IsPredefinedType(kind) || kind == SyntaxKind.VoidKeyword) && !SyntaxFacts.IsAnyUnaryExpression(kind) && (!allowBinaryExpressions || !SyntaxFacts.IsBinaryExpression(kind)))
                    {
                        if (allowAssignmentExpressions)
                        {
                            return SyntaxFacts.IsAssignmentExpressionOperatorToken(kind);
                        }
                        return false;
                    }
                    return true;
            }
        }

        private static bool IsInvalidSubExpression(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.IfKeyword:
                case SyntaxKind.ElseKeyword:
                case SyntaxKind.WhileKeyword:
                case SyntaxKind.ForKeyword:
                case SyntaxKind.ForEachKeyword:
                case SyntaxKind.DoKeyword:
                case SyntaxKind.SwitchKeyword:
                case SyntaxKind.CaseKeyword:
                case SyntaxKind.TryKeyword:
                case SyntaxKind.CatchKeyword:
                case SyntaxKind.FinallyKeyword:
                case SyntaxKind.LockKeyword:
                case SyntaxKind.GotoKeyword:
                case SyntaxKind.BreakKeyword:
                case SyntaxKind.ContinueKeyword:
                case SyntaxKind.ReturnKeyword:
                case SyntaxKind.ConstKeyword:
                case SyntaxKind.UsingKeyword:
                    return true;
                default:
                    return false;
            }
        }

        internal static bool IsRightAssociative(SyntaxKind op)
        {
            if (op == SyntaxKind.CoalesceExpression || op - 8714 <= (SyntaxKind)11)
            {
                return true;
            }
            return false;
        }

        private static Precedence GetPrecedence(SyntaxKind op)
        {
            switch (op)
            {
                case SyntaxKind.QueryExpression:
                    return Precedence.Expression;
                case SyntaxKind.AnonymousMethodExpression:
                case SyntaxKind.SimpleLambdaExpression:
                case SyntaxKind.ParenthesizedLambdaExpression:
                    return Precedence.Expression;
                case SyntaxKind.SimpleAssignmentExpression:
                case SyntaxKind.AddAssignmentExpression:
                case SyntaxKind.SubtractAssignmentExpression:
                case SyntaxKind.MultiplyAssignmentExpression:
                case SyntaxKind.DivideAssignmentExpression:
                case SyntaxKind.ModuloAssignmentExpression:
                case SyntaxKind.AndAssignmentExpression:
                case SyntaxKind.ExclusiveOrAssignmentExpression:
                case SyntaxKind.OrAssignmentExpression:
                case SyntaxKind.LeftShiftAssignmentExpression:
                case SyntaxKind.RightShiftAssignmentExpression:
                case SyntaxKind.CoalesceAssignmentExpression:
                    return Precedence.Expression;
                case SyntaxKind.CoalesceExpression:
                case SyntaxKind.ThrowExpression:
                    return Precedence.Coalescing;
                case SyntaxKind.LogicalOrExpression:
                    return Precedence.ConditionalOr;
                case SyntaxKind.LogicalAndExpression:
                    return Precedence.ConditionalAnd;
                case SyntaxKind.BitwiseOrExpression:
                    return Precedence.LogicalOr;
                case SyntaxKind.ExclusiveOrExpression:
                    return Precedence.LogicalXor;
                case SyntaxKind.BitwiseAndExpression:
                    return Precedence.LogicalAnd;
                case SyntaxKind.EqualsExpression:
                case SyntaxKind.NotEqualsExpression:
                    return Precedence.Equality;
                case SyntaxKind.IsPatternExpression:
                case SyntaxKind.LessThanExpression:
                case SyntaxKind.LessThanOrEqualExpression:
                case SyntaxKind.GreaterThanExpression:
                case SyntaxKind.GreaterThanOrEqualExpression:
                case SyntaxKind.IsExpression:
                case SyntaxKind.AsExpression:
                    return Precedence.Relational;
                case SyntaxKind.SwitchExpression:
                case SyntaxKind.WithExpression:
                    return Precedence.Switch;
                case SyntaxKind.LeftShiftExpression:
                case SyntaxKind.RightShiftExpression:
                    return Precedence.Shift;
                case SyntaxKind.AddExpression:
                case SyntaxKind.SubtractExpression:
                    return Precedence.Additive;
                case SyntaxKind.MultiplyExpression:
                case SyntaxKind.DivideExpression:
                case SyntaxKind.ModuloExpression:
                    return Precedence.Mutiplicative;
                case SyntaxKind.UnaryPlusExpression:
                case SyntaxKind.UnaryMinusExpression:
                case SyntaxKind.BitwiseNotExpression:
                case SyntaxKind.LogicalNotExpression:
                case SyntaxKind.PreIncrementExpression:
                case SyntaxKind.PreDecrementExpression:
                case SyntaxKind.AwaitExpression:
                case SyntaxKind.IndexExpression:
                case SyntaxKind.TypeOfExpression:
                case SyntaxKind.SizeOfExpression:
                case SyntaxKind.CheckedExpression:
                case SyntaxKind.UncheckedExpression:
                case SyntaxKind.MakeRefExpression:
                case SyntaxKind.RefValueExpression:
                case SyntaxKind.RefTypeExpression:
                    return Precedence.Unary;
                case SyntaxKind.CastExpression:
                    return Precedence.Cast;
                case SyntaxKind.PointerIndirectionExpression:
                    return Precedence.PointerIndirection;
                case SyntaxKind.AddressOfExpression:
                    return Precedence.AddressOf;
                case SyntaxKind.RangeExpression:
                    return Precedence.Range;
                case SyntaxKind.ConditionalExpression:
                    return Precedence.Expression;
                case SyntaxKind.IdentifierName:
                case SyntaxKind.GenericName:
                case SyntaxKind.AliasQualifiedName:
                case SyntaxKind.PredefinedType:
                case SyntaxKind.ParenthesizedExpression:
                case SyntaxKind.InvocationExpression:
                case SyntaxKind.ElementAccessExpression:
                case SyntaxKind.ObjectCreationExpression:
                case SyntaxKind.AnonymousObjectCreationExpression:
                case SyntaxKind.ArrayCreationExpression:
                case SyntaxKind.ImplicitArrayCreationExpression:
                case SyntaxKind.StackAllocArrayCreationExpression:
                case SyntaxKind.InterpolatedStringExpression:
                case SyntaxKind.ImplicitObjectCreationExpression:
                case SyntaxKind.SimpleMemberAccessExpression:
                case SyntaxKind.PointerMemberAccessExpression:
                case SyntaxKind.ConditionalAccessExpression:
                case SyntaxKind.PostIncrementExpression:
                case SyntaxKind.PostDecrementExpression:
                case SyntaxKind.ThisExpression:
                case SyntaxKind.BaseExpression:
                case SyntaxKind.ArgListExpression:
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.StringLiteralExpression:
                case SyntaxKind.CharacterLiteralExpression:
                case SyntaxKind.TrueLiteralExpression:
                case SyntaxKind.FalseLiteralExpression:
                case SyntaxKind.NullLiteralExpression:
                case SyntaxKind.DefaultLiteralExpression:
                case SyntaxKind.DefaultExpression:
                case SyntaxKind.TupleExpression:
                case SyntaxKind.DeclarationExpression:
                case SyntaxKind.RefExpression:
                case SyntaxKind.ImplicitStackAllocArrayCreationExpression:
                case SyntaxKind.SuppressNullableWarningExpression:
                    return Precedence.Primary;
                default:
                    throw ExceptionUtilities.UnexpectedValue(op);
            }
        }

        private static bool IsExpectedPrefixUnaryOperator(SyntaxKind kind)
        {
            if (SyntaxFacts.IsPrefixUnaryExpression(kind) && kind != SyntaxKind.RefKeyword)
            {
                return kind != SyntaxKind.OutKeyword;
            }
            return false;
        }

        private static bool IsExpectedBinaryOperator(SyntaxKind kind)
        {
            return SyntaxFacts.IsBinaryExpression(kind);
        }

        private static bool IsExpectedAssignmentOperator(SyntaxKind kind)
        {
            return SyntaxFacts.IsAssignmentExpressionOperatorToken(kind);
        }

        private bool IsPossibleAwaitExpressionStatement()
        {
            if (base.IsScript || IsInAsync)
            {
                return base.CurrentToken.ContextualKind == SyntaxKind.AwaitKeyword;
            }
            return false;
        }

        private bool IsAwaitExpression()
        {
            if (base.CurrentToken.ContextualKind == SyntaxKind.AwaitKeyword)
            {
                if (IsInAsync)
                {
                    return true;
                }
                SyntaxToken syntaxToken = PeekToken(1);
                switch (syntaxToken.Kind)
                {
                    case SyntaxKind.IdentifierToken:
                        return syntaxToken.ContextualKind != SyntaxKind.WithKeyword;
                    case SyntaxKind.TypeOfKeyword:
                    case SyntaxKind.NullKeyword:
                    case SyntaxKind.TrueKeyword:
                    case SyntaxKind.FalseKeyword:
                    case SyntaxKind.DefaultKeyword:
                    case SyntaxKind.NewKeyword:
                    case SyntaxKind.ThisKeyword:
                    case SyntaxKind.BaseKeyword:
                    case SyntaxKind.DelegateKeyword:
                    case SyntaxKind.CheckedKeyword:
                    case SyntaxKind.UncheckedKeyword:
                    case SyntaxKind.InterpolatedStringStartToken:
                    case SyntaxKind.NumericLiteralToken:
                    case SyntaxKind.CharacterLiteralToken:
                    case SyntaxKind.StringLiteralToken:
                    case SyntaxKind.InterpolatedStringToken:
                        return true;
                }
            }
            return false;
        }

        private ExpressionSyntax ParseSubExpression(Precedence precedence)
        {
            _recursionDepth++;
            StackGuard.EnsureSufficientExecutionStack(_recursionDepth);
            ExpressionSyntax result = ParseSubExpressionCore(precedence);
            _recursionDepth--;
            return result;
        }

        private ExpressionSyntax ParseSubExpressionCore(Precedence precedence)
        {
            Precedence precedence2 = Precedence.Expression;
            SyntaxKind kind = base.CurrentToken.Kind;
            if (IsInvalidSubExpression(kind))
            {
                return AddError(CreateMissingIdentifierName(), ErrorCode.ERR_InvalidExprTerm, SyntaxFacts.GetText(kind));
            }
            ExpressionSyntax leftOperand;
            if (IsExpectedPrefixUnaryOperator(kind))
            {
                SyntaxKind prefixUnaryExpression = SyntaxFacts.GetPrefixUnaryExpression(kind);
                precedence2 = GetPrecedence(prefixUnaryExpression);
                SyntaxToken operatorToken = EatToken();
                ExpressionSyntax operand = ParseSubExpression(precedence2);
                leftOperand = _syntaxFactory.PrefixUnaryExpression(prefixUnaryExpression, operatorToken, operand);
            }
            else if (kind == SyntaxKind.DotDotToken)
            {
                SyntaxToken operatorToken2 = EatToken();
                precedence2 = GetPrecedence(SyntaxKind.RangeExpression);
                ExpressionSyntax rightOperand = ((!CanStartExpression()) ? null : ParseSubExpression(precedence2));
                leftOperand = _syntaxFactory.RangeExpression(null, operatorToken2, rightOperand);
            }
            else if (IsAwaitExpression())
            {
                precedence2 = GetPrecedence(SyntaxKind.AwaitExpression);
                SyntaxToken node = EatContextualToken(SyntaxKind.AwaitKeyword);
                node = CheckFeatureAvailability(node, MessageID.IDS_FeatureAsync);
                ExpressionSyntax expression = ParseSubExpression(precedence2);
                leftOperand = _syntaxFactory.AwaitExpression(node, expression);
            }
            else if (IsQueryExpression(mayBeVariableDeclaration: false, mayBeMemberDeclaration: false))
            {
                leftOperand = ParseQueryExpression(precedence);
            }
            else if (base.CurrentToken.ContextualKind == SyntaxKind.FromKeyword && IsInQuery)
            {
                SyntaxToken node2 = EatToken();
                node2 = AddError(node2, ErrorCode.ERR_InvalidExprTerm, base.CurrentToken.Text);
                leftOperand = AddTrailingSkippedSyntax(CreateMissingIdentifierName(), node2);
            }
            else
            {
                if (kind == SyntaxKind.ThrowKeyword)
                {
                    ExpressionSyntax expressionSyntax = ParseThrowExpression();
                    if (precedence > Precedence.Coalescing)
                    {
                        return AddError(expressionSyntax, ErrorCode.ERR_InvalidExprTerm, SyntaxFacts.GetText(kind));
                    }
                    return expressionSyntax;
                }
                leftOperand = ((!IsPossibleDeconstructionLeft(precedence)) ? ParseTerm(precedence) : ParseDeclarationExpression(ParseTypeMode.Normal, MessageID.IDS_FeatureTuples));
            }
            return ParseExpressionContinued(leftOperand, precedence);
        }

        private ExpressionSyntax ParseExpressionContinued(ExpressionSyntax leftOperand, Precedence precedence)
        {
            while (true)
            {
                SyntaxKind contextualKind = base.CurrentToken.ContextualKind;
                bool flag = false;
                SyntaxKind syntaxKind;
                if (IsExpectedBinaryOperator(contextualKind))
                {
                    syntaxKind = SyntaxFacts.GetBinaryExpression(contextualKind);
                }
                else if (IsExpectedAssignmentOperator(contextualKind))
                {
                    syntaxKind = SyntaxFacts.GetAssignmentExpression(contextualKind);
                    flag = true;
                }
                else if (contextualKind == SyntaxKind.DotDotToken)
                {
                    syntaxKind = SyntaxKind.RangeExpression;
                }
                else if (contextualKind == SyntaxKind.SwitchKeyword && PeekToken(1).Kind == SyntaxKind.OpenBraceToken)
                {
                    syntaxKind = SyntaxKind.SwitchExpression;
                }
                else
                {
                    if (contextualKind != SyntaxKind.WithKeyword || PeekToken(1).Kind != SyntaxKind.OpenBraceToken)
                    {
                        break;
                    }
                    syntaxKind = SyntaxKind.WithExpression;
                }
                Precedence precedence2 = GetPrecedence(syntaxKind);
                bool flag2 = false;
                if (contextualKind == SyntaxKind.GreaterThanToken && (PeekToken(1).Kind == SyntaxKind.GreaterThanToken || PeekToken(1).Kind == SyntaxKind.GreaterThanEqualsToken) && base.CurrentToken.GetTrailingTriviaWidth() == 0 && PeekToken(1).GetLeadingTriviaWidth() == 0)
                {
                    if (PeekToken(1).Kind == SyntaxKind.GreaterThanToken)
                    {
                        syntaxKind = SyntaxFacts.GetBinaryExpression(SyntaxKind.GreaterThanGreaterThanToken);
                    }
                    else
                    {
                        syntaxKind = SyntaxFacts.GetAssignmentExpression(SyntaxKind.GreaterThanGreaterThanEqualsToken);
                        flag = true;
                    }
                    precedence2 = GetPrecedence(syntaxKind);
                    flag2 = true;
                }
                if (precedence2 < precedence || (precedence2 == precedence && !IsRightAssociative(syntaxKind)))
                {
                    break;
                }
                SyntaxToken syntaxToken = EatContextualToken(contextualKind);
                Precedence precedence3 = GetPrecedence(leftOperand.Kind);
                if (precedence2 > precedence3)
                {
                    ErrorCode code = ((leftOperand.Kind == SyntaxKind.IsPatternExpression) ? ErrorCode.ERR_UnexpectedToken : ErrorCode.WRN_PrecedenceInversion);
                    syntaxToken = AddError(syntaxToken, code, syntaxToken.Text);
                }
                if (flag2)
                {
                    SyntaxToken syntaxToken2 = EatToken();
                    SyntaxKind kind = ((syntaxToken2.Kind == SyntaxKind.GreaterThanToken) ? SyntaxKind.GreaterThanGreaterThanToken : SyntaxKind.GreaterThanGreaterThanEqualsToken);
                    syntaxToken = SyntaxFactory.Token(syntaxToken.GetLeadingTrivia(), kind, syntaxToken2.GetTrailingTrivia());
                }
                switch (syntaxKind)
                {
                    case SyntaxKind.AsExpression:
                        {
                            TypeSyntax right = ParseType(ParseTypeMode.AsExpression);
                            leftOperand = _syntaxFactory.BinaryExpression(syntaxKind, leftOperand, syntaxToken, right);
                            continue;
                        }
                    case SyntaxKind.IsExpression:
                        leftOperand = ParseIsExpression(leftOperand, syntaxToken);
                        continue;
                }
                if (flag)
                {
                    ExpressionSyntax expressionSyntax = ((syntaxKind != SyntaxKind.SimpleAssignmentExpression || base.CurrentToken.Kind != SyntaxKind.RefKeyword) ? (expressionSyntax = ParseSubExpression(precedence2)) : (expressionSyntax = CheckFeatureAvailability(ParsePossibleRefExpression(), MessageID.IDS_FeatureRefReassignment)));
                    if (syntaxKind == SyntaxKind.CoalesceAssignmentExpression)
                    {
                        syntaxToken = CheckFeatureAvailability(syntaxToken, MessageID.IDS_FeatureCoalesceAssignmentExpression);
                    }
                    leftOperand = _syntaxFactory.AssignmentExpression(syntaxKind, leftOperand, syntaxToken, expressionSyntax);
                    continue;
                }
                switch (syntaxKind)
                {
                    case SyntaxKind.SwitchExpression:
                        leftOperand = ParseSwitchExpression(leftOperand, syntaxToken);
                        continue;
                    case SyntaxKind.WithExpression:
                        leftOperand = ParseWithExpression(leftOperand, syntaxToken);
                        continue;
                }
                if (contextualKind == SyntaxKind.DotDotToken)
                {
                    ExpressionSyntax rightOperand;
                    if (CanStartExpression())
                    {
                        precedence2 = GetPrecedence(syntaxKind);
                        rightOperand = ParseSubExpression(precedence2);
                    }
                    else
                    {
                        rightOperand = null;
                    }
                    leftOperand = _syntaxFactory.RangeExpression(leftOperand, syntaxToken, rightOperand);
                }
                else
                {
                    leftOperand = _syntaxFactory.BinaryExpression(syntaxKind, leftOperand, syntaxToken, ParseSubExpression(precedence2));
                }
            }
            if (base.CurrentToken.Kind == SyntaxKind.QuestionToken && precedence <= Precedence.Conditional)
            {
                SyntaxToken questionToken = EatToken();
                ExpressionSyntax whenTrue = ParsePossibleRefExpression();
                if (base.CurrentToken.Kind == SyntaxKind.EndOfFileToken && lexer.InterpolationFollowedByColon)
                {
                    SyntaxToken colonToken = SyntaxFactory.MissingToken(SyntaxKind.ColonToken);
                    IdentifierNameSyntax whenFalse = _syntaxFactory.IdentifierName(SyntaxFactory.MissingToken(SyntaxKind.IdentifierToken));
                    leftOperand = _syntaxFactory.ConditionalExpression(leftOperand, questionToken, whenTrue, colonToken, whenFalse);
                    leftOperand = AddError(leftOperand, ErrorCode.ERR_ConditionalInInterpolation);
                }
                else
                {
                    SyntaxToken colonToken2 = EatToken(SyntaxKind.ColonToken);
                    ExpressionSyntax whenFalse2 = ParsePossibleRefExpression();
                    leftOperand = _syntaxFactory.ConditionalExpression(leftOperand, questionToken, whenTrue, colonToken2, whenFalse2);
                }
            }
            return leftOperand;
        }

        private ExpressionSyntax ParseDeclarationExpression(ParseTypeMode mode, MessageID feature)
        {
            TypeSyntax type = ParseType(mode);
            VariableDesignationSyntax variableDesignationSyntax = ParseDesignation(forPattern: false);
            if (feature != 0)
            {
                variableDesignationSyntax = CheckFeatureAvailability(variableDesignationSyntax, feature);
            }
            return _syntaxFactory.DeclarationExpression(type, variableDesignationSyntax);
        }

        private ExpressionSyntax ParseThrowExpression()
        {
            SyntaxToken throwKeyword = EatToken(SyntaxKind.ThrowKeyword);
            ExpressionSyntax expression = ParseSubExpression(Precedence.Coalescing);
            ThrowExpressionSyntax node = _syntaxFactory.ThrowExpression(throwKeyword, expression);
            return CheckFeatureAvailability(node, MessageID.IDS_FeatureThrowExpression);
        }

        private ExpressionSyntax ParseIsExpression(ExpressionSyntax leftOperand, SyntaxToken opToken)
        {
            CSharpSyntaxNode cSharpSyntaxNode = ParseTypeOrPatternForIsOperator();
            if (!(cSharpSyntaxNode is PatternSyntax pattern))
            {
                if (cSharpSyntaxNode is TypeSyntax right)
                {
                    return _syntaxFactory.BinaryExpression(SyntaxKind.IsExpression, leftOperand, opToken, right);
                }
                throw ExceptionUtilities.UnexpectedValue(cSharpSyntaxNode);
            }
            IsPatternExpressionSyntax node = _syntaxFactory.IsPatternExpression(leftOperand, opToken, pattern);
            return CheckFeatureAvailability(node, MessageID.IDS_FeaturePatternMatching);
        }

        private ExpressionSyntax ParseTerm(Precedence precedence)
        {
            return ParsePostFixExpression(ParseTermWithoutPostfix(precedence));
        }

        private ExpressionSyntax ParseTermWithoutPostfix(Precedence precedence)
        {
            SyntaxKind kind = base.CurrentToken.Kind;
            switch (kind)
            {
                case SyntaxKind.TypeOfKeyword:
                    return ParseTypeOfExpression();
                case SyntaxKind.DefaultKeyword:
                    return ParseDefaultExpression();
                case SyntaxKind.SizeOfKeyword:
                    return ParseSizeOfExpression();
                case SyntaxKind.MakeRefKeyword:
                    return ParseMakeRefExpression();
                case SyntaxKind.RefTypeKeyword:
                    return ParseRefTypeExpression();
                case SyntaxKind.CheckedKeyword:
                case SyntaxKind.UncheckedKeyword:
                    return ParseCheckedOrUncheckedExpression();
                case SyntaxKind.RefValueKeyword:
                    return ParseRefValueExpression();
                case SyntaxKind.ColonColonToken:
                    return ParseAliasQualifiedName(NameOptions.InExpression);
                case SyntaxKind.EqualsGreaterThanToken:
                    return ParseLambdaExpression();
                case SyntaxKind.StaticKeyword:
                    if (IsPossibleAnonymousMethodExpression())
                    {
                        return ParseAnonymousMethodExpression();
                    }
                    if (IsPossibleLambdaExpression(precedence))
                    {
                        return ParseLambdaExpression();
                    }
                    return AddError(CreateMissingIdentifierName(), ErrorCode.ERR_InvalidExprTerm, base.CurrentToken.Text);
                case SyntaxKind.IdentifierToken:
                    if (IsTrueIdentifier())
                    {
                        if (IsPossibleAnonymousMethodExpression())
                        {
                            return ParseAnonymousMethodExpression();
                        }
                        if (IsPossibleLambdaExpression(precedence))
                        {
                            return ParseLambdaExpression();
                        }
                        if (IsPossibleDeconstructionLeft(precedence))
                        {
                            return ParseDeclarationExpression(ParseTypeMode.Normal, MessageID.IDS_FeatureTuples);
                        }
                        return ParseAliasQualifiedName(NameOptions.InExpression);
                    }
                    return AddError(CreateMissingIdentifierName(), ErrorCode.ERR_InvalidExprTerm, base.CurrentToken.Text);
                case SyntaxKind.OpenBracketToken:
                    if (IsPossibleLambdaExpression(precedence))
                    {
                        return ParseLambdaExpression();
                    }
                    break;
                case SyntaxKind.ThisKeyword:
                    return _syntaxFactory.ThisExpression(EatToken());
                case SyntaxKind.BaseKeyword:
                    return ParseBaseExpression();
                case SyntaxKind.NullKeyword:
                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                case SyntaxKind.ArgListKeyword:
                case SyntaxKind.NumericLiteralToken:
                case SyntaxKind.CharacterLiteralToken:
                case SyntaxKind.StringLiteralToken:
                    return _syntaxFactory.LiteralExpression(SyntaxFacts.GetLiteralExpression(kind), EatToken());
                case SyntaxKind.InterpolatedStringStartToken:
                    throw new NotImplementedException();
                case SyntaxKind.InterpolatedStringToken:
                    return ParseInterpolatedStringToken();
                case SyntaxKind.OpenParenToken:
                    return ParseCastOrParenExpressionOrLambdaOrTuple(precedence);
                case SyntaxKind.NewKeyword:
                    return ParseNewExpression();
                case SyntaxKind.StackAllocKeyword:
                    return ParseStackAllocExpression();
                case SyntaxKind.DelegateKeyword:
                    return ParseAnonymousMethodExpression();
                case SyntaxKind.RefKeyword:
                    return AddError(ParsePossibleRefExpression(), ErrorCode.ERR_InvalidExprTerm, SyntaxFacts.GetText(kind));
            }
            if (IsPredefinedType(kind))
            {
                PredefinedTypeSyntax predefinedTypeSyntax = _syntaxFactory.PredefinedType(EatToken());
                if (base.CurrentToken.Kind != SyntaxKind.DotToken || kind == SyntaxKind.VoidKeyword)
                {
                    predefinedTypeSyntax = AddError(predefinedTypeSyntax, ErrorCode.ERR_InvalidExprTerm, SyntaxFacts.GetText(kind));
                }
                return predefinedTypeSyntax;
            }
            IdentifierNameSyntax node = CreateMissingIdentifierName();
            if (kind == SyntaxKind.EndOfFileToken)
            {
                return AddError(node, ErrorCode.ERR_ExpressionExpected);
            }
            return AddError(node, ErrorCode.ERR_InvalidExprTerm, SyntaxFacts.GetText(kind));
        }

        private ExpressionSyntax ParseBaseExpression()
        {
            return _syntaxFactory.BaseExpression(EatToken());
        }

        private bool IsPossibleDeconstructionLeft(Precedence precedence)
        {
            if (precedence != 0 || (!base.CurrentToken.IsIdentifierVar() && !IsPredefinedType(base.CurrentToken.Kind)))
            {
                return false;
            }
            ResetPoint state = GetResetPoint();
            try
            {
                EatToken();
                return base.CurrentToken.Kind == SyntaxKind.OpenParenToken && ScanDesignator() && base.CurrentToken.Kind == SyntaxKind.EqualsToken;
            }
            finally
            {
                Reset(ref state);
                Release(ref state);
            }
        }

        private bool ScanDesignator()
        {
            SyntaxKind kind = base.CurrentToken.Kind;
            if (kind != SyntaxKind.OpenParenToken)
            {
                if (kind == SyntaxKind.IdentifierToken && IsTrueIdentifier())
                {
                    EatToken();
                    return true;
                }
                return false;
            }
            while (true)
            {
                EatToken();
                if (!ScanDesignator())
                {
                    break;
                }
                switch (base.CurrentToken.Kind)
                {
                    case SyntaxKind.CommaToken:
                        break;
                    case SyntaxKind.CloseParenToken:
                        EatToken();
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }

        private bool IsPossibleAnonymousMethodExpression()
        {
            int i;
            for (i = 0; PeekToken(i).Kind == SyntaxKind.StaticKeyword || PeekToken(i).ContextualKind == SyntaxKind.AsyncKeyword; i++)
            {
            }
            return PeekToken(i).Kind == SyntaxKind.DelegateKeyword;
        }

        private ExpressionSyntax ParsePostFixExpression(ExpressionSyntax expr)
        {
            while (true)
            {
                SyntaxKind kind = base.CurrentToken.Kind;
                switch (kind)
                {
                    case SyntaxKind.OpenParenToken:
                        expr = _syntaxFactory.InvocationExpression(expr, ParseParenthesizedArgumentList());
                        continue;
                    case SyntaxKind.OpenBracketToken:
                        expr = _syntaxFactory.ElementAccessExpression(expr, ParseBracketedArgumentList());
                        continue;
                    case SyntaxKind.MinusMinusToken:
                    case SyntaxKind.PlusPlusToken:
                        expr = _syntaxFactory.PostfixUnaryExpression(SyntaxFacts.GetPostfixUnaryExpression(kind), expr, EatToken());
                        continue;
                    case SyntaxKind.ColonColonToken:
                        if (PeekToken(1).Kind == SyntaxKind.IdentifierToken)
                        {
                            SyntaxToken node = EatToken();
                            node = AddError(node, ErrorCode.ERR_UnexpectedAliasedName);
                            SyntaxToken operatorToken2 = ConvertToMissingWithTrailingTrivia(node, SyntaxKind.DotToken);
                            expr = _syntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expr, operatorToken2, ParseSimpleName(NameOptions.InExpression));
                        }
                        else
                        {
                            expr = AddTrailingSkippedSyntax(expr, EatTokenWithPrejudice(SyntaxKind.DotToken));
                        }
                        continue;
                    case SyntaxKind.MinusGreaterThanToken:
                        expr = _syntaxFactory.MemberAccessExpression(SyntaxKind.PointerMemberAccessExpression, expr, EatToken(), ParseSimpleName(NameOptions.InExpression));
                        continue;
                    case SyntaxKind.DotToken:
                        if (base.CurrentToken.TrailingTrivia.Any(8539) && PeekToken(1).Kind == SyntaxKind.IdentifierToken && PeekToken(2).ContextualKind == SyntaxKind.IdentifierToken)
                        {
                            expr = _syntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expr, EatToken(), AddError(CreateMissingIdentifierName(), ErrorCode.ERR_IdentifierExpected));
                            return expr;
                        }
                        expr = _syntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expr, EatToken(), ParseSimpleName(NameOptions.InExpression));
                        continue;
                    case SyntaxKind.QuestionToken:
                        if (CanStartConsequenceExpression(PeekToken(1).Kind))
                        {
                            SyntaxToken operatorToken = EatToken();
                            ExpressionSyntax whenNotNull = ParseConsequenceSyntax();
                            expr = _syntaxFactory.ConditionalAccessExpression(expr, operatorToken, whenNotNull);
                            expr = CheckFeatureAvailability(expr, MessageID.IDS_FeatureNullPropagatingOperator);
                            continue;
                        }
                        break;
                    case SyntaxKind.ExclamationToken:
                        expr = _syntaxFactory.PostfixUnaryExpression(SyntaxFacts.GetPostfixUnaryExpression(kind), expr, EatToken());
                        expr = CheckFeatureAvailability(expr, MessageID.IDS_FeatureNullableReferenceTypes);
                        continue;
                }
                break;
            }
            return expr;
        }

        private static bool CanStartConsequenceExpression(SyntaxKind kind)
        {
            if (kind != SyntaxKind.DotToken)
            {
                return kind == SyntaxKind.OpenBracketToken;
            }
            return true;
        }

        internal ExpressionSyntax ParseConsequenceSyntax()
        {
            SyntaxKind kind = base.CurrentToken.Kind;
            ExpressionSyntax expressionSyntax = null;
            switch (kind)
            {
                case SyntaxKind.DotToken:
                    expressionSyntax = _syntaxFactory.MemberBindingExpression(EatToken(), ParseSimpleName(NameOptions.InExpression));
                    break;
                case SyntaxKind.OpenBracketToken:
                    expressionSyntax = _syntaxFactory.ElementBindingExpression(ParseBracketedArgumentList());
                    break;
            }
            while (true)
            {
                kind = base.CurrentToken.Kind;
                if (isOptionalExclamationsFollowedByConditionalOperation())
                {
                    while (kind == SyntaxKind.ExclamationToken)
                    {
                        expressionSyntax = _syntaxFactory.PostfixUnaryExpression(SyntaxKind.SuppressNullableWarningExpression, expressionSyntax, EatToken());
                        expressionSyntax = CheckFeatureAvailability(expressionSyntax, MessageID.IDS_FeatureNullableReferenceTypes);
                        kind = base.CurrentToken.Kind;
                    }
                }
                switch (kind)
                {
                    case SyntaxKind.OpenParenToken:
                        expressionSyntax = _syntaxFactory.InvocationExpression(expressionSyntax, ParseParenthesizedArgumentList());
                        break;
                    case SyntaxKind.OpenBracketToken:
                        expressionSyntax = _syntaxFactory.ElementAccessExpression(expressionSyntax, ParseBracketedArgumentList());
                        break;
                    case SyntaxKind.DotToken:
                        expressionSyntax = _syntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expressionSyntax, EatToken(), ParseSimpleName(NameOptions.InExpression));
                        break;
                    case SyntaxKind.QuestionToken:
                        if (CanStartConsequenceExpression(PeekToken(1).Kind))
                        {
                            SyntaxToken operatorToken = EatToken();
                            ExpressionSyntax whenNotNull = ParseConsequenceSyntax();
                            expressionSyntax = _syntaxFactory.ConditionalAccessExpression(expressionSyntax, operatorToken, whenNotNull);
                        }
                        return expressionSyntax;
                    default:
                        return expressionSyntax;
                }
            }
            bool isOptionalExclamationsFollowedByConditionalOperation()
            {
                SyntaxKind kind2 = base.CurrentToken.Kind;
                int num = 1;
                while (true)
                {
                    switch (kind2)
                    {
                        case SyntaxKind.ExclamationToken:
                            break;
                        default:
                            return kind2 == SyntaxKind.QuestionToken;
                        case SyntaxKind.OpenParenToken:
                        case SyntaxKind.OpenBracketToken:
                        case SyntaxKind.DotToken:
                            return true;
                    }
                    kind2 = PeekToken(num).Kind;
                    num++;
                }
            }
        }

        internal ArgumentListSyntax ParseParenthesizedArgumentList()
        {
            if (IsIncrementalAndFactoryContextMatches && base.CurrentNodeKind == SyntaxKind.ArgumentList)
            {
                return (ArgumentListSyntax)EatNode();
            }
            ParseArgumentList(out var openToken, out var arguments, out var closeToken, SyntaxKind.OpenParenToken, SyntaxKind.CloseParenToken);
            return _syntaxFactory.ArgumentList(openToken, arguments, closeToken);
        }

        internal BracketedArgumentListSyntax ParseBracketedArgumentList()
        {
            if (IsIncrementalAndFactoryContextMatches && base.CurrentNodeKind == SyntaxKind.BracketedArgumentList)
            {
                return (BracketedArgumentListSyntax)EatNode();
            }
            ParseArgumentList(out var openToken, out var arguments, out var closeToken, SyntaxKind.OpenBracketToken, SyntaxKind.CloseBracketToken);
            return _syntaxFactory.BracketedArgumentList(openToken, arguments, closeToken);
        }

        private void ParseArgumentList(out SyntaxToken openToken, out Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax> arguments, out SyntaxToken closeToken, SyntaxKind openKind, SyntaxKind closeKind)
        {
            bool flag = openKind == SyntaxKind.OpenBracketToken;
            if (base.CurrentToken.Kind == SyntaxKind.OpenParenToken || base.CurrentToken.Kind == SyntaxKind.OpenBracketToken)
            {
                openToken = EatTokenAsKind(openKind);
            }
            else
            {
                openToken = EatToken(openKind);
            }
            TerminatorState termState = _termState;
            _termState |= TerminatorState.IsEndOfArgumentList;
            SeparatedSyntaxListBuilder<ArgumentSyntax> item = default(SeparatedSyntaxListBuilder<ArgumentSyntax>);
            try
            {
                if (base.CurrentToken.Kind != closeKind && base.CurrentToken.Kind != SyntaxKind.SemicolonToken)
                {
                    do
                    {
                        if (item.IsNull)
                        {
                            item = _pool.AllocateSeparated<ArgumentSyntax>();
                        }
                        if (!IsPossibleArgumentExpression() && base.CurrentToken.Kind != SyntaxKind.CommaToken)
                        {
                            continue;
                        }
                        item.Add(ParseArgumentExpression(flag));
                        int lastTokenPosition = -1;
                        while (IsMakingProgress(ref lastTokenPosition) && base.CurrentToken.Kind != SyntaxKind.CloseParenToken && base.CurrentToken.Kind != SyntaxKind.CloseBracketToken && base.CurrentToken.Kind != SyntaxKind.SemicolonToken)
                        {
                            if (base.CurrentToken.Kind == SyntaxKind.CommaToken || IsPossibleArgumentExpression())
                            {
                                item.AddSeparator(EatToken(SyntaxKind.CommaToken));
                                item.Add(ParseArgumentExpression(flag));
                            }
                            else if (SkipBadArgumentListTokens(ref openToken, item, SyntaxKind.CommaToken, closeKind) == PostSkipAction.Abort)
                            {
                                break;
                            }
                        }
                        break;
                    }
                    while (SkipBadArgumentListTokens(ref openToken, item, SyntaxKind.IdentifierToken, closeKind) == PostSkipAction.Continue);
                }
                else if (flag && base.CurrentToken.Kind == closeKind)
                {
                    if (item.IsNull)
                    {
                        item = _pool.AllocateSeparated<ArgumentSyntax>();
                    }
                    item.Add(ParseArgumentExpression(flag));
                }
                _termState = termState;
                if (base.CurrentToken.Kind == SyntaxKind.CloseParenToken || base.CurrentToken.Kind == SyntaxKind.CloseBracketToken)
                {
                    closeToken = EatTokenAsKind(closeKind);
                }
                else
                {
                    closeToken = EatToken(closeKind);
                }
                arguments = item.ToList();
            }
            finally
            {
                if (!item.IsNull)
                {
                    _pool.Free(in item);
                }
            }
        }

        private PostSkipAction SkipBadArgumentListTokens(ref SyntaxToken open, SeparatedSyntaxListBuilder<ArgumentSyntax> list, SyntaxKind expected, SyntaxKind closeKind)
        {
            return SkipBadSeparatedListTokensWithExpectedKind(ref open, list, (LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleArgumentExpression(), (LanguageParser p) => p.CurrentToken.Kind == closeKind || p.CurrentToken.Kind == SyntaxKind.SemicolonToken || p.IsTerminator(), expected);
        }

        private bool IsEndOfArgumentList()
        {
            if (base.CurrentToken.Kind != SyntaxKind.CloseParenToken)
            {
                return base.CurrentToken.Kind == SyntaxKind.CloseBracketToken;
            }
            return true;
        }

        private bool IsPossibleArgumentExpression()
        {
            if (!IsValidArgumentRefKindKeyword(base.CurrentToken.Kind))
            {
                return IsPossibleExpression();
            }
            return true;
        }

        private static bool IsValidArgumentRefKindKeyword(SyntaxKind kind)
        {
            if (kind - 8360 <= (SyntaxKind)2)
            {
                return true;
            }
            return false;
        }

        private ArgumentSyntax ParseArgumentExpression(bool isIndexer)
        {
            NameColonSyntax nameColon = null;
            if (base.CurrentToken.Kind == SyntaxKind.IdentifierToken && PeekToken(1).Kind == SyntaxKind.ColonToken)
            {
                IdentifierNameSyntax name = ParseIdentifierName();
                SyntaxToken colonToken = EatToken(SyntaxKind.ColonToken);
                nameColon = _syntaxFactory.NameColon(name, colonToken);
                nameColon = CheckFeatureAvailability(nameColon, MessageID.IDS_FeatureNamedArgument);
            }
            SyntaxToken syntaxToken = null;
            if (IsValidArgumentRefKindKeyword(base.CurrentToken.Kind))
            {
                syntaxToken = EatToken();
            }
            ExpressionSyntax expression;
            if (isIndexer && (base.CurrentToken.Kind == SyntaxKind.CommaToken || base.CurrentToken.Kind == SyntaxKind.CloseBracketToken))
            {
                expression = ParseIdentifierName(ErrorCode.ERR_ValueExpected);
            }
            else if (base.CurrentToken.Kind == SyntaxKind.CommaToken)
            {
                expression = ParseIdentifierName(ErrorCode.ERR_MissingArgument);
            }
            else
            {
                if (syntaxToken != null && syntaxToken.Kind == SyntaxKind.InKeyword)
                {
                    syntaxToken = CheckFeatureAvailability(syntaxToken, MessageID.IDS_FeatureReadOnlyReferences);
                }
                expression = ((syntaxToken != null && syntaxToken.Kind == SyntaxKind.OutKeyword) ? ParseExpressionOrDeclaration(ParseTypeMode.Normal, MessageID.IDS_FeatureOutVar, permitTupleDesignation: false) : ParseSubExpression(Precedence.Expression));
            }
            return _syntaxFactory.Argument(nameColon, syntaxToken, expression);
        }

        private TypeOfExpressionSyntax ParseTypeOfExpression()
        {
            SyntaxToken keyword = EatToken();
            SyntaxToken openParenToken = EatToken(SyntaxKind.OpenParenToken);
            TypeSyntax type = ParseTypeOrVoid();
            SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
            return _syntaxFactory.TypeOfExpression(keyword, openParenToken, type, closeParenToken);
        }

        private ExpressionSyntax ParseDefaultExpression()
        {
            SyntaxToken node = EatToken();
            if (base.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                SyntaxToken openParenToken = EatToken(SyntaxKind.OpenParenToken);
                TypeSyntax type = ParseType();
                SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
                node = CheckFeatureAvailability(node, MessageID.IDS_FeatureDefault);
                return _syntaxFactory.DefaultExpression(node, openParenToken, type, closeParenToken);
            }
            node = CheckFeatureAvailability(node, MessageID.IDS_FeatureDefaultLiteral);
            return _syntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression, node);
        }

        private SizeOfExpressionSyntax ParseSizeOfExpression()
        {
            SyntaxToken keyword = EatToken();
            SyntaxToken openParenToken = EatToken(SyntaxKind.OpenParenToken);
            TypeSyntax type = ParseType();
            SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
            return _syntaxFactory.SizeOfExpression(keyword, openParenToken, type, closeParenToken);
        }

        private MakeRefExpressionSyntax ParseMakeRefExpression()
        {
            SyntaxToken keyword = EatToken();
            SyntaxToken openParenToken = EatToken(SyntaxKind.OpenParenToken);
            ExpressionSyntax expression = ParseSubExpression(Precedence.Expression);
            SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
            return _syntaxFactory.MakeRefExpression(keyword, openParenToken, expression, closeParenToken);
        }

        private RefTypeExpressionSyntax ParseRefTypeExpression()
        {
            SyntaxToken keyword = EatToken();
            SyntaxToken openParenToken = EatToken(SyntaxKind.OpenParenToken);
            ExpressionSyntax expression = ParseSubExpression(Precedence.Expression);
            SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
            return _syntaxFactory.RefTypeExpression(keyword, openParenToken, expression, closeParenToken);
        }

        private CheckedExpressionSyntax ParseCheckedOrUncheckedExpression()
        {
            SyntaxToken syntaxToken = EatToken();
            SyntaxKind kind = ((syntaxToken.Kind == SyntaxKind.CheckedKeyword) ? SyntaxKind.CheckedExpression : SyntaxKind.UncheckedExpression);
            SyntaxToken openParenToken = EatToken(SyntaxKind.OpenParenToken);
            ExpressionSyntax expression = ParseSubExpression(Precedence.Expression);
            SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
            return _syntaxFactory.CheckedExpression(kind, syntaxToken, openParenToken, expression, closeParenToken);
        }

        private RefValueExpressionSyntax ParseRefValueExpression()
        {
            SyntaxToken keyword = EatToken(SyntaxKind.RefValueKeyword);
            SyntaxToken openParenToken = EatToken(SyntaxKind.OpenParenToken);
            ExpressionSyntax expression = ParseSubExpression(Precedence.Expression);
            SyntaxToken comma = EatToken(SyntaxKind.CommaToken);
            TypeSyntax type = ParseType();
            SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
            return _syntaxFactory.RefValueExpression(keyword, openParenToken, expression, comma, type, closeParenToken);
        }

        private bool ScanParenthesizedLambda(Precedence precedence)
        {
            if (!ScanParenthesizedImplicitlyTypedLambda(precedence))
            {
                return ScanExplicitlyTypedLambda(precedence);
            }
            return true;
        }

        private bool ScanParenthesizedImplicitlyTypedLambda(Precedence precedence)
        {
            if (precedence != 0)
            {
                return false;
            }
            if (PeekToken(1).Kind == SyntaxKind.IdentifierToken && (!IsInQuery || !IsTokenQueryContextualKeyword(PeekToken(1))) && PeekToken(2).Kind == SyntaxKind.CommaToken)
            {
                int num = 3;
                SyntaxToken syntaxToken;
                do
                {
                    syntaxToken = PeekToken(num++);
                }
                while (syntaxToken.Kind == SyntaxKind.IdentifierToken || SyntaxFacts.IsPredefinedType(syntaxToken.Kind) || syntaxToken.Kind == SyntaxKind.CommaToken || (!IsInQuery && IsTokenQueryContextualKeyword(syntaxToken)));
                if (PeekToken(num - 1).Kind == SyntaxKind.CloseParenToken)
                {
                    return PeekToken(num).Kind == SyntaxKind.EqualsGreaterThanToken;
                }
                return false;
            }
            if (IsTrueIdentifier(PeekToken(1)) && PeekToken(2).Kind == SyntaxKind.CloseParenToken && PeekToken(3).Kind == SyntaxKind.EqualsGreaterThanToken)
            {
                return true;
            }
            if (PeekToken(1).Kind == SyntaxKind.CloseParenToken && PeekToken(2).Kind == SyntaxKind.EqualsGreaterThanToken)
            {
                return true;
            }
            if (PeekToken(1).Kind == SyntaxKind.ParamsKeyword)
            {
                return true;
            }
            return false;
        }

        private bool ScanExplicitlyTypedLambda(Precedence precedence)
        {
            if (precedence != 0)
            {
                return false;
            }
            ResetPoint state = GetResetPoint();
            try
            {
                bool flag = false;
                do
                {
                    EatToken();
                    ParseAttributeDeclarations();
                    switch (base.CurrentToken.Kind)
                    {
                        case SyntaxKind.RefKeyword:
                            EatToken();
                            flag = true;
                            if (base.CurrentToken.Kind == SyntaxKind.ReadOnlyKeyword)
                            {
                                EatToken();
                            }
                            break;
                        case SyntaxKind.OutKeyword:
                        case SyntaxKind.InKeyword:
                        case SyntaxKind.ParamsKeyword:
                            EatToken();
                            flag = true;
                            break;
                    }
                    if (base.CurrentToken.Kind == SyntaxKind.EndOfFileToken)
                    {
                        return flag;
                    }
                    if (ScanType() == ScanTypeFlags.NotType)
                    {
                        return false;
                    }
                    if (IsTrueIdentifier())
                    {
                        EatToken();
                    }
                    switch (base.CurrentToken.Kind)
                    {
                        case SyntaxKind.EndOfFileToken:
                            return flag;
                        case SyntaxKind.CommaToken:
                            break;
                        case SyntaxKind.CloseParenToken:
                            return PeekToken(1).Kind == SyntaxKind.EqualsGreaterThanToken;
                        default:
                            return false;
                    }
                }
                while (!flag);
                return true;
            }
            finally
            {
                Reset(ref state);
                Release(ref state);
            }
        }

        private ExpressionSyntax ParseCastOrParenExpressionOrLambdaOrTuple(Precedence precedence)
        {
            ResetPoint state = GetResetPoint();
            try
            {
                if (ScanParenthesizedImplicitlyTypedLambda(precedence))
                {
                    return ParseLambdaExpression();
                }
                if (ScanCast() && !IsCurrentTokenQueryKeywordInQuery())
                {
                    Reset(ref state);
                    SyntaxToken openParenToken = EatToken(SyntaxKind.OpenParenToken);
                    TypeSyntax type = ParseType();
                    SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
                    ExpressionSyntax expression = ParseSubExpression(Precedence.Cast);
                    return _syntaxFactory.CastExpression(openParenToken, type, closeParenToken, expression);
                }
                Reset(ref state);
                if (ScanExplicitlyTypedLambda(precedence))
                {
                    return ParseLambdaExpression();
                }
                Reset(ref state);
                SyntaxToken syntaxToken = EatToken(SyntaxKind.OpenParenToken);
                ExpressionSyntax expressionSyntax = ParseExpressionOrDeclaration(ParseTypeMode.FirstElementOfPossibleTupleLiteral, MessageID.None, permitTupleDesignation: true);
                if (base.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    ArgumentSyntax firstArg = _syntaxFactory.Argument(null, null, expressionSyntax);
                    return ParseTupleExpressionTail(syntaxToken, firstArg);
                }
                if (expressionSyntax.Kind == SyntaxKind.IdentifierName && base.CurrentToken.Kind == SyntaxKind.ColonToken)
                {
                    NameColonSyntax nameColon = _syntaxFactory.NameColon((IdentifierNameSyntax)expressionSyntax, EatToken());
                    expressionSyntax = ParseExpressionOrDeclaration(ParseTypeMode.FirstElementOfPossibleTupleLiteral, MessageID.None, permitTupleDesignation: true);
                    ArgumentSyntax firstArg2 = _syntaxFactory.Argument(nameColon, null, expressionSyntax);
                    return ParseTupleExpressionTail(syntaxToken, firstArg2);
                }
                SyntaxToken closeParenToken2 = EatToken(SyntaxKind.CloseParenToken);
                return _syntaxFactory.ParenthesizedExpression(syntaxToken, expressionSyntax, closeParenToken2);
            }
            finally
            {
                Release(ref state);
            }
        }

        private TupleExpressionSyntax ParseTupleExpressionTail(SyntaxToken openParen, ArgumentSyntax firstArg)
        {
            SeparatedSyntaxListBuilder<ArgumentSyntax> builder = _pool.AllocateSeparated<ArgumentSyntax>();
            try
            {
                builder.Add(firstArg);
                while (base.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    SyntaxToken separatorToken = EatToken(SyntaxKind.CommaToken);
                    builder.AddSeparator(separatorToken);
                    ExpressionSyntax expressionSyntax = ParseExpressionOrDeclaration(ParseTypeMode.AfterTupleComma, MessageID.None, permitTupleDesignation: true);
                    ArgumentSyntax node;
                    if (expressionSyntax.Kind == SyntaxKind.IdentifierName && base.CurrentToken.Kind == SyntaxKind.ColonToken)
                    {
                        NameColonSyntax nameColon = _syntaxFactory.NameColon((IdentifierNameSyntax)expressionSyntax, EatToken());
                        expressionSyntax = ParseExpressionOrDeclaration(ParseTypeMode.AfterTupleComma, MessageID.None, permitTupleDesignation: true);
                        node = _syntaxFactory.Argument(nameColon, null, expressionSyntax);
                    }
                    else
                    {
                        node = _syntaxFactory.Argument(null, null, expressionSyntax);
                    }
                    builder.Add(node);
                }
                if (builder.Count < 2)
                {
                    builder.AddSeparator(SyntaxFactory.MissingToken(SyntaxKind.CommaToken));
                    IdentifierNameSyntax expression = AddError(CreateMissingIdentifierName(), ErrorCode.ERR_TupleTooFewElements);
                    builder.Add(_syntaxFactory.Argument(null, null, expression));
                }
                SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
                TupleExpressionSyntax node2 = _syntaxFactory.TupleExpression(openParen, builder, closeParenToken);
                return CheckFeatureAvailability(node2, MessageID.IDS_FeatureTuples);
            }
            finally
            {
                _pool.Free(in builder);
            }
        }

        private bool ScanCast(bool forPattern = false)
        {
            if (base.CurrentToken.Kind != SyntaxKind.OpenParenToken)
            {
                return false;
            }
            EatToken();
            ScanTypeFlags scanTypeFlags = ScanType(forPattern);
            if (scanTypeFlags == ScanTypeFlags.NotType)
            {
                return false;
            }
            if (base.CurrentToken.Kind != SyntaxKind.CloseParenToken)
            {
                return false;
            }
            EatToken();
            if (forPattern && base.CurrentToken.Kind == SyntaxKind.IdentifierToken)
            {
                return !isBinaryPattern();
            }
            switch (scanTypeFlags)
            {
                case ScanTypeFlags.MustBeType:
                case ScanTypeFlags.AliasQualifiedName:
                case ScanTypeFlags.NullableType:
                case ScanTypeFlags.PointerOrMultiplication:
                    {
                        bool flag = !forPattern;
                        if (!flag)
                        {
                            SyntaxKind kind = base.CurrentToken.Kind;
                            bool flag2 = kind - 8198 <= SyntaxKind.List || kind - 8202 <= SyntaxKind.List || kind == SyntaxKind.DotDotToken || CanFollowCast(kind);
                            flag = flag2;
                        }
                        return flag;
                    }
                case ScanTypeFlags.GenericTypeOrMethod:
                case ScanTypeFlags.GenericTypeOrExpression:
                case ScanTypeFlags.NonGenericTypeOrExpression:
                case ScanTypeFlags.TupleType:
                    return CanFollowCast(base.CurrentToken.Kind);
                default:
                    throw ExceptionUtilities.UnexpectedValue(scanTypeFlags);
            }
            bool isBinaryPattern()
            {
                if (!isBinaryPatternKeyword())
                {
                    return false;
                }
                bool flag3 = true;
                EatToken();
                while (isBinaryPatternKeyword())
                {
                    flag3 = !flag3;
                    EatToken();
                }
                return flag3 == IsPossibleSubpatternElement();
            }
            bool isBinaryPatternKeyword()
            {
                SyntaxKind contextualKind = base.CurrentToken.ContextualKind;
                return contextualKind == SyntaxKind.OrKeyword || contextualKind == SyntaxKind.AndKeyword;
            }
        }

        private bool IsPossibleLambdaExpression(Precedence precedence)
        {
            if (precedence != 0)
            {
                return false;
            }
            if (base.CurrentToken.Kind == SyntaxKind.OpenBracketToken)
            {
                ResetPoint state = GetResetPoint();
                try
                {
                    ParseAttributeDeclarations();
                    switch (base.CurrentToken.Kind)
                    {
                        case SyntaxKind.StaticKeyword:
                        case SyntaxKind.IdentifierToken:
                            return IsPossibleLambdaExpressionCore(precedence);
                        case SyntaxKind.OpenParenToken:
                            return ScanParenthesizedLambda(precedence);
                        default:
                            return false;
                    }
                }
                finally
                {
                    Reset(ref state);
                    Release(ref state);
                }
            }
            return IsPossibleLambdaExpressionCore(precedence);
        }

        private bool IsPossibleLambdaExpressionCore(Precedence precedence)
        {
            int num = ((base.CurrentToken.Kind == SyntaxKind.StaticKeyword) ? 1 : ((base.CurrentToken.ContextualKind == SyntaxKind.AsyncKeyword && PeekToken(1).Kind == SyntaxKind.StaticKeyword) ? 2 : 0));
            if (PeekToken(num).Kind == SyntaxKind.EqualsGreaterThanToken)
            {
                return true;
            }
            if (PeekToken(num).Kind == SyntaxKind.IdentifierToken && PeekToken(num + 1).Kind == SyntaxKind.EqualsGreaterThanToken)
            {
                return true;
            }
            if (PeekToken(num).Kind == SyntaxKind.OpenParenToken)
            {
                return true;
            }
            if (PeekToken(num).ContextualKind != SyntaxKind.AsyncKeyword)
            {
                return false;
            }
            num++;
            if (PeekToken(num).Kind == SyntaxKind.IdentifierToken && PeekToken(num + 1).Kind == SyntaxKind.EqualsGreaterThanToken)
            {
                return true;
            }
            if (PeekToken(num).Kind != SyntaxKind.OpenParenToken)
            {
                return false;
            }
            ResetPoint state = GetResetPoint();
            for (int i = 0; i < num; i++)
            {
                EatToken();
            }
            bool result = ScanParenthesizedLambda(precedence);
            Reset(ref state);
            Release(ref state);
            return result;
        }

        private static bool CanFollowCast(SyntaxKind kind)
        {
            switch (kind)
            {
                case SyntaxKind.PercentToken:
                case SyntaxKind.CaretToken:
                case SyntaxKind.AmpersandToken:
                case SyntaxKind.AsteriskToken:
                case SyntaxKind.CloseParenToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.PlusToken:
                case SyntaxKind.EqualsToken:
                case SyntaxKind.OpenBraceToken:
                case SyntaxKind.CloseBraceToken:
                case SyntaxKind.OpenBracketToken:
                case SyntaxKind.CloseBracketToken:
                case SyntaxKind.BarToken:
                case SyntaxKind.ColonToken:
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.LessThanToken:
                case SyntaxKind.CommaToken:
                case SyntaxKind.GreaterThanToken:
                case SyntaxKind.DotToken:
                case SyntaxKind.QuestionToken:
                case SyntaxKind.SlashToken:
                case SyntaxKind.DotDotToken:
                case SyntaxKind.BarBarToken:
                case SyntaxKind.AmpersandAmpersandToken:
                case SyntaxKind.MinusMinusToken:
                case SyntaxKind.PlusPlusToken:
                case SyntaxKind.QuestionQuestionToken:
                case SyntaxKind.MinusGreaterThanToken:
                case SyntaxKind.ExclamationEqualsToken:
                case SyntaxKind.EqualsEqualsToken:
                case SyntaxKind.EqualsGreaterThanToken:
                case SyntaxKind.LessThanEqualsToken:
                case SyntaxKind.LessThanLessThanToken:
                case SyntaxKind.LessThanLessThanEqualsToken:
                case SyntaxKind.GreaterThanEqualsToken:
                case SyntaxKind.GreaterThanGreaterThanToken:
                case SyntaxKind.GreaterThanGreaterThanEqualsToken:
                case SyntaxKind.SlashEqualsToken:
                case SyntaxKind.AsteriskEqualsToken:
                case SyntaxKind.BarEqualsToken:
                case SyntaxKind.AmpersandEqualsToken:
                case SyntaxKind.PlusEqualsToken:
                case SyntaxKind.MinusEqualsToken:
                case SyntaxKind.CaretEqualsToken:
                case SyntaxKind.PercentEqualsToken:
                case SyntaxKind.QuestionQuestionEqualsToken:
                case SyntaxKind.SwitchKeyword:
                case SyntaxKind.IsKeyword:
                case SyntaxKind.AsKeyword:
                case SyntaxKind.EndOfFileToken:
                    return false;
                default:
                    return true;
            }
        }

        private ExpressionSyntax ParseNewExpression()
        {
            if (IsAnonymousType())
            {
                return ParseAnonymousTypeExpression();
            }
            if (IsImplicitlyTypedArray())
            {
                return ParseImplicitlyTypedArrayCreation();
            }
            return ParseArrayOrObjectCreationExpression();
        }

        private bool IsAnonymousType()
        {
            if (base.CurrentToken.Kind == SyntaxKind.NewKeyword)
            {
                return PeekToken(1).Kind == SyntaxKind.OpenBraceToken;
            }
            return false;
        }

        private AnonymousObjectCreationExpressionSyntax ParseAnonymousTypeExpression()
        {
            SyntaxToken node = EatToken(SyntaxKind.NewKeyword);
            node = CheckFeatureAvailability(node, MessageID.IDS_FeatureAnonymousTypes);
            SyntaxToken openBrace = EatToken(SyntaxKind.OpenBraceToken);
            SeparatedSyntaxListBuilder<AnonymousObjectMemberDeclaratorSyntax> list = _pool.AllocateSeparated<AnonymousObjectMemberDeclaratorSyntax>();
            ParseAnonymousTypeMemberInitializers(ref openBrace, ref list);
            SyntaxToken closeBraceToken = EatToken(SyntaxKind.CloseBraceToken);
            AnonymousObjectCreationExpressionSyntax result = _syntaxFactory.AnonymousObjectCreationExpression(node, openBrace, list, closeBraceToken);
            _pool.Free(in list);
            return result;
        }

        private void ParseAnonymousTypeMemberInitializers(ref SyntaxToken openBrace, ref SeparatedSyntaxListBuilder<AnonymousObjectMemberDeclaratorSyntax> list)
        {
            if (base.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
            {
                return;
            }
            while (true)
            {
                if (IsPossibleExpression() || base.CurrentToken.Kind == SyntaxKind.CommaToken)
                {
                    list.Add(ParseAnonymousTypeMemberInitializer());
                    int lastTokenPosition = -1;
                    while (true)
                    {
                        if (!IsMakingProgress(ref lastTokenPosition) || base.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                        {
                            return;
                        }
                        if (base.CurrentToken.Kind == SyntaxKind.CommaToken || IsPossibleExpression())
                        {
                            list.AddSeparator(EatToken(SyntaxKind.CommaToken));
                            if (base.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                            {
                                return;
                            }
                            if (!IsPossibleExpression())
                            {
                                break;
                            }
                            list.Add(ParseAnonymousTypeMemberInitializer());
                        }
                        else if (SkipBadInitializerListTokens(ref openBrace, list, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                        {
                            return;
                        }
                    }
                }
                else if (SkipBadInitializerListTokens(ref openBrace, list, SyntaxKind.IdentifierToken) != 0)
                {
                    break;
                }
            }
        }

        private AnonymousObjectMemberDeclaratorSyntax ParseAnonymousTypeMemberInitializer()
        {
            NameEqualsSyntax nameEquals = (IsNamedAssignment() ? ParseNameEquals() : null);
            ExpressionSyntax expression = ParseExpressionCore();
            return _syntaxFactory.AnonymousObjectMemberDeclarator(nameEquals, expression);
        }

        private bool IsInitializerMember()
        {
            if (!IsComplexElementInitializer() && !IsNamedAssignment() && !IsDictionaryInitializer())
            {
                return IsPossibleExpression();
            }
            return true;
        }

        private bool IsComplexElementInitializer()
        {
            return base.CurrentToken.Kind == SyntaxKind.OpenBraceToken;
        }

        private bool IsNamedAssignment()
        {
            if (IsTrueIdentifier())
            {
                return PeekToken(1).Kind == SyntaxKind.EqualsToken;
            }
            return false;
        }

        private bool IsDictionaryInitializer()
        {
            return base.CurrentToken.Kind == SyntaxKind.OpenBracketToken;
        }

        private ExpressionSyntax ParseArrayOrObjectCreationExpression()
        {
            SyntaxToken syntaxToken = EatToken(SyntaxKind.NewKeyword);
            TypeSyntax typeSyntax = null;
            InitializerExpressionSyntax initializerExpressionSyntax = null;
            if (IsImplicitObjectCreation())
            {
                syntaxToken = CheckFeatureAvailability(syntaxToken, MessageID.IDS_FeatureImplicitObjectCreation);
            }
            else
            {
                typeSyntax = ParseType(ParseTypeMode.NewExpression);
                if (typeSyntax.Kind == SyntaxKind.ArrayType)
                {
                    if (base.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
                    {
                        initializerExpressionSyntax = ParseArrayInitializer();
                    }
                    return _syntaxFactory.ArrayCreationExpression(syntaxToken, (ArrayTypeSyntax)typeSyntax, initializerExpressionSyntax);
                }
            }
            ArgumentListSyntax argumentListSyntax = null;
            if (base.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                argumentListSyntax = ParseParenthesizedArgumentList();
            }
            if (base.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
            {
                initializerExpressionSyntax = ParseObjectOrCollectionInitializer();
            }
            if (argumentListSyntax == null && initializerExpressionSyntax == null)
            {
                argumentListSyntax = _syntaxFactory.ArgumentList(EatToken(SyntaxKind.OpenParenToken, ErrorCode.ERR_BadNewExpr, typeSyntax != null && !typeSyntax.ContainsDiagnostics), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<ArgumentSyntax>), SyntaxFactory.MissingToken(SyntaxKind.CloseParenToken));
            }
            if (typeSyntax != null)
            {
                return _syntaxFactory.ObjectCreationExpression(syntaxToken, typeSyntax, argumentListSyntax, initializerExpressionSyntax);
            }
            return _syntaxFactory.ImplicitObjectCreationExpression(syntaxToken, argumentListSyntax, initializerExpressionSyntax);
        }

        private bool IsImplicitObjectCreation()
        {
            if (base.CurrentToken.Kind != SyntaxKind.OpenParenToken)
            {
                return false;
            }
            ResetPoint state = GetResetPoint();
            try
            {
                EatToken();
                if (ScanTupleType(out var _) != 0)
                {
                    SyntaxKind kind = base.CurrentToken.Kind;
                    if (kind == SyntaxKind.OpenParenToken || kind == SyntaxKind.OpenBracketToken || kind == SyntaxKind.QuestionToken)
                    {
                        return false;
                    }
                }
                return true;
            }
            finally
            {
                Reset(ref state);
                Release(ref state);
            }
        }

        private ExpressionSyntax ParseWithExpression(ExpressionSyntax receiverExpression, SyntaxToken withKeyword)
        {
            SyntaxToken startToken = EatToken(SyntaxKind.OpenBraceToken);
            SeparatedSyntaxListBuilder<ExpressionSyntax> item = _pool.AllocateSeparated<ExpressionSyntax>();
            if (base.CurrentToken.Kind != SyntaxKind.CloseBraceToken)
            {
                bool flag = true;
                while (!IsPossibleExpression() && base.CurrentToken.Kind != SyntaxKind.CommaToken)
                {
                    if (SkipBadInitializerListTokens(ref startToken, item, SyntaxKind.IdentifierToken) == PostSkipAction.Abort)
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    item.Add(ParseExpressionCore());
                    int lastTokenPosition = -1;
                    while (IsMakingProgress(ref lastTokenPosition) && base.CurrentToken.Kind != SyntaxKind.CloseBraceToken)
                    {
                        if (IsPossibleExpression() || base.CurrentToken.Kind == SyntaxKind.CommaToken)
                        {
                            item.AddSeparator(EatToken(SyntaxKind.CommaToken));
                            if (base.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                            {
                                break;
                            }
                            item.Add(ParseExpressionCore());
                        }
                        else if (SkipBadInitializerListTokens(ref startToken, item, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                        {
                            break;
                        }
                    }
                }
            }
            SyntaxToken closeBraceToken = EatToken(SyntaxKind.CloseBraceToken);
            InitializerExpressionSyntax initializer = _syntaxFactory.InitializerExpression(SyntaxKind.WithInitializerExpression, startToken, _pool.ToListAndFree(in item), closeBraceToken);
            withKeyword = CheckFeatureAvailability(withKeyword, MessageID.IDS_FeatureRecords);
            return _syntaxFactory.WithExpression(receiverExpression, withKeyword, initializer);
        }

        private InitializerExpressionSyntax ParseObjectOrCollectionInitializer()
        {
            SyntaxToken startToken = EatToken(SyntaxKind.OpenBraceToken);
            SeparatedSyntaxListBuilder<ExpressionSyntax> builder = _pool.AllocateSeparated<ExpressionSyntax>();
            try
            {
                ParseObjectOrCollectionInitializerMembers(ref startToken, builder, out var isObjectInitializer);
                startToken = CheckFeatureAvailability(startToken, isObjectInitializer ? MessageID.IDS_FeatureObjectInitializer : MessageID.IDS_FeatureCollectionInitializer);
                SyntaxToken closeBraceToken = EatToken(SyntaxKind.CloseBraceToken);
                return _syntaxFactory.InitializerExpression(isObjectInitializer ? SyntaxKind.ObjectInitializerExpression : SyntaxKind.CollectionInitializerExpression, startToken, builder, closeBraceToken);
            }
            finally
            {
                _pool.Free(in builder);
            }
        }

        private void ParseObjectOrCollectionInitializerMembers(ref SyntaxToken startToken, SeparatedSyntaxListBuilder<ExpressionSyntax> list, out bool isObjectInitializer)
        {
            isObjectInitializer = true;
            if (base.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
            {
                return;
            }
            do
            {
                if (!IsInitializerMember() && base.CurrentToken.Kind != SyntaxKind.CommaToken)
                {
                    continue;
                }
                isObjectInitializer = false;
                list.Add(ParseObjectOrCollectionInitializerMember(ref isObjectInitializer));
                int lastTokenPosition = -1;
                while (IsMakingProgress(ref lastTokenPosition) && base.CurrentToken.Kind != SyntaxKind.CloseBraceToken)
                {
                    if (base.CurrentToken.Kind == SyntaxKind.CommaToken || IsInitializerMember())
                    {
                        list.AddSeparator(EatToken(SyntaxKind.CommaToken));
                        if (base.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                        {
                            break;
                        }
                        list.Add(ParseObjectOrCollectionInitializerMember(ref isObjectInitializer));
                    }
                    else if (SkipBadInitializerListTokens(ref startToken, list, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                    {
                        break;
                    }
                }
                break;
            }
            while (SkipBadInitializerListTokens(ref startToken, list, SyntaxKind.IdentifierToken) == PostSkipAction.Continue);
        }

        private ExpressionSyntax ParseObjectOrCollectionInitializerMember(ref bool isObjectInitializer)
        {
            if (IsComplexElementInitializer())
            {
                return ParseComplexElementInitializer();
            }
            if (IsDictionaryInitializer())
            {
                isObjectInitializer = true;
                ExpressionSyntax node = ParseDictionaryInitializer();
                return CheckFeatureAvailability(node, MessageID.IDS_FeatureDictionaryInitializer);
            }
            if (IsNamedAssignment())
            {
                isObjectInitializer = true;
                return ParseObjectInitializerNamedAssignment();
            }
            return ParseExpressionCore();
        }

        private PostSkipAction SkipBadInitializerListTokens<T>(ref SyntaxToken startToken, SeparatedSyntaxListBuilder<T> list, SyntaxKind expected) where T : CSharpSyntaxNode
        {
            return SkipBadSeparatedListTokensWithExpectedKind(ref startToken, list, (LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleExpression(), (LanguageParser p) => p.CurrentToken.Kind == SyntaxKind.CloseBraceToken || p.IsTerminator(), expected);
        }

        private ExpressionSyntax ParseObjectInitializerNamedAssignment()
        {
            IdentifierNameSyntax left = ParseIdentifierName();
            SyntaxToken operatorToken = EatToken(SyntaxKind.EqualsToken);
            ExpressionSyntax right = ((base.CurrentToken.Kind != SyntaxKind.OpenBraceToken) ? ParseExpressionCore() : ParseObjectOrCollectionInitializer());
            return _syntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, left, operatorToken, right);
        }

        private ExpressionSyntax ParseDictionaryInitializer()
        {
            BracketedArgumentListSyntax argumentList = ParseBracketedArgumentList();
            SyntaxToken operatorToken = EatToken(SyntaxKind.EqualsToken);
            ExpressionSyntax right = ((base.CurrentToken.Kind == SyntaxKind.OpenBraceToken) ? ParseObjectOrCollectionInitializer() : ParseExpressionCore());
            ImplicitElementAccessSyntax left = _syntaxFactory.ImplicitElementAccess(argumentList);
            return _syntaxFactory.AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, left, operatorToken, right);
        }

        private InitializerExpressionSyntax ParseComplexElementInitializer()
        {
            SyntaxToken openBrace = EatToken(SyntaxKind.OpenBraceToken);
            SeparatedSyntaxListBuilder<ExpressionSyntax> builder = _pool.AllocateSeparated<ExpressionSyntax>();
            try
            {
                ParseExpressionsForComplexElementInitializer(ref openBrace, builder, out var closeBraceError);
                SyntaxToken syntaxToken = EatToken(SyntaxKind.CloseBraceToken);
                if (closeBraceError != null)
                {
                    syntaxToken = WithAdditionalDiagnostics(syntaxToken, closeBraceError);
                }
                return _syntaxFactory.InitializerExpression(SyntaxKind.ComplexElementInitializerExpression, openBrace, builder, syntaxToken);
            }
            finally
            {
                _pool.Free(in builder);
            }
        }

        private void ParseExpressionsForComplexElementInitializer(ref SyntaxToken openBrace, SeparatedSyntaxListBuilder<ExpressionSyntax> list, out DiagnosticInfo closeBraceError)
        {
            closeBraceError = null;
            if (base.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
            {
                return;
            }
            do
            {
                if (!IsPossibleExpression() && base.CurrentToken.Kind != SyntaxKind.CommaToken)
                {
                    continue;
                }
                list.Add(ParseExpressionCore());
                int lastTokenPosition = -1;
                while (IsMakingProgress(ref lastTokenPosition) && base.CurrentToken.Kind != SyntaxKind.CloseBraceToken)
                {
                    if (base.CurrentToken.Kind == SyntaxKind.CommaToken || IsPossibleExpression())
                    {
                        list.AddSeparator(EatToken(SyntaxKind.CommaToken));
                        if (base.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                        {
                            closeBraceError = SyntaxParser.MakeError(base.CurrentToken, ErrorCode.ERR_ExpressionExpected);
                            break;
                        }
                        list.Add(ParseExpressionCore());
                    }
                    else if (SkipBadInitializerListTokens(ref openBrace, list, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                    {
                        break;
                    }
                }
                break;
            }
            while (SkipBadInitializerListTokens(ref openBrace, list, SyntaxKind.IdentifierToken) == PostSkipAction.Continue);
        }

        private bool IsImplicitlyTypedArray()
        {
            return PeekToken(1).Kind == SyntaxKind.OpenBracketToken;
        }

        private ImplicitArrayCreationExpressionSyntax ParseImplicitlyTypedArrayCreation()
        {
            SyntaxToken node = EatToken(SyntaxKind.NewKeyword);
            node = CheckFeatureAvailability(node, MessageID.IDS_FeatureImplicitArray);
            SyntaxToken syntaxToken = EatToken(SyntaxKind.OpenBracketToken);
            SyntaxListBuilder syntaxListBuilder = _pool.Allocate();
            try
            {
                int lastTokenPosition = -1;
                while (IsMakingProgress(ref lastTokenPosition))
                {
                    if (IsPossibleExpression())
                    {
                        ExpressionSyntax skippedSyntax = AddError(ParseExpressionCore(), ErrorCode.ERR_InvalidArray);
                        if (syntaxListBuilder.Count == 0)
                        {
                            syntaxToken = AddTrailingSkippedSyntax(syntaxToken, skippedSyntax);
                        }
                        else
                        {
                            AddTrailingSkippedSyntax(syntaxListBuilder, skippedSyntax);
                        }
                    }
                    if (base.CurrentToken.Kind != SyntaxKind.CommaToken)
                    {
                        break;
                    }
                    syntaxListBuilder.Add(EatToken());
                }
                SyntaxToken closeBracketToken = EatToken(SyntaxKind.CloseBracketToken);
                InitializerExpressionSyntax initializer = ParseArrayInitializer();
                return _syntaxFactory.ImplicitArrayCreationExpression(node, syntaxToken, syntaxListBuilder.ToList(), closeBracketToken, initializer);
            }
            finally
            {
                _pool.Free(syntaxListBuilder);
            }
        }

        private InitializerExpressionSyntax ParseArrayInitializer()
        {
            SyntaxToken openBrace = EatToken(SyntaxKind.OpenBraceToken);
            SeparatedSyntaxListBuilder<ExpressionSyntax> builder = _pool.AllocateSeparated<ExpressionSyntax>();
            try
            {
                if (base.CurrentToken.Kind != SyntaxKind.CloseBraceToken)
                {
                    do
                    {
                    IL_002d:
                        if (!IsPossibleVariableInitializer() && base.CurrentToken.Kind != SyntaxKind.CommaToken)
                        {
                            continue;
                        }
                        builder.Add(ParseVariableInitializer());
                        int lastTokenPosition = -1;
                        while (IsMakingProgress(ref lastTokenPosition) && base.CurrentToken.Kind != SyntaxKind.CloseBraceToken)
                        {
                            if (IsPossibleVariableInitializer() || base.CurrentToken.Kind == SyntaxKind.CommaToken)
                            {
                                builder.AddSeparator(EatToken(SyntaxKind.CommaToken));
                                if (base.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                                {
                                    break;
                                }
                                if (!IsPossibleVariableInitializer())
                                {
                                    goto IL_002d;
                                }
                                builder.Add(ParseVariableInitializer());
                            }
                            else if (SkipBadArrayInitializerTokens(ref openBrace, builder, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                            {
                                break;
                            }
                        }
                        break;
                    }
                    while (SkipBadArrayInitializerTokens(ref openBrace, builder, SyntaxKind.CommaToken) == PostSkipAction.Continue);
                }
                SyntaxToken closeBraceToken = EatToken(SyntaxKind.CloseBraceToken);
                return _syntaxFactory.InitializerExpression(SyntaxKind.ArrayInitializerExpression, openBrace, builder, closeBraceToken);
            }
            finally
            {
                _pool.Free(in builder);
            }
        }

        private PostSkipAction SkipBadArrayInitializerTokens(ref SyntaxToken openBrace, SeparatedSyntaxListBuilder<ExpressionSyntax> list, SyntaxKind expected)
        {
            return SkipBadSeparatedListTokensWithExpectedKind(ref openBrace, list, (LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleVariableInitializer(), (LanguageParser p) => p.CurrentToken.Kind == SyntaxKind.CloseBraceToken || p.IsTerminator(), expected);
        }

        private ExpressionSyntax ParseStackAllocExpression()
        {
            if (IsImplicitlyTypedArray())
            {
                return ParseImplicitlyTypedStackAllocExpression();
            }
            return ParseRegularStackAllocExpression();
        }

        private ExpressionSyntax ParseImplicitlyTypedStackAllocExpression()
        {
            SyntaxToken node = EatToken(SyntaxKind.StackAllocKeyword);
            node = CheckFeatureAvailability(node, MessageID.IDS_FeatureStackAllocInitializer);
            SyntaxToken syntaxToken = EatToken(SyntaxKind.OpenBracketToken);
            int lastTokenPosition = -1;
            while (IsMakingProgress(ref lastTokenPosition))
            {
                if (IsPossibleExpression())
                {
                    ExpressionSyntax skippedSyntax = AddError(ParseExpressionCore(), ErrorCode.ERR_InvalidStackAllocArray);
                    syntaxToken = AddTrailingSkippedSyntax(syntaxToken, skippedSyntax);
                }
                if (base.CurrentToken.Kind != SyntaxKind.CommaToken)
                {
                    break;
                }
                SyntaxToken skippedSyntax2 = AddError(EatToken(), ErrorCode.ERR_InvalidStackAllocArray);
                syntaxToken = AddTrailingSkippedSyntax(syntaxToken, skippedSyntax2);
            }
            SyntaxToken closeBracketToken = EatToken(SyntaxKind.CloseBracketToken);
            InitializerExpressionSyntax initializer = ParseArrayInitializer();
            return _syntaxFactory.ImplicitStackAllocArrayCreationExpression(node, syntaxToken, closeBracketToken, initializer);
        }

        private ExpressionSyntax ParseRegularStackAllocExpression()
        {
            SyntaxToken syntaxToken = EatToken(SyntaxKind.StackAllocKeyword);
            TypeSyntax type = ParseType();
            InitializerExpressionSyntax initializer = null;
            if (base.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
            {
                syntaxToken = CheckFeatureAvailability(syntaxToken, MessageID.IDS_FeatureStackAllocInitializer);
                initializer = ParseArrayInitializer();
            }
            return _syntaxFactory.StackAllocArrayCreationExpression(syntaxToken, type, initializer);
        }

        private AnonymousMethodExpressionSyntax ParseAnonymousMethodExpression()
        {
            bool isInAsync = IsInAsync;
            AnonymousMethodExpressionSyntax result = parseAnonymousMethodExpressionWorker();
            IsInAsync = isInAsync;
            return result;
            AnonymousMethodExpressionSyntax parseAnonymousMethodExpressionWorker()
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> modifiers = ParseAnonymousFunctionModifiers();
                if (modifiers.Any(8435))
                {
                    IsInAsync = true;
                }
                SyntaxToken node = EatToken(SyntaxKind.DelegateKeyword);
                node = CheckFeatureAvailability(node, MessageID.IDS_FeatureAnonDelegates);
                ParameterListSyntax parameterList = null;
                if (base.CurrentToken.Kind == SyntaxKind.OpenParenToken)
                {
                    parameterList = ParseParenthesizedParameterList();
                }
                if (base.CurrentToken.Kind != SyntaxKind.OpenBraceToken)
                {
                    SyntaxToken openBraceToken = EatToken(SyntaxKind.OpenBraceToken);
                    return _syntaxFactory.AnonymousMethodExpression(modifiers, node, parameterList, _syntaxFactory.Block(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), openBraceToken, default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax>), SyntaxFactory.MissingToken(SyntaxKind.CloseBraceToken)), null);
                }
                BlockSyntax block = ParseBlock(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>));
                return _syntaxFactory.AnonymousMethodExpression(modifiers, node, parameterList, block, null);
            }
        }

        private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> ParseAnonymousFunctionModifiers()
        {
            SyntaxListBuilder syntaxListBuilder = _pool.Allocate();
            while (true)
            {
                if (base.CurrentToken.Kind == SyntaxKind.StaticKeyword)
                {
                    SyntaxToken node = EatToken(SyntaxKind.StaticKeyword);
                    node = CheckFeatureAvailability(node, MessageID.IDS_FeatureStaticAnonymousFunction);
                    syntaxListBuilder.Add(node);
                    continue;
                }
                if (base.CurrentToken.ContextualKind != SyntaxKind.AsyncKeyword || PeekToken(1).Kind == SyntaxKind.EqualsGreaterThanToken)
                {
                    break;
                }
                SyntaxToken node2 = EatContextualToken(SyntaxKind.AsyncKeyword);
                node2 = CheckFeatureAvailability(node2, MessageID.IDS_FeatureAsync);
                syntaxListBuilder.Add(node2);
            }
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> syntaxList = syntaxListBuilder.ToList();
            _pool.Free(syntaxListBuilder);
            return syntaxList;
        }

        private LambdaExpressionSyntax ParseLambdaExpression()
        {
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributes = ParseAttributeDeclarations();
            bool isInAsync = IsInAsync;
            LambdaExpressionSyntax result = parseLambdaExpressionWorker();
            IsInAsync = isInAsync;
            return result;
            LambdaExpressionSyntax parseLambdaExpressionWorker()
            {
                Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> modifiers = ParseAnonymousFunctionModifiers();
                if (modifiers.Any(8435))
                {
                    IsInAsync = true;
                }
                if (base.CurrentToken.Kind == SyntaxKind.OpenParenToken)
                {
                    ParameterListSyntax parameterList = ParseLambdaParameterList();
                    SyntaxToken node = EatToken(SyntaxKind.EqualsGreaterThanToken);
                    node = CheckFeatureAvailability(node, MessageID.IDS_FeatureLambda);
                    var (block, expressionBody) = ParseLambdaBody();
                    return _syntaxFactory.ParenthesizedLambdaExpression(attributes, modifiers, parameterList, node, block, expressionBody);
                }
                SyntaxToken identifier = ParseIdentifierToken();
                SyntaxToken node2 = EatToken(SyntaxKind.EqualsGreaterThanToken);
                node2 = CheckFeatureAvailability(node2, MessageID.IDS_FeatureLambda);
                ParameterSyntax parameter = _syntaxFactory.Parameter(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>), default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken>), null, identifier, null);
                var (block2, expressionBody2) = ParseLambdaBody();
                return _syntaxFactory.SimpleLambdaExpression(attributes, modifiers, parameter, node2, block2, expressionBody2);
            }
        }

        private (BlockSyntax, ExpressionSyntax) ParseLambdaBody()
        {
            if (base.CurrentToken.Kind != SyntaxKind.OpenBraceToken)
            {
                return (null, ParsePossibleRefExpression());
            }
            return (ParseBlock(default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax>)), null);
        }

        private ParameterListSyntax ParseLambdaParameterList()
        {
            SyntaxToken openParen = EatToken(SyntaxKind.OpenParenToken);
            TerminatorState termState = _termState;
            _termState |= TerminatorState.IsEndOfParameterList;
            SeparatedSyntaxListBuilder<ParameterSyntax> builder = _pool.AllocateSeparated<ParameterSyntax>();
            try
            {
                if (base.CurrentToken.Kind != SyntaxKind.CloseParenToken)
                {
                    do
                    {
                        if (base.CurrentToken.Kind != SyntaxKind.CommaToken && !IsPossibleLambdaParameter())
                        {
                            continue;
                        }
                        ParameterSyntax node = ParseLambdaParameter();
                        builder.Add(node);
                        int lastTokenPosition = -1;
                        while (IsMakingProgress(ref lastTokenPosition) && base.CurrentToken.Kind != SyntaxKind.CloseParenToken)
                        {
                            if (base.CurrentToken.Kind == SyntaxKind.CommaToken || IsPossibleLambdaParameter())
                            {
                                builder.AddSeparator(EatToken(SyntaxKind.CommaToken));
                                node = ParseLambdaParameter();
                                builder.Add(node);
                            }
                            else if (SkipBadLambdaParameterListTokens(ref openParen, builder, SyntaxKind.CommaToken, SyntaxKind.CloseParenToken) == PostSkipAction.Abort)
                            {
                                break;
                            }
                        }
                        break;
                    }
                    while (SkipBadLambdaParameterListTokens(ref openParen, builder, SyntaxKind.IdentifierToken, SyntaxKind.CloseParenToken) == PostSkipAction.Continue);
                }
                _termState = termState;
                SyntaxToken closeParenToken = EatToken(SyntaxKind.CloseParenToken);
                return _syntaxFactory.ParameterList(openParen, builder, closeParenToken);
            }
            finally
            {
                _pool.Free(in builder);
            }
        }

        private bool IsPossibleLambdaParameter()
        {
            switch (base.CurrentToken.Kind)
            {
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.OpenBracketToken:
                case SyntaxKind.RefKeyword:
                case SyntaxKind.OutKeyword:
                case SyntaxKind.InKeyword:
                case SyntaxKind.ParamsKeyword:
                    return true;
                case SyntaxKind.IdentifierToken:
                    return IsTrueIdentifier();
                case SyntaxKind.DelegateKeyword:
                    return IsFunctionPointerStart();
                default:
                    return IsPredefinedType(base.CurrentToken.Kind);
            }
        }

        private PostSkipAction SkipBadLambdaParameterListTokens(ref SyntaxToken openParen, SeparatedSyntaxListBuilder<ParameterSyntax> list, SyntaxKind expected, SyntaxKind closeKind)
        {
            return SkipBadSeparatedListTokensWithExpectedKind(ref openParen, list, (LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleLambdaParameter(), (LanguageParser p) => p.CurrentToken.Kind == closeKind || p.IsTerminator(), expected);
        }

        private ParameterSyntax ParseLambdaParameter()
        {
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<AttributeListSyntax> attributeLists = ParseAttributeDeclarations();
            bool flag = IsParameterModifier(base.CurrentToken.Kind);
            TypeSyntax type = null;
            SyntaxListBuilder syntaxListBuilder = _pool.Allocate();
            if (ShouldParseLambdaParameterType(flag))
            {
                if (flag)
                {
                    ParseParameterModifiers(syntaxListBuilder);
                }
                type = ParseType(ParseTypeMode.Parameter);
            }
            SyntaxToken identifier = ParseIdentifierToken();
            ParameterSyntax result = _syntaxFactory.Parameter(attributeLists, syntaxListBuilder.ToList(), type, identifier, null);
            _pool.Free(syntaxListBuilder);
            return result;
        }

        private bool ShouldParseLambdaParameterType(bool hasModifier)
        {
            if (hasModifier)
            {
                return true;
            }
            if (IsPredefinedType(base.CurrentToken.Kind))
            {
                return true;
            }
            if (base.CurrentToken.Kind == SyntaxKind.OpenParenToken)
            {
                return true;
            }
            if (IsFunctionPointerStart())
            {
                return true;
            }
            if (IsTrueIdentifier(base.CurrentToken))
            {
                SyntaxToken syntaxToken = PeekToken(1);
                if (syntaxToken.Kind != SyntaxKind.CommaToken && syntaxToken.Kind != SyntaxKind.CloseParenToken && syntaxToken.Kind != SyntaxKind.EqualsGreaterThanToken && syntaxToken.Kind != SyntaxKind.OpenBraceToken)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsTokenQueryContextualKeyword(SyntaxToken token)
        {
            if (IsTokenStartOfNewQueryClause(token))
            {
                return true;
            }
            SyntaxKind contextualKind = token.ContextualKind;
            if (contextualKind == SyntaxKind.ByKeyword || contextualKind - 8430 <= (SyntaxKind)3)
            {
                return true;
            }
            return false;
        }

        private static bool IsTokenStartOfNewQueryClause(SyntaxToken token)
        {
            SyntaxKind contextualKind = token.ContextualKind;
            if (contextualKind - 8421 <= (SyntaxKind)5 || contextualKind - 8428 <= SyntaxKind.List)
            {
                return true;
            }
            return false;
        }

        private bool IsQueryExpression(bool mayBeVariableDeclaration, bool mayBeMemberDeclaration)
        {
            if (base.CurrentToken.ContextualKind == SyntaxKind.FromKeyword)
            {
                return IsQueryExpressionAfterFrom(mayBeVariableDeclaration, mayBeMemberDeclaration);
            }
            return false;
        }

        private bool IsQueryExpressionAfterFrom(bool mayBeVariableDeclaration, bool mayBeMemberDeclaration)
        {
            SyntaxKind kind = PeekToken(1).Kind;
            if (IsPredefinedType(kind))
            {
                return true;
            }
            if (kind == SyntaxKind.IdentifierToken)
            {
                SyntaxKind kind2 = PeekToken(2).Kind;
                if (kind2 == SyntaxKind.InKeyword)
                {
                    return true;
                }
                if (mayBeVariableDeclaration && (kind2 == SyntaxKind.SemicolonToken || kind2 == SyntaxKind.CommaToken || kind2 == SyntaxKind.EqualsToken))
                {
                    return false;
                }
                if (!mayBeMemberDeclaration)
                {
                    return true;
                }
                if (kind2 == SyntaxKind.OpenParenToken || kind2 == SyntaxKind.OpenBraceToken)
                {
                    return false;
                }
            }
            ResetPoint state = GetResetPoint();
            try
            {
                EatToken();
                if (ScanType() != 0 && (base.CurrentToken.Kind == SyntaxKind.IdentifierToken || base.CurrentToken.Kind == SyntaxKind.InKeyword))
                {
                    return true;
                }
            }
            finally
            {
                Reset(ref state);
                Release(ref state);
            }
            return false;
        }

        private QueryExpressionSyntax ParseQueryExpression(Precedence precedence)
        {
            EnterQuery();
            FromClauseSyntax node = ParseFromClause();
            node = CheckFeatureAvailability(node, MessageID.IDS_FeatureQueryExpression);
            if (precedence != 0)
            {
                node = AddError(node, ErrorCode.WRN_PrecedenceInversion, SyntaxFacts.GetText(SyntaxKind.FromKeyword));
            }
            QueryBodySyntax body = ParseQueryBody();
            LeaveQuery();
            return _syntaxFactory.QueryExpression(node, body);
        }

        private QueryBodySyntax ParseQueryBody()
        {
            SyntaxListBuilder<QueryClauseSyntax> syntaxListBuilder = _pool.Allocate<QueryClauseSyntax>();
            try
            {
                SelectOrGroupClauseSyntax selectOrGroupClauseSyntax = null;
                QueryContinuationSyntax continuation = null;
                while (true)
                {
                    switch (base.CurrentToken.ContextualKind)
                    {
                        case SyntaxKind.FromKeyword:
                            {
                                FromClauseSyntax node = ParseFromClause();
                                syntaxListBuilder.Add(node);
                                continue;
                            }
                        case SyntaxKind.JoinKeyword:
                            syntaxListBuilder.Add(ParseJoinClause());
                            continue;
                        case SyntaxKind.LetKeyword:
                            syntaxListBuilder.Add(ParseLetClause());
                            continue;
                        case SyntaxKind.WhereKeyword:
                            syntaxListBuilder.Add(ParseWhereClause());
                            continue;
                        case SyntaxKind.OrderByKeyword:
                            syntaxListBuilder.Add(ParseOrderByClause());
                            continue;
                    }
                    selectOrGroupClauseSyntax = base.CurrentToken.ContextualKind switch
                    {
                        SyntaxKind.SelectKeyword => ParseSelectClause(),
                        SyntaxKind.GroupKeyword => ParseGroupClause(),
                        _ => _syntaxFactory.SelectClause(EatToken(SyntaxKind.SelectKeyword, ErrorCode.ERR_ExpectedSelectOrGroup), CreateMissingIdentifierName()),
                    };
                    if (base.CurrentToken.ContextualKind == SyntaxKind.IntoKeyword)
                    {
                        continuation = ParseQueryContinuation();
                    }
                    return _syntaxFactory.QueryBody(syntaxListBuilder, selectOrGroupClauseSyntax, continuation);
                }
            }
            finally
            {
                _pool.Free(syntaxListBuilder);
            }
        }

        private FromClauseSyntax ParseFromClause()
        {
            SyntaxToken node = EatContextualToken(SyntaxKind.FromKeyword);
            node = CheckFeatureAvailability(node, MessageID.IDS_FeatureQueryExpression);
            TypeSyntax type = null;
            if (PeekToken(1).Kind != SyntaxKind.InKeyword)
            {
                type = ParseType();
            }
            SyntaxToken syntaxToken;
            if (PeekToken(1).ContextualKind == SyntaxKind.InKeyword && (base.CurrentToken.Kind != SyntaxKind.IdentifierToken || SyntaxFacts.IsQueryContextualKeyword(base.CurrentToken.ContextualKind)))
            {
                syntaxToken = EatToken();
                syntaxToken = WithAdditionalDiagnostics(syntaxToken, GetExpectedTokenError(SyntaxKind.IdentifierToken, syntaxToken.ContextualKind, syntaxToken.GetLeadingTriviaWidth(), syntaxToken.Width));
                syntaxToken = ConvertToMissingWithTrailingTrivia(syntaxToken, SyntaxKind.IdentifierToken);
            }
            else
            {
                syntaxToken = ParseIdentifierToken();
            }
            SyntaxToken inKeyword = EatToken(SyntaxKind.InKeyword);
            ExpressionSyntax expression = ParseExpressionCore();
            return _syntaxFactory.FromClause(node, type, syntaxToken, inKeyword, expression);
        }

        private JoinClauseSyntax ParseJoinClause()
        {
            SyntaxToken joinKeyword = EatContextualToken(SyntaxKind.JoinKeyword);
            TypeSyntax type = null;
            if (PeekToken(1).Kind != SyntaxKind.InKeyword)
            {
                type = ParseType();
            }
            SyntaxToken identifier = ParseIdentifierToken();
            SyntaxToken inKeyword = EatToken(SyntaxKind.InKeyword);
            ExpressionSyntax inExpression = ParseExpressionCore();
            SyntaxToken onKeyword = EatContextualToken(SyntaxKind.OnKeyword, ErrorCode.ERR_ExpectedContextualKeywordOn);
            ExpressionSyntax leftExpression = ParseExpressionCore();
            SyntaxToken equalsKeyword = EatContextualToken(SyntaxKind.EqualsKeyword, ErrorCode.ERR_ExpectedContextualKeywordEquals);
            ExpressionSyntax rightExpression = ParseExpressionCore();
            JoinIntoClauseSyntax into = null;
            if (base.CurrentToken.ContextualKind == SyntaxKind.IntoKeyword)
            {
                SyntaxToken intoKeyword = SyntaxParser.ConvertToKeyword(EatToken());
                SyntaxToken identifier2 = ParseIdentifierToken();
                into = _syntaxFactory.JoinIntoClause(intoKeyword, identifier2);
            }
            return _syntaxFactory.JoinClause(joinKeyword, type, identifier, inKeyword, inExpression, onKeyword, leftExpression, equalsKeyword, rightExpression, into);
        }

        private LetClauseSyntax ParseLetClause()
        {
            SyntaxToken letKeyword = EatContextualToken(SyntaxKind.LetKeyword);
            SyntaxToken identifier = ParseIdentifierToken();
            SyntaxToken equalsToken = EatToken(SyntaxKind.EqualsToken);
            ExpressionSyntax expression = ParseExpressionCore();
            return _syntaxFactory.LetClause(letKeyword, identifier, equalsToken, expression);
        }

        private WhereClauseSyntax ParseWhereClause()
        {
            SyntaxToken whereKeyword = EatContextualToken(SyntaxKind.WhereKeyword);
            ExpressionSyntax condition = ParseExpressionCore();
            return _syntaxFactory.WhereClause(whereKeyword, condition);
        }

        private OrderByClauseSyntax ParseOrderByClause()
        {
            SyntaxToken orderByKeyword = EatContextualToken(SyntaxKind.OrderByKeyword);
            SeparatedSyntaxListBuilder<OrderingSyntax> item = _pool.AllocateSeparated<OrderingSyntax>();
            try
            {
                item.Add(ParseOrdering());
                while (base.CurrentToken.Kind == SyntaxKind.CommaToken && base.CurrentToken.Kind != SyntaxKind.CloseParenToken && base.CurrentToken.Kind != SyntaxKind.SemicolonToken)
                {
                    if (base.CurrentToken.Kind == SyntaxKind.CommaToken)
                    {
                        item.AddSeparator(EatToken(SyntaxKind.CommaToken));
                        item.Add(ParseOrdering());
                    }
                    else if (SkipBadOrderingListTokens(item, SyntaxKind.CommaToken) == PostSkipAction.Abort)
                    {
                        break;
                    }
                }
                return _syntaxFactory.OrderByClause(orderByKeyword, item);
            }
            finally
            {
                _pool.Free(in item);
            }
        }

        private PostSkipAction SkipBadOrderingListTokens(SeparatedSyntaxListBuilder<OrderingSyntax> list, SyntaxKind expected)
        {
            CSharpSyntaxNode startToken = null;
            return SkipBadSeparatedListTokensWithExpectedKind(ref startToken, list, (LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CommaToken, (LanguageParser p) => p.CurrentToken.Kind == SyntaxKind.CloseParenToken || p.CurrentToken.Kind == SyntaxKind.SemicolonToken || p.IsCurrentTokenQueryContextualKeyword || p.IsTerminator(), expected);
        }

        private OrderingSyntax ParseOrdering()
        {
            ExpressionSyntax expression = ParseExpressionCore();
            SyntaxToken syntaxToken = null;
            SyntaxKind kind = SyntaxKind.AscendingOrdering;
            if (base.CurrentToken.ContextualKind == SyntaxKind.AscendingKeyword || base.CurrentToken.ContextualKind == SyntaxKind.DescendingKeyword)
            {
                syntaxToken = SyntaxParser.ConvertToKeyword(EatToken());
                if (syntaxToken.Kind == SyntaxKind.DescendingKeyword)
                {
                    kind = SyntaxKind.DescendingOrdering;
                }
            }
            return _syntaxFactory.Ordering(kind, expression, syntaxToken);
        }

        private SelectClauseSyntax ParseSelectClause()
        {
            SyntaxToken selectKeyword = EatContextualToken(SyntaxKind.SelectKeyword);
            ExpressionSyntax expression = ParseExpressionCore();
            return _syntaxFactory.SelectClause(selectKeyword, expression);
        }

        private GroupClauseSyntax ParseGroupClause()
        {
            SyntaxToken groupKeyword = EatContextualToken(SyntaxKind.GroupKeyword);
            ExpressionSyntax groupExpression = ParseExpressionCore();
            SyntaxToken byKeyword = EatContextualToken(SyntaxKind.ByKeyword, ErrorCode.ERR_ExpectedContextualKeywordBy);
            ExpressionSyntax byExpression = ParseExpressionCore();
            return _syntaxFactory.GroupClause(groupKeyword, groupExpression, byKeyword, byExpression);
        }

        private QueryContinuationSyntax ParseQueryContinuation()
        {
            SyntaxToken intoKeyword = EatContextualToken(SyntaxKind.IntoKeyword);
            SyntaxToken identifier = ParseIdentifierToken();
            QueryBodySyntax body = ParseQueryBody();
            return _syntaxFactory.QueryContinuation(intoKeyword, identifier, body);
        }

        internal static bool MatchesFactoryContext(GreenNode green, SyntaxFactoryContext context)
        {
            if (context.IsInAsync == green.ParsedInAsync)
            {
                return context.IsInQuery == green.ParsedInQuery;
            }
            return false;
        }

        private void EnterQuery()
        {
            _syntaxFactoryContext.QueryDepth++;
        }

        private void LeaveQuery()
        {
            _syntaxFactoryContext.QueryDepth--;
        }

        private new ResetPoint GetResetPoint()
        {
            return new ResetPoint(base.GetResetPoint(), _termState, _isInTry, _syntaxFactoryContext.IsInAsync, _syntaxFactoryContext.QueryDepth);
        }

        private void Reset(ref ResetPoint state)
        {
            _termState = state.TerminatorState;
            _isInTry = state.IsInTry;
            _syntaxFactoryContext.IsInAsync = state.IsInAsync;
            _syntaxFactoryContext.QueryDepth = state.QueryDepth;
            Reset(ref state.BaseResetPoint);
        }

        private void Release(ref ResetPoint state)
        {
            Release(ref state.BaseResetPoint);
        }

        internal TNode ConsumeUnexpectedTokens<TNode>(TNode node) where TNode : CSharpSyntaxNode
        {
            if (base.CurrentToken.Kind == SyntaxKind.EndOfFileToken)
            {
                return node;
            }
            SyntaxListBuilder<SyntaxToken> syntaxListBuilder = _pool.Allocate<SyntaxToken>();
            while (base.CurrentToken.Kind != SyntaxKind.EndOfFileToken)
            {
                syntaxListBuilder.Add(EatToken());
            }
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> syntaxList = syntaxListBuilder.ToList();
            _pool.Free(syntaxListBuilder);
            node = AddError(node, ErrorCode.ERR_UnexpectedToken, syntaxList[0]!.ToString());
            node = AddTrailingSkippedSyntax(node, syntaxList.Node);
            return node;
        }

        private string Substring(string s, int first, int last)
        {
            if (last >= s.Length)
            {
                last = s.Length - 1;
            }
            int num = last - first + 1;
            if (last <= s.Length && num > 0)
            {
                return s.Substring(first, num);
            }
            return string.Empty;
        }

        private ExpressionSyntax ParseInterpolatedStringToken()
        {
            SyntaxToken syntaxToken = EatToken();
            string valueText = syntaxToken.ValueText;
            bool flag = valueText.Length > 2 && valueText[0] == '@';
            bool flag2 = flag || (valueText.Length > 2 && valueText[1] == '@');
            ArrayBuilder<Lexer.Interpolation> instance = ArrayBuilder<Lexer.Interpolation>.GetInstance();
            SyntaxDiagnosticInfo error = null;
            bool closeQuoteMissing;
            using (Lexer lexer = new Lexer(SourceText.From(valueText), base.Options, allowPreprocessorDirectives: false))
            {
                Lexer.TokenInfo info = default(Lexer.TokenInfo);
                lexer.ScanInterpolatedStringLiteralTop(instance, flag2, ref info, ref error, out closeQuoteMissing);
            }
            int num = ((!flag2) ? 1 : 2);
            SyntaxKind kind = (flag2 ? SyntaxKind.InterpolatedVerbatimStringStartToken : SyntaxKind.InterpolatedStringStartToken);
            string text = (flag ? "@$\"" : (flag2 ? "$@\"" : "$\""));
            SyntaxToken syntaxToken2 = SyntaxFactory.Token(syntaxToken.GetLeadingTrivia(), kind, text, text, null);
            if (flag)
            {
                syntaxToken2 = CheckFeatureAvailability(syntaxToken2, MessageID.IDS_FeatureAltInterpolatedVerbatimStrings);
            }
            int num2 = (closeQuoteMissing ? valueText.Length : (valueText.Length - 1));
            SyntaxToken stringEndToken = (closeQuoteMissing ? SyntaxFactory.MissingToken(SyntaxKind.InterpolatedStringEndToken).TokenWithTrailingTrivia(syntaxToken.GetTrailingTrivia()) : SyntaxFactory.Token(null, SyntaxKind.InterpolatedStringEndToken, syntaxToken.GetTrailingTrivia()));
            SyntaxListBuilder<InterpolatedStringContentSyntax> syntaxListBuilder = _pool.Allocate<InterpolatedStringContentSyntax>();
            if (instance.Count == 0)
            {
                string text2 = Substring(valueText, num + 1, num2 - 1);
                if (text2.Length > 0)
                {
                    SyntaxToken textToken = MakeStringToken(text2, text2, flag2, SyntaxKind.InterpolatedStringTextToken);
                    syntaxListBuilder.Add(SyntaxFactory.InterpolatedStringText(textToken));
                }
            }
            else
            {
                for (int i = 0; i < instance.Count; i++)
                {
                    Lexer.Interpolation interpolation = instance[i];
                    string text3 = Substring(valueText, (i == 0) ? (num + 1) : (instance[i - 1].CloseBracePosition + 1), interpolation.OpenBracePosition - 1);
                    if (text3.Length > 0)
                    {
                        SyntaxToken textToken2 = MakeStringToken(text3, text3, flag2, SyntaxKind.InterpolatedStringTextToken);
                        syntaxListBuilder.Add(SyntaxFactory.InterpolatedStringText(textToken2));
                    }
                    InterpolationSyntax node = ParseInterpolation(valueText, interpolation, flag2);
                    syntaxListBuilder.Add(node);
                }
                string text4 = Substring(valueText, instance[instance.Count - 1].CloseBracePosition + 1, num2 - 1);
                if (text4.Length > 0)
                {
                    SyntaxToken textToken3 = MakeStringToken(text4, text4, flag2, SyntaxKind.InterpolatedStringTextToken);
                    syntaxListBuilder.Add(SyntaxFactory.InterpolatedStringText(textToken3));
                }
            }
            instance.Free();
            InterpolatedStringExpressionSyntax interpolatedStringExpressionSyntax = SyntaxFactory.InterpolatedStringExpression(syntaxToken2, syntaxListBuilder, stringEndToken);
            _pool.Free(syntaxListBuilder);
            if (error != null)
            {
                InterpolatedStringExpressionSyntax node2 = interpolatedStringExpressionSyntax;
                DiagnosticInfo[] diagnostics = new SyntaxDiagnosticInfo[1] { error };
                interpolatedStringExpressionSyntax = node2.WithDiagnosticsGreen(diagnostics);
            }
            return CheckFeatureAvailability(interpolatedStringExpressionSyntax, MessageID.IDS_FeatureInterpolatedStrings);
        }

        private InterpolationSyntax ParseInterpolation(string text, Lexer.Interpolation interpolation, bool isVerbatim)
        {
            InterpolationAlignmentClauseSyntax alignmentClause = null;
            InterpolationFormatClauseSyntax formatClause = null;
            SyntaxToken syntaxToken = (interpolation.CloseBraceMissing ? SyntaxFactory.MissingToken(SyntaxKind.CloseBraceToken) : SyntaxFactory.Token(SyntaxKind.CloseBraceToken));
            SyntaxToken openBraceToken;
            ExpressionSyntax expr;
            using (Lexer lexer = new Lexer(SourceText.From(Substring(text, interpolation.OpenBracePosition, interpolation.HasColon ? (interpolation.ColonPosition - 1) : (interpolation.CloseBracePosition - 1))), base.Options, allowPreprocessorDirectives: false, interpolation.HasColon))
            {
                using LanguageParser languageParser = new LanguageParser(lexer, null, null);
                languageParser.ParseInterpolationStart(out openBraceToken, out expr, out SyntaxToken commaToken, out ExpressionSyntax alignmentExpression);
                if (alignmentExpression != null)
                {
                    alignmentClause = SyntaxFactory.InterpolationAlignmentClause(commaToken, alignmentExpression);
                }
                GreenNode leadingTrivia = languageParser.CurrentToken.GetLeadingTrivia();
                if (interpolation.HasColon)
                {
                    SyntaxToken colonToken = SyntaxFactory.Token(SyntaxKind.ColonToken).TokenWithLeadingTrivia(leadingTrivia);
                    string text2 = Substring(text, interpolation.ColonPosition + 1, interpolation.FormatEndPosition);
                    SyntaxToken formatStringToken = MakeStringToken(text2, text2, isVerbatim, SyntaxKind.InterpolatedStringTextToken);
                    formatClause = SyntaxFactory.InterpolationFormatClause(colonToken, formatStringToken);
                }
                else
                {
                    syntaxToken = syntaxToken.TokenWithLeadingTrivia(leadingTrivia);
                }
            }
            return SyntaxFactory.Interpolation(openBraceToken, expr, alignmentClause, formatClause, syntaxToken);
        }

        private SyntaxToken MakeStringToken(string text, string bodyText, bool isVerbatim, SyntaxKind kind)
        {
            string text2 = (isVerbatim ? "@\"" : "\"");
            using Lexer lexer = new Lexer(SourceText.From(text2 + bodyText + "\""), base.Options, allowPreprocessorDirectives: false);
            LexerMode mode = LexerMode.Syntax;
            SyntaxToken syntaxToken = lexer.Lex(ref mode);
            SyntaxToken syntaxToken2 = SyntaxFactory.Literal(null, text, kind, syntaxToken.ValueText, null);
            if (syntaxToken.ContainsDiagnostics)
            {
                syntaxToken2 = syntaxToken2.WithDiagnosticsGreen(MoveDiagnostics(syntaxToken.GetDiagnostics(), -text2.Length));
            }
            return syntaxToken2;
        }

        private static DiagnosticInfo[] MoveDiagnostics(DiagnosticInfo[] infos, int offset)
        {
            ArrayBuilder<DiagnosticInfo> instance = ArrayBuilder<DiagnosticInfo>.GetInstance();
            foreach (DiagnosticInfo diagnosticInfo in infos)
            {
                SyntaxDiagnosticInfo syntaxDiagnosticInfo = diagnosticInfo as SyntaxDiagnosticInfo;
                instance.Add(syntaxDiagnosticInfo?.WithOffset(syntaxDiagnosticInfo.Offset + offset) ?? diagnosticInfo);
            }
            return instance.ToArrayAndFree();
        }

        private void ParseInterpolationStart(out SyntaxToken openBraceToken, out ExpressionSyntax expr, out SyntaxToken commaToken, out ExpressionSyntax alignmentExpression)
        {
            openBraceToken = EatToken(SyntaxKind.OpenBraceToken);
            expr = ParseExpressionCore();
            if (base.CurrentToken.Kind == SyntaxKind.CommaToken)
            {
                commaToken = EatToken(SyntaxKind.CommaToken);
                alignmentExpression = ConsumeUnexpectedTokens(ParseExpressionCore());
            }
            else
            {
                commaToken = null;
                alignmentExpression = null;
                expr = ConsumeUnexpectedTokens(expr);
            }
        }

        private CSharpSyntaxNode ParseTypeOrPatternForIsOperator()
        {
            return CheckRecursivePatternFeature(ParseTypeOrPatternForIsOperatorCore());
        }

        private CSharpSyntaxNode CheckRecursivePatternFeature(CSharpSyntaxNode node)
        {
            SyntaxKind kind = node.Kind;
            if (kind == SyntaxKind.RecursivePattern || kind == SyntaxKind.DiscardPattern || (kind == SyntaxKind.VarPattern && ((VarPatternSyntax)node).Designation.Kind == SyntaxKind.ParenthesizedVariableDesignation))
            {
                return CheckFeatureAvailability(node, MessageID.IDS_FeatureRecursivePatterns);
            }
            return node;
        }

        private CSharpSyntaxNode ParseTypeOrPatternForIsOperatorCore()
        {
            PatternSyntax patternSyntax = ParsePattern(GetPrecedence(SyntaxKind.IsPatternExpression), afterIs: true);
            if (!(patternSyntax is ConstantPatternSyntax constantPatternSyntax))
            {
                if (patternSyntax is TypePatternSyntax typePatternSyntax)
                {
                    return typePatternSyntax.Type;
                }
                if (patternSyntax is DiscardPatternSyntax discardPatternSyntax)
                {
                    DiscardPatternSyntax discardPatternSyntax2 = discardPatternSyntax;
                    return _syntaxFactory.IdentifierName(SyntaxParser.ConvertToIdentifier(discardPatternSyntax2.UnderscoreToken));
                }
            }
            else
            {
                ConstantPatternSyntax constantPatternSyntax2 = constantPatternSyntax;
                if (ConvertExpressionToType(constantPatternSyntax2.Expression, out var type))
                {
                    return type;
                }
            }
            return patternSyntax;
        }

        private bool ConvertExpressionToType(ExpressionSyntax expression, out NameSyntax type)
        {
            type = null;
            if (!(expression is SimpleNameSyntax simpleNameSyntax))
            {
                if (expression is MemberAccessExpressionSyntax memberAccessExpressionSyntax)
                {
                    ExpressionSyntax expression2 = memberAccessExpressionSyntax.Expression;
                    SyntaxToken operatorToken = memberAccessExpressionSyntax.OperatorToken;
                    if (operatorToken != null && operatorToken.Kind == SyntaxKind.DotToken)
                    {
                        SimpleNameSyntax name = memberAccessExpressionSyntax.Name;
                        ExpressionSyntax expression3 = expression2;
                        SyntaxToken dotToken = operatorToken;
                        SimpleNameSyntax right = name;
                        if (ConvertExpressionToType(expression3, out var type2))
                        {
                            type = _syntaxFactory.QualifiedName(type2, dotToken, right);
                            return true;
                        }
                    }
                }
                else if (expression is AliasQualifiedNameSyntax aliasQualifiedNameSyntax)
                {
                    AliasQualifiedNameSyntax aliasQualifiedNameSyntax2 = (AliasQualifiedNameSyntax)(type = aliasQualifiedNameSyntax);
                    return true;
                }
                return false;
            }
            SimpleNameSyntax simpleNameSyntax2 = (SimpleNameSyntax)(type = simpleNameSyntax);
            return true;
        }

        private PatternSyntax ParsePattern(Precedence precedence, bool afterIs = false, bool whenIsKeyword = false)
        {
            return ParseDisjunctivePattern(precedence, afterIs, whenIsKeyword);
        }

        private PatternSyntax ParseDisjunctivePattern(Precedence precedence, bool afterIs, bool whenIsKeyword)
        {
            PatternSyntax patternSyntax = ParseConjunctivePattern(precedence, afterIs, whenIsKeyword);
            while (base.CurrentToken.ContextualKind == SyntaxKind.OrKeyword)
            {
                SyntaxToken operatorToken = SyntaxParser.ConvertToKeyword(EatToken());
                PatternSyntax right = ParseConjunctivePattern(precedence, afterIs, whenIsKeyword);
                patternSyntax = _syntaxFactory.BinaryPattern(SyntaxKind.OrPattern, patternSyntax, operatorToken, right);
                patternSyntax = CheckFeatureAvailability(patternSyntax, MessageID.IDS_FeatureOrPattern);
            }
            return patternSyntax;
        }

        private bool LooksLikeTypeOfPattern()
        {
            SyntaxKind kind = base.CurrentToken.Kind;
            if (SyntaxFacts.IsPredefinedType(kind))
            {
                return true;
            }
            if (kind == SyntaxKind.IdentifierToken && base.CurrentToken.ContextualKind != SyntaxKind.UnderscoreToken && (base.CurrentToken.ContextualKind != SyntaxKind.NameOfKeyword || PeekToken(1).Kind != SyntaxKind.OpenParenToken))
            {
                return true;
            }
            if (LooksLikeTupleArrayType())
            {
                return true;
            }
            if (IsFunctionPointerStart())
            {
                return true;
            }
            return false;
        }

        private PatternSyntax ParseConjunctivePattern(Precedence precedence, bool afterIs, bool whenIsKeyword)
        {
            PatternSyntax patternSyntax = ParseNegatedPattern(precedence, afterIs, whenIsKeyword);
            while (base.CurrentToken.ContextualKind == SyntaxKind.AndKeyword)
            {
                SyntaxToken operatorToken = SyntaxParser.ConvertToKeyword(EatToken());
                PatternSyntax right = ParseNegatedPattern(precedence, afterIs, whenIsKeyword);
                patternSyntax = _syntaxFactory.BinaryPattern(SyntaxKind.AndPattern, patternSyntax, operatorToken, right);
                patternSyntax = CheckFeatureAvailability(patternSyntax, MessageID.IDS_FeatureAndPattern);
            }
            return patternSyntax;
        }

        private bool ScanDesignation(bool permitTuple)
        {
            switch (base.CurrentToken.Kind)
            {
                default:
                    return false;
                case SyntaxKind.IdentifierToken:
                    {
                        bool result2 = IsTrueIdentifier();
                        EatToken();
                        return result2;
                    }
                case SyntaxKind.OpenParenToken:
                    {
                        if (!permitTuple)
                        {
                            return false;
                        }
                        bool result = false;
                        while (true)
                        {
                            EatToken();
                            if (!ScanDesignation(permitTuple: true))
                            {
                                break;
                            }
                            switch (base.CurrentToken.Kind)
                            {
                                case SyntaxKind.CloseParenToken:
                                    EatToken();
                                    return result;
                                case SyntaxKind.CommaToken:
                                    break;
                                default:
                                    return false;
                            }
                            result = true;
                        }
                        return false;
                    }
            }
        }

        private PatternSyntax ParseNegatedPattern(Precedence precedence, bool afterIs, bool whenIsKeyword)
        {
            if (base.CurrentToken.ContextualKind == SyntaxKind.NotKeyword)
            {
                SyntaxToken operatorToken = SyntaxParser.ConvertToKeyword(EatToken());
                PatternSyntax pattern = ParseNegatedPattern(precedence, afterIs, whenIsKeyword);
                UnaryPatternSyntax node = _syntaxFactory.UnaryPattern(operatorToken, pattern);
                return CheckFeatureAvailability(node, MessageID.IDS_FeatureNotPattern);
            }
            return ParsePrimaryPattern(precedence, afterIs, whenIsKeyword);
        }

        private PatternSyntax ParsePrimaryPattern(Precedence precedence, bool afterIs, bool whenIsKeyword)
        {
            switch (base.CurrentToken.Kind)
            {
                case SyntaxKind.CloseParenToken:
                case SyntaxKind.CloseBraceToken:
                case SyntaxKind.CloseBracketToken:
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.CommaToken:
                case SyntaxKind.EqualsGreaterThanToken:
                    return _syntaxFactory.ConstantPattern(ParseIdentifierName(ErrorCode.ERR_MissingPattern));
                default:
                    if (base.CurrentToken.ContextualKind == SyntaxKind.UnderscoreToken)
                    {
                        return _syntaxFactory.DiscardPattern(EatContextualToken(SyntaxKind.UnderscoreToken));
                    }
                    switch (base.CurrentToken.Kind)
                    {
                        case SyntaxKind.LessThanToken:
                        case SyntaxKind.GreaterThanToken:
                        case SyntaxKind.ExclamationEqualsToken:
                        case SyntaxKind.EqualsEqualsToken:
                        case SyntaxKind.LessThanEqualsToken:
                        case SyntaxKind.GreaterThanEqualsToken:
                            {
                                SyntaxToken operatorToken = EatToken();
                                ExpressionSyntax expression2 = ParseSubExpression(Precedence.Relational);
                                RelationalPatternSyntax node = _syntaxFactory.RelationalPattern(operatorToken, expression2);
                                return CheckFeatureAvailability(node, MessageID.IDS_FeatureRelationalPattern);
                            }
                        default:
                            {
                                ResetPoint state = GetResetPoint();
                                try
                                {
                                    TypeSyntax typeSyntax = null;
                                    if (LooksLikeTypeOfPattern())
                                    {
                                        typeSyntax = ParseType(afterIs ? ParseTypeMode.AfterIs : ParseTypeMode.DefinitePattern);
                                        if (typeSyntax.IsMissing || !CanTokenFollowTypeInPattern(precedence))
                                        {
                                            Reset(ref state);
                                            typeSyntax = null;
                                        }
                                    }
                                    PatternSyntax patternSyntax = ParsePatternContinued(typeSyntax, precedence, whenIsKeyword);
                                    if (patternSyntax != null)
                                    {
                                        return patternSyntax;
                                    }
                                    Reset(ref state);
                                    ExpressionSyntax expression = ParseSubExpression(precedence);
                                    return _syntaxFactory.ConstantPattern(expression);
                                }
                                finally
                                {
                                    Release(ref state);
                                }
                            }
                    }
            }
        }

        private bool CanTokenFollowTypeInPattern(Precedence precedence)
        {
            SyntaxKind kind = base.CurrentToken.Kind;
            switch (kind)
            {
                case SyntaxKind.OpenParenToken:
                case SyntaxKind.CloseParenToken:
                case SyntaxKind.OpenBraceToken:
                case SyntaxKind.CloseBraceToken:
                case SyntaxKind.CloseBracketToken:
                case SyntaxKind.SemicolonToken:
                case SyntaxKind.CommaToken:
                case SyntaxKind.IdentifierToken:
                    return true;
                case SyntaxKind.DotToken:
                    return false;
                default:
                    if (SyntaxFacts.IsBinaryExpressionOperatorToken(kind))
                    {
                        return GetPrecedence(SyntaxFacts.GetBinaryExpression(kind)) <= precedence;
                    }
                    return true;
            }
        }

        private PatternSyntax ParsePatternContinued(TypeSyntax type, Precedence precedence, bool whenIsKeyword)
        {
            if (type != null && type.Kind == SyntaxKind.IdentifierName)
            {
                SyntaxToken identifier = ((IdentifierNameSyntax)type).Identifier;
                if (identifier.ContextualKind == SyntaxKind.VarKeyword && (base.CurrentToken.Kind == SyntaxKind.OpenParenToken || IsValidPatternDesignation(whenIsKeyword)))
                {
                    SyntaxToken varKeyword = SyntaxParser.ConvertToKeyword(identifier);
                    VariableDesignationSyntax designation = ParseDesignation(forPattern: true);
                    return _syntaxFactory.VarPattern(varKeyword, designation);
                }
            }
            if (base.CurrentToken.Kind == SyntaxKind.OpenParenToken && (type != null || !looksLikeCast()))
            {
                ParseSubpatternList(out var openToken, out var subPatterns, out var closeToken, SyntaxKind.OpenParenToken, SyntaxKind.CloseParenToken);
                parsePropertyPatternClause(out var propertyPatternClauseResult2);
                parseDesignation(out var designationResult2);
                if (type == null && propertyPatternClauseResult2 == null && designationResult2 == null && subPatterns.Count == 1 && subPatterns.SeparatorCount == 0 && subPatterns[0]!.NameColon == null)
                {
                    if (subPatterns[0]!.Pattern is ConstantPatternSyntax constantPatternSyntax)
                    {
                        ExpressionSyntax leftOperand = _syntaxFactory.ParenthesizedExpression(openToken, constantPatternSyntax.Expression, closeToken);
                        leftOperand = ParseExpressionContinued(leftOperand, precedence);
                        return _syntaxFactory.ConstantPattern(leftOperand);
                    }
                    ParenthesizedPatternSyntax node = _syntaxFactory.ParenthesizedPattern(openToken, subPatterns[0]!.Pattern, closeToken);
                    return CheckFeatureAvailability(node, MessageID.IDS_FeatureParenthesizedPattern);
                }
                PositionalPatternClauseSyntax positionalPatternClause = _syntaxFactory.PositionalPatternClause(openToken, subPatterns, closeToken);
                return _syntaxFactory.RecursivePattern(type, positionalPatternClause, propertyPatternClauseResult2, designationResult2);
            }
            if (parsePropertyPatternClause(out var propertyPatternClauseResult3))
            {
                parseDesignation(out var designationResult3);
                return _syntaxFactory.RecursivePattern(type, null, propertyPatternClauseResult3, designationResult3);
            }
            if (type != null)
            {
                if (parseDesignation(out var designationResult4))
                {
                    return _syntaxFactory.DeclarationPattern(type, designationResult4);
                }
                if (ConvertTypeToExpression(type, out var expr))
                {
                    expr = ParseExpressionContinued(expr, precedence);
                    return _syntaxFactory.ConstantPattern(expr);
                }
                TypePatternSyntax node2 = _syntaxFactory.TypePattern(type);
                return CheckFeatureAvailability(node2, MessageID.IDS_FeatureTypePattern);
            }
            return null;
            bool looksLikeCast()
            {
                ResetPoint state = GetResetPoint();
                bool result = ScanCast(forPattern: true);
                Reset(ref state);
                Release(ref state);
                return result;
            }
            bool parseDesignation(out VariableDesignationSyntax designationResult)
            {
                designationResult = null;
                if (IsTrueIdentifier() && IsValidPatternDesignation(whenIsKeyword))
                {
                    designationResult = ParseSimpleDesignation();
                    return true;
                }
                return false;
            }
            bool parsePropertyPatternClause(out PropertyPatternClauseSyntax propertyPatternClauseResult)
            {
                propertyPatternClauseResult = null;
                if (base.CurrentToken.Kind == SyntaxKind.OpenBraceToken)
                {
                    propertyPatternClauseResult = ParsePropertyPatternClause();
                    return true;
                }
                return false;
            }
        }

        private bool IsValidPatternDesignation(bool whenIsKeyword)
        {
            if (base.CurrentToken.Kind == SyntaxKind.IdentifierToken)
            {
                switch (base.CurrentToken.ContextualKind)
                {
                    case SyntaxKind.WhenKeyword:
                        return !whenIsKeyword;
                    case SyntaxKind.OrKeyword:
                    case SyntaxKind.AndKeyword:
                        {
                            SyntaxKind kind = PeekToken(1).Kind;
                            switch (kind)
                            {
                                case SyntaxKind.CloseParenToken:
                                case SyntaxKind.CloseBraceToken:
                                case SyntaxKind.CloseBracketToken:
                                case SyntaxKind.ColonToken:
                                case SyntaxKind.SemicolonToken:
                                case SyntaxKind.CommaToken:
                                case SyntaxKind.QuestionToken:
                                    return true;
                                case SyntaxKind.OpenParenToken:
                                case SyntaxKind.OpenBraceToken:
                                case SyntaxKind.LessThanToken:
                                case SyntaxKind.GreaterThanToken:
                                case SyntaxKind.LessThanEqualsToken:
                                case SyntaxKind.GreaterThanEqualsToken:
                                case SyntaxKind.IdentifierToken:
                                    return false;
                                default:
                                    if (!SyntaxFacts.IsBinaryExpression(kind))
                                    {
                                        ResetPoint state = GetResetPoint();
                                        EatToken();
                                        bool result = !CanStartExpression();
                                        Reset(ref state);
                                        Release(ref state);
                                        return result;
                                    }
                                    return true;
                            }
                        }
                    default:
                        return true;
                }
            }
            return false;
        }

        private CSharpSyntaxNode ParseExpressionOrPatternForSwitchStatement()
        {
            return CheckRecursivePatternFeature(ParseExpressionOrPatternForSwitchStatementCore());
        }

        private CSharpSyntaxNode ParseExpressionOrPatternForSwitchStatementCore()
        {
            PatternSyntax patternSyntax = ParsePattern(Precedence.Conditional, afterIs: false, whenIsKeyword: true);
            if (!(patternSyntax is ConstantPatternSyntax constantPatternSyntax))
            {
                if (!(patternSyntax is TypePatternSyntax typePatternSyntax))
                {
                    if (patternSyntax is DiscardPatternSyntax discardPatternSyntax)
                    {
                        DiscardPatternSyntax discardPatternSyntax2 = discardPatternSyntax;
                        return _syntaxFactory.IdentifierName(SyntaxParser.ConvertToIdentifier(discardPatternSyntax2.UnderscoreToken));
                    }
                }
                else
                {
                    TypePatternSyntax typePatternSyntax2 = typePatternSyntax;
                    if (ConvertTypeToExpression(typePatternSyntax2.Type, out var expr))
                    {
                        return expr;
                    }
                }
                return patternSyntax;
            }
            return constantPatternSyntax.Expression;
        }

        private bool ConvertTypeToExpression(TypeSyntax type, out ExpressionSyntax expr, bool permitTypeArguments = false)
        {
            expr = null;
            if (!(type is GenericNameSyntax genericNameSyntax))
            {
                if (!(type is SimpleNameSyntax simpleNameSyntax))
                {
                    if (type is QualifiedNameSyntax qualifiedNameSyntax)
                    {
                        NameSyntax left = qualifiedNameSyntax.Left;
                        SyntaxToken dotToken = qualifiedNameSyntax.dotToken;
                        SimpleNameSyntax right = qualifiedNameSyntax.Right;
                        if (permitTypeArguments || !(right is GenericNameSyntax))
                        {
                            ExpressionSyntax expression = (ConvertTypeToExpression(left, out ExpressionSyntax expr2, permitTypeArguments: true) ? expr2 : left);
                            expr = _syntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, dotToken, right);
                            return true;
                        }
                    }
                    return false;
                }
                expr = simpleNameSyntax;
                return true;
            }
            expr = genericNameSyntax;
            return permitTypeArguments;
        }

        private bool LooksLikeTupleArrayType()
        {
            if (base.CurrentToken.Kind != SyntaxKind.OpenParenToken)
            {
                return false;
            }
            ResetPoint state = GetResetPoint();
            try
            {
                return ScanType(forPattern: true) != ScanTypeFlags.NotType;
            }
            finally
            {
                Reset(ref state);
                Release(ref state);
            }
        }

        private PropertyPatternClauseSyntax ParsePropertyPatternClause()
        {
            ParseSubpatternList(out var openToken, out var subPatterns, out var closeToken, SyntaxKind.OpenBraceToken, SyntaxKind.CloseBraceToken);
            return _syntaxFactory.PropertyPatternClause(openToken, subPatterns, closeToken);
        }

        private void ParseSubpatternList(out SyntaxToken openToken, out Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<SubpatternSyntax> subPatterns, out SyntaxToken closeToken, SyntaxKind openKind, SyntaxKind closeKind)
        {
            openToken = EatToken(openKind);
            SeparatedSyntaxListBuilder<SubpatternSyntax> item = _pool.AllocateSeparated<SubpatternSyntax>();
            try
            {
                do
                {
                    if (!IsPossibleSubpatternElement() && base.CurrentToken.Kind != SyntaxKind.CommaToken)
                    {
                        continue;
                    }
                    item.Add(ParseSubpatternElement());
                    int lastTokenPosition = -1;
                    while (IsMakingProgress(ref lastTokenPosition) && base.CurrentToken.Kind != SyntaxKind.CloseParenToken && base.CurrentToken.Kind != SyntaxKind.CloseBraceToken && base.CurrentToken.Kind != SyntaxKind.SemicolonToken)
                    {
                        if (base.CurrentToken.Kind == SyntaxKind.CommaToken || IsPossibleSubpatternElement())
                        {
                            item.AddSeparator(EatToken(SyntaxKind.CommaToken));
                            if (base.CurrentToken.Kind == SyntaxKind.CloseBraceToken)
                            {
                                break;
                            }
                            item.Add(ParseSubpatternElement());
                        }
                        else if (SkipBadPatternListTokens(ref openToken, item, SyntaxKind.CommaToken, closeKind) == PostSkipAction.Abort)
                        {
                            break;
                        }
                    }
                    break;
                }
                while (SkipBadPatternListTokens(ref openToken, item, SyntaxKind.IdentifierToken, closeKind) == PostSkipAction.Continue);
                closeToken = EatToken(closeKind);
                subPatterns = item.ToList();
            }
            finally
            {
                _pool.Free(in item);
            }
        }

        private SubpatternSyntax ParseSubpatternElement()
        {
            NameColonSyntax nameColon = null;
            if (base.CurrentToken.Kind == SyntaxKind.IdentifierToken && PeekToken(1).Kind == SyntaxKind.ColonToken)
            {
                IdentifierNameSyntax name = ParseIdentifierName();
                SyntaxToken colonToken = EatToken(SyntaxKind.ColonToken);
                nameColon = _syntaxFactory.NameColon(name, colonToken);
            }
            PatternSyntax pattern = ParsePattern(Precedence.Conditional);
            return _syntaxFactory.Subpattern(nameColon, pattern);
        }

        private bool IsPossibleSubpatternElement()
        {
            bool flag = IsPossibleExpression(allowBinaryExpressions: false, allowAssignmentExpressions: false, allowAttributes: false);
            if (!flag)
            {
                flag = base.CurrentToken.Kind switch
                {
                    SyntaxKind.OpenBraceToken => true,
                    SyntaxKind.LessThanToken => true,
                    SyntaxKind.LessThanEqualsToken => true,
                    SyntaxKind.GreaterThanToken => true,
                    SyntaxKind.GreaterThanEqualsToken => true,
                    _ => false,
                };
            }
            return flag;
        }

        private PostSkipAction SkipBadPatternListTokens(ref SyntaxToken open, SeparatedSyntaxListBuilder<SubpatternSyntax> list, SyntaxKind expected, SyntaxKind closeKind)
        {
            return SkipBadSeparatedListTokensWithExpectedKind(ref open, list, (LanguageParser p) => p.CurrentToken.Kind != SyntaxKind.CommaToken && !p.IsPossibleSubpatternElement(), (LanguageParser p) => p.CurrentToken.Kind == closeKind || p.CurrentToken.Kind == SyntaxKind.SemicolonToken || p.IsTerminator(), expected);
        }

        private ExpressionSyntax ParseSwitchExpression(ExpressionSyntax governingExpression, SyntaxToken switchKeyword)
        {
            SyntaxToken openBraceToken = EatToken(SyntaxKind.OpenBraceToken);
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<SwitchExpressionArmSyntax> arms = ParseSwitchExpressionArms();
            SyntaxToken closeBraceToken = EatToken(SyntaxKind.CloseBraceToken);
            SwitchExpressionSyntax node = _syntaxFactory.SwitchExpression(governingExpression, switchKeyword, openBraceToken, arms, closeBraceToken);
            return CheckFeatureAvailability(node, MessageID.IDS_FeatureRecursivePatterns);
        }

        private Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<SwitchExpressionArmSyntax> ParseSwitchExpressionArms()
        {
            SeparatedSyntaxListBuilder<SwitchExpressionArmSyntax> builder = _pool.AllocateSeparated<SwitchExpressionArmSyntax>();
            while (base.CurrentToken.Kind != SyntaxKind.CloseBraceToken)
            {
                PatternSyntax pattern = ParsePattern(Precedence.Coalescing, afterIs: false, whenIsKeyword: true);
                WhenClauseSyntax whenClause = ParseWhenClause(Precedence.Coalescing);
                SyntaxToken equalsGreaterThanToken = EatToken(SyntaxKind.EqualsGreaterThanToken);
                ExpressionSyntax expression = ParseExpressionCore();
                SwitchExpressionArmSyntax switchExpressionArmSyntax = _syntaxFactory.SwitchExpressionArm(pattern, whenClause, equalsGreaterThanToken, expression);
                if (switchExpressionArmSyntax.Width == 0 && base.CurrentToken.Kind != SyntaxKind.CommaToken)
                {
                    break;
                }
                builder.Add(switchExpressionArmSyntax);
                if (base.CurrentToken.Kind != SyntaxKind.CloseBraceToken)
                {
                    SyntaxToken separatorToken = ((base.CurrentToken.Kind == SyntaxKind.SemicolonToken) ? EatTokenAsKind(SyntaxKind.CommaToken) : EatToken(SyntaxKind.CommaToken));
                    builder.AddSeparator(separatorToken);
                }
            }
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<SwitchExpressionArmSyntax> result = builder;
            _pool.Free(in builder);
            return result;
        }
    }
}
