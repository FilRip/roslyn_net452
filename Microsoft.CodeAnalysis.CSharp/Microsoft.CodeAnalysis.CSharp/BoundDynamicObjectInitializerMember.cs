using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDynamicObjectInitializerMember : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public string MemberName { get; }

        public TypeSymbol ReceiverType { get; }

        public BoundDynamicObjectInitializerMember(SyntaxNode syntax, string memberName, TypeSymbol receiverType, TypeSymbol type, bool hasErrors)
            : base(BoundKind.DynamicObjectInitializerMember, syntax, type, hasErrors)
        {
            MemberName = memberName;
            ReceiverType = receiverType;
        }

        public BoundDynamicObjectInitializerMember(SyntaxNode syntax, string memberName, TypeSymbol receiverType, TypeSymbol type)
            : base(BoundKind.DynamicObjectInitializerMember, syntax, type)
        {
            MemberName = memberName;
            ReceiverType = receiverType;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDynamicObjectInitializerMember(this);
        }

        public BoundDynamicObjectInitializerMember Update(string memberName, TypeSymbol receiverType, TypeSymbol type)
        {
            if (memberName != MemberName || !TypeSymbol.Equals(receiverType, ReceiverType, TypeCompareKind.ConsiderEverything) || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundDynamicObjectInitializerMember boundDynamicObjectInitializerMember = new BoundDynamicObjectInitializerMember(Syntax, memberName, receiverType, type, base.HasErrors);
                boundDynamicObjectInitializerMember.CopyAttributes(this);
                return boundDynamicObjectInitializerMember;
            }
            return this;
        }
    }
}
