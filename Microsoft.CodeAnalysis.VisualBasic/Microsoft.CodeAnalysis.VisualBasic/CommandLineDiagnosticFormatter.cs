using System;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class CommandLineDiagnosticFormatter : VisualBasicDiagnosticFormatter
	{
		private readonly string _baseDirectory;

		private readonly Func<ImmutableArray<AdditionalTextFile>> _getAdditionalTextFiles;

		internal CommandLineDiagnosticFormatter(string baseDirectory, Func<ImmutableArray<AdditionalTextFile>> getAdditionalTextFiles)
		{
			_baseDirectory = baseDirectory;
			_getAdditionalTextFiles = getAdditionalTextFiles;
		}

		public override string Format(Diagnostic diagnostic, IFormatProvider formatter = null)
		{
			SourceText text = null;
			TextSpan? diagnosticSpanAndFileText = GetDiagnosticSpanAndFileText(diagnostic, out text);
			if (!diagnosticSpanAndFileText.HasValue || text == null || text.Length < diagnosticSpanAndFileText.Value.End)
			{
				if (diagnostic.Location != Location.None)
				{
					diagnostic = diagnostic.WithLocation(Location.None);
				}
				return "vbc : " + base.Format(diagnostic, formatter);
			}
			string value = base.Format(diagnostic, formatter);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(value);
			TextSpan value2 = diagnosticSpanAndFileText.Value;
			int start = value2.Start;
			int end = value2.End;
			int num = text.Lines.IndexOf(start);
			TextLine textLine = text.Lines[num];
			if (value2.IsEmpty && textLine.Start == end && num > 0)
			{
				num--;
				textLine = text.Lines[num];
			}
			while (textLine.Start < end)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine(textLine.ToString().Replace("\t", "    "));
				int num2 = Math.Min(start, textLine.Start);
				int num3 = Math.Min(textLine.End, start) - 1;
				for (int i = num2; i <= num3; i++)
				{
					if (EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(text[i]), "\t", TextCompare: false) == 0)
					{
						stringBuilder.Append(' ', 4);
					}
					else
					{
						stringBuilder.Append(" ");
					}
				}
				if (value2.IsEmpty)
				{
					stringBuilder.Append("~");
				}
				else
				{
					int num4 = Math.Max(start, textLine.Start);
					int num5 = Math.Min((end == start) ? end : (end - 1), textLine.End - 1);
					for (int j = num4; j <= num5; j++)
					{
						if (EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(text[j]), "\t", TextCompare: false) == 0)
						{
							stringBuilder.Append('~', 4);
						}
						else
						{
							stringBuilder.Append("~");
						}
					}
				}
				int num6 = Math.Min(end, textLine.End);
				int num7 = textLine.End - 1;
				for (int k = num6; k <= num7; k++)
				{
					if (EmbeddedOperators.CompareString(Microsoft.VisualBasic.CompilerServices.Conversions.ToString(text[k]), "\t", TextCompare: false) == 0)
					{
						stringBuilder.Append(' ', 4);
					}
					else
					{
						stringBuilder.Append(" ");
					}
				}
				num++;
				if (num >= text.Lines.Count)
				{
					break;
				}
				textLine = text.Lines[num];
			}
			return stringBuilder.ToString();
		}

		internal override string FormatSourcePath(string path, string basePath, IFormatProvider formatter)
		{
			return FileUtilities.NormalizeRelativePath(path, basePath, _baseDirectory) ?? path;
		}

		private TextSpan? GetDiagnosticSpanAndFileText(Diagnostic diagnostic, out SourceText text)
		{
			TextSpan? result;
			AdditionalTextFile current;
			if (diagnostic.Location.IsInSource)
			{
				text = diagnostic.Location.SourceTree!.GetText();
				result = diagnostic.Location.SourceSpan;
			}
			else
			{
				if (diagnostic.Location.Kind == LocationKind.ExternalFile)
				{
					string path = diagnostic.Location.GetLineSpan().Path;
					if (path != null)
					{
						ImmutableArray<AdditionalTextFile>.Enumerator enumerator = _getAdditionalTextFiles().GetEnumerator();
						while (enumerator.MoveNext())
						{
							current = enumerator.Current;
							if (!path.Equals(current.Path))
							{
								continue;
							}
							goto IL_0097;
						}
					}
				}
				text = null;
				result = null;
			}
			goto IL_00e2;
			IL_00e2:
			return result;
			IL_0097:
			try
			{
				text = current.GetText();
			}
			catch (Exception projectError)
			{
				ProjectData.SetProjectError(projectError);
				text = null;
				ProjectData.ClearProjectError();
			}
			result = diagnostic.Location.SourceSpan;
			goto IL_00e2;
		}
	}
}
