using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Microsoft.CodeAnalysis
{
    public interface IAttributeNamedArgumentDecoder
    {
        (KeyValuePair<string, TypedConstant> nameValuePair, bool isProperty, SerializationTypeCode typeCode, SerializationTypeCode elementTypeCode) DecodeCustomAttributeNamedArgumentOrThrow(ref BlobReader argReader);
    }
}
