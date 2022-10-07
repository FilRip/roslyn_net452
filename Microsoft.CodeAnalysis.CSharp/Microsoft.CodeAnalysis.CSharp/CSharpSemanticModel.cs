using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class CSharpSemanticModel : SemanticModel
    {
        [Flags()]
        public enum SymbolInfoOptions
        {
            PreferTypeToConstructors = 1,
            PreferConstructorsToType = 2,
            ResolveAliases = 4,
            PreserveAliases = 8,
            DefaultOptions = 6
        }

        public new abstract CSharpCompilation Compilation { get; }

        internal new abstract CSharpSyntaxNode Root { get; }

        public new abstract CSharpSemanticModel ParentModel { get; }

        public new abstract SyntaxTree SyntaxTree { get; }

        public sealed override string Language => "C#";

        protected sealed override Compilation CompilationCore => Compilation;

        protected sealed override SemanticModel ParentModelCore => ParentModel;

        protected sealed override SyntaxTree SyntaxTreeCore => SyntaxTree;

        protected sealed override SyntaxNode RootCore => Root;

        internal static bool CanGetSemanticInfo(CSharpSyntaxNode node, bool allowNamedArgumentName = false, bool isSpeculative = false)
        {
            if (!isSpeculative && IsInStructuredTriviaOtherThanCrefOrNameAttribute(node))
            {
                return false;
            }
            switch (node.Kind())
            {
                case SyntaxKind.ObjectInitializerExpression:
                case SyntaxKind.CollectionInitializerExpression:
                    return false;
                case SyntaxKind.ComplexElementInitializerExpression:
                    return false;
                case SyntaxKind.IdentifierName:
                    if (!isSpeculative && node.Parent != null && node.Parent!.Kind() == SyntaxKind.NameEquals && node.Parent!.Parent!.Kind() == SyntaxKind.UsingDirective)
                    {
                        return false;
                    }
                    break;
                case SyntaxKind.OmittedTypeArgument:
                case SyntaxKind.RefExpression:
                case SyntaxKind.RefType:
                    return false;
            }
            if (node.IsMissing)
            {
                return false;
            }
            if ((!(node is ExpressionSyntax) || (!(isSpeculative || allowNamedArgumentName) && SyntaxFacts.IsNamedArgumentName(node))) && !(node is ConstructorInitializerSyntax) && !(node is PrimaryConstructorBaseTypeSyntax) && !(node is AttributeSyntax))
            {
                return node is CrefSyntax;
            }
            return true;
        }

        internal abstract SymbolInfo GetSymbolInfoWorker(CSharpSyntaxNode node, SymbolInfoOptions options, CancellationToken cancellationToken = default(CancellationToken));

        internal abstract SymbolInfo GetCollectionInitializerSymbolInfoWorker(InitializerExpressionSyntax collectionInitializer, ExpressionSyntax node, CancellationToken cancellationToken = default(CancellationToken));

        internal abstract CSharpTypeInfo GetTypeInfoWorker(CSharpSyntaxNode node, CancellationToken cancellationToken = default(CancellationToken));

        internal abstract BoundExpression GetSpeculativelyBoundExpression(int position, ExpressionSyntax expression, SpeculativeBindingOption bindingOption, out Binder binder, out ImmutableArray<Symbol> crefSymbols);

        internal abstract ImmutableArray<Symbol> GetMemberGroupWorker(CSharpSyntaxNode node, SymbolInfoOptions options, CancellationToken cancellationToken = default(CancellationToken));

        internal abstract ImmutableArray<IPropertySymbol> GetIndexerGroupWorker(CSharpSyntaxNode node, SymbolInfoOptions options, CancellationToken cancellationToken = default(CancellationToken));

        internal abstract Optional<object> GetConstantValueWorker(CSharpSyntaxNode node, CancellationToken cancellationToken = default(CancellationToken));

        internal Binder GetSpeculativeBinder(int position, ExpressionSyntax expression, SpeculativeBindingOption bindingOption)
        {
            position = CheckAndAdjustPosition(position);
            if (bindingOption == SpeculativeBindingOption.BindAsTypeOrNamespace && !(expression is TypeSyntax))
            {
                return null;
            }
            Binder binder = GetEnclosingBinder(position);
            if (binder == null)
            {
                return null;
            }
            if (bindingOption == SpeculativeBindingOption.BindAsTypeOrNamespace && IsInTypeofExpression(position))
            {
                binder = new TypeofBinder(expression, binder);
            }
            binder = new WithNullableContextBinder(SyntaxTree, position, binder);
            return new ExecutableCodeBinder(expression, binder.ContainingMemberOrLambda, binder).GetBinder(expression);
        }

        private Binder GetSpeculativeBinderForAttribute(int position, AttributeSyntax attribute)
        {
            position = CheckAndAdjustPositionForSpeculativeAttribute(position);
            Binder enclosingBinder = GetEnclosingBinder(position);
            if (enclosingBinder == null)
            {
                return null;
            }
            return new ExecutableCodeBinder(attribute, enclosingBinder.ContainingMemberOrLambda, enclosingBinder).GetBinder(attribute);
        }

        private static BoundExpression GetSpeculativelyBoundExpressionHelper(Binder binder, ExpressionSyntax expression, SpeculativeBindingOption bindingOption)
        {
            if (bindingOption == SpeculativeBindingOption.BindAsTypeOrNamespace || binder.Flags.Includes(BinderFlags.CrefParameterOrReturnType))
            {
                return binder.BindNamespaceOrType(expression, BindingDiagnosticBag.Discarded);
            }
            return binder.BindExpression(expression, BindingDiagnosticBag.Discarded);
        }

        protected BoundExpression GetSpeculativelyBoundExpressionWithoutNullability(int position, ExpressionSyntax expression, SpeculativeBindingOption bindingOption, out Binder binder, out ImmutableArray<Symbol> crefSymbols)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            crefSymbols = default(ImmutableArray<Symbol>);
            expression = SyntaxFactory.GetStandaloneExpression(expression);
            binder = GetSpeculativeBinder(position, expression, bindingOption);
            if (binder == null)
            {
                return null;
            }
            if (binder.Flags.Includes(BinderFlags.CrefParameterOrReturnType))
            {
                crefSymbols = ImmutableArray.Create((Symbol)binder.BindType(expression, BindingDiagnosticBag.Discarded).Type);
                return null;
            }
            if (binder.InCref)
            {
                if (expression.IsKind(SyntaxKind.QualifiedName))
                {
                    QualifiedNameSyntax qualifiedNameSyntax = (QualifiedNameSyntax)expression;
                    QualifiedCrefSyntax crefSyntax = SyntaxFactory.QualifiedCref(qualifiedNameSyntax.Left, SyntaxFactory.NameMemberCref(qualifiedNameSyntax.Right));
                    crefSymbols = BindCref(crefSyntax, binder);
                }
                else if (expression is TypeSyntax typeSyntax)
                {
                    CrefSyntax crefSyntax2 = ((typeSyntax is PredefinedTypeSyntax) ? SyntaxFactory.TypeCref(typeSyntax) : ((CrefSyntax)SyntaxFactory.NameMemberCref(typeSyntax)));
                    crefSymbols = BindCref(crefSyntax2, binder);
                }
                return null;
            }
            return GetSpeculativelyBoundExpressionHelper(binder, expression, bindingOption);
        }

        internal static ImmutableArray<Symbol> BindCref(CrefSyntax crefSyntax, Binder binder)
        {
            return binder.BindCref(crefSyntax, out Symbol ambiguityWinner, BindingDiagnosticBag.Discarded);
        }

        internal SymbolInfo GetCrefSymbolInfo(int position, CrefSyntax crefSyntax, SymbolInfoOptions options, bool hasParameterList)
        {
            Binder enclosingBinder = GetEnclosingBinder(position);
            if (enclosingBinder != null && enclosingBinder.InCref)
            {
                return GetCrefSymbolInfo(BindCref(crefSyntax, enclosingBinder), options, hasParameterList);
            }
            return SymbolInfo.None;
        }

        internal static bool HasParameterList(CrefSyntax crefSyntax)
        {
            while (crefSyntax.Kind() == SyntaxKind.QualifiedCref)
            {
                crefSyntax = ((QualifiedCrefSyntax)crefSyntax).Member;
            }
            return crefSyntax.Kind() switch
            {
                SyntaxKind.NameMemberCref => ((NameMemberCrefSyntax)crefSyntax).Parameters != null,
                SyntaxKind.IndexerMemberCref => ((IndexerMemberCrefSyntax)crefSyntax).Parameters != null,
                SyntaxKind.OperatorMemberCref => ((OperatorMemberCrefSyntax)crefSyntax).Parameters != null,
                SyntaxKind.ConversionOperatorMemberCref => ((ConversionOperatorMemberCrefSyntax)crefSyntax).Parameters != null,
                _ => false,
            };
        }

        private static SymbolInfo GetCrefSymbolInfo(ImmutableArray<Symbol> symbols, SymbolInfoOptions options, bool hasParameterList)
        {
            switch (symbols.Length)
            {
                case 0:
                    return SymbolInfo.None;
                case 1:
                    return GetSymbolInfoForSymbol(symbols[0], options);
                default:
                    {
                        if ((options & SymbolInfoOptions.ResolveAliases) == SymbolInfoOptions.ResolveAliases)
                        {
                            symbols = UnwrapAliases(symbols);
                        }
                        LookupResultKind resultKind = LookupResultKind.Ambiguous;
                        SymbolKind firstCandidateKind = symbols[0].Kind;
                        if (hasParameterList && symbols.All((Symbol s) => s.Kind == firstCandidateKind))
                        {
                            resultKind = LookupResultKind.OverloadResolutionFailure;
                        }
                        return SymbolInfoFactory.Create(symbols, resultKind, isDynamic: false);
                    }
            }
        }

        private BoundAttribute GetSpeculativelyBoundAttribute(int position, AttributeSyntax attribute, out Binder binder)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException("attribute");
            }
            binder = GetSpeculativeBinderForAttribute(position, attribute);
            if (binder == null)
            {
                return null;
            }
            Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol attributeType = (Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol)binder.BindType(attribute.Name, BindingDiagnosticBag.Discarded, out Symbols.AliasSymbol alias).Type;
            return new ExecutableCodeBinder(attribute, binder.ContainingMemberOrLambda, binder).BindAttribute(attribute, attributeType, BindingDiagnosticBag.Discarded);
        }

        private int CheckAndAdjustPositionForSpeculativeAttribute(int position)
        {
            position = CheckAndAdjustPosition(position);
            SyntaxToken syntaxToken = Root.FindToken(position);
            if (position == 0 && position != syntaxToken.SpanStart)
            {
                return position;
            }
            CSharpSyntaxNode cSharpSyntaxNode = (CSharpSyntaxNode)syntaxToken.Parent;
            if (position == cSharpSyntaxNode.SpanStart)
            {
                if (cSharpSyntaxNode is BaseTypeDeclarationSyntax baseTypeDeclarationSyntax)
                {
                    position = baseTypeDeclarationSyntax.OpenBraceToken.SpanStart;
                }
                MethodDeclarationSyntax methodDeclarationSyntax = cSharpSyntaxNode.FirstAncestorOrSelf<MethodDeclarationSyntax>();
                if (methodDeclarationSyntax != null && methodDeclarationSyntax.SpanStart == position)
                {
                    position = methodDeclarationSyntax.Identifier.SpanStart;
                }
            }
            return position;
        }

        protected override IOperation GetOperationCore(SyntaxNode node, CancellationToken cancellationToken)
        {
            CSharpSyntaxNode cSharpSyntaxNode = (CSharpSyntaxNode)node;
            CheckSyntaxNode(cSharpSyntaxNode);
            return GetOperationWorker(cSharpSyntaxNode, cancellationToken);
        }

        internal virtual IOperation GetOperationWorker(CSharpSyntaxNode node, CancellationToken cancellationToken)
        {
            return null;
        }

        public abstract SymbolInfo GetSymbolInfo(OrderingSyntax node, CancellationToken cancellationToken = default(CancellationToken));

        public abstract SymbolInfo GetSymbolInfo(SelectOrGroupClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken));

        public SymbolInfo GetSymbolInfo(PositionalPatternClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(node);
            return GetSymbolInfoWorker(node, SymbolInfoOptions.DefaultOptions, cancellationToken);
        }

        public SymbolInfo GetSymbolInfo(ExpressionSyntax expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(expression);
            if (!CanGetSemanticInfo(expression, allowNamedArgumentName: true))
            {
                return SymbolInfo.None;
            }
            if (SyntaxFacts.IsNamedArgumentName(expression))
            {
                return GetNamedArgumentSymbolInfo((IdentifierNameSyntax)expression, cancellationToken);
            }
            if (SyntaxFacts.IsDeclarationExpressionType(expression, out var parent))
            {
                switch (parent.Designation.Kind())
                {
                    case SyntaxKind.SingleVariableDesignation:
                        return GetSymbolInfoFromSymbolOrNone(TypeFromVariable((SingleVariableDesignationSyntax)parent.Designation, cancellationToken).Type);
                    case SyntaxKind.DiscardDesignation:
                        return GetSymbolInfoFromSymbolOrNone(GetTypeInfoWorker(parent, cancellationToken).Type.GetPublicSymbol());
                    case SyntaxKind.ParenthesizedVariableDesignation:
                        if (((TypeSyntax)expression).IsVar)
                        {
                            return SymbolInfo.None;
                        }
                        break;
                }
            }
            else if (expression is DeclarationExpressionSyntax declarationExpressionSyntax)
            {
                if (declarationExpressionSyntax.Designation.Kind() != SyntaxKind.SingleVariableDesignation)
                {
                    return SymbolInfo.None;
                }
                ISymbol declaredSymbol = GetDeclaredSymbol((SingleVariableDesignationSyntax)declarationExpressionSyntax.Designation, cancellationToken);
                if (declaredSymbol == null)
                {
                    return SymbolInfo.None;
                }
                return new SymbolInfo(declaredSymbol);
            }
            return GetSymbolInfoWorker(expression, SymbolInfoOptions.DefaultOptions, cancellationToken);
        }

        private static SymbolInfo GetSymbolInfoFromSymbolOrNone(ITypeSymbol type)
        {
            if (type == null || type.Kind != SymbolKind.ErrorType)
            {
                return new SymbolInfo(type);
            }
            return SymbolInfo.None;
        }

        private (ITypeSymbol Type, Microsoft.CodeAnalysis.NullableAnnotation Annotation) TypeFromVariable(SingleVariableDesignationSyntax variableDesignation, CancellationToken cancellationToken)
        {
            ISymbol declaredSymbol = GetDeclaredSymbol(variableDesignation, cancellationToken);
            if (!(declaredSymbol is ILocalSymbol localSymbol))
            {
                if (declaredSymbol is IFieldSymbol fieldSymbol)
                {
                    return (fieldSymbol.Type, fieldSymbol.NullableAnnotation);
                }
                return default((ITypeSymbol, Microsoft.CodeAnalysis.NullableAnnotation));
            }
            return (localSymbol.Type, localSymbol.NullableAnnotation);
        }

        public SymbolInfo GetCollectionInitializerSymbolInfo(ExpressionSyntax expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(expression);
            if (expression.Parent != null && expression.Parent!.Kind() == SyntaxKind.CollectionInitializerExpression)
            {
                InitializerExpressionSyntax initializerExpressionSyntax = (InitializerExpressionSyntax)expression.Parent;
                while (initializerExpressionSyntax.Parent != null && initializerExpressionSyntax.Parent!.Kind() == SyntaxKind.SimpleAssignmentExpression && ((AssignmentExpressionSyntax)initializerExpressionSyntax.Parent).Right == initializerExpressionSyntax && initializerExpressionSyntax.Parent!.Parent != null && initializerExpressionSyntax.Parent!.Parent!.Kind() == SyntaxKind.ObjectInitializerExpression)
                {
                    initializerExpressionSyntax = (InitializerExpressionSyntax)initializerExpressionSyntax.Parent!.Parent;
                }
                if (initializerExpressionSyntax.Parent is BaseObjectCreationExpressionSyntax baseObjectCreationExpressionSyntax && baseObjectCreationExpressionSyntax.Initializer == initializerExpressionSyntax && CanGetSemanticInfo(baseObjectCreationExpressionSyntax))
                {
                    return GetCollectionInitializerSymbolInfoWorker((InitializerExpressionSyntax)expression.Parent, expression, cancellationToken);
                }
            }
            return SymbolInfo.None;
        }

        public SymbolInfo GetSymbolInfo(ConstructorInitializerSyntax constructorInitializer, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(constructorInitializer);
            if (!CanGetSemanticInfo(constructorInitializer))
            {
                return SymbolInfo.None;
            }
            return GetSymbolInfoWorker(constructorInitializer, SymbolInfoOptions.DefaultOptions, cancellationToken);
        }

        public SymbolInfo GetSymbolInfo(PrimaryConstructorBaseTypeSyntax constructorInitializer, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(constructorInitializer);
            if (!CanGetSemanticInfo(constructorInitializer))
            {
                return SymbolInfo.None;
            }
            return GetSymbolInfoWorker(constructorInitializer, SymbolInfoOptions.DefaultOptions, cancellationToken);
        }

        public SymbolInfo GetSymbolInfo(AttributeSyntax attributeSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(attributeSyntax);
            if (!CanGetSemanticInfo(attributeSyntax))
            {
                return SymbolInfo.None;
            }
            return GetSymbolInfoWorker(attributeSyntax, SymbolInfoOptions.DefaultOptions, cancellationToken);
        }

        public SymbolInfo GetSymbolInfo(CrefSyntax crefSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(crefSyntax);
            if (!CanGetSemanticInfo(crefSyntax))
            {
                return SymbolInfo.None;
            }
            return GetSymbolInfoWorker(crefSyntax, SymbolInfoOptions.DefaultOptions, cancellationToken);
        }

        public SymbolInfo GetSpeculativeSymbolInfo(int position, ExpressionSyntax expression, SpeculativeBindingOption bindingOption)
        {
            if (!CanGetSemanticInfo(expression, allowNamedArgumentName: false, isSpeculative: true))
            {
                return SymbolInfo.None;
            }
            BoundNode speculativelyBoundExpression = GetSpeculativelyBoundExpression(position, expression, bindingOption, out Binder binder, out ImmutableArray<Symbol> crefSymbols);
            if (speculativelyBoundExpression == null)
            {
                if (!crefSymbols.IsDefault)
                {
                    return GetCrefSymbolInfo(crefSymbols, SymbolInfoOptions.DefaultOptions, hasParameterList: false);
                }
                return SymbolInfo.None;
            }
            return GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, speculativelyBoundExpression, speculativelyBoundExpression, null, binder);
        }

        public SymbolInfo GetSpeculativeSymbolInfo(int position, AttributeSyntax attribute)
        {
            BoundNode speculativelyBoundAttribute = GetSpeculativelyBoundAttribute(position, attribute, out Binder binder);
            if (speculativelyBoundAttribute == null)
            {
                return SymbolInfo.None;
            }
            return GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, speculativelyBoundAttribute, speculativelyBoundAttribute, null, binder);
        }

        public SymbolInfo GetSpeculativeSymbolInfo(int position, ConstructorInitializerSyntax constructorInitializer)
        {
            position = CheckAndAdjustPosition(position);
            if (constructorInitializer == null)
            {
                throw new ArgumentNullException("constructorInitializer");
            }
            ConstructorInitializerSyntax constructorInitializerSyntax = Root.FindToken(position).Parent!.AncestorsAndSelf().OfType<ConstructorInitializerSyntax>().FirstOrDefault();
            if (constructorInitializerSyntax == null)
            {
                return SymbolInfo.None;
            }
            MemberSemanticModel memberModel = GetMemberModel(constructorInitializerSyntax);
            if (memberModel == null)
            {
                return SymbolInfo.None;
            }
            Binder enclosingBinder = memberModel.GetEnclosingBinder(position);
            if (enclosingBinder != null)
            {
                enclosingBinder = new ExecutableCodeBinder(constructorInitializer, enclosingBinder.ContainingMemberOrLambda, enclosingBinder);
                BoundExpressionStatement bnode = enclosingBinder.BindConstructorInitializer(constructorInitializer, BindingDiagnosticBag.Discarded);
                return GetSymbolInfoFromBoundConstructorInitializer(memberModel, enclosingBinder, bnode);
            }
            return SymbolInfo.None;
        }

        private static SymbolInfo GetSymbolInfoFromBoundConstructorInitializer(MemberSemanticModel memberModel, Binder binder, BoundExpressionStatement bnode)
        {
            BoundExpression boundExpression;
            for (boundExpression = bnode.Expression; boundExpression is BoundSequence boundSequence; boundExpression = boundSequence.Value)
            {
            }
            return memberModel.GetSymbolInfoForNode(SymbolInfoOptions.DefaultOptions, boundExpression, boundExpression, null, binder);
        }

        public SymbolInfo GetSpeculativeSymbolInfo(int position, PrimaryConstructorBaseTypeSyntax constructorInitializer)
        {
            position = CheckAndAdjustPosition(position);
            if (constructorInitializer == null)
            {
                throw new ArgumentNullException("constructorInitializer");
            }
            PrimaryConstructorBaseTypeSyntax primaryConstructorBaseTypeSyntax = Root.FindToken(position).Parent!.AncestorsAndSelf().OfType<PrimaryConstructorBaseTypeSyntax>().FirstOrDefault();
            if (primaryConstructorBaseTypeSyntax == null)
            {
                return SymbolInfo.None;
            }
            MemberSemanticModel memberModel = GetMemberModel(primaryConstructorBaseTypeSyntax);
            if (memberModel == null)
            {
                return SymbolInfo.None;
            }
            ArgumentListSyntax argumentList = primaryConstructorBaseTypeSyntax.ArgumentList;
            Binder enclosingBinder = memberModel.GetEnclosingBinder(LookupPosition.IsBetweenTokens(position, argumentList.OpenParenToken, argumentList.CloseParenToken) ? position : argumentList.OpenParenToken.SpanStart);
            if (enclosingBinder != null)
            {
                enclosingBinder = new ExecutableCodeBinder(constructorInitializer, enclosingBinder.ContainingMemberOrLambda, enclosingBinder);
                BoundExpressionStatement bnode = enclosingBinder.BindConstructorInitializer(constructorInitializer, BindingDiagnosticBag.Discarded);
                return GetSymbolInfoFromBoundConstructorInitializer(memberModel, enclosingBinder, bnode);
            }
            return SymbolInfo.None;
        }

        public SymbolInfo GetSpeculativeSymbolInfo(int position, CrefSyntax cref, SymbolInfoOptions options = SymbolInfoOptions.DefaultOptions)
        {
            position = CheckAndAdjustPosition(position);
            return GetCrefSymbolInfo(position, cref, options, HasParameterList(cref));
        }

        public TypeInfo GetTypeInfo(ConstructorInitializerSyntax constructorInitializer, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(constructorInitializer);
            return CanGetSemanticInfo(constructorInitializer) ? GetTypeInfoWorker(constructorInitializer, cancellationToken) : CSharpTypeInfo.None;
        }

        public abstract TypeInfo GetTypeInfo(SelectOrGroupClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken));

        public TypeInfo GetTypeInfo(PatternSyntax pattern, CancellationToken cancellationToken = default(CancellationToken))
        {
            while (pattern is ParenthesizedPatternSyntax parenthesizedPatternSyntax)
            {
                pattern = parenthesizedPatternSyntax.Pattern;
            }
            CheckSyntaxNode(pattern);
            return GetTypeInfoWorker(pattern, cancellationToken);
        }

        public TypeInfo GetTypeInfo(ExpressionSyntax expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(expression);
            if (!CanGetSemanticInfo(expression))
            {
                return CSharpTypeInfo.None;
            }
            if (SyntaxFacts.IsDeclarationExpressionType(expression, out var parent))
            {
                switch (parent.Designation.Kind())
                {
                    case SyntaxKind.SingleVariableDesignation:
                        {
                            (ITypeSymbol Type, Microsoft.CodeAnalysis.NullableAnnotation Annotation) tuple = TypeFromVariable((SingleVariableDesignationSyntax)parent.Designation, cancellationToken);
                            ITypeSymbol item = tuple.Type;
                            Microsoft.CodeAnalysis.NullableAnnotation item2 = tuple.Annotation;
                            Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol symbol = item.GetSymbol();
                            NullabilityInfo nullabilityInfo = item2.ToNullabilityInfo(symbol);
                            return new CSharpTypeInfo(symbol, symbol, nullabilityInfo, nullabilityInfo, Conversion.Identity);
                        }
                    case SyntaxKind.DiscardDesignation:
                        {
                            CSharpTypeInfo typeInfoWorker = GetTypeInfoWorker(parent, cancellationToken);
                            return new CSharpTypeInfo(typeInfoWorker.Type, typeInfoWorker.Type, typeInfoWorker.Nullability, typeInfoWorker.Nullability, Conversion.Identity);
                        }
                    case SyntaxKind.ParenthesizedVariableDesignation:
                        if (((TypeSyntax)expression).IsVar)
                        {
                            return CSharpTypeInfo.None;
                        }
                        break;
                }
            }
            return GetTypeInfoWorker(expression, cancellationToken);
        }

        public TypeInfo GetTypeInfo(AttributeSyntax attributeSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(attributeSyntax);
            return CanGetSemanticInfo(attributeSyntax) ? GetTypeInfoWorker(attributeSyntax, cancellationToken) : CSharpTypeInfo.None;
        }

        public Conversion GetConversion(SyntaxNode expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            CSharpSyntaxNode cSharpSyntaxNode = (CSharpSyntaxNode)expression;
            CheckSyntaxNode(cSharpSyntaxNode);
            CSharpTypeInfo obj = (CanGetSemanticInfo(cSharpSyntaxNode) ? GetTypeInfoWorker(cSharpSyntaxNode, cancellationToken) : CSharpTypeInfo.None);
            return obj.ImplicitConversion;
        }

        public TypeInfo GetSpeculativeTypeInfo(int position, ExpressionSyntax expression, SpeculativeBindingOption bindingOption)
        {
            return GetSpeculativeTypeInfoWorker(position, expression, bindingOption);
        }

        internal CSharpTypeInfo GetSpeculativeTypeInfoWorker(int position, ExpressionSyntax expression, SpeculativeBindingOption bindingOption)
        {
            if (!CanGetSemanticInfo(expression, allowNamedArgumentName: false, isSpeculative: true))
            {
                return CSharpTypeInfo.None;
            }
            BoundNode speculativelyBoundExpression = GetSpeculativelyBoundExpression(position, expression, bindingOption, out Binder binder, out ImmutableArray<Symbol> crefSymbols);
            if (speculativelyBoundExpression == null)
            {
                if (crefSymbols.IsDefault || crefSymbols.Length != 1)
                {
                    return CSharpTypeInfo.None;
                }
                return GetTypeInfoForSymbol(crefSymbols[0]);
            }
            return GetTypeInfoForNode(speculativelyBoundExpression, speculativelyBoundExpression, null);
        }

        public Conversion GetSpeculativeConversion(int position, ExpressionSyntax expression, SpeculativeBindingOption bindingOption)
        {
            return GetSpeculativeTypeInfoWorker(position, expression, bindingOption).ImplicitConversion;
        }

        public ImmutableArray<ISymbol> GetMemberGroup(ExpressionSyntax expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(expression);
            if (!CanGetSemanticInfo(expression))
            {
                return ImmutableArray<ISymbol>.Empty;
            }
            return GetMemberGroupWorker(expression, SymbolInfoOptions.DefaultOptions, cancellationToken).GetPublicSymbols();
        }

        public ImmutableArray<ISymbol> GetMemberGroup(AttributeSyntax attribute, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(attribute);
            if (!CanGetSemanticInfo(attribute))
            {
                return ImmutableArray<ISymbol>.Empty;
            }
            return GetMemberGroupWorker(attribute, SymbolInfoOptions.DefaultOptions, cancellationToken).GetPublicSymbols();
        }

        public ImmutableArray<ISymbol> GetMemberGroup(ConstructorInitializerSyntax initializer, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(initializer);
            if (!CanGetSemanticInfo(initializer))
            {
                return ImmutableArray<ISymbol>.Empty;
            }
            return GetMemberGroupWorker(initializer, SymbolInfoOptions.DefaultOptions, cancellationToken).GetPublicSymbols();
        }

        public ImmutableArray<IPropertySymbol> GetIndexerGroup(ExpressionSyntax expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(expression);
            if (!CanGetSemanticInfo(expression))
            {
                return ImmutableArray<IPropertySymbol>.Empty;
            }
            return GetIndexerGroupWorker(expression, SymbolInfoOptions.DefaultOptions, cancellationToken);
        }

        public Optional<object> GetConstantValue(ExpressionSyntax expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(expression);
            if (!CanGetSemanticInfo(expression))
            {
                return default(Optional<object>);
            }
            return GetConstantValueWorker(expression, cancellationToken);
        }

        public abstract QueryClauseInfo GetQueryClauseInfo(QueryClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken));

        public IAliasSymbol GetAliasInfo(IdentifierNameSyntax nameSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(nameSyntax);
            if (!CanGetSemanticInfo(nameSyntax))
            {
                return null;
            }
            return GetSymbolInfoWorker(nameSyntax, SymbolInfoOptions.PreferTypeToConstructors | SymbolInfoOptions.PreserveAliases, cancellationToken).Symbol as IAliasSymbol;
        }

        public IAliasSymbol GetSpeculativeAliasInfo(int position, IdentifierNameSyntax nameSyntax, SpeculativeBindingOption bindingOption)
        {
            BoundNode speculativelyBoundExpression = GetSpeculativelyBoundExpression(position, nameSyntax, bindingOption, out Binder binder, out ImmutableArray<Symbol> crefSymbols);
            if (speculativelyBoundExpression == null)
            {
                if (crefSymbols.IsDefault || crefSymbols.Length != 1)
                {
                    return null;
                }
                return (crefSymbols[0] as Microsoft.CodeAnalysis.CSharp.Symbols.AliasSymbol).GetPublicSymbol();
            }
            return GetSymbolInfoForNode(SymbolInfoOptions.PreferTypeToConstructors | SymbolInfoOptions.PreserveAliases, speculativelyBoundExpression, speculativelyBoundExpression, null, binder).Symbol as IAliasSymbol;
        }

        internal Binder GetEnclosingBinder(int position)
        {
            return GetEnclosingBinderInternal(position);
        }

        internal abstract Binder GetEnclosingBinderInternal(int position);

        internal abstract MemberSemanticModel GetMemberModel(SyntaxNode node);

        internal bool IsInTree(SyntaxNode node)
        {
            return node.SyntaxTree == SyntaxTree;
        }

        private static bool IsInStructuredTriviaOtherThanCrefOrNameAttribute(CSharpSyntaxNode node)
        {
            while (node != null)
            {
                if (node.Kind() == SyntaxKind.XmlCrefAttribute || node.Kind() == SyntaxKind.XmlNameAttribute)
                {
                    return false;
                }
                if (node.IsStructuredTrivia)
                {
                    return true;
                }
                node = node.ParentOrStructuredTriviaParent;
            }
            return false;
        }

        protected int CheckAndAdjustPosition(int position)
        {
            return CheckAndAdjustPosition(position, out SyntaxToken token);
        }

        protected int CheckAndAdjustPosition(int position, out SyntaxToken token)
        {
            int position2 = Root.Position;
            int end = Root.FullSpan.End;
            bool flag = position == end && position == SyntaxTree.GetRoot().FullSpan.End;
            if ((position2 <= position && position < end) || flag)
            {
                token = (flag ? ((CSharpSyntaxNode)SyntaxTree.GetRoot()) : Root).FindTokenIncludingCrefAndNameAttributes(position);
                if (position < token.SpanStart)
                {
                    token = token.GetPreviousToken();
                }
                return Math.Max(token.SpanStart, position2);
            }
            if (position2 == end && position == end)
            {
                token = default(SyntaxToken);
                return position2;
            }
            throw new ArgumentOutOfRangeException("position", position, string.Format(CSharpResources.PositionIsNotWithinSyntax, Root.FullSpan));
        }

        protected int GetAdjustedNodePosition(SyntaxNode node)
        {
            TextSpan fullSpan = Root.FullSpan;
            int num = node.SpanStart;
            SyntaxToken firstToken = node.GetFirstToken();
            if (firstToken.Node != null)
            {
                int spanStart = firstToken.SpanStart;
                if (spanStart < node.Span.End)
                {
                    num = spanStart;
                }
            }
            if (fullSpan.IsEmpty)
            {
                return num;
            }
            if (num == fullSpan.End)
            {
                return CheckAndAdjustPosition(num - 1);
            }
            if (node.IsMissing || node.HasErrors || node.Width == 0 || node.IsPartOfStructuredTrivia())
            {
                return CheckAndAdjustPosition(num);
            }
            return num;
        }

        [Conditional("DEBUG")]
        protected void AssertPositionAdjusted(int position)
        {
        }

        protected void CheckSyntaxNode(CSharpSyntaxNode syntax)
        {
            if (syntax == null)
            {
                throw new ArgumentNullException("syntax");
            }
            if (!IsInTree(syntax))
            {
                throw new ArgumentException(CSharpResources.SyntaxNodeIsNotWithinSynt);
            }
        }

        private void CheckModelAndSyntaxNodeToSpeculate(CSharpSyntaxNode syntax)
        {
            if (syntax == null)
            {
                throw new ArgumentNullException("syntax");
            }
            if (IsSpeculativeSemanticModel)
            {
                throw new InvalidOperationException(CSharpResources.ChainingSpeculativeModelIsNotSupported);
            }
            if (Compilation.ContainsSyntaxTree(syntax.SyntaxTree))
            {
                throw new ArgumentException(CSharpResources.SpeculatedSyntaxNodeCannotBelongToCurrentCompilation);
            }
        }

        public ImmutableArray<ISymbol> LookupSymbols(int position, Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceOrTypeSymbol container = null, string name = null, bool includeReducedExtensionMethods = false)
        {
            LookupOptions options = (includeReducedExtensionMethods ? LookupOptions.IncludeExtensionMethods : LookupOptions.Default);
            return LookupSymbolsInternal(position, container, name, options, useBaseReferenceAccessibility: false);
        }

        public new ImmutableArray<ISymbol> LookupBaseMembers(int position, string name = null)
        {
            return LookupSymbolsInternal(position, null, name, LookupOptions.Default, useBaseReferenceAccessibility: true);
        }

        public ImmutableArray<ISymbol> LookupStaticMembers(int position, Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceOrTypeSymbol container = null, string name = null)
        {
            return LookupSymbolsInternal(position, container, name, LookupOptions.MustNotBeInstance, useBaseReferenceAccessibility: false);
        }

        public ImmutableArray<ISymbol> LookupNamespacesAndTypes(int position, Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceOrTypeSymbol container = null, string name = null)
        {
            return LookupSymbolsInternal(position, container, name, LookupOptions.NamespacesOrTypesOnly, useBaseReferenceAccessibility: false);
        }

        public new ImmutableArray<ISymbol> LookupLabels(int position, string name = null)
        {
            return LookupSymbolsInternal(position, null, name, LookupOptions.LabelsOnly, useBaseReferenceAccessibility: false);
        }

        private ImmutableArray<ISymbol> LookupSymbolsInternal(int position, Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceOrTypeSymbol container, string name, LookupOptions options, bool useBaseReferenceAccessibility)
        {
            if (useBaseReferenceAccessibility)
            {
                options |= LookupOptions.UseBaseReferenceAccessibility;
            }
            options.ThrowIfInvalid();
            position = CheckAndAdjustPosition(position, out var token);
            if ((object)container == null || container.Kind == SymbolKind.Namespace)
            {
                options &= ~LookupOptions.IncludeExtensionMethods;
            }
            Binder enclosingBinder = GetEnclosingBinder(position);
            if (enclosingBinder == null)
            {
                return ImmutableArray<ISymbol>.Empty;
            }
            if (useBaseReferenceAccessibility)
            {
                Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol containingType = enclosingBinder.ContainingType;
                Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol typeSymbol = null;
                if ((object)containingType != null && containingType.Kind == SymbolKind.NamedType && ((Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol)containingType).IsScriptClass)
                {
                    return ImmutableArray<ISymbol>.Empty;
                }
                if ((object)containingType == null || (object)(typeSymbol = containingType.BaseTypeNoUseSiteDiagnostics) == null)
                {
                    throw new ArgumentException("Not a valid position for a call to LookupBaseMembers (must be in a type with a base type)", "position");
                }
                container = typeSymbol;
            }
            if (!enclosingBinder.IsInMethodBody && (options & (LookupOptions.NamespaceAliasesOnly | LookupOptions.NamespacesOrTypesOnly | LookupOptions.LabelsOnly)) == 0 && token.Parent is ExpressionSyntax expressionSyntax && !(expressionSyntax.Parent is XmlNameAttributeSyntax) && !SyntaxFacts.IsInTypeOnlyContext(expressionSyntax))
            {
                options |= LookupOptions.MustNotBeMethodTypeParameter;
            }
            LookupSymbolsInfo instance = LookupSymbolsInfo.GetInstance();
            instance.FilterName = name;
            if ((object)container == null)
            {
                enclosingBinder.AddLookupSymbolsInfo(instance, options);
            }
            else
            {
                enclosingBinder.AddMemberLookupSymbolsInfo(instance, container, options, enclosingBinder);
            }
            ArrayBuilder<ISymbol> instance2 = ArrayBuilder<ISymbol>.GetInstance(instance.Count);
            if (name == null)
            {
                foreach (string name2 in instance.Names)
                {
                    AppendSymbolsWithName(instance2, name2, enclosingBinder, container, options, instance);
                }
            }
            else
            {
                AppendSymbolsWithName(instance2, name, enclosingBinder, container, options, instance);
            }
            instance.Free();
            if ((options & LookupOptions.IncludeExtensionMethods) != 0)
            {
                LookupResult instance3 = LookupResult.GetInstance();
                options |= LookupOptions.AllMethodsOnArityZero;
                options &= ~LookupOptions.MustBeInstance;
                CompoundUseSiteInfo<Microsoft.CodeAnalysis.CSharp.Symbols.AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<Microsoft.CodeAnalysis.CSharp.Symbols.AssemblySymbol>.Discarded;
                enclosingBinder.LookupExtensionMethods(instance3, name, 0, options, ref useSiteInfo);
                if (instance3.IsMultiViable)
                {
                    Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol receiverType = (Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol)container;
                    ArrayBuilder<Symbol>.Enumerator enumerator2 = instance3.Symbols.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol methodSymbol = ((Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol)enumerator2.Current).ReduceExtensionMethod(receiverType, Compilation);
                        if ((object)methodSymbol != null)
                        {
                            instance2.Add(methodSymbol.GetPublicSymbol());
                        }
                    }
                }
                instance3.Free();
            }
            ImmutableArray<ISymbol> immutableArray = instance2.ToImmutableAndFree();
            if (name != null)
            {
                return immutableArray;
            }
            return FilterNotReferencable(immutableArray);
        }

        private void AppendSymbolsWithName(ArrayBuilder<ISymbol> results, string name, Binder binder, Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceOrTypeSymbol container, LookupOptions options, LookupSymbolsInfo info)
        {
            if (!info.TryGetAritiesAndUniqueSymbol(name, out var arities, out var uniqueSymbol))
            {
                return;
            }
            if ((object)uniqueSymbol != null)
            {
                results.Add(RemapSymbolIfNecessary(uniqueSymbol).GetPublicSymbol());
                return;
            }
            if (arities != null)
            {
                foreach (int item in arities)
                {
                    AppendSymbolsWithNameAndArity(results, name, item, binder, container, options);
                }
                return;
            }
            AppendSymbolsWithNameAndArity(results, name, 0, binder, container, options);
        }

        private void AppendSymbolsWithNameAndArity(ArrayBuilder<ISymbol> results, string name, int arity, Binder binder, Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceOrTypeSymbol container, LookupOptions options)
        {
            LookupResult instance = LookupResult.GetInstance();
            CompoundUseSiteInfo<Microsoft.CodeAnalysis.CSharp.Symbols.AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<Microsoft.CodeAnalysis.CSharp.Symbols.AssemblySymbol>.Discarded;
            binder.LookupSymbolsSimpleName(instance, container, name, arity, null, options & ~LookupOptions.IncludeExtensionMethods, diagnose: false, ref useSiteInfo);
            if (instance.IsMultiViable)
            {
                if (instance.Symbols.Any((Symbol t) => t.Kind == SymbolKind.NamedType || t.Kind == SymbolKind.Namespace || t.Kind == SymbolKind.ErrorType))
                {
                    Symbol symbol = binder.ResultSymbol(instance, name, arity, Root, BindingDiagnosticBag.Discarded, suppressUseSiteDiagnostics: true, out bool wasError, container, options);
                    if (!wasError)
                    {
                        results.Add(RemapSymbolIfNecessary(symbol).GetPublicSymbol());
                    }
                    else
                    {
                        ArrayBuilder<Symbol>.Enumerator enumerator = instance.Symbols.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            Symbol current = enumerator.Current;
                            results.Add(RemapSymbolIfNecessary(current).GetPublicSymbol());
                        }
                    }
                }
                else
                {
                    ArrayBuilder<Symbol>.Enumerator enumerator = instance.Symbols.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Symbol current2 = enumerator.Current;
                        results.Add(RemapSymbolIfNecessary(current2).GetPublicSymbol());
                    }
                }
            }
            instance.Free();
        }

        private Symbol RemapSymbolIfNecessary(Symbol symbol)
        {
            if (symbol is Microsoft.CodeAnalysis.CSharp.Symbols.LocalSymbol || symbol is Microsoft.CodeAnalysis.CSharp.Symbols.ParameterSymbol || (symbol is Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol methodSymbol && methodSymbol.MethodKind == MethodKind.AnonymousFunction))
            {
                return RemapSymbolIfNecessaryCore(symbol);
            }
            return symbol;
        }

        internal abstract Symbol RemapSymbolIfNecessaryCore(Symbol symbol);

        private static ImmutableArray<ISymbol> FilterNotReferencable(ImmutableArray<ISymbol> sealedResults)
        {
            ArrayBuilder<ISymbol> arrayBuilder = null;
            int num = 0;
            ImmutableArray<ISymbol>.Enumerator enumerator = sealedResults.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ISymbol current = enumerator.Current;
                if (current.CanBeReferencedByName)
                {
                    arrayBuilder?.Add(current);
                }
                else if (arrayBuilder == null)
                {
                    arrayBuilder = ArrayBuilder<ISymbol>.GetInstance();
                    arrayBuilder.AddRange(sealedResults, num);
                }
                num++;
            }
            return arrayBuilder?.ToImmutableAndFree() ?? sealedResults;
        }

        public bool IsAccessible(int position, Symbol symbol)
        {
            position = CheckAndAdjustPosition(position);
            if ((object)symbol == null)
            {
                throw new ArgumentNullException("symbol");
            }
            Binder enclosingBinder = GetEnclosingBinder(position);
            if (enclosingBinder != null)
            {
                CompoundUseSiteInfo<Microsoft.CodeAnalysis.CSharp.Symbols.AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<Microsoft.CodeAnalysis.CSharp.Symbols.AssemblySymbol>.Discarded;
                return enclosingBinder.IsAccessible(symbol, ref useSiteInfo);
            }
            return false;
        }

        public bool IsEventUsableAsField(int position, Microsoft.CodeAnalysis.CSharp.Symbols.EventSymbol symbol)
        {
            if ((object)symbol != null && symbol.HasAssociatedField)
            {
                return IsAccessible(position, symbol.AssociatedField);
            }
            return false;
        }

        private bool IsInTypeofExpression(int position)
        {
            for (SyntaxNode syntaxNode = Root.FindToken(position).Parent; syntaxNode != Root; syntaxNode = syntaxNode.ParentOrStructuredTriviaParent)
            {
                if (syntaxNode.IsKind(SyntaxKind.TypeOfExpression))
                {
                    return true;
                }
            }
            return false;
        }

        internal SymbolInfo GetSymbolInfoForNode(SymbolInfoOptions options, BoundNode lowestBoundNode, BoundNode highestBoundNode, BoundNode boundNodeForSyntacticParent, Binder binderOpt)
        {
            if (highestBoundNode is BoundRecursivePattern pat)
            {
                return GetSymbolInfoForDeconstruction(pat);
            }
            if (!(lowestBoundNode is BoundSubpattern subpattern))
            {
                if (lowestBoundNode is BoundExpression boundExpression)
                {
                    BoundExpression boundNode = boundExpression;
                    ImmutableArray<Symbol> immutableArray = GetSemanticSymbols(boundNode, boundNodeForSyntacticParent, binderOpt, options, out var isDynamic, out var resultKind, out var _);
                    if (highestBoundNode is BoundExpression boundExpression2)
                    {
                        ImmutableArray<Symbol> semanticSymbols = GetSemanticSymbols(boundExpression2, boundNodeForSyntacticParent, binderOpt, options, out bool isDynamic2, out LookupResultKind resultKind2, out ImmutableArray<Symbol> memberGroup2);
                        if ((immutableArray.Length != 1 || resultKind == LookupResultKind.OverloadResolutionFailure) && semanticSymbols.Length > 0)
                        {
                            immutableArray = semanticSymbols;
                            resultKind = resultKind2;
                            isDynamic = isDynamic2;
                        }
                        else if (resultKind2 != 0 && (int)resultKind2 < (int)resultKind)
                        {
                            resultKind = resultKind2;
                            isDynamic = isDynamic2;
                        }
                        else if (boundExpression2.Kind == BoundKind.TypeOrValueExpression)
                        {
                            immutableArray = semanticSymbols;
                            resultKind = resultKind2;
                            isDynamic = isDynamic2;
                        }
                        else if (boundExpression2.Kind == BoundKind.UnaryOperator && IsUserDefinedTrueOrFalse((BoundUnaryOperator)boundExpression2))
                        {
                            immutableArray = semanticSymbols;
                            resultKind = resultKind2;
                            isDynamic = isDynamic2;
                        }
                    }
                    if (resultKind == LookupResultKind.Empty)
                    {
                        return SymbolInfoFactory.Create(ImmutableArray<Symbol>.Empty, LookupResultKind.Empty, isDynamic);
                    }
                    ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance(immutableArray.Length);
                    ImmutableArray<Symbol>.Enumerator enumerator = immutableArray.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Symbol current = enumerator.Current;
                        AddUnwrappingErrorTypes(instance, current);
                    }
                    immutableArray = instance.ToImmutableAndFree();
                    if ((options & SymbolInfoOptions.ResolveAliases) != 0)
                    {
                        immutableArray = UnwrapAliases(immutableArray);
                    }
                    if (resultKind == LookupResultKind.Viable && immutableArray.Length > 1)
                    {
                        resultKind = LookupResultKind.OverloadResolutionFailure;
                    }
                    return SymbolInfoFactory.Create(immutableArray, resultKind, isDynamic);
                }
                return SymbolInfo.None;
            }
            return GetSymbolInfoForSubpattern(subpattern);
        }

        private SymbolInfo GetSymbolInfoForSubpattern(BoundSubpattern subpattern)
        {
            if (subpattern.Symbol?.OriginalDefinition is Microsoft.CodeAnalysis.CSharp.Symbols.ErrorTypeSymbol errorTypeSymbol)
            {
                return new SymbolInfo(null, errorTypeSymbol.CandidateSymbols.GetPublicSymbols(), errorTypeSymbol.ResultKind.ToCandidateReason());
            }
            return new SymbolInfo(subpattern.Symbol.GetPublicSymbol(), CandidateReason.None);
        }

        private SymbolInfo GetSymbolInfoForDeconstruction(BoundRecursivePattern pat)
        {
            return new SymbolInfo(pat.DeconstructMethod.GetPublicSymbol(), CandidateReason.None);
        }

        private static void AddUnwrappingErrorTypes(ArrayBuilder<Symbol> builder, Symbol s)
        {
            if (s.OriginalDefinition is Microsoft.CodeAnalysis.CSharp.Symbols.ErrorTypeSymbol errorTypeSymbol)
            {
                builder.AddRange(errorTypeSymbol.CandidateSymbols);
            }
            else
            {
                builder.Add(s);
            }
        }

        private static bool IsUserDefinedTrueOrFalse(BoundUnaryOperator @operator)
        {
            UnaryOperatorKind operatorKind = @operator.OperatorKind;
            if (operatorKind != UnaryOperatorKind.UserDefinedTrue)
            {
                return operatorKind == UnaryOperatorKind.UserDefinedFalse;
            }
            return true;
        }

        // Gets the semantic info from a specific bound node and a set of diagnostics
        // lowestBoundNode: The lowest node in the bound tree associated with node
        // highestBoundNode: The highest node in the bound tree associated with node
        // boundNodeForSyntacticParent: The lowest node in the bound tree associated with node.Parent.
        internal CSharpTypeInfo GetTypeInfoForNode(
            BoundNode lowestBoundNode,
            BoundNode highestBoundNode,
            BoundNode boundNodeForSyntacticParent)
        {
            BoundPattern pattern = lowestBoundNode as BoundPattern ?? highestBoundNode as BoundPattern ?? (highestBoundNode is BoundSubpattern sp ? sp.Pattern : null);
            if (pattern != null)
            {
                var discardedUseSiteInfo = CompoundUseSiteInfo<Symbols.AssemblySymbol>.Discarded;
                // https://github.com/dotnet/roslyn/issues/35032: support patterns
                return new CSharpTypeInfo(
                    pattern.InputType, pattern.NarrowedType, nullability: default, convertedNullability: default,
                    Compilation.Conversions.ClassifyBuiltInConversion(pattern.InputType, pattern.NarrowedType, ref discardedUseSiteInfo));
            }

            var highestBoundExpr = highestBoundNode as BoundExpression;

            if (lowestBoundNode is BoundExpression boundExpr &&
                !(boundNodeForSyntacticParent != null &&
                  boundNodeForSyntacticParent.Syntax.Kind() == SyntaxKind.ObjectCreationExpression &&
                  ((ObjectCreationExpressionSyntax)boundNodeForSyntacticParent.Syntax).Type == boundExpr.Syntax)) // Do not return any type information for a ObjectCreationExpressionSyntax.Type node.
            {
                // TODO: Should parenthesized expression really not have symbols? At least for C#, I'm not sure that 
                // is right. For example, C# allows the assignment statement:
                //    (i) = 9;  
                // So I don't assume this code should special case parenthesized expressions.
                Symbols.TypeSymbol type = null;
                NullabilityInfo nullability = boundExpr.TopLevelNullability;

                if (boundExpr.HasExpressionType())
                {
                    type = boundExpr.Type;

                    switch (boundExpr)
                    {
                        case BoundLocal local:
                            {
                                // Use of local before declaration requires some additional fixup.
                                // Due to complications around implicit locals and type inference, we do not
                                // try to obtain a type of a local when it is used before declaration, we use
                                // a special error type symbol. However, semantic model should return the same
                                // type information for usage of a local before and after its declaration.
                                // We will detect the use before declaration cases and replace the error type
                                // symbol with the one obtained from the local. It should be safe to get the type
                                // from the local at this point.
                                if (type is ExtendedErrorTypeSymbol extended && extended.VariableUsedBeforeDeclaration)
                                {
                                    type = local.LocalSymbol.Type;
                                    nullability = local.LocalSymbol.TypeWithAnnotations.NullableAnnotation.ToNullabilityInfo(type);
                                }
                                break;
                            }
                        case BoundConvertedTupleLiteral { SourceTuple: BoundTupleLiteral original }:
                            {
                                // The bound tree fully binds tuple literals. From the language point of
                                // view, however, converted tuple literals represent tuple conversions
                                // from tuple literal expressions which may or may not have types
                                type = original.Type;
                                break;
                            }
                    }
                }

                // we match highestBoundExpr.Kind to various kind frequently, so cache it here.
                // use NoOp kind for the case when highestBoundExpr == null - NoOp will not match anything below.
                var highestBoundExprKind = highestBoundExpr?.Kind ?? BoundKind.NoOpStatement;
                Symbols.TypeSymbol convertedType;
                NullabilityInfo convertedNullability;
                Conversion conversion;

                if (highestBoundExprKind == BoundKind.Lambda) // the enclosing conversion is explicit
                {
                    var lambda = (BoundLambda)highestBoundExpr;
                    convertedType = lambda.Type;
                    // The bound tree always fully binds lambda and anonymous functions. From the language point of
                    // view, however, anonymous functions converted to a real delegate type should only have a 
                    // ConvertedType, not a Type. So set Type to null here. Otherwise you get the edge case where both
                    // Type and ConvertedType are the same, but the conversion isn't Identity.
                    type = null;
                    nullability = default;
                    convertedNullability = new NullabilityInfo(CodeAnalysis.NullableAnnotation.NotAnnotated, CodeAnalysis.NullableFlowState.NotNull);
                    conversion = new Conversion(ConversionKind.AnonymousFunction, lambda.Symbol, false);
                }
                else if ((highestBoundExpr as BoundConversion)?.Conversion.IsTupleLiteralConversion == true)
                {
                    var tupleLiteralConversion = (BoundConversion)highestBoundExpr;
                    if (tupleLiteralConversion.Operand.Kind == BoundKind.ConvertedTupleLiteral)
                    {
                        var convertedTuple = (BoundConvertedTupleLiteral)tupleLiteralConversion.Operand;
                        type = convertedTuple.SourceTuple.Type;
                        nullability = convertedTuple.TopLevelNullability;
                    }
                    else
                    {
                        (type, nullability) = getTypeAndNullability(tupleLiteralConversion.Operand);
                    }

                    (convertedType, convertedNullability) = getTypeAndNullability(tupleLiteralConversion);
                    conversion = tupleLiteralConversion.Conversion;
                }
                else if (highestBoundExprKind == BoundKind.FixedLocalCollectionInitializer)
                {
                    var initializer = (BoundFixedLocalCollectionInitializer)highestBoundExpr;
                    (convertedType, convertedNullability) = getTypeAndNullability(initializer);
                    (type, nullability) = getTypeAndNullability(initializer.Expression);

                    // the most pertinent conversion is the pointer conversion 
                    conversion = initializer.ElementPointerTypeConversion;
                }
                else if (boundExpr is BoundConvertedSwitchExpression { WasTargetTyped: true } convertedSwitch)
                {
                    if (highestBoundExpr is BoundConversion { ConversionKind: ConversionKind.SwitchExpression })
                    {
                        // There was an implicit cast.
                        type = convertedSwitch.NaturalTypeOpt;
                        convertedType = convertedSwitch.Type;
                        convertedNullability = convertedSwitch.TopLevelNullability;
                        conversion = convertedSwitch.Conversion.IsValid ? convertedSwitch.Conversion : Conversion.NoConversion;
                    }
                    else
                    {
                        // There was an explicit cast on top of this
                        type = convertedSwitch.NaturalTypeOpt;
                        (convertedType, convertedNullability) = (type, nullability);
                        conversion = Conversion.Identity;
                    }
                }
                else if (boundExpr is BoundConditionalOperator { WasTargetTyped: true } cond)
                {
                    if (highestBoundExpr is BoundConversion { ConversionKind: ConversionKind.ConditionalExpression })
                    {
                        // There was an implicit cast.
                        type = cond.NaturalTypeOpt;
                        convertedType = cond.Type;
                        convertedNullability = nullability;
                        conversion = Conversion.MakeConditionalExpression(ImmutableArray<Conversion>.Empty);
                    }
                    else
                    {
                        // There was an explicit cast on top of this.
                        type = cond.NaturalTypeOpt;
                        (convertedType, convertedNullability) = (type, nullability);
                        conversion = Conversion.Identity;
                    }
                }
                else if (highestBoundExpr != null && highestBoundExpr != boundExpr && highestBoundExpr.HasExpressionType())
                {
                    (convertedType, convertedNullability) = getTypeAndNullability(highestBoundExpr);
                    if (highestBoundExprKind != BoundKind.Conversion)
                    {
                        conversion = Conversion.Identity;
                    }
                    else if (((BoundConversion)highestBoundExpr).Operand.Kind != BoundKind.Conversion)
                    {
                        conversion = highestBoundExpr.GetConversion();
                        if (conversion.Kind == ConversionKind.AnonymousFunction)
                        {
                            // See comment above: anonymous functions do not have a type
                            type = null;
                            nullability = default;
                        }
                    }
                    else
                    {
                        // There is a sequence of conversions; we use ClassifyConversionFromExpression to report the most pertinent.
                        var binder = this.GetEnclosingBinder(boundExpr.Syntax.Span.Start);
                        var discardedUseSiteInfo = CompoundUseSiteInfo<Symbols.AssemblySymbol>.Discarded;
                        conversion = binder.Conversions.ClassifyConversionFromExpression(boundExpr, convertedType, ref discardedUseSiteInfo);
                    }
                }
                else if (boundNodeForSyntacticParent?.Kind == BoundKind.DelegateCreationExpression)
                {
                    // A delegate creation expression takes the place of a method group or anonymous function conversion.
                    var delegateCreation = (BoundDelegateCreationExpression)boundNodeForSyntacticParent;
                    (convertedType, convertedNullability) = getTypeAndNullability(delegateCreation);
                    switch (boundExpr.Kind)
                    {
                        case BoundKind.MethodGroup:
                            {
                                conversion = new Conversion(ConversionKind.MethodGroup, delegateCreation.MethodOpt, delegateCreation.IsExtensionMethod);
                                break;
                            }
                        case BoundKind.Lambda:
                            {
                                var lambda = (BoundLambda)boundExpr;
                                conversion = new Conversion(ConversionKind.AnonymousFunction, lambda.Symbol, delegateCreation.IsExtensionMethod);
                                break;
                            }
                        case BoundKind.UnboundLambda:
                            {
                                var lambda = ((UnboundLambda)boundExpr).BindForErrorRecovery();
                                conversion = new Conversion(ConversionKind.AnonymousFunction, lambda.Symbol, delegateCreation.IsExtensionMethod);
                                break;
                            }
                        default:
                            conversion = Conversion.Identity;
                            break;
                    }
                }
                else if (boundExpr is BoundConversion { ConversionKind: ConversionKind.MethodGroup, Conversion: var exprConversion, Type: { TypeKind: TypeKind.FunctionPointer }, SymbolOpt: var symbol })
                {
                    // Because the method group is a separate syntax node from the &, the lowest bound node here is the BoundConversion. However,
                    // the conversion represents an implicit method group conversion from a typeless method group to a function pointer type, so
                    // we should reflect that in the types and conversion we return.
                    convertedType = type;
                    convertedNullability = nullability;
                    conversion = exprConversion;
                    type = null;
                    nullability = new NullabilityInfo(CodeAnalysis.NullableAnnotation.NotAnnotated, CodeAnalysis.NullableFlowState.NotNull);
                }
                else
                {
                    convertedType = type;
                    convertedNullability = nullability;
                    conversion = Conversion.Identity;
                }

                return new CSharpTypeInfo(type, convertedType, nullability, convertedNullability, conversion);
            }

            return CSharpTypeInfo.None;

            static (Symbols.TypeSymbol, NullabilityInfo) getTypeAndNullability(BoundExpression expr) => (expr.Type, expr.TopLevelNullability);
        }

        internal ImmutableArray<Symbol> GetMemberGroupForNode(SymbolInfoOptions options, BoundNode lowestBoundNode, BoundNode boundNodeForSyntacticParent, Binder binderOpt)
        {
            if (lowestBoundNode is BoundExpression boundNode)
            {
                GetSemanticSymbols(boundNode, boundNodeForSyntacticParent, binderOpt, options, out var _, out var _, out var memberGroup);
                return memberGroup;
            }
            return ImmutableArray<Symbol>.Empty;
        }

        internal ImmutableArray<IPropertySymbol> GetIndexerGroupForNode(BoundNode lowestBoundNode, Binder binderOpt)
        {
            if (lowestBoundNode is BoundExpression boundExpression && boundExpression.Kind != BoundKind.TypeExpression)
            {
                return GetIndexerGroupSemanticSymbols(boundExpression, binderOpt);
            }
            return ImmutableArray<IPropertySymbol>.Empty;
        }

        internal static SymbolInfo GetSymbolInfoForSymbol(Symbol symbol, SymbolInfoOptions options)
        {
            Symbol symbol2 = UnwrapAlias(symbol);
            Microsoft.CodeAnalysis.CSharp.Symbols.ErrorTypeSymbol errorTypeSymbol = ((symbol2 is Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol typeSymbol) ? (typeSymbol.OriginalDefinition as Microsoft.CodeAnalysis.CSharp.Symbols.ErrorTypeSymbol) : null);
            if ((object)errorTypeSymbol != null)
            {
                ImmutableArray<Symbol> symbols = ImmutableArray<Symbol>.Empty;
                LookupResultKind resultKind = errorTypeSymbol.ResultKind;
                if (resultKind != 0)
                {
                    symbols = errorTypeSymbol.CandidateSymbols;
                }
                if ((options & SymbolInfoOptions.ResolveAliases) != 0)
                {
                    symbols = UnwrapAliases(symbols);
                }
                return SymbolInfoFactory.Create(symbols, resultKind, isDynamic: false);
            }
            return new SymbolInfo((((options & SymbolInfoOptions.ResolveAliases) != 0) ? symbol2 : symbol).GetPublicSymbol(), ImmutableArray<ISymbol>.Empty, CandidateReason.None);
        }

        internal static CSharpTypeInfo GetTypeInfoForSymbol(Symbol symbol)
        {
            Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol obj = UnwrapAlias(symbol) as Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol;
            return new CSharpTypeInfo(obj, obj, default(NullabilityInfo), default(NullabilityInfo), Conversion.Identity);
        }

        protected static Symbol UnwrapAlias(Symbol symbol)
        {
            if (!(symbol is Microsoft.CodeAnalysis.CSharp.Symbols.AliasSymbol aliasSymbol))
            {
                return symbol;
            }
            return aliasSymbol.Target;
        }

        protected static ImmutableArray<Symbol> UnwrapAliases(ImmutableArray<Symbol> symbols)
        {
            bool flag = false;
            ImmutableArray<Symbol>.Enumerator enumerator = symbols.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Kind == SymbolKind.Alias)
                {
                    flag = true;
                }
            }
            if (!flag)
            {
                return symbols;
            }
            ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
            enumerator = symbols.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                AddUnwrappingErrorTypes(instance, UnwrapAlias(current));
            }
            return instance.ToImmutableAndFree();
        }

        internal virtual BoundNode Bind(Binder binder, CSharpSyntaxNode node, BindingDiagnosticBag diagnostics)
        {
            if (!(node is ExpressionSyntax expressionSyntax))
            {
                if (!(node is StatementSyntax node2))
                {
                    if (node is GlobalStatementSyntax globalStatementSyntax)
                    {
                        BoundStatement statement = binder.BindStatement(globalStatementSyntax.Statement, diagnostics);
                        return new BoundGlobalStatementInitializer(node, statement);
                    }
                    return null;
                }
                return binder.BindStatement(node2, diagnostics);
            }
            if (!expressionSyntax.Parent.IsKind(SyntaxKind.GotoStatement))
            {
                return binder.BindNamespaceOrTypeOrExpression(expressionSyntax, diagnostics);
            }
            return binder.BindLabel(expressionSyntax, diagnostics);
        }

        public virtual ControlFlowAnalysis AnalyzeControlFlow(StatementSyntax firstStatement, StatementSyntax lastStatement)
        {
            throw new NotSupportedException();
        }

        public virtual ControlFlowAnalysis AnalyzeControlFlow(StatementSyntax statement)
        {
            return AnalyzeControlFlow(statement, statement);
        }

        public virtual DataFlowAnalysis AnalyzeDataFlow(ExpressionSyntax expression)
        {
            throw new NotSupportedException();
        }

        public virtual DataFlowAnalysis AnalyzeDataFlow(StatementSyntax firstStatement, StatementSyntax lastStatement)
        {
            throw new NotSupportedException();
        }

        public virtual DataFlowAnalysis AnalyzeDataFlow(StatementSyntax statement)
        {
            return AnalyzeDataFlow(statement, statement);
        }

        public bool TryGetSpeculativeSemanticModelForMethodBody(int position, BaseMethodDeclarationSyntax method, out SemanticModel speculativeModel)
        {
            CheckModelAndSyntaxNodeToSpeculate(method);
            return TryGetSpeculativeSemanticModelForMethodBodyCore((SyntaxTreeSemanticModel)this, position, method, out speculativeModel);
        }

        internal abstract bool TryGetSpeculativeSemanticModelForMethodBodyCore(SyntaxTreeSemanticModel parentModel, int position, BaseMethodDeclarationSyntax method, out SemanticModel speculativeModel);

        public bool TryGetSpeculativeSemanticModelForMethodBody(int position, AccessorDeclarationSyntax accessor, out SemanticModel speculativeModel)
        {
            CheckModelAndSyntaxNodeToSpeculate(accessor);
            return TryGetSpeculativeSemanticModelForMethodBodyCore((SyntaxTreeSemanticModel)this, position, accessor, out speculativeModel);
        }

        internal abstract bool TryGetSpeculativeSemanticModelForMethodBodyCore(SyntaxTreeSemanticModel parentModel, int position, AccessorDeclarationSyntax accessor, out SemanticModel speculativeModel);

        public bool TryGetSpeculativeSemanticModel(int position, TypeSyntax type, out SemanticModel speculativeModel, SpeculativeBindingOption bindingOption = SpeculativeBindingOption.BindAsExpression)
        {
            CheckModelAndSyntaxNodeToSpeculate(type);
            return TryGetSpeculativeSemanticModelCore((SyntaxTreeSemanticModel)this, position, type, bindingOption, out speculativeModel);
        }

        internal abstract bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, TypeSyntax type, SpeculativeBindingOption bindingOption, out SemanticModel speculativeModel);

        public bool TryGetSpeculativeSemanticModel(int position, StatementSyntax statement, out SemanticModel speculativeModel)
        {
            CheckModelAndSyntaxNodeToSpeculate(statement);
            return TryGetSpeculativeSemanticModelCore((SyntaxTreeSemanticModel)this, position, statement, out speculativeModel);
        }

        internal abstract bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, StatementSyntax statement, out SemanticModel speculativeModel);

        public bool TryGetSpeculativeSemanticModel(int position, EqualsValueClauseSyntax initializer, out SemanticModel speculativeModel)
        {
            CheckModelAndSyntaxNodeToSpeculate(initializer);
            return TryGetSpeculativeSemanticModelCore((SyntaxTreeSemanticModel)this, position, initializer, out speculativeModel);
        }

        internal abstract bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, EqualsValueClauseSyntax initializer, out SemanticModel speculativeModel);

        public bool TryGetSpeculativeSemanticModel(int position, ArrowExpressionClauseSyntax expressionBody, out SemanticModel speculativeModel)
        {
            CheckModelAndSyntaxNodeToSpeculate(expressionBody);
            return TryGetSpeculativeSemanticModelCore((SyntaxTreeSemanticModel)this, position, expressionBody, out speculativeModel);
        }

        internal abstract bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, ArrowExpressionClauseSyntax expressionBody, out SemanticModel speculativeModel);

        public bool TryGetSpeculativeSemanticModel(int position, ConstructorInitializerSyntax constructorInitializer, out SemanticModel speculativeModel)
        {
            CheckModelAndSyntaxNodeToSpeculate(constructorInitializer);
            return TryGetSpeculativeSemanticModelCore((SyntaxTreeSemanticModel)this, position, constructorInitializer, out speculativeModel);
        }

        internal abstract bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, ConstructorInitializerSyntax constructorInitializer, out SemanticModel speculativeModel);

        public bool TryGetSpeculativeSemanticModel(int position, PrimaryConstructorBaseTypeSyntax constructorInitializer, out SemanticModel speculativeModel)
        {
            CheckModelAndSyntaxNodeToSpeculate(constructorInitializer);
            return TryGetSpeculativeSemanticModelCore((SyntaxTreeSemanticModel)this, position, constructorInitializer, out speculativeModel);
        }

        internal abstract bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, PrimaryConstructorBaseTypeSyntax constructorInitializer, out SemanticModel speculativeModel);

        public bool TryGetSpeculativeSemanticModel(int position, CrefSyntax crefSyntax, out SemanticModel speculativeModel)
        {
            CheckModelAndSyntaxNodeToSpeculate(crefSyntax);
            return TryGetSpeculativeSemanticModelCore((SyntaxTreeSemanticModel)this, position, crefSyntax, out speculativeModel);
        }

        internal abstract bool TryGetSpeculativeSemanticModelCore(SyntaxTreeSemanticModel parentModel, int position, CrefSyntax crefSyntax, out SemanticModel speculativeModel);

        public bool TryGetSpeculativeSemanticModel(int position, AttributeSyntax attribute, out SemanticModel speculativeModel)
        {
            CheckModelAndSyntaxNodeToSpeculate(attribute);
            Binder speculativeBinderForAttribute = GetSpeculativeBinderForAttribute(position, attribute);
            if (speculativeBinderForAttribute == null)
            {
                speculativeModel = null;
                return false;
            }
            Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol attributeType = (Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol)speculativeBinderForAttribute.BindType(attribute.Name, BindingDiagnosticBag.Discarded, out Symbols.AliasSymbol alias).Type;
            speculativeModel = ((SyntaxTreeSemanticModel)this).CreateSpeculativeAttributeSemanticModel(position, attribute, speculativeBinderForAttribute, alias, attributeType);
            return true;
        }

        public abstract Conversion ClassifyConversion(ExpressionSyntax expression, ITypeSymbol destination, bool isExplicitInSource = false);

        public Conversion ClassifyConversion(int position, ExpressionSyntax expression, ITypeSymbol destination, bool isExplicitInSource = false)
        {
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol typeSymbol = destination.EnsureCSharpSymbolOrNull("destination");
            if (expression.Kind() == SyntaxKind.DeclarationExpression)
            {
                return Conversion.NoConversion;
            }
            if (isExplicitInSource)
            {
                return ClassifyConversionForCast(position, expression, typeSymbol);
            }
            position = CheckAndAdjustPosition(position);
            Binder enclosingBinder = GetEnclosingBinder(position);
            if (enclosingBinder != null)
            {
                BoundExpression boundExpression = enclosingBinder.BindExpression(expression, BindingDiagnosticBag.Discarded);
                if (boundExpression != null && !typeSymbol.IsErrorType())
                {
                    CompoundUseSiteInfo<Microsoft.CodeAnalysis.CSharp.Symbols.AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<Microsoft.CodeAnalysis.CSharp.Symbols.AssemblySymbol>.Discarded;
                    return enclosingBinder.Conversions.ClassifyConversionFromExpression(boundExpression, typeSymbol, ref useSiteInfo);
                }
            }
            return Conversion.NoConversion;
        }

        internal abstract Conversion ClassifyConversionForCast(ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol destination);

        internal Conversion ClassifyConversionForCast(int position, ExpressionSyntax expression, Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol destination)
        {
            if ((object)destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            position = CheckAndAdjustPosition(position);
            Binder enclosingBinder = GetEnclosingBinder(position);
            if (enclosingBinder != null)
            {
                BoundExpression boundExpression = enclosingBinder.BindExpression(expression, BindingDiagnosticBag.Discarded);
                if (boundExpression != null && !destination.IsErrorType())
                {
                    CompoundUseSiteInfo<Microsoft.CodeAnalysis.CSharp.Symbols.AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<Microsoft.CodeAnalysis.CSharp.Symbols.AssemblySymbol>.Discarded;
                    return enclosingBinder.Conversions.ClassifyConversionFromExpression(boundExpression, destination, ref useSiteInfo, forCast: true);
                }
            }
            return Conversion.NoConversion;
        }

        public abstract ISymbol GetDeclaredSymbol(MemberDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract ISymbol GetDeclaredSymbol(LocalFunctionStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract IMethodSymbol GetDeclaredSymbol(CompilationUnitSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract INamespaceSymbol GetDeclaredSymbol(NamespaceDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract INamedTypeSymbol GetDeclaredSymbol(BaseTypeDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract INamedTypeSymbol GetDeclaredSymbol(DelegateDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract IFieldSymbol GetDeclaredSymbol(EnumMemberDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract IMethodSymbol GetDeclaredSymbol(BaseMethodDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract ISymbol GetDeclaredSymbol(BasePropertyDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract IPropertySymbol GetDeclaredSymbol(PropertyDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract IPropertySymbol GetDeclaredSymbol(IndexerDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract IEventSymbol GetDeclaredSymbol(EventDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract IPropertySymbol GetDeclaredSymbol(AnonymousObjectMemberDeclaratorSyntax declaratorSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract INamedTypeSymbol GetDeclaredSymbol(AnonymousObjectCreationExpressionSyntax declaratorSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract INamedTypeSymbol GetDeclaredSymbol(TupleExpressionSyntax declaratorSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract ISymbol GetDeclaredSymbol(ArgumentSyntax declaratorSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract IMethodSymbol GetDeclaredSymbol(AccessorDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract IMethodSymbol GetDeclaredSymbol(ArrowExpressionClauseSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract ISymbol GetDeclaredSymbol(VariableDeclaratorSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract ISymbol GetDeclaredSymbol(SingleVariableDesignationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract ILabelSymbol GetDeclaredSymbol(LabeledStatementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract ILabelSymbol GetDeclaredSymbol(SwitchLabelSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract IAliasSymbol GetDeclaredSymbol(UsingDirectiveSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract IAliasSymbol GetDeclaredSymbol(ExternAliasDirectiveSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        public abstract IParameterSymbol GetDeclaredSymbol(ParameterSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        internal abstract ImmutableArray<ISymbol> GetDeclaredSymbols(BaseFieldDeclarationSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken));

        protected Microsoft.CodeAnalysis.CSharp.Symbols.ParameterSymbol GetParameterSymbol(ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbols.ParameterSymbol> parameters, ParameterSyntax parameter, CancellationToken cancellationToken = default(CancellationToken))
        {
            ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbols.ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Microsoft.CodeAnalysis.CSharp.Symbols.ParameterSymbol current = enumerator.Current;
                cancellationToken.ThrowIfCancellationRequested();
                ImmutableArray<Location>.Enumerator enumerator2 = current.Locations.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Location current2 = enumerator2.Current;
                    cancellationToken.ThrowIfCancellationRequested();
                    if (current2.SourceTree == SyntaxTree && parameter.Span.Contains(current2.SourceSpan))
                    {
                        return current;
                    }
                }
            }
            return null;
        }

        public abstract ITypeParameterSymbol GetDeclaredSymbol(TypeParameterSyntax typeParameter, CancellationToken cancellationToken = default(CancellationToken));

        internal BinderFlags GetSemanticModelBinderFlags()
        {
            if (!IgnoresAccessibility)
            {
                return BinderFlags.SemanticModel;
            }
            return BinderFlags.SemanticModel | BinderFlags.IgnoreAccessibility;
        }

        public ILocalSymbol GetDeclaredSymbol(ForEachStatementSyntax forEachStatement, CancellationToken cancellationToken = default(CancellationToken))
        {
            Binder enclosingBinder = GetEnclosingBinder(GetAdjustedNodePosition(forEachStatement));
            if (enclosingBinder == null)
            {
                return null;
            }
            Binder binder = enclosingBinder.GetBinder(forEachStatement);
            if (binder == null)
            {
                return null;
            }
            Microsoft.CodeAnalysis.CSharp.Symbols.LocalSymbol localSymbol = binder.GetDeclaredLocalsForScope(forEachStatement).FirstOrDefault();
            return ((localSymbol is SourceLocalSymbol sourceLocalSymbol && sourceLocalSymbol.DeclarationKind == LocalDeclarationKind.ForEachIterationVariable) ? GetAdjustedLocalSymbol(sourceLocalSymbol) : localSymbol).GetPublicSymbol();
        }

        internal abstract Microsoft.CodeAnalysis.CSharp.Symbols.LocalSymbol GetAdjustedLocalSymbol(SourceLocalSymbol originalSymbol);

        public ILocalSymbol GetDeclaredSymbol(CatchDeclarationSyntax catchDeclaration, CancellationToken cancellationToken = default(CancellationToken))
        {
            CSharpSyntaxNode parent = catchDeclaration.Parent;
            Binder enclosingBinder = GetEnclosingBinder(GetAdjustedNodePosition(parent));
            if (enclosingBinder == null)
            {
                return null;
            }
            if (enclosingBinder.GetBinder(parent) == null)
            {
                return null;
            }
            Microsoft.CodeAnalysis.CSharp.Symbols.LocalSymbol localSymbol = enclosingBinder.GetBinder(parent)!.GetDeclaredLocalsForScope(parent).FirstOrDefault();
            if ((object)localSymbol == null || localSymbol.DeclarationKind != LocalDeclarationKind.CatchVariable)
            {
                return null;
            }
            return localSymbol.GetPublicSymbol();
        }

        public abstract IRangeVariableSymbol GetDeclaredSymbol(QueryClauseSyntax queryClause, CancellationToken cancellationToken = default(CancellationToken));

        public abstract IRangeVariableSymbol GetDeclaredSymbol(JoinIntoClauseSyntax node, CancellationToken cancellationToken = default(CancellationToken));

        public abstract IRangeVariableSymbol GetDeclaredSymbol(QueryContinuationSyntax node, CancellationToken cancellationToken = default(CancellationToken));

        private ImmutableArray<Symbol> GetSemanticSymbols(BoundExpression boundNode, BoundNode boundNodeForSyntacticParent, Binder binderOpt, SymbolInfoOptions options, out bool isDynamic, out LookupResultKind resultKind, out ImmutableArray<Symbol> memberGroup)
        {
            memberGroup = ImmutableArray<Symbol>.Empty;
            ImmutableArray<Symbol> symbols = ImmutableArray<Symbol>.Empty;
            resultKind = LookupResultKind.Viable;
            isDynamic = false;
            switch (boundNode.Kind)
            {
                case BoundKind.MethodGroup:
                    symbols = GetMethodGroupSemanticSymbols((BoundMethodGroup)boundNode, boundNodeForSyntacticParent, binderOpt, out resultKind, out isDynamic, out memberGroup);
                    break;
                case BoundKind.PropertyGroup:
                    symbols = GetPropertyGroupSemanticSymbols((BoundPropertyGroup)boundNode, boundNodeForSyntacticParent, binderOpt, out resultKind, out memberGroup);
                    break;
                case BoundKind.BadExpression:
                    {
                        BoundBadExpression boundBadExpression = (BoundBadExpression)boundNode;
                        resultKind = boundBadExpression.ResultKind;
                        SyntaxKind syntaxKind = boundBadExpression.Syntax.Kind();
                        if (syntaxKind == SyntaxKind.ObjectCreationExpression || syntaxKind == SyntaxKind.ImplicitObjectCreationExpression)
                        {
                            if (resultKind == LookupResultKind.NotCreatable)
                            {
                                return boundBadExpression.Symbols;
                            }
                            if (boundBadExpression.Type.IsDelegateType())
                            {
                                resultKind = LookupResultKind.Empty;
                                return symbols;
                            }
                            memberGroup = boundBadExpression.Symbols;
                        }
                        return boundBadExpression.Symbols;
                    }
                case BoundKind.TypeExpression:
                    {
                        BoundTypeExpression boundTypeExpression = (BoundTypeExpression)boundNode;
                        if (boundNodeForSyntacticParent != null && boundNodeForSyntacticParent.Syntax.Kind() == SyntaxKind.ObjectCreationExpression && ((ObjectCreationExpressionSyntax)boundNodeForSyntacticParent.Syntax).Type == boundTypeExpression.Syntax && boundNodeForSyntacticParent.Kind == BoundKind.BadExpression && ((BoundBadExpression)boundNodeForSyntacticParent).ResultKind == LookupResultKind.NotCreatable)
                        {
                            resultKind = LookupResultKind.NotCreatable;
                        }
                        Symbol symbol = (Symbol)(boundTypeExpression.AliasOpt ?? ((object)boundTypeExpression.Type));
                        if (symbol.OriginalDefinition is Microsoft.CodeAnalysis.CSharp.Symbols.ErrorTypeSymbol errorTypeSymbol)
                        {
                            resultKind = errorTypeSymbol.ResultKind;
                            symbols = errorTypeSymbol.CandidateSymbols;
                        }
                        else
                        {
                            symbols = ImmutableArray.Create(symbol);
                        }
                        break;
                    }
                case BoundKind.TypeOrValueExpression:
                    {
                        BoundExpression valueExpression = ((BoundTypeOrValueExpression)boundNode).Data.ValueExpression;
                        return GetSemanticSymbols(valueExpression, boundNodeForSyntacticParent, binderOpt, options, out isDynamic, out resultKind, out memberGroup);
                    }
                case BoundKind.Call:
                    {
                        BoundCall boundCall = (BoundCall)boundNode;
                        if (boundCall.OriginalMethodsOpt.IsDefault)
                        {
                            if ((object)boundCall.Method != null)
                            {
                                symbols = CreateReducedExtensionMethodIfPossible(boundCall);
                                resultKind = boundCall.ResultKind;
                            }
                        }
                        else
                        {
                            symbols = StaticCast<Symbol>.From(CreateReducedExtensionMethodsFromOriginalsIfNecessary(boundCall, Compilation));
                            resultKind = boundCall.ResultKind;
                        }
                        break;
                    }
                case BoundKind.FunctionPointerInvocation:
                    {
                        BoundFunctionPointerInvocation boundFunctionPointerInvocation = (BoundFunctionPointerInvocation)boundNode;
                        symbols = ImmutableArray.Create((Symbol)boundFunctionPointerInvocation.FunctionPointer);
                        resultKind = boundFunctionPointerInvocation.ResultKind;
                        break;
                    }
                case BoundKind.UnconvertedAddressOfOperator:
                    {
                        symbols = GetMethodGroupSemanticSymbols(((BoundUnconvertedAddressOfOperator)boundNode).Operand, boundNodeForSyntacticParent, binderOpt, out resultKind, out isDynamic, out var _);
                        break;
                    }
                case BoundKind.IndexerAccess:
                    {
                        BoundIndexerAccess boundIndexerAccess = (BoundIndexerAccess)boundNode;
                        resultKind = boundIndexerAccess.ResultKind;
                        ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbols.PropertySymbol> originalIndexersOpt = boundIndexerAccess.OriginalIndexersOpt;
                        symbols = (originalIndexersOpt.IsDefault ? ImmutableArray.Create((Symbol)boundIndexerAccess.Indexer) : StaticCast<Symbol>.From(originalIndexersOpt));
                        break;
                    }
                case BoundKind.IndexOrRangePatternIndexerAccess:
                    {
                        BoundIndexOrRangePatternIndexerAccess boundIndexOrRangePatternIndexerAccess = (BoundIndexOrRangePatternIndexerAccess)boundNode;
                        resultKind = boundIndexOrRangePatternIndexerAccess.ResultKind;
                        symbols = ImmutableArray.Create(boundIndexOrRangePatternIndexerAccess.PatternSymbol);
                        break;
                    }
                case BoundKind.EventAssignmentOperator:
                    {
                        BoundEventAssignmentOperator boundEventAssignmentOperator = (BoundEventAssignmentOperator)boundNode;
                        isDynamic = boundEventAssignmentOperator.IsDynamic;
                        Microsoft.CodeAnalysis.CSharp.Symbols.EventSymbol @event = boundEventAssignmentOperator.Event;
                        Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol methodSymbol = (boundEventAssignmentOperator.IsAddition ? @event.AddMethod : @event.RemoveMethod);
                        if ((object)methodSymbol == null)
                        {
                            symbols = ImmutableArray<Symbol>.Empty;
                            resultKind = LookupResultKind.Empty;
                        }
                        else
                        {
                            symbols = ImmutableArray.Create((Symbol)methodSymbol);
                            resultKind = boundEventAssignmentOperator.ResultKind;
                        }
                        break;
                    }
                case BoundKind.EventAccess:
                    if (boundNodeForSyntacticParent is BoundEventAssignmentOperator boundEventAssignmentOperator2 && boundEventAssignmentOperator2.ResultKind == LookupResultKind.Viable)
                    {
                        Symbol expressionSymbol = boundNode.ExpressionSymbol;
                        if ((object)expressionSymbol != null && boundNode != boundEventAssignmentOperator2.Argument && boundEventAssignmentOperator2.Event.Equals(expressionSymbol, TypeCompareKind.AllNullableIgnoreOptions))
                        {
                            symbols = ImmutableArray.Create((Symbol)boundEventAssignmentOperator2.Event);
                            resultKind = boundEventAssignmentOperator2.ResultKind;
                            break;
                        }
                    }
                    goto default;
                case BoundKind.Conversion:
                    {
                        BoundConversion boundConversion = (BoundConversion)boundNode;
                        isDynamic = boundConversion.ConversionKind.IsDynamic();
                        if (isDynamic)
                        {
                            break;
                        }
                        if (boundConversion.ConversionKind == ConversionKind.MethodGroup && boundConversion.IsExtensionMethod)
                        {
                            symbols = ImmutableArray.Create((Symbol)ReducedExtensionMethodSymbol.Create(boundConversion.SymbolOpt));
                            resultKind = boundConversion.ResultKind;
                            break;
                        }
                        if (boundConversion.ConversionKind.IsUserDefinedConversion())
                        {
                            GetSymbolsAndResultKind(boundConversion, boundConversion.SymbolOpt, boundConversion.OriginalUserDefinedConversionsOpt, out symbols, out resultKind);
                            break;
                        }
                        goto default;
                    }
                case BoundKind.BinaryOperator:
                    GetSymbolsAndResultKind((BoundBinaryOperator)boundNode, out isDynamic, ref resultKind, ref symbols);
                    break;
                case BoundKind.UnaryOperator:
                    GetSymbolsAndResultKind((BoundUnaryOperator)boundNode, out isDynamic, ref resultKind, ref symbols);
                    break;
                case BoundKind.UserDefinedConditionalLogicalOperator:
                    {
                        BoundUserDefinedConditionalLogicalOperator boundUserDefinedConditionalLogicalOperator = (BoundUserDefinedConditionalLogicalOperator)boundNode;
                        isDynamic = false;
                        GetSymbolsAndResultKind(boundUserDefinedConditionalLogicalOperator, boundUserDefinedConditionalLogicalOperator.LogicalOperator, boundUserDefinedConditionalLogicalOperator.OriginalUserDefinedOperatorsOpt, out symbols, out resultKind);
                        break;
                    }
                case BoundKind.CompoundAssignmentOperator:
                    GetSymbolsAndResultKind((BoundCompoundAssignmentOperator)boundNode, out isDynamic, ref resultKind, ref symbols);
                    break;
                case BoundKind.IncrementOperator:
                    GetSymbolsAndResultKind((BoundIncrementOperator)boundNode, out isDynamic, ref resultKind, ref symbols);
                    break;
                case BoundKind.AwaitExpression:
                    {
                        BoundAwaitExpression boundAwaitExpression = (BoundAwaitExpression)boundNode;
                        isDynamic = boundAwaitExpression.AwaitableInfo.IsDynamic;
                        goto default;
                    }
                case BoundKind.ConditionalOperator:
                    {
                        BoundConditionalOperator boundConditionalOperator = (BoundConditionalOperator)boundNode;
                        isDynamic = boundConditionalOperator.IsDynamic;
                        goto default;
                    }
                case BoundKind.Attribute:
                    {
                        BoundAttribute boundAttribute = (BoundAttribute)boundNode;
                        resultKind = boundAttribute.ResultKind;
                        Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol namedTypeSymbol = (Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol)boundAttribute.Type;
                        if (namedTypeSymbol.IsErrorType())
                        {
                            ImmutableArray<Symbol> candidateSymbols = ((Microsoft.CodeAnalysis.CSharp.Symbols.ErrorTypeSymbol)namedTypeSymbol).CandidateSymbols;
                            if (candidateSymbols.Length != 1 || !(candidateSymbols[0] is Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol))
                            {
                                symbols = candidateSymbols;
                                break;
                            }
                            namedTypeSymbol = (Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol)candidateSymbols[0];
                        }
                        AdjustSymbolsForObjectCreation(boundAttribute, namedTypeSymbol, boundAttribute.Constructor, binderOpt, ref resultKind, ref symbols, ref memberGroup);
                        break;
                    }
                case BoundKind.QueryClause:
                    {
                        BoundQueryClause boundQueryClause = (BoundQueryClause)boundNode;
                        ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
                        if (boundQueryClause.Operation != null && (object)boundQueryClause.Operation!.ExpressionSymbol != null)
                        {
                            instance.Add(boundQueryClause.Operation!.ExpressionSymbol);
                        }
                        if ((object)boundQueryClause.DefinedSymbol != null)
                        {
                            instance.Add(boundQueryClause.DefinedSymbol);
                        }
                        if (boundQueryClause.Cast != null && (object)boundQueryClause.Cast!.ExpressionSymbol != null)
                        {
                            instance.Add(boundQueryClause.Cast!.ExpressionSymbol);
                        }
                        symbols = instance.ToImmutableAndFree();
                        break;
                    }
                case BoundKind.DynamicInvocation:
                    {
                        BoundDynamicInvocation boundDynamicInvocation = (BoundDynamicInvocation)boundNode;
                        symbols = (memberGroup = boundDynamicInvocation.ApplicableMethods.Cast<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol, Symbol>());
                        isDynamic = true;
                        break;
                    }
                case BoundKind.DynamicCollectionElementInitializer:
                    {
                        BoundDynamicCollectionElementInitializer boundDynamicCollectionElementInitializer = (BoundDynamicCollectionElementInitializer)boundNode;
                        symbols = (memberGroup = boundDynamicCollectionElementInitializer.ApplicableMethods.Cast<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol, Symbol>());
                        isDynamic = true;
                        break;
                    }
                case BoundKind.DynamicIndexerAccess:
                    {
                        BoundDynamicIndexerAccess boundDynamicIndexerAccess = (BoundDynamicIndexerAccess)boundNode;
                        symbols = (memberGroup = boundDynamicIndexerAccess.ApplicableIndexers.Cast<Microsoft.CodeAnalysis.CSharp.Symbols.PropertySymbol, Symbol>());
                        isDynamic = true;
                        break;
                    }
                case BoundKind.DynamicMemberAccess:
                    isDynamic = true;
                    break;
                case BoundKind.DynamicObjectCreationExpression:
                    {
                        BoundDynamicObjectCreationExpression boundDynamicObjectCreationExpression = (BoundDynamicObjectCreationExpression)boundNode;
                        symbols = (memberGroup = boundDynamicObjectCreationExpression.ApplicableMethods.Cast<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol, Symbol>());
                        isDynamic = true;
                        break;
                    }
                case BoundKind.ObjectCreationExpression:
                    {
                        BoundObjectCreationExpression boundObjectCreationExpression = (BoundObjectCreationExpression)boundNode;
                        if ((object)boundObjectCreationExpression.Constructor != null)
                        {
                            symbols = ImmutableArray.Create((Symbol)boundObjectCreationExpression.Constructor);
                        }
                        else if (boundObjectCreationExpression.ConstructorsGroup.Length > 0)
                        {
                            symbols = StaticCast<Symbol>.From(boundObjectCreationExpression.ConstructorsGroup);
                            resultKind = resultKind.WorseResultKind(LookupResultKind.OverloadResolutionFailure);
                        }
                        memberGroup = boundObjectCreationExpression.ConstructorsGroup.Cast<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol, Symbol>();
                        break;
                    }
                case BoundKind.ThisReference:
                case BoundKind.BaseReference:
                    {
                        Binder obj = binderOpt ?? GetEnclosingBinder(GetAdjustedNodePosition(boundNode.Syntax));
                        Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol containingType = obj.ContainingType;
                        Symbol containingMember = obj.ContainingMember();
                        Microsoft.CodeAnalysis.CSharp.Symbols.ParameterSymbol thisParameter = GetThisParameter(boundNode.Type, containingType, containingMember, out resultKind);
                        symbols = ((thisParameter != null) ? ImmutableArray.Create((Symbol)thisParameter) : ImmutableArray<Symbol>.Empty);
                        break;
                    }
                case BoundKind.FromEndIndexExpression:
                    {
                        BoundFromEndIndexExpression boundFromEndIndexExpression = (BoundFromEndIndexExpression)boundNode;
                        if ((object)boundFromEndIndexExpression.MethodOpt != null)
                        {
                            symbols = ImmutableArray.Create((Symbol)boundFromEndIndexExpression.MethodOpt);
                        }
                        break;
                    }
                case BoundKind.RangeExpression:
                    {
                        BoundRangeExpression boundRangeExpression = (BoundRangeExpression)boundNode;
                        if ((object)boundRangeExpression.MethodOpt != null)
                        {
                            symbols = ImmutableArray.Create((Symbol)boundRangeExpression.MethodOpt);
                        }
                        break;
                    }
                default:
                    {
                        Symbol expressionSymbol2 = boundNode.ExpressionSymbol;
                        if ((object)expressionSymbol2 != null)
                        {
                            symbols = ImmutableArray.Create(expressionSymbol2);
                            resultKind = boundNode.ResultKind;
                        }
                        break;
                    }
                case BoundKind.DelegateCreationExpression:
                    break;
            }
            if (boundNodeForSyntacticParent != null && (options & SymbolInfoOptions.PreferConstructorsToType) != 0)
            {
                AdjustSymbolsForObjectCreation(boundNode, boundNodeForSyntacticParent, binderOpt, ref resultKind, ref symbols, ref memberGroup);
            }
            return symbols;
        }

        private static Microsoft.CodeAnalysis.CSharp.Symbols.ParameterSymbol GetThisParameter(Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol typeOfThis, Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol containingType, Symbol containingMember, out LookupResultKind resultKind)
        {
            if ((object)containingMember == null || (object)containingType == null)
            {
                resultKind = LookupResultKind.NotReferencable;
                return new ThisParameterSymbol(containingMember as Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol, typeOfThis);
            }
            SymbolKind kind = containingMember.Kind;
            Microsoft.CodeAnalysis.CSharp.Symbols.ParameterSymbol result;
            if (kind == SymbolKind.Field || kind == SymbolKind.Method || kind == SymbolKind.Property)
            {
                if (containingMember.IsStatic)
                {
                    resultKind = LookupResultKind.StaticInstanceMismatch;
                    result = new ThisParameterSymbol(containingMember as Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol, containingType);
                }
                else if ((object)typeOfThis == Microsoft.CodeAnalysis.CSharp.Symbols.ErrorTypeSymbol.UnknownResultType)
                {
                    result = new ThisParameterSymbol(containingMember as Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol, containingType);
                    resultKind = LookupResultKind.NotReferencable;
                }
                else
                {
                    switch (containingMember.Kind)
                    {
                        case SymbolKind.Method:
                            resultKind = LookupResultKind.Viable;
                            result = containingMember.EnclosingThisSymbol();
                            break;
                        case SymbolKind.Field:
                        case SymbolKind.Property:
                            resultKind = LookupResultKind.NotReferencable;
                            result = containingMember.EnclosingThisSymbol() ?? new ThisParameterSymbol(null, containingType);
                            break;
                        default:
                            throw ExceptionUtilities.UnexpectedValue(containingMember.Kind);
                    }
                }
            }
            else
            {
                result = new ThisParameterSymbol(containingMember as Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol, typeOfThis);
                resultKind = LookupResultKind.NotReferencable;
            }
            return result;
        }

        private static void GetSymbolsAndResultKind(BoundUnaryOperator unaryOperator, out bool isDynamic, ref LookupResultKind resultKind, ref ImmutableArray<Symbol> symbols)
        {
            UnaryOperatorKind unaryOperatorKind = unaryOperator.OperatorKind.OperandTypes();
            isDynamic = unaryOperator.OperatorKind.IsDynamic();
            if (unaryOperatorKind == UnaryOperatorKind.Error || unaryOperatorKind == UnaryOperatorKind.UserDefined || unaryOperator.ResultKind != LookupResultKind.Viable)
            {
                if (!isDynamic)
                {
                    GetSymbolsAndResultKind(unaryOperator, unaryOperator.MethodOpt, unaryOperator.OriginalUserDefinedOperatorsOpt, out symbols, out resultKind);
                }
            }
            else
            {
                UnaryOperatorKind kind = unaryOperator.OperatorKind.Operator();
                symbols = ImmutableArray.Create((Symbol)new SynthesizedIntrinsicOperatorSymbol(unaryOperator.Operand.Type.StrippedType(), OperatorFacts.UnaryOperatorNameFromOperatorKind(kind), unaryOperator.Type.StrippedType(), unaryOperator.OperatorKind.IsChecked()));
                resultKind = unaryOperator.ResultKind;
            }
        }

        private static void GetSymbolsAndResultKind(BoundIncrementOperator increment, out bool isDynamic, ref LookupResultKind resultKind, ref ImmutableArray<Symbol> symbols)
        {
            UnaryOperatorKind unaryOperatorKind = increment.OperatorKind.OperandTypes();
            isDynamic = increment.OperatorKind.IsDynamic();
            if (unaryOperatorKind == UnaryOperatorKind.Error || unaryOperatorKind == UnaryOperatorKind.UserDefined || increment.ResultKind != LookupResultKind.Viable)
            {
                if (!isDynamic)
                {
                    GetSymbolsAndResultKind(increment, increment.MethodOpt, increment.OriginalUserDefinedOperatorsOpt, out symbols, out resultKind);
                }
            }
            else
            {
                UnaryOperatorKind kind = increment.OperatorKind.Operator();
                symbols = ImmutableArray.Create((Symbol)new SynthesizedIntrinsicOperatorSymbol(increment.Operand.Type.StrippedType(), OperatorFacts.UnaryOperatorNameFromOperatorKind(kind), increment.Type.StrippedType(), increment.OperatorKind.IsChecked()));
                resultKind = increment.ResultKind;
            }
        }

        private static void GetSymbolsAndResultKind(BoundBinaryOperator binaryOperator, out bool isDynamic, ref LookupResultKind resultKind, ref ImmutableArray<Symbol> symbols)
        {
            BinaryOperatorKind binaryOperatorKind = binaryOperator.OperatorKind.OperandTypes();
            BinaryOperatorKind binaryOperatorKind2 = binaryOperator.OperatorKind.Operator();
            isDynamic = binaryOperator.OperatorKind.IsDynamic();
            if (binaryOperatorKind == BinaryOperatorKind.Error || binaryOperatorKind == BinaryOperatorKind.UserDefined || binaryOperator.ResultKind != LookupResultKind.Viable || binaryOperator.OperatorKind.IsLogical())
            {
                if (!isDynamic)
                {
                    GetSymbolsAndResultKind(binaryOperator, binaryOperator.MethodOpt, binaryOperator.OriginalUserDefinedOperatorsOpt, out symbols, out resultKind);
                }
                return;
            }
            if (!isDynamic && (binaryOperatorKind2 == BinaryOperatorKind.Equal || binaryOperatorKind2 == BinaryOperatorKind.NotEqual) && ((binaryOperator.Left.IsLiteralNull() && binaryOperator.Right.Type.IsNullableType()) || (binaryOperator.Right.IsLiteralNull() && binaryOperator.Left.Type.IsNullableType())) && binaryOperator.Type.SpecialType == SpecialType.System_Boolean)
            {
                Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol specialType = binaryOperator.Type.ContainingAssembly.GetSpecialType(SpecialType.System_Object);
                symbols = ImmutableArray.Create((Symbol)new SynthesizedIntrinsicOperatorSymbol(specialType, OperatorFacts.BinaryOperatorNameFromOperatorKind(binaryOperatorKind2), specialType, binaryOperator.Type, binaryOperator.OperatorKind.IsChecked()));
            }
            else
            {
                symbols = ImmutableArray.Create(GetIntrinsicOperatorSymbol(binaryOperatorKind2, isDynamic, binaryOperator.Left.Type, binaryOperator.Right.Type, binaryOperator.Type, binaryOperator.OperatorKind.IsChecked()));
            }
            resultKind = binaryOperator.ResultKind;
        }

        private static Symbol GetIntrinsicOperatorSymbol(BinaryOperatorKind op, bool isDynamic, Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol leftType, Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol rightType, Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol returnType, bool isChecked)
        {
            if (!isDynamic)
            {
                leftType = leftType.StrippedType();
                rightType = rightType.StrippedType();
                returnType = returnType.StrippedType();
            }
            else if ((object)leftType == null)
            {
                leftType = rightType;
            }
            else if ((object)rightType == null)
            {
                rightType = leftType;
            }
            return new SynthesizedIntrinsicOperatorSymbol(leftType, OperatorFacts.BinaryOperatorNameFromOperatorKind(op), rightType, returnType, isChecked);
        }

        private static void GetSymbolsAndResultKind(BoundCompoundAssignmentOperator compoundAssignment, out bool isDynamic, ref LookupResultKind resultKind, ref ImmutableArray<Symbol> symbols)
        {
            BinaryOperatorKind binaryOperatorKind = compoundAssignment.Operator.Kind.OperandTypes();
            BinaryOperatorKind op = compoundAssignment.Operator.Kind.Operator();
            isDynamic = compoundAssignment.Operator.Kind.IsDynamic();
            if (binaryOperatorKind == BinaryOperatorKind.Error || binaryOperatorKind == BinaryOperatorKind.UserDefined || compoundAssignment.ResultKind != LookupResultKind.Viable)
            {
                if (!isDynamic)
                {
                    GetSymbolsAndResultKind(compoundAssignment, compoundAssignment.Operator.Method, compoundAssignment.OriginalUserDefinedOperatorsOpt, out symbols, out resultKind);
                }
            }
            else
            {
                symbols = ImmutableArray.Create(GetIntrinsicOperatorSymbol(op, isDynamic, compoundAssignment.Operator.LeftType, compoundAssignment.Operator.RightType, compoundAssignment.Operator.ReturnType, compoundAssignment.Operator.Kind.IsChecked()));
                resultKind = compoundAssignment.ResultKind;
            }
        }

        private static void GetSymbolsAndResultKind(BoundExpression node, Symbol symbolOpt, ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol> originalCandidates, out ImmutableArray<Symbol> symbols, out LookupResultKind resultKind)
        {
            if ((object)symbolOpt != null)
            {
                symbols = ImmutableArray.Create(symbolOpt);
                resultKind = node.ResultKind;
            }
            else if (!originalCandidates.IsDefault)
            {
                symbols = StaticCast<Symbol>.From(originalCandidates);
                resultKind = node.ResultKind;
            }
            else
            {
                symbols = ImmutableArray<Symbol>.Empty;
                resultKind = LookupResultKind.Empty;
            }
        }

        private void AdjustSymbolsForObjectCreation(BoundExpression boundNode, BoundNode boundNodeForSyntacticParent, Binder binderOpt, ref LookupResultKind resultKind, ref ImmutableArray<Symbol> symbols, ref ImmutableArray<Symbol> memberGroup)
        {
            Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol typeSymbolOpt = null;
            Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol constructorOpt = null;
            SyntaxNode syntax = boundNodeForSyntacticParent.Syntax;
            if (syntax == null || syntax != boundNode.Syntax.Parent || syntax.Kind() != SyntaxKind.Attribute || ((AttributeSyntax)syntax).Name != boundNode.Syntax)
            {
                return;
            }
            ImmutableArray<Symbol> immutableArray = UnwrapAliases(symbols);
            switch (boundNodeForSyntacticParent.Kind)
            {
                case BoundKind.Attribute:
                    {
                        BoundAttribute boundAttribute = (BoundAttribute)boundNodeForSyntacticParent;
                        if (immutableArray.Length == 1 && immutableArray[0].Kind == SymbolKind.NamedType)
                        {
                            typeSymbolOpt = (Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol)immutableArray[0];
                            constructorOpt = boundAttribute.Constructor;
                            resultKind = resultKind.WorseResultKind(boundAttribute.ResultKind);
                        }
                        break;
                    }
                case BoundKind.BadExpression:
                    {
                        BoundBadExpression boundBadExpression = (BoundBadExpression)boundNodeForSyntacticParent;
                        if (immutableArray.Length == 1)
                        {
                            resultKind = resultKind.WorseResultKind(boundBadExpression.ResultKind);
                            typeSymbolOpt = immutableArray[0] as Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol;
                        }
                        break;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(boundNodeForSyntacticParent.Kind);
            }
            AdjustSymbolsForObjectCreation(boundNode, typeSymbolOpt, constructorOpt, binderOpt, ref resultKind, ref symbols, ref memberGroup);
        }

        private void AdjustSymbolsForObjectCreation(BoundNode lowestBoundNode, Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol typeSymbolOpt, Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol constructorOpt, Binder binderOpt, ref LookupResultKind resultKind, ref ImmutableArray<Symbol> symbols, ref ImmutableArray<Symbol> memberGroup)
        {
            if ((object)typeSymbolOpt == null)
            {
                return;
            }
            Binder binder = binderOpt ?? GetEnclosingBinder(GetAdjustedNodePosition(lowestBoundNode.Syntax));
            ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol> immutableArray2;
            if (binder != null)
            {
                ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol> immutableArray = ((typeSymbolOpt.IsInterfaceType() && (object)typeSymbolOpt.ComImportCoClass != null) ? typeSymbolOpt.ComImportCoClass.InstanceConstructors : typeSymbolOpt.InstanceConstructors);
                CompoundUseSiteInfo<Microsoft.CodeAnalysis.CSharp.Symbols.AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<Microsoft.CodeAnalysis.CSharp.Symbols.AssemblySymbol>.Discarded;
                immutableArray2 = binder.FilterInaccessibleConstructors(immutableArray, allowProtectedConstructorsOfBaseType: false, ref useSiteInfo);
                if (((object)constructorOpt == null) ? (!immutableArray2.Any()) : (!immutableArray2.Contains(constructorOpt)))
                {
                    immutableArray2 = immutableArray;
                }
            }
            else
            {
                immutableArray2 = ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol>.Empty;
            }
            if ((object)constructorOpt != null)
            {
                symbols = ImmutableArray.Create((Symbol)constructorOpt);
            }
            else if (immutableArray2.Length > 0)
            {
                symbols = StaticCast<Symbol>.From(immutableArray2);
                resultKind = resultKind.WorseResultKind(LookupResultKind.OverloadResolutionFailure);
            }
            memberGroup = immutableArray2.Cast<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol, Symbol>();
        }

        private ImmutableArray<IPropertySymbol> GetIndexerGroupSemanticSymbols(BoundExpression boundNode, Binder binderOpt)
        {
            Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol type = boundNode.Type;
            if ((object)type == null || type.IsStatic)
            {
                return ImmutableArray<IPropertySymbol>.Empty;
            }
            Binder binder = binderOpt ?? GetEnclosingBinder(GetAdjustedNodePosition(boundNode.Syntax));
            ArrayBuilder<ISymbol> instance = ArrayBuilder<ISymbol>.GetInstance();
            AppendSymbolsWithNameAndArity(instance, "this[]", 0, binder, type, LookupOptions.MustBeInstance);
            if (instance.Count == 0)
            {
                instance.Free();
                return ImmutableArray<IPropertySymbol>.Empty;
            }
            return FilterOverriddenOrHiddenIndexers(instance.ToImmutableAndFree());
        }

        private static ImmutableArray<IPropertySymbol> FilterOverriddenOrHiddenIndexers(ImmutableArray<ISymbol> symbols)
        {
            PooledHashSet<Symbol> pooledHashSet = null;
            ImmutableArray<ISymbol>.Enumerator enumerator = symbols.GetEnumerator();
            while (enumerator.MoveNext())
            {
                OverriddenOrHiddenMembersResult overriddenOrHiddenMembers = ((Microsoft.CodeAnalysis.CSharp.Symbols.PropertySymbol)enumerator.Current.GetSymbol()).OverriddenOrHiddenMembers;
                ImmutableArray<Symbol>.Enumerator enumerator2 = overriddenOrHiddenMembers.OverriddenMembers.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Symbol current = enumerator2.Current;
                    if (pooledHashSet == null)
                    {
                        pooledHashSet = PooledHashSet<Symbol>.GetInstance();
                    }
                    pooledHashSet.Add(current);
                }
                enumerator2 = overriddenOrHiddenMembers.HiddenMembers.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Symbol current2 = enumerator2.Current;
                    if (pooledHashSet == null)
                    {
                        pooledHashSet = PooledHashSet<Symbol>.GetInstance();
                    }
                    pooledHashSet.Add(current2);
                }
            }
            ArrayBuilder<IPropertySymbol> instance = ArrayBuilder<IPropertySymbol>.GetInstance();
            enumerator = symbols.GetEnumerator();
            while (enumerator.MoveNext())
            {
                IPropertySymbol propertySymbol = (IPropertySymbol)enumerator.Current;
                if (pooledHashSet == null || !pooledHashSet.Contains(propertySymbol.GetSymbol()))
                {
                    instance.Add(propertySymbol);
                }
            }
            pooledHashSet?.Free();
            return instance.ToImmutableAndFree();
        }

        private static ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol> FilterOverriddenOrHiddenMethods(ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol> methods)
        {
            if (methods.Length <= 1)
            {
                return methods;
            }
            HashSet<Symbol> hashSet = new HashSet<Symbol>();
            ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol>.Enumerator enumerator = methods.GetEnumerator();
            while (enumerator.MoveNext())
            {
                OverriddenOrHiddenMembersResult overriddenOrHiddenMembers = enumerator.Current.OverriddenOrHiddenMembers;
                ImmutableArray<Symbol>.Enumerator enumerator2 = overriddenOrHiddenMembers.OverriddenMembers.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Symbol current = enumerator2.Current;
                    hashSet.Add(current);
                }
                enumerator2 = overriddenOrHiddenMembers.HiddenMembers.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Symbol current2 = enumerator2.Current;
                    hashSet.Add(current2);
                }
            }
            return methods.WhereAsArray((Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol m, HashSet<Symbol> hiddenSymbols) => !hiddenSymbols.Contains(m), hashSet);
        }

        // Get the symbols and possible method group associated with a method group bound node, as
        // they should be exposed through GetSemanticInfo.
        // NB: It is not safe to pass a null binderOpt during speculative binding.
        // 
        // If the parent node of the method group syntax node provides information (such as arguments) 
        // that allows us to return more specific symbols (a specific overload or applicable candidates)
        // we return these. The complete set of symbols of the method group is then returned in methodGroup parameter.
        private ImmutableArray<Symbol> GetMethodGroupSemanticSymbols(
            BoundMethodGroup boundNode,
            BoundNode boundNodeForSyntacticParent,
            Binder binderOpt,
            out LookupResultKind resultKind,
            out bool isDynamic,
            out ImmutableArray<Symbol> methodGroup)
        {
            Debug.Assert(binderOpt != null || IsInTree(boundNode.Syntax));

            ImmutableArray<Symbol> symbols = ImmutableArray<Symbol>.Empty;

            resultKind = boundNode.ResultKind;
            if (resultKind == LookupResultKind.Empty)
            {
                resultKind = LookupResultKind.Viable;
            }

            isDynamic = false;

            // The method group needs filtering.
            Binder binder = binderOpt ?? GetEnclosingBinder(GetAdjustedNodePosition(boundNode.Syntax));
            methodGroup = GetReducedAndFilteredMethodGroupSymbols(binder, boundNode).Cast<Symbols.MethodSymbol, Symbol>();

            // We want to get the actual node chosen by overload resolution, if possible. 
            if (boundNodeForSyntacticParent != null)
            {
                switch (boundNodeForSyntacticParent.Kind)
                {
                    case BoundKind.Call:
                        // If we are looking for info on M in M(args), we want the symbol that overload resolution
                        // chose for M.
                        var call = (BoundCall)boundNodeForSyntacticParent;
                        InvocationExpressionSyntax invocation = call.Syntax as InvocationExpressionSyntax;
                        if (invocation != null && invocation.Expression.SkipParens() == ((ExpressionSyntax)boundNode.Syntax).SkipParens() && (object)call.Method != null)
                        {
                            if (call.OriginalMethodsOpt.IsDefault)
                            {
                                // Overload resolution succeeded.
                                symbols = CreateReducedExtensionMethodIfPossible(call);
                                resultKind = LookupResultKind.Viable;
                            }
                            else
                            {
                                resultKind = call.ResultKind.WorseResultKind(LookupResultKind.OverloadResolutionFailure);
                                symbols = StaticCast<Symbol>.From(CreateReducedExtensionMethodsFromOriginalsIfNecessary(call, Compilation));
                            }
                        }
                        break;

                    case BoundKind.DelegateCreationExpression:
                        // If we are looking for info on "M" in "new Action(M)" 
                        // we want to get the symbol that overload resolution chose for M, not the whole method group M.
                        var delegateCreation = (BoundDelegateCreationExpression)boundNodeForSyntacticParent;
                        if (delegateCreation.Argument == boundNode && (object)delegateCreation.MethodOpt != null)
                        {
                            symbols = CreateReducedExtensionMethodIfPossible(delegateCreation, boundNode.ReceiverOpt);
                        }
                        break;

                    case BoundKind.Conversion:
                        // If we are looking for info on "M" in "(Action)M" 
                        // we want to get the symbol that overload resolution chose for M, not the whole method group M.
                        var conversion = (BoundConversion)boundNodeForSyntacticParent;

                        var method = conversion.SymbolOpt;
                        if ((object)method != null)
                        {
                            Debug.Assert(conversion.ConversionKind == ConversionKind.MethodGroup);

                            if (conversion.IsExtensionMethod)
                            {
                                method = ReducedExtensionMethodSymbol.Create(method);
                            }

                            symbols = ImmutableArray.Create((Symbol)method);
                            resultKind = conversion.ResultKind;
                        }
                        else
                        {
                            goto default;
                        }

                        break;

                    case BoundKind.DynamicInvocation:
                        var dynamicInvocation = (BoundDynamicInvocation)boundNodeForSyntacticParent;
                        symbols = dynamicInvocation.ApplicableMethods.Cast<Symbols.MethodSymbol, Symbol>();
                        isDynamic = true;
                        break;

                    case BoundKind.BadExpression:
                        // If the bad expression has symbol(s) from this method group, it better indicates any problems.
                        ImmutableArray<Symbol> myMethodGroup = methodGroup;

                        symbols = ((BoundBadExpression)boundNodeForSyntacticParent).Symbols.WhereAsArray((sym, myMethodGroup) => myMethodGroup.Contains(sym), myMethodGroup);
                        if (symbols.Any())
                        {
                            resultKind = ((BoundBadExpression)boundNodeForSyntacticParent).ResultKind;
                        }
                        break;

                    case BoundKind.NameOfOperator:
                        symbols = methodGroup;
                        resultKind = resultKind.WorseResultKind(LookupResultKind.MemberGroup);
                        break;

                    default:
                        symbols = methodGroup;
                        if (symbols.Length > 0)
                        {
                            resultKind = resultKind.WorseResultKind(LookupResultKind.OverloadResolutionFailure);
                        }
                        break;
                }
            }
            else if (methodGroup.Length == 1 && !boundNode.HasAnyErrors)
            {
                // During speculative binding, there won't be a parent bound node. The parent bound
                // node may also be absent if the syntactic parent has errors or if one is simply
                // not specified (see SemanticModel.GetSymbolInfoForNode). However, if there's exactly
                // one candidate, then we should probably succeed.

                symbols = methodGroup;
                if (symbols.Length > 0)
                {
                    resultKind = resultKind.WorseResultKind(LookupResultKind.OverloadResolutionFailure);
                }
            }

            if (!symbols.Any())
            {
                // If we didn't find a better set of symbols, then assume this is a method group that didn't
                // get resolved. Return all members of the method group, with a resultKind of OverloadResolutionFailure
                // (unless the method group already has a worse result kind).
                symbols = methodGroup;
                if (!isDynamic && resultKind > LookupResultKind.OverloadResolutionFailure)
                {
                    resultKind = LookupResultKind.OverloadResolutionFailure;
                }
            }

            return symbols;
        }

        private ImmutableArray<Symbol> GetPropertyGroupSemanticSymbols(BoundPropertyGroup boundNode, BoundNode boundNodeForSyntacticParent, Binder binderOpt, out LookupResultKind resultKind, out ImmutableArray<Symbol> propertyGroup)
        {
            ImmutableArray<Symbol> immutableArray = ImmutableArray<Symbol>.Empty;
            resultKind = boundNode.ResultKind;
            if (resultKind == LookupResultKind.Empty)
            {
                resultKind = LookupResultKind.Viable;
            }
            propertyGroup = boundNode.Properties.Cast<Microsoft.CodeAnalysis.CSharp.Symbols.PropertySymbol, Symbol>();
            if (boundNodeForSyntacticParent != null)
            {
                switch (boundNodeForSyntacticParent.Kind)
                {
                    case BoundKind.IndexerAccess:
                        {
                            BoundIndexerAccess boundIndexerAccess = (BoundIndexerAccess)boundNodeForSyntacticParent;
                            if (boundIndexerAccess.Syntax is ElementAccessExpressionSyntax elementAccessExpressionSyntax && elementAccessExpressionSyntax.Expression == boundNode.Syntax && (object)boundIndexerAccess.Indexer != null)
                            {
                                if (boundIndexerAccess.OriginalIndexersOpt.IsDefault)
                                {
                                    immutableArray = ImmutableArray.Create((Symbol)boundIndexerAccess.Indexer);
                                    resultKind = LookupResultKind.Viable;
                                }
                                else
                                {
                                    resultKind = boundIndexerAccess.ResultKind.WorseResultKind(LookupResultKind.OverloadResolutionFailure);
                                    immutableArray = StaticCast<Symbol>.From(boundIndexerAccess.OriginalIndexersOpt);
                                }
                            }
                            break;
                        }
                    case BoundKind.BadExpression:
                        {
                            ImmutableArray<Symbol> arg = propertyGroup;
                            immutableArray = ((BoundBadExpression)boundNodeForSyntacticParent).Symbols.WhereAsArray<Symbol, ImmutableArray<Symbol>>((Symbol sym, ImmutableArray<Symbol> myPropertyGroup) => myPropertyGroup.Contains(sym), arg);
                            if (immutableArray.Any())
                            {
                                resultKind = ((BoundBadExpression)boundNodeForSyntacticParent).ResultKind;
                            }
                            break;
                        }
                }
            }
            else if (propertyGroup.Length == 1 && !boundNode.HasAnyErrors)
            {
                immutableArray = propertyGroup;
            }
            if (!immutableArray.Any())
            {
                immutableArray = propertyGroup;
                if ((int)resultKind > 12)
                {
                    resultKind = LookupResultKind.OverloadResolutionFailure;
                }
            }
            return immutableArray;
        }

        private SymbolInfo GetNamedArgumentSymbolInfo(IdentifierNameSyntax identifierNameSyntax, CancellationToken cancellationToken)
        {
            string valueText = identifierNameSyntax.Identifier.ValueText;
            if (valueText.Length == 0)
            {
                return SymbolInfo.None;
            }
            CSharpSyntaxNode parent = identifierNameSyntax.Parent!.Parent!.Parent;
            if (parent.IsKind(SyntaxKind.TupleExpression))
            {
                ArgumentSyntax declaratorSyntax = (ArgumentSyntax)identifierNameSyntax.Parent!.Parent;
                ISymbol declaredSymbol = GetDeclaredSymbol(declaratorSyntax, cancellationToken);
                if (declaredSymbol != null)
                {
                    return new SymbolInfo(declaredSymbol, ImmutableArray<ISymbol>.Empty, CandidateReason.None);
                }
                return SymbolInfo.None;
            }
            if (parent.IsKind(SyntaxKind.PropertyPatternClause) || parent.IsKind(SyntaxKind.PositionalPatternClause))
            {
                return GetSymbolInfoWorker(identifierNameSyntax, SymbolInfoOptions.DefaultOptions, cancellationToken);
            }
            CSharpSyntaxNode parent2 = parent.Parent;
            SymbolInfo symbolInfoWorker = GetSymbolInfoWorker(parent2, SymbolInfoOptions.DefaultOptions, cancellationToken);
            if (symbolInfoWorker.Symbol != null)
            {
                Microsoft.CodeAnalysis.CSharp.Symbols.ParameterSymbol parameterSymbol = FindNamedParameter(symbolInfoWorker.Symbol.GetSymbol().GetParameters(), valueText);
                if ((object)parameterSymbol != null)
                {
                    return new SymbolInfo(parameterSymbol.GetPublicSymbol(), ImmutableArray<ISymbol>.Empty, CandidateReason.None);
                }
                return SymbolInfo.None;
            }
            ArrayBuilder<ISymbol> instance = ArrayBuilder<ISymbol>.GetInstance();
            ImmutableArray<ISymbol>.Enumerator enumerator = symbolInfoWorker.CandidateSymbols.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ISymbol current = enumerator.Current;
                SymbolKind kind = current.Kind;
                if (kind == SymbolKind.Method || kind == SymbolKind.Property)
                {
                    Microsoft.CodeAnalysis.CSharp.Symbols.ParameterSymbol parameterSymbol2 = FindNamedParameter(current.GetSymbol().GetParameters(), valueText);
                    if ((object)parameterSymbol2 != null)
                    {
                        instance.Add(parameterSymbol2.GetPublicSymbol());
                    }
                }
            }
            if (instance.Count == 0)
            {
                instance.Free();
                return SymbolInfo.None;
            }
            return new SymbolInfo(null, instance.ToImmutableAndFree(), symbolInfoWorker.CandidateReason);
        }

        private static Microsoft.CodeAnalysis.CSharp.Symbols.ParameterSymbol FindNamedParameter(ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbols.ParameterSymbol> parameters, string argumentName)
        {
            ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbols.ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Microsoft.CodeAnalysis.CSharp.Symbols.ParameterSymbol current = enumerator.Current;
                if (current.Name == argumentName)
                {
                    return current;
                }
            }
            return null;
        }

        internal static ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol> GetReducedAndFilteredMethodGroupSymbols(Binder binder, BoundMethodGroup node)
        {
            ArrayBuilder<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol> instance = ArrayBuilder<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol>.GetInstance();
            ArrayBuilder<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol> instance2 = ArrayBuilder<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol>.GetInstance();
            LookupResultKind resultKind = LookupResultKind.Empty;
            ImmutableArray<TypeWithAnnotations> typeArgumentsOpt = node.TypeArgumentsOpt;
            if (node.Methods.Any())
            {
                ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol>.Enumerator enumerator = FilterOverriddenOrHiddenMethods(node.Methods).GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol current = enumerator.Current;
                    MergeReducedAndFilteredMethodGroupSymbol(instance, instance2, new SingleLookupResult(node.ResultKind, current, node.LookupError), typeArgumentsOpt, null, ref resultKind, binder.Compilation);
                }
            }
            else
            {
                Symbol lookupSymbolOpt = node.LookupSymbolOpt;
                if ((object)lookupSymbolOpt != null && lookupSymbolOpt.Kind == SymbolKind.Method)
                {
                    MergeReducedAndFilteredMethodGroupSymbol(instance, instance2, new SingleLookupResult(node.ResultKind, lookupSymbolOpt, node.LookupError), typeArgumentsOpt, null, ref resultKind, binder.Compilation);
                }
            }
            BoundExpression receiverOpt = node.ReceiverOpt;
            string name = node.Name;
            if (node.SearchExtensionMethods)
            {
                int arity;
                LookupOptions options;
                if (typeArgumentsOpt.IsDefault)
                {
                    arity = 0;
                    options = LookupOptions.AllMethodsOnArityZero;
                }
                else
                {
                    arity = typeArgumentsOpt.Length;
                    options = LookupOptions.Default;
                }
                binder = binder.WithAdditionalFlags(BinderFlags.SemanticModel);
                ExtensionMethodScopeEnumerator enumerator2 = new ExtensionMethodScopes(binder).GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    ExtensionMethodScope current2 = enumerator2.Current;
                    ArrayBuilder<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol> instance3 = ArrayBuilder<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol>.GetInstance();
                    current2.Binder.GetCandidateExtensionMethods(instance3, name, arity, options, binder);
                    ArrayBuilder<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol>.Enumerator enumerator3 = instance3.GetEnumerator();
                    while (enumerator3.MoveNext())
                    {
                        Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol current3 = enumerator3.Current;
                        CompoundUseSiteInfo<Microsoft.CodeAnalysis.CSharp.Symbols.AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<Microsoft.CodeAnalysis.CSharp.Symbols.AssemblySymbol>.Discarded;
                        MergeReducedAndFilteredMethodGroupSymbol(instance, instance2, binder.CheckViability(current3, arity, options, null, diagnose: false, ref useSiteInfo), typeArgumentsOpt, receiverOpt.Type, ref resultKind, binder.Compilation);
                    }
                    instance3.Free();
                }
            }
            instance.Free();
            return instance2.ToImmutableAndFree();
        }

        private static bool AddReducedAndFilteredMethodGroupSymbol(ArrayBuilder<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol> methods, ArrayBuilder<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol> filteredMethods, Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol method, ImmutableArray<TypeWithAnnotations> typeArguments, Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol receiverType, CSharpCompilation compilation)
        {
            Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol methodSymbol = ((typeArguments.IsDefaultOrEmpty || method.Arity != typeArguments.Length) ? method : method.Construct(typeArguments));
            if ((object)receiverType != null)
            {
                methodSymbol = methodSymbol.ReduceExtensionMethod(receiverType, compilation);
                if ((object)methodSymbol == null)
                {
                    return false;
                }
            }
            if (filteredMethods.Contains(methodSymbol))
            {
                return false;
            }
            methods.Add(method);
            filteredMethods.Add(methodSymbol);
            return true;
        }

        private static void MergeReducedAndFilteredMethodGroupSymbol(ArrayBuilder<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol> methods, ArrayBuilder<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol> filteredMethods, SingleLookupResult singleResult, ImmutableArray<TypeWithAnnotations> typeArguments, Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol receiverType, ref LookupResultKind resultKind, CSharpCompilation compilation)
        {
            LookupResultKind kind = singleResult.Kind;
            if ((int)resultKind <= (int)kind)
            {
                if ((int)resultKind < (int)kind)
                {
                    methods.Clear();
                    filteredMethods.Clear();
                    resultKind = LookupResultKind.Empty;
                }
                Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol method = (Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol)singleResult.Symbol;
                if (AddReducedAndFilteredMethodGroupSymbol(methods, filteredMethods, method, typeArguments, receiverType, compilation) && (int)resultKind < (int)kind)
                {
                    resultKind = kind;
                }
            }
        }

        private static ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol> CreateReducedExtensionMethodsFromOriginalsIfNecessary(BoundCall call, CSharpCompilation compilation)
        {
            ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol> originalMethodsOpt = call.OriginalMethodsOpt;
            Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol receiverType = null;
            if (call.InvokedAsExtensionMethod)
            {
                receiverType = ((call.ReceiverOpt == null) ? call.Arguments[0].Type : call.ReceiverOpt!.Type);
            }
            ArrayBuilder<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol> instance = ArrayBuilder<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol>.GetInstance();
            ArrayBuilder<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol> instance2 = ArrayBuilder<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol>.GetInstance();
            ImmutableArray<Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol>.Enumerator enumerator = FilterOverriddenOrHiddenMethods(originalMethodsOpt).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol current = enumerator.Current;
                AddReducedAndFilteredMethodGroupSymbol(instance, instance2, current, default(ImmutableArray<TypeWithAnnotations>), receiverType, compilation);
            }
            instance.Free();
            return instance2.ToImmutableAndFree();
        }

        private ImmutableArray<Symbol> CreateReducedExtensionMethodIfPossible(BoundCall call)
        {
            Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol methodSymbol = call.Method;
            if (call.InvokedAsExtensionMethod && methodSymbol.IsExtensionMethod && methodSymbol.MethodKind != MethodKind.ReducedExtension)
            {
                BoundExpression boundExpression = call.Arguments[0];
                methodSymbol = methodSymbol.ReduceExtensionMethod(boundExpression.Type, Compilation) ?? methodSymbol;
            }
            return ImmutableArray.Create((Symbol)methodSymbol);
        }

        private ImmutableArray<Symbol> CreateReducedExtensionMethodIfPossible(BoundDelegateCreationExpression delegateCreation, BoundExpression receiverOpt)
        {
            Microsoft.CodeAnalysis.CSharp.Symbols.MethodSymbol methodSymbol = delegateCreation.MethodOpt;
            if (delegateCreation.IsExtensionMethod && methodSymbol.IsExtensionMethod && receiverOpt != null)
            {
                methodSymbol = methodSymbol.ReduceExtensionMethod(receiverOpt.Type, Compilation) ?? methodSymbol;
            }
            return ImmutableArray.Create((Symbol)methodSymbol);
        }

        public abstract ForEachStatementInfo GetForEachStatementInfo(ForEachStatementSyntax node);

        public abstract ForEachStatementInfo GetForEachStatementInfo(CommonForEachStatementSyntax node);

        public abstract DeconstructionInfo GetDeconstructionInfo(AssignmentExpressionSyntax node);

        public abstract DeconstructionInfo GetDeconstructionInfo(ForEachVariableStatementSyntax node);

        public abstract AwaitExpressionInfo GetAwaitExpressionInfo(AwaitExpressionSyntax node);

        public PreprocessingSymbolInfo GetPreprocessingSymbolInfo(IdentifierNameSyntax node)
        {
            CheckSyntaxNode(node);
            if (node.Ancestors().Any((SyntaxNode n) => SyntaxFacts.IsPreprocessorDirective(n.Kind())))
            {
                bool isDefined = SyntaxTree.IsPreprocessorSymbolDefined(node.Identifier.ValueText, node.Identifier.SpanStart);
                return new PreprocessingSymbolInfo(new PreprocessingSymbol(node.Identifier.ValueText), isDefined);
            }
            return PreprocessingSymbolInfo.None;
        }

        internal static void ValidateSymbolInfoOptions(SymbolInfoOptions options)
        {
        }

        public new ISymbol GetEnclosingSymbol(int position, CancellationToken cancellationToken = default(CancellationToken))
        {
            position = CheckAndAdjustPosition(position);
            return GetEnclosingBinder(position)?.ContainingMemberOrLambda.GetPublicSymbol();
        }

        private SymbolInfo GetSymbolInfoFromNode(SyntaxNode node, CancellationToken cancellationToken)
        {
            if (node != null)
            {
                if (node is not ExpressionSyntax expression)
                {
                    if (node is not ConstructorInitializerSyntax constructorInitializer)
                    {
                        if (node is not PrimaryConstructorBaseTypeSyntax constructorInitializer2)
                        {
                            if (node is not AttributeSyntax attributeSyntax)
                            {
                                if (node is not CrefSyntax crefSyntax)
                                {
                                    if (node is not SelectOrGroupClauseSyntax node2)
                                    {
                                        if (node is not OrderingSyntax node3)
                                        {
                                            if (node is PositionalPatternClauseSyntax node4)
                                            {
                                                return GetSymbolInfo(node4, cancellationToken);
                                            }
                                            return SymbolInfo.None;
                                        }
                                        return GetSymbolInfo(node3, cancellationToken);
                                    }
                                    return GetSymbolInfo(node2, cancellationToken);
                                }
                                return GetSymbolInfo(crefSyntax, cancellationToken);
                            }
                            return GetSymbolInfo(attributeSyntax, cancellationToken);
                        }
                        return GetSymbolInfo(constructorInitializer2, cancellationToken);
                    }
                    return GetSymbolInfo(constructorInitializer, cancellationToken);
                }
                return GetSymbolInfo(expression, cancellationToken);
            }
            throw new ArgumentNullException("node");
        }

        private TypeInfo GetTypeInfoFromNode(SyntaxNode node, CancellationToken cancellationToken)
        {
            if (node != null)
            {
                if (node is not ExpressionSyntax expression)
                {
                    if (node is not ConstructorInitializerSyntax constructorInitializer)
                    {
                        if (node is not AttributeSyntax attributeSyntax)
                        {
                            if (node is not SelectOrGroupClauseSyntax node2)
                            {
                                if (node is PatternSyntax pattern)
                                {
                                    return GetTypeInfo(pattern, cancellationToken);
                                }
                                return CSharpTypeInfo.None;
                            }
                            return GetTypeInfo(node2, cancellationToken);
                        }
                        return GetTypeInfo(attributeSyntax, cancellationToken);
                    }
                    return GetTypeInfo(constructorInitializer, cancellationToken);
                }
                return GetTypeInfo(expression, cancellationToken);
            }
            throw new ArgumentNullException("node");
        }

        private ImmutableArray<ISymbol> GetMemberGroupFromNode(SyntaxNode node, CancellationToken cancellationToken)
        {
            if (node != null)
            {
                if (node is not ExpressionSyntax expression)
                {
                    if (node is not ConstructorInitializerSyntax initializer)
                    {
                        if (node is AttributeSyntax attribute)
                        {
                            return GetMemberGroup(attribute, cancellationToken);
                        }
                        return ImmutableArray<ISymbol>.Empty;
                    }
                    return GetMemberGroup(initializer, cancellationToken);
                }
                return GetMemberGroup(expression, cancellationToken);
            }
            throw new ArgumentNullException("node");
        }

        protected sealed override ImmutableArray<ISymbol> GetMemberGroupCore(SyntaxNode node, CancellationToken cancellationToken)
        {
            return StaticCast<ISymbol>.From(GetMemberGroupFromNode(node, cancellationToken));
        }

        protected sealed override SymbolInfo GetSpeculativeSymbolInfoCore(int position, SyntaxNode node, SpeculativeBindingOption bindingOption)
        {
            if (!(node is ExpressionSyntax expression))
            {
                if (!(node is ConstructorInitializerSyntax constructorInitializer))
                {
                    if (!(node is PrimaryConstructorBaseTypeSyntax constructorInitializer2))
                    {
                        if (!(node is AttributeSyntax attribute))
                        {
                            if (node is CrefSyntax cref)
                            {
                                return GetSpeculativeSymbolInfo(position, cref);
                            }
                            return SymbolInfo.None;
                        }
                        return GetSpeculativeSymbolInfo(position, attribute);
                    }
                    return GetSpeculativeSymbolInfo(position, constructorInitializer2);
                }
                return GetSpeculativeSymbolInfo(position, constructorInitializer);
            }
            return GetSpeculativeSymbolInfo(position, expression, bindingOption);
        }

        protected sealed override TypeInfo GetSpeculativeTypeInfoCore(int position, SyntaxNode node, SpeculativeBindingOption bindingOption)
        {
            if (!(node is ExpressionSyntax expression))
            {
                return CSharpTypeInfo.None;
            }
            return GetSpeculativeTypeInfo(position, expression, bindingOption);
        }

        protected sealed override IAliasSymbol GetSpeculativeAliasInfoCore(int position, SyntaxNode nameSyntax, SpeculativeBindingOption bindingOption)
        {
            if (!(nameSyntax is IdentifierNameSyntax nameSyntax2))
            {
                return null;
            }
            return GetSpeculativeAliasInfo(position, nameSyntax2, bindingOption);
        }

        protected sealed override SymbolInfo GetSymbolInfoCore(SyntaxNode node, CancellationToken cancellationToken)
        {
            return GetSymbolInfoFromNode(node, cancellationToken);
        }

        protected sealed override TypeInfo GetTypeInfoCore(SyntaxNode node, CancellationToken cancellationToken)
        {
            return GetTypeInfoFromNode(node, cancellationToken);
        }

        protected sealed override IAliasSymbol GetAliasInfoCore(SyntaxNode node, CancellationToken cancellationToken)
        {
            if (!(node is IdentifierNameSyntax nameSyntax))
            {
                return null;
            }
            return GetAliasInfo(nameSyntax, cancellationToken);
        }

        protected sealed override PreprocessingSymbolInfo GetPreprocessingSymbolInfoCore(SyntaxNode node)
        {
            if (!(node is IdentifierNameSyntax node2))
            {
                return PreprocessingSymbolInfo.None;
            }
            return GetPreprocessingSymbolInfo(node2);
        }

        protected sealed override ISymbol GetDeclaredSymbolCore(SyntaxNode node, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (node is not AccessorDeclarationSyntax declarationSyntax)
            {
                if (node is not BaseTypeDeclarationSyntax declarationSyntax2)
                {
                    if (node is not QueryClauseSyntax queryClause)
                    {
                        if (node is MemberDeclarationSyntax declarationSyntax3)
                        {
                            return GetDeclaredSymbol(declarationSyntax3, cancellationToken);
                        }
                        switch (node.Kind())
                        {
                            case SyntaxKind.LocalFunctionStatement:
                                return GetDeclaredSymbol((LocalFunctionStatementSyntax)node, cancellationToken);
                            case SyntaxKind.LabeledStatement:
                                return GetDeclaredSymbol((LabeledStatementSyntax)node, cancellationToken);
                            case SyntaxKind.CaseSwitchLabel:
                            case SyntaxKind.DefaultSwitchLabel:
                                return GetDeclaredSymbol((SwitchLabelSyntax)node, cancellationToken);
                            case SyntaxKind.AnonymousObjectCreationExpression:
                                return GetDeclaredSymbol((AnonymousObjectCreationExpressionSyntax)node, cancellationToken);
                            case SyntaxKind.AnonymousObjectMemberDeclarator:
                                return GetDeclaredSymbol((AnonymousObjectMemberDeclaratorSyntax)node, cancellationToken);
                            case SyntaxKind.TupleExpression:
                                return GetDeclaredSymbol((TupleExpressionSyntax)node, cancellationToken);
                            case SyntaxKind.Argument:
                                return GetDeclaredSymbol((ArgumentSyntax)node, cancellationToken);
                            case SyntaxKind.VariableDeclarator:
                                return GetDeclaredSymbol((VariableDeclaratorSyntax)node, cancellationToken);
                            case SyntaxKind.SingleVariableDesignation:
                                return GetDeclaredSymbol((SingleVariableDesignationSyntax)node, cancellationToken);
                            case SyntaxKind.TupleElement:
                                return GetDeclaredSymbol((TupleElementSyntax)node, cancellationToken);
                            case SyntaxKind.NamespaceDeclaration:
                                return GetDeclaredSymbol((NamespaceDeclarationSyntax)node, cancellationToken);
                            case SyntaxKind.Parameter:
                                return GetDeclaredSymbol((ParameterSyntax)node, cancellationToken);
                            case SyntaxKind.TypeParameter:
                                return GetDeclaredSymbol((TypeParameterSyntax)node, cancellationToken);
                            case SyntaxKind.UsingDirective:
                                {
                                    UsingDirectiveSyntax usingDirectiveSyntax = (UsingDirectiveSyntax)node;
                                    if (usingDirectiveSyntax.Alias != null)
                                    {
                                        return GetDeclaredSymbol(usingDirectiveSyntax, cancellationToken);
                                    }
                                    break;
                                }
                            case SyntaxKind.ForEachStatement:
                                return GetDeclaredSymbol((ForEachStatementSyntax)node, cancellationToken);
                            case SyntaxKind.CatchDeclaration:
                                return GetDeclaredSymbol((CatchDeclarationSyntax)node, cancellationToken);
                            case SyntaxKind.JoinIntoClause:
                                return GetDeclaredSymbol((JoinIntoClauseSyntax)node, cancellationToken);
                            case SyntaxKind.QueryContinuation:
                                return GetDeclaredSymbol((QueryContinuationSyntax)node, cancellationToken);
                            case SyntaxKind.CompilationUnit:
                                return GetDeclaredSymbol((CompilationUnitSyntax)node, cancellationToken);
                        }
                        return null;
                    }
                    return GetDeclaredSymbol(queryClause, cancellationToken);
                }
                return GetDeclaredSymbol(declarationSyntax2, cancellationToken);
            }
            return GetDeclaredSymbol(declarationSyntax, cancellationToken);
        }

        public ISymbol GetDeclaredSymbol(TupleElementSyntax declarationSyntax, CancellationToken cancellationToken = default(CancellationToken))
        {
            CheckSyntaxNode(declarationSyntax);
            if (declarationSyntax.Parent is TupleTypeSyntax tupleTypeSyntax)
            {
                return (GetSymbolInfo(tupleTypeSyntax, cancellationToken).Symbol.GetSymbol() as Microsoft.CodeAnalysis.CSharp.Symbols.NamedTypeSymbol)?.TupleElements.ElementAtOrDefault(tupleTypeSyntax.Elements.IndexOf(declarationSyntax)).GetPublicSymbol();
            }
            return null;
        }

        protected sealed override ImmutableArray<ISymbol> GetDeclaredSymbolsCore(SyntaxNode declaration, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (declaration is BaseFieldDeclarationSyntax declarationSyntax)
            {
                return GetDeclaredSymbols(declarationSyntax, cancellationToken);
            }
            ISymbol declaredSymbolCore = GetDeclaredSymbolCore(declaration, cancellationToken);
            if (declaredSymbolCore != null)
            {
                return ImmutableArray.Create(declaredSymbolCore);
            }
            return ImmutableArray.Create<ISymbol>();
        }

        public override void ComputeDeclarationsInSpan(TextSpan span, bool getSymbol, ArrayBuilder<DeclarationInfo> builder, CancellationToken cancellationToken)
        {
            CSharpDeclarationComputer.ComputeDeclarationsInSpan(this, span, getSymbol, builder, cancellationToken);
        }

        public override void ComputeDeclarationsInNode(SyntaxNode node, ISymbol associatedSymbol, bool getSymbol, ArrayBuilder<DeclarationInfo> builder, CancellationToken cancellationToken, int? levelsToCompute = null)
        {
            CSharpDeclarationComputer.ComputeDeclarationsInNode(this, associatedSymbol, node, getSymbol, builder, cancellationToken, levelsToCompute);
        }

        public abstract override Func<SyntaxNode, bool> GetSyntaxNodesToAnalyzeFilter(SyntaxNode declaredNode, ISymbol declaredSymbol);

        public override SyntaxNode GetTopmostNodeForDiagnosticAnalysis(ISymbol symbol, SyntaxNode declaringSyntax)
        {
            SymbolKind kind = symbol.Kind;
            if ((uint)(kind - 5) <= 1u)
            {
                BaseFieldDeclarationSyntax baseFieldDeclarationSyntax = declaringSyntax.FirstAncestorOrSelf<BaseFieldDeclarationSyntax>();
                if (baseFieldDeclarationSyntax != null)
                {
                    return baseFieldDeclarationSyntax;
                }
            }
            return declaringSyntax;
        }

        protected sealed override ImmutableArray<ISymbol> LookupSymbolsCore(int position, INamespaceOrTypeSymbol container, string name, bool includeReducedExtensionMethods)
        {
            return LookupSymbols(position, container.EnsureCSharpSymbolOrNull("container"), name, includeReducedExtensionMethods);
        }

        protected sealed override ImmutableArray<ISymbol> LookupBaseMembersCore(int position, string name)
        {
            return LookupBaseMembers(position, name);
        }

        protected sealed override ImmutableArray<ISymbol> LookupStaticMembersCore(int position, INamespaceOrTypeSymbol container, string name)
        {
            return LookupStaticMembers(position, container.EnsureCSharpSymbolOrNull("container"), name);
        }

        protected sealed override ImmutableArray<ISymbol> LookupNamespacesAndTypesCore(int position, INamespaceOrTypeSymbol container, string name)
        {
            return LookupNamespacesAndTypes(position, container.EnsureCSharpSymbolOrNull("container"), name);
        }

        protected sealed override ImmutableArray<ISymbol> LookupLabelsCore(int position, string name)
        {
            return LookupLabels(position, name);
        }

        protected sealed override ControlFlowAnalysis AnalyzeControlFlowCore(SyntaxNode firstStatement, SyntaxNode lastStatement)
        {
            if (firstStatement == null)
            {
                throw new ArgumentNullException("firstStatement");
            }
            if (lastStatement == null)
            {
                throw new ArgumentNullException("lastStatement");
            }
            if (firstStatement is not StatementSyntax firstStatement2)
            {
                throw new ArgumentException("firstStatement is not a StatementSyntax.");
            }
            if (lastStatement is not StatementSyntax lastStatement2)
            {
                throw new ArgumentException("firstStatement is a StatementSyntax but lastStatement isn't.");
            }
            return AnalyzeControlFlow(firstStatement2, lastStatement2);
        }

        protected sealed override ControlFlowAnalysis AnalyzeControlFlowCore(SyntaxNode statement)
        {
            if (statement == null)
            {
                throw new ArgumentNullException("statement");
            }
            if (statement is not StatementSyntax statement2)
            {
                throw new ArgumentException("statement is not a StatementSyntax.");
            }
            return AnalyzeControlFlow(statement2);
        }

        protected sealed override DataFlowAnalysis AnalyzeDataFlowCore(SyntaxNode firstStatement, SyntaxNode lastStatement)
        {
            if (firstStatement == null)
            {
                throw new ArgumentNullException("firstStatement");
            }
            if (lastStatement == null)
            {
                throw new ArgumentNullException("lastStatement");
            }
            if (firstStatement is not StatementSyntax firstStatement2)
            {
                throw new ArgumentException("firstStatement is not a StatementSyntax.");
            }
            if (lastStatement is not StatementSyntax lastStatement2)
            {
                throw new ArgumentException("lastStatement is not a StatementSyntax.");
            }
            return AnalyzeDataFlow(firstStatement2, lastStatement2);
        }

        protected sealed override DataFlowAnalysis AnalyzeDataFlowCore(SyntaxNode statementOrExpression)
        {
            if (statementOrExpression != null)
            {
                if (statementOrExpression is not StatementSyntax statement)
                {
                    if (statementOrExpression is ExpressionSyntax expression)
                    {
                        return AnalyzeDataFlow(expression);
                    }
                    throw new ArgumentException("statementOrExpression is not a StatementSyntax or an ExpressionSyntax.");
                }
                return AnalyzeDataFlow(statement);
            }
            throw new ArgumentNullException("statementOrExpression");
        }

        protected sealed override Optional<object> GetConstantValueCore(SyntaxNode node, CancellationToken cancellationToken)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }
            if (node is not ExpressionSyntax expression)
            {
                return default;
            }
            return GetConstantValue(expression, cancellationToken);
        }

        protected sealed override ISymbol GetEnclosingSymbolCore(int position, CancellationToken cancellationToken)
        {
            return GetEnclosingSymbol(position, cancellationToken);
        }

        protected sealed override bool IsAccessibleCore(int position, ISymbol symbol)
        {
            return IsAccessible(position, symbol.EnsureCSharpSymbolOrNull("symbol"));
        }

        protected sealed override bool IsEventUsableAsFieldCore(int position, IEventSymbol symbol)
        {
            return IsEventUsableAsField(position, symbol.EnsureCSharpSymbolOrNull("symbol"));
        }

        public sealed override NullableContext GetNullableContext(int position)
        {
            CSharpSyntaxTree obj = (CSharpSyntaxTree)Root.SyntaxTree;
            NullableContextState nullableContextState = obj.GetNullableContextState(position);
            NullableContextOptions context = ((!obj.IsGeneratedCode(Compilation.Options.SyntaxTreeOptionsProvider, CancellationToken.None)) ? Compilation.Options.NullableContextOptions : NullableContextOptions.Disable);
            return getFlag(nullableContextState.AnnotationsState, context.AnnotationsEnabled(), NullableContext.AnnotationsContextInherited, NullableContext.AnnotationsEnabled) | getFlag(nullableContextState.WarningsState, context.WarningsEnabled(), NullableContext.WarningsContextInherited, NullableContext.WarningsEnabled);
            static NullableContext getFlag(NullableContextState.State contextState, bool defaultEnableState, NullableContext inheritedFlag, NullableContext enableFlag)
            {
                switch (contextState)
                {
                    case NullableContextState.State.Enabled:
                        return enableFlag;
                    case NullableContextState.State.Disabled:
                        return NullableContext.Disabled;
                    default:
                        if (defaultEnableState)
                        {
                            return inheritedFlag | enableFlag;
                        }
                        return inheritedFlag;
                }
            }
        }
    }
}
