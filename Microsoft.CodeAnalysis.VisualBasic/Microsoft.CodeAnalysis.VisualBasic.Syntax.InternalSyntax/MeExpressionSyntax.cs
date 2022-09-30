using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class MeExpressionSyntax : InstanceExpressionSyntax
	{
		internal static Func<ObjectReader, object> CreateInstance;

		internal MeExpressionSyntax(SyntaxKind kind, KeywordSyntax keyword)
			: base(kind, keyword)
		{
			base._slotCount = 1;
		}

		internal MeExpressionSyntax(SyntaxKind kind, KeywordSyntax keyword, ISyntaxFactoryContext context)
			: base(kind, keyword)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
		}

		internal MeExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax keyword)
			: base(kind, errors, annotations, keyword)
		{
			base._slotCount = 1;
		}

		internal MeExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
		}

		static MeExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new MeExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(MeExpressionSyntax), (ObjectReader r) => new MeExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.MeExpressionSyntax(this, parent, startLocation);
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
			return new MeExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _keyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new MeExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _keyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitMeExpression(this);
		}
	}
}
