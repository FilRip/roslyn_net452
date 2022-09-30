using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SourcePropertySymbol : PropertySymbol, IAttributeTargetSymbol
	{
		[Flags]
		private enum StateFlags
		{
			SymbolDeclaredEvent = 1
		}

		private readonly SourceMemberContainerTypeSymbol _containingType;

		private readonly string _name;

		private string _lazyMetadataName;

		private readonly SyntaxReference _syntaxRef;

		private readonly SyntaxReference _blockRef;

		private readonly Location _location;

		private readonly SourceMemberFlags _flags;

		private TypeSymbol _lazyType;

		private ImmutableArray<ParameterSymbol> _lazyParameters;

		private MethodSymbol _getMethod;

		private MethodSymbol _setMethod;

		private FieldSymbol _backingField;

		private string _lazyDocComment;

		private string _lazyExpandedDocComment;

		private ParameterSymbol _lazyMeParameter;

		private CustomAttributesBag<VisualBasicAttributeData> _lazyCustomAttributesBag;

		private CustomAttributesBag<VisualBasicAttributeData> _lazyReturnTypeCustomAttributesBag;

		private ImmutableArray<PropertySymbol> _lazyImplementedProperties;

		private OverriddenMembersResult<PropertySymbol> _lazyOverriddenProperties;

		private int _lazyState;

		private static readonly SyntaxKind[] s_overridableModifierKinds = new SyntaxKind[1] { SyntaxKind.OverridableKeyword };

		private static readonly SyntaxKind[] s_accessibilityModifierKinds = new SyntaxKind[4]
		{
			SyntaxKind.PrivateKeyword,
			SyntaxKind.ProtectedKeyword,
			SyntaxKind.FriendKeyword,
			SyntaxKind.PublicKeyword
		};

		internal DeclarationStatementSyntax DeclarationSyntax
		{
			get
			{
				VisualBasicSyntaxNode visualBasicSyntax = VisualBasicExtensions.GetVisualBasicSyntax(_syntaxRef);
				if (visualBasicSyntax.Kind() == SyntaxKind.PropertyStatement)
				{
					return (PropertyStatementSyntax)visualBasicSyntax;
				}
				return (FieldDeclarationSyntax)visualBasicSyntax.Parent.Parent;
			}
		}

		public override bool ReturnsByRef => false;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override TypeSymbol Type
		{
			get
			{
				EnsureSignature();
				return _lazyType;
			}
		}

		public override string Name => _name;

		public override string MetadataName
		{
			get
			{
				if (_lazyMetadataName == null)
				{
					OverloadingHelper.SetMetadataNameForAllOverloads(_name, SymbolKind.Property, _containingType);
				}
				return _lazyMetadataName;
			}
		}

		public override Symbol ContainingSymbol => _containingType;

		public override NamedTypeSymbol ContainingType => _containingType;

		public SourceMemberContainerTypeSymbol ContainingSourceType => _containingType;

		public override ImmutableArray<Location> Locations => ImmutableArray.Create(_location);

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => Symbol.GetDeclaringSyntaxReferenceHelper(_syntaxRef);

		public AttributeLocation DefaultAttributeLocation => AttributeLocation.Property;

		internal override bool IsDirectlyExcludedFromCodeCoverage => GetDecodedWellKnownAttributeData()?.HasExcludeFromCodeCoverageAttribute ?? false;

		internal override bool HasSpecialName => GetDecodedWellKnownAttributeData()?.HasSpecialNameAttribute ?? false;

		internal MarshalPseudoCustomAttributeData ReturnTypeMarshallingInformation => GetDecodedReturnTypeWellKnownAttributeData()?.MarshallingInformation;

		public override bool IsMustOverride => (_flags & SourceMemberFlags.MustOverride) != 0;

		public override bool IsNotOverridable => (_flags & SourceMemberFlags.NotOverridable) != 0;

		public override bool IsOverridable => (_flags & SourceMemberFlags.Overridable) != 0;

		public override bool IsOverrides => (_flags & SourceMemberFlags.Overrides) != 0;

		public override bool IsShared => (_flags & SourceMemberFlags.Shared) != 0;

		public override bool IsDefault => (_flags & SourceMemberFlags.Default) != 0;

		public override bool IsWriteOnly => (_flags & SourceMemberFlags.WriteOnly) != 0;

		public override bool IsReadOnly => (_flags & SourceMemberFlags.ReadOnly) != 0;

		public override MethodSymbol GetMethod => _getMethod;

		public override MethodSymbol SetMethod => _setMethod;

		public override bool IsOverloads
		{
			get
			{
				if ((_flags & SourceMemberFlags.Shadows) != 0)
				{
					return false;
				}
				if ((_flags & SourceMemberFlags.Overloads) != 0)
				{
					return true;
				}
				return (_flags & SourceMemberFlags.Overrides) != 0;
			}
		}

		public override bool IsWithEvents => (_flags & SourceMemberFlags.WithEvents) != 0;

		internal override bool ShadowsExplicitly => (_flags & SourceMemberFlags.Shadows) != 0;

		internal bool OverloadsExplicitly => (_flags & SourceMemberFlags.Overloads) != 0;

		internal bool OverridesExplicitly => (_flags & SourceMemberFlags.Overrides) != 0;

		internal override CallingConvention CallingConvention
		{
			get
			{
				if (!IsShared)
				{
					return CallingConvention.HasThis;
				}
				return CallingConvention.Default;
			}
		}

		public override ImmutableArray<ParameterSymbol> Parameters
		{
			get
			{
				EnsureSignature();
				return _lazyParameters;
			}
		}

		public override int ParameterCount
		{
			get
			{
				if (!_lazyParameters.IsDefault)
				{
					return _lazyParameters.Length;
				}
				DeclarationStatementSyntax declarationSyntax = DeclarationSyntax;
				if (declarationSyntax.Kind() == SyntaxKind.PropertyStatement)
				{
					return ((PropertyStatementSyntax)declarationSyntax).ParameterList?.Parameters.Count ?? 0;
				}
				return base.ParameterCount;
			}
		}

		internal override ParameterSymbol MeParameter
		{
			get
			{
				if (IsShared)
				{
					return null;
				}
				if ((object)_lazyMeParameter == null)
				{
					Interlocked.CompareExchange(ref _lazyMeParameter, new MeParameterSymbol(this), null);
				}
				return _lazyMeParameter;
			}
		}

		public override ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations
		{
			get
			{
				if (_lazyImplementedProperties.IsDefault)
				{
					BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
					((SourceModuleSymbol)ContainingModule).AtomicStoreArrayAndDiagnostics(ref _lazyImplementedProperties, ComputeExplicitInterfaceImplementations(instance), instance);
					instance.Free();
				}
				return _lazyImplementedProperties;
			}
		}

		internal override OverriddenMembersResult<PropertySymbol> OverriddenMembers
		{
			get
			{
				EnsureSignature();
				return _lazyOverriddenProperties;
			}
		}

		public override ImmutableArray<CustomModifier> TypeCustomModifiers => base.OverriddenProperty?.TypeCustomModifiers ?? ImmutableArray<CustomModifier>.Empty;

		public override Accessibility DeclaredAccessibility => (Accessibility)(_flags & SourceMemberFlags.AccessibilityMask);

		public override bool IsImplicitlyDeclared => _containingType.AreMembersImplicitlyDeclared;

		internal bool IsCustomProperty
		{
			get
			{
				if ((object)_backingField == null)
				{
					return !IsMustOverride;
				}
				return false;
			}
		}

		internal bool IsAutoProperty
		{
			get
			{
				if (!IsWithEvents)
				{
					return (object)_backingField != null;
				}
				return false;
			}
		}

		internal override FieldSymbol AssociatedField => _backingField;

		internal VisualBasicSyntaxNode Syntax
		{
			get
			{
				if (_syntaxRef == null)
				{
					return null;
				}
				return VisualBasicExtensions.GetVisualBasicSyntax(_syntaxRef);
			}
		}

		internal SyntaxReference SyntaxReference => _syntaxRef;

		internal SyntaxReference BlockSyntaxReference => _blockRef;

		internal override ObsoleteAttributeData ObsoleteAttributeData
		{
			get
			{
				if (!_containingType.AnyMemberHasAttributes)
				{
					return null;
				}
				CustomAttributesBag<VisualBasicAttributeData> lazyCustomAttributesBag = _lazyCustomAttributesBag;
				if (lazyCustomAttributesBag != null && lazyCustomAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
				{
					return ((CommonPropertyEarlyWellKnownAttributeData)_lazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData)?.ObsoleteAttributeData;
				}
				return ObsoleteAttributeData.Uninitialized;
			}
		}

		internal SyntaxTree SyntaxTree => _syntaxRef.SyntaxTree;

		internal override bool IsMyGroupCollectionProperty => false;

		private SourcePropertySymbol(SourceMemberContainerTypeSymbol container, string name, SourceMemberFlags flags, SyntaxReference syntaxRef, SyntaxReference blockRef, Location location)
		{
			_containingType = container;
			_name = name;
			_syntaxRef = syntaxRef;
			_blockRef = blockRef;
			_location = location;
			_flags = flags;
			_lazyState = 0;
		}

		internal static SourcePropertySymbol Create(SourceMemberContainerTypeSymbol containingType, Binder bodyBinder, PropertyStatementSyntax syntax, PropertyBlockSyntax blockSyntaxOpt, DiagnosticBag diagnostics)
		{
			MemberModifiers memberModifiers = DecodeModifiers(syntax.Modifiers, containingType, bodyBinder, diagnostics);
			SyntaxToken identifier = syntax.Identifier;
			string valueText = identifier.ValueText;
			string.IsNullOrEmpty(valueText);
			Location location = identifier.GetLocation();
			SyntaxReference syntaxReference = bodyBinder.GetSyntaxReference(syntax);
			SyntaxReference blockRef = ((blockSyntaxOpt == null) ? null : bodyBinder.GetSyntaxReference(blockSyntaxOpt));
			SourcePropertySymbol sourcePropertySymbol = new SourcePropertySymbol(containingType, valueText, memberModifiers.AllFlags, syntaxReference, blockRef, location);
			bodyBinder = new LocationSpecificBinder(BindingLocation.PropertySignature, sourcePropertySymbol, bodyBinder);
			if (syntax.AttributeLists.Count == 0)
			{
				sourcePropertySymbol.SetCustomAttributeData(CustomAttributesBag<VisualBasicAttributeData>.Empty);
			}
			SourceMemberFlags propertyFlags = memberModifiers.AllFlags & ~SourceMemberFlags.AccessibilityMask;
			SourcePropertyAccessorSymbol sourcePropertyAccessorSymbol = null;
			SourcePropertyAccessorSymbol sourcePropertyAccessorSymbol2 = null;
			if (blockSyntaxOpt != null)
			{
				SyntaxList<AccessorBlockSyntax>.Enumerator enumerator = blockSyntaxOpt.Accessors.GetEnumerator();
				while (enumerator.MoveNext())
				{
					AccessorBlockSyntax current = enumerator.Current;
					switch (current.BlockStatement.Kind())
					{
					case SyntaxKind.GetAccessorStatement:
					{
						SourcePropertyAccessorSymbol sourcePropertyAccessorSymbol4 = CreateAccessor(sourcePropertySymbol, SourceMemberFlags.MethodKindPropertyGet, propertyFlags, bodyBinder, current, diagnostics);
						if ((object)sourcePropertyAccessorSymbol == null)
						{
							sourcePropertyAccessorSymbol = sourcePropertyAccessorSymbol4;
						}
						else
						{
							DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_DuplicatePropertyGet, sourcePropertyAccessorSymbol4.Locations[0]);
						}
						break;
					}
					case SyntaxKind.SetAccessorStatement:
					{
						SourcePropertyAccessorSymbol sourcePropertyAccessorSymbol3 = CreateAccessor(sourcePropertySymbol, SourceMemberFlags.MethodKindPropertySet, propertyFlags, bodyBinder, current, diagnostics);
						if ((object)sourcePropertyAccessorSymbol2 == null)
						{
							sourcePropertyAccessorSymbol2 = sourcePropertyAccessorSymbol3;
						}
						else
						{
							DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_DuplicatePropertySet, sourcePropertyAccessorSymbol3.Locations[0]);
						}
						break;
					}
					}
				}
			}
			bool flag = (memberModifiers.FoundFlags & SourceMemberFlags.ReadOnly) != 0;
			bool flag2 = (memberModifiers.FoundFlags & SourceMemberFlags.WriteOnly) != 0;
			if (!sourcePropertySymbol.IsMustOverride)
			{
				if (flag)
				{
					if ((object)sourcePropertyAccessorSymbol != null)
					{
						if (sourcePropertyAccessorSymbol.LocalAccessibility != 0)
						{
							DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_ReadOnlyNoAccessorFlag, GetAccessorBlockBeginLocation(sourcePropertyAccessorSymbol));
						}
					}
					else if (blockSyntaxOpt != null)
					{
						DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_ReadOnlyHasNoGet, location);
					}
					if ((object)sourcePropertyAccessorSymbol2 != null)
					{
						DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_ReadOnlyHasSet, sourcePropertyAccessorSymbol2.Locations[0]);
					}
				}
				if (flag2)
				{
					if ((object)sourcePropertyAccessorSymbol2 != null)
					{
						if (sourcePropertyAccessorSymbol2.LocalAccessibility != 0)
						{
							DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_WriteOnlyNoAccessorFlag, GetAccessorBlockBeginLocation(sourcePropertyAccessorSymbol2));
						}
					}
					else if (blockSyntaxOpt != null)
					{
						DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_WriteOnlyHasNoWrite, location);
					}
					if ((object)sourcePropertyAccessorSymbol != null)
					{
						DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_WriteOnlyHasGet, sourcePropertyAccessorSymbol.Locations[0]);
					}
				}
				if ((object)sourcePropertyAccessorSymbol != null && (object)sourcePropertyAccessorSymbol2 != null)
				{
					if (sourcePropertyAccessorSymbol.LocalAccessibility != 0 && sourcePropertyAccessorSymbol2.LocalAccessibility != 0)
					{
						SourcePropertyAccessorSymbol accessor = ((sourcePropertyAccessorSymbol.Locations[0].SourceSpan.Start < sourcePropertyAccessorSymbol2.Locations[0].SourceSpan.Start) ? sourcePropertyAccessorSymbol2 : sourcePropertyAccessorSymbol);
						DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_OnlyOneAccessorForGetSet, GetAccessorBlockBeginLocation(accessor));
					}
					else if (sourcePropertySymbol.IsOverridable && (sourcePropertyAccessorSymbol.LocalAccessibility == Accessibility.Private || sourcePropertyAccessorSymbol2.LocalAccessibility == Accessibility.Private))
					{
						bodyBinder.ReportModifierError(syntax.Modifiers, ERRID.ERR_BadPropertyAccessorFlags3, diagnostics, s_overridableModifierKinds);
					}
				}
				if (!flag && !flag2 && ((object)sourcePropertyAccessorSymbol == null || (object)sourcePropertyAccessorSymbol2 == null) && blockSyntaxOpt != null && !sourcePropertySymbol.IsMustOverride)
				{
					DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_PropMustHaveGetSet, location);
				}
			}
			if (blockSyntaxOpt == null)
			{
				if (!sourcePropertySymbol.IsMustOverride)
				{
					if (flag2)
					{
						DiagnosticBagExtensions.Add(diagnostics, ERRID.ERR_AutoPropertyCantBeWriteOnly, location);
					}
					string name = "_" + sourcePropertySymbol._name;
					sourcePropertySymbol._backingField = new SynthesizedPropertyBackingFieldSymbol(sourcePropertySymbol, name, sourcePropertySymbol.IsShared);
				}
				SourceMemberFlags sourceMemberFlags = sourcePropertySymbol._flags & ~SourceMemberFlags.MethodKindMask;
				if (!flag2)
				{
					sourcePropertySymbol._getMethod = new SourcePropertyAccessorSymbol(sourcePropertySymbol, Binder.GetAccessorName(sourcePropertySymbol.Name, MethodKind.PropertyGet, isWinMd: false), sourceMemberFlags | SourceMemberFlags.MethodKindPropertyGet, sourcePropertySymbol._syntaxRef, sourcePropertySymbol.Locations);
				}
				if (!flag)
				{
					sourcePropertySymbol._setMethod = new SourcePropertyAccessorSymbol(sourcePropertySymbol, Binder.GetAccessorName(sourcePropertySymbol.Name, MethodKind.PropertySet, SymbolExtensions.IsCompilationOutputWinMdObj(sourcePropertySymbol)), sourceMemberFlags | SourceMemberFlags.MethodKindPropertySet | SourceMemberFlags.Dim, sourcePropertySymbol._syntaxRef, sourcePropertySymbol.Locations);
				}
			}
			else
			{
				sourcePropertySymbol._getMethod = sourcePropertyAccessorSymbol;
				sourcePropertySymbol._setMethod = sourcePropertyAccessorSymbol2;
			}
			return sourcePropertySymbol;
		}

		internal static SourcePropertySymbol CreateWithEvents(SourceMemberContainerTypeSymbol containingType, Binder bodyBinder, SyntaxToken identifier, SyntaxReference syntaxRef, MemberModifiers modifiers, bool firstFieldDeclarationOfType, BindingDiagnosticBag diagnostics)
		{
			string valueText = identifier.ValueText;
			bodyBinder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_CompilerServices_AccessedThroughPropertyAttribute__ctor, (VisualBasicSyntaxNode)identifier.Parent, diagnostics);
			string.IsNullOrEmpty(valueText);
			Location location = identifier.GetLocation();
			SourceMemberFlags sourceMemberFlags = modifiers.AllFlags;
			if ((sourceMemberFlags & SourceMemberFlags.Shared) == 0)
			{
				sourceMemberFlags = modifiers.AllFlags | SourceMemberFlags.Overridable;
			}
			if (firstFieldDeclarationOfType)
			{
				sourceMemberFlags |= SourceMemberFlags.FirstFieldDeclarationOfType;
			}
			SourcePropertySymbol sourcePropertySymbol = new SourcePropertySymbol(containingType, valueText, sourceMemberFlags, syntaxRef, null, location);
			sourcePropertySymbol._lazyImplementedProperties = ImmutableArray<PropertySymbol>.Empty;
			sourcePropertySymbol.SetCustomAttributeData(CustomAttributesBag<VisualBasicAttributeData>.Empty);
			string name = "_" + sourcePropertySymbol._name;
			sourcePropertySymbol._backingField = new SourceWithEventsBackingFieldSymbol(sourcePropertySymbol, syntaxRef, name);
			sourcePropertySymbol._getMethod = new SynthesizedWithEventsGetAccessorSymbol(containingType, sourcePropertySymbol);
			sourcePropertySymbol._setMethod = new SynthesizedWithEventsSetAccessorSymbol(containingType, sourcePropertySymbol, bodyBinder.GetSpecialType(SpecialType.System_Void, identifier, diagnostics), "WithEventsValue");
			return sourcePropertySymbol;
		}

		internal void CloneParametersForAccessor(MethodSymbol method, ArrayBuilder<ParameterSymbol> parameterBuilder)
		{
			MethodSymbol overriddenMethod = method.OverriddenMethod;
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = Parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				ParameterSymbol thisParam = new SourceClonedParameterSymbol((SourceParameterSymbol)current, method, current.Ordinal);
				if ((object)overriddenMethod != null)
				{
					CustomModifierUtils.CopyParameterCustomModifiers(overriddenMethod.Parameters[current.Ordinal], ref thisParam);
				}
				parameterBuilder.Add(thisParam);
			}
		}

		private TypeSymbol ComputeType(BindingDiagnosticBag diagnostics)
		{
			Binder binder = CreateBinderForTypeDeclaration();
			if (IsWithEvents)
			{
				ModifiedIdentifierSyntax modifiedIdentifier = (ModifiedIdentifierSyntax)_syntaxRef.GetSyntax();
				return SourceMemberFieldSymbol.ComputeWithEventsFieldType(this, modifiedIdentifier, binder, (_flags & SourceMemberFlags.FirstFieldDeclarationOfType) == 0, diagnostics);
			}
			PropertyStatementSyntax propertyStatementSyntax = (PropertyStatementSyntax)_syntaxRef.GetSyntax();
			AsClauseSyntax asClause = propertyStatementSyntax.AsClause;
			if (asClause != null && asClause.Kind() == SyntaxKind.AsNewClause && ((AsNewClauseSyntax)asClause).NewExpression.Kind() == SyntaxKind.AnonymousObjectCreationExpression)
			{
				return ErrorTypeSymbol.UnknownResultType;
			}
			Func<DiagnosticInfo> getRequireTypeDiagnosticInfoFunc = null;
			if (!string.IsNullOrEmpty(_name))
			{
				if (binder.OptionStrict == OptionStrict.On)
				{
					getRequireTypeDiagnosticInfoFunc = ErrorFactory.GetErrorInfo_ERR_StrictDisallowsImplicitProc;
				}
				else if (binder.OptionStrict == OptionStrict.Custom)
				{
					getRequireTypeDiagnosticInfoFunc = ErrorFactory.GetErrorInfo_WRN_ObjectAssumedProperty1_WRN_MissingAsClauseinProperty;
				}
			}
			SyntaxToken identifier = propertyStatementSyntax.Identifier;
			TypeSymbol typeSymbol = binder.DecodeIdentifierType(identifier, asClause, getRequireTypeDiagnosticInfoFunc, diagnostics);
			if (!TypeSymbolExtensions.IsErrorType(typeSymbol))
			{
				SyntaxNodeOrToken asClauseLocation = SourceSymbolHelpers.GetAsClauseLocation(identifier, asClause);
				AccessCheck.VerifyAccessExposureForMemberType(this, asClauseLocation, typeSymbol, diagnostics);
				TypeSymbol restrictedType = null;
				if (TypeSymbolExtensions.IsRestrictedTypeOrArrayType(typeSymbol, out restrictedType))
				{
					Binder.ReportDiagnostic(diagnostics, asClauseLocation, ERRID.ERR_RestrictedType1, restrictedType);
				}
				MethodSymbol getMethod = GetMethod;
				if ((object)getMethod != null && getMethod.IsIterator)
				{
					TypeSymbol originalDefinition = typeSymbol.OriginalDefinition;
					if (originalDefinition.SpecialType != SpecialType.System_Collections_Generic_IEnumerable_T && originalDefinition.SpecialType != SpecialType.System_Collections_Generic_IEnumerator_T && typeSymbol.SpecialType != SpecialType.System_Collections_IEnumerable && typeSymbol.SpecialType != SpecialType.System_Collections_IEnumerator)
					{
						Binder.ReportDiagnostic(diagnostics, asClauseLocation, ERRID.ERR_BadIteratorReturn);
					}
				}
			}
			return typeSymbol;
		}

		internal override void SetMetadataName(string metadataName)
		{
			Interlocked.CompareExchange(ref _lazyMetadataName, metadataName, null);
		}

		internal override LexicalSortKey GetLexicalSortKey()
		{
			return new LexicalSortKey(_location, DeclaringCompilation);
		}

		internal override bool IsDefinedInSourceTree(SyntaxTree tree, TextSpan? definedWithinSpan, CancellationToken cancellationToken = default(CancellationToken))
		{
			VisualBasicSyntaxNode syntax = Syntax;
			if (syntax != null)
			{
				return Symbol.IsDefinedInSourceTree(syntax.Parent, tree, definedWithinSpan, cancellationToken);
			}
			return false;
		}

		private OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
		{
			if (IsWithEvents)
			{
				return default(OneOrMany<SyntaxList<AttributeListSyntax>>);
			}
			return OneOrMany.Create(((PropertyStatementSyntax)_syntaxRef.GetSyntax()).AttributeLists);
		}

		private OneOrMany<SyntaxList<AttributeListSyntax>> GetReturnTypeAttributeDeclarations()
		{
			OneOrMany<SyntaxList<AttributeListSyntax>> result;
			if (IsWithEvents)
			{
				result = default(OneOrMany<SyntaxList<AttributeListSyntax>>);
			}
			else
			{
				AsClauseSyntax asClause = ((PropertyStatementSyntax)Syntax).AsClause;
				if (asClause != null)
				{
					return OneOrMany.Create(SyntaxExtensions.Attributes(asClause));
				}
				result = default(OneOrMany<SyntaxList<AttributeListSyntax>>);
			}
			return result;
		}

		internal CustomAttributesBag<VisualBasicAttributeData> GetAttributesBag()
		{
			if (_lazyCustomAttributesBag == null || !_lazyCustomAttributesBag.IsSealed)
			{
				LoadAndValidateAttributes(GetAttributeDeclarations(), ref _lazyCustomAttributesBag);
			}
			return _lazyCustomAttributesBag;
		}

		internal CustomAttributesBag<VisualBasicAttributeData> GetReturnTypeAttributesBag()
		{
			if (_lazyReturnTypeCustomAttributesBag == null || !_lazyReturnTypeCustomAttributesBag.IsSealed)
			{
				LoadAndValidateAttributes(GetReturnTypeAttributeDeclarations(), ref _lazyReturnTypeCustomAttributesBag, AttributeLocation.Return);
			}
			return _lazyReturnTypeCustomAttributesBag;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return GetAttributesBag().Attributes;
		}

		private CommonPropertyWellKnownAttributeData GetDecodedWellKnownAttributeData()
		{
			CustomAttributesBag<VisualBasicAttributeData> customAttributesBag = _lazyCustomAttributesBag;
			if (customAttributesBag == null || !customAttributesBag.IsDecodedWellKnownAttributeDataComputed)
			{
				customAttributesBag = GetAttributesBag();
			}
			return (CommonPropertyWellKnownAttributeData)customAttributesBag.DecodedWellKnownAttributeData;
		}

		private CommonReturnTypeWellKnownAttributeData GetDecodedReturnTypeWellKnownAttributeData()
		{
			CustomAttributesBag<VisualBasicAttributeData> customAttributesBag = _lazyReturnTypeCustomAttributesBag;
			if (customAttributesBag == null || !customAttributesBag.IsDecodedWellKnownAttributeDataComputed)
			{
				customAttributesBag = GetReturnTypeAttributesBag();
			}
			return (CommonReturnTypeWellKnownAttributeData)customAttributesBag.DecodedWellKnownAttributeData;
		}

		internal override VisualBasicAttributeData EarlyDecodeWellKnownAttribute(ref EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments)
		{
			VisualBasicAttributeData boundAttribute = null;
			ObsoleteAttributeData obsoleteData = null;
			if (EarlyDecodeDeprecatedOrExperimentalOrObsoleteAttribute(ref arguments, out boundAttribute, out obsoleteData))
			{
				if (obsoleteData != null)
				{
					arguments.GetOrCreateData<CommonPropertyEarlyWellKnownAttributeData>().ObsoleteAttributeData = obsoleteData;
				}
				return boundAttribute;
			}
			return base.EarlyDecodeWellKnownAttribute(ref arguments);
		}

		internal override void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation> arguments)
		{
			VisualBasicAttributeData attribute = arguments.Attribute;
			BindingDiagnosticBag bindingDiagnosticBag = (BindingDiagnosticBag)arguments.Diagnostics;
			if (attribute.IsTargetAttribute(this, AttributeDescription.TupleElementNamesAttribute))
			{
				bindingDiagnosticBag.Add(ERRID.ERR_ExplicitTupleElementNamesAttribute, arguments.AttributeSyntaxOpt!.Location);
			}
			if (arguments.SymbolPart == AttributeLocation.Return)
			{
				bool flag = attribute.IsTargetAttribute(this, AttributeDescription.MarshalAsAttribute);
				if ((object)_getMethod == null && (object)_setMethod != null && (!flag || !SynthesizedParameterSimpleSymbol.IsMarshalAsAttributeApplicable(_setMethod)))
				{
					bindingDiagnosticBag.Add(ERRID.WRN_ReturnTypeAttributeOnWriteOnlyProperty, arguments.AttributeSyntaxOpt!.GetLocation());
					return;
				}
				if (flag)
				{
					MarshalAsAttributeDecoder<CommonReturnTypeWellKnownAttributeData, AttributeSyntax, VisualBasicAttributeData, AttributeLocation>.Decode(ref arguments, AttributeTargets.Field, MessageProvider.Instance);
					return;
				}
			}
			else
			{
				if (attribute.IsTargetAttribute(this, AttributeDescription.SpecialNameAttribute))
				{
					arguments.GetOrCreateData<CommonPropertyWellKnownAttributeData>().HasSpecialNameAttribute = true;
					return;
				}
				if (attribute.IsTargetAttribute(this, AttributeDescription.ExcludeFromCodeCoverageAttribute))
				{
					arguments.GetOrCreateData<CommonPropertyWellKnownAttributeData>().HasExcludeFromCodeCoverageAttribute = true;
					return;
				}
				if (!IsWithEvents && attribute.IsTargetAttribute(this, AttributeDescription.DebuggerHiddenAttribute))
				{
					if (((object)_getMethod == null || !((SourcePropertyAccessorSymbol)_getMethod).HasDebuggerHiddenAttribute) && ((object)_setMethod == null || !((SourcePropertyAccessorSymbol)_setMethod).HasDebuggerHiddenAttribute))
					{
						bindingDiagnosticBag.Add(ERRID.WRN_DebuggerHiddenIgnoredOnProperties, arguments.AttributeSyntaxOpt!.GetLocation());
					}
					return;
				}
			}
			base.DecodeWellKnownAttribute(ref arguments);
		}

		private void EnsureSignature()
		{
			if (!_lazyParameters.IsDefault)
			{
				return;
			}
			BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
			SourceModuleSymbol sourceModuleSymbol = (SourceModuleSymbol)ContainingModule;
			ImmutableArray<ParameterSymbol> immutableArray = ComputeParameters(instance);
			TypeSymbol typeSymbol = ComputeType(instance);
			OverriddenMembersResult<PropertySymbol> overriddenMembersResult;
			if (!IsOverrides || !OverrideHidingHelper.CanOverrideOrHide(this))
			{
				overriddenMembersResult = OverriddenMembersResult<PropertySymbol>.Empty;
			}
			else
			{
				ArrayBuilder<ParameterSymbol> instance2 = ArrayBuilder<ParameterSymbol>.GetInstance(immutableArray.Length);
				ImmutableArray<ParameterSymbol>.Enumerator enumerator = immutableArray.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ParameterSymbol current = enumerator.Current;
					instance2.Add(new SignatureOnlyParameterSymbol(current.Type, ImmutableArray<CustomModifier>.Empty, ImmutableArray<CustomModifier>.Empty, null, isParamArray: false, current.IsByRef, isOut: false, current.IsOptional));
				}
				overriddenMembersResult = OverrideHidingHelper<PropertySymbol>.MakeOverriddenMembers(new SignatureOnlyPropertySymbol(Name, _containingType, IsReadOnly, IsWriteOnly, instance2.ToImmutableAndFree(), returnsByRef: false, typeSymbol, ImmutableArray<CustomModifier>.Empty, ImmutableArray<CustomModifier>.Empty, isOverrides: true, IsWithEvents));
			}
			PropertySymbol overriddenMember = overriddenMembersResult.OverriddenMember;
			if ((object)overriddenMember != null)
			{
				TypeSymbol type = overriddenMember.Type;
				if (TypeSymbolExtensions.IsSameType(typeSymbol, type, TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds))
				{
					typeSymbol = CustomModifierUtils.CopyTypeCustomModifiers(type, typeSymbol);
				}
				immutableArray = CustomModifierUtils.CopyParameterCustomModifiers(overriddenMember.Parameters, immutableArray);
			}
			Interlocked.CompareExchange(ref _lazyOverriddenProperties, overriddenMembersResult, null);
			Interlocked.CompareExchange(ref _lazyType, typeSymbol, null);
			sourceModuleSymbol.AtomicStoreArrayAndDiagnostics(ref _lazyParameters, immutableArray, instance);
			instance.Free();
		}

		private ImmutableArray<ParameterSymbol> ComputeParameters(BindingDiagnosticBag diagnostics)
		{
			if (IsWithEvents)
			{
				return ImmutableArray<ParameterSymbol>.Empty;
			}
			Binder binder = CreateBinderForTypeDeclaration();
			PropertyStatementSyntax propertyStatementSyntax = (PropertyStatementSyntax)_syntaxRef.GetSyntax();
			ImmutableArray<ParameterSymbol> immutableArray = binder.DecodePropertyParameterList(this, propertyStatementSyntax.ParameterList, diagnostics);
			if (IsDefault)
			{
				if (!HasRequiredParameters(immutableArray))
				{
					diagnostics.Add(ERRID.ERR_DefaultPropertyWithNoParams, _location);
				}
				binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Reflection_DefaultMemberAttribute__ctor, propertyStatementSyntax, diagnostics);
			}
			return immutableArray;
		}

		private ImmutableArray<PropertySymbol> ComputeExplicitInterfaceImplementations(BindingDiagnosticBag diagnostics)
		{
			Binder bodyBinder = CreateBinderForTypeDeclaration();
			PropertyStatementSyntax syntax = (PropertyStatementSyntax)_syntaxRef.GetSyntax();
			return BindImplementsClause(_containingType, bodyBinder, this, syntax, diagnostics);
		}

		internal ImmutableArray<MethodSymbol> GetAccessorImplementations(bool getter)
		{
			ImmutableArray<PropertySymbol> explicitInterfaceImplementations = ExplicitInterfaceImplementations;
			if (explicitInterfaceImplementations.IsEmpty)
			{
				return ImmutableArray<MethodSymbol>.Empty;
			}
			ArrayBuilder<MethodSymbol> instance = ArrayBuilder<MethodSymbol>.GetInstance();
			ImmutableArray<PropertySymbol>.Enumerator enumerator = explicitInterfaceImplementations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				PropertySymbol current = enumerator.Current;
				MethodSymbol methodSymbol = (getter ? current.GetMethod : current.SetMethod);
				if ((object)methodSymbol != null && SymbolExtensions.RequiresImplementation(methodSymbol))
				{
					instance.Add(methodSymbol);
				}
			}
			return instance.ToImmutableAndFree();
		}

		internal Location GetImplementingLocation(PropertySymbol implementedProperty)
		{
			if (_syntaxRef.GetSyntax() is PropertyStatementSyntax propertyStatementSyntax && propertyStatementSyntax.ImplementsClause != null)
			{
				Binder binder = CreateBinderForTypeDeclaration();
				return ImplementsHelper.FindImplementingSyntax(propertyStatementSyntax.ImplementsClause, this, implementedProperty, _containingType, binder).GetLocation();
			}
			return Locations.FirstOrDefault() ?? NoLocation.Singleton;
		}

		private Binder CreateBinderForTypeDeclaration()
		{
			Binder containingBinder = BinderBuilder.CreateBinderForType((SourceModuleSymbol)ContainingModule, _syntaxRef.SyntaxTree, _containingType);
			return new LocationSpecificBinder(BindingLocation.PropertySignature, this, containingBinder);
		}

		private static SourcePropertyAccessorSymbol CreateAccessor(SourcePropertySymbol property, SourceMemberFlags kindFlags, SourceMemberFlags propertyFlags, Binder bodyBinder, AccessorBlockSyntax syntax, DiagnosticBag diagnostics)
		{
			SourcePropertyAccessorSymbol sourcePropertyAccessorSymbol = SourcePropertyAccessorSymbol.CreatePropertyAccessor(property, kindFlags, propertyFlags, bodyBinder, syntax, diagnostics);
			Accessibility localAccessibility = sourcePropertyAccessorSymbol.LocalAccessibility;
			if (!IsAccessibilityMoreRestrictive(property.DeclaredAccessibility, localAccessibility))
			{
				ReportAccessorAccessibilityError(bodyBinder, syntax, ERRID.ERR_BadPropertyAccessorFlagsRestrict, diagnostics);
				return sourcePropertyAccessorSymbol;
			}
			if (property.IsNotOverridable && localAccessibility == Accessibility.Private)
			{
				ReportAccessorAccessibilityError(bodyBinder, syntax, ERRID.ERR_BadPropertyAccessorFlags1, diagnostics);
				return sourcePropertyAccessorSymbol;
			}
			if (property.IsDefault && localAccessibility == Accessibility.Private)
			{
				ReportAccessorAccessibilityError(bodyBinder, syntax, ERRID.ERR_BadPropertyAccessorFlags2, diagnostics);
			}
			return sourcePropertyAccessorSymbol;
		}

		private static bool IsAccessibilityMoreRestrictive(Accessibility property, Accessibility accessor)
		{
			if (accessor == Accessibility.NotApplicable)
			{
				return true;
			}
			return accessor < property && (accessor != Accessibility.Protected || property != Accessibility.Internal);
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (expandIncludes)
			{
				return SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(this, preferredCulture, expandIncludes, ref _lazyExpandedDocComment, cancellationToken);
			}
			return SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(this, preferredCulture, expandIncludes, ref _lazyDocComment, cancellationToken);
		}

		private static MemberModifiers DecodeModifiers(SyntaxTokenList modifiers, SourceMemberContainerTypeSymbol container, Binder binder, DiagnosticBag diagBag)
		{
			MemberModifiers memberModifiers = binder.DecodeModifiers(modifiers, SourceMemberFlags.InvalidInModule | SourceMemberFlags.AllWriteabilityModifiers | SourceMemberFlags.Private | SourceMemberFlags.Friend | SourceMemberFlags.Public | SourceMemberFlags.Overloads | SourceMemberFlags.Iterator, ERRID.ERR_BadPropertyFlags1, Accessibility.Public, diagBag);
			SourceMemberFlags foundFlags = memberModifiers.FoundFlags;
			if ((foundFlags & SourceMemberFlags.Default) != 0 && (foundFlags & SourceMemberFlags.Private) != 0)
			{
				binder.ReportModifierError(modifiers, ERRID.ERR_BadFlagsWithDefault1, diagBag, InvalidModifiers.InvalidModifiersIfDefault);
				foundFlags &= ~SourceMemberFlags.Private;
				memberModifiers = new MemberModifiers(foundFlags, memberModifiers.ComputedFlags);
			}
			return binder.ValidateSharedPropertyAndMethodModifiers(modifiers, memberModifiers, isProperty: true, container, diagBag);
		}

		private static ImmutableArray<PropertySymbol> BindImplementsClause(SourceMemberContainerTypeSymbol containingType, Binder bodyBinder, SourcePropertySymbol prop, PropertyStatementSyntax syntax, BindingDiagnosticBag diagnostics)
		{
			if (syntax.ImplementsClause != null)
			{
				if (!(prop.IsShared & !TypeSymbolExtensions.IsModuleType(containingType)))
				{
					return ImplementsHelper.ProcessImplementsClause(syntax.ImplementsClause, (PropertySymbol)prop, containingType, bodyBinder, diagnostics);
				}
				Binder.ReportDiagnostic(diagnostics, Microsoft.CodeAnalysis.VisualBasicExtensions.First(syntax.Modifiers, SyntaxKind.SharedKeyword), ERRID.ERR_SharedOnProcThatImpl, syntax.Identifier.ToString());
			}
			return ImmutableArray<PropertySymbol>.Empty;
		}

		private static Location GetAccessorBlockBeginLocation(SourcePropertyAccessorSymbol accessor)
		{
			SyntaxTree syntaxTree = accessor.SyntaxTree;
			AccessorBlockSyntax accessorBlockSyntax = (AccessorBlockSyntax)accessor.BlockSyntax;
			return syntaxTree.GetLocation(accessorBlockSyntax.BlockStatement.Span);
		}

		private static void ReportAccessorAccessibilityError(Binder binder, AccessorBlockSyntax syntax, ERRID errorId, DiagnosticBag diagnostics)
		{
			binder.ReportModifierError(syntax.BlockStatement.Modifiers, errorId, diagnostics, s_accessibilityModifierKinds);
		}

		private static bool HasRequiredParameters(ImmutableArray<ParameterSymbol> parameters)
		{
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				if (!current.IsOptional && !current.IsParamArray)
				{
					return true;
				}
			}
			return false;
		}

		private void SetCustomAttributeData(CustomAttributesBag<VisualBasicAttributeData> attributeData)
		{
			_lazyCustomAttributesBag = attributeData;
		}

		internal override void GenerateDeclarationErrors(CancellationToken cancellationToken)
		{
			base.GenerateDeclarationErrors(cancellationToken);
			_ = Type;
			_ = Parameters;
			GetReturnTypeAttributesBag();
			_ = ExplicitInterfaceImplementations;
			if (DeclaringCompilation.EventQueue != null)
			{
				((SourceModuleSymbol)ContainingModule).AtomicSetFlagAndRaiseSymbolDeclaredEvent(ref _lazyState, 1, 0, this);
			}
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			if (TypeSymbolExtensions.ContainsTupleNames(Type))
			{
				Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.SynthesizeTupleNamesAttribute(Type));
			}
		}
	}
}
