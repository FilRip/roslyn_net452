using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundXmlElement : BoundExpression
	{
		private readonly BoundExpression _Argument;

		private readonly ImmutableArray<BoundExpression> _ChildNodes;

		private readonly BoundXmlContainerRewriterInfo _RewriterInfo;

		public BoundExpression Argument => _Argument;

		public ImmutableArray<BoundExpression> ChildNodes => _ChildNodes;

		public BoundXmlContainerRewriterInfo RewriterInfo => _RewriterInfo;

		public BoundXmlElement(SyntaxNode syntax, BoundExpression argument, ImmutableArray<BoundExpression> childNodes, BoundXmlContainerRewriterInfo rewriterInfo, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.XmlElement, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(argument) || BoundNodeExtensions.NonNullAndHasErrors(childNodes))
		{
			_Argument = argument;
			_ChildNodes = childNodes;
			_RewriterInfo = rewriterInfo;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitXmlElement(this);
		}

		public BoundXmlElement Update(BoundExpression argument, ImmutableArray<BoundExpression> childNodes, BoundXmlContainerRewriterInfo rewriterInfo, TypeSymbol type)
		{
			if (argument != Argument || childNodes != ChildNodes || rewriterInfo != RewriterInfo || (object)type != base.Type)
			{
				BoundXmlElement boundXmlElement = new BoundXmlElement(base.Syntax, argument, childNodes, rewriterInfo, type, base.HasErrors);
				boundXmlElement.CopyAttributes(this);
				return boundXmlElement;
			}
			return this;
		}
	}
}
