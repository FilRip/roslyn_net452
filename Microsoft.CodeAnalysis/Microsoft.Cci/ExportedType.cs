namespace Microsoft.Cci
{
    public struct ExportedType
    {
        public readonly ITypeReference Type;

        public readonly bool IsForwarder;

        public readonly int ParentIndex;

        public ExportedType(ITypeReference type, int parentIndex, bool isForwarder)
        {
            Type = type;
            IsForwarder = isForwarder;
            ParentIndex = parentIndex;
        }
    }
}
