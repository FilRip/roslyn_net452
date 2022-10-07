using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal abstract class AbstractTypeParameterMap : AbstractTypeMap
    {
        protected readonly SmallDictionary<TypeParameterSymbol, TypeWithAnnotations> Mapping;

        protected AbstractTypeParameterMap(SmallDictionary<TypeParameterSymbol, TypeWithAnnotations> mapping)
        {
            Mapping = mapping;
        }

        protected sealed override TypeWithAnnotations SubstituteTypeParameter(TypeParameterSymbol typeParameter)
        {
            if (Mapping.TryGetValue(typeParameter, out var value))
            {
                return value;
            }
            return TypeWithAnnotations.Create(typeParameter);
        }

        private string GetDebuggerDisplay()
        {
            StringBuilder stringBuilder = new("[");
            stringBuilder.Append(GetType().Name);
            SmallDictionary<TypeParameterSymbol, TypeWithAnnotations>.Enumerator enumerator = Mapping.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<TypeParameterSymbol, TypeWithAnnotations> current = enumerator.Current;
                stringBuilder.Append(" ").Append(current.Key).Append(":")
                    .Append(current.Value.Type);
            }
            return stringBuilder.Append("]").ToString();
        }
    }
}
