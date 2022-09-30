namespace Microsoft.CodeAnalysis
{
    public interface ISecurityAttributeTarget
    {
        SecurityWellKnownAttributeData GetOrCreateData();
    }
}
