using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class QuickAttributeChecker
	{
		private readonly Dictionary<string, QuickAttributes> _nameToAttributeMap;

		private bool _sealed;

		public QuickAttributeChecker()
		{
			_nameToAttributeMap = new Dictionary<string, QuickAttributes>(CaseInsensitiveComparison.Comparer);
		}

		public QuickAttributeChecker(QuickAttributeChecker other)
		{
			_nameToAttributeMap = new Dictionary<string, QuickAttributes>(other._nameToAttributeMap, CaseInsensitiveComparison.Comparer);
		}

		public void AddName(string name, QuickAttributes newAttributes)
		{
			QuickAttributes value = QuickAttributes.None;
			_nameToAttributeMap.TryGetValue(name, out value);
			_nameToAttributeMap[name] = newAttributes | value;
			if (name.EndsWith("Attribute", StringComparison.OrdinalIgnoreCase))
			{
				_nameToAttributeMap[name.Substring(0, name.Length - "Attribute".Length)] = newAttributes | value;
			}
		}

		public void AddAlias(SimpleImportsClauseSyntax aliasSyntax)
		{
			string finalName = GetFinalName(aliasSyntax.Name);
			if (finalName != null)
			{
				QuickAttributes value = QuickAttributes.None;
				if (_nameToAttributeMap.TryGetValue(finalName, out value))
				{
					AddName(aliasSyntax.Alias.Identifier.ValueText, value);
				}
			}
		}

		public void Seal()
		{
			_sealed = true;
		}

		public QuickAttributes CheckAttributes(SyntaxList<AttributeListSyntax> attributeLists)
		{
			QuickAttributes quickAttributes = QuickAttributes.None;
			if (attributeLists.Count > 0)
			{
				SyntaxList<AttributeListSyntax>.Enumerator enumerator = attributeLists.GetEnumerator();
				while (enumerator.MoveNext())
				{
					SeparatedSyntaxList<AttributeSyntax>.Enumerator enumerator2 = enumerator.Current.Attributes.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						AttributeSyntax current = enumerator2.Current;
						quickAttributes |= CheckAttribute(current);
					}
				}
			}
			return quickAttributes;
		}

		public QuickAttributes CheckAttribute(AttributeSyntax attr)
		{
			TypeSyntax name = attr.Name;
			string finalName = GetFinalName(name);
			if (finalName != null && _nameToAttributeMap.TryGetValue(finalName, out var value))
			{
				return value;
			}
			return QuickAttributes.None;
		}

		private string GetFinalName(TypeSyntax typeSyntax)
		{
			VisualBasicSyntaxNode visualBasicSyntaxNode = typeSyntax;
			while (true)
			{
				switch (visualBasicSyntaxNode.Kind())
				{
				case SyntaxKind.IdentifierName:
					return ((IdentifierNameSyntax)visualBasicSyntaxNode).Identifier.ValueText;
				case SyntaxKind.QualifiedName:
					break;
				default:
					return null;
				}
				visualBasicSyntaxNode = ((QualifiedNameSyntax)visualBasicSyntaxNode).Right;
			}
		}
	}
}
