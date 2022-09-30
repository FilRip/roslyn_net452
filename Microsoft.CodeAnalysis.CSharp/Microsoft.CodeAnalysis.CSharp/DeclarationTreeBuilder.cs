using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class DeclarationTreeBuilder : CSharpSyntaxVisitor<SingleNamespaceOrTypeDeclaration>
    {
        private readonly SyntaxTree _syntaxTree;

        private readonly string _scriptClassName;

        private readonly bool _isSubmission;

        private static readonly ObjectPool<ImmutableHashSet<string>.Builder> s_memberNameBuilderPool = new ObjectPool<ImmutableHashSet<string>.Builder>(() => ImmutableHashSet.CreateBuilder<string>());

        private DeclarationTreeBuilder(SyntaxTree syntaxTree, string scriptClassName, bool isSubmission)
        {
            _syntaxTree = syntaxTree;
            _scriptClassName = scriptClassName;
            _isSubmission = isSubmission;
        }

        public static RootSingleNamespaceDeclaration ForTree(SyntaxTree syntaxTree, string scriptClassName, bool isSubmission)
        {
            return (RootSingleNamespaceDeclaration)new DeclarationTreeBuilder(syntaxTree, scriptClassName, isSubmission).Visit(syntaxTree.GetRoot());
        }

        private ImmutableArray<SingleNamespaceOrTypeDeclaration> VisitNamespaceChildren(CSharpSyntaxNode node, SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.MemberDeclarationSyntax> internalMembers)
        {
            if (members.Count == 0)
            {
                return ImmutableArray<SingleNamespaceOrTypeDeclaration>.Empty;
            }
            bool flag = false;
            bool flag2 = node.Kind() == SyntaxKind.CompilationUnit && _syntaxTree.Options.Kind == SourceCodeKind.Regular;
            bool flag3 = false;
            bool flag4 = false;
            bool flag5 = false;
            Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax globalStatementSyntax = null;
            ArrayBuilder<SingleNamespaceOrTypeDeclaration> instance = ArrayBuilder<SingleNamespaceOrTypeDeclaration>.GetInstance();
            SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax>.Enumerator enumerator = members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax current = enumerator.Current;
                SingleNamespaceOrTypeDeclaration singleNamespaceOrTypeDeclaration = Visit(current);
                if (singleNamespaceOrTypeDeclaration != null)
                {
                    instance.Add(singleNamespaceOrTypeDeclaration);
                }
                else if (flag2 && current.IsKind(SyntaxKind.GlobalStatement))
                {
                    Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax globalStatementSyntax2 = (Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax)current;
                    if (globalStatementSyntax == null)
                    {
                        globalStatementSyntax = globalStatementSyntax2;
                    }
                    Microsoft.CodeAnalysis.CSharp.Syntax.StatementSyntax statement = globalStatementSyntax2.Statement;
                    if (!flag3)
                    {
                        flag3 = SyntaxFacts.HasAwaitOperations(statement);
                    }
                    if (!flag4)
                    {
                        flag4 = SyntaxFacts.HasYieldOperations(statement);
                    }
                    if (!flag5)
                    {
                        flag5 = SyntaxFacts.HasReturnWithExpression(statement);
                    }
                }
                else if (!flag && current.Kind() != SyntaxKind.IncompleteMember)
                {
                    flag = true;
                }
            }
            if (globalStatementSyntax != null)
            {
                instance.Add(CreateSimpleProgram(globalStatementSyntax, flag3, flag4, flag5));
            }
            if (flag)
            {
                SingleTypeDeclaration.TypeDeclarationFlags declFlags = SingleTypeDeclaration.TypeDeclarationFlags.None;
                ImmutableHashSet<string> nonTypeMemberNames = GetNonTypeMemberNames(internalMembers, ref declFlags, flag2);
                SyntaxReference reference = _syntaxTree.GetReference(node);
                instance.Add(CreateImplicitClass(nonTypeMemberNames, reference, declFlags));
            }
            return instance.ToImmutableAndFree();
        }

        private static SingleNamespaceOrTypeDeclaration CreateImplicitClass(ImmutableHashSet<string> memberNames, SyntaxReference container, SingleTypeDeclaration.TypeDeclarationFlags declFlags)
        {
            return new SingleTypeDeclaration(DeclarationKind.ImplicitClass, "<invalid-global-code>", 0, DeclarationModifiers.Sealed | DeclarationModifiers.Internal | DeclarationModifiers.Partial, declFlags, container, new SourceLocation(container), memberNames, ImmutableArray<SingleTypeDeclaration>.Empty, ImmutableArray<Diagnostic>.Empty);
        }

        private static SingleNamespaceOrTypeDeclaration CreateSimpleProgram(Microsoft.CodeAnalysis.CSharp.Syntax.GlobalStatementSyntax firstGlobalStatement, bool hasAwaitExpressions, bool isIterator, bool hasReturnWithExpression)
        {
            int declFlags = (hasAwaitExpressions ? 64 : 0) | (isIterator ? 128 : 0) | (hasReturnWithExpression ? 256 : 0);
            SyntaxReference reference = firstGlobalStatement.SyntaxTree.GetReference(firstGlobalStatement.Parent);
            SyntaxToken token = firstGlobalStatement.GetFirstToken();
            return new SingleTypeDeclaration(DeclarationKind.SimpleProgram, "<Program>$", 0, DeclarationModifiers.Static | DeclarationModifiers.Internal | DeclarationModifiers.Partial, (SingleTypeDeclaration.TypeDeclarationFlags)declFlags, reference, new SourceLocation(in token), ImmutableHashSet<string>.Empty, ImmutableArray<SingleTypeDeclaration>.Empty, ImmutableArray<Diagnostic>.Empty);
        }

        private RootSingleNamespaceDeclaration CreateScriptRootDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax compilationUnit)
        {
            SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax> members = compilationUnit.Members;
            ArrayBuilder<SingleNamespaceOrTypeDeclaration> instance = ArrayBuilder<SingleNamespaceOrTypeDeclaration>.GetInstance();
            ArrayBuilder<SingleTypeDeclaration> instance2 = ArrayBuilder<SingleTypeDeclaration>.GetInstance();
            SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax>.Enumerator enumerator = members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax current = enumerator.Current;
                SingleNamespaceOrTypeDeclaration singleNamespaceOrTypeDeclaration = Visit(current);
                if (singleNamespaceOrTypeDeclaration != null)
                {
                    if (singleNamespaceOrTypeDeclaration.Kind == DeclarationKind.Namespace)
                    {
                        instance.Add(singleNamespaceOrTypeDeclaration);
                    }
                    else
                    {
                        instance2.Add((SingleTypeDeclaration)singleNamespaceOrTypeDeclaration);
                    }
                }
            }
            SingleTypeDeclaration.TypeDeclarationFlags declFlags = SingleTypeDeclaration.TypeDeclarationFlags.None;
            ImmutableHashSet<string> nonTypeMemberNames = GetNonTypeMemberNames(((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CompilationUnitSyntax)compilationUnit.Green).Members, ref declFlags);
            instance.Add(CreateScriptClass(compilationUnit, instance2.ToImmutableAndFree(), nonTypeMemberNames, declFlags));
            return CreateRootSingleNamespaceDeclaration(compilationUnit, instance.ToImmutableAndFree(), isForScript: true);
        }

        private static ImmutableArray<ReferenceDirective> GetReferenceDirectives(Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax compilationUnit)
        {
            IList<Microsoft.CodeAnalysis.CSharp.Syntax.ReferenceDirectiveTriviaSyntax> referenceDirectives = compilationUnit.GetReferenceDirectives((Microsoft.CodeAnalysis.CSharp.Syntax.ReferenceDirectiveTriviaSyntax d) => !d.File.ContainsDiagnostics && !string.IsNullOrEmpty(d.File.ValueText));
            if (referenceDirectives.Count == 0)
            {
                return ImmutableArray<ReferenceDirective>.Empty;
            }
            ArrayBuilder<ReferenceDirective> instance = ArrayBuilder<ReferenceDirective>.GetInstance(referenceDirectives.Count);
            foreach (Microsoft.CodeAnalysis.CSharp.Syntax.ReferenceDirectiveTriviaSyntax item in referenceDirectives)
            {
                instance.Add(new ReferenceDirective(item.File.ValueText, new SourceLocation(item)));
            }
            return instance.ToImmutableAndFree();
        }

        private SingleNamespaceOrTypeDeclaration CreateScriptClass(Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax parent, ImmutableArray<SingleTypeDeclaration> children, ImmutableHashSet<string> memberNames, SingleTypeDeclaration.TypeDeclarationFlags declFlags)
        {
            SyntaxReference reference = _syntaxTree.GetReference(parent);
            string[] array = _scriptClassName.Split(new char[1] { '.' });
            SingleNamespaceOrTypeDeclaration singleNamespaceOrTypeDeclaration = new SingleTypeDeclaration(_isSubmission ? DeclarationKind.Submission : DeclarationKind.Script, array.Last(), 0, DeclarationModifiers.Sealed | DeclarationModifiers.Internal | DeclarationModifiers.Partial, declFlags, reference, new SourceLocation(reference), memberNames, children, ImmutableArray<Diagnostic>.Empty);
            for (int num = array.Length - 2; num >= 0; num--)
            {
                singleNamespaceOrTypeDeclaration = SingleNamespaceDeclaration.Create(array[num], hasUsings: false, hasExternAliases: false, reference, new SourceLocation(reference), ImmutableArray.Create(singleNamespaceOrTypeDeclaration), ImmutableArray<Diagnostic>.Empty);
            }
            return singleNamespaceOrTypeDeclaration;
        }

        public override SingleNamespaceOrTypeDeclaration VisitCompilationUnit(Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax compilationUnit)
        {
            if (_syntaxTree.Options.Kind != 0)
            {
                return CreateScriptRootDeclaration(compilationUnit);
            }
            ImmutableArray<SingleNamespaceOrTypeDeclaration> children = VisitNamespaceChildren(compilationUnit, compilationUnit.Members, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CompilationUnitSyntax)compilationUnit.Green).Members);
            return CreateRootSingleNamespaceDeclaration(compilationUnit, children, isForScript: false);
        }

        private RootSingleNamespaceDeclaration CreateRootSingleNamespaceDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.CompilationUnitSyntax compilationUnit, ImmutableArray<SingleNamespaceOrTypeDeclaration> children, bool isForScript)
        {
            bool flag = false;
            bool hasGlobalUsings = false;
            bool flag2 = false;
            DiagnosticBag instance = DiagnosticBag.GetInstance();
            SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax>.Enumerator enumerator = compilationUnit.Usings.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax current = enumerator.Current;
                if (current.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword))
                {
                    hasGlobalUsings = true;
                    if (flag && !flag2)
                    {
                        flag2 = true;
                        instance.Add(ErrorCode.ERR_GlobalUsingOutOfOrder, current.GlobalKeyword.GetLocation());
                    }
                }
                else
                {
                    flag = true;
                }
            }
            return new RootSingleNamespaceDeclaration(hasGlobalUsings, flag, compilationUnit.Externs.Any(), _syntaxTree.GetReference(compilationUnit), children, isForScript ? GetReferenceDirectives(compilationUnit) : ImmutableArray<ReferenceDirective>.Empty, compilationUnit.AttributeLists.Any(), instance.ToReadOnlyAndFree());
        }

        public override SingleNamespaceOrTypeDeclaration VisitNamespaceDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.NamespaceDeclarationSyntax node)
        {
            ImmutableArray<SingleNamespaceOrTypeDeclaration> children = VisitNamespaceChildren(node, node.Members, node.Green.Members);
            bool hasUsings = node.Usings.Any();
            bool hasExternAliases = node.Externs.Any();
            Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax nameSyntax = node.Name;
            CSharpSyntaxNode node2 = node;
            while (nameSyntax is Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax qualifiedNameSyntax)
            {
                SingleNamespaceDeclaration singleNamespaceDeclaration = SingleNamespaceDeclaration.Create(qualifiedNameSyntax.Right.Identifier.ValueText, hasUsings, hasExternAliases, _syntaxTree.GetReference(node2), new SourceLocation(qualifiedNameSyntax.Right), children, ImmutableArray<Diagnostic>.Empty);
                SingleNamespaceOrTypeDeclaration[] items = new SingleNamespaceDeclaration[1] { singleNamespaceDeclaration };
                children = items.AsImmutableOrNull();
                node2 = (nameSyntax = qualifiedNameSyntax.Left);
                hasUsings = false;
                hasExternAliases = false;
            }
            DiagnosticBag instance = DiagnosticBag.GetInstance();
            if (ContainsGeneric(node.Name))
            {
                instance.Add(ErrorCode.ERR_UnexpectedGenericName, node.Name.GetLocation());
            }
            if (ContainsAlias(node.Name))
            {
                instance.Add(ErrorCode.ERR_UnexpectedAliasedName, node.Name.GetLocation());
            }
            if (node.AttributeLists.Count > 0)
            {
                instance.Add(ErrorCode.ERR_BadModifiersOnNamespace, node.AttributeLists[0].GetLocation());
            }
            if (node.Modifiers.Count > 0)
            {
                instance.Add(ErrorCode.ERR_BadModifiersOnNamespace, node.Modifiers[0].GetLocation());
            }
            SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax>.Enumerator enumerator = node.Usings.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax current = enumerator.Current;
                if (current.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword))
                {
                    instance.Add(ErrorCode.ERR_GlobalUsingInNamespace, current.GlobalKeyword.GetLocation());
                    break;
                }
            }
            return SingleNamespaceDeclaration.Create(nameSyntax.GetUnqualifiedName().Identifier.ValueText, hasUsings, hasExternAliases, _syntaxTree.GetReference(node2), new SourceLocation(nameSyntax), children, instance.ToReadOnlyAndFree());
        }

        private static bool ContainsAlias(Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name)
        {
            return name.Kind() switch
            {
                SyntaxKind.GenericName => false,
                SyntaxKind.AliasQualifiedName => true,
                SyntaxKind.QualifiedName => ContainsAlias(((Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax)name).Left),
                _ => false,
            };
        }

        private static bool ContainsGeneric(Microsoft.CodeAnalysis.CSharp.Syntax.NameSyntax name)
        {
            switch (name.Kind())
            {
                case SyntaxKind.GenericName:
                    return true;
                case SyntaxKind.AliasQualifiedName:
                    return ContainsGeneric(((Microsoft.CodeAnalysis.CSharp.Syntax.AliasQualifiedNameSyntax)name).Name);
                case SyntaxKind.QualifiedName:
                    {
                        Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax qualifiedNameSyntax = (Microsoft.CodeAnalysis.CSharp.Syntax.QualifiedNameSyntax)name;
                        if (!ContainsGeneric(qualifiedNameSyntax.Left))
                        {
                            return ContainsGeneric(qualifiedNameSyntax.Right);
                        }
                        return true;
                    }
                default:
                    return false;
            }
        }

        public override SingleNamespaceOrTypeDeclaration VisitClassDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.ClassDeclarationSyntax node)
        {
            return VisitTypeDeclaration(node, DeclarationKind.Class);
        }

        public override SingleNamespaceOrTypeDeclaration VisitStructDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.StructDeclarationSyntax node)
        {
            return VisitTypeDeclaration(node, DeclarationKind.Struct);
        }

        public override SingleNamespaceOrTypeDeclaration VisitInterfaceDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.InterfaceDeclarationSyntax node)
        {
            return VisitTypeDeclaration(node, DeclarationKind.Interface);
        }

        public override SingleNamespaceOrTypeDeclaration VisitRecordDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax node)
        {
            return VisitTypeDeclaration(node, node.Kind() switch
            {
                SyntaxKind.RecordDeclaration => DeclarationKind.Record,
                SyntaxKind.RecordStructDeclaration => DeclarationKind.RecordStruct,
                _ => throw ExceptionUtilities.UnexpectedValue(node.Kind()),
            });
        }

        private SingleNamespaceOrTypeDeclaration VisitTypeDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax node, DeclarationKind kind)
        {
            SingleTypeDeclaration.TypeDeclarationFlags declFlags = (node.AttributeLists.Any() ? SingleTypeDeclaration.TypeDeclarationFlags.HasAnyAttributes : SingleTypeDeclaration.TypeDeclarationFlags.None);
            if (node.BaseList != null)
            {
                declFlags |= SingleTypeDeclaration.TypeDeclarationFlags.HasBaseDeclarations;
            }
            DiagnosticBag instance = DiagnosticBag.GetInstance();
            if (node.Arity == 0)
            {
                Symbol.ReportErrorIfHasConstraints(node.ConstraintClauses, instance);
            }
            ImmutableHashSet<string> nonTypeMemberNames = GetNonTypeMemberNames(((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeDeclarationSyntax)node.Green).Members, ref declFlags);
            if ((declFlags & SingleTypeDeclaration.TypeDeclarationFlags.HasAnyNontypeMembers) == 0 && node is Microsoft.CodeAnalysis.CSharp.Syntax.RecordDeclarationSyntax recordDeclarationSyntax && recordDeclarationSyntax.ParameterList != null)
            {
                declFlags |= SingleTypeDeclaration.TypeDeclarationFlags.HasAnyNontypeMembers;
            }
            DeclarationModifiers declarationModifiers = node.Modifiers.ToDeclarationModifiers(instance);
            string valueText = node.Identifier.ValueText;
            DeclarationModifiers modifiers = declarationModifiers;
            int arity = node.Arity;
            SingleTypeDeclaration.TypeDeclarationFlags declFlags2 = declFlags;
            SyntaxReference reference = _syntaxTree.GetReference(node);
            SyntaxToken token = node.Identifier;
            return new SingleTypeDeclaration(kind, valueText, arity, modifiers, declFlags2, reference, new SourceLocation(in token), nonTypeMemberNames, VisitTypeChildren(node), instance.ToReadOnlyAndFree());
        }

        private ImmutableArray<SingleTypeDeclaration> VisitTypeChildren(Microsoft.CodeAnalysis.CSharp.Syntax.TypeDeclarationSyntax node)
        {
            if (node.Members.Count == 0)
            {
                return ImmutableArray<SingleTypeDeclaration>.Empty;
            }
            ArrayBuilder<SingleTypeDeclaration> instance = ArrayBuilder<SingleTypeDeclaration>.GetInstance();
            SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax>.Enumerator enumerator = node.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.MemberDeclarationSyntax current = enumerator.Current;
                if (Visit(current) is SingleTypeDeclaration item)
                {
                    instance.Add(item);
                }
            }
            return instance.ToImmutableAndFree();
        }

        public override SingleNamespaceOrTypeDeclaration VisitDelegateDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.DelegateDeclarationSyntax node)
        {
            SingleTypeDeclaration.TypeDeclarationFlags typeDeclarationFlags = (node.AttributeLists.Any() ? SingleTypeDeclaration.TypeDeclarationFlags.HasAnyAttributes : SingleTypeDeclaration.TypeDeclarationFlags.None);
            DiagnosticBag instance = DiagnosticBag.GetInstance();
            if (node.Arity == 0)
            {
                Symbol.ReportErrorIfHasConstraints(node.ConstraintClauses, instance);
            }
            typeDeclarationFlags |= SingleTypeDeclaration.TypeDeclarationFlags.HasAnyNontypeMembers;
            DeclarationModifiers declarationModifiers = node.Modifiers.ToDeclarationModifiers(instance);
            string valueText = node.Identifier.ValueText;
            DeclarationModifiers modifiers = declarationModifiers;
            SingleTypeDeclaration.TypeDeclarationFlags declFlags = typeDeclarationFlags;
            int arity = node.Arity;
            SyntaxReference reference = _syntaxTree.GetReference(node);
            SyntaxToken token = node.Identifier;
            return new SingleTypeDeclaration(DeclarationKind.Delegate, valueText, arity, modifiers, declFlags, reference, new SourceLocation(in token), ImmutableHashSet<string>.Empty, ImmutableArray<SingleTypeDeclaration>.Empty, instance.ToReadOnlyAndFree());
        }

        public override SingleNamespaceOrTypeDeclaration VisitEnumDeclaration(Microsoft.CodeAnalysis.CSharp.Syntax.EnumDeclarationSyntax node)
        {
            SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax> members = node.Members;
            SingleTypeDeclaration.TypeDeclarationFlags declFlags = (node.AttributeLists.Any() ? SingleTypeDeclaration.TypeDeclarationFlags.HasAnyAttributes : SingleTypeDeclaration.TypeDeclarationFlags.None);
            if (node.BaseList != null)
            {
                declFlags |= SingleTypeDeclaration.TypeDeclarationFlags.HasBaseDeclarations;
            }
            ImmutableHashSet<string> enumMemberNames = GetEnumMemberNames(members, ref declFlags);
            DiagnosticBag instance = DiagnosticBag.GetInstance();
            DeclarationModifiers modifiers = node.Modifiers.ToDeclarationModifiers(instance);
            string valueText = node.Identifier.ValueText;
            SingleTypeDeclaration.TypeDeclarationFlags declFlags2 = declFlags;
            SyntaxReference reference = _syntaxTree.GetReference(node);
            SyntaxToken token = node.Identifier;
            return new SingleTypeDeclaration(DeclarationKind.Enum, valueText, 0, modifiers, declFlags2, reference, new SourceLocation(in token), enumMemberNames, ImmutableArray<SingleTypeDeclaration>.Empty, instance.ToReadOnlyAndFree());
        }

        private static ImmutableHashSet<string> ToImmutableAndFree(ImmutableHashSet<string>.Builder builder)
        {
            ImmutableHashSet<string> result = builder.ToImmutable();
            builder.Clear();
            s_memberNameBuilderPool.Free(builder);
            return result;
        }

        private static ImmutableHashSet<string> GetEnumMemberNames(SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax> members, ref SingleTypeDeclaration.TypeDeclarationFlags declFlags)
        {
            int count = members.Count;
            ImmutableHashSet<string>.Builder builder = s_memberNameBuilderPool.Allocate();
            if (count != 0)
            {
                declFlags |= SingleTypeDeclaration.TypeDeclarationFlags.HasAnyNontypeMembers;
            }
            bool flag = false;
            SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax>.Enumerator enumerator = members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.EnumMemberDeclarationSyntax current = enumerator.Current;
                builder.Add(current.Identifier.ValueText);
                if (!flag && current.AttributeLists.Any())
                {
                    flag = true;
                }
            }
            if (flag)
            {
                declFlags |= SingleTypeDeclaration.TypeDeclarationFlags.AnyMemberHasAttributes;
            }
            return ToImmutableAndFree(builder);
        }

        private static ImmutableHashSet<string> GetNonTypeMemberNames(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.MemberDeclarationSyntax> members, ref SingleTypeDeclaration.TypeDeclarationFlags declFlags, bool skipGlobalStatements = false)
        {
            bool flag = false;
            bool flag2 = false;
            bool anyNonTypeMembers = false;
            ImmutableHashSet<string>.Builder builder = s_memberNameBuilderPool.Allocate();
            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.MemberDeclarationSyntax>.Enumerator enumerator = members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.MemberDeclarationSyntax current = enumerator.Current;
                AddNonTypeMemberNames(current, builder, ref anyNonTypeMembers, skipGlobalStatements);
                if (!flag && CheckMethodMemberForExtensionSyntax(current))
                {
                    flag = true;
                }
                if (!flag2 && CheckMemberForAttributes(current))
                {
                    flag2 = true;
                }
            }
            if (flag)
            {
                declFlags |= SingleTypeDeclaration.TypeDeclarationFlags.AnyMemberHasExtensionMethodSyntax;
            }
            if (flag2)
            {
                declFlags |= SingleTypeDeclaration.TypeDeclarationFlags.AnyMemberHasAttributes;
            }
            if (anyNonTypeMembers)
            {
                declFlags |= SingleTypeDeclaration.TypeDeclarationFlags.HasAnyNontypeMembers;
            }
            return ToImmutableAndFree(builder);
        }

        private static bool CheckMethodMemberForExtensionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode member)
        {
            if (member.Kind == SyntaxKind.MethodDeclaration)
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParameterListSyntax parameterList = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.MethodDeclarationSyntax)member).parameterList;
                if (parameterList != null)
                {
                    Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParameterSyntax> parameters = parameterList.Parameters;
                    if (parameters.Count != 0)
                    {
                        Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken>.Enumerator enumerator = parameters[0]!.Modifiers.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Current.Kind == SyntaxKind.ThisKeyword)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        private static bool CheckMemberForAttributes(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode member)
        {
            switch (member.Kind)
            {
                case SyntaxKind.CompilationUnit:
                    return ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CompilationUnitSyntax)member).AttributeLists.Any();
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                case SyntaxKind.EnumDeclaration:
                case SyntaxKind.RecordDeclaration:
                case SyntaxKind.RecordStructDeclaration:
                    return ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BaseTypeDeclarationSyntax)member).AttributeLists.Any();
                case SyntaxKind.DelegateDeclaration:
                    return ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DelegateDeclarationSyntax)member).AttributeLists.Any();
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                    return ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BaseFieldDeclarationSyntax)member).AttributeLists.Any();
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.DestructorDeclaration:
                    return ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BaseMethodDeclarationSyntax)member).AttributeLists.Any();
                case SyntaxKind.PropertyDeclaration:
                case SyntaxKind.EventDeclaration:
                case SyntaxKind.IndexerDeclaration:
                    {
                        Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BasePropertyDeclarationSyntax basePropertyDeclarationSyntax = (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BasePropertyDeclarationSyntax)member;
                        bool flag = basePropertyDeclarationSyntax.AttributeLists.Any();
                        if (!flag && basePropertyDeclarationSyntax.AccessorList != null)
                        {
                            Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AccessorDeclarationSyntax>.Enumerator enumerator = basePropertyDeclarationSyntax.AccessorList!.Accessors.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AccessorDeclarationSyntax current = enumerator.Current;
                                flag |= current.AttributeLists.Any();
                            }
                        }
                        return flag;
                    }
                default:
                    return false;
            }
        }

        private static void AddNonTypeMemberNames(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode member, ImmutableHashSet<string>.Builder set, ref bool anyNonTypeMembers, bool skipGlobalStatements)
        {
            switch (member.Kind)
            {
                case SyntaxKind.FieldDeclaration:
                    {
                        anyNonTypeMembers = true;
                        Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.VariableDeclaratorSyntax> variables = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.FieldDeclarationSyntax)member).Declaration.Variables;
                        int count = variables.Count;
                        for (int i = 0; i < count; i++)
                        {
                            set.Add(variables[i]!.Identifier.ValueText);
                        }
                        break;
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        anyNonTypeMembers = true;
                        Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.VariableDeclaratorSyntax> variables2 = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.EventFieldDeclarationSyntax)member).Declaration.Variables;
                        int count2 = variables2.Count;
                        for (int j = 0; j < count2; j++)
                        {
                            set.Add(variables2[j]!.Identifier.ValueText);
                        }
                        break;
                    }
                case SyntaxKind.MethodDeclaration:
                    {
                        anyNonTypeMembers = true;
                        Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.MethodDeclarationSyntax methodDeclarationSyntax = (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.MethodDeclarationSyntax)member;
                        if (methodDeclarationSyntax.ExplicitInterfaceSpecifier == null)
                        {
                            set.Add(methodDeclarationSyntax.Identifier.ValueText);
                        }
                        break;
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        anyNonTypeMembers = true;
                        Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PropertyDeclarationSyntax propertyDeclarationSyntax = (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PropertyDeclarationSyntax)member;
                        if (propertyDeclarationSyntax.ExplicitInterfaceSpecifier == null)
                        {
                            set.Add(propertyDeclarationSyntax.Identifier.ValueText);
                        }
                        break;
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        anyNonTypeMembers = true;
                        Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.EventDeclarationSyntax eventDeclarationSyntax = (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.EventDeclarationSyntax)member;
                        if (eventDeclarationSyntax.ExplicitInterfaceSpecifier == null)
                        {
                            set.Add(eventDeclarationSyntax.Identifier.ValueText);
                        }
                        break;
                    }
                case SyntaxKind.ConstructorDeclaration:
                    anyNonTypeMembers = true;
                    set.Add(((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ConstructorDeclarationSyntax)member).Modifiers.Any(8347) ? ".cctor" : ".ctor");
                    break;
                case SyntaxKind.DestructorDeclaration:
                    anyNonTypeMembers = true;
                    set.Add("Finalize");
                    break;
                case SyntaxKind.IndexerDeclaration:
                    anyNonTypeMembers = true;
                    set.Add("this[]");
                    break;
                case SyntaxKind.OperatorDeclaration:
                    {
                        anyNonTypeMembers = true;
                        string item = OperatorFacts.OperatorNameFromDeclaration((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.OperatorDeclarationSyntax)member);
                        set.Add(item);
                        break;
                    }
                case SyntaxKind.ConversionOperatorDeclaration:
                    anyNonTypeMembers = true;
                    set.Add((((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ConversionOperatorDeclarationSyntax)member).ImplicitOrExplicitKeyword.Kind == SyntaxKind.ImplicitKeyword) ? "op_Implicit" : "op_Explicit");
                    break;
                case SyntaxKind.GlobalStatement:
                    if (!skipGlobalStatements)
                    {
                        anyNonTypeMembers = true;
                    }
                    break;
            }
        }
    }
}
