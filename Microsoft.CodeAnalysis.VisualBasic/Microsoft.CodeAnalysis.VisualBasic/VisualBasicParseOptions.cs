using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	public sealed class VisualBasicParseOptions : ParseOptions, IEquatable<VisualBasicParseOptions>
	{
		private static ImmutableArray<KeyValuePair<string, object>> s_defaultPreprocessorSymbols;

		private ImmutableDictionary<string, string> _features;

		private ImmutableArray<KeyValuePair<string, object>> _preprocessorSymbols;

		private LanguageVersion _specifiedLanguageVersion;

		private LanguageVersion _languageVersion;

		public static VisualBasicParseOptions Default { get; } = new VisualBasicParseOptions();


		public override string Language => "Visual Basic";

		private static ImmutableArray<KeyValuePair<string, object>> DefaultPreprocessorSymbols
		{
			get
			{
				if (s_defaultPreprocessorSymbols.IsDefaultOrEmpty)
				{
					s_defaultPreprocessorSymbols = ImmutableArray.Create(KeyValuePairUtil.Create("_MYTYPE", (object)"Empty"));
				}
				return s_defaultPreprocessorSymbols;
			}
		}

		public LanguageVersion SpecifiedLanguageVersion => _specifiedLanguageVersion;

		public LanguageVersion LanguageVersion => _languageVersion;

		public ImmutableArray<KeyValuePair<string, object>> PreprocessorSymbols => _preprocessorSymbols;

		public override IEnumerable<string> PreprocessorSymbolNames => _preprocessorSymbols.Select((KeyValuePair<string, object> ps) => ps.Key);

		public override IReadOnlyDictionary<string, string> Features => _features;

		public VisualBasicParseOptions(LanguageVersion languageVersion = LanguageVersion.Default, DocumentationMode documentationMode = DocumentationMode.Parse, SourceCodeKind kind = SourceCodeKind.Regular, IEnumerable<KeyValuePair<string, object>> preprocessorSymbols = null)
			: this(languageVersion, documentationMode, kind, (preprocessorSymbols == null) ? DefaultPreprocessorSymbols : ImmutableArray.CreateRange(preprocessorSymbols), ImmutableDictionary<string, string>.Empty)
		{
		}

		internal VisualBasicParseOptions(LanguageVersion languageVersion, DocumentationMode documentationMode, SourceCodeKind kind, ImmutableArray<KeyValuePair<string, object>> preprocessorSymbols, ImmutableDictionary<string, string> features)
			: base(kind, documentationMode)
		{
			_specifiedLanguageVersion = languageVersion;
			_languageVersion = LanguageVersionFacts.MapSpecifiedToEffectiveVersion(languageVersion);
			_preprocessorSymbols = preprocessorSymbols.ToImmutableArrayOrEmpty();
			_features = features ?? ImmutableDictionary<string, string>.Empty;
		}

		private VisualBasicParseOptions(VisualBasicParseOptions other)
			: this(other._specifiedLanguageVersion, other.DocumentationMode, other.Kind, other._preprocessorSymbols, other._features)
		{
		}

		public VisualBasicParseOptions WithLanguageVersion(LanguageVersion version)
		{
			if (version == _specifiedLanguageVersion)
			{
				return this;
			}
			LanguageVersion languageVersion = LanguageVersionFacts.MapSpecifiedToEffectiveVersion(version);
			return new VisualBasicParseOptions(this)
			{
				_specifiedLanguageVersion = version,
				_languageVersion = languageVersion
			};
		}

		public new VisualBasicParseOptions WithKind(SourceCodeKind kind)
		{
			if (kind == base.SpecifiedKind)
			{
				return this;
			}
			SourceCodeKind kind2 = kind.MapSpecifiedToEffectiveKind();
			return new VisualBasicParseOptions(this)
			{
				SpecifiedKind = kind,
				Kind = kind2
			};
		}

		public new VisualBasicParseOptions WithDocumentationMode(DocumentationMode documentationMode)
		{
			if (documentationMode == base.DocumentationMode)
			{
				return this;
			}
			return new VisualBasicParseOptions(this)
			{
				DocumentationMode = documentationMode
			};
		}

		public VisualBasicParseOptions WithPreprocessorSymbols(IEnumerable<KeyValuePair<string, object>> symbols)
		{
			return WithPreprocessorSymbols(symbols.AsImmutableOrNull());
		}

		public VisualBasicParseOptions WithPreprocessorSymbols(params KeyValuePair<string, object>[] symbols)
		{
			return WithPreprocessorSymbols(symbols.AsImmutableOrNull());
		}

		public VisualBasicParseOptions WithPreprocessorSymbols(ImmutableArray<KeyValuePair<string, object>> symbols)
		{
			if (symbols.IsDefault)
			{
				symbols = ImmutableArray<KeyValuePair<string, object>>.Empty;
			}
			if (symbols.Equals(PreprocessorSymbols))
			{
				return this;
			}
			return new VisualBasicParseOptions(this)
			{
				_preprocessorSymbols = symbols
			};
		}

		public override ParseOptions CommonWithKind(SourceCodeKind kind)
		{
			return WithKind(kind);
		}

		protected override ParseOptions CommonWithDocumentationMode(DocumentationMode documentationMode)
		{
			return WithDocumentationMode(documentationMode);
		}

		protected override ParseOptions CommonWithFeatures(IEnumerable<KeyValuePair<string, string>> features)
		{
			return WithFeatures(features);
		}

		public new VisualBasicParseOptions WithFeatures(IEnumerable<KeyValuePair<string, string>> features)
		{
			if (features == null)
			{
				return new VisualBasicParseOptions(this)
				{
					_features = ImmutableDictionary<string, string>.Empty
				};
			}
			return new VisualBasicParseOptions(this)
			{
				_features = features.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase)
			};
		}

		internal override void ValidateOptions(ArrayBuilder<Diagnostic> builder)
		{
			ValidateOptions(builder, MessageProvider.Instance);
			if (!LanguageVersionEnumBounds.IsValid(LanguageVersion))
			{
				builder.Add(Diagnostic.Create(MessageProvider.Instance, 37287, LanguageVersion.ToString()));
			}
			if (PreprocessorSymbols.IsDefaultOrEmpty)
			{
				return;
			}
			ImmutableArray<KeyValuePair<string, object>>.Enumerator enumerator = PreprocessorSymbols.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, object> current = enumerator.Current;
				if (!SyntaxFacts.IsValidIdentifier(current.Key) || SyntaxFacts.GetKeywordKind(current.Key) != 0)
				{
					builder.Add(Diagnostic.Create(ErrorFactory.ErrorInfo(ERRID.ERR_ConditionalCompilationConstantNotValid, ErrorFactory.ErrorInfo(ERRID.ERR_ExpectedIdentifier), current.Key)));
				}
				if (CConst.TryCreate(RuntimeHelpers.GetObjectValue(current.Value)) == null)
				{
					builder.Add(Diagnostic.Create(MessageProvider.Instance, 37288, current.Key, current.Value.GetType()));
				}
			}
		}

		public bool Equals(VisualBasicParseOptions other)
		{
			if ((object)this == other)
			{
				return true;
			}
			if (!EqualsHelper(other))
			{
				return false;
			}
			if (SpecifiedLanguageVersion != other.SpecifiedLanguageVersion)
			{
				return false;
			}
			if (!PreprocessorSymbols.SequenceEqual(other.PreprocessorSymbols))
			{
				return false;
			}
			return true;
		}

		bool IEquatable<VisualBasicParseOptions>.Equals(VisualBasicParseOptions other)
		{
			//ILSpy generated this explicit interface implementation from .override directive in Equals
			return this.Equals(other);
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as VisualBasicParseOptions);
		}

		public override int GetHashCode()
		{
			return Hash.Combine(GetHashCodeHelper(), (int)SpecifiedLanguageVersion);
		}
	}
}
