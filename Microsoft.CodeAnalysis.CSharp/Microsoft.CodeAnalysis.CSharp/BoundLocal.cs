using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundLocal : BoundExpression
    {
        public override ConstantValue? ConstantValue => ConstantValueOpt;

        public override Symbol ExpressionSymbol => LocalSymbol;

        public new TypeSymbol Type => base.Type;

        public LocalSymbol LocalSymbol { get; }

        public BoundLocalDeclarationKind DeclarationKind { get; }

        public ConstantValue? ConstantValueOpt { get; }

        public bool IsNullableUnknown { get; }

        public BoundLocal(SyntaxNode syntax, LocalSymbol localSymbol, ConstantValue? constantValueOpt, TypeSymbol type, bool hasErrors = false)
            : this(syntax, localSymbol, BoundLocalDeclarationKind.None, constantValueOpt, isNullableUnknown: false, type, hasErrors)
        {
        }

        public BoundLocal Update(LocalSymbol localSymbol, ConstantValue? constantValueOpt, TypeSymbol type)
        {
            return Update(localSymbol, DeclarationKind, constantValueOpt, IsNullableUnknown, type);
        }

        public BoundLocal(SyntaxNode syntax, LocalSymbol localSymbol, BoundLocalDeclarationKind declarationKind, ConstantValue? constantValueOpt, bool isNullableUnknown, TypeSymbol type, bool hasErrors)
            : base(BoundKind.Local, syntax, type, hasErrors)
        {
            LocalSymbol = localSymbol;
            DeclarationKind = declarationKind;
            ConstantValueOpt = constantValueOpt;
            IsNullableUnknown = isNullableUnknown;
        }

        public BoundLocal(SyntaxNode syntax, LocalSymbol localSymbol, BoundLocalDeclarationKind declarationKind, ConstantValue? constantValueOpt, bool isNullableUnknown, TypeSymbol type)
            : base(BoundKind.Local, syntax, type)
        {
            LocalSymbol = localSymbol;
            DeclarationKind = declarationKind;
            ConstantValueOpt = constantValueOpt;
            IsNullableUnknown = isNullableUnknown;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitLocal(this);
        }

        public BoundLocal Update(LocalSymbol localSymbol, BoundLocalDeclarationKind declarationKind, ConstantValue? constantValueOpt, bool isNullableUnknown, TypeSymbol type)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(localSymbol, LocalSymbol) || declarationKind != DeclarationKind || constantValueOpt != ConstantValueOpt || isNullableUnknown != IsNullableUnknown || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundLocal boundLocal = new BoundLocal(Syntax, localSymbol, declarationKind, constantValueOpt, isNullableUnknown, type, base.HasErrors);
                boundLocal.CopyAttributes(this);
                return boundLocal;
            }
            return this;
        }
    }
}
