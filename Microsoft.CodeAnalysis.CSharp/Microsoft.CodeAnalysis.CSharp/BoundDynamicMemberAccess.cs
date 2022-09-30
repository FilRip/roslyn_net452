using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDynamicMemberAccess : BoundExpression
    {
        protected override ImmutableArray<BoundNode?> Children => ImmutableArray.Create((BoundNode)Receiver);

        public new TypeSymbol Type => base.Type;

        public BoundExpression Receiver { get; }

        public ImmutableArray<TypeWithAnnotations> TypeArgumentsOpt { get; }

        public string Name { get; }

        public bool Invoked { get; }

        public bool Indexed { get; }

        public BoundDynamicMemberAccess(SyntaxNode syntax, BoundExpression receiver, ImmutableArray<TypeWithAnnotations> typeArgumentsOpt, string name, bool invoked, bool indexed, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.DynamicMemberAccess, syntax, type, hasErrors || receiver.HasErrors())
        {
            Receiver = receiver;
            TypeArgumentsOpt = typeArgumentsOpt;
            Name = name;
            Invoked = invoked;
            Indexed = indexed;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDynamicMemberAccess(this);
        }

        public BoundDynamicMemberAccess Update(BoundExpression receiver, ImmutableArray<TypeWithAnnotations> typeArgumentsOpt, string name, bool invoked, bool indexed, TypeSymbol type)
        {
            if (receiver != Receiver || typeArgumentsOpt != TypeArgumentsOpt || name != Name || invoked != Invoked || indexed != Indexed || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundDynamicMemberAccess boundDynamicMemberAccess = new BoundDynamicMemberAccess(Syntax, receiver, typeArgumentsOpt, name, invoked, indexed, type, base.HasErrors);
                boundDynamicMemberAccess.CopyAttributes(this);
                return boundDynamicMemberAccess;
            }
            return this;
        }
    }
}
