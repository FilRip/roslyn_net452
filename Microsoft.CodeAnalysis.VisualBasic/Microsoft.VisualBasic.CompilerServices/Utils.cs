using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualBasic.CompilerServices
{
	[Embedded]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class Utils
	{
		private Utils()
		{
		}

		public static Array CopyArray(Array arySrc, Array aryDest)
		{
			if (arySrc == null)
			{
				return aryDest;
			}
			int length = arySrc.Length;
			if (length == 0)
			{
				return aryDest;
			}
			if (aryDest.Rank != arySrc.Rank)
			{
				throw new InvalidCastException();
			}
			int num = aryDest.Rank - 2;
			for (int i = 0; i <= num; i++)
			{
				if (aryDest.GetUpperBound(i) != arySrc.GetUpperBound(i))
				{
					throw new ArrayTypeMismatchException();
				}
			}
			if (length > aryDest.Length)
			{
				length = aryDest.Length;
			}
			if (arySrc.Rank > 1)
			{
				int rank = arySrc.Rank;
				int length2 = arySrc.GetLength(rank - 1);
				int length3 = aryDest.GetLength(rank - 1);
				if (length3 == 0)
				{
					return aryDest;
				}
				int length4 = ((length2 > length3) ? length3 : length2);
				int num2 = arySrc.Length / length2 - 1;
				for (int j = 0; j <= num2; j++)
				{
					Array.Copy(arySrc, j * length2, aryDest, j * length3, length4);
				}
			}
			else
			{
				Array.Copy(arySrc, aryDest, length);
			}
			return aryDest;
		}
	}
}
