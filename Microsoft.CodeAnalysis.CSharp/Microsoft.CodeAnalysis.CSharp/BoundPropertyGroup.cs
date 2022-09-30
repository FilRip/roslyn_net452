using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundPropertyGroup : BoundMethodOrPropertyGroup
    {
        public override object Display
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        public ImmutableArray<PropertySymbol> Properties { get; }

        public BoundPropertyGroup(SyntaxNode syntax, ImmutableArray<PropertySymbol> properties, BoundExpression? receiverOpt, LookupResultKind resultKind, bool hasErrors = false)
            : base(BoundKind.PropertyGroup, syntax, receiverOpt, resultKind, hasErrors || receiverOpt.HasErrors())
        {
            Properties = properties;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitPropertyGroup(this);
        }

        public BoundPropertyGroup Update(ImmutableArray<PropertySymbol> properties, BoundExpression? receiverOpt, LookupResultKind resultKind)
        {
            if (properties != Properties || receiverOpt != base.ReceiverOpt || resultKind != ResultKind)
            {
                BoundPropertyGroup boundPropertyGroup = new BoundPropertyGroup(Syntax, properties, receiverOpt, resultKind, base.HasErrors);
                boundPropertyGroup.CopyAttributes(this);
                return boundPropertyGroup;
            }
            return this;
        }
    }
}
