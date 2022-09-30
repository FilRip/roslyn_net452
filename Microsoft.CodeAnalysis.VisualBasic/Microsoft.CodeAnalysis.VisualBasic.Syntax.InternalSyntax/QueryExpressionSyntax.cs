using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class QueryExpressionSyntax : ExpressionSyntax
	{
		internal readonly GreenNode _clauses;

		internal static Func<ObjectReader, object> CreateInstance;

		internal Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<QueryClauseSyntax> Clauses => new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<QueryClauseSyntax>(_clauses);

		internal QueryExpressionSyntax(SyntaxKind kind, GreenNode clauses)
			: base(kind)
		{
			base._slotCount = 1;
			if (clauses != null)
			{
				AdjustFlagsAndWidth(clauses);
				_clauses = clauses;
			}
		}

		internal QueryExpressionSyntax(SyntaxKind kind, GreenNode clauses, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			if (clauses != null)
			{
				AdjustFlagsAndWidth(clauses);
				_clauses = clauses;
			}
		}

		internal QueryExpressionSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, GreenNode clauses)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			if (clauses != null)
			{
				AdjustFlagsAndWidth(clauses);
				_clauses = clauses;
			}
		}

		internal QueryExpressionSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			GreenNode greenNode = (GreenNode)reader.ReadValue();
			if (greenNode != null)
			{
				AdjustFlagsAndWidth(greenNode);
				_clauses = greenNode;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_clauses);
		}

		static QueryExpressionSyntax()
		{
			CreateInstance = (ObjectReader o) => new QueryExpressionSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(QueryExpressionSyntax), (ObjectReader r) => new QueryExpressionSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.QueryExpressionSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _clauses;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new QueryExpressionSyntax(base.Kind, newErrors, GetAnnotations(), _clauses);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new QueryExpressionSyntax(base.Kind, GetDiagnostics(), annotations, _clauses);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitQueryExpression(this);
		}
	}
}
