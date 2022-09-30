using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedRecordDeconstruct : SynthesizedRecordOrdinaryMethod
    {
        private readonly SynthesizedRecordConstructor _ctor;

        private readonly ImmutableArray<Symbol> _positionalMembers;

        public SynthesizedRecordDeconstruct(SourceMemberContainerTypeSymbol containingType, SynthesizedRecordConstructor ctor, ImmutableArray<Symbol> positionalMembers, int memberOffset, BindingDiagnosticBag diagnostics)
            : base(containingType, "Deconstruct", hasBody: true, memberOffset, diagnostics)
        {
            _ctor = ctor;
            _positionalMembers = positionalMembers;
        }

        protected override DeclarationModifiers MakeDeclarationModifiers(DeclarationModifiers allowedModifiers, BindingDiagnosticBag diagnostics)
        {
            return DeclarationModifiers.Public;
        }

        protected override (TypeWithAnnotations ReturnType, ImmutableArray<ParameterSymbol> Parameters, bool IsVararg, ImmutableArray<TypeParameterConstraintClause> DeclaredConstraintsForOverrideOrImplementation) MakeParametersAndBindReturnType(BindingDiagnosticBag diagnostics)
        {
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            Location returnTypeLocation = ReturnTypeLocation;
            return (TypeWithAnnotations.Create(Binder.GetSpecialType(declaringCompilation, SpecialType.System_Void, returnTypeLocation, diagnostics)), _ctor.Parameters.SelectAsArray((Func<ParameterSymbol, ImmutableArray<Location>, ParameterSymbol>)((ParameterSymbol param, ImmutableArray<Location> locations) => new SourceSimpleParameterSymbol(this, param.TypeWithAnnotations, param.Ordinal, RefKind.Out, param.Name, locations)), Locations), false, ImmutableArray<TypeParameterConstraintClause>.Empty);
        }

        protected override int GetParameterCountFromSyntax()
        {
            return _ctor.ParameterCount;
        }

        internal override void GenerateMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
        {
            SyntheticBoundNodeFactory syntheticBoundNodeFactory = new SyntheticBoundNodeFactory(this, ContainingType.GetNonNullSyntaxNode(), compilationState, diagnostics);
            if (ParameterCount != _positionalMembers.Length)
            {
                syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
                return;
            }
            ArrayBuilder<BoundStatement> instance = ArrayBuilder<BoundStatement>.GetInstance(_positionalMembers.Length + 1);
            for (int i = 0; i < _positionalMembers.Length; i++)
            {
                ParameterSymbol parameterSymbol = Parameters[i];
                Symbol symbol = _positionalMembers[i];
                TypeSymbol type;
                if (!(symbol is PropertySymbol propertySymbol))
                {
                    if (!(symbol is FieldSymbol fieldSymbol))
                    {
                        throw ExceptionUtilities.Unreachable;
                    }
                    type = fieldSymbol.Type;
                }
                else
                {
                    type = propertySymbol.Type;
                }
                TypeSymbol t = type;
                if (!parameterSymbol.Type.Equals(t, TypeCompareKind.AllIgnoreOptions))
                {
                    instance.Free();
                    syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.ThrowNull());
                    return;
                }
                if (!(symbol is PropertySymbol property))
                {
                    if (symbol is FieldSymbol f)
                    {
                        instance.Add(syntheticBoundNodeFactory.Assignment(syntheticBoundNodeFactory.Parameter(parameterSymbol), syntheticBoundNodeFactory.Field(syntheticBoundNodeFactory.This(), f)));
                    }
                }
                else
                {
                    instance.Add(syntheticBoundNodeFactory.Assignment(syntheticBoundNodeFactory.Parameter(parameterSymbol), syntheticBoundNodeFactory.Property(syntheticBoundNodeFactory.This(), property)));
                }
            }
            instance.Add(syntheticBoundNodeFactory.Return());
            syntheticBoundNodeFactory.CloseMethod(syntheticBoundNodeFactory.Block(instance.ToImmutableAndFree()));
        }
    }
}
