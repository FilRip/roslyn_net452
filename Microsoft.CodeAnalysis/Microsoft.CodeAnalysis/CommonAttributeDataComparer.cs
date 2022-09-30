using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    public sealed class CommonAttributeDataComparer : IEqualityComparer<AttributeData>
    {
        public static CommonAttributeDataComparer Instance = new CommonAttributeDataComparer();

        private CommonAttributeDataComparer()
        {
        }

        public bool Equals(AttributeData attr1, AttributeData attr2)
        {
            if (attr1.AttributeClass == attr2.AttributeClass && attr1.AttributeConstructor == attr2.AttributeConstructor && attr1.HasErrors == attr2.HasErrors && attr1.IsConditionallyOmitted == attr2.IsConditionallyOmitted && attr1.CommonConstructorArguments.SequenceEqual(attr2.CommonConstructorArguments))
            {
                return attr1.NamedArguments.SequenceEqual(attr2.NamedArguments);
            }
            return false;
        }

        public int GetHashCode(AttributeData attr)
        {
            int num = attr.AttributeClass?.GetHashCode() ?? 0;
            num = ((attr.AttributeConstructor != null) ? Hash.Combine(attr.AttributeConstructor!.GetHashCode(), num) : num);
            num = Hash.Combine(attr.HasErrors, num);
            num = Hash.Combine(attr.IsConditionallyOmitted, num);
            num = Hash.Combine(GetHashCodeForConstructorArguments(attr.CommonConstructorArguments), num);
            return Hash.Combine(GetHashCodeForNamedArguments(attr.NamedArguments), num);
        }

        private static int GetHashCodeForConstructorArguments(ImmutableArray<TypedConstant> constructorArguments)
        {
            int num = 0;
            ImmutableArray<TypedConstant>.Enumerator enumerator = constructorArguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                num = Hash.Combine(enumerator.Current.GetHashCode(), num);
            }
            return num;
        }

        private static int GetHashCodeForNamedArguments(ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments)
        {
            int num = 0;
            ImmutableArray<KeyValuePair<string, TypedConstant>>.Enumerator enumerator = namedArguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<string, TypedConstant> current = enumerator.Current;
                if (current.Key != null)
                {
                    num = Hash.Combine(current.Key.GetHashCode(), num);
                }
                num = Hash.Combine(current.Value.GetHashCode(), num);
            }
            return num;
        }
    }
}
