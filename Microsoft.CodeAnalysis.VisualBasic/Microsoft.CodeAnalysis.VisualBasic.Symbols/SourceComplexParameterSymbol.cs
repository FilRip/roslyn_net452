using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SourceComplexParameterSymbol : SourceParameterSymbol
	{
		private class SourceComplexParameterSymbolWithCustomModifiers : SourceComplexParameterSymbol
		{
			private readonly ImmutableArray<CustomModifier> _customModifiers;

			private readonly ImmutableArray<CustomModifier> _refCustomModifiers;

			public override ImmutableArray<CustomModifier> CustomModifiers => _customModifiers;

			public override ImmutableArray<CustomModifier> RefCustomModifiers => _refCustomModifiers;

			public SourceComplexParameterSymbolWithCustomModifiers(Symbol container, string name, int ordinal, TypeSymbol type, Location location, SyntaxReference syntaxRef, SourceParameterFlags flags, ConstantValue defaultValueOpt, ImmutableArray<CustomModifier> customModifiers, ImmutableArray<CustomModifier> refCustomModifiers)
				: base(container, name, ordinal, type, location, syntaxRef, flags, defaultValueOpt)
			{
				_customModifiers = customModifiers;
				_refCustomModifiers = refCustomModifiers;
			}

			internal override ParameterSymbol WithTypeAndCustomModifiers(TypeSymbol type, ImmutableArray<CustomModifier> customModifiers, ImmutableArray<CustomModifier> refCustomModifiers)
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		private readonly SyntaxReference _syntaxRef;

		private readonly SourceParameterFlags _flags;

		private ConstantValue _lazyDefaultValue;

		private CustomAttributesBag<VisualBasicAttributeData> _lazyCustomAttributesBag;

		private SourceParameterSymbol BoundAttributesSource
		{
			get
			{
				if (!(base.ContainingSymbol is SourceMemberMethodSymbol sourceMemberMethodSymbol))
				{
					return null;
				}
				SourceMemberMethodSymbol sourcePartialDefinition = sourceMemberMethodSymbol.SourcePartialDefinition;
				if ((object)sourcePartialDefinition == null)
				{
					return null;
				}
				return (SourceParameterSymbol)sourcePartialDefinition.Parameters[base.Ordinal];
			}
		}

		internal override SyntaxList<AttributeListSyntax> AttributeDeclarationList
		{
			get
			{
				if (_syntaxRef != null)
				{
					return ((ParameterSyntax)_syntaxRef.GetSyntax()).AttributeLists;
				}
				return default(SyntaxList<AttributeListSyntax>);
			}
		}

		public override bool HasExplicitDefaultValue => (object)this.get_ExplicitDefaultConstantValue(SymbolsInProgress<ParameterSymbol>.Empty) != null;

		internal override ConstantValue ExplicitDefaultConstantValue
		{
			get
			{
				if ((object)_lazyDefaultValue == ConstantValue.Unset)
				{
					BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
					if ((object)Interlocked.CompareExchange(ref _lazyDefaultValue, BindDefaultValue(inProgress, instance), ConstantValue.Unset) == ConstantValue.Unset)
					{
						((SourceModuleSymbol)ContainingModule).AddDeclarationDiagnostics(instance);
					}
					instance.Free();
				}
				return _lazyDefaultValue;
			}
		}

		internal ParameterSyntax SyntaxNode
		{
			get
			{
				if (_syntaxRef != null)
				{
					return (ParameterSyntax)_syntaxRef.GetSyntax();
				}
				return null;
			}
		}

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
		{
			get
			{
				if (base.IsImplicitlyDeclared)
				{
					return ImmutableArray<SyntaxReference>.Empty;
				}
				if (_syntaxRef != null)
				{
					return Symbol.GetDeclaringSyntaxReferenceHelper(_syntaxRef);
				}
				return base.DeclaringSyntaxReferences;
			}
		}

		public override bool IsOptional => (_flags & SourceParameterFlags.Optional) != 0;

		public override bool IsParamArray
		{
			get
			{
				if ((_flags & SourceParameterFlags.ParamArray) != 0)
				{
					return true;
				}
				return (BoundAttributesSource ?? this).GetEarlyDecodedWellKnownAttributeData()?.HasParamArrayAttribute ?? false;
			}
		}

		internal override bool IsCallerLineNumber => (BoundAttributesSource ?? this).GetEarlyDecodedWellKnownAttributeData()?.HasCallerLineNumberAttribute ?? false;

		internal override bool IsCallerMemberName => (BoundAttributesSource ?? this).GetEarlyDecodedWellKnownAttributeData()?.HasCallerMemberNameAttribute ?? false;

		internal override bool IsCallerFilePath => (BoundAttributesSource ?? this).GetEarlyDecodedWellKnownAttributeData()?.HasCallerFilePathAttribute ?? false;

		internal override bool IsExplicitByRef => (_flags & SourceParameterFlags.ByRef) != 0;

		public override ImmutableArray<CustomModifier> CustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		private OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
		{
			SyntaxList<AttributeListSyntax> attributeDeclarationList = AttributeDeclarationList;
			if (!(base.ContainingSymbol is SourceMemberMethodSymbol sourceMemberMethodSymbol))
			{
				return OneOrMany.Create(attributeDeclarationList);
			}
			SourceMemberMethodSymbol sourcePartialImplementation = sourceMemberMethodSymbol.SourcePartialImplementation;
			SyntaxList<AttributeListSyntax> syntaxList = (((object)sourcePartialImplementation == null) ? default(SyntaxList<AttributeListSyntax>) : ((SourceParameterSymbol)sourcePartialImplementation.Parameters[base.Ordinal]).AttributeDeclarationList);
			if (attributeDeclarationList.Equals(default(SyntaxList<AttributeListSyntax>)))
			{
				return OneOrMany.Create(syntaxList);
			}
			if (syntaxList.Equals(default(SyntaxList<AttributeListSyntax>)))
			{
				return OneOrMany.Create(attributeDeclarationList);
			}
			return OneOrMany.Create(ImmutableArray.Create(attributeDeclarationList, syntaxList));
		}

		internal override CustomAttributesBag<VisualBasicAttributeData> GetAttributesBag()
		{
			if (_lazyCustomAttributesBag == null || !_lazyCustomAttributesBag.IsSealed)
			{
				SourceParameterSymbol boundAttributesSource = BoundAttributesSource;
				if ((object)boundAttributesSource != null)
				{
					CustomAttributesBag<VisualBasicAttributeData> attributesBag = boundAttributesSource.GetAttributesBag();
					Interlocked.CompareExchange(ref _lazyCustomAttributesBag, attributesBag, null);
				}
				else
				{
					OneOrMany<SyntaxList<AttributeListSyntax>> attributeDeclarations = GetAttributeDeclarations();
					LoadAndValidateAttributes(attributeDeclarations, ref _lazyCustomAttributesBag);
				}
			}
			return _lazyCustomAttributesBag;
		}

		internal override ParameterEarlyWellKnownAttributeData GetEarlyDecodedWellKnownAttributeData()
		{
			CustomAttributesBag<VisualBasicAttributeData> customAttributesBag = _lazyCustomAttributesBag;
			if (customAttributesBag == null || !customAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
			{
				customAttributesBag = GetAttributesBag();
			}
			return (ParameterEarlyWellKnownAttributeData)customAttributesBag.EarlyDecodedWellKnownAttributeData;
		}

		internal override CommonParameterWellKnownAttributeData GetDecodedWellKnownAttributeData()
		{
			CustomAttributesBag<VisualBasicAttributeData> customAttributesBag = _lazyCustomAttributesBag;
			if (customAttributesBag == null || !customAttributesBag.IsDecodedWellKnownAttributeDataComputed)
			{
				customAttributesBag = GetAttributesBag();
			}
			return (CommonParameterWellKnownAttributeData)customAttributesBag.DecodedWellKnownAttributeData;
		}

		private ConstantValue BindDefaultValue(SymbolsInProgress<ParameterSymbol> inProgress, BindingDiagnosticBag diagnostics)
		{
			ParameterSyntax syntaxNode = SyntaxNode;
			if (syntaxNode == null)
			{
				return null;
			}
			EqualsValueSyntax @default = syntaxNode.Default;
			if (@default == null)
			{
				return null;
			}
			Binder next = BinderBuilder.CreateBinderForParameterDefaultValue((SourceModuleSymbol)ContainingModule, _syntaxRef.SyntaxTree, this, syntaxNode);
			if (inProgress.Contains(this))
			{
				Binder.ReportDiagnostic(diagnostics, @default.Value, ERRID.ERR_CircularEvaluation1, this);
				return null;
			}
			DefaultParametersInProgressBinder defaultParametersInProgressBinder = new DefaultParametersInProgressBinder(inProgress.Add(this), next);
			ConstantValue constValue = null;
			defaultParametersInProgressBinder.BindParameterDefaultValue(base.Type, @default, diagnostics, out constValue);
			if ((object)constValue != null)
			{
				VerifyParamDefaultValueMatchesAttributeIfAny(constValue, @default.Value, diagnostics);
			}
			return constValue;
		}

		internal sealed override bool IsDefinedInSourceTree(SyntaxTree tree, TextSpan? definedWithinSpan, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Symbol.IsDefinedInSourceTree(SyntaxNode, tree, definedWithinSpan, cancellationToken);
		}

		private SourceComplexParameterSymbol(Symbol container, string name, int ordinal, TypeSymbol type, Location location, SyntaxReference syntaxRef, SourceParameterFlags flags, ConstantValue defaultValueOpt)
			: base(container, name, ordinal, type, location)
		{
			_flags = flags;
			_lazyDefaultValue = defaultValueOpt;
			_syntaxRef = syntaxRef;
		}

		internal static ParameterSymbol Create(Symbol container, string name, int ordinal, TypeSymbol type, Location location, SyntaxReference syntaxRef, SourceParameterFlags flags, ConstantValue defaultValueOpt)
		{
			SourceMethodSymbol sourceMethodSymbol = container as SourceMethodSymbol;
			if (flags != 0 || (object)defaultValueOpt != null || syntaxRef != null || ((object)sourceMethodSymbol != null && sourceMethodSymbol.IsPartial))
			{
				return new SourceComplexParameterSymbol(container, name, ordinal, type, location, syntaxRef, flags, defaultValueOpt);
			}
			return new SourceSimpleParameterSymbol(container, name, ordinal, type, location);
		}

		internal override ParameterSymbol ChangeOwner(Symbol newContainingSymbol)
		{
			return new SourceComplexParameterSymbol(newContainingSymbol, base.Name, base.Ordinal, base.Type, base.Locations[0], _syntaxRef, _flags, _lazyDefaultValue);
		}

		internal static ParameterSymbol CreateFromSyntax(Symbol container, ParameterSyntax syntax, string name, SourceParameterFlags flags, int ordinal, Binder binder, Binder.CheckParameterModifierDelegate checkModifier, BindingDiagnosticBag diagnostics)
		{
			Func<DiagnosticInfo> getRequireTypeDiagnosticInfoFunc = null;
			if (binder.OptionStrict == OptionStrict.On)
			{
				getRequireTypeDiagnosticInfoFunc = ErrorFactory.GetErrorInfo_ERR_StrictDisallowsImplicitArgs;
			}
			else if (binder.OptionStrict == OptionStrict.Custom)
			{
				getRequireTypeDiagnosticInfoFunc = ErrorFactory.GetErrorInfo_WRN_ObjectAssumedVar1_WRN_MissingAsClauseinVarDecl;
			}
			TypeSymbol typeSymbol = binder.DecodeModifiedIdentifierType(syntax.Identifier, syntax.AsClause, null, getRequireTypeDiagnosticInfoFunc, diagnostics, Binder.ModifiedIdentifierTypeDecoderContext.ParameterType);
			if ((flags & SourceParameterFlags.ParamArray) != 0 && typeSymbol.TypeKind != TypeKind.Error)
			{
				if (typeSymbol.TypeKind != TypeKind.Array)
				{
					Binder.ReportDiagnostic(diagnostics, syntax.Identifier, ERRID.ERR_ParamArrayNotArray);
				}
				else
				{
					if (!((ArrayTypeSymbol)typeSymbol).IsSZArray)
					{
						Binder.ReportDiagnostic(diagnostics, syntax.Identifier.Identifier, ERRID.ERR_ParamArrayRank);
					}
					binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_ParamArrayAttribute__ctor, syntax, diagnostics);
				}
			}
			SyntaxReference syntaxRef = null;
			if (syntax.AttributeLists.Count != 0 || syntax.Default != null)
			{
				syntaxRef = binder.GetSyntaxReference(syntax);
			}
			ConstantValue defaultValueOpt = null;
			if ((flags & SourceParameterFlags.Optional) != 0)
			{
				if (syntax.Default != null)
				{
					defaultValueOpt = ConstantValue.Unset;
				}
				switch (typeSymbol.SpecialType)
				{
				case SpecialType.System_DateTime:
					binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_CompilerServices_DateTimeConstantAttribute__ctor, syntax.Default, diagnostics);
					break;
				case SpecialType.System_Decimal:
					binder.ReportUseSiteInfoForSynthesizedAttribute(WellKnownMember.System_Runtime_CompilerServices_DecimalConstantAttribute__ctor, syntax.Default, diagnostics);
					break;
				}
			}
			return Create(container, name, ordinal, typeSymbol, syntax.Identifier.Identifier.GetLocation(), syntaxRef, flags, defaultValueOpt);
		}

		internal override ParameterSymbol WithTypeAndCustomModifiers(TypeSymbol type, ImmutableArray<CustomModifier> customModifiers, ImmutableArray<CustomModifier> refCustomModifiers)
		{
			if (customModifiers.IsEmpty && refCustomModifiers.IsEmpty)
			{
				return new SourceComplexParameterSymbol(base.ContainingSymbol, base.Name, base.Ordinal, type, base.Location, _syntaxRef, _flags, _lazyDefaultValue);
			}
			return new SourceComplexParameterSymbolWithCustomModifiers(base.ContainingSymbol, base.Name, base.Ordinal, type, base.Location, _syntaxRef, _flags, _lazyDefaultValue, customModifiers, refCustomModifiers);
		}
	}
}
