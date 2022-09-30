using System;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class SpeculativeSyntaxTreeSemanticModel : SyntaxTreeSemanticModel
	{
		private readonly SyntaxTreeSemanticModel _parentSemanticModel;

		private readonly ExpressionSyntax _root;

		private readonly Binder _rootBinder;

		private readonly int _position;

		private readonly SpeculativeBindingOption _bindingOption;

		public override bool IsSpeculativeSemanticModel => true;

		public override int OriginalPositionForSpeculation => _position;

		public override SemanticModel ParentModel => _parentSemanticModel;

		internal override SyntaxNode Root => _root;

		public override SyntaxTree SyntaxTree => _root.SyntaxTree;

		public static SpeculativeSyntaxTreeSemanticModel Create(SyntaxTreeSemanticModel parentSemanticModel, ExpressionSyntax root, Binder binder, int position, SpeculativeBindingOption bindingOption)
		{
			return new SpeculativeSyntaxTreeSemanticModel(parentSemanticModel, root, binder, position, bindingOption);
		}

		private SpeculativeSyntaxTreeSemanticModel(SyntaxTreeSemanticModel parentSemanticModel, ExpressionSyntax root, Binder binder, int position, SpeculativeBindingOption bindingOption)
			: base(parentSemanticModel.Compilation, (SourceModuleSymbol)parentSemanticModel.Compilation.SourceModule, root.SyntaxTree)
		{
			_parentSemanticModel = parentSemanticModel;
			_root = root;
			_rootBinder = binder;
			_position = position;
			_bindingOption = bindingOption;
		}

		internal override BoundNode Bind(Binder binder, SyntaxNode node, BindingDiagnosticBag diagnostics)
		{
			return _parentSemanticModel.Bind(binder, node, diagnostics);
		}

		internal override Binder GetEnclosingBinder(int position)
		{
			return _rootBinder;
		}

		private SpeculativeBindingOption GetSpeculativeBindingOption(ExpressionSyntax node)
		{
			if (SyntaxFacts.IsInNamespaceOrTypeContext(node))
			{
				return SpeculativeBindingOption.BindAsTypeOrNamespace;
			}
			return _bindingOption;
		}

		internal override SymbolInfo GetExpressionSymbolInfo(ExpressionSyntax node, SymbolInfoOptions options, CancellationToken cancellationToken = default(CancellationToken))
		{
			if ((options & SymbolInfoOptions.PreserveAliases) != 0)
			{
				IAliasSymbol speculativeAliasInfo = _parentSemanticModel.GetSpeculativeAliasInfo(_position, (IdentifierNameSyntax)node, GetSpeculativeBindingOption(node));
				return SymbolInfoFactory.Create(ImmutableArray.Create((ISymbol)speculativeAliasInfo), (speculativeAliasInfo != null) ? LookupResultKind.Good : LookupResultKind.Empty);
			}
			return _parentSemanticModel.GetSpeculativeSymbolInfo(_position, node, GetSpeculativeBindingOption(node));
		}

		internal override VisualBasicTypeInfo GetExpressionTypeInfo(ExpressionSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _parentSemanticModel.GetSpeculativeTypeInfoWorker(_position, node, GetSpeculativeBindingOption(node));
		}

		internal override ImmutableArray<Symbol> GetExpressionMemberGroup(ExpressionSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _parentSemanticModel.GetExpressionMemberGroup(node, cancellationToken);
		}

		internal override ConstantValue GetExpressionConstantValue(ExpressionSyntax node, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _parentSemanticModel.GetExpressionConstantValue(node, cancellationToken);
		}

		public override ImmutableArray<Diagnostic> GetSyntaxDiagnostics(TextSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			throw new NotSupportedException();
		}

		public override ImmutableArray<Diagnostic> GetDeclarationDiagnostics(TextSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			throw new NotSupportedException();
		}

		public override ImmutableArray<Diagnostic> GetDiagnostics(TextSpan? span = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			throw new NotSupportedException();
		}
	}
}
