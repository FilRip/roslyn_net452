using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	public abstract class VisualBasicSyntaxTree : SyntaxTree
	{
		private class ConditionalSymbolsMap
		{
			private class ConditionalSymbolsMapBuilder
			{
				private Dictionary<string, Stack<Tuple<CConst, int>>> _conditionalsMap;

				private Scanner.PreprocessorState _preprocessorState;

				internal ImmutableDictionary<string, Stack<Tuple<CConst, int>>> Build(SyntaxNodeOrToken root, VisualBasicParseOptions options)
				{
					_conditionalsMap = new Dictionary<string, Stack<Tuple<CConst, int>>>(CaseInsensitiveComparison.Comparer);
					ImmutableDictionary<string, CConst> preprocessorConstants = Scanner.GetPreprocessorConstants(options);
					ProcessCommandLinePreprocessorSymbols(preprocessorConstants);
					_preprocessorState = new Scanner.PreprocessorState(preprocessorConstants);
					IEnumerable<Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax> directives = root.GetDirectives<Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax>();
					ProcessSourceDirectives(directives);
					if (!_conditionalsMap.Any())
					{
						return null;
					}
					return ImmutableDictionary.CreateRange(CaseInsensitiveComparison.Comparer, _conditionalsMap);
				}

				private void ProcessCommandLinePreprocessorSymbols(ImmutableDictionary<string, CConst> preprocessorSymbolsMap)
				{
					foreach (KeyValuePair<string, CConst> item in preprocessorSymbolsMap)
					{
						ProcessConditionalSymbolDefinition(item.Key, item.Value, 0);
					}
				}

				private void ProcessConditionalSymbolDefinition(string name, CConst value, int position)
				{
					Stack<Tuple<CConst, int>> value2 = null;
					if (!_conditionalsMap.TryGetValue(name, out value2))
					{
						value2 = new Stack<Tuple<CConst, int>>();
						_conditionalsMap.Add(name, value2);
					}
					value2.Push(Tuple.Create(value, position));
				}

				private void ProcessSourceDirectives(IEnumerable<Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax> directives)
				{
					foreach (Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax directive in directives)
					{
						ProcessDirective(directive);
					}
				}

				private void ProcessDirective(Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax directive)
				{
					SyntaxKind syntaxKind = directive.Kind();
					if (syntaxKind == SyntaxKind.ConstDirectiveTrivia)
					{
						ImmutableDictionary<string, CConst> symbolsMap = _preprocessorState.SymbolsMap;
						Scanner.PreprocessorState preprocessorState = _preprocessorState;
						Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax statement = (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)directive.Green;
						_preprocessorState = Scanner.ApplyDirective(preprocessorState, ref statement);
						ImmutableDictionary<string, CConst> symbolsMap2 = _preprocessorState.SymbolsMap;
						if (symbolsMap != symbolsMap2)
						{
							string valueText = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.ConstDirectiveTriviaSyntax)directive).Name.ValueText;
							ProcessConditionalSymbolDefinition(valueText, symbolsMap2[valueText], directive.SpanStart);
						}
					}
					else
					{
						Scanner.PreprocessorState preprocessorState2 = _preprocessorState;
						Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax statement = (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.DirectiveTriviaSyntax)directive.Green;
						_preprocessorState = Scanner.ApplyDirective(preprocessorState2, ref statement);
					}
				}
			}

			private readonly ImmutableDictionary<string, Stack<Tuple<CConst, int>>> _conditionalsMap;

			internal static readonly ConditionalSymbolsMap Uninitialized = new ConditionalSymbolsMap();

			private ConditionalSymbolsMap()
			{
			}

			private ConditionalSymbolsMap(ImmutableDictionary<string, Stack<Tuple<CConst, int>>> conditionalsMap)
			{
				_conditionalsMap = conditionalsMap;
			}

			internal static ConditionalSymbolsMap Create(VisualBasicSyntaxNode syntaxRoot, VisualBasicParseOptions options)
			{
				ImmutableDictionary<string, Stack<Tuple<CConst, int>>> immutableDictionary = new ConditionalSymbolsMapBuilder().Build(syntaxRoot, options);
				if (immutableDictionary == null)
				{
					return null;
				}
				return new ConditionalSymbolsMap(immutableDictionary);
			}

			internal VisualBasicPreprocessingSymbolInfo GetPreprocessingSymbolInfo(string conditionalSymbolName, Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax node)
			{
				CConst preprocessorSymbolValue = GetPreprocessorSymbolValue(conditionalSymbolName, node);
				if (preprocessorSymbolValue == null)
				{
					return VisualBasicPreprocessingSymbolInfo.None;
				}
				string name = _conditionalsMap.Keys.First((string key) => CaseInsensitiveComparison.Equals(key, conditionalSymbolName));
				return new VisualBasicPreprocessingSymbolInfo(new PreprocessingSymbol(name), RuntimeHelpers.GetObjectValue(preprocessorSymbolValue.ValueAsObject), isDefined: true);
			}

			private CConst GetPreprocessorSymbolValue(string conditionalSymbolName, SyntaxNodeOrToken node)
			{
				Stack<Tuple<CConst, int>> value = null;
				if (_conditionalsMap.TryGetValue(conditionalSymbolName, out value))
				{
					int spanStart = node.SpanStart;
					foreach (Tuple<CConst, int> item in value)
					{
						if (item.Item2 < spanStart)
						{
							return item.Item1;
						}
					}
				}
				return null;
			}

			internal bool IsConditionalSymbolDefined(string conditionalSymbolName, SyntaxNodeOrToken node)
			{
				if (conditionalSymbolName != null)
				{
					CConst preprocessorSymbolValue = GetPreprocessorSymbolValue(conditionalSymbolName, node);
					if (preprocessorSymbolValue != null && !preprocessorSymbolValue.IsBad)
					{
						switch (preprocessorSymbolValue.SpecialType)
						{
						case SpecialType.System_Boolean:
							return ((CConst<bool>)preprocessorSymbolValue).Value;
						case SpecialType.System_Byte:
							return ((CConst<byte>)preprocessorSymbolValue).Value != 0;
						case SpecialType.System_Int16:
							return ((CConst<short>)preprocessorSymbolValue).Value != 0;
						case SpecialType.System_Int32:
							return ((CConst<int>)preprocessorSymbolValue).Value != 0;
						case SpecialType.System_Int64:
							return ((CConst<long>)preprocessorSymbolValue).Value != 0;
						case SpecialType.System_SByte:
							return ((CConst<sbyte>)preprocessorSymbolValue).Value != 0;
						case SpecialType.System_UInt16:
							return ((CConst<ushort>)preprocessorSymbolValue).Value != 0;
						case SpecialType.System_UInt32:
							return (ulong)((CConst<uint>)preprocessorSymbolValue).Value != 0;
						case SpecialType.System_UInt64:
							return decimal.Compare(new decimal(((CConst<ulong>)preprocessorSymbolValue).Value), 0m) != 0;
						case SpecialType.System_String:
							return true;
						}
					}
				}
				return false;
			}
		}

		internal class DummySyntaxTree : VisualBasicSyntaxTree
		{
			private readonly Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax _node;

			public override Encoding Encoding => Encoding.UTF8;

			public override int Length => 0;

			public override VisualBasicParseOptions Options => VisualBasicParseOptions.Default;

			public override ImmutableDictionary<string, ReportDiagnostic> DiagnosticOptions
			{
				get
				{
					throw ExceptionUtilities.Unreachable;
				}
			}

			public override string FilePath => string.Empty;

			public override bool HasCompilationUnitRoot => true;

			public DummySyntaxTree()
			{
				_node = CloneNodeAsRoot(SyntaxFactory.ParseCompilationUnit(string.Empty));
			}

			public override string ToString()
			{
				return string.Empty;
			}

			public override SourceText GetText(CancellationToken cancellationToken = default(CancellationToken))
			{
				return SourceText.From(string.Empty, Encoding.UTF8);
			}

			public override bool TryGetText(ref SourceText text)
			{
				text = SourceText.From(string.Empty, Encoding.UTF8);
				return true;
			}

			public override SyntaxReference GetReference(SyntaxNode node)
			{
				return new SimpleSyntaxReference(this, node);
			}

			public override SyntaxTree WithChangedText(SourceText newText)
			{
				throw new InvalidOperationException();
			}

			public override VisualBasicSyntaxNode GetRoot(CancellationToken cancellationToken = default(CancellationToken))
			{
				return _node;
			}

			public override bool TryGetRoot(ref VisualBasicSyntaxNode root)
			{
				root = _node;
				return true;
			}

			public override SyntaxTree WithRootAndOptions(SyntaxNode root, ParseOptions options)
			{
				return SyntaxFactory.SyntaxTree(root, options, FilePath);
			}

			public override SyntaxTree WithFilePath(string path)
			{
				return SyntaxFactory.SyntaxTree(_node, Options, path);
			}

			public override SyntaxTree WithDiagnosticOptions(ImmutableDictionary<string, ReportDiagnostic> options)
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		private sealed class LazySyntaxTree : VisualBasicSyntaxTree
		{
			private readonly SourceText _text;

			private readonly VisualBasicParseOptions _options;

			private readonly string _path;

			private readonly ImmutableDictionary<string, ReportDiagnostic> _diagnosticOptions;

			private VisualBasicSyntaxNode _lazyRoot;

			public override string FilePath => _path;

			internal override bool IsMyTemplate => false;

			public override Encoding Encoding => _text.Encoding;

			public override int Length => _text.Length;

			public override bool HasCompilationUnitRoot => true;

			public override VisualBasicParseOptions Options => _options;

			public override ImmutableDictionary<string, ReportDiagnostic> DiagnosticOptions => _diagnosticOptions;

			internal LazySyntaxTree(SourceText text, VisualBasicParseOptions options, string path, ImmutableDictionary<string, ReportDiagnostic> diagnosticOptions)
			{
				_text = text;
				_options = options;
				_path = path ?? string.Empty;
				_diagnosticOptions = diagnosticOptions ?? SyntaxTree.EmptyDiagnosticOptions;
			}

			public override SourceText GetText(CancellationToken cancellationToken = default(CancellationToken))
			{
				return _text;
			}

			public override bool TryGetText(ref SourceText text)
			{
				text = _text;
				return true;
			}

			public override VisualBasicSyntaxNode GetRoot(CancellationToken cancellationToken = default(CancellationToken))
			{
				if (_lazyRoot == null)
				{
					SyntaxTree syntaxTree = SyntaxFactory.ParseSyntaxTree(_text, _options, _path, cancellationToken);
					VisualBasicSyntaxNode value = CloneNodeAsRoot((VisualBasicSyntaxNode)syntaxTree.GetRoot(cancellationToken));
					Interlocked.CompareExchange(ref _lazyRoot, value, null);
				}
				return _lazyRoot;
			}

			public override bool TryGetRoot(ref VisualBasicSyntaxNode root)
			{
				root = _lazyRoot;
				return root != null;
			}

			public override SyntaxReference GetReference(SyntaxNode node)
			{
				return new SimpleSyntaxReference(this, node);
			}

			public override SyntaxTree WithRootAndOptions(SyntaxNode root, ParseOptions options)
			{
				if (_lazyRoot == root && (object)_options == options)
				{
					return this;
				}
				return new ParsedSyntaxTree(null, _text.Encoding, _text.ChecksumAlgorithm, _path, (VisualBasicParseOptions)options, (VisualBasicSyntaxNode)root, isMyTemplate: false, _diagnosticOptions);
			}

			public override SyntaxTree WithFilePath(string path)
			{
				if (string.Equals(_path, path))
				{
					return this;
				}
				VisualBasicSyntaxNode root = null;
				if (TryGetRoot(ref root))
				{
					return new ParsedSyntaxTree(_text, _text.Encoding, _text.ChecksumAlgorithm, path, _options, root, isMyTemplate: false, _diagnosticOptions);
				}
				return new LazySyntaxTree(_text, _options, path, _diagnosticOptions);
			}

			public override SyntaxTree WithDiagnosticOptions(ImmutableDictionary<string, ReportDiagnostic> options)
			{
				if (options == null)
				{
					options = SyntaxTree.EmptyDiagnosticOptions;
				}
				if (object.ReferenceEquals(_diagnosticOptions, options))
				{
					return this;
				}
				VisualBasicSyntaxNode root = null;
				if (TryGetRoot(ref root))
				{
					return new ParsedSyntaxTree(_text, _text.Encoding, _text.ChecksumAlgorithm, _path, _options, root, isMyTemplate: false, options);
				}
				return new LazySyntaxTree(_text, _options, _path, options);
			}
		}

		private class ParsedSyntaxTree : VisualBasicSyntaxTree
		{
			private readonly VisualBasicParseOptions _options;

			private readonly string _path;

			private readonly VisualBasicSyntaxNode _root;

			private readonly bool _hasCompilationUnitRoot;

			private readonly bool _isMyTemplate;

			private readonly Encoding _encodingOpt;

			private readonly SourceHashAlgorithm _checksumAlgorithm;

			private readonly ImmutableDictionary<string, ReportDiagnostic> _diagnosticOptions;

			private SourceText _lazyText;

			public override string FilePath => _path;

			internal override bool IsMyTemplate => _isMyTemplate;

			public override Encoding Encoding => _encodingOpt;

			public override int Length => _root.FullSpan.Length;

			public override bool HasCompilationUnitRoot => _hasCompilationUnitRoot;

			public override VisualBasicParseOptions Options => _options;

			public override ImmutableDictionary<string, ReportDiagnostic> DiagnosticOptions => _diagnosticOptions;

			internal ParsedSyntaxTree(SourceText textOpt, Encoding encodingOpt, SourceHashAlgorithm checksumAlgorithm, string path, VisualBasicParseOptions options, VisualBasicSyntaxNode syntaxRoot, bool isMyTemplate, ImmutableDictionary<string, ReportDiagnostic> diagnosticOptions, bool cloneRoot = true)
			{
				_lazyText = textOpt;
				_encodingOpt = encodingOpt ?? textOpt?.Encoding;
				_checksumAlgorithm = checksumAlgorithm;
				_options = options;
				_path = path ?? string.Empty;
				_root = (cloneRoot ? CloneNodeAsRoot(syntaxRoot) : syntaxRoot);
				_hasCompilationUnitRoot = syntaxRoot.Kind() == SyntaxKind.CompilationUnit;
				_isMyTemplate = isMyTemplate;
				_diagnosticOptions = diagnosticOptions ?? SyntaxTree.EmptyDiagnosticOptions;
			}

			public override SourceText GetText(CancellationToken cancellationToken = default(CancellationToken))
			{
				if (_lazyText == null)
				{
					SourceText text = GetRoot(cancellationToken).GetText(_encodingOpt, _checksumAlgorithm);
					Interlocked.CompareExchange(ref _lazyText, text, null);
				}
				return _lazyText;
			}

			public override bool TryGetText(ref SourceText text)
			{
				text = _lazyText;
				return text != null;
			}

			public override VisualBasicSyntaxNode GetRoot(CancellationToken cancellationToken = default(CancellationToken))
			{
				return _root;
			}

			public override Task<VisualBasicSyntaxNode> GetRootAsync(CancellationToken cancellationToken = default(CancellationToken))
			{
				return Task.FromResult(_root);
			}

			public override bool TryGetRoot(ref VisualBasicSyntaxNode root)
			{
				root = _root;
				return true;
			}

			public override SyntaxReference GetReference(SyntaxNode node)
			{
				return new SimpleSyntaxReference(this, node);
			}

			public override SyntaxTree WithRootAndOptions(SyntaxNode root, ParseOptions options)
			{
				if (_root == root && (object)_options == options)
				{
					return this;
				}
				return new ParsedSyntaxTree(null, _encodingOpt, _checksumAlgorithm, _path, (VisualBasicParseOptions)options, (VisualBasicSyntaxNode)root, _isMyTemplate, _diagnosticOptions);
			}

			public override SyntaxTree WithFilePath(string path)
			{
				if (string.Equals(_path, path))
				{
					return this;
				}
				return new ParsedSyntaxTree(_lazyText, _encodingOpt, _checksumAlgorithm, path, _options, _root, _isMyTemplate, _diagnosticOptions);
			}

			public override SyntaxTree WithDiagnosticOptions(ImmutableDictionary<string, ReportDiagnostic> options)
			{
				if (options == null)
				{
					options = SyntaxTree.EmptyDiagnosticOptions;
				}
				if (object.ReferenceEquals(_diagnosticOptions, options))
				{
					return this;
				}
				return new ParsedSyntaxTree(_lazyText, _encodingOpt, _checksumAlgorithm, _path, _options, _root, _isMyTemplate, options);
			}
		}

		private VisualBasicLineDirectiveMap _lineDirectiveMap;

		internal static readonly VisualBasicSyntaxTree Dummy = new DummySyntaxTree();

		internal static readonly SyntaxReference DummyReference = Dummy.GetReference(Dummy.GetRoot());

		private VisualBasicWarningStateMap _lazyWarningStateMap;

		private ConditionalSymbolsMap _lazySymbolsMap;

		public new abstract VisualBasicParseOptions Options { get; }

		internal virtual bool IsMyTemplate => false;

		internal bool HasReferenceDirectives
		{
			get
			{
				if (Options.Kind == SourceCodeKind.Script)
				{
					return GetCompilationUnitRoot().GetReferenceDirectives().Count > 0;
				}
				return false;
			}
		}

		protected override ParseOptions OptionsCore => Options;

		private ConditionalSymbolsMap ConditionalSymbols
		{
			get
			{
				if (_lazySymbolsMap == ConditionalSymbolsMap.Uninitialized)
				{
					Interlocked.CompareExchange(ref _lazySymbolsMap, ConditionalSymbolsMap.Create(GetRoot(CancellationToken.None), Options), ConditionalSymbolsMap.Uninitialized);
				}
				return _lazySymbolsMap;
			}
		}

		protected VisualBasicSyntaxTree()
		{
			_lazySymbolsMap = ConditionalSymbolsMap.Uninitialized;
		}

		protected T CloneNodeAsRoot<T>(T node) where T : VisualBasicSyntaxNode
		{
			return SyntaxNode.CloneNodeAsRoot(node, this);
		}

		public new abstract VisualBasicSyntaxNode GetRoot(CancellationToken cancellationToken = default(CancellationToken));

		public new virtual Task<VisualBasicSyntaxNode> GetRootAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			VisualBasicSyntaxNode root = null;
			return Task.FromResult(TryGetRoot(ref root) ? root : GetRoot(cancellationToken));
		}

		public abstract bool TryGetRoot(ref VisualBasicSyntaxNode root);

		public Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax GetCompilationUnitRoot(CancellationToken cancellationToken = default(CancellationToken))
		{
			return (Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax)GetRoot(cancellationToken);
		}

		public override SyntaxTree WithChangedText(SourceText newText)
		{
			SourceText text = null;
			if (TryGetText(out text))
			{
				return WithChanges(newText, newText.GetChangeRanges(text).ToArray());
			}
			return WithChanges(newText, new TextChangeRange[1]
			{
				new TextChangeRange(new TextSpan(0, Length), newText.Length)
			});
		}

		private SyntaxTree WithChanges(SourceText newText, TextChangeRange[] changes)
		{
			if (changes == null)
			{
				throw new ArgumentNullException("changes");
			}
			Scanner scanner = ((changes.Length != 1 || !(changes[0].Span == new TextSpan(0, Length)) || changes[0].NewLength != newText.Length) ? new Blender(newText, changes, this, Options) : new Scanner(newText, Options));
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax compilationUnitSyntax;
			using (scanner)
			{
				compilationUnitSyntax = new Parser(scanner).ParseCompilationUnit();
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax syntaxRoot = (Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax)compilationUnitSyntax.CreateRed(null, 0);
			return new ParsedSyntaxTree(newText, newText.Encoding, newText.ChecksumAlgorithm, FilePath, Options, syntaxRoot, isMyTemplate: false, DiagnosticOptions);
		}

		public static SyntaxTree Create(VisualBasicSyntaxNode root, VisualBasicParseOptions options = null, string path = "", Encoding encoding = null, ImmutableDictionary<string, ReportDiagnostic> diagnosticOptions = null)
		{
			if (root == null)
			{
				throw new ArgumentNullException("root");
			}
			return new ParsedSyntaxTree(null, encoding, SourceHashAlgorithm.Sha1, path, options ?? VisualBasicParseOptions.Default, root, isMyTemplate: false, diagnosticOptions);
		}

		internal static SyntaxTree CreateWithoutClone(VisualBasicSyntaxNode root)
		{
			return new ParsedSyntaxTree(null, null, SourceHashAlgorithm.Sha1, "", VisualBasicParseOptions.Default, root, isMyTemplate: false, null, cloneRoot: false);
		}

		internal static SyntaxTree ParseTextLazy(SourceText text, VisualBasicParseOptions options = null, string path = "")
		{
			return new LazySyntaxTree(text, options ?? VisualBasicParseOptions.Default, path, null);
		}

		public static SyntaxTree ParseText(string text, VisualBasicParseOptions options = null, string path = "", Encoding encoding = null, ImmutableDictionary<string, ReportDiagnostic> diagnosticOptions = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return ParseText(text, isMyTemplate: false, options, path, encoding, diagnosticOptions, cancellationToken);
		}

		internal static SyntaxTree ParseText(string text, bool isMyTemplate, VisualBasicParseOptions options = null, string path = "", Encoding encoding = null, ImmutableDictionary<string, ReportDiagnostic> diagnosticOptions = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return ParseText(SourceText.From(text, encoding), isMyTemplate, options, path, diagnosticOptions, cancellationToken);
		}

		public static SyntaxTree ParseText(SourceText text, VisualBasicParseOptions options = null, string path = "", ImmutableDictionary<string, ReportDiagnostic> diagnosticOptions = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			return ParseText(text, isMyTemplate: false, options, path, diagnosticOptions, cancellationToken);
		}

		internal static SyntaxTree ParseText(SourceText text, bool isMyTemplate, VisualBasicParseOptions parseOptions = null, string path = "", ImmutableDictionary<string, ReportDiagnostic> diagnosticOptions = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (text == null)
			{
				throw new ArgumentNullException("text");
			}
			parseOptions = parseOptions ?? VisualBasicParseOptions.Default;
			Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.CompilationUnitSyntax compilationUnitSyntax;
			using (Parser parser = new Parser(text, parseOptions, cancellationToken))
			{
				compilationUnitSyntax = parser.ParseCompilationUnit();
			}
			Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax syntaxRoot = (Microsoft.CodeAnalysis.VisualBasic.Syntax.CompilationUnitSyntax)compilationUnitSyntax.CreateRed(null, 0);
			return new ParsedSyntaxTree(text, text.Encoding, text.ChecksumAlgorithm, path, parseOptions, syntaxRoot, isMyTemplate, diagnosticOptions);
		}

		public override IEnumerable<Diagnostic> GetDiagnostics(SyntaxNode node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			return GetDiagnostics((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)node.Green, ((VisualBasicSyntaxNode)node).Position, InDocumentationComment(node));
		}

		public override IEnumerable<Diagnostic> GetDiagnostics(SyntaxToken token)
		{
			return GetDiagnostics((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken)token.Node, token.Position, InDocumentationComment(token));
		}

		public override IEnumerable<Diagnostic> GetDiagnostics(SyntaxTrivia trivia)
		{
			return GetDiagnostics((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)trivia.UnderlyingNode, trivia.Position, InDocumentationComment(trivia));
		}

		public override IEnumerable<Diagnostic> GetDiagnostics(SyntaxNodeOrToken nodeOrToken)
		{
			return GetDiagnostics((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)nodeOrToken.UnderlyingNode, nodeOrToken.Position, InDocumentationComment(nodeOrToken));
		}

		public override IEnumerable<Diagnostic> GetDiagnostics(CancellationToken cancellationToken = default(CancellationToken))
		{
			return GetDiagnostics(GetRoot(cancellationToken).VbGreen, 0, InDocumentationComment: false);
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_34_EnumerateDiagnostics))]
		internal IEnumerable<Diagnostic> EnumerateDiagnostics(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode node, int position, bool InDocumentationComment)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_34_EnumerateDiagnostics(-2)
			{
				_0024VB_0024Me = this,
				_0024P_node = node,
				_0024P_position = position,
				_0024P_InDocumentationComment = InDocumentationComment
			};
		}

		internal IEnumerable<Diagnostic> GetDiagnostics(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode node, int position, bool InDocumentationComment)
		{
			if (node == null)
			{
				throw new InvalidOperationException();
			}
			if (node.ContainsDiagnostics)
			{
				return EnumerateDiagnostics(node, position, InDocumentationComment);
			}
			return SpecializedCollections.EmptyEnumerable<Diagnostic>();
		}

		private bool InDocumentationComment(SyntaxNode node)
		{
			bool flag = false;
			while (node != null && SyntaxFacts.IsXmlSyntax(VisualBasicExtensions.Kind(node)))
			{
				flag = true;
				node = node.Parent;
			}
			if (flag && node != null)
			{
				return Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(node, SyntaxKind.DocumentationCommentTrivia);
			}
			return false;
		}

		private bool InDocumentationComment(SyntaxNodeOrToken node)
		{
			if (node.IsToken)
			{
				return InDocumentationComment(node.AsToken());
			}
			return InDocumentationComment(node.AsNode());
		}

		private bool InDocumentationComment(SyntaxToken token)
		{
			return InDocumentationComment(token.Parent);
		}

		private bool InDocumentationComment(SyntaxTrivia trivia)
		{
			return InDocumentationComment(trivia.Token);
		}

		public override FileLinePositionSpan GetLineSpan(TextSpan span, CancellationToken cancellationToken = default(CancellationToken))
		{
			return new FileLinePositionSpan(FilePath, GetLinePosition(span.Start), GetLinePosition(span.End));
		}

		public override FileLinePositionSpan GetMappedLineSpan(TextSpan span, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (_lineDirectiveMap == null)
			{
				Interlocked.CompareExchange(ref _lineDirectiveMap, new VisualBasicLineDirectiveMap(this), null);
			}
			return _lineDirectiveMap.TranslateSpan(GetText(cancellationToken), FilePath, span);
		}

		public override LineVisibility GetLineVisibility(int position, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (_lineDirectiveMap == null)
			{
				Interlocked.CompareExchange(ref _lineDirectiveMap, new VisualBasicLineDirectiveMap(this), null);
			}
			return _lineDirectiveMap.GetLineVisibility(GetText(cancellationToken), position);
		}

		internal override FileLinePositionSpan GetMappedLineSpanAndVisibility(TextSpan span, ref bool isHiddenPosition)
		{
			if (_lineDirectiveMap == null)
			{
				Interlocked.CompareExchange(ref _lineDirectiveMap, new VisualBasicLineDirectiveMap(this), null);
			}
			return _lineDirectiveMap.TranslateSpanAndVisibility(GetText(), FilePath, span, ref isHiddenPosition);
		}

		public override bool HasHiddenRegions()
		{
			if (_lineDirectiveMap == null)
			{
				Interlocked.CompareExchange(ref _lineDirectiveMap, new VisualBasicLineDirectiveMap(this), null);
			}
			return _lineDirectiveMap.HasAnyHiddenRegions();
		}

		internal ReportDiagnostic GetWarningState(string id, int position)
		{
			if (_lazyWarningStateMap == null)
			{
				Interlocked.CompareExchange(ref _lazyWarningStateMap, new VisualBasicWarningStateMap(this), null);
			}
			return _lazyWarningStateMap.GetWarningState(id, position);
		}

		private LinePosition GetLinePosition(int position)
		{
			return GetText().Lines.GetLinePosition(position);
		}

		public override Location GetLocation(TextSpan span)
		{
			if (EmbeddedSymbolExtensions.IsEmbeddedSyntaxTree(this))
			{
				return new EmbeddedTreeLocation(EmbeddedSymbolExtensions.GetEmbeddedKind(this), span);
			}
			if (IsMyTemplate)
			{
				return new MyTemplateLocation(this, span);
			}
			return new SourceLocation(this, span);
		}

		public override bool IsEquivalentTo(SyntaxTree tree, bool topLevel = false)
		{
			return SyntaxFactory.AreEquivalent(this, tree, topLevel);
		}

		public override IList<TextSpan> GetChangedSpans(SyntaxTree oldTree)
		{
			if (oldTree == null)
			{
				throw new ArgumentNullException("oldTree");
			}
			return SyntaxDiffer.GetPossiblyDifferentTextSpans(oldTree.GetRoot(), GetRoot());
		}

		public override IList<TextChange> GetChanges(SyntaxTree oldTree)
		{
			if (oldTree == null)
			{
				throw new ArgumentNullException("oldTree");
			}
			return SyntaxDiffer.GetTextChanges(oldTree, this);
		}

		protected override SyntaxNode GetRootCore(CancellationToken CancellationToken)
		{
			return GetRoot(CancellationToken);
		}

		[AsyncStateMachine(typeof(VB_0024StateMachine_53_GetRootAsyncCore))]
		protected override Task<SyntaxNode> GetRootAsyncCore(CancellationToken cancellationToken)
		{
			VB_0024StateMachine_53_GetRootAsyncCore stateMachine = default(VB_0024StateMachine_53_GetRootAsyncCore);
			stateMachine._0024VB_0024Me = this;
			stateMachine._0024VB_0024Local_cancellationToken = cancellationToken;
			stateMachine._0024State = -1;
			stateMachine._0024Builder = AsyncTaskMethodBuilder<SyntaxNode>.Create();
			stateMachine._0024Builder.Start(ref stateMachine);
			return stateMachine._0024Builder.Task;
		}

		protected override bool TryGetRootCore(ref SyntaxNode root)
		{
			VisualBasicSyntaxNode root2 = null;
			if (TryGetRoot(ref root2))
			{
				root = root2;
				return true;
			}
			root = null;
			return false;
		}

		internal bool IsAnyPreprocessorSymbolDefined(IEnumerable<string> conditionalSymbolNames, SyntaxNodeOrToken atNode)
		{
			ConditionalSymbolsMap conditionalSymbols = ConditionalSymbols;
			if (conditionalSymbols == null)
			{
				return false;
			}
			foreach (string conditionalSymbolName in conditionalSymbolNames)
			{
				if (conditionalSymbols.IsConditionalSymbolDefined(conditionalSymbolName, atNode))
				{
					return true;
				}
			}
			return false;
		}

		internal VisualBasicPreprocessingSymbolInfo GetPreprocessingSymbolInfo(Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax identifierNode)
		{
			string valueText = identifierNode.Identifier.ValueText;
			return ConditionalSymbols?.GetPreprocessingSymbolInfo(valueText, identifierNode) ?? VisualBasicPreprocessingSymbolInfo.None;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static SyntaxTree ParseText(string text, VisualBasicParseOptions options, string path, Encoding encoding, CancellationToken cancellationToken)
		{
			return ParseText(text, options, path, encoding, null, cancellationToken);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static SyntaxTree ParseText(SourceText text, VisualBasicParseOptions options, string path, CancellationToken cancellationToken)
		{
			return ParseText(text, options, path, null, cancellationToken);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static SyntaxTree Create(VisualBasicSyntaxNode root, VisualBasicParseOptions options, string path, Encoding encoding)
		{
			return Create(root, options, path, encoding, null);
		}
	}
}
