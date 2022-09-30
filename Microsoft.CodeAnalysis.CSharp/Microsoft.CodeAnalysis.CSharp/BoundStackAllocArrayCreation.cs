using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundStackAllocArrayCreation : BoundStackAllocArrayCreationBase
    {
        protected override ImmutableArray<BoundNode?> Children => StaticCast<BoundNode>.From(BoundStackAllocArrayCreationBase.GetChildInitializers(base.InitializerOpt).Insert(0, base.Count));

        public override object Display
        {
            get
            {
                if ((object)base.Type != null)
                {
                    return base.Display;
                }
                return $"stackalloc {base.ElementType}[{(base.Count.WasCompilerGenerated ? null : base.Count.Syntax.ToString())}]";
            }
        }

        public BoundStackAllocArrayCreation(SyntaxNode syntax, TypeSymbol elementType, BoundExpression count, BoundArrayInitialization? initializerOpt, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.StackAllocArrayCreation, syntax, elementType, count, initializerOpt, type, hasErrors || count.HasErrors() || initializerOpt.HasErrors())
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitStackAllocArrayCreation(this);
        }

        public BoundStackAllocArrayCreation Update(TypeSymbol elementType, BoundExpression count, BoundArrayInitialization? initializerOpt, TypeSymbol? type)
        {
            if (!TypeSymbol.Equals(elementType, base.ElementType, TypeCompareKind.ConsiderEverything) || count != base.Count || initializerOpt != base.InitializerOpt || !TypeSymbol.Equals(type, base.Type, TypeCompareKind.ConsiderEverything))
            {
                BoundStackAllocArrayCreation boundStackAllocArrayCreation = new BoundStackAllocArrayCreation(Syntax, elementType, count, initializerOpt, type, base.HasErrors);
                boundStackAllocArrayCreation.CopyAttributes(this);
                return boundStackAllocArrayCreation;
            }
            return this;
        }
    }
}
