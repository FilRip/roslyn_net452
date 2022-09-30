using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedEmbeddedNativeIntegerAttributeSymbol : SynthesizedEmbeddedAttributeSymbolBase
    {
        private readonly ImmutableArray<FieldSymbol> _fields;

        private readonly ImmutableArray<MethodSymbol> _constructors;

        private readonly TypeSymbol _boolType;

        private const string FieldName = "TransformFlags";

        public override ImmutableArray<MethodSymbol> Constructors => _constructors;

        internal override bool IsRecord => false;

        internal override bool IsRecordStruct => false;

        public SynthesizedEmbeddedNativeIntegerAttributeSymbol(string name, NamespaceSymbol containingNamespace, ModuleSymbol containingModule, NamedTypeSymbol systemAttributeType, TypeSymbol boolType)
            : base(name, containingNamespace, containingModule, systemAttributeType)
        {
            _boolType = boolType;
            TypeWithAnnotations boolArrayType = TypeWithAnnotations.Create(ArrayTypeSymbol.CreateSZArray(boolType.ContainingAssembly, TypeWithAnnotations.Create(boolType)));
            _fields = ImmutableArray.Create((FieldSymbol)new SynthesizedFieldSymbol(this, boolArrayType.Type, "TransformFlags", isPublic: true, isReadOnly: true));
            _constructors = ImmutableArray.Create(new SynthesizedEmbeddedAttributeConstructorWithBodySymbol(this, (MethodSymbol m) => ImmutableArray<ParameterSymbol>.Empty, delegate (SyntheticBoundNodeFactory f, ArrayBuilder<BoundStatement> s, ImmutableArray<ParameterSymbol> p)
            {
                GenerateParameterlessConstructorBody(f, s);
            }), (MethodSymbol)new SynthesizedEmbeddedAttributeConstructorWithBodySymbol(this, (MethodSymbol m) => ImmutableArray.Create(SynthesizedParameterSymbol.Create(m, boolArrayType, 0, RefKind.None)), delegate (SyntheticBoundNodeFactory f, ArrayBuilder<BoundStatement> s, ImmutableArray<ParameterSymbol> p)
            {
                GenerateBoolArrayConstructorBody(f, s, p);
            }));
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

        private void GenerateParameterlessConstructorBody(SyntheticBoundNodeFactory factory, ArrayBuilder<BoundStatement> statements)
        {
            statements.Add(factory.ExpressionStatement(factory.AssignmentExpression(factory.Field(factory.This(), _fields.Single()), factory.Array(_boolType, ImmutableArray.Create((BoundExpression)factory.Literal(value: true))))));
        }

        private void GenerateBoolArrayConstructorBody(SyntheticBoundNodeFactory factory, ArrayBuilder<BoundStatement> statements, ImmutableArray<ParameterSymbol> parameters)
        {
            statements.Add(factory.ExpressionStatement(factory.AssignmentExpression(factory.Field(factory.This(), _fields.Single()), factory.Parameter(parameters.Single()))));
        }
    }
}
