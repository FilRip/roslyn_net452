using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Operations;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundCall : BoundExpression, IBoundInvalidNode
    {
        private readonly LookupResultKind _ResultKind;

        public override Symbol ExpressionSymbol => Method;

        ImmutableArray<BoundNode> IBoundInvalidNode.InvalidNodeChildren => CSharpOperationFactory.CreateInvalidChildrenFromArgumentsExpression(ReceiverOpt, Arguments);

        public new TypeSymbol Type => base.Type;

        public BoundExpression? ReceiverOpt { get; }

        public MethodSymbol Method { get; }

        public ImmutableArray<BoundExpression> Arguments { get; }

        public ImmutableArray<string> ArgumentNamesOpt { get; }

        public ImmutableArray<RefKind> ArgumentRefKindsOpt { get; }

        public bool IsDelegateCall { get; }

        public bool Expanded { get; }

        public bool InvokedAsExtensionMethod { get; }

        public ImmutableArray<int> ArgsToParamsOpt { get; }

        public BitVector DefaultArguments { get; }

        public override LookupResultKind ResultKind => _ResultKind;

        public ImmutableArray<MethodSymbol> OriginalMethodsOpt { get; }

        public BoundCall(SyntaxNode syntax, BoundExpression? receiverOpt, MethodSymbol method, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool isDelegateCall, bool expanded, bool invokedAsExtensionMethod, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, LookupResultKind resultKind, TypeSymbol type, bool hasErrors = false)
            : this(syntax, receiverOpt, method, arguments, argumentNamesOpt, argumentRefKindsOpt, isDelegateCall, expanded, invokedAsExtensionMethod, argsToParamsOpt, defaultArguments, resultKind, default(ImmutableArray<MethodSymbol>), type, hasErrors)
        {
        }

        public BoundCall Update(BoundExpression? receiverOpt, MethodSymbol method, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool isDelegateCall, bool expanded, bool invokedAsExtensionMethod, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, LookupResultKind resultKind, TypeSymbol type)
        {
            return Update(receiverOpt, method, arguments, argumentNamesOpt, argumentRefKindsOpt, isDelegateCall, expanded, invokedAsExtensionMethod, argsToParamsOpt, defaultArguments, resultKind, OriginalMethodsOpt, type);
        }

        public static BoundCall ErrorCall(SyntaxNode node, BoundExpression receiverOpt, MethodSymbol method, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> namedArguments, ImmutableArray<RefKind> refKinds, bool isDelegateCall, bool invokedAsExtensionMethod, ImmutableArray<MethodSymbol> originalMethods, LookupResultKind resultKind, Binder binder)
        {
            if (!originalMethods.IsEmpty)
            {
                resultKind = resultKind.WorseResultKind(LookupResultKind.OverloadResolutionFailure);
            }
            return new BoundCall(node, binder.BindToTypeForErrorRecovery(receiverOpt), method, arguments.SelectAsArray((BoundExpression e, Binder binder) => binder.BindToTypeForErrorRecovery(e), binder), namedArguments, refKinds, isDelegateCall, expanded: false, invokedAsExtensionMethod, default(ImmutableArray<int>), default(BitVector), resultKind, originalMethods, method.ReturnType, hasErrors: true);
        }

        public BoundCall Update(ImmutableArray<BoundExpression> arguments)
        {
            return Update(ReceiverOpt, Method, arguments, ArgumentNamesOpt, ArgumentRefKindsOpt, IsDelegateCall, Expanded, InvokedAsExtensionMethod, ArgsToParamsOpt, DefaultArguments, ResultKind, OriginalMethodsOpt, Type);
        }

        public BoundCall Update(BoundExpression? receiverOpt, MethodSymbol method, ImmutableArray<BoundExpression> arguments)
        {
            return Update(receiverOpt, method, arguments, ArgumentNamesOpt, ArgumentRefKindsOpt, IsDelegateCall, Expanded, InvokedAsExtensionMethod, ArgsToParamsOpt, DefaultArguments, ResultKind, OriginalMethodsOpt, Type);
        }

        public static BoundCall Synthesized(SyntaxNode syntax, BoundExpression? receiverOpt, MethodSymbol method)
        {
            return Synthesized(syntax, receiverOpt, method, ImmutableArray<BoundExpression>.Empty);
        }

        public static BoundCall Synthesized(SyntaxNode syntax, BoundExpression? receiverOpt, MethodSymbol method, BoundExpression arg0)
        {
            return Synthesized(syntax, receiverOpt, method, ImmutableArray.Create(arg0));
        }

        public static BoundCall Synthesized(SyntaxNode syntax, BoundExpression? receiverOpt, MethodSymbol method, BoundExpression arg0, BoundExpression arg1)
        {
            return Synthesized(syntax, receiverOpt, method, ImmutableArray.Create(arg0, arg1));
        }

        public static BoundCall Synthesized(SyntaxNode syntax, BoundExpression? receiverOpt, MethodSymbol method, ImmutableArray<BoundExpression> arguments)
        {
            return new BoundCall(syntax, receiverOpt, method, arguments, default(ImmutableArray<string>), method.ParameterRefKinds, isDelegateCall: false, expanded: false, invokedAsExtensionMethod: false, default(ImmutableArray<int>), default(BitVector), LookupResultKind.Viable, default(ImmutableArray<MethodSymbol>), method.ReturnType, method.OriginalDefinition is ErrorMethodSymbol)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundCall(SyntaxNode syntax, BoundExpression? receiverOpt, MethodSymbol method, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool isDelegateCall, bool expanded, bool invokedAsExtensionMethod, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalMethodsOpt, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.Call, syntax, type, hasErrors || receiverOpt.HasErrors() || arguments.HasErrors())
        {
            ReceiverOpt = receiverOpt;
            Method = method;
            Arguments = arguments;
            ArgumentNamesOpt = argumentNamesOpt;
            ArgumentRefKindsOpt = argumentRefKindsOpt;
            IsDelegateCall = isDelegateCall;
            Expanded = expanded;
            InvokedAsExtensionMethod = invokedAsExtensionMethod;
            ArgsToParamsOpt = argsToParamsOpt;
            DefaultArguments = defaultArguments;
            _ResultKind = resultKind;
            OriginalMethodsOpt = originalMethodsOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitCall(this);
        }

        public BoundCall Update(BoundExpression? receiverOpt, MethodSymbol method, ImmutableArray<BoundExpression> arguments, ImmutableArray<string> argumentNamesOpt, ImmutableArray<RefKind> argumentRefKindsOpt, bool isDelegateCall, bool expanded, bool invokedAsExtensionMethod, ImmutableArray<int> argsToParamsOpt, BitVector defaultArguments, LookupResultKind resultKind, ImmutableArray<MethodSymbol> originalMethodsOpt, TypeSymbol type)
        {
            if (receiverOpt != ReceiverOpt || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(method, Method) || arguments != Arguments || argumentNamesOpt != ArgumentNamesOpt || argumentRefKindsOpt != ArgumentRefKindsOpt || isDelegateCall != IsDelegateCall || expanded != Expanded || invokedAsExtensionMethod != InvokedAsExtensionMethod || argsToParamsOpt != ArgsToParamsOpt || defaultArguments != DefaultArguments || resultKind != ResultKind || originalMethodsOpt != OriginalMethodsOpt || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundCall boundCall = new BoundCall(Syntax, receiverOpt, method, arguments, argumentNamesOpt, argumentRefKindsOpt, isDelegateCall, expanded, invokedAsExtensionMethod, argsToParamsOpt, defaultArguments, resultKind, originalMethodsOpt, type, base.HasErrors);
                boundCall.CopyAttributes(this);
                return boundCall;
            }
            return this;
        }
    }
}
