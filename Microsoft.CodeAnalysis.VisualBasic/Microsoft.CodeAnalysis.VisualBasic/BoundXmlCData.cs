using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundXmlCData : BoundExpression
	{
		private readonly BoundLiteral _Value;

		private readonly BoundExpression _ObjectCreation;

		public BoundLiteral Value => _Value;

		public BoundExpression ObjectCreation => _ObjectCreation;

		public BoundXmlCData(SyntaxNode syntax, BoundLiteral value, BoundExpression objectCreation, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.XmlCData, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(value) || BoundNodeExtensions.NonNullAndHasErrors(objectCreation))
		{
			_Value = value;
			_ObjectCreation = objectCreation;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitXmlCData(this);
		}

		public BoundXmlCData Update(BoundLiteral value, BoundExpression objectCreation, TypeSymbol type)
		{
			if (value != Value || objectCreation != ObjectCreation || (object)type != base.Type)
			{
				BoundXmlCData boundXmlCData = new BoundXmlCData(base.Syntax, value, objectCreation, type, base.HasErrors);
				boundXmlCData.CopyAttributes(this);
				return boundXmlCData;
			}
			return this;
		}
	}
}
