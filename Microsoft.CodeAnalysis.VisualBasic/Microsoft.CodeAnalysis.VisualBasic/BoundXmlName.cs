using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundXmlName : BoundExpression
	{
		private readonly BoundExpression _XmlNamespace;

		private readonly BoundExpression _LocalName;

		private readonly BoundExpression _ObjectCreation;

		public BoundExpression XmlNamespace => _XmlNamespace;

		public BoundExpression LocalName => _LocalName;

		public BoundExpression ObjectCreation => _ObjectCreation;

		public BoundXmlName(SyntaxNode syntax, BoundExpression xmlNamespace, BoundExpression localName, BoundExpression objectCreation, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.XmlName, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(xmlNamespace) || BoundNodeExtensions.NonNullAndHasErrors(localName) || BoundNodeExtensions.NonNullAndHasErrors(objectCreation))
		{
			_XmlNamespace = xmlNamespace;
			_LocalName = localName;
			_ObjectCreation = objectCreation;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitXmlName(this);
		}

		public BoundXmlName Update(BoundExpression xmlNamespace, BoundExpression localName, BoundExpression objectCreation, TypeSymbol type)
		{
			if (xmlNamespace != XmlNamespace || localName != LocalName || objectCreation != ObjectCreation || (object)type != base.Type)
			{
				BoundXmlName boundXmlName = new BoundXmlName(base.Syntax, xmlNamespace, localName, objectCreation, type, base.HasErrors);
				boundXmlName.CopyAttributes(this);
				return boundXmlName;
			}
			return this;
		}
	}
}
