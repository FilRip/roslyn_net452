using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class ForOrForEachBlockBinder : ExitableStatementBinder
	{
		private readonly ForOrForEachBlockSyntax _syntax;

		private ImmutableArray<LocalSymbol> _locals;

		internal override ImmutableArray<LocalSymbol> Locals
		{
			get
			{
				if (_locals.IsDefault)
				{
					ImmutableInterlocked.InterlockedCompareExchange(ref _locals, BuildLocals(), default(ImmutableArray<LocalSymbol>));
				}
				return _locals;
			}
		}

		public ForOrForEachBlockBinder(Binder enclosing, ForOrForEachBlockSyntax syntax)
			: base(enclosing, SyntaxKind.ContinueForStatement, SyntaxKind.ExitForStatement)
		{
			_locals = default(ImmutableArray<LocalSymbol>);
			_syntax = syntax;
		}

		private ImmutableArray<LocalSymbol> BuildLocals()
		{
			LocalSymbol localSymbol = null;
			VisualBasicSyntaxNode visualBasicSyntaxNode = ((_syntax.Kind() != SyntaxKind.ForBlock) ? ((ForEachStatementSyntax)_syntax.ForOrForEachStatement).ControlVariable : ((ForStatementSyntax)_syntax.ForOrForEachStatement).ControlVariable);
			if (visualBasicSyntaxNode is VariableDeclaratorSyntax variableDeclaratorSyntax)
			{
				ModifiedIdentifierSyntax modifiedIdentifierSyntax = variableDeclaratorSyntax.Names[0];
				localSymbol = LocalSymbol.Create(ContainingMember, this, modifiedIdentifierSyntax.Identifier, modifiedIdentifierSyntax, variableDeclaratorSyntax.AsClause, variableDeclaratorSyntax.Initializer, (_syntax.Kind() == SyntaxKind.ForEachBlock) ? LocalDeclarationKind.ForEach : LocalDeclarationKind.For);
			}
			else if (visualBasicSyntaxNode is IdentifierNameSyntax identifierNameSyntax)
			{
				SyntaxToken identifier = identifierNameSyntax.Identifier;
				if (OptionInfer)
				{
					LookupResult instance = LookupResult.GetInstance();
					Binder containingBinder = base.ContainingBinder;
					string valueText = identifier.ValueText;
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
					containingBinder.Lookup(instance, valueText, 0, LookupOptions.AllMethodsOfAnyArity, ref useSiteInfo);
					if (!instance.IsGoodOrAmbiguous || instance.Symbols[0].Kind == SymbolKind.NamedType || instance.Symbols[0].Kind == SymbolKind.TypeParameter)
					{
						localSymbol = CreateLocalSymbol(identifier);
					}
					instance.Free();
				}
			}
			if ((object)localSymbol != null)
			{
				return ImmutableArray.Create(localSymbol);
			}
			return ImmutableArray<LocalSymbol>.Empty;
		}

		private LocalSymbol CreateLocalSymbol(SyntaxToken identifier)
		{
			if (_syntax.Kind() == SyntaxKind.ForBlock)
			{
				ForStatementSyntax forStatementSyntax = (ForStatementSyntax)_syntax.ForOrForEachStatement;
				return LocalSymbol.CreateInferredForFromTo(ContainingMember, this, identifier, forStatementSyntax.FromValue, forStatementSyntax.ToValue, forStatementSyntax.StepClause);
			}
			ForEachStatementSyntax forEachStatementSyntax = (ForEachStatementSyntax)_syntax.ForOrForEachStatement;
			return LocalSymbol.CreateInferredForEach(ContainingMember, this, identifier, forEachStatementSyntax.Expression);
		}
	}
}
