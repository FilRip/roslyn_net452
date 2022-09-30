using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE;
using Microsoft.CodeAnalysis.CSharp.Symbols.Retargeting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.RuntimeMembers;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class CSharpCompilation : Compilation
    {
        internal sealed class NullableData
        {
            internal readonly int MaxRecursionDepth;

            internal readonly ConcurrentDictionary<object, NullableWalker.Data> Data;

            internal NullableData(int maxRecursionDepth = -1)
            {
                MaxRecursionDepth = maxRecursionDepth;
                Data = new ConcurrentDictionary<object, NullableWalker.Data>();
            }
        }

        internal class EntryPoint
        {
            public readonly Symbols.MethodSymbol? MethodSymbol;

            public readonly ImmutableBindingDiagnostic<Symbols.AssemblySymbol> Diagnostics;

            public static readonly EntryPoint None = new EntryPoint(null, ImmutableBindingDiagnostic<Symbols.AssemblySymbol>.Empty);

            public EntryPoint(Symbols.MethodSymbol? methodSymbol, ImmutableBindingDiagnostic<Symbols.AssemblySymbol> diagnostics)
            {
                MethodSymbol = methodSymbol;
                Diagnostics = diagnostics;
            }
        }

        private struct ImportInfo : IEquatable<ImportInfo>
        {
            public readonly SyntaxTree Tree;

            public readonly SyntaxKind Kind;

            public readonly TextSpan Span;

            public ImportInfo(SyntaxTree tree, SyntaxKind kind, TextSpan span)
            {
                Tree = tree;
                Kind = kind;
                Span = span;
            }

            public override bool Equals(object? obj)
            {
                if (obj is ImportInfo)
                {
                    return Equals((ImportInfo)obj);
                }
                return false;
            }

            public bool Equals(ImportInfo other)
            {
                if (other.Kind == Kind && other.Tree == Tree)
                {
                    return other.Span == Span;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return Hash.Combine(Tree, Span.Start);
            }
        }

        private abstract class AbstractSymbolSearcher
        {
            private readonly PooledDictionary<Declaration, Symbols.NamespaceOrTypeSymbol> _cache;

            private readonly CSharpCompilation _compilation;

            private readonly bool _includeNamespace;

            private readonly bool _includeType;

            private readonly bool _includeMember;

            private readonly CancellationToken _cancellationToken;

            protected AbstractSymbolSearcher(CSharpCompilation compilation, SymbolFilter filter, CancellationToken cancellationToken)
            {
                _cache = PooledDictionary<Declaration, Symbols.NamespaceOrTypeSymbol>.GetInstance();
                _compilation = compilation;
                _includeNamespace = (filter & SymbolFilter.Namespace) == SymbolFilter.Namespace;
                _includeType = (filter & SymbolFilter.Type) == SymbolFilter.Type;
                _includeMember = (filter & SymbolFilter.Member) == SymbolFilter.Member;
                _cancellationToken = cancellationToken;
            }

            protected abstract bool Matches(string name);

            protected abstract bool ShouldCheckTypeForMembers(MergedTypeDeclaration current);

            public IEnumerable<Symbol> GetSymbolsWithName()
            {
                HashSet<Symbol> hashSet = new HashSet<Symbol>();
                ArrayBuilder<MergedNamespaceOrTypeDeclaration> instance = ArrayBuilder<MergedNamespaceOrTypeDeclaration>.GetInstance();
                AppendSymbolsWithName(instance, _compilation.MergedRootDeclaration, hashSet);
                instance.Free();
                _cache.Free();
                return hashSet;
            }

            private void AppendSymbolsWithName(ArrayBuilder<MergedNamespaceOrTypeDeclaration> spine, MergedNamespaceOrTypeDeclaration current, HashSet<Symbol> set)
            {
                if (current.Kind == DeclarationKind.Namespace)
                {
                    if (_includeNamespace && Matches(current.Name))
                    {
                        Symbols.NamespaceOrTypeSymbol spineSymbol = GetSpineSymbol(spine);
                        Symbols.NamespaceOrTypeSymbol symbol = GetSymbol(spineSymbol, current);
                        if (symbol != null)
                        {
                            set.Add(symbol);
                        }
                    }
                }
                else
                {
                    if (_includeType && Matches(current.Name))
                    {
                        Symbols.NamespaceOrTypeSymbol spineSymbol2 = GetSpineSymbol(spine);
                        Symbols.NamespaceOrTypeSymbol symbol2 = GetSymbol(spineSymbol2, current);
                        if (symbol2 != null)
                        {
                            set.Add(symbol2);
                        }
                    }
                    if (_includeMember)
                    {
                        MergedTypeDeclaration current2 = (MergedTypeDeclaration)current;
                        if (ShouldCheckTypeForMembers(current2))
                        {
                            AppendMemberSymbolsWithName(spine, current2, set);
                        }
                    }
                }
                spine.Add(current);
                ImmutableArray<Declaration>.Enumerator enumerator = current.Children.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Declaration current3 = enumerator.Current;
                    if (current3 is MergedNamespaceOrTypeDeclaration current4 && (_includeMember || _includeType || current3.Kind == DeclarationKind.Namespace))
                    {
                        AppendSymbolsWithName(spine, current4, set);
                    }
                }
                spine.RemoveAt(spine.Count - 1);
            }

            private void AppendMemberSymbolsWithName(ArrayBuilder<MergedNamespaceOrTypeDeclaration> spine, MergedTypeDeclaration current, HashSet<Symbol> set)
            {
                CancellationToken cancellationToken = _cancellationToken;
                cancellationToken.ThrowIfCancellationRequested();
                spine.Add(current);
                Symbols.NamespaceOrTypeSymbol spineSymbol = GetSpineSymbol(spine);
                if (spineSymbol != null)
                {
                    ImmutableArray<Symbol>.Enumerator enumerator = spineSymbol.GetMembers().GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Symbol current2 = enumerator.Current;
                        if (!current2.IsTypeOrTypeAlias() && (current2.CanBeReferencedByName || current2.IsExplicitInterfaceImplementation() || current2.IsIndexer()) && Matches(current2.Name))
                        {
                            set.Add(current2);
                        }
                    }
                }
                spine.RemoveAt(spine.Count - 1);
            }

            protected Symbols.NamespaceOrTypeSymbol? GetSpineSymbol(ArrayBuilder<MergedNamespaceOrTypeDeclaration> spine)
            {
                if (spine.Count == 0)
                {
                    return null;
                }
                Symbols.NamespaceOrTypeSymbol cachedSymbol = GetCachedSymbol(spine[spine.Count - 1]);
                if (cachedSymbol != null)
                {
                    return cachedSymbol;
                }
                Symbols.NamespaceOrTypeSymbol namespaceOrTypeSymbol = _compilation.GlobalNamespace;
                for (int i = 1; i < spine.Count; i++)
                {
                    namespaceOrTypeSymbol = GetSymbol(namespaceOrTypeSymbol, spine[i]);
                }
                return namespaceOrTypeSymbol;
            }

            private Symbols.NamespaceOrTypeSymbol? GetCachedSymbol(MergedNamespaceOrTypeDeclaration declaration)
            {
                if (!_cache.TryGetValue(declaration, out var value))
                {
                    return null;
                }
                return value;
            }

            private Symbols.NamespaceOrTypeSymbol? GetSymbol(Symbols.NamespaceOrTypeSymbol? container, MergedNamespaceOrTypeDeclaration declaration)
            {
                if (container == null)
                {
                    return _compilation.GlobalNamespace;
                }
                if (declaration.Kind == DeclarationKind.Namespace)
                {
                    AddCache(container!.GetMembers(declaration.Name).OfType<Symbols.NamespaceOrTypeSymbol>());
                }
                else
                {
                    AddCache(container!.GetTypeMembers(declaration.Name));
                }
                return GetCachedSymbol(declaration);
            }

            private void AddCache(IEnumerable<Symbols.NamespaceOrTypeSymbol> symbols)
            {
                foreach (Symbols.NamespaceOrTypeSymbol symbol in symbols)
                {
                    MergedNamespaceSymbol mergedNamespaceSymbol = symbol as MergedNamespaceSymbol;
                    if (mergedNamespaceSymbol != null)
                    {
                        _cache[mergedNamespaceSymbol.ConstituentNamespaces.OfType<SourceNamespaceSymbol>().First().MergedDeclaration] = symbol;
                        continue;
                    }
                    SourceNamespaceSymbol sourceNamespaceSymbol = symbol as SourceNamespaceSymbol;
                    if (sourceNamespaceSymbol != null)
                    {
                        _cache[sourceNamespaceSymbol.MergedDeclaration] = sourceNamespaceSymbol;
                    }
                    else if (symbol is SourceMemberContainerTypeSymbol sourceMemberContainerTypeSymbol)
                    {
                        _cache[sourceMemberContainerTypeSymbol.MergedDeclaration] = sourceMemberContainerTypeSymbol;
                    }
                }
            }
        }

        private class PredicateSymbolSearcher : AbstractSymbolSearcher
        {
            private readonly Func<string, bool> _predicate;

            public PredicateSymbolSearcher(CSharpCompilation compilation, SymbolFilter filter, Func<string, bool> predicate, CancellationToken cancellationToken)
                : base(compilation, filter, cancellationToken)
            {
                _predicate = predicate;
            }

            protected override bool ShouldCheckTypeForMembers(MergedTypeDeclaration current)
            {
                return true;
            }

            protected override bool Matches(string name)
            {
                return _predicate(name);
            }
        }

        private class NameSymbolSearcher : AbstractSymbolSearcher
        {
            private readonly string _name;

            public NameSymbolSearcher(CSharpCompilation compilation, SymbolFilter filter, string name, CancellationToken cancellationToken)
                : base(compilation, filter, cancellationToken)
            {
                _name = name;
            }

            protected override bool ShouldCheckTypeForMembers(MergedTypeDeclaration current)
            {
                ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = current.Declarations.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.MemberNames.Contains(_name))
                    {
                        return true;
                    }
                }
                return false;
            }

            protected override bool Matches(string name)
            {
                return _name == name;
            }
        }

        private class UsingsFromOptionsAndDiagnostics
        {
            public static readonly UsingsFromOptionsAndDiagnostics Empty = new UsingsFromOptionsAndDiagnostics
            {
                UsingNamespacesOrTypes = ImmutableArray<NamespaceOrTypeAndUsingDirective>.Empty,
                Diagnostics = null
            };

            private SymbolCompletionState _state;

            public ImmutableArray<NamespaceOrTypeAndUsingDirective> UsingNamespacesOrTypes { get; init; }

            public DiagnosticBag? Diagnostics { get; set; }

            public static UsingsFromOptionsAndDiagnostics FromOptions(CSharpCompilation compilation)
            {
                ImmutableArray<string> usings = compilation.Options.Usings;
                if (usings.Length == 0)
                {
                    return Empty;
                }
                DiagnosticBag diagnosticBag = new DiagnosticBag();
                InContainerBinder inContainerBinder = new InContainerBinder(compilation.GlobalNamespace, new BuckStopsHereBinder(compilation));
                ArrayBuilder<NamespaceOrTypeAndUsingDirective> instance = ArrayBuilder<NamespaceOrTypeAndUsingDirective>.GetInstance();
                PooledHashSet<Symbols.NamespaceOrTypeSymbol> instance2 = PooledHashSet<Symbols.NamespaceOrTypeSymbol>.GetInstance();
                ImmutableArray<string>.Enumerator enumerator = usings.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    string current = enumerator.Current;
                    if (current.IsValidClrNamespaceName())
                    {
                        string[] array = current.Split(new char[1] { '.' });
                        NameSyntax nameSyntax = SyntaxFactory.IdentifierName(array[0]);
                        for (int i = 1; i < array.Length; i++)
                        {
                            nameSyntax = SyntaxFactory.QualifiedName(nameSyntax, SyntaxFactory.IdentifierName(array[i]));
                        }
                        BindingDiagnosticBag instance3 = BindingDiagnosticBag.GetInstance();
                        Symbols.NamespaceOrTypeSymbol namespaceOrTypeSymbol = inContainerBinder.BindNamespaceOrTypeSymbol(nameSyntax, instance3).NamespaceOrTypeSymbol;
                        if (instance2.Add(namespaceOrTypeSymbol))
                        {
                            instance.Add(new NamespaceOrTypeAndUsingDirective(namespaceOrTypeSymbol, null, instance3.DependenciesBag.ToImmutableArray()));
                        }
                        diagnosticBag.AddRange(instance3.DiagnosticBag);
                        instance3.Free();
                    }
                }
                if (diagnosticBag.IsEmptyWithoutResolution)
                {
                    diagnosticBag = null;
                }
                instance2.Free();
                if (instance.Count == 0 && diagnosticBag == null)
                {
                    instance.Free();
                    return Empty;
                }
                return new UsingsFromOptionsAndDiagnostics
                {
                    UsingNamespacesOrTypes = instance.ToImmutableAndFree(),
                    Diagnostics = diagnosticBag
                };
            }

            internal void Complete(CSharpCompilation compilation, CancellationToken cancellationToken)
            {
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    CompletionPart nextIncompletePart = _state.NextIncompletePart;
                    switch (nextIncompletePart)
                    {
                        case CompletionPart.StartBaseType:
                            if (_state.NotePartComplete(CompletionPart.StartBaseType))
                            {
                                Validate(compilation);
                                _state.NotePartComplete(CompletionPart.FinishBaseType);
                            }
                            break;
                        case CompletionPart.FinishBaseType:
                            _state.SpinWaitComplete(CompletionPart.FinishBaseType, cancellationToken);
                            break;
                        case CompletionPart.None:
                            return;
                        default:
                            _state.NotePartComplete(CompletionPart.MethodSymbolAll | CompletionPart.StartInterfaces | CompletionPart.FinishInterfaces | CompletionPart.EnumUnderlyingType | CompletionPart.TypeArguments | CompletionPart.FinishMemberChecks | CompletionPart.MembersCompleted);
                            break;
                    }
                    _state.SpinWaitComplete(nextIncompletePart, cancellationToken);
                }
            }

            private void Validate(CSharpCompilation compilation)
            {
                CSharpCompilation compilation2 = compilation;
                if (this == Empty)
                {
                    return;
                }
                DiagnosticBag declarationDiagnostics = compilation2.DeclarationDiagnostics;
                BindingDiagnosticBag diagnostics = BindingDiagnosticBag.GetInstance();
                TypeConversions conversions = new TypeConversions(compilation2.SourceAssembly.CorLibrary);
                ImmutableArray<NamespaceOrTypeAndUsingDirective>.Enumerator enumerator = UsingNamespacesOrTypes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    NamespaceOrTypeAndUsingDirective current = enumerator.Current;
                    diagnostics.Clear();
                    diagnostics.AddDependencies(current.Dependencies);
                    Symbols.NamespaceOrTypeSymbol namespaceOrType = current.NamespaceOrType;
                    if (namespaceOrType.IsType)
                    {
                        ((Symbols.TypeSymbol)namespaceOrType).CheckAllConstraints(location: NoLocation.Singleton, compilation: compilation2, conversions: conversions, diagnostics: diagnostics);
                    }
                    declarationDiagnostics.AddRange(diagnostics.DiagnosticBag);
                    recordImportDependencies(namespaceOrType);
                }
                if (Diagnostics != null && !Diagnostics!.IsEmptyWithoutResolution)
                {
                    declarationDiagnostics.AddRange(Diagnostics!.AsEnumerable());
                }
                diagnostics.Free();
                void recordImportDependencies(Symbols.NamespaceOrTypeSymbol target)
                {
                    if (target.IsNamespace)
                    {
                        diagnostics.AddAssembliesUsedByNamespaceReference((Symbols.NamespaceSymbol)target);
                    }
                    compilation2.AddUsedAssemblies(diagnostics.DependenciesBag);
                }
            }
        }

        internal static class TupleNamesEncoder
        {
            public static ImmutableArray<string?> Encode(Symbols.TypeSymbol type)
            {
                ArrayBuilder<string> instance = ArrayBuilder<string>.GetInstance();
                if (!TryGetNames(type, instance))
                {
                    instance.Free();
                    return default(ImmutableArray<string>);
                }
                return instance.ToImmutableAndFree();
            }

            public static ImmutableArray<TypedConstant> Encode(Symbols.TypeSymbol type, Symbols.TypeSymbol stringType)
            {
                ArrayBuilder<string> instance = ArrayBuilder<string>.GetInstance();
                if (!TryGetNames(type, instance))
                {
                    instance.Free();
                    return default(ImmutableArray<TypedConstant>);
                }
                ImmutableArray<TypedConstant> result = instance.SelectAsArray((string name, Symbols.TypeSymbol constantType) => new TypedConstant(constantType, TypedConstantKind.Primitive, name), stringType);
                instance.Free();
                return result;
            }

            internal static bool TryGetNames(Symbols.TypeSymbol type, ArrayBuilder<string?> namesBuilder)
            {
                type.VisitType((Symbols.TypeSymbol t, ArrayBuilder<string> builder, bool _ignore) => AddNames(t, builder), namesBuilder);
                return namesBuilder.Any<string>((string name) => name != null);
            }

            private static bool AddNames(Symbols.TypeSymbol type, ArrayBuilder<string?> namesBuilder)
            {
                if (type.IsTupleType)
                {
                    if (type.TupleElementNames.IsDefaultOrEmpty)
                    {
                        namesBuilder.AddMany(null, type.TupleElementTypesWithAnnotations.Length);
                    }
                    else
                    {
                        namesBuilder.AddRange(type.TupleElementNames);
                    }
                }
                return false;
            }
        }

        internal static class DynamicTransformsEncoder
        {
            internal static ImmutableArray<TypedConstant> Encode(Symbols.TypeSymbol type, RefKind refKind, int customModifiersCount, Symbols.TypeSymbol booleanType)
            {
                ArrayBuilder<bool> instance = ArrayBuilder<bool>.GetInstance();
                Encode(type, customModifiersCount, refKind, instance, addCustomModifierFlags: true);
                ImmutableArray<TypedConstant> result = instance.SelectAsArray((bool flag, Symbols.TypeSymbol constantType) => new TypedConstant(constantType, TypedConstantKind.Primitive, flag), booleanType);
                instance.Free();
                return result;
            }

            internal static ImmutableArray<bool> Encode(Symbols.TypeSymbol type, RefKind refKind, int customModifiersCount)
            {
                ArrayBuilder<bool> instance = ArrayBuilder<bool>.GetInstance();
                Encode(type, customModifiersCount, refKind, instance, addCustomModifierFlags: true);
                return instance.ToImmutableAndFree();
            }

            internal static ImmutableArray<bool> EncodeWithoutCustomModifierFlags(Symbols.TypeSymbol type, RefKind refKind)
            {
                ArrayBuilder<bool> instance = ArrayBuilder<bool>.GetInstance();
                Encode(type, -1, refKind, instance, addCustomModifierFlags: false);
                return instance.ToImmutableAndFree();
            }

            internal static void Encode(Symbols.TypeSymbol type, int customModifiersCount, RefKind refKind, ArrayBuilder<bool> transformFlagsBuilder, bool addCustomModifierFlags)
            {
                if (refKind != 0)
                {
                    transformFlagsBuilder.Add(item: false);
                }
                if (addCustomModifierFlags)
                {
                    HandleCustomModifiers(customModifiersCount, transformFlagsBuilder);
                    type.VisitType((Symbols.TypeSymbol typeSymbol, ArrayBuilder<bool> builder, bool isNested) => AddFlags(typeSymbol, builder, isNested, addCustomModifierFlags: true), transformFlagsBuilder);
                }
                else
                {
                    type.VisitType((Symbols.TypeSymbol typeSymbol, ArrayBuilder<bool> builder, bool isNested) => AddFlags(typeSymbol, builder, isNested, addCustomModifierFlags: false), transformFlagsBuilder);
                }
            }

            private static bool AddFlags(Symbols.TypeSymbol type, ArrayBuilder<bool> transformFlagsBuilder, bool isNestedNamedType, bool addCustomModifierFlags)
            {
                switch (type.TypeKind)
                {
                    case TypeKind.Dynamic:
                        transformFlagsBuilder.Add(item: true);
                        break;
                    case TypeKind.Array:
                        if (addCustomModifierFlags)
                        {
                            HandleCustomModifiers(((Symbols.ArrayTypeSymbol)type).ElementTypeWithAnnotations.CustomModifiers.Length, transformFlagsBuilder);
                        }
                        transformFlagsBuilder.Add(item: false);
                        break;
                    case TypeKind.Pointer:
                        if (addCustomModifierFlags)
                        {
                            HandleCustomModifiers(((Symbols.PointerTypeSymbol)type).PointedAtTypeWithAnnotations.CustomModifiers.Length, transformFlagsBuilder);
                        }
                        transformFlagsBuilder.Add(item: false);
                        break;
                    case TypeKind.FunctionPointer:
                        handleFunctionPointerType((Symbols.FunctionPointerTypeSymbol)type, transformFlagsBuilder, addCustomModifierFlags);
                        return true;
                    default:
                        if (!isNestedNamedType)
                        {
                            transformFlagsBuilder.Add(item: false);
                        }
                        break;
                }
                return false;
                static void handleFunctionPointerType(Symbols.FunctionPointerTypeSymbol funcPtr, ArrayBuilder<bool> transformFlagsBuilder, bool addCustomModifierFlags)
                {
                    ArrayBuilder<bool> transformFlagsBuilder2 = transformFlagsBuilder;
                    Func<Symbols.TypeSymbol, (ArrayBuilder<bool>, bool), bool, bool> visitor = (Symbols.TypeSymbol type, (ArrayBuilder<bool> builder, bool addCustomModifierFlags) param, bool isNestedNamedType) => AddFlags(type, param.builder, isNestedNamedType, param.addCustomModifierFlags);
                    transformFlagsBuilder2.Add(item: false);
                    FunctionPointerMethodSymbol signature = funcPtr.Signature;
                    handle(signature.RefKind, signature.RefCustomModifiers, signature.ReturnTypeWithAnnotations);
                    ImmutableArray<Symbols.ParameterSymbol>.Enumerator enumerator = signature.Parameters.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Symbols.ParameterSymbol current = enumerator.Current;
                        handle(current.RefKind, current.RefCustomModifiers, current.TypeWithAnnotations);
                    }
                    void handle(RefKind refKind, ImmutableArray<CustomModifier> customModifiers, TypeWithAnnotations twa)
                    {
                        if (addCustomModifierFlags)
                        {
                            HandleCustomModifiers(customModifiers.Length, transformFlagsBuilder2);
                        }
                        if (refKind != 0)
                        {
                            transformFlagsBuilder2.Add(item: false);
                        }
                        if (addCustomModifierFlags)
                        {
                            HandleCustomModifiers(twa.CustomModifiers.Length, transformFlagsBuilder2);
                        }
                        twa.Type.VisitType(visitor, (transformFlagsBuilder2, addCustomModifierFlags));
                    }
                }
            }

            private static void HandleCustomModifiers(int customModifiersCount, ArrayBuilder<bool> transformFlagsBuilder)
            {
                transformFlagsBuilder.AddMany(item: false, customModifiersCount);
            }
        }

        internal static class NativeIntegerTransformsEncoder
        {
            internal static void Encode(ArrayBuilder<bool> builder, Symbols.TypeSymbol type)
            {
                type.VisitType((Symbols.TypeSymbol typeSymbol, ArrayBuilder<bool> builder, bool isNested) => AddFlags(typeSymbol, builder), builder);
            }

            private static bool AddFlags(Symbols.TypeSymbol type, ArrayBuilder<bool> builder)
            {
                SpecialType specialType = type.SpecialType;
                if ((uint)(specialType - 21) <= 1u)
                {
                    builder.Add(type.IsNativeIntegerType);
                }
                return false;
            }
        }

        public class SpecialMembersSignatureComparer : SignatureComparer<Symbols.MethodSymbol, Symbols.FieldSymbol, Symbols.PropertySymbol, Symbols.TypeSymbol, Symbols.ParameterSymbol>
        {
            public static readonly SpecialMembersSignatureComparer Instance = new SpecialMembersSignatureComparer();

            protected SpecialMembersSignatureComparer()
            {
            }

            protected override Symbols.TypeSymbol? GetMDArrayElementType(Symbols.TypeSymbol type)
            {
                if (type.Kind != SymbolKind.ArrayType)
                {
                    return null;
                }
                Symbols.ArrayTypeSymbol arrayTypeSymbol = (Symbols.ArrayTypeSymbol)type;
                if (arrayTypeSymbol.IsSZArray)
                {
                    return null;
                }
                return arrayTypeSymbol.ElementType;
            }

            protected override Symbols.TypeSymbol GetFieldType(Symbols.FieldSymbol field)
            {
                return field.Type;
            }

            protected override Symbols.TypeSymbol GetPropertyType(Symbols.PropertySymbol property)
            {
                return property.Type;
            }

            protected override Symbols.TypeSymbol? GetGenericTypeArgument(Symbols.TypeSymbol type, int argumentIndex)
            {
                if (type.Kind != SymbolKind.NamedType)
                {
                    return null;
                }
                Symbols.NamedTypeSymbol namedTypeSymbol = (Symbols.NamedTypeSymbol)type;
                if (namedTypeSymbol.Arity <= argumentIndex)
                {
                    return null;
                }
                if ((object)namedTypeSymbol.ContainingType != null)
                {
                    return null;
                }
                return namedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[argumentIndex].Type;
            }

            protected override Symbols.TypeSymbol? GetGenericTypeDefinition(Symbols.TypeSymbol type)
            {
                if (type.Kind != SymbolKind.NamedType)
                {
                    return null;
                }
                Symbols.NamedTypeSymbol namedTypeSymbol = (Symbols.NamedTypeSymbol)type;
                if ((object)namedTypeSymbol.ContainingType != null)
                {
                    return null;
                }
                if (namedTypeSymbol.Arity == 0)
                {
                    return null;
                }
                return namedTypeSymbol.OriginalDefinition;
            }

            protected override ImmutableArray<Symbols.ParameterSymbol> GetParameters(Symbols.MethodSymbol method)
            {
                return method.Parameters;
            }

            protected override ImmutableArray<Symbols.ParameterSymbol> GetParameters(Symbols.PropertySymbol property)
            {
                return property.Parameters;
            }

            protected override Symbols.TypeSymbol GetParamType(Symbols.ParameterSymbol parameter)
            {
                return parameter.Type;
            }

            protected override Symbols.TypeSymbol? GetPointedToType(Symbols.TypeSymbol type)
            {
                if (type.Kind != SymbolKind.PointerType)
                {
                    return null;
                }
                return ((Symbols.PointerTypeSymbol)type).PointedAtType;
            }

            protected override Symbols.TypeSymbol GetReturnType(Symbols.MethodSymbol method)
            {
                return method.ReturnType;
            }

            protected override Symbols.TypeSymbol? GetSZArrayElementType(Symbols.TypeSymbol type)
            {
                if (type.Kind != SymbolKind.ArrayType)
                {
                    return null;
                }
                Symbols.ArrayTypeSymbol arrayTypeSymbol = (Symbols.ArrayTypeSymbol)type;
                if (!arrayTypeSymbol.IsSZArray)
                {
                    return null;
                }
                return arrayTypeSymbol.ElementType;
            }

            protected override bool IsByRefParam(Symbols.ParameterSymbol parameter)
            {
                return parameter.RefKind != RefKind.None;
            }

            protected override bool IsByRefMethod(Symbols.MethodSymbol method)
            {
                return method.RefKind != RefKind.None;
            }

            protected override bool IsByRefProperty(Symbols.PropertySymbol property)
            {
                return property.RefKind != RefKind.None;
            }

            protected override bool IsGenericMethodTypeParam(Symbols.TypeSymbol type, int paramPosition)
            {
                if (type.Kind != SymbolKind.TypeParameter)
                {
                    return false;
                }
                Symbols.TypeParameterSymbol typeParameterSymbol = (Symbols.TypeParameterSymbol)type;
                if (typeParameterSymbol.ContainingSymbol.Kind != SymbolKind.Method)
                {
                    return false;
                }
                return typeParameterSymbol.Ordinal == paramPosition;
            }

            protected override bool IsGenericTypeParam(Symbols.TypeSymbol type, int paramPosition)
            {
                if (type.Kind != SymbolKind.TypeParameter)
                {
                    return false;
                }
                Symbols.TypeParameterSymbol typeParameterSymbol = (Symbols.TypeParameterSymbol)type;
                if (typeParameterSymbol.ContainingSymbol.Kind != SymbolKind.NamedType)
                {
                    return false;
                }
                return typeParameterSymbol.Ordinal == paramPosition;
            }

            protected override bool MatchArrayRank(Symbols.TypeSymbol type, int countOfDimensions)
            {
                if (type.Kind != SymbolKind.ArrayType)
                {
                    return false;
                }
                return ((Symbols.ArrayTypeSymbol)type).Rank == countOfDimensions;
            }

            protected override bool MatchTypeToTypeId(Symbols.TypeSymbol type, int typeId)
            {
                if ((int)type.OriginalDefinition.SpecialType == typeId)
                {
                    if (type.IsDefinition)
                    {
                        return true;
                    }
                    return type.Equals(type.OriginalDefinition, TypeCompareKind.IgnoreNullableModifiersForReferenceTypes);
                }
                return false;
            }
        }

        internal sealed class WellKnownMembersSignatureComparer : SpecialMembersSignatureComparer
        {
            private readonly CSharpCompilation _compilation;

            public WellKnownMembersSignatureComparer(CSharpCompilation compilation)
            {
                _compilation = compilation;
            }

            protected override bool MatchTypeToTypeId(Symbols.TypeSymbol type, int typeId)
            {
                if (((WellKnownType)typeId).IsWellKnownType())
                {
                    return type.Equals(_compilation.GetWellKnownType((WellKnownType)typeId), TypeCompareKind.IgnoreNullableModifiersForReferenceTypes);
                }
                return base.MatchTypeToTypeId(type, typeId);
            }
        }

        public sealed class ReferenceManager : CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>
        {
            private abstract class AssemblyDataForMetadataOrCompilation : AssemblyData
            {
                private List<Symbols.AssemblySymbol>? _assemblies;

                private readonly AssemblyIdentity _identity;

                private readonly ImmutableArray<AssemblyIdentity> _referencedAssemblies;

                private readonly bool _embedInteropTypes;

                public override AssemblyIdentity Identity => _identity;

                public override IEnumerable<Symbols.AssemblySymbol> AvailableSymbols
                {
                    get
                    {
                        if (_assemblies == null)
                        {
                            _assemblies = new List<Symbols.AssemblySymbol>();
                            AddAvailableSymbols(_assemblies);
                        }
                        return _assemblies;
                    }
                }

                public override ImmutableArray<AssemblyIdentity> AssemblyReferences => _referencedAssemblies;

                public sealed override bool IsLinked => _embedInteropTypes;

                protected AssemblyDataForMetadataOrCompilation(AssemblyIdentity identity, ImmutableArray<AssemblyIdentity> referencedAssemblies, bool embedInteropTypes)
                {
                    _embedInteropTypes = embedInteropTypes;
                    _identity = identity;
                    _referencedAssemblies = referencedAssemblies;
                }

                internal abstract Symbols.AssemblySymbol CreateAssemblySymbol();

                protected abstract void AddAvailableSymbols(List<Symbols.AssemblySymbol> assemblies);

                public override CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.AssemblyReferenceBinding[] BindAssemblyReferences(ImmutableArray<CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.AssemblyData> assemblies, AssemblyIdentityComparer assemblyIdentityComparer)
                {
                    return CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.ResolveReferencedAssemblies(_referencedAssemblies, assemblies, 0, assemblyIdentityComparer);
                }
            }

            private sealed class AssemblyDataForFile : AssemblyDataForMetadataOrCompilation
            {
                public readonly PEAssembly Assembly;

                public readonly WeakList<IAssemblySymbolInternal> CachedSymbols;

                public readonly DocumentationProvider DocumentationProvider;

                private readonly MetadataImportOptions _compilationImportOptions;

                private readonly string _sourceAssemblySimpleName;

                private bool _internalsVisibleComputed;

                private bool _internalsPotentiallyVisibleToCompilation;

                internal bool InternalsMayBeVisibleToCompilation
                {
                    get
                    {
                        if (!_internalsVisibleComputed)
                        {
                            _internalsPotentiallyVisibleToCompilation = CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.InternalsMayBeVisibleToAssemblyBeingCompiled(_sourceAssemblySimpleName, Assembly);
                            _internalsVisibleComputed = true;
                        }
                        return _internalsPotentiallyVisibleToCompilation;
                    }
                }

                internal MetadataImportOptions EffectiveImportOptions
                {
                    get
                    {
                        if (InternalsMayBeVisibleToCompilation && _compilationImportOptions == MetadataImportOptions.Public)
                        {
                            return MetadataImportOptions.Internal;
                        }
                        return _compilationImportOptions;
                    }
                }

                public override bool ContainsNoPiaLocalTypes => Assembly.ContainsNoPiaLocalTypes();

                public override bool DeclaresTheObjectClass => Assembly.DeclaresTheObjectClass;

                public override Compilation? SourceCompilation => null;

                public AssemblyDataForFile(PEAssembly assembly, WeakList<IAssemblySymbolInternal> cachedSymbols, bool embedInteropTypes, DocumentationProvider documentationProvider, string sourceAssemblySimpleName, MetadataImportOptions compilationImportOptions)
                    : base(assembly.Identity, assembly.AssemblyReferences, embedInteropTypes)
                {
                    CachedSymbols = cachedSymbols;
                    Assembly = assembly;
                    DocumentationProvider = documentationProvider;
                    _compilationImportOptions = compilationImportOptions;
                    _sourceAssemblySimpleName = sourceAssemblySimpleName;
                }

                internal override Symbols.AssemblySymbol CreateAssemblySymbol()
                {
                    return new PEAssemblySymbol(Assembly, DocumentationProvider, IsLinked, EffectiveImportOptions);
                }

                protected override void AddAvailableSymbols(List<Symbols.AssemblySymbol> assemblies)
                {
                    lock (CommonReferenceManager.SymbolCacheAndReferenceManagerStateGuard)
                    {
                        foreach (IAssemblySymbolInternal cachedSymbol in CachedSymbols)
                        {
                            PEAssemblySymbol pEAssemblySymbol = cachedSymbol as PEAssemblySymbol;
                            if (IsMatchingAssembly(pEAssemblySymbol))
                            {
                                assemblies.Add(pEAssemblySymbol);
                            }
                        }
                    }
                }

                public override bool IsMatchingAssembly(Symbols.AssemblySymbol? candidateAssembly)
                {
                    return IsMatchingAssembly(candidateAssembly as PEAssemblySymbol);
                }

                private bool IsMatchingAssembly(PEAssemblySymbol? peAssembly)
                {
                    if ((object)peAssembly == null)
                    {
                        return false;
                    }
                    if (peAssembly!.Assembly != Assembly)
                    {
                        return false;
                    }
                    if (EffectiveImportOptions != peAssembly!.PrimaryModule.ImportOptions)
                    {
                        return false;
                    }
                    if (!peAssembly!.DocumentationProvider.Equals(DocumentationProvider))
                    {
                        return false;
                    }
                    return true;
                }
            }

            private sealed class AssemblyDataForCompilation : AssemblyDataForMetadataOrCompilation
            {
                public readonly CSharpCompilation Compilation;

                public override bool ContainsNoPiaLocalTypes => Compilation.MightContainNoPiaLocalTypes();

                public override bool DeclaresTheObjectClass => Compilation.DeclaresTheObjectClass;

                public override Compilation SourceCompilation => Compilation;

                public AssemblyDataForCompilation(CSharpCompilation compilation, bool embedInteropTypes)
                    : base(compilation.Assembly.Identity, GetReferencedAssemblies(compilation), embedInteropTypes)
                {
                    Compilation = compilation;
                }

                private static ImmutableArray<AssemblyIdentity> GetReferencedAssemblies(CSharpCompilation compilation)
                {
                    ArrayBuilder<AssemblyIdentity> instance = ArrayBuilder<AssemblyIdentity>.GetInstance();
                    ImmutableArray<Symbols.ModuleSymbol> modules = compilation.Assembly.Modules;
                    ImmutableArray<AssemblyIdentity> referencedAssemblies = modules[0].GetReferencedAssemblies();
                    ImmutableArray<Symbols.AssemblySymbol> referencedAssemblySymbols = modules[0].GetReferencedAssemblySymbols();
                    for (int i = 0; i < referencedAssemblies.Length; i++)
                    {
                        if (!referencedAssemblySymbols[i].IsLinked)
                        {
                            instance.Add(referencedAssemblies[i]);
                        }
                    }
                    for (int j = 1; j < modules.Length; j++)
                    {
                        instance.AddRange(modules[j].GetReferencedAssemblies());
                    }
                    return instance.ToImmutableAndFree();
                }

                internal override Symbols.AssemblySymbol CreateAssemblySymbol()
                {
                    return new RetargetingAssemblySymbol(Compilation.SourceAssembly, IsLinked);
                }

                protected override void AddAvailableSymbols(List<Symbols.AssemblySymbol> assemblies)
                {
                    assemblies.Add(Compilation.Assembly);
                    lock (CommonReferenceManager.SymbolCacheAndReferenceManagerStateGuard)
                    {
                        Compilation.AddRetargetingAssemblySymbolsNoLock(assemblies);
                    }
                }

                public override bool IsMatchingAssembly(Symbols.AssemblySymbol? candidateAssembly)
                {
                    Symbols.AssemblySymbol assemblySymbol = ((!(candidateAssembly is RetargetingAssemblySymbol retargetingAssemblySymbol)) ? (candidateAssembly as Symbols.SourceAssemblySymbol) : retargetingAssemblySymbol.UnderlyingAssembly);
                    return (object)assemblySymbol == Compilation.Assembly;
                }
            }

            protected override CommonMessageProvider MessageProvider
            {
                get
                {
                    return CSharp.MessageProvider.Instance;
                }
            }

            public ReferenceManager(string simpleAssemblyName, AssemblyIdentityComparer identityComparer, Dictionary<MetadataReference, object>? observedMetadata)
                : base(simpleAssemblyName, identityComparer, observedMetadata)
            {
            }

            protected override CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.AssemblyData CreateAssemblyDataForFile(PEAssembly assembly, WeakList<IAssemblySymbolInternal> cachedSymbols, DocumentationProvider documentationProvider, string sourceAssemblySimpleName, MetadataImportOptions importOptions, bool embedInteropTypes)
            {
                return new AssemblyDataForFile(assembly, cachedSymbols, embedInteropTypes, documentationProvider, sourceAssemblySimpleName, importOptions);
            }

            protected override CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.AssemblyData CreateAssemblyDataForCompilation(CompilationReference compilationReference)
            {
                if (!(compilationReference is CSharpCompilationReference cSharpCompilationReference))
                {
                    throw new NotSupportedException(string.Format(CSharpResources.CantReferenceCompilationOf, compilationReference.GetType(), "C#"));
                }
                return new AssemblyDataForCompilation(cSharpCompilationReference.Compilation, cSharpCompilationReference.Properties.EmbedInteropTypes);
            }

            protected override bool CheckPropertiesConsistency(MetadataReference primaryReference, MetadataReference duplicateReference, DiagnosticBag diagnostics)
            {
                if (primaryReference.Properties.EmbedInteropTypes != duplicateReference.Properties.EmbedInteropTypes)
                {
                    diagnostics.Add(ErrorCode.ERR_AssemblySpecifiedForLinkAndRef, NoLocation.Singleton, duplicateReference.Display, primaryReference.Display);
                    return false;
                }
                return true;
            }

            protected override bool WeakIdentityPropertiesEquivalent(AssemblyIdentity identity1, AssemblyIdentity identity2)
            {
                return AssemblyIdentityComparer.CultureComparer.Equals(identity1.CultureName, identity2.CultureName);
            }

            protected override void GetActualBoundReferencesUsedBy(Symbols.AssemblySymbol assemblySymbol, List<Symbols.AssemblySymbol?> referencedAssemblySymbols)
            {
                ImmutableArray<Symbols.ModuleSymbol>.Enumerator enumerator = assemblySymbol.Modules.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbols.ModuleSymbol current = enumerator.Current;
                    referencedAssemblySymbols.AddRange(current.GetReferencedAssemblySymbols());
                }
                for (int i = 0; i < referencedAssemblySymbols.Count; i++)
                {
                    if (referencedAssemblySymbols[i]!.IsMissing)
                    {
                        referencedAssemblySymbols[i] = null;
                    }
                }
            }

            protected override ImmutableArray<Symbols.AssemblySymbol> GetNoPiaResolutionAssemblies(Symbols.AssemblySymbol candidateAssembly)
            {
                if (candidateAssembly is Symbols.SourceAssemblySymbol)
                {
                    return ImmutableArray<Symbols.AssemblySymbol>.Empty;
                }
                return candidateAssembly.GetNoPiaResolutionAssemblies();
            }

            protected override bool IsLinked(Symbols.AssemblySymbol candidateAssembly)
            {
                return candidateAssembly.IsLinked;
            }

            protected override Symbols.AssemblySymbol? GetCorLibrary(Symbols.AssemblySymbol candidateAssembly)
            {
                Symbols.AssemblySymbol corLibrary = candidateAssembly.CorLibrary;
                if (!corLibrary.IsMissing)
                {
                    return corLibrary;
                }
                return null;
            }

            public void CreateSourceAssemblyForCompilation(CSharpCompilation compilation)
            {
                if (base.IsBound || !CreateAndSetSourceAssemblyFullBind(compilation))
                {
                    if (!base.HasCircularReference)
                    {
                        CreateAndSetSourceAssemblyReuseData(compilation);
                    }
                    else
                    {
                        new ReferenceManager(SimpleAssemblyName, IdentityComparer, ObservedMetadata).CreateAndSetSourceAssemblyFullBind(compilation);
                    }
                }
            }

            public PEAssemblySymbol CreatePEAssemblyForAssemblyMetadata(AssemblyMetadata metadata, MetadataImportOptions importOptions, out ImmutableDictionary<AssemblyIdentity, AssemblyIdentity> assemblyReferenceIdentityMap)
            {
                AssemblyIdentityMap<Symbols.AssemblySymbol> assemblyIdentityMap = new AssemblyIdentityMap<Symbols.AssemblySymbol>();
                ImmutableArray<Symbols.AssemblySymbol>.Enumerator enumerator = base.ReferencedAssemblies.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbols.AssemblySymbol current = enumerator.Current;
                    assemblyIdentityMap.Add(current.Identity, current);
                }
                PEAssembly assembly = metadata.GetAssembly();
                ImmutableArray<Symbols.AssemblySymbol> immutableArray = assembly.AssemblyReferences.SelectAsArray(MapAssemblyIdentityToResolvedSymbol, assemblyIdentityMap);
                assemblyReferenceIdentityMap = CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.GetAssemblyReferenceIdentityBaselineMap(immutableArray, assembly.AssemblyReferences);
                PEAssemblySymbol pEAssemblySymbol = new PEAssemblySymbol(assembly, DocumentationProvider.Default, isLinked: false, importOptions);
                ImmutableArray<UnifiedAssembly<Symbols.AssemblySymbol>> unifiedAssemblies = base.UnifiedAssemblies.WhereAsArray((UnifiedAssembly<Symbols.AssemblySymbol> unified, AssemblyIdentityMap<Symbols.AssemblySymbol> referencedAssembliesByIdentity) => referencedAssembliesByIdentity.Contains(unified.OriginalReference, allowHigherVersion: false), assemblyIdentityMap);
                InitializeAssemblyReuseData(pEAssemblySymbol, immutableArray, unifiedAssemblies);
                if (assembly.ContainsNoPiaLocalTypes())
                {
                    pEAssemblySymbol.SetNoPiaResolutionAssemblies(base.ReferencedAssemblies);
                }
                return pEAssemblySymbol;
            }

            private static Symbols.AssemblySymbol MapAssemblyIdentityToResolvedSymbol(AssemblyIdentity identity, AssemblyIdentityMap<Symbols.AssemblySymbol> map)
            {
                if (map.TryGetValue(identity, out var value, CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.CompareVersionPartsSpecifiedInSource))
                {
                    return value;
                }
                if (map.TryGetValue(identity, out value, (Version v1, Version v2, Symbols.AssemblySymbol s) => true))
                {
                    throw new NotSupportedException(string.Format(CodeAnalysisResources.ChangingVersionOfAssemblyReferenceIsNotAllowedDuringDebugging, identity, value.Identity.Version));
                }
                return new MissingAssemblySymbol(identity);
            }

            private void CreateAndSetSourceAssemblyReuseData(CSharpCompilation compilation)
            {
                string moduleName = compilation.MakeSourceModuleName();
                Symbols.SourceAssemblySymbol sourceAssemblySymbol = new Symbols.SourceAssemblySymbol(compilation, SimpleAssemblyName, moduleName, base.ReferencedModules);
                InitializeAssemblyReuseData(sourceAssemblySymbol, base.ReferencedAssemblies, base.UnifiedAssemblies);
                if ((object)compilation._lazyAssemblySymbol != null)
                {
                    return;
                }
                lock (CommonReferenceManager.SymbolCacheAndReferenceManagerStateGuard)
                {
                    if ((object)compilation._lazyAssemblySymbol == null)
                    {
                        compilation._lazyAssemblySymbol = sourceAssemblySymbol;
                    }
                }
            }

            private void InitializeAssemblyReuseData(Symbols.AssemblySymbol assemblySymbol, ImmutableArray<Symbols.AssemblySymbol> referencedAssemblies, ImmutableArray<UnifiedAssembly<Symbols.AssemblySymbol>> unifiedAssemblies)
            {
                assemblySymbol.SetCorLibrary(base.CorLibraryOpt ?? assemblySymbol);
                ModuleReferences<Symbols.AssemblySymbol> moduleReferences = new ModuleReferences<Symbols.AssemblySymbol>(referencedAssemblies.SelectAsArray((Symbols.AssemblySymbol a) => a.Identity), referencedAssemblies, unifiedAssemblies);
                assemblySymbol.Modules[0].SetReferences(moduleReferences);
                ImmutableArray<Symbols.ModuleSymbol> modules = assemblySymbol.Modules;
                ImmutableArray<ModuleReferences<Symbols.AssemblySymbol>> referencedModulesReferences = base.ReferencedModulesReferences;
                for (int i = 1; i < modules.Length; i++)
                {
                    modules[i].SetReferences(referencedModulesReferences[i - 1]);
                }
            }

            private bool CreateAndSetSourceAssemblyFullBind(CSharpCompilation compilation)
            {
                DiagnosticBag instance = DiagnosticBag.GetInstance();
                PooledDictionary<string, List<ReferencedAssemblyIdentity>> instance2 = PooledDictionary<string, List<ReferencedAssemblyIdentity>>.GetInstance();
                bool referencesSupersedeLowerVersions = compilation.Options.ReferencesSupersedeLowerVersions;
                try
                {
                    ImmutableArray<ResolvedReference> explicitReferenceMap = ResolveMetadataReferences(compilation, instance2, out var references, out var boundReferenceDirectiveMap, out var boundReferenceDirectives, out var assemblies, out var modules, instance);
                    AssemblyDataForAssemblyBeingBuilt item = new AssemblyDataForAssemblyBeingBuilt(new AssemblyIdentity(noThrow: true, SimpleAssemblyName), assemblies, modules);
                    ImmutableArray<AssemblyData> explicitAssemblies = assemblies.Insert(0, item);
                    ImmutableDictionary<AssemblyIdentity, PortableExecutableReference> implicitReferenceResolutions = compilation.ScriptCompilationInfo?.PreviousScriptCompilation?.GetBoundReferenceManager().ImplicitReferenceResolutions ?? ImmutableDictionary<AssemblyIdentity, PortableExecutableReference>.Empty;
                    BoundInputAssembly[] array = Bind(compilation, explicitAssemblies, modules, references, explicitReferenceMap, compilation.Options.MetadataReferenceResolver, compilation.Options.MetadataImportOptions, referencesSupersedeLowerVersions, instance2, out ImmutableArray<AssemblyData> allAssemblies, out ImmutableArray<MetadataReference> implicitlyResolvedReferences, out ImmutableArray<ResolvedReference> implicitlyResolvedReferenceMap, ref implicitReferenceResolutions, instance, out bool hasCircularReference, out int corLibraryIndex);
                    ImmutableArray<MetadataReference> references2 = references.AddRange(implicitlyResolvedReferences);
                    explicitReferenceMap = explicitReferenceMap.AddRange(implicitlyResolvedReferenceMap);
                    CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.BuildReferencedAssembliesAndModulesMaps(array, references2, explicitReferenceMap, modules.Length, assemblies.Length, instance2, referencesSupersedeLowerVersions, out var referencedAssembliesMap, out var referencedModulesMap, out var aliasesOfReferencedAssemblies, out var mergedAssemblyReferencesMapOpt);
                    List<int> list = new List<int>();
                    for (int i = 1; i < array.Length; i++)
                    {
                        ref BoundInputAssembly reference = ref array[i];
                        if ((object)reference.AssemblySymbol == null)
                        {
                            reference.AssemblySymbol = ((AssemblyDataForMetadataOrCompilation)allAssemblies[i]).CreateAssemblySymbol();
                            list.Add(i);
                        }
                    }
                    Symbols.SourceAssemblySymbol sourceAssemblySymbol = new Symbols.SourceAssemblySymbol(compilation, SimpleAssemblyName, compilation.MakeSourceModuleName(), modules);
                    Symbols.AssemblySymbol assemblySymbol = ((corLibraryIndex == 0) ? sourceAssemblySymbol : ((corLibraryIndex <= 0) ? MissingCorLibrarySymbol.Instance : array[corLibraryIndex].AssemblySymbol));
                    sourceAssemblySymbol.SetCorLibrary(assemblySymbol);
                    Dictionary<AssemblyIdentity, MissingAssemblySymbol> missingAssemblies = null;
                    int totalReferencedAssemblyCount = allAssemblies.Length - 1;
                    SetupReferencesForSourceAssembly(sourceAssemblySymbol, modules, totalReferencedAssemblyCount, array, ref missingAssemblies, out var moduleReferences);
                    if (list.Count > 0)
                    {
                        if (hasCircularReference)
                        {
                            array[0].AssemblySymbol = sourceAssemblySymbol;
                        }
                        InitializeNewSymbols(list, sourceAssemblySymbol, allAssemblies, array, missingAssemblies);
                    }
                    if ((object)compilation._lazyAssemblySymbol == null)
                    {
                        lock (CommonReferenceManager.SymbolCacheAndReferenceManagerStateGuard)
                        {
                            if ((object)compilation._lazyAssemblySymbol == null)
                            {
                                if (base.IsBound)
                                {
                                    return false;
                                }
                                UpdateSymbolCacheNoLock(list, allAssemblies, array);
                                InitializeNoLock(referencedAssembliesMap, referencedModulesMap, boundReferenceDirectiveMap, boundReferenceDirectives, references, implicitReferenceResolutions, hasCircularReference, instance.ToReadOnly(), ((object)assemblySymbol == sourceAssemblySymbol) ? null : assemblySymbol, modules, moduleReferences, sourceAssemblySymbol.SourceModule.GetReferencedAssemblySymbols(), aliasesOfReferencedAssemblies, sourceAssemblySymbol.SourceModule.GetUnifiedAssemblies(), mergedAssemblyReferencesMapOpt);
                                compilation._referenceManager = this;
                                compilation._lazyAssemblySymbol = sourceAssemblySymbol;
                            }
                        }
                    }
                    return true;
                }
                finally
                {
                    instance.Free();
                    instance2.Free();
                }
            }

            private static void InitializeNewSymbols(List<int> newSymbols, Symbols.SourceAssemblySymbol sourceAssembly, ImmutableArray<CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.AssemblyData> assemblies, CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.BoundInputAssembly[] bindingResult, Dictionary<AssemblyIdentity, MissingAssemblySymbol>? missingAssemblies)
            {
                Symbols.AssemblySymbol corLibrary = sourceAssembly.CorLibrary;
                foreach (int newSymbol in newSymbols)
                {
                    if (assemblies[newSymbol] is AssemblyDataForCompilation)
                    {
                        SetupReferencesForRetargetingAssembly(bindingResult, ref bindingResult[newSymbol], ref missingAssemblies, sourceAssembly);
                    }
                    else
                    {
                        SetupReferencesForFileAssembly((AssemblyDataForFile)assemblies[newSymbol], bindingResult, ref bindingResult[newSymbol], ref missingAssemblies, sourceAssembly);
                    }
                }
                ArrayBuilder<Symbols.AssemblySymbol> instance = ArrayBuilder<Symbols.AssemblySymbol>.GetInstance();
                ImmutableArray<Symbols.AssemblySymbol> referencedAssemblySymbols = sourceAssembly.Modules[0].GetReferencedAssemblySymbols();
                foreach (int newSymbol2 in newSymbols)
                {
                    ref BoundInputAssembly reference = ref bindingResult[newSymbol2];
                    if (assemblies[newSymbol2].ContainsNoPiaLocalTypes)
                    {
                        reference.AssemblySymbol!.SetNoPiaResolutionAssemblies(referencedAssemblySymbols);
                    }
                    instance.Clear();
                    if (assemblies[newSymbol2].IsLinked)
                    {
                        instance.Add(reference.AssemblySymbol);
                    }
                    AssemblyReferenceBinding[] referenceBinding = reference.ReferenceBinding;
                    for (int i = 0; i < referenceBinding.Length; i++)
                    {
                        AssemblyReferenceBinding assemblyReferenceBinding = referenceBinding[i];
                        if (assemblyReferenceBinding.IsBound && assemblies[assemblyReferenceBinding.DefinitionIndex].IsLinked)
                        {
                            Symbols.AssemblySymbol assemblySymbol = bindingResult[assemblyReferenceBinding.DefinitionIndex].AssemblySymbol;
                            instance.Add(assemblySymbol);
                        }
                    }
                    if (instance.Count > 0)
                    {
                        instance.RemoveDuplicates();
                        reference.AssemblySymbol!.SetLinkedReferencedAssemblies(instance.ToImmutable());
                    }
                    reference.AssemblySymbol!.SetCorLibrary(corLibrary);
                }
                instance.Free();
                if (missingAssemblies == null)
                {
                    return;
                }
                foreach (MissingAssemblySymbol value in missingAssemblies!.Values)
                {
                    value.SetCorLibrary(corLibrary);
                }
            }

            private static void UpdateSymbolCacheNoLock(List<int> newSymbols, ImmutableArray<CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.AssemblyData> assemblies, CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.BoundInputAssembly[] bindingResult)
            {
                foreach (int newSymbol in newSymbols)
                {
                    ref BoundInputAssembly reference = ref bindingResult[newSymbol];
                    if (assemblies[newSymbol] is AssemblyDataForCompilation assemblyDataForCompilation)
                    {
                        assemblyDataForCompilation.Compilation.CacheRetargetingAssemblySymbolNoLock(reference.AssemblySymbol);
                    }
                    else
                    {
                        ((AssemblyDataForFile)assemblies[newSymbol]).CachedSymbols.Add((PEAssemblySymbol)reference.AssemblySymbol);
                    }
                }
            }

            private static void SetupReferencesForRetargetingAssembly(CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.BoundInputAssembly[] bindingResult, ref CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.BoundInputAssembly currentBindingResult, ref Dictionary<AssemblyIdentity, MissingAssemblySymbol>? missingAssemblies, Symbols.SourceAssemblySymbol sourceAssemblyDebugOnly)
            {
                RetargetingAssemblySymbol retargetingAssemblySymbol = (RetargetingAssemblySymbol)currentBindingResult.AssemblySymbol;
                ImmutableArray<Symbols.ModuleSymbol> modules = retargetingAssemblySymbol.Modules;
                int length = modules.Length;
                int num = 0;
                for (int i = 0; i < length; i++)
                {
                    ImmutableArray<AssemblyIdentity> identities = retargetingAssemblySymbol.UnderlyingAssembly.Modules[i].GetReferencedAssemblies();
                    if (i == 0)
                    {
                        ImmutableArray<Symbols.AssemblySymbol> referencedAssemblySymbols = retargetingAssemblySymbol.UnderlyingAssembly.Modules[0].GetReferencedAssemblySymbols();
                        int num2 = 0;
                        ImmutableArray<Symbols.AssemblySymbol>.Enumerator enumerator = referencedAssemblySymbols.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            if (enumerator.Current.IsLinked)
                            {
                                num2++;
                            }
                        }
                        if (num2 > 0)
                        {
                            AssemblyIdentity[] array = new AssemblyIdentity[identities.Length - num2];
                            int num3 = 0;
                            for (int j = 0; j < referencedAssemblySymbols.Length; j++)
                            {
                                if (!referencedAssemblySymbols[j].IsLinked)
                                {
                                    array[num3] = identities[j];
                                    num3++;
                                }
                            }
                            identities = array.AsImmutableOrNull();
                        }
                    }
                    int length2 = identities.Length;
                    Symbols.AssemblySymbol[] array2 = new Symbols.AssemblySymbol[length2];
                    ArrayBuilder<UnifiedAssembly<Symbols.AssemblySymbol>> unifiedAssemblies = null;
                    for (int k = 0; k < length2; k++)
                    {
                        AssemblyReferenceBinding referenceBinding = currentBindingResult.ReferenceBinding[num + k];
                        if (referenceBinding.IsBound)
                        {
                            array2[k] = GetAssemblyDefinitionSymbol(bindingResult, referenceBinding, ref unifiedAssemblies);
                        }
                        else
                        {
                            array2[k] = GetOrAddMissingAssemblySymbol(identities[k], ref missingAssemblies);
                        }
                    }
                    ModuleReferences<Symbols.AssemblySymbol> moduleReferences = new ModuleReferences<Symbols.AssemblySymbol>(identities, array2.AsImmutableOrNull(), unifiedAssemblies.AsImmutableOrEmpty());
                    modules[i].SetReferences(moduleReferences, sourceAssemblyDebugOnly);
                    num += length2;
                }
            }

            private static void SetupReferencesForFileAssembly(AssemblyDataForFile fileData, CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.BoundInputAssembly[] bindingResult, ref CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.BoundInputAssembly currentBindingResult, ref Dictionary<AssemblyIdentity, MissingAssemblySymbol>? missingAssemblies, Symbols.SourceAssemblySymbol sourceAssemblyDebugOnly)
            {
                ImmutableArray<Symbols.ModuleSymbol> modules = ((PEAssemblySymbol)currentBindingResult.AssemblySymbol).Modules;
                int length = modules.Length;
                int num = 0;
                for (int i = 0; i < length; i++)
                {
                    int num2 = fileData.Assembly.ModuleReferenceCounts[i];
                    AssemblyIdentity[] array = new AssemblyIdentity[num2];
                    Symbols.AssemblySymbol[] array2 = new Symbols.AssemblySymbol[num2];
                    fileData.AssemblyReferences.CopyTo(num, array, 0, num2);
                    ArrayBuilder<UnifiedAssembly<Symbols.AssemblySymbol>> unifiedAssemblies = null;
                    for (int j = 0; j < num2; j++)
                    {
                        AssemblyReferenceBinding referenceBinding = currentBindingResult.ReferenceBinding[num + j];
                        if (referenceBinding.IsBound)
                        {
                            array2[j] = GetAssemblyDefinitionSymbol(bindingResult, referenceBinding, ref unifiedAssemblies);
                        }
                        else
                        {
                            array2[j] = GetOrAddMissingAssemblySymbol(array[j], ref missingAssemblies);
                        }
                    }
                    ModuleReferences<Symbols.AssemblySymbol> moduleReferences = new ModuleReferences<Symbols.AssemblySymbol>(array.AsImmutableOrNull(), array2.AsImmutableOrNull(), unifiedAssemblies.AsImmutableOrEmpty());
                    modules[i].SetReferences(moduleReferences, sourceAssemblyDebugOnly);
                    num += num2;
                }
            }

            private static void SetupReferencesForSourceAssembly(Symbols.SourceAssemblySymbol sourceAssembly, ImmutableArray<PEModule> modules, int totalReferencedAssemblyCount, CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.BoundInputAssembly[] bindingResult, ref Dictionary<AssemblyIdentity, MissingAssemblySymbol>? missingAssemblies, out ImmutableArray<ModuleReferences<Symbols.AssemblySymbol>> moduleReferences)
            {
                ImmutableArray<Symbols.ModuleSymbol> modules2 = sourceAssembly.Modules;
                ArrayBuilder<ModuleReferences<Symbols.AssemblySymbol>> arrayBuilder = ((modules2.Length > 1) ? ArrayBuilder<ModuleReferences<Symbols.AssemblySymbol>>.GetInstance() : null);
                int num = 0;
                for (int i = 0; i < modules2.Length; i++)
                {
                    int num2 = ((i == 0) ? totalReferencedAssemblyCount : modules[i - 1].ReferencedAssemblies.Length);
                    AssemblyIdentity[] array = new AssemblyIdentity[num2];
                    Symbols.AssemblySymbol[] array2 = new Symbols.AssemblySymbol[num2];
                    ArrayBuilder<UnifiedAssembly<Symbols.AssemblySymbol>> unifiedAssemblies = null;
                    for (int j = 0; j < num2; j++)
                    {
                        AssemblyReferenceBinding referenceBinding = bindingResult[0].ReferenceBinding[num + j];
                        if (referenceBinding.IsBound)
                        {
                            array2[j] = GetAssemblyDefinitionSymbol(bindingResult, referenceBinding, ref unifiedAssemblies);
                        }
                        else
                        {
                            array2[j] = GetOrAddMissingAssemblySymbol(referenceBinding.ReferenceIdentity, ref missingAssemblies);
                        }
                        array[j] = referenceBinding.ReferenceIdentity;
                    }
                    ModuleReferences<Symbols.AssemblySymbol> moduleReferences2 = new ModuleReferences<Symbols.AssemblySymbol>(array.AsImmutableOrNull(), array2.AsImmutableOrNull(), unifiedAssemblies.AsImmutableOrEmpty());
                    if (i > 0)
                    {
                        arrayBuilder.Add(moduleReferences2);
                    }
                    modules2[i].SetReferences(moduleReferences2, sourceAssembly);
                    num += num2;
                }
                moduleReferences = arrayBuilder.ToImmutableOrEmptyAndFree();
            }

            private static Symbols.AssemblySymbol GetAssemblyDefinitionSymbol(CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.BoundInputAssembly[] bindingResult, CommonReferenceManager<CSharpCompilation, Symbols.AssemblySymbol>.AssemblyReferenceBinding referenceBinding, ref ArrayBuilder<UnifiedAssembly<Symbols.AssemblySymbol>>? unifiedAssemblies)
            {
                Symbols.AssemblySymbol assemblySymbol = bindingResult[referenceBinding.DefinitionIndex].AssemblySymbol;
                if (referenceBinding.VersionDifference != 0)
                {
                    if (unifiedAssemblies == null)
                    {
                        unifiedAssemblies = new ArrayBuilder<UnifiedAssembly<Symbols.AssemblySymbol>>();
                    }
                    unifiedAssemblies!.Add(new UnifiedAssembly<Symbols.AssemblySymbol>(assemblySymbol, referenceBinding.ReferenceIdentity));
                }
                return assemblySymbol;
            }

            private static MissingAssemblySymbol GetOrAddMissingAssemblySymbol(AssemblyIdentity assemblyIdentity, ref Dictionary<AssemblyIdentity, MissingAssemblySymbol>? missingAssemblies)
            {
                MissingAssemblySymbol value;
                if (missingAssemblies == null)
                {
                    missingAssemblies = new Dictionary<AssemblyIdentity, MissingAssemblySymbol>();
                }
                else if (missingAssemblies!.TryGetValue(assemblyIdentity, out value))
                {
                    return value;
                }
                value = new MissingAssemblySymbol(assemblyIdentity);
                missingAssemblies!.Add(assemblyIdentity, value);
                return value;
            }

            internal static bool IsSourceAssemblySymbolCreated(CSharpCompilation compilation)
            {
                return (object)compilation._lazyAssemblySymbol != null;
            }

            internal static bool IsReferenceManagerInitialized(CSharpCompilation compilation)
            {
                return compilation._referenceManager.IsBound;
            }
        }

        private readonly CSharpCompilationOptions _options;

        private readonly Lazy<UsingsFromOptionsAndDiagnostics> _usingsFromOptions;

        private readonly Lazy<ImmutableArray<NamespaceOrTypeAndUsingDirective>> _globalImports;

        private readonly Lazy<Imports> _previousSubmissionImports;

        private readonly Lazy<Symbols.AliasSymbol> _globalNamespaceAlias;

        private readonly Lazy<ImplicitNamedTypeSymbol?> _scriptClass;

        private Symbols.TypeSymbol? _lazyHostObjectTypeSymbol;

        private ConcurrentDictionary<ImportInfo, ImmutableArray<Symbols.AssemblySymbol>>? _lazyImportInfos;

        private ImmutableArray<Diagnostic> _lazyClsComplianceDiagnostics;

        private ImmutableArray<Symbols.AssemblySymbol> _lazyClsComplianceDependencies;

        private Conversions? _conversions;

        private readonly AnonymousTypeManager _anonymousTypeManager;

        private Symbols.NamespaceSymbol? _lazyGlobalNamespace;

        internal readonly BuiltInOperators builtInOperators;

        private Symbols.SourceAssemblySymbol? _lazyAssemblySymbol;

        private ReferenceManager _referenceManager;

        private readonly SyntaxAndDeclarationManager _syntaxAndDeclarations;

        private EntryPoint? _lazyEntryPoint;

        private ThreeState _lazyEmitNullablePublicOnly;

        private HashSet<SyntaxTree>? _lazyCompilationUnitCompletedTrees;

        private ImmutableHashSet<SyntaxTree>? _usageOfUsingsRecordedInTrees = ImmutableHashSet<SyntaxTree>.Empty;

        internal NullableData? NullableAnalysisData;

        private static readonly CSharpCompilationOptions s_defaultOptions = new CSharpCompilationOptions(OutputKind.ConsoleApplication);

        private static readonly CSharpCompilationOptions s_defaultSubmissionOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary).WithReferencesSupersedeLowerVersions(value: true);

        private ConcurrentDictionary<string, Symbols.NamespaceSymbol>? _externAliasTargets;

        private ConcurrentSet<Symbols.MethodSymbol>? _moduleInitializerMethods;

        private WeakReference<BinderFactory>[]? _binderFactories;

        private WeakReference<BinderFactory>[]? _ignoreAccessibilityBinderFactories;

        private DiagnosticBag? _lazyDeclarationDiagnostics;

        private bool _declarationDiagnosticsFrozen;

        private readonly DiagnosticBag _additionalCodegenWarnings = new DiagnosticBag();

        private ConcurrentSet<Symbols.AssemblySymbol>? _lazyUsedAssemblyReferences;

        private bool _usedAssemblyReferencesFrozen;

        internal readonly WellKnownMembersSignatureComparer WellKnownMemberSignatureComparer;

        private Symbols.NamedTypeSymbol?[]? _lazyWellKnownTypes;

        private Symbol?[]? _lazyWellKnownTypeMembers;

        private bool _usesNullableAttributes;

        private int _needsGeneratedAttributes;

        private bool _needsGeneratedAttributes_IsFrozen;

        internal Conversions Conversions
        {
            get
            {
                if (_conversions == null)
                {
                    Interlocked.CompareExchange(ref _conversions, new BuckStopsHereBinder(this).Conversions, null);
                }
                return _conversions;
            }
        }

        internal ImmutableHashSet<SyntaxTree>? UsageOfUsingsRecordedInTrees => Volatile.Read(ref _usageOfUsingsRecordedInTrees);

        public override string Language => "C#";

        public override bool IsCaseSensitive => true;

        public new CSharpCompilationOptions Options => _options;

        internal AnonymousTypeManager AnonymousTypeManager => _anonymousTypeManager;

        public override CommonAnonymousTypeManager CommonAnonymousTypeManager => AnonymousTypeManager;

        internal bool FeatureStrictEnabled => Feature("strict") != null;

        internal bool IsPeVerifyCompatEnabled
        {
            get
            {
                if (LanguageVersion >= LanguageVersion.CSharp7_2)
                {
                    return Feature("peverify-compat") != null;
                }
                return true;
            }
        }

        internal bool IsNullableAnalysisEnabledAlways => GetNullableAnalysisValue() == true;

        public LanguageVersion LanguageVersion { get; }

        public new CSharpScriptCompilationInfo? ScriptCompilationInfo { get; }

        public override ScriptCompilationInfo? CommonScriptCompilationInfo => ScriptCompilationInfo;

        internal CSharpCompilation? PreviousSubmission => ScriptCompilationInfo?.PreviousScriptCompilation;

        public new ImmutableArray<SyntaxTree> SyntaxTrees => _syntaxAndDeclarations.GetLazyState().SyntaxTrees;

        public override ImmutableArray<MetadataReference> DirectiveReferences => GetBoundReferenceManager().DirectiveReferences;

        public override IDictionary<(string path, string content), MetadataReference> ReferenceDirectiveMap => GetBoundReferenceManager().ReferenceDirectiveMap;

        internal IEnumerable<string> ExternAliases => GetBoundReferenceManager().ExternAliases;

        public override IEnumerable<AssemblyIdentity> ReferencedAssemblyNames => Assembly.Modules.SelectMany((Symbols.ModuleSymbol module) => module.GetReferencedAssemblies());

        public override IEnumerable<ReferenceDirective> ReferenceDirectives => Declarations.ReferenceDirectives;

        internal Symbols.SourceAssemblySymbol SourceAssembly
        {
            get
            {
                GetBoundReferenceManager();
                return _lazyAssemblySymbol;
            }
        }

        internal new Symbols.AssemblySymbol Assembly => SourceAssembly;

        internal new Symbols.ModuleSymbol SourceModule => Assembly.Modules[0];

        internal new Symbols.NamespaceSymbol GlobalNamespace
        {
            get
            {
                if ((object)_lazyGlobalNamespace == null)
                {
                    ArrayBuilder<Symbols.ModuleSymbol> instance = ArrayBuilder<Symbols.ModuleSymbol>.GetInstance();
                    GetAllUnaliasedModules(instance);
                    Symbols.NamespaceSymbol value = MergedNamespaceSymbol.Create(new NamespaceExtent(this), null, instance.SelectDistinct((Symbols.ModuleSymbol m) => m.GlobalNamespace));
                    instance.Free();
                    Interlocked.CompareExchange(ref _lazyGlobalNamespace, value, null);
                }
                return _lazyGlobalNamespace;
            }
        }

        internal new Symbols.NamedTypeSymbol? ScriptClass => _scriptClass.Value;

        internal ImmutableArray<NamespaceOrTypeAndUsingDirective> GlobalImports => _globalImports.Value;

        private UsingsFromOptionsAndDiagnostics UsingsFromOptions => _usingsFromOptions.Value;

        internal Symbols.AliasSymbol GlobalNamespaceAlias => _globalNamespaceAlias.Value;

        protected override ITypeSymbol? CommonScriptGlobalsType => GetHostObjectTypeSymbol()?.GetPublicSymbol();

        internal new Symbols.TypeSymbol DynamicType => Symbols.AssemblySymbol.DynamicType;

        internal new Symbols.NamedTypeSymbol ObjectType => Assembly.ObjectType;

        internal bool DeclaresTheObjectClass => SourceAssembly.DeclaresTheObjectClass;

        public override CommonMessageProvider MessageProvider => _syntaxAndDeclarations.MessageProvider;

        internal DiagnosticBag DeclarationDiagnostics
        {
            get
            {
                if (_lazyDeclarationDiagnostics == null)
                {
                    DiagnosticBag value = new DiagnosticBag();
                    Interlocked.CompareExchange(ref _lazyDeclarationDiagnostics, value, null);
                }
                return _lazyDeclarationDiagnostics;
            }
        }

        internal DiagnosticBag AdditionalCodegenWarnings => _additionalCodegenWarnings;

        internal DeclarationTable Declarations => _syntaxAndDeclarations.GetLazyState().DeclarationTable;

        internal MergedNamespaceDeclaration MergedRootDeclaration => Declarations.GetMergedRoot(this);

        public override byte LinkerMajorVersion => 48;

        public override bool IsDelaySigned => SourceAssembly.IsDelaySigned;

        public override StrongNameKeys StrongNameKeys => SourceAssembly.StrongNameKeys;

        public override Guid DebugSourceDocumentLanguageId => DebugSourceDocument.CorSymLanguageTypeCSharp;

        protected override IAssemblySymbol CommonAssembly => Assembly.GetPublicSymbol();

        protected override INamespaceSymbol CommonGlobalNamespace => GlobalNamespace.GetPublicSymbol();

        protected override CompilationOptions CommonOptions => _options;

        protected override ImmutableArray<SyntaxTree> CommonSyntaxTrees => SyntaxTrees;

        protected override IModuleSymbol CommonSourceModule => SourceModule.GetPublicSymbol();

        protected override INamedTypeSymbol? CommonScriptClass => ScriptClass.GetPublicSymbol();

        protected override ITypeSymbol CommonDynamicType => DynamicType.GetPublicSymbol();

        protected override INamedTypeSymbol CommonObjectType => ObjectType.GetPublicSymbol();

        internal bool EmitNullablePublicOnly
        {
            get
            {
                if (!_lazyEmitNullablePublicOnly.HasValue())
                {
                    SyntaxTree? syntaxTree = SyntaxTrees.FirstOrDefault();
                    bool value = syntaxTree != null && syntaxTree!.Options?.Features?.ContainsKey("nullablePublicOnly") == true;
                    _lazyEmitNullablePublicOnly = value.ToThreeState();
                }
                return _lazyEmitNullablePublicOnly.Value();
            }
        }

        internal bool EnableEnumArrayBlockInitialization
        {
            get
            {
                Symbol wellKnownTypeMember = GetWellKnownTypeMember(WellKnownMember.System_Runtime_GCLatencyMode__SustainedLowLatency);
                if (wellKnownTypeMember != null)
                {
                    return wellKnownTypeMember.ContainingAssembly == Assembly.CorLibrary;
                }
                return false;
            }
        }

        internal bool IsNullableAnalysisEnabledIn(SyntaxNode syntax)
        {
            return IsNullableAnalysisEnabledIn((CSharpSyntaxTree)syntax.SyntaxTree, syntax.Span);
        }

        internal bool IsNullableAnalysisEnabledIn(CSharpSyntaxTree tree, TextSpan span)
        {
            return GetNullableAnalysisValue() ?? tree.IsNullableAnalysisEnabled(span) ?? ((Options.NullableContextOptions & NullableContextOptions.Warnings) != 0);
        }

        internal bool IsNullableAnalysisEnabledIn(Symbols.MethodSymbol method)
        {
            return GetNullableAnalysisValue() ?? method.IsNullableAnalysisEnabled();
        }

        private bool? GetNullableAnalysisValue()
        {
            string text = Feature("run-nullable-analysis");
            if (!(text == "always"))
            {
                if (text == "never")
                {
                    return false;
                }
                return null;
            }
            return true;
        }

        protected override INamedTypeSymbol CommonCreateErrorTypeSymbol(INamespaceOrTypeSymbol? container, string name, int arity)
        {
            return new ExtendedErrorTypeSymbol(container.EnsureCSharpSymbolOrNull("container"), name, arity, null).GetPublicSymbol();
        }

        protected override INamespaceSymbol CommonCreateErrorNamespaceSymbol(INamespaceSymbol container, string name)
        {
            return new MissingNamespaceSymbol(container.EnsureCSharpSymbolOrNull("container"), name).GetPublicSymbol();
        }

        public static CSharpCompilation Create(string? assemblyName, IEnumerable<SyntaxTree>? syntaxTrees = null, IEnumerable<MetadataReference>? references = null, CSharpCompilationOptions? options = null)
        {
            return Create(assemblyName, options ?? s_defaultOptions, syntaxTrees, references, null, null, null, isSubmission: false);
        }

        public static CSharpCompilation CreateScriptCompilation(string assemblyName, SyntaxTree? syntaxTree = null, IEnumerable<MetadataReference>? references = null, CSharpCompilationOptions? options = null, CSharpCompilation? previousScriptCompilation = null, Type? returnType = null, Type? globalsType = null)
        {
            Compilation.CheckSubmissionOptions(options);
            Compilation.ValidateScriptCompilationParameters(previousScriptCompilation, returnType, ref globalsType);
            CSharpCompilationOptions options2 = options?.WithReferencesSupersedeLowerVersions(value: true) ?? s_defaultSubmissionOptions;
            IEnumerable<SyntaxTree> syntaxTrees;
            if (syntaxTree == null)
            {
                syntaxTrees = SpecializedCollections.EmptyEnumerable<SyntaxTree>();
            }
            else
            {
                IEnumerable<SyntaxTree> enumerable = new SyntaxTree[1] { syntaxTree };
                syntaxTrees = enumerable;
            }
            return Create(assemblyName, options2, syntaxTrees, references, previousScriptCompilation, returnType, globalsType, isSubmission: true);
        }

        private static CSharpCompilation Create(string? assemblyName, CSharpCompilationOptions options, IEnumerable<SyntaxTree>? syntaxTrees, IEnumerable<MetadataReference>? references, CSharpCompilation? previousSubmission, Type? returnType, Type? hostObjectType, bool isSubmission)
        {
            ImmutableArray<MetadataReference> references2 = Compilation.ValidateReferences<CSharpCompilationReference>(references);
            CSharpCompilation cSharpCompilation = new CSharpCompilation(assemblyName, options, references2, previousSubmission, returnType, hostObjectType, isSubmission, null, reuseReferenceManager: false, new SyntaxAndDeclarationManager(ImmutableArray<SyntaxTree>.Empty, options.ScriptClassName, options.SourceReferenceResolver, CSharp.MessageProvider.Instance, isSubmission, null), null);
            if (syntaxTrees != null)
            {
                cSharpCompilation = cSharpCompilation.AddSyntaxTrees(syntaxTrees);
            }
            return cSharpCompilation;
        }

        private CSharpCompilation(string? assemblyName, CSharpCompilationOptions options, ImmutableArray<MetadataReference> references, CSharpCompilation? previousSubmission, Type? submissionReturnType, Type? hostObjectType, bool isSubmission, ReferenceManager? referenceManager, bool reuseReferenceManager, SyntaxAndDeclarationManager syntaxAndDeclarations, SemanticModelProvider? semanticModelProvider, AsyncQueue<CompilationEvent>? eventQueue = null)
            : this(assemblyName, options, references, previousSubmission, submissionReturnType, hostObjectType, isSubmission, referenceManager, reuseReferenceManager, syntaxAndDeclarations, Compilation.SyntaxTreeCommonFeatures(syntaxAndDeclarations.ExternalSyntaxTrees), semanticModelProvider, eventQueue)
        {
        }

        private CSharpCompilation(string? assemblyName, CSharpCompilationOptions options, ImmutableArray<MetadataReference> references, CSharpCompilation? previousSubmission, Type? submissionReturnType, Type? hostObjectType, bool isSubmission, ReferenceManager? referenceManager, bool reuseReferenceManager, SyntaxAndDeclarationManager syntaxAndDeclarations, IReadOnlyDictionary<string, string> features, SemanticModelProvider? semanticModelProvider, AsyncQueue<CompilationEvent>? eventQueue = null)
            : base(assemblyName, references, features, isSubmission, semanticModelProvider, eventQueue)
        {
            WellKnownMemberSignatureComparer = new WellKnownMembersSignatureComparer(this);
            _options = options;
            builtInOperators = new BuiltInOperators(this);
            _scriptClass = new Lazy<ImplicitNamedTypeSymbol>(BindScriptClass);
            _globalImports = new Lazy<ImmutableArray<NamespaceOrTypeAndUsingDirective>>(BindGlobalImports);
            _usingsFromOptions = new Lazy<UsingsFromOptionsAndDiagnostics>(BindUsingsFromOptions);
            _previousSubmissionImports = new Lazy<Imports>(ExpandPreviousSubmissionImports);
            _globalNamespaceAlias = new Lazy<Symbols.AliasSymbol>(CreateGlobalNamespaceAlias);
            _anonymousTypeManager = new AnonymousTypeManager(this);
            LanguageVersion = CommonLanguageVersion(syntaxAndDeclarations.ExternalSyntaxTrees);
            if (isSubmission)
            {
                ScriptCompilationInfo = new CSharpScriptCompilationInfo(previousSubmission, submissionReturnType, hostObjectType);
            }
            if (reuseReferenceManager)
            {
                if (referenceManager == null)
                {
                    throw new ArgumentNullException("referenceManager");
                }
                _referenceManager = referenceManager;
            }
            else
            {
                _referenceManager = new ReferenceManager(MakeSourceAssemblySimpleName(), Options.AssemblyIdentityComparer, referenceManager?.ObservedMetadata);
            }
            _syntaxAndDeclarations = syntaxAndDeclarations;
            if (base.EventQueue != null)
            {
                base.EventQueue!.TryEnqueue(new CompilationStartedEvent(this));
            }
        }

        public override void ValidateDebugEntryPoint(IMethodSymbol debugEntryPoint, DiagnosticBag diagnostics)
        {
            Symbols.MethodSymbol methodSymbol = (debugEntryPoint as Symbols.PublicModel.MethodSymbol)?.UnderlyingMethodSymbol;
            if (methodSymbol?.DeclaringCompilation != this || !methodSymbol.IsDefinition)
            {
                diagnostics.Add(ErrorCode.ERR_DebugEntryPointNotSourceMethodDefinition, Location.None);
            }
        }

        private static LanguageVersion CommonLanguageVersion(ImmutableArray<SyntaxTree> syntaxTrees)
        {
            LanguageVersion? languageVersion = null;
            ImmutableArray<SyntaxTree>.Enumerator enumerator = syntaxTrees.GetEnumerator();
            while (enumerator.MoveNext())
            {
                LanguageVersion languageVersion2 = ((CSharpParseOptions)enumerator.Current.Options).LanguageVersion;
                if (!languageVersion.HasValue)
                {
                    languageVersion = languageVersion2;
                }
                else if (languageVersion != languageVersion2)
                {
                    throw new ArgumentException(CodeAnalysisResources.InconsistentLanguageVersions, "syntaxTrees");
                }
            }
            return languageVersion ?? LanguageVersion.Default.MapSpecifiedToEffectiveVersion();
        }

        public new CSharpCompilation Clone()
        {
            return new CSharpCompilation(base.AssemblyName, _options, base.ExternalReferences, PreviousSubmission, base.SubmissionReturnType, base.HostObjectType, base.IsSubmission, _referenceManager, reuseReferenceManager: true, _syntaxAndDeclarations, base.SemanticModelProvider);
        }

        private CSharpCompilation Update(ReferenceManager referenceManager, bool reuseReferenceManager, SyntaxAndDeclarationManager syntaxAndDeclarations)
        {
            return new CSharpCompilation(base.AssemblyName, _options, base.ExternalReferences, PreviousSubmission, base.SubmissionReturnType, base.HostObjectType, base.IsSubmission, referenceManager, reuseReferenceManager, syntaxAndDeclarations, base.SemanticModelProvider);
        }

        public new CSharpCompilation WithAssemblyName(string? assemblyName)
        {
            return new CSharpCompilation(assemblyName, _options, base.ExternalReferences, PreviousSubmission, base.SubmissionReturnType, base.HostObjectType, base.IsSubmission, _referenceManager, assemblyName == base.AssemblyName, _syntaxAndDeclarations, base.SemanticModelProvider);
        }

        public new CSharpCompilation WithReferences(IEnumerable<MetadataReference>? references)
        {
            return new CSharpCompilation(base.AssemblyName, _options, Compilation.ValidateReferences<CSharpCompilationReference>(references), PreviousSubmission, base.SubmissionReturnType, base.HostObjectType, base.IsSubmission, null, reuseReferenceManager: false, _syntaxAndDeclarations, base.SemanticModelProvider);
        }

        public new CSharpCompilation WithReferences(params MetadataReference[] references)
        {
            return WithReferences((IEnumerable<MetadataReference>?)references);
        }

        public CSharpCompilation WithOptions(CSharpCompilationOptions options)
        {
            CSharpCompilationOptions options2 = Options;
            bool reuseReferenceManager = options2.CanReuseCompilationReferenceManager(options);
            bool flag = options2.ScriptClassName == options.ScriptClassName && options2.SourceReferenceResolver == options.SourceReferenceResolver;
            return new CSharpCompilation(base.AssemblyName, options, base.ExternalReferences, PreviousSubmission, base.SubmissionReturnType, base.HostObjectType, base.IsSubmission, _referenceManager, reuseReferenceManager, flag ? _syntaxAndDeclarations : new SyntaxAndDeclarationManager(_syntaxAndDeclarations.ExternalSyntaxTrees, options.ScriptClassName, options.SourceReferenceResolver, _syntaxAndDeclarations.MessageProvider, _syntaxAndDeclarations.IsSubmission, null), base.SemanticModelProvider);
        }

        public CSharpCompilation WithScriptCompilationInfo(CSharpScriptCompilationInfo? info)
        {
            if (info == ScriptCompilationInfo)
            {
                return this;
            }
            bool reuseReferenceManager = ScriptCompilationInfo?.PreviousScriptCompilation == info?.PreviousScriptCompilation;
            return new CSharpCompilation(base.AssemblyName, _options, base.ExternalReferences, info?.PreviousScriptCompilation, info?.ReturnTypeOpt, info?.GlobalsType, info != null, _referenceManager, reuseReferenceManager, _syntaxAndDeclarations, base.SemanticModelProvider);
        }

        public override Compilation WithSemanticModelProvider(SemanticModelProvider? semanticModelProvider)
        {
            if (base.SemanticModelProvider == semanticModelProvider)
            {
                return this;
            }
            return new CSharpCompilation(base.AssemblyName, _options, base.ExternalReferences, PreviousSubmission, base.SubmissionReturnType, base.HostObjectType, base.IsSubmission, _referenceManager, reuseReferenceManager: true, _syntaxAndDeclarations, semanticModelProvider);
        }

        public override Compilation WithEventQueue(AsyncQueue<CompilationEvent>? eventQueue)
        {
            return new CSharpCompilation(base.AssemblyName, _options, base.ExternalReferences, PreviousSubmission, base.SubmissionReturnType, base.HostObjectType, base.IsSubmission, _referenceManager, reuseReferenceManager: true, _syntaxAndDeclarations, base.SemanticModelProvider, eventQueue);
        }

        public override bool HasSubmissionResult()
        {
            SyntaxTree syntaxTree = _syntaxAndDeclarations.ExternalSyntaxTrees.SingleOrDefault();
            if (syntaxTree == null)
            {
                return false;
            }
            CompilationUnitSyntax compilationUnitRoot = syntaxTree.GetCompilationUnitRoot();
            if (compilationUnitRoot.HasErrors)
            {
                return false;
            }
            if (compilationUnitRoot.DescendantNodes((SyntaxNode n) => n is GlobalStatementSyntax || n is StatementSyntax || n is CompilationUnitSyntax).Any((SyntaxNode n) => n.IsKind(SyntaxKind.ReturnStatement)))
            {
                return true;
            }
            GlobalStatementSyntax globalStatementSyntax = (GlobalStatementSyntax)compilationUnitRoot.Members.LastOrDefault((MemberDeclarationSyntax m) => m.IsKind(SyntaxKind.GlobalStatement));
            if (globalStatementSyntax != null)
            {
                StatementSyntax statement = globalStatementSyntax.Statement;
                if (statement.IsKind(SyntaxKind.ExpressionStatement))
                {
                    ExpressionStatementSyntax expressionStatementSyntax = (ExpressionStatementSyntax)statement;
                    if (expressionStatementSyntax.SemicolonToken.IsMissing)
                    {
                        SemanticModel semanticModel = GetSemanticModel(syntaxTree);
                        ExpressionSyntax expression = expressionStatementSyntax.Expression;
                        ITypeSymbol? convertedType = semanticModel.GetTypeInfo(expression).ConvertedType;
                        if (convertedType == null)
                        {
                            return true;
                        }
                        return convertedType!.SpecialType != SpecialType.System_Void;
                    }
                }
            }
            return false;
        }

        public new bool ContainsSyntaxTree(SyntaxTree? syntaxTree)
        {
            if (syntaxTree != null)
            {
                return _syntaxAndDeclarations.GetLazyState().RootNamespaces.ContainsKey(syntaxTree);
            }
            return false;
        }

        public new CSharpCompilation AddSyntaxTrees(params SyntaxTree[] trees)
        {
            return AddSyntaxTrees((IEnumerable<SyntaxTree>)trees);
        }

        public new CSharpCompilation AddSyntaxTrees(IEnumerable<SyntaxTree> trees)
        {
            if (trees == null)
            {
                throw new ArgumentNullException("trees");
            }
            if (trees.IsEmpty())
            {
                return this;
            }
            PooledHashSet<SyntaxTree> instance = PooledHashSet<SyntaxTree>.GetInstance();
            SyntaxAndDeclarationManager syntaxAndDeclarations = _syntaxAndDeclarations;
            instance.AddAll(syntaxAndDeclarations.ExternalSyntaxTrees);
            bool flag = true;
            int num = 0;
            foreach (CSharpSyntaxTree item in trees.Cast<CSharpSyntaxTree>())
            {
                if (item == null)
                {
                    throw new ArgumentNullException(string.Format("{0}[{1}]", "trees", num));
                }
                if (!item.HasCompilationUnitRoot)
                {
                    throw new ArgumentException(CSharpResources.TreeMustHaveARootNodeWith, string.Format("{0}[{1}]", "trees", num));
                }
                if (instance.Contains(item))
                {
                    throw new ArgumentException(CSharpResources.SyntaxTreeAlreadyPresent, string.Format("{0}[{1}]", "trees", num));
                }
                if (base.IsSubmission && item.Options.Kind == SourceCodeKind.Regular)
                {
                    throw new ArgumentException(CSharpResources.SubmissionCanOnlyInclude, string.Format("{0}[{1}]", "trees", num));
                }
                instance.Add(item);
                flag &= !item.HasReferenceOrLoadDirectives;
                num++;
            }
            instance.Free();
            if (base.IsSubmission && num > 1)
            {
                throw new ArgumentException(CSharpResources.SubmissionCanHaveAtMostOne, "trees");
            }
            syntaxAndDeclarations = syntaxAndDeclarations.AddSyntaxTrees(trees);
            return Update(_referenceManager, flag, syntaxAndDeclarations);
        }

        public new CSharpCompilation RemoveSyntaxTrees(params SyntaxTree[] trees)
        {
            return RemoveSyntaxTrees((IEnumerable<SyntaxTree>)trees);
        }

        public new CSharpCompilation RemoveSyntaxTrees(IEnumerable<SyntaxTree> trees)
        {
            if (trees == null)
            {
                throw new ArgumentNullException("trees");
            }
            if (trees.IsEmpty())
            {
                return this;
            }
            PooledHashSet<SyntaxTree> instance = PooledHashSet<SyntaxTree>.GetInstance();
            PooledHashSet<SyntaxTree> instance2 = PooledHashSet<SyntaxTree>.GetInstance();
            SyntaxAndDeclarationManager syntaxAndDeclarations = _syntaxAndDeclarations;
            instance2.AddAll(syntaxAndDeclarations.ExternalSyntaxTrees);
            bool flag = true;
            int num = 0;
            foreach (CSharpSyntaxTree item in trees.Cast<CSharpSyntaxTree>())
            {
                if (!instance2.Contains(item))
                {
                    ImmutableDictionary<string, SyntaxTree> loadedSyntaxTreeMap = syntaxAndDeclarations.GetLazyState().LoadedSyntaxTreeMap;
                    if (SyntaxAndDeclarationManager.IsLoadedSyntaxTree(item, loadedSyntaxTreeMap))
                    {
                        throw new ArgumentException(CSharpResources.SyntaxTreeFromLoadNoRemoveReplace, string.Format("{0}[{1}]", "trees", num));
                    }
                    throw new ArgumentException(CSharpResources.SyntaxTreeNotFoundToRemove, string.Format("{0}[{1}]", "trees", num));
                }
                instance.Add(item);
                flag &= !item.HasReferenceOrLoadDirectives;
                num++;
            }
            instance2.Free();
            syntaxAndDeclarations = syntaxAndDeclarations.RemoveSyntaxTrees(instance);
            instance.Free();
            return Update(_referenceManager, flag, syntaxAndDeclarations);
        }

        public new CSharpCompilation RemoveAllSyntaxTrees()
        {
            SyntaxAndDeclarationManager syntaxAndDeclarations = _syntaxAndDeclarations;
            return Update(_referenceManager, !syntaxAndDeclarations.MayHaveReferenceDirectives(), syntaxAndDeclarations.WithExternalSyntaxTrees(ImmutableArray<SyntaxTree>.Empty));
        }

        public new CSharpCompilation ReplaceSyntaxTree(SyntaxTree oldTree, SyntaxTree? newTree)
        {
            oldTree = (CSharpSyntaxTree)oldTree;
            newTree = (CSharpSyntaxTree)newTree;
            if (oldTree == null)
            {
                throw new ArgumentNullException("oldTree");
            }
            if (newTree == null)
            {
                return RemoveSyntaxTrees(oldTree);
            }
            if (newTree == oldTree)
            {
                return this;
            }
            if (!newTree!.HasCompilationUnitRoot)
            {
                throw new ArgumentException(CSharpResources.TreeMustHaveARootNodeWith, "newTree");
            }
            SyntaxAndDeclarationManager syntaxAndDeclarations = _syntaxAndDeclarations;
            ImmutableArray<SyntaxTree> externalSyntaxTrees = syntaxAndDeclarations.ExternalSyntaxTrees;
            if (!externalSyntaxTrees.Contains(oldTree))
            {
                ImmutableDictionary<string, SyntaxTree> loadedSyntaxTreeMap = syntaxAndDeclarations.GetLazyState().LoadedSyntaxTreeMap;
                if (SyntaxAndDeclarationManager.IsLoadedSyntaxTree(oldTree, loadedSyntaxTreeMap))
                {
                    throw new ArgumentException(CSharpResources.SyntaxTreeFromLoadNoRemoveReplace, "oldTree");
                }
                throw new ArgumentException(CSharpResources.SyntaxTreeNotFoundToRemove, "oldTree");
            }
            if (externalSyntaxTrees.Contains(newTree))
            {
                throw new ArgumentException(CSharpResources.SyntaxTreeAlreadyPresent, "newTree");
            }
            bool reuseReferenceManager = !oldTree.HasReferenceOrLoadDirectives() && !newTree.HasReferenceOrLoadDirectives();
            syntaxAndDeclarations = syntaxAndDeclarations.ReplaceSyntaxTree(oldTree, newTree);
            return Update(_referenceManager, reuseReferenceManager, syntaxAndDeclarations);
        }

        public override int GetSyntaxTreeOrdinal(SyntaxTree tree)
        {
            return _syntaxAndDeclarations.GetLazyState().OrdinalMap[tree];
        }

        public override CommonReferenceManager CommonGetBoundReferenceManager()
        {
            return GetBoundReferenceManager();
        }

        public new ReferenceManager GetBoundReferenceManager()
        {
            if ((object)_lazyAssemblySymbol == null)
            {
                _referenceManager.CreateSourceAssemblyForCompilation(this);
            }
            return _referenceManager;
        }

        internal bool ReferenceManagerEquals(CSharpCompilation other)
        {
            return _referenceManager == other._referenceManager;
        }

        internal new Symbol? GetAssemblyOrModuleSymbol(MetadataReference reference)
        {
            if (reference == null)
            {
                throw new ArgumentNullException("reference");
            }
            if (reference.Properties.Kind == MetadataImageKind.Assembly)
            {
                return GetBoundReferenceManager().GetReferencedAssemblySymbol(reference);
            }
            int referencedModuleIndex = GetBoundReferenceManager().GetReferencedModuleIndex(reference);
            if (referencedModuleIndex >= 0)
            {
                return Assembly.Modules[referencedModuleIndex];
            }
            return null;
        }

        public MetadataReference? GetDirectiveReference(ReferenceDirectiveTriviaSyntax directive)
        {
            if (!ReferenceDirectiveMap.TryGetValue((directive.SyntaxTree.FilePath, directive.File.ValueText), out var value))
            {
                return null;
            }
            return value;
        }

        public new CSharpCompilation AddReferences(params MetadataReference[] references)
        {
            return (CSharpCompilation)base.AddReferences(references);
        }

        public new CSharpCompilation AddReferences(IEnumerable<MetadataReference> references)
        {
            return (CSharpCompilation)base.AddReferences(references);
        }

        public new CSharpCompilation RemoveReferences(params MetadataReference[] references)
        {
            return (CSharpCompilation)base.RemoveReferences(references);
        }

        public new CSharpCompilation RemoveReferences(IEnumerable<MetadataReference> references)
        {
            return (CSharpCompilation)base.RemoveReferences(references);
        }

        public new CSharpCompilation RemoveAllReferences()
        {
            return (CSharpCompilation)base.RemoveAllReferences();
        }

        public new CSharpCompilation ReplaceReference(MetadataReference oldReference, MetadataReference newReference)
        {
            return (CSharpCompilation)base.ReplaceReference(oldReference, newReference);
        }

        public override CompilationReference ToMetadataReference(ImmutableArray<string> aliases = default(ImmutableArray<string>), bool embedInteropTypes = false)
        {
            return new CSharpCompilationReference(this, aliases, embedInteropTypes);
        }

        private void GetAllUnaliasedModules(ArrayBuilder<Symbols.ModuleSymbol> modules)
        {
            modules.AddRange(Assembly.Modules);
            ReferenceManager boundReferenceManager = GetBoundReferenceManager();
            for (int i = 0; i < boundReferenceManager.ReferencedAssemblies.Length; i++)
            {
                if (boundReferenceManager.DeclarationsAccessibleWithoutAlias(i))
                {
                    modules.AddRange(boundReferenceManager.ReferencedAssemblies[i].Modules);
                }
            }
        }

        internal void GetUnaliasedReferencedAssemblies(ArrayBuilder<Symbols.AssemblySymbol> assemblies)
        {
            ReferenceManager boundReferenceManager = GetBoundReferenceManager();
            for (int i = 0; i < boundReferenceManager.ReferencedAssemblies.Length; i++)
            {
                if (boundReferenceManager.DeclarationsAccessibleWithoutAlias(i))
                {
                    assemblies.Add(boundReferenceManager.ReferencedAssemblies[i]);
                }
            }
        }

        public new MetadataReference? GetMetadataReference(IAssemblySymbol assemblySymbol)
        {
            return base.GetMetadataReference(assemblySymbol);
        }

        protected override MetadataReference? CommonGetMetadataReference(IAssemblySymbol assemblySymbol)
        {
            if (assemblySymbol is Symbols.PublicModel.AssemblySymbol assemblySymbol2)
            {
                Symbols.AssemblySymbol underlyingAssemblySymbol = assemblySymbol2.UnderlyingAssemblySymbol;
                return GetMetadataReference(underlyingAssemblySymbol);
            }
            return null;
        }

        internal MetadataReference? GetMetadataReference(Symbols.AssemblySymbol? assemblySymbol)
        {
            return GetBoundReferenceManager().GetMetadataReference(assemblySymbol);
        }

        internal new Symbols.NamespaceSymbol? GetCompilationNamespace(INamespaceSymbol namespaceSymbol)
        {
            if (namespaceSymbol is Symbols.PublicModel.NamespaceSymbol namespaceSymbol2 && namespaceSymbol.NamespaceKind == NamespaceKind.Compilation && namespaceSymbol.ContainingCompilation == this)
            {
                return namespaceSymbol2.UnderlyingNamespaceSymbol;
            }
            INamespaceSymbol containingNamespace = namespaceSymbol.ContainingNamespace;
            if (containingNamespace == null)
            {
                return GlobalNamespace;
            }
            return GetCompilationNamespace(containingNamespace)?.GetNestedNamespace(namespaceSymbol.Name);
        }

        internal Symbols.NamespaceSymbol? GetCompilationNamespace(Symbols.NamespaceSymbol namespaceSymbol)
        {
            if (namespaceSymbol.NamespaceKind == NamespaceKind.Compilation && namespaceSymbol.ContainingCompilation == this)
            {
                return namespaceSymbol;
            }
            Symbols.NamespaceSymbol containingNamespace = namespaceSymbol.ContainingNamespace;
            if (containingNamespace == null)
            {
                return GlobalNamespace;
            }
            return GetCompilationNamespace(containingNamespace)?.GetNestedNamespace(namespaceSymbol.Name);
        }

        internal bool GetExternAliasTarget(string aliasName, out Symbols.NamespaceSymbol @namespace)
        {
            if (_externAliasTargets == null)
            {
                Interlocked.CompareExchange(ref _externAliasTargets, new ConcurrentDictionary<string, Symbols.NamespaceSymbol>(), null);
            }
            else if (_externAliasTargets!.TryGetValue(aliasName, out Symbols.NamespaceSymbol value))
            {
                @namespace = value;
                return !(@namespace is MissingNamespaceSymbol);
            }
            ArrayBuilder<Symbols.NamespaceSymbol> arrayBuilder = null;
            ReferenceManager boundReferenceManager = GetBoundReferenceManager();
            for (int i = 0; i < boundReferenceManager.ReferencedAssemblies.Length; i++)
            {
                if (boundReferenceManager.AliasesOfReferencedAssemblies[i].Contains(aliasName))
                {
                    arrayBuilder = arrayBuilder ?? ArrayBuilder<Symbols.NamespaceSymbol>.GetInstance();
                    arrayBuilder.Add(boundReferenceManager.ReferencedAssemblies[i].GlobalNamespace);
                }
            }
            bool flag = arrayBuilder != null;
            @namespace = (flag ? MergedNamespaceSymbol.Create(new NamespaceExtent(this), null, arrayBuilder.ToImmutableAndFree()) : new MissingNamespaceSymbol(new MissingModuleSymbol(new MissingAssemblySymbol(new AssemblyIdentity(Guid.NewGuid().ToString())), -1)));
            @namespace = _externAliasTargets!.GetOrAdd(aliasName, @namespace);
            return flag;
        }

        private ImplicitNamedTypeSymbol? BindScriptClass()
        {
            return (ImplicitNamedTypeSymbol)CommonBindScriptClass().GetSymbol();
        }

        internal bool IsSubmissionSyntaxTree(SyntaxTree tree)
        {
            if (base.IsSubmission)
            {
                return tree == _syntaxAndDeclarations.ExternalSyntaxTrees.SingleOrDefault();
            }
            return false;
        }

        private ImmutableArray<NamespaceOrTypeAndUsingDirective> BindGlobalImports()
        {
            UsingsFromOptionsAndDiagnostics usingsFromOptions = UsingsFromOptions;
            CSharpCompilation previousSubmission = PreviousSubmission;
            ImmutableArray<NamespaceOrTypeAndUsingDirective> result = ((previousSubmission != null) ? Imports.ExpandPreviousSubmissionImports(previousSubmission.GlobalImports, this) : ImmutableArray<NamespaceOrTypeAndUsingDirective>.Empty);
            if (usingsFromOptions.UsingNamespacesOrTypes.IsEmpty)
            {
                return result;
            }
            if (result.IsEmpty)
            {
                return usingsFromOptions.UsingNamespacesOrTypes;
            }
            ArrayBuilder<NamespaceOrTypeAndUsingDirective> instance = ArrayBuilder<NamespaceOrTypeAndUsingDirective>.GetInstance();
            PooledHashSet<Symbols.NamespaceOrTypeSymbol> instance2 = PooledHashSet<Symbols.NamespaceOrTypeSymbol>.GetInstance();
            instance.AddRange(usingsFromOptions.UsingNamespacesOrTypes);
            instance2.AddAll(usingsFromOptions.UsingNamespacesOrTypes.Select((NamespaceOrTypeAndUsingDirective unt) => unt.NamespaceOrType));
            ImmutableArray<NamespaceOrTypeAndUsingDirective>.Enumerator enumerator = result.GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamespaceOrTypeAndUsingDirective current = enumerator.Current;
                if (instance2.Add(current.NamespaceOrType))
                {
                    instance.Add(current);
                }
            }
            instance2.Free();
            return instance.ToImmutableAndFree();
        }

        private UsingsFromOptionsAndDiagnostics BindUsingsFromOptions()
        {
            return UsingsFromOptionsAndDiagnostics.FromOptions(this);
        }

        internal Imports GetSubmissionImports()
        {
            SyntaxTree syntaxTree = _syntaxAndDeclarations.ExternalSyntaxTrees.SingleOrDefault();
            if (syntaxTree == null)
            {
                return Imports.Empty;
            }
            return ((SourceNamespaceSymbol)SourceModule.GlobalNamespace).GetImports((CSharpSyntaxNode)syntaxTree.GetRoot(), null);
        }

        internal Imports GetPreviousSubmissionImports()
        {
            return _previousSubmissionImports.Value;
        }

        private Imports ExpandPreviousSubmissionImports()
        {
            CSharpCompilation previousSubmission = PreviousSubmission;
            if (previousSubmission == null)
            {
                return Imports.Empty;
            }
            return Imports.ExpandPreviousSubmissionImports(previousSubmission.GetPreviousSubmissionImports(), this).Concat(Imports.ExpandPreviousSubmissionImports(previousSubmission.GetSubmissionImports(), this));
        }

        internal new Symbols.NamedTypeSymbol GetSpecialType(SpecialType specialType)
        {
            if (specialType <= SpecialType.None || specialType > SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute)
            {
                throw new ArgumentOutOfRangeException("specialType", $"Unexpected SpecialType: '{(int)specialType}'.");
            }
            if (IsTypeMissing(specialType))
            {
                MetadataTypeName fullName = MetadataTypeName.FromFullName(specialType.GetMetadataName(), useCLSCompliantNameArityEncoding: true);
                return new MissingMetadataTypeSymbol.TopLevel(Assembly.CorLibrary.Modules[0], ref fullName, specialType);
            }
            return Assembly.GetSpecialType(specialType);
        }

        internal Symbol GetSpecialTypeMember(SpecialMember specialMember)
        {
            return Assembly.GetSpecialTypeMember(specialMember);
        }

        public override ISymbolInternal CommonGetSpecialTypeMember(SpecialMember specialMember)
        {
            return GetSpecialTypeMember(specialMember);
        }

        internal Symbols.TypeSymbol GetTypeByReflectionType(Type type, BindingDiagnosticBag diagnostics)
        {
            Symbols.TypeSymbol typeSymbol = Assembly.GetTypeByReflectionType(type, includeReferences: true);
            if ((object)typeSymbol == null)
            {
                ExtendedErrorTypeSymbol extendedErrorTypeSymbol = new ExtendedErrorTypeSymbol(this, type.Name, 0, CreateReflectionTypeNotFoundError(type));
                diagnostics.Add(extendedErrorTypeSymbol.ErrorInfo, NoLocation.Singleton);
                typeSymbol = extendedErrorTypeSymbol;
            }
            return typeSymbol;
        }

        private static CSDiagnosticInfo CreateReflectionTypeNotFoundError(Type type)
        {
            return new CSDiagnosticInfo(ErrorCode.ERR_GlobalSingleTypeNameNotFound, new object[1] { type.AssemblyQualifiedName ?? "" }, ImmutableArray<Symbol>.Empty, ImmutableArray<Location>.Empty);
        }

        internal Symbols.TypeSymbol? GetHostObjectTypeSymbol()
        {
            if (base.HostObjectType != null && (object)_lazyHostObjectTypeSymbol == null)
            {
                Symbols.TypeSymbol typeSymbol = Assembly.GetTypeByReflectionType(base.HostObjectType, includeReferences: true);
                if ((object)typeSymbol == null)
                {
                    MetadataTypeName fullName = MetadataTypeName.FromNamespaceAndTypeName(base.HostObjectType!.Namespace ?? string.Empty, base.HostObjectType!.Name, useCLSCompliantNameArityEncoding: true);
                    typeSymbol = new MissingMetadataTypeSymbol.TopLevel(new MissingAssemblySymbol(AssemblyIdentity.FromAssemblyDefinition(base.HostObjectType.GetTypeInfo().Assembly)).Modules[0], ref fullName, SpecialType.None, CreateReflectionTypeNotFoundError(base.HostObjectType));
                }
                Interlocked.CompareExchange(ref _lazyHostObjectTypeSymbol, typeSymbol, null);
            }
            return _lazyHostObjectTypeSymbol;
        }

        internal SynthesizedInteractiveInitializerMethod? GetSubmissionInitializer()
        {
            if (!base.IsSubmission || (object)ScriptClass == null)
            {
                return null;
            }
            return ScriptClass!.GetScriptInitializer();
        }

        internal new Symbols.NamedTypeSymbol? GetTypeByMetadataName(string fullyQualifiedMetadataName)
        {
            return Assembly.GetTypeByMetadataName(fullyQualifiedMetadataName, includeReferences: true, isWellKnownType: false, out (Symbols.AssemblySymbol, Symbols.AssemblySymbol) conflicts);
        }

        internal new Symbols.MethodSymbol? GetEntryPoint(CancellationToken cancellationToken)
        {
            return GetEntryPointAndDiagnostics(cancellationToken).MethodSymbol;
        }

        internal EntryPoint GetEntryPointAndDiagnostics(CancellationToken cancellationToken)
        {
            if (_lazyEntryPoint == null)
            {
                SynthesizedSimpleProgramEntryPointSymbol simpleProgramEntryPoint = SimpleProgramNamedTypeSymbol.GetSimpleProgramEntryPoint(this);
                EntryPoint entryPoint;
                if (!Options.OutputKind.IsApplication() && (object)ScriptClass == null)
                {
                    if ((object)simpleProgramEntryPoint != null)
                    {
                        BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                        instance.Add(ErrorCode.ERR_SimpleProgramNotAnExecutable, simpleProgramEntryPoint.ReturnTypeSyntax.Location);
                        entryPoint = new EntryPoint(null, instance.ToReadOnlyAndFree());
                    }
                    else
                    {
                        entryPoint = EntryPoint.None;
                    }
                }
                else
                {
                    entryPoint = null;
                    if (Options.MainTypeName != null && !Options.MainTypeName.IsValidClrTypeName())
                    {
                        entryPoint = EntryPoint.None;
                    }
                    if (entryPoint == null)
                    {
                        entryPoint = new EntryPoint(FindEntryPoint(simpleProgramEntryPoint, cancellationToken, out var sealedDiagnostics), sealedDiagnostics);
                    }
                    if (Options.MainTypeName != null && (object)simpleProgramEntryPoint != null)
                    {
                        DiagnosticBag instance2 = DiagnosticBag.GetInstance();
                        instance2.Add(ErrorCode.ERR_SimpleProgramDisallowsMainType, NoLocation.Singleton);
                        entryPoint = new EntryPoint(entryPoint.MethodSymbol, new ImmutableBindingDiagnostic<Symbols.AssemblySymbol>(entryPoint.Diagnostics.Diagnostics.Concat(instance2.ToReadOnlyAndFree()), entryPoint.Diagnostics.Dependencies));
                    }
                }
                Interlocked.CompareExchange(ref _lazyEntryPoint, entryPoint, null);
            }
            return _lazyEntryPoint;
        }

        private Symbols.MethodSymbol? FindEntryPoint(Symbols.MethodSymbol? simpleProgramEntryPointSymbol, CancellationToken cancellationToken, out ImmutableBindingDiagnostic<Symbols.AssemblySymbol> sealedDiagnostics)
        {
            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
            ArrayBuilder<Symbols.MethodSymbol> instance2 = ArrayBuilder<Symbols.MethodSymbol>.GetInstance();
            try
            {
                string mainTypeName = Options.MainTypeName;
                Symbols.NamespaceSymbol globalNamespace = SourceModule.GlobalNamespace;
                Symbols.NamedTypeSymbol scriptClass = ScriptClass;
                Symbols.NamedTypeSymbol namedTypeSymbol;
                ArrayBuilder<Symbols.MethodSymbol>.Enumerator enumerator;
                if (mainTypeName != null)
                {
                    if ((object)scriptClass != null)
                    {
                        instance.Add(ErrorCode.WRN_MainIgnored, NoLocation.Singleton, mainTypeName);
                        return scriptClass.GetScriptEntryPoint();
                    }
                    Symbols.NamespaceOrTypeSymbol namespaceOrTypeSymbol = globalNamespace.GetNamespaceOrTypeByQualifiedName(mainTypeName.Split(new char[1] { '.' })).OfMinimalArity();
                    if ((object)namespaceOrTypeSymbol == null)
                    {
                        instance.Add(ErrorCode.ERR_MainClassNotFound, NoLocation.Singleton, mainTypeName);
                        return null;
                    }
                    namedTypeSymbol = namespaceOrTypeSymbol as Symbols.NamedTypeSymbol;
                    if ((object)namedTypeSymbol == null || namedTypeSymbol.IsGenericType || (namedTypeSymbol.TypeKind != TypeKind.Class && namedTypeSymbol.TypeKind != TypeKind.Struct && !namedTypeSymbol.IsInterface))
                    {
                        instance.Add(ErrorCode.ERR_MainClassNotClass, namespaceOrTypeSymbol.Locations.First(), namespaceOrTypeSymbol);
                        return null;
                    }
                    AddEntryPointCandidates(instance2, namedTypeSymbol.GetMembersUnordered());
                }
                else
                {
                    namedTypeSymbol = null;
                    AddEntryPointCandidates(instance2, GetSymbolsWithNameCore("Main", SymbolFilter.Member, cancellationToken));
                    if ((object)scriptClass != null || (object)simpleProgramEntryPointSymbol != null)
                    {
                        enumerator = instance2.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            Symbols.MethodSymbol current = enumerator.Current;
                            instance.Add(ErrorCode.WRN_MainIgnored, current.Locations.First(), current);
                        }
                        if ((object)scriptClass != null)
                        {
                            return scriptClass.GetScriptEntryPoint();
                        }
                        instance2.Clear();
                        instance2.Add(simpleProgramEntryPointSymbol);
                    }
                }
                ArrayBuilder<(bool, Symbols.MethodSymbol, BindingDiagnosticBag)> instance3 = ArrayBuilder<(bool, Symbols.MethodSymbol, BindingDiagnosticBag)>.GetInstance();
                BindingDiagnosticBag noMainFoundDiagnostics = BindingDiagnosticBag.GetInstance(instance);
                ArrayBuilder<Symbols.MethodSymbol> instance4 = ArrayBuilder<Symbols.MethodSymbol>.GetInstance();
                enumerator = instance2.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbols.MethodSymbol current2 = enumerator.Current;
                    BindingDiagnosticBag instance5 = BindingDiagnosticBag.GetInstance(instance);
                    (bool IsCandidate, bool IsTaskLike) tuple = HasEntryPointSignature(current2, instance5);
                    var (flag, _) = tuple;
                    if (tuple.IsTaskLike)
                    {
                        instance3.Add((flag, current2, instance5));
                        continue;
                    }
                    if (checkValid(current2, flag, instance5))
                    {
                        if (current2.IsAsync)
                        {
                            instance.Add(ErrorCode.ERR_NonTaskMainCantBeAsync, current2.Locations.First(), current2);
                        }
                        else
                        {
                            instance.AddRange(instance5);
                            instance4.Add(current2);
                        }
                    }
                    instance5.Free();
                }
                ArrayBuilder<(bool, Symbols.MethodSymbol, BindingDiagnosticBag)>.Enumerator enumerator2;
                if (instance4.Count == 0)
                {
                    enumerator2 = instance3.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        var (isCandidate2, methodSymbol, bindingDiagnosticBag) = enumerator2.Current;
                        if (checkValid(methodSymbol, isCandidate2, bindingDiagnosticBag) && Binder.CheckFeatureAvailability(methodSymbol.ExtractReturnTypeSyntax(), MessageID.IDS_FeatureAsyncMain, instance))
                        {
                            instance.AddRange(bindingDiagnosticBag);
                            instance4.Add(methodSymbol);
                        }
                    }
                }
                else if (LanguageVersion >= MessageID.IDS_FeatureAsyncMain.RequiredVersion() && instance3.Count > 0)
                {
                    ImmutableArray<Symbol> immutableArray = instance3.SelectAsArray<(bool, Symbols.MethodSymbol, BindingDiagnosticBag), Symbol>(((bool IsValid, Symbols.MethodSymbol Candidate, BindingDiagnosticBag SpecificDiagnostics) s) => s.Candidate);
                    ImmutableArray<Location> additionalLocations = immutableArray.SelectAsArray((Symbol s) => s.Locations[0]);
                    ImmutableArray<Symbol>.Enumerator enumerator3 = immutableArray.GetEnumerator();
                    while (enumerator3.MoveNext())
                    {
                        Symbol current3 = enumerator3.Current;
                        CSDiagnosticInfo info = new CSDiagnosticInfo(ErrorCode.WRN_SyncAndAsyncEntryPoints, new object[2]
                        {
                            current3,
                            instance4[0]
                        }, immutableArray, additionalLocations);
                        instance.Add(new CSDiagnostic(info, current3.Locations[0]));
                    }
                }
                enumerator2 = instance3.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    enumerator2.Current.Item3.Free();
                }
                if (instance4.Count == 0)
                {
                    instance.AddRange(noMainFoundDiagnostics);
                }
                else if ((object)namedTypeSymbol == null)
                {
                    foreach (Diagnostic item in noMainFoundDiagnostics.DiagnosticBag!.AsEnumerable())
                    {
                        if (item.Code == 28 || item.Code == 402)
                        {
                            instance.Add(item);
                        }
                    }
                    instance.AddDependencies(noMainFoundDiagnostics);
                }
                Symbols.MethodSymbol result = null;
                if (instance4.Count == 0)
                {
                    if ((object)namedTypeSymbol == null)
                    {
                        instance.Add(ErrorCode.ERR_NoEntryPoint, NoLocation.Singleton);
                    }
                    else
                    {
                        instance.Add(ErrorCode.ERR_NoMainInClass, namedTypeSymbol.Locations.First(), namedTypeSymbol);
                    }
                }
                else
                {
                    enumerator = instance4.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Symbols.MethodSymbol current5 = enumerator.Current;
                        if (current5.GetUnmanagedCallersOnlyAttributeData(forceComplete: true) != null)
                        {
                            instance.Add(ErrorCode.ERR_EntryPointCannotBeUnmanagedCallersOnly, current5.Locations.First());
                        }
                    }
                    if (instance4.Count > 1)
                    {
                        instance4.Sort(LexicalOrderSymbolComparer.Instance);
                        CSDiagnosticInfo info2 = new CSDiagnosticInfo(ErrorCode.ERR_MultipleEntryPoints, new object[0], instance4.OfType<Symbol>().AsImmutable(), instance4.Select((Symbols.MethodSymbol m) => m.Locations.First()).OfType<Location>().AsImmutable());
                        instance.Add(new CSDiagnostic(info2, instance4.First().Locations.First()));
                    }
                    else
                    {
                        result = instance4[0];
                    }
                }
                instance3.Free();
                instance4.Free();
                noMainFoundDiagnostics.Free();
                return result;
                bool checkValid(Symbols.MethodSymbol candidate, bool isCandidate, BindingDiagnosticBag specificDiagnostics)
                {
                    if (!isCandidate)
                    {
                        noMainFoundDiagnostics.Add(ErrorCode.WRN_InvalidMainSig, candidate.Locations.First(), candidate);
                        noMainFoundDiagnostics.AddRange(specificDiagnostics);
                        return false;
                    }
                    if (candidate.IsGenericMethod || candidate.ContainingType.IsGenericType)
                    {
                        noMainFoundDiagnostics.Add(ErrorCode.WRN_MainCantBeGeneric, candidate.Locations.First(), candidate);
                        return false;
                    }
                    return true;
                }
            }
            finally
            {
                instance2.Free();
                sealedDiagnostics = instance.ToReadOnlyAndFree();
            }
        }

        private static void AddEntryPointCandidates(ArrayBuilder<Symbols.MethodSymbol> entryPointCandidates, IEnumerable<Symbol> members)
        {
            foreach (Symbol member in members)
            {
                if (member is Symbols.MethodSymbol methodSymbol && methodSymbol.IsEntryPointCandidate)
                {
                    entryPointCandidates.Add(methodSymbol);
                }
            }
        }

        internal bool ReturnsAwaitableToVoidOrInt(Symbols.MethodSymbol method, BindingDiagnosticBag diagnostics)
        {
            if (method.ReturnType.IsVoidType() || method.ReturnType.SpecialType == SpecialType.System_Int32)
            {
                return false;
            }
            if (!(method.ReturnType is Symbols.NamedTypeSymbol namedTypeSymbol))
            {
                return false;
            }
            if (!Symbols.TypeSymbol.Equals(namedTypeSymbol.ConstructedFrom, GetWellKnownType(WellKnownType.System_Threading_Tasks_Task), TypeCompareKind.ConsiderEverything) && !Symbols.TypeSymbol.Equals(namedTypeSymbol.ConstructedFrom, GetWellKnownType(WellKnownType.System_Threading_Tasks_Task_T), TypeCompareKind.ConsiderEverything))
            {
                return false;
            }
            CSharpSyntaxNode cSharpSyntaxNode = method.ExtractReturnTypeSyntax();
            BoundLiteral expression = new BoundLiteral(cSharpSyntaxNode, ConstantValue.Null, namedTypeSymbol);
            if (GetBinder(cSharpSyntaxNode).GetAwaitableExpressionInfo(expression, out var getAwaiterGetResultCall, cSharpSyntaxNode, diagnostics))
            {
                if (!getAwaiterGetResultCall.Type.IsVoidType())
                {
                    return getAwaiterGetResultCall.Type!.SpecialType == SpecialType.System_Int32;
                }
                return true;
            }
            return false;
        }

        internal (bool IsCandidate, bool IsTaskLike) HasEntryPointSignature(Symbols.MethodSymbol method, BindingDiagnosticBag bag)
        {
            if (method.IsVararg)
            {
                return (false, false);
            }
            Symbols.TypeSymbol returnType = method.ReturnType;
            bool flag = false;
            if (returnType.SpecialType != SpecialType.System_Int32 && !returnType.IsVoidType())
            {
                flag = ReturnsAwaitableToVoidOrInt(method, bag);
                if (!flag)
                {
                    return (false, false);
                }
            }
            if (method.RefKind != 0)
            {
                return (false, flag);
            }
            if (method.Parameters.Length == 0)
            {
                return (true, flag);
            }
            if (method.Parameters.Length > 1)
            {
                return (false, flag);
            }
            if (!method.ParameterRefKinds.IsDefault)
            {
                return (false, flag);
            }
            TypeWithAnnotations typeWithAnnotations = method.Parameters[0].TypeWithAnnotations;
            if (typeWithAnnotations.TypeKind != TypeKind.Array)
            {
                return (false, flag);
            }
            Symbols.ArrayTypeSymbol arrayTypeSymbol = (Symbols.ArrayTypeSymbol)typeWithAnnotations.Type;
            return (arrayTypeSymbol.IsSZArray && arrayTypeSymbol.ElementType.SpecialType == SpecialType.System_String, flag);
        }

        public override bool IsUnreferencedAssemblyIdentityDiagnosticCode(int code)
        {
            return code == 12;
        }

        internal bool MightContainNoPiaLocalTypes()
        {
            return SourceAssembly.MightContainNoPiaLocalTypes();
        }

        public Conversion ClassifyConversion(ITypeSymbol source, ITypeSymbol destination)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (destination == null)
            {
                throw new ArgumentNullException("destination");
            }
            Symbols.TypeSymbol source2 = source.EnsureCSharpSymbolOrNull("source");
            Symbols.TypeSymbol destination2 = destination.EnsureCSharpSymbolOrNull("destination");
            CompoundUseSiteInfo<Symbols.AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<Symbols.AssemblySymbol>.Discarded;
            return Conversions.ClassifyConversionFromType(source2, destination2, ref useSiteInfo);
        }

        public override CommonConversion ClassifyCommonConversion(ITypeSymbol source, ITypeSymbol destination)
        {
            return ClassifyConversion(source, destination).ToCommonConversion();
        }

        public override IConvertibleConversion ClassifyConvertibleConversion(IOperation source, ITypeSymbol? destination, out ConstantValue? constantValue)
        {
            constantValue = null;
            if (destination == null)
            {
                return Conversion.NoConversion;
            }
            ITypeSymbol type = source.Type;
            ConstantValue constantValue2 = source.GetConstantValue();
            if (type == null)
            {
                if ((object)constantValue2 != null && constantValue2.IsNull && destination!.IsReferenceType)
                {
                    constantValue = constantValue2;
                    return Conversion.NullLiteral;
                }
                return Conversion.NoConversion;
            }
            Conversion conversion = ClassifyConversion(type, destination);
            if (conversion.IsReference && (object)constantValue2 != null && constantValue2.IsNull)
            {
                constantValue = constantValue2;
            }
            return conversion;
        }

        internal Symbols.ArrayTypeSymbol CreateArrayTypeSymbol(Symbols.TypeSymbol elementType, int rank = 1, NullableAnnotation elementNullableAnnotation = NullableAnnotation.Oblivious)
        {
            if ((object)elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }
            if (rank < 1)
            {
                throw new ArgumentException("rank");
            }
            return Symbols.ArrayTypeSymbol.CreateCSharpArray(Assembly, TypeWithAnnotations.Create(elementType, elementNullableAnnotation), rank);
        }

        internal Symbols.PointerTypeSymbol CreatePointerTypeSymbol(Symbols.TypeSymbol elementType, NullableAnnotation elementNullableAnnotation = NullableAnnotation.Oblivious)
        {
            if ((object)elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }
            return new Symbols.PointerTypeSymbol(TypeWithAnnotations.Create(elementType, elementNullableAnnotation));
        }

        protected override bool IsSymbolAccessibleWithinCore(ISymbol symbol, ISymbol within, ITypeSymbol? throughType)
        {
            Symbol symbol2 = symbol.EnsureCSharpSymbolOrNull("symbol");
            Symbol symbol3 = within.EnsureCSharpSymbolOrNull("within");
            Symbols.TypeSymbol throughTypeOpt = throughType.EnsureCSharpSymbolOrNull("throughType");
            CompoundUseSiteInfo<Symbols.AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<Symbols.AssemblySymbol>.Discarded;
            if (symbol3.Kind != SymbolKind.Assembly)
            {
                return AccessCheck.IsSymbolAccessible(symbol2, (Symbols.NamedTypeSymbol)symbol3, ref useSiteInfo, throughTypeOpt);
            }
            return AccessCheck.IsSymbolAccessible(symbol2, (Symbols.AssemblySymbol)symbol3, ref useSiteInfo);
        }

        [Obsolete("Compilation.IsSymbolAccessibleWithin is not designed for use within the compilers", true)]
        internal new bool IsSymbolAccessibleWithin(ISymbol symbol, ISymbol within, ITypeSymbol? throughType = null)
        {
            throw new NotImplementedException();
        }

        internal void AddModuleInitializerMethod(Symbols.MethodSymbol method)
        {
            LazyInitializer.EnsureInitialized(ref _moduleInitializerMethods)!.Add(method);
        }

        public new SemanticModel GetSemanticModel(SyntaxTree syntaxTree, bool ignoreAccessibility)
        {
            if (syntaxTree == null)
            {
                throw new ArgumentNullException("syntaxTree");
            }
            if (!_syntaxAndDeclarations.GetLazyState().RootNamespaces.ContainsKey(syntaxTree))
            {
                throw new ArgumentException(CSharpResources.SyntaxTreeNotFound, "syntaxTree");
            }
            SemanticModel semanticModel = null;
            if (base.SemanticModelProvider != null)
            {
                semanticModel = base.SemanticModelProvider!.GetSemanticModel(syntaxTree, this, ignoreAccessibility);
            }
            return semanticModel ?? CreateSemanticModel(syntaxTree, ignoreAccessibility);
        }

        public override SemanticModel CreateSemanticModel(SyntaxTree syntaxTree, bool ignoreAccessibility)
        {
            return new SyntaxTreeSemanticModel(this, syntaxTree, ignoreAccessibility);
        }

        internal BinderFactory GetBinderFactory(SyntaxTree syntaxTree, bool ignoreAccessibility = false)
        {
            if (ignoreAccessibility && (object)SimpleProgramNamedTypeSymbol.GetSimpleProgramEntryPoint(this) != null)
            {
                return GetBinderFactory(syntaxTree, ignoreAccessibility: true, ref _ignoreAccessibilityBinderFactories);
            }
            return GetBinderFactory(syntaxTree, ignoreAccessibility: false, ref _binderFactories);
        }

        private BinderFactory GetBinderFactory(SyntaxTree syntaxTree, bool ignoreAccessibility, ref WeakReference<BinderFactory>[]? cachedBinderFactories)
        {
            int syntaxTreeOrdinal = GetSyntaxTreeOrdinal(syntaxTree);
            WeakReference<BinderFactory>[] array = cachedBinderFactories;
            if (array == null)
            {
                array = new WeakReference<BinderFactory>[SyntaxTrees.Length];
                array = Interlocked.CompareExchange(ref cachedBinderFactories, array, null) ?? array;
            }
            WeakReference<BinderFactory> weakReference = array[syntaxTreeOrdinal];
            if (weakReference != null && weakReference.TryGetTarget(out var target))
            {
                return target;
            }
            return AddNewFactory(syntaxTree, ignoreAccessibility, ref array[syntaxTreeOrdinal]);
        }

        private BinderFactory AddNewFactory(SyntaxTree syntaxTree, bool ignoreAccessibility, [System.Diagnostics.CodeAnalysis.NotNull] ref WeakReference<BinderFactory>? slot)
        {
            var newFactory = new BinderFactory(this, syntaxTree, ignoreAccessibility);
            var newWeakReference = new WeakReference<BinderFactory>(newFactory);

            while (true)
            {
                BinderFactory? previousFactory;
                WeakReference<BinderFactory>? previousWeakReference = slot;
                if (previousWeakReference != null && previousWeakReference.TryGetTarget(out previousFactory))
                {
                    Debug.Assert(slot is object);
                    return previousFactory;
                }

                if (Interlocked.CompareExchange(ref slot!, newWeakReference, previousWeakReference) == previousWeakReference)
                {
                    return newFactory;
                }
            }
        }

        internal Binder GetBinder(CSharpSyntaxNode syntax)
        {
            return GetBinderFactory(syntax.SyntaxTree).GetBinder(syntax);
        }

        private Symbols.AliasSymbol CreateGlobalNamespaceAlias()
        {
            return Symbols.AliasSymbol.CreateGlobalNamespaceAlias(GlobalNamespace);
        }

        private void CompleteTree(SyntaxTree tree)
        {
            if (_lazyCompilationUnitCompletedTrees == null)
            {
                Interlocked.CompareExchange(ref _lazyCompilationUnitCompletedTrees, new HashSet<SyntaxTree>(), null);
            }
            lock (_lazyCompilationUnitCompletedTrees)
            {
                if (_lazyCompilationUnitCompletedTrees!.Add(tree))
                {
                    base.EventQueue?.TryEnqueue(new CompilationUnitCompletedEvent(this, tree));
                    if (_lazyCompilationUnitCompletedTrees!.Count == SyntaxTrees.Length)
                    {
                        CompleteCompilationEventQueue_NoLock();
                    }
                }
            }
        }

        public override void ReportUnusedImports(DiagnosticBag diagnostics, CancellationToken cancellationToken)
        {
            ReportUnusedImports(null, new BindingDiagnosticBag(diagnostics), cancellationToken);
        }

        private void ReportUnusedImports(SyntaxTree? filterTree, BindingDiagnosticBag diagnostics, CancellationToken cancellationToken)
        {
            if (_lazyImportInfos != null && (filterTree == null || Compilation.ReportUnusedImportsInTree(filterTree)))
            {
                PooledHashSet<Symbols.NamespaceSymbol> pooledHashSet = null;
                if (diagnostics.DependenciesBag != null)
                {
                    pooledHashSet = PooledHashSet<Symbols.NamespaceSymbol>.GetInstance();
                }
                foreach (KeyValuePair<ImportInfo, ImmutableArray<Symbols.AssemblySymbol>> item in _lazyImportInfos!)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    ImportInfo key = item.Key;
                    SyntaxTree tree = key.Tree;
                    if ((filterTree != null && filterTree != tree) || !Compilation.ReportUnusedImportsInTree(tree))
                    {
                        continue;
                    }
                    TextSpan span = key.Span;
                    if (!IsImportDirectiveUsed(tree, span.Start))
                    {
                        ErrorCode code = ((key.Kind == SyntaxKind.ExternAliasDirective) ? ErrorCode.HDN_UnusedExternAlias : ErrorCode.HDN_UnusedUsingDirective);
                        diagnostics.Add(code, tree.GetLocation(span));
                    }
                    else
                    {
                        if (diagnostics.DependenciesBag == null)
                        {
                            continue;
                        }
                        ImmutableArray<Symbols.AssemblySymbol> value = item.Value;
                        if (!value.IsDefaultOrEmpty)
                        {
                            diagnostics.AddDependencies(value);
                        }
                        else if (key.Kind == SyntaxKind.ExternAliasDirective)
                        {
                            ExternAliasDirectiveSyntax externAliasDirectiveSyntax = key.Tree.GetRoot(cancellationToken).FindToken(key.Span.Start).Parent!.FirstAncestorOrSelf<ExternAliasDirectiveSyntax>();
                            if (externAliasDirectiveSyntax != null && GetExternAliasTarget(externAliasDirectiveSyntax.Identifier.ValueText, out var @namespace))
                            {
                                pooledHashSet.Add(@namespace);
                            }
                        }
                    }
                }
                if (pooledHashSet != null)
                {
                    BindingDiagnosticBag bindingDiagnosticBag = new BindingDiagnosticBag(null, PooledHashSet<Symbols.AssemblySymbol>.GetInstance());
                    foreach (Symbols.NamespaceSymbol item2 in pooledHashSet)
                    {
                        bindingDiagnosticBag.Clear();
                        bindingDiagnosticBag.AddAssembliesUsedByNamespaceReference(item2);
                        ConcurrentSet<Symbols.AssemblySymbol>? lazyUsedAssemblyReferences = _lazyUsedAssemblyReferences;
                        if ((lazyUsedAssemblyReferences != null && !lazyUsedAssemblyReferences!.IsEmpty) || diagnostics.DependenciesBag!.Count != 0)
                        {
                            foreach (Symbols.AssemblySymbol item3 in bindingDiagnosticBag.DependenciesBag!)
                            {
                                ConcurrentSet<Symbols.AssemblySymbol>? lazyUsedAssemblyReferences2 = _lazyUsedAssemblyReferences;
                                if ((lazyUsedAssemblyReferences2 != null && lazyUsedAssemblyReferences2!.Contains(item3)) || diagnostics.DependenciesBag!.Contains(item3))
                                {
                                    bindingDiagnosticBag.DependenciesBag!.Clear();
                                    break;
                                }
                            }
                        }
                        diagnostics.AddDependencies(bindingDiagnosticBag);
                    }
                    bindingDiagnosticBag.Free();
                    pooledHashSet.Free();
                }
            }
            CompleteTrees(filterTree);
        }

        public override void CompleteTrees(SyntaxTree? filterTree)
        {
            if (base.EventQueue != null)
            {
                if (filterTree != null)
                {
                    CompleteTree(filterTree);
                }
                else
                {
                    ImmutableArray<SyntaxTree>.Enumerator enumerator = SyntaxTrees.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        SyntaxTree current = enumerator.Current;
                        CompleteTree(current);
                    }
                }
            }
            if (filterTree == null)
            {
                _usageOfUsingsRecordedInTrees = null;
            }
        }

        internal void RecordImport(UsingDirectiveSyntax syntax)
        {
            RecordImportInternal(syntax);
        }

        internal void RecordImport(ExternAliasDirectiveSyntax syntax)
        {
            RecordImportInternal(syntax);
        }

        private void RecordImportInternal(CSharpSyntaxNode syntax)
        {
            LazyInitializer.EnsureInitialized(ref _lazyImportInfos)!.TryAdd(new ImportInfo(syntax.SyntaxTree, syntax.Kind(), syntax.Span), default(ImmutableArray<Symbols.AssemblySymbol>));
        }

        internal void RecordImportDependencies(UsingDirectiveSyntax syntax, ImmutableArray<Symbols.AssemblySymbol> dependencies)
        {
            _lazyImportInfos!.TryUpdate(new ImportInfo(syntax.SyntaxTree, syntax.Kind(), syntax.Span), dependencies, default(ImmutableArray<Symbols.AssemblySymbol>));
        }

        public override ImmutableArray<Diagnostic> GetParseDiagnostics(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetDiagnostics(CompilationStage.Parse, includeEarlierStages: false, cancellationToken);
        }

        public override ImmutableArray<Diagnostic> GetDeclarationDiagnostics(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetDiagnostics(CompilationStage.Declare, includeEarlierStages: false, cancellationToken);
        }

        public override ImmutableArray<Diagnostic> GetMethodBodyDiagnostics(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetDiagnostics(CompilationStage.Compile, includeEarlierStages: false, cancellationToken);
        }

        public override ImmutableArray<Diagnostic> GetDiagnostics(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetDiagnostics(CompilationStage.Compile, includeEarlierStages: true, cancellationToken);
        }

        internal ImmutableArray<Diagnostic> GetDiagnostics(CompilationStage stage, bool includeEarlierStages, CancellationToken cancellationToken)
        {
            DiagnosticBag instance = DiagnosticBag.GetInstance();
            GetDiagnostics(stage, includeEarlierStages, instance, cancellationToken);
            return instance.ToReadOnlyAndFree();
        }

        public override void GetDiagnostics(CompilationStage stage, bool includeEarlierStages, DiagnosticBag diagnostics, CancellationToken cancellationToken = default(CancellationToken))
        {
            DiagnosticBag incoming = DiagnosticBag.GetInstance();
            GetDiagnosticsWithoutFiltering(stage, includeEarlierStages, new BindingDiagnosticBag(incoming), cancellationToken);
            FilterAndAppendAndFreeDiagnostics(diagnostics, ref incoming, cancellationToken);
        }

        private void GetDiagnosticsWithoutFiltering(CompilationStage stage, bool includeEarlierStages, BindingDiagnosticBag builder, CancellationToken cancellationToken)
        {
            BindingDiagnosticBag builder2 = builder;
            if (stage == CompilationStage.Parse || (stage > CompilationStage.Parse && includeEarlierStages))
            {
                ImmutableArray<SyntaxTree> syntaxTrees = SyntaxTrees;
                ImmutableArray<SyntaxTree>.Enumerator enumerator;
                if (Options.ConcurrentBuild)
                {
                    RoslynParallel.For(0, syntaxTrees.Length, UICultureUtilities.WithCurrentUICulture(delegate (int i)
                    {
                        SyntaxTree syntaxTree = syntaxTrees[i];
                        AppendLoadDirectiveDiagnostics(builder2.DiagnosticBag, _syntaxAndDeclarations, syntaxTree);
                        builder2.AddRange(syntaxTree.GetDiagnostics(cancellationToken));
                    }), cancellationToken);
                }
                else
                {
                    enumerator = syntaxTrees.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        SyntaxTree current = enumerator.Current;
                        cancellationToken.ThrowIfCancellationRequested();
                        AppendLoadDirectiveDiagnostics(builder2.DiagnosticBag, _syntaxAndDeclarations, current);
                        cancellationToken.ThrowIfCancellationRequested();
                        builder2.AddRange(current.GetDiagnostics(cancellationToken));
                    }
                }
                HashSet<ParseOptions> hashSet = new HashSet<ParseOptions>();
                enumerator = syntaxTrees.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxTree current2 = enumerator.Current;
                    cancellationToken.ThrowIfCancellationRequested();
                    if (!current2.Options.Errors.IsDefaultOrEmpty && hashSet.Add(current2.Options))
                    {
                        Location location = current2.GetLocation(TextSpan.FromBounds(0, 0));
                        ImmutableArray<Diagnostic>.Enumerator enumerator2 = current2.Options.Errors.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            Diagnostic current3 = enumerator2.Current;
                            builder2.Add(current3.WithLocation(location));
                        }
                    }
                }
            }
            if (stage == CompilationStage.Declare || (stage > CompilationStage.Declare && includeEarlierStages))
            {
                CheckAssemblyName(builder2.DiagnosticBag);
                builder2.AddRange(Options.Errors);
                if (Options.NullableContextOptions != 0 && LanguageVersion < MessageID.IDS_FeatureNullableReferenceTypes.RequiredVersion() && _syntaxAndDeclarations.ExternalSyntaxTrees.Any())
                {
                    builder2.Add(new CSDiagnostic(new CSDiagnosticInfo(ErrorCode.ERR_NullableOptionNotAvailable, "NullableContextOptions", Options.NullableContextOptions, LanguageVersion.ToDisplayString(), new CSharpRequiredLanguageVersion(MessageID.IDS_FeatureNullableReferenceTypes.RequiredVersion())), Location.None));
                }
                cancellationToken.ThrowIfCancellationRequested();
                builder2.AddRange(GetBoundReferenceManager().Diagnostics);
                cancellationToken.ThrowIfCancellationRequested();
                BindingDiagnosticBag bindingDiagnosticBag = builder2;
                CancellationToken cancellationToken2 = cancellationToken;
                bindingDiagnosticBag.AddRange(GetSourceDeclarationDiagnostics(null, null, null, cancellationToken2), allowMismatchInDependencyAccumulation: true);
                if (base.EventQueue != null && SyntaxTrees.Length == 0)
                {
                    EnsureCompilationEventQueueCompleted();
                }
            }
            cancellationToken.ThrowIfCancellationRequested();
            if (stage == CompilationStage.Compile || (stage > CompilationStage.Compile && includeEarlierStages))
            {
                BindingDiagnosticBag bindingDiagnosticBag2 = new BindingDiagnosticBag(DiagnosticBag.GetInstance(), (builder2.DependenciesBag != null) ? new ConcurrentSet<Symbols.AssemblySymbol>() : null);
                GetDiagnosticsForAllMethodBodies(bindingDiagnosticBag2, doLowering: false, cancellationToken);
                builder2.AddRange(bindingDiagnosticBag2);
                bindingDiagnosticBag2.DiagnosticBag!.Free();
            }
        }

        private static void AppendLoadDirectiveDiagnostics(DiagnosticBag builder, SyntaxAndDeclarationManager syntaxAndDeclarations, SyntaxTree syntaxTree, Func<IEnumerable<Diagnostic>, IEnumerable<Diagnostic>>? locationFilterOpt = null)
        {
            if (!syntaxAndDeclarations.GetLazyState().LoadDirectiveMap.TryGetValue(syntaxTree, out var value))
            {
                return;
            }
            ImmutableArray<LoadDirective>.Enumerator enumerator = value.GetEnumerator();
            while (enumerator.MoveNext())
            {
                IEnumerable<Diagnostic> enumerable = enumerator.Current.Diagnostics;
                if (locationFilterOpt != null)
                {
                    enumerable = locationFilterOpt!(enumerable);
                }
                builder.AddRange(enumerable);
            }
        }

        private void GetDiagnosticsForAllMethodBodies(BindingDiagnosticBag diagnostics, bool doLowering, CancellationToken cancellationToken)
        {
            MethodCompiler.CompileMethodBodies(this, doLowering ? ((PEModuleBuilder)CreateModuleBuilder(EmitOptions.Default, null, null, null, null, null, diagnostics.DiagnosticBag, cancellationToken)) : null, emittingPdb: false, emitTestCoverageData: false, hasDeclarationErrors: false, emitMethodBodies: false, diagnostics, null, cancellationToken);
            DocumentationCommentCompiler.WriteDocumentationCommentXml(this, null, null, diagnostics, cancellationToken);
            ReportUnusedImports(null, diagnostics, cancellationToken);
        }

        private static bool IsDefinedOrImplementedInSourceTree(Symbol symbol, SyntaxTree tree, TextSpan? span)
        {
            if (symbol.IsDefinedInSourceTree(tree, span))
            {
                return true;
            }
            if (symbol.Kind == SymbolKind.Method && symbol.IsImplicitlyDeclared && ((Symbols.MethodSymbol)symbol).MethodKind == MethodKind.Constructor)
            {
                return IsDefinedOrImplementedInSourceTree(symbol.ContainingType, tree, span);
            }
            return false;
        }

        private ImmutableArray<Diagnostic> GetDiagnosticsForMethodBodiesInTree(SyntaxTree tree, TextSpan? span, CancellationToken cancellationToken)
        {
            DiagnosticBag instance = DiagnosticBag.GetInstance();
            BindingDiagnosticBag bindingDiagnosticBag = new BindingDiagnosticBag(instance);
            bool flag = (!span.HasValue || span.Value == tree.GetRoot(cancellationToken).FullSpan) && Compilation.ReportUnusedImportsInTree(tree);
            bool flag2 = false;
            if (flag && UsageOfUsingsRecordedInTrees != null)
            {
                ImmutableArray<SingleNamespaceDeclaration>.Enumerator enumerator = ((SourceNamespaceSymbol)SourceModule.GlobalNamespace).MergedDeclaration.Declarations.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SingleNamespaceDeclaration current = enumerator.Current;
                    if (current.SyntaxReference.SyntaxTree == tree)
                    {
                        if (current.HasGlobalUsings)
                        {
                            flag2 = true;
                        }
                        break;
                    }
                }
            }
            if (flag2)
            {
                ImmutableHashSet<SyntaxTree>? usageOfUsingsRecordedInTrees = UsageOfUsingsRecordedInTrees;
                if (usageOfUsingsRecordedInTrees != null && usageOfUsingsRecordedInTrees!.IsEmpty)
                {
                    compileMethodBodiesAndDocComments(null, null, bindingDiagnosticBag, cancellationToken);
                    _usageOfUsingsRecordedInTrees = null;
                    goto IL_0166;
                }
            }
            compileMethodBodiesAndDocComments(tree, span, bindingDiagnosticBag, cancellationToken);
            if (flag)
            {
                registeredUsageOfUsingsInTree(tree);
            }
            if (flag2)
            {
                BindingDiagnosticBag bindingDiagnosticBag2 = new BindingDiagnosticBag(DiagnosticBag.GetInstance());
                ImmutableArray<SyntaxTree>.Enumerator enumerator2 = SyntaxTrees.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    SyntaxTree current2 = enumerator2.Current;
                    ImmutableHashSet<SyntaxTree> usageOfUsingsRecordedInTrees2 = UsageOfUsingsRecordedInTrees;
                    if (usageOfUsingsRecordedInTrees2 == null)
                    {
                        break;
                    }
                    if (!usageOfUsingsRecordedInTrees2.Contains(current2))
                    {
                        compileMethodBodiesAndDocComments(current2, null, bindingDiagnosticBag2, cancellationToken);
                        registeredUsageOfUsingsInTree(current2);
                        bindingDiagnosticBag2.DiagnosticBag!.Clear();
                    }
                }
                bindingDiagnosticBag2.DiagnosticBag!.Free();
            }
            goto IL_0166;
        IL_0166:
            if (flag)
            {
                ReportUnusedImports(tree, bindingDiagnosticBag, cancellationToken);
            }
            return instance.ToReadOnlyAndFree();
            void compileMethodBodiesAndDocComments(SyntaxTree? filterTree, TextSpan? filterSpan, BindingDiagnosticBag bindingDiagnostics, CancellationToken cancellationToken)
            {
                SyntaxTree filterTree2 = filterTree;
                MethodCompiler.CompileMethodBodies(this, null, emittingPdb: false, emitTestCoverageData: false, hasDeclarationErrors: false, emitMethodBodies: false, bindingDiagnostics, (filterTree2 != null) ? ((Symbol s) => IsDefinedOrImplementedInSourceTree(s, filterTree2, filterSpan)) : null, cancellationToken);
                DocumentationCommentCompiler.WriteDocumentationCommentXml(this, null, null, bindingDiagnostics, cancellationToken, filterTree2, filterSpan);
            }
            void registeredUsageOfUsingsInTree(SyntaxTree tree)
            {
                ImmutableHashSet<SyntaxTree> immutableHashSet = UsageOfUsingsRecordedInTrees;
                while (immutableHashSet != null)
                {
                    ImmutableHashSet<SyntaxTree> immutableHashSet2 = immutableHashSet.Add(tree);
                    if (immutableHashSet2 == immutableHashSet)
                    {
                        break;
                    }
                    if (immutableHashSet2.Count == SyntaxTrees.Length)
                    {
                        _usageOfUsingsRecordedInTrees = null;
                        break;
                    }
                    ImmutableHashSet<SyntaxTree> immutableHashSet3 = Interlocked.CompareExchange(ref _usageOfUsingsRecordedInTrees, immutableHashSet2, immutableHashSet);
                    if (immutableHashSet3 == immutableHashSet)
                    {
                        break;
                    }
                    immutableHashSet = immutableHashSet3;
                }
            }
        }

        private ImmutableBindingDiagnostic<Symbols.AssemblySymbol> GetSourceDeclarationDiagnostics(SyntaxTree? syntaxTree = null, TextSpan? filterSpanWithinTree = null, Func<IEnumerable<Diagnostic>, SyntaxTree, TextSpan?, IEnumerable<Diagnostic>>? locationFilterOpt = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            UsingsFromOptions.Complete(this, cancellationToken);
            SourceLocation locationOpt = null;
            if (syntaxTree != null)
            {
                SyntaxNode root = syntaxTree!.GetRoot(cancellationToken);
                locationOpt = (filterSpanWithinTree.HasValue ? new SourceLocation(syntaxTree, filterSpanWithinTree.Value) : new SourceLocation(root));
            }
            Assembly.ForceComplete(locationOpt, cancellationToken);
            if (syntaxTree == null)
            {
                _declarationDiagnosticsFrozen = true;
                _needsGeneratedAttributes_IsFrozen = true;
            }
            IEnumerable<Diagnostic> enumerable = _lazyDeclarationDiagnostics?.AsEnumerable() ?? Enumerable.Empty<Diagnostic>();
            if (locationFilterOpt != null)
            {
                enumerable = locationFilterOpt!(enumerable, syntaxTree, filterSpanWithinTree);
            }
            ImmutableBindingDiagnostic<Symbols.AssemblySymbol> clsComplianceDiagnostics = GetClsComplianceDiagnostics(syntaxTree, filterSpanWithinTree, cancellationToken);
            return new ImmutableBindingDiagnostic<Symbols.AssemblySymbol>(enumerable.AsImmutable().Concat(clsComplianceDiagnostics.Diagnostics), clsComplianceDiagnostics.Dependencies);
        }

        private ImmutableBindingDiagnostic<Symbols.AssemblySymbol> GetClsComplianceDiagnostics(SyntaxTree? syntaxTree, TextSpan? filterSpanWithinTree, CancellationToken cancellationToken)
        {
            if (syntaxTree != null)
            {
                BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance(withDiagnostics: true, withDependencies: false);
                ClsComplianceChecker.CheckCompliance(this, instance, cancellationToken, syntaxTree, filterSpanWithinTree);
                return instance.ToReadOnlyAndFree();
            }
            if (_lazyClsComplianceDiagnostics.IsDefault || _lazyClsComplianceDependencies.IsDefault)
            {
                BindingDiagnosticBag instance2 = BindingDiagnosticBag.GetInstance();
                ClsComplianceChecker.CheckCompliance(this, instance2, cancellationToken);
                ImmutableBindingDiagnostic<Symbols.AssemblySymbol> immutableBindingDiagnostic = instance2.ToReadOnlyAndFree();
                ImmutableInterlocked.InterlockedInitialize(ref _lazyClsComplianceDependencies, immutableBindingDiagnostic.Dependencies);
                ImmutableInterlocked.InterlockedInitialize(ref _lazyClsComplianceDiagnostics, immutableBindingDiagnostic.Diagnostics);
            }
            return new ImmutableBindingDiagnostic<Symbols.AssemblySymbol>(_lazyClsComplianceDiagnostics, _lazyClsComplianceDependencies);
        }

        private static IEnumerable<Diagnostic> FilterDiagnosticsByLocation(IEnumerable<Diagnostic> diagnostics, SyntaxTree tree, TextSpan? filterSpanWithinTree)
        {
            foreach (Diagnostic diagnostic in diagnostics)
            {
                if (diagnostic.HasIntersectingLocation(tree, filterSpanWithinTree))
                {
                    yield return diagnostic;
                }
            }
        }

        internal ImmutableArray<Diagnostic> GetDiagnosticsForSyntaxTree(CompilationStage stage, SyntaxTree syntaxTree, TextSpan? filterSpanWithinTree, bool includeEarlierStages, CancellationToken cancellationToken = default(CancellationToken))
        {
            SyntaxTree syntaxTree2 = syntaxTree;
            cancellationToken.ThrowIfCancellationRequested();
            DiagnosticBag incoming = DiagnosticBag.GetInstance();
            if (stage == CompilationStage.Parse || (stage > CompilationStage.Parse && includeEarlierStages))
            {
                AppendLoadDirectiveDiagnostics(incoming, _syntaxAndDeclarations, syntaxTree2, (IEnumerable<Diagnostic> diagnostics) => FilterDiagnosticsByLocation(diagnostics, syntaxTree2, filterSpanWithinTree));
                IEnumerable<Diagnostic> diagnostics2 = syntaxTree2.GetDiagnostics(cancellationToken);
                diagnostics2 = FilterDiagnosticsByLocation(diagnostics2, syntaxTree2, filterSpanWithinTree);
                incoming.AddRange(diagnostics2);
            }
            cancellationToken.ThrowIfCancellationRequested();
            if (stage == CompilationStage.Declare || (stage > CompilationStage.Declare && includeEarlierStages))
            {
                ImmutableBindingDiagnostic<Symbols.AssemblySymbol> sourceDeclarationDiagnostics = GetSourceDeclarationDiagnostics(syntaxTree2, filterSpanWithinTree, FilterDiagnosticsByLocation, cancellationToken);
                incoming.AddRange(sourceDeclarationDiagnostics.Diagnostics);
            }
            cancellationToken.ThrowIfCancellationRequested();
            if (stage == CompilationStage.Compile || (stage > CompilationStage.Compile && includeEarlierStages))
            {
                IEnumerable<Diagnostic> diagnostics3 = GetDiagnosticsForMethodBodiesInTree(syntaxTree2, filterSpanWithinTree, cancellationToken);
                diagnostics3 = FilterDiagnosticsByLocation(diagnostics3, syntaxTree2, filterSpanWithinTree);
                incoming.AddRange(diagnostics3);
            }
            DiagnosticBag instance = DiagnosticBag.GetInstance();
            FilterAndAppendAndFreeDiagnostics(instance, ref incoming, cancellationToken);
            return instance.ToReadOnlyAndFree<Diagnostic>();
        }

        protected override void AppendDefaultVersionResource(Stream resourceStream)
        {
            Symbols.SourceAssemblySymbol sourceAssembly = SourceAssembly;
            string text = sourceAssembly.FileVersion ?? sourceAssembly.Identity.Version.ToString();
            Win32ResourceConversions.AppendVersionToResourceStream(resourceStream, !Options.OutputKind.IsApplication(), text, SourceModule.Name, SourceModule.Name, sourceAssembly.InformationalVersion ?? text, fileDescription: sourceAssembly.Title ?? " ", assemblyVersion: sourceAssembly.Identity.Version, legalCopyright: sourceAssembly.Copyright ?? " ", legalTrademarks: sourceAssembly.Trademark, productName: sourceAssembly.Product, comments: sourceAssembly.Description, companyName: sourceAssembly.Company);
        }

        public override CommonPEModuleBuilder? CreateModuleBuilder(EmitOptions emitOptions, IMethodSymbol? debugEntryPoint, Stream? sourceLinkStream, IEnumerable<EmbeddedText>? embeddedTexts, IEnumerable<ResourceDescription>? manifestResources, CompilationTestData? testData, DiagnosticBag diagnostics, CancellationToken cancellationToken)
        {
            string runtimeMetadataVersion = GetRuntimeMetadataVersion(emitOptions, diagnostics);
            if (runtimeMetadataVersion == null)
            {
                return null;
            }
            ModulePropertiesForSerialization serializationProperties = ConstructModuleSerializationProperties(emitOptions, runtimeMetadataVersion);
            if (manifestResources == null)
            {
                manifestResources = SpecializedCollections.EmptyEnumerable<ResourceDescription>();
            }
            PEModuleBuilder pEModuleBuilder;
            if (_options.OutputKind.IsNetModule())
            {
                pEModuleBuilder = new PENetModuleBuilder((SourceModuleSymbol)SourceModule, emitOptions, serializationProperties, manifestResources);
            }
            else
            {
                OutputKind outputKind = (_options.OutputKind.IsValid() ? _options.OutputKind : OutputKind.DynamicallyLinkedLibrary);
                pEModuleBuilder = new PEAssemblyBuilder(SourceAssembly, emitOptions, outputKind, serializationProperties, manifestResources);
            }
            if (debugEntryPoint != null)
            {
                pEModuleBuilder.SetDebugEntryPoint(debugEntryPoint.GetSymbol(), diagnostics);
            }
            pEModuleBuilder.SourceLinkStreamOpt = sourceLinkStream;
            if (embeddedTexts != null)
            {
                pEModuleBuilder.EmbeddedTexts = embeddedTexts;
            }
            if (testData != null)
            {
                pEModuleBuilder.SetMethodTestData(testData!.Methods);
                testData!.Module = pEModuleBuilder;
            }
            return pEModuleBuilder;
        }

        public override bool CompileMethods(CommonPEModuleBuilder moduleBuilder, bool emittingPdb, bool emitMetadataOnly, bool emitTestCoverageData, DiagnosticBag diagnostics, Predicate<ISymbolInternal>? filterOpt, CancellationToken cancellationToken)
        {
            PooledHashSet<int> pooledHashSet = null;
            if (emitMetadataOnly)
            {
                pooledHashSet = PooledHashSet<int>.GetInstance();
                pooledHashSet.Add(501);
            }
            bool flag = !FilterAndAppendDiagnostics(diagnostics, GetDiagnostics(CompilationStage.Declare, includeEarlierStages: true, cancellationToken), pooledHashSet, cancellationToken);
            pooledHashSet?.Free();
            PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)moduleBuilder;
            if (emitMetadataOnly)
            {
                if (flag)
                {
                    return false;
                }
                if (pEModuleBuilder.SourceModule.HasBadAttributes)
                {
                    diagnostics.Add(ErrorCode.ERR_ModuleEmitFailure, NoLocation.Singleton, ((INamedEntity)pEModuleBuilder).Name, new LocalizableResourceString("ModuleHasInvalidAttributes", CodeAnalysisResources.ResourceManager, typeof(CodeAnalysisResources)));
                    return false;
                }
                SynthesizedMetadataCompiler.ProcessSynthesizedMembers(this, pEModuleBuilder, cancellationToken);
            }
            else
            {
                if ((emittingPdb || emitTestCoverageData) && !CreateDebugDocuments(pEModuleBuilder.DebugDocumentsBuilder, pEModuleBuilder.EmbeddedTexts, diagnostics))
                {
                    return false;
                }
                DiagnosticBag incoming = DiagnosticBag.GetInstance();
                MethodCompiler.CompileMethodBodies(this, pEModuleBuilder, emittingPdb, emitTestCoverageData, flag, emitMethodBodies: true, new BindingDiagnosticBag(incoming), filterOpt, cancellationToken);
                if (!flag && !CommonCompiler.HasUnsuppressableErrors(incoming))
                {
                    GenerateModuleInitializer(pEModuleBuilder, incoming);
                }
                bool flag2 = !FilterAndAppendAndFreeDiagnostics(diagnostics, ref incoming, cancellationToken);
                if (flag || flag2)
                {
                    return false;
                }
            }
            return true;
        }

        private void GenerateModuleInitializer(PEModuleBuilder moduleBeingBuilt, DiagnosticBag methodBodyDiagnosticBag)
        {
            if (_moduleInitializerMethods == null)
            {
                return;
            }
            ILBuilder iLBuilder = new ILBuilder(moduleBeingBuilt, new LocalSlotManager(null), OptimizationLevel.Release, areLocalsZeroed: false);
            foreach (Symbols.MethodSymbol item in ((IEnumerable<Symbols.MethodSymbol>)_moduleInitializerMethods).OrderBy((IComparer<Symbols.MethodSymbol>?)LexicalOrderSymbolComparer.Instance))
            {
                iLBuilder.EmitOpCode(ILOpCode.Call, 0);
                iLBuilder.EmitToken(moduleBeingBuilt.Translate(item, methodBodyDiagnosticBag, needDeclaration: true), CSharpSyntaxTree.Dummy.GetRoot(), methodBodyDiagnosticBag);
            }
            iLBuilder.EmitRet(isVoid: true);
            iLBuilder.Realize();
            moduleBeingBuilt.RootModuleType.SetStaticConstructorBody(iLBuilder.RealizedIL);
        }

        public override bool GenerateResourcesAndDocumentationComments(CommonPEModuleBuilder moduleBuilder, Stream? xmlDocStream, Stream? win32Resources, bool useRawWin32Resources, string? outputNameOverride, DiagnosticBag diagnostics, CancellationToken cancellationToken)
        {
            DiagnosticBag incoming = DiagnosticBag.GetInstance();
            SetupWin32Resources(moduleBuilder, win32Resources, useRawWin32Resources, incoming);
            ReportManifestResourceDuplicates(moduleBuilder.ManifestResources, from m in SourceAssembly.Modules.Skip(1)
                                                                              select m.Name, AddedModulesResourceNames(incoming), incoming);
            if (!FilterAndAppendAndFreeDiagnostics(diagnostics, ref incoming, cancellationToken))
            {
                return false;
            }
            cancellationToken.ThrowIfCancellationRequested();
            DiagnosticBag incoming2 = DiagnosticBag.GetInstance();
            string assemblyName = FileNameUtilities.ChangeExtension(outputNameOverride, null);
            DocumentationCommentCompiler.WriteDocumentationCommentXml(this, assemblyName, xmlDocStream, new BindingDiagnosticBag(incoming2), cancellationToken);
            return FilterAndAppendAndFreeDiagnostics(diagnostics, ref incoming2, cancellationToken);
        }

        private IEnumerable<string> AddedModulesResourceNames(DiagnosticBag diagnostics)
        {
            ImmutableArray<Symbols.ModuleSymbol> modules = SourceAssembly.Modules;
            for (int i = 1; i < modules.Length; i++)
            {
                PEModuleSymbol pEModuleSymbol = (PEModuleSymbol)modules[i];
                ImmutableArray<EmbeddedResource> embeddedResourcesOrThrow;
                try
                {
                    embeddedResourcesOrThrow = pEModuleSymbol.Module.GetEmbeddedResourcesOrThrow();
                }
                catch (BadImageFormatException)
                {
                    diagnostics.Add(new CSDiagnosticInfo(ErrorCode.ERR_BindToBogus, pEModuleSymbol), NoLocation.Singleton);
                    continue;
                }
                ImmutableArray<EmbeddedResource>.Enumerator enumerator = embeddedResourcesOrThrow.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current.Name;
                }
            }
        }

        public override EmitDifferenceResult EmitDifference(EmitBaseline baseline, IEnumerable<SemanticEdit> edits, Func<ISymbol, bool> isAddedSymbol, Stream metadataStream, Stream ilStream, Stream pdbStream, ICollection<MethodDefinitionHandle> updatedMethods, CompilationTestData? testData, CancellationToken cancellationToken)
        {
            return EmitHelpers.EmitDifference(this, baseline, edits, isAddedSymbol, metadataStream, ilStream, pdbStream, updatedMethods, testData, cancellationToken);
        }

        internal string? GetRuntimeMetadataVersion(EmitOptions emitOptions, DiagnosticBag diagnostics)
        {
            string runtimeMetadataVersion = GetRuntimeMetadataVersion(emitOptions);
            if (runtimeMetadataVersion != null)
            {
                return runtimeMetadataVersion;
            }
            DiagnosticBag incoming = DiagnosticBag.GetInstance();
            incoming.Add(ErrorCode.WRN_NoRuntimeMetadataVersion, NoLocation.Singleton);
            if (!FilterAndAppendAndFreeDiagnostics(diagnostics, ref incoming, CancellationToken.None))
            {
                return null;
            }
            return string.Empty;
        }

        private string? GetRuntimeMetadataVersion(EmitOptions emitOptions)
        {
            if (Assembly.CorLibrary is PEAssemblySymbol pEAssemblySymbol)
            {
                return pEAssemblySymbol.Assembly.ManifestModule.MetadataVersion;
            }
            return emitOptions.RuntimeMetadataVersion;
        }

        public override void AddDebugSourceDocumentsForChecksumDirectives(DebugDocumentsBuilder documentsBuilder, SyntaxTree tree, DiagnosticBag diagnostics)
        {
            foreach (PragmaChecksumDirectiveTriviaSyntax directive in tree.GetRoot().GetDirectives((DirectiveTriviaSyntax d) => d.Kind() == SyntaxKind.PragmaChecksumDirectiveTrivia && !d.ContainsDiagnostics))
            {
                string valueText = directive.File.ValueText;
                string valueText2 = directive.Bytes.ValueText;
                string text = documentsBuilder.NormalizeDebugDocumentPath(valueText, tree.FilePath);
                DebugSourceDocument debugSourceDocument = documentsBuilder.TryGetDebugDocumentForNormalizedPath(text);
                if (debugSourceDocument != null)
                {
                    if (!debugSourceDocument.IsComputedChecksum)
                    {
                        DebugSourceInfo sourceInfo = debugSourceDocument.GetSourceInfo();
                        if (!ChecksumMatches(valueText2, sourceInfo.Checksum) || !(Guid.Parse(directive.Guid.ValueText) == sourceInfo.ChecksumAlgorithmId))
                        {
                            diagnostics.Add(ErrorCode.WRN_ConflictingChecksum, new SourceLocation(directive), valueText);
                        }
                    }
                }
                else
                {
                    DebugSourceDocument document = new DebugSourceDocument(text, DebugSourceDocument.CorSymLanguageTypeCSharp, MakeChecksumBytes(valueText2), Guid.Parse(directive.Guid.ValueText));
                    documentsBuilder.AddDebugDocument(document);
                }
            }
        }

        private static bool ChecksumMatches(string bytesText, ImmutableArray<byte> bytes)
        {
            if (bytesText.Length != bytes.Length * 2)
            {
                return false;
            }
            int i = 0;
            for (int num = bytesText.Length / 2; i < num; i++)
            {
                if (SyntaxFacts.HexValue(bytesText[i * 2]) * 16 + SyntaxFacts.HexValue(bytesText[i * 2 + 1]) != bytes[i])
                {
                    return false;
                }
            }
            return true;
        }

        private static ImmutableArray<byte> MakeChecksumBytes(string bytesText)
        {
            int num = bytesText.Length / 2;
            ArrayBuilder<byte> instance = ArrayBuilder<byte>.GetInstance(num);
            for (int i = 0; i < num; i++)
            {
                int num2 = SyntaxFacts.HexValue(bytesText[i * 2]) * 16 + SyntaxFacts.HexValue(bytesText[i * 2 + 1]);
                instance.Add((byte)num2);
            }
            return instance.ToImmutableAndFree();
        }

        public override bool HasCodeToEmit()
        {
            ImmutableArray<SyntaxTree>.Enumerator enumerator = SyntaxTrees.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.GetCompilationUnitRoot().Members.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }

        protected override Compilation CommonWithReferences(IEnumerable<MetadataReference> newReferences)
        {
            return WithReferences(newReferences);
        }

        protected override Compilation CommonWithAssemblyName(string? assemblyName)
        {
            return WithAssemblyName(assemblyName);
        }

        protected override SemanticModel CommonGetSemanticModel(SyntaxTree syntaxTree, bool ignoreAccessibility)
        {
            return GetSemanticModel(syntaxTree, ignoreAccessibility);
        }

        protected override Compilation CommonAddSyntaxTrees(IEnumerable<SyntaxTree> trees)
        {
            return AddSyntaxTrees(trees);
        }

        protected override Compilation CommonRemoveSyntaxTrees(IEnumerable<SyntaxTree> trees)
        {
            return RemoveSyntaxTrees(trees);
        }

        protected override Compilation CommonRemoveAllSyntaxTrees()
        {
            return RemoveAllSyntaxTrees();
        }

        protected override Compilation CommonReplaceSyntaxTree(SyntaxTree oldTree, SyntaxTree? newTree)
        {
            return ReplaceSyntaxTree(oldTree, newTree);
        }

        protected override Compilation CommonWithOptions(CompilationOptions options)
        {
            return WithOptions((CSharpCompilationOptions)options);
        }

        protected override Compilation CommonWithScriptCompilationInfo(ScriptCompilationInfo? info)
        {
            return WithScriptCompilationInfo((CSharpScriptCompilationInfo)info);
        }

        protected override bool CommonContainsSyntaxTree(SyntaxTree? syntaxTree)
        {
            return ContainsSyntaxTree(syntaxTree);
        }

        protected override ISymbol? CommonGetAssemblyOrModuleSymbol(MetadataReference reference)
        {
            return GetAssemblyOrModuleSymbol(reference).GetPublicSymbol();
        }

        protected override Compilation CommonClone()
        {
            return Clone();
        }

        protected override INamedTypeSymbolInternal CommonGetSpecialType(SpecialType specialType)
        {
            return GetSpecialType(specialType);
        }

        protected override INamespaceSymbol? CommonGetCompilationNamespace(INamespaceSymbol namespaceSymbol)
        {
            return GetCompilationNamespace(namespaceSymbol).GetPublicSymbol();
        }

        protected override INamedTypeSymbol? CommonGetTypeByMetadataName(string metadataName)
        {
            return GetTypeByMetadataName(metadataName).GetPublicSymbol();
        }

        protected override IArrayTypeSymbol CommonCreateArrayTypeSymbol(ITypeSymbol elementType, int rank, Microsoft.CodeAnalysis.NullableAnnotation elementNullableAnnotation)
        {
            return CreateArrayTypeSymbol(elementType.EnsureCSharpSymbolOrNull("elementType"), rank, elementNullableAnnotation.ToInternalAnnotation()).GetPublicSymbol();
        }

        protected override IPointerTypeSymbol CommonCreatePointerTypeSymbol(ITypeSymbol elementType)
        {
            return CreatePointerTypeSymbol(elementType.EnsureCSharpSymbolOrNull("elementType"), elementType.NullableAnnotation.ToInternalAnnotation()).GetPublicSymbol();
        }

        protected override IFunctionPointerTypeSymbol CommonCreateFunctionPointerTypeSymbol(ITypeSymbol returnType, RefKind returnRefKind, ImmutableArray<ITypeSymbol> parameterTypes, ImmutableArray<RefKind> parameterRefKinds, SignatureCallingConvention callingConvention, ImmutableArray<INamedTypeSymbol> callingConventionTypes)
        {
            if (returnType == null)
            {
                throw new ArgumentNullException("returnType");
            }
            if (parameterTypes.IsDefault)
            {
                throw new ArgumentNullException("parameterTypes");
            }
            for (int j = 0; j < parameterTypes.Length; j++)
            {
                if (parameterTypes[j] == null)
                {
                    throw new ArgumentNullException(string.Format("{0}[{1}]", "parameterTypes", j));
                }
            }
            if (parameterRefKinds.IsDefault)
            {
                throw new ArgumentNullException("parameterRefKinds");
            }
            if (parameterRefKinds.Length != parameterTypes.Length)
            {
                throw new ArgumentException(string.Format(CSharpResources.NotSameNumberParameterTypesAndRefKinds, parameterTypes.Length, parameterRefKinds.Length));
            }
            if (returnRefKind == RefKind.Out)
            {
                throw new ArgumentException(CSharpResources.OutIsNotValidForReturn);
            }
            if (callingConvention != SignatureCallingConvention.Unmanaged && !callingConventionTypes.IsDefaultOrEmpty)
            {
                throw new ArgumentException(string.Format(CSharpResources.CallingConventionTypesRequireUnmanaged, "callingConventionTypes", "callingConvention"));
            }
            if (!callingConvention.IsValid())
            {
                throw new ArgumentOutOfRangeException("callingConvention");
            }
            TypeWithAnnotations returnType2 = TypeWithAnnotations.Create(returnType.EnsureCSharpSymbolOrNull("returnType"), returnType.NullableAnnotation.ToInternalAnnotation());
            ImmutableArray<TypeWithAnnotations> parameterTypes2 = parameterTypes.SelectAsArray((ITypeSymbol type) => TypeWithAnnotations.Create(type.EnsureCSharpSymbolOrNull("parameterTypes"), type.NullableAnnotation.ToInternalAnnotation()));
            CallingConvention num = callingConvention.FromSignatureConvention();
            ImmutableArray<CustomModifier> callingConventionModifiers = ((num == CallingConvention.Unmanaged && !callingConventionTypes.IsDefaultOrEmpty) ? callingConventionTypes.SelectAsArray((INamedTypeSymbol type, int i, CSharpCompilation @this) => getCustomModifierForType(type, @this, i), this) : ImmutableArray<CustomModifier>.Empty);
            return Symbols.FunctionPointerTypeSymbol.CreateFromParts(num, callingConventionModifiers, returnType2, returnRefKind, parameterTypes2, parameterRefKinds, this).GetPublicSymbol();
            static CustomModifier getCustomModifierForType(INamedTypeSymbol type, CSharpCompilation @this, int index)
            {
                if (type == null)
                {
                    throw new ArgumentNullException(string.Format("{0}[{1}]", "callingConventionTypes", index));
                }
                Symbols.NamedTypeSymbol namedTypeSymbol = type.EnsureCSharpSymbolOrNull(string.Format("{0}[{1}]", "callingConventionTypes", index));
                if (!Symbols.FunctionPointerTypeSymbol.IsCallingConventionModifier(namedTypeSymbol) || @this.Assembly.CorLibrary != namedTypeSymbol.ContainingAssembly)
                {
                    throw new ArgumentException(string.Format(CSharpResources.CallingConventionTypeIsInvalid, type.ToDisplayString()));
                }
                return CSharpCustomModifier.CreateOptional(namedTypeSymbol);
            }
        }

        protected override INamedTypeSymbol CommonCreateNativeIntegerTypeSymbol(bool signed)
        {
            return CreateNativeIntegerTypeSymbol(signed).GetPublicSymbol();
        }

        internal new Symbols.NamedTypeSymbol CreateNativeIntegerTypeSymbol(bool signed)
        {
            return GetSpecialType(signed ? SpecialType.System_IntPtr : SpecialType.System_UIntPtr).AsNativeInteger();
        }

        protected override INamedTypeSymbol CommonCreateTupleTypeSymbol(ImmutableArray<ITypeSymbol> elementTypes, ImmutableArray<string?> elementNames, ImmutableArray<Location?> elementLocations, ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation> elementNullableAnnotations)
        {
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance(elementTypes.Length);
            for (int i = 0; i < elementTypes.Length; i++)
            {
                ITypeSymbol typeSymbol = elementTypes[i];
                Symbols.TypeSymbol typeSymbol2 = typeSymbol.EnsureCSharpSymbolOrNull(string.Format("{0}[{1}]", "elementTypes", i));
                NullableAnnotation nullableAnnotation = (elementNullableAnnotations.IsDefault ? typeSymbol.NullableAnnotation : elementNullableAnnotations[i]).ToInternalAnnotation();
                instance.Add(TypeWithAnnotations.Create(typeSymbol2, nullableAnnotation));
            }
            return Symbols.NamedTypeSymbol.CreateTuple(null, instance.ToImmutableAndFree(), elementLocations, elementNames, this, shouldCheckConstraints: false, includeNullability: false, default(ImmutableArray<bool>)).GetPublicSymbol();
        }

        protected override INamedTypeSymbol CommonCreateTupleTypeSymbol(INamedTypeSymbol underlyingType, ImmutableArray<string?> elementNames, ImmutableArray<Location?> elementLocations, ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation> elementNullableAnnotations)
        {
            Symbols.NamedTypeSymbol? namedTypeSymbol = underlyingType.EnsureCSharpSymbolOrNull("underlyingType");
            if (!namedTypeSymbol!.IsTupleTypeOfCardinality(out var tupleCardinality))
            {
                throw new ArgumentException(CodeAnalysisResources.TupleUnderlyingTypeMustBeTupleCompatible, "underlyingType");
            }
            elementNames = Compilation.CheckTupleElementNames(tupleCardinality, elementNames);
            Compilation.CheckTupleElementLocations(tupleCardinality, elementLocations);
            Compilation.CheckTupleElementNullableAnnotations(tupleCardinality, elementNullableAnnotations);
            Symbols.NamedTypeSymbol namedTypeSymbol2 = Symbols.NamedTypeSymbol.CreateTuple(namedTypeSymbol, elementNames, default(ImmutableArray<bool>), elementLocations);
            if (!elementNullableAnnotations.IsDefault)
            {
                namedTypeSymbol2 = namedTypeSymbol2.WithElementTypes(namedTypeSymbol2.TupleElementTypesWithAnnotations.ZipAsArray(elementNullableAnnotations, (TypeWithAnnotations t, Microsoft.CodeAnalysis.NullableAnnotation a) => TypeWithAnnotations.Create(t.Type, a.ToInternalAnnotation())));
            }
            return namedTypeSymbol2.GetPublicSymbol();
        }

        protected override INamedTypeSymbol CommonCreateAnonymousTypeSymbol(ImmutableArray<ITypeSymbol> memberTypes, ImmutableArray<string> memberNames, ImmutableArray<Location> memberLocations, ImmutableArray<bool> memberIsReadOnly, ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation> memberNullableAnnotations)
        {
            int i = 0;
            for (int length = memberTypes.Length; i < length; i++)
            {
                memberTypes[i].EnsureCSharpSymbolOrNull(string.Format("{0}[{1}]", "memberTypes", i));
            }
            if (!memberIsReadOnly.IsDefault && memberIsReadOnly.Any((bool v) => !v))
            {
                throw new ArgumentException("Non-ReadOnly members are not supported in C# anonymous types.");
            }
            ArrayBuilder<AnonymousTypeField> instance = ArrayBuilder<AnonymousTypeField>.GetInstance();
            int j = 0;
            for (int length2 = memberTypes.Length; j < length2; j++)
            {
                Symbols.TypeSymbol symbol = memberTypes[j].GetSymbol();
                string name = memberNames[j];
                Location location = (memberLocations.IsDefault ? Location.None : memberLocations[j]);
                NullableAnnotation nullableAnnotation = (memberNullableAnnotations.IsDefault ? NullableAnnotation.Oblivious : memberNullableAnnotations[j].ToInternalAnnotation());
                instance.Add(new AnonymousTypeField(name, location, TypeWithAnnotations.Create(symbol, nullableAnnotation)));
            }
            AnonymousTypeDescriptor typeDescr = new AnonymousTypeDescriptor(instance.ToImmutableAndFree(), Location.None);
            return AnonymousTypeManager.ConstructAnonymousTypeSymbol(typeDescr).GetPublicSymbol();
        }

        protected override IMethodSymbol? CommonGetEntryPoint(CancellationToken cancellationToken)
        {
            return GetEntryPoint(cancellationToken).GetPublicSymbol();
        }

        public override int CompareSourceLocations(Location loc1, Location loc2)
        {
            int num = CompareSyntaxTreeOrdering(loc1.SourceTree, loc2.SourceTree);
            if (num != 0)
            {
                return num;
            }
            return loc1.SourceSpan.Start - loc2.SourceSpan.Start;
        }

        public override int CompareSourceLocations(SyntaxReference loc1, SyntaxReference loc2)
        {
            int num = CompareSyntaxTreeOrdering(loc1.SyntaxTree, loc2.SyntaxTree);
            if (num != 0)
            {
                return num;
            }
            return loc1.Span.Start - loc2.Span.Start;
        }

        public override bool ContainsSymbolsWithName(Func<string, bool> predicate, SymbolFilter filter = SymbolFilter.TypeAndMember, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (filter == SymbolFilter.None)
            {
                throw new ArgumentException(CSharpResources.NoNoneSearchCriteria, "filter");
            }
            return DeclarationTable.ContainsName(MergedRootDeclaration, predicate, filter, cancellationToken);
        }

        public override IEnumerable<ISymbol> GetSymbolsWithName(Func<string, bool> predicate, SymbolFilter filter = SymbolFilter.TypeAndMember, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("predicate");
            }
            if (filter == SymbolFilter.None)
            {
                throw new ArgumentException(CSharpResources.NoNoneSearchCriteria, "filter");
            }
            return new PredicateSymbolSearcher(this, filter, predicate, cancellationToken).GetSymbolsWithName().GetPublicSymbols();
        }

        public override bool ContainsSymbolsWithName(string name, SymbolFilter filter = SymbolFilter.TypeAndMember, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (filter == SymbolFilter.None)
            {
                throw new ArgumentException(CSharpResources.NoNoneSearchCriteria, "filter");
            }
            return DeclarationTable.ContainsName(MergedRootDeclaration, name, filter, cancellationToken);
        }

        public override IEnumerable<ISymbol> GetSymbolsWithName(string name, SymbolFilter filter = SymbolFilter.TypeAndMember, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetSymbolsWithNameCore(name, filter, cancellationToken).GetPublicSymbols();
        }

        internal IEnumerable<Symbol> GetSymbolsWithNameCore(string name, SymbolFilter filter = SymbolFilter.TypeAndMember, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (filter == SymbolFilter.None)
            {
                throw new ArgumentException(CSharpResources.NoNoneSearchCriteria, "filter");
            }
            return new NameSymbolSearcher(this, filter, name, cancellationToken).GetSymbolsWithName();
        }

        internal bool HasDynamicEmitAttributes(BindingDiagnosticBag diagnostics, Location location)
        {
            if ((object)Binder.GetWellKnownTypeMember(this, WellKnownMember.System_Runtime_CompilerServices_DynamicAttribute__ctor, diagnostics, location) != null)
            {
                return (object)Binder.GetWellKnownTypeMember(this, WellKnownMember.System_Runtime_CompilerServices_DynamicAttribute__ctorTransformFlags, diagnostics, location) != null;
            }
            return false;
        }

        internal bool HasTupleNamesAttributes(BindingDiagnosticBag diagnostics, Location location)
        {
            return (object)Binder.GetWellKnownTypeMember(this, WellKnownMember.System_Runtime_CompilerServices_TupleElementNamesAttribute__ctorTransformNames, diagnostics, location) != null;
        }

        internal bool CanEmitBoolean()
        {
            return CanEmitSpecialType(SpecialType.System_Boolean);
        }

        internal bool CanEmitSpecialType(SpecialType type)
        {
            DiagnosticInfo diagnosticInfo = GetSpecialType(type).GetUseSiteInfo().DiagnosticInfo;
            if (diagnosticInfo != null)
            {
                return diagnosticInfo.Severity != DiagnosticSeverity.Error;
            }
            return true;
        }

        internal bool ShouldEmitNullableAttributes(Symbol symbol)
        {
            if (symbol.ContainingModule != SourceModule)
            {
                return false;
            }
            if (!EmitNullablePublicOnly)
            {
                return true;
            }
            symbol = getExplicitAccessibilitySymbol(symbol);
            if (!AccessCheck.IsEffectivelyPublicOrInternal(symbol, out var isInternal))
            {
                return false;
            }
            if (isInternal)
            {
                return SourceAssembly.InternalsAreVisible;
            }
            return true;
            static Symbol getExplicitAccessibilitySymbol(Symbol symbol)
            {
                while (true)
                {
                    switch (symbol.Kind)
                    {
                        case SymbolKind.Event:
                        case SymbolKind.Parameter:
                        case SymbolKind.Property:
                        case SymbolKind.TypeParameter:
                            break;
                        default:
                            return symbol;
                    }
                    symbol = symbol.ContainingSymbol;
                }
            }
        }

        public override AnalyzerDriver CreateAnalyzerDriver(ImmutableArray<DiagnosticAnalyzer> analyzers, AnalyzerManager analyzerManager, SeverityFilter severityFilter)
        {
            Func<SyntaxNode, SyntaxKind> getKind = (SyntaxNode node) => node.Kind();
            Func<SyntaxTrivia, bool> isComment = (SyntaxTrivia trivia) => trivia.Kind() == SyntaxKind.SingleLineCommentTrivia || trivia.Kind() == SyntaxKind.MultiLineCommentTrivia;
            return new AnalyzerDriver<SyntaxKind>(analyzers, getKind, analyzerManager, severityFilter, isComment);
        }

        internal void SymbolDeclaredEvent(Symbol symbol)
        {
            base.EventQueue?.TryEnqueue(new SymbolDeclaredCompilationEvent(this, symbol.GetPublicSymbol()));
        }

        public override void SerializePdbEmbeddedCompilationOptions(BlobBuilder builder)
        {
            BlobBuilder builder2 = builder;
            WriteValue("language-version", LanguageVersion.ToDisplayString());
            if (Options.CheckOverflow)
            {
                WriteValue("checked", Options.CheckOverflow.ToString());
            }
            if (Options.NullableContextOptions != 0)
            {
                WriteValue("nullable", Options.NullableContextOptions.ToString());
            }
            if (Options.AllowUnsafe)
            {
                WriteValue("unsafe", Options.AllowUnsafe.ToString());
            }
            ImmutableArray<string> preprocessorSymbols = GetPreprocessorSymbols();
            if (preprocessorSymbols.Any())
            {
                WriteValue("define", string.Join(",", preprocessorSymbols));
            }
            void WriteValue(string key, string value)
            {
                builder2.WriteUTF8(key);
                builder2.WriteByte(0);
                builder2.WriteUTF8(value);
                builder2.WriteByte(0);
            }
        }

        private ImmutableArray<string> GetPreprocessorSymbols()
        {
            return ((CSharpSyntaxTree)SyntaxTrees.FirstOrDefault())?.Options.PreprocessorSymbolNames.ToImmutableArray() ?? ImmutableArray<string>.Empty;
        }

        public override ImmutableArray<MetadataReference> GetUsedAssemblyReferences(CancellationToken cancellationToken = default(CancellationToken))
        {
            ConcurrentSet<Symbols.AssemblySymbol> completeSetOfUsedAssemblies = GetCompleteSetOfUsedAssemblies(cancellationToken);
            if (completeSetOfUsedAssemblies == null)
            {
                return ImmutableArray<MetadataReference>.Empty;
            }
            HashSet<MetadataReference> hashSet = new HashSet<MetadataReference>(ReferenceEqualityComparer.Instance);
            ImmutableDictionary<MetadataReference, ImmutableArray<MetadataReference>> mergedAssemblyReferencesMap = GetBoundReferenceManager().MergedAssemblyReferencesMap;
            foreach (MetadataReference reference in base.References)
            {
                if (reference.Properties.Kind == MetadataImageKind.Assembly)
                {
                    Symbol referencedAssemblySymbol = GetBoundReferenceManager().GetReferencedAssemblySymbol(reference);
                    if ((object)referencedAssemblySymbol != null && completeSetOfUsedAssemblies.Contains((Symbols.AssemblySymbol)referencedAssemblySymbol) && hashSet.Add(reference) && mergedAssemblyReferencesMap.TryGetValue(reference, out var value))
                    {
                        hashSet.AddAll(value);
                    }
                }
            }
            ArrayBuilder<MetadataReference> instance = ArrayBuilder<MetadataReference>.GetInstance(hashSet.Count);
            foreach (MetadataReference reference2 in base.References)
            {
                if (hashSet.Contains(reference2))
                {
                    instance.Add(reference2);
                }
            }
            return instance.ToImmutableAndFree();
        }

        private ConcurrentSet<Symbols.AssemblySymbol>? GetCompleteSetOfUsedAssemblies(CancellationToken cancellationToken)
        {
            if (!_usedAssemblyReferencesFrozen && !Volatile.Read(ref _usedAssemblyReferencesFrozen))
            {
                BindingDiagnosticBag bindingDiagnosticBag = new BindingDiagnosticBag(DiagnosticBag.GetInstance(), new ConcurrentSet<Symbols.AssemblySymbol>());
                GetDiagnosticsWithoutFiltering(CompilationStage.Declare, includeEarlierStages: true, bindingDiagnosticBag, cancellationToken);
                bool flag = bindingDiagnosticBag.HasAnyErrors();
                if (!flag)
                {
                    bindingDiagnosticBag.DiagnosticBag!.Clear();
                    GetDiagnosticsForAllMethodBodies(bindingDiagnosticBag, doLowering: true, cancellationToken);
                    flag = bindingDiagnosticBag.HasAnyErrors();
                    if (!flag)
                    {
                        AddUsedAssemblies(bindingDiagnosticBag.DependenciesBag);
                    }
                }
                completeTheSetOfUsedAssemblies(flag, cancellationToken);
                bindingDiagnosticBag.DiagnosticBag!.Free();
            }
            return _lazyUsedAssemblyReferences;
            void addReferencedAssemblies(Symbols.AssemblySymbol assembly, bool includeMainModule, ArrayBuilder<Symbols.AssemblySymbol> stack)
            {
                for (int i = ((!includeMainModule) ? 1 : 0); i < assembly.Modules.Length; i++)
                {
                    ImmutableArray<Symbols.AssemblySymbol>.Enumerator enumerator = assembly.Modules[i].ReferencedAssemblySymbols.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Symbols.AssemblySymbol current = enumerator.Current;
                        addUsedAssembly(current, stack);
                    }
                }
            }
            void addUsedAssembly(Symbols.AssemblySymbol dependency, ArrayBuilder<Symbols.AssemblySymbol> stack)
            {
                if (AddUsedAssembly(dependency))
                {
                    stack.Push(dependency);
                }
            }
            void completeTheSetOfUsedAssemblies(bool seenErrors, CancellationToken cancellationToken)
            {
                if (!_usedAssemblyReferencesFrozen && !Volatile.Read(ref _usedAssemblyReferencesFrozen))
                {
                    if (seenErrors)
                    {
                        ImmutableArray<Symbols.AssemblySymbol>.Enumerator enumerator2 = SourceModule.ReferencedAssemblySymbols.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            Symbols.AssemblySymbol current2 = enumerator2.Current;
                            AddUsedAssembly(current2);
                        }
                    }
                    else
                    {
                        for (int j = 1; j < SourceAssembly.Modules.Length; j++)
                        {
                            ImmutableArray<Symbols.AssemblySymbol>.Enumerator enumerator2 = SourceAssembly.Modules[j].ReferencedAssemblySymbols.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                Symbols.AssemblySymbol current3 = enumerator2.Current;
                                AddUsedAssembly(current3);
                            }
                        }
                        if (_usedAssemblyReferencesFrozen || Volatile.Read(ref _usedAssemblyReferencesFrozen))
                        {
                            return;
                        }
                        if (_lazyUsedAssemblyReferences != null)
                        {
                            lock (_lazyUsedAssemblyReferences)
                            {
                                if (_usedAssemblyReferencesFrozen || Volatile.Read(ref _usedAssemblyReferencesFrozen))
                                {
                                    return;
                                }
                                ArrayBuilder<Symbols.AssemblySymbol> instance = ArrayBuilder<Symbols.AssemblySymbol>.GetInstance(_lazyUsedAssemblyReferences!.Count);
                                instance.AddRange(_lazyUsedAssemblyReferences);
                                while (instance.Count != 0)
                                {
                                    Symbols.AssemblySymbol assemblySymbol = instance.Pop();
                                    if (!(assemblySymbol is Symbols.SourceAssemblySymbol sourceAssemblySymbol))
                                    {
                                        if (assemblySymbol is RetargetingAssemblySymbol retargetingAssemblySymbol)
                                        {
                                            ConcurrentSet<Symbols.AssemblySymbol> completeSetOfUsedAssemblies = retargetingAssemblySymbol.UnderlyingAssembly.DeclaringCompilation.GetCompleteSetOfUsedAssemblies(cancellationToken);
                                            if (completeSetOfUsedAssemblies != null)
                                            {
                                                ImmutableArray<Symbols.AssemblySymbol>.Enumerator enumerator2 = retargetingAssemblySymbol.UnderlyingAssembly.SourceModule.ReferencedAssemblySymbols.GetEnumerator();
                                                while (enumerator2.MoveNext())
                                                {
                                                    Symbols.AssemblySymbol current4 = enumerator2.Current;
                                                    if (!current4.IsLinked && completeSetOfUsedAssemblies.Contains(current4))
                                                    {
                                                        if (!((RetargetingModuleSymbol)retargetingAssemblySymbol.Modules[0]).RetargetingDefinitions(current4, out var to))
                                                        {
                                                            to = current4;
                                                        }
                                                        addUsedAssembly(to, instance);
                                                    }
                                                }
                                            }
                                            addReferencedAssemblies(retargetingAssemblySymbol, includeMainModule: false, instance);
                                        }
                                        else
                                        {
                                            addReferencedAssemblies(assemblySymbol, includeMainModule: true, instance);
                                        }
                                    }
                                    else
                                    {
                                        ConcurrentSet<Symbols.AssemblySymbol> completeSetOfUsedAssemblies = sourceAssemblySymbol.DeclaringCompilation.GetCompleteSetOfUsedAssemblies(cancellationToken);
                                        if (completeSetOfUsedAssemblies != null)
                                        {
                                            ConcurrentSet<Symbols.AssemblySymbol>.KeyEnumerator enumerator3 = completeSetOfUsedAssemblies.GetEnumerator();
                                            while (enumerator3.MoveNext())
                                            {
                                                Symbols.AssemblySymbol current5 = enumerator3.Current;
                                                addUsedAssembly(current5, instance);
                                            }
                                        }
                                    }
                                }
                                instance.Free();
                            }
                        }
                        if ((object)SourceAssembly.CorLibrary != null)
                        {
                            AddUsedAssembly(SourceAssembly.CorLibrary);
                        }
                    }
                    _usedAssemblyReferencesFrozen = true;
                }
            }
        }

        internal void AddUsedAssemblies(ICollection<Symbols.AssemblySymbol>? assemblies)
        {
            if (assemblies.IsNullOrEmpty())
            {
                return;
            }
            foreach (Symbols.AssemblySymbol item in assemblies!)
            {
                AddUsedAssembly(item);
            }
        }

        internal bool AddUsedAssembly(Symbols.AssemblySymbol? assembly)
        {
            if ((object)assembly == null || assembly == SourceAssembly || assembly!.IsMissing)
            {
                return false;
            }
            if (_lazyUsedAssemblyReferences == null)
            {
                Interlocked.CompareExchange(ref _lazyUsedAssemblyReferences, new ConcurrentSet<Symbols.AssemblySymbol>(), null);
            }
            return _lazyUsedAssemblyReferences!.Add(assembly);
        }

        internal EmbeddableAttributes GetNeedsGeneratedAttributes()
        {
            _needsGeneratedAttributes_IsFrozen = true;
            return (EmbeddableAttributes)_needsGeneratedAttributes;
        }

        private void SetNeedsGeneratedAttributes(EmbeddableAttributes attributes)
        {
            ThreadSafeFlagOperations.Set(ref _needsGeneratedAttributes, (int)attributes);
        }

        internal bool GetUsesNullableAttributes()
        {
            _needsGeneratedAttributes_IsFrozen = true;
            return _usesNullableAttributes;
        }

        private void SetUsesNullableAttributes()
        {
            _usesNullableAttributes = true;
        }

        internal Symbol? GetWellKnownTypeMember(WellKnownMember member)
        {
            if (IsMemberMissing(member))
            {
                return null;
            }
            if (_lazyWellKnownTypeMembers == null || (object)_lazyWellKnownTypeMembers[(int)member] == Symbols.ErrorTypeSymbol.UnknownResultType)
            {
                if (_lazyWellKnownTypeMembers == null)
                {
                    Symbol[] array = new Symbol[418];
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = Symbols.ErrorTypeSymbol.UnknownResultType;
                    }
                    Interlocked.CompareExchange(ref _lazyWellKnownTypeMembers, array, null);
                }
                MemberDescriptor descriptor = WellKnownMembers.GetDescriptor(member);
                Symbols.NamedTypeSymbol namedTypeSymbol = ((descriptor.DeclaringTypeId <= 45) ? GetSpecialType((SpecialType)descriptor.DeclaringTypeId) : GetWellKnownType((WellKnownType)descriptor.DeclaringTypeId));
                Symbol value = null;
                if (!namedTypeSymbol.IsErrorType())
                {
                    value = GetRuntimeMember(namedTypeSymbol, in descriptor, WellKnownMemberSignatureComparer, Assembly);
                }
                Interlocked.CompareExchange(ref _lazyWellKnownTypeMembers[(int)member], value, Symbols.ErrorTypeSymbol.UnknownResultType);
            }
            return _lazyWellKnownTypeMembers[(int)member];
        }

        internal Symbols.NamedTypeSymbol GetWellKnownType(WellKnownType type)
        {
            bool ignoreCorLibraryDuplicatedTypes = Options.TopLevelBinderFlags.Includes(BinderFlags.IgnoreCorLibraryDuplicatedTypes);
            int num = (int)(type - 46);
            if (_lazyWellKnownTypes == null || (object)_lazyWellKnownTypes[num] == null)
            {
                if (_lazyWellKnownTypes == null)
                {
                    Interlocked.CompareExchange(ref _lazyWellKnownTypes, new Symbols.NamedTypeSymbol[257], null);
                }
                string metadataName = type.GetMetadataName();
                DiagnosticBag instance = DiagnosticBag.GetInstance();
                (Symbols.AssemblySymbol, Symbols.AssemblySymbol) conflicts = default((Symbols.AssemblySymbol, Symbols.AssemblySymbol));
                Symbols.NamedTypeSymbol namedTypeSymbol;
                if (IsTypeMissing(type))
                {
                    namedTypeSymbol = null;
                }
                else
                {
                    DiagnosticBag warnings = ((type <= WellKnownType.System_IFormatProvider) ? instance : null);
                    namedTypeSymbol = Assembly.GetTypeByMetadataName(metadataName, includeReferences: true, isWellKnownType: true, out conflicts, useCLSCompliantNameArityEncoding: true, warnings, ignoreCorLibraryDuplicatedTypes);
                }
                if ((object)namedTypeSymbol == null)
                {
                    MetadataTypeName fullName = MetadataTypeName.FromFullName(metadataName, useCLSCompliantNameArityEncoding: true);
                    namedTypeSymbol = ((!type.IsValueTupleType()) ? new MissingMetadataTypeSymbol.TopLevel(Assembly.Modules[0], ref fullName, type) : new MissingMetadataTypeSymbol.TopLevel(errorInfo: ((object)conflicts.Item1 != null) ? new CSDiagnosticInfo(ErrorCode.ERR_PredefinedValueTupleTypeAmbiguous3, fullName.FullName, conflicts.Item1, conflicts.Item2) : new CSDiagnosticInfo(ErrorCode.ERR_PredefinedValueTupleTypeNotFound, fullName.FullName), module: Assembly.Modules[0], fullName: ref fullName, wellKnownType: type));
                }
                if ((object)Interlocked.CompareExchange(ref _lazyWellKnownTypes[num], namedTypeSymbol, null) == null)
                {
                    AdditionalCodegenWarnings.AddRange(instance);
                }
                instance.Free();
            }
            return _lazyWellKnownTypes[num];
        }

        internal bool IsAttributeType(Symbols.TypeSymbol type)
        {
            CompoundUseSiteInfo<Symbols.AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<Symbols.AssemblySymbol>.Discarded;
            return IsEqualOrDerivedFromWellKnownClass(type, WellKnownType.System_Attribute, ref useSiteInfo);
        }

        public override bool IsAttributeType(ITypeSymbol type)
        {
            return IsAttributeType(type.EnsureCSharpSymbolOrNull("type"));
        }

        internal bool IsExceptionType(Symbols.TypeSymbol type, ref CompoundUseSiteInfo<Symbols.AssemblySymbol> useSiteInfo)
        {
            return IsEqualOrDerivedFromWellKnownClass(type, WellKnownType.System_Exception, ref useSiteInfo);
        }

        internal bool IsReadOnlySpanType(Symbols.TypeSymbol type)
        {
            return Symbols.TypeSymbol.Equals(type.OriginalDefinition, GetWellKnownType(WellKnownType.System_ReadOnlySpan_T), TypeCompareKind.ConsiderEverything);
        }

        internal bool IsEqualOrDerivedFromWellKnownClass(Symbols.TypeSymbol type, WellKnownType wellKnownType, ref CompoundUseSiteInfo<Symbols.AssemblySymbol> useSiteInfo)
        {
            if (type.Kind != SymbolKind.NamedType || type.TypeKind != TypeKind.Class)
            {
                return false;
            }
            Symbols.NamedTypeSymbol wellKnownType2 = GetWellKnownType(wellKnownType);
            if (!type.Equals(wellKnownType2, TypeCompareKind.ConsiderEverything))
            {
                return type.IsDerivedFrom(wellKnownType2, TypeCompareKind.ConsiderEverything, ref useSiteInfo);
            }
            return true;
        }

        public override bool IsSystemTypeReference(ITypeSymbolInternal type)
        {
            return Symbols.TypeSymbol.Equals((Symbols.TypeSymbol)type, GetWellKnownType(WellKnownType.System_Type), TypeCompareKind.ConsiderEverything);
        }

        public override ISymbolInternal? CommonGetWellKnownTypeMember(WellKnownMember member)
        {
            return GetWellKnownTypeMember(member);
        }

        public override ITypeSymbolInternal CommonGetWellKnownType(WellKnownType wellknownType)
        {
            return GetWellKnownType(wellknownType);
        }

        internal static Symbol? GetRuntimeMember(Symbols.NamedTypeSymbol declaringType, in MemberDescriptor descriptor, SignatureComparer<Symbols.MethodSymbol, Symbols.FieldSymbol, Symbols.PropertySymbol, Symbols.TypeSymbol, Symbols.ParameterSymbol> comparer, Symbols.AssemblySymbol? accessWithinOpt)
        {
            return GetRuntimeMember(declaringType.GetMembers(descriptor.Name), in descriptor, comparer, accessWithinOpt);
        }

        internal static Symbol? GetRuntimeMember(ImmutableArray<Symbol> members, in MemberDescriptor descriptor, SignatureComparer<Symbols.MethodSymbol, Symbols.FieldSymbol, Symbols.PropertySymbol, Symbols.TypeSymbol, Symbols.ParameterSymbol> comparer, Symbols.AssemblySymbol? accessWithinOpt)
        {
            MethodKind methodKind = MethodKind.Ordinary;
            bool flag = (descriptor.Flags & MemberFlags.Static) != 0;
            Symbol symbol = null;
            SymbolKind symbolKind;
            switch (descriptor.Flags & MemberFlags.KindMask)
            {
                case MemberFlags.Constructor:
                    symbolKind = SymbolKind.Method;
                    methodKind = MethodKind.Constructor;
                    break;
                case MemberFlags.Method:
                    symbolKind = SymbolKind.Method;
                    break;
                case MemberFlags.PropertyGet:
                    symbolKind = SymbolKind.Method;
                    methodKind = MethodKind.PropertyGet;
                    break;
                case MemberFlags.Field:
                    symbolKind = SymbolKind.Field;
                    break;
                case MemberFlags.Property:
                    symbolKind = SymbolKind.Property;
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(descriptor.Flags);
            }
            ImmutableArray<Symbol>.Enumerator enumerator = members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (!current.Name.Equals(descriptor.Name) || current.Kind != symbolKind || current.IsStatic != flag || (current.DeclaredAccessibility != Accessibility.Public && ((object)accessWithinOpt == null || !Symbol.IsSymbolAccessible(current, accessWithinOpt))))
                {
                    continue;
                }
                switch (symbolKind)
                {
                    case SymbolKind.Method:
                        {
                            Symbols.MethodSymbol methodSymbol = (Symbols.MethodSymbol)current;
                            MethodKind methodKind2 = methodSymbol.MethodKind;
                            if (methodKind2 == MethodKind.Conversion || methodKind2 == MethodKind.UserDefinedOperator)
                            {
                                methodKind2 = MethodKind.Ordinary;
                            }
                            if (methodSymbol.Arity != descriptor.Arity || methodKind2 != methodKind || (descriptor.Flags & MemberFlags.Virtual) != 0 != (methodSymbol.IsVirtual || methodSymbol.IsOverride || methodSymbol.IsAbstract) || !comparer.MatchMethodSignature(methodSymbol, descriptor.Signature))
                            {
                                continue;
                            }
                            break;
                        }
                    case SymbolKind.Property:
                        {
                            Symbols.PropertySymbol propertySymbol = (Symbols.PropertySymbol)current;
                            if ((descriptor.Flags & MemberFlags.Virtual) != 0 != (propertySymbol.IsVirtual || propertySymbol.IsOverride || propertySymbol.IsAbstract) || !comparer.MatchPropertySignature(propertySymbol, descriptor.Signature))
                            {
                                continue;
                            }
                            break;
                        }
                    case SymbolKind.Field:
                        if (!comparer.MatchFieldSignature((Symbols.FieldSymbol)current, descriptor.Signature))
                        {
                            continue;
                        }
                        break;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(symbolKind);
                }
                if ((object)symbol != null)
                {
                    symbol = null;
                    break;
                }
                symbol = current;
            }
            return symbol;
        }

        internal SynthesizedAttributeData? TrySynthesizeAttribute(WellKnownMember constructor, ImmutableArray<TypedConstant> arguments = default(ImmutableArray<TypedConstant>), ImmutableArray<KeyValuePair<WellKnownMember, TypedConstant>> namedArguments = default(ImmutableArray<KeyValuePair<WellKnownMember, TypedConstant>>), bool isOptionalUse = false)
        {
            Symbols.MethodSymbol methodSymbol = (Symbols.MethodSymbol)Binder.GetWellKnownTypeMember(this, constructor, out UseSiteInfo<Symbols.AssemblySymbol> useSiteInfo, isOptional: true);
            if ((object)methodSymbol == null)
            {
                return null;
            }
            if (arguments.IsDefault)
            {
                arguments = ImmutableArray<TypedConstant>.Empty;
            }
            ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments2;
            if (namedArguments.IsDefault)
            {
                namedArguments2 = ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty;
            }
            else
            {
                ArrayBuilder<KeyValuePair<string, TypedConstant>> arrayBuilder = new ArrayBuilder<KeyValuePair<string, TypedConstant>>(namedArguments.Length);
                ImmutableArray<KeyValuePair<WellKnownMember, TypedConstant>>.Enumerator enumerator = namedArguments.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    KeyValuePair<WellKnownMember, TypedConstant> current = enumerator.Current;
                    Symbol wellKnownTypeMember = Binder.GetWellKnownTypeMember(this, current.Key, out useSiteInfo, isOptional: true);
                    if (wellKnownTypeMember == null || wellKnownTypeMember is Symbols.ErrorTypeSymbol)
                    {
                        return null;
                    }
                    arrayBuilder.Add(new KeyValuePair<string, TypedConstant>(wellKnownTypeMember.Name, current.Value));
                }
                namedArguments2 = arrayBuilder.ToImmutableAndFree();
            }
            return new SynthesizedAttributeData(methodSymbol, arguments, namedArguments2);
        }

        internal SynthesizedAttributeData? TrySynthesizeAttribute(SpecialMember constructor, bool isOptionalUse = false)
        {
            Symbols.MethodSymbol methodSymbol = (Symbols.MethodSymbol)GetSpecialTypeMember(constructor);
            if ((object)methodSymbol == null)
            {
                return null;
            }
            return new SynthesizedAttributeData(methodSymbol, ImmutableArray<TypedConstant>.Empty, ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty);
        }

        internal SynthesizedAttributeData? SynthesizeDecimalConstantAttribute(decimal value)
        {
            value.GetBits(out var isNegative, out var scale, out var low, out var mid, out var high);
            Symbols.NamedTypeSymbol specialType = GetSpecialType(SpecialType.System_Byte);
            Symbols.NamedTypeSymbol specialType2 = GetSpecialType(SpecialType.System_UInt32);
            return TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_DecimalConstantAttribute__ctor, ImmutableArray.Create<TypedConstant>(new TypedConstant(specialType, TypedConstantKind.Primitive, scale), new TypedConstant(specialType, TypedConstantKind.Primitive, (byte)(isNegative ? 128u : 0u)), new TypedConstant(specialType2, TypedConstantKind.Primitive, high), new TypedConstant(specialType2, TypedConstantKind.Primitive, mid), new TypedConstant(specialType2, TypedConstantKind.Primitive, low)));
        }

        internal SynthesizedAttributeData? SynthesizeDebuggerBrowsableNeverAttribute()
        {
            if (Options.OptimizationLevel != 0)
            {
                return null;
            }
            return TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggerBrowsableAttribute__ctor, ImmutableArray.Create(new TypedConstant(GetWellKnownType(WellKnownType.System_Diagnostics_DebuggerBrowsableState), TypedConstantKind.Enum, DebuggerBrowsableState.Never)));
        }

        internal SynthesizedAttributeData? SynthesizeDebuggerStepThroughAttribute()
        {
            if (Options.OptimizationLevel != 0)
            {
                return null;
            }
            return TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggerStepThroughAttribute__ctor);
        }

        private void EnsureEmbeddableAttributeExists(EmbeddableAttributes attribute, BindingDiagnosticBag? diagnostics, Location location, bool modifyCompilation)
        {
            if (CheckIfAttributeShouldBeEmbedded(attribute, diagnostics, location) && modifyCompilation)
            {
                SetNeedsGeneratedAttributes(attribute);
            }
            if ((attribute & (EmbeddableAttributes.NullableAttribute | EmbeddableAttributes.NullableContextAttribute)) != 0 && modifyCompilation)
            {
                SetUsesNullableAttributes();
            }
        }

        internal void EnsureIsReadOnlyAttributeExists(BindingDiagnosticBag? diagnostics, Location location, bool modifyCompilation)
        {
            EnsureEmbeddableAttributeExists(EmbeddableAttributes.IsReadOnlyAttribute, diagnostics, location, modifyCompilation);
        }

        internal void EnsureIsByRefLikeAttributeExists(BindingDiagnosticBag? diagnostics, Location location, bool modifyCompilation)
        {
            EnsureEmbeddableAttributeExists(EmbeddableAttributes.IsByRefLikeAttribute, diagnostics, location, modifyCompilation);
        }

        internal void EnsureIsUnmanagedAttributeExists(BindingDiagnosticBag? diagnostics, Location location, bool modifyCompilation)
        {
            EnsureEmbeddableAttributeExists(EmbeddableAttributes.IsUnmanagedAttribute, diagnostics, location, modifyCompilation);
        }

        internal void EnsureNullableAttributeExists(BindingDiagnosticBag? diagnostics, Location location, bool modifyCompilation)
        {
            EnsureEmbeddableAttributeExists(EmbeddableAttributes.NullableAttribute, diagnostics, location, modifyCompilation);
        }

        internal void EnsureNullableContextAttributeExists(BindingDiagnosticBag? diagnostics, Location location, bool modifyCompilation)
        {
            EnsureEmbeddableAttributeExists(EmbeddableAttributes.NullableContextAttribute, diagnostics, location, modifyCompilation);
        }

        internal void EnsureNativeIntegerAttributeExists(BindingDiagnosticBag? diagnostics, Location location, bool modifyCompilation)
        {
            EnsureEmbeddableAttributeExists(EmbeddableAttributes.NativeIntegerAttribute, diagnostics, location, modifyCompilation);
        }

        internal bool CheckIfAttributeShouldBeEmbedded(EmbeddableAttributes attribute, BindingDiagnosticBag? diagnosticsOpt, Location locationOpt)
        {
            return attribute switch
            {
                EmbeddableAttributes.IsReadOnlyAttribute => CheckIfAttributeShouldBeEmbedded(diagnosticsOpt, locationOpt, WellKnownType.System_Runtime_CompilerServices_IsReadOnlyAttribute, WellKnownMember.System_Runtime_CompilerServices_IsReadOnlyAttribute__ctor),
                EmbeddableAttributes.IsByRefLikeAttribute => CheckIfAttributeShouldBeEmbedded(diagnosticsOpt, locationOpt, WellKnownType.System_Runtime_CompilerServices_IsByRefLikeAttribute, WellKnownMember.System_Runtime_CompilerServices_IsByRefLikeAttribute__ctor),
                EmbeddableAttributes.IsUnmanagedAttribute => CheckIfAttributeShouldBeEmbedded(diagnosticsOpt, locationOpt, WellKnownType.System_Runtime_CompilerServices_IsUnmanagedAttribute, WellKnownMember.System_Runtime_CompilerServices_IsUnmanagedAttribute__ctor),
                EmbeddableAttributes.NullableAttribute => CheckIfAttributeShouldBeEmbedded(diagnosticsOpt, locationOpt, WellKnownType.System_Runtime_CompilerServices_NullableAttribute, WellKnownMember.System_Runtime_CompilerServices_NullableAttribute__ctorByte, WellKnownMember.System_Runtime_CompilerServices_NullableAttribute__ctorTransformFlags),
                EmbeddableAttributes.NullableContextAttribute => CheckIfAttributeShouldBeEmbedded(diagnosticsOpt, locationOpt, WellKnownType.System_Runtime_CompilerServices_NullableContextAttribute, WellKnownMember.System_Runtime_CompilerServices_NullableContextAttribute__ctor),
                EmbeddableAttributes.NullablePublicOnlyAttribute => CheckIfAttributeShouldBeEmbedded(diagnosticsOpt, locationOpt, WellKnownType.System_Runtime_CompilerServices_NullablePublicOnlyAttribute, WellKnownMember.System_Runtime_CompilerServices_NullablePublicOnlyAttribute__ctor),
                EmbeddableAttributes.NativeIntegerAttribute => CheckIfAttributeShouldBeEmbedded(diagnosticsOpt, locationOpt, WellKnownType.System_Runtime_CompilerServices_NativeIntegerAttribute, WellKnownMember.System_Runtime_CompilerServices_NativeIntegerAttribute__ctor, WellKnownMember.System_Runtime_CompilerServices_NativeIntegerAttribute__ctorTransformFlags),
                _ => throw ExceptionUtilities.UnexpectedValue(attribute),
            };
        }

        private bool CheckIfAttributeShouldBeEmbedded(BindingDiagnosticBag? diagnosticsOpt, Location? locationOpt, WellKnownType attributeType, WellKnownMember attributeCtor, WellKnownMember? secondAttributeCtor = null)
        {
            Symbols.NamedTypeSymbol wellKnownType = GetWellKnownType(attributeType);
            if (wellKnownType is MissingMetadataTypeSymbol)
            {
                if (Options.OutputKind != OutputKind.NetModule)
                {
                    return true;
                }
                if (diagnosticsOpt != null)
                {
                    Binder.ReportUseSite(wellKnownType, diagnosticsOpt, locationOpt);
                }
            }
            else if (diagnosticsOpt != null && Binder.GetWellKnownTypeMember(this, attributeCtor, diagnosticsOpt, locationOpt) != null && secondAttributeCtor.HasValue)
            {
                Binder.GetWellKnownTypeMember(this, secondAttributeCtor.Value, diagnosticsOpt, locationOpt);
            }
            return false;
        }

        internal SynthesizedAttributeData? SynthesizeDebuggableAttribute()
        {
            if (GetWellKnownType(WellKnownType.System_Diagnostics_DebuggableAttribute) is MissingMetadataTypeSymbol)
            {
                return null;
            }
            Symbols.TypeSymbol wellKnownType = GetWellKnownType(WellKnownType.System_Diagnostics_DebuggableAttribute__DebuggingModes);
            if (wellKnownType is MissingMetadataTypeSymbol)
            {
                return null;
            }
            Symbols.FieldSymbol fieldSymbol = (Symbols.FieldSymbol)GetWellKnownTypeMember(WellKnownMember.System_Diagnostics_DebuggableAttribute_DebuggingModes__IgnoreSymbolStoreSequencePoints);
            if ((object)fieldSymbol == null || !fieldSymbol.HasConstantValue)
            {
                return null;
            }
            int num = fieldSymbol.GetConstantValue(ConstantFieldsInProgress.Empty, earlyDecodingWellKnownAttributes: false).Int32Value;
            if (_options.OptimizationLevel == OptimizationLevel.Debug)
            {
                Symbols.FieldSymbol fieldSymbol2 = (Symbols.FieldSymbol)GetWellKnownTypeMember(WellKnownMember.System_Diagnostics_DebuggableAttribute_DebuggingModes__Default);
                if ((object)fieldSymbol2 == null || !fieldSymbol2.HasConstantValue)
                {
                    return null;
                }
                Symbols.FieldSymbol fieldSymbol3 = (Symbols.FieldSymbol)GetWellKnownTypeMember(WellKnownMember.System_Diagnostics_DebuggableAttribute_DebuggingModes__DisableOptimizations);
                if ((object)fieldSymbol3 == null || !fieldSymbol3.HasConstantValue)
                {
                    return null;
                }
                num |= fieldSymbol2.GetConstantValue(ConstantFieldsInProgress.Empty, earlyDecodingWellKnownAttributes: false).Int32Value;
                num |= fieldSymbol3.GetConstantValue(ConstantFieldsInProgress.Empty, earlyDecodingWellKnownAttributes: false).Int32Value;
            }
            if (_options.EnableEditAndContinue)
            {
                Symbols.FieldSymbol fieldSymbol4 = (Symbols.FieldSymbol)GetWellKnownTypeMember(WellKnownMember.System_Diagnostics_DebuggableAttribute_DebuggingModes__EnableEditAndContinue);
                if ((object)fieldSymbol4 == null || !fieldSymbol4.HasConstantValue)
                {
                    return null;
                }
                num |= fieldSymbol4.GetConstantValue(ConstantFieldsInProgress.Empty, earlyDecodingWellKnownAttributes: false).Int32Value;
            }
            TypedConstant item = new TypedConstant(wellKnownType, TypedConstantKind.Enum, num);
            return TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggableAttribute__ctorDebuggingModes, ImmutableArray.Create(item));
        }

        internal SynthesizedAttributeData? SynthesizeDynamicAttribute(Symbols.TypeSymbol type, int customModifiersCount, RefKind refKindOpt = RefKind.None)
        {
            if (type.IsDynamic() && refKindOpt == RefKind.None && customModifiersCount == 0)
            {
                return TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_DynamicAttribute__ctor);
            }
            Symbols.NamedTypeSymbol specialType = GetSpecialType(SpecialType.System_Boolean);
            ImmutableArray<TypedConstant> array = DynamicTransformsEncoder.Encode(type, refKindOpt, customModifiersCount, specialType);
            ImmutableArray<TypedConstant> arguments = ImmutableArray.Create(new TypedConstant(Symbols.ArrayTypeSymbol.CreateSZArray(specialType.ContainingAssembly, TypeWithAnnotations.Create(specialType)), array));
            return TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_DynamicAttribute__ctorTransformFlags, arguments);
        }

        internal SynthesizedAttributeData? SynthesizeTupleNamesAttribute(Symbols.TypeSymbol type)
        {
            Symbols.NamedTypeSymbol specialType = GetSpecialType(SpecialType.System_String);
            ImmutableArray<TypedConstant> array = TupleNamesEncoder.Encode(type, specialType);
            ImmutableArray<TypedConstant> arguments = ImmutableArray.Create(new TypedConstant(Symbols.ArrayTypeSymbol.CreateSZArray(specialType.ContainingAssembly, TypeWithAnnotations.Create(specialType)), array));
            return TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_TupleElementNamesAttribute__ctorTransformNames, arguments);
        }

        internal SynthesizedAttributeData? SynthesizeAttributeUsageAttribute(AttributeTargets targets, bool allowMultiple, bool inherited)
        {
            Symbols.NamedTypeSymbol wellKnownType = GetWellKnownType(WellKnownType.System_AttributeTargets);
            Symbols.NamedTypeSymbol specialType = GetSpecialType(SpecialType.System_Boolean);
            ImmutableArray<TypedConstant> arguments = ImmutableArray.Create(new TypedConstant(wellKnownType, TypedConstantKind.Enum, targets));
            ImmutableArray<KeyValuePair<WellKnownMember, TypedConstant>> namedArguments = ImmutableArray.Create(new KeyValuePair<WellKnownMember, TypedConstant>(WellKnownMember.System_AttributeUsageAttribute__AllowMultiple, new TypedConstant(specialType, TypedConstantKind.Primitive, allowMultiple)), new KeyValuePair<WellKnownMember, TypedConstant>(WellKnownMember.System_AttributeUsageAttribute__Inherited, new TypedConstant(specialType, TypedConstantKind.Primitive, inherited)));
            return TrySynthesizeAttribute(WellKnownMember.System_AttributeUsageAttribute__ctor, arguments, namedArguments);
        }
    }
}
