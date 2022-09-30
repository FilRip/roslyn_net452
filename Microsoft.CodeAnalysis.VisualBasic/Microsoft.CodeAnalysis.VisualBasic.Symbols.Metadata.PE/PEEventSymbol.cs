using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Reflection;
using System.Reflection.Metadata;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE
{
	internal sealed class PEEventSymbol : EventSymbol
	{
		private readonly string _name;

		private readonly EventAttributes _flags;

		private readonly PENamedTypeSymbol _containingType;

		private readonly EventDefinitionHandle _handle;

		private readonly TypeSymbol _eventType;

		private readonly PEMethodSymbol _addMethod;

		private readonly PEMethodSymbol _removeMethod;

		private readonly PEMethodSymbol _raiseMethod;

		private ImmutableArray<VisualBasicAttributeData> _lazyCustomAttributes;

		private Tuple<CultureInfo, string> _lazyDocComment;

		private CachedUseSiteInfo<AssemblySymbol> _lazyCachedUseSiteInfo;

		private ObsoleteAttributeData _lazyObsoleteAttributeData;

		private const int s_unsetAccessibility = -1;

		private int _lazyDeclaredAccessibility;

		public override bool IsWindowsRuntimeEvent
		{
			get
			{
				NamedTypeSymbol eventRegistrationTokenType = ((PEModuleSymbol)ContainingModule).GetEventRegistrationTokenType();
				if (TypeSymbol.Equals(_addMethod.ReturnType, eventRegistrationTokenType, TypeCompareKind.ConsiderEverything) && _addMethod.ParameterCount == 1 && _removeMethod.ParameterCount == 1)
				{
					return TypeSymbol.Equals(_removeMethod.Parameters[0].Type, eventRegistrationTokenType, TypeCompareKind.ConsiderEverything);
				}
				return false;
			}
		}

		public override Symbol ContainingSymbol => _containingType;

		public override NamedTypeSymbol ContainingType => _containingType;

		public override string Name => _name;

		internal EventAttributes EventFlags => _flags;

		internal override bool HasSpecialName => (_flags & EventAttributes.SpecialName) != 0;

		internal override bool HasRuntimeSpecialName => (_flags & EventAttributes.RTSpecialName) != 0;

		internal EventDefinitionHandle Handle => _handle;

		public override Accessibility DeclaredAccessibility
		{
			get
			{
				if (_lazyDeclaredAccessibility == -1)
				{
					Accessibility declaredAccessibilityFromAccessors = PEPropertyOrEventHelpers.GetDeclaredAccessibilityFromAccessors(AddMethod, RemoveMethod);
					Interlocked.CompareExchange(ref _lazyDeclaredAccessibility, (int)declaredAccessibilityFromAccessors, -1);
				}
				return (Accessibility)_lazyDeclaredAccessibility;
			}
		}

		public override bool IsMustOverride => AddMethod?.IsMustOverride ?? false;

		public override bool IsNotOverridable => AddMethod?.IsNotOverridable ?? false;

		public override bool IsOverridable => AddMethod?.IsOverridable ?? false;

		public override bool IsOverrides => AddMethod?.IsOverrides ?? false;

		public override bool IsShared => AddMethod?.IsShared ?? true;

		public override TypeSymbol Type => _eventType;

		public override MethodSymbol AddMethod => _addMethod;

		public override MethodSymbol RemoveMethod => _removeMethod;

		public override MethodSymbol RaiseMethod => _raiseMethod;

		internal override FieldSymbol AssociatedField => null;

		public override ImmutableArray<Location> Locations => _containingType.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		internal override ObsoleteAttributeData ObsoleteAttributeData
		{
			get
			{
				ObsoleteAttributeHelpers.InitializeObsoleteDataFromMetadata(ref _lazyObsoleteAttributeData, _handle, (PEModuleSymbol)ContainingModule);
				return _lazyObsoleteAttributeData;
			}
		}

		public override ImmutableArray<EventSymbol> ExplicitInterfaceImplementations
		{
			get
			{
				if (AddMethod.ExplicitInterfaceImplementations.Length == 0 && RemoveMethod.ExplicitInterfaceImplementations.Length == 0)
				{
					return ImmutableArray<EventSymbol>.Empty;
				}
				ISet<EventSymbol> eventsForExplicitlyImplementedAccessor = PEPropertyOrEventHelpers.GetEventsForExplicitlyImplementedAccessor(AddMethod);
				eventsForExplicitlyImplementedAccessor.IntersectWith(PEPropertyOrEventHelpers.GetEventsForExplicitlyImplementedAccessor(RemoveMethod));
				ArrayBuilder<EventSymbol> instance = ArrayBuilder<EventSymbol>.GetInstance();
				foreach (EventSymbol item in eventsForExplicitlyImplementedAccessor)
				{
					instance.Add(item);
				}
				return instance.ToImmutableAndFree();
			}
		}

		internal override VisualBasicCompilation DeclaringCompilation => null;

		internal PEEventSymbol(PEModuleSymbol moduleSymbol, PENamedTypeSymbol containingType, EventDefinitionHandle handle, PEMethodSymbol addMethod, PEMethodSymbol removeMethod, PEMethodSymbol raiseMethod)
		{
			_lazyCachedUseSiteInfo = CachedUseSiteInfo<AssemblySymbol>.Uninitialized;
			_lazyObsoleteAttributeData = ObsoleteAttributeData.Uninitialized;
			_lazyDeclaredAccessibility = -1;
			_containingType = containingType;
			PEModule module = moduleSymbol.Module;
			EntityHandle type = default(EntityHandle);
			try
			{
				module.GetEventDefPropsOrThrow(handle, out _name, out _flags, out type);
			}
			catch (BadImageFormatException ex)
			{
				ProjectData.SetProjectError(ex);
				BadImageFormatException mrEx = ex;
				if (_name == null)
				{
					_name = string.Empty;
				}
				_lazyCachedUseSiteInfo.Initialize(ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedEvent1, this));
				if (type.IsNil)
				{
					_eventType = new UnsupportedMetadataTypeSymbol(mrEx);
				}
				ProjectData.ClearProjectError();
			}
			_addMethod = addMethod;
			_removeMethod = removeMethod;
			_raiseMethod = raiseMethod;
			_handle = handle;
			if ((object)_eventType == null)
			{
				MetadataDecoder metadataDecoder = new MetadataDecoder(moduleSymbol, containingType);
				_eventType = metadataDecoder.GetTypeOfToken(type);
				_eventType = TupleTypeDecoder.DecodeTupleTypesIfApplicable(_eventType, handle, moduleSymbol);
			}
			if ((object)_addMethod != null)
			{
				_addMethod.SetAssociatedEvent(this, MethodKind.EventAdd);
			}
			if ((object)_removeMethod != null)
			{
				_removeMethod.SetAssociatedEvent(this, MethodKind.EventRemove);
			}
			if ((object)_raiseMethod != null)
			{
				_raiseMethod.SetAssociatedEvent(this, MethodKind.EventRaise);
			}
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			if (_lazyCustomAttributes.IsDefault)
			{
				((PEModuleSymbol)ContainingModule).LoadCustomAttributes(_handle, ref _lazyCustomAttributes);
			}
			return _lazyCustomAttributes;
		}

		internal override IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(ModuleCompilationState compilationState)
		{
			return GetAttributes();
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return PEDocumentationCommentUtils.GetDocumentationComment(this, _containingType.ContainingPEModule, preferredCulture, cancellationToken, ref _lazyDocComment);
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
	}
}
