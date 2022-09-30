#nullable enable

namespace Microsoft.CodeAnalysis
{
    public static class CommonAttributeDataExtensions
    {
        public static bool TryGetGuidAttributeValue(this AttributeData attrData, out string? guidString)
        {
            if (attrData.CommonConstructorArguments.Length == 1)
            {
                object valueInternal = attrData.CommonConstructorArguments[0].ValueInternal;
                if (valueInternal == null || valueInternal is string)
                {
                    guidString = (string)valueInternal;
                    return true;
                }
            }
            guidString = null;
            return false;
        }
    }
}
