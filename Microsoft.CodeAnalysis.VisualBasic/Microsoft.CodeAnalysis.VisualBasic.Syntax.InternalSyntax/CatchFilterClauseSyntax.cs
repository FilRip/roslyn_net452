using System;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class CatchFilterClauseSyntax : VisualBasicSyntaxNode
	{
		internal readonly KeywordSyntax _whenKeyword;

		internal readonly ExpressionSyntax _filter;

		internal static Func<ObjectReader, object> CreateInstance;

		internal KeywordSyntax WhenKeyword => _whenKeyword;

		internal ExpressionSyntax Filter => _filter;

		internal CatchFilterClauseSyntax(SyntaxKind kind, KeywordSyntax whenKeyword, ExpressionSyntax filter)
			: base(kind)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(whenKeyword);
			_whenKeyword = whenKeyword;
			AdjustFlagsAndWidth(filter);
			_filter = filter;
		}

		internal CatchFilterClauseSyntax(SyntaxKind kind, KeywordSyntax whenKeyword, ExpressionSyntax filter, ISyntaxFactoryContext context)
			: base(kind)
		{
			base._slotCount = 2;
			SetFactoryContext(context);
			AdjustFlagsAndWidth(whenKeyword);
			_whenKeyword = whenKeyword;
			AdjustFlagsAndWidth(filter);
			_filter = filter;
		}

		internal CatchFilterClauseSyntax(SyntaxKind kind, DiagnosticInfo[] errors, SyntaxAnnotation[] annotations, KeywordSyntax whenKeyword, ExpressionSyntax filter)
			: base(kind, errors, annotations)
		{
			base._slotCount = 2;
			AdjustFlagsAndWidth(whenKeyword);
			_whenKeyword = whenKeyword;
			AdjustFlagsAndWidth(filter);
			_filter = filter;
		}

		internal CatchFilterClauseSyntax(ObjectReader reader)
			: base(reader)
		{
			base._slotCount = 2;
			KeywordSyntax keywordSyntax = (KeywordSyntax)reader.ReadValue();
			if (keywordSyntax != null)
			{
				AdjustFlagsAndWidth(keywordSyntax);
				_whenKeyword = keywordSyntax;
			}
			ExpressionSyntax expressionSyntax = (ExpressionSyntax)reader.ReadValue();
			if (expressionSyntax != null)
			{
				AdjustFlagsAndWidth(expressionSyntax);
				_filter = expressionSyntax;
			}
		}

		internal override void WriteTo(ObjectWriter writer)
		{
			base.WriteTo(writer);
			writer.WriteValue(_whenKeyword);
			writer.WriteValue(_filter);
		}

		static CatchFilterClauseSyntax()
		{
			CreateInstance = (ObjectReader o) => new CatchFilterClauseSyntax(o);
			ObjectBinder.RegisterTypeReader(typeof(CatchFilterClauseSyntax), (ObjectReader r) => new CatchFilterClauseSyntax(r));
		}

		internal override SyntaxNode CreateRed(SyntaxNode parent, int startLocation)
		{
			return new Microsoft.CodeAnalysis.VisualBasic.Syntax.CatchFilterClauseSyntax(this, parent, startLocation);
		}

		internal override GreenNode GetSlot(int i)
		{
			return i switch
			{
				0 => _whenKeyword, 
				1 => _filter, 
				_ => null, 
			};
		}

		internal override GreenNode SetDiagnostics(DiagnosticInfo[] newErrors)
		{
			return new CatchFilterClauseSyntax(base.Kind, newErrors, GetAnnotations(), _whenKeyword, _filter);
		}

		internal override GreenNode SetAnnotations(SyntaxAnnotation[] annotations)
		{
			return new CatchFilterClauseSyntax(base.Kind, GetDiagnostics(), annotations, _whenKeyword, _filter);
		}

		public override VisualBasicSyntaxNode Accept(VisualBasicSyntaxVisitor visitor)
		{
			return visitor.VisitCatchFilterClause(this);
		}
	}
}
