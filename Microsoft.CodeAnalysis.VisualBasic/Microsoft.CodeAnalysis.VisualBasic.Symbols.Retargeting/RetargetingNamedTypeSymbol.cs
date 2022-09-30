using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
{
	internal sealed class RetargetingNamedTypeSymbol : InstanceTypeSymbol
	{
		private readonly RetargetingModuleSymbol _retargetingModule;

		private readonly NamedTypeSymbol _underlyingType;

		private ImmutableArray<TypeParameterSymbol> _lazyTypeParameters;

		private TypeSymbol _lazyCoClass;

		private ImmutableArray<VisualBasicAttributeData> _lazyCustomAttributes;

		private CachedUseSiteInfo<AssemblySymbol> _lazyCachedUseSiteInfo;

		private RetargetingModuleSymbol.RetargetingSymbolTranslator RetargetingTranslator => _retargetingModule.RetargetingTranslator;

		public NamedTypeSymbol UnderlyingNamedType => _underlyingType;

		public override bool IsImplicitlyDeclared => _underlyingType.IsImplicitlyDeclared;

		public override int Arity => _underlyingType.Arity;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters
		{
			get
			{
				if (_lazyTypeParameters.IsDefault)
				{
					if (Arity == 0)
					{
						_lazyTypeParameters = ImmutableArray<TypeParameterSymbol>.Empty;
					}
					else
					{
						ImmutableInterlocked.InterlockedCompareExchange(ref _lazyTypeParameters, RetargetingTranslator.Retarget(_underlyingType.TypeParameters), default(ImmutableArray<TypeParameterSymbol>));
					}
				}
				return _lazyTypeParameters;
			}
		}

		public override NamedTypeSymbol ConstructedFrom => this;

		public override NamedTypeSymbol EnumUnderlyingType
		{
			get
			{
				NamedTypeSymbol enumUnderlyingType = _underlyingType.EnumUnderlyingType;
				if ((object)enumUnderlyingType != null)
				{
					return RetargetingTranslator.Retarget(enumUnderlyingType, RetargetOptions.RetargetPrimitiveTypesByTypeCode);
				}
				return null;
			}
		}

		public override bool MightContainExtensionMethods => _underlyingType.MightContainExtensionMethods;

		internal override bool HasCodeAnalysisEmbeddedAttribute => _underlyingType.HasCodeAnalysisEmbeddedAttribute;

		internal override bool HasVisualBasicEmbeddedAttribute => _underlyingType.HasVisualBasicEmbeddedAttribute;

		internal override bool IsExtensibleInterfaceNoUseSiteDiagnostics => _underlyingType.IsExtensibleInterfaceNoUseSiteDiagnostics;

		internal override bool IsWindowsRuntimeImport => _underlyingType.IsWindowsRuntimeImport;

		internal override bool ShouldAddWinRTMembers => _underlyingType.ShouldAddWinRTMembers;

		internal override bool IsComImport => _underlyingType.IsComImport;

		internal override TypeSymbol CoClassType
		{
			get
			{
				if ((object)_lazyCoClass == ErrorTypeSymbol.UnknownResultType)
				{
					TypeSymbol typeSymbol = _underlyingType.CoClassType;
					if ((object)typeSymbol != null)
					{
						typeSymbol = RetargetingTranslator.Retarget(typeSymbol, RetargetOptions.RetargetPrimitiveTypesByName);
					}
					Interlocked.CompareExchange(ref _lazyCoClass, typeSymbol, ErrorTypeSymbol.UnknownResultType);
				}
				return _lazyCoClass;
			}
		}

		internal override bool HasDeclarativeSecurity => _underlyingType.HasDeclarativeSecurity;

		public override string Name => _underlyingType.Name;

		public override string MetadataName => _underlyingType.MetadataName;

		internal override bool MangleName => _underlyingType.MangleName;

		internal override bool HasSpecialName => _underlyingType.HasSpecialName;

		public override bool IsSerializable => _underlyingType.IsSerializable;

		internal override TypeLayout Layout => _underlyingType.Layout;

		internal override CharSet MarshallingCharSet => _underlyingType.MarshallingCharSet;

		public override IEnumerable<string> MemberNames => _underlyingType.MemberNames;

		public override Accessibility DeclaredAccessibility => _underlyingType.DeclaredAccessibility;

		public override TypeKind TypeKind => _underlyingType.TypeKind;

		internal override bool IsInterface => _underlyingType.IsInterface;

		public override Symbol ContainingSymbol => RetargetingTranslator.Retarget(_underlyingType.ContainingSymbol);

		public override ImmutableArray<Location> Locations => _underlyingType.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _underlyingType.DeclaringSyntaxReferences;

		public override bool IsMustInherit => _underlyingType.IsMustInherit;

		internal override bool IsMetadataAbstract => _underlyingType.IsMetadataAbstract;

		public override bool IsNotInheritable => _underlyingType.IsNotInheritable;

		internal override bool IsMetadataSealed => _underlyingType.IsMetadataSealed;

		internal override ObsoleteAttributeData ObsoleteAttributeData => _underlyingType.ObsoleteAttributeData;

		public override AssemblySymbol ContainingAssembly => _retargetingModule.ContainingAssembly;

		public override ModuleSymbol ContainingModule => _retargetingModule;

		internal override string DefaultPropertyName => _underlyingType.DefaultPropertyName;

		internal override VisualBasicCompilation DeclaringCompilation => null;

		public RetargetingNamedTypeSymbol(RetargetingModuleSymbol retargetingModule, NamedTypeSymbol underlyingType)
		{
			_lazyCoClass = ErrorTypeSymbol.UnknownResultType;
			_lazyCachedUseSiteInfo = CachedUseSiteInfo<AssemblySymbol>.Uninitialized;
			if (underlyingType is RetargetingNamedTypeSymbol)
			{
				throw new ArgumentException();
			}
			_retargetingModule = retargetingModule;
			_underlyingType = underlyingType;
		}

		internal override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			return _underlyingType.GetAppliedConditionalSymbols();
		}

		internal override AttributeUsageInfo GetAttributeUsageInfo()
		{
			return _underlyingType.GetAttributeUsageInfo();
		}

		internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			return _underlyingType.GetSecurityInformation();
		}

		internal override void AppendProbableExtensionMethods(string name, ArrayBuilder<MethodSymbol> methods)
		{
			int count = methods.Count;
			_underlyingType.AppendProbableExtensionMethods(name, methods);
			int num = methods.Count - 1;
			for (int i = count; i <= num; i++)
			{
				methods[i] = RetargetingTranslator.Retarget(methods[i]);
			}
		}

		internal override void BuildExtensionMethodsMap(Dictionary<string, ArrayBuilder<MethodSymbol>> map, NamespaceSymbol appendThrough)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override void GetExtensionMethods(ArrayBuilder<MethodSymbol> methods, NamespaceSymbol appendThrough, string Name)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override void AddExtensionMethodLookupSymbolsInfo(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
		{
			_underlyingType.AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder, this);
		}

		internal override bool AddExtensionMethodLookupSymbolsInfoViabilityCheck(MethodSymbol method, LookupOptions options, LookupSymbolsInfo nameSet, Binder originalBinder)
		{
			return base.AddExtensionMethodLookupSymbolsInfoViabilityCheck(RetargetingTranslator.Retarget(method), options, nameSet, originalBinder);
		}

		internal override void AddExtensionMethodLookupSymbolsInfo(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder, NamedTypeSymbol appendThrough)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override string GetEmittedNamespaceName()
		{
			return _underlyingType.GetEmittedNamespaceName();
		}

		public override ImmutableArray<Symbol> GetMembers()
		{
			return RetargetingTranslator.Retarget(_underlyingType.GetMembers());
		}

		internal override ImmutableArray<Symbol> GetMembersUnordered()
		{
			return RetargetingTranslator.Retarget(_underlyingType.GetMembersUnordered());
		}

		public override ImmutableArray<Symbol> GetMembers(string name)
		{
			return RetargetingTranslator.Retarget(_underlyingType.GetMembers(name));
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_68_GetFieldsToEmit))]
		internal override IEnumerable<FieldSymbol> GetFieldsToEmit()
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_68_GetFieldsToEmit(-2)
			{
				_0024VB_0024Me = this
			};
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_69_GetMethodsToEmit))]
		internal override IEnumerable<MethodSymbol> GetMethodsToEmit()
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_69_GetMethodsToEmit(-2)
			{
				_0024VB_0024Me = this
			};
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_70_GetPropertiesToEmit))]
		internal override IEnumerable<PropertySymbol> GetPropertiesToEmit()
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_70_GetPropertiesToEmit(-2)
			{
				_0024VB_0024Me = this
			};
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_71_GetEventsToEmit))]
		internal override IEnumerable<EventSymbol> GetEventsToEmit()
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_71_GetEventsToEmit(-2)
			{
				_0024VB_0024Me = this
			};
		}

		internal override ImmutableArray<NamedTypeSymbol> GetTypeMembersUnordered()
		{
			return RetargetingTranslator.Retarget(_underlyingType.GetTypeMembersUnordered());
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
		{
			return RetargetingTranslator.Retarget(_underlyingType.GetTypeMembers());
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
		{
			return RetargetingTranslator.Retarget(_underlyingType.GetTypeMembers(name));
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
		{
			return RetargetingTranslator.Retarget(_underlyingType.GetTypeMembers(name, arity));
		}

		internal override NamedTypeSymbol MakeDeclaredBase(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			NamedTypeSymbol declaredBase = _underlyingType.GetDeclaredBase(basesBeingResolved);
			if ((object)declaredBase == null)
			{
				return null;
			}
			return RetargetingTranslator.Retarget(declaredBase, RetargetOptions.RetargetPrimitiveTypesByName);
		}

		internal override IEnumerable<NamedTypeSymbol> GetInterfacesToEmit()
		{
			return RetargetingTranslator.Retarget(_underlyingType.GetInterfacesToEmit());
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeDeclaredInterfaces(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			ImmutableArray<NamedTypeSymbol> declaredInterfacesNoUseSiteDiagnostics = _underlyingType.GetDeclaredInterfacesNoUseSiteDiagnostics(basesBeingResolved);
			return RetargetingTranslator.Retarget(declaredInterfacesNoUseSiteDiagnostics);
		}

		private static ErrorTypeSymbol CyclicInheritanceError(DiagnosticInfo diag)
		{
			return new ExtendedErrorTypeSymbol(diag, reportErrorWhenReferenced: true);
		}

		internal override NamedTypeSymbol MakeAcyclicBaseType(BindingDiagnosticBag diagnostics)
		{
			DiagnosticInfo dependencyDiagnosticsForImportedClass = BaseTypeAnalysis.GetDependencyDiagnosticsForImportedClass(this);
			if (dependencyDiagnosticsForImportedClass != null)
			{
				return CyclicInheritanceError(dependencyDiagnosticsForImportedClass);
			}
			NamedTypeSymbol namedTypeSymbol = GetDeclaredBase(default(BasesBeingResolved));
			if ((object)namedTypeSymbol == null)
			{
				NamedTypeSymbol baseTypeNoUseSiteDiagnostics = _underlyingType.BaseTypeNoUseSiteDiagnostics;
				if ((object)baseTypeNoUseSiteDiagnostics != null)
				{
					namedTypeSymbol = RetargetingTranslator.Retarget(baseTypeNoUseSiteDiagnostics, RetargetOptions.RetargetPrimitiveTypesByName);
				}
			}
			return namedTypeSymbol;
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeAcyclicInterfaces(BindingDiagnosticBag diagnostics)
		{
			ImmutableArray<NamedTypeSymbol> declaredInterfacesNoUseSiteDiagnostics = GetDeclaredInterfacesNoUseSiteDiagnostics(default(BasesBeingResolved));
			if (!IsInterface)
			{
				return declaredInterfacesNoUseSiteDiagnostics;
			}
			return (from t in declaredInterfacesNoUseSiteDiagnostics
				select new VB_0024AnonymousType_2<NamedTypeSymbol, DiagnosticInfo>(t, BaseTypeAnalysis.GetDependencyDiagnosticsForImportedBaseInterface(this, t)) into _0024VB_0024It
				select (_0024VB_0024It.diag != null) ? CyclicInheritanceError(_0024VB_0024It.diag) : _0024VB_0024It.t).AsImmutable();
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return RetargetingTranslator.GetRetargetedAttributes(_underlyingType, ref _lazyCustomAttributes);
		}

		internal override IEnumerable<VisualBasicAttributeData> GetCustomAttributesToEmit(ModuleCompilationState compilationState)
		{
			return RetargetingTranslator.RetargetAttributes(_underlyingType.GetCustomAttributesToEmit(compilationState));
		}

		internal override NamedTypeSymbol LookupMetadataType(ref MetadataTypeName emittedTypeName)
		{
			return RetargetingTranslator.Retarget(_underlyingType.LookupMetadataType(ref emittedTypeName), RetargetOptions.RetargetPrimitiveTypesByName);
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

		internal override void GenerateDeclarationErrors(CancellationToken cancellationToken)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override bool GetGuidString(ref string guidString)
		{
			return _underlyingType.GetGuidString(ref guidString);
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _underlyingType.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_119_GetSynthesizedWithEventsOverrides))]
		internal override IEnumerable<PropertySymbol> GetSynthesizedWithEventsOverrides()
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_119_GetSynthesizedWithEventsOverrides(-2)
			{
				_0024VB_0024Me = this
			};
		}
	}
}
