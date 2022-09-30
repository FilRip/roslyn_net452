using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class SynthesizedClosureMethod : SynthesizedMethodBaseSymbol, ISynthesizedMethodBodyImplementationSymbol, ISymbolInternal
    {
        private readonly ImmutableArray<NamedTypeSymbol> _structEnvironments;

        internal readonly DebugId LambdaId;

        internal MethodSymbol TopLevelMethod { get; }

        protected override ImmutableArray<ParameterSymbol> BaseMethodParameters => BaseMethod.Parameters;

        protected override ImmutableArray<TypeSymbol> ExtraSynthesizedRefParameters => ImmutableArray<TypeSymbol>.CastUp(_structEnvironments);

        internal int ExtraSynthesizedParameterCount
        {
            get
            {
                if (!_structEnvironments.IsDefault)
                {
                    return _structEnvironments.Length;
                }
                return 0;
            }
        }

        internal override bool InheritsBaseMethodAttributes => true;

        internal override bool GenerateDebugInfo => !IsAsync;

        internal override bool IsExpressionBodied => false;

        IMethodSymbolInternal? ISynthesizedMethodBodyImplementationSymbol.Method => TopLevelMethod;

        bool ISynthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency => true;

        public ClosureKind ClosureKind { get; }

        internal SynthesizedClosureMethod(NamedTypeSymbol containingType, ImmutableArray<SynthesizedClosureEnvironment> structEnvironments, ClosureKind closureKind, MethodSymbol topLevelMethod, DebugId topLevelMethodId, MethodSymbol originalMethod, SyntaxReference blockSyntax, DebugId lambdaId)
            : base(containingType, originalMethod, blockSyntax, originalMethod.DeclaringSyntaxReferences[0].GetLocation(), (originalMethod is LocalFunctionSymbol) ? MakeName(topLevelMethod.Name, originalMethod.Name, topLevelMethodId, closureKind, lambdaId) : MakeName(topLevelMethod.Name, topLevelMethodId, closureKind, lambdaId), MakeDeclarationModifiers(closureKind, originalMethod))
        {
            TopLevelMethod = topLevelMethod;
            ClosureKind = closureKind;
            LambdaId = lambdaId;
            SynthesizedClosureEnvironment synthesizedClosureEnvironment = ContainingType as SynthesizedClosureEnvironment;
            TypeMap typeMap;
            ImmutableArray<TypeParameterSymbol> newTypeParameters;
            ImmutableArray<TypeParameterSymbol> oldTypeParameters;
            switch (closureKind)
            {
                case ClosureKind.Singleton:
                case ClosureKind.General:
                    typeMap = synthesizedClosureEnvironment.TypeMap.WithConcatAlphaRename(originalMethod, this, out newTypeParameters, out oldTypeParameters, synthesizedClosureEnvironment.OriginalContainingMethodOpt);
                    break;
                case ClosureKind.Static:
                case ClosureKind.ThisOnly:
                    typeMap = TypeMap.Empty.WithConcatAlphaRename(originalMethod, this, out newTypeParameters, out oldTypeParameters);
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(closureKind);
            }
            if (!structEnvironments.IsDefaultOrEmpty && newTypeParameters.Length != 0)
            {
                ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
                ImmutableArray<SynthesizedClosureEnvironment>.Enumerator enumerator = structEnvironments.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SynthesizedClosureEnvironment current = enumerator.Current;
                    NamedTypeSymbol item;
                    if (current.Arity == 0)
                    {
                        item = current;
                    }
                    else
                    {
                        ImmutableArray<TypeParameterSymbol> constructedFromTypeParameters = current.ConstructedFromTypeParameters;
                        ImmutableArray<TypeParameterSymbol> immutableArray = typeMap.SubstituteTypeParameters(constructedFromTypeParameters);
                        item = current.Construct(immutableArray);
                    }
                    instance.Add(item);
                }
                _structEnvironments = instance.ToImmutableAndFree();
            }
            else
            {
                _structEnvironments = ImmutableArray<NamedTypeSymbol>.CastUp(structEnvironments);
            }
            AssignTypeMapAndTypeParameters(typeMap, newTypeParameters);
        }

        private static DeclarationModifiers MakeDeclarationModifiers(ClosureKind closureKind, MethodSymbol originalMethod)
        {
            DeclarationModifiers declarationModifiers = ((closureKind == ClosureKind.ThisOnly) ? DeclarationModifiers.Private : DeclarationModifiers.Internal);
            if (closureKind == ClosureKind.Static)
            {
                declarationModifiers |= DeclarationModifiers.Static;
            }
            if (originalMethod.IsAsync)
            {
                declarationModifiers |= DeclarationModifiers.Async;
            }
            if (originalMethod.IsExtern)
            {
                declarationModifiers |= DeclarationModifiers.Extern;
            }
            return declarationModifiers;
        }

        private static string MakeName(string topLevelMethodName, string localFunctionName, DebugId topLevelMethodId, ClosureKind closureKind, DebugId lambdaId)
        {
            return GeneratedNames.MakeLocalFunctionName(topLevelMethodName, localFunctionName, (closureKind == ClosureKind.General) ? (-1) : topLevelMethodId.Ordinal, topLevelMethodId.Generation, lambdaId.Ordinal, lambdaId.Generation);
        }

        private static string MakeName(string topLevelMethodName, DebugId topLevelMethodId, ClosureKind closureKind, DebugId lambdaId)
        {
            return GeneratedNames.MakeLambdaMethodName(topLevelMethodName, (closureKind == ClosureKind.General) ? (-1) : topLevelMethodId.Ordinal, topLevelMethodId.Generation, lambdaId.Ordinal, lambdaId.Generation);
        }

        internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
        {
            return TopLevelMethod.CalculateLocalSyntaxOffset(localPosition, localTree);
        }
    }
}
