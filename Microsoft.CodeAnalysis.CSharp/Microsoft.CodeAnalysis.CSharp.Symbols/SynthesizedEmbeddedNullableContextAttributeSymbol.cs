using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedEmbeddedNullableContextAttributeSymbol : SynthesizedEmbeddedAttributeSymbolBase
    {
        private readonly ImmutableArray<FieldSymbol> _fields;

        private readonly ImmutableArray<MethodSymbol> _constructors;

        public override ImmutableArray<MethodSymbol> Constructors => _constructors;

        internal override bool IsRecord => false;

        internal override bool IsRecordStruct => false;

        public SynthesizedEmbeddedNullableContextAttributeSymbol(string name, NamespaceSymbol containingNamespace, ModuleSymbol containingModule, NamedTypeSymbol systemAttributeType, TypeSymbol systemByteType)
            : base(name, containingNamespace, containingModule, systemAttributeType)
        {
            _fields = ImmutableArray.Create((FieldSymbol)new SynthesizedFieldSymbol(this, systemByteType, "Flag", isPublic: true, isReadOnly: true));
            _constructors = ImmutableArray.Create((MethodSymbol)new SynthesizedEmbeddedAttributeConstructorWithBodySymbol(this, (MethodSymbol m) => ImmutableArray.Create(SynthesizedParameterSymbol.Create(m, TypeWithAnnotations.Create(systemByteType), 0, RefKind.None)), GenerateConstructorBody));
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
            return new AttributeUsageInfo(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Method | AttributeTargets.Interface | AttributeTargets.Delegate, allowMultiple: false, inherited: false);
        }

        private void GenerateConstructorBody(SyntheticBoundNodeFactory factory, ArrayBuilder<BoundStatement> statements, ImmutableArray<ParameterSymbol> parameters)
        {
            statements.Add(factory.ExpressionStatement(factory.AssignmentExpression(factory.Field(factory.This(), _fields.Single()), factory.Parameter(parameters.Single()))));
        }
    }
}
