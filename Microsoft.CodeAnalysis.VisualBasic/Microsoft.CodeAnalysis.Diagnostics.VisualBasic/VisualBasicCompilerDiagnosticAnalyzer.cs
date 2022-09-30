using System;
using System.Collections;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.Diagnostics.VisualBasic
{
	[DiagnosticAnalyzer("Visual Basic", new string[] { })]
	internal class VisualBasicCompilerDiagnosticAnalyzer : CompilerDiagnosticAnalyzer
	{
		internal override CommonMessageProvider MessageProvider => Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance;

		internal override ImmutableArray<int> GetSupportedErrorCodes()
		{
			Array values = Enum.GetValues(typeof(ERRID));
			ImmutableArray<int>.Builder builder = ImmutableArray.CreateBuilder<int>();
			IEnumerator enumerator = default(IEnumerator);
			try
			{
				enumerator = values.GetEnumerator();
				while (enumerator.MoveNext())
				{
					int num = Microsoft.VisualBasic.CompilerServices.Conversions.ToInteger(enumerator.Current);
					if (num != 31091 && num != 35000 && num != 36597 && num > 0 && num < 42600)
					{
						builder.Add(num);
					}
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			return builder.ToImmutable();
		}
	}
}
