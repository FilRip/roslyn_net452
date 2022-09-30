namespace Microsoft.CodeAnalysis
{
    public interface IMarshalAsAttributeTarget
    {
        MarshalPseudoCustomAttributeData GetOrCreateData();
    }
}
