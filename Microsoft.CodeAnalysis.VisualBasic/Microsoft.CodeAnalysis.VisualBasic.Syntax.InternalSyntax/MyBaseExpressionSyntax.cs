using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class MyBaseExpressionSyntax : InstanceExpressionSyntax
	{
		internal static Func<ObjectReader, object> CreateInstance;

		internal MyBaseExpressionSyntax(SyntaxKind kind, KeywordSyntax keyword)
			: base(kind, keyword)
		{
			base._slotCount = 1;
		}

		internal MyBaseExpressionSyntax(SyntaxKind kind, KeywordSyntax keyword, ISyntaxFactoryContext context)
			: base(kind, keyword)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
		}

		internal MyBaseExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyword)
			: base(kind, errors, annotations, keyword)
		{
			base._slotCount = 1;
		}

		internal MyBaseExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
		}

		static MyBaseExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new MyBaseExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(MyBaseExpressionSyntax), (ObjectReader r) => new MyBaseExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.MyBaseExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _keyword;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new MyBaseExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _keyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new MyBaseExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _keyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitMyBaseExpression(this);
		}
	}
}
