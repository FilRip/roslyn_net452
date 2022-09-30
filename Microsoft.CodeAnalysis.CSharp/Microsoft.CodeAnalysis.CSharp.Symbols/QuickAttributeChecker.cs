using System;
using System.Collections.Generic;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class QuickAttributeChecker
    {
        private readonly Dictionary<string, QuickAttributes> _nameToAttributeMap;

        private static QuickAttributeChecker _lazyPredefinedQuickAttributeChecker;

        internal static QuickAttributeChecker Predefined
        {
            get
            {
                if (_lazyPredefinedQuickAttributeChecker == null)
                {
                    Interlocked.CompareExchange(ref _lazyPredefinedQuickAttributeChecker, CreatePredefinedQuickAttributeChecker(), null);
                }
                return _lazyPredefinedQuickAttributeChecker;
            }
        }

        private static QuickAttributeChecker CreatePredefinedQuickAttributeChecker()
        {
            QuickAttributeChecker quickAttributeChecker = new QuickAttributeChecker();
            quickAttributeChecker.AddName(AttributeDescription.TypeIdentifierAttribute.Name, QuickAttributes.TypeIdentifier);
            quickAttributeChecker.AddName(AttributeDescription.TypeForwardedToAttribute.Name, QuickAttributes.TypeForwardedTo);
            quickAttributeChecker.AddName(AttributeDescription.AssemblyKeyNameAttribute.Name, QuickAttributes.AssemblyKeyName);
            quickAttributeChecker.AddName(AttributeDescription.AssemblyKeyFileAttribute.Name, QuickAttributes.AssemblyKeyFile);
            quickAttributeChecker.AddName(AttributeDescription.AssemblySignatureKeyAttribute.Name, QuickAttributes.AssemblySignatureKey);
            return quickAttributeChecker;
        }

        private QuickAttributeChecker()
        {
            _nameToAttributeMap = new Dictionary<string, QuickAttributes>(StringComparer.Ordinal);
        }

        private QuickAttributeChecker(QuickAttributeChecker previous)
        {
            _nameToAttributeMap = new Dictionary<string, QuickAttributes>(previous._nameToAttributeMap, StringComparer.Ordinal);
        }

        private void AddName(string name, QuickAttributes newAttributes)
        {
            QuickAttributes value = QuickAttributes.None;
            _nameToAttributeMap.TryGetValue(name, out value);
            QuickAttributes value2 = newAttributes | value;
            _nameToAttributeMap[name] = value2;
        }

        internal QuickAttributeChecker AddAliasesIfAny(SyntaxList<UsingDirectiveSyntax> usingsSyntax, bool onlyGlobalAliases = false)
        {
            if (usingsSyntax.Count == 0)
            {
                return this;
            }
            QuickAttributeChecker quickAttributeChecker = null;
            SyntaxList<UsingDirectiveSyntax>.Enumerator enumerator = usingsSyntax.GetEnumerator();
            while (enumerator.MoveNext())
            {
                UsingDirectiveSyntax current = enumerator.Current;
                if (current.Alias != null && (!onlyGlobalAliases || current.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword)))
                {
                    string valueText = current.Alias!.Name.Identifier.ValueText;
                    string valueText2 = current.Name.GetUnqualifiedName().Identifier.ValueText;
                    if (_nameToAttributeMap.TryGetValue(valueText2, out var value))
                    {
                        (quickAttributeChecker ?? (quickAttributeChecker = new QuickAttributeChecker(this))).AddName(valueText, value);
                    }
                }
            }
            if (quickAttributeChecker != null)
            {
                return quickAttributeChecker;
            }
            return this;
        }

        public bool IsPossibleMatch(AttributeSyntax attr, QuickAttributes pattern)
        {
            string valueText = attr.Name.GetUnqualifiedName().Identifier.ValueText;
            if (_nameToAttributeMap.TryGetValue(valueText, out var value) || _nameToAttributeMap.TryGetValue(valueText + "Attribute", out value))
            {
                return (value & pattern) != 0;
            }
            return false;
        }
    }
}
