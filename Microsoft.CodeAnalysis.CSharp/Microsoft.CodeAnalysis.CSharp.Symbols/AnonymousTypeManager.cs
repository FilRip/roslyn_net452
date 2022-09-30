using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.RuntimeMembers;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class AnonymousTypeManager : CommonAnonymousTypeManager
    {
        private sealed class AnonymousTypeConstructorSymbol : SynthesizedMethodBase
        {
            private readonly ImmutableArray<ParameterSymbol> _parameters;

            internal override bool HasSpecialName => true;

            public override MethodKind MethodKind => MethodKind.Constructor;

            public override bool ReturnsVoid => true;

            public override RefKind RefKind => RefKind.None;

            public override TypeWithAnnotations ReturnTypeWithAnnotations => TypeWithAnnotations.Create(base.Manager.System_Void);

            public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

            public override bool IsOverride => false;

            internal override bool IsMetadataFinal => false;

            public override ImmutableArray<Location> Locations => ContainingSymbol.Locations;

            internal override void GenerateMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
            {
                SyntheticBoundNodeFactory syntheticBoundNodeFactory = CreateBoundNodeFactory(compilationState, diagnostics);
                int parameterCount = ParameterCount;
                BoundStatement[] array = new BoundStatement[parameterCount + 2];
                int num = 0;
                BoundExpression boundExpression = MethodCompiler.GenerateBaseParameterlessConstructorInitializer(this, diagnostics);
                if (boundExpression == null)
                {
                    return;
                }
                array[num++] = syntheticBoundNodeFactory.ExpressionStatement(boundExpression);
                if (parameterCount > 0)
                {
                    AnonymousTypeTemplateSymbol anonymousTypeTemplateSymbol = (AnonymousTypeTemplateSymbol)ContainingType;
                    for (int i = 0; i < ParameterCount; i++)
                    {
                        array[num++] = syntheticBoundNodeFactory.Assignment(syntheticBoundNodeFactory.Field(syntheticBoundNodeFactory.This(), anonymousTypeTemplateSymbol.Properties[i].BackingField), syntheticBoundNodeFactory.Parameter(_parameters[i]));
                    }
                }
                array[num++] = syntheticBoundNodeFactory.Return();
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.Block(array));
            }

            internal AnonymousTypeConstructorSymbol(NamedTypeSymbol container, ImmutableArray<AnonymousTypePropertySymbol> properties)
                : base(container, ".ctor")
            {
                int length = properties.Length;
                if (length > 0)
                {
                    ParameterSymbol[] array = new ParameterSymbol[length];
                    for (int i = 0; i < length; i++)
                    {
                        PropertySymbol propertySymbol = properties[i];
                        array[i] = SynthesizedParameterSymbol.Create(this, propertySymbol.TypeWithAnnotations, i, RefKind.None, propertySymbol.Name);
                    }
                    _parameters = array.AsImmutableOrNull();
                }
                else
                {
                    _parameters = ImmutableArray<ParameterSymbol>.Empty;
                }
            }

            internal sealed override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
            {
                return false;
            }
        }

        private sealed class AnonymousTypePropertyGetAccessorSymbol : SynthesizedMethodBase
        {
            private readonly AnonymousTypePropertySymbol _property;

            internal override bool HasSpecialName => true;

            public override MethodKind MethodKind => MethodKind.PropertyGet;

            public override bool ReturnsVoid => false;

            public override RefKind RefKind => RefKind.None;

            public override TypeWithAnnotations ReturnTypeWithAnnotations => _property.TypeWithAnnotations;

            public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

            public override Symbol AssociatedSymbol => _property;

            public override ImmutableArray<Location> Locations => _property.Locations;

            public override bool IsOverride => false;

            internal override bool IsMetadataFinal => false;

            internal override void GenerateMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
            {
                SyntheticBoundNodeFactory syntheticBoundNodeFactory = CreateBoundNodeFactory(compilationState, diagnostics);
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.Block(syntheticBoundNodeFactory.Return(syntheticBoundNodeFactory.Field(syntheticBoundNodeFactory.This(), _property.BackingField))));
            }

            internal AnonymousTypePropertyGetAccessorSymbol(AnonymousTypePropertySymbol property)
                : base(property.ContainingType, SourcePropertyAccessorSymbol.GetAccessorName(property.Name, getNotSet: true, isWinMdOutput: false))
            {
                _property = property;
            }

            internal sealed override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
            {
                return false;
            }

            internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
            {
            }
        }

        private sealed class AnonymousTypeEqualsMethodSymbol : SynthesizedMethodBase
        {
            private readonly ImmutableArray<ParameterSymbol> _parameters;

            internal override bool HasSpecialName => false;

            public override MethodKind MethodKind => MethodKind.Ordinary;

            public override bool ReturnsVoid => false;

            public override RefKind RefKind => RefKind.None;

            public override TypeWithAnnotations ReturnTypeWithAnnotations => TypeWithAnnotations.Create(base.Manager.System_Boolean);

            public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

            public override bool IsOverride => true;

            internal override bool IsMetadataFinal => false;

            internal override void GenerateMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
            {
                AnonymousTypeManager manager = ((AnonymousTypeTemplateSymbol)ContainingType).Manager;
                SyntheticBoundNodeFactory syntheticBoundNodeFactory = CreateBoundNodeFactory(compilationState, diagnostics);
                AnonymousTypeTemplateSymbol anonymousTypeTemplateSymbol = (AnonymousTypeTemplateSymbol)ContainingType;
                BoundLocal boundLocal = syntheticBoundNodeFactory.StoreToTemp(syntheticBoundNodeFactory.As(syntheticBoundNodeFactory.Parameter(_parameters[0]), anonymousTypeTemplateSymbol), out BoundAssignmentOperator store);
                BoundStatement boundStatement = syntheticBoundNodeFactory.ExpressionStatement(store);
                BoundExpression boundExpression = syntheticBoundNodeFactory.Binary(BinaryOperatorKind.ObjectNotEqual, manager.System_Boolean, syntheticBoundNodeFactory.Convert(manager.System_Object, boundLocal), syntheticBoundNodeFactory.Null(manager.System_Object));
                if (anonymousTypeTemplateSymbol.Properties.Length > 0)
                {
                    ArrayBuilder<FieldSymbol> instance = ArrayBuilder<FieldSymbol>.GetInstance(anonymousTypeTemplateSymbol.Properties.Length);
                    ImmutableArray<AnonymousTypePropertySymbol>.Enumerator enumerator = anonymousTypeTemplateSymbol.Properties.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        AnonymousTypePropertySymbol current = enumerator.Current;
                        instance.Add(current.BackingField);
                    }
                    boundExpression = MethodBodySynthesizer.GenerateFieldEquals(boundExpression, boundLocal, instance, syntheticBoundNodeFactory);
                    instance.Free();
                }
                boundExpression = syntheticBoundNodeFactory.LogicalOr(syntheticBoundNodeFactory.ObjectEqual(syntheticBoundNodeFactory.This(), boundLocal), boundExpression);
                BoundStatement boundStatement2 = syntheticBoundNodeFactory.Return(boundExpression);
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.Block(ImmutableArray.Create(boundLocal.LocalSymbol), boundStatement, boundStatement2));
            }

            internal AnonymousTypeEqualsMethodSymbol(NamedTypeSymbol container)
                : base(container, "Equals")
            {
                _parameters = ImmutableArray.Create(SynthesizedParameterSymbol.Create(this, TypeWithAnnotations.Create(base.Manager.System_Object), 0, RefKind.None, "value"));
            }

            internal sealed override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
            {
                return true;
            }
        }

        private sealed class AnonymousTypeGetHashCodeMethodSymbol : SynthesizedMethodBase
        {
            internal override bool HasSpecialName => false;

            public override MethodKind MethodKind => MethodKind.Ordinary;

            public override bool ReturnsVoid => false;

            public override RefKind RefKind => RefKind.None;

            public override TypeWithAnnotations ReturnTypeWithAnnotations => TypeWithAnnotations.Create(base.Manager.System_Int32);

            public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

            public override bool IsOverride => true;

            internal override bool IsMetadataFinal => false;

            internal override void GenerateMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
            {
                AnonymousTypeManager manager = ((AnonymousTypeTemplateSymbol)ContainingType).Manager;
                SyntheticBoundNodeFactory syntheticBoundNodeFactory = CreateBoundNodeFactory(compilationState, diagnostics);
                AnonymousTypeTemplateSymbol anonymousTypeTemplateSymbol = (AnonymousTypeTemplateSymbol)ContainingType;
                int num = 0;
                ImmutableArray<AnonymousTypePropertySymbol>.Enumerator enumerator = anonymousTypeTemplateSymbol.Properties.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    AnonymousTypePropertySymbol current = enumerator.Current;
                    num = num * -1521134295 + Hash.GetFNVHashCode(current.BackingField.Name);
                }
                BoundExpression boundExpression = syntheticBoundNodeFactory.Literal(num);
                MethodSymbol system_Collections_Generic_EqualityComparer_T__GetHashCode = manager.System_Collections_Generic_EqualityComparer_T__GetHashCode;
                MethodSymbol system_Collections_Generic_EqualityComparer_T__get_Default = manager.System_Collections_Generic_EqualityComparer_T__get_Default;
                BoundLiteral boundHashFactor = null;
                for (int i = 0; i < anonymousTypeTemplateSymbol.Properties.Length; i++)
                {
                    boundExpression = MethodBodySynthesizer.GenerateHashCombine(boundExpression, system_Collections_Generic_EqualityComparer_T__GetHashCode, system_Collections_Generic_EqualityComparer_T__get_Default, ref boundHashFactor, syntheticBoundNodeFactory.Field(syntheticBoundNodeFactory.This(), anonymousTypeTemplateSymbol.Properties[i].BackingField), syntheticBoundNodeFactory);
                }
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.Block(syntheticBoundNodeFactory.Return(boundExpression)));
            }

            internal AnonymousTypeGetHashCodeMethodSymbol(NamedTypeSymbol container)
                : base(container, "GetHashCode")
            {
            }

            internal sealed override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
            {
                return true;
            }
        }

        private sealed class AnonymousTypeToStringMethodSymbol : SynthesizedMethodBase
        {
            internal override bool HasSpecialName => false;

            public override MethodKind MethodKind => MethodKind.Ordinary;

            public override bool ReturnsVoid => false;

            public override RefKind RefKind => RefKind.None;

            public override TypeWithAnnotations ReturnTypeWithAnnotations => TypeWithAnnotations.Create(base.Manager.System_String, NullableAnnotation.NotAnnotated);

            public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

            public override bool IsOverride => true;

            internal override bool IsMetadataFinal => false;

            internal override void GenerateMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
            {
                AnonymousTypeManager manager = ((AnonymousTypeTemplateSymbol)ContainingType).Manager;
                SyntheticBoundNodeFactory syntheticBoundNodeFactory = CreateBoundNodeFactory(compilationState, diagnostics);
                AnonymousTypeTemplateSymbol anonymousTypeTemplateSymbol = (AnonymousTypeTemplateSymbol)ContainingType;
                int length = anonymousTypeTemplateSymbol.Properties.Length;
                BoundExpression boundExpression = null;
                if (length > 0)
                {
                    BoundExpression[] array = new BoundExpression[length];
                    PooledStringBuilder instance = PooledStringBuilder.GetInstance();
                    for (int i = 0; i < length; i++)
                    {
                        AnonymousTypePropertySymbol anonymousTypePropertySymbol = anonymousTypeTemplateSymbol.Properties[i];
                        instance.Builder.AppendFormat((i == 0) ? "{{{{ {0} = {{{1}}}" : ", {0} = {{{1}}}", anonymousTypePropertySymbol.Name, i);
                        array[i] = syntheticBoundNodeFactory.Convert(manager.System_Object, new BoundLoweredConditionalAccess(syntheticBoundNodeFactory.Syntax, syntheticBoundNodeFactory.Field(syntheticBoundNodeFactory.This(), anonymousTypePropertySymbol.BackingField), null, syntheticBoundNodeFactory.Call(new BoundConditionalReceiver(syntheticBoundNodeFactory.Syntax, i, anonymousTypePropertySymbol.BackingField.Type), manager.System_Object__ToString), null, i, manager.System_String), Conversion.ImplicitReference);
                    }
                    instance.Builder.Append(" }}");
                    BoundExpression boundExpression2 = syntheticBoundNodeFactory.Literal(instance.ToStringAndFree());
                    MethodSymbol system_String__Format_IFormatProvider = manager.System_String__Format_IFormatProvider;
                    boundExpression = syntheticBoundNodeFactory.StaticCall(manager.System_String, system_String__Format_IFormatProvider, syntheticBoundNodeFactory.Null(system_String__Format_IFormatProvider.Parameters[0].Type), boundExpression2, syntheticBoundNodeFactory.ArrayOrEmpty(manager.System_Object, array));
                }
                else
                {
                    boundExpression = syntheticBoundNodeFactory.Literal("{ }");
                }
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.Block(syntheticBoundNodeFactory.Return(boundExpression)));
            }

            internal AnonymousTypeToStringMethodSymbol(NamedTypeSymbol container)
                : base(container, "ToString")
            {
            }

            internal sealed override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
            {
                return true;
            }
        }

        private struct SynthesizedDelegateKey : IEquatable<SynthesizedDelegateKey>
        {
            private readonly BitVector _byRefs;

            private readonly ushort _parameterCount;

            private readonly bool _returnsVoid;

            private readonly int _generation;

            public SynthesizedDelegateKey(int parameterCount, BitVector byRefs, bool returnsVoid, int generation)
            {
                _parameterCount = (ushort)parameterCount;
                _returnsVoid = returnsVoid;
                _generation = generation;
                _byRefs = byRefs;
            }

            public string MakeTypeName()
            {
                return GeneratedNames.MakeDynamicCallSiteDelegateName(_byRefs, _returnsVoid, _generation);
            }

            public override bool Equals(object obj)
            {
                if (obj is SynthesizedDelegateKey)
                {
                    return Equals((SynthesizedDelegateKey)obj);
                }
                return false;
            }

            public bool Equals(SynthesizedDelegateKey other)
            {
                if (_parameterCount == other._parameterCount && _returnsVoid == other._returnsVoid && _generation == other._generation)
                {
                    return _byRefs.Equals(other._byRefs);
                }
                return false;
            }

            public override int GetHashCode()
            {
                int newKey = Hash.Combine(_parameterCount, _generation);
                int hashCode = _returnsVoid.GetHashCode();
                BitVector byRefs = _byRefs;
                return Hash.Combine(newKey, Hash.Combine(hashCode, byRefs.GetHashCode()));
            }
        }

        private struct SynthesizedDelegateValue
        {
            public readonly SynthesizedDelegateSymbol Delegate;

            public readonly AnonymousTypeManager Manager;

            public SynthesizedDelegateValue(AnonymousTypeManager manager, SynthesizedDelegateSymbol @delegate)
            {
                Manager = manager;
                Delegate = @delegate;
            }
        }

        private class SynthesizedDelegateSymbolComparer : IComparer<SynthesizedDelegateSymbol>
        {
            public static readonly SynthesizedDelegateSymbolComparer Instance = new SynthesizedDelegateSymbolComparer();

            public int Compare(SynthesizedDelegateSymbol x, SynthesizedDelegateSymbol y)
            {
                return x.MetadataName.CompareTo(y.MetadataName);
            }
        }

        private sealed class AnonymousTypeComparer : IComparer<AnonymousTypeTemplateSymbol>
        {
            private readonly CSharpCompilation _compilation;

            public AnonymousTypeComparer(CSharpCompilation compilation)
            {
                _compilation = compilation;
            }

            public int Compare(AnonymousTypeTemplateSymbol x, AnonymousTypeTemplateSymbol y)
            {
                if ((object)x == y)
                {
                    return 0;
                }
                int num = CompareLocations(x.SmallestLocation, y.SmallestLocation);
                if (num == 0)
                {
                    num = string.CompareOrdinal(x.TypeDescriptorKey, y.TypeDescriptorKey);
                }
                return num;
            }

            private int CompareLocations(Location x, Location y)
            {
                if (x == y)
                {
                    return 0;
                }
                if (x == Location.None)
                {
                    return -1;
                }
                if (y == Location.None)
                {
                    return 1;
                }
                return _compilation.CompareSourceLocations(x, y);
            }
        }

        internal sealed class AnonymousTypePublicSymbol : NamedTypeSymbol
        {
            private readonly ImmutableArray<Symbol> _members;

            internal readonly ImmutableArray<AnonymousTypePropertySymbol> Properties;

            private readonly MultiDictionary<string, Symbol> _nameToSymbols = new MultiDictionary<string, Symbol>();

            internal readonly AnonymousTypeManager Manager;

            internal readonly AnonymousTypeDescriptor TypeDescriptor;

            internal override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotationsNoUseSiteDiagnostics => ImmutableArray<TypeWithAnnotations>.Empty;

            internal override bool HasCodeAnalysisEmbeddedAttribute => false;

            public override IEnumerable<string> MemberNames => _nameToSymbols.Keys;

            public override Symbol ContainingSymbol => Manager.Compilation.SourceModule.GlobalNamespace;

            public override string Name => string.Empty;

            public override string MetadataName => string.Empty;

            internal override bool MangleName => false;

            public override int Arity => 0;

            public override bool IsImplicitlyDeclared => false;

            public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

            public override bool IsAbstract => false;

            public sealed override bool IsRefLikeType => false;

            public sealed override bool IsReadOnly => false;

            public override bool IsSealed => true;

            public override bool MightContainExtensionMethods => false;

            internal override bool HasSpecialName => false;

            public override Accessibility DeclaredAccessibility => Accessibility.Internal;

            internal override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics => Manager.System_Object;

            public override TypeKind TypeKind => TypeKind.Class;

            internal override bool IsInterface => false;

            public override ImmutableArray<Location> Locations => ImmutableArray.Create(TypeDescriptor.Location);

            public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => Symbol.GetDeclaringSyntaxReferenceHelper<AnonymousObjectCreationExpressionSyntax>(Locations);

            public override bool IsStatic => false;

            public override bool IsAnonymousType => true;

            public override NamedTypeSymbol ConstructedFrom => this;

            internal override bool ShouldAddWinRTMembers => false;

            internal override bool IsWindowsRuntimeImport => false;

            internal override bool IsComImport => false;

            internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

            internal override TypeLayout Layout => default(TypeLayout);

            internal override CharSet MarshallingCharSet => base.DefaultMarshallingCharSet;

            public override bool IsSerializable => false;

            public sealed override bool AreLocalsZeroed
            {
                get
                {
                    throw ExceptionUtilities.Unreachable;
                }
            }

            internal override bool HasDeclarativeSecurity => false;

            internal sealed override NamedTypeSymbol NativeIntegerUnderlyingType => null;

            internal override bool IsRecord => false;

            internal override bool IsRecordStruct => false;

            internal AnonymousTypePublicSymbol(AnonymousTypeManager manager, AnonymousTypeDescriptor typeDescr)
            {
                Manager = manager;
                TypeDescriptor = typeDescr;
                ImmutableArray<AnonymousTypeField> fields = typeDescr.Fields;
                ImmutableArray<AnonymousTypePropertySymbol> properties = fields.SelectAsArray((AnonymousTypeField field, int i, AnonymousTypePublicSymbol type) => new AnonymousTypePropertySymbol(type, field, i), this);
                ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance(fields.Length * 2 + 1);
                ImmutableArray<AnonymousTypePropertySymbol>.Enumerator enumerator = properties.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    AnonymousTypePropertySymbol current = enumerator.Current;
                    instance.Add(current);
                    instance.Add(current.GetMethod);
                }
                Properties = properties;
                instance.Add(new AnonymousTypeConstructorSymbol(this, properties));
                _members = instance.ToImmutableAndFree();
                ImmutableArray<Symbol>.Enumerator enumerator2 = _members.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Symbol current2 = enumerator2.Current;
                    _nameToSymbols.Add(current2.Name, current2);
                }
            }

            protected override NamedTypeSymbol WithTupleDataCore(TupleExtraData newData)
            {
                throw ExceptionUtilities.Unreachable;
            }

            public override ImmutableArray<Symbol> GetMembers()
            {
                return _members;
            }

            internal override IEnumerable<FieldSymbol> GetFieldsToEmit()
            {
                throw ExceptionUtilities.Unreachable;
            }

            public override ImmutableArray<Symbol> GetMembers(string name)
            {
                MultiDictionary<string, Symbol>.ValueSet valueSet = _nameToSymbols[name];
                ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance(valueSet.Count);
                foreach (Symbol item in valueSet)
                {
                    instance.Add(item);
                }
                return instance.ToImmutableAndFree();
            }

            internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers()
            {
                return GetMembersUnordered();
            }

            internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers(string name)
            {
                return GetMembers(name);
            }

            public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol> basesBeingResolved)
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit()
            {
                throw ExceptionUtilities.Unreachable;
            }

            internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
            {
                throw ExceptionUtilities.Unreachable;
            }

            internal override ImmutableArray<string> GetAppliedConditionalSymbols()
            {
                return ImmutableArray<string>.Empty;
            }

            internal override AttributeUsageInfo GetAttributeUsageInfo()
            {
                return AttributeUsageInfo.Null;
            }

            internal override NamedTypeSymbol GetDeclaredBaseType(ConsList<TypeSymbol> basesBeingResolved)
            {
                return Manager.System_Object;
            }

            internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<TypeSymbol> basesBeingResolved)
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            internal sealed override NamedTypeSymbol AsNativeInteger()
            {
                throw ExceptionUtilities.Unreachable;
            }

            internal override bool Equals(TypeSymbol t2, TypeCompareKind comparison)
            {
                if ((object)this == t2)
                {
                    return true;
                }
                if (t2 is AnonymousTypePublicSymbol anonymousTypePublicSymbol)
                {
                    return TypeDescriptor.Equals(anonymousTypePublicSymbol.TypeDescriptor, comparison);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return TypeDescriptor.GetHashCode();
            }

            internal override bool HasPossibleWellKnownCloneMethod()
            {
                return false;
            }
        }

        private sealed class AnonymousTypeFieldSymbol : FieldSymbol
        {
            private readonly PropertySymbol _property;

            public override string Name => GeneratedNames.MakeAnonymousTypeBackingFieldName(_property.Name);

            public override FlowAnalysisAnnotations FlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

            internal override bool HasSpecialName => false;

            internal override bool HasRuntimeSpecialName => false;

            internal override bool IsNotSerialized => false;

            internal override MarshalPseudoCustomAttributeData MarshallingInformation => null;

            internal override int? TypeLayoutOffset => null;

            public override Symbol AssociatedSymbol => _property;

            public override bool IsReadOnly => true;

            public override bool IsVolatile => false;

            public override bool IsConst => false;

            internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

            public override Symbol ContainingSymbol => _property.ContainingType;

            public override NamedTypeSymbol ContainingType => _property.ContainingType;

            public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

            public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

            public override Accessibility DeclaredAccessibility => Accessibility.Private;

            public override bool IsStatic => false;

            public override bool IsImplicitlyDeclared => true;

            public AnonymousTypeFieldSymbol(PropertySymbol property)
            {
                _property = property;
            }

            internal override TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
            {
                return _property.TypeWithAnnotations;
            }

            internal override ConstantValue GetConstantValue(ConstantFieldsInProgress inProgress, bool earlyDecodingWellKnownAttributes)
            {
                return null;
            }

            internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
            {
                base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
                AnonymousTypeManager manager = ((AnonymousTypeTemplateSymbol)ContainingSymbol).Manager;
                Symbol.AddSynthesizedAttribute(ref attributes, manager.Compilation.TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggerBrowsableAttribute__ctor, ImmutableArray.Create(new TypedConstant(manager.System_Diagnostics_DebuggerBrowsableState, TypedConstantKind.Enum, DebuggerBrowsableState.Never))));
            }
        }

        internal sealed class AnonymousTypePropertySymbol : PropertySymbol
        {
            private readonly NamedTypeSymbol _containingType;

            private readonly TypeWithAnnotations _typeWithAnnotations;

            private readonly string _name;

            private readonly int _index;

            private readonly ImmutableArray<Location> _locations;

            private readonly AnonymousTypePropertyGetAccessorSymbol _getMethod;

            private readonly FieldSymbol _backingField;

            internal override int? MemberIndexOpt => _index;

            public override RefKind RefKind => RefKind.None;

            public override TypeWithAnnotations TypeWithAnnotations => _typeWithAnnotations;

            public override string Name => _name;

            internal override bool HasSpecialName => false;

            public override bool IsImplicitlyDeclared => false;

            public override ImmutableArray<Location> Locations => _locations;

            public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => Symbol.GetDeclaringSyntaxReferenceHelper<AnonymousObjectMemberDeclaratorSyntax>(Locations);

            public override bool IsStatic => false;

            public override bool IsOverride => false;

            public override bool IsVirtual => false;

            public override bool IsIndexer => false;

            public override bool IsSealed => false;

            public override bool IsAbstract => false;

            internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

            public override ImmutableArray<ParameterSymbol> Parameters => ImmutableArray<ParameterSymbol>.Empty;

            public override MethodSymbol SetMethod => null;

            public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

            internal override Microsoft.Cci.CallingConvention CallingConvention => Microsoft.Cci.CallingConvention.HasThis;

            public override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations => ImmutableArray<PropertySymbol>.Empty;

            public override Symbol ContainingSymbol => _containingType;

            public override NamedTypeSymbol ContainingType => _containingType;

            public override Accessibility DeclaredAccessibility => Accessibility.Public;

            internal override bool MustCallMethodsDirectly => false;

            public override bool IsExtern => false;

            public override MethodSymbol GetMethod => _getMethod;

            public FieldSymbol BackingField => _backingField;

            internal AnonymousTypePropertySymbol(AnonymousTypeTemplateSymbol container, AnonymousTypeField field, TypeWithAnnotations fieldTypeWithAnnotations, int index)
                : this(container, field, fieldTypeWithAnnotations, index, ImmutableArray<Location>.Empty, includeBackingField: true)
            {
            }

            internal AnonymousTypePropertySymbol(AnonymousTypePublicSymbol container, AnonymousTypeField field, int index)
                : this(container, field, field.TypeWithAnnotations, index, ImmutableArray.Create(field.Location), includeBackingField: false)
            {
            }

            private AnonymousTypePropertySymbol(NamedTypeSymbol container, AnonymousTypeField field, TypeWithAnnotations fieldTypeWithAnnotations, int index, ImmutableArray<Location> locations, bool includeBackingField)
            {
                _containingType = container;
                _typeWithAnnotations = fieldTypeWithAnnotations;
                _name = field.Name;
                _index = index;
                _locations = locations;
                _getMethod = new AnonymousTypePropertyGetAccessorSymbol(this);
                _backingField = (includeBackingField ? new AnonymousTypeFieldSymbol(this) : null);
            }

            public override bool Equals(Symbol obj, TypeCompareKind compareKind)
            {
                if (obj == null)
                {
                    return false;
                }
                if ((object)this == obj)
                {
                    return true;
                }
                if (!(obj is AnonymousTypePropertySymbol anonymousTypePropertySymbol))
                {
                    return false;
                }
                if ((object)anonymousTypePropertySymbol != null && anonymousTypePropertySymbol.Name == Name)
                {
                    return anonymousTypePropertySymbol.ContainingType.Equals(ContainingType, compareKind);
                }
                return false;
            }

            public override int GetHashCode()
            {
                return Hash.Combine(ContainingType.GetHashCode(), Name.GetHashCode());
            }
        }

        private abstract class SynthesizedMethodBase : SynthesizedInstanceMethodSymbol
        {
            private readonly NamedTypeSymbol _containingType;

            private readonly string _name;

            internal sealed override bool GenerateDebugInfo => false;

            public sealed override int Arity => 0;

            public sealed override Symbol ContainingSymbol => _containingType;

            public override NamedTypeSymbol ContainingType => _containingType;

            public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

            public sealed override Accessibility DeclaredAccessibility => Accessibility.Public;

            public sealed override bool IsStatic => false;

            public sealed override bool IsVirtual => false;

            public sealed override bool IsAsync => false;

            internal sealed override MethodImplAttributes ImplementationAttributes => MethodImplAttributes.IL;

            internal sealed override Microsoft.Cci.CallingConvention CallingConvention => Microsoft.Cci.CallingConvention.HasThis;

            public sealed override bool IsExtensionMethod => false;

            public sealed override bool HidesBaseMethodsByName => false;

            public sealed override bool IsVararg => false;

            public sealed override FlowAnalysisAnnotations ReturnTypeFlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

            public sealed override ImmutableHashSet<string> ReturnNotNullIfParameterNotNull => ImmutableHashSet<string>.Empty;

            public sealed override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotations => ImmutableArray<TypeWithAnnotations>.Empty;

            public sealed override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

            internal sealed override bool IsExplicitInterfaceImplementation => false;

            public sealed override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray<MethodSymbol>.Empty;

            internal sealed override bool IsDeclaredReadOnly => false;

            internal sealed override bool IsInitOnly => false;

            public sealed override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

            public override Symbol AssociatedSymbol => null;

            public sealed override bool IsAbstract => false;

            public sealed override bool IsSealed => false;

            public sealed override bool IsExtern => false;

            public sealed override string Name => _name;

            protected AnonymousTypeManager Manager
            {
                get
                {
                    if (!(_containingType is AnonymousTypeTemplateSymbol anonymousTypeTemplateSymbol))
                    {
                        return ((AnonymousTypePublicSymbol)_containingType).Manager;
                    }
                    return anonymousTypeTemplateSymbol.Manager;
                }
            }

            internal sealed override bool RequiresSecurityObject => false;

            internal sealed override MarshalPseudoCustomAttributeData ReturnValueMarshallingInformation => null;

            internal sealed override bool HasDeclarativeSecurity => false;

            internal override bool SynthesizesLoweredBoundBody => true;

            public SynthesizedMethodBase(NamedTypeSymbol containingType, string name)
            {
                _containingType = containingType;
                _name = name;
            }

            internal sealed override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
            {
                return false;
            }

            internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
            {
                base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
                Symbol.AddSynthesizedAttribute(ref attributes, Manager.Compilation.TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggerHiddenAttribute__ctor));
            }

            public sealed override DllImportData GetDllImportData()
            {
                return null;
            }

            internal sealed override IEnumerable<SecurityAttribute> GetSecurityInformation()
            {
                throw ExceptionUtilities.Unreachable;
            }

            internal sealed override ImmutableArray<string> GetAppliedConditionalSymbols()
            {
                return ImmutableArray<string>.Empty;
            }

            protected SyntheticBoundNodeFactory CreateBoundNodeFactory(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
            {
                return new SyntheticBoundNodeFactory(this, this.GetNonNullSyntaxNode(), compilationState, diagnostics)
                {
                    CurrentFunction = this
                };
            }

            internal sealed override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal sealed class NameAndIndex
        {
            public readonly string Name;

            public readonly int Index;

            public NameAndIndex(string name, int index)
            {
                Name = name;
                Index = index;
            }
        }

        internal sealed class AnonymousTypeTemplateSymbol : NamedTypeSymbol
        {
            private NameAndIndex _nameAndIndex;

            private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;

            private readonly ImmutableArray<Symbol> _members;

            internal readonly ImmutableArray<MethodSymbol> SpecialMembers;

            internal readonly ImmutableArray<AnonymousTypePropertySymbol> Properties;

            private readonly MultiDictionary<string, Symbol> _nameToSymbols = new MultiDictionary<string, Symbol>();

            internal readonly AnonymousTypeManager Manager;

            private Location _smallestLocation;

            internal readonly string TypeDescriptorKey;

            internal Location SmallestLocation => _smallestLocation;

            internal NameAndIndex NameAndIndex
            {
                get
                {
                    return _nameAndIndex;
                }
                set
                {
                    Interlocked.CompareExchange(ref _nameAndIndex, value, null);
                }
            }

            internal override bool HasCodeAnalysisEmbeddedAttribute => false;

            internal override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotationsNoUseSiteDiagnostics => GetTypeParametersAsTypeArguments();

            public override IEnumerable<string> MemberNames => _nameToSymbols.Keys;

            public override Symbol ContainingSymbol => Manager.Compilation.SourceModule.GlobalNamespace;

            public override string Name => _nameAndIndex.Name;

            internal override bool HasSpecialName => false;

            internal override bool MangleName => Arity > 0;

            public override int Arity => _typeParameters.Length;

            public override bool IsImplicitlyDeclared => true;

            public override ImmutableArray<TypeParameterSymbol> TypeParameters => _typeParameters;

            public override bool IsAbstract => false;

            public sealed override bool IsRefLikeType => false;

            public sealed override bool IsReadOnly => false;

            public override bool IsSealed => true;

            public override bool MightContainExtensionMethods => false;

            public sealed override bool AreLocalsZeroed => ContainingModule.AreLocalsZeroed;

            public override Accessibility DeclaredAccessibility => Accessibility.Internal;

            internal override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics => Manager.System_Object;

            public override TypeKind TypeKind => TypeKind.Class;

            internal override bool IsInterface => false;

            public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

            public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

            public override bool IsStatic => false;

            public override NamedTypeSymbol ConstructedFrom => this;

            internal override bool ShouldAddWinRTMembers => false;

            internal override bool IsWindowsRuntimeImport => false;

            internal override bool IsComImport => false;

            internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

            internal override TypeLayout Layout => default(TypeLayout);

            internal override CharSet MarshallingCharSet => base.DefaultMarshallingCharSet;

            public override bool IsSerializable => false;

            internal override bool HasDeclarativeSecurity => false;

            internal sealed override NamedTypeSymbol NativeIntegerUnderlyingType => null;

            internal override bool IsRecord => false;

            internal override bool IsRecordStruct => false;

            internal AnonymousTypeTemplateSymbol(AnonymousTypeManager manager, AnonymousTypeDescriptor typeDescr)
            {
                Manager = manager;
                TypeDescriptorKey = typeDescr.Key;
                _smallestLocation = typeDescr.Location;
                _nameAndIndex = null;
                int length = typeDescr.Fields.Length;
                ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance(length * 3 + 1);
                ArrayBuilder<AnonymousTypePropertySymbol> instance2 = ArrayBuilder<AnonymousTypePropertySymbol>.GetInstance(length);
                ArrayBuilder<TypeParameterSymbol> instance3 = ArrayBuilder<TypeParameterSymbol>.GetInstance(length);
                for (int i = 0; i < length; i++)
                {
                    AnonymousTypeField field = typeDescr.Fields[i];
                    AnonymousTypeParameterSymbol anonymousTypeParameterSymbol = new AnonymousTypeParameterSymbol(this, i, GeneratedNames.MakeAnonymousTypeParameterName(field.Name));
                    instance3.Add(anonymousTypeParameterSymbol);
                    AnonymousTypePropertySymbol anonymousTypePropertySymbol = new AnonymousTypePropertySymbol(this, field, TypeWithAnnotations.Create(anonymousTypeParameterSymbol), i);
                    instance2.Add(anonymousTypePropertySymbol);
                    instance.Add(anonymousTypePropertySymbol);
                    instance.Add(anonymousTypePropertySymbol.BackingField);
                    instance.Add(anonymousTypePropertySymbol.GetMethod);
                }
                _typeParameters = instance3.ToImmutableAndFree();
                Properties = instance2.ToImmutableAndFree();
                instance.Add(new AnonymousTypeConstructorSymbol(this, Properties));
                _members = instance.ToImmutableAndFree();
                ImmutableArray<Symbol>.Enumerator enumerator = _members.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current = enumerator.Current;
                    _nameToSymbols.Add(current.Name, current);
                }
                SpecialMembers = ImmutableArray.Create<MethodSymbol>(new AnonymousTypeEqualsMethodSymbol(this), new AnonymousTypeGetHashCodeMethodSymbol(this), new AnonymousTypeToStringMethodSymbol(this));
            }

            protected override NamedTypeSymbol WithTupleDataCore(TupleExtraData newData)
            {
                throw ExceptionUtilities.Unreachable;
            }

            internal AnonymousTypeKey GetAnonymousTypeKey()
            {
                return new AnonymousTypeKey(Properties.SelectAsArray((AnonymousTypePropertySymbol p) => new AnonymousTypeKeyField(p.Name, isKey: false, ignoreCase: false)));
            }

            internal void AdjustLocation(Location location)
            {
                Location smallestLocation;
                do
                {
                    smallestLocation = _smallestLocation;
                }
                while ((!(smallestLocation != null) || Manager.Compilation.CompareSourceLocations(smallestLocation, location) >= 0) && (object)Interlocked.CompareExchange(ref _smallestLocation, location, smallestLocation) != smallestLocation);
            }

            public override ImmutableArray<Symbol> GetMembers()
            {
                return _members;
            }

            internal override IEnumerable<FieldSymbol> GetFieldsToEmit()
            {
                ImmutableArray<Symbol>.Enumerator enumerator = GetMembers().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current = enumerator.Current;
                    if (current.Kind == SymbolKind.Field)
                    {
                        yield return (FieldSymbol)current;
                    }
                }
            }

            public override ImmutableArray<Symbol> GetMembers(string name)
            {
                MultiDictionary<string, Symbol>.ValueSet valueSet = _nameToSymbols[name];
                ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance(valueSet.Count);
                foreach (Symbol item in valueSet)
                {
                    instance.Add(item);
                }
                return instance.ToImmutableAndFree();
            }

            internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers()
            {
                return GetMembersUnordered();
            }

            internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers(string name)
            {
                return GetMembers(name);
            }

            public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol> basesBeingResolved)
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit()
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            internal override NamedTypeSymbol GetDeclaredBaseType(ConsList<TypeSymbol> basesBeingResolved)
            {
                return Manager.System_Object;
            }

            internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<TypeSymbol> basesBeingResolved)
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
            {
                throw ExceptionUtilities.Unreachable;
            }

            internal override ImmutableArray<string> GetAppliedConditionalSymbols()
            {
                return ImmutableArray<string>.Empty;
            }

            internal override AttributeUsageInfo GetAttributeUsageInfo()
            {
                return AttributeUsageInfo.Null;
            }

            internal sealed override NamedTypeSymbol AsNativeInteger()
            {
                throw ExceptionUtilities.Unreachable;
            }

            internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
            {
                base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
                Symbol.AddSynthesizedAttribute(ref attributes, Manager.Compilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
                if (Manager.Compilation.Options.OptimizationLevel == OptimizationLevel.Debug)
                {
                    Symbol.AddSynthesizedAttribute(ref attributes, TrySynthesizeDebuggerDisplayAttribute());
                }
            }

            private SynthesizedAttributeData TrySynthesizeDebuggerDisplayAttribute()
            {
                string value;
                if (Properties.Length == 0)
                {
                    value = "\\{ }";
                }
                else
                {
                    PooledStringBuilder instance = PooledStringBuilder.GetInstance();
                    StringBuilder builder = instance.Builder;
                    builder.Append("\\{ ");
                    int num = Math.Min(Properties.Length, 10);
                    for (int i = 0; i < num; i++)
                    {
                        string name = Properties[i].Name;
                        if (i > 0)
                        {
                            builder.Append(", ");
                        }
                        builder.Append(name);
                        builder.Append(" = {");
                        builder.Append(name);
                        builder.Append("}");
                    }
                    if (Properties.Length > num)
                    {
                        builder.Append(" ...");
                    }
                    builder.Append(" }");
                    value = instance.ToStringAndFree();
                }
                return Manager.Compilation.TrySynthesizeAttribute(WellKnownMember.System_Diagnostics_DebuggerDisplayAttribute__ctor, ImmutableArray.Create(new TypedConstant(Manager.System_String, TypedConstantKind.Primitive, value)), ImmutableArray.Create(new KeyValuePair<WellKnownMember, TypedConstant>(WellKnownMember.System_Diagnostics_DebuggerDisplayAttribute__Type, new TypedConstant(Manager.System_String, TypedConstantKind.Primitive, "<Anonymous Type>"))));
            }

            internal override bool HasPossibleWellKnownCloneMethod()
            {
                return false;
            }
        }

        internal sealed class AnonymousTypeParameterSymbol : TypeParameterSymbol
        {
            private readonly Symbol _container;

            private readonly int _ordinal;

            private readonly string _name;

            public override TypeParameterKind TypeParameterKind => TypeParameterKind.Type;

            public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

            public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

            public override int Ordinal => _ordinal;

            public override string Name => _name;

            public override bool HasConstructorConstraint => false;

            public override bool HasReferenceTypeConstraint => false;

            public override bool IsReferenceTypeFromConstraintTypes => false;

            internal override bool? ReferenceTypeConstraintIsNullable => false;

            public override bool HasNotNullConstraint => false;

            internal override bool? IsNotNullable => null;

            public override bool HasValueTypeConstraint => false;

            public override bool IsValueTypeFromConstraintTypes => false;

            public override bool HasUnmanagedTypeConstraint => false;

            public override bool IsImplicitlyDeclared => true;

            public override VarianceKind Variance => VarianceKind.None;

            public override Symbol ContainingSymbol => _container;

            public AnonymousTypeParameterSymbol(Symbol container, int ordinal, string name)
            {
                _container = container;
                _ordinal = ordinal;
                _name = name;
            }

            internal override void EnsureAllConstraintsAreResolved()
            {
            }

            internal override ImmutableArray<TypeWithAnnotations> GetConstraintTypes(ConsList<TypeParameterSymbol> inProgress)
            {
                return ImmutableArray<TypeWithAnnotations>.Empty;
            }

            internal override ImmutableArray<NamedTypeSymbol> GetInterfaces(ConsList<TypeParameterSymbol> inProgress)
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            internal override NamedTypeSymbol GetEffectiveBaseClass(ConsList<TypeParameterSymbol> inProgress)
            {
                return null;
            }

            internal override TypeSymbol GetDeducedBaseType(ConsList<TypeParameterSymbol> inProgress)
            {
                return null;
            }
        }

        private ConcurrentDictionary<string, AnonymousTypeTemplateSymbol> _lazyAnonymousTypeTemplates;

        private ConcurrentDictionary<SynthesizedDelegateKey, SynthesizedDelegateValue> _lazySynthesizedDelegates;

        public CSharpCompilation Compilation { get; }

        public NamedTypeSymbol System_Object => Compilation.GetSpecialType(SpecialType.System_Object);

        public NamedTypeSymbol System_Void => Compilation.GetSpecialType(SpecialType.System_Void);

        public NamedTypeSymbol System_Boolean => Compilation.GetSpecialType(SpecialType.System_Boolean);

        public NamedTypeSymbol System_String => Compilation.GetSpecialType(SpecialType.System_String);

        public NamedTypeSymbol System_Int32 => Compilation.GetSpecialType(SpecialType.System_Int32);

        public NamedTypeSymbol System_Diagnostics_DebuggerBrowsableState => Compilation.GetWellKnownType(WellKnownType.System_Diagnostics_DebuggerBrowsableState);

        public MethodSymbol System_Object__Equals => Compilation.GetSpecialTypeMember(SpecialMember.System_Object__Equals) as MethodSymbol;

        public MethodSymbol System_Object__ToString => Compilation.GetSpecialTypeMember(SpecialMember.System_Object__ToString) as MethodSymbol;

        public MethodSymbol System_Object__GetHashCode => Compilation.GetSpecialTypeMember(SpecialMember.System_Object__GetHashCode) as MethodSymbol;

        public MethodSymbol System_Collections_Generic_EqualityComparer_T__Equals => Compilation.GetWellKnownTypeMember(WellKnownMember.System_Collections_Generic_EqualityComparer_T__Equals) as MethodSymbol;

        public MethodSymbol System_Collections_Generic_EqualityComparer_T__GetHashCode => Compilation.GetWellKnownTypeMember(WellKnownMember.System_Collections_Generic_EqualityComparer_T__GetHashCode) as MethodSymbol;

        public MethodSymbol System_Collections_Generic_EqualityComparer_T__get_Default => Compilation.GetWellKnownTypeMember(WellKnownMember.System_Collections_Generic_EqualityComparer_T__get_Default) as MethodSymbol;

        public MethodSymbol System_String__Format_IFormatProvider => Compilation.GetWellKnownTypeMember(WellKnownMember.System_String__Format_IFormatProvider) as MethodSymbol;

        private ConcurrentDictionary<string, AnonymousTypeTemplateSymbol> AnonymousTypeTemplates
        {
            get
            {
                if (_lazyAnonymousTypeTemplates == null)
                {
                    ConcurrentDictionary<string, AnonymousTypeTemplateSymbol> concurrentDictionary = Compilation.PreviousSubmission?.AnonymousTypeManager.AnonymousTypeTemplates;
                    Interlocked.CompareExchange(ref _lazyAnonymousTypeTemplates, (concurrentDictionary == null) ? new ConcurrentDictionary<string, AnonymousTypeTemplateSymbol>() : new ConcurrentDictionary<string, AnonymousTypeTemplateSymbol>(concurrentDictionary), null);
                }
                return _lazyAnonymousTypeTemplates;
            }
        }

        private ConcurrentDictionary<SynthesizedDelegateKey, SynthesizedDelegateValue> SynthesizedDelegates
        {
            get
            {
                if (_lazySynthesizedDelegates == null)
                {
                    ConcurrentDictionary<SynthesizedDelegateKey, SynthesizedDelegateValue> concurrentDictionary = Compilation.PreviousSubmission?.AnonymousTypeManager._lazySynthesizedDelegates;
                    Interlocked.CompareExchange(ref _lazySynthesizedDelegates, (concurrentDictionary == null) ? new ConcurrentDictionary<SynthesizedDelegateKey, SynthesizedDelegateValue>() : new ConcurrentDictionary<SynthesizedDelegateKey, SynthesizedDelegateValue>(concurrentDictionary), null);
                }
                return _lazySynthesizedDelegates;
            }
        }

        internal AnonymousTypeManager(CSharpCompilation compilation)
        {
            Compilation = compilation;
        }

        public NamedTypeSymbol ConstructAnonymousTypeSymbol(AnonymousTypeDescriptor typeDescr)
        {
            return new AnonymousTypePublicSymbol(this, typeDescr);
        }

        internal static PropertySymbol GetAnonymousTypeProperty(NamedTypeSymbol type, int index)
        {
            return ((AnonymousTypePublicSymbol)type).Properties[index];
        }

        internal static ImmutableArray<TypeWithAnnotations> GetAnonymousTypePropertyTypesWithAnnotations(NamedTypeSymbol type)
        {
            return ((AnonymousTypePublicSymbol)type).TypeDescriptor.Fields.SelectAsArray((AnonymousTypeField f) => f.TypeWithAnnotations);
        }

        public static NamedTypeSymbol ConstructAnonymousTypeSymbol(NamedTypeSymbol type, ImmutableArray<TypeWithAnnotations> newFieldTypes)
        {
            AnonymousTypePublicSymbol anonymousTypePublicSymbol = (AnonymousTypePublicSymbol)type;
            return anonymousTypePublicSymbol.Manager.ConstructAnonymousTypeSymbol(anonymousTypePublicSymbol.TypeDescriptor.WithNewFieldsTypes(newFieldTypes));
        }

        public bool ReportMissingOrErroneousSymbols(BindingDiagnosticBag diagnostics)
        {
            bool hasError = false;
            ReportErrorOnSymbol(System_Object, diagnostics, ref hasError);
            ReportErrorOnSymbol(System_Void, diagnostics, ref hasError);
            ReportErrorOnSymbol(System_Boolean, diagnostics, ref hasError);
            ReportErrorOnSymbol(System_String, diagnostics, ref hasError);
            ReportErrorOnSymbol(System_Int32, diagnostics, ref hasError);
            ReportErrorOnSpecialMember(System_Object__Equals, SpecialMember.System_Object__Equals, diagnostics, ref hasError);
            ReportErrorOnSpecialMember(System_Object__ToString, SpecialMember.System_Object__ToString, diagnostics, ref hasError);
            ReportErrorOnSpecialMember(System_Object__GetHashCode, SpecialMember.System_Object__GetHashCode, diagnostics, ref hasError);
            ReportErrorOnWellKnownMember(System_String__Format_IFormatProvider, WellKnownMember.System_String__Format_IFormatProvider, diagnostics, ref hasError);
            ReportErrorOnWellKnownMember(System_Collections_Generic_EqualityComparer_T__Equals, WellKnownMember.System_Collections_Generic_EqualityComparer_T__Equals, diagnostics, ref hasError);
            ReportErrorOnWellKnownMember(System_Collections_Generic_EqualityComparer_T__GetHashCode, WellKnownMember.System_Collections_Generic_EqualityComparer_T__GetHashCode, diagnostics, ref hasError);
            ReportErrorOnWellKnownMember(System_Collections_Generic_EqualityComparer_T__get_Default, WellKnownMember.System_Collections_Generic_EqualityComparer_T__get_Default, diagnostics, ref hasError);
            return hasError;
        }

        private static void ReportErrorOnSymbol(Symbol symbol, BindingDiagnosticBag diagnostics, ref bool hasError)
        {
            if ((object)symbol != null)
            {
                hasError |= diagnostics.ReportUseSite(symbol, NoLocation.Singleton);
            }
        }

        private static void ReportErrorOnSpecialMember(Symbol symbol, SpecialMember member, BindingDiagnosticBag diagnostics, ref bool hasError)
        {
            if ((object)symbol == null)
            {
                MemberDescriptor descriptor = SpecialMembers.GetDescriptor(member);
                diagnostics.Add(ErrorCode.ERR_MissingPredefinedMember, NoLocation.Singleton, descriptor.DeclaringTypeMetadataName, descriptor.Name);
                hasError = true;
            }
            else
            {
                ReportErrorOnSymbol(symbol, diagnostics, ref hasError);
            }
        }

        private static void ReportErrorOnWellKnownMember(Symbol symbol, WellKnownMember member, BindingDiagnosticBag diagnostics, ref bool hasError)
        {
            if ((object)symbol == null)
            {
                MemberDescriptor descriptor = WellKnownMembers.GetDescriptor(member);
                diagnostics.Add(ErrorCode.ERR_MissingPredefinedMember, NoLocation.Singleton, descriptor.DeclaringTypeMetadataName, descriptor.Name);
                hasError = true;
            }
            else
            {
                ReportErrorOnSymbol(symbol, diagnostics, ref hasError);
                ReportErrorOnSymbol(symbol.ContainingType, diagnostics, ref hasError);
            }
        }

        [Conditional("DEBUG")]
        private void CheckSourceLocationSeen(AnonymousTypePublicSymbol anonymous)
        {
        }

        internal SynthesizedDelegateSymbol SynthesizeDelegate(int parameterCount, BitVector byRefParameters, bool returnsVoid, int generation)
        {
            SynthesizedDelegateKey key = new SynthesizedDelegateKey(parameterCount, byRefParameters, returnsVoid, generation);
            if (SynthesizedDelegates.TryGetValue(key, out var value))
            {
                return value.Delegate;
            }
            return SynthesizedDelegates.GetOrAdd(key, new SynthesizedDelegateValue(this, new SynthesizedDelegateSymbol(Compilation.Assembly.GlobalNamespace, key.MakeTypeName(), System_Object, Compilation.GetSpecialType(SpecialType.System_IntPtr), returnsVoid ? Compilation.GetSpecialType(SpecialType.System_Void) : null, parameterCount, byRefParameters))).Delegate;
        }

        private NamedTypeSymbol ConstructAnonymousTypeImplementationSymbol(AnonymousTypePublicSymbol anonymous)
        {
            AnonymousTypeDescriptor typeDescriptor = anonymous.TypeDescriptor;
            if (!AnonymousTypeTemplates.TryGetValue(typeDescriptor.Key, out var value))
            {
                value = AnonymousTypeTemplates.GetOrAdd(typeDescriptor.Key, new AnonymousTypeTemplateSymbol(this, typeDescriptor));
            }
            if (value.Manager == this)
            {
                value.AdjustLocation(typeDescriptor.Location);
            }
            if (value.Arity == 0)
            {
                return value;
            }
            ImmutableArray<TypeSymbol> typeArguments = typeDescriptor.Fields.SelectAsArray((AnonymousTypeField f) => f.Type);
            return value.Construct(typeArguments);
        }

        private AnonymousTypeTemplateSymbol CreatePlaceholderTemplate(AnonymousTypeKey key)
        {
            ImmutableArray<AnonymousTypeField> fields = key.Fields.SelectAsArray((AnonymousTypeKeyField f) => new AnonymousTypeField(f.Name, Location.None, default(TypeWithAnnotations)));
            AnonymousTypeDescriptor typeDescr = new AnonymousTypeDescriptor(fields, Location.None);
            return new AnonymousTypeTemplateSymbol(this, typeDescr);
        }

        public void AssignTemplatesNamesAndCompile(MethodCompiler compiler, PEModuleBuilder moduleBeingBuilt, BindingDiagnosticBag diagnostics)
        {
            ImmutableArray<AnonymousTypeKey>.Enumerator enumerator = moduleBeingBuilt.GetPreviousAnonymousTypes().GetEnumerator();
            while (enumerator.MoveNext())
            {
                AnonymousTypeKey key = enumerator.Current;
                string key2 = AnonymousTypeDescriptor.ComputeKey(key.Fields, (AnonymousTypeKeyField f) => f.Name);
                AnonymousTypeTemplates.GetOrAdd(key2, (string k) => CreatePlaceholderTemplate(key));
            }
            ArrayBuilder<AnonymousTypeTemplateSymbol> instance = ArrayBuilder<AnonymousTypeTemplateSymbol>.GetInstance();
            GetCreatedAnonymousTypeTemplates(instance);
            if (!base.AreTemplatesSealed)
            {
                string text;
                if (moduleBeingBuilt.OutputKind == OutputKind.NetModule)
                {
                    text = moduleBeingBuilt.Name;
                    string defaultExtension = OutputKind.NetModule.GetDefaultExtension();
                    if (text.EndsWith(defaultExtension, StringComparison.OrdinalIgnoreCase))
                    {
                        text = text.Substring(0, text.Length - defaultExtension.Length);
                    }
                    text = MetadataHelpers.MangleForTypeNameIfNeeded(text);
                }
                else
                {
                    text = string.Empty;
                }
                int nextAnonymousTypeIndex = moduleBeingBuilt.GetNextAnonymousTypeIndex();
                ArrayBuilder<AnonymousTypeTemplateSymbol>.Enumerator enumerator2 = instance.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    AnonymousTypeTemplateSymbol current = enumerator2.Current;
                    if (!moduleBeingBuilt.TryGetAnonymousTypeName(current, out var name, out var index))
                    {
                        index = nextAnonymousTypeIndex++;
                        name = GeneratedNames.MakeAnonymousTypeTemplateName(index, Compilation.GetSubmissionSlotIndex(), text);
                    }
                    current.NameAndIndex = new NameAndIndex(name, index);
                }
                SealTemplates();
            }
            if (instance.Count > 0 && !ReportMissingOrErroneousSymbols(diagnostics))
            {
                ArrayBuilder<AnonymousTypeTemplateSymbol>.Enumerator enumerator2 = instance.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    AnonymousTypeTemplateSymbol current2 = enumerator2.Current;
                    ImmutableArray<MethodSymbol>.Enumerator enumerator3 = current2.SpecialMembers.GetEnumerator();
                    while (enumerator3.MoveNext())
                    {
                        MethodSymbol current3 = enumerator3.Current;
                        moduleBeingBuilt.AddSynthesizedDefinition(current2, current3.GetCciAdapter());
                    }
                    compiler.Visit(current2);
                }
            }
            instance.Free();
            ArrayBuilder<SynthesizedDelegateSymbol> instance2 = ArrayBuilder<SynthesizedDelegateSymbol>.GetInstance();
            GetCreatedSynthesizedDelegates(instance2);
            ArrayBuilder<SynthesizedDelegateSymbol>.Enumerator enumerator4 = instance2.GetEnumerator();
            while (enumerator4.MoveNext())
            {
                SynthesizedDelegateSymbol current4 = enumerator4.Current;
                compiler.Visit(current4);
            }
            instance2.Free();
        }

        private void GetCreatedAnonymousTypeTemplates(ArrayBuilder<AnonymousTypeTemplateSymbol> builder)
        {
            ConcurrentDictionary<string, AnonymousTypeTemplateSymbol> lazyAnonymousTypeTemplates = _lazyAnonymousTypeTemplates;
            if (lazyAnonymousTypeTemplates == null)
            {
                return;
            }
            foreach (AnonymousTypeTemplateSymbol value in lazyAnonymousTypeTemplates.Values)
            {
                if (value.Manager == this)
                {
                    builder.Add(value);
                }
            }
            builder.Sort(new AnonymousTypeComparer(Compilation));
        }

        private void GetCreatedSynthesizedDelegates(ArrayBuilder<SynthesizedDelegateSymbol> builder)
        {
            ConcurrentDictionary<SynthesizedDelegateKey, SynthesizedDelegateValue> lazySynthesizedDelegates = _lazySynthesizedDelegates;
            if (lazySynthesizedDelegates == null)
            {
                return;
            }
            foreach (SynthesizedDelegateValue value in lazySynthesizedDelegates.Values)
            {
                if (value.Manager == this)
                {
                    builder.Add(value.Delegate);
                }
            }
            builder.Sort(SynthesizedDelegateSymbolComparer.Instance);
        }

        internal IReadOnlyDictionary<AnonymousTypeKey, AnonymousTypeValue> GetAnonymousTypeMap()
        {
            Dictionary<AnonymousTypeKey, AnonymousTypeValue> dictionary = new Dictionary<AnonymousTypeKey, AnonymousTypeValue>();
            ArrayBuilder<AnonymousTypeTemplateSymbol> instance = ArrayBuilder<AnonymousTypeTemplateSymbol>.GetInstance();
            GetCreatedAnonymousTypeTemplates(instance);
            ArrayBuilder<AnonymousTypeTemplateSymbol>.Enumerator enumerator = instance.GetEnumerator();
            while (enumerator.MoveNext())
            {
                AnonymousTypeTemplateSymbol current = enumerator.Current;
                NameAndIndex nameAndIndex = current.NameAndIndex;
                AnonymousTypeKey anonymousTypeKey = current.GetAnonymousTypeKey();
                AnonymousTypeValue value = new AnonymousTypeValue(nameAndIndex.Name, nameAndIndex.Index, current.GetCciAdapter());
                dictionary.Add(anonymousTypeKey, value);
            }
            instance.Free();
            return dictionary;
        }

        internal ImmutableArray<NamedTypeSymbol> GetAllCreatedTemplates()
        {
            ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
            ArrayBuilder<AnonymousTypeTemplateSymbol> instance2 = ArrayBuilder<AnonymousTypeTemplateSymbol>.GetInstance();
            GetCreatedAnonymousTypeTemplates(instance2);
            instance.AddRange(instance2);
            instance2.Free();
            ArrayBuilder<SynthesizedDelegateSymbol> instance3 = ArrayBuilder<SynthesizedDelegateSymbol>.GetInstance();
            GetCreatedSynthesizedDelegates(instance3);
            instance.AddRange(instance3);
            instance3.Free();
            return instance.ToImmutableAndFree();
        }

        internal static bool IsAnonymousTypeTemplate(NamedTypeSymbol type)
        {
            return type is AnonymousTypeTemplateSymbol;
        }

        internal static ImmutableArray<MethodSymbol> GetAnonymousTypeHiddenMethods(NamedTypeSymbol type)
        {
            return ((AnonymousTypeTemplateSymbol)type).SpecialMembers;
        }

        internal static NamedTypeSymbol TranslateAnonymousTypeSymbol(NamedTypeSymbol type)
        {
            AnonymousTypePublicSymbol anonymousTypePublicSymbol = (AnonymousTypePublicSymbol)type;
            return anonymousTypePublicSymbol.Manager.ConstructAnonymousTypeImplementationSymbol(anonymousTypePublicSymbol);
        }

        internal static MethodSymbol TranslateAnonymousTypeMethodSymbol(MethodSymbol method)
        {
            NamedTypeSymbol namedTypeSymbol = TranslateAnonymousTypeSymbol(method.ContainingType);
            ImmutableArray<Symbol>.Enumerator enumerator = namedTypeSymbol.OriginalDefinition.GetMembers(method.Name).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind == SymbolKind.Method)
                {
                    return ((MethodSymbol)current).AsMember(namedTypeSymbol);
                }
            }
            throw ExceptionUtilities.Unreachable;
        }
    }
}
