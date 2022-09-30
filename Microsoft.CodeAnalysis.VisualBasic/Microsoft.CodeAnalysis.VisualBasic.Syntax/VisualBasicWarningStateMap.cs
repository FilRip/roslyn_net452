using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	internal class VisualBasicWarningStateMap : AbstractWarningStateMap<ReportDiagnostic>
	{
		public VisualBasicWarningStateMap(SyntaxTree tree)
			: base(tree)
		{
		}

		protected override WarningStateMapEntry[] CreateWarningStateMapEntries(SyntaxTree syntaxTree)
		{
			ArrayBuilder<DirectiveTriviaSyntax> instance = ArrayBuilder<DirectiveTriviaSyntax>.GetInstance();
			GetAllWarningDirectives(syntaxTree, instance);
			return CreateWarningStateEntries(instance.ToImmutableAndFree());
		}

		private static void GetAllWarningDirectives(SyntaxTree syntaxTree, ArrayBuilder<DirectiveTriviaSyntax> directiveList)
		{
			foreach (DirectiveTriviaSyntax directive in VisualBasicExtensions.GetDirectives(syntaxTree.GetRoot()))
			{
				if (Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(directive, SyntaxKind.EnableWarningDirectiveTrivia))
				{
					EnableWarningDirectiveTriviaSyntax enableWarningDirectiveTriviaSyntax = (EnableWarningDirectiveTriviaSyntax)directive;
					if (!enableWarningDirectiveTriviaSyntax.EnableKeyword.IsMissing && !enableWarningDirectiveTriviaSyntax.EnableKeyword.ContainsDiagnostics && !enableWarningDirectiveTriviaSyntax.WarningKeyword.IsMissing && !enableWarningDirectiveTriviaSyntax.WarningKeyword.ContainsDiagnostics)
					{
						directiveList.Add(enableWarningDirectiveTriviaSyntax);
					}
				}
				else if (Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(directive, SyntaxKind.DisableWarningDirectiveTrivia))
				{
					DisableWarningDirectiveTriviaSyntax disableWarningDirectiveTriviaSyntax = (DisableWarningDirectiveTriviaSyntax)directive;
					if (!disableWarningDirectiveTriviaSyntax.DisableKeyword.IsMissing && !disableWarningDirectiveTriviaSyntax.DisableKeyword.ContainsDiagnostics && !disableWarningDirectiveTriviaSyntax.WarningKeyword.IsMissing && !disableWarningDirectiveTriviaSyntax.WarningKeyword.ContainsDiagnostics)
					{
						directiveList.Add(disableWarningDirectiveTriviaSyntax);
					}
				}
			}
		}

		private static WarningStateMapEntry[] CreateWarningStateEntries(ImmutableArray<DirectiveTriviaSyntax> directiveList)
		{
			WarningStateMapEntry[] array = new WarningStateMapEntry[directiveList.Length + 1];
			int num = 0;
			array[num] = new WarningStateMapEntry(0, ReportDiagnostic.Default, null);
			ReportDiagnostic general = ReportDiagnostic.Default;
			ImmutableDictionary<string, ReportDiagnostic> immutableDictionary = ImmutableDictionary.Create<string, ReportDiagnostic>(CaseInsensitiveComparison.Comparer);
			ReportDiagnostic reportDiagnostic = default(ReportDiagnostic);
			SeparatedSyntaxList<IdentifierNameSyntax> errorCodes = default(SeparatedSyntaxList<IdentifierNameSyntax>);
			while (num < directiveList.Length)
			{
				DirectiveTriviaSyntax directiveTriviaSyntax = directiveList[num];
				if (Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(directiveTriviaSyntax, SyntaxKind.EnableWarningDirectiveTrivia))
				{
					reportDiagnostic = ReportDiagnostic.Default;
					errorCodes = ((EnableWarningDirectiveTriviaSyntax)directiveTriviaSyntax).ErrorCodes;
				}
				else if (Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(directiveTriviaSyntax, SyntaxKind.DisableWarningDirectiveTrivia))
				{
					reportDiagnostic = ReportDiagnostic.Suppress;
					errorCodes = ((DisableWarningDirectiveTriviaSyntax)directiveTriviaSyntax).ErrorCodes;
				}
				if (errorCodes.Count == 0)
				{
					general = reportDiagnostic;
					immutableDictionary = ImmutableDictionary.Create<string, ReportDiagnostic>(CaseInsensitiveComparison.Comparer);
				}
				else
				{
					int num2 = errorCodes.Count - 1;
					for (int i = 0; i <= num2; i++)
					{
						IdentifierNameSyntax identifierNameSyntax = errorCodes[i];
						if (!identifierNameSyntax.IsMissing && !identifierNameSyntax.ContainsDiagnostics)
						{
							immutableDictionary = immutableDictionary.SetItem(identifierNameSyntax.Identifier.ValueText, reportDiagnostic);
						}
					}
				}
				num++;
				array[num] = new WarningStateMapEntry(directiveTriviaSyntax.GetLocation().SourceSpan.End, general, immutableDictionary);
			}
			return array;
		}
	}
}
