using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedEmbeddedNullableAttributeSymbol : SynthesizedEmbeddedAttributeSymbolBase
    {
        private readonly ImmutableArray<FieldSymbol> _fields;

        private readonly ImmutableArray<MethodSymbol> _constructors;

        private readonly TypeSymbol _byteTypeSymbol;

        private const string NullableFlagsFieldName = "NullableFlags";

        public override ImmutableArray<MethodSymbol> Constructors => _constructors;

        internal override bool IsRecord => false;

        internal override bool IsRecordStruct => false;

        public SynthesizedEmbeddedNullableAttributeSymbol(string name, NamespaceSymbol containingNamespace, ModuleSymbol containingModule, NamedTypeSymbol systemAttributeType, TypeSymbol systemByteType)
            : base(name, containingNamespace, containingModule, systemAttributeType)
        {
            _byteTypeSymbol = systemByteType;
            TypeWithAnnotations annotatedByteType = TypeWithAnnotations.Create(systemByteType);
            TypeWithAnnotations byteArrayType = TypeWithAnnotations.Create(ArrayTypeSymbol.CreateSZArray(systemByteType.ContainingAssembly, annotatedByteType));
            _fields = ImmutableArray.Create((FieldSymbol)new SynthesizedFieldSymbol(this, byteArrayType.Type, "NullableFlags", isPublic: true, isReadOnly: true));
            _constructors = ImmutableArray.Create(new SynthesizedEmbeddedAttributeConstructorWithBodySymbol(this, (MethodSymbol m) => ImmutableArray.Create(SynthesizedParameterSymbol.Create(m, annotatedByteType, 0, RefKind.None)), GenerateSingleByteConstructorBody), (MethodSymbol)new SynthesizedEmbeddedAttributeConstructorWithBodySymbol(this, (MethodSymbol m) => ImmutableArray.Create(SynthesizedParameterSymbol.Create(m, byteArrayType, 0, RefKind.None)), GenerateByteArrayConstructorBody));
        }

        internal override IEnumerable<FieldSymbol> GetFieldsToEmit()
        {
            return _fields;
        }

        internal override bool HasPossibleWellKnownCloneMethod()
        {
            return false;
        }

        internal override AttributeUsageInfo GetAttributeUsageInfo()
        {
            return new AttributeUsageInfo(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.ReturnValue | AttributeTargets.GenericParameter, allowMultiple: false, inherited: false);
        }

        private void GenerateByteArrayConstructorBody(SyntheticBoundNodeFactory factory, ArrayBuilder<BoundStatement> statements, ImmutableArray<ParameterSymbol> parameters)
        {
            statements.Add(factory.ExpressionStatement(factory.AssignmentExpression(factory.Field(factory.This(), _fields.Single()), factory.Parameter(parameters.Single()))));
        }

        private void GenerateSingleByteConstructorBody(SyntheticBoundNodeFactory factory, ArrayBuilder<BoundStatement> statements, ImmutableArray<ParameterSymbol> parameters)
        {
            statements.Add(factory.ExpressionStatement(factory.AssignmentExpression(factory.Field(factory.This(), _fields.Single()), factory.Array(_byteTypeSymbol, ImmutableArray.Create((BoundExpression)factory.Parameter(parameters.Single()))))));
        }
    }
}
