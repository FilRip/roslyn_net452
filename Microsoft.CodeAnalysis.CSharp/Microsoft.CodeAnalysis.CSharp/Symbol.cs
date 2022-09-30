using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class Symbol : IReference, ISymbolInternal, IFormattable
    {
        [Flags()]
        internal enum AllowedRequiredModifierType
        {
            None = 0,
            System_Runtime_CompilerServices_Volatile = 1,
            System_Runtime_InteropServices_InAttribute = 2,
            System_Runtime_CompilerServices_IsExternalInit = 4,
            System_Runtime_CompilerServices_OutAttribute = 8
        }

        [Flags()]
        internal enum ReservedAttributes
        {
            DynamicAttribute = 2,
            IsReadOnlyAttribute = 4,
            IsUnmanagedAttribute = 8,
            IsByRefLikeAttribute = 0x10,
            TupleElementNamesAttribute = 0x20,
            NullableAttribute = 0x40,
            NullableContextAttribute = 0x80,
            NullablePublicOnlyAttribute = 0x100,
            NativeIntegerAttribute = 0x200,
            CaseSensitiveExtensionAttribute = 0x400
        }

        private ISymbol _lazyISymbol;

        private static readonly SymbolDisplayFormat s_debuggerDisplayFormat = SymbolDisplayFormat.TestFormat.AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier | SymbolDisplayMiscellaneousOptions.IncludeNotNullableReferenceTypeModifier).WithCompilerInternalOptions(SymbolDisplayCompilerInternalOptions.None);

        internal Symbol AdaptedSymbol => this;

        internal virtual bool RequiresCompletion => false;

        public virtual string Name => string.Empty;

        public virtual string MetadataName => Name;

        public abstract SymbolKind Kind { get; }

        public abstract Symbol ContainingSymbol { get; }

        public virtual NamedTypeSymbol ContainingType
        {
            get
            {
                Symbol containingSymbol = ContainingSymbol;
                NamedTypeSymbol namedTypeSymbol = containingSymbol as NamedTypeSymbol;
                if ((object)namedTypeSymbol == containingSymbol)
                {
                    return namedTypeSymbol;
                }
                return containingSymbol.ContainingType;
            }
        }

        public virtual NamespaceSymbol ContainingNamespace
        {
            get
            {
                Symbol containingSymbol = ContainingSymbol;
                while ((object)containingSymbol != null)
                {
                    if (containingSymbol is NamespaceSymbol result)
                    {
                        return result;
                    }
                    containingSymbol = containingSymbol.ContainingSymbol;
                }
                return null;
            }
        }

        public virtual AssemblySymbol ContainingAssembly => ContainingSymbol?.ContainingAssembly;

        internal virtual CSharpCompilation DeclaringCompilation
        {
            get
            {
                switch (Kind)
                {
                    case SymbolKind.ErrorType:
                        return null;
                    case SymbolKind.Assembly:
                        return null;
                    case SymbolKind.NetModule:
                        return null;
                    default:
                        if (ContainingModule is SourceModuleSymbol sourceModuleSymbol)
                        {
                            return sourceModuleSymbol.DeclaringCompilation;
                        }
                        return null;
                }
            }
        }

        Compilation ISymbolInternal.DeclaringCompilation => DeclaringCompilation;

        string ISymbolInternal.Name => Name;

        string ISymbolInternal.MetadataName => MetadataName;

        ISymbolInternal ISymbolInternal.ContainingSymbol => ContainingSymbol;

        IModuleSymbolInternal ISymbolInternal.ContainingModule => ContainingModule;

        IAssemblySymbolInternal ISymbolInternal.ContainingAssembly => ContainingAssembly;

        ImmutableArray<Location> ISymbolInternal.Locations => Locations;

        INamespaceSymbolInternal ISymbolInternal.ContainingNamespace => ContainingNamespace;

        bool ISymbolInternal.IsImplicitlyDeclared => IsImplicitlyDeclared;

        INamedTypeSymbolInternal ISymbolInternal.ContainingType => ContainingType;

        internal virtual ModuleSymbol ContainingModule => ContainingSymbol?.ContainingModule;

        internal virtual int? MemberIndexOpt => null;

        public Symbol OriginalDefinition => OriginalSymbolDefinition;

        protected virtual Symbol OriginalSymbolDefinition => this;

        public bool IsDefinition => (object)this == OriginalDefinition;

        public abstract ImmutableArray<Location> Locations { get; }

        public abstract ImmutableArray<SyntaxReference> DeclaringSyntaxReferences { get; }

        public abstract Accessibility DeclaredAccessibility { get; }

        public abstract bool IsStatic { get; }

        public abstract bool IsVirtual { get; }

        public abstract bool IsOverride { get; }

        public abstract bool IsAbstract { get; }

        public abstract bool IsSealed { get; }

        public abstract bool IsExtern { get; }

        public virtual bool IsImplicitlyDeclared => false;

        public bool CanBeReferencedByName
        {
            get
            {
                switch (Kind)
                {
                    case SymbolKind.Alias:
                    case SymbolKind.Label:
                    case SymbolKind.Local:
                    case SymbolKind.RangeVariable:
                        return true;
                    case SymbolKind.NamedType:
                        if (((NamedTypeSymbol)this).IsSubmissionClass)
                        {
                            return false;
                        }
                        break;
                    case SymbolKind.Property:
                        {
                            PropertySymbol propertySymbol = (PropertySymbol)this;
                            if (propertySymbol.IsIndexer || propertySymbol.MustCallMethodsDirectly)
                            {
                                return false;
                            }
                            break;
                        }
                    case SymbolKind.Method:
                        {
                            MethodSymbol methodSymbol = (MethodSymbol)this;
                            switch (methodSymbol.MethodKind)
                            {
                                case MethodKind.Destructor:
                                    return true;
                                case MethodKind.DelegateInvoke:
                                    return true;
                                case MethodKind.PropertyGet:
                                case MethodKind.PropertySet:
                                    if (!((PropertySymbol)methodSymbol.AssociatedSymbol).CanCallMethodsDirectly())
                                    {
                                        return false;
                                    }
                                    break;
                                default:
                                    return false;
                                case MethodKind.Ordinary:
                                case MethodKind.ReducedExtension:
                                case MethodKind.LocalFunction:
                                    break;
                            }
                            break;
                        }
                    case SymbolKind.ArrayType:
                    case SymbolKind.Assembly:
                    case SymbolKind.DynamicType:
                    case SymbolKind.NetModule:
                    case SymbolKind.PointerType:
                    case SymbolKind.Discard:
                    case SymbolKind.FunctionPointerType:
                        return false;
                    default:
                        throw ExceptionUtilities.UnexpectedValue(Kind);
                    case SymbolKind.ErrorType:
                    case SymbolKind.Event:
                    case SymbolKind.Field:
                    case SymbolKind.Namespace:
                    case SymbolKind.Parameter:
                    case SymbolKind.TypeParameter:
                        break;
                }
                if (SyntaxFacts.IsValidIdentifier(Name))
                {
                    return !SyntaxFacts.ContainsDroppedIdentifierCharacters(Name);
                }
                return false;
            }
        }

        internal bool CanBeReferencedByNameIgnoringIllegalCharacters
        {
            get
            {
                if (Kind == SymbolKind.Method)
                {
                    MethodSymbol methodSymbol = (MethodSymbol)this;
                    switch (methodSymbol.MethodKind)
                    {
                        case MethodKind.DelegateInvoke:
                        case MethodKind.Destructor:
                        case MethodKind.Ordinary:
                        case MethodKind.LocalFunction:
                            return true;
                        case MethodKind.PropertyGet:
                        case MethodKind.PropertySet:
                            return ((PropertySymbol)methodSymbol.AssociatedSymbol).CanCallMethodsDirectly();
                        default:
                            return false;
                    }
                }
                return true;
            }
        }

        internal bool Dangerous_IsFromSomeCompilation => DeclaringCompilation != null;

        internal bool HasUseSiteError
        {
            get
            {
                DiagnosticInfo? diagnosticInfo = GetUseSiteInfo().DiagnosticInfo;
                if (diagnosticInfo == null)
                {
                    return false;
                }
                return diagnosticInfo!.Severity == DiagnosticSeverity.Error;
            }
        }

        protected AssemblySymbol PrimaryDependency
        {
            get
            {
                AssemblySymbol containingAssembly = ContainingAssembly;
                if ((object)containingAssembly != null && containingAssembly.CorLibrary == containingAssembly)
                {
                    return null;
                }
                return containingAssembly;
            }
        }

        protected virtual int HighestPriorityUseSiteError => int.MaxValue;

        public virtual bool HasUnsupportedMetadata => false;

        internal ThreeState ObsoleteState
        {
            get
            {
                switch (ObsoleteKind)
                {
                    case ObsoleteAttributeKind.None:
                    case ObsoleteAttributeKind.Experimental:
                        return ThreeState.False;
                    case ObsoleteAttributeKind.Uninitialized:
                        return ThreeState.Unknown;
                    default:
                        return ThreeState.True;
                }
            }
        }

        internal ObsoleteAttributeKind ObsoleteKind => ObsoleteAttributeData?.Kind ?? ObsoleteAttributeKind.None;

        internal abstract ObsoleteAttributeData ObsoleteAttributeData { get; }

        bool ISymbolInternal.IsStatic => IsStatic;

        bool ISymbolInternal.IsVirtual => IsVirtual;

        bool ISymbolInternal.IsOverride => IsOverride;

        bool ISymbolInternal.IsAbstract => IsAbstract;

        Accessibility ISymbolInternal.DeclaredAccessibility => DeclaredAccessibility;

        internal ISymbol ISymbol
        {
            get
            {
                if (_lazyISymbol == null)
                {
                    Interlocked.CompareExchange(ref _lazyISymbol, CreateISymbol(), null);
                }
                return _lazyISymbol;
            }
        }

        public static bool IsSymbolAccessible(Symbol symbol, NamedTypeSymbol within, NamedTypeSymbol throughTypeOpt = null)
        {
            if ((object)symbol == null)
            {
                throw new ArgumentNullException("symbol");
            }
            if ((object)within == null)
            {
                throw new ArgumentNullException("within");
            }
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            return AccessCheck.IsSymbolAccessible(symbol, within, ref useSiteInfo, throughTypeOpt);
        }

        public static bool IsSymbolAccessible(Symbol symbol, AssemblySymbol within)
        {
            if ((object)symbol == null)
            {
                throw new ArgumentNullException("symbol");
            }
            if ((object)within == null)
            {
                throw new ArgumentNullException("within");
            }
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            return AccessCheck.IsSymbolAccessible(symbol, within, ref useSiteInfo);
        }

        IDefinition IReference.AsDefinition(EmitContext context)
        {
            throw ExceptionUtilities.Unreachable;
        }

        ISymbolInternal IReference.GetInternalSymbol()
        {
            return AdaptedSymbol;
        }

        void IReference.Dispatch(MetadataVisitor visitor)
        {
            throw ExceptionUtilities.Unreachable;
        }

        IEnumerable<ICustomAttribute> IReference.GetAttributes(EmitContext context)
        {
            return AdaptedSymbol.GetCustomAttributesToEmit((PEModuleBuilder)context.Module);
        }

        internal Symbol GetCciAdapter()
        {
            return this;
        }

        [Conditional("DEBUG")]
        protected internal void CheckDefinitionInvariant()
        {
        }

        IReference ISymbolInternal.GetCciAdapter()
        {
            return GetCciAdapter();
        }

        internal bool IsDefinitionOrDistinct()
        {
            if (!IsDefinition)
            {
                return !Equals(OriginalDefinition, SymbolEqualityComparer.ConsiderEverything.CompareKind);
            }
            return true;
        }

        internal virtual IEnumerable<CSharpAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder)
        {
            return GetCustomAttributesToEmit(moduleBuilder, emittingAssemblyAttributesInNetModule: false);
        }

        internal IEnumerable<CSharpAttributeData> GetCustomAttributesToEmit(PEModuleBuilder moduleBuilder, bool emittingAssemblyAttributesInNetModule)
        {
            ArrayBuilder<SynthesizedAttributeData> attributes = null;
            ImmutableArray<CSharpAttributeData> attributes2 = GetAttributes();
            AddSynthesizedAttributes(moduleBuilder, ref attributes);
            return GetCustomAttributesToEmit(attributes2, attributes, isReturnType: false, emittingAssemblyAttributesInNetModule);
        }

        internal IEnumerable<CSharpAttributeData> GetCustomAttributesToEmit(ImmutableArray<CSharpAttributeData> userDefined, ArrayBuilder<SynthesizedAttributeData> synthesized, bool isReturnType, bool emittingAssemblyAttributesInNetModule)
        {
            if (userDefined.IsEmpty && synthesized == null)
            {
                return SpecializedCollections.EmptyEnumerable<CSharpAttributeData>();
            }
            return GetCustomAttributesToEmitIterator(userDefined, synthesized, isReturnType, emittingAssemblyAttributesInNetModule);
        }

        private IEnumerable<CSharpAttributeData> GetCustomAttributesToEmitIterator(ImmutableArray<CSharpAttributeData> userDefined, ArrayBuilder<SynthesizedAttributeData> synthesized, bool isReturnType, bool emittingAssemblyAttributesInNetModule)
        {
            if (synthesized != null)
            {
                ArrayBuilder<SynthesizedAttributeData>.Enumerator enumerator = synthesized.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Current;
                }
                synthesized.Free();
            }
            for (int i = 0; i < userDefined.Length; i++)
            {
                CSharpAttributeData cSharpAttributeData = userDefined[i];
                if ((Kind != SymbolKind.Assembly || !((SourceAssemblySymbol)this).IsIndexOfOmittedAssemblyAttribute(i)) && cSharpAttributeData.ShouldEmitAttribute(this, isReturnType, emittingAssemblyAttributesInNetModule))
                {
                    yield return cSharpAttributeData;
                }
            }
        }

        internal virtual void ForceComplete(SourceLocation locationOpt, CancellationToken cancellationToken)
        {
        }

        internal virtual bool HasComplete(CompletionPart part)
        {
            return true;
        }

        ISymbol ISymbolInternal.GetISymbol()
        {
            return ISymbol;
        }

        internal virtual LexicalSortKey GetLexicalSortKey()
        {
            ImmutableArray<Location> locations = Locations;
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            if (locations.Length <= 0)
            {
                return LexicalSortKey.NotInSource;
            }
            return new LexicalSortKey(locations[0], declaringCompilation);
        }

        internal static ImmutableArray<SyntaxReference> GetDeclaringSyntaxReferenceHelper<TNode>(ImmutableArray<Location> locations) where TNode : CSharpSyntaxNode
        {
            if (locations.IsEmpty)
            {
                return ImmutableArray<SyntaxReference>.Empty;
            }
            ArrayBuilder<SyntaxReference> instance = ArrayBuilder<SyntaxReference>.GetInstance();
            ImmutableArray<Location>.Enumerator enumerator = locations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Location location = enumerator.Current;
                if (location == null || !location.IsInSource)
                {
                    continue;
                }
                if (location.SourceSpan.Length != 0)
                {
                    SyntaxToken token = location.SourceTree!.GetRoot().FindToken(location.SourceSpan.Start);
                    if (token.Kind() != 0)
                    {
                        CSharpSyntaxNode cSharpSyntaxNode = token.Parent!.FirstAncestorOrSelf<TNode>();
                        if (cSharpSyntaxNode != null)
                        {
                            instance.Add(cSharpSyntaxNode.GetReference());
                        }
                    }
                    continue;
                }
                SyntaxNode root = location.SourceTree!.GetRoot();
                SyntaxNode syntaxNode = null;
                foreach (SyntaxNode item in root.DescendantNodesAndSelf((SyntaxNode c) => c.Location.SourceSpan.Contains(location.SourceSpan)))
                {
                    if (item is TNode && item.Location.SourceSpan.Contains(location.SourceSpan))
                    {
                        syntaxNode = item;
                    }
                }
                if (syntaxNode != null)
                {
                    instance.Add(syntaxNode.GetReference());
                }
            }
            return instance.ToImmutableAndFree();
        }

        internal virtual void AfterAddingTypeMembersChecks(ConversionsBase conversions, BindingDiagnosticBag diagnostics)
        {
        }

        public static bool operator ==(Symbol left, Symbol right)
        {
            if ((object)right == null)
            {
                return (object)left == null;
            }
            if ((object)left != right)
            {
                return right.Equals(left);
            }
            return true;
        }

        public static bool operator !=(Symbol left, Symbol right)
        {
            if ((object)right == null)
            {
                return (object)left != null;
            }
            if ((object)left != right)
            {
                return !right.Equals(left);
            }
            return false;
        }

        public sealed override bool Equals(object obj)
        {
            return Equals(obj as Symbol, SymbolEqualityComparer.Default.CompareKind);
        }

        bool ISymbolInternal.Equals(ISymbolInternal other, TypeCompareKind compareKind)
        {
            return Equals(other as Symbol, compareKind);
        }

        public virtual bool Equals(Symbol other, TypeCompareKind compareKind)
        {
            return (object)this == other;
        }

        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }

        public static bool Equals(Symbol first, Symbol second, TypeCompareKind compareKind)
        {
            return first?.Equals(second, compareKind) ?? ((object)second == null);
        }

        public sealed override string ToString()
        {
            return ToDisplayString();
        }

        internal abstract TResult Accept<TArgument, TResult>(CSharpSymbolVisitor<TArgument, TResult> visitor, TArgument a);

        internal Symbol()
        {
        }

        internal virtual void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
        }

        internal static void AddSynthesizedAttribute(ref ArrayBuilder<SynthesizedAttributeData> attributes, SynthesizedAttributeData attribute)
        {
            if (attribute != null)
            {
                if (attributes == null)
                {
                    attributes = new ArrayBuilder<SynthesizedAttributeData>(1);
                }
                attributes.Add(attribute);
            }
        }

        internal CharSet? GetEffectiveDefaultMarshallingCharSet()
        {
            return ContainingModule.DefaultMarshallingCharSet;
        }

        internal bool IsFromCompilation(CSharpCompilation compilation)
        {
            return compilation == DeclaringCompilation;
        }

        internal virtual bool IsDefinedInSourceTree(SyntaxTree tree, TextSpan? definedWithinSpan, CancellationToken cancellationToken = default(CancellationToken))
        {
            ImmutableArray<SyntaxReference> declaringSyntaxReferences = DeclaringSyntaxReferences;
            if (IsImplicitlyDeclared && declaringSyntaxReferences.Length == 0)
            {
                return ContainingSymbol.IsDefinedInSourceTree(tree, definedWithinSpan, cancellationToken);
            }
            ImmutableArray<SyntaxReference>.Enumerator enumerator = declaringSyntaxReferences.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SyntaxReference current = enumerator.Current;
                cancellationToken.ThrowIfCancellationRequested();
                if (current.SyntaxTree == tree && (!definedWithinSpan.HasValue || current.Span.IntersectsWith(definedWithinSpan.Value)))
                {
                    return true;
                }
            }
            return false;
        }

        internal static void ForceCompleteMemberByLocation(SourceLocation locationOpt, Symbol member, CancellationToken cancellationToken)
        {
            if (locationOpt == null || member.IsDefinedInSourceTree(locationOpt.SourceTree, locationOpt.SourceSpan, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                member.ForceComplete(locationOpt, cancellationToken);
            }
        }

        public virtual string GetDocumentationCommentId()
        {
            PooledStringBuilder instance = PooledStringBuilder.GetInstance();
            try
            {
                StringBuilder builder = instance.Builder;
                DocumentationCommentIDVisitor.Instance.Visit(this, builder);
                return (builder.Length == 0) ? null : builder.ToString();
            }
            finally
            {
                instance.Free();
            }
        }

        public virtual string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            return "";
        }

        internal virtual string GetDebuggerDisplay()
        {
            return $"{Kind} {ToDisplayString(s_debuggerDisplayFormat)}";
        }

        internal virtual void AddDeclarationDiagnostics(BindingDiagnosticBag diagnostics)
        {
            DiagnosticBag? diagnosticBag = diagnostics.DiagnosticBag;
            if (diagnosticBag == null || diagnosticBag!.IsEmptyWithoutResolution)
            {
                ICollection<AssemblySymbol>? dependenciesBag = diagnostics.DependenciesBag;
                if (dependenciesBag == null || dependenciesBag!.Count <= 0)
                {
                    return;
                }
            }
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            declaringCompilation.AddUsedAssemblies(diagnostics.DependenciesBag);
            DiagnosticBag? diagnosticBag2 = diagnostics.DiagnosticBag;
            if (diagnosticBag2 != null && !diagnosticBag2!.IsEmptyWithoutResolution)
            {
                declaringCompilation.DeclarationDiagnostics.AddRange(diagnostics.DiagnosticBag);
            }
        }

        public virtual UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
        {
            return default(UseSiteInfo<AssemblySymbol>);
        }

        internal bool MergeUseSiteDiagnostics(ref DiagnosticInfo result, DiagnosticInfo info)
        {
            if (info == null)
            {
                return false;
            }
            if (info.Severity == DiagnosticSeverity.Error && (info.Code == HighestPriorityUseSiteError || HighestPriorityUseSiteError == int.MaxValue))
            {
                result = info;
                return true;
            }
            if (result == null || (result.Severity == DiagnosticSeverity.Warning && info.Severity == DiagnosticSeverity.Error))
            {
                result = info;
                return false;
            }
            return false;
        }

        internal bool MergeUseSiteInfo(ref UseSiteInfo<AssemblySymbol> result, UseSiteInfo<AssemblySymbol> info)
        {
            DiagnosticInfo result2 = result.DiagnosticInfo;
            bool result3 = MergeUseSiteDiagnostics(ref result2, info.DiagnosticInfo);
            if (result2 != null && result2.Severity == DiagnosticSeverity.Error)
            {
                result = new UseSiteInfo<AssemblySymbol>(result2);
                return result3;
            }
            ImmutableHashSet<AssemblySymbol> secondaryDependencies = result.SecondaryDependencies;
            AssemblySymbol primaryDependency = result.PrimaryDependency;
            info.MergeDependencies(ref primaryDependency, ref secondaryDependencies);
            result = new UseSiteInfo<AssemblySymbol>(result2, primaryDependency, secondaryDependencies);
            return result3;
        }

        internal static bool ReportUseSiteDiagnostic(DiagnosticInfo info, DiagnosticBag diagnostics, Location location)
        {
            if (info.Code == 1702 || info.Code == 1701 || info.Code == 1705)
            {
                location = NoLocation.Singleton;
            }
            diagnostics.Add(info, location);
            return info.Severity == DiagnosticSeverity.Error;
        }

        internal static bool ReportUseSiteDiagnostic(DiagnosticInfo info, BindingDiagnosticBag diagnostics, Location location)
        {
            return diagnostics.ReportUseSiteDiagnostic(info, location);
        }

        internal bool DeriveUseSiteInfoFromType(ref UseSiteInfo<AssemblySymbol> result, TypeSymbol type)
        {
            UseSiteInfo<AssemblySymbol> info = type.GetUseSiteInfo();
            DiagnosticInfo? diagnosticInfo = info.DiagnosticInfo;
            if (diagnosticInfo != null && diagnosticInfo!.Code == 648)
            {
                GetSymbolSpecificUnsupportedMetadataUseSiteErrorInfo(ref info);
            }
            return MergeUseSiteInfo(ref result, info);
        }

        private void GetSymbolSpecificUnsupportedMetadataUseSiteErrorInfo(ref UseSiteInfo<AssemblySymbol> info)
        {
            SymbolKind kind = Kind;
            if ((uint)(kind - 5) <= 1u || kind == SymbolKind.Method || kind == SymbolKind.Property)
            {
                info = info.AdjustDiagnosticInfo(new CSDiagnosticInfo(ErrorCode.ERR_BindToBogus, this));
            }
        }

        private UseSiteInfo<AssemblySymbol> GetSymbolSpecificUnsupportedMetadataUseSiteErrorInfo()
        {
            UseSiteInfo<AssemblySymbol> info = new UseSiteInfo<AssemblySymbol>(new CSDiagnosticInfo(ErrorCode.ERR_BogusType, string.Empty));
            GetSymbolSpecificUnsupportedMetadataUseSiteErrorInfo(ref info);
            return info;
        }

        internal bool DeriveUseSiteInfoFromType(ref UseSiteInfo<AssemblySymbol> result, TypeWithAnnotations type, AllowedRequiredModifierType allowedRequiredModifierType)
        {
            if (!DeriveUseSiteInfoFromType(ref result, type.Type))
            {
                return DeriveUseSiteInfoFromCustomModifiers(ref result, type.CustomModifiers, allowedRequiredModifierType);
            }
            return true;
        }

        internal bool DeriveUseSiteInfoFromParameter(ref UseSiteInfo<AssemblySymbol> result, ParameterSymbol param)
        {
            if (!DeriveUseSiteInfoFromType(ref result, param.TypeWithAnnotations, AllowedRequiredModifierType.None))
            {
                return DeriveUseSiteInfoFromCustomModifiers(ref result, param.RefCustomModifiers, (this is MethodSymbol methodSymbol && methodSymbol.MethodKind == MethodKind.FunctionPointerSignature) ? (AllowedRequiredModifierType.System_Runtime_InteropServices_InAttribute | AllowedRequiredModifierType.System_Runtime_CompilerServices_OutAttribute) : AllowedRequiredModifierType.System_Runtime_InteropServices_InAttribute);
            }
            return true;
        }

        internal bool DeriveUseSiteInfoFromParameters(ref UseSiteInfo<AssemblySymbol> result, ImmutableArray<ParameterSymbol> parameters)
        {
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                if (DeriveUseSiteInfoFromParameter(ref result, current))
                {
                    return true;
                }
            }
            return false;
        }

        internal bool DeriveUseSiteInfoFromCustomModifiers(ref UseSiteInfo<AssemblySymbol> result, ImmutableArray<CustomModifier> customModifiers, AllowedRequiredModifierType allowedRequiredModifierType)
        {
            AllowedRequiredModifierType allowedRequiredModifierType2 = AllowedRequiredModifierType.None;
            bool flag = true;
            ImmutableArray<CustomModifier>.Enumerator enumerator = customModifiers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CustomModifier current = enumerator.Current;
                NamedTypeSymbol namedTypeSymbol = ((CSharpCustomModifier)current).ModifierSymbol;
                if (flag && !current.IsOptional)
                {
                    AllowedRequiredModifierType allowedRequiredModifierType3 = AllowedRequiredModifierType.None;
                    if ((allowedRequiredModifierType & AllowedRequiredModifierType.System_Runtime_InteropServices_InAttribute) != 0 && namedTypeSymbol.IsWellKnownTypeInAttribute())
                    {
                        allowedRequiredModifierType3 = AllowedRequiredModifierType.System_Runtime_InteropServices_InAttribute;
                    }
                    else if ((allowedRequiredModifierType & AllowedRequiredModifierType.System_Runtime_CompilerServices_Volatile) != 0 && namedTypeSymbol.SpecialType == SpecialType.System_Runtime_CompilerServices_IsVolatile)
                    {
                        allowedRequiredModifierType3 = AllowedRequiredModifierType.System_Runtime_CompilerServices_Volatile;
                    }
                    else if ((allowedRequiredModifierType & AllowedRequiredModifierType.System_Runtime_CompilerServices_IsExternalInit) != 0 && namedTypeSymbol.IsWellKnownTypeIsExternalInit())
                    {
                        allowedRequiredModifierType3 = AllowedRequiredModifierType.System_Runtime_CompilerServices_IsExternalInit;
                    }
                    else if ((allowedRequiredModifierType & AllowedRequiredModifierType.System_Runtime_CompilerServices_OutAttribute) != 0 && namedTypeSymbol.IsWellKnownTypeOutAttribute())
                    {
                        allowedRequiredModifierType3 = AllowedRequiredModifierType.System_Runtime_CompilerServices_OutAttribute;
                    }
                    if (allowedRequiredModifierType3 == AllowedRequiredModifierType.None || (allowedRequiredModifierType3 != allowedRequiredModifierType2 && allowedRequiredModifierType2 != 0))
                    {
                        if (MergeUseSiteInfo(ref result, GetSymbolSpecificUnsupportedMetadataUseSiteErrorInfo()))
                        {
                            return true;
                        }
                        flag = false;
                    }
                    allowedRequiredModifierType2 |= allowedRequiredModifierType3;
                }
                if (namedTypeSymbol.IsUnboundGenericType)
                {
                    namedTypeSymbol = namedTypeSymbol.OriginalDefinition;
                }
                if (DeriveUseSiteInfoFromType(ref result, namedTypeSymbol))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool GetUnificationUseSiteDiagnosticRecursive<T>(ref DiagnosticInfo result, ImmutableArray<T> types, Symbol owner, ref HashSet<TypeSymbol> checkedTypes) where T : TypeSymbol
        {
            ImmutableArray<T>.Enumerator enumerator = types.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.GetUnificationUseSiteDiagnosticRecursive(ref result, owner, ref checkedTypes))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool GetUnificationUseSiteDiagnosticRecursive(ref DiagnosticInfo result, ImmutableArray<TypeWithAnnotations> types, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
        {
            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = types.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.GetUnificationUseSiteDiagnosticRecursive(ref result, owner, ref checkedTypes))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool GetUnificationUseSiteDiagnosticRecursive(ref DiagnosticInfo result, ImmutableArray<CustomModifier> modifiers, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
        {
            ImmutableArray<CustomModifier>.Enumerator enumerator = modifiers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (((CSharpCustomModifier)enumerator.Current).ModifierSymbol.GetUnificationUseSiteDiagnosticRecursive(ref result, owner, ref checkedTypes))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool GetUnificationUseSiteDiagnosticRecursive(ref DiagnosticInfo result, ImmutableArray<ParameterSymbol> parameters, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
        {
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                if (current.TypeWithAnnotations.GetUnificationUseSiteDiagnosticRecursive(ref result, owner, ref checkedTypes) || GetUnificationUseSiteDiagnosticRecursive(ref result, current.RefCustomModifiers, owner, ref checkedTypes))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool GetUnificationUseSiteDiagnosticRecursive(ref DiagnosticInfo result, ImmutableArray<TypeParameterSymbol> typeParameters, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
        {
            ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = typeParameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypeParameterSymbol current = enumerator.Current;
                if (GetUnificationUseSiteDiagnosticRecursive(ref result, current.ConstraintTypesNoUseSiteDiagnostics, owner, ref checkedTypes))
                {
                    return true;
                }
            }
            return false;
        }

        internal bool GetGuidStringDefaultImplementation(out string guidString)
        {
            ImmutableArray<CSharpAttributeData>.Enumerator enumerator = GetAttributes().GetEnumerator();
            while (enumerator.MoveNext())
            {
                CSharpAttributeData current = enumerator.Current;
                if (current.IsTargetAttribute(this, AttributeDescription.GuidAttribute) && current.TryGetGuidAttributeValue(out guidString))
                {
                    return true;
                }
            }
            guidString = null;
            return false;
        }

        public string ToDisplayString(SymbolDisplayFormat format = null)
        {
            return SymbolDisplay.ToDisplayString(ISymbol, format);
        }

        public ImmutableArray<SymbolDisplayPart> ToDisplayParts(SymbolDisplayFormat format = null)
        {
            return SymbolDisplay.ToDisplayParts(ISymbol, format);
        }

        public string ToMinimalDisplayString(SemanticModel semanticModel, int position, SymbolDisplayFormat format = null)
        {
            return SymbolDisplay.ToMinimalDisplayString(ISymbol, semanticModel, position, format);
        }

        public ImmutableArray<SymbolDisplayPart> ToMinimalDisplayParts(SemanticModel semanticModel, int position, SymbolDisplayFormat format = null)
        {
            return SymbolDisplay.ToMinimalDisplayParts(ISymbol, semanticModel, position, format);
        }

        internal static void ReportErrorIfHasConstraints(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, DiagnosticBag diagnostics)
        {
            if (constraintClauses.Count > 0)
            {
                diagnostics.Add(ErrorCode.ERR_ConstraintOnlyAllowedOnGenericDecl, constraintClauses[0].WhereKeyword.GetLocation());
            }
        }

        internal static void CheckForBlockAndExpressionBody(CSharpSyntaxNode block, CSharpSyntaxNode expression, CSharpSyntaxNode syntax, BindingDiagnosticBag diagnostics)
        {
            if (block != null && expression != null)
            {
                diagnostics.Add(ErrorCode.ERR_BlockBodyAndExpressionBody, syntax.GetLocation());
            }
        }

        internal bool ReportExplicitUseOfReservedAttributes(in DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments, ReservedAttributes reserved)
        {
            CSharpAttributeData attribute2 = arguments.Attribute;
            BindingDiagnosticBag diagnostics = (BindingDiagnosticBag)arguments.Diagnostics;
            if ((reserved & ReservedAttributes.DynamicAttribute) != 0 && attribute2.IsTargetAttribute(this, AttributeDescription.DynamicAttribute))
            {
                diagnostics.Add(ErrorCode.ERR_ExplicitDynamicAttr, arguments.AttributeSyntaxOpt!.Location);
            }
            else if (((reserved & ReservedAttributes.IsReadOnlyAttribute) == 0 || !reportExplicitUseOfReservedAttribute(attribute2, in arguments, in AttributeDescription.IsReadOnlyAttribute)) && ((reserved & ReservedAttributes.IsUnmanagedAttribute) == 0 || !reportExplicitUseOfReservedAttribute(attribute2, in arguments, in AttributeDescription.IsUnmanagedAttribute)) && ((reserved & ReservedAttributes.IsByRefLikeAttribute) == 0 || !reportExplicitUseOfReservedAttribute(attribute2, in arguments, in AttributeDescription.IsByRefLikeAttribute)))
            {
                if ((reserved & ReservedAttributes.TupleElementNamesAttribute) != 0 && attribute2.IsTargetAttribute(this, AttributeDescription.TupleElementNamesAttribute))
                {
                    diagnostics.Add(ErrorCode.ERR_ExplicitTupleElementNamesAttribute, arguments.AttributeSyntaxOpt!.Location);
                }
                else if ((reserved & ReservedAttributes.NullableAttribute) != 0 && attribute2.IsTargetAttribute(this, AttributeDescription.NullableAttribute))
                {
                    diagnostics.Add(ErrorCode.ERR_ExplicitNullableAttribute, arguments.AttributeSyntaxOpt!.Location);
                }
                else if (((reserved & ReservedAttributes.NullableContextAttribute) == 0 || !reportExplicitUseOfReservedAttribute(attribute2, in arguments, in AttributeDescription.NullableContextAttribute)) && ((reserved & ReservedAttributes.NullablePublicOnlyAttribute) == 0 || !reportExplicitUseOfReservedAttribute(attribute2, in arguments, in AttributeDescription.NullablePublicOnlyAttribute)) && ((reserved & ReservedAttributes.NativeIntegerAttribute) == 0 || !reportExplicitUseOfReservedAttribute(attribute2, in arguments, in AttributeDescription.NativeIntegerAttribute)))
                {
                    if ((reserved & ReservedAttributes.CaseSensitiveExtensionAttribute) == 0 || !attribute2.IsTargetAttribute(this, AttributeDescription.CaseSensitiveExtensionAttribute))
                    {
                        return false;
                    }
                    diagnostics.Add(ErrorCode.ERR_ExplicitExtension, arguments.AttributeSyntaxOpt!.Location);
                }
            }
            return true;
            bool reportExplicitUseOfReservedAttribute(CSharpAttributeData attribute, in DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments, in AttributeDescription attributeDescription)
            {
                if (attribute.IsTargetAttribute(this, attributeDescription))
                {
                    diagnostics.Add(ErrorCode.ERR_ExplicitReservedAttr, arguments.AttributeSyntaxOpt!.Location, attributeDescription.FullName);
                    return true;
                }
                return false;
            }
        }

        internal virtual byte? GetNullableContextValue()
        {
            return GetLocalNullableContextValue() ?? ContainingSymbol?.GetNullableContextValue();
        }

        internal virtual byte? GetLocalNullableContextValue()
        {
            return null;
        }

        internal void GetCommonNullableValues(CSharpCompilation compilation, ref MostCommonNullableValueBuilder builder)
        {
            switch (Kind)
            {
                case SymbolKind.NamedType:
                    if (compilation.ShouldEmitNullableAttributes(this))
                    {
                        builder.AddValue(GetLocalNullableContextValue());
                    }
                    break;
                case SymbolKind.Event:
                    if (compilation.ShouldEmitNullableAttributes(this))
                    {
                        builder.AddValue(((EventSymbol)this).TypeWithAnnotations);
                    }
                    break;
                case SymbolKind.Field:
                    {
                        FieldSymbol fieldSymbol = (FieldSymbol)this;
                        if (fieldSymbol is TupleElementFieldSymbol tupleElementFieldSymbol)
                        {
                            fieldSymbol = tupleElementFieldSymbol.TupleUnderlyingField;
                        }
                        if (compilation.ShouldEmitNullableAttributes(fieldSymbol))
                        {
                            builder.AddValue(fieldSymbol.TypeWithAnnotations);
                        }
                        break;
                    }
                case SymbolKind.Method:
                    if (compilation.ShouldEmitNullableAttributes(this))
                    {
                        builder.AddValue(GetLocalNullableContextValue());
                    }
                    break;
                case SymbolKind.Property:
                    if (compilation.ShouldEmitNullableAttributes(this))
                    {
                        builder.AddValue(((PropertySymbol)this).TypeWithAnnotations);
                    }
                    break;
                case SymbolKind.Parameter:
                    builder.AddValue(((ParameterSymbol)this).TypeWithAnnotations);
                    break;
                case SymbolKind.TypeParameter:
                    if (this is SourceTypeParameterSymbolBase sourceTypeParameterSymbolBase)
                    {
                        builder.AddValue(sourceTypeParameterSymbolBase.GetSynthesizedNullableAttributeValue());
                        ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = sourceTypeParameterSymbolBase.ConstraintTypesNoUseSiteDiagnostics.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            TypeWithAnnotations current = enumerator.Current;
                            builder.AddValue(current);
                        }
                    }
                    break;
                case SymbolKind.Label:
                case SymbolKind.Local:
                case SymbolKind.NetModule:
                case SymbolKind.Namespace:
                case SymbolKind.PointerType:
                case SymbolKind.RangeVariable:
                    break;
            }
        }

        internal bool ShouldEmitNullableContextValue(out byte value)
        {
            byte? localNullableContextValue = GetLocalNullableContextValue();
            if (!localNullableContextValue.HasValue)
            {
                value = 0;
                return false;
            }
            value = localNullableContextValue.GetValueOrDefault();
            byte valueOrDefault = (ContainingSymbol?.GetNullableContextValue()).GetValueOrDefault();
            return value != valueOrDefault;
        }

        internal static bool IsCaptured(Symbol variable, SourceMethodSymbol containingSymbol)
        {
            switch (variable.Kind)
            {
                case SymbolKind.Event:
                case SymbolKind.Field:
                case SymbolKind.Property:
                case SymbolKind.RangeVariable:
                    return false;
                case SymbolKind.Local:
                    if (((LocalSymbol)variable).IsConst)
                    {
                        return false;
                    }
                    break;
                case SymbolKind.Method:
                    if (variable is LocalFunctionSymbol localFunctionSymbol)
                    {
                        if (localFunctionSymbol.IsStatic)
                        {
                            return false;
                        }
                        break;
                    }
                    throw ExceptionUtilities.UnexpectedValue(variable);
                default:
                    throw ExceptionUtilities.UnexpectedValue(variable.Kind);
                case SymbolKind.Parameter:
                    break;
            }
            Symbol containingSymbol2 = variable.ContainingSymbol;
            while ((object)containingSymbol2 != null)
            {
                if ((object)containingSymbol2 == containingSymbol)
                {
                    return false;
                }
                containingSymbol2 = containingSymbol2.ContainingSymbol;
            }
            return true;
        }

        public abstract void Accept(CSharpSymbolVisitor visitor);

        public abstract TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor);

        string IFormattable.ToString(string format, IFormatProvider formatProvider)
        {
            return ToString();
        }

        protected abstract ISymbol CreateISymbol();

        public virtual ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return ImmutableArray<CSharpAttributeData>.Empty;
        }

        internal virtual AttributeTargets GetAttributeTarget()
        {
            switch (Kind)
            {
                case SymbolKind.Assembly:
                    return AttributeTargets.Assembly;
                case SymbolKind.Field:
                    return AttributeTargets.Field;
                case SymbolKind.Method:
                    {
                        MethodKind methodKind = ((MethodSymbol)this).MethodKind;
                        if (methodKind == MethodKind.Constructor || methodKind == MethodKind.StaticConstructor)
                        {
                            return AttributeTargets.Constructor;
                        }
                        return AttributeTargets.Method;
                    }
                case SymbolKind.NamedType:
                    {
                        NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)this;
                        switch (namedTypeSymbol.TypeKind)
                        {
                            case TypeKind.Class:
                                return AttributeTargets.Class;
                            case TypeKind.Delegate:
                                return AttributeTargets.Delegate;
                            case TypeKind.Enum:
                                return AttributeTargets.Enum;
                            case TypeKind.Interface:
                                return AttributeTargets.Interface;
                            case TypeKind.Struct:
                                return AttributeTargets.Struct;
                            case TypeKind.TypeParameter:
                                return AttributeTargets.GenericParameter;
                            case TypeKind.Submission:
                                throw ExceptionUtilities.UnexpectedValue(namedTypeSymbol.TypeKind);
                        }
                        break;
                    }
                case SymbolKind.NetModule:
                    return AttributeTargets.Module;
                case SymbolKind.Parameter:
                    return AttributeTargets.Parameter;
                case SymbolKind.Property:
                    return AttributeTargets.Property;
                case SymbolKind.Event:
                    return AttributeTargets.Event;
                case SymbolKind.TypeParameter:
                    return AttributeTargets.GenericParameter;
            }
            return 0;
        }

        internal virtual void EarlyDecodeWellKnownAttributeType(NamedTypeSymbol attributeType, AttributeSyntax attributeSyntax)
        {
        }

        internal virtual void PostEarlyDecodeWellKnownAttributeTypes()
        {
        }

        internal virtual CSharpAttributeData EarlyDecodeWellKnownAttribute(ref EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments)
        {
            return null;
        }

        internal static bool EarlyDecodeDeprecatedOrExperimentalOrObsoleteAttribute(ref EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments, out CSharpAttributeData attributeData, out ObsoleteAttributeData obsoleteData)
        {
            NamedTypeSymbol attributeType = arguments.AttributeType;
            AttributeSyntax attributeSyntax = arguments.AttributeSyntax;
            ObsoleteAttributeKind kind;
            if (CSharpAttributeData.IsTargetEarlyAttribute(attributeType, attributeSyntax, AttributeDescription.ObsoleteAttribute))
            {
                kind = ObsoleteAttributeKind.Obsolete;
            }
            else if (CSharpAttributeData.IsTargetEarlyAttribute(attributeType, attributeSyntax, AttributeDescription.DeprecatedAttribute))
            {
                kind = ObsoleteAttributeKind.Deprecated;
            }
            else
            {
                if (!CSharpAttributeData.IsTargetEarlyAttribute(attributeType, attributeSyntax, AttributeDescription.ExperimentalAttribute))
                {
                    obsoleteData = null;
                    attributeData = null;
                    return false;
                }
                kind = ObsoleteAttributeKind.Experimental;
            }
            attributeData = arguments.Binder.GetAttribute(attributeSyntax, attributeType, out var generatedDiagnostics);
            if (!attributeData.HasErrors)
            {
                obsoleteData = attributeData.DecodeObsoleteAttribute(kind);
                if (generatedDiagnostics)
                {
                    attributeData = null;
                }
            }
            else
            {
                obsoleteData = null;
                attributeData = null;
            }
            return true;
        }

        internal virtual void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments)
        {
        }

        internal virtual void PostDecodeWellKnownAttributes(ImmutableArray<CSharpAttributeData> boundAttributes, ImmutableArray<AttributeSyntax> allAttributeSyntaxNodes, BindingDiagnosticBag diagnostics, AttributeLocation symbolPart, WellKnownAttributeData decodedData)
        {
        }

        internal bool LoadAndValidateAttributes(OneOrMany<SyntaxList<AttributeListSyntax>> attributesSyntaxLists, ref CustomAttributesBag<CSharpAttributeData> lazyCustomAttributesBag, AttributeLocation symbolPart = AttributeLocation.None, bool earlyDecodingOnly = false, Binder binderOpt = null, Func<AttributeSyntax, bool> attributeMatchesOpt = null)
        {
            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            ImmutableArray<AttributeSyntax> attributesToBind = GetAttributesToBind(attributesSyntaxLists, symbolPart, instance, declaringCompilation, attributeMatchesOpt, binderOpt, out ImmutableArray<Binder> binders);
            ImmutableArray<CSharpAttributeData> immutableArray2;
            WellKnownAttributeData wellKnownAttributeData;
            if (attributesToBind.Any())
            {
                if (lazyCustomAttributesBag == null)
                {
                    Interlocked.CompareExchange(ref lazyCustomAttributesBag, new CustomAttributesBag<CSharpAttributeData>(), null);
                }
                int length = attributesToBind.Length;
                NamedTypeSymbol[] array = new NamedTypeSymbol[length];
                Binder.BindAttributeTypes(binders, attributesToBind, this, array, instance);
                ImmutableArray<NamedTypeSymbol> immutableArray = array.AsImmutableOrNull();
                EarlyDecodeWellKnownAttributeTypes(immutableArray, attributesToBind);
                PostEarlyDecodeWellKnownAttributeTypes();
                CSharpAttributeData[] array2 = new CSharpAttributeData[length];
                EarlyWellKnownAttributeData earlyDecodedWellKnownAttributeData = EarlyDecodeWellKnownAttributes(binders, immutableArray, attributesToBind, symbolPart, array2);
                lazyCustomAttributesBag.SetEarlyDecodedWellKnownAttributeData(earlyDecodedWellKnownAttributeData);
                if (earlyDecodingOnly)
                {
                    instance.Free();
                    return false;
                }
                Binder.GetAttributes(binders, attributesToBind, immutableArray, array2, instance);
                immutableArray2 = array2.AsImmutableOrNull();
                wellKnownAttributeData = ValidateAttributeUsageAndDecodeWellKnownAttributes(binders, attributesToBind, immutableArray2, instance, symbolPart);
                lazyCustomAttributesBag.SetDecodedWellKnownAttributeData(wellKnownAttributeData);
            }
            else
            {
                if (earlyDecodingOnly)
                {
                    instance.Free();
                    return false;
                }
                immutableArray2 = ImmutableArray<CSharpAttributeData>.Empty;
                wellKnownAttributeData = null;
                Interlocked.CompareExchange(ref lazyCustomAttributesBag, CustomAttributesBag<CSharpAttributeData>.WithEmptyData(), null);
                PostEarlyDecodeWellKnownAttributeTypes();
            }
            PostDecodeWellKnownAttributes(immutableArray2, attributesToBind, instance, symbolPart, wellKnownAttributeData);
            bool result = false;
            if (lazyCustomAttributesBag.SetAttributes(immutableArray2))
            {
                if (attributeMatchesOpt == null)
                {
                    RecordPresenceOfBadAttributes(immutableArray2);
                    AddDeclarationDiagnostics(instance);
                }
                result = true;
                if (lazyCustomAttributesBag.IsEmpty)
                {
                    lazyCustomAttributesBag = CustomAttributesBag<CSharpAttributeData>.Empty;
                }
            }
            instance.Free();
            return result;
        }

        private void RecordPresenceOfBadAttributes(ImmutableArray<CSharpAttributeData> boundAttributes)
        {
            ImmutableArray<CSharpAttributeData>.Enumerator enumerator = boundAttributes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.HasErrors)
                {
                    ((SourceModuleSymbol)DeclaringCompilation.SourceModule).RecordPresenceOfBadAttributes();
                    break;
                }
            }
        }

        private ImmutableArray<AttributeSyntax> GetAttributesToBind(OneOrMany<SyntaxList<AttributeListSyntax>> attributeDeclarationSyntaxLists, AttributeLocation symbolPart, BindingDiagnosticBag diagnostics, CSharpCompilation compilation, Func<AttributeSyntax, bool> attributeMatchesOpt, Binder rootBinderOpt, out ImmutableArray<Binder> binders)
        {
            IAttributeTargetSymbol attributeTarget = (IAttributeTargetSymbol)this;
            ArrayBuilder<AttributeSyntax> arrayBuilder = null;
            ArrayBuilder<Binder> arrayBuilder2 = null;
            int num = 0;
            for (int i = 0; i < attributeDeclarationSyntaxLists.Count; i++)
            {
                SyntaxList<AttributeListSyntax> syntaxList = attributeDeclarationSyntaxLists[i];
                if (!syntaxList.Any())
                {
                    continue;
                }
                int num2 = num;
                SyntaxList<AttributeListSyntax>.Enumerator enumerator = syntaxList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    AttributeListSyntax current = enumerator.Current;
                    if (!MatchAttributeTarget(attributeTarget, symbolPart, current.Target, diagnostics))
                    {
                        continue;
                    }
                    if (arrayBuilder == null)
                    {
                        arrayBuilder = new ArrayBuilder<AttributeSyntax>();
                        arrayBuilder2 = new ArrayBuilder<Binder>();
                    }
                    SeparatedSyntaxList<AttributeSyntax> attributes = current.Attributes;
                    if (attributeMatchesOpt == null)
                    {
                        arrayBuilder.AddRange(attributes);
                        num += attributes.Count;
                        continue;
                    }
                    SeparatedSyntaxList<AttributeSyntax>.Enumerator enumerator2 = attributes.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        AttributeSyntax current2 = enumerator2.Current;
                        if (attributeMatchesOpt(current2))
                        {
                            arrayBuilder.Add(current2);
                            num++;
                        }
                    }
                }
                if (num != num2)
                {
                    SyntaxTree syntaxTree = syntaxList.Node!.SyntaxTree;
                    Binder enclosing = rootBinderOpt ?? compilation.GetBinderFactory(syntaxTree).GetBinder(syntaxList.Node);
                    enclosing = new ContextualAttributeBinder(enclosing, this);
                    for (int j = 0; j < num - num2; j++)
                    {
                        arrayBuilder2.Add(enclosing);
                    }
                }
            }
            if (arrayBuilder != null)
            {
                binders = arrayBuilder2.ToImmutableAndFree();
                return arrayBuilder.ToImmutableAndFree();
            }
            binders = ImmutableArray<Binder>.Empty;
            return ImmutableArray<AttributeSyntax>.Empty;
        }

        private static bool MatchAttributeTarget(IAttributeTargetSymbol attributeTarget, AttributeLocation symbolPart, AttributeTargetSpecifierSyntax targetOpt, BindingDiagnosticBag diagnostics)
        {
            IAttributeTargetSymbol attributesOwner = attributeTarget.AttributesOwner;
            bool flag = symbolPart == AttributeLocation.None && attributesOwner == attributeTarget;
            if (targetOpt == null)
            {
                return flag;
            }
            AttributeLocation allowedAttributeLocations = attributesOwner.AllowedAttributeLocations;
            AttributeLocation attributeLocation = targetOpt.GetAttributeLocation();
            if (attributeLocation == AttributeLocation.None)
            {
                if (flag)
                {
                    diagnostics.Add(ErrorCode.WRN_InvalidAttributeLocation, targetOpt.Identifier.GetLocation(), targetOpt.Identifier.ValueText, allowedAttributeLocations.ToDisplayString());
                }
                return false;
            }
            if ((attributeLocation & allowedAttributeLocations) == 0)
            {
                if (flag)
                {
                    if (allowedAttributeLocations == AttributeLocation.None)
                    {
                        AttributeLocation defaultAttributeLocation = attributeTarget.DefaultAttributeLocation;
                        if ((uint)(defaultAttributeLocation - 1) > 1u)
                        {
                            throw ExceptionUtilities.UnexpectedValue(attributeTarget.DefaultAttributeLocation);
                        }
                        diagnostics.Add(ErrorCode.ERR_GlobalAttributesNotAllowed, targetOpt.Identifier.GetLocation());
                    }
                    else
                    {
                        diagnostics.Add(ErrorCode.WRN_AttributeLocationOnBadDeclaration, targetOpt.Identifier.GetLocation(), targetOpt.Identifier.ToString(), allowedAttributeLocations.ToDisplayString());
                    }
                }
                return false;
            }
            if (symbolPart == AttributeLocation.None)
            {
                return attributeLocation == attributeTarget.DefaultAttributeLocation;
            }
            return attributeLocation == symbolPart;
        }

        internal EarlyWellKnownAttributeData EarlyDecodeWellKnownAttributes(ImmutableArray<Binder> binders, ImmutableArray<NamedTypeSymbol> boundAttributeTypes, ImmutableArray<AttributeSyntax> attributesToBind, AttributeLocation symbolPart, CSharpAttributeData[] boundAttributesBuilder)
        {
            EarlyWellKnownAttributeBinder earlyWellKnownAttributeBinder = new EarlyWellKnownAttributeBinder(binders[0]);
            EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments = default(EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation>);
            arguments.SymbolPart = symbolPart;
            for (int i = 0; i < boundAttributeTypes.Length; i++)
            {
                NamedTypeSymbol namedTypeSymbol = boundAttributeTypes[i];
                if (!namedTypeSymbol.IsErrorType())
                {
                    if (binders[i] != earlyWellKnownAttributeBinder.Next)
                    {
                        earlyWellKnownAttributeBinder = new EarlyWellKnownAttributeBinder(binders[i]);
                    }
                    arguments.Binder = earlyWellKnownAttributeBinder;
                    arguments.AttributeType = namedTypeSymbol;
                    arguments.AttributeSyntax = attributesToBind[i];
                    CSharpAttributeData cSharpAttributeData = (boundAttributesBuilder[i] = EarlyDecodeWellKnownAttribute(ref arguments));
                }
            }
            if (!arguments.HasDecodedData)
            {
                return null;
            }
            return arguments.DecodedData;
        }

        private void EarlyDecodeWellKnownAttributeTypes(ImmutableArray<NamedTypeSymbol> attributeTypes, ImmutableArray<AttributeSyntax> attributeSyntaxList)
        {
            for (int i = 0; i < attributeTypes.Length; i++)
            {
                NamedTypeSymbol namedTypeSymbol = attributeTypes[i];
                if (!namedTypeSymbol.IsErrorType())
                {
                    EarlyDecodeWellKnownAttributeType(namedTypeSymbol, attributeSyntaxList[i]);
                }
            }
        }

        private WellKnownAttributeData ValidateAttributeUsageAndDecodeWellKnownAttributes(ImmutableArray<Binder> binders, ImmutableArray<AttributeSyntax> attributeSyntaxList, ImmutableArray<CSharpAttributeData> boundAttributes, BindingDiagnosticBag diagnostics, AttributeLocation symbolPart)
        {
            int length = boundAttributes.Length;
            HashSet<NamedTypeSymbol> uniqueAttributeTypes = new HashSet<NamedTypeSymbol>();
            DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation> arguments = default(DecodeWellKnownAttributeArguments<AttributeSyntax, CSharpAttributeData, AttributeLocation>);
            arguments.Diagnostics = diagnostics;
            arguments.AttributesCount = length;
            arguments.SymbolPart = symbolPart;
            for (int i = 0; i < length; i++)
            {
                CSharpAttributeData cSharpAttributeData = boundAttributes[i];
                AttributeSyntax attributeSyntax = attributeSyntaxList[i];
                Binder binder = binders[i];
                if (!cSharpAttributeData.HasErrors && ValidateAttributeUsage(cSharpAttributeData, attributeSyntax, binder.Compilation, symbolPart, diagnostics, uniqueAttributeTypes))
                {
                    arguments.Attribute = cSharpAttributeData;
                    arguments.AttributeSyntaxOpt = attributeSyntax;
                    arguments.Index = i;
                    DecodeWellKnownAttribute(ref arguments);
                }
            }
            if (!arguments.HasDecodedData)
            {
                return null;
            }
            return arguments.DecodedData;
        }

        private bool ValidateAttributeUsage(CSharpAttributeData attribute, AttributeSyntax node, CSharpCompilation compilation, AttributeLocation symbolPart, BindingDiagnosticBag diagnostics, HashSet<NamedTypeSymbol> uniqueAttributeTypes)
        {
            NamedTypeSymbol attributeClass = attribute.AttributeClass;
            AttributeUsageInfo attributeUsageInfo = attributeClass.GetAttributeUsageInfo();
            if (!uniqueAttributeTypes.Add(attributeClass) && !attributeUsageInfo.AllowMultiple)
            {
                diagnostics.Add(ErrorCode.ERR_DuplicateAttribute, node.Name.Location, node.GetErrorDisplayName());
                return false;
            }
            AttributeTargets attributeTargets = ((symbolPart != AttributeLocation.Return) ? GetAttributeTarget() : AttributeTargets.ReturnValue);
            if ((attributeTargets & attributeUsageInfo.ValidTargets) == 0)
            {
                diagnostics.Add(ErrorCode.ERR_AttributeOnBadSymbolType, node.Name.Location, node.GetErrorDisplayName(), attributeUsageInfo.GetValidTargetsErrorArgument());
                return false;
            }
            if (attribute.IsSecurityAttribute(compilation))
            {
                SymbolKind kind = Kind;
                if (kind != SymbolKind.Assembly && kind != SymbolKind.Method && kind != SymbolKind.NamedType)
                {
                    diagnostics.Add(ErrorCode.ERR_SecurityAttributeInvalidTarget, node.Name.Location, node.GetErrorDisplayName());
                    return false;
                }
            }
            return true;
        }

        internal void ForceCompleteObsoleteAttribute()
        {
            if (ObsoleteState == ThreeState.Unknown)
            {
                GetAttributes();
            }
        }
    }
}
