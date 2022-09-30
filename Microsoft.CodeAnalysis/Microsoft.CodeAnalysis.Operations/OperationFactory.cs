using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.Operations
{
    public static class OperationFactory
    {
        private class IdentityConvertibleConversion : IConvertibleConversion
        {
            public CommonConversion ToCommonConversion()
            {
                return new CommonConversion(exists: true, isIdentity: true, isNumeric: false, isReference: false, isImplicit: true, isNullable: false, null);
            }
        }

        public static readonly IConvertibleConversion IdentityConversion = new IdentityConvertibleConversion();

        public static IInvalidOperation CreateInvalidOperation(SemanticModel semanticModel, SyntaxNode syntax, ImmutableArray<IOperation> children, bool isImplicit)
        {
            return new InvalidOperation(children, semanticModel, syntax, null, null, isImplicit);
        }
    }
}
