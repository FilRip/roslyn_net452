using System.Collections.Generic;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class UsingInfo
	{
		public readonly Dictionary<TypeSymbol, (BoundRValuePlaceholder, BoundExpression, BoundExpression)> PlaceholderInfo;

		public readonly UsingBlockSyntax UsingStatementSyntax;

		public UsingInfo(UsingBlockSyntax usingStatementSyntax, Dictionary<TypeSymbol, (BoundRValuePlaceholder, BoundExpression, BoundExpression)> placeholderInfo)
		{
			PlaceholderInfo = placeholderInfo;
			UsingStatementSyntax = usingStatementSyntax;
		}
	}
}
