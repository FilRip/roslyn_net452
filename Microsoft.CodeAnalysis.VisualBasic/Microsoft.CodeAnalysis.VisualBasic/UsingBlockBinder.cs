using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class UsingBlockBinder : BlockBaseBinder
	{
		private readonly UsingBlockSyntax _syntax;

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

		public UsingBlockBinder(Binder enclosing, UsingBlockSyntax syntax)
			: base(enclosing)
		{
			_locals = default(ImmutableArray<LocalSymbol>);
			_syntax = syntax;
		}

		private ImmutableArray<LocalSymbol> BuildLocals()
		{
			SeparatedSyntaxList<VariableDeclaratorSyntax> variables = _syntax.UsingStatement.Variables;
			if (variables.Count > 0)
			{
				ArrayBuilder<LocalSymbol> instance = ArrayBuilder<LocalSymbol>.GetInstance();
				SeparatedSyntaxList<VariableDeclaratorSyntax>.Enumerator enumerator = variables.GetEnumerator();
				while (enumerator.MoveNext())
				{
					VariableDeclaratorSyntax current = enumerator.Current;
					bool flag = current.Initializer != null && (current.AsClause == null || current.AsClause.Kind() != SyntaxKind.AsNewClause);
					SeparatedSyntaxList<ModifiedIdentifierSyntax> names = current.Names;
					int num = names.Count - 1;
					for (int i = 0; i <= num; i++)
					{
						ModifiedIdentifierSyntax modifiedIdentifierSyntax = names[i];
						instance.Add(LocalSymbol.Create(ContainingMember, this, modifiedIdentifierSyntax.Identifier, modifiedIdentifierSyntax, current.AsClause, (flag && i == names.Count - 1) ? current.Initializer : null, LocalDeclarationKind.Using));
					}
				}
				return instance.ToImmutableAndFree();
			}
			return ImmutableArray<LocalSymbol>.Empty;
		}
	}
}
