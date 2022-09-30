using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundObjectOrCollectionValuePlaceholder : BoundValuePlaceholderBase
    {
        public new TypeSymbol Type => base.Type;

        public bool IsNewInstance { get; }

        public BoundObjectOrCollectionValuePlaceholder(SyntaxNode syntax, bool isNewInstance, TypeSymbol type, bool hasErrors)
            : base(BoundKind.ObjectOrCollectionValuePlaceholder, syntax, type, hasErrors)
        {
            IsNewInstance = isNewInstance;
        }

        public BoundObjectOrCollectionValuePlaceholder(SyntaxNode syntax, bool isNewInstance, TypeSymbol type)
            : base(BoundKind.ObjectOrCollectionValuePlaceholder, syntax, type)
        {
            IsNewInstance = isNewInstance;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitObjectOrCollectionValuePlaceholder(this);
        }

        public BoundObjectOrCollectionValuePlaceholder Update(bool isNewInstance, TypeSymbol type)
        {
            if (isNewInstance != IsNewInstance || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundObjectOrCollectionValuePlaceholder boundObjectOrCollectionValuePlaceholder = new BoundObjectOrCollectionValuePlaceholder(Syntax, isNewInstance, type, base.HasErrors);
                boundObjectOrCollectionValuePlaceholder.CopyAttributes(this);
                return boundObjectOrCollectionValuePlaceholder;
            }
            return this;
        }
    }
}
