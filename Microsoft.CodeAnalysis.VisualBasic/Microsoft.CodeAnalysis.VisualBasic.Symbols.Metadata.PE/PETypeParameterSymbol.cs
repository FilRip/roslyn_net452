using System;
using System.Collections.Immutable;
using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
{
	internal sealed class PETypeParameterSymbol : SubstitutableTypeParameterSymbol
	{
		private readonly Symbol _containingSymbol;

		private readonly GenericParameterHandle _handle;

		private ImmutableArray<VisualBasicAttributeData> _lazyCustomAttributes;

		private readonly string _name;

		private readonly ushort _ordinal;

		private readonly GenericParameterAttributes _flags;

		private ImmutableArray<TypeSymbol> _lazyConstraintTypes;

		private CachedUseSiteInfo<AssemblySymbol> _lazyCachedBoundsUseSiteInfo;

		public override TypeParameterKind TypeParameterKind
		{
			get
			{
				if (ContainingSymbol.Kind != SymbolKind.Method)
				{
					return TypeParameterKind.Type;
				}
				return TypeParameterKind.Method;
			}
		}

		public override int Ordinal => _ordinal;

		public override string Name => _name;

		internal GenericParameterHandle Handle => _handle;

		public override Symbol ContainingSymbol => _containingSymbol;

		public override AssemblySymbol ContainingAssembly => _containingSymbol.ContainingAssembly;

		internal override ImmutableArray<TypeSymbol> ConstraintTypesNoUseSiteDiagnostics
		{
			get
			{
				EnsureAllConstraintsAreResolved();
				return _lazyConstraintTypes;
			}
		}

		public override ImmutableArray<Location> Locations => _containingSymbol.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public override bool HasConstructorConstraint => (_flags & GenericParameterAttributes.DefaultConstructorConstraint) != 0;

		public override bool HasReferenceTypeConstraint => (_flags & GenericParameterAttributes.ReferenceTypeConstraint) != 0;

		public override bool HasValueTypeConstraint => (_flags & GenericParameterAttributes.NotNullableValueTypeConstraint) != 0;

		public override VarianceKind Variance => (VarianceKind)(_flags & GenericParameterAttributes.VarianceMask);

		internal override VisualBasicCompilation DeclaringCompilation => null;

		internal PETypeParameterSymbol(PEModuleSymbol moduleSymbol, PENamedTypeSymbol definingNamedType, ushort ordinal, GenericParameterHandle handle)
			: this(moduleSymbol, (Symbol)definingNamedType, ordinal, handle)
		{
		}

		internal PETypeParameterSymbol(PEModuleSymbol moduleSymbol, PEMethodSymbol definingMethod, ushort ordinal, GenericParameterHandle handle)
			: this(moduleSymbol, (Symbol)definingMethod, ordinal, handle)
		{
		}

		private PETypeParameterSymbol(PEModuleSymbol moduleSymbol, Symbol definingSymbol, ushort ordinal, GenericParameterHandle handle)
		{
			_lazyCachedBoundsUseSiteInfo = CachedUseSiteInfo<AssemblySymbol>.Uninitialized;
			_containingSymbol = definingSymbol;
			GenericParameterAttributes flags = default(GenericParameterAttributes);
			try
			{
				moduleSymbol.Module.GetGenericParamPropsOrThrow(handle, out _name, out flags);
			}
			catch (BadImageFormatException ex)
			{
				ProjectData.SetProjectError(ex);
				BadImageFormatException ex2 = ex;
				if (_name == null)
				{
					_name = string.Empty;
				}
				_lazyCachedBoundsUseSiteInfo.Initialize(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedType1, this));
				ProjectData.ClearProjectError();
			}
			_flags = (((flags & GenericParameterAttributes.NotNullableValueTypeConstraint) == 0) ? flags : (flags & ~GenericParameterAttributes.DefaultConstructorConstraint));
			_ordinal = ordinal;
			_handle = handle;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			if (_lazyCustomAttributes.IsDefault)
			{
				((PEModuleSymbol)ContainingModule).LoadCustomAttributes(_handle, ref _lazyCustomAttributes);
			}
			return _lazyCustomAttributes;
		}

		private ImmutableArray<TypeParameterConstraint> GetDeclaredConstraints()
		{
			ArrayBuilder<TypeParameterConstraint> instance = ArrayBuilder<TypeParameterConstraint>.GetInstance();
			if (HasConstructorConstraint)
			{
				instance.Add(new TypeParameterConstraint(TypeParameterConstraintKind.Constructor, null));
			}
			if (HasReferenceTypeConstraint)
			{
				instance.Add(new TypeParameterConstraint(TypeParameterConstraintKind.ReferenceType, null));
			}
			if (HasValueTypeConstraint)
			{
				instance.Add(new TypeParameterConstraint(TypeParameterConstraintKind.ValueType, null));
			}
			PEMethodSymbol pEMethodSymbol = null;
			PENamedTypeSymbol pENamedTypeSymbol;
			if (_containingSymbol.Kind == SymbolKind.Method)
			{
				pEMethodSymbol = (PEMethodSymbol)_containingSymbol;
				pENamedTypeSymbol = (PENamedTypeSymbol)pEMethodSymbol.ContainingSymbol;
			}
			else
			{
				pENamedTypeSymbol = (PENamedTypeSymbol)_containingSymbol;
			}
			PEModuleSymbol containingPEModule = pENamedTypeSymbol.ContainingPEModule;
			MetadataReader metadataReader = containingPEModule.Module.MetadataReader;
			GenericParameterConstraintHandleCollection genericParameterConstraintHandleCollection;
			try
			{
				genericParameterConstraintHandleCollection = metadataReader.GetGenericParameter(_handle).GetConstraints();
			}
			catch (BadImageFormatException ex)
			{
				ProjectData.SetProjectError(ex);
				BadImageFormatException ex2 = ex;
				genericParameterConstraintHandleCollection = default(GenericParameterConstraintHandleCollection);
				_lazyCachedBoundsUseSiteInfo.InterlockedCompareExchange(null, new UseSiteInfo<AssemblySymbol>(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedType1, this)));
				ProjectData.ClearProjectError();
			}
			if (genericParameterConstraintHandleCollection.Count > 0)
			{
				MetadataDecoder metadataDecoder = (((object)pEMethodSymbol == null) ? new MetadataDecoder(containingPEModule, pENamedTypeSymbol) : new MetadataDecoder(containingPEModule, pEMethodSymbol));
				foreach (GenericParameterConstraintHandle item in genericParameterConstraintHandleCollection)
				{
					EntityHandle type = metadataReader.GetGenericParameterConstraint(item).Type;
					TypeSymbol typeOfToken = metadataDecoder.GetTypeOfToken(type);
					if ((_flags & GenericParameterAttributes.NotNullableValueTypeConstraint) == 0 || typeOfToken.SpecialType != SpecialType.System_ValueType)
					{
						typeOfToken = TupleTypeDecoder.DecodeTupleTypesIfApplicable(typeOfToken, item, containingPEModule);
						instance.Add(new TypeParameterConstraint(typeOfToken, null));
					}
				}
			}
			return instance.ToImmutableAndFree();
		}

		internal override void EnsureAllConstraintsAreResolved()
		{
			if (_lazyConstraintTypes.IsDefault)
			{
				TypeParameterSymbol.EnsureAllConstraintsAreResolved((_containingSymbol.Kind == SymbolKind.Method) ? ((PEMethodSymbol)_containingSymbol).TypeParameters : ((PENamedTypeSymbol)_containingSymbol).TypeParameters);
			}
		}

		internal override void ResolveConstraints(ConsList<TypeParameterSymbol> inProgress)
		{
			if (!_lazyConstraintTypes.IsDefault)
			{
				return;
			}
			ArrayBuilder<TypeParameterDiagnosticInfo> instance = ArrayBuilder<TypeParameterDiagnosticInfo>.GetInstance();
			if (_containingSymbol.Kind == SymbolKind.Method)
			{
				_ = ((MethodSymbol)_containingSymbol).IsOverrides;
			}
			else
				_ = 0;
			ImmutableArray<TypeParameterConstraint> constraints = ConstraintsHelper.RemoveDirectConstraintConflicts(this, GetDeclaredConstraints(), inProgress.Prepend(this), DirectConstraintConflictKind.None, instance);
			AssemblySymbol primaryDependency = base.PrimaryDependency;
			UseSiteInfo<AssemblySymbol> useSiteInfo = new UseSiteInfo<AssemblySymbol>(primaryDependency);
			ArrayBuilder<TypeParameterDiagnosticInfo>.Enumerator enumerator = instance.GetEnumerator();
			while (enumerator.MoveNext())
			{
				useSiteInfo = MergeUseSiteInfo(second: enumerator.Current.UseSiteInfo, first: useSiteInfo);
				if (useSiteInfo.DiagnosticInfo != null)
				{
					break;
				}
			}
			instance.Free();
			_lazyCachedBoundsUseSiteInfo.InterlockedCompareExchange(primaryDependency, useSiteInfo);
			ImmutableInterlocked.InterlockedInitialize(ref _lazyConstraintTypes, TypeParameterSymbol.GetConstraintTypesOnly(constraints));
		}

		internal override UseSiteInfo<AssemblySymbol> GetConstraintsUseSiteInfo()
		{
			EnsureAllConstraintsAreResolved();
			return _lazyCachedBoundsUseSiteInfo.ToUseSiteInfo(base.PrimaryDependency);
		}
	}
}
