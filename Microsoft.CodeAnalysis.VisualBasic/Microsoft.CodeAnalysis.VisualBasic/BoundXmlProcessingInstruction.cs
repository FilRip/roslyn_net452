using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundXmlProcessingInstruction : BoundExpression
	{
		private readonly BoundExpression _Target;

		private readonly BoundExpression _Data;

		private readonly BoundExpression _ObjectCreation;

		public BoundExpression Target => _Target;

		public BoundExpression Data => _Data;

		public BoundExpression ObjectCreation => _ObjectCreation;

		public BoundXmlProcessingInstruction(SyntaxNode syntax, BoundExpression target, BoundExpression data, BoundExpression objectCreation, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.XmlProcessingInstruction, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(target) || BoundNodeExtensions.NonNullAndHasErrors(data) || BoundNodeExtensions.NonNullAndHasErrors(objectCreation))
		{
			_Target = target;
			_Data = data;
			_ObjectCreation = objectCreation;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitXmlProcessingInstruction(this);
		}

		public BoundXmlProcessingInstruction Update(BoundExpression target, BoundExpression data, BoundExpression objectCreation, TypeSymbol type)
		{
			if (target != Target || data != Data || objectCreation != ObjectCreation || (object)type != base.Type)
			{
				BoundXmlProcessingInstruction boundXmlProcessingInstruction = new BoundXmlProcessingInstruction(base.Syntax, target, data, objectCreation, type, base.HasErrors);
				boundXmlProcessingInstruction.CopyAttributes(this);
				return boundXmlProcessingInstruction;
			}
			return this;
		}
	}
}
