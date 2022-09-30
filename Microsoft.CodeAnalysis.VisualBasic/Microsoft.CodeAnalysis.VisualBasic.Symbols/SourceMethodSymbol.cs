using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SourceMethodSymbol : MethodSymbol, IAttributeTargetSymbol
	{
		protected readonly SourceMemberFlags m_flags;

		protected readonly NamedTypeSymbol m_containingType;

		private ParameterSymbol _lazyMeParameter;

		protected CustomAttributesBag<VisualBasicAttributeData> m_lazyCustomAttributesBag;

		protected CustomAttributesBag<VisualBasicAttributeData> m_lazyReturnTypeCustomAttributesBag;

		protected readonly SyntaxReference m_syntaxReferenceOpt;

		private ImmutableArray<Location> _lazyLocations;

		private string _lazyDocComment;

		private string _lazyExpandedDocComment;

		private ImmutableArray<Diagnostic> _cachedDiagnostics;

		internal ImmutableArray<Diagnostic> Diagnostics => _cachedDiagnostics;

		public override bool IsImplicitlyDeclared => m_containingType.AreMembersImplicitlyDeclared;

		internal override bool GenerateDebugInfoImpl => true;

		public sealed override MethodSymbol ConstructedFrom => this;

		public sealed override Symbol ContainingSymbol => m_containingType;

		public override NamedTypeSymbol ContainingType => m_containingType;

		public SourceModuleSymbol ContainingSourceModule => (SourceModuleSymbol)ContainingModule;

		public override Symbol AssociatedSymbol => null;

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray<MethodSymbol>.Empty;

		public override MethodKind MethodKind => SourceMemberFlagsExtensions.ToMethodKind(m_flags);

		internal sealed override bool IsMethodKindBasedOnSyntax => true;

		public override Accessibility DeclaredAccessibility => (Accessibility)(m_flags & SourceMemberFlags.AccessibilityMask);

		public sealed override bool IsMustOverride => (m_flags & SourceMemberFlags.MustOverride) != 0;

		public sealed override bool IsNotOverridable => (m_flags & SourceMemberFlags.NotOverridable) != 0;

		public sealed override bool IsOverloads
		{
			get
			{
				if ((m_flags & SourceMemberFlags.Shadows) != 0)
				{
					return false;
				}
				if ((m_flags & SourceMemberFlags.Overloads) != 0)
				{
					return true;
				}
				return (m_flags & SourceMemberFlags.Overrides) != 0;
			}
		}

		public sealed override bool IsOverridable => (m_flags & SourceMemberFlags.Overridable) != 0;

		public sealed override bool IsOverrides => (m_flags & SourceMemberFlags.Overrides) != 0;

		public sealed override bool IsShared => (m_flags & SourceMemberFlags.Shared) != 0;

		internal bool IsPartial => (m_flags & SourceMemberFlags.Partial) != 0;

		internal override bool ShadowsExplicitly => (m_flags & SourceMemberFlags.Shadows) != 0;

		internal bool OverloadsExplicitly => (m_flags & SourceMemberFlags.Overloads) != 0;

		internal bool OverridesExplicitly => (m_flags & SourceMemberFlags.Overrides) != 0;

		internal bool HandlesEvents => (m_flags & SourceMemberFlags.Const) != 0;

		internal sealed override Microsoft.Cci.CallingConvention CallingConvention => ((!IsShared) ? Microsoft.Cci.CallingConvention.HasThis : Microsoft.Cci.CallingConvention.Default) | (IsGenericMethod ? Microsoft.Cci.CallingConvention.Generic : Microsoft.Cci.CallingConvention.Default);

		internal MethodBlockBaseSyntax BlockSyntax
		{
			get
			{
				if (m_syntaxReferenceOpt == null)
				{
					return null;
				}
				return m_syntaxReferenceOpt.GetSyntax().Parent as MethodBlockBaseSyntax;
			}
		}

		internal override SyntaxNode Syntax
		{
			get
			{
				if (m_syntaxReferenceOpt == null)
				{
					return null;
				}
				MethodBlockBaseSyntax blockSyntax = BlockSyntax;
				if (blockSyntax != null)
				{
					return blockSyntax;
				}
				return VisualBasicExtensions.GetVisualBasicSyntax(m_syntaxReferenceOpt);
			}
		}

		public SyntaxTree SyntaxTree
		{
			get
			{
				if (m_syntaxReferenceOpt != null)
				{
					return m_syntaxReferenceOpt.SyntaxTree;
				}
				return null;
			}
		}

		internal MethodBaseSyntax DeclarationSyntax
		{
			get
			{
				if (m_syntaxReferenceOpt == null)
				{
					return null;
				}
				return (MethodBaseSyntax)m_syntaxReferenceOpt.GetSyntax();
			}
		}

		internal virtual bool HasEmptyBody
		{
			get
			{
				MethodBlockBaseSyntax blockSyntax = BlockSyntax;
				if (blockSyntax != null)
				{
					return !blockSyntax.Statements.Any();
				}
				return true;
			}
		}

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => Symbol.GetDeclaringSyntaxReferenceHelper(m_syntaxReferenceOpt);

		internal Location NonMergedLocation
		{
			get
			{
				if (m_syntaxReferenceOpt == null)
				{
					return null;
				}
				return GetSymbolLocation(m_syntaxReferenceOpt);
			}
		}

		public override ImmutableArray<Location> Locations
		{
			get
			{
				if (_lazyLocations.IsDefault)
				{
					Location nonMergedLocation = NonMergedLocation;
					ImmutableInterlocked.InterlockedCompareExchange(ref _lazyLocations, ((object)nonMergedLocation == null) ? ImmutableArray<Location>.Empty : ImmutableArray.Create(nonMergedLocation), default(ImmutableArray<Location>));
				}
				return _lazyLocations;
			}
		}

		public sealed override bool IsVararg => false;

		public sealed override bool IsGenericMethod => Arity != 0;

		public sealed override ImmutableArray<TypeSymbol> TypeArguments => StaticCast<TypeSymbol>.From(TypeParameters);

		public override int Arity => TypeParameters.Length;

		public sealed override bool ReturnsByRef => false;

		public sealed override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

		public override bool IsSub => (m_flags & SourceMemberFlags.Dim) != 0;

		public sealed override bool IsAsync => (m_flags & SourceMemberFlags.Async) != 0;

		public sealed override bool IsIterator => (m_flags & SourceMemberFlags.Iterator) != 0;

		public sealed override bool IsInitOnly => false;

		public override ImmutableArray<CustomModifier> ReturnTypeCustomModifiers
		{
			get
			{
				MethodSymbol overriddenMethod = OverriddenMethod;
				if ((object)overriddenMethod == null)
				{
					return ImmutableArray<CustomModifier>.Empty;
				}
				return MethodSymbolExtensions.ConstructIfGeneric(overriddenMethod, TypeArguments).ReturnTypeCustomModifiers;
			}
		}

		protected virtual SourceMethodSymbol BoundAttributesSource => null;

		protected virtual SourcePropertySymbol BoundReturnTypeAttributesSource => null;

		protected SyntaxList<AttributeListSyntax> AttributeDeclarationSyntaxList
		{
			get
			{
				if (m_syntaxReferenceOpt == null)
				{
					return default(SyntaxList<AttributeListSyntax>);
				}
				return DeclarationSyntax.AttributeLists;
			}
		}

		protected SyntaxList<AttributeListSyntax> ReturnTypeAttributeDeclarationSyntaxList
		{
			get
			{
				MethodBaseSyntax declarationSyntax = DeclarationSyntax;
				if (declarationSyntax != null)
				{
					AsClauseSyntax asClauseInternal = declarationSyntax.AsClauseInternal;
					if (asClauseInternal != null)
					{
						return SyntaxExtensions.Attributes(asClauseInternal);
					}
				}
				return default(SyntaxList<AttributeListSyntax>);
			}
		}

		public AttributeLocation DefaultAttributeLocation => AttributeLocation.Method;

		public override bool IsExtensionMethod => GetEarlyDecodedWellKnownAttributeData()?.IsExtensionMethod ?? false;

		internal abstract override bool MayBeReducibleExtensionMethod { get; }

		public override bool IsExternalMethod
		{
			get
			{
				MethodWellKnownAttributeData decodedWellKnownAttributeData = GetDecodedWellKnownAttributeData();
				if (decodedWellKnownAttributeData == null)
				{
					return false;
				}
				if (decodedWellKnownAttributeData.DllImportPlatformInvokeData != null)
				{
					return true;
				}
				if ((decodedWellKnownAttributeData.MethodImplAttributes & MethodImplAttributes.InternalCall) != 0)
				{
					return true;
				}
				if ((decodedWellKnownAttributeData.MethodImplAttributes & MethodImplAttributes.CodeTypeMask) == MethodImplAttributes.CodeTypeMask)
				{
					return true;
				}
				return false;
			}
		}

		internal sealed override MarshalPseudoCustomAttributeData ReturnTypeMarshallingInformation => GetDecodedReturnTypeWellKnownAttributeData()?.MarshallingInformation;

		internal override MethodImplAttributes ImplementationAttributes
		{
			get
			{
				if (ContainingType.IsComImport && !ContainingType.IsInterface)
				{
					return (MethodImplAttributes)4099;
				}
				return GetDecodedWellKnownAttributeData()?.MethodImplAttributes ?? MethodImplAttributes.IL;
			}
		}

		internal sealed override bool HasDeclarativeSecurity => GetDecodedWellKnownAttributeData()?.HasDeclarativeSecurity ?? false;

		internal sealed override bool IsDirectlyExcludedFromCodeCoverage => GetDecodedWellKnownAttributeData()?.HasExcludeFromCodeCoverageAttribute ?? false;

		internal override bool HasRuntimeSpecialName
		{
			get
			{
				if (!base.HasRuntimeSpecialName)
				{
					return IsVtableGapInterfaceMethod();
				}
				return true;
			}
		}

		internal override bool HasSpecialName
		{
			get
			{
				switch (MethodKind)
				{
				case MethodKind.Constructor:
				case MethodKind.Conversion:
				case MethodKind.EventAdd:
				case MethodKind.EventRaise:
				case MethodKind.EventRemove:
				case MethodKind.UserDefinedOperator:
				case MethodKind.PropertyGet:
				case MethodKind.PropertySet:
				case MethodKind.StaticConstructor:
					return true;
				default:
					if (IsVtableGapInterfaceMethod())
					{
						return true;
					}
					return GetDecodedWellKnownAttributeData()?.HasSpecialNameAttribute ?? false;
				}
			}
		}

		private bool HasSTAThreadOrMTAThreadAttribute
		{
			get
			{
				MethodWellKnownAttributeData decodedWellKnownAttributeData = GetDecodedWellKnownAttributeData();
				if (decodedWellKnownAttributeData != null)
				{
					if (!decodedWellKnownAttributeData.HasSTAThreadAttribute)
					{
						return decodedWellKnownAttributeData.HasMTAThreadAttribute;
					}
					return true;
				}
				return false;
			}
		}

		internal override ObsoleteAttributeData ObsoleteAttributeData
		{
			get
			{
				if (!(m_containingType is SourceMemberContainerTypeSymbol sourceMemberContainerTypeSymbol) || !sourceMemberContainerTypeSymbol.AnyMemberHasAttributes)
				{
					return null;
				}
				CustomAttributesBag<VisualBasicAttributeData> lazyCustomAttributesBag = m_lazyCustomAttributesBag;
				if (lazyCustomAttributesBag != null && lazyCustomAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
				{
					return ((MethodEarlyWellKnownAttributeData)m_lazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData)?.ObsoleteAttributeData;
				}
				if (DeclaringSyntaxReferences.IsEmpty)
				{
					return null;
				}
				return ObsoleteAttributeData.Uninitialized;
			}
		}

		public abstract override TypeSymbol ReturnType { get; }

		public abstract override ImmutableArray<ParameterSymbol> Parameters { get; }

		internal abstract override OverriddenMembersResult<MethodSymbol> OverriddenMembers { get; }

		protected SourceMethodSymbol(NamedTypeSymbol containingType, SourceMemberFlags flags, SyntaxReference syntaxRef, ImmutableArray<Location> locations = default(ImmutableArray<Location>))
		{
			m_containingType = containingType;
			m_flags = flags;
			m_syntaxReferenceOpt = syntaxRef;
			_lazyLocations = locations;
		}

		internal static SourceMethodSymbol CreateRegularMethod(SourceMemberContainerTypeSymbol container, MethodStatementSyntax syntax, Binder binder, DiagnosticBag diagBag)
		{
			SourceMemberFlags sourceMemberFlags = DecodeMethodModifiers(syntax.Modifiers, container, binder, diagBag).AllFlags | SourceMemberFlags.MethodKindOrdinary;
			if (syntax.Kind() == SyntaxKind.SubStatement)
			{
				sourceMemberFlags |= SourceMemberFlags.Dim;
			}
			if (syntax.HandlesClause != null)
			{
				sourceMemberFlags |= SourceMemberFlags.Const;
			}
			string valueText = syntax.Identifier.ValueText;
			ImmutableArray<HandledEvent> handledEvents;
			if (syntax.HandlesClause != null)
			{
				if (container.TypeKind == TypeKind.Struct)
				{
					Binder.ReportDiagnostic(diagBag, syntax.Identifier, ERRID.ERR_StructsCannotHandleEvents);
				}
				else if (container.IsInterface)
				{
					Binder.ReportDiagnostic(diagBag, syntax.HandlesClause, ERRID.ERR_BadInterfaceMethodFlags1, syntax.HandlesClause.HandlesKeyword.ToString());
				}
				else if (GetTypeParameterListSyntax(syntax) != null)
				{
					Binder.ReportDiagnostic(diagBag, syntax.Identifier, ERRID.ERR_HandlesInvalidOnGenericMethod);
				}
				handledEvents = default(ImmutableArray<HandledEvent>);
			}
			else
			{
				handledEvents = ImmutableArray<HandledEvent>.Empty;
			}
			int arity = ((syntax.TypeParameterList != null) ? syntax.TypeParameterList.Parameters.Count : 0);
			SourceMemberMethodSymbol sourceMemberMethodSymbol = new SourceMemberMethodSymbol(container, valueText, sourceMemberFlags, binder, syntax, arity, handledEvents);
			if (sourceMemberMethodSymbol.IsPartial && sourceMemberMethodSymbol.IsSub)
			{
				if (sourceMemberMethodSymbol.IsAsync)
				{
					Binder.ReportDiagnostic(diagBag, syntax.Identifier, ERRID.ERR_PartialMethodsMustNotBeAsync1, valueText);
				}
				ReportPartialMethodErrors(syntax.Modifiers, binder, diagBag);
			}
			return sourceMemberMethodSymbol;
		}

		internal static TypeParameterListSyntax GetTypeParameterListSyntax(MethodBaseSyntax methodSyntax)
		{
			if (methodSyntax.Kind() == SyntaxKind.SubStatement || methodSyntax.Kind() == SyntaxKind.FunctionStatement)
			{
				return ((MethodStatementSyntax)methodSyntax).TypeParameterList;
			}
			return null;
		}

		private static void ReportPartialMethodErrors(SyntaxTokenList modifiers, Binder binder, DiagnosticBag diagBag)
		{
			bool flag = true;
			SyntaxToken syntaxToken = default(SyntaxToken);
			List<SyntaxToken> list = modifiers.ToList();
			int num = list.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				SyntaxToken syntaxToken2 = list[i];
				SyntaxToken token;
				int num2;
				int num3;
				Location location;
				switch (VisualBasicExtensions.Kind(syntaxToken2))
				{
				case SyntaxKind.MustInheritKeyword:
				case SyntaxKind.MustOverrideKeyword:
				case SyntaxKind.NotOverridableKeyword:
				case SyntaxKind.OverridableKeyword:
				case SyntaxKind.OverridesKeyword:
				case SyntaxKind.PublicKeyword:
					Binder.ReportDiagnostic(diagBag, syntaxToken2, ERRID.ERR_OnlyPrivatePartialMethods1, SyntaxFacts.GetText(VisualBasicExtensions.Kind(syntaxToken2)));
					flag = false;
					break;
				case SyntaxKind.ProtectedKeyword:
					if (i >= list.Count - 1 || VisualBasicExtensions.Kind(list[i + 1]) != SyntaxKind.FriendKeyword)
					{
						goto case SyntaxKind.MustInheritKeyword;
					}
					goto IL_00ef;
				case SyntaxKind.FriendKeyword:
					if (i >= list.Count - 1 || VisualBasicExtensions.Kind(list[i + 1]) != SyntaxKind.ProtectedKeyword)
					{
						goto case SyntaxKind.MustInheritKeyword;
					}
					goto IL_00ef;
				case SyntaxKind.PartialKeyword:
					syntaxToken = syntaxToken2;
					break;
				case SyntaxKind.PrivateKeyword:
					{
						flag = false;
						break;
					}
					IL_00ef:
					i++;
					token = list[i];
					num2 = Math.Min(syntaxToken2.SpanStart, token.SpanStart);
					num3 = Math.Max(syntaxToken2.Span.End, token.Span.End);
					location = binder.SyntaxTree.GetLocation(new TextSpan(num2, num3 - num2));
					Binder.ReportDiagnostic(diagBag, location, ERRID.ERR_OnlyPrivatePartialMethods1, GeneratedExtensionSyntaxFacts.GetText(VisualBasicExtensions.Kind(syntaxToken2)) + " " + GeneratedExtensionSyntaxFacts.GetText(VisualBasicExtensions.Kind(token)));
					flag = false;
					break;
				}
			}
			if (flag)
			{
				Binder.ReportDiagnostic(diagBag, syntaxToken, ERRID.ERR_PartialMethodsMustBePrivate);
			}
		}

		internal static SourceMethodSymbol CreateDeclareMethod(SourceMemberContainerTypeSymbol container, DeclareStatementSyntax syntax, Binder binder, DiagnosticBag diagBag)
		{
			MemberModifiers memberModifiers = binder.DecodeModifiers(syntax.Modifiers, SourceMemberFlags.AllAccessibilityModifiers | SourceMemberFlags.AllShadowingModifiers, ERRID.ERR_BadDeclareFlags1, Accessibility.Public, diagBag);
			if (container.TypeKind == TypeKind.Module)
			{
				if ((memberModifiers.FoundFlags & SourceMemberFlags.Overloads) != 0)
				{
					SyntaxToken syntaxToken = syntax.Modifiers.First((SyntaxToken m) => VisualBasicExtensions.Kind(m) == SyntaxKind.OverloadsKeyword);
					DiagnosticBagExtensions.Add(diagBag, ERRID.ERR_OverloadsModifierInModule, syntaxToken.GetLocation(), syntaxToken.ValueText);
				}
				else if ((memberModifiers.FoundFlags & SourceMemberFlags.Protected) != 0)
				{
					SyntaxToken syntaxToken2 = syntax.Modifiers.First((SyntaxToken m) => VisualBasicExtensions.Kind(m) == SyntaxKind.ProtectedKeyword);
					DiagnosticBagExtensions.Add(diagBag, ERRID.ERR_ModuleCantUseDLLDeclareSpecifier1, syntaxToken2.GetLocation(), syntaxToken2.ValueText);
				}
			}
			else if (container.TypeKind == TypeKind.Struct && (memberModifiers.FoundFlags & SourceMemberFlags.Protected) != 0)
			{
				SyntaxToken syntaxToken3 = syntax.Modifiers.First((SyntaxToken m) => VisualBasicExtensions.Kind(m) == SyntaxKind.ProtectedKeyword);
				DiagnosticBagExtensions.Add(diagBag, ERRID.ERR_StructCantUseDLLDeclareSpecifier1, syntaxToken3.GetLocation(), syntaxToken3.ValueText);
			}
			if ((object)container != null && container.IsGenericType)
			{
				DiagnosticBagExtensions.Add(diagBag, ERRID.ERR_DeclaresCantBeInGeneric, syntax.Identifier.GetLocation());
			}
			SourceMemberFlags sourceMemberFlags = memberModifiers.AllFlags | SourceMemberFlags.FirstFieldDeclarationOfType | SourceMemberFlags.Shared;
			if (syntax.Kind() == SyntaxKind.DeclareSubStatement)
			{
				sourceMemberFlags |= SourceMemberFlags.Dim;
			}
			string valueText = syntax.Identifier.ValueText;
			string text = syntax.LibraryName.Token.ValueText;
			if (string.IsNullOrEmpty(text) && !syntax.LibraryName.IsMissing)
			{
				DiagnosticBagExtensions.Add(diagBag, ERRID.ERR_BadAttribute1, syntax.LibraryName.GetLocation(), valueText);
				text = null;
			}
			string text2;
			if (syntax.AliasName != null)
			{
				text2 = syntax.AliasName.Token.ValueText;
				if (string.IsNullOrEmpty(text2))
				{
					DiagnosticBagExtensions.Add(diagBag, ERRID.ERR_BadAttribute1, syntax.LibraryName.GetLocation(), valueText);
					text2 = null;
				}
			}
			else
			{
				text2 = null;
			}
			DllImportData platformInvokeInfo = new DllImportData(text, text2, GetPInvokeAttributes(syntax));
			return new SourceDeclareMethodSymbol(container, valueText, sourceMemberFlags, binder, syntax, platformInvokeInfo);
		}

		private static MethodImportAttributes GetPInvokeAttributes(DeclareStatementSyntax syntax)
		{
			MethodImportAttributes methodImportAttributes = default(MethodImportAttributes);
			switch (VisualBasicExtensions.Kind(syntax.CharsetKeyword))
			{
			case SyntaxKind.None:
			case SyntaxKind.AnsiKeyword:
				methodImportAttributes = MethodImportAttributes.ExactSpelling | MethodImportAttributes.CharSetAnsi;
				break;
			case SyntaxKind.UnicodeKeyword:
				methodImportAttributes = MethodImportAttributes.ExactSpelling | MethodImportAttributes.CharSetUnicode;
				break;
			case SyntaxKind.AutoKeyword:
				methodImportAttributes = MethodImportAttributes.CharSetAuto;
				break;
			}
			return methodImportAttributes | MethodImportAttributes.CallingConventionWinApi | MethodImportAttributes.SetLastError;
		}

		internal static SourceMethodSymbol CreateOperator(SourceMemberContainerTypeSymbol container, OperatorStatementSyntax syntax, Binder binder, DiagnosticBag diagBag)
		{
			SourceMemberFlags allFlags = DecodeOperatorModifiers(syntax, binder, diagBag).AllFlags;
			string memberNameFromSyntax = GetMemberNameFromSyntax(syntax);
			ERRID eRRID;
			switch (VisualBasicExtensions.Kind(syntax.OperatorToken))
			{
			case SyntaxKind.CTypeKeyword:
			case SyntaxKind.NotKeyword:
			case SyntaxKind.IsFalseKeyword:
			case SyntaxKind.IsTrueKeyword:
				eRRID = ERRID.ERR_OneParameterRequired1;
				break;
			case SyntaxKind.PlusToken:
			case SyntaxKind.MinusToken:
				eRRID = ERRID.ERR_OneOrTwoParametersRequired1;
				break;
			case SyntaxKind.AndKeyword:
			case SyntaxKind.LikeKeyword:
			case SyntaxKind.ModKeyword:
			case SyntaxKind.OrKeyword:
			case SyntaxKind.XorKeyword:
			case SyntaxKind.AmpersandToken:
			case SyntaxKind.AsteriskToken:
			case SyntaxKind.SlashToken:
			case SyntaxKind.LessThanToken:
			case SyntaxKind.LessThanEqualsToken:
			case SyntaxKind.LessThanGreaterThanToken:
			case SyntaxKind.EqualsToken:
			case SyntaxKind.GreaterThanToken:
			case SyntaxKind.GreaterThanEqualsToken:
			case SyntaxKind.BackslashToken:
			case SyntaxKind.CaretToken:
			case SyntaxKind.LessThanLessThanToken:
			case SyntaxKind.GreaterThanGreaterThanToken:
				eRRID = ERRID.ERR_TwoParametersRequired1;
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(VisualBasicExtensions.Kind(syntax.OperatorToken));
			}
			switch (eRRID)
			{
			case ERRID.ERR_OneParameterRequired1:
				if (syntax.ParameterList.Parameters.Count == 1)
				{
					eRRID = ERRID.ERR_None;
				}
				break;
			case ERRID.ERR_TwoParametersRequired1:
				if (syntax.ParameterList.Parameters.Count == 2)
				{
					eRRID = ERRID.ERR_None;
				}
				break;
			case ERRID.ERR_OneOrTwoParametersRequired1:
				if (syntax.ParameterList.Parameters.Count == 1 || 2 == syntax.ParameterList.Parameters.Count)
				{
					eRRID = ERRID.ERR_None;
				}
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(eRRID);
			}
			if (eRRID != 0)
			{
				Binder.ReportDiagnostic(diagBag, syntax.OperatorToken, eRRID, SyntaxFacts.GetText(VisualBasicExtensions.Kind(syntax.OperatorToken)));
			}
			allFlags |= ((VisualBasicExtensions.Kind(syntax.OperatorToken) == SyntaxKind.CTypeKeyword) ? SourceMemberFlags.MethodKindConversion : SourceMemberFlags.MethodKindOperator);
			return new SourceMemberMethodSymbol(container, memberNameFromSyntax, allFlags, binder, syntax, 0);
		}

		internal static SourceMethodSymbol CreateConstructor(SourceMemberContainerTypeSymbol container, SubNewStatementSyntax syntax, Binder binder, DiagnosticBag diagBag)
		{
			SourceMemberFlags sourceMemberFlags = DecodeConstructorModifiers(syntax.Modifiers, container, binder, diagBag).AllFlags | SourceMemberFlags.Dim;
			string name;
			if ((sourceMemberFlags & SourceMemberFlags.Shared) != 0)
			{
				name = ".cctor";
				sourceMemberFlags |= SourceMemberFlags.MethodKindSharedConstructor;
				if (syntax.ParameterList != null && syntax.ParameterList.Parameters.Count > 0)
				{
					Binder.ReportDiagnostic(diagBag, syntax.ParameterList, ERRID.ERR_SharedConstructorWithParams);
				}
			}
			else
			{
				name = ".ctor";
				sourceMemberFlags |= SourceMemberFlags.Static;
			}
			SourceMemberMethodSymbol sourceMemberMethodSymbol = new SourceMemberMethodSymbol(container, name, sourceMemberFlags, binder, syntax, 0);
			if ((sourceMemberFlags & SourceMemberFlags.Shared) == 0 && container.TypeKind == TypeKind.Struct && sourceMemberMethodSymbol.ParameterCount == 0)
			{
				Binder.ReportDiagnostic(diagBag, syntax.NewKeyword, ERRID.ERR_NewInStruct);
			}
			return sourceMemberMethodSymbol;
		}

		private static MemberModifiers DecodeMethodModifiers(SyntaxTokenList modifiers, SourceMemberContainerTypeSymbol container, Binder binder, DiagnosticBag diagBag)
		{
			MemberModifiers memberModifiers = binder.DecodeModifiers(modifiers, SourceMemberFlags.AllAccessibilityModifiers | SourceMemberFlags.AllOverrideModifiers | SourceMemberFlags.AllShadowingModifiers | SourceMemberFlags.Shared | SourceMemberFlags.Overrides | SourceMemberFlags.Partial | SourceMemberFlags.Async | SourceMemberFlags.Iterator, ERRID.ERR_BadMethodFlags1, Accessibility.Public, diagBag);
			memberModifiers = binder.ValidateSharedPropertyAndMethodModifiers(modifiers, memberModifiers, isProperty: false, container, diagBag);
			if ((memberModifiers.FoundFlags & (SourceMemberFlags.Async | SourceMemberFlags.Iterator)) == (SourceMemberFlags.Async | SourceMemberFlags.Iterator))
			{
				binder.ReportModifierError(modifiers, ERRID.ERR_InvalidAsyncIteratorModifiers, diagBag, InvalidModifiers.InvalidAsyncIterator);
			}
			return memberModifiers;
		}

		private static MemberModifiers DecodeOperatorModifiers(OperatorStatementSyntax syntax, Binder binder, DiagnosticBag diagBag)
		{
			SourceMemberFlags allowableModifiers = SourceMemberFlags.AllAccessibilityModifiers | SourceMemberFlags.AllShadowingModifiers | SourceMemberFlags.AllConversionModifiers | SourceMemberFlags.Shared;
			MemberModifiers memberModifiers = binder.DecodeModifiers(syntax.Modifiers, allowableModifiers, ERRID.ERR_BadOperatorFlags1, Accessibility.Public, diagBag);
			SourceMemberFlags sourceMemberFlags = memberModifiers.FoundFlags;
			SourceMemberFlags sourceMemberFlags2 = memberModifiers.ComputedFlags;
			SourceMemberFlags sourceMemberFlags3 = sourceMemberFlags & SourceMemberFlags.AllAccessibilityModifiers;
			if (sourceMemberFlags3 != 0 && sourceMemberFlags3 != SourceMemberFlags.Public)
			{
				binder.ReportModifierError(syntax.Modifiers, ERRID.ERR_OperatorMustBePublic, diagBag, SyntaxKind.PrivateKeyword, SyntaxKind.ProtectedKeyword, SyntaxKind.FriendKeyword);
				sourceMemberFlags &= ~SourceMemberFlags.AllAccessibilityModifiers;
				sourceMemberFlags2 = (sourceMemberFlags2 & ~SourceMemberFlags.AccessibilityMask) | SourceMemberFlags.AccessibilityPublic;
			}
			if ((sourceMemberFlags & SourceMemberFlags.Shared) == 0)
			{
				Binder.ReportDiagnostic(diagBag, syntax.OperatorToken, ERRID.ERR_OperatorMustBeShared);
				sourceMemberFlags2 |= SourceMemberFlags.Shared;
			}
			if (VisualBasicExtensions.Kind(syntax.OperatorToken) == SyntaxKind.CTypeKeyword)
			{
				if ((sourceMemberFlags & SourceMemberFlags.AllConversionModifiers) == 0)
				{
					Binder.ReportDiagnostic(diagBag, syntax.OperatorToken, ERRID.ERR_ConvMustBeWideningOrNarrowing);
					sourceMemberFlags2 |= SourceMemberFlags.Narrowing;
				}
			}
			else if ((sourceMemberFlags & SourceMemberFlags.AllConversionModifiers) != 0)
			{
				binder.ReportModifierError(syntax.Modifiers, ERRID.ERR_InvalidSpecifierOnNonConversion1, diagBag, SyntaxKind.NarrowingKeyword, SyntaxKind.WideningKeyword);
				sourceMemberFlags &= ~SourceMemberFlags.AllConversionModifiers;
			}
			return new MemberModifiers(sourceMemberFlags, sourceMemberFlags2);
		}

		internal static MemberModifiers DecodeConstructorModifiers(SyntaxTokenList modifiers, SourceMemberContainerTypeSymbol container, Binder binder, DiagnosticBag diagBag)
		{
			MemberModifiers memberModifiers = DecodeMethodModifiers(modifiers, container, binder, diagBag);
			SourceMemberFlags sourceMemberFlags = memberModifiers.FoundFlags;
			SourceMemberFlags sourceMemberFlags2 = memberModifiers.ComputedFlags;
			if ((sourceMemberFlags & (SourceMemberFlags.AllOverrideModifiers | SourceMemberFlags.Shadows)) != 0)
			{
				binder.ReportModifierError(modifiers, ERRID.ERR_BadFlagsOnNew1, diagBag, SyntaxKind.OverridableKeyword, SyntaxKind.MustOverrideKeyword, SyntaxKind.NotOverridableKeyword, SyntaxKind.ShadowsKeyword);
				sourceMemberFlags &= ~(SourceMemberFlags.AllOverrideModifiers | SourceMemberFlags.Shadows);
			}
			if ((sourceMemberFlags & SourceMemberFlags.Overrides) != 0)
			{
				binder.ReportModifierError(modifiers, ERRID.ERR_CantOverrideConstructor, diagBag, SyntaxKind.OverridesKeyword);
				sourceMemberFlags &= ~SourceMemberFlags.Overrides;
			}
			if ((sourceMemberFlags & SourceMemberFlags.Partial) != 0)
			{
				binder.ReportModifierError(modifiers, ERRID.ERR_ConstructorCannotBeDeclaredPartial, diagBag, SyntaxKind.PartialKeyword);
				sourceMemberFlags &= ~SourceMemberFlags.Partial;
			}
			if ((sourceMemberFlags & SourceMemberFlags.Overloads) != 0)
			{
				binder.ReportModifierError(modifiers, ERRID.ERR_BadFlagsOnNewOverloads, diagBag, SyntaxKind.OverloadsKeyword);
				sourceMemberFlags &= ~SourceMemberFlags.Overloads;
			}
			if ((sourceMemberFlags & SourceMemberFlags.Async) != 0)
			{
				binder.ReportModifierError(modifiers, ERRID.ERR_ConstructorAsync, diagBag, SyntaxKind.AsyncKeyword);
			}
			if ((memberModifiers.AllFlags & SourceMemberFlags.Shared) != 0)
			{
				if ((sourceMemberFlags & SourceMemberFlags.AllAccessibilityModifiers) != 0)
				{
					binder.ReportModifierError(modifiers, ERRID.ERR_SharedConstructorIllegalSpec1, diagBag, SyntaxKind.PublicKeyword, SyntaxKind.PrivateKeyword, SyntaxKind.FriendKeyword, SyntaxKind.ProtectedKeyword);
				}
				sourceMemberFlags = (sourceMemberFlags & ~SourceMemberFlags.AllAccessibilityModifiers) | SourceMemberFlags.Private;
				sourceMemberFlags2 = (sourceMemberFlags2 & ~SourceMemberFlags.AccessibilityMask) | SourceMemberFlags.AccessibilityPrivate;
			}
			return new MemberModifiers(sourceMemberFlags, sourceMemberFlags2);
		}

		internal override void GenerateDeclarationErrors(CancellationToken cancellationToken)
		{
			base.GenerateDeclarationErrors(cancellationToken);
			_ = ReturnType;
			GetReturnTypeAttributes();
			ImmutableArray<ParameterSymbol>.Enumerator enumerator = Parameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				ParameterSymbol current = enumerator.Current;
				current.GetAttributes();
				if (current.HasExplicitDefaultValue)
				{
					_ = current.ExplicitDefaultConstantValue;
				}
			}
			ImmutableArray<TypeParameterSymbol>.Enumerator enumerator2 = TypeParameters.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				_ = enumerator2.Current.ConstraintTypesNoUseSiteDiagnostics;
			}
			_ = HandledEvents;
		}

		internal bool SetDiagnostics(ImmutableArray<Diagnostic> diags)
		{
			return ImmutableInterlocked.InterlockedInitialize(ref _cachedDiagnostics, diags);
		}

		internal sealed override bool IsDefinedInSourceTree(SyntaxTree tree, TextSpan? definedWithinSpan, CancellationToken cancellationToken = default(CancellationToken))
		{
			return Symbol.IsDefinedInSourceTree(Syntax, tree, definedWithinSpan, cancellationToken);
		}

		public sealed override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (expandIncludes)
			{
				return SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(this, preferredCulture, expandIncludes, ref _lazyExpandedDocComment, cancellationToken);
			}
			return SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(this, preferredCulture, expandIncludes, ref _lazyDocComment, cancellationToken);
		}

		internal override LexicalSortKey GetLexicalSortKey()
		{
			if (m_syntaxReferenceOpt == null)
			{
				return LexicalSortKey.NotInSource;
			}
			return new LexicalSortKey(m_syntaxReferenceOpt, DeclaringCompilation);
		}

		private Location GetSymbolLocation(SyntaxReference syntaxRef)
		{
			VisualBasicSyntaxNode visualBasicSyntax = VisualBasicExtensions.GetVisualBasicSyntax(syntaxRef);
			return syntaxRef.SyntaxTree.GetLocation(GetMethodLocationFromSyntax(visualBasicSyntax));
		}

		private static TextSpan GetMethodLocationFromSyntax(VisualBasicSyntaxNode node)
		{
			switch (node.Kind())
			{
			case SyntaxKind.SingleLineFunctionLambdaExpression:
			case SyntaxKind.SingleLineSubLambdaExpression:
			case SyntaxKind.MultiLineFunctionLambdaExpression:
			case SyntaxKind.MultiLineSubLambdaExpression:
				return ((LambdaExpressionSyntax)node).SubOrFunctionHeader.Span;
			case SyntaxKind.SubStatement:
			case SyntaxKind.FunctionStatement:
				return ((MethodStatementSyntax)node).Identifier.Span;
			case SyntaxKind.DeclareSubStatement:
			case SyntaxKind.DeclareFunctionStatement:
				return ((DeclareStatementSyntax)node).Identifier.Span;
			case SyntaxKind.SubNewStatement:
				return ((SubNewStatementSyntax)node).NewKeyword.Span;
			case SyntaxKind.OperatorStatement:
				return ((OperatorStatementSyntax)node).OperatorToken.Span;
			default:
				throw ExceptionUtilities.UnexpectedValue(node.Kind());
			}
		}

		internal ImmutableArray<TypeParameterConstraint> BindTypeParameterConstraints(TypeParameterSyntax syntax, BindingDiagnosticBag diagnostics)
		{
			Binder containingBinder = BinderBuilder.CreateBinderForType(ContainingSourceModule, SyntaxTree, m_containingType);
			containingBinder = BinderBuilder.CreateBinderForGenericMethodDeclaration(this, containingBinder);
			if (VisualBasicExtensions.Kind(syntax.VarianceKeyword) != 0)
			{
				Binder.ReportDiagnostic(diagnostics, syntax.VarianceKeyword, ERRID.ERR_VarianceDisallowedHere);
			}
			containingBinder = new LocationSpecificBinder(BindingLocation.GenericConstraintsClause, this, containingBinder);
			return containingBinder.BindTypeParameterConstraintClause(this, syntax.TypeParameterConstraintClause, diagnostics);
		}

		internal static string GetMemberNameFromSyntax(MethodBaseSyntax node)
		{
			switch (node.Kind())
			{
			case SyntaxKind.SubStatement:
			case SyntaxKind.FunctionStatement:
				return ((MethodStatementSyntax)node).Identifier.ValueText;
			case SyntaxKind.PropertyStatement:
				return ((PropertyStatementSyntax)node).Identifier.ValueText;
			case SyntaxKind.DeclareSubStatement:
			case SyntaxKind.DeclareFunctionStatement:
				return ((DeclareStatementSyntax)node).Identifier.ValueText;
			case SyntaxKind.OperatorStatement:
			{
				OperatorStatementSyntax operatorStatementSyntax = (OperatorStatementSyntax)node;
				switch (VisualBasicExtensions.Kind(operatorStatementSyntax.OperatorToken))
				{
				case SyntaxKind.NotKeyword:
					return "op_OnesComplement";
				case SyntaxKind.IsTrueKeyword:
					return "op_True";
				case SyntaxKind.IsFalseKeyword:
					return "op_False";
				case SyntaxKind.PlusToken:
					if (operatorStatementSyntax.ParameterList.Parameters.Count <= 1)
					{
						return "op_UnaryPlus";
					}
					return "op_Addition";
				case SyntaxKind.MinusToken:
					if (operatorStatementSyntax.ParameterList.Parameters.Count <= 1)
					{
						return "op_UnaryNegation";
					}
					return "op_Subtraction";
				case SyntaxKind.AsteriskToken:
					return "op_Multiply";
				case SyntaxKind.SlashToken:
					return "op_Division";
				case SyntaxKind.BackslashToken:
					return "op_IntegerDivision";
				case SyntaxKind.ModKeyword:
					return "op_Modulus";
				case SyntaxKind.CaretToken:
					return "op_Exponent";
				case SyntaxKind.EqualsToken:
					return "op_Equality";
				case SyntaxKind.LessThanGreaterThanToken:
					return "op_Inequality";
				case SyntaxKind.LessThanToken:
					return "op_LessThan";
				case SyntaxKind.GreaterThanToken:
					return "op_GreaterThan";
				case SyntaxKind.LessThanEqualsToken:
					return "op_LessThanOrEqual";
				case SyntaxKind.GreaterThanEqualsToken:
					return "op_GreaterThanOrEqual";
				case SyntaxKind.LikeKeyword:
					return "op_Like";
				case SyntaxKind.AmpersandToken:
					return "op_Concatenate";
				case SyntaxKind.AndKeyword:
					return "op_BitwiseAnd";
				case SyntaxKind.OrKeyword:
					return "op_BitwiseOr";
				case SyntaxKind.XorKeyword:
					return "op_ExclusiveOr";
				case SyntaxKind.LessThanLessThanToken:
					return "op_LeftShift";
				case SyntaxKind.GreaterThanGreaterThanToken:
					return "op_RightShift";
				case SyntaxKind.CTypeKeyword:
				{
					SyntaxTokenList.Enumerator enumerator2 = operatorStatementSyntax.Modifiers.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						switch (Binder.MapKeywordToFlag(enumerator2.Current))
						{
						case SourceMemberFlags.Widening:
							return "op_Implicit";
						case SourceMemberFlags.Narrowing:
							return "op_Explicit";
						}
					}
					return "op_Explicit";
				}
				default:
					throw ExceptionUtilities.UnexpectedValue(VisualBasicExtensions.Kind(operatorStatementSyntax.OperatorToken));
				}
			}
			case SyntaxKind.SubNewStatement:
			{
				bool flag = false;
				SyntaxTokenList.Enumerator enumerator = node.Modifiers.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (VisualBasicExtensions.Kind(enumerator.Current) == SyntaxKind.SharedKeyword)
					{
						flag = true;
					}
				}
				if (node.Parent != null && (node.Parent.Kind() == SyntaxKind.ModuleBlock || (node.Parent.Parent != null && node.Parent.Parent.Kind() == SyntaxKind.ModuleBlock)))
				{
					flag = true;
				}
				return flag ? ".cctor" : ".ctor";
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(node.Kind());
			}
		}

		internal static Symbol FindSymbolFromSyntax(MethodBaseSyntax syntax, SyntaxTree tree, NamedTypeSymbol container)
		{
			switch (syntax.Kind())
			{
			case SyntaxKind.GetAccessorStatement:
			case SyntaxKind.SetAccessorStatement:
				if (syntax.Parent.Parent is PropertyBlockSyntax propertyBlockSyntax)
				{
					SyntaxToken identifier3 = propertyBlockSyntax.PropertyStatement.Identifier;
					PropertySymbol propertySymbol = (PropertySymbol)NamedTypeSymbolExtensions.FindMember(container, identifier3.ValueText, SymbolKind.Property, identifier3.Span, tree);
					if ((object)propertySymbol == null)
					{
						return null;
					}
					MethodSymbol methodSymbol3 = ((syntax.Kind() == SyntaxKind.GetAccessorStatement) ? propertySymbol.GetMethod : propertySymbol.SetMethod);
					if (methodSymbol3.Syntax == syntax.Parent)
					{
						return methodSymbol3;
					}
					return null;
				}
				return null;
			case SyntaxKind.AddHandlerAccessorStatement:
			case SyntaxKind.RemoveHandlerAccessorStatement:
			case SyntaxKind.RaiseEventAccessorStatement:
				if (syntax.Parent.Parent is EventBlockSyntax eventBlockSyntax)
				{
					SyntaxToken identifier2 = eventBlockSyntax.EventStatement.Identifier;
					EventSymbol eventSymbol = (EventSymbol)NamedTypeSymbolExtensions.FindMember(container, identifier2.ValueText, SymbolKind.Event, identifier2.Span, tree);
					if ((object)eventSymbol == null)
					{
						return null;
					}
					MethodSymbol methodSymbol2 = null;
					switch (syntax.Kind())
					{
					case SyntaxKind.AddHandlerAccessorStatement:
						methodSymbol2 = eventSymbol.AddMethod;
						break;
					case SyntaxKind.RemoveHandlerAccessorStatement:
						methodSymbol2 = eventSymbol.RemoveMethod;
						break;
					case SyntaxKind.RaiseEventAccessorStatement:
						methodSymbol2 = eventSymbol.RaiseMethod;
						break;
					}
					if ((object)methodSymbol2 != null && methodSymbol2.Syntax == syntax.Parent)
					{
						return methodSymbol2;
					}
					return null;
				}
				return null;
			case SyntaxKind.PropertyStatement:
			{
				SyntaxToken identifier = ((PropertyStatementSyntax)syntax).Identifier;
				return NamedTypeSymbolExtensions.FindMember(container, identifier.ValueText, SymbolKind.Property, identifier.Span, tree);
			}
			case SyntaxKind.EventStatement:
			{
				SyntaxToken identifier4 = ((EventStatementSyntax)syntax).Identifier;
				return NamedTypeSymbolExtensions.FindMember(container, identifier4.ValueText, SymbolKind.Event, identifier4.Span, tree);
			}
			case SyntaxKind.DelegateSubStatement:
			case SyntaxKind.DelegateFunctionStatement:
			{
				SyntaxToken identifier5 = ((DelegateStatementSyntax)syntax).Identifier;
				return NamedTypeSymbolExtensions.FindMember(container, identifier5.ValueText, SymbolKind.NamedType, identifier5.Span, tree);
			}
			default:
			{
				MethodSymbol methodSymbol = (MethodSymbol)NamedTypeSymbolExtensions.FindMember(container, GetMemberNameFromSyntax(syntax), SymbolKind.Method, GetMethodLocationFromSyntax(syntax), tree);
				if ((object)methodSymbol != null)
				{
					MethodSymbol partialImplementationPart = methodSymbol.PartialImplementationPart;
					if ((object)partialImplementationPart != null && partialImplementationPart.Syntax == syntax.Parent)
					{
						methodSymbol = partialImplementationPart;
					}
				}
				return methodSymbol;
			}
			}
		}

		internal Location GetImplementingLocation(MethodSymbol implementedMethod)
		{
			MethodStatementSyntax methodStatementSyntax = null;
			SyntaxTree tree = null;
			SourceMemberContainerTypeSymbol sourceMemberContainerTypeSymbol = m_containingType as SourceMemberContainerTypeSymbol;
			if (m_syntaxReferenceOpt != null)
			{
				methodStatementSyntax = m_syntaxReferenceOpt.GetSyntax() as MethodStatementSyntax;
				tree = m_syntaxReferenceOpt.SyntaxTree;
			}
			if (methodStatementSyntax != null && methodStatementSyntax.ImplementsClause != null && (object)sourceMemberContainerTypeSymbol != null)
			{
				Binder binder = BinderBuilder.CreateBinderForType(sourceMemberContainerTypeSymbol.ContainingSourceModule, tree, sourceMemberContainerTypeSymbol);
				return ImplementsHelper.FindImplementingSyntax(methodStatementSyntax.ImplementsClause, this, implementedMethod, sourceMemberContainerTypeSymbol, binder).GetLocation();
			}
			return Locations.FirstOrDefault() ?? NoLocation.Singleton;
		}

		internal override BoundBlock GetBoundMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, ref Binder methodBodyBinder = null)
		{
			SyntaxTree syntaxTree = SyntaxTree;
			MethodBlockBaseSyntax blockSyntax = BlockSyntax;
			methodBodyBinder = BinderBuilder.CreateBinderForMethodBody(ContainingSourceModule, syntaxTree, this);
			BoundStatement boundStatement = methodBodyBinder.BindStatement(blockSyntax, diagnostics);
			if (boundStatement.Kind == BoundKind.Block)
			{
				return (BoundBlock)boundStatement;
			}
			return new BoundBlock(blockSyntax, blockSyntax.Statements, ImmutableArray<LocalSymbol>.Empty, ImmutableArray.Create(boundStatement));
		}

		internal sealed override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			MethodBlockBaseSyntax blockSyntax = BlockSyntax;
			if (blockSyntax != null && localTree == blockSyntax.SyntaxTree)
			{
				if (localPosition == blockSyntax.BlockStatement.SpanStart)
				{
					return -1;
				}
				TextSpan span = blockSyntax.Statements.Span;
				if (span.Contains(localPosition))
				{
					return localPosition - span.Start;
				}
			}
			int syntaxOffset = default(int);
			if (((SourceNamedTypeSymbol)ContainingType).TryCalculateSyntaxOffsetOfPositionInInitializer(localPosition, localTree, IsShared, ref syntaxOffset))
			{
				return syntaxOffset;
			}
			throw ExceptionUtilities.Unreachable;
		}

		internal sealed override bool TryGetMeParameter(out ParameterSymbol meParameter)
		{
			if (IsShared)
			{
				meParameter = null;
			}
			else
			{
				if ((object)_lazyMeParameter == null)
				{
					Interlocked.CompareExchange(ref _lazyMeParameter, new MeParameterSymbol(this), null);
				}
				meParameter = _lazyMeParameter;
			}
			return true;
		}

		protected virtual OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
		{
			return OneOrMany.Create(AttributeDeclarationSyntaxList);
		}

		protected virtual OneOrMany<SyntaxList<AttributeListSyntax>> GetReturnTypeAttributeDeclarations()
		{
			return OneOrMany.Create(ReturnTypeAttributeDeclarationSyntaxList);
		}

		private CustomAttributesBag<VisualBasicAttributeData> GetAttributesBag()
		{
			return GetAttributesBag(ref m_lazyCustomAttributesBag, forReturnType: false);
		}

		private CustomAttributesBag<VisualBasicAttributeData> GetReturnTypeAttributesBag()
		{
			return GetAttributesBag(ref m_lazyReturnTypeCustomAttributesBag, forReturnType: true);
		}

		private CustomAttributesBag<VisualBasicAttributeData> GetAttributesBag(ref CustomAttributesBag<VisualBasicAttributeData> lazyCustomAttributesBag, bool forReturnType)
		{
			if (lazyCustomAttributesBag == null || !lazyCustomAttributesBag.IsSealed)
			{
				if (forReturnType)
				{
					SourcePropertySymbol boundReturnTypeAttributesSource = BoundReturnTypeAttributesSource;
					if ((object)boundReturnTypeAttributesSource != null)
					{
						CustomAttributesBag<VisualBasicAttributeData> returnTypeAttributesBag = boundReturnTypeAttributesSource.GetReturnTypeAttributesBag();
						Interlocked.CompareExchange(ref lazyCustomAttributesBag, returnTypeAttributesBag, null);
					}
					else
					{
						LoadAndValidateAttributes(GetReturnTypeAttributeDeclarations(), ref lazyCustomAttributesBag, AttributeLocation.Return);
					}
				}
				else
				{
					SourceMethodSymbol boundAttributesSource = BoundAttributesSource;
					if ((object)boundAttributesSource != null)
					{
						CustomAttributesBag<VisualBasicAttributeData> attributesBag = boundAttributesSource.GetAttributesBag();
						Interlocked.CompareExchange(ref lazyCustomAttributesBag, attributesBag, null);
					}
					else
					{
						LoadAndValidateAttributes(GetAttributeDeclarations(), ref lazyCustomAttributesBag);
					}
				}
			}
			return lazyCustomAttributesBag;
		}

		public sealed override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return GetAttributesBag().Attributes;
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			VisualBasicCompilation declaringCompilation = DeclaringCompilation;
			MethodSymbol entryPoint = declaringCompilation.GetEntryPoint(CancellationToken.None);
			if ((object)this == entryPoint && !HasSTAThreadOrMTAThreadAttribute)
			{
				Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_STAThreadAttribute__ctor));
			}
		}

		internal override void AddSynthesizedReturnTypeAttributes(ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedReturnTypeAttributes(ref attributes);
			if (TypeSymbolExtensions.ContainsTupleNames(ReturnType))
			{
				Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.SynthesizeTupleNamesAttribute(ReturnType));
			}
		}

		protected MethodWellKnownAttributeData GetDecodedWellKnownAttributeData()
		{
			CustomAttributesBag<VisualBasicAttributeData> customAttributesBag = m_lazyCustomAttributesBag;
			if (customAttributesBag == null || !customAttributesBag.IsDecodedWellKnownAttributeDataComputed)
			{
				customAttributesBag = GetAttributesBag();
			}
			return (MethodWellKnownAttributeData)customAttributesBag.DecodedWellKnownAttributeData;
		}

		public sealed override ImmutableArray<VisualBasicAttributeData> GetReturnTypeAttributes()
		{
			return GetReturnTypeAttributesBag().Attributes;
		}

		private CommonReturnTypeWellKnownAttributeData GetDecodedReturnTypeWellKnownAttributeData()
		{
			CustomAttributesBag<VisualBasicAttributeData> customAttributesBag = m_lazyReturnTypeCustomAttributesBag;
			if (customAttributesBag == null || !customAttributesBag.IsDecodedWellKnownAttributeDataComputed)
			{
				customAttributesBag = GetReturnTypeAttributesBag();
			}
			return (CommonReturnTypeWellKnownAttributeData)customAttributesBag.DecodedWellKnownAttributeData;
		}

		internal override VisualBasicAttributeData EarlyDecodeWellKnownAttribute(ref EarlyDecodeWellKnownAttributeArguments<EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation> arguments)
		{
			bool generatedDiagnostics = false;
			if (arguments.SymbolPart != AttributeLocation.Return)
			{
				if (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.CaseInsensitiveExtensionAttribute))
				{
					bool flag = false;
					if ((MethodKind == MethodKind.Ordinary || MethodKind == MethodKind.DeclareMethod) && NamedTypeSymbolExtensions.AllowsExtensionMethods(m_containingType) && ParameterCount != 0)
					{
						ParameterSymbol parameterSymbol = Parameters[0];
						if (!parameterSymbol.IsOptional && !parameterSymbol.IsParamArray && ValidateGenericConstraintsOnExtensionMethodDefinition())
						{
							flag = m_containingType.MightContainExtensionMethods;
						}
					}
					if (flag)
					{
						SourceAttributeData attribute = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out generatedDiagnostics);
						if (!attribute.HasErrors)
						{
							arguments.GetOrCreateData<MethodEarlyWellKnownAttributeData>().IsExtensionMethod = true;
							return (!generatedDiagnostics) ? attribute : null;
						}
					}
					return null;
				}
				if (VisualBasicAttributeData.IsTargetEarlyAttribute(arguments.AttributeType, arguments.AttributeSyntax, AttributeDescription.ConditionalAttribute))
				{
					SourceAttributeData attribute2 = arguments.Binder.GetAttribute(arguments.AttributeSyntax, arguments.AttributeType, out generatedDiagnostics);
					if (!attribute2.HasErrors)
					{
						string constructorArgument = attribute2.GetConstructorArgument<string>(0, SpecialType.System_String);
						arguments.GetOrCreateData<MethodEarlyWellKnownAttributeData>().AddConditionalSymbol(constructorArgument);
						return (!generatedDiagnostics) ? attribute2 : null;
					}
					return null;
				}
				VisualBasicAttributeData boundAttribute = null;
				ObsoleteAttributeData obsoleteData = null;
				if (EarlyDecodeDeprecatedOrExperimentalOrObsoleteAttribute(ref arguments, out boundAttribute, out obsoleteData))
				{
					if (obsoleteData != null)
					{
						arguments.GetOrCreateData<MethodEarlyWellKnownAttributeData>().ObsoleteAttributeData = obsoleteData;
					}
					return boundAttribute;
				}
			}
			return base.EarlyDecodeWellKnownAttribute(ref arguments);
		}

		private MethodEarlyWellKnownAttributeData GetEarlyDecodedWellKnownAttributeData()
		{
			CustomAttributesBag<VisualBasicAttributeData> customAttributesBag = m_lazyCustomAttributesBag;
			if (customAttributesBag == null || !customAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed)
			{
				customAttributesBag = GetAttributesBag();
			}
			return (MethodEarlyWellKnownAttributeData)customAttributesBag.EarlyDecodedWellKnownAttributeData;
		}

		internal override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			return GetEarlyDecodedWellKnownAttributeData()?.ConditionalSymbols ?? ImmutableArray<string>.Empty;
		}

		internal override void DecodeWellKnownAttribute(ref DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation> arguments)
		{
			VisualBasicAttributeData attribute = arguments.Attribute;
			if (attribute.IsTargetAttribute(this, AttributeDescription.TupleElementNamesAttribute))
			{
				((BindingDiagnosticBag)arguments.Diagnostics).Add(ERRID.ERR_ExplicitTupleElementNamesAttribute, arguments.AttributeSyntaxOpt!.Location);
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.UnmanagedCallersOnlyAttribute))
			{
				((BindingDiagnosticBag)arguments.Diagnostics).Add(ERRID.ERR_UnmanagedCallersOnlyNotSupported, arguments.AttributeSyntaxOpt!.Location);
			}
			if (arguments.SymbolPart == AttributeLocation.Return)
			{
				DecodeWellKnownAttributeAppliedToReturnValue(ref arguments);
			}
			else
			{
				DecodeWellKnownAttributeAppliedToMethod(ref arguments);
			}
			base.DecodeWellKnownAttribute(ref arguments);
		}

		private void DecodeWellKnownAttributeAppliedToMethod(ref DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation> arguments)
		{
			BindingDiagnosticBag bindingDiagnosticBag = (BindingDiagnosticBag)arguments.Diagnostics;
			VisualBasicAttributeData attribute = arguments.Attribute;
			if (attribute.IsTargetAttribute(this, AttributeDescription.CaseInsensitiveExtensionAttribute))
			{
				if (MethodKind != MethodKind.Ordinary && MethodKind != MethodKind.DeclareMethod)
				{
					bindingDiagnosticBag.Add(ERRID.ERR_ExtensionOnlyAllowedOnModuleSubOrFunction, arguments.AttributeSyntaxOpt!.GetLocation());
					return;
				}
				if (!NamedTypeSymbolExtensions.AllowsExtensionMethods(m_containingType))
				{
					bindingDiagnosticBag.Add(ERRID.ERR_ExtensionMethodNotInModule, arguments.AttributeSyntaxOpt!.GetLocation());
					return;
				}
				if (ParameterCount == 0)
				{
					bindingDiagnosticBag.Add(ERRID.ERR_ExtensionMethodNoParams, Locations[0]);
					return;
				}
				ParameterSymbol parameterSymbol = Parameters[0];
				if (parameterSymbol.IsOptional)
				{
					bindingDiagnosticBag.Add(ErrorFactory.ErrorInfo(ERRID.ERR_ExtensionMethodOptionalFirstArg), parameterSymbol.Locations[0]);
				}
				else if (parameterSymbol.IsParamArray)
				{
					bindingDiagnosticBag.Add(ErrorFactory.ErrorInfo(ERRID.ERR_ExtensionMethodParamArrayFirstArg), parameterSymbol.Locations[0]);
				}
				else if (!ValidateGenericConstraintsOnExtensionMethodDefinition())
				{
					bindingDiagnosticBag.Add(ErrorFactory.ErrorInfo(ERRID.ERR_ExtensionMethodUncallable1, Name), Locations[0]);
				}
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.WebMethodAttribute))
			{
				ImmutableArray<ParameterSymbol>.Enumerator enumerator = Parameters.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsOptional)
					{
						bindingDiagnosticBag.Add(ErrorFactory.ErrorInfo(ERRID.ERR_InvalidOptionalParameterUsage1, "WebMethod"), Locations[0]);
					}
				}
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.PreserveSigAttribute))
			{
				arguments.GetOrCreateData<MethodWellKnownAttributeData>().SetPreserveSignature(arguments.Index);
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.MethodImplAttribute))
			{
				AttributeData.DecodeMethodImplAttribute<MethodWellKnownAttributeData, AttributeSyntax, VisualBasicAttributeData, AttributeLocation>(ref arguments, MessageProvider.Instance);
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.DllImportAttribute))
			{
				if (!IsDllImportAttributeAllowed(arguments.AttributeSyntaxOpt, bindingDiagnosticBag))
				{
					return;
				}
				string text = attribute.CommonConstructorArguments[0].ValueInternal as string;
				if (!MetadataHelpers.IsValidMetadataIdentifier(text))
				{
					bindingDiagnosticBag.Add(ERRID.ERR_BadAttribute1, arguments.AttributeSyntaxOpt!.ArgumentList.Arguments[0].GetLocation(), attribute.AttributeClass);
				}
				CharSet? effectiveDefaultMarshallingCharSet;
				CharSet? charSet = (effectiveDefaultMarshallingCharSet = base.EffectiveDefaultMarshallingCharSet);
				CharSet charSet2 = ((!charSet.HasValue) ? CharSet.None : effectiveDefaultMarshallingCharSet.GetValueOrDefault());
				string text2 = null;
				bool preserveSig = true;
				System.Runtime.InteropServices.CallingConvention callingConvention = System.Runtime.InteropServices.CallingConvention.Winapi;
				bool setLastError = false;
				bool exactSpelling = false;
				bool? useBestFit = null;
				bool? throwOnUnmappable = null;
				int num = 1;
				ImmutableArray<KeyValuePair<string, TypedConstant>>.Enumerator enumerator2 = attribute.CommonNamedArguments.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					KeyValuePair<string, TypedConstant> current = enumerator2.Current;
					string key = current.Key;
					switch (_003CPrivateImplementationDetails_003E.ComputeStringHash(key))
					{
					case 610589883u:
						if (EmbeddedOperators.CompareString(key, "EntryPoint", TextCompare: false) == 0)
						{
							text2 = current.Value.ValueInternal as string;
							if (!MetadataHelpers.IsValidMetadataIdentifier(text2))
							{
								bindingDiagnosticBag.Add(ERRID.ERR_BadAttribute1, arguments.AttributeSyntaxOpt!.ArgumentList.Arguments[num].GetLocation(), attribute.AttributeClass);
								return;
							}
						}
						break;
					case 1916247947u:
						if (EmbeddedOperators.CompareString(key, "CharSet", TextCompare: false) == 0)
						{
							charSet2 = current.Value.DecodeValue<CharSet>(SpecialType.System_Enum);
						}
						break;
					case 568184203u:
						if (EmbeddedOperators.CompareString(key, "SetLastError", TextCompare: false) == 0)
						{
							setLastError = current.Value.DecodeValue<bool>(SpecialType.System_Boolean);
						}
						break;
					case 2102564466u:
						if (EmbeddedOperators.CompareString(key, "ExactSpelling", TextCompare: false) == 0)
						{
							exactSpelling = current.Value.DecodeValue<bool>(SpecialType.System_Boolean);
						}
						break;
					case 799797190u:
						if (EmbeddedOperators.CompareString(key, "PreserveSig", TextCompare: false) == 0)
						{
							preserveSig = current.Value.DecodeValue<bool>(SpecialType.System_Boolean);
						}
						break;
					case 2826917920u:
						if (EmbeddedOperators.CompareString(key, "CallingConvention", TextCompare: false) == 0)
						{
							callingConvention = current.Value.DecodeValue<System.Runtime.InteropServices.CallingConvention>(SpecialType.System_Enum);
						}
						break;
					case 1683410592u:
						if (EmbeddedOperators.CompareString(key, "BestFitMapping", TextCompare: false) == 0)
						{
							useBestFit = current.Value.DecodeValue<bool>(SpecialType.System_Boolean);
						}
						break;
					case 3232400899u:
						if (EmbeddedOperators.CompareString(key, "ThrowOnUnmappableChar", TextCompare: false) == 0)
						{
							throwOnUnmappable = current.Value.DecodeValue<bool>(SpecialType.System_Boolean);
						}
						break;
					}
					num++;
				}
				arguments.GetOrCreateData<MethodWellKnownAttributeData>().SetDllImport(arguments.Index, text, text2, DllImportData.MakeFlags(exactSpelling, charSet2, setLastError, callingConvention, useBestFit, throwOnUnmappable), preserveSig);
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.SpecialNameAttribute))
			{
				arguments.GetOrCreateData<MethodWellKnownAttributeData>().HasSpecialNameAttribute = true;
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.ExcludeFromCodeCoverageAttribute))
			{
				arguments.GetOrCreateData<MethodWellKnownAttributeData>().HasExcludeFromCodeCoverageAttribute = true;
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.SuppressUnmanagedCodeSecurityAttribute))
			{
				arguments.GetOrCreateData<MethodWellKnownAttributeData>().HasSuppressUnmanagedCodeSecurityAttribute = true;
			}
			else if (attribute.IsSecurityAttribute(DeclaringCompilation))
			{
				attribute.DecodeSecurityAttribute<MethodWellKnownAttributeData>(this, DeclaringCompilation, ref arguments);
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.STAThreadAttribute))
			{
				arguments.GetOrCreateData<MethodWellKnownAttributeData>().HasSTAThreadAttribute = true;
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.MTAThreadAttribute))
			{
				arguments.GetOrCreateData<MethodWellKnownAttributeData>().HasMTAThreadAttribute = true;
			}
			else if (attribute.IsTargetAttribute(this, AttributeDescription.ConditionalAttribute))
			{
				if (!IsSub)
				{
					bindingDiagnosticBag.Add(ERRID.WRN_ConditionalNotValidOnFunction, Locations[0]);
				}
			}
			else
			{
				if (VerifyObsoleteAttributeAppliedToMethod(ref arguments, AttributeDescription.ObsoleteAttribute) || VerifyObsoleteAttributeAppliedToMethod(ref arguments, AttributeDescription.DeprecatedAttribute))
				{
					return;
				}
				if (arguments.Attribute.IsTargetAttribute(this, AttributeDescription.ModuleInitializerAttribute))
				{
					bindingDiagnosticBag.Add(ERRID.WRN_AttributeNotSupportedInVB, arguments.AttributeSyntaxOpt!.Location, AttributeDescription.ModuleInitializerAttribute.FullName);
					return;
				}
				MethodSymbol methodSymbol = (IsPartial ? PartialImplementationPart : this);
				if ((object)methodSymbol != null && (methodSymbol.IsAsync || methodSymbol.IsIterator) && !TypeSymbolExtensions.IsInterfaceType(methodSymbol.ContainingType))
				{
					if (attribute.IsTargetAttribute(this, AttributeDescription.SecurityCriticalAttribute))
					{
						Binder.ReportDiagnostic(bindingDiagnosticBag, arguments.AttributeSyntaxOpt!.GetLocation(), ERRID.ERR_SecurityCriticalAsync, "SecurityCritical");
					}
					else if (attribute.IsTargetAttribute(this, AttributeDescription.SecuritySafeCriticalAttribute))
					{
						Binder.ReportDiagnostic(bindingDiagnosticBag, arguments.AttributeSyntaxOpt!.GetLocation(), ERRID.ERR_SecurityCriticalAsync, "SecuritySafeCritical");
					}
				}
			}
		}

		private bool VerifyObsoleteAttributeAppliedToMethod(ref DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation> arguments, AttributeDescription description)
		{
			if (arguments.Attribute.IsTargetAttribute(this, description))
			{
				if (SymbolExtensions.IsAccessor(this) && AssociatedSymbol.Kind == SymbolKind.Event)
				{
					((BindingDiagnosticBag)arguments.Diagnostics).Add(ERRID.ERR_ObsoleteInvalidOnEventMember, Locations[0], description.FullName);
				}
				return true;
			}
			return false;
		}

		private void DecodeWellKnownAttributeAppliedToReturnValue(ref DecodeWellKnownAttributeArguments<AttributeSyntax, VisualBasicAttributeData, AttributeLocation> arguments)
		{
			if (arguments.Attribute.IsTargetAttribute(this, AttributeDescription.MarshalAsAttribute))
			{
				MarshalAsAttributeDecoder<CommonReturnTypeWellKnownAttributeData, AttributeSyntax, VisualBasicAttributeData, AttributeLocation>.Decode(ref arguments, AttributeTargets.ReturnValue, MessageProvider.Instance);
			}
		}

		private bool IsDllImportAttributeAllowed(AttributeSyntax syntax, BindingDiagnosticBag diagnostics)
		{
			switch (MethodKind)
			{
			case MethodKind.DeclareMethod:
				diagnostics.Add(ERRID.ERR_DllImportNotLegalOnDeclare, syntax.Name.GetLocation());
				return false;
			case MethodKind.PropertyGet:
			case MethodKind.PropertySet:
				diagnostics.Add(ERRID.ERR_DllImportNotLegalOnGetOrSet, syntax.Name.GetLocation());
				return false;
			case MethodKind.EventAdd:
			case MethodKind.EventRaise:
			case MethodKind.EventRemove:
				diagnostics.Add(ERRID.ERR_DllImportNotLegalOnEventMethod, syntax.Name.GetLocation());
				return false;
			default:
				if ((object)ContainingType != null && ContainingType.IsInterface)
				{
					diagnostics.Add(ERRID.ERR_DllImportOnInterfaceMethod, syntax.Name.GetLocation());
					return false;
				}
				if (IsGenericMethod || ((object)ContainingType != null && ContainingType.IsGenericType))
				{
					diagnostics.Add(ERRID.ERR_DllImportOnGenericSubOrFunction, syntax.Name.GetLocation());
					return false;
				}
				if (!IsShared)
				{
					diagnostics.Add(ERRID.ERR_DllImportOnInstanceMethod, syntax.Name.GetLocation());
					return false;
				}
				if ((IsPartial ? PartialImplementationPart : this) is SourceMethodSymbol sourceMethodSymbol && (sourceMethodSymbol.IsAsync || sourceMethodSymbol.IsIterator) && !TypeSymbolExtensions.IsInterfaceType(sourceMethodSymbol.ContainingType))
				{
					Location nonMergedLocation = sourceMethodSymbol.NonMergedLocation;
					if ((object)nonMergedLocation != null)
					{
						Binder.ReportDiagnostic(diagnostics, nonMergedLocation, ERRID.ERR_DllImportOnResumableMethod);
						return false;
					}
				}
				if (!HasEmptyBody)
				{
					diagnostics.Add(ERRID.ERR_DllImportOnNonEmptySubOrFunction, syntax.Name.GetLocation());
					return false;
				}
				return true;
			}
		}

		internal override void PostDecodeWellKnownAttributes(ImmutableArray<VisualBasicAttributeData> boundAttributes, ImmutableArray<AttributeSyntax> allAttributeSyntaxNodes, BindingDiagnosticBag diagnostics, AttributeLocation symbolPart, WellKnownAttributeData decodedData)
		{
			if (symbolPart != AttributeLocation.Return)
			{
				MethodWellKnownAttributeData methodWellKnownAttributeData = (MethodWellKnownAttributeData)decodedData;
				if (methodWellKnownAttributeData != null && methodWellKnownAttributeData.HasSTAThreadAttribute && methodWellKnownAttributeData.HasMTAThreadAttribute)
				{
					diagnostics.Add(ERRID.ERR_STAThreadAndMTAThread0, NonMergedLocation);
				}
			}
			base.PostDecodeWellKnownAttributes(boundAttributes, allAttributeSyntaxNodes, diagnostics, symbolPart, decodedData);
		}

		public override DllImportData GetDllImportData()
		{
			return GetDecodedWellKnownAttributeData()?.DllImportPlatformInvokeData;
		}

		internal sealed override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			CustomAttributesBag<VisualBasicAttributeData> attributesBag = GetAttributesBag();
			MethodWellKnownAttributeData methodWellKnownAttributeData = (MethodWellKnownAttributeData)attributesBag.DecodedWellKnownAttributeData;
			if (methodWellKnownAttributeData != null)
			{
				SecurityWellKnownAttributeData securityInformation = methodWellKnownAttributeData.SecurityInformation;
				if (securityInformation != null)
				{
					return securityInformation.GetSecurityAttributes(attributesBag.Attributes);
				}
			}
			return SpecializedCollections.EmptyEnumerable<SecurityAttribute>();
		}

		private bool IsVtableGapInterfaceMethod()
		{
			if (ContainingType.IsInterface)
			{
				return ModuleExtensions.GetVTableGapSize(MetadataName) > 0;
			}
			return false;
		}
	}
}
