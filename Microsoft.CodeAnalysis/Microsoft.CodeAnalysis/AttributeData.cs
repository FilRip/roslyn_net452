using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public abstract class AttributeData
    {
        public INamedTypeSymbol? AttributeClass => CommonAttributeClass;

        protected abstract INamedTypeSymbol? CommonAttributeClass { get; }

        public IMethodSymbol? AttributeConstructor => CommonAttributeConstructor;

        protected abstract IMethodSymbol? CommonAttributeConstructor { get; }

        public SyntaxReference? ApplicationSyntaxReference => CommonApplicationSyntaxReference;

        protected abstract SyntaxReference? CommonApplicationSyntaxReference { get; }

        public ImmutableArray<TypedConstant> ConstructorArguments => CommonConstructorArguments;

        public abstract ImmutableArray<TypedConstant> CommonConstructorArguments { get; }

        public ImmutableArray<KeyValuePair<string, TypedConstant>> NamedArguments => CommonNamedArguments;

        public abstract ImmutableArray<KeyValuePair<string, TypedConstant>> CommonNamedArguments { get; }

        public virtual bool IsConditionallyOmitted => false;

        [MemberNotNullWhen(true, new string[] { "AttributeClass", "AttributeConstructor" })]
        public virtual bool HasErrors
        {
            [MemberNotNullWhen(true, new string[] { "AttributeClass", "AttributeConstructor" })]
            get
            {
                return false;
            }
        }

        public static bool IsTargetEarlyAttribute(INamedTypeSymbolInternal attributeType, int attributeArgCount, AttributeDescription description)
        {
            ISymbolInternal containingSymbol = attributeType.ContainingSymbol;
            if (containingSymbol == null || containingSymbol.Kind != SymbolKind.Namespace)
            {
                return false;
            }
            int num = description.Signatures.Length;
            for (int i = 0; i < num; i++)
            {
                int parameterCount = description.GetParameterCount(i);
                if (attributeArgCount == parameterCount)
                {
                    StringComparison stringComparison = (description.MatchIgnoringCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                    if (attributeType.Name.Equals(description.Name, stringComparison))
                    {
                        return namespaceMatch(attributeType.ContainingNamespace, description.Namespace, stringComparison);
                    }
                    return false;
                }
            }
            return false;
            static bool namespaceMatch(INamespaceSymbolInternal container, string namespaceName, StringComparison options)
            {
                int num2 = namespaceName.Length;
                bool flag = false;
                do
                {
                    if (container.IsGlobalNamespace)
                    {
                        return num2 == 0;
                    }
                    if (flag)
                    {
                        num2--;
                        if (num2 < 0 || namespaceName[num2] != '.')
                        {
                            return false;
                        }
                    }
                    else
                    {
                        flag = true;
                    }
                    string name = container.Name;
                    int length = name.Length;
                    num2 -= length;
                    if (num2 < 0 || string.Compare(namespaceName, num2, name, 0, length, options) != 0)
                    {
                        return false;
                    }
                    container = container.ContainingNamespace;
                }
                while (container != null);
                return false;
            }
        }

        public T? GetConstructorArgument<T>(int i, SpecialType specialType)
        {
            return CommonConstructorArguments[i].DecodeValue<T>(specialType);
        }

        internal T? DecodeNamedArgument<T>(string name, SpecialType specialType, T? defaultValue = default(T?))
        {
            return DecodeNamedArgument(CommonNamedArguments, name, specialType, defaultValue);
        }

        private static T? DecodeNamedArgument<T>(ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments, string name, SpecialType specialType, T? defaultValue = default(T?))
        {
            int num = IndexOfNamedArgument(namedArguments, name);
            if (num < 0)
            {
                return defaultValue;
            }
            return namedArguments[num].Value.DecodeValue<T>(specialType);
        }

        private static int IndexOfNamedArgument(ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments, string name)
        {
            for (int num = namedArguments.Length - 1; num >= 0; num--)
            {
                if (string.Equals(namedArguments[num].Key, name, StringComparison.Ordinal))
                {
                    return num;
                }
            }
            return -1;
        }

        public ConstantValue DecodeDecimalConstantValue()
        {
            ImmutableArray<IParameterSymbol> parameters = AttributeConstructor!.Parameters;
            ImmutableArray<TypedConstant> commonConstructorArguments = CommonConstructorArguments;
            byte scale = commonConstructorArguments[0].DecodeValue<byte>(SpecialType.System_Byte);
            bool isNegative = commonConstructorArguments[1].DecodeValue<byte>(SpecialType.System_Byte) != 0;
            int hi;
            int mid;
            int lo;
            if (parameters[2].Type.SpecialType == SpecialType.System_Int32)
            {
                hi = commonConstructorArguments[2].DecodeValue<int>(SpecialType.System_Int32);
                mid = commonConstructorArguments[3].DecodeValue<int>(SpecialType.System_Int32);
                lo = commonConstructorArguments[4].DecodeValue<int>(SpecialType.System_Int32);
            }
            else
            {
                hi = (int)commonConstructorArguments[2].DecodeValue<uint>(SpecialType.System_UInt32);
                mid = (int)commonConstructorArguments[3].DecodeValue<uint>(SpecialType.System_UInt32);
                lo = (int)commonConstructorArguments[4].DecodeValue<uint>(SpecialType.System_UInt32);
            }
            return ConstantValue.Create(new decimal(lo, mid, hi, isNegative, scale));
        }

        public ConstantValue DecodeDateTimeConstantValue()
        {
            long num = CommonConstructorArguments[0].DecodeValue<long>(SpecialType.System_Int64);
            DateTime minValue = DateTime.MinValue;
            if (num >= minValue.Ticks)
            {
                minValue = DateTime.MaxValue;
                if (num <= minValue.Ticks)
                {
                    return ConstantValue.Create(new DateTime(num));
                }
            }
            return ConstantValue.Bad;
        }

        public ObsoleteAttributeData DecodeObsoleteAttribute(ObsoleteAttributeKind kind)
        {
            return kind switch
            {
                ObsoleteAttributeKind.Obsolete => DecodeObsoleteAttribute(),
                ObsoleteAttributeKind.Deprecated => DecodeDeprecatedAttribute(),
                ObsoleteAttributeKind.Experimental => DecodeExperimentalAttribute(),
                _ => throw ExceptionUtilities.UnexpectedValue(kind),
            };
        }

        private ObsoleteAttributeData DecodeObsoleteAttribute()
        {
            ImmutableArray<TypedConstant> commonConstructorArguments = CommonConstructorArguments;
            string message = null;
            bool isError = false;
            TypedConstant value;
            if (commonConstructorArguments.Length > 0)
            {
                value = commonConstructorArguments[0];
                message = (string)value.ValueInternal;
                if (commonConstructorArguments.Length == 2)
                {
                    value = commonConstructorArguments[1];
                    isError = (bool)value.ValueInternal;
                }
            }
            string text = null;
            string text2 = null;
            ImmutableArray<KeyValuePair<string, TypedConstant>>.Enumerator enumerator = CommonNamedArguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePairUtil.Deconstruct(enumerator.Current, out var key, out value);
                string text3 = key;
                TypedConstant typedConstant = value;
                if (text == null && text3 == "DiagnosticId" && IsStringProperty("DiagnosticId"))
                {
                    text = typedConstant.ValueInternal as string;
                }
                else if (text2 == null && text3 == "UrlFormat" && IsStringProperty("UrlFormat"))
                {
                    text2 = typedConstant.ValueInternal as string;
                }
                if (text != null && text2 != null)
                {
                    break;
                }
            }
            return new ObsoleteAttributeData(ObsoleteAttributeKind.Obsolete, message, isError, text, text2);
        }

        protected virtual bool IsStringProperty(string memberName)
        {
            throw ExceptionUtilities.Unreachable;
        }

        private ObsoleteAttributeData DecodeDeprecatedAttribute()
        {
            ImmutableArray<TypedConstant> commonConstructorArguments = CommonConstructorArguments;
            string message = null;
            bool isError = false;
            if (commonConstructorArguments.Length == 3 || commonConstructorArguments.Length == 4)
            {
                message = (string)commonConstructorArguments[0].ValueInternal;
                isError = (int)commonConstructorArguments[1].ValueInternal == 1;
            }
            return new ObsoleteAttributeData(ObsoleteAttributeKind.Deprecated, message, isError, null, null);
        }

        private ObsoleteAttributeData DecodeExperimentalAttribute()
        {
            return ObsoleteAttributeData.Experimental;
        }

        public static void DecodeMethodImplAttribute<T, TAttributeSyntaxNode, TAttributeData, TAttributeLocation>(ref DecodeWellKnownAttributeArguments<TAttributeSyntaxNode, TAttributeData, TAttributeLocation> arguments, CommonMessageProvider messageProvider) where T : CommonMethodWellKnownAttributeData, new() where TAttributeSyntaxNode : SyntaxNode where TAttributeData : AttributeData
        {
            TAttributeData attribute = arguments.Attribute;
            MethodImplOptions methodImplOptions;
            if (attribute.CommonConstructorArguments.Length == 1)
            {
                methodImplOptions = ((attribute.AttributeConstructor!.Parameters[0].Type.SpecialType != SpecialType.System_Int16) ? attribute.CommonConstructorArguments[0].DecodeValue<MethodImplOptions>(SpecialType.System_Enum) : ((MethodImplOptions)attribute.CommonConstructorArguments[0].DecodeValue<short>(SpecialType.System_Int16)));
                if ((methodImplOptions & (MethodImplOptions)3) != 0)
                {
                    messageProvider.ReportInvalidAttributeArgument(arguments.Diagnostics, arguments.AttributeSyntaxOpt, 0, attribute);
                    methodImplOptions &= (MethodImplOptions)(-4);
                }
            }
            else
            {
                methodImplOptions = 0;
            }
            MethodImplAttributes methodImplAttributes = MethodImplAttributes.IL;
            int num = 1;
            ImmutableArray<KeyValuePair<string, TypedConstant>>.Enumerator enumerator = attribute.CommonNamedArguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<string, TypedConstant> current = enumerator.Current;
                if (current.Key == "MethodCodeType")
                {
                    MethodImplAttributes methodImplAttributes2 = (MethodImplAttributes)current.Value.DecodeValue<int>(SpecialType.System_Enum);
                    if (methodImplAttributes2 < MethodImplAttributes.IL || methodImplAttributes2 > MethodImplAttributes.CodeTypeMask)
                    {
                        messageProvider.ReportInvalidNamedArgument(arguments.Diagnostics, arguments.AttributeSyntaxOpt, num, attribute.AttributeClass, "MethodCodeType");
                    }
                    else
                    {
                        methodImplAttributes = methodImplAttributes2;
                    }
                }
                num++;
            }
            arguments.GetOrCreateData<T>().SetMethodImplementation(arguments.Index, (MethodImplAttributes)((int)methodImplOptions | (int)methodImplAttributes));
        }

        public static void DecodeStructLayoutAttribute<TTypeWellKnownAttributeData, TAttributeSyntaxNode, TAttributeData, TAttributeLocation>(ref DecodeWellKnownAttributeArguments<TAttributeSyntaxNode, TAttributeData, TAttributeLocation> arguments, CharSet defaultCharSet, int defaultAutoLayoutSize, CommonMessageProvider messageProvider) where TTypeWellKnownAttributeData : CommonTypeWellKnownAttributeData, new() where TAttributeSyntaxNode : SyntaxNode where TAttributeData : AttributeData
        {
            TAttributeData attribute = arguments.Attribute;
            CharSet charSet = ((defaultCharSet != CharSet.None) ? defaultCharSet : CharSet.Ansi);
            int? num = null;
            int? num2 = null;
            bool flag = false;
            LayoutKind layoutKind = attribute.CommonConstructorArguments[0].DecodeValue<LayoutKind>(SpecialType.System_Enum);
            if (layoutKind != 0 && (uint)(layoutKind - 2) > 1u)
            {
                messageProvider.ReportInvalidAttributeArgument(arguments.Diagnostics, arguments.AttributeSyntaxOpt, 0, attribute);
                flag = true;
            }
            int num3 = 1;
            ImmutableArray<KeyValuePair<string, TypedConstant>>.Enumerator enumerator = attribute.CommonNamedArguments.GetEnumerator();
            while (enumerator.MoveNext())
            {
                KeyValuePair<string, TypedConstant> current = enumerator.Current;
                switch (current.Key)
                {
                    case "CharSet":
                        charSet = current.Value.DecodeValue<CharSet>(SpecialType.System_Enum);
                        switch (charSet)
                        {
                            case CharSet.None:
                                charSet = CharSet.Ansi;
                                break;
                            default:
                                messageProvider.ReportInvalidNamedArgument(arguments.Diagnostics, arguments.AttributeSyntaxOpt, num3, attribute.AttributeClass, current.Key);
                                flag = true;
                                break;
                            case CharSet.Ansi:
                            case CharSet.Unicode:
                            case CharSet.Auto:
                                break;
                        }
                        break;
                    case "Pack":
                        num2 = current.Value.DecodeValue<int>(SpecialType.System_Int32);
                        if (num2 > 128 || (num2 & (num2 - 1)) != 0)
                        {
                            messageProvider.ReportInvalidNamedArgument(arguments.Diagnostics, arguments.AttributeSyntaxOpt, num3, attribute.AttributeClass, current.Key);
                            flag = true;
                        }
                        break;
                    case "Size":
                        num = current.Value.DecodeValue<int>(SpecialType.System_Int32);
                        if (num < 0)
                        {
                            messageProvider.ReportInvalidNamedArgument(arguments.Diagnostics, arguments.AttributeSyntaxOpt, num3, attribute.AttributeClass, current.Key);
                            flag = true;
                        }
                        break;
                }
                num3++;
            }
            if (!flag)
            {
                if (layoutKind == LayoutKind.Auto && !num.HasValue && num2.HasValue)
                {
                    num = defaultAutoLayoutSize;
                }
                arguments.GetOrCreateData<TTypeWellKnownAttributeData>().SetStructLayout(new TypeLayout(layoutKind, num.GetValueOrDefault(), (byte)num2.GetValueOrDefault()), charSet);
            }
        }

        public AttributeUsageInfo DecodeAttributeUsageAttribute()
        {
            return DecodeAttributeUsageAttribute(CommonConstructorArguments[0], CommonNamedArguments);
        }

        public static AttributeUsageInfo DecodeAttributeUsageAttribute(TypedConstant positionalArg, ImmutableArray<KeyValuePair<string, TypedConstant>> namedArgs)
        {
            AttributeTargets validTargets = (AttributeTargets)positionalArg.ValueInternal;
            bool allowMultiple = DecodeNamedArgument(namedArgs, "AllowMultiple", SpecialType.System_Boolean, defaultValue: false);
            bool inherited = DecodeNamedArgument(namedArgs, "Inherited", SpecialType.System_Boolean, defaultValue: true);
            return new AttributeUsageInfo(validTargets, allowMultiple, inherited);
        }
    }
}
