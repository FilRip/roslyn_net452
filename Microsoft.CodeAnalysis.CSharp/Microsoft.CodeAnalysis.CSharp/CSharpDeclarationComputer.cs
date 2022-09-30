using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal class CSharpDeclarationComputer : DeclarationComputer
    {
        public static void ComputeDeclarationsInSpan(SemanticModel model, TextSpan span, bool getSymbol, ArrayBuilder<DeclarationInfo> builder, CancellationToken cancellationToken)
        {
            ComputeDeclarations(model, null, model.SyntaxTree.GetRoot(cancellationToken), (SyntaxNode node, int? level) => !node.Span.OverlapsWith(span) || InvalidLevel(level), getSymbol, builder, null, cancellationToken);
        }

        public static void ComputeDeclarationsInNode(SemanticModel model, ISymbol associatedSymbol, SyntaxNode node, bool getSymbol, ArrayBuilder<DeclarationInfo> builder, CancellationToken cancellationToken, int? levelsToCompute = null)
        {
            ComputeDeclarations(model, associatedSymbol, node, (SyntaxNode n, int? level) => InvalidLevel(level), getSymbol, builder, levelsToCompute, cancellationToken);
        }

        private static bool InvalidLevel(int? level)
        {
            if (level.HasValue)
            {
                return level.Value <= 0;
            }
            return false;
        }

        private static int? DecrementLevel(int? level)
        {
            if (!level.HasValue)
            {
                return level;
            }
            return level - 1;
        }

        private static void ComputeDeclarations(SemanticModel model, ISymbol associatedSymbol, SyntaxNode node, Func<SyntaxNode, int?, bool> shouldSkip, bool getSymbol, ArrayBuilder<DeclarationInfo> builder, int? levelsToCompute, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (shouldSkip(node, levelsToCompute))
            {
                return;
            }
            int? levelsToCompute2 = DecrementLevel(levelsToCompute);
            switch (node.Kind())
            {
                case SyntaxKind.NamespaceDeclaration:
                    {
                        NamespaceDeclarationSyntax namespaceDeclarationSyntax = (NamespaceDeclarationSyntax)node;
                        SyntaxList<MemberDeclarationSyntax>.Enumerator enumerator = namespaceDeclarationSyntax.Members.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            MemberDeclarationSyntax current6 = enumerator.Current;
                            ComputeDeclarations(model, null, current6, shouldSkip, getSymbol, builder, levelsToCompute2, cancellationToken);
                        }
                        DeclarationInfo declarationInfo = DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, cancellationToken);
                        builder.Add(declarationInfo);
                        NameSyntax nameSyntax = namespaceDeclarationSyntax.Name;
                        INamespaceSymbol namespaceSymbol = declarationInfo.DeclaredSymbol as INamespaceSymbol;
                        while (nameSyntax.Kind() == SyntaxKind.QualifiedName)
                        {
                            nameSyntax = ((QualifiedNameSyntax)nameSyntax).Left;
                            INamespaceSymbol namespaceSymbol2 = (getSymbol ? namespaceSymbol?.ContainingNamespace : null);
                            builder.Add(new DeclarationInfo(nameSyntax, ImmutableArray<SyntaxNode>.Empty, namespaceSymbol2));
                            namespaceSymbol = namespaceSymbol2;
                        }
                        break;
                    }
                case SyntaxKind.RecordDeclaration:
                case SyntaxKind.RecordStructDeclaration:
                    if (associatedSymbol is IMethodSymbol)
                    {
                        RecordDeclarationSyntax obj = (RecordDeclarationSyntax)node;
                        IEnumerable<SyntaxNode> enumerable = GetParameterListInitializersAndAttributes(obj.ParameterList);
                        if (obj.BaseList?.Types.FirstOrDefault() is PrimaryConstructorBaseTypeSyntax value)
                        {
                            enumerable = enumerable.Concat(value);
                        }
                        builder.Add(DeclarationComputer.GetDeclarationInfo(node, associatedSymbol, enumerable));
                        break;
                    }
                    goto case SyntaxKind.ClassDeclaration;
                case SyntaxKind.ClassDeclaration:
                case SyntaxKind.StructDeclaration:
                case SyntaxKind.InterfaceDeclaration:
                    {
                        TypeDeclarationSyntax typeDeclarationSyntax = (TypeDeclarationSyntax)node;
                        SyntaxList<MemberDeclarationSyntax>.Enumerator enumerator = typeDeclarationSyntax.Members.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            MemberDeclarationSyntax current5 = enumerator.Current;
                            ComputeDeclarations(model, null, current5, shouldSkip, getSymbol, builder, levelsToCompute2, cancellationToken);
                        }
                        IEnumerable<SyntaxNode> executableCodeBlocks3 = GetAttributes(typeDeclarationSyntax.AttributeLists).Concat(GetTypeParameterListAttributes(typeDeclarationSyntax.TypeParameterList));
                        builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, executableCodeBlocks3, cancellationToken));
                        break;
                    }
                case SyntaxKind.EnumDeclaration:
                    {
                        EnumDeclarationSyntax enumDeclarationSyntax = (EnumDeclarationSyntax)node;
                        SeparatedSyntaxList<EnumMemberDeclarationSyntax>.Enumerator enumerator3 = enumDeclarationSyntax.Members.GetEnumerator();
                        while (enumerator3.MoveNext())
                        {
                            EnumMemberDeclarationSyntax current3 = enumerator3.Current;
                            ComputeDeclarations(model, null, current3, shouldSkip, getSymbol, builder, levelsToCompute2, cancellationToken);
                        }
                        IEnumerable<SyntaxNode> attributes4 = GetAttributes(enumDeclarationSyntax.AttributeLists);
                        builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, attributes4, cancellationToken));
                        break;
                    }
                case SyntaxKind.EnumMemberDeclaration:
                    {
                        EnumMemberDeclarationSyntax obj2 = (EnumMemberDeclarationSyntax)node;
                        IEnumerable<SyntaxNode> executableCodeBlocks = Enumerable.Concat(second: GetAttributes(obj2.AttributeLists), first: SpecializedCollections.SingletonEnumerable(obj2.EqualsValue));
                        builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, executableCodeBlocks, cancellationToken));
                        break;
                    }
                case SyntaxKind.DelegateDeclaration:
                    {
                        DelegateDeclarationSyntax delegateDeclarationSyntax = (DelegateDeclarationSyntax)node;
                        IEnumerable<SyntaxNode> executableCodeBlocks5 = GetAttributes(delegateDeclarationSyntax.AttributeLists).Concat(GetParameterListInitializersAndAttributes(delegateDeclarationSyntax.ParameterList)).Concat(GetTypeParameterListAttributes(delegateDeclarationSyntax.TypeParameterList));
                        builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, executableCodeBlocks5, cancellationToken));
                        break;
                    }
                case SyntaxKind.EventDeclaration:
                    {
                        EventDeclarationSyntax eventDeclarationSyntax = (EventDeclarationSyntax)node;
                        if (eventDeclarationSyntax.AccessorList != null)
                        {
                            SyntaxList<AccessorDeclarationSyntax>.Enumerator enumerator2 = eventDeclarationSyntax.AccessorList!.Accessors.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                AccessorDeclarationSyntax current8 = enumerator2.Current;
                                ComputeDeclarations(model, null, current8, shouldSkip, getSymbol, builder, levelsToCompute2, cancellationToken);
                            }
                        }
                        IEnumerable<SyntaxNode> attributes7 = GetAttributes(eventDeclarationSyntax.AttributeLists);
                        builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, attributes7, cancellationToken));
                        break;
                    }
                case SyntaxKind.FieldDeclaration:
                case SyntaxKind.EventFieldDeclaration:
                    {
                        BaseFieldDeclarationSyntax obj3 = (BaseFieldDeclarationSyntax)node;
                        IEnumerable<SyntaxNode> attributes5 = GetAttributes(obj3.AttributeLists);
                        SeparatedSyntaxList<VariableDeclaratorSyntax>.Enumerator enumerator4 = obj3.Declaration.Variables.GetEnumerator();
                        while (enumerator4.MoveNext())
                        {
                            VariableDeclaratorSyntax current4 = enumerator4.Current;
                            IEnumerable<SyntaxNode> executableCodeBlocks2 = SpecializedCollections.SingletonEnumerable(current4.Initializer).Concat(attributes5);
                            builder.Add(DeclarationComputer.GetDeclarationInfo(model, current4, getSymbol, executableCodeBlocks2, cancellationToken));
                        }
                        break;
                    }
                case SyntaxKind.ArrowExpressionClause:
                    if (node.Parent is BasePropertyDeclarationSyntax declarationWithExpressionBody)
                    {
                        builder.Add(GetExpressionBodyDeclarationInfo(declarationWithExpressionBody, (ArrowExpressionClauseSyntax)node, model, getSymbol, cancellationToken));
                    }
                    break;
                case SyntaxKind.PropertyDeclaration:
                    {
                        PropertyDeclarationSyntax propertyDeclarationSyntax = (PropertyDeclarationSyntax)node;
                        if (propertyDeclarationSyntax.AccessorList != null)
                        {
                            SyntaxList<AccessorDeclarationSyntax>.Enumerator enumerator2 = propertyDeclarationSyntax.AccessorList!.Accessors.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                AccessorDeclarationSyntax current7 = enumerator2.Current;
                                ComputeDeclarations(model, null, current7, shouldSkip, getSymbol, builder, levelsToCompute2, cancellationToken);
                            }
                        }
                        if (propertyDeclarationSyntax.ExpressionBody != null)
                        {
                            ComputeDeclarations(model, null, propertyDeclarationSyntax.ExpressionBody, shouldSkip, getSymbol, builder, levelsToCompute, cancellationToken);
                        }
                        IEnumerable<SyntaxNode> attributes6 = GetAttributes(propertyDeclarationSyntax.AttributeLists);
                        IEnumerable<SyntaxNode> executableCodeBlocks4 = SpecializedCollections.SingletonEnumerable(propertyDeclarationSyntax.Initializer).Concat(attributes6);
                        builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, executableCodeBlocks4, cancellationToken));
                        break;
                    }
                case SyntaxKind.IndexerDeclaration:
                    {
                        IndexerDeclarationSyntax indexerDeclarationSyntax = (IndexerDeclarationSyntax)node;
                        if (indexerDeclarationSyntax.AccessorList != null)
                        {
                            SyntaxList<AccessorDeclarationSyntax>.Enumerator enumerator2 = indexerDeclarationSyntax.AccessorList!.Accessors.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                AccessorDeclarationSyntax current2 = enumerator2.Current;
                                ComputeDeclarations(model, null, current2, shouldSkip, getSymbol, builder, levelsToCompute2, cancellationToken);
                            }
                        }
                        if (indexerDeclarationSyntax.ExpressionBody != null)
                        {
                            ComputeDeclarations(model, null, indexerDeclarationSyntax.ExpressionBody, shouldSkip, getSymbol, builder, levelsToCompute, cancellationToken);
                        }
                        IEnumerable<SyntaxNode> parameterListInitializersAndAttributes = GetParameterListInitializersAndAttributes(indexerDeclarationSyntax.ParameterList);
                        IEnumerable<SyntaxNode> attributes2 = GetAttributes(indexerDeclarationSyntax.AttributeLists);
                        parameterListInitializersAndAttributes = parameterListInitializersAndAttributes.Concat(attributes2);
                        builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, parameterListInitializersAndAttributes, cancellationToken));
                        break;
                    }
                case SyntaxKind.GetAccessorDeclaration:
                case SyntaxKind.SetAccessorDeclaration:
                case SyntaxKind.AddAccessorDeclaration:
                case SyntaxKind.RemoveAccessorDeclaration:
                case SyntaxKind.InitAccessorDeclaration:
                    {
                        AccessorDeclarationSyntax accessorDeclarationSyntax = (AccessorDeclarationSyntax)node;
                        ArrayBuilder<SyntaxNode> instance = ArrayBuilder<SyntaxNode>.GetInstance();
                        instance.AddIfNotNull(accessorDeclarationSyntax.Body);
                        instance.AddIfNotNull(accessorDeclarationSyntax.ExpressionBody);
                        instance.AddRange(GetAttributes(accessorDeclarationSyntax.AttributeLists));
                        builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, instance, cancellationToken));
                        instance.Free();
                        break;
                    }
                case SyntaxKind.MethodDeclaration:
                case SyntaxKind.OperatorDeclaration:
                case SyntaxKind.ConversionOperatorDeclaration:
                case SyntaxKind.ConstructorDeclaration:
                case SyntaxKind.DestructorDeclaration:
                    {
                        BaseMethodDeclarationSyntax baseMethodDeclarationSyntax = (BaseMethodDeclarationSyntax)node;
                        IEnumerable<SyntaxNode> parameterListInitializersAndAttributes2 = GetParameterListInitializersAndAttributes(baseMethodDeclarationSyntax.ParameterList);
                        parameterListInitializersAndAttributes2 = parameterListInitializersAndAttributes2.Concat(baseMethodDeclarationSyntax.Body);
                        if (baseMethodDeclarationSyntax is ConstructorDeclarationSyntax constructorDeclarationSyntax && constructorDeclarationSyntax.Initializer != null)
                        {
                            parameterListInitializersAndAttributes2 = parameterListInitializersAndAttributes2.Concat(constructorDeclarationSyntax.Initializer);
                        }
                        ArrowExpressionClauseSyntax expressionBodySyntax = GetExpressionBodySyntax(baseMethodDeclarationSyntax);
                        if (expressionBodySyntax != null)
                        {
                            parameterListInitializersAndAttributes2 = parameterListInitializersAndAttributes2.Concat(expressionBodySyntax);
                        }
                        parameterListInitializersAndAttributes2 = parameterListInitializersAndAttributes2.Concat(GetAttributes(baseMethodDeclarationSyntax.AttributeLists));
                        if (node is MethodDeclarationSyntax methodDeclarationSyntax && methodDeclarationSyntax.TypeParameterList != null)
                        {
                            parameterListInitializersAndAttributes2 = parameterListInitializersAndAttributes2.Concat(GetTypeParameterListAttributes(methodDeclarationSyntax.TypeParameterList));
                        }
                        builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, parameterListInitializersAndAttributes2, cancellationToken));
                        break;
                    }
                case SyntaxKind.CompilationUnit:
                    {
                        CompilationUnitSyntax compilationUnitSyntax = (CompilationUnitSyntax)node;
                        if (associatedSymbol is IMethodSymbol)
                        {
                            builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol, new CompilationUnitSyntax[1] { compilationUnitSyntax }, cancellationToken));
                            break;
                        }
                        SyntaxList<MemberDeclarationSyntax>.Enumerator enumerator = compilationUnitSyntax.Members.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            MemberDeclarationSyntax current = enumerator.Current;
                            ComputeDeclarations(model, null, current, shouldSkip, getSymbol, builder, levelsToCompute2, cancellationToken);
                        }
                        if (compilationUnitSyntax.AttributeLists.Any())
                        {
                            IEnumerable<SyntaxNode> attributes = GetAttributes(compilationUnitSyntax.AttributeLists);
                            builder.Add(DeclarationComputer.GetDeclarationInfo(model, node, getSymbol: false, attributes, cancellationToken));
                        }
                        break;
                    }
            }
        }

        private static IEnumerable<SyntaxNode> GetAttributes(SyntaxList<AttributeListSyntax> attributeLists)
        {
            SyntaxList<AttributeListSyntax>.Enumerator enumerator = attributeLists.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AttributeListSyntax current = enumerator.Current;
                SeparatedSyntaxList<AttributeSyntax>.Enumerator enumerator2 = current.Attributes.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    yield return enumerator2.Current;
                }
            }
        }

        private static IEnumerable<SyntaxNode> GetParameterListInitializersAndAttributes(BaseParameterListSyntax parameterList)
        {
            if (parameterList == null)
            {
                return SpecializedCollections.EmptyEnumerable<SyntaxNode>();
            }
            return parameterList.Parameters.SelectMany((ParameterSyntax p) => GetParameterInitializersAndAttributes(p));
        }

        private static IEnumerable<SyntaxNode> GetParameterInitializersAndAttributes(ParameterSyntax parameter)
        {
            return SpecializedCollections.SingletonEnumerable(parameter.Default).Concat(GetAttributes(parameter.AttributeLists));
        }

        private static IEnumerable<SyntaxNode> GetTypeParameterListAttributes(TypeParameterListSyntax typeParameterList)
        {
            if (typeParameterList == null)
            {
                return SpecializedCollections.EmptyEnumerable<SyntaxNode>();
            }
            return typeParameterList.Parameters.SelectMany((TypeParameterSyntax p) => GetAttributes(p.AttributeLists));
        }

        private static DeclarationInfo GetExpressionBodyDeclarationInfo(BasePropertyDeclarationSyntax declarationWithExpressionBody, ArrowExpressionClauseSyntax expressionBody, SemanticModel model, bool getSymbol, CancellationToken cancellationToken)
        {
            IMethodSymbol declaredSymbol = ((!getSymbol) ? null : (model.GetDeclaredSymbol(declarationWithExpressionBody, cancellationToken) as IPropertySymbol)?.GetMethod);
            return new DeclarationInfo(expressionBody, ImmutableArray.Create((SyntaxNode)expressionBody), declaredSymbol);
        }

        internal static ArrowExpressionClauseSyntax GetExpressionBodySyntax(CSharpSyntaxNode node)
        {
            ArrowExpressionClauseSyntax result = null;
            switch (node.Kind())
            {
                case SyntaxKind.ArrowExpressionClause:
                    result = (ArrowExpressionClauseSyntax)node;
                    break;
                case SyntaxKind.MethodDeclaration:
                    result = ((MethodDeclarationSyntax)node).ExpressionBody;
                    break;
                case SyntaxKind.OperatorDeclaration:
                    result = ((OperatorDeclarationSyntax)node).ExpressionBody;
                    break;
                case SyntaxKind.ConversionOperatorDeclaration:
                    result = ((ConversionOperatorDeclarationSyntax)node).ExpressionBody;
                    break;
                case SyntaxKind.PropertyDeclaration:
                    result = ((PropertyDeclarationSyntax)node).ExpressionBody;
                    break;
                case SyntaxKind.IndexerDeclaration:
                    result = ((IndexerDeclarationSyntax)node).ExpressionBody;
                    break;
                case SyntaxKind.ConstructorDeclaration:
                    result = ((ConstructorDeclarationSyntax)node).ExpressionBody;
                    break;
                case SyntaxKind.DestructorDeclaration:
                    result = ((DestructorDeclarationSyntax)node).ExpressionBody;
                    break;
                default:
                    ExceptionUtilities.UnexpectedValue(node.Kind());
                    break;
            }
            return result;
        }
    }
}
