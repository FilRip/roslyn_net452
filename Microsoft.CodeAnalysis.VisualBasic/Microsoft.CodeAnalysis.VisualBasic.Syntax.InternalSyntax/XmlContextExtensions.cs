using System.Collections.Generic;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	[StandardModule]
	internal sealed class XmlContextExtensions
	{
		internal static void Push(this List<XmlContext> @this, XmlContext context)
		{
			@this.Add(context);
		}

		internal static XmlContext Pop(this List<XmlContext> @this)
		{
			int index = @this.Count - 1;
			XmlContext result = @this[index];
			@this.RemoveAt(index);
			return result;
		}

		internal static XmlContext Peek(this List<XmlContext> @this, int i = 0)
		{
			int num = @this.Count - 1;
			return @this[num - i];
		}

		internal static int MatchEndElement(this List<XmlContext> @this, XmlNameSyntax name)
		{
			int num = @this.Count - 1;
			if (name == null)
			{
				return num;
			}
			int num2;
			for (num2 = num; num2 >= 0; num2--)
			{
				XmlNodeSyntax name2 = @this[num2].StartElement.Name;
				if (name2.Kind == SyntaxKind.XmlName)
				{
					XmlNameSyntax xmlNameSyntax = (XmlNameSyntax)name2;
					if (EmbeddedOperators.CompareString(xmlNameSyntax.LocalName.Text, name.LocalName.Text, TextCompare: false) == 0)
					{
						XmlPrefixSyntax prefix = xmlNameSyntax.Prefix;
						XmlPrefixSyntax prefix2 = name.Prefix;
						if (prefix == prefix2 || (prefix != null && prefix2 != null && EmbeddedOperators.CompareString(prefix.Name.Text, prefix2.Name.Text, TextCompare: false) == 0))
						{
							break;
						}
					}
				}
			}
			return num2;
		}
	}
}
