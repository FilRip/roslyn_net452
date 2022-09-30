using System.Linq;

using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal struct UnaryOperatorSignature
    {
        public static UnaryOperatorSignature Error;

        public readonly MethodSymbol Method;

        public readonly TypeSymbol OperandType;

        public readonly TypeSymbol ReturnType;

        public readonly UnaryOperatorKind Kind;

        public RefKind RefKind
        {
            get
            {
                if ((object)Method != null && !Method.ParameterRefKinds.IsDefaultOrEmpty)
                {
                    return Method.ParameterRefKinds.Single();
                }
                return RefKind.None;
            }
        }

        public UnaryOperatorSignature(UnaryOperatorKind kind, TypeSymbol operandType, TypeSymbol returnType, MethodSymbol method = null)
        {
            Kind = kind;
            OperandType = operandType;
            ReturnType = returnType;
            Method = method;
        }

        public override string ToString()
        {
            return $"kind: {Kind} operandType: {OperandType} operandRefKind: {RefKind} return: {ReturnType}";
        }
    }
}
