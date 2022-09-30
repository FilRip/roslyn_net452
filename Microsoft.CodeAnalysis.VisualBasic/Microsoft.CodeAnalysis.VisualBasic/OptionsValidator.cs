using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class OptionsValidator
	{
		internal static GlobalImport[] ParseImports(IEnumerable<string> importsClauses, DiagnosticBag diagnostics)
		{
			string[] array = importsClauses.Select(StringExtensions.Unquote).ToArray();
			if (array.Length > 0)
			{
				SyntaxTree tree = VisualBasicSyntaxTree.ParseText(SourceText.From(array.Select((string name) => "Imports " + name + "\r\n\r\n").Aggregate((string a, string b) => a + b)), VisualBasicParseOptions.Default);
				List<GlobalImport> list = new List<GlobalImport>();
				SyntaxList<ImportsStatementSyntax> imports = VisualBasicExtensions.GetCompilationUnitRoot(tree).Imports;
				int num = imports.Count - 1;
				_Closure_0024__0_002D0 closure_0024__0_002D = default(_Closure_0024__0_002D0);
				for (int i = 0; i <= num; i++)
				{
					SeparatedSyntaxList<ImportsClauseSyntax> importsClauses2 = imports[i].ImportsClauses;
					if (importsClauses.Count() > 0)
					{
						closure_0024__0_002D = new _Closure_0024__0_002D0(closure_0024__0_002D);
						ImportsClauseSyntax importsClauseSyntax = importsClauses2[0];
						IEnumerable<Diagnostic> source = importsClauseSyntax.GetSyntaxErrors(tree);
						if (importsClauses2.Count > 1)
						{
							source = source.Concat(new VBDiagnostic(new DiagnosticInfo(MessageProvider.Instance, 30205), importsClauses2[1].GetLocation()));
						}
						closure_0024__0_002D._0024VB_0024Local_import = new GlobalImport(importsClauseSyntax, array[i]);
						IEnumerable<Diagnostic> enumerable = source.Select(closure_0024__0_002D._Lambda_0024__2);
						diagnostics.AddRange(enumerable);
						if (!enumerable.Any((Diagnostic diag) => diag.Severity == DiagnosticSeverity.Error))
						{
							list.Add(closure_0024__0_002D._0024VB_0024Local_import);
						}
					}
				}
				return list.ToArray();
			}
			return Array.Empty<GlobalImport>();
		}

		internal static bool IsValidNamespaceName(string name)
		{
			int num = 0;
			while (true)
			{
				int num2 = name.IndexOf('.', num);
				if (!IsValidRootNamespaceComponent(name, num, (num2 < 0) ? name.Length : num2, allowEscaping: true))
				{
					return false;
				}
				if (num2 < 0)
				{
					break;
				}
				num = num2 + 1;
			}
			return true;
		}

		private static bool IsValidRootNamespaceComponent(string name, int start, int end, bool allowEscaping)
		{
			if (start == end)
			{
				return false;
			}
			int num = end - 1;
			if (allowEscaping && SyntaxFacts.ReturnFullWidthOrSelf(name[start]) == '［')
			{
				if (SyntaxFacts.ReturnFullWidthOrSelf(name[num]) != '］')
				{
					return false;
				}
				return IsValidRootNamespaceComponent(name, start + 1, num, allowEscaping: false);
			}
			if (!SyntaxFacts.IsIdentifierStartCharacter(name[start]))
			{
				return false;
			}
			if (end - start == 1 && SyntaxFacts.ReturnFullWidthOrSelf(name[start]) == '\uff3f')
			{
				return false;
			}
			int num2 = start + 1;
			int num3 = num;
			for (int i = num2; i <= num3; i++)
			{
				if (!SyntaxFacts.IsIdentifierPartCharacter(name[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}
