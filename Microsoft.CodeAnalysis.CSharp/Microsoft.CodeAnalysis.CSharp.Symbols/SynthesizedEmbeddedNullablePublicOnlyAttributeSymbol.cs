using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedEmbeddedNullablePublicOnlyAttributeSymbol : SynthesizedEmbeddedAttributeSymbolBase
    {
        private readonly ImmutableArray<FieldSymbol> _fields;

        private readonly ImmutableArray<MethodSymbol> _constructors;

        public override ImmutableArray<MethodSymbol> Constructors => _constructors;

        internal override bool IsRecord => false;

        internal override bool IsRecordStruct => false;

        public SynthesizedEmbeddedNullablePublicOnlyAttributeSymbol(string name, NamespaceSymbol containingNamespace, ModuleSymbol containingModule, NamedTypeSymbol systemAttributeType, TypeSymbol systemBooleanType)
            : base(name, containingNamespace, containingModule, systemAttributeType)
        {
            _fields = ImmutableArray.Create((FieldSymbol)new SynthesizedFieldSymbol(this, systemBooleanType, "IncludesInternals", isPublic: true, isReadOnly: true));
            _constructors = ImmutableArray.Create((MethodSymbol)new SynthesizedEmbeddedAttributeConstructorWithBodySymbol(this, (MethodSymbol m) => ImmutableArray.Create(SynthesizedParameterSymbol.Create(m, TypeWithAnnotations.Create(systemBooleanType), 0, RefKind.None)), GenerateConstructorBody));
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
            return new AttributeUsageInfo(AttributeTargets.Module, allowMultiple: false, inherited: false);
        }

        private void GenerateConstructorBody(SyntheticBoundNodeFactory factory, ArrayBuilder<BoundStatement> statements, ImmutableArray<ParameterSymbol> parameters)
        {
            statements.Add(factory.ExpressionStatement(factory.AssignmentExpression(factory.Field(factory.This(), _fields.Single()), factory.Parameter(parameters.Single()))));
        }
    }
}
