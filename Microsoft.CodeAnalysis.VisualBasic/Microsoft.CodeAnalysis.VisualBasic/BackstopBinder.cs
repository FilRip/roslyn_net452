using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BackstopBinder : Binder
	{
		private static readonly Dictionary<string, string> s_defaultXmlNamespaces = new Dictionary<string, string>
		{
			{ "", "" },
			{ "xml", "http://www.w3.org/XML/1998/namespace" },
			{ "xmlns", "http://www.w3.org/2000/xmlns/" }
		};

		public override ConsList<LocalSymbol> ImplicitlyTypedLocalsBeingBound => ConsList<LocalSymbol>.Empty;

		public override Symbol ContainingMember
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override ImmutableArray<Symbol> AdditionalContainingMembers
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override bool IsInQuery
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override NamespaceOrTypeSymbol ContainingNamespaceOrType
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override NamedTypeSymbol ContainingType
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal override ConstantFieldsInProgress ConstantFieldsInProgress => ConstantFieldsInProgress.Empty;

		internal override SymbolsInProgress<ParameterSymbol> DefaultParametersInProgress => SymbolsInProgress<ParameterSymbol>.Empty;

		public override OptionStrict OptionStrict
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override bool OptionInfer
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override bool OptionExplicit
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override bool OptionCompareText
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override bool CheckOverflow
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		public override bool AllImplicitVariableDeclarationsAreHandled => false;

		public override bool ImplicitVariableDeclarationAllowed => false;

		public override ImmutableArray<LocalSymbol> ImplicitlyDeclaredVariables => ImmutableArray<LocalSymbol>.Empty;

		protected override bool IsInsideChainedConstructorCallArguments => false;

		public override BindingLocation BindingLocation => BindingLocation.None;

		internal override bool HasImportedXmlNamespaces => false;

		public override QuickAttributeChecker QuickAttributeChecker
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal override bool IsDefaultInstancePropertyAllowed => true;

		internal override bool SuppressCallerInfo => false;

		internal override bool SuppressObsoleteDiagnostics => false;

		public override bool IsSemanticModelBinder => false;

		public BackstopBinder()
			: base(null)
		{
		}

		public override AccessCheckResult CheckAccessibility(Symbol sym, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, TypeSymbol accessThroughType = null, BasesBeingResolved basesBeingResolved = default(BasesBeingResolved))
		{
			throw ExceptionUtilities.Unreachable;
		}

		public override Binder GetBinder(SyntaxNode node)
		{
			throw ExceptionUtilities.Unreachable;
		}

		public override Binder GetBinder(SyntaxList<StatementSyntax> stmtList)
		{
			throw ExceptionUtilities.Unreachable;
		}

		public override bool IsUnboundTypeAllowed(GenericNameSyntax syntax)
		{
			return false;
		}

		public override SyntaxReference GetSyntaxReference(VisualBasicSyntaxNode node)
		{
			throw ExceptionUtilities.Unreachable;
		}

		protected override BoundStatement CreateBoundWithBlock(WithBlockSyntax node, Binder boundBlockBinder, BindingDiagnosticBag diagnostics)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override ImmutableArray<Symbol> BindInsideCrefAttributeValue(TypeSyntax name, bool preserveAliases, BindingDiagnosticBag diagnosticBag, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			return ImmutableArray<Symbol>.Empty;
		}

		internal override ImmutableArray<Symbol> BindInsideCrefAttributeValue(CrefReferenceSyntax reference, bool preserveAliases, BindingDiagnosticBag diagnosticBag, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			return ImmutableArray<Symbol>.Empty;
		}

		internal override ImmutableArray<Symbol> BindXmlNameAttributeValue(IdentifierNameSyntax identifier, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			return ImmutableArray<Symbol>.Empty;
		}

		protected internal override BoundExpression TryBindOmittedLeftForMemberAccess(MemberAccessExpressionSyntax node, BindingDiagnosticBag diagnostics, Binder accessingBinder, out bool wholeMemberAccessExpressionBound)
		{
			return null;
		}

		protected override BoundExpression TryBindOmittedLeftForDictionaryAccess(MemberAccessExpressionSyntax node, Binder accessingBinder, BindingDiagnosticBag diagnostics)
		{
			return null;
		}

		protected override BoundExpression TryBindOmittedLeftForConditionalAccess(ConditionalAccessExpressionSyntax node, Binder accessingBinder, BindingDiagnosticBag diagnostics)
		{
			return null;
		}

		protected internal override BoundExpression TryBindOmittedLeftForXmlMemberAccess(XmlMemberAccessExpressionSyntax node, BindingDiagnosticBag diagnostics, Binder accessingBinder)
		{
			return null;
		}

		protected override BoundExpression TryGetConditionalAccessReceiver(ConditionalAccessExpressionSyntax node)
		{
			return null;
		}

		public override LocalSymbol DeclareImplicitLocalVariable(IdentifierNameSyntax nameSyntax, BindingDiagnosticBag diagnostics)
		{
			throw ExceptionUtilities.Unreachable;
		}

		public override void DisallowFurtherImplicitVariableDeclaration(BindingDiagnosticBag diagnostics)
		{
		}

		public override LabelSymbol GetContinueLabel(SyntaxKind continueSyntaxKind)
		{
			return null;
		}

		public override LabelSymbol GetExitLabel(SyntaxKind exitSyntaxKind)
		{
			return null;
		}

		public override LabelSymbol GetReturnLabel()
		{
			return null;
		}

		public override LocalSymbol GetLocalForFunctionValue()
		{
			return null;
		}

		internal override bool LookupXmlNamespace(string prefix, bool ignoreXmlNodes, out string @namespace, out bool fromImports)
		{
			fromImports = false;
			return s_defaultXmlNamespaces.TryGetValue(prefix, out @namespace);
		}

		internal override void GetInScopeXmlNamespaces(ArrayBuilder<KeyValuePair<string, string>> builder)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal override LabelSymbol LookupLabelByNameToken(SyntaxToken labelName)
		{
			return null;
		}

		internal override BoundExpression GetWithStatementPlaceholderSubstitute(BoundValuePlaceholderBase placeholder)
		{
			return null;
		}

		internal override LookupOptions BinderSpecificLookupOptions(LookupOptions options)
		{
			return options;
		}
	}
}
