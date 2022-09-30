using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundXmlNamespace : BoundExpression
	{
		private readonly BoundExpression _XmlNamespace;

		private readonly BoundExpression _ObjectCreation;

		public BoundExpression XmlNamespace => _XmlNamespace;

		public BoundExpression ObjectCreation => _ObjectCreation;

		public BoundXmlNamespace(SyntaxNode syntax, BoundExpression xmlNamespace, BoundExpression objectCreation, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.XmlNamespace, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(xmlNamespace) || BoundNodeExtensions.NonNullAndHasErrors(objectCreation))
		{
			_XmlNamespace = xmlNamespace;
			_ObjectCreation = objectCreation;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitXmlNamespace(this);
		}

		public BoundXmlNamespace Update(BoundExpression xmlNamespace, BoundExpression objectCreation, TypeSymbol type)
		{
			if (xmlNamespace != XmlNamespace || objectCreation != ObjectCreation || (object)type != base.Type)
			{
				BoundXmlNamespace boundXmlNamespace = new BoundXmlNamespace(base.Syntax, xmlNamespace, objectCreation, type, base.HasErrors);
				boundXmlNamespace.CopyAttributes(this);
				return boundXmlNamespace;
			}
			return this;
		}
	}
}
