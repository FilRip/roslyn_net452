using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis
{
    public sealed class AdditionalSourcesCollection
    {
        private readonly ArrayBuilder<GeneratedSourceText> _sourcesAdded;

        private readonly string _fileExtension;

        private const StringComparison _hintNameComparison = StringComparison.OrdinalIgnoreCase;

        private static readonly StringComparer s_hintNameComparer = StringComparer.OrdinalIgnoreCase;

        public AdditionalSourcesCollection(string fileExtension)
        {
            _sourcesAdded = ArrayBuilder<GeneratedSourceText>.GetInstance();
            _fileExtension = fileExtension;
        }

        public void Add(string hintName, SourceText source)
        {
            if (string.IsNullOrWhiteSpace(hintName))
            {
                throw new ArgumentNullException("hintName");
            }
            for (int i = 0; i < hintName.Length; i++)
            {
                char c = hintName[i];
                if (!UnicodeCharacterUtilities.IsIdentifierPartCharacter(c) && c != '.' && c != ',' && c != '-' && c != '_' && c != ' ' && c != '(' && c != ')' && c != '[' && c != ']' && c != '{' && c != '}')
                {
                    throw new ArgumentException(string.Format(CodeAnalysisResources.HintNameInvalidChar, c, i), "hintName");
                }
            }
            hintName = AppendExtensionIfRequired(hintName);
            if (Contains(hintName))
            {
                throw new ArgumentException(CodeAnalysisResources.HintNameUniquePerGenerator, "hintName");
            }
            if (source.Encoding == null)
            {
                throw new ArgumentException(CodeAnalysisResources.SourceTextRequiresEncoding, "source");
            }
            _sourcesAdded.Add(new GeneratedSourceText(hintName, source));
        }

        public void AddRange(ImmutableArray<GeneratedSourceText> texts)
        {
            _sourcesAdded.AddRange(texts);
        }

        public void AddRange(ImmutableArray<GeneratedSyntaxTree> trees)
        {
            _sourcesAdded.AddRange(trees.SelectAsArray((GeneratedSyntaxTree t) => new GeneratedSourceText(t.HintName, t.Text)));
        }

        public void RemoveSource(string hintName)
        {
            hintName = AppendExtensionIfRequired(hintName);
            for (int i = 0; i < _sourcesAdded.Count; i++)
            {
                if (s_hintNameComparer.Equals(_sourcesAdded[i].HintName, hintName))
                {
                    _sourcesAdded.RemoveAt(i);
                    break;
                }
            }
        }

        public bool Contains(string hintName)
        {
            hintName = AppendExtensionIfRequired(hintName);
            for (int i = 0; i < _sourcesAdded.Count; i++)
            {
                if (s_hintNameComparer.Equals(_sourcesAdded[i].HintName, hintName))
                {
                    return true;
                }
            }
            return false;
        }

        internal ImmutableArray<GeneratedSourceText> ToImmutableAndFree()
        {
            return _sourcesAdded.ToImmutableAndFree();
        }

        private string AppendExtensionIfRequired(string hintName)
        {
            if (!hintName.EndsWith(_fileExtension, StringComparison.OrdinalIgnoreCase))
            {
                hintName += _fileExtension;
            }
            return hintName;
        }
    }
}
