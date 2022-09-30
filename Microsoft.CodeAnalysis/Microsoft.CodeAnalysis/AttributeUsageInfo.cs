using System;
using System.Globalization;

using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public struct AttributeUsageInfo : IEquatable<AttributeUsageInfo>
    {
        [Flags()]
        private enum PackedAttributeUsage
        {
            None = 0,
            Assembly = 1,
            Module = 2,
            Class = 4,
            Struct = 8,
            Enum = 0x10,
            Constructor = 0x20,
            Method = 0x40,
            Property = 0x80,
            Field = 0x100,
            Event = 0x200,
            Interface = 0x400,
            Parameter = 0x800,
            Delegate = 0x1000,
            ReturnValue = 0x2000,
            GenericParameter = 0x4000,
            All = 0x7FFF,
            Initialized = 0x8000,
            AllowMultiple = 0x10000,
            Inherited = 0x20000
        }

        private struct ValidTargetsStringLocalizableErrorArgument : IFormattable
        {
            private readonly string[]? _targetResourceIds;

            internal ValidTargetsStringLocalizableErrorArgument(string[] targetResourceIds)
            {
                _targetResourceIds = targetResourceIds;
            }

            public override string ToString()
            {
                return ToString(null, null);
            }

            public string ToString(string? format, IFormatProvider? formatProvider)
            {
                PooledStringBuilder instance = PooledStringBuilder.GetInstance();
                CultureInfo culture = formatProvider as CultureInfo;
                if (_targetResourceIds != null)
                {
                    string[] targetResourceIds = _targetResourceIds;
                    foreach (string name in targetResourceIds)
                    {
                        if (instance.Builder.Length > 0)
                        {
                            instance.Builder.Append(", ");
                        }
                        instance.Builder.Append(CodeAnalysisResources.ResourceManager.GetString(name, culture));
                    }
                }
                string result = instance.Builder.ToString();
                instance.Free();
                return result;
            }
        }

        private readonly PackedAttributeUsage _flags;

        public static readonly AttributeUsageInfo Default = new AttributeUsageInfo(AttributeTargets.All, allowMultiple: false, inherited: true);

        public static readonly AttributeUsageInfo Null = default(AttributeUsageInfo);

        public bool IsNull => (_flags & PackedAttributeUsage.Initialized) == 0;

        public AttributeTargets ValidTargets => (AttributeTargets)(_flags & PackedAttributeUsage.All);

        public bool AllowMultiple => (_flags & PackedAttributeUsage.AllowMultiple) != 0;

        public bool Inherited => (_flags & PackedAttributeUsage.Inherited) != 0;

        public bool HasValidAttributeTargets
        {
            get
            {
                int validTargets = (int)ValidTargets;
                if (validTargets != 0)
                {
                    return (validTargets & -32768) == 0;
                }
                return false;
            }
        }

        public AttributeUsageInfo(AttributeTargets validTargets, bool allowMultiple, bool inherited)
        {
            _flags = (PackedAttributeUsage)(validTargets | (AttributeTargets)32768);
            if (allowMultiple)
            {
                _flags |= PackedAttributeUsage.AllowMultiple;
            }
            if (inherited)
            {
                _flags |= PackedAttributeUsage.Inherited;
            }
        }

        public static bool operator ==(AttributeUsageInfo left, AttributeUsageInfo right)
        {
            return left._flags == right._flags;
        }

        public static bool operator !=(AttributeUsageInfo left, AttributeUsageInfo right)
        {
            return left._flags != right._flags;
        }

        public override bool Equals(object? obj)
        {
            if (obj is AttributeUsageInfo)
            {
                return Equals((AttributeUsageInfo)obj);
            }
            return false;
        }

        public bool Equals(AttributeUsageInfo other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return _flags.GetHashCode();
        }

        public object GetValidTargetsErrorArgument()
        {
            int num = (int)ValidTargets;
            if (!HasValidAttributeTargets)
            {
                return string.Empty;
            }
            ArrayBuilder<string> instance = ArrayBuilder<string>.GetInstance();
            int num2 = 0;
            while (num > 0)
            {
                if (((uint)num & (true ? 1u : 0u)) != 0)
                {
                    instance.Add(GetErrorDisplayNameResourceId((AttributeTargets)(1 << num2)));
                }
                num >>= 1;
                num2++;
            }
            return new ValidTargetsStringLocalizableErrorArgument(instance.ToArrayAndFree());
        }

        private static string GetErrorDisplayNameResourceId(AttributeTargets target)
        {
            return target switch
            {
                AttributeTargets.Assembly => "Assembly",
                AttributeTargets.Class => "Class1",
                AttributeTargets.Constructor => "Constructor",
                AttributeTargets.Delegate => "Delegate1",
                AttributeTargets.Enum => "Enum1",
                AttributeTargets.Event => "Event1",
                AttributeTargets.Field => "Field",
                AttributeTargets.GenericParameter => "TypeParameter",
                AttributeTargets.Interface => "Interface1",
                AttributeTargets.Method => "Method",
                AttributeTargets.Module => "Module",
                AttributeTargets.Parameter => "Parameter",
                AttributeTargets.Property => "Property",
                AttributeTargets.ReturnValue => "Return1",
                AttributeTargets.Struct => "Struct1",
                _ => throw ExceptionUtilities.UnexpectedValue(target),
            };
        }
    }
}
