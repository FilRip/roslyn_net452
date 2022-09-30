using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal struct MostCommonNullableValueBuilder
    {
        private int _value0;

        private int _value1;

        private int _value2;

        internal byte? MostCommonValue
        {
            get
            {
                int num;
                byte value;
                if (_value1 > _value0)
                {
                    num = _value1;
                    value = 1;
                }
                else
                {
                    num = _value0;
                    value = 0;
                }
                if (_value2 > num)
                {
                    return 2;
                }
                if (num != 0)
                {
                    return value;
                }
                return null;
            }
        }

        internal void AddValue(byte value)
        {
            switch (value)
            {
                case 0:
                    _value0++;
                    break;
                case 1:
                    _value1++;
                    break;
                case 2:
                    _value2++;
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(value);
            }
        }

        internal void AddValue(byte? value)
        {
            if (value.HasValue)
            {
                AddValue(value.GetValueOrDefault());
            }
        }

        internal void AddValue(TypeWithAnnotations type)
        {
            ArrayBuilder<byte> instance = ArrayBuilder<byte>.GetInstance();
            type.AddNullableTransforms(instance);
            AddValue(GetCommonValue(instance));
            instance.Free();
        }

        internal static byte? GetCommonValue(ArrayBuilder<byte> builder)
        {
            int count = builder.Count;
            if (count == 0)
            {
                return null;
            }
            byte b = builder[0];
            for (int i = 1; i < count; i++)
            {
                if (builder[i] != b)
                {
                    return null;
                }
            }
            return b;
        }
    }
}
