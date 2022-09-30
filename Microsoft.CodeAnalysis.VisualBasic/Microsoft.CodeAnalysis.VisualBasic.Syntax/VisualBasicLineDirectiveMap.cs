using System;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	internal class VisualBasicLineDirectiveMap : LineDirectiveMap<DirectiveTriviaSyntax>
	{
		public VisualBasicLineDirectiveMap(SyntaxTree tree)
			: base(tree)
		{
		}

		protected override bool ShouldAddDirective(DirectiveTriviaSyntax directive)
		{
			if (directive.Kind() != SyntaxKind.ExternalSourceDirectiveTrivia)
			{
				return directive.Kind() == SyntaxKind.EndExternalSourceDirectiveTrivia;
			}
			return true;
		}

		protected override LineMappingEntry GetEntry(DirectiveTriviaSyntax directive, SourceText sourceText, LineMappingEntry previous)
		{
			int num = sourceText.Lines.IndexOf(directive.SpanStart) + 1;
			int num2 = num;
			int mappedLine = previous.MappedLine + num - previous.UnmappedLine;
			string mappedPathOpt = previous.MappedPathOpt;
			PositionState state = default(PositionState);
			if (directive.Kind() == SyntaxKind.ExternalSourceDirectiveTrivia)
			{
				ExternalSourceDirectiveTriviaSyntax externalSourceDirectiveTriviaSyntax = (ExternalSourceDirectiveTriviaSyntax)directive;
				if (!externalSourceDirectiveTriviaSyntax.LineStart.IsMissing && !externalSourceDirectiveTriviaSyntax.ExternalSource.IsMissing)
				{
					mappedLine = (int)(Math.Min(Microsoft.VisualBasic.CompilerServices.Conversions.ToLong(externalSourceDirectiveTriviaSyntax.LineStart.Value), 2147483647L) - 1);
					mappedPathOpt = Microsoft.VisualBasic.CompilerServices.Conversions.ToString(externalSourceDirectiveTriviaSyntax.ExternalSource.Value);
				}
				state = (PositionState)((previous.State != 0) ? PositionState.RemappedAfterHidden : PositionState.RemappedAfterUnknown);
			}
			else if (directive.Kind() == SyntaxKind.EndExternalSourceDirectiveTrivia)
			{
				mappedLine = num2;
				mappedPathOpt = null;
				state = (PositionState)((num2 > previous.UnmappedLine + 1 && ((uint)previous.State == 4u || (uint)previous.State == 3u)) ? PositionState.Hidden : (((uint)previous.State == 4u) ? PositionState.Hidden : PositionState.Unknown));
			}
			return new LineMappingEntry(num2, mappedLine, mappedPathOpt, state);
		}

		protected override LineMappingEntry InitializeFirstEntry()
		{
			return new LineMappingEntry(0, 0, (string?)null, PositionState.Unknown);
		}

		public override LineVisibility GetLineVisibility(SourceText sourceText, int position)
		{
			int index = FindEntryIndex(sourceText.Lines.GetLinePosition(position).Line);
			return GetLineVisibility(index);
		}

		private LineVisibility GetLineVisibility(int index)
		{
			LineMappingEntry lineMappingEntry = Entries[index];
			if ((uint)lineMappingEntry.State == 0u)
			{
				if (Entries.Length < index + 3)
				{
					return LineVisibility.Visible;
				}
				if ((uint)Entries[index + 1].State == 0u)
				{
					return GetLineVisibility(index + 1);
				}
				int result;
				switch (Entries[index + 2].State)
				{
				case PositionState.Unknown:
					return GetLineVisibility(index + 2);
				default:
					result = 2;
					break;
				case PositionState.Hidden:
					result = 1;
					break;
				}
				return (LineVisibility)result;
			}
			return ((uint)lineMappingEntry.State == 5u) ? LineVisibility.Hidden : LineVisibility.Visible;
		}

		internal override FileLinePositionSpan TranslateSpanAndVisibility(SourceText sourceText, string treeFilePath, TextSpan span, ref bool isHiddenPosition)
		{
			LinePosition linePosition = sourceText.Lines.GetLinePosition(span.Start);
			LinePosition linePosition2 = sourceText.Lines.GetLinePosition(span.End);
			int num = FindEntryIndex(linePosition.Line);
			isHiddenPosition = GetLineVisibility(num) == LineVisibility.Hidden;
			LineMappingEntry entry = Entries[num];
			return TranslateSpan(entry, treeFilePath, linePosition, linePosition2);
		}
	}
}
