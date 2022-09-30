using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
{
	internal sealed class RetargetingEventSymbol : EventSymbol
	{
		private readonly RetargetingModuleSymbol _retargetingModule;

		private readonly EventSymbol _underlyingEvent;

		private ImmutableArray<CustomModifier> _lazyCustomModifiers;

		private ImmutableArray<VisualBasicAttributeData> _lazyCustomAttributes;

		private ImmutableArray<EventSymbol> _lazyExplicitInterfaceImplementations;

		private CachedUseSiteInfo<AssemblySymbol> _lazyCachedUseSiteInfo;

		private RetargetingModuleSymbol.RetargetingSymbolTranslator RetargetingTranslator => _retargetingModule.RetargetingTranslator;

		public EventSymbol UnderlyingEvent => _underlyingEvent;

		public RetargetingModuleSymbol RetargetingModule => _retargetingModule;

		public override Symbol ContainingSymbol => RetargetingTranslator.Retarget(_underlyingEvent.ContainingSymbol);

		public override Accessibility DeclaredAccessibility => _underlyingEvent.DeclaredAccessibility;

		public override MethodSymbol AddMethod
		{
			get
			{
				if ((object)_underlyingEvent.AddMethod != null)
				{
					return RetargetingTranslator.Retarget(_underlyingEvent.AddMethod);
				}
				return null;
			}
		}

		internal override FieldSymbol AssociatedField
		{
			get
			{
				if ((object)_underlyingEvent.AssociatedField != null)
				{
					return RetargetingTranslator.Retarget(_underlyingEvent.AssociatedField);
				}
				return null;
			}
		}

		public override ImmutableArray<EventSymbol> ExplicitInterfaceImplementations
		{
			get
			{
				if (_lazyExplicitInterfaceImplementations.IsDefault)
				{
					ImmutableInterlocked.InterlockedCompareExchange(ref _lazyExplicitInterfaceImplementations, RetargetExplicitInterfaceImplementations(), default(ImmutableArray<EventSymbol>));
				}
				return _lazyExplicitInterfaceImplementations;
			}
		}

		public override bool IsMustOverride => _underlyingEvent.IsMustOverride;

		public override bool IsNotOverridable => _underlyingEvent.IsNotOverridable;

		public override bool IsOverridable => _underlyingEvent.IsOverridable;

		public override bool IsOverrides => _underlyingEvent.IsOverrides;

		public override bool IsShared => _underlyingEvent.IsShared;

		public override ImmutableArray<Location> Locations => _underlyingEvent.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _underlyingEvent.DeclaringSyntaxReferences;

		public override MethodSymbol RaiseMethod
		{
			get
			{
				if ((object)_underlyingEvent.RaiseMethod != null)
				{
					return RetargetingTranslator.Retarget(_underlyingEvent.RaiseMethod);
				}
				return null;
			}
		}

		public override MethodSymbol RemoveMethod
		{
			get
			{
				if ((object)_underlyingEvent.RemoveMethod != null)
				{
					return RetargetingTranslator.Retarget(_underlyingEvent.RemoveMethod);
				}
				return null;
			}
		}

		public override TypeSymbol Type => RetargetingTranslator.Retarget(_underlyingEvent.Type, RetargetOptions.RetargetPrimitiveTypesByTypeCode);

		public override string Name => _underlyingEvent.Name;

		public override string MetadataName => _underlyingEvent.MetadataName;

		internal override bool HasSpecialName => _underlyingEvent.HasSpecialName;

		internal override bool HasRuntimeSpecialName => _underlyingEvent.HasRuntimeSpecialName;

		internal override ObsoleteAttributeData ObsoleteAttributeData => _underlyingEvent.ObsoleteAttributeData;

		public override bool IsWindowsRuntimeEvent => _underlyingEvent.IsWindowsRuntimeEvent;

		internal override VisualBasicCompilation DeclaringCompilation => null;

		public RetargetingEventSymbol(RetargetingModuleSymbol retargetingModule, EventSymbol underlyingEvent)
		{
			_lazyCachedUseSiteInfo = CachedUseSiteInfo<AssemblySymbol>.Uninitialized;
			if (underlyingEvent is RetargetingEventSymbol)
			{
				throw new ArgumentException();
			}
			_retargetingModule = retargetingModule;
			_underlyingEvent = underlyingEvent;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return RetargetingTranslator.GetRetargetedAttributes(_underlyingEvent, ref _lazyCustomAttributes);
		}

		internal override IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(ModuleCompilationState compilationState)
		{
			return RetargetingTranslator.RetargetAttributes(_underlyingEvent.GetCustomAttributesToEmit(compilationState));
		}

		private ImmutableArray<EventSymbol> RetargetExplicitInterfaceImplementations()
		{
			ImmutableArray<EventSymbol> explicitInterfaceImplementations = UnderlyingEvent.ExplicitInterfaceImplementations;
			if (explicitInterfaceImplementations.IsEmpty)
			{
				return explicitInterfaceImplementations;
			}
			ArrayBuilder<EventSymbol> instance = ArrayBuilder<EventSymbol>.GetInstance();
			int num = explicitInterfaceImplementations.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				EventSymbol eventSymbol = RetargetingModule.RetargetingTranslator.RetargetImplementedEvent(explicitInterfaceImplementations[i]);
				if ((object)eventSymbol != null)
				{
					instance.Add(eventSymbol);
				}
			}
			return instance.ToImmutableAndFree();
		}

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			AssemblySymbol primaryDependency = base.PrimaryDependency;
			if (!_lazyCachedUseSiteInfo.IsInitialized)
			{
				_lazyCachedUseSiteInfo.Initialize(primaryDependency, CalculateUseSiteInfo());
			}
			return _lazyCachedUseSiteInfo.ToUseSiteInfo(primaryDependency);
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _underlyingEvent.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}
	}
}
