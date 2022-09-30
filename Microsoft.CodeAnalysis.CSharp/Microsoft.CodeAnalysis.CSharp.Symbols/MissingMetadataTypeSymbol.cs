using System;
using System.Collections.Immutable;
using System.Threading;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal abstract class MissingMetadataTypeSymbol : ErrorTypeSymbol
    {
        internal sealed class TopLevel : MissingMetadataTypeSymbol
        {
            private readonly string _namespaceName;

            private readonly ModuleSymbol _containingModule;

            private readonly bool _isNativeInt;

            private DiagnosticInfo? _lazyErrorInfo;

            private NamespaceSymbol? _lazyContainingNamespace;

            private int _lazyTypeId;

            public string NamespaceName => _namespaceName;

            internal override ModuleSymbol ContainingModule => _containingModule;

            public override AssemblySymbol ContainingAssembly => _containingModule.ContainingAssembly;

            public override Symbol ContainingSymbol
            {
                get
                {
                    if ((object)_lazyContainingNamespace == null)
                    {
                        NamespaceSymbol namespaceSymbol = _containingModule.GlobalNamespace;
                        if (_namespaceName.Length > 0)
                        {
                            ImmutableArray<string> immutableArray = MetadataHelpers.SplitQualifiedName(_namespaceName);
                            int i;
                            for (i = 0; i < immutableArray.Length; i++)
                            {
                                NamespaceSymbol namespaceSymbol2 = null;
                                ImmutableArray<Symbol>.Enumerator enumerator = namespaceSymbol.GetMembers(immutableArray[i]).GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    NamespaceOrTypeSymbol namespaceOrTypeSymbol = (NamespaceOrTypeSymbol)enumerator.Current;
                                    if (namespaceOrTypeSymbol.Kind == SymbolKind.Namespace)
                                    {
                                        namespaceSymbol2 = (NamespaceSymbol)namespaceOrTypeSymbol;
                                        break;
                                    }
                                }
                                if ((object)namespaceSymbol2 == null)
                                {
                                    break;
                                }
                                namespaceSymbol = namespaceSymbol2;
                            }
                            for (; i < immutableArray.Length; i++)
                            {
                                namespaceSymbol = new MissingNamespaceSymbol(namespaceSymbol, immutableArray[i]);
                            }
                        }
                        Interlocked.CompareExchange(ref _lazyContainingNamespace, namespaceSymbol, null);
                    }
                    return _lazyContainingNamespace;
                }
            }

            private int TypeId
            {
                get
                {
                    if (_lazyTypeId == -1)
                    {
                        SpecialType value = SpecialType.None;
                        AssemblySymbol containingAssembly = _containingModule.ContainingAssembly;
                        if ((Arity == 0 || MangleName) && (object)containingAssembly != null && (object)containingAssembly == containingAssembly.CorLibrary && _containingModule.Ordinal == 0)
                        {
                            value = SpecialTypes.GetTypeFromMetadataName(MetadataHelpers.BuildQualifiedName(_namespaceName, MetadataName));
                        }
                        Interlocked.CompareExchange(ref _lazyTypeId, (int)value, -1);
                    }
                    return _lazyTypeId;
                }
            }

            public override SpecialType SpecialType
            {
                get
                {
                    if (TypeId < 46)
                    {
                        return (SpecialType)_lazyTypeId;
                    }
                    return SpecialType.None;
                }
            }

            internal override DiagnosticInfo ErrorInfo
            {
                get
                {
                    if (_lazyErrorInfo == null)
                    {
                        DiagnosticInfo value = ((TypeId != 0) ? new CSDiagnosticInfo(ErrorCode.ERR_PredefinedTypeNotFound, MetadataHelpers.BuildQualifiedName(_namespaceName, MetadataName)) : base.ErrorInfo);
                        Interlocked.CompareExchange(ref _lazyErrorInfo, value, null);
                    }
                    return _lazyErrorInfo;
                }
            }

            internal sealed override bool IsNativeIntegerType => _isNativeInt;

            internal sealed override NamedTypeSymbol? NativeIntegerUnderlyingType
            {
                get
                {
                    if (!_isNativeInt)
                    {
                        return null;
                    }
                    return AsNativeInteger(asNativeInt: false);
                }
            }

            public TopLevel(ModuleSymbol module, string @namespace, string name, int arity, bool mangleName)
                : this(module, @namespace, name, arity, mangleName, isNativeInt: false, null, null, -1, null)
            {
            }

            public TopLevel(ModuleSymbol module, ref MetadataTypeName fullName, DiagnosticInfo? errorInfo = null)
                : this(module, ref fullName, -1, errorInfo)
            {
            }

            public TopLevel(ModuleSymbol module, ref MetadataTypeName fullName, SpecialType specialType, DiagnosticInfo? errorInfo = null)
                : this(module, ref fullName, (int)specialType, errorInfo)
            {
            }

            public TopLevel(ModuleSymbol module, ref MetadataTypeName fullName, WellKnownType wellKnownType, DiagnosticInfo? errorInfo = null)
                : this(module, ref fullName, (int)wellKnownType, errorInfo)
            {
            }

            private TopLevel(ModuleSymbol module, ref MetadataTypeName fullName, int typeId, DiagnosticInfo? errorInfo)
                : this(module, ref fullName, fullName.ForcedArity == -1 || fullName.ForcedArity == fullName.InferredArity, errorInfo, typeId)
            {
            }

            private TopLevel(ModuleSymbol module, ref MetadataTypeName fullName, bool mangleName, DiagnosticInfo? errorInfo, int typeId)
                : this(module, fullName.NamespaceName, mangleName ? fullName.UnmangledTypeName : fullName.TypeName, mangleName ? fullName.InferredArity : fullName.ForcedArity, mangleName, isNativeInt: false, errorInfo, null, typeId, null)
            {
            }

            private TopLevel(ModuleSymbol module, string @namespace, string name, int arity, bool mangleName, bool isNativeInt, DiagnosticInfo? errorInfo, NamespaceSymbol? containingNamespace, int typeId, TupleExtraData? tupleData)
                : base(name, arity, mangleName, tupleData)
            {
                _namespaceName = @namespace;
                _containingModule = module;
                _isNativeInt = isNativeInt;
                _lazyErrorInfo = errorInfo;
                _lazyContainingNamespace = containingNamespace;
                _lazyTypeId = typeId;
            }

            protected override NamedTypeSymbol WithTupleDataCore(TupleExtraData newData)
            {
                return new TopLevel(_containingModule, _namespaceName, name, arity, mangleName, _isNativeInt, _lazyErrorInfo, _lazyContainingNamespace, _lazyTypeId, newData);
            }

            public override int GetHashCode()
            {
                if (SpecialType == SpecialType.System_Object)
                {
                    return 1;
                }
                return Hash.Combine(MetadataName, Hash.Combine(_containingModule, Hash.Combine(_namespaceName, arity)));
            }

            internal sealed override NamedTypeSymbol AsNativeInteger()
            {
                return AsNativeInteger(asNativeInt: true);
            }

            private TopLevel AsNativeInteger(bool asNativeInt)
            {
                if (asNativeInt == _isNativeInt)
                {
                    return this;
                }
                return new TopLevel(_containingModule, _namespaceName, name, arity, mangleName, asNativeInt, _lazyErrorInfo, _lazyContainingNamespace, _lazyTypeId, base.TupleData);
            }

            internal override bool Equals(TypeSymbol t2, TypeCompareKind comparison)
            {
                if ((object)this == t2)
                {
                    return true;
                }
                if ((comparison & TypeCompareKind.IgnoreDynamic) != 0 && (object)t2 != null && t2.TypeKind == TypeKind.Dynamic && SpecialType == SpecialType.System_Object)
                {
                    return true;
                }
                if (!(t2 is TopLevel topLevel))
                {
                    return false;
                }
                if ((comparison & TypeCompareKind.IgnoreNativeIntegers) == 0 && _isNativeInt != topLevel._isNativeInt)
                {
                    return false;
                }
                if (string.Equals(MetadataName, topLevel.MetadataName, StringComparison.Ordinal) && arity == topLevel.arity && string.Equals(_namespaceName, topLevel.NamespaceName, StringComparison.Ordinal))
                {
                    return _containingModule.Equals(topLevel._containingModule);
                }
                return false;
            }
        }

        internal sealed class Nested : MissingMetadataTypeSymbol
        {
            private readonly NamedTypeSymbol _containingType;

            public override Symbol ContainingSymbol => _containingType;

            public override SpecialType SpecialType => SpecialType.None;

            public Nested(NamedTypeSymbol containingType, string name, int arity, bool mangleName)
                : base(name, arity, mangleName)
            {
                _containingType = containingType;
            }

            public Nested(NamedTypeSymbol containingType, ref MetadataTypeName emittedName)
                : this(containingType, ref emittedName, emittedName.ForcedArity == -1 || emittedName.ForcedArity == emittedName.InferredArity)
            {
            }

            private Nested(NamedTypeSymbol containingType, ref MetadataTypeName emittedName, bool mangleName)
                : this(containingType, mangleName ? emittedName.UnmangledTypeName : emittedName.TypeName, mangleName ? emittedName.InferredArity : emittedName.ForcedArity, mangleName)
            {
            }

            protected override NamedTypeSymbol WithTupleDataCore(TupleExtraData newData)
            {
                throw ExceptionUtilities.Unreachable;
            }

            public override int GetHashCode()
            {
                return Hash.Combine(_containingType, Hash.Combine(MetadataName, arity));
            }

            internal override bool Equals(TypeSymbol t2, TypeCompareKind comparison)
            {
                if ((object)this == t2)
                {
                    return true;
                }
                if (t2 is Nested nested && string.Equals(MetadataName, nested.MetadataName, StringComparison.Ordinal) && arity == nested.arity)
                {
                    return _containingType.Equals(nested._containingType, comparison);
                }
                return false;
            }
        }

        protected readonly string name;

        protected readonly int arity;

        protected readonly bool mangleName;

        public override string Name => name;

        internal override bool MangleName => mangleName;

        public override int Arity => arity;

        internal override DiagnosticInfo ErrorInfo
        {
            get
            {
                AssemblySymbol containingAssembly = ContainingAssembly;
                if (containingAssembly.IsMissing)
                {
                    return new CSDiagnosticInfo(ErrorCode.ERR_NoTypeDef, this, containingAssembly.Identity);
                }
                ModuleSymbol containingModule = ContainingModule;
                if (containingModule.IsMissing)
                {
                    return new CSDiagnosticInfo(ErrorCode.ERR_NoTypeDefFromModule, this, containingModule.Name);
                }
                if (containingAssembly.Dangerous_IsFromSomeCompilation)
                {
                    return new CSDiagnosticInfo(ErrorCode.ERR_MissingTypeInSource, this);
                }
                return new CSDiagnosticInfo(ErrorCode.ERR_MissingTypeInAssembly, this, containingAssembly.Name);
            }
        }

        private MissingMetadataTypeSymbol(string name, int arity, bool mangleName, TupleExtraData? tupleData = null)
            : base(tupleData)
        {
            this.name = name;
            this.arity = arity;
            this.mangleName = mangleName && arity > 0;
        }
    }
}
