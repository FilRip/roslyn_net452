// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System;
using System.Collections.Generic;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    /// <summary>
    /// The QuickAttributeChecker applies a simple fast heuristic for determining probable
    /// attributes of certain kinds without binding attribute types, just by looking at the final syntax of an
    /// attribute usage.
    /// </summary>
    /// <remarks>
    /// It works by maintaining a dictionary of all possible simple names that might map to the given
    /// attribute.
    /// </remarks>
    internal sealed class QuickAttributeChecker
    {
        private readonly Dictionary<string, QuickAttributes> _nameToAttributeMap;
        private static QuickAttributeChecker _lazyPredefinedQuickAttributeChecker;

#if DEBUG
#pragma warning disable CS0414, IDE0052
        private bool _sealed;
#pragma warning restore CS0414, IDE0052
#endif

        internal static QuickAttributeChecker Predefined
        {
            get
            {
                if (_lazyPredefinedQuickAttributeChecker is null)
                {
                    Interlocked.CompareExchange(ref _lazyPredefinedQuickAttributeChecker, CreatePredefinedQuickAttributeChecker(), null);
                }

                return _lazyPredefinedQuickAttributeChecker;
            }
        }

        private static QuickAttributeChecker CreatePredefinedQuickAttributeChecker()
        {
            var result = new QuickAttributeChecker();
            result.AddName(AttributeDescription.TypeIdentifierAttribute.Name, QuickAttributes.TypeIdentifier);
            result.AddName(AttributeDescription.TypeForwardedToAttribute.Name, QuickAttributes.TypeForwardedTo);
            result.AddName(AttributeDescription.AssemblyKeyNameAttribute.Name, QuickAttributes.AssemblyKeyName);
            result.AddName(AttributeDescription.AssemblyKeyFileAttribute.Name, QuickAttributes.AssemblyKeyFile);
            result.AddName(AttributeDescription.AssemblySignatureKeyAttribute.Name, QuickAttributes.AssemblySignatureKey);

#if DEBUG
            result._sealed = true;
#endif
            return result;
        }

        private QuickAttributeChecker()
        {
            _nameToAttributeMap = new Dictionary<string, QuickAttributes>(StringComparer.Ordinal);
            // NOTE: caller must seal
        }

        private QuickAttributeChecker(QuickAttributeChecker previous)
        {
            _nameToAttributeMap = new Dictionary<string, QuickAttributes>(previous._nameToAttributeMap, StringComparer.Ordinal);
            // NOTE: caller must seal
        }

        private void AddName(string name, QuickAttributes newAttributes)
        {
#if DEBUG
#endif
            var currentValue = QuickAttributes.None;
            _nameToAttributeMap.TryGetValue(name, out currentValue);

            QuickAttributes newValue = newAttributes | currentValue;
            _nameToAttributeMap[name] = newValue;
        }

        internal QuickAttributeChecker AddAliasesIfAny(SyntaxList<UsingDirectiveSyntax> usingsSyntax, bool onlyGlobalAliases = false)
        {
            if (usingsSyntax.Count == 0)
            {
                return this;
            }

            QuickAttributeChecker newChecker = null;

            foreach (var usingDirective in usingsSyntax)
            {
                if (usingDirective.Alias != null && (!onlyGlobalAliases || usingDirective.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword)))
                {
                    string name = usingDirective.Alias.Name.Identifier.ValueText;
                    string target = usingDirective.Name.GetUnqualifiedName().Identifier.ValueText;

                    if (_nameToAttributeMap.TryGetValue(target, out var foundAttributes))
                    {
                        // copy the QuickAttributes from alias target to alias name
                        (newChecker ??= new QuickAttributeChecker(this)).AddName(name, foundAttributes);
                    }
                }
            }

            if (newChecker != null)
            {
#if DEBUG
                newChecker._sealed = true;
#endif
                return newChecker;
            }

            return this;
        }

        public bool IsPossibleMatch(AttributeSyntax attr, QuickAttributes pattern)
        {
#if DEBUG
#endif
            string name = attr.Name.GetUnqualifiedName().Identifier.ValueText;

            // We allow "Name" to bind to "NameAttribute"
            if (_nameToAttributeMap.TryGetValue(name, out QuickAttributes foundAttributes) ||
                _nameToAttributeMap.TryGetValue(name + "Attribute", out foundAttributes))
            {
                return (foundAttributes & pattern) != 0;
            }

            return false;
        }
    }

    [Flags()]
    internal enum QuickAttributes : byte
    {
        None = 0,
        TypeIdentifier = 1 << 0,
        TypeForwardedTo = 1 << 1,
        AssemblyKeyName = 1 << 2,
        AssemblyKeyFile = 1 << 3,
        AssemblySignatureKey = 1 << 4,
    }
}
