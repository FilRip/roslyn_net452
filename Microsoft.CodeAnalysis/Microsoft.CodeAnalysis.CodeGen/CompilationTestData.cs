using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.DiaSymReader;

#nullable enable

namespace Microsoft.CodeAnalysis.CodeGen
{
    public sealed class CompilationTestData
    {
        public struct MethodData
        {
            public readonly ILBuilder ILBuilder;

            public readonly IMethodSymbolInternal Method;

            public MethodData(ILBuilder ilBuilder, IMethodSymbolInternal method)
            {
                ILBuilder = ilBuilder;
                Method = method;
            }
        }

        public readonly ConcurrentDictionary<IMethodSymbolInternal, MethodData> Methods = new ConcurrentDictionary<IMethodSymbolInternal, MethodData>();

        public CommonPEModuleBuilder? Module;

        public Func<ISymWriterMetadataProvider, SymUnmanagedWriter>? SymWriterFactory;

        private ImmutableDictionary<string, MethodData>? _lazyMethodsByName;

        private static readonly SymbolDisplayFormat _testDataKeyFormat = new SymbolDisplayFormat(SymbolDisplayCompilerInternalOptions.UseMetadataMethodNames | SymbolDisplayCompilerInternalOptions.UseValueTuple, SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining, SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces, SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance, SymbolDisplayMemberOptions.IncludeExplicitInterface | SymbolDisplayMemberOptions.IncludeParameters | SymbolDisplayMemberOptions.IncludeContainingType, SymbolDisplayParameterOptions.IncludeExtensionThis | SymbolDisplayParameterOptions.IncludeParamsRefOut | SymbolDisplayParameterOptions.IncludeType, SymbolDisplayDelegateStyle.NameOnly, SymbolDisplayExtensionMethodStyle.Default, SymbolDisplayPropertyStyle.NameOnly, SymbolDisplayLocalOptions.None, SymbolDisplayKindOptions.None, SymbolDisplayMiscellaneousOptions.UseSpecialTypes | SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.UseAsterisksInMultiDimensionalArrays | SymbolDisplayMiscellaneousOptions.UseErrorTypeSymbolName);

        private static readonly SymbolDisplayFormat _testDataOperatorKeyFormat = new SymbolDisplayFormat(_testDataKeyFormat.CompilerInternalOptions, _testDataKeyFormat.GlobalNamespaceStyle, _testDataKeyFormat.TypeQualificationStyle, _testDataKeyFormat.GenericsOptions, _testDataKeyFormat.MemberOptions | SymbolDisplayMemberOptions.IncludeType, _testDataKeyFormat.ParameterOptions, _testDataKeyFormat.DelegateStyle, _testDataKeyFormat.ExtensionMethodStyle, _testDataKeyFormat.PropertyStyle, _testDataKeyFormat.LocalOptions, _testDataKeyFormat.KindOptions, _testDataKeyFormat.MiscellaneousOptions);

        public ILBuilder GetIL(Func<IMethodSymbolInternal, bool> predicate)
        {
            Func<IMethodSymbolInternal, bool> predicate2 = predicate;
            return Methods.Single<KeyValuePair<IMethodSymbolInternal, MethodData>>((KeyValuePair<IMethodSymbolInternal, MethodData> p) => predicate2(p.Key)).Value.ILBuilder;
        }

        public ImmutableDictionary<string, MethodData> GetMethodsByName()
        {
            if (_lazyMethodsByName == null)
            {
                Dictionary<string, MethodData> dictionary = new Dictionary<string, MethodData>();
                foreach (KeyValuePair<IMethodSymbolInternal, MethodData> method in Methods)
                {
                    string methodName = GetMethodName(method.Key);
                    if (dictionary.ContainsKey(methodName))
                    {
                        dictionary[methodName] = default;
                    }
                    else
                    {
                        dictionary.Add(methodName, method.Value);
                    }
                }
                ImmutableDictionary<string, MethodData> value = dictionary.Where((KeyValuePair<string, MethodData> p) => p.Value.Method != null).ToImmutableDictionary();
                Interlocked.CompareExchange(ref _lazyMethodsByName, value, null);
            }
            return _lazyMethodsByName;
        }

        private static string GetMethodName(IMethodSymbolInternal methodSymbol)
        {
            IMethodSymbol obj = (IMethodSymbol)methodSymbol.GetISymbol();
            SymbolDisplayFormat format = ((obj.MethodKind == MethodKind.UserDefinedOperator) ? _testDataOperatorKeyFormat : _testDataKeyFormat);
            return obj.ToDisplayString(format);
        }
    }
}
