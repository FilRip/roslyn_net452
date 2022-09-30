using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class ExecutableCodeBinder : Binder
	{
		public class LabelVisitor : StatementSyntaxWalker
		{
			private readonly ArrayBuilder<SourceLabelSymbol> _labels;

			private readonly MethodSymbol _containingMethod;

			private readonly Binder _binder;

			public LabelVisitor(ArrayBuilder<SourceLabelSymbol> labels, MethodSymbol containingMethod, Binder binder)
			{
				_labels = labels;
				_containingMethod = containingMethod;
				_binder = binder;
			}

			public override void VisitLabelStatement(LabelStatementSyntax node)
			{
				_labels.Add(new SourceLabelSymbol(node.LabelToken, _containingMethod, _binder));
			}
		}

		private readonly SyntaxNode _syntaxRoot;

		private readonly DescendantBinderFactory _descendantBinderFactory;

		private MultiDictionary<string, SourceLabelSymbol> _labelsMap;

		private ImmutableArray<SourceLabelSymbol> _labels;

		private static readonly MultiDictionary<string, SourceLabelSymbol> s_emptyLabelMap = new MultiDictionary<string, SourceLabelSymbol>(0, CaseInsensitiveComparison.Comparer);

		internal ImmutableArray<SourceLabelSymbol> Labels
		{
			get
			{
				if (_labels.IsDefault)
				{
					ImmutableInterlocked.InterlockedCompareExchange(ref _labels, BuildLabels(), default(ImmutableArray<SourceLabelSymbol>));
				}
				return _labels;
			}
		}

		private MultiDictionary<string, SourceLabelSymbol> LabelsMap
		{
			get
			{
				if (_labelsMap == null)
				{
					Interlocked.CompareExchange(ref _labelsMap, BuildLabelsMap(Labels), null);
				}
				return _labelsMap;
			}
		}

		public SyntaxNode Root => _descendantBinderFactory.Root;

		public ImmutableDictionary<SyntaxNode, BlockBaseBinder> NodeToBinderMap => _descendantBinderFactory.NodeToBinderMap;

		internal ImmutableDictionary<SyntaxList<StatementSyntax>, BlockBaseBinder> StmtListToBinderMap => _descendantBinderFactory.StmtListToBinderMap;

		public ExecutableCodeBinder(SyntaxNode root, Binder containingBinder)
			: base(containingBinder)
		{
			_labels = default(ImmutableArray<SourceLabelSymbol>);
			_syntaxRoot = root;
			_descendantBinderFactory = new DescendantBinderFactory(this, root);
		}

		private ImmutableArray<SourceLabelSymbol> BuildLabels()
		{
			ArrayBuilder<SourceLabelSymbol> instance = ArrayBuilder<SourceLabelSymbol>.GetInstance();
			LabelVisitor labelVisitor = new LabelVisitor(instance, (MethodSymbol)ContainingMember, this);
			switch (VisualBasicExtensions.Kind(_syntaxRoot))
			{
			case SyntaxKind.SingleLineFunctionLambdaExpression:
			case SyntaxKind.SingleLineSubLambdaExpression:
				labelVisitor.Visit(((SingleLineLambdaExpressionSyntax)_syntaxRoot).Body);
				break;
			case SyntaxKind.MultiLineFunctionLambdaExpression:
			case SyntaxKind.MultiLineSubLambdaExpression:
				labelVisitor.VisitList(((MultiLineLambdaExpressionSyntax)_syntaxRoot).Statements);
				break;
			default:
				labelVisitor.Visit(_syntaxRoot);
				break;
			}
			if (instance.Count > 0)
			{
				return instance.ToImmutableAndFree();
			}
			instance.Free();
			return ImmutableArray<SourceLabelSymbol>.Empty;
		}

		private static MultiDictionary<string, SourceLabelSymbol> BuildLabelsMap(ImmutableArray<SourceLabelSymbol> labels)
		{
			if (!labels.IsEmpty)
			{
				MultiDictionary<string, SourceLabelSymbol> multiDictionary = new MultiDictionary<string, SourceLabelSymbol>(labels.Length, CaseInsensitiveComparison.Comparer);
				ImmutableArray<SourceLabelSymbol>.Enumerator enumerator = labels.GetEnumerator();
				while (enumerator.MoveNext())
				{
					SourceLabelSymbol current = enumerator.Current;
					multiDictionary.Add(current.Name, current);
				}
				return multiDictionary;
			}
			return s_emptyLabelMap;
		}

		internal override LabelSymbol LookupLabelByNameToken(SyntaxToken labelName)
		{
			string valueText = labelName.ValueText;
			foreach (SourceLabelSymbol item in LabelsMap[valueText])
			{
				if (item.LabelName == labelName)
				{
					return item;
				}
			}
			return base.LookupLabelByNameToken(labelName);
		}

		internal override void LookupInSingleBinder(LookupResult lookupResult, string name, int arity, LookupOptions options, Binder originalBinder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if ((options & LookupOptions.LabelsOnly) != LookupOptions.LabelsOnly || LabelsMap == null)
			{
				return;
			}
			MultiDictionary<string, SourceLabelSymbol>.ValueSet valueSet = LabelsMap[name];
			switch (valueSet.Count)
			{
			case 1:
				lookupResult.SetFrom(SingleLookupResult.Good(valueSet.Single()));
				return;
			case 0:
				return;
			}
			SourceLabelSymbol sourceLabelSymbol = null;
			Location first = null;
			foreach (SourceLabelSymbol item in valueSet)
			{
				Location location = item.Locations[0];
				if ((object)sourceLabelSymbol == null || base.Compilation.CompareSourceLocations(first, location) > 0)
				{
					sourceLabelSymbol = item;
					first = location;
				}
			}
			lookupResult.SetFrom(SingleLookupResult.Good(sourceLabelSymbol));
		}

		internal override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
		{
			if (!Labels.IsEmpty && (options & LookupOptions.LabelsOnly) == LookupOptions.LabelsOnly)
			{
				ImmutableArray<SourceLabelSymbol>.Enumerator enumerator = Labels.GetEnumerator();
				while (enumerator.MoveNext())
				{
					SourceLabelSymbol current = enumerator.Current;
					nameSet.AddSymbol(current, current.Name, 0);
				}
			}
		}

		public override Binder GetBinder(SyntaxList<StatementSyntax> stmtList)
		{
			return _descendantBinderFactory.GetBinder(stmtList);
		}

		public override Binder GetBinder(SyntaxNode node)
		{
			return _descendantBinderFactory.GetBinder(node);
		}
	}
}
