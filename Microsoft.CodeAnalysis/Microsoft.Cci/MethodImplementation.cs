namespace Microsoft.Cci
{
    public struct MethodImplementation
    {
        public readonly IMethodDefinition ImplementingMethod;

        public readonly IMethodReference ImplementedMethod;

        public ITypeDefinition ContainingType => ImplementingMethod.ContainingTypeDefinition;

        public MethodImplementation(IMethodDefinition ImplementingMethod, IMethodReference ImplementedMethod)
        {
            this.ImplementingMethod = ImplementingMethod;
            this.ImplementedMethod = ImplementedMethod;
        }
    }
}
