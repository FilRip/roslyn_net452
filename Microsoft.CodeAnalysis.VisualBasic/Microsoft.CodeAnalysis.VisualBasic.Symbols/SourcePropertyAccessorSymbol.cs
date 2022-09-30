using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SourcePropertyAccessorSymbol : SourceMethodSymbol
	{
		protected readonly SourcePropertySymbol m_property;

		private readonly string _name;

		private string _lazyMetadataName;

		private ImmutableArray<MethodSymbol> _lazyExplicitImplementations;

		private ImmutableArray<ParameterSymbol> _lazyParameters;

		private TypeSymbol _lazyReturnType;

		private static readonly Binder.CheckParameterModifierDelegate s_checkParameterModifierCallback = CheckParameterModifier;

		public override MethodSymbol OverriddenMethod => m_property.GetAccessorOverride(MethodKind == MethodKind.PropertyGet);

		internal override OverriddenMembersResult<MethodSymbol> OverriddenMembers => OverriddenMembersResult<MethodSymbol>.Empty;

		public override bool IsImplicitlyDeclared
		{
			get
			{
				if (m_property.IsCustomProperty)
				{
					return base.IsImplicitlyDeclared;
				}
				return true;
			}
		}

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
		{
			get
			{
				if (!m_property.IsCustomProperty)
				{
					return ImmutableArray<SyntaxReference>.Empty;
				}
				return base.DeclaringSyntaxReferences;
			}
		}

		public override string Name => _name;

		public override string MetadataName
		{
			get
			{
				if (_lazyMetadataName == null)
				{
					MethodSymbol overriddenMethod = OverriddenMethod;
					if ((object)overriddenMethod != null)
					{
						Interlocked.CompareExchange(ref _lazyMetadataName, overriddenMethod.MetadataName, null);
					}
					else
					{
						Interlocked.CompareExchange(ref _lazyMetadataName, _name, null);
					}
				}
				return _lazyMetadataName;
			}
		}

		public override Accessibility DeclaredAccessibility
		{
			get
			{
				Accessibility localAccessibility = LocalAccessibility;
				if (localAccessibility != 0)
				{
					return localAccessibility;
				}
				return m_property.DeclaredAccessibility;
			}
		}

		public override TypeSymbol ReturnType
		{
			get
			{
				TypeSymbol lazyReturnType = _lazyReturnType;
				if ((object)lazyReturnType == null)
				{
					BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
					SourceModuleSymbol containingSourceModule = base.ContainingSourceModule;
					SyntaxNodeOrToken errorLocation = default(SyntaxNodeOrToken);
					lazyReturnType = GetReturnType(containingSourceModule, ref errorLocation, instance);
					if (!Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(errorLocation, SyntaxKind.None))
					{
						ArrayBuilder<TypeParameterDiagnosticInfo> instance2 = ArrayBuilder<TypeParameterDiagnosticInfo>.GetInstance();
						ArrayBuilder<TypeParameterDiagnosticInfo> useSiteDiagnosticsBuilder = null;
						ConstraintsHelper.CheckAllConstraints(lazyReturnType, instance2, ref useSiteDiagnosticsBuilder, new CompoundUseSiteInfo<AssemblySymbol>(instance, containingSourceModule.ContainingAssembly));
						if (useSiteDiagnosticsBuilder != null)
						{
							instance2.AddRange(useSiteDiagnosticsBuilder);
						}
						ArrayBuilder<TypeParameterDiagnosticInfo>.Enumerator enumerator = instance2.GetEnumerator();
						while (enumerator.MoveNext())
						{
							instance.Add(enumerator.Current.UseSiteInfo, errorLocation.GetLocation());
						}
						instance2.Free();
					}
					containingSourceModule.AtomicStoreReferenceAndDiagnostics(ref _lazyReturnType, lazyReturnType, instance);
					instance.Free();
					lazyReturnType = _lazyReturnType;
				}
				return lazyReturnType;
			}
		}

		public override ImmutableArray<ParameterSymbol> Parameters
		{
			get
			{
				ImmutableArray<ParameterSymbol> lazyParameters = _lazyParameters;
				if (lazyParameters.IsDefault)
				{
					BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
					SourceModuleSymbol containingSourceModule = base.ContainingSourceModule;
					lazyParameters = GetParameters(containingSourceModule, instance);
					ImmutableArray<ParameterSymbol>.Enumerator enumerator = lazyParameters.GetEnumerator();
					while (enumerator.MoveNext())
					{
						ParameterSymbol current = enumerator.Current;
						if (current.Locations.Length > 0)
						{
							ConstraintsHelper.CheckAllConstraints(current.Type, current.Locations[0], instance, new CompoundUseSiteInfo<AssemblySymbol>(instance, containingSourceModule.ContainingAssembly));
						}
					}
					containingSourceModule.AtomicStoreArrayAndDiagnostics(ref _lazyParameters, lazyParameters, instance);
					instance.Free();
					lazyParameters = _lazyParameters;
				}
				return lazyParameters;
			}
		}

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => ImmutableArray<TypeParameterSymbol>.Empty;

		public override Symbol AssociatedSymbol => m_property;

		internal override bool ShadowsExplicitly => m_property.ShadowsExplicitly;

		public override bool IsExtensionMethod => false;

		internal override bool MayBeReducibleExtensionMethod => false;

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations
		{
			get
			{
				if (_lazyExplicitImplementations.IsDefault)
				{
					ImmutableInterlocked.InterlockedCompareExchange(ref _lazyExplicitImplementations, m_property.GetAccessorImplementations(MethodKind == MethodKind.PropertyGet), default(ImmutableArray<MethodSymbol>));
				}
				return _lazyExplicitImplementations;
			}
		}

		public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers => OverriddenMethod?.ReturnTypeCustomModifiers ?? ((MethodKind == MethodKind.PropertySet) ? ImmutableArray<CustomModifier>.Empty : m_property.TypeCustomModifiers);

		protected override SourcePropertySymbol BoundReturnTypeAttributesSource
		{
			get
			{
				if (MethodKind != MethodKind.PropertyGet)
				{
					return null;
				}
				return m_property;
			}
		}

		internal Accessibility LocalAccessibility => base.DeclaredAccessibility;

		internal bool HasDebuggerHiddenAttribute => GetDecodedWellKnownAttributeData()?.IsPropertyAccessorWithDebuggerHiddenAttribute ?? false;

		internal override bool GenerateDebugInfoImpl
		{
			get
			{
				if (!m_property.IsAutoProperty)
				{
					return base.GenerateDebugInfoImpl;
				}
				return false;
			}
		}

		internal SourcePropertyAccessorSymbol(SourcePropertySymbol propertySymbol, string name, SourceMemberFlags flags, SyntaxReference syntaxRef, ImmutableArray<Location> locations)
			: base(propertySymbol.ContainingSourceType, (SourceMemberFlagsExtensions.ToMethodKind(flags) == MethodKind.PropertyGet) ? flags : (flags & ~SourceMemberFlags.Iterator), syntaxRef, locations)
		{
			m_property = propertySymbol;
			_name = name;
		}

		private static ImmutableArray<ParameterSymbol> SynthesizeAutoGetterParameters(SourcePropertyAccessorSymbol getter, SourcePropertySymbol propertySymbol)
		{
			if (propertySymbol.ParameterCount == 0)
			{
				return ImmutableArray<ParameterSymbol>.Empty;
			}
			ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance(propertySymbol.ParameterCount);
			propertySymbol.CloneParametersForAccessor(getter, instance);
			return instance.ToImmutableAndFree();
		}

		private static ImmutableArray<ParameterSymbol> SynthesizeAutoSetterParameters(SourcePropertyAccessorSymbol setter, SourcePropertySymbol propertySymbol)
		{
			ParameterSymbol item = SynthesizedParameterSymbol.CreateSetAccessorValueParameter(setter, propertySymbol, propertySymbol.IsAutoProperty ? "AutoPropertyValue" : "Value");
			if (propertySymbol.ParameterCount == 0)
			{
				return ImmutableArray.Create(item);
			}
			ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance(propertySymbol.ParameterCount + 1);
			propertySymbol.CloneParametersForAccessor(setter, instance);
			instance.Add(item);
			return instance.ToImmutableAndFree();
		}

		internal static SourcePropertyAccessorSymbol CreatePropertyAccessor(SourcePropertySymbol propertySymbol, SourceMemberFlags kindFlags, SourceMemberFlags propertyFlags, Binder binder, AccessorBlockSyntax blockSyntax, DiagnosticBag diagnostics)
		{
			MethodBaseSyntax blockStatement = blockSyntax.BlockStatement;
			MemberModifiers memberModifiers = binder.DecodeModifiers(blockStatement.Modifiers, SourceMemberFlags.AllAccessibilityModifiers, ERRID.ERR_BadPropertyAccessorFlags, Accessibility.NotApplicable, diagnostics);
			if ((memberModifiers.FoundFlags & SourceMemberFlags.Private) != 0)
			{
				propertyFlags &= ~SourceMemberFlags.Overridable;
			}
			if ((memberModifiers.FoundFlags & SourceMemberFlags.Protected) != 0)
			{
				switch (propertySymbol.ContainingType.TypeKind)
				{
				case TypeKind.Struct:
					binder.ReportModifierError(blockStatement.Modifiers, ERRID.ERR_StructCantUseVarSpecifier1, diagnostics, SyntaxKind.ProtectedKeyword);
					memberModifiers = new MemberModifiers(memberModifiers.FoundFlags & ~SourceMemberFlags.Protected, memberModifiers.ComputedFlags & ~SourceMemberFlags.AccessibilityMask);
					break;
				case TypeKind.Module:
					binder.ReportModifierError(blockStatement.Modifiers, ERRID.ERR_BadFlagsOnStdModuleProperty1, diagnostics, SyntaxKind.ProtectedKeyword);
					break;
				}
			}
			SourceMemberFlags sourceMemberFlags = memberModifiers.AllFlags | kindFlags | propertyFlags;
			MethodKind methodKind = SourceMemberFlagsExtensions.ToMethodKind(kindFlags);
			if (methodKind == MethodKind.PropertySet)
			{
				sourceMemberFlags |= SourceMemberFlags.Dim;
			}
			return new SourcePropertyAccessorSymbol(propertySymbol, Binder.GetAccessorName(propertySymbol.Name, methodKind, SymbolExtensions.IsCompilationOutputWinMdObj(propertySymbol)), sourceMemberFlags, binder.GetSyntaxReference(blockStatement), ImmutableArray.Create(blockStatement.DeclarationKeyword.GetLocation()));
		}

		private TypeSymbol GetReturnType(SourceModuleSymbol sourceModule, ref SyntaxNodeOrToken errorLocation, BindingDiagnosticBag diagBag)
		{
			switch (MethodKind)
			{
			case MethodKind.PropertyGet:
			{
				TypeSymbol typeSymbol = ((PropertySymbol)AssociatedSymbol).Type;
				MethodSymbol overriddenMethod = OverriddenMethod;
				if ((object)overriddenMethod != null && TypeSymbolExtensions.IsSameTypeIgnoringAll(overriddenMethod.ReturnType, typeSymbol))
				{
					typeSymbol = overriddenMethod.ReturnType;
				}
				return typeSymbol;
			}
			case MethodKind.PropertySet:
				return BinderBuilder.CreateBinderForType(sourceModule, base.SyntaxTree, m_property.ContainingSourceType).GetSpecialType(SpecialType.System_Void, base.DeclarationSyntax, diagBag);
			default:
				throw ExceptionUtilities.Unreachable;
			}
		}

		private ImmutableArray<ParameterSymbol> GetParameters(SourceModuleSymbol sourceModule, BindingDiagnosticBag diagBag)
		{
			if (m_property.IsCustomProperty)
			{
				Binder containingBinder = BinderBuilder.CreateBinderForType(sourceModule, base.SyntaxTree, m_property.ContainingSourceType);
				containingBinder = new LocationSpecificBinder(BindingLocation.PropertyAccessorSignature, this, containingBinder);
				return BindParameters(m_property, this, Locations.FirstOrDefault(), containingBinder, base.BlockSyntax.BlockStatement.ParameterList, diagBag);
			}
			return (MethodKind == MethodKind.PropertyGet) ? SynthesizeAutoGetterParameters(this, m_property) : SynthesizeAutoSetterParameters(this, m_property);
		}

		internal override LexicalSortKey GetLexicalSortKey()
		{
			if (!m_property.IsCustomProperty)
			{
				return m_property.GetLexicalSortKey();
			}
			return base.GetLexicalSortKey();
		}

		protected override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
		{
			if (m_property.IsCustomProperty)
			{
				return OneOrMany.Create(base.AttributeDeclarationSyntaxList);
			}
			return default(OneOrMany<SyntaxList<AttributeListSyntax>>);
		}

		protected override OneOrMany<SyntaxList<AttributeListSyntax>> GetReturnTypeAttributeDeclarations()
		{
			return default(OneOrMany<SyntaxList<AttributeListSyntax>>);
		}

		private static ImmutableArray<ParameterSymbol> BindParameters(SourcePropertySymbol propertySymbol, SourcePropertyAccessorSymbol method, Location location, Binder binder, ParameterListSyntax parameterListOpt, BindingDiagnosticBag diagnostics)
		{
			int length = propertySymbol.Parameters.Length;
			bool flag = method.MethodKind == MethodKind.PropertySet;
			SeparatedSyntaxList<ParameterSyntax> syntax = ((parameterListOpt == null || !flag) ? default(SeparatedSyntaxList<ParameterSyntax>) : parameterListOpt.Parameters);
			bool flag2 = flag && syntax.Count == 0;
			ArrayBuilder<ParameterSymbol> instance = ArrayBuilder<ParameterSymbol>.GetInstance(length + syntax.Count + (flag2 ? 1 : 0));
			propertySymbol.CloneParametersForAccessor(method, instance);
			if (syntax.Count > 0)
			{
				binder.DecodeParameterList(method, isFromLambda: false, SourceMemberFlags.None, syntax, instance, s_checkParameterModifierCallback, diagnostics);
				ParameterSymbol parameterSymbol = instance[length];
				if (!CaseInsensitiveComparison.Equals(parameterSymbol.Name, "Value"))
				{
					ParameterSyntax syntax2 = syntax[0];
					Binder.CheckParameterNameNotDuplicate(instance, length, syntax2, parameterSymbol, diagnostics);
				}
				if (syntax.Count == 1)
				{
					TypeSymbol type = propertySymbol.Type;
					ParameterSymbol thisParam = instance[instance.Count - 1];
					TypeSymbol type2 = thisParam.Type;
					if (!TypeSymbolExtensions.IsSameTypeIgnoringAll(type, type2))
					{
						if (!TypeSymbolExtensions.IsErrorType(type) && !TypeSymbolExtensions.IsErrorType(type2))
						{
							diagnostics.Add(ERRID.ERR_SetValueNotPropertyType, thisParam.Locations[0]);
						}
					}
					else
					{
						MethodSymbol overriddenMethod = method.OverriddenMethod;
						if ((object)overriddenMethod != null)
						{
							ParameterSymbol parameterSymbol2 = overriddenMethod.Parameters[instance.Count - 1];
							if (TypeSymbolExtensions.IsSameTypeIgnoringAll(parameterSymbol2.Type, type2) && CustomModifierUtils.CopyParameterCustomModifiers(parameterSymbol2, ref thisParam))
							{
								instance[instance.Count - 1] = thisParam;
							}
						}
					}
				}
				else
				{
					diagnostics.Add(ERRID.ERR_SetHasOnlyOneParam, location);
				}
			}
			else if (flag2)
			{
				ParameterSymbol item = SynthesizedParameterSymbol.CreateSetAccessorValueParameter(method, propertySymbol, "Value");
				instance.Add(item);
			}
			return instance.ToImmutableAndFree();
		}

		private static SourceParameterFlags CheckParameterModifier(Symbol container, SyntaxToken token, SourceParameterFlags flag, BindingDiagnosticBag diagnostics)
		{
			if (flag != SourceParameterFlags.ByVal)
			{
				Location location = token.GetLocation();
				diagnostics.Add(ERRID.ERR_SetHasToBeByVal1, location, token.ToString());
				return flag & SourceParameterFlags.ByVal;
			}
			return SourceParameterFlags.ByVal;
		}

		internal override BoundBlock GetBoundMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, ref Binder methodBodyBinder = null)
		{
			if (m_property.IsAutoProperty)
			{
				return SynthesizedPropertyAccessorHelper.GetBoundMethodBody(this, m_property.AssociatedField, ref methodBodyBinder);
			}
			return base.GetBoundMethodBody(compilationState, diagnostics, ref methodBodyBinder);
		}

		internal override void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation> arguments)
		{
			if (arguments.SymbolPart == AttributeLocation.None && arguments.Attribute.IsTargetAttribute(this, AttributeDescription.DebuggerHiddenAttribute))
			{
				arguments.GetOrCreateData<MethodWellKnownAttributeData>().IsPropertyAccessorWithDebuggerHiddenAttribute = true;
			}
			base.DecodeWellKnownAttribute(ref arguments);
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			if (m_property.IsAutoProperty)
			{
				VisualBasicCompilation declaringCompilation = DeclaringCompilation;
				Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor));
			}
		}
	}
}
