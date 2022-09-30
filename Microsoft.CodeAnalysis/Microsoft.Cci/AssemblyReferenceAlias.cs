namespace Microsoft.Cci
{
    public struct AssemblyReferenceAlias
    {
        public readonly string Name;

        public readonly IAssemblyReference Assembly;

        public AssemblyReferenceAlias(string name, IAssemblyReference assembly)
        {
            Name = name;
            Assembly = assembly;
        }
    }
}
