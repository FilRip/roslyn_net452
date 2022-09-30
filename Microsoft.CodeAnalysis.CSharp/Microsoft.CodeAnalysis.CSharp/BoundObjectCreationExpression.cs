using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Operations;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundObjectCreationExpression : BoundExpression, IBoundInvalidNode
    {
        public override ConstantValue? ConstantValue => ConstantValueOpt;

        public override Symbol ExpressionSymbol => Constructor;

        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren => CSharpOperationFactory.CreateInvalidChildrenFromArgumentsExpression(null, Arguments, InitializerExpressionOpt);

        public new TypeSymbol Type => base.Type;

        public MethodSymbol Constructor { get; }

        public ImmutableArray<MethodSymbol> ConstructorsGroup { get; }

        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<string> ArgumentNamesOpt { get; }

        public ImmutableArray<RefKind> ArgumentRefKindsOpt { get; }

        public bool Expanded { get; }

        public ImmutableArray<int> ArgsToParamsOpt { get; }

        public BitVector DefaultArguments { get; }

        public ConstantValue? ConstantValueOpt { get; }

        public BoundObjectInitializerExpressionBase? InitializerExpressionOpt { get; }

        public bool WasTargetTyped { get; }

        internal BoundObjectCreationExpression UpdateArgumentsAndInitializer(ImmutableArray<BoundExpression> newArguments, ImmutableArray<RefKind> newRefKinds, BoundObjectInitializerExpressionBase? newInitializerExpression, TypeSymbol? changeTypeOpt = null)
        {
            return Update(Constructor, newArguments, default(ImmutableArray<string>), newRefKinds, expanded: false, default(ImmutableArray<int>), default(BitVector), ConstantValueOpt, newInitializerExpression, changeTypeOpt ?? Type);
        }

        public BoundObjectCreationExpression(SyntaxNode syntax, MethodSymbol constructor, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, ConstantValue? constantValueOpt, BoundObjectInitializerExpressionBase? initializerExpressionOpt, TypeSymbol type, bool hasErrors = false)
            : this(syntax, constructor, ImmutableArray<MethodSymbol>.Empty, arguments, argumentNamesOpt, argumentRefKindsOpt, expanded, argsToParamsOpt, defaultArguments, constantValueOpt, initializerExpressionOpt, wasTargetTyped: false, type, hasErrors)
        {
        }

        public BoundObjectCreationExpression Update(MethodSymbol constructor, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, ConstantValue? constantValueOpt, BoundObjectInitializerExpressionBase? initializerExpressionOpt, TypeSymbol type)
        {
            return Update(constructor, ImmutableArray<MethodSymbol>.Empty, arguments, argumentNamesOpt, argumentRefKindsOpt, expanded, argsToParamsOpt, defaultArguments, constantValueOpt, initializerExpressionOpt, WasTargetTyped, type);
        }

        public BoundObjectCreationExpression Update(MethodSymbol constructor, ImmutableArray<MethodSymbol> constructorsGroup, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, ConstantValue? constantValueOpt, BoundObjectInitializerExpressionBase? initializerExpressionOpt, TypeSymbol type)
        {
            return Update(constructor, constructorsGroup, arguments, argumentNamesOpt, argumentRefKindsOpt, expanded, argsToParamsOpt, defaultArguments, constantValueOpt, initializerExpressionOpt, WasTargetTyped, type);
        }

        public BoundObjectCreationExpression(SyntaxNode syntax, MethodSymbol constructor, params BoundExpression[] arguments)
            : this(syntax, constructor, ImmutableArray.Create(arguments), default(ImmutableArray<string>), default(ImmutableArray<RefKind>), expanded: false, default(ImmutableArray<int>), default(BitVector), null, null, constructor.ContainingType)
        {
        }

        public BoundObjectCreationExpression(SyntaxNode syntax, MethodSymbol constructor, ImmutableArray<BoundExpression> arguments)
            : this(syntax, constructor, arguments, default(ImmutableArray<string>), default(ImmutableArray<RefKind>), expanded: false, default(ImmutableArray<int>), default(BitVector), null, null, constructor.ContainingType)
        {
        }

        internal static ImmutableArray<BoundExpression> GetChildInitializers(BoundExpression? objectOrCollectionInitializer)
        {
            if (objectOrCollectionInitializer is BoundObjectInitializerExpression boundObjectInitializerExpression)
            {
                return boundObjectInitializerExpression.Initializers;
            }
            if (objectOrCollectionInitializer is BoundCollectionInitializerExpression boundCollectionInitializerExpression)
            {
                return boundCollectionInitializerExpression.Initializers;
            }
            return ImmutableArray<BoundExpression>.Empty;
        }

        public BoundObjectCreationExpression(SyntaxNode syntax, MethodSymbol constructor, ImmutableArray<MethodSymbol> constructorsGroup, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, ConstantValue? constantValueOpt, BoundObjectInitializerExpressionBase? initializerExpressionOpt, bool wasTargetTyped, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.ObjectCreationExpression, syntax, type, hasErrors || arguments.HasErrors() || initializerExpressionOpt.HasErrors())
        {
            Constructor = constructor;
            ConstructorsGroup = constructorsGroup;
            Arguments = arguments;
            ArgumentNamesOpt = argumentNamesOpt;
            ArgumentRefKindsOpt = argumentRefKindsOpt;
            Expanded = expanded;
            ArgsToParamsOpt = argsToParamsOpt;
            DefaultArguments = defaultArguments;
            ConstantValueOpt = constantValueOpt;
            InitializerExpressionOpt = initializerExpressionOpt;
            WasTargetTyped = wasTargetTyped;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitObjectCreationExpression(this);
        }

        public BoundObjectCreationExpression Update(MethodSymbol constructor, ImmutableArray<MethodSymbol> constructorsGroup, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool expanded, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, ConstantValue? constantValueOpt, BoundObjectInitializerExpressionBase? initializerExpressionOpt, bool wasTargetTyped, TypeSymbol type)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(constructor, Constructor) || constructorsGroup != ConstructorsGroup || arguments != Arguments || argumentNamesOpt != ArgumentNamesOpt || argumentRefKindsOpt != ArgumentRefKindsOpt || expanded != Expanded || argsToParamsOpt != ArgsToParamsOpt || defaultArguments != DefaultArguments || constantValueOpt != ConstantValueOpt || initializerExpressionOpt != InitializerExpressionOpt || wasTargetTyped != WasTargetTyped || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundObjectCreationExpression boundObjectCreationExpression = new BoundObjectCreationExpression(Syntax, constructor, constructorsGroup, arguments, argumentNamesOpt, argumentRefKindsOpt, expanded, argsToParamsOpt, defaultArguments, constantValueOpt, initializerExpressionOpt, wasTargetTyped, type, base.HasErrors);
                boundObjectCreationExpression.CopyAttributes(this);
                return boundObjectCreationExpression;
            }
            return this;
        }
    }
}
