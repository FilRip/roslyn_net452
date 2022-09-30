using System.Globalization;
using System.Threading;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class SourceDocumentationCommentUtils
	{
		internal static string GetAndCacheDocumentationComment(Symbol symbol, CultureInfo preferredCulture, bool expandIncludes, ref string lazyXmlText, CancellationToken cancellationToken)
		{
			if (lazyXmlText == null)
			{
				string documentationCommentForSymbol = GetDocumentationCommentForSymbol(symbol, preferredCulture, expandIncludes, cancellationToken);
				Interlocked.CompareExchange(ref lazyXmlText, documentationCommentForSymbol, null);
			}
			return lazyXmlText;
		}

		internal static string GetDocumentationCommentForSymbol(Symbol symbol, CultureInfo preferredCulture, bool expandIncludes, CancellationToken cancellationToken)
		{
			return VisualBasicCompilation.DocumentationCommentCompiler.GetDocumentationCommentXml(symbol, expandIncludes, preferredCulture, cancellationToken);
		}
	}
}
