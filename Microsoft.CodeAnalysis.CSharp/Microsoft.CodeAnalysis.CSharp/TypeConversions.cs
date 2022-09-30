using Microsoft.CodeAnalysis.CSharp.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class TypeConversions : ConversionsBase
    {
        public TypeConversions(AssemblySymbol corLibrary, bool includeNullability = false)
            : this(corLibrary, 0, includeNullability, null)
        {
        }

        private TypeConversions(AssemblySymbol corLibrary, int currentRecursionDepth, bool includeNullability, TypeConversions otherNullabilityOpt)
            : base(corLibrary, currentRecursionDepth, includeNullability, otherNullabilityOpt)
        {
        }

        protected override ConversionsBase CreateInstance(int currentRecursionDepth)
        {
            return new TypeConversions(corLibrary, currentRecursionDepth, IncludeNullability, null);
        }

        protected override ConversionsBase WithNullabilityCore(bool includeNullability)
        {
            return new TypeConversions(corLibrary, currentRecursionDepth, includeNullability, this);
        }

        public override Conversion GetMethodGroupDelegateConversion(BoundMethodGroup source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override Conversion GetMethodGroupFunctionPointerConversion(BoundMethodGroup source, FunctionPointerTypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            throw ExceptionUtilities.Unreachable;
        }

        public override Conversion GetStackAllocConversion(BoundStackAllocArrayCreation sourceExpression, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            throw ExceptionUtilities.Unreachable;
        }

        protected override Conversion GetInterpolatedStringConversion(BoundUnconvertedInterpolatedString source, TypeSymbol destination, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}
