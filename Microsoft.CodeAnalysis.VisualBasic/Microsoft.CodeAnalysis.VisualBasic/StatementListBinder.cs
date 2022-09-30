using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class StatementListBinder : BlockBaseBinder
	{
		private readonly SyntaxList<StatementSyntax> _statementList;

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

		public StatementListBinder(Binder containing, SyntaxList<StatementSyntax> statementList)
			: base(containing)
		{
			_locals = default(ImmutableArray<LocalSymbol>);
			_statementList = statementList;
		}

		private ImmutableArray<LocalSymbol> BuildLocals()
		{
			ArrayBuilder<LocalSymbol> arrayBuilder = null;
			SyntaxList<StatementSyntax> statementList = _statementList;
			SyntaxList<StatementSyntax>.Enumerator enumerator = statementList.GetEnumerator();
			while (enumerator.MoveNext())
			{
				StatementSyntax current = enumerator.Current;
				if (current.Kind() != SyntaxKind.LocalDeclarationStatement)
				{
					continue;
				}
				if (arrayBuilder == null)
				{
					arrayBuilder = ArrayBuilder<LocalSymbol>.GetInstance();
				}
				LocalDeclarationStatementSyntax localDeclarationStatementSyntax = (LocalDeclarationStatementSyntax)current;
				LocalDeclarationKind declarationKind = LocalDeclarationKind.Variable;
				SyntaxTokenList.Enumerator enumerator2 = localDeclarationStatementSyntax.Modifiers.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					switch (VisualBasicExtensions.Kind(enumerator2.Current))
					{
					case SyntaxKind.ConstKeyword:
						break;
					case SyntaxKind.StaticKeyword:
						declarationKind = LocalDeclarationKind.Static;
						continue;
					default:
						continue;
					}
					declarationKind = LocalDeclarationKind.Constant;
					break;
				}
				SeparatedSyntaxList<VariableDeclaratorSyntax>.Enumerator enumerator3 = localDeclarationStatementSyntax.Declarators.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					VariableDeclaratorSyntax current2 = enumerator3.Current;
					AsClauseSyntax asClause = current2.AsClause;
					bool flag = current2.Initializer != null && (current2.AsClause == null || current2.AsClause.Kind() != SyntaxKind.AsNewClause);
					SeparatedSyntaxList<ModifiedIdentifierSyntax> names = current2.Names;
					int num = names.Count - 1;
					for (int i = 0; i <= num; i++)
					{
						ModifiedIdentifierSyntax modifiedIdentifierSyntax = names[i];
						LocalSymbol item = LocalSymbol.Create(ContainingMember, this, modifiedIdentifierSyntax.Identifier, modifiedIdentifierSyntax, asClause, (flag && i == names.Count - 1) ? current2.Initializer : null, declarationKind);
						arrayBuilder.Add(item);
					}
				}
			}
			return arrayBuilder?.ToImmutableAndFree() ?? ImmutableArray<LocalSymbol>.Empty;
		}
	}
}
