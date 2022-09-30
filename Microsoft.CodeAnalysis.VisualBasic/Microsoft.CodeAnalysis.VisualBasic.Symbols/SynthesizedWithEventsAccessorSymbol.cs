using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SynthesizedWithEventsAccessorSymbol : SynthesizedPropertyAccessorBase<PropertySymbol>
	{
		private ImmutableArray<ParameterSymbol> _lazyParameters;

		private ImmutableArray<MethodSymbol> _lazyExplicitImplementations;

		protected PropertySymbol ContainingProperty => m_propertyOrEvent;

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations
		{
			get
			{
				if (_lazyExplicitImplementations.IsDefault)
				{
					ImmutableInterlocked.InterlockedInitialize(ref _lazyExplicitImplementations, GetExplicitInterfaceImplementations());
				}
				return _lazyExplicitImplementations;
			}
		}

		public override MethodSymbol OverriddenMethod => ContainingProperty.GetAccessorOverride(MethodKind == MethodKind.PropertyGet);

		internal override SyntaxNode Syntax
		{
			get
			{
				if (ContainingProperty is SourcePropertySymbol sourcePropertySymbol)
				{
					return sourcePropertySymbol.Syntax;
				}
				return null;
			}
		}

		internal override FieldSymbol BackingFieldSymbol
		{
			get
			{
				if (ContainingProperty is SourcePropertySymbol sourcePropertySymbol)
				{
					return sourcePropertySymbol.AssociatedField;
				}
				return null;
			}
		}

		public override ImmutableArray<ParameterSymbol> Parameters
		{
			get
			{
				if (_lazyParameters.IsDefault)
				{
					ImmutableInterlocked.InterlockedInitialize(ref _lazyParameters, GetParameters());
				}
				return _lazyParameters;
			}
		}

		internal sealed override bool GenerateDebugInfoImpl => false;

		protected SynthesizedWithEventsAccessorSymbol(SourceMemberContainerTypeSymbol container, PropertySymbol property)
			: base((NamedTypeSymbol)container, property)
		{
		}

		private ImmutableArray<MethodSymbol> GetExplicitInterfaceImplementations()
		{
			if (ContainingProperty is SourcePropertySymbol sourcePropertySymbol)
			{
				return sourcePropertySymbol.GetAccessorImplementations(MethodKind == MethodKind.PropertyGet);
			}
			return ImmutableArray<MethodSymbol>.Empty;
		}

		protected abstract ImmutableArray<ParameterSymbol> GetParameters();

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			VisualBasicCompilation declaringCompilation = DeclaringCompilation;
			Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
		}

		internal sealed override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			throw ExceptionUtilities.Unreachable;
		}
	}
}
