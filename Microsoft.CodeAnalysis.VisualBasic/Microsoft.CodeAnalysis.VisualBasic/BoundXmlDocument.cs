using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundXmlDocument : BoundExpression
	{
		private readonly BoundExpression _Declaration;

		private readonly ImmutableArray<BoundExpression> _ChildNodes;

		private readonly BoundXmlContainerRewriterInfo _RewriterInfo;

		public BoundExpression Declaration => _Declaration;

		public ImmutableArray<BoundExpression> ChildNodes => _ChildNodes;

		public BoundXmlContainerRewriterInfo RewriterInfo => _RewriterInfo;

		public BoundXmlDocument(SyntaxNode syntax, BoundExpression declaration, ImmutableArray<BoundExpression> childNodes, BoundXmlContainerRewriterInfo rewriterInfo, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.XmlDocument, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(declaration) || BoundNodeExtensions.NonNullAndHasErrors(childNodes))
		{
			_Declaration = declaration;
			_ChildNodes = childNodes;
			_RewriterInfo = rewriterInfo;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitXmlDocument(this);
		}

		public BoundXmlDocument Update(BoundExpression declaration, ImmutableArray<BoundExpression> childNodes, BoundXmlContainerRewriterInfo rewriterInfo, TypeSymbol type)
		{
			if (declaration != Declaration || childNodes != ChildNodes || rewriterInfo != RewriterInfo || (object)type != base.Type)
			{
				BoundXmlDocument boundXmlDocument = new BoundXmlDocument(base.Syntax, declaration, childNodes, rewriterInfo, type, base.HasErrors);
				boundXmlDocument.CopyAttributes(this);
				return boundXmlDocument;
			}
			return this;
		}
	}
}
