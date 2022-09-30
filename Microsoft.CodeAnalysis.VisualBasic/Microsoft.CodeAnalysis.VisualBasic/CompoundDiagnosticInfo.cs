using System;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class CompoundDiagnosticInfo : DiagnosticInfo
	{
		internal CompoundDiagnosticInfo(DiagnosticInfo[] arguments)
			: base(Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance, 0, arguments)
		{
		}

		public override string GetMessage(IFormatProvider formatProvider = null)
		{
			PooledStringBuilder instance = PooledStringBuilder.GetInstance();
			if (base.Arguments != null)
			{
				object[] arguments = base.Arguments;
				for (int i = 0; i < arguments.Length; i = checked(i + 1))
				{
					DiagnosticInfo diagnosticInfo = (DiagnosticInfo)arguments[i];
					instance.Builder.Append(diagnosticInfo.GetMessage(formatProvider));
				}
			}
			string result = instance.Builder.ToString();
			instance.Free();
			return result;
		}
	}
}
