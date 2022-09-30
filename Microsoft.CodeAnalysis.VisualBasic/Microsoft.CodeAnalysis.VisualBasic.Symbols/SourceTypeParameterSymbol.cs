using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SourceTypeParameterSymbol : SubstitutableTypeParameterSymbol
	{
		private readonly int _ordinal;

		private readonly string _name;

		private ImmutableArray<TypeParameterConstraint> _lazyConstraints;

		private ImmutableArray<TypeSymbol> _lazyConstraintTypes;

		public override int Ordinal => _ordinal;

		public override string Name => _name;

		public override bool HasConstructorConstraint
		{
			get
			{
				EnsureAllConstraintsAreResolved();
				ImmutableArray<TypeParameterConstraint>.Enumerator enumerator = _lazyConstraints.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsConstructorConstraint)
					{
						return true;
					}
				}
				return false;
			}
		}

		public override bool HasReferenceTypeConstraint
		{
			get
			{
				EnsureAllConstraintsAreResolved();
				ImmutableArray<TypeParameterConstraint>.Enumerator enumerator = _lazyConstraints.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsReferenceTypeConstraint)
					{
						return true;
					}
				}
				return false;
			}
		}

		public override bool HasValueTypeConstraint
		{
			get
			{
				EnsureAllConstraintsAreResolved();
				ImmutableArray<TypeParameterConstraint>.Enumerator enumerator = _lazyConstraints.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsValueTypeConstraint)
					{
						return true;
					}
				}
				return false;
			}
		}

		internal override ImmutableArray<TypeSymbol> ConstraintTypesNoUseSiteDiagnostics
		{
			get
			{
				EnsureAllConstraintsAreResolved();
				return _lazyConstraintTypes;
			}
		}

		protected abstract ImmutableArray<TypeParameterSymbol> ContainerTypeParameters { get; }

		public override bool IsImplicitlyDeclared => ContainingSymbol.IsImplicitlyDeclared;

		protected SourceTypeParameterSymbol(int ordinal, string name)
		{
			_ordinal = ordinal;
			_name = name;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return ImmutableArray<VisualBasicAttributeData>.Empty;
		}

		internal override ImmutableArray<TypeParameterConstraint> GetConstraints()
		{
			EnsureAllConstraintsAreResolved();
			return _lazyConstraints;
		}

		internal override void ResolveConstraints(ConsList<TypeParameterSymbol> inProgress)
		{
			if (!_lazyConstraintTypes.IsDefault)
			{
				return;
			}
			BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
			ImmutableArray<TypeParameterConstraint> declaredConstraints = GetDeclaredConstraints(instance);
			DirectConstraintConflictKind reportConflicts = DirectConstraintConflictKind.DuplicateTypeConstraint | (ReportRedundantConstraints() ? DirectConstraintConflictKind.RedundantConstraint : DirectConstraintConflictKind.None);
			ArrayBuilder<TypeParameterDiagnosticInfo> instance2 = ArrayBuilder<TypeParameterDiagnosticInfo>.GetInstance();
			declaredConstraints = ConstraintsHelper.RemoveDirectConstraintConflicts(this, declaredConstraints, inProgress.Prepend(this), reportConflicts, instance2);
			ImmutableInterlocked.InterlockedInitialize(ref _lazyConstraints, declaredConstraints);
			if (ImmutableInterlocked.InterlockedInitialize(ref _lazyConstraintTypes, TypeParameterSymbol.GetConstraintTypesOnly(declaredConstraints)))
			{
				ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder = null;
				ConstraintsHelper.ReportIndirectConstraintConflicts(this, instance2, ref useSiteDiagnosticsBuilder);
				if (useSiteDiagnosticsBuilder != null)
				{
					instance2.AddRange(useSiteDiagnosticsBuilder);
				}
				ArrayBuilder<TypeParameterDiagnosticInfo>.Enumerator enumerator = instance2.GetEnumerator();
				while (enumerator.MoveNext())
				{
					TypeParameterDiagnosticInfo current = enumerator.Current;
					Location location = GetLocation(current);
					instance.Add(current.UseSiteInfo, location);
				}
				CheckConstraintTypeConstraints(declaredConstraints, instance);
				((SourceModuleSymbol)ContainingModule).AddDeclarationDiagnostics(instance);
			}
			instance2.Free();
			instance.Free();
		}

		internal override void EnsureAllConstraintsAreResolved()
		{
			if (_lazyConstraintTypes.IsDefault)
			{
				TypeParameterSymbol.EnsureAllConstraintsAreResolved(ContainerTypeParameters);
			}
		}

		protected abstract ImmutableArray<TypeParameterConstraint> GetDeclaredConstraints(BindingDiagnosticBag diagnostics);

		protected abstract bool ReportRedundantConstraints();

		protected static Location GetSymbolLocation(SyntaxReference syntaxRef)
		{
			SyntaxNode syntax = syntaxRef.GetSyntax();
			return syntaxRef.SyntaxTree.GetLocation(((TypeParameterSyntax)syntax).Identifier.Span);
		}

		private void CheckConstraintTypeConstraints(ImmutableArray<TypeParameterConstraint> constraints, BindingDiagnosticBag diagnostics)
		{
			AssemblySymbol containingAssembly = ContainingAssembly;
			ImmutableArray<TypeParameterConstraint>.Enumerator enumerator = constraints.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeParameterConstraint current = enumerator.Current;
				TypeSymbol typeConstraint = current.TypeConstraint;
				if ((object)typeConstraint != null)
				{
					Location locationOpt = current.LocationOpt;
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, containingAssembly);
					TypeSymbolExtensions.AddUseSiteInfo(typeConstraint, ref useSiteInfo);
					if (!diagnostics.Add(locationOpt, useSiteInfo))
					{
						ConstraintsHelper.CheckAllConstraints(typeConstraint, locationOpt, diagnostics, new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, containingAssembly));
					}
				}
			}
		}

		private static Location GetLocation(TypeParameterDiagnosticInfo diagnostic)
		{
			Location locationOpt = diagnostic.Constraint.LocationOpt;
			if ((object)locationOpt != null)
			{
				return locationOpt;
			}
			ImmutableArray<Location> locations = diagnostic.TypeParameter.Locations;
			if (locations.Length > 0)
			{
				return locations[0];
			}
			return null;
		}
	}
}
