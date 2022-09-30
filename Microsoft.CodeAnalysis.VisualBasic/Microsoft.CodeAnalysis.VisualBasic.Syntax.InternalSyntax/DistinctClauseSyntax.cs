using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class DistinctClauseSyntax : QueryClauseSyntax
	{
		internal readonly KeywordSyntax _distinctKeyword;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax DistinctKeyword => _distinctKeyword;

		internal DistinctClauseSyntax(SyntaxKind kind, KeywordSyntax distinctKeyword)
			: base(kind)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(distinctKeyword);
			_distinctKeyword = distinctKeyword;
		}

		internal DistinctClauseSyntax(SyntaxKind kind, KeywordSyntax distinctKeyword, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 1;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(distinctKeyword);
			_distinctKeyword = distinctKeyword;
		}

		internal DistinctClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax distinctKeyword)
			: base(kind, errors, annotations)
		{
			base._slotCount = 1;
			AdjustFlagsAndWidth(distinctKeyword);
			_distinctKeyword = distinctKeyword;
		}

		internal DistinctClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 1;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_distinctKeyword = keywordSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_distinctKeyword);
		}

		static DistinctClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new DistinctClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(DistinctClauseSyntax), (ObjectReader r) => new DistinctClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.DistinctClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			if (i == 0)
			{
				return _distinctKeyword;
			}
			return null;
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new DistinctClauseSyntax(base.Kind, newErrors, GetAnnotations(), _distinctKeyword);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new DistinctClauseSyntax(base.Kind, GetDiagnostics(), annotations, _distinctKeyword);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitDistinctClause(this);
		}
	}
}
