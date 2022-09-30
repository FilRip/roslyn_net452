using System;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal class TypeHelpers
	{
		private TypeHelpers()
		{
		}

		internal static long UncheckedCLng(CConst v)
		{
			SpecialType specialType = v.SpecialType;
			if (specialType.IsIntegralType())
			{
				return Microsoft.VisualBasic.CompilerServices.Conversions.ToLong(v.ValueAsObject);
			}
			return specialType switch
			{
				SpecialType.System_Char => (int)Microsoft.VisualBasic.CompilerServices.Conversions.ToChar(v.ValueAsObject), 
				SpecialType.System_DateTime => Microsoft.VisualBasic.CompilerServices.Conversions.ToDate(v.ValueAsObject).ToBinary(), 
				_ => throw ExceptionUtilities.UnexpectedValue(specialType), 
			};
		}

		internal static bool VarDecAdd(decimal pdecLeft, decimal pdecRight, ref decimal pdecResult)
		{
			try
			{
				pdecResult = decimal.Add(pdecLeft, pdecRight);
			}
			catch (OverflowException ex)
			{
				ProjectData.SetProjectError(ex);
				OverflowException ex2 = ex;
				bool result = true;
				ProjectData.ClearProjectError();
				return result;
			}
			return false;
		}

		internal static bool VarDecSub(decimal pdecLeft, decimal pdecRight, ref decimal pdecResult)
		{
			try
			{
				pdecResult = decimal.Subtract(pdecLeft, pdecRight);
			}
			catch (OverflowException ex)
			{
				ProjectData.SetProjectError(ex);
				OverflowException ex2 = ex;
				bool result = true;
				ProjectData.ClearProjectError();
				return result;
			}
			return false;
		}

		internal static bool VarDecMul(decimal pdecLeft, decimal pdecRight, ref decimal pdecResult)
		{
			try
			{
				pdecResult = decimal.Multiply(pdecLeft, pdecRight);
			}
			catch (OverflowException ex)
			{
				ProjectData.SetProjectError(ex);
				OverflowException ex2 = ex;
				bool result = true;
				ProjectData.ClearProjectError();
				return result;
			}
			return false;
		}

		internal static bool VarDecDiv(decimal pdecLeft, decimal pdecRight, ref decimal pdecResult)
		{
			try
			{
				pdecResult = decimal.Divide(pdecLeft, pdecRight);
			}
			catch (OverflowException ex)
			{
				ProjectData.SetProjectError(ex);
				OverflowException ex2 = ex;
				bool result = true;
				ProjectData.ClearProjectError();
				return result;
			}
			return false;
		}
	}
}
