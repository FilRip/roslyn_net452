using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDelegateCreationExpression : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundExpression Argument { get; }

        public MethodSymbol? MethodOpt { get; }

        public bool IsExtensionMethod { get; }

        public BoundDelegateCreationExpression(SyntaxNode syntax, BoundExpression argument, MethodSymbol? methodOpt, bool isExtensionMethod, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.DelegateCreationExpression, syntax, type, hasErrors || argument.HasErrors())
        {
            Argument = argument;
            MethodOpt = methodOpt;
            IsExtensionMethod = isExtensionMethod;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDelegateCreationExpression(this);
        }

        public BoundDelegateCreationExpression Update(BoundExpression argument, MethodSymbol? methodOpt, bool isExtensionMethod, TypeSymbol type)
        {
            if (argument != Argument || !Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(methodOpt, MethodOpt) || isExtensionMethod != IsExtensionMethod || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundDelegateCreationExpression boundDelegateCreationExpression = new BoundDelegateCreationExpression(Syntax, argument, methodOpt, isExtensionMethod, type, base.HasErrors);
                boundDelegateCreationExpression.CopyAttributes(this);
                return boundDelegateCreationExpression;
            }
            return this;
        }
    }
}
