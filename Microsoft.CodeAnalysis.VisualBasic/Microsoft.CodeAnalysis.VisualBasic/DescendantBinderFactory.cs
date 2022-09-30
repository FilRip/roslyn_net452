using System.Collections.Immutable;
using System.Threading;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class DescendantBinderFactory
	{
		private readonly ExecutableCodeBinder _rootBinder;

		private readonly SyntaxNode _root;

		private ImmutableDictionary<SyntaxNode, BlockBaseBinder> _lazyNodeToBinderMap;

		private ImmutableDictionary<SyntaxList<StatementSyntax>, BlockBaseBinder> _lazyStmtListToBinderMap;

		internal SyntaxNode Root => _root;

		internal ExecutableCodeBinder RootBinder => _rootBinder;

		internal ImmutableDictionary<SyntaxNode, BlockBaseBinder> NodeToBinderMap
		{
			get
			{
				if (_lazyNodeToBinderMap == null)
				{
					BuildBinderMaps();
				}
				return _lazyNodeToBinderMap;
			}
		}

		internal ImmutableDictionary<SyntaxList<StatementSyntax>, BlockBaseBinder> StmtListToBinderMap
		{
			get
			{
				if (_lazyStmtListToBinderMap == null)
				{
					BuildBinderMaps();
				}
				return _lazyStmtListToBinderMap;
			}
		}

		public DescendantBinderFactory(ExecutableCodeBinder binder, SyntaxNode root)
		{
			_rootBinder = binder;
			_root = root;
		}

		internal Binder GetBinder(SyntaxNode node)
		{
			BlockBaseBinder value = null;
			if (NodeToBinderMap.TryGetValue(node, out value))
			{
				return value;
			}
			return null;
		}

		internal Binder GetBinder(SyntaxList<StatementSyntax> statementList)
		{
			BlockBaseBinder value = null;
			if (StmtListToBinderMap.TryGetValue(statementList, out value))
			{
				return value;
			}
			return null;
		}

		private void BuildBinderMaps()
		{
			LocalBinderBuilder localBinderBuilder = new LocalBinderBuilder((MethodSymbol)_rootBinder.ContainingMember);
			localBinderBuilder.MakeBinder(Root, RootBinder);
			Interlocked.CompareExchange(ref _lazyNodeToBinderMap, localBinderBuilder.NodeToBinderMap, null);
			Interlocked.CompareExchange(ref _lazyStmtListToBinderMap, localBinderBuilder.StmtListToBinderMap, null);
		}
	}
}
