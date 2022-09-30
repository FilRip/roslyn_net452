using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class SynthesizedLambdaMethod : SynthesizedMethod, ISynthesizedMethodBodyImplementationSymbol
	{
		private readonly LambdaSymbol _lambda;

		private readonly ImmutableArray<ParameterSymbol> _parameters;

		private readonly ImmutableArray<Location> _locations;

		private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;

		private readonly TypeSubstitution _typeMap;

		private readonly MethodSymbol _topLevelMethod;

		public override Accessibility DeclaredAccessibility
		{
			get
			{
				if (!(base.ContainingType is LambdaFrame))
				{
					return Accessibility.Private;
				}
				return Accessibility.Internal;
			}
		}

		internal override TypeSubstitution TypeMap => _typeMap;

		public MethodSymbol TopLevelMethod => _topLevelMethod;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => _typeParameters;

		public override ImmutableArray<TypeSymbol> TypeArguments
		{
			get
			{
				if (Arity > 0)
				{
					return StaticCast<TypeSymbol>.From(TypeParameters);
				}
				return ImmutableArray<TypeSymbol>.Empty;
			}
		}

		public override ImmutableArray<Location> Locations => _locations;

		public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

		public override TypeSymbol ReturnType => _lambda.ReturnType.InternalSubstituteTypeParameters(TypeMap).Type;

		public override bool IsShared => false;

		public override bool IsVararg => false;

		public override int Arity => _typeParameters.Length;

		internal override bool HasSpecialName => true;

		public override bool IsAsync => _lambda.IsAsync;

		public override bool IsIterator => _lambda.IsIterator;

		internal override bool GenerateDebugInfoImpl => _lambda.GenerateDebugInfoImpl;

		public bool HasMethodBodyDependency => true;

		public IMethodSymbolInternal Method => _topLevelMethod;

		internal SynthesizedLambdaMethod(InstanceTypeSymbol containingType, ClosureKind closureKind, MethodSymbol topLevelMethod, DebugId topLevelMethodId, BoundLambda lambdaNode, DebugId lambdaId, BindingDiagnosticBag diagnostics)
			: base(lambdaNode.Syntax, containingType, MakeName(topLevelMethodId, closureKind, lambdaNode.LambdaSymbol.SynthesizedKind, lambdaId), isShared: false)
		{
			_lambda = lambdaNode.LambdaSymbol;
			_locations = ImmutableArray.Create(lambdaNode.Syntax.GetLocation());
			if (!topLevelMethod.IsGenericMethod)
			{
				_typeMap = null;
				_typeParameters = ImmutableArray<TypeParameterSymbol>.Empty;
			}
			else if (containingType is LambdaFrame lambdaFrame)
			{
				_typeParameters = ImmutableArray<TypeParameterSymbol>.Empty;
				_typeMap = lambdaFrame.TypeMap;
			}
			else
			{
				_typeParameters = SynthesizedClonedTypeParameterSymbol.MakeTypeParameters(topLevelMethod.TypeParameters, this, LambdaFrame.CreateTypeParameter);
				_typeMap = TypeSubstitution.Create(topLevelMethod, topLevelMethod.TypeParameters, TypeArguments);
			}
			ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance();
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = _lambda.Parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				instance.Add(SynthesizedMethod.WithNewContainerAndType(this, current.Type.InternalSubstituteTypeParameters(TypeMap).Type, current));
			}
			_parameters = instance.ToImmutableAndFree();
			_topLevelMethod = topLevelMethod;
		}

		private static string MakeName(DebugId topLevelMethodId, ClosureKind closureKind, SynthesizedLambdaKind lambdaKind, DebugId lambdaId)
		{
			return GeneratedNames.MakeLambdaMethodName((closureKind == ClosureKind.General) ? (-1) : topLevelMethodId.Ordinal, topLevelMethodId.Generation, lambdaId.Ordinal, lambdaId.Generation, lambdaKind);
		}

		internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
		{
			return false;
		}

		internal MethodSymbol AsMember(NamedTypeSymbol constructedFrame)
		{
			if ((object)constructedFrame == base.ContainingType)
			{
				return this;
			}
			return (MethodSymbol)((SubstitutedNamedType)constructedFrame).GetMemberForDefinition(this);
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			if (!GenerateDebugInfoImpl)
			{
				Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.SynthesizeDebuggerHiddenAttribute());
			}
			if (IsAsync || IsIterator)
			{
				Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.SynthesizeStateMachineAttribute(this, compilationState));
				if (IsAsync)
				{
					Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.SynthesizeOptionalDebuggerStepThroughAttribute());
				}
			}
		}

		internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			return _topLevelMethod.CalculateLocalSyntaxOffset(localPosition, localTree);
		}
	}
}
