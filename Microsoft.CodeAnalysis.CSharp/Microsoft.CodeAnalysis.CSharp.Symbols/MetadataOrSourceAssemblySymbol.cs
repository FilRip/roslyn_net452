using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;

using Microsoft.CodeAnalysis.RuntimeMembers;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class MetadataOrSourceAssemblySymbol : NonMissingAssemblySymbol
    {
        private NamedTypeSymbol[] _lazySpecialTypes;

        private int _cachedSpecialTypes;

        private NativeIntegerTypeSymbol[] _lazyNativeIntegerTypes;

        private ICollection<string> _lazyTypeNames;

        private ICollection<string> _lazyNamespaceNames;

        private Symbol[] _lazySpecialTypeMembers;

        private ConcurrentDictionary<AssemblySymbol, IVTConclusion> _assembliesToWhichInternalAccessHasBeenAnalyzed;

        internal override bool KeepLookingForDeclaredSpecialTypes
        {
            get
            {
                if ((object)base.CorLibrary == this)
                {
                    return _cachedSpecialTypes < 45;
                }
                return false;
            }
        }

        public override ICollection<string> TypeNames
        {
            get
            {
                if (_lazyTypeNames == null)
                {
                    Interlocked.CompareExchange(ref _lazyTypeNames, UnionCollection<string>.Create(Modules, (ModuleSymbol m) => m.TypeNames), null);
                }
                return _lazyTypeNames;
            }
        }

        public override ICollection<string> NamespaceNames
        {
            get
            {
                if (_lazyNamespaceNames == null)
                {
                    Interlocked.CompareExchange(ref _lazyNamespaceNames, UnionCollection<string>.Create(Modules, (ModuleSymbol m) => m.NamespaceNames), null);
                }
                return _lazyNamespaceNames;
            }
        }

        private ConcurrentDictionary<AssemblySymbol, IVTConclusion> AssembliesToWhichInternalAccessHasBeenDetermined
        {
            get
            {
                if (_assembliesToWhichInternalAccessHasBeenAnalyzed == null)
                {
                    Interlocked.CompareExchange(ref _assembliesToWhichInternalAccessHasBeenAnalyzed, new ConcurrentDictionary<AssemblySymbol, IVTConclusion>(), null);
                }
                return _assembliesToWhichInternalAccessHasBeenAnalyzed;
            }
        }

        internal sealed override NamedTypeSymbol GetDeclaredSpecialType(SpecialType type)
        {
            if (_lazySpecialTypes == null || (object)_lazySpecialTypes[(int)type] == null)
            {
                MetadataTypeName emittedName = MetadataTypeName.FromFullName(type.GetMetadataName(), useCLSCompliantNameArityEncoding: true);
                ModuleSymbol moduleSymbol = Modules[0];
                NamedTypeSymbol namedTypeSymbol = moduleSymbol.LookupTopLevelMetadataType(ref emittedName);
                if (namedTypeSymbol.Kind != SymbolKind.ErrorType && namedTypeSymbol.DeclaredAccessibility != Accessibility.Public)
                {
                    namedTypeSymbol = new MissingMetadataTypeSymbol.TopLevel(moduleSymbol, ref emittedName, type);
                }
                RegisterDeclaredSpecialType(namedTypeSymbol);
            }
            return _lazySpecialTypes[(int)type];
        }

        internal sealed override void RegisterDeclaredSpecialType(NamedTypeSymbol corType)
        {
            SpecialType specialType = corType.SpecialType;
            if (_lazySpecialTypes == null)
            {
                Interlocked.CompareExchange(ref _lazySpecialTypes, new NamedTypeSymbol[46], null);
            }
            if ((object)Interlocked.CompareExchange(ref _lazySpecialTypes[(int)specialType], corType, null) == null)
            {
                Interlocked.Increment(ref _cachedSpecialTypes);
            }
        }

        internal sealed override NamedTypeSymbol GetNativeIntegerType(NamedTypeSymbol underlyingType)
        {
            if (_lazyNativeIntegerTypes == null)
            {
                Interlocked.CompareExchange(ref _lazyNativeIntegerTypes, new NativeIntegerTypeSymbol[2], null);
            }
            int num = underlyingType.SpecialType switch
            {
                SpecialType.System_IntPtr => 0,
                SpecialType.System_UIntPtr => 1,
                _ => throw ExceptionUtilities.UnexpectedValue(underlyingType.SpecialType),
            };
            if ((object)_lazyNativeIntegerTypes[num] == null)
            {
                Interlocked.CompareExchange(ref _lazyNativeIntegerTypes[num], new NativeIntegerTypeSymbol(underlyingType), null);
            }
            return _lazyNativeIntegerTypes[num];
        }

        internal override Symbol GetDeclaredSpecialTypeMember(SpecialMember member)
        {
            if (_lazySpecialTypeMembers == null || (object)_lazySpecialTypeMembers[(int)member] == ErrorTypeSymbol.UnknownResultType)
            {
                if (_lazySpecialTypeMembers == null)
                {
                    Symbol[] array = new Symbol[124];
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = ErrorTypeSymbol.UnknownResultType;
                    }
                    Interlocked.CompareExchange(ref _lazySpecialTypeMembers, array, null);
                }
                MemberDescriptor descriptor = SpecialMembers.GetDescriptor(member);
                NamedTypeSymbol declaredSpecialType = GetDeclaredSpecialType((SpecialType)descriptor.DeclaringTypeId);
                Symbol value = null;
                if (!declaredSpecialType.IsErrorType())
                {
                    value = CSharpCompilation.GetRuntimeMember(declaredSpecialType, in descriptor, CSharpCompilation.SpecialMembersSignatureComparer.Instance, null);
                }
                Interlocked.CompareExchange(ref _lazySpecialTypeMembers[(int)member], value, ErrorTypeSymbol.UnknownResultType);
            }
            return _lazySpecialTypeMembers[(int)member];
        }

        protected IVTConclusion MakeFinalIVTDetermination(AssemblySymbol potentialGiverOfAccess)
        {
            if (AssembliesToWhichInternalAccessHasBeenDetermined.TryGetValue(potentialGiverOfAccess, out var value))
            {
                return value;
            }
            value = IVTConclusion.NoRelationshipClaimed;
            IEnumerable<ImmutableArray<byte>> internalsVisibleToPublicKeys = potentialGiverOfAccess.GetInternalsVisibleToPublicKeys(Name);
            if (internalsVisibleToPublicKeys.Any() && IsNetModule())
            {
                return IVTConclusion.Match;
            }
            foreach (ImmutableArray<byte> item in internalsVisibleToPublicKeys)
            {
                value = potentialGiverOfAccess.Identity.PerformIVTCheck(PublicKey, item);
                if (value == IVTConclusion.Match || value == IVTConclusion.OneSignedOneNot)
                {
                    break;
                }
            }
            AssembliesToWhichInternalAccessHasBeenDetermined.TryAdd(potentialGiverOfAccess, value);
            return value;
        }

        internal virtual bool IsNetModule()
        {
            return false;
        }
    }
}
