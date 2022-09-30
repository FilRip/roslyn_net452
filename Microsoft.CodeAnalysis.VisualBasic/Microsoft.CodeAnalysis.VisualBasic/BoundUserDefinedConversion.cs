using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundUserDefinedConversion : BoundExpression
	{
		private readonly BoundExpression _UnderlyingExpression;

		private readonly byte _InOutConversionFlags;

		public BoundExpression Operand
		{
			get
			{
				if (((uint)InOutConversionFlags & (true ? 1u : 0u)) != 0)
				{
					return ((BoundConversion)Call.Arguments[0]).Operand;
				}
				return Call.Arguments[0];
			}
		}

		public BoundConversion InConversionOpt
		{
			get
			{
				if (((uint)InOutConversionFlags & (true ? 1u : 0u)) != 0)
				{
					return (BoundConversion)Call.Arguments[0];
				}
				return null;
			}
		}

		public BoundConversion OutConversionOpt
		{
			get
			{
				if ((InOutConversionFlags & 2u) != 0)
				{
					return (BoundConversion)UnderlyingExpression;
				}
				return null;
			}
		}

		public BoundCall Call
		{
			get
			{
				if ((InOutConversionFlags & 2u) != 0)
				{
					return (BoundCall)((BoundConversion)UnderlyingExpression).Operand;
				}
				return (BoundCall)UnderlyingExpression;
			}
		}

		public BoundExpression UnderlyingExpression => _UnderlyingExpression;

		public byte InOutConversionFlags => _InOutConversionFlags;

		public BoundUserDefinedConversion(SyntaxNode syntax, BoundExpression underlyingExpression, byte inOutConversionFlags, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.UserDefinedConversion, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(underlyingExpression))
		{
			_UnderlyingExpression = underlyingExpression;
			_InOutConversionFlags = inOutConversionFlags;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitUserDefinedConversion(this);
		}

		public BoundUserDefinedConversion Update(BoundExpression underlyingExpression, byte inOutConversionFlags, TypeSymbol type)
		{
			if (underlyingExpression != UnderlyingExpression || inOutConversionFlags != InOutConversionFlags || (object)type != base.Type)
			{
				BoundUserDefinedConversion boundUserDefinedConversion = new BoundUserDefinedConversion(base.Syntax, underlyingExpression, inOutConversionFlags, type, base.HasErrors);
				boundUserDefinedConversion.CopyAttributes(this);
				return boundUserDefinedConversion;
			}
			return this;
		}
	}
}
