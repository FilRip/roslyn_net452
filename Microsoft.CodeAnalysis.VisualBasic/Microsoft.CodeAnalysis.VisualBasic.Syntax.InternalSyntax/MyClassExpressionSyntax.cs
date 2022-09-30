using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class MyClassExpressionSyntax : InstanceExpressionSyntax
	{
		internal static Func<ObjectReader, object> CreateInstance;

		internal MyClassExpressionSyntax(SyntaxKind kind, KeywordSyntax keyword)
			: base(kind, keyword)
		{
			base._slotCount = 1;
		}

		internal MyClassExpressionSyntax(SyntaxKind kind, KeywordSyntax keyword, ISyntaxFactoryContext context)
			: base(kind, keyword)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
		}

		internal MyClassExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyword)
			: base(kind, errors, annotations, keyword)
		{
			base._slotCount = 1;
		}

		internal MyClassExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
		}

		static MyClassExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new MyClassExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(MyClassExpressionSyntax), (ObjectReader r) => new MyClassExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.MyClassExpressionSyntax(this, parent, startLocation);
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
			return new MyClassExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _keyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new MyClassExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _keyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitMyClassExpression(this);
		}
	}
}
