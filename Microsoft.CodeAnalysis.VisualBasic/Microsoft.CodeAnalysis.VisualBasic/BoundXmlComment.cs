using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundXmlComment : BoundExpression
	{
		private readonly BoundExpression _Value;

		private readonly BoundExpression _ObjectCreation;

		public BoundExpression Value => _Value;

		public BoundExpression ObjectCreation => _ObjectCreation;

		public BoundXmlComment(SyntaxNode syntax, BoundExpression value, BoundExpression objectCreation, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.XmlComment, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(value) || BoundNodeExtensions.NonNullAndHasErrors(objectCreation))
		{
			_Value = value;
			_ObjectCreation = objectCreation;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitXmlComment(this);
		}

		public BoundXmlComment Update(BoundExpression value, BoundExpression objectCreation, TypeSymbol type)
		{
			if (value != Value || objectCreation != ObjectCreation || (object)type != base.Type)
			{
				BoundXmlComment boundXmlComment = new BoundXmlComment(base.Syntax, value, objectCreation, type, base.HasErrors);
				boundXmlComment.CopyAttributes(this);
				return boundXmlComment;
			}
			return this;
		}
	}
}
