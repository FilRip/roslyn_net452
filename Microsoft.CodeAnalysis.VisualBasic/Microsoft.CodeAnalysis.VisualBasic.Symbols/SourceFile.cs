using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class SourceFile : IImportScope
	{
		private sealed class BoundFileInformation
		{
			public readonly ImmutableArray<NamespaceOrTypeAndImportsClausePosition> MemberImports;

			public readonly ImmutableArray<SyntaxReference> MemberImportsSyntax;

			public readonly IReadOnlyDictionary<string, AliasAndImportsClausePosition> AliasImportsOpt;

			public readonly IReadOnlyDictionary<string, XmlNamespaceAndImportsClausePosition> XmlNamespacesOpt;

			public readonly bool? OptionStrict;

			public readonly bool? OptionInfer;

			public readonly bool? OptionExplicit;

			public readonly bool? OptionCompareText;

			public BoundFileInformation(ImmutableArray<NamespaceOrTypeAndImportsClausePosition> memberImports, ImmutableArray<SyntaxReference> memberImportsSyntax, IReadOnlyDictionary<string, AliasAndImportsClausePosition> importAliasesOpt, IReadOnlyDictionary<string, XmlNamespaceAndImportsClausePosition> xmlNamespacesOpt, bool? optionStrict, bool? optionInfer, bool? optionExplicit, bool? optionCompareText)
			{
				MemberImports = memberImports;
				MemberImportsSyntax = memberImportsSyntax;
				AliasImportsOpt = importAliasesOpt;
				XmlNamespacesOpt = xmlNamespacesOpt;
				OptionStrict = optionStrict;
				OptionInfer = optionInfer;
				OptionExplicit = optionExplicit;
				OptionCompareText = optionCompareText;
			}
		}

		private sealed class FileImportData : ImportData
		{
			private readonly ArrayBuilder<NamespaceOrTypeAndImportsClausePosition> _membersBuilder;

			private readonly ArrayBuilder<SyntaxReference> _membersSyntaxBuilder;

			public FileImportData(ArrayBuilder<NamespaceOrTypeAndImportsClausePosition> membersBuilder, ArrayBuilder<SyntaxReference> membersSyntaxBuilder)
				: base(new HashSet<NamespaceOrTypeSymbol>(), new Dictionary<string, AliasAndImportsClausePosition>(CaseInsensitiveComparison.Comparer), new Dictionary<string, XmlNamespaceAndImportsClausePosition>())
			{
				_membersBuilder = membersBuilder;
				_membersSyntaxBuilder = membersSyntaxBuilder;
			}

			public override void AddMember(SyntaxReference syntaxRef, NamespaceOrTypeSymbol member, int importsClausePosition, IReadOnlyCollection<AssemblySymbol> dependencies)
			{
				NamespaceOrTypeAndImportsClausePosition item = new NamespaceOrTypeAndImportsClausePosition(member, importsClausePosition, dependencies.ToImmutableArray());
				Members.Add(member);
				_membersBuilder.Add(item);
				_membersSyntaxBuilder.Add(syntaxRef);
			}

			public override void AddAlias(SyntaxReference syntaxRef, string name, AliasSymbol alias, int importsClausePosition, IReadOnlyCollection<AssemblySymbol> dependencies)
			{
				Aliases.Add(name, new AliasAndImportsClausePosition(alias, importsClausePosition, dependencies.ToImmutableArray()));
			}
		}

		private readonly SourceModuleSymbol _sourceModule;

		private readonly SyntaxTree _syntaxTree;

		private readonly DiagnosticBag _diagnosticBagDeclare;

		private BoundFileInformation _lazyBoundInformation;

		private int _importsValidated;

		private QuickAttributeChecker _lazyQuickAttributeChecker;

		private ImmutableArray<UsedNamespaceOrType> _lazyTranslatedImports;

		public DiagnosticBag DeclarationDiagnostics => _diagnosticBagDeclare;

		public QuickAttributeChecker QuickAttributeChecker
		{
			get
			{
				if (_lazyQuickAttributeChecker == null)
				{
					Interlocked.CompareExchange(ref _lazyQuickAttributeChecker, CreateQuickAttributeChecker(), null);
				}
				return _lazyQuickAttributeChecker;
			}
		}

		private BoundFileInformation BoundInformation => GetBoundInformation(CancellationToken.None);

		public ImmutableArray<NamespaceOrTypeAndImportsClausePosition> MemberImports => BoundInformation.MemberImports;

		public IReadOnlyDictionary<string, AliasAndImportsClausePosition> AliasImportsOpt => BoundInformation.AliasImportsOpt;

		public IReadOnlyDictionary<string, XmlNamespaceAndImportsClausePosition> XmlNamespacesOpt => BoundInformation.XmlNamespacesOpt;

		public bool? OptionStrict => BoundInformation.OptionStrict;

		public bool? OptionInfer => BoundInformation.OptionInfer;

		public bool? OptionExplicit => BoundInformation.OptionExplicit;

		public bool? OptionCompareText => BoundInformation.OptionCompareText;

		public IImportScope Parent => null;

		public SourceFile(SourceModuleSymbol sourceModule, SyntaxTree tree)
		{
			_diagnosticBagDeclare = new DiagnosticBag();
			_sourceModule = sourceModule;
			_syntaxTree = tree;
		}

		private QuickAttributeChecker CreateQuickAttributeChecker()
		{
			QuickAttributeChecker quickAttributeChecker = new QuickAttributeChecker(_sourceModule.QuickAttributeChecker);
			SyntaxList<ImportsStatementSyntax>.Enumerator enumerator = VisualBasicExtensions.GetCompilationUnitRoot(_syntaxTree).Imports.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SeparatedSyntaxList<ImportsClauseSyntax>.Enumerator enumerator2 = enumerator.Current.ImportsClauses.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					ImportsClauseSyntax current = enumerator2.Current;
					if (current.Kind() == SyntaxKind.SimpleImportsClause)
					{
						SimpleImportsClauseSyntax simpleImportsClauseSyntax = (SimpleImportsClauseSyntax)current;
						if (simpleImportsClauseSyntax.Alias != null)
						{
							quickAttributeChecker.AddAlias(simpleImportsClauseSyntax);
						}
					}
				}
			}
			quickAttributeChecker.Seal();
			return quickAttributeChecker;
		}

		private BoundFileInformation GetBoundInformation(CancellationToken cancellationToken)
		{
			if (_lazyBoundInformation == null)
			{
				BindingDiagnosticBag bindingDiagnosticBag = new BindingDiagnosticBag(new DiagnosticBag());
				BoundFileInformation value = BindFileInformation(bindingDiagnosticBag.DiagnosticBag, cancellationToken);
				_sourceModule.AtomicStoreReferenceAndDiagnostics(ref _lazyBoundInformation, value, bindingDiagnosticBag);
			}
			return _lazyBoundInformation;
		}

		private void EnsureImportsValidated()
		{
			if (_importsValidated == 0)
			{
				BoundFileInformation boundInformation = BoundInformation;
				BindingDiagnosticBag bindingDiagnosticBag = new BindingDiagnosticBag();
				ValidateImports(_sourceModule.DeclaringCompilation, boundInformation.MemberImports, boundInformation.MemberImportsSyntax, boundInformation.AliasImportsOpt, bindingDiagnosticBag);
				_sourceModule.AtomicStoreIntegerAndDiagnostics(ref _importsValidated, 1, 0, bindingDiagnosticBag);
			}
		}

		private BoundFileInformation BindFileInformation(DiagnosticBag diagBag, CancellationToken cancellationToken, TextSpan? filterSpan = null)
		{
			Binder binder = BinderBuilder.CreateBinderForSourceFileImports(_sourceModule, _syntaxTree);
			CompilationUnitSyntax compilationUnitRoot = VisualBasicExtensions.GetCompilationUnitRoot(_syntaxTree);
			bool? optionStrict = default(bool?);
			bool? optionInfer = default(bool?);
			bool? optionExplicit = default(bool?);
			bool? optionCompareText = default(bool?);
			BindOptions(compilationUnitRoot.Options, diagBag, ref optionStrict, ref optionInfer, ref optionExplicit, ref optionCompareText, filterSpan);
			ImmutableArray<NamespaceOrTypeAndImportsClausePosition> importMembersOf = default(ImmutableArray<NamespaceOrTypeAndImportsClausePosition>);
			ImmutableArray<SyntaxReference> importMembersOfSyntax = default(ImmutableArray<SyntaxReference>);
			IReadOnlyDictionary<string, AliasAndImportsClausePosition> importAliasesOpt = null;
			IReadOnlyDictionary<string, XmlNamespaceAndImportsClausePosition> xmlNamespacesOpt = null;
			BindImports(compilationUnitRoot.Imports, binder, diagBag, out importMembersOf, out importMembersOfSyntax, out importAliasesOpt, out xmlNamespacesOpt, cancellationToken, filterSpan);
			return new BoundFileInformation(importMembersOf, importMembersOfSyntax, importAliasesOpt, xmlNamespacesOpt, optionStrict, optionInfer, optionExplicit, optionCompareText);
		}

		private static void BindOptions(SyntaxList<OptionStatementSyntax> optionsSyntax, DiagnosticBag diagBag, ref bool? optionStrict, ref bool? optionInfer, ref bool? optionExplicit, ref bool? optionCompareText, TextSpan? filterSpan = null)
		{
			optionStrict = null;
			optionInfer = null;
			optionExplicit = null;
			optionCompareText = null;
			SyntaxList<OptionStatementSyntax>.Enumerator enumerator = optionsSyntax.GetEnumerator();
			while (enumerator.MoveNext())
			{
				OptionStatementSyntax current = enumerator.Current;
				if (filterSpan.HasValue && !filterSpan.Value.IntersectsWith(current.FullSpan))
				{
					continue;
				}
				switch (VisualBasicExtensions.Kind(current.NameKeyword))
				{
				case SyntaxKind.StrictKeyword:
					if (optionStrict.HasValue)
					{
						Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_DuplicateOption1, "Strict");
					}
					else
					{
						optionStrict = Binder.DecodeOnOff(current.ValueKeyword);
					}
					break;
				case SyntaxKind.InferKeyword:
					if (optionInfer.HasValue)
					{
						Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_DuplicateOption1, "Infer");
					}
					else
					{
						optionInfer = Binder.DecodeOnOff(current.ValueKeyword);
					}
					break;
				case SyntaxKind.ExplicitKeyword:
					if (optionExplicit.HasValue)
					{
						Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_DuplicateOption1, "Explicit");
					}
					else
					{
						optionExplicit = Binder.DecodeOnOff(current.ValueKeyword);
					}
					break;
				case SyntaxKind.CompareKeyword:
					if (optionCompareText.HasValue)
					{
						Binder.ReportDiagnostic(diagBag, current, ERRID.ERR_DuplicateOption1, "Compare");
					}
					else
					{
						optionCompareText = Binder.DecodeTextBinary(current.ValueKeyword);
					}
					break;
				}
			}
		}

		private static void BindImports(SyntaxList<ImportsStatementSyntax> importsListSyntax, Binder binder, DiagnosticBag diagBag, out ImmutableArray<NamespaceOrTypeAndImportsClausePosition> importMembersOf, out ImmutableArray<SyntaxReference> importMembersOfSyntax, out IReadOnlyDictionary<string, AliasAndImportsClausePosition> importAliasesOpt, out IReadOnlyDictionary<string, XmlNamespaceAndImportsClausePosition> xmlNamespacesOpt, CancellationToken cancellationToken, TextSpan? filterSpan = null)
		{
			ArrayBuilder<NamespaceOrTypeAndImportsClausePosition> instance = ArrayBuilder<NamespaceOrTypeAndImportsClausePosition>.GetInstance();
			ArrayBuilder<SyntaxReference> instance2 = ArrayBuilder<SyntaxReference>.GetInstance();
			FileImportData fileImportData = new FileImportData(instance, instance2);
			try
			{
				SyntaxList<ImportsStatementSyntax>.Enumerator enumerator = importsListSyntax.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ImportsStatementSyntax current = enumerator.Current;
					if (filterSpan.HasValue && !filterSpan.Value.IntersectsWith(current.FullSpan))
					{
						continue;
					}
					cancellationToken.ThrowIfCancellationRequested();
					binder.Compilation.RecordImports(current);
					SeparatedSyntaxList<ImportsClauseSyntax>.Enumerator enumerator2 = current.ImportsClauses.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						ImportsClauseSyntax current2 = enumerator2.Current;
						if (!filterSpan.HasValue || filterSpan.Value.IntersectsWith(current.FullSpan))
						{
							cancellationToken.ThrowIfCancellationRequested();
							binder.BindImportClause(current2, fileImportData, diagBag);
						}
					}
				}
				importMembersOf = instance.ToImmutable();
				importMembersOfSyntax = instance2.ToImmutable();
				importAliasesOpt = ((fileImportData.Aliases.Count == 0) ? null : fileImportData.Aliases);
				xmlNamespacesOpt = ((fileImportData.XmlNamespaces.Count > 0) ? fileImportData.XmlNamespaces : null);
			}
			finally
			{
				instance.Free();
				instance2.Free();
			}
		}

		private static void ValidateImports(VisualBasicCompilation compilation, ImmutableArray<NamespaceOrTypeAndImportsClausePosition> memberImports, ImmutableArray<SyntaxReference> memberImportsSyntax, IReadOnlyDictionary<string, AliasAndImportsClausePosition> aliasImportsOpt, BindingDiagnosticBag diagnostics)
		{
			BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
			int num = memberImports.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				ValidateImportsClause(compilation, instance, memberImports[i].NamespaceOrType, memberImports[i].Dependencies, memberImportsSyntax[i].GetLocation(), memberImports[i].ImportsClausePosition, diagnostics);
			}
			if (aliasImportsOpt != null)
			{
				foreach (AliasAndImportsClausePosition value in aliasImportsOpt.Values)
				{
					ValidateImportsClause(compilation, instance, value.Alias.Target, value.Dependencies, value.Alias.Locations[0], value.ImportsClausePosition, diagnostics);
				}
			}
			instance.Free();
		}

		private static void ValidateImportsClause(VisualBasicCompilation compilation, BindingDiagnosticBag clauseDiagnostics, NamespaceOrTypeSymbol namespaceOrType, ImmutableArray<AssemblySymbol> dependencies, Location location, int importsClausePosition, BindingDiagnosticBag diagnostics)
		{
			if (namespaceOrType is TypeSymbol type)
			{
				clauseDiagnostics.Clear();
				ConstraintsHelper.CheckAllConstraints(type, location, clauseDiagnostics, new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, compilation.Assembly));
				diagnostics.AddRange(clauseDiagnostics.DiagnosticBag);
				if (Compilation.ReportUnusedImportsInTree(LocationExtensions.PossiblyEmbeddedOrMySourceTree(location)))
				{
					if (clauseDiagnostics.DependenciesBag!.Count != 0)
					{
						if (!dependencies.IsEmpty)
						{
							clauseDiagnostics.AddDependencies(dependencies);
						}
						dependencies = clauseDiagnostics.DependenciesBag.ToImmutableArray();
					}
					compilation.RecordImportsClauseDependencies(LocationExtensions.PossiblyEmbeddedOrMySourceTree(location), importsClausePosition, dependencies);
				}
				else
				{
					diagnostics.AddDependencies(dependencies);
					diagnostics.AddDependencies(clauseDiagnostics.DependenciesBag);
				}
			}
			else if (!Compilation.ReportUnusedImportsInTree(LocationExtensions.PossiblyEmbeddedOrMySourceTree(location)))
			{
				diagnostics.AddAssembliesUsedByNamespaceReference((NamespaceSymbol)namespaceOrType);
			}
		}

		internal void GenerateAllDeclarationErrors()
		{
			_ = BoundInformation;
			EnsureImportsValidated();
		}

		internal IEnumerable<Diagnostic> GetDeclarationErrorsInSpan(TextSpan filterSpan, CancellationToken cancellationToken)
		{
			DiagnosticBag instance = DiagnosticBag.GetInstance();
			BindFileInformation(instance, cancellationToken, filterSpan);
			return instance.ToReadOnlyAndFree();
		}

		public IImportScope Translate(PEModuleBuilder moduleBuilder, DiagnosticBag diagnostics)
		{
			if (_lazyTranslatedImports.IsDefault)
			{
				ImmutableInterlocked.InterlockedInitialize(ref _lazyTranslatedImports, TranslateImports(moduleBuilder, diagnostics));
			}
			return this;
		}

		public ImmutableArray<UsedNamespaceOrType> GetUsedNamespaces()
		{
			return _lazyTranslatedImports;
		}

		ImmutableArray<UsedNamespaceOrType> IImportScope.GetUsedNamespaces()
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetUsedNamespaces
			return this.GetUsedNamespaces();
		}

		private ImmutableArray<UsedNamespaceOrType> TranslateImports(PEModuleBuilder moduleBuilder, DiagnosticBag diagnostics)
		{
			return NamespaceScopeBuilder.BuildNamespaceScope(moduleBuilder, XmlNamespacesOpt, (AliasImportsOpt != null) ? AliasImportsOpt.Values : null, MemberImports, diagnostics);
		}
	}
}
